using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using MyUI;
using NPOI;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class personSignature : ControllerBase
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private const long MAX_FILE_SIZE = 2 * 1024 * 1024; // 2MB
        private readonly string[] ALLOWED_CONTENT_TYPES = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        /// <summary>
        /// 初始化人員簽名資料表
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("init")]
        [HttpPost]
        public string POST_init([FromBody] returnData returnData)
        {
            try
            {
                returnData ??= new returnData();

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "人員資料");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(
                        returnData.ServerName,
                        returnData.ServerType,
                        "人員資料"
                    );
                }

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                var serverSetting = sys_serverSettingClasses[0];

                CheckCreatTable(serverSetting);
                CheckCreatTable_log(serverSetting);

                returnData.Code = 200;
                returnData.Result = "person_signature、person_signaturelog，兩張資料表建立完成";
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
        /// 初始化人員簽名異動紀錄資料表
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("init_log")]
        [HttpPost]
        public string POST_init_log([FromBody] returnData returnData)
        {
            try
            {
                returnData ??= new returnData();

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "人員資料");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(
                        returnData.ServerName,
                        returnData.ServerType,
                        "人員資料"
                    );
                }

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                return CheckCreatTable_log(sys_serverSettingClasses[0]);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 根據PersonGUID獲取用戶簽章
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構，Value為PersonGUID</param>
        /// {
        ///     "Value": "PersonGUID"
        /// }
        /// <returns>[returnData.Data]personSignature物件</returns>
        [Route("personSignature_by_guid")]
        [HttpPost]
        public string personSignature_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "personSignature_by_guid";
            try
            {
                string PersonGUID = returnData.Value;
                if (PersonGUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "PersonGUID不可為空";
                    return returnData.JsonSerializationt();
                }

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "人員資料");
                SQLControl sQLControl_signature = new SQLControl(Server, DB, "person_Signature", UserName, Password, Port, SSLMode);

                List<object[]> list_value = sQLControl_signature.GetRowsByDefult(null, (int)enum_personSignature.PersonGUID, PersonGUID);
                if (list_value.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無該用戶簽章資料";
                    return returnData.JsonSerializationt();
                }

                object[] obj = list_value[0];
                HIS_DB_Lib.personSignature signature = BuildSignatureFromArray(obj);
                returnData.Data = signature;
                returnData.Code = 200;
                returnData.Result = "獲取簽章成功";
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
        /// 批量獲取多個人員的簽章（用於PDF預覽）
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構，ValueAry為PersonGUID陣列</param>
        /// <returns>[returnData.Data]personSignature物件陣列</returns>
        [Route("personSignature_by_guids")]
        [HttpPost]
        public string personSignature_by_guids([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "personSignature_by_guids";
            try
            {
                if (returnData.ValueAry == null || returnData.ValueAry.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "PersonGUID陣列不可為空";
                    return returnData.JsonSerializationt();
                }

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "人員資料");
                SQLControl sQLControl_signature = new SQLControl(Server, DB, "person_Signature", UserName, Password, Port, SSLMode);

                List<HIS_DB_Lib.personSignature> signatures = new List<HIS_DB_Lib.personSignature>();

                foreach (string PersonGUID in returnData.ValueAry)
                {
                    if (PersonGUID.StringIsEmpty()) continue;

                    List<object[]> list_value = sQLControl_signature.GetRowsByDefult(null, (int)enum_personSignature.PersonGUID, PersonGUID);
                    if (list_value.Count > 0)
                    {
                        HIS_DB_Lib.personSignature signature = BuildSignatureFromArray(list_value[0]);
                        signatures.Add(signature);
                    }
                }

                returnData.Data = signatures;
                returnData.Code = 200;
                returnData.Result = $"獲取{signatures.Count}筆簽章成功";
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
        /// 新增或更新人員簽章（FormData上傳圖片）
        /// </summary>
        /// <remarks>
        /// FormData格式：
        /// - PersonGUID: 人員GUID
        /// - OperatorGUID: 操作者GUID
        /// - file: 圖片文件
        /// </remarks>
        [Route("add_PersonSignature")]
        [HttpPost]
        public async Task<string> POST_add_PersonSignature()
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData returnData = new returnData();
            returnData.Method = "POST_add_PersonSignature";

            try
            {
                var form = await Request.ReadFormAsync();
                string PersonGUID = form["PersonGUID"].ToString();
                string OperatorGUID = form["OperatorGUID"].ToString();
                var file = form.Files.FirstOrDefault();

                if (PersonGUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "PersonGUID不可為空";
                    return returnData.JsonSerializationt();
                }

                if (file == null || file.Length == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "未提供有效的圖片文件";
                    return returnData.JsonSerializationt();
                }

                // 驗證文件大小
                if (file.Length > MAX_FILE_SIZE)
                {
                    returnData.Code = -200;
                    returnData.Result = $"文件大小超過限制（最大2MB，當前{file.Length / (1024 * 1024)}MB）";
                    return returnData.JsonSerializationt();
                }

                // 驗證文件類型
                if (!ALLOWED_CONTENT_TYPES.Contains(file.ContentType.ToLower()))
                {
                    returnData.Code = -200;
                    returnData.Result = "只接受圖片格式（jpg, png, gif, webp）";
                    return returnData.JsonSerializationt();
                }

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "人員資料");

                // 驗證PersonGUID是否存在於person_page表，並取得用戶ID
                SQLControl sQLControl_person = new SQLControl(Server, DB, "person_page", UserName, Password, Port, SSLMode);
                List<object[]> person_list = sQLControl_person.GetRowsByDefult(null, (int)enum_人員資料.GUID, PersonGUID);
                if (person_list.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無該人員資料";
                    return returnData.JsonSerializationt();
                }

                personPageClass person = person_list[0].SQLToClass<personPageClass, enum_人員資料>();
                string UserID = person.ID;

                // 轉換文件為Base64
                string signatureBase64 = await FileToBase64(file);

                SQLControl sQLControl_signature = new SQLControl(Server, DB, "person_Signature", UserName, Password, Port, SSLMode);
                List<object[]> signature_list = sQLControl_signature.GetRowsByDefult(null, (int)enum_personSignature.PersonGUID, PersonGUID);

                DateTime now = DateTime.Now;

                HIS_DB_Lib.personSignature signature;

                if (signature_list.Count > 0)
                {
                    // 更新現有簽章 - 直接修改 object[] 陣列
                    object[] obj = signature_list[0];
                    string beforeFileName = obj[(int)enum_personSignature.FileName].ObjectToString();
                    string signatureGUID = obj[(int)enum_personSignature.GUID].ObjectToString();

                    obj[(int)enum_personSignature.SignatureBase64] = signatureBase64;
                    obj[(int)enum_personSignature.ContentType] = file.ContentType;
                    obj[(int)enum_personSignature.FileName] = file.FileName;
                    obj[(int)enum_personSignature.FileSize] = file.Length.ToString();
                    obj[(int)enum_personSignature.UpdateAt] = now;

                    List<object[]> list_update = new List<object[]> { obj };

                    if (list_update.Count > 0)
                        await sQLControl_signature.UpdateRowsAsync(null, list_update);

                    // 記錄日誌
                    LogSignatureChange(
                        actionType: "Update",
                        signatureGUID: signatureGUID,
                        targetPersonGUID: PersonGUID,
                        operatorGUID: OperatorGUID,
                        beforeFileName: beforeFileName,
                        afterFileName: file.FileName
                    );

                    // 構建返回對象（不再轉換以避免 DateTime 問題）
                    signature = new HIS_DB_Lib.personSignature
                    {
                        GUID = obj[(int)enum_personSignature.GUID].ObjectToString(),
                        PersonGUID = obj[(int)enum_personSignature.PersonGUID].ObjectToString(),
                        SignatureBase64 = obj[(int)enum_personSignature.SignatureBase64].ObjectToString(),
                        ContentType = obj[(int)enum_personSignature.ContentType].ObjectToString(),
                        FileSize = obj[(int)enum_personSignature.FileSize].ObjectToString(),
                        CreatBy = obj[(int)enum_personSignature.CreatBy].ObjectToString(),
                        CreatAt = obj[(int)enum_personSignature.CreatAt] is DateTime dt1 ? dt1 : null,
                        UpdateAt = now
                    };

                    returnData.Data = signature;
                    returnData.Code = 200;
                    returnData.Result = "更新簽章成功";
                }
                else
                {
                    // 新增簽章
                    string newSignatureGUID = Guid.NewGuid().ToString();
                    signature = new HIS_DB_Lib.personSignature
                    {
                        GUID = newSignatureGUID,
                        PersonGUID = PersonGUID,
                        SignatureBase64 = signatureBase64,
                        ContentType = file.ContentType,
                        FileName = file.FileName,
                        FileSize = file.Length.ToString(),
                        CreatBy = UserID,
                        UpdateAt = now,
                        CreatAt = now
                    };

                    List<object[]> list_add = new List<object[]>();
                    object[] obj = signature.ClassToSQL<HIS_DB_Lib.personSignature, enum_personSignature>();
                    list_add.Add(obj);

                    if (list_add.Count > 0)
                        await sQLControl_signature.AddRowsAsync(null, list_add);

                    // 記錄日誌
                    LogSignatureChange(
                        actionType: "Create",
                        signatureGUID: newSignatureGUID,
                        targetPersonGUID: PersonGUID,
                        operatorGUID: OperatorGUID,
                        beforeFileName: null,
                        afterFileName: file.FileName
                    );

                    returnData.Data = signature;
                    returnData.Code = 200;
                    returnData.Result = "新增簽章成功";
                }

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
        /// 刪除人員簽章（清空SignatureBase64欄位）
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構
        ///  {
        ///  "Value": "PersonGUID"
        ///   }
        /// </param>
        /// <returns></returns>
        [Route("delete_PersonSignature")]
        [HttpPost]
        public string POST_delete_PersonSignature([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "delete_PersonSignature";
            try
            {
                string identifier = returnData.Value;
                if (identifier.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "PersonGUID或GUID不可為空";
                    return returnData.JsonSerializationt();
                }

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "人員資料");
                SQLControl sQLControl_signature = new SQLControl(Server, DB, "person_Signature", UserName, Password, Port, SSLMode);

                List<object[]> signature_list = null;

                // 先嘗試用PersonGUID查找
                signature_list = sQLControl_signature.GetRowsByDefult(null, (int)enum_personSignature.PersonGUID, identifier);

                // 如果沒找到，嘗試用GUID查找
                if (signature_list.Count == 0)
                {
                    signature_list = sQLControl_signature.GetRowsByDefult(null, (int)enum_personSignature.GUID, identifier);
                }

                if (signature_list.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無簽章資料";
                    return returnData.JsonSerializationt();
                }

                // 清空SignatureBase64欄位
                object[] obj = signature_list[0];
                obj[(int)enum_personSignature.SignatureBase64] = "";
                obj[(int)enum_personSignature.UpdateAt] = DateTime.Now;
                sQLControl_signature.UpdateByDefulteExtra(null, obj);

                returnData.Code = 200;
                returnData.Result = "刪除簽章成功";
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
        /// 記錄簽章異動日誌
        /// </summary>
        private void LogSignatureChange(string actionType, string signatureGUID,
            string targetPersonGUID, string operatorGUID, string beforeFileName, string afterFileName)
        {
            try
            {
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "人員資料");
                SQLControl sQLControl_log = new SQLControl(Server, DB, "person_SignatureLog", UserName, Password, Port, SSLMode);

                HIS_DB_Lib.person_SignatureLog log = new HIS_DB_Lib.person_SignatureLog
                {
                    GUID = Guid.NewGuid().ToString(),
                    SignatureGUID = signatureGUID,
                    TargetPersonGUID = targetPersonGUID,
                    OperatorGUID = operatorGUID,
                    ActionType = actionType,
                    BeforeFileName = beforeFileName,
                    AfterFileName = afterFileName,
                    CreateAt = DateTime.Now
                };

                List<object[]> list_add = new List<object[]>();
                object[] obj = log.ClassToSQL<HIS_DB_Lib.person_SignatureLog, HIS_DB_Lib.enum_personSignatureLog>();
                list_add.Add(obj);

                if (list_add.Count > 0)
                    sQLControl_log.AddRows(null, list_add);
            }
            catch (Exception e)
            {
                // 日誌寫入失敗不應該影響主流程，但可以記錄到應用日誌
                Console.WriteLine($"簽章日誌寫入失敗: {e.Message}");
            }
        }

        /// <summary>
        /// 從 object[] 數組安全地構建 personSignature 對象，避免 DateTime 轉換問題
        /// </summary>
        private HIS_DB_Lib.personSignature BuildSignatureFromArray(object[] obj)
        {
            return new HIS_DB_Lib.personSignature
            {
                GUID = obj[(int)enum_personSignature.GUID]?.ToString() ?? "",
                PersonGUID = obj[(int)enum_personSignature.PersonGUID]?.ToString() ?? "",
                SignatureBase64 = obj[(int)enum_personSignature.SignatureBase64]?.ToString() ?? "",
                ContentType = obj[(int)enum_personSignature.ContentType]?.ToString() ?? "",
                FileSize = obj[(int)enum_personSignature.FileSize]?.ToString() ?? "",
                CreatBy = obj[(int)enum_personSignature.CreatBy]?.ToString() ?? "",
                CreatAt = TryParseDateTime(obj[(int)enum_personSignature.CreatAt]),
                UpdateAt = TryParseDateTime(obj[(int)enum_personSignature.UpdateAt])
            };
        }

        /// <summary>
        /// 安全地解析 DateTime，處理字符串和 DateTime 類型
        /// </summary>
        private DateTime? TryParseDateTime(object value)
        {
            if (value == null) return null;
            if (value is DateTime dt) return dt;
            if (value is string str && DateTime.TryParse(str, out var parsed)) return parsed;
            return null;
        }

        /// <summary>
        /// 將IFormFile轉換為Base64字符串
        /// </summary>
        private async Task<string> FileToBase64(Microsoft.AspNetCore.Http.IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                byte[] bytes = stream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            Table table = MethodClass.CheckCreatTable(
                sys_serverSettingClass,
                new HIS_DB_Lib.enum_personSignature()
            );

            return table.JsonSerializationt(true);
        }

        private string CheckCreatTable_log(sys_serverSettingClass sys_serverSettingClass)
        {
            Table table = MethodClass.CheckCreatTable(
                sys_serverSettingClass,
                new HIS_DB_Lib.enum_personSignatureLog()
            );

            return table.JsonSerializationt(true);
        }
    }
}