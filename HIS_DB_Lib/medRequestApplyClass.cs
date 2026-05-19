using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.ComponentModel;
using System.Reflection;
using SQLUI;

namespace HIS_DB_Lib
{
    /// <summary>
    /// 申請換領報表資料
    /// </summary>
    [EnumDescription("medRequestApply")]
    public enum enum_申請換領報表資料
    {
        [Description("GUID,VARCHAR,40,PRIMARY")]
        訂單編號,

        [Description("OrderlistGuid,VARCHAR,40,INDEX")]
        orderlist編號,

        [Description("RequestNo,VARCHAR,20,NONE")]
        換領單編號,

        [Description("PrescribingDoctorNarcoticLicenseNo,VARCHAR,40,NONE")]
        處方醫師麻管證號,

        [Description("DrugReceiver,VARCHAR,40,NONE")]
        領藥人,

        [Description("DrugAdministrator,VARCHAR,40,NONE")]
        施打者,

        [Description("DrugDestroyer,VARCHAR,40,NONE")]
        銷毀人,

        [Description("Witness,VARCHAR,40,NONE")]
        見證人,

        [Description("CheckingPharmacist,VARCHAR,40,NONE")]
        核對藥師,

        [Description("HandoverSignature,VARCHAR,40,NONE")]
        交班簽名,

        [Description("CreatBy,VARCHAR,40,NONE")]
        建立人,

        [Description("UDOGIVDOSE,VARCHAR,10,NONE")]
        劑量使用單位,

        [Description("UpdateTime,DATETIME,20,NONE")]
        最後修改時間,

        [Description("CreatAt,DATETIME,20,NONE")]
        建立時間,
    }

    /// <summary>
    /// 申請換領報表資料
    /// </summary>
    public class medRequestApply
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        [JsonPropertyName("GUID")]
        public string 訂單編號 { get; set; }

        /// <summary>
        /// orderlist編號
        /// </summary>
        [JsonPropertyName("OrderlistGUID")]
        public string orderlist編號 { get; set; }

        /// <summary>
        /// 換領單編號
        /// </summary>
        [JsonPropertyName("RequestNo")]
        public string 換領單編號 { get; set; }

        /// <summary>
        /// 處方醫師麻管證號
        /// </summary>
        [JsonPropertyName("PrescribingDoctorNarcoticLicenseNo")]
        public string 處方醫師麻管證號 { get; set; }

        /// <summary>
        /// 領藥人
        /// </summary>
        [JsonPropertyName("DrugReceiver")]
        public string 領藥人 { get; set; }

        /// <summary>
        /// 施打者
        /// </summary>
        [JsonPropertyName("DrugAdministrator")]
        public string 施打者 { get; set; }

        /// <summary>
        /// 銷毀人
        /// </summary>
        [JsonPropertyName("DrugDestroyer")]
        public string 銷毀人 { get; set; }

        /// <summary>
        /// 見證人
        /// </summary>
        [JsonPropertyName("Witness")]
        public string 見證人 { get; set; }

        /// <summary>
        /// 核對藥師
        /// </summary>
        [JsonPropertyName("CheckingPharmacist")]
        public string 核對藥師 { get; set; }

        /// <summary>
        /// 交班簽名
        /// </summary>
        [JsonPropertyName("HandoverSignature")]
        public string 交班簽名 { get; set; }

        /// <summary>
        /// 建立人
        /// </summary>
        [JsonPropertyName("CreatBy")]
        public string 建立人 { get; set; }

        /// <summary>
        /// 劑量使用單位
        /// </summary>
        [JsonPropertyName("UDOGIVDOSE")]
        public string 劑量使用單位 { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        [JsonPropertyName("UpdateTime")]
        public string 最後修改時間 { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonPropertyName("CreatAt")]
        public string 建立時間 { get; set; }

        /// <summary>
        /// 處方醫師姓名
        /// </summary>
        [JsonPropertyName("PrescribingDoctorNarcoticLicenseName")]
        public string 處方醫師姓名 { get; set; }

        /// <summary>
        /// 領藥人姓名
        /// </summary>
        [JsonPropertyName("DrugReceiverName")]
        public string 領藥人姓名 { get; set; }

        /// <summary>
        /// 施打者姓名
        /// </summary>
        [JsonPropertyName("DrugAdministratorName")]
        public string 施打者姓名 { get; set; }

        /// <summary>
        /// 銷毀人名稱
        /// </summary>
        [JsonPropertyName("DrugDestroyerName")]
        public string 銷毀人姓名 { get; set; }

        /// <summary>
        /// 見證人名稱
        /// </summary>
        [JsonPropertyName("WitnessName")]
        public string 見證人姓名 { get; set; }

        /// <summary>
        /// 核對藥師名稱
        /// </summary>
        [JsonPropertyName("CheckingPharmacistName")]
        public string 核對藥師姓名 { get; set; }

        /// <summary>
        /// 交班簽名人名稱
        /// </summary>
        [JsonPropertyName("HandoverSignatureName")]
        public string 交班簽名人姓名 { get; set; }

        static public SQLUI.Table init(string API_Server)
        {
            string url = $"{API_Server}/api/medRequestApply/init";

            returnData returnData = new returnData();
            string tableName = "";

            returnData.TableName = tableName;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            SQLUI.Table table = json_out.JsonDeserializet<SQLUI.Table>();
            return table;
        }

        static public SQLUI.Table init(string API_Server, string ServerName, string ServerType)
        {
            string url = $"{API_Server}/api/medRequestApply/init";

            returnData returnData = new returnData();
            string tableName = "";

            returnData.TableName = tableName;
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            SQLUI.Table table = json_out.JsonDeserializet<SQLUI.Table>();
            return table;
        }

        static public List<medRequestApply> add_batch(string API_Server, List<medRequestApply> applications)
        {
            string url = $"{API_Server}/api/medRequestApply/add_medrequestapply";

            returnData returnData = new returnData();
            returnData.Data = applications;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {
                return null;
            }
            if (returnData_out.Data == null)
            {
                return null;
            }
            Console.WriteLine($"{returnData_out}");
            List<medRequestApply> applications_out = returnData_out.Data.ObjToClass<List<medRequestApply>>();
            return applications_out;
        }

        static public medRequestApply get_by_guid(string API_Server, string guid)
        {
            string url = $"{API_Server}/api/medRequestApply/get_by_guid";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(guid);

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {
                return null;
            }
            if (returnData_out.Data == null)
            {
                return null;
            }
            Console.WriteLine($"{returnData_out}");
            medRequestApply application = returnData_out.Data.ObjToClass<medRequestApply>();
            return application;
        }

        static public List<medRequestApply> get_by_request_no(string API_Server, string requestNo)
        {
            string url = $"{API_Server}/api/medRequestApply/get_by_request_no";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(requestNo);

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {
                return null;
            }
            if (returnData_out.Data == null)
            {
                return null;
            }
            Console.WriteLine($"{returnData_out}");
            List<medRequestApply> applications = returnData_out.Data.ObjToClass<List<medRequestApply>>();
            return applications;
        }

        static public void update_by_guid(string API_Server, medRequestApply application)
        {
            List<medRequestApply> applications = new List<medRequestApply>();
            applications.Add(application);
            update_by_guid(API_Server, applications);
        }

        static public void update_by_guid(string API_Server, List<medRequestApply> applications)
        {
            string url = $"{API_Server}/api/medRequestApply/update_by_guid";

            returnData returnData = new returnData();
            returnData.Data = applications;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {

            }
            if (returnData_out.Data == null)
            {

            }
            Console.WriteLine($"{returnData_out}");
        }

        /// <summary>
        /// 部分更新申請換領報表（按GUID）- 支援欄位級別的部分更新
        /// </summary>
        /// <param name="API_Server">API 伺服器地址</param>
        /// <param name="guid">申請單GUID</param>
        /// <param name="updateFields">要更新的欄位字典，例如：new { DrugAdministrator = "名稱", Witness = "見證人" }</param>
        static public medRequestApply partial_update_by_guid(string API_Server, string guid, object updateFields)
        {
            string url = $"{API_Server}/api/medRequestApply/partial_update_by_guid";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(guid);
            returnData.Data = updateFields;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null || returnData_out.Data == null)
            {
                return null;
            }

            medRequestApply application = returnData_out.Data.ObjToClass<medRequestApply>();
            return application;
        }

    }
}