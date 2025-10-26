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
    //病床處方
    //Computerized Physician Order Entry
    [EnumDescription("med_cpoe")]
    public enum enum_med_cpoe
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("PRI_KEY,VARCHAR,200,INDEX")]
        PRI_KEY,
        [Description("更新時間,DATETIME,10,NONE")]
        更新時間,
        [Description("調劑狀態,VARCHAR,10,NONE")]
        調劑狀態,
        [Description("公藥,VARCHAR,10,NONE")]
        公藥,
        [Description("藥局,VARCHAR,10,INDEX")]
        藥局,
        [Description("護理站,VARCHAR,10,INDEX")]
        護理站,
        [Description("姓名,VARCHAR,50,NONE")]
        姓名,
        [Description("床號,VARCHAR,10,NONE")]
        床號,
        [Description("住院號,VARCHAR,50,INDEX")]
        住院號,
        [Description("病歷號,VARCHAR,100,NONE")]
        病歷號,
        [Description("序號,VARCHAR,150,NONE")]
        序號,
        [Description("狀態,VARCHAR,10,NONE")]
        狀態,
        [Description("開始時間,DATETIME,20,NONE")]
        開始時間,
        [Description("結束時間,DATETIME,20,NONE")]
        結束時間,
        [Description("藥碼,VARCHAR,50,NONE")]
        藥碼,
        [Description("頻次,VARCHAR,10,NONE")]
        頻次,
        [Description("藥品名,VARCHAR,150,NONE")]
        藥品名,
        [Description("中文名,VARCHAR,150,NONE")]
        中文名,
        [Description("途徑,VARCHAR,30,NONE")]
        途徑,
        [Description("數量,VARCHAR,10,NONE")]
        數量,
        [Description("劑量,VARCHAR,10,NONE")]
        劑量,
        [Description("單位,VARCHAR,10,NONE")]
        單位,
        [Description("儲位,VARCHAR,50,NONE")]
        儲位,
        [Description("自購,VARCHAR,10,NONE")]
        自購,
        [Description("處方醫師,VARCHAR,10,NONE")]
        處方醫師,
        [Description("處方醫師姓名,VARCHAR,10,NONE")]
        處方醫師姓名,
        [Description("註記,VARCHAR,10,NONE")]
        註記,
        [Description("大瓶點滴,VARCHAR,10,NONE")]
        大瓶點滴,
        [Description("LKFLAG,VARCHAR,10,NONE")]
        LKFLAG,
        [Description("排序,VARCHAR,10,NONE")]
        排序,
        [Description("勿磨,VARCHAR,10,NONE")]
        勿磨,
        [Description("重複用藥,VARCHAR,10,NONE")]
        重複用藥,
        [Description("DC確認,VARCHAR,10,NONE")]
        DC確認,
        [Description("調劑異動,VARCHAR,10,NONE")]
        調劑異動,
        [Description("覆核狀態,VARCHAR,10,NONE")]
        覆核狀態,
        [Description("備註,VARCHAR,200,NONE")]
        備註,
    }
    public enum enum_med_cpoe_export
    {
        藥碼,
        藥品名,
        中文名,
        數量
    }
    public enum 藥品總量群組
    {
        針劑,
        大瓶藥,
        冷藏,
        口服,
        外用
    }
    /// <summary>
    /// medCpoeClass資料
    /// </summary>
    public class medCpoeClass
    {
        /// <summary>
        /// GUID
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Master_GUID
        /// </summary>
        [JsonPropertyName("Master_GUID")]
        public string Master_GUID { get; set; }
        /// <summary>
        /// PRI_KEY
        /// </summary>
        [JsonPropertyName("PRI_KEY")]
        public string PRI_KEY { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        [JsonPropertyName("update_time")]
        public string 更新時間 { get; set; }
        /// <summary>
        /// 調劑狀態
        /// </summary>
        [JsonPropertyName("dispens_status")]
        public string 調劑狀態 { get; set; }
        /// <summary>
        /// 公藥
        /// </summary>
        [JsonPropertyName("pub_med")]
        public string 公藥 { get; set; }
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
        /// 姓名
        /// </summary>
        [JsonPropertyName("pnamec")]
        public string 姓名 { get; set; }
        /// <summary>
        /// 床號
        /// </summary>
        [JsonPropertyName("bednum")]
        public string 床號 { get; set; }
        /// <summary>
        /// 住院號
        /// </summary>
        [JsonPropertyName("caseno")]
        public string 住院號 { get; set; }
        /// <summary>
        /// 病歷號
        /// </summary>
        [JsonPropertyName("histno")]
        public string 病歷號 { get; set; }
        /// <summary>
        /// 序號
        /// </summary>
        [JsonPropertyName("ordseq")]
        public string 序號 { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        [JsonPropertyName("status")]
        public string 狀態 { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        [JsonPropertyName("starttm")]
        public string 開始時間 { get; set; }
        /// <summary>
        /// 結束時間
        /// </summary>
        [JsonPropertyName("endtm")]
        public string 結束時間 { get; set; }
        /// <summary>
        /// 藥碼
        /// </summary>
        [JsonPropertyName("code")]
        public string 藥碼 { get; set; }
        /// <summary>
        /// 頻次代碼
        /// </summary>
        [JsonPropertyName("freqn")]
        public string 頻次 { get; set; }
        /// <summary>
        /// 藥品名
        /// </summary>
        [JsonPropertyName("name")]
        public string 藥品名 { get; set; }
        /// <summary>
        /// 中文名
        /// </summary>
        [JsonPropertyName("cht_name")]
        public string 中文名 { get; set; }
        /// <summary>
        /// 途徑
        /// </summary>
        [JsonPropertyName("route")]
        public string 途徑 { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        [JsonPropertyName("qty")]
        public string 數量 { get; set; }
        /// <summary>
        /// 劑量
        /// </summary>
        [JsonPropertyName("dosage")]
        public string 劑量 { get; set; }
        /// <summary>
        /// 單位
        /// </summary>
        [JsonPropertyName("dunit")]
        public string 單位 { get; set; }
        /// <summary>
        /// 儲位
        /// </summary>
        [JsonPropertyName("store_position")]
        public string 儲位 { get; set; }
        /// <summary>
        /// 自購
        /// </summary>
        [JsonPropertyName("self")]
        public string 自購 { get; set; }
        /// <summary>
        /// 處方醫師
        /// </summary>
        [JsonPropertyName("orsign")]
        public string 處方醫師 { get; set; }
        /// <summary>
        /// 處方醫師姓名
        /// </summary>
        [JsonPropertyName("signam")]
        public string 處方醫師姓名 { get; set; }
        /// <summary>
        /// 註記
        /// </summary>
        [JsonPropertyName("remark")]
        public string 註記 { get; set; }
        /// <summary>
        /// 大瓶點滴
        /// </summary>
        [JsonPropertyName("large")]
        public string 大瓶點滴 { get; set; }
        /// <summary>
        /// LKFLAG
        /// </summary>
        [JsonPropertyName("lkflag")]
        public string LKFLAG { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [JsonPropertyName("rank")]
        public string 排序 { get; set; }
        /// <summary>
        /// 勿磨
        /// </summary>
        [JsonPropertyName("udngt")]
        public string 勿磨 { get; set; }
        /// <summary>
        /// 重複用藥
        /// </summary>
        [JsonPropertyName("samedg")]
        public string 重複用藥 { get; set; }
        /// <summary>
        /// DC確認
        /// </summary>
        [JsonPropertyName("dc_check")]
        public string DC確認 { get; set; }
        /// <summary>
        /// 調劑異動
        /// </summary>
        [JsonPropertyName("dispens_change")]
        public string 調劑異動 { get; set; }
        /// <summary>
        /// 覆核狀態
        /// </summary>
        [JsonPropertyName("check_status")]
        public string 覆核狀態 { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        [JsonPropertyName("note")]
        public string 備註 { get; set; }
        /// <summary>
        /// 調劑台
        /// </summary>
        [JsonPropertyName("dispens_name")]
        public string 調劑台 { get; set; }
        /// <summary>
        /// 雲端藥檔
        /// </summary>
        [JsonPropertyName("med_cloud")]
        public List<medClass> 雲端藥檔 { get; set; }
        /// <summary>
        /// 藥品價格
        /// </summary>
        [JsonPropertyName("medprice")]
        public List<medPriceClass> 藥品價格 { get; set; }
        /// <summary>
        /// 調劑紀錄
        /// </summary>
        [JsonPropertyName("med_inve_log")]
        public List<medInventoryLogClass> 調劑紀錄 { get; set; }
        [JsonPropertyName("bed_list")]
        public List<bedListClass> 病床清單 { get; set; }
        [JsonPropertyName("prn")]
        public string PRN註記 { get; set; }
        [JsonPropertyName("prn_self")]
        public string 自費PRN註記 { get; set; }
        [JsonPropertyName("med_group")]
        public List<string> 藥品群組   { get; set; }
        [JsonPropertyName("nearmiss")]
        public nearMissClass nearmiss { get; set; }


        public class ICP_By_Rank : IComparer<medCpoeClass>
        {
            public int Compare(medCpoeClass x, medCpoeClass y)
            {
                int result = string.Compare(x.排序, y.排序);

                // 如果 排序 相同，則依照 藥名 排序
                if (result == 0)
                {
                    result = string.Compare(x.藥品名, y.藥品名);
                }

                return result;
            }
        }
        public class ICP_By_bedNum : IComparer<medCpoeClass>
        {
            public int Compare(medCpoeClass x, medCpoeClass y)
            {
                int result = (x.藥局).CompareTo(y.藥局);
                if(result == 0)
                {
                    result = (x.護理站).CompareTo(y.護理站);
                    if(result == 0)
                    {
                        result = (x.床號.StringToInt32()).CompareTo(y.床號.StringToInt32());
                        if(result == 0)
                        {
                            result = string.Compare(x.更新時間, y.更新時間) * -1;
                        }
                    }
                }

                return result;
            }
        }
        public class ICP_By_medName : IComparer<medCpoeClass>
        {
            public int Compare(medCpoeClass x, medCpoeClass y)
            {
                int result = (x.藥品名).CompareTo(y.藥品名);
                if (result == 0)
                {
                    result = (x.中文名).CompareTo(y.中文名);
                    
                }
                return result;
            }
        }
        static public returnData debit(string API_Server, string 操作人, string 調劑台, string GUID)
        {
            string url = $"{API_Server}/api/med_cart/debit";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(GUID);
            returnData.UserName = 操作人;
            returnData.ServerName = 調劑台;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public returnData refund(string API_Server, string 操作人, string 調劑台, string GUID)
        {
            string url = $"{API_Server}/api/med_cart/refund";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(GUID);
            returnData.UserName = 操作人;
            returnData.ServerName = 調劑台;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public returnData update_med_cpoe(string API_Server, List<medCpoeClass> medCpoeClasses)
        {
            List<medCpoeClass> out_medCpoeClass = new List<medCpoeClass>();
            string url = $"{API_Server}/api/med_cart/update_med_cpoe";

            returnData returnData = new returnData();
            returnData.Data = medCpoeClasses;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            //if (returnData == null) return null;
            //if (returnData.Code != 200) return null;
            out_medCpoeClass = returnData.Data.ObjToClass<List<medCpoeClass>>();
            Console.WriteLine($"{returnData}");
            return returnData;
        }
        static public returnData update_med_cpoe_for_check(string API_Server, List<medCpoeClass> medCpoeClasses)
        {
            List<medCpoeClass> out_medCpoeClass = new List<medCpoeClass>();
            string url = $"{API_Server}/api/med_cart/update_med_cpoe_for_check";

            returnData returnData = new returnData();
            returnData.Data = medCpoeClasses;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            //if (returnData == null) return null;
            //if (returnData.Code != 200) return null;
            out_medCpoeClass = returnData.Data.ObjToClass<List<medCpoeClass>>();
            Console.WriteLine($"{returnData}");
            return returnData;
        }
        static public returnData update_med_cpoe_New(string API_Server, List<medCpoeClass> medCpoeClasses)
        {
            List<medCpoeClass> out_medCpoeClass = new List<medCpoeClass>();
            string url = $"{API_Server}/api/med_cart/update_med_cpoe_New";

            returnData returnData = new returnData();
            returnData.Data = medCpoeClasses;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            //if (returnData == null) return null;
            //if (returnData.Code != 200) return null;
            //out_medCpoeClass = returnData.Data.ObjToClass<List<medCpoeClass>>();
            Console.WriteLine($"{returnData}");
            return returnData;
        }
        static public returnData update_med_cpoe_DC_by_medChange(string API_Server, string 藥局, string 護理站)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(藥局);
            returnData.ValueAry.Add(護理站);
            string url = $"{API_Server}/api/med_cart/update_med_cpoe_DC_by_medChange";

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            //if (returnData == null) return null;
            //if (returnData.Code != 200) return null;
            //Console.WriteLine($"{returnData}");
            return returnData;
        }
        static public returnData get_patient_with_NOdispens_summary_dispense(string API_Server, string 藥局, string 護理站)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(藥局);
            returnData.ValueAry.Add(護理站);
            string url = $"{API_Server}/api/med_cart/get_patient_with_NOdispens_summary_dispense";

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            //if (returnData == null) return null;
            //if (returnData.Code != 200) return null;
            //Console.WriteLine($"{returnData}");
            return returnData;
        }

        static public List<medCpoeClass> add_med_cpoe(string API_Server, List<medCpoeClass> medCpoeClasses)
        {
            List<medCpoeClass> out_medCpoeClass = new List<medCpoeClass>();
            string url = $"{API_Server}/api/med_cart/add_med_cpoe";

            returnData returnData = new returnData();
            returnData.Data = medCpoeClasses;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200) return null;
            out_medCpoeClass = returnData.Data.ObjToClass<List<medCpoeClass>>();
            Console.WriteLine($"{returnData}");
            return out_medCpoeClass;
        }

        static public List<medCpoeClass> get_med_qty(string API_Server, string value, List<string> valueAry)
        {
            string url = $"{API_Server}/api/med_cart/get_med_qty";
            returnData returnData = new returnData();
            returnData.ValueAry = valueAry;
            returnData.Value = value;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200) return null;
            List<medCpoeClass> out_medQtyClass = returnData.Data.ObjToClass<List<medCpoeClass>>();
            Console.WriteLine($"{returnData}");
            return out_medQtyClass;
        }
        static public returnData handover(string API_Server, List<string> valueAry)
        {
            string url = $"{API_Server}/api/med_cart/handover";

            returnData returnData = new returnData();
            returnData.ValueAry = valueAry;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            //if (returnData == null) return returnData;
            //if (returnData.Code != 200) return returnData;

            //medCarListClass medCarListClass = returnData.Data.ObjToClass<medCarListClass>();
            return returnData;
        }
        static public List<medCpoeClass> get_medCpoe(string API_Server)
        {
            List<medCpoeClass> out_medCpoeClass = new List<medCpoeClass>();
            string url = $"{API_Server}/api/med_cart/get_medCpoe";

            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200) return null;
            out_medCpoeClass = returnData.Data.ObjToClass<List<medCpoeClass>>();
            Console.WriteLine($"{returnData}");
            return out_medCpoeClass;
        }
        static public List<medClass> get_med_clouds_by_codes(string API_Server, List<string> code)
        {
            string url = $"{API_Server}/api/MED_page/get_med_clouds_by_codes";
            returnData returnData = new returnData();
            returnData.ValueAry = code;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200) return null;
            List<medClass> out_medClass = new List<medClass>();
            out_medClass = returnData.Data.ObjToClass<List<medClass>>();
            Console.WriteLine($"{returnData}");
            return out_medClass;
        }
        static public Dictionary<string, List<medCpoeClass>> ToDictByMasterGUID(List<medCpoeClass> medCpoeClasses)
        {
            Dictionary<string, List<medCpoeClass>> dictionary = new Dictionary<string, List<medCpoeClass>>();
            foreach (var item in medCpoeClasses)
            {
                if (dictionary.TryGetValue(item.Master_GUID, out List<medCpoeClass> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.Master_GUID] = new List<medCpoeClass> { item };
                }
            }
            return dictionary;
        }
        static public List<medCpoeClass> GetByMasterGUID(Dictionary<string, List<medCpoeClass>> dict, string master_GUID)
        {
            if (dict.TryGetValue(master_GUID, out List<medCpoeClass> medCpoeClasses))
            {
                return medCpoeClasses;
            }
            else
            {
                return new List<medCpoeClass>();
            }
        }

    }
    public class bedListClass
    {
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        [JsonPropertyName("Master_GUID")]
        public string Master_GUID { get; set; }
        [JsonPropertyName("bednum")]
        public string 床號 { get; set; }
        [JsonPropertyName("lqnty")]
        public string 數量 { get; set; }
        [JsonPropertyName("dosage")]
        public string 劑量 { get; set; }
        [JsonPropertyName("dispens_status")]
        public string 調劑狀態 { get; set; }
        [JsonPropertyName("check_status")]
        public string 覆核狀態 { get; set; }
        [JsonPropertyName("freqn")]
        public string 頻次 { get; set; }
        [JsonPropertyName("large")]
        public string 大瓶點滴 { get; set; }
        [JsonPropertyName("self")]
        public string 自費 { get; set; }
        [JsonPropertyName("selfPRN")]
        public string 自費PRN { get; set; }
        [JsonPropertyName("update_time")]
        public string 更新時間 { get; set; }

        public class ICP_By_bedNum : IComparer<bedListClass>
        {
            public int Compare(bedListClass x, bedListClass y)
            {
                return (x.床號.StringToInt32()).CompareTo(y.床號.StringToInt32());
            }
        }
    }
    public class dispensClass
    {
        [JsonPropertyName("code")]
        public string 藥碼 { get; set; }
        [JsonPropertyName("server_name")]
        public string ServerName { get; set; }
        [JsonPropertyName("server_type")]
        public string ServerType { get; set; }
    }

}