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
using Basic;
using SQLUI;
using HIS_DB_Lib;
using NPOI.SS.Formula.Functions;
using static System.Windows.Forms.AxHost;

namespace 勤務傳送系統
{
    public partial class Dialog_發藥狀態選擇 : MyDialog
    {
        private bool flag_不發藥 = false;
        private bool flag_大瓶藥 = false;
        private OrderClass orderClass;
        public Dialog_發藥狀態選擇(OrderClass orderClass)
        {
            InitializeComponent();
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;
            this.FormClosing += Dialog_發藥狀態選擇_FormClosing;


            this.orderClass = orderClass;
            panel_不發藥.Click += Panel_不發藥_Click;
            panel_大瓶藥.Click += Panel_大瓶藥_Click;

            foreach (var orderConfig in orderClass.orderConfig)
            {
                if (orderConfig.功能備註 == "不發藥" && orderConfig.狀態.StringToBool())
                {
                    flag_不發藥 = true;
                }
                if (orderConfig.功能備註 == "大瓶藥" && orderConfig.狀態.StringToBool())
                {
                    flag_大瓶藥 = true;
                }
            }

            panel_不發藥.BackgroundImage = flag_不發藥 ? global::勤務傳送系統.Properties.Resources.發藥ON : global::勤務傳送系統.Properties.Resources.發藥OFF;
            panel_大瓶藥.BackgroundImage = flag_大瓶藥 ? global::勤務傳送系統.Properties.Resources.大型點滴ON : global::勤務傳送系統.Properties.Resources.大型點滴OFF;

        }

        private void Panel_大瓶藥_Click(object sender, EventArgs e)
        {
            flag_大瓶藥 = !flag_大瓶藥;
            panel_大瓶藥.BackgroundImage = flag_大瓶藥 ? global::勤務傳送系統.Properties.Resources.大型點滴ON : global::勤務傳送系統.Properties.Resources.大型點滴OFF;
        }
        private void Panel_不發藥_Click(object sender, EventArgs e)
        {
            flag_不發藥 = !flag_不發藥;
            panel_不發藥.BackgroundImage = flag_不發藥 ? global::勤務傳送系統.Properties.Resources.發藥ON : global::勤務傳送系統.Properties.Resources.發藥OFF;
        }

        private void Dialog_發藥狀態選擇_FormClosing(object sender, FormClosingEventArgs e)
        {
          
            List<orderConfigClass> orderConfigs_update = new List<orderConfigClass>();
            orderConfigClass orderConfigClass_不發藥 = new orderConfigClass();
            orderConfigClass_不發藥.Order_GUID = orderClass.GUID;
            orderConfigClass_不發藥.狀態 = flag_不發藥 ? "True" : "False";
            orderConfigClass_不發藥.功能備註 = "不發藥";
            orderConfigs_update.Add(orderConfigClass_不發藥);
            orderConfigClass orderConfigClass_大型點滴 = new orderConfigClass();
            orderConfigClass_大型點滴.Order_GUID = orderClass.GUID;
            orderConfigClass_大型點滴.狀態 = flag_大瓶藥 ? "True" : "False";
            orderConfigClass_大型點滴.功能備註 = "大瓶藥";
            orderConfigs_update.Add(orderConfigClass_大型點滴);
    
            orderConfigClass.update(Main_Form.API_Server, orderConfigs_update);
        }
        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
