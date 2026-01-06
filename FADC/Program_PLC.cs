using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using MinasA6DLL;
using MyUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FADC
{
    public partial class Main_Form : Form
    {
        public enum enunm_InOutBoard
        {
            ServerPower = 3,
            輸送帶反轉 = 4,
            輸送帶正轉 = 5,
            輸送帶前進 = 6,
            輸送帶後退 = 7,
        }

        public static PLC_Device PLC_Device_Z軸馬達位置 = new PLC_Device("D4000");
        public static PLC_Device PLC_Device_Z軸馬達速度 = new PLC_Device("D4001");
        public static PLC_Device PLC_Device_Z軸馬達加速度 = new PLC_Device("D4002");
        public static PLC_Device PLC_Device_Z軸馬達減速度 = new PLC_Device("D4003");

        public static PLC_Device PLC_Device_目標位置 = new PLC_Device("D4010");
        public static PLC_Device PLC_Device_第一層位置 = new PLC_Device("D4011");
        public static PLC_Device PLC_Device_第二層位置 = new PLC_Device("D4012");
        public static PLC_Device PLC_Device_第三層位置 = new PLC_Device("D4013");
        public static PLC_Device PLC_Device_第四層位置 = new PLC_Device("D4014");
        public static PLC_Device PLC_Device_第五層位置 = new PLC_Device("D4015");
        public static PLC_Device PLC_Device_頂層位置 = new PLC_Device("D4020");

        public static PLC_Device PLC_Device_Z軸馬達復歸 = new PLC_Device("S100");
        public static PLC_Device PLC_Device_移動到第一層位置 = new PLC_Device("S1001");
        public static PLC_Device PLC_Device_移動到第二層位置 = new PLC_Device("S1002");
        public static PLC_Device PLC_Device_移動到第三層位置 = new PLC_Device("S1003");
        public static PLC_Device PLC_Device_移動到第四層位置 = new PLC_Device("S1004");
        public static PLC_Device PLC_Device_移動到第五層位置 = new PLC_Device("S1005");
        public static PLC_Device PLC_Device_移動到頂層位置 = new PLC_Device("S1010");

        public static PLC_Device PLC_Device_輸送帶正轉 = new PLC_Device("S2000");
        public static PLC_Device PLC_Device_輸送帶正轉時間 = new PLC_Device("D2000");
        public static PLC_Device PLC_Device_輸送帶反轉 = new PLC_Device("S2001");
        public static PLC_Device PLC_Device_輸送帶反轉時間 = new PLC_Device("D2001");

        public static PLC_Device PLC_Device_輸送帶前進 = new PLC_Device("S2003");
        public static PLC_Device PLC_Device_輸送帶前進時間 = new PLC_Device("D2003");
        public static PLC_Device PLC_Device_輸送帶後退 = new PLC_Device("S2004");
        public static PLC_Device PLC_Device_輸送帶後退時間 = new PLC_Device("D2004");

        public static PLC_Device PLC_Device_出貨一次 = new PLC_Device("S3000");
        public static PLC_Device PLC_Device_出貨一次_Z軸層數 = new PLC_Device("D3000");
        public static string IP_出貨一次 = "";

        public static PLC_Device PLC_Device_出貨到領藥平台 = new PLC_Device("S3001");
   
        public bool flag_輸送帶在後方 = false;

        public static PLC_Device PLC_Device_Z軸馬達激磁 = new PLC_Device("Y10");
        public static PLC_Device PLC_Device_Z軸馬達原點狀態 = new PLC_Device("Y11");
        public static PLC_Device PLC_Device_Z軸Alarm = new PLC_Device("Y12");
        public static PLC_Device PLC_Device_Z軸Ready = new PLC_Device("Y13");

        public bool flag_program_PLC_int = false;
        public bool flag_minasA6_isOpen = false;
        public bool flag_servoOn = false;
        public bool flag_servoHome = false;
        public bool flag_servoStop = false;
        public bool flag_servoClearAlarm = false;
        public bool flag_servoJogPos = false;
        public bool flag_servoJogNeg = false;
        public byte deviceID = 1;

        private MyThread myThread_PLC;

        public void Program_PLC()
        {
            if(flag_program_PLC_int == false)
            {
                minasA6 = new MinasA6(myConfigClass.ServoZ_Com);
                try
                {
                    minasA6.Open();
                    flag_minasA6_isOpen = true;
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog($"Exception : {ex.Message}");
                }
                flag_program_PLC_int = true;
            }

            plC_RJ_Button_Z軸激磁.MouseDownEvent += PlC_RJ_Button_Z軸激磁_MouseDownEvent;
            plC_RJ_Button_Z軸復歸.MouseDownEvent += PlC_RJ_Button_Z軸復歸_MouseDownEvent;
            plC_RJ_Button_Z軸Alarm.MouseDownEvent += PlC_RJ_Button_Z軸Alarm_MouseDownEvent;

            plC_RJ_Button_Z軸停止.MouseDownEvent += PlC_RJ_Button_Z軸停止_MouseDownEvent;
            plC_RJ_Button_Z軸上升.MouseDownEvent += PlC_RJ_Button_Z軸上升_MouseDownEvent;
            plC_RJ_Button_Z軸下降.MouseDownEvent += PlC_RJ_Button_Z軸下降_MouseDownEvent;

            myThread_PLC = new MyThread();
            myThread_PLC.Add_Method(sub_Program_PLC);
            myThread_PLC.SetSleepTime(1);
            myThread_PLC.AutoRun(true);
            myThread_PLC.Trigger();
        }
        public void sub_Program_PLC()
        {
            if (flag_minasA6_isOpen == true)
            {
                try
                {
                    var servo = minasA6.GetServoStatus(deviceID);
                    var ready = !minasA6.IsBusy(deviceID);
                    var alarm = minasA6.GetServoAlarmStatus(deviceID);

                    var limit = minasA6.GetLimitStatus(deviceID);

                    int pos = minasA6.GetPosition(deviceID);

                    PLC_Device_Z軸馬達位置.Value = pos;
                    PLC_Device_Z軸馬達激磁.Bool = servo;
                    PLC_Device_Z軸馬達原點狀態.Bool = limit.Home;
                    PLC_Device_Z軸Alarm.Bool = alarm;
                    PLC_Device_Z軸Ready.Bool = ready;
                    //lbPositive.BackColor = limit.Positive ? Color.Red : Color.Green;
                    //lbNegative.BackColor = limit.Negative ? Color.Red : Color.Green;
                    if (flag_servoClearAlarm)
                    {
                        flag_servoClearAlarm = false;
                        if (PLC_Device_Z軸Alarm.Bool)
                        {
                            MyMessageBox.ShowDialog("Z軸警報解除");
                        }
                    }
                    if (flag_servoOn)
                    {
                        if (PLC_Device_Z軸馬達激磁.Bool == false) minasA6.ServoOn(deviceID);
                        else minasA6.ServoOff(deviceID);
                        flag_servoOn = false;
                    }
                    if (flag_servoHome)
                    {
                        minasA6.Home(deviceID, HomeMode.HomeSensorZPhase);
                        flag_servoHome = false;
                    }
                    if (flag_servoStop)
                    {
                        if (ready == false)
                        {
                            minasA6.S_Stop(deviceID);
                        }
                        flag_servoStop = false;
                    }


                    if (flag_servoJogPos)
                    {
                        if (ready == true)
                        {
                            minasA6.JogPositive(deviceID, PLC_Device_Z軸馬達速度.Value, PLC_Device_Z軸馬達加速度.Value, PLC_Device_Z軸馬達減速度.Value);
                        }
                        flag_servoJogPos = false;
                    }
                    if (flag_servoJogNeg)
                    {
                        if (ready == true)
                        {
                            minasA6.JogNegative(deviceID, PLC_Device_Z軸馬達速度.Value, PLC_Device_Z軸馬達加速度.Value, PLC_Device_Z軸馬達減速度.Value);
                        }
                        flag_servoJogNeg = false;
                    }
                }
                catch(Exception ex)
                {
                    Logger.Log("Z-erroe", $"Exception : {ex.Message}");
                }
                

                sub_Program_Z軸絕對位置移動();
                sub_Program_Z軸移動到第一層();
                sub_Program_Z軸移動到第二層();
                sub_Program_Z軸移動到第三層();
                sub_Program_Z軸移動到第四層();
                sub_Program_Z軸移動到第五層();
                sub_Program_Z軸移動到頂層();

                sub_Program_輸送帶正轉();
                sub_Program_輸送帶反轉();
                sub_Program_輸送帶前進();
                sub_Program_輸送帶後退();
                sub_Program_出貨一次();
                sub_Program_出貨到領藥平台();
            }
        }

        #region PLC_出貨一次
        MyTimerBasic MyTimerBasic_出貨一次_檢查延遲 = new MyTimerBasic();
        Task Task_出貨一次;
        int MotorCnt = 0;
        int MotorDelayCnt = 0;
        MyTimer MyTimer_出貨一次_結束延遲 = new MyTimer();
        int cnt_Program_出貨一次 = 65534;
        void sub_Program_出貨一次()
        {
            if (PLC_Device_出貨到領藥平台.Bool) PLC_Device_出貨一次.Bool = false;
            if (cnt_Program_出貨一次 == 65534)
            {
                this.MyTimer_出貨一次_結束延遲.StartTickTime(10000);
                PLC_Device_出貨一次.SetComment("PLC_出貨一次");
                PLC_Device_出貨一次.Bool = false;
                cnt_Program_出貨一次 = 65535;
            }
            if (cnt_Program_出貨一次 == 65535) cnt_Program_出貨一次 = 1;
            if (cnt_Program_出貨一次 == 1) cnt_Program_出貨一次_檢查按下(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 2) cnt_Program_出貨一次_初始化(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 3) cnt_Program_出貨一次_等待輸送帶後退(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 4) cnt_Program_出貨一次_輸送帶後退完成(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 5) cnt_Program_出貨一次_等待Z軸移動到層數(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 6) cnt_Program_出貨一次_Z軸移動到層數完成(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 7) cnt_Program_出貨一次_等待輸送帶前進(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 8) cnt_Program_出貨一次_輸送帶前進完成(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 9) cnt_Program_出貨一次_出料一次開始(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 10) cnt_Program_出貨一次_等待出料一次完成(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 11) cnt_Program_出貨一次_等待輸送帶後退(ref cnt_Program_出貨一次);
            if (cnt_Program_出貨一次 == 12) cnt_Program_出貨一次_輸送帶後退完成(ref cnt_Program_出貨一次);
         
            if (cnt_Program_出貨一次 == 13) cnt_Program_出貨一次 = 65500;
            if (cnt_Program_出貨一次 > 1) cnt_Program_出貨一次_檢查放開(ref cnt_Program_出貨一次);

            if (cnt_Program_出貨一次 == 65500)
            {
                //minasA6.S_Stop(deviceID);
                this.MyTimer_出貨一次_結束延遲.TickStop();
                this.MyTimer_出貨一次_結束延遲.StartTickTime(10000);
                PLC_Device_出貨一次.Bool = false;

                PLC_Device_Z軸移動到頂層.Bool = false;
                PLC_Device_輸送帶後退.Bool = false;
                PLC_Device_輸送帶前進.Bool = false;
                PLC_Device_移動到第一層位置.Bool = false;
                PLC_Device_移動到第二層位置.Bool = false;
                PLC_Device_移動到第三層位置.Bool = false;
                PLC_Device_移動到第四層位置.Bool = false;
                PLC_Device_移動到第五層位置.Bool = false;
                cnt_Program_出貨一次 = 65535;
            }
        }
        void cnt_Program_出貨一次_檢查按下(ref int cnt)
        {
            if (PLC_Device_出貨一次.Bool) cnt++;
        }
        void cnt_Program_出貨一次_檢查放開(ref int cnt)
        {
            if (!PLC_Device_出貨一次.Bool) cnt = 65500;
        }
        void cnt_Program_出貨一次_初始化(ref int cnt)
        {
            if(plC_ScreenPage_Main.PageText == "工程模式") IP_出貨一次 = this.rJ_TextBox_出貨一次_IP.Text;

            if (IP_出貨一次.Check_IP_Adress() == false)
            {
                Console.WriteLine($"[出貨一次] - IP字元異常,{IP_出貨一次}");
                cnt = 65500;
                return;
            }
            List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses = storageMedBoxIOConfigClass.get_all(API_Server, ServerName, ServerType);
            storageMedBoxIOConfigClass storageMedBoxIO = storageMedBoxIOConfigClasses.Where(x => x.IP == IP_出貨一次).FirstOrDefault();
            if (storageMedBoxIO == null)
            {
                Console.WriteLine($"[出貨一次] - 伺服器找無此IP({IP_出貨一次})資料");
                cnt = 65500;
                return;
            }
            string udp_json = storageUI_EPD_266.GetUDPJsonString(IP_出貨一次);
            if(udp_json.StringIsEmpty())
            {
                Console.WriteLine($"[出貨一次] - UdpJson異常");
                cnt = 65500;
                return;
            }
            UDP_READ_basic uDP_READ_Basic = udp_json.JsonDeserializet<UDP_READ_basic>();

   
            if (uDP_READ_Basic == null)
            {
                Console.WriteLine($"[出貨一次] - UdpJson異常");
                cnt = 65500;
                return;
            }
            if (storageMedBoxIO.出料位置Y.StringIsInt32() == false)
            {
                Console.WriteLine($"[出貨一次] - 出料位置({storageMedBoxIO.出料位置Y})參數錯誤");
                cnt = 65500;
                return;
            }
            MotorCnt = uDP_READ_Basic.FADC_motorCnt;
            Console.WriteLine($"[出貨一次] - {IP_出貨一次} ,MotorCnt({MotorCnt})參數");
            int temp = storageMedBoxIO.出料位置Y.StringToInt32();

            if (temp < 0) temp = 1;
            if (temp > 5) temp = 5;

            PLC_Device_出貨一次_Z軸層數.Value = temp;
            if (PLC_Device_Z軸Ready.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_出貨一次_等待輸送帶後退(ref int cnt)
        {
            if(flag_輸送帶在後方)
            {
                Console.WriteLine($"[出貨到領藥平台] - 輸送帶已經在後方");

                cnt++;
                return;
            }
            if(PLC_Device_輸送帶後退.Bool == false)
            {
                Console.WriteLine($"[出貨一次] - 等待輸送帶後退");
                PLC_Device_輸送帶後退.Bool = true;
                cnt++;
           }      
        }
        void cnt_Program_出貨一次_輸送帶後退完成(ref int cnt)
        {
            if (flag_輸送帶在後方)
            {
                Console.WriteLine($"[出貨到領藥平台] - 輸送帶已經在後方");

                cnt++;
                return;
            }
            if (PLC_Device_輸送帶後退.Bool == false)
            {
                Console.WriteLine($"[出貨一次] - 等待輸送帶完成");
                cnt++;
            }
        }
        void cnt_Program_出貨一次_等待Z軸移動到層數(ref int cnt)
        {
          
            if (PLC_Device_出貨一次_Z軸層數.Value == 1)
            {
                if(PLC_Device_移動到第一層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - 等待Z軸移動到層數({PLC_Device_出貨一次_Z軸層數.Value})");
                    PLC_Device_移動到第一層位置.Bool = true;
                    cnt++;
                }
            }
            if (PLC_Device_出貨一次_Z軸層數.Value == 2)
            {
                if (PLC_Device_移動到第二層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - 等待Z軸移動到層數({PLC_Device_出貨一次_Z軸層數.Value})");
                    PLC_Device_移動到第二層位置.Bool = true;
                    cnt++;
                }
            }
            if (PLC_Device_出貨一次_Z軸層數.Value == 3)
            {
                if (PLC_Device_移動到第三層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - 等待Z軸移動到層數({PLC_Device_出貨一次_Z軸層數.Value})");
                    PLC_Device_移動到第三層位置.Bool = true;
                    cnt++;
                }
            }
            if (PLC_Device_出貨一次_Z軸層數.Value == 4)
            {
                if (PLC_Device_移動到第四層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - 等待Z軸移動到層數({PLC_Device_出貨一次_Z軸層數.Value})");
                    PLC_Device_移動到第四層位置.Bool = true;
                    cnt++;
                }
            }
            if (PLC_Device_出貨一次_Z軸層數.Value == 5)
            {
                if (PLC_Device_移動到第五層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - 等待Z軸移動到層數({PLC_Device_出貨一次_Z軸層數.Value})");
                    PLC_Device_移動到第五層位置.Bool = true;
                    cnt++;
                }
            }
        }
        void cnt_Program_出貨一次_Z軸移動到層數完成(ref int cnt)
        {

            if (PLC_Device_出貨一次_Z軸層數.Value == 1)
            {
                if (PLC_Device_移動到第一層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - Z軸移動到層數完成({PLC_Device_出貨一次_Z軸層數.Value})");
                    cnt++;
                }
            }
            if (PLC_Device_出貨一次_Z軸層數.Value == 2)
            {
                if (PLC_Device_移動到第二層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - Z軸移動到層數完成({PLC_Device_出貨一次_Z軸層數.Value})");
                    cnt++;
                }
            }
            if (PLC_Device_出貨一次_Z軸層數.Value == 3)
            {
                if (PLC_Device_移動到第三層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - Z軸移動到層數完成({PLC_Device_出貨一次_Z軸層數.Value})");
                    cnt++;
                }
            }
            if (PLC_Device_出貨一次_Z軸層數.Value == 4)
            {
                if (PLC_Device_移動到第四層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - Z軸移動到層數完成({PLC_Device_出貨一次_Z軸層數.Value})");
                    cnt++;
                }
            }
            if (PLC_Device_出貨一次_Z軸層數.Value == 5)
            {
                if (PLC_Device_移動到第五層位置.Bool == false)
                {
                    Console.WriteLine($"[出貨一次] - Z軸移動到層數完成({PLC_Device_出貨一次_Z軸層數.Value})");
                    cnt++;
                }
            }
        }
        void cnt_Program_出貨一次_等待輸送帶前進(ref int cnt)
        {
            if (PLC_Device_輸送帶前進.Bool == false)
            {
                Console.WriteLine($"[出貨一次] - 等待輸送帶前進");
                PLC_Device_輸送帶前進.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_出貨一次_輸送帶前進完成(ref int cnt)
        {
            if (PLC_Device_輸送帶前進.Bool == false)
            {
                Console.WriteLine($"[出貨一次] - 輸送帶前進完成");
                cnt++;
            }
        }
        void cnt_Program_出貨一次_出料一次開始(ref int cnt)
        {
            PLC_Device_輸送帶反轉.Bool = true;
           
            List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses = storageMedBoxIOConfigClass.get_all(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType);
            storageMedBoxIOConfigClass storageMedBoxIO = storageMedBoxIOConfigClasses.Where(x => x.IP == IP_出貨一次).FirstOrDefault();
            Console.WriteLine($"[出貨一次] - 出料一次開始");
            int time = 0;
            if (storageMedBoxIO!= null)
            {
                if (storageMedBoxIO.出料馬達輸入延遲時間.StringIsInt32())
                {
                    time = storageMedBoxIO.出料馬達輸入延遲時間.StringToInt32();
                    if (time < 0) time = 0;
                }
            }
            this.storageUI_EPD_266.Set_ADCMotorTrigger(IP_出貨一次, 29000, time);
            cnt++;
        }
        void cnt_Program_出貨一次_等待出料一次完成(ref int cnt)
        {
            string udp_json = storageUI_EPD_266.GetUDPJsonString(IP_出貨一次);
            UDP_READ_basic uDP_READ_Basic = udp_json.JsonDeserializet<UDP_READ_basic>();
            if (uDP_READ_Basic != null)
            {
                if(uDP_READ_Basic.FADC_motorCnt != MotorCnt)
                {
                    Console.WriteLine($"[出貨一次] - 出料一次完成");
                    cnt++;
                }
        
            }
           
        }



        #endregion
        #region PLC_出貨到領藥平台
        MyTimerBasic MyTimerBasic_出貨到領藥平台_檢查延遲 = new MyTimerBasic();
        Task Task_出貨到領藥平台;

        MyTimer MyTimer_出貨到領藥平台_結束延遲 = new MyTimer();
        int cnt_Program_出貨到領藥平台 = 65534;
        void sub_Program_出貨到領藥平台()
        {
            if (PLC_Device_出貨一次.Bool) PLC_Device_出貨到領藥平台.Bool = false;

            if (cnt_Program_出貨到領藥平台 == 65534)
            {
                this.MyTimer_出貨到領藥平台_結束延遲.StartTickTime(10000);
                PLC_Device_出貨到領藥平台.SetComment("PLC_出貨到領藥平台");
                PLC_Device_出貨到領藥平台.Bool = false;
                cnt_Program_出貨到領藥平台 = 65535;
            }
            if (cnt_Program_出貨到領藥平台 == 65535) cnt_Program_出貨到領藥平台 = 1;
            if (cnt_Program_出貨到領藥平台 == 1) cnt_Program_出貨到領藥平台_檢查按下(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 2) cnt_Program_出貨到領藥平台_初始化(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 3) cnt_Program_出貨到領藥平台_等待輸送帶後退(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 4) cnt_Program_出貨到領藥平台_輸送帶後退完成(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 5) cnt_Program_出貨到領藥平台_等待Z軸移動到頂層(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 6) cnt_Program_出貨到領藥平台_Z軸移動到頂層結束(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 7) cnt_Program_出貨到領藥平台_等待輸送帶前進(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 8) cnt_Program_出貨到領藥平台_輸送帶前進完成(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 9) cnt_Program_出貨到領藥平台_輸送帶正轉開始(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 10) cnt_Program_出貨到領藥平台_輸送帶正轉完成(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 11) cnt_Program_出貨到領藥平台_等待輸送帶後退(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 12) cnt_Program_出貨到領藥平台_輸送帶後退完成(ref cnt_Program_出貨到領藥平台);
            if (cnt_Program_出貨到領藥平台 == 13) cnt_Program_出貨到領藥平台 = 65500;
            if (cnt_Program_出貨到領藥平台 > 1) cnt_Program_出貨到領藥平台_檢查放開(ref cnt_Program_出貨到領藥平台);

            if (cnt_Program_出貨到領藥平台 == 65500)
            {
                //minasA6.S_Stop(deviceID);
                this.MyTimer_出貨到領藥平台_結束延遲.TickStop();
                this.MyTimer_出貨到領藥平台_結束延遲.StartTickTime(10000);
                PLC_Device_出貨到領藥平台.Bool = false;
                PLC_Device_Z軸移動到頂層.Bool = false;
                PLC_Device_輸送帶後退.Bool = false;
                PLC_Device_輸送帶前進.Bool = false;
                PLC_Device_移動到第一層位置.Bool = false;
                PLC_Device_移動到第二層位置.Bool = false;
                PLC_Device_移動到第三層位置.Bool = false;
                PLC_Device_移動到第四層位置.Bool = false;
                PLC_Device_移動到第五層位置.Bool = false;
                cnt_Program_出貨到領藥平台 = 65535;
            }
        }
        void cnt_Program_出貨到領藥平台_檢查按下(ref int cnt)
        {
            if (PLC_Device_出貨到領藥平台.Bool) cnt++;
        }
        void cnt_Program_出貨到領藥平台_檢查放開(ref int cnt)
        {
            if (!PLC_Device_出貨到領藥平台.Bool) cnt = 65500;
        }
        void cnt_Program_出貨到領藥平台_初始化(ref int cnt)
        {
           
          
            if (PLC_Device_Z軸Ready.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_出貨到領藥平台_等待輸送帶後退(ref int cnt)
        {
            if (flag_輸送帶在後方)
            {
                Console.WriteLine($"[出貨到領藥平台] - 輸送帶已經在後方");
                cnt++;
                return;
            }
            else if (PLC_Device_輸送帶後退.Bool == false)
            {
                Console.WriteLine($"[出貨到領藥平台] - 等待輸送帶後退");
                PLC_Device_輸送帶後退.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_出貨到領藥平台_輸送帶後退完成(ref int cnt)
        {
            if (flag_輸送帶在後方)
            {
                Console.WriteLine($"[出貨到領藥平台] - 輸送帶已經在後方");
                cnt++;
                return;
            }
            else if (PLC_Device_輸送帶後退.Bool == false)
            {
                Console.WriteLine($"[出貨到領藥平台] - 等待輸送帶完成");
                cnt++;
            }
        }


        void cnt_Program_出貨到領藥平台_等待Z軸移動到頂層(ref int cnt)
        {
            if (PLC_Device_Z軸移動到頂層.Bool == false)
            {
                Console.WriteLine($"[出貨到領藥平台] - 等待Z軸移動到頂層");
                PLC_Device_Z軸移動到頂層.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_出貨到領藥平台_Z軸移動到頂層結束(ref int cnt)
        {
            if (PLC_Device_Z軸移動到頂層.Bool == false)
            {
                Console.WriteLine($"[出貨到領藥平台] - Z軸移動到頂層完成");
                cnt++;
            }
        }

        void cnt_Program_出貨到領藥平台_等待輸送帶前進(ref int cnt)
        {
            if (PLC_Device_輸送帶前進.Bool == false)
            {
                Console.WriteLine($"[出貨到領藥平台] - 等待輸送帶前進");
                PLC_Device_輸送帶前進.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_出貨到領藥平台_輸送帶前進完成(ref int cnt)
        {
            if (PLC_Device_輸送帶前進.Bool == false)
            {
                Console.WriteLine($"[出貨到領藥平台] - 輸送帶前進完成");
                cnt++;
            }
        }

        void cnt_Program_出貨到領藥平台_輸送帶正轉開始(ref int cnt)
        {
            if (PLC_Device_輸送帶正轉.Bool == false)
            {
                Console.WriteLine($"[出貨到領藥平台] - 輸送帶正轉開始");
                PLC_Device_輸送帶正轉.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_出貨到領藥平台_輸送帶正轉完成(ref int cnt)
        {
            if (PLC_Device_輸送帶正轉.Bool == false)
            {
                Console.WriteLine($"[出貨到領藥平台] - 輸送帶正轉完成");
                cnt++;
            }
        }
        #endregion


        #region PLC_Z軸絕對位置移動
        PLC_Device PLC_Device_Z軸絕對位置移動 = new PLC_Device("S1000");
        PLC_Device PLC_Device_Z軸絕對位置移動_OK = new PLC_Device("S1000");
        MyTimerBasic MyTimerBasic_Z軸絕對位置移動_檢查延遲 = new MyTimerBasic();
        Task Task_Z軸絕對位置移動;
        MyTimer MyTimer_Z軸絕對位置移動_結束延遲 = new MyTimer();
        int cnt_Program_Z軸絕對位置移動 = 65534;
        void sub_Program_Z軸絕對位置移動()
        {
            if (cnt_Program_Z軸絕對位置移動 == 65534)
            {
                this.MyTimer_Z軸絕對位置移動_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸絕對位置移動.SetComment("PLC_Z軸絕對位置移動");
                PLC_Device_Z軸絕對位置移動_OK.SetComment("PLC_Z軸絕對位置移動_OK");
                PLC_Device_Z軸絕對位置移動.Bool = false;
                cnt_Program_Z軸絕對位置移動 = 65535;
            }
            if (cnt_Program_Z軸絕對位置移動 == 65535) cnt_Program_Z軸絕對位置移動 = 1;
            if (cnt_Program_Z軸絕對位置移動 == 1) cnt_Program_Z軸絕對位置移動_檢查按下(ref cnt_Program_Z軸絕對位置移動);
            if (cnt_Program_Z軸絕對位置移動 == 2) cnt_Program_Z軸絕對位置移動_初始化(ref cnt_Program_Z軸絕對位置移動);
            if (cnt_Program_Z軸絕對位置移動 == 3) cnt_Program_Z軸絕對位置移動_開始移動(ref cnt_Program_Z軸絕對位置移動);
            if (cnt_Program_Z軸絕對位置移動 == 4) cnt_Program_Z軸絕對位置移動_等待移動完成(ref cnt_Program_Z軸絕對位置移動);
            if (cnt_Program_Z軸絕對位置移動 == 5) cnt_Program_Z軸絕對位置移動 = 65500;
            if (cnt_Program_Z軸絕對位置移動 > 1) cnt_Program_Z軸絕對位置移動_檢查放開(ref cnt_Program_Z軸絕對位置移動);

            if (cnt_Program_Z軸絕對位置移動 == 65500)
            {
                //minasA6.S_Stop(deviceID);
                this.MyTimer_Z軸絕對位置移動_結束延遲.TickStop();
                this.MyTimer_Z軸絕對位置移動_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸絕對位置移動.Bool = false;
                PLC_Device_Z軸絕對位置移動_OK.Bool = false;
                cnt_Program_Z軸絕對位置移動 = 65535;
            }
        }
        void cnt_Program_Z軸絕對位置移動_檢查按下(ref int cnt)
        {
            if (PLC_Device_Z軸絕對位置移動.Bool) cnt++;
        }
        void cnt_Program_Z軸絕對位置移動_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool) cnt = 65500;
        }
        void cnt_Program_Z軸絕對位置移動_初始化(ref int cnt)
        {
            if(PLC_Device_Z軸Ready.Bool)
            {
                cnt++;
            }       
        }
        void cnt_Program_Z軸絕對位置移動_開始移動(ref int cnt)
        {

            minasA6.MoveAbsolute(deviceID, PLC_Device_目標位置.Value, PLC_Device_Z軸馬達速度.Value, PLC_Device_Z軸馬達加速度.Value, PLC_Device_Z軸馬達減速度.Value);
            MyTimerBasic_Z軸絕對位置移動_檢查延遲.TickStop();
            MyTimerBasic_Z軸絕對位置移動_檢查延遲.StartTickTime(100);

            Console.WriteLine( $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [Z軸移動] DeviceID={deviceID}, 目標位置={PLC_Device_目標位置.Value}, 速度={PLC_Device_Z軸馬達速度.Value}, 加速度={PLC_Device_Z軸馬達加速度.Value}, 減速度={PLC_Device_Z軸馬達減速度.Value}");
            cnt++;
        }
        void cnt_Program_Z軸絕對位置移動_等待移動完成(ref int cnt)
        {
            if(MyTimerBasic_Z軸絕對位置移動_檢查延遲.IsTimeOut() || true)
            {
                //minasA6.MoveAbsolute(deviceID, PLC_Device_目標位置.Value, PLC_Device_Z軸馬達速度.Value, PLC_Device_Z軸馬達加速度.Value, PLC_Device_Z軸馬達減速度.Value);

                int pos = minasA6.GetPosition(deviceID);

                if (PLC_Device_目標位置.Value >= pos - 100 && PLC_Device_目標位置.Value <= pos + 100)
                {
                    cnt++;
                }

            }         
        }






        #endregion
        #region PLC_Z軸移動到第一層
        PLC_Device PLC_Device_Z軸移動到第一層 = new PLC_Device("S1001");
        PLC_Device PLC_Device_Z軸移動到第一層_OK = new PLC_Device("S1001");
        Task Task_Z軸移動到第一層;
        MyTimer MyTimer_Z軸移動到第一層_結束延遲 = new MyTimer();
        int cnt_Program_Z軸移動到第一層 = 65534;
        void sub_Program_Z軸移動到第一層()
        {
            if (cnt_Program_Z軸移動到第一層 == 65534)
            {
                this.MyTimer_Z軸移動到第一層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第一層.SetComment("PLC_Z軸移動到第一層");
                PLC_Device_Z軸移動到第一層_OK.SetComment("PLC_Z軸移動到第一層_OK");
                PLC_Device_Z軸移動到第一層.Bool = false;
                cnt_Program_Z軸移動到第一層 = 65535;
            }
            if (cnt_Program_Z軸移動到第一層 == 65535) cnt_Program_Z軸移動到第一層 = 1;
            if (cnt_Program_Z軸移動到第一層 == 1) cnt_Program_Z軸移動到第一層_檢查按下(ref cnt_Program_Z軸移動到第一層);
            if (cnt_Program_Z軸移動到第一層 == 2) cnt_Program_Z軸移動到第一層_初始化(ref cnt_Program_Z軸移動到第一層);
            if (cnt_Program_Z軸移動到第一層 == 3) cnt_Program_Z軸移動到第一層_等待移動完成(ref cnt_Program_Z軸移動到第一層);
            if (cnt_Program_Z軸移動到第一層 == 4) cnt_Program_Z軸移動到第一層 = 65500;
            if (cnt_Program_Z軸移動到第一層 > 1) cnt_Program_Z軸移動到第一層_檢查放開(ref cnt_Program_Z軸移動到第一層);

            if (cnt_Program_Z軸移動到第一層 == 65500)
            {
                PLC_Device_Z軸絕對位置移動.Bool = false;
                this.MyTimer_Z軸移動到第一層_結束延遲.TickStop();
                this.MyTimer_Z軸移動到第一層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第一層.Bool = false;
                PLC_Device_Z軸移動到第一層_OK.Bool = false;
                cnt_Program_Z軸移動到第一層 = 65535;
            }
        }
        void cnt_Program_Z軸移動到第一層_檢查按下(ref int cnt)
        {
            if (PLC_Device_Z軸移動到第一層.Bool) cnt++;
        }
        void cnt_Program_Z軸移動到第一層_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Z軸移動到第一層.Bool) cnt = 65500;
        }
        void cnt_Program_Z軸移動到第一層_初始化(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                PLC_Device_目標位置.Value = PLC_Device_第一層位置.Value;
                PLC_Device_Z軸絕對位置移動.Bool = true;
                cnt++;
            }
        }
  
        void cnt_Program_Z軸移動到第一層_等待移動完成(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                cnt++;
            }
        }






        #endregion
        #region PLC_Z軸移動到第二層
        PLC_Device PLC_Device_Z軸移動到第二層 = new PLC_Device("S1002");
        PLC_Device PLC_Device_Z軸移動到第二層_OK = new PLC_Device("S1002");
        Task Task_Z軸移動到第二層;
        MyTimer MyTimer_Z軸移動到第二層_結束延遲 = new MyTimer();
        int cnt_Program_Z軸移動到第二層 = 65534;
        void sub_Program_Z軸移動到第二層()
        {
            if (cnt_Program_Z軸移動到第二層 == 65534)
            {
                this.MyTimer_Z軸移動到第二層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第二層.SetComment("PLC_Z軸移動到第二層");
                PLC_Device_Z軸移動到第二層_OK.SetComment("PLC_Z軸移動到第二層_OK");
                PLC_Device_Z軸移動到第二層.Bool = false;
                cnt_Program_Z軸移動到第二層 = 65535;
            }
            if (cnt_Program_Z軸移動到第二層 == 65535) cnt_Program_Z軸移動到第二層 = 1;
            if (cnt_Program_Z軸移動到第二層 == 1) cnt_Program_Z軸移動到第二層_檢查按下(ref cnt_Program_Z軸移動到第二層);
            if (cnt_Program_Z軸移動到第二層 == 2) cnt_Program_Z軸移動到第二層_初始化(ref cnt_Program_Z軸移動到第二層);
            if (cnt_Program_Z軸移動到第二層 == 3) cnt_Program_Z軸移動到第二層_等待移動完成(ref cnt_Program_Z軸移動到第二層);
            if (cnt_Program_Z軸移動到第二層 == 4) cnt_Program_Z軸移動到第二層 = 65500;
            if (cnt_Program_Z軸移動到第二層 > 1) cnt_Program_Z軸移動到第二層_檢查放開(ref cnt_Program_Z軸移動到第二層);

            if (cnt_Program_Z軸移動到第二層 == 65500)
            {
                PLC_Device_Z軸絕對位置移動.Bool = false;
                this.MyTimer_Z軸移動到第二層_結束延遲.TickStop();
                this.MyTimer_Z軸移動到第二層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第二層.Bool = false;
                PLC_Device_Z軸移動到第二層_OK.Bool = false;
                cnt_Program_Z軸移動到第二層 = 65535;
            }
        }
        void cnt_Program_Z軸移動到第二層_檢查按下(ref int cnt)
        {
            if (PLC_Device_Z軸移動到第二層.Bool) cnt++;
        }
        void cnt_Program_Z軸移動到第二層_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Z軸移動到第二層.Bool) cnt = 65500;
        }
        void cnt_Program_Z軸移動到第二層_初始化(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                PLC_Device_目標位置.Value = PLC_Device_第二層位置.Value;
                PLC_Device_Z軸絕對位置移動.Bool = true;
                cnt++;
            }
        }

        void cnt_Program_Z軸移動到第二層_等待移動完成(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                cnt++;
            }
        }






        #endregion
        #region PLC_Z軸移動到第三層
        PLC_Device PLC_Device_Z軸移動到第三層 = new PLC_Device("S1003");
        PLC_Device PLC_Device_Z軸移動到第三層_OK = new PLC_Device("S1003");
        Task Task_Z軸移動到第三層;
        MyTimer MyTimer_Z軸移動到第三層_結束延遲 = new MyTimer();
        int cnt_Program_Z軸移動到第三層 = 65534;
        void sub_Program_Z軸移動到第三層()
        {
            if (cnt_Program_Z軸移動到第三層 == 65534)
            {
                this.MyTimer_Z軸移動到第三層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第三層.SetComment("PLC_Z軸移動到第三層");
                PLC_Device_Z軸移動到第三層_OK.SetComment("PLC_Z軸移動到第三層_OK");
                PLC_Device_Z軸移動到第三層.Bool = false;
                cnt_Program_Z軸移動到第三層 = 65535;
            }
            if (cnt_Program_Z軸移動到第三層 == 65535) cnt_Program_Z軸移動到第三層 = 1;
            if (cnt_Program_Z軸移動到第三層 == 1) cnt_Program_Z軸移動到第三層_檢查按下(ref cnt_Program_Z軸移動到第三層);
            if (cnt_Program_Z軸移動到第三層 == 2) cnt_Program_Z軸移動到第三層_初始化(ref cnt_Program_Z軸移動到第三層);
            if (cnt_Program_Z軸移動到第三層 == 3) cnt_Program_Z軸移動到第三層_等待移動完成(ref cnt_Program_Z軸移動到第三層);
            if (cnt_Program_Z軸移動到第三層 == 4) cnt_Program_Z軸移動到第三層 = 65500;
            if (cnt_Program_Z軸移動到第三層 > 1) cnt_Program_Z軸移動到第三層_檢查放開(ref cnt_Program_Z軸移動到第三層);

            if (cnt_Program_Z軸移動到第三層 == 65500)
            {
                PLC_Device_Z軸絕對位置移動.Bool = false;
                this.MyTimer_Z軸移動到第三層_結束延遲.TickStop();
                this.MyTimer_Z軸移動到第三層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第三層.Bool = false;
                PLC_Device_Z軸移動到第三層_OK.Bool = false;
                cnt_Program_Z軸移動到第三層 = 65535;
            }
        }
        void cnt_Program_Z軸移動到第三層_檢查按下(ref int cnt)
        {
            if (PLC_Device_Z軸移動到第三層.Bool) cnt++;
        }
        void cnt_Program_Z軸移動到第三層_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Z軸移動到第三層.Bool) cnt = 65500;
        }
        void cnt_Program_Z軸移動到第三層_初始化(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                PLC_Device_目標位置.Value = PLC_Device_第三層位置.Value;
                PLC_Device_Z軸絕對位置移動.Bool = true;
                cnt++;
            }
        }

        void cnt_Program_Z軸移動到第三層_等待移動完成(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                cnt++;
            }
        }






        #endregion
        #region PLC_Z軸移動到第四層
        PLC_Device PLC_Device_Z軸移動到第四層 = new PLC_Device("S1004");
        PLC_Device PLC_Device_Z軸移動到第四層_OK = new PLC_Device("S1004");
        Task Task_Z軸移動到第四層;
        MyTimer MyTimer_Z軸移動到第四層_結束延遲 = new MyTimer();
        int cnt_Program_Z軸移動到第四層 = 65534;
        void sub_Program_Z軸移動到第四層()
        {
            if (cnt_Program_Z軸移動到第四層 == 65534)
            {
                this.MyTimer_Z軸移動到第四層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第四層.SetComment("PLC_Z軸移動到第四層");
                PLC_Device_Z軸移動到第四層_OK.SetComment("PLC_Z軸移動到第四層_OK");
                PLC_Device_Z軸移動到第四層.Bool = false;
                cnt_Program_Z軸移動到第四層 = 65535;
            }
            if (cnt_Program_Z軸移動到第四層 == 65535) cnt_Program_Z軸移動到第四層 = 1;
            if (cnt_Program_Z軸移動到第四層 == 1) cnt_Program_Z軸移動到第四層_檢查按下(ref cnt_Program_Z軸移動到第四層);
            if (cnt_Program_Z軸移動到第四層 == 2) cnt_Program_Z軸移動到第四層_初始化(ref cnt_Program_Z軸移動到第四層);
            if (cnt_Program_Z軸移動到第四層 == 3) cnt_Program_Z軸移動到第四層_等待移動完成(ref cnt_Program_Z軸移動到第四層);
            if (cnt_Program_Z軸移動到第四層 == 4) cnt_Program_Z軸移動到第四層 = 65500;
            if (cnt_Program_Z軸移動到第四層 > 1) cnt_Program_Z軸移動到第四層_檢查放開(ref cnt_Program_Z軸移動到第四層);

            if (cnt_Program_Z軸移動到第四層 == 65500)
            {
                PLC_Device_Z軸絕對位置移動.Bool = false;
                this.MyTimer_Z軸移動到第四層_結束延遲.TickStop();
                this.MyTimer_Z軸移動到第四層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第四層.Bool = false;
                PLC_Device_Z軸移動到第四層_OK.Bool = false;
                cnt_Program_Z軸移動到第四層 = 65535;
            }
        }
        void cnt_Program_Z軸移動到第四層_檢查按下(ref int cnt)
        {
            if (PLC_Device_Z軸移動到第四層.Bool) cnt++;
        }
        void cnt_Program_Z軸移動到第四層_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Z軸移動到第四層.Bool) cnt = 65500;
        }
        void cnt_Program_Z軸移動到第四層_初始化(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                PLC_Device_目標位置.Value = PLC_Device_第四層位置.Value;
                PLC_Device_Z軸絕對位置移動.Bool = true;
                cnt++;
            }
        }

        void cnt_Program_Z軸移動到第四層_等待移動完成(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                cnt++;
            }
        }






        #endregion
        #region PLC_Z軸移動到第五層
        PLC_Device PLC_Device_Z軸移動到第五層 = new PLC_Device("S1005");
        PLC_Device PLC_Device_Z軸移動到第五層_OK = new PLC_Device("S1005");
        Task Task_Z軸移動到第五層;
        MyTimer MyTimer_Z軸移動到第五層_結束延遲 = new MyTimer();
        int cnt_Program_Z軸移動到第五層 = 65534;
        void sub_Program_Z軸移動到第五層()
        {
            if (cnt_Program_Z軸移動到第五層 == 65534)
            {
                this.MyTimer_Z軸移動到第五層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第五層.SetComment("PLC_Z軸移動到第五層");
                PLC_Device_Z軸移動到第五層_OK.SetComment("PLC_Z軸移動到第五層_OK");
                PLC_Device_Z軸移動到第五層.Bool = false;
                cnt_Program_Z軸移動到第五層 = 65535;
            }
            if (cnt_Program_Z軸移動到第五層 == 65535) cnt_Program_Z軸移動到第五層 = 1;
            if (cnt_Program_Z軸移動到第五層 == 1) cnt_Program_Z軸移動到第五層_檢查按下(ref cnt_Program_Z軸移動到第五層);
            if (cnt_Program_Z軸移動到第五層 == 2) cnt_Program_Z軸移動到第五層_初始化(ref cnt_Program_Z軸移動到第五層);
            if (cnt_Program_Z軸移動到第五層 == 3) cnt_Program_Z軸移動到第五層_等待移動完成(ref cnt_Program_Z軸移動到第五層);
            if (cnt_Program_Z軸移動到第五層 == 4) cnt_Program_Z軸移動到第五層 = 65500;
            if (cnt_Program_Z軸移動到第五層 > 1) cnt_Program_Z軸移動到第五層_檢查放開(ref cnt_Program_Z軸移動到第五層);

            if (cnt_Program_Z軸移動到第五層 == 65500)
            {
                PLC_Device_Z軸絕對位置移動.Bool = false;
                this.MyTimer_Z軸移動到第五層_結束延遲.TickStop();
                this.MyTimer_Z軸移動到第五層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到第五層.Bool = false;
                PLC_Device_Z軸移動到第五層_OK.Bool = false;
                cnt_Program_Z軸移動到第五層 = 65535;
            }
        }
        void cnt_Program_Z軸移動到第五層_檢查按下(ref int cnt)
        {
            if (PLC_Device_Z軸移動到第五層.Bool) cnt++;
        }
        void cnt_Program_Z軸移動到第五層_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Z軸移動到第五層.Bool) cnt = 65500;
        }
        void cnt_Program_Z軸移動到第五層_初始化(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                PLC_Device_目標位置.Value = PLC_Device_第五層位置.Value;
                PLC_Device_Z軸絕對位置移動.Bool = true;
                cnt++;
            }
        }

        void cnt_Program_Z軸移動到第五層_等待移動完成(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                cnt++;
            }
        }






        #endregion
        #region PLC_Z軸移動到頂層
        PLC_Device PLC_Device_Z軸移動到頂層 = new PLC_Device("S1010");
        PLC_Device PLC_Device_Z軸移動到頂層_OK = new PLC_Device("S1010");
        Task Task_Z軸移動到頂層;
        MyTimer MyTimer_Z軸移動到頂層_結束延遲 = new MyTimer();
        int cnt_Program_Z軸移動到頂層 = 65534;
        void sub_Program_Z軸移動到頂層()
        {
            if (cnt_Program_Z軸移動到頂層 == 65534)
            {
                this.MyTimer_Z軸移動到頂層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到頂層.SetComment("PLC_Z軸移動到頂層");
                PLC_Device_Z軸移動到頂層_OK.SetComment("PLC_Z軸移動到頂層_OK");
                PLC_Device_Z軸移動到頂層.Bool = false;
                cnt_Program_Z軸移動到頂層 = 65535;
            }
            if (cnt_Program_Z軸移動到頂層 == 65535) cnt_Program_Z軸移動到頂層 = 1;
            if (cnt_Program_Z軸移動到頂層 == 1) cnt_Program_Z軸移動到頂層_檢查按下(ref cnt_Program_Z軸移動到頂層);
            if (cnt_Program_Z軸移動到頂層 == 2) cnt_Program_Z軸移動到頂層_初始化(ref cnt_Program_Z軸移動到頂層);
            if (cnt_Program_Z軸移動到頂層 == 3) cnt_Program_Z軸移動到頂層_等待移動完成(ref cnt_Program_Z軸移動到頂層);
            if (cnt_Program_Z軸移動到頂層 == 4) cnt_Program_Z軸移動到頂層 = 65500;
            if (cnt_Program_Z軸移動到頂層 > 1) cnt_Program_Z軸移動到頂層_檢查放開(ref cnt_Program_Z軸移動到頂層);

            if (cnt_Program_Z軸移動到頂層 == 65500)
            {
                PLC_Device_Z軸絕對位置移動.Bool = false;
                this.MyTimer_Z軸移動到頂層_結束延遲.TickStop();
                this.MyTimer_Z軸移動到頂層_結束延遲.StartTickTime(10000);
                PLC_Device_Z軸移動到頂層.Bool = false;
                PLC_Device_Z軸移動到頂層_OK.Bool = false;
                cnt_Program_Z軸移動到頂層 = 65535;
            }
        }
        void cnt_Program_Z軸移動到頂層_檢查按下(ref int cnt)
        {
            if (PLC_Device_Z軸移動到頂層.Bool) cnt++;
        }
        void cnt_Program_Z軸移動到頂層_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Z軸移動到頂層.Bool) cnt = 65500;
        }
        void cnt_Program_Z軸移動到頂層_初始化(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                PLC_Device_目標位置.Value = PLC_Device_頂層位置.Value;
                PLC_Device_Z軸絕對位置移動.Bool = true;
                cnt++;
            }
        }

        void cnt_Program_Z軸移動到頂層_等待移動完成(ref int cnt)
        {
            if (!PLC_Device_Z軸絕對位置移動.Bool)
            {
                cnt++;
            }
        }






        #endregion

        #region PLC_輸送帶正轉

        MyTimerBasic MyTimerBasic_輸送帶正轉_檢查延遲 = new MyTimerBasic();
        Task Task_輸送帶正轉;
        MyTimer MyTimer_輸送帶正轉_結束延遲 = new MyTimer();
        int cnt_Program_輸送帶正轉 = 65534;
        void sub_Program_輸送帶正轉()
        {
            if (cnt_Program_輸送帶正轉 == 65534)
            {
                this.MyTimer_輸送帶正轉_結束延遲.StartTickTime(10000);
                PLC_Device_輸送帶正轉.SetComment("PLC_輸送帶正轉");
                PLC_Device_輸送帶正轉.Bool = false;
                cnt_Program_輸送帶正轉 = 65535;
            }
            if (cnt_Program_輸送帶正轉 == 65535) cnt_Program_輸送帶正轉 = 1;
            if (cnt_Program_輸送帶正轉 == 1) cnt_Program_輸送帶正轉_檢查按下(ref cnt_Program_輸送帶正轉);
            if (cnt_Program_輸送帶正轉 == 2) cnt_Program_輸送帶正轉_初始化(ref cnt_Program_輸送帶正轉);
            if (cnt_Program_輸送帶正轉 == 3) cnt_Program_輸送帶正轉_開始移動(ref cnt_Program_輸送帶正轉);
            if (cnt_Program_輸送帶正轉 == 4) cnt_Program_輸送帶正轉_等待移動完成(ref cnt_Program_輸送帶正轉);
            if (cnt_Program_輸送帶正轉 == 5) cnt_Program_輸送帶正轉 = 65500;
            if (cnt_Program_輸送帶正轉 > 1) cnt_Program_輸送帶正轉_檢查放開(ref cnt_Program_輸送帶正轉);

            if (cnt_Program_輸送帶正轉 == 65500)
            {
                //minasA6.S_Stop(deviceID);
                this.MyTimer_輸送帶正轉_結束延遲.TickStop();
                this.MyTimer_輸送帶正轉_結束延遲.StartTickTime(10000);
                this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶正轉, false);
                this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶反轉, false);
                PLC_Device_輸送帶正轉.Bool = false;
                cnt_Program_輸送帶正轉 = 65535;
            }
        }
        void cnt_Program_輸送帶正轉_檢查按下(ref int cnt)
        {
            if (PLC_Device_輸送帶正轉.Bool) cnt++;
        }
        void cnt_Program_輸送帶正轉_檢查放開(ref int cnt)
        {
            if (!PLC_Device_輸送帶正轉.Bool) cnt = 65500;
        }
        void cnt_Program_輸送帶正轉_初始化(ref int cnt)
        {
            if (PLC_Device_Z軸Ready.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_輸送帶正轉_開始移動(ref int cnt)
        {
            this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶正轉, true);
            this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶反轉, false);
            MyTimerBasic_輸送帶正轉_檢查延遲.TickStop();
            MyTimerBasic_輸送帶正轉_檢查延遲.StartTickTime(PLC_Device_輸送帶正轉時間.Value);
            cnt++;
        }
        void cnt_Program_輸送帶正轉_等待移動完成(ref int cnt)
        {
            if (MyTimerBasic_輸送帶正轉_檢查延遲.IsTimeOut())
            {
                cnt++;
            }
        }






        #endregion
        #region PLC_輸送帶反轉
     
        MyTimerBasic MyTimerBasic_輸送帶反轉_檢查延遲 = new MyTimerBasic();
        Task Task_輸送帶反轉;
        MyTimer MyTimer_輸送帶反轉_結束延遲 = new MyTimer();
        int cnt_Program_輸送帶反轉 = 65534;
        void sub_Program_輸送帶反轉()
        {
            if (cnt_Program_輸送帶反轉 == 65534)
            {
                this.MyTimer_輸送帶反轉_結束延遲.StartTickTime(10000);
                PLC_Device_輸送帶反轉.SetComment("PLC_輸送帶反轉");
                PLC_Device_輸送帶反轉.Bool = false;
                cnt_Program_輸送帶反轉 = 65535;
            }
            if (cnt_Program_輸送帶反轉 == 65535) cnt_Program_輸送帶反轉 = 1;
            if (cnt_Program_輸送帶反轉 == 1) cnt_Program_輸送帶反轉_檢查按下(ref cnt_Program_輸送帶反轉);
            if (cnt_Program_輸送帶反轉 == 2) cnt_Program_輸送帶反轉_初始化(ref cnt_Program_輸送帶反轉);
            if (cnt_Program_輸送帶反轉 == 3) cnt_Program_輸送帶反轉_開始移動(ref cnt_Program_輸送帶反轉);
            if (cnt_Program_輸送帶反轉 == 4) cnt_Program_輸送帶反轉_等待移動完成(ref cnt_Program_輸送帶反轉);
            if (cnt_Program_輸送帶反轉 == 5) cnt_Program_輸送帶反轉 = 65500;
            if (cnt_Program_輸送帶反轉 > 1) cnt_Program_輸送帶反轉_檢查放開(ref cnt_Program_輸送帶反轉);

            if (cnt_Program_輸送帶反轉 == 65500)
            {
                //minasA6.S_Stop(deviceID);
                this.MyTimer_輸送帶反轉_結束延遲.TickStop();
                this.MyTimer_輸送帶反轉_結束延遲.StartTickTime(10000);
                this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶正轉, false);
                this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶反轉, false);
                PLC_Device_輸送帶反轉.Bool = false;
                cnt_Program_輸送帶反轉 = 65535;
            }
        }
        void cnt_Program_輸送帶反轉_檢查按下(ref int cnt)
        {
            if (PLC_Device_輸送帶反轉.Bool) cnt++;
        }
        void cnt_Program_輸送帶反轉_檢查放開(ref int cnt)
        {
            if (!PLC_Device_輸送帶反轉.Bool) cnt = 65500;
        }
        void cnt_Program_輸送帶反轉_初始化(ref int cnt)
        {
            if (PLC_Device_Z軸Ready.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_輸送帶反轉_開始移動(ref int cnt)
        {
            this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶正轉, false);
            this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶反轉, true);

            MyTimerBasic_輸送帶反轉_檢查延遲.TickStop();
            MyTimerBasic_輸送帶反轉_檢查延遲.StartTickTime(PLC_Device_輸送帶反轉時間.Value);
            cnt++;
        }
        void cnt_Program_輸送帶反轉_等待移動完成(ref int cnt)
        {
            if (MyTimerBasic_輸送帶反轉_檢查延遲.IsTimeOut())
            {
                cnt++;
            }
        }






        #endregion
        #region PLC_輸送帶前進
      
        MyTimerBasic MyTimerBasic_輸送帶前進_檢查延遲 = new MyTimerBasic();
        Task Task_輸送帶前進;
        MyTimer MyTimer_輸送帶前進_結束延遲 = new MyTimer();
        int cnt_Program_輸送帶前進 = 65534;
        void sub_Program_輸送帶前進()
        {
            if (cnt_Program_輸送帶前進 == 65534)
            {
                this.MyTimer_輸送帶前進_結束延遲.StartTickTime(10000);
                PLC_Device_輸送帶前進.SetComment("PLC_輸送帶前進");
                PLC_Device_輸送帶前進.Bool = false;
                cnt_Program_輸送帶前進 = 65535;
            }
            if (cnt_Program_輸送帶前進 == 65535) cnt_Program_輸送帶前進 = 1;
            if (cnt_Program_輸送帶前進 == 1) cnt_Program_輸送帶前進_檢查按下(ref cnt_Program_輸送帶前進);
            if (cnt_Program_輸送帶前進 == 2) cnt_Program_輸送帶前進_初始化(ref cnt_Program_輸送帶前進);
            if (cnt_Program_輸送帶前進 == 3) cnt_Program_輸送帶前進_開始移動(ref cnt_Program_輸送帶前進);
            if (cnt_Program_輸送帶前進 == 4) cnt_Program_輸送帶前進_等待移動完成(ref cnt_Program_輸送帶前進);
            if (cnt_Program_輸送帶前進 == 5) cnt_Program_輸送帶前進 = 65500;
            if (cnt_Program_輸送帶前進 > 1) cnt_Program_輸送帶前進_檢查放開(ref cnt_Program_輸送帶前進);

            if (cnt_Program_輸送帶前進 == 65500)
            {
                //minasA6.S_Stop(deviceID);
                this.MyTimer_輸送帶前進_結束延遲.TickStop();
                this.MyTimer_輸送帶前進_結束延遲.StartTickTime(10000);
                this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶前進, false);
                this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶後退, false);
                PLC_Device_輸送帶前進.Bool = false;
                cnt_Program_輸送帶前進 = 65535;
            }
        }
        void cnt_Program_輸送帶前進_檢查按下(ref int cnt)
        {
            if (PLC_Device_輸送帶前進.Bool) cnt++;
        }
        void cnt_Program_輸送帶前進_檢查放開(ref int cnt)
        {
            if (!PLC_Device_輸送帶前進.Bool) cnt = 65500;
        }
        void cnt_Program_輸送帶前進_初始化(ref int cnt)
        {
            if (PLC_Device_Z軸Ready.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_輸送帶前進_開始移動(ref int cnt)
        {
            if(PLC_Device_輸送帶前進時間.Value == 0)
            {
                cnt++;
                return;
            }
            this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶前進, true);
            this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶後退, false);
            MyTimerBasic_輸送帶前進_檢查延遲.TickStop();
            MyTimerBasic_輸送帶前進_檢查延遲.StartTickTime(PLC_Device_輸送帶前進時間.Value);
            cnt++;
        }
        void cnt_Program_輸送帶前進_等待移動完成(ref int cnt)
        {
            if (PLC_Device_輸送帶前進時間.Value == 0)
            {
                cnt++;
                return;
            }
            if (MyTimerBasic_輸送帶前進_檢查延遲.IsTimeOut())
            {
                flag_輸送帶在後方 = false;
                cnt++;
            }
        }






        #endregion
        #region PLC_輸送帶後退

        MyTimerBasic MyTimerBasic_輸送帶後退_檢查延遲 = new MyTimerBasic();
        Task Task_輸送帶後退;
        MyTimer MyTimer_輸送帶後退_結束延遲 = new MyTimer();
        int cnt_Program_輸送帶後退 = 65534;
        void sub_Program_輸送帶後退()
        {
            if (cnt_Program_輸送帶後退 == 65534)
            {
                this.MyTimer_輸送帶後退_結束延遲.StartTickTime(10000);
                PLC_Device_輸送帶後退.SetComment("PLC_輸送帶後退");
                PLC_Device_輸送帶後退.Bool = false;
                cnt_Program_輸送帶後退 = 65535;
            }
            if (cnt_Program_輸送帶後退 == 65535) cnt_Program_輸送帶後退 = 1;
            if (cnt_Program_輸送帶後退 == 1) cnt_Program_輸送帶後退_檢查按下(ref cnt_Program_輸送帶後退);
            if (cnt_Program_輸送帶後退 == 2) cnt_Program_輸送帶後退_初始化(ref cnt_Program_輸送帶後退);
            if (cnt_Program_輸送帶後退 == 3) cnt_Program_輸送帶後退_開始移動(ref cnt_Program_輸送帶後退);
            if (cnt_Program_輸送帶後退 == 4) cnt_Program_輸送帶後退_等待移動完成(ref cnt_Program_輸送帶後退);
            if (cnt_Program_輸送帶後退 == 5) cnt_Program_輸送帶後退 = 65500;
            if (cnt_Program_輸送帶後退 > 1) cnt_Program_輸送帶後退_檢查放開(ref cnt_Program_輸送帶後退);

            if (cnt_Program_輸送帶後退 == 65500)
            {
                //minasA6.S_Stop(deviceID);
                this.MyTimer_輸送帶後退_結束延遲.TickStop();
                this.MyTimer_輸送帶後退_結束延遲.StartTickTime(10000);

                this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶前進, false);
                this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶後退, false);

                PLC_Device_輸送帶後退.Bool = false;
                cnt_Program_輸送帶後退 = 65535;
            }
        }
        void cnt_Program_輸送帶後退_檢查按下(ref int cnt)
        {
            if (PLC_Device_輸送帶後退.Bool) cnt++;
        }
        void cnt_Program_輸送帶後退_檢查放開(ref int cnt)
        {
            if (!PLC_Device_輸送帶後退.Bool) cnt = 65500;
        }
        void cnt_Program_輸送帶後退_初始化(ref int cnt)
        {
            if (PLC_Device_Z軸Ready.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_輸送帶後退_開始移動(ref int cnt)
        {
            this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶前進, false);
            this.rfiD_UI.Set_OutputPIN(myConfigClass.Board_IP, 29010, (int)enunm_InOutBoard.輸送帶後退, true);
            MyTimerBasic_輸送帶後退_檢查延遲.TickStop();
            MyTimerBasic_輸送帶後退_檢查延遲.StartTickTime(PLC_Device_輸送帶後退時間.Value);
            cnt++;
        }
        void cnt_Program_輸送帶後退_等待移動完成(ref int cnt)
        {
            if (MyTimerBasic_輸送帶後退_檢查延遲.IsTimeOut())
            {
                flag_輸送帶在後方 = true;
                cnt++;
            }
        }






        #endregion

        private void PlC_RJ_Button_Z軸下降_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_servoJogNeg = true;
        }
        private void PlC_RJ_Button_Z軸上升_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_servoJogPos = true;
        }
        private void PlC_RJ_Button_Z軸停止_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_servoStop = true;
        }
        private void PlC_RJ_Button_Z軸復歸_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_servoHome = true;
        }
        private void PlC_RJ_Button_Z軸激磁_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_servoOn = true;
        }
        private void PlC_RJ_Button_Z軸Alarm_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_servoClearAlarm = true;
        }


    }
}
