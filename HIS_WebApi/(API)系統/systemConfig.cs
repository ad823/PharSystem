using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace HIS_WebApi
{
    /// <summary>
    /// 系統參數管理 API。
    /// </summary>
    /// <remarks>
    /// Controller 路由：
    /// /api/systemConfig
    ///
    /// 功能：
    /// 1. 儲存一般參數。
    /// 2. 儲存數字參數。
    /// 3. 儲存布林參數。
    /// 4. 儲存 JSON 設定。
    /// 5. 儲存圖片 Base64。
    /// 6. 儲存檔案 Base64。
    ///
    /// 設計規則：
    /// 1. 檔案與圖片全部以 Base64 儲存在資料庫。
    /// 2. 單筆原始檔案大小不可超過 10MB。
    /// 3. 類別 + 鍵值 不可重複。
    /// 4. 若 set 時資料已存在，則更新。
    /// 5. 若 set 時資料不存在，則新增。
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
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class systemConfig : Controller
    {
        static public string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        /// <summary>
        /// 最大檔案大小限制。
        /// 10MB。
        /// </summary>
        private const int MaxFileSizeBytes = 10 * 1024 * 1024;

        #region API

        /// <summary>
        /// 初始化 system_config 資料表。
        /// </summary>
        /// <remarks>
        /// API URL：
        /// POST /api/systemConfig/init
        ///
        /// 用途：
        /// 建立 system_config 資料表。
        ///
        /// Request 範例：
        /// <code>
        /// {
        ///   "Data": {}
        /// }
        /// </code>
        ///
        /// Response 範例：
        /// <code>
        /// {
        ///   "Code": 200,
        ///   "Result": "system_config 資料表初始化完成"
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 JSON 字串</returns>
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(systemConfigClass))]
        [Route("init")]
        [HttpPost]
        public string GET_init([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            if (returnData == null) returnData = new returnData();

            try
            {
                returnData.Method = "init";
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無Server資料!";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt();
                }

                string result = CheckCreatTable(sys_serverSettingClasses[0]);
                return result;
            }
            catch (Exception ex)
            {
                returnData.Code = -500;
                returnData.Result = $"內部伺服器錯誤: {ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 新增或更新單一系統參數。
        /// </summary>
        /// <remarks>
        /// API URL：
        /// POST /api/systemConfig/set
        ///
        /// 用途：
        /// 依照「類別 + 鍵值」新增或更新系統參數。
        ///
        /// 規則：
        /// 1. 類別 + 鍵值 已存在時，更新原資料。
        /// 2. 類別 + 鍵值 不存在時，新增資料。
        /// 3. image / file 類型必須提供 base64。
        /// 4. base64 原始檔案大小不可超過 10MB。
        ///
        /// Request 範例：
        /// <code>
        /// {
        ///   "Data": {
        ///     "category": "系統設定",
        ///     "name": "公司名稱",
        ///     "key": "company_name",
        ///     "value_type": "string",
        ///     "value": "鴻森智能科技股份有限公司",
        ///     "enable": "true",
        ///     "remark": "系統顯示用公司名稱"
        ///   }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 JSON 字串</returns>
        [HttpPost("set")]
        public string POST_set([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            if (returnData == null) returnData = new returnData();

            returnData.Method = "set";
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

            try
            {
                GET_init(returnData);

                SQLControl sQLControl_systemConfig = GetSQLControl(returnData);
                if (sQLControl_systemConfig == null)
                {
                    return returnData.JsonSerializationt();
                }

                systemConfigClass input = returnData.Data.ObjToClass<systemConfigClass>();
                string validation = ValidateSystemConfig(input);
                if (validation.StringIsEmpty() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = validation;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt();
                }

                NormalizeSystemConfig(input);

                List<object[]> rows = GetRowsByCategoryKey(sQLControl_systemConfig, input.類別, input.鍵值);

                List<systemConfigClass> addList = new List<systemConfigClass>();
                List<systemConfigClass> replaceList = new List<systemConfigClass>();

                if (rows.Count == 0)
                {
                    input.GUID = Guid.NewGuid().ToString().ToUpper();
                    input.建立時間 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    input.更新時間 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    addList.Add(input);
                }
                else
                {
                    systemConfigClass oldConfig = rows[0].SQLToClass<systemConfigClass, enum_system_config>();

                    input.GUID = oldConfig.GUID;
                    input.建立時間 = oldConfig.建立時間;
                    input.更新時間 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    replaceList.Add(input);
                }

                List<object[]> list_add = addList.ClassToSQL<systemConfigClass, enum_system_config>();
                List<object[]> list_replace = replaceList.ClassToSQL<systemConfigClass, enum_system_config>();

                if (list_add.Count > 0)
                {
                    sQLControl_systemConfig.AddRows(null, list_add);
                }

                if (list_replace.Count > 0)
                {
                    sQLControl_systemConfig.UpdateByDefulteExtra(null, list_replace);
                }

                returnData.Code = 200;
                returnData.Result = $"儲存系統參數成功,新增<{list_add.Count}>筆,修改<{list_replace.Count}>筆";
                returnData.Data = input;
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -500;
                returnData.Result = $"內部伺服器錯誤: {ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 批次新增或更新系統參數。
        /// </summary>
        /// <remarks>
        /// API URL：
        /// POST /api/systemConfig/set_list
        ///
        /// 用途：
        /// 一次新增或更新多筆系統參數。
        ///
        /// Request 範例：
        /// <code>
        /// {
        ///   "Data": [
        ///     {
        ///       "category": "系統設定",
        ///       "name": "公司名稱",
        ///       "key": "company_name",
        ///       "value_type": "string",
        ///       "value": "鴻森智能科技股份有限公司",
        ///       "enable": "true"
        ///     },
        ///     {
        ///       "category": "功能開關",
        ///       "name": "啟用語音",
        ///       "key": "enable_voice",
        ///       "value_type": "bool",
        ///       "value": "true",
        ///       "enable": "true"
        ///     }
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 JSON 字串</returns>
        [HttpPost("set_list")]
        public string POST_set_list([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            if (returnData == null) returnData = new returnData();

            returnData.Method = "set_list";
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

            try
            {
                GET_init(returnData);

                SQLControl sQLControl_systemConfig = GetSQLControl(returnData);
                if (sQLControl_systemConfig == null)
                {
                    return returnData.JsonSerializationt();
                }

                List<systemConfigClass> inputs = returnData.Data.ObjToClass<List<systemConfigClass>>();
                if (inputs == null || inputs.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "Data 不可為空";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt();
                }

                List<systemConfigClass> addList = new List<systemConfigClass>();
                List<systemConfigClass> replaceList = new List<systemConfigClass>();
                List<systemConfigClass> outputList = new List<systemConfigClass>();

                for (int i = 0; i < inputs.Count; i++)
                {
                    systemConfigClass input = inputs[i];

                    string validation = ValidateSystemConfig(input);
                    if (validation.StringIsEmpty() == false)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"第<{i + 1}>筆資料異常 : {validation}";
                        returnData.TimeTaken = myTimerBasic.ToString();
                        return returnData.JsonSerializationt();
                    }

                    NormalizeSystemConfig(input);

                    List<object[]> rows = GetRowsByCategoryKey(sQLControl_systemConfig, input.類別, input.鍵值);

                    if (rows.Count == 0)
                    {
                        input.GUID = Guid.NewGuid().ToString().ToUpper();
                        input.建立時間 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        input.更新時間 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        addList.Add(input);
                        outputList.Add(input);
                    }
                    else
                    {
                        systemConfigClass oldConfig = rows[0].SQLToClass<systemConfigClass, enum_system_config>();

                        input.GUID = oldConfig.GUID;
                        input.建立時間 = oldConfig.建立時間;
                        input.更新時間 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        replaceList.Add(input);
                        outputList.Add(input);
                    }
                }

                List<object[]> list_add = addList.ClassToSQL<systemConfigClass, enum_system_config>();
                List<object[]> list_replace = replaceList.ClassToSQL<systemConfigClass, enum_system_config>();

                if (list_add.Count > 0)
                {
                    sQLControl_systemConfig.AddRows(null, list_add);
                }

                if (list_replace.Count > 0)
                {
                    sQLControl_systemConfig.UpdateByDefulteExtra(null, list_replace);
                }

                returnData.Code = 200;
                returnData.Result = $"儲存系統參數成功,新增<{list_add.Count}>筆,修改<{list_replace.Count}>筆";
                returnData.Data = outputList;
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -500;
                returnData.Result = $"內部伺服器錯誤: {ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 依類別與鍵值取得系統參數。
        /// </summary>
        /// <remarks>
        /// API URL：
        /// POST /api/systemConfig/get_by_key
        ///
        /// ValueAry 支援兩種格式：
        ///
        /// 格式一：
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "category=系統設定",
        ///     "key=company_name"
        ///   ]
        /// }
        /// </code>
        ///
        /// 格式二：
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "系統設定",
        ///     "company_name"
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 JSON 字串</returns>
        [HttpPost("get_by_key")]
        public string POST_get_by_key([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            if (returnData == null) returnData = new returnData();

            returnData.Method = "get_by_key";
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

            try
            {
                SQLControl sQLControl_systemConfig = GetSQLControl(returnData);
                if (sQLControl_systemConfig == null)
                {
                    return returnData.JsonSerializationt();
                }

                string category = GetValue(returnData, "category");
                string key = GetValue(returnData, "key");

                if (category.StringIsEmpty() || key.StringIsEmpty())
                {
                    if (returnData.ValueAry == null || returnData.ValueAry.Count != 2)
                    {
                        returnData.Code = -200;
                        returnData.Result = "returnData.ValueAry 內容應為[category,key]或[category=類別,key=鍵值]";
                        returnData.TimeTaken = myTimerBasic.ToString();
                        return returnData.JsonSerializationt(true);
                    }

                    category = returnData.ValueAry[0];
                    key = returnData.ValueAry[1];
                }

                List<object[]> rows = GetRowsByCategoryKey(sQLControl_systemConfig, category, key);

                if (rows.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                systemConfigClass config = rows[0].SQLToClass<systemConfigClass, enum_system_config>();

                returnData.Code = 200;
                returnData.Result = "取得系統參數成功";
                returnData.Data = config;
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -500;
                returnData.Result = $"內部伺服器錯誤: {ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 依類別取得系統參數清單。
        /// </summary>
        /// <remarks>
        /// API URL：
        /// POST /api/systemConfig/get_by_category
        ///
        /// ValueAry 支援兩種格式：
        ///
        /// 格式一：
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "category=圖片設定"
        ///   ]
        /// }
        /// </code>
        ///
        /// 格式二：
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "圖片設定"
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 JSON 字串</returns>
        [HttpPost("get_by_category")]
        public string POST_get_by_category([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            if (returnData == null) returnData = new returnData();

            returnData.Method = "get_by_category";
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

            try
            {
                SQLControl sQLControl_systemConfig = GetSQLControl(returnData);
                if (sQLControl_systemConfig == null)
                {
                    return returnData.JsonSerializationt();
                }

                string category = GetValue(returnData, "category");

                if (category.StringIsEmpty())
                {
                    if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                    {
                        returnData.Code = -200;
                        returnData.Result = "returnData.ValueAry 內容應為[category]或[category=類別]";
                        returnData.TimeTaken = myTimerBasic.ToString();
                        return returnData.JsonSerializationt(true);
                    }

                    category = returnData.ValueAry[0];
                }

                List<object[]> rows = sQLControl_systemConfig.GetRowsByDefult(null, (int)enum_system_config.類別, category);
                List<systemConfigClass> configs = rows.SQLToClass<systemConfigClass, enum_system_config>();

                configs = configs
                    .OrderByDescending(x => x.更新時間)
                    .ToList();

                returnData.Code = 200;
                returnData.Result = $"取得系統參數成功,共<{configs.Count}>筆";
                returnData.Data = configs;
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -500;
                returnData.Result = $"內部伺服器錯誤: {ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 查詢全部系統參數。
        /// </summary>
        /// <remarks>
        /// API URL：
        /// POST /api/systemConfig/get_all
        ///
        /// ValueAry 可用參數：
        /// page=1
        /// pageSize=50
        /// category=系統設定
        /// keyword=logo
        /// enable=true
        /// value_type=image
        ///
        /// Request 範例：
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "page=1",
        ///     "pageSize=50",
        ///     "keyword=logo"
        ///   ]
        /// }
        /// </code>
        ///
        /// Response：
        /// Data：systemConfigClass List
        /// ValueAry：
        /// [
        ///   "TotalCount=100",
        ///   "TotalPages=2",
        ///   "CurrentPage=1",
        ///   "PageSize=50"
        /// ]
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 JSON 字串</returns>
        [HttpPost("get_all")]
        public string POST_get_all([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            if (returnData == null) returnData = new returnData();

            returnData.Method = "get_all";
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

            try
            {
                SQLControl sQLControl_systemConfig = GetSQLControl(returnData);
                if (sQLControl_systemConfig == null)
                {
                    return returnData.JsonSerializationt();
                }

                int page = GetValue(returnData, "page").StringToInt32();
                int pageSize = GetValue(returnData, "pageSize").StringToInt32();

                if (page <= 0) page = 1;
                if (pageSize <= 0) pageSize = 50;

                string category = GetValue(returnData, "category");
                string keyword = GetValue(returnData, "keyword");
                string enable = GetValue(returnData, "enable");
                string valueType = GetValue(returnData, "value_type");

                List<object[]> rows = sQLControl_systemConfig.GetAllRows(null);
                List<systemConfigClass> configs = rows.SQLToClass<systemConfigClass, enum_system_config>();

                if (category.StringIsEmpty() == false)
                {
                    configs = configs.Where(x => x.類別 == category).ToList();
                }

                if (enable.StringIsEmpty() == false)
                {
                    configs = configs.Where(x => x.啟用 == enable).ToList();
                }

                if (valueType.StringIsEmpty() == false)
                {
                    configs = configs.Where(x => x.資料類型 == valueType).ToList();
                }

                if (keyword.StringIsEmpty() == false)
                {
                    configs = configs.Where(x =>
                        (x.名稱 != null && x.名稱.Contains(keyword)) ||
                        (x.鍵值 != null && x.鍵值.Contains(keyword)) ||
                        (x.備註 != null && x.備註.Contains(keyword))
                    ).ToList();
                }

                configs = configs
                    .OrderByDescending(x => x.更新時間)
                    .ToList();

                int totalCount = configs.Count;
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                configs = configs
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                returnData.Code = 200;
                returnData.Result = $"取得系統參數成功,共<{totalCount}>筆";
                returnData.Data = configs;
                returnData.ValueAry = new List<string>();
                returnData.ValueAry.Add($"TotalCount={totalCount}");
                returnData.ValueAry.Add($"TotalPages={totalPages}");
                returnData.ValueAry.Add($"CurrentPage={page}");
                returnData.ValueAry.Add($"PageSize={pageSize}");
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -500;
                returnData.Result = $"內部伺服器錯誤: {ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 依 GUID 刪除系統參數。
        /// </summary>
        /// <remarks>
        /// API URL：
        /// POST /api/systemConfig/delete_by_guid
        ///
        /// ValueAry 支援兩種格式：
        ///
        /// 格式一：
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "GUID=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
        ///   ]
        /// }
        /// </code>
        ///
        /// 格式二：
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 JSON 字串</returns>
        [HttpPost("delete_by_guid")]
        public string POST_delete_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            if (returnData == null) returnData = new returnData();

            returnData.Method = "delete_by_guid";
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

            try
            {
                SQLControl sQLControl_systemConfig = GetSQLControl(returnData);
                if (sQLControl_systemConfig == null)
                {
                    return returnData.JsonSerializationt();
                }

                string guid = GetValue(returnData, "GUID");

                if (guid.StringIsEmpty())
                {
                    if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                    {
                        returnData.Code = -200;
                        returnData.Result = "returnData.ValueAry 內容應為[GUID]或[GUID=指定GUID]";
                        returnData.TimeTaken = myTimerBasic.ToString();
                        return returnData.JsonSerializationt(true);
                    }

                    guid = returnData.ValueAry[0];
                }

                List<object[]> rows = sQLControl_systemConfig.GetRowsByDefult(null, (int)enum_system_config.GUID, guid);

                if (rows.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                sQLControl_systemConfig.DeleteByDefult(null, (int)enum_system_config.GUID, guid);

                returnData.Code = 200;
                returnData.Result = $"刪除系統參數成功, GUID={guid}";
                returnData.Data = "";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -500;
                returnData.Result = $"內部伺服器錯誤: {ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 依類別與鍵值刪除系統參數。
        /// </summary>
        /// <remarks>
        /// API URL：
        /// POST /api/systemConfig/delete_by_key
        ///
        /// ValueAry 支援兩種格式：
        ///
        /// 格式一：
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "category=圖片設定",
        ///     "key=login_logo"
        ///   ]
        /// }
        /// </code>
        ///
        /// 格式二：
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "圖片設定",
        ///     "login_logo"
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 JSON 字串</returns>
        [HttpPost("delete_by_key")]
        public string POST_delete_by_key([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            if (returnData == null) returnData = new returnData();

            returnData.Method = "delete_by_key";
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

            try
            {
                SQLControl sQLControl_systemConfig = GetSQLControl(returnData);
                if (sQLControl_systemConfig == null)
                {
                    return returnData.JsonSerializationt();
                }

                string category = GetValue(returnData, "category");
                string key = GetValue(returnData, "key");

                if (category.StringIsEmpty() || key.StringIsEmpty())
                {
                    if (returnData.ValueAry == null || returnData.ValueAry.Count != 2)
                    {
                        returnData.Code = -200;
                        returnData.Result = "returnData.ValueAry 內容應為[category,key]或[category=類別,key=鍵值]";
                        returnData.TimeTaken = myTimerBasic.ToString();
                        return returnData.JsonSerializationt(true);
                    }

                    category = returnData.ValueAry[0];
                    key = returnData.ValueAry[1];
                }

                List<object[]> rows = GetRowsByCategoryKey(sQLControl_systemConfig, category, key);

                if (rows.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                systemConfigClass config = rows[0].SQLToClass<systemConfigClass, enum_system_config>();
                sQLControl_systemConfig.DeleteByDefult(null, (int)enum_system_config.GUID, config.GUID);

                returnData.Code = 200;
                returnData.Result = $"刪除系統參數成功, category={category}, key={key}";
                returnData.Data = "";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -500;
                returnData.Result = $"內部伺服器錯誤: {ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 上傳圖片參數。
        /// </summary>
        /// <remarks>
        /// API URL：
        /// POST /api/systemConfig/upload_image
        ///
        /// Content-Type：
        /// multipart/form-data
        ///
        /// Postman form-data 參數：
        /// image    File    必填，圖片檔案
        /// key      Text    必填，參數名/鍵值，例如 login_logo
        /// category Text    選填，預設「圖片設定」
        /// name     Text    選填，顯示名稱，未填時使用 key
        /// remark   Text    選填，備註
        ///
        /// 最小 Postman 參數：
        /// image = 選擇圖片檔
        /// key = login_logo
        ///
        /// 儲存規則：
        /// 1. 呼叫時會先執行 init，確保 system_config 資料表存在。
        /// 2. 圖片會轉成 Base64 存入 system_config。
        /// 3. 資料類型固定為 image。
        /// 4. 類別 category 預設為「圖片設定」。
        /// 5. 若 類別 + 鍵值 已存在，則更新。
        /// 6. 若 類別 + 鍵值 不存在，則新增。
        /// 7. 圖片原始大小不可超過 10MB。
        ///
        /// Response 範例：
        /// {
        ///   "Code": 200,
        ///   "Result": "上傳圖片參數成功,新增<1>筆,修改<0>筆",
        ///   "Data": {
        ///     "category": "圖片設定",
        ///     "name": "login_logo",
        ///     "key": "login_logo",
        ///     "value_type": "image"
        ///   }
        /// }
        /// </remarks>
        /// <param name="image">圖片檔案。</param>
        /// <param name="key">參數名/鍵值。</param>
        /// <param name="category">類別，預設圖片設定。</param>
        /// <param name="name">顯示名稱，未填則使用 key。</param>
        /// <param name="remark">備註。</param>
        /// <returns>回傳 JSON 字串。</returns>
        [HttpPost("upload_image")]
        public string POST_upload_image(
            [FromForm] IFormFile image,
            [FromForm] string key,
            [FromForm] string category = "圖片設定",
            [FromForm] string name = "",
            [FromForm] string remark = "")
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            returnData returnData = new returnData();
            returnData.Method = "upload_image";
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

            try
            {
                // 呼叫時先初始化資料表，並檢查初始化是否成功
                string initResult = GET_init(returnData);
                returnData initReturnData = initResult.JsonDeserializet<returnData>();

                if (initReturnData != null && initReturnData.Code < 0)
                {
                    initReturnData.Method = "upload_image";
                    initReturnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
                    initReturnData.TimeTaken = myTimerBasic.ToString();
                    return initReturnData.JsonSerializationt(true);
                }

                if (image == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "image 不可為空，請使用 form-data 上傳圖片檔案";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                if (key.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "key 不可為空，請輸入參數名";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                if (category.StringIsEmpty())
                {
                    category = "圖片設定";
                }

                if (name.StringIsEmpty())
                {
                    name = key;
                }

                if (image.Length <= 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "圖片檔案大小不可為 0";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                if (image.Length > MaxFileSizeBytes)
                {
                    returnData.Code = -200;
                    returnData.Result = "圖片大小不可超過 10MB";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                string contentType = image.ContentType ?? "";
                if (contentType.StringIsEmpty() == false && contentType.StartsWith("image/") == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"上傳檔案不是圖片格式, ContentType={contentType}";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                string base64 = "";
                using (MemoryStream ms = new MemoryStream())
                {
                    image.CopyTo(ms);
                    byte[] bytes = ms.ToArray();
                    base64 = Convert.ToBase64String(bytes);
                }

                SQLControl sQLControl_systemConfig = GetSQLControl(returnData);
                if (sQLControl_systemConfig == null)
                {
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                systemConfigClass input = new systemConfigClass();
                input.類別 = category;
                input.鍵值 = key;
                input.名稱 = name;
                input.資料類型 = systemConfigClass.GetValueTypeName(enum_system_config_value_type.image);
                input.文字值 = "";
                input.JSON值 = "";
                input.base64 = base64;
                input.檔案名稱 = "";
                input.副檔名 = "";
                input.ContentType = contentType.StringIsEmpty() ? "image/png" : contentType;
                input.檔案大小 = image.Length.ToString();
                input.版本 = "";
                input.啟用 = "true";
                input.備註 = remark;

                string validation = ValidateSystemConfig(input);
                if (validation.StringIsEmpty() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = validation;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                NormalizeSystemConfig(input);

                List<object[]> rows = GetRowsByCategoryKey(sQLControl_systemConfig, input.類別, input.鍵值);

                List<systemConfigClass> addList = new List<systemConfigClass>();
                List<systemConfigClass> replaceList = new List<systemConfigClass>();

                if (rows.Count == 0)
                {
                    input.GUID = Guid.NewGuid().ToString().ToUpper();
                    input.建立時間 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    input.更新時間 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    addList.Add(input);
                }
                else
                {
                    systemConfigClass oldConfig = rows[0].SQLToClass<systemConfigClass, enum_system_config>();

                    input.GUID = oldConfig.GUID;
                    input.建立時間 = oldConfig.建立時間;
                    input.更新時間 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    replaceList.Add(input);
                }

                List<object[]> list_add = addList.ClassToSQL<systemConfigClass, enum_system_config>();
                List<object[]> list_replace = replaceList.ClassToSQL<systemConfigClass, enum_system_config>();

                if (list_add.Count > 0)
                {
                    sQLControl_systemConfig.AddRows(null, list_add);
                }

                if (list_replace.Count > 0)
                {
                    sQLControl_systemConfig.UpdateByDefulteExtra(null, list_replace);
                }

                returnData.Code = 200;
                returnData.Result = $"上傳圖片參數成功,新增<{list_add.Count}>筆,修改<{list_replace.Count}>筆";
                returnData.Data = input;
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -500;
                returnData.Result = $"內部伺服器錯誤: {ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt(true);
            }
        }
        #endregion

        #region Private - Validate / Normalize

        /// <summary>
        /// 驗證系統參數資料。
        /// </summary>
        /// <param name="input">系統參數物件</param>
        /// <returns>驗證成功回傳空字串，失敗回傳錯誤訊息</returns>
        private string ValidateSystemConfig(systemConfigClass input)
        {
            if (input == null) return "Data 不可為空";

            if (input.類別.StringIsEmpty()) return "缺少必要欄位：category";
            if (input.鍵值.StringIsEmpty()) return "缺少必要欄位：key";
            if (input.資料類型.StringIsEmpty()) return "缺少必要欄位：value_type";

            string valueType = input.資料類型.Trim().ToLower();

            if (systemConfigClass.IsValidValueType(valueType) == false)
            {
                return "資料類型只允許 string / number / bool / json / image / file";
            }

            string jsonType = systemConfigClass.GetValueTypeName(enum_system_config_value_type.json);
            string imageType = systemConfigClass.GetValueTypeName(enum_system_config_value_type.image);
            string fileType = systemConfigClass.GetValueTypeName(enum_system_config_value_type.file);

            if (valueType == imageType || valueType == fileType)
            {
                if (input.base64.StringIsEmpty())
                {
                    return "image / file 類型必須提供 base64";
                }

                string sizeCheck = CheckBase64FileSize(input.base64);
                if (sizeCheck.StringIsEmpty() == false)
                {
                    return sizeCheck;
                }
            }

            if (valueType == jsonType)
            {
                if (input.JSON值.StringIsEmpty())
                {
                    return "json 類型必須提供 json_value";
                }
            }

            return "";
        }

        /// <summary>
        /// 正規化系統參數資料。
        /// </summary>
        /// <param name="input">系統參數物件</param>
        private void NormalizeSystemConfig(systemConfigClass input)
        {
            input.GUID = input.GUID ?? "";
            input.類別 = input.類別 ?? "";
            input.名稱 = input.名稱 ?? "";
            input.鍵值 = input.鍵值 ?? "";
            input.資料類型 = input.資料類型 ?? "";
            input.文字值 = input.文字值 ?? "";
            input.JSON值 = input.JSON值 ?? "";
            input.base64 = input.base64 ?? "";
            input.檔案名稱 = input.檔案名稱 ?? "";
            input.副檔名 = input.副檔名 ?? "";
            input.ContentType = input.ContentType ?? "";
            input.檔案大小 = input.檔案大小 ?? "";
            input.版本 = input.版本 ?? "";
            input.啟用 = input.啟用 ?? "";
            input.備註 = input.備註 ?? "";
            input.建立時間 = input.建立時間 ?? "";
            input.更新時間 = input.更新時間 ?? "";

            input.類別 = input.類別.Trim();
            input.鍵值 = input.鍵值.Trim();
            input.資料類型 = input.資料類型.Trim().ToLower();

            if (input.啟用.StringIsEmpty())
            {
                input.啟用 = "true";
            }

            string imageType = systemConfigClass.GetValueTypeName(enum_system_config_value_type.image);
            string fileType = systemConfigClass.GetValueTypeName(enum_system_config_value_type.file);

            if (input.資料類型 == imageType || input.資料類型 == fileType)
            {
                string pureBase64 = NormalizeBase64(input.base64);

                try
                {
                    byte[] bytes = Convert.FromBase64String(pureBase64);
                    input.檔案大小 = bytes.Length.ToString();
                    input.base64 = pureBase64;
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 檢查 Base64 原始檔案大小。
        /// </summary>
        /// <param name="base64">Base64 字串</param>
        /// <returns>檢查成功回傳空字串，失敗回傳錯誤訊息</returns>
        private string CheckBase64FileSize(string base64)
        {
            try
            {
                if (base64.StringIsEmpty()) return "";

                string pureBase64 = NormalizeBase64(base64);
                byte[] bytes = Convert.FromBase64String(pureBase64);

                if (bytes.Length > MaxFileSizeBytes)
                {
                    return "檔案大小不可超過 10MB";
                }

                return "";
            }
            catch
            {
                return "base64 格式錯誤";
            }
        }

        /// <summary>
        /// 移除 Data URL 前綴，取得純 Base64。
        /// </summary>
        /// <param name="base64">Base64 字串</param>
        /// <returns>純 Base64 字串</returns>
        private string NormalizeBase64(string base64)
        {
            if (base64.StringIsEmpty()) return "";

            string result = base64.Trim();

            int commaIndex = result.IndexOf(",");
            if (result.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
            {
                result = result.Substring(commaIndex + 1);
            }

            return result;
        }

        #endregion

        #region Private - SQLControl / ServerSetting

        /// <summary>
        /// 取得 VM 端 ServerSetting。
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 sys_serverSettingClass，失敗回傳 null</returns>
        private sys_serverSettingClass GetVMServerSetting(returnData returnData)
        {
            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

            if (sys_serverSettingClasses_buf.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = "找無Server資料";
                return null;
            }

            return sys_serverSettingClasses_buf[0];
        }

        /// <summary>
        /// 取得 system_config SQLControl。
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>回傳 SQLControl，失敗回傳 null</returns>
        private SQLControl GetSQLControl(returnData returnData)
        {
            sys_serverSettingClass sys_serverSettingClass_VM = GetVMServerSetting(returnData);

            if (sys_serverSettingClass_VM == null)
            {
                return null;
            }

            string Server = sys_serverSettingClass_VM.Server;
            string DB = sys_serverSettingClass_VM.DBName;
            string UserName = sys_serverSettingClass_VM.User;
            string Password = sys_serverSettingClass_VM.Password;
            uint Port = (uint)sys_serverSettingClass_VM.Port.StringToInt32();

            SQLControl sQLControl_systemConfig = new SQLControl(
                Server,
                DB,
                new enum_system_config().GetEnumDescription(),
                UserName,
                Password,
                Port,
                SSLMode
            );

            return sQLControl_systemConfig;
        }

        /// <summary>
        /// 檢查並建立資料表。
        /// </summary>
        /// <param name="sys_serverSettingClass">ServerSetting</param>
        /// <returns>回傳 Table JSON</returns>
        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_system_config());
            return table.JsonSerializationt(true);
        }

        /// <summary>
        /// 依類別與鍵值取得資料列。
        /// </summary>
        /// <param name="sQLControl_systemConfig">SQLControl</param>
        /// <param name="category">類別</param>
        /// <param name="key">鍵值</param>
        /// <returns>回傳 object[] 清單</returns>
        private List<object[]> GetRowsByCategoryKey(SQLControl sQLControl_systemConfig, string category, string key)
        {
            List<object[]> rows_category = sQLControl_systemConfig.GetRowsByDefult(null, (int)enum_system_config.類別, category);

            rows_category = rows_category
                .Where(row =>
                {
                    string rowKey = row[(int)enum_system_config.鍵值].ObjectToString();
                    return rowKey == key;
                })
                .ToList();

            return rows_category;
        }

        #endregion

        #region Private - ValueAry

        /// <summary>
        /// 從 returnData.ValueAry 取得指定 key 的值。
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <param name="key">參數名稱</param>
        /// <returns>回傳參數值</returns>
        private string GetValue(returnData returnData, string key)
        {
            if (returnData == null) return "";
            if (returnData.ValueAry == null) return "";
            if (key.StringIsEmpty()) return "";

            foreach (string item in returnData.ValueAry)
            {
                if (item.StringIsEmpty()) continue;

                int index = item.IndexOf("=");

                if (index < 0)
                {
                    continue;
                }

                string itemKey = item.Substring(0, index).Trim();
                string itemValue = item.Substring(index + 1);

                if (itemKey.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return itemValue;
                }
            }

            return "";
        }

        #endregion
    }
}