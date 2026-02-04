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

namespace 勤務傳送系統
{
    public partial class Main_Form : Form
    {
        private void Program_勤務取藥_Init()
        {
            this.plC_UI_Init.Add_Method(this.Program_勤務取藥);
            this.textBox_勤務取藥_條碼刷入區.KeyPress += TextBox_勤務取藥_條碼刷入區_KeyPress;

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

                        this.textBox_勤務取藥_條碼刷入區.Focus();

                    }));
                    MySerialPort_Scanner01.ClearReadByte();
                    flag_勤務取藥_頁面更新_init = false;
                }
            }
            else
            {
                flag_勤務取藥_頁面更新_init = true;
            }

            sub_Program_勤務取藥_刷入藥袋();
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
                    Task_勤務取藥_刷入藥袋 = new Task(new Action(delegate { Function_勤務取藥_刷入藥袋(); }));
                }
                if (Task_勤務取藥_刷入藥袋.Status == TaskStatus.RanToCompletion)
                {
                    Task_勤務取藥_刷入藥袋 = new Task(new Action(delegate { Function_勤務取藥_刷入藥袋(); }));
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
        MyTimerBasic MyTimerBasic_勤務取藥_刷藥單結束計時 = new MyTimerBasic();
        string 勤務取藥_text = "";
        private void Function_勤務取藥_刷入藥袋()
        {
            if (MyTimerBasic_勤務取藥_刷藥單結束計時.IsTimeOut())
            {
                if (rJ_Lable_勤務取藥_狀態.Text != "等待刷藥單...")
                {
                    this.Invoke(new Action(delegate
                    {
                        rJ_Lable_勤務取藥_狀態.BackgroundColor = Color.MidnightBlue;
                        rJ_Lable_勤務取藥_狀態.Text = "等待刷藥單...";

                        rJ_Lable_勤務取藥_藥名.Text = "";
                        rJ_Lable_勤務取藥_總量.Text = "";
                        rJ_Lable_勤務取藥_頻次.Text = "";
                        rJ_Lable_勤務取藥_病人姓名.Text = "";
                        rJ_Lable_勤務取藥_病歷號.Text = "";
                        rJ_Lable_勤務取藥_開方時間.Text = "";
                        rJ_Lable_勤務取藥_病房.Text = "";
                        //Application.DoEvents();
                    }));
                }
            }


            string text = MySerialPort_Scanner01.ReadString();
            if (text != null)
            {
                System.Threading.Thread.Sleep(200);
                MyTimerBasic_勤務取藥_刷藥單結束計時.TickStop();
                MyTimerBasic_勤務取藥_刷藥單結束計時.StartTickTime(100000);
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
                    textBox_勤務取藥_條碼刷入區.Text = text;
                    TextBox_勤務取藥_條碼刷入區_KeyPress(null, new KeyPressEventArgs((char)Keys.Enter));
                    Console.WriteLine($"接收掃碼內容:{text}");
                    //Application.DoEvents();
                }

            }));
            if (勤務取藥_text.StringIsEmpty() == true) return;
            勤務取藥_text = 勤務取藥_text.Replace("\r\n", "");
            List<OrderClass> orderClasses = this.Function_醫令資料_API呼叫(dBConfigClass.OrderApiURL, 勤務取藥_text);
            勤務取藥_text = "";
            if (orderClasses.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    rJ_Lable_勤務取藥_狀態.BackgroundColor = Color.HotPink;
                    rJ_Lable_勤務取藥_狀態.Text = "找無藥單資料!";
                    //Application.DoEvents();
                    MyTimerBasic_勤務取藥_刷藥單結束計時.TickStop();
                    MyTimerBasic_勤務取藥_刷藥單結束計時.StartTickTime(3000);
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
                textBox_勤務取藥_條碼刷入區.Text = "";
                rJ_Lable_勤務取藥_藥名.Text = $"  {orderClasses[0].藥品名稱}";
                rJ_Lable_勤務取藥_總量.Text = orderClasses[0].交易量;
                rJ_Lable_勤務取藥_頻次.Text = orderClasses[0].頻次;
                rJ_Lable_勤務取藥_病人姓名.Text = orderClasses[0].病人姓名;
                rJ_Lable_勤務取藥_病歷號.Text = orderClasses[0].病歷號;
                rJ_Lable_勤務取藥_開方時間.Text = orderClasses[0].開方日期;
                rJ_Lable_勤務取藥_病房.Text = $"{orderClasses[0].病房}-{orderClasses[0].床號}";
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
                    rJ_Lable_勤務取藥_狀態.BackgroundColor = Color.HotPink;
                    rJ_Lable_勤務取藥_狀態.Text = "此藥單未配藥,請通知藥局刷入";
                    //Application.DoEvents();
                    MyTimerBasic_勤務取藥_刷藥單結束計時.TickStop();
                    MyTimerBasic_勤務取藥_刷藥單結束計時.StartTickTime(3000);
                    using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\ttsmaker-請藥師重刷.wav"))
                    {
                        sp.Stop();
                        sp.Play();
                        sp.PlaySync();
                    }
                }));
                textBox_勤務取藥_條碼刷入區.Text = "";
                return;
            }
            else
            {
                if (list_交易紀錄[0][(int)enum_交易記錄查詢資料.領用時間].ToDateTimeString().StringToDateTime() == "1999-01-01 00:00:00".StringToDateTime())
                {
                    this.Invoke(new Action(delegate
                    {
                        string 領用人 = list_交易紀錄[0][(int)enum_交易記錄查詢資料.領用人].ObjectToString();
                        rJ_Lable_勤務取藥_狀態.BackgroundColor = Color.DarkGreen;
                        rJ_Lable_勤務取藥_狀態.Text = $"[{領用人}] 刷取成功!";
                        textBox_勤務取藥_條碼刷入區.Text = "";
                        //Application.DoEvents();
                    }));
                }
                else
                {
                    this.Invoke(new Action(delegate
                    {
                        rJ_Lable_勤務取藥_狀態.BackgroundColor = Color.HotPink;
                        rJ_Lable_勤務取藥_狀態.Text = "藥單重複刷取";
                        //Application.DoEvents();
                        MyTimerBasic_勤務取藥_刷藥單結束計時.TickStop();
                        MyTimerBasic_勤務取藥_刷藥單結束計時.StartTickTime(3000);
                        using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\fail_01.wav"))
                        {
                            sp.Stop();
                            sp.Play();
                            sp.PlaySync();
                        }
                    }));
                    textBox_勤務取藥_條碼刷入區.Text = "";
                    return;
                }


            }

            this.Invoke(new Action(delegate
            {
                //Application.DoEvents();
                MyTimerBasic_勤務取藥_刷藥單結束計時.TickStop();
                MyTimerBasic_勤務取藥_刷藥單結束計時.StartTickTime(5000);
                using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\sucess_01.wav"))
                {
                    sp.Stop();
                    sp.Play();
                    sp.PlaySync();
                }
                textBox_勤務取藥_條碼刷入區.Text = "";
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
        #endregion
        #region Event
        private void TextBox_勤務取藥_條碼刷入區_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || sender == null)
            {
                勤務取藥_text = textBox_勤務取藥_條碼刷入區.Text;
                textBox_勤務取藥_條碼刷入區.Text = "";
            }
        }
        private void PlC_RJ_Button_勤務取藥_條碼刷入區_清除_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                textBox_勤務取藥_條碼刷入區.Text = "";
            }));
        }
        #endregion
    }
}
