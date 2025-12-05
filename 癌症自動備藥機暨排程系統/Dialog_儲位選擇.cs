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

namespace 癌症備藥機
{
    public partial class Dialog_儲位選擇 : MyDialog
    {
        private string _藥碼;
        private string _藥名;
        public Device Value = null;
  
        public Dialog_儲位選擇(string 藥碼, string 藥名)
        {
            InitializeComponent();

   

            this.Load += Dialog_儲位選擇_Load;
            this._藥碼 = 藥碼;
            this._藥名 = 藥名;

            this.rJ_Lable_藥品資訊.Text = $"({this._藥碼 }) {this._藥名 }";

            Table table = new Table("");
            table.AddColumnList("GUID", Table.StringType.VARCHAR, 50, Table.IndexType.None);

            this.sqL_DataGridView_儲位選擇.RowsHeight = 50;
            this.sqL_DataGridView_儲位選擇.Init(table);
            this.sqL_DataGridView_儲位選擇.Set_ColumnWidth(sqL_DataGridView_儲位選擇.Width - 20, DataGridViewContentAlignment.MiddleLeft, "GUID");

            this.sqL_DataGridView_儲位選擇.RowPostPaintingEvent += SqL_DataGridView_儲位選擇_RowPostPaintingEvent;
            this.sqL_DataGridView_儲位選擇.RowClickEvent += SqL_DataGridView_儲位選擇_RowClickEvent;

            List<Device> devices = Main_Form.Function_取得本地儲位();

            List<Device> devices_buf = (from temp in devices
                                          where temp.Code == 藥碼
                                          select temp).ToList();
           
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < devices_buf.Count; i++)
            {
                string json = devices_buf[i].JsonSerializationt();
                list_value.Add(new object[] { json });
            }
            this.sqL_DataGridView_儲位選擇.RefreshGrid(list_value);
            this.sqL_DataGridView_儲位選擇.SetSelectRow(0);
            this.rJ_Button_確認選擇.MouseDownEvent += RJ_Button_確認選擇_MouseDownEvent;
        }

       

        private void Dialog_儲位選擇_Load(object sender, EventArgs e)
        {
        
        }

        #region Event
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
            Device device;
            List<object[]> list_value = this.sqL_DataGridView_儲位選擇.GetAllRows();
            for (int i = 0; i < list_value.Count; i++)
            {
                device = list_value[i][0].ObjectToString().JsonDeserializet<Storage>();
                if (device.DeviceType == DeviceType.EPD266 || device.DeviceType == DeviceType.EPD290)
                {
                    Main_Form._storageUI_EPD_266.Set_Stroage_LED_UDP((Storage)device, Color.Black);
                }
                if (device.DeviceType == DeviceType.RowsLED)
                {
                    RowsDevice rowsDevice = Main_Form.List_RowsLED_本地資料.GetRowsDevice(device.GUID);
                    RowsLED rowsLED = Main_Form.List_RowsLED_本地資料.SortByIP(rowsDevice.IP);
                    rowsLED.LED_Bytes = RowsLEDUI.Get_Rows_LEDBytes(ref rowsLED.LED_Bytes, rowsDevice, Color.Black);
                    Main_Form._rowsLEDUI.Set_Rows_LED_UDP(rowsLED);
                }

     

            }
            device = RowValue[0].ObjectToString().JsonDeserializet<Device>();
            if(device != null)
            {
                if (device.DeviceType == DeviceType.EPD266 || device.DeviceType == DeviceType.EPD290)
                {
                    Main_Form._storageUI_EPD_266.Set_Stroage_LED_UDP((Storage)device, Color.Blue);
                }
                if (device.DeviceType == DeviceType.RFID_Device)
                {
                    RowsDevice rowsDevice = Main_Form.List_RowsLED_本地資料.GetRowsDevice(device.GUID);
                    RowsLED rowsLED = Main_Form.List_RowsLED_本地資料.SortByIP(rowsDevice.IP);
                    rowsLED.LED_Bytes = RowsLEDUI.Get_Rows_LEDBytes(ref rowsLED.LED_Bytes, rowsDevice, Color.Blue);
                    Main_Form._rowsLEDUI.Set_Rows_LED_UDP(rowsLED);
                }
            }
  
        }
        private void RJ_Button_確認選擇_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<object[]> list_value = this.sqL_DataGridView_儲位選擇.Get_All_Select_RowsValues();
                if(list_value.Count == 0)
                {
                    Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("未選取資料", 1500);
                    dialog_AlarmForm.ShowDialog();
                    return;
                }
                this.Value = list_value[0][0].ObjectToString().JsonDeserializet<Device>();
                this.Close();
                this.DialogResult = DialogResult.Yes;
            }));
        }
        #endregion
    }
}
