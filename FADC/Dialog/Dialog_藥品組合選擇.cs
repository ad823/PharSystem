using MyUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using H_Pannel_lib;
using HIS_DB_Lib;
using SQLUI;
using Basic;

namespace FADC
{
    public partial class Dialog_藥品組合選擇 : MyDialog
    {
        public enum enum_藥品組合
        {
            [Description("GUID,VARCHAR,15,NONE")]
            GUID,
            [Description("藥碼,VARCHAR,15,NONE")]
            藥碼,
            [Description("藥名,VARCHAR,15,NONE")]
            藥名,
            [Description("數量,VARCHAR,15,NONE")]
            數量,
        }
        private StockClass stock = new StockClass();
        public Dialog_藥品組合選擇(StockClass stockClass)
        {
            form.Invoke(new Action(delegate { InitializeComponent(); }));

            stock = stockClass;

            this.LoadFinishedEvent += Dialog_藥品組合選擇_LoadFinishedEvent;

            
        }

        private void Dialog_藥品組合選擇_LoadFinishedEvent(EventArgs e)
        {
            rJ_Lable_藥品資訊.Text = $"({stock.Code}){stock.Name} : {stock.Qty}";
            rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;

            sqL_DataGridView_藥品組合.Init(new Table(new enum_藥品組合()));
            sqL_DataGridView_藥品組合.Set_ColumnVisible(false, new enum_藥品組合().GetEnumName());
        }
       
        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            
        }
        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {

        }
    }
}
