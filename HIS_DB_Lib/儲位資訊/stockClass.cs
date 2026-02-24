using Basic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HIS_DB_Lib
{
    /// <summary>
    /// 貨品表資料結構  
    /// 用於紀錄藥品在各層架或裝置上的位置、批號、效期與數量。
    /// </summary>
    [Description("stock")]
    public class stockClass
    {
        /// <summary>
        /// 唯一識別碼 (GUID)
        /// </summary>
        [Description("GUID,VARCHAR,50,PRIMARY")]
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        /// <summary>
        /// 對應的層架 GUID
        /// </summary>
        [Description("shelf_GUID,VARCHAR,50,INDEX")]
        [JsonPropertyName("shelf_guid")]
        public string Shelf_GUID { get; set; }

        /// <summary>
        /// 位置描述 (例如 上層第2層第3格)
        /// </summary>
        [Description("位置,VARCHAR,100,NONE")]
        [JsonPropertyName("location")]
        public string 位置 { get; set; }

        /// <summary>
        /// 裝置 IP 位址
        /// </summary>
        [Description("IP,VARCHAR,50,NONE")]
        [JsonPropertyName("ip")]
        public string IP { get; set; }

        /// <summary>
        /// 裝置類型 (例如 shelf、drawer、cabinet)
        /// </summary>
        [Description("device_type,VARCHAR,50,NONE")]
        [JsonPropertyName("device_type")]
        public string device_type { get; set; }

        /// <summary>
        /// 燈條亮燈位置 (LED Index)
        /// </summary>
        [Description("燈條亮燈位置,VARCHAR,50,NONE")]
        [JsonPropertyName("led_index")]
        public string 燈條亮燈位置 { get; set; }

        /// <summary>
        /// 藥品代碼
        /// </summary>
        [Description("藥碼,VARCHAR,50,INDEX")]
        [JsonPropertyName("code")]
        public string 藥碼 { get; set; }

        /// <summary>
        /// 藥品名稱
        /// </summary>
        [Description("藥名,VARCHAR,200,NONE")]
        [JsonPropertyName("name")]
        public string 藥名 { get; set; }

        /// <summary>
        /// 料號 (Material Number)
        /// </summary>
        [Description("料號,VARCHAR,100,NONE")]
        [JsonPropertyName("material_no")]
        public string 料號 { get; set; }

        /// <summary>
        /// 批號 (Lot Number)
        /// </summary>
        [JsonPropertyName("lot")]
        public List<string> 批號 { get; set; }

        /// <summary>
        /// 效期 (Expiry Date)
        /// </summary>
        [JsonPropertyName("expiry_date")]
        public List<string> 效期 { get; set; }

        /// <summary>
        /// 數量 (Quantity)
        /// </summary>
        [JsonPropertyName("qty")]
        public List<string> 數量 { get; set; }
        /// <summary>
        /// Value (DeviceBasic)
        /// </summary>
        [Description("VARCHAR,1000,NONE")]
        public string Value { get; set; }
        /// <summary>
        /// Classify_GUID
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        public string Classify_GUID { get; set; }
        /// <summary>
        /// Classify
        /// </summary>
        public medClassifyClass Classify { get; set; }
        /// <summary>
        /// 雲端藥檔
        /// </summary>
        public medClass med_cloud { get; set; }
        /// <summary>
        /// 藥品單位
        /// </summary>
        public List<medUnitClass> med_unit { get; set; }
        /// <summary>
        /// 驗收項目
        /// </summary>
        public List<inspectionClass.content> content { get; set; }
        public string serverName { get; set; }
        public string serverType { get; set; }

        static public returnData get_stock(string API_Server, string serverName, string serverType)
        {
            string url = $"{API_Server}/api/stock/get_stock";

            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public returnData update(string API_Server, string serverName, string serverType, List<stockClass> stockClasses)
        {
            string url = $"{API_Server}/api/stock/update";

            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;
            returnData.Data = stockClasses;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public returnData add(string API_Server, string serverName, string serverType, List<stockClass> stockClasses)
        {
            string url = $"{API_Server}/api/stock/add";

            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;
            returnData.Data = stockClasses;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
    }
    /// <summary>
    /// 貨品表資料結構  
    /// 用於紀錄藥品在各層架或裝置上的位置、批號、效期與數量。
    /// </summary>
    [Description("stockLight")]
    public class stockLightClass
    {
        /// <summary>
        /// 唯一識別碼 (GUID)
        /// </summary>
        [Description("VARCHAR,50,PRIMARY")]
        public string GUID { get; set; }
        /// <summary>
        /// ip
        /// </summary>
        [Description("VARCHAR,20,NONE")]
        public string ip { get; set; }
        /// <summary>
        /// 開始位置
        /// </summary>
        [Description("VARCHAR,20,NONE")]
        public string start_num { get; set; }
        /// <summary>
        /// 結束位置
        /// </summary>
        [Description("VARCHAR,20,NONE")]
        public string end_num { get; set; }
        /// <summary>
        /// 裝置類型
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        public string device_type { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        [Description("DATETIME,20,NONE")]
        public string start_time { get; set; }
        /// <summary>
        /// 結束時間
        /// </summary>
        [Description("DATETIME,20,NONE")]
        public string end_time { get; set; }
        static public returnData get_stockLight_all(string API_Server)
        {
            string url = $"{API_Server}/api/stock/get_stockLight_all";

            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();

            List<stockLightClass> stockLightClasses = returnData.Data.ObjToClass<List<stockLightClass>>();
            Console.WriteLine($"{returnData}");
            return returnData;
        }
    }
    public enum enum_stockLight
    {
        GUID,
        ip,
        start_num,
        end_num,
        device_type,
        start_time,
        end_time

    }
    public static class stockClassMethod
    {
        static public Dictionary<string, List<stockClass>> ToDictByShelfGUID(this List<stockClass> stockClasses)
        {
            Dictionary<string, List<stockClass>> dictionary = new Dictionary<string, List<stockClass>>();
            foreach (var item in stockClasses)
            {
                if (dictionary.TryGetValue(item.Shelf_GUID, out List<stockClass> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.Shelf_GUID] = new List<stockClass> { item };
                }
            }
            return dictionary;
        }
        static public List<stockClass> GetByShelfGUID(this Dictionary<string, List<stockClass>> dict, string Shelf_GUID)
        {
            if (dict.TryGetValue(Shelf_GUID, out List<stockClass> stockClasses))
            {
                return stockClasses;
            }
            else
            {
                return new List<stockClass>();
            }
        }
        static public Dictionary<string, List<stockClass>> ToDictByCode(this List<stockClass> stockClasses)
        {
            Dictionary<string, List<stockClass>> dictionary = new Dictionary<string, List<stockClass>>();
            foreach (var item in stockClasses)
            {
                if (dictionary.TryGetValue(item.藥碼, out List<stockClass> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.藥碼] = new List<stockClass> { item };
                }
            }
            return dictionary;
        }
        static public List<stockClass> GetByCode(this Dictionary<string, List<stockClass>> dict, string code)
        {
            if (dict.TryGetValue(code, out List<stockClass> stockClasses))
            {
                return stockClasses;
            }
            else
            {
                return new List<stockClass>();
            }
        }



    }

}
