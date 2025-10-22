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


[assembly: AssemblyVersion("1.0.0.0000")]
[assembly: AssemblyFileVersion("1.0.25.0000")]
namespace FADC
{
    public partial class MainForm : Form
    {
        public static MinasA6 minasA6 = null;
        public static string currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        #region MyConfigClass
        private static string MyConfigFileName = $@"{currentDirectory}\MyConfig.txt";
        static public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {

            private bool controlMode = false;
            private string servoZ_Com = "COM1";

            public bool ControlMode { get => controlMode; set => controlMode = value; }
            public string ServoZ_Com { get => servoZ_Com; set => servoZ_Com = value; }
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

            MyMessageBox.form = this.FindForm();

            Net.DebugLog = false;
            this.lowerMachine_Panel.Run();

            this.plC_UI_Init.音效 = false;
            this.plC_UI_Init.全螢幕顯示 = false;

            this.plC_UI_Init.UI_Finished_Event += PlC_UI_Init_UI_Finished_Event;
            this.plC_UI_Init.Run(this.FindForm(), this.lowerMachine_Panel);
            
        }

        private void PlC_UI_Init_UI_Finished_Event()
        {
            this.WindowState = FormWindowState.Maximized;
            PLC_UI_Init.Set_PLC_ScreenPage(panel_Main, this.plC_ScreenPage_Main);

            LoadMyConfig();

            Program_PLC();
        }
    }
}
