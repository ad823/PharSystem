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
using FpMatchLib;
using HIS_DB_Lib;
namespace 調劑台管理系統
{
    public partial class Dialog_指紋登入 : MyDialog
    {
        private MyThread myThread = new MyThread();
        public FpMatchClass Value;
        public personPageClass Value2;
        public int 台號 = -1;
        private bool about = false;
        public Dialog_指紋登入()
        {
            InitializeComponent();

            List<StepEntity> list = new List<StepEntity>();
            list.Add(new StepEntity("1", "初始化", 1, "等待辨識機待命", eumStepState.Completed, null));
            list.Add(new StepEntity("2", "選擇台號", 2, "請選擇要登入的台號", eumStepState.Completed, null));
            list.Add(new StepEntity("3", "按壓指紋", 3, "請壓上指紋至辨識機", eumStepState.Waiting, null));
            list.Add(new StepEntity("4", "完成", 4, "自動登入", eumStepState.Waiting, null));
            this.stepViewer1.CurrentStep = 1;
            this.stepViewer1.ListDataSource = list;

            this.LoadFinishedEvent += Dialog_指紋登入_LoadFinishedEvent;
            this.plC_RJ_Button_返回.MouseDownEvent += PlC_RJ_Button_返回_MouseDownEvent;
            plC_RJ_Button_1號.Enabled = (Main_Form.myConfigClass.Scanner01_COMPort.StringIsEmpty() == false);
            plC_RJ_Button_2號.Enabled = (Main_Form.myConfigClass.Scanner02_COMPort.StringIsEmpty() == false);
            plC_RJ_Button_3號.Enabled = (Main_Form.myConfigClass.Scanner03_COMPort.StringIsEmpty() == false);
            plC_RJ_Button_4號.Enabled = (Main_Form.myConfigClass.Scanner04_COMPort.StringIsEmpty() == false);

            plC_RJ_Button_1號.MouseDownEvent += PlC_RJ_Button_1號_MouseDownEvent;
            plC_RJ_Button_2號.MouseDownEvent += PlC_RJ_Button_2號_MouseDownEvent;
            plC_RJ_Button_3號.MouseDownEvent += PlC_RJ_Button_3號_MouseDownEvent;
            plC_RJ_Button_4號.MouseDownEvent += PlC_RJ_Button_4號_MouseDownEvent;

            this.FormClosing += Dialog_指紋登入_FormClosing;
            
        }

        private void Dialog_指紋登入_FormClosing(object sender, FormClosingEventArgs e)
        {
            myThread.Abort();
            myThread.Stop();
        }

        bool flag_init = false;
        bool flag_step_2 = false;
        bool flag_step_3 = false;
        bool flag_step_4 = false;
        private void sub_program()
        {
            plC_RJ_Button_1號.Run();
            plC_RJ_Button_2號.Run();
            plC_RJ_Button_3號.Run();
            plC_RJ_Button_4號.Run();
            if(flag_init == false)
            {
                if (Main_Form.Function_指紋辨識初始化(true,false) == false)
                {
                    this.Invoke(new Action(delegate
                    {
                        this.DialogResult = DialogResult.No;
                        this.Close();
                    }));
                    return;
                }
                this.stepViewer1.Next();
                if (Main_Form.myConfigClass.Scanner01_COMPort.StringIsEmpty() == false)
                {
                    if (Main_Form.myConfigClass.Scanner02_COMPort.StringIsEmpty() == true
                    && Main_Form.myConfigClass.Scanner03_COMPort.StringIsEmpty() == true
                    && Main_Form.myConfigClass.Scanner04_COMPort.StringIsEmpty() == true)
                    {
                        this.Invoke(new Action(delegate
                        {
                            plC_RJ_Button_1號.Bool = true;
                        }));
                    }
                }
                this.Invoke(new Action(delegate
                {
                    this.plC_RJ_Button_返回.Enabled = true;
                }));
              
                flag_init = true;
            }
          
            if(plC_RJ_Button_1號.Bool || plC_RJ_Button_2號.Bool || plC_RJ_Button_3號.Bool || plC_RJ_Button_4號.Bool)
            {
                if(flag_step_2 == false)
                {
                    this.stepViewer1.Next();
                    flag_step_2 = true;
                }
               
            }
            if (this.stepViewer1.CurrentStep == 3)
            {
                flag_step_3 = true;
                if (Main_Form.fingerModle == Main_Form.FingerModleType.fpMatchSoket)
                {
                    Value = Main_Form.fpMatchSoket.GetFeatureOnce();
                    if (Value == null) return;
                    if (Value.featureLen == 768)
                    {

                        List<object[]> list_人員資料 = Main_Form._sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                        object[] value = null;
                        for (int i = 0; i < list_人員資料.Count; i++)
                        {
                            string feature = list_人員資料[i][(int)enum_人員資料.指紋辨識].ObjectToString();
                            if (Main_Form.fpMatchSoket.Match(Value.feature, feature))
                            {
                                value = list_人員資料[i];

                            }
                        }
                        if (value != null) this.stepViewer1.Next();
                        else
                        {
                            Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("找無符合指紋資訊", 2000);
                            dialog_AlarmForm.ShowDialog();
                            flag_step_3 = false;
                        }
                    }
                }
                else
                {
                    Dialog_HID指紋登入 dialog_HID = new Dialog_HID指紋登入();
                    if (dialog_HID.ShowDialog() != DialogResult.Yes) return;
                    Value2 = dialog_HID.Value;
                }
               
            
            }
            if(this.stepViewer1.CurrentStep == 4 && flag_step_4 == false)
            {
                flag_step_4 = true;
                this.Invoke(new Action(delegate
                {
                    if (plC_RJ_Button_1號.Bool) 台號 = 1;
                    if (plC_RJ_Button_2號.Bool) 台號 = 2;
                    if (plC_RJ_Button_3號.Bool) 台號 = 3;
                    if (plC_RJ_Button_4號.Bool) 台號 = 4;
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                }));
            }
        }
        #region Event
        private void Dialog_指紋登入_LoadFinishedEvent(EventArgs e)
        {
          

            myThread.AutoRun(true);
            myThread.SetSleepTime(50);
            myThread.Add_Method(sub_program);
            myThread.AutoStop(true);
            myThread.Trigger();
        }
        private void PlC_RJ_Button_返回_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Main_Form.fpMatchSoket.Abort();
                this.DialogResult = DialogResult.No;
                this.Close();
            }));
        }
        private void PlC_RJ_Button_1號_MouseDownEvent(MouseEventArgs mevent)
        {
            plC_RJ_Button_1號.Bool = true;
            plC_RJ_Button_2號.Bool = false;
            plC_RJ_Button_3號.Bool = false;
            plC_RJ_Button_4號.Bool = false;
        }
        private void PlC_RJ_Button_2號_MouseDownEvent(MouseEventArgs mevent)
        {
            plC_RJ_Button_1號.Bool = false;
            plC_RJ_Button_2號.Bool = true;
            plC_RJ_Button_3號.Bool = false;
            plC_RJ_Button_4號.Bool = false;
        }
        private void PlC_RJ_Button_3號_MouseDownEvent(MouseEventArgs mevent)
        {
            plC_RJ_Button_1號.Bool = false;
            plC_RJ_Button_2號.Bool = false;
            plC_RJ_Button_3號.Bool = true;
            plC_RJ_Button_4號.Bool = false;
        }
        private void PlC_RJ_Button_4號_MouseDownEvent(MouseEventArgs mevent)
        {
            plC_RJ_Button_1號.Bool = false;
            plC_RJ_Button_2號.Bool = false;
            plC_RJ_Button_3號.Bool = false;
            plC_RJ_Button_4號.Bool = true;
        }
        #endregion
    }
}
