namespace DesktopICO
{
    public partial class InputDialog : Form
    {

        public InputDialog(string oldName)
        {
            InitializeComponent();
            InputTextBox.Text = oldName;
            InputTextBox.SelectAll();
        }
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("名称不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // 防止回车键在TextBox中产生换行
                ButtonOK_Click(sender, e);
            }
        }
    }

}
