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
        private void Program_藥品資料_藥檔資料_Init()
        {

            string url = $"{dBConfigClass.Api_URL}/api/MED_page/init";
            returnData returnData = new returnData();
            returnData.ServerType = enum_sys_serverSetting_Type.傳送櫃.GetEnumName();
            returnData.ServerName = $"{dBConfigClass.Name}";
            returnData.TableName = "medicine_page";
            string json_in = returnData.JsonSerializationt();
            string json = Basic.Net.WEBApiPostJson($"{url}", json_in);
            Table table = json.JsonDeserializet<Table>();
            if (table == null)
            {
                MyMessageBox.ShowDialog($"本地藥檔表單建立失敗!! Api_URL:{dBConfigClass.Api_URL}");
                return;
            }
            this.comboBox_藥品資料_藥檔資料_管制級別.SelectedIndex = 0;

            this.sqL_DataGridView_藥品資料_藥檔資料.Init(table);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnVisible(false, new enum_藥品資料_藥檔資料().GetEnumNames());
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品碼);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(400, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品名稱);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(400, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.中文名稱);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(400, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品學名);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.包裝單位);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.高價藥品);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.管制級別);

            this.sqL_DataGridView_藥品資料_藥檔資料.RowDoubleClickEvent += SqL_DataGridView_藥品資料_藥檔資料_RowDoubleClickEvent;

            this.plC_RJ_Button_藥品資料_藥品碼_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_藥品碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_藥品名稱_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_藥品名稱_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_藥品條碼_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_藥品條碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_中文名_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_中文名_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_商品名_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_商品名_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_管制級別_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_管制級別_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_高價藥品_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_高價藥品_搜尋_MouseDownEvent;

            this.plC_RJ_Button_藥品資料_登錄.MouseDownEvent += PlC_RJ_Button_藥品資料_登錄_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_HIS下載全部藥檔.MouseDownEvent += PlC_RJ_Button_藥品資料_HIS下載全部藥檔_MouseDownEvent;
            this.plC_RJ_Button__藥品資料_藥檔資料_顯示全部.MouseDownEvent += PlC_RJ_Button__藥品資料_藥檔資料_顯示全部_MouseDownEvent;

            this.plC_UI_Init.Add_Method(this.sub_Program_藥品資料_藥檔資料);
        }

        bool flag_藥品資料_藥檔資料_頁面更新 = false;
        private void sub_Program_藥品資料_藥檔資料()
        {
            if (this.plC_ScreenPage_Main.PageText == "藥品資料" )
            {
          
                if (!this.flag_藥品資料_藥檔資料_頁面更新)
                {

                    this.flag_藥品資料_藥檔資料_頁面更新 = true;
                }
            }
            else
            {
                this.flag_藥品資料_藥檔資料_頁面更新 = false;
            }
        }
        #region Function
        private bool Function_藥品資料_藥檔資料_確認欄位正確(object[] SQL_Data, bool IsMyMessageBoxShow)
        {
            bool flag_OK = false;
            List<string> List_error_msg = new List<string>();
            string str_error_msg = "";
            if (SQL_Data[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString().StringIsEmpty())
            {
                List_error_msg.Add("'藥品碼'欄位空白");
            }
            //if (SQL_Data[(int)enum_藥品資料_藥檔資料.藥品條碼].ObjectToString().StringIsEmpty())
            //{
            //    List_error_msg.Add("'藥品條碼'欄位空白");
            //}
            //if (SQL_Data[(int)enum_藥品資料_藥檔資料.藥品名稱].ObjectToString().StringIsEmpty())
            //{
            //    List_error_msg.Add("'藥品名稱'欄位空白");
            //}
            //if (SQL_Data[(int)enum_藥品資料_藥檔資料.健保碼].ObjectToString().StringIsEmpty())
            //{
            //    List_error_msg.Add("'健保碼'欄位空白");
            //}

            if (SQL_Data[(int)enum_藥品資料_藥檔資料.庫存].ObjectToString().StringToInt32() < 0)
            {
                SQL_Data[(int)enum_藥品資料_藥檔資料.庫存] = "0";
            }

            if (SQL_Data[(int)enum_藥品資料_藥檔資料.安全庫存].ObjectToString().StringToInt32() < 0)
            {
                SQL_Data[(int)enum_藥品資料_藥檔資料.安全庫存] = "0";
            }
            for (int i = 0; i < List_error_msg.Count; i++)
            {
                str_error_msg += i.ToString("00") + ". " + List_error_msg[i] + "\n\r";
            }
            if (str_error_msg == "") flag_OK = true;
            else
            {
                if (IsMyMessageBoxShow) MyMessageBox.ShowDialog(str_error_msg);
            }
            return flag_OK;
        }
        private void Function_藥品資料_藥檔資料_清除攔位()
        {
            this.Invoke(new Action(delegate
            {
                this.textBox_藥品資料_藥檔資料_藥品碼.Text = "";
                this.textBox_藥品資料_藥檔資料_藥品名稱.Text = "";
                this.textBox_藥品資料_藥檔資料_藥品學名.Text = "";
                this.textBox_藥品資料_藥檔資料_中文名稱.Text = "";
                this.textBox_藥品資料_藥檔資料_健保碼.Text = "";
                this.textBox_藥品資料_藥檔資料_庫存.Text = "";
                this.textBox_藥品資料_藥檔資料_安全庫存.Text = "";
                this.textBox_藥品資料_藥檔資料_包裝單位.Text = "";
                this.textBox_藥品資料_藥檔資料_藥品條碼.Text = "";
                this.textBox_藥品資料_藥檔資料_廠牌.Text = "";
                this.textBox_藥品資料_藥檔資料_許可證號.Text = "";

                this.plC_CheckBox_藥品資料_藥檔資料_警訊藥品.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_自定義設定.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_效期管理.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_複盤.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_盲盤.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_結存報表.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_麻醉藥品.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_形狀相似.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_發音相似.Checked = false;
            }));

        }
        private void Function_藥品資料_藥檔資料_登錄()
        {
            object[] value = new object[new enum_藥品資料_藥檔資料().GetLength()];
            string 藥品碼 = Function_藥品碼檢查(this.textBox_藥品資料_藥檔資料_藥品碼.Text);
            value[(int)enum_藥品資料_藥檔資料.藥品碼] = 藥品碼;
            value[(int)enum_藥品資料_藥檔資料.藥品名稱] = this.textBox_藥品資料_藥檔資料_藥品名稱.Text;
            value[(int)enum_藥品資料_藥檔資料.藥品學名] = this.textBox_藥品資料_藥檔資料_藥品學名.Text;
            value[(int)enum_藥品資料_藥檔資料.中文名稱] = this.textBox_藥品資料_藥檔資料_中文名稱.Text;
            value[(int)enum_藥品資料_藥檔資料.健保碼] = this.textBox_藥品資料_藥檔資料_健保碼.Text;
            value[(int)enum_藥品資料_藥檔資料.藥品條碼] = this.textBox_藥品資料_藥檔資料_藥品條碼.Text;
            value[(int)enum_藥品資料_藥檔資料.包裝單位] = this.textBox_藥品資料_藥檔資料_包裝單位.Text;
            value[(int)enum_藥品資料_藥檔資料.庫存] = this.textBox_藥品資料_藥檔資料_庫存.Text;
            value[(int)enum_藥品資料_藥檔資料.安全庫存] = this.textBox_藥品資料_藥檔資料_安全庫存.Text;
            value[(int)enum_藥品資料_藥檔資料.廠牌] = this.textBox_藥品資料_藥檔資料_廠牌.Text;
            value[(int)enum_藥品資料_藥檔資料.藥品許可證號] = this.textBox_藥品資料_藥檔資料_許可證號.Text;
            value[(int)enum_藥品資料_藥檔資料.警訊藥品] = this.plC_CheckBox_藥品資料_藥檔資料_警訊藥品.Checked.ToString();
            value[(int)enum_藥品資料_藥檔資料.高價藥品] = this.plC_CheckBox_藥品資料_藥檔資料_高價藥品.Checked.ToString();
            value[(int)enum_藥品資料_藥檔資料.生物製劑] = this.plC_CheckBox_藥品資料_藥檔資料_生物製劑.Checked.ToString();
            value[(int)enum_藥品資料_藥檔資料.管制級別] = this.comboBox_藥品資料_藥檔資料_管制級別.Texts;
            if (this.Function_藥品資料_藥檔資料_確認欄位正確(value, true))
            {
                List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetRows(enum_藥品資料_藥檔資料.藥品碼.GetEnumName(), this.textBox_藥品資料_藥檔資料_藥品碼.Text, false);
                if (list_value.Count > 0)
                {
                    value[(int)enum_藥品資料_藥檔資料.GUID] = list_value[0][(int)enum_藥品資料_藥檔資料.GUID].ObjectToString();
                    this.sqL_DataGridView_藥品資料_藥檔資料.SQL_Replace((int)enum_藥品資料_藥檔資料.GUID, value[(int)enum_藥品資料_藥檔資料.GUID].ObjectToString(), value, false);
                    this.sqL_DataGridView_藥品資料_藥檔資料.ReplaceExtra(value, true);
                }
                else
                {
                    value[(int)enum_藥品資料_藥檔資料.GUID] = Guid.NewGuid().ToString();
                    this.sqL_DataGridView_藥品資料_藥檔資料.SQL_AddRow(value, false);
                    this.sqL_DataGridView_藥品資料_藥檔資料.AddRow(value, true);
                }
              
                this.Function_藥品資料_藥檔資料_清除攔位();

            }


        }
        private void Function_藥品資料_藥檔資料_搜尋BarCode(string BarCode)
        {
            List<medClass> medClasses = Function_搜尋Barcode(BarCode);
            List<object[]> list_藥檔資料 = new List<object[]>();
            for (int i = 0; i < medClasses.Count; i++)
            {
                List<object[]> list_藥檔資料_buf = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetRows((int)enum_藥品資料_藥檔資料.藥品碼, medClasses[i].藥品碼, false);
                if (list_藥檔資料_buf.Count > 0) list_藥檔資料.Add(list_藥檔資料_buf[0]);
            }
            this.sqL_DataGridView_藥品資料_藥檔資料.RefreshGrid(list_藥檔資料);
            if (list_藥檔資料.Count == 0)
            {
                MyMessageBox.ShowDialog("查無此條碼資訊!");
                return;
            }
            else
            {
                this.Invoke(new Action(delegate
                {
                    this.sqL_DataGridView_藥品資料_藥檔資料.SetSelectRow(list_藥檔資料[0]);
                    this.sqL_DataGridView_藥品資料_藥檔資料.On_RowDoubleClick();
                }));

            }
        }
        #endregion
        #region Event
        private void SqL_DataGridView_藥品資料_藥檔資料_RowDoubleClickEvent(object[] RowValue)
        {
            string 藥品碼 = RowValue[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
            this.textBox_藥品資料_藥檔資料_藥品碼.Text = RowValue[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
            this.textBox_藥品資料_藥檔資料_藥品名稱.Text = RowValue[(int)enum_藥品資料_藥檔資料.藥品名稱].ObjectToString();
            this.textBox_藥品資料_藥檔資料_藥品學名.Text = RowValue[(int)enum_藥品資料_藥檔資料.藥品學名].ObjectToString();
            this.textBox_藥品資料_藥檔資料_中文名稱.Text = RowValue[(int)enum_藥品資料_藥檔資料.中文名稱].ObjectToString();
            this.textBox_藥品資料_藥檔資料_健保碼.Text = RowValue[(int)enum_藥品資料_藥檔資料.健保碼].ObjectToString();
            this.textBox_藥品資料_藥檔資料_庫存.Text = RowValue[(int)enum_藥品資料_藥檔資料.庫存].ObjectToString();
            this.textBox_藥品資料_藥檔資料_安全庫存.Text = RowValue[(int)enum_藥品資料_藥檔資料.安全庫存].ObjectToString();
            this.textBox_藥品資料_藥檔資料_包裝單位.Text = RowValue[(int)enum_藥品資料_藥檔資料.包裝單位].ObjectToString();
            this.textBox_藥品資料_藥檔資料_藥品條碼.Text = RowValue[(int)enum_藥品資料_藥檔資料.藥品條碼].ObjectToString();
            this.textBox_藥品資料_藥檔資料_廠牌.Text = RowValue[(int)enum_藥品資料_藥檔資料.廠牌].ObjectToString();
            this.textBox_藥品資料_藥檔資料_許可證號.Text = RowValue[(int)enum_藥品資料_藥檔資料.藥品許可證號].ObjectToString();
            this.plC_CheckBox_藥品資料_藥檔資料_警訊藥品.Checked = (RowValue[(int)enum_藥品資料_藥檔資料.警訊藥品].ObjectToString() == true.ToString());
            this.plC_CheckBox_藥品資料_藥檔資料_高價藥品.Checked = (RowValue[(int)enum_藥品資料_藥檔資料.高價藥品].ObjectToString() == true.ToString());
            this.plC_CheckBox_藥品資料_藥檔資料_生物製劑.Checked = (RowValue[(int)enum_藥品資料_藥檔資料.生物製劑].ObjectToString() == true.ToString());


            this.comboBox_藥品資料_藥檔資料_管制級別.Texts = RowValue[(int)enum_藥品資料_藥檔資料.管制級別].ObjectToString();
        }

        private void PlC_RJ_Button__藥品資料_藥檔資料_顯示全部_MouseDownEvent(MouseEventArgs mevent)
        {
            this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(true);
        }
        private void PlC_RJ_Button_藥品資料_藥品條碼_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (textBox_藥品資料_藥檔資料_資料查詢_藥品條碼.Text != "")
            {
                Function_藥品資料_藥檔資料_搜尋BarCode(textBox_藥品資料_藥檔資料_資料查詢_藥品條碼.Text);
            }
        }
        private void PlC_RJ_Button_藥品資料_藥品名稱_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            if (textBox_藥品資料_藥檔資料_資料查詢_藥品名稱.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未輸入搜尋資料!");
                return;
            }

            if (!textBox_藥品資料_藥檔資料_資料查詢_藥品名稱.Text.StringIsEmpty())
            {

                if (rJ_RatioButton_藥品資料_藥檔資料_前綴.Checked)
                {
                    if (textBox_藥品資料_藥檔資料_資料查詢_藥品名稱.Text.Length < 3)
                    {
                        MyMessageBox.ShowDialog("藥品名稱搜尋字元不得小於3個!");
                        return;
                    }
                    list_value = list_value.GetRowsStartWithByLike((int)enum_藥品資料_藥檔資料.藥品名稱, textBox_藥品資料_藥檔資料_資料查詢_藥品名稱.Text);
                }
                else if (rJ_RatioButton_藥品資料_藥檔資料_模糊.Checked)
                {
                    list_value = list_value.GetRowsByLike((int)enum_藥品資料_藥檔資料.藥品名稱, textBox_藥品資料_藥檔資料_資料查詢_藥品名稱.Text, true);
                }
                this.sqL_DataGridView_藥品資料_藥檔資料.RefreshGrid(list_value);
                if (list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("查無資料!");
                }
            }
        }
        private void PlC_RJ_Button_藥品資料_藥品碼_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            if (!textBox_藥品資料_藥檔資料_資料查詢_藥品碼.Text.StringIsEmpty()) list_value = list_value.GetRowsByLike((int)enum_藥品資料_藥檔資料.藥品碼, textBox_藥品資料_藥檔資料_資料查詢_藥品碼.Text);
            this.sqL_DataGridView_藥品資料_藥檔資料.RefreshGrid(list_value);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
            }
        }
        private void PlC_RJ_Button_藥品資料_商品名_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            if (!textBox_藥品資料_藥檔資料_資料查詢_商品名.Text.StringIsEmpty())
            {

                if (rJ_RatioButton_藥品資料_藥檔資料_前綴.Checked)
                {
                    if (textBox_藥品資料_藥檔資料_資料查詢_商品名.Text.Length < 3)
                    {
                        MyMessageBox.ShowDialog("藥品名稱搜尋字元不得小於3個!");
                        return;
                    }
                    list_value = list_value.GetRowsStartWithByLike((int)enum_藥品資料_藥檔資料.藥品學名, textBox_藥品資料_藥檔資料_資料查詢_商品名.Text);
                }
                else if (rJ_RatioButton_藥品資料_藥檔資料_模糊.Checked)
                {
                    list_value = list_value.GetRowsByLike((int)enum_藥品資料_藥檔資料.藥品學名, textBox_藥品資料_藥檔資料_資料查詢_商品名.Text, true);
                }
                this.sqL_DataGridView_藥品資料_藥檔資料.RefreshGrid(list_value);
                if (list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("查無資料!");
                }
            }
        }
        private void PlC_RJ_Button_藥品資料_中文名_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            if (!textBox_藥品資料_藥檔資料_資料查詢_中文名.Text.StringIsEmpty())
            {

                if (rJ_RatioButton_藥品資料_藥檔資料_前綴.Checked)
                {
                    if (textBox_藥品資料_藥檔資料_資料查詢_中文名.Text.Length < 3)
                    {
                        MyMessageBox.ShowDialog("藥品名稱搜尋字元不得小於3個!");
                        return;
                    }
                    list_value = list_value.GetRowsStartWithByLike((int)enum_藥品資料_藥檔資料.中文名稱, textBox_藥品資料_藥檔資料_資料查詢_中文名.Text);
                }
                else if (rJ_RatioButton_藥品資料_藥檔資料_模糊.Checked)
                {
                    list_value = list_value.GetRowsByLike((int)enum_藥品資料_藥檔資料.中文名稱, textBox_藥品資料_藥檔資料_資料查詢_中文名.Text, true);
                }
                this.sqL_DataGridView_藥品資料_藥檔資料.RefreshGrid(list_value);
                if (list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("查無資料!");
                }
            }
        }
        private void PlC_RJ_Button_藥品資料_高價藥品_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            list_value = list_value.GetRowsByLike((int)enum_藥品資料_藥檔資料.高價藥品, true.ToString().ToUpper(), true);
            this.sqL_DataGridView_藥品資料_藥檔資料.RefreshGrid(list_value);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
            }
        }
        private void PlC_RJ_Button_藥品資料_管制級別_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            if (rJ_ComboBox_藥品資料_藥檔資料_資料查詢_管制級別.SelectedItem.ObjectToString().StringIsEmpty())
            {
                MyMessageBox.ShowDialog("請選擇資料!");
                return;
            }
            list_value = list_value.GetRows((int)enum_藥品資料_藥檔資料.管制級別, rJ_ComboBox_藥品資料_藥檔資料_資料查詢_管制級別.SelectedItem.ObjectToString());
            this.sqL_DataGridView_藥品資料_藥檔資料.RefreshGrid(list_value);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
            }
        }

        private void PlC_RJ_Button_藥品資料_登錄_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.Function_藥品資料_藥檔資料_登錄();
            }));

        }
        private void PlC_RJ_Button_藥品資料_HIS下載全部藥檔_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("是否下載全部藥檔?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
     
            List<object[]> list_雲端藥檔_buf = new List<object[]>();
            List<object[]> list_藥品資料 = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_藥品資料_buf = new List<object[]>();
            List<object[]> list_藥品資料_add = new List<object[]>();
            List<object[]> list_藥品資料_replace = new List<object[]>();
            List<object[]> list_藥品資料_delete = new List<object[]>();
            Dialog_Prcessbar dialog_Prcessbar = new Dialog_Prcessbar(0);
            dialog_Prcessbar.State = $"開始比對線上藥檔...";
            string url = dBConfigClass.MedApiURL;
            string response = "";
            if (!url.StringIsEmpty())
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                response = Basic.Net.WEBApiGet($"{url}");
                Console.WriteLine($"HIS下載全部藥檔 , response:{response},耗時{myTimer.ToString()}ms");
            }
            returnData returnData = response.JsonDeserializet<returnData>();
            List<object[]> list_雲端藥檔 = new List<object[]>();
            List<medClass> list_medClass = returnData.Data.ObjToListClass<medClass>();
            if(list_medClass == null)
            {
                list_雲端藥檔 = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            }
            else
            {
                list_雲端藥檔 = list_medClass.ClassToSQL<medClass, enum_雲端藥檔>();
            }
            dialog_Prcessbar.MaxValue = list_medClass.Count;



            for (int i = 0; i < list_雲端藥檔.Count; i++)
            {
                dialog_Prcessbar.Value = i;
                list_藥品資料_buf = list_藥品資料.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, list_雲端藥檔[i][(int)enum_雲端藥檔.藥品碼].ObjectToString());
                if (list_藥品資料_buf.Count == 0)
                {
                    object[] value = new object[new enum_藥品資料_藥檔資料().GetLength()];
                    value[(int)enum_藥品資料_藥檔資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品碼].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品名稱] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品名稱].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品學名] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品學名].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.中文名稱] = list_雲端藥檔[i][(int)enum_雲端藥檔.中文名稱].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.健保碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.健保碼].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.包裝單位] = list_雲端藥檔[i][(int)enum_雲端藥檔.包裝單位].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.庫存] = "0";
                    value[(int)enum_藥品資料_藥檔資料.安全庫存] = "0";
                    value[(int)enum_藥品資料_藥檔資料.基準量] = "0";
                    value[(int)enum_藥品資料_藥檔資料.警訊藥品] = list_雲端藥檔[i][(int)enum_雲端藥檔.警訊藥品].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.管制級別] = list_雲端藥檔[i][(int)enum_雲端藥檔.管制級別].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.生物製劑] = list_雲端藥檔[i][(int)enum_雲端藥檔.生物製劑].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.高價藥品] = list_雲端藥檔[i][(int)enum_雲端藥檔.高價藥品].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品許可證號] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品許可證號].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.開檔狀態] = list_雲端藥檔[i][(int)enum_雲端藥檔.開檔狀態].ObjectToString();
                    list_藥品資料_add.Add(value);
                }
                else
                {
                    object[] value = list_藥品資料_buf[0];
                    value[(int)enum_藥品資料_藥檔資料.藥品碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品碼].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品名稱] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品名稱].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品學名] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品學名].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.中文名稱] = list_雲端藥檔[i][(int)enum_雲端藥檔.中文名稱].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.健保碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.健保碼].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.包裝單位] = list_雲端藥檔[i][(int)enum_雲端藥檔.包裝單位].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.警訊藥品] = list_雲端藥檔[i][(int)enum_雲端藥檔.警訊藥品].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.管制級別] = list_雲端藥檔[i][(int)enum_雲端藥檔.管制級別].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.生物製劑] = list_雲端藥檔[i][(int)enum_雲端藥檔.生物製劑].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.高價藥品] = list_雲端藥檔[i][(int)enum_雲端藥檔.高價藥品].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品許可證號] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品許可證號].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.開檔狀態] = list_雲端藥檔[i][(int)enum_雲端藥檔.開檔狀態].ObjectToString();
                    list_藥品資料_replace.Add(value);
                }
            }
            dialog_Prcessbar.State = $"寫入資料庫...";
            if (list_藥品資料_add.Count > 0) this.sqL_DataGridView_藥品資料_藥檔資料.SQL_AddRows(list_藥品資料_add, false);
            if (list_藥品資料_replace.Count > 0) this.sqL_DataGridView_藥品資料_藥檔資料.SQL_ReplaceExtra(list_藥品資料_replace, false);
            dialog_Prcessbar.Close();
        }
        #endregion
    }
}
