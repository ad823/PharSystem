using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MyUI;
using Basic;
using SQLUI;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

using System.Reflection;
using System.Runtime.InteropServices;
using MyPrinterlib;
using MyOffice;
using HIS_DB_Lib;
using H_Pannel_lib;
namespace 癌症備藥機
{
    public partial class Main_Form : Form
    {
        static public StorageUI_EPD_266 _storageUI_EPD_266;
        static public RowsLEDUI _rowsLEDUI;
        static public List<Storage> List_EPD266_本地資料 = new List<Storage>();
        static public List<RowsLED> List_RowsLED_本地資料 = new List<RowsLED>();

        
        #region 燈條亮燈
        public enum enum_燈條亮燈儲位總表
        {
            [Description("GUID,VARCHAR,15,NONE")]
            GUID,
            [Description("IP,VARCHAR,15,NONE")]
            IP,
            [Description("名稱,VARCHAR,15,NONE")]
            名稱,
            [Description("區域,VARCHAR,15,NONE")]
            區域,
        }
        public enum enum_燈條亮燈儲位列表
        {
            [Description("GUID,VARCHAR,15,NONE")]
            GUID,
            [Description("編號,VARCHAR,15,NONE")]
            編號,
            [Description("儲位名稱,VARCHAR,15,NONE")]
            儲位名稱,
            [Description("藥碼,VARCHAR,15,NONE")]
            藥碼,
            [Description("藥名,VARCHAR,15,NONE")]
            藥名,
            [Description("庫存,VARCHAR,15,NONE")]
            庫存,
        }
        #endregion
        public enum enum_效期及批號
        {
            [Description("效期,VARCHAR,15,NONE")]
            效期,
            [Description("批號,VARCHAR,15,NONE")]
            批號,
            [Description("庫存,VARCHAR,15,NONE")]
            庫存,
        }

        public enum enum_面板_儲位列表
        {
            GUID,
            IP,
            藥碼,
            藥名,
            單位,
            包裝量,
            庫存,            
        }
        private void Program_儲位設定_Init()
        {
            this.plC_RJ_Button_儲位設定_藥品搜尋_填入至儲位.MouseDownEvent += PlC_RJ_Button_儲位設定_藥品搜尋_填入至儲位_MouseDownEvent;

            #region 燈條
            this.rowsLEDUI.Init(dBConfigClass.DB_Storagelist);
            _rowsLEDUI = this.rowsLEDUI;
            this.rowsLED_Pannel.Init(Main_Form._rowsLEDUI.List_UDP_Local);
            this.rowsLED_Pannel.AutoWrite = true;


            Table table_燈條亮燈儲位總表 = new Table(new enum_燈條亮燈儲位總表());
            this.sqL_DataGridView_RowsLED_燈條列表.RowsHeight = 40;
            this.sqL_DataGridView_RowsLED_燈條列表.Init(table_燈條亮燈儲位總表);
            this.sqL_DataGridView_RowsLED_燈條列表.Set_ColumnVisible(false, new enum_燈條亮燈儲位總表().GetEnumNames());
            this.sqL_DataGridView_RowsLED_燈條列表.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleCenter, enum_燈條亮燈儲位總表.IP);
            this.sqL_DataGridView_RowsLED_燈條列表.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleLeft, enum_燈條亮燈儲位總表.名稱);
            this.sqL_DataGridView_RowsLED_燈條列表.RowEnterEvent += SqL_DataGridView_RowsLED_燈條列表_RowEnterEvent;


            Table table_燈條亮燈儲位列表 = new Table(new enum_燈條亮燈儲位列表());
            this.sqL_DataGridView_RowsLED_儲位資料.RowsHeight = 40;
            this.sqL_DataGridView_RowsLED_儲位資料.Init(table_燈條亮燈儲位列表);
            this.sqL_DataGridView_RowsLED_儲位資料.Set_ColumnVisible(false, new enum_燈條亮燈儲位列表().GetEnumNames());
            this.sqL_DataGridView_RowsLED_儲位資料.Set_ColumnWidth(60, DataGridViewContentAlignment.MiddleCenter, enum_燈條亮燈儲位列表.編號);
            this.sqL_DataGridView_RowsLED_儲位資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_燈條亮燈儲位列表.藥碼);
            this.sqL_DataGridView_RowsLED_儲位資料.Set_ColumnWidth(370, DataGridViewContentAlignment.MiddleLeft, enum_燈條亮燈儲位列表.藥名);
            this.sqL_DataGridView_RowsLED_儲位資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_燈條亮燈儲位列表.庫存);
            this.sqL_DataGridView_RowsLED_儲位資料.RowEnterEvent += SqL_DataGridView_RowsLED_儲位資料_RowEnterEvent;

            Table table_效期及批號 = new Table(new enum_效期及批號());
            this.sqL_DataGridView_儲位管理_RowsLED_效期及批號.RowsHeight = 40;
            this.sqL_DataGridView_儲位管理_RowsLED_效期及批號.Init(table_效期及批號);
            this.sqL_DataGridView_儲位管理_RowsLED_效期及批號.Set_ColumnVisible(false, new enum_效期及批號().GetEnumNames());
            this.sqL_DataGridView_儲位管理_RowsLED_效期及批號.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleCenter, enum_效期及批號.效期);
            this.sqL_DataGridView_儲位管理_RowsLED_效期及批號.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_效期及批號.批號);
            this.sqL_DataGridView_儲位管理_RowsLED_效期及批號.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_效期及批號.庫存);


            this.plC_RJ_Button_RowLED_儲位設定_效期及批號_新增.MouseDownEvent += PlC_RJ_Button_RowLED_儲位設定_效期及批號_新增_MouseDownEvent;
            this.plC_RJ_Button_RowLED_儲位設定_效期及批號_刪除.MouseDownEvent += PlC_RJ_Button_RowLED_儲位設定_效期及批號_刪除_MouseDownEvent;
            this.plC_RJ_Button_RowLED_儲位設定_效期及批號_修改.MouseDownEvent += PlC_RJ_Button_RowLED_儲位設定_效期及批號_修改_MouseDownEvent;

            this.rJ_Button_RowsLED_新增儲位.MouseDownEvent += RJ_Button_RowsLED_新增儲位_MouseDownEvent;
            this.rJ_Button_RowsLED_刪除儲位.MouseDownEvent += RJ_Button_RowsLED_刪除儲位_MouseDownEvent;
            this.rJ_Button_RowsLED_清除燈號.MouseDownEvent += RJ_Button_RowsLED_清除燈號_MouseDownEvent;
            this.rJ_Button_RowsLED_搜尋.MouseDownEvent += RJ_Button_RowsLED_搜尋_MouseDownEvent;
            comboBox_RowsLED_搜尋條件.SelectedIndex = 0;

            #endregion
            #region 面板
            this.storageUI_EPD_266.Init(dBConfigClass.DB_Storagelist);
            _storageUI_EPD_266 = this.storageUI_EPD_266;
            this.plC_ScreenPage_main.TabChangeEvent += PlC_ScreenPage_main_TabChangeEvent;
            sqL_DataGridView_儲位設定_藥品搜尋.Init(sqL_DataGridView_藥檔資料);
            sqL_DataGridView_儲位設定_藥品搜尋.Set_ColumnVisible(false, new enum_雲端藥檔().GetEnumNames());
            sqL_DataGridView_儲位設定_藥品搜尋.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品碼);
            sqL_DataGridView_儲位設定_藥品搜尋.Set_ColumnWidth(750, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品名稱);
            sqL_DataGridView_儲位設定_藥品搜尋.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.包裝單位);
            sqL_DataGridView_儲位設定_藥品搜尋.DataGridRefreshEvent += SqL_DataGridView_儲位設定_藥品搜尋_DataGridRefreshEvent;
            sqL_DataGridView_儲位設定_藥品搜尋.DataGridRowsChangeRefEvent += SqL_DataGridView_儲位設定_藥品搜尋_DataGridRowsChangeRefEvent;


            this.plC_RJ_Button_儲位設定_藥品搜尋_藥碼搜尋.MouseDownEvent += PlC_RJ_Button_儲位設定_藥品搜尋_藥碼搜尋_MouseDownEvent;
            this.plC_RJ_Button_儲位設定_藥品搜尋_藥名搜尋.MouseDownEvent += PlC_RJ_Button_儲位設定_藥品搜尋_藥名搜尋_MouseDownEvent;


            Table table = new Table("");
            table.AddColumnList("GUID", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("IP", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("藥碼", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("藥名", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("單位", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("包裝量", Table.StringType.VARCHAR, 50, Table.IndexType.None);
            table.AddColumnList("庫存", Table.StringType.VARCHAR, 50, Table.IndexType.None);

            this.sqL_DataGridView_面板_儲位列表.Init(table);
            sqL_DataGridView_面板_儲位列表.Set_ColumnVisible(false, new enum_面板_儲位列表().GetEnumNames());
            sqL_DataGridView_面板_儲位列表.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleLeft, enum_面板_儲位列表.IP);
            sqL_DataGridView_面板_儲位列表.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_面板_儲位列表.藥碼);
            sqL_DataGridView_面板_儲位列表.Set_ColumnWidth(450, DataGridViewContentAlignment.MiddleLeft, enum_面板_儲位列表.藥名);
            sqL_DataGridView_面板_儲位列表.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_面板_儲位列表.單位);
            sqL_DataGridView_面板_儲位列表.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_面板_儲位列表.包裝量);
            sqL_DataGridView_面板_儲位列表.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_面板_儲位列表.庫存);
            sqL_DataGridView_面板_儲位列表.DataGridRowsChangeRefEvent += SqL_DataGridView_面板_儲位列表_DataGridRowsChangeRefEvent;


            this.plC_RJ_Button_儲位設定_面板_儲位列表_重新整理.MouseDownEvent += PlC_RJ_Button_儲位設定_面板_儲位列表_重新整理_MouseDownEvent;
            this.plC_RJ_Button_儲位設定_面板_儲位列表_刪除儲位.MouseDownEvent += PlC_RJ_Button_儲位設定_面板_儲位列表_刪除儲位_MouseDownEvent;
            this.plC_RJ_Button_儲位設定_面板_儲位列表_上傳面板.MouseDownEvent += PlC_RJ_Button_儲位設定_面板_儲位列表_上傳面板_MouseDownEvent;
            this.plC_RJ_Button_儲位設定_面板_儲位列表_清除燈號.MouseDownEvent += PlC_RJ_Button_儲位設定_面板_儲位列表_清除燈號_MouseDownEvent;
            this.plC_RJ_Button_儲位設定_面板_儲位列表_亮燈.MouseDownEvent += PlC_RJ_Button_儲位設定_面板_儲位列表_亮燈_MouseDownEvent;
            this.plC_RJ_Button_儲位設定_效期批號修正.MouseDownEvent += PlC_RJ_Button_儲位設定_效期批號修正_MouseDownEvent;
            #endregion
            Function_取得本地儲位();
            plC_UI_Init.Add_Method(Program_儲位設定);
        }



        private void Program_儲位設定()
        {
            if (myConfigClass.主機模式 == false) return;
            sub_Program_儲位設定_出料一次();
        }
        #region PLC_儲位設定_出料一次
        PLC_Device PLC_Device_儲位設定_出料一次 = new PLC_Device("M12101");
        PLC_Device PLC_Device_儲位設定_出料一次_OK = new PLC_Device("");
        Task Task_儲位設定_出料一次;
        MyTimer MyTimer_儲位設定_出料一次_結束延遲 = new MyTimer();
        MyTimer MyTimer_儲位設定_出料一次_開始延遲 = new MyTimer();
        int cnt_Program_儲位設定_出料一次 = 65534;
        void sub_Program_儲位設定_出料一次()
        {
            if (cnt_Program_儲位設定_出料一次 == 65534)
            {
                this.MyTimer_儲位設定_出料一次_結束延遲.StartTickTime(10000);
                this.MyTimer_儲位設定_出料一次_開始延遲.StartTickTime(10000);
                PLC_Device_儲位設定_出料一次.SetComment("PLC_儲位設定_出料一次");
                PLC_Device_儲位設定_出料一次_OK.SetComment("PLC_儲位設定_出料一次_OK");
                PLC_Device_儲位設定_出料一次.Bool = false;
                cnt_Program_儲位設定_出料一次 = 65535;
            }
            if (cnt_Program_儲位設定_出料一次 == 65535) cnt_Program_儲位設定_出料一次 = 1;
            if (cnt_Program_儲位設定_出料一次 == 1) cnt_Program_儲位設定_出料一次_檢查按下(ref cnt_Program_儲位設定_出料一次);
            if (cnt_Program_儲位設定_出料一次 == 2) cnt_Program_儲位設定_出料一次_初始化(ref cnt_Program_儲位設定_出料一次);
            if (cnt_Program_儲位設定_出料一次 == 3) cnt_Program_儲位設定_出料一次_開始(ref cnt_Program_儲位設定_出料一次);
            if (cnt_Program_儲位設定_出料一次 == 4) cnt_Program_儲位設定_出料一次_等待完成(ref cnt_Program_儲位設定_出料一次);
            if (cnt_Program_儲位設定_出料一次 == 5) cnt_Program_儲位設定_出料一次 = 65500;
            if (cnt_Program_儲位設定_出料一次 > 1) cnt_Program_儲位設定_出料一次_檢查放開(ref cnt_Program_儲位設定_出料一次);

            if (cnt_Program_儲位設定_出料一次 == 65500)
            {
                PLC_Device_儲位設定_出料一次.Bool = false;
                this.MyTimer_儲位設定_出料一次_結束延遲.TickStop();
                this.MyTimer_儲位設定_出料一次_結束延遲.StartTickTime(10000);
                PLC_Device_出料一次.Bool = false;
                cnt_Program_儲位設定_出料一次 = 65535;
            }
        }
        void cnt_Program_儲位設定_出料一次_檢查按下(ref int cnt)
        {
            if (PLC_Device_儲位設定_出料一次.Bool) cnt++;
        }
        void cnt_Program_儲位設定_出料一次_檢查放開(ref int cnt)
        {
            if (!PLC_Device_儲位設定_出料一次.Bool) cnt = 65500;
        }
        void cnt_Program_儲位設定_出料一次_初始化(ref int cnt)
        {
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_面板_儲位列表.Get_All_Select_RowsValues();
            Dialog_AlarmForm dialog_AlarmForm;
            if (list_面板_儲位列表.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                cnt = 65500;
                return;
            }

            cnt++;
        }
        void cnt_Program_儲位設定_出料一次_開始(ref int cnt)
        {
            if (PLC_Device_出料一次.Bool) return;
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_面板_儲位列表.Get_All_Select_RowsValues();
            出料一次_IP = list_面板_儲位列表[0][(int)enum_面板_儲位列表.IP].ObjectToString();
            PLC_Device_出料一次.Bool = true;
            cnt++;
            return;
        }
        void cnt_Program_儲位設定_出料一次_等待完成(ref int cnt)
        {
            if (PLC_Device_出料一次.Bool) return;
            cnt++;
            return;
        }





        #endregion
        #region Function
        private void Function_儲位設定_EPD266_重新整理()
        {
            List<Storage> storages = storageUI_EPD_266.SQL_GetAllStorage();
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < storages.Count; i++)
            {
                string GUID = storages[i].GUID;
                string IP = storages[i].IP;
                string 藥碼 = storages[i].Code;
                string 藥名 = storages[i].Name;
                string 單位 = storages[i].Package;
                string 包裝量 = storages[i].Min_Package_Num;
                string 庫存 = storages[i].Inventory;
                object[] value = new object[new enum_面板_儲位列表().GetLength()];
                value[(int)enum_面板_儲位列表.GUID] = GUID;
                value[(int)enum_面板_儲位列表.IP] = IP;
                value[(int)enum_面板_儲位列表.藥碼] = 藥碼;
                value[(int)enum_面板_儲位列表.藥名] = 藥名;
                value[(int)enum_面板_儲位列表.單位] = 單位;
                value[(int)enum_面板_儲位列表.包裝量] = 包裝量;
                value[(int)enum_面板_儲位列表.庫存] = 庫存;
                list_value.Add(value);
            }
            sqL_DataGridView_面板_儲位列表.RefreshGrid(list_value);
        }
        private void Function_儲位設定_燈條亮燈_重新整理()
        {
            List<RowsLED> rowsLEDs = deviceApiClass.GetRowsLEDs(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType);
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                object[] value = new object[new enum_燈條亮燈儲位總表().GetLength()];
                value[(int)enum_燈條亮燈儲位總表.GUID] = rowsLEDs[i].IP;
                value[(int)enum_燈條亮燈儲位總表.IP] = rowsLEDs[i].IP;
                value[(int)enum_燈條亮燈儲位總表.名稱] = rowsLEDs[i].Name;
                value[(int)enum_燈條亮燈儲位總表.區域] = rowsLEDs[i].Area;
                list_value.Add(value);
            }
            this.sqL_DataGridView_RowsLED_燈條列表.RefreshGrid(list_value);
        }
        private void Function_儲位設定_重新整理()
        {
            Function_儲位設定_EPD266_重新整理();
            Function_儲位設定_燈條亮燈_重新整理();
        }
        #endregion
        #region Event
    
        private void SqL_DataGridView_面板_儲位列表_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {
            RowsList.Sort(new ICP_面板_儲位列表());
        }
        private void SqL_DataGridView_儲位設定_藥品搜尋_DataGridRefreshEvent()
        {
           
        }
        private void SqL_DataGridView_儲位設定_藥品搜尋_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {

        }
        private void PlC_RJ_Button_儲位設定_面板_儲位列表_出料一次_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_面板_儲位列表.Get_All_Select_RowsValues();
            Dialog_AlarmForm dialog_AlarmForm;
            if (list_面板_儲位列表.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            if (MyMessageBox.ShowDialog($"選取儲位將強制出料,請確認!", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            LoadingForm.ShowLoadingForm();
            string IP = list_面板_儲位列表[0][(int)enum_面板_儲位列表.IP].ObjectToString();
            List<object[]> list_replace = new List<object[]>();
            List<object[]> list_馬達輸出索引表 = this.sqL_DataGridView_馬達輸出索引表.SQL_GetRows((int)enum_storageMedBoxIOConfig.IP, IP, false);
            if (list_馬達輸出索引表.Count == 0)
            {
                Console.WriteLine($"找無馬達索引 {DateTime.Now.ToDateTimeString()}");
            }
            list_馬達輸出索引表[0][(int)enum_storageMedBoxIOConfig.出料馬達輸出觸發] = true.ToString();
            list_replace.LockAdd(list_馬達輸出索引表[0]);



            if (list_replace.Count > 0) sqL_DataGridView_馬達輸出索引表.SQL_ReplaceExtra(list_replace, false);

            LoadingForm.CloseLoadingForm();
        }
        private void PlC_RJ_Button_儲位設定_面板_儲位列表_亮燈_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_面板_儲位列表.Get_All_Select_RowsValues();
            Dialog_AlarmForm dialog_AlarmForm;
            if (list_面板_儲位列表.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            LoadingForm.ShowLoadingForm();
            List<Task> tasks = new List<Task>();
            List<Storage> storages = storageUI_EPD_266.SQL_GetAllStorage();
            Color color = Color.Black;
            this.Invoke(new Action(delegate 
            {
                if (comboBox_儲位設定_面板_儲位列表_亮燈顏色.Text == "紅")
                {
                    color = Color.Red;
                }
                if (comboBox_儲位設定_面板_儲位列表_亮燈顏色.Text == "藍")
                {
                    color = Color.Blue;
                }
                if (comboBox_儲位設定_面板_儲位列表_亮燈顏色.Text == "綠")
                {
                    color = Color.Lime;
                }
                if (comboBox_儲位設定_面板_儲位列表_亮燈顏色.Text == "白")
                {
                    color = Color.White;
                }
            }));
            for (int i = 0; i < list_面板_儲位列表.Count; i++)
            {
                string IP = list_面板_儲位列表[i][(int)enum_面板_儲位列表.IP].ObjectToString();
                Storage storage = storages.SortByIP(IP);
                if (storage == null) continue;

                tasks.Add(Task.Run(new Action(delegate
                {
                    storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                })));

            }
            Task.WhenAll(tasks).Wait();
            LoadingForm.CloseLoadingForm();
        }
        private void PlC_RJ_Button_儲位設定_面板_儲位列表_清除燈號_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_面板_儲位列表.Get_All_Select_RowsValues();
            Dialog_AlarmForm dialog_AlarmForm;
            if (list_面板_儲位列表.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            LoadingForm.ShowLoadingForm();
            List<Task> tasks = new List<Task>();
            List<Storage> storages = storageUI_EPD_266.SQL_GetAllStorage();
            for (int i = 0; i < list_面板_儲位列表.Count; i++)
            {
                string IP = list_面板_儲位列表[i][(int)enum_面板_儲位列表.IP].ObjectToString();
                Storage storage = storages.SortByIP(IP);
                if (storage == null) continue;

                tasks.Add(Task.Run(new Action(delegate
                {
                    storageUI_EPD_266.Set_Stroage_LED_UDP(storage , Color.Black);
                })));

            }
            Task.WhenAll(tasks).Wait();
            LoadingForm.CloseLoadingForm();
        }
        private void PlC_RJ_Button_儲位設定_面板_儲位列表_上傳面板_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_面板_儲位列表.Get_All_Select_RowsValues();
            Dialog_AlarmForm dialog_AlarmForm;
            if (list_面板_儲位列表.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            LoadingForm.ShowLoadingForm();
            List<Task> tasks = new List<Task>();
            List<Storage> storages = storageUI_EPD_266.SQL_GetAllStorage();
            for (int i = 0; i < list_面板_儲位列表.Count; i++)
            {
                string IP = list_面板_儲位列表[i][(int)enum_面板_儲位列表.IP].ObjectToString();
                Storage storage = storages.SortByIP(IP);
                if (storage == null) continue;

                tasks.Add(Task.Run(new Action(delegate 
                {
                    storageUI_EPD_266.DrawToEpd_UDP(storage);
                })));

            }
            Task.WhenAll(tasks).Wait();
            LoadingForm.CloseLoadingForm();
        }
        private void PlC_RJ_Button_儲位設定_面板_儲位列表_刪除儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_面板_儲位列表.Get_All_Select_RowsValues();
            Dialog_AlarmForm dialog_AlarmForm;
            if (list_面板_儲位列表.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            if (MyMessageBox.ShowDialog($"是否清除選取<{list_面板_儲位列表.Count}>筆儲內容?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            LoadingForm.ShowLoadingForm();
            for (int i = 0; i < list_面板_儲位列表.Count; i++)
            {
                string IP = list_面板_儲位列表[i][(int)enum_面板_儲位列表.IP].ObjectToString();
                Storage storage = storageUI_EPD_266.SQL_GetStorage(IP);
                storage.ClearStorage();
                storageUI_EPD_266.SQL_ReplaceStorage(storage);

                object[] value = new object[new enum_面板_儲位列表().GetLength()];
                value[(int)enum_面板_儲位列表.GUID] = storage.GUID;
                value[(int)enum_面板_儲位列表.IP] = IP;
                value[(int)enum_面板_儲位列表.庫存] = "0";
                sqL_DataGridView_面板_儲位列表.ReplaceExtra(value, true);
            }
            
            dialog_AlarmForm = new Dialog_AlarmForm("設定完成", 1000, 0, -200, Color.DarkRed);
            dialog_AlarmForm.ShowDialog();
            LoadingForm.CloseLoadingForm();
        }
        private void PlC_RJ_Button_儲位設定_藥品搜尋_填入至儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_藥品搜尋 = sqL_DataGridView_儲位設定_藥品搜尋.Get_All_Select_RowsValues();
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_面板_儲位列表.Get_All_Select_RowsValues();
            Dialog_AlarmForm dialog_AlarmForm;
            if (list_藥品搜尋.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇藥品", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
       
            string 藥碼 = list_藥品搜尋[0][(int)enum_雲端藥檔.藥品碼].ObjectToString();
            string 藥名 = list_藥品搜尋[0][(int)enum_雲端藥檔.藥品名稱].ObjectToString();
            string 單位 = list_藥品搜尋[0][(int)enum_雲端藥檔.包裝單位].ObjectToString();
            string 包裝量 = list_藥品搜尋[0][(int)enum_雲端藥檔.包裝數量].ObjectToString();

            if (this.plC_ScreenPage_儲位設定.PageText == "面板")
            {
                if (list_面板_儲位列表.Count == 0)
                {
                    dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                    dialog_AlarmForm.ShowDialog();
                    return;
                }
                string GUID = list_面板_儲位列表[0][(int)enum_面板_儲位列表.GUID].ObjectToString();
                string IP = list_面板_儲位列表[0][(int)enum_面板_儲位列表.IP].ObjectToString();

                Storage storage = storageUI_EPD_266.SQL_GetStorage(IP);
                storage.ClearStorage();
                storage.Code = 藥碼;
                storage.Name = 藥名;
                storage.Package = 單位;
                storage.Min_Package_Num = 包裝量;
                storageUI_EPD_266.SQL_ReplaceStorage(storage);

                object[] value = new object[new enum_面板_儲位列表().GetLength()];
                value[(int)enum_面板_儲位列表.GUID] = GUID;
                value[(int)enum_面板_儲位列表.IP] = IP;
                value[(int)enum_面板_儲位列表.藥碼] = 藥碼;
                value[(int)enum_面板_儲位列表.藥名] = 藥名;
                value[(int)enum_面板_儲位列表.單位] = 單位;
                value[(int)enum_面板_儲位列表.包裝量] = 包裝量;
                value[(int)enum_面板_儲位列表.庫存] = "0";
                sqL_DataGridView_面板_儲位列表.ReplaceExtra(value, true);
                dialog_AlarmForm = new Dialog_AlarmForm("設定完成", 1000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
            }
            if (this.plC_ScreenPage_儲位設定.PageText == "燈條")
            {
                List<object[]> list_燈條列表 = this.sqL_DataGridView_RowsLED_燈條列表.Get_All_Select_RowsValues();
                List<object[]> list_儲位資料 = this.sqL_DataGridView_RowsLED_儲位資料.Get_All_Select_RowsValues();

                if (list_儲位資料.Count == 0 || list_燈條列表.Count == 0)
                {
                    dialog_AlarmForm = new Dialog_AlarmForm("未選取資料", 1500);
                    dialog_AlarmForm.ShowDialog();
                    return;
                }
                string IP = list_燈條列表[0][(int)enum_燈條亮燈儲位總表.IP].ObjectToString();
                string GUID = list_儲位資料[0][(int)enum_燈條亮燈儲位列表.GUID].ObjectToString();
                RowsLED rowsLED = deviceApiClass.GetRowsLED_ByIP(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, IP);

                RowsDevice rowsDevice = rowsLED.SortByGUID(GUID);
                rowsDevice.Code = 藥碼;
                rowsDevice.Name = 藥名;
                rowsDevice.Package = 包裝量;
                rowsLED.ReplaceRowsDevice(rowsDevice);

                deviceApiClass.ReplaceRowsLED(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, rowsLED);
                this.sqL_DataGridView_RowsLED_燈條列表.On_RowEnter();
            }


        }
        private void PlC_RJ_Button_儲位設定_藥品搜尋_藥名搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_儲位設定_藥品搜尋_藥名.Texts.StringIsEmpty() == true)
            {
                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("搜尋欄位不得空白", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            List<object[]> list_value = sqL_DataGridView_儲位設定_藥品搜尋.SQL_GetAllRows(false);
            list_value = list_value.GetRowsByLike((int)enum_雲端藥檔.藥品名稱, rJ_TextBox_儲位設定_藥品搜尋_藥名.Texts);
            if (list_value.Count == 0)
            {
                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("查無資料", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            sqL_DataGridView_儲位設定_藥品搜尋.RefreshGrid(list_value);

        }
        private void PlC_RJ_Button_儲位設定_藥品搜尋_藥碼搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_儲位設定_藥品搜尋_藥碼.Texts.StringIsEmpty() == true)
            {
                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("搜尋欄位不得空白", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            List<object[]> list_value = sqL_DataGridView_儲位設定_藥品搜尋.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_雲端藥檔.藥品碼, rJ_TextBox_儲位設定_藥品搜尋_藥碼.Texts);
            if (list_value.Count == 0)
            {
                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("查無資料", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            sqL_DataGridView_儲位設定_藥品搜尋.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_儲位設定_面板_儲位列表_重新整理_MouseDownEvent(MouseEventArgs mevent)
        {
            Function_儲位設定_重新整理();
        }
        private void PlC_RJ_Button_儲位設定_效期批號修正_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_面板_儲位列表.Get_All_Select_RowsValues();
            Dialog_AlarmForm dialog_AlarmForm;
            if (list_面板_儲位列表.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            string IP = list_面板_儲位列表[0][(int)enum_面板_儲位列表.IP].ObjectToString();
            Storage storage = storageUI_EPD_266.SQL_GetStorage(IP);
            Dialog_效期批號修改 dialog_效期批號修改 = new Dialog_效期批號修改(storage, storageUI_EPD_266);
            dialog_效期批號修改.ShowDialog();

            Function_儲位設定_重新整理();
        }

        private void SqL_DataGridView_RowsLED_燈條列表_RowEnterEvent(object[] RowValue)
        {
            string IP = RowValue[(int)enum_燈條亮燈儲位總表.IP].ObjectToString();
            RowsLED rowsLED = deviceApiClass.GetRowsLED_ByIP(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, IP);
            this.rowsLED_Pannel.CurrentRowsLED = rowsLED;
            rowsLED_Pannel.Maximum = rowsLED.Maximum;

            rJ_TextBox_儲位名稱_RowsLED.Text = rowsLED.Name;

            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < rowsLED.RowsDevices.Count; i++)
            {
                object[] value = new object[new enum_燈條亮燈儲位列表().GetLength()];
                value[(int)enum_燈條亮燈儲位列表.GUID] = rowsLED.RowsDevices[i].GUID;
                value[(int)enum_燈條亮燈儲位列表.編號] = rowsLED.RowsDevices[i].Index;
                value[(int)enum_燈條亮燈儲位列表.藥碼] = rowsLED.RowsDevices[i].Code;
                value[(int)enum_燈條亮燈儲位列表.藥名] = rowsLED.RowsDevices[i].Name;
                value[(int)enum_燈條亮燈儲位列表.儲位名稱] = rowsLED.RowsDevices[i].StorageName;
                value[(int)enum_燈條亮燈儲位列表.庫存] = rowsLED.RowsDevices[i].Inventory;
                list_value.Add(value);
            }
            this.sqL_DataGridView_RowsLED_儲位資料.RefreshGrid(list_value);
        }
        private void SqL_DataGridView_RowsLED_儲位資料_RowEnterEvent(object[] RowValue)
        {
            if (this.rowsLED_Pannel.CurrentRowsLED != null) deviceApiClass.ReplaceRowsLED(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, this.rowsLED_Pannel.CurrentRowsLED);
            string GUID = RowValue[(int)enum_燈條亮燈儲位列表.GUID].ObjectToString();
            RowsLED rowsLED = this.rowsLED_Pannel.CurrentRowsLED;
            RowsDevice rowsDevice = rowsLED.SortByGUID(GUID);
            if (rowsDevice == null) return;
            this.rowsLED_Pannel.RowsDeviceGUID = GUID;

            if (rJ_RatioButton_儲位資料_RowsLED_紅.Checked)
            {
                this.rowsLED_Pannel.SliderColor = Color.Red;
            }
            else if (rJ_RatioButton_儲位資料_RowsLED_藍.Checked)
            {
                this.rowsLED_Pannel.SliderColor = Color.Blue;
            }
            else if (rJ_RatioButton_儲位資料_RowsLED_綠.Checked)
            {
                this.rowsLED_Pannel.SliderColor = Color.Lime;
            }
            else if (rJ_RatioButton_儲位資料_RowsLED_白.Checked)
            {
                this.rowsLED_Pannel.SliderColor = Color.White;
            }

            sqL_DataGridView_儲位管理_RowsLED_效期及批號.ClearGrid();

            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < rowsDevice.List_Validity_period.Count; i++)
            {
                object[] value = new object[new enum_效期及批號().GetLength()];
                value[(int)enum_效期及批號.效期] = rowsDevice.List_Validity_period[i];
                value[(int)enum_效期及批號.批號] = rowsDevice.List_Lot_number[i];
                value[(int)enum_效期及批號.庫存] = rowsDevice.List_Inventory[i];
                list_value.Add(value);
            }

            sqL_DataGridView_儲位管理_RowsLED_效期及批號.RefreshGrid(list_value);
        }
        private void RJ_Button_RowsLED_新增儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_RowsLED_燈條列表.Get_All_Select_RowsValues();
            if (list_value.Count == 0)
            {
                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("未選取資料", 1500);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            string IP = list_value[0][(int)enum_燈條亮燈儲位總表.IP].ObjectToString();
            RowsLED rowsLED = deviceApiClass.GetRowsLED_ByIP(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, IP);
            rowsLED.Add(0, 8);
            deviceApiClass.ReplaceRowsLED(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, rowsLED);
            this.sqL_DataGridView_RowsLED_燈條列表.On_RowEnter();
        }
        private void RJ_Button_RowsLED_刪除儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("是否刪除儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            List<object[]> list_燈條列表 = this.sqL_DataGridView_RowsLED_燈條列表.Get_All_Select_RowsValues();
            List<object[]> list_儲位資料 = this.sqL_DataGridView_RowsLED_儲位資料.Get_All_Select_RowsValues();
            if (list_儲位資料.Count == 0 || list_燈條列表.Count == 0)
            {
                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("未選取資料", 1500);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            string IP = list_燈條列表[0][(int)enum_燈條亮燈儲位總表.IP].ObjectToString();
            int index = list_儲位資料[0][(int)enum_燈條亮燈儲位列表.編號].StringToInt32();
            RowsLED rowsLED = deviceApiClass.GetRowsLED_ByIP(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, IP);
            rowsLED.Delete(index);

            deviceApiClass.ReplaceRowsLED(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, rowsLED);
            this.sqL_DataGridView_RowsLED_燈條列表.On_RowEnter();
        }
        private void RJ_Button_RowsLED_清除燈號_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                Color color = Color.Black;

                List<object[]> list_燈條列表 = this.sqL_DataGridView_RowsLED_燈條列表.Get_All_Select_RowsValues();
                if (list_燈條列表.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選取燈條列表");
                    return;
                }
                List<RowsLED> rowsLEDs = Main_Form._rowsLEDUI.SQL_GetAllRowsLED();
                List<RowsLED> rowsLEDs_buf = new List<RowsLED>();

                for (int i = 0; i < list_燈條列表.Count; i++)
                {
                    string IP = list_燈條列表[i][(int)enum_燈條亮燈儲位總表.IP].ObjectToString();
                    RowsLED rowsLED = rowsLEDs.SortByIP(IP);
                    if (rowsLED != null)
                    {
                        rowsLEDs_buf.Add(rowsLED);
                    }
                }
                List<Task> tasks = new List<Task>();

                for (int i = 0; i < rowsLEDs_buf.Count; i++)
                {
                    RowsLED rowsLED = rowsLEDs_buf[i];
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        Main_Form._rowsLEDUI.Set_Rows_LED_Clear_UDP(rowsLED);

                    })));

                }
                Task.WhenAll(tasks).Wait();
            }
            catch
            {

            }
            finally
            {


            }

        }
        private void RJ_Button_RowsLED_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            string text = "";
            string comboBox_text = "";
            this.Invoke(new Action(delegate
            {
                text = this.comboBox_RowsLED_搜尋內容.Text;
                comboBox_text = this.comboBox_RowsLED_搜尋條件.Text;
            }));

            if (text.StringIsEmpty()) return;
            List<RowsDevice> rowsDevices = new List<RowsDevice>();
            int select_index = -1;
            if (comboBox_text == "藥碼")
            {
                List<RowsDevice> rowsDevices_buf = Main_Form.List_RowsLED_本地資料.GetAllRowsDevices();
                rowsDevices_buf = (from temp in rowsDevices_buf
                                   where temp.Code.ToUpper().Contains(text.ToUpper())
                                   select temp).ToList();
                rowsDevices = rowsDevices_buf;
            }
            if (comboBox_text == "藥名")
            {
                List<RowsDevice> rowsDevices_buf = Main_Form.List_RowsLED_本地資料.GetAllRowsDevices();
                rowsDevices_buf = (from temp in rowsDevices_buf
                                   where temp.Code.ToUpper().Contains(text.ToUpper())
                                   select temp).ToList();
                rowsDevices = rowsDevices_buf;
            }
            if (comboBox_text == "商品名")
            {
                List<RowsDevice> rowsDevices_buf = Main_Form.List_RowsLED_本地資料.GetAllRowsDevices();
                rowsDevices_buf = (from temp in rowsDevices_buf
                                   where temp.Code.ToUpper().Contains(text.ToUpper())
                                   select temp).ToList();
                rowsDevices = rowsDevices_buf;
            }

            if (rowsDevices.Count == 0)
            {
                MyMessageBox.ShowDialog("查無無此藥品!!");
                return;
            }
            string IP = "0.0.0.0";
            int index = 0;
            if (this.rowsLED_Pannel.CurrentRowsLED != null)
            {
                IP = this.rowsLED_Pannel.CurrentRowsLED.IP;
            }
            if (this.rowsLED_Pannel.CurrentRowsDevice != null)
            {
                index = this.rowsLED_Pannel.CurrentRowsDevice.Index;
            }
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                if (rowsDevices[i].IP == IP)
                {
                    if (rowsDevices[i].Index == index)
                    {
                        select_index = i;
                    }
                }
            }
            RowsDevice rowsDevice;
            if (select_index == -1)
            {
                rowsDevice = rowsDevices[0];
            }
            else if ((select_index + 1) == rowsDevices.Count)
            {
                rowsDevice = rowsDevices[0];
            }
            else
            {
                rowsDevice = rowsDevices[select_index + 1];
            }
            RowsLED rowsLED = Main_Form.List_RowsLED_本地資料.SortByIP(rowsDevice.IP);
            if (rowsLED == null) return;
            sqL_DataGridView_RowsLED_燈條列表.SetSelectRow(enum_燈條亮燈儲位總表.IP.GetEnumName(), rowsDevice.IP);
            sqL_DataGridView_RowsLED_儲位資料.SetSelectRow(enum_燈條亮燈儲位列表.GUID.GetEnumName(), rowsDevice.GUID);
        }
        private void PlC_RJ_Button_RowLED_儲位設定_效期及批號_新增_MouseDownEvent(MouseEventArgs mevent)
        {

        }
        private void PlC_RJ_Button_RowLED_儲位設定_效期及批號_刪除_MouseDownEvent(MouseEventArgs mevent)
        {

        }
        private void PlC_RJ_Button_RowLED_儲位設定_效期及批號_修改_MouseDownEvent(MouseEventArgs mevent)
        {

        }

        #endregion



        private class ICP_面板_儲位列表 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string IP_0 = x[(int)enum_面板_儲位列表.IP].ObjectToString();
                string IP_1 = y[(int)enum_面板_儲位列表.IP].ObjectToString();
                string[] IP_0_Array = IP_0.Split('.');
                string[] IP_1_Array = IP_1.Split('.');
                IP_0 = "";
                IP_1 = "";
                for (int i = 0; i < 4; i++)
                {
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];

                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];

                    IP_0 += IP_0_Array[i];
                    IP_1 += IP_1_Array[i];
                }
                int cmp = IP_0_Array[2].CompareTo(IP_1_Array[2]);
                if (cmp > 0)
                {
                    return 1;
                }
                else if (cmp < 0)
                {
                    return -1;
                }
                else if (cmp == 0)
                {
                    cmp = IP_0_Array[3].CompareTo(IP_1_Array[3]);
                    if (cmp > 0)
                    {
                        return 1;
                    }
                    else if (cmp < 0)
                    {
                        return -1;
                    }
                    else if (cmp == 0)
                    {
                        return 0;
                    }
                }

                return 0;

            }
        }
    }
}
