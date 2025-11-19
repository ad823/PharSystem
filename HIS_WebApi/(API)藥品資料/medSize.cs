using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MyUI;
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
    public class medSize : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
         serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
         {
             var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

             if (string.IsNullOrWhiteSpace(Password))
                 throw new SecurityException("Database password cannot be null or empty (medUnit).");

             return (Server, DB, UserName, Password, Port);
         });

        private static string tableName_medSize = "medsize";
        /// <summary>
        ///初始化藥品地圖資料庫
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":[""]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("init")]
        public string init([FromBody] returnData returnData)
        {
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
        /// 新增藥品貨物尺寸與數量資料。
        /// </summary>
        /// <remarks>
        /// 此 API 會將傳入的藥品貨物資訊 (長、寬、高、深、數量) 進行驗證後，新增至資料庫。
        /// 
        /// 傳入格式範例：
        /// <code>
        /// {
        ///   "Data": [
        ///     {
        ///       "GUID": "",
        ///       "code": "00013",
        ///       "length": "20",
        ///       "height": "10",
        ///       "depth": "15",
        ///       "quantity": "50"
        ///     }
        ///   ],
        ///   "Value": "",
        ///   "ValueAry": [],
        ///   "TableName": "",
        ///   "ServerName": "",
        ///   "ServerType": "",
        ///   "TimeTaken": ""
        /// }
        /// </code>
        /// - Data 可為單筆物件或陣列，必填欄位：
        ///   - code (藥碼)
        ///   - length (貨品長)
        ///   - height (貨品高)
        ///   - depth (貨品深)
        ///   - quantity (數量)
        /// - 系統會自動產生 GUID。
        /// - 若任一必填欄位為空，該筆資料將略過不新增。
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構，包含 Data 與額外控制參數</param>
        /// <returns>
        /// JSON 字串，內容包含：
        /// - Code：200 代表成功，-200 代表失敗  
        /// - Result：執行結果描述  
        /// - Data：實際新增成功的資料清單  
        /// - TimeTaken：耗時  
        /// </returns>
        ///  <returns>
        /// JSON 字串，內容包含：
        /// - Code：200 代表成功，-200 代表失敗  
        /// - Result：執行結果描述  
        /// - Data：實際新增成功的資料清單  
        /// - TimeTaken：耗時  
        ///
        /// 成功回傳範例：
        /// <code>
        /// {
        ///   "Code": 200,
        ///   "Result": "藥品貨物資訊新增<1>筆成功",
        ///   "Data": [
        ///     {
        ///       "GUID": "c2a34e0d-5dbb-4c5d-8b07-1a9c9f2a7e4b",
        ///       "code": "00013",
        ///       "length": "20",
        ///       "height": "10",
        ///       "depth": "15",
        ///       "quantity": "50"
        ///     }
        ///   ],
        ///   "TimeTaken": "00:00:00.123"
        /// }
        /// </code>
        ///
        /// 失敗回傳範例：
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "returnData.Data不得為空!",
        ///   "Data": null,
        ///   "TimeTaken": "00:00:00.005"
        /// }
        /// </code>
        /// </returns>
        [HttpPost("add")]
        public async Task<string> add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if(returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data不得為空!";
                    return returnData.JsonSerializationt(true);
                }
                List<medSizeClass> medSizeClasses = returnData.Data.ObjToClass<List<medSizeClass>>();
                if(medSizeClasses == null)
                {
                    medSizeClasses = new List<medSizeClass>();
                    medSizeClass medSizeClass = returnData.Data.ObjToClass<medSizeClass>();
                    if (medSizeClass == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data不得為空!";
                        return returnData.JsonSerializationt(true);
                    }
                    medSizeClasses.Add(medSizeClass);
                }
                List<medSizeClass> add_medSizeClass = new List<medSizeClass>();
                foreach (medSizeClass medSizeClass in medSizeClasses)
                {
                    if (medSizeClass.藥碼.StringIsEmpty()) continue;
                    if (medSizeClass.貨品長.StringIsEmpty()) continue;
                    if (medSizeClass.貨品高.StringIsEmpty()) continue;
                    if (medSizeClass.貨品深.StringIsEmpty()) continue;
                    if (medSizeClass.數量.StringIsEmpty()) continue;
                    medSizeClass.GUID = Guid.NewGuid().ToString();
                    add_medSizeClass.Add(medSizeClass);
                }
                if(add_medSizeClass.Count > 0)
                {

                    (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                    SQLControl sQLControl = new SQLControl(Server, DB, tableName_medSize, UserName, Password, Port, SSLMode);
                    List<object[]> add = add_medSizeClass.ClassToSQL<medSizeClass, enum_medSize>();
                    await sQLControl.AddRowsAsync(null, add);
                }
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = add_medSizeClass;
                returnData.Result = $"藥品貨物資訊新增<{add_medSizeClass.Count}>筆成功";
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
        /// 更新藥品貨物尺寸與數量資料（Partial Update）。
        /// </summary>
        /// <remarks>
        /// 本 API 會：
        /// 1. 解析 <c>returnData.Data</c>（可為單筆或多筆）。  
        /// 2. 批次取回既有資料後，依 <c>GUID</c> 對應到原資料。  
        /// 3. 僅覆寫「有提供值」的欄位（未提供的欄位不變），將更新後的資料回寫資料庫。  
        ///
        /// ✅ 必填條件：每筆資料需包含 <c>GUID</c>；否則將略過不處理。  
        /// ✅ 部分更新：僅覆寫有值的欄位（例如：<c>code/藥碼</c>、<c>length/貨品長</c>、<c>height/貨品高</c>、<c>depth/貨品深</c>、<c>quantity/數量</c>）。  
        ///
        /// 🔄 請求範例（單筆）：
        /// <code>
        /// {
        ///   "Data": {
        ///     "GUID": "e2f7e0c9-8ab1-4e2b-9d12-2e0a0e9f1a23",
        ///     "code": "00013",
        ///     "length": "22",
        ///     "height": "11",
        ///     "depth": "15",
        ///     "quantity": "60"
        ///   },
        ///   "Value": "",
        ///   "ValueAry": [],
        ///   "TableName": "med_size",
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "TimeTaken": ""
        /// }
        /// </code>
        ///
        /// 🔄 請求範例（多筆，含略過情境）：
        /// <code>
        /// {
        ///   "Data": [
        ///     {
        ///       "GUID": "c2a34e0d-5dbb-4c5d-8b07-1a9c9f2a7e4b",
        ///       "length": "25"     // 只改長度，其餘不變
        ///     },
        ///     {
        ///       "GUID": "",        // 會被略過：缺 GUID
        ///       "code": "00020"
        ///     },
        ///     {
        ///       "GUID": "a1b2c3d4-e5f6-7890-ab12-34567890abcd",
        ///       "quantity": "80"   // 只改數量
        ///     }
        ///   ],
        ///   "Value": "",
        ///   "ValueAry": [],
        ///   "TableName": "med_size",
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "TimeTaken": ""
        /// }
        /// </code>
        ///
        /// 📌 欄位對應說明：
        /// - 英文鍵名（<c>code/length/height/depth/quantity</c>）對應到類別上的中文屬性（<c>藥碼/貨品長/貨品高/貨品深/數量</c>），
        ///   實際綁定依你類別上的 <c>[JsonPropertyName]</c> 設定為準。
        ///
        /// 🟩 成功回傳範例：
        /// <code>
        /// {
        ///   "Code": 200,
        ///   "Result": "藥品貨物資訊更新&lt;2&gt;筆成功",
        ///   "Data": [
        ///     {
        ///       "GUID": "c2a34e0d-5dbb-4c5d-8b07-1a9c9f2a7e4b",
        ///       "code": "00013",
        ///       "length": "25",
        ///       "height": "11",
        ///       "depth": "15",
        ///       "quantity": "60"
        ///     },
        ///     {
        ///       "GUID": "a1b2c3d4-e5f6-7890-ab12-34567890abcd",
        ///       "code": "00021",
        ///       "length": "20",
        ///       "height": "10",
        ///       "depth": "15",
        ///       "quantity": "80"
        ///     }
        ///   ],
        ///   "TimeTaken": "00:00:00.123"
        /// }
        /// </code>
        ///
        /// 🟥 失敗回傳範例（Data 為空）：
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "returnData.Data不得為空!",
        ///   "Data": null,
        ///   "TimeTaken": "00:00:00.004"
        /// }
        /// </code>
        ///
        /// 📝 備註：
        /// - 本 API 僅對「查到舊資料且具 <c>GUID</c> 對應成功」之筆數進行更新並計數；
        ///   缺少 <c>GUID</c> 或查無對應資料者將略過，不影響其他筆更新。  
        /// - 回傳的 <c>Data</c> 為實際送交資料庫更新的資料集合（更新後的內容）。  
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構（含 Data/Value/ValueAry 等）。</param>
        /// <returns>
        /// JSON 字串：
        /// - <c>Code</c>：200 成功、-200 失敗  
        /// - <c>Result</c>：執行結果描述（包含成功更新筆數）  
        /// - <c>Data</c>：實際送交更新之資料清單  
        /// - <c>TimeTaken</c>：執行耗時  
        /// </returns>
        [HttpPost("update")]
        public async Task<string> update([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data不得為空!";
                    return returnData.JsonSerializationt(true);
                }

                List<medSizeClass> medSizeClasses = returnData.Data.ObjToClass<List<medSizeClass>>();
                if (medSizeClasses == null)
                {
                    medSizeClasses = new List<medSizeClass>();
                    medSizeClass medSizeClass = returnData.Data.ObjToClass<medSizeClass>();
                    if (medSizeClass == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data不得為空!";
                        return returnData.JsonSerializationt(true);
                    }
                    medSizeClasses.Add(medSizeClass);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, tableName_medSize, UserName, Password, Port, SSLMode);
                string[] codes = medSizeClasses.Select(x => x.GUID).ToArray();
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medSize.藥碼, codes);
                List<medSizeClass> medSizes = objects.SQLToClass<medSizeClass, enum_medSize>();

                List<medSizeClass> update_medSizeClass = new List<medSizeClass>();
                foreach (medSizeClass medSizeClass in medSizeClasses)
                {
                    medSizeClass old_medSizeClass = medSizes.FirstOrDefault(x => x.GUID == medSizeClass.GUID);
                    if (old_medSizeClass == null) continue;
                    if (medSizeClass.藥碼.StringIsEmpty() == false) old_medSizeClass.藥碼 = medSizeClass.藥碼;
                    if (medSizeClass.貨品長.StringIsEmpty() == false) old_medSizeClass.貨品長 = medSizeClass.貨品長;
                    if (medSizeClass.貨品高.StringIsEmpty() == false) old_medSizeClass.貨品高 = medSizeClass.貨品高;
                    if (medSizeClass.貨品深.StringIsEmpty() == false) old_medSizeClass.貨品深 = medSizeClass.貨品深;
                    if (medSizeClass.數量.StringIsEmpty() == false) old_medSizeClass.數量 = medSizeClass.數量;
                    update_medSizeClass.Add(old_medSizeClass);
                }

                if (update_medSizeClass.Count > 0)
                {
                    List<object[]> updateData = update_medSizeClass.ClassToSQL<medSizeClass, enum_medSize>();
                    await sQLControl.UpdateRowsAsync(null, updateData);
                }

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = update_medSizeClass;
                returnData.Result = $"藥品貨物資訊更新<{update_medSizeClass.Count}>筆成功";
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
        /// 取得所有藥品貨物尺寸與數量資料。
        /// </summary>
        /// <remarks>
        /// 本 API 會：
        /// 1. 從資料表 <c>tableName_medSize</c> 取出全部資料。  
        /// 2. 依藥碼 (<c>code/藥碼</c>) 進行排序後回傳。  
        ///
        /// 🔄 請求範例：
        /// <code>
        /// {
        ///   "Data": null,
        ///   "Value": "",
        ///   "ValueAry": [],
        ///   "TableName": "med_size",
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "TimeTaken": ""
        /// }
        /// </code>
        ///
        /// 🟩 成功回傳範例：
        /// <code>
        /// {
        ///   "Code": 200,
        ///   "Result": "取得藥品貨物資訊&lt;3&gt;筆成功",
        ///   "Data": [
        ///     {
        ///       "GUID": "11111111-1111-1111-1111-111111111111",
        ///       "code": "00013",
        ///       "length": "20",
        ///       "height": "10",
        ///       "depth": "15",
        ///       "quantity": "50"
        ///     },
        ///     {
        ///       "GUID": "22222222-2222-2222-2222-222222222222",
        ///       "code": "00014",
        ///       "length": "25",
        ///       "height": "12",
        ///       "depth": "18",
        ///       "quantity": "30"
        ///     },
        ///     {
        ///       "GUID": "33333333-3333-3333-3333-333333333333",
        ///       "code": "00015",
        ///       "length": "10",
        ///       "height": "8",
        ///       "depth": "6",
        ///       "quantity": "100"
        ///     }
        ///   ],
        ///   "TimeTaken": "00:00:00.089"
        /// }
        /// </code>
        ///
        /// 🟥 失敗回傳範例：
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "資料庫連線失敗",
        ///   "Data": null,
        ///   "TimeTaken": "00:00:00.003"
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構（Data 可為 null）。</param>
        /// <returns>
        /// JSON 字串：
        /// - <c>Code</c>：200 成功、-200 失敗  
        /// - <c>Result</c>：執行結果描述（含總筆數）  
        /// - <c>Data</c>：依藥碼排序後的完整清單  
        /// - <c>TimeTaken</c>：執行耗時  
        /// </returns>
        [HttpPost("get_all")]
        public async Task<string> get_all([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, tableName_medSize, UserName, Password, Port, SSLMode);

                // 取出所有資料
                List<object[]> rows = await sQLControl.GetAllRowsAsync(null);
                List<medSizeClass> medSizeClasses = rows.SQLToClass<medSizeClass, enum_medSize>();

                // 依藥碼排序
                medSizeClasses = medSizeClasses
                    .OrderBy(m => m.藥碼)
                    .ToList();

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medSizeClasses;
                returnData.Result = $"取得藥品貨物資訊<{medSizeClasses.Count}>筆成功";
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
        /// 依藥碼批次查詢 <c>med_size</c>（<c>tableName_medSize</c>）的尺寸與數量資料，回傳 English 欄位格式的清單。
        /// </summary>
        /// <remarks>
        /// 📌 使用方式
        /// - 路由：<c>POST api/.../get_by_code</c>
        /// - 參數：於 <c>returnData.ValueAry[0]</c> 放入以分號 <c>;</c> 分隔的藥碼字串（伺服端會 Distinct）
        /// - 結果：依藥碼升冪排序回傳
        ///
        /// 📥 請求 Body 範例
        /// <code>
        /// {
        ///   "Data": null,
        ///   "Value": "",
        ///   "ValueAry": [ "EPAR;AB123;ZX999" ],
        /// }
        /// </code>
        ///
        /// 📤 成功回應（200）範例
        /// <code>
        /// {
        ///   "Data": [
        ///     {
        ///       "GUID": "d165706f-eb88-4abd-be3e-bf5026e8db23",
        ///       "code": "EPAR",
        ///       "length": "35",
        ///       "height": "20",
        ///       "depth": "35",
        ///       "quantity": "30"
        ///     }
        ///   ],
        ///   "Code": 200,
        ///   "Method": "",
        ///   "Result": "依藥碼查詢&lt;1&gt;筆成功",
        ///   "Value": "",
        ///   "ValueAry": [ "EPAR" ],
        ///   "TimeTaken": "72.182ms"
        /// }
        /// </code>
        ///
        /// ⚠️ 失敗回應
        /// - 參數錯誤（<c>ValueAry</c> 為 null 或長度不為 1）
        /// <code>{ "Code": -200, "Result": "請提供至少一個藥碼(code)!", "Data": null }</code>
        /// - 無有效藥碼（分割後長度為 0）
        /// <code>{ "Code": -200, "Result": "請提供至少一個藥碼(code)!", "Data": null }</code>
        /// - 系統例外
        /// <code>{ "Code": -200, "Result": "(Exception.Message)", "Data": null }</code>
        ///
        /// 🔎 邏輯重點
        /// - 僅接受 <c>ValueAry.Count == 1</c>，以 <c>;</c> 分隔多碼；伺服端會 <c>Distinct()</c>
        /// - 查詢欄位：<c>enum_medSize.藥碼</c>，回傳欄位為 English 命名（<c>code/length/height/depth/quantity</c>）
        /// - 回傳 <c>Data</c> 為 <c>List&lt;medSizeClass&gt;</c>（其屬性序列化為上述英文字段）
        ///
        /// 📝 前端整合建議
        /// - 以分號組字串：<c>"CODE1;CODE2;CODE3"</c> 放入 <c>ValueAry[0]</c>
        /// - 欄位皆為字串型別（例如長、寬、高、數量）—若要運算請在前端轉型
        /// - 若需標示查無資料的藥碼，可用原輸入清單對回傳的 <c>code</c> 清單比對
        /// </remarks>
        /// <param name="returnData">
        /// 於 <c>ValueAry[0]</c> 傳入以 <c>;</c> 分隔的藥碼清單；<c>Data</c> 不需使用。
        /// </param>
        /// <returns>標準回傳模型 JSON 字串（含 <c>Code</c>、<c>Result</c>、<c>Data</c>、<c>TimeTaken</c>）。</returns>

        [HttpPost("get_by_code")]
        public async Task<string> get_by_code([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                // 收集要查詢的藥碼              
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = "請提供至少一個藥碼(code)!";
                    returnData.Data = null;
                    return await returnData.JsonSerializationtAsync(true);
                }


                string[] codes = returnData.ValueAry[0].Split(";").Distinct().ToArray();

                if (codes.Length == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = $"{myTimerBasic}";
                    returnData.Result = "請提供至少一個藥碼(code)!";
                    returnData.Data = null;
                    return await returnData.JsonSerializationtAsync(true);
                }

                // 連線資訊
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, tableName_medSize, UserName, Password, Port, SSLMode);

                // 查詢
                List<object[]> rows = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medSize.藥碼, codes.ToArray());
                List<medSizeClass> medSizeClasses = rows.SQLToClass<medSizeClass, enum_medSize>();

                // 排序（依藥碼）
                medSizeClasses = medSizeClasses.OrderBy(m => m.藥碼).ToList();

                // 回傳
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medSizeClasses;
                returnData.Result = $"依藥碼查詢<{medSizeClasses.Count}>筆成功";
                return await returnData.JsonSerializationtAsync(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
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
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medSize()));
            return tables.JsonSerializationt(true);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_by_code(string guid)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(guid);

            string result = await get_by_code(returnData);
            return result.JsonDeserializet<returnData>();
        }
    }
}
