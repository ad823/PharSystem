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
namespace ServerSettingForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MyMessageBox.音效 = false;
            MyMessageBox.form = this.FindForm();
            Dialog_調劑台.form = this.FindForm();
            Dialog_癌症備藥機.form = this.FindForm();
            Dialog_藥庫.form = this.FindForm();
            Dialog_更新資訊.form = this.FindForm();
            Dialog_網頁.form = this.FindForm();
            Dialog_中藥調劑系統.form = this.FindForm();
            Dialog_中心叫號系統.form = this.FindForm();
            Dialog_設定.form = this.FindForm();
            Dialog_FADC.form = this.FindForm();

            this.button_網頁.Click += Button_網頁_Click;
            this.button_調劑台.Click += Button_調劑台_Click;
            this.button_藥庫.Click += Button_藥庫_Click;
            this.button_傳送櫃.Click += Button_傳送櫃_Click;
            this.button_更新資訊.Click += Button_更新資訊_Click;
            this.button_癌症備藥機.Click += Button_癌症備藥機_Click;
            this.button_中藥調劑系統.Click += Button_中藥調劑系統_Click;
            this.button_中心叫號系統.Click += Button_中心叫號系統_Click;
            this.button_FADC.Click += Button_FADC_Click;
            this.button_設定.Click += Button_設定_Click;

        }

        private void Button_FADC_Click(object sender, EventArgs e)
        {
            Dialog_FADC dialog_FADC = new Dialog_FADC();
            dialog_FADC.ShowDialog();
        }

        private void Button_設定_Click(object sender, EventArgs e)
        {
            Dialog_設定 dialog_設定 = new Dialog_設定();
            dialog_設定.ShowDialog();
        }
        private void Button_中心叫號系統_Click(object sender, EventArgs e)
        {
            Dialog_中心叫號系統 dialog_中心叫號系統 = new Dialog_中心叫號系統();
            dialog_中心叫號系統.ShowDialog();
        }
        private void Button_癌症備藥機_Click(object sender, EventArgs e)
        {
            Dialog_癌症備藥機 dialog_癌症備藥機 = new Dialog_癌症備藥機();
            dialog_癌症備藥機.ShowDialog();
        }
        private void Button_網頁_Click(object sender, EventArgs e)
        {
            Dialog_網頁 dialog_網頁 = new Dialog_網頁();
            dialog_網頁.ShowDialog();
        }

        private void Button_藥庫_Click(object sender, EventArgs e)
        {
            Dialog_藥庫 dialog_藥庫 = new Dialog_藥庫();
            dialog_藥庫.ShowDialog();
        }
        private void Button_調劑台_Click(object sender, EventArgs e)
        {
            Dialog_調劑台 dialog_調劑台 = new Dialog_調劑台();
            dialog_調劑台.ShowDialog();
        }
        private void Button_傳送櫃_Click(object sender, EventArgs e)
        {
            Dialog_傳送櫃 dialog_傳送櫃 = new Dialog_傳送櫃();
            dialog_傳送櫃.ShowDialog();
        }
        private void Button_中藥調劑系統_Click(object sender, EventArgs e)
        {
            Dialog_中藥調劑系統 dialog_中藥調劑系統 = new Dialog_中藥調劑系統();
            dialog_中藥調劑系統.ShowDialog();
        }
        private void Button_更新資訊_Click(object sender, EventArgs e)
        {
            Dialog_更新資訊 dialog_更新資訊 = new Dialog_更新資訊();
            dialog_更新資訊.ShowDialog();
        }
    }
}
