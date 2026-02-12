
namespace 勤務傳送系統
{
    partial class Dialog_時段設定
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
            this.numTextBox_st_HH = new MyUI.NumTextBox();
            this.numTextBox_st_MM = new MyUI.NumTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numTextBox_end_MM = new MyUI.NumTextBox();
            this.numTextBox_end_HH = new MyUI.NumTextBox();
            this.plC_RJ_Button_確認送出 = new MyUI.PLC_RJ_Button();
            this.SuspendLayout();
            // 
            // numTextBox_st_HH
            // 
            this.numTextBox_st_HH.Font = new System.Drawing.Font("微軟正黑體", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numTextBox_st_HH.Location = new System.Drawing.Point(48, 75);
            this.numTextBox_st_HH.Name = "numTextBox_st_HH";
            this.numTextBox_st_HH.Size = new System.Drawing.Size(114, 54);
            this.numTextBox_st_HH.TabIndex = 0;
            this.numTextBox_st_HH.Text = "00";
            this.numTextBox_st_HH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTextBox_st_HH.字元長度 = MyUI.NumTextBox.WordLengthEnum.單字元;
            this.numTextBox_st_HH.小數點位置 = 0;
            this.numTextBox_st_HH.音效 = false;
            this.numTextBox_st_HH.顯示螢幕小鍵盤 = false;
            // 
            // numTextBox_st_MM
            // 
            this.numTextBox_st_MM.Font = new System.Drawing.Font("微軟正黑體", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numTextBox_st_MM.Location = new System.Drawing.Point(208, 75);
            this.numTextBox_st_MM.Name = "numTextBox_st_MM";
            this.numTextBox_st_MM.Size = new System.Drawing.Size(114, 54);
            this.numTextBox_st_MM.TabIndex = 1;
            this.numTextBox_st_MM.Text = "00";
            this.numTextBox_st_MM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTextBox_st_MM.字元長度 = MyUI.NumTextBox.WordLengthEnum.單字元;
            this.numTextBox_st_MM.小數點位置 = 0;
            this.numTextBox_st_MM.音效 = false;
            this.numTextBox_st_MM.顯示螢幕小鍵盤 = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(169, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 45);
            this.label1.TabIndex = 2;
            this.label1.Text = ":";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(21, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "起始";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(356, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 21);
            this.label3.TabIndex = 7;
            this.label3.Text = "結束";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(504, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 45);
            this.label4.TabIndex = 6;
            this.label4.Text = ":";
            // 
            // numTextBox_end_MM
            // 
            this.numTextBox_end_MM.Font = new System.Drawing.Font("微軟正黑體", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numTextBox_end_MM.Location = new System.Drawing.Point(543, 75);
            this.numTextBox_end_MM.Name = "numTextBox_end_MM";
            this.numTextBox_end_MM.Size = new System.Drawing.Size(114, 54);
            this.numTextBox_end_MM.TabIndex = 5;
            this.numTextBox_end_MM.Text = "59";
            this.numTextBox_end_MM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTextBox_end_MM.字元長度 = MyUI.NumTextBox.WordLengthEnum.單字元;
            this.numTextBox_end_MM.小數點位置 = 0;
            this.numTextBox_end_MM.音效 = false;
            this.numTextBox_end_MM.顯示螢幕小鍵盤 = false;
            // 
            // numTextBox_end_HH
            // 
            this.numTextBox_end_HH.Font = new System.Drawing.Font("微軟正黑體", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numTextBox_end_HH.Location = new System.Drawing.Point(383, 75);
            this.numTextBox_end_HH.Name = "numTextBox_end_HH";
            this.numTextBox_end_HH.Size = new System.Drawing.Size(114, 54);
            this.numTextBox_end_HH.TabIndex = 4;
            this.numTextBox_end_HH.Text = "23";
            this.numTextBox_end_HH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTextBox_end_HH.字元長度 = MyUI.NumTextBox.WordLengthEnum.單字元;
            this.numTextBox_end_HH.小數點位置 = 0;
            this.numTextBox_end_HH.音效 = false;
            this.numTextBox_end_HH.顯示螢幕小鍵盤 = false;
            // 
            // plC_RJ_Button_確認送出
            // 
            this.plC_RJ_Button_確認送出.AutoResetState = true;
            this.plC_RJ_Button_確認送出.BackgroundColor = System.Drawing.Color.White;
            this.plC_RJ_Button_確認送出.Bool = false;
            this.plC_RJ_Button_確認送出.BorderColor = System.Drawing.Color.Black;
            this.plC_RJ_Button_確認送出.BorderRadius = 15;
            this.plC_RJ_Button_確認送出.BorderSize = 1;
            this.plC_RJ_Button_確認送出.but_press = false;
            this.plC_RJ_Button_確認送出.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_確認送出.DisenableColor = System.Drawing.Color.Gray;
            this.plC_RJ_Button_確認送出.Dock = System.Windows.Forms.DockStyle.Right;
            this.plC_RJ_Button_確認送出.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_確認送出.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_確認送出.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_確認送出.GUID = "";
            this.plC_RJ_Button_確認送出.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_確認送出.Image_padding = new System.Windows.Forms.Padding(18, 7, 23, 5);
            this.plC_RJ_Button_確認送出.Location = new System.Drawing.Point(710, 44);
            this.plC_RJ_Button_確認送出.Name = "plC_RJ_Button_確認送出";
            this.plC_RJ_Button_確認送出.OFF_文字內容 = "確認";
            this.plC_RJ_Button_確認送出.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_確認送出.OFF_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_確認送出.OFF_背景顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_確認送出.ON_BorderSize = 1;
            this.plC_RJ_Button_確認送出.ON_文字內容 = "確認";
            this.plC_RJ_Button_確認送出.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_確認送出.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_確認送出.ON_背景顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_確認送出.ProhibitionBorderLineWidth = 1;
            this.plC_RJ_Button_確認送出.ProhibitionLineWidth = 4;
            this.plC_RJ_Button_確認送出.ProhibitionSymbolSize = 30;
            this.plC_RJ_Button_確認送出.ShadowColor = System.Drawing.Color.DimGray;
            this.plC_RJ_Button_確認送出.ShadowSize = 3;
            this.plC_RJ_Button_確認送出.ShowLoadingForm = false;
            this.plC_RJ_Button_確認送出.Size = new System.Drawing.Size(108, 102);
            this.plC_RJ_Button_確認送出.State = false;
            this.plC_RJ_Button_確認送出.TabIndex = 146;
            this.plC_RJ_Button_確認送出.Text = "確認";
            this.plC_RJ_Button_確認送出.TextColor = System.Drawing.Color.Black;
            this.plC_RJ_Button_確認送出.TextHeight = 35;
            this.plC_RJ_Button_確認送出.Texts = "確認";
            this.plC_RJ_Button_確認送出.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_確認送出.字型鎖住 = false;
            this.plC_RJ_Button_確認送出.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_確認送出.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_確認送出.文字鎖住 = false;
            this.plC_RJ_Button_確認送出.背景圖片 = global::勤務傳送系統.Properties.Resources.adjusted_checkmark_removebg_preview;
            this.plC_RJ_Button_確認送出.讀取位元反向 = false;
            this.plC_RJ_Button_確認送出.讀寫鎖住 = false;
            this.plC_RJ_Button_確認送出.音效 = false;
            this.plC_RJ_Button_確認送出.顯示 = false;
            this.plC_RJ_Button_確認送出.顯示狀態 = false;
            // 
            // Dialog_時段設定
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CaptionHeight = 40;
            this.ClientSize = new System.Drawing.Size(822, 150);
            this.CloseBoxSize = new System.Drawing.Size(40, 40);
            this.ControlBox = true;
            this.Controls.Add(this.plC_RJ_Button_確認送出);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numTextBox_end_MM);
            this.Controls.Add(this.numTextBox_end_HH);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numTextBox_st_MM);
            this.Controls.Add(this.numTextBox_st_HH);
            this.MaximizeBox = false;
            this.MaxSize = new System.Drawing.Size(40, 40);
            this.MiniSize = new System.Drawing.Size(40, 40);
            this.Name = "Dialog_時段設定";
            this.Text = "時段設定";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyUI.NumTextBox numTextBox_st_HH;
        private MyUI.NumTextBox numTextBox_st_MM;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private MyUI.NumTextBox numTextBox_end_MM;
        private MyUI.NumTextBox numTextBox_end_HH;
        private MyUI.PLC_RJ_Button plC_RJ_Button_確認送出;
    }
}