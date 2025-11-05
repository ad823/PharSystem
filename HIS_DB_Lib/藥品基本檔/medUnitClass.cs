using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HIS_DB_Lib
{
    public enum enum_medUnit
    {
        GUID,
        Med_GUID,
        單位類型,
        單位名稱,
        換算數量,
        排序順序
    }
    /// <summary>
    /// 藥品單位資料類別  
    /// 對應各層級單位（採購、撥補、調劑）及其換算關係。
    /// </summary>
    [Description("medUnit")]
    public class medUnitClass   
    {
        /// <summary>
        /// 單位 GUID（唯一識別碼）
        /// </summary>
        [Description("GUID,VARCHAR,50,PRIMARY")]
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        /// <summary>
        /// 對應的藥品 GUID
        /// </summary>
        [Description("Med_GUID,VARCHAR,50,INDEX")]
        [JsonPropertyName("med_guid")]
        public string Med_GUID { get; set; }

        /// <summary>
        /// 單位類型（例如：採購、撥補、調劑）
        /// </summary>
        [Description("單位類型,VARCHAR,50,NONE")]
        [JsonPropertyName("unit_type")]
        public string 單位類型 { get; set; }

        /// <summary>
        /// 單位名稱（例如：箱、盒、顆）
        /// </summary>
        [Description("單位名稱,VARCHAR,50,NONE")]
        [JsonPropertyName("unit_name")]
        public string 單位名稱 { get; set; }

        /// <summary>
        /// 與上層單位的換算數量（若無則為 null）
        /// </summary>
        [Description("換算數量,VARCHAR,20,NONE")]
        [JsonPropertyName("quantity_per_parent")]
        public string 換算數量 { get; set; }

        /// <summary>
        /// 排序順序（數字越小越前面）
        /// </summary>
        [Description("排序順序,VARCHAR,10,NONE")]
        [JsonPropertyName("sort_order")]
        public string 排序順序 { get; set; }
    }
    public static class medUnitClassMethod
    {
        static public Dictionary<string, List<medUnitClass>> ToDictByMedGuid(this List<medUnitClass> medUnitClasses)
        {
            Dictionary<string, List<medUnitClass>> dictionary = new Dictionary<string, List<medUnitClass>>();
            foreach (var item in medUnitClasses)
            {
                if (dictionary.TryGetValue(item.Med_GUID, out List<medUnitClass> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.Med_GUID] = new List<medUnitClass> { item };
                }
            }
            return dictionary;
        }
        static public List<medUnitClass> GetByMasterGUID(this Dictionary<string, List<medUnitClass>> dict, string med_guid)
        {
            if (dict.TryGetValue(med_guid, out List<medUnitClass> medUnitClasses))
            {
                return medUnitClasses;
            }
            else
            {
                return new List<medUnitClass>();
            }
        }

    }
}
