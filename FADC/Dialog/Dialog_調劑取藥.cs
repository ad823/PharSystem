using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using MyUI;
using SQLUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
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
            for (int i = 0; i < stockClasses.Count; i++)
            {
                object[] value = new object[new enum_處方藥品().GetLength()];
                value[(int)enum_處方藥品.GUID] = i;
                value[(int)enum_處方藥品.藥碼] = stockClasses[i].Code;
                value[(int)enum_處方藥品.藥名] = stockClasses[i].Name;
                value[(int)enum_處方藥品.領藥量] = Math.Abs(stockClasses[i].Qty.StringToInt32()).ToString();
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

            Main_Form.Function_從SQL取得儲位到本地資料();

            myThread = new MyThread();
            myThread.Add_Method(sub_program);
            myThread.AutoRun(true);
            myThread.SetSleepTime(10);
            myThread.Trigger();

        }
        private int cnt = 1;
        List<object[]> objects_storages = new List<object[]>();
        List<int> list_取過X格數 = new List<int>();
        object[] objects_storage;
        private void sub_program()
        {
            if (cnt == 1)
            {
                List<object[]> objects = this.sqL_DataGridView_處方藥品.GetAllRows();
                for (int i = 0; i < objects.Count; i++)
                {
                    string code = objects[i][(int)enum_處方藥品.藥碼].ObjectToString();
                    double storage_qty = Main_Form.Function_從SQL取得庫存(code);
                    int qty = Math.Abs(objects[i][(int)enum_處方藥品.領藥量].StringToInt32());
                    if (storage_qty < qty)
                    {
                        objects[i][(int)enum_處方藥品.狀態] = "庫存不足";
                    }
                    this.sqL_DataGridView_處方藥品.RefreshGrid(objects);
                }
                cnt++;
            }
            if (cnt == 2)
            {
                objects_storages.Clear();
                List<object[]> objects = this.sqL_DataGridView_處方藥品.GetAllRows();
                Main_Form.Function_從SQL取得儲位到本地資料();
                for (int i = 0; i < objects.Count; i++)
                {
                    string code = objects[i][(int)enum_處方藥品.藥碼].ObjectToString();
                    string state = objects[i][(int)enum_處方藥品.狀態].ObjectToString();
                    int qty = objects[i][(int)enum_處方藥品.領藥量].StringToInt32();

                    if (state != "庫存不足")
                    {
                        for (int k = 0; k < (int)qty; k++)
                        {
                            List<object[]> objects_ = Main_Form.Function_取得異動儲位資訊從本地資料(code, -1);
                            if (objects_.Count > 0)
                            {
                                objects_[0][(int)Main_Form.enum_儲位資訊.藥碼] = code;
                                objects_storages.Add(objects_[0]);
                            }
                        }
                    }
                    this.sqL_DataGridView_處方藥品.RefreshGrid(objects);
                }
                cnt++;
            }
            if (cnt == 3)
            {
                bool flag_有藥品要取 = false;
                objects_storage = null;
                for (int i = 0; i < objects_storages.Count; i++)
                {
                    if (objects_storages[i][(int)Main_Form.enum_儲位資訊.狀態].ObjectToString() == "已領過") continue;

                    string IP = objects_storages[i][(int)Main_Form.enum_儲位資訊.IP].ObjectToString();
                    List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses = storageMedBoxIOConfigClass.get_all(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType);
                    storageMedBoxIOConfigClass storageMedBoxIO = storageMedBoxIOConfigClasses.Where(x => x.IP == IP).FirstOrDefault();
                    if (storageMedBoxIO == null) continue;
                    Console.WriteLine($"{IP} : {storageMedBoxIO.出料位置X},{storageMedBoxIO.出料位置Y}");
                    if (list_取過X格數.Contains(storageMedBoxIO.出料位置X.StringToInt32()) == false)
                    {
                        objects_storage = objects_storages[i];
                        list_取過X格數.Add(storageMedBoxIO.出料位置X.StringToInt32());
                        Main_Form.IP_出貨一次 = IP;
                        flag_有藥品要取 = true;
                        break;
                    }
                    else
                    {

                    }
                }
                if (flag_有藥品要取)
                {
                    cnt = 500;
                }
                else
                {
                    cnt = 1000;
                }
            }
            if (cnt == 500)
            {
                if (Main_Form.PLC_Device_出貨一次.Bool == false)
                {
                    List<object[]> objects = this.sqL_DataGridView_處方藥品.GetAllRows();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        string code = objects[i][(int)enum_處方藥品.藥碼].ObjectToString();
                        if (code == objects_storage[(int)Main_Form.enum_儲位資訊.藥碼].ObjectToString())
                        {
                            objects[i][(int)enum_處方藥品.狀態] = "取藥中";
                        }

                    }
                    this.sqL_DataGridView_處方藥品.RefreshGrid(objects);
                    Main_Form.PLC_Device_出貨一次.Bool = true;
                    cnt++;
                    return;
                }
            }
            if (cnt == 501)
            {
                if (Main_Form.PLC_Device_出貨一次.Bool == false)
                {
                    string code = "";
                    objects_storage[(int)Main_Form.enum_儲位資訊.狀態] = "已領過";
                    List<object[]> objects = this.sqL_DataGridView_處方藥品.GetAllRows();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        code = objects[i][(int)enum_處方藥品.藥碼].ObjectToString();
                        if (code == objects_storage[(int)Main_Form.enum_儲位資訊.藥碼].ObjectToString())
                        {
                            objects[i][(int)enum_處方藥品.已取量] = (objects[i][(int)enum_處方藥品.已取量].StringToInt32() + 1).ToString();
                            if (objects[i][(int)enum_處方藥品.已取量].ObjectToString() == objects[i][(int)enum_處方藥品.領藥量].ObjectToString())
                            {
                                objects[i][(int)enum_處方藥品.狀態] = "取藥完成";
                            }
                            else
                            {
                                objects[i][(int)enum_處方藥品.狀態] = "待命中";
                            }
                        }

                    }
                    Storage storage = Main_Form._storageUI_EPD_266.SQL_GetStorage(Main_Form.IP_出貨一次);
                    if (storage != null)
                    {
                        string 庫存量 = storage.Inventory;
                        string 備註 = "";
                        List<StockClass> stockClasses = storage.庫存異動(-1, true);
                        Main_Form._storageUI_EPD_266.SQL_ReplaceStorage(storage);
                        Task.Run(new Action(delegate
                        {
                            Main_Form._storageUI_EPD_266.DrawToEpd_UDP(storage);
                        }));
                        medClass _medClass = medClass.get_med_clouds_by_code(Main_Form.API_Server, code);

                        transactionsClass transactionsClass = new transactionsClass();
                        transactionsClass.GUID = Guid.NewGuid().ToString();
                        transactionsClass.藥品碼 = code;

                        if (_medClass != null)
                        {
                            transactionsClass.藥品名稱 = _medClass.藥品名稱;
                        }
                        transactionsClass.庫存量 = 庫存量;
                        transactionsClass.交易量 = "-1";
                        transactionsClass.結存量 = storage.Inventory;

                        for (int i = 0; i < stockClasses.Count; i++)
                        {
                            備註 += $"[效期]:{stockClasses[i].Validity_period},[批號]:{stockClasses[i].Lot_number}";
                            if (i != stockClasses.Count - 1) 備註 += "\n";
                        }
                        transactionsClass.備註 = 備註;

                        if (Main_Form.personpageClass_調劑畫面 != null)
                        {
                            transactionsClass.操作人 = Main_Form.personpageClass_調劑畫面.姓名;
                            transactionsClass.藥師證字號 = Main_Form.personpageClass_調劑畫面.藥師證字號;
                            transactionsClass.操作時間 = DateTime.Now.ToDateTimeString_6();
                        }
                        transactionsClass.add(Main_Form.API_Server, transactionsClass, Main_Form.ServerName, Main_Form.ServerType);
                    }

                    this.sqL_DataGridView_處方藥品.RefreshGrid(objects);
                    cnt++;
                    return;
                }
            }
            if (cnt == 502) cnt = 3;
            if (cnt == 1000)
            {
                if (Main_Form.PLC_Device_出貨到領藥平台.Bool == false)
                {
                    Main_Form.PLC_Device_出貨到領藥平台.Bool = true;
                    cnt++;
                    return;
                }
            }
            if (cnt == 1001)
            {
                if (Main_Form.PLC_Device_出貨到領藥平台.Bool == false)
                {
                    list_取過X格數.Clear();
                    bool flag_已完成 = true;
                    for (int i = 0; i < objects_storages.Count; i++)
                    {
                        if (objects_storages[i][(int)Main_Form.enum_儲位資訊.狀態].ObjectToString() == "已領過") continue;
                        flag_已完成 = false;

                    }
                    if (flag_已完成)
                    {
                        cnt = 65500;
                        return;
                    }
                    else
                    {
                        cnt = 3;
                        return;
                    }

                }
            }
            if (cnt == 65500)
            {
                "取藥完成".PlayGooleVoiceAsync(Main_Form.API_Server);
                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("取藥完成", 1500, Color.Green);
                DialogResult = DialogResult.Yes;
                this.Close();
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
