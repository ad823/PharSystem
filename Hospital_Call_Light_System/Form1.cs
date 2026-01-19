using System;
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
using HIS_DB_Lib;
using System.Reflection;
using System.Runtime.InteropServices;
[assembly: AssemblyVersion("1.0.0.10")]
[assembly: AssemblyFileVersion("1.0.0.10")]
namespace Hospital_Call_Light_System
{

    public partial class Form1 : System.Windows.Forms.Form
    {
        private GlobalKeyboardScanner _scanner;
        public enum enum_顯示方式
        {
            號碼,
            圖片,
            不顯示
        }
        public static string currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static Voice voice = new Voice();
        OpenFileDialog openFileDialog_LoadImage = new OpenFileDialog();
        private string last_keyData = "";
        private bool 全螢幕 = false;
        MyTimer myTimer_ESC = new MyTimer();
        MyTimer myTimer_ProcessCmdKey = new MyTimer();
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (plC_ScreenPage_Main.PageText == "設定")
            {
                if (rJ_TextBox_第一台_加一號.IsFocused) rJ_TextBox_第一台_加一號.Text = keyData.ToString();
                if (rJ_TextBox_第一台_減一號.IsFocused) rJ_TextBox_第一台_減一號.Text = keyData.ToString();
                if (rJ_TextBox_第一台_加二號.IsFocused) rJ_TextBox_第一台_加二號.Text = keyData.ToString();
                if (rJ_TextBox_第一台_減二號.IsFocused) rJ_TextBox_第一台_減二號.Text = keyData.ToString();
                if (rJ_TextBox_第一台_加十號.IsFocused) rJ_TextBox_第一台_加十號.Text = keyData.ToString();
                if (rJ_TextBox_第一台_減十號.IsFocused) rJ_TextBox_第一台_減十號.Text = keyData.ToString();

                if (rJ_TextBox_第二台_加一號.IsFocused) rJ_TextBox_第二台_加一號.Text = keyData.ToString();
                if (rJ_TextBox_第二台_減一號.IsFocused) rJ_TextBox_第二台_減一號.Text = keyData.ToString();
                if (rJ_TextBox_第二台_加二號.IsFocused) rJ_TextBox_第二台_加二號.Text = keyData.ToString();
                if (rJ_TextBox_第二台_減二號.IsFocused) rJ_TextBox_第二台_減二號.Text = keyData.ToString();
                if (rJ_TextBox_第二台_加十號.IsFocused) rJ_TextBox_第二台_加十號.Text = keyData.ToString();
                if (rJ_TextBox_第二台_減十號.IsFocused) rJ_TextBox_第二台_減十號.Text = keyData.ToString();
            }
            if (plC_ScreenPage_Main.PageText == "主畫面")
            {
                if (keyData.ToString() == "Escape")
                {
                    myTimer_ESC.StartTickTime(1000);
                    if (myTimer_ESC.IsTimeOut())
                    {
                      
             
                        return base.ProcessCmdKey(ref msg, keyData);
                    }

                }
                else
                {
                    myTimer_ESC.IsTimeOut();
                }
 
            }
            else
            {
            }
            last_keyData = keyData.ToString();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Dialog_小叫號台_FormClosingEvent()
        {
            Basic.Screen.FullScreen(this.FindForm(), 0, true);
            Basic.Screen.FullScreen(this.FindForm(), 0, false);
            panel_Main.Visible = true;
        }

        private string keyData_buf = "";
        private void Hook_KeyUp(int nCode, IntPtr wParam, Keys Keys)
        {
            if(keyData_buf != Keys.ToString())
            {
                keyData_buf = Keys.ToString();

                if (radioButton_一號台_號碼.Checked) Function_第一台_號碼增減檢查(myConfigClass.一號台名稱, Keys.ToString());
                if (radioButton_二號台_號碼.Checked) Function_第二台_號碼增減檢查(myConfigClass.二號台名稱, Keys.ToString());
            }
        }
        private void Hook_KeyDown(int nCode, IntPtr wParam, Keys Keys)
        {
            keyData_buf = "";
        }
        private void Function_第一台_號碼增減檢查(string 機台名稱, string keyData)
        {
            if (radioButton_一號台_號碼.Checked == false)
            {
                return;
            }
            bool flag_replace = false;
            if (myConfigClass.第一台加一號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num + 1).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第一台減一號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();
                if ((num - 1) < 0) return;
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num - 1).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第一台加二號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num + 2).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第一台減二號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();
                if ((num - 2) < 0) return;
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num - 2).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第一台加十號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num + 10).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第一台減十號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();
                if ((num - 2) < 0) return;
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num - 10).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
        }
        private void Function_第二台_號碼增減檢查(string 機台名稱, string keyData)
        {
            bool flag_replace = false;
            if (radioButton_二號台_號碼.Checked == false)
            {
                return;
            }
            if (myConfigClass.第二台加一號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num + 1).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第二台減一號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();

                if ((num - 1) < 0) return;
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num - 1).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第二台加二號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();

                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num + 2).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第二台減二號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();

                if ((num - 2) < 0) return;
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num - 2).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第二台加十號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();

                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num + 10).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }
            if (myConfigClass.第二台減十號 == keyData)
            {
                List<object[]> list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                list_叫號內容設定 = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                if (list_叫號內容設定.Count == 0) return;
                int num = list_叫號內容設定[0][(int)enum_叫號內容設定.號碼].StringToInt32();

                if ((num - 2) < 0) return;
                list_叫號內容設定[0][(int)enum_叫號內容設定.號碼] = (num - 10).ToString("0000");
                flag_replace = true;
                if (flag_replace) this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_叫號內容設定, false);
            }

        }

        #region DBConfigClass
        private static string DBConfigFileName = $@"{currentDirectory}\DBConfig.txt";
        public DBConfigClass dBConfigClass = new DBConfigClass();
        public class DBConfigClass
        {
            private SQL_DataGridView.ConnentionClass dB_Basic = new SQL_DataGridView.ConnentionClass();
            private string orderApiURL = "";

            public SQL_DataGridView.ConnentionClass DB_Basic { get => dB_Basic; set => dB_Basic = value; }
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
        public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {
            private string scanner01_COMPort = "COM1";
            private bool _鍵盤掃碼模式 = true;
            private enum_顯示方式 _一號台_顯示方式 = enum_顯示方式.號碼;
            private enum_顯示方式 _二號台_顯示方式 = enum_顯示方式.號碼;
            private int _一號台_顯示圖片控制 = 0;
            private int _二號台_顯示圖片控制 = 0;

            private string _一號台名稱 = "";
            private string _二號台名稱 = "";

            private bool _本地音效 = false;
            private bool _全局音效 = false;
            private bool _推撥語音至資料庫 = false;
            
            private string _公告名稱 = "";

            private string _第一台加一號 = "";
            private string _第一台減一號 = "";
            private string _第一台加二號 = "";
            private string _第一台減二號 = "";
            private string _第一台加十號 = "";
            private string _第一台減十號 = "";

            private string _第二台加一號 = "";
            private string _第二台減一號 = "";
            private string _第二台加二號 = "";
            private string _第二台減二號 = "";
            private string _第二台加十號 = "";
            private string _第二台減十號 = "";

            public string 一號台名稱 { get => _一號台名稱; set => _一號台名稱 = value; }
            public string 二號台名稱 { get => _二號台名稱; set => _二號台名稱 = value; }
            public string 第一台加一號 { get => _第一台加一號; set => _第一台加一號 = value; }
            public string 第一台減一號 { get => _第一台減一號; set => _第一台減一號 = value; }
            public string 第一台加二號 { get => _第一台加二號; set => _第一台加二號 = value; }
            public string 第一台減二號 { get => _第一台減二號; set => _第一台減二號 = value; }
            public string 第二台加一號 { get => _第二台加一號; set => _第二台加一號 = value; }
            public string 第二台減一號 { get => _第二台減一號; set => _第二台減一號 = value; }
            public string 第二台加二號 { get => _第二台加二號; set => _第二台加二號 = value; }
            public string 第二台減二號 { get => _第二台減二號; set => _第二台減二號 = value; }
            public string 第一台加十號 { get => _第一台加十號; set => _第一台加十號 = value; }
            public string 第一台減十號 { get => _第一台減十號; set => _第一台減十號 = value; }
            public string 第二台加十號 { get => _第二台加十號; set => _第二台加十號 = value; }
            public string 第二台減十號 { get => _第二台減十號; set => _第二台減十號 = value; }
            public bool 本地音效 { get => _本地音效; set => _本地音效 = value; }
            public bool 全局音效 { get => _全局音效; set => _全局音效 = value; }
            public string 公告名稱 { get => _公告名稱; set => _公告名稱 = value; }
            public enum_顯示方式 一號台_顯示方式 { get => _一號台_顯示方式; set => _一號台_顯示方式 = value; }
            public enum_顯示方式 二號台_顯示方式 { get => _二號台_顯示方式; set => _二號台_顯示方式 = value; }
            public int 一號台_顯示圖片控制 { get => _一號台_顯示圖片控制; set => _一號台_顯示圖片控制 = value; }
            public int 二號台_顯示圖片控制 { get => _二號台_顯示圖片控制; set => _二號台_顯示圖片控制 = value; }
            public string Scanner01_COMPort { get => scanner01_COMPort; set => scanner01_COMPort = value; }
            public bool 鍵盤掃碼模式 { get => _鍵盤掃碼模式; set => _鍵盤掃碼模式 = value; }
            public bool 推撥語音至資料庫 { get => _推撥語音至資料庫; set => _推撥語音至資料庫 = value; }
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
        }

        private void Form_Load(object sender, EventArgs e)
        {

            openFileDialog_LoadImage.Title = "選擇圖片"; // 設定對話框標題
            openFileDialog_LoadImage.Filter = "圖片檔案|*.jpg;*.jpeg;*.png;*.gif"; // 設定可選擇的檔案類型
            openFileDialog_LoadImage.Multiselect = true; // 僅允許單一檔案選擇

            MyMessageBox.音效 = false;
            MyMessageBox.form = this.FindForm();
            Dialog_螢幕選擇.form = this.FindForm();
            Dialog_NumPannel.form = this.FindForm();
            this.LoadDBConfig();
            this.LoadMyConfig();

            this.plC_RJ_Button_全螢幕顯示.MouseDownEvent += PlC_RJ_Button_全螢幕顯示_MouseDownEvent;
            this.plC_UI_Init.Run(this.FindForm(), this.lowerMachine_Panel);
            this.plC_UI_Init.掃描速度 = 10;
            this.plC_UI_Init.UI_Finished_Event += PlC_UI_Init_UI_Finished_Event;
            myTimer_ProcessCmdKey.StartTickTime(200);
            this.Text += $" Ver{this.ProductVersion}";
            Dialog_小叫號台.FormClosingEvent += Dialog_小叫號台_FormClosingEvent;
            Basic.Keyboard.Hook.KeyDown += Hook_KeyDown;
            Basic.Keyboard.Hook.KeyUp += Hook_KeyUp;
            this.rJ_TextBox_第一台_加一號.Text = myConfigClass.第一台加一號;
            this.rJ_TextBox_第一台_減一號.Text = myConfigClass.第一台減一號;
            this.rJ_TextBox_第一台_加二號.Text = myConfigClass.第一台加二號;
            this.rJ_TextBox_第一台_減二號.Text = myConfigClass.第一台減二號;
            this.rJ_TextBox_第一台_加十號.Text = myConfigClass.第一台加十號;
            this.rJ_TextBox_第一台_減十號.Text = myConfigClass.第一台減十號;

            this.rJ_TextBox_第二台_加一號.Text = myConfigClass.第二台加一號;
            this.rJ_TextBox_第二台_減一號.Text = myConfigClass.第二台減一號;
            this.rJ_TextBox_第二台_加二號.Text = myConfigClass.第二台加二號;
            this.rJ_TextBox_第二台_減二號.Text = myConfigClass.第二台減二號;
            this.rJ_TextBox_第二台_加十號.Text = myConfigClass.第二台加十號;
            this.rJ_TextBox_第二台_減十號.Text = myConfigClass.第二台減十號;

            this.checkBox_本地音效.Checked = myConfigClass.本地音效;
            this.checkBox_推撥語音至資料庫.Checked = myConfigClass.推撥語音至資料庫;
            this.checkBox_全局音效.Checked = myConfigClass.全局音效;

            _scanner = new GlobalKeyboardScanner(200);
            _scanner.OnScanCompleted += scan =>
            {
                Console.WriteLine($"掃碼完成: {scan}");
                byte[] data = Encoding.UTF8.GetBytes(scan);
                MySerialPort_Scanner01.SetReadByte(data);
            };

            _scanner.Start();
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scanner?.Dispose();
        }

        private void PlC_UI_Init_UI_Finished_Event()
        {
            PLC_UI_Init.Set_PLC_ScreenPage(this.panel_Main, this.plC_ScreenPage_Main);
            this.plC_ScreenPage_Main.TabChangeEvent += PlC_ScreenPage_Main_TabChangeEvent;

          
            this.Program_公告設定_Init();
            this.Program_樣式設定_Init();
            this.Program_叫號內容_Init();
            this.Program_顯示設定_Init();
            this.Program_圖片上傳_Init();
            this.Program_系統_Init();
            this.Program_設定_Init();
            this.Program_主畫面_Init();

            this.plC_UI_Init.Add_Method(sub_program);
            //this.WindowState = FormWindowState.Maximized;

        }
        static public MySerialPort MySerialPort_Scanner01 = new MySerialPort();
        private bool flag_mySerialPort = false;
        private void sub_program()
        {
            if(dBConfigClass.OrderApiURL.StringIsEmpty() == false)
            {
                MySerialPort_Scanner01.ConsoleWrite = true;
                if (myConfigClass.鍵盤掃碼模式 == false)
                {
                    if (myConfigClass.Scanner01_COMPort.StringIsEmpty() == false && flag_mySerialPort == false)
                    {
                        MySerialPort_Scanner01.Init(myConfigClass.Scanner01_COMPort, 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
                        MySerialPort_Scanner01.SerialPortOpen();
                        flag_mySerialPort = true;
                    }
                }
                   
                string text = Function_ReadBacodeScanner01();
                if (text == null) return;
                if (text.StringIsEmpty()) return;
                List<OrderClass> orderClasses = Function_醫令資料_API呼叫(dBConfigClass.OrderApiURL, text);
                if (orderClasses == null) return;
                if (orderClasses.Count == 0) return;
                string 領藥號 = orderClasses[0].領藥號;
                if (領藥號.StringIsInt32())
                {
                    if (領藥號.StringToInt32() > 0)
                    {
                        if (radioButton_一號台_號碼.Checked == false)
                        {
                            MyMessageBox.ShowDialog("第一台目前非叫號模式!");
                            return;
                        }
                        string 機台名稱 = this.comboBox_一號台名稱.GetComboBoxText();
                        List<object[]> list_value = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
                        list_value = list_value.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
                        if (list_value.Count == 0)
                        {
                            MyMessageBox.ShowDialog("找無資料!");
                            return;
                        }
                        list_value[0][(int)enum_叫號內容設定.號碼] = 領藥號.StringToInt32().ToString("0000");
                        this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_value, false);
                    }
                }
            }
        
        }
        public static string Function_ReadBacodeScanner01()
        {
            try
            {

                System.Threading.Thread.Sleep(200);
                string text = MySerialPort_Scanner01.ReadString();
                if (text == null) return null;
                text = text.Replace("\0", "");
                if (text.StringIsEmpty()) return null;
                if (text.Length <= 2 || text.Length > 200) return null;
                //if (PLC_Device_刷取藥單要檢查回車.Bool) if (text.Substring(text.Length - 2, 2) != "\r\n") return null;

                text = text.Replace("\r\n", "");
                return text;
            }
            catch (Exception ex)
            {
                Logger.Log("error", $"Function_ReadBacodeScanner01 : {ex.Message}");
                return null;
            }
            finally
            {
                MySerialPort_Scanner01.ClearReadByte();
            }

        }
        private void PlC_ScreenPage_Main_TabChangeEvent(string PageText)
        {
            if (PageText == "設定")
            {
                comboBox_樣式設定_代碼.SelectedIndex = 0;
                Function_樣式設定讀取(comboBox_樣式設定_代碼.Text);

                this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(true);
                comboBox_叫號內容設定_代碼.SelectedIndex = 0;
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
        public enum OrderAction
        {
            領藥 = 0, 退藥 = 1
        }
        static public List<OrderClass> Function_醫令資料_API呼叫(string url, string barcode, OrderAction action = OrderAction.領藥)
        {
            barcode = barcode.Replace("\r\n", "");
            barcode = Uri.EscapeDataString(barcode);
            List<OrderClass> orderClasses = new List<OrderClass>();
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            string apitext = $"{url}{barcode}&action={(int)action}";

            Console.Write($"Call api : {apitext}\n");
            string jsonString = Basic.Net.WEBApiGet(apitext);
            Console.Write($"{jsonString}\n");
            Console.Write($"耗時 {myTimer.ToString()}ms\n");
            if (jsonString.StringIsEmpty())
            {
                voice.SpeakOnTask("網路異常");
            
                return orderClasses;
            }
            returnData returnData = jsonString.JsonDeserializet<returnData>();
            if (returnData == null)
            {
                voice.SpeakOnTask("藥單條碼錯誤");
         
                return new List<OrderClass>();
            }
            if (returnData.Code == 100)
            {
                return new List<OrderClass>();
            }
            else if (returnData.Code != 200)
            {
              
                return new List<OrderClass>();

            }
            orderClasses = returnData.Data.ObjToListClass<OrderClass>();
            if (orderClasses == null)
            {
                Console.WriteLine($"串接資料傳回格式錯誤!");
                voice.SpeakOnTask("資料錯誤");
                orderClasses = new List<OrderClass>();

            }

            return orderClasses;
        }

 
    }
}
