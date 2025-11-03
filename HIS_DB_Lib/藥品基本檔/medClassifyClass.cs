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
    /// 藥品安全量/基準量分類 
    /// 用於紀錄藥品在各層架或裝置上的位置、批號、效期與數量。
    /// </summary>
    [Description("medClassify")]
    public class medClassifyClass
    {
        /// <summary>
        /// GUID（唯一識別碼）
        /// </summary>
        [Description("VARCHAR,50,INDEX")]
        public string GUID { get; set; }
        /// <summary>
        /// 分類名稱
        /// </summary>
        [Description("VARCHAR,100,NONE")]
        [JsonPropertyName("name")]
        public string 分類名稱 { get; set; }
        /// <summary>
        /// 安全量天數
        /// </summary>
        [Description("VARCHAR,10,NONE")]
        [JsonPropertyName("safe_day")]
        public string 安全量天數 { get; set; }
        /// <summary>
        /// 基準量天數
        /// </summary>
        [Description("VARCHAR,10,NONE")]
        [JsonPropertyName("standard_day")]
        public string 基準量天數 { get; set; }

    }
}
