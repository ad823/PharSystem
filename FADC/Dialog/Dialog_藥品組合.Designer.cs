namespace FADC
{
    partial class Dialog_藥品組合
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox_搜尋內容 = new MyUI.RJ_TextBox();
            this.rJ_Button_搜尋 = new MyUI.RJ_Button();
            this.comboBox_搜尋條件 = new System.Windows.Forms.ComboBox();
            this.sqL_DataGridView_藥品搜尋 = new SQLUI.SQL_DataGridView();
            this.rJ_Lable1 = new MyUI.RJ_Lable();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rJ_Button_刪除組合 = new MyUI.RJ_Button();
            this.rJ_Button_加入 = new MyUI.RJ_Button();
            this.rJ_Button_確認組合 = new MyUI.RJ_Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.sqL_DataGridView_藥品搜尋);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(4, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(748, 768);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rJ_Button_加入);
            this.panel2.Controls.Add(this.textBox_搜尋內容);
            this.panel2.Controls.Add(this.rJ_Button_搜尋);
            this.panel2.Controls.Add(this.comboBox_搜尋條件);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 668);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(748, 100);
            this.panel2.TabIndex = 123;
            // 
            // textBox_搜尋內容
            // 
            this.textBox_搜尋內容.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_搜尋內容.BorderColor = System.Drawing.Color.Black;
            this.textBox_搜尋內容.BorderFocusColor = System.Drawing.Color.HotPink;
            this.textBox_搜尋內容.BorderRadius = 0;
            this.textBox_搜尋內容.BorderSize = 2;
            this.textBox_搜尋內容.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_搜尋內容.ForeColor = System.Drawing.Color.DimGray;
            this.textBox_搜尋內容.GUID = "";
            this.textBox_搜尋內容.Location = new System.Drawing.Point(232, 33);
            this.textBox_搜尋內容.Multiline = false;
            this.textBox_搜尋內容.Name = "textBox_搜尋內容";
            this.textBox_搜尋內容.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.textBox_搜尋內容.PassWordChar = false;
            this.textBox_搜尋內容.PlaceholderColor = System.Drawing.Color.Silver;
            this.textBox_搜尋內容.PlaceholderText = "請輸入搜尋條件";
            this.textBox_搜尋內容.ShowTouchPannel = false;
            this.textBox_搜尋內容.Size = new System.Drawing.Size(252, 37);
            this.textBox_搜尋內容.TabIndex = 13;
            this.textBox_搜尋內容.TextAlgin = System.Windows.Forms.HorizontalAlignment.Left;
            this.textBox_搜尋內容.Texts = "";
            this.textBox_搜尋內容.UnderlineStyle = false;
            // 
            // rJ_Button_搜尋
            // 
            this.rJ_Button_搜尋.AutoResetState = false;
            this.rJ_Button_搜尋.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_搜尋.BackgroundColor = System.Drawing.Color.Black;
            this.rJ_Button_搜尋.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_搜尋.BorderRadius = 10;
            this.rJ_Button_搜尋.BorderSize = 0;
            this.rJ_Button_搜尋.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_搜尋.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_搜尋.FlatAppearance.BorderSize = 0;
            this.rJ_Button_搜尋.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_搜尋.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_搜尋.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_搜尋.GUID = "";
            this.rJ_Button_搜尋.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_搜尋.Location = new System.Drawing.Point(490, 19);
            this.rJ_Button_搜尋.Name = "rJ_Button_搜尋";
            this.rJ_Button_搜尋.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_搜尋.ProhibitionLineWidth = 4;
            this.rJ_Button_搜尋.ProhibitionSymbolSize = 30;
            this.rJ_Button_搜尋.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_搜尋.ShadowSize = 3;
            this.rJ_Button_搜尋.ShowLoadingForm = false;
            this.rJ_Button_搜尋.Size = new System.Drawing.Size(119, 63);
            this.rJ_Button_搜尋.State = false;
            this.rJ_Button_搜尋.TabIndex = 12;
            this.rJ_Button_搜尋.Text = "搜尋";
            this.rJ_Button_搜尋.TextColor = System.Drawing.Color.White;
            this.rJ_Button_搜尋.TextHeight = 0;
            this.rJ_Button_搜尋.UseVisualStyleBackColor = false;
            // 
            // comboBox_搜尋條件
            // 
            this.comboBox_搜尋條件.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_搜尋條件.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_搜尋條件.FormattingEnabled = true;
            this.comboBox_搜尋條件.Items.AddRange(new object[] {
            "藥名",
            "藥碼",
            "中文名",
            "全部顯示"});
            this.comboBox_搜尋條件.Location = new System.Drawing.Point(24, 35);
            this.comboBox_搜尋條件.Name = "comboBox_搜尋條件";
            this.comboBox_搜尋條件.Size = new System.Drawing.Size(202, 35);
            this.comboBox_搜尋條件.TabIndex = 11;
            // 
            // sqL_DataGridView_藥品搜尋
            // 
            this.sqL_DataGridView_藥品搜尋.AutoSelectToDeep = false;
            this.sqL_DataGridView_藥品搜尋.backColor = System.Drawing.Color.DarkGray;
            this.sqL_DataGridView_藥品搜尋.BorderColor = System.Drawing.Color.DarkGray;
            this.sqL_DataGridView_藥品搜尋.BorderRadius = 0;
            this.sqL_DataGridView_藥品搜尋.BorderSize = 2;
            this.sqL_DataGridView_藥品搜尋.CellBorderColor = System.Drawing.Color.Silver;
            this.sqL_DataGridView_藥品搜尋.cellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.sqL_DataGridView_藥品搜尋.cellStylBackColor = System.Drawing.Color.Silver;
            this.sqL_DataGridView_藥品搜尋.cellStyleFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.sqL_DataGridView_藥品搜尋.cellStylForeColor = System.Drawing.Color.Black;
            this.sqL_DataGridView_藥品搜尋.checkedRowBackColor = System.Drawing.Color.YellowGreen;
            this.sqL_DataGridView_藥品搜尋.columnHeaderBackColor = System.Drawing.SystemColors.Control;
            this.sqL_DataGridView_藥品搜尋.columnHeaderBorderColor = System.Drawing.Color.Gainsboro;
            this.sqL_DataGridView_藥品搜尋.columnHeaderFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.sqL_DataGridView_藥品搜尋.columnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_藥品搜尋.columnHeadersHeight = 18;
            this.sqL_DataGridView_藥品搜尋.columnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sqL_DataGridView_藥品搜尋.DataGridViewAutoSizeColumnMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.NotSet;
            this.sqL_DataGridView_藥品搜尋.DataKeyEnable = false;
            this.sqL_DataGridView_藥品搜尋.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqL_DataGridView_藥品搜尋.Font = new System.Drawing.Font("新細明體", 12F);
            this.sqL_DataGridView_藥品搜尋.ImageBox = false;
            this.sqL_DataGridView_藥品搜尋.Location = new System.Drawing.Point(0, 0);
            this.sqL_DataGridView_藥品搜尋.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.sqL_DataGridView_藥品搜尋.Name = "sqL_DataGridView_藥品搜尋";
            this.sqL_DataGridView_藥品搜尋.OnlineState = SQLUI.SQL_DataGridView.OnlineEnum.Online;
            this.sqL_DataGridView_藥品搜尋.Password = "user82822040";
            this.sqL_DataGridView_藥品搜尋.Port = ((uint)(3306u));
            this.sqL_DataGridView_藥品搜尋.rowBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            this.sqL_DataGridView_藥品搜尋.rowHeaderBackColor = System.Drawing.Color.CornflowerBlue;
            this.sqL_DataGridView_藥品搜尋.rowHeaderBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            this.sqL_DataGridView_藥品搜尋.rowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_藥品搜尋.RowsColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sqL_DataGridView_藥品搜尋.RowsHeight = 50;
            this.sqL_DataGridView_藥品搜尋.SaveFileName = "SQL_DataGridView";
            this.sqL_DataGridView_藥品搜尋.selectedBorderSize = 0;
            this.sqL_DataGridView_藥品搜尋.selectedRowBackColor = System.Drawing.Color.Blue;
            this.sqL_DataGridView_藥品搜尋.selectedRowBorderColor = System.Drawing.Color.Blue;
            this.sqL_DataGridView_藥品搜尋.selectedRowForeColor = System.Drawing.Color.White;
            this.sqL_DataGridView_藥品搜尋.Server = "localhost";
            this.sqL_DataGridView_藥品搜尋.Size = new System.Drawing.Size(748, 668);
            this.sqL_DataGridView_藥品搜尋.SSLMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            this.sqL_DataGridView_藥品搜尋.TabIndex = 125;
            this.sqL_DataGridView_藥品搜尋.UserName = "root";
            this.sqL_DataGridView_藥品搜尋.可拖曳欄位寬度 = true;
            this.sqL_DataGridView_藥品搜尋.可選擇多列 = true;
            this.sqL_DataGridView_藥品搜尋.單格樣式 = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.sqL_DataGridView_藥品搜尋.自動換行 = true;
            this.sqL_DataGridView_藥品搜尋.表單字體 = new System.Drawing.Font("新細明體", 12F);
            this.sqL_DataGridView_藥品搜尋.邊框樣式 = System.Windows.Forms.BorderStyle.None;
            this.sqL_DataGridView_藥品搜尋.顯示CheckBox = false;
            this.sqL_DataGridView_藥品搜尋.顯示首列 = true;
            this.sqL_DataGridView_藥品搜尋.顯示首行 = true;
            this.sqL_DataGridView_藥品搜尋.首列樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_藥品搜尋.首行樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            // 
            // rJ_Lable1
            // 
            this.rJ_Lable1.BackColor = System.Drawing.Color.White;
            this.rJ_Lable1.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable1.BorderColor = System.Drawing.Color.Black;
            this.rJ_Lable1.BorderRadius = 5;
            this.rJ_Lable1.BorderSize = 1;
            this.rJ_Lable1.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable1.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable1.ForeColor = System.Drawing.Color.White;
            this.rJ_Lable1.GUID = "";
            this.rJ_Lable1.Location = new System.Drawing.Point(752, 44);
            this.rJ_Lable1.Name = "rJ_Lable1";
            this.rJ_Lable1.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable1.ShadowSize = 0;
            this.rJ_Lable1.Size = new System.Drawing.Size(666, 86);
            this.rJ_Lable1.TabIndex = 30;
            this.rJ_Lable1.Text = "藥品組合";
            this.rJ_Lable1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable1.TextColor = System.Drawing.Color.Black;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rJ_Button_確認組合);
            this.panel3.Controls.Add(this.rJ_Button_刪除組合);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(752, 712);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(666, 100);
            this.panel3.TabIndex = 124;
            // 
            // rJ_Button_刪除組合
            // 
            this.rJ_Button_刪除組合.AutoResetState = false;
            this.rJ_Button_刪除組合.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_刪除組合.BackgroundColor = System.Drawing.Color.Red;
            this.rJ_Button_刪除組合.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_刪除組合.BorderRadius = 10;
            this.rJ_Button_刪除組合.BorderSize = 0;
            this.rJ_Button_刪除組合.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_刪除組合.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_刪除組合.FlatAppearance.BorderSize = 0;
            this.rJ_Button_刪除組合.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_刪除組合.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_刪除組合.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_刪除組合.GUID = "";
            this.rJ_Button_刪除組合.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_刪除組合.Location = new System.Drawing.Point(410, 19);
            this.rJ_Button_刪除組合.Name = "rJ_Button_刪除組合";
            this.rJ_Button_刪除組合.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_刪除組合.ProhibitionLineWidth = 4;
            this.rJ_Button_刪除組合.ProhibitionSymbolSize = 30;
            this.rJ_Button_刪除組合.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_刪除組合.ShadowSize = 3;
            this.rJ_Button_刪除組合.ShowLoadingForm = false;
            this.rJ_Button_刪除組合.Size = new System.Drawing.Size(119, 63);
            this.rJ_Button_刪除組合.State = false;
            this.rJ_Button_刪除組合.TabIndex = 12;
            this.rJ_Button_刪除組合.Text = "刪除";
            this.rJ_Button_刪除組合.TextColor = System.Drawing.Color.White;
            this.rJ_Button_刪除組合.TextHeight = 0;
            this.rJ_Button_刪除組合.UseVisualStyleBackColor = false;
            // 
            // rJ_Button_加入
            // 
            this.rJ_Button_加入.AutoResetState = false;
            this.rJ_Button_加入.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_加入.BackgroundColor = System.Drawing.Color.Black;
            this.rJ_Button_加入.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_加入.BorderRadius = 10;
            this.rJ_Button_加入.BorderSize = 0;
            this.rJ_Button_加入.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_加入.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_加入.FlatAppearance.BorderSize = 0;
            this.rJ_Button_加入.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_加入.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_加入.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_加入.GUID = "";
            this.rJ_Button_加入.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_加入.Location = new System.Drawing.Point(615, 19);
            this.rJ_Button_加入.Name = "rJ_Button_加入";
            this.rJ_Button_加入.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_加入.ProhibitionLineWidth = 4;
            this.rJ_Button_加入.ProhibitionSymbolSize = 30;
            this.rJ_Button_加入.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_加入.ShadowSize = 3;
            this.rJ_Button_加入.ShowLoadingForm = false;
            this.rJ_Button_加入.Size = new System.Drawing.Size(119, 63);
            this.rJ_Button_加入.State = false;
            this.rJ_Button_加入.TabIndex = 14;
            this.rJ_Button_加入.Text = "加入";
            this.rJ_Button_加入.TextColor = System.Drawing.Color.White;
            this.rJ_Button_加入.TextHeight = 0;
            this.rJ_Button_加入.UseVisualStyleBackColor = false;
            // 
            // rJ_Button_確認組合
            // 
            this.rJ_Button_確認組合.AutoResetState = false;
            this.rJ_Button_確認組合.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_確認組合.BackgroundColor = System.Drawing.Color.Black;
            this.rJ_Button_確認組合.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_確認組合.BorderRadius = 10;
            this.rJ_Button_確認組合.BorderSize = 0;
            this.rJ_Button_確認組合.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_確認組合.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_確認組合.FlatAppearance.BorderSize = 0;
            this.rJ_Button_確認組合.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_確認組合.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_確認組合.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_確認組合.GUID = "";
            this.rJ_Button_確認組合.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_確認組合.Location = new System.Drawing.Point(535, 19);
            this.rJ_Button_確認組合.Name = "rJ_Button_確認組合";
            this.rJ_Button_確認組合.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_確認組合.ProhibitionLineWidth = 4;
            this.rJ_Button_確認組合.ProhibitionSymbolSize = 30;
            this.rJ_Button_確認組合.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_確認組合.ShadowSize = 3;
            this.rJ_Button_確認組合.ShowLoadingForm = false;
            this.rJ_Button_確認組合.Size = new System.Drawing.Size(119, 63);
            this.rJ_Button_確認組合.State = false;
            this.rJ_Button_確認組合.TabIndex = 13;
            this.rJ_Button_確認組合.Text = "確認";
            this.rJ_Button_確認組合.TextColor = System.Drawing.Color.White;
            this.rJ_Button_確認組合.TextHeight = 0;
            this.rJ_Button_確認組合.UseVisualStyleBackColor = false;
            // 
            // Dialog_藥品組合
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CaptionHeight = 40;
            this.ClientSize = new System.Drawing.Size(1422, 816);
            this.ControlBox = true;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.rJ_Lable1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.Name = "Dialog_藥品組合";
            this.Text = "藥品組合";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private MyUI.RJ_TextBox textBox_搜尋內容;
        private MyUI.RJ_Button rJ_Button_搜尋;
        private System.Windows.Forms.ComboBox comboBox_搜尋條件;
        private SQLUI.SQL_DataGridView sqL_DataGridView_藥品搜尋;
        private MyUI.RJ_Lable rJ_Lable1;
        private System.Windows.Forms.Panel panel3;
        private MyUI.RJ_Button rJ_Button_刪除組合;
        private MyUI.RJ_Button rJ_Button_加入;
        private MyUI.RJ_Button rJ_Button_確認組合;
    }
}