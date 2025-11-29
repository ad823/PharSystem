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
[assembly: AssemblyVersion("1.0.25.11291")]
[assembly: AssemblyFileVersion("1.0.25.11291")]
namespace 智能藥庫系統
{
    public partial class Main_Form : Form
    {
        public static PLC_Device PLC_Device_主機模式 = new PLC_Device("S1050");

        public static string 登入者名稱 = "";

        public static string API_Server = "http://127.0.0.1:4433";
        public static string ServerType = "藥庫";
        public static string ServerName = "http://127.0.0.1:4433";
        public PLC_Device PLC_Device_已登入 = new PLC_Device("S4000");
        public static string currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public static StorageUI_EPD_266 _storageUI_EPD_266 = null;
        public static DrawerUI_EPD_583 _drawerUI_EPD_583 = null;
        public static H_Pannel_lib.RFID_UI _rFID_UI = null;
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
            private string api_get_full_inv_cmb_DataTable_by_SN = "";

            public string Name { get => name; set => name = value; }
            public string Api_Server { get => api_Server; set => api_Server = value; }

            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_Basic { get => dB_Basic; set => dB_Basic = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_VM { get => dB_VM; set => dB_VM = value; }
            [JsonIgnore]
            public SQL_DataGridView.ConnentionClass DB_儲位資料 { get => dB_儲位資料; set => dB_儲位資料 = value; }
            [JsonIgnore]
            public string OrderApiURL { get => orderApiURL; set => orderApiURL = value; }
            [JsonIgnore]
            public string Api_get_full_inv_cmb_DataTable_by_SN { get => api_get_full_inv_cmb_DataTable_by_SN; set => api_get_full_inv_cmb_DataTable_by_SN = value; }
        }
        private void LoadDBConfig()
        {

            //this.LoadcommandLineArgs();
            string jsonstr = MyFileStream.LoadFileAllText($"{DBConfigFileName}");

            Console.WriteLine($"[LoadDBConfig] path : {DBConfigFileName} ");
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

        public Main_Form()
        {       
            InitializeComponent();
            this.Load += Main_Form_Load;
            plC_UI_Init.UI_Finished_Event += PlC_UI_Init_UI_Finished_Event;

            this.plC_RJ_Button_庫存查詢.MouseDownEvent += PlC_RJ_Button_庫存查詢_MouseDownEvent;
            this.plC_RJ_Button_儲位管理.MouseDownEvent += PlC_RJ_Button_儲位管理_MouseDownEvent;
            this.plC_RJ_Button_驗收管理.MouseDownEvent += PlC_RJ_Button_驗收管理_MouseDownEvent;
            this.plC_RJ_Button_申領.MouseDownEvent += PlC_RJ_Button_申領_MouseDownEvent;
            this.plC_RJ_Button_盤點管理_匯入表單.MouseDownEvent += PlC_RJ_Button_盤點管理_匯入表單_MouseDownEvent;
            this.plC_RJ_Button_盤點管理_表單管理.MouseDownEvent += PlC_RJ_Button_盤點管理_表單管理_MouseDownEvent;
            this.plC_RJ_Button_盤點管理_表單合併.MouseDownEvent += PlC_RJ_Button_盤點管理_表單合併_MouseDownEvent;

            Form_Menu_Init();
        }

    

        private void Main_Form_Load(object sender, EventArgs e)
        {
            MyMessageBox.音效 = false;
            MyMessageBox.form = this.FindForm();
            LoadingForm.form = this.FindForm();
            MyDialog.form = this.FindForm();
            Dialog_儲位管理.form = this.FindForm();
            Dialog_申領.form = this.FindForm();
            Dialog_盤點單匯入.form = this.FindForm();
            Dialog_盤點明細.form = this.FindForm();
            Dialog_盤點單合併.form = this.FindForm();
            Dialog_盤點單合併_庫存設定.form = this.FindForm();
            Dialog_盤點單合併_單價設定.form = this.FindForm();
            this.WindowState = FormWindowState.Maximized;

            LoadDBConfig();
            LoadMyConfig();

       

            ApiServerSetting(dBConfigClass.Name);
            this.storageUI_EPD_266.Init(dBConfigClass.DB_儲位資料);
            this.drawerUI_EPD_583.Init(dBConfigClass.DB_儲位資料);
            this.rfiD_UI.Init(dBConfigClass.DB_儲位資料);

            byte[] bytes_emg_voice = materialRequisitionClass.emg_voice_download(API_Server);
            bytes_emg_voice.SaveFileStream($"{currentDirectory}\\緊急申領.wav");

            byte[] bytes_normal_voice = materialRequisitionClass.normal_voice_download(API_Server);
            bytes_normal_voice.SaveFileStream($"{currentDirectory}\\一般申領.wav");

            this.plC_UI_Init.Run(this.FindForm(), lowerMachine_Panel);
        }
        private void PlC_UI_Init_UI_Finished_Event()
        {
            _storageUI_EPD_266 = this.storageUI_EPD_266;
            _drawerUI_EPD_583 = this.drawerUI_EPD_583;
            _rFID_UI = this.rfiD_UI;
            PLC_UI_Init.Set_PLC_ScreenPage(panel_main, this.plC_ScreenPage_main);
            PLC_UI_Init.Set_PLC_ScreenPage(plC_RJ_ScreenButtonEx_主畫面, this.plC_ScreenPage_main);

            PLC_Device_主機模式.Bool = myConfigClass.主機模式;

            Program_系統_Init();
            Program_藥品區域_Init();
            Program_堆疊資料_Init();
            Program_交易紀錄查詢_Init();
            Program_申領_Init();
        }

        private void PlC_RJ_Button_儲位管理_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_儲位管理 dialog_儲位管理 = Dialog_儲位管理.GetForm();
            this.Invoke(new Action(delegate
            {
                dialog_儲位管理.ShowChildForm(panel_MainForm);
            }));

        }
        private void PlC_RJ_Button_庫存查詢_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_庫存查詢 dialog_庫存查詢 = Dialog_庫存查詢.GetForm();
            this.Invoke(new Action(delegate
            {
                dialog_庫存查詢.ShowChildForm(panel_MainForm);
            }));
        }
        private void PlC_RJ_Button_驗收管理_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_驗收管理 dialog_驗收管理 = Dialog_驗收管理.GetForm();
            this.Invoke(new Action(delegate
            {
                dialog_驗收管理.ShowChildForm(panel_MainForm);
            }));
        }
        private void PlC_RJ_Button_申領_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_申領 dialog_申領 = Dialog_申領.GetForm();
            this.Invoke(new Action(delegate
            {
                dialog_申領.ShowChildForm(panel_MainForm);
            }));
        }
        private void PlC_RJ_Button_盤點管理_匯入表單_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_盤點單匯入 dialog_盤點單匯入 = new Dialog_盤點單匯入();
            dialog_盤點單匯入.ShowDialog();
        }
        private void PlC_RJ_Button_盤點管理_表單管理_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_盤點單管理 dialog_盤點單管理 = new Dialog_盤點單管理();
            dialog_盤點單管理.ShowDialog();
        }
        private void PlC_RJ_Button_盤點管理_表單合併_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_盤點單合併 dialog_盤點單合併 = Dialog_盤點單合併.GetForm();
            this.Invoke(new Action(delegate
            {
                dialog_盤點單合併.ShowChildForm(panel_MainForm);
            }));
        }

        private void ApiServerSetting(string Name)
        {
            API_Server = dBConfigClass.Api_Server;
            Console.WriteLine($"[ApiServerSetting] url : {dBConfigClass.Api_Server}");
            Console.WriteLine($"[ApiServerSetting] name : {dBConfigClass.Name}");

            this.Text = $"智能藥庫系統 Ver{this.ProductVersion}";
            ServerName = Name;
            List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}/api/serversetting");
            Console.WriteLine($"[sys_serverSettingClassMethod.WebApiGet] url : {API_Server}/api/serversetting");
            Console.WriteLine($"[sys_serverSettingClassMethod.WebApiGet] 取得<{sys_serverSettingClasses.Count}>筆資料");

            sys_serverSettingClass sys_serverSettingClass_一般資料 = sys_serverSettingClasses.myFind(Name, "藥庫", "一般資料");
            sys_serverSettingClass sys_serverSettingClass_儲位資料 = sys_serverSettingClasses.myFind(Name, "藥庫", "儲位資料");
            sys_serverSettingClass sys_serverSettingClass_VM端 = sys_serverSettingClasses.myFind(Name, "藥庫", "VM端");


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

            sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses.myFind("Main", "網頁", "get_full_inv_cmb_DataTable_by_SN");
            if (sys_serverSettingClass != null) dBConfigClass.Api_get_full_inv_cmb_DataTable_by_SN = sys_serverSettingClass.Server;
        }
    }
}
