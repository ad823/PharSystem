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
    public partial class Dialog_藥檔維護 : Form
    {
        public static Form form;
        public DialogResult ShowDialog()
        {
            if (form == null)
            {
                base.ShowDialog();
            }
            else
            {
                form.Invoke(new Action(delegate
                {
                    base.ShowDialog();
                }));
            }

            return this.DialogResult;
        }
        public Dialog_藥檔維護()
        {
            InitializeComponent();
            this.Load += Dialog_藥檔維護_Load;

            this.plC_RJ_Button_顯示全部.MouseDownEvent += PlC_RJ_Button_顯示全部_MouseDownEvent;
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;
            this.plC_RJ_Button_取消.MouseDownEvent += PlC_RJ_Button_取消_MouseDownEvent;

            this.plC_RJ_Button_藥品碼_搜尋.MouseDownEvent += PlC_RJ_Button_藥品碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_藥名_搜尋.MouseDownEvent += PlC_RJ_Button_藥名_搜尋_MouseDownEvent;
            this.plC_RJ_Button_中文名_搜尋.MouseDownEvent += PlC_RJ_Button_中文名_搜尋_MouseDownEvent;
            this.plC_RJ_Button_管制級別_搜尋.MouseDownEvent += PlC_RJ_Button_管制級別_搜尋_MouseDownEvent;
            this.plC_RJ_Button_高價藥品_搜尋.MouseDownEvent += PlC_RJ_Button_高價藥品_搜尋_MouseDownEvent;
        }

 


        #region Function
        private void Function_新增藥品()
        {
            List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.Get_All_Checked_RowsValues();
            List<medClass> medClasses = list_value.SQLToClass<medClass, enum_雲端藥檔>();
            List<object[]> list_value_add = medClasses.ClassToSQL<medClass, enum_藥品資料_藥檔資料>();
            this.sqL_DataGridView_本地藥檔.SQL_AddRows(list_value_add, false);
            MyMessageBox.ShowDialog($"新增藥品成功!共<{list_value_add.Count}>筆");
        }
        private void Function_雲端藥檔_Init()
        {
            string url = $"{Main_Form.API_Server}/api/MED_page/init";
            returnData returnData = new returnData();
            returnData.ServerType = enum_sys_serverSetting_Type.調劑台.GetEnumName();
            returnData.ServerName = $"{Main_Form.ServerName}";
            returnData.TableName = "medicine_page_cloud";
            string json_in = returnData.JsonSerializationt();
            string json = Basic.Net.WEBApiPostJson($"{url}", json_in);
            Table table = json.JsonDeserializet<Table>();
            if (table == null)
            {
                MyMessageBox.ShowDialog($"雲端藥檔表單建立失敗!! Api_URL:{Main_Form.API_Server}");
                return;
            }

            this.sqL_DataGridView_雲端藥檔.Init(table);
            this.sqL_DataGridView_雲端藥檔.Set_ColumnVisible(false, new enum_雲端藥檔().GetEnumNames());
            this.sqL_DataGridView_雲端藥檔.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品碼);
            this.sqL_DataGridView_雲端藥檔.Set_ColumnWidth(280, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品名稱);
            this.sqL_DataGridView_雲端藥檔.Set_ColumnWidth(280, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.中文名稱);
            this.sqL_DataGridView_雲端藥檔.Set_ColumnWidth(280, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品學名);
            this.sqL_DataGridView_雲端藥檔.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.包裝單位);
            this.sqL_DataGridView_雲端藥檔.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.警訊藥品);
            this.sqL_DataGridView_雲端藥檔.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.管制級別);

            this.sqL_DataGridView_雲端藥檔.DataGridRowsChangeRefEvent += SqL_DataGridView_雲端藥檔_DataGridRowsChangeRefEvent;
        }
        private void Function_本地藥檔_Init()
        {
            string url = $"{Main_Form.API_Server}/api/MED_page/init";
            returnData returnData = new returnData();
            returnData.ServerType = enum_sys_serverSetting_Type.調劑台.GetEnumName();
            returnData.ServerName = $"{Main_Form.ServerName}";
            returnData.TableName = "medicine_page";
            string json_in = returnData.JsonSerializationt();
            string json = Basic.Net.WEBApiPostJson($"{url}", json_in);
            Table table = json.JsonDeserializet<Table>();
            if (table == null)
            {
                MyMessageBox.ShowDialog($"本地藥檔表單建立失敗!! Api_URL:{Main_Form.API_Server}");
                return;
            }

            this.sqL_DataGridView_本地藥檔.Init(table);
            this.sqL_DataGridView_本地藥檔.Set_ColumnVisible(false, new enum_藥品資料_藥檔資料().GetEnumNames());
            this.sqL_DataGridView_本地藥檔.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品碼);
            this.sqL_DataGridView_本地藥檔.Set_ColumnWidth(280, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品名稱);
            this.sqL_DataGridView_本地藥檔.Set_ColumnWidth(280, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.中文名稱);
            this.sqL_DataGridView_本地藥檔.Set_ColumnWidth(280, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品學名);
            this.sqL_DataGridView_本地藥檔.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.包裝單位);
            this.sqL_DataGridView_本地藥檔.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.警訊藥品);
            this.sqL_DataGridView_本地藥檔.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.管制級別);
        }
        #endregion
        #region Event
        private void SqL_DataGridView_雲端藥檔_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {
            List<object[]> list_本地藥檔 = this.sqL_DataGridView_本地藥檔.SQL_GetAllRows(false);
            List<object[]> list_本地藥檔_buf = new List<object[]>();
            List<object[]> list_value = new List<object[]>();
            for(int i = 0; i < RowsList.Count; i++)
            {
                string 藥品碼 = RowsList[i][(int)enum_雲端藥檔.藥品碼].ObjectToString();
                list_本地藥檔_buf = list_本地藥檔.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, 藥品碼);
                if(list_本地藥檔_buf.Count == 0)
                {
                    list_value.Add(RowsList[i]);
                }
            }
            RowsList = list_value;
            if(list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("無可新增藥品!");
            }
        }
        private void PlC_RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }));
        }
        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.Function_新增藥品();
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }
        private void Dialog_藥檔維護_Load(object sender, EventArgs e)
        {
            string json_result = Basic.Net.WEBApiGet($"{Main_Form.API_Server}/api/ServerSetting");
            if (json_result.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("API Server 連結失敗!");
                return;
            }
            //Console.WriteLine(json_result);
            returnData returnData = json_result.JsonDeserializet<returnData>();
            List<HIS_DB_Lib.sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();
            HIS_DB_Lib.sys_serverSettingClass sys_serverSettingClass;

            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Main_Form.ServerName, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.一般資料);
            if (sys_serverSettingClass != null)
            {
                SQLUI.SQL_DataGridView.ConnentionClass connentionClass = new SQL_DataGridView.ConnentionClass();
                connentionClass.IP = sys_serverSettingClass.Server;
                connentionClass.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                connentionClass.DataBaseName = sys_serverSettingClass.DBName;
                connentionClass.UserName = sys_serverSettingClass.User;
                connentionClass.Password = sys_serverSettingClass.Password;
                SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_本地藥檔, connentionClass);
                this.sqL_DataGridView_本地藥檔.SQL_Reset();
            }
            sys_serverSettingClass = sys_serverSettingClasses.MyFind(Main_Form.ServerName, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.藥檔資料);
            if (sys_serverSettingClass != null)
            {
                SQLUI.SQL_DataGridView.ConnentionClass connentionClass = new SQL_DataGridView.ConnentionClass();
                connentionClass.IP = sys_serverSettingClass.Server;
                connentionClass.Port = (uint)(sys_serverSettingClass.Port.StringToInt32());
                connentionClass.DataBaseName = sys_serverSettingClass.DBName;
                connentionClass.UserName = sys_serverSettingClass.User;
                connentionClass.Password = sys_serverSettingClass.Password;
                SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_雲端藥檔, connentionClass);
                this.sqL_DataGridView_雲端藥檔.SQL_Reset();
            }
            Function_雲端藥檔_Init();
            Function_本地藥檔_Init();

        }
        private void PlC_RJ_Button_顯示全部_MouseDownEvent(MouseEventArgs mevent)
        {
            this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(true);
        }
        private void PlC_RJ_Button_高價藥品_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            list_value = list_value.GetRowsByLike((int)enum_雲端藥檔.高價藥品, true.ToString().ToUpper(), true);
            this.sqL_DataGridView_雲端藥檔.RefreshGrid(list_value);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
            }
        }
        private void PlC_RJ_Button_管制級別_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            if (rJ_ComboBox_藥品資料_管制級別.SelectedItem.ObjectToString().StringIsEmpty())
            {
                MyMessageBox.ShowDialog("請選擇資料!");
                return;
            }
            list_value = list_value.GetRows((int)enum_雲端藥檔.管制級別, rJ_ComboBox_藥品資料_管制級別.SelectedItem.ObjectToString());
            this.sqL_DataGridView_雲端藥檔.RefreshGrid(list_value);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
            }
        }
        private void PlC_RJ_Button_中文名_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            if (!textBox_藥品資料_中文名.Text.StringIsEmpty())
            {

                if (rJ_RatioButton_藥品資料_前綴.Checked)
                {
                    if (textBox_藥品資料_中文名.Text.Length < 3)
                    {
                        MyMessageBox.ShowDialog("藥品名稱搜尋字元不得小於3個!");
                        return;
                    }
                    list_value = list_value.GetRowsStartWithByLike((int)enum_雲端藥檔.中文名稱, textBox_藥品資料_中文名.Text);
                }
                else if (rJ_RatioButton_藥品資料_模糊.Checked)
                {
                    list_value = list_value.GetRowsByLike((int)enum_雲端藥檔.中文名稱, textBox_藥品資料_中文名.Text, true);
                }
                this.sqL_DataGridView_雲端藥檔.RefreshGrid(list_value);
                if (list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("查無資料!");
                }
            }
        }
        private void PlC_RJ_Button_藥名_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {

            List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            if (textBox_藥品資料_藥品名稱.Text.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未輸入搜尋資料!");
                return;
            }

            if (!textBox_藥品資料_藥品名稱.Text.StringIsEmpty())
            {

                if (rJ_RatioButton_藥品資料_前綴.Checked)
                {
                    if (textBox_藥品資料_藥品名稱.Text.Length < 3)
                    {
                        MyMessageBox.ShowDialog("藥品名稱搜尋字元不得小於3個!");
                        return;
                    }
                    list_value = list_value.GetRowsStartWithByLike((int)enum_雲端藥檔.藥品名稱, textBox_藥品資料_藥品名稱.Text);
                }
                else if (rJ_RatioButton_藥品資料_模糊.Checked)
                {
                    list_value = list_value.GetRowsByLike((int)enum_雲端藥檔.藥品名稱, textBox_藥品資料_藥品名稱.Text, true);
                }
                this.sqL_DataGridView_雲端藥檔.RefreshGrid(list_value);
                if (list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("查無資料!");
                }
            }
        }
        private void PlC_RJ_Button_藥品碼_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            if (!textBox_藥品資料_藥品碼.Text.StringIsEmpty()) list_value = list_value.GetRowsByLike((int)enum_雲端藥檔.藥品碼, textBox_藥品資料_藥品碼.Text);
            this.sqL_DataGridView_雲端藥檔.RefreshGrid(list_value);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
            }
        }


        #endregion

        private void plC_RJ_Button_取消_MouseDownEvent_1(MouseEventArgs mevent)
        {

        }

        private void plC_RJ_Button_顯示全部_MouseDownEvent_1(MouseEventArgs mevent)
        {

        }
    }
}
