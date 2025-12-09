using Basic;
using HIS_DB_Lib;
using MyUI;
using RFID_FX600lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FADC
{
    public partial class Dialog_等待RFID感應 : MyDialog
    {

        public string Value = "";
        private MyThread MyThread_program;

  
        public Dialog_等待RFID感應()
        {
            InitializeComponent();
            this.Load += Dialog_等待RFID感應_Load;
            this.FormClosed += Dialog_等待RFID感應_FormClosed;
            rJ_Button_退出.MouseDownEvent += RJ_Button_退出_MouseDownEvent;
        }

     
        private void sub_program()
        {
            string UID_01 =Main_Form._RFID_FX600_UI.Get_RFID_UID(1);

            if (UID_01.StringIsEmpty() == false && this.IsHandleCreated && UID_01.StringToInt32() != 0)
            {
                this.Invoke(new Action(delegate
                {
                    Value = UID_01;
                    this.label_state.Text = $"成功刷入!{Value}";
                    this.label_state.BackColor = Color.GreenYellow;
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(1000);
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                }));
            
            }
        }
        #region Event
        private void Dialog_等待RFID感應_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MyThread_program != null)
            {
                MyThread_program.Abort();
                MyThread_program = null;
            }
        }
        private void RJ_Button_退出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                this.Close();
    
            }));
        }
        private void Dialog_等待RFID感應_Load(object sender, EventArgs e)
        {
            MyThread_program = new MyThread();
            MyThread_program.Add_Method(sub_program);
            MyThread_program.AutoRun(true);
            MyThread_program.SetSleepTime(10);
            MyThread_program.Trigger();
        }
        #endregion
    }
}
