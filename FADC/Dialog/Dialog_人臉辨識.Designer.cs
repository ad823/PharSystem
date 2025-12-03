namespace FADC
{
    partial class Dialog_人臉辨識
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
            this.faceRecognitionCanvas = new FaceRecognitionUserControl.FaceRecognitionCanvas();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rJ_Button_取消 = new MyUI.RJ_Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // faceRecognitionCanvas
            // 
            this.faceRecognitionCanvas.Location = new System.Drawing.Point(69, 47);
            this.faceRecognitionCanvas.Margin = new System.Windows.Forms.Padding(0);
            this.faceRecognitionCanvas.Name = "faceRecognitionCanvas";
            this.faceRecognitionCanvas.Size = new System.Drawing.Size(800, 600);
            this.faceRecognitionCanvas.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rJ_Button_取消);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(4, 677);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(949, 82);
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
            this.rJ_Button_取消.Location = new System.Drawing.Point(823, 0);
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
            // Dialog_人臉辨識
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(957, 763);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.faceRecognitionCanvas);
            this.Name = "Dialog_人臉辨識";
            this.Text = "人臉辨識";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FaceRecognitionUserControl.FaceRecognitionCanvas faceRecognitionCanvas;
        private System.Windows.Forms.Panel panel1;
        private MyUI.RJ_Button rJ_Button_取消;
    }
}