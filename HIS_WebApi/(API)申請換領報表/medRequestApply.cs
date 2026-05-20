using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using MyUI;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class medRequestApply : Controller
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        /// <summary>
        /// 初始化資料庫
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data":
        ///     {
        ///
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("init")]
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(medRequestApply))]
        [HttpPost]
        public string GET_init([FromBody] returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "申請換領報表");
                }

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                return CheckCreatTable(sys_serverSettingClasses[0]);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 批量新增申請換領報表
        /// </summary>
        /// <remarks>
        /// <para><b>功能說明</b></para>
        /// 批量新增申請換領報表資料
        ///
        /// <para><b>請求 JSON 範例</b></para>
        /// <code>
        /// {
        ///   "Data": [
        ///     {
        ///       "cGuid": "guid-value",
        ///       "cOrderlistGuid": "order-guid",
        ///       "cRequestNo": "REQ202605150000001",
        ///       "cCreatBy": "藥師名稱",
        ///       "cCreatAt": "2026-05-15 10:30:00"
        ///     }
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        [Route("add_medrequestapply")]
        [HttpPost]
        public string add_medrequestapply([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "add_batch";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "medRequestApply";

                SQLControl sQLControl = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                List<HIS_DB_Lib.medRequestApply> applications = returnData.Data.ObjToClass<List<HIS_DB_Lib.medRequestApply>>();

                if (applications == null || applications.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無申請單資料";
                    return returnData.JsonSerializationt();
                }

                // 初始化 order_list 表的 SQLControl 用於查詢單次劑量
                SQLControl orderListSQLControl = new SQLControl(Server, DB, "order_list", UserName, Password, Port, SSLMode);

                // 初始化 person_page 表的 SQLControl 用於查詢正確的用戶 GUID
                SQLControl personPageSQLControl = new SQLControl(Server, DB, "person_page", UserName, Password, Port, SSLMode);

                // 從前端請求中提取領藥人信息和建立人信息
                string pickupPersonGuid = "";
                string pickupPersonName = "";
                string pickupPersonId = "";
                Dictionary<int, string> creatByFromJson = new Dictionary<int, string>();

                try
                {
                    // 將 returnData.Data 轉為 JsonElement 以提取原始值
                    if (returnData.Data is System.Text.Json.JsonElement jsonData && jsonData.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        int arrayLength = jsonData.GetArrayLength();
                        for (int idx = 0; idx < arrayLength; idx++)
                        {
                            var item = jsonData[idx];

                            if (idx == 0)
                            {
                                if (item.TryGetProperty("pickupPersonGuid", out var guidProp) && guidProp.ValueKind != System.Text.Json.JsonValueKind.Null)
                                    pickupPersonGuid = guidProp.GetString() ?? "";
                                if (item.TryGetProperty("pickupPersonName", out var nameProp) && nameProp.ValueKind != System.Text.Json.JsonValueKind.Null)
                                    pickupPersonName = nameProp.GetString() ?? "";
                                if (item.TryGetProperty("pickupPersonId", out var idProp) && idProp.ValueKind != System.Text.Json.JsonValueKind.Null)
                                    pickupPersonId = idProp.GetString() ?? "";
                            }

                            if (item.TryGetProperty("CreatBy", out var creatByProp) && creatByProp.ValueKind != System.Text.Json.JsonValueKind.Null)
                            {
                                string creatByValue = creatByProp.GetString() ?? "";
                                if (!string.IsNullOrEmpty(creatByValue))
                                {
                                    creatByFromJson[idx] = creatByValue;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"提取領藥人信息失敗: {ex.Message}");
                }

                // 根據 pickupPersonName 從 person_page 查詢正確的 GUID
                if (!string.IsNullOrEmpty(pickupPersonName))
                {
                    try
                    {
                        List<object[]> personRows = personPageSQLControl.GetRowsByDefult(null, (int)HIS_DB_Lib.enum_人員資料.姓名, pickupPersonName);

                        if (personRows != null && personRows.Count > 0)
                        {
                            int guidColIndex = (int)HIS_DB_Lib.enum_人員資料.GUID;
                            if (personRows[0].Length > guidColIndex)
                            {
                                pickupPersonGuid = personRows[0][guidColIndex]?.ToString() ?? "";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"從person_page查詢GUID失敗: {ex.Message}");
                    }
                }


                // 生成換領單編號：yyyyMMdd-序號
                string todayDate = DateTime.Now.ToString("yyyyMMdd");
                int nextSeq = 1;

                try
                {
                    // 查詢所有換領單，找出今天最高的序號
                    List<object[]> allRows = sQLControl.GetAllRows(null);

                    if (allRows != null && allRows.Count > 0)
                    {
                        int requestNoColIndex = (int)HIS_DB_Lib.enum_申請換領報表資料.換領單編號;
                        int maxSeq = 0;

                        foreach (var row in allRows)
                        {
                            if (row != null && row.Length > requestNoColIndex && row[requestNoColIndex] != null)
                            {
                                string requestNo = row[requestNoColIndex].ToString();
                                // 檢查是否為今天的格式 yyyyMMdd-seq
                                if (requestNo.StartsWith(todayDate + "-") && requestNo.Length == 12)
                                {
                                    string seqStr = requestNo.Substring(9);
                                    if (int.TryParse(seqStr, out int seq))
                                    {
                                        maxSeq = Math.Max(maxSeq, seq);
                                    }
                                }
                            }
                        }
                        nextSeq = maxSeq + 1;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"查詢最高序號失敗: {ex.Message}, 將使用預設序號 1");
                }

                string newRequestNo = $"{todayDate}-{nextSeq:D3}";

                for (int i = 0; i < applications.Count; i++)
                {
                    if (string.IsNullOrEmpty(applications[i].訂單編號))
                    {
                        applications[i].訂單編號 = Guid.NewGuid().ToString();
                    }
                    // 自動設置換領單編號（忽略前端傳入的值）
                    applications[i].換領單編號 = newRequestNo;
                    applications[i].建立時間 = DateTime.Now.ToDateTimeString_6();
                    applications[i].最後修改時間 = DateTime.Now.ToDateTimeString_6();

                    // 強制使用正確的 GUID（從 person_page 查詢出的）
                    if (!string.IsNullOrEmpty(pickupPersonGuid))
                    {
                        applications[i].建立人 = pickupPersonGuid;
                    }
                    else if (creatByFromJson.ContainsKey(i) && !string.IsNullOrEmpty(creatByFromJson[i]))
                    {
                        applications[i].建立人 = creatByFromJson[i];
                    }

                    // 申請領藥人 = 申請單建立人
                    applications[i].領藥人 = applications[i].建立人;

                    // 從 order_list 表中獲取單次劑量（SD）並寫入 cUDOGIVDOSE
                    if (!string.IsNullOrEmpty(applications[i].orderlist編號))
                    {
                        try
                        {
                            List<object[]> orderRows = orderListSQLControl.GetRowsByDefult(null, (int)HIS_DB_Lib.enum_醫囑資料.GUID, applications[i].orderlist編號);

                            if (orderRows != null && orderRows.Count > 0)
                            {
                                int sdColIndex = (int)HIS_DB_Lib.enum_醫囑資料.單次劑量;

                                if (orderRows[0].Length > sdColIndex)
                                {
                                    string dosage = orderRows[0][sdColIndex]?.ToString() ?? "";
                                    applications[i].劑量使用單位 = dosage;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"查詢訂單 {applications[i].orderlist編號} 的單次劑量失敗: {ex.Message}");
                        }
                    }
                }

                List<object[]> list_value = applications.ClassToSQL<HIS_DB_Lib.medRequestApply, HIS_DB_Lib.enum_申請換領報表資料>();
                sQLControl.AddRows(null, list_value);

                // 更新 order_list 表的領藥ID和領藥姓名
                // 領藥ID 使用 pickupPersonId（登入ID，如 HS001）
                if (!string.IsNullOrEmpty(pickupPersonId) && !string.IsNullOrEmpty(pickupPersonName))
                {
                    try
                    {
                        // 更新每個訂單的領藥ID和領藥姓名
                        for (int i = 0; i < applications.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(applications[i].orderlist編號))
                            {
                                try
                                {
                                    List<object[]> orderRows = orderListSQLControl.GetRowsByDefult(null, (int)HIS_DB_Lib.enum_醫囑資料.GUID, applications[i].orderlist編號);

                                    if (orderRows != null && orderRows.Count > 0)
                                    {
                                        // 更新領藥ID和領藥姓名
                                        int pickupIdColIndex = (int)HIS_DB_Lib.enum_醫囑資料.領藥ID;
                                        int pickupNameColIndex = (int)HIS_DB_Lib.enum_醫囑資料.領藥姓名;
                                        int statusColIndex = (int)HIS_DB_Lib.enum_醫囑資料.狀態;

                                        if (orderRows[0].Length > pickupIdColIndex && orderRows[0].Length > pickupNameColIndex && orderRows[0].Length > statusColIndex)
                                        {

                                            orderRows[0][pickupIdColIndex] = pickupPersonId;      // 使用登入ID（如 HS001）
                                            orderRows[0][pickupNameColIndex] = pickupPersonName;  // 使用用戶姓名
                                            orderRows[0][statusColIndex] = "換領中";  // 更新狀態為換領中

                                            // 使用 UpdateByDefulteExtra 更新記錄
                                            orderListSQLControl.UpdateByDefulteExtra(null, orderRows);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log($"更新訂單 {applications[i].orderlist編號} 的領藥信息失敗: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"更新 order_list 表失敗: {ex.Message}");
                    }
                }
                else
                {
                    // 未提供必要的領藥人信息，跳過 order_list 更新
                }

                returnData.Code = 200;
                returnData.Result = $"新增申請換領報表!共<{applications.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = applications;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 以GUID查詢申請換領報表
        /// </summary>
        [Route("get_by_guid")]
        [HttpPost]
        public string POST_get_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_guid";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "medRequestApply";

                SQLControl sQLControl = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                if (returnData.ValueAry == null || returnData.ValueAry.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "未提供GUID";
                    return returnData.JsonSerializationt();
                }

                List<object[]> list_value_buf = sQLControl.GetRowsByDefult(null, (int)HIS_DB_Lib.enum_申請換領報表資料.訂單編號, returnData.ValueAry[0]);
                List<HIS_DB_Lib.medRequestApply> applications = list_value_buf.SQLToClass<HIS_DB_Lib.medRequestApply, HIS_DB_Lib.enum_申請換領報表資料>();

                if (applications.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<HIS_DB_Lib.medRequestApply>();
                    return returnData.JsonSerializationt();
                }

                // 查詢 person_page API 獲取人員姓名映射
                var personNameMap = new Dictionary<string, string>();
                try
                {
                    string person_page_url = "http://localhost:4433/api/person_page/get_all";
                    var personResponse = Net.WEBApiPostJson(person_page_url, "{}");
                    var jsonDoc = System.Text.Json.JsonDocument.Parse(personResponse);
                    var root = jsonDoc.RootElement;

                    if (root.TryGetProperty("Data", out JsonElement dataElement) && dataElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var person in dataElement.EnumerateArray())
                        {
                            string guid = null;
                            string name = null;

                            if (person.TryGetProperty("GUID", out JsonElement guidProp) && guidProp.ValueKind != System.Text.Json.JsonValueKind.Null)
                                guid = guidProp.GetString();
                            else if (person.TryGetProperty("Guid", out JsonElement guidProp2) && guidProp2.ValueKind != System.Text.Json.JsonValueKind.Null)
                                guid = guidProp2.GetString();

                            if (person.TryGetProperty("name", out JsonElement nameProp) && nameProp.ValueKind != System.Text.Json.JsonValueKind.Null)
                                name = nameProp.GetString();
                            else if (person.TryGetProperty("姓名", out JsonElement nameProp2) && nameProp2.ValueKind != System.Text.Json.JsonValueKind.Null)
                                name = nameProp2.GetString();

                            if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(name) && !personNameMap.ContainsKey(guid))
                            {
                                personNameMap[guid] = name;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"查詢 person_page API 失敗: {ex.Message}");
                }

                // 為申請單添加人員姓名字段
                foreach (var app in applications)
                {
                    app.處方醫師姓名 = !string.IsNullOrEmpty(app.處方醫師麻管證號) && personNameMap.ContainsKey(app.處方醫師麻管證號) ? personNameMap[app.處方醫師麻管證號] : "";
                    app.領藥人姓名 = !string.IsNullOrEmpty(app.領藥人) && personNameMap.ContainsKey(app.領藥人) ? personNameMap[app.領藥人] : "";
                    app.施打者姓名 = !string.IsNullOrEmpty(app.施打者) && personNameMap.ContainsKey(app.施打者) ? personNameMap[app.施打者] : "";
                    app.銷毀人姓名 = !string.IsNullOrEmpty(app.銷毀人) && personNameMap.ContainsKey(app.銷毀人) ? personNameMap[app.銷毀人] : "";
                    app.見證人姓名 = !string.IsNullOrEmpty(app.見證人) && personNameMap.ContainsKey(app.見證人) ? personNameMap[app.見證人] : "";
                    app.核對藥師姓名 = !string.IsNullOrEmpty(app.核對藥師) && personNameMap.ContainsKey(app.核對藥師) ? personNameMap[app.核對藥師] : "";
                    app.交班簽名人姓名 = !string.IsNullOrEmpty(app.交班簽名) && personNameMap.ContainsKey(app.交班簽名) ? personNameMap[app.交班簽名] : "";
                }

                returnData.Code = 200;
                returnData.Result = $"查詢成功!共<{applications.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = applications;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 獲取所有未過帳的訂單（用於編輯申請單清單）
        /// </summary>
        /// <remarks>
        /// 返回所有未過帳的訂單，並包含其對應的申請單資訊
        /// </remarks>
        [Route("get_AllRequest")]
        [HttpPost]
        public string POST_get_AllRequest([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_AllRequest";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                // 查詢 order_list 表中所有訂單
                SQLControl orderSQLControl = new SQLControl(Server, DB, "order_list", UserName, Password, Port, SSLMode);
                List<object[]> allOrderRows = orderSQLControl.GetAllRows(null);

                List<HIS_DB_Lib.OrderClass> unpostedOrders = new List<HIS_DB_Lib.OrderClass>();

                if (allOrderRows != null && allOrderRows.Count > 0)
                {
                    // 查詢 medRequestApply 表，建立 orderGuid 到申請單資訊的映射
                    SQLControl applySQLControl = new SQLControl(Server, DB, "medRequestApply", UserName, Password, Port, SSLMode);
                    List<object[]> allApplyRows = applySQLControl.GetAllRows(null);

                    Dictionary<string, (string requestNo, string cGuid)> applyMap = new Dictionary<string, (string, string)>();

                    if (allApplyRows != null && allApplyRows.Count > 0)
                    {
                        int cGuidIndex = (int)HIS_DB_Lib.enum_申請換領報表資料.訂單編號;
                        int orderlistGuidIndex = (int)HIS_DB_Lib.enum_申請換領報表資料.orderlist編號;
                        int requestNoIndex = (int)HIS_DB_Lib.enum_申請換領報表資料.換領單編號;

                        foreach (var applyRow in allApplyRows)
                        {
                            if (applyRow != null && applyRow.Length > orderlistGuidIndex)
                            {
                                string cGuid = applyRow.Length > cGuidIndex ? applyRow[cGuidIndex]?.ToString() ?? "" : "";
                                string orderGuid = applyRow[orderlistGuidIndex]?.ToString() ?? "";
                                string requestNo = applyRow.Length > requestNoIndex ? applyRow[requestNoIndex]?.ToString() ?? "" : "";

                                if (!string.IsNullOrEmpty(orderGuid) && !applyMap.ContainsKey(orderGuid))
                                {
                                    applyMap[orderGuid] = (requestNo, cGuid);
                                }
                            }
                        }
                    }

                    // 轉換訂單行為 OrderClass 對象，返回所有訂單，並配對申請單資訊
                    unpostedOrders = allOrderRows.SQLToClass<HIS_DB_Lib.OrderClass, HIS_DB_Lib.enum_醫囑資料>();

                    // 返回所有有申請單的訂單，並添加申請號資訊（包含 cGuid）
                    var filteredOrders = new List<object>();

                    foreach (var order in unpostedOrders)
                    {
                        // 只保留有申請單配對的訂單
                        if (applyMap.ContainsKey(order.GUID))
                        {
                            // 將申請號和 cGuid 加入訂單
                            var (requestNo, cGuid) = applyMap[order.GUID];
                            order.備註 = requestNo;

                            // 轉換為動態對象以包含 medrequestGuid
                            var orderJson = System.Text.Json.JsonSerializer.Serialize(order);
                            var orderDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(orderJson);
                            orderDict["medrequestGuid"] = cGuid;
                            orderDict["OrderlistGuid"] = order.GUID;
                            filteredOrders.Add(orderDict);
                        }
                    }

                    returnData.Data = filteredOrders;
                }
                else
                {
                    returnData.Data = new List<object>();
                }

                var resultList = returnData.Data as List<object> ?? new List<object>();
                returnData.Code = 200;
                returnData.Result = $"查詢成功!共<{resultList.Count}>筆訂單";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 查詢指定日期內所有已新增的申請單
        /// </summary>
        [Route("get_orderguids_by_date")]
        [HttpPost]
        public string POST_get_orderguids_by_date([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_orderguids_by_date";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "medRequestApply";

                SQLControl sQLControl = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                // 直接返回原始行數據
                List<object[]> allRows = sQLControl.GetAllRows(null);

                returnData.Code = 200;
                returnData.Result = $"查詢成功!共<{(allRows != null ? allRows.Count : 0)}>筆申請單";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = allRows;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 以換領單編號查詢申請換領報表
        /// </summary>
        [Route("get_by_request_no")]
        [HttpPost]
        public string POST_get_by_request_no([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_request_no";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "medRequestApply";

                SQLControl sQLControl = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                if (returnData.ValueAry == null || returnData.ValueAry.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "未提供換領單編號";
                    return returnData.JsonSerializationt();
                }

                List<object[]> list_value_buf = sQLControl.GetRowsByDefult(null, (int)HIS_DB_Lib.enum_申請換領報表資料.換領單編號, returnData.ValueAry[0]);
                List<HIS_DB_Lib.medRequestApply> applications = list_value_buf.SQLToClass<HIS_DB_Lib.medRequestApply, HIS_DB_Lib.enum_申請換領報表資料>();

                if (applications.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<HIS_DB_Lib.medRequestApply>();
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"查詢成功!共<{applications.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = applications;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 以GUID更新申請換領報表
        /// </summary>
        [Route("update_by_guid")]
        [HttpPost]
        public string POST_update_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_by_guid";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "medRequestApply";

                SQLControl sQLControl = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                List<HIS_DB_Lib.medRequestApply> applications = returnData.Data.ObjToClass<List<HIS_DB_Lib.medRequestApply>>();

                if (applications == null || applications.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無更新資料";
                    return returnData.JsonSerializationt();
                }

                for (int i = 0; i < applications.Count; i++)
                {
                    applications[i].最後修改時間 = DateTime.Now.ToDateTimeString_6();
                }

                List<object[]> list_value = applications.ClassToSQL<HIS_DB_Lib.medRequestApply, HIS_DB_Lib.enum_申請換領報表資料>();
                sQLControl.UpdateByDefulteExtra(null, list_value);

                returnData.Code = 200;
                returnData.Result = $"更新申請換領報表!共<{applications.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = applications;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 部分更新申請換領報表（按orderGuid或cGuid）- 支援欄位級別的部分更新
        /// </summary>
        /// <remarks>
        /// <para><b>功能說明</b></para>
        /// 支援只更新指定欄位，無需提供完整物件。常用於編輯詳情彈窗中逐步填入人員資訊
        /// 可傳入 order_list 的 GUID（cOrderlistGuid）或 medRequestApply 的 GUID（cGuid）
        ///
        /// <para><b>請求 JSON 範例</b></para>
        /// <code>
        /// {
        ///   "ValueAry": ["order-guid"],
        ///   "Data": {
        ///     "cDrugAdministrator": "張三",
        ///     "cWitness": "李四"
        ///   }
        /// }
        /// </code>
        /// </remarks>
        [Route("partial_update_by_guid")]
        [HttpPost]
        public string POST_partial_update_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "partial_update_by_guid";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                if (returnData.ValueAry == null || returnData.ValueAry.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "未提供GUID";
                    return returnData.JsonSerializationt();
                }

                string guid = returnData.ValueAry[0].ToString();
                var updateFields = returnData.Data as System.Text.Json.JsonElement?;

                if (updateFields == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "無更新欄位資料";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "medRequestApply";

                SQLControl sQLControl = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                // 按 cGuid（訂單編號）查詢
                List<object[]> list_value_buf = sQLControl.GetRowsByDefult(null, (int)HIS_DB_Lib.enum_申請換領報表資料.orderlist編號, guid);

                List<HIS_DB_Lib.medRequestApply> applications = list_value_buf.SQLToClass<HIS_DB_Lib.medRequestApply, HIS_DB_Lib.enum_申請換領報表資料>();

                if (applications.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無GUID={guid}的資料";
                    return returnData.JsonSerializationt();
                }

                HIS_DB_Lib.medRequestApply application = applications[0];

                // 更新提供的欄位
                if (updateFields.HasValue)
                {
                    var jsonElement = updateFields.Value;
                    foreach (var property in jsonElement.EnumerateObject())
                    {
                        string fieldName = property.Name;
                        string fieldValue = property.Value.GetString() ?? "";

                        // 對應字段更新
                        switch (fieldName)
                        {
                            case "PrescribingDoctorNarcoticLicenseNo":
                                application.處方醫師麻管證號 = fieldValue;
                                break;
                            case "DrugReceiver":
                                application.領藥人 = fieldValue;
                                break;
                            case "DrugAdministrator":
                                application.施打者 = fieldValue;
                                break;
                            case "DrugDestroyer":
                                application.銷毀人 = fieldValue;
                                break;
                            case "Witness":
                                application.見證人 = fieldValue;
                                break;
                            case "CheckingPharmacist":
                                application.核對藥師 = fieldValue;
                                break;
                            case "HandoverSignature":
                                application.交班簽名 = fieldValue;
                                break;
                            case "RequestNo":
                                application.換領單編號 = fieldValue;
                                break;
                            case "UDOGIVDOSE":
                                application.劑量使用單位 = fieldValue;
                                break;
                        }
                    }
                }

                application.最後修改時間 = DateTime.Now.ToDateTimeString_6();

                // 轉換為 object[] 以供更新
                object[] updateData = new List<HIS_DB_Lib.medRequestApply> { application }
                    .ClassToSQL<HIS_DB_Lib.medRequestApply, HIS_DB_Lib.enum_申請換領報表資料>()[0];

                // 使用 UpdateByDefult 按主鍵（cGuid）更新
                if (string.IsNullOrEmpty(application.訂單編號))
                {
                    returnData.Code = -200;
                    returnData.Result = "訂單編號為空，無法更新";
                    return returnData.JsonSerializationt();
                }

                int result = sQLControl.UpdateByDefult(null, (int)HIS_DB_Lib.enum_申請換領報表資料.訂單編號, application.訂單編號, updateData);

                if (result <= 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"更新失敗：影響行數為 {result}";
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"部分更新成功";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = application;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 取得 PDF 列印所需的完整資料（包含 order_list 和 medRequestApply 的資料）
        /// </summary>
        /// <remarks>
        /// <para><b>功能說明</b></para>
        /// 根據 orderlist GUID 查詢 order_list 表的基礎資料，計算使用量、殘餘量、扣帳量，
        /// 並查詢 medRequestApply 表的人員信息，一次返回所有 PDF 需要的資料
        ///
        /// <para><b>請求 JSON 範例</b></para>
        /// <code>
        /// {
        ///   "ValueAry": ["orderlist-guid"]
        /// }
        /// </code>
        /// </remarks>
        [Route("get_pdf_data")]
        [HttpPost]
        public string POST_get_pdf_data([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_pdf_data";
            try
            {
                if (returnData.ValueAry == null || returnData.ValueAry.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "未提供 orderlist GUID";
                    return returnData.JsonSerializationt();
                }

                string orderlistGuid = returnData.ValueAry[0].ToString();

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無 Server 資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                // 查詢 order_list 表的資料
                SQLControl orderListSQLControl = new SQLControl(Server, DB, "order_list", UserName, Password, Port, SSLMode);
                List<object[]> orderRows = orderListSQLControl.GetRowsByDefult(null, (int)HIS_DB_Lib.enum_醫囑資料.GUID, orderlistGuid);

                if (orderRows == null || orderRows.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無 orderlist GUID={orderlistGuid} 的資料";
                    return returnData.JsonSerializationt();
                }

                HIS_DB_Lib.OrderClass orderData = orderRows.SQLToClass<HIS_DB_Lib.OrderClass, HIS_DB_Lib.enum_醫囑資料>()[0];

                // 計算使用量、殘餘量、扣帳量
                decimal singleDose = 0m;
                if (decimal.TryParse(orderData.單次劑量, out decimal parsedDose))
                {
                    singleDose = parsedDose;
                }

                decimal ceiledDose = Math.Ceiling(singleDose);
                decimal residualQty = ceiledDose - singleDose;
                decimal deductedQty = ceiledDose;

                // 查詢 medRequestApply 表的人員資料
                SQLControl medRequestApplySQLControl = new SQLControl(Server, DB, "medRequestApply", UserName, Password, Port, SSLMode);
                List<object[]> applyRows = medRequestApplySQLControl.GetRowsByDefult(null, (int)HIS_DB_Lib.enum_申請換領報表資料.orderlist編號, orderlistGuid);

                HIS_DB_Lib.medRequestApply applyData = null;
                if (applyRows != null && applyRows.Count > 0)
                {
                    applyData = applyRows.SQLToClass<HIS_DB_Lib.medRequestApply, HIS_DB_Lib.enum_申請換領報表資料>()[0];
                }

                // 查詢 person_page 表並建立 GUID -> 姓名映射
                Dictionary<string, string> personNameMap = new Dictionary<string, string>();
                try
                {
                    SQLControl personPageSQLControl = new SQLControl(Server, DB, "person_page", UserName, Password, Port, SSLMode);
                    List<object[]> personRows = personPageSQLControl.GetAllRows(null);

                    if (personRows != null && personRows.Count > 0)
                    {
                        int guidColIndex = (int)HIS_DB_Lib.enum_人員資料.GUID;
                        int nameColIndex = (int)HIS_DB_Lib.enum_人員資料.姓名;

                        foreach (var row in personRows)
                        {
                            if (row != null && row.Length > guidColIndex && row.Length > nameColIndex)
                            {
                                string guid = row[guidColIndex]?.ToString() ?? "";
                                string name = row[nameColIndex]?.ToString() ?? "";

                                if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(name) && !personNameMap.ContainsKey(guid))
                                {
                                    personNameMap[guid] = name;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"查詢 person_page 表失敗: {ex.Message}");
                }

                // 將 GUID 轉換為姓名
                Func<string, string> GetPersonName = (guid) =>
                {
                    if (string.IsNullOrEmpty(guid))
                        return "";
                    return personNameMap.ContainsKey(guid) ? personNameMap[guid] : guid;
                };

                // 組合返回資料
                Dictionary<string, object> pdfData = new Dictionary<string, object>
                {
                    { "藥品碼", orderData.藥品碼 ?? "" },
                    { "藥品名稱", orderData.藥品名稱 ?? "" },
                    { "劑量單位", orderData.劑量單位 ?? "" },
                    { "交易量", orderData.交易量 ?? "" },
                    { "護理站", orderData.病房 ?? "" },

                    { "使用量", singleDose.ToString() },
                    { "殘餘量", residualQty.ToString() },
                    { "扣帳量", deductedQty.ToString() },

                    { "處方醫師麻管證號", GetPersonName(applyData?.處方醫師麻管證號 ?? "") },
                    { "領藥人", GetPersonName(applyData?.領藥人 ?? "") },
                    { "施打者", GetPersonName(applyData?.施打者 ?? "") },
                    { "銷毀人", GetPersonName(applyData?.銷毀人 ?? "") },
                    { "見證人", GetPersonName(applyData?.見證人 ?? "") },
                    { "核對藥師", GetPersonName(applyData?.核對藥師 ?? "") },
                    { "交班簽名", GetPersonName(applyData?.交班簽名 ?? "") },
                    { "劑量使用單位", applyData?.劑量使用單位 ?? "" },
                    { "領藥人簽名", orderData.領藥姓名 ?? "" }
                };

                returnData.Code = 200;
                returnData.Result = $"查詢成功";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = pdfData;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 按建立時間範圍查詢已過帳的申請換領報表（含PDF預覽資料）
        /// </summary>
        /// <remarks>
        /// <para><b>功能說明</b></para>
        /// 查詢指定時間範圍內的 medRequestApply 資料，篩選對應的 order_list 狀態為已過帳，
        /// 並同時返回 PDF 預覽所需的完整資料
        ///
        /// <para><b>請求 JSON 範例</b></para>
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "2026-05-01 00:00:00",
        ///     "2026-05-18 23:59:59"
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        [Route("get_by_creat_time_posted")]
        [HttpPost]
        public string POST_get_by_creat_time_posted([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_creat_time_posted";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                if (returnData.ValueAry == null || returnData.ValueAry.Count < 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[起始時間][結束時間][選項：護理站名稱]";
                    return returnData.JsonSerializationt(true);
                }

                string 起始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];
                string 護理站名稱 = returnData.ValueAry.Count > 2 ? returnData.ValueAry[2] : "";

                if (!起始時間.Check_Date_String() || !結束時間.Check_Date_String())
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }

                DateTime date_st = 起始時間.StringToDateTime();
                DateTime date_end = 結束時間.StringToDateTime();

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                // 查詢 person_page 表並建立 GUID -> 姓名映射
                Dictionary<string, string> personNameMap = new Dictionary<string, string>();
                try
                {
                    SQLControl personPageSQLControl = new SQLControl(Server, DB, "person_page", UserName, Password, Port, SSLMode);
                    List<object[]> personRows = personPageSQLControl.GetAllRows(null);

                    if (personRows != null && personRows.Count > 0)
                    {
                        int guidColIndex = (int)HIS_DB_Lib.enum_人員資料.GUID;
                        int nameColIndex = (int)HIS_DB_Lib.enum_人員資料.姓名;

                        foreach (var row in personRows)
                        {
                            if (row != null && row.Length > guidColIndex && row.Length > nameColIndex)
                            {
                                string guid = row[guidColIndex]?.ToString() ?? "";
                                string name = row[nameColIndex]?.ToString() ?? "";

                                if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(name) && !personNameMap.ContainsKey(guid))
                                {
                                    personNameMap[guid] = name;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"查詢 person_page 表失敗: {ex.Message}");
                }

                // 查詢 medRequestApply 表
                SQLControl medRequestApplySQLControl = new SQLControl(Server, DB, "medRequestApply", UserName, Password, Port, SSLMode);
                List<object[]> applyRows = medRequestApplySQLControl.GetRowsByBetween(null, (int)HIS_DB_Lib.enum_申請換領報表資料.建立時間, date_st.ToDateTimeString(), date_end.ToDateTimeString());
                List<HIS_DB_Lib.medRequestApply> applications = applyRows.SQLToClass<HIS_DB_Lib.medRequestApply, HIS_DB_Lib.enum_申請換領報表資料>();

                // 查詢 order_list 表以比對狀態
                SQLControl orderListSQLControl = new SQLControl(Server, DB, "order_list", UserName, Password, Port, SSLMode);

                // 篩選：只保留有對應的 order_list 且狀態為已過帳的記錄
                var filteredResults = new List<Dictionary<string, object>>();
                foreach (var app in applications)
                {
                    if (!string.IsNullOrEmpty(app.orderlist編號))
                    {
                        try
                        {
                            List<object[]> orderRows = orderListSQLControl.GetRowsByDefult(null, (int)HIS_DB_Lib.enum_醫囑資料.GUID, app.orderlist編號);
                            if (orderRows != null && orderRows.Count > 0)
                            {
                                List<HIS_DB_Lib.OrderClass> orders = orderRows.SQLToClass<HIS_DB_Lib.OrderClass, HIS_DB_Lib.enum_醫囑資料>();

                                // 檢查狀態是否為已過帳，且若指定了護理站則檢查病房是否匹配
                                if (orders.Count > 0 && orders[0].狀態 == "已過帳" &&
                                    (string.IsNullOrEmpty(護理站名稱) || orders[0].病房 == 護理站名稱))
                                {
                                    var orderData = orders[0];

                                    // 計算使用量、殘餘量、扣帳量（PDF資料）
                                    decimal singleDose = 0m;
                                    if (decimal.TryParse(orderData.單次劑量, out decimal parsedDose))
                                    {
                                        singleDose = parsedDose;
                                    }

                                    decimal ceiledDose = Math.Ceiling(singleDose);
                                    decimal residualQty = ceiledDose - singleDose;
                                    decimal deductedQty = ceiledDose;

                                    // 轉換 GUID 為姓名
                                    Func<string, string> GetPersonName = (guid) =>
                                    {
                                        if (string.IsNullOrEmpty(guid))
                                            return "";
                                        return personNameMap.ContainsKey(guid) ? personNameMap[guid] : guid;
                                    };

                                    // 組合完整資料
                                    var appJson = System.Text.Json.JsonSerializer.Serialize(app);
                                    var resultDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(appJson);

                                    // 添加 order_list 資料
                                    resultDict["order_GUID"] = orderData.GUID;
                                    resultDict["order_藥品碼"] = orderData.藥品碼 ?? "";
                                    resultDict["order_藥品名稱"] = orderData.藥品名稱 ?? "";
                                    resultDict["order_劑量單位"] = orderData.劑量單位 ?? "";
                                    resultDict["order_交易量"] = orderData.交易量 ?? "";
                                    resultDict["order_病房"] = orderData.病房 ?? "";
                                    resultDict["order_病歷號"] = orderData.病歷號 ?? "";
                                    resultDict["order_病人姓名"] = orderData.病人姓名 ?? "";
                                    resultDict["order_床號"] = orderData.床號 ?? "";
                                    resultDict["order_單次劑量"] = orderData.單次劑量 ?? "";
                                    resultDict["order_頻次"] = orderData.頻次 ?? "";
                                    resultDict["order_途徑"] = orderData.途徑 ?? "";
                                    resultDict["order_產出時間"] = orderData.產出時間 ?? "";

                                    // 添加 PDF 預覽資料
                                    resultDict["pdf_使用量"] = singleDose.ToString();
                                    resultDict["pdf_殘餘量"] = residualQty.ToString();
                                    resultDict["pdf_扣帳量"] = deductedQty.ToString();
                                    resultDict["pdf_處方醫師麻管證號"] = GetPersonName(app.處方醫師麻管證號 ?? "");
                                    resultDict["pdf_領藥人"] = GetPersonName(app.領藥人 ?? "");
                                    resultDict["pdf_施打者"] = GetPersonName(app.施打者 ?? "");
                                    resultDict["pdf_銷毀人"] = GetPersonName(app.銷毀人 ?? "");
                                    resultDict["pdf_見證人"] = GetPersonName(app.見證人 ?? "");
                                    resultDict["pdf_核對藥師"] = GetPersonName(app.核對藥師 ?? "");
                                    resultDict["pdf_交班簽名"] = GetPersonName(app.交班簽名 ?? "");
                                    resultDict["pdf_劑量使用單位"] = app.劑量使用單位 ?? "";
                                    resultDict["pdf_領藥人簽名"] = orderData.領藥姓名 ?? "";

                                    filteredResults.Add(resultDict);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"查詢訂單 {app.orderlist編號} 失敗: {ex.Message}");
                        }
                    }
                }

                returnData.Code = 200;
                returnData.Result = $"查詢成功!共<{filteredResults.Count}>筆已過帳的申請換領報表";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = filteredResults;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, new HIS_DB_Lib.enum_申請換領報表資料());
            return table.JsonSerializationt(true);
        }
    }
}