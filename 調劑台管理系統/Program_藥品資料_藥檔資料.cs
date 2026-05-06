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
using H_Pannel_lib;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using HIS_DB_Lib;
using SQLUI;
namespace 調劑台管理系統
{
    public enum ContextMenuStrip_藥品資料_藥檔資料
    {
        [Description("M8000")]
        匯出,
        [Description("M8000")]
        匯入,
        [Description("M8000")]
        顯示有儲位藥品,
        [Description("M8000")]
        下載未建置條碼藥品,
        [Description("M8000")]
        下載已建置條碼藥品,
        [Description("M8000")]
        上載條碼Excel表,
        [Description("M8000")]
        藥檔維護,
        [Description("S4077")]
        更新至線上藥檔,
    }
  
    public enum enum_藥品資料_藥檔資料_匯入
    {
        藥碼,
        中文名,
        藥名,
        藥品學名,
        健保碼,
        藥品條碼,
        包裝單位,
        警訊藥品,
        高價藥品,
        管制級別,
        類別,
        廠牌,
        藥品許可證號,
    }
    public enum enum_藥品資料_藥檔資料_匯出
    {
        藥碼,
        中文名,
        藥名,
        藥品學名,
        健保碼,
        包裝單位,
        庫存,
        安全庫存,
        警訊藥品,
        高價藥品,
        管制級別,
        類別,
        廠牌,
        藥品許可證號,
    }
    public enum enum_藥品資料_藥檔資料_未建置藥品條碼_匯出
    {
        藥品碼,
        料號,
        藥品名稱,
    }
    public enum enum_藥品資料_藥檔資料_已建置藥品條碼_匯出
    {
        藥品碼,
        料號,
        藥品名稱,
        條碼1,
        條碼2,
        條碼3,
        條碼4,
    }
    public enum enum_藥品資料_藥檔資料_已建置藥品條碼_匯入
    {
        藥品碼,
        料號,
        藥品名稱,
        條碼1,
        條碼2,
        條碼3,
        條碼4,
    }
    public enum enum_medGroup
    {
        GUID,
        群組序號,
        群組名稱,
    }
   
    public partial class Main_Form : Form
    {
        private DeviceBasicClass DeviceBasicClass_儲位庫存 = new DeviceBasicClass();
        static public SQL_DataGridView _sqL_DataGridView_藥品資料_藥檔資料;
        private void Program_藥品資料_藥檔資料_Init()
        {
            Table table = medClass.init(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, medClass.StoreType.調劑台);
            
            table[enum_藥品資料_藥檔資料.開檔狀態.GetEnumName()].TypeName = Table.GetTypeName(Table.OtherType.ENUM, new enum_開檔狀態().GetEnumNames());

            if (table == null)
            {
                MyMessageBox.ShowDialog($"本地藥檔表單建立失敗!! Api_URL:{dBConfigClass.Api_URL}");
                return;
            }
            _sqL_DataGridView_藥品資料_藥檔資料 = this.sqL_DataGridView_藥品資料_藥檔資料;
            this.sqL_DataGridView_藥品資料_藥檔資料.Init(table);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnVisible(false, new enum_藥品資料_藥檔資料().GetEnumNames());
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品碼);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(300, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品名稱);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(300, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.中文名稱);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品學名);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.包裝單位);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(70, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.庫存);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(70, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.安全庫存);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(70, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.基準量);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.警訊藥品);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.管制級別);  
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.開檔狀態);

            this.sqL_DataGridView_藥品資料_藥檔資料.ComboBoxSelectedIndexChangedEvent += SqL_DataGridView_藥品資料_藥檔資料_ComboBoxSelectedIndexChangedEvent;

            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnText("藥碼", enum_藥品資料_藥檔資料.藥品碼);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnText("藥名", enum_藥品資料_藥檔資料.藥品名稱);
            this.sqL_DataGridView_藥品資料_藥檔資料.Set_ColumnText("中文名", enum_藥品資料_藥檔資料.中文名稱);

            this.sqL_DataGridView_藥品資料_藥檔資料.RowEnterEvent += SqL_DataGridView_藥品資料_藥檔資料_RowEnterEvent;
            this.sqL_DataGridView_藥品資料_藥檔資料.RowDoubleClickEvent += SqL_DataGridView_藥品資料_藥檔資料_RowDoubleClickEvent;
            this.sqL_DataGridView_藥品資料_藥檔資料.MouseDown += SqL_DataGridView_藥品資料_藥檔資料_MouseDown;
            this.sqL_DataGridView_藥品資料_藥檔資料.DataGridRefreshEvent += sqL_DataGridView_藥品資料_藥檔資料_DataGridRefreshEvent;
            this.sqL_DataGridView_藥品資料_藥檔資料.DataGridRowsChangeEvent += SqL_DataGridView_藥品資料_藥檔資料_DataGridRowsChangeEvent;
            this.comboBox_藥品資料_藥檔資料_管制級別.SelectedIndex = 0;

            this.plC_RJ_Button_藥品資料_顯示全部.MouseDownEvent += PlC_RJ_Button_藥品資料_顯示全部_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_藥品碼_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_藥品碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_藥品名稱_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_藥品名稱_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_藥品條碼_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_藥品條碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_中文名_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_中文名_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_商品名_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_商品名_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_管制級別_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_管制級別_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_高價藥品_搜尋.MouseDownEvent += PlC_RJ_Button_藥品資料_高價藥品_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_條碼管理.MouseDownEvent += PlC_RJ_Button_藥品資料_條碼管理_MouseDownEvent;

            this.plC_RJ_Button_藥品資料_登錄.MouseDownEvent += PlC_RJ_Button_藥品資料_登錄_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_刪除.MouseDownEvent += PlC_RJ_Button_藥品資料_刪除_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_新藥建置.MouseDownEvent += PlC_RJ_Button_藥品資料_新藥建置_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_更新藥櫃資料.MouseDownEvent += PlC_RJ_Button_藥品資料_更新藥櫃資料_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_HIS下載全部藥檔.MouseDownEvent += PlC_RJ_Button_藥品資料_HIS下載全部藥檔_MouseDownEvent;
            this.plC_RJ_Button_藥檔資料_藥品群組.MouseDownEvent += PlC_RJ_Button_藥檔資料_藥品群組_MouseDownEvent;
            this.plC_RJ_Button_藥品資料_批次設定.MouseDownEvent += PlC_RJ_Button_藥品資料_批次設定_MouseDownEvent;

            this.plC_CheckBox_藥品資料_藥檔資料_自定義設定.CheckedChanged += PlC_CheckBox_藥品資料_藥檔資料_自定義設定_CheckedChanged;
            this.plC_CheckBox_藥品資料_藥檔資料_效期管理.CheckedChanged += PlC_CheckBox_藥品資料_藥檔資料_效期管理_CheckedChanged;
            this.plC_CheckBox_藥品資料_藥檔資料_複盤.CheckedChanged += PlC_CheckBox_藥品資料_藥檔資料_複盤_CheckedChanged;
            this.plC_CheckBox_藥品資料_藥檔資料_盲盤.CheckedChanged += PlC_CheckBox_藥品資料_藥檔資料_盲盤_CheckedChanged;
            this.plC_CheckBox_藥品資料_藥檔資料_結存報表.CheckedChanged += PlC_CheckBox_藥品資料_藥檔資料_結存報表_CheckedChanged;

            this.plC_UI_Init.Add_Method(this.sub_Program_藥品資料_藥檔資料);
        }



        bool flag_藥品資料_藥檔資料_頁面更新 = false;
        private void sub_Program_藥品資料_藥檔資料()
        {
            if (this.plC_ScreenPage_Main.PageText == "藥品資料" && this.plC_ScreenPage_藥品資料.PageText == "藥檔資料")
            {
                if(Dialog_條碼管理.IsShown == false)
                {
                    string bacode01 = Function_ReadBacodeScanner01();
                    string bacode02 = Function_ReadBacodeScanner02();
                    if (bacode01.StringIsEmpty() == false)
                    {
                        Function_藥品資料_藥檔資料_搜尋BarCode(bacode01);
                    }
                    else if (bacode02.StringIsEmpty() == false)
                    {
                        Function_藥品資料_藥檔資料_搜尋BarCode(bacode02);
                    }
                }
            
                if (this.plC_CheckBox_藥品資料_藥檔資料_自定義設定.Checked)
                {
                    if(groupBox_藥品資料_藥檔資料_設定.Enabled != true)
                    {
                        this.Invoke(new Action(delegate
                        {
                            groupBox_藥品資料_藥檔資料_設定.Enabled = true;
                        }));
                    }
                }
                else
                {
                    if (groupBox_藥品資料_藥檔資料_設定.Enabled != false)
                    {
                        this.Invoke(new Action(delegate
                        {
                            groupBox_藥品資料_藥檔資料_設定.Enabled = false;
                        }));
                    }
                }
                if (!this.flag_藥品資料_藥檔資料_頁面更新)
                {
                    Function_從SQL取得儲位到本地資料();
         
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
          
            if (SQL_Data[(int)enum_藥品資料_藥檔資料.庫存].ObjectToString().StringToDouble() < 0 )
            {
                SQL_Data[(int)enum_藥品資料_藥檔資料.庫存] = "0";
            }

            if (SQL_Data[(int)enum_藥品資料_藥檔資料.安全庫存].ObjectToString().StringToDouble() < 0)
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
        private string Function_藥品資料_藥檔資料_檢查內容(object[] value)
        {
            string str_error = "";
            List<string> list_error = new List<string>();
            if (value[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString().StringIsEmpty())
            {
                list_error.Add("'藥品碼'欄位不得空白!");
            }
            for (int i = 0; i < list_error.Count; i++)
            {
                str_error += $"{(i + 1).ToString("00")}. {list_error[i]}";
                if (i != list_error.Count - 1) str_error += "\n";
            }
            return str_error;
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
                value = Function_藥品資料_藥檔資料_檢查藥品設定表(藥品碼);

                medConfigClass medConfigClass = medConfigClass.get_by_codes(Main_Form.API_Server, 藥品碼);
                medConfigClass.自定義 = plC_CheckBox_藥品資料_藥檔資料_自定義設定.Checked.ToString();
                medConfigClass.效期管理 = plC_CheckBox_藥品資料_藥檔資料_效期管理.Checked.ToString();
                medConfigClass.盲盤 = plC_CheckBox_藥品資料_藥檔資料_盲盤.Checked.ToString();
                medConfigClass.複盤 = plC_CheckBox_藥品資料_藥檔資料_複盤.Checked.ToString();
                medConfigClass.結存報表 = plC_CheckBox_藥品資料_藥檔資料_結存報表.Checked.ToString();
                medConfigClass.雙人覆核 = plC_CheckBox_藥品資料_藥檔資料_雙人覆核.Checked.ToString();
                medConfigClass.麻醉藥品 = plC_CheckBox_藥品資料_藥檔資料_麻醉藥品.Checked.ToString();
                medConfigClass.形狀相似 = plC_CheckBox_藥品資料_藥檔資料_形狀相似.Checked.ToString();
                medConfigClass.發音相似 = plC_CheckBox_藥品資料_藥檔資料_發音相似.Checked.ToString();
                medConfigClass.調劑註記 = plC_CheckBox_藥品資料_藥檔資料_調劑註記.Checked.ToString();

                medConfigClass.add(Main_Form.API_Server, medConfigClass);

                this.Function_藥品資料_藥檔資料_清除攔位();
               
            }


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
                this.plC_CheckBox_藥品資料_藥檔資料_調劑註記.Checked = false;
            }));
            
        }
        private void Function_藥品資料_藥檔資料_匯出()
        {
            saveFileDialog_SaveExcel.OverwritePrompt = false;
            if (saveFileDialog_SaveExcel.ShowDialog(this) == DialogResult.OK)
            {
                DataTable datatable = new DataTable();
                datatable = sqL_DataGridView_藥品資料_藥檔資料.GetDataTable();
                datatable = datatable.ReorderTable(new enum_藥品資料_藥檔資料_匯出());
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
        private void Function_藥品資料_藥檔資料_匯入()
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
                DataTable datatable_buf = dataTable.ReorderTable(new enum_藥品資料_藥檔資料_匯入());
                //datatable_buf = dataTable.ReorderTable(new enum_藥品資料_藥檔資料());
                if (datatable_buf == null)
                {
                    MyMessageBox.ShowDialog("匯入檔案,資料錯誤!");
                    this.Cursor = Cursors.Default;
                    return;
                }
                List<object[]> list_LoadValue = datatable_buf.DataTableToRowList();
                List<object[]> list_SQL_Value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
                List<object[]> list_Add = new List<object[]>();
                List<object[]> list_Delete_ColumnName = new List<object[]>();
                List<object[]> list_Delete_SerchValue = new List<object[]>();
                List<string> list_Replace_SerchValue = new List<string>();
                List<object[]> list_Replace_Value = new List<object[]>();
                List<object[]> list_SQL_Value_buf = new List<object[]>();

                for (int i = 0; i < list_LoadValue.Count; i++)
                {
                    object[] value_load = new object[new enum_藥品資料_藥檔資料().GetLength()];
                    value_load[(int)enum_藥品資料_藥檔資料.藥品碼] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.藥碼].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.藥品名稱] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.藥名].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.藥品學名] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.藥品學名].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.管制級別] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.管制級別].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.廠牌] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.廠牌].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.中文名稱] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.中文名].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.健保碼] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.健保碼].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.包裝單位] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.包裝單位].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.藥品許可證號] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.藥品許可證號].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.高價藥品] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.高價藥品].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.警訊藥品] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.警訊藥品].ObjectToString();
                    value_load[(int)enum_藥品資料_藥檔資料.類別] = list_LoadValue[i][(int)enum_藥品資料_藥檔資料_匯入.類別].ObjectToString();
                    medClass medClass = new medClass();

                    if (!Function_藥品資料_藥檔資料_檢查內容(value_load).StringIsEmpty()) continue;
                    list_SQL_Value_buf = list_SQL_Value.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, value_load[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString());
                    if (list_SQL_Value_buf.Count > 0)
                    {
                        object[] value_SQL = list_SQL_Value_buf[0];
                        value_load[(int)enum_藥品資料_藥檔資料.GUID] = value_SQL[(int)enum_藥品資料_藥檔資料.GUID];
                        bool flag_Equal = value_load.IsEqual(value_SQL);
                        if (!flag_Equal)
                        {
                            list_Replace_SerchValue.Add(value_load[(int)enum_藥品資料_藥檔資料.GUID].ObjectToString());
                            list_Replace_Value.Add(value_load);
                        }
                    }
                    else
                    {
                        value_load[(int)enum_藥品資料_藥檔資料.GUID] = Guid.NewGuid().ToString();
                        list_Add.Add(value_load);
                    }
                }
     
                this.sqL_DataGridView_藥品資料_藥檔資料.SQL_AddRows(list_Add, false);
                this.sqL_DataGridView_藥品資料_藥檔資料.SQL_ReplaceExtra(list_Replace_Value, false);
                this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(true);
                this.Cursor = Cursors.Default;
            }
            this.Cursor = Cursors.Default;
            MyMessageBox.ShowDialog("匯入完成!");
        }
        private void Function_藥品資料_藥檔資料_搜尋BarCode(string BarCode)
        {
            List<medClass> medClasses = medClass.serch_by_BarCode(Main_Form.API_Server, BarCode);
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
        private object[] Function_藥品資料_藥檔資料_檢查藥品設定表(string 藥品碼)
        {
            List<object[]> list_藥品設定表 = this.sqL_DataGridView_藥品設定表.SQL_GetRows((int)enum_medConfig.藥碼, 藥品碼, false);
            object[] value;
            if (list_藥品設定表.Count == 0)
            {
                value = new object[new enum_medConfig().GetLength()];
                value[(int)enum_medConfig.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_medConfig.藥碼] = 藥品碼;
                value[(int)enum_medConfig.效期管理] = false.ToString();
                value[(int)enum_medConfig.盲盤] = false.ToString();
                value[(int)enum_medConfig.複盤] = false.ToString();
                value[(int)enum_medConfig.結存報表] = false.ToString();
                value[(int)enum_medConfig.雙人覆核] = false.ToString();
                value[(int)enum_medConfig.麻醉藥品] = false.ToString();
                value[(int)enum_medConfig.形狀相似] = false.ToString();
                value[(int)enum_medConfig.發音相似] = false.ToString();
                value[(int)enum_medConfig.自定義] = false.ToString();
                this.sqL_DataGridView_藥品設定表.SQL_AddRow(value, false);
            }
            else
            {
                value = list_藥品設定表[0];
            }
            return value;
        }
        private void Function_藥品資料_藥檔資料_下載未建置條碼藥品()
        {

            List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<medClass> medClasses = list_value.SQLToClass<medClass, enum_雲端藥檔>();
            List<medClass> medClasses_buf = new List<medClass>();
            for (int i = 0; i < medClasses.Count; i++)
            {
                if (medClasses[i].Barcode.Count == 0)
                {
                    medClasses_buf.Add(medClasses[i]);
                }
            }
            list_value_buf = medClasses_buf.ClassToSQL<medClass, enum_藥品資料_藥檔資料_未建置藥品條碼_匯出>();
            saveFileDialog_SaveExcel.OverwritePrompt = false;
            if (saveFileDialog_SaveExcel.ShowDialog(this) == DialogResult.OK)
            {
                DataTable datatable = new DataTable();
                datatable = list_value_buf.ToDataTable(new enum_藥品資料_藥檔資料_未建置藥品條碼_匯出());
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

            }
        }
        private void Function_藥品資料_藥檔資料_下載已建置條碼藥品()
        {
            List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<medClass> medClasses = list_value.SQLToClass<medClass, enum_雲端藥檔>();
            List<medClass> medClasses_buf = new List<medClass>();
            for (int i = 0; i < medClasses.Count; i++)
            {
                if (medClasses[i].Barcode.Count != 0)
                {
                    medClasses_buf.Add(medClasses[i]);
                }
            }
            for (int i = 0; i < medClasses_buf.Count; i++)
            {
                object[] value = new object[new enum_藥品資料_藥檔資料_已建置藥品條碼_匯出().GetLength()];
                value[(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯出.藥品碼] = medClasses_buf[i].藥品碼;
                value[(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯出.藥品名稱] = medClasses_buf[i].藥品名稱;
                value[(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯出.料號] = medClasses_buf[i].料號;
                for (int k = 0; k < medClasses_buf[i].Barcode.Count; k++)
                {
                    if (k == 0) value[(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯出.條碼1] = medClasses_buf[i].Barcode[k];
                    if (k == 1) value[(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯出.條碼2] = medClasses_buf[i].Barcode[k];
                    if (k == 2) value[(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯出.條碼3] = medClasses_buf[i].Barcode[k];
                    if (k == 3) value[(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯出.條碼4] = medClasses_buf[i].Barcode[k];
                }
                list_value_buf.Add(value);
            }
            saveFileDialog_SaveExcel.OverwritePrompt = false;
            if (saveFileDialog_SaveExcel.ShowDialog(this) == DialogResult.OK)
            {
                DataTable datatable = new DataTable();
                datatable = list_value_buf.ToDataTable(new enum_藥品資料_藥檔資料_已建置藥品條碼_匯出());
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

            }
        }
        private void Function_藥品資料_藥檔資料_上載條碼Excel表()
        {
            this.Invoke(new Action(delegate
            {
                if (openFileDialog_LoadExcel.ShowDialog(this) != DialogResult.OK) return;
                DataTable dataTable = MyOffice.ExcelClass.NPOI_LoadFile(openFileDialog_LoadExcel.FileName);
                DataTable dt_reorder = dataTable.ReorderTable(new enum_藥品資料_藥檔資料_已建置藥品條碼_匯入());

                if (dt_reorder == null)
                {
                    MyMessageBox.ShowDialog("讀取Excel表單失敗!");
                    return;
                }
                List<object[]> list_load = dt_reorder.DataTableToRowList();


                List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
                List<object[]> list_value_buf = new List<object[]>();
                List<medClass> medClasses = list_value.SQLToClass<medClass, enum_雲端藥檔>();
                List<medClass> medClasses_buf = new List<medClass>();
                List<medClass> medClasses_replace = new List<medClass>();
                List<string> list_barcode = new List<string>();
                List<string> list_barcode_buf = new List<string>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    for (int k = 0; k < medClasses[i].Barcode.Count; k++)
                    {
                        list_barcode.Add(medClasses[i].Barcode[k]);
                    }
                }
                for(int i = 0; i < list_load.Count; i++)
                {
                    string 藥品碼 = list_load[i][(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯入.藥品碼].ObjectToString();
                    string 條碼1 = list_load[i][(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯入.條碼1].ObjectToString();
                    string 條碼2 = list_load[i][(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯入.條碼2].ObjectToString();
                    string 條碼3 = list_load[i][(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯入.條碼3].ObjectToString();
                    string 條碼4 = list_load[i][(int)enum_藥品資料_藥檔資料_已建置藥品條碼_匯入.條碼4].ObjectToString();
                    medClasses_buf = (from temp in medClasses
                                      where temp.藥品碼 == 藥品碼
                                      select temp).ToList();
                    if(medClasses_buf.Count > 0)
                    {
                        bool flag_replace = false;
                        if (條碼1.StringIsEmpty() == false)
                        {
                            list_barcode_buf = (from temp in list_barcode
                                                where temp == 條碼1
                                                select temp).ToList();
                            if (list_barcode_buf.Count == 0)
                            {
                                medClasses_buf[0].Add_BarCode(條碼1);
                                list_barcode.Add(條碼1);
                                flag_replace = true;
                            }
                        }
                        if (條碼2.StringIsEmpty() == false)
                        {
                            list_barcode_buf = (from temp in list_barcode
                                                where temp == 條碼2
                                                select temp).ToList();
                            if (list_barcode_buf.Count == 0)
                            {
                                medClasses_buf[0].Add_BarCode(條碼2);
                                list_barcode.Add(條碼2);
                                flag_replace = true;
                            }
                        }
                        if (條碼3.StringIsEmpty() == false)
                        {
                            list_barcode_buf = (from temp in list_barcode
                                                where temp == 條碼3
                                                select temp).ToList();
                            if (list_barcode_buf.Count == 0)
                            {
                                medClasses_buf[0].Add_BarCode(條碼3);
                                list_barcode.Add(條碼3);
                                flag_replace = true;
                            }
                        }
                        if (條碼4.StringIsEmpty() == false)
                        {
                            list_barcode_buf = (from temp in list_barcode
                                                where temp == 條碼4
                                                select temp).ToList();
                            if (list_barcode_buf.Count == 0)
                            {
                                medClasses_buf[0].Add_BarCode(條碼4);
                                list_barcode.Add(條碼4);
                                flag_replace = true;
                            }
                        }
                        if (flag_replace) medClasses_replace.Add(medClasses_buf[0]);
                    }
                }

                List<object[]> list_雲端藥檔_replace = medClasses_replace.ClassToSQL<medClass, enum_雲端藥檔>();
                this.sqL_DataGridView_雲端藥檔.SQL_ReplaceExtra(list_雲端藥檔_replace, false);
                MyMessageBox.ShowDialog("上傳完成!"); 
            }));
         
            
        }
        private void Function_藥品資料_藥檔資料_顯示有儲位藥品()
        {
            List<object[]> list_藥品資料 = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_藥品資料_buf = new List<object[]>();
            List<object[]> list_value = new List<object[]>();

            List<Device> devices = this.Function_從SQL取得所有儲位();
            List<string> list_code = (from value in devices
                                      select value.Code).ToList().Distinct().ToList();
            for (int i = 0; i < list_code.Count; i++)
            {
                list_藥品資料_buf = list_藥品資料.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, list_code[i]);
                if (list_藥品資料_buf.Count > 0)
                {
                    list_value.Add(list_藥品資料_buf[0]);
                }
            }
            this.sqL_DataGridView_藥品資料_藥檔資料.RefreshGrid(list_value);
        }
        #endregion
        #region Event
        private void PlC_RJ_Button_藥檔資料_藥品群組_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_藥品群組 dialog_藥品群組 = new Dialog_藥品群組();
            dialog_藥品群組.ShowDialog();
        }
        private void PlC_RJ_Button_藥品資料_條碼管理_MouseDownEvent(MouseEventArgs mevent)
        {
            string 藥品碼 = this.textBox_藥品資料_藥檔資料_藥品碼.Text;
            if (藥品碼.StringIsEmpty()) return;
            Dialog_條碼管理 dialog_條碼管理 = new Dialog_條碼管理(藥品碼);
            dialog_條碼管理.ShowDialog();
        }
        private void SqL_DataGridView_藥品資料_藥檔資料_ComboBoxSelectedIndexChangedEvent(object sender, string colName, object[] RowValue)
        {
            if (colName == enum_藥品資料_藥檔資料.開檔狀態.GetEnumName())
            {
                string Code = RowValue[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
                string 開檔狀態 = RowValue[(int)enum_藥品資料_藥檔資料.開檔狀態].ObjectToString();
            
                this.sqL_DataGridView_藥品資料_藥檔資料.SQL_ReplaceExtra(RowValue, false);
                this.sqL_DataGridView_藥品資料_藥檔資料.ReplaceExtra(RowValue, true);

                medClass _medclass = medClass.get_med_clouds_by_code(API_Server, Code);
                if (_medclass == null) return;
                _medclass.開檔狀態 = 開檔狀態;
                medClass.add_med_clouds(API_Server, _medclass);
            }
        }
        private void SqL_DataGridView_藥品資料_藥檔資料_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_藥品資料_藥檔資料());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_藥品資料_藥檔資料.匯出.GetEnumName())
                    {
                        Function_藥品資料_藥檔資料_匯出();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_藥品資料_藥檔資料.匯入.GetEnumName())
                    {
                        Function_藥品資料_藥檔資料_匯入();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_藥品資料_藥檔資料.更新至線上藥檔.GetEnumName())
                    {
                        List<object[]> list_value = sqL_DataGridView_藥品資料_藥檔資料.Get_All_Select_RowsValues();
                        List<object[]> list_value_upload = new List<object[]>();
                        if (list_value.Count == 0)
                        {
                            MyMessageBox.ShowDialog("未選取資料!");
                            return;
                        }
                        List<medClass> medClasses = list_value.SQLToClass<medClass , enum_藥品資料_藥檔資料>();
                        list_value_upload = medClasses.ClassToSQL<medClass, enum_雲端藥檔>();

                        List<object[]> list_雲端藥檔 = sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
                        List<object[]> list_雲端藥檔_buf = new List<object[]>();
                        List<object[]> list_雲端藥檔_Add = new List<object[]>();
                        List<object[]> list_雲端藥檔_Replaced = new List<object[]>();
                        for (int i = 0; i < list_value_upload.Count; i++)
                        {
                            string 藥品碼 = list_value_upload[i][(int)enum_雲端藥檔.藥品碼].ObjectToString();
                            list_雲端藥檔_buf = list_雲端藥檔.GetRows((int)enum_雲端藥檔.藥品碼, 藥品碼);
                            list_value_upload[i][(int)enum_雲端藥檔.中西藥] = "西藥";
                            if (list_雲端藥檔_buf.Count == 0)
                            {
                                list_value_upload[i][(int)enum_雲端藥檔.GUID] = Guid.NewGuid().ToString();
                                list_雲端藥檔_Add.Add(list_value_upload[i]);

                            }
                            else
                            {
                                list_value_upload[i][(int)enum_雲端藥檔.GUID] = Guid.NewGuid().ToString();
                                list_雲端藥檔_Replaced.Add(list_value_upload[i]);
                            }
                        }
                        sqL_DataGridView_雲端藥檔.SQL_AddRows(list_雲端藥檔_Add, false);
                        sqL_DataGridView_雲端藥檔.SQL_ReplaceExtra(list_雲端藥檔_Replaced, false);
                        MyMessageBox.ShowDialog($"新增<{list_雲端藥檔_Add.Count}>筆資料,修改<{list_雲端藥檔_Replaced.Count}>筆資料");

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_藥品資料_藥檔資料.下載未建置條碼藥品.GetEnumName())
                    {
                        this.Function_藥品資料_藥檔資料_下載未建置條碼藥品();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_藥品資料_藥檔資料.下載已建置條碼藥品.GetEnumName())
                    {
                        this.Function_藥品資料_藥檔資料_下載已建置條碼藥品();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_藥品資料_藥檔資料.上載條碼Excel表.GetEnumName())
                    {
                        this.Function_藥品資料_藥檔資料_上載條碼Excel表();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_藥品資料_藥檔資料.顯示有儲位藥品.GetEnumName())
                    {
                        this.Function_藥品資料_藥檔資料_顯示有儲位藥品();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_藥品資料_藥檔資料.藥檔維護.GetEnumName())
                    {
                        Dialog_藥檔維護 dialog_藥檔維護 = new Dialog_藥檔維護();
                        dialog_藥檔維護.ShowDialog();

                    }
                }
            }
        }
        private void SqL_DataGridView_藥品資料_藥檔資料_RowEnterEvent(object[] RowValue)
        {
   
        }
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
            this.textBox_藥品資料_藥檔資料_廠牌.Text = RowValue[(int)enum_藥品資料_藥檔資料.廠牌].ObjectToString();
            this.textBox_藥品資料_藥檔資料_許可證號.Text = RowValue[(int)enum_藥品資料_藥檔資料.藥品許可證號].ObjectToString();
            this.plC_CheckBox_藥品資料_藥檔資料_警訊藥品.Checked = (RowValue[(int)enum_藥品資料_藥檔資料.警訊藥品].ObjectToString() == true.ToString());
            this.plC_CheckBox_藥品資料_藥檔資料_高價藥品.Checked = (RowValue[(int)enum_藥品資料_藥檔資料.高價藥品].ObjectToString() == true.ToString());
            this.plC_CheckBox_藥品資料_藥檔資料_生物製劑.Checked = (RowValue[(int)enum_藥品資料_藥檔資料.生物製劑].ObjectToString() == true.ToString());

            
            this.comboBox_藥品資料_藥檔資料_管制級別.Texts = RowValue[(int)enum_藥品資料_藥檔資料.管制級別].ObjectToString();

            medConfigClass medConfigClass = medConfigClass.get_by_codes(API_Server, 藥品碼);

            if (medConfigClass == null)
            {
                this.plC_CheckBox_藥品資料_藥檔資料_自定義設定.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_效期管理.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_複盤.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_盲盤.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_結存報表.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_雙人覆核.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_麻醉藥品.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_形狀相似.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_發音相似.Checked = false;
                this.plC_CheckBox_藥品資料_藥檔資料_調劑註記.Checked = false;
            }
            else
            {
                object[] value = medConfigClass.ClassToSQL<medConfigClass,enum_medConfig>();

                this.plC_CheckBox_藥品資料_藥檔資料_自定義設定.Checked = value[(int)enum_medConfig.自定義].ObjectToString().StringToBool();
                this.plC_CheckBox_藥品資料_藥檔資料_效期管理.Checked = value[(int)enum_medConfig.效期管理].ObjectToString().StringToBool();
                this.plC_CheckBox_藥品資料_藥檔資料_複盤.Checked = value[(int)enum_medConfig.複盤].ObjectToString().StringToBool();
                this.plC_CheckBox_藥品資料_藥檔資料_盲盤.Checked = value[(int)enum_medConfig.盲盤].ObjectToString().StringToBool();
                this.plC_CheckBox_藥品資料_藥檔資料_結存報表.Checked = value[(int)enum_medConfig.結存報表].ObjectToString().StringToBool();
                this.plC_CheckBox_藥品資料_藥檔資料_雙人覆核.Checked = value[(int)enum_medConfig.雙人覆核].ObjectToString().StringToBool();
                this.plC_CheckBox_藥品資料_藥檔資料_麻醉藥品.Checked = value[(int)enum_medConfig.麻醉藥品].ObjectToString().StringToBool();
                this.plC_CheckBox_藥品資料_藥檔資料_形狀相似.Checked = value[(int)enum_medConfig.形狀相似].ObjectToString().StringToBool();
                this.plC_CheckBox_藥品資料_藥檔資料_發音相似.Checked = value[(int)enum_medConfig.發音相似].ObjectToString().StringToBool();
                this.plC_CheckBox_藥品資料_藥檔資料_調劑註記.Checked = value[(int)enum_medConfig.調劑註記].ObjectToString().StringToBool();

            }



        }
        private void SqL_DataGridView_藥品資料_藥檔資料_DataGridRowsChangeEvent(List<object[]> RowsList)
        {
            for (int i = 0; i < RowsList.Count; i++)
            {
                RowsList[i][(int)enum_藥品資料_藥檔資料.庫存] = this.Function_從本地資料取得庫存(RowsList[i][(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString()).ToString();
                if (RowsList[i][(int)enum_藥品資料_藥檔資料.庫存].ObjectToString() == "-999") RowsList[i][(int)enum_藥品資料_藥檔資料.庫存] = "0";
                if (RowsList[i][(int)enum_藥品資料_藥檔資料.管制級別].ObjectToString().StringIsEmpty())
                {
                    RowsList[i][(int)enum_藥品資料_藥檔資料.管制級別] = "N";
                }
               
            }
            if (!plC_CheckBox_藥品資料_開檔中.Checked)
            {
                RowsList.RemoveAll(row =>
                    row[(int)enum_藥品資料_藥檔資料.開檔狀態].ObjectToString() == enum_開檔狀態.開檔中.GetEnumName());
            }
            if (!plC_CheckBox_藥品資料_關檔中.Checked)
            {
                RowsList.RemoveAll(row =>
                    row[(int)enum_藥品資料_藥檔資料.開檔狀態].ObjectToString() == enum_開檔狀態.關檔中.GetEnumName());
            }
            if (!plC_CheckBox_藥品資料_未設定.Checked)
            {
                RowsList.RemoveAll(row =>
                    row[(int)enum_藥品資料_藥檔資料.開檔狀態].ObjectToString().StringIsEmpty());
            }
            RowsList.Sort(new Icp_藥品資料_藥檔資料());
        }
        private void sqL_DataGridView_藥品資料_藥檔資料_DataGridRefreshEvent()
        {
            for (int i = 0; i < this.sqL_DataGridView_藥品資料_藥檔資料.dataGridView.Rows.Count; i++)
            {
                if (this.sqL_DataGridView_藥品資料_藥檔資料.dataGridView.Rows[i].Cells[enum_藥品資料_藥檔資料.安全庫存.GetEnumName()].Value.ToString().StringToDouble() != 0)
                {
                    if (this.sqL_DataGridView_藥品資料_藥檔資料.dataGridView.Rows[i].Cells[enum_藥品資料_藥檔資料.庫存.GetEnumName()].Value.ToString().StringToDouble() < this.sqL_DataGridView_藥品資料_藥檔資料.dataGridView.Rows[i].Cells[enum_藥品資料_藥檔資料.安全庫存.GetEnumName()].Value.ToString().StringToDouble())
                    {
                        this.sqL_DataGridView_藥品資料_藥檔資料.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                        this.sqL_DataGridView_藥品資料_藥檔資料.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void TextBox_藥品資料_藥檔資料_藥品碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_藥品資料_HIS填入_MouseDownEvent(null);
            }
        }
        private void TextBox_藥品資料_藥檔資料_資料查詢_藥品條碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_藥品資料_藥品條碼_搜尋_MouseDownEvent(null);
            }
        }

        private void PlC_RJ_Button_藥品資料_顯示全部_MouseDownEvent(MouseEventArgs mevent)
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
            if(textBox_藥品資料_藥檔資料_資料查詢_藥品名稱.Text.StringIsEmpty())
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

        private void PlC_RJ_Button_藥品資料_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (MyMessageBox.ShowDialog("是否刪除選取資料", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;
                    List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.Get_All_Checked_RowsValues();
                    List<object> list_delete_serchValue = new List<object>();
                    for (int i = 0; i < list_value.Count; i++)
                    {
                        string GUID = list_value[i][(int)enum_藥品資料_藥檔資料.GUID].ObjectToString();
                        string 藥品碼 = list_value[i][(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
                        if(Function_從SQL取得儲位到本地資料(藥品碼).Count > 0)
                        {
                            //MyMessageBox.ShowDialog("刪除藥品有建立儲位,無法刪除!");
                            //return;
                            continue;
                        }

                        list_delete_serchValue.Add(GUID);
                    }
                    this.sqL_DataGridView_藥品資料_藥檔資料.SQL_DeleteExtra(enum_藥品資料_藥檔資料.GUID.GetEnumName(), list_delete_serchValue, true);
                   
                }
                this.Cursor = Cursors.Default;
            }));
           
        }
        private void PlC_RJ_Button_藥品資料_登錄_MouseDownEvent(MouseEventArgs mevent)
        {
            LoadingForm.ShowLoadingForm();
            try
            {
                Function_藥品資料_藥檔資料_登錄();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
            }
            finally
            {

            }
   
            LoadingForm.CloseLoadingForm();
        }
        private void PlC_RJ_Button_藥品資料_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Function_藥品資料_藥檔資料_匯出();
            }));
        }
        private void PlC_RJ_Button_藥品資料_匯入_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Function_藥品資料_藥檔資料_匯入();
            }));
        }
        private void PlC_RJ_Button_藥品資料_更新藥櫃資料_MouseDownEvent(MouseEventArgs mevent)
        {
            List<medClass> medClasses_本地藥檔 = medClass.get_dps_medClass(Main_Form.API_Server, Main_Form.ServerName);
            List<object[]> list_本地藥檔 = medClasses_本地藥檔.ClassToSQL<medClass, enum_藥品資料_藥檔資料>();
            List<object[]> list_本地藥檔_replace = new List<object[]>();
            string url = dBConfigClass.MedApiURL;
            if (!url.StringIsEmpty())
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                string response = Basic.Net.WEBApiGet($"{url}?Code");
                if (response == "OK")
                {
              
           
                    Console.WriteLine($"HIS填入成功! , response:{response},耗時{myTimer.ToString()}ms");
                }
                else 
                {
                    Console.WriteLine($"HIS填入失敗! , response:{response},耗時{myTimer.ToString()}ms");
                }
            }
            List<medClass> medClasses_雲端藥檔 = medClass.get_med_cloud(Main_Form.API_Server);

            List<object[]> list_雲端藥檔 = medClasses_雲端藥檔.ClassToSQL<medClass, enum_雲端藥檔>();
            List<object[]> list_雲端藥檔_buf = new List<object[]>();
            
            for (int i = 0; i < list_本地藥檔.Count; i++)
            {
                string 藥品碼 = list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
                list_雲端藥檔_buf = list_雲端藥檔.GetRows((int)enum_雲端藥檔.藥品碼, 藥品碼);
                if (list_雲端藥檔_buf.Count > 0)
                {
                    bool replace = false;
                    if (list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.藥品名稱].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_雲端藥檔.藥品名稱].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.中文名稱].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_雲端藥檔.中文名稱].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.藥品學名].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_雲端藥檔.藥品學名].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.包裝單位].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_雲端藥檔.包裝數量].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.警訊藥品].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_雲端藥檔.警訊藥品].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.管制級別].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_雲端藥檔.管制級別].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.開檔狀態].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_雲端藥檔.開檔狀態].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.類別].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_雲端藥檔.類別].ObjectToString()) replace = true;

                    list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.藥品名稱] = list_雲端藥檔_buf[0][(int)enum_雲端藥檔.藥品名稱];
                    list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.藥品學名] = list_雲端藥檔_buf[0][(int)enum_雲端藥檔.藥品學名];
                    list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.中文名稱] = list_雲端藥檔_buf[0][(int)enum_雲端藥檔.中文名稱];
                    list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.包裝單位] = list_雲端藥檔_buf[0][(int)enum_雲端藥檔.包裝單位];
                    list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.藥品條碼] = list_雲端藥檔_buf[0][(int)enum_雲端藥檔.藥品條碼1];
                    list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.警訊藥品] = list_雲端藥檔_buf[0][(int)enum_雲端藥檔.警訊藥品];
                    list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.管制級別] = list_雲端藥檔_buf[0][(int)enum_雲端藥檔.管制級別];
                    list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.開檔狀態] = list_雲端藥檔_buf[0][(int)enum_雲端藥檔.開檔狀態];
                    list_本地藥檔[i][(int)enum_藥品資料_藥檔資料.類別] = list_雲端藥檔_buf[0][(int)enum_雲端藥檔.類別];
                    if (replace)
                    {
                        list_本地藥檔_replace.Add(list_本地藥檔[i]);
                    }

                }
            }
            if (list_本地藥檔_replace.Count == 0)
            {
                MyMessageBox.ShowDialog("無資料更新!");
                return;
            }
            this.sqL_DataGridView_藥品資料_藥檔資料.SQL_ReplaceExtra(list_本地藥檔_replace, false);
            MyMessageBox.ShowDialog("更新完成...");
        }
        private void PlC_RJ_Button_藥品資料_HIS填入_MouseDownEvent(MouseEventArgs mevent)
        {
            string 藥品碼 = this.textBox_藥品資料_藥檔資料_藥品碼.Text;
            string url = dBConfigClass.MedApiURL;
            if (!url.StringIsEmpty())
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                string response = Basic.Net.WEBApiGet($"{url}?Code={藥品碼}");
                Console.WriteLine($"HIS填入 , response:{response},耗時{myTimer.ToString()}ms");
            }
            List<medClass> medClasses = medClass.get_med_cloud(Main_Form.API_Server);
          
            if (medClasses.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
                return;
            }
            this.textBox_藥品資料_藥檔資料_藥品名稱.Text = medClasses[0].藥品名稱;
            this.textBox_藥品資料_藥檔資料_藥品學名.Text = medClasses[0].藥品學名;
            this.textBox_藥品資料_藥檔資料_中文名稱.Text = medClasses[0].中文名稱;
            this.textBox_藥品資料_藥檔資料_健保碼.Text = medClasses[0].健保碼;
            this.textBox_藥品資料_藥檔資料_包裝單位.Text = medClasses[0].包裝單位;
            this.plC_CheckBox_藥品資料_藥檔資料_警訊藥品.Checked = (medClasses[0].警訊藥品.ToLower() == "true");
            this.comboBox_藥品資料_藥檔資料_管制級別.Text = medClasses[0].管制級別;
        }
        private void PlC_RJ_Button_藥品資料_新藥建置_MouseDownEvent(MouseEventArgs mevent)
        {
            LoadingForm.ShowLoadingForm();
            try
            {
                string 藥品碼 = this.textBox_藥品資料_藥檔資料_藥品碼.Text;
                if (藥品碼.StringIsEmpty())
                {
                    MyMessageBox.ShowDialog("請輸入藥品碼");
                    return;
                }
                string url = dBConfigClass.MedApiURL;
                if (!url.StringIsEmpty())
                {
                    MyTimer myTimer = new MyTimer();
                    myTimer.StartTickTime(50000);
                    string response = Basic.Net.WEBApiGet($"{url}?Code={藥品碼}");
                    Console.WriteLine($"HIS填入 , response:{response},耗時{myTimer.ToString()}ms");
                }
                medClass _medClass = medClass.get_med_clouds_by_code(Main_Form.API_Server, 藥品碼);

                if (_medClass == null)
                {
                    MyMessageBox.ShowDialog("查無資料!");
                    return;
                }
                this.textBox_藥品資料_藥檔資料_藥品名稱.Text = _medClass.藥品名稱;
                this.textBox_藥品資料_藥檔資料_藥品學名.Text = _medClass.藥品學名;
                this.textBox_藥品資料_藥檔資料_中文名稱.Text = _medClass.中文名稱;
                this.textBox_藥品資料_藥檔資料_健保碼.Text = _medClass.健保碼;
                this.textBox_藥品資料_藥檔資料_包裝單位.Text = _medClass.包裝單位;
                this.plC_CheckBox_藥品資料_藥檔資料_警訊藥品.Checked = (_medClass.警訊藥品.ToLower() == "true");
                this.comboBox_藥品資料_藥檔資料_管制級別.Text = _medClass.管制級別;

                PlC_RJ_Button_藥品資料_登錄_MouseDownEvent(null);
                if (mevent == null) return;
                MyMessageBox.ShowDialog($"藥碼:{_medClass.藥品碼}\n藥名:{_medClass.藥品名稱}\n建置完成!");
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
          
        }
        private void PlC_RJ_Button_藥品資料_HIS下載全部藥檔_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("是否下載全部藥檔?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;

            Dialog_中西藥選擇 dialog_中西藥選擇 = new Dialog_中西藥選擇();
            if (dialog_中西藥選擇.ShowDialog() != DialogResult.Yes) return;


            List<object[]> list_雲端藥檔_all = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            List<object[]> list_雲端藥檔_中藥 = list_雲端藥檔_all.GetRows((int)enum_雲端藥檔.中西藥, "中藥");
            List<object[]> list_雲端藥檔_西藥 = list_雲端藥檔_all.GetRows((int)enum_雲端藥檔.中西藥, "西藥");
            List<object[]> list_雲端藥檔 = new List<object[]>();
            if (dialog_中西藥選擇.flag_中藥) list_雲端藥檔.LockAdd(list_雲端藥檔_中藥);
            if (dialog_中西藥選擇.flag_西藥) list_雲端藥檔.LockAdd(list_雲端藥檔_西藥);

            Console.WriteLine($"取得西藥藥品<{list_雲端藥檔_西藥.Count}>筆,中藥藥品<{list_雲端藥檔_中藥.Count}>筆");

            List<object[]> list_雲端藥檔_buf = new List<object[]>();
            List<object[]> list_藥品資料 = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_藥品資料_buf = new List<object[]>();
            List<object[]> list_藥品資料_add = new List<object[]>();
            List<object[]> list_藥品資料_replace = new List<object[]>();
            List<object[]> list_藥品資料_delete = new List<object[]>();
            Dialog_Prcessbar dialog_Prcessbar = new Dialog_Prcessbar(list_雲端藥檔.Count);
            dialog_Prcessbar.State = $"開始比對線上藥檔...";
            string url = dBConfigClass.MedApiURL;

            if (!url.StringIsEmpty())
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                string response = Basic.Net.WEBApiGet($"{url}");
                Console.WriteLine($"HIS下載全部藥檔 , response:{response},耗時{myTimer.ToString()}ms");
            }
            
            for (int i = 0; i < list_雲端藥檔.Count; i++)
            {
                dialog_Prcessbar.Value = i;
                list_藥品資料_buf = list_藥品資料.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, list_雲端藥檔[i][(int)enum_雲端藥檔.藥品碼].ObjectToString());
                if(list_藥品資料_buf.Count == 0)
                {
                    object[] value = new object[new enum_藥品資料_藥檔資料().GetLength()];

                    value[(int)enum_藥品資料_藥檔資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品碼].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品名稱] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品名稱].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品學名] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品學名].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.中文名稱] = list_雲端藥檔[i][(int)enum_雲端藥檔.中文名稱].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品條碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品條碼1].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.健保碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.健保碼].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.包裝單位] = list_雲端藥檔[i][(int)enum_雲端藥檔.包裝單位].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.庫存] = "0";
                    value[(int)enum_藥品資料_藥檔資料.安全庫存] = "0";        
                    value[(int)enum_藥品資料_藥檔資料.警訊藥品] = list_雲端藥檔[i][(int)enum_雲端藥檔.警訊藥品].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.管制級別] = list_雲端藥檔[i][(int)enum_雲端藥檔.管制級別].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.類別] = list_雲端藥檔[i][(int)enum_雲端藥檔.類別].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.開檔狀態] = list_雲端藥檔[i][(int)enum_雲端藥檔.開檔狀態].ObjectToString();

                    list_藥品資料_add.Add(value);
                }
                else
                {
                    object[] value = list_藥品資料_buf[0];

                    value[(int)enum_藥品資料_藥檔資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品碼].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品名稱] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品名稱].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品學名] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品學名].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.中文名稱] = list_雲端藥檔[i][(int)enum_雲端藥檔.中文名稱].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.藥品條碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.藥品條碼1].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.健保碼] = list_雲端藥檔[i][(int)enum_雲端藥檔.健保碼].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.包裝單位] = list_雲端藥檔[i][(int)enum_雲端藥檔.包裝單位].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.警訊藥品] = list_雲端藥檔[i][(int)enum_雲端藥檔.警訊藥品].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.管制級別] = list_雲端藥檔[i][(int)enum_雲端藥檔.管制級別].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.類別] = list_雲端藥檔[i][(int)enum_雲端藥檔.類別].ObjectToString();
                    value[(int)enum_藥品資料_藥檔資料.開檔狀態] = list_雲端藥檔[i][(int)enum_雲端藥檔.開檔狀態].ObjectToString();

                    list_藥品資料_replace.Add(value);
                }
            }

            dialog_Prcessbar.State = $"寫入資料庫...";
            if (list_藥品資料_add.Count > 0) this.sqL_DataGridView_藥品資料_藥檔資料.SQL_AddRows(list_藥品資料_add, false);
            if (list_藥品資料_replace.Count > 0) this.sqL_DataGridView_藥品資料_藥檔資料.SQL_ReplaceExtra(list_藥品資料_replace, false);
            dialog_Prcessbar.Close();
        }
        private void PlC_RJ_Button_藥品資料_批次設定_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_藥品資料_批次設定 dialog_藥品資料_批次設定 = new Dialog_藥品資料_批次設定();
            dialog_藥品資料_批次設定.ShowDialog();
        }
        private void PlC_CheckBox_藥品資料_藥檔資料_結存報表_CheckedChanged(object sender, EventArgs e)
        {
          
        }
        private void PlC_CheckBox_藥品資料_藥檔資料_複盤_CheckedChanged(object sender, EventArgs e)
        {
            if (plC_CheckBox_藥品資料_藥檔資料_複盤.Checked)
            {
                if (this.plC_CheckBox_藥品資料_藥檔資料_盲盤.Checked) this.plC_CheckBox_藥品資料_藥檔資料_盲盤.Checked = false;
            }        
        }
        private void PlC_CheckBox_藥品資料_藥檔資料_盲盤_CheckedChanged(object sender, EventArgs e)
        {
            if (plC_CheckBox_藥品資料_藥檔資料_盲盤.Checked)
            {
                if (this.plC_CheckBox_藥品資料_藥檔資料_複盤.Checked) this.plC_CheckBox_藥品資料_藥檔資料_複盤.Checked = false;
            }        
        }
        private void PlC_CheckBox_藥品資料_藥檔資料_效期管理_CheckedChanged(object sender, EventArgs e)
        {
           
        }
        private void PlC_CheckBox_藥品資料_藥檔資料_自定義設定_CheckedChanged(object sender, EventArgs e)
        {

        }
        #endregion
        public class Icp_藥品資料_藥檔資料 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string 藥品碼_0 = x[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
                string 藥品碼_1 = y[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
                return 藥品碼_0.CompareTo(藥品碼_1);
            }
        }
        public class Icp_藥品群組 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                int index_0 = x[(int)enum_medGroup.群組序號].ObjectToString().StringToInt32();
                int index_1 = y[(int)enum_medGroup.群組序號].ObjectToString().StringToInt32();
                return index_0.CompareTo(index_1);
            }
        }
    }
}
