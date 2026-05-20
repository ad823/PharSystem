using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyUI;
using Basic;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using MySQL_Login;
using H_Pannel_lib;
using HIS_DB_Lib;
using SQLUI;
using MyOffice;
namespace 勤務傳送系統
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
            藥師證字號,
            卡號,
            一維條碼,
            開門權限
        }
        public enum enum_人員資料_匯入
        {
            ID,
            姓名,
            性別,
            密碼,
            單位,
            藥師證字號,
            卡號,
            一維條碼,
            開門權限
        }
        public enum ContextMenuStrip_人員資料
        {
            [Description("M8000")]
            匯出,
            [Description("M8000")]
            匯入,
            [Description("M8000")]
            匯出選取資料,
            [Description("M8000")]
            登錄資料,
            [Description("M8000")]
            刪除選取資料,
        }

        private List<PLC_Device> List_PLC_Device_權限管理 = new List<PLC_Device>();
        private List<LoginDataWebAPI.Class_login_data> List_class_Login_Data = new List<LoginDataWebAPI.Class_login_data>();
        private List<LoginDataWebAPI.Class_login_data_index> List_class_Login_Data_index = new List<LoginDataWebAPI.Class_login_data_index>();
        private List<OpenDoorPermission_UI> openDoorPermission_UIs = new List<OpenDoorPermission_UI>();
        private void Program_人員資料_Init()
        {

            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_人員資料, dBConfigClass.DB_person_page);

            this.loginUI.Set_login_data_DB(dBConfigClass.DB_person_page);
            this.loginUI.Set_login_data_index_DB(dBConfigClass.DB_person_page);
            this.loginUI.Init();

            string url = $"{dBConfigClass.Api_URL}/api/person_page/init";
            returnData returnData = new returnData();
            returnData.ServerType = enum_sys_serverSetting_Type.傳送櫃.GetEnumName();
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
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_人員資料.權限等級);
            this.sqL_DataGridView_人員資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_人員資料.卡號);
            this.sqL_DataGridView_人員資料.RowDoubleClickEvent += SqL_DataGridView_人員資料_RowDoubleClickEvent;
            this.sqL_DataGridView_人員資料.DataGridRefreshEvent += SqL_DataGridView_人員資料_DataGridRefreshEvent;

            this.plC_Button_權限設定_設定至Server.MouseDownEvent += PlC_Button_權限設定_設定至Server_MouseDownEvent;
            this.plC_RJ_ComboBox_權限管理_權限等級.OnSelectedIndexChanged += PlC_RJ_ComboBox_權限管理_權限等級_OnSelectedIndexChanged;
            this.plC_RJ_ComboBox_權限管理_權限等級.Items.Clear();
            int level_num = (int)this.loginUI.Level_num;
            for (int i = 1; i <= level_num; i++)
            {
                this.plC_RJ_ComboBox_權限管理_權限等級.Items.Add(i.ToString("00"));
            }
            for (int i = 0; i < 256; i++) this.List_PLC_Device_權限管理.Add(new PLC_Device($"S{39000 + i}"));


            this.plC_RJ_Button_人員資料_匯出.MouseDownEvent += PlC_RJ_Button_人員資料_匯出_MouseDownEvent;
            this.plC_RJ_Button_人員資料_匯入.MouseDownEvent += PlC_RJ_Button_人員資料_匯入_MouseDownEvent;
            this.plC_RJ_Button_人員資料_登錄.MouseDownEvent += PlC_RJ_Button_人員資料_登錄_MouseDownEvent;
            this.plC_RJ_Button_人員資料_刪除.MouseDownEvent += PlC_RJ_Button_人員資料_刪除_MouseDownEvent;
            this.plC_RJ_Button_人員資料_清除內容.MouseDownEvent += PlC_RJ_Button_人員資料_清除內容_MouseDownEvent;
            this.plC_RJ_Button_人員資料_開門權限全開.MouseDownEvent += PlC_RJ_Button_人員資料_開門權限全開_MouseDownEvent;
            this.plC_RJ_Button_人員資料_開門權限全關.MouseDownEvent += PlC_RJ_Button_人員資料_開門權限全關_MouseDownEvent;

            this.plC_RJ_Button_人員資料_資料查詢_ID.MouseDownEvent += PlC_RJ_Button_人員資料_資料查詢_ID_MouseDownEvent;
            this.plC_RJ_Button_人員資料_資料查詢_姓名.MouseDownEvent += PlC_RJ_Button_人員資料_資料查詢_姓名_MouseDownEvent;
            this.plC_RJ_Button_人員資料_資料查詢_卡號.MouseDownEvent += PlC_RJ_Button_人員資料_資料查詢_卡號_MouseDownEvent;
            this.plC_RJ_Button_人員資料_資料查詢_一維條碼.MouseDownEvent += PlC_RJ_Button_人員資料_資料查詢_一維條碼_MouseDownEvent;
            this.plC_RJ_Button_人員資料_顯示全部.MouseDownEvent += PlC_RJ_Button_人員資料_顯示全部_MouseDownEvent;

            this.Function_人員資料_開門權限_初始化();

            this.plC_UI_Init.Add_Method(this.Program_人員資料);
        }

        bool flag_人員資料_頁面更新 = false;
        private void Program_人員資料()
        {
            if (this.plC_ScreenPage_Main.PageText == "人員資料")
            {
                if (!this.flag_人員資料_頁面更新)
                {
                    this.List_class_Login_Data = this.loginUI.Get_login_data();
                    this.List_class_Login_Data_index = this.loginUI.Get_login_data_index();
                    this.plC_RJ_ComboBox_權限管理_權限等級.SetValue(0);
                    this.loginIndex_Pannel.Set_Login_Data_Index(this.List_class_Login_Data_index);
                    this.loginIndex_Pannel.Set_Login_Data(this.List_class_Login_Data[0]);

                    this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);

                    string IP = "";
                    int RFID_Num = -1;
                    List<object[]> list_Box_Index_Table = this.sqL_DataGridView_Box_Index_Table.SQL_GetAllRows(false);
                    List<object[]> list_Box_Index_Table_buf = new List<object[]>();
                    List<string> list_wardName = Pannel_Box.GetAllWardName();
                    for (int i = 0; i < list_wardName.Count; i++)
                    {
      
                        this.Invoke(new Action(delegate

                        {
                            openDoorPermission_UIs[i].WardName = list_wardName[i];
                            openDoorPermission_UIs[i].Visible = true;
                        }));
               
                    }
                    Application.DoEvents();


                    this.flag_人員資料_頁面更新 = true;
                }
            }
            else
            {
                this.flag_人員資料_頁面更新 = false;
            }
            this.Program_人員資料_讀取RFID();
        }
        #region PLC_人員資料_讀取RFID
        PLC_Device PLC_Device_人員資料_讀取RFID = new PLC_Device("");
        int cnt_Program_人員資料_讀取RFID = 65534;
        void Program_人員資料_讀取RFID()
        {
            if (this.plC_ScreenPage_Main.PageText == "人員資料")
            {
                PLC_Device_人員資料_讀取RFID.Bool = true;
            }
            else
            {
                PLC_Device_人員資料_讀取RFID.Bool = false;
            }
            if (cnt_Program_人員資料_讀取RFID == 65534)
            {
                PLC_Device_人員資料_讀取RFID.Bool = false;
                cnt_Program_人員資料_讀取RFID = 65535;
            }
            if (cnt_Program_人員資料_讀取RFID == 65535) cnt_Program_人員資料_讀取RFID = 1;
            if (cnt_Program_人員資料_讀取RFID == 1) cnt_Program_人員資料_讀取RFID_檢查按下(ref cnt_Program_人員資料_讀取RFID);
            if (cnt_Program_人員資料_讀取RFID == 2) cnt_Program_人員資料_讀取RFID_初始化(ref cnt_Program_人員資料_讀取RFID);
            if (cnt_Program_人員資料_讀取RFID == 3) cnt_Program_人員資料_讀取RFID = 65500;
            if (cnt_Program_人員資料_讀取RFID > 1) cnt_Program_人員資料_讀取RFID_檢查放開(ref cnt_Program_人員資料_讀取RFID);

            if (cnt_Program_人員資料_讀取RFID == 65500)
            {
                PLC_Device_人員資料_讀取RFID.Bool = false;
                cnt_Program_人員資料_讀取RFID = 65535;
            }
        }
        void cnt_Program_人員資料_讀取RFID_檢查按下(ref int cnt)
        {
            if ((PLC_Device_人員資料_讀取RFID.Bool == true)) cnt++;
        }
        void cnt_Program_人員資料_讀取RFID_檢查放開(ref int cnt)
        {
            if (!(PLC_Device_人員資料_讀取RFID.Bool == false)) cnt = 65500;
        }
        void cnt_Program_人員資料_讀取RFID_初始化(ref int cnt)
        {
            List<RFID_FX600lib.RFID_FX600_UI.RFID_Device> list_RFID_Devices = this.rfiD_FX600_UI.Get_RFID();
            if (list_RFID_Devices.Count > 0)
            {
                string cardNo = Function_RFID卡號轉換(list_RFID_Devices[0].UID);
                this.Invoke(new Action(delegate
                {
                    this.rJ_TextBox_人員資料_卡號.Text = cardNo;
                    rJ_TextBox_人員資料_資料查詢_一維條碼.Text = cardNo;
                }));
                cnt++;
                return;

            }
            cnt++;
        }

        #endregion

        #region Function
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
                value[(int)enum_人員資料.開門權限] = this.openDoorPermission_UIs.GetOpenDoorPermission();
                string str_error = this.Function_人員資料_檢查內容(value);
                if (!str_error.StringIsEmpty())
                {
                    MyMessageBox.ShowDialog(str_error);
                    return;
                }
                this.sqL_DataGridView_人員資料.SQL_AddRow(value, true);
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
                    value[(int)enum_人員資料.開門權限] = this.openDoorPermission_UIs.GetOpenDoorPermission();
                    string str_error = this.Function_人員資料_檢查內容(value);
                    if (!str_error.StringIsEmpty())
                    {
                        MyMessageBox.ShowDialog(str_error);
                        return;
                    }
                    this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(value, true);
                }
            }

            Function_人員資料_清除內容();
        }
        private void Function_人員資料_匯出()
        {
            saveFileDialog_SaveExcel.OverwritePrompt = false;
            if (saveFileDialog_SaveExcel.ShowDialog(this) == DialogResult.OK)
            {
                this.Invoke(new Action(delegate
                {
                    DataTable datatable = new DataTable();
                    List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                    for (int i = 0; i < list_value.Count; i++)
                    {
                        string str = "";
                        List<string> list_開門權限 = list_value[i][(int)enum_人員資料.開門權限].ObjectToString().JsonDeserializet<List<string>>();
                        if (list_開門權限 == null) continue;
                        for(int k = 0; k < list_開門權限.Count; k++)
                        {
                            str += list_開門權限[k];
                            if (k != list_開門權限.Count - 1) str += ",";
                        }
                        list_value[i][(int)enum_人員資料.開門權限] = str;
                    }
                    datatable = list_value.ToDataTable(new enum_人員資料());
                    datatable = datatable.ReorderTable(new enum_人員資料_匯出());
                    string Extension = System.IO.Path.GetExtension(this.saveFileDialog_SaveExcel.FileName);
                    if (Extension == ".txt")
                    {
                        CSVHelper.SaveFile(datatable, this.saveFileDialog_SaveExcel.FileName);
                        MyMessageBox.ShowDialog("匯出完成!");
                    }
                    else if (Extension == ".xls" || Extension == ".xlsx")
                    {
                        MyOffice.ExcelClass.NPOI_SaveFile(datatable, this.saveFileDialog_SaveExcel.FileName);
                        MyMessageBox.ShowDialog("匯出完成!");
                    }
                    else if (Extension == ".csv")
                    {
                        CSVHelper.SaveFile(datatable, this.saveFileDialog_SaveExcel.FileName);
                        MyMessageBox.ShowDialog("匯出完成!");
                    }
                }));

            }
        }
        private void Function_人員資料_匯入()
        {
            if (openFileDialog_LoadExcel.ShowDialog(this) == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;

                DataTable dataTable = new DataTable();
                string Extension = System.IO.Path.GetExtension(this.openFileDialog_LoadExcel.FileName);

                if (Extension == ".txt")
                {
                    CSVHelper.LoadFile(this.openFileDialog_LoadExcel.FileName, 0, dataTable);
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
                List<object[]> list_SQL_Value_buf = new List<object[]>();
                List<object[]> list_Add = new List<object[]>();

                List<string> list_Replace_SerchValue = new List<string>();
                List<object[]> list_Replace_Value = new List<object[]>();

                for (int i = 0; i < list_LoadValue.Count; i++)
                {
                    object[] value_load = list_LoadValue[i];
                    value_load = value_load.CopyRow(new enum_人員資料_匯入(), new enum_人員資料());
                    if (!Function_人員資料_檢查內容(value_load).StringIsEmpty()) continue;

                    string 性別 = value_load[(int)enum_人員資料.性別].ObjectToString();
                    string 權限等級 = value_load[(int)enum_人員資料.權限等級].ObjectToString();
                    if (!(性別 == "男" || 性別 == "女")) 性別 = "男";
                    if (權限等級.StringToInt32() <= 0 || 權限等級.StringToInt32() > 20) 權限等級 = "01";
                    value_load[(int)enum_人員資料.性別] = 性別;
                    value_load[(int)enum_人員資料.權限等級] = 權限等級;
                    string str_開門權限 = value_load[(int)enum_人員資料.開門權限].ObjectToString();
                    string[] ary_開門權限 = str_開門權限.Split(',');
                    List<string> list_開門權限 = new List<string>();
                    for (int k = 0; k < ary_開門權限.Length; k++)
                    {
                        list_開門權限.Add(ary_開門權限[k]);
                    }
                    value_load[(int)enum_人員資料.開門權限] = list_開門權限.JsonSerializationt();
                    list_SQL_Value_buf = list_SQL_Value.GetRows((int)enum_人員資料.ID, value_load[(int)enum_人員資料.ID].ObjectToString());
                    if (list_SQL_Value_buf.Count > 0)
                    {
                        object[] value_SQL = list_SQL_Value_buf[0];
                        value_load[(int)enum_人員資料.GUID] = value_SQL[(int)enum_人員資料.GUID];
                        list_Replace_Value.Add(value_load);
                    }
                    else
                    {
                        value_load[(int)enum_人員資料.GUID] = Guid.NewGuid().ToString();
                        list_Add.Add(value_load);
                    }
                }
                this.sqL_DataGridView_人員資料.SQL_AddRows(list_Add, false);
                this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(list_Replace_Value, false);
                this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);
                this.Cursor = Cursors.Default;
                MyMessageBox.ShowDialog("匯入完成!");
            }
            this.Cursor = Cursors.Default;

        }

        private void Function_登入權限資料_取得權限(int level)
        {
            LoginDataWebAPI.Class_login_data class_Login_Data = this.loginUI.Get_login_data(level);
            if (class_Login_Data != null)
            {
                for (int i = 0; i < class_Login_Data.data.Count; i++)
                {
                    this.List_PLC_Device_權限管理[i].Bool = class_Login_Data.data[i];
                }
            }
        }
        private void Function_登入權限資料_最高權限()
        {
            for (int i = 0; i < 256; i++)
            {
                this.List_PLC_Device_權限管理[i].Bool = true;
            }
            PLC_Device_最高權限.Bool = true;
        }
        private void Function_登入權限資料_清除權限()
        {
            for (int i = 0; i < 256; i++)
            {
                this.List_PLC_Device_權限管理[i].Bool = false;
            }
            PLC_Device_最高權限.Bool = false;
        }


        private void Function_人員資料_開門權限_初始化()
        {
            this.SuspendLayout();
  
            List<FlowLayoutPanel> flowLayoutPanels = new List<FlowLayoutPanel>();
            flowLayoutPanels.Add(flowLayoutPanel_開門權限_01);
            flowLayoutPanels.Add(flowLayoutPanel_開門權限_02);
            flowLayoutPanels.Add(flowLayoutPanel_開門權限_03);
            flowLayoutPanels.Add(flowLayoutPanel_開門權限_04);
            flowLayoutPanels.Add(flowLayoutPanel_開門權限_05);
            flowLayoutPanels.Add(flowLayoutPanel_開門權限_06);
            flowLayoutPanels.Add(flowLayoutPanel_開門權限_07);
            flowLayoutPanels.Add(flowLayoutPanel_開門權限_08);

            for(int i = 0; i < 160; i++)
            {
                OpenDoorPermission_UI openDoorPermission_UI = new OpenDoorPermission_UI();
                flowLayoutPanels[i / 20].Controls.Add(openDoorPermission_UI);
                //openDoorPermission_UI.Index = (i);
                openDoorPermission_UI.Size = new Size(250, 55);
                openDoorPermission_UI.TabIndex = i + 5;
                openDoorPermission_UI.Visible = false;
                this.openDoorPermission_UIs.Add(openDoorPermission_UI);
            }
            this.ResumeLayout(false);
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
                }
            }
        }
    
        private void SqL_DataGridView_人員資料_DataGridRefreshEvent()
        {
            for (int i = 0; i < this.sqL_DataGridView_人員資料.dataGridView.Rows.Count; i++)
            {
                Color color = this.sqL_DataGridView_人員資料.dataGridView.Rows[i].Cells[(int)enum_人員資料.顏色].Value.ObjectToString().ToColor();
                this.sqL_DataGridView_人員資料.dataGridView.Rows[i].Cells[(int)enum_人員資料.顏色].Style.BackColor = color;
                this.sqL_DataGridView_人員資料.dataGridView.Rows[i].Cells[(int)enum_人員資料.顏色].Style.ForeColor = color;
            }
        }
        private void SqL_DataGridView_人員資料_RowDoubleClickEvent(object[] RowValue)
        {
            rJ_TextBox_人員資料_ID.Text = RowValue[(int)enum_人員資料.ID].ObjectToString();
            rJ_TextBox_人員資料_姓名.Text = RowValue[(int)enum_人員資料.姓名].ObjectToString();
            rJ_TextBox_人員資料_密碼.Text = RowValue[(int)enum_人員資料.密碼].ObjectToString();
            rJ_TextBox_人員資料_單位.Text = RowValue[(int)enum_人員資料.單位].ObjectToString();
            comboBox_人員資料_權限等級.Text = RowValue[(int)enum_人員資料.權限等級].ObjectToString();
            rJ_TextBox_人員資料_卡號.Text = RowValue[(int)enum_人員資料.卡號].ObjectToString();
            this.openDoorPermission_UIs.SetOpenDoorPermission(RowValue[(int)enum_人員資料.開門權限].ObjectToString());
            OpenDoorPermission_UI.LoginID = rJ_TextBox_人員資料_ID.Text;
            string 性別 = RowValue[(int)enum_人員資料.性別].ObjectToString();
            if (性別 == "男") rJ_RatioButton_人員資料_男.Checked = true;
            else rJ_RatioButton_人員資料_女.Checked = true;
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
                DialogResult Result = MyMessageBox.ShowDialog("是否刪除選取欄位資料?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel);
                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    List<object[]> list_value = this.sqL_DataGridView_人員資料.Get_All_Select_RowsValues();
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
        private void PlC_RJ_Button_人員資料_開門權限全關_MouseDownEvent(MouseEventArgs mevent)
        {
            string value = "";
            byte[] bytes = new byte[openDoorPermission_UIs.Count / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    bytes[i] |= (byte)(0 << k);
                }
            }
            value = $"{bytes.ByteToStringHex()}";

            this.openDoorPermission_UIs.SetOpenDoorPermission(value);
        }
        private void PlC_RJ_Button_人員資料_開門權限全開_MouseDownEvent(MouseEventArgs mevent)
        {
            string value = "";
            byte[] bytes = new byte[openDoorPermission_UIs.Count / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    bytes[i] |= (byte)(1 << k);
                }
            }
            value = $"{bytes.ByteToStringHex()}";

            this.openDoorPermission_UIs.SetOpenDoorPermission(value);
        }

        private void PlC_RJ_Button_人員資料_顯示全部_MouseDownEvent(MouseEventArgs mevent)
        {
            this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);
        }
        private void PlC_RJ_Button_人員資料_資料查詢_一維條碼_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_人員資料_資料查詢_一維條碼.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("搜尋條件空白!");
                return;
            }
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetRows((int)enum_人員資料.一維條碼, rJ_TextBox_人員資料_資料查詢_一維條碼.Text, false);
            if (list_value.Count == 0)
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
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetRows((int)enum_人員資料.卡號, rJ_TextBox_人員資料_資料查詢_卡號.Text, false);
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
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            list_value = list_value.GetRowsByLike((int)enum_人員資料.姓名, rJ_TextBox_人員資料_資料查詢_姓名.Text);
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
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetRowsByLike((int)enum_人員資料.ID, rJ_TextBox_人員資料_資料查詢_ID.Text, false);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
                return;
            }
            this.sqL_DataGridView_人員資料.RefreshGrid(list_value);
        }
        #endregion
    }
}
