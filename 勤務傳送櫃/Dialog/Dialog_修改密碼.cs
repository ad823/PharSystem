using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basic;
namespace 勤務傳送系統
{
    public partial class Dialog_修改密碼 : Form
    {
        public static Form form;
        public DialogResult ShowDialog()
        {
            if (form == null)
            {
                base.ShowDialog();
            }
            else
            {
                form.Invoke(new Action(delegate
                {
                    base.ShowDialog();
                }));
            }

            return this.DialogResult;
        }
        private string _Password = "";
        public string Password
        {
            get
            {
                return this._Password;
            }
            private set
            {
                this._Password = value;
            }
        }
        public enum enum_Result : int
        {
            確認, 取消
        }
        public enum_Result Result = enum_Result.取消;

        public Dialog_修改密碼()
        {
            InitializeComponent();
        }

        private void button_取消_Click(object sender, EventArgs e)
        {
            this.Result = enum_Result.取消;
            this.Close();
        }

        private void button_確認_Click(object sender, EventArgs e)
        {
            if (this.textBox_新密碼.Text != this.textBox_密碼確認.Text)
            {
                MyMessageBox.ShowDialog("密碼輸入不一致!");
                return;
            }
            this.Password = this.textBox_密碼確認.Text;
            this.Result = enum_Result.確認;
            this.Close();
        }
    }
}
