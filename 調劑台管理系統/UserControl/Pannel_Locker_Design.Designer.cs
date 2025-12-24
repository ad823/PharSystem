
namespace 調劑台管理系統
{
    partial class Pannel_Locker_Design
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pannel_Locker_Design));
            this.panel_control = new System.Windows.Forms.Panel();
            this.checkBox_設計模式 = new System.Windows.Forms.CheckBox();
            this.plC_RJ_Button_刷新 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_讀檔 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_存檔 = new MyUI.PLC_RJ_Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.plC_RJ_Button_刪除元件 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_新增容器 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_新增鎖控 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_ScreenButton_存檔 = new MyUI.PLC_RJ_ScreenButton();
            this.plC_RJ_ScreenButton_顯示 = new MyUI.PLC_RJ_ScreenButton();
            this.plC_ScreenPage_main = new MyUI.PLC_ScreenPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.transparentPanel = new MyUI.TransparentPanel();
            this.panel_UI = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.plC_RJ_GroupBox1 = new MyUI.PLC_RJ_GroupBox();
            this.sqL_DataGridView_panel_lock_ui_jsonstring = new SQLUI.SQL_DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部 = new MyUI.PLC_RJ_Button();
            this.panel_control.SuspendLayout();
            this.panel1.SuspendLayout();
            this.plC_ScreenPage_main.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.plC_RJ_GroupBox1.ContentsPanel.SuspendLayout();
            this.plC_RJ_GroupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_control
            // 
            this.panel_control.Controls.Add(this.checkBox_設計模式);
            this.panel_control.Controls.Add(this.plC_RJ_Button_刷新);
            this.panel_control.Controls.Add(this.plC_RJ_Button_讀檔);
            this.panel_control.Controls.Add(this.plC_RJ_Button_存檔);
            this.panel_control.Controls.Add(this.panel1);
            this.panel_control.Controls.Add(this.plC_RJ_ScreenButton_存檔);
            this.panel_control.Controls.Add(this.plC_RJ_ScreenButton_顯示);
            this.panel_control.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_control.Location = new System.Drawing.Point(0, 801);
            this.panel_control.Name = "panel_control";
            this.panel_control.Size = new System.Drawing.Size(1459, 58);
            this.panel_control.TabIndex = 1;
            // 
            // checkBox_設計模式
            // 
            this.checkBox_設計模式.AutoSize = true;
            this.checkBox_設計模式.Location = new System.Drawing.Point(364, 12);
            this.checkBox_設計模式.Name = "checkBox_設計模式";
            this.checkBox_設計模式.Size = new System.Drawing.Size(72, 16);
            this.checkBox_設計模式.TabIndex = 15;
            this.checkBox_設計模式.Text = "設計模式";
            this.checkBox_設計模式.UseVisualStyleBackColor = true;
            // 
            // plC_RJ_Button_刷新
            // 
            this.plC_RJ_Button_刷新.AutoResetState = false;
            this.plC_RJ_Button_刷新.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.plC_RJ_Button_刷新.Bool = false;
            this.plC_RJ_Button_刷新.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_刷新.BorderRadius = 5;
            this.plC_RJ_Button_刷新.BorderSize = 0;
            this.plC_RJ_Button_刷新.but_press = false;
            this.plC_RJ_Button_刷新.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_刷新.Dock = System.Windows.Forms.DockStyle.Right;
            this.plC_RJ_Button_刷新.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_刷新.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_刷新.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_刷新.GUID = "";
            this.plC_RJ_Button_刷新.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_刷新.Location = new System.Drawing.Point(696, 0);
            this.plC_RJ_Button_刷新.Name = "plC_RJ_Button_刷新";
            this.plC_RJ_Button_刷新.OFF_文字內容 = "刷新";
            this.plC_RJ_Button_刷新.OFF_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_刷新.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_刷新.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_刷新.ON_BorderSize = 5;
            this.plC_RJ_Button_刷新.ON_文字內容 = "刷新";
            this.plC_RJ_Button_刷新.ON_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_刷新.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_刷新.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_刷新.ShadowColor = System.Drawing.Color.DimGray;
            this.plC_RJ_Button_刷新.ShadowSize = 0;
            this.plC_RJ_Button_刷新.ShowLoadingForm = false;
            this.plC_RJ_Button_刷新.Size = new System.Drawing.Size(122, 58);
            this.plC_RJ_Button_刷新.State = false;
            this.plC_RJ_Button_刷新.TabIndex = 14;
            this.plC_RJ_Button_刷新.Text = "刷新";
            this.plC_RJ_Button_刷新.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_刷新.Texts = "刷新";
            this.plC_RJ_Button_刷新.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_刷新.字型鎖住 = false;
            this.plC_RJ_Button_刷新.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_刷新.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_刷新.文字鎖住 = false;
            this.plC_RJ_Button_刷新.讀取位元反向 = false;
            this.plC_RJ_Button_刷新.讀寫鎖住 = false;
            this.plC_RJ_Button_刷新.音效 = true;
            this.plC_RJ_Button_刷新.顯示 = false;
            this.plC_RJ_Button_刷新.顯示狀態 = false;
            // 
            // plC_RJ_Button_讀檔
            // 
            this.plC_RJ_Button_讀檔.AutoResetState = false;
            this.plC_RJ_Button_讀檔.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.plC_RJ_Button_讀檔.Bool = false;
            this.plC_RJ_Button_讀檔.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_讀檔.BorderRadius = 5;
            this.plC_RJ_Button_讀檔.BorderSize = 0;
            this.plC_RJ_Button_讀檔.but_press = false;
            this.plC_RJ_Button_讀檔.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_讀檔.Dock = System.Windows.Forms.DockStyle.Right;
            this.plC_RJ_Button_讀檔.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_讀檔.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_讀檔.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_讀檔.GUID = "";
            this.plC_RJ_Button_讀檔.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_讀檔.Location = new System.Drawing.Point(818, 0);
            this.plC_RJ_Button_讀檔.Name = "plC_RJ_Button_讀檔";
            this.plC_RJ_Button_讀檔.OFF_文字內容 = "讀檔";
            this.plC_RJ_Button_讀檔.OFF_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_讀檔.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_讀檔.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_讀檔.ON_BorderSize = 5;
            this.plC_RJ_Button_讀檔.ON_文字內容 = "讀檔";
            this.plC_RJ_Button_讀檔.ON_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_讀檔.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_讀檔.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_讀檔.ShadowColor = System.Drawing.Color.DimGray;
            this.plC_RJ_Button_讀檔.ShadowSize = 0;
            this.plC_RJ_Button_讀檔.ShowLoadingForm = false;
            this.plC_RJ_Button_讀檔.Size = new System.Drawing.Size(122, 58);
            this.plC_RJ_Button_讀檔.State = false;
            this.plC_RJ_Button_讀檔.TabIndex = 13;
            this.plC_RJ_Button_讀檔.Text = "讀檔";
            this.plC_RJ_Button_讀檔.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_讀檔.Texts = "讀檔";
            this.plC_RJ_Button_讀檔.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_讀檔.字型鎖住 = false;
            this.plC_RJ_Button_讀檔.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_讀檔.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_讀檔.文字鎖住 = false;
            this.plC_RJ_Button_讀檔.讀取位元反向 = false;
            this.plC_RJ_Button_讀檔.讀寫鎖住 = false;
            this.plC_RJ_Button_讀檔.音效 = true;
            this.plC_RJ_Button_讀檔.顯示 = false;
            this.plC_RJ_Button_讀檔.顯示狀態 = false;
            // 
            // plC_RJ_Button_存檔
            // 
            this.plC_RJ_Button_存檔.AutoResetState = false;
            this.plC_RJ_Button_存檔.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.plC_RJ_Button_存檔.Bool = false;
            this.plC_RJ_Button_存檔.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_存檔.BorderRadius = 5;
            this.plC_RJ_Button_存檔.BorderSize = 0;
            this.plC_RJ_Button_存檔.but_press = false;
            this.plC_RJ_Button_存檔.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_存檔.Dock = System.Windows.Forms.DockStyle.Right;
            this.plC_RJ_Button_存檔.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_存檔.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_存檔.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_存檔.GUID = "";
            this.plC_RJ_Button_存檔.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_存檔.Location = new System.Drawing.Point(940, 0);
            this.plC_RJ_Button_存檔.Name = "plC_RJ_Button_存檔";
            this.plC_RJ_Button_存檔.OFF_文字內容 = "存檔";
            this.plC_RJ_Button_存檔.OFF_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_存檔.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_存檔.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_存檔.ON_BorderSize = 5;
            this.plC_RJ_Button_存檔.ON_文字內容 = "存檔";
            this.plC_RJ_Button_存檔.ON_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_存檔.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_存檔.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_存檔.ShadowColor = System.Drawing.Color.DimGray;
            this.plC_RJ_Button_存檔.ShadowSize = 0;
            this.plC_RJ_Button_存檔.ShowLoadingForm = false;
            this.plC_RJ_Button_存檔.Size = new System.Drawing.Size(122, 58);
            this.plC_RJ_Button_存檔.State = false;
            this.plC_RJ_Button_存檔.TabIndex = 12;
            this.plC_RJ_Button_存檔.Text = "存檔";
            this.plC_RJ_Button_存檔.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_存檔.Texts = "存檔";
            this.plC_RJ_Button_存檔.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_存檔.字型鎖住 = false;
            this.plC_RJ_Button_存檔.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_存檔.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_存檔.文字鎖住 = false;
            this.plC_RJ_Button_存檔.讀取位元反向 = false;
            this.plC_RJ_Button_存檔.讀寫鎖住 = false;
            this.plC_RJ_Button_存檔.音效 = true;
            this.plC_RJ_Button_存檔.顯示 = false;
            this.plC_RJ_Button_存檔.顯示狀態 = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.plC_RJ_Button_刪除元件);
            this.panel1.Controls.Add(this.plC_RJ_Button_新增容器);
            this.panel1.Controls.Add(this.plC_RJ_Button_新增鎖控);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1062, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(397, 58);
            this.panel1.TabIndex = 11;
            // 
            // plC_RJ_Button_刪除元件
            // 
            this.plC_RJ_Button_刪除元件.AutoResetState = false;
            this.plC_RJ_Button_刪除元件.BackgroundColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_刪除元件.Bool = false;
            this.plC_RJ_Button_刪除元件.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_刪除元件.BorderRadius = 5;
            this.plC_RJ_Button_刪除元件.BorderSize = 0;
            this.plC_RJ_Button_刪除元件.but_press = false;
            this.plC_RJ_Button_刪除元件.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_刪除元件.Dock = System.Windows.Forms.DockStyle.Right;
            this.plC_RJ_Button_刪除元件.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_刪除元件.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_刪除元件.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_刪除元件.GUID = "";
            this.plC_RJ_Button_刪除元件.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_刪除元件.Location = new System.Drawing.Point(37, 0);
            this.plC_RJ_Button_刪除元件.Name = "plC_RJ_Button_刪除元件";
            this.plC_RJ_Button_刪除元件.OFF_文字內容 = "刪除元件";
            this.plC_RJ_Button_刪除元件.OFF_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_刪除元件.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_刪除元件.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_刪除元件.ON_BorderSize = 5;
            this.plC_RJ_Button_刪除元件.ON_文字內容 = "刪除元件";
            this.plC_RJ_Button_刪除元件.ON_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_刪除元件.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_刪除元件.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_刪除元件.ShadowColor = System.Drawing.Color.DimGray;
            this.plC_RJ_Button_刪除元件.ShadowSize = 0;
            this.plC_RJ_Button_刪除元件.ShowLoadingForm = false;
            this.plC_RJ_Button_刪除元件.Size = new System.Drawing.Size(120, 58);
            this.plC_RJ_Button_刪除元件.State = false;
            this.plC_RJ_Button_刪除元件.TabIndex = 3;
            this.plC_RJ_Button_刪除元件.Text = "刪除元件";
            this.plC_RJ_Button_刪除元件.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_刪除元件.Texts = "刪除元件";
            this.plC_RJ_Button_刪除元件.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_刪除元件.字型鎖住 = false;
            this.plC_RJ_Button_刪除元件.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_刪除元件.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_刪除元件.文字鎖住 = false;
            this.plC_RJ_Button_刪除元件.讀取位元反向 = false;
            this.plC_RJ_Button_刪除元件.讀寫鎖住 = false;
            this.plC_RJ_Button_刪除元件.音效 = true;
            this.plC_RJ_Button_刪除元件.顯示 = false;
            this.plC_RJ_Button_刪除元件.顯示狀態 = false;
            // 
            // plC_RJ_Button_新增容器
            // 
            this.plC_RJ_Button_新增容器.AutoResetState = false;
            this.plC_RJ_Button_新增容器.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.plC_RJ_Button_新增容器.Bool = false;
            this.plC_RJ_Button_新增容器.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_新增容器.BorderRadius = 5;
            this.plC_RJ_Button_新增容器.BorderSize = 0;
            this.plC_RJ_Button_新增容器.but_press = false;
            this.plC_RJ_Button_新增容器.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_新增容器.Dock = System.Windows.Forms.DockStyle.Right;
            this.plC_RJ_Button_新增容器.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_新增容器.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_新增容器.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_新增容器.GUID = "";
            this.plC_RJ_Button_新增容器.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_新增容器.Location = new System.Drawing.Point(157, 0);
            this.plC_RJ_Button_新增容器.Name = "plC_RJ_Button_新增容器";
            this.plC_RJ_Button_新增容器.OFF_文字內容 = "新增容器";
            this.plC_RJ_Button_新增容器.OFF_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_新增容器.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_新增容器.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_新增容器.ON_BorderSize = 5;
            this.plC_RJ_Button_新增容器.ON_文字內容 = "新增容器";
            this.plC_RJ_Button_新增容器.ON_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_新增容器.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_新增容器.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_新增容器.ShadowColor = System.Drawing.Color.DimGray;
            this.plC_RJ_Button_新增容器.ShadowSize = 0;
            this.plC_RJ_Button_新增容器.ShowLoadingForm = false;
            this.plC_RJ_Button_新增容器.Size = new System.Drawing.Size(120, 58);
            this.plC_RJ_Button_新增容器.State = false;
            this.plC_RJ_Button_新增容器.TabIndex = 2;
            this.plC_RJ_Button_新增容器.Text = "新增容器";
            this.plC_RJ_Button_新增容器.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_新增容器.Texts = "新增容器";
            this.plC_RJ_Button_新增容器.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_新增容器.字型鎖住 = false;
            this.plC_RJ_Button_新增容器.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_新增容器.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_新增容器.文字鎖住 = false;
            this.plC_RJ_Button_新增容器.讀取位元反向 = false;
            this.plC_RJ_Button_新增容器.讀寫鎖住 = false;
            this.plC_RJ_Button_新增容器.音效 = true;
            this.plC_RJ_Button_新增容器.顯示 = false;
            this.plC_RJ_Button_新增容器.顯示狀態 = false;
            // 
            // plC_RJ_Button_新增鎖控
            // 
            this.plC_RJ_Button_新增鎖控.AutoResetState = false;
            this.plC_RJ_Button_新增鎖控.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.plC_RJ_Button_新增鎖控.Bool = false;
            this.plC_RJ_Button_新增鎖控.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_新增鎖控.BorderRadius = 5;
            this.plC_RJ_Button_新增鎖控.BorderSize = 0;
            this.plC_RJ_Button_新增鎖控.but_press = false;
            this.plC_RJ_Button_新增鎖控.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_新增鎖控.Dock = System.Windows.Forms.DockStyle.Right;
            this.plC_RJ_Button_新增鎖控.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_新增鎖控.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_新增鎖控.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_新增鎖控.GUID = "";
            this.plC_RJ_Button_新增鎖控.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_新增鎖控.Location = new System.Drawing.Point(277, 0);
            this.plC_RJ_Button_新增鎖控.Name = "plC_RJ_Button_新增鎖控";
            this.plC_RJ_Button_新增鎖控.OFF_文字內容 = "新增鎖控";
            this.plC_RJ_Button_新增鎖控.OFF_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_新增鎖控.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_新增鎖控.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_新增鎖控.ON_BorderSize = 5;
            this.plC_RJ_Button_新增鎖控.ON_文字內容 = "新增鎖控";
            this.plC_RJ_Button_新增鎖控.ON_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_新增鎖控.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_新增鎖控.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_新增鎖控.ShadowColor = System.Drawing.Color.DimGray;
            this.plC_RJ_Button_新增鎖控.ShadowSize = 0;
            this.plC_RJ_Button_新增鎖控.ShowLoadingForm = false;
            this.plC_RJ_Button_新增鎖控.Size = new System.Drawing.Size(120, 58);
            this.plC_RJ_Button_新增鎖控.State = false;
            this.plC_RJ_Button_新增鎖控.TabIndex = 0;
            this.plC_RJ_Button_新增鎖控.Text = "新增鎖控";
            this.plC_RJ_Button_新增鎖控.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_新增鎖控.Texts = "新增鎖控";
            this.plC_RJ_Button_新增鎖控.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_新增鎖控.字型鎖住 = false;
            this.plC_RJ_Button_新增鎖控.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_新增鎖控.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_新增鎖控.文字鎖住 = false;
            this.plC_RJ_Button_新增鎖控.讀取位元反向 = false;
            this.plC_RJ_Button_新增鎖控.讀寫鎖住 = false;
            this.plC_RJ_Button_新增鎖控.音效 = true;
            this.plC_RJ_Button_新增鎖控.顯示 = false;
            this.plC_RJ_Button_新增鎖控.顯示狀態 = false;
            // 
            // plC_RJ_ScreenButton_存檔
            // 
            this.plC_RJ_ScreenButton_存檔.but_press = false;
            this.plC_RJ_ScreenButton_存檔.Dock = System.Windows.Forms.DockStyle.Left;
            this.plC_RJ_ScreenButton_存檔.IconChar = FontAwesome.Sharp.IconChar.None;
            this.plC_RJ_ScreenButton_存檔.IconSize = 32;
            this.plC_RJ_ScreenButton_存檔.Location = new System.Drawing.Point(172, 0);
            this.plC_RJ_ScreenButton_存檔.Margin = new System.Windows.Forms.Padding(0);
            this.plC_RJ_ScreenButton_存檔.Name = "plC_RJ_ScreenButton_存檔";
            this.plC_RJ_ScreenButton_存檔.OffBackColor = System.Drawing.Color.DarkCyan;
            this.plC_RJ_ScreenButton_存檔.OffFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_ScreenButton_存檔.OffForeColor = System.Drawing.Color.White;
            this.plC_RJ_ScreenButton_存檔.OffIconColor = System.Drawing.Color.Black;
            this.plC_RJ_ScreenButton_存檔.OffText = "資料庫";
            this.plC_RJ_ScreenButton_存檔.OnBackColor = System.Drawing.Color.LightSeaGreen;
            this.plC_RJ_ScreenButton_存檔.OnFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_ScreenButton_存檔.OnForeColor = System.Drawing.Color.White;
            this.plC_RJ_ScreenButton_存檔.OnIconColor = System.Drawing.Color.Black;
            this.plC_RJ_ScreenButton_存檔.OnText = "資料庫";
            this.plC_RJ_ScreenButton_存檔.ShowIcon = false;
            this.plC_RJ_ScreenButton_存檔.Size = new System.Drawing.Size(172, 58);
            this.plC_RJ_ScreenButton_存檔.TabIndex = 10;
            this.plC_RJ_ScreenButton_存檔.字元長度 = MyUI.PLC_RJ_ScreenButton.WordLengthEnum.單字元;
            this.plC_RJ_ScreenButton_存檔.寫入位置註解 = "";
            this.plC_RJ_ScreenButton_存檔.寫入元件位置 = "";
            this.plC_RJ_ScreenButton_存檔.按鈕型態 = MyUI.PLC_RJ_ScreenButton.StatusEnum.保持型;
            this.plC_RJ_ScreenButton_存檔.控制位址 = "D0";
            this.plC_RJ_ScreenButton_存檔.換頁選擇方式 = MyUI.PLC_RJ_ScreenButton.換頁選擇方式Enum.名稱;
            this.plC_RJ_ScreenButton_存檔.致能讀取位置 = "";
            this.plC_RJ_ScreenButton_存檔.讀取位元反向 = false;
            this.plC_RJ_ScreenButton_存檔.讀取位置註解 = "";
            this.plC_RJ_ScreenButton_存檔.讀取元件位置 = "";
            this.plC_RJ_ScreenButton_存檔.音效 = true;
            this.plC_RJ_ScreenButton_存檔.頁面名稱 = "資料庫";
            this.plC_RJ_ScreenButton_存檔.頁面編號 = 0;
            this.plC_RJ_ScreenButton_存檔.顯示方式 = MyUI.PLC_RJ_ScreenButton.StateEnum.顯示為OFF;
            this.plC_RJ_ScreenButton_存檔.顯示狀態 = false;
            this.plC_RJ_ScreenButton_存檔.顯示讀取位置 = "";
            // 
            // plC_RJ_ScreenButton_顯示
            // 
            this.plC_RJ_ScreenButton_顯示.but_press = false;
            this.plC_RJ_ScreenButton_顯示.Dock = System.Windows.Forms.DockStyle.Left;
            this.plC_RJ_ScreenButton_顯示.IconChar = FontAwesome.Sharp.IconChar.None;
            this.plC_RJ_ScreenButton_顯示.IconSize = 32;
            this.plC_RJ_ScreenButton_顯示.Location = new System.Drawing.Point(0, 0);
            this.plC_RJ_ScreenButton_顯示.Margin = new System.Windows.Forms.Padding(0);
            this.plC_RJ_ScreenButton_顯示.Name = "plC_RJ_ScreenButton_顯示";
            this.plC_RJ_ScreenButton_顯示.OffBackColor = System.Drawing.Color.DarkCyan;
            this.plC_RJ_ScreenButton_顯示.OffFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.plC_RJ_ScreenButton_顯示.OffForeColor = System.Drawing.Color.White;
            this.plC_RJ_ScreenButton_顯示.OffIconColor = System.Drawing.Color.Black;
            this.plC_RJ_ScreenButton_顯示.OffText = "顯示";
            this.plC_RJ_ScreenButton_顯示.OnBackColor = System.Drawing.Color.LightSeaGreen;
            this.plC_RJ_ScreenButton_顯示.OnFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_ScreenButton_顯示.OnForeColor = System.Drawing.Color.White;
            this.plC_RJ_ScreenButton_顯示.OnIconColor = System.Drawing.Color.Black;
            this.plC_RJ_ScreenButton_顯示.OnText = "顯示";
            this.plC_RJ_ScreenButton_顯示.ShowIcon = false;
            this.plC_RJ_ScreenButton_顯示.Size = new System.Drawing.Size(172, 58);
            this.plC_RJ_ScreenButton_顯示.TabIndex = 9;
            this.plC_RJ_ScreenButton_顯示.字元長度 = MyUI.PLC_RJ_ScreenButton.WordLengthEnum.單字元;
            this.plC_RJ_ScreenButton_顯示.寫入位置註解 = "";
            this.plC_RJ_ScreenButton_顯示.寫入元件位置 = "";
            this.plC_RJ_ScreenButton_顯示.按鈕型態 = MyUI.PLC_RJ_ScreenButton.StatusEnum.保持型;
            this.plC_RJ_ScreenButton_顯示.控制位址 = "D0";
            this.plC_RJ_ScreenButton_顯示.換頁選擇方式 = MyUI.PLC_RJ_ScreenButton.換頁選擇方式Enum.名稱;
            this.plC_RJ_ScreenButton_顯示.致能讀取位置 = "";
            this.plC_RJ_ScreenButton_顯示.讀取位元反向 = false;
            this.plC_RJ_ScreenButton_顯示.讀取位置註解 = "";
            this.plC_RJ_ScreenButton_顯示.讀取元件位置 = "";
            this.plC_RJ_ScreenButton_顯示.音效 = true;
            this.plC_RJ_ScreenButton_顯示.頁面名稱 = "顯示";
            this.plC_RJ_ScreenButton_顯示.頁面編號 = 0;
            this.plC_RJ_ScreenButton_顯示.顯示方式 = MyUI.PLC_RJ_ScreenButton.StateEnum.顯示為OFF;
            this.plC_RJ_ScreenButton_顯示.顯示狀態 = false;
            this.plC_RJ_ScreenButton_顯示.顯示讀取位置 = "";
            // 
            // plC_ScreenPage_main
            // 
            this.plC_ScreenPage_main.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.plC_ScreenPage_main.BackColor = System.Drawing.Color.White;
            this.plC_ScreenPage_main.Controls.Add(this.tabPage1);
            this.plC_ScreenPage_main.Controls.Add(this.tabPage2);
            this.plC_ScreenPage_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plC_ScreenPage_main.ForekColor = System.Drawing.Color.Black;
            this.plC_ScreenPage_main.ItemSize = new System.Drawing.Size(54, 21);
            this.plC_ScreenPage_main.Location = new System.Drawing.Point(0, 0);
            this.plC_ScreenPage_main.Name = "plC_ScreenPage_main";
            this.plC_ScreenPage_main.SelectedIndex = 0;
            this.plC_ScreenPage_main.Size = new System.Drawing.Size(1459, 801);
            this.plC_ScreenPage_main.TabBackColor = System.Drawing.Color.White;
            this.plC_ScreenPage_main.TabIndex = 0;
            this.plC_ScreenPage_main.顯示標籤列 = MyUI.PLC_ScreenPage.TabVisibleEnum.顯示;
            this.plC_ScreenPage_main.顯示頁面 = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.transparentPanel);
            this.tabPage1.Controls.Add(this.panel_UI);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1451, 772);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "顯示";
            // 
            // transparentPanel
            // 
            this.transparentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transparentPanel.Location = new System.Drawing.Point(0, 0);
            this.transparentPanel.Name = "transparentPanel";
            this.transparentPanel.Size = new System.Drawing.Size(1451, 772);
            this.transparentPanel.TabIndex = 1;
            // 
            // panel_UI
            // 
            this.panel_UI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_UI.Location = new System.Drawing.Point(0, 0);
            this.panel_UI.Name = "panel_UI";
            this.panel_UI.Size = new System.Drawing.Size(1451, 772);
            this.panel_UI.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.White;
            this.tabPage2.Controls.Add(this.plC_RJ_GroupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1451, 772);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "資料庫";
            // 
            // plC_RJ_GroupBox1
            // 
            // 
            // plC_RJ_GroupBox1.ContentsPanel
            // 
            this.plC_RJ_GroupBox1.ContentsPanel.BackColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox1.ContentsPanel.BackgroundColor = System.Drawing.Color.Transparent;
            this.plC_RJ_GroupBox1.ContentsPanel.BorderColor = System.Drawing.Color.SkyBlue;
            this.plC_RJ_GroupBox1.ContentsPanel.BorderRadius = 5;
            this.plC_RJ_GroupBox1.ContentsPanel.BorderSize = 2;
            this.plC_RJ_GroupBox1.ContentsPanel.Controls.Add(this.sqL_DataGridView_panel_lock_ui_jsonstring);
            this.plC_RJ_GroupBox1.ContentsPanel.Controls.Add(this.panel2);
            this.plC_RJ_GroupBox1.ContentsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plC_RJ_GroupBox1.ContentsPanel.ForeColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox1.ContentsPanel.IsSelected = false;
            this.plC_RJ_GroupBox1.ContentsPanel.Location = new System.Drawing.Point(0, 37);
            this.plC_RJ_GroupBox1.ContentsPanel.Name = "ContentsPanel";
            this.plC_RJ_GroupBox1.ContentsPanel.Padding = new System.Windows.Forms.Padding(5);
            this.plC_RJ_GroupBox1.ContentsPanel.ShadowColor = System.Drawing.Color.DimGray;
            this.plC_RJ_GroupBox1.ContentsPanel.ShadowSize = 0;
            this.plC_RJ_GroupBox1.ContentsPanel.Size = new System.Drawing.Size(607, 735);
            this.plC_RJ_GroupBox1.ContentsPanel.TabIndex = 2;
            this.plC_RJ_GroupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.plC_RJ_GroupBox1.GUID = "";
            this.plC_RJ_GroupBox1.Location = new System.Drawing.Point(0, 0);
            this.plC_RJ_GroupBox1.Name = "plC_RJ_GroupBox1";
            this.plC_RJ_GroupBox1.PannelBackColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox1.PannelBorderColor = System.Drawing.Color.SkyBlue;
            this.plC_RJ_GroupBox1.PannelBorderRadius = 5;
            this.plC_RJ_GroupBox1.PannelBorderSize = 2;
            this.plC_RJ_GroupBox1.Size = new System.Drawing.Size(607, 772);
            this.plC_RJ_GroupBox1.TabIndex = 2;
            this.plC_RJ_GroupBox1.TitleBackColor = System.Drawing.Color.DeepSkyBlue;
            this.plC_RJ_GroupBox1.TitleBorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_GroupBox1.TitleBorderRadius = 5;
            this.plC_RJ_GroupBox1.TitleBorderSize = 0;
            this.plC_RJ_GroupBox1.TitleFont = new System.Drawing.Font("新細明體", 12F);
            this.plC_RJ_GroupBox1.TitleForeColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox1.TitleHeight = 37;
            this.plC_RJ_GroupBox1.TitleTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.plC_RJ_GroupBox1.TitleTexts = "UI位置";
            // 
            // sqL_DataGridView_panel_lock_ui_jsonstring
            // 
            this.sqL_DataGridView_panel_lock_ui_jsonstring.AutoSelectToDeep = true;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.backColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.BorderColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.BorderRadius = 10;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.BorderSize = 2;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.cellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.cellStylBackColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.cellStyleFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.sqL_DataGridView_panel_lock_ui_jsonstring.cellStylForeColor = System.Drawing.Color.Black;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.columnHeaderBackColor = System.Drawing.Color.SkyBlue;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.columnHeaderFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.sqL_DataGridView_panel_lock_ui_jsonstring.columnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.columnHeadersHeight = 26;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.columnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Columns.Add(((SQLUI.SQL_DataGridView.ColumnElement)(resources.GetObject("sqL_DataGridView_panel_lock_ui_jsonstring.Columns"))));
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Columns.Add(((SQLUI.SQL_DataGridView.ColumnElement)(resources.GetObject("sqL_DataGridView_panel_lock_ui_jsonstring.Columns1"))));
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Columns.Add(((SQLUI.SQL_DataGridView.ColumnElement)(resources.GetObject("sqL_DataGridView_panel_lock_ui_jsonstring.Columns2"))));
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Font = new System.Drawing.Font("新細明體", 9F);
            this.sqL_DataGridView_panel_lock_ui_jsonstring.ImageBox = false;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Location = new System.Drawing.Point(5, 5);
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Name = "sqL_DataGridView_panel_lock_ui_jsonstring";
            this.sqL_DataGridView_panel_lock_ui_jsonstring.OnlineState = SQLUI.SQL_DataGridView.OnlineEnum.Online;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Password = "user82822040";
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Port = ((uint)(3306u));
            this.sqL_DataGridView_panel_lock_ui_jsonstring.rowHeaderBackColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.rowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.RowsColor = System.Drawing.SystemColors.Control;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.RowsHeight = 30;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.SaveFileName = "SQL_DataGridView";
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Server = "127.0.0.0";
            this.sqL_DataGridView_panel_lock_ui_jsonstring.Size = new System.Drawing.Size(597, 657);
            this.sqL_DataGridView_panel_lock_ui_jsonstring.SSLMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.TabIndex = 1;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.TableName = "panel_lock_ui_jsonstring_new";
            this.sqL_DataGridView_panel_lock_ui_jsonstring.UserName = "root";
            this.sqL_DataGridView_panel_lock_ui_jsonstring.可拖曳欄位寬度 = false;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.可選擇多列 = false;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.單格樣式 = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.自動換行 = true;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.表單字體 = new System.Drawing.Font("新細明體", 9F);
            this.sqL_DataGridView_panel_lock_ui_jsonstring.邊框樣式 = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.顯示CheckBox = false;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.顯示首列 = true;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.顯示首行 = true;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.首列樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.sqL_DataGridView_panel_lock_ui_jsonstring.首行樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(5, 662);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(597, 68);
            this.panel2.TabIndex = 0;
            // 
            // plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部
            // 
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.AutoResetState = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.Bool = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.BorderRadius = 5;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.BorderSize = 0;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.but_press = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.GUID = "";
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.Location = new System.Drawing.Point(428, 6);
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.Name = "plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部";
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.OFF_文字內容 = "顯示全部";
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.OFF_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.ON_BorderSize = 5;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.ON_文字內容 = "顯示全部";
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.ON_文字字體 = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.ShadowColor = System.Drawing.Color.DimGray;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.ShadowSize = 0;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.ShowLoadingForm = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.Size = new System.Drawing.Size(150, 58);
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.State = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.TabIndex = 1;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.Text = "顯示全部";
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.Texts = "顯示全部";
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.字型鎖住 = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.文字鎖住 = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.讀取位元反向 = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.讀寫鎖住 = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.音效 = true;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.顯示 = false;
            this.plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部.顯示狀態 = false;
            // 
            // Pannel_Locker_Design
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.plC_ScreenPage_main);
            this.Controls.Add(this.panel_control);
            this.Name = "Pannel_Locker_Design";
            this.Size = new System.Drawing.Size(1459, 859);
            this.panel_control.ResumeLayout(false);
            this.panel_control.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.plC_ScreenPage_main.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.plC_RJ_GroupBox1.ContentsPanel.ResumeLayout(false);
            this.plC_RJ_GroupBox1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MyUI.PLC_ScreenPage plC_ScreenPage_main;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel_control;
        private MyUI.PLC_RJ_ScreenButton plC_RJ_ScreenButton_存檔;
        private MyUI.PLC_RJ_ScreenButton plC_RJ_ScreenButton_顯示;
        private System.Windows.Forms.Panel panel1;
        private MyUI.PLC_RJ_Button plC_RJ_Button_新增鎖控;
        private System.Windows.Forms.Panel panel_UI;
        private MyUI.PLC_RJ_Button plC_RJ_Button_讀檔;
        private MyUI.PLC_RJ_Button plC_RJ_Button_存檔;
        private MyUI.TransparentPanel transparentPanel;
        private MyUI.PLC_RJ_Button plC_RJ_Button_刷新;
        private MyUI.PLC_RJ_Button plC_RJ_Button_panel_lock_ui_jsonstring_顯示全部;
        private System.Windows.Forms.CheckBox checkBox_設計模式;
        private MyUI.PLC_RJ_Button plC_RJ_Button_刪除元件;
        private MyUI.PLC_RJ_Button plC_RJ_Button_新增容器;
        private MyUI.PLC_RJ_GroupBox plC_RJ_GroupBox1;
        private SQLUI.SQL_DataGridView sqL_DataGridView_panel_lock_ui_jsonstring;
        private System.Windows.Forms.Panel panel2;
    }
}
