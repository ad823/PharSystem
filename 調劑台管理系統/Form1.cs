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
[assembly: AssemblyVersion("1.2.1.161")]
[assembly: AssemblyFileVersion("1.2.1.161")]
namespace 調劑台管理系統
{

    public partial class Form1 : Form
    {
        public bool ControlMode = false;
        private bool flag_Init = false;
        public static string ServerName = "";
        public static string ServerType = enum_ServerSetting_Type.調劑台.GetEnumName();
        public static string API_Server = "";
        public static string Order_URL = "";
        public static string OrderByCodeApi_URL = "";
        public static string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public string 領藥台_01名稱
        {
            get
            {
                if(this.PLC_Device_主機扣賬模式.Bool == true) return $"{this.textBox_工程模式_領藥台_名稱.Text}_01";
                else return $"{this.textBox_工程模式_領藥台_名稱.Text}_S01";

            }
        }
        public string 領藥台_02名稱
        {
            get
            {
                if (this.PLC_Device_主機扣賬模式.Bool == true) return $"{this.textBox_工程模式_領藥台_名稱.Text}_02";
                else return $"{this.textBox_工程模式_領藥台_名稱.Text}_S02";
            }
        }
        public string 領藥台_03名稱
        {
            get
            {
                if (this.PLC_Device_主機扣賬模式.Bool == true) return $"{this.textBox_工程模式_領藥台_名稱.Text}_03";
                else return $"{this.textBox_工程模式_領藥台_名稱.Text}_S03";
            }
        }
        public string 領藥台_04名稱
        {
            get
            {
                if (this.PLC_Device_主機扣賬模式.Bool == true) return $"{this.textBox_工程模式_領藥台_名稱.Text}_04";
                else return $"{this.textBox_工程模式_領藥台_名稱.Text}_S04";
            }
        }

        private PrinterClass printerClass = new PrinterClass();
        private string FormText = "";
        private LadderConnection.LowerMachine PLC;
        MyTimer MyTimer_TickTime = new MyTimer();
        private Stopwatch stopwatch = new Stopwatch();
        List<Pannel_Locker> List_Locker = new List<Pannel_Locker>();
        Basic.MyConvert myConvert = new Basic.MyConvert();


        PLC_Device PLC_Device_主機輸出模式 = new PLC_Device("S1001");
        PLC_Device PLC_Device_主機扣賬模式 = new PLC_Device("S1002");
        PLC_Device PLC_Device_掃碼槍COM通訊 = new PLC_Device("S1003");
        PLC_Device PLC_Device_抽屜不鎖上 = new PLC_Device("S1004");
        PLC_Device PLC_Device_藥物辨識圖片顯示 = new PLC_Device("S1005");

        #region DBConfigClass
        private static string DBConfigFileName = $@"{currentDirectory}\DBConfig.txt";
        public DBConfigClass dBConfigClass = new DBConfigClass();
        public class DBConfigClass
        {

            public string Name { get => name; set => name = value; }
            public string Api_Server { get => api_Server; set => api_Server = value; }

            private SQL_DataGridView.ConnentionClass dB_Basic = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_person_page = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_order_list = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_tradding = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_Medicine_Cloud = new SQL_DataGridView.ConnentionClass();

            private string web_URL = "";
            private string api_URL = "";
            private string login_URL = "";
            private string name = "";
            private string api_Server = "";

            private string orderApiURL = "";
            private string orderByCodeApiURL = "";
            private string medApiURL = "";
            private string med_Update_ApiURL = "";



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
            public string OrderApiURL { get => orderApiURL; set => orderApiURL = value; }
            [JsonIgnore]
            public string MedApiURL { get => medApiURL; set => medApiURL = value; }
            [JsonIgnore]
            public string Api_URL { get => api_URL; set => api_URL = value; }
            [JsonIgnore]
            public string Web_URL { get => web_URL; set => web_URL = value; } 
            [JsonIgnore]
            public string Login_URL { get => login_URL; set => login_URL = value; }


            public string Med_Update_ApiURL { get => med_Update_ApiURL; set => med_Update_ApiURL = value; }
            [JsonIgnore]
            public string OrderByCodeApiURL { get => orderByCodeApiURL; set => orderByCodeApiURL = value; }
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
        public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {

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
            private bool _帳密登入_Enable = true;
            private bool _外部輸出 = false;

            private string rFID_COMPort = "COM1";
            private string scanner01_COMPort = "COM2";
            private string scanner02_COMPort = "COM3";
            private string scanner03_COMPort = "";
            private string scanner04_COMPort = "";
            private string _藥物辨識網址 = "";
    

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
        

        public Form1()
        {
            InitializeComponent();
            #region PLC_MultiStateDisplay
            MyUI.PLC_MultiStateDisplay.TextValue textValue1 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue2 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue3 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue4 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue5 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue6 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue7 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue8 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue9 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue10 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue11 = new MyUI.PLC_MultiStateDisplay.TextValue();
            MyUI.PLC_MultiStateDisplay.TextValue textValue12 = new MyUI.PLC_MultiStateDisplay.TextValue();
            //textValue1.Name = "M5000";
            //textValue1.Text = "請登入身分...";
            //textValue1.字體 = new System.Drawing.Font("微軟正黑體", 15.75F);
            //textValue1.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue1.文字顏色 = System.Drawing.Color.Black;
            //textValue1.自定義參數 = false;
            //textValue2.Name = "M5001";
            //textValue2.Text = "登入者姓名 : XXX";
            //textValue2.字體 = new System.Drawing.Font("微軟正黑體", 30F);
            //textValue2.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue2.文字顏色 = System.Drawing.Color.Black;
            //textValue2.自定義參數 = true;
            //textValue3.Name = "M5002";
            //textValue3.Text = "登入失敗,查無此資料!";
            //textValue3.字體 = new System.Drawing.Font("微軟正黑體", 15.75F);
            //textValue3.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue3.文字顏色 = System.Drawing.Color.Red;
            //textValue3.自定義參數 = false;
            //textValue4.Name = "M5005";
            //textValue4.Text = "請選擇領/退藥";
            //textValue4.字體 = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold);
            //textValue4.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue4.文字顏色 = System.Drawing.Color.Red;
            //textValue4.自定義參數 = false;
            //textValue5.Name = "M5006";
            //textValue5.Text = "此藥單已領用過!";
            //textValue5.字體 = new System.Drawing.Font("微軟正黑體", 15.75F);
            //textValue5.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue5.文字顏色 = System.Drawing.Color.Red;
            //textValue5.自定義參數 = false;
            //textValue6.Name = "M5007";
            //textValue6.Text = "掃碼失敗!";
            //textValue6.字體 = new System.Drawing.Font("微軟正黑體", 15.75F);
            //textValue6.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue6.文字顏色 = System.Drawing.Color.Red;
            //textValue6.自定義參數 = false;
            //this.plC_MultiStateDisplay_領藥台_02_狀態顯示.狀態內容.Add(textValue1);
            //this.plC_MultiStateDisplay_領藥台_02_狀態顯示.狀態內容.Add(textValue2);
            //this.plC_MultiStateDisplay_領藥台_02_狀態顯示.狀態內容.Add(textValue3);
            //this.plC_MultiStateDisplay_領藥台_02_狀態顯示.狀態內容.Add(textValue4);
            //this.plC_MultiStateDisplay_領藥台_02_狀態顯示.狀態內容.Add(textValue5);
            //this.plC_MultiStateDisplay_領藥台_02_狀態顯示.狀態內容.Add(textValue6);
            //textValue7.Name = "M4000";
            //textValue7.Text = "請登入身分...";
            //textValue7.字體 = new System.Drawing.Font("微軟正黑體", 15.75F);
            //textValue7.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue7.文字顏色 = System.Drawing.Color.Black;
            //textValue7.自定義參數 = false;
            //textValue8.Name = "M4001";
            //textValue8.Text = "登入者姓名 : XXX";
            //textValue8.字體 = new System.Drawing.Font("微軟正黑體", 30F);
            //textValue8.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue8.文字顏色 = System.Drawing.Color.Black;
            //textValue8.自定義參數 = true;
            //textValue9.Name = "M4002";
            //textValue9.Text = "登入失敗,查無此資料!";
            //textValue9.字體 = new System.Drawing.Font("微軟正黑體", 15.75F);
            //textValue9.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue9.文字顏色 = System.Drawing.Color.Red;
            //textValue9.自定義參數 = false;
            //textValue10.Name = "M4005";
            //textValue10.Text = "請選擇領/退藥";
            //textValue10.字體 = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold);
            //textValue10.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue10.文字顏色 = System.Drawing.Color.Red;
            //textValue10.自定義參數 = false;
            //textValue11.Name = "M4006";
            //textValue11.Text = "此藥單已領用過!";
            //textValue11.字體 = new System.Drawing.Font("微軟正黑體", 15.75F);
            //textValue11.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue11.文字顏色 = System.Drawing.Color.Red;
            //textValue11.自定義參數 = false;
            //textValue12.Name = "M4007";
            //textValue12.Text = "掃碼失敗!";
            //textValue12.字體 = new System.Drawing.Font("微軟正黑體", 15.75F);
            //textValue12.文字對齊方式 = MyUI.PLC_MultiStateDisplay.TextValue.Alignment.Left;
            //textValue12.文字顏色 = System.Drawing.Color.Red;
            //textValue12.自定義參數 = false;
            //this.plC_MultiStateDisplay_領藥台_01_狀態顯示.狀態內容.Add(textValue7);
            //this.plC_MultiStateDisplay_領藥台_01_狀態顯示.狀態內容.Add(textValue8);
            //this.plC_MultiStateDisplay_領藥台_01_狀態顯示.狀態內容.Add(textValue9);
            //this.plC_MultiStateDisplay_領藥台_01_狀態顯示.狀態內容.Add(textValue10);
            //this.plC_MultiStateDisplay_領藥台_01_狀態顯示.狀態內容.Add(textValue11);
            //this.plC_MultiStateDisplay_領藥台_01_狀態顯示.狀態內容.Add(textValue12);
            #endregion
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.DesignMode == false)
            {
                MyMessageBox.form = this.FindForm();
                Dialog_交易紀錄明細.form = this.FindForm();
                Dialog_NumPannel.form = this.FindForm();
                Dialog_輸入批號.form = this.FindForm();
                Dialog_輸入效期.form = this.FindForm();
                Dialog_輸入藥品碼.form = this.FindForm();
                Dialog_手輸醫令.form = this.FindForm();
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
                Dialog_調劑作業_病歷號輸入.form = this.FindForm();
                Dialog_藥檔維護.form = this.FindForm();
                Dialog_錯誤提示.form = this.FindForm();
                Dialog_共用區設置.form = this.FindForm();
                Dialog_AlarmForm.form = this.FindForm();

                LoadDBConfig();
                LoadMyConfig();

                ApiServerSetting();

                this.stopwatch.Start();
                
                this.FormText = this.Text;
                this.WindowState = FormWindowState.Maximized;

                this.plC_UI_Init.Run(this.FindForm(), this.lowerMachine_Panel);
                this.plC_UI_Init.音效 = false;
                this.plC_UI_Init.全螢幕顯示 = true;

                Basic.Keyboard.Hook.KeyDown += Hook_KeyDown;
                Basic.Keyboard.Hook.MouseDown += Hook_MouseDown;
                Basic.Keyboard.Hook.MouseUp += Hook_MouseUp;
                this.button_調劑台切換.Click += Button_調劑台切換_Click;
                Basic.MyMessageBox.音效 = false;
                string ProcessName = "WINWORD";//換成想要結束的進程名字
                System.Diagnostics.Process[] MyProcess = System.Diagnostics.Process.GetProcessesByName(ProcessName);
                for (int i = 0; i < MyProcess.Length; i++)
                {
                    MyProcess[i].Kill();
                }
                printerClass.Init();
                printerClass.PrintPageEvent += PrinterClass_PrintPageEvent;

                this.plC_UI_Init.UI_Finished_Event += PlC_UI_Init_UI_Finished_Event;

            }

        }
        private void PlC_UI_Init_UI_Finished_Event()
        {
            this.PLC = this.lowerMachine_Panel.GetlowerMachine();
            
            PLC_Device_主機輸出模式.Bool = myConfigClass.主機輸出模式;
            PLC_Device_主機扣賬模式.Bool = myConfigClass.主機扣帳模式;
            PLC_Device_掃碼槍COM通訊.Bool = myConfigClass.掃碼槍COM通訊;
            PLC_Device_藥物辨識圖片顯示.Bool = myConfigClass.藥物辨識圖片顯示;
       


            if (myConfigClass.Scanner01_COMPort.StringIsEmpty())
            {
                rJ_GroupBox_領藥台_01.Visible = false;
            }
            if (myConfigClass.Scanner02_COMPort.StringIsEmpty())
            {
                rJ_GroupBox_領藥台_02.Visible = false;
            }
            if (myConfigClass.Scanner03_COMPort.StringIsEmpty())
            {
                rJ_GroupBox_領藥台_03.Visible = false;
            }
            if (myConfigClass.Scanner04_COMPort.StringIsEmpty())
            {
                rJ_GroupBox_領藥台_04.Visible = false;
            }
            if(myConfigClass.Scanner03_COMPort.StringIsEmpty() && myConfigClass.Scanner04_COMPort.StringIsEmpty())
            {
                panel_領藥台_03_04.Visible = false;
            }
            if (dBConfigClass.Web_URL.StringIsEmpty()) plC_RJ_GroupBox_後台網址_QRCODE.Visible = false;
            else
            {
                pictureBox_後台網址_QRCODE.Image = H_Pannel_lib.Communication.CreateQRCode(dBConfigClass.Web_URL, pictureBox_後台網址_QRCODE.Width, pictureBox_後台網址_QRCODE.Height);
            }
            plC_Button_合併同藥品.Bool = false;
            if (this.ControlMode)
            {
                this.plC_RJ_GroupBox_調劑台切換.Visible = true;
                PLC_Device_主機輸出模式.Bool = false;
                PLC_Device_主機扣賬模式.Bool = false;
                PLC_Device_掃碼槍COM通訊.Bool = false;
                PLC_Device_藥物辨識圖片顯示.Bool = false;
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


            this.Program_Scanner_RS232_Init();
            this.Program_系統_Init();
          
            this.Program_醫令資料_Init();
            this.Program_藥品資料_藥檔資料_Init();

            this.Program_儲位管理_EPD583_Init();
            this.Program_儲位管理_EPD266_Init();
            this.Program_儲位管理_EPD1020_Init();
            this.Program_儲位管理_RowsLED_Init();
            this.Program_儲位管理_RFID_Init();
            this.Program_儲位管理_Pannel35_Init();


            if (!this.ControlMode) this.Program_調劑作業_Init();

         
            this.Program_藥品資料_儲位總庫存表_Init();
            this.Program_藥品資料_儲位效期表_Init();
            this.Program_藥品資料_管藥設定_Init();

            this.Program_雲端藥檔_Init();
            this.Program_人員資料_Init();
            this.Program_工程模式_Init();
            this.Program_交易記錄查詢_Init();
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
            this.Program_共用區_Init();

            this.sub_Program_盤點作業_定盤_Init();
            this.sub_Program_盤點作業_新增盤點_Init();
            this.sub_Program_盤點作業_單號查詢_Init();
            this.sub_Program_盤點作業_資料庫_Init();
            this.Program_取藥堆疊資料_Init();
            if (!this.ControlMode) this.Program_輸出入檢查_Init();
            this.Program_收支作業_Init();
            this.plC_UI_Init.Add_Method(this.sub_Program_Scanner_RS232);

            this.Program_異常通知_覆盤錯誤_Init();

            this.LoadConfig工程模式();

            if (!this.ControlMode) this.Function_取藥堆疊資料_刪除指定調劑台名稱母資料(領藥台_01名稱);
            if (!this.ControlMode) this.Function_取藥堆疊資料_刪除指定調劑台名稱母資料(領藥台_02名稱);

            this.drawerUI_EPD_1020.Set_UDP_WriteTime(10);
            this.drawerUI_EPD_583.Set_UDP_WriteTime(10);


            Task task = Task.Run(new Action(delegate
            {
                if (!this.ControlMode) Function_從SQL取得儲位到本地資料();
            }));
            if (!this.ControlMode) this.plC_ScreenPage_Main.SelecteTabText("調劑作業");
            flag_Init = true;
        }
        #region Event
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
            if(DPS_Name.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未選擇調劑台!");
                return;
            }
            this.ApiServerSetting(DPS_Name, "一般資料");
            this.DBConfigInit();
            MyMessageBox.ShowDialog("切換完成!");
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }
        #endregion
        #region Function
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

            this.drawerUI_EPD_583.Init(dBConfigClass.DB_Basic.DataBaseName, dBConfigClass.DB_Basic.UserName, dBConfigClass.DB_Basic.Password, dBConfigClass.DB_Basic.IP, dBConfigClass.DB_Basic.Port, dBConfigClass.DB_Basic.MySqlSslMode);
            this.drawerUI_EPD_1020.Init(dBConfigClass.DB_Basic.DataBaseName, dBConfigClass.DB_Basic.UserName, dBConfigClass.DB_Basic.Password, dBConfigClass.DB_Basic.IP, dBConfigClass.DB_Basic.Port, dBConfigClass.DB_Basic.MySqlSslMode);
            this.storageUI_EPD_266.Init(dBConfigClass.DB_Basic.DataBaseName, dBConfigClass.DB_Basic.UserName, dBConfigClass.DB_Basic.Password, dBConfigClass.DB_Basic.IP, dBConfigClass.DB_Basic.Port, dBConfigClass.DB_Basic.MySqlSslMode);
            this.rowsLEDUI.Init(dBConfigClass.DB_Basic.DataBaseName, dBConfigClass.DB_Basic.UserName, dBConfigClass.DB_Basic.Password, dBConfigClass.DB_Basic.IP, dBConfigClass.DB_Basic.Port, dBConfigClass.DB_Basic.MySqlSslMode);
            this.rfiD_UI.Init(dBConfigClass.DB_Basic.DataBaseName, dBConfigClass.DB_Basic.UserName, dBConfigClass.DB_Basic.Password, dBConfigClass.DB_Basic.IP, dBConfigClass.DB_Basic.Port, dBConfigClass.DB_Basic.MySqlSslMode);
            this.storageUI_WT32.Init(dBConfigClass.DB_Basic.DataBaseName, dBConfigClass.DB_Basic.UserName, dBConfigClass.DB_Basic.Password, dBConfigClass.DB_Basic.IP, dBConfigClass.DB_Basic.Port, dBConfigClass.DB_Basic.MySqlSslMode);
            if(flag_DBConfigInit == true)
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
            if(ControlMode)
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
            Console.WriteLine(json_result);
            returnData returnData = json_result.JsonDeserializet<returnData>();
            List<HIS_DB_Lib.ServerSettingClass> serverSettingClasses = returnData.Data.ObjToListClass<ServerSettingClass>();
            HIS_DB_Lib.ServerSettingClass serverSettingClass;
            ServerName = Name;
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, basicName);
            List<string> DPS_Names = (from temp in serverSettingClasses
                                      where temp.類別 == enum_ServerSetting_Type.調劑台.GetEnumName()
                                      select temp.設備名稱).Distinct().ToList();
            if (!flag_Init) comboBox_調劑台名稱.DataSource = DPS_Names;

            if (serverSettingClass != null)
            {
                dBConfigClass.DB_Basic.IP = serverSettingClass.Server;
                dBConfigClass.DB_Basic.Port = (uint)(serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_Basic.DataBaseName = serverSettingClass.DBName;
                dBConfigClass.DB_Basic.UserName = serverSettingClass.User;
                dBConfigClass.DB_Basic.Password = serverSettingClass.Password;
            }
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, enum_ServerSetting_調劑台.人員資料);
            if (serverSettingClass != null)
            {
                dBConfigClass.DB_person_page.IP = serverSettingClass.Server;
                dBConfigClass.DB_person_page.Port = (uint)(serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_person_page.DataBaseName = serverSettingClass.DBName;
                dBConfigClass.DB_person_page.UserName = serverSettingClass.User;
                dBConfigClass.DB_person_page.Password = serverSettingClass.Password;
            }
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, enum_ServerSetting_調劑台.藥檔資料);
            if (serverSettingClass != null)
            {
                dBConfigClass.DB_Medicine_Cloud.IP = serverSettingClass.Server;
                dBConfigClass.DB_Medicine_Cloud.Port = (uint)(serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_Medicine_Cloud.DataBaseName = serverSettingClass.DBName;
                dBConfigClass.DB_Medicine_Cloud.UserName = serverSettingClass.User;
                dBConfigClass.DB_Medicine_Cloud.Password = serverSettingClass.Password;
            }
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, "醫囑資料");
            if (serverSettingClass != null)
            {
                dBConfigClass.DB_order_list.IP = serverSettingClass.Server;
                dBConfigClass.DB_order_list.Port = (uint)(serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_order_list.DataBaseName = serverSettingClass.DBName;
                dBConfigClass.DB_order_list.UserName = serverSettingClass.User;
                dBConfigClass.DB_order_list.Password = serverSettingClass.Password;
            }
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, enum_ServerSetting_調劑台.交易紀錄資料);
            if (serverSettingClass != null)
            {
                dBConfigClass.DB_tradding.IP = serverSettingClass.Server;
                dBConfigClass.DB_tradding.Port = (uint)(serverSettingClass.Port.StringToInt32());
                dBConfigClass.DB_tradding.DataBaseName = serverSettingClass.DBName;
                dBConfigClass.DB_tradding.UserName = serverSettingClass.User;
                dBConfigClass.DB_tradding.Password = serverSettingClass.Password;
            }
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, enum_ServerSetting_調劑台.API02);
            if (serverSettingClass != null)
            {
                dBConfigClass.Api_URL = serverSettingClass.Server;
                API_Server = serverSettingClass.Server;
            }
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, enum_ServerSetting_調劑台.Order_API);
            if (serverSettingClass != null) dBConfigClass.OrderApiURL = serverSettingClass.Server;
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, "Order_By_Code_API");
            if (serverSettingClass != null) dBConfigClass.OrderByCodeApiURL = serverSettingClass.Server;
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, enum_ServerSetting_調劑台.Med_API);
            if (serverSettingClass != null) dBConfigClass.MedApiURL = serverSettingClass.Server;
            serverSettingClass = serverSettingClasses.MyFind(Name, enum_ServerSetting_Type.調劑台, enum_ServerSetting_調劑台.Website);
            if (serverSettingClass != null) dBConfigClass.Web_URL = serverSettingClass.Server;
            serverSettingClass = serverSettingClasses.MyFind("Main", enum_ServerSetting_Type.網頁, enum_ServerSetting_網頁.API_Login);
            if (serverSettingClass != null) dBConfigClass.Login_URL = serverSettingClass.Server;


            OrderByCodeApi_URL = dBConfigClass.OrderByCodeApiURL;
            Order_URL = dBConfigClass.OrderApiURL;
            if(OrderByCodeApi_URL.StringIsEmpty())
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




    }


}
