using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS_DB_Lib;
using Basic;
using MyUI;
using SQLUI;
using DrawingClass;
using H_Pannel_lib;

namespace FADC
{
    public partial class Dialog_藥品組合 : MyDialog
    {
        public bool flag_已選擇組合 = false;
        public bool IsDeviceSerch = true;
        public Dialog_藥品組合()
        {
            form.Invoke(new Action(delegate { InitializeComponent(); }));
          
            this.LoadFinishedEvent += Dialog_藥品組合_LoadFinishedEvent;
        }

        private void Dialog_藥品組合_LoadFinishedEvent(EventArgs e)
        {
            Table table = medClass.init(Main_Form.API_Server);
            sqL_DataGridView_藥品搜尋.Init(table);
            sqL_DataGridView_藥品搜尋.Set_ColumnVisible(false, new enum_雲端藥檔().GetEnumNames());
            sqL_DataGridView_藥品搜尋.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_雲端藥檔.藥品碼);
            sqL_DataGridView_藥品搜尋.Set_ColumnWidth(400, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品名稱);
            sqL_DataGridView_藥品搜尋.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_雲端藥檔.包裝單位);
            sqL_DataGridView_藥品搜尋.Set_ColumnText("藥碼", enum_雲端藥檔.藥品碼);
            sqL_DataGridView_藥品搜尋.Set_ColumnText("藥名", enum_雲端藥檔.藥品名稱);
            sqL_DataGridView_藥品搜尋.Set_ColumnText("單位", enum_雲端藥檔.包裝單位);

            sqL_DataGridView_藥品搜尋.RowEnterEvent += SqL_DataGridView_藥品搜尋_RowEnterEvent;

            table = new Table(new enum_medCombo());
            sqL_DataGridView_藥品組合.Init(table);
            sqL_DataGridView_藥品組合.Set_ColumnVisible(false, new enum_medCombo().GetEnumNames());
            sqL_DataGridView_藥品組合.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_medCombo.藥碼);
            sqL_DataGridView_藥品組合.Set_ColumnWidth(400, DataGridViewContentAlignment.MiddleCenter, enum_medCombo.藥名);


            rJ_Button_搜尋.MouseDownEvent += RJ_Button_搜尋_MouseDownEvent;
            rJ_Button_加入.MouseDownEvent += RJ_Button_加入_MouseDownEvent;
            rJ_Button_刪除組合.MouseDownEvent += RJ_Button_刪除組合_MouseDownEvent;
            rJ_Button_確認組合.MouseDownEvent += RJ_Button_確認組合_MouseDownEvent;
            rJ_Button_重新選擇組合.MouseDownEvent += RJ_Button_重新選擇組合_MouseDownEvent;
            this.comboBox_搜尋條件.SelectedIndex = 0;

            RJ_Button_搜尋_MouseDownEvent(null);
        }

      

        private void SqL_DataGridView_藥品搜尋_RowEnterEvent(object[] RowValue)
        {
            List<medComboClass> medCombos = medComboClass.get_by_code(Main_Form.API_Server, RowValue[(int)enum_雲端藥檔.藥品碼].ObjectToString());
            if (medCombos.Count > 0)
            {
                if (sqL_DataGridView_藥品組合.SQL_GetAllRows(false).Count > 0)
                {
                    if (MyMessageBox.ShowDialog("選取藥品已有組合,是否放棄當前組合?") == DialogResult.Yes)
                    {
                        sqL_DataGridView_藥品組合.RefreshGrid(medCombos.ClassToSQL<medComboClass, enum_medCombo>());
                        flag_已選擇組合 = false;
                        return;
                    }
                }
                sqL_DataGridView_藥品組合.RefreshGrid(medCombos.ClassToSQL<medComboClass, enum_medCombo>());

            }
        }
        private void RJ_Button_重新選擇組合_MouseDownEvent(MouseEventArgs mevent)
        {
            sqL_DataGridView_藥品組合.ClearGrid();
            flag_已選擇組合 = false;
        }
        private void RJ_Button_加入_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = sqL_DataGridView_藥品搜尋.Get_All_Select_RowsValues();
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("請選擇資料");
                return;
            }
            for (int i = 0; i < list_value.Count; i++)
            {
                string code = list_value[i][(int)enum_雲端藥檔.藥品碼].ObjectToString();
                string name = list_value[i][(int)enum_雲端藥檔.藥品名稱].ObjectToString();
                if (sqL_DataGridView_藥品組合.GetRows((int)enum_medCombo.藥碼, code, false).Count > 0)
                {
                    MyMessageBox.ShowDialog($"({code}){name},已加入過");
                    return;
                }
            }
            List<medComboClass> medCombos = new List<medComboClass>();
            for (int i = 0; i < list_value.Count; i++)
            {
                string code = list_value[i][(int)enum_雲端藥檔.藥品碼].ObjectToString();
                string name = list_value[i][(int)enum_雲端藥檔.藥品名稱].ObjectToString();
                medComboClass medComboClass = new medComboClass();
                medComboClass.GUID = Guid.NewGuid().ToString();
                medComboClass.藥碼 = code;
                medComboClass.藥名 = name;
                medCombos.Add(medComboClass);
            }

            sqL_DataGridView_藥品組合.AddRows(medCombos.ClassToSQL<medComboClass, enum_medCombo>(), true);
            flag_已選擇組合 = true;
        }
        private void RJ_Button_刪除組合_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = sqL_DataGridView_藥品組合.Get_All_Select_RowsValues();
            if(list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("請選擇資料");
                return;
            }
            List<medComboClass> medCombos = list_value.SQLToClass<medComboClass, enum_medCombo>();
            medComboClass.delete_by_guid(Main_Form.API_Server, medCombos);
            sqL_DataGridView_藥品組合.DeleteExtra(list_value, true);
        }
        private void RJ_Button_確認組合_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = sqL_DataGridView_藥品組合.GetAllRows();
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("無資料可上傳");
                return;
            }
            List<medComboClass> medCombos = list_value.SQLToClass<medComboClass, enum_medCombo>();
            medComboClass.delete_by_guid(Main_Form.API_Server, medCombos);

            medComboClass.add(Main_Form.API_Server, medCombos);
            MyMessageBox.ShowDialog("完成");
            flag_已選擇組合 = false;
            sqL_DataGridView_藥品組合.ClearGrid();
        }

        private void RJ_Button_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                string text = textBox_搜尋內容.Texts;
                string cmb_text = "";
                this.Invoke(new Action(delegate
                {
                    cmb_text = this.comboBox_搜尋條件.Text;
                }));

                List<medClass> medClasses = new List<medClass>();
                LoadingForm.ShowLoadingForm();
                if (cmb_text == "全部顯示")
                {
                    medClasses = medClass.get_med_cloud(Main_Form.API_Server);
                }
                if (cmb_text == "有儲位藥品")
                {
                    medClasses = medClass.get_med_cloud(Main_Form.API_Server);

                    List<Device> devices = Main_Form.Function_從SQL取得所有儲位();
                    List<string> codes = devices.Select(x => x.Code).Distinct().ToList();

                    medClasses = medClasses.Where(x => codes.Contains(x.藥品碼)).ToList();
              
                }
                if (cmb_text == "藥碼")
                {
                    if (text.StringIsEmpty())
                    {
                        Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("搜尋條件空白", 1500);
                        dialog_AlarmForm.ShowDialog();
                        return;
                    }
                    medClass medClass = medClass.get_med_clouds_by_code(Main_Form.API_Server, text);
                    medClasses.Add(medClass);
                }
                if (cmb_text == "藥名")
                {
                    if (text.StringIsEmpty())
                    {
                        Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("搜尋條件空白", 1500);
                        dialog_AlarmForm.ShowDialog();
                        return;
                    }
                    medClasses = medClass.get_med_clouds_by_name(Main_Form.API_Server, text);
                }
                if (cmb_text == "中文名")
                {
                    if (text.StringIsEmpty())
                    {
                        Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("搜尋條件空白", 1500);
                        dialog_AlarmForm.ShowDialog();
                        return;
                    }
                    medClasses = medClass.get_med_clouds_by_chtname(Main_Form.API_Server, text);
                }
                //if (IsDeviceSerch)
                //{
                //    medClasses = (from temp in medClasses
                //                  where temp.DeviceBasics.Count > 0
                //                  select temp).ToList();
                //}


                if (medClasses.Count == 0)
                {
                    Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("查無資料", 1500);
                    dialog_AlarmForm.ShowDialog();
                    return;
                }

                medClasses = (from temp in medClasses
                              where temp.開檔狀態 == enum_開檔狀態.開檔中.GetEnumName() || temp.開檔狀態.StringIsEmpty()
                              select temp).ToList();

     
                List<object[]> list_value = medClasses.ClassToSQL<medClass, enum_雲端藥檔>();



                this.sqL_DataGridView_藥品搜尋.RefreshGrid(list_value);

            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }

        }
    }
}
