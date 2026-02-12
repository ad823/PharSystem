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
using HIS_DB_Lib;
using System.Runtime.InteropServices;
using NPOI.SS.Formula.Functions;
using SQLUI;
using MyOffice;

namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
        public void Program_storageMedBoxIOConfig_Init()
        {
            Table table = storageMedBoxIOConfigClass.init(API_Server, ServerName, ServerType);
            sqL_DataGridView_storageMedBoxIOConfig.InitEx(table);

            plC_RJ_Button_storageMedBoxIOConfig_匯出.MouseDownEvent += PlC_RJ_Button_storageMedBoxIOConfig_匯出_MouseDownEvent;
            plC_RJ_Button_storageMedBoxIOConfig_匯入.MouseDownEvent += PlC_RJ_Button_storageMedBoxIOConfig_匯入_MouseDownEvent;

            sqL_DataGridView_storageMedBoxIOConfig.MouseDown += SqL_DataGridView_storageMedBoxIOConfig_MouseDown;

            sqL_DataGridView_storageMedBoxIOConfig.RowDoubleClickEvent += SqL_DataGridView_storageMedBoxIOConfig_RowDoubleClickEvent;

            plC_UI_Init.Add_Method(Program_storageMedBoxIOConfig);
        }

        private void SqL_DataGridView_storageMedBoxIOConfig_RowDoubleClickEvent(object[] RowValue)
        {
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
            RowValue[(int)enum_storageMedBoxIOConfig.出料馬達輸入延遲時間] = dialog_NumPannel.Value.ToString();
            sqL_DataGridView_storageMedBoxIOConfig.SQL_ReplaceExtra(RowValue, true);
        }

        private void SqL_DataGridView_storageMedBoxIOConfig_MouseDown(object sender, MouseEventArgs e)
        {
            sqL_DataGridView_storageMedBoxIOConfig.SQL_GetAllRows(true);
        }

        private void PlC_RJ_Button_storageMedBoxIOConfig_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                try
                {
                    if (this.saveFileDialog_SaveExcel.ShowDialog() != DialogResult.OK) { return; }
                    string filename = this.saveFileDialog_SaveExcel.FileName;

                    List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses = storageMedBoxIOConfigClass.get_all(API_Server, ServerName, ServerType);
                    if (storageMedBoxIOConfigClasses == null)
                    {
                        MyMessageBox.ShowDialog("伺服器回傳失敗");
                        return;
                    }

                    storageMedBoxIOConfigClasses.ClassToSQL<storageMedBoxIOConfigClass, enum_storageMedBoxIOConfig>().ToDataTable(new enum_storageMedBoxIOConfig()).NPOI_SaveFile(filename);
                    sqL_DataGridView_storageMedBoxIOConfig.SQL_GetAllRows(true);

                    MyMessageBox.ShowDialog("匯出成功");
                }
                catch
                {
                    MyMessageBox.ShowDialog("操作失敗");
                }
                finally
                {

                }


            }));
        }
        private void PlC_RJ_Button_storageMedBoxIOConfig_匯入_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                this.Invoke(new Action(delegate
                {
                    if (this.openFileDialog_LoadExcel.ShowDialog() != DialogResult.OK) { return; }

                    string filename = this.openFileDialog_LoadExcel.FileName;
                    DataTable dataTable = filename.NPOI_LoadFile();
                    List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses = dataTable.DataTableToRowList().SQLToClass<storageMedBoxIOConfigClass, enum_storageMedBoxIOConfig>();
                    storageMedBoxIOConfigClass.add_update(API_Server, ServerName, ServerType, storageMedBoxIOConfigClasses);
                    storageMedBoxIOConfigClasses.ClassToSQL<storageMedBoxIOConfigClass, enum_storageMedBoxIOConfig>().ToDataTable(new enum_storageMedBoxIOConfig()).NPOI_SaveFile(filename);

                    MyMessageBox.ShowDialog("匯入成功");

                }));
            }
            catch
            {
                MyMessageBox.ShowDialog("操作失敗");
            }
            finally
            {

            }

        }


        public void Program_storageMedBoxIOConfig()
        {

        }
    }
}
