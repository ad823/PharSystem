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
using System.Reflection;
using MyUI;
using Basic;
using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using SQLUI;
using H_Pannel_lib;
using HIS_DB_Lib;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

[assembly: AssemblyVersion("1.0.26.05181")]
[assembly: AssemblyFileVersion("1.0.26.05181")]
namespace 勤務傳送系統
{
    public partial class Main_Form : Form
    {
        public static string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static MySerialPort MySerialPort_Scanner01 = new MySerialPort();
        static MySerialPort MySerialPort_Scanner02 = new MySerialPort();
        private Voice voice = new Voice();
        private string FormText = "";
        private LadderConnection.LowerMachine PLC;
        MyTimer MyTimer_TickTime = new MyTimer();
        private Stopwatch stopwatch = new Stopwatch();
        Basic.MyConvert myConvert = new Basic.MyConvert();
        static public string API_Server = "http://127.0.0.1:4433";
        static public string ServerName = "";
        static public string ServerType = "";
        #region DBConfigClass
        private static string DBConfigFileName = $@"{currentDirectory}\DBConfig.txt";
        public DBConfigClass dBConfigClass = new DBConfigClass();
        public class DBConfigClass
        {
            private SQL_DataGridView.ConnentionClass dB_Basic = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_person_page = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_Medicine_Cloud = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_tradding = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_order = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_storage = new SQL_DataGridView.ConnentionClass();

            private string web_URL = "";
            private string api_URL = "";
            private string orderApiURL = "";
            private string medApiURL = "";
            private string orderCheckinApiURL = "";
            private string orderTakeOutApiURL = "";
            private string doorOpenApiURL = "";
            private string doorCloseApiURL = "";
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_Basic { get => dB_Basic; set => dB_Basic = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_person_page { get => dB_person_page; set => dB_person_page = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_Medicine_Cloud { get => dB_Medicine_Cloud; set => dB_Medicine_Cloud = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_tradding { get => dB_tradding; set => dB_tradding = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_order { get => dB_order; set => dB_order = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_storage { get => dB_storage; set => dB_storage = value; }

            private string name = "";
            private string api_Server = "";

            public string Name { get => name; set => name = value; }
            public string Api_Server { get => api_Server; set => api_Server = value; }
            [JsonIgnore]
            public string Web_URL { get => web_URL; set => web_URL = value; }
            [JsonIgnore]
            public string Api_URL { get => api_URL; set => api_URL = value; }
            [JsonIgnore]
            public string OrderApiURL { get => orderApiURL; set => orderApiURL = value; }
            [JsonIgnore]
            public string MedApiURL { get => medApiURL; set => medApiURL = value; }
            [JsonIgnore]
            public string OrderCheckinApiURL { get => orderCheckinApiURL; set => orderCheckinApiURL = value; }
            [JsonIgnore]
            public string OrderTakeOutApiURL { get => orderTakeOutApiURL; set => orderTakeOutApiURL = value; }
            [JsonIgnore]
            public string DoorOpenApiURL { get => doorOpenApiURL; set => doorOpenApiURL = value; }
            [JsonIgnore]
            public string DoorCloseApiURL { get => doorCloseApiURL; set => doorCloseApiURL = value; }
        }
        private void LoadDBConfig()
        {
            this.LoadcommandLineArgs();
            string jsonstr = MyFileStream.LoadFileAllText($"{DBConfigFileName}");
            if (jsonstr.StringIsEmpty())
            {
 
                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(new DBConfigClass());
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

                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(dBConfigClass);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{DBConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{DBConfigFileName}檔案失敗!");
                }

            }
        }
        #endregion
        #region MyConfigClass 
        private static string MyConfigFileName = $@"{currentDirectory}\MyConfig.txt";
        public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {

            private string rFID_COMPort = "";
            private string scanner01_COMPort = "COM2";
            private string scanner02_COMPort = "COM3";
            private string resetIP = "192.168.32.240";
            private bool show_login = true;
            private bool rFID卡號轉十進制 = false;
            private bool _配藥核對單處方模式 = true;
            private bool _勤務取藥單處方模式 = true;

            public string RFID_COMPort { get => rFID_COMPort; set => rFID_COMPort = value; }
            public string Scanner01_COMPort { get => scanner01_COMPort; set => scanner01_COMPort = value; }
            public string Scanner02_COMPort { get => scanner02_COMPort; set => scanner02_COMPort = value; }
            public bool Show_login { get => show_login; set => show_login = value; }
            public bool RFID卡號轉十進制 { get => rFID卡號轉十進制; set => rFID卡號轉十進制 = value; }
            public string ResetIP { get => resetIP; set => resetIP = value; }
            public bool 配藥核對單處方模式 { get => _配藥核對單處方模式; set => _配藥核對單處方模式 = value; }
            public bool 勤務取藥單處方模式 { get => _勤務取藥單處方模式; set => _勤務取藥單處方模式 = value; }
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

           // this.ftp_DounloadUI1.FTP_Server = myConfigClass.FTP_Server;
        }
        private string Function_RFID卡號轉換(string cardNo)
        {
            if (cardNo.StringIsEmpty()) return "";
            cardNo = cardNo.Trim();
            if (myConfigClass.RFID卡號轉十進制 == false) return cardNo;
            if (cardNo.Length < 8) return cardNo;

            string head = cardNo.Substring(0, 8);
            if (head.Any(c => Uri.IsHexDigit(c) == false)) return cardNo;

            try
            {
                long dec = Convert.ToInt64(head, 16);
                return dec.ToString("D10");
            }
            catch
            {
                return cardNo;
            }
        }
        #endregion

        static public PLC_Device PLC_Device_主頁面頁碼 = new PLC_Device("D0");
        static public PLC_Device PLC_Device_開門異常時間 = new PLC_Device("D3000");
        static public PLC_Device PLC_Device_單層格數 = new PLC_Device("D3001");
        static public PLC_Device PLC_Device_單行格數 = new PLC_Device("D3005");
        static public PLC_Device PLC_Device_格子X寬度 = new PLC_Device("D3006");
        static public PLC_Device PLC_Device_格子Y寬度 = new PLC_Device("D3007");
        static public PLC_Device PLC_Device_勤務關門閃燈提醒 = new PLC_Device("S4510");

        public Main_Form()
        {
            InitializeComponent();
        }

  
        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.DesignMode == false)
            {
                LoadingForm.form = this.FindForm();
                MyMessageBox.form = this.FindForm();
                Dialog_修改密碼.form = this.FindForm();
                Dialog_更改病房名稱.form = this.FindForm();
                Dialog_EPDPanel.form = this.FindForm();

                LoadDBConfig();
                LoadMyConfig();
                this.stopwatch.Start();
                this.Text += "Ver" + this.ProductVersion;
                this.FormText = this.Text;
                this.WindowState = FormWindowState.Maximized;
                this.plC_UI_Init.Run(this.FindForm(), this.lowerMachine_Pane);
                this.plC_UI_Init.音效 = false;
                this.plC_UI_Init.全螢幕顯示 = false;
                Basic.MyMessageBox.音效 = false;
                string ProcessName = "WINWORD";//換成想要結束的進程名字
                System.Diagnostics.Process[] MyProcess = System.Diagnostics.Process.GetProcessesByName(ProcessName);
                for (int i = 0; i < MyProcess.Length; i++)
                {
                    MyProcess[i].Kill();
                }

                if (myConfigClass.RFID_COMPort.StringIsEmpty() == false)
                {
                    this.rfiD_FX600_UI.Init(myConfigClass.RFID_COMPort);
                }
                this.textBox_登入畫面_密碼.PassWordChar = true;
                this.plC_UI_Init.UI_Finished_Event += PlC_UI_Init_UI_Finished_Event;
                StorageUI_EPD_266.Get_Storage_bmpChangeEvent += StorageUI_EPD_266_Get_Storage_bmpChangeEvent;

                if (myConfigClass.勤務取藥單處方模式) tabControlEx_勤務取藥.SelectTab("勤務取藥_單處方");
                else tabControlEx_勤務取藥.SelectTab("勤務取藥_全處方");

                if (myConfigClass.配藥核對單處方模式) tabControlEx_配藥核對.SelectTab("配藥核對_單處方");
                else tabControlEx_配藥核對.SelectTab("配藥核對_全處方");

            }
        }
        private Bitmap StorageUI_EPD_266_Get_Storage_bmpChangeEvent(Storage storage)
        {
            Bitmap bitmap = null;
            if (storage.DeviceType == DeviceType.EPD266 || storage.DeviceType == DeviceType.EPD266_lock) bitmap = new Bitmap(296, 152);
            else if (storage.DeviceType == DeviceType.EPD290 || storage.DeviceType == DeviceType.EPD290_lock) bitmap = new Bitmap(296, 128);

            int Pannel_Width = bitmap.Width;
            int Pannel_Height = bitmap.Height;
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                Rectangle rect = new Rectangle(0, 0, Pannel_Width, Pannel_Height);
                int Line_Height = (Pannel_Height / 3) * 2;
                g.FillRectangle(new SolidBrush(storage.BackColor), rect);

                SizeF size_name = g.MeasureString(storage.Name, storage.Name_font, new Size(rect.Width, rect.Height), StringFormat.GenericDefault);
                size_name = new SizeF((int)size_name.Width, (int)size_name.Height);
                g.DrawString(storage.Name, storage.Name_font, new SolidBrush(storage.Name_ForeColor), new RectangleF((rect.Width - size_name.Width) / 2, (rect.Height - size_name.Height) / 2, Pannel_Width, Pannel_Height), StringFormat.GenericDefault);


                string[] ip_array = storage.IP.Split('.');
                SizeF size_IP = new SizeF();
                if (ip_array.Length == 4)
                {
                    string ip = ip_array[2] + "." + ip_array[3];
                    size_IP = TextRenderer.MeasureText(ip, new Font("微軟正黑體", 8, FontStyle.Bold));
                    g.DrawString(ip, new Font("微軟正黑體", 8, FontStyle.Bold), new SolidBrush(storage.Name_ForeColor), (Pannel_Width - size_IP.Width), (Pannel_Height - size_IP.Height));
                }
            }


            return bitmap;
        }
        private void PlC_UI_Init_UI_Finished_Event()
        {
            if (!myConfigClass.Scanner01_COMPort.StringIsEmpty())
            {
                MySerialPort_Scanner01.ConsoleWrite = true;
                MySerialPort_Scanner01.Init(myConfigClass.Scanner01_COMPort, 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
                if (!MySerialPort_Scanner01.IsConnected)
                {
                    MyMessageBox.ShowDialog("掃碼器[01]初始化失敗!");
                }
            }
            if (!myConfigClass.Scanner02_COMPort.StringIsEmpty())
            {
                MySerialPort_Scanner02.ConsoleWrite = true;
                MySerialPort_Scanner02.Init(myConfigClass.Scanner02_COMPort, 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
                if (!MySerialPort_Scanner02.IsConnected)
                {
                    MyMessageBox.ShowDialog("掃碼器[02]初始化失敗!");
                }
            }
            this.ApiServerSetting();

            SQLUI.SQL_DataGridView.SQL_Set_Properties(dBConfigClass.DB_Basic.DataBaseName, dBConfigClass.DB_Basic.UserName, dBConfigClass.DB_Basic.Password, dBConfigClass.DB_Basic.IP, dBConfigClass.DB_Basic.Port, dBConfigClass.DB_Basic.MySqlSslMode, this.FindForm());
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_雲端藥檔, dBConfigClass.DB_Medicine_Cloud);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_交易記錄查詢, dBConfigClass.DB_tradding);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_人員資料, dBConfigClass.DB_person_page);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_醫令資料, dBConfigClass.DB_order);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_Box_Index_Table, dBConfigClass.DB_Basic);

            
            this.rfiD_UI.Init(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);
            this.storageUI_EPD_266.Init(dBConfigClass.DB_Basic);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_Main, this.plC_ScreenPage_Main);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_人員資料_權限設定, this.plC_ScreenPage_人員資料_權限設定);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_人員資料_開門權限, this.plC_ScreenPage_人員資料_開門權限);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_系統頁面, this.plC_ScreenPage_系統頁面);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_櫃體狀態_PannelBox, this.plC_ScreenPage_櫃體狀態_PannelBox);

         
            this.Program_系統頁面_Init();
            this.Program_醫令資料_Init();
            this.Program_交易紀錄_Init();
            this.Program_人員資料_Init();
            this.Program_登入畫面_Init();       
            this.Program_工程模式_Init();
            this.Program_櫃體狀態_Init();
            this.Program_雲端藥檔_Init();
            this.Program_藥品資料_藥檔資料_Init();
            this.Program_勤務取藥_Init();
            this.Program_配藥核對_Init();
            this.Pannel_Box_Init();

            if (this.myConfigClass.Show_login == false)
            {
                this.plC_ScreenPage_Main.SelecteTabText("勤務取藥");
                panel_Main.Visible = false;
            }

            if (PLC_Device_開門異常時間.Value <= 5000) PLC_Device_開門異常時間.Value = 5000;
            Pannel_Box.AlarmTime = PLC_Device_開門異常時間.Value;
       
            this.WindowState = FormWindowState.Maximized;
        }
        private void ApiServerSetting()
        {
            List<HIS_DB_Lib.sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{dBConfigClass.Api_Server}/api/ServerSetting");
            if (sys_serverSettingClasses.Count == 0)
            {
                MyMessageBox.ShowDialog("API Server 連結失敗!");
                return;
            }
            HIS_DB_Lib.sys_serverSettingClass sys_serverSettingClass;

            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "本地端");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_Basic.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_Basic.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_Basic.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_Basic.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_Basic.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "VM端");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_person_page.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_person_page.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_person_page.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_person_page.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_person_page.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "藥檔資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_Medicine_Cloud.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_Medicine_Cloud.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_Medicine_Cloud.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_Medicine_Cloud.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_Medicine_Cloud.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "交易紀錄資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_tradding.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_tradding.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_tradding.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_tradding.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_tradding.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "醫囑資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_order.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_order.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_order.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_order.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_order.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "儲位資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_storage.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_storage.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_storage.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_storage.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_storage.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "API01");
            if (sys_serverSettingClass != null) dBConfigClass.Api_URL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "Order_API");
            if (sys_serverSettingClass != null) dBConfigClass.OrderApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "Med_API");
            if (sys_serverSettingClass != null) dBConfigClass.MedApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "OrderCheckin_API");
            if (sys_serverSettingClass != null) dBConfigClass.OrderCheckinApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "OrderTakeOut_API");
            if (sys_serverSettingClass != null) dBConfigClass.OrderTakeOutApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "DoorOpen_API");
            if (sys_serverSettingClass != null) dBConfigClass.DoorOpenApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(dBConfigClass.Name, enum_sys_serverSetting_Type.傳送櫃, "DoorClose_API");
            if (sys_serverSettingClass != null) dBConfigClass.DoorCloseApiURL = sys_serverSettingClass.Server;

            API_Server = dBConfigClass.Api_URL;
            ServerName = dBConfigClass.Name;
            ServerType = enum_sys_serverSetting_Type.傳送櫃.GetEnumName();

        }
        private void LoadcommandLineArgs()
        {
            string jsonstr = MyFileStream.LoadFileAllText($"{DBConfigFileName}");
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length >= 3)
            {
                dBConfigClass.Api_Server = commandLineArgs[1];
                dBConfigClass.Name = commandLineArgs[2];
              
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
        #region PLC_Method
        PLC_Device PLC_Device_Method = new PLC_Device("");
        PLC_Device PLC_Device_Method_OK = new PLC_Device("");
        Task Task_Method;
        MyTimer MyTimer_Method_結束延遲 = new MyTimer();
        int cnt_Program_Method = 65534;
        void sub_Program_Method()
        {
            if (cnt_Program_Method == 65534)
            {
                this.MyTimer_Method_結束延遲.StartTickTime(10000);
                PLC_Device_Method.SetComment("PLC_Method");
                PLC_Device_Method_OK.SetComment("PLC_Method_OK");
                PLC_Device_Method.Bool = false;
                cnt_Program_Method = 65535;
            }
            if (cnt_Program_Method == 65535) cnt_Program_Method = 1;
            if (cnt_Program_Method == 1) cnt_Program_Method_檢查按下(ref cnt_Program_Method);
            if (cnt_Program_Method == 2) cnt_Program_Method_初始化(ref cnt_Program_Method);
            if (cnt_Program_Method == 3) cnt_Program_Method = 65500;
            if (cnt_Program_Method > 1) cnt_Program_Method_檢查放開(ref cnt_Program_Method);

            if (cnt_Program_Method == 65500)
            {
                this.MyTimer_Method_結束延遲.TickStop();
                this.MyTimer_Method_結束延遲.StartTickTime(10000);
                PLC_Device_Method.Bool = false;
                PLC_Device_Method_OK.Bool = false;
                cnt_Program_Method = 65535;
            }
        }
        void cnt_Program_Method_檢查按下(ref int cnt)
        {
            if (PLC_Device_Method.Bool) cnt++;
        }
        void cnt_Program_Method_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Method.Bool) cnt = 65500;
        }
        void cnt_Program_Method_初始化(ref int cnt)
        {
            if (this.MyTimer_Method_結束延遲.IsTimeOut())
            {
                if (Task_Method == null)
                {
                    Task_Method = new Task(new Action(delegate { }));
                }
                if (Task_Method.Status == TaskStatus.RanToCompletion)
                {
                    Task_Method = new Task(new Action(delegate { }));
                }
                if (Task_Method.Status == TaskStatus.Created)
                {
                    Task_Method.Start();
                }
                cnt++;
            }
        }







        #endregion


    }
}
