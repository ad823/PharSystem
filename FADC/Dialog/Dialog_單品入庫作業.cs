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
using FingerprintLib;
using System.Threading;
using FaceRecognitionUserControl;
using System;
using NPOI.SS.UserModel;
using FaceRecognitionDll.Models;
using SQLUI;
using H_Pannel_lib;
namespace FADC
{
    public partial class Dialog_單品入庫作業 : MyDialog
    {
        public Dialog_單品入庫作業()
        {
            form.Invoke(new Action(delegate { InitializeComponent(); }));
            
            this.LoadFinishedEvent += Dialog_入庫作業_LoadFinishedEvent;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
        }

        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Close();
        }
        private void Dialog_入庫作業_LoadFinishedEvent(EventArgs e)
        {
            List<StepEntity> list = new List<StepEntity>();
            list.Add(new StepEntity("1", "選擇藥品", 1, "刷條碼或搜尋", eumStepState.Completed, null));
            list.Add(new StepEntity("2", "選擇儲位", 2, "選擇儲位", eumStepState.Completed, null));
            list.Add(new StepEntity("3", "效期批號輸入", 3, "選擇或輸入效期批號", eumStepState.Completed, null));
            list.Add(new StepEntity("4", "輸入數量", 4, "輸入數量", eumStepState.Waiting, null));
            list.Add(new StepEntity("5", "完成", 5, "按下確認存檔", eumStepState.Waiting, null));
            this.stepViewer1.CurrentStep = 1;
            this.stepViewer1.ListDataSource = list;

            Table table = medClass.init(Main_Form.API_Server);
            sqL_DataGridView_藥品資料.Init(table);
            sqL_DataGridView_藥品資料.Set_ColumnVisible(false, new enum_雲端藥檔().GetEnumNames());
            //sqL_DataGridView_藥品資料.Set_ColumnVisible(true, enum_雲端藥檔.藥品碼);
            //sqL_DataGridView_藥品資料.Set_ColumnVisible(true, enum_雲端藥檔.藥品名稱);
            sqL_DataGridView_藥品資料.Set_ColumnWidth(150, enum_雲端藥檔.藥品碼);
            sqL_DataGridView_藥品資料.Set_ColumnWidth(400, enum_雲端藥檔.藥品名稱);
            sqL_DataGridView_藥品資料.Set_ColumnText("藥碼", enum_雲端藥檔.藥品碼);
            sqL_DataGridView_藥品資料.Set_ColumnText("藥名", enum_雲端藥檔.藥品名稱);
            sqL_DataGridView_藥品資料.RowDoubleClickEvent += SqL_DataGridView_藥品資料_RowDoubleClickEvent;

            rJ_Button_藥品搜尋_下一步.MouseDownEvent += RJ_Button_藥品搜尋_下一步_MouseDownEvent;
            rJ_Button_藥品搜尋.MouseDownEvent += RJ_Button_藥品搜尋_MouseDownEvent;
            comboBox_藥品搜尋種類.SelectedIndex = 0;
            tabControlEx.SelectTab("藥品搜尋");
        }

      
        private void SqL_DataGridView_藥品資料_RowDoubleClickEvent(object[] RowValue)
        {
            medClass _medClass = RowValue.SQLToClass<medClass, enum_雲端藥檔>();
            if(_medClass != null )
            {
                rJ_Lable_藥品資訊_藥碼.Text = $"{_medClass.藥品碼}";
                rJ_Lable_藥品資訊_藥名.Text = $"{_medClass.藥品名稱}";
                medPicClass medPic = medPicClass.get_by_code(Main_Form.API_Server, _medClass.藥品碼);
                if(medPic != null )
                {
                    if (medPic.pic_base64.StringIsEmpty() == false)
                    {
                        pictureBox_藥品圖片.Image = medPic.pic_base64.Base64ToImage();
             
                    }
                }
                rJ_Button_藥品搜尋_下一步.Enabled = true;
            }

        }
        private void RJ_Button_藥品搜尋_下一步_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                this.stepViewer1.Next();
                tabControlEx.SelectTab("儲位選擇");
            }));
         
        }
        private void RJ_Button_藥品搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                string serch_type = comboBox_藥品搜尋種類.GetComboBoxText();
                string value = textBox_藥品搜尋內容.Text;
                List<medClass> medClasses = new List<medClass>();
                if (serch_type == "藥碼") medClasses = medClass.serch_by_BarCode(Main_Form.API_Server, value);
                if (serch_type == "藥名") medClasses = medClass.get_med_clouds_by_name(Main_Form.API_Server, value);

                List<Device> devices = Main_Form._storageUI_EPD_266.SQL_GetAllDevice();
                List<string> devices_codes = devices.Select(x => x.Code).ToList();
                List<medClass> medClasses_buf = medClasses.Where(x => devices_codes.Contains(x.藥品碼)).ToList();
                if (medClasses_buf.Count == 0)
                {
                    MyMessageBox.ShowDialog("查無資料");
                    return;
                }

                sqL_DataGridView_藥品資料.RefreshGrid(medClasses_buf.ClassToSQL<medClass, enum_雲端藥檔>());

            }
            catch
            {

            }
        
        }
    }
}
