using System;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using HIS_DB_Lib;
using System.Text.Json.Serialization;
using Basic;
using System.Linq;

/// <summary>
/// 化療處方資料 (Chemotherapy Order)
/// </summary>
[Description("chemotherapy_orders")]
public class chemotherapyOrderClass
{
    /// <summary>唯一識別碼 (GUID)</summary>
    [JsonPropertyName("GUID")]
    [Description("VARCHAR,50,PRIMARY")]
    public string GUID { get; set; }

    [JsonPropertyName("PRI_KEY")]
    [Description("VARCHAR,50,INDEX")]
    public string PRI_KEY { get; set; }

    /// <summary>BARCODE</summary>
    [JsonPropertyName("BARCODE")]
    [Description("VARCHAR,50,INDEX")]
    public string BARCODE { get; set; }

    /// <summary>化療處方號</summary>
    [JsonPropertyName("CHE_CHEKEY")]
    [Description("VARCHAR,10,INDEX")]
    public string 化療處方號 { get; set; }

    /// <summary>化療處方流水號</summary>
    [JsonPropertyName("CHE_RECNO")]
    [Description("VARCHAR,22,INDEX")]
    public string 化療處方流水號 { get; set; }

    /// <summary>病歷號碼</summary>
    [JsonPropertyName("CHE_PATID")]
    [Description("VARCHAR,10,INDEX")]
    public string 病歷號 { get; set; }

    /// <summary>處方日期</summary>
    [JsonPropertyName("CHE_VISITDT")]
    [Description("DATETIME,20,INDEX")]
    public string 處方日期 { get; set; }

    /// <summary>門診序號 / 住院號</summary>
    [JsonPropertyName("CHE_SEQ")]
    [Description("VARCHAR,10,INDEX")]
    public string 門診住院序號 { get; set; }

    /// <summary>處方來源 (1.門診 2.住院 3.急診)</summary>
    [JsonPropertyName("CHE_KIND")]
    [Description("VARCHAR,5,NONE")]
    public string 處方來源 { get; set; }

    /// <summary>類型 (1.化療前 2.化療 3.化療後)</summary>
    [JsonPropertyName("CHE_CD1TYPE")]
    [Description("VARCHAR,1,NONE")]
    public string 類型_化療階段 { get; set; }

    /// <summary>醫令順序流水號</summary>
    [JsonPropertyName("CHE_CD1PSRNO")]
    [Description("VARCHAR,22,INDEX")]
    public string 醫令順序流水號 { get; set; }

    /// <summary>開始執行日期</summary>
    [JsonPropertyName("CHE_STARTDT")]
    [Description("DATETIME,20,INDEX")]
    public string 開始執行日期 { get; set; }

    /// <summary>結束執行日期</summary>
    [JsonPropertyName("CHE_ENDDT")]
    [Description("DATETIME,20,INDEX")]
    public string 結束執行日期 { get; set; }

    /// <summary>化療處方名稱</summary>
    [JsonPropertyName("CHE_REGIMEN")]
    [Description("VARCHAR,100,NONE")]
    public string 化療處方名稱 { get; set; }

    /// <summary>醫令代碼</summary>
    [JsonPropertyName("CHE_DIACODE")]
    [Description("VARCHAR,10,INDEX")]
    public string 藥碼 { get; set; }

    /// <summary>醫令名稱</summary>
    [JsonPropertyName("CHE_EGNAME")]
    [Description("VARCHAR,120,NONE")]
    public string 藥品名稱 { get; set; }

    /// <summary>次劑量</summary>
    [JsonPropertyName("CHE_QTY_PERTIME")]
    [Description("VARCHAR,22,NONE")]
    public string 次劑量 { get; set; }

    /// <summary>頻率</summary>
    [JsonPropertyName("CHE_FEQNO")]
    [Description("VARCHAR,8,NONE")]
    public string 頻次 { get; set; }

    /// <summary>途徑</summary>
    [JsonPropertyName("CHE_PATHNO")]
    [Description("VARCHAR,10,NONE")]
    public string 用藥途徑 { get; set; }

    /// <summary>總量</summary>
    [JsonPropertyName("CHE_SUMQTY")]
    [Description("VARCHAR,22,NONE")]
    public string 總量 { get; set; }

    /// <summary>是否自費 (Y/N)</summary>
    [JsonPropertyName("CHE_SELF_PAY")]
    [Description("VARCHAR,10,NONE")]
    public string 費用別 { get; set; }

    /// <summary>流速</summary>
    [JsonPropertyName("CHE_FLOW_RATE")]
    [Description("VARCHAR,10,NONE")]
    public string 流速 { get; set; }

    /// <summary>總時間 (單位: 分鐘)</summary>
    [JsonPropertyName("CHE_TIMESPAN")]
    [Description("VARCHAR,22,NONE")]
    public string 總時間分鐘 { get; set; }

    /// <summary>建立時間</summary>
    [JsonPropertyName("created_at")]
    [Description("DATETIME,20,NONE")]
    public string 建立時間 { get; set; }

    /// <summary>更新時間</summary>
    [JsonPropertyName("updated_at")]
    [Description("DATETIME,20,NONE")]
    public string 更新時間 { get; set; }

    /// <summary>每日詳細紀錄 (子表)</summary>
    [JsonPropertyName("day_records")]
    public List<chemotherapyOrderDayClass> 每日紀錄 { get; set; } = new List<chemotherapyOrderDayClass>();

    static public returnData update_order_list(string API_Server, List<chemotherapyOrderClass> chemotherapyOrderClasses)
    {
        string url = $"{API_Server}/api/chemotherapyOrder/update_order_list";

        returnData returnData = new returnData();
        returnData.Data = chemotherapyOrderClasses;

        string json_in = returnData.JsonSerializationt();
        string json_out = Net.WEBApiPostJson(url, json_in);
        returnData = json_out.JsonDeserializet<returnData>();  
        return returnData;
    }
    static public returnData get_by_barcode(string API_Server,string barcode)
    {
        string url = $"{API_Server}/api/chemotherapyOrder/get_by_barcode";

        returnData returnData = new returnData();
        returnData.ValueAry.Add(barcode);

        string json_in = returnData.JsonSerializationt();
        string json_out = Net.WEBApiPostJson(url, json_in);
        returnData = json_out.JsonDeserializet<returnData>();
        return returnData;
    }

    static public returnData update_chemotherapyOrderDay_by_guid(string API_Server, List<chemotherapyOrderDayClass>  chemotherapyOrderDays)
    {
        string url = $"{API_Server}/api/chemotherapyOrder/update_chemotherapyOrderDay_by_guid";

        returnData returnData = new returnData();
        returnData.Data = chemotherapyOrderDays;

        string json_in = returnData.JsonSerializationt();
        string json_out = Net.WEBApiPostJson(url, json_in);
        returnData = json_out.JsonDeserializet<returnData>();
        return returnData;
    }

}


static public class chemotherapyOrderClassMethod
{
    public static List<DateTime> GetOrderAllDates(this List<chemotherapyOrderClass> orders)
    {
        List<DateTime> result = new List<DateTime>();

        foreach (var order in orders)
        {
            if (DateTime.TryParse(order.開始執行日期, out DateTime s) &&
                DateTime.TryParse(order.結束執行日期, out DateTime e))
            {
                for (DateTime d = s.Date; d <= e.Date; d = d.AddDays(1))
                {
                    result.Add(d);
                }
            }
        
        }

        return result
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }
}