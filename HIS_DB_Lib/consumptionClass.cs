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
    [EnumDescription("consumption")]
    public enum enum_consumption
    {
        [Description("藥碼,VARCHAR,20,INDEX")]
        藥碼,
        [Description("藥名,VARCHAR,300,NONE")]
        藥名,
        [Description("類別,VARCHAR,20,NONE")]
        類別,
        [Description("消耗量,VARCHAR,20,NONE")]
        消耗量,
        [Description("結存量,VARCHAR,20,NONE")]
        結存量,
        [Description("實調量,VARCHAR,20,NONE")]
        實調量,
        [Description("庫存量,VARCHAR,20,NONE")]
        庫存量,
    }
    [Description("consumption")]
    public class consumptionClass
    {
        [Description("VARCHAR,50,PRIMARY")]
        public string GUID { get; set; }

        [Description("VARCHAR,20,INDEX")]
        [JsonPropertyName("CODE")]
        public string 藥碼 { get; set; }

        [Description("VARCHAR,300,NONE")]
        [JsonPropertyName("NAME")]
        public string 藥名 { get; set; }

        [Description("VARCHAR,20,NONE")]
        [JsonPropertyName("TYPE")]
        public string 類別 { get; set; }

        [Description("VARCHAR,20,NONE")]
        [JsonPropertyName("CONSUMPTION")]
        public string 消耗量 { get; set; }

        [Description("VARCHAR,20,NONE")]
        [JsonPropertyName("BALANCE")]
        public string 結存量 { get; set; }

        [Description("VARCHAR,20,NONE")]
        [JsonPropertyName("DISPENSED")]
        public string 實調量 { get; set; }

        [Description("VARCHAR,20,NONE")]
        [JsonPropertyName("STOCK")]
        public string 庫存量 { get; set; }

        [Description("DATETIME,20,NONE")]
        [JsonPropertyName("ADD_TIME")]
        public string 建立時間 { get; set; }
        [JsonPropertyName("ANG_QTY")]
        public string 平均消耗量 { get; set; }

        static public (int code, string result, List<consumptionClass>) serch_by_ST_END_full(
            string API_Server,
            string serverName,
            string serverType,
            string startDate,
            string endDate,
            string groupName = "")
        {
            return serch_by_ST_END_full(
                API_Server,
                serverName,
                serverType,
                DateTime.Parse(startDate),
                DateTime.Parse(endDate),
                groupName
            );
        }

        static public (int code, string result, List<consumptionClass>) serch_by_ST_END_full(
            string API_Server,
            string serverName,
            string serverType,
            DateTime startDate,
            DateTime endDate,
            string groupName = "")
        {
            string url = $"{API_Server}/api/consumption/serch_by_ST_END";

            returnData sendData = new returnData();
            sendData.ServerName = serverName;
            sendData.ServerType = serverType;
            sendData.Method = "serch_by_ST_END";
            sendData.Value = $"{startDate.ToDateTimeString()},{endDate.ToDateTimeString()}";
            if (!string.IsNullOrWhiteSpace(groupName)) sendData.ValueAry = new List<string> { groupName };

            string json_in = sendData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
                return (0, "returnData_out == null", null);

            if (returnData_out.Data == null)
                return (0, "returnData_out.Data == null", null);

            List<consumptionClass> list = returnData_out.Data.ObjToClass<List<consumptionClass>>();
            return (returnData_out.Code, returnData_out.Result, list);
        }

        static public List<consumptionClass> serch_by_ST_END(
            string API_Server,
            string serverName,
            string serverType,
            string startDate,
            string endDate,
            string groupName = "")
        {
            var (code, result, list) = serch_by_ST_END_full(API_Server, serverName, serverType, startDate, endDate, groupName);
            return list;
        }

        static public List<consumptionClass> serch_by_ST_END(
            string API_Server,
            string serverName,
            string serverType,
            DateTime startDate,
            DateTime endDate,
            string groupName = "")
        {
            var (code, result, list) = serch_by_ST_END_full(API_Server, serverName, serverType, startDate, endDate, groupName);
            return list;
        }

    }
}
