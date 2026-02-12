namespace FADC
{
    partial class Dialog_藥品組合選擇
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
     
            this.panel3 = new System.Windows.Forms.Panel();
            this.rJ_Lable_藥品資訊 = new MyUI.RJ_Lable();
            this.rJ_Button_確認 = new MyUI.RJ_Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rJ_Button_取消 = new MyUI.RJ_Button();
            sqL_DataGridView_藥品組合 = new SQLUI.SQL_DataGridView();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rJ_Button_取消);
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Controls.Add(this.rJ_Button_確認);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(4, 611);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1119, 103);
            this.panel3.TabIndex = 125;
            // 
            // rJ_Lable_藥品資訊
            // 
            this.rJ_Lable_藥品資訊.BackColor = System.Drawing.Color.White;
            this.rJ_Lable_藥品資訊.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥品資訊.BorderColor = System.Drawing.Color.Black;
            this.rJ_Lable_藥品資訊.BorderRadius = 5;
            this.rJ_Lable_藥品資訊.BorderSize = 1;
            this.rJ_Lable_藥品資訊.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable_藥品資訊.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_藥品資訊.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_藥品資訊.ForeColor = System.Drawing.Color.White;
            this.rJ_Lable_藥品資訊.GUID = "";
            this.rJ_Lable_藥品資訊.Location = new System.Drawing.Point(4, 44);
            this.rJ_Lable_藥品資訊.Name = "rJ_Lable_藥品資訊";
            this.rJ_Lable_藥品資訊.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_藥品資訊.ShadowSize = 0;
            this.rJ_Lable_藥品資訊.Size = new System.Drawing.Size(1119, 86);
            this.rJ_Lable_藥品資訊.TabIndex = 126;
            this.rJ_Lable_藥品資訊.Text = "(code) name";
            this.rJ_Lable_藥品資訊.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable_藥品資訊.TextColor = System.Drawing.Color.Black;
            // 
            // sqL_DataGridView_藥品組合
            // 
            sqL_DataGridView_藥品組合.AutoSelectToDeep = false;
            sqL_DataGridView_藥品組合.backColor = System.Drawing.Color.DarkGray;
            sqL_DataGridView_藥品組合.BorderColor = System.Drawing.Color.DarkGray;
            sqL_DataGridView_藥品組合.BorderRadius = 0;
            sqL_DataGridView_藥品組合.BorderSize = 2;
            sqL_DataGridView_藥品組合.CellBorderColor = System.Drawing.Color.Silver;
            sqL_DataGridView_藥品組合.cellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            sqL_DataGridView_藥品組合.cellStylBackColor = System.Drawing.Color.Silver;
            sqL_DataGridView_藥品組合.cellStyleFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            sqL_DataGridView_藥品組合.cellStylForeColor = System.Drawing.Color.Black;
            sqL_DataGridView_藥品組合.checkedRowBackColor = System.Drawing.Color.YellowGreen;
            sqL_DataGridView_藥品組合.columnHeaderBackColor = System.Drawing.SystemColors.Control;
            sqL_DataGridView_藥品組合.columnHeaderBorderColor = System.Drawing.Color.Gainsboro;
            sqL_DataGridView_藥品組合.columnHeaderFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            sqL_DataGridView_藥品組合.columnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            sqL_DataGridView_藥品組合.columnHeadersHeight = 18;
            sqL_DataGridView_藥品組合.columnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            sqL_DataGridView_藥品組合.DataGridViewAutoSizeColumnMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.NotSet;
            sqL_DataGridView_藥品組合.DataKeyEnable = false;
            sqL_DataGridView_藥品組合.Dock = System.Windows.Forms.DockStyle.Fill;
            sqL_DataGridView_藥品組合.Font = new System.Drawing.Font("新細明體", 12F);
            sqL_DataGridView_藥品組合.ImageBox = false;
            sqL_DataGridView_藥品組合.Location = new System.Drawing.Point(4, 130);
            sqL_DataGridView_藥品組合.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            sqL_DataGridView_藥品組合.Name = "sqL_DataGridView_藥品組合";
            sqL_DataGridView_藥品組合.OnlineState = SQLUI.SQL_DataGridView.OnlineEnum.Online;
            sqL_DataGridView_藥品組合.Password = "user82822040";
            sqL_DataGridView_藥品組合.Port = ((uint)(3306u));
            sqL_DataGridView_藥品組合.rowBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            sqL_DataGridView_藥品組合.rowHeaderBackColor = System.Drawing.Color.CornflowerBlue;
            sqL_DataGridView_藥品組合.rowHeaderBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            sqL_DataGridView_藥品組合.rowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            sqL_DataGridView_藥品組合.RowsColor = System.Drawing.SystemColors.ButtonHighlight;
            sqL_DataGridView_藥品組合.RowsHeight = 50;
            sqL_DataGridView_藥品組合.SaveFileName = "SQL_DataGridView";
            sqL_DataGridView_藥品組合.selectedBorderSize = 0;
            sqL_DataGridView_藥品組合.selectedRowBackColor = System.Drawing.Color.Blue;
            sqL_DataGridView_藥品組合.selectedRowBorderColor = System.Drawing.Color.Blue;
            sqL_DataGridView_藥品組合.selectedRowForeColor = System.Drawing.Color.White;
            sqL_DataGridView_藥品組合.Server = "localhost";
            sqL_DataGridView_藥品組合.Size = new System.Drawing.Size(1119, 481);
            sqL_DataGridView_藥品組合.SSLMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            sqL_DataGridView_藥品組合.TabIndex = 127;
            sqL_DataGridView_藥品組合.UserName = "root";
            sqL_DataGridView_藥品組合.可拖曳欄位寬度 = true;
            sqL_DataGridView_藥品組合.可選擇多列 = true;
            sqL_DataGridView_藥品組合.單格樣式 = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            sqL_DataGridView_藥品組合.自動換行 = true;
            sqL_DataGridView_藥品組合.表單字體 = new System.Drawing.Font("新細明體", 12F);
            sqL_DataGridView_藥品組合.邊框樣式 = System.Windows.Forms.BorderStyle.None;
            sqL_DataGridView_藥品組合.顯示CheckBox = false;
            sqL_DataGridView_藥品組合.顯示首列 = true;
            sqL_DataGridView_藥品組合.顯示首行 = true;
            sqL_DataGridView_藥品組合.首列樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            sqL_DataGridView_藥品組合.首行樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            // 
            // rJ_Button_確認
            // 
            this.rJ_Button_確認.AutoResetState = false;
            this.rJ_Button_確認.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_確認.BackgroundColor = System.Drawing.Color.Black;
            this.rJ_Button_確認.BorderColor = System.Drawing.Color.DimGray;
            this.rJ_Button_確認.BorderRadius = 22;
            this.rJ_Button_確認.BorderSize = 1;
            this.rJ_Button_確認.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_確認.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_確認.Dock = System.Windows.Forms.DockStyle.Right;
            this.rJ_Button_確認.FlatAppearance.BorderSize = 0;
            this.rJ_Button_確認.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_確認.Font = new System.Drawing.Font("微軟正黑體", 14.25F);
            this.rJ_Button_確認.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_確認.GUID = "";
            this.rJ_Button_確認.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_確認.Location = new System.Drawing.Point(993, 0);
            this.rJ_Button_確認.Name = "rJ_Button_確認";
            this.rJ_Button_確認.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_確認.ProhibitionLineWidth = 4;
            this.rJ_Button_確認.ProhibitionSymbolSize = 30;
            this.rJ_Button_確認.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_確認.ShadowSize = 0;
            this.rJ_Button_確認.ShowLoadingForm = false;
            this.rJ_Button_確認.Size = new System.Drawing.Size(126, 103);
            this.rJ_Button_確認.State = false;
            this.rJ_Button_確認.TabIndex = 165;
            this.rJ_Button_確認.Text = "確認";
            this.rJ_Button_確認.TextColor = System.Drawing.Color.White;
            this.rJ_Button_確認.TextHeight = 0;
            this.rJ_Button_確認.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(983, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(10, 103);
            this.panel1.TabIndex = 166;
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
            this.rJ_Button_取消.Location = new System.Drawing.Point(857, 0);
            this.rJ_Button_取消.Name = "rJ_Button_取消";
            this.rJ_Button_取消.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_取消.ProhibitionLineWidth = 4;
            this.rJ_Button_取消.ProhibitionSymbolSize = 30;
            this.rJ_Button_取消.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_取消.ShadowSize = 0;
            this.rJ_Button_取消.ShowLoadingForm = false;
            this.rJ_Button_取消.Size = new System.Drawing.Size(126, 103);
            this.rJ_Button_取消.State = false;
            this.rJ_Button_取消.TabIndex = 167;
            this.rJ_Button_取消.Text = "取消";
            this.rJ_Button_取消.TextColor = System.Drawing.Color.Black;
            this.rJ_Button_取消.TextHeight = 0;
            this.rJ_Button_取消.UseVisualStyleBackColor = false;
            // 
            // Dialog_藥品組合選擇
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CaptionHeight = 40;
            this.ClientSize = new System.Drawing.Size(1127, 718);
            this.Controls.Add(sqL_DataGridView_藥品組合);
            this.Controls.Add(this.rJ_Lable_藥品資訊);
            this.Controls.Add(this.panel3);
            this.Name = "Dialog_藥品組合選擇";
            this.Text = "藥品組合選擇";
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        SQLUI.SQL_DataGridView sqL_DataGridView_藥品組合;
        private System.Windows.Forms.Panel panel3;
        private MyUI.RJ_Lable rJ_Lable_藥品資訊;
        private MyUI.RJ_Button rJ_Button_取消;
        private System.Windows.Forms.Panel panel1;
        private MyUI.RJ_Button rJ_Button_確認;
    }
}