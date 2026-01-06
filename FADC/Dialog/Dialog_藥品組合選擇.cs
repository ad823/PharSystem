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
        public List<StockClass> stocks = new List<StockClass>();
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

            List<medComboClass> medComboClasses = medComboClass.get_by_code(Main_Form.API_Server, stock.Code);
            List<object[]> list_value = new List<object[]>();
            sqL_DataGridView_藥品組合.Init(new Table(new enum_藥品組合()));

            sqL_DataGridView_藥品組合.RowDoubleClickEvent += SqL_DataGridView_藥品組合_RowDoubleClickEvent;

            sqL_DataGridView_藥品組合.Set_ColumnVisible(false, new enum_藥品組合().GetEnumName());
            sqL_DataGridView_藥品組合.Set_ColumnWidth(400, enum_藥品組合.藥名);

            for (int i = 0; i < medComboClasses.Count; i++)
            {
                object[] value = new object[new enum_藥品組合().GetLength()];
                value[(int)enum_藥品組合.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_藥品組合.藥碼] = medComboClasses[i].藥碼;
                value[(int)enum_藥品組合.藥名] = medComboClasses[i].藥名;
                value[(int)enum_藥品組合.數量] = "0";
                list_value.Add(value);
            }
            sqL_DataGridView_藥品組合.AddRows(list_value, true);
        }

        private void SqL_DataGridView_藥品組合_RowDoubleClickEvent(object[] RowValue)
        {
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;

            RowValue[(int)enum_藥品組合.數量] = dialog_NumPannel.Value;
            sqL_DataGridView_藥品組合.ReplaceExtra(RowValue, true);
        }

        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = sqL_DataGridView_藥品組合.GetAllRows();
            List<StockClass> stockClasses = new List<StockClass>();
            double qty = 0;
            for (int i = 0; i < list_value.Count; i++)
            {
                string 藥碼 = list_value[i][(int)enum_藥品組合.藥碼].ObjectToString();
                string 藥名 = list_value[i][(int)enum_藥品組合.藥名].ObjectToString();
                string 數量 = list_value[i][(int)enum_藥品組合.數量].ObjectToString();

                StockClass stockClass = new StockClass();
                stockClass.Code = 藥碼;
                stockClass.Name = 藥名;
                stockClass.Qty = 數量;
                qty += stockClass.Qty.StringToDouble();
                stockClasses.Add(stockClass);
            }
            if (qty == 0)
            {
                MyMessageBox.ShowDialog($"數量皆為【0】,無法寫入");
                return;
            }
            stocks = stockClasses;
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Close();
        }
    }
}
