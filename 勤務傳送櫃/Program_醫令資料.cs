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

namespace 勤務傳送系統
{
    public partial class Main_Form : Form
    {
        private void Program_醫令資料_Init()
        {


            string url = $"{dBConfigClass.Api_URL}/api/order/init";
            returnData returnData = new returnData();
            returnData.ServerType = enum_sys_serverSetting_Type.傳送櫃.GetEnumName();
            returnData.ServerName = $"{dBConfigClass.Name}";
            string json_in = returnData.JsonSerializationt();
            string json = Basic.Net.WEBApiPostJson($"{url}", json_in);
            Table table = json.JsonDeserializet<Table>();
            if (table == null)
            {
                MyMessageBox.ShowDialog($"醫令資料表單建立失敗!! Api_URL:{dBConfigClass.Api_URL}");
                return;
            }

            this.sqL_DataGridView_醫令資料.InitEx(table);
            this.sqL_DataGridView_醫令資料.Set_ColumnVisible(false, new enum_醫囑資料().GetEnumNames());
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.藥局代碼);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.藥袋類型);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.領藥號);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.藥品碼);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.藥品名稱);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(50, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.劑量單位);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.頻次);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(50, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.病房);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.病人姓名);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.病歷號);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleRight, enum_醫囑資料.交易量);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleCenter, enum_醫囑資料.開方日期);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleCenter, enum_醫囑資料.產出時間);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleCenter, enum_醫囑資料.過帳時間);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.狀態);
            this.sqL_DataGridView_醫令資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_醫囑資料.備註);


            this.sqL_DataGridView_醫令資料.DataGridRowsChangeRefEvent += SqL_DataGridView_醫令資料_DataGridRowsChangeRefEvent;
            this.sqL_DataGridView_醫令資料.DataGridRefreshEvent += SqL_DataGridView_醫令資料_DataGridRefreshEvent;
            this.sqL_DataGridView_醫令資料.DataGridRowsChangeEvent += SqL_DataGridView_醫令資料_DataGridRowsChangeEvent;
            this.sqL_DataGridView_醫令資料.RowDoubleClickEvent += SqL_DataGridView_醫令資料_RowDoubleClickEvent;

            this.plC_RJ_Button_醫令資料_開方日期_搜尋.MouseDownEvent += PlC_RJ_Button_醫令資料_開方日期_搜尋_MouseDownEvent;
            this.plC_RJ_Button_醫令資料_藥碼_搜尋.MouseDownEvent += PlC_RJ_Button_醫令資料_藥碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_醫令資料_藥名_搜尋.MouseDownEvent += PlC_RJ_Button_醫令資料_藥名_搜尋_MouseDownEvent;
            this.plC_RJ_Button_醫令資料_病歷號_搜尋.MouseDownEvent += PlC_RJ_Button_醫令資料_病歷號_搜尋_MouseDownEvent;
            this.plC_RJ_Button_醫令資料_領藥號_搜尋.MouseDownEvent += PlC_RJ_Button_醫令資料_領藥號_搜尋_MouseDownEvent;

            this.plC_RJ_Button_醫令資料_設為未過帳.MouseDownEvent += PlC_RJ_Button_醫令資料_設為未過帳_MouseDownEvent;

            this.plC_UI_Init.Add_Method(Program_醫令資料);
        }



        private void Program_醫令資料()
        {
      
        }
        #region Function

        #endregion
        #region Event
        private void SqL_DataGridView_醫令資料_RowDoubleClickEvent(object[] RowValue)
        {
            string pri_key = RowValue[(int)enum_醫囑資料.PRI_KEY].ObjectToString();
            this.Invoke(new Action(delegate 
            {
                textBox_醫令資料_PRI_KEY.Text = pri_key;
            }));
        }
        private void SqL_DataGridView_醫令資料_DataGridRefreshEvent()
        {
            String 狀態 = "";
            for (int i = 0; i < this.sqL_DataGridView_醫令資料.dataGridView.Rows.Count; i++)
            {
                狀態 = this.sqL_DataGridView_醫令資料.dataGridView.Rows[i].Cells[(int)enum_醫囑資料.狀態].Value.ToString();
                if (狀態 == "已過帳")
                {
                    this.sqL_DataGridView_醫令資料.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                    this.sqL_DataGridView_醫令資料.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                if (狀態 == enum_醫囑資料_狀態.庫存不足.GetEnumName())
                {
                    this.sqL_DataGridView_醫令資料.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    this.sqL_DataGridView_醫令資料.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                if (狀態 == enum_醫囑資料_狀態.無儲位.GetEnumName())
                {
                    this.sqL_DataGridView_醫令資料.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    this.sqL_DataGridView_醫令資料.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }
        private void SqL_DataGridView_醫令資料_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {
            RowsList.Sort(new ICP_醫令資料());
        }
        private void SqL_DataGridView_醫令資料_DataGridRowsChangeEvent(List<object[]> RowsList)
        {
            // RowsList.Sort(new ICP_醫令資料());
        }
        private void PlC_RJ_Button_醫令資料_開方日期_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                LoadingForm.ShowLoadingForm();
                MyTimer myTimer = new MyTimer();
                myTimer.TickStop();
                myTimer.StartTickTime(50000);
                List<object[]> list_value = this.sqL_DataGridView_醫令資料.SQL_GetRowsByBetween((int)enum_醫囑資料.開方日期, dateTimePicke_醫令資料_開方日期_起始, dateTimePicke_醫令資料_開方日期_結束, false);

                Console.Write($"取得醫令資料 , 耗時 : {myTimer.ToString()} ms\n");
                this.sqL_DataGridView_醫令資料.RefreshGrid(list_value);
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
        }
        private void PlC_RJ_Button_醫令資料_領藥號_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                LoadingForm.ShowLoadingForm();
                MyTimer myTimer = new MyTimer();
                myTimer.TickStop();
                myTimer.StartTickTime(50000);
                List<object[]> list_value = this.sqL_DataGridView_醫令資料.SQL_GetRows((int)enum_醫囑資料.領藥號, rJ_TextBox_醫令資料_搜尋條件_領藥號.Text, false);
                Console.Write($"取得醫令資料 , 耗時 : {myTimer.ToString()} ms\n");
                this.sqL_DataGridView_醫令資料.RefreshGrid(list_value);
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
        }
        private void PlC_RJ_Button_醫令資料_病歷號_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                LoadingForm.ShowLoadingForm();
                MyTimer myTimer = new MyTimer();
                myTimer.TickStop();
                myTimer.StartTickTime(50000);
                List<object[]> list_value = this.sqL_DataGridView_醫令資料.SQL_GetRows((int)enum_醫囑資料.病歷號, rJ_TextBox_醫令資料_搜尋條件_病歷號.Text, false);

                Console.Write($"取得醫令資料 , 耗時 : {myTimer.ToString()} ms\n");
                this.sqL_DataGridView_醫令資料.RefreshGrid(list_value);
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
        }
        private void PlC_RJ_Button_醫令資料_藥名_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                LoadingForm.ShowLoadingForm();
                MyTimer myTimer = new MyTimer();
                myTimer.TickStop();
                myTimer.StartTickTime(50000);
                List<object[]> list_value = this.sqL_DataGridView_醫令資料.SQL_GetRows((int)enum_醫囑資料.藥品名稱, rJ_TextBox_醫令資料_搜尋條件_藥名.Text, false);
                Console.Write($"取得醫令資料 , 耗時 : {myTimer.ToString()} ms\n");
                this.sqL_DataGridView_醫令資料.RefreshGrid(list_value);
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
        }
        private void PlC_RJ_Button_醫令資料_藥碼_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                LoadingForm.ShowLoadingForm();
                MyTimer myTimer = new MyTimer();
                myTimer.TickStop();
                myTimer.StartTickTime(50000);
                List<object[]> list_value = this.sqL_DataGridView_醫令資料.SQL_GetRows((int)enum_醫囑資料.藥品碼, rJ_TextBox_醫令資料_搜尋條件_藥碼.Text, false);
                Console.Write($"取得醫令資料 , 耗時 : {myTimer.ToString()} ms\n");
                this.sqL_DataGridView_醫令資料.RefreshGrid(list_value);
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
        }
        private void PlC_RJ_Button_醫令資料_設為未過帳_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_醫令資料.Get_All_Select_RowsValues();
            for (int i = 0; i < list_value.Count; i++)
            {
                list_value[i][(int)enum_醫囑資料.狀態] = "未過帳";
            }
            this.sqL_DataGridView_醫令資料.SQL_ReplaceExtra(list_value, false);
            this.sqL_DataGridView_醫令資料.ReplaceExtra(list_value, true);

        }
        private void PlC_RJ_Button_醫令資料_選取資料刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_醫令資料.Get_All_Select_RowsValues();
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            if (MyMessageBox.ShowDialog($"確認刪除選取{list_value.Count}筆資料?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            this.sqL_DataGridView_醫令資料.SQL_DeleteExtra(list_value, false);
            this.sqL_DataGridView_醫令資料.DeleteExtra(list_value, true);
        }
        #endregion

        private class ICP_醫令資料 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {

                string date01 = x[(int)enum_醫囑資料.產出時間].ToDateTimeString_6();
                string date02 = y[(int)enum_醫囑資料.產出時間].ToDateTimeString_6();
                return date01.CompareTo(date02);

            }
        }
    }
}
