using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

/// <summary>
/// 化療處方每日紀錄 (Chemotherapy Order Day Record)
/// </summary>
[Description("chemotherapy_order_days")]
public class chemotherapyOrderDayClass
{
    /// <summary>唯一識別碼 (GUID)</summary>
    [JsonPropertyName("GUID")]
    [Description("VARCHAR,50,PRIMARY")]
    public string GUID { get; set; }

    /// <summary>主表化療處方 GUID (chemotherapy_orders.GUID)</summary>
    [JsonPropertyName("order_guid")]
    [Description("VARCHAR,50,INDEX")]
    public string 主表GUID { get; set; }

    /// <summary>第幾天 (1~15)</summary>
    [JsonPropertyName("day_no")]
    [Description("VARCHAR,5,INDEX")]
    public string 第幾天 { get; set; }

    /// <summary>該日是否使用 (Y/N)</summary>
    [JsonPropertyName("is_used")]
    [Description("VARCHAR,10,NONE")]
    public string 是否使用 { get; set; }

    /// <summary>審核藥師</summary>
    [JsonPropertyName("verify_pharm")]
    [Description("VARCHAR,50,NONE")]
    public string 審核藥師 { get; set; }

    /// <summary>審核時間</summary>
    [JsonPropertyName("verify_time")]
    [Description("DATETIME,20,NONE")]
    public string 審核時間 { get; set; }

    /// <summary>調劑藥師</summary>
    [JsonPropertyName("dispense_pharm")]
    [Description("VARCHAR,50,NONE")]
    public string 調劑藥師 { get; set; }

    /// <summary>調劑時間</summary>
    [JsonPropertyName("dispense_time")]
    [Description("DATETIME,20,NONE")]
    public string 調劑時間 { get; set; }

    /// <summary>核對藥師</summary>
    [JsonPropertyName("check_pharm")]
    [Description("VARCHAR,50,NONE")]
    public string 核對藥師 { get; set; }

    /// <summary>核對時間</summary>
    [JsonPropertyName("check_time")]
    [Description("DATETIME,20,NONE")]
    public string 核對時間 { get; set; }

    /// <summary>建立時間</summary>
    [JsonPropertyName("created_at")]
    [Description("DATETIME,20,NONE")]
    public string 建立時間 { get; set; }

    /// <summary>更新時間</summary>
    [JsonPropertyName("updated_at")]
    [Description("DATETIME,20,NONE")]
    public string 更新時間 { get; set; }
}
