using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS_DB_Lib;
using Basic;
using SQLUI;
using MyUI;
using FpMatchLib;
using RfidReaderLib;
using SpeechLib;
using H_Pannel_lib;

namespace 調劑台管理系統
{
    public partial class UC_調劑作業_TypeA : UserControl
    {
        public string Title
        {
            set
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(delegate { this.rJ_Lable_Title.Text = value; }));
                }
                else
                {
                    this.rJ_Lable_Title.Text = value;
                }
            }
            get
            {
                return this.rJ_Lable_Title.Text;
            }
        }
        public int index = -1;
        public string 調劑台名稱 = "";
        public string 登入者姓名 = "";
        public string 密碼 = "";
        public string 卡號 = "";
        public string ID = "";
        private string _顏色 = "";
     
        public string 顏色
        {
            get
            {
                return _顏色;
            }
            set
            {
                _顏色 = value;
            }
        }
        public string 固定顏色 = "";
        public string 藥師證字號 = "";
        public string 一維碼 = "";
        public string 醫令條碼 = "";
        public string 年齡_ = "";
        public PLC_Device PLC_Device_已登入;
        public PLC_Device PLC_Device_單醫令模式;
        public FpMatchLib.FpMatchClass FpMatchClass_指紋資訊;
        public int RFID站號
        {
            get
            {
                return index + 1;
            }
        }
        public suspiciousRxLogClass suspiciousRxLog = null;
        MyTimer MyTimer_閒置登出時間 = new MyTimer("D100");
        PLC_Device PLC_Device_閒置登出時間 = new PLC_Device("D100");
        MyTimer MyTimer_入賬完成時間 = new MyTimer("D101");
        PLC_Device PLC_Device_入賬完成時間 = new PLC_Device("D101");

        private MyTimer myTimer_Logout = new MyTimer();
        private MyThread myThread_program;

        public UC_調劑作業_TypeA()
        {
            InitializeComponent();
        }
        public void Init(int index)
        {
            this.index = index;
            Table table = new Table(new enum_取藥堆疊母資料());
            this.sqL_DataGridView_領藥內容.Init(table);
            this.sqL_DataGridView_領藥內容.Set_ColumnVisible(false, new enum_取藥堆疊母資料().GetEnumNames());
            this.sqL_DataGridView_領藥內容.Set_ColumnWidth(40, DataGridViewContentAlignment.MiddleCenter, enum_取藥堆疊母資料.序號);

            this.sqL_DataGridView_領藥內容.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_取藥堆疊母資料.藥品碼);
            this.sqL_DataGridView_領藥內容.Set_ColumnWidth(350, DataGridViewContentAlignment.MiddleLeft, enum_取藥堆疊母資料.藥品名稱);
            this.sqL_DataGridView_領藥內容.Set_ColumnWidth(75, DataGridViewContentAlignment.MiddleRight, enum_取藥堆疊母資料.庫存量);
            this.sqL_DataGridView_領藥內容.Set_ColumnWidth(90, DataGridViewContentAlignment.MiddleRight, enum_取藥堆疊母資料.總異動量);
            this.sqL_DataGridView_領藥內容.Set_ColumnWidth(75, DataGridViewContentAlignment.MiddleRight, enum_取藥堆疊母資料.結存量);
            this.sqL_DataGridView_領藥內容.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleRight, enum_取藥堆疊母資料.單位);

            this.sqL_DataGridView_領藥內容.Set_ColumnText("No", enum_取藥堆疊母資料.序號);
            this.sqL_DataGridView_領藥內容.Set_ColumnText("藥碼", enum_取藥堆疊母資料.藥品碼);
            this.sqL_DataGridView_領藥內容.Set_ColumnText("藥名", enum_取藥堆疊母資料.藥品名稱);
            this.sqL_DataGridView_領藥內容.Set_ColumnText("數量", enum_取藥堆疊母資料.總異動量);
            this.sqL_DataGridView_領藥內容.Set_ColumnText("結存", enum_取藥堆疊母資料.結存量);
            this.sqL_DataGridView_領藥內容.Set_ColumnText("庫存", enum_取藥堆疊母資料.庫存量);

            this.sqL_DataGridView_領藥內容.Set_ColumnFont(new Font("微軟正黑體", 24, FontStyle.Bold), enum_取藥堆疊母資料.總異動量);
            this.sqL_DataGridView_領藥內容.Set_ColumnFont(new Font("微軟正黑體", 20, FontStyle.Bold), enum_取藥堆疊母資料.藥品名稱);


            this.sqL_DataGridView_領藥內容.DataGridRowsChangeRefEvent += SqL_DataGridView_領藥內容_DataGridRowsChangeRefEvent;
            this.sqL_DataGridView_領藥內容.DataGridRefreshEvent += SqL_DataGridView_領藥內容_DataGridRefreshEvent;
            this.sqL_DataGridView_領藥內容.RowEnterEvent += SqL_DataGridView_領藥內容_RowEnterEvent;
            this.sqL_DataGridView_領藥內容.DataGridClearGridEvent += SqL_DataGridView_領藥內容_DataGridClearGridEvent;
            toolStripMenuItem_調劑畫面_顯示設定.Click += ToolStripMenuItem_調劑畫面_顯示設定_Click;
            rJ_Lable_Title.Text = $" {(Main_Form.PLC_Device_導引模式.Bool ? "(導引模式)" : "")}[未登入]";

            textBox_密碼.PassWordChar = true;
            textBox_帳號.KeyPress += TextBox_帳號_KeyPress;
            textBox_密碼.KeyPress += TextBox_密碼_KeyPress;
            plC_Button_領.ValueChangeEvent += PlC_Button_領_ValueChangeEvent;
            plC_Button_退.ValueChangeEvent += PlC_Button_退_ValueChangeEvent;

            plC_RJ_Button_登入.MouseDownEvent += PlC_RJ_Button_登入_MouseDownEvent;
            plC_RJ_Button_取消作業.MouseDownEvent += PlC_RJ_Button_取消作業_MouseDownEvent;
            if (Main_Form.PLC_Device_AI處方核對啟用.Bool)
            {
                pictureBox_藥品圖片01.Click += PictureBox_藥品圖片_Click;
                pictureBox_藥品圖片02.Click += PictureBox_藥品圖片_Click;
                rJ_Lable_MedGPT_Title.Visible = true;
                Main_Form.PLC_Device_顯示診斷訊息.Bool = true;
            }
            if(Main_Form.PLC_Device_顯示診斷訊息.Bool)
            {
                panel_診斷及交互資訊.Height = 100;
            }
            else
            {
                panel_診斷及交互資訊.Height = 0;
            }

            myThread_program = new MyThread();
            myThread_program.Add_Method(sub_program);
            myThread_program.AutoRun(true);
            myThread_program.AutoStop(true);
            myThread_program.SetSleepTime(10);
            myThread_program.Trigger();
        }

    
        public void Login()
        {
            this.PlC_RJ_Button_登入_MouseDownEvent(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
        }
        public void Logout()
        {
            this.PlC_RJ_Button_登出_MouseDownEvent(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
        }
        public void Cancel()
        {
            this.PlC_RJ_Button_取消作業_MouseDownEvent(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
        }

        private void sub_program()
        {
            if (Dialog_藥品搜索.IsShown) return;
            if (PLC_Device_閒置登出時間.Value != 0)
            {
                if (PLC_Device_已登入.Bool)
                {
                    if (MyTimer_閒置登出時間.IsTimeOut())
                    {
                        Logout();
                    }
                }
                else
                {
                    MyTimer_閒置登出時間.TickStop();
                    MyTimer_閒置登出時間.StartTickTime(PLC_Device_閒置登出時間.Value);
                }
            }
            if (PLC_Device_入賬完成時間.Value != 0)
            {
                if (PLC_Device_已登入.Bool)
                {
                    if (MyTimer_入賬完成時間.IsTimeOut())
                    {
                        Cancel();
                        MyTimer_入賬完成時間.TickStop();
                        MyTimer_入賬完成時間.StartTickTime(PLC_Device_入賬完成時間.Value);
                    }
                }
                else
                {
                    MyTimer_入賬完成時間.TickStop();
                    MyTimer_入賬完成時間.StartTickTime(PLC_Device_入賬完成時間.Value);
                }
            }

            sub_Program_檢查登入();
            sub_Program_檢查輸入資料();
            sub_Program_刷新領藥內容();
        }

        #region PLC_檢查登入

        PLC_Device PLC_Device_檢查登入 = new PLC_Device("");
        PLC_Device PLC_Device_檢查登入_OK = new PLC_Device("");


        int cnt_Program_檢查登入 = 65534;
        void sub_Program_檢查登入()
        {
            if (Main_Form._plC_ScreenPage_Main.PageText == "調劑作業") PLC_Device_檢查登入.Bool = true;
            else PLC_Device_檢查登入.Bool = false;
            if (cnt_Program_檢查登入 == 65534)
            {
                PLC_Device_檢查登入.SetComment("PLC_檢查登入");
                PLC_Device_檢查登入_OK.SetComment("PLC_Device_檢查登入_OK");
                PLC_Device_檢查登入.Bool = false;
                PLC_Device_已登入.Bool = false;
                cnt_Program_檢查登入 = 65535;
            }
            if (cnt_Program_檢查登入 == 65535) cnt_Program_檢查登入 = 1;
            if (cnt_Program_檢查登入 == 1) cnt_Program_檢查登入_檢查按下(ref cnt_Program_檢查登入);
            if (cnt_Program_檢查登入 == 2) cnt_Program_檢查登入_初始化(ref cnt_Program_檢查登入);
            if (cnt_Program_檢查登入 == 3) cnt_Program_檢查登入_外部設備資料或帳號密碼登入(ref cnt_Program_檢查登入);
            if (cnt_Program_檢查登入 == 4) cnt_Program_檢查登入 = 65500;
            if (cnt_Program_檢查登入 > 1) cnt_Program_檢查登入_檢查放開(ref cnt_Program_檢查登入);

            if (cnt_Program_檢查登入 == 65500)
            {
                PLC_Device_檢查登入.Bool = false;
                cnt_Program_檢查登入 = 65535;
            }
        }
        void cnt_Program_檢查登入_檢查按下(ref int cnt)
        {
            if (PLC_Device_檢查登入.Bool) cnt++;
        }
        void cnt_Program_檢查登入_檢查放開(ref int cnt)
        {
            if (!PLC_Device_檢查登入.Bool) cnt = 65500;
        }
        void cnt_Program_檢查登入_初始化(ref int cnt)
        {
            PLC_Device_檢查登入_OK.Bool = false;
            cnt++;
        }
        void cnt_Program_檢查登入_外部設備資料或帳號密碼登入(ref int cnt)
        {
            if (Dialog_手輸醫令.IsShown)
            {
                cnt = 65500;
                return;
            }

            if (Dialog_使用者登入.IsShown)
            {
                string scanner_text = Main_Form.Function_ReadBacodeScanner(index);
                if (scanner_text.StringIsEmpty()) return;
                return;
            }
            if (PLC_Device_已登入.Bool == false) CheckFpMatchLogin();
            if (Main_Form.PLC_Device_導引模式.Bool == true && PLC_Device_已登入.Bool == false)
            {

                string scanner_text = Main_Form.Function_ReadBacodeScanner(index);
                if (scanner_text.StringIsEmpty()) return;
                if (Main_Form.Function_檢查是否完成交班() == false)
                {
                    MyMessageBox.ShowDialog("請先完成交班");
                    return;
                }
                personPageClass personPageClass = new personPageClass();

                if (Function_檢查是否為藥品條碼(scanner_text))
                {
                    cnt = 65500;
                    return;
                }

                if (this.顏色.StringIsEmpty())
                {
                    if (index == 0) this.顏色 = Color.Red.ToColorString();
                    if (index == 1) this.顏色 = Color.Green.ToColorString();
                    if (index == 2) this.顏色 = Color.Blue.ToColorString();
                    if (index == 3) this.顏色 = Color.Yellow.ToColorString();
                    if (index == 4) this.顏色 = Color.PaleVioletRed.ToColorString();
                }

                personPageClass.ID = "";
                personPageClass.姓名 = "導引模式";
                personPageClass.藥師證字號 = "";
                personPageClass.顏色 = this.顏色;
              
                if (Main_Form.Function_醫令領藥(scanner_text, personPageClass, 調劑台名稱, PLC_Device_單醫令模式.Bool , this) == null)
                {
                    cnt = 65500;
                    return;
                }

            }
            string UID = Main_Form._RFID_FX600_UI.Get_RFID_UID(RFID站號);
            if (!UID.StringIsEmpty() && UID.StringToInt32() != 0 && Dialog_使用者登入.myTimerBasic_覆核完成.IsTimeOut())
            {
                Console.WriteLine($"成功讀取RFID  {UID}");
                if (卡號 == UID && PLC_Device_已登入.Bool)
                {
                    Console.WriteLine($"使用者01已經登入中....");
                    return;
                }
                卡號 = UID;
                List<object[]> list_人員資料 = Main_Form._sqL_DataGridView_人員資料.SQL_GetRows(enum_人員資料.卡號.GetEnumName(), 卡號, false);
                if (list_人員資料.Count == 0) return;
                Console.WriteLine($"取得人員資料完成!");
                if (PLC_Device_已登入.Bool && (ID != list_人員資料[0][(int)enum_人員資料.ID].ObjectToString())) Logout();
                this.Invoke(new Action(delegate
                {
                    textBox_帳號.Texts = list_人員資料[0][(int)enum_人員資料.ID].ObjectToString();
                    textBox_密碼.Texts = list_人員資料[0][(int)enum_人員資料.密碼].ObjectToString();
                    Login();
                }));
                Main_Form.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.RFID登入, 登入者姓名, "01.號使用者");
                cnt++;
                return;
            }
            else if (Main_Form.Function_ReadBacodeScanner_pre(index) != null && !PLC_Device_已登入.Bool)
            {
                string scanner_text = Main_Form.Function_ReadBacodeScanner(index);
                Console.WriteLine($"[掃描器 {index}] 一維碼讀取 = {scanner_text}");

                一維碼 = scanner_text;
                List<object[]> list_人員資料 = Main_Form._sqL_DataGridView_人員資料.SQL_GetRows(enum_人員資料.一維條碼.GetEnumName(), 一維碼, false);
                Console.WriteLine($"[掃描器 {index}] 人員資料查詢結果 = {list_人員資料.Count} 筆");

                if (list_人員資料.Count == 0)
                {
                    Console.WriteLine($"[掃描器 {index}] 查無此一維碼，播放提示音");
                    Voice.MediaPlayAsync($@"{Main_Form.currentDirectory}\查無此一維碼.wav");
                    return;
                }

                this.Invoke(new Action(delegate
                {
                    string id = list_人員資料[0][(int)enum_人員資料.ID].ObjectToString();
                    string pwd = list_人員資料[0][(int)enum_人員資料.密碼].ObjectToString();
                    Console.WriteLine($"[掃描器 {index}] 登入資訊 - ID: {id}, 密碼: {pwd}");
                    textBox_帳號.Texts = id;
                    textBox_密碼.Texts = pwd;
                    Login();
                }));

                Console.WriteLine($"[掃描器 {index}] 動作紀錄新增 - 一維碼登入");
                Main_Form.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.一維碼登入, 登入者姓名, "01.號使用者");

                cnt++;
                Console.WriteLine($"[掃描器 {index}] 流程結束，cnt = {cnt}");
                return;
            }
            else if (Main_Form.VoiceSample != null && !PLC_Device_已登入.Bool)
            {

                登入者姓名 = Main_Form.VoiceSample.name;
                Main_Form.VoiceSample = null;
                if (登入者姓名.StringIsEmpty()) return;
                List<object[]> list_人員資料 = Main_Form._sqL_DataGridView_人員資料.SQL_GetRows(enum_人員資料.姓名.GetEnumName(), 登入者姓名, false);
                if (list_人員資料.Count == 0) return;
                if (PLC_Device_已登入.Bool && (ID != list_人員資料[0][(int)enum_人員資料.ID].ObjectToString())) Logout();

                this.Invoke(new Action(delegate
                {
                    textBox_帳號.Texts = list_人員資料[0][(int)enum_人員資料.ID].ObjectToString();
                    textBox_密碼.Texts = list_人員資料[0][(int)enum_人員資料.密碼].ObjectToString();
                    Login();
                }));

                Main_Form.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.密碼登入, 登入者姓名, "01.號使用者");

                Voice voice = new Voice();
                voice.SpeakOnTask($"{登入者姓名},聲紋辨識登入成功");
                cnt++;
                return;
            }
            //else if (FpMatchClass_指紋資訊 != null)
            //{
            //    List<object[]> list_人員資料 = Main_Form._sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            //    object[] value = null;
            //    for (int i = 0; i < list_人員資料.Count; i++)
            //    {
            //        string feature = list_人員資料[i][(int)enum_人員資料.指紋辨識].ObjectToString();
            //        if (Main_Form.fpMatchSoket.Match(FpMatchClass_指紋資訊.feature, feature))
            //        {
            //            value = list_人員資料[i];
            //            break;
            //        }
            //    }
            //    FpMatchClass_指紋資訊 = null;
            //    if (value == null)
            //    {
            //        Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("找無符合指紋資訊", 2000);
            //        dialog_AlarmForm.ShowDialog();
            //        cnt = 65500;
            //        return;
            //    }
            //    if (PLC_Device_已登入.Bool && (ID != value[(int)enum_人員資料.ID].ObjectToString())) Logout();

            //    this.Invoke(new Action(delegate
            //    {
            //        textBox_帳號.Texts = value[(int)enum_人員資料.ID].ObjectToString();
            //        textBox_密碼.Texts = value[(int)enum_人員資料.密碼].ObjectToString();
            //        Login();
            //    }));

            //    Main_Form.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.指紋登入, 登入者姓名, "01.號使用者");
            //}
            cnt = 65500;
            return;


        }

        #endregion
        #region PLC_檢查輸入資料

        PLC_Device PLC_Device_檢查輸入資料 = new PLC_Device("");
        PLC_Device PLC_Device_檢查輸入資料_OK = new PLC_Device("");

        MyTimer MyTimer_檢查輸入資料_NG訊息延遲 = new MyTimer();
        int cnt_Program_檢查輸入資料 = 65534;
        void sub_Program_檢查輸入資料()
        {
            if (Main_Form._plC_ScreenPage_Main.PageText == "調劑作業" && PLC_Device_已登入.Bool)
            {
                PLC_Device_檢查輸入資料.Bool = true;
            }
            else
            {
                PLC_Device_檢查輸入資料.Bool = false;
            }
            if (Dialog_使用者登入.IsShown)
            {
                PLC_Device_檢查輸入資料.Bool = false;
            }
            if (cnt_Program_檢查輸入資料 == 65534)
            {
                PLC_Device_檢查輸入資料.SetComment("PLC_檢查輸入資料");
                PLC_Device_檢查輸入資料_OK.SetComment("PLC_Device_檢查輸入資料_OK");
                PLC_Device_檢查輸入資料.Bool = false;
                cnt_Program_檢查輸入資料 = 65535;
            }
            if (cnt_Program_檢查輸入資料 == 65535) cnt_Program_檢查輸入資料 = 1;
            if (cnt_Program_檢查輸入資料 == 1) cnt_Program_檢查輸入資料_檢查按下(ref cnt_Program_檢查輸入資料);
            if (cnt_Program_檢查輸入資料 == 2) cnt_Program_檢查輸入資料_初始化(ref cnt_Program_檢查輸入資料);
            if (cnt_Program_檢查輸入資料 == 3) cnt_Program_檢查輸入資料_設定開始掃描(ref cnt_Program_檢查輸入資料);
            if (cnt_Program_檢查輸入資料 == 4) cnt_Program_檢查輸入資料_等待掃描結束(ref cnt_Program_檢查輸入資料);
            if (cnt_Program_檢查輸入資料 == 5) cnt_Program_檢查輸入資料_檢查醫令資料及寫入(ref cnt_Program_檢查輸入資料);
            if (cnt_Program_檢查輸入資料 == 6) cnt_Program_檢查輸入資料 = 65500;


            if (cnt_Program_檢查輸入資料 > 1) cnt_Program_檢查輸入資料_檢查放開(ref cnt_Program_檢查輸入資料);

            if (cnt_Program_檢查輸入資料 == 65500)
            {
                PLC_Device_檢查輸入資料.Bool = false;
                cnt_Program_檢查輸入資料 = 65535;
            }
        }
        void cnt_Program_檢查輸入資料_檢查按下(ref int cnt)
        {
            if (PLC_Device_檢查輸入資料.Bool) cnt++;
        }
        void cnt_Program_檢查輸入資料_檢查放開(ref int cnt)
        {
            if (!PLC_Device_檢查輸入資料.Bool) cnt = 65500;
        }
        void cnt_Program_檢查輸入資料_初始化(ref int cnt)
        {
            //PLC_Device_Scanner_讀取藥單資料.Bool = false;
            cnt++;
        }
        void cnt_Program_檢查輸入資料_設定開始掃描(ref int cnt)
        {
            if (Dialog_手輸醫令.IsShown)
            {
                cnt = 65500;
                return;
            }
          
            if (Main_Form.Function_ReadBacodeScanner_pre(index) != null)
            {
                醫令條碼 = Main_Form.Function_ReadBacodeScanner(index);
                if (醫令條碼.StringIsEmpty()) return;
                cnt++;
                return;
            }
        }
        void cnt_Program_檢查輸入資料_等待掃描結束(ref int cnt)
        {
            if (PLC_Device_已登入.Bool)
            {
                if (醫令條碼.Length < 15)
                {
                    string text = 醫令條碼.Replace("\n", "");
                    text = text.Replace("\r", "");
                    if (text.StringIsEmpty())
                    {
                        cnt = 65500;
                        return;
                    }
                    Console.WriteLine($"{text}");

                    if(Function_檢查是否為藥品條碼(text))
                    {
                        cnt = 65500;
                        return;
                    }
                  
                    if (text == Main_Form.QR_Code_醫令模式切換)
                    {
                        PLC_Device_單醫令模式.Bool = !PLC_Device_單醫令模式.Bool;

                        string text_temp = PLC_Device_單醫令模式.Bool ? "【單醫令】" : "【多醫令】";
                        Console.WriteLine($"切換模式至{text_temp}");
                        Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm($"切換模式至{text_temp}", 1000, 0, 0);
                        this.Invoke(new Action(delegate
                        {

                            rJ_Lable_Title.Text = $" {(Main_Form.PLC_Device_導引模式.Bool ? "(導引模式)" : "")}[{登入者姓名}] {text_temp}";
                            rJ_Lable_Title.BackgroundColor = Color.GreenYellow;
                            rJ_Lable_Title.TextColor = Color.Black;
                        }));
                        dialog_AlarmForm.ShowDialog();
                        cnt = 65500;
                        return;
                    }
                    if (text.StringIsEmpty())
                    {
                        cnt = 65500;
                        return;
                    }
                    List<object[]> list_人員資料 = Main_Form._sqL_DataGridView_人員資料.SQL_GetRows(enum_人員資料.一維條碼.GetEnumName(), text, false);
                    if (list_人員資料.Count > 0)
                    {
                        if (ID != list_人員資料[0][(int)enum_人員資料.ID].ObjectToString())
                        {
                            Logout();

                            this.Invoke(new Action(delegate
                            {
                                textBox_帳號.Texts = list_人員資料[0][(int)enum_人員資料.ID].ObjectToString();
                                textBox_密碼.Texts = list_人員資料[0][(int)enum_人員資料.密碼].ObjectToString();
                                Login();
                            }));
                            Main_Form.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.一維碼登入, 登入者姓名, 調劑台名稱);

                        }
                        cnt = 65500;
                        return;
                    }
                }

            }


            cnt++;
            return;
        }
        void cnt_Program_檢查輸入資料_檢查醫令資料及寫入(ref int cnt)
        {
            if (Dialog_手輸醫令.IsShown)
            {
                cnt = 65500;
                return;
            }
            if (plC_Button_領.Bool)
            {
               Function_醫令領藥(醫令條碼);
            }
            else if (plC_Button_退.Bool)
            {
                Function_醫令退藥(醫令條碼);
            }
            cnt++;

        }
   

        private void PictureBox_藥品圖片_Click(object sender, EventArgs e)
        {
            if(suspiciousRxLog == null)
            {
                return;
            }
            if (suspiciousRxLog.狀態 != enum_suspiciousRxLog_status.無異狀.GetEnumName())
            {
                Dialog_醫師疑義處方紀錄表 dialog_醫師疑義處方紀錄表 = new Dialog_醫師疑義處方紀錄表(suspiciousRxLog, 登入者姓名);

                if (dialog_醫師疑義處方紀錄表.ShowDialog() != DialogResult.Yes) return;

                (int code, string resuult, suspiciousRxLogClass _suspiciousRxLog) = suspiciousRxLogClass.update_full(Main_Form.API_Server, dialog_醫師疑義處方紀錄表.Value);
                if (code != 200)
                {
                    MyMessageBox.ShowDialog(resuult);
                    return;
                }
            }
        }
   
        #endregion
        #region PLC_刷新領藥內容
        PLC_Device PLC_Device_刷新領藥內容 = new PLC_Device("");
        PLC_Device PLC_Device_刷新領藥內容_OK = new PLC_Device("");
        MyTimer MyTimer__刷新領藥內容_刷新間隔 = new MyTimer();
        int cnt_Program_刷新領藥內容 = 65534;
        void sub_Program_刷新領藥內容()
        {
            if (Main_Form._plC_ScreenPage_Main.PageText == "調劑作業")
            {
                PLC_Device_刷新領藥內容.Bool = true;
            }
            else
            {
                PLC_Device_刷新領藥內容.Bool = false;
            }
            PLC_Device_刷新領藥內容.Bool = (Main_Form._plC_ScreenPage_Main.PageText == "調劑作業");
            if (cnt_Program_刷新領藥內容 == 65534)
            {
                PLC_Device_刷新領藥內容.SetComment("PLC_刷新領藥內容");
                PLC_Device_刷新領藥內容_OK.SetComment("PLC_刷新領藥內容_OK");
                PLC_Device_刷新領藥內容.Bool = false;
                cnt_Program_刷新領藥內容 = 65535;
            }
            if (cnt_Program_刷新領藥內容 == 65535) cnt_Program_刷新領藥內容 = 1;
            if (cnt_Program_刷新領藥內容 == 1) cnt_Program_刷新領藥內容_檢查按下(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 2) cnt_Program_刷新領藥內容_初始化(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 3) cnt_Program_刷新領藥內容_取得資料(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 4) cnt_Program_刷新領藥內容_檢查雙人覆核(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 5) cnt_Program_刷新領藥內容_檢查RFID使用(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 6) cnt_Program_刷新領藥內容_檢查盲盤作業(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 7) cnt_Program_刷新領藥內容_檢查複盤作業(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 8) cnt_Program_刷新領藥內容_檢查作業完成(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 9) cnt_Program_刷新領藥內容_檢查是否需輸入效期(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 10) cnt_Program_刷新領藥內容_檢查是否需選擇效期(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 11) cnt_Program_刷新領藥內容_檢查自動登出(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 12) cnt_Program_刷新領藥內容_等待刷新間隔(ref cnt_Program_刷新領藥內容);
            if (cnt_Program_刷新領藥內容 == 13) cnt_Program_刷新領藥內容 = 65500;
            if (cnt_Program_刷新領藥內容 > 1) cnt_Program_刷新領藥內容_檢查放開(ref cnt_Program_刷新領藥內容);

            if (cnt_Program_刷新領藥內容 == 65500)
            {
                PLC_Device_刷新領藥內容.Bool = false;
                PLC_Device_刷新領藥內容_OK.Bool = false;
                cnt_Program_刷新領藥內容 = 65535;
            }
        }
        void cnt_Program_刷新領藥內容_檢查按下(ref int cnt)
        {
            if (PLC_Device_刷新領藥內容.Bool) cnt++;
        }
        void cnt_Program_刷新領藥內容_檢查放開(ref int cnt)
        {
            if (!PLC_Device_刷新領藥內容.Bool) cnt = 65500;
        }
        void cnt_Program_刷新領藥內容_初始化(ref int cnt)
        {
            cnt++;
        }
        void cnt_Program_刷新領藥內容_取得資料(ref int cnt)
        {

            List<object[]> list_value = new List<object[]>();
            List<object[]> list_取藥堆疊資料 = new List<object[]>();
            List<object[]> list_取藥堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥堆疊資料_add = new List<object[]>();
            if (Main_Form.myConfigClass.系統取藥模式) list_取藥堆疊資料 = Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
            else list_取藥堆疊資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            List<object[]> list_取藥堆疊資料_replace = new List<object[]>();
            string GUID = "";
            string 序號 = "";
            string 動作 = "";
            string 藥袋序號 = "";
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 病歷號 = "";
            string 操作時間 = "";
            string 開方時間 = "";
            string 庫存量 = "";
            string 總異動量 = "";
            string 結存量 = "";
            string 單位 = "";
            string 狀態 = "";
            string 床號 = "";
            string 盤點量 = "";
            string 儲位描述 = "";
            list_取藥堆疊資料.Sort(new Main_Form.Icp_取藥堆疊母資料_index排序());

            for (int i = 0; i < list_取藥堆疊資料.Count; i++)
            {
                if (Main_Form.Function_取藥堆疊資料_取得作業模式(list_取藥堆疊資料[i], enum_取藥堆疊母資料_作業模式.庫存不足語音提示))
                {
                    Main_Form.voice.SpeakOnTask("庫存不足");
                    Main_Form.Function_取藥堆疊資料_設定作業模式(list_取藥堆疊資料[i], enum_取藥堆疊母資料_作業模式.庫存不足語音提示, false);
                    list_取藥堆疊資料_replace.Add(list_取藥堆疊資料[i]);
                }
                GUID = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                序號 = (i + 1).ToString();
                動作 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.動作].ObjectToString();
                藥袋序號 = $"{list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.藥袋序號].ObjectToString()}:{list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.頻次].ObjectToString()}";
                藥品碼 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                藥品名稱 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                病歷號 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.病歷號].ObjectToString();
                操作時間 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.操作時間].ObjectToString();
                開方時間 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.開方時間].ObjectToString();
                庫存量 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.庫存量].ObjectToString();
                總異動量 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString();
                結存量 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.結存量].ObjectToString();
                單位 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.單位].ObjectToString();
                狀態 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString();
                床號 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.床號].ObjectToString();
                盤點量 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.盤點量].ObjectToString();
                儲位描述 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.儲位描述].ObjectToString();
                if (Main_Form.Function_取藥堆疊資料_取得作業模式(list_取藥堆疊資料[i], enum_取藥堆疊母資料_作業模式.盲盤))
                {
                    庫存量 = "無";
                    結存量 = "無";
                }
                object[] value = new object[new enum_取藥堆疊母資料().GetLength()];
                value[(int)enum_取藥堆疊母資料.GUID] = GUID;
                value[(int)enum_取藥堆疊母資料.序號] = 序號;
                value[(int)enum_取藥堆疊母資料.動作] = 動作;
                value[(int)enum_取藥堆疊母資料.藥袋序號] = 藥袋序號;
                value[(int)enum_取藥堆疊母資料.藥品碼] = 藥品碼;
                value[(int)enum_取藥堆疊母資料.藥品名稱] = 藥品名稱;
                value[(int)enum_取藥堆疊母資料.病歷號] = 病歷號;
                value[(int)enum_取藥堆疊母資料.操作時間] = 操作時間;
                value[(int)enum_取藥堆疊母資料.開方時間] = 開方時間;
                value[(int)enum_取藥堆疊母資料.庫存量] = 庫存量;
                value[(int)enum_取藥堆疊母資料.總異動量] = 總異動量;
                value[(int)enum_取藥堆疊母資料.結存量] = 結存量;
                value[(int)enum_取藥堆疊母資料.單位] = 單位;
                value[(int)enum_取藥堆疊母資料.狀態] = 狀態;
                value[(int)enum_取藥堆疊母資料.床號] = 床號;
                value[(int)enum_取藥堆疊母資料.盤點量] = 盤點量;
                value[(int)enum_取藥堆疊母資料.儲位描述] = 儲位描述;

                list_value.Add(value);


            }

            if (Main_Form.PLC_Device_調劑畫面合併相同藥品.Bool)
            {
                List<object[]> list_value_new = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();
                string[] 藥品碼_array = (from value in list_value
                                      select value[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString()).Distinct().ToList().ToArray();
                for (int i = 0; i < 藥品碼_array.Length; i++)
                {
                    list_value_buf = (from value in list_value
                                      where value[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString() == 藥品碼_array[i]
                                      select value).ToList();
                    list_value_buf.Sort(new ICP_取藥堆疊母資料());
                    if (list_value_buf.Count == 0) continue;
                    object[] value_領藥內容 = new object[new enum_取藥堆疊母資料().GetLength()];
                    value_領藥內容[(int)enum_取藥堆疊母資料.GUID] = list_value_buf[0][(int)enum_取藥堆疊母資料.GUID];
                    value_領藥內容[(int)enum_取藥堆疊母資料.序號] = "";
                    value_領藥內容[(int)enum_取藥堆疊母資料.動作] = 動作;
                    value_領藥內容[(int)enum_取藥堆疊母資料.藥袋序號] = list_value_buf[0][(int)enum_取藥堆疊母資料.藥袋序號];
                    value_領藥內容[(int)enum_取藥堆疊母資料.藥品碼] = list_value_buf[0][(int)enum_取藥堆疊母資料.藥品碼];
                    value_領藥內容[(int)enum_取藥堆疊母資料.藥品名稱] = list_value_buf[0][(int)enum_取藥堆疊母資料.藥品名稱];
                    value_領藥內容[(int)enum_取藥堆疊母資料.病歷號] = list_value_buf[0][(int)enum_取藥堆疊母資料.病歷號];
                    value_領藥內容[(int)enum_取藥堆疊母資料.操作時間] = list_value_buf[0][(int)enum_取藥堆疊母資料.操作時間];
                    value_領藥內容[(int)enum_取藥堆疊母資料.開方時間] = list_value_buf[0][(int)enum_取藥堆疊母資料.開方時間];
                    value_領藥內容[(int)enum_取藥堆疊母資料.庫存量] = list_value_buf[list_value_buf.Count - 1][(int)enum_取藥堆疊母資料.庫存量].ObjectToString();
                    value_領藥內容[(int)enum_取藥堆疊母資料.總異動量] = "";
                    value_領藥內容[(int)enum_取藥堆疊母資料.結存量] = list_value_buf[list_value_buf.Count - 1][(int)enum_取藥堆疊母資料.結存量].ObjectToString();
                    value_領藥內容[(int)enum_取藥堆疊母資料.單位] = list_value_buf[0][(int)enum_取藥堆疊母資料.單位];
                    value_領藥內容[(int)enum_取藥堆疊母資料.床號] = list_value_buf[0][(int)enum_取藥堆疊母資料.床號];
                    double 總異動量_temp = 0;
                    bool flag_入賬完成 = true;
                    bool flag_無儲位 = false;
                    bool flag_庫存不足 = false;
                    bool flag_已領用過 = false;
                    for (int k = 0; k < list_value_buf.Count; k++)
                    {
                        if (list_value_buf[k][(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName())
                        {
                            flag_入賬完成 = false;
                        }
                        if (list_value_buf[k][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.無儲位.GetEnumName())
                        {
                            flag_無儲位 = true;
                        }
                        if (list_value_buf[k][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName())
                        {
                            flag_庫存不足 = true;
                        }
                        if (list_value_buf[k][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.已領用過.GetEnumName())
                        {
                            flag_已領用過 = true;
                        }
                        總異動量_temp += list_value_buf[k][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToDouble();
                    }
                    value_領藥內容[(int)enum_取藥堆疊母資料.總異動量] = 總異動量_temp;
                    if (flag_入賬完成)
                    {
                        value_領藥內容[(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName();
                    }
                    else if (flag_無儲位)
                    {
                        value_領藥內容[(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.無儲位.GetEnumName();
                    }
                    else if (flag_庫存不足)
                    {
                        value_領藥內容[(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName();
                    }
                    else if (flag_已領用過)
                    {
                        value_領藥內容[(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.已領用過.GetEnumName();
                    }
                    else
                    {
                        value_領藥內容[(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待作業.GetEnumName();
                    }
                    list_value_new.Add(value_領藥內容);

                }
                for (int i = 0; i < list_value_new.Count; i++)
                {
                    list_value_new[i][(int)enum_取藥堆疊母資料.序號] = (i + 1).ToString();
                    藥品碼 = list_value_new[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    double 庫存量_temp = Main_Form.Function_從SQL取得庫存(藥品碼);
                    double 結存量_temp = 庫存量_temp + list_value_new[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToDouble();
                    //list_value_new[i][(int)enum_取藥堆疊母資料.庫存量] = 庫存量_temp.ToString();
                    //list_value_new[i][(int)enum_取藥堆疊母資料.結存量] = 結存量_temp.ToString();
                }
                list_value = list_value_new;
            }
            try
            {
                for (int i = 0; i < list_取藥堆疊資料.Count; i++)
                {
                    list_取藥堆疊資料_buf = sqL_DataGridView_領藥內容.GetRows("GUID", list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString(), false);
                    if (list_取藥堆疊資料_buf.Count == 0)
                    {
                        list_取藥堆疊資料_add.Add(list_取藥堆疊資料[i]);
                    }
                }
                if (list_取藥堆疊資料_add.Count > 0)
                {
                    string 藥碼 = list_取藥堆疊資料_add[0][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    string 藥名 = list_取藥堆疊資料_add[0][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                    string 姓名 = list_取藥堆疊資料_add[0][(int)enum_取藥堆疊母資料.病人姓名].ObjectToString();
                    string 年齡 = 年齡_;
                    string 領藥號 = list_取藥堆疊資料_add[0][(int)enum_取藥堆疊母資料.領藥號].ObjectToString();
                    病歷號 = list_取藥堆疊資料_add[0][(int)enum_取藥堆疊母資料.病歷號].ObjectToString();
                    開方時間 = list_取藥堆疊資料_add[0][(int)enum_取藥堆疊母資料.開方時間].ObjectToString();
                    Function_調劑作業_醫令資訊更新(藥碼, 藥名, 姓名, 年齡, 領藥號, 病歷號, 開方時間);
                }
            }
            catch
            {

            }

            sqL_DataGridView_領藥內容.RefreshGrid(list_value);
            Application.DoEvents();
            if (list_取藥堆疊資料_replace.Count > 0) Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊資料_replace, false);
            cnt++;
        }
        void cnt_Program_刷新領藥內容_檢查雙人覆核(ref int cnt)
        {
            List<object[]> list_取藥堆疊母資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            List<object[]> list_取藥堆疊母資料_replace = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_delete = new List<object[]>();

            list_取藥堆疊母資料 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.雙人覆核.GetEnumName());
            for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
            {
                string 藥碼 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 藥名 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                Application.DoEvents();
                Dialog_使用者登入 dialog_使用者登入 = new Dialog_使用者登入(ID, 藥名);

                if (dialog_使用者登入.ShowDialog() != DialogResult.Yes)
                {
                    list_取藥堆疊母資料_delete.Add(list_取藥堆疊母資料[i]);
                    Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料_delete, false);
                    Fuction_時間重置();
                    continue;
                }
                Fuction_時間重置();
                list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.覆核藥師姓名] = dialog_使用者登入.UserName;
                list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.覆核藥師ID] = dialog_使用者登入.UserID;
                list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待作業.GetEnumName();
                Main_Form.Function_取藥堆疊資料_設定作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.雙人覆核, false);
                list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.收支原因] = $"{list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.收支原因].ObjectToString()} \n覆核:{dialog_使用者登入.UserName}";
                list_取藥堆疊母資料_replace.Add(list_取藥堆疊母資料[i]);
            }
            if (list_取藥堆疊母資料_replace.Count > 0)
            {
                Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_replace, false);
                cnt = 1;
            }
            if (list_取藥堆疊母資料_delete.Count > 0)
            {
                Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料_delete, false);
                cnt = 1;
            }
            if (cnt == 1) return;
            cnt++;

        }
        void cnt_Program_刷新領藥內容_檢查RFID使用(ref int cnt)
        {
            List<object[]> list_取藥堆疊母資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            List<object[]> list_取藥堆疊母資料_add = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_replace = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_delete = new List<object[]>();

            list_取藥堆疊母資料 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.RFID使用.GetEnumName());
            if (list_取藥堆疊母資料.Count > 0 )
            {
                if(Main_Form.RfidReaderEnable == false)
                {
                    Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料, false);
                    Main_Form.voice.SpeakOnTask("RFID讀取器未開啟");
                    cnt = 1;
                    return;
                }
                List<takeMedicineStackClass> takeMedicineStackClasses = list_取藥堆疊母資料.ToTakeMedicineStackClassList();
                Dialog_HFRFID調劑作業 dialog_HFRFID調劑作業 = new Dialog_HFRFID調劑作業(takeMedicineStackClasses[0].調劑台名稱 ,(takeMedicineStackClasses[0].動作.Contains("退")? IncomeOutcomeMode.收入 : IncomeOutcomeMode.支出));
                if (dialog_HFRFID調劑作業.ShowDialog() != DialogResult.Yes)
                {
                    Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料, false);
                    Fuction_時間重置();
                    cnt = 1;
                    return;
                }
                Fuction_時間重置();

                list_取藥堆疊母資料_add = dialog_HFRFID調劑作業.takeMedicineStackClasses.ClassToSQL<takeMedicineStackClass, enum_取藥堆疊母資料>();
                list_取藥堆疊母資料_delete.LockAdd(list_取藥堆疊母資料);
            }
       
            if (list_取藥堆疊母資料_delete.Count > 0)
            {
                Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料_delete, false);
                cnt = 1;
            }
            if (list_取藥堆疊母資料_add.Count > 0)
            {
                Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_AddRows(list_取藥堆疊母資料_add, false);
                cnt = 1;
            }
            if (list_取藥堆疊母資料_replace.Count > 0)
            {
                Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_replace, false);
                cnt = 1;
            }
         
            if (cnt == 1) return;
            cnt++;

        }
        void cnt_Program_刷新領藥內容_檢查盲盤作業(ref int cnt)
        {
            List<object[]> list_取藥堆疊母資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            List<object[]> list_取藥堆疊母資料_replace = new List<object[]>();

            list_取藥堆疊母資料 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName());
            for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
            {
                int try_error = 0;
                int retry = 0;
                string 藥碼 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 藥名 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                double 總異動量 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.總異動量].StringToDouble();
                double 庫存量 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.庫存量].StringToDouble();
                double 結存量 = 0;
                double 差異值 = medRecheckLogClass.get_unresolved_qty_by_code(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, 藥碼);
                結存量 = 庫存量 + 總異動量 + 差異值;
                Console.WriteLine($"{藥碼}-{藥名} 庫存量:{庫存量},總異動量:{總異動量},差異值:{差異值},結存量:{結存量}");
                Voice.MediaPlayAsync($@"{Main_Form.currentDirectory}\請輸入盲盤數量.wav"); ;
                while (true)
                {
                    Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel($"(盲盤)請輸入取藥後盤點數量\n交易量 : {總異動量}", $"藥碼:{藥碼} \n藥名:{藥名} ");
                    dialog_NumPannel.TitleFont = new Font("微軟正黑體", 20, FontStyle.Bold);
                    dialog_NumPannel.X_Visible = true;

                    if (dialog_NumPannel.ShowDialog() != DialogResult.Yes)
                    {
                        Main_Form.Function_取藥堆疊資料_刪除母資料(list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString());
                        Fuction_時間重置();
                        break;
                    }
                    list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.盤點量] = dialog_NumPannel.Value.ToString();
                    
                    //double 庫存量 = Main_Form.Function_從SQL取得庫存(藥碼);
                
          
                   
                    list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.結存量] = 結存量;
                    if (結存量 == dialog_NumPannel.Value)
                    {
                        Main_Form.Function_取藥堆疊資料_設定作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.盲盤, false);
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待作業.GetEnumName();
                        list_取藥堆疊母資料_replace.Add(list_取藥堆疊母資料[i]);
                        break;
                    }
                    Voice.MediaPlayAsync($@"{Main_Form.currentDirectory}\盲盤數量錯誤.wav");
                    if (retry == 0)
                    {
                        Dialog_錯誤提示 dialog_錯誤提示 = new Dialog_錯誤提示("請再次覆盤", 2000);
                        dialog_錯誤提示.ShowDialog();
                    }
                    if (retry == 1)
                    {
                        Dialog_錯誤提示 dialog_錯誤提示 = new Dialog_錯誤提示($"異常紀錄,盤點值 : {dialog_NumPannel.Value}", 2000);
                        dialog_錯誤提示.ShowDialog();
                        Main_Form.Function_取藥堆疊資料_設定作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.盲盤, false);
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待作業.GetEnumName();
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.收支原因] = "盤點異常";
                        list_取藥堆疊母資料_replace.Add(list_取藥堆疊母資料[i]);
                        break;
                    }
                    try_error++;
                    retry++;
                }
                Fuction_時間重置();
            }
            if (list_取藥堆疊母資料_replace.Count > 0)
            {
                Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_replace, false);
                Main_Form._sqL_DataGridView_取藥堆疊母資料.ReplaceExtra(list_取藥堆疊母資料, true);
                cnt = 1;
                return;
            }
            cnt++;

        }
        void cnt_Program_刷新領藥內容_檢查複盤作業(ref int cnt)
        {
            List<object[]> list_取藥堆疊母資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            List<object[]> list_取藥堆疊母資料_replace = new List<object[]>();

            list_取藥堆疊母資料 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.等待複盤.GetEnumName());
            for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
            {
                int try_error = 0;
                string 藥碼 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 藥名 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                string 結存量 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.結存量].ObjectToString();
                double 總異動量 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.總異動量].StringToDouble();

                Voice.MediaPlayAsync($@"{Main_Form.currentDirectory}\請輸入複盤數量.wav");
                while (true)
                {
                    if (try_error == 1)
                    {
                        Dialog_盤點數量錯誤 dialog_盤點數量錯誤 = new Dialog_盤點數量錯誤();
                        if (dialog_盤點數量錯誤.ShowDialog() == DialogResult.Yes)
                        {
                            Fuction_時間重置();
                            try_error = 0;
                        }
                        else
                        {
                            Fuction_時間重置();
                            try_error++;
                        }
                        continue;
                    }
                    if (try_error == 2)
                    {
                        Dialog_收支原因選擇 dialog_收支原因選擇 = new Dialog_收支原因選擇();
                        dialog_收支原因選擇.Title = $"複盤數量錯誤({結存量}) 選擇原因";
                        dialog_收支原因選擇.ShowDialog();
                        Fuction_時間重置();
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.收支原因] = $"{list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.收支原因].ObjectToString()} \n複盤錯誤原因:{dialog_收支原因選擇.Value}";
                        Main_Form.Function_取藥堆疊資料_設定作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.複盤, false);
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待作業.GetEnumName();
                        list_取藥堆疊母資料_replace.Add(list_取藥堆疊母資料[i]);
                        break;
                    }

                    Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel($"(明盤)請輸入取藥後盤點數量\n交易量 : {總異動量}", $"藥碼:{藥碼} \n藥名:{藥名}");
                    dialog_NumPannel.TitleFont = new Font("微軟正黑體", 20, FontStyle.Bold);
                    dialog_NumPannel.X_Visible = true;
              
                    if (dialog_NumPannel.ShowDialog() != DialogResult.Yes)
                    {
                        //list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.取消作業.GetEnumName();
                        Main_Form.Function_取藥堆疊資料_刪除母資料(list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString());
                        Fuction_時間重置();
                        break;
                    }
                    Fuction_時間重置();
                    list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.盤點量] = dialog_NumPannel.Value.ToString();
                    if (結存量 == dialog_NumPannel.Value.ToString())
                    {
                        Main_Form.Function_取藥堆疊資料_設定作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.複盤, false);
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待作業.GetEnumName();
                        list_取藥堆疊母資料_replace.Add(list_取藥堆疊母資料[i]);
                        break;
                    }
                    Voice.MediaPlayAsync($@"{Main_Form.currentDirectory}\複盤數量錯誤.wav");
                    try_error++;

                }


                list_取藥堆疊母資料_replace.Add(list_取藥堆疊母資料[i]);
            }
            if (list_取藥堆疊母資料_replace.Count > 0)
            {
                Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_replace, false);
                cnt = 1;
                return;
            }
            cnt++;

        }
        void cnt_Program_刷新領藥內容_檢查作業完成(ref int cnt)
        {
            List<object[]> list_取藥堆疊母資料 = new List<object[]>();
            List<object[]> list_取藥堆疊子資料 = new List<object[]>();

            if (Main_Form.myConfigClass.系統取藥模式)
            {
                list_取藥堆疊母資料 = Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
                list_取藥堆疊子資料 = Main_Form._sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
            }
            else
            {
                list_取藥堆疊母資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
                list_取藥堆疊子資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱);
            }

            List<object[]> list_取藥堆疊子資料_buf = new List<object[]>();
            List<object[]> list_取藥堆疊子資料_replace = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_replace = new List<object[]>();
            list_取藥堆疊母資料 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.作業完成.GetEnumName());
            for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
            {
                string Master_GUID = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                string 藥碼 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 藥名 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                //if (Function_取藥堆疊資料_取得作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.複盤))
                //{
                //    voice.SpeakOnTask("請輸入盤點數量");
                //    Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel($"藥碼:{藥碼} 藥名:{藥名}  請輸入盤點數量");
                //    dialog_NumPannel.ShowDialog();
                //    list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.盤點量] = dialog_NumPannel.Value.ToString();
                //    list_取藥堆疊母資料_replace.Add(list_取藥堆疊母資料[i]);
                //}

                list_取藥堆疊子資料_buf = list_取藥堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);
                for (int k = 0; k < list_取藥堆疊子資料_buf.Count; k++)
                {
                    list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.調劑結束] = true.ToString();
                    list_取藥堆疊子資料_replace.Add(list_取藥堆疊子資料_buf[k]);
                }
            }
            if (list_取藥堆疊母資料_replace.Count > 0) Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_replace, false);
            if (list_取藥堆疊子資料_replace.Count > 0) Main_Form._sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥堆疊子資料_replace, false);
            cnt++;
        }
        void cnt_Program_刷新領藥內容_檢查是否需輸入效期(ref int cnt)
        {
            List<object[]> list_取藥堆疊資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            list_取藥堆疊資料 = list_取藥堆疊資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.輸入新效期.GetEnumName());

            string GIUD = "";
            for (int i = 0; i < list_取藥堆疊資料.Count; i++)
            {
                Dialog_輸入效期 dialog = new Dialog_輸入效期();
                DialogResult dialogResult = DialogResult.None;
                this.Invoke(new Action(delegate
                {
                    dialogResult = dialog.ShowDialog();

                }));
                if (dialogResult != DialogResult.Yes)
                {
                    cnt = 65500;
                    return;
                }
                string 效期 = dialog.效期.StringToDateTime().ToDateString(TypeConvert.Enum_Year_Type.Anno_Domini, "/");
                dialog.Dispose();
                GIUD = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.效期] = 效期;
                list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.新增效期.GetEnumName();

                Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_Replace(enum_取藥堆疊母資料.GUID.GetEnumName(), GIUD, list_取藥堆疊資料[i], false);
            }
            cnt++;
        }
        void cnt_Program_刷新領藥內容_檢查是否需選擇效期(ref int cnt)
        {
            List<object[]> list_取藥堆疊資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            list_取藥堆疊資料 = list_取藥堆疊資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.選擇效期.GetEnumName());

            string GIUD = "";
            for (int i = 0; i < list_取藥堆疊資料.Count; i++)
            {
                string 藥品碼 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 藥品名稱 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                double 交易量 = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.總異動量].StringToDouble();
                List<string> list_效期 = new List<string>();
                List<string> list_效期_buf = new List<string>();
                List<string> list_批號 = new List<string>();
                List<string> list_數量 = new List<string>();

                List<object> list_device = Main_Form.Function_從SQL取得儲位到本地資料(藥品碼);
                if (list_device.Count == 0)
                {
                    list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.無儲位.GetEnumName();
                    Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_Replace(list_取藥堆疊資料[i], false);
                    continue;
                }
                for (int k = 0; k < list_device.Count; k++)
                {
                    Device device = list_device[k] as Device;
                    if (device != null)
                    {
                        for (int m = 0; m < device.List_Validity_period.Count; m++)
                        {
                            list_效期_buf = (from value in list_效期
                                           where value == device.List_Validity_period[m]
                                           select value).ToList();
                            if (list_效期_buf.Count == 0)
                            {
                                list_效期.Add(device.List_Validity_period[m]);
                                list_批號.Add(device.List_Lot_number[m]);
                                list_數量.Add(device.List_Inventory[m]);
                            }
                        }
                    }
                }
                Dialog_選擇效期 dialog = new Dialog_選擇效期(藥品碼, 藥品名稱, 交易量, list_效期, list_批號, list_數量);
                DialogResult dialogResult = DialogResult.None;
                this.Invoke(new Action(delegate
                {
                    Main_Form.voice.SpeakOnTask("請選擇效期");
                    dialogResult = dialog.ShowDialog();

                }));
                if (dialogResult != DialogResult.Yes)
                {
                    list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.取消作業.GetEnumName();
                    Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_Replace(list_取藥堆疊資料[i], false);
                    cnt = 65500;
                    return;
                }

                GIUD = list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.效期] = dialog.Value;
                list_取藥堆疊資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName();
                dialog.Dispose();
                Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_Replace(list_取藥堆疊資料[i], false);
            }
            cnt++;
        }
        void cnt_Program_刷新領藥內容_檢查自動登出(ref int cnt)
        {
            List<object[]> list_取藥堆疊資料 = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            List<object[]> list_取藥堆疊資料_buf = new List<object[]>();
            list_取藥堆疊資料_buf = (from value in list_取藥堆疊資料
                               where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName()
                               where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName()
                               where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()
                               where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.無可匹配數量.GetEnumName()
                               select value
                                ).ToList();

            if (list_取藥堆疊資料.Count == 0 && !PLC_Device_單醫令模式.Bool == false)
            {
                MyTimer_閒置登出時間.TickStop();
                MyTimer_閒置登出時間.StartTickTime();

                MyTimer_入賬完成時間.TickStop();
                MyTimer_入賬完成時間.StartTickTime();
            }
            else
            {
                if (list_取藥堆疊資料_buf.Count > 0)
                {
                    MyTimer_閒置登出時間.TickStop();
                    MyTimer_閒置登出時間.StartTickTime();

                    MyTimer_入賬完成時間.TickStop();
                    MyTimer_入賬完成時間.StartTickTime();
                }
                else
                {
                    MyTimer_閒置登出時間.StartTickTime();
                    MyTimer_入賬完成時間.StartTickTime();
                    if (PLC_Device_閒置登出時間.Value != 0)
                    {
                        if(Main_Form.PLC_Device_閒置登出要警示.Bool == true)
                        {
                            if ((PLC_Device_閒置登出時間.Value - (int)MyTimer_閒置登出時間.GetTickTime()) <= 20000)
                            {
                                myTimer_Logout.StartTickTime(5000);
                                if (myTimer_Logout.IsTimeOut())
                                {
                                    myTimer_Logout.TickStop();
                                    Task.Run(new Action(delegate
                                    {
                                        using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{Main_Form.currentDirectory}\logout.wav"))
                                        {
                                            sp.Stop();
                                            sp.Play();
                                            sp.PlaySync();
                                        }

                                    }));
                                }
                            }
                        }
                       
                    }

                }
            }
            this.MyTimer__刷新領藥內容_刷新間隔.TickStop();
            this.MyTimer__刷新領藥內容_刷新間隔.StartTickTime(100);
            cnt++;
        }
        void cnt_Program_刷新領藥內容_等待刷新間隔(ref int cnt)
        {
            if (this.MyTimer__刷新領藥內容_刷新間隔.IsTimeOut())
            {
                cnt++;
            }
        }
        #endregion

        private void ToolStripMenuItem_調劑畫面_顯示設定_Click(object sender, EventArgs e)
        {
            Control control = contextMenuStrip_調劑畫面.SourceControl;

            Dialog_調劑畫面顯示調整 dialog_調劑畫面顯示調整 = new Dialog_調劑畫面顯示調整(index);
            if (dialog_調劑畫面顯示調整.ShowDialog() != DialogResult.Yes) return;
            Main_Form.SaveConfig工程模式();
        }
        private void TextBox_密碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Login();
            }
        }
        private void TextBox_帳號_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox_密碼.Focus();
            }
        }
        private void SqL_DataGridView_領藥內容_DataGridClearGridEvent()
        { 
            Function_調劑作業_醫令資訊更新();
        }
        private void SqL_DataGridView_領藥內容_RowEnterEvent(object[] RowValue)
        {
            string 藥碼 = RowValue[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
            string 藥名 = RowValue[(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
            string 姓名 = RowValue[(int)enum_取藥堆疊母資料.病人姓名].ObjectToString();
            string 年齡 = "";
            string 領藥號 = RowValue[(int)enum_取藥堆疊母資料.領藥號].ObjectToString();
            string 病歷號 = RowValue[(int)enum_取藥堆疊母資料.病歷號].ObjectToString();
            string 開方時間 = RowValue[(int)enum_取藥堆疊母資料.開方時間].ObjectToString();
            Function_調劑作業_醫令資訊更新(藥碼, 藥名, 姓名, 年齡, 領藥號, 病歷號, 開方時間);
        }
        private void SqL_DataGridView_領藥內容_DataGridRefreshEvent()
        {
            String 狀態 = "";
            for (int i = 0; i < this.sqL_DataGridView_領藥內容.dataGridView.Rows.Count; i++)
            {
                狀態 = this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].Cells[(int)enum_取藥堆疊母資料.狀態].Value.ToString();
                if (狀態 == enum_取藥堆疊母資料_狀態.等待作業.GetEnumName())
                {
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (狀態 == enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName())
                {
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (狀態 == enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName())
                {
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (狀態 == enum_取藥堆疊母資料_狀態.無儲位.GetEnumName())
                {
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (狀態 == enum_取藥堆疊母資料_狀態.無可匹配數量.GetEnumName())
                {
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (狀態 == enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName())
                {
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (狀態 == enum_取藥堆疊母資料_狀態.已領用過.GetEnumName())
                {
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.DimGray;
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                }
                else if (狀態 == enum_取藥堆疊母資料_狀態.未授權.GetEnumName())
                {
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.DarkRed;
                    this.sqL_DataGridView_領藥內容.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                }
            }
        }
        private void SqL_DataGridView_領藥內容_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {
            try
            {
                RowsList = Function_領藥內容_重新排序(RowsList);
            }
            catch
            (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message);
            }
        }
   
        private void PlC_Button_退_ValueChangeEvent(bool Value)
        {
            this.plC_Button_領.Bool = !Value;
            this.plC_Button_退.Bool = Value;
        }
        private void PlC_Button_領_ValueChangeEvent(bool Value)
        {
            this.plC_Button_領.Bool = Value;
            this.plC_Button_退.Bool = !Value;
        }
        private void PlC_RJ_Button_登出_MouseDownEvent(MouseEventArgs mevent)
        {
            if (!this.PLC_Device_已登入.Bool) return;
  
            Function_調劑作業_醫令資訊更新();
            Main_Form.Function_取藥堆疊資料_刪除指定調劑台名稱母資料(調劑台名稱);
            this.sqL_DataGridView_領藥內容.ClearGrid();

            Main_Form.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.登出, 登入者姓名, 調劑台名稱);
            登入者姓名 = "None";
            this.PLC_Device_已登入.Bool = false;
            Main_Form.PLC_Device_最高權限.Bool = false;
            this.Invoke(new Action(delegate
            {
                textBox_帳號.Texts = "";
                textBox_密碼.Texts = "";
                plC_RJ_Button_登入.Texts = "登入";
                rJ_Lable_Title.Text = $" {(Main_Form.PLC_Device_導引模式.Bool ? "(導引模式)" : "")}[未登入]";
                this.rJ_Lable_Title.BorderColor = Color.DimGray;
                this.rJ_Lable_Title.BackgroundColor = Color.DimGray;
                this.rJ_Lable_Title.TextColor = Color.White;

            }));
        }
        private void PlC_RJ_Button_登入_MouseDownEvent(MouseEventArgs mevent)
        {
            if (Main_Form.Function_檢查是否完成交班() == false)
            {
                MyMessageBox.ShowDialog("請先完成交班");
                return;
            }
            if (plC_RJ_Button_登入.Texts == "登出")
            {
                PlC_RJ_Button_登出_MouseDownEvent(null);
                return;
            }
            Main_Form.Function_取藥堆疊資料_刪除指定調劑台名稱母資料(調劑台名稱);
            if (PLC_Device_已登入.Bool) return;
            if (textBox_帳號.Texts.StringIsEmpty()) return;

            if (textBox_帳號.Texts.ToUpper() == Main_Form.Admin_ID.ToUpper())
            {
                if (textBox_密碼.Texts.ToUpper() == Main_Form.Admoin_Password.ToUpper())
                {
                    this.PLC_Device_已登入.Bool = true;
                    登入者姓名 = "最高管理權限";
                    ID = "admin";
                    Main_Form.PLC_Device_最高權限.Bool = true;
                    return;
                }
            }

            List<object[]> list_value = Main_Form._sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_人員資料.ID, textBox_帳號.Texts);
            if (list_value.Count == 0)
            {
                Dialog_錯誤提示 dialog_錯誤提示 = new Dialog_錯誤提示("查無此帳號", 2000);
                dialog_錯誤提示.ShowDialog();
                //MyMessageBox.ShowDialog("查無此帳號!");
                return;
            }
            string password = list_value[0][(int)enum_人員資料.密碼].ObjectToString();
            if (textBox_密碼.Texts != password)
            {
                Dialog_錯誤提示 dialog_錯誤提示 = new Dialog_錯誤提示("密碼錯誤", 2000);
                dialog_錯誤提示.ShowDialog();
                //MyMessageBox.ShowDialog("密碼錯誤!");
                return;
            }
            登入者姓名 = list_value[0][(int)enum_人員資料.姓名].ObjectToString();
            ID = list_value[0][(int)enum_人員資料.ID].ObjectToString();
            密碼 = list_value[0][(int)enum_人員資料.密碼].ObjectToString();
            if (Main_Form.PLC_Device_掃碼顏色固定.Bool)
            {
                if (index == 0) 顏色 = Main_Form._panel_工程模式_領藥台_01_顏色.BackColor.ToColorString();
                if (index == 1) 顏色 = Main_Form._panel_工程模式_領藥台_02_顏色.BackColor.ToColorString();
                if (index == 2) 顏色 = Main_Form._panel_工程模式_領藥台_03_顏色.BackColor.ToColorString();
                if (index == 3) 顏色 = Main_Form._panel_工程模式_領藥台_04_顏色.BackColor.ToColorString();
            }
            else 顏色 = list_value[0][(int)enum_人員資料.顏色].ObjectToString();
            藥師證字號 = list_value[0][(int)enum_人員資料.藥師證字號].ObjectToString();
            this.PLC_Device_已登入.Bool = true;
            if (mevent != null) Main_Form.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.密碼登入, 登入者姓名, 調劑台名稱);
  
            this.plC_Button_領.Bool = true;
            this.plC_Button_退.Bool = false;

            Console.WriteLine($"登入成功! ID : {ID}, 名稱 : {登入者姓名}");
            this.Invoke(new Action(delegate
            {
                textBox_帳號.Texts = "";
                textBox_密碼.Texts = "";
                plC_RJ_Button_登入.Texts = "登出";
                string text_temp = PLC_Device_單醫令模式.Bool ? "【單醫令】" : "【多醫令】";
                rJ_Lable_Title.Text = $" {(Main_Form.PLC_Device_導引模式.Bool ? "(導引模式)" : "")}[{登入者姓名}] {text_temp}";
                //this.rJ_Lable_Title.BorderColor = this.panel_工程模式_顏色.BackColor;
                this.rJ_Lable_Title.BackgroundColor = Color.GreenYellow;
                this.rJ_Lable_Title.TextColor = Color.Black;
            }));
            Main_Form.commonSapceClasses = Main_Form.Function_取得共用區所有儲位();
            //$"{登入者姓名},登入成功".PlayGooleVoiceAsync(Main_Form.API_Server);

            醫令條碼 = "";
        }
        private void PlC_RJ_Button_取消作業_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.PLC_Device_已登入.Bool == false && Main_Form.PLC_Device_導引模式.Bool == false) return;
            if (this.PLC_Device_已登入.Bool == false && Main_Form.PLC_Device_導引模式.Bool == true)
            {
                rJ_Lable_Title.Text = $" {(Main_Form.PLC_Device_導引模式.Bool ? "(導引模式)" : "")}[未登入]";
            }
            //Main_Form.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.取消作業, 登入者姓名, 調劑台名稱);
            Main_Form.Function_取藥堆疊資料_刪除指定調劑台名稱母資料(調劑台名稱);
            Main_Form.commonSapceClasses.DeleteTakeMedicineStack(調劑台名稱);
            Function_調劑作業_醫令資訊更新();
            this.sqL_DataGridView_領藥內容.ClearGrid();
        }

        public void Function_調劑作業_醫令資訊更新()
        {
            this.Invoke(new Action(delegate
            {
                if (pictureBox_藥品圖片01.BackgroundImage != null) pictureBox_藥品圖片01.BackgroundImage.Dispose();
                pictureBox_藥品圖片01.BackgroundImage = null;
                pictureBox_藥品圖片01.Visible = false;
                if (pictureBox_藥品圖片02.BackgroundImage != null) pictureBox_藥品圖片02.BackgroundImage.Dispose();
                pictureBox_藥品圖片02.BackgroundImage = null;
                pictureBox_藥品圖片02.Visible = false;
                suspiciousRxLog = null;

                this.rJ_Lable_姓名.Text = "---------";
                this.rJ_Lable_年齡.Text = "----";
                this.rJ_Lable_領藥號.Text = "-----";
                this.rJ_Lable_病歷號.Text = "---------";
                rJ_Lable_診斷.Text = "";
            }));

        }
        public void Function_調劑作業_醫令資訊更新(string 藥碼, string 藥名,string 姓名,string 年齡, string 領藥住院號, string 病歷號, string 開方時間)
        {

            Task.Run(new Action(delegate
            {
                List<Image> images = new List<Image>();
                if (Main_Form.PLC_Device_AI處方核對啟用.Bool == false) images = Main_Form.Function_取得藥品圖片(藥碼);
                this.Invoke(new Action(delegate
                {
                    if (藥名.StringIsEmpty()) 藥名 = "-------------------------";
                    if (領藥住院號.StringIsEmpty()) 領藥住院號 = "-----";
                    if (病歷號.StringIsEmpty()) 病歷號 = "---------";
                    if (開方時間.Check_Date_String() == false) 開方時間 = "---------";
                    else 開方時間 = 開方時間.StringToDateTime().ToDateTimeString();
                    if (姓名.StringIsEmpty()) 姓名 = "---------";
                    if (年齡.StringIsEmpty()) 年齡 = "----";
                    if (Main_Form.PLC_Device_AI處方核對啟用.Bool == false)
                    {
                        if (images.Count >= 2)
                        {
                            if (pictureBox_藥品圖片01.BackgroundImage != null) pictureBox_藥品圖片01.BackgroundImage.Dispose();
                            if (pictureBox_藥品圖片02.BackgroundImage != null) pictureBox_藥品圖片02.BackgroundImage.Dispose();
                            if (images[0] != null && images[1] != null)
                            {
                                pictureBox_藥品圖片01.BackgroundImage = images[0];
                                pictureBox_藥品圖片02.BackgroundImage = images[1];
                                pictureBox_藥品圖片01.Visible = true;
                                pictureBox_藥品圖片02.Visible = true;
                            }
                            else if (images[0] == null && images[1] != null)
                            {
                                pictureBox_藥品圖片01.BackgroundImage = images[1];
                                pictureBox_藥品圖片01.Visible = true;
                                pictureBox_藥品圖片02.Visible = false;
                            }
                            else if (images[0] != null && images[1] == null)
                            {
                                pictureBox_藥品圖片01.BackgroundImage = images[0];
                                pictureBox_藥品圖片01.Visible = true;
                                pictureBox_藥品圖片02.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        // 範例欄位（你應從某處傳入這些屬性）
                      
                    }
                       


                    this.rJ_Lable_姓名.Text = 姓名;
                    this.rJ_Lable_年齡.Text = 年齡;
                    this.rJ_Lable_領藥號.Text = 領藥住院號;
                    this.rJ_Lable_病歷號.Text = 病歷號;



                }));
            }));
        }
        public static Image DrawSimpleWarningImage(string level, string errorType, string eventDesc, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                // 設定字型
                Font titleFont = new Font("Arial", 14, FontStyle.Bold);
                Font labelFont = new Font("Arial", 12, FontStyle.Bold);
                Font textFont = new Font("Arial", 12, FontStyle.Regular);
                Font exclamationFont = new Font("Arial", 24, FontStyle.Bold);

                // 上方：警示色塊（固定高 35）
                int topHeight = 35;
                Brush headerBrush = level == "Critical" ? Brushes.Red : Brushes.Yellow;
                g.FillRectangle(headerBrush, 0, 0, width, topHeight);

                // 顯示 "Critical Warning" 或 "Warning"
                g.DrawString($"{level} Warning", titleFont, Brushes.Black, new PointF(35, 5));

                // 若為 Critical，加入驚嘆號圖示（❗）
                if (level == "Critical")
                {
                    g.DrawString("❗", exclamationFont, Brushes.White, new PointF(5, 0));
                }

                // 中段：錯誤類別
                string errorTypeText = "錯誤類別: " + errorType;
                SizeF errorSize = g.MeasureString(errorTypeText, labelFont, width - 10);
                float errorTop = topHeight + 5;
                g.DrawString(errorTypeText, labelFont, Brushes.Black, new RectangleF(5, errorTop, width - 10, errorSize.Height));

                // 下方：簡述事件
                float eventTop = errorTop + errorSize.Height + 5;
                float eventHeight = height - eventTop - 5;
                g.DrawString(eventDesc, textFont, Brushes.Black, new RectangleF(5, eventTop, width - 10, eventHeight));
            }

            return bmp;
        }
        public async void Function_醫令領藥(string BarCode)
        {
            personPageClass personPageClass = new personPageClass();
            personPageClass.ID = ID;
            personPageClass.姓名 = 登入者姓名;
            personPageClass.密碼 = 密碼;
            personPageClass.藥師證字號 = 藥師證字號;
            personPageClass.顏色 = 顏色;
            List<OrderClass> orderClasses = Main_Form.Function_醫令領藥(BarCode, personPageClass, 調劑台名稱, PLC_Device_單醫令模式.Bool);



            if (orderClasses != null)
            {
                if (Main_Form.PLC_Device_AI處方核對啟用.Bool || Main_Form.PLC_Device_顯示診斷訊息.Bool)
                {
                    Task.Run(new Action(delegate
                    {
                        this.Invoke(new Action(delegate
                        {
                            rJ_Lable_診斷.Text = "";
                        }));

                        for (int i = 0; i < orderClasses.Count; i++)
                        {
                            orderClasses[i].藥師姓名 = 登入者姓名;
                            orderClasses[i].藥師ID = ID;
                        }
                        (int code, string resuult, suspiciousRxLogClass suspiciousRxLogClass) = suspiciousRxLogClass.medGPT_full(Main_Form.API_Server, orderClasses);
                        if (code == -200)
                        {
                            Logger.Log("error", resuult);
                            //return;
                        }
                        suspiciousRxLog = suspiciousRxLogClass;

                        if (suspiciousRxLog == null) return;
                        if(suspiciousRxLog.年齡.StringIsEmpty() == false)
                        {
                            年齡_ = suspiciousRxLog.年齡;
                        }
                      
                        
                        string text = "";
                        int text_height = 30;
                        int text_height_basic = TextRenderer.MeasureText("測試", rJ_Lable_診斷.Font).Height;
                        if(suspiciousRxLog.diseaseClasses != null)
                        {
                            for (int i = 0; i < suspiciousRxLog.diseaseClasses.Count; i++)
                            {
                                text += $"  {i + 1}.[{suspiciousRxLog.diseaseClasses[i].疾病代碼.StringLength(0)}]{suspiciousRxLog.diseaseClasses[i].中文說明}";
                                if (i != suspiciousRxLog.diseaseClasses.Count - 1 || suspiciousRxLog.交互作用紀錄 != null) text += "\n";
                                text_height += text_height_basic;
                            }
                        }
                        if (suspiciousRxLog.交互作用紀錄 != null)
                        {
                            for (int i = 0; i < suspiciousRxLog.交互作用紀錄.Count; i++)
                            {
                                text += $"  ※ [{suspiciousRxLog.交互作用紀錄[i].code.StringLength(0)}]{suspiciousRxLog.交互作用紀錄[i].name}";
                                if (i != suspiciousRxLog.交互作用紀錄.Count - 1) text += "\n";
                                text_height += text_height_basic;
                            }
                        }
                        string text_用藥警示 = "";
                        if (suspiciousRxLog.過敏紀錄 != null)
                        {
                            for (int i = 0; i < suspiciousRxLog.過敏紀錄.Count; i++)
                            {
                                text_用藥警示 += $"  ※ [{suspiciousRxLog.過敏紀錄[i].code.StringLength(0)}]{suspiciousRxLog.過敏紀錄[i].name}";
                                if (i != suspiciousRxLog.過敏紀錄.Count - 1) text_用藥警示 += "\n";
                            }
                        }
                       
                       
                        if(text_用藥警示.StringIsEmpty() == false)
                        {
                            Voice.MediaPlayAsync($@"{Main_Form.currentDirectory}\alarm.wav");
                            Console.WriteLine($"{text_用藥警示}");

                            Task.Run(new Action(delegate
                            {
                                Dialog_用藥警示 dialog_用藥警示 = new Dialog_用藥警示(text_用藥警示);
                                dialog_用藥警示.ShowDialog();

                            }));


                        }
                       
                        this.Invoke(new Action(delegate
                        {
                            if (pictureBox_藥品圖片01.BackgroundImage != null)
                                pictureBox_藥品圖片01.BackgroundImage.Dispose();
                            if (pictureBox_藥品圖片02.BackgroundImage != null)
                                pictureBox_藥品圖片02.BackgroundImage.Dispose();

                            pictureBox_藥品圖片01.Visible = false;
                            pictureBox_藥品圖片02.Visible = false;
                            rJ_Lable_診斷.Text = text;
                            panel_診斷及交互資訊.Height = text_height;
                            panel_診斷及交互資訊.Visible = true;

                            if (Main_Form.PLC_Device_AI處方核對啟用.Bool)
                            {
                                string 提報等級 = this.suspiciousRxLog?.提報等級;
                                string 錯誤類別 = this.suspiciousRxLog?.錯誤類別;
                                string 簡述事件 = this.suspiciousRxLog?.簡述事件;
                                if (!string.IsNullOrEmpty(提報等級))
                                {
                                    Voice.MediaPlayAsync($@"{Main_Form.currentDirectory}\處方有疑義,請審核.wav");



                                    if (提報等級 == enum_suspiciousRxLog_ReportLevel.Normal.GetEnumName() || 提報等級 == enum_suspiciousRxLog_ReportLevel.Important.GetEnumName())
                                    {
                                        if (pictureBox_藥品圖片01.BackgroundImage != null)
                                            pictureBox_藥品圖片01.BackgroundImage.Dispose();
                                        pictureBox_藥品圖片01.Visible = true;
                                        pictureBox_藥品圖片02.Visible = false;
                                        pictureBox_藥品圖片01.Invalidate();
                                        //panel_藥品圖片.Refresh();
                                        int pbWidth = pictureBox_藥品圖片01.Width;
                                        int pbHeight = pictureBox_藥品圖片01.Height;
                                        pictureBox_藥品圖片01.BackgroundImage = DrawSimpleWarningImage("Normal", 錯誤類別, 簡述事件, pbWidth, pbHeight);

                                    }
                                    else if (提報等級 == enum_suspiciousRxLog_ReportLevel.Critical.GetEnumName())
                                    {
                                        if (pictureBox_藥品圖片02.BackgroundImage != null)
                                            pictureBox_藥品圖片02.BackgroundImage.Dispose();
                                        pictureBox_藥品圖片01.Visible = false;
                                        pictureBox_藥品圖片02.Visible = true;
                                        pictureBox_藥品圖片02.Invalidate();
                                        //panel_藥品圖片.Refresh();
                                        int pbWidth = pictureBox_藥品圖片02.Width;
                                        int pbHeight = pictureBox_藥品圖片02.Height;
                                        pictureBox_藥品圖片02.BackgroundImage = DrawSimpleWarningImage("Critical", 錯誤類別, 簡述事件, pbWidth, pbHeight);
                                        PictureBox_藥品圖片_Click(null, null);
                                    }
                                    //else
                                    //{
                                    //    if (pictureBox_藥品圖片01.BackgroundImage != null)
                                    //        pictureBox_藥品圖片01.BackgroundImage.Dispose();
                                    //    if (pictureBox_藥品圖片02.BackgroundImage != null)
                                    //        pictureBox_藥品圖片02.BackgroundImage.Dispose();
                                    //}
                                }
                            }
                               
                        }));


                    }));
                }
            }

        }
        public void Function_醫令退藥(string BarCode)
        {
            personPageClass personPageClass = new personPageClass();
            personPageClass.ID = ID;
            personPageClass.姓名 = 登入者姓名;
            personPageClass.藥師證字號 = 藥師證字號;
            personPageClass.顏色 = 顏色;
            Main_Form.Function_醫令退藥(BarCode, personPageClass, 調劑台名稱, PLC_Device_單醫令模式.Bool);
        }
        public void Fuction_時間重置()
        {
            MyTimer_閒置登出時間.TickStop();
            MyTimer_閒置登出時間.StartTickTime();
            MyTimer_入賬完成時間.TickStop();
            MyTimer_入賬完成時間.StartTickTime();
        }
        public bool Function_檢查是否為藥品條碼(string barcode)
        {
            List<medClass> medClasses = medClass.serch_by_BarCode(Main_Form.API_Server, barcode);
            personPageClass personPageClass = new personPageClass();
            bool flag = false;
            if (medClasses.Count > 0)
            {
                flag = true;
                if(Main_Form.PLC_Device_掃碼顏色固定.Bool)
                {
                    if (index == 0) 顏色 = Main_Form._panel_工程模式_領藥台_01_顏色.BackColor.ToColorString();
                    if (index == 1) 顏色 = Main_Form._panel_工程模式_領藥台_02_顏色.BackColor.ToColorString();
                    if (index == 2) 顏色 = Main_Form._panel_工程模式_領藥台_03_顏色.BackColor.ToColorString();
                    if (index == 3) 顏色 = Main_Form._panel_工程模式_領藥台_04_顏色.BackColor.ToColorString();

                }
            

                List<medConfigClass> medConfigClasses = medConfigClass.get_dispense_note_by_codes(Main_Form.API_Server, medClasses[0].藥品碼);

                if (medConfigClasses.Count > 0)
                {
                    Dialog_使用者登入 dialog_使用者登入 = new Dialog_使用者登入();
                    dialog_使用者登入.ShowDialog();
                    if (dialog_使用者登入.DialogResult != DialogResult.Yes) return flag;
                    personPageClass = dialog_使用者登入.personPageClass;
                    this.Title = $" {(Main_Form.PLC_Device_導引模式.Bool ? "(導引模式)" : "")}[{ personPageClass.姓名}]";

                }

                if (medConfigClasses.Count > 0)
                {
                    Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel("請輸入【退藥】數量");
                    if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return flag;

                    Main_Form.Function_取藥堆疊資料_刪除指定調劑台名稱母資料(調劑台名稱);
                    takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
                    takeMedicineStackClass.藥品碼 = medClasses[0].藥品碼;
                    takeMedicineStackClass.藥品名稱 = medClasses[0].藥品名稱;
                    takeMedicineStackClass.動作 = enum_交易記錄查詢動作.系統退藥.GetEnumName();
                    takeMedicineStackClass.總異動量 = dialog_NumPannel.Value.ToString();
                    takeMedicineStackClass.調劑台名稱 = 調劑台名稱;
                    takeMedicineStackClass.顏色 = this.顏色;
                    takeMedicineStackClass.操作人 = personPageClass.姓名;
                    takeMedicineStackClass.ID = personPageClass.ID;
                    takeMedicineStackClass.藥師證字號 = personPageClass.藥師證字號;
                    Main_Form.Function_取藥堆疊資料_新增母資料(takeMedicineStackClass);
                }
                else
                {
                    Main_Form.Function_取藥堆疊資料_刪除指定調劑台名稱母資料(調劑台名稱);
                    takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
                    takeMedicineStackClass.藥品碼 = medClasses[0].藥品碼;
                    takeMedicineStackClass.藥品名稱 = medClasses[0].藥品名稱;
                    takeMedicineStackClass.動作 = enum_交易記錄查詢動作.系統領藥.GetEnumName();
                    takeMedicineStackClass.總異動量 = "0";
                    takeMedicineStackClass.調劑台名稱 = 調劑台名稱;
                    takeMedicineStackClass.顏色 = this.顏色;
                    takeMedicineStackClass.操作人 = personPageClass.姓名;
                    takeMedicineStackClass.ID = personPageClass.ID;
                    takeMedicineStackClass.藥師證字號 = personPageClass.藥師證字號;
                    Main_Form.Function_取藥堆疊資料_新增母資料(takeMedicineStackClass);
                }

                return flag;
            }
            return flag;
        }
        private List<object[]> Function_領藥內容_重新排序(List<object[]> list_value)
        {
            List<object[]> list_value_buf = new List<object[]>();
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.新增資料.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.雙人覆核.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.等待複盤.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.盲盤完成.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.複盤完成.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.等待作業.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.作業完成.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.新增效期.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.輸入新效期.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.選擇效期.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.已領用過.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.未授權.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.DC處方.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.無可匹配數量.GetEnumName()));
            list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.RFID使用.GetEnumName()));
            if (!Main_Form._plC_CheckBox_領藥無儲位不顯示.Checked) list_value_buf.LockAdd(list_value.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()));
            return list_value_buf;
        }
        public void CheckFpMatchLogin()
        {
            if (FpMatchClass_指紋資訊 != null)
            {
                List<object[]> list_人員資料 = Main_Form._sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                object[] value = null;
                for (int i = 0; i < list_人員資料.Count; i++)
                {
                    string feature = list_人員資料[i][(int)enum_人員資料.指紋辨識].ObjectToString();
                    if (Main_Form.fpMatchSoket.Match(FpMatchClass_指紋資訊.feature, feature))
                    {
                        value = list_人員資料[i];
                        break;
                    }
                }
                FpMatchClass_指紋資訊 = null;
                if (value == null)
                {
                    Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("找無符合指紋資訊", 2000);
                    dialog_AlarmForm.ShowDialog();
                    return;
                }
                if (PLC_Device_已登入.Bool && (ID != value[(int)enum_人員資料.ID].ObjectToString())) Logout();

                this.Invoke(new Action(delegate
                {
                    textBox_帳號.Texts = value[(int)enum_人員資料.ID].ObjectToString();
                    textBox_密碼.Texts = value[(int)enum_人員資料.密碼].ObjectToString();
                    Login();
                }));

                Main_Form.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.指紋登入, 登入者姓名, "01.號使用者");
            }
        }
    }
}
