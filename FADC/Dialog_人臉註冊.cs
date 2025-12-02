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
    public partial class Dialog_人臉註冊 : MyDialog
    {
        private bool Isregister = false;
        private string id = "";
        private MyThread myThread;
        public Dialog_人臉註冊(string ID)
        {
       
            InitializeComponent();
            this.LoadFinishedEvent += Dialog_人臉註冊_LoadFinishedEvent;
            this.FormClosing += Dialog_人臉註冊_FormClosing;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
            this.rJ_Button_重新註冊.MouseDownEvent += RJ_Button_重新註冊_MouseDownEvent;

       

            this.id = ID;
        }
        private void sub_program()
        {
            try
            {
                if (Isregister == true) return;

                this.Invoke(new Action(delegate
                {
                    rJ_Lable_state.Text = "【3】秒後拍攝註冊相片....";
                }));
                Thread.Sleep(1000);
                this.Invoke(new Action(delegate
                {
                    rJ_Lable_state.Text = "【2】秒後拍攝註冊相片....";
                }));
                Thread.Sleep(1000);
                this.Invoke(new Action(delegate
                {
                    rJ_Lable_state.Text = "【1】秒後拍攝註冊相片....";
                }));
                Thread.Sleep(1000);
                this.Invoke(new Action(delegate
                {
                    pictureBox_snap.Image = faceRecognitionCanvas.Snap();
                }));

                if (MyMessageBox.ShowDialog("確認相片是否正確?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes)
                {
                    this.Invoke(new Action(delegate
                    {
                        pictureBox_snap.Image = null;
                        rJ_Lable_state.Text = "等待重新拍攝....";
                    }));

                    Thread.Sleep(1000);
                    return;
                }
                Response<FaceRegisterData> response = faceRecognitionCanvas.Register(id, pictureBox_snap.Image);
                if (response == null)
                {
                    this.Invoke(new Action(delegate
                    {
                        rJ_Lable_state.Text = "拍攝失敗,重新拍攝...";
                    }));

                    Thread.Sleep(1000);
                    return;
                }
                if (response.Message == "no_face")
                {
                    this.Invoke(new Action(delegate
                    {
                        rJ_Lable_state.Text = "拍攝失敗,重新拍攝...";
                    }));

                    Thread.Sleep(1000);
                    return;
                }
                MyMessageBox.ShowDialog("註冊成功");
                this.Invoke(new Action(delegate
                {
                    myThread.Abort();
                    Thread.Sleep(500);
                    this.Close();
                }));
            }
            catch
            {

            }
           
          
        }
        private void RJ_Button_重新註冊_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
               
                if (MyMessageBox.ShowDialog("是否重新註冊", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
                LoadingForm.ShowLoadingForm();
                FaceRecognitionUserList.DeletePhoto(id);
                this.Invoke(new Action(delegate
                {
                    this.rJ_Button_重新註冊.Visible = false;
                    Isregister = false;
                    MyMessageBox.ShowDialog("完成");
                }));
              
            }
            catch
            {
                LoadingForm.CloseLoadingForm();
            }
        }
        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Close();
        }
        async private void Dialog_人臉註冊_LoadFinishedEvent(System.EventArgs e)
        {
            this.faceRecognitionCanvas.StartCapture(Main_Form.videoCapture);
            FaceRecognitionUserList.Initial();
            FaceRecognitionUserList.GetUserList();
            await FaceRecognitionUserList.DisplayImagesAsync(id, this.flowLayout);
            flowLayout.Refresh();

            if(FaceRecognitionUserList.GetUserList(id).Count > 0)
            {
                this.rJ_Button_重新註冊.Visible = true;
                Isregister= true; 
            }

            myThread = new MyThread();
            myThread.Add_Method(sub_program);
            myThread.AutoRun(true);
            myThread.SetSleepTime(10);
            myThread.Trigger();
        }
        private void Dialog_人臉註冊_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.faceRecognitionCanvas.StopCaptureSoft();
        }

       
    }
}
