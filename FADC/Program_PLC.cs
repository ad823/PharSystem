using Basic;
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
    public partial class MainForm : Form
    {
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


        public static PLC_Device PLC_Device_Z軸馬達歸零 = new PLC_Device("S100");
        public static PLC_Device PLC_Device_移動到第一層位置 = new PLC_Device("S1000");
        public static PLC_Device PLC_Device_移動到第二層位置 = new PLC_Device("S1001");
        public static PLC_Device PLC_Device_移動到第三層位置 = new PLC_Device("S1002");
        public static PLC_Device PLC_Device_移動到第四層位置 = new PLC_Device("S1003");
        public static PLC_Device PLC_Device_移動到第五層位置 = new PLC_Device("S1004");
        public static PLC_Device PLC_Device_移動到頂層位置 = new PLC_Device("S1010");

        public static PLC_Device PLC_Device_Z軸馬達激磁 = new PLC_Device("Y10");
        public static PLC_Device PLC_Device_Z軸馬達激磁狀態 = new PLC_Device("Y11");
        public static PLC_Device PLC_Device_Z軸馬達Alarm狀態 = new PLC_Device("Y11");
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
            plC_RJ_Button_Z軸回零.MouseDownEvent += PlC_RJ_Button_Z軸回零_MouseDownEvent;
            plC_RJ_Button_Z軸Alarm.MouseDownEvent += PlC_RJ_Button_Z軸Alarm_MouseDownEvent;

            plC_RJ_Button1_Z軸停止.MouseDownEvent += PlC_RJ_Button1_Z軸停止_MouseDownEvent;
            plC_RJ_Button_Z軸上升.MouseDownEvent += PlC_RJ_Button_Z軸上升_MouseDownEvent;
            plC_RJ_Button_Z軸下降.MouseDownEvent += PlC_RJ_Button_Z軸下降_MouseDownEvent;

            this.plC_UI_Init.Add_Method(sub_Program_PLC);
        }

 
        public void sub_Program_PLC()
        {
            if (flag_minasA6_isOpen == true)
            {
                try
                {

                }
                catch(Exception ex)
                {
                    Logger.Log("Z-erroe", $"Exception : {ex.Message}");
                }
                var servo = minasA6.GetServoStatus(deviceID);
                var ready = !minasA6.IsBusy(deviceID);
                var alarm = minasA6.GetServoAlarmStatus(deviceID);

                var limit = minasA6.GetLimitStatus(deviceID);

                int pos = minasA6.GetPosition(deviceID);

                PLC_Device_Z軸馬達位置.Value = pos;
                PLC_Device_Z軸馬達激磁.Bool = servo;
                PLC_Device_Z軸馬達激磁狀態.Bool = limit.Home;
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
                if(flag_servoHome)
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
                   
                
                if(flag_servoJogPos)
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

                sub_Program_Z軸絕對位置移動();
                sub_Program_Z軸移動到第一層();
                sub_Program_Z軸移動到第二層();
                sub_Program_Z軸移動到第三層();
                sub_Program_Z軸移動到第四層();
                sub_Program_Z軸移動到第五層();
                sub_Program_Z軸移動到頂層();
            }
        }


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
                minasA6.S_Stop(deviceID);
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
            cnt++;
        }
        void cnt_Program_Z軸絕對位置移動_等待移動完成(ref int cnt)
        {
            if(MyTimerBasic_Z軸絕對位置移動_檢查延遲.IsTimeOut())
            {
                if(PLC_Device_Z軸Ready.Bool)
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
        private void PlC_RJ_Button_Z軸下降_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_servoJogNeg = true;
        }

        private void PlC_RJ_Button_Z軸上升_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_servoJogPos = true;
        }

        private void PlC_RJ_Button1_Z軸停止_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_servoStop = true;
        }

        private void PlC_RJ_Button_Z軸回零_MouseDownEvent(MouseEventArgs mevent)
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
