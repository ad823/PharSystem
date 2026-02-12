namespace 勤務傳送系統
{
    partial class Dialog_修改密碼
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel89 = new System.Windows.Forms.Panel();
            this.textBox_新密碼 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_密碼確認 = new System.Windows.Forms.TextBox();
            this.button_確認 = new System.Windows.Forms.Button();
            this.button_取消 = new System.Windows.Forms.Button();
            this.panel89.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel89
            // 
            this.panel89.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel89.Controls.Add(this.label1);
            this.panel89.Controls.Add(this.textBox_新密碼);
            this.panel89.Location = new System.Drawing.Point(12, 21);
            this.panel89.Name = "panel89";
            this.panel89.Size = new System.Drawing.Size(399, 47);
            this.panel89.TabIndex = 4;
            // 
            // textBox_新密碼
            // 
            this.textBox_新密碼.Font = new System.Drawing.Font("新細明體", 12F);
            this.textBox_新密碼.Location = new System.Drawing.Point(81, 9);
            this.textBox_新密碼.Name = "textBox_新密碼";
            this.textBox_新密碼.PasswordChar = '*';
            this.textBox_新密碼.Size = new System.Drawing.Size(304, 27);
            this.textBox_新密碼.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 12F);
            this.label1.Location = new System.Drawing.Point(7, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "新密碼";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBox_密碼確認);
            this.panel1.Location = new System.Drawing.Point(12, 74);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(399, 47);
            this.panel1.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 12F);
            this.label2.Location = new System.Drawing.Point(5, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "密碼確認";
            // 
            // textBox_密碼確認
            // 
            this.textBox_密碼確認.Font = new System.Drawing.Font("新細明體", 12F);
            this.textBox_密碼確認.Location = new System.Drawing.Point(81, 9);
            this.textBox_密碼確認.Name = "textBox_密碼確認";
            this.textBox_密碼確認.PasswordChar = '*';
            this.textBox_密碼確認.Size = new System.Drawing.Size(304, 27);
            this.textBox_密碼確認.TabIndex = 1;
            // 
            // button_確認
            // 
            this.button_確認.Location = new System.Drawing.Point(433, 75);
            this.button_確認.Name = "button_確認";
            this.button_確認.Size = new System.Drawing.Size(75, 47);
            this.button_確認.TabIndex = 7;
            this.button_確認.Text = "確認";
            this.button_確認.UseVisualStyleBackColor = true;
            this.button_確認.Click += new System.EventHandler(this.button_確認_Click);
            // 
            // button_取消
            // 
            this.button_取消.Location = new System.Drawing.Point(433, 22);
            this.button_取消.Name = "button_取消";
            this.button_取消.Size = new System.Drawing.Size(75, 47);
            this.button_取消.TabIndex = 8;
            this.button_取消.Text = "取消";
            this.button_取消.UseVisualStyleBackColor = true;
            this.button_取消.Click += new System.EventHandler(this.button_取消_Click);
            // 
            // Dialog_修改密碼
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 134);
            this.ControlBox = false;
            this.Controls.Add(this.button_取消);
            this.Controls.Add(this.button_確認);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel89);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Dialog_修改密碼";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改密碼";
            this.panel89.ResumeLayout(false);
            this.panel89.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel89;
        private System.Windows.Forms.TextBox textBox_新密碼;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_密碼確認;
        private System.Windows.Forms.Button button_確認;
        private System.Windows.Forms.Button button_取消;
    }
}