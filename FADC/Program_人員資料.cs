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
using FingerprintLib;

namespace FADC
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

            Table table = personPageClass.Init(API_Server);
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
            this.sqL_DataGridView_人員資料.RowDoubleClickEvent += SqL_DataGridView_人員資料_RowDoubleClickEvent;
            this.sqL_DataGridView_人員資料.MouseDown += SqL_DataGridView_人員資料_MouseDown;
            this.sqL_DataGridView_人員資料.RefreshGrid(Function_人員資料_取得人員資料().ClassToSQL<personPageClass, enum_人員資料>());

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
            this.plC_RJ_Button_人員資料_人臉註冊.MouseDownEvent += PlC_RJ_Button_人員資料_人臉註冊_MouseDownEvent;

            this.plC_UI_Init.Add_Method(this.sub_Program_人員資料);


        }

   

        bool flag_人員資料_權限管理_頁面更新 = false;
        private void sub_Program_人員資料()
        {
         
            if (this.plC_ScreenPage_Main.PageText == "人員資料" )
            {
                if (!this.flag_人員資料_權限管理_頁面更新)
                {
  
                    this.Invoke(new Action(delegate
                    {
                        PLC_Device pLC_Device = new PLC_Device("S39014");
                        this.sqL_DataGridView_人員資料.RefreshGrid(Function_人員資料_取得人員資料().ClassToSQL<personPageClass, enum_人員資料>());
                        this.comboBox_人員資料_權限等級.Enabled = pLC_Device.Bool;
                    }));
                    this.flag_人員資料_權限管理_頁面更新 = true;
                }
            }
            else
            {
                this.flag_人員資料_權限管理_頁面更新 = false;
            }
            //this.sub_Program_人員資料_接收設備資料();
        }

        #region Function
        private List<personPageClass> Function_人員資料_取得人員資料()
        {
            List<personPageClass> personPageClasses = new List<personPageClass>();

            personPageClasses =  personPageClass.get_all(API_Server);

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
                else if (Extension == ".xls" || Extension == ".xlsx")
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
  
  
   
        private void Function_登入權限資料_取得權限(List<PermissionsClass> Permissions)
        {
            Permissions = (from temp in Permissions
                           where temp.類別.Contains("FADC")
                           select temp).ToList();
            for (int i = 0; i < Permissions.Count; i++)
            {

                if (Permissions[i].狀態)
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

        private void SqL_DataGridView_人員資料_DataGridRefreshEvent()
        {
            for (int i = 0; i < this.sqL_DataGridView_人員資料.dataGridView.Rows.Count; i++)
            {

                Color color = this.sqL_DataGridView_人員資料.dataGridView.Rows[i].Cells[enum_人員資料.顏色.GetEnumName()].Value.ObjectToString().ToColor();
                this.sqL_DataGridView_人員資料.dataGridView.Rows[i].Cells[enum_人員資料.顏色.GetEnumName()].Style.BackColor = color;
                this.sqL_DataGridView_人員資料.dataGridView.Rows[i].Cells[enum_人員資料.顏色.GetEnumName()].Style.ForeColor = color;
            }
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

        }
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

                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string color_str = list_value[i][(int)enum_人員資料.顏色].ObjectToString();
                                if (color_str.ToColor() == Color.Black || color_str.StringIsEmpty())
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
            if (rJ_TextBox_人員資料_資料查詢_一維條碼.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("搜尋條件空白!");
                return;
            }
            string text = rJ_TextBox_人員資料_資料查詢_一維條碼.Text;
            List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
            list_value = (from temp in list_value
                          where temp[(int)enum_人員資料.一維條碼].ObjectToString().ToUpper().Contains(text.ToUpper())
                          select temp).ToList();
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
        private void PlC_RJ_Button_人員資料_人臉註冊_MouseDownEvent(MouseEventArgs mevent)
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
                string id = list_value[0][(int)enum_人員資料.ID].ObjectToString();
                Dialog_人臉註冊 dialog_人臉註冊 = new Dialog_人臉註冊(id);
                dialog_人臉註冊.ShowDialog();

            


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
        private void PlC_Button_人員資料_指紋註冊_MouseDownEvent(MouseEventArgs mevent)
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
                Dialog_HID指紋註冊 dialog_HID指紋註冊 = new Dialog_HID指紋註冊();
                if (dialog_HID指紋註冊.ShowDialog() != DialogResult.Yes) return;

                string fmd = dialog_HID指紋註冊.resultFmd.ToBase64();

                list_value[0][(int)enum_人員資料.指紋辨識] = fmd;
                this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(list_value[0], false);
                this.sqL_DataGridView_人員資料.ReplaceExtra(list_value[0], true);
                dialog_AlarmForm = new Dialog_AlarmForm("設定完成", 1500, Color.Green);
                dialog_AlarmForm.ShowDialog();


            }
            finally
            {
            }
          
        }
    }
     
}
