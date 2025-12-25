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
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using H_Pannel_lib;
namespace 勤務傳送櫃
{

    public partial class Pannel_Box : UserControl
    {
        public static List<Pannel_Box> Panels = new List<Pannel_Box>();
        public static int AlarmTime = 5000;
        public static int AlarmBeepTime = 5000;
        public static int PharLightOnTime = 5000;
        public static int InitDelay = 5000;
        public static bool flag_Run = false;
        public static bool LightCheck = true;
      

        public static List<string> GetAllWardName()
        {
            List<string> list_temp = new List<string>();
            for (int i = 0; i < Panels.Count; i++)
            {
                foreach(string temp in Panels[i].list_serchName)
                {
                    list_temp.Add(temp);
                }
            }
            List<string> names = (from temp in list_temp
                                  select temp).Distinct().ToList();
            return names;
        }
        public static List<Pannel_Box> SerchByWardName(List<string> wardNames)
        {
            List<Pannel_Box> pannel_Boxes = new List<Pannel_Box>();
            for (int i = 0; i < wardNames.Count; i++)
            {
                pannel_Boxes.LockAdd(SerchByWardName(wardNames[i]));
            }
            return pannel_Boxes;
        }
        public static List<Pannel_Box> SerchByWardName(string WardName)
        {
            List<Pannel_Box> pannel_Boxes = new List<Pannel_Box>();
            for (int i = 0; i < Panels.Count; i++)
            {
                foreach (string temp in Panels[i].list_serchName)
                {
                    if (temp.StringIsEmpty()) continue;
                    if (temp.ToUpper() == WardName.ToUpper())
                    {
                        pannel_Boxes.Add(Panels[i]);
                        break;
                    }
                }
            }
            return pannel_Boxes;
        }

        public static bool PharLightOn(string WardName)
        {
            List<Pannel_Box> pannel_Boxes = SerchByWardName(WardName);
            bool flag = false;
            for(int i = 0; i < pannel_Boxes.Count; i++)
            {
                pannel_Boxes[i].PharmacyLightOn();
                flag = true;

            }
            return flag;
        }
        public static void PanelLightOnCheck(List<string> wardNames)
        {
            if (LightCheck == false) return ;
            for (int i = 0; i < Panels.Count; i++)
            {
                if (Panels[i].list_serchName.Count == 0) continue;
                if(Panels[i].CheckWardName(wardNames))
                {
                    Panels[i].PanelLightOn();
                }
                else
                {
                    Panels[i].PanelLightOff();
                }
            }
        }
        public static void H_COST_LightOnCheck(List<string> wardNames)
        {
            if (LightCheck == false) return;
            for (int i = 0; i < Panels.Count; i++)
            {
                if (Panels[i].list_serchName.Count == 0) continue;
                if (Panels[i].CheckWardName(wardNames))
                {
                    Panels[i].H_COST_LightOn();
                }
                else
                {
                    Panels[i].H_COST_LightOff();
                }
            }
        }

        public delegate void EPDSettingEventHandler(string EPD_IP , string Name);
        public event EPDSettingEventHandler EPDSettingEvent;
        public delegate void PharmacyLightEventHandler(string EPD_IP, string Name);
        public event PharmacyLightEventHandler PharmacyLightEvent;


        public delegate void InSideBoxOnEventHandler(Pannel_Box pannel_Box);
        public delegate void AlarmEventHandler(Pannel_Box pannel_Box);
        public delegate void CloseEventHandler(Pannel_Box pannel_Box);
        public delegate void OpenEventHandler(Pannel_Box pannel_Box);
        public event AlarmEventHandler AlarmEvent;
        public event CloseEventHandler CloseEvent;
        public event OpenEventHandler OpenEvent;
        public event InSideBoxOnEventHandler InSideBoxOnEvent;

        public bool AlarmBeep = false;

        private RFID_UI rFID_UI;
        private H_Pannel_lib.StorageUI_EPD_266 storageUI_EPD_266;
        static public Color H_COST_Color = Color.Red;
        private Color _H_COST_Color = Color.Gray;

        private PLC_Device pLC_Device_sensor_input = new PLC_Device();
        private PLC_Device pLC_Device_lock_input = new PLC_Device();
        private PLC_Device pLC_Device_output = new PLC_Device();
        private PLC_Device pLC_Device_LED_State = new PLC_Device();

        private int cnt_alarm_led_blink_time = 0;
        private MyTimer myTimer_InitDelay = new MyTimer();
        private MyTimer myTimer_InitinputDelay = new MyTimer();
        private MyTimer myTimer_alarm_time = new MyTimer();
        private MyTimer myTimer_sensor_input_time = new MyTimer();
        private MyTimer myTimer_opendoor_delayTime = new MyTimer();
        private MyTimer myTimer_alarm_led_blink_time = new MyTimer();
        private MyTimer myTimer_alarm_beep_time = new MyTimer();
        private MyTimer myTimer_pharlightOn_time = new MyTimer();
        private bool flag_pharlightOn_time = false;
        private int rFID_num = -1;
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public int RFID_num
        {
            get
            {
                return rFID_num;
            }
            set
            {
                rFID_num = value;
            }
        }
        private int lock_output_num = -1;
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public int Lock_output_num
        {
            get
            {
                return lock_output_num;
            }
            set
            {
                lock_output_num = value;
            }
        }
        private int lock_input_num = -1;
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public int Lock_input_num
        {
            get
            {
                return lock_input_num;
            }
            set
            {
                lock_input_num = value;
            }
        }
        private int sensor_input_num = -1;
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public int Sensor_input_num
        {
            get
            {
                return sensor_input_num;
            }
            set
            {
                sensor_input_num = value;
            }
        }
        private int led_output_num = -1;
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public int Led_output_num
        {
            get
            {
                return led_output_num;
            }
            set
            {
                led_output_num = value;
            }
        }

        private bool sQL_Write = false;
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public bool SQL_Write
        {
            get
            {
                return this.sQL_Write;
            }
            set
            {
                if (this.sQL_Write != value)
                {
                    this.sQL_Write = value;

                }
            }
        }
        private string iP = "";
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public string IP
        {
            get
            {
                return iP;
            }
            set
            {
                iP = value;
            }
        }
        private int port = 0;
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }

        private string number = "";
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public string Number
        {
            get
            {
                return this.number;
            }
            set
            {
                if (this.number != value)
                {
                    this.number = value;
                    this.label_編號.Text = value;

                }
            }
        }
        private string wardName = "";
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public string WardName
        {
            get
            {
                return this.wardName;
            }
            set
            {
                if (this.wardName != value)
                {
                    this.wardName = value;
                    this.label_病房名稱.Text = value;
                }        
            }
        }
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public Font WardFont
        {
            get
            {
                return this.label_病房名稱.Font;
            }
            set
            {
                if (this.label_病房名稱.Font != value)
                {
                    this.label_病房名稱.Font = value;
                }
            }
        }

        private bool mVisible = true;
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public bool MVisible
        {
            get
            {
                return this.mVisible;
            }
            set
            {
                this.Visible = value;
            }
        }

        private string ePD_IP = "";
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public string EPD_IP
        {
            get
            {
                return ePD_IP;
            }
            set
            {
                ePD_IP = value;
            }
        }

        private string cT_Name = "";
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public string CT_Name
        {
            get
            {
                return cT_Name;
            }
            set
            {
                cT_Name = value;
            }
        }

        private List<string> list_serchName = new List<string>();
        [ReadOnly(false), Browsable(false), Category("Config"), Description(""), DefaultValue("")]
        public List<string> List_serchName
        {
            get
            {
                return list_serchName;
            }
            set
            {
                list_serchName = value;
            }
        }
        public string serchName
        {
            set
            {
                List<string> temp = value.JsonDeserializet<List<string>>();
                if (temp == null) temp = new List<string>();
                list_serchName = temp;
            }
        }


        public enum enum_door_state
        {
            Close,
            Open,
            Alarm,
            None,
        }
        private string _Sensor_Input_Adress = "";
        public string Sensor_Input_Adress
        {
            get
            {
                return this._Sensor_Input_Adress;
            }
            set
            {
                this._Sensor_Input_Adress = value;
            }
        }

        private string _Input_Adress = "";
        public string Input_Adress
        {
            get
            {
                return this._Input_Adress;
            }
            set
            {
                this._Input_Adress = value;
            }
        }
        private string _Output_Adress = "";
        public string Output_Adress
        {
            get
            {
                return this._Output_Adress;
            }
            set
            {
                this._Output_Adress = value;
            }
        }
        private string _Alarm_Time_Adress = "";
        public string Alarm_Time_Adress
        {
            get
            {
                return this._Alarm_Time_Adress;
            }
            set
            {
                this._Alarm_Time_Adress = value;
            }
        }
        private string _LED_Adress = "";
        public string LED_Adress
        {
            get
            {
                return this._LED_Adress;
            }
            set
            {
                this._LED_Adress = value;
            }
        }
        private string _LED_State_Adress = "";
        public string LED_State_Adress
        {
            get
            {
                return this._LED_State_Adress;
            }
            set
            {
                this._LED_State_Adress = value;
            }
        }
        private enum_door_state _enum_door_state = enum_door_state.None;
        public enum_door_state enum_Door_State
        {
            get
            {
                return this._enum_door_state;
            }
            set
            {
                if (this.IsHandleCreated)
                {

                    if (value != this._enum_door_state)
                    {
                        this.Invoke(new Action(delegate
                        {
                            this._enum_door_state = value;
                            if (value == enum_door_state.Open)
                            {
                                this.label_病房名稱.BackColor = Color.Yellow;
                            }
                            else if (value == enum_door_state.Close)
                            {
                                this.label_病房名稱.BackColor = Color.Pink;
                                cnt_alarm_led_blink_time = 0;
                            }
                            else if (value == enum_door_state.Alarm)
                            {
                                this.label_病房名稱.BackColor = Color.Red;
                                cnt_alarm_led_blink_time = 0;
                            }
                        }));
                    }

                }
                else
                {
                    if (value != this._enum_door_state)
                    {

                        this.FindForm().Invoke(new Action(delegate
                        {
                            this._enum_door_state = value;
                            if (value == enum_door_state.Open)
                            {
                                this.label_病房名稱.BackColor = Color.Yellow;
                            }
                            else if (value == enum_door_state.Close)
                            {
                                this.label_病房名稱.BackColor = Color.Pink;
                            }
                            else if (value == enum_door_state.Alarm)
                            {
                                this.label_病房名稱.BackColor = Color.Red;
                                cnt_alarm_led_blink_time = 0;
                            }
                        }));

                    }
                }
       
                
            }
        }

        public void Init(int number , string input_Adress , string output_Adress, string alarm_time_Adress, string LED_Adress , string LED_State_Adress ,string Sensor_Input_Adress)
        {
            this.Number = number.ToString();
            this.Input_Adress = input_Adress;
            this.Output_Adress = output_Adress;
            this.Alarm_Time_Adress = alarm_time_Adress;
            this.LED_Adress = LED_Adress;
            this.LED_State_Adress = LED_State_Adress;
            this.Sensor_Input_Adress = Sensor_Input_Adress;
            this.pLC_Device_lock_input = new PLC_Device(this.Input_Adress);
            this.pLC_Device_output = new PLC_Device(this.Output_Adress);
            this.pLC_Device_LED_State = new PLC_Device(this.LED_State_Adress);
            this.pLC_Device_sensor_input = new PLC_Device(this.Sensor_Input_Adress);
            this.myTimer_alarm_time = new MyTimer(this.Alarm_Time_Adress);
            this.myTimer_sensor_input_time = new MyTimer();
        }
        public void Init(int number ,RFID_UI rFID_UI, H_Pannel_lib.StorageUI_EPD_266 _storageUI_EPD_266)
        {
            this.Number = number.ToString();
            this.rFID_UI = rFID_UI;
            this.storageUI_EPD_266 = _storageUI_EPD_266;
        }
        public static int input_time = 3000;
        private bool flag_alarm_is_active = true;
        private bool flag_Init = false;
        private bool flag_LED_State = false;
        private bool flag_sensor_State = false;
        private bool flag_lockinput_State = false;
        private bool flag_lock_input = false;
        private bool flag_lock_output = false;
        private bool flag_outSideDoorOpened = false;

        private MyTimerBasic MyTimerBasic_lock_input = new MyTimerBasic(200);
        private MyTimerBasic MyTimerBasic_outSideBoxPutinDelay = new MyTimerBasic(3000);
        private MyTimerBasic MyTimerBasic_outSideBoxPutinInitDelay = new MyTimerBasic(3000);
        private MyTimerBasic MyTimerBasic_input = new MyTimerBasic(input_time);
        public void Run()
        {
            if (!flag_Run) return;
            if(this.pLC_Device_lock_input.Bool)
            {
                if (MyTimerBasic_lock_input.IsTimeOut())
                {
                    flag_lock_input = true;
                }
            }
            else
            {
                MyTimerBasic_lock_input.TickStop();
                MyTimerBasic_lock_input.StartTickTime(200);
                flag_lock_input = false;
            }
            if (this.MVisible == false)
            {
                return;
            }
            string jsonstring = this.rFID_UI.GetUDPJsonString(IP);
            if (jsonstring.StringIsEmpty())
            {
                return;
            }
            if (!flag_Init)
            {
                this.myTimer_alarm_time.StartTickTime(AlarmTime);
                this.myTimer_InitDelay.StartTickTime(InitDelay);
                this.myTimer_InitinputDelay.StartTickTime(200000);
                MyTimerBasic_outSideBoxPutinInitDelay.StartTickTime(200000);
                if (this.myTimer_InitDelay.IsTimeOut())
                {
                    PharmacyLightOff();
                    this.myTimer_InitinputDelay.TickStop();
                    this.myTimer_InitinputDelay.StartTickTime(2000);
                    MyTimerBasic_outSideBoxPutinInitDelay.TickStop();
                    MyTimerBasic_outSideBoxPutinInitDelay.StartTickTime(3000);
                    flag_Init = true;
                }
                return;
            }
            else
            {
                myTimer_opendoor_delayTime.StartTickTime(500);
                if (myTimer_pharlightOn_time.IsTimeOut() && flag_pharlightOn_time)
                {
                    if (LightCheck == true) PharmacyLightOff();
                    flag_pharlightOn_time = false;
                }
            }
            if (flag_lock_output == true)
            {
                this.rFID_UI.Set_OutputPINTrigger(IP, Port, this.lock_output_num, true);
                this.OpenEvent(this);
           
                flag_lock_output = false;
            }

            this.pLC_Device_LED_State.Bool = this.rFID_UI.GetOutput(this.IP, this.led_output_num);
            flag_lock_input = this.rFID_UI.GetInput(this.IP, this.lock_input_num);
            bool flag_input = this.rFID_UI.GetInput(this.IP, this.sensor_input_num);
 
            if(flag_input)
            {
                if(MyTimerBasic_input.IsTimeOut())
                {
                    if (flag_input != this.pLC_Device_sensor_input.Bool)
                    {
                        if(MyTimerBasic_outSideBoxPutinDelay.IsTimeOut()) flag_outSideDoorOpened = false;

                        if (InSideBoxOnEvent != null) InSideBoxOnEvent(this);
                    }
                    this.pLC_Device_sensor_input.Bool = true;

                   
                }
            }
            else
            {
                this.pLC_Device_sensor_input.Bool = false;
                MyTimerBasic_input.TickStop();
                MyTimerBasic_input.StartTickTime(input_time);
            }

            if (this.pLC_Device_LED_State.Bool != this.flag_LED_State)
            {
                this.flag_LED_State = this.pLC_Device_LED_State.Bool;
             
                this.Invoke(new Action(delegate
                {
                    if (this.flag_LED_State)
                    {
                        this.label_編號.BackColor = Color.Lime;
                    }
                    else
                    {
                        this.label_編號.BackColor = Color.White;
                    }
                }));
               
            }

            if (this.pLC_Device_sensor_input.Bool != this.flag_sensor_State)
            {
                this.flag_sensor_State = this.pLC_Device_sensor_input.Bool;
                if (this.flag_sensor_State)
                {
                    bool flag = this.rFID_UI.GetOutput(this.IP, this.Led_output_num);
                    if (!flag) this.rFID_UI.Set_OutputPIN(IP, Port, this.Led_output_num, true);
                    if (LightCheck == false) PharmacyLightOn();
                    this.Invoke(new Action(delegate
                    {
                        this.label_sensor.BackColor = Color.Lime;
                    }));
                }
                else
                {
                    this.Invoke(new Action(delegate
                    {
                        this.label_sensor.BackColor = Color.White;
                    }));
                }
            }

            if(flag_lock_input != flag_lockinput_State)
            {
                this.flag_lockinput_State = flag_lock_input;
                if(LightCheck == false)
                {
                    if (this.flag_lockinput_State)
                    {
                        bool flag = this.rFID_UI.GetOutput(this.IP, this.Led_output_num);
                        if (flag) this.rFID_UI.Set_OutputPIN(IP, Port, this.Led_output_num, false);
                        if (LightCheck == false) PharmacyLightOff();
                    }
                    else
                    {
                        bool flag = this.rFID_UI.GetOutput(this.IP, this.Led_output_num);
                        if (!flag) this.rFID_UI.Set_OutputPIN(IP, Port, this.Led_output_num, true);
                        if (LightCheck == false) PharmacyLightOn();

                    }
                }
           
              
            }

            if (!flag_lock_input)
            {
                if(_enum_door_state != enum_door_state.Alarm)
                {
                    this.enum_Door_State = enum_door_state.Open;
                }
                
            }
            else
            {
                if(this.enum_Door_State != enum_door_state.Close)
                {
                    if (this.CloseEvent != null && flag_Init)
                    {
                        if (this.myTimer_InitinputDelay.IsTimeOut())
                        {
                            if (this.pLC_Device_sensor_input.Bool)
                            {
                                Task.Run(new Action(delegate
                                {
                                    using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{Main_Form.currentDirectory}\放回籃聲音.wav"))
                                    {
                                        sp.Stop();
                                        sp.Play();
                                        sp.PlaySync();
                                    }

                                }));
                                Console.WriteLine($"[CloseEvent]  關門且有籃子開始撥放音樂 {DateTime.Now.ToDateTimeString_6()}");
                                if (MyTimerBasic_outSideBoxPutinInitDelay.IsTimeOut())
                                {
                                    if (Main_Form.PLC_Device_勤務關門閃燈提醒.Bool)
                                    {
                                        MyTimerBasic_outSideBoxPutinDelay.TickStop();
                                        MyTimerBasic_outSideBoxPutinDelay.StartTickTime(3000);
                                        flag_outSideDoorOpened = true;
                                    }
                                }
                            }
                            this.CloseEvent(this);
                            Console.WriteLine($"[CloseEvent]  WardName:{WardName},Number{Number} { DateTime.Now.ToDateTimeString_6()}");
                        }
  
                    }
                }
                this.enum_Door_State = enum_door_state.Close;
            }

            if (this.enum_Door_State == enum_door_state.Open)
            {
              
                if (this.myTimer_alarm_time.IsTimeOut())
                {
                    if(flag_alarm_is_active)
                    {
                        enum_Door_State = enum_door_state.Alarm;
                        this.myTimer_alarm_beep_time.TickStop();
                        this.myTimer_alarm_beep_time.StartTickTime(AlarmBeepTime);

                        if (this.AlarmEvent != null && flag_Init) this.AlarmEvent(this);

                        flag_alarm_is_active = false;
                    }
              
                }
            }
            else if (this.enum_Door_State == enum_door_state.Close)
            {
                if (pLC_Device_sensor_input.Bool)
                {
                    if (myTimer_sensor_input_time.IsTimeOut())
                    {
                        this.myTimer_sensor_input_time.TickStop();
                        this.myTimer_sensor_input_time.StartTickTime(200);
                    }
                }
                else
                {
                    this.myTimer_sensor_input_time.TickStop();
                    this.myTimer_sensor_input_time.StartTickTime(200);
                }

                this.myTimer_alarm_time.TickStop();
                this.myTimer_alarm_time.StartTickTime(AlarmTime);
                if(flag_outSideDoorOpened)
                {
                    program_alarmLight(1000);
                }
                flag_alarm_is_active = true;
            }
            else if (this.enum_Door_State == enum_door_state.Alarm)
            {
                program_alarmLight(500);


            }
            if (this.enum_Door_State == enum_door_state.Alarm && !this.myTimer_alarm_beep_time.IsTimeOut())
            {
                AlarmBeep = true;
            }
            else
            {
                AlarmBeep = false;
            }
            flag_Init = true;

        }
        private void program_alarmLight(int time)
        {
            if (cnt_alarm_led_blink_time == 0)
            {
                this.rFID_UI.Set_OutputPIN(IP, Port, this.Led_output_num, true);
                this.myTimer_alarm_led_blink_time.TickStop();
                this.myTimer_alarm_led_blink_time.StartTickTime(time);
                cnt_alarm_led_blink_time++;
            }
            if (cnt_alarm_led_blink_time == 1)
            {
                if (this.myTimer_alarm_led_blink_time.IsTimeOut())
                {
                    this.rFID_UI.Set_OutputPIN(IP, Port, this.Led_output_num, false);
                    this.myTimer_alarm_led_blink_time.TickStop();
                    this.myTimer_alarm_led_blink_time.StartTickTime(time);
                    cnt_alarm_led_blink_time++;
                }
            }
            if (cnt_alarm_led_blink_time == 2)
            {
                if (this.myTimer_alarm_led_blink_time.IsTimeOut())
                {
                    cnt_alarm_led_blink_time++;
                }
            }
            if (cnt_alarm_led_blink_time == 3)
            {
                cnt_alarm_led_blink_time = 0;
            }
        }
        public void Open()
        {
            if (myTimer_opendoor_delayTime.IsTimeOut())
            {
                flag_lock_output = true;
                myTimer_opendoor_delayTime.TickStop();
                myTimer_opendoor_delayTime.StartTickTime(500);
            }
    
        }
        public bool IsOpen()
        {
            if (!flag_lock_input) return true;
            return false;
        }
        public void PharmacyLightOff()
        {
            Storage storage = this.storageUI_EPD_266.SQL_GetStorage(EPD_IP);
            if (storage == null)
            {
                Console.WriteLine($"找無[{EPD_IP}]內容,無法控制開關燈!");
                return;
            }
            this.storageUI_EPD_266.Set_OutputPIN(storage, false);
        }
        public void PharmacyLightOn()
        {
            Storage storage = this.storageUI_EPD_266.SQL_GetStorage(EPD_IP);
            if (storage == null)
            {
                Console.WriteLine($"找無[{EPD_IP}]內容,無法控制開關燈!");
                return;
            }
            this.storageUI_EPD_266.Set_OutputPIN(storage, true);
            flag_pharlightOn_time = true;
            myTimer_pharlightOn_time.StartTickTime(PharLightOnTime);
        }
        public void PharmacyLightToggle()
        {
            Storage storage = this.storageUI_EPD_266.SQL_GetStorage(EPD_IP);
            if (storage == null)
            {
                Console.WriteLine($"找無[{EPD_IP}]內容,無法控制開關燈!");
                return;
            }
            bool flag = this.storageUI_EPD_266.Get_OutputPIN(storage);
            if (flag) PharmacyLightOff();
            else PharmacyLightOn();
        }
        public void PanelLightOff()
        {
            bool flag = this.rFID_UI.GetOutput(this.IP, this.Led_output_num);
            if (flag)
            {
                this.rFID_UI.Set_OutputPIN(IP, Port, this.Led_output_num, false);
                Console.WriteLine($"\n{WardName} : 外面板亮燈 {DateTime.Now.ToDateTimeString()}");
            }
        }
        public void PanelLightOn()
        {
            bool flag = this.rFID_UI.GetOutput(this.IP, this.Led_output_num);
            if (!flag)
            {
                this.rFID_UI.Set_OutputPIN(IP, Port, this.Led_output_num, true);
                Console.WriteLine($"\n{WardName} : 外面板滅燈 {DateTime.Now.ToDateTimeString()}");
            }
        }
        public void H_COST_LightOff()
        {
            if(this._H_COST_Color.ToColorString() != Color.Black.ToColorString())
            {
                this._H_COST_Color = Color.Black;
                Storage storage = this.storageUI_EPD_266.SQL_GetStorage(EPD_IP);
                if (storage == null)
                {
                    Console.WriteLine($"找無[{EPD_IP}]內容,無法控制外高價藥燈!");
                    return;
                }
                this.storageUI_EPD_266.Set_Stroage_LED_UDP(storage, Color.Black);
            
                Console.WriteLine($"\n{WardName} : 高價藥品亮燈 {DateTime.Now.ToDateTimeString()}");
            }
        }
        public void H_COST_LightOn()
        {
            if (this._H_COST_Color.ToColorString() != H_COST_Color.ToColorString())
            {
                this._H_COST_Color = H_COST_Color;
                Storage storage = this.storageUI_EPD_266.SQL_GetStorage(EPD_IP);
                if (storage == null)
                {
                    Console.WriteLine($"找無[{EPD_IP}]內容,無法控制外高價藥燈!");
                    return;
                }
                this.storageUI_EPD_266.Set_Stroage_LED_UDP(storage, H_COST_Color);
         
                Console.WriteLine($"\n{WardName} : 高價藥品滅燈 {DateTime.Now.ToDateTimeString()}");
            }
        }

        public bool CheckWardName(string WardName)
        {
            if (LightCheck == false) return true;
            for (int i = 0; i < this.list_serchName.Count; i++)
            {
                if (WardName.ToUpper() == this.list_serchName[i].ToUpper())
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckWardName(List<string> WardNames)
        {
            if (LightCheck == false) return true;
            for (int i = 0; i < WardNames.Count; i++)
            {
                if (this.CheckWardName(WardNames[i])) return true;
            }
            return false;
        }
        public Pannel_Box()
        {
            InitializeComponent();
            this.list_serchName = list_serchName;
        }

        private void 開門ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Open();
        }
        private void 更改病房名稱ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialog_更改病房名稱 form = new Dialog_更改病房名稱(this.WardName , this.list_serchName);
            form.ShowDialog();
            if(form.Enum_Type == Dialog_更改病房名稱.enum_Type.OK)
            {
                RFIDClass rFIDClass = this.rFID_UI.SQL_GetRFIDClass(this.IP);
                if (rFIDClass == null) return;
                if (this.rFID_num >= rFIDClass.DeviceClasses.Length) return;
                rFIDClass.DeviceClasses[this.rFID_num].Name = form.修改名稱;
                rFIDClass.DeviceClasses[this.rFID_num].WardName = form.病房名稱.JsonSerializationt();
                this.rFID_UI.SQL_ReplaceRFIDClass(rFIDClass);
                this.WardName = form.修改名稱;
               
            }

        }
        private void 更改字體ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            RFIDClass rFIDClass = this.rFID_UI.SQL_GetRFIDClass(this.IP);
            if (rFIDClass == null) return;
            if (this.rFID_num >= rFIDClass.DeviceClasses.Length) return;
            this.fontDialog.Font = rFIDClass.DeviceClasses[this.rFID_num].Name_font;

            if (this.fontDialog.ShowDialog() == DialogResult.OK)
            {
                rFIDClass.DeviceClasses[this.rFID_num].Name_font = this.fontDialog.Font;
                this.rFID_UI.SQL_ReplaceRFIDClass(rFIDClass);
                this.WardFont = this.fontDialog.Font;
            }
        }
        private void label_病房名稱_Click(object sender, EventArgs e)
        {
            bool flag = this.rFID_UI.GetOutput(this.IP, this.Led_output_num);
            this.rFID_UI.Set_OutputPIN(IP, Port, this.Led_output_num, !flag);

            if (flag && (LightCheck == false)) PharmacyLightOff();
            if (!flag && (LightCheck == false)) PharmacyLightOn();
        }
        private void 電子紙設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EPDSettingEvent != null) EPDSettingEvent(EPD_IP , WardName);
        }
        private void 藥局內燈開關ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PharmacyLightEvent != null) PharmacyLightEvent(EPD_IP, WardName);
            PharmacyLightToggle();
        }
    }
    static class Pannel_Box_Method
    {
        static public Pannel_Box SortByOutput(this List<Pannel_Box> list_Pannel_Box, string Output_Adress)
        {
            List<Pannel_Box> list_Pannel_Box_buf = (from value in list_Pannel_Box
                                                    where value.Output_Adress == Output_Adress
                                                    select value).ToList();
            if (list_Pannel_Box_buf.Count == 0) return null;
            return list_Pannel_Box_buf[0];
        }
        static public Pannel_Box SortByRFID(this List<Pannel_Box> list_Pannel_Box, string IP, int RFID_Num)
        {
            List<Pannel_Box> list_Pannel_Box_buf = (from value in list_Pannel_Box
                                                    where value.IP == IP
                                                    where value.RFID_num == RFID_Num
                                                    select value).ToList();
            if (list_Pannel_Box_buf.Count == 0) return null;
            return list_Pannel_Box_buf[0];
        }
    }
}
