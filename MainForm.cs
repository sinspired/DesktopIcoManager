using Microsoft.Win32;
using System.Text.Json;

namespace DesktopICO
{
    public partial class MainForm : Form
    {
        private const int MaxPathDisplayLength = 50;
        private const string LayoutPrefix = "布局";

        // 获取保存目录路径
        private string LayoutsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "DesktopIcoLayouts");

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
            UpdateAutoStartButtonText();
        }

        #region 事件处理程序

        private async void MainForm_Load(object sender, EventArgs e)
        {
            UpdateRunningStatus("加载布局中...");
            await LoadSavedLayoutsAsync();
            UpdateRunningStatus("加载完成");
        }

        /// <summary>
        /// 同步加载所有布局
        /// </summary>
        private async Task LoadSavedLayoutsAsync()
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                Directory.CreateDirectory(LayoutsPath);
                var files = Directory.GetFiles(LayoutsPath, "*.json");

                if (LayoutsPath.Length > MaxPathDisplayLength)
                {
                    SavePathLabel.Text = $"路径长度超过{MaxPathDisplayLength - 3}，显示前{MaxPathDisplayLength - 3}个字符...";

                    // 创建ToolTip以在鼠标悬停时显示完整路径
                    ToolTip toolTip = new();
                    SavePathLabel.MouseHover += (sender, e) =>
                    {
                        toolTip.Show(LayoutsPath, SavePathLabel);
                    };
                    SavePathLabel.MouseLeave += (sender, e) =>
                    {
                        toolTip.Hide(SavePathLabel);
                    };
                }
                else
                {
                    SavePathLabel.Text = $"保存目录：{LayoutsPath}";
                }

                var layoutItems = new List<(ListViewItem Item, DateTime LastWrite)>();
                ListViewItem? latestItem = null;
                DateTime latestDate = DateTime.MinValue;

                await Task.Run(() =>
                {
                    foreach (var file in files)
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                        var parts = fileNameWithoutExtension.Split('_');
                        string displayName = $"{parts[0]}[{parts[1]}]";
                        if (parts[0] == "Layout")
                        {
                            displayName = parts.Length >= 3 ? $"{LayoutPrefix}[{parts[1]}]" : fileNameWithoutExtension;
                        }

                        string userName = parts.Length >= 3 ? parts[2] : "";

                        string dateString = $"{parts[3]}{parts[4]}{parts[5]}";
                        DateTime lastWrite = DateTime.ParseExact(dateString, "yyyyMMddHHmmss", null);
                        var item = new ListViewItem(displayName) { Tag = file };
                        item.SubItems.Add(userName);
                        item.SubItems.Add(lastWrite.ToString("yyyy-MM-dd HH:mm"));
                        layoutItems.Add((item, lastWrite));
                    }

                    // 按时间倒序排序，最新的在最后面
                    layoutItems.Sort((x, y) => x.LastWrite.CompareTo(y.LastWrite));

                    // 获取最新项（现在就是最后一项）
                    var newest = layoutItems.LastOrDefault();
                    if (newest.Item != null)
                    {
                        newest.Item.ForeColor = Color.Green;
                        latestItem = newest.Item;
                    }
                });

                this.Invoke(() =>
                {
                    LayoutsListView.BeginUpdate();
                    LayoutsListView.Items.Clear();
                    LayoutsListView.Items.AddRange(layoutItems.Select(x => x.Item).ToArray());
                    LayoutsListView.EndUpdate();

                    if (latestItem != null)
                    {
                        latestItem.Selected = true;
                        LayoutsListView.EnsureVisible(latestItem.Index);
                    }
                });
            }, "加载布局时出错");
        }

        private async Task SaveLayoutToFileAsync(bool autoBackup = false)
        {
            SaveButton.Enabled = false;
            try
            {
                await ExecuteWithErrorHandlingAsync(async () =>
                {
                    var desktopManager = new DesktopIcoManager();
                    string layoutData = desktopManager.SaveLayout();

                    Directory.CreateDirectory(LayoutsPath);
                    string fileName = LayoutFileHelper.CreateFileName("Layout", autoBackup);
                    string filePath = Path.Combine(LayoutsPath, fileName + ".json");

                    if (!File.Exists(filePath))
                    {
                        UpdateRunningStatus("正在保存布局...");
                        await File.WriteAllTextAsync(filePath, layoutData);

                        if (!autoBackup)
                        {
                            await LoadSavedLayoutsAsync();
                        }
                        else
                        {
                            var fileInfo = LayoutFileHelper.ParseFileName(fileName);
                            var item = new ListViewItem(LayoutFileHelper.CreateDisplayName(fileInfo.Prefix, fileInfo.Resolution))
                            {
                                Tag = filePath,
                                ForeColor = Color.RoyalBlue
                            };
                            item.SubItems.Add(fileInfo.UserName);
                            item.SubItems.Add(fileInfo.Timestamp.ToString("yyyy-MM-dd HH:mm"));
                            LayoutsListView.Items.Add(item);
                        }
                    }
                    UpdateRunningStatus("布局保存成功");
                }, "保存布局时出错");
            }
            finally
            {
                SaveButton.Enabled = true;
            }
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            UpdateRunningStatus("正在更新布局列表...");
            await SaveLayoutToFileAsync();
        }

        private async Task BackupCurrentLayoutAsync()
        {
            UpdateRunningStatus("正在备份布局列表...");
            await SaveLayoutToFileAsync(true);
        }

        // 恢复桌面布局
        private async void RestoreButton_Click(object sender, EventArgs e)
        {
            string? filePath = GetSelectedLayoutFilePath();
            if (filePath == null)
                return;

            var fileInfo = LayoutFileHelper.ParseFileName(Path.GetFileName(filePath));
            var latestTime = GetLatestLayoutTimestamp();

            // 如果当前选择的不是最新的布局，并且距离最新布局超过1小时，则进行备份
            if (latestTime.HasValue &&
                fileInfo.Timestamp <= latestTime.Value &&
                (DateTime.Now - latestTime.Value).TotalHours > 1)
            {
                UpdateRunningStatus("将自动进行布局备份");
                await BackupCurrentLayoutAsync();
            }

            await ExecuteWithErrorHandlingAsync(async () =>
            {
                string layoutData = await File.ReadAllTextAsync(filePath);
                var desktopManager = new DesktopIcoManager();
                desktopManager.RestoreLayout(layoutData);
                UpdateRunningStatus("布局恢复成功！");
            }, "恢复布局时出错");
        }

        private DateTime? GetLatestLayoutTimestamp()
        {
            var files = Directory.GetFiles(LayoutsPath, "*.json");
            if (files.Length == 1)
            {
                
                var fileInfo = LayoutFileHelper.ParseFileName(Path.GetFileName(files[0]));
                return fileInfo.Timestamp;
            }

            DateTime? latestTime = null;
            foreach (var file in files)
            {
                try
                {
                    var fileInfo = LayoutFileHelper.ParseFileName(Path.GetFileName(file));
                    if (!latestTime.HasValue || fileInfo.Timestamp > latestTime)
                    {
                        latestTime = fileInfo.Timestamp;
                    }
                }
                catch (ArgumentException)
                {
                    // 跳过无效的文件名格式
                    continue;
                }
            }
            return latestTime;
        }

        // 重命名布局优化
        private void RenameButton_Click(object sender, EventArgs e)
        {
            string? oldFilePath = GetSelectedLayoutFilePath();
            if (oldFilePath == null)
                return;

            var fileInfo = LayoutFileHelper.ParseFileName(Path.GetFileName(oldFilePath));
            using var renameDialog = new InputDialog(fileInfo.Prefix);

            if (renameDialog.ShowDialog() == DialogResult.OK)
            {
                string newName = renameDialog.InputTextBox.Text.Trim();
                string directoryName = Path.GetDirectoryName(oldFilePath) ?? throw new Exception("无法获取文件目录路径");

                string newFileName = $"{newName}_{fileInfo.Resolution}_{fileInfo.UserName}_{fileInfo.Timestamp:yyyy_MMdd_HHmmss}";
                string newFilePath = Path.Combine(directoryName, newFileName + ".json");

                ExecuteWithErrorHandling(() =>
                {
                    File.Move(oldFilePath, newFilePath);
                    var selectedItem = LayoutsListView.SelectedItems[0];
                    selectedItem.Text = LayoutFileHelper.CreateDisplayName(newName, fileInfo.Resolution);
                    selectedItem.Tag = newFilePath;
                    UpdateRunningStatus("重命名成功");
                }, "重命名布局时出错");
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string? filePath = GetSelectedLayoutFilePath();
            if (filePath == null)
                return;

            if (MessageBox.Show("确定要删除选中的布局吗", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ExecuteWithErrorHandling(() =>
                {
                    File.Delete(filePath);
                    LayoutsListView.Items.Remove(LayoutsListView.SelectedItems[0]);
                    UpdateRunningStatus("布局删除成功");
                }, "删除布局时出错");
            }
        }
        private void LayoutsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 获取当前列的排序顺序
            SortOrder sortOrder = LayoutsListView.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            LayoutsListView.Sorting = sortOrder;

            // 设置排序器
            LayoutsListView.ListViewItemSorter = new ListViewItemComparer(e.Column, sortOrder);
            LayoutsListView.Sort();
        }

        // 自定义排序器
        private class ListViewItemComparer : System.Collections.IComparer
        {
            private readonly int _column;
            private readonly SortOrder _order;

            public ListViewItemComparer(int column, SortOrder order)
            {
                _column = column;
                _order = order;
            }

            public int Compare(object? x, object? y)
            {
                if (x is ListViewItem itemX && y is ListViewItem itemY)
                {
                    int result;
                    if (_column == 2) // 创建时间列
                    {
                        DateTime dateX = DateTime.ParseExact(itemX.SubItems[_column].Text, "yyyy-MM-dd HH:mm", null);
                        DateTime dateY = DateTime.ParseExact(itemY.SubItems[_column].Text, "yyyy-MM-dd HH:mm", null);
                        result = DateTime.Compare(dateX, dateY);
                    }
                    else
                    {
                        result = string.Compare(itemX.SubItems[_column].Text, itemY.SubItems[_column].Text, StringComparison.Ordinal);
                    }

                    return _order == SortOrder.Ascending ? result : -result;
                }
                return 0;
            }
        }

        private void AutoStartButton_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key != null)
                {
                    if (key.GetValue("DesktopLayoutManager") != null)
                    {
                        key.DeleteValue("DesktopLayoutManager");
                        AutoStartButton.Text = "设置开机自启";
                        UpdateRunningStatus("已取消开机自启");
                    }
                    else
                    {
                        key.SetValue("DesktopLayoutManager", Application.ExecutablePath);
                        AutoStartButton.Text = "取消开机自启";
                        UpdateRunningStatus("已设置开机自启");
                    }
                }
            }, "设置开机自启时出错");
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "这是一个用于备份和恢复桌面文件和图标位置的工具。\n\n" +
                "使用方法\n" +
                "1. 点击“保存桌面布局”按钮保存当前桌面文件和图标位置”\n" +
                "2. 选择一个布局，然后点击“恢复桌面图标布局”按钮，即可恢复桌面布局（无需重启）\n" +
                "3. 支持重命名与删除已保存的布局",
                "桌面布局管理器 v1.0",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void LayoutsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }
        private async void ShowSavedLayoutDetails(object sender, EventArgs e)
        {
            string? filePath = GetSelectedLayoutFilePath();
            if (filePath == null)
            {
                return;
            }
            else if (!File.Exists(filePath))
            {
                LayoutsListView.Items.Remove(LayoutsListView.SelectedItems[0]);
                UpdateRunningStatus("布局文件不存在，已删除无效项目");
                return;
            }

            await ExecuteWithErrorHandlingAsync(async () =>
            {
                try
                {
                    string layoutData = await File.ReadAllTextAsync(filePath);
                    var icons = JsonSerializer.Deserialize<List<IconData>>(layoutData);

                    if (icons != null)
                    {
                        int totalIcons = icons.Count;
                        UpdateRunningStatus($"总图标数量: {totalIcons}");
                    }
                    else
                    {
                        UpdateRunningStatus("无法解析布局数据");
                    }
                }
                catch (FileNotFoundException)
                {
                    UpdateRunningStatus("读取布局图标数量失败");
                }
            }, "加载布局详情时出错");
        }
        #endregion

        #region 辅助方法

        private void UpdateRunningStatus(string message)
        {
            StatusLabel.Text = message;
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = LayoutsListView.SelectedItems.Count > 0;
            RestoreButton.Enabled = hasSelection;
            DeleteButton.Enabled = hasSelection;
            RenameButton.Enabled = hasSelection;
        }

        private void UpdateAutoStartButtonText()
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (key != null)
            {
                AutoStartButton.Text = key.GetValue("DesktopLayoutManager") != null ? "取消开机自启" : "设置开机自启";
            }
        }

        private void ExecuteWithErrorHandling(Action action, string errorMessage)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ShowError(errorMessage, ex);
            }
        }

        private async Task ExecuteWithErrorHandlingAsync(Func<Task> asyncAction, string errorMessage)
        {
            try
            {
                await asyncAction();
            }
            catch (Exception ex)
            {
                ShowError(errorMessage, ex);
            }
        }

        private string? GetSelectedLayoutFilePath()
        {
            if (LayoutsListView.SelectedItems.Count == 0)
            {
                UpdateRunningStatus("未选择任何布局文件");
                return null;
            }
            string? filePath = LayoutsListView.SelectedItems[0].Tag?.ToString();
            return filePath;
        }

        private static void ShowError(string message, Exception ex)
        {
            MessageBox.Show(
                $"{message}\n\n错误详情：{ex.Message}",
                "错误",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private class IconData
        {
            public required string Name { get; set; }
            public required Position Position { get; set; }
        }

        private class Position
        {
            public bool IsEmpty { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }
        private void OpenLayoutsFoder(object sender, EventArgs e)
        {
            // 打开桌面布局文件存储目录
            //string argument = $"/select, \"{LayoutsPath}\"";
            System.Diagnostics.Process.Start("explorer.exe", LayoutsPath);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

    }
}
