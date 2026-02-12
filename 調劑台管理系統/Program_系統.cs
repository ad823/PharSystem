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
using SQLUI;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using HIS_DB_Lib;

namespace 調劑台管理系統
{
 
    public partial class Main_Form : Form
    {
    
        #region Locker_Index_Table
  
        enum enum_lockerIndex_匯出
        {
            IP,
            Num,
            輸入位置,
            輸出位置,
            同步輸出,
        }
        enum enum_lockerIndex_匯入
        {
            IP,
            Num,
            輸入位置,
            輸出位置,
            同步輸出,
        }
        public enum ContextMenuStrip_Locker_Index_Table
        {
            [Description("M8000")]
            匯出,
            [Description("M8000")]
            匯入,
            [Description("M8000")]
            刷新,
            [Description("M8000")]
            刪除選取資料,
        }
        #endregion
        #region 特殊輸出表
        [EnumDescription("special_output_table")]
        public enum enum_特殊輸出表
        {
            [Description("GUID,VARCHAR,50,PRIMARY")]
            GUID,
            [Description("Name,VARCHAR,100,NONE")]
            Name,
            [Description("IP,VARCHAR,20,NONE")]
            IP,
            [Description("Num,VARCHAR,20,NONE")]
            Num,
            [Description("輸出位置,VARCHAR,20,NONE")]
            輸出位置,
            [Description("輸出狀態,VARCHAR,20,NONE")]
            輸出狀態
        }
        enum enum_特殊輸出表_匯出
        {
            IP,
            Name,
            Num,
            輸出位置,
        }
        enum enum_特殊輸出表_匯入
        {
            IP,
            Name,
            Num,
            輸出位置,
        }
        public enum ContextMenuStrip_特殊輸出表
        {
            [Description("M8000")]
            匯出,
            [Description("M8000")]
            匯入,
            [Description("M8000")]
            刷新,
            [Description("M8000")]
            刪除選取資料,
        }
        #endregion
        public static SQLUI.SQL_DataGridView _sqL_DataGridView_Locker_Index_Table;
        private void Program_系統_Init()
        {
            Table table = new Table("");
            table = new Table(new enum_lockerIndex());
            _sqL_DataGridView_Locker_Index_Table = this.sqL_DataGridView_Locker_Index_Table;
            this.sqL_DataGridView_Locker_Index_Table.Init(table);
            this.sqL_DataGridView_Locker_Index_Table.Set_ColumnVisible(false, enum_lockerIndex.GUID.GetEnumName(), enum_lockerIndex.Master_GUID.GetEnumName(), enum_lockerIndex.Slave_GUID.GetEnumName());
            this.sqL_DataGridView_Locker_Index_Table.Set_ColumnWidth(200, enum_lockerIndex.IP);


            if (!this.sqL_DataGridView_Locker_Index_Table.SQL_IsTableCreat()) this.sqL_DataGridView_Locker_Index_Table.SQL_CreateTable();
            else this.sqL_DataGridView_Locker_Index_Table.SQL_CheckAllColumnName(true);

            this.sqL_DataGridView_Locker_Index_Table.MouseDown += SqL_DataGridView_Locker_Index_Table_MouseDown;
            this.sqL_DataGridView_Locker_Index_Table.DataGridRowsChangeEvent += SqL_DataGridView_Locker_Index_Table_DataGridRowsChangeEvent;

            table = new Table(new enum_特殊輸出表());
            this.sqL_DataGridView_特殊輸出表.Init(table);
            if (!this.sqL_DataGridView_特殊輸出表.SQL_IsTableCreat()) this.sqL_DataGridView_特殊輸出表.SQL_CreateTable();
            this.sqL_DataGridView_特殊輸出表.MouseDown += SqL_DataGridView_特殊輸出表_MouseDown;

        

            this.plC_RJ_Button_檢查頁面不顯示.MouseDownEvent += PlC_RJ_Button_檢查頁面不顯示_MouseDownEvent;
            this.plC_UI_Init.Add_Method(this.sub_Program_系統);
        }



        private bool flag_sub_Program_系統_設定06 = false;
        private bool flag_sub_Program_系統_設定03 = false;

        private void sub_Program_系統()
        {
           
           
            if (plC_ScreenPage_Main.PageText == "系統" && plC_ScreenPage_系統.PageText == "設定03")
            {
                if (!flag_sub_Program_系統_設定03)
                {
                    this.sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(true);
                    this.sqL_DataGridView_特殊輸出表.SQL_GetAllRows(true);
                    flag_sub_Program_系統_設定03 = true;
                }
            }
            else
            {
                flag_sub_Program_系統_設定03 = false;
            }


            if (plC_ScreenPage_Main.PageText == "系統" && plC_ScreenPage_系統.PageText == "設定06")
            {
                if (!flag_sub_Program_系統_設定06)
                {
                    this.sqL_DataGridView_醫令資料.SQL_GetAllRows(true);
                    flag_sub_Program_系統_設定06 = true;
                }
            }
            else
            {
                flag_sub_Program_系統_設定06 = false;
            }
            sub_Program_醫令資料_掃碼();
            sub_Program_檢查頁面不顯示();

        }

        #region PLC_醫令資料_掃碼
        PLC_Device PLC_Device_醫令資料_掃碼 = new PLC_Device("");
        PLC_Device PLC_Device_醫令資料_掃碼_OK = new PLC_Device("");
        int cnt_Program_醫令資料_掃碼 = 65534;
        void sub_Program_醫令資料_掃碼()
        {
            if (plC_ScreenPage_Main.PageText == "系統" && plC_ScreenPage_系統.PageText == "設定06")
            {
                PLC_Device_醫令資料_掃碼.Bool = true;
            }
            else
            {
                PLC_Device_醫令資料_掃碼.Bool = false;
            }

            if (cnt_Program_醫令資料_掃碼 == 65534)
            {
                PLC_Device_醫令資料_掃碼.SetComment("PLC_醫令資料_掃碼");
                PLC_Device_醫令資料_掃碼_OK.SetComment("PLC_醫令資料_掃碼_OK");
                PLC_Device_醫令資料_掃碼.Bool = false;
                cnt_Program_醫令資料_掃碼 = 65535;
            }
            if (cnt_Program_醫令資料_掃碼 == 65535) cnt_Program_醫令資料_掃碼 = 1;
            if (cnt_Program_醫令資料_掃碼 == 1) cnt_Program_醫令資料_掃碼_檢查按下(ref cnt_Program_醫令資料_掃碼);
            if (cnt_Program_醫令資料_掃碼 == 2) cnt_Program_醫令資料_掃碼_初始化(ref cnt_Program_醫令資料_掃碼);
            if (cnt_Program_醫令資料_掃碼 == 3) cnt_Program_醫令資料_掃碼 = 65500;
            if (cnt_Program_醫令資料_掃碼 > 1) cnt_Program_醫令資料_掃碼_檢查放開(ref cnt_Program_醫令資料_掃碼);

            if (cnt_Program_醫令資料_掃碼 == 65500)
            {
                PLC_Device_醫令資料_掃碼.Bool = false;
                PLC_Device_醫令資料_掃碼_OK.Bool = false;
                cnt_Program_醫令資料_掃碼 = 65535;
            }
        }
        void cnt_Program_醫令資料_掃碼_檢查按下(ref int cnt)
        {
            if (PLC_Device_醫令資料_掃碼.Bool) cnt++;
        }
        void cnt_Program_醫令資料_掃碼_檢查放開(ref int cnt)
        {
            if (!PLC_Device_醫令資料_掃碼.Bool) cnt = 65500;
        }
        void cnt_Program_醫令資料_掃碼_初始化(ref int cnt)
        {
          
            cnt++;
        }



























































        #endregion
        #region PLC_檢查頁面不顯示
        PLC_Device PLC_Device_檢查頁面不顯示 = new PLC_Device("");
        PLC_Device PLC_Device_檢查頁面不顯示_OK = new PLC_Device("");
        MyTimer MyTimer_檢查頁面不顯示_結束延遲 = new MyTimer();
        int cnt_Program_檢查頁面不顯示 = 65534;
        bool flag_檢查頁面不顯示_init = false;
        void sub_Program_檢查頁面不顯示()
        {
            PLC_Device_檢查頁面不顯示.Bool = true;
            if (ControlMode) return;
            if (cnt_Program_檢查頁面不顯示 == 65534)
            {
                this.MyTimer_檢查頁面不顯示_結束延遲.StartTickTime(2000);
                PLC_Device_檢查頁面不顯示.SetComment("PLC_檢查頁面不顯示");
                PLC_Device_檢查頁面不顯示_OK.SetComment("PLC_檢查頁面不顯示_OK");
                PLC_Device_檢查頁面不顯示.Bool = false;
                cnt_Program_檢查頁面不顯示 = 65535;
            }
            if (cnt_Program_檢查頁面不顯示 == 65535) cnt_Program_檢查頁面不顯示 = 1;
            if (cnt_Program_檢查頁面不顯示 == 1) cnt_Program_檢查頁面不顯示_檢查按下(ref cnt_Program_檢查頁面不顯示);
            if (cnt_Program_檢查頁面不顯示 == 2) cnt_Program_檢查頁面不顯示_初始化(ref cnt_Program_檢查頁面不顯示);
            if (cnt_Program_檢查頁面不顯示 == 3) cnt_Program_檢查頁面不顯示 = 65500;
            if (cnt_Program_檢查頁面不顯示 > 1) cnt_Program_檢查頁面不顯示_檢查放開(ref cnt_Program_檢查頁面不顯示);

            if (cnt_Program_檢查頁面不顯示 == 65500)
            {
                this.MyTimer_檢查頁面不顯示_結束延遲.TickStop();
                this.MyTimer_檢查頁面不顯示_結束延遲.StartTickTime(2000);
                PLC_Device_檢查頁面不顯示.Bool = false;
                PLC_Device_檢查頁面不顯示_OK.Bool = false;
                cnt_Program_檢查頁面不顯示 = 65535;
            }
        }
        void cnt_Program_檢查頁面不顯示_檢查按下(ref int cnt)
        {
            if (PLC_Device_檢查頁面不顯示.Bool) cnt++;
        }
        void cnt_Program_檢查頁面不顯示_檢查放開(ref int cnt)
        {
            if (!PLC_Device_檢查頁面不顯示.Bool) cnt = 65500;
        }
        void cnt_Program_檢查頁面不顯示_初始化(ref int cnt)
        {
            if (this.MyTimer_檢查頁面不顯示_結束延遲.IsTimeOut())
            {
                if (Task_Method == null)
                {
                    Task_Method = new Task(new Action(delegate { PlC_RJ_Button_檢查頁面不顯示_MouseDownEvent(null); }));
                }
                if (Task_Method.Status == TaskStatus.RanToCompletion)
                {
                    Task_Method = new Task(new Action(delegate { PlC_RJ_Button_檢查頁面不顯示_MouseDownEvent(null); }));
                }
                if (Task_Method.Status == TaskStatus.Created)
                {
                    Task_Method.Start();
                }
                cnt++;
            }
        }



















        #endregion
        #region Event
      
        private void SqL_DataGridView_特殊輸出表_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) return;
            Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_特殊輸出表());
            if (dialog_ContextMenuStrip.ShowDialog() != DialogResult.Yes) return;
            if (dialog_ContextMenuStrip.Value == ContextMenuStrip_特殊輸出表.匯出.GetEnumName())
            {
                saveFileDialog_SaveExcel.OverwritePrompt = false;
                if (saveFileDialog_SaveExcel.ShowDialog(this) == DialogResult.OK)
                {
                    DataTable datatable = new DataTable();
                    datatable = sqL_DataGridView_特殊輸出表.GetDataTable();
                    datatable = datatable.ReorderTable(new enum_特殊輸出表_匯出());
                    CSVHelper.SaveFile(datatable, saveFileDialog_SaveExcel.FileName);
                    MyMessageBox.ShowDialog("匯出完成!");
                }
            }
            else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_特殊輸出表.匯入.GetEnumName())
            {
                if (openFileDialog_LoadExcel.ShowDialog(this) == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    DataTable dataTable = new DataTable();
                    CSVHelper.LoadFile(this.openFileDialog_LoadExcel.FileName, 0, dataTable);
                    DataTable datatable_buf = dataTable.ReorderTable(new enum_特殊輸出表_匯入());
                    if (datatable_buf == null)
                    {
                        MyMessageBox.ShowDialog("匯入檔案,資料錯誤!");
                        return;
                    }
                    List<object[]> list_LoadValue = datatable_buf.DataTableToRowList();
                    List<object[]> list_SQL_Value = this.sqL_DataGridView_特殊輸出表.SQL_GetAllRows(false);
                    List<object[]> list_Add = new List<object[]>();
                    List<object[]> list_Delete_ColumnName = new List<object[]>();
                    List<object[]> list_Delete_SerchValue = new List<object[]>();
                    List<string> list_Replace_SerchValue = new List<string>();
                    List<object[]> list_Replace_Value = new List<object[]>();
                    List<object[]> list_SQL_Value_buf = new List<object[]>();

                    for (int i = 0; i < list_LoadValue.Count; i++)
                    {
                        object[] value_load = list_LoadValue[i];
                        value_load = value_load.CopyRow(new enum_特殊輸出表_匯入(), new enum_特殊輸出表());
                        list_SQL_Value_buf = list_SQL_Value.GetRows((int)enum_特殊輸出表.IP, value_load[(int)enum_特殊輸出表.IP].ObjectToString());
                        value_load[(int)enum_特殊輸出表.輸出狀態] = false.ToString();
                        if (list_SQL_Value_buf.Count > 0)
                        {
                            object[] value_SQL = list_SQL_Value_buf[0];
                            bool flag_Equal = value_load.IsEqual(value_SQL);
                            if (!flag_Equal)
                            {
                                list_Replace_Value.Add(value_load);
                            }
                        }
                        else
                        {
                            value_load[(int)enum_特殊輸出表.GUID] = Guid.NewGuid().ToString();
                            list_Add.Add(value_load);
                        }
                    }
                    this.sqL_DataGridView_特殊輸出表.SQL_AddRows(list_Add, false);
                    this.sqL_DataGridView_特殊輸出表.SQL_ReplaceExtra(list_Replace_Value, false);
                    this.sqL_DataGridView_特殊輸出表.SQL_GetAllRows(true);
                    this.Cursor = Cursors.Default;
                }
                this.Cursor = Cursors.Default;
                MyMessageBox.ShowDialog("匯入完成!");
            }
            else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_特殊輸出表.刪除選取資料.GetEnumName())
            {
                if (MyMessageBox.ShowDialog("是否刪除選取資料", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;
                    List<object[]> list_value = this.sqL_DataGridView_特殊輸出表.Get_All_Select_RowsValues();
                    List<object> list_delete_serchValue = new List<object>();
                    for (int i = 0; i < list_value.Count; i++)
                    {
                        string GUID = list_value[i][(int)enum_藥品資料_藥檔資料.GUID].ObjectToString();
                        list_delete_serchValue.Add(GUID);
                    }
                    this.sqL_DataGridView_特殊輸出表.SQL_DeleteExtra(list_delete_serchValue, true);
                    this.Cursor = Cursors.Default;
                }
            }
            else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_特殊輸出表.刷新.GetEnumName())
            {
                this.sqL_DataGridView_特殊輸出表.SQL_GetAllRows(true);
            }
        }
        private void SqL_DataGridView_Locker_Index_Table_DataGridRowsChangeEvent(List<object[]> RowsList)
        {
            RowsList.Sort(new ICP_Locker_Index_Table());
        }
        private void SqL_DataGridView_Locker_Index_Table_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) return;
            Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_Locker_Index_Table());
            if (dialog_ContextMenuStrip.ShowDialog() != DialogResult.Yes) return;
            if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Locker_Index_Table.匯出.GetEnumName())
            {
                saveFileDialog_SaveExcel.OverwritePrompt = false;
                if (saveFileDialog_SaveExcel.ShowDialog(this) == DialogResult.OK)
                {
                    DataTable datatable = new DataTable();
                    datatable = sqL_DataGridView_Locker_Index_Table.GetDataTable();
                    datatable = datatable.ReorderTable(new enum_lockerIndex_匯出());
                    CSVHelper.SaveFile(datatable, saveFileDialog_SaveExcel.FileName);
                    MyMessageBox.ShowDialog("匯出完成!");
                }
            }
            else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Locker_Index_Table.匯入.GetEnumName())
            {
                if (openFileDialog_LoadExcel.ShowDialog(this) == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    DataTable dataTable = new DataTable();
                    CSVHelper.LoadFile(this.openFileDialog_LoadExcel.FileName, 0, dataTable);
                    DataTable datatable_buf = dataTable.ReorderTable(new enum_lockerIndex_匯入());
                    if (datatable_buf == null)
                    {
                        MyMessageBox.ShowDialog("匯入檔案,資料錯誤!");
                        return;
                    }
                    List<object[]> list_LoadValue = datatable_buf.DataTableToRowList();
                    List<object[]> list_SQL_Value = this.sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(false);
                    List<object[]> list_Add = new List<object[]>();
                    List<object[]> list_Delete_ColumnName = new List<object[]>();
                    List<object[]> list_Delete_SerchValue = new List<object[]>();
                    List<string> list_Replace_SerchValue = new List<string>();
                    List<object[]> list_Replace_Value = new List<object[]>();
                    List<object[]> list_SQL_Value_buf = new List<object[]>();

                    for (int i = 0; i < list_LoadValue.Count; i++)
                    {
                        object[] value_load = list_LoadValue[i];
                        value_load = value_load.CopyRow(new enum_lockerIndex_匯入(), new enum_lockerIndex());
                        list_SQL_Value_buf = list_SQL_Value.GetRows((int)enum_lockerIndex.IP, value_load[(int)enum_lockerIndex.IP].ObjectToString());
                        value_load[(int)enum_lockerIndex.輸出狀態] = false.ToString();
                        if (list_SQL_Value_buf.Count > 0)
                        {
                            object[] value_SQL = list_SQL_Value_buf[0];
                            bool flag_Equal = value_load.IsEqual(value_SQL);
                            if (!flag_Equal)
                            {
                                list_Replace_Value.Add(value_load);
                            }
                        }
                        else
                        {
                            value_load[(int)enum_lockerIndex.GUID] = Guid.NewGuid().ToString();
                            list_Add.Add(value_load);
                        }
                    }
                    this.sqL_DataGridView_Locker_Index_Table.SQL_AddRows(list_Add, false);
                    this.sqL_DataGridView_Locker_Index_Table.SQL_ReplaceExtra(list_Replace_Value, false);
                    this.sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(true);
                    this.Cursor = Cursors.Default;
                }
                this.Cursor = Cursors.Default;
                MyMessageBox.ShowDialog("匯入完成!");
            }
            else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Locker_Index_Table.刪除選取資料.GetEnumName())
            {
                if (MyMessageBox.ShowDialog("是否刪除選取資料", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;
                    List<object[]> list_value = this.sqL_DataGridView_Locker_Index_Table.Get_All_Select_RowsValues();
                    List<object> list_delete_serchValue = new List<object>();
                    for (int i = 0; i < list_value.Count; i++)
                    {
                        string GUID = list_value[i][(int)enum_特殊輸出表.GUID].ObjectToString();
                        list_delete_serchValue.Add(GUID);
                    }
                    this.sqL_DataGridView_Locker_Index_Table.SQL_DeleteExtra(list_delete_serchValue, true);
                    this.Cursor = Cursors.Default;
                }
            }
            else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Locker_Index_Table.刷新.GetEnumName())
            {
                this.sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(true);
            }
        }
   
        private void PlC_RJ_Button_檢查頁面不顯示_MouseDownEvent(MouseEventArgs mevent)
        {
       
        
            plC_RJ_ScreenButton_調劑作業.SetVisible(!plC_CheckBox_不顯示設定_調劑作業.Checked);
            plC_RJ_ScreenButton_管制抽屜.SetVisible(!plC_CheckBox_不顯示設定_管制抽屜.Checked);
            plC_RJ_ScreenButton_交班作業.SetVisible(!plC_CheckBox_不顯示設定_交班作業.Checked);
            plC_RJ_Button_手輸醫令.SetVisible(!plC_CheckBox_調劑功能_手輸醫令.Checked);
            plC_RJ_Button_醫令檢索.SetVisible(!plC_CheckBox_調劑功能_醫令檢索.Checked);
            plC_RJ_Button_藥單條碼輸入.SetVisible(!plC_CheckBox_調劑功能_藥單條碼輸入.Checked);
            plC_RJ_Button_全部滅燈.SetVisible(!plC_CheckBox_調劑功能_全部滅燈.Checked);
            plC_RJ_Button_藥品搜索.SetVisible(!plC_CheckBox_調劑功能_藥品搜索.Checked);
            plC_RJ_Button_藥品調入.SetVisible(!plC_CheckBox_調劑功能_藥品調入.Checked);
            plC_RJ_Button_交班對點.SetVisible(!plC_CheckBox_調劑功能_交班對點.Checked);
            plC_RJ_Button_申領.SetVisible(!plC_CheckBox_調劑功能_申領.Checked);
            plC_RJ_Button_藥品搜索.SetVisible(!plC_CheckBox_調劑功能_藥品搜索.Checked);
            plC_RJ_Button_藥品搜索.SetVisible(!plC_CheckBox_調劑功能_藥品搜索.Checked);

            if (flag_檢查頁面不顯示_init == false)
            {
                this.Invoke(new Action(delegate 
                {
                    plC_RJ_Button_手輸醫令.Visible = true;
                    plC_RJ_Button_醫令檢索.Visible = false;
                    plC_RJ_Button_藥單條碼輸入.Visible = false;
                    plC_RJ_Button_全部滅燈.Visible = false;
                    plC_RJ_Button_藥品搜索.Visible = false;
                    plC_RJ_Button_藥品調入.Visible = false;
                    plC_RJ_Button_交班對點.Visible = false;
                    plC_RJ_Button_申領.Visible = false;
                    plC_RJ_Button_藥品搜索.Visible = false;
                    plC_RJ_Button_藥品搜索.Visible = false;
                }));

                flag_檢查頁面不顯示_init = true;
            }

            if (plC_CheckBox_不顯示設定_收支作業.Checked)
            {
                plC_RJ_ScreenButton_收支作業.SetVisible(false);
            }
            if (plC_CheckBox_不顯示設定_交易紀錄查詢.Checked)
            {
                plC_RJ_ScreenButton_交易紀錄查詢.SetVisible(false);
            }
            if (plC_CheckBox_不顯示設定_醫令資料.Checked)
            {
                plC_RJ_ScreenButton_醫令資料.SetVisible(false);
            }
            if (plC_CheckBox_不顯示設定_藥品資料.Checked)
            {
                plC_RJ_ScreenButton_藥品資料.SetVisible(false);
            }
            if (plC_CheckBox_不顯示設定_人員資料.Checked)
            {
                plC_RJ_ScreenButton_人員資料.SetVisible(false);
            }
            if (plC_CheckBox_不顯示設定_儲位管理.Checked)
            {
                plC_RJ_ScreenButton_儲位管理.SetVisible(false);
            }
            if (plC_CheckBox_不顯示設定_儲位管理.Checked)
            {
                plC_RJ_ScreenButton_儲位管理.SetVisible(false);
            }
            if (plC_CheckBox_不顯示設定_盤點作業.Checked)
            {
                plC_RJ_ScreenButton_盤點作業.SetVisible(false);
            }
            if (plC_CheckBox_不顯示設定_工程模式.Checked)
            {
                plC_RJ_ScreenButton_工程模式.SetVisible(false);
            }
        }
        #endregion

        private class ICP_Locker_Index_Table : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string IP_0 = x[(int)enum_lockerIndex.IP].ObjectToString();
                string IP_1 = y[(int)enum_lockerIndex.IP].ObjectToString();
                string Num_0 = x[(int)enum_lockerIndex.Num].ObjectToString();
                string Num_1 = y[(int)enum_lockerIndex.Num].ObjectToString();
                string[] IP_0_Array = IP_0.Split('.');
                string[] IP_1_Array = IP_1.Split('.');
                if (!IP_0.Check_IP_Adress()) return 0;
                if (!IP_1.Check_IP_Adress()) return 0;
                for (int i = 0; i < 4; i++)
                {
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];

                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];

                    IP_0 += IP_0_Array[i];
                    IP_1 += IP_1_Array[i];
                }
                int cmp = IP_0_Array[2].CompareTo(IP_1_Array[2]);
                if (cmp > 0)
                {
                    return 1;
                }
                else if (cmp < 0)
                {
                    return -1;
                }
                else if (cmp == 0)
                {
                    cmp = IP_0_Array[3].CompareTo(IP_1_Array[3]);
                    if (cmp > 0)
                    {
                        return 1;
                    }
                    else if (cmp < 0)
                    {
                        return -1;
                    }
                    else if (cmp == 0)
                    {
                        return Num_0.CompareTo(Num_1);
                    }
                }

                return 0;

            }
        }
    }
}
