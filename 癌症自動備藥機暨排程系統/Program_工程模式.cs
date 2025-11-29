using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MyUI;
using Basic;
using SQLUI;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

using System.Reflection;
using System.Runtime.InteropServices;
using MyPrinterlib;
using MyOffice;
using HIS_DB_Lib;

namespace 癌症備藥機
{
    public partial class Main_Form : Form
    {

        PLC_Device PLC_IO_冷藏區_輸送帶前進 = new PLC_Device("Y11");
        PLC_Device PLC_IO_冷藏區_輸送台進退終點 = new PLC_Device("X200");

        PLC_Device PLC_IO_冷藏區_輸送帶後退 = new PLC_Device("Y10");
        PLC_Device PLC_IO_冷藏區_輸送台進退原點 = new PLC_Device("X03");

        PLC_Device PLC_IO_冷藏區_輸送門開啟 = new PLC_Device("S241");
        PLC_Device PLC_IO_冷藏區_輸送台開啟到位Sensor = new PLC_Device("X05");
        PLC_Device PLC_IO_冷藏區_輸送台開啟到位 = new PLC_Device("S1500");
        int 冷藏區_輸送台開啟到位延遲毫秒 = 3000;
        PLC_Device PLC_IO_冷藏區_輸送門關閉 = new PLC_Device("S240");
        PLC_Device PLC_IO_冷藏區_輸送台關閉到位 = new PLC_Device("X04");

        PLC_Device PLC_IO_冷藏區_輸送帶啟動 = new PLC_Device("Y2");
        PLC_Device PLC_IO_冷藏區_輸送帶反轉 = new PLC_Device("Y3");

        MyTimer MyTimer_冷藏區_藥盒中感應延遲 = new MyTimer("R12101");
        PLC_Device PLC_IO_冷藏區_藥盒中感應_Input = new PLC_Device("X01");
        PLC_Device PLC_IO_冷藏區_藥盒中感應 = new PLC_Device("S12101");

        PLC_Device PLC_IO_冷藏區_藥盒左感應 = new PLC_Device("X02");
        PLC_Device PLC_IO_冷藏區_藥盒右感應 = new PLC_Device("X00");


        PLC_Device PLC_IO_常溫區_輸送帶前進 = new PLC_Device("Y07");
        PLC_Device PLC_IO_常溫區_輸送台進退終點 = new PLC_Device("X201");

        PLC_Device PLC_IO_常溫區_輸送帶後退 = new PLC_Device("Y06");
        PLC_Device PLC_IO_常溫區_輸送台進退原點 = new PLC_Device("X13");

        PLC_Device PLC_IO_常溫區_輸送門開啟 = new PLC_Device("S243");
        PLC_Device PLC_IO_常溫區_輸送台開啟到位Sensor = new PLC_Device("X15");
        PLC_Device PLC_IO_常溫區_輸送台開啟到位 = new PLC_Device("S1501");
        int 常溫區_輸送台開啟到位延遲毫秒 = 3000;

        PLC_Device PLC_IO_常溫區_輸送門關閉 = new PLC_Device("S242");
        PLC_Device PLC_IO_常溫區_輸送台關閉到位 = new PLC_Device("X14");

        PLC_Device PLC_IO_常溫區_輸送帶啟動 = new PLC_Device("Y0");
        PLC_Device PLC_IO_常溫區_輸送帶反轉 = new PLC_Device("Y1");

        MyTimer MyTimer_常溫區_藥盒中感應延遲 = new MyTimer("R12111");
        PLC_Device PLC_IO_常溫區_藥盒中感應_Input = new PLC_Device("X11");
        PLC_Device PLC_IO_常溫區_藥盒中感應 = new PLC_Device("S12111");

        PLC_Device PLC_IO_常溫區_藥盒左感應 = new PLC_Device("X10");
        PLC_Device PLC_IO_常溫區_藥盒右感應 = new PLC_Device("X12");

        PLC_Device PLC_IO_進出盒區_輸送帶啟動 = new PLC_Device("Y4");
        PLC_Device PLC_IO_進出盒區_輸送帶反轉 = new PLC_Device("Y5");

        PLC_Device PLC_IO_進出盒區_藥盒到位感應 = new PLC_Device("S1520");
        PLC_Device PLC_IO_進出盒區_藥盒到位感應Sensor = new PLC_Device("X06");


        private void Program_工程模式_Init()
        {
            plC_UI_Init.Add_Method(Program_工程模式);
        }
        private void Program_工程模式()
        {
            if(PLC_IO_冷藏區_藥盒中感應_Input.Bool)
            {
                if(MyTimer_冷藏區_藥盒中感應延遲.IsTimeOut())
                {
                    PLC_IO_冷藏區_藥盒中感應.Bool = true;
                }
            }
            else
            {
                PLC_IO_冷藏區_藥盒中感應.Bool = false;
                MyTimer_冷藏區_藥盒中感應延遲.TickStop();
                MyTimer_冷藏區_藥盒中感應延遲.StartTickTime();
            }

            if (PLC_IO_常溫區_藥盒中感應_Input.Bool)
            {
                if (MyTimer_常溫區_藥盒中感應延遲.IsTimeOut())
                {
                    PLC_IO_常溫區_藥盒中感應.Bool = true;
                }
            }
            else
            {
                PLC_IO_常溫區_藥盒中感應.Bool = false;
                MyTimer_常溫區_藥盒中感應延遲.TickStop();
                MyTimer_常溫區_藥盒中感應延遲.StartTickTime();
            }

            if (myConfigClass.主機模式 == false) return;
            sub_Program_冷藏區藥盒輸送至左方();
            sub_Program_冷藏區藥盒輸送至右方();
            sub_Program_冷藏區藥盒輸送至中間();

            sub_Program_常溫區藥盒輸送至左方();
            sub_Program_常溫區藥盒輸送至右方();
            sub_Program_常溫區藥盒輸送至中間();

            sub_Program_進出盒區藥盒輸送至左方();

            sub_Program_將藥盒從冷藏區傳接至常溫區();
            sub_Program_將藥盒從常溫區傳接至冷藏區();
            sub_Program_將藥盒從常溫區傳接至進出盒區();
            sub_Program_將藥盒從進出盒區傳接至常溫區();
            sub_Program_出料一次();
        }

        #region PLC_冷藏區藥盒輸送至左方
        PLC_Device PLC_Device_冷藏區藥盒輸送至左方 = new PLC_Device("S5100");
        PLC_Device PLC_Device_冷藏區藥盒輸送至左方_OK = new PLC_Device("");
        Task Task_冷藏區藥盒輸送至左方;
        MyTimer MyTimer_冷藏區藥盒輸送至左方_結束延遲 = new MyTimer();
        MyTimer MyTimer_冷藏區藥盒輸送至左方_開始延遲 = new MyTimer();
        int cnt_Program_冷藏區藥盒輸送至左方 = 65534;
        void sub_Program_冷藏區藥盒輸送至左方()
        {
            if (cnt_Program_冷藏區藥盒輸送至左方 == 65534)
            {
                this.MyTimer_冷藏區藥盒輸送至左方_結束延遲.StartTickTime(10000);
                this.MyTimer_冷藏區藥盒輸送至左方_開始延遲.StartTickTime(10000);
                PLC_Device_冷藏區藥盒輸送至左方.SetComment("PLC_冷藏區藥盒輸送至左方");
                PLC_Device_冷藏區藥盒輸送至左方_OK.SetComment("PLC_冷藏區藥盒輸送至左方_OK");
                PLC_Device_冷藏區藥盒輸送至左方.Bool = false;
                cnt_Program_冷藏區藥盒輸送至左方 = 65535;
            }
            if (cnt_Program_冷藏區藥盒輸送至左方 == 65535) cnt_Program_冷藏區藥盒輸送至左方 = 1;
            if (cnt_Program_冷藏區藥盒輸送至左方 == 1) cnt_Program_冷藏區藥盒輸送至左方_檢查按下(ref cnt_Program_冷藏區藥盒輸送至左方);
            if (cnt_Program_冷藏區藥盒輸送至左方 == 2) cnt_Program_冷藏區藥盒輸送至左方_初始化(ref cnt_Program_冷藏區藥盒輸送至左方);
            if (cnt_Program_冷藏區藥盒輸送至左方 == 3) cnt_Program_冷藏區藥盒輸送至左方_輸送帶啟動(ref cnt_Program_冷藏區藥盒輸送至左方);
            if (cnt_Program_冷藏區藥盒輸送至左方 == 4) cnt_Program_冷藏區藥盒輸送至左方 = 65500;
            if (cnt_Program_冷藏區藥盒輸送至左方 > 1) cnt_Program_冷藏區藥盒輸送至左方_檢查放開(ref cnt_Program_冷藏區藥盒輸送至左方);

            if (cnt_Program_冷藏區藥盒輸送至左方 == 65500)
            {
                this.MyTimer_冷藏區藥盒輸送至左方_結束延遲.TickStop();
                this.MyTimer_冷藏區藥盒輸送至左方_結束延遲.StartTickTime(10000);

                PLC_IO_冷藏區_輸送帶啟動.Bool = false;
                PLC_IO_冷藏區_輸送帶反轉.Bool = false;

                PLC_Device_冷藏區藥盒輸送至左方.Bool = false;
                PLC_Device_冷藏區藥盒輸送至左方_OK.Bool = false;

                cnt_Program_冷藏區藥盒輸送至左方 = 65535;
            }
        }
        void cnt_Program_冷藏區藥盒輸送至左方_檢查按下(ref int cnt)
        {
            if (PLC_Device_冷藏區藥盒輸送至左方.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_冷藏區藥盒輸送至左方_檢查放開(ref int cnt)
        {
            if (!PLC_Device_冷藏區藥盒輸送至左方.Bool) cnt = 65500;
        }
        void cnt_Program_冷藏區藥盒輸送至左方_初始化(ref int cnt)
        {
            PLC_IO_冷藏區_輸送帶啟動.Bool = false;
            PLC_IO_冷藏區_輸送帶反轉.Bool = false;

            cnt++;
        }
        void cnt_Program_冷藏區藥盒輸送至左方_輸送帶啟動(ref int cnt)
        {
            if (PLC_IO_冷藏區_藥盒左感應.Bool)
            {
                cnt++;
                return;
            }
            PLC_IO_冷藏區_輸送帶反轉.Bool = true;
            PLC_IO_冷藏區_輸送帶啟動.Bool = true;
        }
        #endregion
        #region PLC_冷藏區藥盒輸送至右方
        PLC_Device PLC_Device_冷藏區藥盒輸送至右方 = new PLC_Device("S5102");
        PLC_Device PLC_Device_冷藏區藥盒輸送至右方_OK = new PLC_Device("");
        Task Task_冷藏區藥盒輸送至右方;
        MyTimer MyTimer_冷藏區藥盒輸送至右方_結束延遲 = new MyTimer();
        MyTimer MyTimer_冷藏區藥盒輸送至右方_開始延遲 = new MyTimer();
        int cnt_Program_冷藏區藥盒輸送至右方 = 65534;
        void sub_Program_冷藏區藥盒輸送至右方()
        {
            if (cnt_Program_冷藏區藥盒輸送至右方 == 65534)
            {
                this.MyTimer_冷藏區藥盒輸送至右方_結束延遲.StartTickTime(10000);
                this.MyTimer_冷藏區藥盒輸送至右方_開始延遲.StartTickTime(10000);
                PLC_Device_冷藏區藥盒輸送至右方.SetComment("PLC_冷藏區藥盒輸送至右方");
                PLC_Device_冷藏區藥盒輸送至右方_OK.SetComment("PLC_冷藏區藥盒輸送至右方_OK");
                PLC_Device_冷藏區藥盒輸送至右方.Bool = false;
                cnt_Program_冷藏區藥盒輸送至右方 = 65535;
            }
            if (cnt_Program_冷藏區藥盒輸送至右方 == 65535) cnt_Program_冷藏區藥盒輸送至右方 = 1;
            if (cnt_Program_冷藏區藥盒輸送至右方 == 1) cnt_Program_冷藏區藥盒輸送至右方_檢查按下(ref cnt_Program_冷藏區藥盒輸送至右方);
            if (cnt_Program_冷藏區藥盒輸送至右方 == 2) cnt_Program_冷藏區藥盒輸送至右方_初始化(ref cnt_Program_冷藏區藥盒輸送至右方);
            if (cnt_Program_冷藏區藥盒輸送至右方 == 3) cnt_Program_冷藏區藥盒輸送至右方_輸送帶啟動(ref cnt_Program_冷藏區藥盒輸送至右方);
            if (cnt_Program_冷藏區藥盒輸送至右方 == 4) cnt_Program_冷藏區藥盒輸送至右方 = 65500;
            if (cnt_Program_冷藏區藥盒輸送至右方 > 1) cnt_Program_冷藏區藥盒輸送至右方_檢查放開(ref cnt_Program_冷藏區藥盒輸送至右方);

            if (cnt_Program_冷藏區藥盒輸送至右方 == 65500)
            {
                this.MyTimer_冷藏區藥盒輸送至右方_結束延遲.TickStop();
                this.MyTimer_冷藏區藥盒輸送至右方_結束延遲.StartTickTime(10000);

                PLC_IO_冷藏區_輸送帶啟動.Bool = false;
                PLC_IO_冷藏區_輸送帶反轉.Bool = false;

                PLC_Device_冷藏區藥盒輸送至右方.Bool = false;
                PLC_Device_冷藏區藥盒輸送至右方_OK.Bool = false;

                cnt_Program_冷藏區藥盒輸送至右方 = 65535;
            }
        }
        void cnt_Program_冷藏區藥盒輸送至右方_檢查按下(ref int cnt)
        {
            if (PLC_Device_冷藏區藥盒輸送至右方.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_冷藏區藥盒輸送至右方_檢查放開(ref int cnt)
        {
            if (!PLC_Device_冷藏區藥盒輸送至右方.Bool) cnt = 65500;
        }
        void cnt_Program_冷藏區藥盒輸送至右方_初始化(ref int cnt)
        {
            PLC_IO_冷藏區_輸送帶啟動.Bool = false;
            PLC_IO_冷藏區_輸送帶反轉.Bool = false;

            cnt++;
        }
        void cnt_Program_冷藏區藥盒輸送至右方_輸送帶啟動(ref int cnt)
        {
            if (PLC_IO_冷藏區_藥盒右感應.Bool)
            {
                cnt++;
                return;
            }
            PLC_IO_冷藏區_輸送帶反轉.Bool = false;
            PLC_IO_冷藏區_輸送帶啟動.Bool = true;
        }
        #endregion
        #region PLC_冷藏區藥盒輸送至中間
        PLC_Device PLC_Device_冷藏區藥盒輸送至中間 = new PLC_Device("S5101");
        PLC_Device PLC_Device_冷藏區藥盒輸送至中間_OK = new PLC_Device("");
        Task Task_冷藏區藥盒輸送至中間;
        MyTimer MyTimer_冷藏區藥盒輸送至中間_結束延遲 = new MyTimer();
        MyTimer MyTimer_冷藏區藥盒輸送至中間_開始延遲 = new MyTimer();
        int cnt_Program_冷藏區藥盒輸送至中間 = 65534;
        void sub_Program_冷藏區藥盒輸送至中間()
        {
            if (cnt_Program_冷藏區藥盒輸送至中間 == 65534)
            {
                this.MyTimer_冷藏區藥盒輸送至中間_結束延遲.StartTickTime(10000);
                this.MyTimer_冷藏區藥盒輸送至中間_開始延遲.StartTickTime(10000);
                PLC_Device_冷藏區藥盒輸送至中間.SetComment("PLC_冷藏區藥盒輸送至中間");
                PLC_Device_冷藏區藥盒輸送至中間_OK.SetComment("PLC_冷藏區藥盒輸送至中間_OK");
                PLC_Device_冷藏區藥盒輸送至中間.Bool = false;
                cnt_Program_冷藏區藥盒輸送至中間 = 65535;
            }
            if (cnt_Program_冷藏區藥盒輸送至中間 == 65535) cnt_Program_冷藏區藥盒輸送至中間 = 1;
            if (cnt_Program_冷藏區藥盒輸送至中間 == 1) cnt_Program_冷藏區藥盒輸送至中間_檢查按下(ref cnt_Program_冷藏區藥盒輸送至中間);
            if (cnt_Program_冷藏區藥盒輸送至中間 == 2) cnt_Program_冷藏區藥盒輸送至中間_初始化(ref cnt_Program_冷藏區藥盒輸送至中間);
            if (cnt_Program_冷藏區藥盒輸送至中間 == 3) cnt_Program_冷藏區藥盒輸送至中間_輸送至右方(ref cnt_Program_冷藏區藥盒輸送至中間);
            if (cnt_Program_冷藏區藥盒輸送至中間 == 4) cnt_Program_冷藏區藥盒輸送至中間_等待輸送至右方完成(ref cnt_Program_冷藏區藥盒輸送至中間);
            if (cnt_Program_冷藏區藥盒輸送至中間 == 5) cnt_Program_冷藏區藥盒輸送至中間_輸送帶啟動(ref cnt_Program_冷藏區藥盒輸送至中間);
            if (cnt_Program_冷藏區藥盒輸送至中間 == 6) cnt_Program_冷藏區藥盒輸送至中間 = 65500;
            if (cnt_Program_冷藏區藥盒輸送至中間 > 1) cnt_Program_冷藏區藥盒輸送至中間_檢查放開(ref cnt_Program_冷藏區藥盒輸送至中間);

            if (cnt_Program_冷藏區藥盒輸送至中間 == 65500)
            {
                this.MyTimer_冷藏區藥盒輸送至中間_結束延遲.TickStop();
                this.MyTimer_冷藏區藥盒輸送至中間_結束延遲.StartTickTime(10000);

                PLC_IO_冷藏區_輸送帶啟動.Bool = false;
                PLC_IO_冷藏區_輸送帶反轉.Bool = false;

                PLC_Device_冷藏區藥盒輸送至右方.Bool = false;
                PLC_Device_冷藏區藥盒輸送至中間.Bool = false;
                PLC_Device_冷藏區藥盒輸送至中間_OK.Bool = false;

                cnt_Program_冷藏區藥盒輸送至中間 = 65535;
            }
        }
        void cnt_Program_冷藏區藥盒輸送至中間_檢查按下(ref int cnt)
        {
            if (PLC_Device_冷藏區藥盒輸送至中間.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_冷藏區藥盒輸送至中間_檢查放開(ref int cnt)
        {
            if (!PLC_Device_冷藏區藥盒輸送至中間.Bool) cnt = 65500;
        }
        void cnt_Program_冷藏區藥盒輸送至中間_初始化(ref int cnt)
        {
            PLC_IO_冷藏區_輸送帶啟動.Bool = false;
            PLC_IO_冷藏區_輸送帶反轉.Bool = false;

            cnt++;
        }
        void cnt_Program_冷藏區藥盒輸送至中間_輸送至右方(ref int cnt)
        {
            if (PLC_Device_冷藏區藥盒輸送至右方.Bool) return;
            PLC_Device_冷藏區藥盒輸送至右方.Bool = true;
            cnt++;
        }
        void cnt_Program_冷藏區藥盒輸送至中間_等待輸送至右方完成(ref int cnt)
        {
            if (PLC_Device_冷藏區藥盒輸送至右方.Bool) return;
            cnt++;
        }
        void cnt_Program_冷藏區藥盒輸送至中間_輸送帶啟動(ref int cnt)
        {
            if (PLC_IO_冷藏區_藥盒中感應.Bool)
            {
                cnt++;
                return;
            }
            PLC_IO_冷藏區_輸送帶反轉.Bool = true;
            PLC_IO_冷藏區_輸送帶啟動.Bool = true;
        }
        #endregion
        #region PLC_冷藏區輸送帶前進
        PLC_Device PLC_Device_冷藏區輸送帶前進 = new PLC_Device("S5050");
        PLC_Device PLC_Device_冷藏區輸送帶前進_OK = new PLC_Device("");
        Task Task_冷藏區輸送帶前進;
        MyTimer MyTimer_冷藏區輸送帶前進_結束延遲 = new MyTimer();
        MyTimer MyTimer_冷藏區輸送帶前進_開始延遲 = new MyTimer();
        int cnt_Program_冷藏區輸送帶前進 = 65534;
        void sub_Program_冷藏區輸送帶前進()
        {
            if (cnt_Program_冷藏區輸送帶前進 == 65534)
            {
                this.MyTimer_冷藏區輸送帶前進_結束延遲.StartTickTime(10000);
                this.MyTimer_冷藏區輸送帶前進_開始延遲.StartTickTime(10000);
                PLC_Device_冷藏區輸送帶前進.SetComment("PLC_冷藏區輸送帶前進");
                PLC_Device_冷藏區輸送帶前進_OK.SetComment("PLC_冷藏區輸送帶前進_OK");
                PLC_Device_冷藏區輸送帶前進.Bool = false;
                cnt_Program_冷藏區輸送帶前進 = 65535;
            }
            if (cnt_Program_冷藏區輸送帶前進 == 65535) cnt_Program_冷藏區輸送帶前進 = 1;
            if (cnt_Program_冷藏區輸送帶前進 == 1) cnt_Program_冷藏區輸送帶前進_檢查按下(ref cnt_Program_冷藏區輸送帶前進);
            if (cnt_Program_冷藏區輸送帶前進 == 2) cnt_Program_冷藏區輸送帶前進_初始化(ref cnt_Program_冷藏區輸送帶前進);
            if (cnt_Program_冷藏區輸送帶前進 == 3) cnt_Program_冷藏區輸送帶前進_輸送帶前進(ref cnt_Program_冷藏區輸送帶前進);
            if (cnt_Program_冷藏區輸送帶前進 == 4) cnt_Program_冷藏區輸送帶前進 = 65500;
            if (cnt_Program_冷藏區輸送帶前進 > 1) cnt_Program_冷藏區輸送帶前進_檢查放開(ref cnt_Program_冷藏區輸送帶前進);

            if (cnt_Program_冷藏區輸送帶前進 == 65500)
            {
                this.MyTimer_冷藏區輸送帶前進_結束延遲.TickStop();
                this.MyTimer_冷藏區輸送帶前進_結束延遲.StartTickTime(10000);
                PLC_Device_冷藏區輸送帶前進.Bool = false;
                PLC_IO_冷藏區_輸送帶前進.Bool = false;
                cnt_Program_冷藏區輸送帶前進 = 65535;
            }
        }
        void cnt_Program_冷藏區輸送帶前進_檢查按下(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送帶前進.Bool) cnt++;
        }
        void cnt_Program_冷藏區輸送帶前進_檢查放開(ref int cnt)
        {
            if (!PLC_Device_冷藏區輸送帶前進.Bool) cnt = 65500;
        }
        void cnt_Program_冷藏區輸送帶前進_初始化(ref int cnt)
        {
            PLC_IO_冷藏區_輸送帶後退.Bool = false;
            PLC_Device_冷藏區輸送帶後退.Bool = false;
            cnt++;
        }
        void cnt_Program_冷藏區輸送帶前進_輸送帶前進(ref int cnt)
        {
            PLC_IO_冷藏區_輸送帶前進.Bool = true;
            if (PLC_IO_冷藏區_輸送台進退終點.Bool)
            {
                cnt++;
            }

        }






        #endregion
        #region PLC_冷藏區輸送帶後退
        PLC_Device PLC_Device_冷藏區輸送帶後退 = new PLC_Device("S5051");
        PLC_Device PLC_Device_冷藏區輸送帶後退_OK = new PLC_Device("");
        Task Task_冷藏區輸送帶後退;
        MyTimer MyTimer_冷藏區輸送帶後退_結束延遲 = new MyTimer();
        MyTimer MyTimer_冷藏區輸送帶後退_開始延遲 = new MyTimer();
        int cnt_Program_冷藏區輸送帶後退 = 65534;
        void sub_Program_冷藏區輸送帶後退()
        {
            PLC_Device_冷藏區輸送帶後退.Bool = plC_RJ_Button_冷藏區_輸送帶後退.Bool;
            if (cnt_Program_冷藏區輸送帶後退 == 65534)
            {
                this.MyTimer_冷藏區輸送帶後退_結束延遲.StartTickTime(10000);
                this.MyTimer_冷藏區輸送帶後退_開始延遲.StartTickTime(10000);
                PLC_Device_冷藏區輸送帶後退.SetComment("PLC_冷藏區輸送帶後退");
                PLC_Device_冷藏區輸送帶後退_OK.SetComment("PLC_冷藏區輸送帶後退_OK");
                PLC_Device_冷藏區輸送帶後退.Bool = false;
                cnt_Program_冷藏區輸送帶後退 = 65535;
            }
            if (cnt_Program_冷藏區輸送帶後退 == 65535) cnt_Program_冷藏區輸送帶後退 = 1;
            if (cnt_Program_冷藏區輸送帶後退 == 1) cnt_Program_冷藏區輸送帶後退_檢查按下(ref cnt_Program_冷藏區輸送帶後退);
            if (cnt_Program_冷藏區輸送帶後退 == 2) cnt_Program_冷藏區輸送帶後退_初始化(ref cnt_Program_冷藏區輸送帶後退);
            if (cnt_Program_冷藏區輸送帶後退 == 3) cnt_Program_冷藏區輸送帶後退_輸送帶後退(ref cnt_Program_冷藏區輸送帶後退);
            if (cnt_Program_冷藏區輸送帶後退 == 4) cnt_Program_冷藏區輸送帶後退 = 65500;
            if (cnt_Program_冷藏區輸送帶後退 > 1) cnt_Program_冷藏區輸送帶後退_檢查放開(ref cnt_Program_冷藏區輸送帶後退);

            if (cnt_Program_冷藏區輸送帶後退 == 65500)
            {
                this.MyTimer_冷藏區輸送帶後退_結束延遲.TickStop();
                this.MyTimer_冷藏區輸送帶後退_結束延遲.StartTickTime(10000);
                PLC_Device_冷藏區輸送帶後退.Bool = false;
                PLC_IO_冷藏區_輸送帶後退.Bool = false;
                cnt_Program_冷藏區輸送帶後退 = 65535;
            }
        }
        void cnt_Program_冷藏區輸送帶後退_檢查按下(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送帶後退.Bool) cnt++;
        }
        void cnt_Program_冷藏區輸送帶後退_檢查放開(ref int cnt)
        {
            if (!PLC_Device_冷藏區輸送帶後退.Bool) cnt = 65500;
        }
        void cnt_Program_冷藏區輸送帶後退_初始化(ref int cnt)
        {
            PLC_IO_冷藏區_輸送帶前進.Bool = false;
            PLC_Device_冷藏區輸送帶前進.Bool = false;
            cnt++;
        }
        void cnt_Program_冷藏區輸送帶後退_輸送帶後退(ref int cnt)
        {
            PLC_IO_冷藏區_輸送帶後退.Bool = true;
            if (PLC_IO_冷藏區_輸送台進退原點.Bool)
            {
                cnt++;
            }

        }
        #endregion
        #region PLC_冷藏區輸送門開啟
        PLC_Device PLC_Device_冷藏區輸送門開啟 = new PLC_Device("S5052");
        PLC_Device PLC_Device_冷藏區輸送門開啟_OK = new PLC_Device("");
        Task Task_冷藏區輸送門開啟;
        MyTimer MyTimer_冷藏區輸送門開啟_到位延遲 = new MyTimer();
        MyTimer MyTimer_冷藏區輸送門開啟_結束延遲 = new MyTimer();
        MyTimer MyTimer_冷藏區輸送門開啟_開始延遲 = new MyTimer();
        int cnt_Program_冷藏區輸送門開啟 = 65534;
        void sub_Program_冷藏區輸送門開啟()
        {
            if(PLC_IO_冷藏區_輸送台開啟到位Sensor.Bool)
            {
                if(MyTimer_冷藏區輸送門開啟_到位延遲.IsTimeOut())
                {
                    PLC_IO_冷藏區_輸送台開啟到位.Bool = true;
                }
            }
            else
            {
                MyTimer_冷藏區輸送門開啟_到位延遲.TickStop();
                MyTimer_冷藏區輸送門開啟_到位延遲.StartTickTime(冷藏區_輸送台開啟到位延遲毫秒);
                PLC_IO_冷藏區_輸送台開啟到位.Bool = false;
            }
            PLC_Device_冷藏區輸送門開啟.Bool = plC_RJ_Button_冷藏區_輸送門開啟.Bool;
            if (cnt_Program_冷藏區輸送門開啟 == 65534)
            {
                MyTimer_冷藏區輸送門開啟_到位延遲.TickStop();
                MyTimer_冷藏區輸送門開啟_到位延遲.StartTickTime(冷藏區_輸送台開啟到位延遲毫秒);
                this.MyTimer_冷藏區輸送門開啟_結束延遲.StartTickTime(10000);
                this.MyTimer_冷藏區輸送門開啟_開始延遲.StartTickTime(10000);
                PLC_Device_冷藏區輸送門開啟.SetComment("PLC_冷藏區輸送門開啟");
                PLC_Device_冷藏區輸送門開啟_OK.SetComment("PLC_冷藏區輸送門開啟_OK");
                PLC_Device_冷藏區輸送門開啟.Bool = false;
                cnt_Program_冷藏區輸送門開啟 = 65535;
            }
            if (cnt_Program_冷藏區輸送門開啟 == 65535) cnt_Program_冷藏區輸送門開啟 = 1;
            if (cnt_Program_冷藏區輸送門開啟 == 1) cnt_Program_冷藏區輸送門開啟_檢查按下(ref cnt_Program_冷藏區輸送門開啟);
            if (cnt_Program_冷藏區輸送門開啟 == 2) cnt_Program_冷藏區輸送門開啟_初始化(ref cnt_Program_冷藏區輸送門開啟);
            if (cnt_Program_冷藏區輸送門開啟 == 3) cnt_Program_冷藏區輸送門開啟_輸送門開啟(ref cnt_Program_冷藏區輸送門開啟);
            if (cnt_Program_冷藏區輸送門開啟 == 4) cnt_Program_冷藏區輸送門開啟 = 65500;
            if (cnt_Program_冷藏區輸送門開啟 > 1) cnt_Program_冷藏區輸送門開啟_檢查放開(ref cnt_Program_冷藏區輸送門開啟);

            if (cnt_Program_冷藏區輸送門開啟 == 65500)
            {
                this.MyTimer_冷藏區輸送門開啟_結束延遲.TickStop();
                this.MyTimer_冷藏區輸送門開啟_結束延遲.StartTickTime(10000);
                PLC_Device_冷藏區輸送門開啟.Bool = false;
                PLC_IO_冷藏區_輸送門開啟.Bool = false;
                cnt_Program_冷藏區輸送門開啟 = 65535;
            }
        }
        void cnt_Program_冷藏區輸送門開啟_檢查按下(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門開啟.Bool) cnt++;
        }
        void cnt_Program_冷藏區輸送門開啟_檢查放開(ref int cnt)
        {
            if (!PLC_Device_冷藏區輸送門開啟.Bool) cnt = 65500;
        }
        void cnt_Program_冷藏區輸送門開啟_初始化(ref int cnt)
        {
            PLC_IO_冷藏區_輸送門關閉.Bool = false;
            PLC_Device_冷藏區輸送門關閉.Bool = false;
            cnt++;
        }
        void cnt_Program_冷藏區輸送門開啟_輸送門開啟(ref int cnt)
        {
            PLC_IO_冷藏區_輸送門開啟.Bool = true;
            if (PLC_IO_冷藏區_輸送台開啟到位.Bool)
            {
                cnt++;
            }

        }

        #endregion
        #region PLC_冷藏區輸送門關閉
        PLC_Device PLC_Device_冷藏區輸送門關閉 = new PLC_Device("S5053");
        PLC_Device PLC_Device_冷藏區輸送門關閉_OK = new PLC_Device("");
        Task Task_冷藏區輸送門關閉;
        MyTimer MyTimer_冷藏區輸送門關閉_結束延遲 = new MyTimer();
        MyTimer MyTimer_冷藏區輸送門關閉_開始延遲 = new MyTimer();
        int cnt_Program_冷藏區輸送門關閉 = 65534;
        void sub_Program_冷藏區輸送門關閉()
        {
            PLC_Device_冷藏區輸送門關閉.Bool = plC_RJ_Button_冷藏區_輸送門關閉.Bool;
            if (cnt_Program_冷藏區輸送門關閉 == 65534)
            {
                this.MyTimer_冷藏區輸送門關閉_結束延遲.StartTickTime(10000);
                this.MyTimer_冷藏區輸送門關閉_開始延遲.StartTickTime(10000);
                PLC_Device_冷藏區輸送門關閉.SetComment("PLC_冷藏區輸送門關閉");
                PLC_Device_冷藏區輸送門關閉_OK.SetComment("PLC_冷藏區輸送門關閉_OK");
                PLC_Device_冷藏區輸送門關閉.Bool = false;
                cnt_Program_冷藏區輸送門關閉 = 65535;
            }
            if (cnt_Program_冷藏區輸送門關閉 == 65535) cnt_Program_冷藏區輸送門關閉 = 1;
            if (cnt_Program_冷藏區輸送門關閉 == 1) cnt_Program_冷藏區輸送門關閉_檢查按下(ref cnt_Program_冷藏區輸送門關閉);
            if (cnt_Program_冷藏區輸送門關閉 == 2) cnt_Program_冷藏區輸送門關閉_初始化(ref cnt_Program_冷藏區輸送門關閉);
            if (cnt_Program_冷藏區輸送門關閉 == 3) cnt_Program_冷藏區輸送門關閉_輸送門關閉(ref cnt_Program_冷藏區輸送門關閉);
            if (cnt_Program_冷藏區輸送門關閉 == 4) cnt_Program_冷藏區輸送門關閉 = 65500;
            if (cnt_Program_冷藏區輸送門關閉 > 1) cnt_Program_冷藏區輸送門關閉_檢查放開(ref cnt_Program_冷藏區輸送門關閉);

            if (cnt_Program_冷藏區輸送門關閉 == 65500)
            {
                this.MyTimer_冷藏區輸送門關閉_結束延遲.TickStop();
                this.MyTimer_冷藏區輸送門關閉_結束延遲.StartTickTime(10000);
                PLC_Device_冷藏區輸送門關閉.Bool = false;
                PLC_IO_冷藏區_輸送門關閉.Bool = false;
                cnt_Program_冷藏區輸送門關閉 = 65535;
            }
        }
        void cnt_Program_冷藏區輸送門關閉_檢查按下(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門關閉.Bool) cnt++;
        }
        void cnt_Program_冷藏區輸送門關閉_檢查放開(ref int cnt)
        {
            if (!PLC_Device_冷藏區輸送門關閉.Bool) cnt = 65500;
        }
        void cnt_Program_冷藏區輸送門關閉_初始化(ref int cnt)
        {
            PLC_IO_冷藏區_輸送門開啟.Bool = false;
            PLC_Device_冷藏區輸送門開啟.Bool = false;

            cnt++;
        }
        void cnt_Program_冷藏區輸送門關閉_輸送門關閉(ref int cnt)
        {
            PLC_IO_冷藏區_輸送門關閉.Bool = true;
            if (PLC_IO_冷藏區_輸送台關閉到位.Bool)
            {
                cnt++;
            }

        }
        #endregion


        #region PLC_常溫區輸送帶前進
        PLC_Device PLC_Device_常溫區輸送帶前進 = new PLC_Device("S5250");
        PLC_Device PLC_Device_常溫區輸送帶前進_OK = new PLC_Device("");
        Task Task_常溫區輸送帶前進;
        MyTimer MyTimer_常溫區輸送帶前進_結束延遲 = new MyTimer();
        MyTimer MyTimer_常溫區輸送帶前進_開始延遲 = new MyTimer();
        int cnt_Program_常溫區輸送帶前進 = 65534;
        void sub_Program_常溫區輸送帶前進()
        {
            if (cnt_Program_常溫區輸送帶前進 == 65534)
            {
                this.MyTimer_常溫區輸送帶前進_結束延遲.StartTickTime(10000);
                this.MyTimer_常溫區輸送帶前進_開始延遲.StartTickTime(10000);
                PLC_Device_常溫區輸送帶前進.SetComment("PLC_常溫區輸送帶前進");
                PLC_Device_常溫區輸送帶前進_OK.SetComment("PLC_常溫區輸送帶前進_OK");
                PLC_Device_常溫區輸送帶前進.Bool = false;
                cnt_Program_常溫區輸送帶前進 = 65535;
            }
            if (cnt_Program_常溫區輸送帶前進 == 65535) cnt_Program_常溫區輸送帶前進 = 1;
            if (cnt_Program_常溫區輸送帶前進 == 1) cnt_Program_常溫區輸送帶前進_檢查按下(ref cnt_Program_常溫區輸送帶前進);
            if (cnt_Program_常溫區輸送帶前進 == 2) cnt_Program_常溫區輸送帶前進_初始化(ref cnt_Program_常溫區輸送帶前進);
            if (cnt_Program_常溫區輸送帶前進 == 3) cnt_Program_常溫區輸送帶前進_輸送帶前進(ref cnt_Program_常溫區輸送帶前進);
            if (cnt_Program_常溫區輸送帶前進 == 4) cnt_Program_常溫區輸送帶前進 = 65500;
            if (cnt_Program_常溫區輸送帶前進 > 1) cnt_Program_常溫區輸送帶前進_檢查放開(ref cnt_Program_常溫區輸送帶前進);

            if (cnt_Program_常溫區輸送帶前進 == 65500)
            {
                this.MyTimer_常溫區輸送帶前進_結束延遲.TickStop();
                this.MyTimer_常溫區輸送帶前進_結束延遲.StartTickTime(10000);
                PLC_Device_常溫區輸送帶前進.Bool = false;
                PLC_IO_常溫區_輸送帶前進.Bool = false;
                cnt_Program_常溫區輸送帶前進 = 65535;
            }
        }
        void cnt_Program_常溫區輸送帶前進_檢查按下(ref int cnt)
        {
            if (PLC_Device_常溫區輸送帶前進.Bool) cnt++;
        }
        void cnt_Program_常溫區輸送帶前進_檢查放開(ref int cnt)
        {
            if (!PLC_Device_常溫區輸送帶前進.Bool) cnt = 65500;
        }
        void cnt_Program_常溫區輸送帶前進_初始化(ref int cnt)
        {
            PLC_IO_常溫區_輸送帶後退.Bool = false;
            PLC_Device_常溫區輸送帶後退.Bool = false;
            cnt++;
        }
        void cnt_Program_常溫區輸送帶前進_輸送帶前進(ref int cnt)
        {
            PLC_IO_常溫區_輸送帶前進.Bool = true;
            if (PLC_IO_常溫區_輸送台進退終點.Bool)
            {
                cnt++;
            }

        }






        #endregion
        #region PLC_常溫區輸送帶後退
        PLC_Device PLC_Device_常溫區輸送帶後退 = new PLC_Device("S5251");
        PLC_Device PLC_Device_常溫區輸送帶後退_OK = new PLC_Device("");
        Task Task_常溫區輸送帶後退;
        MyTimer MyTimer_常溫區輸送帶後退_結束延遲 = new MyTimer();
        MyTimer MyTimer_常溫區輸送帶後退_開始延遲 = new MyTimer();
        int cnt_Program_常溫區輸送帶後退 = 65534;
        void sub_Program_常溫區輸送帶後退()
        {
            PLC_Device_常溫區輸送帶後退.Bool = plC_RJ_Button_常溫區_輸送帶後退.Bool;
            if (cnt_Program_常溫區輸送帶後退 == 65534)
            {
                this.MyTimer_常溫區輸送帶後退_結束延遲.StartTickTime(10000);
                this.MyTimer_常溫區輸送帶後退_開始延遲.StartTickTime(10000);
                PLC_Device_常溫區輸送帶後退.SetComment("PLC_常溫區輸送帶後退");
                PLC_Device_常溫區輸送帶後退_OK.SetComment("PLC_常溫區輸送帶後退_OK");
                PLC_Device_常溫區輸送帶後退.Bool = false;
                cnt_Program_常溫區輸送帶後退 = 65535;
            }
            if (cnt_Program_常溫區輸送帶後退 == 65535) cnt_Program_常溫區輸送帶後退 = 1;
            if (cnt_Program_常溫區輸送帶後退 == 1) cnt_Program_常溫區輸送帶後退_檢查按下(ref cnt_Program_常溫區輸送帶後退);
            if (cnt_Program_常溫區輸送帶後退 == 2) cnt_Program_常溫區輸送帶後退_初始化(ref cnt_Program_常溫區輸送帶後退);
            if (cnt_Program_常溫區輸送帶後退 == 3) cnt_Program_常溫區輸送帶後退_輸送帶後退(ref cnt_Program_常溫區輸送帶後退);
            if (cnt_Program_常溫區輸送帶後退 == 4) cnt_Program_常溫區輸送帶後退 = 65500;
            if (cnt_Program_常溫區輸送帶後退 > 1) cnt_Program_常溫區輸送帶後退_檢查放開(ref cnt_Program_常溫區輸送帶後退);

            if (cnt_Program_常溫區輸送帶後退 == 65500)
            {
                this.MyTimer_常溫區輸送帶後退_結束延遲.TickStop();
                this.MyTimer_常溫區輸送帶後退_結束延遲.StartTickTime(10000);
                PLC_Device_常溫區輸送帶後退.Bool = false;
                PLC_IO_常溫區_輸送帶後退.Bool = false;
                cnt_Program_常溫區輸送帶後退 = 65535;
            }
        }
        void cnt_Program_常溫區輸送帶後退_檢查按下(ref int cnt)
        {
            if (PLC_Device_常溫區輸送帶後退.Bool) cnt++;
        }
        void cnt_Program_常溫區輸送帶後退_檢查放開(ref int cnt)
        {
            if (!PLC_Device_常溫區輸送帶後退.Bool) cnt = 65500;
        }
        void cnt_Program_常溫區輸送帶後退_初始化(ref int cnt)
        {
            PLC_IO_常溫區_輸送帶前進.Bool = false;
            PLC_Device_常溫區輸送帶前進.Bool = false;
            cnt++;
        }
        void cnt_Program_常溫區輸送帶後退_輸送帶後退(ref int cnt)
        {
            PLC_IO_常溫區_輸送帶後退.Bool = true;
            if (PLC_IO_常溫區_輸送台進退原點.Bool)
            {
                cnt++;
            }

        }
        #endregion
        #region PLC_常溫區輸送門開啟
        PLC_Device PLC_Device_常溫區輸送門開啟 = new PLC_Device("S5252");
        PLC_Device PLC_Device_常溫區輸送門開啟_OK = new PLC_Device("");
        Task Task_常溫區輸送門開啟;
        MyTimer MyTimer_常溫區輸送門開啟_到位延遲 = new MyTimer();
        MyTimer MyTimer_常溫區輸送門開啟_結束延遲 = new MyTimer();
        MyTimer MyTimer_常溫區輸送門開啟_開始延遲 = new MyTimer();
        int cnt_Program_常溫區輸送門開啟 = 65534;
        void sub_Program_常溫區輸送門開啟()
        {
            if (PLC_IO_常溫區_輸送台開啟到位Sensor.Bool)
            {
                if (MyTimer_常溫區輸送門開啟_到位延遲.IsTimeOut())
                {
                    PLC_IO_常溫區_輸送台開啟到位.Bool = true;
                }
            }
            else
            {
                MyTimer_常溫區輸送門開啟_到位延遲.TickStop();
                MyTimer_常溫區輸送門開啟_到位延遲.StartTickTime(常溫區_輸送台開啟到位延遲毫秒);
                PLC_IO_常溫區_輸送台開啟到位.Bool = false;
            }
            PLC_Device_常溫區輸送門開啟.Bool = plC_RJ_Button_常溫區_輸送門開啟.Bool;
            if (cnt_Program_常溫區輸送門開啟 == 65534)
            {
                this.MyTimer_常溫區輸送門開啟_結束延遲.StartTickTime(10000);
                this.MyTimer_常溫區輸送門開啟_開始延遲.StartTickTime(10000);
                PLC_Device_常溫區輸送門開啟.SetComment("PLC_常溫區輸送門開啟");
                PLC_Device_常溫區輸送門開啟_OK.SetComment("PLC_常溫區輸送門開啟_OK");
                PLC_Device_常溫區輸送門開啟.Bool = false;
                cnt_Program_常溫區輸送門開啟 = 65535;
            }
            if (cnt_Program_常溫區輸送門開啟 == 65535) cnt_Program_常溫區輸送門開啟 = 1;
            if (cnt_Program_常溫區輸送門開啟 == 1) cnt_Program_常溫區輸送門開啟_檢查按下(ref cnt_Program_常溫區輸送門開啟);
            if (cnt_Program_常溫區輸送門開啟 == 2) cnt_Program_常溫區輸送門開啟_初始化(ref cnt_Program_常溫區輸送門開啟);
            if (cnt_Program_常溫區輸送門開啟 == 3) cnt_Program_常溫區輸送門開啟_輸送門開啟(ref cnt_Program_常溫區輸送門開啟);
            if (cnt_Program_常溫區輸送門開啟 == 4) cnt_Program_常溫區輸送門開啟 = 65500;
            if (cnt_Program_常溫區輸送門開啟 > 1) cnt_Program_常溫區輸送門開啟_檢查放開(ref cnt_Program_常溫區輸送門開啟);

            if (cnt_Program_常溫區輸送門開啟 == 65500)
            {
                this.MyTimer_常溫區輸送門開啟_結束延遲.TickStop();
                this.MyTimer_常溫區輸送門開啟_結束延遲.StartTickTime(10000);
                PLC_Device_常溫區輸送門開啟.Bool = false;
                PLC_IO_常溫區_輸送門開啟.Bool = false;
                cnt_Program_常溫區輸送門開啟 = 65535;
            }
        }
        void cnt_Program_常溫區輸送門開啟_檢查按下(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門開啟.Bool) cnt++;
        }
        void cnt_Program_常溫區輸送門開啟_檢查放開(ref int cnt)
        {
            if (!PLC_Device_常溫區輸送門開啟.Bool) cnt = 65500;
        }
        void cnt_Program_常溫區輸送門開啟_初始化(ref int cnt)
        {
            PLC_IO_常溫區_輸送門關閉.Bool = false;
            PLC_Device_常溫區輸送門關閉.Bool = false;
            cnt++;
        }
        void cnt_Program_常溫區輸送門開啟_輸送門開啟(ref int cnt)
        {
            PLC_IO_常溫區_輸送門開啟.Bool = true;
            if (PLC_IO_常溫區_輸送台開啟到位.Bool)
            {
                cnt++;
            }

        }
  
        #endregion
        #region PLC_常溫區輸送門關閉
        PLC_Device PLC_Device_常溫區輸送門關閉 = new PLC_Device("S5253");
        PLC_Device PLC_Device_常溫區輸送門關閉_OK = new PLC_Device("");
        Task Task_常溫區輸送門關閉;
        MyTimer MyTimer_常溫區輸送門關閉_結束延遲 = new MyTimer();
        MyTimer MyTimer_常溫區輸送門關閉_開始延遲 = new MyTimer();
        int cnt_Program_常溫區輸送門關閉 = 65534;
        void sub_Program_常溫區輸送門關閉()
        {
            PLC_Device_常溫區輸送門關閉.Bool = plC_RJ_Button_常溫區_輸送門關閉.Bool;
            if (cnt_Program_常溫區輸送門關閉 == 65534)
            {
                this.MyTimer_常溫區輸送門關閉_結束延遲.StartTickTime(10000);
                this.MyTimer_常溫區輸送門關閉_開始延遲.StartTickTime(10000);
                PLC_Device_常溫區輸送門關閉.SetComment("PLC_常溫區輸送門關閉");
                PLC_Device_常溫區輸送門關閉_OK.SetComment("PLC_常溫區輸送門關閉_OK");
                PLC_Device_常溫區輸送門關閉.Bool = false;
                cnt_Program_常溫區輸送門關閉 = 65535;
            }
            if (cnt_Program_常溫區輸送門關閉 == 65535) cnt_Program_常溫區輸送門關閉 = 1;
            if (cnt_Program_常溫區輸送門關閉 == 1) cnt_Program_常溫區輸送門關閉_檢查按下(ref cnt_Program_常溫區輸送門關閉);
            if (cnt_Program_常溫區輸送門關閉 == 2) cnt_Program_常溫區輸送門關閉_初始化(ref cnt_Program_常溫區輸送門關閉);
            if (cnt_Program_常溫區輸送門關閉 == 3) cnt_Program_常溫區輸送門關閉_輸送門關閉(ref cnt_Program_常溫區輸送門關閉);
            if (cnt_Program_常溫區輸送門關閉 == 4) cnt_Program_常溫區輸送門關閉 = 65500;
            if (cnt_Program_常溫區輸送門關閉 > 1) cnt_Program_常溫區輸送門關閉_檢查放開(ref cnt_Program_常溫區輸送門關閉);

            if (cnt_Program_常溫區輸送門關閉 == 65500)
            {
                this.MyTimer_常溫區輸送門關閉_結束延遲.TickStop();
                this.MyTimer_常溫區輸送門關閉_結束延遲.StartTickTime(10000);
                PLC_Device_常溫區輸送門關閉.Bool = false;
                PLC_IO_常溫區_輸送門關閉.Bool = false;
                cnt_Program_常溫區輸送門關閉 = 65535;
            }
        }
        void cnt_Program_常溫區輸送門關閉_檢查按下(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門關閉.Bool) cnt++;
        }
        void cnt_Program_常溫區輸送門關閉_檢查放開(ref int cnt)
        {
            if (!PLC_Device_常溫區輸送門關閉.Bool) cnt = 65500;
        }
        void cnt_Program_常溫區輸送門關閉_初始化(ref int cnt)
        {
            PLC_IO_常溫區_輸送門開啟.Bool = false;
            PLC_Device_常溫區輸送門開啟.Bool = false;

            cnt++;
        }
        void cnt_Program_常溫區輸送門關閉_輸送門關閉(ref int cnt)
        {
            PLC_IO_常溫區_輸送門關閉.Bool = true;
            if (PLC_IO_常溫區_輸送台關閉到位.Bool)
            {
                cnt++;
            }

        }
        #endregion
        #region PLC_常溫區藥盒輸送至左方
        PLC_Device PLC_Device_常溫區藥盒輸送至左方 = new PLC_Device("S6100");
        PLC_Device PLC_Device_常溫區藥盒輸送至左方_OK = new PLC_Device("");
        Task Task_常溫區藥盒輸送至左方;
        MyTimer MyTimer_常溫區藥盒輸送至左方_結束延遲 = new MyTimer();
        MyTimer MyTimer_常溫區藥盒輸送至左方_開始延遲 = new MyTimer();
        int cnt_Program_常溫區藥盒輸送至左方 = 65534;
        void sub_Program_常溫區藥盒輸送至左方()
        {
            if (cnt_Program_常溫區藥盒輸送至左方 == 65534)
            {
                this.MyTimer_常溫區藥盒輸送至左方_結束延遲.StartTickTime(10000);
                this.MyTimer_常溫區藥盒輸送至左方_開始延遲.StartTickTime(10000);
                PLC_Device_常溫區藥盒輸送至左方.SetComment("PLC_常溫區藥盒輸送至左方");
                PLC_Device_常溫區藥盒輸送至左方_OK.SetComment("PLC_常溫區藥盒輸送至左方_OK");
                PLC_Device_常溫區藥盒輸送至左方.Bool = false;
                cnt_Program_常溫區藥盒輸送至左方 = 65535;
            }
            if (cnt_Program_常溫區藥盒輸送至左方 == 65535) cnt_Program_常溫區藥盒輸送至左方 = 1;
            if (cnt_Program_常溫區藥盒輸送至左方 == 1) cnt_Program_常溫區藥盒輸送至左方_檢查按下(ref cnt_Program_常溫區藥盒輸送至左方);
            if (cnt_Program_常溫區藥盒輸送至左方 == 2) cnt_Program_常溫區藥盒輸送至左方_初始化(ref cnt_Program_常溫區藥盒輸送至左方);
            if (cnt_Program_常溫區藥盒輸送至左方 == 3) cnt_Program_常溫區藥盒輸送至左方_輸送帶啟動(ref cnt_Program_常溫區藥盒輸送至左方);
            if (cnt_Program_常溫區藥盒輸送至左方 == 4) cnt_Program_常溫區藥盒輸送至左方 = 65500;
            if (cnt_Program_常溫區藥盒輸送至左方 > 1) cnt_Program_常溫區藥盒輸送至左方_檢查放開(ref cnt_Program_常溫區藥盒輸送至左方);

            if (cnt_Program_常溫區藥盒輸送至左方 == 65500)
            {
                this.MyTimer_常溫區藥盒輸送至左方_結束延遲.TickStop();
                this.MyTimer_常溫區藥盒輸送至左方_結束延遲.StartTickTime(10000);

                PLC_IO_常溫區_輸送帶啟動.Bool = false;
                PLC_IO_常溫區_輸送帶反轉.Bool = false;

                PLC_Device_常溫區藥盒輸送至左方.Bool = false;
                PLC_Device_常溫區藥盒輸送至左方_OK.Bool = false;

                cnt_Program_常溫區藥盒輸送至左方 = 65535;
            }
        }
        void cnt_Program_常溫區藥盒輸送至左方_檢查按下(ref int cnt)
        {
            if (PLC_Device_常溫區藥盒輸送至左方.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_常溫區藥盒輸送至左方_檢查放開(ref int cnt)
        {
            if (!PLC_Device_常溫區藥盒輸送至左方.Bool) cnt = 65500;
        }
        void cnt_Program_常溫區藥盒輸送至左方_初始化(ref int cnt)
        {
            PLC_IO_常溫區_輸送帶啟動.Bool = false;
            PLC_IO_常溫區_輸送帶反轉.Bool = false;

            cnt++;
        }
        void cnt_Program_常溫區藥盒輸送至左方_輸送帶啟動(ref int cnt)
        {
            if (PLC_IO_常溫區_藥盒左感應.Bool)
            {
                cnt++;
                return;
            }
            PLC_IO_常溫區_輸送帶反轉.Bool = true;
            PLC_IO_常溫區_輸送帶啟動.Bool = true;
        }
        #endregion
        #region PLC_常溫區藥盒輸送至右方
        PLC_Device PLC_Device_常溫區藥盒輸送至右方 = new PLC_Device("S6102");
        PLC_Device PLC_Device_常溫區藥盒輸送至右方_OK = new PLC_Device("");
        Task Task_常溫區藥盒輸送至右方;
        MyTimer MyTimer_常溫區藥盒輸送至右方_結束延遲 = new MyTimer();
        MyTimer MyTimer_常溫區藥盒輸送至右方_開始延遲 = new MyTimer();
        int cnt_Program_常溫區藥盒輸送至右方 = 65534;
        void sub_Program_常溫區藥盒輸送至右方()
        {
            if (cnt_Program_常溫區藥盒輸送至右方 == 65534)
            {
                this.MyTimer_常溫區藥盒輸送至右方_結束延遲.StartTickTime(10000);
                this.MyTimer_常溫區藥盒輸送至右方_開始延遲.StartTickTime(10000);
                PLC_Device_常溫區藥盒輸送至右方.SetComment("PLC_常溫區藥盒輸送至右方");
                PLC_Device_常溫區藥盒輸送至右方_OK.SetComment("PLC_常溫區藥盒輸送至右方_OK");
                PLC_Device_常溫區藥盒輸送至右方.Bool = false;
                cnt_Program_常溫區藥盒輸送至右方 = 65535;
            }
            if (cnt_Program_常溫區藥盒輸送至右方 == 65535) cnt_Program_常溫區藥盒輸送至右方 = 1;
            if (cnt_Program_常溫區藥盒輸送至右方 == 1) cnt_Program_常溫區藥盒輸送至右方_檢查按下(ref cnt_Program_常溫區藥盒輸送至右方);
            if (cnt_Program_常溫區藥盒輸送至右方 == 2) cnt_Program_常溫區藥盒輸送至右方_初始化(ref cnt_Program_常溫區藥盒輸送至右方);
            if (cnt_Program_常溫區藥盒輸送至右方 == 3) cnt_Program_常溫區藥盒輸送至右方_輸送帶啟動(ref cnt_Program_常溫區藥盒輸送至右方);
            if (cnt_Program_常溫區藥盒輸送至右方 == 4) cnt_Program_常溫區藥盒輸送至右方 = 65500;
            if (cnt_Program_常溫區藥盒輸送至右方 > 1) cnt_Program_常溫區藥盒輸送至右方_檢查放開(ref cnt_Program_常溫區藥盒輸送至右方);

            if (cnt_Program_常溫區藥盒輸送至右方 == 65500)
            {
                this.MyTimer_常溫區藥盒輸送至右方_結束延遲.TickStop();
                this.MyTimer_常溫區藥盒輸送至右方_結束延遲.StartTickTime(10000);

                PLC_IO_常溫區_輸送帶啟動.Bool = false;
                PLC_IO_常溫區_輸送帶反轉.Bool = false;

                PLC_Device_常溫區藥盒輸送至右方.Bool = false;
                PLC_Device_常溫區藥盒輸送至右方_OK.Bool = false;

                cnt_Program_常溫區藥盒輸送至右方 = 65535;
            }
        }
        void cnt_Program_常溫區藥盒輸送至右方_檢查按下(ref int cnt)
        {
            if (PLC_Device_常溫區藥盒輸送至右方.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_常溫區藥盒輸送至右方_檢查放開(ref int cnt)
        {
            if (!PLC_Device_常溫區藥盒輸送至右方.Bool) cnt = 65500;
        }
        void cnt_Program_常溫區藥盒輸送至右方_初始化(ref int cnt)
        {
            PLC_IO_常溫區_輸送帶啟動.Bool = false;
            PLC_IO_常溫區_輸送帶反轉.Bool = false;

            cnt++;
        }
        void cnt_Program_常溫區藥盒輸送至右方_輸送帶啟動(ref int cnt)
        {
            if (PLC_IO_常溫區_藥盒右感應.Bool)
            {
                cnt++;
                return;
            }
            PLC_IO_常溫區_輸送帶反轉.Bool = false;
            PLC_IO_常溫區_輸送帶啟動.Bool = true;
        }
        #endregion
        #region PLC_常溫區藥盒輸送至中間
        PLC_Device PLC_Device_常溫區藥盒輸送至中間 = new PLC_Device("S6101");
        PLC_Device PLC_Device_常溫區藥盒輸送至中間_OK = new PLC_Device("");
        Task Task_常溫區藥盒輸送至中間;
        MyTimer MyTimer_常溫區藥盒輸送至中間_結束延遲 = new MyTimer();
        MyTimer MyTimer_常溫區藥盒輸送至中間_開始延遲 = new MyTimer();
        int cnt_Program_常溫區藥盒輸送至中間 = 65534;
        void sub_Program_常溫區藥盒輸送至中間()
        {
            if (cnt_Program_常溫區藥盒輸送至中間 == 65534)
            {
                this.MyTimer_常溫區藥盒輸送至中間_結束延遲.StartTickTime(10000);
                this.MyTimer_常溫區藥盒輸送至中間_開始延遲.StartTickTime(10000);
                PLC_Device_常溫區藥盒輸送至中間.SetComment("PLC_常溫區藥盒輸送至中間");
                PLC_Device_常溫區藥盒輸送至中間_OK.SetComment("PLC_常溫區藥盒輸送至中間_OK");
                PLC_Device_常溫區藥盒輸送至中間.Bool = false;
                cnt_Program_常溫區藥盒輸送至中間 = 65535;
            }
            if (cnt_Program_常溫區藥盒輸送至中間 == 65535) cnt_Program_常溫區藥盒輸送至中間 = 1;
            if (cnt_Program_常溫區藥盒輸送至中間 == 1) cnt_Program_常溫區藥盒輸送至中間_檢查按下(ref cnt_Program_常溫區藥盒輸送至中間);
            if (cnt_Program_常溫區藥盒輸送至中間 == 2) cnt_Program_常溫區藥盒輸送至中間_初始化(ref cnt_Program_常溫區藥盒輸送至中間);
            if (cnt_Program_常溫區藥盒輸送至中間 == 3) cnt_Program_常溫區藥盒輸送至中間_輸送至右方(ref cnt_Program_常溫區藥盒輸送至中間);
            if (cnt_Program_常溫區藥盒輸送至中間 == 4) cnt_Program_常溫區藥盒輸送至中間_等待輸送至右方完成(ref cnt_Program_常溫區藥盒輸送至中間);
            if (cnt_Program_常溫區藥盒輸送至中間 == 5) cnt_Program_常溫區藥盒輸送至中間_輸送帶啟動(ref cnt_Program_常溫區藥盒輸送至中間);
            if (cnt_Program_常溫區藥盒輸送至中間 == 6) cnt_Program_常溫區藥盒輸送至中間 = 65500;
            if (cnt_Program_常溫區藥盒輸送至中間 > 1) cnt_Program_常溫區藥盒輸送至中間_檢查放開(ref cnt_Program_常溫區藥盒輸送至中間);

            if (cnt_Program_常溫區藥盒輸送至中間 == 65500)
            {
                this.MyTimer_常溫區藥盒輸送至中間_結束延遲.TickStop();
                this.MyTimer_常溫區藥盒輸送至中間_結束延遲.StartTickTime(10000);

                PLC_IO_常溫區_輸送帶啟動.Bool = false;
                PLC_IO_常溫區_輸送帶反轉.Bool = false;
                PLC_Device_常溫區藥盒輸送至右方.Bool = false;
                PLC_Device_常溫區藥盒輸送至中間.Bool = false;
                PLC_Device_常溫區藥盒輸送至中間_OK.Bool = false;

                cnt_Program_常溫區藥盒輸送至中間 = 65535;
            }
        }
        void cnt_Program_常溫區藥盒輸送至中間_檢查按下(ref int cnt)
        {
            if (PLC_Device_常溫區藥盒輸送至中間.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_常溫區藥盒輸送至中間_檢查放開(ref int cnt)
        {
            if (!PLC_Device_常溫區藥盒輸送至中間.Bool) cnt = 65500;
        }
        void cnt_Program_常溫區藥盒輸送至中間_初始化(ref int cnt)
        {
            PLC_IO_常溫區_輸送帶啟動.Bool = false;
            PLC_IO_常溫區_輸送帶反轉.Bool = false;

            cnt++;
        }
        void cnt_Program_常溫區藥盒輸送至中間_輸送至右方(ref int cnt)
        {
            if (PLC_Device_常溫區藥盒輸送至右方.Bool) return;
            PLC_Device_常溫區藥盒輸送至右方.Bool = true;
            cnt++;
        }
        void cnt_Program_常溫區藥盒輸送至中間_等待輸送至右方完成(ref int cnt)
        {
            if (PLC_Device_常溫區藥盒輸送至右方.Bool) return;
            cnt++;
        }
        void cnt_Program_常溫區藥盒輸送至中間_輸送帶啟動(ref int cnt)
        {
            if (PLC_IO_常溫區_藥盒中感應.Bool)
            {
                cnt++;
                return;
            }
            PLC_IO_常溫區_輸送帶反轉.Bool = true;
            PLC_IO_常溫區_輸送帶啟動.Bool = true;
        }
        #endregion

        #region PLC_進出盒區藥盒輸送至左方
        MyTimer MyTimer_進出盒區藥盒輸送至左方_到位延遲 = new MyTimer();

        PLC_Device PLC_Device_進出盒區藥盒輸送至左方 = new PLC_Device("S7100");
        PLC_Device PLC_Device_進出盒區藥盒輸送至左方_OK = new PLC_Device("");
        Task Task_進出盒區藥盒輸送至左方;
        MyTimer MyTimer_進出盒區藥盒輸送至左方_結束延遲 = new MyTimer();
        MyTimer MyTimer_進出盒區藥盒輸送至左方_開始延遲 = new MyTimer();
        int cnt_Program_進出盒區藥盒輸送至左方 = 65534;
        void sub_Program_進出盒區藥盒輸送至左方()
        {
            if (PLC_IO_進出盒區_藥盒到位感應Sensor.Bool)
            {
                if (MyTimer_進出盒區藥盒輸送至左方_到位延遲.IsTimeOut())
                {
                    PLC_IO_進出盒區_藥盒到位感應.Bool = true;
                }
            }
            else
            {
                MyTimer_進出盒區藥盒輸送至左方_到位延遲.TickStop();
                MyTimer_進出盒區藥盒輸送至左方_到位延遲.StartTickTime(500);
                PLC_IO_進出盒區_藥盒到位感應.Bool = false;
            }
            if (cnt_Program_進出盒區藥盒輸送至左方 == 65534)
            {
                this.MyTimer_進出盒區藥盒輸送至左方_結束延遲.StartTickTime(10000);
                this.MyTimer_進出盒區藥盒輸送至左方_開始延遲.StartTickTime(10000);
                PLC_Device_進出盒區藥盒輸送至左方.SetComment("PLC_進出盒區藥盒輸送至左方");
                PLC_Device_進出盒區藥盒輸送至左方_OK.SetComment("PLC_進出盒區藥盒輸送至左方_OK");
                PLC_Device_進出盒區藥盒輸送至左方.Bool = false;
                cnt_Program_進出盒區藥盒輸送至左方 = 65535;
            }
            if (cnt_Program_進出盒區藥盒輸送至左方 == 65535) cnt_Program_進出盒區藥盒輸送至左方 = 1;
            if (cnt_Program_進出盒區藥盒輸送至左方 == 1) cnt_Program_進出盒區藥盒輸送至左方_檢查按下(ref cnt_Program_進出盒區藥盒輸送至左方);
            if (cnt_Program_進出盒區藥盒輸送至左方 == 2) cnt_Program_進出盒區藥盒輸送至左方_初始化(ref cnt_Program_進出盒區藥盒輸送至左方);
            if (cnt_Program_進出盒區藥盒輸送至左方 == 3) cnt_Program_進出盒區藥盒輸送至左方_輸送帶啟動(ref cnt_Program_進出盒區藥盒輸送至左方);
            if (cnt_Program_進出盒區藥盒輸送至左方 == 4) cnt_Program_進出盒區藥盒輸送至左方 = 65500;
            if (cnt_Program_進出盒區藥盒輸送至左方 > 1) cnt_Program_進出盒區藥盒輸送至左方_檢查放開(ref cnt_Program_進出盒區藥盒輸送至左方);

            if (cnt_Program_進出盒區藥盒輸送至左方 == 65500)
            {
                this.MyTimer_進出盒區藥盒輸送至左方_結束延遲.TickStop();
                this.MyTimer_進出盒區藥盒輸送至左方_結束延遲.StartTickTime(10000);

                PLC_IO_進出盒區_輸送帶啟動.Bool = false;
                PLC_IO_進出盒區_輸送帶反轉.Bool = false;

                PLC_Device_進出盒區藥盒輸送至左方.Bool = false;
                PLC_Device_進出盒區藥盒輸送至左方_OK.Bool = false;

                cnt_Program_進出盒區藥盒輸送至左方 = 65535;
            }
        }
        void cnt_Program_進出盒區藥盒輸送至左方_檢查按下(ref int cnt)
        {
            if (PLC_Device_進出盒區藥盒輸送至左方.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_進出盒區藥盒輸送至左方_檢查放開(ref int cnt)
        {
            if (!PLC_Device_進出盒區藥盒輸送至左方.Bool) cnt = 65500;
        }
        void cnt_Program_進出盒區藥盒輸送至左方_初始化(ref int cnt)
        {
            PLC_IO_進出盒區_輸送帶啟動.Bool = false;
            PLC_IO_進出盒區_輸送帶反轉.Bool = false;

            cnt++;
        }
        void cnt_Program_進出盒區藥盒輸送至左方_輸送帶啟動(ref int cnt)
        {
            if (PLC_IO_進出盒區_藥盒到位感應.Bool)
            {
                cnt++;
                return;
            }
            PLC_IO_進出盒區_輸送帶反轉.Bool = false;
            PLC_IO_進出盒區_輸送帶啟動.Bool = true;
        }
        #endregion

        #region PLC_將藥盒從冷藏區傳接至常溫區
        PLC_Device PLC_Device_將藥盒從冷藏區傳接至常溫區 = new PLC_Device("M12000");
        PLC_Device PLC_Device_將藥盒從冷藏區傳接至常溫區_OK = new PLC_Device("");
        PLC_Device PLC_Device_將藥盒從冷藏區傳接至常溫區_已到傳接位置 = new PLC_Device("");
        PLC_Device PLC_Device_將藥盒從冷藏區傳接至常溫區_已到結束待命位置 = new PLC_Device("");
        Task Task_將藥盒從冷藏區傳接至常溫區;
        MyTimer MyTimer_將藥盒從冷藏區傳接至常溫區_結束延遲 = new MyTimer();
        MyTimer MyTimer_將藥盒從冷藏區傳接至常溫區_開始延遲 = new MyTimer();
        int cnt_Program_將藥盒從冷藏區傳接至常溫區 = 65534;
        void sub_Program_將藥盒從冷藏區傳接至常溫區()
        {
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 65534)
            {
                this.MyTimer_將藥盒從冷藏區傳接至常溫區_結束延遲.StartTickTime(10000);
                this.MyTimer_將藥盒從冷藏區傳接至常溫區_開始延遲.StartTickTime(10000);
                PLC_Device_將藥盒從冷藏區傳接至常溫區.SetComment("PLC_將藥盒從冷藏區傳接至常溫區");
                PLC_Device_將藥盒從冷藏區傳接至常溫區_OK.SetComment("PLC_將藥盒從冷藏區傳接至常溫區_OK");
                PLC_Device_將藥盒從冷藏區傳接至常溫區.Bool = false;
                cnt_Program_將藥盒從冷藏區傳接至常溫區 = 65535;
            }
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 65535) cnt_Program_將藥盒從冷藏區傳接至常溫區 = 1;
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 1) cnt_Program_將藥盒從冷藏區傳接至常溫區_檢查按下(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 2) cnt_Program_將藥盒從冷藏區傳接至常溫區_初始化(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 3) cnt_Program_將藥盒從冷藏區傳接至常溫區_移動至待命位置(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 4) cnt_Program_將藥盒從冷藏區傳接至常溫區_等待移動至待命位置完成(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 5) cnt_Program_將藥盒從冷藏區傳接至常溫區_冷藏區輸送門開啟(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 6) cnt_Program_將藥盒從冷藏區傳接至常溫區_等待冷藏區輸送門開啟完成(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 7) cnt_Program_將藥盒從冷藏區傳接至常溫區_移動至傳接位置(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 8) cnt_Program_將藥盒從冷藏區傳接至常溫區_等待移動至傳接位置完成(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 9) cnt_Program_將藥盒從冷藏區傳接至常溫區_輸送帶傳接(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 10) cnt_Program_將藥盒從冷藏區傳接至常溫區_等待輸送帶傳接完成(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 11) cnt_Program_將藥盒從冷藏區傳接至常溫區_移動至結束位置(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 12) cnt_Program_將藥盒從冷藏區傳接至常溫區_等待移動至結束位置完成(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 13) cnt_Program_將藥盒從冷藏區傳接至常溫區_冷藏區輸送門關閉(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 14) cnt_Program_將藥盒從冷藏區傳接至常溫區_等待冷藏區輸送門關閉完成(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 15) cnt_Program_將藥盒從冷藏區傳接至常溫區 = 65500;
            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 > 1) cnt_Program_將藥盒從冷藏區傳接至常溫區_檢查放開(ref cnt_Program_將藥盒從冷藏區傳接至常溫區);

            if (cnt_Program_將藥盒從冷藏區傳接至常溫區 == 65500)
            {
                this.MyTimer_將藥盒從冷藏區傳接至常溫區_結束延遲.TickStop();
                this.MyTimer_將藥盒從冷藏區傳接至常溫區_結束延遲.StartTickTime(10000);

                PLC_Device_冷藏區_移動至待命位置.Bool = false;
                PLC_Device_常溫區_移動至待命位置.Bool = false;
                PLC_Device_冷藏區輸送門開啟.Bool = false;
                PLC_Device_冷藏區輸送門關閉.Bool = false;
                PLC_Device_將藥盒從冷藏區傳接至常溫區.Bool = false;
                PLC_Device_常溫區藥盒輸送至左方.Bool = false;
                PLC_IO_冷藏區_輸送帶啟動.Bool = false;
                PLC_Device_冷藏區_移動至與常溫區藥盒傳接位置.Bool = false;
                PLC_Device_常溫區_移動至與冷藏區藥盒傳接位置.Bool = false;


                PLC_Device_將藥盒從冷藏區傳接至常溫區_OK.Bool = false;

                cnt_Program_將藥盒從冷藏區傳接至常溫區 = 65535;
            }
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_檢查按下(ref int cnt)
        {
            if (PLC_Device_將藥盒從冷藏區傳接至常溫區.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_檢查放開(ref int cnt)
        {
            if (!PLC_Device_將藥盒從冷藏區傳接至常溫區.Bool) cnt = 65500;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_初始化(ref int cnt)
        {
            PLC_Device_將藥盒從冷藏區傳接至常溫區_已到傳接位置.Bool = false;
            PLC_Device_將藥盒從冷藏區傳接至常溫區_已到結束待命位置.Bool = false;
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_移動至待命位置(ref int cnt)
        {
            if (PLC_Device_冷藏區_移動至待命位置.Bool || PLC_Device_常溫區_移動至待命位置.Bool)
            {
                return;
            }
            PLC_Device_冷藏區_移動至待命位置.Bool = true;
            PLC_Device_常溫區_移動至待命位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_等待移動至待命位置完成(ref int cnt)
        {
            if (PLC_Device_冷藏區_移動至待命位置.Bool || PLC_Device_常溫區_移動至待命位置.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_冷藏區輸送門開啟(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門開啟.Bool)
            {
                return;
            }
            PLC_Device_冷藏區輸送門開啟.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_等待冷藏區輸送門開啟完成(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門開啟.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_移動至傳接位置(ref int cnt)
        {
            if (PLC_Device_冷藏區_移動至與常溫區藥盒傳接位置.Bool || PLC_Device_常溫區_移動至與冷藏區藥盒傳接位置.Bool)
            {
                return;
            }
            PLC_Device_冷藏區_移動至與常溫區藥盒傳接位置.Bool = true;
            PLC_Device_常溫區_移動至與冷藏區藥盒傳接位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_等待移動至傳接位置完成(ref int cnt)
        {
            if (PLC_Device_冷藏區_移動至與常溫區藥盒傳接位置.Bool || PLC_Device_常溫區_移動至與冷藏區藥盒傳接位置.Bool)
            {
                return;
            }
            PLC_Device_將藥盒從冷藏區傳接至常溫區_已到傳接位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_移動至結束位置(ref int cnt)
        {
            if (PLC_Device_冷藏區_移動至待命位置.Bool || PLC_Device_常溫區_移動至待命位置.Bool)
            {
                return;
            }
            PLC_Device_冷藏區_移動至待命位置.Bool = true;
            PLC_Device_常溫區_移動至待命位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_等待移動至結束位置完成(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至待命位置.Bool)
            {
                return;
            }

            if (PLC_Device_冷藏區_移動至待命位置.Bool)
            {
                return;
            }
            PLC_Device_將藥盒從冷藏區傳接至常溫區_已到結束待命位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_輸送帶傳接(ref int cnt)
        {
            if (PLC_Device_常溫區藥盒輸送至左方.Bool)
            {
                return;
            }
            PLC_Device_常溫區藥盒輸送至左方.Bool = true;
            PLC_IO_冷藏區_輸送帶反轉.Bool = true;
            PLC_IO_冷藏區_輸送帶啟動.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_等待輸送帶傳接完成(ref int cnt)
        {
            if (PLC_Device_常溫區藥盒輸送至左方.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_冷藏區輸送門關閉(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門關閉.Bool)
            {
                return;
            }
            PLC_Device_冷藏區輸送門關閉.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從冷藏區傳接至常溫區_等待冷藏區輸送門關閉完成(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門關閉.Bool)
            {
                return;
            }
            cnt++;
        }


        #endregion
        #region PLC_將藥盒從常溫區傳接至冷藏區
        PLC_Device PLC_Device_將藥盒從常溫區傳接至冷藏區 = new PLC_Device("M12001");
        PLC_Device PLC_Device_將藥盒從常溫區傳接至冷藏區_OK = new PLC_Device("");
        Task Task_將藥盒從常溫區傳接至冷藏區;
        MyTimer MyTimer_將藥盒從常溫區傳接至冷藏區_結束延遲 = new MyTimer();
        MyTimer MyTimer_將藥盒從常溫區傳接至冷藏區_開始延遲 = new MyTimer();
        int cnt_Program_將藥盒從常溫區傳接至冷藏區 = 65534;
        void sub_Program_將藥盒從常溫區傳接至冷藏區()
        {
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 65534)
            {
                this.MyTimer_將藥盒從常溫區傳接至冷藏區_結束延遲.StartTickTime(10000);
                this.MyTimer_將藥盒從常溫區傳接至冷藏區_開始延遲.StartTickTime(10000);
                PLC_Device_將藥盒從常溫區傳接至冷藏區.SetComment("PLC_將藥盒從常溫區傳接至冷藏區");
                PLC_Device_將藥盒從常溫區傳接至冷藏區_OK.SetComment("PLC_將藥盒從常溫區傳接至冷藏區_OK");
                PLC_Device_將藥盒從常溫區傳接至冷藏區.Bool = false;
                cnt_Program_將藥盒從常溫區傳接至冷藏區 = 65535;
            }
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 65535) cnt_Program_將藥盒從常溫區傳接至冷藏區 = 1;
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 1) cnt_Program_將藥盒從常溫區傳接至冷藏區_檢查按下(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 2) cnt_Program_將藥盒從常溫區傳接至冷藏區_初始化(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 3) cnt_Program_將藥盒從常溫區傳接至冷藏區_移動至待命位置(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 4) cnt_Program_將藥盒從常溫區傳接至冷藏區_等待移動至待命位置完成(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 5) cnt_Program_將藥盒從常溫區傳接至冷藏區_冷藏區輸送門開啟(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 6) cnt_Program_將藥盒從常溫區傳接至冷藏區_等待冷藏區輸送門開啟完成(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 7) cnt_Program_將藥盒從常溫區傳接至冷藏區_移動至傳接位置(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 8) cnt_Program_將藥盒從常溫區傳接至冷藏區_等待移動至傳接位置完成(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 9) cnt_Program_將藥盒從常溫區傳接至冷藏區_輸送帶傳接(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 10) cnt_Program_將藥盒從常溫區傳接至冷藏區_等待輸送帶傳接完成(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 11) cnt_Program_將藥盒從常溫區傳接至冷藏區_移動至待命位置(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 12) cnt_Program_將藥盒從常溫區傳接至冷藏區_等待移動至待命位置完成(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 13) cnt_Program_將藥盒從常溫區傳接至冷藏區_冷藏區輸送門關閉(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 14) cnt_Program_將藥盒從常溫區傳接至冷藏區_等待冷藏區輸送門關閉完成(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 15) cnt_Program_將藥盒從常溫區傳接至冷藏區 = 65500;
            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 > 1) cnt_Program_將藥盒從常溫區傳接至冷藏區_檢查放開(ref cnt_Program_將藥盒從常溫區傳接至冷藏區);

            if (cnt_Program_將藥盒從常溫區傳接至冷藏區 == 65500)
            {
                this.MyTimer_將藥盒從常溫區傳接至冷藏區_結束延遲.TickStop();
                this.MyTimer_將藥盒從常溫區傳接至冷藏區_結束延遲.StartTickTime(10000);

                PLC_Device_冷藏區_移動至待命位置.Bool = false;
                PLC_Device_常溫區_移動至待命位置.Bool = false;
                PLC_Device_冷藏區輸送門開啟.Bool = false;
                PLC_Device_冷藏區輸送門關閉.Bool = false;
                PLC_Device_將藥盒從常溫區傳接至冷藏區.Bool = false;
                PLC_Device_冷藏區藥盒輸送至右方.Bool = false;
                PLC_IO_常溫區_輸送帶啟動.Bool = false;
                PLC_Device_冷藏區_移動至與常溫區藥盒傳接位置.Bool = false;
                PLC_Device_常溫區_移動至與冷藏區藥盒傳接位置.Bool = false;


                PLC_Device_將藥盒從常溫區傳接至冷藏區_OK.Bool = false;

                cnt_Program_將藥盒從常溫區傳接至冷藏區 = 65535;
            }
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_檢查按下(ref int cnt)
        {
            if (PLC_Device_將藥盒從常溫區傳接至冷藏區.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_檢查放開(ref int cnt)
        {
            if (!PLC_Device_將藥盒從常溫區傳接至冷藏區.Bool) cnt = 65500;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_初始化(ref int cnt)
        {

            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_移動至待命位置(ref int cnt)
        {
            if (PLC_Device_冷藏區_移動至待命位置.Bool || PLC_Device_常溫區_移動至待命位置.Bool)
            {
                return;
            }
            PLC_Device_冷藏區_移動至待命位置.Bool = true;
            PLC_Device_常溫區_移動至待命位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_等待移動至待命位置完成(ref int cnt)
        {
            if (PLC_Device_冷藏區_移動至待命位置.Bool || PLC_Device_常溫區_移動至待命位置.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_冷藏區輸送門開啟(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門開啟.Bool)
            {
                return;
            }
            PLC_Device_冷藏區輸送門開啟.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_等待冷藏區輸送門開啟完成(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門開啟.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_移動至傳接位置(ref int cnt)
        {
            if (PLC_Device_冷藏區_移動至與常溫區藥盒傳接位置.Bool || PLC_Device_常溫區_移動至與冷藏區藥盒傳接位置.Bool)
            {
                return;
            }
            PLC_Device_冷藏區_移動至與常溫區藥盒傳接位置.Bool = true;
            PLC_Device_常溫區_移動至與冷藏區藥盒傳接位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_等待移動至傳接位置完成(ref int cnt)
        {
            if (PLC_Device_冷藏區_移動至與常溫區藥盒傳接位置.Bool || PLC_Device_常溫區_移動至與冷藏區藥盒傳接位置.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_輸送帶傳接(ref int cnt)
        {
            if (PLC_Device_冷藏區藥盒輸送至右方.Bool)
            {
                return;
            }
            PLC_Device_冷藏區藥盒輸送至右方.Bool = true;
            PLC_IO_常溫區_輸送帶反轉.Bool = false;
            PLC_IO_常溫區_輸送帶啟動.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_等待輸送帶傳接完成(ref int cnt)
        {
            if (PLC_Device_冷藏區藥盒輸送至右方.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_冷藏區輸送門關閉(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門關閉.Bool)
            {
                return;
            }
            PLC_Device_冷藏區輸送門關閉.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至冷藏區_等待冷藏區輸送門關閉完成(ref int cnt)
        {
            if (PLC_Device_冷藏區輸送門關閉.Bool)
            {
                return;
            }
            cnt++;
        }
        #endregion
        #region PLC_將藥盒從常溫區傳接至進出盒區
        PLC_Device PLC_Device_將藥盒從常溫區傳接至進出盒區 = new PLC_Device("M12002");
        PLC_Device PLC_Device_將藥盒從常溫區傳接至進出盒區_OK = new PLC_Device("");
        Task Task_將藥盒從常溫區傳接至進出盒區;
        MyTimer MyTimer_將藥盒從常溫區傳接至進出盒區_結束延遲 = new MyTimer();
        MyTimer MyTimer_將藥盒從常溫區傳接至進出盒區_開始延遲 = new MyTimer();
        int cnt_Program_將藥盒從常溫區傳接至進出盒區 = 65534;
        void sub_Program_將藥盒從常溫區傳接至進出盒區()
        {
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 65534)
            {
                this.MyTimer_將藥盒從常溫區傳接至進出盒區_結束延遲.StartTickTime(10000);
                this.MyTimer_將藥盒從常溫區傳接至進出盒區_開始延遲.StartTickTime(10000);
                PLC_Device_將藥盒從常溫區傳接至進出盒區.SetComment("PLC_將藥盒從常溫區傳接至進出盒區");
                PLC_Device_將藥盒從常溫區傳接至進出盒區_OK.SetComment("PLC_將藥盒從常溫區傳接至進出盒區_OK");
                PLC_Device_將藥盒從常溫區傳接至進出盒區.Bool = false;
                cnt_Program_將藥盒從常溫區傳接至進出盒區 = 65535;
            }
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 65535) cnt_Program_將藥盒從常溫區傳接至進出盒區 = 1;
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 1) cnt_Program_將藥盒從常溫區傳接至進出盒區_檢查按下(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 2) cnt_Program_將藥盒從常溫區傳接至進出盒區_初始化(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 3) cnt_Program_將藥盒從常溫區傳接至進出盒區_移動至待命位置(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 4) cnt_Program_將藥盒從常溫區傳接至進出盒區_等待移動至待命位置完成(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 5) cnt_Program_將藥盒從常溫區傳接至進出盒區_常溫區輸送門開啟(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 6) cnt_Program_將藥盒從常溫區傳接至進出盒區_等待常溫區輸送門開啟完成(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 7) cnt_Program_將藥盒從常溫區傳接至進出盒區_移動至傳接位置(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 8) cnt_Program_將藥盒從常溫區傳接至進出盒區_等待移動至傳接位置完成(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 9) cnt_Program_將藥盒從常溫區傳接至進出盒區_輸送帶傳接(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 10) cnt_Program_將藥盒從常溫區傳接至進出盒區_等待輸送帶傳接完成(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 11) cnt_Program_將藥盒從常溫區傳接至進出盒區_移動至結束位置(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 12) cnt_Program_將藥盒從常溫區傳接至進出盒區_等待移動至結束位置完成(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 13) cnt_Program_將藥盒從常溫區傳接至進出盒區_常溫區輸送門關閉(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 14) cnt_Program_將藥盒從常溫區傳接至進出盒區_等待常溫區輸送門關閉完成(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 15) cnt_Program_將藥盒從常溫區傳接至進出盒區 = 65500;
            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 > 1) cnt_Program_將藥盒從常溫區傳接至進出盒區_檢查放開(ref cnt_Program_將藥盒從常溫區傳接至進出盒區);

            if (cnt_Program_將藥盒從常溫區傳接至進出盒區 == 65500)
            {
                this.MyTimer_將藥盒從常溫區傳接至進出盒區_結束延遲.TickStop();
                this.MyTimer_將藥盒從常溫區傳接至進出盒區_結束延遲.StartTickTime(10000);

                PLC_Device_常溫區_移動至待命位置.Bool = false;
                PLC_Device_進出盒區_移動至與常溫區藥盒傳接位置.Bool = false;
                PLC_Device_常溫區輸送門開啟.Bool = false;
                PLC_Device_常溫區輸送門關閉.Bool = false;
                PLC_Device_將藥盒從常溫區傳接至進出盒區.Bool = false;
                PLC_Device_進出盒區藥盒輸送至左方.Bool = false;
                PLC_IO_常溫區_輸送帶啟動.Bool = false;
                PLC_Device_常溫區_移動至與進出盒區藥盒傳接位置.Bool = false;


                PLC_Device_將藥盒從常溫區傳接至進出盒區_OK.Bool = false;

                cnt_Program_將藥盒從常溫區傳接至進出盒區 = 65535;
            }
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_檢查按下(ref int cnt)
        {
            if (PLC_Device_將藥盒從常溫區傳接至進出盒區.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_檢查放開(ref int cnt)
        {
            if (!PLC_Device_將藥盒從常溫區傳接至進出盒區.Bool) cnt = 65500;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_初始化(ref int cnt)
        {

            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_移動至待命位置(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至待命位置.Bool || PLC_Device_進出盒區_移動至與常溫區藥盒傳接位置.Bool)
            {
                return;
            }
            PLC_Device_常溫區_移動至待命位置.Bool = true;
            PLC_Device_進出盒區_移動至與常溫區藥盒傳接位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_等待移動至待命位置完成(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至待命位置.Bool || PLC_Device_進出盒區_移動至與常溫區藥盒傳接位置.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_常溫區輸送門開啟(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門開啟.Bool)
            {
                return;
            }
            PLC_Device_常溫區輸送門開啟.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_等待常溫區輸送門開啟完成(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門開啟.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_移動至傳接位置(ref int cnt)
        {
            if ( PLC_Device_常溫區_移動至與進出盒區藥盒傳接位置.Bool)
            {
                return;
            }
            PLC_Device_常溫區_移動至與進出盒區藥盒傳接位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_等待移動至傳接位置完成(ref int cnt)
        {
            if ( PLC_Device_常溫區_移動至與進出盒區藥盒傳接位置.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_輸送帶傳接(ref int cnt)
        {
            if (PLC_Device_進出盒區藥盒輸送至左方.Bool)
            {
                return;
            }
            PLC_Device_進出盒區藥盒輸送至左方.Bool = true;
            PLC_IO_常溫區_輸送帶反轉.Bool = true;
            PLC_IO_常溫區_輸送帶啟動.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_等待輸送帶傳接完成(ref int cnt)
        {
            if (PLC_Device_進出盒區藥盒輸送至左方.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_移動至結束位置(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至待命位置.Bool || PLC_Device_進出盒區_移動至待命位置.Bool)
            {
                return;
            }
            PLC_Device_常溫區_移動至待命位置.Bool = true;
            PLC_Device_進出盒區_移動至待命位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_等待移動至結束位置完成(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至待命位置.Bool || PLC_Device_進出盒區_移動至待命位置.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_常溫區輸送門關閉(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門關閉.Bool)
            {
                return;
            }
            PLC_Device_常溫區輸送門關閉.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從常溫區傳接至進出盒區_等待常溫區輸送門關閉完成(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門關閉.Bool)
            {
                return;
            }
            cnt++;
        }
        #endregion
        #region PLC_將藥盒從進出盒區傳接至常溫區
        PLC_Device PLC_Device_將藥盒從進出盒區傳接至常溫區 = new PLC_Device("M12003");
        PLC_Device PLC_Device_將藥盒從進出盒區傳接至常溫區_OK = new PLC_Device("");
        Task Task_將藥盒從進出盒區傳接至常溫區;
        MyTimer MyTimer_將藥盒從進出盒區傳接至常溫區_結束延遲 = new MyTimer();
        MyTimer MyTimer_將藥盒從進出盒區傳接至常溫區_開始延遲 = new MyTimer();
        int cnt_Program_將藥盒從進出盒區傳接至常溫區 = 65534;
        void sub_Program_將藥盒從進出盒區傳接至常溫區()
        {
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 65534)
            {
                this.MyTimer_將藥盒從進出盒區傳接至常溫區_結束延遲.StartTickTime(10000);
                this.MyTimer_將藥盒從進出盒區傳接至常溫區_開始延遲.StartTickTime(10000);
                PLC_Device_將藥盒從進出盒區傳接至常溫區.SetComment("PLC_將藥盒從進出盒區傳接至常溫區");
                PLC_Device_將藥盒從進出盒區傳接至常溫區_OK.SetComment("PLC_將藥盒從進出盒區傳接至常溫區_OK");
                PLC_Device_將藥盒從進出盒區傳接至常溫區.Bool = false;
                cnt_Program_將藥盒從進出盒區傳接至常溫區 = 65535;
            }
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 65535) cnt_Program_將藥盒從進出盒區傳接至常溫區 = 1;
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 1) cnt_Program_將藥盒從進出盒區傳接至常溫區_檢查按下(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 2) cnt_Program_將藥盒從進出盒區傳接至常溫區_初始化(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 3) cnt_Program_將藥盒從進出盒區傳接至常溫區_移動至待命位置(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 4) cnt_Program_將藥盒從進出盒區傳接至常溫區_等待移動至待命位置完成(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 5) cnt_Program_將藥盒從進出盒區傳接至常溫區_常溫區輸送門開啟(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 6) cnt_Program_將藥盒從進出盒區傳接至常溫區_等待常溫區輸送門開啟完成(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 7) cnt_Program_將藥盒從進出盒區傳接至常溫區_移動至傳接位置(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 8) cnt_Program_將藥盒從進出盒區傳接至常溫區_等待移動至傳接位置完成(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 9) cnt_Program_將藥盒從進出盒區傳接至常溫區_輸送帶傳接(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 10) cnt_Program_將藥盒從進出盒區傳接至常溫區_等待輸送帶傳接完成(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 11) cnt_Program_將藥盒從進出盒區傳接至常溫區_移動至結束位置(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 12) cnt_Program_將藥盒從進出盒區傳接至常溫區_等待移動至結束位置完成(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 13) cnt_Program_將藥盒從進出盒區傳接至常溫區_常溫區輸送門關閉(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 14) cnt_Program_將藥盒從進出盒區傳接至常溫區_等待常溫區輸送門關閉完成(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 15) cnt_Program_將藥盒從進出盒區傳接至常溫區 = 65500;
            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 > 1) cnt_Program_將藥盒從進出盒區傳接至常溫區_檢查放開(ref cnt_Program_將藥盒從進出盒區傳接至常溫區);

            if (cnt_Program_將藥盒從進出盒區傳接至常溫區 == 65500)
            {
                this.MyTimer_將藥盒從進出盒區傳接至常溫區_結束延遲.TickStop();
                this.MyTimer_將藥盒從進出盒區傳接至常溫區_結束延遲.StartTickTime(10000);

                PLC_Device_常溫區_移動至待命位置.Bool = false;
                PLC_Device_進出盒區_移動至與常溫區藥盒傳接位置.Bool = false;
                PLC_Device_常溫區輸送門開啟.Bool = false;
                PLC_Device_常溫區輸送門關閉.Bool = false;
                PLC_Device_將藥盒從進出盒區傳接至常溫區.Bool = false;
                PLC_Device_常溫區藥盒輸送至右方.Bool = false;
                PLC_IO_進出盒區_輸送帶啟動.Bool = false;
                PLC_Device_常溫區_移動至與進出盒區藥盒傳接位置.Bool = false;


                PLC_Device_將藥盒從進出盒區傳接至常溫區_OK.Bool = false;

                cnt_Program_將藥盒從進出盒區傳接至常溫區 = 65535;
            }
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_檢查按下(ref int cnt)
        {
            if (PLC_Device_將藥盒從進出盒區傳接至常溫區.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_檢查放開(ref int cnt)
        {
            if (!PLC_Device_將藥盒從進出盒區傳接至常溫區.Bool) cnt = 65500;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_初始化(ref int cnt)
        {

            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_移動至待命位置(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至待命位置.Bool || PLC_Device_進出盒區_移動至與常溫區藥盒傳接位置.Bool)
            {
                return;
            }
            PLC_Device_常溫區_移動至待命位置.Bool = true;
            PLC_Device_進出盒區_移動至與常溫區藥盒傳接位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_等待移動至待命位置完成(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至待命位置.Bool || PLC_Device_進出盒區_移動至與常溫區藥盒傳接位置.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_常溫區輸送門開啟(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門開啟.Bool)
            {
                return;
            }
            PLC_Device_常溫區輸送門開啟.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_等待常溫區輸送門開啟完成(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門開啟.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_移動至傳接位置(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至與進出盒區藥盒傳接位置.Bool)
            {
                return;
            }
            PLC_Device_常溫區_移動至與進出盒區藥盒傳接位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_等待移動至傳接位置完成(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至與進出盒區藥盒傳接位置.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_輸送帶傳接(ref int cnt)
        {
            if (PLC_Device_常溫區藥盒輸送至右方.Bool)
            {
                return;
            }
            PLC_Device_常溫區藥盒輸送至右方.Bool = true;
            PLC_IO_進出盒區_輸送帶反轉.Bool = true;
            PLC_IO_進出盒區_輸送帶啟動.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_等待輸送帶傳接完成(ref int cnt)
        {
            if (PLC_Device_常溫區藥盒輸送至右方.Bool)
            {
         
                return;
            }
            PLC_IO_進出盒區_輸送帶反轉.Bool = false;
            PLC_IO_進出盒區_輸送帶啟動.Bool = false;
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_移動至結束位置(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至待命位置.Bool || PLC_Device_進出盒區_移動至待命位置.Bool)
            {
                return;
            }
            PLC_Device_常溫區_移動至待命位置.Bool = true;
            PLC_Device_進出盒區_移動至待命位置.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_等待移動至結束位置完成(ref int cnt)
        {
            if (PLC_Device_常溫區_移動至待命位置.Bool || PLC_Device_進出盒區_移動至待命位置.Bool)
            {
                return;
            }
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_常溫區輸送門關閉(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門關閉.Bool)
            {
                return;
            }
            PLC_Device_常溫區輸送門關閉.Bool = true;
            cnt++;
        }
        void cnt_Program_將藥盒從進出盒區傳接至常溫區_等待常溫區輸送門關閉完成(ref int cnt)
        {
            if (PLC_Device_常溫區輸送門關閉.Bool)
            {
                return;
            }
            cnt++;
        }
        #endregion
        #region PLC_出料一次
        string 出料一次_IP = "";
        string 出料一次_藥盒方位 = "";
        string 出料一次_區域 = "";
        int 出料一次_出料位置X = 0;
        int 出料一次_出料位置Z = 0;
        PLC_Device PLC_Device_出料一次 = new PLC_Device("M12100");
        PLC_Device PLC_Device_出料一次_OK = new PLC_Device("");
        Task Task_出料一次;
        MyTimer MyTimer_出料一次_結束延遲 = new MyTimer();
        MyTimer MyTimer_出料一次_開始延遲 = new MyTimer();
        MyTimer MyTimer_出料一次_觸發出料一次延遲 = new MyTimer();
        MyTimer MyTimer_出料一次_出料後延遲 = new MyTimer();
        int cnt_Program_出料一次 = 65534;
        void sub_Program_出料一次()
        {
     
            if (cnt_Program_出料一次 == 65534)
            {

                this.MyTimer_出料一次_結束延遲.StartTickTime(10000);
                this.MyTimer_出料一次_開始延遲.StartTickTime(10000);
                PLC_Device_出料一次.SetComment("PLC_出料一次");
                PLC_Device_出料一次_OK.SetComment("PLC_出料一次_OK");
                PLC_Device_出料一次.Bool = false;
                cnt_Program_出料一次 = 65535;
            }
            if (cnt_Program_出料一次 == 65535) cnt_Program_出料一次 = 1;
            if (cnt_Program_出料一次 == 1) cnt_Program_出料一次_檢查按下(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 2) cnt_Program_出料一次_初始化(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 3) cnt_Program_出料一次_輸送帶後退及藥盒定位(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 4) cnt_Program_出料一次_等待輸送帶後退完成(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 5) cnt_Program_出料一次_XZ移動至指定位置(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 6) cnt_Program_出料一次_等待XZ移動至指定位置完成(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 7) cnt_Program_出料一次_等待藥盒定位完成(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 8) cnt_Program_出料一次_輸送帶前進(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 9) cnt_Program_出料一次_等待輸送帶前進完成(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 10) cnt_Program_出料一次_觸發出料一次(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 11) cnt_Program_出料一次_等待觸發出料一次延遲(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 12) cnt_Program_出料一次_等待出料一次完成(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 13) cnt_Program_出料一次_等待出料一次完成後延遲(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 14) cnt_Program_出料一次_輸送帶後退(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 15) cnt_Program_出料一次_等待輸送帶後退完成(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 16) cnt_Program_出料一次 = 65500;
            if (cnt_Program_出料一次 > 1) cnt_Program_出料一次_檢查放開(ref cnt_Program_出料一次);

            if (cnt_Program_出料一次 == 65500)
            {
                this.MyTimer_出料一次_結束延遲.TickStop();
                this.MyTimer_出料一次_結束延遲.StartTickTime(10000);
                PLC_Device_出料一次.Bool = false;
                PLC_Device_常溫區輸送帶後退.Bool = false;
                PLC_Device_常溫區藥盒輸送至左方.Bool = false;
                PLC_Device_常溫區藥盒輸送至中間.Bool = false;
                PLC_Device_常溫區藥盒輸送至右方.Bool = false;

                PLC_Device_冷藏區輸送帶後退.Bool = false;
                PLC_Device_冷藏區藥盒輸送至左方.Bool = false;
                PLC_Device_冷藏區藥盒輸送至中間.Bool = false;
                PLC_Device_冷藏區藥盒輸送至右方.Bool = false;
                cnt_Program_出料一次 = 65535;
            }
        }
        void cnt_Program_出料一次_檢查按下(ref int cnt)
        {
            if (PLC_Device_出料一次.Bool) cnt++;
        }
        void cnt_Program_出料一次_檢查放開(ref int cnt)
        {
            if (!PLC_Device_出料一次.Bool) cnt = 65500;
        }
        void cnt_Program_出料一次_初始化(ref int cnt)
        {
            List<object[]> list_馬達輸出索引表 = this.sqL_DataGridView_馬達輸出索引表.SQL_GetRows((int)enum_storageMedBoxIOConfig.IP, 出料一次_IP, false);
         
            if(list_馬達輸出索引表.Count == 0)
            {
                PLC_Device_出料一次_OK.Bool = false;
                cnt = 65500;
                Console.WriteLine($"[出料一次] : 找無({出料一次_IP})馬達輸出索引表 {DateTime.Now.ToDateTimeString()}");
                return;
            }
            出料一次_出料位置X = (int)(list_馬達輸出索引表[0][(int)enum_storageMedBoxIOConfig.出料位置X].StringToDouble() * 100);
            出料一次_出料位置Z = (int)(list_馬達輸出索引表[0][(int)enum_storageMedBoxIOConfig.出料位置Y].StringToDouble() * 100);
            出料一次_藥盒方位 = list_馬達輸出索引表[0][(int)enum_storageMedBoxIOConfig.藥盒方位].ObjectToString();
            出料一次_區域 = list_馬達輸出索引表[0][(int)enum_storageMedBoxIOConfig.區域].ObjectToString();
            if (出料一次_區域 != "常溫" && 出料一次_區域 != "冷藏")
            {
                Console.WriteLine($"[出料一次] : ({出料一次_IP})({出料一次_區域})區域資訊錯誤 {DateTime.Now.ToDateTimeString()}");
                cnt = 65500;
                return;
            }
            if (出料一次_藥盒方位 != "左" && 出料一次_藥盒方位 != "中" && 出料一次_藥盒方位 != "右")
            {
                Console.WriteLine($"[出料一次] : ({出料一次_IP})({出料一次_藥盒方位})藥盒方位資訊錯誤 {DateTime.Now.ToDateTimeString()}");
                cnt = 65500;
                return;
            }
           
            Console.WriteLine("-----------------出料參數-----------------");
            Console.WriteLine($"IP       : {出料一次_IP}");
            Console.WriteLine($"位置X    : {出料一次_出料位置X}");
            Console.WriteLine($"位置Z    : {出料一次_出料位置Z}");
            Console.WriteLine($"藥盒方位 : {出料一次_藥盒方位}");
            Console.WriteLine($"區域     : {出料一次_區域}");
            Console.WriteLine("------------------------------------------");
            cnt++;
        }
        void cnt_Program_出料一次_輸送帶後退及藥盒定位(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (出料一次_區域 == "常溫")
                {
                    if (PLC_Device_常溫區輸送帶後退.Bool) return;
                    if (出料一次_藥盒方位 == "左")
                    {
                        if (PLC_Device_常溫區藥盒輸送至左方.Bool) return;
                        PLC_Device_常溫區輸送帶後退.Bool = true;
                        PLC_Device_常溫區藥盒輸送至左方.Bool = true;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                    if (出料一次_藥盒方位 == "中")
                    {
                        if (PLC_Device_常溫區藥盒輸送至中間.Bool) return;
                        PLC_Device_常溫區輸送帶後退.Bool = true;
                        PLC_Device_常溫區藥盒輸送至中間.Bool = true;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                    if (出料一次_藥盒方位 == "右")
                    {
                        if (PLC_Device_常溫區藥盒輸送至右方.Bool) return;
                        PLC_Device_常溫區輸送帶後退.Bool = true;
                        PLC_Device_常溫區藥盒輸送至右方.Bool = true;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                }
                if (出料一次_區域 == "冷藏")
                {
                    if (PLC_Device_冷藏區輸送帶後退.Bool) return;
                    if (出料一次_藥盒方位 == "左")
                    {
                        if (PLC_Device_冷藏區藥盒輸送至左方.Bool) return;
                        PLC_Device_冷藏區輸送帶後退.Bool = true;
                        PLC_Device_冷藏區藥盒輸送至左方.Bool = true;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                    if (出料一次_藥盒方位 == "中")
                    {
                        if (PLC_Device_冷藏區藥盒輸送至中間.Bool) return;
                        PLC_Device_冷藏區輸送帶後退.Bool = true;
                        PLC_Device_冷藏區藥盒輸送至中間.Bool = true;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                    if (出料一次_藥盒方位 == "右")
                    {
                        if (PLC_Device_冷藏區藥盒輸送至右方.Bool) return;
                        PLC_Device_冷藏區輸送帶後退.Bool = true;
                        PLC_Device_冷藏區藥盒輸送至右方.Bool = true;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] 輸送帶後退及藥盒定位 Done! {DateTime.Now.ToDateTimeString()}");
            }

        }
        void cnt_Program_出料一次_等待輸送帶後退完成(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                if (出料一次_區域 == "常溫")
                {
                    if (PLC_Device_常溫區輸送帶後退.Bool) return;
                    flag_OK = true;
                    cnt++;
                    return;
                }
                if (出料一次_區域 == "冷藏")
                {
                    if (PLC_Device_冷藏區輸送帶後退.Bool) return;
                    flag_OK = true;
                    cnt++;
                    return;
                }
            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] 等待輸送帶後退完成 Done! {DateTime.Now.ToDateTimeString()}");

            }
   
        }
        void cnt_Program_出料一次_XZ移動至指定位置(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                if (出料一次_區域 == "常溫")
                {
                    if (PLC_Device_常溫區X軸_絕對位置移動.Bool) return;
                    if (PLC_Device_常溫區Z軸_絕對位置移動.Bool) return;
                    PLC_Device_常溫區X軸_絕對位置移動_目標位置.Value = 出料一次_出料位置X;
                    PLC_Device_常溫區Z軸_絕對位置移動_目標位置.Value = 出料一次_出料位置Z;
                    PLC_Device_常溫區X軸_絕對位置移動.Bool = true;
                    PLC_Device_常溫區Z軸_絕對位置移動.Bool = true;
                    flag_OK = true;
                    cnt++;
                    return;
                }
                if (出料一次_區域 == "冷藏")
                {
                    if (PLC_Device_冷藏區X軸_絕對位置移動.Bool) return;
                    if (PLC_Device_冷藏區Z軸_絕對位置移動.Bool) return;
                    PLC_Device_冷藏區X軸_絕對位置移動_目標位置.Value = 出料一次_出料位置X;
                    PLC_Device_冷藏區Z軸_絕對位置移動_目標位置.Value = 出料一次_出料位置Z;
                    PLC_Device_冷藏區X軸_絕對位置移動.Bool = true;
                    PLC_Device_冷藏區Z軸_絕對位置移動.Bool = true;
                    flag_OK = true;
                    cnt++;
                    return;
                }
            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] XZ移動至指定位置 Done! {DateTime.Now.ToDateTimeString()}");
            }
  
        }
        void cnt_Program_出料一次_等待XZ移動至指定位置完成(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                if (出料一次_區域 == "常溫")
                {
                    if (PLC_Device_常溫區X軸_絕對位置移動.Bool) return;
                    if (PLC_Device_常溫區Z軸_絕對位置移動.Bool) return;
                    flag_OK = true;
                    cnt++;
                    return;
                }
                if (出料一次_區域 == "冷藏")
                {
                    if (PLC_Device_冷藏區X軸_絕對位置移動.Bool) return;
                    if (PLC_Device_冷藏區Z軸_絕對位置移動.Bool) return;
                    flag_OK = true;
                    cnt++;
                    return;
                }
            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] 等待XZ移動至指定位置完成 Done! {DateTime.Now.ToDateTimeString()}");
            }
    
        }
        void cnt_Program_出料一次_等待藥盒定位完成(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                if (出料一次_區域 == "常溫")
                {
                    if (出料一次_藥盒方位 == "左")
                    {
                        if (PLC_Device_常溫區藥盒輸送至左方.Bool) return;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                    if (出料一次_藥盒方位 == "中")
                    {
                        if (PLC_Device_常溫區藥盒輸送至中間.Bool) return;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                    if (出料一次_藥盒方位 == "右")
                    {
                        if (PLC_Device_常溫區藥盒輸送至右方.Bool) return;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                }
                if (出料一次_區域 == "冷藏")
                {
                    if (PLC_Device_冷藏區輸送帶後退.Bool) return;
                    if (出料一次_藥盒方位 == "左")
                    {
                        if (PLC_Device_冷藏區藥盒輸送至左方.Bool) return;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                    if (出料一次_藥盒方位 == "中")
                    {
                        if (PLC_Device_冷藏區藥盒輸送至中間.Bool) return;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                    if (出料一次_藥盒方位 == "右")
                    {
                        if (PLC_Device_冷藏區藥盒輸送至右方.Bool) return;
                        flag_OK = true;
                        cnt++;
                        return;
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] 等待藥盒定位完成 Done! {DateTime.Now.ToDateTimeString()}");
            }
    
        }
        void cnt_Program_出料一次_輸送帶前進(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                if (出料一次_區域 == "常溫")
                {
                    if (PLC_Device_常溫區輸送帶前進.Bool) return;
                    PLC_Device_常溫區輸送帶前進.Bool = true;
                    flag_OK = true;
                    cnt++;
                    return;
                }
                if (出料一次_區域 == "冷藏")
                {
                    if (PLC_Device_冷藏區輸送帶前進.Bool) return;
                    PLC_Device_冷藏區輸送帶前進.Bool = true;
                    flag_OK = true;
                    cnt++;
                    return;
                }
            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] 輸送帶前進 Done! {DateTime.Now.ToDateTimeString()}");
            }

        }
        void cnt_Program_出料一次_等待輸送帶前進完成(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                if (出料一次_區域 == "常溫")
                {
                    if (PLC_Device_常溫區輸送帶前進.Bool) return;
                    flag_OK = true;
                    cnt++;
                    return;
                }
                if (出料一次_區域 == "冷藏")
                {
                    if (PLC_Device_冷藏區輸送帶前進.Bool) return;
                    flag_OK = true;
                    cnt++;
                    return;
                }
            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] 等待輸送帶前進完成 Done! {DateTime.Now.ToDateTimeString()}");
            }

        }
        void cnt_Program_出料一次_觸發出料一次(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                string IP = 出料一次_IP;
                List<object[]> list_replace = new List<object[]>();
                List<object[]> list_馬達輸出索引表 = this.sqL_DataGridView_馬達輸出索引表.SQL_GetRows((int)enum_storageMedBoxIOConfig.IP, IP, false);
                if (list_馬達輸出索引表.Count == 0)
                {
                    Console.WriteLine($"[出料一次] 找無馬達索引 {DateTime.Now.ToDateTimeString()}");
                    cnt = 65500;
                    return;
                }
                list_馬達輸出索引表[0][(int)enum_storageMedBoxIOConfig.出料馬達輸出觸發] = true.ToString();
                list_replace.LockAdd(list_馬達輸出索引表[0]);
                if (list_replace.Count > 0) sqL_DataGridView_馬達輸出索引表.SQL_ReplaceExtra(list_replace, false);
                flag_OK = true;             
                cnt++;
                return;
            }
            catch
            {

            }
            finally
            {
                MyTimer_出料一次_觸發出料一次延遲.TickStop();
                MyTimer_出料一次_觸發出料一次延遲.StartTickTime(300);
                if (flag_OK) Console.WriteLine($"[出料一次] 觸發出料一次 Done! {DateTime.Now.ToDateTimeString()}");
            }

        }
        void cnt_Program_出料一次_等待觸發出料一次延遲(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                if (MyTimer_出料一次_觸發出料一次延遲.IsTimeOut())
                {
                    flag_OK = true;
                    cnt++;
                    return;
                }
           
            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] 等待觸發出料一次延遲 Done! {DateTime.Now.ToDateTimeString()}");
            }
        }
        void cnt_Program_出料一次_等待出料一次完成(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                string IP = 出料一次_IP;
                List<object[]> list_馬達輸出索引表 = this.sqL_DataGridView_馬達輸出索引表.SQL_GetRows((int)enum_storageMedBoxIOConfig.IP, IP, false);
                if (list_馬達輸出索引表.Count == 0)
                {
                    Console.WriteLine($"[出料一次] 找無馬達索引 {DateTime.Now.ToDateTimeString()}");
                    cnt = 65500;
                    return;
                }
                string 馬達輸出 = list_馬達輸出索引表[0][(int)enum_storageMedBoxIOConfig.出料馬達輸出索引].ObjectToString();
                PLC_Device pLC_Device = new PLC_Device($"{馬達輸出}");
                if (pLC_Device.Bool == false)
                {
                    flag_OK = true;
                    cnt++;
                    return;
                }
    
            }
            catch
            {

            }
            finally
            {
                MyTimer_出料一次_出料後延遲.TickStop();
                MyTimer_出料一次_出料後延遲.StartTickTime(plC_NumBox_出料後延遲.Value);
                if (flag_OK) Console.WriteLine($"[出料一次] 等待出料一次完成 Done! {DateTime.Now.ToDateTimeString()}");
            }

         
        }
        void cnt_Program_出料一次_等待出料一次完成後延遲(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                if (MyTimer_出料一次_出料後延遲.IsTimeOut())
                {
                    flag_OK = true;
                    cnt++;
                    return;
                }

            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] 等待出料一次完成後延遲 Done! {DateTime.Now.ToDateTimeString()}");
            }
        }
        void cnt_Program_出料一次_輸送帶後退(ref int cnt)
        {
            bool flag_OK = false;
            try
            {
                if (!Function_檢查軸控Alarm())
                {
                    cnt = 65500;
                    return;
                }
                if (出料一次_區域 == "常溫")
                {
                    if (PLC_Device_常溫區輸送帶後退.Bool) return;
                    PLC_Device_常溫區輸送帶後退.Bool = true;
                    flag_OK = true;
                    cnt++;
                    return;
                }
                if (出料一次_區域 == "冷藏")
                {
                    if (PLC_Device_冷藏區輸送帶後退.Bool) return;
                    PLC_Device_冷藏區輸送帶後退.Bool = true;
                    flag_OK = true;
                    cnt++;
                    return;
                }
            }
            catch
            {

            }
            finally
            {
                if (flag_OK) Console.WriteLine($"[出料一次] 輸送帶後退 Done! {DateTime.Now.ToDateTimeString()}");
            }

        }
        #endregion
    }
}
