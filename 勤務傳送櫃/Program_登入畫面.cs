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
namespace 勤務傳送系統
{
    public partial class Main_Form : Form
    {
        readonly private string Admin_ID = "admin";
        readonly private string Admoin_Password = "66437068";
        private bool flag_登入畫面_頁面更新 = false;

        private PLC_Device PLC_Device_已登入 = new PLC_Device("S4000");
        private PLC_Device PLC_Device_最高權限 = new PLC_Device("S4077");
        private string 登入者名稱
        {
            get
            {
                return this.rJ_TextBox_登入者姓名.Texts;
            }
            set
            {
                this.rJ_TextBox_登入者姓名.Texts = value;
            }
        }
        private string 登入者ID
        {
            get
            {
                return this.rJ_TextBox_登入者ID.Texts;
            }
            set
            {
                this.rJ_TextBox_登入者ID.Texts = value;
            }
        }
        private string 登入者顏色
        {
            get
            {
                return this.rJ_TextBox_登入者顏色.Text;
            }
            set
            {
                Color color = value.ToColor();
                this.rJ_TextBox_登入者顏色.ForeColor = color;
                this.rJ_TextBox_登入者顏色.BackColor = color;
                this.rJ_TextBox_登入者顏色.Text = color.ToColorString();
            }
        }

        private void Program_登入畫面_Init()
        {
            plC_RJ_Button_登入畫面_登入.MouseDownEvent += PlC_RJ_Button_登入畫面_登入_MouseDownEvent;
            plC_RJ_Button_登入畫面_登出.MouseDownEvent += PlC_RJ_Button_登入畫面_登出_MouseDownEvent;
            textBox_登入畫面_帳號.KeyPress += TextBox_登入畫面_帳號_KeyPress;
            textBox_登入畫面_密碼.KeyPress += TextBox_登入畫面_密碼_KeyPress;

            this.Function_登出();

            this.plC_UI_Init.Add_Method(this.Program_登入畫面);


        }
        private void Program_登入畫面()
        {
            if (this.plC_ScreenPage_Main.PageText == "登入畫面")
            {
                if (!this.flag_登入畫面_頁面更新)
                {
                    this.Invoke(new Action(delegate
                    {
                        this.textBox_登入畫面_帳號.Text = "";
                        this.textBox_登入畫面_密碼.Text = "";
                    }));
                    this.flag_登入畫面_頁面更新 = true;
                }
            }
            else
            {
                this.flag_登入畫面_頁面更新 = false;
            }
            this.Program_登入畫面_RFID登入();

            //if (plC_CheckBox_勤務取藥.Checked)
            //{
            //    this.plC_ScreenPage_Main.SelecteTabText("勤務取藥");
            //}
        }
        #region PLC_登入畫面_RFID登入
        PLC_Device PLC_Device_登入畫面_RFID登入 = new PLC_Device("");
        int cnt_Program_登入畫面_RFID登入 = 65534;
        void Program_登入畫面_RFID登入()
        {
            if (this.plC_ScreenPage_Main.PageText == "登入畫面")
            {
                PLC_Device_登入畫面_RFID登入.Bool = true;
            }
            else
            {
                PLC_Device_登入畫面_RFID登入.Bool = false;
            }
            if (cnt_Program_登入畫面_RFID登入 == 65534)
            {
                PLC_Device_登入畫面_RFID登入.SetComment("PLC_登入畫面_RFID登入");
                PLC_Device_登入畫面_RFID登入.Bool = false;
                cnt_Program_登入畫面_RFID登入 = 65535;
            }
            if (cnt_Program_登入畫面_RFID登入 == 65535) cnt_Program_登入畫面_RFID登入 = 1;
            if (cnt_Program_登入畫面_RFID登入 == 1) cnt_Program_登入畫面_RFID登入_檢查按下(ref cnt_Program_登入畫面_RFID登入);
            if (cnt_Program_登入畫面_RFID登入 == 2) cnt_Program_登入畫面_RFID登入_初始化(ref cnt_Program_登入畫面_RFID登入);
            if (cnt_Program_登入畫面_RFID登入 == 3) cnt_Program_登入畫面_RFID登入_檢查權限登入(ref cnt_Program_登入畫面_RFID登入);
            if (cnt_Program_登入畫面_RFID登入 == 4) cnt_Program_登入畫面_RFID登入_讀取RFID(ref cnt_Program_登入畫面_RFID登入);
            if (cnt_Program_登入畫面_RFID登入 == 5) cnt_Program_登入畫面_RFID登入_開始登入(ref cnt_Program_登入畫面_RFID登入);
            if (cnt_Program_登入畫面_RFID登入 == 6) cnt_Program_登入畫面_RFID登入_等待登入完成(ref cnt_Program_登入畫面_RFID登入);
            if (cnt_Program_登入畫面_RFID登入 == 7) cnt_Program_登入畫面_RFID登入 = 65500;
            if (cnt_Program_登入畫面_RFID登入 > 1) cnt_Program_登入畫面_RFID登入_檢查放開(ref cnt_Program_登入畫面_RFID登入);

            if (cnt_Program_登入畫面_RFID登入 == 65500)
            {
                PLC_Device_登入畫面_RFID登入.Bool = false;
                cnt_Program_登入畫面_RFID登入 = 65535;
            }
        }
        void cnt_Program_登入畫面_RFID登入_檢查按下(ref int cnt)
        {
            if (PLC_Device_登入畫面_RFID登入.Bool) cnt++;
        }
        void cnt_Program_登入畫面_RFID登入_檢查放開(ref int cnt)
        {
            if (!PLC_Device_登入畫面_RFID登入.Bool) cnt = 65500;
        }
        void cnt_Program_登入畫面_RFID登入_初始化(ref int cnt)
        {

            cnt++;
        }
        void cnt_Program_登入畫面_RFID登入_檢查權限登入(ref int cnt)
        {
            if (!this.PLC_Device_已登入.Bool)
            {
                cnt++;
                return;
            }
            else
            {
                cnt = 65500;
                return;
            }

        }
        void cnt_Program_登入畫面_RFID登入_讀取RFID(ref int cnt)
        {
            string RFID = "0";
            List<RFID_FX600lib.RFID_FX600_UI.RFID_Device> list_RFID = this.rfiD_FX600_UI.Get_RFID();
            if (list_RFID.Count != 0)
            {
                string rfid = Function_RFID卡號轉換(list_RFID[0].UID);
                if (rfid.StringToInt32() != 0)
                {
                    RFID = rfid;
                }
            }         
            else
            {
                //List<RFID_UI.RFID_UID_Class> list_RFID_UID_Class = this.rfiD_UI.GetRFID();
                //if (list_RFID_UID_Class.Count > 0)
                //{

                //    RFID = list_RFID_UID_Class[0].UID;
                //}

            }
           
            if (RFID.StringToInt32() == 0 || RFID.StringIsEmpty())
            {
                cnt = 65500;
                return;
            }

            List<object[]> list_人員資料 = this.sqL_DataGridView_人員資料.SQL_GetRows(enum_人員資料.卡號.GetEnumName(), RFID, false);
            if (list_人員資料.Count > 0)
            {
                this.Invoke(new Action(delegate
                {
                    this.textBox_登入畫面_帳號.Texts = list_人員資料[0][(int)enum_人員資料.ID].ObjectToString();
                    this.textBox_登入畫面_密碼.Texts = list_人員資料[0][(int)enum_人員資料.密碼].ObjectToString();
                }));
                新增交易紀錄(enum_交易記錄查詢動作.RFID登入, this.登入者ID, this.登入者名稱, "登入畫面");
            }
            else
            {
                MyMessageBox.ShowDialog(string.Format("查無此卡帳號! {0}", RFID));
                cnt = 65500;
                return;
            }

            cnt++;
        }
        void cnt_Program_登入畫面_RFID登入_開始登入(ref int cnt)
        {
            Function_登入();
            cnt++;
        }
        void cnt_Program_登入畫面_RFID登入_等待登入完成(ref int cnt)
        {
            cnt++;
        }
        #endregion

        #region Function
        private bool Function_登入()
        {
            bool flag = false;

            this.Invoke(new Action(delegate
            {
                if (this.textBox_登入畫面_帳號.Text.ToUpper() == Admin_ID.ToUpper())
                {
                    if (this.textBox_登入畫面_密碼.Text.ToUpper() == Admoin_Password.ToUpper())
                    {
                        this.登入者名稱 = "最高管理權限";
                        this.登入者ID = "admin";
                        this.登入者顏色 = "";
                        this.textBox_登入畫面_帳號.Text = "";
                        this.textBox_登入畫面_密碼.Text = "";
                        this.Function_登入權限資料_最高權限();
                        this.PLC_Device_已登入.Bool = true;
                        this.Text = $"{this.FormText}         [登入者名稱 : {登入者名稱}] [登入者ID : {登入者ID}]";
                        flag = true;
                        Function_功能顯示();
                        return;
                    }
                }


                List<object[]> list_人員資料 = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                List<object[]> list_人員資料_buf = new List<object[]>();
                string ID = this.textBox_登入畫面_帳號.Text;
                string password = textBox_登入畫面_密碼.Text;
                list_人員資料_buf = list_人員資料.GetRows((int)enum_人員資料.ID, ID);
                if (list_人員資料_buf.Count > 0)
                {
                    if (password != list_人員資料_buf[0][(int)enum_人員資料.密碼].ObjectToString())
                    {
                        flag = false;
                        return;
                    }
                    this.登入者名稱 = list_人員資料_buf[0][(int)enum_人員資料.姓名].ObjectToString();
                    this.登入者ID = list_人員資料_buf[0][(int)enum_人員資料.ID].ObjectToString();
                    this.登入者顏色 = list_人員資料_buf[0][(int)enum_人員資料.顏色].ObjectToString();
                    int level = list_人員資料_buf[0][(int)enum_人員資料.權限等級].StringToInt32();
                    this.Function_登入權限資料_取得權限(level);
                    this.textBox_登入畫面_帳號.Text = "";
                    this.textBox_登入畫面_密碼.Text = "";
                    this.PLC_Device_已登入.Bool = true;
                    this.Text = $"{this.FormText}         [登入者名稱 : {登入者名稱}] [登入者ID : {登入者ID}]";
                    flag = true;
                    Function_功能顯示();
                    return;
                }
                flag = false;
            }));
            return flag;
        }
        private void Function_登出()
        {
            if (this.登入者名稱.StringIsEmpty() == false) 新增交易紀錄(enum_交易記錄查詢動作.登出, this.登入者ID, this.登入者名稱, "登入畫面");
            this.Invoke(new Action(delegate
            {
                this.登入者名稱 = "";
                this.登入者ID = "";
                this.登入者顏色 = "";
                this.textBox_登入畫面_帳號.Text = "";
                this.textBox_登入畫面_密碼.Text = "";
                this.PLC_Device_已登入.Bool = false;
                this.Function_登入權限資料_清除權限();
                this.PLC_Device_最高權限.Bool = false;
                this.Text = $"{this.FormText}";
                Function_功能隱藏();
            }));
            PLC_Device_主頁面頁碼.Value = 0;
        }
        private void Function_功能顯示()
        {
            if(this.plC_CheckBox_配藥核對.Checked || PLC_Device_最高權限.Bool)
            {
                this.plC_ScreenPage_Main.SelecteTabText("配藥核對");
                this.plC_RJ_ScreenButton_系統頁面.Visible = true;
                this.plC_RJ_ScreenButton_人員資料.Visible = true;
                this.plC_RJ_ScreenButton_交易紀錄.Visible = true;
                this.plC_RJ_ScreenButton_醫令資料.Visible = true;
                this.plC_RJ_ScreenButton_藥品資料.Visible = true;
                this.plC_RJ_ScreenButton_配藥核對.Visible = true;
                this.plC_RJ_ScreenButton_櫃體狀態.Visible = true;
                panel_Main.Visible = true;

            }
            else if (this.plC_CheckBox_氣送作業.Checked)
            {
                this.plC_ScreenPage_Main.SelecteTabText("勤務取藥");
            }
            
        }
        private void Function_功能隱藏()
        {
            this.plC_RJ_ScreenButton_櫃體狀態.Visible = false;
            this.plC_RJ_ScreenButton_配藥核對.Visible = false;
            this.plC_RJ_ScreenButton_藥品資料.Visible = false;
            this.plC_RJ_ScreenButton_醫令資料.Visible = false;
            this.plC_RJ_ScreenButton_交易紀錄.Visible = false;
            this.plC_RJ_ScreenButton_人員資料.Visible = false;
            this.plC_RJ_ScreenButton_系統頁面.Visible = false;
            this.plC_ScreenPage_Main.SelecteTabText("登入畫面");
        }
        #endregion
        #region Event
        private void TextBox_登入畫面_帳號_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Tab)
            {
                textBox_登入畫面_密碼.Focus();
            }
        }
        private void TextBox_登入畫面_密碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (!this.Function_登入())
                {
                    textBox_登入畫面_帳號.Focus();
                }
                else
                {
                    新增交易紀錄(enum_交易記錄查詢動作.密碼登入, this.登入者ID, this.登入者名稱, "登入畫面");
                }
            }
        }
        private void PlC_RJ_Button_登入畫面_登入_MouseDownEvent(MouseEventArgs mevent)
        {
            Function_登入();
        }
        private void PlC_RJ_Button_登入畫面_登出_MouseDownEvent(MouseEventArgs mevent)
        {
            Function_登出();
        }
        #endregion
    }
}
