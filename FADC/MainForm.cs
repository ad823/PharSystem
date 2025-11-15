using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basic;
using MyUI;
using MinasA6DLL;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using SQLUI;

[assembly: AssemblyVersion("1.0.0.0000")]
[assembly: AssemblyFileVersion("1.0.25.0000")]
namespace FADC
{
    public partial class MainForm : Form
    {
        public bool ControlMode = false;
        public static MinasA6 minasA6 = null;
        public static string currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        #region MyConfigClass
        private static string MyConfigFileName = $@"{currentDirectory}\MyConfig.txt";
        static public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {

            private bool controlMode = false;
            private string servoZ_Com = "COM1";
            private string board_IP = "";

            public bool ControlMode { get => controlMode; set => controlMode = value; }
            public string ServoZ_Com { get => servoZ_Com; set => servoZ_Com = value; }
            public string Board_IP { get => board_IP; set => board_IP = value; }
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
        public MainForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            this.Load += MainFrom_Load;
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
        private void MainFrom_Load(object sender, EventArgs e)
        {
            H_Pannel_lib.Communication.ConsoleWrite = false;
            MyMessageBox.音效 = false;
            MyMessageBox.form = this.FindForm();

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

            LoadMyConfig();
            LoadDBConfig();

            Program_PLC();
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
    }
}
