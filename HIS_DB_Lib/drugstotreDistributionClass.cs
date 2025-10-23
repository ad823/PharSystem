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
using H_Pannel_lib;
using System.Text.Json;
namespace HIS_DB_Lib
{
    public enum enum_撥補單上傳_Excel
    {
        藥碼,
        藥名,
        撥發量,
        效期,
        批號
    }
    public class distribution_excel
    {
        public string 藥碼 { get; set; }
        public string 藥名 { get; set; }
        public string 撥發量 { get; set; }
        public string 效期 { get; set; }
        public string 批號 { get; set; }
    }
    /// <summary>
    /// 枚舉表示藥品撥補的字段和屬性描述。
    /// </summary>
    [EnumDescription("drug_stotre_Distribution")]
    public enum enum_drugStotreDistribution
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("來源庫別,VARCHAR,20,NONE")]
        來源庫別,
        [Description("目的庫別,VARCHAR,20,NONE")]
        目的庫別,
        [Description("藥碼,VARCHAR,20,INDEX")]
        藥碼,
        [Description("藥名,VARCHAR,300,NONE")]
        藥名,
        [Description("批號,VARCHAR,50,NONE")]
        批號,
        [Description("效期,VARCHAR,20,NONE")]
        效期,
        [Description("包裝單位,VARCHAR,50,NONE")]
        包裝單位,
        [Description("包裝量,VARCHAR,300,NONE")]
        包裝量,
        [Description("來源庫庫存,VARCHAR,20,NONE")]
        來源庫庫存,
        [Description("目的庫庫存,VARCHAR,20,NONE")]
        目的庫庫存,
        [Description("撥發量,VARCHAR,20,NONE")]
        撥發量,
        [Description("實撥量,VARCHAR,20,NONE")]
        實撥量,
        [Description("簽收量,VARCHAR,20,NONE")]
        簽收量,
        [Description("來源庫結存,VARCHAR,20,NONE")]
        來源庫結存,
        [Description("目的庫結存,VARCHAR,20,NONE")]
        目的庫結存,
        [Description("撥發人員,VARCHAR,50,NONE")]
        撥發人員,
        [Description("撥發單位,VARCHAR,100,NONE")]
        撥發單位,
        [Description("加入時間,DATETIME,20,INDEX")]
        加入時間,
        [Description("撥發時間,DATETIME,20,INDEX")]
        撥發時間,
        [Description("簽收人員,VARCHAR,50,NONE")]
        簽收人員,
        [Description("簽收時間,DATETIME,20,INDEX")]
        簽收時間,
        [Description("報表名稱,VARCHAR,100,NONE")]
        報表名稱,
        [Description("報表生成時間,DATETIME,20,INDEX")]
        報表生成時間,
        [Description("狀態,VARCHAR,50,INDEX")]
        狀態,
        [Description("撥發細節,VARCHAR,500,NONE")]
        撥發細節,
        [Description("實撥細節,VARCHAR,500,NONE")]
        實撥細節,
        [Description("備註,VARCHAR,300,NONE")]
        備註,
        [Description("通知註記,VARCHAR,10,NONE")]
        通知註記,

    }
    /// <summary>
    /// 表示藥品撥補的數據類別。
    /// </summary>
    public class drugStotreDistributionClass
    {
        /// <summary>
        /// 獨特標識符。
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// 來源庫別。
        /// </summary>
        [JsonPropertyName("sourceStoreType")]
        public string 來源庫別 { get; set; }
        /// <summary>
        /// 目的庫別。
        /// </summary>
        [JsonPropertyName("destinationStoreType")]
        public string 目的庫別 { get; set; }
        /// <summary>
        /// 藥品代碼。
        /// </summary>
        [JsonPropertyName("code")]
        public string 藥碼 { get; set; }
        /// <summary>
        /// 藥品名稱。
        /// </summary>
        [JsonPropertyName("name")]
        public string 藥名 { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        [JsonPropertyName("LOT")]
        public string 批號 { get; set; }
        /// <summary>
        /// 效期
        /// </summary>
        [JsonPropertyName("VAL")]
        public string 效期 { get; set; }
        /// <summary>
        /// 包裝單位。
        /// </summary>
        [JsonPropertyName("packageUnit")]
        public string 包裝單位 { get; set; }
        /// <summary>
        /// 包裝量。
        /// </summary>
        [JsonPropertyName("packageQuantity")]
        public string 包裝量 { get; set; }
        /// <summary>
        /// 來源庫庫存。
        /// </summary>
        [JsonPropertyName("sourceStoreInventory")]
        public string 來源庫庫存 { get; set; }
        /// <summary>
        /// 目的庫庫存。
        /// </summary>
        [JsonPropertyName("destinationStoreInventory")]
        public string 目的庫庫存 { get; set; }
        /// <summary>
        /// 撥發量。
        /// </summary>
        [JsonPropertyName("issuedQuantity")]
        public string 撥發量 { get; set; }
        /// <summary>
        /// 實際撥發量。
        /// </summary>
        [JsonPropertyName("actualIssuedQuantity")]
        public string 實撥量 { get; set; }
        /// <summary>
        /// 簽收量。
        /// </summary>
        [JsonPropertyName("signedIssuedQuantity")]
        public string 簽收量 { get; set; }
        /// <summary>
        /// 來源庫結存。
        /// </summary>
        [JsonPropertyName("sourceStoreBalance")]
        public string 來源庫結存 { get; set; }
        /// <summary>
        /// 目的庫結存。
        /// </summary>
        [JsonPropertyName("destinationStoreBalance")]
        public string 目的庫結存 { get; set; }
        /// <summary>
        /// 撥發人員。
        /// </summary>
        [JsonPropertyName("issuer")]
        public string 撥發人員 { get; set; }
        /// <summary>
        /// 撥發單位。
        /// </summary>
        [JsonPropertyName("issuingUnit")]
        public string 撥發單位 { get; set; }
        /// <summary>
        /// 加入時間。
        /// </summary>
        [JsonPropertyName("addedTime")]
        public DateTime 加入時間 { get; set; }
        /// <summary>
        /// 撥發時間。
        /// </summary>
        [JsonPropertyName("issuanceTime")]
        public DateTime 撥發時間 { get; set; }
        /// <summary>
        /// 簽收人員。
        /// </summary>
        [JsonPropertyName("signedPerson")]
        public string 簽收人員 { get; set; }
        /// <summary>
        /// 簽收時間。
        /// </summary>
        [JsonPropertyName("signedTime")]
        public DateTime 簽收時間 { get; set; }
        /// <summary>
        /// 報表名稱。
        /// </summary>
        [JsonPropertyName("reportName")]
        public string 報表名稱 { get; set; }
        /// <summary>
        /// 報表生成時間。
        /// </summary>
        [JsonPropertyName("reportGenerationTime")]
        public DateTime 報表生成時間 { get; set; }
        /// <summary>
        /// 狀態。
        /// </summary>
        [JsonPropertyName("state")]
        public string 狀態 { get; set; }
        /// <summary>
        /// 撥發細節
        /// </summary>
        [JsonPropertyName("issuedStocks_text")]
        public string 撥發細節 { get; set; }
        /// <summary>
        /// 撥發細節
        /// </summary>
        [JsonPropertyName("actualIssuedStocks_text")]
        public string 實撥細節 { get; set; }
        /// <summary>
        /// 備註。
        /// </summary>
        [JsonPropertyName("remarks")]
        public string 備註 { get; set; }
        /// <summary>
        /// 通知註記。
        /// </summary>
        [JsonPropertyName("notice")]
        public string 通知註記 { get; set; }
        [JsonIgnore]
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            // PropertyNameCaseInsensitive = true,
        };
        /// <summary>
        /// 撥發細節
        /// </summary>
        [JsonPropertyName("issuedStocks")]
        public List<StockClass> issuedStocks
        {
            get
            {
                if (撥發細節.StringIsEmpty()) return new List<StockClass>(); ;
                List<StockClass> temp = JsonSerializer.Deserialize<List<StockClass>>(撥發細節, jsonSerializerOptions);

                return temp;
            }
            set
            {
                撥發細節 = JsonSerializer.Serialize(value, jsonSerializerOptions);
            }
        }
        /// <summary>
        /// 實撥細節
        /// </summary>
        [JsonPropertyName("actualIssuedStocks")]
        public List<StockClass> actualIssuedStocks
        {
            get
            {
                if (實撥細節.StringIsEmpty()) return new List<StockClass>(); ;
                List<StockClass> temp = JsonSerializer.Deserialize<List<StockClass>>(實撥細節, jsonSerializerOptions);

                return temp;
            }
            set
            {
                實撥細節 = JsonSerializer.Serialize(value, jsonSerializerOptions);
            }
        }
        
        
        
        
        
        static public SQLUI.Table init(string API_Server)
        {
            string url = $"{API_Server}/api/drugStotreDistribution/init";

            returnData returnData = new returnData();
            returnData.ServerName = "Main";
            returnData.ServerType = "網頁";

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
       
            List<SQLUI.Table> tables = json_out.JsonDeserializet<List<SQLUI.Table>>();
            return tables.GetTable(new enum_drugStotreDistribution());

        }
        static public void add(string API_Server, List<drugStotreDistributionClass> drugStotreDistributionClasses)
        {
            string url = $"{API_Server}/api/drugStotreDistribution/add";
            string str_serverNames = "";
            string str_serverTypes = "";

            returnData returnData = new returnData();
            returnData.ServerName = "";
            returnData.ServerType = "";
            returnData.Data = drugStotreDistributionClasses;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return;
            }
            if (returnData_out.Data == null)
            {
                return;
            }
            if (returnData_out.Code != 200) return;

            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");
            return;
        }
        static public returnData update_by_guid(string API_Server, List<drugStotreDistributionClass> drugStotreDistributionClasses)
        {
            string url = $"{API_Server}/api/drugStotreDistribution/update_by_guid";
            string str_serverNames = "";
            string str_serverTypes = "";

            returnData returnData = new returnData();
            returnData.ServerName = "";
            returnData.ServerType = "";
            returnData.Data = drugStotreDistributionClasses;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            return returnData_out;
            
        }
        static public List<drugStotreDistributionClass> get_by_addedTime(string API_Server, DateTime st_datetime , DateTime end_datetime)
        {
            List<drugStotreDistributionClass> drugStotreDistributionClasses = new List<drugStotreDistributionClass>();
            string url = $"{API_Server}/api/drugStotreDistribution/get_by_addedTime";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(st_datetime.ToDateTimeString());
            returnData.ValueAry.Add(end_datetime.ToDateTimeString());

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200) return null;
            drugStotreDistributionClasses = returnData.Data.ObjToClass<List<drugStotreDistributionClass>>();
            Console.WriteLine($"[{returnData.Method}]:{returnData.Result}");
            return drugStotreDistributionClasses;
        }
        static public returnData update_by_guid(string API_Server, string GUID)
        {
            string url = $"{API_Server}/api/drugStotreDistribution/get_by_guid";
            string str_serverNames = "";
            string str_serverTypes = "";

            returnData returnData = new returnData();
            returnData.ServerName = "";
            returnData.ServerType = "";
            returnData.ValueAry.Add(GUID);
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            return returnData_out;
            //if (returnData_out == null)
            //{
            //    return;
            //}
            //if (returnData_out.Data == null)
            //{
            //    return;
            //}
            //if (returnData_out.Code != 200) return;

        }
        static public byte[] download_excel_by_addedTime(string API_Server, DateTime st_datetime, DateTime end_datetime)
        {
            string url = $"{API_Server}/api/drugStotreDistribution/download_excel_by_addedTime";
            returnData returnData = new returnData();
            returnData.ValueAry.Add(st_datetime.ToDateTimeString());
            returnData.ValueAry.Add(end_datetime.ToDateTimeString());
            string json_in = returnData.JsonSerializationt();
            byte[] bytes = Basic.Net.WEBApiPostDownloaFile(url, json_in);
            return bytes;
        }


        public class ICP_By_Code : IComparer<drugStotreDistributionClass>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(drugStotreDistributionClass x, drugStotreDistributionClass y)
            {
                return x.藥碼.CompareTo(y.藥碼);
            }
        }
        public class ICP_By_addedTime : IComparer<drugStotreDistributionClass>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(drugStotreDistributionClass x, drugStotreDistributionClass y)
            {
                return x.加入時間.CompareTo(y.加入時間);
            }
        }
    }
}
