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
using H_Pannel_lib;
[assembly: AssemblyVersion("1.0.25.10196")]
[assembly: AssemblyFileVersion("1.0.25.10196")]
namespace 調劑台管理系統
{

    public partial class Main_Form : Form
    {

        public static bool StorageAlarm = true;
        public bool ControlMode = false;
        private bool flag_Init = false;
        public static string ServerName = "";
        public static string ServerType = enum_sys_serverSetting_Type.調劑台.GetEnumName();
        public static string API_Server = "";
        public static string Order_URL = "";
        public static string OrderByCodeApi_URL = "";
        public static string currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static RFID_FX600lib.RFID_FX600_UI _RFID_FX600_UI = null;
        public static System.Windows.Forms.TextBox _textBox_工程模式_領藥台_名稱;
        public static Pannel_Locker_Design _pannel_Locker_Design;
        public static PLC_ScreenPage _plC_ScreenPage_Main;

        public static string 領藥台_01名稱
        {
            get
            {
                if (PLC_Device_主機扣賬模式.Bool == true) return $"{_textBox_工程模式_領藥台_名稱.Text}_01";
                else return $"{_textBox_工程模式_領藥台_名稱.Text}_S01";

            }
        }
        public static string 領藥台_02名稱
        {
            get
            {
                if (PLC_Device_主機扣賬模式.Bool == true) return $"{_textBox_工程模式_領藥台_名稱.Text}_02";
                else return $"{_textBox_工程模式_領藥台_名稱.Text}_S02";
            }
        }
        public static string 領藥台_03名稱
        {
            get
            {
                if (PLC_Device_主機扣賬模式.Bool == true) return $"{_textBox_工程模式_領藥台_名稱.Text}_03";
                else return $"{_textBox_工程模式_領藥台_名稱.Text}_S03";
            }
        }
        public static string 領藥台_04名稱
        {
            get
            {
                if (PLC_Device_主機扣賬模式.Bool == true) return $"{_textBox_工程模式_領藥台_名稱.Text}_04";
                else return $"{_textBox_工程模式_領藥台_名稱.Text}_S04";
            }
        }

        private PrinterClass printerClass = new PrinterClass();
        private string FormText = "";
        private LadderConnection.LowerMachine PLC;
        MyTimer MyTimer_TickTime = new MyTimer();
        private Stopwatch stopwatch = new Stopwatch();
        List<Pannel_Locker> List_Locker = new List<Pannel_Locker>();
        Basic.MyConvert myConvert = new Basic.MyConvert();

        static public PLC_Device PLC_Device_主機輸出模式 = new PLC_Device("S1001");
        static public PLC_Device PLC_Device_申領_不需輸入申領量 = new PLC_Device("S5025");
        static public PLC_Device PLC_Device_面板於調劑結束更新 = new PLC_Device("S5030");
        static public PLC_Device PLC_Device_面板於過帳後更新 = new PLC_Device("S5031");
        static public PLC_Device PLC_Device_閒置登出要警示 = new PLC_Device("S5040");
        static public PLC_Device PLC_Device_未交班無法調劑 = new PLC_Device("S3104");
        static public PLC_Device PLC_Device_導引模式 = new PLC_Device("S3105");
        static public PLC_Device PLC_Device_掃碼顏色固定 = new PLC_Device("S3112");
        static public PLC_Device PLC_Device_AI處方核對啟用 = new PLC_Device("S3120");
        static public PLC_Device PLC_Device_顯示診斷訊息 = new PLC_Device("S3121");
        static public PLC_Device PLC_Device_主機扣賬模式 = new PLC_Device("S1002");
        static public PLC_Device PLC_Device_掃碼槍COM通訊 = new PLC_Device("S1003");
        static public PLC_Device PLC_Device_抽屜不鎖上 = new PLC_Device("S1004");
        static public PLC_Device PLC_Device_藥物辨識圖片顯示 = new PLC_Device("S1005");
        static public PLC_Device PLC_Device_醫令檢查範圍 = new PLC_Device("D4020");
        static public PLC_Device PLC_Device_手輸數量 = new PLC_Device("S4067");
        static public PLC_Device PLC_Device_領藥處方選取 = new PLC_Device("S5027");
        static public PLC_Device PLC_Device_領藥不檢查是否掃碼領藥過 = new PLC_Device("S5010");
        static public PLC_Device PLC_Device_退藥不檢查是否掃碼領藥過 = new PLC_Device("S5020");
        static public PLC_Device PLC_Device_同藥碼同時取藥亮紫色 = new PLC_Device("S5033");
        static public PLC_Device PLC_Device_調劑畫面合併相同藥品 = new PLC_Device("S5011");
        static public PLC_Device PLC_Device_S800 = new PLC_Device("S800");
        static public PLC_Device PLC_Device_刷藥袋有相同藥品需警示 = new PLC_Device("S5026");
        static public PLC_Device PLC_Device_批次領藥_藥品總量調劑 = new PLC_Device("S5022");
        static public PLC_Device PLC_Device_盤點_顯示庫存量及預帶盤點量 = new PLC_Device("S5050");
        static public PLC_Device PLC_Device_退藥不選擇效期批號 = new PLC_Device("S5053");
        static public PLC_Device PLC_Device_盤點異常量直接寫入庫存 = new PLC_Device("S5054");
        static public PLC_Device PLC_Device_手輸醫令要選擇收支原因 = new PLC_Device("S5055");
        static public PLC_Device PLC_Device_退藥要選擇收支原因 = new PLC_Device("S5056");
        static public PLC_Device PLC_Device_調劑登入提示語音開啟 = new PLC_Device("S5057");
        static public PLC_Device PLC_Device_藥單重複刷取開檢查 = new PLC_Device("S5058");


        static public PLC_Device PLC_Device_使用藥品群組排序盤點 = new PLC_Device("S5051");
        static public PLC_Device PLC_Device_多醫令模式 = new PLC_Device("S5013");

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
        #region MyConfigClass
        private static string MyConfigFileName = $@"{currentDirectory}\MyConfig.txt";
        static public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {

            private bool _ControlMode = false;
            private bool _主機扣帳模式 = false;
            private bool _主機輸出模式 = false;
            private bool _系統取藥模式 = false;
            private bool _RFID使用 = true;
            private bool _掃碼槍COM通訊 = true;
            private bool _藥物辨識圖片顯示 = true;
            private bool ePD583_Enable = true;
            private bool ePD266_Enable = true;
            private bool ePD1020_Enable = true;
            private bool rowsLED_Enable = true;
            private bool rFID_Enable = true;
            private bool pannel35_Enable = true;
            private bool _舊版晶片 = false;

            private int ePD583_Port = 29005;
            private int ePD266_Port = 29000;
            private int ePD1020_Port = 29012;
            private int rowsLED_Port = 29001;
            private int pannel35_Port = 29020;

            private bool _帳密登入_Enable = true;
            private bool _外部輸出 = false;
            private bool _全螢幕顯示 = true;
            private bool _鍵盤掃碼模式 = false;
            private string _批次領藥篩選 = "";

            private string rFID_COMPort = "COM1";
            private string scanner01_COMPort = "COM2";
            private string scanner02_COMPort = "COM3";
            private string scanner03_COMPort = "";
            private string scanner04_COMPort = "";
            private string hFRFID_1_COMPort = "";
            private string hFRFID_2_COMPort = "";
            private string _藥物辨識網址 = "";
            private string _聲紋辨識_IP = "";


            public bool 主機扣帳模式 { get => _主機扣帳模式; set => _主機扣帳模式 = value; }
            public bool 主機輸出模式 { get => _主機輸出模式; set => _主機輸出模式 = value; }
            public bool RFID使用 { get => _RFID使用; set => _RFID使用 = value; }
            public bool 掃碼槍COM通訊 { get => _掃碼槍COM通訊; set => _掃碼槍COM通訊 = value; }
            public string RFID_COMPort { get => rFID_COMPort; set => rFID_COMPort = value; }
            public string Scanner01_COMPort { get => scanner01_COMPort; set => scanner01_COMPort = value; }
            public string Scanner02_COMPort { get => scanner02_COMPort; set => scanner02_COMPort = value; }
            public string Scanner03_COMPort { get => scanner03_COMPort; set => scanner03_COMPort = value; }
            public string Scanner04_COMPort { get => scanner04_COMPort; set => scanner04_COMPort = value; }
            public bool 藥物辨識圖片顯示 { get => _藥物辨識圖片顯示; set => _藥物辨識圖片顯示 = value; }
            public bool EPD583_Enable { get => ePD583_Enable; set => ePD583_Enable = value; }
            public bool EPD266_Enable { get => ePD266_Enable; set => ePD266_Enable = value; }
            public bool RowsLED_Enable { get => rowsLED_Enable; set => rowsLED_Enable = value; }
            public bool RFID_Enable { get => rFID_Enable; set => rFID_Enable = value; }
            public bool Pannel35_Enable { get => pannel35_Enable; set => pannel35_Enable = value; }
            public bool 帳密登入_Enable { get => _帳密登入_Enable; set => _帳密登入_Enable = value; }
            public string 藥物辨識網址 { get => _藥物辨識網址; set => _藥物辨識網址 = value; }
            public bool 外部輸出 { get => _外部輸出; set => _外部輸出 = value; }
            public bool 系統取藥模式 { get => _系統取藥模式; set => _系統取藥模式 = value; }
            public bool EPD1020_Enable { get => ePD1020_Enable; set => ePD1020_Enable = value; }
            public bool 全螢幕顯示 { get => _全螢幕顯示; set => _全螢幕顯示 = value; }
            public bool 鍵盤掃碼模式 { get => _鍵盤掃碼模式; set => _鍵盤掃碼模式 = value; }
            public int EPD583_Port { get => ePD583_Port; set => ePD583_Port = value; }
            public int EPD266_Port { get => ePD266_Port; set => ePD266_Port = value; }
            public int EPD1020_Port { get => ePD1020_Port; set => ePD1020_Port = value; }
            public int RowsLED_Port { get => rowsLED_Port; set => rowsLED_Port = value; }
            public int Pannel35_Port { get => pannel35_Port; set => pannel35_Port = value; }
            public bool ControlMode { get => _ControlMode; set => _ControlMode = value; }
            public bool 舊版晶片 { get => _舊版晶片; set => _舊版晶片 = value; }
            public string 聲紋辨識_IP { get => _聲紋辨識_IP; set => _聲紋辨識_IP = value; }
            public string HFRFID_1_COMPort { get => hFRFID_1_COMPort; set => hFRFID_1_COMPort = value; }
            public string HFRFID_2_COMPort { get => hFRFID_2_COMPort; set => hFRFID_2_COMPort = value; }
            public string 批次領藥篩選 { get => _批次領藥篩選; set => _批次領藥篩選 = value; }
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
        #region MyParameter
        private static string MyParameterFileName = $@"{currentDirectory}\parameter.txt";
        static public MyParameter myParameter = new MyParameter();
        public class MyParameter
        {

            private List<string> _交班開鎖抽屜 = new List<string>();

            public List<string> 交班開鎖抽屜 { get => _交班開鎖抽屜; set => _交班開鎖抽屜 = value; }
        }
        static public void SaveMyParameter()
        {
            string jsonstr = myParameter.JsonSerializationt();
            if (!MyFileStream.SaveFile($"{MyParameterFileName}", jsonstr))
            {
                MyMessageBox.ShowDialog($"建立{MyParameterFileName}檔案失敗!");

            }
        }
        static public void LoadMyParameter()
        {
            string jsonstr = MyFileStream.LoadFileAllText($"{MyParameterFileName}");
            myParameter = jsonstr.JsonDeserializet<MyParameter>();
            if (myParameter == null)
            {
                myParameter = new MyParameter();
                SaveMyParameter();
            }

        }
        #endregion

        public Main_Form()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
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
        }
        private void Main_Form_Load(object sender, EventArgs e)
        {
            CloseProcessByName("batch_StackDataAccounting");

            _textBox_工程模式_領藥台_名稱 = this.textBox_工程模式_領藥台_名稱;
            _pannel_Locker_Design = this.pannel_Locker_Design;
            _plC_ScreenPage_Main = this.plC_ScreenPage_Main;

            

            H_Pannel_lib.Communication.ConsoleWrite = false;
            Net.DebugLog = false;
            if (this.DesignMode == false)
            {
                MyDialog.form = this.FindForm();
                LoadingForm.form = this.FindForm();
                MyMessageBox.form = this.FindForm();
                Dialog_交易紀錄明細.form = this.FindForm();
                Dialog_NumPannel.form = this.FindForm();
                Dialog_輸入批號.form = this.FindForm();
                Dialog_輸入效期.form = this.FindForm();
                Dialog_輸入藥品碼.form = this.FindForm();
                Dialog_條碼輸入.form = this.FindForm();
                Dialog_醫令退藥.form = this.FindForm();
                Dialog_設定產出時間.form = this.FindForm();
                Dialog_RFID領退藥頁面.form = this.FindForm();
                Dialog_輸入輸出設定.form = this.FindForm();
                Dialog_新增容器設定.form = this.FindForm();
                Dialog_選擇效期.form = this.FindForm();
                Dialog_收支原因設定.form = this.FindForm();
                Dialog_收支原因選擇.form = this.FindForm();
                Dialog_DateTime.form = this.FindForm();
                Dialog_條碼管理.form = this.FindForm();
                Dialog_使用者登入.form = this.FindForm();
                Dialog_盤點數量錯誤.form = this.FindForm();
                Dialog_病歷號輸入.form = this.FindForm();
                Dialog_藥檔維護.form = this.FindForm();
                Dialog_錯誤提示.form = this.FindForm();
                Dialog_共用區設置.form = this.FindForm();
                Dialog_AlarmForm.form = this.FindForm();
                Dialog_藥品調出.form = this.FindForm();
                Dialog_交班對點.form = this.FindForm();
                Dialog_藥品群組.form = this.FindForm();
                Dialog_異常通知.form = this.FindForm();
                Dialog_申領.form = this.FindForm();
                Dialog_批次入庫.form = this.FindForm();


                LoadDBConfig();
                LoadMyConfig();

                if (myConfigClass.ControlMode) ControlMode = true;

                ApiServerSetting();

                this.stopwatch.Start();

                this.FormText = this.Text;
                this.plC_UI_Init.音效 = false;
                this.plC_UI_Init.全螢幕顯示 = myConfigClass.全螢幕顯示;

                this.plC_UI_Init.UI_Finished_Event += PlC_UI_Init_UI_Finished_Event;
                this.plC_UI_Init.Run(this.FindForm(), this.lowerMachine_Panel);


                Basic.Keyboard.Hook.KeyDown += Hook_KeyDown;
                Basic.Keyboard.Hook.MouseDown += Hook_MouseDown;
                Basic.Keyboard.Hook.MouseUp += Hook_MouseUp;
                this.button_調劑台切換.Click += Button_調劑台切換_Click;
                Basic.MyMessageBox.音效 = false;
            
                printerClass.Init();
                printerClass.PrintPageEvent += PrinterClass_PrintPageEvent;


                this.plC_ScreenPage_Main.TabChangeEvent += PlC_ScreenPage_Main_TabChangeEvent;
                this.plC_ScreenPage_調劑樣式.Resize += PlC_ScreenPage_調劑樣式_Resize;

                plC_CheckBox_面板於調劑結束更新.CheckedChanged += PlC_CheckBox_面板於調劑結束更新_CheckedChanged;
                plC_CheckBox_面板於過帳後更新.CheckedChanged += PlC_CheckBox_面板於過帳後更新_CheckedChanged;

                this.ToolStripMenuItem_顯示主控台.Click += ToolStripMenuItem_顯示主控台_Click;
                this.ToolStripMenuItem_隱藏主控台.Click += ToolStripMenuItem_隱藏主控台_Click;

                MyMessageBox.TimerEvent += MyMessageBox_TimerEvent;

            }
        }


        #region Event
        private void MyMessageBox_TimerEvent(MyMessageBox myMessageBox)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Function_ReadBacodeScanner_pre(i) != null)
                {
                    myMessageBox.Close();
                    return;
                }
            }
        }
        private void PlC_ScreenPage_Main_TabChangeEvent(string PageText)
        {
            if (this.plC_ScreenPage_Main.PageText == "後台登入")
            {
                panel_主畫面登入.Location = new Point((後台登入.Width - panel_主畫面登入.Width) / 2, (後台登入.Height - panel_主畫面登入.Height) / 2);
            }
        }
        private void PlC_UI_Init_UI_Finished_Event()
        {


            if (myConfigClass.全螢幕顯示 == false)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            this.PLC = this.lowerMachine_Panel.GetlowerMachine();

            PLC_Device_主機輸出模式.Bool = myConfigClass.主機輸出模式;
            PLC_Device_主機扣賬模式.Bool = myConfigClass.主機扣帳模式;
            PLC_Device_掃碼槍COM通訊.Bool = myConfigClass.掃碼槍COM通訊;
            PLC_Device_藥物辨識圖片顯示.Bool = myConfigClass.藥物辨識圖片顯示;
            PLC_Device_S800.Bool = false;


            if (myConfigClass.Scanner01_COMPort.StringIsEmpty())
            {
                rJ_Pannel_領藥台_01.Visible = false;
            }
            if (myConfigClass.Scanner02_COMPort.StringIsEmpty())
            {
                rJ_Pannel_領藥台_02.Visible = false;
            }
            if (myConfigClass.Scanner03_COMPort.StringIsEmpty())
            {
                rJ_Pannel_領藥台_03.Visible = false;
            }
            if (myConfigClass.Scanner04_COMPort.StringIsEmpty())
            {
                rJ_Pannel_領藥台_04.Visible = false;
            }
            if (myConfigClass.Scanner03_COMPort.StringIsEmpty() && myConfigClass.Scanner04_COMPort.StringIsEmpty())
            {
                panel_領藥台_03_04.Visible = false;
            }
            if (dBConfigClass.Web_URL.StringIsEmpty()) plC_RJ_GroupBox_後台網址_QRCODE.Visible = false;
            else
            {
                pictureBox_後台網址_QRCODE.Image = H_Pannel_lib.Communication.CreateQRCode(dBConfigClass.Web_URL, pictureBox_後台網址_QRCODE.Width, pictureBox_後台網址_QRCODE.Height);
            }
            if (this.ControlMode)
            {
                this.plC_RJ_GroupBox_調劑台切換.Visible = true;
                PLC_Device_主機輸出模式.Bool = false;
                PLC_Device_主機扣賬模式.Bool = false;
                PLC_Device_掃碼槍COM通訊.Bool = false;

                PLC_Device_藥物辨識圖片顯示.Bool = false;
                myConfigClass.RFID使用 = false;
                this.plC_RJ_ScreenButton_調劑作業.顯示讀取位置 = "M8001";
                this.plC_RJ_ScreenButton_管制抽屜.顯示讀取位置 = "M8001";
                this.plC_RJ_ScreenButton_交班作業.顯示讀取位置 = "M8000";
                this.plC_RJ_ScreenButton_收支作業.顯示讀取位置 = "M8001";
                this.plC_RJ_ScreenButton_交易紀錄查詢.顯示讀取位置 = "M8000";
                this.plC_RJ_ScreenButton_醫令資料.顯示讀取位置 = "M8000";
                this.plC_RJ_ScreenButton_藥品資料.顯示讀取位置 = "M8000";
                this.plC_RJ_ScreenButton_人員資料.顯示讀取位置 = "M8000";
                this.plC_RJ_ScreenButton_儲位管理.顯示讀取位置 = "M8000";
                this.plC_RJ_ScreenButton_盤點作業.顯示讀取位置 = "M8001";
                this.plC_RJ_ScreenButton_工程模式.顯示讀取位置 = "M8001";
                this.plC_ScreenPage_Main.SelecteTabText("後台登入");
            }

            PLC_UI_Init.Set_PLC_ScreenPage(panel_Main, this.plC_ScreenPage_Main);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_交班作業, this.plC_ScreenPage_交班作業);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_藥品資料, this.plC_ScreenPage_藥品資料);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_系統, this.plC_ScreenPage_系統);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_系統_Pannel設定, this.plC_ScreenPage_系統_Pannel設定);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_儲位管理, this.plC_ScreenPage_儲位管理);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_人員資料, this.plC_ScreenPage_人員資料);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_盤點作業, this.plC_ScreenPage_盤點作業);
            PLC_UI_Init.Set_PLC_ScreenPage(panel_交易紀錄查詢, this.plC_ScreenPage_交易紀錄查詢);

            this.plC_RJ_ScreenButton_EPD583.Visible = myConfigClass.EPD583_Enable;
            this.plC_RJ_ScreenButton_EPD266.Visible = myConfigClass.EPD266_Enable;
            this.plC_RJ_ScreenButton_EPD1020.Visible = myConfigClass.EPD1020_Enable;
            this.plC_RJ_ScreenButton_RowsLED.Visible = myConfigClass.RowsLED_Enable;
            this.plC_RJ_ScreenButton_RFID.Visible = myConfigClass.RFID_Enable;
            this.plC_RJ_ScreenButton_Pannel35.Visible = myConfigClass.Pannel35_Enable;


            this.DBConfigInit();

            this.Program_系統_Init();

      
            this.Program_Scanner_RS232_Init();

            _drawerUI_EPD_1020 = this.drawerUI_EPD_1020;
            _storageUI_EPD_266 = this.storageUI_EPD_266;
            _storageUI_WT32 = this.storageUI_WT32;
            _drawerUI_EPD_583 = this.drawerUI_EPD_583;
            _storageUI_LCD_114 = this.storageUI_LCD_114;

            this.Program_醫令資料_Init();
            this.Program_藥品資料_藥檔資料_Init();
            this.Program_藥品區域_Init();
            this.Program_儲位管理_EPD583_Init();
            this.Program_儲位管理_EPD266_Init();
            this.Program_儲位管理_EPD1020_Init();
            this.Program_儲位管理_RowsLED_Init();
            this.Program_儲位管理_RFID_Init();
            this.Program_儲位管理_Pannel35_Init();
            this.Program_共用區_Init();

            this.Program_LCD114_索引表_Init();
            this.Program_取藥堆疊資料_Init();
            RFID_Iint();
            if (!this.ControlMode) this.Program_調劑作業_Init();


            this.Program_藥品資料_儲位總庫存表_Init();
            this.Program_藥品資料_儲位效期表_Init();
            this.Program_藥品資料_管藥設定_Init();

            this.Program_雲端藥檔_Init();
            this.Program_人員資料_Init();
            this.Program_工程模式_Init();
            this.Program_交易記錄查詢_Init();
            this.Program_交易記錄查詢_結存量_Init();
            this.Program_效期管理_Init();

            this.Program_後台登入_Init();
            this.Program_批次領藥_Init();


            this.Program_管制抽屜_Init();
            this.Program_設備資料_Init();
            this.Program_交班作業_對點作業_Init();
            this.Program_交班作業_管制結存_Init();
            this.Program_交班作業_交班表_Init();
            this.Program_藥品管制方式設定_Init();
            this.Program_藥品設定表_Init();


            this.sub_Program_盤點作業_定盤_Init();
            this.sub_Program_盤點作業_新增盤點_Init();
            this.sub_Program_盤點作業_單號查詢_Init();
            this.sub_Program_盤點作業_資料庫_Init();
            this.Program_異常通知_Init();

            if (!this.ControlMode) this.Program_輸出入檢查_Init();
            this.Program_收支作業_Init();
            this.Program_指紋辨識_Init();
            this.Program_HFRFID_Init();

            this.Program_異常通知_盤點錯誤_Init();
            this.Program_聲紋辨識_Init();
            this.LoadConfig工程模式();

            if (!this.ControlMode) Main_Form.Function_取藥堆疊資料_刪除指定調劑台名稱母資料(領藥台_01名稱);
            if (!this.ControlMode) Main_Form.Function_取藥堆疊資料_刪除指定調劑台名稱母資料(領藥台_02名稱);

            this.drawerUI_EPD_1020.Set_UDP_WriteTime(10);
            this.drawerUI_EPD_583.Set_UDP_WriteTime(10);
            this.storageUI_EPD_266.Set_UDP_WriteTime(1);
            this.storageUI_LCD_114.Set_UDP_WriteTime(1);
            this.storageUI_WT32.Set_UDP_WriteTime(5);

       

            DateTime dateTime = sys_serverSettingClass.GetServerTime(API_Server);
            Console.WriteLine($"從伺服器取得時間: {dateTime:yyyy/MM/dd HH:mm:ss}");

            if (dateTime != DateTime.MinValue)
            {
                if (NTP_ServerLib.NTPServerClass.SyncTime(dateTime))
                {
                    Console.WriteLine("已同步系統時間。");
                }
                else
                {
                    Console.WriteLine("同步系統時間失敗。");

                }
            }
            else
            {
                Console.WriteLine("取得伺服器時間失敗，未進行同步。");
            }

            Task task = Task.Run(new Action(delegate
            {
                if (!this.ControlMode) Function_從SQL取得儲位到本地資料();

              
            }));
            if (!this.ControlMode) this.plC_ScreenPage_Main.SelecteTabText("調劑作業");


         


            flag_Init = true;
        }
        private void PrinterClass_PrintPageEvent(object sender, Graphics g, int width, int height, int page_num)
        {
            Rectangle rectangle = new Rectangle(0, 0, width, height);
            using (Bitmap bitmap = printerClass.GetSheetClass(page_num).GetBitmap(width, height, 0.63, H_Alignment.Center, V_Alignment.Top, 0, 50))
            {
                rectangle.Height = bitmap.Height;
                g.DrawImage(bitmap, rectangle);
            }
        }
        private void Hook_MouseDown(int nCode, int mouse_x, int mouse_y)
        {
            if (this.CanFocus)
            {
                this.myTimer_登出計時.TickStop();
                this.myTimer_登出計時.StartTickTime(1200000);
            }
        }
        private void Hook_KeyDown(int nCode, IntPtr wParam, Keys Keys)
        {
            if (this.CanFocus)
            {
                this.myTimer_登出計時.TickStop();
                this.myTimer_登出計時.StartTickTime(1200000);
            }
        }
        private void Hook_MouseUp(int nCode, int mouse_x, int mouse_y)
        {

        }
        private void plC_RJ_Button_系統更新_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Update();
        }
        private void Button_調劑台切換_Click(object sender, EventArgs e)
        {
            string DPS_Name = this.comboBox_調劑台名稱.Text;
            if (DPS_Name.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未選擇調劑台!");
                return;
            }
            this.ApiServerSetting(DPS_Name, "一般資料");
            this.DBConfigInit();
            MyMessageBox.ShowDialog("切換完成!");
        }
        private void Main_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }
        private void PlC_ScreenPage_調劑樣式_Resize(object sender, EventArgs e)
        {
            Control control = sender as Control;
            int basic_width = 1660;
            int offset_width = (control.Width - basic_width) / 2;

            uC_調劑作業_TypeA_3.panel_藥品資訊.Visible = false;
            uC_調劑作業_TypeA_4.panel_藥品資訊.Visible = false;

            if (NumOfConnectedScanner > 1)
            {
                if (flag_Init && uC_調劑作業_TypeA_1.sqL_DataGridView_領藥內容.CustomEnable == false) this.uC_調劑作業_TypeA_1.sqL_DataGridView_領藥內容.Set_ColumnWidth(355 + offset_width, DataGridViewContentAlignment.MiddleLeft, enum_取藥堆疊母資料.藥品名稱);
                if (flag_Init && uC_調劑作業_TypeA_2.sqL_DataGridView_領藥內容.CustomEnable == false) this.uC_調劑作業_TypeA_2.sqL_DataGridView_領藥內容.Set_ColumnWidth(355 + offset_width, DataGridViewContentAlignment.MiddleLeft, enum_取藥堆疊母資料.藥品名稱);
                if (flag_Init && uC_調劑作業_TypeA_3.sqL_DataGridView_領藥內容.CustomEnable == false) this.uC_調劑作業_TypeA_3.sqL_DataGridView_領藥內容.Set_ColumnWidth(355 + offset_width, DataGridViewContentAlignment.MiddleLeft, enum_取藥堆疊母資料.藥品名稱);
                if (flag_Init && uC_調劑作業_TypeA_4.sqL_DataGridView_領藥內容.CustomEnable == false) this.uC_調劑作業_TypeA_4.sqL_DataGridView_領藥內容.Set_ColumnWidth(355 + offset_width, DataGridViewContentAlignment.MiddleLeft, enum_取藥堆疊母資料.藥品名稱);
            }
            if (NumOfConnectedScanner == 4)
            {

                int width = control.Width / 2 - 10;
                int height = control.Height / 2 - 10;
                rJ_Pannel_領藥台_01.Width = width;
                rJ_Pannel_領藥台_02.Width = width;
                rJ_Pannel_領藥台_03.Width = width;
                rJ_Pannel_領藥台_04.Width = width;
                panel_領藥台_01_02.Height = height;
                panel_領藥台_03_04.Height = height;
                uC_調劑作業_TypeA_1.panel_藥品資訊.Visible = false;
                uC_調劑作業_TypeA_2.panel_藥品資訊.Visible = false;
            }
            else if (NumOfConnectedScanner == 3)
            {
                int width = control.Width / 2 - 10;
                int height = control.Height / 2 - 10;
                rJ_Pannel_領藥台_01.Width = width;
                rJ_Pannel_領藥台_02.Width = width;
                rJ_Pannel_領藥台_03.Width = width;
                rJ_Pannel_領藥台_04.Width = width;
                panel_領藥台_01_02.Height = height;
                panel_領藥台_03_04.Height = height;
                uC_調劑作業_TypeA_1.panel_藥品資訊.Visible = false;
                uC_調劑作業_TypeA_2.panel_藥品資訊.Visible = false;
            }
            else if (NumOfConnectedScanner == 2)
            {
                int width = control.Width / 2 - 10;
                int height = control.Height / 1 - 10;
                rJ_Pannel_領藥台_01.Width = width;
                rJ_Pannel_領藥台_02.Width = width;
                rJ_Pannel_領藥台_03.Width = width;
                rJ_Pannel_領藥台_04.Width = width;
                panel_領藥台_01_02.Height = height;
                panel_領藥台_03_04.Height = height;

                uC_調劑作業_TypeA_1.pictureBox_藥品圖片01.Width = uC_調劑作業_TypeA_1.panel_藥品圖片.Width / 2;
                uC_調劑作業_TypeA_1.pictureBox_藥品圖片02.Width = uC_調劑作業_TypeA_1.panel_藥品圖片.Width / 2;

                uC_調劑作業_TypeA_2.pictureBox_藥品圖片01.Width = uC_調劑作業_TypeA_2.panel_藥品圖片.Width / 2;
                uC_調劑作業_TypeA_2.pictureBox_藥品圖片02.Width = uC_調劑作業_TypeA_2.panel_藥品圖片.Width / 2;
            }
            else if (NumOfConnectedScanner == 1)
            {
                int width = control.Width / 1 - 10;
                int height = control.Height / 1 - 10;
                rJ_Pannel_領藥台_01.Width = width;
                rJ_Pannel_領藥台_02.Width = width;
                rJ_Pannel_領藥台_03.Width = width;
                rJ_Pannel_領藥台_04.Width = width;
                panel_領藥台_01_02.Height = height;
                panel_領藥台_03_04.Height = height;

                //panel_領藥台01_處方資訊.Visible = true;
                //panel_領藥台01_處方資訊.Height = 240;

                uC_調劑作業_TypeA_1.pictureBox_藥品圖片01.Width = uC_調劑作業_TypeA_1.panel_藥品圖片.Width / 2;
                uC_調劑作業_TypeA_1.pictureBox_藥品圖片02.Width = uC_調劑作業_TypeA_1.panel_藥品圖片.Width / 2;

                uC_調劑作業_TypeA_2.pictureBox_藥品圖片01.Width = uC_調劑作業_TypeA_2.panel_藥品圖片.Width / 2;
                uC_調劑作業_TypeA_2.pictureBox_藥品圖片02.Width = uC_調劑作業_TypeA_2.panel_藥品圖片.Width / 2;
            }
        }
        private void ToolStripMenuItem_顯示主控台_Click(object sender, EventArgs e)
        {
            Basic.Screen.ShowConsole();
        }
        private void ToolStripMenuItem_隱藏主控台_Click(object sender, EventArgs e)
        {
            Basic.Screen.CloseConsole();
        }
        private void PlC_CheckBox_面板於過帳後更新_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked)
            {
                plC_CheckBox_面板於調劑結束更新.Checked = false;
            }
        }
        private void PlC_CheckBox_面板於調劑結束更新_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked)
            {
                plC_CheckBox_面板於過帳後更新.Checked = false;
            }
        }

        #endregion
        #region Function
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
                    if (myConfigClass.RFID使用 == false) break;

                    if (MyTimer_rfiD_FX600_UI_Init.IsTimeOut() && !flag_rfiD_FX600_UI_Init)
                    {
                        if (myConfigClass.RFID使用)
                        {
                            int num = 1;
                            if (myConfigClass.Scanner01_COMPort.StringIsEmpty() == false)
                            {
                                num++;
                            }
                            if (myConfigClass.Scanner02_COMPort.StringIsEmpty() == false)
                            {
                                num++;
                            }
                            if (myConfigClass.Scanner03_COMPort.StringIsEmpty() == false)
                            {
                                num++;
                            }
                            if (myConfigClass.Scanner04_COMPort.StringIsEmpty() == false)
                            {
                                num++;
                            }

                            this.rfiD_FX600_UI.Init(RFID_FX600lib.RFID_FX600_UI.Baudrate._9600, num, myConfigClass.RFID_COMPort);

                        }
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
        private bool flag_DBConfigInit = false;
        private void DBConfigInit()
        {
            this.pannel_Locker_Design.Init(dBConfigClass.DB_Basic);
            Dialog_RFID領退藥頁面.connentionClass = dBConfigClass.DB_Basic;
            SQLUI.SQL_DataGridView.SQL_Set_Properties(dBConfigClass.DB_Basic.DataBaseName, dBConfigClass.DB_Basic.UserName, dBConfigClass.DB_Basic.Password, dBConfigClass.DB_Basic.IP, dBConfigClass.DB_Basic.Port, dBConfigClass.DB_Basic.MySqlSslMode, this.FindForm());
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_批次領藥資料, dBConfigClass.DB_Basic.DataBaseName, dBConfigClass.DB_Basic.UserName, dBConfigClass.DB_Basic.Password, dBConfigClass.DB_Basic.IP, dBConfigClass.DB_Basic.Port, dBConfigClass.DB_Basic.MySqlSslMode);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_藥品資料_藥檔資料, dBConfigClass.DB_Basic);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_雲端藥檔, dBConfigClass.DB_Medicine_Cloud);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_人員資料, dBConfigClass.DB_person_page);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_醫令資料, dBConfigClass.DB_order_list);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_交易記錄查詢, dBConfigClass.DB_tradding);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_管制抽屜權限資料, dBConfigClass.DB_Basic);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_設備資料, dBConfigClass.DB_Medicine_Cloud);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_藥品管制方式設定, dBConfigClass.DB_Medicine_Cloud);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_藥品設定表, dBConfigClass.DB_Medicine_Cloud);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_藥品區域, dBConfigClass.DB_Basic);
            SQLUI.SQL_DataGridView.ConnentionClass dB_local = new SQL_DataGridView.ConnentionClass();
            dB_local.IP = dBConfigClass.DB_Basic.IP;
            dB_local.DataBaseName = dBConfigClass.DB_Basic.DataBaseName;
            dB_local.Port = dBConfigClass.DB_Basic.Port;
            dB_local.UserName = "user";
            dB_local.Password = "66437068";
            dB_local.MySqlSslMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_取藥堆疊母資料, dB_local);
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_取藥堆疊子資料, dB_local);

            this.sqL_DataGridView_藥品資料_藥檔資料.SQL_Reset();
            this.sqL_DataGridView_人員資料.SQL_Reset();
            this.sqL_DataGridView_醫令資料.SQL_Reset();
            this.sqL_DataGridView_管制抽屜權限資料.SQL_Reset();
            this.sqL_DataGridView_設備資料.SQL_Reset();
            this.sqL_DataGridView_藥品管制方式設定.SQL_Reset();
            this.sqL_DataGridView_藥品設定表.SQL_Reset();
            this.sqL_DataGridView_雲端藥檔.SQL_Reset();
            this.sqL_DataGridView_交易記錄查詢.SQL_Reset();

            this.drawerUI_EPD_583.InitEx(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);
            this.drawerUI_EPD_1020.InitEx(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);
            this.storageUI_EPD_266.InitEx(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);
            this.rowsLEDUI.InitEx(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);
            this.rfiD_UI.Init(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);
            this.storageUI_WT32.InitEx(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);
            this.storageUI_LCD_114.Init(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);
     
            if (flag_DBConfigInit == true)
            {
                this.sqL_DataGridView_儲位管理_EPD266_藥品資料_藥檔資料.Init(this.sqL_DataGridView_藥品資料_藥檔資料);
                this.sqL_DataGridView_儲位管理_EPD1020_藥品資料_藥檔資料.Init(this.sqL_DataGridView_藥品資料_藥檔資料);
                this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.Init(this.sqL_DataGridView_藥品資料_藥檔資料);
                this.sqL_DataGridView_儲位管理_Pannel35_藥品資料_藥檔資料.Init(this.sqL_DataGridView_藥品資料_藥檔資料);
                this.sqL_DataGridView_儲位管理_RFID_藥品資料_藥檔資料.Init(this.sqL_DataGridView_藥品資料_藥檔資料);
                this.sqL_DataGridView_儲位管理_RowsLED_藥品資料_藥檔資料.Init(this.sqL_DataGridView_藥品資料_藥檔資料);
            }

            flag_DBConfigInit = true;
            this.DeviceBasicClass_儲位庫存.Init(dBConfigClass.DB_Basic, "devicebasic_jsonstring");
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
            this.textBox_工程模式_領藥台_名稱.Text = Name;
            this.Text = $"智慧調劑台管理系統 Ver{this.ProductVersion} [{this.textBox_工程模式_領藥台_名稱.Text}] {(this.ControlMode ? "****控制中心模式****" : "")}";
            this.label_版本.Text = $"Ver {this.ProductVersion} [{this.textBox_工程模式_領藥台_名稱.Text}] {(this.ControlMode ? "****控制中心模式****" : "")}";
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
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, basicName);
            List<string> DPS_Names = (from temp in sys_serverSettingClasses
                                      where temp.類別 == enum_sys_serverSetting_Type.調劑台.GetEnumName()
                                      select temp.設備名稱).Distinct().ToList();
            if (!flag_Init) comboBox_調劑台名稱.DataSource = DPS_Names;

            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_Basic.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_Basic.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_Basic.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_Basic.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_Basic.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.人員資料);
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_person_page.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_person_page.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_person_page.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_person_page.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_person_page.Password = sys_serverSettingClass.Password;

            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.藥檔資料);
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_Medicine_Cloud.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_Medicine_Cloud.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_Medicine_Cloud.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_Medicine_Cloud.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_Medicine_Cloud.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, "醫囑資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_order_list.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_order_list.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_order_list.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_order_list.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_order_list.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, "儲位資料");
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_storage.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_storage.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_storage.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_storage.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_storage.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.交易紀錄資料);
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.DB_tradding.IP = sys_serverSettingClass.Server;
                dBConfigClass.DB_tradding.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_tradding.DataBaseName = sys_serverSettingClass.DBName;
                dBConfigClass.DB_tradding.UserName = sys_serverSettingClass.User;
                dBConfigClass.DB_tradding.Password = sys_serverSettingClass.Password;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.API02);
            if (sys_serverSettingClass != null)
            {
                dBConfigClass.Api_URL = sys_serverSettingClass.Server;
                API_Server = sys_serverSettingClass.Server;
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, "Order_API");
            if (sys_serverSettingClass != null) dBConfigClass.OrderApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, "Order_By_Code_API");
            if (sys_serverSettingClass != null) dBConfigClass.OrderByCodeApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, "Order_upload_API");
            if (sys_serverSettingClass != null) dBConfigClass.Order_upload_ApiURL = sys_serverSettingClass.Server;

            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, "Order_By_MRN_API");
            if (sys_serverSettingClass != null) dBConfigClass.Order_mrn_ApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, "Order_By_BAG_NUM_API");
            if (sys_serverSettingClass != null) dBConfigClass.Order_bag_num_ApiURL = sys_serverSettingClass.Server;

            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.Med_API);
            if (sys_serverSettingClass != null) dBConfigClass.MedApiURL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.Website);
            if (sys_serverSettingClass != null) dBConfigClass.Web_URL = sys_serverSettingClass.Server;
            sys_serverSettingClass = sys_serverSettingClasses.MyFind("Main", enum_sys_serverSetting_Type.網頁, enum_sys_serverSetting_網頁.API_Login);
            if (sys_serverSettingClass != null) dBConfigClass.Login_URL = sys_serverSettingClass.Server;


            OrderByCodeApi_URL = dBConfigClass.OrderByCodeApiURL;
            Order_URL = dBConfigClass.OrderApiURL;
            if (OrderByCodeApi_URL.StringIsEmpty())
            {
                OrderByCodeApi_URL = Order_URL;
            }
        }
        new private void Update()
        {
            if (this.ftp_DounloadUI.DownloadFile())
            {
                if (this.ftp_DounloadUI.SaveFile())
                {
                    this.ftp_DounloadUI.RunFile(this.FindForm());
                }
                else
                {
                    Basic.MyMessageBox.ShowDialog("安裝檔存檔失敗!");
                }
            }
            else
            {
                Basic.MyMessageBox.ShowDialog("下載失敗!");
            }
        }
        #endregion
        #region PLC_Method
        PLC_Device PLC_Device_Method = new PLC_Device("");
        PLC_Device PLC_Device_Method_OK = new PLC_Device("");
        Task Task_Method;
        MyTimer MyTimer_Method_結束延遲 = new MyTimer();
        MyTimer MyTimer_Method_開始延遲 = new MyTimer();
        int cnt_Program_Method = 65534;
        void sub_Program_Method()
        {
            if (cnt_Program_Method == 65534)
            {
                this.MyTimer_Method_結束延遲.StartTickTime(10000);
                this.MyTimer_Method_開始延遲.StartTickTime(10000);
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

        public static void CloseProcessByName(string processName)
        {
            // 取得所有與指定名稱相符的進程
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                Console.WriteLine($"{processName} 未在執行中。");
            }
            else
            {
                foreach (Process process in processes)
                {
                    try
                    {
                        process.Kill(); // 終止進程
                        Console.WriteLine($"{processName} 已關閉 (PID: {process.Id})。");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"無法關閉 {processName} (PID: {process.Id})：{ex.Message}");
                    }
                }
            }
        }

    }


}
