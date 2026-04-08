using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS_DB_Lib;
using Basic;
using MyUI;
using FingerprintLib;
using System.Threading;


namespace 調劑台管理系統
{
    public partial class Dialog_藥品圖片確認 : MyDialog
    {
        public Dialog_藥品圖片確認(Image image)
        {
            InitializeComponent();
            rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
            if(image != null)
            {
                pictureBox1.Image = image;
            }
        }

        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Close();
        }

        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
