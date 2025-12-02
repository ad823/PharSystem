using MyUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.Text.RegularExpressions;
namespace FADC
{
    public partial class Main_Form : Form
    {
        public static VideoCapture videoCapture = null;
        private void Program_faceReconition_Init()
        {
            try
            {
                videoCapture = new VideoCapture(0);
                videoCapture.Open(0);
                // 設定參數（OpenCV 設定會自動忽略未支援的設備）
                videoCapture.FrameWidth = 1280;
                videoCapture.FrameHeight = 720;
                videoCapture.Fps = 60;
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"WebCam 初始化失敗 , {ex.Message}");
            }


            plC_UI_Init.Add_Method(sub_Program_faceReconition);
        }
        private void sub_Program_faceReconition()
        {

        }
    }
}
