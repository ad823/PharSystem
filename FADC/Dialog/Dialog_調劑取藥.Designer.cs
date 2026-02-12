namespace FADC
{
    partial class Dialog_調劑取藥
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
            this.sqL_DataGridView_處方藥品 = new SQLUI.SQL_DataGridView();
            this.rJ_Lable_狀態 = new MyUI.RJ_Lable();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rJ_Lable_狀態);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(4, 596);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1192, 100);
            this.panel1.TabIndex = 155;
            // 
            // sqL_DataGridView_處方藥品
            // 
            this.sqL_DataGridView_處方藥品.AutoSelectToDeep = false;
            this.sqL_DataGridView_處方藥品.backColor = System.Drawing.Color.Gainsboro;
            this.sqL_DataGridView_處方藥品.BorderColor = System.Drawing.Color.Transparent;
            this.sqL_DataGridView_處方藥品.BorderRadius = 0;
            this.sqL_DataGridView_處方藥品.BorderSize = 0;
            this.sqL_DataGridView_處方藥品.CellBorderColor = System.Drawing.Color.Gainsboro;
            this.sqL_DataGridView_處方藥品.cellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.sqL_DataGridView_處方藥品.cellStylBackColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_處方藥品.cellStyleFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.sqL_DataGridView_處方藥品.cellStylForeColor = System.Drawing.Color.Black;
            this.sqL_DataGridView_處方藥品.checkedRowBackColor = System.Drawing.Color.YellowGreen;
            this.sqL_DataGridView_處方藥品.columnHeaderBackColor = System.Drawing.Color.DarkGray;
            this.sqL_DataGridView_處方藥品.columnHeaderBorderColor = System.Drawing.Color.DimGray;
            this.sqL_DataGridView_處方藥品.columnHeaderFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.sqL_DataGridView_處方藥品.columnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_處方藥品.columnHeadersHeight = 40;
            this.sqL_DataGridView_處方藥品.columnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.sqL_DataGridView_處方藥品.DataGridViewAutoSizeColumnMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.NotSet;
            this.sqL_DataGridView_處方藥品.DataKeyEnable = true;
            this.sqL_DataGridView_處方藥品.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqL_DataGridView_處方藥品.Font = new System.Drawing.Font("新細明體", 9F);
            this.sqL_DataGridView_處方藥品.ImageBox = false;
            this.sqL_DataGridView_處方藥品.Location = new System.Drawing.Point(4, 34);
            this.sqL_DataGridView_處方藥品.Margin = new System.Windows.Forms.Padding(4);
            this.sqL_DataGridView_處方藥品.Name = "sqL_DataGridView_處方藥品";
            this.sqL_DataGridView_處方藥品.OnlineState = SQLUI.SQL_DataGridView.OnlineEnum.Online;
            this.sqL_DataGridView_處方藥品.Password = "user82822040";
            this.sqL_DataGridView_處方藥品.Port = ((uint)(3306u));
            this.sqL_DataGridView_處方藥品.rowBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            this.sqL_DataGridView_處方藥品.rowHeaderBackColor = System.Drawing.Color.Gray;
            this.sqL_DataGridView_處方藥品.rowHeaderBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            this.sqL_DataGridView_處方藥品.rowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_處方藥品.RowsColor = System.Drawing.SystemColors.Window;
            this.sqL_DataGridView_處方藥品.RowsHeight = 40;
            this.sqL_DataGridView_處方藥品.SaveFileName = "SQL_DataGridView";
            this.sqL_DataGridView_處方藥品.selectedBorderSize = 2;
            this.sqL_DataGridView_處方藥品.selectedRowBackColor = System.Drawing.Color.Blue;
            this.sqL_DataGridView_處方藥品.selectedRowBorderColor = System.Drawing.Color.Blue;
            this.sqL_DataGridView_處方藥品.selectedRowForeColor = System.Drawing.Color.Black;
            this.sqL_DataGridView_處方藥品.Server = "127.0.0.0";
            this.sqL_DataGridView_處方藥品.Size = new System.Drawing.Size(1192, 562);
            this.sqL_DataGridView_處方藥品.SSLMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            this.sqL_DataGridView_處方藥品.TabIndex = 156;
            this.sqL_DataGridView_處方藥品.UserName = "root";
            this.sqL_DataGridView_處方藥品.可拖曳欄位寬度 = false;
            this.sqL_DataGridView_處方藥品.可選擇多列 = false;
            this.sqL_DataGridView_處方藥品.單格樣式 = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.sqL_DataGridView_處方藥品.自動換行 = true;
            this.sqL_DataGridView_處方藥品.表單字體 = new System.Drawing.Font("新細明體", 9F);
            this.sqL_DataGridView_處方藥品.邊框樣式 = System.Windows.Forms.BorderStyle.None;
            this.sqL_DataGridView_處方藥品.顯示CheckBox = false;
            this.sqL_DataGridView_處方藥品.顯示首列 = true;
            this.sqL_DataGridView_處方藥品.顯示首行 = true;
            this.sqL_DataGridView_處方藥品.首列樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_處方藥品.首行樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            // 
            // rJ_Lable_狀態
            // 
            this.rJ_Lable_狀態.BackColor = System.Drawing.Color.White;
            this.rJ_Lable_狀態.BackgroundColor = System.Drawing.Color.White;
            this.rJ_Lable_狀態.BorderColor = System.Drawing.Color.White;
            this.rJ_Lable_狀態.BorderRadius = 20;
            this.rJ_Lable_狀態.BorderSize = 1;
            this.rJ_Lable_狀態.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rJ_Lable_狀態.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_狀態.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_狀態.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_狀態.GUID = "";
            this.rJ_Lable_狀態.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable_狀態.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable_狀態.Name = "rJ_Lable_狀態";
            this.rJ_Lable_狀態.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_狀態.ShadowSize = 0;
            this.rJ_Lable_狀態.Size = new System.Drawing.Size(1192, 100);
            this.rJ_Lable_狀態.TabIndex = 146;
            this.rJ_Lable_狀態.Text = "-----------";
            this.rJ_Lable_狀態.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable_狀態.TextColor = System.Drawing.Color.Black;
            // 
            // Dialog_調劑取藥
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CaptionHeight = 40;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.CloseBoxSize = new System.Drawing.Size(40, 40);
            this.ControlBox = true;
            this.Controls.Add(this.sqL_DataGridView_處方藥品);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MaxSize = new System.Drawing.Size(40, 40);
            this.MiniSize = new System.Drawing.Size(40, 40);
            this.Name = "Dialog_調劑取藥";
            this.Text = "調劑取藥";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private SQLUI.SQL_DataGridView sqL_DataGridView_處方藥品;
        private MyUI.RJ_Lable rJ_Lable_狀態;
    }
}