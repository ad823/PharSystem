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
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerSettingController : Controller
    {
        static private string Server = ConfigurationManager.AppSettings["server"];
        static private uint Port = (uint)ConfigurationManager.AppSettings["port"].StringToInt32();
        static private string UserName = ConfigurationManager.AppSettings["user"];
        static private string Password = ConfigurationManager.AppSettings["password"];
        static private string DB = "dbvm";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        /// <summary>
        /// 取得所有連線資訊
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpGet]
        public string GET()
        {
            returnData returnData = new returnData();
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            returnData.Method = "get";

            try
            {
                this.CheckCreatTable();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl.GetAllRows(null);

                List<sys_serverSettingClass> sys_serverSettingClasses = list_value.SQLToClass<sys_serverSettingClass, enum_sys_serverSetting>();

                returnData.Code = 200;
                returnData.Data = sys_serverSettingClasses;
                returnData.Result = $"取得伺服器設定成功!共<{sys_serverSettingClasses.Count}>筆";
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 初始化資料庫
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        /// 
        [Route("init")]
        [HttpGet]
        public string GET_init()
        {
            returnData returnData = new returnData();
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "GET_init";
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }
            try
            {
                return CheckCreatTable();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt();
            }

        }
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
        [Route("post_init")]
        [HttpPost]
        public string POST_init([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);

            try
            {
                try
                {
                    returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
                }
                catch
                {

                }
                returnData.Method = "POST_init";

                return CheckCreatTable();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得Type(調劑台、藥庫...)
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("type")]
        [HttpGet]
        public string GET_type()
        {
            return new enum_sys_serverSetting_Type().GetEnumNames().JsonSerializationt();
        }
        /// <summary>
        /// 取得程式類別(SQLServer、API、WEB...)
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("program")]
        [HttpGet]
        public string GET_program()
        {
            return new enum_sys_serverSetting_ProgramType().GetEnumNames().JsonSerializationt();
        }
        /// <summary>
        /// 新增連線資訊
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        [List<sys_serverSettingClasses>]
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("add")]
        [HttpPost]
        public string POST_add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }
            returnData.Method = "add";
            try
            {
                this.CheckCreatTable();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl.GetAllRows(null);
                List<object[]> list_value_returnData = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();

                List<sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();
                list_value_returnData = sys_serverSettingClasses.ClassToSQL<sys_serverSettingClass, enum_sys_serverSetting>();
                for (int i = 0; i < list_value_returnData.Count; i++)
                {
                    string 名稱 = list_value_returnData[i][(int)enum_sys_serverSetting.設備名稱].ObjectToString();
                    string 類別 = list_value_returnData[i][(int)enum_sys_serverSetting.類別].ObjectToString();
                    string 程式類別 = list_value_returnData[i][(int)enum_sys_serverSetting.程式類別].ObjectToString();
                    string 內容 = list_value_returnData[i][(int)enum_sys_serverSetting.內容].ObjectToString();

                    list_value_buf = list_value.GetRows((int)enum_sys_serverSetting.設備名稱, 名稱);
                    list_value_buf = list_value_buf.GetRows((int)enum_sys_serverSetting.類別, 類別);
                    list_value_buf = list_value_buf.GetRows((int)enum_sys_serverSetting.程式類別, 程式類別);
                    list_value_buf = list_value_buf.GetRows((int)enum_sys_serverSetting.內容, 內容);
                    if (list_value_buf.Count == 0)
                    {
                        object[] value = list_value_returnData[i];
                        value[(int)enum_sys_serverSetting.GUID] = Guid.NewGuid().ToString();
                        list_value_add.Add(value);
                    }
                    else
                    {
                        object[] value = list_value_returnData[i];
                        value[(int)enum_sys_serverSetting.GUID] = list_value_buf[0][(int)enum_sys_serverSetting.GUID].ObjectToString();
                        list_value_replace.Add(value);
                    }
                }
                sQLControl.AddRows(null, list_value_add);
                sQLControl.UpdateByDefulteExtra(null, list_value_replace);

                returnData.Code = 200;
                returnData.Result = "新增伺服器資料成功!";
                returnData.Data = sys_serverSettingClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 刪除連線資訊
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        [List<sys_serverSettingClasses>]
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("delete")]
        [HttpPost]
        public string POST_delete([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }
            returnData.Method = "delete";
            try
            {
                this.CheckCreatTable();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl.GetAllRows(null);
                List<object[]> list_value_returnData = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();
                List<sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();
                list_value_returnData = sys_serverSettingClasses.ClassToSQL<sys_serverSettingClass, enum_sys_serverSetting>();

                sQLControl.DeleteExtra(null, list_value_returnData);

                returnData.Code = 200;
                returnData.Result = "刪除伺服器資料成功!";
                returnData.Data = sys_serverSettingClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以Type取得連線資訊(調劑台、藥庫...)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "調劑台"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_serversetting_by_type")]
        [HttpPost]
        public string POST_get_serversetting_by_type([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }
            returnData.Method = "get_serversetting_by_type";
            try
            {
                this.CheckCreatTable();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl.GetAllRows(null);
                List<object[]> list_value_returnData = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[Type]";
                    return returnData.JsonSerializationt(true);
                }
                string Type = returnData.ValueAry[0];

                List<sys_serverSettingClass> sys_serverSettingClasses = GetAllServerSetting();
                sys_serverSettingClasses = (from temp in sys_serverSettingClasses
                                            where temp.類別 == Type
                                            where temp.內容 == "一般資料"
                                            select temp).ToList();

                sys_serverSettingClasses.Sort(new sys_serverSettingClass.ICP_By_dps_name());
                returnData.Code = 200;
                returnData.Result = $"取得連線資訊,共<{sys_serverSettingClasses.Count}>筆";
                returnData.Data = sys_serverSettingClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 取得 調劑台、藥庫 設備名稱
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "調劑台"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_name")]
        [HttpPost]
        public string get_name([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }
            returnData.Method = "get_name";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.Where(temp => (temp.類別 == "調劑台" || temp.類別 == "藥庫") && temp.內容 == "一般資料").ToList();
                sys_serverSettingClasses.Sort(new sys_serverSettingClass.ICP_By_dps_name());
                returnData.Code = 200;
                returnData.Result = $"取得連線資訊,共<{sys_serverSettingClasses.Count}>筆";
                returnData.Data = sys_serverSettingClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 取得指定Type(調劑台、藥庫...)伺服器資訊的服務單位
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "調劑台"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_department_type")]
        [HttpPost]
        public string POST_get_department_type([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);

            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }

            returnData.Method = "get_department_type";
            try
            {
                this.CheckCreatTable();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl.GetAllRows(null);
                List<object[]> list_value_returnData = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[Type]";
                    return returnData.JsonSerializationt(true);
                }
                string Type = returnData.ValueAry[0];

                List<sys_serverSettingClass> sys_serverSettingClasses = GetAllServerSetting();
                List<string> department_types = (from temp in sys_serverSettingClasses
                                                 where temp.類別 == Type
                                                 where temp.內容 == "一般資料"
                                                 select temp.單位).Distinct().ToList();

                department_types.Remove("");
                returnData.Code = 200;
                returnData.Result = $"取得伺服器服務端未,共<{department_types.Count}>筆種類";
                returnData.Data = department_types;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 以GUID取得資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "GUID"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_GUID")]
        public async Task<string> get_by_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            returnData.Method = "get_by_GUID";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID]";
                    return returnData.JsonSerializationt(true);
                }
                string[] GUID = returnData.ValueAry[0].Split(";").ToArray();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value = await sQLControl.GetRowsByDefultAsync(null, (int)enum_sys_serverSetting.GUID, GUID);
                List<sys_serverSettingClass> sys_ServerSettingClasses = list_value.SQLToClass<sys_serverSettingClass, enum_sys_serverSetting>();

                returnData.Code = 200;
                returnData.Result = $"取得伺服器服務,共<{sys_ServerSettingClasses.Count}>筆";
                returnData.Data = sys_ServerSettingClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }

        /// <summary>
        /// 以單位別取得連線資訊(門診、急診、住院、藥庫...)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "門診"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_serversetting_by_department_type")]
        [HttpPost]
        public async Task<string> POST_get_serversetting_by_department_type([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }
            returnData.Method = "get_serversetting_by_department_type";
            try
            {
                this.CheckCreatTable();

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[Type]";
                    return returnData.JsonSerializationt(true);
                }
                string department_type = returnData.ValueAry[0];

                List<sys_serverSettingClass> sys_serverSettingClasses = await GetAllServerSettingasync();
                sys_serverSettingClasses = (from temp in sys_serverSettingClasses
                                            where temp.單位 == department_type
                                            where temp.內容 == "一般資料"
                                            select temp).ToList();

                sys_serverSettingClasses.Sort(new sys_serverSettingClass.ICP_By_dps_name());
                returnData.Code = 200;
                returnData.Result = $"取得連線資訊,共<{sys_serverSettingClasses.Count}>筆";
                returnData.Data = sys_serverSettingClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 取得VM伺服器資訊
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_VM_Server")]
        [HttpPost]
        public string POST_get_VM_Server([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);

            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }

            returnData.Method = "get_VM_Server";
            try
            {
                this.CheckCreatTable();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl.GetAllRows(null);
                List<object[]> list_value_returnData = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }

                List<sys_serverSettingClass> sys_serverSettingClasses = GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_VM = (from temp in sys_serverSettingClasses
                                                                            where temp.類別 == "網頁"
                                                                            where temp.內容 == "VM端"
                                                                            select temp).ToList();
                if (sys_serverSettingClasses_VM.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt(true);
                }
                returnData.Code = 200;
                returnData.Result = $"取得伺服器服務端,共<{sys_serverSettingClasses_VM.Count}>筆";
                returnData.Data = sys_serverSettingClasses_VM[0];
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 取得VM伺服器資訊
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       [名稱(ds01)],
        ///       [類別(藥庫)],
        ///       [內容(一般資料)]
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_server")]
        [HttpPost]
        public string POST_get_server([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);

            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }

            returnData.Method = "get_server";
            try
            {
                this.CheckCreatTable();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value_returnData = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }

                List<sys_serverSettingClass> sys_serverSettingClasses = GetAllServerSetting();

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 3)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[名稱],[類別],[內容]";
                    return returnData.JsonSerializationt(true);
                }
                string 類別 = returnData.ValueAry[1].ToUpper();
                string 設備名稱 = returnData.ValueAry[0].ToUpper();
                string 內容 = returnData.ValueAry[2].ToUpper();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = (from temp in sys_serverSettingClasses
                                                                             where temp.類別.ToUpper() == 類別
                                                                             where temp.設備名稱.ToUpper() == 設備名稱
                                                                             where temp.內容.ToUpper() == 內容
                                                                             select temp).ToList();
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt(true);
                }
                returnData.Code = 200;
                returnData.Result = $"取得伺服器服務端,共<{sys_serverSettingClasses_buf.Count}>筆";
                returnData.Data = sys_serverSettingClasses_buf[0];
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 取得網頁模組開啟資訊
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       [盤點單管理模組不啟用],
        ///       [條碼建置模組不啟用],
        ///       [藥品管理模組不啟用]
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_web_peremeter")]
        [Route("et_web_peremeter")]
        [HttpPost]
        public string POST_get_web_peremeter([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);

            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }

            returnData.Method = "et_web_peremeter";
            try
            {
                this.CheckCreatTable();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value_returnData = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }

                List<sys_serverSettingClass> sys_serverSettingClasses = GetAllServerSetting();

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容空白";
                    return returnData.JsonSerializationt(true);
                }
                string 類別 = "網頁";
                List<sys_serverSettingClass> sys_serverSettingClasses_temp = (from temp in sys_serverSettingClasses
                                                                              where temp.類別.ToUpper() == 類別
                                                                              where temp.程式類別.ToUpper() == "peremeter".ToUpper()
                                                                              select temp).ToList();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = new List<sys_serverSettingClass>();
                for (int i = 0; i < sys_serverSettingClasses_temp.Count; i++)
                {
                    foreach (string str_temp in returnData.ValueAry)
                    {
                        if (sys_serverSettingClasses_temp[i].內容 == str_temp)
                        {
                            sys_serverSettingClasses_buf.Add(sys_serverSettingClasses_temp[i]);
                        }
                    }
                }
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt(true);
                }
                returnData.Code = 200;
                returnData.Result = $"取得伺服器服務端,共<{sys_serverSettingClasses_buf.Count}>筆";
                returnData.Data = sys_serverSettingClasses_buf;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 取得網頁模組參數名稱
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_web_peremeter_name")]
        [HttpPost]
        public string POST_get_web_peremeter_name([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);

            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }

            returnData.Method = "get_web_peremeter_name";
            try
            {
                this.CheckCreatTable();
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                List<object[]> list_value_returnData = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }

                List<sys_serverSettingClass> sys_serverSettingClasses = GetAllServerSetting();

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string 類別 = "網頁";
                List<sys_serverSettingClass> sys_serverSettingClasses_temp = (from temp in sys_serverSettingClasses
                                                                              where temp.類別.ToUpper() == 類別
                                                                              where temp.程式類別.ToUpper() == "peremeter".ToUpper()
                                                                              select temp).ToList();

                if (sys_serverSettingClasses_temp.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<string> strs = (from temp in sys_serverSettingClasses_temp
                                     select temp.內容).ToList();

                returnData.Code = 200;
                returnData.Result = $"取得伺服器服務端,共<{sys_serverSettingClasses_temp.Count}>筆";
                returnData.Data = strs;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }

        }

        private string CheckCreatTable()
        {
            sys_serverSettingClass sys_serverSettingClass = new sys_serverSettingClass();
            sys_serverSettingClass.Server = Server;
            sys_serverSettingClass.Port = Port.ToString();
            sys_serverSettingClass.User = UserName;
            sys_serverSettingClass.Password = Password;
            sys_serverSettingClass.DBName = DB;

            return CheckCreatTable(sys_serverSettingClass);
        }
        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_sys_serverSetting());
            return table.JsonSerializationt(true);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        static public List<sys_serverSettingClass> GetAllServerSetting()
        {
            SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
            List<object[]> list_value = sQLControl.GetAllRows(null);

            List<sys_serverSettingClass> sys_serverSettingClasses = list_value.SQLToClass<sys_serverSettingClass, enum_sys_serverSetting>();
            return sys_serverSettingClasses;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public async Task<List<sys_serverSettingClass>> GetAllServerSettingasync()
        {
            SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
            string command = $"SELECT * FROM {DB}.ServerSetting ;";
            Task<List<object[]>> task = sQLControl.WriteCommandAsync(command);
            await Task.WhenAll(task);
            List<object[]> list_value = task.Result;
            List<sys_serverSettingClass> sys_serverSettingClasses = list_value.SQLToClass<sys_serverSettingClass, enum_sys_serverSetting>();
            return sys_serverSettingClasses;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public async Task<List<sys_serverSettingClass>> GetAllServerSettingasync(string Name, string Type, string Content)
        {
            SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
            string command = $"SELECT * FROM {DB}.ServerSetting WHERE 設備名稱 = '{Name}' AND 類別 = '{Type}' AND 內容 = '{Content}';";
            Task<List<object[]>> task = sQLControl.WriteCommandAsync(command);
            await Task.WhenAll(task);
            List<object[]> list_value = task.Result;
            List<sys_serverSettingClass> sys_serverSettingClasses = list_value.SQLToClass<sys_serverSettingClass, enum_sys_serverSetting>();
            return sys_serverSettingClasses;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public async Task<List<sys_serverSettingClass>> GetServerSettingasync(string Name, string Type)
        {
            try 
            {
                SQLControl sQLControl = new SQLControl(Server, DB, "ServerSetting", UserName, Password, Port, SSLMode);
                string command = $"SELECT * FROM {DB}.ServerSetting WHERE 設備名稱 = '{Name}' AND 類別 = '{Type}' ;";
                List<object[]> list_value = await sQLControl.WriteCommandAsync(command);
                List<sys_serverSettingClass> sys_serverSettingClasses = list_value.SQLToClass<sys_serverSettingClass, enum_sys_serverSetting>();
                return sys_serverSettingClasses;
            }
            catch 
            {
                return null;
            }
            
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_serversetting_by_department_type(string department_type)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(department_type);
            string result = await POST_get_serversetting_by_department_type(returnData);
            returnData = result.JsonDeserializet<returnData>();
            return returnData;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_by_GUID(string GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(GUID);
            string result = await get_by_GUID(returnData);
            returnData = result.JsonDeserializet<returnData>();
            return returnData;
        } 
    }
}
