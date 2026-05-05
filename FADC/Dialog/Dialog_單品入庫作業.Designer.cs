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
            this.panel5 = new System.Windows.Forms.Panel();
            this.stepViewer1 = new MyUI.StepViewer();
            this.rJ_Button_取消 = new MyUI.RJ_Button();
            this.panel_下一步 = new System.Windows.Forms.Panel();
            this.rJ_Button_下一步 = new MyUI.RJ_Button();
            this.tabControlEx = new MyUI.TabControlEx();
            this.藥品搜尋 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rJ_Lable_藥品資訊_藥名 = new MyUI.RJ_Lable();
            this.pictureBox_藥品圖片 = new System.Windows.Forms.PictureBox();
            this.rJ_Lable_藥品資訊_藥碼 = new MyUI.RJ_Lable();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBox_藥品搜尋種類 = new System.Windows.Forms.ComboBox();
            this.textBox_藥品搜尋內容 = new MyUI.RJ_TextBox();
            this.rJ_Button_藥品搜尋 = new MyUI.RJ_Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.sqL_DataGridView_藥品資料 = new SQLUI.SQL_DataGridView();
            this.儲位選擇 = new System.Windows.Forms.TabPage();
            this.sqL_DataGridView_儲位選擇 = new SQLUI.SQL_DataGridView();
            this.rJ_Lable_儲位選擇_藥品資訊 = new MyUI.RJ_Lable();
            this.rJ_Lable1 = new MyUI.RJ_Lable();
            this.效期批號輸入 = new System.Windows.Forms.TabPage();
            this.panel6 = new System.Windows.Forms.Panel();
            this.userControl_NumPanel1 = new MyUI.UserControl_NumPanel();
            this.rJ_Lable2 = new MyUI.RJ_Lable();
            this.batchExpiryControl = new FADC.BatchExpiryControl();
            this.確認結果 = new System.Windows.Forms.TabPage();
            this.rJ_Button_確認 = new MyUI.RJ_Button();
            this.panel12 = new System.Windows.Forms.Panel();
            this.rJ_Lable_數量 = new MyUI.RJ_Lable();
            this.rJ_Lable13 = new MyUI.RJ_Lable();
            this.panel11 = new System.Windows.Forms.Panel();
            this.rJ_Lable_批號 = new MyUI.RJ_Lable();
            this.rJ_Lable11 = new MyUI.RJ_Lable();
            this.panel10 = new System.Windows.Forms.Panel();
            this.rJ_Lable_效期 = new MyUI.RJ_Lable();
            this.rJ_Lable9 = new MyUI.RJ_Lable();
            this.panel9 = new System.Windows.Forms.Panel();
            this.rJ_Lable_藥名 = new MyUI.RJ_Lable();
            this.rJ_Lable7 = new MyUI.RJ_Lable();
            this.panel8 = new System.Windows.Forms.Panel();
            this.rJ_Lable_藥碼 = new MyUI.RJ_Lable();
            this.rJ_Lable4 = new MyUI.RJ_Lable();
            this.panel7 = new System.Windows.Forms.Panel();
            this.rJ_Lable3 = new MyUI.RJ_Lable();
            this.panel5.SuspendLayout();
            this.panel_下一步.SuspendLayout();
            this.tabControlEx.SuspendLayout();
            this.藥品搜尋.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_藥品圖片)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.儲位選擇.SuspendLayout();
            this.效期批號輸入.SuspendLayout();
            this.panel6.SuspendLayout();
            this.確認結果.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.stepViewer1);
            this.panel5.Controls.Add(this.rJ_Button_取消);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(4, 44);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1398, 77);
            this.panel5.TabIndex = 11;
            // 
            // stepViewer1
            // 
            this.stepViewer1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.stepViewer1.CurrentStep = 0;
            this.stepViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stepViewer1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.stepViewer1.LineWidth = 60;
            this.stepViewer1.ListDataSource = null;
            this.stepViewer1.Location = new System.Drawing.Point(0, 0);
            this.stepViewer1.Margin = new System.Windows.Forms.Padding(4);
            this.stepViewer1.Name = "stepViewer1";
            this.stepViewer1.Size = new System.Drawing.Size(1272, 77);
            this.stepViewer1.TabIndex = 168;
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
            this.rJ_Button_取消.Size = new System.Drawing.Size(126, 77);
            this.rJ_Button_取消.State = false;
            this.rJ_Button_取消.TabIndex = 165;
            this.rJ_Button_取消.Text = "取消";
            this.rJ_Button_取消.TextColor = System.Drawing.Color.Black;
            this.rJ_Button_取消.TextHeight = 0;
            this.rJ_Button_取消.UseVisualStyleBackColor = false;
            // 
            // panel_下一步
            // 
            this.panel_下一步.Controls.Add(this.rJ_Button_下一步);
            this.panel_下一步.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_下一步.Location = new System.Drawing.Point(4, 740);
            this.panel_下一步.Name = "panel_下一步";
            this.panel_下一步.Size = new System.Drawing.Size(1398, 100);
            this.panel_下一步.TabIndex = 12;
            // 
            // rJ_Button_下一步
            // 
            this.rJ_Button_下一步.AutoResetState = false;
            this.rJ_Button_下一步.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_下一步.BackgroundColor = System.Drawing.Color.Black;
            this.rJ_Button_下一步.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_下一步.BorderRadius = 20;
            this.rJ_Button_下一步.BorderSize = 0;
            this.rJ_Button_下一步.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_下一步.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_下一步.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rJ_Button_下一步.Enabled = false;
            this.rJ_Button_下一步.FlatAppearance.BorderSize = 0;
            this.rJ_Button_下一步.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_下一步.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_下一步.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_下一步.GUID = "";
            this.rJ_Button_下一步.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_下一步.Location = new System.Drawing.Point(0, 0);
            this.rJ_Button_下一步.Name = "rJ_Button_下一步";
            this.rJ_Button_下一步.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_下一步.ProhibitionLineWidth = 4;
            this.rJ_Button_下一步.ProhibitionSymbolSize = 30;
            this.rJ_Button_下一步.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_下一步.ShadowSize = 3;
            this.rJ_Button_下一步.ShowLoadingForm = false;
            this.rJ_Button_下一步.Size = new System.Drawing.Size(1398, 100);
            this.rJ_Button_下一步.State = false;
            this.rJ_Button_下一步.TabIndex = 167;
            this.rJ_Button_下一步.Text = "下一步";
            this.rJ_Button_下一步.TextColor = System.Drawing.Color.White;
            this.rJ_Button_下一步.TextHeight = 0;
            this.rJ_Button_下一步.UseVisualStyleBackColor = false;
            // 
            // tabControlEx
            // 
            this.tabControlEx.Controls.Add(this.藥品搜尋);
            this.tabControlEx.Controls.Add(this.儲位選擇);
            this.tabControlEx.Controls.Add(this.效期批號輸入);
            this.tabControlEx.Controls.Add(this.確認結果);
            this.tabControlEx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlEx.Location = new System.Drawing.Point(4, 121);
            this.tabControlEx.Name = "tabControlEx";
            this.tabControlEx.SelectedIndex = 0;
            this.tabControlEx.Size = new System.Drawing.Size(1398, 619);
            this.tabControlEx.TabIndex = 16;
            // 
            // 藥品搜尋
            // 
            this.藥品搜尋.BackColor = System.Drawing.Color.White;
            this.藥品搜尋.Controls.Add(this.panel3);
            this.藥品搜尋.Controls.Add(this.panel2);
            this.藥品搜尋.Location = new System.Drawing.Point(4, 22);
            this.藥品搜尋.Name = "藥品搜尋";
            this.藥品搜尋.Size = new System.Drawing.Size(1390, 593);
            this.藥品搜尋.TabIndex = 0;
            this.藥品搜尋.Text = "藥品搜尋";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.rJ_Lable_藥品資訊_藥名);
            this.panel3.Controls.Add(this.pictureBox_藥品圖片);
            this.panel3.Controls.Add(this.rJ_Lable_藥品資訊_藥碼);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(682, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(708, 593);
            this.panel3.TabIndex = 2;
            // 
            // rJ_Lable_藥品資訊_藥名
            // 
            this.rJ_Lable_藥品資訊_藥名.BackColor = System.Drawing.Color.White;
            this.rJ_Lable_藥品資訊_藥名.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥品資訊_藥名.BorderColor = System.Drawing.Color.Black;
            this.rJ_Lable_藥品資訊_藥名.BorderRadius = 10;
            this.rJ_Lable_藥品資訊_藥名.BorderSize = 1;
            this.rJ_Lable_藥品資訊_藥名.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable_藥品資訊_藥名.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_藥品資訊_藥名.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_藥品資訊_藥名.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥品資訊_藥名.GUID = "";
            this.rJ_Lable_藥品資訊_藥名.Location = new System.Drawing.Point(0, 73);
            this.rJ_Lable_藥品資訊_藥名.Name = "rJ_Lable_藥品資訊_藥名";
            this.rJ_Lable_藥品資訊_藥名.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_藥品資訊_藥名.ShadowSize = 0;
            this.rJ_Lable_藥品資訊_藥名.Size = new System.Drawing.Size(706, 73);
            this.rJ_Lable_藥品資訊_藥名.TabIndex = 42;
            this.rJ_Lable_藥品資訊_藥名.Text = "--------------------------------";
            this.rJ_Lable_藥品資訊_藥名.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable_藥品資訊_藥名.TextColor = System.Drawing.Color.Black;
            // 
            // pictureBox_藥品圖片
            // 
            this.pictureBox_藥品圖片.BackColor = System.Drawing.Color.Gainsboro;
            this.pictureBox_藥品圖片.Location = new System.Drawing.Point(85, 159);
            this.pictureBox_藥品圖片.Name = "pictureBox_藥品圖片";
            this.pictureBox_藥品圖片.Size = new System.Drawing.Size(564, 414);
            this.pictureBox_藥品圖片.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_藥品圖片.TabIndex = 40;
            this.pictureBox_藥品圖片.TabStop = false;
            // 
            // rJ_Lable_藥品資訊_藥碼
            // 
            this.rJ_Lable_藥品資訊_藥碼.BackColor = System.Drawing.Color.White;
            this.rJ_Lable_藥品資訊_藥碼.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.rJ_Lable_藥品資訊_藥碼.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_藥品資訊_藥碼.BorderRadius = 30;
            this.rJ_Lable_藥品資訊_藥碼.BorderSize = 0;
            this.rJ_Lable_藥品資訊_藥碼.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable_藥品資訊_藥碼.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_藥品資訊_藥碼.Font = new System.Drawing.Font("微軟正黑體", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_藥品資訊_藥碼.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥品資訊_藥碼.GUID = "";
            this.rJ_Lable_藥品資訊_藥碼.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable_藥品資訊_藥碼.Name = "rJ_Lable_藥品資訊_藥碼";
            this.rJ_Lable_藥品資訊_藥碼.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_藥品資訊_藥碼.ShadowSize = 0;
            this.rJ_Lable_藥品資訊_藥碼.Size = new System.Drawing.Size(706, 73);
            this.rJ_Lable_藥品資訊_藥碼.TabIndex = 38;
            this.rJ_Lable_藥品資訊_藥碼.Text = "-----";
            this.rJ_Lable_藥品資訊_藥碼.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable_藥品資訊_藥碼.TextColor = System.Drawing.Color.Black;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.comboBox_藥品搜尋種類);
            this.panel2.Controls.Add(this.textBox_藥品搜尋內容);
            this.panel2.Controls.Add(this.rJ_Button_藥品搜尋);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(682, 593);
            this.panel2.TabIndex = 1;
            // 
            // comboBox_藥品搜尋種類
            // 
            this.comboBox_藥品搜尋種類.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_藥品搜尋種類.FormattingEnabled = true;
            this.comboBox_藥品搜尋種類.Items.AddRange(new object[] {
            "全部顯示",
            "藥碼",
            "藥名"});
            this.comboBox_藥品搜尋種類.Location = new System.Drawing.Point(8, 644);
            this.comboBox_藥品搜尋種類.Name = "comboBox_藥品搜尋種類";
            this.comboBox_藥品搜尋種類.Size = new System.Drawing.Size(121, 32);
            this.comboBox_藥品搜尋種類.TabIndex = 40;
            // 
            // textBox_藥品搜尋內容
            // 
            this.textBox_藥品搜尋內容.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_藥品搜尋內容.BorderColor = System.Drawing.Color.Black;
            this.textBox_藥品搜尋內容.BorderFocusColor = System.Drawing.Color.HotPink;
            this.textBox_藥品搜尋內容.BorderRadius = 0;
            this.textBox_藥品搜尋內容.BorderSize = 2;
            this.textBox_藥品搜尋內容.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_藥品搜尋內容.ForeColor = System.Drawing.Color.DimGray;
            this.textBox_藥品搜尋內容.GUID = "";
            this.textBox_藥品搜尋內容.Location = new System.Drawing.Point(135, 641);
            this.textBox_藥品搜尋內容.Multiline = false;
            this.textBox_藥品搜尋內容.Name = "textBox_藥品搜尋內容";
            this.textBox_藥品搜尋內容.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.textBox_藥品搜尋內容.PassWordChar = false;
            this.textBox_藥品搜尋內容.PlaceholderColor = System.Drawing.Color.Silver;
            this.textBox_藥品搜尋內容.PlaceholderText = "請輸入藥名";
            this.textBox_藥品搜尋內容.ShowTouchPannel = false;
            this.textBox_藥品搜尋內容.Size = new System.Drawing.Size(393, 37);
            this.textBox_藥品搜尋內容.TabIndex = 15;
            this.textBox_藥品搜尋內容.TextAlgin = System.Windows.Forms.HorizontalAlignment.Left;
            this.textBox_藥品搜尋內容.Texts = "";
            this.textBox_藥品搜尋內容.UnderlineStyle = false;
            // 
            // rJ_Button_藥品搜尋
            // 
            this.rJ_Button_藥品搜尋.AutoResetState = false;
            this.rJ_Button_藥品搜尋.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_藥品搜尋.BackgroundColor = System.Drawing.Color.Black;
            this.rJ_Button_藥品搜尋.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_藥品搜尋.BorderRadius = 20;
            this.rJ_Button_藥品搜尋.BorderSize = 0;
            this.rJ_Button_藥品搜尋.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_藥品搜尋.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_藥品搜尋.FlatAppearance.BorderSize = 0;
            this.rJ_Button_藥品搜尋.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_藥品搜尋.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_藥品搜尋.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_藥品搜尋.GUID = "";
            this.rJ_Button_藥品搜尋.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_藥品搜尋.Location = new System.Drawing.Point(547, 626);
            this.rJ_Button_藥品搜尋.Name = "rJ_Button_藥品搜尋";
            this.rJ_Button_藥品搜尋.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_藥品搜尋.ProhibitionLineWidth = 4;
            this.rJ_Button_藥品搜尋.ProhibitionSymbolSize = 30;
            this.rJ_Button_藥品搜尋.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_藥品搜尋.ShadowSize = 3;
            this.rJ_Button_藥品搜尋.ShowLoadingForm = false;
            this.rJ_Button_藥品搜尋.Size = new System.Drawing.Size(119, 63);
            this.rJ_Button_藥品搜尋.State = false;
            this.rJ_Button_藥品搜尋.TabIndex = 14;
            this.rJ_Button_藥品搜尋.Text = "搜尋";
            this.rJ_Button_藥品搜尋.TextColor = System.Drawing.Color.White;
            this.rJ_Button_藥品搜尋.TextHeight = 0;
            this.rJ_Button_藥品搜尋.UseVisualStyleBackColor = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.sqL_DataGridView_藥品資料);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(3);
            this.panel4.Size = new System.Drawing.Size(682, 620);
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
            this.sqL_DataGridView_藥品資料.Size = new System.Drawing.Size(676, 614);
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
            // 儲位選擇
            // 
            this.儲位選擇.BackColor = System.Drawing.Color.White;
            this.儲位選擇.Controls.Add(this.sqL_DataGridView_儲位選擇);
            this.儲位選擇.Controls.Add(this.rJ_Lable_儲位選擇_藥品資訊);
            this.儲位選擇.Controls.Add(this.rJ_Lable1);
            this.儲位選擇.Location = new System.Drawing.Point(4, 22);
            this.儲位選擇.Name = "儲位選擇";
            this.儲位選擇.Size = new System.Drawing.Size(1390, 593);
            this.儲位選擇.TabIndex = 1;
            this.儲位選擇.Text = "儲位選擇";
            // 
            // sqL_DataGridView_儲位選擇
            // 
            this.sqL_DataGridView_儲位選擇.AutoSelectToDeep = true;
            this.sqL_DataGridView_儲位選擇.backColor = System.Drawing.Color.Silver;
            this.sqL_DataGridView_儲位選擇.BorderColor = System.Drawing.Color.Silver;
            this.sqL_DataGridView_儲位選擇.BorderRadius = 0;
            this.sqL_DataGridView_儲位選擇.BorderSize = 2;
            this.sqL_DataGridView_儲位選擇.CellBorderColor = System.Drawing.Color.White;
            this.sqL_DataGridView_儲位選擇.cellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.sqL_DataGridView_儲位選擇.cellStylBackColor = System.Drawing.Color.PowderBlue;
            this.sqL_DataGridView_儲位選擇.cellStyleFont = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.sqL_DataGridView_儲位選擇.cellStylForeColor = System.Drawing.Color.Black;
            this.sqL_DataGridView_儲位選擇.checkedRowBackColor = System.Drawing.Color.YellowGreen;
            this.sqL_DataGridView_儲位選擇.columnHeaderBackColor = System.Drawing.Color.DarkGray;
            this.sqL_DataGridView_儲位選擇.columnHeaderBorderColor = System.Drawing.Color.DimGray;
            this.sqL_DataGridView_儲位選擇.columnHeaderFont = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.sqL_DataGridView_儲位選擇.columnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.sqL_DataGridView_儲位選擇.columnHeadersHeight = 40;
            this.sqL_DataGridView_儲位選擇.columnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.sqL_DataGridView_儲位選擇.DataGridViewAutoSizeColumnMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.sqL_DataGridView_儲位選擇.DataKeyEnable = false;
            this.sqL_DataGridView_儲位選擇.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqL_DataGridView_儲位選擇.Font = new System.Drawing.Font("新細明體", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.sqL_DataGridView_儲位選擇.ImageBox = false;
            this.sqL_DataGridView_儲位選擇.Location = new System.Drawing.Point(0, 186);
            this.sqL_DataGridView_儲位選擇.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.sqL_DataGridView_儲位選擇.Name = "sqL_DataGridView_儲位選擇";
            this.sqL_DataGridView_儲位選擇.OnlineState = SQLUI.SQL_DataGridView.OnlineEnum.Online;
            this.sqL_DataGridView_儲位選擇.Password = "user82822040";
            this.sqL_DataGridView_儲位選擇.Port = ((uint)(3306u));
            this.sqL_DataGridView_儲位選擇.rowBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            this.sqL_DataGridView_儲位選擇.rowHeaderBackColor = System.Drawing.Color.Gray;
            this.sqL_DataGridView_儲位選擇.rowHeaderBorderStyleOption = SQLUI.SQL_DataGridView.RowBorderStyleOption.All;
            this.sqL_DataGridView_儲位選擇.rowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.sqL_DataGridView_儲位選擇.RowsColor = System.Drawing.SystemColors.Control;
            this.sqL_DataGridView_儲位選擇.RowsHeight = 80;
            this.sqL_DataGridView_儲位選擇.SaveFileName = "SQL_DataGridView";
            this.sqL_DataGridView_儲位選擇.selectedBorderSize = 0;
            this.sqL_DataGridView_儲位選擇.selectedRowBackColor = System.Drawing.Color.Blue;
            this.sqL_DataGridView_儲位選擇.selectedRowBorderColor = System.Drawing.Color.Blue;
            this.sqL_DataGridView_儲位選擇.selectedRowForeColor = System.Drawing.Color.White;
            this.sqL_DataGridView_儲位選擇.Server = "127.0.0.0";
            this.sqL_DataGridView_儲位選擇.Size = new System.Drawing.Size(1390, 407);
            this.sqL_DataGridView_儲位選擇.SSLMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            this.sqL_DataGridView_儲位選擇.TabIndex = 20;
            this.sqL_DataGridView_儲位選擇.UserName = "root";
            this.sqL_DataGridView_儲位選擇.可拖曳欄位寬度 = false;
            this.sqL_DataGridView_儲位選擇.可選擇多列 = false;
            this.sqL_DataGridView_儲位選擇.單格樣式 = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.sqL_DataGridView_儲位選擇.自動換行 = true;
            this.sqL_DataGridView_儲位選擇.表單字體 = new System.Drawing.Font("新細明體", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.sqL_DataGridView_儲位選擇.邊框樣式 = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sqL_DataGridView_儲位選擇.顯示CheckBox = false;
            this.sqL_DataGridView_儲位選擇.顯示首列 = false;
            this.sqL_DataGridView_儲位選擇.顯示首行 = false;
            this.sqL_DataGridView_儲位選擇.首列樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.sqL_DataGridView_儲位選擇.首行樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            // 
            // rJ_Lable_儲位選擇_藥品資訊
            // 
            this.rJ_Lable_儲位選擇_藥品資訊.BackColor = System.Drawing.Color.White;
            this.rJ_Lable_儲位選擇_藥品資訊.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_儲位選擇_藥品資訊.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_儲位選擇_藥品資訊.BorderRadius = 10;
            this.rJ_Lable_儲位選擇_藥品資訊.BorderSize = 0;
            this.rJ_Lable_儲位選擇_藥品資訊.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable_儲位選擇_藥品資訊.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_儲位選擇_藥品資訊.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_儲位選擇_藥品資訊.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_儲位選擇_藥品資訊.GUID = "";
            this.rJ_Lable_儲位選擇_藥品資訊.Location = new System.Drawing.Point(0, 108);
            this.rJ_Lable_儲位選擇_藥品資訊.Name = "rJ_Lable_儲位選擇_藥品資訊";
            this.rJ_Lable_儲位選擇_藥品資訊.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_儲位選擇_藥品資訊.ShadowSize = 0;
            this.rJ_Lable_儲位選擇_藥品資訊.Size = new System.Drawing.Size(1390, 78);
            this.rJ_Lable_儲位選擇_藥品資訊.TabIndex = 19;
            this.rJ_Lable_儲位選擇_藥品資訊.Text = "(XXXXXX) XXXXXXXXXXXXXXXXXXX";
            this.rJ_Lable_儲位選擇_藥品資訊.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable_儲位選擇_藥品資訊.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable1
            // 
            this.rJ_Lable1.BackColor = System.Drawing.Color.White;
            this.rJ_Lable1.BackgroundColor = System.Drawing.Color.DarkGray;
            this.rJ_Lable1.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable1.BorderRadius = 10;
            this.rJ_Lable1.BorderSize = 0;
            this.rJ_Lable1.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable1.Font = new System.Drawing.Font("微軟正黑體", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable1.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable1.GUID = "";
            this.rJ_Lable1.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable1.Name = "rJ_Lable1";
            this.rJ_Lable1.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable1.ShadowSize = 3;
            this.rJ_Lable1.Size = new System.Drawing.Size(1390, 108);
            this.rJ_Lable1.TabIndex = 18;
            this.rJ_Lable1.Text = "儲 位 選 擇";
            this.rJ_Lable1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable1.TextColor = System.Drawing.Color.White;
            // 
            // 效期批號輸入
            // 
            this.效期批號輸入.Controls.Add(this.panel6);
            this.效期批號輸入.Controls.Add(this.batchExpiryControl);
            this.效期批號輸入.Location = new System.Drawing.Point(4, 22);
            this.效期批號輸入.Name = "效期批號輸入";
            this.效期批號輸入.Size = new System.Drawing.Size(1390, 593);
            this.效期批號輸入.TabIndex = 2;
            this.效期批號輸入.Text = "效期批號輸入";
            this.效期批號輸入.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.userControl_NumPanel1);
            this.panel6.Controls.Add(this.rJ_Lable2);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(852, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(538, 593);
            this.panel6.TabIndex = 1;
            // 
            // userControl_NumPanel1
            // 
            this.userControl_NumPanel1.BackColor = System.Drawing.Color.White;
            this.userControl_NumPanel1.Content = "";
            this.userControl_NumPanel1.ContentFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.userControl_NumPanel1.Location = new System.Drawing.Point(81, 111);
            this.userControl_NumPanel1.Name = "userControl_NumPanel1";
            this.userControl_NumPanel1.Size = new System.Drawing.Size(401, 469);
            this.userControl_NumPanel1.TabIndex = 22;
            this.userControl_NumPanel1.Title = "";
            this.userControl_NumPanel1.TitleFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.userControl_NumPanel1.Value = 0;
            // 
            // rJ_Lable2
            // 
            this.rJ_Lable2.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable2.BackgroundColor = System.Drawing.Color.DarkGray;
            this.rJ_Lable2.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable2.BorderRadius = 10;
            this.rJ_Lable2.BorderSize = 0;
            this.rJ_Lable2.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable2.Font = new System.Drawing.Font("微軟正黑體", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable2.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable2.GUID = "";
            this.rJ_Lable2.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable2.Name = "rJ_Lable2";
            this.rJ_Lable2.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable2.ShadowSize = 3;
            this.rJ_Lable2.Size = new System.Drawing.Size(538, 108);
            this.rJ_Lable2.TabIndex = 21;
            this.rJ_Lable2.Text = "輸 入 數 量";
            this.rJ_Lable2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable2.TextColor = System.Drawing.Color.White;
            // 
            // batchExpiryControl
            // 
            this.batchExpiryControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.batchExpiryControl.Location = new System.Drawing.Point(0, 0);
            this.batchExpiryControl.Name = "batchExpiryControl";
            this.batchExpiryControl.Size = new System.Drawing.Size(852, 593);
            this.batchExpiryControl.TabIndex = 0;
            // 
            // 確認結果
            // 
            this.確認結果.Controls.Add(this.rJ_Button_確認);
            this.確認結果.Controls.Add(this.panel12);
            this.確認結果.Controls.Add(this.panel11);
            this.確認結果.Controls.Add(this.panel10);
            this.確認結果.Controls.Add(this.panel9);
            this.確認結果.Controls.Add(this.panel8);
            this.確認結果.Controls.Add(this.panel7);
            this.確認結果.Controls.Add(this.rJ_Lable3);
            this.確認結果.Location = new System.Drawing.Point(4, 22);
            this.確認結果.Name = "確認結果";
            this.確認結果.Padding = new System.Windows.Forms.Padding(20, 20, 20, 5);
            this.確認結果.Size = new System.Drawing.Size(1390, 593);
            this.確認結果.TabIndex = 3;
            this.確認結果.Text = "確認結果";
            this.確認結果.UseVisualStyleBackColor = true;
            // 
            // rJ_Button_確認
            // 
            this.rJ_Button_確認.AutoResetState = false;
            this.rJ_Button_確認.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_確認.BackgroundColor = System.Drawing.Color.Green;
            this.rJ_Button_確認.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_確認.BorderRadius = 20;
            this.rJ_Button_確認.BorderSize = 0;
            this.rJ_Button_確認.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_確認.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_確認.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rJ_Button_確認.FlatAppearance.BorderSize = 0;
            this.rJ_Button_確認.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_確認.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_確認.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_確認.GUID = "";
            this.rJ_Button_確認.Image_padding = new System.Windows.Forms.Padding(0);
            this.rJ_Button_確認.Location = new System.Drawing.Point(20, 488);
            this.rJ_Button_確認.Name = "rJ_Button_確認";
            this.rJ_Button_確認.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_確認.ProhibitionLineWidth = 4;
            this.rJ_Button_確認.ProhibitionSymbolSize = 30;
            this.rJ_Button_確認.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_確認.ShadowSize = 3;
            this.rJ_Button_確認.ShowLoadingForm = false;
            this.rJ_Button_確認.Size = new System.Drawing.Size(1350, 100);
            this.rJ_Button_確認.State = false;
            this.rJ_Button_確認.TabIndex = 168;
            this.rJ_Button_確認.Text = "確認";
            this.rJ_Button_確認.TextColor = System.Drawing.Color.White;
            this.rJ_Button_確認.TextHeight = 0;
            this.rJ_Button_確認.UseVisualStyleBackColor = false;
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.rJ_Lable_數量);
            this.panel12.Controls.Add(this.rJ_Lable13);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel12.Location = new System.Drawing.Point(20, 430);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(1350, 71);
            this.panel12.TabIndex = 25;
            // 
            // rJ_Lable_數量
            // 
            this.rJ_Lable_數量.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_數量.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_數量.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_數量.BorderRadius = 10;
            this.rJ_Lable_數量.BorderSize = 0;
            this.rJ_Lable_數量.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rJ_Lable_數量.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_數量.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_數量.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_數量.GUID = "";
            this.rJ_Lable_數量.Location = new System.Drawing.Point(114, 0);
            this.rJ_Lable_數量.Name = "rJ_Lable_數量";
            this.rJ_Lable_數量.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_數量.ShadowSize = 0;
            this.rJ_Lable_數量.Size = new System.Drawing.Size(1236, 71);
            this.rJ_Lable_數量.TabIndex = 21;
            this.rJ_Lable_數量.Text = "-------";
            this.rJ_Lable_數量.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable_數量.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable13
            // 
            this.rJ_Lable13.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable13.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable13.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable13.BorderRadius = 10;
            this.rJ_Lable13.BorderSize = 0;
            this.rJ_Lable13.Dock = System.Windows.Forms.DockStyle.Left;
            this.rJ_Lable13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable13.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable13.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable13.GUID = "";
            this.rJ_Lable13.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable13.Name = "rJ_Lable13";
            this.rJ_Lable13.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable13.ShadowSize = 0;
            this.rJ_Lable13.Size = new System.Drawing.Size(114, 71);
            this.rJ_Lable13.TabIndex = 20;
            this.rJ_Lable13.Text = "數量 :";
            this.rJ_Lable13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable13.TextColor = System.Drawing.Color.Black;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.rJ_Lable_批號);
            this.panel11.Controls.Add(this.rJ_Lable11);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(20, 359);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(1350, 71);
            this.panel11.TabIndex = 24;
            // 
            // rJ_Lable_批號
            // 
            this.rJ_Lable_批號.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_批號.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_批號.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_批號.BorderRadius = 10;
            this.rJ_Lable_批號.BorderSize = 0;
            this.rJ_Lable_批號.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rJ_Lable_批號.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_批號.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_批號.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_批號.GUID = "";
            this.rJ_Lable_批號.Location = new System.Drawing.Point(114, 0);
            this.rJ_Lable_批號.Name = "rJ_Lable_批號";
            this.rJ_Lable_批號.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_批號.ShadowSize = 0;
            this.rJ_Lable_批號.Size = new System.Drawing.Size(1236, 71);
            this.rJ_Lable_批號.TabIndex = 21;
            this.rJ_Lable_批號.Text = "-------";
            this.rJ_Lable_批號.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable_批號.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable11
            // 
            this.rJ_Lable11.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable11.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable11.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable11.BorderRadius = 10;
            this.rJ_Lable11.BorderSize = 0;
            this.rJ_Lable11.Dock = System.Windows.Forms.DockStyle.Left;
            this.rJ_Lable11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable11.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable11.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable11.GUID = "";
            this.rJ_Lable11.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable11.Name = "rJ_Lable11";
            this.rJ_Lable11.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable11.ShadowSize = 0;
            this.rJ_Lable11.Size = new System.Drawing.Size(114, 71);
            this.rJ_Lable11.TabIndex = 20;
            this.rJ_Lable11.Text = "批號 :";
            this.rJ_Lable11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable11.TextColor = System.Drawing.Color.Black;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.rJ_Lable_效期);
            this.panel10.Controls.Add(this.rJ_Lable9);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Location = new System.Drawing.Point(20, 288);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(1350, 71);
            this.panel10.TabIndex = 23;
            // 
            // rJ_Lable_效期
            // 
            this.rJ_Lable_效期.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_效期.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_效期.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_效期.BorderRadius = 10;
            this.rJ_Lable_效期.BorderSize = 0;
            this.rJ_Lable_效期.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rJ_Lable_效期.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_效期.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_效期.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_效期.GUID = "";
            this.rJ_Lable_效期.Location = new System.Drawing.Point(114, 0);
            this.rJ_Lable_效期.Name = "rJ_Lable_效期";
            this.rJ_Lable_效期.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_效期.ShadowSize = 0;
            this.rJ_Lable_效期.Size = new System.Drawing.Size(1236, 71);
            this.rJ_Lable_效期.TabIndex = 21;
            this.rJ_Lable_效期.Text = "-------";
            this.rJ_Lable_效期.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable_效期.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable9
            // 
            this.rJ_Lable9.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable9.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable9.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable9.BorderRadius = 10;
            this.rJ_Lable9.BorderSize = 0;
            this.rJ_Lable9.Dock = System.Windows.Forms.DockStyle.Left;
            this.rJ_Lable9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable9.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable9.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable9.GUID = "";
            this.rJ_Lable9.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable9.Name = "rJ_Lable9";
            this.rJ_Lable9.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable9.ShadowSize = 0;
            this.rJ_Lable9.Size = new System.Drawing.Size(114, 71);
            this.rJ_Lable9.TabIndex = 20;
            this.rJ_Lable9.Text = "效期 :";
            this.rJ_Lable9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable9.TextColor = System.Drawing.Color.Black;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.rJ_Lable_藥名);
            this.panel9.Controls.Add(this.rJ_Lable7);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(20, 217);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(1350, 71);
            this.panel9.TabIndex = 22;
            // 
            // rJ_Lable_藥名
            // 
            this.rJ_Lable_藥名.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥名.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥名.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_藥名.BorderRadius = 10;
            this.rJ_Lable_藥名.BorderSize = 0;
            this.rJ_Lable_藥名.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rJ_Lable_藥名.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_藥名.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_藥名.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥名.GUID = "";
            this.rJ_Lable_藥名.Location = new System.Drawing.Point(114, 0);
            this.rJ_Lable_藥名.Name = "rJ_Lable_藥名";
            this.rJ_Lable_藥名.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_藥名.ShadowSize = 0;
            this.rJ_Lable_藥名.Size = new System.Drawing.Size(1236, 71);
            this.rJ_Lable_藥名.TabIndex = 21;
            this.rJ_Lable_藥名.Text = "-------";
            this.rJ_Lable_藥名.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable_藥名.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable7
            // 
            this.rJ_Lable7.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable7.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable7.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable7.BorderRadius = 10;
            this.rJ_Lable7.BorderSize = 0;
            this.rJ_Lable7.Dock = System.Windows.Forms.DockStyle.Left;
            this.rJ_Lable7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable7.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable7.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable7.GUID = "";
            this.rJ_Lable7.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable7.Name = "rJ_Lable7";
            this.rJ_Lable7.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable7.ShadowSize = 0;
            this.rJ_Lable7.Size = new System.Drawing.Size(114, 71);
            this.rJ_Lable7.TabIndex = 20;
            this.rJ_Lable7.Text = "藥名 :";
            this.rJ_Lable7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable7.TextColor = System.Drawing.Color.Black;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.rJ_Lable_藥碼);
            this.panel8.Controls.Add(this.rJ_Lable4);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(20, 146);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(1350, 71);
            this.panel8.TabIndex = 21;
            // 
            // rJ_Lable_藥碼
            // 
            this.rJ_Lable_藥碼.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥碼.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥碼.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_藥碼.BorderRadius = 10;
            this.rJ_Lable_藥碼.BorderSize = 0;
            this.rJ_Lable_藥碼.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rJ_Lable_藥碼.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_藥碼.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_藥碼.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_藥碼.GUID = "";
            this.rJ_Lable_藥碼.Location = new System.Drawing.Point(114, 0);
            this.rJ_Lable_藥碼.Name = "rJ_Lable_藥碼";
            this.rJ_Lable_藥碼.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_藥碼.ShadowSize = 0;
            this.rJ_Lable_藥碼.Size = new System.Drawing.Size(1236, 71);
            this.rJ_Lable_藥碼.TabIndex = 21;
            this.rJ_Lable_藥碼.Text = "-------";
            this.rJ_Lable_藥碼.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable_藥碼.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable4
            // 
            this.rJ_Lable4.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable4.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable4.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable4.BorderRadius = 10;
            this.rJ_Lable4.BorderSize = 0;
            this.rJ_Lable4.Dock = System.Windows.Forms.DockStyle.Left;
            this.rJ_Lable4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable4.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable4.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable4.GUID = "";
            this.rJ_Lable4.Location = new System.Drawing.Point(0, 0);
            this.rJ_Lable4.Name = "rJ_Lable4";
            this.rJ_Lable4.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable4.ShadowSize = 0;
            this.rJ_Lable4.Size = new System.Drawing.Size(114, 71);
            this.rJ_Lable4.TabIndex = 20;
            this.rJ_Lable4.Text = "藥碼 :";
            this.rJ_Lable4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rJ_Lable4.TextColor = System.Drawing.Color.Black;
            // 
            // panel7
            // 
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(20, 128);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1350, 18);
            this.panel7.TabIndex = 20;
            // 
            // rJ_Lable3
            // 
            this.rJ_Lable3.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable3.BackgroundColor = System.Drawing.Color.Black;
            this.rJ_Lable3.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable3.BorderRadius = 40;
            this.rJ_Lable3.BorderSize = 0;
            this.rJ_Lable3.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable3.Font = new System.Drawing.Font("微軟正黑體", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable3.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable3.GUID = "";
            this.rJ_Lable3.Location = new System.Drawing.Point(20, 20);
            this.rJ_Lable3.Name = "rJ_Lable3";
            this.rJ_Lable3.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable3.ShadowSize = 0;
            this.rJ_Lable3.Size = new System.Drawing.Size(1350, 108);
            this.rJ_Lable3.TabIndex = 19;
            this.rJ_Lable3.Text = "請 關 閉 抽 屜 完 成 【入 庫 作 業】";
            this.rJ_Lable3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable3.TextColor = System.Drawing.Color.White;
            // 
            // Dialog_單品入庫作業
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CaptionHeight = 40;
            this.ClientSize = new System.Drawing.Size(1406, 844);
            this.Controls.Add(this.tabControlEx);
            this.Controls.Add(this.panel_下一步);
            this.Controls.Add(this.panel5);
            this.Name = "Dialog_單品入庫作業";
            this.Text = "入庫作業";
            this.panel5.ResumeLayout(false);
            this.panel_下一步.ResumeLayout(false);
            this.tabControlEx.ResumeLayout(false);
            this.藥品搜尋.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_藥品圖片)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.儲位選擇.ResumeLayout(false);
            this.效期批號輸入.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.確認結果.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel5;
        private MyUI.RJ_Button rJ_Button_取消;
        private MyUI.StepViewer stepViewer1;
        private System.Windows.Forms.Panel panel_下一步;
        private MyUI.RJ_Button rJ_Button_下一步;
        private MyUI.TabControlEx tabControlEx;
        private System.Windows.Forms.TabPage 藥品搜尋;
        private System.Windows.Forms.Panel panel3;
        private MyUI.RJ_Lable rJ_Lable_藥品資訊_藥名;
        private System.Windows.Forms.PictureBox pictureBox_藥品圖片;
        private MyUI.RJ_Lable rJ_Lable_藥品資訊_藥碼;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox comboBox_藥品搜尋種類;
        private MyUI.RJ_TextBox textBox_藥品搜尋內容;
        private MyUI.RJ_Button rJ_Button_藥品搜尋;
        private System.Windows.Forms.Panel panel4;
        private SQLUI.SQL_DataGridView sqL_DataGridView_藥品資料;
        private System.Windows.Forms.TabPage 儲位選擇;
        private SQLUI.SQL_DataGridView sqL_DataGridView_儲位選擇;
        private MyUI.RJ_Lable rJ_Lable_儲位選擇_藥品資訊;
        private MyUI.RJ_Lable rJ_Lable1;
        private System.Windows.Forms.TabPage 效期批號輸入;
        private BatchExpiryControl batchExpiryControl;
        private System.Windows.Forms.Panel panel6;
        private MyUI.RJ_Lable rJ_Lable2;
        private MyUI.UserControl_NumPanel userControl_NumPanel1;
        private System.Windows.Forms.TabPage 確認結果;
        private System.Windows.Forms.Panel panel12;
        private MyUI.RJ_Lable rJ_Lable_數量;
        private MyUI.RJ_Lable rJ_Lable13;
        private System.Windows.Forms.Panel panel11;
        private MyUI.RJ_Lable rJ_Lable_批號;
        private MyUI.RJ_Lable rJ_Lable11;
        private System.Windows.Forms.Panel panel10;
        private MyUI.RJ_Lable rJ_Lable_效期;
        private MyUI.RJ_Lable rJ_Lable9;
        private System.Windows.Forms.Panel panel9;
        private MyUI.RJ_Lable rJ_Lable_藥名;
        private MyUI.RJ_Lable rJ_Lable7;
        private System.Windows.Forms.Panel panel8;
        private MyUI.RJ_Lable rJ_Lable_藥碼;
        private MyUI.RJ_Lable rJ_Lable4;
        private System.Windows.Forms.Panel panel7;
        private MyUI.RJ_Lable rJ_Lable3;
        private MyUI.RJ_Button rJ_Button_確認;
    }
}