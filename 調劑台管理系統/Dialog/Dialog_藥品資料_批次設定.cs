using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using MyUI;
using Basic;
using SQLUI;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using HIS_DB_Lib;
using MyOffice;
using H_Pannel_lib;

namespace 調劑台管理系統
{
    public partial class Dialog_藥品資料_批次設定 : MyDialog
    {
        public Dialog_藥品資料_批次設定()
        {
            form.Invoke(new Action(() =>
            {
                InitializeComponent();

                Table table = medClass.init(Main_Form.API_Server);
                sqL_DataGridView_藥品資料.Init(table);
                this.sqL_DataGridView_藥品資料.Init(table);
                this.sqL_DataGridView_藥品資料.RowsHeight = 60;
                this.sqL_DataGridView_藥品資料.Set_ColumnVisible(false, new enum_雲端藥檔().GetEnumNames());
                this.sqL_DataGridView_藥品資料.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品碼);
                this.sqL_DataGridView_藥品資料.Set_ColumnWidth(1000, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品名稱);
                this.sqL_DataGridView_藥品資料.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleCenter, enum_雲端藥檔.包裝單位);

                this.sqL_DataGridView_藥品資料.Set_ColumnText("藥碼", enum_雲端藥檔.藥品碼);
                this.sqL_DataGridView_藥品資料.Set_ColumnText("藥名", enum_雲端藥檔.藥品名稱);

                sqL_DataGridView_藥品資料.DataGridRowsChangeRefEvent += SqL_DataGridView_藥品資料_DataGridRowsChangeRefEvent;
                sqL_DataGridView_藥品資料.DataGridClearGridEvent += SqL_DataGridView_藥品資料_DataGridClearGridEvent;
                comboBox_搜尋條件.SelectedIndex = 0;
                comboBox_搜尋條件.SelectedIndexChanged += ComboBox_搜尋條件_SelectedIndexChanged;
                comboBox_設定種類.SelectedIndexChanged += ComboBox_設定種類_SelectedIndexChanged;

                this.LoadFinishedEvent += Dialog_藥品資料_批次設定_LoadFinishedEvent;
                rJ_Button_藥品搜尋.MouseDownEvent += RJ_Button_藥品搜尋_MouseDownEvent;
                rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            }));
         
        }

     

        private void Dialog_藥品資料_批次設定_LoadFinishedEvent(EventArgs e)
        {
            comboBox_設定種類.SelectedIndex = 0;
        }

        private void SqL_DataGridView_藥品資料_DataGridClearGridEvent()
        {
            sqL_DataGridView_藥品資料.ClearDataKeys();
        }
        private void SqL_DataGridView_藥品資料_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {
            List<object[]> RowsList_buf = new List<object[]>();
            if (checkBox_只顯示調劑台品項.Checked)
            {
                List<medClass> medClasses_dps = medClass.get_dps_medClass(Main_Form.API_Server, Main_Form.ServerName);
                List<medClass> medClasses_dps_buf = new List<medClass>();
                medClasses_dps = (from temp in medClasses_dps
                                  where temp.DeviceBasics.Count > 0
                                  select temp).ToList();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_dps = medClasses_dps.CoverToDictionaryByCode();
                for (int i = 0; i < RowsList.Count; i++)
                {
                    medClasses_dps_buf = keyValuePairs_medClasses_dps.SortDictionaryByCode(RowsList[i][(int)enum_雲端藥檔.藥品碼].ObjectToString());
                    if (medClasses_dps_buf.Count > 0)
                    {
                        RowsList_buf.Add(RowsList[i]);
                    }
                }

                RowsList = RowsList_buf;
            }
        }
        private void RJ_Button_藥品搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            LoadingForm.ShowLoadingForm();
            try
            {


                List<medClass> medClasses = new List<medClass>();

                if (comboBox_搜尋條件.GetComboBoxText() == "全部顯示")
                {
                    medClasses = medClass.get_med_cloud(Main_Form.API_Server);
                }
                if (comboBox_搜尋條件.GetComboBoxText() == "藥碼")
                {
                    medClass medClass = medClass.get_med_clouds_by_code(Main_Form.API_Server, comboBox_搜尋內容.GetComboBoxText());
                    medClasses.Add(medClass);
                }
                if (comboBox_搜尋條件.GetComboBoxText() == "藥名")
                {
                    medClasses = medClass.get_med_clouds_by_name(Main_Form.API_Server, comboBox_搜尋內容.GetComboBoxText());
                }
                if (comboBox_搜尋條件.GetComboBoxText() == "中文名")
                {
                    medClasses = medClass.get_med_clouds_by_chtname(Main_Form.API_Server, comboBox_搜尋內容.GetComboBoxText());
                }
                if (comboBox_搜尋條件.GetComboBoxText() == "管制級別")
                {
                    medClasses = medClass.get_med_clouds_by_durgkind(Main_Form.API_Server, comboBox_搜尋內容.GetComboBoxText());
                }
                if (comboBox_搜尋條件.GetComboBoxText() == "已選藥品")
                {
                    string text = comboBox_設定種類.GetComboBoxText();
                    if (text == "調劑註記" || text == "形狀相似" || text == "發音相似" || text == "使用RFID")
                    {
                        medClasses = medClass.get_med_cloud(Main_Form.API_Server);
                        List<medClass> medClasses_temp = new List<medClass>();
                        List<medClass> medClasses_buf = new List<medClass>();
                        Dictionary<string, List<medClass>> keyValuePairs_cloud = medClasses.CoverToDictionaryByCode();
                        List<medConfigClass> medConfigClasses = new List<medConfigClass>();
                        if (text == "調劑註記") medConfigClasses = medConfigClass.get_dispense_note(Main_Form.API_Server);
                        if (text == "形狀相似") medConfigClasses = medConfigClass.get_isShapeSimilar_note(Main_Form.API_Server);
                        if (text == "發音相似") medConfigClasses = medConfigClass.get_isSoundSimilar_note(Main_Form.API_Server);
                        if (text == "使用RFID") medConfigClasses = medConfigClass.get_useRFID(Main_Form.API_Server);


                        for (int i = 0; i < medConfigClasses.Count; i++)
                        {
                            medClasses_temp = keyValuePairs_cloud.SortDictionaryByCode(medConfigClasses[i].藥碼);
                            if (medClasses_temp.Count > 0)
                            {
                                medClasses_buf.Add(medClasses_temp[0]);
                            }
                        }
                        medClasses = medClasses_buf;
                    }
                }


                List<object[]> list_med = medClasses.ClassToSQL<medClass, enum_雲端藥檔>();
                this.sqL_DataGridView_藥品資料.RefreshGrid(list_med);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
        }
        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
      

            LoadingForm.ShowLoadingForm();
            DialogResult dialogResult = DialogResult.None;
            try
            {
                string text = comboBox_設定種類.GetComboBoxText();
                if (text == "調劑註記" || text == "形狀相似" || text == "發音相似" || text == "使用RFID")
                {
                    List<medClass> medClasses_cloud = medClass.get_med_cloud(Main_Form.API_Server);
                    List<object[]> list_med = medClasses_cloud.ClassToSQL<medClass, enum_雲端藥檔>();
                    List<object[]> list_value = this.sqL_DataGridView_藥品資料.Get_All_Checked_RowsValuesEx(list_med);
                    List<medClass> medClasses_checked = list_value.SQLToClass<medClass, enum_雲端藥檔>();

                    List<medConfigClass> medConfigClasses = medConfigClass.get_all(Main_Form.API_Server);
                    Dictionary<string, List<medConfigClass>> keyValuePairs_medConfig = medConfigClasses.CoverToDictionaryByCode();
                    List<medConfigClass> medConfigClasses_buf = new List<medConfigClass>();
                    List<medConfigClass> medConfigClasses_update = new List<medConfigClass>();
                    for (int i = 0; i < medClasses_cloud.Count; i++)
                    {
                        string code = medClasses_cloud[i].藥品碼;
                        medClass medClass_checked = medClasses_checked.Find(x => x.藥品碼 == code);

                        medConfigClasses_buf = keyValuePairs_medConfig.SortDictionaryByCode(code);
                        medConfigClass medConfigClass;
                        if (medConfigClasses_buf.Count > 0)
                        {
                            medConfigClass = medConfigClasses_buf[0];                  
                        }
                        else
                        {
                            medConfigClass = new medConfigClass();
                            medConfigClass.藥碼 = code;
                        }
                        if (text == "調劑註記") medConfigClass.調劑註記 = (medClass_checked != null).ToString();
                        if (text == "形狀相似") medConfigClass.形狀相似 = (medClass_checked != null).ToString();
                        if (text == "發音相似") medConfigClass.發音相似 = (medClass_checked != null).ToString();
                        if (text == "使用RFID") medConfigClass.使用RFID = (medClass_checked != null).ToString();
                        medConfigClasses_update.Add(medConfigClass);

                    }


                    dialogResult = MyMessageBox.ShowDialog($"是否儲存資料,共<{medConfigClasses_update.Count}>筆?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel);
                    if (dialogResult != DialogResult.Yes) return;

                    medConfigClass.add(Main_Form.API_Server, medConfigClasses_update);
                }
                   
          
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
                if (dialogResult == DialogResult.Yes)
                {
                    MyMessageBox.ShowDialog("儲存成功");
                }
            }

        }
        private void ComboBox_搜尋條件_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate
            {
                if (comboBox_搜尋條件.GetComboBoxText() == "管制級別")
                {
                    comboBox_搜尋內容.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBox_搜尋內容.DataSource = new string[] { "1", "2", "3", "4", "N", };
                    comboBox_搜尋內容.SelectedIndex = 0;
                    comboBox_搜尋內容.Invalidate();
                }
                else
                {
                    comboBox_搜尋內容.DropDownStyle = ComboBoxStyle.DropDown;
                    comboBox_搜尋內容.Invalidate();
                }
            }));
        }
        private void ComboBox_設定種類_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadingForm.ShowLoadingForm();
         
            try
            {
                string text = comboBox_設定種類.GetComboBoxText();
                if (text == "調劑註記" || text == "形狀相似" || text == "發音相似" || text == "使用RFID")
                {
                    List<medClass> medClasses = medClass.get_med_cloud(Main_Form.API_Server);
                    List<medClass> medClasses_temp = new List<medClass>();
                    List<medClass> medClasses_buf = new List<medClass>();
                    Dictionary<string, List<medClass>> keyValuePairs_cloud = medClasses.CoverToDictionaryByCode();

                    List<medConfigClass> medConfigClasses = new List<medConfigClass>();
                    if (text == "調劑註記") medConfigClasses = medConfigClass.get_dispense_note(Main_Form.API_Server);
                    if (text == "形狀相似") medConfigClasses = medConfigClass.get_isShapeSimilar_note(Main_Form.API_Server);
                    if (text == "發音相似") medConfigClasses = medConfigClass.get_isSoundSimilar_note(Main_Form.API_Server);
                    if (text == "使用RFID") medConfigClasses = medConfigClass.get_useRFID(Main_Form.API_Server);

                    
                    for (int i = 0; i < medConfigClasses.Count; i++)
                    {
                        medClasses_temp = keyValuePairs_cloud.SortDictionaryByCode(medConfigClasses[i].藥碼);
                        if (medClasses_temp.Count > 0)
                        {
                            medClasses_buf.Add(medClasses_temp[0]);
                        }
                    }
                    List<object[]> list_med = medClasses_buf.ClassToSQL<medClass, enum_雲端藥檔>();
                    this.sqL_DataGridView_藥品資料.ClearDataKeys();
                    this.sqL_DataGridView_藥品資料.SetDataKeys(list_med, true);
                    this.sqL_DataGridView_藥品資料.RefreshGrid(medClasses.ClassToSQL<medClass, enum_雲端藥檔>());

                }
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog("設定失敗");
            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
          
        }
    }
}
