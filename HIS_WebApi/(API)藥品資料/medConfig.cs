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
using MyOffice;
using NPOI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using MyUI;
using H_Pannel_lib;
using HIS_DB_Lib;
using System.Data;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class medConfig : Controller
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        /// <summary>
        /// 初始化資料庫
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(medConfigClass))]
        [Route("init")]
        [HttpPost]
        public string GET_init([FromBody] returnData returnData)
        {
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
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
        /// 取得全部藥品設定
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
        [Route("get_all")]
        [HttpPost]
        public string get_all([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_all";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> _sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (_sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                string Server = _sys_serverSettingClasses[0].Server;
                string DB = _sys_serverSettingClasses[0].DBName;
                string UserName = _sys_serverSettingClasses[0].User;
                string Password = _sys_serverSettingClasses[0].Password;
                uint Port = (uint)_sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_config = sQLControl_med_config.GetAllRows(null);

                List<medConfigClass> medConfigClasses = list_med_config.SQLToClass<medConfigClass, enum_medConfig>();
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medConfigClasses;
                returnData.Result = $"取得藥品設定成功,共<{medConfigClasses.Count}>筆資料";
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                GET_init(returnData);
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }
        /// <summary>
        /// 以藥碼取得藥品設定
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "ValueAry":["code1,code2,code3"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_codes")]
        [HttpPost]
        public string get_by_codes([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_codes";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> _sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (_sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[codes]";
                    return returnData.JsonSerializationt(true);
                }
                string[] codes = returnData.ValueAry[0].Split(",");
                if (codes.Length == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[codes]";
                    return returnData.JsonSerializationt(true);
                }
                string Server = _sys_serverSettingClasses[0].Server;
                string DB = _sys_serverSettingClasses[0].DBName;
                string UserName = _sys_serverSettingClasses[0].User;
                string Password = _sys_serverSettingClasses[0].Password;
                uint Port = (uint)_sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                List<object[]> list_value = new List<object[]>();
                string sqlList = string.Join(", ", codes.Select(code => $"'{code}'"));
                string command = $"select * from {DB}.{TableName} where UPPER({enum_medConfig.藥碼.GetEnumName()}) in ({sqlList});";
                DataTable dataTable = sQLControl_med_config.WtrteCommandAndExecuteReader(command);

                list_value = dataTable.DataTableToRowList();

                List<medConfigClass> medConfigClasses = list_value.SQLToClass<medConfigClass, enum_medConfig>();
                returnData.Code = 200;
                returnData.Data = medConfigClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"取得藥品設定成功,共<{list_value.Count}>筆資料";
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
        /// 新增或修改藥品設定
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        [medConfigClass陣列]
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("add")]
        [HttpPost]
        public string add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "add";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> _sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (_sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                List<medConfigClass> medConfigClasses = returnData.Data.ObjToClass<List<medConfigClass>>();
                if (medConfigClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
                string Server = _sys_serverSettingClasses[0].Server;
                string DB = _sys_serverSettingClasses[0].DBName;
                string UserName = _sys_serverSettingClasses[0].User;
                string Password = _sys_serverSettingClasses[0].Password;
                uint Port = (uint)_sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_config = sQLControl_med_config.GetAllRows(null);
                List<object[]> list_med_config_buf = new List<object[]>();
                List<object[]> list_med_config_add = new List<object[]>();
                List<object[]> list_med_config_replace = new List<object[]>();
                for (int i = 0; i < medConfigClasses.Count; i++)
                {
                    string 藥碼 = medConfigClasses[i].藥碼;
                    list_med_config_buf = list_med_config.GetRows((int)enum_medConfig.藥碼, 藥碼);
                    if (list_med_config_buf.Count > 0)
                    {
                        object[] value = new object[new enum_medConfig().GetLength()];
                        medConfigClasses[i].GUID = list_med_config_buf[0][(int)enum_medConfig.GUID].ObjectToString();
                        value = medConfigClasses[i].ClassToSQL<medConfigClass, enum_medConfig>();
                        list_med_config_replace.Add(value);
                    }
                    else
                    {
                        object[] value = new object[new enum_medConfig().GetLength()];
                        medConfigClasses[i].GUID = Guid.NewGuid().ToString();
                        value = medConfigClasses[i].ClassToSQL<medConfigClass, enum_medConfig>();
                        list_med_config_add.Add(value);
                    }
                }
                sQLControl_med_config.AddRows(null, list_med_config_add);
                sQLControl_med_config.UpdateByDefulteExtra(null, list_med_config_replace);
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medConfigClasses;
                returnData.Result = $"新增及修改藥品設定成功,共新增<{list_med_config_add.Count}>筆資料,共修改<{list_med_config_replace.Count}>筆";
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
        /// 以GUID刪除藥品設定
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "[GUID]"
        ///     ] 
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("delete_by_guid")]
        [HttpPost]
        public string delete_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "delete_by_guid";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> _sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (_sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
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
                string GUID = returnData.ValueAry[0];

                string Server = _sys_serverSettingClasses[0].Server;
                string DB = _sys_serverSettingClasses[0].DBName;
                string UserName = _sys_serverSettingClasses[0].User;
                string Password = _sys_serverSettingClasses[0].Password;
                uint Port = (uint)_sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_med_config.GetRowsByDefult(null, (int)enum_medConfig.GUID, GUID);
                if (list_value.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt(true);
                }

                sQLControl_med_config.DeleteExtra(null, list_value);
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"刪除藥品設定成功,共刪除<{list_value.Count}>筆資料";
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
        /// 以藥碼刪除藥品設定
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "ValueAry":["code1,code2,code3"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("delete_by_codes")]
        [HttpPost]
        public string delete_by_codes([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "delete_by_codes";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> _sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (_sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[codes]";
                    return returnData.JsonSerializationt(true);
                }
                string[] codes = returnData.ValueAry[0].Split(",");
                if (codes.Length == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[codes]";
                    return returnData.JsonSerializationt(true);
                }
                string Server = _sys_serverSettingClasses[0].Server;
                string DB = _sys_serverSettingClasses[0].DBName;
                string UserName = _sys_serverSettingClasses[0].User;
                string Password = _sys_serverSettingClasses[0].Password;
                uint Port = (uint)_sys_serverSettingClasses[0].Port.StringToInt32();
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                List<object[]> list_value = new List<object[]>();
                string sqlList = string.Join(", ", codes.Select(code => $"'{code}'"));
                string command = $"select * from {DB}.{TableName} where UPPER({enum_medConfig.藥碼.GetEnumName()}) in ({sqlList});";
                DataTable dataTable = sQLControl_med_config.WtrteCommandAndExecuteReader(command);

                list_value = dataTable.DataTableToRowList();
                sQLControl_med_config.DeleteExtra(null, list_value);
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"刪除藥品設定成功,共刪除<{list_value.Count}>筆資料";
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
        /// 取得調劑註記藥品
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     }
        ///   
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_dispense_note")]
        [HttpPost]
        public string get_dispense_note([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_dispense_note";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

           

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "藥檔資料");
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_config = new List<object[]>();
                string command = $"select * from {DB}.{TableName} where ( UPPER({enum_medConfig.調劑註記.GetEnumName()}) = 'y' or UPPER({enum_medConfig.調劑註記.GetEnumName()}) = 'true');";
                DataTable dataTable = sQLControl_med_config.WtrteCommandAndExecuteReader(command);
                list_med_config = dataTable.DataTableToRowList();
                List<medConfigClass> medConfigClasses = list_med_config.SQLToClass<medConfigClass, enum_medConfig>();


                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medConfigClasses;
                returnData.Result = $"取得藥品設定成功,共<{medConfigClasses.Count}>筆資料";
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
        /// 取得形狀相似藥品
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     }
        ///   
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_isShapeSimilar_note")]
        [HttpPost]
        public string get_isShapeSimilar_note([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_isShapeSimilar_note";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);



                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "藥檔資料");
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_config = new List<object[]>();
                string command = $"select * from {DB}.{TableName} where ( UPPER({enum_medConfig.形狀相似.GetEnumName()}) = 'y' or UPPER({enum_medConfig.形狀相似.GetEnumName()}) = 'true');";
                DataTable dataTable = sQLControl_med_config.WtrteCommandAndExecuteReader(command);
                list_med_config = dataTable.DataTableToRowList();
                List<medConfigClass> medConfigClasses = list_med_config.SQLToClass<medConfigClass, enum_medConfig>();


                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medConfigClasses;
                returnData.Result = $"取得藥品設定成功,共<{medConfigClasses.Count}>筆資料";
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
        /// 取得發音相似藥品
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     }
        ///   
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_isSoundSimilar_note")]
        [HttpPost]
        public string get_isSoundSimilar_note([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_isSoundSimilar_note";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);



                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "藥檔資料");
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_config = new List<object[]>();
                string command = $"select * from {DB}.{TableName} where ( UPPER({enum_medConfig.發音相似.GetEnumName()}) = 'y' or UPPER({enum_medConfig.發音相似.GetEnumName()}) = 'true');";
                DataTable dataTable = sQLControl_med_config.WtrteCommandAndExecuteReader(command);
                list_med_config = dataTable.DataTableToRowList();
                List<medConfigClass> medConfigClasses = list_med_config.SQLToClass<medConfigClass, enum_medConfig>();


                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medConfigClasses;
                returnData.Result = $"取得藥品設定成功,共<{medConfigClasses.Count}>筆資料";
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
        /// 取得使用RFID管控藥品
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     }
        ///   
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_useRFID")]
        [HttpPost]
        public string get_useRFID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_useRFID";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);



                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "藥檔資料");
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_config = new List<object[]>();
                string command = $"select * from {DB}.{TableName} where ( UPPER({enum_medConfig.使用RFID.GetEnumName()}) = 'y' or UPPER({enum_medConfig.使用RFID.GetEnumName()}) = 'true');";
                DataTable dataTable = sQLControl_med_config.WtrteCommandAndExecuteReader(command);
                list_med_config = dataTable.DataTableToRowList();
                List<medConfigClass> medConfigClasses = list_med_config.SQLToClass<medConfigClass, enum_medConfig>();


                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medConfigClasses;
                returnData.Result = $"取得藥品設定成功,共<{medConfigClasses.Count}>筆資料";
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
        /// 以藥碼取得調劑註記藥品
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry":["code1,code2,code3"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_dispense_note_by_codes")]
        [HttpPost]
        public string get_dispense_note_by_codes([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_dispense_note_by_codes";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt();
                }
                string[] codes = returnData.ValueAry[0].Split(',');
                HashSet<string> excludeSet = null;
                if (codes != null && codes.Length > 0)
                {
                    excludeSet = new HashSet<string>(
                        codes.Select(e => e.ToUpper())
                    );
                }

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "藥檔資料");
                string TableName = "med_config";
                SQLControl sQLControl_med_config = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_config = new List<object[]>();
                string sqlList = string.Join(", ", codes.Select(code => $"'{code}'"));
                string command = $"select * from {DB}.{TableName} where UPPER({enum_medConfig.藥碼.GetEnumName()}) in ({sqlList}) and ( UPPER({enum_medConfig.調劑註記.GetEnumName()}) = 'y' or UPPER({enum_medConfig.調劑註記.GetEnumName()}) = 'true');";
                DataTable dataTable = sQLControl_med_config.WtrteCommandAndExecuteReader(command);
                list_med_config = dataTable.DataTableToRowList();
                List<medConfigClass> medConfigClasses = list_med_config.SQLToClass<medConfigClass, enum_medConfig>();


                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medConfigClasses;
                returnData.Result = $"取得藥品設定成功,共<{medConfigClasses.Count}>筆資料";
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
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_medConfig()));
            return tables.JsonSerializationt(true);
        }

    }
}
