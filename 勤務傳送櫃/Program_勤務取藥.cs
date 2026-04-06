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
using DrawingClass;

namespace 勤務傳送系統
{
    public partial class Main_Form : Form
    {
        MyThread myThread_勤務取藥;

        private enum enum_勤務取藥_處方
        {
            [Description("GUID,VARCHAR,50,None")]
            GUID,
            [Description("Value,VARCHAR,50,None")]
            Value,

        }

        private void Program_勤務取藥_Init()
        {
            this.plC_UI_Init.Add_Method(this.Program_勤務取藥);
            this.textBox_勤務取藥_單處方_條碼刷入區.KeyPress += TextBox_勤務取藥_單處方_條碼刷入區_KeyPress;

            Table table = new Table(new enum_勤務取藥_處方());
            sqL_DataGridView_勤務取藥_全處方.RowsHeight = 60;
            sqL_DataGridView_勤務取藥_全處方.顯示首列 = false;
            sqL_DataGridView_勤務取藥_全處方.Init(table);
            sqL_DataGridView_勤務取藥_全處方.Set_ColumnVisible(false, new enum_勤務取藥_處方().GetEnumNames());
            sqL_DataGridView_勤務取藥_全處方.RowPostPaintingEventEx += SqL_DataGridView_勤務取藥_全處方_RowPostPaintingEventEx;
            sqL_DataGridView_勤務取藥_全處方.RowHeaderPostPaintingEvent += SqL_DataGridView_勤務取藥_全處方_RowHeaderPostPaintingEvent;

            button_勤務取藥_全處方_輸入醫令條碼.Click += Button_勤務取藥_全處方_輸入醫令條碼_Click;

            this.plC_UI_Init.Add_Method(this.Program_勤務取藥);
            myThread_勤務取藥 = new MyThread();
            myThread_勤務取藥.AutoRun(true);
            myThread_勤務取藥.Add_Method(sub_Program_勤務取藥_刷入藥袋);
            myThread_勤務取藥.SetSleepTime(10);
            myThread_勤務取藥.Trigger();

            this.plC_RJ_Button_勤務取藥_條碼刷入區_清除.MouseDownEvent += PlC_RJ_Button_勤務取藥_條碼刷入區_清除_MouseDownEvent;
        }

     

        bool flag_勤務取藥_頁面更新_init = false;
        private void Program_勤務取藥()
        {
            if (this.plC_ScreenPage_Main.PageText == "勤務取藥")
            {
                if (flag_勤務取藥_頁面更新_init)
                {
                    this.Invoke(new Action(delegate
                    {
                        if (this.plC_CheckBox_氣送作業.Checked)
                        {
                            if (PLC_Device_已登入.Bool == false)
                            {
                                MyMessageBox.ShowDialog("氣送作業模式,請先登入使用者!");
                                this.plC_ScreenPage_Main.SelecteTabText("登入畫面");
                            }
                            rJ_Lable_勤務取藥系統.Text = $"[氣送作業]勤務取藥系統";
                        }
                        else rJ_Lable_勤務取藥系統.Text = $"勤務取藥系統";

                        this.textBox_勤務取藥_單處方_條碼刷入區.Focus();

                    }));
                    MySerialPort_Scanner01.ClearReadByte();
                    flag_勤務取藥_頁面更新_init = false;
                }
            }
            else
            {
                flag_勤務取藥_頁面更新_init = true;
            }

        }
        #region PLC_勤務取藥_刷入藥袋
        PLC_Device PLC_Device_勤務取藥_刷入藥袋 = new PLC_Device("");
        PLC_Device PLC_Device_勤務取藥_刷入藥袋_OK = new PLC_Device("");
        Task Task_勤務取藥_刷入藥袋;
        MyTimer MyTimer_勤務取藥_刷入藥袋_結束延遲 = new MyTimer();
        int cnt_Program_勤務取藥_刷入藥袋 = 65534;
        void sub_Program_勤務取藥_刷入藥袋()
        {
            if (cnt_Program_勤務取藥_刷入藥袋 == 65534)
            {
                this.MyTimer_勤務取藥_刷入藥袋_結束延遲.StartTickTime(100);
                PLC_Device_勤務取藥_刷入藥袋.SetComment("PLC_勤務取藥_刷入藥袋");
                PLC_Device_勤務取藥_刷入藥袋_OK.SetComment("PLC_勤務取藥_刷入藥袋_OK");
                PLC_Device_勤務取藥_刷入藥袋.Bool = false;
                cnt_Program_勤務取藥_刷入藥袋 = 65535;
            }
            if (this.plC_ScreenPage_Main.PageText == "勤務取藥") PLC_Device_勤務取藥_刷入藥袋.Bool = true;
            if (cnt_Program_勤務取藥_刷入藥袋 == 65535) cnt_Program_勤務取藥_刷入藥袋 = 1;
            if (cnt_Program_勤務取藥_刷入藥袋 == 1) cnt_Program_勤務取藥_刷入藥袋_檢查按下(ref cnt_Program_勤務取藥_刷入藥袋);
            if (cnt_Program_勤務取藥_刷入藥袋 == 2) cnt_Program_勤務取藥_刷入藥袋_初始化(ref cnt_Program_勤務取藥_刷入藥袋);
            if (cnt_Program_勤務取藥_刷入藥袋 == 3) cnt_Program_勤務取藥_刷入藥袋 = 65500;
            if (cnt_Program_勤務取藥_刷入藥袋 > 1) cnt_Program_勤務取藥_刷入藥袋_檢查放開(ref cnt_Program_勤務取藥_刷入藥袋);

            if (cnt_Program_勤務取藥_刷入藥袋 == 65500)
            {
                this.MyTimer_勤務取藥_刷入藥袋_結束延遲.TickStop();
                this.MyTimer_勤務取藥_刷入藥袋_結束延遲.StartTickTime(100);
                PLC_Device_勤務取藥_刷入藥袋.Bool = false;
                PLC_Device_勤務取藥_刷入藥袋_OK.Bool = false;
                cnt_Program_勤務取藥_刷入藥袋 = 65535;
            }
        }
        void cnt_Program_勤務取藥_刷入藥袋_檢查按下(ref int cnt)
        {
            if (PLC_Device_勤務取藥_刷入藥袋.Bool) cnt++;
        }
        void cnt_Program_勤務取藥_刷入藥袋_檢查放開(ref int cnt)
        {
            if (!PLC_Device_勤務取藥_刷入藥袋.Bool) cnt = 65500;
        }
        void cnt_Program_勤務取藥_刷入藥袋_初始化(ref int cnt)
        {
            if (this.MyTimer_勤務取藥_刷入藥袋_結束延遲.IsTimeOut())
            {
                if (Task_勤務取藥_刷入藥袋 == null)
                {
                    Task_勤務取藥_刷入藥袋 = new Task(new Action(delegate
                    {
                        if (tabControlEx_勤務取藥.PageText == "勤務取藥_單處方") Function_勤務取藥_單處方_刷入藥袋();
                        if (tabControlEx_勤務取藥.PageText == "勤務取藥_全處方") Function_勤務取藥_全處方_刷入藥袋();
                    }));
                }
                if (Task_勤務取藥_刷入藥袋.Status == TaskStatus.RanToCompletion)
                {
                    Task_勤務取藥_刷入藥袋 = new Task(new Action(delegate 
                    {
                        if (tabControlEx_勤務取藥.PageText == "勤務取藥_單處方") Function_勤務取藥_單處方_刷入藥袋();
                        if (tabControlEx_勤務取藥.PageText == "勤務取藥_全處方") Function_勤務取藥_全處方_刷入藥袋();
                    }));
                }
                if (Task_勤務取藥_刷入藥袋.Status == TaskStatus.Created)
                {
                    Task_勤務取藥_刷入藥袋.Start();
                }
                cnt++;
            }
        }

        #endregion


        #region Function
        MyTimerBasic MyTimerBasic_勤務取藥_單處方_刷藥單結束計時 = new MyTimerBasic();
        string 勤務取藥_單處方_text = "";
        private void Function_勤務取藥_單處方_刷入藥袋()
        {
            if (MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.IsTimeOut())
            {
                if (rJ_Lable_勤務取藥_單處方_狀態.Text != "等待刷藥單...")
                {
                    this.Invoke(new Action(delegate
                    {
                        rJ_Lable_勤務取藥_單處方_狀態.BackgroundColor = Color.MidnightBlue;
                        rJ_Lable_勤務取藥_單處方_狀態.Text = "等待刷藥單...";

                        rJ_Lable_勤務取藥_單處方_藥名.Text = "";
                        rJ_Lable_勤務取藥_單處方_總量.Text = "";
                        rJ_Lable_勤務取藥_單處方_頻次.Text = "";
                        rJ_Lable_勤務取藥_單處方_病人姓名.Text = "";
                        rJ_Lable_勤務取藥_單處方_病歷號.Text = "";
                        rJ_Lable_勤務取藥_單處方_開方時間.Text = "";
                        rJ_Lable_勤務取藥_單處方_病房.Text = "";
                        //Application.DoEvents();
                    }));
                }
            }


            string text = MySerialPort_Scanner01.ReadString();
            if (text != null)
            {
                System.Threading.Thread.Sleep(200);
                MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.TickStop();
                MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.StartTickTime(100000);
                text = MySerialPort_Scanner01.ReadString();
                MySerialPort_Scanner01.ClearReadByte();
                text = text.Replace("\0", "");
                if (text.StringIsEmpty()) return;
                if (text.Length <= 2 || text.Length > 500)
                {
                    return;
                }
                text = text.Replace("\r\n", "");
            }



            this.Invoke(new Action(delegate
            {
                if (text.StringIsEmpty() == false)
                {
                    textBox_勤務取藥_單處方_條碼刷入區.Text = text;
                    TextBox_勤務取藥_單處方_條碼刷入區_KeyPress(null, new KeyPressEventArgs((char)Keys.Enter));
                    Console.WriteLine($"接收掃碼內容:{text}");
                    //Application.DoEvents();
                }

            }));
            if (勤務取藥_單處方_text.StringIsEmpty() == true) return;
            勤務取藥_單處方_text = 勤務取藥_單處方_text.Replace("\r\n", "");
            List<OrderClass> orderClasses = this.Function_醫令資料_API呼叫(dBConfigClass.OrderApiURL, 勤務取藥_單處方_text);
            勤務取藥_單處方_text = "";
            if (orderClasses.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    rJ_Lable_勤務取藥_單處方_狀態.BackgroundColor = Color.HotPink;
                    rJ_Lable_勤務取藥_單處方_狀態.Text = "找無藥單資料!";
                    //Application.DoEvents();
                    MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.TickStop();
                    MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.StartTickTime(3000);
                    using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\ttsmaker-請重刷.wav"))
                    {
                        sp.Stop();
                        sp.Play();
                        sp.PlaySync();
                    }
                }));
                return;
            }


            this.Invoke(new Action(delegate
            {
                textBox_勤務取藥_單處方_條碼刷入區.Text = "";
                rJ_Lable_勤務取藥_單處方_藥名.Text = $"  {orderClasses[0].藥品名稱}";
                rJ_Lable_勤務取藥_單處方_總量.Text = orderClasses[0].交易量;
                rJ_Lable_勤務取藥_單處方_頻次.Text = orderClasses[0].頻次;
                rJ_Lable_勤務取藥_單處方_病人姓名.Text = orderClasses[0].病人姓名;
                rJ_Lable_勤務取藥_單處方_病歷號.Text = orderClasses[0].病歷號;
                rJ_Lable_勤務取藥_單處方_開方時間.Text = orderClasses[0].開方日期;
                rJ_Lable_勤務取藥_單處方_病房.Text = $"{orderClasses[0].病房}-{orderClasses[0].床號}";
                //Application.DoEvents();
            }));
            List<object[]> list_交易紀錄 = this.sqL_DataGridView_交易記錄查詢.SQL_GetRows((int)enum_交易記錄查詢資料.GUID, orderClasses[0].GUID, false);
            if (this.plC_CheckBox_氣送作業.Checked)
            {
                //if (list_交易紀錄.Count == 0)
                //{
                //    object[] value = new object[new enum_交易記錄查詢資料().GetLength()];
                //    value[(int)enum_交易記錄查詢資料.GUID] = orderClasses[0].GUID;
                //    value[(int)enum_交易記錄查詢資料.動作] = enum_交易記錄查詢動作.藥袋刷入.GetEnumName();
                //    value[(int)enum_交易記錄查詢資料.藥品碼] = orderClasses[0].藥品碼;
                //    value[(int)enum_交易記錄查詢資料.領藥號] = orderClasses[0].領藥號;
                //    value[(int)enum_交易記錄查詢資料.藥品名稱] = orderClasses[0].藥品名稱;
                //    value[(int)enum_交易記錄查詢資料.頻次] = orderClasses[0].頻次;
                //    value[(int)enum_交易記錄查詢資料.病房號] = orderClasses[0].病房;
                //    value[(int)enum_交易記錄查詢資料.交易量] = orderClasses[0].交易量;
                //    value[(int)enum_交易記錄查詢資料.病人姓名] = orderClasses[0].病人姓名;
                //    value[(int)enum_交易記錄查詢資料.病歷號] = orderClasses[0].病歷號;
                //    value[(int)enum_交易記錄查詢資料.開方時間] = orderClasses[0].開方日期;
                //    value[(int)enum_交易記錄查詢資料.領用人] = this.登入者名稱;
                //    value[(int)enum_交易記錄查詢資料.領用時間] = "1999-01-01 00:00:00";
                //    value[(int)enum_交易記錄查詢資料.操作時間] = DateTime.Now.ToDateTimeString_6();
                //    value[(int)enum_交易記錄查詢資料.操作人] = this.登入者名稱;
                //    value[(int)enum_交易記錄查詢資料.備註] = "氣送作業";
                //    this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value, false);
                //}
                //else
                //{

                //    object[] value = list_交易紀錄[0];
                //    value[(int)enum_交易記錄查詢資料.領用人] = this.登入者名稱;
                //    value[(int)enum_交易記錄查詢資料.領用時間] = "1999-01-01 00:00:00";
                //    value[(int)enum_交易記錄查詢資料.備註] = "氣送作業";
                //    this.sqL_DataGridView_交易記錄查詢.SQL_ReplaceExtra(list_交易紀錄[0], false);
                //}

                //object[] value_醫令資料 = orderClasses[0].ClassToSQL<OrderClass, enum_醫囑資料>();
                //value_醫令資料[(int)enum_醫囑資料.狀態] = "已過帳";
                //value_醫令資料[(int)enum_醫囑資料.結方日期] = DateTime.MinValue.ToDateTimeString();
                //value_醫令資料[(int)enum_醫囑資料.展藥時間] = DateTime.MinValue.ToDateTimeString();
                //value_醫令資料[(int)enum_醫囑資料.過帳時間] = DateTime.Now.ToDateTimeString_6();

                //this.sqL_DataGridView_醫令資料.SQL_ReplaceExtra(value_醫令資料, false);
            }


            list_交易紀錄 = this.sqL_DataGridView_交易記錄查詢.SQL_GetRows((int)enum_交易記錄查詢資料.GUID, orderClasses[0].GUID, false);
            if (list_交易紀錄.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    rJ_Lable_勤務取藥_單處方_狀態.BackgroundColor = Color.HotPink;
                    rJ_Lable_勤務取藥_單處方_狀態.Text = "此藥單未配藥,請通知藥局刷入";
                    //Application.DoEvents();
                    MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.TickStop();
                    MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.StartTickTime(3000);
                    using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\ttsmaker-請藥師重刷.wav"))
                    {
                        sp.Stop();
                        sp.Play();
                        sp.PlaySync();
                    }
                }));
                textBox_勤務取藥_單處方_條碼刷入區.Text = "";
                return;
            }
            else
            {
                if (list_交易紀錄[0][(int)enum_交易記錄查詢資料.領用時間].ToDateTimeString().StringToDateTime() == "1999-01-01 00:00:00".StringToDateTime())
                {
                    this.Invoke(new Action(delegate
                    {
                        string 領用人 = list_交易紀錄[0][(int)enum_交易記錄查詢資料.領用人].ObjectToString();
                        rJ_Lable_勤務取藥_單處方_狀態.BackgroundColor = Color.DarkGreen;
                        rJ_Lable_勤務取藥_單處方_狀態.Text = $"[{領用人}] 刷取成功!";
                        textBox_勤務取藥_單處方_條碼刷入區.Text = "";
                        //Application.DoEvents();
                    }));
                }
                else
                {
                    this.Invoke(new Action(delegate
                    {
                        rJ_Lable_勤務取藥_單處方_狀態.BackgroundColor = Color.HotPink;
                        rJ_Lable_勤務取藥_單處方_狀態.Text = "藥單重複刷取";
                        //Application.DoEvents();
                        MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.TickStop();
                        MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.StartTickTime(3000);
                        using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\fail_01.wav"))
                        {
                            sp.Stop();
                            sp.Play();
                            sp.PlaySync();
                        }
                    }));
                    textBox_勤務取藥_單處方_條碼刷入區.Text = "";
                    return;
                }


            }

            this.Invoke(new Action(delegate
            {
                //Application.DoEvents();
                MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.TickStop();
                MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.StartTickTime(5000);
                using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\sucess_01.wav"))
                {
                    sp.Stop();
                    sp.Play();
                    sp.PlaySync();
                }
                textBox_勤務取藥_單處方_條碼刷入區.Text = "";
            }));
            list_交易紀錄[0][(int)enum_交易記錄查詢資料.領用時間] = DateTime.Now.ToDateTimeString_6();
            if (this.plC_CheckBox_氣送作業.Checked)
            {
                list_交易紀錄[0][(int)enum_交易記錄查詢資料.領用人] = this.登入者名稱;
            }
          
          
            string str = list_交易紀錄[0][(int)enum_交易記錄查詢資料.領用人].ObjectToString();
            transactionsClass transactionsClass = list_交易紀錄[0].SQLToClass<transactionsClass, enum_交易記錄查詢資料>();
            List<transactionsClass> transactionsClasses = new List<transactionsClass>();
            transactionsClasses.Add(transactionsClass);
            transactionsClass.update_by_guid(Main_Form.API_Server, transactionsClasses, Main_Form.ServerName, Main_Form.ServerType);
            if (this.plC_CheckBox_氣送作業.Checked)
            {
                str = str + "(氣送)";
            }
            Funtion_勤務取藥API(orderClasses[0], str, "");

        }

        string 勤務取藥_Keyin_barcode = "";
        MyTimerBasic MyTimerBasic_勤務取藥_全處方_刷藥單結束計時 = new MyTimerBasic();
        private void Function_勤務取藥_全處方_刷入藥袋()
        {
            try
            {
                if (MyTimerBasic_勤務取藥_全處方_刷藥單結束計時.IsTimeOut())
                {
                    if (rJ_Lable_勤務取藥_全處方_狀態.Text != "等待刷藥單...")
                    {
                        this.Invoke(new Action(delegate
                        {
                            rJ_Lable_勤務取藥_全處方_狀態.BackgroundColor = Color.White;
                            rJ_Lable_勤務取藥_全處方_狀態.BorderSize = 0;
                            rJ_Lable_勤務取藥_全處方_狀態.Text = "等待刷藥單";

                            rJ_Lable_勤務取藥_全處方_病房.Text = "";

                            sqL_DataGridView_勤務取藥_全處方.ClearGrid();
                        }));


                    }
                }
                string text = null;
                if (勤務取藥_Keyin_barcode.StringIsEmpty())
                {
                    int scn_load = 0;
                    if (text == null)
                    {
                        if (MySerialPort_Scanner01.IsConnected)
                        {
                            text = MySerialPort_Scanner01.ReadString();
                            scn_load = 1;
                        }
                    }
                    if (text == null)
                    {
                        if (MySerialPort_Scanner02.IsConnected)
                        {
                            text = MySerialPort_Scanner02.ReadString();
                            scn_load = 2;
                        }
                    }
                    if (text == null)
                    {
                        return;
                    }
                    System.Threading.Thread.Sleep(200);
                    MyTimerBasic_勤務取藥_全處方_刷藥單結束計時.TickStop();
                    MyTimerBasic_勤務取藥_全處方_刷藥單結束計時.StartTickTime(100000);
                    if (scn_load == 1) text = MySerialPort_Scanner01.ReadString();
                    if (scn_load == 2) text = MySerialPort_Scanner02.ReadString();
                    MySerialPort_Scanner01.ClearReadByte();
                    MySerialPort_Scanner02.ClearReadByte();
                    text = text.Replace("\0", "");
                    text = text.Replace("\n", "");
                    if (text.StringIsEmpty()) return;
                    if (text.Length <= 2 || text.Length > 500)
                    {
                        MySerialPort_Scanner01.ClearReadByte();
                        MySerialPort_Scanner02.ClearReadByte();

                        return;
                    }
                    text = text.Replace("\r\n", "");
                    Console.WriteLine($"接收掃碼內容:{text}");
                }
                else
                {
                    text = 勤務取藥_Keyin_barcode;
                    勤務取藥_Keyin_barcode = "";
                    Console.WriteLine($"接收Keyin內容:{text}");
                }

                List<OrderClass> orderClasses = this.Function_醫令資料_API呼叫(dBConfigClass.OrderApiURL, text);

                List<object[]> list_交易紀錄 = sqL_DataGridView_交易記錄查詢.SQL_GetRowsByIn((int)enum_交易記錄查詢資料.Order_GUID, orderClasses.Select(x => x.GUID).ToArray(), false);
                if (orderClasses.Count == 0)
                {
                    this.Invoke(new Action(delegate
                    {
                        rJ_Lable_勤務取藥_全處方_狀態.BackgroundColor = Color.HotPink;
                        rJ_Lable_勤務取藥_全處方_狀態.Text = "找無藥單資料!";
                        //Application.DoEvents();
                        MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.TickStop();
                        MyTimerBasic_勤務取藥_單處方_刷藥單結束計時.StartTickTime(3000);
                        using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\ttsmaker-請重刷.wav"))
                        {
                            sp.Stop();
                            sp.Play();
                            sp.PlaySync();
                        }
                    }));
                    return;
                }
                if (list_交易紀錄.Count == 0)
                {
                    this.Invoke(new Action(delegate
                    {
                        rJ_Lable_勤務取藥_全處方_狀態.BorderColor = Color.Red;
                        rJ_Lable_勤務取藥_全處方_狀態.BorderSize = 3;
                        rJ_Lable_勤務取藥_全處方_狀態.Text = "此藥單未配藥,請通知藥局刷入";
                        //Application.DoEvents();
                        MyTimerBasic_勤務取藥_全處方_刷藥單結束計時.TickStop();
                        MyTimerBasic_勤務取藥_全處方_刷藥單結束計時.StartTickTime(3000);
                        using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\ttsmaker-請藥師重刷.wav"))
                        {
                            sp.Stop();
                            sp.Play();
                            sp.PlaySync();
                        }
                    }));
                    textBox_勤務取藥_單處方_條碼刷入區.Text = "";
                    return;
                }
                if (orderClasses.Count > 0)
                {
                    this.Invoke(new Action(delegate
                    {
                        rJ_Lable_勤務取藥_全處方_病房.Text = orderClasses[0].病房;

                    }));
                }
                using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\sucess_01.wav"))
                {
                    sp.Stop();
                    sp.PlaySync();
                }

                foreach (OrderClass order in orderClasses)
                {
                    sqL_DataGridView_勤務取藥_全處方.AddRow(new object[] { order.GUID, order.JsonSerializationt() }, false);
                }
                sqL_DataGridView_勤務取藥_全處方.RefreshGrid();

                for(int i = 0; i < list_交易紀錄.Count; i++)
                {
                    if (list_交易紀錄[i][(int)enum_交易記錄查詢資料.領用時間].ToDateTimeString().StringToDateTime() == "1999-01-01 00:00:00".StringToDateTime())list_交易紀錄[i][(int)enum_交易記錄查詢資料.領用時間] = DateTime.Now.ToDateTimeString_6();
                }
             

                勤務取藥_Keyin_barcode = "";
                MyTimerBasic_勤務取藥_全處方_刷藥單結束計時.TickStop();
                MyTimerBasic_勤務取藥_全處方_刷藥單結束計時.StartTickTime(5000);
                transactionsClass.add(API_Server, list_交易紀錄.SQLToClass<transactionsClass , enum_交易記錄查詢資料>(), ServerName, ServerType);
                //OrderClass.add_and_updete_by_guid(API_Server, "", "", orderClasses);
                //Funtion_藥袋刷入API(orderClasses);

            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        #region Event
        private void Button_勤務取藥_全處方_輸入醫令條碼_Click(object sender, EventArgs e)
        {
            Dialog_OrderKeyin dialog_OrderKeyin = new Dialog_OrderKeyin();
            if (dialog_OrderKeyin.ShowDialog() != DialogResult.Yes) return;
            勤務取藥_Keyin_barcode = dialog_OrderKeyin.Value;
        }
        private void SqL_DataGridView_勤務取藥_全處方_RowHeaderPostPaintingEvent(object sender, Graphics g, Rectangle rect_hedder, Brush brush_background, Pen pen_border)
        {
            Brush brush = brush_background;
            Pen pen = pen_border;

            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            Rectangle rectangle;
            DataGridView dataGridView = this.sqL_DataGridView_勤務取藥_全處方.dataGridView;
            DataGridViewColumnCollection columns = dataGridView.Columns;
            using (Brush brush_title_background = new SolidBrush(Color.White))
            using (Brush brush_hedder = new SolidBrush(Color.White))
            {
                rectangle = this.sqL_DataGridView_勤務取藥_全處方.GetColumnBounds(enum_勤務取藥_處方.Value.GetEnumName());
                rectangle.Width = this.sqL_DataGridView_勤務取藥_全處方.Width;
                g.FillRectangle(brush_hedder, rectangle);
                g.DrawRectangle(new Pen(new SolidBrush(Color.White)), rectangle);


                //rectangle = this.sqL_DataGridView_庫存查詢.GetColumnBounds(enum_庫存查詢.藥碼.GetEnumName());
                //g.DrawRectangle(pen, rectangle);
                //DrawingClass.Draw.DrawString(g, "藥碼", new Font("微軟正黑體", 15, FontStyle.Bold), rectangle, Color.Black, DataGridViewContentAlignment.MiddleCenter);


            }
        }
        private void SqL_DataGridView_勤務取藥_全處方_RowPostPaintingEventEx(SQL_DataGridView sQL_DataGridView, DataGridViewRowPostPaintEventArgs e)
        {

            Color row_Backcolor = Color.WhiteSmoke;
            Color row_Forecolor = Color.Black;
            if (this.sqL_DataGridView_勤務取藥_全處方.dataGridView.Rows[e.RowIndex].Selected)
            {
                row_Backcolor = Color.Blue;
                row_Forecolor = Color.White;
            }
            object[] value = this.sqL_DataGridView_勤務取藥_全處方.GetRowsList()[e.RowIndex];
            OrderClass order = value[1].ObjectToString().JsonDeserializet<OrderClass>();
            bool has大瓶藥 = order.orderConfig.Any(m => m.狀態.StringToBool() && m.功能備註 == "大瓶藥");
            bool has不發藥 = order.orderConfig.Any(m => m.狀態.StringToBool() && m.功能備註 == "不發藥");
            if (has不發藥) row_Forecolor = Color.Gray;
            if (has大瓶藥) row_Backcolor = Color.Yellow;
            using (Brush brush = new SolidBrush(row_Backcolor))
            {
                int x = e.RowBounds.Left;
                int y = e.RowBounds.Top;
                int width = e.RowBounds.Width;
                int height = e.RowBounds.Height;


                e.Graphics.FillRectangle(brush, e.RowBounds);
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.White)), new Rectangle(x - 0, y - 0, width + 1, height + 1));
                Size size = new Size();
                PointF pointF = new PointF();
            

                string 序號 = $"{e.RowIndex + 1}.";
                double val = order.交易量.StringToDouble() * -1;
                size = val.ToString("0.00").MeasureText(new Font("標楷體", 30, FontStyle.Bold));
                
                DrawingClass.Draw.文字左上繪製(序號, new PointF(10, y + 15), new Font("標楷體", 20), row_Forecolor, e.Graphics);
                DrawingClass.Draw.文字左上繪製(order.藥品碼, new PointF(50, y + 15), new Font("標楷體", 20, FontStyle.Bold), row_Forecolor, e.Graphics);
                if (has不發藥 == false) DrawingClass.Draw.文字左上繪製(val.ToString("0.00"), new PointF(200, y + 8), new Font("標楷體", 32, FontStyle.Bold), Color.Green, e.Graphics);
                if (has不發藥 == true) DrawingClass.Draw.文字左上繪製(val.ToString("0.00"), new PointF(200, y + 8), new Font("標楷體", 32, FontStyle.Bold), row_Forecolor, e.Graphics);
                DrawingClass.Draw.文字左上繪製(order.藥品名稱, new PointF(400, y + 15), new Font("標楷體", 20, FontStyle.Bold), row_Forecolor, e.Graphics);
                string note = order.頻次;


                size = note.MeasureText(new Font("標楷體", 30, FontStyle.Regular));
                DrawingClass.Draw.文字左上繪製(note, new PointF(e.RowBounds.Width - size.Width - 120, y + 10), new Font("標楷體", 30, FontStyle.Regular), row_Forecolor, e.Graphics);
                var configMap = new Dictionary<string, Image>()
                {
                    { "大瓶藥", global::勤務傳送系統.Properties.Resources.大型點滴ON },
                    { "不發藥", global::勤務傳送系統.Properties.Resources.發藥ON }
                };

                var sortOrder = new Dictionary<string, int>()
                {
                    { "大瓶藥", 1 },
                    { "不發藥", 2 }
                };

                int img_num = 0;

                var sortedConfigs = order.orderConfig
                    .Where(z => z.狀態.StringToBool() && configMap.ContainsKey(z.功能備註))
                    .OrderBy(z => sortOrder.ContainsKey(z.功能備註) ? sortOrder[z.功能備註] : 999);

                foreach (var orderConfig in sortedConfigs)
                {
                    var img = configMap[orderConfig.功能備註];

                    e.Graphics.DrawImage(img,
                        e.RowBounds.Width - 50 - img_num * 50,
                        y + 10,
                        40,
                        40);

                    img_num++;
                }

            }
        }
        private void TextBox_勤務取藥_單處方_條碼刷入區_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || sender == null)
            {
                勤務取藥_單處方_text = textBox_勤務取藥_單處方_條碼刷入區.Text;
                textBox_勤務取藥_單處方_條碼刷入區.Text = "";
            }
        }
        private void PlC_RJ_Button_勤務取藥_條碼刷入區_清除_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                textBox_勤務取藥_單處方_條碼刷入區.Text = "";
            }));
        }
        #endregion
    }
}
