using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using MyUI;
using Basic;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using H_Pannel_lib;
using HIS_DB_Lib;
using SQLUI;
using AudioProcessingLibrary;
namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
        public class MyColor
        {
            public int R = 0;
            public int G = 0;
            public int B = 0;
            public MyColor(int r, int g, int b)
            {
                this.R = r;
                this.G = g;
                this.B = b;
            }
            public bool IsEqual(int r, int g, int b)
            {
                return (R == r && G == g && B == b);
            }
        }
        private static List<medClass> medClasses_cloud_global = null;
        List<object[]> list_取藥堆疊母資料 = new List<object[]>();
        List<object[]> list_取藥堆疊子資料 = new List<object[]>();
        private MyThread MyThread_取藥堆疊資料_檢查資料;
        private MyThread MyThread_取藥堆疊資料_儲位亮燈;
        static public SQL_DataGridView _sqL_DataGridView_取藥堆疊母資料 = null;
        static public SQL_DataGridView _sqL_DataGridView_取藥堆疊子資料 = null;
        static public PLC_CheckBox _plC_CheckBox_面板於調劑結束更新;
        #region Function
        public class Icp_取藥堆疊母資料_index排序 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string index_0 = x[(int)enum_取藥堆疊母資料.序號].ObjectToString();
                string index_1 = y[(int)enum_取藥堆疊母資料.序號].ObjectToString();
                DateTime date0 = index_0.StringToDateTime();
                DateTime date1 = index_1.StringToDateTime();
                return DateTime.Compare(date0, date1);

            }
        }
        public class Icp_取藥堆疊母資料_操作時間排序 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string index_0 = x[(int)enum_取藥堆疊母資料.操作時間].ToDateTimeString_6();
                string index_1 = y[(int)enum_取藥堆疊母資料.操作時間].ToDateTimeString_6();
                DateTime date0 = index_0.StringToDateTime();
                DateTime date1 = index_1.StringToDateTime();
                return DateTime.Compare(date0, date1);

            }
        }
        public class Icp_取藥堆疊子資料_index排序 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string index_0 = x[(int)enum_取藥堆疊子資料.序號].ObjectToString();
                string index_1 = y[(int)enum_取藥堆疊子資料.序號].ObjectToString();
                UInt64 temp0 = 0;
                UInt64 temp1 = 0;
                UInt64.TryParse(index_0, out temp0);
                UInt64.TryParse(index_1, out temp1);
                if (temp0 > temp1) return 1;
                else if (temp0 < temp1) return -1;
                else return 0;
            }
        }
        public class Icp_取藥堆疊子資料_致能排序 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string 致能_A = x[(int)enum_取藥堆疊子資料.致能].ObjectToString();
                string 致能_B = y[(int)enum_取藥堆疊子資料.致能].ObjectToString();
                if (致能_A == true.ToString()) 致能_A = "1";
                else 致能_A = "0";
                if (致能_B == true.ToString()) 致能_B = "1";
                else 致能_B = "0";
                return 致能_B.CompareTo(致能_A);
            }
        }
        static public takeMedicineStackClass Function_取藥堆疊資料_設定作業模式(takeMedicineStackClass takeMedicineStackClass, enum_取藥堆疊母資料_作業模式 enum_value ,bool state)
        {
            object[] value = Function_取藥堆疊資料_設定作業模式(takeMedicineStackClass.ClassToSQL<takeMedicineStackClass, enum_取藥堆疊母資料>(), enum_value, state);
            return value.SQLToClass<takeMedicineStackClass, enum_取藥堆疊母資料>();
        }
        static public object[] Function_取藥堆疊資料_設定作業模式(object[] value, enum_取藥堆疊母資料_作業模式 enum_value)
        {
            return Function_取藥堆疊資料_設定作業模式(value, enum_value, true);
        }
        static public object[] Function_取藥堆疊資料_設定作業模式(object[] value, enum_取藥堆疊母資料_作業模式 enum_value, bool state)
        {
            UInt32 temp = value[(int)enum_取藥堆疊母資料.作業模式].StringToUInt32();
            temp.SetBit((int)enum_value, state);
            value[(int)enum_取藥堆疊母資料.作業模式] = temp.ToString();
            return value;
        }
        static public bool Function_取藥堆疊資料_取得作業模式(takeMedicineStackClass takeMedicineStackClass, enum_取藥堆疊母資料_作業模式 enum_value)
        {
            object[] value = takeMedicineStackClass.ClassToSQL<takeMedicineStackClass, enum_取藥堆疊母資料>();
            return Function_取藥堆疊資料_取得作業模式(value, enum_value);
        }
        static public bool Function_取藥堆疊資料_取得作業模式(object[] value, enum_取藥堆疊母資料_作業模式 enum_value)
        {
            UInt32 temp = value[(int)enum_取藥堆疊母資料.作業模式].StringToUInt32();
            return temp.GetBit((int)enum_value);
        }
        private void Function_取藥堆疊資料_取得儲位資訊內容(object[] value, ref string Device_GUID, ref string TYPE, ref string IP, ref string Num, ref string 效期, ref string 批號, ref string 庫存, ref string 異動量)
        {
            if (value[(int)enum_儲位資訊.Value] is Device)
            {
                Device device = value[(int)enum_儲位資訊.Value] as Device;
                Num = (-1).ToString();
                value[(int)enum_儲位資訊.IP] = device.IP;
                value[(int)enum_儲位資訊.TYPE] = device.DeviceType.GetEnumName();
                Device_GUID = device.GUID;
                if (device.DeviceType == DeviceType.RFID_Device)
                {
                    Num = device.MasterIndex.ToString();
                }

            }
            IP = value[(int)enum_儲位資訊.IP].ObjectToString();
            TYPE = value[(int)enum_儲位資訊.TYPE].ObjectToString();
            效期 = value[(int)enum_儲位資訊.效期].ObjectToString();
            批號 = value[(int)enum_儲位資訊.批號].ObjectToString();
            庫存 = value[(int)enum_儲位資訊.庫存].ObjectToString();
            異動量 = value[(int)enum_儲位資訊.異動量].ObjectToString();

        }

        static public List<object[]> Function_取藥堆疊資料_取得母資料()
        {
            List<object[]> list_value = _sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
            if (list_value.Count > 0)
            {

            }
            return list_value;
        }
        private List<object[]> Function_取藥堆疊資料_取得子資料()
        {
            return this.sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
        }

        static public void Function_取藥堆疊資料_新增母資料(takeMedicineStackClass takeMedicineStackClass)
        {
            List<takeMedicineStackClass> takeMedicineStackClasses = new List<takeMedicineStackClass>();
            takeMedicineStackClasses.Add(takeMedicineStackClass);
            Function_取藥堆疊資料_新增母資料(takeMedicineStackClasses);

        }
        static public void Function_取藥堆疊資料_新增母資料(List<takeMedicineStackClass> takeMedicineStackClasses)
        {
            try
            {
                List<takeMedicineStackClass> takeMedicineStackClasses_buf = new List<takeMedicineStackClass>();

                List<Task> taskList = new List<Task>();
                MyTimer myTimer = new MyTimer(500000);
                MyTimer myTimer_total = new MyTimer(500000);
                List<string> codes = takeMedicineStackClasses.Select(x => x.藥品碼).ToList();
                List<medClass> medClasses_cloud = medClass.get_med_clouds_by_codes(Main_Form.API_Server, codes);
                List<medClass> medClasses_cloud_buf = new List<medClass>();
                if (medClasses_cloud_global == null) medClasses_cloud_global = medClass.get_med_cloud(Main_Form.API_Server);
                List<object[]> list_藥品設定表 = medConfigClass.get_all(Main_Form.API_Server).ClassToSQL<medConfigClass, enum_medConfig>();
                List<object[]> list_藥品管制方式設定 = _sqL_DataGridView_藥品管制方式設定.SQL_GetAllRows(false);
                List<object[]> list_堆疊母資料 = _sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
                List<object[]> list_堆疊母資料_add = new List<object[]>();
                bool flag_複盤 = false;
                bool flag_盲盤 = false;
                bool flag_效期管理 = false;
                bool flag_雙人覆核 = false;
                bool flag_RFID使用 = false;
                bool flag_自動出藥 = false;


                string 顏色 = "";
                for (int i = 0; i < takeMedicineStackClasses.Count; i++)
                {
                    List<object[]> list_堆疊母資料_buf = new List<object[]>();
                    string 藥品碼 = takeMedicineStackClasses[i].藥品碼;
                                                                                                                                                                                              
                 

                    string 病歷號 = takeMedicineStackClasses[i].病歷號;
                    string 開方時間 = takeMedicineStackClasses[i].開方時間;
                    string 藥袋序號 = takeMedicineStackClasses[i].藥袋序號;
                    顏色 = takeMedicineStackClasses[i].顏色;


                    list_堆疊母資料 = list_堆疊母資料_buf.GetRows((int)enum_取藥堆疊母資料.藥品碼, 藥品碼);
                    list_堆疊母資料_buf = list_堆疊母資料_buf.GetRows((int)enum_取藥堆疊母資料.病歷號, takeMedicineStackClasses[i].病歷號);
                    list_堆疊母資料_buf = list_堆疊母資料_buf.GetRows((int)enum_取藥堆疊母資料.開方時間, takeMedicineStackClasses[i].開方時間);
                    list_堆疊母資料_buf = list_堆疊母資料_buf.GetRows((int)enum_取藥堆疊母資料.藥袋序號, takeMedicineStackClasses[i].藥袋序號);
                    if (list_堆疊母資料_buf.Count != 0) continue;
                    if (takeMedicineStackClasses[i].GUID.StringIsEmpty())
                    {
                        takeMedicineStackClasses[i].GUID = Guid.NewGuid().ToString();

                    }
                    if (takeMedicineStackClasses[i].序號.StringIsEmpty()) takeMedicineStackClasses[i].序號 = DateTime.Now.ToDateTimeString_6();
                    takeMedicineStackClasses[i].操作時間 = DateTime.Now.ToDateTimeString_6();
                    if (takeMedicineStackClasses[i].狀態 != enum_取藥堆疊母資料_狀態.刪除資料.GetEnumName()
                        && takeMedicineStackClasses[i].狀態 != enum_取藥堆疊母資料_狀態.已領用過.GetEnumName()
                        && takeMedicineStackClasses[i].狀態 != enum_取藥堆疊母資料_狀態.DC處方.GetEnumName()
                        && takeMedicineStackClasses[i].狀態 != enum_取藥堆疊母資料_狀態.未授權.GetEnumName()) takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName();

                    //if (takeMedicineStackClasses[i].動作 != enum_交易記錄查詢動作.入庫作業.GetEnumName()) takeMedicineStackClasses[i].IP = "";
                    if (takeMedicineStackClasses[i].動作 != enum_交易記錄查詢動作.掃碼領藥.GetEnumName()
                        && takeMedicineStackClasses[i].動作 != enum_交易記錄查詢動作.撥入作業.GetEnumName()
                        && takeMedicineStackClasses[i].動作 != enum_交易記錄查詢動作.調入作業.GetEnumName()
                          && takeMedicineStackClasses[i].動作 != enum_交易記錄查詢動作.入庫作業.GetEnumName())
                    {
                        if (takeMedicineStackClasses[i].總異動量.StringToDouble() > 0) takeMedicineStackClasses[i].動作 = enum_交易記錄查詢動作.掃碼退藥.GetEnumName();
                    }
                    if (takeMedicineStackClasses[i].效期.Check_Date_String())
                    {
                        if (takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.入庫作業.GetEnumName() || takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.調入作業.GetEnumName()
                            || takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.掃碼退藥.GetEnumName() || takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.手輸退藥.GetEnumName())
                        {
                            takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.新增效期.GetEnumName();
                        }
                    }
                    if (PLC_Device_主機扣賬模式.Bool == false) takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.新增資料.GetEnumName();
                    takeMedicineStackClasses[i].庫存量 = "0";
                    takeMedicineStackClasses[i].結存量 = "0";
                    takeMedicineStackClasses[i].作業模式 = "0";
                    object[] value = takeMedicineStackClasses[i].ClassToSQL<takeMedicineStackClass, enum_取藥堆疊母資料>();
                    value[(int)enum_取藥堆疊母資料.動作] = takeMedicineStackClasses[i].動作.GetEnumName();
                    value[(int)enum_取藥堆疊母資料.狀態] = takeMedicineStackClasses[i].狀態.GetEnumName();
                    //List<object[]> list_藥品資料_buf = list_藥品資料.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, 藥品碼);

                    if (PLC_Device_主機扣賬模式.Bool == true)
                    {
                        medClasses_cloud_buf = medClasses_cloud.Where(x => x.藥品碼 == 藥品碼).ToList();
                        if (medClasses_cloud_buf.Count > 0)
                        {
                            flag_RFID使用 = Function_藥品設定表_取得管制方式(list_藥品設定表, enum_medConfig.使用RFID, 藥品碼);
                            flag_自動出藥 = (List_EPD266_本地資料.Where(x => x.IsFADC  && x.Code == 藥品碼).ToList().Count > 0);
                            if (takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.入庫作業.GetEnumName() || takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.調入作業.GetEnumName()
                             || takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.掃碼退藥.GetEnumName() || takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.手輸退藥.GetEnumName())
                            {
                                flag_自動出藥 = false;
                            }
                            if (Function_藥品設定表_取得是否自訂義(list_藥品設定表, 藥品碼))
                            {
                                flag_複盤 = Function_藥品設定表_取得管制方式(list_藥品設定表, enum_medConfig.複盤, 藥品碼);
                                flag_盲盤 = Function_藥品設定表_取得管制方式(list_藥品設定表, enum_medConfig.盲盤, 藥品碼);
                                flag_效期管理 = Function_藥品設定表_取得管制方式(list_藥品設定表, enum_medConfig.效期管理, 藥品碼);
                                flag_雙人覆核 = Function_藥品設定表_取得管制方式(list_藥品設定表, enum_medConfig.雙人覆核, 藥品碼);

                            }
                            else
                            {
                                string 管制級別 = medClasses_cloud_buf[0].管制級別;
                                string 警訊藥品 = (medClasses_cloud_buf[0].警訊藥品 == true.ToString()) ? "警訊" : "";
                                string 高價藥品 = (medClasses_cloud_buf[0].高價藥品 == true.ToString()) ? "高價" : "";
                                string 生物製劑 = (medClasses_cloud_buf[0].生物製劑 == true.ToString()) ? "生物製劑" : "";
                                flag_複盤 = (Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.複盤, 管制級別) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.複盤, 警訊藥品) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.複盤, 高價藥品) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.複盤, 生物製劑));
                                flag_盲盤 = (Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.盲盤, 管制級別) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.盲盤, 警訊藥品) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.盲盤, 高價藥品) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.盲盤, 生物製劑));
                                flag_效期管理 = (Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.效期管理, 管制級別) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.效期管理, 警訊藥品) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.效期管理, 高價藥品) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.效期管理, 生物製劑));
                                flag_雙人覆核 = (Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.雙人覆核, 管制級別) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.雙人覆核, 警訊藥品) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.雙人覆核, 高價藥品) || Function_藥品管制方式設定_取得管制方式(list_藥品管制方式設定, enum_medGeneralConfig.雙人覆核, 生物製劑));
                            }
                            if (flag_複盤) Function_取藥堆疊資料_設定作業模式(value, enum_取藥堆疊母資料_作業模式.複盤);
                            if (flag_盲盤) Function_取藥堆疊資料_設定作業模式(value, enum_取藥堆疊母資料_作業模式.盲盤);
                            if (flag_效期管理) Function_取藥堆疊資料_設定作業模式(value, enum_取藥堆疊母資料_作業模式.效期管控);
                            if (flag_雙人覆核) Function_取藥堆疊資料_設定作業模式(value, enum_取藥堆疊母資料_作業模式.雙人覆核);
                            if (flag_RFID使用) Function_取藥堆疊資料_設定作業模式(value, enum_取藥堆疊母資料_作業模式.RFID使用);
                            if (flag_自動出藥) Function_取藥堆疊資料_設定作業模式(value, enum_取藥堆疊母資料_作業模式.自動出藥);
                            if (flag_複盤 || flag_盲盤 || flag_雙人覆核 || flag_RFID使用 || flag_自動出藥) Function_取藥堆疊資料_設定作業模式(value, enum_取藥堆疊母資料_作業模式.獨立作業);

                        }
                        if (flag_自動出藥)
                        {
                            value[(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.自動出藥.GetEnumName();
                        }
                        takeMedicineStackClasses[i].作業模式 = value[(int)enum_取藥堆疊母資料.作業模式].ObjectToString();
                    }

                    list_堆疊母資料_add.Add(value);
                    PrintTakeMedicineStackDetail(takeMedicineStackClasses[i], i);
                }


                List<string> list_藥品碼 = (from temp in takeMedicineStackClasses
                                         select temp.藥品碼).Distinct().ToList();
                if (PLC_Device_主機扣賬模式.Bool == true)
                {
                    if (PLC_Device_刷藥袋有相同藥品需警示.Bool)
                    {
                        string msg = "";
                        for (int i = 0; i < list_藥品碼.Count; i++)
                        {

                            takeMedicineStackClasses_buf = (from temp in takeMedicineStackClasses
                                                            where temp.藥品碼 == list_藥品碼[i]
                                                            where temp.動作 == enum_交易記錄查詢動作.掃碼領藥.GetEnumName()
                                                            where temp.狀態 != enum_取藥堆疊母資料_狀態.已領用過.GetEnumName()
                                                            where temp.狀態 != enum_取藥堆疊母資料_狀態.DC處方.GetEnumName()
                                                            where temp.狀態 != enum_取藥堆疊母資料_狀態.未授權.GetEnumName()
                                                            select temp).ToList();
                            if (takeMedicineStackClasses_buf.Count >= 2)
                            {
                                msg += $"{takeMedicineStackClasses_buf[0].藥品名稱}\n";
                            }

                        }
                        if (msg.StringIsEmpty() == false)
                        {
                            msg += "請注意,有相同藥品";

                            Task.Run(new Action(delegate
                            {
                                Basic.Voice.MediaPlayAsync($@"{currentDirectory}\有相同藥品.wav");
                                MyMessageBox.ShowDialog(msg);
                            }));


                        }
                    }
                    List<string> list_lock_IP = new List<string>();
                    for (int i = 0; i < list_藥品碼.Count; i++)
                    {
                        string 藥品碼 = list_藥品碼[i];
                        if (藥品碼.StringIsEmpty()) continue;
                        takeMedicineStackClasses_buf = (from temp in takeMedicineStackClasses
                                                        where temp.藥品碼 == 藥品碼
                                                        where temp.狀態 != "DC處方" && temp.狀態 != "已領用過"
                                                        select temp).ToList();
                        if (takeMedicineStackClasses_buf.Count > 0)
                        {
                            顏色 = takeMedicineStackClasses_buf[0].顏色;
                            if (Function_取藥堆疊資料_取得作業模式(takeMedicineStackClasses_buf[0], enum_取藥堆疊母資料_作業模式.雙人覆核)) continue;
                            if (Function_取藥堆疊資料_取得作業模式(takeMedicineStackClasses_buf[0], enum_取藥堆疊母資料_作業模式.自動出藥)) continue;

                            Main_Form.LightOn lightOn = new Main_Form.LightOn(藥品碼, 顏色.ToColor());
                            lightOn.數量 = Math.Abs(takeMedicineStackClasses_buf[0].總異動量.StringToDouble());
                            if (takeMedicineStackClasses_buf[0].IP.StringIsEmpty() == false) lightOn.IP = takeMedicineStackClasses_buf[0].IP;
                            Function_儲位亮燈(lightOn, ref list_lock_IP);
                        }

                    }
                    Task allTask = Task.WhenAll(taskList);
                    allTask.Wait();
                    for (int i = 0; i < list_堆疊母資料_add.Count; i++)
                    {
                        string 狀態 = list_堆疊母資料_add[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString();
                        list_堆疊母資料_add[i][(int)enum_取藥堆疊母資料.儲位描述] = list_堆疊母資料_add[i][(int)enum_取藥堆疊母資料.儲位描述];
                        if (狀態 == enum_取藥堆疊母資料_狀態.已領用過.GetEnumName() || 狀態 == enum_取藥堆疊母資料_狀態.DC處方.GetEnumName() || 狀態 == enum_取藥堆疊母資料_狀態.未授權.GetEnumName() || 狀態 == enum_取藥堆疊母資料_狀態.自動出藥.GetEnumName()) continue;
                        if (Function_取藥堆疊資料_取得作業模式(list_堆疊母資料_add[i], enum_取藥堆疊母資料_作業模式.雙人覆核)) Function_外門片解鎖(list_lock_IP);
                        else
                        {
                            if (PLC_Device_刷取藥單不要都開啟抽屜.Bool == false)
                            {
                                Function_抽屜解鎖(list_lock_IP);
                            }
                        }
                    }

                    Console.WriteLine($"#1 commonSapceClasses : {commonSapceClasses.Count}");
                    if (list_堆疊母資料_add.Count > 0)
                    {

                        Task.Run(new Action(delegate
                        {
                            commonSapceClasses.WriteTakeMedicineStack(list_堆疊母資料_add);
                        }));

                        _sqL_DataGridView_取藥堆疊母資料.SQL_AddRows(list_堆疊母資料_add, false);
                    }

                    List<object[]> list_value_delete = new List<object[]>();
                    for (int i = 0; i < takeMedicineStackClasses.Count; i++)
                    {
                        string 藥品碼 = takeMedicineStackClasses[i].藥品碼;
                        List<object[]> list_value = _sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
                        List<object[]> list_value_buf = new List<object[]>();

                        list_value_buf = list_value.GetRows((int)enum_取藥堆疊母資料.藥品碼, 藥品碼);
                        list_value_buf = list_value_buf.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, "刷新面板");
                        if (list_value_buf.Count > 0)
                        {
                            list_value_delete.Add(list_value_buf[0]);
                        }
                    }
                    if (list_value_delete.Count > 0)
                    {
                        _sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_value_delete, false);
                    }
                    Console.WriteLine($" 新增取藥資料 (耗時){myTimer_total.ToString()} ");
                }
                else
                {
                    _sqL_DataGridView_取藥堆疊母資料.SQL_AddRows(list_堆疊母資料_add, false);
                }
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog($"Exception : {ex.Message}");
            }
       


        }
        private static void PrintTakeMedicineStackDetail(takeMedicineStackClass tm, int index)
        {
            Console.WriteLine("===============================================");
            Console.WriteLine($"【取藥堆疊資料 - 第 {index} 筆】");

            Console.WriteLine($" GUID                : {tm.GUID}");
            Console.WriteLine($" Order_GUID          : {tm.Order_GUID}");
            Console.WriteLine($" 序號                : {tm.序號}");
            Console.WriteLine($" 調劑台名稱          : {tm.調劑台名稱}");
            Console.WriteLine($" IP                  : {tm.IP}");
            Console.WriteLine($" 操作人              : {tm.操作人}");
            Console.WriteLine($" ID                  : {tm.ID}");
            Console.WriteLine($" 藥師證字號          : {tm.藥師證字號}");
            Console.WriteLine($" 覆核藥師姓名        : {tm.覆核藥師姓名}");
            Console.WriteLine($" 覆核藥師ID          : {tm.覆核藥師ID}");
            Console.WriteLine($" 動作                : {tm.動作}");
            Console.WriteLine($" 作業模式            : {tm.作業模式}");
            Console.WriteLine($" 藥袋序號            : {tm.藥袋序號}");
            Console.WriteLine($" 領藥號              : {tm.領藥號}");
            Console.WriteLine($" 病房號              : {tm.病房號}");
            Console.WriteLine($" 類別                : {tm.類別}");
            Console.WriteLine($" 藥品碼              : {tm.藥品碼}");
            Console.WriteLine($" 藥品名稱            : {tm.藥品名稱}");
            Console.WriteLine($" 單位                : {tm.單位}");
            Console.WriteLine($" 病歷號              : {tm.病歷號}");
            Console.WriteLine($" 病人姓名            : {tm.病人姓名}");
            Console.WriteLine($" 床號                : {tm.床號}");
            Console.WriteLine($" 頻次                : {tm.頻次}");
            Console.WriteLine($" 開方時間            : {tm.開方時間}");
            Console.WriteLine($" 操作時間            : {tm.操作時間}");
            Console.WriteLine($" 顏色                : {tm.顏色}");
            Console.WriteLine($" 狀態                : {tm.狀態}");
            Console.WriteLine($" 庫存量              : {tm.庫存量}");
            Console.WriteLine($" 總異動量            : {tm.總異動量}");
            Console.WriteLine($" 結存量              : {tm.結存量}");
            Console.WriteLine($" 盤點量              : {tm.盤點量}");
            Console.WriteLine($" 效期                : {tm.效期}");
            Console.WriteLine($" 批號                : {tm.批號}");
            Console.WriteLine($" 備註                : {tm.備註}");
            Console.WriteLine($" 收支原因            : {tm.收支原因}");
            Console.WriteLine($" 診別                : {tm.診別}");
            Console.WriteLine($" 儲位描述            : {tm.儲位描述}");

            Console.WriteLine("===============================================");
        }

        private object[] Function_取藥堆疊資料_新增子資料(string Master_GUID, string Device_GUID, string 調劑台名稱, string 藥品碼, string IP, string Num, string _enum_取藥堆疊_TYPE, string 效期, string 批號, string 異動量)
        {

            object[] value = new object[new enum_取藥堆疊子資料().GetLength()];
            value[(int)enum_取藥堆疊子資料.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_取藥堆疊子資料.Master_GUID] = Master_GUID;
            value[(int)enum_取藥堆疊子資料.Device_GUID] = Device_GUID;
            value[(int)enum_取藥堆疊子資料.序號] = SQLControl.GetTimeNow_6();
            value[(int)enum_取藥堆疊子資料.調劑台名稱] = 調劑台名稱;
            value[(int)enum_取藥堆疊子資料.藥品碼] = 藥品碼;
            value[(int)enum_取藥堆疊子資料.IP] = IP;
            value[(int)enum_取藥堆疊子資料.Num] = Num;
            value[(int)enum_取藥堆疊子資料.TYPE] = _enum_取藥堆疊_TYPE;
            value[(int)enum_取藥堆疊子資料.效期] = 效期;
            value[(int)enum_取藥堆疊子資料.批號] = 批號;
            value[(int)enum_取藥堆疊子資料.異動量] = 異動量.ToString();
            value[(int)enum_取藥堆疊子資料.致能] = false.ToString();
            value[(int)enum_取藥堆疊子資料.流程作業完成] = false.ToString();
            value[(int)enum_取藥堆疊子資料.配藥完成] = false.ToString();
            value[(int)enum_取藥堆疊子資料.調劑結束] = false.ToString();
            value[(int)enum_取藥堆疊子資料.已入帳] = false.ToString();
            PrintTakeMedicineStackSubData(value); this.sqL_DataGridView_取藥堆疊子資料.SQL_AddRow(value, false);
            return value;
        }
        private void PrintTakeMedicineStackSubData(object[] value)
        {
            Logger.Log("--------------------------------------------------");
            Logger.Log($"【新增取藥堆疊子資料】 {DateTime.Now.ToDateTimeString()}");

            foreach (enum_取藥堆疊子資料 e in Enum.GetValues(typeof(enum_取藥堆疊子資料)))
            {
                Logger.Log($" {e,-12} : {value[(int)e].ObjectToString()}");
            }

            Logger.Log("--------------------------------------------------");
        }
        static public void Function_取藥堆疊資料_刪除母資料(string GUID)
        {
            List<object[]> list_value = Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
            List<object[]> list_value_buf = list_value.GetRows((int)enum_取藥堆疊母資料.GUID, GUID);
            if (list_value_buf.Count == 0) return;
            Main_Form._sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_value_buf, false);

            string 藥品碼 = list_value_buf[0][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
            //string 操作人 = list_value_buf[0][(int)enum_取藥堆疊母資料.操作人].ObjectToString();
            //list_value_buf = list_value.GetRows((int)enum_取藥堆疊母資料.藥品碼, 藥品碼);
            //list_value_buf = list_value_buf.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, "刷新面板");
            //if (list_value_buf.Count == 0)
            //{
            //    takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
            //    takeMedicineStackClass.GUID = Guid.NewGuid().ToString();
            //    takeMedicineStackClass.藥品碼 = 藥品碼;
            //    takeMedicineStackClass.操作時間 = DateTime.Now.ToDateTimeString_6();
            //    takeMedicineStackClass.開方時間 = DateTime.Now.ToDateTimeString_6();
            //    takeMedicineStackClass.調劑台名稱 = "刷新面板";
            //    takeMedicineStackClass.操作人 = $"{操作人}";
            //    this.sqL_DataGridView_取藥堆疊母資料.SQL_AddRow(takeMedicineStackClass.ClassToSQL<takeMedicineStackClass, enum_取藥堆疊母資料>(), false);
            //}
            //else
            //{
            //    takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
            //    takeMedicineStackClass.GUID = list_value_buf[0][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
            //    takeMedicineStackClass.操作時間 = DateTime.Now.ToDateTimeString_6();
            //    takeMedicineStackClass.開方時間 = DateTime.Now.ToDateTimeString_6();
            //    takeMedicineStackClass.調劑台名稱 = "刷新面板";
            //    takeMedicineStackClass.操作人 = $"{操作人}";
            //    this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(takeMedicineStackClass.ClassToSQL<takeMedicineStackClass, enum_取藥堆疊母資料>(), false);
            //}

            Function_儲位亮燈(new LightOn(藥品碼, Color.Black));
        }
        static public void Function_取藥堆疊資料_刪除子資料(string GUID)
        {
            Function_取藥堆疊資料_刪除子資料(GUID, true);
        }
        static public void Function_取藥堆疊資料_刪除子資料(string GUID, bool color_off)
        {
            List<object[]> list_value = _sqL_DataGridView_取藥堆疊子資料.SQL_GetRows(enum_取藥堆疊子資料.GUID.GetEnumName(), GUID, false);
            if (list_value.Count > 0)
            {
                string 藥品碼 = list_value[0][(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                _sqL_DataGridView_取藥堆疊子資料.SQL_Delete(enum_取藥堆疊子資料.GUID.GetEnumName(), GUID, false);
                if (藥品碼.StringIsEmpty()) return;

                Console.WriteLine($"{DateTime.Now.ToDateTimeString()}-刪除子資料 藥品碼: {藥品碼}");

                if (color_off) Function_儲位亮燈(new Main_Form.LightOn(藥品碼, Color.Black));

            }
        }
        private void Function_取藥堆疊資料_語音提示(object[] 取藥堆疊子資料)
        {
            string device_Type = 取藥堆疊子資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString();
            string IP = 取藥堆疊子資料[(int)enum_取藥堆疊子資料.IP].ObjectToString();
            string 藥品碼 = 取藥堆疊子資料[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
            string device_GUID = 取藥堆疊子資料[(int)enum_取藥堆疊子資料.Device_GUID].ObjectToString();

            if (device_Type == DeviceType.EPD266.GetEnumName() || device_Type == DeviceType.EPD266_lock.GetEnumName()
                || device_Type == DeviceType.EPD290.GetEnumName() || device_Type == DeviceType.EPD290_lock.GetEnumName()
                || device_Type == DeviceType.EPD420.GetEnumName() || device_Type == DeviceType.EPD420_lock.GetEnumName()
                || device_Type == DeviceType.EPD213.GetEnumName() || device_Type == DeviceType.EPD213_lock.GetEnumName()
                || device_Type == DeviceType.EPD420G.GetEnumName() || device_Type == DeviceType.EPD420G_lock.GetEnumName()
                || device_Type == DeviceType.EPD360E.GetEnumName() || device_Type == DeviceType.EPD360E_lock.GetEnumName())
            {
                Storage storage = List_EPD266_雲端資料.SortByIP(IP);
                if (storage != null && storage.Speaker.StringIsEmpty() == false)
                {
                    Task.Run(() =>
                    {
                        voice.SpeakOnTask(storage.Speaker);
                    });
                }
            }
            else if (device_Type == DeviceType.Pannel35.GetEnumName() || device_Type == DeviceType.Pannel35_lock.GetEnumName())
            {
                Storage storage = List_Pannel35_雲端資料.SortByIP(IP);
                if (storage != null && storage.Speaker.StringIsEmpty() == false)
                {
                    Task.Run(() =>
                    {
                        voice.SpeakOnTask(storage.Speaker);
                    });

                }
            }
            else if (device_Type == DeviceType.EPD583.GetEnumName() || device_Type == DeviceType.EPD583_lock.GetEnumName() 
                     || device_Type == DeviceType.EPD420_D.GetEnumName() || device_Type == DeviceType.EPD420_D_lock.GetEnumName()
                     || device_Type == DeviceType.EPD730E.GetEnumName() || device_Type == DeviceType.EPD730E_lock.GetEnumName())
            {
                Drawer drawer = List_EPD583_雲端資料.SortByIP(IP);
                if (drawer != null && drawer.Speaker.StringIsEmpty() == false)
                {
                    Task.Run(() =>
                    {
                        voice.SpeakOnTask(drawer.Speaker);
                    });
                }
            }
            else if (device_Type == DeviceType.EPD1020.GetEnumName() || device_Type == DeviceType.EPD1020_lock.GetEnumName())
            {
                Drawer drawer = List_EPD1020_雲端資料.SortByIP(IP);
                if (drawer != null && drawer.Speaker.StringIsEmpty() == false)
                {
                    Task.Run(() =>
                    {
                        voice.SpeakOnTask(drawer.Speaker);
                    });
                }
            }
            else if (device_Type == DeviceType.RowsLED.GetEnumName())
            {
                RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(IP);
                RowsDevice rowsDevice = rowsLED.SortByGUID(device_GUID);
                if (rowsLED != null && rowsLED.Speaker.StringIsEmpty() == false)
                {
                    Task.Run(() =>
                    {
                        voice.SpeakOnTask(rowsLED.Speaker);
                    });
                }
            }
        }
        static public void Function_取藥堆疊資料_刷新面板(string 藥品碼)
        {
            Function_取藥堆疊資料_刷新面板(藥品碼, -1);
        }
        static public void Function_取藥堆疊資料_刷新面板(string 藥品碼, int 庫存)
        {
            List<object[]> list_value = _sqL_DataGridView_取藥堆疊母資料.SQL_GetRows((int)enum_取藥堆疊母資料.藥品碼, 藥品碼, false);
            list_value = list_value.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, "刷新面板");
            List<object[]> list_value_add = new List<object[]>();
            if (list_value.Count == 0)
            {
                takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
                takeMedicineStackClass.GUID = Guid.NewGuid().ToString();
                takeMedicineStackClass.藥品碼 = 藥品碼;
                takeMedicineStackClass.動作 = enum_交易記錄查詢動作.None.GetEnumName();
                takeMedicineStackClass.狀態 = enum_取藥堆疊母資料_狀態.None.GetEnumName();
                takeMedicineStackClass.操作時間 = DateTime.Now.ToDateTimeString_6();
                takeMedicineStackClass.開方時間 = DateTime.Now.ToDateTimeString_6();
                takeMedicineStackClass.調劑台名稱 = "刷新面板";
                if (庫存 != -1) takeMedicineStackClass.庫存量 = 庫存.ToString();
                object[] value = takeMedicineStackClass.ClassToSQL<takeMedicineStackClass, enum_取藥堆疊母資料>();
                value[(int)enum_取藥堆疊母資料.動作] = enum_交易記錄查詢動作.None.GetEnumName();
                value[(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.None.GetEnumName();
                list_value_add.Add(value);
                Console.WriteLine($"{takeMedicineStackClass.JsonSerializationt(true)}");
            }
            if (list_value_add.Count > 0) _sqL_DataGridView_取藥堆疊母資料.SQL_AddRows(list_value_add, false);
        }
        static public void Function_取藥堆疊資料_刪除指定調劑台名稱母資料(string 調劑台名稱)
        {
            while (true)
            {
                List<object[]> list_value = _sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
                List<object[]> list_value_buf = list_value.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, 調劑台名稱);
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                if (list_value_buf.Count == 0) break;
                _sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_value_buf, false);
                for (int i = 0; i < list_value_buf.Count; i++)
                {

                    string 藥品碼 = list_value_buf[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    string Master_GUID = list_value_buf[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                    if (藥品碼.StringIsEmpty()) continue;
                    Function_儲位亮燈(new Main_Form.LightOn(藥品碼, Color.Black));
                    if (_plC_CheckBox_面板於調劑結束更新.Checked) Function_取藥堆疊資料_刷新面板(藥品碼);

                }
                if (list_value_add.Count > 0) _sqL_DataGridView_取藥堆疊母資料.SQL_AddRows(list_value_add, false);
                if (list_value_replace.Count > 0) _sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_value_replace, false);

            }
            System.Threading.Thread.Sleep(100);
        }
        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱母資料(string 調劑台名稱)
        {
            List<object[]> list_values = _sqL_DataGridView_取藥堆疊母資料.SQL_GetRows((int)enum_取藥堆疊母資料.調劑台名稱, 調劑台名稱, false);
            return list_values;
        }
        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱母資料(string 調劑台名稱, string 藥品碼)
        {
            List<object[]> list_values = Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString() == 藥品碼).ToList();
            return list_values;
        }

        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱子資料(string 調劑台名稱)
        {
            List<object[]> list_values = Main_Form._sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString() == 調劑台名稱).ToList();
            return list_values;
        }
        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱子資料(string 調劑台名稱, string 藥品碼)
        {
            List<object[]> list_values = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱);
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString() == 藥品碼).ToList();
            return list_values;
        }
        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱子資料(string 調劑台名稱, string 藥品碼, string IP)
        {
            List<object[]> list_values = Main_Form.Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱);
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString() == 藥品碼).ToList();
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊子資料.IP].ObjectToString() == IP).ToList();
            return list_values;
        }

        private Color Function_取藥堆疊母資料_取得指定Master_GUID顏色(string GUID)
        {
            string[] serch_cols = new string[] { enum_取藥堆疊母資料.GUID.GetEnumName() };
            string[] serch_values = new string[] { GUID };
            List<object[]> list_values = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetRows(serch_cols, serch_values, false);
            if (list_values.Count > 0)
            {
                return list_values[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
            }
            return Color.Black;
        }
        private string Function_取藥堆疊母資料_取得指定Master_GUID結存量(string GUID)
        {
            string[] serch_cols = new string[] { enum_取藥堆疊母資料.GUID.GetEnumName() };
            string[] serch_values = new string[] { GUID };
            List<object[]> list_values = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetRows(serch_cols, serch_values, false);
            if (list_values.Count > 0)
            {
                return list_values[0][(int)enum_取藥堆疊母資料.結存量].ObjectToString();
            }
            return "";
        }
        private string Function_取藥堆疊母資料_取得指定Master_GUID調劑台名稱(string GUID)
        {
            string[] serch_cols = new string[] { enum_取藥堆疊母資料.GUID.GetEnumName() };
            string[] serch_values = new string[] { GUID };
            List<object[]> list_values = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetRows(serch_cols, serch_values, false);
            if (list_values.Count > 0)
            {
                return list_values[0][(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString();
            }
            return "";
        }
        private void Function_取藥堆疊子資料_設定流程作業完成ByCode(string 調劑台名稱, string 藥品碼)
        {
            string Master_GUID = "";
            List<object[]> list_堆疊母資料 = Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱, 藥品碼);
            List<object[]> list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱, 藥品碼);
            List<object[]> list_堆疊子資料_buf;
            List<object[]> list_serch_values = new List<object[]>();
            for (int i = 0; i < list_堆疊母資料.Count; i++)
            {
                Master_GUID = list_堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                list_堆疊子資料_buf = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);

                for (int k = 0; k < list_堆疊子資料_buf.Count; k++)
                {
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_serch_values.Add(list_堆疊子資料_buf[k]);
                }
            }
            this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_serch_values, false);

        }
        private void Function_取藥堆疊子資料_設定流程作業完成ByCode(string 調劑台名稱, string 藥品碼, string IP)
        {
            List<object[]> list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱, 藥品碼, IP);
            List<object[]> list_serch_values = new List<object[]>();
            for (int k = 0; k < list_堆疊子資料.Count; k++)
            {
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                list_serch_values.Add(list_堆疊子資料[k]);
            }
            this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_serch_values, false);
        }
        private List<object[]> Function_取藥堆疊子資料_設定流程作業完成ByIP(string 調劑台名稱, string IP)
        {
            return Function_取藥堆疊子資料_設定流程作業完成ByIP(調劑台名稱, IP, "-1");
        }
        private List<object[]> Function_取藥堆疊子資料_設定流程作業完成ByIP(string 調劑台名稱, string IP, string Num)
        {
            List<object[]> list_堆疊子資料 = new List<object[]>();
            List<object[]> serch_values = new List<object[]>();
            if (調劑台名稱 != "None")
            {
                list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.IP, IP);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Num, Num);

                for (int i = 0; i < list_堆疊子資料.Count; i++)
                {
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    serch_values.Add(list_堆疊子資料[i]);

                }
                this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(serch_values, false);
            }
            else
            {
                if (Num.StringIsEmpty()) Num = "-1";
                list_堆疊子資料 = this.Function_取藥堆疊資料_取得子資料();
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.IP, IP);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Num, Num);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.致能, true.ToString());
                for (int i = 0; i < list_堆疊子資料.Count; i++)
                {
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    serch_values.Add(list_堆疊子資料[i]);
                }
                this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(serch_values, false);
            }
            return list_堆疊子資料;
        }
        private void Function_取藥堆疊子資料_設定配藥完成ByCode(string 調劑台名稱, string 藥品碼)
        {
            string Master_GUID = "";
            List<object[]> list_堆疊母資料 = Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱, 藥品碼);
            List<object[]> list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱, 藥品碼);
            List<object[]> list_堆疊子資料_buf;
            List<object[]> list_serch_values = new List<object[]>();
            for (int i = 0; i < list_堆疊母資料.Count; i++)
            {
                Master_GUID = list_堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                list_堆疊子資料_buf = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);
                for (int k = 0; k < list_堆疊子資料_buf.Count; k++)
                {
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    list_serch_values.Add(list_堆疊子資料_buf[k]);
                }
            }
            this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_serch_values, false);
        }
        private void Function_取藥堆疊子資料_設定配藥完成ByCode(string 調劑台名稱, string 藥品碼, string IP)
        {
            List<object[]> list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱, 藥品碼, IP);
            List<object[]> list_serch_values = new List<object[]>();
            for (int k = 0; k < list_堆疊子資料.Count; k++)
            {
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                list_serch_values.Add(list_堆疊子資料[k]);
            }
            this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_serch_values, false);
        }
        private void Function_取藥堆疊子資料_設定配藥完成ByIP(string 調劑台名稱, string IP)
        {
            Function_取藥堆疊子資料_設定配藥完成ByIP(調劑台名稱, IP, "-1");
        }
        private bool Function_取藥堆疊子資料_設定配藥完成ByIP(string 調劑台名稱, string IP, string Num)
        {
            List<object[]> list_堆疊子資料 = new List<object[]>();
            List<object[]> serch_values = new List<object[]>();
            if (調劑台名稱 != "None")
            {
                list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱);
                Console.WriteLine($"「設定配藥完成」IP : {IP} ,PC_NAME : {調劑台名稱} , connt : {list_取藥堆疊子資料.Count}");
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.IP, IP);
                //list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Num, Num);
                for (int i = 0; i < list_堆疊子資料.Count; i++)
                {
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    serch_values.Add(list_堆疊子資料[i]);

                }
                this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(serch_values, false);
                return (list_堆疊子資料.Count > 0);
            }
            else
            {
                if (Num.StringIsEmpty()) Num = "-1";
                list_堆疊子資料 = this.Function_取藥堆疊資料_取得子資料();
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.IP, IP);
                //list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Num, Num);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.致能, true.ToString());
                if (plC_CheckBox_需等待手勢感測_關閉抽屜才可入帳.Checked) list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.流程作業完成, true.ToString());
                Console.WriteLine($"「設定配藥完成」IP : {IP} ,PC_NAME : {調劑台名稱} , connt : {list_取藥堆疊子資料.Count}");
                for (int i = 0; i < list_堆疊子資料.Count; i++)
                {
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    serch_values.Add(list_堆疊子資料[i]);
                }
                this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(serch_values, false);
                return (list_堆疊子資料.Count > 0);
            }
        }
        private void Function_取藥堆疊子資料_設定調劑結束(string 調劑台名稱, string 藥品碼)
        {
            string GUID = "";
            List<object[]> list_values = Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱, 藥品碼);
            for (int i = 0; i < list_values.Count; i++)
            {
                GUID = list_values[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                this.Function_取藥堆疊子資料_設定調劑結束(GUID);
            }
        }
        private void Function_取藥堆疊子資料_設定調劑結束(string 調劑台名稱)
        {
            string GUID = "";
            List<object[]> list_values = Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            for (int i = 0; i < list_values.Count; i++)
            {
                GUID = list_values[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                this.Function_取藥堆疊子資料_設定調劑結束(GUID);
            }

        }
        private object[] Function_取藥堆疊子資料_設定已入帳(object[] 堆疊子資料)
        {
            string IP = 堆疊子資料[(int)enum_取藥堆疊子資料.IP].ObjectToString();
            string 藥品碼 = 堆疊子資料[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
            string str_TYPE = 堆疊子資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString();
            string 效期 = 堆疊子資料[(int)enum_取藥堆疊子資料.效期].ObjectToString();
            double 異動量 = 堆疊子資料[(int)enum_取藥堆疊子資料.異動量].StringToDouble();
            double 儲位庫存 = 0;
            string 批號 = 堆疊子資料[(int)enum_取藥堆疊子資料.批號].ObjectToString();

            Logger.Log("==================================================");
            Logger.Log($"【設定已入帳 - 子資料】 {DateTime.Now.ToDateTimeString()}");
            Logger.Log($" IP       : {IP}");
            Logger.Log($" 藥品碼   : {藥品碼}");
            Logger.Log($" TYPE     : {str_TYPE}");
            Logger.Log($" 效期     : {效期}");
            Logger.Log($" 異動量   : {異動量}");
            Logger.Log("--------------------------------------------------");

            if (plC_CheckBox_面板於過帳後更新.Checked)
            {
                Logger.Log("→ 刷新面板");
                Function_取藥堆疊資料_刷新面板(藥品碼);
            }

            // *****************************************************
            //  EPD583, 420D, 730E 系列
            // *****************************************************
            if (str_TYPE == DeviceType.EPD583.GetEnumName() || str_TYPE == DeviceType.EPD583_lock.GetEnumName()
             || str_TYPE == DeviceType.EPD420_D.GetEnumName() || str_TYPE == DeviceType.EPD420_D_lock.GetEnumName()
             || str_TYPE == DeviceType.EPD730E.GetEnumName() || str_TYPE == DeviceType.EPD730E_lock.GetEnumName())
            {
                Logger.Log("→ 設備類型：EPD583 / 420D / 730E 系列");

                List<Box> boxes = List_EPD583_入賬資料.SortByCode(藥品碼);
                for (int i = 0; i < boxes.Count; i++)
                {
                    if (boxes[i].IP != IP) continue;

                    Logger.Log($"→ 找到儲位 BOX (IP: {IP})");
                    boxes[i] = this.drawerUI_EPD_583.SQL_GetBox(boxes[i]);
                    儲位庫存 = boxes[i].取得庫存(效期);

                    Logger.Log($"   儲位庫存 : {儲位庫存}");
                    Logger.Log($"   異動量   : {異動量}");

                    if (儲位庫存 + 異動量 < 0)
                    {
                        Logger.Log("   → 庫存不足，執行庫存異動");

                        List<string> list_效期 = new List<string>();
                        List<string> list_批號 = new List<string>();
                        List<string> list_異動量 = new List<string>();
                        boxes[i].庫存異動(異動量, out list_效期, out list_批號);

                        Drawer drawer = List_EPD583_入賬資料.ReplaceBox(boxes[i]);
                        List_EPD583_入賬資料.Add_NewDrawer(boxes[i]);
                        this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                        break;
                    }
                    else if (儲位庫存 >= 0)
                    {
                        Logger.Log("   → 正常異動，更新效期庫存");
                        boxes[i].效期庫存異動(效期, 異動量);
                        批號 = boxes[i].取得批號(效期);

                        Drawer drawer = List_EPD583_入賬資料.ReplaceBox(boxes[i]);
                        List_EPD583_入賬資料.Add_NewDrawer(boxes[i]);
                        this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                        break;
                    }
                    else if (儲位庫存 == -1)
                    {
                        Logger.Log("   → 無此效期，新增效期");
                        boxes[i].新增效期(效期, 批號, 異動量.ToString());

                        Drawer drawer = List_EPD583_入賬資料.ReplaceBox(boxes[i]);
                        List_EPD583_入賬資料.Add_NewDrawer(boxes[i]);
                        this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                        break;
                    }
                }
            }

            // *****************************************************
            //  EPD1020 系列
            // *****************************************************
            else if (str_TYPE == DeviceType.EPD1020.GetEnumName() || str_TYPE == DeviceType.EPD1020_lock.GetEnumName())
            {
                Logger.Log("→ 設備類型：EPD1020");

                List<Box> boxes = List_EPD1020_入賬資料.SortByCode(藥品碼);
                for (int i = 0; i < boxes.Count; i++)
                {
                    if (boxes[i].IP != IP) continue;

                    Logger.Log($"→ 找到儲位 (IP: {IP})");
                    boxes[i] = this.drawerUI_EPD_1020.SQL_GetBox(boxes[i]);
                    儲位庫存 = boxes[i].取得庫存(效期);

                    Logger.Log($"   儲位庫存 : {儲位庫存}");
                    Logger.Log($"   異動量   : {異動量}");

                    if (儲位庫存 + 異動量 < 0)
                    {
                        Logger.Log("   → 庫存不足，執行庫存異動");

                        List<string> list_效期 = new List<string>();
                        List<string> list_批號 = new List<string>();
                        List<string> list_異動量 = new List<string>();
                        boxes[i].庫存異動(異動量, out list_效期, out list_批號);

                        Drawer drawer = List_EPD1020_入賬資料.ReplaceBox(boxes[i]);
                        List_EPD1020_入賬資料.Add_NewDrawer(boxes[i]);
                        this.drawerUI_EPD_1020.SQL_ReplaceDrawer(drawer);
                        break;
                    }
                    else if (儲位庫存 >= 0)
                    {
                        Logger.Log("   → 正常異動，更新效期庫存");

                        boxes[i].效期庫存異動(效期, 異動量);
                        Drawer drawer = List_EPD1020_入賬資料.ReplaceByGUID(boxes[i]);
                        List_EPD1020_入賬資料.Add_NewDrawer(boxes[i]);
                        this.drawerUI_EPD_1020.SQL_ReplaceDrawer(drawer);
                        break;
                    }
                    else if (儲位庫存 == -1)
                    {
                        Logger.Log("   → 無此效期，新增加效期");

                        boxes[i].新增效期(效期, 批號, 異動量.ToString());
                        Drawer drawer = List_EPD1020_入賬資料.ReplaceBox(boxes[i]);
                        List_EPD1020_入賬資料.Add_NewDrawer(boxes[i]);
                        this.drawerUI_EPD_1020.SQL_ReplaceDrawer(drawer);
                        break;
                    }
                }
            }

            // *****************************************************
            //  EPD266 / 290 / 420 / 213 / 360E 系列
            // *****************************************************
            else if (str_TYPE == DeviceType.EPD266.GetEnumName() || str_TYPE == DeviceType.EPD266_lock.GetEnumName()
                 || str_TYPE == DeviceType.EPD290.GetEnumName() || str_TYPE == DeviceType.EPD290_lock.GetEnumName()
                 || str_TYPE == DeviceType.EPD290G.GetEnumName() || str_TYPE == DeviceType.EPD290G_lock.GetEnumName()
                 || str_TYPE == DeviceType.EPD420.GetEnumName() || str_TYPE == DeviceType.EPD420_lock.GetEnumName()
                 || str_TYPE == DeviceType.EPD420G.GetEnumName() || str_TYPE == DeviceType.EPD420G_lock.GetEnumName()
                 || str_TYPE == DeviceType.EPD213.GetEnumName() || str_TYPE == DeviceType.EPD213_lock.GetEnumName()  
                 || str_TYPE == DeviceType.EPD360E.GetEnumName() || str_TYPE == DeviceType.EPD360E_lock.GetEnumName())
            {
                Logger.Log("→ 設備類型：EPD266 / 290 / 420 / 213 / 360E 系列");

                Storage storage = List_EPD266_入賬資料.SortByIP(IP);
                if (storage != null)
                {
                    Logger.Log("→ 找到儲位 Storage");

                    storage = this.storageUI_EPD_266.SQL_GetStorage(storage);
                    儲位庫存 = storage.取得庫存(效期);

                    Logger.Log($"   儲位庫存 : {儲位庫存}");
                    Logger.Log($"   異動量   : {異動量}");

                    if (儲位庫存 + 異動量 < 0)
                    {
                        Logger.Log("   → 庫存不足，執行庫存異動");

                        List<string> list_效期 = new List<string>();
                        List<string> list_批號 = new List<string>();
                        List<string> list_異動量 = new List<string>();
                        storage.庫存異動(異動量, out list_效期, out list_批號);

                        List_EPD266_入賬資料.Add_NewStorage(storage);
                        this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                    }
                    else if (儲位庫存 >= 0)
                    {
                        Logger.Log("   → 正常異動，更新效期庫存");

                        storage.效期庫存異動(效期, 異動量);

                        List_EPD266_入賬資料.Add_NewStorage(storage);
                        this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                    }
                    else if (儲位庫存 == -1)
                    {
                        Logger.Log("   → 無此效期，新增加效期");

                        storage.新增效期(效期, 批號, 異動量.ToString());
                        List_EPD266_入賬資料.Add_NewStorage(storage);
                        this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                    }
                }
                else
                {
                    Logger.Log("→ 未找到 Storage，改由 EPD583 BOX 模式處理");

                    // fallback to BOX
                    List<Box> boxes = List_EPD583_入賬資料.SortByCode(藥品碼);
                    for (int i = 0; i < boxes.Count; i++)
                    {
                        if (boxes[i].IP != IP) continue;

                        boxes[i] = this.drawerUI_EPD_583.SQL_GetBox(boxes[i]);
                        儲位庫存 = boxes[i].取得庫存(效期);

                        Logger.Log($"   儲位庫存 : {儲位庫存}");
                        Logger.Log($"   異動量   : {異動量}");

                        if (儲位庫存 + 異動量 < 0)
                        {
                            Logger.Log("   → 庫存不足，執行庫存異動");

                            List<string> list_效期 = new List<string>();
                            List<string> list_批號 = new List<string>();
                            List<string> list_異動量 = new List<string>();
                            boxes[i].庫存異動(異動量, out list_效期, out list_批號);

                            Drawer drawer = List_EPD583_入賬資料.ReplaceBox(boxes[i]);
                            List_EPD583_入賬資料.Add_NewDrawer(boxes[i]);
                            this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                            break;
                        }
                        else if (儲位庫存 >= 0)
                        {
                            Logger.Log("   → 正常異動，更新效期庫存");

                            boxes[i].效期庫存異動(效期, 異動量);
                            批號 = boxes[i].取得批號(效期);

                            Drawer drawer = List_EPD583_入賬資料.ReplaceBox(boxes[i]);
                            List_EPD583_入賬資料.Add_NewDrawer(boxes[i]);
                            this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                            break;
                        }
                        else if (儲位庫存 == -1)
                        {
                            Logger.Log("   → 無此效期，新增加效期");

                            boxes[i].新增效期(效期, 批號, 異動量.ToString());
                            Drawer drawer = List_EPD583_入賬資料.ReplaceBox(boxes[i]);
                            List_EPD583_入賬資料.Add_NewDrawer(boxes[i]);
                            this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                            break;
                        }
                    }
                }
            }

            // *****************************************************
            //  Pannel35
            // *****************************************************
            else if (str_TYPE == DeviceType.Pannel35_lock.GetEnumName() || str_TYPE == DeviceType.Pannel35.GetEnumName())
            {
                Logger.Log("→ 設備類型：WT32 Pannel35");

                Storage storage = List_Pannel35_入賬資料.SortByIP(IP);
                storage = this.storageUI_WT32.SQL_GetStorage(storage);
                儲位庫存 = storage.取得庫存(效期);

                Logger.Log($"   儲位庫存 : {儲位庫存}");
                Logger.Log($"   異動量   : {異動量}");

                if (儲位庫存 + 異動量 < 0)
                {
                    Logger.Log("   → 庫存不足，執行庫存異動");

                    List<string> list_效期 = new List<string>();
                    List<string> list_批號 = new List<string>();
                    List<string> list_異動量 = new List<string>();
                    storage.庫存異動(異動量, out list_效期, out list_批號);

                    List_Pannel35_入賬資料.Add_NewStorage(storage);
                    this.storageUI_WT32.SQL_ReplaceStorage(storage);
                }
                else if (儲位庫存 >= 0)
                {
                    Logger.Log("   → 正常異動，更新效期庫存");

                    storage.效期庫存異動(效期, 異動量);
                    List_Pannel35_入賬資料.Add_NewStorage(storage);
                    this.storageUI_WT32.SQL_ReplaceStorage(storage);

                    if (!plC_CheckBox_測試模式.Checked)
                    {
                        Logger.Log("   → 重新繪製 WT32 畫面 JPEG (背景模式)");
                        Task.Run(() =>
                        {
                            this.storageUI_WT32.Set_DrawPannelJEPG(storage);
                        });
                    }
                }
                else if (儲位庫存 == -1)
                {
                    Logger.Log("   → 無此效期，新增加效期");

                    storage.新增效期(效期, 批號, 異動量.ToString());
                    List_Pannel35_入賬資料.Add_NewStorage(storage);
                    this.storageUI_WT32.SQL_ReplaceStorage(storage);
                }
            }

            // *****************************************************
            //  RowsLED
            // *****************************************************
            else if (str_TYPE == DeviceType.RowsLED.GetEnumName())
            {
                Logger.Log("→ 設備類型：Rows LED");

                List<RowsDevice> rowsDevices = List_RowsLED_入賬資料.SortByCode(藥品碼);
                for (int i = 0; i < rowsDevices.Count; i++)
                {
                    if (rowsDevices[i].IP != IP) continue;

                    rowsDevices[i] = this.rowsLEDUI.SQL_GetRowsDevice(rowsDevices[i]);
                    儲位庫存 = rowsDevices[i].取得庫存(效期);

                    Logger.Log($"   儲位庫存 : {儲位庫存}");
                    Logger.Log($"   異動量   : {異動量}");

                    if (儲位庫存 + 異動量 < 0)
                    {
                        Logger.Log("   → 庫存不足，執行庫存異動");
                        List<string> list_效期 = new List<string>();
                        List<string> list_批號 = new List<string>();
                        List<string> list_異動量 = new List<string>();
                        rowsDevices[i].庫存異動(異動量, out list_效期, out list_批號);

                        List_RowsLED_入賬資料.Add_NewRowsLED(rowsDevices[i]);
                        RowsLED rowsLED = List_RowsLED_入賬資料.SortByIP(rowsDevices[i].IP);
                        this.rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                        break;
                    }
                    else if (儲位庫存 >= 0)
                    {
                        Logger.Log("   → 正常異動，更新效期庫存");

                        rowsDevices[i].效期庫存異動(效期, 異動量);
                        List_RowsLED_入賬資料.Add_NewRowsLED(rowsDevices[i]);
                        RowsLED rowsLED = List_RowsLED_入賬資料.SortByIP(rowsDevices[i].IP);
                        this.rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                        break;
                    }
                    else if (儲位庫存 == -1)
                    {
                        Logger.Log("   → 新增效期");

                        rowsDevices[i].新增效期(效期, 批號, 異動量.ToString());
                        List_RowsLED_入賬資料.Add_NewRowsLED(rowsDevices[i]);
                        RowsLED rowsLED = List_RowsLED_入賬資料.SortByIP(rowsDevices[i].IP);
                        this.rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                    }
                }
            }

            // *****************************************************
            //  RFID
            // *****************************************************
            else if (str_TYPE == DeviceType.RFID_Device.GetEnumName())
            {
                Logger.Log("→ 設備類型：RFID");

                List<RFIDDevice> rFIDDevices = List_RFID_入賬資料.SortByCode(藥品碼);
                for (int i = 0; i < rFIDDevices.Count; i++)
                {
                    if (rFIDDevices[i].IP != IP) continue;

                    rFIDDevices[i] = this.rfiD_UI.SQL_GetDevice(rFIDDevices[i]);
                    儲位庫存 = rFIDDevices[i].取得庫存(效期);

                    Logger.Log($"   儲位庫存 : {儲位庫存}");
                    Logger.Log($"   異動量   : {異動量}");

                    if (儲位庫存 >= 0)
                    {
                        Logger.Log("   → 正常異動，更新效期庫存");

                        rFIDDevices[i].效期庫存異動(效期, 異動量);
                        List_RFID_入賬資料.Add_NewRFIDClass(rFIDDevices[i]);
                        RFIDClass rFIDClass = List_RFID_入賬資料.SortByIP(rFIDDevices[i].IP);
                        this.rfiD_UI.SQL_ReplaceRFIDClass(rFIDClass);
                        break;
                    }
                    else if (儲位庫存 == -1)
                    {
                        Logger.Log("   → 無效期紀錄，新增效期");

                        rFIDDevices[i].新增效期(效期, 批號, 異動量.ToString());
                        List_RFID_入賬資料.Add_NewRFIDClass(rFIDDevices[i]);
                        RFIDClass rFIDClass = List_RFID_入賬資料.SortByIP(rFIDDevices[i].IP);
                        this.rfiD_UI.SQL_ReplaceRFIDClass(rFIDClass);
                    }
                }
            }

            // *****************************************************
            //  設定已入帳 Flags
            // *****************************************************
            堆疊子資料[(int)enum_取藥堆疊子資料.已入帳] = true.ToString();
            堆疊子資料[(int)enum_取藥堆疊子資料.致能] = true.ToString();
            堆疊子資料[(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
            堆疊子資料[(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();

            Logger.Log("--------------------------------------------------");
            Logger.Log(" 子資料標記：已入帳 = True");
            Logger.Log(" 子資料標記：致能 = True");
            Logger.Log(" 子資料標記：流程作業完成 = True");
            Logger.Log(" 子資料標記：配藥完成 = True");
            Logger.Log("==================================================");

            return 堆疊子資料;
        }

        private List<object[]> Function_取藥堆疊子資料_取得可致能(ref List<object[]> list_value)
        {
            string IP;
            string Num = "";
            string 調劑台名稱 = "";
            string 藥品碼 = "";
            string Type = "";
            string 致能 = "";
            string 流程作業完成 = "";
            string 配藥完成 = "";
            bool flag_可致能資料 = true;
            List<object[]> list_取藥堆疊子資料 = list_value;
            List<object[]> list_取藥堆疊子資料_buf = new List<object[]>();
            list_取藥堆疊子資料.Sort(new Icp_取藥堆疊子資料_致能排序());
            for (int i = 0; i < list_取藥堆疊子資料.Count; i++)
            {
                flag_可致能資料 = true;
                IP = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.IP].ObjectToString();
                Num = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.Num].ObjectToString();
                致能 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.致能].ObjectToString();
                流程作業完成 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString();
                配藥完成 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.配藥完成].ObjectToString();
                藥品碼 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();
                Type = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString();
                if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                {
                    if (Type == DeviceType.EPD266.GetEnumName() || Type == DeviceType.EPD266_lock.GetEnumName()) flag_可致能資料 = false;
                    if (Type == DeviceType.EPD290.GetEnumName() || Type == DeviceType.EPD290_lock.GetEnumName()) flag_可致能資料 = false;
                    if (Type == DeviceType.EPD213.GetEnumName() || Type == DeviceType.EPD213_lock.GetEnumName()) flag_可致能資料 = false;
                    if (Type == DeviceType.EPD420.GetEnumName() || Type == DeviceType.EPD420_lock.GetEnumName()) flag_可致能資料 = false;
                    if (Type == DeviceType.EPD1020.GetEnumName() || Type == DeviceType.EPD1020_lock.GetEnumName()) flag_可致能資料 = false;
                    if (Type == DeviceType.EPD420G.GetEnumName() || Type == DeviceType.EPD420G_lock.GetEnumName()) flag_可致能資料 = false;
                    if (Type == DeviceType.EPD360E.GetEnumName() || Type == DeviceType.EPD360E_lock.GetEnumName()) flag_可致能資料 = false;
                    if (Type == DeviceType.RowsLED.GetEnumName()) flag_可致能資料 = false;
                    if (Type == DeviceType.RFID_Device.GetEnumName()) flag_可致能資料 = false;
                }

                if (flag_可致能資料)
                {
                    List<object[]> list_temp = (from value in list_取藥堆疊子資料_buf
                                                where IP == value[(int)enum_取藥堆疊子資料.IP].ObjectToString()
                                                where Num == value[(int)enum_取藥堆疊子資料.Num].ObjectToString()
                                                where 調劑台名稱 != value[(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString()
                                                where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                                select value).ToList();
                    if (list_temp.Count > 0) flag_可致能資料 = false;
                }

                if (flag_可致能資料)
                {
                    list_取藥堆疊子資料_buf.Add(list_取藥堆疊子資料[i]);
                }
            }
            return list_取藥堆疊子資料_buf;
        }
        private List<object[]> Function_取藥堆疊母資料_取得可入賬資料()
        {
            List<object[]> list_取藥堆疊母資料 = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
            list_取藥堆疊母資料 = (from value in list_取藥堆疊母資料
                            where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName()
                            select value).ToList();
            return list_取藥堆疊母資料;
        }

        private void Function_取藥堆疊資料_檢查資料儲位正常(object[] list_母資料)
        {
            string Master_GUID = list_母資料[(int)enum_取藥堆疊母資料.GUID].ObjectToString();
            if (list_母資料[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == "系統領藥") return;
            double 總異動量 = list_母資料[(int)enum_取藥堆疊母資料.總異動量].StringToDouble();
            if (list_母資料[(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringIsDouble() == false)
            {
                總異動量 = -99999;
            }
            List<object[]> list_取藥堆疊子資料 = this.sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
            list_取藥堆疊子資料 = list_取藥堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);
            double 累積異動量 = 0;
            for (int i = 0; i < list_取藥堆疊子資料.Count; i++)
            {
                累積異動量 += list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.異動量].StringToDouble();
            }
            if (總異動量 != 累積異動量)
            {
                this.sqL_DataGridView_取藥堆疊子資料.SQL_DeleteExtra(list_取藥堆疊子資料, false);
            }
        }
        #endregion

        private void Program_取藥堆疊資料_Init()
        {

            string url = $"{dBConfigClass.Api_URL}/api/OutTakeMed/init";
            returnData returnData = new returnData();
            returnData.ServerType = enum_sys_serverSetting_Type.調劑台.GetEnumName();
            returnData.ServerName = $"{dBConfigClass.Name}";
            string json_in = returnData.JsonSerializationt();
            string json = Basic.Net.WEBApiPostJson($"{url}", json_in);
            List<SQLUI.Table> tables = takeMedicineStackClass.init(Main_Form.API_Server, $"{dBConfigClass.Name}", enum_sys_serverSetting_Type.調劑台.GetEnumName());


            _plC_CheckBox_面板於調劑結束更新 = this.plC_CheckBox_面板於調劑結束更新;

            if (tables == null)
            {
                MyMessageBox.ShowDialog($"取藥堆疊表單建立失敗!! Api_URL:{url}");
                return;
            }


            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].TableName == this.sqL_DataGridView_取藥堆疊母資料.TableName)
                {
                    this.sqL_DataGridView_取藥堆疊母資料.Init(tables[i]);
                }
                if (tables[i].TableName == this.sqL_DataGridView_取藥堆疊子資料.TableName)
                {
                    this.sqL_DataGridView_取藥堆疊子資料.Init(tables[i]);
                }
            }
            if (PLC_Device_主機扣賬模式.Bool)
            {
                sqL_DataGridView_取藥堆疊母資料.Server = "127.0.0.1";
                sqL_DataGridView_取藥堆疊子資料.Server = "127.0.0.1";
            }


            _sqL_DataGridView_取藥堆疊母資料 = this.sqL_DataGridView_取藥堆疊母資料;
            _sqL_DataGridView_取藥堆疊子資料 = this.sqL_DataGridView_取藥堆疊子資料;


            this.MyThread_取藥堆疊資料_檢查資料 = new MyThread();
            this.MyThread_取藥堆疊資料_檢查資料.AutoRun(true);
            this.MyThread_取藥堆疊資料_檢查資料.AutoStop(true);
            this.MyThread_取藥堆疊資料_檢查資料.Add_Method(this.sub_Program_取藥堆疊資料_檢查資料);
            this.MyThread_取藥堆疊資料_檢查資料.Add_Method(this.sub_Program_取藥堆疊資料_狀態更新);
            this.MyThread_取藥堆疊資料_檢查資料.Add_Method(this.sub_Program_取藥堆疊資料_流程作業檢查);
            this.MyThread_取藥堆疊資料_檢查資料.Add_Method(this.sub_Program_取藥堆疊資料_入賬檢查);
            this.MyThread_取藥堆疊資料_檢查資料.Trigger();


            this.MyThread_取藥堆疊資料_儲位亮燈 = new MyThread();
            this.MyThread_取藥堆疊資料_儲位亮燈.AutoRun(true);
            this.MyThread_取藥堆疊資料_儲位亮燈.AutoStop(true);
            this.MyThread_取藥堆疊資料_儲位亮燈.Add_Method(this.sub_Program_取藥堆疊資料_儲位亮燈);
            this.MyThread_取藥堆疊資料_儲位亮燈.Trigger();
        }
        public class LightOn
        {
            public LightOn(string 藥碼, Color color, double qty)
            {
                藥品碼 = 藥碼;
                顏色 = color;
                數量 = qty;
                LCD_Color = color;
                flag_Refresh_LCD = true;
            }
            public LightOn(string 藥碼, Color color)
            {
                藥品碼 = 藥碼;
                顏色 = color;
                LCD_Color = color;
                數量 = 0;
            }
            public LightOn()
            {

            }
            public LightOn Copy()
            {
                return Copy(this);
            }
            public LightOn Copy(LightOn lightOn)
            {
                LightOn lightOn_buf = new LightOn();

                lightOn_buf.IP = lightOn.IP;
                lightOn_buf.藥品碼 = lightOn.藥品碼;
                lightOn_buf.顏色 = lightOn.顏色;
                lightOn_buf.數量 = lightOn.數量;
                lightOn_buf.LCD_Color = lightOn.LCD_Color;
                lightOn_buf.flag_Refresh_LCD = lightOn.flag_Refresh_LCD;
                lightOn_buf.flag_Refresh_Light = lightOn.flag_Refresh_Light;
                lightOn_buf.flag_Refresh_breathing = lightOn.flag_Refresh_breathing;

                return lightOn_buf;
            }
            public string IP { get; set; }
            public string 藥品碼 { get; set; }
            public Color 顏色 { get; set; }
            public Color LCD_Color { get; set; }
            public double 數量 { get; set; }
            public bool flag_Refresh_LCD = false;
            public bool flag_Refresh_Light = false;
            public bool flag_Refresh_breathing = false;
        }
        static private List<LightOn> lightOns = new List<LightOn>();
        private int cnt_儲位亮燈 = 0;
        private int 儲位亮燈數量_temp = 0;
        private MyTimerBasic MyTimerBasic_儲位亮燈 = new MyTimerBasic();
        private void sub_Program_取藥堆疊資料_儲位亮燈()
        {
            if (lightOns.Count > 0)
            {
                lock (lightOns)
                {
                    try
                    {
                        if (cnt_儲位亮燈 == 0)
                        {
                            儲位亮燈數量_temp = lightOns.Count;
                            MyTimerBasic_儲位亮燈.TickStop();
                            MyTimerBasic_儲位亮燈.StartTickTime(20);
                            cnt_儲位亮燈++;
                        }
                        if (cnt_儲位亮燈 == 1)
                        {
                            if (MyTimerBasic_儲位亮燈.IsTimeOut())
                            {
                                if (儲位亮燈數量_temp == lightOns.Count)
                                {
                                    cnt_儲位亮燈++;
                                }
                                else
                                {
                                    cnt_儲位亮燈 = 0;
                                }
                            }
                        }
                        if (cnt_儲位亮燈 == 2)
                        {
                            List<LightOn> lightOns_buf = new List<LightOn>();
                            for (int i = 0; i < lightOns.Count; i++)
                            {
                                LightOn lightOn = lightOns[i].Copy();

                                lightOns_buf.Add(lightOn);
                            }
                            lightOns.Clear();

                            List<Task> taskList_抽屜層架 = new List<Task>();

                            List<string> list_抽屜亮燈_IP = new List<string>();
                            List<string> list_層架亮燈_IP = new List<string>();
                            List<string> list_EPD1020亮燈_IP = new List<string>();

                            for (int i = 0; i < lightOns_buf.Count; i++)
                            {
                                string 藥品碼 = lightOns_buf[i].藥品碼;
                                Color 顏色 = lightOns_buf[i].顏色;
                                list_抽屜亮燈_IP.LockAdd(Function_儲位亮燈_取得抽屜亮燈IP(lightOns_buf[i]));
                                list_層架亮燈_IP.LockAdd(Function_儲位亮燈_取得層架亮燈IP(藥品碼, 顏色));
                                list_EPD1020亮燈_IP.LockAdd(Function_儲位亮燈_取得EPD1020亮燈IP(藥品碼, 顏色));
                            }

                            taskList_抽屜層架.Add(Task.Run(() =>
                            {
                                Function_儲位亮燈_抽屜亮燈(list_抽屜亮燈_IP);
                            }));
                            taskList_抽屜層架.Add(Task.Run(() =>
                            {
                                Function_儲位亮燈_層架亮燈(list_層架亮燈_IP);
                            }));
                            taskList_抽屜層架.Add(Task.Run(() =>
                            {
                                Function_儲位亮燈_EPD1020亮燈(list_EPD1020亮燈_IP);
                            }));

                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < lightOns_buf.Count; i++)
                            {
                                LightOn lightOn = lightOns_buf[i];

                                taskList.Add(Task.Run(() =>
                                {
                                    Function_儲位亮燈_Ex(lightOn);

                                }));
                            }
                            Task.WhenAll(taskList).Wait();
                            Task.WhenAll(taskList_抽屜層架).Wait();
                            cnt_儲位亮燈++;
                        }
                        if (cnt_儲位亮燈 == 3)
                        {
                            cnt_儲位亮燈 = 0;
                        }


                    }
                    catch
                    {

                    }
                    finally
                    {

                    }


                }
            }

        }
        public List<string> Function_儲位亮燈_取得抽屜亮燈IP(LightOn lightOn)
        {
            string 藥品碼 = lightOn.藥品碼;
            Color color = lightOn.顏色;
            if (藥品碼.StringIsEmpty()) return new List<string>();
            List<object> list_Device = Function_從本地資料取得儲位(藥品碼);
            //List<object> list_commonSpace_device = Function_從共用區取得儲位(藥品碼);
            //for (int i = 0; i < list_commonSpace_device.Count; i++)
            //{
            //    list_Device.Add(list_commonSpace_device[i]);
            //}
            bool flag_led_refresh = true;
            List<string> list_IP = new List<string>();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;

                if (device == null) continue;

                if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock
                    || device.DeviceType == DeviceType.EPD420_D || device.DeviceType == DeviceType.EPD420_D_lock
                    || device.DeviceType == DeviceType.EPD730E || device.DeviceType == DeviceType.EPD730E_lock)
                {
                    Box box = list_Device[i] as Box;
                    if (box != null)
                    {
                        bool flag_common_device = false;
                        Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
                        int numOfLED = 400;
                        if (drawer.DrawerType == Drawer.Enum_DrawerType._4X8 || drawer.DrawerType == Drawer.Enum_DrawerType._3X8) numOfLED = 400;
                        if (drawer.DrawerType == Drawer.Enum_DrawerType._4X8_A || drawer.DrawerType == Drawer.Enum_DrawerType._5X8_A) numOfLED = 365;
                        if (drawer == null)
                        {
                            drawer = CommonSapceClass.GetEPD583(IP, ref commonSapceClasses);
                            //drawer.LED_Bytes = this.drawerUI_EPD_583.Get_Drawer_LED_UDP(drawer);

                            flag_common_device = true;
                        }
                        if (drawer == null) continue;
                        List<Box> boxes = drawer.SortByCode(藥品碼);

                        if (drawer.IsAllLight)
                        {
                            byte[] LED_Bytes = new byte[drawer.LED_Bytes.Length];
                            for (int k = 0; k < drawer.LED_Bytes.Length; k++)
                            {
                                LED_Bytes[k] = drawer.LED_Bytes[k];
                            }
                            bool flag_commonlight = false;
                            List<MyColor> myColors = new List<MyColor>();
                            if (color.R != 0 || color.G != 0 || color.B != 0)
                            {
                                for (int k = 0; k < numOfLED; k++)
                                {
                                    int R = LED_Bytes[k * 3 + 0];
                                    int G = LED_Bytes[k * 3 + 1];
                                    int B = LED_Bytes[k * 3 + 2];
                                    if (R == 0 && G == 0 && B == 0) continue;

                                    if (R != color.R || G != color.G || B != color.B)
                                    {
                                        Console.WriteLine($"藥品碼 : {藥品碼} , {color.R},{color.G},{color.B} [{R},{G},{B}]");
                                        flag_commonlight = true;
                                        break;
                                    }
                                }
                            }

                            drawer.LED_Bytes = DrawerUI_EPD_583.Set_LEDBytes(drawer, boxes, color, !lightOn.flag_Refresh_Light);
                            if (!flag_commonlight || lightOn.flag_Refresh_Light)
                            {
                                if (color.R != 0 || color.G != 0 || color.B != 0)
                                {
                                    if (lightOn.flag_Refresh_Light == false) drawer.LED_Bytes = DrawerUI_EPD_583.Set_Pannel_LEDBytes(drawer, color);
                                }
                                else
                                {
                                    bool flag_led_black_enable = true;
                                    for (int k = 0; k < numOfLED; k++)
                                    {
                                        int R = drawer.LED_Bytes[k * 3 + 0];
                                        int G = drawer.LED_Bytes[k * 3 + 1];
                                        int B = drawer.LED_Bytes[k * 3 + 2];
                                        if (R != 0 || G != 0 || B != 0)
                                        {
                                            flag_led_black_enable = false;
                                            break;
                                        }
                                    }
                                    if (flag_led_black_enable || flag_common_device) drawer.LED_Bytes = DrawerUI_EPD_583.Set_Pannel_LEDBytes(drawer, color);

                                }
                            }
                            else drawer.LED_Bytes = DrawerUI_EPD_583.Set_Pannel_LEDBytes(drawer, Color.Purple);


                            flag_led_refresh = true;
                            for (int k = 0; k < drawer.LED_Bytes.Length; k++)
                            {
                                if (LED_Bytes[k] != drawer.LED_Bytes[k])
                                {
                                    flag_led_refresh = true;
                                }
                            }
                        }
                        else
                        {
                            drawer.LED_Bytes = DrawerUI_EPD_583.Set_LEDBytes(drawer, color);
                            flag_led_refresh = true;
                        }
                    }
                    list_IP.Add(IP);
                }

            }
            list_IP = (from temp in list_IP
                       select temp).Distinct().ToList();
            return list_IP;
        }
        public void Function_儲位亮燈_抽屜亮燈(List<string> list_IP)
        {
            try
            {
                list_IP = (from temp in list_IP
                           select temp).Distinct().ToList();
                Task allTask;
                List<Task> taskList = new List<Task>();
                for (int i = 0; i < list_IP.Count; i++)
                {
                    string IP = list_IP[i];
                    taskList.Add(Task.Run(() =>
                    {
                        Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
                        if (drawer == null)
                        {
                            drawer = CommonSapceClass.GetEPD583(IP, ref commonSapceClasses);
                        }
                        if (drawer == null) return;
                        if (!plC_CheckBox_測試模式.Checked)
                        {
                            bool flag_black = true;
                            int num_of_panel_led = DrawerUI_EPD_583.GetPanel_LED_num(drawer.DrawerType);
                            int num_of_drawer_led = DrawerUI_EPD_583.GetDrawer_LED_num(drawer.DrawerType);
                            for (int k = 0; k < num_of_drawer_led * 3; k++)
                            {
                                if (drawer.LED_Bytes[k] != 0)
                                {
                                    flag_black = false;
                                    break;
                                }
                            }
                            if (flag_black && drawer.BreathLight)
                            {
                                drawer.LED_Bytes = DrawerUI_EPD_583.Get_Empty_LEDBytes();
                            }
                            this.drawerUI_EPD_583.Set_LED_UDP(drawer);
                        }

                    }));

                }
                allTask = Task.WhenAll(taskList);
                allTask.Wait();
            }
            catch
            {

            }
            finally
            {

            }

        }

        public List<string> Function_儲位亮燈_取得層架亮燈IP(string 藥品碼, Color color)
        {
            if (藥品碼.StringIsEmpty()) return new List<string>();
            List<object> list_Device = Function_從本地資料取得儲位(藥品碼);
            //List<object> list_commonSpace_device = Function_從共用區取得儲位(藥品碼);
            //for (int i = 0; i < list_commonSpace_device.Count; i++)
            //{
            //    list_Device.Add(list_commonSpace_device[i]);
            //}
            bool flag_led_refresh = true;
            List<string> list_IP = new List<string>();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;

                if (device == null) continue;
                if (device.DeviceType == DeviceType.RowsLED)
                {
                    RowsDevice rowsDevice = list_Device[i] as RowsDevice;
                    if (rowsDevice != null)
                    {
                        RowsLED rowsLED = List_RowsLED_本地資料.SortByIP(rowsDevice.IP);
                        if (rowsLED == null)
                        {
                            rowsLED = CommonSapceClass.GetRowsLED(IP, ref commonSapceClasses);
                            //rowsLED.LED_Bytes = this.rowsLEDUI.Get_RowsLED_LED_UDP(rowsLED);
                        }
                        rowsLED.LED_Bytes = RowsLEDUI.Get_Rows_LEDBytes(ref rowsLED.LED_Bytes, rowsDevice, color);
                    }

                    list_IP.Add(IP);
                }



            }
            list_IP = (from temp in list_IP
                       select temp).Distinct().ToList();
            return list_IP;
        }
        public void Function_儲位亮燈_層架亮燈(List<string> list_IP)
        {
            try
            {
                list_IP = (from temp in list_IP
                           select temp).Distinct().ToList();
                Task allTask;
                List<Task> taskList = new List<Task>();
                for (int i = 0; i < list_IP.Count; i++)
                {
                    string IP = list_IP[i];
                    taskList.Add(Task.Run(() =>
                    {
                        RowsLED rowsLED = List_RowsLED_本地資料.SortByIP(IP);
                        if (rowsLED == null)
                        {
                            rowsLED = CommonSapceClass.GetRowsLED(IP, ref commonSapceClasses);
                        }
                        taskList.Add(Task.Run(() =>
                        {
                            if (!plC_CheckBox_測試模式.Checked)
                            {
                                this.rowsLEDUI.Set_Rows_LED_UDP(rowsLED);
                            }

                        }));


                    }));

                }
                allTask = Task.WhenAll(taskList);
                allTask.Wait();
            }
            catch
            {

            }
            finally
            {

            }

        }

        public List<string> Function_儲位亮燈_取得EPD1020亮燈IP(string 藥品碼, Color color)
        {
            if (藥品碼.StringIsEmpty()) return new List<string>();
            List<object> list_Device = Function_從本地資料取得儲位(藥品碼);

            bool flag_led_refresh = true;
            List<string> list_IP = new List<string>();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;

                if (device == null) continue;

                if (device.DeviceType == DeviceType.EPD1020 || device.DeviceType == DeviceType.EPD1020_lock)
                {
                    Box box = list_Device[i] as Box;
                    if (box != null)
                    {
                        Drawer drawer = List_EPD1020_雲端資料.SortByIP(IP);

                        if (drawer == null) continue;
                        List<Box> boxes = drawer.SortByCode(藥品碼);
                        drawer.LED_Bytes = DrawerUI_EPD_1020.Set_Pannel_LEDBytes(drawer, color);
                        drawer.LED_Bytes = DrawerUI_EPD_1020.Set_LEDBytes(drawer, color);
                    }
                    list_IP.Add(IP);
                }

            }
            list_IP = (from temp in list_IP
                       select temp).Distinct().ToList();
            return list_IP;
        }
        public void Function_儲位亮燈_EPD1020亮燈(List<string> list_IP)
        {
            try
            {
                list_IP = (from temp in list_IP
                           select temp).Distinct().ToList();
                Task allTask;
                List<Task> taskList = new List<Task>();
                for (int i = 0; i < list_IP.Count; i++)
                {
                    string IP = list_IP[i];
                    taskList.Add(Task.Run(() =>
                    {
                        Drawer drawer = List_EPD1020_雲端資料.SortByIP(IP);

                        if (drawer == null) return;
                        if (!plC_CheckBox_測試模式.Checked)
                        {
                            this.drawerUI_EPD_1020.Set_LED_UDP(drawer);
                        }

                    }));

                }
                allTask = Task.WhenAll(taskList);
                allTask.Wait();
            }
            catch
            {

            }
            finally
            {

            }

        }


        public void Function_儲位亮燈_Ex(LightOn lightOn)
        {
            string 藥品碼 = lightOn.藥品碼;
            Color color = lightOn.顏色;
            double 數量 = lightOn.數量;

            if (藥品碼.StringIsEmpty()) return;
            List<object> list_Device = new List<object>();
            if(lightOn.IP.StringIsEmpty()) list_Device = Function_從雲端資料取得儲位(藥品碼);
            else list_Device = Function_從雲端資料取得儲位_ByIP(lightOn.IP);
            List<object> list_commonSpace_device = Function_從共用區取得儲位(藥品碼);
            for (int i = 0; i < list_commonSpace_device.Count; i++)
            {
                list_Device.Add(list_commonSpace_device[i]);
            }


            if (color == Color.Black)
            {
                Console.WriteLine($"●●儲位滅燈●●,藥品碼:{藥品碼},color{color.ToColorString()}-------------");
            }
            else
            {
                Console.WriteLine($"◇◇儲位亮燈◇◇,藥品碼:{藥品碼},color{color.ToColorString()}");
            }
            Task allTask;
            List<Task> taskList = new List<Task>();
            List<string> list_IP = new List<string>();
            List<string> list_IP_buf = new List<string>();
            bool flag_led_refresh = false;

            list_IP.Clear();
            list_IP_buf.Clear();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;

                if (device != null)
                {


                }
            }
            taskList.Clear();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;
                list_IP_buf = (from value in list_IP
                               where value == IP
                               select value).ToList();
                if (list_IP_buf.Count > 0) continue;

                if (device != null)
                {
                    if (device.DeviceIsStorage())
                    {
                        Storage storage = list_Device[i] as Storage;
                        if (storage == null) continue;
                        //if (storage.IsFADC) continue;
                        taskList.Add(Task.Run(() =>
                        {
                            if (plC_CheckBox_測試模式.Checked) return;
                            if (storage.TOFON == false)
                            {
                                if (color == Color.Black)
                                {
                                    if (myConfigClass.舊版晶片 == false)
                                    {
                                        this.storageUI_EPD_266.Set_WS2812B_breathing(storage, 30, 30, color);
                                    }
                                    else
                                    {
                                        this.storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                                    }
                                }
                                else
                                {
                                    this.storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                                }
                            }
                            else
                            {
                                if (color == Color.Black)
                                {
                                    if (myConfigClass.舊版晶片 == false)
                                    {
                                        this.storageUI_EPD_266.Set_WS2812B_breathing(storage, 30, 30, color);
                                    }
                                    else
                                    {
                                        this.storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);

                                    }

                                }
                                else if (lightOn.flag_Refresh_LCD || lightOn.flag_Refresh_Light)
                                {
                                    if (lightOn.flag_Refresh_breathing)
                                    {
                                        if (myConfigClass.舊版晶片 == false)
                                        {
                                            this.storageUI_EPD_266.Set_WS2812B_breathing(storage, 30, 30, color);
                                        }
                                        else
                                        {
                                            this.storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                                        }

                                    }
                                    else this.storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                                }

                            }
                            string index_IP = Funcion_取得LCD114索引表_index_IP(storage.IP);
                            if (index_IP.StringIsEmpty()) return;
                            if (color == Color.Black) storageUI_LCD_114.ClearCanvas(index_IP, 29008);
                            if (lightOn.flag_Refresh_LCD)
                            {
                                Color color_fore = Color.White;
                                if (lightOn.LCD_Color.R > 230 && lightOn.LCD_Color.G > 230 && lightOn.LCD_Color.B > 230) color_fore = Color.Black;
                                storageUI_LCD_114.DrawImage(index_IP, 29008, 數量.ToString(), new Font("標楷體", 70, FontStyle.Bold), color_fore, lightOn.LCD_Color);
                            }

                        }));

                        list_IP.Add(IP);
                    }
                    else if (device.DeviceType == DeviceType.Pannel35 || device.DeviceType == DeviceType.Pannel35_lock)
                    {
                        Storage storage = list_Device[i] as Storage;
                        if (storage != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                if (!plC_CheckBox_測試模式.Checked)
                                {
                                    this.storageUI_WT32.Set_Stroage_LED_UDP(storage, color);
                                }

                            }));

                            list_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock
                        || device.DeviceType == DeviceType.EPD420_D || device.DeviceType == DeviceType.EPD420_D_lock
                        || device.DeviceType == DeviceType.EPD730E || device.DeviceType == DeviceType.EPD730E_lock)
                    {
                        taskList.Add(Task.Run(() =>
                        {

                            string index_IP = Funcion_取得LCD114索引表_index_IP(device.IP);
                            if (index_IP.StringIsEmpty()) return;
                            if (color == Color.Black) storageUI_LCD_114.ClearCanvas(index_IP, 29008);
                            if (lightOn.flag_Refresh_LCD)
                            {
                                Color color_fore = Color.White;
                                if (lightOn.LCD_Color.R > 230 && lightOn.LCD_Color.G > 230 && lightOn.LCD_Color.B > 230) color_fore = Color.Black;
                                storageUI_LCD_114.DrawImage(index_IP, 29008, 數量.ToString(), new Font("標楷體", 70, FontStyle.Bold), color_fore, lightOn.LCD_Color);
                            }

                        }));
                    }
           
                }
            }
            allTask = Task.WhenAll(taskList);
            allTask.Wait();
        }

        #region PLC_取藥堆疊資料_檢查資料
        PLC_Device PLC_Device_取藥堆疊資料_檢查資料 = new PLC_Device("");
        PLC_Device PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料 = new PLC_Device("");
        MyTimer MyTimer_取藥堆疊資料_刷新時間 = new MyTimer("K1");
        MyTimer MyTimer_取藥堆疊資料_自動過帳時間 = new MyTimer();
        MyTimer MyTimer_取藥堆疊資料_資料更新時間 = new MyTimer();
        int cnt_Program_取藥堆疊資料_檢查資料 = 65534;
        void sub_Program_取藥堆疊資料_檢查資料()
        {
            //this.MyThread_取藥堆疊資料_檢查資料.GetCycleTime(100, this.label_取要推疊_資料更新時間);
            PLC_Device_取藥堆疊資料_檢查資料.Bool = PLC_Device_主機扣賬模式.Bool;
            if (cnt_Program_取藥堆疊資料_檢查資料 == 65534)
            {
                List<object[]> list_堆疊母資料 = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
                List<object[]> list_堆疊子資料 = this.sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
                this.sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_堆疊母資料, false);
                this.sqL_DataGridView_取藥堆疊子資料.SQL_DeleteExtra(list_堆疊子資料, false);
                PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料.Bool = true;
                PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料.SetComment("PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料");
                PLC_Device_取藥堆疊資料_檢查資料.SetComment("PLC_取藥堆疊資料_檢查資料");
                PLC_Device_取藥堆疊資料_檢查資料.Bool = false;
                cnt_Program_取藥堆疊資料_檢查資料 = 65535;
            }
            if (cnt_Program_取藥堆疊資料_檢查資料 == 65535) cnt_Program_取藥堆疊資料_檢查資料 = 1;
            if (cnt_Program_取藥堆疊資料_檢查資料 == 1) cnt_Program_取藥堆疊資料_檢查資料_檢查按下(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 2) cnt_Program_取藥堆疊資料_檢查資料_初始化(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 3) cnt_Program_取藥堆疊資料_檢查資料_檢查新增資料(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 4) cnt_Program_取藥堆疊資料_檢查資料_檢查儲位刷新(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 5) cnt_Program_取藥堆疊資料_檢查資料_檢查儲位亮燈(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 6) cnt_Program_取藥堆疊資料_檢查資料_檢查處方存在時間到達(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 7) cnt_Program_取藥堆疊資料_檢查資料_刷新新增效期(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 8) cnt_Program_取藥堆疊資料_檢查資料_堆疊資料整理(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 9) cnt_Program_取藥堆疊資料_檢查資料_從SQL讀取儲位資料(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 10) cnt_Program_取藥堆疊資料_檢查資料_刷新無庫存(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 11) cnt_Program_取藥堆疊資料_檢查資料_刷新資料(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 12) cnt_Program_取藥堆疊資料_檢查資料_設定致能(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 13) cnt_Program_取藥堆疊資料_檢查資料_等待刷新時間到達(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 14) cnt_Program_取藥堆疊資料_檢查資料 = 65500;
            if (cnt_Program_取藥堆疊資料_檢查資料 > 1) cnt_Program_取藥堆疊資料_檢查資料_檢查放開(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 65500)
            {
                PLC_Device_取藥堆疊資料_檢查資料.Bool = false;
                cnt_Program_取藥堆疊資料_檢查資料 = 65535;
            }
        }
        void cnt_Program_取藥堆疊資料_檢查資料_檢查按下(ref int cnt)
        {
            if (PLC_Device_取藥堆疊資料_檢查資料.Bool) cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取藥堆疊資料_檢查資料.Bool) cnt = 65500;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_初始化(ref int cnt)
        {
            MyTimer_取藥堆疊資料_資料更新時間.TickStop();
            MyTimer_取藥堆疊資料_資料更新時間.StartTickTime(9999999);

            MyTimer_取藥堆疊資料_自動過帳時間.StartTickTime(10000);
            if (MyTimer_取藥堆疊資料_自動過帳時間.IsTimeOut())
            {
                if (plC_CheckBox_自動過帳.Checked)
                {
                    //PlC_RJ_Button_醫囑資料_自動過帳_MouseDownEvent(null);
                }
                MyTimer_取藥堆疊資料_自動過帳時間.TickStop();
            }

            cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_檢查新增資料(ref int cnt)
        {
            List<object[]> list_value = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetRows((int)enum_取藥堆疊母資料.狀態, "新增資料", false);

            if (list_value.Count > 0)
            {
                List<string> names = (from temp in list_value
                                      select temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString()).Distinct().ToList();

                for (int i = 0; i < names.Count; i++)
                {
                    Function_取藥堆疊資料_刪除指定調劑台名稱母資料(names[i]);
                }

                this.sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_value, false);
                Console.WriteLine($"刪除[新增資料]共<{list_value.Count}>筆");
                List<takeMedicineStackClass> takeMedicineStackClasses = list_value.SQLToClass<takeMedicineStackClass, enum_取藥堆疊母資料>();
                Function_取藥堆疊資料_新增母資料(takeMedicineStackClasses);
            }

            cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_檢查儲位刷新(ref int cnt)
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------
            //檢查系統領藥是否資料是否到達時間
            this.list_取藥堆疊母資料 = Function_取藥堆疊資料_取得母資料();
            this.list_取藥堆疊母資料 = this.list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, "刷新面板");
            List<object[]> list_取藥堆疊母資料_delete = new List<object[]>();
            int 刷新時間 = 2;
            if (plC_CheckBox_面板於過帳後更新.Checked) 刷新時間 = 0;
            for (int i = 0; i < this.list_取藥堆疊母資料.Count; i++)
            {
                DateTime dt_start = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.操作時間].ObjectToString().StringToDateTime();
                DateTime dt_end = DateTime.Now;
                TimeSpan ts = dt_end - dt_start;
                if (ts.TotalSeconds >= 刷新時間)
                {
                    list_取藥堆疊母資料_delete.Add(this.list_取藥堆疊母資料[i]);
                    string 藥品碼 = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    string IP = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.IP].ObjectToString();
                    if (藥品碼.StringIsEmpty() == false && 藥品碼 != "None")
                    {
                        Task.Run(() =>
                        {
                            Function_從SQL取得儲位到本地資料(藥品碼);
                            Function_儲位刷新(藥品碼);
                        });
                    }
                    if (IP.StringIsEmpty() == false)
                    {
                        Task.Run(() =>
                        {
                            //Function_從SQL取得儲位到本地資料(IP);
                            this.Function_儲位刷新_byIP(IP);
                        });
                    }

                }
            }
            if (list_取藥堆疊母資料_delete.Count > 0)
            {
                this.sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料_delete, false);
            }
            cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_檢查儲位亮燈(ref int cnt)
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------
            //檢查系統領藥是否資料是否到達時間
            List<object[]> list_取藥堆疊母資料 = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetRows((int)enum_取藥堆疊母資料.調劑台名稱, "儲位亮燈", false);
            List<object[]> list_取藥堆疊母資料_buf = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_buf_未亮燈 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.備註, "");
            List<object[]> list_取藥堆疊母資料_buf_已亮燈 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.備註, "已亮燈");
            List<object[]> list_取藥堆疊母資料_delete = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_retplace = new List<object[]>();

            //檢查亮燈時間到達,需滅燈資料
            for (int i = 0; i < list_取藥堆疊母資料_buf_已亮燈.Count; i++)
            {
                string 藥品碼 = list_取藥堆疊母資料_buf_已亮燈[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 顏色 = list_取藥堆疊母資料_buf_已亮燈[i][(int)enum_取藥堆疊母資料.顏色].ObjectToString();
                DateTime dt_start = list_取藥堆疊母資料_buf_已亮燈[i][(int)enum_取藥堆疊母資料.操作時間].ObjectToString().StringToDateTime();
                DateTime dt_end = DateTime.Now;
                TimeSpan ts = dt_end - dt_start;
                double 刷新時間 = list_取藥堆疊母資料_buf_已亮燈[i][(int)enum_取藥堆疊母資料.總異動量].StringToDouble();

                if (ts.TotalSeconds >= 刷新時間)
                {
                    list_取藥堆疊母資料_delete.Add(list_取藥堆疊母資料_buf_已亮燈[i]);
                    if (藥品碼.StringIsEmpty() == false)
                    {
                        Function_儲位亮燈(new Main_Form.LightOn(藥品碼, Color.Black));
                    }
                }
            }
            if (list_取藥堆疊母資料_delete.Count > 0)
            {
                this.sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料_delete, false);
                Console.WriteLine($"{DateTime.Now.ToDateTimeString()}-儲位亮燈-刪除資料  : 共刪除[取藥堆疊母資料]<{list_取藥堆疊母資料_delete.Count}>筆資料");
                for (int i = 0; i < list_取藥堆疊母資料_delete.Count; i++)
                {
                    string 藥品碼 = list_取藥堆疊母資料_delete[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    string Master_GUID = list_取藥堆疊母資料_delete[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                    List<object[]> list_value_delete = this.sqL_DataGridView_取藥堆疊子資料.SQL_GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID, false);

                    if (list_value_delete.Count > 0)
                    {
                        this.sqL_DataGridView_取藥堆疊子資料.SQL_DeleteExtra(list_value_delete, false);
                        Console.WriteLine($"{DateTime.Now.ToDateTimeString()}-儲位亮燈-刪除資料 藥品碼 : {藥品碼} ,共刪除<{list_value_delete.Count}>筆資料");
                    }
                }
                return;
            }
            list_取藥堆疊母資料_delete.Clear();


            for (int i = 0; i < list_取藥堆疊母資料_buf_未亮燈.Count; i++)
            {
                list_取藥堆疊母資料_buf_未亮燈[i][(int)enum_取藥堆疊母資料.備註] = "已亮燈";
                list_取藥堆疊母資料_retplace.Add(list_取藥堆疊母資料_buf_未亮燈[i]);
                string 藥品碼 = list_取藥堆疊母資料_buf_未亮燈[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 顏色 = list_取藥堆疊母資料_buf_未亮燈[i][(int)enum_取藥堆疊母資料.顏色].ObjectToString();
                if (藥品碼.StringIsEmpty() == false)
                {

                    Function_儲位亮燈(new Main_Form.LightOn(藥品碼, 顏色.ToColor()));
                    if (顏色.ToColor() == Color.Black)
                    {
                        list_取藥堆疊母資料_delete.Add(list_取藥堆疊母資料_buf_未亮燈[i]);
                    }
                    else
                    {
                        list_取藥堆疊母資料_buf = list_取藥堆疊母資料_buf_已亮燈.GetRows((int)enum_取藥堆疊母資料.藥品碼, 藥品碼);
                        if (list_取藥堆疊母資料_buf.Count > 0)
                        {
                            list_取藥堆疊母資料_delete.LockAdd(list_取藥堆疊母資料_buf);
                        }
                    }
                }
            }
            if (list_取藥堆疊母資料_retplace.Count > 0 || list_取藥堆疊母資料_delete.Count > 0)
            {
                if (list_取藥堆疊母資料_retplace.Count > 0)
                {
                    this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_retplace, false);
                    Console.WriteLine($"{DateTime.Now.ToDateTimeString()}-儲位亮燈-刪除資料  : 共更新[取藥堆疊母資料]<{list_取藥堆疊母資料_retplace.Count}>筆資料");
                }
                if (list_取藥堆疊母資料_delete.Count > 0)
                {
                    this.sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料_delete, false);
                    Console.WriteLine($"{DateTime.Now.ToDateTimeString()}-儲位亮燈-刪除資料  : 共刪除[取藥堆疊母資料]<{list_取藥堆疊母資料_delete.Count}>筆資料");
                }
                return;
            }

            cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_檢查處方存在時間到達(ref int cnt)
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------
            //檢查領藥是否資料是否到達時間
            if (plC_NumBox_處方存在時間.Value < 20000) plC_NumBox_處方存在時間.Value = 20000;
            this.list_取藥堆疊母資料 = Function_取藥堆疊資料_取得母資料();
            this.list_取藥堆疊母資料 = (from temp in this.list_取藥堆疊母資料
                                 where temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()
                                 || temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName()
                                 || temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.已領用過.GetEnumName()
                                 || temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.DC處方.GetEnumName()
                                 || temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.未授權.GetEnumName()
                                 || temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName()
                                 || temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.無可匹配數量.GetEnumName()
                                 select temp).ToList();
            List<object[]> list_取藥堆疊母資料_delete = new List<object[]>();
            int 處方存在時間 = plC_NumBox_處方存在時間.Value / 1000;
            for (int i = 0; i < this.list_取藥堆疊母資料.Count; i++)
            {
                string code = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                DateTime dt_start = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.操作時間].ObjectToString().StringToDateTime();
                DateTime dt_end = DateTime.Now;
                TimeSpan ts = dt_end - dt_start;
                if (ts.TotalSeconds >= 處方存在時間)
                {
                    Console.WriteLine($"藥碼:{code} 處方時間到達 $ ({ts.TotalSeconds} >= {處方存在時間})");
                    list_取藥堆疊母資料_delete.Add(this.list_取藥堆疊母資料[i]);
                    if (plC_CheckBox_面板於調劑結束更新.Checked) Function_取藥堆疊資料_刷新面板(code);

                }
            }
            if (list_取藥堆疊母資料_delete.Count > 0)
            {
                this.sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料_delete, false);
            }
            for (int i = 0; i < list_取藥堆疊母資料_delete.Count; i++)
            {
                string code = list_取藥堆疊母資料_delete[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                Function_儲位亮燈(new Main_Form.LightOn(code, Color.Black));
            }
            //-----------------------------------------------------------------------------------------------------------------------------------------

            cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_堆疊資料整理(ref int cnt)
        {
            string GUID = "";
            this.list_取藥堆疊母資料 = Function_取藥堆疊資料_取得母資料();
            this.list_取藥堆疊子資料 = this.Function_取藥堆疊資料_取得子資料();
            List<object[]> list_取藥堆疊子資料_DeleteValue = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_資料更新 = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_取消作業 = new List<object[]>();

            //檢查更新雲端資料
            list_取藥堆疊母資料_資料更新 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, "更新資料");
            for (int i = 0; i < list_取藥堆疊母資料_資料更新.Count; i++)
            {
                GUID = list_取藥堆疊母資料_資料更新[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                this.list_取藥堆疊母資料.Remove(list_取藥堆疊母資料_資料更新[i]);
                Function_取藥堆疊資料_刪除母資料(GUID);
                PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料.Bool = true;
            }
            if (PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料.Bool)
            {
                if (dBConfigClass.Med_Update_ApiURL.StringIsEmpty() == false)
                {
                    string Jsonstring = Basic.Net.WEBApiGet(dBConfigClass.Med_Update_ApiURL);
                    Console.WriteLine(Jsonstring);
                }
                Function_從SQL取得儲位到雲端資料();
                PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料.Bool = false;
            }

            //檢查取消作業-刪除母資料
            list_取藥堆疊母資料_取消作業 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.取消作業.GetEnumName());
            for (int i = 0; i < list_取藥堆疊母資料_取消作業.Count; i++)
            {
                GUID = list_取藥堆疊母資料_取消作業[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                this.list_取藥堆疊母資料.Remove(list_取藥堆疊母資料_取消作業[i]);
                Function_取藥堆疊資料_刪除母資料(GUID);
            }

            //檢查無效資料-刪除子資料
            for (int i = 0; i < this.list_取藥堆疊子資料.Count; i++)
            {
                GUID = this.list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                if (list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.GUID, GUID).Count == 0)
                {
                    list_取藥堆疊子資料_DeleteValue.Add(this.list_取藥堆疊子資料[i]);
                }

            }
            for (int i = 0; i < list_取藥堆疊子資料_DeleteValue.Count; i++)
            {
                Function_取藥堆疊資料_刪除子資料(list_取藥堆疊子資料_DeleteValue[i][(int)enum_取藥堆疊子資料.GUID].ObjectToString(), true);
            }
            this.list_取藥堆疊子資料 = this.Function_取藥堆疊資料_取得子資料();
            this.list_取藥堆疊子資料.Sort(new Icp_取藥堆疊子資料_index排序());
            this.list_取藥堆疊母資料.Sort(new Icp_取藥堆疊母資料_index排序());
            cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_刷新新增效期(ref int cnt)
        {
            List<object[]> _list_取藥堆疊母資料 = Function_取藥堆疊資料_取得母資料();
            if (_list_取藥堆疊母資料.Count > 0)
            {
                List<object[]> list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
                List<object[]> list_取藥堆疊母資料_buf = new List<object[]>();
                List<string> TYPE = new List<string>();
                List<object> values = new List<object>();
                string 藥品碼 = "";
                string 異動量 = "";
                string 效期 = "";
                string 批號 = "";
                string IP = "";
                list_取藥堆疊母資料_buf = _list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.新增效期.GetEnumName());
                for (int i = 0; i < list_取藥堆疊母資料_buf.Count; i++)
                {
                    藥品碼 = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    效期 = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.效期].ObjectToString();
                    批號 = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.批號].ObjectToString();
                    IP = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.IP].ObjectToString();
                    Function_從SQL取得儲位到雲端資料(藥品碼);
                    Function_從雲端資料取得儲位(藥品碼, ref TYPE, ref values);
                    for (int k = 0; k < values.Count; k++)
                    {
                        if (TYPE[k] == DeviceType.EPD266_lock.GetEnumName() || TYPE[k] == DeviceType.EPD266.GetEnumName()
                            || TYPE[k] == DeviceType.EPD290_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290.GetEnumName()
                            || TYPE[k] == DeviceType.EPD420_lock.GetEnumName() || TYPE[k] == DeviceType.EPD420.GetEnumName()
                            || TYPE[k] == DeviceType.EPD213_lock.GetEnumName() || TYPE[k] == DeviceType.EPD213.GetEnumName()
                            || TYPE[k] == DeviceType.EPD360E_lock.GetEnumName() || TYPE[k] == DeviceType.EPD360E.GetEnumName()
                            || TYPE[k] == DeviceType.EPD420G_lock.GetEnumName() || TYPE[k] == DeviceType.EPD420G.GetEnumName())
                        {

                            Storage storage = (Storage)values[k];
                            if (storage.取得庫存(效期) == -1)
                            {
                                if (!IP.StringIsEmpty())
                                {
                                    if (storage.IP != IP) continue;
                                }
                                storage.新增效期(效期, 批號, "00");
                                List_EPD266_雲端資料.Add_NewStorage(storage);
                                //this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                                break;
                            }

                        }
                        else if (TYPE[k] == DeviceType.Pannel35.GetEnumName() || TYPE[k] == DeviceType.Pannel35_lock.GetEnumName())
                        {

                            Storage storage = (Storage)values[k];
                            if (storage.取得庫存(效期) == -1)
                            {
                                if (!IP.StringIsEmpty())
                                {
                                    if (storage.IP != IP) continue;
                                }
                                storage.新增效期(效期, 批號, "00");
                                List_Pannel35_雲端資料.Add_NewStorage(storage);
                                //this.storageUI_WT32.SQL_ReplaceStorage(storage);
                                break;
                            }

                        }
                        else if (TYPE[k] == DeviceType.EPD583_lock.GetEnumName() || TYPE[k] == DeviceType.EPD583.GetEnumName()
                            || TYPE[k] == DeviceType.EPD420_D.GetEnumName() || TYPE[k] == DeviceType.EPD420_D_lock.GetEnumName()
                            || TYPE[k] == DeviceType.EPD730E.GetEnumName() || TYPE[k] == DeviceType.EPD730E_lock.GetEnumName())
                        {
                            Box box = (Box)values[k];
                            if (!IP.StringIsEmpty())
                            {
                                if (box.IP != IP) continue;
                            }
                            if (box.取得庫存(效期) == -1)
                            {
                                box.新增效期(效期, 批號, "00");
                                Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                                drawer.ReplaceBox(box);
                                List_EPD583_雲端資料.Add_NewDrawer(drawer);
                                //this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                                break;
                            }
                        }
                        else if (TYPE[k] == DeviceType.EPD1020_lock.GetEnumName() || TYPE[k] == DeviceType.EPD1020.GetEnumName())
                        {
                            Box box = (Box)values[k];
                            if (!IP.StringIsEmpty())
                            {
                                if (box.IP != IP) continue;
                            }
                            if (box.取得庫存(效期) == -1)
                            {
                                box.新增效期(效期, 批號, "00");
                                Drawer drawer = List_EPD1020_雲端資料.SortByIP(box.IP);
                                drawer.ReplaceByGUID(box);
                                List_EPD1020_雲端資料.Add_NewDrawer(drawer);
                                //this.drawerUI_EPD_1020.SQL_ReplaceDrawer(drawer);
                                break;
                            }
                        }
                        else if (TYPE[k] == DeviceType.RowsLED.GetEnumName())
                        {
                            RowsDevice rowsDevice = values[k] as RowsDevice;
                            if (!IP.StringIsEmpty())
                            {
                                if (rowsDevice.IP != IP) continue;
                            }
                            if (rowsDevice.取得庫存(效期) == -1)
                            {
                                rowsDevice.新增效期(效期, 批號, "00");
                                RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(rowsDevice.IP);
                                rowsLED.ReplaceRowsDevice(rowsDevice);
                                List_RowsLED_雲端資料.Add_NewRowsLED(rowsDevice);
                                //this.rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                                break;
                            }
                        }
                        else if (TYPE[k] == DeviceType.RFID_Device.GetEnumName())
                        {
                            RFIDDevice rFIDDevice = values[k] as RFIDDevice;
                            if (!IP.StringIsEmpty())
                            {
                                if (rFIDDevice.IP != IP) continue;
                            }
                            if (rFIDDevice.取得庫存(效期) == -1)
                            {
                                rFIDDevice.新增效期(效期, 批號, "00");
                                RFIDClass rFIDClass = List_RFID_雲端資料.SortByIP(rFIDDevice.IP);
                                rFIDClass.ReplaceRFIDDevice(rFIDDevice);
                                List_RFID_雲端資料.Add_NewRFIDClass(rFIDDevice);
                                //this.rfiD_UI.SQL_ReplaceRFIDClass(rFIDClass);
                                break;
                            }
                        }
                    }

                    list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName();
                    list_取藥堆疊母資料_ReplaceValue.Add(list_取藥堆疊母資料_buf[i]);
                }
                if (list_取藥堆疊母資料_ReplaceValue.Count > 0) this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_ReplaceValue, false);
            }
            cnt++;

        }
        void cnt_Program_取藥堆疊資料_檢查資料_從SQL讀取儲位資料(ref int cnt)
        {
            this.list_取藥堆疊母資料 = Function_取藥堆疊資料_取得母資料();
            this.list_取藥堆疊母資料 = (from temp in this.list_取藥堆疊母資料
                                 where temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != "新增資料"
                                 select temp).ToList();
            if (this.list_取藥堆疊母資料.Count > 0)
            {
                var Code_LINQ = (from value in list_取藥堆疊母資料
                                 select value[(int)enum_取藥堆疊母資料.藥品碼]).ToList().Distinct();
                List<object> list_code = Code_LINQ.ToList();
                for (int i = 0; i < list_code.Count; i++)
                {
                    Function_從SQL取得儲位到雲端資料(list_code[i].ObjectToString());
                }

                List<object[]> list_取藥堆疊母資料_buf = new List<object[]>();
                List<object[]> list_藥品設定表 = this.sqL_DataGridView_藥品設定表.SQL_GetAllRows(false);
                List<object[]> list_藥品管制方式設定 = this.sqL_DataGridView_藥品管制方式設定.SQL_GetAllRows(false);
                bool flag_獨立作業 = false;
                bool flag_雙人覆核 = false;
                bool flag_RFID使用 = false;
                bool flag_自動出藥 = false;
                string GUID = "";
                string 藥品碼 = "";
                string 調劑台名稱 = "";
                double 總異動量 = 0;
                double 庫存量 = 0;
                double 結存量 = 0;
                List<string> lock_ip = new List<string>();
                for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
                {
                    GUID = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                    藥品碼 = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    調劑台名稱 = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString();
                    總異動量 = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToDouble();
                    庫存量 = Function_從雲端資料取得庫存(藥品碼);
                    結存量 = (庫存量 + 總異動量);
                    flag_獨立作業 = Function_取藥堆疊資料_取得作業模式(this.list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.獨立作業);
                    flag_雙人覆核 = Function_取藥堆疊資料_取得作業模式(this.list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.雙人覆核);
                    flag_RFID使用 = Function_取藥堆疊資料_取得作業模式(this.list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.RFID使用);
                    flag_自動出藥 = Function_取藥堆疊資料_取得作業模式(this.list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.自動出藥);
                    if (庫存量 == -999)
                    {

                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()) continue;
                        this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.無儲位.GetEnumName();
                        Color color = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                        if (myConfigClass.系統取藥模式)
                        {
                            Function_取藥堆疊資料_新增子資料(GUID, "", 調劑台名稱, 藥品碼, "", "", "", "", "", "0");
                            Function_儲位亮燈(new Main_Form.LightOn(藥品碼, color));
                        }
                        this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(this.list_取藥堆疊母資料[i], false);
                        return;
                    }
                    if (結存量 < 0)
                    {
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName() || this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName()) continue;
                        this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName();
                        Function_取藥堆疊資料_設定作業模式(this.list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.庫存不足語音提示);
                        this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(this.list_取藥堆疊母資料[i], false);
                        return;
                    }
                    if (flag_獨立作業)
                    {
                        if (flag_雙人覆核)
                        {
                            if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName())
                            {
                                this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.雙人覆核.GetEnumName();
                                this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(this.list_取藥堆疊母資料[i], false);
                                return;
                            }
                        }
                        if (flag_RFID使用)
                        {
                            if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName())
                            {
                                this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.RFID使用.GetEnumName();
                                Console.WriteLine("更改取藥堆疊母資料 狀態 ---->>【RFID使用】");
                                this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(this.list_取藥堆疊母資料[i], false);
                                return;
                            }
                        }

                        bool flag_單循環取藥 = false;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.RFID使用.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.自動出藥.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.雙人覆核.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待作業.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待複盤.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.盲盤完成.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.複盤完成.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.作業完成.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.選擇效期.GetEnumName()) flag_單循環取藥 = true;
                        if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.輸入新效期.GetEnumName()) flag_單循環取藥 = true;
                        if (flag_單循環取藥)
                        {
                            list_取藥堆疊母資料_buf.Add(list_取藥堆疊母資料[i]);
                            this.list_取藥堆疊母資料 = list_取藥堆疊母資料_buf;
                            cnt++;
                            return;
                        }
                    }
                }
            }
            cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_刷新無庫存(ref int cnt)
        {
            if (this.list_取藥堆疊母資料.Count > 0)
            {
                if (!plC_CheckBox_無庫存自動補足.Bool)
                {
                    cnt++;
                    return;
                }
                List<object[]> list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
                List<object[]> list_取藥堆疊母資料_buf = new List<object[]>();
                List<string> TYPE = new List<string>();
                List<object> values = new List<object>();
                string 藥品碼 = "";
                string 異動量 = "";
                string 效期 = "";
                string 批號 = "";
                string IP = "";
                list_取藥堆疊母資料_buf = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName());
                for (int i = 0; i < list_取藥堆疊母資料_buf.Count; i++)
                {
                    藥品碼 = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    效期 = "2200/01/01";
                    批號 = "自動補足";
                    IP = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.IP].ObjectToString();
                    Function_從SQL取得儲位到雲端資料(藥品碼);

                    Function_從雲端資料取得儲位(藥品碼, ref TYPE, ref values);
                    for (int k = 0; k < values.Count; k++)
                    {
                        if (TYPE[k] == DeviceType.EPD266_lock.GetEnumName() || TYPE[k] == DeviceType.EPD266.GetEnumName()
                            || TYPE[k] == DeviceType.EPD290_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290.GetEnumName()
                             || TYPE[k] == DeviceType.EPD290G_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290G.GetEnumName()
                            || TYPE[k] == DeviceType.EPD420_lock.GetEnumName() || TYPE[k] == DeviceType.EPD420.GetEnumName()
                            || TYPE[k] == DeviceType.EPD213_lock.GetEnumName() || TYPE[k] == DeviceType.EPD213.GetEnumName()
                            || TYPE[k] == DeviceType.EPD420G_lock.GetEnumName() || TYPE[k] == DeviceType.EPD420G.GetEnumName()
                            || TYPE[k] == DeviceType.EPD360E_lock.GetEnumName() || TYPE[k] == DeviceType.EPD360E.GetEnumName())
                        {
                            if (values[k] is Storage)
                            {
                                Storage storage = (Storage)values[k];
                                if (!IP.StringIsEmpty())
                                {
                                    if (storage.IP != IP) continue;
                                }
                                storage.新增效期(效期, 批號, "100000");
                                List_EPD266_雲端資料.Add_NewStorage(storage);
                                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                                break;
                            }
                            if (values[k] is Box)
                            {
                                Box box = (Box)values[k];
                                if (!IP.StringIsEmpty())
                                {
                                    if (box.IP != IP) continue;
                                }
                                box.新增效期(效期, 批號, "100000");
                                Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                                drawer.ReplaceBox(box);
                                List_EPD583_雲端資料.Add_NewDrawer(drawer);
                                this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                                break;
                            }
                        }
                        else if (TYPE[k] == DeviceType.Pannel35.GetEnumName() || TYPE[k] == DeviceType.Pannel35_lock.GetEnumName())
                        {

                            Storage storage = (Storage)values[k];
                            if (!IP.StringIsEmpty())
                            {
                                if (storage.IP != IP) continue;
                            }
                            storage.新增效期(效期, 批號, "100000");
                            List_Pannel35_雲端資料.Add_NewStorage(storage);
                            this.storageUI_WT32.SQL_ReplaceStorage(storage);
                            break;

                        }
                        else if (TYPE[k] == DeviceType.EPD583_lock.GetEnumName() || TYPE[k] == DeviceType.EPD583.GetEnumName()
                            || TYPE[k] == DeviceType.EPD420_D.GetEnumName() || TYPE[k] == DeviceType.EPD420_D_lock.GetEnumName()
                            || TYPE[k] == DeviceType.EPD730E.GetEnumName() || TYPE[k] == DeviceType.EPD730E_lock.GetEnumName())
                        {

                            Box box = (Box)values[k];
                            if (!IP.StringIsEmpty())
                            {
                                if (box.IP != IP) continue;
                            }
                            box.新增效期(效期, 批號, "100000");
                            Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                            drawer.ReplaceBox(box);
                            List_EPD583_雲端資料.Add_NewDrawer(drawer);
                            this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                            break;

                        }
                        else if (TYPE[k] == DeviceType.EPD1020_lock.GetEnumName() || TYPE[k] == DeviceType.EPD1020.GetEnumName())
                        {

                            Box box = (Box)values[k];
                            if (!IP.StringIsEmpty())
                            {
                                if (box.IP != IP) continue;
                            }
                            box.新增效期(效期, 批號, "100000");
                            Drawer drawer = List_EPD1020_雲端資料.SortByIP(box.IP);
                            drawer.ReplaceByGUID(box);
                            List_EPD1020_雲端資料.Add_NewDrawer(drawer);
                            this.drawerUI_EPD_1020.SQL_ReplaceDrawer(drawer);
                            break;

                        }
                        else if (TYPE[k] == DeviceType.RowsLED.GetEnumName())
                        {
                            RowsDevice rowsDevice = values[k] as RowsDevice;
                            if (!IP.StringIsEmpty())
                            {
                                if (rowsDevice.IP != IP) continue;
                            }
                            rowsDevice.新增效期(效期, 批號, "100000");
                            RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(rowsDevice.IP);
                            rowsLED.ReplaceRowsDevice(rowsDevice);
                            List_RowsLED_雲端資料.Add_NewRowsLED(rowsDevice);
                            this.rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                            break;
                        }
                        else if (TYPE[k] == DeviceType.RFID_Device.GetEnumName())
                        {
                            RFIDDevice rFIDDevice = values[k] as RFIDDevice;
                            if (!IP.StringIsEmpty())
                            {
                                if (rFIDDevice.IP != IP) continue;
                            }
                            rFIDDevice.新增效期(效期, 批號, "100000");
                            RFIDClass rFIDClass = List_RFID_雲端資料.SortByIP(rFIDDevice.IP);
                            rFIDClass.ReplaceRFIDDevice(rFIDDevice);
                            List_RFID_雲端資料.Add_NewRFIDClass(rFIDDevice);
                            this.rfiD_UI.SQL_ReplaceRFIDClass(rFIDClass);
                            break;
                        }
                    }

                    list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName();
                    list_取藥堆疊母資料_ReplaceValue.Add(list_取藥堆疊母資料_buf[i]);
                }
                if (list_取藥堆疊母資料_ReplaceValue.Count > 0) this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_ReplaceValue, false);
            }
            cnt++;

        }
        void cnt_Program_取藥堆疊資料_檢查資料_刷新資料(ref int cnt)
        {
            if (this.list_取藥堆疊母資料.Count > 0)
            {

                //list_取藥堆疊子資料 = sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
                string 藥品碼 = "";
                string 調劑台名稱 = "";
                string GUID = "";
                string 效期 = "";
                string 批號 = "";
                string IP = "";
                double 總異動量 = 0;
                double 庫存量 = 0;
                double 結存量 = 0;
                bool flag_取藥堆疊母資料_Update = false;
                List<object[]> list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
                List<object[]> list_取藥堆疊母資料_DeleteValue = new List<object[]>();
                List<object[]> list_取藥堆疊子資料_buf = new List<object[]>();
                List<object[]> list_取藥堆疊子資料_DeleteValue = new List<object[]>();
                List<object[]> list_取藥堆疊子資料_ReplaceValue = new List<object[]>();


                this.list_取藥堆疊母資料 = (from value in this.list_取藥堆疊母資料
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.None.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.新增資料.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.新增效期.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.選擇效期.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.雙人覆核.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.已領用過.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.DC處方.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.未授權.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()
                                     where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.無可匹配數量.GetEnumName()
                                     select value).ToList();

                for (int i = 0; i < this.list_取藥堆疊母資料.Count; i++)
                {
                    flag_取藥堆疊母資料_Update = false;
                    GUID = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                    調劑台名稱 = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString();
                    藥品碼 = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    總異動量 = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToDouble();
                    庫存量 = Function_從雲端資料取得庫存(藥品碼);
                    結存量 = (庫存量 + 總異動量);
                    效期 = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.效期].ObjectToString();
                    批號 = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.批號].ObjectToString();
                    IP = this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.IP].ObjectToString();
                    if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.庫存量].StringToDouble() != 庫存量)
                    {
                        this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.庫存量] = 庫存量.ToString();
                        flag_取藥堆疊母資料_Update = true;
                    }
                    if (this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.結存量].StringToDouble() != 結存量)
                    {
                        this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.結存量] = 結存量.ToString();
                        flag_取藥堆疊母資料_Update = true;
                    }


                    //找無儲位
                    if (庫存量 == -999)
                    {
                        this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.無儲位.GetEnumName();
                        flag_取藥堆疊母資料_Update = true;
                    }
                    //無庫存
                    else if (結存量 < 0)
                    {
                        this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName();
                        if (!plC_CheckBox_無庫存自動補足.Bool) Function_儲位亮燈_Ex(new LightOn(藥品碼, Color.Black, 0));
                        // Function_取藥堆疊資料_設定作業模式(this.list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.庫存不足語音提示);
                        flag_取藥堆疊母資料_Update = true;
                    }
                    //更新取藥子堆疊資料
                    else if (總異動量 == 0 || 庫存量 >= 0)
                    {
                        if (Function_取藥堆疊資料_取得作業模式(this.list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.效期管控) && 效期.StringIsEmpty())
                        {
                            this.list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.選擇效期.GetEnumName();
                            flag_取藥堆疊母資料_Update = true;
                        }
                        else
                        {
                            List<object[]> 儲位資訊 = new List<object[]>();
                            string 儲位資訊_TYPE = "";
                            string 儲位資訊_IP = "";
                            string 儲位資訊_Num = "";
                            string 儲位資訊_效期 = "";
                            string 儲位資訊_批號 = "";

                            string 儲位資訊_庫存 = "";
                            string 儲位資訊_異動量 = "";
                            string 儲位資訊_GUID = "";
                            list_取藥堆疊子資料_buf = sqL_DataGridView_取藥堆疊子資料.SQL_GetRows((int)enum_取藥堆疊子資料.Master_GUID, GUID, false);


                            if (效期.StringIsEmpty())
                            {
                              
                                if (IP.StringIsEmpty())
                                {
                                    儲位資訊 = Function_取得異動儲位資訊從雲端資料(藥品碼, 總異動量);
                                }
                                else
                                {
                                    儲位資訊 = Function_取得異動儲位資訊從雲端資料(藥品碼, 總異動量, 效期, IP);
                                }
                            }
                            else
                            {
                                if (IP.StringIsEmpty())
                                {
                                    儲位資訊 = Function_取得異動儲位資訊從雲端資料(藥品碼, 總異動量, 效期);
                                }
                                else
                                {
                                    儲位資訊 = Function_取得異動儲位資訊從雲端資料(藥品碼, 總異動量, 效期, IP);
                                }
                            }


                            if (儲位資訊.Count == 0 && 結存量 > 0)
                            {
                                List<string> list_效期 = new List<string>();
                                List<string> list_批號 = new List<string>();
                                if (效期.StringIsEmpty())
                                {
                                    Funnction_交易記錄查詢_取得指定藥碼批號期效期(藥品碼, ref list_效期, ref list_批號);
                                    if (list_效期.Count > 0 && list_批號.Count > 0)
                                    {
                                        效期 = list_效期[0];
                                        批號 = list_批號[0];
                                    }
                                }

                                儲位資訊 = Function_新增效期至雲端資料(藥品碼, 總異動量, 效期, 批號, IP);
                            }

                            List<object[]> list_sortValue = new List<object[]>();
                            //檢查子資料新增及修改
                            for (int m = 0; m < list_取藥堆疊子資料_buf.Count; m++)
                            {
                                bool flag_Delete = true;
                                for (int k = 0; k < 儲位資訊.Count; k++)
                                {
                                    this.Function_取藥堆疊資料_取得儲位資訊內容(儲位資訊[k], ref 儲位資訊_GUID, ref 儲位資訊_TYPE, ref 儲位資訊_IP, ref 儲位資訊_Num, ref 儲位資訊_效期, ref 儲位資訊_批號, ref 儲位資訊_庫存, ref 儲位資訊_異動量);
                                    if (list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == 儲位資訊_TYPE)
                                        if (list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.IP].ObjectToString() == 儲位資訊_IP)
                                            if (list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.Num].ObjectToString() == 儲位資訊_Num)
                                                if (list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.效期].ObjectToString() == 儲位資訊_效期)
                                                {
                                                    flag_Delete = false;
                                                    break;
                                                }
                                }
                                if (flag_Delete)
                                {
                                    Function_取藥堆疊資料_刪除子資料(list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.GUID].ObjectToString(), false);
                                }
                            }
                            for (int k = 0; k < 儲位資訊.Count; k++)
                            {

                                this.Function_取藥堆疊資料_取得儲位資訊內容(儲位資訊[k], ref 儲位資訊_GUID, ref 儲位資訊_TYPE, ref 儲位資訊_IP, ref 儲位資訊_Num, ref 儲位資訊_效期, ref 儲位資訊_批號, ref 儲位資訊_庫存, ref 儲位資訊_異動量);

                                list_sortValue = (from value in list_取藥堆疊子資料_buf
                                                  where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == 儲位資訊_TYPE
                                                  where value[(int)enum_取藥堆疊子資料.IP].ObjectToString() == 儲位資訊_IP
                                                  where value[(int)enum_取藥堆疊子資料.Num].ObjectToString() == 儲位資訊_Num
                                                  where value[(int)enum_取藥堆疊子資料.效期].ObjectToString() == 儲位資訊_效期
                                                  select value).ToList();
                                if (list_sortValue.Count != 1)
                                {
                                    for (int m = 0; m < list_sortValue.Count; m++)
                                    {
                                        Function_取藥堆疊資料_刪除子資料(list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.GUID].ObjectToString(), false);
                                    }
                                    object[] value = this.Function_取藥堆疊資料_新增子資料(GUID, 儲位資訊_GUID, 調劑台名稱, 藥品碼, 儲位資訊_IP, 儲位資訊_Num, 儲位資訊_TYPE, 儲位資訊_效期, 儲位資訊_批號, 儲位資訊_異動量);

                                    list_取藥堆疊子資料_buf.Add(value);
                                    Function_庫存異動至雲端資料(儲位資訊[k]);
                                }
                                else
                                {
                                    if (list_sortValue[0][(int)enum_取藥堆疊子資料.異動量].ObjectToString() != 儲位資訊_異動量)
                                    {
                                        list_sortValue[0][(int)enum_取藥堆疊子資料.異動量] = 儲位資訊_異動量;
                                        list_取藥堆疊子資料_ReplaceValue.Add(list_sortValue[0]);
                                    }
                                    if (list_sortValue[0][(int)enum_取藥堆疊子資料.已入帳].ObjectToString() == false.ToString())
                                    {
                                        Function_庫存異動至雲端資料(儲位資訊[k]);
                                    }
                                }
                            }
                        }



                    }



                    if (flag_取藥堆疊母資料_Update)
                    {
                        list_取藥堆疊母資料_ReplaceValue.Add(this.list_取藥堆疊母資料[i]);
                    }
                    else
                    {
                        this.Function_取藥堆疊資料_檢查資料儲位正常(this.list_取藥堆疊母資料[i]);
                    }
                }

                if (list_取藥堆疊母資料_DeleteValue.Count > 0)
                {
                    this.sqL_DataGridView_取藥堆疊母資料.SQL_DeleteExtra(list_取藥堆疊母資料_DeleteValue, false);
                }
                if (list_取藥堆疊母資料_ReplaceValue.Count > 0)
                {
                    this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_ReplaceValue, false);
                }
                if (list_取藥堆疊子資料_ReplaceValue.Count > 0) this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥堆疊子資料_ReplaceValue, false);

            }
            cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_設定致能(ref int cnt)
        {
            this.list_取藥堆疊子資料 = this.sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
            List<object[]> list_取藥堆疊子資料 = this.Function_取藥堆疊子資料_取得可致能(ref this.list_取藥堆疊子資料);
            List<object[]> list_取藥堆疊母資料_buf;
            List<object[]> list_取藥堆疊資料_ReplaceValue = new List<object[]>();
            List<object[]> list_locker_table_value = this.sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(false);
            List<object[]> list_locker_table_value_buf = new List<object[]>();
            List<object[]> list_locker_table_value_ReplaceValue = new List<object[]>();
            List<string> list_lock_IP = new List<string>();
            string IP = "";
            string Slave_GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            string Num = "";
            string 藥品碼 = "";
            string Type = "";
            double 數量 = 0;
            Color color = Color.Black;

            List<string> list_已亮燈藥碼 = new List<string>();
            List<string> list_已亮燈藥碼_buf = new List<string>();

            list_取藥堆疊子資料 = list_取藥堆疊子資料.GetRows((int)enum_取藥堆疊子資料.致能, false.ToString());

            foreach (object[] 取藥堆疊資料 in list_取藥堆疊子資料)
            {
                Function_取藥堆疊資料_語音提示(取藥堆疊資料);
                IP = 取藥堆疊資料[(int)enum_取藥堆疊子資料.IP].ObjectToString();
                Master_GUID = 取藥堆疊資料[(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                Slave_GUID = 取藥堆疊資料[(int)enum_取藥堆疊子資料.GUID].ObjectToString();
                Device_GUID = 取藥堆疊資料[(int)enum_取藥堆疊子資料.Device_GUID].ObjectToString();
                藥品碼 = 取藥堆疊資料[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                Type = 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString();

                list_取藥堆疊母資料_buf = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);
                if (list_取藥堆疊母資料_buf.Count > 0)
                {
                    if (list_取藥堆疊母資料_buf[0][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == "自動出藥") continue;
                    color = list_取藥堆疊母資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                    數量 = list_取藥堆疊母資料_buf[0][(int)enum_取藥堆疊母資料.總異動量].StringToDouble();
                    if (數量 < 0) 數量 = 數量 * -1;
                }
                取藥堆疊資料[(int)enum_取藥堆疊子資料.致能] = true.ToString();
                list_取藥堆疊資料_ReplaceValue.Add(取藥堆疊資料);

                list_已亮燈藥碼_buf = (from temp in list_已亮燈藥碼
                                  where temp == 藥品碼
                                  select temp).ToList();
                if (list_已亮燈藥碼_buf.Count != 0) continue;

                if (Type.Contains("EPD583") || Type.Contains("EPD730")) Function_儲位亮燈(new Main_Form.LightOn(藥品碼, color), ref list_lock_IP);
                else if (Type == DeviceType.RFID_Device.GetEnumName())
                {
                    RFIDClass rFIDClass = List_RFID_雲端資料.SortByIP(IP);
                    RFIDDevice rFIDDevice = rFIDClass.SortByGUID(Device_GUID);
                    Num = rFIDDevice.MasterIndex.ToString();
                    list_locker_table_value_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.IP, IP);
                    list_locker_table_value_buf = list_locker_table_value_buf.GetRows((int)enum_lockerIndex.Num, Num.ToString());
                    if (list_locker_table_value_buf.Count > 0)
                    {
                        list_locker_table_value_buf[0][(int)enum_lockerIndex.Master_GUID] = Master_GUID;
                        list_locker_table_value_buf[0][(int)enum_lockerIndex.Device_GUID] = Device_GUID;
                        list_locker_table_value_buf[0][(int)enum_lockerIndex.Slave_GUID] = Slave_GUID;
                        list_locker_table_value_buf[0][(int)enum_lockerIndex.輸出狀態] = true.ToString();
                        list_locker_table_value_ReplaceValue.Add(list_locker_table_value_buf[0]);
                        Console.WriteLine($"開啟RFID抽屜致能,IP:{IP} {DateTime.Now.ToDateTimeString()}");
                    }
                }
                else if (Type == DeviceType.EPD360E_lock.GetEnumName())
                {
                    Main_Form.LightOn lightOn = new LightOn(藥品碼, color, 數量);
                    lightOn.IP = IP;
                    Function_儲位亮燈(lightOn, ref list_lock_IP);
                }
                else Function_儲位亮燈(new Main_Form.LightOn(藥品碼, color, 數量), ref list_lock_IP);
                list_已亮燈藥碼.Add(藥品碼);


            }

            list_lock_IP = (from temp in list_lock_IP
                            select temp).Distinct().ToList();
            for (int k = 0; k < list_lock_IP.Count; k++)
            {
                list_locker_table_value_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.IP, list_lock_IP[k]);
                if (list_locker_table_value_buf.Count > 0)
                {
                    list_locker_table_value_buf[0][(int)enum_lockerIndex.Master_GUID] = Master_GUID;
                    list_locker_table_value_buf[0][(int)enum_lockerIndex.Device_GUID] = Device_GUID;
                    list_locker_table_value_buf[0][(int)enum_lockerIndex.Slave_GUID] = Slave_GUID;
                    list_locker_table_value_buf[0][(int)enum_lockerIndex.輸出狀態] = true.ToString();
                    list_locker_table_value_ReplaceValue.Add(list_locker_table_value_buf[0]);

                    Console.WriteLine($"開啟抽屜致能,IP:{list_lock_IP[k]} {DateTime.Now.ToDateTimeString()}");

                }
            }
            if (list_locker_table_value_ReplaceValue.Count > 0) this.sqL_DataGridView_Locker_Index_Table.SQL_ReplaceExtra(list_locker_table_value_ReplaceValue, false);
            if (list_取藥堆疊資料_ReplaceValue.Count > 0) this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥堆疊資料_ReplaceValue, false);

            this.MyTimer_取藥堆疊資料_刷新時間.TickStop();
            this.MyTimer_取藥堆疊資料_刷新時間.StartTickTime();
            cnt++;
        }
        void cnt_Program_取藥堆疊資料_檢查資料_等待刷新時間到達(ref int cnt)
        {
            if (this.MyTimer_取藥堆疊資料_刷新時間.IsTimeOut())
            {
                cnt++;
            }
        }
        #endregion
        #region PLC_取藥堆疊資料_狀態更新
        PLC_Device PLC_Device_取藥堆疊資料_狀態更新 = new PLC_Device("");
        PLC_Device PLC_Device_取藥堆疊資料_狀態更新_OK = new PLC_Device("");
        int cnt_Program_取藥堆疊資料_狀態更新 = 65534;
        void sub_Program_取藥堆疊資料_狀態更新()
        {
            PLC_Device_取藥堆疊資料_狀態更新.Bool = PLC_Device_主機扣賬模式.Bool;
            if (cnt_Program_取藥堆疊資料_狀態更新 == 65534)
            {
                PLC_Device_取藥堆疊資料_狀態更新.SetComment("PLC_取藥堆疊資料_狀態更新");
                PLC_Device_取藥堆疊資料_狀態更新_OK.SetComment("PLC_取藥堆疊資料_狀態更新_OK");
                PLC_Device_取藥堆疊資料_狀態更新.Bool = false;
                cnt_Program_取藥堆疊資料_狀態更新 = 65535;
            }
            if (cnt_Program_取藥堆疊資料_狀態更新 == 65535) cnt_Program_取藥堆疊資料_狀態更新 = 1;
            if (cnt_Program_取藥堆疊資料_狀態更新 == 1) cnt_Program_取藥堆疊資料_狀態更新_檢查按下(ref cnt_Program_取藥堆疊資料_狀態更新);
            if (cnt_Program_取藥堆疊資料_狀態更新 == 2) cnt_Program_取藥堆疊資料_狀態更新_初始化(ref cnt_Program_取藥堆疊資料_狀態更新);
            if (cnt_Program_取藥堆疊資料_狀態更新 == 3) cnt_Program_取藥堆疊資料_狀態更新 = 65500;
            if (cnt_Program_取藥堆疊資料_狀態更新 > 1) cnt_Program_取藥堆疊資料_狀態更新_檢查放開(ref cnt_Program_取藥堆疊資料_狀態更新);

            if (cnt_Program_取藥堆疊資料_狀態更新 == 65500)
            {
                PLC_Device_取藥堆疊資料_狀態更新.Bool = false;
                PLC_Device_取藥堆疊資料_狀態更新_OK.Bool = false;
                cnt_Program_取藥堆疊資料_狀態更新 = 65535;
            }
        }
        void cnt_Program_取藥堆疊資料_狀態更新_檢查按下(ref int cnt)
        {
            if (PLC_Device_取藥堆疊資料_狀態更新.Bool) cnt++;
        }
        void cnt_Program_取藥堆疊資料_狀態更新_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取藥堆疊資料_狀態更新.Bool) cnt = 65500;
        }
        void cnt_Program_取藥堆疊資料_狀態更新_初始化(ref int cnt)
        {
            string 狀態 = "";
            string 狀態_buf = "";
            string GUID = "";
            bool 致能 = true;
            bool 流程作業完成 = true;
            bool 配藥完成 = true;
            bool 調劑結束 = true;
            bool 已入帳 = true;
            List<object[]> _list_取藥堆疊母資料 = Function_取藥堆疊資料_取得母資料();
            List<object[]> _list_取藥堆疊子資料 = this.Function_取藥堆疊資料_取得子資料();
            List<object[]> _list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
            List<object[]> _list_取藥堆疊子資料_ReplaceValue = new List<object[]>();
            List<object[]> _list_取藥堆疊子資料_buf = new List<object[]>();


            _list_取藥堆疊母資料 = (from value in _list_取藥堆疊母資料
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName()
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName()
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.新增效期.GetEnumName()
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName()
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.無可匹配數量.GetEnumName()
                             select value).ToList();

            _list_取藥堆疊母資料.Sort(new Icp_取藥堆疊母資料_index排序());
            for (int i = 0; i < _list_取藥堆疊母資料.Count; i++)
            {
                致能 = true;
                流程作業完成 = true;
                配藥完成 = true;
                調劑結束 = true;
                已入帳 = true;
                狀態_buf = 狀態 = _list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString();
                GUID = _list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                _list_取藥堆疊子資料_buf = _list_取藥堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, GUID);

                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能].ObjectToString() == false.ToString())
                    {
                        致能 = false;
                        break;
                    }
                }
                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == false.ToString())
                    {
                        流程作業完成 = false;
                        break;
                    }
                }
                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString())
                    {
                        配藥完成 = false;
                        break;
                    }
                }
                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.調劑結束].ObjectToString() == false.ToString())
                    {
                        調劑結束 = false;
                        break;
                    }
                }
                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.已入帳].ObjectToString() == false.ToString())
                    {
                        已入帳 = false;
                        break;
                    }
                }
                if (_list_取藥堆疊子資料_buf.Count > 0)
                {
                    if (已入帳) 狀態_buf = enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName();
                    else if (調劑結束) 狀態_buf = enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName();
                    else if (配藥完成) 狀態_buf = enum_取藥堆疊母資料_狀態.作業完成.GetEnumName();
                    else if (致能)
                    {
                        if (Function_取藥堆疊資料_取得作業模式(_list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.複盤)) 狀態_buf = enum_取藥堆疊母資料_狀態.等待複盤.GetEnumName();
                        else if (Function_取藥堆疊資料_取得作業模式(_list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.盲盤)) 狀態_buf = enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName();
                        else if (Function_取藥堆疊資料_取得作業模式(_list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.RFID使用)) 狀態_buf = enum_取藥堆疊母資料_狀態.RFID使用.GetEnumName();
                        else if (Function_取藥堆疊資料_取得作業模式(_list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.自動出藥))
                        {
                            狀態_buf = enum_取藥堆疊母資料_狀態.自動出藥.GetEnumName();
                        }
                        else
                        {
                            狀態_buf = enum_取藥堆疊母資料_狀態.等待作業.GetEnumName();
                        }
                    }
                    if (_list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.動作].ObjectToString().Contains("系統"))

                    {
                        if (狀態_buf == enum_取藥堆疊母資料_狀態.作業完成.GetEnumName())
                        {
                            狀態_buf = new enum_取藥堆疊母資料_狀態().GetEnumName();
                            for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                            {
                                _list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                                _list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                _list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                                _list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.調劑結束] = true.ToString();
                                _list_取藥堆疊子資料_ReplaceValue.Add(_list_取藥堆疊子資料_buf[k]);
                            }
                        }
                    }
                }

                if (狀態_buf != 狀態)
                {
                    Console.WriteLine($"狀態從[{狀態}] >>>>> [{狀態_buf}]");

                    狀態 = 狀態_buf;
                    _list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = 狀態;
                    _list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.操作時間] = DateTime.Now.ToDateTimeString_6();
                    _list_取藥堆疊母資料_ReplaceValue.Add(_list_取藥堆疊母資料[i]);
                }



            }
            if (_list_取藥堆疊母資料_ReplaceValue.Count > 0)
            {
                this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(_list_取藥堆疊母資料_ReplaceValue, false);
            }
            if (_list_取藥堆疊子資料_ReplaceValue.Count > 0)
            {
                this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(_list_取藥堆疊子資料_ReplaceValue, false);
            }
            cnt++;
        }


        #endregion
        #region PLC_取藥堆疊資料_流程作業檢查
        List<object[]> list_流程作業檢查_取藥母堆疊資料 = new List<object[]>();
        List<object[]> list_流程作業檢查_取藥子堆疊資料 = new List<object[]>();
        PLC_Device PLC_Device_取藥堆疊資料_流程作業檢查 = new PLC_Device("");
        PLC_Device PLC_Device_取藥堆疊資料_流程作業檢查_不檢測 = new PLC_Device("S5246");
        public int 取藥堆疊資料_流程作業檢查_感測設定值 = 100;
        MyTimer MyTimer_取藥堆疊資料_流程作業檢查 = new MyTimer("K1");
        MyTimer MyTimer_取藥堆疊資料_流程作業檢查時間 = new MyTimer();
        int cnt_Program_取藥堆疊資料_流程作業檢查 = 65534;
        void sub_Program_取藥堆疊資料_流程作業檢查()
        {
            PLC_Device_取藥堆疊資料_流程作業檢查.Bool = PLC_Device_主機扣賬模式.Bool;
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 65534)
            {
                PLC_Device_取藥堆疊資料_流程作業檢查.SetComment("PLC_取藥堆疊資料_流程作業檢查");
                PLC_Device_取藥堆疊資料_流程作業檢查.Bool = false;
                cnt_Program_取藥堆疊資料_流程作業檢查 = 65535;
            }
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 65535) cnt_Program_取藥堆疊資料_流程作業檢查 = 1;
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 1) cnt_Program_取藥堆疊資料_流程作業檢查_檢查按下(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 2) cnt_Program_取藥堆疊資料_流程作業檢查_初始化(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 3) cnt_Program_取藥堆疊資料_流程作業檢查_檢查盲盤複盤(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 4) cnt_Program_取藥堆疊資料_流程作業檢查_檢查同藥碼全亮(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 5) cnt_Program_取藥堆疊資料_流程作業檢查_檢查層架手勢感測感應到(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 6) cnt_Program_取藥堆疊資料_流程作業檢查_檢查層架手勢感測離開(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 7) cnt_Program_取藥堆疊資料_流程作業檢查_檢查抽屜手勢感測感應到(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 8) cnt_Program_取藥堆疊資料_流程作業檢查 = 65500;
            if (cnt_Program_取藥堆疊資料_流程作業檢查 > 1) cnt_Program_取藥堆疊資料_流程作業檢查_檢查放開(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 65500)
            {
                this.MyTimer_取藥堆疊資料_流程作業檢查.TickStop();
                this.MyTimer_取藥堆疊資料_流程作業檢查.StartTickTime();
                cnt_Program_取藥堆疊資料_流程作業檢查 = 65501;
            }
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 65501)
            {
                if (this.MyTimer_取藥堆疊資料_流程作業檢查.IsTimeOut())
                {
                    PLC_Device_取藥堆疊資料_流程作業檢查.Bool = false;
                    cnt_Program_取藥堆疊資料_流程作業檢查 = 65535;
                }
            }
        }
        void cnt_Program_取藥堆疊資料_流程作業檢查_檢查按下(ref int cnt)
        {
            if (PLC_Device_取藥堆疊資料_流程作業檢查.Bool) cnt++;
        }
        void cnt_Program_取藥堆疊資料_流程作業檢查_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取藥堆疊資料_流程作業檢查.Bool) cnt = 65500;
        }
        void cnt_Program_取藥堆疊資料_流程作業檢查_初始化(ref int cnt)
        {




            cnt++;
        }
        void cnt_Program_取藥堆疊資料_流程作業檢查_檢查盲盤複盤(ref int cnt)
        {
            string IP = "";
            string 藥品碼 = "";
            string 調劑台名稱 = "";
            string GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            bool flag_TOFON = false;
            Color color = Color.Black;
            list_流程作業檢查_取藥母堆疊資料 = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
            list_流程作業檢查_取藥子堆疊資料 = this.sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);

            List<object[]> list_取藥母堆疊資料 = list_流程作業檢查_取藥母堆疊資料;
            List<object[]> list_取藥母堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料 = list_流程作業檢查_取藥子堆疊資料;
            List<object[]> list_取藥子堆疊資料_buf = new List<object[]>();

            list_取藥子堆疊資料_buf = (from value in list_取藥子堆疊資料
                                where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                select value).ToList();

            for (int i = 0; i < list_取藥子堆疊資料_buf.Count; i++)
            {
                GUID = list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.GUID].ObjectToString();
                Master_GUID = list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();

                List<object[]> _list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);

                if (_list_取藥母堆疊資料_buf.Count > 0)
                {
                    bool flag_remove = false;
                    if (_list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待複盤.GetEnumName()) flag_remove = true;
                    if (_list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName()) flag_remove = true;
                    if (_list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.RFID使用.GetEnumName()) flag_remove = true;
                    if (_list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.自動出藥.GetEnumName()) flag_remove = true;
                    if (flag_remove)
                    {
                        list_取藥母堆疊資料.RemoveByGUID(_list_取藥母堆疊資料_buf[0]);
                        list_取藥子堆疊資料.RemoveByGUID(GUID);
                    }

                }
            }
            cnt++;
        }
        void cnt_Program_取藥堆疊資料_流程作業檢查_檢查同藥碼全亮(ref int cnt)
        {
            string IP = "";
            string 藥品碼 = "";
            string 調劑台名稱 = "";
            string GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            bool flag_TOFON = false;
            Color color = Color.Black;

            List<object[]> list_取藥子堆疊資料 = list_流程作業檢查_取藥子堆疊資料;
            List<object[]> list_取藥子堆疊資料_buf = new List<object[]>();

            list_取藥子堆疊資料_buf = (from value in list_取藥子堆疊資料
                                where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                where value[(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == false.ToString()
                                where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                select value).ToList();

            List<object[]> list_取藥子堆疊資料_Replace = new List<object[]>();
            for (int i = 0; i < list_取藥子堆疊資料_buf.Count; i++)
            {
                藥品碼 = list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                Master_GUID = list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                IP = list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.IP].ObjectToString();
                RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(IP);
                if (rowsLED != null)
                {
                    list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    list_取藥子堆疊資料_Replace.Add(list_取藥子堆疊資料_buf[i]);
                    continue;
                }

                if (plC_CheckBox_同藥品全部亮燈.Bool)
                {
                    string index_IP = Funcion_取得LCD114索引表_index_IP(IP);
                    Storage storage = List_EPD266_雲端資料.SortByIP(IP);
                    if (storage == null) storage = List_Pannel35_雲端資料.SortByIP(IP);
                    if (storage != null && index_IP.StringIsEmpty())
                    {

                        list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                        /*if (storage.IsFADC == false)*/
                        list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                        list_取藥子堆疊資料_Replace.Add(list_取藥子堆疊資料_buf[i]);
                    }
                    else if (storage != null && (storage.DeviceIsStorage()))
                    {
                        if (!storage.TOFON)
                        {
                            list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                            /*if (storage.IsFADC == false)*/
                            list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                            list_取藥子堆疊資料_Replace.Add(list_取藥子堆疊資料_buf[i]);
                        }
                        else
                        {
                            flag_TOFON = true;
                        }
                    }
                }
           


            }
            if (list_取藥子堆疊資料_Replace.Count > 0)
            {
                this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥子堆疊資料_Replace, false);
            }
            this.MyTimer_取藥堆疊資料_流程作業檢查.TickStop();
            this.MyTimer_取藥堆疊資料_流程作業檢查.StartTickTime();
            cnt++;
        }
        void cnt_Program_取藥堆疊資料_流程作業檢查_檢查層架手勢感測感應到(ref int cnt)
        {
            List<Task> taskList = new List<Task>();
            string IP = "";
            string 藥品碼 = "";
            string 調劑台名稱 = "";
            string GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            double 數量 = 0;
            bool flag_TOFON = false;
            Color color = Color.Black;
            List<object[]> list_取藥母堆疊資料 = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
            List<object[]> list_取藥母堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料 = this.sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
            List<object[]> list_取藥子堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料_replace = new List<object[]>();



            Task allTask;
            List<string[]> list_需更新資料;
            List<object[]> list_取藥子堆疊資料_手勢感測作業檢查 = new List<object[]>();

            list_取藥子堆疊資料_手勢感測作業檢查 = (from value in list_取藥子堆疊資料
                                     where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                     where value[(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == false.ToString()
                                     where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                     where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD266.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD213.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420G.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD360E.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.Pannel35.GetEnumName()
                                     select value).ToList();
            taskList = new List<Task>();
            list_需更新資料 = new List<string[]>();
            List<string[]> list_手勢檢查資料 = new List<string[]>();
            for (int i = 0; i < list_取藥子堆疊資料_手勢感測作業檢查.Count; i++)
            {
                IP = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.IP].ObjectToString();
                藥品碼 = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();
                Master_GUID = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);

                list_取藥子堆疊資料_buf = (from temp in list_取藥子堆疊資料_replace
                                    where temp[(int)enum_取藥堆疊母資料.IP].ObjectToString() == IP
                                    select temp).ToList();
                if (list_取藥母堆疊資料_buf.Count > 0)
                {
                    color = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                    數量 = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToDouble();
                    if (數量 < 0) 數量 = 數量 * -1;
                    if (list_取藥子堆疊資料_buf.Count == 0)
                    {
                        string LCD_Laser_ON_IP = "";
                        Storage storage = null;

                        storage = List_EPD266_雲端資料.SortByIP(IP);
                        if(storage != null)
                        {
                            //if(storage.IsFADC)
                            //{
                            //    cnt++;
                            //    return;
                            //}
                        }
                        storage = null;
                        List<Storage> storages = List_EPD266_雲端資料.SortByCode(藥品碼);
                        if (storages.Count != 0)
                        {
                            for (int k = 0; k < storages.Count; k++)
                            {
                                object uDP_READ = null;
                                string index_IP = Funcion_取得LCD114索引表_index_IP(storages[k].IP);
                                if (index_IP.StringIsEmpty() == false)
                                {
                                    uDP_READ = this.storageUI_LCD_114.Get_UDP_READ(index_IP);
                                }
                                else
                                {
                                    list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                    if (plC_CheckBox_同藥品全部亮燈.Bool) list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                                    list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);
                                    continue;
                                }
                                // 若在 LCD 讀取不到，則嘗試從 EPD 讀取
                                if (uDP_READ == null)
                                {
                                    uDP_READ = this.storageUI_EPD_266.Get_UDP_READ(storages[k].IP);
                                }

                                // 如果仍然為 null，跳過該存儲
                                if (uDP_READ == null) continue;

                                // 檢查並轉換為 StorageUI_LCD_114.UDP_READ
                                if (uDP_READ is StorageUI_LCD_114.UDP_READ lcdUdpRead)
                                {
                                    if (lcdUdpRead.LASER_ON && lcdUdpRead.LaserDistance != 999)
                                    {
                                        LCD_Laser_ON_IP = lcdUdpRead.IP;
                                        storage = storages[k];
                                    }
                                }
                                // 檢查並轉換為 StorageUI_EPD_266.UDP_READ
                                else if (uDP_READ is StorageUI_EPD_266.UDP_READ epdUdpRead)
                                {
                                    if (epdUdpRead.LASER_ON && epdUdpRead.LaserDistance != 999)
                                    {
                                        LCD_Laser_ON_IP = epdUdpRead.IP;
                                        storage = storages[k];
                                    }
                                }
                            }
                            if (LCD_Laser_ON_IP.StringIsEmpty() == false)
                            {
                                Console.WriteLine($"IP : {storage.IP} , index_IP : {LCD_Laser_ON_IP}, Laser_ON : {true}");
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.Check_IP] = storage.IP;
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.配藥完成] = false.ToString();
                                list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);

                                LightOn lightOn = new Main_Form.LightOn(藥品碼, color);
                                lightOn.顏色 = Color.FromArgb((int)(color.R * 0.1), (int)(color.G * 0.1), (int)(color.B * 0.1));
                                lightOn.LCD_Color = Color.Black;
                                lightOn.flag_Refresh_Light = true;
                                lightOn.flag_Refresh_breathing = true;
                                Function_儲位亮燈(lightOn);
                                if (plC_CheckBox_取藥正確語音.Checked)
                                {
                                    Task.Run(new Action(delegate
                                    {
                                        using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\取藥正確.wav"))
                                        {
                                            sp.Stop();
                                            sp.Play();
                                            sp.PlaySync();
                                        }

                                    }));
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            allTask = Task.WhenAll(taskList);
            allTask.Wait();
            if (list_取藥子堆疊資料_replace.Count > 0)
            {
                this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥子堆疊資料_replace, false);
            }

            cnt++;
        }
        void cnt_Program_取藥堆疊資料_流程作業檢查_檢查層架手勢感測離開(ref int cnt)
        {
            List<Task> taskList = new List<Task>();
            string IP = "";
            string Check_IP = "";
            string 藥品碼 = "";
            string 調劑台名稱 = "";
            string GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            bool flag_TOFON = false;
            Color color = Color.Black;
            List<object[]> list_取藥母堆疊資料 = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
            List<object[]> list_取藥母堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料 = this.sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
            List<object[]> list_取藥子堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料_replace = new List<object[]>();

            Task allTask;
            List<string[]> list_需更新資料;
            List<object[]> list_取藥子堆疊資料_手勢感測作業檢查 = new List<object[]>();

            list_取藥子堆疊資料_手勢感測作業檢查 = (from value in list_取藥子堆疊資料
                                     where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                     where value[(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == true.ToString()
                                     where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                     where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD266.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290.GetEnumName()
                                     || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD213.GetEnumName()
                                     || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420G.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD360E.GetEnumName()
                                     || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.Pannel35.GetEnumName()
                                     select value).ToList();
            taskList = new List<Task>();
            list_需更新資料 = new List<string[]>();
            List<string[]> list_手勢檢查資料 = new List<string[]>();
            for (int i = 0; i < list_取藥子堆疊資料_手勢感測作業檢查.Count; i++)
            {
                IP = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.IP].ObjectToString();
                Check_IP = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.Check_IP].ObjectToString();
                藥品碼 = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();
                Master_GUID = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);

                list_取藥子堆疊資料_buf = (from temp in list_取藥子堆疊資料_replace
                                    where temp[(int)enum_取藥堆疊母資料.IP].ObjectToString() == IP
                                    select temp).ToList();
                if (list_取藥母堆疊資料_buf.Count > 0)
                {
                    color = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
               
                    if (list_取藥子堆疊資料_buf.Count == 0 && Check_IP.Check_IP_Adress())
                    {
                        Storage storage = List_EPD266_雲端資料.SortByIP(Check_IP);
                        if (storage != null)
                        {
                            StorageUI_LCD_114.UDP_READ uDP_READ_LCD = null;
                            StorageUI_EPD_266.UDP_READ uDP_READ_266 = null;
                            string index_IP = Funcion_取得LCD114索引表_index_IP(storage.IP);
                            if (index_IP.StringIsEmpty() == false)
                            {
                                uDP_READ_LCD = this.storageUI_LCD_114.Get_UDP_READ(index_IP);
                            }
                            else
                            {
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                                list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);
                                continue;
                            }

                            uDP_READ_266 = this.storageUI_EPD_266.Get_UDP_READ(storage.IP);
                            if (uDP_READ_LCD == null && uDP_READ_266 == null) continue;
                            bool Laser_ON = false;
                            if (uDP_READ_LCD != null) if (uDP_READ_LCD.LaserDistance != -999) Laser_ON = uDP_READ_LCD.LASER_ON;
                                else if (uDP_READ_266 != null) if (uDP_READ_266.LaserDistance != -999) Laser_ON = uDP_READ_266.LASER_ON;

                            if (Laser_ON == false)
                            {
                                Console.WriteLine($"IP : {storage.IP} , index_IP : {index_IP}, Laser_ON : {Laser_ON}");

                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                                list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);

                                LightOn lightOn = new Main_Form.LightOn(藥品碼, Color.DarkGray);
                                lightOn.flag_Refresh_Light = true;
                                lightOn.flag_Refresh_breathing = true;

                                Function_儲位亮燈(lightOn);
                            }
                        }
                    }
                }
            }

            allTask = Task.WhenAll(taskList);
            allTask.Wait();
            if (list_取藥子堆疊資料_replace.Count > 0)
            {
                this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥子堆疊資料_replace, false);
            }

            cnt++;
        }
        void cnt_Program_取藥堆疊資料_流程作業檢查_檢查抽屜手勢感測感應到(ref int cnt)
        {
            List<Task> taskList = new List<Task>();
            string IP = "";
            string 藥品碼 = "";
            string 調劑台名稱 = "";
            string GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            bool flag_TOFON = false;
            Color color = Color.Black;
            List<object[]> list_取藥母堆疊資料 = this.sqL_DataGridView_取藥堆疊母資料.SQL_GetAllRows(false);
            List<object[]> list_取藥母堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料 = this.sqL_DataGridView_取藥堆疊子資料.SQL_GetAllRows(false);
            List<object[]> list_取藥子堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料_replace = new List<object[]>();



            Task allTask;
            List<string[]> list_需更新資料;
            List<object[]> list_取藥子堆疊資料_手勢感測作業檢查 = new List<object[]>();

            list_取藥子堆疊資料_手勢感測作業檢查 = (from value in list_取藥子堆疊資料
                                     where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                     where value[(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == false.ToString()
                                     where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                     where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD583.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD583_lock.GetEnumName()
                                           || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD730E.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD730E_lock.GetEnumName()
                                     select value).ToList();

            //if (plC_CheckBox_同藥品全部亮燈.Bool)
            //{
            //    for (int i = 0; i < list_取藥子堆疊資料_手勢感測作業檢查.Count; i++) list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
            //    if (list_取藥子堆疊資料_手勢感測作業檢查.Count > 0) this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥子堆疊資料_手勢感測作業檢查, false);
            //    cnt++;
            //    return;
            //}

            taskList = new List<Task>();
            list_需更新資料 = new List<string[]>();
            List<string[]> list_手勢檢查資料 = new List<string[]>();
            for (int i = 0; i < list_取藥子堆疊資料_手勢感測作業檢查.Count; i++)
            {
                IP = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.IP].ObjectToString();
                藥品碼 = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();
                Master_GUID = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);

                list_取藥子堆疊資料_buf = (from temp in list_取藥子堆疊資料_replace
                                    where temp[(int)enum_取藥堆疊母資料.IP].ObjectToString() == IP
                                    select temp).ToList();
                if (list_取藥母堆疊資料_buf.Count > 0)
                {
                    color = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                    double 數量 = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.總異動量].StringToDouble();
                    if (數量 < 0) 數量 = 數量 * -1;
                    if (list_取藥子堆疊資料_buf.Count == 0)
                    {
                        List<Box> boxes = List_EPD583_本地資料.SortByCode(藥品碼);
                        for (int k = 0; k < boxes.Count; k++)
                        {
                            Drawer drawer = List_EPD583_本地資料.SortByIP(boxes[k].IP);
                            if (drawer == null) continue;


                            string index_IP = Funcion_取得LCD114索引表_index_IP(boxes[k].IP);
                            if (index_IP.StringIsEmpty())
                            {
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                if (plC_CheckBox_同藥品全部亮燈.Bool) list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                                list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);
                                continue;
                            }
                            Rectangle rectangle = DrawerUI_EPD_583.Get_Box_rect(drawer, boxes[k]);
                            DrawerUI_EPD_583.LightSensorClass lightSensorClass = DrawerUI_EPD_583.Get_LightSensorClass(rectangle);

                            StorageUI_LCD_114.UDP_READ uDP_READ = this.storageUI_LCD_114.Get_UDP_READ(index_IP);
                            if (uDP_READ == null) continue;
                            bool Sensor_ON = uDP_READ.IsSensorOn(lightSensorClass);
                            if (Sensor_ON)
                            {
                                Console.WriteLine($"lightSensorClass : {lightSensorClass}");
                                Console.WriteLine($"IP : {boxes[k].IP} , index_IP : {index_IP}, Sensor_ON : {Sensor_ON}");

                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);
                                LightOn lightOn = new Main_Form.LightOn(藥品碼, color, 數量);
                                lightOn.顏色 = Color.FromArgb((int)(color.R * 0.1), (int)(color.G * 0.1), (int)(color.B * 0.1));
                                lightOn.flag_Refresh_Light = true;
                                Function_儲位亮燈(lightOn);
                                if (plC_CheckBox_取藥正確語音.Checked)
                                {
                                    Task.Run(new Action(delegate
                                    {
                                        using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\取藥正確.wav"))
                                        {
                                            sp.Stop();
                                            sp.Play();
                                            sp.PlaySync();
                                        }

                                    }));
                                    break;
                                }

                            }
                            else if (uDP_READ.Input > 0)
                            {
                                int temp = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.暫存參數].StringToInt32();
                                list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);

                                if (plC_CheckBox_取藥錯誤語音.Checked)
                                {
                                    if (temp != uDP_READ.Input)
                                    {
                                        Task.Run(new Action(delegate
                                        {
                                            using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{currentDirectory}\取藥錯誤_1.wav"))
                                            {
                                                sp.Stop();
                                                sp.Play();
                                                sp.PlaySync();
                                            }

                                        }));
                                        list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.暫存參數] = uDP_READ.Input.ToString();
                                        Console.WriteLine($"{DateTime.Now.ToDateTimeString()}-[取藥錯誤] ({index_IP}),暫存參數 : {temp}");
                                    }
                                }


                            }
                        }

                    }
                }
            }

            allTask = Task.WhenAll(taskList);
            allTask.Wait();
            if (list_取藥子堆疊資料_replace.Count > 0)
            {
                this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥子堆疊資料_replace, false);
            }

            cnt++;
        }
        #endregion      
        #region PLC_取藥堆疊資料_入賬檢查
        PLC_Device PLC_Device_取藥堆疊資料_入賬檢查 = new PLC_Device("");
        MyTimer MyTimer_取藥堆疊資料_入賬檢查刷新延遲 = new MyTimer();
        int cnt_Program_取藥堆疊資料_入賬檢查 = 65534;
        void sub_Program_取藥堆疊資料_入賬檢查()
        {
            PLC_Device_取藥堆疊資料_入賬檢查.Bool = PLC_Device_主機扣賬模式.Bool;
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 65534)
            {
                PLC_Device_取藥堆疊資料_入賬檢查.SetComment("PLC_取藥堆疊資料_入賬檢查");
                PLC_Device_取藥堆疊資料_入賬檢查.Bool = false;
                cnt_Program_取藥堆疊資料_入賬檢查 = 65535;
            }
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 65535) cnt_Program_取藥堆疊資料_入賬檢查 = 1;
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 1) cnt_Program_取藥堆疊資料_入賬檢查_檢查按下(ref cnt_Program_取藥堆疊資料_入賬檢查);
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 2) cnt_Program_取藥堆疊資料_入賬檢查_初始化(ref cnt_Program_取藥堆疊資料_入賬檢查);
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 3) cnt_Program_取藥堆疊資料_入賬檢查_等待延遲(ref cnt_Program_取藥堆疊資料_入賬檢查);
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 4) cnt_Program_取藥堆疊資料_入賬檢查 = 65500;
            if (cnt_Program_取藥堆疊資料_入賬檢查 > 1) cnt_Program_取藥堆疊資料_入賬檢查_檢查放開(ref cnt_Program_取藥堆疊資料_入賬檢查);
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 65500)
            {

                PLC_Device_取藥堆疊資料_入賬檢查.Bool = false;
                cnt_Program_取藥堆疊資料_入賬檢查 = 65535;
            }
        }
        void cnt_Program_取藥堆疊資料_入賬檢查_檢查按下(ref int cnt)
        {
            if (PLC_Device_取藥堆疊資料_入賬檢查.Bool) cnt++;
        }
        void cnt_Program_取藥堆疊資料_入賬檢查_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取藥堆疊資料_入賬檢查.Bool) cnt = 65500;
        }
        void cnt_Program_取藥堆疊資料_入賬檢查_初始化(ref int cnt)
        {
            List<object[]> list_可入賬母資料 = this.Function_取藥堆疊母資料_取得可入賬資料();
            List<object[]> list_子資料 = this.Function_取藥堆疊資料_取得子資料();
            List<object[]> list_子資料_buf;
            List<object[]> list_取藥堆疊子資料_ReplaceValue = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_Add = new List<object[]>();
            List<object[]> list_交易紀錄新增資料_AddValue = new List<object[]>();
            List<object[]> list_醫囑資料_ReplaceValue = new List<object[]>();

            bool flag_修正盤點量 = false;
            string GUID = "";
            string Order_GUID = "";
            string Master_GUID = "";
            double 庫存量 = 0;
            double 結存量 = 0;
            double 總異動量 = 0;
            string 盤點量 = "";
            string 動作 = "";
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 藥袋序號 = "";
            string 類別 = "";
            string 交易量 = "";
            string 操作人 = "";
            string ID = "";
            string 覆核藥師姓名 = "";
            string 覆核藥師ID = "";
            string 病人姓名 = "";
            string 床號 = "";
            string 頻次 = "";
            string 病歷號 = "";
            string 操作時間 = "";
            string 開方時間 = "";
            string 備註 = "";
            string 收支原因 = "";
            string 診別 = "";
            string 藥師證字號 = "";
            string 效期 = "";
            string 批號 = "";
            string 顏色 = "";
            string 領藥號 = "";
            string 病房號 = "";
            string 醫令_GUID = "";
            string 交易紀錄_GUID = "";

            List<string> List_效期 = new List<string>();
            List<string> List_批號 = new List<string>();
            List<string> list_儲位刷新_藥品碼 = new List<string>();
            List<string> list_儲位刷新_藥品碼_buf = new List<string>();
            list_可入賬母資料.Sort(new Icp_取藥堆疊母資料_index排序());
            List<string> list_Codes = (from temp in list_可入賬母資料
                                       select temp[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString()).Distinct().ToList();
            for (int i = 0; i < list_Codes.Count; i++)
            {
                this.Function_從SQL取得儲位到入賬資料(list_Codes[i]);
            }
            for (int i = 0; i < list_可入賬母資料.Count; i++)
            {

                Master_GUID = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                Order_GUID = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.Order_GUID].ObjectToString();
                動作 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.動作].ObjectToString();
                診別 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.診別].ObjectToString();
                藥品碼 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                //this.Function_從SQL取得儲位到入賬資料(藥品碼);
                藥品名稱 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                藥袋序號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.藥袋序號].ObjectToString();
                類別 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.類別].ObjectToString();
                操作人 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.操作人].ObjectToString();
                ID = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.ID].ObjectToString();
                藥師證字號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.藥師證字號].ObjectToString();
                總異動量 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToDouble();
                交易量 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString();
                盤點量 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.盤點量].ObjectToString();
                顏色 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.顏色].ObjectToString();
                領藥號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.領藥號].ObjectToString();
                病房號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.病房號].ObjectToString();
                病人姓名 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.病人姓名].ObjectToString();
                床號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.床號].ObjectToString();
                頻次 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.頻次].ObjectToString();
                病歷號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.病歷號].ObjectToString();
                操作時間 = DateTime.Now.ToDateTimeString_6();
                開方時間 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.開方時間].ObjectToString();
                備註 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.備註].ObjectToString();
                收支原因 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.收支原因].ObjectToString();
                效期 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.效期].ObjectToString();
                批號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.批號].ObjectToString();
                覆核藥師姓名 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.覆核藥師姓名].ObjectToString();
                覆核藥師ID = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.覆核藥師ID].ObjectToString();
                庫存量 = this.Function_從入賬資料取得庫存(藥品碼);
                結存量 = (庫存量 + 總異動量);


                if (藥品名稱.StringIsEmpty())
                {
                    medClass medClass = medClass.get_med_clouds_by_code(Main_Form.API_Server, 藥品碼);
                    if (medClass != null)
                    {
                        藥品名稱 = medClass.藥品名稱;
                    }
                }

                List_效期.Clear();
                List_批號.Clear();
                list_子資料_buf = list_子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);

                for (int k = 0; k < list_子資料_buf.Count; k++)
                {

                    list_子資料_buf[k] = this.Function_取藥堆疊子資料_設定已入帳(list_子資料_buf[k]);
                    List_效期.Add(list_子資料_buf[k][(int)enum_取藥堆疊子資料.效期].ObjectToString());
                    List_批號.Add(list_子資料_buf[k][(int)enum_取藥堆疊子資料.批號].ObjectToString());
                    list_取藥堆疊子資料_ReplaceValue.Add(list_子資料_buf[k]);
                    //Function_取藥堆疊資料_語音提示(list_子資料_buf[k]);
                }
                list_可入賬母資料[i][(int)enum_取藥堆疊母資料.庫存量] = 庫存量.ToString();
                list_可入賬母資料[i][(int)enum_取藥堆疊母資料.結存量] = 結存量.ToString();
                list_可入賬母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName();
                list_取藥堆疊母資料_ReplaceValue.Add(list_可入賬母資料[i]);



                //新增交易紀錄資料
                for (int k = 0; k < List_效期.Count; k++)
                {
                    備註 += $"[效期]:{List_效期[k]},[批號]:{List_批號[k]}";
                    if (k != List_效期.Count - 1) 備註 += "\n";
                }

                object[] value_trading = new object[new enum_交易記錄查詢資料().GetLength()];
                value_trading[(int)enum_交易記錄查詢資料.GUID] = Guid.NewGuid().ToString();
                交易紀錄_GUID = value_trading[(int)enum_交易記錄查詢資料.GUID].ObjectToString();
                value_trading[(int)enum_交易記錄查詢資料.動作] = 動作;
                value_trading[(int)enum_交易記錄查詢資料.Order_GUID] = Order_GUID;
                value_trading[(int)enum_交易記錄查詢資料.診別] = 診別;
                value_trading[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
                value_trading[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
                value_trading[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
                value_trading[(int)enum_交易記錄查詢資料.藥師證字號] = 藥師證字號;
                value_trading[(int)enum_交易記錄查詢資料.領藥號] = 領藥號;
                value_trading[(int)enum_交易記錄查詢資料.病房號] = 病房號;
                value_trading[(int)enum_交易記錄查詢資料.類別] = 類別;
                value_trading[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
                value_trading[(int)enum_交易記錄查詢資料.交易量] = 交易量;
                value_trading[(int)enum_交易記錄查詢資料.結存量] = 結存量;
                if (盤點量.StringIsEmpty()) 盤點量 = "無";
                else flag_修正盤點量 = true;
                value_trading[(int)enum_交易記錄查詢資料.盤點量] = 盤點量;
                value_trading[(int)enum_交易記錄查詢資料.操作人] = 操作人;
                value_trading[(int)enum_交易記錄查詢資料.覆核藥師] = 覆核藥師姓名;
                value_trading[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
                value_trading[(int)enum_交易記錄查詢資料.床號] = 床號;
                value_trading[(int)enum_交易記錄查詢資料.頻次] = 頻次;
                value_trading[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
                value_trading[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
                if (開方時間.StringIsEmpty()) 開方時間 = DateTime.Now.ToDateTimeString_6();
                value_trading[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
                value_trading[(int)enum_交易記錄查詢資料.備註] = 備註;
                收支原因 = $"{收支原因}";
                value_trading[(int)enum_交易記錄查詢資料.收支原因] = 收支原因;

                if ((動作.Contains("系統")))
                {
                    if (總異動量 == 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (顏色 == Color.Black.ToColorString())
                        {
                            list_儲位刷新_藥品碼_buf = (from temp in list_儲位刷新_藥品碼
                                                 where temp == 藥品碼
                                                 select temp).ToList();
                            if (list_儲位刷新_藥品碼_buf.Count == 0)
                            {
                                list_儲位刷新_藥品碼.Add(藥品碼);
                            }
                        }

                    }
                }
                if (收支原因.Contains("盤點異常"))
                {

                    medRecheckLogClass medRecheckLogClass = new medRecheckLogClass();
                    int 差異值 = medRecheckLogClass.get_unresolved_qty_by_code(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, 藥品碼);
                    medRecheckLogClass.發生類別 = "盤點異常";
                    medRecheckLogClass.藥碼 = 藥品碼;
                    medRecheckLogClass.藥名 = 藥品名稱;
                    medRecheckLogClass.庫存值 = (結存量 + 差異值).ToString();
                    medRecheckLogClass.盤點值 = 盤點量;
                    medRecheckLogClass.盤點藥師1 = 操作人;
                    medRecheckLogClass.盤點藥師2 = 覆核藥師姓名;
                    medRecheckLogClass.交易紀錄_GUID = 交易紀錄_GUID;
                    for (int m = 0; m < List_效期.Count; m++)
                    {
                        medRecheckLogClass.效期 += List_效期[m];
                        if (m != List_效期.Count - 1) medRecheckLogClass.效期 += ",";
                    }
                    for (int m = 0; m < List_批號.Count; m++)
                    {
                        medRecheckLogClass.批號 += List_批號[m];
                        if (m != List_批號.Count - 1) medRecheckLogClass.批號 += ",";
                    }
                    medRecheckLogClass.add(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, medRecheckLogClass);

                }
                list_交易紀錄新增資料_AddValue.Add(value_trading);
                Console.WriteLine($"寫入交易紀錄,藥碼 : {藥品碼} ,交易量 : {交易量}");


            }
            for (int i = 0; i < list_取藥堆疊母資料_ReplaceValue.Count; i++)
            {

                Order_GUID = list_取藥堆疊母資料_ReplaceValue[i][(int)enum_取藥堆疊母資料.Order_GUID].ObjectToString();
                List<object[]> list_value = this.sqL_DataGridView_醫令資料.SQL_GetRows((int)enum_醫囑資料.GUID, Order_GUID, false);
          
                操作人 = list_取藥堆疊母資料_ReplaceValue[i][(int)enum_取藥堆疊母資料.操作人].ObjectToString();
                總異動量 = list_取藥堆疊母資料_ReplaceValue[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToDouble();
                if (list_value.Count == 0) continue;
                for (int m = 0; m < list_value.Count; m++)
                {
                    //if (list_value[m][(int)enum_醫囑資料.狀態].ObjectToString() == enum_醫囑資料_狀態.已過帳.GetEnumName()) continue;
                    list_value[m][(int)enum_醫囑資料.狀態] = enum_醫囑資料_狀態.已過帳.GetEnumName();
                    list_value[m][(int)enum_醫囑資料.過帳時間] = DateTime.Now.ToDateTimeString_6();
                    list_value[m][(int)enum_醫囑資料.藥師ID] = ID;
                    list_value[m][(int)enum_醫囑資料.藥師姓名] = 操作人;

                    string 實際調劑量 = list_value[m][(int)enum_醫囑資料.實際調劑量].ObjectToString();
                    if(實際調劑量.StringIsEmpty()) 實際調劑量 = "0";
                    實際調劑量 = (實際調劑量.StringToDouble() + 總異動量.StringToDouble()).ToString();
                    list_value[m][(int)enum_醫囑資料.實際調劑量] = 實際調劑量;
                    list_醫囑資料_ReplaceValue.Add(list_value[m]);
                }


                //Console.WriteLine($"{orderClasses.JsonSerializationt()}");
            }
            for (int i = 0; i < list_儲位刷新_藥品碼.Count; i++)
            {
                Function_從SQL取得儲位到本地資料(list_儲位刷新_藥品碼[i]);
                Function_儲位刷新(list_儲位刷新_藥品碼[i]);
            }

            if (list_交易紀錄新增資料_AddValue.Count > 0) this.sqL_DataGridView_交易記錄查詢.SQL_AddRows(list_交易紀錄新增資料_AddValue, false);
            if (list_取藥堆疊子資料_ReplaceValue.Count > 0) this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥堆疊子資料_ReplaceValue, false);
            if (list_取藥堆疊母資料_ReplaceValue.Count > 0) this.sqL_DataGridView_取藥堆疊母資料.SQL_ReplaceExtra(list_取藥堆疊母資料_ReplaceValue, false);
            if (list_醫囑資料_ReplaceValue.Count > 0)
            {
                if (dBConfigClass.Order_upload_ApiURL.StringIsEmpty() == false)
                {
                    List<OrderClass> orderClasses = list_醫囑資料_ReplaceValue.SQLToClass<OrderClass, enum_醫囑資料>();
                    returnData returnData = new returnData();
                    returnData.Data = orderClasses;
                    Basic.Net.WEBApiPostJsonAsync(dBConfigClass.Order_upload_ApiURL, returnData.JsonSerializationt(), false);
                }
                this.sqL_DataGridView_醫令資料.SQL_ReplaceExtra(list_醫囑資料_ReplaceValue, false);
            }
            cnt++;
        }
        void cnt_Program_取藥堆疊資料_入賬檢查_等待延遲(ref int cnt)
        {
            cnt++;
        }




        #endregion

    }

    public class ICP_取藥堆疊母資料 : IComparer<object[]>
    {
        //實作Compare方法
        //依Speed由小排到大。
        public int Compare(object[] x, object[] y)
        {
            DateTime datetime1 = x[(int)enum_取藥堆疊母資料.操作時間].ToDateTimeString_6().StringToDateTime();
            DateTime datetime2 = y[(int)enum_取藥堆疊母資料.操作時間].ToDateTimeString_6().StringToDateTime();
            int compare = DateTime.Compare(datetime1, datetime2);
            return compare;
        }
    }
}