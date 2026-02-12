using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basic;
using MyUI;
namespace 勤務傳送系統
{
    public partial class Main_Form : Form
    {
  
        private void Program_工程模式_Init()
        {
            Pannel_Box.LightCheck = !plC_CheckBox_不檢查處方亮燈.Bool;
            this.plC_UI_Init.Add_Method(Program_蜂鳴警報檢查);
        }
        #region PLC_蜂鳴警報檢查
        PLC_Device PLC_Device_蜂鳴持續時間 = new PLC_Device("D3002");
        PLC_Device PLC_Device_蜂鳴警報檢查 = new PLC_Device("");
        PLC_Device PLC_Device_蜂鳴警報檢查_OK = new PLC_Device("");
        MyTimer MyTimer_蜂鳴警報檢查_結束延遲 = new MyTimer();
        int cnt_Program_蜂鳴警報檢查 = 65534;
        void Program_蜂鳴警報檢查()
        {
            PLC_Device_蜂鳴警報檢查.Bool = true;
            if (cnt_Program_蜂鳴警報檢查 == 65534)
            {
                this.MyTimer_蜂鳴警報檢查_結束延遲.StartTickTime(1000);
                PLC_Device_蜂鳴警報檢查.SetComment("PLC_蜂鳴警報檢查");
                PLC_Device_蜂鳴警報檢查_OK.SetComment("PLC_蜂鳴警報檢查_OK");
                PLC_Device_蜂鳴警報檢查.Bool = false;
                cnt_Program_蜂鳴警報檢查 = 65535;
            }
            if (cnt_Program_蜂鳴警報檢查 == 65535) cnt_Program_蜂鳴警報檢查 = 1;
            if (cnt_Program_蜂鳴警報檢查 == 1) cnt_Program_蜂鳴警報檢查_檢查按下(ref cnt_Program_蜂鳴警報檢查);
            if (cnt_Program_蜂鳴警報檢查 == 2) cnt_Program_蜂鳴警報檢查_初始化(ref cnt_Program_蜂鳴警報檢查);
            if (cnt_Program_蜂鳴警報檢查 == 3) cnt_Program_蜂鳴警報檢查 = 65500;
            if (cnt_Program_蜂鳴警報檢查 > 1) cnt_Program_蜂鳴警報檢查_檢查放開(ref cnt_Program_蜂鳴警報檢查);

            if (cnt_Program_蜂鳴警報檢查 == 65500)
            {
                this.MyTimer_蜂鳴警報檢查_結束延遲.TickStop();
                this.MyTimer_蜂鳴警報檢查_結束延遲.StartTickTime(1000);
                PLC_Device_蜂鳴警報檢查.Bool = false;
                PLC_Device_蜂鳴警報檢查_OK.Bool = false;
                cnt_Program_蜂鳴警報檢查 = 65535;
            }
        }
        void cnt_Program_蜂鳴警報檢查_檢查按下(ref int cnt)
        {
            if (PLC_Device_蜂鳴警報檢查.Bool) cnt++;
        }
        void cnt_Program_蜂鳴警報檢查_檢查放開(ref int cnt)
        {
            if (!PLC_Device_蜂鳴警報檢查.Bool) cnt = 65500;
        }
        void cnt_Program_蜂鳴警報檢查_初始化(ref int cnt)
        {
            if (this.MyTimer_蜂鳴警報檢查_結束延遲.IsTimeOut())
            {
                Pannel_Box.AlarmBeepTime = PLC_Device_蜂鳴持續時間.Value;
                for (int i = 0; i < Pannel_Box.Panels.Count; i++)
                {
                    if (Pannel_Box.Panels[i].AlarmBeep)
                    {
                        Voice voice = new Voice();
                        voice.SpeakOnTask("門未關閉");
                        break;
                    }
                }
             
                cnt++;
            }
        }
        #endregion      
        #region Event

        #endregion
    }
}
