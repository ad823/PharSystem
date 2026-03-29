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

namespace 勤務傳送系統
{
    public partial class Dialog_發藥狀態選擇 : MyDialog
    {
        private MyThread myThread_program;
        private OrderClass orderClass;
        public Dialog_發藥狀態選擇(OrderClass orderClass)
        {
            InitializeComponent();
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;
            this.FormClosing += Dialog_發藥狀態選擇_FormClosing;

            myThread_program = new MyThread();
            myThread_program.Add_Method(sub_program);
            myThread_program.AutoRun(true);
            myThread_program.SetSleepTime(100);
            myThread_program.Trigger();

            this.orderClass = orderClass;
         
            plC_RJ_Button_不發藥.StateChangeEvent += PlC_RJ_Button_不發藥_StateChangeEvent;
            plC_RJ_Button_大型點滴.StateChangeEvent += PlC_RJ_Button_大型點滴_StateChangeEvent;

           
        }

        private void PlC_RJ_Button_大型點滴_StateChangeEvent(RJ_Button rJ_Button, bool state)
        {
            rJ_Button.BackgroundImage = state ? global::勤務傳送系統.Properties.Resources.大型點滴ON : global::勤務傳送系統.Properties.Resources.大型點滴OFF;
        }

        private void PlC_RJ_Button_不發藥_StateChangeEvent(RJ_Button rJ_Button, bool state)
        {
            rJ_Button.BackgroundImage = state ? global::勤務傳送系統.Properties.Resources.發藥ON : global::勤務傳送系統.Properties.Resources.發藥OFF;
        }
        private bool init = false;
        private void sub_program()
        {
            plC_RJ_Button_不發藥.Run();
            plC_RJ_Button_大型點滴.Run();
            if(init == false)
            {
                foreach (var orderConfig in orderClass.orderConfig)
                {
                    if (orderConfig.功能備註 == "不發藥" && orderConfig.狀態.StringToBool())
                    {
                        //plC_RJ_Button_不發藥.Bool = true;
                        plC_RJ_Button_不發藥.but_press = true;

                    }
                    if (orderConfig.功能備註 == "大瓶藥" && orderConfig.狀態.StringToBool())
                    {
                        //plC_RJ_Button_大型點滴.Bool = true;
                        plC_RJ_Button_大型點滴.but_press = true;

                    }
                }
                init = true;
            }
         
        }
        private void Dialog_發藥狀態選擇_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (myThread_program != null)
            {
                myThread_program.Abort();
                myThread_program = null;
            }
            List<orderConfigClass> orderConfigs_update = new List<orderConfigClass>();
            orderConfigClass orderConfigClass_不發藥 = new orderConfigClass();
            orderConfigClass_不發藥.Order_GUID = orderClass.GUID;
            orderConfigClass_不發藥.狀態 = plC_RJ_Button_不發藥.Bool ? "True" : "False";
            orderConfigClass_不發藥.功能備註 = "不發藥";
            orderConfigs_update.Add(orderConfigClass_不發藥);
            orderConfigClass orderConfigClass_大型點滴 = new orderConfigClass();
            orderConfigClass_不發藥.Order_GUID = orderClass.GUID;
            orderConfigClass_大型點滴.狀態 = plC_RJ_Button_大型點滴.Bool ? "True" : "False";
            orderConfigClass_大型點滴.功能備註 = "大瓶藥";
            orderConfigs_update.Add(orderConfigClass_大型點滴);
            //foreach (var orderConfig in orderClass.orderConfig)
            //{
            //    if (orderConfig.功能備註 == "不發藥")
            //    {
            //        orderConfig.狀態 = plC_RJ_Button_不發藥.Bool ? "True" : "False";
            //        orderConfigs_update.Add(orderConfig);
            //    }
            //    if (orderConfig.功能備註 == "大瓶藥" )
            //    {
            //        orderConfig.狀態 = plC_RJ_Button_大型點滴.Bool ? "True" : "False";
            //        orderConfigs_update.Add(orderConfig);
            //        plC_RJ_Button_大型點滴.but_press = true;

            //    }
            //}
            orderConfigClass.update(Main_Form.API_Server, orderConfigs_update);
        }
        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
