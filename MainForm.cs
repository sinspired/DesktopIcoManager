using Microsoft.Win32;
using System.Text;
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

                var layoutItems = new List<ListViewItem>();

                await Task.Run(() =>
                {
                    foreach (var file in files)
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                        var parts = fileNameWithoutExtension.Split('_');
                        string displayName = parts.Length >= 3 ? $"{LayoutPrefix}[{parts[1]}]" : fileNameWithoutExtension;
                        string userName = parts.Length >= 3 ? parts[2] : "";
                        DateTime lastWrite = File.GetLastWriteTime(file);

                        var item = new ListViewItem(displayName) { Tag = file };
                        item.SubItems.Add(userName);
                        item.SubItems.Add(lastWrite.ToString("yyyy-MM-dd HH:mm"));

                        layoutItems.Add(item);
                    }
                });

                this.Invoke(() =>
                {
                    LayoutsListView.BeginUpdate();
                    LayoutsListView.Items.Clear();
                    LayoutsListView.Items.AddRange([.. layoutItems]);
                    LayoutsListView.EndUpdate();
                });
            }, "加载布局时出错");
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            // 禁用保存按钮防止重复点击
            SaveButton.Enabled = false;
            UpdateRunningStatus("正在更新布局列表...");
            try
            {
                await ExecuteWithErrorHandlingAsync(async () =>
                {
                    var desktopManager = new DesktopIcoManager();
                    string layoutData = desktopManager.SaveLayout();

                    Directory.CreateDirectory(LayoutsPath);
                    string userName = Environment.UserName;
                    string timestamp = DateTime.Now.ToString("yyyy_MMdd_HHmmss");
                    string currentYear = DateTime.Now.ToString("yyyy");

                    // 获取屏幕分辨率
                    string screenResolution = $"{Screen.PrimaryScreen?.Bounds.Width}x{Screen.PrimaryScreen?.Bounds.Height}";

                    // 以 LayoutPrefix 作为前缀
                    string fileName = $"Layout_{screenResolution}_{userName}_{timestamp}";
                    string filePath = Path.Combine(LayoutsPath, fileName + ".json");



                    // 检查是否存在同名文件
                    if (!File.Exists(filePath))
                    {
                        UpdateRunningStatus("正在保存布局...");
                        // 写入文件，await确保写入完成
                        await File.WriteAllTextAsync(filePath, layoutData);
                        
                        // 保存成功后，更新UI：添加布局项目
                        string displayName = $"{LayoutPrefix}[{screenResolution}]";
                        var item = new ListViewItem(displayName) { Tag = filePath };

                        item.SubItems.Add(userName);
                        item.SubItems.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        LayoutsListView.Items.Add(item);
                    }
                        UpdateRunningStatus("布局保存成功");
                }, "保存布局时出错");
            }
            finally
            {
                // 操作完成后重新启用按钮
                SaveButton.Enabled = true;
            }
        }


        private async void RestoreButton_Click(object sender, EventArgs e)
        {
            string? filePath = GetSelectedLayoutFilePath();
            if (filePath == null)
                return;

            await ExecuteWithErrorHandlingAsync(async () =>
            {
                string layoutData = await File.ReadAllTextAsync(filePath);
                var desktopManager = new DesktopIcoManager();
                desktopManager.RestoreLayout(layoutData);
                UpdateRunningStatus("布局恢复成功，已应用新布局");
            }, "恢复布局时出错");
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            string? oldFilePath = GetSelectedLayoutFilePath();
            if (oldFilePath == null)
                return;

            string oldName = Path.GetFileNameWithoutExtension(oldFilePath);
            using var renameDialog = new InputDialog(oldName);
            if (renameDialog.ShowDialog() == DialogResult.OK)
            {
                string newName = renameDialog.InputTextBox.Text.Trim();
                string? directoryName = Path.GetDirectoryName(oldFilePath);
                if (directoryName == null)
                {
                    throw new Exception("无法获取文件目录路径");
                }
                string newFilePath = Path.Combine(directoryName, newName + ".json");
                ExecuteWithErrorHandling(() =>
                {
                    File.Move(oldFilePath, newFilePath);
                    var selectedItem = LayoutsListView.SelectedItems[0];
                    selectedItem.Text = newName;
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
                try
                {
                    LayoutsListView.Items.Remove(LayoutsListView.SelectedItems[0]);
                    UpdateRunningStatus("布局文件不存在，已删除无效项目");
                    return null;
                }
                catch (Exception ex)
                {
                    UpdateRunningStatus("布局文件不存在，已删除无效项目");
                    return null;
                }
            }

            return LayoutsListView.SelectedItems[0].Tag?.ToString();
        }

        private static void ShowError(string message, Exception ex)
        {
            MessageBox.Show(
                $"{message}\n\n错误详情：{ex.Message}",
                "错误",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private async void ShowSavedLayoutDetails(object sender, EventArgs e)
        {
            string? filePath = GetSelectedLayoutFilePath();
            if (filePath == null)
            {
                try
                {
                    LayoutsListView.Items.Remove(LayoutsListView.SelectedItems[0]);
                    UpdateRunningStatus("布局文件不存在，已删除无效项目");
                    return;
                }
                catch (Exception ex)
                {
                    UpdateRunningStatus("布局文件不存在，已删除无效项目");
                    return;
                }
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
                    LayoutsListView.Items.Remove(LayoutsListView.SelectedItems[0]);
                    UpdateRunningStatus("布局文件不存在，已删除无效项目");
                }
            }, "加载布局详情时出错");
        }

        private class IconData
        {
            public string Name { get; set; }
            public Position Position { get; set; }
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
