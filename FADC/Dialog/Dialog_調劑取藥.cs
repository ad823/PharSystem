using Basic;
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
using SQLUI;
using HIS_DB_Lib;
using H_Pannel_lib;
namespace FADC
{
    public partial class Dialog_調劑取藥 : MyDialog
    {
        MyThread myThread;

        public enum enum_處方藥品
        {
            [Description("GUID,VARCHAR,15,NONE")]
            GUID,
            [Description("藥碼,VARCHAR,15,NONE")]
            藥碼,
            [Description("藥名,VARCHAR,15,NONE")]
            藥名,
            [Description("領藥量,VARCHAR,15,NONE")]
            領藥量,
            [Description("已取量,VARCHAR,15,NONE")]
            已取量,
            [Description("狀態,VARCHAR,15,NONE")]
            狀態,
        }

        public Dialog_調劑取藥(List<StockClass> stockClasses)
        {
            form.Invoke(new Action(delegate 
            {
                InitializeComponent();
            }));
            this.LoadFinishedEvent += Dialog_調劑取藥_LoadFinishedEvent;
            this.FormClosed += Dialog_調劑取藥_FormClosed;
            this.sqL_DataGridView_處方藥品.Init(new Table(new enum_處方藥品()));
            this.sqL_DataGridView_處方藥品.RowsHeight = 60;
            this.sqL_DataGridView_處方藥品.Set_ColumnVisible(false, new enum_處方藥品().GetEnumNames());
            this.sqL_DataGridView_處方藥品.Set_ColumnWidth(80, enum_處方藥品.藥碼);
            this.sqL_DataGridView_處方藥品.Set_ColumnWidth(600, DataGridViewContentAlignment.MiddleLeft, enum_處方藥品.藥名);
            this.sqL_DataGridView_處方藥品.Set_ColumnWidth(100, enum_處方藥品.領藥量);
            this.sqL_DataGridView_處方藥品.Set_ColumnWidth(100, enum_處方藥品.已取量);
            this.sqL_DataGridView_處方藥品.Set_ColumnWidth(150, enum_處方藥品.狀態);

            List<object[]> objects = new List<object[]>();
            for(int i = 0; i < stockClasses.Count; i++)
            {
                object[] value = new object[new enum_處方藥品().GetLength()];
                value[(int)enum_處方藥品.GUID] = i;
                value[(int)enum_處方藥品.藥碼] = stockClasses[i].Code;
                value[(int)enum_處方藥品.藥名] = stockClasses[i].Name;
                value[(int)enum_處方藥品.領藥量] = Math.Abs(stockClasses[i].Qty.StringToInt32());
                value[(int)enum_處方藥品.已取量] = "0";
                value[(int)enum_處方藥品.狀態] = "等待中";
                objects.Add(value);
            }
            this.sqL_DataGridView_處方藥品.RefreshGrid(objects);

            this.sqL_DataGridView_處方藥品.DataGridRefreshEvent += SqL_DataGridView_處方藥品_DataGridRefreshEvent;
        }

        private void SqL_DataGridView_處方藥品_DataGridRefreshEvent()
        {
            for (int i = 0; i < this.sqL_DataGridView_處方藥品.dataGridView.Rows.Count; i++)
            {
                if (this.sqL_DataGridView_處方藥品.dataGridView.Rows[i].Cells[enum_處方藥品.狀態.GetEnumName()].Value.ToString() == "庫存不足")
                {
                    this.sqL_DataGridView_處方藥品.dataGridView.Rows[i].Cells[enum_處方藥品.狀態.GetEnumName()].Style.BackColor = Color.Red;
                    this.sqL_DataGridView_處方藥品.dataGridView.Rows[i].Cells[enum_處方藥品.狀態.GetEnumName()].Style.ForeColor = Color.White;
                }
            }
        }
        private void Dialog_調劑取藥_LoadFinishedEvent(EventArgs e)
        {
            this.sqL_DataGridView_處方藥品.ClearSelection();
            this.sqL_DataGridView_處方藥品.Enabled = false;

            myThread = new MyThread();
            myThread.Add_Method(sub_program);
            myThread.AutoRun(true);
            myThread.SetSleepTime(10);
            myThread.Trigger();

        }
        private int cnt = 1;

        private void sub_program()
        {
            if (cnt == 1)
            {
                List<object[]> objects = this.sqL_DataGridView_處方藥品.GetAllRows();
                for(int i = 0; i < objects.Count; i++)
                {
                    string code = objects[i][(int)enum_處方藥品.藥碼].ObjectToString();
                    double storage_qty = Main_Form.Function_從SQL取得庫存(code);
                    double qty = Math.Abs(objects[i][(int)enum_處方藥品.領藥量].StringToDouble());
                    if(storage_qty <  qty)
                    {
                        objects[i][(int)enum_處方藥品.狀態] = "庫存不足";
                    }
                    this.sqL_DataGridView_處方藥品.RefreshGrid(objects);
                }
                cnt++;
            }
        }
        private void Dialog_調劑取藥_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (myThread != null)
            {
                myThread.Abort();
                myThread = null;
            }
        }


    }
}
