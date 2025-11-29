using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        public enum enum_儲位資訊
        {
            IP,
            TYPE,
            效期,
            批號,
            庫存,
            異動量,
            Value,
        }
        static public List<Device> Function_取得本地儲位()
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(Task.Run(new Action(delegate
            {
                List_EPD266_本地資料 = _storageUI_EPD_266.SQL_GetAllStorage();
            })));

            tasks.Add(Task.Run(new Action(delegate
            {
                List_RowsLED_本地資料 = _rowsLEDUI.SQL_GetAllRowsLED();
            })));

            Task.WhenAll(tasks).Wait();
            List<Device> devices = new List<Device>();
            devices.LockAdd(List_EPD266_本地資料.GetAllDevice());
            devices.LockAdd(List_RowsLED_本地資料.GetAllDevice());


            return devices;
        }
        static public double Function_從SQL取得庫存(string 藥品碼)
        {
            int 庫存 = 0;
            List<object> list_value = Function_從SQL取得儲位到本地資料(藥品碼);
            for (int i = 0; i < list_value.Count; i++)
            {

                if (list_value[i] is Device)
                {
                    Device device = list_value[i] as Device;
                    if (device != null)
                    {
                        庫存 += device.Inventory.StringToInt32();
                    }
                }

            }
            return 庫存;
        }
        static public List<object> Function_從SQL取得儲位到本地資料(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            List<Storage> storages = List_EPD266_本地資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_本地資料.SortByCode(藥品碼);

     

            for (int i = 0; i < rowsDevices.Count; i++)
            {
                RowsDevice rowsDevice = _rowsLEDUI.SQL_GetRowsDevice(rowsDevices[i]);
                List_RowsLED_本地資料.Add_NewRowsLED(rowsDevice);
                list_value.Add(rowsDevice);
            }
            for (int i = 0; i < storages.Count; i++)
            {
                Storage storage = _storageUI_EPD_266.SQL_GetStorage(storages[i]);
                List_EPD266_本地資料.Add_NewStorage(storage);
                list_value.Add(storage);
            }

            return list_value;
        }

        static public List<object[]> Function_取得異動儲位資訊從本地資料(List<StockClass> stockClasses, ref List<object[]> list_value_常溫, ref List<object[]> list_value_冷藏)
        {
            Function_取得本地儲位();

            List<object[]> list_馬達輸出索引表 = _sqL_DataGridView_馬達輸出索引表.SQL_GetAllRows(false);
            List<object[]> list_馬達輸出索引表_buf = new List<object[]>();
            List<object[]> list_value = new List<object[]>();
            List<object[]> list_value_buf = new List<object[]>();
            for (int i = 0; i < stockClasses.Count; i++)
            {
                list_value_buf.LockAdd(Function_取得異動儲位資訊從本地資料(stockClasses[i].Code, stockClasses[i].Qty.StringToInt32()));
            }

            for (int i = 0; i < list_value_buf.Count; i++)
            {
 
                int qty = list_value_buf[i][(int)enum_儲位資訊.異動量].StringToInt32();
                if (qty < 0) qty = qty * -1;
                for (int k = 0; k < qty; k++)
                {
                    object[] value = new object[new enum_儲位資訊().GetLength()];
                    value[(int)enum_儲位資訊.IP] = list_value_buf[i][(int)enum_儲位資訊.IP];
                    value[(int)enum_儲位資訊.TYPE] = list_value_buf[i][(int)enum_儲位資訊.TYPE];
                    value[(int)enum_儲位資訊.效期] = list_value_buf[i][(int)enum_儲位資訊.效期];
                    value[(int)enum_儲位資訊.批號] = list_value_buf[i][(int)enum_儲位資訊.批號];
                    value[(int)enum_儲位資訊.庫存] = list_value_buf[i][(int)enum_儲位資訊.IP];
                    value[(int)enum_儲位資訊.異動量] = "-1";
                    value[(int)enum_儲位資訊.Value] = list_value_buf[i][(int)enum_儲位資訊.Value];
                    list_value.Add(value);
                }
            }


            for(int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_儲位資訊.IP].ObjectToString();
                list_馬達輸出索引表_buf = list_馬達輸出索引表.GetRows((int)enum_storageMedBoxIOConfig.IP, IP);
                if(list_馬達輸出索引表_buf.Count > 0)
                {
                    string 區域 = list_馬達輸出索引表_buf[0][(int)enum_storageMedBoxIOConfig.區域].ObjectToString();
                    if(區域 == "常溫")
                    {
                        list_value_常溫.Add(list_value[i]);
                    }
                    if (區域 == "冷藏")
                    {
                        list_value_冷藏.Add(list_value[i]);
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
            return list_value;
        }
        static public List<object[]> Function_取得異動儲位資訊從本地資料(string 藥品碼, int 異動量)
        {
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從本地資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref 儲位);
            List<object[]> 儲位資訊_buf = new List<object[]>();
            List<object[]> 儲位資訊 = new List<object[]>();
            if (儲位.Count == 0) return 儲位資訊_buf;


            for (int k = 0; k < 儲位.Count; k++)
            {
                object value_device = 儲位[k];
                if (value_device is Device)
                {
                    Device device = (Device)value_device;
                    for (int i = 0; i < device.List_Validity_period.Count; i++)
                    {
                        object[] value = new object[new enum_儲位資訊().GetLength()];
                        value[(int)enum_儲位資訊.IP] = device.IP;
                        value[(int)enum_儲位資訊.TYPE] = 儲位_TYPE[k];
                        value[(int)enum_儲位資訊.效期] = device.List_Validity_period[i];
                        value[(int)enum_儲位資訊.批號] = device.List_Lot_number[i];
                        value[(int)enum_儲位資訊.庫存] = device.List_Inventory[i];
                        value[(int)enum_儲位資訊.異動量] = "0";
                        value[(int)enum_儲位資訊.Value] = value_device;
                        儲位資訊.Add(value);
                    }
                }
            }
            儲位資訊 = 儲位資訊.OrderBy(r => DateTime.Parse(r[(int)enum_儲位資訊.效期].ToDateString())).ToList();

            if (異動量 == 0) return 儲位資訊;
            int 使用數量 = 異動量;
            int 庫存數量 = 0;
            int 剩餘庫存數量 = 0;
            for (int i = 0; i < 儲位資訊.Count; i++)
            {
                庫存數量 = 儲位資訊[i][(int)enum_儲位資訊.庫存].ObjectToString().StringToInt32();
                if ((使用數量 < 0 && 庫存數量 > 0) || (使用數量 > 0 && 庫存數量 >= 0))
                {
                    剩餘庫存數量 = 庫存數量 + 使用數量;
                    if (剩餘庫存數量 >= 0)
                    {
                        儲位資訊[i][(int)enum_儲位資訊.異動量] = (使用數量).ToString();
                        儲位資訊_buf.Add(儲位資訊[i]);
                        break;
                    }
                    else
                    {
                        儲位資訊[i][(int)enum_儲位資訊.異動量] = (庫存數量 * -1).ToString();
                        使用數量 = 剩餘庫存數量;
                        儲位資訊_buf.Add(儲位資訊[i]);
                    }
                }
            }

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
            List<Storage> storages = List_EPD266_本地資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_本地資料.SortByCode(藥品碼);
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                list_value.Add(rowsDevices[i]);
            }
            for (int i = 0; i < storages.Count; i++)
            {
                list_value.Add(storages[i]);
            }
           
            return list_value;
        }
        static public object Function_庫存異動至本地資料(object[] 儲位資訊, bool upToSQL)
        {
            object Value = 儲位資訊[(int)enum_儲位資訊.Value];
            string 效期 = 儲位資訊[(int)enum_儲位資訊.效期].ObjectToString();
            string 異動量 = 儲位資訊[(int)enum_儲位資訊.異動量].ObjectToString();
            string TYPE = 儲位資訊[(int)enum_儲位資訊.TYPE].ObjectToString();
            if (Value is Storage)
            {
                if (TYPE == DeviceType.EPD266.GetEnumName() || TYPE == DeviceType.EPD266_lock.GetEnumName() || TYPE == DeviceType.EPD290.GetEnumName() || TYPE == DeviceType.EPD290_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = _storageUI_EPD_266.SQL_GetStorage(storage);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, false);
                        List_EPD266_本地資料.Add_NewStorage(storage);
                        if (upToSQL) _storageUI_EPD_266.SQL_ReplaceStorage(storage);
                        storage.UpToSQL = true;
                        return storage;
                    }
                }

            }
            
            return null;
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
            
                        taskList.Add(Task.Run(() =>
                        {
                            this.rowsLEDUI.Set_Rows_LED_UDP(rowsLED);

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

        static private List<medClass> Function_取得藥檔資料()
        {
            string url = $"{API_Server}/api/MED_page/get_by_apiserver";
            returnData returnData = new returnData();
            returnData.ServerType = enum_sys_serverSetting_Type.癌症備藥機.GetEnumName();
            returnData.ServerName = $"{dBConfigClass.Name}";
            returnData.TableName = "medicine_page";
            string json_in = returnData.JsonSerializationt();
            string json = Basic.Net.WEBApiPostJson($"{url}", json_in);
            returnData returnData_result = json.JsonDeserializet<returnData>();
            List<medClass> medClasses = returnData_result.Data.ObjToClass<List<medClass>>();

            return medClasses;
        }
        static public List<medClass> Function_取得有儲位藥檔資料()
        {
            List<medClass> medClasses = Function_取得藥檔資料();
            List<medClass> medClasses_buf = new List<medClass>();
            List<medClass> medClasses_result = new List<medClass>();
            List<Device> storages = Function_取得本地儲位();
            storages.Sort(new Icp_Storage());
            List<string> codes = (from temp in storages
                                  select temp.Code).Distinct().ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                medClasses_buf = (from temp in medClasses
                                  where temp.藥品碼 == codes[i]
                                  select temp).ToList();
                if (medClasses_buf.Count > 0)
                {
                    medClasses_result.Add(medClasses_buf[0]);
                }
            }

            return medClasses_result;
        }


        static public string Function_ReadBacodeScanner01()
        {
            string text = "";
            text = MySerialPort_Scanner01.ReadString();
            if (text == null) return null;

 
            text = text.Replace("\0", "");
            if (text.StringIsEmpty()) return null;
            if (text.Length <= 2 || text.Length > 300) return null;


            MySerialPort_Scanner01.ClearReadByte();
            text = text.Replace("\r\n", "");
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            Console.WriteLine($"[Scanner01]接收資料:{text}");
            return text;
        }
        static public string Function_ReadBacodeScanner02()
        {
            string text = "";
            text = MySerialPort_Scanner02.ReadString();
            if (text == null) return null;

      
            text = text.Replace("\0", "");
            if (text.StringIsEmpty()) return null;
            if (text.Length <= 2 || text.Length > 300) return null;
            MySerialPort_Scanner02.ClearReadByte();
            text = text.Replace("\r\n", "");
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            Console.WriteLine($"[Scanner02]接收資料:{text}");
            return text;
        }

        static public string Function_藥品碼檢查(string Code)
        {

            return Code;
        }
    }

    public class Icp_Storage : IComparer<Device>
    {
        public int Compare(Device x, Device y)
        {
            string 藥品碼_0 = x.Code;
            string 藥品碼_1 = y.Code;
            return 藥品碼_0.CompareTo(藥品碼_1);
        }
    }

}
