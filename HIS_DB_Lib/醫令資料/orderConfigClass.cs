using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.ComponentModel;


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

    }
}
