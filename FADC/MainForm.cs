using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Basic;
using MyUI;
using MinasA6DLL;
using System.Text.Json.Serialization;
using SQLUI;
using H_Pannel_lib;
using HIS_DB_Lib;
using FpMatchLib;
using SpeechRecognitionUserControl;
using System.Text;
using System.Threading.Tasks;


[assembly: AssemblyVersion("1.0.0.0000")]
[assembly: AssemblyFileVersion("1.0.0.0000")]
namespace FADC
{
    public partial class Main_Form : Form
    {
        public static StorageUI_EPD_266 _storageUI_EPD_266 = null;
        public static RFID_UI _rfiD_UI = null;

        public static string API_Server = "http://127.0.0.1:4433";
        public static string ServerName = "";
        public static string ServerType = "";
        public static string Order_URL = "";
        public static string OrderByCodeApi_URL = "";
        public static RFID_FX600lib.RFID_FX600_UI _RFID_FX600_UI = null;
        public bool ControlMode = false;
        public static MinasA6 minasA6 = null;
        public static string currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private string FormText = "";

        #region MyConfigClass
        private static string MyConfigFileName = $@"{currentDirectory}\MyConfig.txt";
        static public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {

            private bool controlMode = false;
            private string servoZ_Com = "COM1";
            private string board_IP = "";
            private string scanner01_COMPort = "COM2";
            private string rFID_COMPort = "COM3";

            public bool ControlMode { get => controlMode; set => controlMode = value; }
            public string ServoZ_Com { get => servoZ_Com; set => servoZ_Com = value; }
            public string Board_IP { get => board_IP; set => board_IP = value; }
            public string Scanner01_COMPort { get => scanner01_COMPort; set => scanner01_COMPort = value; }
            public string RFID_COMPort { get => rFID_COMPort; set => rFID_COMPort = value; }
        }
        private void LoadMyConfig()
        {
            string jsonstr = MyFileStream.LoadFileAllText($"{MyConfigFileName}");
            if (jsonstr.StringIsEmpty())
            {
                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(new MyConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                }
                MyMessageBox.ShowDialog($"未建立參數文件!請至子目錄設定{MyConfigFileName}");
                Application.Exit();
            }
            else
            {
                myConfigClass = Basic.Net.JsonDeserializet<MyConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(myConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                }

            }

        }

        #endregion
        #region DBConfigClass
        private static string DBConfigFileName = $@"{currentDirectory}\DBConfig.txt";
        static public DBConfigClass dBConfigClass = new DBConfigClass();
        public class DBConfigClass
        {

            public string Name { get => name; set => name = value; }
            public string Api_Server { get => api_Server; set => api_Server = value; }

            private SQL_DataGridView.ConnentionClass dB_Basic = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_person_page = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_order_list = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_tradding = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_Medicine_Cloud = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_storage = new SQL_DataGridView.ConnentionClass();

            private string web_URL = "";
            private string api_URL = "";
            private string login_URL = "";
            private string name = "";
            private string api_Server = "";

            private string orderApiURL = "";
            private string order_mrn_ApiURL = "";
            private string order_bag_num_ApiURL = "";
            private string order_upload_ApiURL = "";
            private string orderByCodeApiURL = "";
            private string medApiURL = "";
            private string med_Update_ApiURL = "";
            private string med_Sort = "";
            private string storage_Sort = "";

            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_Basic { get => dB_Basic; set => dB_Basic = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_person_page { get => dB_person_page; set => dB_person_page = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_order_list { get => dB_order_list; set => dB_order_list = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_Medicine_Cloud { get => dB_Medicine_Cloud; set => dB_Medicine_Cloud = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_tradding { get => dB_tradding; set => dB_tradding = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_storage { get => dB_storage; set => dB_storage = value; }

            [JsonIgnore]
            public string OrderApiURL { get => orderApiURL; set => orderApiURL = value; }
            [JsonIgnore]
            public string MedApiURL { get => medApiURL; set => medApiURL = value; }
            [JsonIgnore]
            public string Api_URL { get => api_URL; set => api_URL = value; }
            [JsonIgnore]
            public string Web_URL { get => web_URL; set => web_URL = value; }
            [JsonIgnore]
            public string Login_URL { get => login_URL; set => login_URL = value; }
            [JsonIgnore]
            public string Med_Update_ApiURL { get => med_Update_ApiURL; set => med_Update_ApiURL = value; }
            [JsonIgnore]
            public string OrderByCodeApiURL { get => orderByCodeApiURL; set => orderByCodeApiURL = value; }
            [JsonIgnore]
            public string Order_upload_ApiURL { get => order_upload_ApiURL; set => order_upload_ApiURL = value; }
            [JsonIgnore]
            public string Order_mrn_ApiURL { get => order_mrn_ApiURL; set => order_mrn_ApiURL = value; }
            [JsonIgnore]
            public string Order_bag_num_ApiURL { get => order_bag_num_ApiURL; set => order_bag_num_ApiURL = value; }
            [JsonIgnore]
            public string Med_Sort { get => med_Sort; set => med_Sort = value; }
            [JsonIgnore]
            public string Storage_Sort { get => storage_Sort; set => storage_Sort = value; }
        }

        private void LoadDBConfig()
        {

            this.LoadcommandLineArgs();
            string jsonstr = MyFileStream.LoadFileAllText($"{DBConfigFileName}");
            if (jsonstr.StringIsEmpty())
            {

                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(new DBConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{DBConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{DBConfigFileName}檔案失敗!");
                }
                MyMessageBox.ShowDialog($"未建立參數文件!請至子目錄設定{DBConfigFileName}");
                Application.Exit();
            }
            else
            {
                dBConfigClass = Basic.Net.JsonDeserializet<DBConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(dBConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{DBConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{DBConfigFileName}檔案失敗!");
                }

            }
        }
        #endregion

        public Main_Form()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            this.Load += MainFrom_Load;
            this.Resize += Main_Form_Resize;
        }

        private void Main_Form_Resize(object sender, EventArgs e)
        {
            //rJ_Pannel_登入卡片.Location = new System.Drawing.Point((tabPage_登入畫面.Width - rJ_Pannel_登入卡片.Width) / 2, (tabPage_登入畫面.Height - rJ_Pannel_登入卡片.Height) / 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // 使用雙重緩衝
            BufferedGraphicsContext currentContext;
            BufferedGraphics myBuffer;

            currentContext = BufferedGraphicsManager.Current;
            myBuffer = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);

            // 在緩衝區域進行繪製
            Graphics g = myBuffer.Graphics;
            g.Clear(this.BackColor); // 清除背景
            base.OnPaint(new PaintEventArgs(g, this.ClientRectangle));

            // 將緩衝區域的內容繪製到表單
            myBuffer.Render(e.Graphics);
            myBuffer.Dispose(); // 釋放緩衝區資源

            rJ_Pannel_登入卡片.Location = new System.Drawing.Point((登入畫面.Width - rJ_Pannel_登入卡片.Width) / 2, (登入畫面.Height - rJ_Pannel_登入卡片.Height) / 2);

        }
        private void MainFrom_Load(object sender, EventArgs e)
        {
            H_Pannel_lib.Communication.ConsoleWrite = false;
            MyMessageBox.音效 = false;
            MyMessageBox.form = this.FindForm();
            LoadingForm.form = this.FindForm();
            Net.DebugLog = false;
            this.lowerMachine_Panel.Run();

            this.plC_UI_Init.音效 = false;
            this.plC_UI_Init.全螢幕顯示 = false;

            this.plC_UI_Init.UI_Finished_Event += PlC_UI_Init_UI_Finished_Event;
            this.plC_UI_Init.Run(this.FindForm(), this.lowerMachine_Panel);
            LoadDBConfig();
            LoadMyConfig();
        }

        private void PlC_UI_Init_UI_Finished_Event()
        {
            
            this.WindowState = FormWindowState.Maximized;
            PLC_UI_Init.Set_PLC_ScreenPage(panel_Main, this.plC_ScreenPage_Main);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_setting, this.plC_ScreenPage_setting);

            speechRecognitionUserControl.Init();
            speechRecognitionUserControl.OnRecognized += SpeechRecognitionUserControl_OnRecognized;
            LoadMyConfig();
            LoadDBConfig();
            ApiServerSetting();
            
            RFID_Iint();

            Program_faceReconition_Init();
            Program_storageMedBoxIOConfig_Init();
            Program_Scanner_RS232_Init();
            Program_fingerMatch_Init();
            Program_人員資料_Init();
            Program_後台登入_Init();
            Program_交易紀錄查詢_Init();   
            Program_儲位管理_Init();
            Program_調劑作業_Init();
            Program_PLC();

         
        }

        private void SpeechRecognitionUserControl_OnRecognized(SpeechRecognitionDll.Response<SpeechRecognitionDll.Detail> response)
        {
            StringBuilder log = new StringBuilder();

            log.AppendLine($"Success: {response.State}");
            log.AppendLine($"Message: {response.Message}");
            log.AppendLine($"ErrorCode: {response.ErrorCode}");
            log.AppendLine($"Command: {response.Command}");

            var data = response.Data.JsonSerializationt(true);
            log.AppendLine($"Data: {data}");

            Console.WriteLine(log);
        }

        private void RFID_Iint()
        {
            Task.Run(new Action(delegate
            {
                MyTimer MyTimer_rfiD_FX600_UI_Init = new MyTimer();
                bool flag_rfiD_FX600_UI_Init = false;
                MyTimer_rfiD_FX600_UI_Init.TickStop();
                MyTimer_rfiD_FX600_UI_Init.StartTickTime(5000);
                while (true)
                {
                    _RFID_FX600_UI = this.rfiD_FX600_UI;

                    if (MyTimer_rfiD_FX600_UI_Init.IsTimeOut() && !flag_rfiD_FX600_UI_Init)
                    {
                        int num = 1;
                        this.rfiD_FX600_UI.Init(RFID_FX600lib.RFID_FX600_UI.Baudrate._9600, num, myConfigClass.RFID_COMPort);

                        flag_rfiD_FX600_UI_Init = true;
                        break;

                    }
                    System.Threading.Thread.Sleep(100);
                }
            }));
        }
        private void LoadcommandLineArgs()
        {
            string jsonstr = MyFileStream.LoadFileAllText($"{DBConfigFileName}");
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length >= 3)
            {
                dBConfigClass.Api_Server = commandLineArgs[1];
                dBConfigClass.Name = commandLineArgs[2];
                if (commandLineArgs.Length == 4)
                {
                    this.ControlMode = (commandLineArgs[3] == true.ToString());

                }
                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(dBConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{DBConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{DBConfigFileName}檔案失敗!");
                }
                return;
            }
        }
        private void ApiServerSetting()
        {

            if (ControlMode)
            {
                this.ApiServerSetting(dBConfigClass.Name, "一般資料");
            }
            else
            {
                this.ApiServerSetting(dBConfigClass.Name, "一般資料(LAN)");
            }

        }
        private void ApiServerSetting(string Name, string basicName)
        {

            string json_result = Basic.Net.WEBApiGet($"{dBConfigClass.Api_Server}/api/ServerSetting");
            if (json_result.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("API Server 連結失敗!");
                return;
            }
            //Console.WriteLine(json_result);
            returnData returnData = json_result.JsonDeserializet<returnData>();
            List<HIS_DB_Lib.sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();
            HIS_DB_Lib.sys_serverSettingClass sys_serverSettingClass;
            ServerName = Name;
            ServerType = enum_sys_serverSetting_Type.FADC.GetEnumName();
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.FADC, basicName);
            List<string> DPS_Names = (from temp in sys_serverSettingClasses
                                      where temp.類別 == enum_sys_serverSetting_Type.FADC.GetEnumName()
                                      select temp.設備名稱).Distinct().ToList();

            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_Basic.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_Basic.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_Basic.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_Basic.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_Basic.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "人員資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_person_page.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_person_page.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_person_page.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_person_page.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_person_page.Password = sys_serverSettingClass.Password;

            }
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "藥檔資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_Medicine_Cloud.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_Medicine_Cloud.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_Medicine_Cloud.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_Medicine_Cloud.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_Medicine_Cloud.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "醫囑資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_order_list.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_order_list.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_order_list.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_order_list.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_order_list.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "儲位資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_storage.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_storage.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_storage.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_storage.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_storage.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "交易紀錄資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_tradding.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_tradding.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_tradding.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_tradding.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_tradding.Password = sys_serverSettingClass.Password;
            }
            API_Server = dBConfigClass.Api_Server;
            //sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "API02");
            //if (sys_serverSettingClass != null)
            //{
            //    dBConfigClass.Api_URL = sys_serverSettingClass.Server;
            //    API_Server = sys_serverSettingClass.Server;
            //}
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "Order_API");
            if (sys_serverSettingClass != null) dBConfigClass.OrderApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "Order_By_Code_API");
            if (sys_serverSettingClass != null) dBConfigClass.OrderByCodeApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "Order_upload_API");
            if (sys_serverSettingClass != null) dBConfigClass.Order_upload_ApiURL = sys_serverSettingClass.Server;

            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "Order_By_MRN_API");
            if (sys_serverSettingClass != null) dBConfigClass.Order_mrn_ApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "Order_By_BAG_NUM_API");
            if (sys_serverSettingClass != null) dBConfigClass.Order_bag_num_ApiURL = sys_serverSettingClass.Server;

            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "Med_API");
            if (sys_serverSettingClass != null) dBConfigClass.MedApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.myFind(Name, "FADC", "Website");
            if (sys_serverSettingClass != null) dBConfigClass.Web_URL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.myFind("Main", "網頁", "API_Login");
            if (sys_serverSettingClass != null) dBConfigClass.Login_URL = sys_serverSettingClass.Server;


            OrderByCodeApi_URL = dBConfigClass.OrderByCodeApiURL;
            Order_URL = dBConfigClass.OrderApiURL;
            if (OrderByCodeApi_URL.StringIsEmpty())
            {
                OrderByCodeApi_URL = Order_URL;
            }
        }
    }
}
