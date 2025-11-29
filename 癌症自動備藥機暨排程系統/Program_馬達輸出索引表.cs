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
using DeltaMotor485;
using H_Pannel_lib;
namespace 癌症備藥機
{
    public enum enum_storageMedBoxIOConfig_匯出
    {
        IP,
        鎖控輸出索引,
        鎖控輸入索引,
        出料馬達輸出索引,
        出料馬達輸入索引,
        出料馬達輸入延遲時間,
        出料位置X,
        出料位置Y,
        藥盒方位,
        區域,
    }
    public enum enum_storageMedBoxIOConfig_匯入
    {
        IP,
        鎖控輸出索引,
        鎖控輸入索引,
        出料馬達輸出索引,
        出料馬達輸入索引,
        出料馬達輸入延遲時間,
        出料位置X,
        出料位置Y,
        藥盒方位,
        區域,
    }
    public partial class Main_Form : Form
    {
        private static SQL_DataGridView _sqL_DataGridView_馬達輸出索引表;
        private void Program_馬達輸出索引表_Init()
        {
            string url = $"{API_Server}/api/CPMP_StorageConfig/init";
            returnData returnData = new returnData();
            returnData.ServerName = "cheom";
            returnData.ServerType = "癌症備藥機";
            string json_in = returnData.JsonSerializationt();
            string json = Basic.Net.WEBApiPostJson($"{url}", json_in);

            Table table = json.JsonDeserializet<Table>();
            this.sqL_DataGridView_馬達輸出索引表.Server = table.Server;
            this.sqL_DataGridView_馬達輸出索引表.DataBaseName = table.DBName;
            this.sqL_DataGridView_馬達輸出索引表.UserName = table.Username;
            this.sqL_DataGridView_馬達輸出索引表.Password = table.Password;
            this.sqL_DataGridView_馬達輸出索引表.Port = table.Port.StringToUInt32();
            this.sqL_DataGridView_馬達輸出索引表.SSLMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            this.sqL_DataGridView_馬達輸出索引表.MouseDown += SqL_DataGridView_馬達輸出索引表_MouseDown;
            this.sqL_DataGridView_馬達輸出索引表.Init(table);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnVisible(false, new enum_storageMedBoxIOConfig().GetEnumNames());
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.IP);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.鎖控輸出索引);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.鎖控輸入索引);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.出料馬達輸出索引);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.出料馬達輸入索引);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.出料馬達輸入延遲時間);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.出料位置X);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.出料位置Y);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.藥盒方位);
            this.sqL_DataGridView_馬達輸出索引表.Set_ColumnWidth(80, enum_storageMedBoxIOConfig.區域);
            this.sqL_DataGridView_馬達輸出索引表.DataGridRowsChangeRefEvent += SqL_DataGridView_馬達輸出索引表_DataGridRowsChangeRefEvent;
            _sqL_DataGridView_馬達輸出索引表 = this.sqL_DataGridView_馬達輸出索引表;


            this.plC_RJ_Button_馬達輸出索引表_匯出.MouseDownEvent += PlC_RJ_Button_馬達輸出索引表_匯出_MouseDownEvent;
            this.plC_RJ_Button_馬達輸出索引表_匯入.MouseDownEvent += PlC_RJ_Button_馬達輸出索引表_匯入_MouseDownEvent;
            this.plC_RJ_Button_馬達輸出索引表_出料一次.MouseDownEvent += PlC_RJ_Button_馬達輸出索引表_出料一次_MouseDownEvent;
            this.plC_RJ_Button_馬達輸出索引表_出料測試.MouseDownEvent += PlC_RJ_Button_馬達輸出索引表_出料測試_MouseDownEvent;
            this.plC_RJ_Button_馬達輸出索引表_刪除.MouseDownEvent += PlC_RJ_Button_馬達輸出索引表_刪除_MouseDownEvent;
            this.plC_UI_Init.Add_Method(Program_馬達輸出索引表);
        }

     

        private void Program_馬達輸出索引表()
        {
            if (myConfigClass.主機模式 == false) return;
            sub_Program_馬達輸出索引表_檢查輸入輸出狀態();
            sub_Program_馬達輸出索引表_檢查馬達輸出();
        }

        #region PLC_馬達輸出索引表_檢查輸入輸出狀態
        PLC_Device PLC_Device_馬達輸出索引表_檢查輸入輸出狀態 = new PLC_Device("");
        PLC_Device PLC_Device_馬達輸出索引表_檢查輸入輸出狀態_OK = new PLC_Device("");
        Task Task_馬達輸出索引表_檢查輸入輸出狀態;
        MyTimer MyTimer_馬達輸出索引表_檢查輸入輸出狀態_結束延遲 = new MyTimer();
        int cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 = 65534;
        void sub_Program_馬達輸出索引表_檢查輸入輸出狀態()
        {
            PLC_Device_馬達輸出索引表_檢查輸入輸出狀態.Bool = true;
            if (cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 == 65534)
            {
                this.MyTimer_馬達輸出索引表_檢查輸入輸出狀態_結束延遲.StartTickTime(50);
                PLC_Device_馬達輸出索引表_檢查輸入輸出狀態.SetComment("PLC_馬達輸出索引表_檢查輸入輸出狀態");
                PLC_Device_馬達輸出索引表_檢查輸入輸出狀態_OK.SetComment("PLC_馬達輸出索引表_檢查輸入輸出狀態_OK");
                PLC_Device_馬達輸出索引表_檢查輸入輸出狀態.Bool = false;
                cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 = 65535;
            }
            if (cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 == 65535) cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 = 1;
            if (cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 == 1) cnt_Program_馬達輸出索引表_檢查輸入輸出狀態_檢查按下(ref cnt_Program_馬達輸出索引表_檢查輸入輸出狀態);
            if (cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 == 2) cnt_Program_馬達輸出索引表_檢查輸入輸出狀態_初始化(ref cnt_Program_馬達輸出索引表_檢查輸入輸出狀態);
            if (cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 == 3) cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 = 65500;
            if (cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 > 1) cnt_Program_馬達輸出索引表_檢查輸入輸出狀態_檢查放開(ref cnt_Program_馬達輸出索引表_檢查輸入輸出狀態);

            if (cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 == 65500)
            {
                this.MyTimer_馬達輸出索引表_檢查輸入輸出狀態_結束延遲.TickStop();
                this.MyTimer_馬達輸出索引表_檢查輸入輸出狀態_結束延遲.StartTickTime(50);
                PLC_Device_馬達輸出索引表_檢查輸入輸出狀態.Bool = false;
                PLC_Device_馬達輸出索引表_檢查輸入輸出狀態_OK.Bool = false;
                cnt_Program_馬達輸出索引表_檢查輸入輸出狀態 = 65535;
            }
        }
        void cnt_Program_馬達輸出索引表_檢查輸入輸出狀態_檢查按下(ref int cnt)
        {
            if (PLC_Device_馬達輸出索引表_檢查輸入輸出狀態.Bool) cnt++;
        }
        void cnt_Program_馬達輸出索引表_檢查輸入輸出狀態_檢查放開(ref int cnt)
        {
            if (!PLC_Device_馬達輸出索引表_檢查輸入輸出狀態.Bool) cnt = 65500;
        }
        void cnt_Program_馬達輸出索引表_檢查輸入輸出狀態_初始化(ref int cnt)
        {
            if (this.MyTimer_馬達輸出索引表_檢查輸入輸出狀態_結束延遲.IsTimeOut())
            {
                if (Task_馬達輸出索引表_檢查輸入輸出狀態 == null)
                {
                    Task_馬達輸出索引表_檢查輸入輸出狀態 = new Task(new Action(delegate { Function_馬達輸出索引表_檢查輸入輸出狀態(); }));
                }
                if (Task_馬達輸出索引表_檢查輸入輸出狀態.Status == TaskStatus.RanToCompletion)
                {
                    Task_馬達輸出索引表_檢查輸入輸出狀態 = new Task(new Action(delegate { Function_馬達輸出索引表_檢查輸入輸出狀態(); }));
                }
                if (Task_馬達輸出索引表_檢查輸入輸出狀態.Status == TaskStatus.Created)
                {
                    Task_馬達輸出索引表_檢查輸入輸出狀態.Start();

                }
                cnt++;
            }
        }







        #endregion
        #region PLC_馬達輸出索引表_檢查馬達輸出
        PLC_Device PLC_Device_馬達輸出索引表_檢查馬達輸出 = new PLC_Device("");
        PLC_Device PLC_Device_馬達輸出索引表_檢查馬達輸出_OK = new PLC_Device("");
        Task Task_馬達輸出索引表_檢查馬達輸出;
        MyTimer MyTimer_馬達輸出索引表_檢查馬達輸出_結束延遲 = new MyTimer();
        int cnt_Program_馬達輸出索引表_檢查馬達輸出 = 65534;
        void sub_Program_馬達輸出索引表_檢查馬達輸出()
        {
            PLC_Device_馬達輸出索引表_檢查馬達輸出.Bool = true;
            if (cnt_Program_馬達輸出索引表_檢查馬達輸出 == 65534)
            {
                this.MyTimer_馬達輸出索引表_檢查馬達輸出_結束延遲.StartTickTime(50);
                PLC_Device_馬達輸出索引表_檢查馬達輸出.SetComment("PLC_馬達輸出索引表_檢查馬達輸出");
                PLC_Device_馬達輸出索引表_檢查馬達輸出_OK.SetComment("PLC_馬達輸出索引表_檢查馬達輸出_OK");
                PLC_Device_馬達輸出索引表_檢查馬達輸出.Bool = false;
                cnt_Program_馬達輸出索引表_檢查馬達輸出 = 65535;
            }
            if (cnt_Program_馬達輸出索引表_檢查馬達輸出 == 65535) cnt_Program_馬達輸出索引表_檢查馬達輸出 = 1;
            if (cnt_Program_馬達輸出索引表_檢查馬達輸出 == 1) cnt_Program_馬達輸出索引表_檢查馬達輸出_檢查按下(ref cnt_Program_馬達輸出索引表_檢查馬達輸出);
            if (cnt_Program_馬達輸出索引表_檢查馬達輸出 == 2) cnt_Program_馬達輸出索引表_檢查馬達輸出_初始化(ref cnt_Program_馬達輸出索引表_檢查馬達輸出);
            if (cnt_Program_馬達輸出索引表_檢查馬達輸出 == 3) cnt_Program_馬達輸出索引表_檢查馬達輸出 = 65500;
            if (cnt_Program_馬達輸出索引表_檢查馬達輸出 > 1) cnt_Program_馬達輸出索引表_檢查馬達輸出_檢查放開(ref cnt_Program_馬達輸出索引表_檢查馬達輸出);

            if (cnt_Program_馬達輸出索引表_檢查馬達輸出 == 65500)
            {
                this.MyTimer_馬達輸出索引表_檢查馬達輸出_結束延遲.TickStop();
                this.MyTimer_馬達輸出索引表_檢查馬達輸出_結束延遲.StartTickTime(50);
                PLC_Device_馬達輸出索引表_檢查馬達輸出.Bool = false;
                PLC_Device_馬達輸出索引表_檢查馬達輸出_OK.Bool = false;
                cnt_Program_馬達輸出索引表_檢查馬達輸出 = 65535;
            }
        }
        void cnt_Program_馬達輸出索引表_檢查馬達輸出_檢查按下(ref int cnt)
        {
            if (PLC_Device_馬達輸出索引表_檢查馬達輸出.Bool) cnt++;
        }
        void cnt_Program_馬達輸出索引表_檢查馬達輸出_檢查放開(ref int cnt)
        {
            if (!PLC_Device_馬達輸出索引表_檢查馬達輸出.Bool) cnt = 65500;
        }
        void cnt_Program_馬達輸出索引表_檢查馬達輸出_初始化(ref int cnt)
        {
            if (this.MyTimer_馬達輸出索引表_檢查馬達輸出_結束延遲.IsTimeOut())
            {
                if (Task_馬達輸出索引表_檢查馬達輸出 == null)
                {
                    Task_馬達輸出索引表_檢查馬達輸出 = new Task(new Action(delegate { Function_馬達輸出索引表_檢查馬達輸出(); }));
                }
                if (Task_馬達輸出索引表_檢查馬達輸出.Status == TaskStatus.RanToCompletion)
                {
                    Task_馬達輸出索引表_檢查馬達輸出 = new Task(new Action(delegate { Function_馬達輸出索引表_檢查馬達輸出(); }));
                }
                if (Task_馬達輸出索引表_檢查馬達輸出.Status == TaskStatus.Created)
                {
                    Task_馬達輸出索引表_檢查馬達輸出.Start();

                }
                cnt++;
            }
        }







        #endregion
        #region Function

        private void Function_馬達輸出索引表_檢查輸入輸出狀態()
        {
            List<object[]> list_value = this.sqL_DataGridView_馬達輸出索引表.SQL_GetAllRows(false);                      
            for (int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_storageMedBoxIOConfig.IP].ObjectToString();
                string 馬達輸出 = list_value[i][(int)enum_storageMedBoxIOConfig.出料馬達輸出索引].ObjectToString();
                Storage storage = List_EPD266_本地資料.SortByIP(IP);
                if (storage == null) continue;
                bool flag_output = storageUI_EPD_266.Get_OutputPIN(storage);
                PLC_Device pLC_Device = new PLC_Device($"{馬達輸出}");
                pLC_Device.Bool = flag_output;
            }
        }
        private void Function_馬達輸出索引表_檢查馬達輸出()
        {
            List<object[]> list_value = this.sqL_DataGridView_馬達輸出索引表.SQL_GetAllRows(false);
            List<Task> tasks = new List<Task>();
            List<object[]> list_replace = new List<object[]>();
            for (int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_storageMedBoxIOConfig.IP].ObjectToString();
                string 出料馬達輸出觸發 = list_value[i][(int)enum_storageMedBoxIOConfig.出料馬達輸出觸發].ObjectToString();
                if (出料馬達輸出觸發 != true.ToString()) continue;
                Storage storage = List_EPD266_本地資料.SortByIP(IP);
                int ms = list_value[0][(int)enum_storageMedBoxIOConfig.出料馬達輸入延遲時間].StringToInt32();
                if (ms < 0) ms = 0;
                tasks.Add(Task.Run(new Action(delegate
                {
                    storageUI_EPD_266.Set_ADCMotorTrigger(storage, ms);
                    Console.WriteLine($"[出料馬達輸出] {storage.IP} ({storage.Code}){storage.Name} {DateTime.Now.ToDateTimeString()}");
                })));
                list_value[i][(int)enum_storageMedBoxIOConfig.出料馬達輸出觸發] = false.ToString();
                list_replace.LockAdd(list_value[i]);
            }
            Task.WhenAll(tasks).Wait();
            if (list_replace.Count > 0) sqL_DataGridView_馬達輸出索引表.SQL_ReplaceExtra(list_replace, false);
        }
        #endregion
        #region Event
        private void SqL_DataGridView_馬達輸出索引表_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {
            RowsList.Sort(new ICP_馬達輸出索引表());
        }
        private void SqL_DataGridView_馬達輸出索引表_MouseDown(object sender, MouseEventArgs e)
        {
            this.sqL_DataGridView_馬達輸出索引表.SQL_GetAllRows(true);
        }
        private void PlC_RJ_Button_馬達輸出索引表_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (this.saveFileDialog_SaveExcel.ShowDialog() != DialogResult.OK) return;
                List<object[]> list_value = this.sqL_DataGridView_馬達輸出索引表.SQL_GetAllRows(false);
                list_value.Sort(new ICP_馬達輸出索引表());
                DataTable dataTable = list_value.ToDataTable(new enum_storageMedBoxIOConfig());
                dataTable = dataTable.ReorderTable(new enum_storageMedBoxIOConfig_匯出());

                dataTable.NPOI_SaveFile(this.saveFileDialog_SaveExcel.FileName);

                MyMessageBox.ShowDialog("匯出完成!");

            }));
        }
        private void PlC_RJ_Button_馬達輸出索引表_匯入_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (this.openFileDialog_LoadExcel.ShowDialog() != DialogResult.OK) return;
                DataTable dataTable = MyOffice.ExcelClass.NPOI_LoadFile(this.openFileDialog_LoadExcel.FileName);
                List<object[]> list_匯入資料 = dataTable.DataTableToRowList();
                List<object[]> list_匯入資料_buf = new List<object[]>();
                List<object[]> list_馬達輸出索引表 = this.sqL_DataGridView_馬達輸出索引表.SQL_GetAllRows(false);
                List<object[]> list_馬達輸出索引表_buf = new List<object[]>();
                List<object[]> list_馬達輸出索引表_add = new List<object[]>();
                List<object[]> list_馬達輸出索引表_replace = new List<object[]>();
                for (int i = 0; i < list_匯入資料.Count; i++)
                {
                    string IP = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.IP].ObjectToString();
                    list_馬達輸出索引表_buf = list_馬達輸出索引表.GetRows((int)enum_storageMedBoxIOConfig.IP, IP);
                    if(list_馬達輸出索引表_buf.Count == 0)
                    {
                        object[] value = new object[new enum_storageMedBoxIOConfig().GetLength()];
                        value[(int)enum_storageMedBoxIOConfig.GUID] = Guid.NewGuid().ToString();
                        value[(int)enum_storageMedBoxIOConfig.IP] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.IP].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料位置X] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.出料位置X].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料位置Y] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.出料位置Y].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料馬達輸入延遲時間] = "100";
                        value[(int)enum_storageMedBoxIOConfig.出料馬達輸入狀態] = "False";
                        value[(int)enum_storageMedBoxIOConfig.出料馬達輸入索引] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.出料馬達輸入索引].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料馬達輸出索引] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.出料馬達輸出索引].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料馬達輸出觸發] = "False";
                        value[(int)enum_storageMedBoxIOConfig.藥盒方位] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.藥盒方位].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.鎖控輸入狀態] = "False";
                        value[(int)enum_storageMedBoxIOConfig.鎖控輸入索引] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.鎖控輸入索引].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.鎖控輸出索引] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.鎖控輸出索引].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.鎖控輸出觸發] = "False";
                        value[(int)enum_storageMedBoxIOConfig.區域] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.區域].ObjectToString();

                        list_馬達輸出索引表_add.Add(value);
                    }
                    else
                    {
                        object[] value = list_馬達輸出索引表_buf[0];
                        value[(int)enum_storageMedBoxIOConfig.IP] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.IP].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料位置X] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.出料位置X].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料位置Y] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.出料位置Y].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料馬達輸入索引] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.出料馬達輸入索引].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料馬達輸出索引] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.出料馬達輸出索引].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.藥盒方位] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.藥盒方位].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.鎖控輸入索引] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.鎖控輸入索引].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.鎖控輸出索引] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.鎖控輸出索引].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.區域] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.區域].ObjectToString();
                        value[(int)enum_storageMedBoxIOConfig.出料馬達輸入延遲時間] = list_匯入資料[i][(int)enum_storageMedBoxIOConfig_匯入.出料馬達輸入延遲時間].ObjectToString();


                        list_馬達輸出索引表_replace.Add(value);
                    }

                }
                this.sqL_DataGridView_馬達輸出索引表.SQL_AddRows(list_馬達輸出索引表_add, false);
                this.sqL_DataGridView_馬達輸出索引表.SQL_ReplaceExtra(list_馬達輸出索引表_replace, false);
                this.sqL_DataGridView_馬達輸出索引表.SQL_GetAllRows(true);
                MyMessageBox.ShowDialog("匯入完成!");

            }));
        }
        private void PlC_RJ_Button_馬達輸出索引表_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = sqL_DataGridView_馬達輸出索引表.Get_All_Select_RowsValues();
            if (list_value.Count == 0)
            {
                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("未選取資料", 1500);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            sqL_DataGridView_馬達輸出索引表.SQL_DeleteExtra(list_value, false);
            sqL_DataGridView_馬達輸出索引表.DeleteExtra(list_value, true);

        }
        private void PlC_RJ_Button_馬達輸出索引表_出料測試_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_馬達輸出索引表.Get_All_Select_RowsValues();

            Dialog_AlarmForm dialog_AlarmForm;
            if (list_面板_儲位列表.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            LoadingForm.ShowLoadingForm();
            if (plC_NumBox_馬達輸出索引表_出料測試次數.Value < 0) plC_NumBox_馬達輸出索引表_出料測試次數.Value = 1;
            int cnt = 0;
            int temp = 0;
            string IP = list_面板_儲位列表[0][(int)enum_面板_儲位列表.IP].ObjectToString();
            Storage storage = List_EPD266_本地資料.SortByIP(IP);
            MyTimer myTimer = new MyTimer();
            while (true)
            {
                if (cnt == 0)
                {
                    if (temp >= plC_NumBox_馬達輸出索引表_出料測試次數.Value) break;
                    cnt++;
                }
                if (cnt == 1)
                {
 
                   
                    int ms = list_面板_儲位列表[0][(int)enum_storageMedBoxIOConfig.出料馬達輸入延遲時間].StringToInt32();
                    if (!storageUI_EPD_266.Set_ADCMotorTrigger(storage, ms))
                    {
                        break;
                    }
                    Console.WriteLine($"第{temp}次,[出料馬達輸出] {storage.IP} ({storage.Code}){storage.Name} {DateTime.Now.ToDateTimeString()}");
                    myTimer.StartTickTime(200);
                     cnt++;
                }
                if (cnt == 2)
                {
                    if (myTimer.IsTimeOut())
                    {
                        cnt++;
                    }

                }
                if (cnt == 3)
                {
                    bool flag_output = storageUI_EPD_266.Get_OutputPIN(storage);
                    if (!flag_output)
                    {
                        temp++;
                        cnt = 0;
                    }

                }


                System.Threading.Thread.Sleep(10);
            }

            LoadingForm.CloseLoadingForm();
        }
        private void PlC_RJ_Button_馬達輸出索引表_出料一次_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_面板_儲位列表 = sqL_DataGridView_馬達輸出索引表.Get_All_Select_RowsValues();
            Dialog_AlarmForm dialog_AlarmForm;
            if (list_面板_儲位列表.Count == 0)
            {
                dialog_AlarmForm = new Dialog_AlarmForm("未選擇儲位", 2000, 0, -200, Color.DarkRed);
                dialog_AlarmForm.ShowDialog();
                return;
            }
            List<object[]> list_replace = new List<object[]>();

            LoadingForm.ShowLoadingForm();

            string IP = list_面板_儲位列表[0][(int)enum_面板_儲位列表.IP].ObjectToString();
   
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
        #endregion

        private class ICP_馬達輸出索引表 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string IP_0 = x[(int)enum_storageMedBoxIOConfig.IP].ObjectToString();
                string IP_1 = y[(int)enum_storageMedBoxIOConfig.IP].ObjectToString();
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
