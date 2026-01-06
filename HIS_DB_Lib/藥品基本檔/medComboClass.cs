using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.Text.Json;
using H_Pannel_lib;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ComponentModel;
namespace HIS_DB_Lib
{
    [EnumDescription("med_Combo")]
    public enum enum_medCombo
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("序列號,VARCHAR,50,INDEX")]
        序列號,
        [Description("藥碼,VARCHAR,10,INDEX")]
        藥碼,
        [Description("藥名,VARCHAR,300,NONE")]
        藥名,

    }
    public class medComboClass
    {
        /// <summary>
        /// 唯一KEY。
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// 序列號。
        /// </summary>
        [JsonPropertyName("sn")]
        public string 序列號 { get; set; }
        /// <summary>
        /// 藥碼。
        /// </summary>
        [JsonPropertyName("code")]
        public string 藥碼 { get; set; }
        /// <summary>
        /// 藥名。
        /// </summary>
        [JsonPropertyName("name")]
        public string 藥名 { get; set; }

        static public List<medComboClass> get_by_code(string API_Server, string code, string serverName = "", string serverType = "")
        {
            return get_by_code_full(API_Server, code, serverName, serverType).medCombos;
        }
        static public (int code, string result, List<medComboClass> medCombos) get_by_code_full(string API_Server, string code, string serverName = "", string serverType = "")
        {
            string url = $"{API_Server}/api/medCombo/get_by_code";

            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;
            returnData.ValueAry.Add(code);
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
            List<medComboClass> medComboClasses = returnData_out.Data.ObjToClass<List<medComboClass>>();
            return (returnData_out.Code, returnData_out.Result, medComboClasses);
        }

        static public List<medComboClass> add(string API_Server, List<medComboClass> medComboClasses, string serverName = "", string serverType = "")
        {
            return add_full(API_Server, medComboClasses, serverName, serverType).medCombos;
        }
        static public (int code, string result, List<medComboClass> medCombos) add_full(string API_Server, List<medComboClass> medComboClasses, string serverName = "", string serverType = "")
        {
            string url = $"{API_Server}/api/medCombo/add";

            returnData returnData = new returnData();
            returnData.Data = medComboClasses;
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
            medComboClasses = returnData_out.Data.ObjToClass<List<medComboClass>>();
            return (returnData_out.Code, returnData_out.Result, medComboClasses);
        }

        static public List<medComboClass> delete_by_guid(string API_Server, List<medComboClass> medComboClasses, string serverName = "", string serverType = "")
        {
            return delete_by_guid_full(API_Server, medComboClasses, serverName, serverType).medCombos;
        }
        static public (int code, string result, List<medComboClass> medCombos) delete_by_guid_full(string API_Server, List<medComboClass> medComboClasses, string serverName = "", string serverType = "")
        {
            string url = $"{API_Server}/api/medCombo/delete_by_guid";

            returnData returnData = new returnData();
            returnData.Data = medComboClasses;
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
            medComboClasses = returnData_out.Data.ObjToClass<List<medComboClass>>();
            return (returnData_out.Code, returnData_out.Result, medComboClasses);
        }
    }
}
