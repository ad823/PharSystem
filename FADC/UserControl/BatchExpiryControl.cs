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
using H_Pannel_lib;
using NPOI.SS.Formula.Functions;

namespace FADC
{
    public partial class BatchExpiryControl : UserControl
    {
        public delegate void ConrimValueEventHandler(StockClass stockClass);
        public event ConrimValueEventHandler ConrimValueEvent;

        public string 效期 = "";
        public string 批號 = "";
        private string _藥碼;
        private string _API_Server = "";
        private string _ServerName = "";
        private string _ServerType = "";
        public BatchExpiryControl()
        {
            InitializeComponent();
          
            this.Load += BatchExpiryControl_Load;
        }
        public StockClass GetStock()
        {
            StockClass stockClass = null;
            if(rJ_DatePicker_效期.IsDefaultDate() == false)
            {
                stockClass = new StockClass();
                stockClass.Code = _藥碼;
                stockClass.Validity_period = rJ_DatePicker_效期.Value.ToDateString();
                stockClass.Lot_number = rJ_TextBox_批號.Text;
            }         
            return stockClass;
        }
        public void Init(string code, string API_Server, string ServerName, string ServerType)
        {
            this._藥碼 = code;
            this._API_Server = API_Server;
            this._ServerName = ServerName;
            this._ServerType = ServerType;

            Table table = new Table("");
            List<StockClass> stockClasses = transactionsClass.get_stock_by_code(_API_Server, _藥碼, _ServerName, _ServerType);
            stockClasses = stockClasses
            .Where(s =>
            {
                if (DateTime.TryParse(s.Validity_period, out DateTime dt))
                {
                    return dt > DateTime.Now;
                }
                return false;
            })
            .ToList();
            table.AddColumnList("GUID", Table.StringType.VARCHAR, 50, Table.IndexType.None);

            this.sqL_DataGridView_效期批號.RowsHeight = 50;
            this.sqL_DataGridView_效期批號.Init(table);
            this.Invoke(new Action(delegate 
            {
                this.sqL_DataGridView_效期批號.Set_ColumnWidth(sqL_DataGridView_效期批號.Width - 20, DataGridViewContentAlignment.MiddleLeft, "GUID");
            }));
          

            this.sqL_DataGridView_效期批號.RowPostPaintingEvent += SqL_DataGridView_效期批號_RowPostPaintingEvent;
            this.sqL_DataGridView_效期批號.RowEnterEvent += SqL_DataGridView_效期批號_RowEnterEvent;
            if (stockClasses == null) stockClasses = new List<StockClass>();
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < stockClasses.Count; i++)
            {
                object[] value = new object[] { stockClasses[i].JsonSerializationt() };
                list_value.Add(value);
                if (list_value.Count >= 3) break;
            }
            this.sqL_DataGridView_效期批號.RefreshGrid(list_value);
            rJ_DatePicker_效期.SetDefaultDate();
        }
        private void BatchExpiryControl_Load(object sender, EventArgs e)
        {
           
        }
        private void SqL_DataGridView_效期批號_RowEnterEvent(object[] RowValue)
        {
            StockClass stockClass = RowValue[0].ObjectToString().JsonDeserializet<StockClass>();
            rJ_DatePicker_效期.SetDate(stockClass.Validity_period.StringToDateTime());
            rJ_TextBox_批號.Texts = stockClass.Lot_number;
    
            if (ConrimValueEvent != null) ConrimValueEvent(stockClass);
        }
        private void SqL_DataGridView_效期批號_RowPostPaintingEvent(DataGridViewRowPostPaintEventArgs e)
        {
            Color row_Backcolor = Color.LightGray;
            Color row_Forecolor = Color.Black;

            if (this.sqL_DataGridView_效期批號.GetSelectRow() == e.RowIndex)
            {
                row_Backcolor = this.sqL_DataGridView_效期批號.selectedRowBackColor;
                row_Forecolor = this.sqL_DataGridView_效期批號.selectedRowForeColor;
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
                object[] value = this.sqL_DataGridView_效期批號.GetRowsList()[e.RowIndex];
                StockClass stockClass = value[0].ObjectToString().JsonDeserializet<StockClass>();
                string 序號 = $"{e.RowIndex + 1}.";
                string 效期 = $"{stockClass.Validity_period}";
                string 批號 = $"{stockClass.Lot_number}";


                string str = $"{序號} 效期 : {效期} 批號 {((批號.StringIsEmpty()) ? "無" : $"{批號}")}";
                DrawingClass.Draw.文字左上繪製(str, new PointF(10, y + 10), new Font("標楷體", 16), row_Forecolor, e.Graphics);

            }
        }
    }
}
