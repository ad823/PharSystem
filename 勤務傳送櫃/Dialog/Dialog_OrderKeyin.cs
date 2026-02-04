using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using MyUI;
using Basic;
using SQLUI;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using HIS_DB_Lib;
using MyOffice;
using H_Pannel_lib;

namespace 勤務傳送系統
{
    public partial class Dialog_OrderKeyin : MyDialog
    {
        public string Value = "";
        public Dialog_OrderKeyin()
        {
            InitializeComponent();
            this.rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
        }

        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Value = rJ_TextBox1.Text;
            if(this.Value.StringIsEmpty())
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
