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
using SQLUI;
using System.Text.Json.Serialization;
using System.Reflection;
using HIS_DB_Lib;
using H_Pannel_lib;
[assembly: AssemblyVersion("1.0.25.03031")]
[assembly: AssemblyFileVersion("1.0.25.03031")]
namespace 中藥調劑系統
{
    public partial class Main_Form : Form
    {
        public static string API_Server = "http://127.0.0.1:4433";
        public static string ServerType = "中藥調劑系統";
        public static string ServerName = "http://127.0.0.1:4433";
        public PLC_Device PLC_Device_已登入 = new PLC_Device("S4000");
        public static string currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private LadderConnection.LowerMachine PLC;
        public static RowsLEDUI _rowsLEDUI = null;
        public static StorageUI_EPD_266 _storageUI_EPD_266 = null;
        public Main_Form()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
            this.FormClosing += Main_Form_FormClosing;
            this.rJ_Button_磅秤扣重.MouseDownEvent += RJ_Button_磅秤扣重_MouseDownEvent;
            this.rJ_Button_磅秤歸零.MouseDownEvent += RJ_Button_磅秤歸零_MouseDownEvent;
            MyMessageBox.TimerEvent += MyMessageBox_TimerEvent;
        }

   

        private void RJ_Button_磅秤歸零_MouseDownEvent(MouseEventArgs mevent)
        {
            ExcelScaleLib_Port.set_to_zero();
        }
        private void RJ_Button_磅秤扣重_MouseDownEvent(MouseEventArgs mevent)
        {
            EXCELL_set_sub_current_weight();
        }

        #region DBConfigClass
        private static string DBConfigFileName = $@"{currentDirectory}\DBConfig.txt";
        static public DBConfigClass dBConfigClass = new DBConfigClass();
        public class DBConfigClass
        {
            private SQL_DataGridView.ConnentionClass dB_Basic = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_VM = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_儲位資料 = new SQL_DataGridView.ConnentionClass();
            private string name = "";
            private string api_Server = "";
            private string orderApiURL = "";
            private string orderTApiURL = "";

            public string Name { get => name; set => name = value; }
            public string Api_Server { get => api_Server; set => api_Server = value; }

            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_Basic { get => dB_Basic; set => dB_Basic = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_VM { get => dB_VM; set => dB_VM = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_儲位資料 { get => dB_儲位資料; set => dB_儲位資料 = value; }
            [JsonIgnore]
            public string OrderTApiURL { get => orderTApiURL; set => orderTApiURL = value; }
            public string OrderApiURL { get => orderApiURL; set => orderApiURL = value; }
        }
        private void LoadDBConfig()
        {

            //this.LoadcommandLineArgs();
            string jsonstr = MyFileStream.LoadFileAllText($"{DBConfigFileName}");

            Console.WriteLine($"[LoadDBConfig] path : {MyConfigFileName} ");
            Console.WriteLine($"[LoadDBConfig] jsonstr : {jsonstr} ");

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

            private bool _主機模式 = false;
            private bool _主機輸出模式 = false;
            
            private string rFID_COMPort = "COM1";
            private string scanner01_COMPort = "COM2";
            private string scanner02_COMPort = "COM3";
            private string scanner03_COMPort = "";
            private string scanner04_COMPort = "";
            private string sCALE_COMPort = "COM4";

            public bool 主機模式 { get => _主機模式; set => _主機模式 = value; }
            public bool 主機輸出模式 { get => _主機輸出模式; set => _主機輸出模式 = value; }
            public string RFID_COMPort { get => rFID_COMPort; set => rFID_COMPort = value; }
            public string Scanner01_COMPort { get => scanner01_COMPort; set => scanner01_COMPort = value; }
            public string Scanner02_COMPort { get => scanner02_COMPort; set => scanner02_COMPort = value; }
            public string Scanner03_COMPort { get => scanner03_COMPort; set => scanner03_COMPort = value; }
            public string Scanner04_COMPort { get => scanner04_COMPort; set => scanner04_COMPort = value; }
            public string SCALE_COMPort { get => sCALE_COMPort; set => sCALE_COMPort = value; }
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

        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Function_全部滅燈();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            //string ProcessName = "中藥調劑系統";//換成想要結束的進程名字
            //System.Diagnostics.Process[] MyProcess = System.Diagnostics.Process.GetProcessesByName(ProcessName);
            //for (int i = 0; i < MyProcess.Length; i++)
            //{
            //    MyProcess[i].Kill();
            //}
            LoadingForm.form = this.FindForm();
            Dialog_AlarmForm.form = this.FindForm();
            Dialog_藥品搜尋.form = this.FindForm();
            Dialog_其它處方.form = this.FindForm();

            MyDialog.form = this.FindForm();
            MyMessageBox.form = this.FindForm();
            MyMessageBox.音效 = false;
            this.plC_UI_Init.音效 = false;

            ExcelScaleLib.Communication.ConsoleWrite = false;
            this.plC_RJ_Button_儲位設定.MouseDownEvent += PlC_RJ_Button_儲位設定_MouseDownEvent;
            this.plC_RJ_Button_人員資料.MouseDownEvent += PlC_RJ_Button_人員資料_MouseDownEvent;
            this.plC_RJ_Button_強制滅燈.MouseDownEvent += PlC_RJ_Button_強制滅燈_MouseDownEvent;
            this.plC_RJ_Button_其它處方.MouseDownEvent += PlC_RJ_Button_其它處方_MouseDownEvent;
            plC_UI_Init.Run(this.FindForm(), this.lowerMachine_Panel);
            plC_UI_Init.UI_Finished_Event += PlC_UI_Init_UI_Finished_Event;
        }


        private void MyMessageBox_TimerEvent(MyMessageBox myMessageBox)
        {
            myTimer_MySerialPort_Scanner01.TickStop();
            myTimer_MySerialPort_Scanner01.StartTickTime(1000);
            flag_MySerialPort_Scanner01_enable = false;
            string text = MySerialPort_Scanner01.ReadString();
            if (text.StringIsEmpty()) return;
            System.Threading.Thread.Sleep(50);
            text = MySerialPort_Scanner01.ReadString();
            MySerialPort_Scanner01.ClearReadByte();

            myMessageBox.Close();
        }
        private void PlC_RJ_Button_其它處方_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_其它處方 dialog_其它處方 = new Dialog_其它處方();
            if (dialog_其它處方.ShowDialog() != DialogResult.Yes) return;
        }
        private void PlC_RJ_Button_強制滅燈_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("是否全部強制滅燈?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            LoadingForm.ShowLoadingForm();
            Function_全部滅燈();
            LoadingForm.CloseLoadingForm();
        }
        private void PlC_RJ_Button_人員資料_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Dialog_人員資料 dialog_人員資料 = new Dialog_人員資料();
                dialog_人員資料.ShowDialog();
            }));
        }
        private void PlC_RJ_Button_儲位設定_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                Dialog_儲位設定 dialog_儲位設定 = new Dialog_儲位設定();
                dialog_儲位設定.ShowDialog();
            }));
  
        }
        private void PlC_UI_Init_UI_Finished_Event()
        {
            this.WindowState = FormWindowState.Maximized;
            PLC = this.lowerMachine_Panel.GetlowerMachine();
            LoadDBConfig();
            LoadMyConfig();
    
            ApiServerSetting(dBConfigClass.Name);

            PLC_UI_Init.Set_PLC_ScreenPage(panel_main, this.plC_ScreenPage_main);
            PLC_Device_已登入.Bool = false;

            Program_藥品區域_Init();
            Program_調劑畫面_Init();
            Program_處方搜尋_Init();
            Program_交易紀錄_Init();
            Program_RS232_EXCELL_SCALE_Init();
            Program_RS232_Scanner_Init();
            Program_RFID_Init(); 
            Program_藥品搜尋_Init();
            Program_設定_Init();

            Function_Init();

            _rowsLEDUI = this.rowsLEDUI;
            this.rowsLEDUI.Init(dBConfigClass.DB_儲位資料);
            _storageUI_EPD_266 = this.storageUI_EPD_266;
            this.storageUI_EPD_266.Init(dBConfigClass.DB_儲位資料);
            if (plC_NumBox_亮度.Value == 0) plC_NumBox_亮度.Value = 80;
            RowsLEDUI.Lightness = (double)plC_NumBox_亮度.Value / 100D;
            Task.Run(new Action(delegate 
            {
                System.Threading.Thread.Sleep(3000);
                //Function_全部滅燈();
            }));
           
        }

        private void ApiServerSetting(string Name)
        {
            API_Server = dBConfigClass.Api_Server;
            Console.WriteLine($"[ApiServerSetting] url : {dBConfigClass.Api_Server}");
            Console.WriteLine($"[ApiServerSetting] name : {dBConfigClass.Name}");

            this.Text = $"中藥調劑系統 Ver{this.ProductVersion}";
            ServerName = Name;
            List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}/api/serversetting");
            Console.WriteLine($"[sys_serverSettingClassMethod.WebApiGet] url : {API_Server}/api/serversetting");
            Console.WriteLine($"[sys_serverSettingClassMethod.WebApiGet] 取得<{sys_serverSettingClasses.Count}>筆資料");

            sys_serverSettingClass sys_serverSettingClass_一般資料 = sys_serverSettingClasses.myFind(Name, "中藥調劑系統", "一般資料");
            sys_serverSettingClass sys_serverSettingClass_儲位資料 = sys_serverSettingClasses.myFind(Name, "中藥調劑系統", "儲位資料");
            sys_serverSettingClass sys_serverSettingClass_VM端 = sys_serverSettingClasses.myFind(Name, "中藥調劑系統", "VM端");
            sys_serverSettingClass sys_serverSettingClass_OrderT_API = sys_serverSettingClasses.myFind(Name, "中藥調劑系統", "OrderT_API");
            sys_serverSettingClass sys_serverSettingClass_Order_API = sys_serverSettingClasses.myFind(Name, "中藥調劑系統", "Order_API");


            if (sys_serverSettingClass_一般資料 != null)
            {
                dBConfigClass.DB_Basic.IP = sys_serverSettingClass_一般資料.Server;
                dBConfigClass.DB_Basic.Port = (uint)(sys_serverSettingClass_一般資料.Port.StringToInt32());
                dBConfigClass.DB_Basic.DataBaseName = sys_serverSettingClass_一般資料.DBName;
                dBConfigClass.DB_Basic.UserName = sys_serverSettingClass_一般資料.User;
                dBConfigClass.DB_Basic.Password = sys_serverSettingClass_一般資料.Password;
            }
            if (sys_serverSettingClass_VM端 != null)
            {
                dBConfigClass.DB_VM.IP = sys_serverSettingClass_VM端.Server;
                dBConfigClass.DB_VM.Port = (uint)(sys_serverSettingClass_VM端.Port.StringToInt32());
                dBConfigClass.DB_VM.DataBaseName = sys_serverSettingClass_VM端.DBName;
                dBConfigClass.DB_VM.UserName = sys_serverSettingClass_VM端.User;
                dBConfigClass.DB_VM.Password = sys_serverSettingClass_VM端.Password;
            }
            if (sys_serverSettingClass_儲位資料 != null)
            {
                dBConfigClass.DB_儲位資料.IP = sys_serverSettingClass_儲位資料.Server;
                dBConfigClass.DB_儲位資料.Port = (uint)(sys_serverSettingClass_儲位資料.Port.StringToInt32());
                dBConfigClass.DB_儲位資料.DataBaseName = sys_serverSettingClass_儲位資料.DBName;
                dBConfigClass.DB_儲位資料.UserName = sys_serverSettingClass_儲位資料.User;
                dBConfigClass.DB_儲位資料.Password = sys_serverSettingClass_儲位資料.Password;
            }
            if (sys_serverSettingClass_OrderT_API != null) dBConfigClass.OrderTApiURL = sys_serverSettingClass_OrderT_API.Server;
            if (sys_serverSettingClass_Order_API != null) dBConfigClass.OrderApiURL = sys_serverSettingClass_Order_API.Server;
        }
    }
}


