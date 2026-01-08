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
    public enum enum_取藥堆疊母資料_狀態
    {
        None,
        雙人覆核,
        覆核完成,
        庫存不足,
        無儲位,
        等待盲盤,
        盲盤完成,
        等待複盤,
        複盤完成,
        輸入新效期,
        選擇效期,
        新增效期,
        等待刷新,
        等待作業,
        作業完成,
        等待入賬,
        入賬完成,
        取消作業,
        刪除資料,
        已領用過,
        新增資料,
        DC處方,
        NEW處方,
        RFID使用,
        自動出藥,
        無可匹配數量,
        未授權,
    }
    public enum enum_取藥堆疊母資料_作業模式
    {
        效期管控,
        複盤,
        盲盤,
        雙人覆核,
        獨立作業,
        庫存不足語音提示,
        RFID使用,
        自動出藥,
    }
    [EnumDescription("take_medicine_stack_new")]
    public enum enum_取藥堆疊母資料
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Order_GUID,VARCHAR,200,INDEX")]
        Order_GUID,
        [Description("序號,VARCHAR,200,None")]
        序號,
        [Description("調劑台名稱,VARCHAR,50,None")]
        調劑台名稱,
        [Description("IP,VARCHAR,20,None")]
        IP,
        [Description("操作人,VARCHAR,20,None")]
        操作人,
        [Description("ID,VARCHAR,20,None")]
        ID,
        [Description("藥師證字號,VARCHAR,20,None")]
        藥師證字號,
        [Description("覆核藥師姓名,VARCHAR,20,None")]
        覆核藥師姓名,
        [Description("覆核藥師ID,VARCHAR,20,None")]
        覆核藥師ID,
        [Description("動作,VARCHAR,20,None")]
        動作,
        [Description("作業模式,VARCHAR,20,None")]
        作業模式,
        [Description("藥袋序號,VARCHAR,200,None")]
        藥袋序號,
        [Description("領藥號,VARCHAR,20,None")]
        領藥號,
        [Description("病房號,VARCHAR,20,None")]
        病房號,
        [Description("類別,VARCHAR,15,None")]
        類別,
        [Description("藥品碼,VARCHAR,15,None")]
        藥品碼,
        [Description("藥品名稱,VARCHAR,300,None")]
        藥品名稱,
        [Description("單位,VARCHAR,20,None")]
        單位,
        [Description("病歷號,VARCHAR,20,None")]
        病歷號,
        [Description("病人姓名,VARCHAR,50,None")]
        病人姓名,
        [Description("床號,VARCHAR,20,None")]
        床號,
        [Description("頻次,VARCHAR,20,None")]
        頻次,
        [Description("開方時間,VARCHAR,50,None")]
        開方時間,
        [Description("操作時間,VARCHAR,50,None")]
        操作時間,
        [Description("顏色,VARCHAR,20,None")]
        顏色,
        [Description("狀態,VARCHAR,20,None")]
        狀態,
        [Description("庫存量,VARCHAR,10,None")]
        庫存量,
        [Description("總異動量,VARCHAR,10,None")]
        總異動量,
        [Description("結存量,VARCHAR,10,None")]
        結存量,
        [Description("盤點量,VARCHAR,10,None")]
        盤點量,
        [Description("效期,VARCHAR,50,None")]
        效期,
        [Description("批號,VARCHAR,50,None")]
        批號,
        [Description("備註,VARCHAR,200,None")]
        備註,
        [Description("收支原因,VARCHAR,200,None")]
        收支原因,
        [Description("診別,VARCHAR,20,None")]
        診別,
        [Description("儲位描述,VARCHAR,200,None")]
        儲位描述

    }
    [EnumDescription("take_medicine_substack_new")]
    public enum enum_取藥堆疊子資料
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,None")]
        Master_GUID,
        [Description("Device_GUID,VARCHAR,50,None")]
        Device_GUID,
        [Description("序號,VARCHAR,200,None")]
        序號,
        [Description("調劑台名稱,VARCHAR,50,None")]
        調劑台名稱,
        [Description("藥品碼,VARCHAR,50,None")]
        藥品碼,
        [Description("IP,VARCHAR,50,None")]
        IP,
        [Description("Num,VARCHAR,10,None")]
        Num,
        [Description("TYPE,VARCHAR,50,None")]
        TYPE,
        [Description("Check_IP,VARCHAR,50,None")]
        Check_IP,
        [Description("效期,VARCHAR,50,None")]
        效期,
        [Description("批號,VARCHAR,50,None")]
        批號,
        [Description("異動量,VARCHAR,20,None")]
        異動量,
        [Description("致能,VARCHAR,10,None")]
        致能,
        [Description("流程作業完成,VARCHAR,10,None")]
        流程作業完成,
        [Description("配藥完成,VARCHAR,10,None")]
        配藥完成,
        [Description("調劑結束,VARCHAR,10,None")]
        調劑結束,
        [Description("已入帳,VARCHAR,10,None")]
        已入帳,
        [Description("暫存參數,VARCHAR,20,None")]
        暫存參數,
    }
    public class class_OutTakeMed_data
    {
        [JsonPropertyName("PRI_KEY")]
        public string PRI_KEY { get; set; }
        /// <summary>
        /// Order_GUID
        /// </summary>
        [JsonPropertyName("Order_GUID")]
        public string Order_GUID { get; set; }
        [JsonPropertyName("MC_name")]
        public string 電腦名稱 { get; set; }
        [JsonPropertyName("cost_center")]
        public string 成本中心 { get; set; }
        [JsonPropertyName("src_storehouse")]
        public string 來源庫別 { get; set; }
        [JsonPropertyName("nursingstation")]
        public string 護理站 { get; set; }
        [JsonPropertyName("src_oper")]
        public string 加退藥來源 { get; set; }
        [JsonPropertyName("code")]
        public string 藥品碼 { get; set; }
        [JsonPropertyName("name")]
        public string 藥名 { get; set; }
        [JsonPropertyName("package")]
        public string 單位 { get; set; }
        [JsonPropertyName("MED_BAG_NUM")]
        public string 領藥號 { get; set; }
        [JsonPropertyName("WARD_NAME")]
        public string 病房號 { get; set; }
        [JsonPropertyName("OD_type")]
        public string 類別 { get; set; }
        [JsonPropertyName("bed_code")]
        public string 床號 { get; set; }
        [JsonPropertyName("value")]
        public string 交易量 { get; set; }
        [JsonPropertyName("operator")]
        public string 操作人 { get; set; }
        [JsonPropertyName("ID")]
        public string ID { get; set; }
        [JsonPropertyName("recheck_name")]
        public string 覆核藥師姓名 { get; set; }
        [JsonPropertyName("recheck_id")]
        public string 覆核藥師ID { get; set; }
        [JsonPropertyName("patient_name")]
        public string 病人姓名 { get; set; }
        [JsonPropertyName("patient_code")]
        public string 病歷號 { get; set; }
        [JsonPropertyName("prescription_time")]
        public string 開方時間 { get; set; }
        [JsonPropertyName("OP_type")]
        public string 功能類型 { get; set; }
        [JsonPropertyName("VAL")]
        public string 效期 { get; set; }
        [JsonPropertyName("LOT")]
        public string 批號 { get; set; }
        [JsonPropertyName("date")]
        public string 日期 { get; set; }
        [JsonPropertyName("time")]
        public string 時間 { get; set; }
        [JsonPropertyName("RSN")]
        public string 收支原因 { get; set; }
        [JsonPropertyName("color")]
        public string 顏色 { get; set; }

        static public returnData OutTakeMed(string API_Server, string ServerName, List<class_OutTakeMed_data> outTakeMed_Datas)
        {
            string url = $"{API_Server}/api/OutTakeMed/new";
            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.Data = outTakeMed_Datas;
            string json_in = returnData.JsonSerializationt();
            string json_out = Basic.Net.WEBApiPostJson(url, json_in);
            Logger.Log("OutTakeMed_dataInput", $"{json_in}");
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
    }

    [Serializable]
    /// <summary>
    /// 藥物取用堆疊類別
    /// </summary>
    public class takeMedicineStackClass
    {
        /// <summary>
        /// 全局唯一標識符
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// 訂單全局唯一標識符
        /// </summary>
        [JsonPropertyName("Order_GUID")]
        public string Order_GUID { get; set; }
        /// <summary>
        /// 序號
        /// </summary>
        [JsonPropertyName("serial_number")]
        public string 序號 { get; set; }
        /// <summary>
        /// 調劑台名稱
        /// </summary>
        [JsonPropertyName("dispensing_station_name")]
        public string 調劑台名稱 { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        [JsonPropertyName("ip_address")]
        public string IP { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        [JsonPropertyName("operator")]
        public string 操作人 { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        [JsonPropertyName("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 藥師證字號
        /// </summary>
        [JsonPropertyName("pharmacist_license_number")]
        public string 藥師證字號 { get; set; }
        /// <summary>
        /// 覆核藥師姓名
        /// </summary>
        [JsonPropertyName("recheck_name")]
        public string 覆核藥師姓名 { get; set; }
        /// <summary>
        /// 覆核藥師ID
        /// </summary>
        [JsonPropertyName("recheck_id")]
        public string 覆核藥師ID { get; set; }
        /// <summary>
        /// 交易記錄查詢動作
        /// </summary>
        [JsonPropertyName("action")]
        public string 動作 { get; set; }
        /// <summary>
        /// 作業模式
        /// </summary>
        [JsonPropertyName("operation_mode")]
        public string 作業模式 { get; set; }
        /// <summary>
        /// 藥袋序號
        /// </summary>
        [JsonPropertyName("medicine_bag_serial_number")]
        public string 藥袋序號 { get; set; }
        /// <summary>
        /// 領藥號
        /// </summary>
        [JsonPropertyName("medicine_bag_number")]
        public string 領藥號 { get; set; }
        /// <summary>
        /// 病房號
        /// </summary>
        [JsonPropertyName("ward_number")]
        public string 病房號 { get; set; }
        /// <summary>
        /// 類別
        /// </summary>
        [JsonPropertyName("category")]
        public string 類別 { get; set; }
        /// <summary>
        /// 藥品碼
        /// </summary>
        [JsonPropertyName("medicine_code")]
        public string 藥品碼 { get; set; }
        /// <summary>
        /// 藥品名稱
        /// </summary>
        [JsonPropertyName("medicine_name")]
        public string 藥品名稱 { get; set; }
        /// <summary>
        /// 單位
        /// </summary>
        [JsonPropertyName("unit")]
        public string 單位 { get; set; }
        /// <summary>
        /// 病歷號
        /// </summary>
        [JsonPropertyName("patient_record_number")]
        public string 病歷號 { get; set; }
        /// <summary>
        /// 病人姓名
        /// </summary>
        [JsonPropertyName("patient_name")]
        public string 病人姓名 { get; set; }
        /// <summary>
        /// 床號
        /// </summary>
        [JsonPropertyName("bed_number")]
        public string 床號 { get; set; }
        /// <summary>
        /// 頻次
        /// </summary>
        [JsonPropertyName("frequency")]
        public string 頻次 { get; set; }
        /// <summary>
        /// 開方時間
        /// </summary>
        [JsonPropertyName("prescription_time")]
        public string 開方時間 { get; set; }
        /// <summary>
        /// 操作時間
        /// </summary>
        [JsonPropertyName("operation_time")]
        public string 操作時間 { get; set; }
        /// <summary>
        /// 顏色
        /// </summary>
        [JsonPropertyName("color")]
        public string 顏色 { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        [JsonPropertyName("status")]
        public string 狀態 { get; set; }
        /// <summary>
        /// 庫存量
        /// </summary>
        [JsonPropertyName("inventory")]
        public string 庫存量 { get; set; }
        /// <summary>
        /// 總異動量
        /// </summary>
        [JsonPropertyName("total_change")]
        public string 總異動量 { get; set; }
        /// <summary>
        /// 結存量
        /// </summary>
        [JsonPropertyName("balance")]
        public string 結存量 { get; set; }
        /// <summary>
        /// 盤點量
        /// </summary>
        [JsonPropertyName("inventory_count")]
        public string 盤點量 { get; set; }
        /// <summary>
        /// 效期
        /// </summary>
        [JsonPropertyName("expiry_date")]
        public string 效期 { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        [JsonPropertyName("batch_number")]
        public string 批號 { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        [JsonPropertyName("remarks")]
        public string 備註 { get; set; }
        /// <summary>
        /// 收支原因
        /// </summary>
        [JsonPropertyName("reason")]
        public string 收支原因 { get; set; }
        /// <summary>
        /// 診別
        /// </summary>
        [JsonPropertyName("clinic_type")]
        public string 診別 { get; set; }
        /// <summary>
        /// 儲位描述
        /// </summary>
        [JsonPropertyName("storage_note")]
        public string 儲位描述 { get; set; }

        static public List<SQLUI.Table> init(string API_Server, string ServerName, string ServerType)
        {
            string url = $"{API_Server}/api/OutTakeMed/init";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            string tableName = "";
            returnData.TableName = tableName;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            List<SQLUI.Table> tables = json_out.JsonDeserializet<List<SQLUI.Table>>();
            return tables;
        }
        static public void set_device_tradding(string API_Server, string ServerName, string ServerType, List<takeMedicineStackClass> takeMedicineStackClasses)
        {
            string url = $"{API_Server}/api/OutTakeMed/set_device_tradding";

            returnData returnData = new returnData();
            returnData.ServerName = ServerName;
            returnData.ServerType = ServerType;
            returnData.Data = takeMedicineStackClasses;


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
            Console.WriteLine($"{returnData_out}");

        }

    }
    static public class takeMedicineStackClassMethod
    {
        static public List<takeMedicineStackClass> ToTakeMedicineStackClassList(this List<object[]> list_value)
        {
            List<takeMedicineStackClass> takeMedicineStackClasses = list_value.SQLToClass<takeMedicineStackClass, enum_取藥堆疊母資料>();

            return takeMedicineStackClasses;
        }
    }
}

 
