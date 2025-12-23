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
using DrawingClass;
using GestureRecognitionDll;
using System.Text.RegularExpressions;
using System.Reflection;
namespace FADC
{
    public partial class Dialog_單品入庫作業 : MyDialog
    {
        private Device device = null;
        private medClass _medClass = null;
        public Dialog_單品入庫作業()
        {
            form.Invoke(new Action(delegate { InitializeComponent(); }));
            
            this.LoadFinishedEvent += Dialog_入庫作業_LoadFinishedEvent;
            this.FormClosed += Dialog_單品入庫作業_FormClosed;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
        }

     

    
        private void Dialog_入庫作業_LoadFinishedEvent(EventArgs e)
        {
            try
            {
                LoadingForm.ShowLoadingForm();
                List<StepEntity> list = new List<StepEntity>();
                list.Add(new StepEntity("1", "選擇藥品", 1, "刷條碼或搜尋", eumStepState.Completed, null));
                list.Add(new StepEntity("2", "選擇儲位", 2, "選擇儲位", eumStepState.Completed, null));
                list.Add(new StepEntity("3", "效期批號輸入", 3, "選擇或輸入效期批號", eumStepState.Completed, null));
                list.Add(new StepEntity("4", "確認結果", 4, "關閉抽屜或按確認完成", eumStepState.Waiting, null));
                this.stepViewer1.CurrentStep = 1;
                this.stepViewer1.ListDataSource = list;

                Table table = medClass.init(Main_Form.API_Server);
                sqL_DataGridView_藥品資料.Init(table);
                sqL_DataGridView_藥品資料.Set_ColumnVisible(false, new enum_雲端藥檔().GetEnumNames());
                sqL_DataGridView_藥品資料.Height = 80;
                sqL_DataGridView_藥品資料.Set_ColumnWidth(100, enum_雲端藥檔.藥品碼);
                sqL_DataGridView_藥品資料.Set_ColumnWidth(500, enum_雲端藥檔.藥品名稱);
                sqL_DataGridView_藥品資料.Set_ColumnText("藥碼", enum_雲端藥檔.藥品碼);
                sqL_DataGridView_藥品資料.Set_ColumnText("藥名", enum_雲端藥檔.藥品名稱);
                sqL_DataGridView_藥品資料.RowDoubleClickEvent += SqL_DataGridView_藥品資料_RowDoubleClickEvent;
                sqL_DataGridView_藥品資料.RowEnterEvent += SqL_DataGridView_藥品資料_RowEnterEvent;
                rJ_Button_下一步.MouseDownEvent += RJ_Button_下一步_MouseDownEvent;
                rJ_Button_藥品搜尋.MouseDownEvent += RJ_Button_藥品搜尋_MouseDownEvent;

                comboBox_藥品搜尋種類.SelectedIndex = 0;
                RJ_Button_藥品搜尋_MouseDownEvent(null);

    
                table = new Table("");
                table.AddColumnList("GUID", Table.StringType.VARCHAR, 50, Table.IndexType.None);

                this.sqL_DataGridView_儲位選擇.RowsHeight = 50;
                this.sqL_DataGridView_儲位選擇.Init(table);
                this.sqL_DataGridView_儲位選擇.Set_ColumnWidth(sqL_DataGridView_儲位選擇.Width - 20, DataGridViewContentAlignment.MiddleLeft, "GUID");
                this.sqL_DataGridView_儲位選擇.RowPostPaintingEvent += SqL_DataGridView_儲位選擇_RowPostPaintingEvent;
                this.sqL_DataGridView_儲位選擇.RowClickEvent += SqL_DataGridView_儲位選擇_RowClickEvent;

                this.rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;

                tabControlEx.SelectTab("藥品搜尋");
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
           
        }


        private void Dialog_單品入庫作業_FormClosed(object sender, FormClosedEventArgs e)
        {
            gestureRecognitionCanvas.StopCaptureSoft();
        }
        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            double 庫存 = Main_Form.Function_從SQL取得庫存(_medClass.藥品碼);
            double 異動 = rJ_Lable_數量.Text.StringToDouble();
            double 結存 = 庫存 + 異動;
            transactionsClass transactionsClass = new transactionsClass();
            transactionsClass.GUID = Guid.NewGuid().ToString();
            transactionsClass.動作 = enum_交易記錄查詢動作.入庫作業.GetEnumName();
            transactionsClass.藥品碼 = _medClass.藥品碼;
            transactionsClass.藥品名稱 = _medClass.藥品名稱;
            transactionsClass.庫存量 = 庫存.ToString();
            transactionsClass.交易量 = 異動.ToString();
            transactionsClass.結存量 = 結存.ToString();
            transactionsClass.收支原因 = $"({device.IP})";
            transactionsClass.備註 = $"[效期]:{rJ_Lable_效期.Text},[批號]:{rJ_Lable_批號.Text}";
    
            device.效期庫存異動(rJ_Lable_效期.Text, rJ_Lable_批號.Text, 異動.ToString());
            Main_Form._storageUI_EPD_266.SQL_ReplaceStorage((Storage)device);
            transactionsClass.add(Main_Form.API_Server, transactionsClass, Main_Form.ServerName, Main_Form.ServerType);

            this.Close();
        }
        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Close();
        }
        private void RJ_Button_下一步_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if(tabControlEx.SelectedTab.Text == "藥品搜尋")
                {
                    List<Device> devices = Main_Form.Function_從SQL取得所有儲位();

                    List<Device> devices_buf = (from temp in devices
                                                where temp.Code == _medClass.藥品碼
                                                select temp).ToList();

                    List<object[]> list_value = new List<object[]>();
                    for (int i = 0; i < devices_buf.Count; i++)
                    {
                        string json = devices_buf[i].JsonSerializationt();
                        list_value.Add(new object[] { json });
                    }
                    this.sqL_DataGridView_儲位選擇.RefreshGrid(list_value);
                    this.sqL_DataGridView_儲位選擇.SetSelectRow(0);
                    this.stepViewer1.Next();
                    tabControlEx.SelectTab("儲位選擇");
                    //rJ_Button_下一步.Enabled = false;
                }
                else if (tabControlEx.SelectedTab.Text == "儲位選擇")
                {
                    Task.Run(new Action(delegate 
                    {
                        batchExpiryControl.Init(_medClass.藥品碼, Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType);
                        Main_Form._storageUI_EPD_266.Set_LockOpen(device.IP, device.Port);
                    }));
                
                    this.stepViewer1.Next();
                    tabControlEx.SelectTab("效期批號輸入");

                }
                else if (tabControlEx.SelectedTab.Text == "效期批號輸入")
                {
                    if(batchExpiryControl.GetStock() == null)
                    {               
                        MyMessageBox.ShowDialog("未選擇或輸入效期及批號");
                        return;
                    }
                    if(this.userControl_NumPanel1.Value == 0)
                    {
                        MyMessageBox.ShowDialog("入庫數量不得為'0'");
                        return;
                    }
                    panel_下一步.Visible = false;
                    rJ_Lable_藥碼.Text = _medClass.藥品碼;
                    rJ_Lable_藥名.Text = _medClass.藥品名稱;
                    rJ_Lable_效期.Text = batchExpiryControl.GetStock().Validity_period;
                    rJ_Lable_批號.Text = batchExpiryControl.GetStock().Lot_number;
                    rJ_Lable_數量.Text = userControl_NumPanel1.Value.ToString();

                    gestureRecognitionCanvas.UpdateRecognitionResultEvent += GestureRecognitionCanvas_UpdateRecognitionResultEvent;
                    gestureRecognitionCanvas.StartCapture(Main_Form.videoCapture);
                    Console.WriteLine($"手勢感測開始...");

                    this.stepViewer1.Next();
                    tabControlEx.SelectTab("確認結果");

                }
                else if (tabControlEx.SelectedTab.Text == "確認結果")
                {
                
                }
            }));
        }
        private void GestureRecognitionCanvas_UpdateRecognitionResultEvent(StringBuilder builder, GestureRecognitionDll.Response<HandPoseInfo> result)
        {
            // 顯示 Log
           
            if (result != null && result.State && result.Data != null)
            {
                Console.WriteLine($"手勢: {result.Data.Pose}");
                if (result.Data.Pose == "ok" || result.Data.Pose == "good")
                {
                    Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("【確認手勢】", 1500, Color.Green);
                    dialog_AlarmForm.ShowDialog();
                    RJ_Button_確認_MouseDownEvent(null);
                }
                else if(result.Data.Pose == "bad")
                {
                    Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("【取消手勢】", 1500, Color.Green);
                    dialog_AlarmForm.ShowDialog();
                    RJ_Button_取消_MouseDownEvent(null);

                }
            }
            else
            {
                //Console.WriteLine($"無法辨識");
            }

        }
        private void SqL_DataGridView_藥品資料_RowDoubleClickEvent(object[] RowValue)
        {
       
        }
        private void SqL_DataGridView_藥品資料_RowEnterEvent(object[] RowValue)
        {
            medClass _medClass = RowValue.SQLToClass<medClass, enum_雲端藥檔>();
            if (_medClass != null)
            {
                rJ_Lable_藥品資訊_藥碼.Text = $"{_medClass.藥品碼}";
                rJ_Lable_藥品資訊_藥名.Text = $"{_medClass.藥品名稱}";
                rJ_Lable_儲位選擇_藥品資訊.Text = $"({_medClass.藥品碼}){_medClass.藥品名稱}";
                medPicClass medPic = medPicClass.get_by_code(Main_Form.API_Server, _medClass.藥品碼);
                if (medPic != null)
                {
                    if (medPic.pic_base64.StringIsEmpty() == false)
                    {
                        pictureBox_藥品圖片.Image = medPic.pic_base64.Base64ToImage();

                    }
                }
                this._medClass = _medClass;
                rJ_Button_下一步.Enabled = true;
            }
        }
        private void RJ_Button_藥品搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                string serch_type = comboBox_藥品搜尋種類.GetComboBoxText();
                string value = textBox_藥品搜尋內容.Text;
                List<medClass> medClasses = new List<medClass>();
                if (serch_type == "全部顯示") medClasses = medClass.get_med_cloud(Main_Form.API_Server);
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


        private void SqL_DataGridView_儲位選擇_RowPostPaintingEvent(DataGridViewRowPostPaintEventArgs e)
        {
            Color row_Backcolor = Color.LightGray;
            Color row_Forecolor = Color.Black;

            if (this.sqL_DataGridView_儲位選擇.GetSelectRow() == e.RowIndex)
            {
                row_Backcolor = this.sqL_DataGridView_儲位選擇.selectedRowBackColor;
                row_Forecolor = this.sqL_DataGridView_儲位選擇.selectedRowForeColor;
            }

            using (Brush brush = new SolidBrush(row_Backcolor))
            {
                int x = e.RowBounds.Left;
                int y = e.RowBounds.Top;
                int width = e.RowBounds.Width;
                int height = e.RowBounds.Height;
                e.Graphics.FillRectangle(brush, e.RowBounds);
                DrawingClass.Draw.DrawRoundShadow(e.Graphics, new RectangleF(x - 1, y - 1, width, height), Color.DarkGray, 5, 5);

                Size size = new Size();
                PointF pointF = new PointF();
                object[] value = this.sqL_DataGridView_儲位選擇.GetRowsList()[e.RowIndex];
                Device device = value[0].ObjectToString().JsonDeserializet<Device>();
                string 序號 = $"{e.RowIndex + 1}.";
                string IP = $"({device.IP})";
                string 儲位名稱 = $"[{device.StorageName}]";
                string 庫存 = $"庫存:{device.Inventory}";

                DrawingClass.Draw.文字左上繪製(序號, new PointF(10, y + 10), new Font("標楷體", 16), row_Forecolor, e.Graphics);
                DrawingClass.Draw.文字左上繪製(IP, new PointF(50, y + 10), new Font("標楷體", 16, FontStyle.Bold), row_Forecolor, e.Graphics);
                DrawingClass.Draw.文字左上繪製(儲位名稱, new PointF(250, y + 10), new Font("標楷體", 16, FontStyle.Bold), row_Forecolor, e.Graphics);

                size = 庫存.MeasureText(new Font("標楷體", 16, FontStyle.Bold));
                DrawingClass.Draw.文字左上繪製(庫存, new PointF(e.RowBounds.Width - size.Width - 10, y + 10), new Font("標楷體", 16, FontStyle.Bold), Color.Black, e.Graphics);

            }
        }
        private void SqL_DataGridView_儲位選擇_RowClickEvent(object[] RowValue)
        {
            List<object[]> list_value = this.sqL_DataGridView_儲位選擇.GetAllRows();
            Task.Run(new Action(delegate 
            {
                for (int i = 0; i < list_value.Count; i++)
                {
                    device = list_value[i][0].ObjectToString().JsonDeserializet<Storage>();
                    Main_Form._storageUI_EPD_266.Set_Stroage_LED_UDP((Storage)device, Color.Black);
                }
                device = RowValue[0].ObjectToString().JsonDeserializet<Storage>();
                if (device != null)
                {
                    Main_Form._storageUI_EPD_266.Set_Stroage_LED_UDP((Storage)device, Color.Blue);
                    rJ_Button_下一步.Enabled = true;
                }
            }));
          

        }
    }
}
