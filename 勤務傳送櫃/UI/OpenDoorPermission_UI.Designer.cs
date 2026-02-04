namespace 勤務傳送系統
{
    partial class OpenDoorPermission_UI
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
            this.button_setting = new System.Windows.Forms.Button();
            this.label_wardname = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_setting
            // 
            this.button_setting.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_setting.Location = new System.Drawing.Point(191, 0);
            this.button_setting.Name = "button_setting";
            this.button_setting.Size = new System.Drawing.Size(28, 51);
            this.button_setting.TabIndex = 0;
            this.button_setting.Text = "...";
            this.button_setting.UseVisualStyleBackColor = true;
            // 
            // label_wardname
            // 
            this.label_wardname.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_wardname.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_wardname.Font = new System.Drawing.Font("微軟正黑體", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_wardname.Location = new System.Drawing.Point(0, 0);
            this.label_wardname.Name = "label_wardname";
            this.label_wardname.Size = new System.Drawing.Size(191, 51);
            this.label_wardname.TabIndex = 3;
            this.label_wardname.Text = "label1";
            this.label_wardname.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OpenDoorPermission_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label_wardname);
            this.Controls.Add(this.button_setting);
            this.Name = "OpenDoorPermission_UI";
            this.Size = new System.Drawing.Size(219, 51);
            this.Load += new System.EventHandler(this.OpenDoorPermission_UI_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_setting;
        private System.Windows.Forms.Label label_wardname;
    }
}
