using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HIS_DB_Lib
{
    /// <summary>
    /// 藥車交車LOG
    /// </summary>
    [Description("med_carlog")]
    public class med_carlogClass
    {
        /// <summary>
        /// 唯一識別碼 (GUID)
        /// </summary>
        [Description("VARCHAR,50,PRIMARY")]
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        /// <summary>
        ///  藥局
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        [JsonPropertyName("pharm")]
        public string 藥局 { get; set; }

        /// <summary>
        /// 護理站
        /// </summary>
        [Description("VARCHAR,100,NONE")]
        [JsonPropertyName("nurnum")]
        public string 護理站 { get; set; }

        /// <summary>
        /// 傳送人員ID
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        [JsonPropertyName("senderId")]
        public string 傳送人員ID { get; set; }

        /// <summary>
        /// 傳送人員姓名
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        [JsonPropertyName("senderName")]
        public string 傳送人員姓名 { get; set; }

        /// <summary>
        /// 傳送時間
        /// </summary>
        [Description("DATETIME,50,NONE")]
        [JsonPropertyName("sendTime")]
        public string 傳送時間 { get; set; }

        /// <summary>
        /// 護理站人員ID
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        [JsonPropertyName("nurseId")]
        public string 護理站人員ID { get; set; }

        /// <summary>
        /// 護理站人員姓名
        /// </summary>
        [Description("VARCHAR,200,NONE")]
        [JsonPropertyName("nurseName")]
        public string 護理站人員姓名 { get; set; }

        /// <summary>
        /// 護理站簽收時間
        /// </summary>
        [Description("DATETIME,100,NONE")]
        [JsonPropertyName("nurseReceiveTime")]
        public string 護理站簽收時間 { get; set; }
    }
}
