namespace FADC
{
    partial class Dialog_單品入庫作業
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
            this.stepViewer1 = new MyUI.StepViewer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rJ_Button_取消 = new MyUI.RJ_Button();
            this.tabControlEx = new MyUI.TabControlEx();
            this.藥品搜尋 = new System.Windows.Forms.TabPage();
            this.批號效期選擇 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rJ_Lable_藥品資訊 = new MyUI.RJ_Lable();
            this.panel4 = new System.Windows.Forms.Panel();
            this.sqL_DataGridView_藥品資料 = new SQLUI.SQL_DataGridView();
            this.textBox_搜尋內容 = new MyUI.RJ_TextBox();
            this.rJ_Button_搜尋 = new MyUI.RJ_Button();
            this.panel1.SuspendLayout();
            this.tabControlEx.SuspendLayout();
            this.藥品搜尋.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // stepViewer1
            // 
            this.stepViewer1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.stepViewer1.CurrentStep = 0;
            this.stepViewer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stepViewer1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.stepViewer1.LineWidth = 60;
            this.stepViewer1.ListDataSource = null;
            this.stepViewer1.Location = new System.Drawing.Point(4, 44);
            this.stepViewer1.Margin = new System.Windows.Forms.Padding(4);
            this.stepViewer1.Name = "stepViewer1";
            this.stepViewer1.Size = new System.Drawing.Size(1398, 91);
            this.stepViewer1.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rJ_Button_取消);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(4, 758);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1398, 82);
            this.panel1.TabIndex = 9;
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
            this.rJ_Button_取消.Location = new System.Drawing.Point(1272, 0);
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
            // tabControlEx
            // 
            this.tabControlEx.Controls.Add(this.藥品搜尋);
            this.tabControlEx.Controls.Add(this.批號效期選擇);
            this.tabControlEx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlEx.ItemSize = new System.Drawing.Size(0, 1);
            this.tabControlEx.Location = new System.Drawing.Point(4, 135);
            this.tabControlEx.Multiline = true;
            this.tabControlEx.Name = "tabControlEx";
            this.tabControlEx.SelectedIndex = 0;
            this.tabControlEx.Size = new System.Drawing.Size(1398, 623);
            this.tabControlEx.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlEx.TabIndex = 10;
            // 
            // 藥品搜尋
            // 
            this.藥品搜尋.BackColor = System.Drawing.Color.White;
            this.藥品搜尋.Controls.Add(this.panel3);
            this.藥品搜尋.Controls.Add(this.panel2);
            this.藥品搜尋.Location = new System.Drawing.Point(4, 5);
            this.藥品搜尋.Name = "藥品搜尋";
            this.藥品搜尋.Size = new System.Drawing.Size(1390, 614);
            this.藥品搜尋.TabIndex = 0;
            this.藥品搜尋.Text = "藥品搜尋";
            // 
            // 批號效期選擇
            // 
            this.批號效期選擇.BackColor = System.Drawing.Color.White;
            this.批號效期選擇.Location = new System.Drawing.Point(4, 5);
            this.批號效期選擇.Name = "批號效期選擇";
            this.批號效期選擇.Size = new System.Drawing.Size(1208, 541);
            this.批號效期選擇.TabIndex = 1;
            this.批號效期選擇.Text = "批號效期選擇";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox_搜尋內容);
            this.panel2.Controls.Add(this.rJ_Button_搜尋);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(682, 614);
            this.panel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rJ_Lable_藥品資訊);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(682, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(708, 614);
            this.panel3.TabIndex = 1;
            // 
            // rJ_Lable_藥品資訊
            // 
            this.rJ_Lable_藥品資訊.BackColor = System.Drawing.Color.White;
            this.rJ_Lable_藥品資訊.BackgroundColor = System.Drawing.Color.White;
            this.rJ_Lable_藥品資訊.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_藥品資訊.BorderRadius = 10;
            this.rJ_Lable_藥品資訊.BorderSize = 0;
            this.rJ_Lable_藥品資訊.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable_藥品資訊.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_藥品資訊.Font = new System.Drawing.Font("微軟正黑體", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_藥品資訊.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥品資訊.GUID = "";
            this.rJ_Lable_藥品資訊.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable_藥品資訊.Name = "rJ_Lable_藥品資訊";
            this.rJ_Lable_藥品資訊.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_藥品資訊.ShadowSize = 0;
            this.rJ_Lable_藥品資訊.Size = new System.Drawing.Size(708, 85);
            this.rJ_Lable_藥品資訊.TabIndex = 38;
            this.rJ_Lable_藥品資訊.Text = "(------) --------------------------------";
            this.rJ_Lable_藥品資訊.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable_藥品資訊.TextColor = System.Drawing.Color.Black;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.sqL_DataGridView_藥品資料);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(3);
            this.panel4.Size = new System.Drawing.Size(682, 524);
            this.panel4.TabIndex = 0;
            // 
            // sqL_DataGridView_藥品資料
            // 
            this.sqL_DataGridView_藥品資料.AutoSelectToDeep = false;
            this.sqL_DataGridView_藥品資料.backColor = System.Drawing.Color.DarkGray;
            this.sqL_DataGridView_藥品資料.BorderColor = System.Drawing.Color.DarkGray;
            this.sqL_DataGridView_藥品資料.BorderRadius = 0;
            this.sqL_DataGridView_藥品資料.BorderSize = 2;
            this.sqL_DataGridView_藥品資料.CellBorderColor = System.Drawing.Color.Silver;
            this.sqL_DataGridView_藥品資料.cellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.sqL_DataGridView_藥品資料.cellStylBackColor = System.Drawing.Color.Silver;
            this.sqL_DataGridView_藥品資料.cellStyleFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.sqL_DataGridView_藥品資料.cellStylForeColor = System.Drawing.Color.Black;
            this.sqL_DataGridView_藥品資料.checkedRowBackColor = System.Drawing.Color.YellowGreen;
            this.sqL_DataGridView_藥品資料.columnHeaderBackColor = System.Drawing.SystemColors.Control;
            this.sqL_DataGridView_藥品資料.columnHeaderBorderColor = System.Drawing.Color.Gainsboro;
            this.sqL_DataGridView_藥品資料.columnHeaderFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.sqL_DataGridView_藥品資料.columnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_藥品資料.columnHeadersHeight = 18;
            this.sqL_DataGridView_藥品資料.columnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sqL_DataGridView_藥品資料.DataGridViewAutoSizeColumnMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.NotSet;
            this.sqL_DataGridView_藥品資料.DataKeyEnable = false;
            this.sqL_DataGridView_藥品資料.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqL_DataGridView_藥品資料.Enabled = false;
            this.sqL_DataGridView_藥品資料.Font = new System.Drawing.Font("新細明體", 12F);
            this.sqL_DataGridView_藥品資料.ImageBox = false;
            this.sqL_DataGridView_藥品資料.Location = new System.Drawing.Point(3, 3);
            this.sqL_DataGridView_藥品資料.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.sqL_DataGridView_藥品資料.Name = "sqL_DataGridView_藥品資料";
            this.sqL_DataGridView_藥品資料.OnlineState = SQLUI.SQL_DataGridView.OnlineEnum.Online;
            this.sqL_DataGridView_藥品資料.Password = "user82822040";
            this.sqL_DataGridView_藥品資料.Port = ((uint)(3306u));
            this.sqL_DataGridView_藥品資料.rowBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            this.sqL_DataGridView_藥品資料.rowHeaderBackColor = System.Drawing.Color.CornflowerBlue;
            this.sqL_DataGridView_藥品資料.rowHeaderBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            this.sqL_DataGridView_藥品資料.rowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_藥品資料.RowsColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sqL_DataGridView_藥品資料.RowsHeight = 50;
            this.sqL_DataGridView_藥品資料.SaveFileName = "SQL_DataGridView";
            this.sqL_DataGridView_藥品資料.selectedBorderSize = 0;
            this.sqL_DataGridView_藥品資料.selectedRowBackColor = System.Drawing.Color.Blue;
            this.sqL_DataGridView_藥品資料.selectedRowBorderColor = System.Drawing.Color.Blue;
            this.sqL_DataGridView_藥品資料.selectedRowForeColor = System.Drawing.Color.White;
            this.sqL_DataGridView_藥品資料.Server = "localhost";
            this.sqL_DataGridView_藥品資料.Size = new System.Drawing.Size(676, 518);
            this.sqL_DataGridView_藥品資料.SSLMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            this.sqL_DataGridView_藥品資料.TabIndex = 128;
            this.sqL_DataGridView_藥品資料.UserName = "root";
            this.sqL_DataGridView_藥品資料.可拖曳欄位寬度 = true;
            this.sqL_DataGridView_藥品資料.可選擇多列 = true;
            this.sqL_DataGridView_藥品資料.單格樣式 = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.sqL_DataGridView_藥品資料.自動換行 = true;
            this.sqL_DataGridView_藥品資料.表單字體 = new System.Drawing.Font("新細明體", 12F);
            this.sqL_DataGridView_藥品資料.邊框樣式 = System.Windows.Forms.BorderStyle.None;
            this.sqL_DataGridView_藥品資料.顯示CheckBox = false;
            this.sqL_DataGridView_藥品資料.顯示首列 = true;
            this.sqL_DataGridView_藥品資料.顯示首行 = true;
            this.sqL_DataGridView_藥品資料.首列樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_藥品資料.首行樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
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
            this.textBox_搜尋內容.Location = new System.Drawing.Point(46, 552);
            this.textBox_搜尋內容.Multiline = false;
            this.textBox_搜尋內容.Name = "textBox_搜尋內容";
            this.textBox_搜尋內容.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.textBox_搜尋內容.PassWordChar = false;
            this.textBox_搜尋內容.PlaceholderColor = System.Drawing.Color.Silver;
            this.textBox_搜尋內容.PlaceholderText = "請輸入藥名";
            this.textBox_搜尋內容.ShowTouchPannel = false;
            this.textBox_搜尋內容.Size = new System.Drawing.Size(393, 37);
            this.textBox_搜尋內容.TabIndex = 15;
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
            this.rJ_Button_搜尋.BorderRadius = 20;
            this.rJ_Button_搜尋.BorderSize = 0;
            this.rJ_Button_搜尋.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_搜尋.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_搜尋.FlatAppearance.BorderSize = 0;
            this.rJ_Button_搜尋.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_搜尋.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_搜尋.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_搜尋.GUID = "";
            this.rJ_Button_搜尋.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_搜尋.Location = new System.Drawing.Point(458, 537);
            this.rJ_Button_搜尋.Name = "rJ_Button_搜尋";
            this.rJ_Button_搜尋.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_搜尋.ProhibitionLineWidth = 4;
            this.rJ_Button_搜尋.ProhibitionSymbolSize = 30;
            this.rJ_Button_搜尋.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_搜尋.ShadowSize = 3;
            this.rJ_Button_搜尋.ShowLoadingForm = false;
            this.rJ_Button_搜尋.Size = new System.Drawing.Size(119, 63);
            this.rJ_Button_搜尋.State = false;
            this.rJ_Button_搜尋.TabIndex = 14;
            this.rJ_Button_搜尋.Text = "搜尋";
            this.rJ_Button_搜尋.TextColor = System.Drawing.Color.White;
            this.rJ_Button_搜尋.TextHeight = 0;
            this.rJ_Button_搜尋.UseVisualStyleBackColor = false;
            // 
            // Dialog_單品入庫作業
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CaptionHeight = 40;
            this.ClientSize = new System.Drawing.Size(1406, 844);
            this.Controls.Add(this.tabControlEx);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.stepViewer1);
            this.Name = "Dialog_單品入庫作業";
            this.Text = "入庫作業";
            this.panel1.ResumeLayout(false);
            this.tabControlEx.ResumeLayout(false);
            this.藥品搜尋.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MyUI.StepViewer stepViewer1;
        private System.Windows.Forms.Panel panel1;
        private MyUI.RJ_Button rJ_Button_取消;
        private MyUI.TabControlEx tabControlEx;
        private System.Windows.Forms.TabPage 藥品搜尋;
        private System.Windows.Forms.TabPage 批號效期選擇;
        private System.Windows.Forms.Panel panel3;
        private MyUI.RJ_Lable rJ_Lable_藥品資訊;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private SQLUI.SQL_DataGridView sqL_DataGridView_藥品資料;
        private MyUI.RJ_TextBox textBox_搜尋內容;
        private MyUI.RJ_Button rJ_Button_搜尋;
    }
}