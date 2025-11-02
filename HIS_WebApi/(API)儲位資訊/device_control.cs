using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Drawing;
using System.Text;
using System.Configuration;
using MyOffice;
using MyUI;
using H_Pannel_lib;
using HIS_DB_Lib;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class device_control : Controller
    {
        public static string LabelRenderer_Url = "http://127.0.0.1:8700/render";

        private int rowsLED_port = 29001;
        private int EPD_port = 29000;
        public enum DeviceType
        {
            RowsLED,
            EPD213_3Color,
            EPD213_4Color,
            EPD290_3Color,
            EPD290_4Color,
            EPD420_3Color,
            EPD420_4Color,
            EPD360_6Color,
            EPD583_3Color,
            EPD700_6Color,
            EPD730_7Color,
        }
        private static List<RowsLED> rowsLEDs = new List<RowsLED>();
        private static List<Storage> storages = new List<Storage>();


        /// <summary>
        /// 觸發指定裝置的燈號動作 (RowsLED / EPD)
        /// </summary>
        /// <remarks>
        /// ## 📌 用途  
        /// 本 API 用於控制指定 IP 裝置的燈號顯示行為，可支援：
        /// - **RowsLED** → 控制燈條中指定編號範圍的燈號顏色  
        /// - **EPD 類型裝置** → 控制整體燈號顏色  
        ///
        /// ## ⚙️ 功能說明  
        /// 系統依據 `device_type` 決定控制方式：  
        /// - 若為 <c>RowsLED</c>，會以指定區間 (`start_num`~`end_num`) 控制燈條段落顏色。  
        /// - 若為 <c>EPD</c>，則控制整體亮燈顏色。  
        /// 若裝置不存在，系統會自動建立物件實例。  
        ///
        /// ## 📥 Request JSON 範例
        /// ```json
        /// {
        ///   "Method": "light_action",
        ///   "ValueAry": [
        ///     "ip=192.168.1.50",
        ///     "start_num=0",
        ///     "end_num=20",
        ///     "color=255,0,0",
        ///     "lightness=0.9",
        ///     "device_type=RowsLED"
        ///   ],
        ///   "Data": {}
        /// }
        /// ```
        ///
        /// ## 🔍 參數說明  
        /// | 參數名稱 | 類型 | 必填 | 說明 |
        /// |------------|------|------|------|
        /// | ip | string | ✅ | 裝置 IP，需符合 IPv4 格式 |
        /// | start_num | string | ✅ (RowsLED 時) | 起始燈號編號 (≥0) |
        /// | end_num | string | ✅ (RowsLED 時) | 結束燈號編號 (≥0，且不得小於 start_num) |
        /// | color | string | ✅ | 燈號顏色，格式為 `"B,G,R"` (例如 `"255,0,0"` 代表紅色) |
        /// | lightness | string | ✅ | 亮度0.9(90%) |
        /// | device_type | string | ✅ | 裝置類型，可為 `"RowsLED"` 或 `"EPDXXXX"` 類型 |
        ///
        /// **顏色格式說明**  
        /// 由於系統內部的 `ToColor()` 函式採用 `B,G,R` 順序：  
        /// ```csharp
        /// string[] array_color = item.Split(',');
        /// B = array_color[0]; G = array_color[1]; R = array_color[2];
        /// ```
        /// 因此：  
        /// - `"255,0,0"` → 紅色  
        /// - `"0,255,0"` → 綠色  
        /// - `"0,0,255"` → 藍色  
        ///
        /// ## 📤 Response JSON 範例 (成功)
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Method": "light_action",
        ///   "Result": "裝置觸發成功",
        ///   "TimeTaken": "35ms"
        /// }
        /// ```
        ///
        /// ## ❌ Response JSON 範例 (錯誤)
        /// - IP 格式錯誤：  
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "ip 檢核失敗"
        /// }
        /// ```
        ///
        /// - 參數錯誤：  
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "請輸入有效的 start_num,end_num"
        /// }
        /// ```
        ///
        /// - 觸發失敗：  
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "裝置觸發失敗"
        /// }
        /// ```
        ///
        /// - 系統例外：  
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "Exception : 傳輸逾時"
        /// }
        /// ```
        ///
        /// ## 📑 注意事項  
        /// - 若傳入 <c>RowsLED</c> 類型，必須指定 <c>start_num</c> 與 <c>end_num</c>。  
        /// - 若為 <c>EPD</c> 類型，僅需指定顏色即可。  
        /// - 系統自動建立裝置實例並快取在記憶體中。  
        /// - <c>color</c> 格式錯誤或 RGB 任一項為 -1 時，將使用預設黑色。  
        /// </remarks>
        /// <param name="returnData">統一封裝的 API 請求資料物件，包含 ValueAry 參數</param>
        /// <returns>JSON 格式的回應字串，描述裝置亮燈結果</returns>
        [Route("light_action")]
        [HttpPost]
        public string light_action(returnData returnData)
        {
            try
            {
                returnData.Method = "light_action";
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                // 解析參數
                string GetVal(string key) =>
                   returnData.ValueAry.FirstOrDefault(x => x.StartsWith($"{key}=", StringComparison.OrdinalIgnoreCase))
                    ?.Split('=')[1];
                string ip = GetVal("ip") ?? "";
                string start_num = GetVal("start_num") ?? "";
                string end_num = GetVal("end_num") ?? "";
                string color = GetVal("color") ?? "";
                string lightness = GetVal("lightness") ?? "";
                string device_type = GetVal("device_type") ?? "";
                double _lightness = 0.9;
                if (lightness.StringIsDouble()) _lightness = lightness.StringToDouble();
                if (ip.Check_IP_Adress() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ip 檢核失敗";
                    return returnData.JsonSerializationt(true);
                }
                if (device_type.ToLower() == DeviceType.RowsLED.GetEnumName().ToLower())
                {
                    UDP_Class uDP_Class = new UDP_Class(ip, rowsLED_port, false);
                    RowsLED rowsLED = rowsLEDs.Where(x => x.IP == ip).FirstOrDefault();
                    if (rowsLED == null)
                    {
                        rowsLED = new RowsLED(ip, rowsLED_port);
                        rowsLEDs.Add(rowsLED);
                    }
                    int _start_num = start_num.StringToInt32();
                    int _end_num = end_num.StringToInt32();
                    if (_start_num < 0 || _end_num < 0)
                    {
                        returnData.Code = -200;
                        returnData.TimeTaken = myTimerBasic.ToString();
                        returnData.Result = $"請輸入有效的 start_num,end_num";
                        return returnData.JsonSerializationt(true);
                    }
                    if(_start_num > _end_num)
                    {
                        returnData.Code = -200;
                        returnData.TimeTaken = myTimerBasic.ToString();
                        returnData.Result = $"start_num 不得大於 end_num";
                        return returnData.JsonSerializationt(true);
                    }
                    rowsLED.LED_Bytes = Get_Rows_LEDBytes(ref rowsLED.LED_Bytes, start_num.StringToInt32(), end_num.StringToInt32(), color.ToColor(), _lightness);
                    bool flag = Communication.Set_WS2812_Buffer(uDP_Class, rowsLED.IP, 0, rowsLED.LED_Bytes);

                    if (flag == false)
                    {
                        returnData.Code = -200;
                        returnData.TimeTaken = myTimerBasic.ToString();
                        returnData.Result = $"裝置觸發失敗";
                        return returnData.JsonSerializationt(true);
                    }
                    else
                    {
                        returnData.Code = 200;
                        returnData.TimeTaken = myTimerBasic.ToString();
                        returnData.Result = $"裝置觸發成功";
                        return returnData.JsonSerializationt(true);
                    }
                }
                else if(device_type.Contains("EPD") == true)
                {
                    UDP_Class uDP_Class = new UDP_Class(ip, EPD_port, false);
                    Storage storage = storages.Where(x => x.IP == ip).FirstOrDefault();
                    if (storage == null)
                    {
                        storage = new Storage(ip, EPD_port);
                        storages.Add(storage);
                    }
                    storage.LED_Bytes = Get_Storage_LEDBytes(ref storage.LED_Bytes, color.ToColor(), _lightness);
                    bool flag = Communication.Set_WS2812_Buffer(uDP_Class, storage.IP, 0, storage.LED_Bytes);
                    if (flag == false)
                    {
                        returnData.Code = -200;
                        returnData.TimeTaken = myTimerBasic.ToString();
                        returnData.Result = $"裝置觸發失敗";
                        return returnData.JsonSerializationt(true);
                    }
                    else
                    {
                        returnData.Code = 200;
                        returnData.TimeTaken = myTimerBasic.ToString();
                        returnData.Result = $"裝置觸發成功";
                        return returnData.JsonSerializationt(true);
                    }
                }
                else
                {
                    returnData.Code = 200;
                    returnData.Result = $"請傳入有效 device_type";
                    return returnData.JsonSerializationt(true);
                }
                
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }       
        }


        [Route("print_paper")]
        [HttpPost]
        public string print_paper(returnData returnData)
        {
            try
            {
                returnData.Method = "print_paper";
                MyTimerBasic myTimerBasic = new MyTimerBasic();


                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting("220.135.128.247");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                // 解析參數
                string GetVal(string key) =>
                   returnData.ValueAry.FirstOrDefault(x => x.StartsWith($"{key}=", StringComparison.OrdinalIgnoreCase))
                    ?.Split('=')[1];
                string ip = GetVal("ip") ?? "";
                if (ip.Check_IP_Adress() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ip 檢核失敗";
                    return returnData.JsonSerializationt(true);
                }

                string tableName = "epd266_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, MySqlSslMode.None);
                Storage storage = StorageMethod.SQL_GetStorageByIP(sQLControl_device, ip);

                if (storage == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.Data = storage;
                string json_in = returnData.JsonSerializationt();
                string json_out = Basic.Net.WEBApiPostJson(LabelRenderer_Url, json_in);

                return returnData.JsonSerializationt(true);
          

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }

        static public byte[] Get_Rows_Empty_LEDBytes()
        {
            return new byte[250 * 3];
        }
  
        static public byte[] Get_Rows_LEDBytes(ref byte[] LED_Bytes, int startNum, int EndNum, Color color , double _lightness)
        {
            for (int i = startNum; i < EndNum; i++)
            {
                if (i > LED_Bytes.Length) break;
                LED_Bytes[i * 3 + 0] = (byte)(color.R * _lightness);
                LED_Bytes[i * 3 + 1] = (byte)(color.G * _lightness);
                LED_Bytes[i * 3 + 2] = (byte)(color.B * _lightness);
            }
            return LED_Bytes;
        }

        static public byte[] Get_Storage_Empty_LEDBytes()
        {
            return new byte[10 * 3];
        }

        static public byte[] Get_Storage_LEDBytes(ref byte[] LED_Bytes, Color color, double _lightness)
        {
            for (int i = 0; i < LED_Bytes.Length / 3; i++)
            {
                if (i > LED_Bytes.Length) break;
                LED_Bytes[i * 3 + 0] = (byte)(color.R * _lightness);
                LED_Bytes[i * 3 + 1] = (byte)(color.G * _lightness);
                LED_Bytes[i * 3 + 2] = (byte)(color.B * _lightness);
            }
            return LED_Bytes;
        }
    }
}
