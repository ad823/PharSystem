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

namespace FADC
{
    public partial class Dialog_人臉辨識 : MyDialog
    {
        private MyThread myThread;
        private string detected_id = "";
        private int cnt_detected_id = 0;
        private bool flag_closeForm = false;
        public personPageClass Value = null;
        public Dialog_人臉辨識()
        {
            InitializeComponent();

            this.LoadFinishedEvent += Dialog_人臉辨識_LoadFinishedEvent;
            this.faceRecognitionCanvas.FaceRecognitionResultEvent += FaceRecognitionCanvas_FaceRecognitionResultEvent;

            this.FormClosing += Dialog_人臉辨識_FormClosing;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;

        }

   

        private void sub_program()
        {
            try
            {
                if (flag_closeForm)
                {
                    this.Invoke(new Action(delegate
                    {
                        $"{Value.姓名}登入成功".PlayGooleVoiceAsync(Main_Form.API_Server);
                        Dialog_AlarmForm alarmForm = new Dialog_AlarmForm($"【{Value.姓名}】登入成功", 1500, Color.Green);
                        alarmForm.ShowDialog();
                        this.DialogResult = DialogResult.Yes;
                        this.Close(); 
                    }));
                }
            }
            catch
            {

            }


        }
        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate { this.Close(); }));
        }
        private void Dialog_人臉辨識_LoadFinishedEvent(EventArgs e)
        {
            this.faceRecognitionCanvas.StartCapture(Main_Form.videoCapture);


            myThread = new MyThread();
            myThread.Add_Method(sub_program);
            myThread.AutoRun(true);
            myThread.SetSleepTime(10);
            myThread.Trigger();
        }
        private void FaceRecognitionCanvas_FaceRecognitionResultEvent(string id , double score)
        {
            if(score <= 0.7) cnt_detected_id = 0;
            if (detected_id != id) cnt_detected_id = 0;
            detected_id = id;
            cnt_detected_id++;
            if (cnt_detected_id > 3)
            {
                personPageClass personPage = personPageClass.serch_by_id(Main_Form.API_Server, id);
                if (personPage != null)
                {
                    Value = personPage;
                    flag_closeForm = true;
                }
            }
        }
        private void Dialog_人臉辨識_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (myThread != null)
            {
                myThread.Abort();
                myThread = null;
            }
            this.faceRecognitionCanvas.FaceRecognitionResultEvent -= FaceRecognitionCanvas_FaceRecognitionResultEvent;
            this.faceRecognitionCanvas.StopCaptureSoft();
        }
    }
}
