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
    public enum enum_medRecheckLog_ICDT_TYPE
    {
        [Description("交班對點")]
        交班對點,
        [Description("過期藥品")]
        過期藥品,
        [Description("RFID入庫異常")]
        RFID入庫異常,
        [Description("RFID出庫異常")]
        RFID出庫異常,
        [Description("RFID調劑異常")]
        RFID調劑異常,
        [Description("RFID退藥異常")]
        RFID退藥異常,
        [Description("盤點異常")]
        盤點異常,
    }
    public enum enum_medRecheckLog_State
    {
        未排除,
        已排除
    }
    [EnumDescription("med_recheck_log_new")]
    public enum enum_medRecheckLog
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("發生類別,VARCHAR,50,INDEX")]
        發生類別,
        [Description("藥碼,VARCHAR,20,INDEX")]
        藥碼,
        [Description("藥名,VARCHAR,300,NONE")]
        藥名,
        [Description("庫存值,VARCHAR,20,NONE")]
        庫存值,
        [Description("盤點值,VARCHAR,20,NONE")]
        盤點值,
        [Description("差異值,VARCHAR,20,NONE")]
        差異值,
        [Description("批號,VARCHAR,200,NONE")]
        批號,
        [Description("效期,VARCHAR,200,NONE")]
        效期,
        [Description("醫令_GUID,VARCHAR,50,INDEX")]
        醫令_GUID,
        [Description("交易紀錄_GUID,VARCHAR,50,INDEX")]
        交易紀錄_GUID,
        [Description("盤點藥師1,VARCHAR,50,INDEX")]
        盤點藥師1,
        [Description("盤點藥師ID1,VARCHAR,50,INDEX")]
        盤點藥師ID1,
        [Description("盤點藥師2,VARCHAR,50,INDEX")]
        盤點藥師2,
        [Description("盤點藥師ID2,VARCHAR,50,INDEX")]
        盤點藥師ID2,
        [Description("排除藥師,VARCHAR,50,INDEX")]
        排除藥師,
        [Description("發生時間,DATETIME,300,INDEX")]
        發生時間,
        [Description("排除時間,DATETIME,300,INDEX")]
        排除時間,
        [Description("異常原因,VARCHAR,500,NONE")]
        異常原因,
        [Description("狀態,VARCHAR,20,INDEX")]
        狀態,
        [Description("事件描述,VARCHAR,500,NONE")]
        事件描述,
        [Description("通知註記,VARCHAR,50,INDEX")]
        通知註記,
        [Description("通知時間,DATETIME,300,INDEX")]
        通知時間,
        [Description("參數1,VARCHAR,300,NONE")]
        參數1,
        [Description("參數2,VARCHAR,300,NONE")]
        參數2,
    }

    /// <summary>
    /// 表示藥品覆盤日志的數據類別。
    /// </summary>
    public class medRecheckLogClass
    {
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        [JsonPropertyName("ICDT_TYPE")]
        public string 發生類別 { get; set; }

        [JsonPropertyName("CODE")]
        public string 藥碼 { get; set; }

        [JsonPropertyName("NAME")]
        public string 藥名 { get; set; }

        [JsonPropertyName("INV_QTY")]
        public string 庫存值 { get; set; }

        [JsonPropertyName("CHECK_QTY")]
        public string 盤點值 { get; set; }

        [JsonPropertyName("UPDATE_QTY")]
        public string 差異值 { get; set; }

        [JsonPropertyName("LOT")]
        public string 批號 { get; set; }

        [JsonPropertyName("VAL")]
        public string 效期 { get; set; }

        [JsonPropertyName("ORDER_GUID")]
        public string 醫令_GUID { get; set; }

        [JsonPropertyName("TRADING_GUID")]
        public string 交易紀錄_GUID { get; set; }

        [JsonPropertyName("CHECK_OP1")]
        public string 盤點藥師1 { get; set; }

        [JsonPropertyName("CHECK_OP_ID1")]
        public string 盤點藥師ID1 { get; set; }

        [JsonPropertyName("CHECK_OP2")]
        public string 盤點藥師2 { get; set; }

        [JsonPropertyName("CHECK_OP_ID2")]
        public string 盤點藥師ID2 { get; set; }

        [JsonPropertyName("TROUBLE_OP")]
        public string 排除藥師 { get; set; }

        [JsonPropertyName("occurrence_time")]
        public string 發生時間 { get; set; }

        [JsonPropertyName("troubleshooting_time")]
        public string 排除時間 { get; set; }

        [JsonPropertyName("ALRM_REASON")]
        public string 異常原因 { get; set; }

        [JsonPropertyName("state")]
        public string 狀態 { get; set; }

        [JsonPropertyName("EVENT_DESC")]
        public string 事件描述 { get; set; }

        [JsonPropertyName("NOTICE_FLAG")]
        public string 通知註記 { get; set; }

        [JsonPropertyName("NOTICE_TIME")]
        public string 通知時間 { get; set; }

        [JsonPropertyName("PARAM_1")]
        public string 參數1 { get; set; }

        [JsonPropertyName("PARAM_2")]
        public string 參數2 { get; set; }



        static public SQLUI.Table init(string API_Server, string ServerName, string ServerType)
        {
            string url = $"{API_Server}/api/medRecheckLog/init";

            returnData returnData = new returnData();
            returnData.ServerType = ServerType;
            returnData.ServerName = ServerName;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            SQLUI.Table table = json_out.JsonDeserializet<SQLUI.Table>();
            return table;
        }
        static public void add(string API_Server, string ServerName, string ServerType, medRecheckLogClass medRecheckLogClass)
        {
            List<medRecheckLogClass> medRecheckLogClasses = new List<medRecheckLogClass>();
            medRecheckLogClasses.Add(medRecheckLogClass);
            add(API_Server, ServerName, ServerType, medRecheckLogClasses);
        }
        static public void add(string API_Server, string ServerName, string ServerType, List<medRecheckLogClass> medRecheckLogClasses)
        {

            string url = $"{API_Server}/api/medRecheckLog/add";
            returnData returnData = new returnData();
            returnData.Data = medRecheckLogClasses;
            returnData.ServerType = ServerType;
            returnData.ServerName = ServerName;
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
            Console.WriteLine($"{returnData_out}");

        }

        static public void replace_by_guid(string API_Server, string ServerName, string ServerType, medRecheckLogClass medRecheckLogClass)
        {
            List<medRecheckLogClass> medRecheckLogClasses = new List<medRecheckLogClass>();
            medRecheckLogClasses.Add(medRecheckLogClass);
            replace_by_guid(API_Server, ServerName, ServerType, medRecheckLogClasses);
        }
        static public void replace_by_guid(string API_Server, string ServerName, string ServerType, List<medRecheckLogClass> medRecheckLogClasses)
        {

            string url = $"{API_Server}/api/medRecheckLog/replace_by_guid";
            returnData returnData = new returnData();
            returnData.Data = medRecheckLogClasses;
            returnData.ServerType = ServerType;
            returnData.ServerName = ServerName;
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
            Console.WriteLine($"{returnData_out}");

        }
        static public List<medRecheckLogClass> get_by_occurrence_time_st_end(string API_Server, string ServerName, string ServerType, DateTime dateTime_start, DateTime dateTime_end)
        {

            string url = $"{API_Server}/api/medRecheckLog/get_by_occurrence_time_st_end";

            returnData returnData = new returnData();
            returnData.Value = $"{dateTime_start.ToDateTimeString()},{dateTime_end.ToDateTimeString()}";
            returnData.ServerType = ServerType;
            returnData.ServerName = ServerName;

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
            Console.WriteLine($"{returnData_out}");

            List<medRecheckLogClass> medRecheckLogClasses = returnData_out.Data.ObjToClass<List<medRecheckLogClass>>();
            medRecheckLogClasses.Sort(new ICP_By_occurrence_time());
            return medRecheckLogClasses;
        }
        static public List<medRecheckLogClass> get_by_troubleshooting_time_st_end(string API_Server, string ServerName, string ServerType, DateTime dateTime_start, DateTime dateTime_end)
        {

            string url = $"{API_Server}/api/medRecheckLog/get_by_troubleshooting_time_st_end";

            returnData returnData = new returnData();
            returnData.Value = $"{dateTime_start.ToDateTimeString()},{dateTime_end.ToDateTimeString()}";
            returnData.ServerType = ServerType;
            returnData.ServerName = ServerName;
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
            Console.WriteLine($"{returnData_out}");

            List<medRecheckLogClass> medRecheckLogClasses = returnData_out.Data.ObjToClass<List<medRecheckLogClass>>();
            medRecheckLogClasses.Sort(new ICP_By_troubleshooting_time());
            return medRecheckLogClasses;
        }
        static public List<medRecheckLogClass> get_ng_state_data(string API_Server, string ServerName, string ServerType)
        {

            string url = $"{API_Server}/api/medRecheckLog/get_ng_state_data";

            returnData returnData = new returnData();
            returnData.ServerType = ServerType;
            returnData.ServerName = ServerName;
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
            //Console.WriteLine($"{returnData_out}");

            List<medRecheckLogClass> medRecheckLogClasses = returnData_out.Data.ObjToClass<List<medRecheckLogClass>>();
            if (medRecheckLogClasses == null) return new List<medRecheckLogClass>();
            medRecheckLogClasses.Sort(new ICP_By_occurrence_time());
            return medRecheckLogClasses;
        }
        static public int get_unresolved_qty_by_code(string API_Server, string ServerName, string ServerType, string code)
        {

            string url = $"{API_Server}/api/medRecheckLog/get_unresolved_qty_by_code";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(code);
            returnData.ServerType = ServerType;
            returnData.ServerName = ServerName;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {
                return 0;
            }
            if (returnData_out.Data == null)
            {
                return 0;
            }
            Console.WriteLine($"{returnData_out}");

            medRecheckLogClass medRecheckLogClass = returnData_out.Data.ObjToClass<medRecheckLogClass>();
            return medRecheckLogClass.差異值.StringToInt32();
        }
        static public void set_unresolved_data_by_code(string API_Server, string ServerName, string ServerType, string code , string TROUBLE_OP)
        {

            string url = $"{API_Server}/api/medRecheckLog/set_unresolved_data_by_code";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(code);
            returnData.ValueAry.Add(TROUBLE_OP);
            returnData.ServerType = ServerType;
            returnData.ServerName = ServerName;
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
            Console.WriteLine($"{returnData_out}");

        }

        static public (int code, string result) set_unresolved_data_by_guid(string API_Server, string ServerName, string ServerType, string guid, string pharmacist)
        {
            string url = $"{API_Server}/api/medRecheckLog/set_unresolved_data_by_guid";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(guid);
            returnData.ValueAry.Add(pharmacist);
            returnData.ServerType = ServerType;
            returnData.ServerName = ServerName;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {
                return (0, "returnData_out == null");
            }

            return (returnData_out.Code, returnData_out.Result);
        }

        /// <summary>
        /// 呼叫 API 取得所有未排除盤點異常資料（依發生時間降冪排序）
        /// </summary>
        /// <param name="API_Server">API伺服器網址</param>
        /// <param name="serverName">Server 名稱</param>
        /// <param name="serverType">Server 類型</param>
        /// <returns>未排除盤點異常資料</returns>
        static public List<medRecheckLogClass> get_all_unresolved_data(string API_Server, string serverName, string serverType)
        {
            var (code, result, list) = get_all_unresolved_data_full(API_Server, serverName, serverType);
            return list;
        }
        /// <summary>
        /// 呼叫 API 取得所有未排除盤點異常資料（依發生時間降冪排序，完整回傳）
        /// </summary>
        /// <param name="API_Server">API伺服器網址</param>
        /// <param name="serverName">Server 名稱</param>
        /// <param name="serverType">Server 類型</param>
        /// <returns>(狀態碼, 結果訊息, 資料清單)</returns>
        static public (int code, string result, List<medRecheckLogClass>) get_all_unresolved_data_full(string API_Server, string serverName, string serverType)
        {
            string url = $"{API_Server}/api/medRecheckLog/get_all_unresolved_data";

            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return (0, "returnData_out == null", null);
            }
            if (returnData_out.Data == null)
            {
                return (0, "returnData_out.Data == null", null);
            }

            Console.WriteLine($"{returnData_out}");

            List<medRecheckLogClass> list = returnData_out.Data.ObjToClass<List<medRecheckLogClass>>();
            return (returnData_out.Code, returnData_out.Result, list);
        }


        public class ICP_By_occurrence_time : IComparer<medRecheckLogClass>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(medRecheckLogClass x, medRecheckLogClass y)
            {
                DateTime temp0 = x.發生時間.StringToDateTime();
                DateTime temp1 = y.發生時間.StringToDateTime();
                int compare = temp0.CompareTo(temp1);
                return compare;

            }
        }
        public class ICP_By_troubleshooting_time : IComparer<medRecheckLogClass>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(medRecheckLogClass x, medRecheckLogClass y)
            {
                DateTime temp0 = x.排除時間.StringToDateTime();
                DateTime temp1 = y.排除時間.StringToDateTime();
                int compare = temp0.CompareTo(temp1);
                return compare;

            }
        }
    }
    static public class medRecheckLogClassMethod
    {
        /// <summary>
        /// 根據狀態與（可選）發生類別過濾資料。
        /// </summary>
        public static List<medRecheckLogClass> FilterByStateAndType(this List<medRecheckLogClass> list, enum_medRecheckLog_State 狀態, enum_medRecheckLog_ICDT_TYPE? 發生類別 = null)
        {
            if (list == null) return new List<medRecheckLogClass>();

            string 狀態描述 = 狀態.GetEnumDescription();
            string 類別描述 = 發生類別?.GetEnumDescription();

            var result = list
                .Where(x => x.狀態 == 狀態描述 &&
                            (string.IsNullOrEmpty(類別描述) || x.發生類別 == 類別描述))
                .ToList();

            return result;
        }
        /// <summary>
        /// 只根據狀態過濾資料。
        /// </summary>
        public static List<medRecheckLogClass> FilterByState(this List<medRecheckLogClass> list, enum_medRecheckLog_State 狀態)
        {
            return FilterByStateAndType(list, 狀態, null);
        }
        /// <summary>
        /// 只根據發生類別過濾資料。
        /// </summary>
        public static List<medRecheckLogClass> FilterByType(this List<medRecheckLogClass> list, enum_medRecheckLog_ICDT_TYPE 發生類別)
        {
            if (list == null) return new List<medRecheckLogClass>();

            string 類別描述 = 發生類別.GetEnumName();
            return list
                .Where(x => x.發生類別 == 類別描述)
                .ToList();
        }
        public static List<medRecheckLogClass> FilterByTypes(this List<medRecheckLogClass> list, params enum_medRecheckLog_ICDT_TYPE[] 發生類別集合)
        {
            if (list == null || 發生類別集合 == null || 發生類別集合.Length == 0)
            {
                return new List<medRecheckLogClass>();
            }

            var 類別描述集合 = 發生類別集合.Select(e => e.GetEnumName()).ToHashSet();
            return list.Where(x => 類別描述集合.Contains(x.發生類別)).ToList();
        }

        static public Dictionary<string, List<medRecheckLogClass>> CoverToDictionaryBy_Code(this List<medRecheckLogClass> medRecheckLogClasses)
        {
            Dictionary<string, List<medRecheckLogClass>> dictionary = new Dictionary<string, List<medRecheckLogClass>>();

            foreach (var item in medRecheckLogClasses)
            {
                string key = item.藥碼;

                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(item);
                }
                else
                {
                    dictionary[key] = new List<medRecheckLogClass> { item };
                }
            }

            return dictionary;
        }
        static public List<medRecheckLogClass> SortDictionaryBy_Code(this Dictionary<string, List<medRecheckLogClass>> dictionary, string code)
        {
            if (dictionary.ContainsKey(code))
            {
                return dictionary[code];
            }
            return new List<medRecheckLogClass>();
        }

    }

}
