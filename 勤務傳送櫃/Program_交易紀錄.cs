using System;
using System.IO;
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
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Runtime.InteropServices;
using MyOffice;
using HIS_DB_Lib;
namespace 勤務傳送系統
{
    public partial class Main_Form : Form
    {
        PLC_RJ_Button plC_RJ_Button_交易記錄查詢_選取資料設定為已領用;
        PLC_RJ_Button plC_RJ_Button_交易記錄查詢_選取資料設定為未領用;

        public enum enum_交易記錄查詢資料_匯出
        {
            動作,
            領藥號,
            藥品碼,
            藥品名稱,
            交易量,
            操作人,
            領用人,
            病人姓名,
            病房號,
            病歷號,
            操作時間,
            領用時間,
            開方時間,
            備註,
        }

        public enum ContextMenuStrip_交易紀錄
        {
            [Description("M8000")]
            選取資料設定為已領用,
            [Description("M8000")]
            選取資料設定為未領用,

        }

        private void Program_交易紀錄_Init()
        {
            string url = $"{dBConfigClass.Api_URL}/api/transactions/init";
            returnData returnData = new returnData();
            returnData.ServerType = enum_sys_serverSetting_Type.傳送櫃.GetEnumName();
            returnData.ServerName = $"{dBConfigClass.Name}";
            string json_in = returnData.JsonSerializationt();
            string json = Basic.Net.WEBApiPostJson($"{url}", json_in);
            Table table = json.JsonDeserializet<Table>();
            if (table == null)
            {
                MyMessageBox.ShowDialog($"交易紀錄表單建立失敗!! Api_URL:{dBConfigClass.Api_URL}");
                return;
            }
            this.sqL_DataGridView_交易記錄查詢.Init(table);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnVisible(false, new enum_交易記錄查詢資料().GetEnumNames());

            this.sqL_DataGridView_交易記錄查詢.MouseDown += SqL_DataGridView_交易記錄查詢_MouseDown;

            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.動作);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(70, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.領藥號);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_交易記錄查詢資料.藥品碼);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(300, DataGridViewContentAlignment.MiddleLeft, enum_交易記錄查詢資料.藥品名稱);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleRight, enum_交易記錄查詢資料.交易量);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.操作人);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.領用人);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.病人姓名);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.病房號);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.病歷號);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(110, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.操作時間);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(110, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.領用時間);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(110, DataGridViewContentAlignment.MiddleCenter, enum_交易記錄查詢資料.開方時間);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_交易記錄查詢資料.備註);

            this.sqL_DataGridView_交易記錄查詢.Set_ColumnSortMode(DataGridViewColumnSortMode.Automatic, enum_交易記錄查詢資料.操作時間);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnSortMode(DataGridViewColumnSortMode.Automatic, enum_交易記錄查詢資料.領用時間);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnSortMode(DataGridViewColumnSortMode.Automatic, enum_交易記錄查詢資料.開方時間);
            this.sqL_DataGridView_交易記錄查詢.Set_ColumnSortMode(DataGridViewColumnSortMode.Automatic, enum_交易記錄查詢資料.領藥號);

            this.sqL_DataGridView_交易記錄查詢.DataGridRowsChangeRefEvent += SqL_DataGridView_交易記錄查詢_DataGridRowsChangeRefEvent;
            this.sqL_DataGridView_交易記錄查詢.DataGridRefreshEvent += SqL_DataGridView_交易記錄查詢_DataGridRefreshEvent;


            this.plC_RJ_Button_交易記錄查詢_顯示全部.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_顯示全部_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_刪除資料.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_刪除資料_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_藥品碼_搜尋.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_藥品碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_藥品名稱_搜尋.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_藥品名稱_搜尋_MouseDownEvent;
            this.plC_RJ_Button__交易記錄查詢_調劑人_搜尋.MouseDownEvent += PlC_RJ_Button__交易記錄查詢_調劑人_搜尋_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_領用人_搜尋.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_領用人_搜尋_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_操作時間_搜尋.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_操作時間_搜尋_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_開方時間_搜尋.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_開方時間_搜尋_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_領用時間_搜尋.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_領用時間_搜尋_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_病歷號_搜尋.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_病歷號_搜尋_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_病房號_搜尋.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_病房號_搜尋_MouseDownEvent;
            this.plC_RJ_Button_交易記錄查詢_匯出.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_匯出_MouseDownEvent;
            this.Program_交易紀錄_狀態操作按鈕_Init();

            this.plC_UI_Init.Add_Method(this.Program_交易紀錄);

        }

        private void Program_交易紀錄_狀態操作按鈕_Init()
        {
            plC_RJ_Button_交易記錄查詢_選取資料設定為已領用 = Program_交易紀錄_建立狀態操作按鈕("設為已領用", new Point(1335, 755), Color.SeaGreen);
            plC_RJ_Button_交易記錄查詢_選取資料設定為未領用 = Program_交易紀錄_建立狀態操作按鈕("設為未領用", new Point(1497, 755), Color.DarkOrange);

            plC_RJ_Button_交易記錄查詢_選取資料設定為已領用.Name = "plC_RJ_Button_交易記錄查詢_選取資料設定為已領用";
            plC_RJ_Button_交易記錄查詢_選取資料設定為未領用.Name = "plC_RJ_Button_交易記錄查詢_選取資料設定為未領用";

            plC_RJ_Button_交易記錄查詢_選取資料設定為已領用.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_選取資料設定為已領用_MouseDownEvent;
            plC_RJ_Button_交易記錄查詢_選取資料設定為未領用.MouseDownEvent += PlC_RJ_Button_交易記錄查詢_選取資料設定為未領用_MouseDownEvent;

            this.交易紀錄.Controls.Add(plC_RJ_Button_交易記錄查詢_選取資料設定為已領用);
            this.交易紀錄.Controls.Add(plC_RJ_Button_交易記錄查詢_選取資料設定為未領用);
            plC_RJ_Button_交易記錄查詢_選取資料設定為已領用.BringToFront();
            plC_RJ_Button_交易記錄查詢_選取資料設定為未領用.BringToFront();
        }

        private PLC_RJ_Button Program_交易紀錄_建立狀態操作按鈕(string text, Point location, Color backColor)
        {
            PLC_RJ_Button button = new PLC_RJ_Button();
            button.AutoResetState = false;
            button.BackgroundColor = backColor;
            button.Bool = false;
            button.BorderColor = Color.PaleVioletRed;
            button.BorderRadius = 20;
            button.BorderSize = 0;
            button.buttonType = RJ_Button.ButtonType.Toggle;
            button.DisenableColor = Color.Gray;
            button.FlatAppearance.BorderSize = 0;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("微軟正黑體", 14F);
            button.GUID = "";
            button.Icon = MessageBoxIcon.Warning;
            button.Image_padding = new Padding(0);
            button.Location = location;
            button.OFF_文字內容 = text;
            button.OFF_文字字體 = new Font("微軟正黑體", 14F);
            button.OFF_文字顏色 = Color.White;
            button.OFF_背景顏色 = backColor;
            button.ON_BorderSize = 5;
            button.ON_文字內容 = text;
            button.ON_文字字體 = new Font("微軟正黑體", 14F);
            button.ON_文字顏色 = Color.Black;
            button.ON_背景顏色 = backColor;
            button.ProhibitionBorderLineWidth = 1;
            button.ProhibitionLineWidth = 4;
            button.ProhibitionSymbolSize = 30;
            button.ShadowColor = Color.DimGray;
            button.ShadowSize = 3;
            button.ShowLoadingForm = false;
            button.Size = new Size(156, 69);
            button.State = false;
            button.Text = text;
            button.TextColor = Color.White;
            button.TextHeight = 0;
            button.Texts = text;
            button.UseVisualStyleBackColor = false;
            button.字型鎖住 = false;
            button.按鈕型態 = PLC_RJ_Button.StatusEnum.保持型;
            button.按鍵方式 = PLC_RJ_Button.PressEnum.Mouse_左鍵;
            button.文字鎖住 = false;
            button.背景圖片 = null;
            button.讀取位元反向 = false;
            button.讀寫鎖住 = false;
            button.音效 = true;
            button.顯示 = false;
            button.顯示狀態 = false;
            button.顯示讀取位置 = "S4077";
            return button;
        }


        bool flag_交易紀錄_頁面更新_init = false;
        private void Program_交易紀錄() 
        {
            if (this.plC_ScreenPage_Main.PageText == "交易紀錄")
            {
                if (flag_交易紀錄_頁面更新_init)
                {
                    this.Invoke(new Action(delegate
                    {

                    }));
                    flag_交易紀錄_頁面更新_init = false;
                }
            }
            else
            {
                flag_交易紀錄_頁面更新_init = true;
            }



        }
        #region Function

        private void 新增交易紀錄(enum_交易記錄查詢動作 _enum_交易記錄查詢動作, string 操作人 , string 房名, string 備註)
        {
            string GUID = $"{_enum_交易記錄查詢動作.GetEnumName()};{操作人};{房名};{DateTime.Now.ToDateTimeString()}";
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.SQL_GetRows((int)enum_交易記錄查詢資料.GUID, GUID, false);
            if (list_value.Count == 0)
            {
                object[] values = new object[new enum_交易記錄查詢資料().GetLength()];
                values[(int)enum_交易記錄查詢資料.GUID] = GUID;
                values[(int)enum_交易記錄查詢資料.動作] = _enum_交易記錄查詢動作.GetEnumName();
                values[(int)enum_交易記錄查詢資料.操作人] = 操作人;
                values[(int)enum_交易記錄查詢資料.病房號] = 房名;
                values[(int)enum_交易記錄查詢資料.操作時間] = DateTime.Now.ToDateTimeString();
                values[(int)enum_交易記錄查詢資料.備註] = 備註;
                this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(values, false);
            }
            
        }
        #endregion

        #region Event
        private void SqL_DataGridView_交易記錄查詢_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_交易紀錄());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_交易紀錄.選取資料設定為已領用.GetEnumName())
                    {
                        Function_交易記錄查詢_選取資料設定為已領用();
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_交易紀錄.選取資料設定為未領用.GetEnumName())
                    {
                        Function_交易記錄查詢_選取資料設定為未領用();
                    }
                }
            }
        }
        private void PlC_RJ_Button_交易記錄查詢_選取資料設定為已領用_MouseDownEvent(MouseEventArgs mevent)
        {
            Function_交易記錄查詢_選取資料設定為已領用();
        }
        private void PlC_RJ_Button_交易記錄查詢_選取資料設定為未領用_MouseDownEvent(MouseEventArgs mevent)
        {
            Function_交易記錄查詢_選取資料設定為未領用();
        }
        private void Function_交易記錄查詢_選取資料設定為已領用()
        {
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.Get_All_Select_RowsValues();
            int index = 0;
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i][(int)enum_交易記錄查詢資料.領用時間].ToDateTimeString() == new DateTime(1999, 1, 1, 0, 0, 0).ToDateTimeString()) continue;
                list_value[i][(int)enum_交易記錄查詢資料.領用人] = this.登入者名稱;
                list_value[i][(int)enum_交易記錄查詢資料.領用時間] = DateTime.Now.ToDateTimeString_6();
                list_value[i][(int)enum_交易記錄查詢資料.備註] = "[強制領用]";
                index++;
            }
            this.sqL_DataGridView_交易記錄查詢.SQL_ReplaceExtra(list_value, false);
            this.sqL_DataGridView_交易記錄查詢.ReplaceExtra(list_value, true);
            MyMessageBox.ShowDialog($"已修正領用數量{index}筆!");
        }
        private void Function_交易記錄查詢_選取資料設定為未領用()
        {
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.Get_All_Select_RowsValues();
            int index = 0;
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i][(int)enum_交易記錄查詢資料.領用時間].ToDateTimeString() == new DateTime(1999, 1, 1, 0, 0, 0).ToDateTimeString()) continue;
                list_value[i][(int)enum_交易記錄查詢資料.領用人] = "未領用";
                list_value[i][(int)enum_交易記錄查詢資料.領用時間] = "1999-01-01 00:00:00";
                list_value[i][(int)enum_交易記錄查詢資料.備註] = "";
                index++;
            }
            this.sqL_DataGridView_交易記錄查詢.SQL_ReplaceExtra(list_value, false);
            this.sqL_DataGridView_交易記錄查詢.ReplaceExtra(list_value, true);
            MyMessageBox.ShowDialog($"已修正領用數量{index}筆!");
        }
        private void SqL_DataGridView_交易記錄查詢_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {

            List<object[]> RowsList_buf = RowsList.ToList();

            // 已領用 / 未領用
            if (plC_CheckBox_交易記錄查詢_顯示已領用.Checked && !plC_CheckBox_交易記錄查詢_顯示未領用.Checked)
            {
                RowsList_buf = RowsList_buf
                    .Where(temp => temp[(int)enum_交易記錄查詢資料.領用時間].ToDateString() != "1999/01/01")
                    .ToList();
            }
            else if (!plC_CheckBox_交易記錄查詢_顯示已領用.Checked && plC_CheckBox_交易記錄查詢_顯示未領用.Checked)
            {
                RowsList_buf = RowsList_buf
                    .Where(temp => temp[(int)enum_交易記錄查詢資料.領用時間].ToDateString() == "1999/01/01")
                    .ToList();
            }
            // 如果兩個都勾，就不過濾
            // 如果兩個都沒勾，可視需求決定是否清空
            else if (!plC_CheckBox_交易記錄查詢_顯示已領用.Checked && !plC_CheckBox_交易記錄查詢_顯示未領用.Checked)
            {
                RowsList_buf = new List<object[]>();
            }

            // 顯示細節
            if (plC_CheckBox_交易記錄查詢_顯示細節.Checked == false)
            {
                RowsList_buf = RowsList_buf
                    .Where(temp => temp[(int)enum_交易記錄查詢資料.動作].ToString() == enum_交易記錄查詢動作.藥袋刷入.GetEnumName())
                    .ToList();
            }

            // 要發藥 / 不發藥
            if (plC_CheckBox_交易記錄查詢_顯示要發藥.Checked && !plC_CheckBox_交易記錄查詢_顯示不發藥.Checked)
            {
                RowsList_buf = RowsList_buf
                    .Where(temp => temp[(int)enum_交易記錄查詢資料.備註].ToString().Contains("[不發藥]") == false)
                    .ToList();
            }
            else if (!plC_CheckBox_交易記錄查詢_顯示要發藥.Checked && plC_CheckBox_交易記錄查詢_顯示不發藥.Checked)
            {
                RowsList_buf = RowsList_buf
                    .Where(temp => temp[(int)enum_交易記錄查詢資料.備註].ToString().Contains("[不發藥]") == true)
                    .ToList();
            }
            else if (!plC_CheckBox_交易記錄查詢_顯示要發藥.Checked && !plC_CheckBox_交易記錄查詢_顯示不發藥.Checked)
            {
                RowsList_buf = new List<object[]>();
            }
            RowsList_buf.Sort(new ICP_交易記錄查詢());
            RowsList = RowsList_buf;

        }
        private void SqL_DataGridView_交易記錄查詢_DataGridRefreshEvent()
        {
            string date = "";
            string 動作 = "";
            for (int i = 0; i < this.sqL_DataGridView_交易記錄查詢.dataGridView.Rows.Count; i++)
            {
                date = this.sqL_DataGridView_交易記錄查詢.dataGridView.Rows[i].Cells[enum_交易記錄查詢資料.領用時間.GetEnumName()].Value.ToString();
                if(date.Replace("/","-") == "1999-01-01 00:00:00")
                {
                    this.sqL_DataGridView_交易記錄查詢.dataGridView.Rows[i].Cells[enum_交易記錄查詢資料.領用時間.GetEnumName()].Value = "-";
                    this.sqL_DataGridView_交易記錄查詢.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(245, 218, 120);
                    this.sqL_DataGridView_交易記錄查詢.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }

                動作 = this.sqL_DataGridView_交易記錄查詢.dataGridView.Rows[i].Cells[(int)enum_交易記錄查詢資料.動作].Value.ToString();
                if (動作 == enum_交易記錄查詢動作.人臉識別登入.GetEnumName()
                    || 動作 == enum_交易記錄查詢動作.一維碼登入.GetEnumName()
                    || 動作 == enum_交易記錄查詢動作.密碼登入.GetEnumName()
                    || 動作 == enum_交易記錄查詢動作.登出.GetEnumName()
                    || 動作 == enum_交易記錄查詢動作.效期庫存異動.GetEnumName()
                    || 動作 == enum_交易記錄查詢動作.操作工程模式.GetEnumName()
                    )
                {
                    this.sqL_DataGridView_交易記錄查詢.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                    this.sqL_DataGridView_交易記錄查詢.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }
        private void PlC_RJ_Button_交易記錄查詢_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.GetAllRows();
                if(list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("無資料可匯出!");
                    return;
                }
                if(this.saveFileDialog_SaveExcel.ShowDialog() == DialogResult.OK)
                {
                    string filename = this.saveFileDialog_SaveExcel.FileName;
                    DataTable dataTable = list_value.ToDataTable(new enum_交易記錄查詢資料());
                    dataTable = dataTable.ReorderTable(new enum_交易記錄查詢資料_匯出().GetEnumNames());

                    LoadingForm.ShowLoadingForm();
                    string Extension = System.IO.Path.GetExtension(this.saveFileDialog_SaveExcel.FileName);
                    if (Extension == ".txt")
                    {
                        CSVHelper.SaveFile(dataTable, this.saveFileDialog_SaveExcel.FileName);
                        MyMessageBox.ShowDialog("匯出完成!");
                    }
                    else if (Extension == ".xls" || Extension == ".xlsx")
                    {
                        MyOffice.ExcelClass.NPOI_SaveFile(dataTable, this.saveFileDialog_SaveExcel.FileName);
                        MyMessageBox.ShowDialog("匯出完成!");
                    }
                    else if (Extension == ".csv")
                    {
                        CSVHelper.SaveFile(dataTable, this.saveFileDialog_SaveExcel.FileName);
                        MyMessageBox.ShowDialog("匯出完成!");
                    }
                    LoadingForm.CloseLoadingForm();
                }
            }));
        }
        private void PlC_RJ_Button_交易記錄查詢_顯示全部_MouseDownEvent(MouseEventArgs mevent)
        {
            this.sqL_DataGridView_交易記錄查詢.SQL_GetAllRows(true);
        }
        private void PlC_RJ_Button_交易記錄查詢_刪除資料_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.Get_All_Select_RowsValues();
            if(list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            this.sqL_DataGridView_交易記錄查詢.SQL_DeleteExtra(list_value, false);
            this.sqL_DataGridView_交易記錄查詢.DeleteExtra(list_value, true);
            MyMessageBox.ShowDialog("刪除完成!");
        }

        private void PlC_RJ_Button_交易記錄查詢_藥品名稱_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.rJ_TextBox_交易記錄查詢_藥品名稱.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未輸入資料!");
                return;
            }
            this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByLike((int)enum_交易記錄查詢資料.藥品名稱, this.rJ_TextBox_交易記錄查詢_藥品名稱.Text, true);
        }
        private void PlC_RJ_Button_交易記錄查詢_藥品碼_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.textBox_交易記錄查詢_藥品碼.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未輸入資料!");
                return;
            }
            this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByLike((int)enum_交易記錄查詢資料.藥品碼, this.textBox_交易記錄查詢_藥品碼.Text, true);
        }
        private void PlC_RJ_Button_交易記錄查詢_領用人_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.rJ_TextBox_交易記錄查詢_領用人.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未輸入資料!");
                return;
            }
            this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByLike((int)enum_交易記錄查詢資料.領用人, this.rJ_TextBox_交易記錄查詢_領用人.Text, true);
        }
        private void PlC_RJ_Button__交易記錄查詢_調劑人_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.rJ_TextBox_交易記錄查詢_調劑人.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未輸入資料!");
                return;
            }
            this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByLike((int)enum_交易記錄查詢資料.操作人, this.rJ_TextBox_交易記錄查詢_調劑人.Text, true);
        }
        private void PlC_RJ_Button_交易記錄查詢_病歷號_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.rJ_TextBox_交易記錄查詢_病歷號.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未輸入資料!");
                return;
            }
            this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByLike((int)enum_交易記錄查詢資料.病歷號, this.rJ_TextBox_交易記錄查詢_病歷號.Text, true);
        }
        private void PlC_RJ_Button_交易記錄查詢_病房號_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.rJ_TextBox_交易記錄查詢_病房號.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未輸入資料!");
                return;
            }
            this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByLike((int)enum_交易記錄查詢資料.病房號, this.rJ_TextBox_交易記錄查詢_病房號.Text, true);
        }
        private void PlC_RJ_Button_交易記錄查詢_領用時間_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            DateTime dateTime_st = dateTimePicker_交易記錄查詢_領用時間_起始.Value;
            DateTime dateTime_end = dateTimePicker_交易記錄查詢_領用時間_結束.Value;
            dateTime_st = new DateTime(dateTime_st.Year, dateTime_st.Month, dateTime_st.Day, 00, 00, 00);
            dateTime_end = new DateTime(dateTime_end.Year, dateTime_end.Month, dateTime_end.Day, 23, 59, 59);
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByBetween((int)enum_交易記錄查詢資料.領用時間, dateTime_st, dateTime_end, false);
          
            this.sqL_DataGridView_交易記錄查詢.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_交易記錄查詢_開方時間_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            DateTime dateTime_st = dateTimePicker_交易記錄查詢_開方時間_起始.Value;
            DateTime dateTime_end = dateTimePicker_交易記錄查詢_開方時間_結束.Value;
            dateTime_st = new DateTime(dateTime_st.Year, dateTime_st.Month, dateTime_st.Day, 00, 00, 00);
            dateTime_end = new DateTime(dateTime_end.Year, dateTime_end.Month, dateTime_end.Day, 23, 59, 59);
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByBetween((int)enum_交易記錄查詢資料.開方時間, dateTime_st, dateTime_end, false);
         
            this.sqL_DataGridView_交易記錄查詢.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_交易記錄查詢_操作時間_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            DateTime dateTime_st = dateTimePicker_交易記錄查詢_操作時間_起始.Value;
            DateTime dateTime_end = dateTimePicker_交易記錄查詢_操作時間_結束.Value;
            dateTime_st = new DateTime(dateTime_st.Year, dateTime_st.Month, dateTime_st.Day, 00, 00, 00);
            dateTime_end = new DateTime(dateTime_end.Year, dateTime_end.Month, dateTime_end.Day, 23, 59, 59);
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.SQL_GetRowsByBetween((int)enum_交易記錄查詢資料.操作時間, dateTime_st, dateTime_end, false);
          
            this.sqL_DataGridView_交易記錄查詢.RefreshGrid(list_value);
        }
        #endregion


        public class ICP_交易記錄查詢 : IComparer<object[]>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(object[] x, object[] y)
            {
                DateTime datetime1 = x[(int)enum_交易記錄查詢資料.操作時間].ToDateTimeString_6().StringToDateTime();
                DateTime datetime2 = y[(int)enum_交易記錄查詢資料.操作時間].ToDateTimeString_6().StringToDateTime();
                int compare = DateTime.Compare(datetime1, datetime2);
                return compare;
            }
        }
    }
}
