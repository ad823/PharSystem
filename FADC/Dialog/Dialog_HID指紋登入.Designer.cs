namespace FADC
{
    partial class Dialog_HID指紋登入
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.rJ_Button_取消 = new MyUI.RJ_Button();
            this.rJ_Lable_state = new MyUI.RJ_Lable();
            this.pbFinger1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFinger1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rJ_Button_取消);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(4, 302);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(863, 82);
            this.panel1.TabIndex = 8;
            // 
            // rJ_Button_取消
            // 
            this.rJ_Button_取消.AutoResetState = false;
            this.rJ_Button_取消.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_取消.BackgroundColor = System.Drawing.Color.White;
            this.rJ_Button_取消.BorderColor = System.Drawing.Color.DimGray;
            this.rJ_Button_取消.BorderRadius = 22;
            this.rJ_Button_取消.BorderSize = 1;
            this.rJ_Button_取消.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_取消.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_取消.Dock = System.Windows.Forms.DockStyle.Right;
            this.rJ_Button_取消.FlatAppearance.BorderSize = 0;
            this.rJ_Button_取消.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_取消.Font = new System.Drawing.Font("微軟正黑體", 14.25F);
            this.rJ_Button_取消.ForeColor = System.Drawing.Color.Black;
            this.rJ_Button_取消.GUID = "";
            this.rJ_Button_取消.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_取消.Location = new System.Drawing.Point(737, 0);
            this.rJ_Button_取消.Name = "rJ_Button_取消";
            this.rJ_Button_取消.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_取消.ProhibitionLineWidth = 4;
            this.rJ_Button_取消.ProhibitionSymbolSize = 30;
            this.rJ_Button_取消.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_取消.ShadowSize = 0;
            this.rJ_Button_取消.ShowLoadingForm = false;
            this.rJ_Button_取消.Size = new System.Drawing.Size(126, 82);
            this.rJ_Button_取消.State = false;
            this.rJ_Button_取消.TabIndex = 164;
            this.rJ_Button_取消.Text = "取消";
            this.rJ_Button_取消.TextColor = System.Drawing.Color.Black;
            this.rJ_Button_取消.TextHeight = 0;
            this.rJ_Button_取消.UseVisualStyleBackColor = false;
            // 
            // rJ_Lable_state
            // 
            this.rJ_Lable_state.BackColor = System.Drawing.Color.White;
            this.rJ_Lable_state.BackgroundColor = System.Drawing.Color.White;
            this.rJ_Lable_state.BorderColor = System.Drawing.Color.Black;
            this.rJ_Lable_state.BorderRadius = 10;
            this.rJ_Lable_state.BorderSize = 2;
            this.rJ_Lable_state.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_state.Font = new System.Drawing.Font("微軟正黑體", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_state.ForeColor = System.Drawing.Color.Black;
            this.rJ_Lable_state.GUID = "";
            this.rJ_Lable_state.Location = new System.Drawing.Point(240, 58);
            this.rJ_Lable_state.Name = "rJ_Lable_state";
            this.rJ_Lable_state.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_state.ShadowSize = 0;
            this.rJ_Lable_state.Size = new System.Drawing.Size(613, 220);
            this.rJ_Lable_state.TabIndex = 10;
            this.rJ_Lable_state.Text = "請將手指放上指紋機";
            this.rJ_Lable_state.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable_state.TextColor = System.Drawing.Color.Black;
            // 
            // pbFinger1
            // 
            this.pbFinger1.BackColor = System.Drawing.Color.Gainsboro;
            this.pbFinger1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbFinger1.Location = new System.Drawing.Point(21, 58);
            this.pbFinger1.Name = "pbFinger1";
            this.pbFinger1.Size = new System.Drawing.Size(200, 220);
            this.pbFinger1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbFinger1.TabIndex = 9;
            this.pbFinger1.TabStop = false;
            // 
            // Dialog_HID指紋登入
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(871, 388);
            this.Controls.Add(this.rJ_Lable_state);
            this.Controls.Add(this.pbFinger1);
            this.Controls.Add(this.panel1);
            this.Name = "Dialog_HID指紋登入";
            this.Text = "指紋登入";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbFinger1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private MyUI.RJ_Button rJ_Button_取消;
        private MyUI.RJ_Lable rJ_Lable_state;
        private System.Windows.Forms.PictureBox pbFinger1;
    }
}