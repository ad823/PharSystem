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
    [EnumDescription("med_config")]
    public enum enum_medConfig
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("藥碼,VARCHAR,50,INDEX")]
        藥碼,
        [Description("效期管理,VARCHAR,15,NONE")]
        效期管理,
        [Description("盲盤,VARCHAR,15,NONE")]
        盲盤,
        [Description("複盤,VARCHAR,15,NONE")]
        複盤,
        [Description("結存報表,VARCHAR,15,NONE")]
        結存報表,
        [Description("雙人覆核,VARCHAR,15,NONE")]
        雙人覆核,
        [Description("麻醉藥品,VARCHAR,15,NONE")]
        麻醉藥品,
        [Description("形狀相似,VARCHAR,15,NONE")]
        形狀相似,
        [Description("發音相似,VARCHAR,15,NONE")]
        發音相似,
        [Description("LASA,VARCHAR,15,NONE")]
        LASA,
        [Description("自定義,VARCHAR,15,NONE")]
        自定義,
        [Description("調劑註記,VARCHAR,300,NONE")]
        調劑註記,
        [Description("使用RFID,VARCHAR,5,NONE")]
        使用RFID,
        [Description("每日盤點,VARCHAR,20,NONE")]
        每日盤點,

    }
    public class medConfigClass
    {
        /// <summary>
        /// 唯一KEY.
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// 藥碼
        /// </summary>
        [JsonPropertyName("CODE")]
        public string 藥碼 { get; set; }
        /// <summary>
        /// 效期管理
        /// </summary>
        [JsonPropertyName("expiry")]
        public string 效期管理 { get; set; }
        /// <summary>
        /// 盲盤
        /// </summary>
        [JsonPropertyName("blind")]
        public string 盲盤 { get; set; }
        /// <summary>
        /// 複盤
        /// </summary>
        [JsonPropertyName("recheck")]
        public string 複盤 { get; set; }
        /// <summary>
        /// 結存報表
        /// </summary>
        [JsonPropertyName("inventoryReport")]
        public string 結存報表 { get; set; }
        /// <summary>
        /// 雙人覆核
        /// </summary>
        [JsonPropertyName("dual_verification")]
        public string 雙人覆核 { get; set; }
        /// <summary>
        /// 麻醉藥品
        /// </summary>
        [JsonPropertyName("isAnesthetic")]
        public string 麻醉藥品 { get; set; }
        /// <summary>
        /// 形狀相似
        /// </summary>
        [JsonPropertyName("isShapeSimilar")]
        public string 形狀相似 { get; set; }
        /// <summary>
        /// 發音相似
        /// </summary>
        [JsonPropertyName("isSoundSimilar")]
        public string 發音相似 { get; set; }
        /// <summary>
        /// LASA
        /// </summary>
        [JsonPropertyName("lasa")]
        public string LASA { get; set; }
        /// <summary>
        /// 自定義
        /// </summary>
        [JsonPropertyName("customVar")]
        public string 自定義 { get; set; }
        /// <summary>
        /// 調劑註記。
        /// </summary>
        [JsonPropertyName("dispensing_note")]
        public string 調劑註記 { get; set; }
        /// <summary>
        /// 是否啟用 RFID 管控
        /// </summary>
        [JsonPropertyName("useRFID")]
        public string 使用RFID { get; set; }
        /// <summary>
        /// 每日盤點
        /// </summary>
        [JsonPropertyName("inv_daily")]
        public string 每日盤點 { get; set; }

        static public SQLUI.Table init(string API_Server)
        {
            string url = $"{API_Server}/api/medconfig/init";

            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            List<SQLUI.Table> tables = json_out.JsonDeserializet<List<SQLUI.Table>>();
            SQLUI.Table table = SQLUI.TableMethod.GetTable(tables, new enum_medConfig());
            return table;
        }
        static public List<medConfigClass> get_all(string API_Server)
        {
            var (code, result, list) = get_all_full(API_Server);
            return list;
        }
        static public (int code, string result, List<medConfigClass>) get_all_full(string API_Server)
        {
            string url = $"{API_Server}/api/medConfig/get_all";

            returnData returnData = new returnData();


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
            List<medConfigClass> medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }

        static public medConfigClass get_by_codes(string API_Server, string code)
        {
            var (Code, result, list) = get_by_codes_full(API_Server, new List<string> { code });
            if(list.Count == 0)
            {
                return null;
            }
            return list[0];
        }
        static public (int code, string result, List<medConfigClass>) get_by_codes_full(string API_Server, List<string> codes)
        {
            string url = $"{API_Server}/api/medConfig/get_by_codes";
            string sqlList = string.Join(", ", codes.Select(code => $"{code}"));

            returnData returnData = new returnData();
            returnData.ValueAry.Add(sqlList);


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
            List<medConfigClass> medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }

        static public List<medConfigClass> get_dispense_note_by_codes(string API_Server, List<string> codes)
        {
            var (Code, result, list) = get_dispense_note_by_codes_full(API_Server, codes);
            return list;
        }
        static public List<medConfigClass> get_dispense_note_by_codes(string API_Server, string code)
        {
            var (Code, result, list) = get_dispense_note_by_codes_full(API_Server, new List<string> { code });
            return list;
        }
        static public (int code, string result, List<medConfigClass>) get_dispense_note_by_codes_full(string API_Server, List<string> codes)
        {
            string url = $"{API_Server}/api/medConfig/get_dispense_note_by_codes";
            string sqlList = string.Join(",", codes.Select(code => $"{code}"));
            returnData returnData = new returnData();
            returnData.ValueAry.Add(sqlList);


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
            List<medConfigClass> medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }

        static public List<medConfigClass> get_dispense_note(string API_Server)
        {
            var (Code, result, list) = get_dispense_note_full(API_Server);
            return list;
        }
        static public (int code, string result, List<medConfigClass>) get_dispense_note_full(string API_Server)
        {
            string url = $"{API_Server}/api/medConfig/get_dispense_note";
            returnData returnData = new returnData();


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
            List<medConfigClass> medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }

        static public List<medConfigClass> get_isShapeSimilar_note(string API_Server)
        {
            var (Code, result, list) = get_isShapeSimilar_note_full(API_Server);
            return list;
        }
        static public (int code, string result, List<medConfigClass>) get_isShapeSimilar_note_full(string API_Server)
        {
            string url = $"{API_Server}/api/medConfig/get_isShapeSimilar_note";
            returnData returnData = new returnData();


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
            List<medConfigClass> medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }

        static public List<medConfigClass> get_isSoundSimilar_note(string API_Server)
        {
            var (Code, result, list) = get_isSoundSimilar_note_full(API_Server);
            return list;
        }
        static public (int code, string result, List<medConfigClass>) get_isSoundSimilar_note_full(string API_Server)
        {
            string url = $"{API_Server}/api/medConfig/get_isSoundSimilar_note";
            returnData returnData = new returnData();


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
            List<medConfigClass> medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }

        static public List<medConfigClass> get_useRFID(string API_Server)
        {
            var (Code, result, list) = get_useRFID_full(API_Server);
            return list;
        }
        static public (int code, string result, List<medConfigClass>) get_useRFID_full(string API_Server)
        {
            string url = $"{API_Server}/api/medConfig/get_useRFID";
            returnData returnData = new returnData();


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
            List<medConfigClass> medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }

        static public List<medConfigClass> add(string API_Server, medConfigClass medConfigClass)
        {
            var (code, result, list) = add_full(API_Server, new List<medConfigClass> { medConfigClass });
            return list;
        }
        static public List<medConfigClass> add(string API_Server, List<medConfigClass> medConfigClasses)
        {
            var (code, result, list) = add_full(API_Server, medConfigClasses);
            return list;
        }
        static public (int code, string result, List<medConfigClass>) add_full(string API_Server, List<medConfigClass> medConfigClasses)
        {
            string url = $"{API_Server}/api/medConfig/add";

            returnData returnData = new returnData();
            returnData.Data = medConfigClasses;

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
            medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }

        static public List<medConfigClass> delete_by_guid(string API_Server, string guid)
        {
            var (code, result, list) = delete_by_guid_full(API_Server, guid);
            return list;
        }
        static public (int code, string result, List<medConfigClass>) delete_by_guid_full(string API_Server, string guid)
        {
            string url = $"{API_Server}/api/medConfig/delete_by_guid";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(guid);


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
            List<medConfigClass> medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }

        static public List<medConfigClass> delete_by_codes(string API_Server, List<string> codes)
        {
            var (code, result, list) = delete_by_codes_full(API_Server, codes);
            return list;
        }
        static public (int code, string result, List<medConfigClass>) delete_by_codes_full(string API_Server, List<string> codes)
        {
            string url = $"{API_Server}/api/medConfig/delete_by_codes";
            string sqlList = string.Join(", ", codes.Select(code => $"{code}"));
            returnData returnData = new returnData();
            returnData.ValueAry.Add(sqlList);


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
            List<medConfigClass> medConfigClasses = returnData_out.Data.ObjToClass<List<medConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, medConfigClasses);
        }
    }

    public static class medConfigMethod
    {
        static public System.Collections.Generic.Dictionary<string, List<medConfigClass>> CoverToDictionaryByCode(this List<medConfigClass> medClasses)
        {
            Dictionary<string, List<medConfigClass>> dictionary = new Dictionary<string, List<medConfigClass>>();

            foreach (var item in medClasses)
            {
                string key = item.藥碼;

                // 如果字典中已經存在該索引鍵，則將值添加到對應的列表中
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(item);
                }
                // 否則創建一個新的列表並添加值
                else
                {
                    List<medConfigClass> values = new List<medConfigClass> { item };
                    dictionary[key] = values;
                }
            }

            return dictionary;
        }
        static public List<medConfigClass> SortDictionaryByCode(this System.Collections.Generic.Dictionary<string, List<medConfigClass>> dictionary, string code)
        {
            if (dictionary.ContainsKey(code))
            {
                return dictionary[code];
            }
            return new List<medConfigClass>();
        }


        
    }


}
