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
    [EnumDescription("person_Signature")]
    public enum enum_personSignature
    {
        [Description("GUID,VARCHAR,40,PRIMARY")]
        GUID,

        [Description("PersonGUID,VARCHAR,40,INDEX")]
        PersonGUID,

        [Description("SignatureBase64,LONGTEXT,50,NONE")]
        SignatureBase64,

        [Description("ContentType,VARCHAR,50,NONE")]
        ContentType,

        [Description("FileName,VARCHAR,100,NONE")]
        FileName,

        [Description("FileSize,VARCHAR,50,NONE")]
        FileSize,

        [Description("CreatBy,VARCHAR,40,NONE")]
        CreatBy,

        [Description("UpdateAt,DATETIME,20,NONE")]
        UpdateAt,

        [Description("CreatAt,DATETIME,20,NONE")]
        CreatAt
    }

    /// <summary>
    /// 人員簽名資料
    /// </summary>
    public class personSignature
    {
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        [JsonPropertyName("PersonGUID")]
        public string PersonGUID { get; set; }

        [JsonPropertyName("SignatureBase64")]
        public string SignatureBase64 { get; set; }

        [JsonPropertyName("ContentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("FileName")]
        public string FileName { get; set; }

        [JsonPropertyName("FileSize")]
        public string FileSize { get; set; }

        [JsonPropertyName("CreatBy")]
        public string CreatBy { get; set; }

        [JsonPropertyName("UpdateAt")]
        public DateTime? UpdateAt { get; set; }

        [JsonPropertyName("CreatAt")]
        public DateTime? CreatAt { get; set; }
    }

    #region 簽章維護log
    [EnumDescription("person_SignatureLog")]
    public enum enum_personSignatureLog
    {
        [Description("GUID,VARCHAR,40,PRIMARY")]
        GUID,

        [Description("SignatureGUID,VARCHAR,40,INDEX")]
        SignatureGUID,

        [Description("TargetPersonGUID,VARCHAR,40,INDEX")]
        TargetPersonGUID,

        [Description("OperatorGUID,VARCHAR,40,INDEX")]
        OperatorGUID,

        [Description("ActionType,VARCHAR,30,NONE")]
        ActionType,

        [Description("BeforeFileName,VARCHAR,255,NONE")]
        BeforeFileName,

        [Description("AfterFileName,VARCHAR,255,NONE")]
        AfterFileName,

        [Description("CreateAt,DATETIME,20,NONE")]
        CreateAt
    }

    /// <summary>
    /// 人員簽名異動紀錄
    /// </summary>
    public class person_SignatureLog
    {
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        [JsonPropertyName("SignatureGUID")]
        public string SignatureGUID { get; set; }

        /// <summary>
        /// 被修改簽章的人
        /// </summary>
        [JsonPropertyName("TargetPersonGUID")]
        public string TargetPersonGUID { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        [JsonPropertyName("OperatorGUID")]
        public string OperatorGUID { get; set; }

        /// <summary>
        /// CREATE / UPDATE / DELETE
        /// </summary>
        [JsonPropertyName("ActionType")]
        public string ActionType { get; set; }

        [JsonPropertyName("BeforeFileName")]
        public string BeforeFileName { get; set; }

        [JsonPropertyName("AfterFileName")]
        public string AfterFileName { get; set; }

        [JsonPropertyName("CreateAt")]
        public DateTime? CreateAt { get; set; }
    }
    #endregion
}
