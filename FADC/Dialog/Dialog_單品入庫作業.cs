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
using FaceRecognitionUserControl;
using System;
using NPOI.SS.UserModel;
using FaceRecognitionDll.Models;
using SQLUI;

namespace FADC
{
    public partial class Dialog_單品入庫作業 : MyDialog
    {
        public Dialog_單品入庫作業()
        {
            form.Invoke(new Action(delegate { InitializeComponent(); }));
            
            this.LoadFinishedEvent += Dialog_入庫作業_LoadFinishedEvent;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
        }

        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Close();
        }
        private void Dialog_入庫作業_LoadFinishedEvent(EventArgs e)
        {
            List<StepEntity> list = new List<StepEntity>();
            list.Add(new StepEntity("1", "選擇儲位", 1, "刷條碼或搜尋", eumStepState.Completed, null));
            list.Add(new StepEntity("2", "效期批號輸入", 2, "選擇或輸入效期批號", eumStepState.Completed, null));
            list.Add(new StepEntity("3", "輸入數量", 3, "輸入數量", eumStepState.Waiting, null));
            list.Add(new StepEntity("4", "完成", 4, "按下確認存檔", eumStepState.Waiting, null));
            this.stepViewer1.CurrentStep = 1;
            this.stepViewer1.ListDataSource = list;

            Table table = medClass.init(Main_Form.API_Server);
            sqL_DataGridView_藥品資料.Init(table); 


            tabControlEx.SelectTab("藥品搜尋");
        }
    }
}
