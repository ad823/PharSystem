using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.ComponentModel;
using System.Reflection;
namespace HIS_DB_Lib
{
    public enum enum_人員資料
    {
        GUID,
        ID,
        姓名,
        性別,
        密碼,
        單位,
        藥師證字號,
        權限等級,
        顏色,
        卡號,
        一維條碼,
        識別圖案,
        指紋辨識,
        指紋ID,
        開門權限,
        身分,
    }
    [EnumDescription("person_time_period")]
    public enum enum_personTimePeriod
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("ID,VARCHAR,50,INDEX")]
        ID,
        [Description("start_date,DATETIME,50,NONE")]
        start_date,
        [Description("end_date,DATETIME,50,NONE")]
        end_date,
        [Description("period1,VARCHAR,50,NONE")]
        period1,
        [Description("period2,VARCHAR,50,NONE")]
        period2,
        [Description("period3,VARCHAR,50,NONE")]
        period3,
        [Description("period4,VARCHAR,50,NONE")]
        period4,
        [Description("period5,VARCHAR,50,NONE")]
        period5,
    }
   
    /// <summary>
    /// 人員可用時段設定資料
    /// </summary>
    public class personTimePeriodClass
    {
        /// <summary>
        /// 唯一識別碼
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        /// <summary>
        /// 人員 ID
        /// </summary>
        [JsonPropertyName("ID")]
        public string ID { get; set; }

        /// <summary>
        /// 可使用起始日期
        /// </summary>
        [JsonPropertyName("start_date")]
        public string start_date { get; set; }

        /// <summary>
        /// 可使用結束日期
        /// </summary>
        [JsonPropertyName("end_date")]
        public string end_date { get; set; }

        /// <summary>
        /// 可使用時段 1（字串格式）
        /// </summary>
        [JsonPropertyName("period1")]
        public string period1 { get; set; }

        /// <summary>
        /// 可使用時段 2
        /// </summary>
        [JsonPropertyName("period2")]
        public string period2 { get; set; }

        /// <summary>
        /// 可使用時段 3
        /// </summary>
        [JsonPropertyName("period3")]
        public string period3 { get; set; }

        /// <summary>
        /// 可使用時段 4
        /// </summary>
        [JsonPropertyName("period4")]
        public string period4 { get; set; }

        /// <summary>
        /// 可使用時段 5
        /// </summary>
        [JsonPropertyName("period5")]
        public string period5 { get; set; }

        /// <summary>
        /// 檢查現在是否在日期內 + 任一時段內
        /// </summary>
        public static bool IsAllowedNow(personTimePeriodClass item, DateTime now)
        {
            if (!IsInDateRange(now, item.start_date.StringToDateTime(), item.end_date.StringToDateTime()))
                return false;

            if (!IsInPeriodRange(
                    now,
                    item.period1,
                    item.period2,
                    item.period3,
                    item.period4,
                    item.period5))
                return false;

            return true;
        }

        // -----------------------------
        // 日期檢查（允許 null）
        // -----------------------------
        public static bool IsInDateRange(DateTime now, DateTime? start, DateTime? end)
        {
            if (start == null && end == null) return true;
            if (start == null) return now <= end.Value;
            if (end == null) return now >= start.Value;

            return now >= start.Value && now <= end.Value;
        }

        // -----------------------------
        // 多時段檢查（自動忽略空字串）
        // -----------------------------
        public static bool IsInPeriodRange(DateTime now, params string[] periods)
        {
            foreach (var p in periods)
            {
                if (IsPeriodValid(p) && IsNowInPeriod(now, p))
                    return true;
            }
            return false;
        }

        // -----------------------------
        // 時段格式合法性（HHmm-HHmm）
        // -----------------------------
        public static bool IsPeriodValid(string period)
        {
            if (string.IsNullOrWhiteSpace(period)) return false;

            var parts = period.Split('-');
            if (parts.Length != 2) return false;

            if (!int.TryParse(parts[0], out int s)) return false;
            if (!int.TryParse(parts[1], out int e)) return false;

            int sh = s / 100, sm = s % 100;
            int eh = e / 100, em = e % 100;

            if (sh < 0 || sh > 23 || sm < 0 || sm > 59) return false;
            if (eh < 0 || eh > 23 || em < 0 || em > 59) return false;

            return true;
        }

        // -----------------------------
        // 判斷時間是否在單一時段內（支援跨午夜）
        // -----------------------------
        public static bool IsNowInPeriod(DateTime now, string period)
        {
            if (string.IsNullOrWhiteSpace(period)) return false;

            var parts = period.Split('-');
            int s = int.Parse(parts[0]);
            int e = int.Parse(parts[1]);

            TimeSpan start = new TimeSpan(s / 100, s % 100, 0);
            TimeSpan end = new TimeSpan(e / 100, e % 100, 0);
            TimeSpan nowTime = now.TimeOfDay;

            // ★ 支援跨午夜（例如 2300–0200）
            if (end < start)
            {
                // start → 23:59
                if (nowTime >= start)
                    return true;

                // 00:00 → end
                if (nowTime <= end)
                    return true;

                return false;
            }

            // 一般情況（未跨日）
            return nowTime >= start && nowTime <= end;
        }
    }
    public class personPageClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        [JsonPropertyName("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [JsonPropertyName("name")]
        public string 姓名 { get; set; }
        /// <summary>
        /// 性別
        /// </summary>
        [JsonPropertyName("gender")]
        public string 性別 { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        [JsonPropertyName("password")]
        public string 密碼 { get; set; }
        /// <summary>
        /// 單位
        /// </summary>
        [JsonPropertyName("employer")]
        public string 單位 { get; set; }
        /// <summary>
        /// 藥師證字號
        /// </summary>
        [JsonPropertyName("license")]
        public string 藥師證字號 { get; set; }
        /// <summary>
        /// 權限等級
        /// </summary>
        [JsonPropertyName("level")]
        public string 權限等級 { get; set; }
        /// <summary>
        /// 顏色
        /// </summary>
        [JsonPropertyName("color")]
        public string 顏色 { get; set; }
        /// <summary>
        /// 卡號
        /// </summary>
        [JsonPropertyName("UID")]
        public string 卡號 { get; set; }
        /// <summary>
        /// 一維條碼
        /// </summary>
        [JsonPropertyName("barcode")]
        public string 一維條碼 { get; set; }
        /// <summary>
        /// 識別圖案
        /// </summary>
        [JsonPropertyName("face_image")]
        public string 識別圖案 { get; set; }
        /// <summary>
        /// 指紋辨識
        /// </summary>
        [JsonPropertyName("finger_feature")]
        public string 指紋辨識 { get; set; }
        /// <summary>
        /// 指紋ID
        /// </summary>
        [JsonPropertyName("finger_ID")]
        public string 指紋ID { get; set; }
        /// <summary>
        /// 開門權限
        /// </summary>
        [JsonPropertyName("open_access")]
        public string 開門權限 { get; set; }

        [JsonPropertyName("identity")]
        public string 身分 { get; set; }


        static public List<personTimePeriodClass> get_person_time_period(string API_Server, string serverName = "", string serverType = "")
        {
            string url = $"{API_Server}/api/person_page/get_person_time_period";
            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return null;
            }
            if (returnData_out.Data == null)
            {
                return null;
            }

            List<personTimePeriodClass> personTimes = returnData_out.Data.ObjToClass<List<personTimePeriodClass>>();
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");
            return personTimes;
        }
        static public void add_person_time_period(string API_Server, personTimePeriodClass personTimePeriod, string serverName = "", string serverType = "")
        {
            string url = $"{API_Server}/api/person_page/add_person_time_period";
            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;
            returnData.Data = personTimePeriod;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return;
            }
            if (returnData_out.Data == null)
            {
                return;
            }

            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");
            return;

        }
        static public List<personPageClass> get_all(string API_Server , string serverName = "" , string serverType = "")
        {
            string url = $"{API_Server}/api/person_page/get_all";
    

            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;
            returnData.Data = null;


            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return null;
            }
            if (returnData_out.Data == null)
            {
                return null;
            }
            List<personPageClass> personPageClasses = returnData_out.Data.ObjToClass<List<personPageClass>>();

            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");
            return personPageClasses;
        }
        static public personPageClass serch_by_id(string API_Server , string ID)
        {
            string url = $"{API_Server}/api/person_page/serch_by_id";
            string str_serverNames = "";
            string str_serverTypes = "";

            returnData returnData = new returnData();
            returnData.ServerName = "";
            returnData.ServerType = "";
            returnData.Data = null;
            returnData.Value = ID;
            if (ID.StringIsEmpty()) return null;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return null;
            }
            if (returnData_out.Data == null)
            {
                return null;
            }
            if (returnData_out.Code != 200) return null;
            personPageClass personPageClass = returnData_out.Data.ObjToClass<personPageClass>();

            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");
            return personPageClass;
        }
        static public personPageClass serch_by_name(string API_Server, string name)
        {
            string url = $"{API_Server}/api/person_page/serch_by_name";
            string str_serverNames = "";
            string str_serverTypes = "";

            returnData returnData = new returnData();
            returnData.ServerName = "";
            returnData.ServerType = "";
            returnData.Data = null;
            returnData.Value = name;
            if (name.StringIsEmpty()) return null;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return null;
            }
            if (returnData_out.Data == null)
            {
                return null;
            }
            if (returnData_out.Code != 200) return null;
            personPageClass personPageClass = returnData_out.Data.ObjToClass<personPageClass>();

            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");
            return personPageClass;
        }
        static public SQLUI.Table Init(string API_Server)
        {
            string url = $"{API_Server}/api/person_page/init";

            returnData returnData = new returnData();
            returnData.ServerName = "";
            returnData.ServerType = "";

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            SQLUI.Table table = json_out.JsonDeserializet<SQLUI.Table>();
            return table;
        }
        static public returnData add (string API_Server, List<personPageClass> personPageClasses)
        {
            string url = $"{API_Server}/api/person_page/add";
           
            returnData returnData = new returnData();
            returnData.Data = personPageClasses;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            return returnData_out;
           
        }
    }
    public static class personPageClassMethod
    {
        public static personPageClass searchByName(this List<personPageClass> personPageClasses, string personName)
        {
            if (personPageClasses == null) return null;
            personPageClass personPageClass = personPageClasses.Where(temp => temp.姓名 == personName).FirstOrDefault();
            return personPageClass;
        }
    }
}
