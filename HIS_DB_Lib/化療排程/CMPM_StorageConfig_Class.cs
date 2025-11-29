using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.ComponentModel;


namespace HIS_DB_Lib
{
    [EnumDescription("storage_config")]
    public enum enum_CMPM_StorageConfig
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
    public class CMPM_StorageConfig_Class
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
    }
}
