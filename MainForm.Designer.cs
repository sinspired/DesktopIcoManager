using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace DesktopICO
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            LayoutsListView = new ListView();
            布局名称 = new ColumnHeader();
            用户名 = new ColumnHeader();
            创建时间 = new ColumnHeader();
            DescriptionLabel = new Label();
            SaveButton = new Button();
            RestoreButton = new Button();
            RenameButton = new Button();
            DeleteButton = new Button();
            contextMenuStripNotify = new ContextMenuStrip(components);
            toolStripMenuItem_savelayout = new ToolStripMenuItem();
            toolStripMenuItem_about = new ToolStripMenuItem();
            toolStripMenuItem_exit = new ToolStripMenuItem();
            AutoStartButton = new Button();
            AboutButton = new Button();
            SavePathLabel = new Label();
            StatusLabel = new Label();
            main_notifyIcon = new NotifyIcon(components);
            contextMenuStripNotify.SuspendLayout();
            SuspendLayout();
            // 
            // LayoutsListView
            // 
            LayoutsListView.AccessibleDescription = "布局列表";
            LayoutsListView.Alignment = ListViewAlignment.Default;
            LayoutsListView.Columns.AddRange(new ColumnHeader[] { 布局名称, 用户名, 创建时间 });
            LayoutsListView.ForeColor = SystemColors.GrayText;
            LayoutsListView.FullRowSelect = true;
            LayoutsListView.ImeMode = ImeMode.HangulFull;
            LayoutsListView.LabelWrap = false;
            LayoutsListView.Location = new Point(29, 142);
            LayoutsListView.Margin = new Padding(4);
            LayoutsListView.MultiSelect = false;
            LayoutsListView.Name = "LayoutsListView";
            LayoutsListView.ShowGroups = false;
            LayoutsListView.Size = new Size(720, 398);
            LayoutsListView.TabIndex = 6;
            LayoutsListView.UseCompatibleStateImageBehavior = false;
            LayoutsListView.View = View.Details;
            LayoutsListView.ColumnClick += LayoutsListView_ColumnClick;
            LayoutsListView.ItemActivate += RestoreButton_Click;
            LayoutsListView.ItemSelectionChanged += ShowSavedLayoutDetails;
            LayoutsListView.SelectedIndexChanged += LayoutsListView_SelectedIndexChanged;
            // 
            // 布局名称
            // 
            布局名称.Text = "布局名称";
            布局名称.Width = 275;
            // 
            // 用户名
            // 
            用户名.Text = "用户名";
            用户名.Width = 200;
            // 
            // 创建时间
            // 
            创建时间.Text = "创建时间";
            创建时间.Width = 240;
            // 
            // DescriptionLabel
            // 
            DescriptionLabel.AutoSize = true;
            DescriptionLabel.Font = new Font("Microsoft YaHei UI", 9F);
            DescriptionLabel.ForeColor = SystemColors.ControlDarkDark;
            DescriptionLabel.Location = new Point(29, 28);
            DescriptionLabel.Margin = new Padding(4, 0, 4, 0);
            DescriptionLabel.Name = "DescriptionLabel";
            DescriptionLabel.Size = new Size(546, 28);
            DescriptionLabel.TabIndex = 1;
            DescriptionLabel.Text = "此工具可以帮助您备份和恢复Windows桌面上的图标位置";
            // 
            // SaveButton
            // 
            SaveButton.Font = new Font("Microsoft YaHei UI", 9F);
            SaveButton.Location = new Point(29, 76);
            SaveButton.Margin = new Padding(4);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(173, 49);
            SaveButton.TabIndex = 0;
            SaveButton.Text = "保存桌面布局";
            SaveButton.Click += SaveButton_Click;
            // 
            // RestoreButton
            // 
            RestoreButton.Enabled = false;
            RestoreButton.Font = new Font("Microsoft YaHei UI", 9F);
            RestoreButton.Location = new Point(217, 76);
            RestoreButton.Margin = new Padding(4);
            RestoreButton.Name = "RestoreButton";
            RestoreButton.Size = new Size(173, 49);
            RestoreButton.TabIndex = 1;
            RestoreButton.Text = "恢复选中布局";
            RestoreButton.Click += RestoreButton_Click;
            // 
            // RenameButton
            // 
            RenameButton.Enabled = false;
            RenameButton.Font = new Font("Microsoft YaHei UI", 9F);
            RenameButton.Location = new Point(404, 76);
            RenameButton.Margin = new Padding(4);
            RenameButton.Name = "RenameButton";
            RenameButton.Size = new Size(173, 49);
            RenameButton.TabIndex = 2;
            RenameButton.Text = "重命名布局";
            RenameButton.Click += RenameButton_Click;
            // 
            // DeleteButton
            // 
            DeleteButton.Enabled = false;
            DeleteButton.Font = new Font("Microsoft YaHei UI", 9F);
            DeleteButton.Location = new Point(592, 76);
            DeleteButton.Margin = new Padding(4);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new Size(159, 49);
            DeleteButton.TabIndex = 3;
            DeleteButton.Text = "删除";
            DeleteButton.Click += DeleteButton_Click;
            // 
            // contextMenuStripNotify
            // 
            contextMenuStripNotify.AccessibleDescription = "系统托盘右键菜单";
            contextMenuStripNotify.AccessibleName = "桌面图标位置管理器右键菜单";
            contextMenuStripNotify.ImageScalingSize = new Size(28, 28);
            contextMenuStripNotify.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_savelayout, toolStripMenuItem_about, toolStripMenuItem_exit });
            contextMenuStripNotify.Name = "contextMenuStrip1";
            contextMenuStripNotify.Size = new Size(253, 106);
            contextMenuStripNotify.Text = "桌面图标位置管理器";
            // 
            // toolStripMenuItem_savelayout
            // 
            toolStripMenuItem_savelayout.AccessibleDescription = "保存桌面布局";
            toolStripMenuItem_savelayout.Name = "toolStripMenuItem_savelayout";
            toolStripMenuItem_savelayout.Size = new Size(252, 34);
            toolStripMenuItem_savelayout.Text = "快速保存桌面布局";
            toolStripMenuItem_savelayout.Click += SaveButton_Click;
            // 
            // toolStripMenuItem_about
            // 
            toolStripMenuItem_about.AccessibleDescription = "关于桌面图标位置管理器";
            toolStripMenuItem_about.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripMenuItem_about.Name = "toolStripMenuItem_about";
            toolStripMenuItem_about.Size = new Size(252, 34);
            toolStripMenuItem_about.Text = "关于";
            toolStripMenuItem_about.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolStripMenuItem_about.Click += AboutButton_Click;
            // 
            // toolStripMenuItem_exit
            // 
            toolStripMenuItem_exit.AccessibleDescription = "退出软件";
            toolStripMenuItem_exit.Name = "toolStripMenuItem_exit";
            toolStripMenuItem_exit.Size = new Size(252, 34);
            toolStripMenuItem_exit.Text = "退出";
            toolStripMenuItem_exit.Click += ExitToolStripMenuItem_Click;
            // 
            // AutoStartButton
            // 
            AutoStartButton.Font = new Font("Microsoft YaHei UI", 9F);
            AutoStartButton.Location = new Point(404, 616);
            AutoStartButton.Margin = new Padding(4);
            AutoStartButton.Name = "AutoStartButton";
            AutoStartButton.Size = new Size(173, 49);
            AutoStartButton.TabIndex = 4;
            AutoStartButton.Text = "开机启动";
            AutoStartButton.Click += AutoStartButton_Click;
            // 
            // AboutButton
            // 
            AboutButton.Font = new Font("Microsoft YaHei UI", 9F);
            AboutButton.Location = new Point(592, 616);
            AboutButton.Margin = new Padding(4);
            AboutButton.Name = "AboutButton";
            AboutButton.Size = new Size(159, 49);
            AboutButton.TabIndex = 5;
            AboutButton.Text = "关于";
            AboutButton.Click += AboutButton_Click;
            // 
            // SavePathLabel
            // 
            SavePathLabel.AutoSize = true;
            SavePathLabel.Font = new Font("Microsoft YaHei UI", 9F);
            SavePathLabel.ForeColor = SystemColors.ButtonShadow;
            SavePathLabel.Location = new Point(29, 554);
            SavePathLabel.Margin = new Padding(4, 0, 4, 0);
            SavePathLabel.Name = "SavePathLabel";
            SavePathLabel.Size = new Size(117, 28);
            SavePathLabel.TabIndex = 8;
            SavePathLabel.Text = "保存目录：";
            SavePathLabel.Click += OpenLayoutsFoder;
            // 
            // StatusLabel
            // 
            StatusLabel.AutoSize = true;
            StatusLabel.Font = new Font("Microsoft YaHei UI", 9F);
            StatusLabel.Location = new Point(29, 623);
            StatusLabel.Margin = new Padding(4, 0, 4, 0);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new Size(138, 28);
            StatusLabel.TabIndex = 7;
            StatusLabel.Text = "程序运行状态";
            // 
            // main_notifyIcon
            // 
            main_notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            main_notifyIcon.BalloonTipText = "桌面图标位置管理器";
            main_notifyIcon.BalloonTipTitle = "提供桌面图标位置备份、恢复，避免重装系统等操作导致桌面图标混乱。";
            main_notifyIcon.ContextMenuStrip = contextMenuStripNotify;
            main_notifyIcon.Icon = (Icon)resources.GetObject("main_notifyIcon.Icon");
            main_notifyIcon.Text = "桌面图标位置管理器";
            main_notifyIcon.Visible = true;
            // 
            // MainForm
            // 
            AcceptButton = SaveButton;
            AccessibleDescription = "桌面图标位置管理器";
            AccessibleName = "DesktopManager";
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(780, 700);
            Controls.Add(DescriptionLabel);
            Controls.Add(SaveButton);
            Controls.Add(RestoreButton);
            Controls.Add(RenameButton);
            Controls.Add(DeleteButton);
            Controls.Add(LayoutsListView);
            Controls.Add(AutoStartButton);
            Controls.Add(AboutButton);
            Controls.Add(StatusLabel);
            Controls.Add(SavePathLabel);
            Font = new Font("Microsoft YaHei UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "MainForm";
            Padding = new Padding(22, 21, 22, 21);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "桌面图标位置管理器";
            contextMenuStripNotify.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button RestoreButton;
        private System.Windows.Forms.Button RenameButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.ListView LayoutsListView;
        private ColumnHeader 布局名称;
        private ColumnHeader 用户名;
        private ColumnHeader 创建时间;
        private System.Windows.Forms.Button AutoStartButton;
        private System.Windows.Forms.Button AboutButton;
        private System.Windows.Forms.Label SavePathLabel;
        private System.Windows.Forms.Label StatusLabel;
        private NotifyIcon main_notifyIcon;
        private ContextMenuStrip contextMenuStripNotify;
        private ToolStripMenuItem toolStripMenuItem_about;
        private ToolStripMenuItem toolStripMenuItem_exit;
        private ToolStripMenuItem toolStripMenuItem_savelayout;
    }
}
