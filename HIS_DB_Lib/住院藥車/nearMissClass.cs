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
    [EnumDescription("nearMiss")]
    public enum enum_nearMiss
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("pat_GUID,VARCHAR,50,INDEX")]
        pat_GUID,
        [Description("cpoe_GUID,VARCHAR,50,INDEX")]
        cpoe_GUID,
        [Description("藥局,VARCHAR,20,NONE")]
        藥局,
        [Description("護理站,VARCHAR,20,NONE")]
        護理站,
        [Description("調劑人ID,VARCHAR,20,NONE")]
        調劑人ID,
        [Description("調劑人姓名,VARCHAR,20,NONE")]
        調劑人姓名,
        [Description("通報人ID,VARCHAR,20,NONE")]
        通報人ID,
        [Description("通報人姓名,VARCHAR,20,NONE")]
        通報人姓名,
        [Description("建立時間,DATETIME,30,NONE")]
        建立時間,
        [Description("原因,VARCHAR,300,NONE")]
        原因,
        [Description("備註,VARCHAR,500,NONE")]
        備註,
    }
    public class nearMissClass
    {
        /// <summary>
        /// GUID
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// 病人GUID
        /// </summary>
        [JsonPropertyName("pat_GUID")]
        public string pat_GUID { get; set; }
        /// <summary>
        /// 處方GUID
        /// </summary>
        [JsonPropertyName("cpoe_GUID")]
        public string cpoe_GUID { get; set; }
        /// <summary>
        /// 藥局
        /// </summary>
        [JsonPropertyName("pharm")]
        public string 藥局 { get; set; }
        /// <summary>
        /// 護理站
        /// </summary>
        [JsonPropertyName("nurnum")]
        public string 護理站 { get; set; }
        /// <summary>
        /// 調劑人ID
        /// </summary>
        [JsonPropertyName("disp_id")]
        public string 調劑人ID { get; set; }
        /// <summary>
        /// 調劑人姓名
        /// </summary>
        [JsonPropertyName("disp_name")]
        public string 調劑人姓名 { get; set; }
        /// <summary>
        /// 通報人ID
        /// </summary>
        [JsonPropertyName("reporter_id")]
        public string 通報人ID { get; set; }
        /// <summary>
        /// 通報人姓名
        /// </summary>
        [JsonPropertyName("reporter_name")]
        public string 通報人姓名 { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonPropertyName("creat_time")]
        public string 建立時間 { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        [JsonPropertyName("reason")]
        public string 原因 { get; set; }
        [JsonPropertyName("note")]
        public string 備註 { get; set; }
        public patientInfoClass patientInfo{ get; set; }
        public medCpoeClass medCpoe { get; set; }
    }
    public enum enum_nearmiss_report
    {
        藥局 = 0,
        護理站,
        調劑人ID,
        調劑人姓名,
        通報人ID,
        通報人姓名,
        建立時間,
        原因,
        備註,
        姓名,
        床號,
        住院號,
        病歷號,
        序號,
        開始時間,
        結束時間,
        藥碼,
        頻次,
        藥品名,
        中文名,
        途徑,
        數量,
        劑量,
        單位,
        自購,
        處方醫師,
        處方醫師姓名
    }
}
