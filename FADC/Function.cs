using Basic;
using H_Pannel_lib;
using MinasA6DLL;
using MyUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLUI;
using HIS_DB_Lib;

namespace FADC
{
    public partial class Main_Form : Form
    {
        public enum enum_儲位資訊
        {
            IP,
            TYPE,
            包裝量,
            效期,
            批號,
            庫存,
            異動量,
            Value,
            藥碼,
            狀態,
        }
        static public  List<medPicClass> medPicClasses = new List<medPicClass>();
        static public  List<Image> Function_取得藥品圖片(string Code)
        {
            bool IsValidImage(Image img)
            {
                return img != null && img.Width > 0 && img.Height > 0;
            }

            List<medPicClass> medPicClasse_buf = medPicClasses.Where(temp => temp.藥碼 == Code).ToList();
            List<Image> images = new List<Image>();

            if (medPicClasse_buf.Count == 0)
            {
                medPicClass medPicClass = new medPicClass();
                List<Image> loadedImages = medPicClass.get_images_by_code(Main_Form.API_Server, Code);

                medPicClass.藥碼 = Code;

                if (loadedImages != null)
                {
                    if (loadedImages.Count > 0 && IsValidImage(loadedImages[0]))
                    {
                        medPicClass.Image_0 = loadedImages[0];
                        images.Add(loadedImages[0]);
                    }
                    if (loadedImages.Count > 1 && IsValidImage(loadedImages[1]))
                    {
                        medPicClass.Image_1 = loadedImages[1];
                        images.Add(loadedImages[1]);
                    }
                }

                medPicClasses.Add(medPicClass);
                return images;
            }
            else
            {
                var cached = medPicClasse_buf[0];
                if (IsValidImage(cached.Image_0)) images.Add(cached.Image_0);
                if (IsValidImage(cached.Image_1)) images.Add(cached.Image_1);
                return images;
            }
        }
        static public void Function_從SQL取得儲位到本地資料()
        {

            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            Console.WriteLine($"開始SQL讀取儲位資料到本地!");
            List<Task> taskList = new List<Task>();
     
            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer1 = new MyTimer();
                myTimer1.StartTickTime(50000);
                List_EPD266_本地資料 = _storageUI_EPD_266.SQL_GetAllStorage();
                Console.WriteLine($"讀取EPD266資料! 耗時 :{myTimer1.GetTickTime().ToString("0.000")} ");

            }));
          
            List<Device> deviceBasics = new List<Device>();

            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();


            Console.WriteLine($"SQL讀取儲位資料到本地結束! 耗時 : {myTimer.GetTickTime().ToString("0.000")}");
        }
        static public List<object> Function_從SQL取得儲位到本地資料(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            List<Storage> storages = List_EPD266_本地資料.SortByCode(藥品碼);

            for (int i = 0; i < storages.Count; i++)
            {
                Storage storage = _storageUI_EPD_266.SQL_GetStorage(storages[i]);
                List_EPD266_本地資料.Add_NewStorage(storage);
                list_value.Add(storage);
            }
          
            return list_value;
        }
        static public double Function_從SQL取得庫存(string 藥品碼)
        {
            double 庫存 = 0;
            List<object> list_value = Function_從SQL取得儲位到本地資料(藥品碼);
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    Device device = list_value[i] as Device;
                    if (device != null)
                    {
                        庫存 += device.Inventory.StringToDouble();
                    }
                }
            }
            return 庫存;
        }
        static public List<Device> Function_從SQL取得所有儲位()
        {
            List<List<Device>> list_list_devices = new List<List<Device>>();
            List<Device> devices = new List<Device>();
            Function_從SQL取得儲位到本地資料();

            list_list_devices.Add(List_EPD266_本地資料.GetAllDevice());


            for (int i = 0; i < list_list_devices.Count; i++)
            {
                foreach (Device device in list_list_devices[i])
                {
                    device.確認效期庫存(true);
                    devices.Add(device);
                }
            }
            return devices;
        }
        static public List<chemotherapyOrderClass> Function_取得醫令(string barcode)
        {
            string url = $"{Order_URL}{barcode}";
            string json = Basic.Net.WEBApiGet(url);
            returnData returnData = json.JsonDeserializet<returnData>();
            if(returnData == null)
            {
                return null;
            }
            List<chemotherapyOrderClass> chemotherapyOrderClasses = returnData.Data.ObjToClass<List<chemotherapyOrderClass>>();
            return chemotherapyOrderClasses;

        }
        static public void Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作 enum_交易記錄查詢動作, string 操作人, string 備註)
        {
            if (操作人.StringIsEmpty()) return;
            string GUID = Guid.NewGuid().ToString();
            string 動作 = enum_交易記錄查詢動作.GetEnumName();
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 藥袋序號 = "";
            string 庫存量 = "";
            string 交易量 = "";
            string 結存量 = "";
            string 病人姓名 = "";
            string 病歷號 = "";
            string 操作時間 = DateTime.Now.ToDateTimeString_6();
            string 開方時間 = DateTime.Now.ToDateTimeString_6();
            object[] value = new object[new enum_交易記錄查詢資料().GetLength()];
            value[(int)enum_交易記錄查詢資料.GUID] = GUID;
            value[(int)enum_交易記錄查詢資料.動作] = 動作;
            value[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
            value[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
            value[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
            value[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
            value[(int)enum_交易記錄查詢資料.交易量] = 交易量;
            value[(int)enum_交易記錄查詢資料.結存量] = 結存量;
            value[(int)enum_交易記錄查詢資料.操作人] = 操作人;
            value[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
            value[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
            value[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
            value[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
            value[(int)enum_交易記錄查詢資料.領用時間] = DateTime.MinValue.ToDateTimeString();
            value[(int)enum_交易記錄查詢資料.備註] = 備註;

            transactionsClass.add(API_Server, value.SQLToClass<transactionsClass, enum_交易記錄查詢資料>(), ServerName, ServerType);
        }
        static public List<object[]> Function_取得異動儲位資訊從本地資料(string 藥品碼, double 異動量)
        {
            bool debug = false;
            if (debug) Console.WriteLine($"[取得異動儲位資訊] 藥品碼={藥品碼}, 異動量={異動量}");

            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從本地資料取得儲位(藥品碼, ref 儲位_TYPE, ref 儲位);

            List<object[]> 儲位資訊_buf = new List<object[]>();
            List<object[]> 儲位資訊 = new List<object[]>();

            if (儲位.Count == 0)
            {
                if (debug) Console.WriteLine("[取得異動儲位資訊] 無儲位資料");
                return 儲位資訊_buf;
            }
            // 組儲位資訊
            for (int k = 0; k < 儲位.Count; k++)
            {
                object value_device = 儲位[k];
                if (value_device is Device device)
                {
                    for (int i = 0; i < device.List_Validity_period.Count; i++)
                    {
                        object[] value = new object[new enum_儲位資訊().GetLength()];
                        value[(int)enum_儲位資訊.IP] = device.IP;
                        value[(int)enum_儲位資訊.TYPE] = 儲位_TYPE[k];
                        if (device.Min_Package_Num.StringToDouble() < 1) device.Min_Package_Num = "1";
                        value[(int)enum_儲位資訊.包裝量] = device.Min_Package_Num;
                        value[(int)enum_儲位資訊.效期] = device.List_Validity_period[i];
                        value[(int)enum_儲位資訊.批號] = device.List_Lot_number[i];
                        value[(int)enum_儲位資訊.庫存] = device.List_Inventory[i];
                        value[(int)enum_儲位資訊.異動量] = "0";
                        value[(int)enum_儲位資訊.Value] = value_device;
                        儲位資訊.Add(value);
                    }
                }
            }
            if (debug) Console.WriteLine($"[儲位總數] {儲位資訊.Count} 筆");

            for (int i = 0; i < 儲位資訊.Count; i++)
            {
                string ip = 儲位資訊[i][(int)enum_儲位資訊.IP].ObjectToString();
                string type = 儲位資訊[i][(int)enum_儲位資訊.TYPE].ObjectToString();
                string pack = 儲位資訊[i][(int)enum_儲位資訊.包裝量].ObjectToString();
                string lot = 儲位資訊[i][(int)enum_儲位資訊.批號].ObjectToString();
                string exp = 儲位資訊[i][(int)enum_儲位資訊.效期].ToDateString();
                string stock = 儲位資訊[i][(int)enum_儲位資訊.庫存].ObjectToString();

                if (debug) Console.WriteLine($"[儲位明細] IP={ip}, TYPE={type}, 包裝量={pack}, 效期={exp}, 批號={lot}, 庫存={stock}");
            }

            if (異動量 == 0)
            {
                if (debug) Console.WriteLine("[異動量為0] 無需異動");
                return 儲位資訊;
            }

            double 使用數量 = 異動量;
            儲位資訊_buf.Clear();

            // 分組
            var 儲位_大包裝 = 儲位資訊
                  .Where(r => r[(int)enum_儲位資訊.包裝量].StringToDouble() > 1)
                  .OrderByDescending(r => r[(int)enum_儲位資訊.包裝量].StringToDouble())
                  .ThenBy(r => TryParseDateTimeOrMax(r[(int)enum_儲位資訊.效期].ToDateString()))
                  .ToList();

            var 儲位_單包裝 = 儲位資訊
                  .Where(r => r[(int)enum_儲位資訊.包裝量].StringToDouble() == 1)
                  .OrderBy(r => TryParseDateTimeOrMax(r[(int)enum_儲位資訊.效期].ToDateString()))
                  .ToList();

            if (debug) Console.WriteLine($"[大包裝儲位] {儲位_大包裝.Count} 筆");
            if (debug) Console.WriteLine($"[單包裝儲位] {儲位_單包裝.Count} 筆");

            void 處理異動量(List<object[]> 儲位清單, string 類型)
            {
                for (int i = 0; i < 儲位清單.Count; i++)
                {
                    double 庫存數量 = 儲位清單[i][(int)enum_儲位資訊.庫存].ObjectToString().StringToDouble();
                    double 包裝量 = 儲位清單[i][(int)enum_儲位資訊.包裝量].ObjectToString().StringToDouble();
                    string IP = 儲位清單[i][(int)enum_儲位資訊.IP].ObjectToString();
                    string 效期 = 儲位清單[i][(int)enum_儲位資訊.效期].ObjectToString();

                    if (包裝量 <= 0) 包裝量 = 1;

                    double 可用包數 = (double)(庫存數量 / 包裝量);
                    double 需異動包數 = (double)(Math.Abs(使用數量) / 包裝量);
                    //if (需異動包數 == 0 && Math.Abs(使用數量) > 0) 需異動包數 = 1;

                    if ((使用數量 < 0 && 可用包數 > 0) || (使用數量 > 0 && 庫存數量 >= 0))
                    {
                        double 實際異動包數 = Math.Min(可用包數, 需異動包數);
                        double 異動量實值 = 實際異動包數 * 包裝量 * (使用數量 > 0 ? 1 : -1);

                        if (實際異動包數 > 0)
                        {
                            儲位清單[i][(int)enum_儲位資訊.異動量] = 異動量實值.ToString("0.#####");
                            儲位資訊_buf.Add(儲位清單[i]);

                            if (debug) Console.WriteLine($"[{類型}] IP={IP}, 效期={效期}, 包裝量={包裝量}, 庫存={庫存數量}, 異動量={異動量實值}, 剩餘異動={使用數量 - 異動量實值}");

                            使用數量 -= 異動量實值;

                            if ((異動量 > 0 && 使用數量 <= 0) || (異動量 < 0 && 使用數量 >= 0))
                                break;
                        }
                    }
                }
            }

            處理異動量(儲位_大包裝, "大包裝處理");

            if ((異動量 > 0 && 使用數量 > 0) || (異動量 < 0 && 使用數量 < 0))
            {
                if (debug) Console.WriteLine($"[進入單包裝處理] 剩餘異動量={使用數量}");
                處理異動量(儲位_單包裝, "單包裝處理");
            }

            if ((異動量 > 0 && 使用數量 > 0) || (異動量 < 0 && 使用數量 < 0))
            {
                if (debug) Console.WriteLine($"[異動不足警告] 剩餘未完成異動量={使用數量}");
            }

            if (debug) Console.WriteLine($"[異動完成] 已異動儲位數={儲位資訊_buf.Count}");
            return 儲位資訊_buf;
        }
        static public void Function_從本地資料取得儲位(string 藥品碼, ref List<string> TYPE, ref List<object> values)
        {
            List<object> list_value = Function_從本地資料取得儲位(藥品碼);
            TYPE.Clear();
            values.Clear();
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    Device device = (Device)list_value[i];
                    values.Add(list_value[i]);
                    TYPE.Add(device.DeviceType.GetEnumName());
                }

            }
        }
        static public List<object> Function_從本地資料取得儲位(string 藥品碼)
        {
            List<object> list_value = new List<object>();

            // 使用 Task 執行每個集合的 SortByCode 操作
            var taskStorages = Task.Run(() => List_EPD266_本地資料.SortByCode(藥品碼));


            // 使用 Task.WaitAll 同步等待所有任務完成
            Task.WaitAll( taskStorages);

            // 將所有結果加入 list_value
            list_value.AddRange(taskStorages.Result); // storages


            return list_value;
        }

        static private DateTime TryParseDateTimeOrMax(string dateStr)
        {
            if (DateTime.TryParse(dateStr, out DateTime dt))
                return dt;
            return DateTime.MaxValue; // 無法解析的效期放在最後
        }
    }
}
