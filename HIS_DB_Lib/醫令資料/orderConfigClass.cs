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
    public enum enum_orderConfig
    {
        GUID,
        Order_GUID,
        功能備註,
        狀態,
        更新時間
    }
    [Description("orderConfig")]
    public class orderConfigClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [Description("VARCHAR,50,PRIMARY")]
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Order_GUID
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        [JsonPropertyName("Order_GUID")]
        public string Order_GUID { get; set; }
        /// <summary>
        /// 功能備註
        /// </summary>
        [Description("VARCHAR,20,NONE")]
        [JsonPropertyName("note")]
        public string 功能備註 { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        [Description("VARCHAR,10,NONE")]

        [JsonPropertyName("status")]
        public string 狀態 { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        [Description("VARCHAR,20,NONE")]
        [JsonPropertyName("update_time")]
        public string 更新時間 { get; set; }
        static public returnData add(string API_Server, List<orderConfigClass> orderConfigClasses)
        {
            string url = $"{API_Server}/api/orderConfig/add";

            returnData returnData = new returnData();
            returnData.Data = orderConfigClasses;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public (int code, string result, List<orderConfigClass> orderConfigs) update(string API_Server, orderConfigClass orderConfig)
        {
            List<orderConfigClass> orderConfigClasses = new List<orderConfigClass>();
            orderConfigClasses.Add(orderConfig);
            return update(API_Server, orderConfigClasses);
        }
        static public (int code, string result, List<orderConfigClass> orderConfigs) update(string API_Server, List<orderConfigClass> orderConfigClasses)
        {
            string url = $"{API_Server}/api/orderConfig/update";

            returnData returnData = new returnData();
            returnData.Data = orderConfigClasses;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
                return (0, "returnData_out == null", null);

            if (returnData_out.Data == null)
                return (0, "returnData_out.Data == null", null);

            List<orderConfigClass> list = returnData_out.Data.ObjToClass<List<orderConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, list);
        }
        static public (int code, string result, List<orderConfigClass> orderConfigs) get_by_orderGUID(string API_Server, string order_guid)
        {
            List<string> order_guids = new List<string>();
            order_guids.Add(order_guid);
            return get_by_orderGUID(API_Server, order_guids);
        }
        static public (int code, string result, List<orderConfigClass> orderConfigs) get_by_orderGUID(string API_Server, List<string> order_guid)
        {
            string url = $"{API_Server}/api/orderConfig/get_by_orderGUID";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(string.Join(";", order_guid));
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
                return (0, "returnData_out == null", null);

            if (returnData_out.Data == null)
                return (0, "returnData_out.Data == null", null);

            List<orderConfigClass> list = returnData_out.Data.ObjToClass<List<orderConfigClass>>();
            return (returnData_out.Code, returnData_out.Result, list);
        }

    }
}
