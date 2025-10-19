using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.ComponentModel;
using System.Reflection;


namespace HIS_DB_Lib
{
    [EnumDescription("med_group")]
    public enum enum_medGroup
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("名稱,VARCHAR,200,NONE")]
        名稱,
        [Description("顯示資訊,VARCHAR,500,NONE")]
        顯示資訊,
        [Description("建立時間,DATETIME,200,NONE")]
        建立時間,
        [Description("住院藥車,VARCHAR,20,NONE")]
        住院藥車,
    }
    [EnumDescription("med_sub_group")]
    public enum enum_sub_medGroup
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,200,INDEX")]
        Master_GUID,
        [Description("排列號,VARCHAR,10,None")]
        排列號,
        [Description("藥品碼,VARCHAR,20,INDEX")]
        藥品碼,
    }
    public static class medGroupClassMethod
    {
        /// <summary>
        /// 將 medGroupClass 內的 MedClasses 依「排列號」數值排序（由小到大）
        /// </summary>
        /// <param name="medGroup">目標群組</param>
        /// <returns>排序後的 medGroupClass（原物件已更新）</returns>
        public static medGroupClass SortByOrderNumber(this medGroupClass medGroup)
        {
            if (medGroup == null) return null;
            if (medGroup.MedClasses == null || medGroup.MedClasses.Count == 0) return medGroup;

            medGroup.MedClasses = medGroup.MedClasses
                .OrderBy(e =>
                {
                    if (e == null) return int.MaxValue;
                // 將排列號字串轉成數字排序
                if (int.TryParse(e.排列號, out int orderNum))
                        return orderNum;
                    return int.MaxValue;
                })
                .ToList();

            return medGroup;
        }
    }
    public class medGroupClass
    {
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        [JsonPropertyName("NAME")]
        public string 名稱 { get; set; }
        [JsonPropertyName("VIS_INFO")]
        public string 顯示資訊 { get; set; } = "";
        [JsonPropertyName("CT_TIME")]
        public string 建立時間 { get; set; }
        [JsonPropertyName("UD")]
        public string 住院藥車 { get; set; }

        private List<medClass> medClasses = new List<medClass>();
        public List<medClass> MedClasses { get => medClasses; set => medClasses = value; }

        /// <summary>
        /// 初始化藥品群組資料表
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <returns>藥品群組資料表列表</returns>
        static public List<SQLUI.Table> init(string API_Server)
        {
            string url = $"{API_Server}/api/medGroup/init";

            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            List<SQLUI.Table> tables = json_out.JsonDeserializet<List<SQLUI.Table>>();
            return tables;
        }


        /// <summary>
        /// 呼叫 API 取得藥品群組上層（僅群組資訊，不含 MedClasses，可依顯示資訊過濾）
        /// </summary>
        /// <param name="API_Server">API 伺服器</param>
        /// <param name="filterServerName">要比對的顯示資訊（可為空）</param>
        /// <returns>過濾後的藥品群組清單</returns>
        static public List<medGroupClass> get_all_group_header(string API_Server, string filterServerName = "")
        {
            var (code, result, list) = get_all_group_header_full(API_Server);
            if (list == null) return new List<medGroupClass>();

            if (!filterServerName.StringIsEmpty())
            {
                list = list
                    .Where(x => x.顯示資訊.StringIsEmpty() || x.顯示資訊.Contains(filterServerName))
                    .ToList();
            }

            return list;
        }
        /// <summary>
        /// 呼叫 API 取得藥品群組上層（完整版本，回傳 code、result、list）
        /// </summary>
        static public (int code, string result, List<medGroupClass>) get_all_group_header_full(string API_Server)
        {
            string url = $"{API_Server}/api/medGroup/get_all_group_header";

            returnData returnData = new returnData();

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
                return (0, "returnData_out == null", null);

            if (returnData_out.Data == null)
                return (0, "returnData_out.Data == null", null);

            List<medGroupClass> medGroupClasses = returnData_out.Data.ObjToClass<List<medGroupClass>>();
            return (returnData_out.Code, returnData_out.Result, medGroupClasses);
        }
        /// <summary>
        /// 獲取所有藥品群組名稱列表
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <returns>藥品群組名稱列表</returns>
        /// <summary>
        /// 獲取所有藥品群組名稱列表（僅上層，根據 filterServerName 過濾顯示資訊）
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="filterServerName">用於比對 medGroupClass.顯示資訊（可為空）</param>
        /// <returns>藥品群組名稱列表</returns>
        static public List<string> get_medGroupList_header(string API_Server, string filterServerName = "")
        {
            List<medGroupClass> medGroupClasses = get_all_group_header(API_Server);

            if (medGroupClasses == null) return new List<string>();

            List<string> names = medGroupClasses
                .Where(x =>
                    !x.名稱.StringIsEmpty() &&
                    (x.顯示資訊.StringIsEmpty() || x.顯示資訊.Contains(filterServerName))
                )
                .Select(x => x.名稱)
                .Distinct()
                .ToList();

            return names;
        }

        static public medGroupClass get_group_by_name(string API_Server, string groupName)
        {
            var (code, result, group) = get_group_by_name_full(API_Server, groupName);
            return group;
        }
        

        static public (int code, string result, medGroupClass group) get_group_by_name_full(string API_Server, string groupName)
        {
            string url = $"{API_Server}/api/medGroup/get_group_by_name";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(groupName);

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {
                return (0, "returnData_out == null", null);
            }
            if (returnData_out.Data == null)
            {
                return (0, "returnData_out.Data == null", null);
            }

            // ✅ 將回傳的資料轉型成 medGroupClass（單一筆）
            medGroupClass medGroup = returnData_out.Data.ObjToClass<medGroupClass>();
            medGroup = medGroup.SortByOrderNumber();
            return (returnData_out.Code, returnData_out.Result, medGroup);
        }


        /// <summary>
        /// 獲取所有藥品群組（可指定過濾 ServerName）
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="filterServerName">要過濾的伺服器名稱，若為 null 則不過濾</param>
        /// <returns>藥品群組列表</returns>
        static public List<medGroupClass> get_all_group(string API_Server, string filterServerName = null)
        {
            string url = $"{API_Server}/api/medGroup/get_all_group";
            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null || returnData_out.Code != 200) return null;

            List<medGroupClass> medGroupClasses = returnData_out.Data.ObjToClass<List<medGroupClass>>();

            // ✅ 加入 ServerName 過濾條件（可選）
            if (!filterServerName.StringIsEmpty())
            {
                medGroupClasses = medGroupClasses
                    .Where(x => x.顯示資訊.StringIsEmpty() || x.顯示資訊.Contains(filterServerName))
                    .ToList();
            }

            return medGroupClasses;
        }


        /// <summary>
        /// 獲取所有藥品群組名稱
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <returns>藥品群組名稱列表</returns>
        static public List<string> get_all_group_name(string API_Server)
        {

            List<medGroupClass> medGroupClasses = get_all_group(API_Server);
            List<string> vs = (from temp in medGroupClasses
                               select temp.名稱).Distinct().ToList();
            return vs;
        }
        static public async Task<List<medGroupClass>> get_group_name(string API_Server)
        {
            string url = $"{API_Server}/api/medGroup/get_group_name";
            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            Task<string> result = Net.WEBApiPostJsonAsync(url, json_in, false);
            string json_out = await result;
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200) return null;
            List<medGroupClass> medGroupClasses = returnData.Data.ObjToClass<List<medGroupClass>>();
            return medGroupClasses;
        }

        /// <summary>
        /// 新增藥品群組
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="medGroupClass">藥品群組</param>
        static public void add_group(string API_Server, medGroupClass medGroupClass)
        {
            string url = $"{API_Server}/api/medGroup/add_group";
            returnData returnData = new returnData();
            returnData.Data = medGroupClass;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return;
            if (returnData_out.Code != 200) return;
            Console.WriteLine($"{returnData}");
        }
        /// <summary>
        /// 新增藥品群組
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="medGroupClass">藥品群組</param>
        static public void add_sub_group(string API_Server, List<medClass> medClass)
        {
            string url = $"{API_Server}/api/medGroup/add_sub_group";
            returnData returnData = new returnData();
            returnData.Data = medClass;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return;
            if (returnData_out.Code != 200) return;
        }
        /// <summary>
        /// 取得藥品群組List<string>
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="medGroupClass">藥品群組</param>
        static public List<string> get_sub_group_by_Master_GUID(string API_Server, string master_guid)
        {
            string url = $"{API_Server}/api/medGroup/get_sub_group_by_Master_GUID";
            returnData returnData = new returnData();
            returnData.ValueAry.Add(master_guid);
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return null;
            if (returnData_out.Code != 200) return null;
            return returnData_out.Data.ObjToClass<List<string>>();

        }
        /// <summary>
        /// 根據GUID刪除藥品群組
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="medGroupClass">藥品群組</param>
        static public void delete_group_by_guid(string API_Server, medGroupClass medGroupClass)
        {
            delete_group_by_guid(API_Server, medGroupClass.GUID);
        }

        /// <summary>
        /// 根據GUID刪除藥品群組
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="GUID">藥品群組GUID</param>
        static public void delete_group_by_guid(string API_Server, string GUID)
        {
            string url = $"{API_Server}/api/medGroup/delete_group_by_guid";
            returnData returnData = new returnData();
            returnData.Value = GUID;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return;
            if (returnData_out.Code != 200) return;
            Console.WriteLine($"{returnData}");
        }

        /// <summary>
        /// 刪除藥品群組中的藥品
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="GUID">藥品群組GUID</param>
        /// <param name="medClasses">藥品列表</param>
        static public void delete_meds_in_group(string API_Server, string GUID, List<medClass> medClasses)
        {
            medGroupClass medGroupClass = new medGroupClass();
            medGroupClass.GUID = GUID;
            medGroupClass.medClasses = medClasses;
            delete_meds_in_group(API_Server, medGroupClass);
        }

        /// <summary>
        /// 刪除藥品群組中的藥品
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="medGroupClass">藥品群組</param>
        static public void delete_meds_in_group(string API_Server, medGroupClass medGroupClass)
        {
            string url = $"{API_Server}/api/medGroup/delete_meds_in_group";
            returnData returnData = new returnData();
            returnData.Data = medGroupClass;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return;
            if (returnData_out.Code != 200) return;
            Console.WriteLine($"{returnData}");
        }

        /// <summary>
        /// 新增藥品到藥品群組中
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="medGroupClass">藥品群組</param>
        static public void add_meds_in_group(string API_Server, medGroupClass medGroupClass)
        {
            string url = $"{API_Server}/api/medGroup/add_meds_in_group";
            returnData returnData = new returnData();
            returnData.Data = medGroupClass;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return;
            if (returnData_out.Code != 200) return;
            Console.WriteLine($"{returnData}");
        }
        /// <summary>
        /// 修改指定藥品群組顯示資訊
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="GUID">藥品群組GUID</param>
        /// <param name="visibleDevices">顯示設備列表</param>
        /// <returns>更新結果，包含狀態碼和結果訊息</returns>
        static public (int code, string result) update_visible_info(string API_Server, string GUID, List<string> visibleDevices)
        {
            string url = $"{API_Server}/api/medGroup/visible_set_by_guid";
            returnData returnData = new returnData();
            returnData.ValueAry = new List<string> { GUID };
            returnData.ValueAry.AddRange(visibleDevices);
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return (-200, "更新失敗");
            return (returnData_out.Code, returnData_out.Result);
        }
        /// <summary>
        /// 根據GUID獲取藥品群組
        /// </summary>
        /// <param name="API_Server">API伺服器地址</param>
        /// <param name="GUID">藥品群組GUID</param>
        /// <returns>藥品群組</returns>
        static public (int code, string result, medGroupClass medGroupClass) get_group_by_guid(string API_Server, string GUID)
        {
            string url = $"{API_Server}/api/medGroup/get_group_by_guid";
            returnData returnData = new returnData();
            returnData.ValueAry = new List<string> { GUID };
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return (-200, "取得失敗", null);
            if (returnData_out.Code != 200) return (returnData_out.Code, returnData_out.Result, null);
            List<medGroupClass> medGroupClasses = returnData_out.Data.ObjToClass<List<medGroupClass>>();
            if (medGroupClasses.Count == 0) return (returnData_out.Code, returnData_out.Result, null);
            return (returnData_out.Code, returnData_out.Result, medGroupClasses[0]); 
        }

        
    }
  
}
