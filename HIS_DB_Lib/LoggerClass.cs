using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using System.Text.Json.Serialization;
using System.ComponentModel;


namespace HIS_DB_Lib
{
    [EnumDescription("logger")]
    public enum enum_logger
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("來源,VARCHAR,250,NONE")]
        來源,
        [Description("網址,VARCHAR,250,NONE")]
        網址,
        [Description("事件,VARCHAR,100,INDEX")]
        事件,
        [Description("操作類型,VARCHAR,50,INDEX")]
        操作類型,
        [Description("操作者ID,VARCHAR,100,NONE")]
        操作者ID,
        [Description("操作者姓名,VARCHAR,100,INDEX")]
        操作者姓名,
        [Description("操作時間,DATETIME,50,INDEX")]
        操作時間,
        [Description("回傳結果,VARCHAR,1000,NONE")]
        回傳結果
    }
    public class loggerClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// 來源
        /// </summary>
        [JsonPropertyName("source")]
        public string 來源 { get; set; }
        /// <summary>
        /// 網址
        /// </summary>
        [JsonPropertyName("url")]
        public string 網址 { get; set; }
        /// <summary>
        /// 事件
        /// </summary>
        [JsonPropertyName("event")]
        public string 事件 { get; set; }
        /// <summary>
        /// 操作類型
        /// </summary>
        [JsonPropertyName("op_type")]
        public string 操作類型 { get; set; }
        /// <summary>
        /// 操作者ID
        /// </summary>
        [JsonPropertyName("op_id")]
        public string 操作者ID { get; set; }
        /// <summary>
        /// 操作者姓名
        /// </summary>
        [JsonPropertyName("op_name")]
        public string 操作者姓名 { get; set; }
        /// <summary>
        /// 操作時間
        /// </summary>
        [JsonPropertyName("op_time")]
        public string 操作時間 { get; set; }
        /// <summary>
        /// 時間
        /// </summary>
        [JsonPropertyName("result")]
        public string 回傳結果 { get; set; }


        public class ICP_By_OP_Time : IComparer<loggerClass>
        {
            public int Compare(loggerClass x, loggerClass y)
            {
                return x.操作時間.CompareTo(y.操作時間);
            }
        }
    }
}
