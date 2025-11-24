using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyUI;
namespace FADC
{
    public partial class Dialog_輸入批號 : MyDialog
    {
    
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == System.Windows.Forms.Keys.Enter)
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
                return true;
            }
            else if (keyData == System.Windows.Forms.Keys.Escape)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
      
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private string value = "";
        public string Value
        {
            get
            {
                return this.rJ_TextBox_批號.Texts;
            }
            set
            {
                this.rJ_TextBox_批號.Texts = value;
            }
        }
        public Dialog_輸入批號()
        {
            InitializeComponent();
        }
        public Dialog_輸入批號(string value)
        {
            InitializeComponent();
            this.value = value;
           
        }

        private void Dialog_輸入批號_Load(object sender, EventArgs e)
        {
            this.Value = value;
            this.rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
        }
        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }));
        }
        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }
      
    }
}
