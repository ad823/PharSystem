using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json.Serialization;
using Basic;
using SQLUI;

namespace HIS_DB_Lib
{
    /// <summary>
    /// 系統參數資料表欄位定義。
    /// </summary>
    /// <remarks>
    /// 資料表名稱：system_config
    ///
    /// 用途：
    /// 1. 儲存一般系統參數。
    /// 2. 儲存 JSON 設定。
    /// 3. 儲存圖片 Base64。
    /// 4. 儲存檔案 Base64。
    /// 5. 儲存報表模板、列印模板、Logo、授權檔等資料。
    ///
    /// 唯一規則：
    /// 類別 + 鍵值 不可重複。
    ///
    /// 檔案限制：
    /// 單筆 Base64 原始檔案大小不可超過 10MB，由 API 端檢查。
    /// </remarks>
    [EnumDescription("system_config")]
    public enum enum_system_config
    {
        /// <summary>
        /// 唯一識別碼。
        /// </summary>
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,

        /// <summary>
        /// 參數類別，例如：系統設定、圖片設定、報表模板、列印設定。
        /// </summary>
        [Description("類別,VARCHAR,100,INDEX")]
        類別,

        /// <summary>
        /// 顯示名稱。
        /// </summary>
        [Description("名稱,VARCHAR,200,NONE")]
        名稱,

        /// <summary>
        /// 程式查詢用鍵值。
        /// </summary>
        [Description("鍵值,VARCHAR,200,INDEX")]
        鍵值,

        /// <summary>
        /// 資料類型。
        /// string / number / bool / json / image / file
        /// </summary>
        [Description("資料類型,VARCHAR,50,INDEX")]
        資料類型,

        /// <summary>
        /// 一般文字值。
        /// </summary>
        [Description("文字值,LONGTEXT,50,NONE")]
        文字值,

        /// <summary>
        /// JSON 字串值。
        /// </summary>
        [Description("JSON值,LONGTEXT,50,NONE")]
        JSON值,

        /// <summary>
        /// 圖片或檔案 Base64。
        /// </summary>
        [Description("base64,LONGTEXT,50,NONE")]
        base64,

        /// <summary>
        /// 檔案名稱。
        /// </summary>
        [Description("檔案名稱,VARCHAR,300,NONE")]
        檔案名稱,

        /// <summary>
        /// 副檔名。
        /// </summary>
        [Description("副檔名,VARCHAR,50,NONE")]
        副檔名,

        /// <summary>
        /// Content-Type。
        /// </summary>
        [Description("ContentType,VARCHAR,100,NONE")]
        ContentType,

        /// <summary>
        /// 檔案大小，單位 bytes。
        /// </summary>
        [Description("檔案大小,VARCHAR,50,NONE")]
        檔案大小,

        /// <summary>
        /// 版本。
        /// </summary>
        [Description("版本,VARCHAR,50,NONE")]
        版本,

        /// <summary>
        /// 是否啟用。
        /// true / false
        /// </summary>
        [Description("啟用,VARCHAR,10,INDEX")]
        啟用,

        /// <summary>
        /// 備註。
        /// </summary>
        [Description("備註,VARCHAR,500,NONE")]
        備註,

        /// <summary>
        /// 建立時間。
        /// </summary>
        [Description("建立時間,DATETIME,20,NONE")]
        建立時間,

        /// <summary>
        /// 更新時間。
        /// </summary>
        [Description("更新時間,DATETIME,20,INDEX")]
        更新時間,
    }

    /// <summary>
    /// 系統參數資料類型。
    /// </summary>
    /// <remarks>
    /// string 與 bool 為 C# 關鍵字，因此 enum 成員使用 @string、@bool。
    /// 存入資料庫時仍會轉成 string、bool。
    /// </remarks>
    public enum enum_system_config_value_type
    {
        /// <summary>
        /// 一般字串。
        /// </summary>
        @string,

        /// <summary>
        /// 數字。
        /// </summary>
        number,

        /// <summary>
        /// 布林值。
        /// </summary>
        @bool,

        /// <summary>
        /// JSON 字串。
        /// </summary>
        json,

        /// <summary>
        /// 圖片 Base64。
        /// </summary>
        image,

        /// <summary>
        /// 檔案 Base64。
        /// </summary>
        file
    }

    /// <summary>
    /// 系統參數資料類別。
    /// </summary>
    /// <remarks>
    /// 可儲存：
    /// 1. 一般參數。
    /// 2. 數字參數。
    /// 3. 布林參數。
    /// 4. JSON 設定。
    /// 5. 圖片 Base64。
    /// 6. 檔案 Base64。
    ///
    /// 主要 API：
    /// POST /api/systemConfig/init
    /// POST /api/systemConfig/set
    /// POST /api/systemConfig/set_list
    /// POST /api/systemConfig/get_by_key
    /// POST /api/systemConfig/get_by_category
    /// POST /api/systemConfig/get_all
    /// POST /api/systemConfig/delete_by_guid
    /// POST /api/systemConfig/delete_by_key
    ///
    /// 更新參數：
    /// 呼叫 set() 即可。
    /// 若 類別 + 鍵值 已存在，API 端會更新。
    /// 若 類別 + 鍵值 不存在，API 端會新增。
    /// </remarks>
    public class systemConfigClass
    {
        /// <summary>
        /// 唯一識別碼。
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        /// <summary>
        /// 類別。
        /// 例如：系統設定、圖片設定、報表模板、列印設定。
        /// </summary>
        [JsonPropertyName("category")]
        public string 類別 { get; set; }

        /// <summary>
        /// 顯示名稱。
        /// </summary>
        [JsonPropertyName("name")]
        public string 名稱 { get; set; }

        /// <summary>
        /// 鍵值。
        /// 程式查詢用，建議使用英文，例如：company_name、login_logo。
        /// </summary>
        [JsonPropertyName("key")]
        public string 鍵值 { get; set; }

        /// <summary>
        /// 資料類型。
        /// string / number / bool / json / image / file
        /// </summary>
        [JsonPropertyName("value_type")]
        public string 資料類型 { get; set; }

        /// <summary>
        /// 一般文字值。
        /// 適用 string / number / bool。
        /// </summary>
        [JsonPropertyName("value")]
        public string 文字值 { get; set; }

        /// <summary>
        /// JSON 字串值。
        /// 適用 json。
        /// </summary>
        [JsonPropertyName("json_value")]
        public string JSON值 { get; set; }

        /// <summary>
        /// 圖片或檔案 Base64。
        /// 適用 image / file。
        /// </summary>
        [JsonPropertyName("base64")]
        public string base64 { get; set; }

        /// <summary>
        /// 檔案名稱。
        /// </summary>
        [JsonPropertyName("file_name")]
        public string 檔案名稱 { get; set; }

        /// <summary>
        /// 副檔名。
        /// 例如：.png、.jpg、.pdf、.xlsx。
        /// </summary>
        [JsonPropertyName("extension")]
        public string 副檔名 { get; set; }

        /// <summary>
        /// ContentType。
        /// 例如：image/png、application/pdf。
        /// </summary>
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// 檔案大小。
        /// 單位 bytes，以字串儲存。
        /// </summary>
        [JsonPropertyName("file_size")]
        public string 檔案大小 { get; set; }

        /// <summary>
        /// 版本。
        /// </summary>
        [JsonPropertyName("version")]
        public string 版本 { get; set; }

        /// <summary>
        /// 是否啟用。
        /// true / false。
        /// </summary>
        [JsonPropertyName("enable")]
        public string 啟用 { get; set; }

        /// <summary>
        /// 備註。
        /// </summary>
        [JsonPropertyName("remark")]
        public string 備註 { get; set; }

        /// <summary>
        /// 建立時間。
        /// 格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [JsonPropertyName("created_at")]
        public string 建立時間 { get; set; }

        /// <summary>
        /// 更新時間。
        /// 格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string 更新時間 { get; set; }

        #region Static Helper Methods

        /// <summary>
        /// 取得系統參數資料類型字串。
        /// </summary>
        /// <param name="valueType">系統參數資料類型 enum。</param>
        /// <returns>回傳 string / number / bool / json / image / file。</returns>
        public static string GetValueTypeName(enum_system_config_value_type valueType)
        {
            return valueType.ToString().Replace("@", "").ToLower();
        }

        /// <summary>
        /// 判斷資料類型是否合法。
        /// </summary>
        /// <param name="valueType">資料類型字串。</param>
        /// <returns>合法回傳 true，否則回傳 false。</returns>
        public static bool IsValidValueType(string valueType)
        {
            if (valueType.StringIsEmpty()) return false;

            valueType = valueType.Trim().ToLower();

            foreach (enum_system_config_value_type item in Enum.GetValues(typeof(enum_system_config_value_type)))
            {
                string name = GetValueTypeName(item);
                if (name == valueType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 建立一般文字參數物件。
        /// </summary>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="value">文字值。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 systemConfigClass。</returns>
        public static systemConfigClass CreateString(string category, string key, string name, string value, string remark = "")
        {
            return new systemConfigClass()
            {
                類別 = category,
                鍵值 = key,
                名稱 = name,
                資料類型 = GetValueTypeName(enum_system_config_value_type.@string),
                文字值 = value,
                JSON值 = "",
                base64 = "",
                檔案名稱 = "",
                副檔名 = "",
                ContentType = "",
                啟用 = "true",
                備註 = remark
            };
        }

        /// <summary>
        /// 建立數字參數物件。
        /// </summary>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="value">數字值。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 systemConfigClass。</returns>
        public static systemConfigClass CreateNumber(string category, string key, string name, double value, string remark = "")
        {
            return new systemConfigClass()
            {
                類別 = category,
                鍵值 = key,
                名稱 = name,
                資料類型 = GetValueTypeName(enum_system_config_value_type.number),
                文字值 = value.ToString(),
                JSON值 = "",
                base64 = "",
                檔案名稱 = "",
                副檔名 = "",
                ContentType = "",
                啟用 = "true",
                備註 = remark
            };
        }

        /// <summary>
        /// 建立布林參數物件。
        /// </summary>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="value">布林值。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 systemConfigClass。</returns>
        public static systemConfigClass CreateBool(string category, string key, string name, bool value, string remark = "")
        {
            return new systemConfigClass()
            {
                類別 = category,
                鍵值 = key,
                名稱 = name,
                資料類型 = GetValueTypeName(enum_system_config_value_type.@bool),
                文字值 = value ? "true" : "false",
                JSON值 = "",
                base64 = "",
                檔案名稱 = "",
                副檔名 = "",
                ContentType = "",
                啟用 = "true",
                備註 = remark
            };
        }

        /// <summary>
        /// 建立 JSON 參數物件。
        /// </summary>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="jsonValue">JSON 字串。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 systemConfigClass。</returns>
        public static systemConfigClass CreateJson(string category, string key, string name, string jsonValue, string remark = "")
        {
            return new systemConfigClass()
            {
                類別 = category,
                鍵值 = key,
                名稱 = name,
                資料類型 = GetValueTypeName(enum_system_config_value_type.json),
                文字值 = "",
                JSON值 = jsonValue,
                base64 = "",
                檔案名稱 = "",
                副檔名 = "",
                ContentType = "",
                啟用 = "true",
                備註 = remark
            };
        }

        /// <summary>
        /// 建立圖片 Base64 參數物件。
        /// </summary>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="base64">圖片 Base64。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 systemConfigClass。</returns>
        public static systemConfigClass CreateImageBase64(string category, string key, string name, string base64, string remark = "")
        {
            return new systemConfigClass()
            {
                類別 = category,
                鍵值 = key,
                名稱 = name,
                資料類型 = GetValueTypeName(enum_system_config_value_type.image),
                文字值 = "",
                JSON值 = "",
                base64 = base64,
                檔案名稱 = "",
                副檔名 = ".png",
                ContentType = "image/png",
                啟用 = "true",
                備註 = remark
            };
        }

        /// <summary>
        /// 建立圖片參數物件。
        /// 圖片會固定轉成 PNG Base64。
        /// </summary>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="image">圖片物件。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 systemConfigClass。</returns>
        public static systemConfigClass CreateImage(string category, string key, string name, Image image, string remark = "")
        {
            string base64 = ImageToBase64(image);
            return CreateImageBase64(category, key, name, base64, remark);
        }

        /// <summary>
        /// 建立檔案 Base64 參數物件。
        /// </summary>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="base64">檔案 Base64。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 systemConfigClass。</returns>
        public static systemConfigClass CreateFileBase64(string category, string key, string name, string base64, string remark = "")
        {
            return new systemConfigClass()
            {
                類別 = category,
                鍵值 = key,
                名稱 = name,
                資料類型 = GetValueTypeName(enum_system_config_value_type.file),
                文字值 = "",
                JSON值 = "",
                base64 = base64,
                檔案名稱 = "",
                副檔名 = "",
                ContentType = "application/octet-stream",
                啟用 = "true",
                備註 = remark
            };
        }

        /// <summary>
        /// 將 Image 轉成 PNG Base64 字串。
        /// </summary>
        /// <param name="image">圖片物件。</param>
        /// <returns>回傳 Base64 字串，失敗回傳空字串。</returns>
        public static string ImageToBase64(Image image)
        {
            if (image == null) return "";

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    byte[] bytes = ms.ToArray();
                    return Convert.ToBase64String(bytes);
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 將檔案轉成 Base64 字串。
        /// </summary>
        /// <param name="filePath">檔案路徑。</param>
        /// <returns>回傳 Base64 字串。若檔案不存在則回傳空字串。</returns>
        public static string FileToBase64(string filePath)
        {
            if (filePath.StringIsEmpty()) return "";
            if (File.Exists(filePath) == false) return "";

            byte[] bytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 取得常見副檔名對應的 ContentType。
        /// </summary>
        /// <param name="extension">副檔名，例如 .png。</param>
        /// <returns>回傳 ContentType。</returns>
        public static string GetContentTypeByExtension(string extension)
        {
            if (extension.StringIsEmpty()) return "application/octet-stream";

            extension = extension.Trim().ToLower();

            if (extension == ".png") return "image/png";
            if (extension == ".jpg") return "image/jpeg";
            if (extension == ".jpeg") return "image/jpeg";
            if (extension == ".gif") return "image/gif";
            if (extension == ".bmp") return "image/bmp";
            if (extension == ".webp") return "image/webp";
            if (extension == ".pdf") return "application/pdf";
            if (extension == ".xlsx") return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            if (extension == ".xls") return "application/vnd.ms-excel";
            if (extension == ".docx") return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            if (extension == ".doc") return "application/msword";
            if (extension == ".txt") return "text/plain";
            if (extension == ".json") return "application/json";
            if (extension == ".zip") return "application/zip";

            return "application/octet-stream";
        }

        #endregion

        #region API Client Methods

        /// <summary>
        /// 初始化 system_config 資料表。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <returns>回傳資料表資訊。</returns>
        public static SQLUI.Table init(string API_Server)
        {
            string url = $"{API_Server}/api/systemConfig/init";

            returnData returnData = new returnData();

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            SQLUI.Table table = json_out.JsonDeserializet<SQLUI.Table>();
            return table;
        }

        /// <summary>
        /// 新增或更新單一系統參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="config">系統參數物件。</param>
        /// <returns>回傳 API 執行結果。</returns>
        /// <remarks>
        /// 若 類別 + 鍵值 已存在，API 端會更新。
        /// 若 類別 + 鍵值 不存在，API 端會新增。
        /// </remarks>
        public static returnData set(string API_Server, systemConfigClass config)
        {
            string url = $"{API_Server}/api/systemConfig/set";

            returnData returnData = new returnData();
            returnData.Data = config;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            return returnData_out;
        }

        /// <summary>
        /// 新增或更新多筆系統參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="configs">系統參數清單。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData set_list(string API_Server, List<systemConfigClass> configs)
        {
            string url = $"{API_Server}/api/systemConfig/set_list";

            returnData returnData = new returnData();
            returnData.Data = configs;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            return returnData_out;
        }

        /// <summary>
        /// 更新或新增文字參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="value">文字值。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData set_string(string API_Server, string category, string key, string name, string value, string remark = "")
        {
            systemConfigClass config = CreateString(category, key, name, value, remark);
            return set(API_Server, config);
        }

        /// <summary>
        /// 更新或新增數字參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="value">數字值。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData set_number(string API_Server, string category, string key, string name, double value, string remark = "")
        {
            systemConfigClass config = CreateNumber(category, key, name, value, remark);
            return set(API_Server, config);
        }

        /// <summary>
        /// 更新或新增布林參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="value">布林值。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData set_bool(string API_Server, string category, string key, string name, bool value, string remark = "")
        {
            systemConfigClass config = CreateBool(category, key, name, value, remark);
            return set(API_Server, config);
        }

        /// <summary>
        /// 更新或新增 JSON 參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="jsonValue">JSON 字串。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData set_json(string API_Server, string category, string key, string name, string jsonValue, string remark = "")
        {
            systemConfigClass config = CreateJson(category, key, name, jsonValue, remark);
            return set(API_Server, config);
        }

        /// <summary>
        /// 更新或新增圖片 Base64 參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="base64">圖片 Base64。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData set_image_base64(string API_Server, string category, string key, string name, string base64, string remark = "")
        {
            systemConfigClass config = CreateImageBase64(category, key, name, base64, remark);
            return set(API_Server, config);
        }

        /// <summary>
        /// 更新或新增圖片參數。
        /// 圖片會固定轉成 PNG Base64。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="image">圖片物件。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData set_image(string API_Server, string category, string key, string name, Image image, string remark = "")
        {
            if (image == null)
            {
                returnData error = new returnData();
                error.Code = -200;
                error.Result = "image 不可為 null";
                return error;
            }

            systemConfigClass config = CreateImage(category, key, name, image, remark);
            return set(API_Server, config);
        }

        /// <summary>
        /// 更新或新增檔案 Base64 參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="base64">檔案 Base64。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData set_file_base64(string API_Server, string category, string key, string name, string base64, string remark = "")
        {
            systemConfigClass config = CreateFileBase64(category, key, name, base64, remark);
            return set(API_Server, config);
        }

        /// <summary>
        /// 更新或新增本機檔案參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="name">名稱。</param>
        /// <param name="filePath">本機檔案路徑。</param>
        /// <param name="isImage">是否為圖片。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData set_file_from_path(string API_Server, string category, string key, string name, string filePath, bool isImage = false, string remark = "")
        {
            if (File.Exists(filePath) == false)
            {
                returnData error = new returnData();
                error.Code = -200;
                error.Result = $"檔案不存在：{filePath}";
                return error;
            }

            string base64 = FileToBase64(filePath);

            if (isImage)
            {
                return set_image_base64(API_Server, category, key, name, base64, remark);
            }
            else
            {
                return set_file_base64(API_Server, category, key, name, base64, remark);
            }
        }

        /// <summary>
        /// 依照類別與鍵值取得系統參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <returns>回傳系統參數物件，查無資料則回傳 null。</returns>
        public static systemConfigClass get_by_key(string API_Server, string category, string key)
        {
            string url = $"{API_Server}/api/systemConfig/get_by_key";

            returnData returnData = new returnData();
            returnData.ValueAry.Add($"category={category}");
            returnData.ValueAry.Add($"key={key}");

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null) return null;
            if (returnData_out.Data == null) return null;
            if (returnData_out.Code != 200) return null;

            return returnData_out.Data.ObjToClass<systemConfigClass>();
        }

        /// <summary>
        /// 依照類別取得系統參數清單。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <returns>回傳系統參數清單。</returns>
        public static List<systemConfigClass> get_by_category(string API_Server, string category)
        {
            string url = $"{API_Server}/api/systemConfig/get_by_category";

            returnData returnData = new returnData();
            returnData.ValueAry.Add($"category={category}");

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null) return new List<systemConfigClass>();
            if (returnData_out.Data == null) return new List<systemConfigClass>();
            if (returnData_out.Code != 200) return new List<systemConfigClass>();

            return returnData_out.Data.ObjToClass<List<systemConfigClass>>();
        }

        /// <summary>
        /// 取得全部系統參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <returns>回傳系統參數清單。</returns>
        public static List<systemConfigClass> get_all(string API_Server)
        {
            string url = $"{API_Server}/api/systemConfig/get_all";

            returnData returnData = new returnData();

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null) return new List<systemConfigClass>();
            if (returnData_out.Data == null) return new List<systemConfigClass>();
            if (returnData_out.Code != 200) return new List<systemConfigClass>();

            return returnData_out.Data.ObjToClass<List<systemConfigClass>>();
        }

        /// <summary>
        /// 依 GUID 刪除系統參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="guid">GUID。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData delete_by_guid(string API_Server, string guid)
        {
            string url = $"{API_Server}/api/systemConfig/delete_by_guid";

            returnData returnData = new returnData();
            returnData.ValueAry.Add($"GUID={guid}");

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            return returnData_out;
        }

        /// <summary>
        /// 依類別與鍵值刪除系統參數。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <returns>回傳 API 執行結果。</returns>
        public static returnData delete_by_key(string API_Server, string category, string key)
        {
            string url = $"{API_Server}/api/systemConfig/delete_by_key";

            returnData returnData = new returnData();
            returnData.ValueAry.Add($"category={category}");
            returnData.ValueAry.Add($"key={key}");

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);

            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            return returnData_out;
        }

        #endregion

        #region Get Value Helper Methods

        /// <summary>
        /// 取得文字值。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="defaultValue">預設值。</param>
        /// <returns>回傳文字值。</returns>
        public static string get_value(string API_Server, string category, string key, string defaultValue = "")
        {
            systemConfigClass config = get_by_key(API_Server, category, key);

            if (config == null) return defaultValue;
            if (config.文字值.StringIsEmpty()) return defaultValue;

            return config.文字值;
        }

        /// <summary>
        /// 取得 JSON 字串值。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="defaultValue">預設值。</param>
        /// <returns>回傳 JSON 字串。</returns>
        public static string get_json(string API_Server, string category, string key, string defaultValue = "")
        {
            systemConfigClass config = get_by_key(API_Server, category, key);

            if (config == null) return defaultValue;
            if (config.JSON值.StringIsEmpty()) return defaultValue;

            return config.JSON值;
        }

        /// <summary>
        /// 取得布林值。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="defaultValue">預設值。</param>
        /// <returns>回傳布林值。</returns>
        public static bool get_bool(string API_Server, string category, string key, bool defaultValue = false)
        {
            string value = get_value(API_Server, category, key, "");

            if (value.StringIsEmpty()) return defaultValue;

            value = value.Trim().ToLower();

            if (value == "true") return true;
            if (value == "1") return true;
            if (value == "yes") return true;
            if (value == "y") return true;

            if (value == "false") return false;
            if (value == "0") return false;
            if (value == "no") return false;
            if (value == "n") return false;

            return defaultValue;
        }

        /// <summary>
        /// 取得整數值。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="defaultValue">預設值。</param>
        /// <returns>回傳整數值。</returns>
        public static int get_int(string API_Server, string category, string key, int defaultValue = 0)
        {
            string value = get_value(API_Server, category, key, "");

            if (value.StringIsEmpty()) return defaultValue;

            int result = 0;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 取得 double 數值。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="defaultValue">預設值。</param>
        /// <returns>回傳 double 數值。</returns>
        public static double get_double(string API_Server, string category, string key, double defaultValue = 0)
        {
            string value = get_value(API_Server, category, key, "");

            if (value.StringIsEmpty()) return defaultValue;

            double result = 0;
            if (double.TryParse(value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 取得 Base64。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <returns>回傳 Base64 字串。</returns>
        public static string get_base64(string API_Server, string category, string key)
        {
            systemConfigClass config = get_by_key(API_Server, category, key);

            if (config == null) return "";
            if (config.base64.StringIsEmpty()) return "";

            return config.base64;
        }

        /// <summary>
        /// 取得檔案 bytes。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <returns>回傳 byte[]，失敗回傳 null。</returns>
        public static byte[] get_file_bytes(string API_Server, string category, string key)
        {
            string base64 = get_base64(API_Server, category, key);

            if (base64.StringIsEmpty()) return null;

            try
            {
                return Convert.FromBase64String(base64);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 取得 Image 圖片物件。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <returns>回傳 Image，失敗回傳 null。</returns>
        public static Image get_image(string API_Server, string category, string key)
        {
            byte[] bytes = get_file_bytes(API_Server, category, key);

            if (bytes == null) return null;

            try
            {
                MemoryStream ms = new MemoryStream(bytes);
                Image image = Image.FromStream(ms);
                return image;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 將系統參數中的 Base64 檔案另存到本機路徑。
        /// </summary>
        /// <param name="API_Server">API Server 位址。</param>
        /// <param name="category">類別。</param>
        /// <param name="key">鍵值。</param>
        /// <param name="savePath">儲存路徑。</param>
        /// <returns>成功回傳 true，失敗回傳 false。</returns>
        public static bool save_file_to_path(string API_Server, string category, string key, string savePath)
        {
            byte[] bytes = get_file_bytes(API_Server, category, key);

            if (bytes == null) return false;
            if (savePath.StringIsEmpty()) return false;

            try
            {
                string dir = Path.GetDirectoryName(savePath);
                if (dir.StringIsEmpty() == false && Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllBytes(savePath, bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}