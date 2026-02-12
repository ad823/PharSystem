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
using MyUI;
using Basic;
using SQLUI;
using H_Pannel_lib;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using HIS_DB_Lib;
namespace 勤務傳送系統
{
    public partial class Main_Form : Form
    {
        #region Box_Index_Table
        enum enum_Box_Index_Table
        {
            GUID,
            Number,
            IP,
            RFID_num,
            Lock_output_num,
            Lock_input_num,
            Sensor_input_num,
            Led_output_num,
            EPD_IP,
        }
        enum enum_Box_Index_Table_匯出
        {
            Number,
            IP,
            RFID_num,
            Lock_output_num,
            Lock_input_num,
            Sensor_input_num,
            Led_output_num,
            EPD_IP,
        }
        enum enum_Box_Index_Table_匯入
        {
            Number,
            IP,
            RFID_num,
            Lock_output_num,
            Lock_input_num,
            Sensor_input_num,
            Led_output_num,
            EPD_IP,
        }

        #endregion

        private void Program_系統頁面_Init()
        {
            Table table = new Table("box_index_table");
            table.AddColumnList("GUID", Table.StringType.VARCHAR, 50, Table.IndexType.PRIMARY);
            table.AddColumnList("Number", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("IP", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("RFID_num", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("Lock_output_num", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("Lock_input_num", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("Sensor_input_num", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("Led_output_num", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("EPD_IP", Table.StringType.VARCHAR, 50, Table.IndexType.None);


            this.sqL_DataGridView_Box_Index_Table.Init(table);
            if (!this.sqL_DataGridView_Box_Index_Table.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_Box_Index_Table.SQL_CreateTable();
            }
            else
            {
                this.sqL_DataGridView_Box_Index_Table.SQL_CheckAllColumnName(true);
            }
            this.sqL_DataGridView_Box_Index_Table.DataGridRowsChangeEvent += SqL_DataGridView_Box_Index_Table_DataGridRowsChangeEvent;

            this.plC_RJ_Button_Box_Index_Table_匯出.MouseDownEvent += PlC_RJ_Button_Box_Index_Table_匯出_MouseDownEvent;
            this.plC_RJ_Button_Box_Index_Table_匯入.MouseDownEvent += PlC_RJ_Button_Box_Index_Table_匯入_MouseDownEvent;
            this.plC_RJ_Button_Box_Index_Table_刪除.MouseDownEvent += PlC_RJ_Button_Box_Index_Table_刪除_MouseDownEvent;
            this.plC_RJ_Button_Box_Index_Table_更新.MouseDownEvent += PlC_RJ_Button_Box_Index_Table_更新_MouseDownEvent;

            this.plC_RJ_Button_檢查病房有藥未過帳.MouseDownEvent += PlC_RJ_Button_檢查病房有藥未過帳_MouseDownEvent;

            this.plC_UI_Init.Add_Method(Program_系統頁面);
        }

        private void Program_系統頁面()
        {
            Pannel_Box.input_time = plC_NumBox_配藥完成藥籃亮燈延遲時間.Value;
            if (plC_NumBox_病房提示亮燈.Value < 5000) plC_NumBox_病房提示亮燈.Value = 5000;
            Pannel_Box.PharLightOnTime = plC_NumBox_病房提示亮燈.Value;
            if(plC_CheckBox_主機模式.Checked)
            {
                sub_Program_檢查病房有藥未過帳();
            }
            Pannel_Box.flag_Run = this.plC_CheckBox_主機模式.Checked;


        }

        #region PLC_檢查病房有藥未過帳
        PLC_Device PLC_Device_檢查病房有藥未過帳 = new PLC_Device("");
        PLC_Device PLC_Device_檢查病房有藥未過帳_OK = new PLC_Device("");
        Task Task_檢查病房有藥未過帳;
        MyTimer MyTimer_檢查病房有藥未過帳_結束延遲 = new MyTimer();
        int cnt_Program_檢查病房有藥未過帳 = 65534;
        void sub_Program_檢查病房有藥未過帳()
        {
            if (plC_CheckBox_主機模式.Checked) PLC_Device_檢查病房有藥未過帳.Bool = true;
            if (cnt_Program_檢查病房有藥未過帳 == 65534)
            {
                this.MyTimer_檢查病房有藥未過帳_結束延遲.StartTickTime(1000);
                PLC_Device_檢查病房有藥未過帳.SetComment("PLC_檢查病房有藥未過帳");
                PLC_Device_檢查病房有藥未過帳_OK.SetComment("PLC_檢查病房有藥未過帳_OK");
                PLC_Device_檢查病房有藥未過帳.Bool = false;
                cnt_Program_檢查病房有藥未過帳 = 65535;
            }
            if (cnt_Program_檢查病房有藥未過帳 == 65535) cnt_Program_檢查病房有藥未過帳 = 1;
            if (cnt_Program_檢查病房有藥未過帳 == 1) cnt_Program_檢查病房有藥未過帳_檢查按下(ref cnt_Program_檢查病房有藥未過帳);
            if (cnt_Program_檢查病房有藥未過帳 == 2) cnt_Program_檢查病房有藥未過帳_初始化(ref cnt_Program_檢查病房有藥未過帳);
            if (cnt_Program_檢查病房有藥未過帳 == 3) cnt_Program_檢查病房有藥未過帳 = 65500;
            if (cnt_Program_檢查病房有藥未過帳 > 1) cnt_Program_檢查病房有藥未過帳_檢查放開(ref cnt_Program_檢查病房有藥未過帳);

            if (cnt_Program_檢查病房有藥未過帳 == 65500)
            {
                this.MyTimer_檢查病房有藥未過帳_結束延遲.TickStop();
                this.MyTimer_檢查病房有藥未過帳_結束延遲.StartTickTime(1000);
                PLC_Device_檢查病房有藥未過帳.Bool = false;
                PLC_Device_檢查病房有藥未過帳_OK.Bool = false;
                cnt_Program_檢查病房有藥未過帳 = 65535;
            }
        }
        void cnt_Program_檢查病房有藥未過帳_檢查按下(ref int cnt)
        {
            if (PLC_Device_檢查病房有藥未過帳.Bool) cnt++;
        }
        void cnt_Program_檢查病房有藥未過帳_檢查放開(ref int cnt)
        {
            if (!PLC_Device_檢查病房有藥未過帳.Bool) cnt = 65500;
        }
        void cnt_Program_檢查病房有藥未過帳_初始化(ref int cnt)
        {
            if (this.MyTimer_檢查病房有藥未過帳_結束延遲.IsTimeOut())
            {
                if (Task_檢查病房有藥未過帳 == null)
                {
                    Task_檢查病房有藥未過帳 = new Task(new Action(delegate { PlC_RJ_Button_檢查病房有藥未過帳_MouseDownEvent(null); }));
                }
                if (Task_檢查病房有藥未過帳.Status == TaskStatus.RanToCompletion)
                {
                    Task_檢查病房有藥未過帳 = new Task(new Action(delegate { PlC_RJ_Button_檢查病房有藥未過帳_MouseDownEvent(null); }));
                }
                if (Task_檢查病房有藥未過帳.Status == TaskStatus.Created)
                {
                    Task_檢查病房有藥未過帳.Start();
                }
                cnt++;
            }
        }







        #endregion
        #region Event
        private void PlC_RJ_Button_檢查病房有藥未過帳_MouseDownEvent(MouseEventArgs mevent)
        {
            DateTime dt_st = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 00, 00, 00);
            DateTime dt_end = new DateTime(DateTime.Now.AddDays(0).Year, DateTime.Now.AddDays(0).Month, DateTime.Now.AddDays(0).Day, 23, 59, 59);
            List<object[]> list_交易記錄 = this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByBetween((int)enum_交易記錄查詢資料.操作時間, dt_st, dt_end, false);
            List<object[]> list_交易記錄_buf = (from temp in list_交易記錄
                                             where temp[(int)enum_交易記錄查詢資料.動作].ObjectToString() == enum_交易記錄查詢動作.藥袋刷入.GetEnumName()
                                             where temp[(int)enum_交易記錄查詢資料.領用時間].ToDateTimeString().StringToDateTime() == "1999-01-01 00:00:00".StringToDateTime()
                                             select temp).ToList();

            List<string> wardName_一般藥品 = (from temp in list_交易記錄_buf
                                          select temp[(int)enum_交易記錄查詢資料.病房號].ObjectToString()).Distinct().ToList();
            Pannel_Box.PanelLightOnCheck(wardName_一般藥品);

            List<object[]> list_交易記錄_高價藥品 = new List<object[]>();
            List<object[]> list_高價藥品 = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetRows((int)enum_藥品資料_藥檔資料.高價藥品, "True", false);
            List<object[]> list_高價藥品_buf = new List<object[]>();
            for(int i = 0; i < list_交易記錄_buf.Count; i++)
            {
                string 藥品碼 = list_交易記錄_buf[i][(int)enum_交易記錄查詢資料.藥品碼].ObjectToString();
                list_高價藥品_buf = list_高價藥品.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, 藥品碼);
                if(list_高價藥品_buf.Count > 0)
                {
                    list_交易記錄_高價藥品.Add(list_交易記錄_buf[i]);
                }
            }
            List<string> wardName_高價藥品 = (from temp in list_交易記錄_高價藥品
                                          select temp[(int)enum_交易記錄查詢資料.病房號].ObjectToString()).Distinct().ToList();
            Pannel_Box.H_COST_LightOnCheck(wardName_高價藥品);
        }
        private void SqL_DataGridView_Box_Index_Table_DataGridRowsChangeEvent(List<object[]> RowsList)
        {
            RowsList.Sort(new ICP_Box_Index_Table());
        }
        private void PlC_RJ_Button_Box_Index_Table_匯入_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (openFileDialog_LoadExcel.ShowDialog(this) == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    DataTable dataTable = new DataTable();
                    CSVHelper.LoadFile(this.openFileDialog_LoadExcel.FileName, 0, dataTable);
                    DataTable datatable_buf = dataTable.ReorderTable(new enum_Box_Index_Table_匯入());
                    if (datatable_buf == null)
                    {
                        MyMessageBox.ShowDialog("匯入檔案,資料錯誤!");
                        return;
                    }
                    List<object[]> list_LoadValue = datatable_buf.DataTableToRowList();
                    List<object[]> list_SQL_Value = this.sqL_DataGridView_Box_Index_Table.SQL_GetAllRows(false);
                    List<object[]> list_Add = new List<object[]>();
                    List<object[]> list_Delete_ColumnName = new List<object[]>();
                    List<object[]> list_Delete_SerchValue = new List<object[]>();
                    List<string> list_Replace_SerchValue = new List<string>();
                    List<object[]> list_Replace_Value = new List<object[]>();
                    List<object[]> list_SQL_Value_buf = new List<object[]>();

                    Dialog_Prcessbar dialog_Prcessbar = new Dialog_Prcessbar(list_LoadValue.Count);
                    dialog_Prcessbar.State = "開始匯入資料...";
                    for (int i = 0; i < list_LoadValue.Count; i++)
                    {
                        dialog_Prcessbar.Value = i;
                        object[] value_load = list_LoadValue[i];
                        value_load = value_load.CopyRow(new enum_Box_Index_Table_匯入(), new enum_Box_Index_Table());
                        list_SQL_Value_buf = list_SQL_Value.GetRows((int)enum_Box_Index_Table.IP, value_load[(int)enum_Box_Index_Table.IP].ObjectToString());
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
                            value_load[(int)enum_Box_Index_Table.GUID] = Guid.NewGuid().ToString();
                            list_Add.Add(value_load);
                        }
                    }
                    dialog_Prcessbar.State = "上傳資料...";
                    this.sqL_DataGridView_Box_Index_Table.SQL_AddRows(list_Add, false);
                    this.sqL_DataGridView_Box_Index_Table.SQL_ReplaceExtra(list_Replace_Value, false);
                    this.sqL_DataGridView_Box_Index_Table.SQL_GetAllRows(true);
                    this.Cursor = Cursors.Default;
                    dialog_Prcessbar.Close();
                }
            
            }));
         
        }
        private void PlC_RJ_Button_Box_Index_Table_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                saveFileDialog_SaveExcel.OverwritePrompt = false;
                if (saveFileDialog_SaveExcel.ShowDialog(this) == DialogResult.OK)
                {
                    DataTable datatable = new DataTable();
                    datatable = sqL_DataGridView_Box_Index_Table.GetDataTable();
                    datatable = datatable.ReorderTable(new enum_Box_Index_Table_匯出());
                    CSVHelper.SaveFile(datatable, saveFileDialog_SaveExcel.FileName);
                    MyMessageBox.ShowDialog("匯出完成!");
                }
            }));

        }
        private void PlC_RJ_Button_Box_Index_Table_更新_MouseDownEvent(MouseEventArgs mevent)
        {
            this.sqL_DataGridView_Box_Index_Table.SQL_GetAllRows(true);
        }

        private void PlC_RJ_Button_Box_Index_Table_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_Box_Index_Table.Get_All_Select_RowsValues();
            this.sqL_DataGridView_Box_Index_Table.SQL_DeleteExtra(list_value, true);
        }
        #endregion
        private class ICP_Box_Index_Table : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {

                string Num_0 = x[(int)enum_Box_Index_Table.Number].ObjectToString();
                string Num_1 = y[(int)enum_Box_Index_Table.Number].ObjectToString();
                return Num_0.CompareTo(Num_1);

            }
        }
    }
}
