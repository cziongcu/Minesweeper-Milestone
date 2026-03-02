using System;
using System.Drawing;
using System.Windows.Forms;

namespace MinesweeperGUI
{
    public class Form3 : Form
    {
        public string WinnerName { get; private set; }

        private Label lblMessage;
        private TextBox txtName;
        private Label lblScore;
        private Button btnOk;

        public Form3(int score)
        {
            this.Text = "Start a game";
            this.Size = new Size(350, 180);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblMessage = new Label();
            lblMessage.Text = "Congratulations you win. Enter your name.";
            lblMessage.Location = new Point(20, 20);
            lblMessage.AutoSize = true;

            txtName = new TextBox();
            txtName.Location = new Point(20, 50);
            txtName.Width = 290;

            lblScore = new Label();
            lblScore.Text = $"Score: {score}";
            lblScore.Location = new Point(20, 80);
            lblScore.AutoSize = true;

            btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(120, 100);
            btnOk.Click += BtnOk_Click;

            this.Controls.Add(lblMessage);
            this.Controls.Add(txtName);
            this.Controls.Add(lblScore);
            this.Controls.Add(btnOk);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                WinnerName = txtName.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a name.");
            }
        }
    }
}
