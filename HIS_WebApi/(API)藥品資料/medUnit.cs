using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MyUI;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using SQLUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class medUnit : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private string APIServer = Method.GetServerAPI("Main", "網頁", "API01");
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
           serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
           {
               var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

               if (string.IsNullOrWhiteSpace(Password))
                   throw new SecurityException("Database password cannot be null or empty (medUnit).");

               return (Server, DB, UserName, Password, Port);
           });

        private static string tableName = "medunit";
        [HttpPost("init")]
        public string init()
        {
            returnData returnData = new returnData();
            try
            {
                return CheckCreatTable();
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"{ex.Message}";
                return returnData.JsonSerializationt();
            }
        }
        /// <summary>
        /// 新增或更新「藥品單位」資料（支援單筆或多筆）。
        /// </summary>
        /// <remarks>
        /// 此 API 會將傳入的 <c>medUnitClass</c> 清單依 <c>Med_GUID</c> 分組後：
        /// 1) 以同組內「<c>單位類型</c>」為唯一鍵與資料庫現有資料比對。<br/>
        /// 2) 若該 <c>Med_GUID</c> 下不存在相同「<c>單位類型</c>」→ 新增（自動補 <c>GUID</c>）。<br/>
        /// 3) 若存在相同「<c>單位類型</c>」→ 覆蓋更新（<c>單位類型</c>、<c>單位名稱</c>、<c>換算數量</c>、<c>排序順序</c>）。<br/>
        ///
        /// ✅ 支援傳入單筆物件或物件陣列，方法內會自動轉為清單處理。<br/>
        /// ✅ 僅處理 <c>Med_GUID</c> 非空的資料。<br/>
        /// ✅ 以 <c>Med_GUID + 單位類型</c> 作為唯一比對鍵（若需求改變，請同步調整邏輯與資料庫唯一索引）。<br/>
        ///
        /// <para>
        /// <b>必要欄位規格</b><br/>
        /// - <c>Med_GUID</c>：不可為空。<br/>
        /// - <c>單位類型</c>：不可為空（例如：採購、撥補、調劑）。<br/>
        /// - <c>單位名稱</c>：不可為空（例如：箱、盒、顆）。<br/>
        /// - <c>換算數量</c>：可空（與上層單位換算量，字串型別，允許 null/空字串）。<br/>
        /// - <c>排序順序</c>：可空（字串型別；如需嚴格整數可在程式內驗證）。<br/>
        /// - <c>GUID</c>：新增時若未提供會自動產生，更新時以資料庫既有主鍵為準。<br/>
        /// </para>
        ///
        /// <para>
        /// <b>安全/效能注意事項</b><br/>
        /// - 已將輸入資料先過濾 <c>Med_GUID</c>，但查詢資料庫現況時，建議優先使用 <c>GetRowsByDefultAsync</c>（吃陣列參數）替代手寫 SQL 並避免字串串接造成 SQL Injection 風險。<br/>
        /// - 更新行為為「覆蓋式」，若要避免把既有值被空字串清掉，請在指派前加上非空判斷。<br/>
        /// </para>
        ///
        /// <example>
        /// <code>
        /// POST /api/medUnit/add
        /// Content-Type: application/json
        ///
        /// // ✅ 多筆請求範例
        /// {
        ///   "Data": [
        ///     {
        ///       "guid": null,
        ///       "med_guid": "M001",
        ///       "unit_type": "採購",
        ///       "unit_name": "箱",
        ///       "quantity_per_parent": null,
        ///       "sort_order": "1"
        ///     },
        ///     {
        ///       "guid": null,
        ///       "med_guid": "M001",
        ///       "unit_type": "撥補",
        ///       "unit_name": "盒",
        ///       "quantity_per_parent": "10",
        ///       "sort_order": "2"
        ///     },
        ///     {
        ///       "guid": null,
        ///       "med_guid": "M001",
        ///       "unit_type": "調劑",
        ///       "unit_name": "顆",
        ///       "quantity_per_parent": "10",
        ///       "sort_order": "3"
        ///     }
        ///   ],
        ///   "Value": "",
        ///   "ValueAry": [],
        ///   "TableName": "med_unit",
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "TimeTaken": ""
        /// }
        /// </code>
        ///
        /// <code>
        /// // ✅ 單筆請求亦可
        /// {
        ///   "Data": {
        ///     "guid": null,
        ///     "med_guid": "M002",
        ///     "unit_type": "撥補",
        ///     "unit_name": "盒",
        ///     "quantity_per_parent": "5",
        ///     "sort_order": "10"
        ///   }
        /// }
        /// </code>
        /// <code>
        /// // 🟥 失敗回傳（格式錯誤或 Data 為空）
        /// {
        ///   "Code": -200,
        ///   "Result": "returnData.Data格式錯誤或不得為空!",
        ///   "Data": null,
        ///   "TimeTaken": "3.012ms",
        ///   "Method": "add"
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="returnData">
        /// 請求包裝物件。<br/>
        /// - <c>Data</c>：<c>medUnitClass</c> 或 <c>List&lt;medUnitClass&gt;</c>。<br/>
        /// - 其他欄位（<c>ServerName</c>、<c>ServerType</c>、<c>TableName</c> 等）依你平台慣例傳入。<br/>
        /// </param>
        /// <returns>
        /// 成功時回傳 <c>Code = 200</c>，<c>Result</c> 會帶出新增/更新筆數，<c>Data</c> 內容為：<br/>
        /// <c>{ add = List&lt;medUnitClass&gt;, update = List&lt;medUnitClass&gt; }</c>。<br/>
        /// 失敗時回傳 <c>Code = -200</c> 與錯誤訊息。<br/>
        /// </returns>

        [HttpPost("add")]
        [HttpPost("update")]
        public async Task<string> add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                // 1) 檢查 Data
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data不得為空!";
                    return returnData.JsonSerializationt(true);
                }

                // 2) 嘗試解析為 List<medUnitClass>，否則嘗試解析單筆並包裝為 List
                List<medUnitClass> unitList = returnData.Data.ObjToClass<List<medUnitClass>>();
                if (unitList == null)
                {
                    unitList = new List<medUnitClass>();
                    medUnitClass single = returnData.Data.ObjToClass<medUnitClass>();
                    if (single == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = "returnData.Data格式錯誤或不得為空!";
                        return returnData.JsonSerializationt(true);
                    }
                    unitList.Add(single);
                }
                unitList = unitList.Where(x => x.Med_GUID.StringIsEmpty() == false).ToList();
                string[] med_guids = unitList.Select(x => x.Med_GUID).Distinct().ToArray();
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                string command = $"SELECT * FROM {tableName} WHERE Med_GUID IN ('{string.Join("','", med_guids)}')";
                List<object[]> objects = await sQLControl.WriteCommandAsync(command);
                List<medUnitClass> medUnitClasses = objects.SQLToClass<medUnitClass>();
                Dictionary<string, List<medUnitClass>> dic = medUnitClasses.ToDictByMedGuid();
                Dictionary<string, List<medUnitClass>> dic_add = unitList.ToDictByMedGuid();

                // 3) 欄位檢核與資料整備
                List<medUnitClass> add_units = new List<medUnitClass>();
                List<medUnitClass> update_units = new List<medUnitClass>();

                foreach (var key in dic_add.Keys)
                {
                    List<medUnitClass> medUnits = dic_add.GetByMasterGUID(key);
                    List<medUnitClass> medUnits_db = dic.GetByMasterGUID(key);
                    medUnitClass medUnits_buff = new medUnitClass();
                    for (int i = 0; i < medUnits.Count; i++)
                    {
                        if (medUnits[i].單位類型.StringIsEmpty() || medUnits[i].單位名稱.StringIsEmpty()) continue;
                        medUnits_buff = medUnits_db.FirstOrDefault (x => x.單位類型 == medUnits[i].單位類型);
                        if (medUnits_buff == null)
                        {
                            medUnits[i].GUID = Guid.NewGuid().ToString();
                            add_units.Add(medUnits[i]);
                        }
                        else
                        {                            
                            medUnits_buff.單位類型 = medUnits[i].單位類型;
                            medUnits_buff.單位名稱 = medUnits[i].單位名稱;
                            medUnits_buff.數量 = medUnits[i].數量;
                            medUnits_buff.排序順序 = medUnits[i].排序順序;
                            update_units.Add(medUnits_buff);

                        }
                    }
                }
                List<object[]> add_rows = add_units.ClassToSQL<medUnitClass>();
                List<object[]> update_rows = update_units.ClassToSQL<medUnitClass>();

                // 4) 進資料庫
                if (add_units.Count > 0) await sQLControl.AddRowsAsync(null, add_rows);
                if (update_rows.Count > 0) await sQLControl.UpdateRowsAsync(null, update_rows);



                // 5) 統一回傳
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = add_units;
                returnData.Result = $"藥品單位資訊新增<{add_units.Count}>筆，更新<{update_units.Count}>筆";
                return await returnData.JsonSerializationtAsync(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 依 <c>Med_GUID</c> 查詢藥品單位資訊。
        /// </summary>
        /// <remarks>
        /// 此 API 會：
        /// 1. 從傳入的 <c>returnData.ValueAry</c> 取得欲查詢的 <c>Med_GUID</c>。  
        /// 2. 若傳入多筆以分號 (<c>;</c>) 分隔，可一次查詢多個 <c>Med_GUID</c>。  
        /// 3. 回傳符合條件的所有藥品單位資料。  
        ///
        /// <example>
        /// <code>
        /// POST /api/medUnit/get_by_Med_GUID
        /// {
        ///   "ValueAry": [ "M001;M002" ],
        ///   "TableName": "med_unit",
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁"
        /// }
        /// </code>
        ///
        /// 🟩 成功回傳範例：
        /// <code>
        /// {
        ///   "Code": 200,
        ///   "Result": "依Med_GUID查詢&lt;3&gt;筆成功",
        ///   "Data": [
        ///     {
        ///       "GUID": "U001",
        ///       "Med_GUID": "M001",
        ///       "單位類型": "採購",
        ///       "單位名稱": "箱",
        ///       "換算數量": null,
        ///       "排序順序": "1"
        ///     },
        ///     {
        ///       "GUID": "U002",
        ///       "Med_GUID": "M001",
        ///       "單位類型": "撥補",
        ///       "單位名稱": "盒",
        ///       "換算數量": "10",
        ///       "排序順序": "2"
        ///     }
        ///   ]
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        [HttpPost("get_by_Med_GUID")]
        public async Task<string> get_by_Med_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                // 1️⃣ 檢查輸入
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = "請提供至少一個 Med_GUID!";
                    returnData.Data = null;
                    return await returnData.JsonSerializationtAsync(true);
                }

                // 2️⃣ 解析 Med_GUID（可多筆以分號分隔）
                string[] med_guids = returnData.ValueAry[0].Split(";").Distinct().ToArray();
                if (med_guids.Length == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = $"{myTimerBasic}";
                    returnData.Result = "請提供至少一個 Med_GUID!";
                    returnData.Data = null;
                    return await returnData.JsonSerializationtAsync(true);
                }

                // 3️⃣ 取得連線資訊
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);

                // 4️⃣ 查詢資料
                List<object[]> rows = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medUnit.Med_GUID, med_guids);
                List<medUnitClass> medUnitClasses = rows.SQLToClass<medUnitClass>();

                // 5️⃣ 排序（依 Med_GUID、排序順序）
                medUnitClasses = medUnitClasses
                    .OrderBy(m => m.Med_GUID)
                    .ThenBy(m => m.排序順序)
                    .ToList();

                // 6️⃣ 組回傳
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medUnitClasses;
                returnData.Result = $"依Med_GUID查詢<{medUnitClasses.Count}>筆成功";
                return await returnData.JsonSerializationtAsync(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 依藥碼 (<c>code</c>) 查詢藥品單位資訊。
        /// </summary>
        /// <remarks>
        /// 此 API 為「兩階段查詢」：  
        /// 1️⃣ 先透過 <c>MED_pageController.get_by_code()</c> 查詢藥品主檔資料，取得該藥品的 <c>Med_GUID</c>。<br/>
        /// 2️⃣ 再呼叫 <c>get_by_Med_GUID()</c> 查詢該藥品的所有單位設定資料（例如採購單位、撥補單位、調劑單位等）。  
        ///
        /// ✅ 若藥碼不存在或查無資料，會回傳 Code = -200 並顯示「查無藥品資料!」。<br/>
        /// ✅ 成功時回傳對應藥品的所有單位設定列表。  
        ///
        /// <para>
        /// <b>邏輯流程摘要</b><br/>
        /// - 驗證 <c>returnData.ValueAry</c> 是否包含藥碼（需為 1 筆）。<br/>
        /// - 呼叫 <c>MED_pageController.get_by_code()</c> 取得 <c>Med_GUID</c>。<br/>
        /// - 若成功，呼叫 <c>get_by_Med_GUID(med_guid)</c> 查出所有單位資料。<br/>
        /// - 結果包含多筆 <c>medUnitClass</c>，依 <c>Med_GUID</c> 與 <c>排序順序</c> 排序。<br/>
        /// </para>
        ///
        /// <para>
        /// <b>輸入格式</b><br/>
        /// - <c>ValueAry</c>：需包含 1 筆藥碼字串，例如 ["EPAR"]。<br/>
        /// - 其他欄位（<c>TableName</c>、<c>ServerName</c>、<c>ServerType</c>）可依平台預設帶入。<br/>
        /// </para>
        ///
        /// <para>
        /// <b>回傳格式</b><br/>
        /// - <c>Code</c>：200 表示成功，-200 表示失敗。<br/>
        /// - <c>Data</c>：為 <c>List&lt;medUnitClass&gt;</c>，每筆包含：<br/>
        /// &nbsp;&nbsp;• <c>GUID</c> — 單位識別碼<br/>
        /// &nbsp;&nbsp;• <c>Med_GUID</c> — 對應藥品 GUID<br/>
        /// &nbsp;&nbsp;• <c>單位類型</c> — 採購 / 撥補 / 調劑<br/>
        /// &nbsp;&nbsp;• <c>單位名稱</c> — 箱 / 盒 / 顆<br/>
        /// &nbsp;&nbsp;• <c>換算數量</c> — 與上層單位換算數量<br/>
        /// &nbsp;&nbsp;• <c>排序順序</c> — 顯示順序（越小越前）<br/>
        /// </para>
        ///
        /// <example>
        /// <code>
        /// POST /api/medUnit/get_by_code
        /// Content-Type: application/json
        ///
        /// {
        ///   "ValueAry": [ "EPAR" ],
        ///   "TableName": "med_unit",
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁"
        /// }
        /// </code>
        ///
        /// 🟥 錯誤回傳範例（查無藥品）：
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "查無藥品資料!",
        ///   "Data": null,
        ///   "Method": "get_by_code",
        ///   "TimeTaken": "25.491ms"
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="returnData">
        /// 請求包裝物件。  
        /// - <c>ValueAry</c>：藥碼清單（只取第一筆）。  
        /// - <c>TableName</c>、<c>ServerName</c>、<c>ServerType</c> 為一般系統參數。  
        /// </param>
        /// <returns>
        /// 回傳 <c>returnData</c> 格式字串：  
        /// - 成功：<c>Code = 200</c>，附帶 <c>List&lt;medUnitClass&gt;</c>。  
        /// - 失敗：<c>Code = -200</c> 與錯誤訊息（如「查無藥品資料!」、「請提供至少一個藥碼!」等）。  
        /// </returns>
        [HttpPost("get_by_code")]
        public async Task<string> get_by_code([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                // 1️⃣ 檢查輸入
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = "請提供至少一個 藥碼!";
                    returnData.Data = null;
                    return await returnData.JsonSerializationtAsync(true);
                }
                string code = returnData.ValueAry[0];
                returnData returnData_med_page = await new MED_pageController().get_by_code(code);
                if (returnData_med_page == null || returnData_med_page.Code != 200)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = $"{myTimerBasic}";
                    returnData.Result = "查無藥品資料!";
                    returnData.Data = null;
                    return await returnData.JsonSerializationtAsync(true);
                }
                List<medClass> medPageClasses = returnData_med_page.Data.ObjToClass<List<medClass>>();
                if (medPageClasses == null || medPageClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = $"{myTimerBasic}";
                    returnData.Result = "查無藥品資料!";
                    returnData.Data = null;
                    return await returnData.JsonSerializationtAsync(true);
                }
                string med_guid = medPageClasses[0].GUID;
                returnData returnData_medUnit = await get_by_Med_GUID(med_guid);
                returnData_medUnit.Method = "get_by_code";
                returnData_medUnit.TimeTaken = $"{myTimerBasic}";
                return await returnData_medUnit.JsonSerializationtAsync(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return await returnData.JsonSerializationtAsync(true);
            }
            
        }


        private string CheckCreatTable()
        {
            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
            if (sys_serverSettingClasses.Count == 0)
            {
                returnData returnData = new returnData();
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return returnData.JsonSerializationt();
            }

            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable<medUnitClass>(sys_serverSettingClasses[0]));
            return tables.JsonSerializationt(true);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<returnData> get_by_Med_GUID(string med_guid)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(med_guid);

            string result = await get_by_Med_GUID(returnData);
            return result.JsonDeserializet<returnData>();
        }
    }
}
