
namespace FADC
{
    partial class Dialog_等待條碼刷入
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
            this.rJ_Pannel1 = new MyUI.RJ_Pannel();
            this.label_state = new System.Windows.Forms.Label();
            this.rJ_Button_退出 = new MyUI.RJ_Button();
            this.rJ_Pannel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rJ_Pannel1
            // 
            this.rJ_Pannel1.BackColor = System.Drawing.Color.White;
            this.rJ_Pannel1.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Pannel1.BorderColor = System.Drawing.Color.Red;
            this.rJ_Pannel1.BorderRadius = 5;
            this.rJ_Pannel1.BorderSize = 5;
            this.rJ_Pannel1.Controls.Add(this.label_state);
            this.rJ_Pannel1.Controls.Add(this.rJ_Button_退出);
            this.rJ_Pannel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rJ_Pannel1.ForeColor = System.Drawing.Color.White;
            this.rJ_Pannel1.IsSelected = false;
            this.rJ_Pannel1.Location = new System.Drawing.Point(4, 28);
            this.rJ_Pannel1.Name = "rJ_Pannel1";
            this.rJ_Pannel1.Padding = new System.Windows.Forms.Padding(10);
            this.rJ_Pannel1.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Pannel1.ShadowSize = 0;
            this.rJ_Pannel1.Size = new System.Drawing.Size(675, 91);
            this.rJ_Pannel1.TabIndex = 1;
            // 
            // label_state
            // 
            this.label_state.BackColor = System.Drawing.Color.White;
            this.label_state.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_state.Font = new System.Drawing.Font("微軟正黑體", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_state.ForeColor = System.Drawing.Color.Black;
            this.label_state.Location = new System.Drawing.Point(10, 10);
            this.label_state.Margin = new System.Windows.Forms.Padding(0);
            this.label_state.Name = "label_state";
            this.label_state.Size = new System.Drawing.Size(505, 71);
            this.label_state.TabIndex = 24;
            this.label_state.Text = "等待條碼刷入...";
            this.label_state.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rJ_Button_退出
            // 
            this.rJ_Button_退出.AutoResetState = false;
            this.rJ_Button_退出.BackColor = System.Drawing.Color.Gray;
            this.rJ_Button_退出.BackgroundColor = System.Drawing.Color.Gray;
            this.rJ_Button_退出.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_退出.BorderRadius = 5;
            this.rJ_Button_退出.BorderSize = 0;
            this.rJ_Button_退出.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_退出.Dock = System.Windows.Forms.DockStyle.Right;
            this.rJ_Button_退出.FlatAppearance.BorderSize = 0;
            this.rJ_Button_退出.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_退出.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_退出.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_退出.GUID = "";
            this.rJ_Button_退出.Image_padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.rJ_Button_退出.Location = new System.Drawing.Point(515, 10);
            this.rJ_Button_退出.Margin = new System.Windows.Forms.Padding(0);
            this.rJ_Button_退出.Name = "rJ_Button_退出";
            this.rJ_Button_退出.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_退出.ShadowSize = 0;
            this.rJ_Button_退出.ShowLoadingForm = false;
            this.rJ_Button_退出.Size = new System.Drawing.Size(150, 71);
            this.rJ_Button_退出.State = false;
            this.rJ_Button_退出.TabIndex = 23;
            this.rJ_Button_退出.Text = "退出";
            this.rJ_Button_退出.TextColor = System.Drawing.Color.White;
            this.rJ_Button_退出.TextHeight = 0;
            this.rJ_Button_退出.UseVisualStyleBackColor = false;
            // 
            // Dialog_等待條碼刷入
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(683, 123);
            this.Controls.Add(this.rJ_Pannel1);
            this.Name = "Dialog_等待條碼刷入";
            this.rJ_Pannel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MyUI.RJ_Pannel rJ_Pannel1;
        private System.Windows.Forms.Label label_state;
        private MyUI.RJ_Button rJ_Button_退出;
    }
}