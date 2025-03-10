namespace DesktopICO
{
    partial class InputDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            InputTextBox = new TextBox();
            buttonOK = new Button();
            buttonCancel = new Button();
            labelPrompt = new Label();
            SuspendLayout();
            // 
            // InputTextBox
            // 
            InputTextBox.Location = new Point(159, 53);
            InputTextBox.Margin = new Padding(6, 4, 4, 4);
            InputTextBox.Name = "InputTextBox";
            InputTextBox.Size = new Size(303, 34);
            InputTextBox.TabIndex = 0;
            InputTextBox.KeyDown += new KeyEventHandler(InputTextBox_KeyDown);
            // 
            // buttonOK
            // 
            buttonOK.Location = new Point(104, 140);
            buttonOK.Margin = new Padding(4);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(130, 56);
            buttonOK.TabIndex = 1;
            buttonOK.Text = "确定";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += ButtonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(286, 140);
            buttonCancel.Margin = new Padding(4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(130, 56);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "取消";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // labelPrompt
            // 
            labelPrompt.AutoSize = true;
            labelPrompt.Location = new Point(26, 56);
            labelPrompt.Margin = new Padding(4, 0, 4, 0);
            labelPrompt.Name = "labelPrompt";
            labelPrompt.Size = new Size(138, 28);
            labelPrompt.TabIndex = 0;
            labelPrompt.Text = "请输入名称：";
            // 
            // InputDialog
            // 
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 252);
            Controls.Add(labelPrompt);
            Controls.Add(InputTextBox);
            Controls.Add(buttonOK);
            Controls.Add(buttonCancel);
            Margin = new Padding(4);
            Name = "InputDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "重命名布局";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public TextBox InputTextBox;
        private Button buttonOK;
        private Button buttonCancel;
        private Label labelPrompt;

    }
}
