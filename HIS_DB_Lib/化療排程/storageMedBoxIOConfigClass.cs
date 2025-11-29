using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.ComponentModel;
using SQLUI;


namespace HIS_DB_Lib
{
    [EnumDescription("storage_config")]
    public enum enum_storageMedBoxIOConfig
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("IP,VARCHAR,50,INDEX")]
        IP,
        [Description("鎖控輸出索引,VARCHAR,50,NONE")]
        鎖控輸出索引,
        [Description("鎖控輸出觸發,VARCHAR,50,NONE")]
        鎖控輸出觸發,
        [Description("鎖控輸入索引,VARCHAR,50,NONE")]
        鎖控輸入索引,
        [Description("鎖控輸入狀態,VARCHAR,50,NONE")]
        鎖控輸入狀態,
        [Description("出料馬達輸出索引,VARCHAR,50,NONE")]
        出料馬達輸出索引,
        [Description("出料馬達輸出觸發,VARCHAR,50,NONE")]
        出料馬達輸出觸發,
        [Description("出料馬達輸入索引,VARCHAR,50,NONE")]
        出料馬達輸入索引,
        [Description("出料馬達輸入狀態,VARCHAR,50,NONE")]
        出料馬達輸入狀態,
        [Description("出料馬達輸入延遲時間,VARCHAR,50,NONE")]
        出料馬達輸入延遲時間,
        [Description("出料位置X,VARCHAR,50,NONE")]
        出料位置X,
        [Description("出料位置Y,VARCHAR,50,NONE")]
        出料位置Y,
        [Description("藥盒方位,VARCHAR,50,NONE")]
        藥盒方位,
        [Description("區域,VARCHAR,50,NONE")]
        區域,
    }
    public class storageMedBoxIOConfigClass
    {
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        [JsonPropertyName("IP")]
        public string IP { get; set; }
        [JsonPropertyName("lock_output_index")]
        public string 鎖控輸出索引 { get; set; }
        [JsonPropertyName("lock_output_trigger")]
        public string 鎖控輸出觸發 { get; set; }
        [JsonPropertyName("lock_input_index")]
        public string 鎖控輸入索引 { get; set; }
        [JsonPropertyName("lock_input_state")]
        public string 鎖控輸入狀態 { get; set; }
        [JsonPropertyName("motor_output_index")]
        public string 出料馬達輸出索引 { get; set; }
        [JsonPropertyName("motor_output_trigger")]
        public string 出料馬達輸出觸發 { get; set; }
        [JsonPropertyName("motor_input_index")]
        public string 出料馬達輸入索引 { get; set; }
        [JsonPropertyName("motor_input_state")]
        public string 出料馬達輸入狀態 { get; set; }
        [JsonPropertyName("motor_input_delay_time")]
        public string 出料馬達輸入延遲時間 { get; set; }
        [JsonPropertyName("position_x")]
        public string 出料位置X { get; set; }
        [JsonPropertyName("position_y")]
        public string 出料位置Y { get; set; }
        [JsonPropertyName("box_direction")]
        public string 藥盒方位 { get; set; }
        [JsonPropertyName("area")]
        public string 區域 { get; set; }

        static public Table init(string API_Server, string serverName, string serverType)
        {
            return init_full(API_Server, serverName, serverType).table;
        }
        static public (int code, string result, Table table) init_full(string API_Server, string serverName, string serverType)
        {

            string url = $"{API_Server}/api/storageMedBoxIOConfig/init";
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
            Table out_value = returnData_out.Data.ObjToClass<Table>();

            return (returnData_out.Code, returnData_out.Result, out_value);
        }

        static public List<storageMedBoxIOConfigClass> get_all(string API_Server, string serverName, string serverType)
        {
            return get_all_full(API_Server,serverName,serverType).storageMedBoxes;
        }
        static public (int code, string result, List<storageMedBoxIOConfigClass> storageMedBoxes) get_all_full(string API_Server , string serverName,string serverType)
        {

            string url = $"{API_Server}/api/storageMedBoxIOConfig/get_all";
            returnData returnData = new returnData();
            returnData.ServerName = serverName ;    
            returnData.ServerType = serverType ;


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
            List<storageMedBoxIOConfigClass> out_value = returnData_out.Data.ObjToClass<List<storageMedBoxIOConfigClass>>();

            return (returnData_out.Code, returnData_out.Result, out_value);
        }

        static public List<storageMedBoxIOConfigClass> add_update(string API_Server, string serverName, string serverType, List<storageMedBoxIOConfigClass> storageMedBoxes)
        {
            return add_update_full(API_Server, serverName, serverType, storageMedBoxes).storageMedBoxes;
        }
        static public (int code, string result, List<storageMedBoxIOConfigClass> storageMedBoxes) add_update_full(string API_Server, string serverName, string serverType, List<storageMedBoxIOConfigClass> storageMedBoxes)
        {

            string url = $"{API_Server}/api/storageMedBoxIOConfig/add_update_full";
            returnData returnData = new returnData();
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;
            returnData.Data = storageMedBoxes;

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
            List<storageMedBoxIOConfigClass> out_value = returnData_out.Data.ObjToClass<List<storageMedBoxIOConfigClass>>();

            return (returnData_out.Code, returnData_out.Result, out_value);
        }

    }
}
