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
using FingerprintLib;
using System.Threading;
using SQLUI;
using H_Pannel_lib;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace 調劑台管理系統
{
    public partial class Dialog_自動出藥 : MyDialog
    {
        private IpCallCounter ipCallCounter = new IpCallCounter();
        private MyThread myThread = new MyThread();
        public personPageClass personPage = new personPageClass();
        public List<takeMedicineStackClass> takeMedicines = new List<takeMedicineStackClass>();
        List<string> refresh_ip = new List<string>();
        public enum enum_出藥資訊
        {
            [Description("GUID,VARCHAR,50,NONE")]
            GUID,
            [Description("master_guid,VARCHAR,50,NONE")]
            master_guid,
            [Description("藥碼,VARCHAR,50,NONE")]
            藥碼,
            [Description("藥名,VARCHAR,50,NONE")]
            藥名,
            [Description("應出,VARCHAR,50,NONE")]
            應出,
            [Description("實出,VARCHAR,50,NONE")]
            實出,
            [Description("狀態,VARCHAR,50,NONE")]
            狀態,
            [Description("Value,VARCHAR,50,NONE")]
            Value,
        }
        public Dialog_自動出藥(List<takeMedicineStackClass> takeMedicines)
        {
            form.Invoke(new Action(delegate 
            {
                InitializeComponent();
                sqL_DataGridView_出藥資訊.RowsHeight = 60;
                sqL_DataGridView_出藥資訊.Init(new Table(new enum_出藥資訊()));
                sqL_DataGridView_出藥資訊.Set_ColumnVisible(false, new enum_出藥資訊().GetEnumNames());
                sqL_DataGridView_出藥資訊.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_出藥資訊.藥碼);
                sqL_DataGridView_出藥資訊.Set_ColumnWidth(600, DataGridViewContentAlignment.MiddleLeft, enum_出藥資訊.藥名);
                sqL_DataGridView_出藥資訊.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_出藥資訊.應出);
                sqL_DataGridView_出藥資訊.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_出藥資訊.實出);
                sqL_DataGridView_出藥資訊.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_出藥資訊.狀態);
            }));
        

            this.takeMedicines = takeMedicines;
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < takeMedicines.Count; i++)
            {
                string GUID = takeMedicines[i].GUID;
                int qty = (int)Math.Abs(takeMedicines[i].總異動量.StringToDouble());
                for (int k = 0; k < qty; k++)
                {
                    object[] value = new object[new enum_出藥資訊().GetLength()];
                    value[(int)enum_出藥資訊.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_出藥資訊.master_guid] = takeMedicines[i].GUID;
                    value[(int)enum_出藥資訊.藥碼] = takeMedicines[i].藥品碼;
                    value[(int)enum_出藥資訊.藥名] = takeMedicines[i].藥品名稱;
                    value[(int)enum_出藥資訊.應出] = "1";
                    value[(int)enum_出藥資訊.實出] = "0";
                    value[(int)enum_出藥資訊.狀態] = "等待中";
                    list_value.Add(value);
                }
            }
            sqL_DataGridView_出藥資訊.RefreshGrid(list_value);

            myThread.SetSleepTime(10);
            myThread.Add_Method(sub_program);
            myThread.AutoRun(true);
            myThread.Trigger();

            this.LoadFinishedEvent += Dialog_自動出藥_LoadFinishedEvent;
            this.FormClosed += Dialog_自動出藥_FormClosed;
        }

        private void Dialog_自動出藥_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (myThread != null)
            {
                myThread.Abort();
                myThread = null;
            }
         
        }


        private void Dialog_自動出藥_LoadFinishedEvent(EventArgs e)
        {

        }
        private List<object[]> objects_storages = new List<object[]>();
        private object[] objects_storage;
        private object[] objects_出藥資訊;
        private storageMedBoxIOConfigClass storageMedBoxIO;
        private int MotorCnt;
        private int cnt = 1;
        private bool flag_有藥品要取 = false;
        private void sub_program()
        {
            if (cnt == 1)
            {
                List<object[]> objects = this.sqL_DataGridView_出藥資訊.GetAllRows();

                cnt++;
            }
            if (cnt == 2)
            {
                objects_storages.Clear();
                List<object[]> objects = this.sqL_DataGridView_出藥資訊.GetAllRows();
                Main_Form.Function_從SQL取得儲位到本地資料();
                for (int i = 0; i < objects.Count; i++)
                {
                    string code = objects[i][(int)enum_出藥資訊.藥碼].ObjectToString();
                    string state = objects[i][(int)enum_出藥資訊.狀態].ObjectToString();
                    int qty = objects[i][(int)enum_出藥資訊.應出].StringToInt32();

                    List<object[]> objects_ = Main_Form.Function_取得異動儲位資訊從本地資料(code, -qty);
                    if (objects_.Count == 0)
                    {
                        objects[i][(int)enum_出藥資訊.狀態] = "庫存不足";
                        this.sqL_DataGridView_出藥資訊.ReplaceExtra(objects[i], true);

                        if (MyMessageBox.ShowDialog("庫存無法完全取藥,是否繼續?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.No)
                        {
                            Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("中斷取藥", 1500, Color.Red);
                            DialogResult = DialogResult.Yes;
                            dialog_AlarmForm.ShowDialog();
                            this.Close();
                            return;
                        }
                        continue;
                    }
                    objects[i][(int)enum_出藥資訊.Value] = objects_[0];
                    Main_Form.Function_庫存異動至本地資料(objects_[0]);
                    this.sqL_DataGridView_出藥資訊.RefreshGrid(objects);
                }
                cnt++;
            }
            if (cnt == 3)
            {
                string IP = "";
                List<object[]> objects = this.sqL_DataGridView_出藥資訊.GetAllRows();
                objects_出藥資訊 = null;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i][(int)enum_出藥資訊.狀態].ObjectToString() == "已領過" || objects[i][(int)enum_出藥資訊.狀態].ObjectToString() == "連線異常" || objects[i][(int)enum_出藥資訊.狀態].ObjectToString() == "庫存不足") continue;
                    objects[i][(int)enum_出藥資訊.狀態] = "領用中";
                    objects_出藥資訊 = objects[i];
           
                    objects_storage = (object[])objects[i][(int)enum_出藥資訊.Value];
                    if(objects_storage == null)
                    {
                        continue;
                    }
                    IP = objects_storage[(int)Main_Form.enum_儲位資訊.IP].ObjectToString();
             

                    List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses = storageMedBoxIOConfigClass.get_all(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType);
                    storageMedBoxIO = storageMedBoxIOConfigClasses.Where(x => x.IP == IP).FirstOrDefault();
                    if (storageMedBoxIO == null)
                    {
                        Console.WriteLine($"[出貨一次] - 未建立馬達索引表");
                        objects[i][(int)enum_出藥資訊.狀態] = "連線異常";
                        continue;
                    }
                    string udp_json = Main_Form._storageUI_EPD_266.GetUDPJsonString(IP);
                    if (udp_json.StringIsEmpty())
                    {
                        Console.WriteLine($"[出貨一次] - UdpJson異常");
                        objects[i][(int)enum_出藥資訊.狀態] = "連線異常";
                        continue;
                    }
                    UDP_READ_basic uDP_READ_Basic = udp_json.JsonDeserializet<UDP_READ_basic>();
                    MotorCnt = uDP_READ_Basic.FADC_motorCnt;
                    break;
               
                }

                if(objects_出藥資訊 == null)
                {
                    cnt = 65500;
                    return;
                }
                IP = objects_storage[(int)Main_Form.enum_儲位資訊.IP].ObjectToString();
                Console.WriteLine($"[出貨一次] - {IP} ,MotorCnt({MotorCnt})參數 , 馬達延遲({storageMedBoxIO.出料馬達輸入延遲時間})");
                int time = 0;
                if (storageMedBoxIO != null)
                {
                    if (storageMedBoxIO.出料馬達輸入延遲時間.StringIsInt32())
                    {
                        time = storageMedBoxIO.出料馬達輸入延遲時間.StringToInt32();
                        if (time < 0) time = 0;
                    }
                }
                Storage storage = Main_Form._storageUI_EPD_266.SQL_GetStorage(IP);
                if(storage !=null)
                {
                    Main_Form._storageUI_EPD_266.Set_Stroage_LED_UDP(storage, Color.Blue);
                }
         
                Main_Form._storageUI_EPD_266.Set_ADCMotorTrigger(IP, 29000, time);
                cnt++;
            
            }
            if (cnt == 4)
            {
                string IP = objects_storage[(int)Main_Form.enum_儲位資訊.IP].ObjectToString();

                string udp_json = Main_Form._storageUI_EPD_266.GetUDPJsonString(IP);
                UDP_READ_basic uDP_READ_Basic = udp_json.JsonDeserializet<UDP_READ_basic>();
                if (uDP_READ_Basic != null)
                {
                    if (uDP_READ_Basic.FADC_motorCnt != MotorCnt)
                    {
                        Console.WriteLine($"[出貨一次] - 出料一次完成");
                        ipCallCounter.Record(IP);
                        if(ipCallCounter.GetCount(IP) >= 2)
                        {
                            Main_Form.voice.SpeakOnTask("請先取出藥品");
                            MyMessageBox.ShowDialog("請先取出藥品後按下確認");

                            ipCallCounter.Remove(IP);
                        }

                        cnt++;
                    }

                }
            }
            if (cnt == 5)
            {
                string IP = objects_storage[(int)Main_Form.enum_儲位資訊.IP].ObjectToString();
                Storage storage = Main_Form._storageUI_EPD_266.SQL_GetStorage(IP);
                refresh_ip.Add(IP);
                if (storage != null)
                {
                    string 庫存量 = Main_Form.Function_從SQL取得庫存(storage.Code).ToString();
                    string 備註 = "";
                    List<StockClass> stockClasses = storage.庫存異動(-1, true);
                    Main_Form._storageUI_EPD_266.SQL_ReplaceStorage(storage);
              
                    medClass _medClass = medClass.get_med_clouds_by_code(Main_Form.API_Server, storage.Code);

                    transactionsClass transactionsClass = new transactionsClass();
                    transactionsClass.GUID = Guid.NewGuid().ToString();
                    transactionsClass.藥品碼 = storage.Code;

                    if (_medClass != null)
                    {
                        transactionsClass.藥品名稱 = _medClass.藥品名稱;
                    }
                    transactionsClass.動作 = enum_交易記錄查詢動作.掃碼領藥.GetEnumName();
                    transactionsClass.庫存量 = 庫存量;
                    transactionsClass.交易量 = "-1";
                    transactionsClass.結存量 = (庫存量.StringToDouble() - 1).ToString();

                    for (int i = 0; i < stockClasses.Count; i++)
                    {
                        備註 += $"[效期]:{stockClasses[i].Validity_period},[批號]:{stockClasses[i].Lot_number}";
                        if (i != stockClasses.Count - 1) 備註 += "\n";
                    }
                    transactionsClass.備註 = 備註;

                    if (personPage != null)
                    {
                        transactionsClass.操作人 = personPage.姓名;
                        transactionsClass.藥師證字號 = personPage.藥師證字號;
                        transactionsClass.操作時間 = DateTime.Now.ToDateTimeString_6();
                    }
                    transactionsClass.add(Main_Form.API_Server, transactionsClass, Main_Form.ServerName, Main_Form.ServerType);
                }
                objects_出藥資訊[(int)enum_出藥資訊.狀態] = "已領過";
                this.sqL_DataGridView_出藥資訊.ReplaceExtra(objects_出藥資訊 , true);
                cnt = 3;
                return;
            }
           
            if (cnt == 65500)
            {
                Main_Form.voice.SpeakOnTask("取藥完成");
                refresh_ip = refresh_ip.Select(x => x).Distinct().ToList();

                List<Task> tasks = new List<Task>();
                foreach (var IP in refresh_ip)
                {
                    Storage storage = Main_Form._storageUI_EPD_266.SQL_GetStorage(IP);
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        if (storage != null)
                        {
                            Main_Form._storageUI_EPD_266.DrawToEpd_UDP(storage);
                            Main_Form._storageUI_EPD_266.Set_Stroage_LED_UDP(storage, Color.Black);
                        }
                    })));
                }

                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("取藥完成", 1500, Color.Green);
                DialogResult = DialogResult.Yes;
                dialog_AlarmForm.ShowDialog();
                Task.WhenAll(tasks).Wait();

                this.Close();
            }
        }


        public class IpCallCounter
        {
            // 使用 Thread-safe Dictionary
            private readonly ConcurrentDictionary<string, int> _ipCounter
                = new ConcurrentDictionary<string, int>();

            /// <summary>
            /// 記錄 IP 呼叫一次
            /// </summary>
            public void Record(string ip)
            {
                if (string.IsNullOrWhiteSpace(ip)) return;

                _ipCounter.AddOrUpdate(ip, 1, (key, oldValue) => oldValue + 1);
            }

            /// <summary>
            /// 取得某個 IP 的呼叫次數
            /// </summary>
            public int GetCount(string ip)
            {
                if (string.IsNullOrWhiteSpace(ip)) return 0;

                return _ipCounter.TryGetValue(ip, out int count) ? count : 0;
            }

            /// <summary>
            /// 取得所有 IP 與次數
            /// </summary>
            public Dictionary<string, int> GetAll()
            {
                return _ipCounter.ToDictionary(k => k.Key, v => v.Value);
            }

            /// <summary>
            /// 取得 Top N 呼叫最多的 IP
            /// </summary>
            public List<KeyValuePair<string, int>> GetTop(int topN)
            {
                return _ipCounter
                    .OrderByDescending(x => x.Value)
                    .Take(topN)
                    .ToList();
            }
            /// <summary>
            /// 清除指定 IP 的計數
            /// </summary>
            public bool Remove(string ip)
            {
                if (string.IsNullOrWhiteSpace(ip)) return false;

                return _ipCounter.TryRemove(ip, out _);
            }
            /// <summary>
            /// 清除所有紀錄
            /// </summary>
            public void Clear()
            {
                _ipCounter.Clear();
            }
        }
    }
}
