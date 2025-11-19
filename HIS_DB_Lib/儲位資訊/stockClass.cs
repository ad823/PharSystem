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
    /// 貨品表資料結構  
    /// 用於紀錄藥品在各層架或裝置上的位置、批號、效期與數量。
    /// </summary>
    [Description("stock")]
    public class stockClass
    {
        /// <summary>
        /// 唯一識別碼 (GUID)
        /// </summary>
        [Description("GUID,VARCHAR,50,PRIMARY")]
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        /// <summary>
        /// 對應的層架 GUID
        /// </summary>
        [Description("shelf_GUID,VARCHAR,50,INDEX")]
        [JsonPropertyName("shelf_guid")]
        public string Shelf_GUID { get; set; }

        /// <summary>
        /// 位置描述 (例如 上層第2層第3格)
        /// </summary>
        [Description("位置,VARCHAR,100,NONE")]
        [JsonPropertyName("location")]
        public string 位置 { get; set; }

        /// <summary>
        /// 裝置 IP 位址
        /// </summary>
        [Description("IP,VARCHAR,50,NONE")]
        [JsonPropertyName("ip")]
        public string IP { get; set; }

        /// <summary>
        /// 裝置類型 (例如 shelf、drawer、cabinet)
        /// </summary>
        [Description("device_type,VARCHAR,50,NONE")]
        [JsonPropertyName("device_type")]
        public string device_type { get; set; }

        /// <summary>
        /// 燈條亮燈位置 (LED Index)
        /// </summary>
        [Description("燈條亮燈位置,VARCHAR,50,NONE")]
        [JsonPropertyName("led_index")]
        public string 燈條亮燈位置 { get; set; }

        /// <summary>
        /// 藥品代碼
        /// </summary>
        [Description("藥碼,VARCHAR,50,INDEX")]
        [JsonPropertyName("code")]
        public string 藥碼 { get; set; }

        /// <summary>
        /// 藥品名稱
        /// </summary>
        [Description("藥名,VARCHAR,200,NONE")]
        [JsonPropertyName("name")]
        public string 藥名 { get; set; }

        /// <summary>
        /// 料號 (Material Number)
        /// </summary>
        [Description("料號,VARCHAR,100,NONE")]
        [JsonPropertyName("material_no")]
        public string 料號 { get; set; }

        /// <summary>
        /// 批號 (Lot Number)
        /// </summary>
        [JsonPropertyName("lot")]
        public List<string> 批號 { get; set; }

        /// <summary>
        /// 效期 (Expiry Date)
        /// </summary>
        [JsonPropertyName("expiry_date")]
        public List<string> 效期 { get; set; }

        /// <summary>
        /// 數量 (Quantity)
        /// </summary>
        [JsonPropertyName("qty")]
        public List<string> 數量 { get; set; }
        /// <summary>
        /// Value (DeviceBasic)
        /// </summary>
        [Description("VARCHAR,1000,NONE")]
        public string Value { get; set; }
        /// <summary>
        /// Classify_GUID
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        public string Classify_GUID { get; set; }
        /// <summary>
        /// Classify
        /// </summary>
        public medClassifyClass Classify { get; set; }
        /// <summary>
        /// 雲端藥檔
        /// </summary>
        public medClass med_cloud { get; set; }
        /// <summary>
        /// 藥品單位
        /// </summary>
        public List<medUnitClass> med_unit { get; set; }
    }
}
