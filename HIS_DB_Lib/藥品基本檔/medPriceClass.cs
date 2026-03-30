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
    [EnumDescription("medPrice")]
    public enum enum_medPrice
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("藥品碼,VARCHAR,50,INDEX")]
        藥品碼,
        [Description("售價,VARCHAR,15,NONE")]
        售價,
        [Description("成本價,VARCHAR,15,NONE")]
        成本價,
        [Description("最近一次售價,VARCHAR,15,NONE")]
        最近一次售價,
        [Description("最近一次成本價,VARCHAR,15,NONE")]
        最近一次成本價,
        [Description("健保價,VARCHAR,15,NONE")]
        健保價,
        [Description("ATC,VARCHAR,20,NONE")]
        ATC,
        [Description("加入時間,DATETIME,20,NONE")]
        加入時間,
    }
    public class medPriceClass
    {
        /// <summary>
        /// GUID
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// 藥品碼
        /// </summary>
        [JsonPropertyName("Code")]
        public string 藥品碼 { get; set; }
        /// <summary>
        /// 售價
        /// </summary>
        [JsonPropertyName("SalePrice")]
        public string 售價 { get; set; }
        /// <summary>
        /// 成本價
        /// </summary>
        [JsonPropertyName("CostPrice")]
        public string 成本價 { get; set; }
        /// <summary>
        /// 最近一次售價
        /// </summary>
        [JsonPropertyName("RecentSalePrice")]
        public string 最近一次售價 { get; set; }
        /// <summary>
        /// 最近一次成本價
        /// </summary>
        [JsonPropertyName("RecentCostPrice")]
        public string 最近一次成本價 { get; set; }
        /// <summary>
        /// 健保價
        /// </summary>
        [JsonPropertyName("HealthInsurancePrice")]
        public string 健保價 { get; set; }
        /// <summary>
        /// ATC
        /// </summary>
        [JsonPropertyName("ATC")]
        public string ATC { get; set; }
        /// <summary>
        /// 加入時間
        /// </summary>
        [JsonPropertyName("add_time")]
        public string 加入時間 { get; set; }
        static public returnData update(string API_Server, List<medPriceClass> medPriceClasses)
        {
            string url = $"{API_Server}/api/medPirce/update";

            returnData returnData = new returnData();
            returnData.Data = medPriceClasses;           
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public returnData get_all(string API_Server)
        {
            string url = $"{API_Server}/api/medPirce/get_all";

            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public List<medPriceClass> get_by_codes(string API_Server, List<string> code)
        {
            string url = $"{API_Server}/api/medPirce/get_by_codes";

            returnData returnData = new returnData();
            string codes = string.Join(",", code);
            returnData.ValueAry.Add(codes);
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            List<medPriceClass> medPriceClasses = returnData.Data.ObjToClass<List<medPriceClass>>();
            return medPriceClasses;
        }
        static public Dictionary<string, List<medPriceClass>> CoverToDicByCode(List<medPriceClass> medPriceClasses)
        {
            Dictionary<string, List<medPriceClass>> dictionary = new Dictionary<string, List<medPriceClass>>();
            foreach (var item in medPriceClasses)
            {
                if (dictionary.TryGetValue(item.藥品碼, out List<medPriceClass> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.藥品碼] = new List<medPriceClass> { item };
                }
            }
            return dictionary;
        }
        static public List<medPriceClass> GetByCode(Dictionary<string, List<medPriceClass>> dict, string code)
        {
            if (dict.TryGetValue(code, out List<medPriceClass> medPriceClasses))
            {
                return medPriceClasses;
            }
            else
            {
                return new List<medPriceClass>();
            }
        }
    }
}
