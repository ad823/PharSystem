using System;
using System.Drawing;
using System.Windows.Forms;

namespace \u8abf\u5291\u53f0\u7ba1\u7406\u7cfb\u7d71
{
    public class Form_StartupProgress : Form
    {
        private readonly Label label_Title = new Label();
        private readonly Label label_Message = new Label();
        private readonly ProgressBar progressBar = new ProgressBar();

        public Form_StartupProgress()
        {
            this.Text = "Starting";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ControlBox = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.White;
            this.ClientSize = new Size(520, 150);

            label_Title.AutoSize = false;
            label_Title.Font = new Font("Microsoft JhengHei", 16F, FontStyle.Bold);
            label_Title.ForeColor = Color.FromArgb(40, 40, 40);
            label_Title.Location = new Point(24, 20);
            label_Title.Size = new Size(472, 34);
            label_Title.Text = "Dispensing System";

            label_Message.AutoSize = false;
            label_Message.Font = new Font("Microsoft JhengHei", 11F, FontStyle.Regular);
            label_Message.ForeColor = Color.FromArgb(80, 80, 80);
            label_Message.Location = new Point(26, 66);
            label_Message.Size = new Size(468, 26);
            label_Message.Text = "Starting...";

            progressBar.Location = new Point(28, 104);
            progressBar.Size = new Size(464, 22);
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Style = ProgressBarStyle.Continuous;

            this.Controls.Add(label_Title);
            this.Controls.Add(label_Message);
            this.Controls.Add(progressBar);
        }

        public void SetProgress(string message, int value)
        {
            if (this.IsDisposed) return;

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string, int>(SetProgress), message, value);
                return;
            }

            if (value < progressBar.Minimum) value = progressBar.Minimum;
            if (value > progressBar.Maximum) value = progressBar.Maximum;

            label_Message.Text = message;
            progressBar.Value = value;
            label_Message.Refresh();
            progressBar.Refresh();
            this.Refresh();
        }
    }
}
