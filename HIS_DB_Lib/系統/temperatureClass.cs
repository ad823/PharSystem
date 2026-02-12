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
    [EnumDescription("temperature")]
    public enum enum_temperature
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("IP,VARCHAR,50,INDEX")]
        IP,
        [Description("溫度,VARCHAR,20,NONE")]
        溫度,
        [Description("濕度,VARCHAR,20,NONE")]
        濕度,
        [Description("新增時間,DATETIME,20,INDEX")]
        新增時間,
    }
    [EnumDescription("temperature_set")]
    public enum enum_temperature_set
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("IP,VARCHAR,50,INDEX")]
        IP,
        [Description("別名,VARCHAR,20,INDEX")]
        別名,
        [Description("溫度上限,VARCHAR,20,NONE")]
        溫度上限,
        [Description("溫度下限,VARCHAR,20,NONE")]
        溫度下限,
        [Description("溫度補償,VARCHAR,20,NONE")]
        溫度補償,
        [Description("濕度上限,VARCHAR,20,NONE")]
        濕度上限,
        [Description("濕度下限,VARCHAR,20,NONE")]
        濕度下限,
        [Description("濕度補償,VARCHAR,20,NONE")]
        濕度補償,
        [Description("警報,VARCHAR,20,NONE")]
        警報,
        [Description("發信,VARCHAR,20,NONE")]
        發信,
    }
    /// <summary>
    /// temperatureClass物件
    /// </summary>
    public class temperatureClass
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        [JsonPropertyName("IP")]
        public string IP { get; set; }
        /// <summary>
        /// 溫度
        /// </summary>
        [JsonPropertyName("temp")]
        public string 溫度 { get; set; }
        /// <summary>
        /// 濕度
        /// </summary>
        [JsonPropertyName("humidity")]
        public string 濕度 { get; set; }
        /// <summary>
        /// 新增時間
        /// </summary>
        [JsonPropertyName("add_time")]
        public string 新增時間 { get; set; }
        public class ICP_By_time : IComparer<temperatureClass>
        {
            public int Compare(temperatureClass x, temperatureClass y)
            {
                
                int result = (x.IP).CompareTo(y.IP);
                if (result == 0)
                {
                    result = string.Compare(x.新增時間, y.新增時間) * -1;
                }
                return result;
            }
        }
        static public (int code, string resuult) add(string API_Server, temperatureClass temperatureClass)
        {
            string url = $"{API_Server}/api/temperature/add";
            returnData returnData = new returnData();
            returnData.Data = temperatureClass;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {
                return (0, "returnData_out == null");
            }
            if (returnData_out.Code == -200)
            {
                return (0, "returnData_out.Code == -200");
            }
            Console.WriteLine($"{returnData_out}");
            return (returnData_out.Code, returnData_out.Result);

        }

    }
    /// <summary>
    /// temperature_setClass物件
    /// </summary>
    public class temperature_setClass
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        [JsonPropertyName("IP")]
        public string IP { get; set; }
        /// <summary>
        /// 別名
        /// </summary>
        [JsonPropertyName("name")]
        public string 別名 { get; set; }
        /// <summary>
        /// 溫度上限
        /// </summary>
        [JsonPropertyName("temp_max")]
        public string 溫度上限 { get; set; }
        /// <summary>
        /// 溫度下限
        /// </summary>
        [JsonPropertyName("temp_min")]
        public string 溫度下限 { get; set; }
        /// <summary>
        /// 溫度補償
        /// </summary>
        [JsonPropertyName("temp_offset")]
        public string 溫度補償 { get; set; }
        /// <summary>
        /// 濕度上限
        /// </summary>
        [JsonPropertyName("humidity_max")]
        public string 濕度上限 { get; set; }
        /// <summary>
        /// 濕度下限
        /// </summary>
        [JsonPropertyName("humidity_min")]
        public string 濕度下限 { get; set; }
        /// <summary>
        /// 濕度補償
        /// </summary>
        [JsonPropertyName("humidity_offset")]
        public string 濕度補償 { get; set; }
        /// <summary>
        /// 警報
        /// </summary>
        [JsonPropertyName("alert")]
        public string 警報 { get; set; }
        /// <summary>
        /// 發信
        /// </summary>
        [JsonPropertyName("mail")]
        public string 發信 { get; set; }
        public List<temperatureClass> temperatureClasses { get; set; }
    }
    public static class temperature_setClassMethod
    {
        public static temperature_setClass searchByIp(this List<temperature_setClass> temperature_SetClasses, string ip)
        {
            if(temperature_SetClasses == null) return null;
            temperature_setClass temperature_SetClass = temperature_SetClasses.Where(item => item.IP == ip).FirstOrDefault();
            return temperature_SetClass;
        }
    }
}
