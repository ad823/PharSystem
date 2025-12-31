using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using MyUI;
using Basic;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using H_Pannel_lib;
using HIS_DB_Lib;
using SQLUI;

namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
        //PLC_Device PLC_Device_異常通知_盤點錯誤 = new PLC_Device("S800");
        private void Program_異常通知_盤點錯誤_Init()
        {

            Table table = medRecheckLogClass.init(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType);
            this.sqL_DataGridView_異常通知_盤點錯誤.InitEx(table);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnVisible(false, new enum_medRecheckLog().GetEnumNames());

            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_medRecheckLog.發生類別);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_medRecheckLog.藥碼);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(400, DataGridViewContentAlignment.MiddleLeft, enum_medRecheckLog.藥名);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_medRecheckLog.盤點藥師1);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_medRecheckLog.盤點藥師2);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_medRecheckLog.排除藥師);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleRight, enum_medRecheckLog.庫存值);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleRight, enum_medRecheckLog.差異值);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleRight, enum_medRecheckLog.盤點值);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_medRecheckLog.排除時間);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_medRecheckLog.發生時間);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_medRecheckLog.異常原因);
            this.sqL_DataGridView_異常通知_盤點錯誤.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_medRecheckLog.狀態);

            this.plC_RJ_Button_異常通知_盤點錯誤_未排除顯示.MouseDownEvent += PlC_RJ_Button_異常通知_盤點錯誤_未排除顯示_MouseDownEvent;
            this.plC_RJ_Button_異常通知_盤點錯誤_選取藥品異常排除.MouseDownEvent += PlC_RJ_Button_異常通知_盤點錯誤_選取藥品異常排除_MouseDownEvent;
            this.plC_RJ_Button_異常通知_盤點錯誤_發生時段搜尋.MouseDownEvent += PlC_RJ_Button_異常通知_盤點錯誤_發生時段搜尋_MouseDownEvent;
            this.plC_RJ_Button_異常通知_盤點錯誤_排除時段搜尋.MouseDownEvent += PlC_RJ_Button_異常通知_盤點錯誤_排除時段搜尋_MouseDownEvent;

            rJ_DatePicker_異常通知_盤點錯誤_排除時段_起始.Value = rJ_DatePicker_異常通知_盤點錯誤_排除時段_起始.Value.AddMonths(-1);
            rJ_DatePicker_異常通知_盤點錯誤_發生時段_起始.Value = rJ_DatePicker_異常通知_盤點錯誤_發生時段_起始.Value.AddMonths(-1);
        }

        private void PlC_RJ_Button_異常通知_盤點錯誤_選取藥品異常排除_MouseDownEvent(MouseEventArgs mevent)
        {
            LoadingForm.ShowLoadingForm();
            try
            {
                string msg = "";

                List<object[]> list_value = this.sqL_DataGridView_異常通知_盤點錯誤.Get_All_Select_RowsValues();
                if (list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選取資料");
                    return;
                }
                if (PLC_Device_盤點異常排除不改動庫存.Bool == false)
                {
                    string 藥碼 = list_value[0][(int)enum_medRecheckLog.藥碼].ObjectToString();
                    string 藥名 = list_value[0][(int)enum_medRecheckLog.藥名].ObjectToString();
                    string 庫存 = Function_從SQL取得庫存(藥碼).ToString();
                    double 差異值 = medRecheckLogClass.get_unresolved_qty_by_code(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, 藥碼);
                    Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel("請輸入現在庫存值", 藥名);
                    if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
                    double 現在庫存值 = dialog_NumPannel.Value;
                    差異值 = 現在庫存值 - 庫存.StringToDouble();

                    msg = $"藥碼:{藥碼}\n" +
                        $"藥名:{藥名}\n" +
                        $"庫存:{庫存} , 差異值:{差異值}\n" +
                        $"是否排除盤點異常?";
                    if (MyMessageBox.ShowDialog($"{msg}", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
                    medRecheckLogClass.set_unresolved_data_by_code(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, 藥碼, Main_Form._登入者名稱);
                    Function_異常通知_盤點錯誤_庫存異動(藥碼, 藥名, 差異值, Main_Form._登入者名稱);
                    this.sqL_DataGridView_異常通知_盤點錯誤.ClearGrid();
                    uC_調劑作業_TypeA_1.Logout();
                    uC_調劑作業_TypeA_2.Logout();
                    uC_調劑作業_TypeA_3.Logout();
                    uC_調劑作業_TypeA_4.Logout();

                    MyMessageBox.ShowDialog("完成");
                    return;

                }
                if(PLC_Device_盤點異常排除要修正原因.Bool = true)
                {
                    medRecheckLogClass medRecheckLog = list_value[0].SQLToClass<medRecheckLogClass, enum_medRecheckLog>();
                    if (medRecheckLog.交易紀錄_GUID.StringIsEmpty() == false)
                    {
                        transactionsClass transactions = transactionsClass.get_by_guid(API_Server, medRecheckLog.交易紀錄_GUID, ServerName, ServerType);
                        Dialog_收支原因選擇 dialog_收支原因選擇 = new Dialog_收支原因選擇();
                        if (dialog_收支原因選擇.ShowDialog() != DialogResult.Yes) return;

                        if (dialog_收支原因選擇.Value.StringIsEmpty()) dialog_收支原因選擇.Value = "無";
                        transactions.收支原因 = $"{transactions.收支原因}({dialog_收支原因選擇.Value})";
                        transactionsClass.update_by_guid(API_Server, transactions, ServerName, ServerType);


                        medRecheckLogClass.set_unresolved_data_by_guid(API_Server, medRecheckLog.GUID, Main_Form._登入者名稱);
                        this.sqL_DataGridView_異常通知_盤點錯誤.ClearGrid();
                        uC_調劑作業_TypeA_1.Logout();
                        uC_調劑作業_TypeA_2.Logout();
                        uC_調劑作業_TypeA_3.Logout();
                        uC_調劑作業_TypeA_4.Logout();

                        MyMessageBox.ShowDialog("完成");
                    }


                }
    
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
       
        }
        private void PlC_RJ_Button_異常通知_盤點錯誤_排除時段搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            DateTime dateTime_st = rJ_DatePicker_異常通知_盤點錯誤_排除時段_起始.Value.GetStartDate();
            DateTime dateTime_end = rJ_DatePicker_異常通知_盤點錯誤_排除時段_結束.Value.GetEndDate();
            List<medRecheckLogClass> medRecheckLogClasses = medRecheckLogClass.get_by_occurrence_time_st_end(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, dateTime_st, dateTime_end);
            medRecheckLogClasses = medRecheckLogClasses.FilterByTypes(enum_medRecheckLog_ICDT_TYPE.交班對點, enum_medRecheckLog_ICDT_TYPE.盤點異常);
            if (medRecheckLogClasses.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料");
                return;
            }
            List<object[]> list_med_recheck = medRecheckLogClasses.ClassToSQL<medRecheckLogClass, enum_medRecheckLog>();
            this.sqL_DataGridView_異常通知_盤點錯誤.RefreshGrid(list_med_recheck);
        }
        private void PlC_RJ_Button_異常通知_盤點錯誤_發生時段搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            DateTime dateTime_st = rJ_DatePicker_異常通知_盤點錯誤_發生時段_起始.Value.GetStartDate(); 
            DateTime dateTime_end = rJ_DatePicker_異常通知_盤點錯誤_發生時段_結束.Value.GetEndDate();
            List<medRecheckLogClass> medRecheckLogClasses = medRecheckLogClass.get_by_occurrence_time_st_end(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, dateTime_st, dateTime_end);
            medRecheckLogClasses = medRecheckLogClasses.FilterByTypes(enum_medRecheckLog_ICDT_TYPE.交班對點, enum_medRecheckLog_ICDT_TYPE.盤點異常);
            if (medRecheckLogClasses.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料");
                return;
            }
            List<object[]> list_med_recheck = medRecheckLogClasses.ClassToSQL<medRecheckLogClass, enum_medRecheckLog>();
            this.sqL_DataGridView_異常通知_盤點錯誤.RefreshGrid(list_med_recheck);
        }
        private void PlC_RJ_Button_異常通知_盤點錯誤_未排除顯示_MouseDownEvent(MouseEventArgs mevent)
        {
            List<medRecheckLogClass> medRecheckLogClasses = medRecheckLogClass.get_ng_state_data(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType);
            medRecheckLogClasses = medRecheckLogClasses.FilterByTypes(enum_medRecheckLog_ICDT_TYPE.交班對點, enum_medRecheckLog_ICDT_TYPE.盤點異常);
            if (medRecheckLogClasses.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料");
                return;
            }
            if (medRecheckLogClasses == null) return;
            List<object[]> list_med_recheck = medRecheckLogClasses.ClassToSQL<medRecheckLogClass, enum_medRecheckLog>();
            this.sqL_DataGridView_異常通知_盤點錯誤.RefreshGrid(list_med_recheck);
        }
        #region Function

        static public void Function_異常通知_盤點錯誤_庫存異動(string 藥碼, string 藥名 , double 差異值 , string _登入者名稱 = "" , medRecheckLogClass medRecheckLog = null)
        {
            List<string> list_效期 = new List<string>();
            List<string> list_批號 = new List<string>();
            List<string> list_異動量 = new List<string>();
            List<object[]> list_交易紀錄_Add = new List<object[]>();

            string 備註 = "";
            Function_從SQL取得儲位到雲端資料(藥碼);
            double 庫存量 = Function_從雲端資料取得庫存(藥碼);
            double 異動量 = 差異值;
            double 結存量 = 庫存量 + 異動量;

            if (庫存量 == 0 || 異動量 > 0)
            {
                List<string> 儲位_TYPE = new List<string>();
                List<object> 儲位 = new List<object>();
                Function_從雲端資料取得儲位(Function_藥品碼檢查(藥碼), ref 儲位_TYPE, ref 儲位);
                Funnction_交易記錄查詢_取得指定藥碼批號期效期(藥碼, ref list_效期, ref list_批號);
                if (list_效期.Count == 0)
                {
                    list_效期.Add("2050/01/01");
                    list_批號.Add("自動補足");
                }
                備註 += $"[效期]:{list_效期[0]},[批號]:{list_批號[0]}";

                if (儲位.Count == 0)
                {
                    return;
                }
                object device = Function_庫存異動至雲端資料(儲位[0], 儲位_TYPE[0], list_效期[0], list_批號[0], 異動量.ToString(), true);
                transactionsClass transactionsClass = new transactionsClass();
                transactionsClass.GUID = Guid.NewGuid().ToString();
                transactionsClass.動作 = enum_交易記錄查詢動作.盤點校正.GetEnumName();
                transactionsClass.藥品碼 = 藥碼;
                transactionsClass.藥品名稱 = 藥名;
                transactionsClass.庫存量 = 庫存量.ToString();
                transactionsClass.交易量 = 異動量.ToString();
                transactionsClass.結存量 = 結存量.ToString();             
                transactionsClass.操作人 = _登入者名稱;
                transactionsClass.操作時間 = DateTime.Now.ToDateTimeString_6();
                transactionsClass.開方時間 = DateTime.Now.ToDateTimeString_6();
                transactionsClass.備註 = 備註;
                if (medRecheckLog != null)
                {
                    medRecheckLog.交易紀錄_GUID = transactionsClass.GUID;
                    transactionsClass.收支原因 = medRecheckLog.異常原因;
                }

                object[] trading_value = transactionsClass.ClassToSQL<transactionsClass, enum_交易記錄查詢資料>();
                list_交易紀錄_Add.Add(trading_value);

            }
            else
            {
                List<object[]> list_儲位資訊 = Function_取得異動儲位資訊從雲端資料(藥碼, 異動量);
                if (list_儲位資訊.Count == 0)
                {
                    return;
                }
                for (int k = 0; k < list_儲位資訊.Count; k++)
                {
                    string 效期 = list_儲位資訊[k][(int)enum_儲位資訊.效期].ObjectToString();
                    string 批號 = list_儲位資訊[k][(int)enum_儲位資訊.批號].ObjectToString();
                    備註 += $"[效期]:{效期},[批號]:{批號}";
                    if (k != list_儲位資訊.Count - 1) 備註 += "\n";
                    Function_庫存異動至雲端資料(list_儲位資訊[k], true);
                }

                transactionsClass transactionsClass = new transactionsClass();
                transactionsClass.GUID = Guid.NewGuid().ToString();
                transactionsClass.動作 = enum_交易記錄查詢動作.盤點校正.GetEnumName();
                transactionsClass.藥品碼 = 藥碼;
                transactionsClass.藥品名稱 = 藥名;
                transactionsClass.庫存量 = 庫存量.ToString();
                transactionsClass.交易量 = 異動量.ToString();
                transactionsClass.結存量 = 結存量.ToString();
                transactionsClass.操作人 = _登入者名稱;
                transactionsClass.操作時間 = DateTime.Now.ToDateTimeString_6();
                transactionsClass.開方時間 = DateTime.Now.ToDateTimeString_6();
                transactionsClass.備註 = 備註;

                if (medRecheckLog != null)
                {
                    medRecheckLog.交易紀錄_GUID = transactionsClass.GUID;
                    transactionsClass.收支原因 = medRecheckLog.異常原因;
                }

                object[] trading_value = transactionsClass.ClassToSQL<transactionsClass, enum_交易記錄查詢資料>();

                list_交易紀錄_Add.Add(trading_value);
            }
            if (list_交易紀錄_Add.Count > 0) _sqL_DataGridView_交易記錄查詢.SQL_AddRows(list_交易紀錄_Add, false);
        }
        #endregion
    }
}
