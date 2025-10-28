using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using MyUI;
using Basic;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using HIS_DB_Lib;
using H_Pannel_lib;
using SQLUI;
using DrawingClass;
namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
        static public PLC_CheckBox _plC_CheckBox_領藥無儲位不顯示;
        public enum enum_ContextMenuStrip_Main_領藥內容
        {
            修改數量,
        }
        public enum enum_ContextMenuStrip_Main_醫令檢索
        {
            [Description("M2500")]
            病歷號,
            [Description("M2501")]
            領藥號,
            [Description("M8000")]
            長期用藥,
        }
        static public UC_調劑作業_TypeA _uC_調劑作業_TypeA_1;
        static public UC_調劑作業_TypeA _uC_調劑作業_TypeA_2;
        static public UC_調劑作業_TypeA _uC_調劑作業_TypeA_3;
        static public UC_調劑作業_TypeA _uC_調劑作業_TypeA_4;
        static public SQL_DataGridView _sqL_DataGridView_領藥台_01_領藥內容;
        static public SQL_DataGridView _sqL_DataGridView_領藥台_02_領藥內容;
        static public SQL_DataGridView _sqL_DataGridView_領藥台_03_領藥內容;
        static public SQL_DataGridView _sqL_DataGridView_領藥台_04_領藥內容;
        static public Panel _panel_領藥台_01_藥品資訊;
        static public Panel _panel_領藥台_02_藥品資訊;

        static public string[] LoginUsers
        {
            get
            {
                string[] loginUsers = new string[4];

                loginUsers[0] = _uC_調劑作業_TypeA_1.登入者姓名;
                loginUsers[1] = _uC_調劑作業_TypeA_2.登入者姓名;
                loginUsers[2] = _uC_調劑作業_TypeA_3.登入者姓名;
                loginUsers[3] = _uC_調劑作業_TypeA_4.登入者姓名;

                return loginUsers;
            }
        }
        static public string QR_Code_醫令模式切換 = "%%001";
        private MyTimer myTimer_領藥台_01_Logout = new MyTimer(5000);
        private MyTimer myTimer_領藥台_02_Logout = new MyTimer(5000);
        private MyTimer myTimer_領藥台_03_Logout = new MyTimer(5000);
        private MyTimer myTimer_領藥台_04_Logout = new MyTimer(5000);
        private Basic.MyThread MyThread_領藥台_02;
        private Basic.MyThread MyThread_領藥台_03;
        private Basic.MyThread MyThread_領藥台_04;
        private Basic.MyThread MyThread_領藥_RFID;
        private Basic.MyThread MyThread_領藥_RFID_入出庫資料檢查;
        private Basic.MyThread MyThread_DHT;
        private Basic.MyThread MyThread_調劑作業;

        private MyTimerBasic MyTimerBasic_dht_timeout = new MyTimerBasic();

        static public Voice voice = new Voice();
        private bool flag_Program_領藥台_01_換頁 = false;
        private bool flag_Program_領藥台_02_換頁 = false;
        private bool flag_Program_領藥台_03_換頁 = false;
        private bool flag_Program_領藥台_04_換頁 = false;
        private bool flag_Program_領藥_RFID_換頁 = false;


        private void Program_調劑作業_領藥台_01_Init()
        {

            //if ((myConfigClass.Scanner01_COMPort.StringIsEmpty() == true) && myConfigClass.Scanner02_COMPort.StringIsEmpty()
            //    && (myConfigClass.Scanner03_COMPort.StringIsEmpty() && myConfigClass.Scanner04_COMPort.StringIsEmpty()))
            //{
            //    this.sqL_DataGridView_領藥台_01_領藥內容.Set_ColumnWidth(75, DataGridViewContentAlignment.MiddleRight, enum_取藥堆疊母資料.盤點量);
            //    this.sqL_DataGridView_領藥台_01_領藥內容.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_取藥堆疊母資料.動作);
            //    this.sqL_DataGridView_領藥台_01_領藥內容.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleRight, enum_取藥堆疊母資料.狀態);
            //}
            _uC_調劑作業_TypeA_1 = uC_調劑作業_TypeA_1;
            if (PLC_Device_掃碼顏色固定.Bool)
            {
                uC_調劑作業_TypeA_1.固定顏色 = panel_工程模式_領藥台_01_顏色.BackColor.ToColorString();
            }

            uC_調劑作業_TypeA_1.調劑台名稱 = 領藥台_01名稱;
            uC_調劑作業_TypeA_1.PLC_Device_已登入 = new PLC_Device(PLC_Device_領藥台_01_已登入.GetAdress());
            uC_調劑作業_TypeA_1.PLC_Device_單醫令模式 = PLC_Device_領藥台_01_單醫令模式;
            uC_調劑作業_TypeA_1.Init(0);


        }
        private void Program_調劑作業_領藥台_02_Init()
        {
            _uC_調劑作業_TypeA_2 = uC_調劑作業_TypeA_2;
            if (PLC_Device_掃碼顏色固定.Bool)
            {
                uC_調劑作業_TypeA_2.固定顏色 = panel_工程模式_領藥台_02_顏色.BackColor.ToColorString();
            }

            uC_調劑作業_TypeA_2.調劑台名稱 = 領藥台_02名稱;
            uC_調劑作業_TypeA_2.PLC_Device_已登入 = PLC_Device_領藥台_02_已登入;
            uC_調劑作業_TypeA_2.PLC_Device_單醫令模式 = PLC_Device_領藥台_02_單醫令模式;
            uC_調劑作業_TypeA_2.Init(1);
        }
        private void Program_調劑作業_領藥台_03_Init()
        {
            _uC_調劑作業_TypeA_3 = uC_調劑作業_TypeA_3;
            if (PLC_Device_掃碼顏色固定.Bool)
            {
                uC_調劑作業_TypeA_3.固定顏色 = panel_工程模式_領藥台_03_顏色.BackColor.ToColorString();
            }

            uC_調劑作業_TypeA_3.調劑台名稱 = 領藥台_03名稱;
            uC_調劑作業_TypeA_3.PLC_Device_已登入 = PLC_Device_領藥台_03_已登入;
            uC_調劑作業_TypeA_3.PLC_Device_單醫令模式 = PLC_Device_領藥台_03_單醫令模式;
            uC_調劑作業_TypeA_3.Init(2);
        }
        private void Program_調劑作業_領藥台_04_Init()
        {
            _uC_調劑作業_TypeA_4 = uC_調劑作業_TypeA_4;
            if (PLC_Device_掃碼顏色固定.Bool)
            {
                uC_調劑作業_TypeA_4.固定顏色 = panel_工程模式_領藥台_04_顏色.BackColor.ToColorString();
            }

            uC_調劑作業_TypeA_4.調劑台名稱 = 領藥台_04名稱;
            uC_調劑作業_TypeA_4.PLC_Device_已登入 = PLC_Device_領藥台_04_已登入;
            uC_調劑作業_TypeA_4.PLC_Device_單醫令模式 = PLC_Device_領藥台_04_單醫令模式;
            uC_調劑作業_TypeA_4.Init(3);
        }
        static private void Fuction_領藥台_時間重置()
        {
            _uC_調劑作業_TypeA_1.Fuction_時間重置();
            _uC_調劑作業_TypeA_2.Fuction_時間重置();
            _uC_調劑作業_TypeA_3.Fuction_時間重置();
            _uC_調劑作業_TypeA_4.Fuction_時間重置();

        }
        private void SqL_DataGridView_領藥內容_RowPostPaintingEventEx(SQL_DataGridView sQL_DataGridView, DataGridViewRowPostPaintEventArgs e)
        {
            try
            {
                object[] value = sQL_DataGridView.GetRowValues(e.RowIndex);
                if (value != null)
                {

                    Color row_Backcolor = Color.White;
                    Color row_Forecolor = Color.Black;

                    string 狀態 = value[(int)enum_取藥堆疊母資料.狀態].ObjectToString();
                    if (狀態 == enum_取藥堆疊母資料_狀態.等待作業.GetEnumName())
                    {
                        row_Backcolor = Color.Yellow;
                    }
                    else if (狀態 == enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName())
                    {
                        row_Backcolor = Color.Lime;
                    }
                    else if (狀態 == enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName())
                    {
                        row_Backcolor = Color.Red;
                    }
                    else if (狀態 == enum_取藥堆疊母資料_狀態.無儲位.GetEnumName())
                    {
                        row_Backcolor = Color.Pink;
                    }
                    else if (狀態 == enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName())
                    {
                        row_Backcolor = Color.Pink;
                    }
                    else if (狀態 == enum_取藥堆疊母資料_狀態.已領用過.GetEnumName())
                    {
                        row_Backcolor = Color.White;
                    }
                    using (Brush brush = new SolidBrush(row_Backcolor))
                    {
                        //Console.WriteLine($"[Row {e.RowIndex}] 進入繪圖區");

                        int x = e.RowBounds.Left;
                        int y = e.RowBounds.Top;
                        int width = e.RowBounds.Width;
                        int height = e.RowBounds.Height;
                        int image_width = 250;

                        e.Graphics.FillRectangle(brush, e.RowBounds);
                        DrawingClass.Draw.DrawRoundShadow(e.Graphics, new RectangleF(x - 1, y - 1, width, height), Color.DarkGray, 5, 5);

                        Size size = new Size();
                        PointF pointF = new PointF();
                        float temp_x = 0;
                        Font font;

                        string Code = value[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                        //Console.WriteLine($"[Row {e.RowIndex}] 藥品碼：{Code}");

                        int col_width = 0;
                        List<Image> images = Main_Form.Function_取得藥品圖片(Code);
                        if (images != null && images.Count > 0 && images[0] != null)
                        {
                            Image img = images[0];
                            if (img.Width > 0 && img.Height > 0 && image_width > 2 && height > 2)
                            {
                                try
                                {
                                    //Console.WriteLine($"[Row {e.RowIndex}] 圖片尺寸：{img.Width}x{img.Height}");
                                    e.Graphics.DrawImage(img, x + 2, y + 2, image_width - 2, height - 2);
                                }
                                catch (Exception drawEx)
                                {
                                    Console.WriteLine($"[Row {e.RowIndex}] ❗圖片繪製失敗");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[Row {e.RowIndex}] ❗圖片尺寸異常，跳過繪製");
                            }
                        }
                        else
                        {
                            //Console.WriteLine($"[Row {e.RowIndex}] ❗圖片為 null 或 Count = 0");
                        }


                        col_width = sQL_DataGridView.Get_ColumnWidth(enum_取藥堆疊母資料.藥品名稱.GetEnumName());
                        font = sQL_DataGridView.Get_ColumnFont(enum_取藥堆疊母資料.藥品名稱.GetEnumName());
                        string 藥名 = value[(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                        //Console.WriteLine($"[Row {e.RowIndex}] 藥名：{藥名}");
                        size = 藥名.MeasureText(font);
                        pointF = new PointF(10 + image_width, y);
                        DrawingClass.Draw.DrawString(e.Graphics, 藥名, font, new Rectangle((int)pointF.X, (int)pointF.Y, col_width, height), row_Forecolor, DataGridViewContentAlignment.MiddleLeft);
                        temp_x = pointF.X + col_width;

                        pointF = new PointF(temp_x + 10, y);
                        col_width = sQL_DataGridView.Get_ColumnWidth(enum_取藥堆疊母資料.總異動量.GetEnumName());
                        font = sQL_DataGridView.Get_ColumnFont(enum_取藥堆疊母資料.總異動量.GetEnumName());
                        string 總異動量 = value[(int)enum_取藥堆疊母資料.總異動量].ObjectToString();
                        //Console.WriteLine($"[Row {e.RowIndex}] 總異動量：{總異動量}");
                        size = 總異動量.MeasureText(font);
                        DrawingClass.Draw.DrawString(e.Graphics, 總異動量, font, new Rectangle((int)pointF.X, (int)pointF.Y, col_width, height), row_Forecolor, DataGridViewContentAlignment.MiddleLeft);
                        temp_x = pointF.X + col_width;

                        pointF = new PointF(temp_x + 10, y);
                        col_width = sQL_DataGridView.Get_ColumnWidth(enum_取藥堆疊母資料.結存量.GetEnumName());
                        font = sQL_DataGridView.Get_ColumnFont(enum_取藥堆疊母資料.結存量.GetEnumName());
                        string 結存量 = value[(int)enum_取藥堆疊母資料.結存量].ObjectToString();
                        //Console.WriteLine($"[Row {e.RowIndex}] 結存量：{結存量}");
                        size = 結存量.MeasureText(font);
                        DrawingClass.Draw.DrawString(e.Graphics, $"({結存量})", font, new Rectangle((int)pointF.X, (int)pointF.Y, col_width, height), row_Forecolor, DataGridViewContentAlignment.MiddleLeft);
                        temp_x = pointF.X + col_width;

                        font = new Font("標楷體", 14, FontStyle.Bold);
                        size = 狀態.MeasureText(font);
                        pointF = new PointF(e.RowBounds.Right - size.Width, e.RowBounds.Bottom - size.Height - 10);
                        DrawingClass.Draw.文字左上繪製($"{狀態}", pointF, font, row_Forecolor, e.Graphics);

                        if (sQL_DataGridView.dataGridView.Rows[e.RowIndex].Selected)
                        {
                            DrawingClass.Draw.方框繪製(new Point(x, y), new Size(width, height), Color.Blue, 2, false, e.Graphics, 1, 1);
                        }
                        else
                        {
                            DrawingClass.Draw.方框繪製(new Point(x, y), new Size(width, height), Color.Black, 1, false, e.Graphics, 1, 1);
                        }

                        //Console.WriteLine($"[Row {e.RowIndex}] 繪圖結束");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Row {e.RowIndex}] ❗繪圖區發生錯誤：{ex.Message}");
            }
        }
        private void Program_調劑作業_Init()
        {
            _plC_CheckBox_領藥無儲位不顯示 = this.plC_CheckBox_領藥無儲位不顯示;

            _sqL_DataGridView_領藥台_01_領藥內容 = uC_調劑作業_TypeA_1.sqL_DataGridView_領藥內容;
            _sqL_DataGridView_領藥台_02_領藥內容 = uC_調劑作業_TypeA_2.sqL_DataGridView_領藥內容;
            _sqL_DataGridView_領藥台_03_領藥內容 = uC_調劑作業_TypeA_3.sqL_DataGridView_領藥內容;
            _sqL_DataGridView_領藥台_04_領藥內容 = uC_調劑作業_TypeA_4.sqL_DataGridView_領藥內容;

            Program_調劑作業_領藥台_01_Init();
            Program_調劑作業_領藥台_02_Init();
            Program_調劑作業_領藥台_03_Init();
            Program_調劑作業_領藥台_04_Init();


            _panel_領藥台_01_藥品資訊 = uC_調劑作業_TypeA_1.panel_藥品資訊;
            _panel_領藥台_02_藥品資訊 = uC_調劑作業_TypeA_2.panel_藥品資訊;

            Dialog_使用者登入.myTimerBasic_覆核完成.StartTickTime(1);
            this.plC_RJ_Button_指紋登入.MouseDownEvent += PlC_RJ_Button_指紋登入_MouseDownEvent;
            this.plC_RJ_Button_手輸醫令.MouseDownEvent += PlC_RJ_Button_手輸醫令_MouseDownEvent;
            this.plC_RJ_Button_藥單條碼輸入.MouseDownEvent += PlC_RJ_Button_條碼輸入_MouseDownEvent;
            this.plC_RJ_Button_醫令檢索.MouseDownEvent += PlC_RJ_Button_醫令檢索_MouseDownEvent;
            this.plC_RJ_Button_藥品調入.MouseDownEvent += PlC_RJ_Button_藥品調入_MouseDownEvent;
            this.plC_RJ_Button_交班對點.MouseDownEvent += PlC_RJ_Button_交班對點_MouseDownEvent;
            this.plC_RJ_Button_藥品搜索.MouseDownEvent += PlC_RJ_Button_藥品搜索_MouseDownEvent;
            this.plC_RJ_Button_申領.MouseDownEvent += PlC_RJ_Button_申領_MouseDownEvent;
            this.plC_RJ_Button_全部滅燈.MouseDownEvent += PlC_RJ_Button_全部滅燈_MouseDownEvent;

            this.toolStripMenuItem_調劑畫面_顯示設定.Click += ToolStripMenuItem_調劑畫面_顯示設定_Click;


            this.MyThread_DHT = new Basic.MyThread(this.FindForm());
            this.MyThread_DHT.Add_Method(this.sub_Program_DHT);
            this.MyThread_DHT.AutoRun(true);
            this.MyThread_DHT.AutoStop(false);
            this.MyThread_DHT.SetSleepTime(500);
            this.MyThread_DHT.Trigger();

            this.MyThread_調劑作業 = new Basic.MyThread(this.FindForm());
            this.MyThread_調劑作業.Add_Method(this.sub_Program_調劑作業);
            this.MyThread_調劑作業.AutoRun(true);
            this.MyThread_調劑作業.AutoStop(false);
            this.MyThread_調劑作業.SetSleepTime(10);
            this.MyThread_調劑作業.Trigger();
            


            PLC_Device_領藥台_01_已登入.ValueChangeEvent += PLC_Device_領藥台_已登入_ValueChangeEvent;
            PLC_Device_領藥台_02_已登入.ValueChangeEvent += PLC_Device_領藥台_已登入_ValueChangeEvent;
            PLC_Device_領藥台_03_已登入.ValueChangeEvent += PLC_Device_領藥台_已登入_ValueChangeEvent;
            PLC_Device_領藥台_04_已登入.ValueChangeEvent += PLC_Device_領藥台_已登入_ValueChangeEvent;

            this.plC_UI_Init.Add_Method(Program_調劑作業);
        }

        bool flag_調劑作業_頁面更新 = false;
        private void Program_調劑作業()
        {
            if (this.plC_ScreenPage_Main.PageText == "調劑作業")
            {
                if (!this.flag_調劑作業_頁面更新)
                {
                    if (PLC_Device_已登入.Bool) Function_登出();
                    this.flag_調劑作業_頁面更新 = true;
                }
            }
            else
            {

                this.flag_調劑作業_頁面更新 = false;
            }
        }
        private void sub_Program_調劑作業()
        {
           

        }
   
        private void sub_Program_領藥_RFID()
        {
            if (this.plC_ScreenPage_Main.PageText == "調劑作業")
            {

                if (flag_Program_領藥_RFID_換頁)
                {
                    flag_Program_領藥_RFID_換頁 = false;
                }
            }
            else
            {
                flag_Program_領藥_RFID_換頁 = true;
            }
            this.sub_Program_領藥_RFID_檢查刷卡();
        }
        private void sub_Program_DHT()
        {
            StorageUI_EPD_266.UDP_READ uDP_READ_266 = this.storageUI_EPD_266.Get_UDP_READ("192.168.0.50");
            bool flag_ping = Basic.Net.Ping("192.168.48.21", 2, 100);
            if (uDP_READ_266 != null)
            {
                this.Invoke(new Action(delegate 
                {
                    if (this.panel_DHT.Visible == false)
                    {
                        this.panel_DHT.Visible = true;
                    }
                  

                    label_溫度.Text = $"{uDP_READ_266.dht_t.ToString("0.00")}°C";
                    label_濕度.Text = $"{uDP_READ_266.dht_h.ToString("0.00")} %";
                }));
                MyTimerBasic_dht_timeout.TickStop();
                MyTimerBasic_dht_timeout.StartTickTime(3000);
            }
            if (this.panel_DHT.Visible == true)
            {
                this.Invoke(new Action(delegate
                {
                    this.panel_DHT.BackColor = flag_ping ? Color.White : Color.Yellow;
                }));
            }

        }

        PLC_Device PLC_Device_領藥台_01_已登入 = new PLC_Device("S100");
        PLC_Device PLC_Device_領藥台_01_單醫令模式 = new PLC_Device("S110");
        PLC_Device PLC_Device_領藥台_02_已登入 = new PLC_Device("S200");
        PLC_Device PLC_Device_領藥台_02_單醫令模式 = new PLC_Device("S210");
        PLC_Device PLC_Device_領藥台_03_已登入 = new PLC_Device("S300");
        PLC_Device PLC_Device_領藥台_03_單醫令模式 = new PLC_Device("S310");
        PLC_Device PLC_Device_領藥台_04_已登入 = new PLC_Device("S400");
        PLC_Device PLC_Device_領藥台_04_單醫令模式 = new PLC_Device("S410");

        private void PLC_Device_領藥台_已登入_ValueChangeEvent(object Value)
        {
            if ((bool)Value == true)
            {
                this.Invoke(new Action(delegate
                {
                    this.panel_Main.Collapse();
                }));
            }
        }

        #region PLC_領藥_RFID_檢查刷卡
        PLC_Device PLC_Device_領藥_RFID_檢查刷卡 = new PLC_Device("");
        PLC_Device PLC_Device_領藥_RFID_檢查刷卡_OK = new PLC_Device("");
        PLC_Device PLC_Device_領藥_RFID_檢查刷卡_TEST = new PLC_Device("");
        string 領藥_RFID_檢查刷卡_登入者ID;
        string 領藥_RFID_檢查刷卡_登入者姓名;
        Class_領藥_RFID_檢查刷卡 class_領藥_RFID_檢查刷卡 = new Class_領藥_RFID_檢查刷卡();
        private class Class_領藥_RFID_檢查刷卡
        {
            public string IP;
            public int Num;
            public string Name;
            public string RFID;
            public List<Device> devices = new List<Device>();
        }
        int cnt_Program_領藥_RFID_檢查刷卡 = 65534;
        void sub_Program_領藥_RFID_檢查刷卡()
        {
            if (this.plC_ScreenPage_Main.PageText != "管制抽屜" && this.plC_ScreenPage_Main.PageText != "調劑作業") PLC_Device_領藥_RFID_檢查刷卡.Bool = false;
            else PLC_Device_領藥_RFID_檢查刷卡.Bool = true;
            if (cnt_Program_領藥_RFID_檢查刷卡 == 65534)
            {
                PLC_Device_領藥_RFID_檢查刷卡.SetComment("PLC_領藥_RFID_檢查刷卡");
                PLC_Device_領藥_RFID_檢查刷卡_OK.SetComment("PLC_領藥_RFID_檢查刷卡_OK");
                PLC_Device_領藥_RFID_檢查刷卡.Bool = false;
                cnt_Program_領藥_RFID_檢查刷卡 = 65535;
            }
            if (cnt_Program_領藥_RFID_檢查刷卡 == 65535) cnt_Program_領藥_RFID_檢查刷卡 = 1;
            if (cnt_Program_領藥_RFID_檢查刷卡 == 1) cnt_Program_領藥_RFID_檢查刷卡_檢查按下(ref cnt_Program_領藥_RFID_檢查刷卡);
            if (cnt_Program_領藥_RFID_檢查刷卡 == 2) cnt_Program_領藥_RFID_檢查刷卡_初始化(ref cnt_Program_領藥_RFID_檢查刷卡);
            if (cnt_Program_領藥_RFID_檢查刷卡 == 3) cnt_Program_領藥_RFID_檢查刷卡_取得刷卡ID(ref cnt_Program_領藥_RFID_檢查刷卡);
            if (cnt_Program_領藥_RFID_檢查刷卡 == 4) cnt_Program_領藥_RFID_檢查刷卡_取得登入者資訊(ref cnt_Program_領藥_RFID_檢查刷卡);
            if (cnt_Program_領藥_RFID_檢查刷卡 == 5) cnt_Program_領藥_RFID_檢查刷卡_顯示RFID領退藥頁面(ref cnt_Program_領藥_RFID_檢查刷卡);
            if (cnt_Program_領藥_RFID_檢查刷卡 == 6) cnt_Program_領藥_RFID_檢查刷卡 = 65500;
            if (cnt_Program_領藥_RFID_檢查刷卡 > 1) cnt_Program_領藥_RFID_檢查刷卡_檢查放開(ref cnt_Program_領藥_RFID_檢查刷卡);

            if (cnt_Program_領藥_RFID_檢查刷卡 == 65500)
            {
                PLC_Device_領藥_RFID_檢查刷卡.Bool = false;
                PLC_Device_領藥_RFID_檢查刷卡_OK.Bool = false;
                cnt_Program_領藥_RFID_檢查刷卡 = 65535;
            }
        }
        void cnt_Program_領藥_RFID_檢查刷卡_檢查按下(ref int cnt)
        {
            if (PLC_Device_領藥_RFID_檢查刷卡.Bool) cnt++;
        }
        void cnt_Program_領藥_RFID_檢查刷卡_檢查放開(ref int cnt)
        {
            if (!PLC_Device_領藥_RFID_檢查刷卡.Bool) cnt = 65500;
        }
        void cnt_Program_領藥_RFID_檢查刷卡_初始化(ref int cnt)
        {

            cnt++;
        }
        void cnt_Program_領藥_RFID_檢查刷卡_取得刷卡ID(ref int cnt)
        {
            for (int i = 0; i < List_RFID_本地資料.Count; i++)
            {
                for (int k = 0; k < List_RFID_本地資料[i].DeviceClasses.Length; k++)
                {
                    if (List_RFID_本地資料[i].DeviceClasses[k].Enable && !List_RFID_本地資料[i].DeviceClasses[k].IsLocker)
                    {
                        string RFID = this.rfiD_UI.GetRFID(List_RFID_本地資料[i].IP, k);
                        if (RFID.StringToInt32() != 0 && !RFID.StringIsEmpty() || PLC_Device_領藥_RFID_檢查刷卡_TEST.Bool)
                        {
                            PLC_Device_領藥_RFID_檢查刷卡_TEST.Bool = false;
                            this.class_領藥_RFID_檢查刷卡.devices.Clear();
                            for (int d = 0; d < List_RFID_本地資料[i].DeviceClasses[k].RFIDDevices.Count; d++)
                            {
                                this.class_領藥_RFID_檢查刷卡.devices.Add(List_RFID_本地資料[i].DeviceClasses[k].RFIDDevices[d]);
                            }
                            this.class_領藥_RFID_檢查刷卡.IP = List_RFID_本地資料[i].IP;
                            this.class_領藥_RFID_檢查刷卡.Num = k;
                            this.class_領藥_RFID_檢查刷卡.RFID = RFID;
                            this.class_領藥_RFID_檢查刷卡.Name = List_RFID_本地資料[i].DeviceClasses[k].Name;


                            cnt++;
                            return;
                        }

                    }
                }
            }
            cnt = 65500;
        }
        void cnt_Program_領藥_RFID_檢查刷卡_取得登入者資訊(ref int cnt)
        {
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_人員資料.卡號, class_領藥_RFID_檢查刷卡.RFID);
            if (list_value.Count == 0)
            {
                cnt = 65500;
                return;
            }
            領藥_RFID_檢查刷卡_登入者ID = list_value[0][(int)enum_人員資料.ID].ObjectToString();
            領藥_RFID_檢查刷卡_登入者姓名 = list_value[0][(int)enum_人員資料.姓名].ObjectToString();
            cnt++;
        }
        void cnt_Program_領藥_RFID_檢查刷卡_顯示RFID領退藥頁面(ref int cnt)
        {
            Function_取藥堆疊資料_刪除指定調劑台名稱母資料(領藥台_02名稱);
            for (int i = 0; i < this.class_領藥_RFID_檢查刷卡.devices.Count; i++)
            {
                this.class_領藥_RFID_檢查刷卡.devices[i] = this.rfiD_UI.SQL_GetDevice((RFIDDevice)this.class_領藥_RFID_檢查刷卡.devices[i]);
            }

            this.Invoke(new Action(delegate
            {
                Dialog_RFID領退藥頁面 dialog_RFID領退藥頁面 = new Dialog_RFID領退藥頁面(領藥_RFID_檢查刷卡_登入者ID, 領藥_RFID_檢查刷卡_登入者姓名, this.class_領藥_RFID_檢查刷卡.Name, this.class_領藥_RFID_檢查刷卡.devices, 領藥台_02名稱, List_領藥_入出庫資料檢查);
                if (dialog_RFID領退藥頁面.ShowDialog() == DialogResult.Yes)
                {
                    Function_取藥堆疊資料_刪除指定調劑台名稱母資料(領藥台_02名稱);
                }
            }));

            cnt++;
        }

        #endregion
        #region PLC_領藥_入出庫資料檢查
        PLC_Device PLC_Device_領藥_入出庫資料檢查 = new PLC_Device("");
        PLC_Device PLC_Device_領藥_入出庫資料檢查_OK = new PLC_Device("");
        private List<object[]> List_領藥_入出庫資料檢查 = new List<object[]>();

        int cnt_Program_領藥_入出庫資料檢查 = 65534;
        void sub_Program_領藥_入出庫資料檢查()
        {
            if (this.plC_ScreenPage_Main.PageText != "調劑作業") PLC_Device_領藥_入出庫資料檢查.Bool = false;
            else PLC_Device_領藥_入出庫資料檢查.Bool = true;

            if (cnt_Program_領藥_入出庫資料檢查 == 65534)
            {
                PLC_Device_領藥_入出庫資料檢查.SetComment("PLC_領藥_入出庫資料檢查");
                PLC_Device_領藥_入出庫資料檢查_OK.SetComment("PLC_領藥_入出庫資料檢查_OK");
                PLC_Device_領藥_入出庫資料檢查.Bool = false;
                cnt_Program_領藥_入出庫資料檢查 = 65535;
            }
            if (cnt_Program_領藥_入出庫資料檢查 == 65535) cnt_Program_領藥_入出庫資料檢查 = 1;
            if (cnt_Program_領藥_入出庫資料檢查 == 1) cnt_Program_領藥_入出庫資料檢查_檢查按下(ref cnt_Program_領藥_入出庫資料檢查);
            if (cnt_Program_領藥_入出庫資料檢查 == 2) cnt_Program_領藥_入出庫資料檢查_初始化(ref cnt_Program_領藥_入出庫資料檢查);
            if (cnt_Program_領藥_入出庫資料檢查 == 3) cnt_Program_領藥_入出庫資料檢查 = 65500;
            if (cnt_Program_領藥_入出庫資料檢查 > 1) cnt_Program_領藥_入出庫資料檢查_檢查放開(ref cnt_Program_領藥_入出庫資料檢查);

            if (cnt_Program_領藥_入出庫資料檢查 == 65500)
            {
                PLC_Device_領藥_入出庫資料檢查.Bool = false;
                PLC_Device_領藥_入出庫資料檢查_OK.Bool = false;
                cnt_Program_領藥_入出庫資料檢查 = 65535;
            }
        }
        void cnt_Program_領藥_入出庫資料檢查_檢查按下(ref int cnt)
        {
            if (PLC_Device_領藥_入出庫資料檢查.Bool) cnt++;
        }
        void cnt_Program_領藥_入出庫資料檢查_檢查放開(ref int cnt)
        {
            if (!PLC_Device_領藥_入出庫資料檢查.Bool) cnt = 65500;
        }
        void cnt_Program_領藥_入出庫資料檢查_初始化(ref int cnt)
        {
            string GUID = "";
            string 調劑台名稱 = "";
            enum_交易記錄查詢動作 動作 = enum_交易記錄查詢動作.None;
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 藥袋序號 = "";
            string 單位 = "";
            string 病歷號 = "";
            string 病人姓名 = "";
            string 開方時間 = "";
            string ID = "";
            string 操作人 = "";
            string 顏色 = "";
            double 總異動量 = 0;
            string 效期 = "";
            string 床號 = "";
            List<object[]> list_藥品設定表 = this.sqL_DataGridView_藥品設定表.SQL_GetAllRows(false);
            List<object[]> list_藥品資料 = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            for (int i = 0; i < List_領藥_入出庫資料檢查.Count; i++)
            {
                GUID = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.GUID].ObjectToString();
                調劑台名稱 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.調劑台名稱].ObjectToString();
                藥品碼 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.藥品碼].ObjectToString();
                藥品名稱 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.藥品名稱].ObjectToString();
                藥袋序號 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.藥袋序號].ObjectToString();
                單位 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.單位].ObjectToString();
                病歷號 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.病歷號].ObjectToString();
                病人姓名 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.病人姓名].ObjectToString();
                開方時間 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.開方時間].ObjectToString();
                ID = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.ID].ObjectToString();
                操作人 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.操作人].ObjectToString();
                顏色 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.顏色].ObjectToString();
                總異動量 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.總異動量].ObjectToString().StringToDouble();
                效期 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.效期].ObjectToString();
                string str_動作 = List_領藥_入出庫資料檢查[i][(int)Dialog_RFID領退藥頁面.enum_入出庫資料檢查.動作].ObjectToString();
                if (str_動作 == enum_交易記錄查詢動作.手輸領藥.GetEnumName())
                {
                    動作 = enum_交易記錄查詢動作.手輸領藥;
                }
                if (str_動作 == enum_交易記錄查詢動作.入庫作業.GetEnumName())
                {
                    動作 = enum_交易記錄查詢動作.入庫作業;
                }

                takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
                takeMedicineStackClass.GUID = GUID;
                takeMedicineStackClass.調劑台名稱 = 調劑台名稱;
                takeMedicineStackClass.動作 = 動作.GetEnumName();
                takeMedicineStackClass.顏色 = 顏色;
                takeMedicineStackClass.藥品碼 = 藥品碼;
                takeMedicineStackClass.藥品名稱 = 藥品名稱;
                takeMedicineStackClass.藥袋序號 = 藥袋序號;
                takeMedicineStackClass.單位 = 單位;
                takeMedicineStackClass.病歷號 = 病歷號;
                takeMedicineStackClass.床號 = 床號;
                takeMedicineStackClass.病人姓名 = 病人姓名;
                takeMedicineStackClass.開方時間 = 開方時間;
                takeMedicineStackClass.操作人 = 操作人;
                takeMedicineStackClass.顏色 = 顏色;
                takeMedicineStackClass.總異動量 = 總異動量.ToString();
                takeMedicineStackClass.效期 = 效期;
                takeMedicineStackClass.ID = ID;

                Function_取藥堆疊資料_新增母資料(takeMedicineStackClass);
                List_領藥_入出庫資料檢查.RemoveAt(i);
                voice.SpeakOnTask("成功");
                break;
            }
            cnt++;
        }




        #endregion

        private void PlC_RJ_Button_指紋登入_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_AlarmForm dialog_AlarmForm;
            if (fpMatchSoket.IsOpen == false && flag_指紋辨識_Init == false)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("指紋模組未初始化", 2000);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            Dialog_指紋登入 dialog_指紋登入 = new Dialog_指紋登入();
            if (dialog_指紋登入.ShowDialog() != DialogResult.Yes) return;
            if (dialog_指紋登入.台號 == 1) uC_調劑作業_TypeA_1.FpMatchClass_指紋資訊 = dialog_指紋登入.Value;
            if (dialog_指紋登入.台號 == 2) uC_調劑作業_TypeA_2.FpMatchClass_指紋資訊 = dialog_指紋登入.Value;
            if (dialog_指紋登入.台號 == 3) uC_調劑作業_TypeA_3.FpMatchClass_指紋資訊 = dialog_指紋登入.Value;
            if (dialog_指紋登入.台號 == 4) uC_調劑作業_TypeA_4.FpMatchClass_指紋資訊 = dialog_指紋登入.Value;

        }
        private void PlC_RJ_Button_醫令檢索_MouseDownEvent(MouseEventArgs mevent)
        {
            MyTimer myTimer = new MyTimer();
            List<OrderClass> orderClasses = new List<OrderClass>();
            Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new enum_ContextMenuStrip_Main_醫令檢索());

            dialog_ContextMenuStrip.TitleFont = new Font("微軟正黑體", 24, FontStyle.Bold);
            dialog_ContextMenuStrip.ControlsFont = new Font("微軟正黑體", 22);
            dialog_ContextMenuStrip.ControlsWidth = 400;
            dialog_ContextMenuStrip.ControlsHeight = 80;

            dialog_ContextMenuStrip.SetEnable(enum_ContextMenuStrip_Main_醫令檢索.病歷號, !dBConfigClass.Order_mrn_ApiURL.StringIsEmpty());
            dialog_ContextMenuStrip.SetEnable(enum_ContextMenuStrip_Main_醫令檢索.領藥號, !dBConfigClass.Order_bag_num_ApiURL.StringIsEmpty());
            dialog_ContextMenuStrip.TitleText = "醫令檢索";
            dialog_ContextMenuStrip.TopMost = true;
            if (dialog_ContextMenuStrip.ShowDialog() != DialogResult.Yes) return;
            if (dialog_ContextMenuStrip.Value == enum_ContextMenuStrip_Main_醫令檢索.病歷號.GetEnumName())
            {
                Dialog_病歷號輸入 dialog_病歷號輸入;

                this.Invoke(new Action(delegate
                {
                    dialog_病歷號輸入 = new Dialog_病歷號輸入();
                    if (dialog_病歷號輸入.ShowDialog() != DialogResult.Yes) return;
                    orderClasses = dialog_病歷號輸入.Value;
                }));

            }
            if (dialog_ContextMenuStrip.Value == enum_ContextMenuStrip_Main_醫令檢索.領藥號.GetEnumName())
            {
                Dialog_領藥號輸入 dialog_領藥號輸入;
                this.Invoke(new Action(delegate
                {
                    dialog_領藥號輸入 = new Dialog_領藥號輸入();
                    if (dialog_領藥號輸入.ShowDialog() != DialogResult.Yes) return;
                    orderClasses = dialog_領藥號輸入.Value;
                }));

            }
            if (dialog_ContextMenuStrip.Value == enum_ContextMenuStrip_Main_醫令檢索.長期用藥.GetEnumName())
            {
                this.Invoke(new Action(delegate
                {
                    personPageClass personPageClass = new personPageClass();
                    personPageClass.ID = uC_調劑作業_TypeA_1.ID;
                    personPageClass.姓名 = uC_調劑作業_TypeA_1.登入者姓名;
                    personPageClass.藥師證字號 = uC_調劑作業_TypeA_1.藥師證字號;
                    personPageClass.顏色 = uC_調劑作業_TypeA_1.顏色;

                    Dialog_長期醫令 dialog_長期醫令;
                    dialog_長期醫令 = new Dialog_長期醫令(personPageClass, 領藥台_01名稱);
                    if (dialog_長期醫令.ShowDialog() != DialogResult.Yes) return;
                }));
             
                return;
            }


            List<object[]> list_藥品資料 = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_醫令資料 = orderClasses.ClassToSQL<OrderClass, enum_醫囑資料>();
            bool flag_雙人覆核 = false;
            List<object[]> list_藥品設定表 = this.sqL_DataGridView_藥品設定表.SQL_GetAllRows(false);
            List<object[]> list_藥品管制方式設定 = this.sqL_DataGridView_藥品管制方式設定.SQL_GetAllRows(false);
            List<object[]> list_藥品資料_buf = new List<object[]>();
            Console.Write($"取得藥品資料 , 耗時{myTimer.ToString()}\n");
            Function_取藥堆疊資料_刪除指定調劑台名稱母資料(領藥台_01名稱);
            Console.Write($"刪除調劑台資料資料 , 耗時{myTimer.ToString()}\n");
            List<takeMedicineStackClass> takeMedicineStackClasses = new List<takeMedicineStackClass>();
            PLC_Device pLC_Device = new PLC_Device(plC_CheckBox_領藥不檢查是否掃碼領藥過.讀取元件位置);

            List<string> Codes = (from temp in list_醫令資料
                                  select temp[(int)enum_醫囑資料.藥品碼].ObjectToString()).Distinct().ToList();
            List<medClass> medClasses = medClass.get_med_clouds_by_codes(API_Server, Codes);
            List<medClass> medClasses_buf = new List<medClass>();
            Dictionary<string, List<medClass>> keyValuePairs_medcloud = medClasses.CoverToDictionaryByCode();

            for (int i = 0; i < list_醫令資料.Count; i++)
            {


                string GUID = list_醫令資料[i][(int)enum_醫囑資料.GUID].ObjectToString();
                string 調劑台名稱 = 領藥台_01名稱;
                enum_交易記錄查詢動作 動作 = enum_交易記錄查詢動作.掃碼領藥;
                string 藥品碼 = list_醫令資料[i][(int)enum_醫囑資料.藥品碼].ObjectToString();
                string 診別 = list_醫令資料[i][(int)enum_醫囑資料.藥局代碼].ObjectToString();

                list_藥品資料_buf = list_藥品資料.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, 藥品碼);
                if (list_藥品資料_buf.Count == 0) continue;
                string 藥品名稱 = list_藥品資料_buf[0][(int)enum_藥品資料_藥檔資料.藥品名稱].ObjectToString();
                string 藥袋序號 = list_醫令資料[i][(int)enum_醫囑資料.PRI_KEY].ObjectToString();
                string 單位 = list_藥品資料_buf[0][(int)enum_藥品資料_藥檔資料.包裝單位].ObjectToString();
                string 病歷號 = list_醫令資料[i][(int)enum_醫囑資料.病歷號].ObjectToString();
                string 病人姓名 = list_醫令資料[i][(int)enum_醫囑資料.病人姓名].ObjectToString();
                string 床號 = "";
                string 開方時間 = list_醫令資料[i][(int)enum_醫囑資料.開方日期].ToDateTimeString_6();
                string ID = uC_調劑作業_TypeA_1.ID;
                string 操作人 = uC_調劑作業_TypeA_1.登入者姓名;
                string 藥師證字號 = uC_調劑作業_TypeA_1.藥師證字號;
                string 顏色 = uC_調劑作業_TypeA_1.顏色;
                double 總異動量 = list_醫令資料[i][(int)enum_醫囑資料.交易量].ObjectToString().StringToDouble();
                string 效期 = "";
                string 收支原因 = "";


                takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
                takeMedicineStackClass.GUID = GUID;
                takeMedicineStackClass.動作 = 動作.GetEnumName();
                takeMedicineStackClass.調劑台名稱 = 調劑台名稱;
                takeMedicineStackClass.藥品碼 = 藥品碼;
                takeMedicineStackClass.診別 = 診別;
                takeMedicineStackClass.顏色 = 顏色;
                takeMedicineStackClass.藥品名稱 = 藥品名稱;
                takeMedicineStackClass.藥袋序號 = 藥袋序號;
                takeMedicineStackClass.病歷號 = 病歷號;
                takeMedicineStackClass.病人姓名 = 病人姓名;
                takeMedicineStackClass.床號 = 床號;
                takeMedicineStackClass.開方時間 = 開方時間;
                takeMedicineStackClass.操作人 = 操作人;
                takeMedicineStackClass.藥師證字號 = 藥師證字號;
                takeMedicineStackClass.總異動量 = 總異動量.ToString();
                takeMedicineStackClass.效期 = 效期;
                takeMedicineStackClass.收支原因 = 收支原因;
                takeMedicineStackClass.ID = ID;

                bool flag_檢查過帳 = false;
                medClasses_buf = keyValuePairs_medcloud.SortDictionaryByCode(藥品碼);
                if (medClasses_buf.Count > 0)
                {
                    if (medClasses_buf[0].高價藥品.ToUpper() == true.ToString().ToUpper())
                    {
                        flag_檢查過帳 = true;
                    }
                    if (medClasses_buf[0].管制級別.StringIsEmpty() == false && medClasses_buf[0].管制級別 != "N")
                    {
                        flag_檢查過帳 = true;
                    }
                }

                if (pLC_Device.Bool == false || flag_檢查過帳 == true)
                {
                    if (list_醫令資料[i][(int)enum_醫囑資料.狀態].ObjectToString() == enum_醫囑資料_狀態.已過帳.GetEnumName())
                    {
                        takeMedicineStackClass.狀態 = enum_取藥堆疊母資料_狀態.已領用過.GetEnumName();
                    }
                }
                if (flag_雙人覆核)
                {
                    Function_取藥堆疊資料_新增母資料(takeMedicineStackClass);
                    continue;
                }

                takeMedicineStackClasses.Add(takeMedicineStackClass);

            }
            Function_取藥堆疊資料_新增母資料(takeMedicineStackClasses);
        }
        private void PlC_RJ_Button_條碼輸入_MouseDownEvent(MouseEventArgs mevent)
        {
            if(Function_檢查是否完成交班() == false)
            {
                MyMessageBox.ShowDialog("請先完成交班");
                return;
            }
            Dialog_條碼輸入 dialog_條碼輸入 = new Dialog_條碼輸入();
            if (dialog_條碼輸入.ShowDialog() != DialogResult.Yes) return;
            string Barcode = dialog_條碼輸入.Value;
            if (uC_調劑作業_TypeA_1.plC_Button_領.Bool)
            {
                uC_調劑作業_TypeA_1.Function_醫令領藥(Barcode);
            }
            else if (uC_調劑作業_TypeA_1.plC_Button_退.Bool)
            {
                uC_調劑作業_TypeA_1.Function_醫令退藥(Barcode);
            }
        }
        private void PlC_RJ_Button_手輸醫令_MouseDownEvent(MouseEventArgs mevent)
        {
            if (Function_檢查是否完成交班() == false)
            {
                MyMessageBox.ShowDialog("請先完成交班");
                return;
            }
            this.Invoke(new Action(delegate
            {
                Dialog_手輸醫令.enum_狀態 enum_狀態 = Dialog_手輸醫令.enum_狀態.領藥;
                if (uC_調劑作業_TypeA_1.plC_Button_領.Bool)
                {
                    enum_狀態 = Dialog_手輸醫令.enum_狀態.領藥;
                }
                if (uC_調劑作業_TypeA_1.plC_Button_退.Bool)
                {
                    enum_狀態 = Dialog_手輸醫令.enum_狀態.退藥;
                }
                Dialog_手輸醫令 dialog_手輸醫令 = new Dialog_手輸醫令((Main_Form)this.FindForm(), this.sqL_DataGridView_藥品資料_藥檔資料, enum_狀態);
                dialog_手輸醫令.ShowDialog();
                List<object[]> list_value = dialog_手輸醫令.Value;
                if (list_value.Count == 0) return;

                List<string> Codes = (from temp in list_value
                                      select temp[(int)enum_選擇藥品.藥品碼].ObjectToString()).Distinct().ToList();
                List<medClass> medClasses = medClass.get_med_clouds_by_codes(API_Server, Codes);
                List<medClass> medClasses_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medcloud = medClasses.CoverToDictionaryByCode();

                sessionClass _sessionClass = sessionClass.LoginByID(Main_Form.API_Server, uC_調劑作業_TypeA_1.ID, uC_調劑作業_TypeA_1.密碼);


                bool flag_雙人覆核 = false;
                List<object[]> list_藥品設定表 = this.sqL_DataGridView_藥品設定表.SQL_GetAllRows(false);
                List<object[]> list_藥品管制方式設定 = this.sqL_DataGridView_藥品管制方式設定.SQL_GetAllRows(false);
                List<takeMedicineStackClass> takeMedicineStackClasses = new List<takeMedicineStackClass>();
                for (int i = 0; i < list_value.Count; i++)
                {
                    string GUID = Guid.NewGuid().ToString();
                    string 調劑台名稱 = 領藥台_01名稱;

                    string 藥品碼 = list_value[i][(int)enum_選擇藥品.藥品碼].ObjectToString();
                    string 床號 = dialog_手輸醫令.transactionsClass.病房號;
                    medClasses_buf = keyValuePairs_medcloud.SortDictionaryByCode(藥品碼);
                    if (medClasses_buf.Count == 0) continue;
                    string 藥品名稱 = medClasses_buf[0].藥品名稱;
                    string 藥袋序號 = "";
                    string 單位 = medClasses_buf[0].包裝單位;
                    string 病歷號 = dialog_手輸醫令.transactionsClass.病歷號;
                    string 領藥號 = dialog_手輸醫令.transactionsClass.領藥號;
                    string 病房號 = dialog_手輸醫令.transactionsClass.病房號;
                    string 病人姓名 = dialog_手輸醫令.transactionsClass.病人姓名;
                    string 開方時間 = DateTime.Now.ToDateTimeString_6();
                    string ID = uC_調劑作業_TypeA_1.ID;
                    string 操作人 = uC_調劑作業_TypeA_1.登入者姓名;
                    string 藥師證字號 = uC_調劑作業_TypeA_1.藥師證字號;
                    string 顏色 = uC_調劑作業_TypeA_1.顏色;
                    string 收支原因 = $"手動{(enum_狀態 == Dialog_手輸醫令.enum_狀態.領藥? "領":"退")}藥";
                    if (dialog_手輸醫令.transactionsClass.收支原因.StringIsEmpty() == false)
                    {
                        收支原因 = $"{收支原因}[{dialog_手輸醫令.transactionsClass.收支原因}]";
                    }
                    double 總異動量 = list_value[i][(int)enum_選擇藥品.交易量].ObjectToString().StringToDouble();
                    enum_交易記錄查詢動作 動作 = enum_交易記錄查詢動作.掃碼領藥;
                    if (總異動量 <= 0)
                    {
                        動作 = enum_交易記錄查詢動作.手輸領藥;
                    }
                    else
                    {
                        動作 = enum_交易記錄查詢動作.手輸退藥;
                    }


                    string 效期 = "";
                    takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
                    takeMedicineStackClass.GUID = GUID;
                    takeMedicineStackClass.調劑台名稱 = 調劑台名稱;
                    takeMedicineStackClass.動作 = 動作.GetEnumName();
                    takeMedicineStackClass.顏色 = 顏色;
                    takeMedicineStackClass.藥品碼 = 藥品碼;
                    takeMedicineStackClass.藥品名稱 = 藥品名稱;
                    takeMedicineStackClass.藥袋序號 = 藥袋序號;
                    takeMedicineStackClass.單位 = 單位;
                    takeMedicineStackClass.病歷號 = 病歷號;
                    takeMedicineStackClass.床號 = 床號;
                    takeMedicineStackClass.領藥號 = 領藥號;
                    takeMedicineStackClass.病房號 = 病房號;
                    takeMedicineStackClass.病人姓名 = 病人姓名;
                    takeMedicineStackClass.開方時間 = 開方時間;
                    takeMedicineStackClass.操作人 = 操作人;
                    takeMedicineStackClass.藥師證字號 = 藥師證字號;
                    takeMedicineStackClass.顏色 = 顏色;
                    takeMedicineStackClass.總異動量 = 總異動量.ToString();
                    takeMedicineStackClass.收支原因 = 收支原因;
                    takeMedicineStackClass.效期 = 效期;
                    takeMedicineStackClass.ID = ID;

                    PermissionsClass permissionsClass = _sessionClass.GetPermission("調劑台", "禁止調劑1-3級管制藥品");
                    if (permissionsClass != null)
                    {
                        if (_sessionClass.GetPermission("調劑台", "禁止調劑1-3級管制藥品").狀態)
                        {
                            if (medClasses_buf[0].管制級別 == "1" || medClasses_buf[0].管制級別 == "2" || medClasses_buf[0].管制級別 == "3")
                            {
                                takeMedicineStackClass.狀態 = enum_取藥堆疊母資料_狀態.未授權.GetEnumName();
                            }
                        }
                    }

                    if (flag_雙人覆核)
                    {
                        Function_取藥堆疊資料_新增母資料(takeMedicineStackClass);
                        continue;
                    }
                    takeMedicineStackClasses.Add(takeMedicineStackClass);

                }
                List<Task> taskList = new List<Task>();
                taskList.Clear();

                taskList.Add(Task.Run(new Action(delegate
                {
                    Function_取藥堆疊資料_新增母資料(takeMedicineStackClasses);
                })));
                Task.WhenAll(taskList).Wait();
       
            }));
        }
        private void PlC_RJ_Button_藥品調入_MouseDownEvent(MouseEventArgs mevent)
        {
            if (Function_檢查是否完成交班() == false)
            {
                MyMessageBox.ShowDialog("請先完成交班");
                return;
            }
            Dialog_藥品調入 dialog_藥品調入 = new Dialog_藥品調入();
            dialog_藥品調入.ShowDialog();
        }
        private void PlC_RJ_Button_全部滅燈_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("是否全部滅燈?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            LoadingForm.ShowLoadingForm();
            try
            {
                List<Drawer> drawers_epd583 = new List<Drawer>();
                List<Storage> storages_epd266 = new List<Storage>();
                List<RowsLED> rowsLEDs = new List<RowsLED>();

                List<CommonSapceClass> commonSapceClasses = Function_取得共用區所有儲位();
                for (int i = 0; i < commonSapceClasses.Count; i++)
                {
                    drawers_epd583.LockAdd(commonSapceClasses[i].List_EPD583);
                    storages_epd266.LockAdd(commonSapceClasses[i].List_EPD266);
                    rowsLEDs.LockAdd(commonSapceClasses[i].List_RowsLED);
                }
                drawers_epd583.LockAdd(List_EPD583_本地資料);
                storages_epd266.LockAdd(List_EPD266_本地資料);
                rowsLEDs.LockAdd(List_RowsLED_本地資料);

                List<Task> tasks = new List<Task>();

                for (int i = 0; i < drawers_epd583.Count; i++)
                {
                    Drawer drawer = drawers_epd583[i];
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        drawerUI_EPD_583.Set_LED_Clear_UDP(drawer);
                    })));
                }


                for (int i = 0; i < storages_epd266.Count; i++)
                {
                    Storage storage = storages_epd266[i];
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        storageUI_EPD_266.Set_Stroage_LED_UDP(storage, Color.Black);
                    })));
                }

                for (int i = 0; i < rowsLEDs.Count; i++)
                {
                    RowsLED rowsLED = rowsLEDs[i];
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        rowsLEDUI.Set_Rows_LED_Clear_UDP(rowsLED);
                    })));
                }

                List<object[]> list_value = sqL_DataGridView_LCD114_索引表.SQL_GetAllRows(false);
                for (int i = 0; i < list_value.Count; i++)
                {
                    string IP = list_value[i][(int)enum_LCD114_索引表.index_IP].ObjectToString();
                    try
                    {
                        tasks.Add(Task.Run(new Action(delegate
                        {
                            storageUI_LCD_114.ClearCanvas(IP, 29008);
                        })));

                    }
                    catch
                    {

                    }


                }
                Task.WhenAll(tasks).Wait();
            }
            catch
            {
            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }


        }
        private void PlC_RJ_Button_交班對點_MouseDownEvent(MouseEventArgs mevent)
        {
            StorageAlarm = false;
            try
            {
                Dialog_交班對點 dialog_交班對點 = new Dialog_交班對點();
                dialog_交班對點.flag_單人交班 = plC_CheckBox_單人交班.Checked;
                dialog_交班對點.ShowDialog();
            }
            catch
            {

            }
            finally
            {
                StorageAlarm = true;
            }

        }
        private void PlC_RJ_Button_藥品搜索_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_藥品搜索 dialog_藥品搜索 = new Dialog_藥品搜索();
            dialog_藥品搜索.ShowDialog();
        }
        private void PlC_RJ_Button_申領_MouseDownEvent(MouseEventArgs mevent)
        {
            if (Function_檢查是否完成交班() == false)
            {
                MyMessageBox.ShowDialog("請先完成交班");
                return;
            }
            Dialog_申領 dialog_申領 = new Dialog_申領();
            dialog_申領.ShowDialog();
        }

        private void ToolStripMenuItem_調劑畫面_顯示設定_Click(object sender, EventArgs e)
        {
            Control control = contextMenuStrip_調劑畫面.SourceControl;

            if (control.Name == "sqL_DataGridView_領藥台_01_領藥內容")
            {
                Dialog_調劑畫面顯示調整 dialog_調劑畫面顯示調整 = new Dialog_調劑畫面顯示調整(0);
                if (dialog_調劑畫面顯示調整.ShowDialog() != DialogResult.Yes) return;
                SaveConfig工程模式();
            }
            if (control.Name == "sqL_DataGridView_領藥台_02_領藥內容")
            {
                Dialog_調劑畫面顯示調整 dialog_調劑畫面顯示調整 = new Dialog_調劑畫面顯示調整(1);
                if (dialog_調劑畫面顯示調整.ShowDialog() != DialogResult.Yes) return;
                SaveConfig工程模式();
            }
            if (control.Name == "sqL_DataGridView_領藥台_03_領藥內容")
            {
                Dialog_調劑畫面顯示調整 dialog_調劑畫面顯示調整 = new Dialog_調劑畫面顯示調整(2);
                if (dialog_調劑畫面顯示調整.ShowDialog() != DialogResult.Yes) return;
                SaveConfig工程模式();
            }
            if (control.Name == "sqL_DataGridView_領藥台_04_領藥內容")
            {
                Dialog_調劑畫面顯示調整 dialog_調劑畫面顯示調整 = new Dialog_調劑畫面顯示調整(3);
                if (dialog_調劑畫面顯示調整.ShowDialog() != DialogResult.Yes) return;
                SaveConfig工程模式();
            }
        }

     
 
    }
}
