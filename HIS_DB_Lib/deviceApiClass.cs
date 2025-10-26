using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.Text.Json;
using H_Pannel_lib;
using System.Drawing;

namespace HIS_DB_Lib
{
    public class deviceApiClass
    {
        public enum StoreType
        {
            藥庫,
            藥局,
        }
        public enum DeviceType
        {
            WT32,
            EPD266,
            EPD290,
        }
        static public List<RowsLED> GetRowsLEDs(string API_Server, string ServerName, string ServerType)
        {
            string url = $"{API_Server}/api/device/get_rowLEDs";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            string tableName = "RowsLED_Jsonstring";

            returnData.TableName = tableName;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            List<RowsLED> rowsLEDs = returnData_result.Data.ObjToClass<List<RowsLED>>();
            rowsLEDs.Sort(new RowsLED.ICP_SortByIP());
            return rowsLEDs;
        }
        static public RowsLED GetRowsLED_ByIP(string API_Server, string ServerName, string ServerType , string IP)
        {
            string url = $"{API_Server}/api/device/get_rowLED_ByIP";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(IP);
            string tableName = "RowsLED_Jsonstring";

            returnData.TableName = tableName;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            RowsLED rowsLED = returnData_result.Data.ObjToClass<RowsLED>();
            return rowsLED;
        }
        static public List<RowsLED> GetRowsLED_By_Code(string API_Server, string ServerName, string ServerType, string Code)
        {
            string url = $"{API_Server}/api/device/get_rowLED_By_Code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(Code);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            List<RowsLED> rowsLEDs = returnData_result.Data.ObjToClass<List<RowsLED>>();
            return rowsLEDs;
        }
        static public List<DeviceBasic> GetRowsLED_DeviceBasic_By_Code(string API_Server, string ServerName, string ServerType, string Code)
        {
            string url = $"{API_Server}/api/device/get_rowLED_DeviceBasics_By_Code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(Code);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            List<DeviceBasic> deviceBasics = returnData_result.Data.ObjToClass<List<DeviceBasic>>();
            return deviceBasics;
        }
        static public void ReplaceRowsLED(string API_Server, string ServerName, string ServerType, RowsLED rowsLED)
        {
            List<RowsLED> rowsLEDs = new List<RowsLED>();
            rowsLEDs.Add(rowsLED);
            ReplaceRowsLED(API_Server, ServerName, ServerType, rowsLEDs);
        }
        static public void ReplaceRowsLED(string API_Server, string ServerName, string ServerType, List<RowsLED> rowsLEDs)
        {
            string url = $"{API_Server}/api/device/update_rowsLEDs";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.Data = rowsLEDs;
            string tableName = "RowsLED_Jsonstring";

            returnData.TableName = tableName;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return;
            }
            Console.WriteLine($"{returnData_result}");

        }

        static public List<Drawer> Get_EPD583_Drawers(string API_Server, string ServerName, string ServerType)
        {
            string url = $"{API_Server}/api/device/get_epd583_Drawers";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            List<Drawer> drawers = returnData_result.Data.ObjToClass<List<Drawer>>();
            return drawers;
        }
        static public Drawer Get_EPD583_Drawer_ByIP(string API_Server, string ServerName, string ServerType, string IP)
        {
            string url = $"{API_Server}/api/device/get_epd583_Drawer_ByIP";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(IP);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            Drawer drawer = returnData_result.Data.ObjToClass<Drawer>();
            return drawer;
        }
        static public List<Drawer> Get_EPD583_Drawer_By_Code(string API_Server, string ServerName, string ServerType, string Code)
        {
            string url = $"{API_Server}/api/device/get_epd583_Drawer_By_Code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(Code);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            List<Drawer> drawers = returnData_result.Data.ObjToClass<List<Drawer>>();
            return drawers;
        }
        static public List<DeviceBasic> Get_epd583_DeviceBasics_By_Code(string API_Server, string ServerName, string ServerType, string Code)
        {
            string url = $"{API_Server}/api/device/get_epd583_DeviceBasics_By_Code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(Code);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            List<DeviceBasic> DeviceBasices = returnData_result.Data.ObjToClass<List<DeviceBasic>>();
            return DeviceBasices;
        }
        static public void Replace_EPD583_Drawers(string API_Server, string ServerName, string ServerType, Drawer drawer)
        {
            List<Drawer> drawers = new List<Drawer>();
            drawers.Add(drawer);
            Replace_EPD583_Drawers(API_Server, ServerName, ServerType, drawers);
        }
        static public void Replace_EPD583_Drawers(string API_Server, string ServerName, string ServerType, List<Drawer> drawers)
        {
            string url = $"{API_Server}/api/device/update_epd583_Drawers";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.Data = drawers;


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return;
            }
            Console.WriteLine($"{returnData_result}");

        }
        /// <summary>
        /// 呼叫 API: 合併儲位 (CombineBoxes)
        /// </summary>
        /// <param name="API_Server">API 伺服器位址 (例如: http://127.0.0.1:5000)</param>
        /// <param name="ServerName">伺服器名稱</param>
        /// <param name="ServerType">伺服器類型</param>
        /// <param name="drawer">要進行合併的 Drawer 物件</param>
        /// <param name="selectColumns">要合併的欄位清單</param>
        /// <param name="selectRows">要合併的列清單</param>
        /// <returns>回傳更新後的 Drawer，如果失敗則回傳 null</returns>
        public static Drawer combine_drawer_boxes( string API_Server, string ServerName, string ServerType, Drawer drawer, List<int> selectColumns, List<int> selectRows)
        {
            string url = $"{API_Server}/api/device/combine_drawer_boxes";

            // 準備 returnData 結構
            returnData returnData = new returnData
            {
                ServerName = ServerName,
                ServerType = ServerType,
                Data = drawer,
                ValueAry = new List<string>
        {
            $"SelectColumns={string.Join(",", selectColumns)}",
            $"SelectRows={string.Join(",", selectRows)}"
        }
            };

            try
            {
                // JSON 輸入
                string json_in = returnData.JsonSerializationt();

                // 呼叫 API
                string json_out = Net.WEBApiPostJson(url, json_in);

                // 解析回傳
                returnData returnData_result = json_out.JsonDeserializet<returnData>();
                if (returnData_result.Code != 200)
                {
                    Console.WriteLine($"[CombineBoxes API Error] {returnData_result.Result}");
                    return null;
                }

                // 成功 → 轉換回 Drawer
                Drawer updatedDrawer = returnData_result.Data.ObjToClass<Drawer>();
                return updatedDrawer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CombineBoxes API Exception] {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// 呼叫 API: 分割儲位 (CombineBoxes)
        /// </summary>
        /// <param name="API_Server">API 伺服器位址 (例如: http://127.0.0.1:5000)</param>
        /// <param name="ServerName">伺服器名稱</param>
        /// <param name="ServerType">伺服器類型</param>
        /// <param name="drawer">要進行合併的 Drawer 物件</param>
        /// <param name="selectColumns">要合併的欄位清單</param>
        /// <param name="selectRows">要合併的列清單</param>
        /// <returns>回傳更新後的 Drawer，如果失敗則回傳 null</returns>
        public static Drawer separate_drawer_boxes(string API_Server, string ServerName, string ServerType, Drawer drawer, List<int> selectColumns, List<int> selectRows)
        {
            string url = $"{API_Server}/api/device/separate_drawer_boxes";

            // 準備 returnData 結構
            returnData returnData = new returnData
            {
                ServerName = ServerName,
                ServerType = ServerType,
                Data = drawer,
                ValueAry = new List<string>
        {
            $"SelectColumns={string.Join(",", selectColumns)}",
            $"SelectRows={string.Join(",", selectRows)}"
        }
            };

            try
            {
                // JSON 輸入
                string json_in = returnData.JsonSerializationt();

                // 呼叫 API
                string json_out = Net.WEBApiPostJson(url, json_in);

                // 解析回傳
                returnData returnData_result = json_out.JsonDeserializet<returnData>();
                if (returnData_result.Code != 200)
                {
                    Console.WriteLine($"[CombineBoxes API Error] {returnData_result.Result}");
                    return null;
                }

                // 成功 → 轉換回 Drawer
                Drawer updatedDrawer = returnData_result.Data.ObjToClass<Drawer>();
                return updatedDrawer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CombineBoxes API Exception] {ex.Message}");
                return null;
            }
        }

        static public List<Storage> Get_EPD266_Storage(string API_Server, string ServerName, string ServerType)
        {
            string url = $"{API_Server}/api/device/get_epd266_storage";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            List<Storage> storages = returnData_result.Data.ObjToClass<List<Storage>>();
            return storages;
        }
        static public List<Storage> Get_EPD266_storage_By_Code(string API_Server, string ServerName, string ServerType, string Code)
        {
            string url = $"{API_Server}/api/device/get_epd266_storage_By_Code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(Code);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            List<Storage> storages = returnData_result.Data.ObjToClass<List<Storage>>();
            return storages;
        }
        static public List<DeviceBasic> Get_EPD266_DeviceBasics_By_Code(string API_Server, string ServerName, string ServerType, string Code)
        {
            string url = $"{API_Server}/api/device/get_epd266_DeviceBasics_By_Code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(Code);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            List<DeviceBasic> DeviceBasices = returnData_result.Data.ObjToClass<List<DeviceBasic>>();
            return DeviceBasices;
        }
        static public Storage Get_EPD266_Storage_ByIP(string API_Server, string ServerName, string ServerType, string IP)
        {
            string url = $"{API_Server}/api/device/get_epd266_storage_ByIP";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(IP);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            Storage storage = returnData_result.Data.ObjToClass<Storage>();
            return storage;
        }
        static public void Replace_EPD266_Storage(string API_Server, string ServerName, string ServerType, Storage storage)
        {
            List<Storage> storages = new List<Storage>();
            storages.Add(storage);
            Replace_EPD266_Storage(API_Server, ServerName, ServerType, storages);
        }
        static public void Replace_EPD266_Storage(string API_Server, string ServerName, string ServerType, List<Storage> storages)
        {
            string url = $"{API_Server}/api/device/update_epd266_storages";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.Data = storages;


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return;
            }
            Console.WriteLine($"{returnData_result}");

        }

        static public List<Storage> Get_Panel35_Storage(string API_Server, string ServerName, string ServerType)
        {
            string url = $"{API_Server}/api/device/get_Panel35_storage";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            List<Storage> storages = returnData_result.Data.ObjToClass<List<Storage>>();
            return storages;
        }
        static public List<Storage> Get_Panel35_storage_By_Code(string API_Server, string ServerName, string ServerType, string Code)
        {
            string url = $"{API_Server}/api/device/get_Panel35_storage_By_Code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(Code);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            List<Storage> storages = returnData_result.Data.ObjToClass<List<Storage>>();
            return storages;
        }
        static public List<DeviceBasic> Get_Panel35_DeviceBasics_By_Code(string API_Server, string ServerName, string ServerType, string Code)
        {
            string url = $"{API_Server}/api/device/get_Panel35_DeviceBasics_By_Code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(Code);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            List<DeviceBasic> DeviceBasices = returnData_result.Data.ObjToClass<List<DeviceBasic>>();
            return DeviceBasices;
        }
        static public Storage Get_Panel35_Storage_ByIP(string API_Server, string ServerName, string ServerType, string IP)
        {
            string url = $"{API_Server}/api/device/get_Panel35_storage_ByIP";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(IP);


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            Console.WriteLine($"{returnData_result}");
            string jsonStr = returnData_result.Value;
            Storage storage = returnData_result.Data.ObjToClass<Storage>();
            return storage;
        }
        static public void Replace_Panel35_Storage(string API_Server, string ServerName, string ServerType, Storage storage)
        {
            List<Storage> storages = new List<Storage>();
            storages.Add(storage);
            Replace_Panel35_Storage(API_Server, ServerName, ServerType, storages);
        }
        static public void Replace_Panel35_Storage(string API_Server, string ServerName, string ServerType, List<Storage> storages)
        {
            string url = $"{API_Server}/api/device/update_Panel35_storages";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.Data = storages;


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return;
            }
            Console.WriteLine($"{returnData_result}");

        }

        static public void light_on_by_code(string API_Server, string ServerName, string ServerType, List<string> Codes, string str_color)
        {
            string url = $"{API_Server}/api/device/light_on_by_code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            string str_codes = "";

            for(int i = 0; i < Codes.Count; i++)
            {
                str_codes += Codes;
                if (i < Codes.Count - 1) str_codes += ",";
            }
            returnData.ValueAry.Add(str_codes);
            returnData.ValueAry.Add(str_color);

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return;
            }
            Console.WriteLine($"{returnData_result}");
        }

        static public List<DeviceBasic> Get_Pharma_DeviceBasicsByCode(string API_Server, string ServerName, string ServerType, string Code)
        {
            string url = $"{API_Server}/api/device/get_from_pharma_by_code";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.ValueAry.Add(Code);

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return new List<DeviceBasic>();
            }
            Console.WriteLine($"{returnData_result}");

            string jsonStr = returnData_result.Value;
            List<DeviceBasic> deviceBasics = returnData_result.Data.ObjToClass<List<DeviceBasic>>();
            return deviceBasics;
        }

        static public List<Device> GetDevice(string API_Server, string ServerName, string ServerType, DeviceType deviceType)
        {
            string url = $"{API_Server}/api/device/get_device";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            string tableName = "";

            if (deviceType == DeviceType.WT32)
            {
                tableName = "WT32_Jsonstring";
            }
            if (deviceType == DeviceType.EPD266)
            {
                tableName = "EPD266_Jsonstring";
            }
            if (deviceType == DeviceType.EPD290)
            {
                tableName = "EPD290_Jsonstring";
            }
         
            returnData.TableName = tableName;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            Console.WriteLine($"{returnData_result}");
            List<Device> devices = returnData_result.Data.ObjToClass<List<Device>>();
            return devices;
        }


        static public List<DeviceBasic> GetDeviceBasics(string API_Server, string ServerName, string ServerType, StoreType storeType)
        {
            string url = $"{API_Server}/api/device/all";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            string tableName = "";
            if (storeType == StoreType.藥庫)
            {
                tableName = "firstclass_device_jsonstring";
            }
            if (storeType == StoreType.藥局)
            {
                tableName = "sd0_device_jsonstring";
            }

            returnData.TableName = tableName;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            Console.WriteLine($"{returnData_result}");
            List<DeviceBasic> deviceBasics = returnData_result.Data.ObjToClass<List<DeviceBasic>>();
            return deviceBasics;
        }
        static public void SetDeviceBasics(string API_Server, string ServerName, string ServerType, StoreType storeType , List<DeviceBasic> deviceBasics)
        {
            string url = $"{API_Server}/api/device/update_deviceBasic";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            string tableName = "";
            if (storeType == StoreType.藥庫)
            {
                tableName = "firstclass_device_jsonstring";
            }
            if (storeType == StoreType.藥局)
            {
                tableName = "sd0_device_jsonstring";
            }
            returnData.Data = deviceBasics;
            returnData.TableName = tableName;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return;
            }
            Console.WriteLine($"{returnData_result}");
        }

        static public List<DeviceBasic> Get_StoreHouse_DeviceBasicsByCodes(string API_Server, string ServerName, string ServerType, string Code, StoreType storeType)
        {
            List<string> Codes = new List<string>();
            Codes.Add(Code);
            return Get_StoreHouse_DeviceBasicsByCodes(API_Server, ServerName, ServerType, Codes , storeType);
        }
        static public List<DeviceBasic> Get_StoreHouse_DeviceBasicsByCodes(string API_Server, string ServerName, string ServerType, List<string> Codes, StoreType storeType)
        {
            string url = $"{API_Server}/api/device/get_form_storehouse_by_codes";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            string text = "";
            for (int i = 0; i < Codes.Count; i++)
            {
                text += Codes[i];
                if (i != Codes.Count - 1) text += ",";
            }
            returnData.ValueAry.Add(text);
            string tableName = "";
            if (storeType == StoreType.藥庫)
            {
                tableName = "firstclass_device_jsonstring";
            }
            if (storeType == StoreType.藥局)
            {
                tableName = "sd0_device_jsonstring";
            }

            returnData.TableName = tableName;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            Console.WriteLine($"{returnData_result}");
            List<DeviceBasic> deviceBasics = returnData_result.Data.ObjToClass<List<DeviceBasic>>();
            return deviceBasics;
        }
        static public List<DeviceBasic> Get_dps_med(string API_Server, string ServerName)
        {
            string url = $"{API_Server}/api/device/list/{ServerName}";          
            string json_out = Net.WEBApiGet(url);
            //string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_result = json_out.JsonDeserializet<returnData>();
            if (returnData_result.Code != 200)
            {
                return null;
            }
            Console.WriteLine($"{returnData_result}");
            List<DeviceBasic> deviceBasics = returnData_result.Data.ObjToClass<List<DeviceBasic>>();
            return deviceBasics;
        }
        static public async Task<returnData> light_action(string API_Server, List<string> strings)
        {
            string url = $"{API_Server}/api/device_control/light_action";

            returnData returnData = new returnData();
            returnData.ValueAry = strings;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData =  json_out.JsonDeserializet<returnData>();
            return returnData;
        }

        /// <summary>
        /// 呼叫 API 以藥碼亮燈（只回傳是否成功）
        /// </summary>
        static public bool light_by_drugCodes(string API_Server, List<(string 藥碼, string 顏色, string 秒數)> lightList, string serverName = "", string serverType = "")
        {
            var (code, result) = light_by_drugCodes_full(API_Server, lightList, serverName, serverType);
            return code == 200;
        }
        /// <summary>
        /// 呼叫 API 以藥碼亮燈（回傳完整結果）
        /// </summary>
        static public (int code, string result) light_by_drugCodes_full(string API_Server, List<(string 藥碼, string 顏色, string 秒數)> lightList, string serverName = "", string serverType = "")
        {
            string url = $"{API_Server}/api/device/light_by_drugCodes";

            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;

            // 組成 ValueAry，每列為 [藥碼, 顏色, 秒數]
            returnData.ValueAry = lightList
                .Select(x => new List<string> { x.藥碼, x.顏色, x.秒數 }.JsonSerializationt())
                .ToList();

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return (0, "回傳為 null");
            }

            return (returnData_out.Code, returnData_out.Result);
        }
        /// <summary>
        /// 呼叫 API 以 IP 清單刷新設備面板（只回傳是否成功）
        /// </summary>
        static public bool refresh_canvas_by_ip(string API_Server, List<string> ipList, string serverName = "", string serverType = "")
        {
            var (code, result) = refresh_canvas_by_ip_full(API_Server, ipList, serverName, serverType);
            return code == 200;
        }
        /// <summary>
        /// 呼叫 API 以 IP 清單刷新設備面板（回傳完整結果）
        /// </summary>
        static public (int code, string result) refresh_canvas_by_ip_full(string API_Server, List<string> ipList, string serverName = "", string serverType = "")
        {
            string url = $"{API_Server}/api/device/refresh_canvas_by_ip";

            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;

            // 將 IP 清單轉為 ValueAry 格式：List<string[]> → 每筆為 ["192.168.0.10"]
            returnData.ValueAry = ipList
                .Select(ip => new List<string> { ip }.JsonSerializationt())
                .ToList();

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return (0, "回傳為 null");
            }

            return (returnData_out.Code, returnData_out.Result);
        }
        /// <summary>
        /// 呼叫 API 以藥碼刷新設備面板（只回傳是否成功）
        /// </summary>
        static public bool refresh_canvas_by_drugCodes(string API_Server, List<(string 藥碼, string 延遲秒數)> drugList, string serverName = "", string serverType = "")
        {
            var (code, result) = refresh_canvas_by_drugCodes_full(API_Server, drugList, serverName, serverType);
            return code == 200;
        }
        /// <summary>
        /// 呼叫 API 以藥碼刷新設備面板（回傳完整結果）
        /// </summary>
        static public (int code, string result) refresh_canvas_by_drugCodes_full(string API_Server, List<(string 藥碼, string 延遲秒數)> drugList, string serverName = "", string serverType = "")
        {
            string url = $"{API_Server}/api/refresh_canvas_by_drugCodes";

            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;

            // 組成 ValueAry 格式：每列為 ["藥碼", "延遲秒數"]
            returnData.ValueAry = drugList
                .Select(x => new List<string> { x.藥碼, x.延遲秒數 }.JsonSerializationt())
                .ToList();

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return (0, "回傳為 null");
            }

            return (returnData_out.Code, returnData_out.Result);
        }

    }
}
