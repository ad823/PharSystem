namespace 勤務傳送系統
{
    partial class Pannel_Box
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
            this.components = new System.ComponentModel.Container();
            this.panel_Main = new System.Windows.Forms.Panel();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.開門ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.更改病房名稱ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.更改字體ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.電子紙設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label_病房名稱 = new System.Windows.Forms.Label();
            this.label_編號 = new System.Windows.Forms.Label();
            this.label_sensor = new System.Windows.Forms.Label();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.藥局內燈開關ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_Main.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_Main
            // 
            this.panel_Main.BackColor = System.Drawing.Color.White;
            this.panel_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Main.ContextMenuStrip = this.contextMenuStrip;
            this.panel_Main.Controls.Add(this.label_病房名稱);
            this.panel_Main.Controls.Add(this.label_編號);
            this.panel_Main.Controls.Add(this.label_sensor);
            this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Main.Location = new System.Drawing.Point(0, 0);
            this.panel_Main.Name = "panel_Main";
            this.panel_Main.Size = new System.Drawing.Size(231, 177);
            this.panel_Main.TabIndex = 1;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.開門ToolStripMenuItem,
            this.更改病房名稱ToolStripMenuItem,
            this.更改字體ToolStripMenuItem,
            this.電子紙設定ToolStripMenuItem,
            this.藥局內燈開關ToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(187, 146);
            // 
            // 開門ToolStripMenuItem
            // 
            this.開門ToolStripMenuItem.Name = "開門ToolStripMenuItem";
            this.開門ToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
            this.開門ToolStripMenuItem.Text = "開門";
            this.開門ToolStripMenuItem.Click += new System.EventHandler(this.開門ToolStripMenuItem_Click);
            // 
            // 更改病房名稱ToolStripMenuItem
            // 
            this.更改病房名稱ToolStripMenuItem.Name = "更改病房名稱ToolStripMenuItem";
            this.更改病房名稱ToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
            this.更改病房名稱ToolStripMenuItem.Text = "更改病房名稱...";
            this.更改病房名稱ToolStripMenuItem.Click += new System.EventHandler(this.更改病房名稱ToolStripMenuItem_Click);
            // 
            // 更改字體ToolStripMenuItem
            // 
            this.更改字體ToolStripMenuItem.Name = "更改字體ToolStripMenuItem";
            this.更改字體ToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
            this.更改字體ToolStripMenuItem.Text = "更改字體...";
            this.更改字體ToolStripMenuItem.Click += new System.EventHandler(this.更改字體ToolStripMenuItem_Click);
            // 
            // 電子紙設定ToolStripMenuItem
            // 
            this.電子紙設定ToolStripMenuItem.Name = "電子紙設定ToolStripMenuItem";
            this.電子紙設定ToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
            this.電子紙設定ToolStripMenuItem.Text = "電子紙設定...";
            this.電子紙設定ToolStripMenuItem.Click += new System.EventHandler(this.電子紙設定ToolStripMenuItem_Click);
            // 
            // label_病房名稱
            // 
            this.label_病房名稱.BackColor = System.Drawing.Color.White;
            this.label_病房名稱.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_病房名稱.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_病房名稱.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_病房名稱.Location = new System.Drawing.Point(0, 25);
            this.label_病房名稱.Name = "label_病房名稱";
            this.label_病房名稱.Size = new System.Drawing.Size(229, 135);
            this.label_病房名稱.TabIndex = 2;
            this.label_病房名稱.Text = "病房名稱";
            this.label_病房名稱.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_病房名稱.Click += new System.EventHandler(this.label_病房名稱_Click);
            // 
            // label_編號
            // 
            this.label_編號.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_編號.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_編號.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_編號.Location = new System.Drawing.Point(0, 0);
            this.label_編號.Name = "label_編號";
            this.label_編號.Size = new System.Drawing.Size(229, 25);
            this.label_編號.TabIndex = 1;
            this.label_編號.Text = "01";
            this.label_編號.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_sensor
            // 
            this.label_sensor.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_sensor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label_sensor.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_sensor.Location = new System.Drawing.Point(0, 160);
            this.label_sensor.Name = "label_sensor";
            this.label_sensor.Size = new System.Drawing.Size(229, 15);
            this.label_sensor.TabIndex = 3;
            this.label_sensor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // 藥局內燈開關ToolStripMenuItem
            // 
            this.藥局內燈開關ToolStripMenuItem.Name = "藥局內燈開關ToolStripMenuItem";
            this.藥局內燈開關ToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
            this.藥局內燈開關ToolStripMenuItem.Text = "藥局內燈開關";
            this.藥局內燈開關ToolStripMenuItem.Click += new System.EventHandler(this.藥局內燈開關ToolStripMenuItem_Click);
            // 
            // Pannel_Box
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel_Main);
            this.Name = "Pannel_Box";
            this.Size = new System.Drawing.Size(231, 177);
            this.panel_Main.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_Main;
        private System.Windows.Forms.Label label_病房名稱;
        private System.Windows.Forms.Label label_編號;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem 開門ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 更改病房名稱ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 更改字體ToolStripMenuItem;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Label label_sensor;
        private System.Windows.Forms.ToolStripMenuItem 電子紙設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 藥局內燈開關ToolStripMenuItem;
    }
}
