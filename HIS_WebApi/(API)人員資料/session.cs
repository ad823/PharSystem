using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Configuration;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Hosting.Server;
using System.Net;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class sessionController : Controller
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static string API = Method.GetServerAPI("Main", "網頁", "API01");
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        /// <summary>
        /// 初始化資料庫
        /// </summary>
        /// <remarks>
        /// {
        ///     
        /// }
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("init_login_data_index")]
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(sessionClass))]
        [HttpPost]
        public string init_login_data_index([FromBody] returnData returnData)
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
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "人員資料");
                }
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                string result = CheckCreatTable(sys_serverSettingClasses[0]);
                loadData();
                return result;

            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }
        [HttpGet]
        public string Get(string level)
        {
            return GetPermissions(level.StringToInt32() ,"","").JsonSerializationt();
        }
        /// <summary>
        /// 使用者登入並建立/更新登入工作階段（session），
        /// 支援三種登入方式：帳號+密碼（ID+Password） / UID（卡號） / BARCODE（一維條碼）。
        /// admin 帳號具有最高權限（-1）。 
        /// </summary>
        /// <remarks>
        /// <para><b>路由</b>：POST /login</para>
        /// <para><b>登入流程摘要</b>：</para>
        /// <list type="number">
        ///   <item>
        ///     <description>解析資料庫連線組態（若 <c>ServerName</c>/<c>ServerType</c> 未提供，預設搜尋「Main / 網頁 / VM端」）。</description>
        ///   </item>
        ///   <item>
        ///     <description>若提供 <c>UID</c> 或 <c>BARCODE</c>，先以人員資料表查得對應 <c>ID</c> 與 <c>Password</c>。</description>
        ///   </item>
        ///   <item>
        ///     <description>驗證帳號是否存在與密碼是否正確；或比對是否為 admin（最高權限）。</description>
        ///   </item>
        ///   <item>
        ///     <description>於 <c>login_session</c> 建立或更新使用者的 session 記錄（GUID、verifyTime、loginTime 等）。</description>
        ///   </item>
        ///   <item>
        ///     <description>回傳 <c>sessionClass</c>（含人員名稱、單位、權限、顏色等），以及對應權限清單 <c>Permissions</c>。</description>
        ///   </item>
        /// </list>
        ///
        /// <para><b>Request Body 範例</b>（任一方式即可）：</para>
        /// <code>
        /// {
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "Data": {
        ///     "ID": "pharm001",
        ///     "Password": "******",
        ///     "UID": "",
        ///     "BARCODE": ""
        ///   }
        /// }
        /// </code>
        /// <code>
        /// {
        ///   "Data": {
        ///     "ID": "",
        ///     "Password": "",
        ///     "UID": "E2000017221101441890ABCD",
        ///     "BARCODE": ""
        ///   }
        /// }
        /// </code>
        /// <code>
        /// {
        ///   "Data": {
        ///     "ID": "",
        ///     "Password": "",
        ///     "UID": "",
        ///     "BARCODE": "A123456789"
        ///   }
        /// }
        /// </code>
        ///
        /// <para><b>成功回應範例</b>（<c>Code=200</c>）：</para>
        /// <code>
        /// {
        ///   "Code": 200,
        ///   "Method": "login",
        ///   "Result": "登入成功!",
        ///   "Data": {
        ///     "GUID": "f3f5c3c1-2b1a-489b-9b7f-4b6b8b9f6a0e",
        ///     "ID": "pharm001",
        ///     "Password": "******",
        ///     "Name": "王小藥",
        ///     "Employer": "藥劑科",
        ///     "verifyTime": "2025-11-10 09:35:12",
        ///     "loginTime": "2025-11-10 09:35:12",
        ///     "level": "2",
        ///     "Color": "#FF9933",
        ///     "license": "藥師證字號XXXXXX",
        ///     "Permissions": [ "庫存查詢", "盤點作業", "交班對點" ]
        ///   }
        /// }
        /// </code>
        ///
        /// <para><b>錯誤回應範例</b>：</para>
        /// <list type="table">
        ///   <listheader>
        ///     <term>Code</term>
        ///     <description>說明 / Result</description>
        ///   </listheader>
        ///   <item>
        ///     <term>-1</term>
        ///     <description>找無此帳號或透過 UID/BARCODE 找無資料（例如：<c>此UID找無資料!</c> / <c>此一維條碼找無資料!</c> / <c>找無此帳號!</c>）。</description>
        ///   </item>
        ///   <item>
        ///     <term>-2</term>
        ///     <description>密碼錯誤（<c>密碼錯誤!</c>）。</description>
        ///   </item>
        ///   <item>
        ///     <term>-200</term>
        ///     <description>系統或資料庫連線錯誤（含找無資料庫參數、例外訊息等）。</description>
        ///   </item>
        ///   <!-- 若未來啟用重複登入阻擋，可回覆：
        ///   <item>
        ///     <term>-3</term>
        ///     <description>已登入帳號，需登出（<c>已登入帳號,需登出!</c>）。</description>
        ///   </item>
        ///   -->
        /// </list>
        ///
        /// <para><b>資料表需求</b>：</para>
        /// <list type="bullet">
        ///   <item><description><c>person_page</c>：欄位需包含 <c>ID</c>、<c>密碼</c>、<c>姓名</c>、<c>單位</c>、<c>卡號(UID)</c>、<c>一維條碼(BARCODE)</c>、<c>權限等級</c>、<c>顏色</c>、<c>藥師證字號</c> 等。</description></item>
        ///   <item><description><c>login_session</c>：欄位需包含 <c>GUID</c>、<c>ID</c>、<c>Name</c>、<c>Employer</c>、<c>verifyTime</c>、<c>loginTime</c> 等。</description></item>
        /// </list>
        ///
        /// <para><b>安全性與實作建議</b>：</para>
        /// <list type="bullet">
        ///   <item><description>傳輸中的 <c>Password</c>（或 Credential）不應為明碼；建議使用雜湊（如 PBKDF2/BCrypt/Argon2）搭配 TLS。</description></item>
        ///   <item><description>admin 最高權限建議以環境變數或安全儲存管理，避免硬編碼；或在正式環境停用此後門。</description></item>
        ///   <item><description><c>GUID</c> 建議於每次驗證更新，以利稽核追蹤；若需單一登入（SSO/踢人機制），可在建立 session 前檢查舊紀錄並視需求拒絕或覆蓋。</description></item>
        /// </list>
        /// </remarks>
        /// <param name="returnData">
        ///  共用資料結構，<c>Data</c> 需包含以下任一組：
        ///  <list type="bullet">
        ///    <item><description><c>ID</c> + <c>Password</c></description></item>
        ///    <item><description><c>UID</c>（系統將回填 <c>ID</c>/<c>Password</c> 後驗證）</description></item>
        ///    <item><description><c>BARCODE</c>（系統將回填 <c>ID</c>/<c>Password</c> 後驗證）</description></item>
        ///  </list>
        ///  可選：<c>ServerName</c>、<c>ServerType</c> 決定 DB 連線來源。
        /// </param>
        /// <returns>
        ///  JSON 字串：<c>returnData</c> 物件序列化結果。成功時 <c>Code=200</c> 並於 <c>Data</c> 回傳 <c>sessionClass</c>；失敗時回傳對應錯誤碼與訊息。
        /// </returns>
        [Route("login")]
        [HttpPost]
        public string POST_login([FromBody] returnData returnData)
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
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "人員資料");
                }
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無資料庫參數!";
                    return returnData.JsonSerializationt();
                }
                string IP = sys_serverSettingClasses[0].Server;
                string DataBaseName = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                bool flag_admin = false;

                SQLControl sQLControl_login_session = new SQLControl(IP, DataBaseName, "login_session", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_person_page = new SQLControl(IP, DataBaseName, "person_page", UserName, Password, Port, SSLMode);

                CheckCreatTable(sys_serverSettingClasses[0]);
                sessionClass data = returnData.Data.ObjToClass<sessionClass>();
                List<object[]> list_login_session = sQLControl_login_session.GetAllRows(null);
                List<object[]> list_person_page = sQLControl_person_page.GetAllRows(null);
                List<object[]> list_login_session_add = new List<object[]>();
                List<object[]> list_login_session_replace = new List<object[]>();
                sessionClass sessionClass = new sessionClass();
                if (data.ID.StringIsEmpty() == false && data.Password.StringIsEmpty() == false)
                {
                    if (data.ID.ToUpper() == "admin".ToUpper() && data.Password.ToUpper() == "66437068")
                    {
                        flag_admin = true;
                    }
                }
            
                if (flag_admin == false)
                {
                    if (data.UID.StringIsEmpty() == false)
                    {
                        list_person_page = list_person_page.GetRows((int)enum_人員資料.卡號, data.UID);
                        if (list_person_page.Count == 0)
                        {
                            returnData.Code = -1;
                            returnData.Result = "此UID找無資料!";
                            return returnData.JsonSerializationt();
                        }
                        data.ID = list_person_page[0][(int)enum_人員資料.ID].ObjectToString();
                        data.Password = list_person_page[0][(int)enum_人員資料.密碼].ObjectToString();
                    }
                    else if (data.BARCODE.StringIsEmpty() == false)
                    {
                        list_person_page = list_person_page.GetRows((int)enum_人員資料.一維條碼, data.BARCODE);
                        if (list_person_page.Count == 0)
                        {
                            returnData.Code = -1;
                            returnData.Result = "此一維條碼找無資料!";
                            return returnData.JsonSerializationt();
                        }
                        data.ID = list_person_page[0][(int)enum_人員資料.ID].ObjectToString();
                        data.Password = list_person_page[0][(int)enum_人員資料.密碼].ObjectToString();
                    }
                    list_person_page = list_person_page.GetRows((int)enum_人員資料.ID, data.ID);
                    if (list_person_page.Count == 0)
                    {
                        returnData.Code = -1;
                        returnData.Result = "找無此帳號!";
                        return returnData.JsonSerializationt();
                    }
                    if (list_person_page[0][(int)enum_人員資料.密碼].ObjectToString() != data.Password)
                    {
                        returnData.Code = -2;
                        returnData.Result = "密碼錯誤!";
                        return returnData.JsonSerializationt();
                    }
                    list_login_session = list_login_session.GetRows((int)enum_login_session.ID, data.ID);

                    object[] value = new object[new enum_login_session().GetLength()];
                    if (list_login_session.Count == 0)
                    {
                        value = new object[new enum_login_session().GetLength()];
                        value[(int)enum_login_session.GUID] = Guid.NewGuid().ToString();
                        value[(int)enum_login_session.ID] = list_person_page[0][(int)enum_人員資料.ID].ObjectToString();
                        value[(int)enum_login_session.Name] = list_person_page[0][(int)enum_人員資料.姓名].ObjectToString();
                        value[(int)enum_login_session.Employer] = list_person_page[0][(int)enum_人員資料.單位].ObjectToString();
                        value[(int)enum_login_session.verifyTime] = DateTime.Now.ToDateTimeString();
                        value[(int)enum_login_session.loginTime] = DateTime.Now.ToDateTimeString();
                        list_login_session_add.Add(value);
                    }
                    else
                    {
                        value = list_login_session[0];
                        value[(int)enum_login_session.GUID] = Guid.NewGuid().ToString();
                        value[(int)enum_login_session.ID] = list_person_page[0][(int)enum_人員資料.ID].ObjectToString();
                        value[(int)enum_login_session.Name] = list_person_page[0][(int)enum_人員資料.姓名].ObjectToString();
                        value[(int)enum_login_session.Employer] = list_person_page[0][(int)enum_人員資料.單位].ObjectToString();
                        value[(int)enum_login_session.verifyTime] = DateTime.Now.ToDateTimeString();
                        value[(int)enum_login_session.loginTime] = DateTime.Now.ToDateTimeString();
                        list_login_session_replace.Add(value);
                    }
                    if (list_login_session_add.Count > 0) sQLControl_login_session.AddRows(null, list_login_session_add);
                    if (list_login_session_replace.Count > 0) sQLControl_login_session.UpdateByDefulteExtra(null, list_login_session_replace);


                    sessionClass.GUID = value[(int)enum_login_session.GUID].ObjectToString();
                    sessionClass.ID = list_person_page[0][(int)enum_人員資料.ID].ObjectToString();
                    sessionClass.Password = list_person_page[0][(int)enum_人員資料.密碼].ObjectToString();
                    sessionClass.Name = list_person_page[0][(int)enum_人員資料.姓名].ObjectToString();
                    sessionClass.Employer = list_person_page[0][(int)enum_人員資料.單位].ObjectToString();
                    sessionClass.verifyTime = value[(int)enum_login_session.verifyTime].ObjectToString();
                    sessionClass.loginTime = value[(int)enum_login_session.loginTime].ObjectToString();
                    sessionClass.level = list_person_page[0][(int)enum_人員資料.權限等級].ObjectToString();
                    sessionClass.Color = list_person_page[0][(int)enum_人員資料.顏色].ObjectToString();
                    sessionClass.license = list_person_page[0][(int)enum_人員資料.藥師證字號].ObjectToString();
                    sessionClass.Permissions = GetPermissions(sessionClass.level.StringToInt32(), returnData.ServerName, returnData.ServerType);
                }
                else
                {
                    sessionClass.GUID = "";
                    sessionClass.ID = "admin";
                    sessionClass.Password = "66437068";
                    sessionClass.Name = "最高管理權限";
                    sessionClass.Employer = "";
                    sessionClass.verifyTime = DateTime.Now.ToDateTimeString();
                    sessionClass.loginTime = DateTime.Now.ToDateTimeString();
                    sessionClass.level = "-1";
                    sessionClass.Color = System.Drawing.Color.Red.ToColorString();
                    sessionClass.Permissions = GetPermissions(sessionClass.level.StringToInt32(), returnData.ServerName, returnData.ServerType);
                }





                returnData.Data = sessionClass;

                //if (list_login_session.Count > 0)
                //{
                //    returnData.Code = -3;
                //    returnData.Result = "已登入帳號,需登出!";
                //    return returnData.JsonSerializationt();
                //}



                returnData.Code = 200;
                returnData.Result = "登入成功!";
                return returnData.JsonSerializationt();
            }
            catch(Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
            
        }

        [Route("logout")]
        [HttpPost]
        public string POST_logout([FromBody] returnData returnData)
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
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "人員資料");
                }
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無資料庫參數!";
                    return returnData.JsonSerializationt();
                }
                string IP = sys_serverSettingClasses[0].Server;
                string DataBaseName = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_login_session = new SQLControl(IP, DataBaseName, "login_session", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_person_page = new SQLControl(IP, DataBaseName, "person_page", UserName, Password, Port, SSLMode);

                CheckCreatTable(sys_serverSettingClasses[0]);
                sessionClass sessionClass = returnData.Data.ObjToClass<sessionClass>();
                List<object[]> list_login_session = sQLControl_login_session.GetAllRows(null);
                list_login_session = list_login_session.GetRows((int)enum_login_session.ID, sessionClass.ID);
                if (list_login_session.Count > 0)
                {
                    sQLControl_login_session.DeleteExtra(null, list_login_session);
                    returnData.Code = 200;
                    returnData.Result = $"ID :{sessionClass.ID} ,清除session成功!";
                    return returnData.JsonSerializationt();
                }
                returnData.Code = 200;
                returnData.Result = $"ID :{sessionClass.ID} ,找無此session!";
                return returnData.JsonSerializationt();

            }
            catch(Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
           

        }

        [Route("check_session")]
        [HttpPost]
        public string POST_check([FromBody] returnData returnData)
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
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "人員資料");
                }
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無資料庫參數!";
                    return returnData.JsonSerializationt();
                }
                string IP = sys_serverSettingClasses[0].Server;
                string DataBaseName = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_login_session = new SQLControl(IP, DataBaseName, "login_session", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_person_page = new SQLControl(IP, DataBaseName, "person_page", UserName, Password, Port, SSLMode);

                                CheckCreatTable(sys_serverSettingClasses[0]);

                sessionClass sessionClass = returnData.Data.ObjToClass<sessionClass>();
                List<object[]> list_login_session = sQLControl_login_session.GetAllRows(null);
                list_login_session = list_login_session.GetRows((int)enum_login_session.ID, sessionClass.ID);
                if (list_login_session.Count == 0)
                {
                    returnData.Result = "其他裝置登入,即將登出!";
                    returnData.Code = -2;
                    return returnData.JsonSerializationt();
                }
                else
                {

                }
                DateTime loginTime = sessionClass.loginTime.StringToDateTime().StringToDateTime();
                if (sessionClass.loginTime.StringToDateTime().ToDateTimeString() != list_login_session[0][(int)enum_login_session.loginTime].StringToDateTime().ToDateTimeString())
                {
                    returnData.Result = "其他裝置登入,即將登出!";
                    returnData.Code = -2;
                    return returnData.JsonSerializationt();
                }
                DateTime verifyTime = list_login_session[0][(int)enum_login_session.verifyTime].StringToDateTime();


                TimeSpan timeDifference = DateTime.Now.Subtract(verifyTime);
                double secondsDifference = timeDifference.TotalSeconds;
                if (secondsDifference >= sessionClass.check_sec.StringToInt32())
                {
                    returnData.Result = "驗證逾時,即將登出!";
                    returnData.Code = -1;
                    return returnData.JsonSerializationt();
                }
                list_login_session[0][(int)enum_login_session.verifyTime] = DateTime.Now.ToDateTimeString();
                sQLControl_login_session.UpdateByDefulteExtra(null, list_login_session);

                returnData.Result = "驗證成功!";
                returnData.Code = 200;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
           
        }

        [Route("update_session")]
        [HttpPost]
        public string POST_update_session([FromBody] returnData returnData)
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
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "人員資料");
                }
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無資料庫參數!";
                    return returnData.JsonSerializationt();
                }
                string IP = sys_serverSettingClasses[0].Server;
                string DataBaseName = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_login_session = new SQLControl(IP, DataBaseName, "login_session", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_person_page = new SQLControl(IP, DataBaseName, "person_page", UserName, Password, Port, SSLMode);

                                CheckCreatTable(sys_serverSettingClasses[0]);

                sessionClass sessionClass = returnData.Data.ObjToClass<sessionClass>();
                List<object[]> list_login_session = sQLControl_login_session.GetAllRows(null);
                list_login_session = list_login_session.GetRows((int)enum_login_session.ID, sessionClass.ID);
                if (list_login_session.Count == 0)
                {
                    returnData.Result = "找無session!";
                    returnData.Code = -2;
                    return returnData.JsonSerializationt();
                }
                list_login_session[0][(int)enum_login_session.GUID] = sessionClass.GUID;
                returnData.Result = "更新session完成!";
                returnData.Code = 200;
                return returnData.JsonSerializationt();
            }
            catch(Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
           
        }

        [Route("get_permissions")]
        [HttpPost]
        public string POST_get_permissions([FromBody] returnData returnData)
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
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "人員資料");
                }
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無資料庫參數!";
                    return returnData.JsonSerializationt();
                }
                string IP = sys_serverSettingClasses[0].Server;
                string DataBaseName = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_login_session = new SQLControl(IP, DataBaseName, "login_session", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_person_page = new SQLControl(IP, DataBaseName, "person_page", UserName, Password, Port, SSLMode);
                                CheckCreatTable(sys_serverSettingClasses[0]);
                sessionClass data = returnData.Data.ObjToClass<sessionClass>();
                List<object[]> list_login_session = sQLControl_login_session.GetAllRows(null);
                List<object[]> list_person_page = sQLControl_person_page.GetAllRows(null);
                List<object[]> list_login_session_add = new List<object[]>();
                List<object[]> list_login_session_replace = new List<object[]>();

                list_person_page = list_person_page.GetRows((int)enum_人員資料.ID, data.ID);
                if (list_person_page.Count == 0)
                {
                    returnData.Code = -1;
                    returnData.Result = "找無此帳號!";
                    return returnData.JsonSerializationt();
                }
                if (list_person_page[0][(int)enum_人員資料.密碼].ObjectToString() != data.Password)
                {
                    returnData.Code = -2;
                    returnData.Result = "密碼錯誤!";
                    return returnData.JsonSerializationt();
                }
                sessionClass sessionClass = new sessionClass();
                sessionClass.ID = list_person_page[0][(int)enum_人員資料.ID].ObjectToString();
                sessionClass.Password = list_person_page[0][(int)enum_人員資料.密碼].ObjectToString();
                sessionClass.Name = list_person_page[0][(int)enum_人員資料.姓名].ObjectToString();
                sessionClass.Employer = list_person_page[0][(int)enum_人員資料.單位].ObjectToString();
                sessionClass.level = list_person_page[0][(int)enum_人員資料.權限等級].ObjectToString();
                sessionClass.Color = list_person_page[0][(int)enum_人員資料.顏色].ObjectToString();
                sessionClass.Permissions = GetPermissions(sessionClass.level.StringToInt32(), returnData.ServerName, returnData.ServerType);



                returnData.Data = sessionClass;

                returnData.Code = 200;
                returnData.Result = "取得權限成功!";
                return returnData.JsonSerializationt();
            }
            catch(Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
           
        }
        /// <summary>
        /// 以權限等級取得資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "ValueAry":["權限等級"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_login_data_index")]
        [HttpPost]
        public string get_login_data_index([FromBody] returnData returnData)
        {
            try
            {
                
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                returnData.Method = "get_login_data_index";
                string ServerName = returnData.ServerName;
                string ServerType = returnData.ServerType;
                string Content = "人員資料";
                if (ServerName.StringIsEmpty())
                {
                    ServerName = "Main";
                    ServerType = "網頁";
                    Content = "VM端";
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo(ServerName, ServerType, Content);
                SQLControl sQLControl_login_data_index = new SQLControl(Server, DB, "login_data_index", UserName, Password, Port, SSLMode);
                List<object[]> login_data_index = sQLControl_login_data_index.GetAllRows(null);
                List<loginDataIndexClass> loginDataIndexClasses = login_data_index.SQLToClass<loginDataIndexClass, enum_login_data_index>();
                loginDataIndexClasses.Sort(new loginDataIndexClass.ICP_By_index());
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = loginDataIndexClasses;
                returnData.Result = $"取得權限設定，共{loginDataIndexClasses.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新權限資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": [
        ///     {
        ///         "name":"交班對點頁面"
        ///         "index":0,
        ///         "type":"調劑台",
        ///         "state": true or false
        ///     }],
        ///     "ValueAry":["權限等級"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("update_login_data_index")]
        [HttpPost]
        public string update_login_data_index([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                string ServerName = returnData.ServerName;
                string ServerType = returnData.ServerType;
                string Content = "人員資料";
                if (ServerName.StringIsEmpty())
                {
                    ServerName = "Main";
                    ServerType = "網頁";
                    Content = "VM端";
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo(ServerName, ServerType, Content);
                SQLControl sQLControl_login_data_index = new SQLControl(Server, DB, "login_data_index", UserName, Password, Port, SSLMode);
                List<loginDataIndexClass> input_loginDataIndex = returnData.Data.ObjToClass<List<loginDataIndexClass>>();

                List<object[]> loginDataIndex = sQLControl_login_data_index.GetAllRows(null);
                List<loginDataIndexClass> sql_loginDataIndex = loginDataIndex.SQLToClass<loginDataIndexClass, enum_login_data_index>();
                List<loginDataIndexClass> update_loginDataIndex = new List<loginDataIndexClass>();
                List<loginDataIndexClass> add_loginDataIndex = new List<loginDataIndexClass>();


                foreach (var item in input_loginDataIndex)
                {
                    string index = item.索引;
                    loginDataIndexClass loginDataIndexClass = sql_loginDataIndex.Where(temp => temp.索引 == index).FirstOrDefault();
                    if(loginDataIndexClass != null)
                    {
                        loginDataIndexClass.Name = item.Name;
                        loginDataIndexClass.Type = item.Type;
                        loginDataIndexClass.群組 = item.群組;
                        loginDataIndexClass.描述 = item.描述;

                        update_loginDataIndex.Add(loginDataIndexClass);
                    }
                    else
                    {
                        loginDataIndexClass.GUID = Guid.NewGuid().ToString();
                        loginDataIndexClass.Name = item.Name;
                        loginDataIndexClass.Type = item.Type;
                        loginDataIndexClass.群組 = item.群組;
                        loginDataIndexClass.描述 = item.描述;

                        add_loginDataIndex.Add(loginDataIndexClass);
                    }
                }


                List<object[]> update = update_loginDataIndex.ClassToSQL<loginDataIndexClass, enum_login_data_index>();
                List<object[]> add = add_loginDataIndex.ClassToSQL<loginDataIndexClass, enum_login_data_index>();

                if(update.Count > 0) sQLControl_login_data_index.UpdateByDefulteExtra(null, update);
                if (add.Count > 0) sQLControl_login_data_index.AddRows(null, add);
                add_loginDataIndex.AddRange(update_loginDataIndex);
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = add_loginDataIndex;
                returnData.Result = $"更新權限表，共{add_loginDataIndex.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以權限等級和類別取得資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "ValueAry":["權限等級","類別(調劑台\藥庫\網頁)"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_setting_by_type")]
        [HttpPost]
        public string get_setting_by_type([FromBody] returnData returnData)
        {
            try
            {
                init_login_data_index(returnData);
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry不得為空";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry應為[\"權限等級\",\"調劑台or藥庫\"]";
                    return returnData.JsonSerializationt(true);
                }
                string ServerName = returnData.ServerName;
                string ServerType = returnData.ServerType;
                string Content = "人員資料";
                if (ServerName.StringIsEmpty())
                {
                    ServerName = "Main";
                    ServerType = "網頁";
                    Content = "VM端";
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo(ServerName, ServerType, Content); 
                int level = returnData.ValueAry[0].StringToInt32();
                string 類別 = returnData.ValueAry[1];
                List<PermissionsClass> PermissionsClasses = GetPermissions(level, ServerName, ServerType);

                PermissionsClasses = PermissionsClasses.Where(item => item.類別 == 類別).ToList();

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = PermissionsClasses;
                returnData.Result = $"取得{類別}權限設定，共{PermissionsClasses.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新權限資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": [
        ///     {
        ///         "name":"交班對點頁面"
        ///         "index":0,
        ///         "type":"調劑台",
        ///         "state": true or false
        ///     }],
        ///     "ValueAry":["權限等級"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("update_setting")]
        [HttpPost]
        public string update_setting([FromBody] returnData returnData)
        {
            try
            {

                MyTimerBasic myTimerBasic = new MyTimerBasic();
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry不得為空";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry應為[\"權限等級\"]";
                    return returnData.JsonSerializationt(true);
                }
                if(returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data不得為空";
                    return returnData.JsonSerializationt(true);
                }
                List<PermissionsClass> update_permiss = returnData.Data.ObjToClass<List<PermissionsClass>>();
                string ServerName = returnData.ServerName;
                string ServerType = returnData.ServerType;
                string Content = "人員資料";
                if (ServerName.StringIsEmpty())
                {
                    ServerName = "Main";
                    ServerType = "網頁";
                    Content = "VM端";
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo(ServerName, ServerType, Content);
                string level = returnData.ValueAry[0];
                List<PermissionsClass> PermissionsClasses = GetPermissions(level.StringToInt32(), ServerName,ServerType);

                foreach(var item in PermissionsClasses)
                {
                    PermissionsClass permissionsClasses = update_permiss.Where(temp => temp.索引 == item.索引).FirstOrDefault();
                    if(permissionsClasses != null)
                    {
                        item.狀態 = permissionsClasses.狀態;
                    }                        
                }
                List<loginDataClass> loginDataClasses = HIS_DB_Lib.loginDataClass.get_permission_index(API);
                loginDataClass loginData = loginDataClasses.Where(item => item.權限等級 == level).FirstOrDefault();
                if(loginData == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無{level}權限";
                    return returnData.JsonSerializationt(true);
                }
                List<loginDataClass> loginDataClass = PackPermissionBitsToLongs(loginData, PermissionsClasses);
                List<object[]> update_login_data = loginDataClass.ClassToSQL<loginDataClass, enum_login_data>();
                SQLControl sQLControl_login_data = new SQLControl(Server, DB, "login_data", UserName, Password, Port, SSLMode);
                sQLControl_login_data.UpdateByDefulteExtra(null, update_login_data);
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = update_permiss;
                returnData.Result = $"更新權限設定，共{update_permiss.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得權限
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_permission_index")]
        [HttpPost]
        public string get_permission_index([FromBody] returnData returnData)
        {
            try
            {
                
                MyTimerBasic myTimerBasic = new MyTimerBasic();

                string ServerName = returnData.ServerName;
                string ServerType = returnData.ServerType;
                string Content = "人員資料";
                if (ServerName.StringIsEmpty())
                {
                    ServerName = "Main";
                    ServerType = "網頁";
                    Content = "VM端";
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo(ServerName, ServerType, Content);
                SQLControl sQLControl_login_data = new SQLControl(Server, DB, "login_data", UserName, Password, Port, SSLMode);
                List<object[]> session_data = sQLControl_login_data.GetAllRows(null);
                List<loginDataClass> loginDataClasses = session_data.SQLToClass<loginDataClass, enum_login_data>();
                

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = loginDataClasses;
                returnData.Result = $"取得權限表單，共{loginDataClasses.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch(Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        private List<PermissionsClass> GetPermissions(int level ,string serverName , string serverType)
        {
            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            if (serverName.StringIsEmpty() || serverType.StringIsEmpty())
            {
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
            }
            else
            {
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(serverName, serverType, "人員資料");
            }
            if (sys_serverSettingClasses.Count == 0)
            {
                return new List<PermissionsClass>();
            }
            string IP = sys_serverSettingClasses[0].Server;
            string DataBaseName = sys_serverSettingClasses[0].DBName;
            string UserName = sys_serverSettingClasses[0].User;
            string Password = sys_serverSettingClasses[0].Password;
            uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

            List<PermissionsClass> result = new List<PermissionsClass>();
            SQLControl sQLControl_login_data_index = new SQLControl(IP, DataBaseName, "login_data_index", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_login_data = new SQLControl(IP, DataBaseName, "login_data", UserName, Password, Port, SSLMode);
            List<MySQL_Login.LoginDataWebAPI.Class_login_data> list_class_login_data = MySQL_Login.LoginDataWebAPI.Get_login_data(sQLControl_login_data);
            List<object[]> login_data_index = sQLControl_login_data_index.GetAllRows(null);
            List<object[]> login_data_index_buf = new List<object[]>();
            if(level == -1)
            {
                list_class_login_data = new List<MySQL_Login.LoginDataWebAPI.Class_login_data>();
                MySQL_Login.LoginDataWebAPI.Class_login_data class_Login_Data = new MySQL_Login.LoginDataWebAPI.Class_login_data();
                for(int i = 0; i < class_Login_Data.data.Count; i++)
                {
                    class_Login_Data.data[i] = true;
                }

                list_class_login_data.Add(class_Login_Data);
            }
            if (list_class_login_data.Count > 0)
            {
                list_class_login_data = (from value in list_class_login_data
                                         where value.level.StringToInt32().ToString() == level.StringToInt32().ToString()
                                         select value).ToList();
                if (list_class_login_data.Count == 0) return new List<PermissionsClass>();
                for (int i = 0; i < list_class_login_data[0].data.Count; i++)
                {
                    login_data_index_buf = login_data_index.GetRows((int)MySQL_Login.LoginDataWebAPI.enum_login_data_index.索引, i.ToString("00"));
                    if (list_class_login_data[0].data[i])
                    {
                        
                        if (login_data_index_buf.Count > 0)
                        {
                            PermissionsClass permissionsClass = new PermissionsClass();
                            permissionsClass.名稱 = login_data_index_buf[0][(int)MySQL_Login.LoginDataWebAPI.enum_login_data_index.Name].ObjectToString();
                            permissionsClass.類別 = login_data_index_buf[0][(int)MySQL_Login.LoginDataWebAPI.enum_login_data_index.Type].ObjectToString();
                            permissionsClass.索引 = i;
                            permissionsClass.狀態 = true;
                            permissionsClass.群組 = login_data_index_buf[0][(int)enum_login_data_index.群組].ObjectToString();
                            permissionsClass.描述 = login_data_index_buf[0][(int)enum_login_data_index.描述].ObjectToString();

                            result.Add(permissionsClass);
                        }
                    }
                    else
                    {
                        if (login_data_index_buf.Count > 0)
                        {
                            PermissionsClass permissionsClass = new PermissionsClass();
                            permissionsClass.名稱 = login_data_index_buf[0][(int)MySQL_Login.LoginDataWebAPI.enum_login_data_index.Name].ObjectToString();
                            permissionsClass.類別 = login_data_index_buf[0][(int)MySQL_Login.LoginDataWebAPI.enum_login_data_index.Type].ObjectToString();
                            permissionsClass.索引 = i;
                            permissionsClass.狀態 = false;
                            permissionsClass.群組 = login_data_index_buf[0][(int)enum_login_data_index.群組].ObjectToString();
                            permissionsClass.描述 = login_data_index_buf[0][(int)enum_login_data_index.描述].ObjectToString();
                            result.Add(permissionsClass);
                        }
                    }

                }
            }
            return result;
        }
        private List<loginDataClass> PackPermissionBitsToLongs(loginDataClass loginDataClass,List<PermissionsClass> permissions)
        {
            bool[] data = new bool[256];

            foreach (var permission in permissions)
            {
                int index = permission.索引;
                if (index >= 0 && index < 256)
                {
                    data[index] = permission.狀態;
                }
            }

            List<long> packedLongs = new List<long>();

            for (int section = 0; section < 4; section++)
            {
                long value = 0;
                for (int bit = 0; bit < 64; bit++)
                {
                    if (data[section * 64 + bit])
                    {
                        value |= 1L << bit;
                    }
                }
                packedLongs.Add(value);
            }

            loginDataClass loginData = new loginDataClass
            {
                GUID = loginDataClass.GUID,
                權限等級 = loginDataClass.權限等級,
                Data01 = packedLongs[0].ToString(),
                Data02 = packedLongs[1].ToString(),
                Data03 = packedLongs[2].ToString(),
                Data04 = packedLongs[3].ToString()
            };

            return new List<loginDataClass> { loginData };
        }

    
        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_login_session()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_login_data_index()));
            return tables.JsonSerializationt(true);
        }
        private void loadData()
        {
            string data = Basic.MyFileStream.LoadFileAllText(@"./login_data_index.txt", "utf-8");
            //string loadText = Basic.MyFileStream.LoadFileAllText(@"./excel_emg_tradding.txt", "utf-8");
            returnData returnData = data.JsonDeserializet<returnData>();
            update_login_data_index(returnData);
        }

    }
}
