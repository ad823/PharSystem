using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HIS_DB_Lib
{
    [EnumDescription("settingPage")]
    public enum enum_settingPage
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("頁面名稱,VARCHAR,200,INDEX")]
        頁面名稱,
        [Description("欄位名稱,VARCHAR,20,NONE")]
        欄位名稱,
        [Description("欄位代碼,VARCHAR,30,NONE")]
        欄位代碼,
        [Description("欄位種類,VARCHAR,20,NONE")]
        欄位種類,
        [Description("選項,VARCHAR,50,NONE")]
        選項,
        [Description("設定值,VARCHAR,100,NONE")]
        設定值,

    }
    public static class settingPageClassMethod
    {
        public static settingPageClass myFind(this List<settingPageClass> settingPageClasses, string 頁面名稱, string 欄位名稱)
        {
            if (settingPageClasses == null) return null;
            settingPageClass settingPageClass = settingPageClasses.FirstOrDefault(temp => temp.頁面名稱 == 頁面名稱 && temp.欄位名稱 == 欄位名稱);
            return settingPageClass;
        }
    }
    public class settingPageClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// 頁面名稱
        /// </summary>
        [JsonPropertyName("page_name")]
        public string 頁面名稱 { get; set; }
        /// <summary>
        /// 欄位名稱
        /// </summary>
        [JsonPropertyName("cht")]
        public string 欄位名稱 { get; set; }
        /// <summary>
        /// 欄位代碼
        /// </summary>
        [JsonPropertyName("name")]
        public string 欄位代碼 { get; set; }
        /// <summary>
        /// 欄位種類
        /// </summary>
        [JsonPropertyName("value_type")]
        public string 欄位種類 { get; set; }
        /// <summary>
        /// 選項
        /// </summary>
        [JsonPropertyName("option_str")]
        public string 選項 { get; set; }
        /// <summary>
        /// value_db
        /// </summary>
        [JsonPropertyName("value_db")]
        public string 設定值 { get; set; }
        public List<string> option { get; set; }
        public object value { get; set; }
        static public List<settingPageClass> get_all(string API_Server)
        {
            string url = $"{API_Server}/api/settingPage/get_all";
            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200) return null;
            List<settingPageClass> settingPageClasses = returnData.Data.ObjToClass<List<settingPageClass>>();
            return settingPageClasses;

        }
        static public async Task<List<settingPageClass>> get_by_page_name(string API_Server, string page_name)
        {
            string url = $"{API_Server}/api/settingPage/get_by_page_name";
            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            Task<string> result = Net.WEBApiPostJsonAsync(url, json_in, false);
            string json_out = await result;
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200) return null;
            List<settingPageClass> settingPageClasses = returnData.Data.ObjToClass<List<settingPageClass>>();
            return settingPageClasses;

        }
        public class ICP_By_type : IComparer<settingPageClass>
        {
            public int Compare(settingPageClass x, settingPageClass y)
            {
                int result = x.欄位種類.CompareTo(y.欄位種類);
                if (result == 0)
                {
                    result = x.欄位名稱.CompareTo(y.欄位名稱);
                }
                return result;
            }
        }

    }
    public class uiConfig
    {
        [JsonPropertyName("cht")]
        public string 欄位名稱 { get; set; }
        [JsonPropertyName("name")]
        public string 欄位代碼 { get; set; }
        [JsonPropertyName("value")]
        public string 設定值 { get; set; }
    }
}
