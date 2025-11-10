using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLUI;
using MyUI;
using Basic;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using MySQL_Login;
using HIS_DB_Lib;
namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
  
        public enum enum_人員資料_匯出
        {
            ID,
            姓名,
            性別,
            密碼,
            單位,
            卡號,
            一維條碼,
            藥師證字號,
        }
        public enum enum_人員資料_匯入
        {
            ID,
            姓名,
            性別,
            密碼,
            單位,
            卡號,
            一維條碼,
            藥師證字號,
        }
        public enum ContextMenuStrip_人員資料
        {
            [Description("S39014")]
            匯出,
            [Description("S39014")]
            匯入,
            [Description("S39014")]
            匯出選取資料,
            [Description("S39014")]
            登錄資料,
            [Description("S39014")]
            刪除選取資料,
            [Description("M8000")]
            自動分配未配置顏色人員,
        }


        private List<PLC_Device> List_PLC_Device_權限管理 = new List<PLC_Device>();
        private List<LoginDataWebAPI.Class_login_data> List_class_Login_Data = new List<LoginDataWebAPI.Class_login_data>();
        private List<LoginDataWebAPI.Class_login_data_index> List_class_Login_Data_index = new List<LoginDataWebAPI.Class_login_data_index>();
        public static string 人員資料_UID = "";
        public static string 人員資料_BarCode = "";
        public static SQL_DataGridView _sqL_DataGridView_人員資料;

        private void Program_人員資料_Init()
        {
            _sqL_DataGridView_人員資料 = this.sqL_DataGridView_人員資料;
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_人員資料, dBConfigClass.DB_person_page);

            this.loginUI.Set_login_data_DB(dBConfigClass.DB_person_page);
            this.loginUI.Set_login_data_index_DB(dBConfigClass.DB_person_page);
            this.loginUI.Init();


            string url = $"{dBConfigClass.Api_URL}/api/person_page/init";
            returnData returnData = new returnData();
            returnData.ServerType = enum_sys_serverSetting_Type.調劑台.GetEnumName();
            returnData.ServerName = $"{dBConfigClass.Name}";
            string json_in = returnData.JsonSerializationt();
            string json = Basic.Net.WEBApiPostJson($"{url}", json_in);
      

            Table table = json.JsonDeserializet<Table>();
            if (table == null)
            {
                MyMessageBox.ShowDialog($"人員資料表單建立失敗!! Api_URL:{dBConfigClass.Api_URL}");
                return;
            }
            this.sqL_DataGridView_人員資料.InitEx(table);
            this.sqL_DataGridView_人員資料.Set_ColumnVisible(false, new enum_人員資料().GetEnumNames());
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleLeft, enum_人員資料.ID);
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_人員資料.姓名);
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleCenter, enum_人員資料.性別);
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(300, DataGridViewContentAlignment.MiddleLeft, enum_人員資料.單位);
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_人員資料.藥師證字號);
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_人員資料.權限等級);
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_人員資料.顏色);
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_人員資料.卡號);
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_人員資料.一維條碼);

            this.sqL_DataGridView_人員資料.Set_ColumnSortMode(DataGridViewColumnSortMode.Automatic, enum_人員資料.ID);
            this.sqL_DataGridView_人員資料.Set_ColumnSortMode(DataGridViewColumnSortMode.Automatic, enum_人員資料.姓名);
            this.sqL_DataGridView_人員資料.Set_ColumnSortMode(DataGridViewColumnSortMode.Automatic, enum_人員資料.藥師證字號);
            this.sqL_DataGridView_人員資料.Set_ColumnSortMode(DataGridViewColumnSortMode.Automatic, enum_人員資料.單位);


            this.sqL_DataGridView_人員資料.DataGridRefreshEvent += SqL_DataGridView_人員資料_DataGridRefreshEvent;
            this.sqL_DataGridView_人員資料.RowEnterEvent += SqL_DataGridView_人員資料_RowEnterEvent;
            this.sqL_DataGridView_人員資料.RowDoubleClickEvent += SqL_DataGridView_人員資料_RowDoubleClickEvent;
            this.sqL_DataGridView_人員資料.MouseDown += SqL_DataGridView_人員資料_MouseDown;
            this.sqL_DataGridView_人員資料.RefreshGrid(Function_人員資料_取得人員資料().ClassToSQL<personPageClass, enum_人員資料>());


            this.plC_Button_權限設定_設定至Server.MouseDownEvent += PlC_Button_權限設定_設定至Server_MouseDownEvent;
            this.plC_RJ_ComboBox_權限管理_權限等級.OnSelectedIndexChanged += PlC_RJ_ComboBox_權限管理_權限等級_OnSelectedIndexChanged;
            this.plC_RJ_ComboBox_權限管理_權限等級.Items.Clear();
            int level_num = (int)this.loginUI.Level_num;
            for (int i = 1; i <= level_num; i++)
            {
                this.plC_RJ_ComboBox_權限管理_權限等級.Items.Add(i.ToString("00"));
            }
            for (int i = 0; i < 256; i++) this.List_PLC_Device_權限管理.Add(new PLC_Device($"S{39000 + i}"));

            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_交班作業_頁面顯示, 0);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_交班作業_交班對點, 3);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_交班作業_管制結存表匯出, 8);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_交班作業_管制結存表列印, 9);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_收支作業_頁面顯示, 32);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_收支作業_收支原因維護, 37);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_收支作業_入庫功能, 33);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_收支作業_出庫功能, 34);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_收支作業_調入功能, 35);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_收支作業_調出功能, 36);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_交易紀錄查詢_頁面顯示, 1);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_交易紀錄查詢_交易紀錄匯出, 38);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_醫令資料_頁面顯示, 22);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_藥品資料_頁面顯示, 2);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_藥品資料_資料更動, 7);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_藥品資料_儲位效期表顯示, 15);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_藥品資料_儲位總庫存表顯示, 16);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_藥品資料_藥品群組修改, 21);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_藥品資料_管制藥設定, 39);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_人員資料_頁面顯示, 4);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_人員資料_基本資料更動, 14);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_人員資料_權限內容更動, 17);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_儲位管理_頁面顯示, 5);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_儲位管理_庫存效期異動, 12);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_儲位管理_儲位刪除, 11);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_儲位管理_面板資訊更改, 13);       
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_盤點作業_頁面顯示, 26);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_盤點作業_報表下載, 27);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_盤點作業_報表鎖定, 28);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_盤點作業_報表刪除, 29);
            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_工程模式_頁面顯示, 6);

            this.loginIndex_Pannel.Set_CheckBox(plC_CheckBox_調劑作業_管1_3禁止調劑, 40);

            

            this.plC_RJ_Button_人員資料_匯出.MouseDownEvent += PlC_RJ_Button_人員資料_匯出_MouseDownEvent;
            this.plC_RJ_Button_人員資料_匯入.MouseDownEvent += PlC_RJ_Button_人員資料_匯入_MouseDownEvent;
            this.plC_RJ_Button_人員資料_登錄.MouseDownEvent += PlC_RJ_Button_人員資料_登錄_MouseDownEvent;
            this.plC_RJ_Button_人員資料_刪除.MouseDownEvent += PlC_RJ_Button_人員資料_刪除_MouseDownEvent;
            this.plC_RJ_Button_人員資料_清除內容.MouseDownEvent += PlC_RJ_Button_人員資料_清除內容_MouseDownEvent;

            this.plC_RJ_Button_人員資料_資料查詢_ID.MouseDownEvent += PlC_RJ_Button_人員資料_資料查詢_ID_MouseDownEvent;
            this.plC_RJ_Button_人員資料_資料查詢_姓名.MouseDownEvent += PlC_RJ_Button_人員資料_資料查詢_姓名_MouseDownEvent;
            this.plC_RJ_Button_人員資料_資料查詢_卡號.MouseDownEvent += PlC_RJ_Button_人員資料_資料查詢_卡號_MouseDownEvent;
            this.plC_RJ_Button_人員資料_資料查詢_一維條碼.MouseDownEvent += PlC_RJ_Button_人員資料_資料查詢_一維條碼_MouseDownEvent;

            this.plC_RJ_Button_人員資料_顯示全部.MouseDownEvent += PlC_RJ_Button_人員資料_顯示全部_MouseDownEvent;

            this.plC_RJ_Button_人員資料_RFID註冊.MouseDownEvent += PlC_RJ_Button_人員資料_RFID註冊_MouseDownEvent;
            this.plC_RJ_Button_人員資料_條碼註冊.MouseDownEvent += PlC_RJ_Button_人員資料_條碼註冊_MouseDownEvent;
 
            this.plC_Button_人員資料_指紋註冊.MouseDownEvent += PlC_Button_人員資料_指紋註冊_MouseDownEvent;
            this.plC_UI_Init.Add_Method(this.sub_Program_人員資料);
        }

        bool flag_人員資料_資料維護_頁面更新 = false;
        bool flag_人員資料_權限管理_頁面更新 = false;
        private void sub_Program_人員資料()
        {
            if (this.plC_ScreenPage_Main.PageText == "人員資料" && this.plC_ScreenPage_人員資料.PageText == "資料維護")
            {
                if (!this.flag_人員資料_資料維護_頁面更新)
                {
                    this.Function_管制抽屜_鎖控按鈕更新();
                    this.Function_人員資料_管制抽屜開鎖權限_UI更新();
                    this.plC_RJ_ComboBox_權限管理_權限等級.SetValue(0);
                    this.sqL_DataGridView_人員資料.RefreshGrid(Function_人員資料_取得人員資料().ClassToSQL<personPageClass, enum_人員資料>());
                    this.flag_人員資料_資料維護_頁面更新 = true;
                }
            }
            else
            {
                this.flag_人員資料_資料維護_頁面更新 = false;
            }
            if (this.plC_ScreenPage_Main.PageText == "人員資料" && this.plC_ScreenPage_人員資料.PageText == "權限管理")
            {
                if (!this.flag_人員資料_權限管理_頁面更新)
                {
                    this.List_class_Login_Data = this.loginUI.Get_login_data();
                    this.List_class_Login_Data_index = this.loginUI.Get_login_data_index();
                    this.loginIndex_Pannel.Set_Login_Data_Index(this.List_class_Login_Data_index, enum_login_data_type.調劑台);
                    this.loginIndex_Pannel.Set_Login_Data(this.List_class_Login_Data[0]);


                    this.Invoke(new Action(delegate
                    {
                        PLC_Device pLC_Device = new PLC_Device("S39014");
                        this.comboBox_人員資料_權限等級.Enabled = pLC_Device.Bool;
                    }));


                    this.flag_人員資料_權限管理_頁面更新 = true;
                }
            }
            else
            {
                this.flag_人員資料_權限管理_頁面更新 = false;
            }
            this.sub_Program_人員資料_接收設備資料();
        }
        #region PLC_人員資料_接收設備資料
        PLC_Device PLC_Device_人員資料_接收設備資料 = new PLC_Device("");
        int cnt_Program_人員資料_接收設備資料 = 65534;
        void sub_Program_人員資料_接收設備資料()
        {
            if (this.plC_ScreenPage_Main.PageText == "人員資料")
            {
                PLC_Device_人員資料_接收設備資料.Bool = true;
            }
            else
            {
                PLC_Device_人員資料_接收設備資料.Bool = false;
            }
            if (cnt_Program_人員資料_接收設備資料 == 65534)
            {
                PLC_Device_人員資料_接收設備資料.Bool = false;
                cnt_Program_人員資料_接收設備資料 = 65535;
            }
            if (cnt_Program_人員資料_接收設備資料 == 65535) cnt_Program_人員資料_接收設備資料 = 1;
            if (cnt_Program_人員資料_接收設備資料 == 1) cnt_Program_人員資料_接收設備資料_檢查按下(ref cnt_Program_人員資料_接收設備資料);
            if (cnt_Program_人員資料_接收設備資料 == 2) cnt_Program_人員資料_接收設備資料_初始化(ref cnt_Program_人員資料_接收設備資料);
            if (cnt_Program_人員資料_接收設備資料 == 3) cnt_Program_人員資料_接收設備資料 = 65500;
            if (cnt_Program_人員資料_接收設備資料 > 1) cnt_Program_人員資料_接收設備資料_檢查放開(ref cnt_Program_人員資料_接收設備資料);

            if (cnt_Program_人員資料_接收設備資料 == 65500)
            {
                PLC_Device_人員資料_接收設備資料.Bool = false;
                cnt_Program_人員資料_接收設備資料 = 65535;
            }
        }
        void cnt_Program_人員資料_接收設備資料_檢查按下(ref int cnt)
        {
            if ((PLC_Device_人員資料_接收設備資料.Bool == true)) cnt++;
        }
        void cnt_Program_人員資料_接收設備資料_檢查放開(ref int cnt)
        {
            if (!(PLC_Device_人員資料_接收設備資料.Bool == false)) cnt = 65500;
        }
        void cnt_Program_人員資料_接收設備資料_初始化(ref int cnt)
        {
            List<RFID_FX600lib.RFID_FX600_UI.RFID_Device> list_RFID_Devices = this.rfiD_FX600_UI.Get_RFID();
            if(list_RFID_Devices.Count > 0)
            {
                this.Invoke(new Action(delegate 
                {
                    人員資料_UID = list_RFID_Devices[0].UID;
                    rJ_TextBox_人員資料_資料查詢_卡號.Text = 人員資料_UID;
                }));
             
            }
            for (int i = 0; i < List_RFID_本地資料.Count; i++)
            {
                for (int k = 0; k < List_RFID_本地資料[i].DeviceClasses.Length; k++)
                {
                    if (List_RFID_本地資料[i].DeviceClasses[k].Enable)
                    {
                        string RFID = this.rfiD_UI.GetRFID(List_RFID_本地資料[i].IP, k);
                        if (RFID.StringToInt32() != 0 && !RFID.StringIsEmpty())
                        {
                            this.Invoke(new Action(delegate
                            {
                                人員資料_UID = RFID;
                                rJ_TextBox_人員資料_資料查詢_卡號.Text = 人員資料_UID;
                            }));
                            cnt++;
                            return;
                        }

                    }
                }
            }
            if (MySerialPort_Scanner01.ReadByte() != null)
            {
                System.Threading.Thread.Sleep(50);
                string text = MySerialPort_Scanner01.ReadString();
                MySerialPort_Scanner01.ClearReadByte();
                if (text == null) return;
                text = text.Replace("\0", "");
                if (text.Length <= 2 || text.Length > 30) return;
                if (text.Substring(text.Length - 2, 2) != "\r\n") return;
                text = text.Replace("\r\n", "");
                this.Invoke(new Action(delegate
                {
                    人員資料_BarCode = text;
                    rJ_TextBox_人員資料_資料查詢_一維條碼.Text = 人員資料_BarCode;
                }));
            }
            //if (MySerialPort_Scanner02.ReadByte() != null)
            //{
            //    System.Threading.Thread.Sleep(50);
            //    string text = MySerialPort_Scanner02.ReadString();
            //    MySerialPort_Scanner02.ClearReadByte();
            //    if (text == null) return;
            //    text = text.Replace("\0", "");
            //    if (text.Length <= 2 || text.Length > 30) return;
            //    if (text.Substring(text.Length - 2, 2) != "\r\n") return;
            //    text = text.Replace("\r\n", "");
            //    this.Invoke(new Action(delegate
            //    {
            //        人員資料_BarCode = text;
            //        rJ_TextBox_人員資料_資料查詢_一維條碼.Text = 人員資料_BarCode;
            //    }));
            //}
            cnt++;
        }


        #endregion

        #region Function
        private List<personPageClass> Function_人員資料_取得人員資料()
        {
            List<personPageClass> personPageClasses = new List<personPageClass>();
            string url = $"{dBConfigClass.Api_URL}/api/person_page/";
            returnData returnData = new returnData(url);
            returnData.ServerType = enum_sys_serverSetting_Type.調劑台.GetEnumName();
            returnData.ServerName = $"{dBConfigClass.Name}";
            string json = returnData.ApiPostJson();
            if(returnData.ResultData == null)
            {
                MyMessageBox.ShowDialog("取得人員資料失敗!");
                return personPageClasses;
            }
            personPageClasses = returnData.ResultData.Data.ObjToListClass<personPageClass>();
            return personPageClasses;
        }
        private string Function_人員資料_檢查內容(object[] value)
        {
            string str_error = "";
            List<string> list_error = new List<string>();
            if (value[(int)enum_人員資料.姓名].ObjectToString().StringIsEmpty())
            {
                list_error.Add("'姓名'欄位不得空白!");
            }
            if (value[(int)enum_人員資料.ID].ObjectToString().StringIsEmpty())
            {
                list_error.Add("'ID'欄位不得空白!");
            }
            for (int i = 0; i < list_error.Count; i++)
            {
                str_error += $"{(i + 1).ToString("00")}. {list_error[i]}";
                if (i != list_error.Count - 1) str_error += "\n";
            }
            return str_error;
        }
        private void Function_人員資料_清除內容()
        {
            this.Invoke(new Action(delegate
            {
                this.rJ_TextBox_人員資料_ID.Text = "";
                this.rJ_TextBox_人員資料_姓名.Text = "";
                this.rJ_TextBox_人員資料_密碼.Text = "";
                this.rJ_TextBox_人員資料_單位.Text = "";
                this.rJ_TextBox_人員資料_卡號.Text = "";
                this.textBox_人員資料_顏色.Text = colorDialog.Color.ToColorString();
                this.comboBox_人員資料_權限等級.Text = "";
                this.rJ_TextBox_人員資料_一維條碼.Text = "";
                this.rJ_TextBox_人員資料_識別圖案.Text = "";
                this.rJ_TextBox_人員資料_藥師證字號.Text = "";
            }));
        
        }
        private void Function_人員資料_登錄資料()
        {
            string 性別 = rJ_RatioButton_人員資料_男.Checked ? "男" : "女";
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            list_value_buf = list_value.GetRows((int)enum_人員資料.ID, rJ_TextBox_人員資料_ID.Text);
            object[] value = new object[new enum_人員資料().GetLength()];
            if (list_value_buf.Count == 0)
            {
                value[(int)enum_人員資料.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_人員資料.ID] = this.rJ_TextBox_人員資料_ID.Text;
                value[(int)enum_人員資料.姓名] = this.rJ_TextBox_人員資料_姓名.Text;
                value[(int)enum_人員資料.性別] = 性別;
                value[(int)enum_人員資料.密碼] = this.rJ_TextBox_人員資料_密碼.Text;
                value[(int)enum_人員資料.單位] = this.rJ_TextBox_人員資料_單位.Text;
                value[(int)enum_人員資料.卡號] = this.rJ_TextBox_人員資料_卡號.Text;
                value[(int)enum_人員資料.權限等級] = this.comboBox_人員資料_權限等級.Text;
                value[(int)enum_人員資料.顏色] = this.textBox_人員資料_顏色.Text;
                value[(int)enum_人員資料.一維條碼] = this.rJ_TextBox_人員資料_一維條碼.Text;
                value[(int)enum_人員資料.藥師證字號] = this.rJ_TextBox_人員資料_藥師證字號.Text;
                string str_error = this.Function_人員資料_檢查內容(value);
                if (!str_error.StringIsEmpty())
                {
                    MyMessageBox.ShowDialog(str_error);
                    return;
                }
                this.Function_人員資料_管制抽屜開鎖權限_從UI取得資料寫入SQL(value[(int)enum_人員資料.GUID].ObjectToString());
                this.sqL_DataGridView_人員資料.SQL_AddRow(value, false);
                this.sqL_DataGridView_人員資料.AddRow(value, true);
            }
            else
            {
                if (MyMessageBox.ShowDialog("此ID已註冊,是否覆寫?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    value = list_value_buf[0];
                    value[(int)enum_人員資料.ID] = this.rJ_TextBox_人員資料_ID.Text;
                    value[(int)enum_人員資料.姓名] = this.rJ_TextBox_人員資料_姓名.Text;
                    value[(int)enum_人員資料.性別] = 性別;
                    value[(int)enum_人員資料.密碼] = this.rJ_TextBox_人員資料_密碼.Text;
                    value[(int)enum_人員資料.單位] = this.rJ_TextBox_人員資料_單位.Text;
                    value[(int)enum_人員資料.卡號] = this.rJ_TextBox_人員資料_卡號.Text;
                    value[(int)enum_人員資料.權限等級] = this.comboBox_人員資料_權限等級.Text;
                    value[(int)enum_人員資料.顏色] = this.textBox_人員資料_顏色.Text;
                    value[(int)enum_人員資料.一維條碼] = this.rJ_TextBox_人員資料_一維條碼.Text;
                    value[(int)enum_人員資料.藥師證字號] = this.rJ_TextBox_人員資料_藥師證字號.Text;
                    string str_error = this.Function_人員資料_檢查內容(value);
                    if (!str_error.StringIsEmpty())
                    {
                        MyMessageBox.ShowDialog(str_error);
                        return;
                    }
                    this.Function_人員資料_管制抽屜開鎖權限_從UI取得資料寫入SQL(value[(int)enum_人員資料.GUID].ObjectToString());
                    this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(value, false);
                    this.sqL_DataGridView_人員資料.ReplaceExtra(value, true);
                }
            }

            Function_人員資料_清除內容();
        }
        private void Function_人員資料_匯出()
        {
            saveFileDialog_SaveExcel.OverwritePrompt = false;
            if (saveFileDialog_SaveExcel.ShowDialog(this) == DialogResult.OK)
            {
                DataTable datatable = new DataTable();
                datatable = sqL_DataGridView_人員資料.GetDataTable();
                datatable = datatable.ReorderTable(new enum_人員資料_匯出());
                string Extension = System.IO.Path.GetExtension(this.saveFileDialog_SaveExcel.FileName);
                if (Extension == ".txt")
                {
                    CSVHelper.SaveFile(datatable, this.saveFileDialog_SaveExcel.FileName);
                    MyMessageBox.ShowDialog("匯出完成!");
                }
                else if (Extension == ".xls"|| Extension == ".xlsx")
                {
                    MyOffice.ExcelClass.NPOI_SaveFile(datatable, this.saveFileDialog_SaveExcel.FileName);
                    MyMessageBox.ShowDialog("匯出完成!");
                }
            }
        }
        private void Function_人員資料_匯入()
        {
            try
            {
                List<object[]> list_SQL_Value_add = new List<object[]>();
                List<object[]> list_SQL_Value_replace = new List<object[]>();
                if (openFileDialog_LoadExcel.ShowDialog(this) == DialogResult.OK)
                {

                    DataTable dataTable = new DataTable();
                    string Extension = System.IO.Path.GetExtension(this.openFileDialog_LoadExcel.FileName);

                    if (Extension == ".txt")
                    {
                        dataTable = CSVHelper.LoadFile(this.openFileDialog_LoadExcel.FileName, 0, dataTable);
                    }
                    else if (Extension == ".xls" || Extension == ".xlsx")
                    {
                        dataTable = MyOffice.ExcelClass.NPOI_LoadFile(this.openFileDialog_LoadExcel.FileName);
                    }
                    if (dataTable == null)
                    {
                        MyMessageBox.ShowDialog("匯入失敗,請檢查是否檔案開啟中!");
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    DataTable datatable_buf = dataTable.ReorderTable(new enum_人員資料_匯入());
                    if (datatable_buf == null)
                    {
                        MyMessageBox.ShowDialog("匯入檔案,資料錯誤!");
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    List<object[]> list_LoadValue = datatable_buf.DataTableToRowList();
                    List<object[]> list_SQL_Value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);


                    if (datatable_buf.Columns.Contains("ID") == false)
                    {
                        MyMessageBox.ShowDialog("載入資料未包含'人員ID'");
                        return;
                    }
                    for (int i = 0; i < datatable_buf.Rows.Count; i++)
                    {
                        string ID = datatable_buf.Rows[i]["ID"].ToString();
                        string 姓名 = (datatable_buf.Columns.Contains(enum_人員資料.姓名.GetEnumName())) ? datatable_buf.Rows[i]["姓名"].ToString() : "none";
                        string 權限等級 = (datatable_buf.Columns.Contains(enum_人員資料.權限等級.GetEnumName())) ? datatable_buf.Rows[i]["權限等級"].ToString() : "none";
                        string 性別 = (datatable_buf.Columns.Contains(enum_人員資料.性別.GetEnumName())) ? datatable_buf.Rows[i]["性別"].ToString() : "none";
                        string 藥師證字號 = (datatable_buf.Columns.Contains(enum_人員資料.藥師證字號.GetEnumName())) ? datatable_buf.Rows[i]["藥師證字號"].ToString() : "none";
                        if (!(性別 == "男" || 性別 == "女") && 性別 != "none") 性別 = "男";

                        object[] obj = list_SQL_Value.Where(x => x[(int)enum_人員資料.ID].ObjectToString() == ID).FirstOrDefault();
                        if (obj == null)
                        {
                            obj = new object[new enum_人員資料().GetLength()];
                            obj[(int)enum_人員資料.GUID] = Guid.NewGuid().ToString();
                            obj[(int)enum_人員資料.ID] = ID;
                            if (姓名 != "none") obj[(int)enum_人員資料.姓名] = 姓名;
                            if (權限等級 != "none") obj[(int)enum_人員資料.權限等級] = 權限等級;
                            if (性別 != "none") obj[(int)enum_人員資料.性別] = 性別;
                            if (藥師證字號 != "none") obj[(int)enum_人員資料.藥師證字號] = 藥師證字號;
                            list_SQL_Value_add.Add(obj);
                        }
                        else
                        {
                            if (姓名 != "none") obj[(int)enum_人員資料.姓名] = 姓名;
                            if (權限等級 != "none") obj[(int)enum_人員資料.權限等級] = 權限等級;
                            if (性別 != "none") obj[(int)enum_人員資料.性別] = 性別;
                            if (藥師證字號 != "none") obj[(int)enum_人員資料.藥師證字號] = 藥師證字號;
                            list_SQL_Value_replace.Add(obj);
                        }

                    }


                    this.sqL_DataGridView_人員資料.SQL_AddRows(list_SQL_Value_add, false);
                    this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(list_SQL_Value_replace, false);
                    this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);
                    MyMessageBox.ShowDialog($"匯入完成,新增<{list_SQL_Value_add.Count}>筆,修改<{list_SQL_Value_replace.Count}>筆");
                }
              
            }
            catch
            {

            }
            finally
            {

            }
          
        }
        private void Function_人員資料_管制抽屜開鎖權限_UI更新()
        {
            Point basic_location = new Point(10, 10);
            int pLC_CheckBox_width = 250;
            int pLC_CheckBox_height = 25;
            Font pLC_CheckBox_font = new Font("微軟正黑體", 12);
            Point current_location = new Point(basic_location.X, basic_location.Y);
            this.Invoke(new Action(delegate 
            {
                List<Pannel_Locker> pannel_Lockers = this.pannel_Locker_Design.GetAllPannel_Locker();
                pannel_Lockers = pannel_Lockers.Distinct(new Pannel_Locker_Design.Distinct_Pannel_Locker()).ToList();
                this.panel_人員資料_管制抽屜開鎖權限.SuspendLayout();
                this.panel_人員資料_管制抽屜開鎖權限.Controls.Clear();
                for(int i = 0; i < pannel_Lockers.Count; i++)
                {
                    string title = "";
                    PLC_CheckBox pLC_CheckBox = new PLC_CheckBox();
                    pLC_CheckBox.音效 = false;
                    title = $"[{pannel_Lockers[i].ShortIP}:{pannel_Lockers[i].Num}] {pannel_Lockers[i].StorageName}";

                    pLC_CheckBox.Text = $"[{pannel_Lockers[i].ShortIP}:{pannel_Lockers[i].Num}] {pannel_Lockers[i].StorageName}";
                    pLC_CheckBox.Width = pLC_CheckBox_width;
                    pLC_CheckBox.Height = pLC_CheckBox_height;
                    pLC_CheckBox.Location = new Point(current_location.X, current_location.Y);
                    current_location.Y += pLC_CheckBox_height;
                    if (current_location.Y >= this.panel_人員資料_管制抽屜開鎖權限.Height)
                    {
                        current_location.X += pLC_CheckBox_width;
                        current_location.Y = basic_location.Y;
                    }
                    this.panel_人員資料_管制抽屜開鎖權限.Controls.Add(pLC_CheckBox);
                }
                this.panel_人員資料_管制抽屜開鎖權限.ResumeLayout(false);
            }));
        }
        private void Function_人員資料_管制抽屜開鎖權限_從SQL取得資料更新UI(string MasterGUID)
        {
            List<object[]> list_value = this.sqL_DataGridView_管制抽屜權限資料.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            list_value = list_value.GetRows((int)enum_管制抽屜權限.Master_GUID, MasterGUID);
            this.Invoke(new Action(delegate
            {

                for (int i = 0; i < this.panel_人員資料_管制抽屜開鎖權限.Controls.Count; i++)
                {
                    PLC_CheckBox pLC_CheckBox = this.panel_人員資料_管制抽屜開鎖權限.Controls[i] as PLC_CheckBox;
                    pLC_CheckBox.Checked = false;
                    if (pLC_CheckBox == null) continue;
                    string temp = pLC_CheckBox.Text.Split(' ')[0];
                    temp = temp.Replace("[", "");
                    temp = temp.Replace("]", "");
                    if (temp.Split(':').Length < 2) continue;
                    string IP = temp.Split(':')[0];
                    IP = $"192.168.{IP}";
                    string Num = temp.Split(':')[1];
                    list_value_buf = list_value.GetRows((int)enum_管制抽屜權限.IP, IP);
                    list_value_buf = list_value_buf.GetRows((int)enum_管制抽屜權限.Num, Num);
                    if (list_value_buf.Count > 0)
                    {
                        if(list_value_buf[0][(int)enum_管制抽屜權限.開鎖權限].ObjectToString().ToUpper() == true.ToString().ToUpper())
                        {
                            pLC_CheckBox.Checked = true;
                        }
                    }

                }
            }));

        }
        private void Function_人員資料_管制抽屜開鎖權限_從UI取得資料寫入SQL(string MasterGUID)
        {
            List<object[]> list_value = this.sqL_DataGridView_管制抽屜權限資料.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<object[]> list_value_add = new List<object[]>();
            List<object[]> list_value_replace = new List<object[]>();
            list_value = list_value.GetRows((int)enum_管制抽屜權限.Master_GUID, MasterGUID);
            this.Invoke(new Action(delegate
            {
                for (int i = 0; i < this.panel_人員資料_管制抽屜開鎖權限.Controls.Count; i++)
                {
                    PLC_CheckBox pLC_CheckBox = this.panel_人員資料_管制抽屜開鎖權限.Controls[i] as PLC_CheckBox;
                    if (pLC_CheckBox == null) continue;
                    string temp = pLC_CheckBox.Text.Split(' ')[0];
                    temp = temp.Replace("[", "");
                    temp = temp.Replace("]", "");
                    string IP = temp.Split(':')[0];
                    IP = $"192.168.{IP}";
                    string Num = temp.Split(':')[1];
                    list_value_buf = list_value.GetRows((int)enum_管制抽屜權限.IP, IP);
                    list_value_buf = list_value_buf.GetRows((int)enum_管制抽屜權限.Num, Num);
                    if (list_value_buf.Count == 0)
                    {
                        object[] value = new object[new enum_管制抽屜權限().GetLength()];
                        value[(int)enum_管制抽屜權限.GUID] = Guid.NewGuid().ToString();
                        value[(int)enum_管制抽屜權限.Master_GUID] = MasterGUID;
                        value[(int)enum_管制抽屜權限.IP] = IP;
                        value[(int)enum_管制抽屜權限.Num] = Num;
                        value[(int)enum_管制抽屜權限.開鎖權限] = pLC_CheckBox.Checked.ToString();
                        list_value_add.Add(value);
                    }
                    else
                    {
                        object[] value = list_value_buf[0];
                        value[(int)enum_管制抽屜權限.Master_GUID] = MasterGUID;
                        value[(int)enum_管制抽屜權限.IP] = IP;
                        value[(int)enum_管制抽屜權限.Num] = Num;
                        value[(int)enum_管制抽屜權限.開鎖權限] = pLC_CheckBox.Checked.ToString();
                        list_value_replace.Add(value);
                    }
                }
            }));

            this.sqL_DataGridView_管制抽屜權限資料.SQL_AddRows(list_value_add, false);
            this.sqL_DataGridView_管制抽屜權限資料.SQL_ReplaceExtra(list_value_replace, false);
        }
        private bool Function_人員資料_管制抽屜開鎖權限_從SQL取得權限(string MasterGUID, string IP, int num)
        {
            List<object[]> list_value = this.sqL_DataGridView_管制抽屜權限資料.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            list_value = list_value.GetRows((int)enum_管制抽屜權限.Master_GUID, MasterGUID);
            list_value = list_value.GetRows((int)enum_管制抽屜權限.IP, IP);
            list_value = list_value.GetRows((int)enum_管制抽屜權限.Num, num.ToString());
            if (list_value.Count > 0)
            {
                if (list_value[0][(int)enum_管制抽屜權限.開鎖權限].ObjectToString().ToUpper() == true.ToString().ToUpper()) return true;
            }
            return false;
        }
        private void Function_登入權限資料_取得權限(List<PermissionsClass> Permissions)
        {
            Permissions = (from temp in Permissions
                           where temp.類別.Contains("調劑台")
                           select temp).ToList();
            for (int i = 0; i < Permissions.Count; i++)
            {
               
                if(Permissions[i].狀態)
                {
                    this.List_PLC_Device_權限管理[Permissions[i].索引].Bool = true;
                }
                else
                {
                    this.List_PLC_Device_權限管理[Permissions[i].索引].Bool = false;
                }

            }
        }
    
        private void Function_登入權限資料_清除權限()
        {
            for (int i = 0; i < 256; i++)
            {
                if (i >= this.List_PLC_Device_權限管理.Count) break;
                this.List_PLC_Device_權限管理[i].Bool = false;
            }
            PLC_Device_最高權限.Bool = false;
            PLC_Device_最高權限 = PLC_Device_最高權限;

        }
      
        #endregion
        #region Event
        private void SqL_DataGridView_人員資料_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_人員資料());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_人員資料.匯入.GetEnumName())
                    {
                        Function_人員資料_匯入();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_人員資料.匯出.GetEnumName())
                    {
                        Function_人員資料_匯出();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_人員資料.匯出選取資料.GetEnumName())
                    {
                        saveFileDialog_SaveExcel.OverwritePrompt = false;
                        if (saveFileDialog_SaveExcel.ShowDialog(this) == DialogResult.OK)
                        {
                            DataTable datatable = new DataTable();
                            datatable = sqL_DataGridView_人員資料.GetSelectRowsDataTable();
                            datatable = datatable.ReorderTable(new enum_人員資料_匯出());
                            CSVHelper.SaveFile(datatable, saveFileDialog_SaveExcel.FileName);
                            MyMessageBox.ShowDialog("匯出完成!");
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_人員資料.刪除選取資料.GetEnumName())
                    {
                        DialogResult Result = MyMessageBox.ShowDialog("是否刪除選取欄位資料?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel);
                        if (Result == System.Windows.Forms.DialogResult.Yes)
                        {
                            List<object[]> list_value = this.sqL_DataGridView_人員資料.Get_All_Select_RowsValues();
                            this.sqL_DataGridView_人員資料.SQL_DeleteExtra(list_value, true);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_人員資料.登錄資料.GetEnumName())
                    {
                        Function_人員資料_登錄資料();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_人員資料.自動分配未配置顏色人員.GetEnumName())
                    {
                        int index = 0;
                        DialogResult Result = MyMessageBox.ShowDialog("是否自動分配未配置顏色人員?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel);
                        if (Result == System.Windows.Forms.DialogResult.Yes)
                        {
                            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);

                            for(int i = 0; i < list_value.Count; i++)
                            {
                                string color_str = list_value[i][(int)enum_人員資料.顏色].ObjectToString();
                                if(color_str.ToColor() == Color.Black || color_str.StringIsEmpty())
                                {
                                    if (index > 6) index = 0;
                                    if (index == 0) list_value[i][(int)enum_人員資料.顏色] = Color.Red.ToColorString();
                                    if (index == 1) list_value[i][(int)enum_人員資料.顏色] = Color.Orange.ToColorString();
                                    if (index == 2) list_value[i][(int)enum_人員資料.顏色] = Color.Yellow.ToColorString();
                                    if (index == 3) list_value[i][(int)enum_人員資料.顏色] = Color.Linen.ToColorString();
                                    if (index == 4) list_value[i][(int)enum_人員資料.顏色] = Color.Blue.ToColorString();
                                    if (index == 5) list_value[i][(int)enum_人員資料.顏色] = Color.Pink.ToColorString();
                                    if (index == 6) list_value[i][(int)enum_人員資料.顏色] = Color.PeachPuff.ToColorString();
                                    index++;


                                }
                            }
                            this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(list_value, true);
                        }
                    }
                }
            }
        }
        private void button_人員資料_顏色選擇_Click(object sender, EventArgs e)
        {
            if(this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_人員資料_顏色.Text = this.colorDialog.Color.ToColorString();
                textBox_人員資料_顏色.BackColor = textBox_人員資料_顏色.Text.ToColor();
            }
        }
        private void SqL_DataGridView_人員資料_DataGridRefreshEvent()
        {
            for (int i = 0; i < this.sqL_DataGridView_人員資料.dataGridView.Rows.Count; i++)
            {

                Color color = this.sqL_DataGridView_人員資料.dataGridView.Rows[i].Cells[enum_人員資料.顏色.GetEnumName()].Value.ObjectToString().ToColor();
                this.sqL_DataGridView_人員資料.dataGridView.Rows[i].Cells[enum_人員資料.顏色.GetEnumName()].Style.BackColor = color;
                this.sqL_DataGridView_人員資料.dataGridView.Rows[i].Cells[enum_人員資料.顏色.GetEnumName()].Style.ForeColor = color;
            }
        }
        private void SqL_DataGridView_人員資料_RowEnterEvent(object[] RowValue)
        {
            //rJ_TextBox_人員資料_ID.Text = RowValue[(int)enum_人員資料.ID].ObjectToString();
            //rJ_TextBox_人員資料_姓名.Text = RowValue[(int)enum_人員資料.姓名].ObjectToString();
            //rJ_TextBox_人員資料_密碼.Text = RowValue[(int)enum_人員資料.密碼].ObjectToString();
            //rJ_TextBox_人員資料_單位.Text = RowValue[(int)enum_人員資料.單位].ObjectToString();
            //comboBox_人員資料_權限等級.Text = RowValue[(int)enum_人員資料.權限等級].ObjectToString();
            //textBox_人員資料_顏色.Text = RowValue[(int)enum_人員資料.顏色].ObjectToString();
            //textBox_人員資料_顏色.BackColor = textBox_人員資料_顏色.Text.ToColor();
            //rJ_TextBox_人員資料_卡號.Text = RowValue[(int)enum_人員資料.卡號].ObjectToString();
            //rJ_TextBox_人員資料_一維條碼.Text = RowValue[(int)enum_人員資料.一維條碼].ObjectToString();
            //rJ_TextBox_人員資料_識別圖案.Text = RowValue[(int)enum_人員資料.識別圖案].ObjectToString();


            //string 性別 = RowValue[(int)enum_人員資料.性別].ObjectToString();
            //if (性別 == "男") rJ_RatioButton_人員資料_男.Checked = true;
            //else rJ_RatioButton_人員資料_女.Checked = true;
        }
        private void SqL_DataGridView_人員資料_RowDoubleClickEvent(object[] RowValue)
        {
            rJ_TextBox_人員資料_ID.Text = RowValue[(int)enum_人員資料.ID].ObjectToString();
            rJ_TextBox_人員資料_姓名.Text = RowValue[(int)enum_人員資料.姓名].ObjectToString();
            rJ_TextBox_人員資料_密碼.Text = RowValue[(int)enum_人員資料.密碼].ObjectToString();
            rJ_TextBox_人員資料_單位.Text = RowValue[(int)enum_人員資料.單位].ObjectToString();
            comboBox_人員資料_權限等級.Text = RowValue[(int)enum_人員資料.權限等級].ObjectToString();
            textBox_人員資料_顏色.Text = RowValue[(int)enum_人員資料.顏色].ObjectToString();
            textBox_人員資料_顏色.BackColor = textBox_人員資料_顏色.Text.ToColor();
            rJ_TextBox_人員資料_卡號.Text = RowValue[(int)enum_人員資料.卡號].ObjectToString();
            rJ_TextBox_人員資料_一維條碼.Text = RowValue[(int)enum_人員資料.一維條碼].ObjectToString();
            rJ_TextBox_人員資料_識別圖案.Text = RowValue[(int)enum_人員資料.識別圖案].ObjectToString();
            rJ_TextBox_人員資料_藥師證字號.Text = RowValue[(int)enum_人員資料.藥師證字號].ObjectToString();


            string 性別 = RowValue[(int)enum_人員資料.性別].ObjectToString();
            if (性別 == "男") rJ_RatioButton_人員資料_男.Checked = true;
            else rJ_RatioButton_人員資料_女.Checked = true;

            this.Function_人員資料_管制抽屜開鎖權限_從SQL取得資料更新UI(RowValue[(int)enum_人員資料.GUID].ObjectToString());
        }

        private void PlC_RJ_ComboBox_權限管理_權限等級_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            int level = plC_RJ_ComboBox_權限管理_權限等級.Text.StringToInt32();
            if (level > 0)
            {
                List<LoginDataWebAPI.Class_login_data> List_class_Login_Data_buf = new List<LoginDataWebAPI.Class_login_data>();
                List_class_Login_Data_buf = (from value in List_class_Login_Data
                                             where value.level.StringToInt32() == level
                                             select value).ToList();
                if (List_class_Login_Data_buf.Count > 0)
                {
                    this.loginIndex_Pannel.Set_Login_Data(List_class_Login_Data_buf[0]);
                }
            }
        }
        private void PlC_Button_權限設定_設定至Server_MouseDownEvent(MouseEventArgs mevent)
        {
            for (int i = 0; i < List_class_Login_Data.Count; i++)
            {
                this.loginUI.Set_login_data(List_class_Login_Data[i]);
            }
            this.Invoke(new Action(delegate
            {
                MyMessageBox.ShowDialog("權限更動完成!");
            }));
        }
        private void PlC_RJ_Button_人員資料_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<object[]> list_value = this.sqL_DataGridView_人員資料.Get_All_Checked_RowsValues();

                DialogResult Result = MyMessageBox.ShowDialog($"是否刪除選取欄位資料,共<{list_value.Count}>筆?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel);
                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
               
                    this.sqL_DataGridView_人員資料.SQL_DeleteExtra(list_value, true);
                }
            }));
        }
        private void PlC_RJ_Button_人員資料_登錄_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.Function_人員資料_登錄資料();
            }));
        }
        private void PlC_RJ_Button_人員資料_匯入_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.Function_人員資料_匯入();
            }));
        }
        private void PlC_RJ_Button_人員資料_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.Function_人員資料_匯出();
            }));
        }
        private void PlC_RJ_Button_人員資料_清除內容_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Function_人員資料_清除內容();
        }

        private void PlC_RJ_Button_人員資料_資料查詢_一維條碼_MouseDownEvent(MouseEventArgs mevent)
        {
            if(rJ_TextBox_人員資料_資料查詢_一維條碼.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("搜尋條件空白!");
                return;
            }
            string text = rJ_TextBox_人員資料_資料查詢_一維條碼.Text;
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            list_value = (from temp in list_value
                          where temp[(int)enum_人員資料.一維條碼].ObjectToString().ToUpper().Contains(text.ToUpper())
                          select temp).ToList();
            if(list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
                return;
            }
            this.sqL_DataGridView_人員資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_人員資料_資料查詢_卡號_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_人員資料_資料查詢_卡號.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("搜尋條件空白!");
                return;
            }
            string text = rJ_TextBox_人員資料_資料查詢_卡號.Text;
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            list_value = (from temp in list_value
                          where temp[(int)enum_人員資料.卡號].ObjectToString().ToUpper().Contains(text.ToUpper())
                          select temp).ToList();
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
                return;
            }
            this.sqL_DataGridView_人員資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_人員資料_資料查詢_姓名_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_人員資料_資料查詢_姓名.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("搜尋條件空白!");
                return;
            }
            string text = rJ_TextBox_人員資料_資料查詢_姓名.Text;
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            list_value = (from temp in list_value
                          where temp[(int)enum_人員資料.姓名].ObjectToString().ToUpper().Contains(text.ToUpper())
                          select temp).ToList();
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
                return;
            }
            this.sqL_DataGridView_人員資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_人員資料_資料查詢_ID_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_人員資料_資料查詢_ID.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("搜尋條件空白!");
                return;
            }
            string text = rJ_TextBox_人員資料_資料查詢_ID.Text;

            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            list_value = (from temp in list_value
                          where temp[(int)enum_人員資料.ID].ObjectToString().ToUpper().Contains(text.ToUpper())
                          select temp).ToList();

            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
                return;
            }
            this.sqL_DataGridView_人員資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_人員資料_顯示全部_MouseDownEvent(MouseEventArgs mevent)
        {
            this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);
        }
        private void PlC_Button_人員資料_指紋註冊_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_AlarmForm dialog_AlarmForm;
            List<object[]> list_value = this.sqL_DataGridView_人員資料.Get_All_Select_RowsValues();
            if (list_value.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選取資料", 2000);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            string name = list_value[0][(int)enum_人員資料.姓名].ObjectToString();
            string id = list_value[0][(int)enum_人員資料.ID].ObjectToString();
            if (fpMatchSoket.IsOpen == false && flag_指紋辨識_Init == false)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("指紋模組未初始化", 2000);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            Dialog_指紋建置 dialog_指紋建置 = new Dialog_指紋建置(name, id);
            if (dialog_指紋建置.ShowDialog() != DialogResult.Yes) return;
            list_value[0][(int)enum_人員資料.指紋辨識] = dialog_指紋建置.Value.feature;

            this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(list_value[0], false);
            dialog_AlarmForm = new Dialog_AlarmForm("設定完成", 1500, Color.Green);
            dialog_AlarmForm.ShowDialog();
        }
        private void PlC_RJ_Button_人員資料_條碼註冊_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                Dialog_AlarmForm dialog_AlarmForm;
                List<object[]> list_value = this.sqL_DataGridView_人員資料.Get_All_Select_RowsValues();
                if (list_value.Count == 0)
                {
                    dialog_AlarmForm = new Dialog_AlarmForm("未選取資料", 2000);
                    dialog_AlarmForm.ShowDialog();
                    return;
                }
                人員資料_BarCode = "";
                Dialog_等待條碼刷入 dialog_等待條碼刷入 = new Dialog_等待條碼刷入();
                if (dialog_等待條碼刷入.ShowDialog() != DialogResult.Yes) return;
                string UID = dialog_等待條碼刷入.Value;

                list_value[0][(int)enum_人員資料.一維條碼] = UID;
                rJ_TextBox_人員資料_一維條碼.Text = UID;
                this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(list_value[0], false);
                this.sqL_DataGridView_人員資料.ReplaceExtra(list_value[0], true);
                dialog_AlarmForm = new Dialog_AlarmForm("設定完成", 1500, Color.Green);
                dialog_AlarmForm.ShowDialog();

               
            }
            finally
            {

            }
        }
        private void PlC_RJ_Button_人員資料_RFID註冊_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                Dialog_AlarmForm dialog_AlarmForm;
                List<object[]> list_value = this.sqL_DataGridView_人員資料.Get_All_Select_RowsValues();
                if (list_value.Count == 0)
                {
                    dialog_AlarmForm = new Dialog_AlarmForm("未選取資料", 2000);
                    dialog_AlarmForm.ShowDialog();
                    return;
                }
                人員資料_UID = "";
                Dialog_等待RFID感應 dialog_等待RFID感應 = new Dialog_等待RFID感應();
                if (dialog_等待RFID感應.ShowDialog() != DialogResult.Yes) return;
                string UID = dialog_等待RFID感應.Value;

                list_value[0][(int)enum_人員資料.卡號] = UID;
                this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(list_value[0], false);
                this.sqL_DataGridView_人員資料.ReplaceExtra(list_value[0], true);
                dialog_AlarmForm = new Dialog_AlarmForm("設定完成", 1500, Color.Green);
                dialog_AlarmForm.ShowDialog();


            }
            finally
            {
            }
        }

        #endregion
    }
}
