using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using HIS_WebApi._API_藥品資料;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace HIS_WebApi
{

    //api/MED_page/
    [Route("api/[controller]")]
    [ApiController]
    public class MED_pageController : Controller
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private static readonly Lazy<Task<sys_serverSettingClass>>
          GetServerAsync = new Lazy<Task<sys_serverSettingClass>>(async () =>
          {
              sys_serverSettingClass sys_ServerSetting = await Method.GetServerAsync("Main", "網頁", "藥檔資料");

              if (sys_ServerSetting == null)
                  throw new SecurityException("Database password cannot be null or empty (medUnit).");

              return sys_ServerSetting;
          });

        /// <summary>
        /// 初始化資料庫
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("init")]
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(OrderClass))]
        [HttpPost]
        public string init([FromBody] returnData returnData)
        {
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
                return CheckCreatTable(returnData);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        [HttpPost]
        public string Get([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                string Server = returnData.Server;
                string DbName = returnData.DbName;
                string TableName = returnData.TableName;
                string UserName = returnData.UserName;
                string Password = returnData.Password;
                uint Port = returnData.Port;
                SQLControl sQLControl_med = new SQLControl(Server, DbName, TableName, UserName, Password, Port, SSLMode);
                string[] colName = sQLControl_med.GetAllColumn_Name(TableName);
                List<object[]> list_med = sQLControl_med.GetAllRows(null);

                if (sQLControl_med.ColumnName_Enum_IsEqual(new enum_雲端藥檔()))
                {
                    returnData.Data = list_med.SQLToClass<medClass, enum_雲端藥檔>();
                    returnData.Code = 200;
                    returnData.Result = "雲端藥檔取得成功!";
                    return returnData.JsonSerializationt(true);
                }
                if (sQLControl_med.ColumnName_Enum_IsEqual(new enum_medDrugstore()))
                {
                    returnData.Data = list_med.SQLToClass<medClass, enum_medDrugstore>();
                    returnData.Code = 200;
                    returnData.Result = "藥庫藥檔取得成功!";
                    return returnData.JsonSerializationt(true);
                }
                if (sQLControl_med.ColumnName_Enum_IsEqual(new enum_medDrugstore()))
                {
                    returnData.Data = list_med.SQLToClass<medClass, enum_medDrugstore>();
                    returnData.Code = 200;
                    returnData.Result = "藥局藥檔取得成功!";
                    return returnData.JsonSerializationt(true);
                }
                if (sQLControl_med.ColumnName_Enum_IsEqual(new enum_藥品資料_藥檔資料()))
                {
                    returnData.Data = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    returnData.Code = 200;
                    returnData.Result = "調劑台藥檔取得成功!";
                    return returnData.JsonSerializationt(true);
                }
                returnData.Code = -5;
                returnData.Result = "藥檔取得失敗!";

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
        /// 取得雲端藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [HttpPost("get_med_cloud")]
        public async Task<string> get_med_cloud(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_med_cloud";
            try
            {
                var sbLog = new StringBuilder();

                returnData.ServerName = "Main";
                returnData.ServerType = "網頁";
                returnData.TableName = "medicine_page_cloud";
                init(returnData);

                // === 取得 Server Info ===
                (string Server, string DB, string UserName, string Password, uint Port)
                    = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "藥檔資料");
                sbLog.AppendLine($"[取得ServerInfo] {myTimerBasic.GetTickTime()} ms");

                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);

                // === 同步執行藥檔與儲位讀取 ===
                Task<List<object[]>> task_list_med = sQLControl_med.GetAllRowsAsync(null);
                Task<returnData> task_returnData = new medStorage().get_all();

                returnData returnData_medStorage = await task_returnData;
                sbLog.AppendLine($"[取得儲位資料] {myTimerBasic.GetTickTime()} ms");

                if (returnData_medStorage == null || returnData_medStorage.Code != 200)
                {
                    returnData.Code = -200;
                    returnData.Result = $"儲位取得失敗! (耗時: {myTimerBasic.ToString()})";
                    return await returnData.JsonSerializationtAsync(true);
                }

                List<medStorageClass> medStorageClasses = returnData_medStorage.Data.ObjToClass<List<medStorageClass>>();
                List<object[]> list_med = await task_list_med;
                sbLog.AppendLine($"[取得雲端藥檔Raw] {myTimerBasic.GetTickTime()} ms");

                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_雲端藥檔>();
                sbLog.AppendLine($"[轉換雲端藥檔Class] {myTimerBasic.GetTickTime()} ms");

                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗! (耗時: {myTimerBasic.ToString()})";
                    return returnData.JsonSerializationt();
                }

                // === 合併儲位資訊 ===
                if (medStorageClasses != null && medStorageClasses.Count > 0)
                {
                    Dictionary<string, List<medStorageClass>> dict_medStorage = medStorageClass.ToDictByCode(medStorageClasses);
                    foreach (var item in medClasses)
                    {
                        List<medStorageClass> medStorages = medStorageClass.GetDictByCode(dict_medStorage, item.藥品碼);
                        if (medStorages != null && medStorages.Count > 0)
                        {
                            item.storageInfo = medStorages;
                        }
                    }
                }
                sbLog.AppendLine($"[合併藥檔與儲位] {myTimerBasic.GetTickTime()} ms");

                // === 排序 ===
                medClasses.Sort(new medClass.ICP_By_name());
                sbLog.AppendLine($"[排序] {myTimerBasic.GetTickTime()} ms");

                // === 成功回傳 ===
                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = "雲端藥檔取得成功!" + Environment.NewLine + sbLog.ToString();
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"[異常] {e.Message} (耗時: {myTimerBasic.ToString()})";
                Logger.Log($"MED_page", returnData.Result);
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得雲端藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_med_cloud_old")]
        [HttpPost]
        public string get_med_cloud_old(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_med_cloud";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }

                returnData.ServerName = "Main";
                returnData.ServerType = "網頁";
                returnData.TableName = "medicine_page_cloud";
                init(returnData);
                List<medClass> medClasses = Get_med_cloud(sys_serverSettingClasses_buf[0]);
                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗!";
                    return returnData.JsonSerializationt();
                }
                medClasses.Sort(new medClass.ICP_By_name());
                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = "雲端藥檔取得成功!";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] {returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        //[HttpPost("get_med_cloudAsync")]
        //public async Task<string> get_med_cloudAsync(returnData returnData)
        //{
        //    MyTimerBasic myTimerBasic = new MyTimerBasic();
        //    myTimerBasic.StartTickTime(50000);
        //    returnData.Method = "get_med_cloud";
        //    returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
        //    try
        //    {

        //        sys_serverSettingClass sys_serverSettingClass = await HIS_WebApi.Method.GetServerAsync("Main", "網頁", "藥檔資料");

        //        if (sys_serverSettingClass == null)
        //        {
        //            returnData.Code = -200;
        //            returnData.Result = $"找無Server資料!";
        //            return returnData.JsonSerializationt();
        //        }

        //        returnData.ServerName = "Main";
        //        returnData.ServerType = "網頁";
        //        returnData.TableName = "medicine_page_cloud";
        //        init(returnData);
        //        List<medClass> medClasses = await Get_med_cloudAsync(sys_serverSettingClass);
        //        if (medClasses == null)
        //        {
        //            returnData.Code = -200;
        //            returnData.Result = $"藥檔取得失敗!";
        //            return returnData.JsonSerializationt();
        //        }
        //        medClasses.Sort(new medClass.ICP_By_name());
        //        returnData.Data = medClasses;
        //        returnData.Code = 200;
        //        returnData.Result = "雲端藥檔取得成功!";
        //        returnData.TimeTaken = myTimerBasic.ToString();

        //        return returnData.JsonSerializationt(false);
        //    }
        //    catch (Exception e)
        //    {
        //        returnData.Code = -200;
        //        returnData.Data = null;
        //        returnData.Result = $"{e.Message}";
        //        Logger.Log($"MED_page", $"[異常] {returnData.Result}");
        //        return returnData.JsonSerializationt(true);
        //    }
        //}
        /// <summary>
        /// 取得中藥雲端藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_Tmed_cloud")]
        [HttpPost]
        public string Tget_med_cloud(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_Tmed_cloud";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }

                returnData.ServerName = "Main";
                returnData.ServerType = "網頁";
                returnData.TableName = "medicine_page_cloud";
                init(returnData);
                List<medClass> medClasses = Get_med_cloud(sys_serverSettingClasses_buf[0]);
                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗!";
                    return returnData.JsonSerializationt();
                }
                medClasses = medClasses.Where(temp => temp.中西藥 == "中藥").ToList();
                medClasses.Sort(new medClass.ICP_By_name());
                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"中藥雲端藥檔取得成功,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以藥碼取得雲端藥檔資料（含儲位資訊）
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///         [藥碼1,藥碼2]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_med_clouds_by_codes")]
        [HttpPost]
        public async Task<string> get_med_clouds_by_codes(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_med_clouds_by_codes";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                sys_serverSettingClass sys_serverSettingClasses = await GetServerAsync.Value;

                             
                
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼1,藥碼2,藥碼3]";
                    return returnData.JsonSerializationt(true);
                }

                string[] Codes = returnData.ValueAry[0].Split(",");
                List<medClass> medClasses = Get_med_cloud(sys_serverSettingClasses, Codes);
                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗!";
                    return returnData.JsonSerializationt();
                }

                // 取得 medStorage 資料
                Task<returnData> task_returnData = new medStorage().get_all();
                returnData returnData_medStorage = await task_returnData;
                if (returnData_medStorage == null || returnData_medStorage.Code != 200)
                {
                    returnData.Code = -200;
                    returnData.Result = $"儲位取得失敗!";
                    return await returnData.JsonSerializationtAsync(true);
                }
                List<medStorageClass> medStorageClasses = returnData_medStorage.Data.ObjToClass<List<medStorageClass>>();

                // 藥檔與儲位資料合併
                if (medStorageClasses != null && medStorageClasses.Count > 0)
                {
                    Dictionary<string, List<medStorageClass>> dict_medStorage = medStorageClass.ToDictByCode(medStorageClasses);
                    foreach (var item in medClasses)
                    {
                        List<medStorageClass> medStorages = medStorageClass.GetDictByCode(dict_medStorage, item.藥品碼);
                        if (medStorages != null && medStorages.Count > 0)
                        {
                            item.storageInfo = medStorages;
                        }
                    }
                }

                // 按照輸入順序過濾
                List<medClass> medClasses_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses = medClass.CoverToDictionaryByCode(medClasses);
                for (int i = 0; i < Codes.Length; i++)
                {
                    var medClasses_temp = keyValuePairs_medClasses.SortDictionaryByCode(Codes[i]);
                    if (medClasses_temp.Count == 0) continue;
                    medClasses_buf.Add(medClasses_temp[0]);
                }

                returnData.Data = medClasses_buf;
                returnData.Code = 200;
                returnData.Result = $"雲端藥檔取得成功,共<{medClasses_buf.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }

        /// <summary>
        /// 以藥名取得雲端藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "Value" : "[前綴]",
        ///     "ValueAry" : 
        ///     [
        ///         [藥名]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_med_clouds_by_name")]
        [HttpPost]
        public string get_med_clouds_by_name(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_med_clouds_by_name";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥名]";
                    return returnData.JsonSerializationt(true);
                }
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses_buf[0];
                string text = returnData.ValueAry[0];


                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToDouble();
                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_med.GetAllRows(null);
                if (returnData.Value == "前綴")
                {
                    list_value = list_value.GetRowsStartWithByLike((int)enum_雲端藥檔.藥品名稱, text);
                }
                else
                {
                    list_value = list_value.GetRowsByLike((int)enum_雲端藥檔.藥品名稱, text);
                }

                List<medClass> medClasses = list_value.SQLToClass<medClass, enum_雲端藥檔>();

                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗!";
                    return returnData.JsonSerializationt();
                }


                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"雲端藥檔取得成功,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以商品名取得雲端藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "Value" : "[前綴]",
        ///     "ValueAry" : 
        ///     [
        ///         [商品名]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_med_clouds_by_dianame")]
        [HttpPost]
        public string get_med_clouds_by_dianame(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_med_clouds_by_dianame";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥名]";
                    return returnData.JsonSerializationt(true);
                }
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses_buf[0];
                string text = returnData.ValueAry[0];


                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToDouble();
                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_med.GetAllRows(null);
                if (returnData.Value == "前綴")
                {
                    list_value = list_value.GetRowsStartWithByLike((int)enum_雲端藥檔.藥品學名, text);
                }
                else
                {
                    list_value = list_value.GetRowsByLike((int)enum_雲端藥檔.藥品學名, text);
                }

                List<medClass> medClasses = list_value.SQLToClass<medClass, enum_雲端藥檔>();

                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗!";
                    return returnData.JsonSerializationt();
                }


                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"雲端藥檔取得成功,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以中文名取得雲端藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "Value" : "[前綴]",
        ///     "ValueAry" : 
        ///     [
        ///         [中文名]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_med_clouds_by_chtname")]
        [HttpPost]
        public string get_med_clouds_by_chtname(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_med_clouds_by_dianame";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥名]";
                    return returnData.JsonSerializationt(true);
                }
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses_buf[0];
                string text = returnData.ValueAry[0];


                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToDouble();
                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_med.GetAllRows(null);
                if (returnData.Value == "前綴")
                {
                    list_value = list_value.GetRowsStartWithByLike((int)enum_雲端藥檔.中文名稱, text);
                }
                else
                {
                    list_value = list_value.GetRowsByLike((int)enum_雲端藥檔.中文名稱, text);
                }

                List<medClass> medClasses = list_value.SQLToClass<medClass, enum_雲端藥檔>();

                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗!";
                    return returnData.JsonSerializationt();
                }

                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗!";
                    return returnData.JsonSerializationt();
                }


                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"雲端藥檔取得成功,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以管制級別取得雲端藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///         [管制級別]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_med_clouds_by_durgkind")]
        [HttpPost]
        public string get_med_clouds_by_durgkind(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_med_clouds_by_durgkind";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥名]";
                    return returnData.JsonSerializationt(true);
                }
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses_buf[0];
                string text = returnData.ValueAry[0];


                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToDouble();
                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_med.GetAllRows(null);
                list_value = list_value.GetRows((int)enum_雲端藥檔.管制級別, text);


                List<medClass> medClasses = list_value.SQLToClass<medClass, enum_雲端藥檔>();

                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗!";
                    return returnData.JsonSerializationt();
                }

                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔取得失敗!";
                    return returnData.JsonSerializationt();
                }


                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"雲端藥檔取得成功,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }

        /// <summary>
        /// 新增及修改雲端藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        [medclass陣列]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///        
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("add_med_clouds")]
        [HttpPost]
        public string add_med_clouds(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "add_med_clouds";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                List<medClass> medClasses_src = returnData.Data.ObjToClass<List<medClass>>();
                if (medClasses_src == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料錯誤";
                    return returnData.JsonSerializationt();
                }
                List<string> codes = (from temp in medClasses_src
                                      select temp.藥品碼).Distinct().ToList();

                Dictionary<string, List<medClass>> keyValuePairs = medClasses_src.CoverToDictionaryByCode();
                List<medClass> medClasses_src_buf = new List<medClass>();
                List<medClass> medClasses_temp = new List<medClass>();

                for (int i = 0; i < codes.Count; i++)
                {
                    medClasses_temp = keyValuePairs.SortDictionaryByCode(codes[i]);
                    if (medClasses_temp.Count > 0)
                    {
                        medClasses_src_buf.Add(medClasses_temp[0]);
                    }
                }


                List<object[]> list_value_add_buf = new List<object[]>();
                List<object[]> list_value_update_buf = new List<object[]>();


                List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
                Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClasses_cloud.CoverToDictionaryByCode();
                List<medClass> medClasses_cloud_buf = new List<medClass>();
                List<medClass> medClasses_add = new List<medClass>();
                List<medClass> medClasses_replace = new List<medClass>();


                for (int i = 0; i < medClasses_src_buf.Count; i++)
                {
                    string 藥碼 = medClasses_src_buf[i].藥品碼;
                    medClasses_cloud_buf = keyValuePairs_med_cloud.SortDictionaryByCode(藥碼);
                    if (medClasses_cloud_buf.Count == 0)
                    {
                        medClasses_src_buf[i].GUID = Guid.NewGuid().ToString();
                        medClasses_add.Add(medClasses_src_buf[i]);
                    }
                    else
                    {
                        medClass medClass_update = medClasses_cloud_buf[0];
                        if (medClasses_src_buf[i].料號.StringIsEmpty() == false) medClass_update.料號 = medClasses_src_buf[i].料號;
                        if (medClasses_src_buf[i].ATC.StringIsEmpty() == false) medClass_update.ATC = medClasses_src_buf[i].ATC;
                        if (medClasses_src_buf[i].中文名稱.StringIsEmpty() == false) medClass_update.中文名稱 = medClasses_src_buf[i].中文名稱;
                        if (medClasses_src_buf[i].藥品名稱.StringIsEmpty() == false) medClass_update.藥品名稱 = medClasses_src_buf[i].藥品名稱;
                        if (medClasses_src_buf[i].藥品學名.StringIsEmpty() == false) medClass_update.藥品學名 = medClasses_src_buf[i].藥品學名;
                        if (medClasses_src_buf[i].藥品群組.StringIsEmpty() == false) medClass_update.藥品群組 = medClasses_src_buf[i].藥品群組;
                        if (medClasses_src_buf[i].健保碼.StringIsEmpty() == false) medClass_update.健保碼 = medClasses_src_buf[i].健保碼;
                        if (medClasses_src_buf[i].包裝單位.StringIsEmpty() == false) medClass_update.包裝單位 = medClasses_src_buf[i].包裝單位;
                        if (medClasses_src_buf[i].包裝數量.StringIsEmpty() == false) medClass_update.包裝數量 = medClasses_src_buf[i].包裝數量;
                        if (medClasses_src_buf[i].最小包裝單位.StringIsEmpty() == false) medClass_update.最小包裝單位 = medClasses_src_buf[i].最小包裝單位;
                        if (medClasses_src_buf[i].建議劑量.StringIsEmpty() == false) medClass_update.建議劑量 = medClasses_src_buf[i].建議劑量;
                        if (medClasses_src_buf[i].建議頻次.StringIsEmpty() == false) medClass_update.建議頻次 = medClasses_src_buf[i].建議頻次;
                        if (medClasses_src_buf[i].治療分類代碼.StringIsEmpty() == false) medClass_update.治療分類代碼 = medClasses_src_buf[i].治療分類代碼;
                        if (medClasses_src_buf[i].治療分類名.StringIsEmpty() == false) medClass_update.治療分類名 = medClasses_src_buf[i].治療分類名;
                        if (medClasses_src_buf[i].藥理分類序號.StringIsEmpty() == false) medClass_update.藥理分類序號 = medClasses_src_buf[i].藥理分類序號;
                        if (medClasses_src_buf[i].藥理分類代碼.StringIsEmpty() == false) medClass_update.藥理分類代碼 = medClasses_src_buf[i].藥理分類代碼;
                        if (medClasses_src_buf[i].藥理分類名.StringIsEmpty() == false) medClass_update.藥理分類名 = medClasses_src_buf[i].藥理分類名;
                        if (medClasses_src_buf[i].適應症.StringIsEmpty() == false) medClass_update.適應症 = medClasses_src_buf[i].適應症;
                        if (medClasses_src_buf[i].健保規範.StringIsEmpty() == false) medClass_update.健保規範 = medClasses_src_buf[i].健保規範;
                        if (medClasses_src_buf[i].使用說明.StringIsEmpty() == false) medClass_update.使用說明 = medClasses_src_buf[i].使用說明;
                        if (medClasses_src_buf[i].警訊藥品.StringIsEmpty() == false) medClass_update.警訊藥品 = medClasses_src_buf[i].警訊藥品;
                        if (medClasses_src_buf[i].高價藥品.StringIsEmpty() == false) medClass_update.高價藥品 = medClasses_src_buf[i].高價藥品;
                        if (medClasses_src_buf[i].自費藥品.StringIsEmpty() == false) medClass_update.自費藥品 = medClasses_src_buf[i].自費藥品;
                        if (medClasses_src_buf[i].冷藏藥品.StringIsEmpty() == false) medClass_update.冷藏藥品 = medClasses_src_buf[i].冷藏藥品;
                        if (medClasses_src_buf[i].生物製劑.StringIsEmpty() == false) medClass_update.生物製劑 = medClasses_src_buf[i].生物製劑;
                        if (medClasses_src_buf[i].管制級別.StringIsEmpty() == false) medClass_update.管制級別 = medClasses_src_buf[i].管制級別;
                        if (medClasses_src_buf[i].懷孕用藥級別.StringIsEmpty() == false) medClass_update.懷孕用藥級別 = medClasses_src_buf[i].懷孕用藥級別;
                        if (medClasses_src_buf[i].圖片網址.StringIsEmpty() == false) medClass_update.圖片網址 = medClasses_src_buf[i].圖片網址;
                        if (medClasses_src_buf[i].仿單網址.StringIsEmpty() == false) medClass_update.仿單網址 = medClasses_src_buf[i].仿單網址;
                        if (medClasses_src_buf[i].說明書網址.StringIsEmpty() == false) medClass_update.說明書網址 = medClasses_src_buf[i].說明書網址;
                        if (medClasses_src_buf[i].類別.StringIsEmpty() == false) medClass_update.類別 = medClasses_src_buf[i].類別;
                        if (medClasses_src_buf[i].中西藥.StringIsEmpty() == false) medClass_update.中西藥 = medClasses_src_buf[i].中西藥;
                        if (medClasses_src_buf[i].廠牌.StringIsEmpty() == false) medClass_update.廠牌 = medClasses_src_buf[i].廠牌;
                        if (medClasses_src_buf[i].藥品許可證號.StringIsEmpty() == false) medClass_update.藥品許可證號 = medClasses_src_buf[i].藥品許可證號;
                        if (medClasses_src_buf[i].供貨廠商.StringIsEmpty() == false) medClass_update.供貨廠商 = medClasses_src_buf[i].供貨廠商;
                        if (medClasses_src_buf[i].供貨商證字號.StringIsEmpty() == false) medClass_update.供貨商證字號 = medClasses_src_buf[i].供貨商證字號;
                        if (medClasses_src_buf[i].開檔狀態.StringIsEmpty() == false) medClass_update.開檔狀態 = medClasses_src_buf[i].開檔狀態;
                        if (medClasses_src_buf[i].儲位描述.StringIsEmpty() == false) medClass_update.儲位描述 = medClasses_src_buf[i].儲位描述;
                        if (medClasses_src_buf[i].備註.StringIsEmpty() == false) medClass_update.備註 = medClasses_src_buf[i].備註;
                        if (medClasses_src_buf[i].藥品條碼2.StringIsEmpty() == false) medClass_update.藥品條碼2 = medClasses_src_buf[i].藥品條碼2;


                        medClasses_replace.Add(medClass_update);
                    }
                }
                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();

                SQLControl sQLControl = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);

                list_value_add_buf = medClasses_add.ClassToSQL<medClass, enum_雲端藥檔>();
                list_value_update_buf = medClasses_replace.ClassToSQL<medClass, enum_雲端藥檔>();
                List<medClass> result = new List<medClass>();
                result.AddRange(medClasses_add);
                result.AddRange(medClasses_replace);
                sQLControl.AddRows(null, list_value_add_buf);
                sQLControl.UpdateByDefulteExtra(null, list_value_update_buf);

                returnData.Code = 200;
                returnData.Data = result;
                returnData.Result = $"更新雲端藥檔成功,共新增<{list_value_add_buf.Count}>筆資料,共修改<{list_value_update_buf.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 新增及修改雲端藥檔含藥品條碼資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        [medclass陣列]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///        
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("add_med_clouds_barcode")]
        [HttpPost]
        public string add_med_clouds_barcode(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "add_med_clouds_barcode";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                List<medClass> medClasses_src = returnData.Data.ObjToClass<List<medClass>>();
                if (medClasses_src == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料錯誤";
                    return returnData.JsonSerializationt();
                }
                List<string> codes = (from temp in medClasses_src
                                      select temp.藥品碼).Distinct().ToList();

                Dictionary<string, List<medClass>> keyValuePairs = medClasses_src.CoverToDictionaryByCode();
                List<medClass> medClasses_src_buf = new List<medClass>();
                List<medClass> medClasses_temp = new List<medClass>();

                for (int i = 0; i < codes.Count; i++)
                {
                    medClasses_temp = keyValuePairs.SortDictionaryByCode(codes[i]);
                    if (medClasses_temp.Count > 0)
                    {
                        medClasses_src_buf.Add(medClasses_temp[0]);
                    }
                }


                List<object[]> list_value_add_buf = new List<object[]>();
                List<object[]> list_value_update_buf = new List<object[]>();


                List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
                Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClasses_cloud.CoverToDictionaryByCode();
                List<medClass> medClasses_cloud_buf = new List<medClass>();
                List<medClass> medClasses_add = new List<medClass>();
                List<medClass> medClasses_replace = new List<medClass>();


                for (int i = 0; i < medClasses_src_buf.Count; i++)
                {
                    string 藥碼 = medClasses_src_buf[i].藥品碼;
                    medClasses_cloud_buf = keyValuePairs_med_cloud.SortDictionaryByCode(藥碼);
                    if (medClasses_cloud_buf.Count == 0)
                    {
                        medClasses_src_buf[i].GUID = Guid.NewGuid().ToString();
                        medClasses_add.Add(medClasses_src_buf[i]);
                    }
                    else
                    {
                        medClass medClass_update = medClasses_cloud_buf[0];
                        medClass_update.藥品碼 = medClasses_src_buf[i].藥品碼;
                        medClass_update.料號 = medClasses_src_buf[i].料號;
                        medClass_update.藥品名稱 = medClasses_src_buf[i].藥品名稱;
                        medClass_update.藥品學名 = medClasses_src_buf[i].藥品學名;
                        medClass_update.管制級別 = medClasses_src_buf[i].管制級別;
                        medClass_update.包裝單位 = medClasses_src_buf[i].包裝單位;
                        medClass_update.建議劑量 = medClasses_src_buf[i].建議劑量;
                        medClass_update.建議頻次 = medClasses_src_buf[i].建議頻次;
                        medClass_update.適應症 = medClasses_src_buf[i].適應症;
                        medClass_update.使用說明 = medClasses_src_buf[i].使用說明;
                        medClass_update.警訊藥品 = medClasses_src_buf[i].警訊藥品;
                        medClass_update.懷孕用藥級別 = medClasses_src_buf[i].懷孕用藥級別;
                        medClass_update.高價藥品 = medClasses_src_buf[i].高價藥品;
                        medClass_update.冷藏藥品 = medClasses_src_buf[i].冷藏藥品;
                        medClass_update.自費藥品 = medClasses_src_buf[i].自費藥品;
                        medClass_update.生物製劑 = medClasses_src_buf[i].生物製劑;
                        medClass_update.健保碼 = medClasses_src_buf[i].健保碼;
                        medClass_update.健保規範 = medClasses_src_buf[i].健保規範;
                        medClass_update.廠牌 = medClasses_src_buf[i].廠牌;
                        medClass_update.治療分類名 = medClasses_src_buf[i].治療分類名;
                        medClass_update.治療分類代碼 = medClasses_src_buf[i].治療分類代碼;
                        medClass_update.開檔狀態 = medClasses_src_buf[i].開檔狀態;
                        medClass_update.ATC = medClasses_src_buf[i].ATC;
                        medClass_update.中文名稱 = medClasses_src_buf[i].中文名稱;
                        medClass_update.儲位描述 = medClasses_src_buf[i].儲位描述;
                        medClass_update.圖片網址 = medClasses_src_buf[i].圖片網址;
                        medClass_update.仿單網址 = medClasses_src_buf[i].仿單網址;
                        medClass_update.說明書網址 = medClasses_src_buf[i].說明書網址;
                        medClass_update.類別 = medClasses_src_buf[i].類別;
                        medClass_update.中西藥 = medClasses_src_buf[i].中西藥;
                        medClass_update.最小包裝單位 = medClasses_src_buf[i].最小包裝單位;
                        medClass_update.備註 = medClasses_src_buf[i].備註;
                        medClass_update.藥品條碼2 = medClasses_src_buf[i].藥品條碼2;

                        medClasses_replace.Add(medClass_update);
                    }
                }
                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();

                SQLControl sQLControl = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);

                list_value_add_buf = medClasses_add.ClassToSQL<medClass, enum_雲端藥檔>();
                list_value_update_buf = medClasses_replace.ClassToSQL<medClass, enum_雲端藥檔>();
                List<medClass> result = new List<medClass>();
                result.AddRange(medClasses_add);
                result.AddRange(medClasses_replace);
                sQLControl.AddRows(null, list_value_add_buf);
                sQLControl.UpdateByDefulteExtra(null, list_value_update_buf);

                returnData.Code = 200;
                returnData.Data = result;
                returnData.Result = $"更新雲端藥檔成功,共新增<{list_value_add_buf.Count}>筆資料,共修改<{list_value_update_buf.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] {returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以GUID更新雲端藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        [medclass陣列]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///         
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("update_med_clouds_by_guid")]
        [HttpPost]
        public string update_med_clouds_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_med_clouds_by_guid";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                List<medClass> medClasses = returnData.Data.ObjToClass<List<medClass>>();
                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料錯誤";
                    return returnData.JsonSerializationt();
                }
                //returnData.ServerName = "Main";
                //returnData.ServerType = "網頁";
                //returnData.TableName = "medicine_page_cloud";
                //init(returnData);
                List<object[]> list_value_replace = medClasses.ClassToSQL<medClass, enum_雲端藥檔>();


                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();

                SQLControl sQLControl = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);

                sQLControl.UpdateByDefulteExtra(null, list_value_replace);

                returnData.Code = 200;
                returnData.Result = $"更新雲端藥檔成功,共<{list_value_replace.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以GUID更新雲端藥檔狀態資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        [medclass陣列]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///         "GUID","開檔中 or 停用中"
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("update_med_clouds_status_by_guid")]
        [HttpPost]
        public string update_med_clouds_status_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_med_clouds_status_by_guid";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry 不得為空";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry 應為[\"GUID\",\"True or False\"]";
                    return returnData.JsonSerializationt();
                }
                string GUID = returnData.ValueAry[0];
                string Status = returnData.ValueAry[1];

                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();

                SQLControl sQLControl = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);
                List<object[]> med_page = sQLControl.GetRowsByDefult(null, (int)enum_雲端藥檔.GUID, GUID);
                if (med_page.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無對應資料";
                    return returnData.JsonSerializationt();
                }
                List<medClass> medClasses = med_page.SQLToClass<medClass, enum_雲端藥檔>();
                medClasses[0].開檔狀態 = Status;
                List<object[]> update = medClasses.ClassToSQL<medClass, enum_雲端藥檔>();
                sQLControl.UpdateByDefulteExtra(null, update);

                returnData.Code = 200;
                returnData.Data = medClasses;
                returnData.Result = $"更新雲端藥檔成功,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }


        /// <summary>
        /// 從線上藥檔更新藥庫系統的藥局藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "ServerName" : "ds01",
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("update_ds_pharma_med_from_medcloud")]
        [HttpPost]
        public string update_ds_pharma_med_from_medcloud(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_ds_pharma_med";
            returnData.TableName = "medicine_page_phar";
            returnData.ServerType = "藥庫";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, "藥庫", "本地端");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                CheckCreatTable(returnData);
                string TaskTime_藥局 = "";
                string TaskTime_藥庫 = "";
                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];
                List<medClass> medClasses_cloud = Get_med_cloud(sys_serverSettingClasses_med);
                List<medClass> medClasses_cloud_buf = new List<medClass>();





                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_phar", UserName, Password, Port, SSLMode);
                List<object[]> list_med = sQLControl_med.GetAllRows(null);
                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_medPharmacy>();
                List<medClass> medClasses_buf = new List<medClass>();
                List<medClass> medClasses_add = new List<medClass>();
                List<medClass> medClasses_replace = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses = medClasses.CoverToDictionaryByCode();

                for (int i = 0; i < medClasses_cloud.Count; i++)
                {
                    string 藥碼 = medClasses_cloud[i].藥品碼;
                    medClasses_buf = keyValuePairs_medClasses.SortDictionaryByCode(藥碼);
                    if (medClasses_buf.Count == 0)
                    {
                        medClass medClass = medClasses_cloud[i];
                        medClass.GUID = Guid.NewGuid().ToString();
                        medClasses_add.Add(medClass);
                    }
                    else
                    {
                        if (medClasses_cloud[i].IsEqual(medClasses_buf[0]) == false)
                        {
                            medClass medClass = medClasses_buf[0];
                            medClass.Format(medClasses_cloud[i]);
                            medClasses_replace.Add(medClass);
                        }

                    }
                }
                List<object[]> list_medclass_add = medClasses_add.ClassToSQL<medClass, enum_medPharmacy>();
                List<object[]> list_medclass_replace = medClasses_replace.ClassToSQL<medClass, enum_medPharmacy>();
                if (list_medclass_add.Count > 0) sQLControl_med.AddRows(null, list_medclass_add);
                if (list_medclass_replace.Count > 0) sQLControl_med.UpdateByDefulteExtra(null, list_medclass_replace);



                SQLControl sQLControl_device = new SQLControl(Server, DB, "sd0_device_jsonstring", UserName, Password, Port, SSLMode);
                DeviceBasicMethod.SQL_Init(sQLControl_device);
                List<DeviceBasic> deviceBasics = DeviceBasicMethod.SQL_GetAllDeviceBasic(sQLControl_device);
                List<DeviceBasic> deviceBasics_buf = new List<DeviceBasic>();
                List<DeviceBasic> deviceBasics_add = new List<DeviceBasic>();

                Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                for (int i = 0; i < medClasses_cloud.Count; i++)
                {
                    string 藥碼 = medClasses_cloud[i].藥品碼;
                    deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                    if (deviceBasics_buf.Count == 0)
                    {
                        DeviceBasic deviceBasic = new DeviceBasic();
                        deviceBasic.Code = medClasses_cloud[i].藥品碼;
                        deviceBasic.Name = medClasses_cloud[i].藥品名稱;
                        deviceBasic.Scientific_Name = medClasses_cloud[i].藥品學名;
                        deviceBasic.ChineseName = medClasses_cloud[i].中文名稱;
                        deviceBasic.Package = medClasses_cloud[i].包裝單位;
                        deviceBasics_add.Add(deviceBasic);

                    }

                }


                DeviceBasicMethod.SQL_AddDeviceBasic(sQLControl_device, deviceBasics_add);
                returnData.Data = "";
                returnData.Code = 200;
                returnData.Result = $"藥局更新成功,藥檔更新<{list_medclass_replace.Count}>筆,新增<{list_medclass_add.Count}>筆,新增[DeviceBasic]共<{deviceBasics_add.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得藥庫系統的藥局藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "ServerName" : "ds01",
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_ds_pharma_med")]
        [HttpPost]
        public string get_ds_pharma_med(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_ds_pharma_med";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, "藥庫", "本地端");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }

                string TaskTime_藥局 = "";
                string TaskTime_藥庫 = "";
                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = new Dictionary<string, List<medClass>>();
                List<medClass> medClasses_cloud = new List<medClass>();
                List<medClass> medClasses_cloud_buf = new List<medClass>();


                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_phar", UserName, Password, Port, SSLMode);
                List<object[]> list_med = sQLControl_med.GetAllRows(null);
                List<object[]> list_med_buf = new List<object[]>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_藥局 = new List<H_Pannel_lib.DeviceBasic>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_藥庫 = new List<H_Pannel_lib.DeviceBasic>();
                Dictionary<string, List<H_Pannel_lib.DeviceBasic>> dictionary_藥局 = new Dictionary<string, List<DeviceBasic>>();
                Dictionary<string, List<H_Pannel_lib.DeviceBasic>> dictionary_藥庫 = new Dictionary<string, List<DeviceBasic>>();
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(new Action(delegate
                {
                    medClasses_cloud = Get_med_cloud(sys_serverSettingClasses_med);
                    keyValuePairs_medClasses_cloud = medClasses_cloud.CoverToDictionaryByCode();

                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    MyTimerBasic myTimerBasic1 = new MyTimerBasic();
                    deviceBasics_藥局 = deviceController.Function_Get_device(sys_serverSettingClasses_buf[0], "sd0_device_jsonstring");
                    dictionary_藥局 = deviceBasics_藥局.CoverToDictionaryByCode();
                    TaskTime_藥局 = myTimerBasic1.ToString();

                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    MyTimerBasic myTimerBasic1 = new MyTimerBasic();
                    deviceBasics_藥庫 = deviceController.Function_Get_device(sys_serverSettingClasses_buf[0], "firstclass_device_jsonstring");
                    dictionary_藥庫 = deviceBasics_藥庫.CoverToDictionaryByCode();
                    TaskTime_藥庫 = myTimerBasic1.ToString();
                })));
                Task.WhenAll(tasks.ToArray()).Wait();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();
                string 藥碼 = "";
                double inventory = 0;
                List<medClassBasic> medClasses = list_med.SQLToClass<medClassBasic, enum_medPharmacy>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    藥碼 = medClasses[i].藥品碼;
                    inventory = 0;
                    medClasses_cloud_buf = keyValuePairs_medClasses_cloud.SortDictionaryByCode(medClasses[i].藥品碼);
                    if (medClasses_cloud_buf.Count > 0)
                    {
                        medClasses[i].藥品名稱 = medClasses_cloud_buf[0].藥品名稱;
                        medClasses[i].藥品學名 = medClasses_cloud_buf[0].藥品學名;
                        medClasses[i].藥品許可證號 = medClasses_cloud_buf[0].藥品許可證號;
                        medClasses[i].開檔狀態 = medClasses_cloud_buf[0].開檔狀態;
                        medClasses[i].類別 = medClasses_cloud_buf[0].類別;
                    }
                    deviceBasics_buf = new List<DeviceBasic>();
                    if (dictionary_藥局.ContainsKey(藥碼))
                    {
                        deviceBasics_buf = dictionary_藥局[藥碼];
                    }
                    if (deviceBasics_buf.Count == 0) continue;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }
                    medClasses[i].DeviceBasics = deviceBasics_buf;
                    medClasses[i].藥局庫存 = inventory.ToString();

                    inventory = 0;
                    deviceBasics_buf = new List<DeviceBasic>();
                    if (dictionary_藥庫.ContainsKey(藥碼))
                    {
                        deviceBasics_buf = dictionary_藥庫[藥碼];
                    }
                    if (deviceBasics_buf.Count == 0) continue;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }
                    medClasses[i].藥庫庫存 = inventory.ToString();
                    medClasses[i].總庫存 = (medClasses[i].藥局庫存.StringToDouble() + medClasses[i].藥庫庫存.StringToDouble()).ToString();
                }

                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"藥局藥檔取得成功,藥局庫存取得耗時[{TaskTime_藥局}],藥庫庫存取得耗時[{TaskTime_藥庫}]";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以GUID更新藥庫系統的藥局藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        [medclass陣列]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///         
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("update_ds_pharma_by_guid")]
        [HttpPost]
        public string update_ds_pharma_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_ds_pharma_by_guid";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, "藥庫", "本地端");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                List<medClass> medClasses = returnData.Data.ObjToClass<List<medClass>>();
                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料錯誤";
                    return returnData.JsonSerializationt();
                }

                List<object[]> list_value_replace = medClasses.ClassToSQL<medClass, enum_medPharmacy>();


                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();

                SQLControl sQLControl = new SQLControl(Server, DB, "medicine_page_phar", UserName, Password, Port, SSLMode);

                sQLControl.UpdateByDefulteExtra(null, list_value_replace);

                returnData.Code = 200;
                returnData.Result = $"更新藥庫系統的藥局藥檔成功,共<{list_value_replace.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }


        /// <summary>
        /// 從線上藥檔更新藥庫系統的藥庫藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "ServerName" : "ds01",
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("update_ds_drugstore_med_from_medcloud")]
        [HttpPost]
        public string update_ds_drugstore_med_from_medcloud(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_ds_pharma_med";
            returnData.TableName = "medicine_page_firstclass";
            returnData.ServerType = "藥庫";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, "藥庫", "本地端");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                CheckCreatTable(returnData);
                string TaskTime_藥局 = "";
                string TaskTime_藥庫 = "";
                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];
                List<medClass> medClasses_cloud = Get_med_cloud(sys_serverSettingClasses_med);
                List<medClass> medClasses_cloud_buf = new List<medClass>();





                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_firstclass", UserName, Password, Port, SSLMode);
                List<object[]> list_med = sQLControl_med.GetAllRows(null);
                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_medDrugstore>();
                List<medClass> medClasses_buf = new List<medClass>();
                List<medClass> medClasses_add = new List<medClass>();
                List<medClass> medClasses_replace = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses = medClasses.CoverToDictionaryByCode();

                for (int i = 0; i < medClasses_cloud.Count; i++)
                {
                    string 藥碼 = medClasses_cloud[i].藥品碼;
                    medClasses_buf = keyValuePairs_medClasses.SortDictionaryByCode(藥碼);
                    if (medClasses_buf.Count == 0)
                    {
                        medClass medClass = medClasses_cloud[i];
                        medClass.GUID = Guid.NewGuid().ToString();
                        medClasses_add.Add(medClass);
                    }
                    else
                    {
                        if (medClasses_cloud[i].IsEqual(medClasses_buf[0]) == false)
                        {
                            medClass medClass = medClasses_buf[0];
                            medClass.Format(medClasses_cloud[i]);
                            medClasses_replace.Add(medClass);
                        }

                    }
                }
                List<object[]> list_medclass_add = medClasses_add.ClassToSQL<medClass, enum_medDrugstore>();
                List<object[]> list_medclass_replace = medClasses_replace.ClassToSQL<medClass, enum_medDrugstore>();
                if (list_medclass_add.Count > 0) sQLControl_med.AddRows(null, list_medclass_add);
                if (list_medclass_replace.Count > 0) sQLControl_med.UpdateByDefulteExtra(null, list_medclass_replace);



                SQLControl sQLControl_device = new SQLControl(Server, DB, "firstclass_device_jsonstring", UserName, Password, Port, SSLMode);
                DeviceBasicMethod.SQL_Init(sQLControl_device);
                List<DeviceBasic> deviceBasics = DeviceBasicMethod.SQL_GetAllDeviceBasic(sQLControl_device);
                List<DeviceBasic> deviceBasics_buf = new List<DeviceBasic>();
                List<DeviceBasic> deviceBasics_add = new List<DeviceBasic>();

                Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                for (int i = 0; i < medClasses_cloud.Count; i++)
                {
                    string 藥碼 = medClasses_cloud[i].藥品碼;
                    deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                    if (deviceBasics_buf.Count == 0)
                    {
                        DeviceBasic deviceBasic = new DeviceBasic();
                        deviceBasic.Code = medClasses_cloud[i].藥品碼;
                        deviceBasic.Name = medClasses_cloud[i].藥品名稱;
                        deviceBasic.Scientific_Name = medClasses_cloud[i].藥品學名;
                        deviceBasic.ChineseName = medClasses_cloud[i].中文名稱;
                        deviceBasic.Package = medClasses_cloud[i].包裝單位;
                        deviceBasics_add.Add(deviceBasic);

                    }

                }


                DeviceBasicMethod.SQL_AddDeviceBasic(sQLControl_device, deviceBasics_add);
                returnData.Data = "";
                returnData.Code = 200;
                returnData.Result = $"藥庫更新成功,藥檔更新<{list_medclass_replace.Count}>筆,新增<{list_medclass_add.Count}>筆,新增[DeviceBasic]共<{deviceBasics_add.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得藥庫系統的藥庫藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "ServerName" : "ds01",
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_ds_drugstore_med")]
        [HttpPost]
        public string get_ds_drugstore_med(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_ds_drugstore_med";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, "藥庫", "本地端");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }

                string TaskTime_藥局 = "";
                string TaskTime_藥庫 = "";
                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = new Dictionary<string, List<medClass>>();
                List<medClass> medClasses_cloud = new List<medClass>();
                List<medClass> medClasses_cloud_buf = new List<medClass>();


                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_firstclass", UserName, Password, Port, SSLMode);
                List<object[]> list_med = sQLControl_med.GetAllRows(null);
                List<object[]> list_med_buf = new List<object[]>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_藥局 = new List<H_Pannel_lib.DeviceBasic>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_藥庫 = new List<H_Pannel_lib.DeviceBasic>();
                Dictionary<string, List<H_Pannel_lib.DeviceBasic>> dictionary_藥局 = new Dictionary<string, List<DeviceBasic>>();
                Dictionary<string, List<H_Pannel_lib.DeviceBasic>> dictionary_藥庫 = new Dictionary<string, List<DeviceBasic>>();
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(new Action(delegate
                {
                    medClasses_cloud = Get_med_cloud(sys_serverSettingClasses_med);
                    keyValuePairs_medClasses_cloud = medClasses_cloud.CoverToDictionaryByCode();

                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    MyTimerBasic myTimerBasic1 = new MyTimerBasic();
                    deviceBasics_藥局 = deviceController.Function_Get_device(sys_serverSettingClasses_buf[0], "sd0_device_jsonstring");
                    dictionary_藥局 = deviceBasics_藥局.CoverToDictionaryByCode();
                    TaskTime_藥局 = myTimerBasic1.ToString();

                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    MyTimerBasic myTimerBasic1 = new MyTimerBasic();
                    deviceBasics_藥庫 = deviceController.Function_Get_device(sys_serverSettingClasses_buf[0], "firstclass_device_jsonstring");
                    dictionary_藥庫 = deviceBasics_藥庫.CoverToDictionaryByCode();
                    TaskTime_藥庫 = myTimerBasic1.ToString();
                })));
                Task.WhenAll(tasks.ToArray()).Wait();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();
                string 藥碼 = "";
                double inventory = 0;
                List<medClassBasic> medClasses = list_med.SQLToClass<medClassBasic, enum_medDrugstore>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    藥碼 = medClasses[i].藥品碼;
                    inventory = 0;
                    medClasses_cloud_buf = keyValuePairs_medClasses_cloud.SortDictionaryByCode(medClasses[i].藥品碼);
                    if (medClasses_cloud_buf.Count > 0)
                    {
                        medClasses[i].藥品名稱 = medClasses_cloud_buf[0].藥品名稱;
                        medClasses[i].藥品學名 = medClasses_cloud_buf[0].藥品學名;
                        medClasses[i].藥品許可證號 = medClasses_cloud_buf[0].藥品許可證號;
                        medClasses[i].開檔狀態 = medClasses_cloud_buf[0].開檔狀態;
                        medClasses[i].類別 = medClasses_cloud_buf[0].類別;
                    }
                    deviceBasics_buf = new List<DeviceBasic>();
                    if (dictionary_藥局.ContainsKey(藥碼))
                    {
                        deviceBasics_buf = dictionary_藥局[藥碼];
                    }
                    if (deviceBasics_buf.Count == 0) continue;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }

                    medClasses[i].藥局庫存 = inventory.ToString();

                    deviceBasics_buf = new List<DeviceBasic>();
                    if (dictionary_藥庫.ContainsKey(藥碼))
                    {
                        deviceBasics_buf = dictionary_藥庫[藥碼];
                    }

                    inventory = 0;
                    if (deviceBasics_buf.Count == 0) continue;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }
                    medClasses[i].DeviceBasics = deviceBasics_buf;
                    medClasses[i].藥庫庫存 = inventory.ToString();
                    medClasses[i].總庫存 = (medClasses[i].藥局庫存.StringToDouble() + medClasses[i].藥庫庫存.StringToDouble()).ToString();
                }

                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"藥局藥檔取得成功,藥局庫存取得耗時[{TaskTime_藥局}],藥庫庫存取得耗時[{TaskTime_藥庫}]";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以GUID更新藥庫系統的藥庫藥檔資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///        [medclass陣列]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///         
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("update_ds_drugstore_by_guid")]
        [HttpPost]
        public string update_ds_store_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_ds_drugstore_by_guid";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, "藥庫", "本地端");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                }
                List<medClass> medClasses = returnData.Data.ObjToClass<List<medClass>>();
                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料錯誤";
                    return returnData.JsonSerializationt();
                }

                List<object[]> list_value_replace = medClasses.ClassToSQL<medClass, enum_medDrugstore>();


                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();

                SQLControl sQLControl = new SQLControl(Server, DB, "medicine_page_firstclass", UserName, Password, Port, SSLMode);

                sQLControl.UpdateByDefulteExtra(null, list_value_replace);

                returnData.Code = 200;
                returnData.Result = $"更新藥庫系統的藥庫藥檔成功,共<{list_value_replace.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();

                return returnData.JsonSerializationt(false);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"MED_page", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }

        /// <summary>
        /// 查詢藥品資料JSON格式範例
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.ServerName] : 填入記憶的伺服器名稱(medicine_page_cloud可預設為"Main")<br/> 
        ///  2.[returnData.ServerType] : 填入記憶的種類(medicine_page_cloud可預設為"網頁")<br/> 
        ///  3.[returnData.TableName] : 選擇其中一種資料表名稱:medicine_page_cloud、medicine_page_firstclass、medicine_page_phar、medicine_page<br/> 
        ///  --------------------------------------------
        /// <code>
        /// {
        ///     "ServerName" : "Main",
        ///     "ServerType" : "網頁",  
        ///     "TableName" : "medicine_page_cloud",  
        ///     "Data": 
        ///     {
        ///         
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        [Route("get_by_apiserver")]
        [HttpPost]
        public string get_by_apiserver([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string TableName = returnData.TableName;
            returnData.Method = "get_by_apiserver";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                if (TableName == "medicine_page_cloud")
                {
                    string json_out = get_med_cloud_old(returnData);
                    return json_out;
                }
                //藥庫藥品資料
                if (TableName == "medicine_page_firstclass")
                {
                    string TaskTime_藥局 = "";
                    string TaskTime_藥庫 = "";
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                    List<object[]> list_med = sQLControl_med.GetAllRows(null);
                    List<object[]> list_med_buf = new List<object[]>();
                    List<H_Pannel_lib.DeviceSimple> deviceBasics_藥局 = new List<H_Pannel_lib.DeviceSimple>();
                    List<H_Pannel_lib.DeviceSimple> deviceBasics_藥庫 = new List<H_Pannel_lib.DeviceSimple>();

                    List<Task> tasks = new List<Task>();

                    tasks.Add(Task.Run(new Action(delegate
                    {
                        MyTimerBasic myTimerBasic1 = new MyTimerBasic();
                        deviceBasics_藥局 = deviceController.Function_Get_deviceSimple(sys_serverSettingClasses[0], "sd0_device_jsonstring");
                        List<H_Pannel_lib.DeviceSimple> deviceBasics_buf = new List<H_Pannel_lib.DeviceSimple>();
                        string 藥碼 = "";
                        double inventory = 0;
                        for (int i = 0; i < list_med.Count; i++)
                        {
                            inventory = 0;
                            藥碼 = list_med[i][(int)enum_medDrugstore.藥品碼].ObjectToString();
                            deviceBasics_buf = (from temp in deviceBasics_藥局
                                                where temp.Code == 藥碼
                                                select temp).ToList();
                            for (int k = 0; k < deviceBasics_buf.Count; k++)
                            {
                                inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                            }
                            list_med[i][(int)enum_medDrugstore.藥局庫存] = inventory.ToString();
                        }
                        TaskTime_藥局 = myTimerBasic1.ToString();

                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        MyTimerBasic myTimerBasic1 = new MyTimerBasic();
                        deviceBasics_藥庫 = deviceController.Function_Get_deviceSimple(sys_serverSettingClasses[0], "firstclass_device_jsonstring");
                        List<H_Pannel_lib.DeviceSimple> deviceBasics_buf = new List<H_Pannel_lib.DeviceSimple>();
                        string 藥碼 = "";
                        double inventory = 0;
                        for (int i = 0; i < list_med.Count; i++)
                        {
                            inventory = 0;
                            藥碼 = list_med[i][(int)enum_medDrugstore.藥品碼].ObjectToString();
                            deviceBasics_buf = (from temp in deviceBasics_藥庫
                                                where temp.Code == 藥碼
                                                select temp).ToList();
                            for (int k = 0; k < deviceBasics_buf.Count; k++)
                            {
                                inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                            }
                            list_med[i][(int)enum_medDrugstore.藥庫庫存] = inventory.ToString();
                        }
                        TaskTime_藥庫 = myTimerBasic1.ToString();
                    })));
                    Task.WhenAll(tasks).Wait();

                    returnData.Data = list_med.SQLToClass<medClass, enum_medDrugstore>();
                    returnData.Code = 200;
                    returnData.Result = $"藥庫藥檔取得成功,藥局庫存取得耗時[{TaskTime_藥局}],藥庫庫存取得耗時[{TaskTime_藥庫}]";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                //藥局藥品資料
                if (TableName == "medicine_page_phar")
                {

                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");

                    string TaskTime_藥局 = "";
                    string TaskTime_藥庫 = "";
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }

                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                    List<object[]> list_med = sQLControl_med.GetAllRows(null);
                    List<object[]> list_med_buf = new List<object[]>();
                    List<H_Pannel_lib.DeviceSimple> deviceBasics_藥局 = new List<H_Pannel_lib.DeviceSimple>();
                    List<H_Pannel_lib.DeviceSimple> deviceBasics_藥庫 = new List<H_Pannel_lib.DeviceSimple>();

                    List<Task> tasks = new List<Task>();

                    tasks.Add(Task.Run(new Action(delegate
                    {
                        MyTimerBasic myTimerBasic1 = new MyTimerBasic();
                        deviceBasics_藥局 = deviceController.Function_Get_deviceSimple(sys_serverSettingClasses[0], "sd0_device_jsonstring");
                        Dictionary<string, List<H_Pannel_lib.DeviceSimple>> dictionary = ConvertToDictionary(deviceBasics_藥局);
                        List<H_Pannel_lib.DeviceSimple> deviceBasics_buf = new List<H_Pannel_lib.DeviceSimple>();
                        string 藥碼 = "";
                        double inventory = 0;
                        for (int i = 0; i < list_med.Count; i++)
                        {
                            inventory = 0;
                            藥碼 = list_med[i][(int)enum_medDrugstore.藥品碼].ObjectToString();
                            deviceBasics_buf = new List<H_Pannel_lib.DeviceSimple>();
                            if (dictionary.ContainsKey(藥碼))
                            {
                                deviceBasics_buf = dictionary[藥碼];
                            }
                            if (deviceBasics_buf == null) continue;
                            for (int k = 0; k < deviceBasics_buf.Count; k++)
                            {
                                inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                            }
                            list_med[i][(int)enum_medDrugstore.藥局庫存] = inventory.ToString();
                        }
                        TaskTime_藥局 = myTimerBasic1.ToString();

                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        MyTimerBasic myTimerBasic1 = new MyTimerBasic();
                        deviceBasics_藥庫 = deviceController.Function_Get_deviceSimple(sys_serverSettingClasses[0], "firstclass_device_jsonstring");
                        List<H_Pannel_lib.DeviceSimple> deviceBasics_buf = new List<H_Pannel_lib.DeviceSimple>();
                        Dictionary<string, List<H_Pannel_lib.DeviceSimple>> dictionary = ConvertToDictionary(deviceBasics_藥庫);
                        string 藥碼 = "";
                        double inventory = 0;

                        for (int i = 0; i < list_med.Count; i++)
                        {
                            inventory = 0;
                            藥碼 = list_med[i][(int)enum_medDrugstore.藥品碼].ObjectToString();
                            //deviceBasics_buf = (from temp in deviceBasics_藥庫
                            //                    where temp.Code == 藥碼
                            //                    select temp).ToList();
                            deviceBasics_buf = new List<H_Pannel_lib.DeviceSimple>();
                            if (dictionary.ContainsKey(藥碼))
                            {
                                deviceBasics_buf = dictionary[藥碼];
                            }
                            if (deviceBasics_buf == null) continue;
                            for (int k = 0; k < deviceBasics_buf.Count; k++)
                            {
                                inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                            }
                            list_med[i][(int)enum_medDrugstore.藥庫庫存] = inventory.ToString();
                        }
                        TaskTime_藥庫 = myTimerBasic1.ToString();
                    })));
                    Task.WhenAll(tasks.ToArray()).Wait();



                    returnData.Data = list_med.SQLToClass<medClassBasic, enum_medDrugstore>();
                    returnData.Code = 200;
                    returnData.Result = $"藥局藥檔取得成功,藥局庫存取得耗時[{TaskTime_藥局}],藥庫庫存取得耗時[{TaskTime_藥庫}]";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                //調劑台藥品資料
                if (TableName == "medicine_page")
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];

                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                    //取得雲端藥檔資料
                    List<medClass> medClasses_cloud = new List<medClass>();
                    List<medClass> medClasses_cloud_buf = new List<medClass>();
                    List<medClass> medClasses_src = new List<medClass>();
                    List<medClass> medClasses_src_buf = new List<medClass>();
                    Dictionary<string, List<medClass>> keyValuePairs_medClasses_src = new Dictionary<string, List<medClass>>();
                    Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = new Dictionary<string, List<medClass>>();

                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                    List<object[]> list_med = new List<object[]>();
                    List<object[]> list_med_add = new List<object[]>();

                    List<H_Pannel_lib.DeviceBasic> deviceBasics = new List<H_Pannel_lib.DeviceBasic>();
                    List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();

                    Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = new Dictionary<string, List<DeviceBasic>>();

                    List<Task> tasks = new List<Task>();
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        medClasses_cloud = Get_med_cloud(sys_serverSettingClasses_med);
                        keyValuePairs_medClasses_cloud = medClasses_cloud.CoverToDictionaryByCode();
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        list_med = sQLControl_med.GetAllRows(null);
                        medClasses_src = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                        keyValuePairs_medClasses_src = medClasses_src.CoverToDictionaryByCode();
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        deviceBasics = deviceController.Function_Get_device(sys_serverSettingClasses[0]);
                        keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {


                    })));
                    Task.WhenAll(tasks).Wait();



                    string 藥碼 = "";
                    for (int i = 0; i < medClasses_cloud.Count; i++)
                    {
                        藥碼 = medClasses_cloud[i].藥品碼;
                        medClasses_src = keyValuePairs_medClasses_src.SortDictionaryByCode(藥碼);
                        if (medClasses_src.Count == 0)
                        {
                            object[] value = medClasses_cloud[i].ClassToSQL<medClass, enum_藥品資料_藥檔資料>();
                            value[(int)enum_藥品資料_藥檔資料.GUID] = Guid.NewGuid().ToString();
                            value[(int)enum_藥品資料_藥檔資料.圖片網址] = "";
                            list_med_add.Add(value);
                        }
                    }
                    sQLControl_med.AddRows(null, list_med_add);
                    list_med.LockAdd(list_med_add);
                    list_med.Sort(new Icp_藥品資料_藥檔資料());
                    List<medClass> medClasses = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    for (int i = 0; i < medClasses.Count; i++)
                    {
                        藥碼 = medClasses[i].藥品碼;
                        deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                        double inventory = 0;
                        medClasses[i].DeviceBasics = deviceBasics_buf;
                        for (int k = 0; k < deviceBasics_buf.Count; k++)
                        {
                            inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                        }
                        medClasses[i].庫存 = inventory.ToString();
                    }



                    returnData.Data = medClasses;
                    returnData.Code = 200;
                    returnData.Result = $"調劑台藥檔取得成功!新增<{list_med_add.Count}>筆資料!";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    string json_out = returnData.JsonSerializationt(false);
                    return json_out;
                }

                returnData.Code = -200;
                returnData.Result = "藥檔取得失敗!";
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
        /// 取得[調劑台藥檔]
        /// </summary>
        /// <remarks>
        /// <code>
        /// {
        ///     "ServerName" : "A5",
        ///     "Data": 
        ///     {
        ///         
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///        
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        [Route("get_dps_medClass")]
        [HttpPost]
        public string get_dps_medclass([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string TableName = "medicine_page";
            returnData.ServerType = "調劑台";
            returnData.Method = "get_dps_medclass";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];

                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                //取得雲端藥檔資料
                List<medClass> medClasses_cloud = new List<medClass>();
                List<medClass> medClasses_cloud_buf = new List<medClass>();
                List<medClass> medClasses_src = new List<medClass>();
                List<medClass> medClasses_src_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_src = new Dictionary<string, List<medClass>>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = new Dictionary<string, List<medClass>>();

                SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med = new List<object[]>();
                List<object[]> list_med_add = new List<object[]>();

                List<H_Pannel_lib.DeviceBasic> deviceBasics = new List<H_Pannel_lib.DeviceBasic>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();

                Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = new Dictionary<string, List<DeviceBasic>>();

                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(new Action(delegate
                {
                    medClasses_cloud = Get_med_cloud(sys_serverSettingClasses_med);
                    keyValuePairs_medClasses_cloud = medClasses_cloud.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    list_med = sQLControl_med.GetAllRows(null);
                    medClasses_src = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    keyValuePairs_medClasses_src = medClasses_src.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    deviceBasics = deviceController.Function_Get_device(sys_serverSettingClasses[0]);
                    keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {


                })));
                Task.WhenAll(tasks).Wait();



                string 藥碼 = "";
                //for (int i = 0; i < medClasses_cloud.Count; i++)
                //{
                //    藥碼 = medClasses_cloud[i].藥品碼;
                //    medClasses_src = keyValuePairs_medClasses_src.SortDictionaryByCode(藥碼);
                //    if (medClasses_src.Count == 0)
                //    {
                //        object[] value = medClasses_cloud[i].ClassToSQL<medClass, enum_藥品資料_藥檔資料>();
                //        value[(int)enum_藥品資料_藥檔資料.GUID] = Guid.NewGuid().ToString();
                //        list_med_add.Add(value);
                //    }
                //}
                //sQLControl_med.AddRows(null, list_med_add);
                //list_med.LockAdd(list_med_add);
                list_med.Sort(new Icp_藥品資料_藥檔資料());
                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    藥碼 = medClasses[i].藥品碼;
                    deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                    double inventory = 0;
                    medClasses[i].DeviceBasics = deviceBasics_buf;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }
                    medClasses[i].庫存 = inventory.ToString();
                }



                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"[調劑台]藥檔取得成功!新增<{list_med_add.Count}>筆資料!";
                returnData.TimeTaken = myTimerBasic.ToString();
                string json_out = returnData.JsonSerializationt(false);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以藥碼搜尋[調劑台藥檔]
        /// </summary>
        /// <remarks>
        /// <code>
        /// {
        ///     "ServerName" : "A5",
        ///     "Data": 
        ///     {
        ///         
        ///     },
        ///     "Value" : "[前綴,模糊]",
        ///     "ValueAry" : 
        ///     [
        ///        [藥碼1,藥碼2,藥碼3]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        [Route("get_dps_medClass_by_code")]
        [HttpPost]
        public string get_dps_medClass_by_code([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string TableName = "medicine_page";
            returnData.ServerType = "調劑台";
            returnData.Method = "get_dps_medClass_by_code";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];

                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼1,藥碼2,藥碼3]";
                    return returnData.JsonSerializationt(true);
                }

                //取得雲端藥檔資料
                List<medClass> medClasses_cloud = new List<medClass>();
                List<medClass> medClasses_cloud_buf = new List<medClass>();
                List<medClass> medClasses_src = new List<medClass>();
                List<medClass> medClasses_src_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_src = new Dictionary<string, List<medClass>>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = new Dictionary<string, List<medClass>>();

                SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_src = new List<object[]>();
                List<object[]> list_med = new List<object[]>();
                List<object[]> list_med_buf = new List<object[]>();

                List<H_Pannel_lib.DeviceBasic> deviceBasics = new List<H_Pannel_lib.DeviceBasic>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();

                Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = new Dictionary<string, List<DeviceBasic>>();

                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(new Action(delegate
                {
                    string[] Codes = returnData.ValueAry[0].Split(",");
                    if (Codes.Length == 1)
                    {
                        medClass _medClass = Get_med_cloud(sys_serverSettingClasses_med, Codes[0]);
                        medClasses_cloud.Add(_medClass);
                    }
                    else
                    {
                        medClasses_cloud = Get_med_cloud(sys_serverSettingClasses_med);
                    }

                    keyValuePairs_medClasses_cloud = medClasses_cloud.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    string[] Codes = returnData.ValueAry[0].Split(",");
                    if (Codes.Length == 1)
                    {
                        list_med = sQLControl_med.GetRowsByDefult(null, (int)enum_藥品資料_藥檔資料.藥品碼, Codes[0]);
                    }
                    else
                    {

                        list_med_src = sQLControl_med.GetAllRows(null);
                        List<medClass> medClasses_src = list_med_src.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                        List<medClass> medClasses_src_buf = new List<medClass>();
                        List<medClass> medClasses_src_temp = new List<medClass>();
                        keyValuePairs_medClasses_src = medClasses_src.CoverToDictionaryByCode();
                        for (int i = 0; i < Codes.Length; i++)
                        {
                            medClasses_src_buf = keyValuePairs_medClasses_src.SortDictionaryByCode(Codes[i]);
                            if (medClasses_src_buf.Count > 0)
                            {
                                medClasses_src_temp.Add(medClasses_src_buf[0]);
                            }
                        }
                        list_med = medClasses_src_temp.ClassToSQL<medClass, enum_藥品資料_藥檔資料>();
                    }



                    medClasses_src = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    keyValuePairs_medClasses_src = medClasses_src.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    string[] Codes = returnData.ValueAry[0].Split(",");
                    if (Codes.Length == 1)
                    {
                        deviceBasics = deviceController.Function_讀取儲位_By_Code(sys_serverSettingClasses[0], Codes[0]);
                        keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                    }
                    else
                    {
                        deviceBasics = deviceController.Function_Get_device(sys_serverSettingClasses[0]);

                        keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                    }

                })));

                Task.WhenAll(tasks).Wait();



                string 藥碼 = "";
                list_med.Sort(new Icp_藥品資料_藥檔資料());
                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    藥碼 = medClasses[i].藥品碼;
                    deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                    double inventory = 0;
                    medClasses[i].DeviceBasics = deviceBasics_buf;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }
                    medClasses[i].庫存 = inventory.ToString();
                }



                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"[調劑台] ServerName : {returnData.ServerName} , 藥碼 : {returnData.ValueAry[0]} ,藥檔取得成功 ,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                string json_out = returnData.JsonSerializationt(false);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        [Route("get_dps_medClass_by_codeAsync")]
        [HttpPost]
        public async Task<string> get_dps_medClass_by_codeAsync([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string TableName = "medicine_page";
            returnData.ServerType = "調劑台";
            returnData.Method = "get_dps_medClass_by_codeAsync";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                sys_serverSettingClass sys_serverSettingClasses = await HIS_WebApi.Method.GetServerAsync(returnData.ServerName, returnData.ServerType, "本地端");
                sys_serverSettingClass sys_serverSettingClasses_med = await GetServerAsync.Value;
                if (sys_serverSettingClasses == null || sys_serverSettingClasses_med == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses.Server;
                string DB = sys_serverSettingClasses.DBName;
                string UserName = sys_serverSettingClasses.User;
                string Password = sys_serverSettingClasses.Password;
                uint Port = (uint)sys_serverSettingClasses.Port.StringToDouble();
               
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼1,藥碼2,藥碼3]";
                    return returnData.JsonSerializationt(true);
                }

                //取得雲端藥檔資料
                List<medClass> medClasses_cloud = new List<medClass>();
                List<medClass> medClasses_cloud_buf = new List<medClass>();
                List<medClass> medClasses_src = new List<medClass>();
                List<medClass> medClasses_src_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_src = new Dictionary<string, List<medClass>>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = new Dictionary<string, List<medClass>>();

                SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_src = new List<object[]>();
                List<object[]> list_med = new List<object[]>();
                List<object[]> list_med_buf = new List<object[]>();

                List<H_Pannel_lib.DeviceBasic> deviceBasics = new List<H_Pannel_lib.DeviceBasic>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();

                Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = new Dictionary<string, List<DeviceBasic>>();

                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(new Action(delegate
                {
                    string[] Codes = returnData.ValueAry[0].Split(",");
                    if (Codes.Length == 1)
                    {
                        medClass _medClass = Get_med_cloud(sys_serverSettingClasses_med, Codes[0]);
                        medClasses_cloud.Add(_medClass);
                    }
                    else
                    {
                        medClasses_cloud = Get_med_cloud(sys_serverSettingClasses_med);
                    }

                    keyValuePairs_medClasses_cloud = medClasses_cloud.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    string[] Codes = returnData.ValueAry[0].Split(",");
                    if (Codes.Length == 1)
                    {
                        list_med = sQLControl_med.GetRowsByDefult(null, (int)enum_藥品資料_藥檔資料.藥品碼, Codes[0]);
                    }
                    else
                    {

                        list_med_src = sQLControl_med.GetAllRows(null);
                        List<medClass> medClasses_src = list_med_src.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                        List<medClass> medClasses_src_buf = new List<medClass>();
                        List<medClass> medClasses_src_temp = new List<medClass>();
                        keyValuePairs_medClasses_src = medClasses_src.CoverToDictionaryByCode();
                        for (int i = 0; i < Codes.Length; i++)
                        {
                            medClasses_src_buf = keyValuePairs_medClasses_src.SortDictionaryByCode(Codes[i]);
                            if (medClasses_src_buf.Count > 0)
                            {
                                medClasses_src_temp.Add(medClasses_src_buf[0]);
                            }
                        }
                        list_med = medClasses_src_temp.ClassToSQL<medClass, enum_藥品資料_藥檔資料>();
                    }



                    medClasses_src = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    keyValuePairs_medClasses_src = medClasses_src.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    string[] Codes = returnData.ValueAry[0].Split(",");
                    if (Codes.Length == 1)
                    {
                        deviceBasics = deviceController.Function_讀取儲位_By_Code(sys_serverSettingClasses, Codes[0]);
                        keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                    }
                    else
                    {
                        deviceBasics = deviceController.Function_Get_device(sys_serverSettingClasses);

                        keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                    }

                })));

                Task.WhenAll(tasks).Wait();



                string 藥碼 = "";
                list_med.Sort(new Icp_藥品資料_藥檔資料());
                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    藥碼 = medClasses[i].藥品碼;
                    deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                    double inventory = 0;
                    medClasses[i].DeviceBasics = deviceBasics_buf;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }
                    medClasses[i].庫存 = inventory.ToString();
                }



                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"[調劑台] ServerName : {returnData.ServerName} , 藥碼 : {returnData.ValueAry[0]} ,藥檔取得成功 ,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                string json_out = returnData.JsonSerializationt(false);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以藥名搜尋[調劑台藥檔]
        /// </summary>
        /// <remarks>
        /// <code>
        /// {
        ///     "ServerName" : "A5",
        ///     "Data": 
        ///     {
        ///         
        ///     },
        ///     "Value" : "[前綴]",
        ///     "ValueAry" : 
        ///     [
        ///        [藥名]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        [Route("get_dps_medClass_by_name")]
        [HttpPost]
        public string get_dps_medClass_by_name([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string TableName = "medicine_page";
            returnData.ServerType = "調劑台";
            returnData.Method = "get_dps_medClass_by_name";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];

                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥名]";
                    return returnData.JsonSerializationt(true);
                }

                //取得雲端藥檔資料

                List<medClass> medClasses_src = new List<medClass>();
                List<medClass> medClasses_src_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_src = new Dictionary<string, List<medClass>>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = new Dictionary<string, List<medClass>>();

                SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_src = new List<object[]>();
                List<object[]> list_med = new List<object[]>();
                List<object[]> list_med_buf = new List<object[]>();

                List<H_Pannel_lib.DeviceBasic> deviceBasics = new List<H_Pannel_lib.DeviceBasic>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();

                Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = new Dictionary<string, List<DeviceBasic>>();

                List<Task> tasks = new List<Task>();

                tasks.Add(Task.Run(new Action(delegate
                {
                    list_med_src = sQLControl_med.GetAllRows(null);

                    if (returnData.Value == "前綴")
                    {
                        list_med_buf = list_med_src.GetRowsStartWithByLike((int)enum_藥品資料_藥檔資料.藥品名稱, returnData.ValueAry[0]);
                    }
                    else if (returnData.Value == "標準")
                    {
                        list_med_buf = list_med_src.GetRows((int)enum_藥品資料_藥檔資料.藥品名稱, returnData.ValueAry[0]);
                    }
                    else
                    {
                        list_med_buf = list_med_src.GetRowsByLike((int)enum_藥品資料_藥檔資料.藥品名稱, returnData.ValueAry[0]);
                    }


                    list_med = list_med_buf;
                    medClasses_src = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    keyValuePairs_medClasses_src = medClasses_src.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    deviceBasics = deviceController.Function_Get_device(sys_serverSettingClasses[0]);
                    keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {


                })));
                Task.WhenAll(tasks).Wait();



                string 藥碼 = "";
                list_med.Sort(new Icp_藥品資料_藥檔資料());
                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    藥碼 = medClasses[i].藥品碼;
                    deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                    double inventory = 0;
                    medClasses[i].DeviceBasics = deviceBasics_buf;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }
                    medClasses[i].庫存 = inventory.ToString();
                }



                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"[調劑台] ServerName : {returnData.ServerName} , 藥碼 : {returnData.ValueAry[0]} ,藥檔取得成功 ,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                string json_out = returnData.JsonSerializationt(false);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以中文名搜尋[調劑台藥檔]
        /// </summary>
        /// <remarks>
        /// <code>
        /// {
        ///     "ServerName" : "A5",
        ///     "Data": 
        ///     {
        ///         
        ///     },
        ///     "Value" : "[前綴]",
        ///     "ValueAry" : 
        ///     [
        ///        [中文名]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        [Route("get_dps_medClass_by_chtname")]
        [HttpPost]
        public string get_dps_medClass_by_chtname([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string TableName = "medicine_page";
            returnData.ServerType = "調劑台";
            returnData.Method = "get_dps_medClass_by_chtname";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];

                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[中文名]";
                    return returnData.JsonSerializationt(true);
                }

                //取得雲端藥檔資料

                List<medClass> medClasses_src = new List<medClass>();
                List<medClass> medClasses_src_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_src = new Dictionary<string, List<medClass>>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = new Dictionary<string, List<medClass>>();

                SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_src = new List<object[]>();
                List<object[]> list_med = new List<object[]>();
                List<object[]> list_med_buf = new List<object[]>();

                List<H_Pannel_lib.DeviceBasic> deviceBasics = new List<H_Pannel_lib.DeviceBasic>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();

                Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = new Dictionary<string, List<DeviceBasic>>();

                List<Task> tasks = new List<Task>();

                tasks.Add(Task.Run(new Action(delegate
                {
                    list_med_src = sQLControl_med.GetAllRows(null);

                    if (returnData.Value == "前綴")
                    {
                        list_med_buf = list_med_src.GetRowsStartWithByLike((int)enum_藥品資料_藥檔資料.中文名稱, returnData.ValueAry[0]);
                    }
                    else if (returnData.Value == "標準")
                    {
                        list_med_buf = list_med_src.GetRows((int)enum_藥品資料_藥檔資料.中文名稱, returnData.ValueAry[0]);
                    }
                    else
                    {
                        list_med_buf = list_med_src.GetRowsByLike((int)enum_藥品資料_藥檔資料.中文名稱, returnData.ValueAry[0]);
                    }


                    list_med = list_med_buf;
                    medClasses_src = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    keyValuePairs_medClasses_src = medClasses_src.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    deviceBasics = deviceController.Function_Get_device(sys_serverSettingClasses[0]);
                    keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {


                })));
                Task.WhenAll(tasks).Wait();



                string 藥碼 = "";
                list_med.Sort(new Icp_藥品資料_藥檔資料());
                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    藥碼 = medClasses[i].藥品碼;
                    deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                    double inventory = 0;
                    medClasses[i].DeviceBasics = deviceBasics_buf;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }
                    medClasses[i].庫存 = inventory.ToString();
                }



                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"[調劑台] ServerName : {returnData.ServerName} , 藥碼 : {returnData.ValueAry[0]} ,藥檔取得成功 ,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                string json_out = returnData.JsonSerializationt(false);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以中文名搜尋[調劑台藥檔]
        /// </summary>
        /// <remarks>
        /// <code>
        /// {
        ///     "ServerName" : "A5",
        ///     "Data": 
        ///     {
        ///         
        ///     },
        ///     "Value" : "[前綴]",
        ///     "ValueAry" : 
        ///     [
        ///        [商品名]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        [Route("get_dps_medClass_by_dianame")]
        [HttpPost]
        public string get_dps_medClass_by_dianame([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string TableName = "medicine_page";
            returnData.ServerType = "調劑台";
            returnData.Method = "get_dps_medClass_by_dianame";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];

                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[中文名]";
                    return returnData.JsonSerializationt(true);
                }

                //取得雲端藥檔資料

                List<medClass> medClasses_src = new List<medClass>();
                List<medClass> medClasses_src_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_src = new Dictionary<string, List<medClass>>();
                Dictionary<string, List<medClass>> keyValuePairs_medClasses_cloud = new Dictionary<string, List<medClass>>();

                SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_med_src = new List<object[]>();
                List<object[]> list_med = new List<object[]>();
                List<object[]> list_med_buf = new List<object[]>();

                List<H_Pannel_lib.DeviceBasic> deviceBasics = new List<H_Pannel_lib.DeviceBasic>();
                List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();

                Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = new Dictionary<string, List<DeviceBasic>>();

                List<Task> tasks = new List<Task>();

                tasks.Add(Task.Run(new Action(delegate
                {
                    list_med_src = sQLControl_med.GetAllRows(null);

                    if (returnData.Value == "前綴")
                    {
                        list_med_buf = list_med_src.GetRowsStartWithByLike((int)enum_藥品資料_藥檔資料.藥品學名, returnData.ValueAry[0]);
                    }
                    else if (returnData.Value == "標準")
                    {
                        list_med_buf = list_med_src.GetRows((int)enum_藥品資料_藥檔資料.藥品學名, returnData.ValueAry[0]);
                    }
                    else
                    {
                        list_med_buf = list_med_src.GetRowsByLike((int)enum_藥品資料_藥檔資料.藥品學名, returnData.ValueAry[0]);
                    }


                    list_med = list_med_buf;
                    medClasses_src = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    keyValuePairs_medClasses_src = medClasses_src.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    deviceBasics = deviceController.Function_Get_device(sys_serverSettingClasses[0]);
                    keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();
                })));
                tasks.Add(Task.Run(new Action(delegate
                {


                })));
                Task.WhenAll(tasks).Wait();



                string 藥碼 = "";
                list_med.Sort(new Icp_藥品資料_藥檔資料());
                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    藥碼 = medClasses[i].藥品碼;
                    deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                    double inventory = 0;
                    medClasses[i].DeviceBasics = deviceBasics_buf;
                    for (int k = 0; k < deviceBasics_buf.Count; k++)
                    {
                        inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                    }
                    medClasses[i].庫存 = inventory.ToString();
                }



                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"[調劑台] ServerName : {returnData.ServerName} , 藥碼 : {returnData.ValueAry[0]} ,藥檔取得成功 ,共<{medClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                string json_out = returnData.JsonSerializationt(false);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以藥碼搜尋[調劑台藥檔]
        /// </summary>
        /// <remarks>
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///         
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///        [藥碼]
        ///        [servername1,servername2]
        ///     ]
        ///     
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        [Route("get_datas_dps_medClass_by_code")]
        [HttpPost]
        public string get_datas_dps_medClass_by_code([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string TableName = "medicine_page";
            returnData.ServerType = "調劑台";
            returnData.Method = "get_dps_medClass_by_code";
            returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];

                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼][ServerName1,ServerName2]";
                    return returnData.JsonSerializationt(true);
                }
                string 藥碼 = returnData.ValueAry[0];
                string[] ServerNames = returnData.ValueAry[1].Split(',');

                //取得雲端藥檔資料
                medClass medClass_cloud = Get_med_cloud(sys_serverSettingClasses_med, 藥碼);
                medClass[] medClasses = new medClass[ServerNames.Length];
                if (medClass_cloud == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"線上藥檔查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<Task> tasks = new List<Task>();

                for (int i = 0; i < ServerNames.Length; i++)
                {
                    int index = i;
                    string serverName = ServerNames[i];
                    string serverType = "調劑台";
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        List<H_Pannel_lib.DeviceBasic> deviceBasics = new List<H_Pannel_lib.DeviceBasic>();
                        List<H_Pannel_lib.DeviceBasic> deviceBasics_buf = new List<H_Pannel_lib.DeviceBasic>();

                        Dictionary<string, List<DeviceBasic>> keyValuePairs_deviceBasics = new Dictionary<string, List<DeviceBasic>>();

                        List<sys_serverSettingClass> _sys_serverSettingClasses = sys_serverSettingClasses.MyFind(serverName, serverType, "本地端");
                        if (_sys_serverSettingClasses.Count == 0) return;
                        string Server = _sys_serverSettingClasses[0].Server;
                        string DB = _sys_serverSettingClasses[0].DBName;
                        string UserName = _sys_serverSettingClasses[0].User;
                        string Password = _sys_serverSettingClasses[0].Password;
                        uint Port = (uint)_sys_serverSettingClasses[0].Port.StringToDouble();
                        string TableName = "medicine_page";
                        SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                        List<object[]> list_med = sQLControl_med.GetRowsByDefult(null, (int)enum_藥品資料_藥檔資料.藥品碼, 藥碼);
                        if (list_med.Count > 0)
                        {
                            medClass _medClass = list_med[0].SQLToClass<medClass, enum_藥品資料_藥檔資料>();

                            deviceBasics = deviceController.Function_讀取儲位_By_Code(_sys_serverSettingClasses[0], 藥碼);
                            keyValuePairs_deviceBasics = deviceBasics.CoverToDictionaryByCode();

                            deviceBasics_buf = keyValuePairs_deviceBasics.SortDictionaryByCode(藥碼);
                            double inventory = 0;
                            _medClass.DeviceBasics = deviceBasics_buf;
                            for (int k = 0; k < deviceBasics_buf.Count; k++)
                            {
                                inventory += deviceBasics_buf[k].Inventory.StringToDouble();
                            }
                            _medClass.庫存 = inventory.ToString();

                            _medClass.ServerName = serverName;
                            _medClass.ServerType = serverType;

                            medClasses[index] = _medClass;
                        }
                    })));


                }
                Task.WhenAll(tasks).Wait();




                returnData.Data = medClasses;
                returnData.Code = 200;
                returnData.Result = $"藥碼 : {returnData.ValueAry[0]} ,藥檔取得成功 ,共<{medClasses.Length}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                string json_out = returnData.JsonSerializationt(false);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }

        [Route("get_by_code")]
        [HttpPost]
        public string get_by_code([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                string TableName = returnData.TableName;
                returnData.Method = "get_by_code";
                string Code = returnData.Value;
                if (Code.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "Code空白!";
                    return returnData.JsonSerializationt();
                }
                if (TableName == "medicine_page_cloud")
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "藥檔資料");
                    if (sys_serverSettingClasses_buf.Count == 0)
                    {
                        sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "VM端");
                    }
                    string Server = sys_serverSettingClasses_buf[0].Server;
                    string DB = sys_serverSettingClasses_buf[0].DBName;
                    string UserName = sys_serverSettingClasses_buf[0].User;
                    string Password = sys_serverSettingClasses_buf[0].Password;
                    uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToDouble();
                    if (sys_serverSettingClasses_buf.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }

                    CheckCreatTable(returnData);
                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                    List<object[]> list_med = sQLControl_med.GetRowsByDefult(null, (int)enum_雲端藥檔.藥品碼, Code);
                    returnData.Data = list_med.SQLToClass<medClass, enum_雲端藥檔>();
                    returnData.Code = 200;
                    returnData.Result = "雲端藥檔取得成功!";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                if (TableName == "medicine_page_firstclass")
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "藥檔資料");
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                    List<object[]> list_med = sQLControl_med.GetRowsByDefult(null, (int)enum_medDrugstore.藥品碼, Code);
                    returnData.Data = list_med.SQLToClass<medClass, enum_medDrugstore>();
                    returnData.Code = 200;
                    returnData.Result = "藥庫藥檔取得成功!";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                if (TableName == "medicine_page_phar")
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "藥檔資料");
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                    List<object[]> list_med = sQLControl_med.GetRowsByDefult(null, (int)enum_medDrugstore.藥品碼, Code);
                    returnData.Data = list_med.SQLToClass<medClass, enum_medDrugstore>();
                    returnData.Code = 200;
                    returnData.Result = "藥局藥檔取得成功!";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                if (TableName == "medicine_page")
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }
                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                    List<object[]> list_med = sQLControl_med.GetRowsByDefult(null, (int)enum_藥品資料_藥檔資料.藥品碼, Code);
                    returnData.Data = list_med.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    returnData.Code = 200;
                    returnData.Result = "調劑台藥檔取得成功!";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }

                returnData.Code = -200;
                returnData.Result = "藥檔取得失敗!";
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        [Route("upadte_by_guid")]
        [HttpPost]
        public string upadte_by_guid([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                string TableName = returnData.TableName;
                returnData.Method = "upadte_by_guid";


                List<medClass> medClasses = new List<medClass>();
                medClasses = returnData.Data.ObjToListClass<medClass>();
                if (medClasses == null)
                {
                    medClass medClass = returnData.Data.ObjToClass<medClass>();
                    if (medClass != null)
                    {
                        medClasses = new List<medClass>();
                        medClasses.Add(medClass);
                    }
                }
                if (medClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "反序列化失敗!";
                    return returnData.JsonSerializationt();
                }
                if (TableName == "medicine_page_cloud")
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }

                    CheckCreatTable(returnData);
                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                    List<object[]> list_value = sQLControl_med.GetAllRows(null);
                    List<object[]> list_value_buf = new List<object[]>();
                    List<object[]> list_value_add = new List<object[]>();
                    List<object[]> list_value_replace = new List<object[]>();
                    for (int i = 0; i < medClasses.Count; i++)
                    {
                        string Code = medClasses[i].藥品碼;
                        list_value_buf = list_value.GetRows((int)enum_雲端藥檔.藥品碼, Code);
                        if (list_value_buf.Count == 0)
                        {
                            object[] value = medClasses[i].ClassToSQL<medClass, enum_雲端藥檔>();
                            value[(int)enum_雲端藥檔.GUID] = Guid.NewGuid().ToString();
                            list_value_add.Add(value);
                        }
                        else
                        {
                            object[] value = medClasses[i].ClassToSQL<medClass, enum_雲端藥檔>();
                            value[(int)enum_雲端藥檔.GUID] = list_value_buf[0][(int)enum_雲端藥檔.GUID].ObjectToString();
                            list_value_replace.Add(value);
                        }

                    }

                    sQLControl_med.UpdateByDefulteExtra(null, list_value_replace);
                    sQLControl_med.AddRows(null, list_value_add);
                    returnData.Code = 200;
                    returnData.Result = $"雲端藥檔更新成功!新增<{list_value_add.Count}>筆,修改<{list_value_replace.Count}>筆";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                if (TableName == "medicine_page_firstclass")
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "藥檔資料");
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }

                    CheckCreatTable(returnData);
                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                    List<object[]> list_replace = medClasses.ClassToSQL<medClass, enum_medDrugstore>();
                    sQLControl_med.UpdateByDefulteExtra(null, list_replace);
                    returnData.Code = 200;
                    returnData.Result = "藥庫藥檔更新成功!";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                if (TableName == "medicine_page_phar")
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "藥檔資料");
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }

                    CheckCreatTable(returnData);
                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                    List<object[]> list_replace = medClasses.ClassToSQL<medClass, enum_medDrugstore>();
                    sQLControl_med.UpdateByDefulteExtra(null, list_replace);
                    returnData.Code = 200;
                    returnData.Result = "藥局藥檔更新成功!";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                if (TableName == "medicine_page")
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                    string Server = sys_serverSettingClasses[0].Server;
                    string DB = sys_serverSettingClasses[0].DBName;
                    string UserName = sys_serverSettingClasses[0].User;
                    string Password = sys_serverSettingClasses[0].Password;
                    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToDouble();
                    if (sys_serverSettingClasses.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"找無Server資料!";
                        return returnData.JsonSerializationt();
                    }

                    CheckCreatTable(returnData);
                    SQLControl sQLControl_med = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                    List<object[]> list_replace = medClasses.ClassToSQL<medClass, enum_藥品資料_藥檔資料>();
                    sQLControl_med.UpdateByDefulteExtra(null, list_replace);
                    returnData.Code = 200;
                    returnData.Result = "調劑台藥檔更新成功!";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                returnData.Code = -200;
                returnData.Result = "更新藥檔失敗!";

                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }

        [Route("serch_by_BarCode")]
        [HttpPost]
        public string serch_by_BarCode([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                List<medClass> medClasses = returnData.Data.ObjToListClass<medClass>();
                if (medClasses == null || medClasses.Count == 0)
                {
                    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                    sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];
                    medClasses = Get_med_cloud(sys_serverSettingClasses_med);
                }

                returnData.Method = "serch_by_BarCode";
                string BarCode = returnData.Value;
                if (BarCode.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "BarCode空白!";
                    return returnData.JsonSerializationt();
                }

                List<medClass> medClasses_buf = new List<medClass>();

                for (int i = 0; i < medClasses.Count; i++)
                {
                    if (BarCode == medClasses[i].藥品碼)
                    {
                        medClasses_buf.Add(medClasses[i]);
                        continue;
                    }
                    if (BarCode == medClasses[i].料號)
                    {
                        medClasses_buf.Add(medClasses[i]);
                        continue;
                    }
                    foreach (string barcode in medClasses[i].Barcode)
                    {
                        if (barcode == BarCode)
                        {
                            medClasses_buf.Add(medClasses[i]);
                            continue;
                        }
                    }
                }
                if (medClasses_buf.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.Data = medClasses_buf;
                returnData.Code = 200;
                returnData.Result = "搜尋BARCODE完成!";
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        [Route("excel_upload")]
        [HttpPost]
        public async Task<string> excel_upload([FromForm] IFormFile file)
        {
            returnData returnData = new returnData();
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();


                returnData.Method = "excel_upload";
                var formFile = Request.Form.Files.FirstOrDefault();

                if (formFile == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "文件不得為空";
                    return returnData.JsonSerializationt(true);
                }

                string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                inventoryClass.creat creat = new inventoryClass.creat();
                string error = "";
                List<medClass> medClasses = new List<medClass>();
                string str = "";
                List<string> 中西藥 = new List<string> { "中藥", "西藥" };
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);
                    dt = dt.ReorderTable(new enum_雲端藥檔());
                    if (dt == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = "上傳文件表頭無效!";
                        return returnData.JsonSerializationt(true);
                    }
                    List<object[]> list_value = dt.DataTableToRowList();
                    if (list_value.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"文件內容不得為空";
                        return returnData.JsonSerializationt(true);
                    }
                    medClasses = list_value.SQLToClass<medClass,enum_雲端藥檔>();
                    //for (int i = 0; i < list_value.Count; i++)
                    //{
                    //    string 中西藥品 = list_value[i][(int)enum_雲端藥檔_EXCEL.中西藥].ObjectToString();
                    //    if (!中西藥.Contains(中西藥品))
                    //    {
                    //        string 藥品名稱 = list_value[i][(int)enum_雲端藥檔_EXCEL.藥名].ObjectToString();
                    //        str += $"藥品 :{藥品名稱} 未加入成功，中西藥欄位需填入\"中藥\"或\"西藥\"，";
                    //        continue;
                    //    }

                    //    medClass medClass = new medClass
                    //    {
                    //        藥品碼 = list_value[i][(int)enum_雲端藥檔_EXCEL.藥碼].ObjectToString(),
                    //        中文名稱 = list_value[i][(int)enum_雲端藥檔_EXCEL.中文名].ObjectToString(),
                    //        藥品名稱 = list_value[i][(int)enum_雲端藥檔_EXCEL.藥名].ObjectToString(),
                    //        藥品學名 = list_value[i][(int)enum_雲端藥檔_EXCEL.藥品學名].ObjectToString(),
                    //        健保碼 = list_value[i][(int)enum_雲端藥檔_EXCEL.健保碼].ObjectToString(),
                    //        包裝單位 = list_value[i][(int)enum_雲端藥檔_EXCEL.包裝單位].ObjectToString(),
                    //        庫存 = list_value[i][(int)enum_雲端藥檔_EXCEL.庫存].ObjectToString(),
                    //        安全庫存 = list_value[i][(int)enum_雲端藥檔_EXCEL.安全庫存].ObjectToString(),
                    //        警訊藥品 = list_value[i][(int)enum_雲端藥檔_EXCEL.警訊藥品].ObjectToString(),
                    //        高價藥品 = list_value[i][(int)enum_雲端藥檔_EXCEL.高價藥品].ObjectToString(),
                    //        管制級別 = list_value[i][(int)enum_雲端藥檔_EXCEL.管制級別].ObjectToString(),
                    //        類別 = list_value[i][(int)enum_雲端藥檔_EXCEL.類別].ObjectToString(),
                    //        廠牌 = list_value[i][(int)enum_雲端藥檔_EXCEL.廠牌].ObjectToString(),
                    //        藥品許可證號 = list_value[i][(int)enum_雲端藥檔_EXCEL.藥品許可證號].ObjectToString(),
                    //        中西藥 = list_value[i][(int)enum_雲端藥檔_EXCEL.中西藥].ObjectToString()
                    //    };
                    //    medClasses.Add(medClass);
                    //}
                }
                if (medClasses.Count > 0)
                {
                    medClass.add_med_clouds("http://127.0.0.1:4433", medClasses);
                    returnData.Data = medClasses;
                    returnData.Code = 200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"{str}共新增{medClasses.Count}筆藥檔";
                    return returnData.JsonSerializationt(true);
                }
                else
                {
                    returnData.Data = medClasses;
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"{str}";
                    return returnData.JsonSerializationt(true);
                }

            }

            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        [Route("excel_download")]
        [HttpGet]
        public IActionResult excel_download()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                var names = Enum.GetNames(typeof(enum_雲端藥檔));
                int i = 1;
                foreach (var name in names)
                {
                    if (name == "GUID") continue;
                    worksheet.Cells[1, i].Value = name;
                    i++;
                }
                //worksheet.Cells[1, 1].Value = enum_雲端藥檔.藥品碼.GetEnumName();

                //worksheet.Cells[1, 1].Value = "藥碼";
                //worksheet.Cells[1, 2].Value = "中文名";
                //worksheet.Cells[1, 3].Value = "藥名";
                //worksheet.Cells[1, 4].Value = "藥品學名";
                //worksheet.Cells[1, 5].Value = "健保碼";
                //worksheet.Cells[1, 6].Value = "包裝單位";
                //worksheet.Cells[1, 7].Value = "庫存";
                //worksheet.Cells[1, 8].Value = "安全庫存";
                //worksheet.Cells[1, 9].Value = "警訊藥品";
                //worksheet.Cells[1, 10].Value = "高價藥品";
                //worksheet.Cells[1, 11].Value = "管制級別";
                //worksheet.Cells[1, 12].Value = "類別";
                //worksheet.Cells[1, 13].Value = "廠牌";
                //worksheet.Cells[1, 14].Value = "藥品許可證號";
                //worksheet.Cells[1, 15].Value = "中西藥";


                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = "medpage.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }
        /// <summary>
        ///下載藥檔
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "Data":[{medClass}]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("download_med_cloud")]
        public async Task<ActionResult> download_med_cloud([FromBody] returnData returnData)
        {
            try
            {

                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data 無傳入資料";
                    return Content($"下載失敗：{returnData.Result}");
                }

                List<medClass> medClasses = returnData.Data.ObjToClass<List<medClass>>();
                if (medClasses == null)
                {
                    //medClasses = new List<medClass>();
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data 無傳入資料";
                    return Content($"下載失敗：{returnData.Result}");
                }
                List<object[]> objects = medClasses.ClassToSQL<medClass, enum_雲端藥檔>();
                System.Data.DataTable dataTable = objects.ToDataTable(new enum_雲端藥檔());
                if (dataTable.Columns.Count > 0)
                {
                    dataTable.Columns.RemoveAt(0);
                }
                //dataTable = dataTable.ReorderTable(new enum_med_cpoe_export());

                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";
                byte[] excelData = MyOffice.ExcelClass.NPOI_GetBytes(dataTable, Excel_Type.xlsx);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_藥檔.xlsx"));
            }
            catch
            {
                return null;
            }

        }
        static public Dictionary<string, List<H_Pannel_lib.DeviceSimple>> ConvertToDictionary(List<H_Pannel_lib.DeviceSimple> deviceSimples)
        {
            Dictionary<string, List<H_Pannel_lib.DeviceSimple>> dictionary = new Dictionary<string, List<H_Pannel_lib.DeviceSimple>>();

            foreach (var item in deviceSimples)
            {
                string _key = item.Code;

                // 如果字典中已經存在該索引鍵，則將值添加到對應的列表中
                if (dictionary.ContainsKey(_key))
                {
                    dictionary[_key].Add(item);
                }
                // 否則創建一個新的列表並添加值
                else
                {
                    List<H_Pannel_lib.DeviceSimple> values = new List<H_Pannel_lib.DeviceSimple> { item };
                    dictionary[_key] = values;
                }
            }

            return dictionary;
        }
        private string CheckCreatTable(returnData returnData)
        {
            string TableName = returnData.TableName;
            Table table = new Table(TableName);
            if (TableName == "medicine_page_cloud")
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "藥檔資料");

                returnData.Method = "get_init";

                if (sys_serverSettingClasses.Count == 0)
                {
                    return $"找無Server資料!";
                }
                table = MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_雲端藥檔());
            }
            if (TableName == "medicine_page_firstclass")
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                returnData.Method = "get_init";

                if (sys_serverSettingClasses.Count == 0)
                {
                    return $"找無Server資料!";
                }


                table = MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medDrugstore());
            }
            if (TableName == "medicine_page_phar")
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                returnData.Method = "get_init";

                if (sys_serverSettingClasses.Count == 0)
                {
                    return $"找無Server資料!";
                }

                table = MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medPharmacy());
            }

            if (TableName == "medicine_page")
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                returnData.Method = "get_init";

                if (sys_serverSettingClasses.Count == 0)
                {
                    return $"找無Server資料!";
                }

                table = MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_藥品資料_藥檔資料());
            }


            return table.JsonSerializationt(true);
        }
        private async Task<string> CheckCreatTableAsync(returnData returnData)
        {
            string TableName = returnData.TableName;
            Table table = new Table(TableName);
            if (TableName == "medicine_page_cloud")
            {
            
                sys_serverSettingClass sys_serverSettingClass = await HIS_WebApi.Method.GetServerAsync(returnData.ServerName, returnData.ServerType, "藥檔資料");

                returnData.Method = "get_init";

                if (sys_serverSettingClass == null)
                {
                    return $"找無Server資料!";
                }
                table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_雲端藥檔());
            }
            if (TableName == "medicine_page_firstclass")
            {
                sys_serverSettingClass sys_serverSettingClass = await HIS_WebApi.Method.GetServerAsync(returnData.ServerName, returnData.ServerType, "本地端");

                returnData.Method = "get_init";

                if (sys_serverSettingClass == null)
                {
                    return $"找無Server資料!";
                }
                table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_medDrugstore());
            }
            if (TableName == "medicine_page_phar")
            {
         
                sys_serverSettingClass sys_serverSettingClass = await HIS_WebApi.Method.GetServerAsync(returnData.ServerName, returnData.ServerType, "本地端");

                returnData.Method = "get_init";

                if (sys_serverSettingClass == null)
                {
                    return $"找無Server資料!";
                }

                table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_medPharmacy());
            }

            if (TableName == "medicine_page")
            {
                
                sys_serverSettingClass sys_serverSettingClass = await HIS_WebApi.Method.GetServerAsync(returnData.ServerName, returnData.ServerType, "本地端");

                returnData.Method = "get_init";

                if (sys_serverSettingClass == null)
                {
                    return $"找無Server資料!";
                }

                table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_藥品資料_藥檔資料());
            }


            return table.JsonSerializationt(true);
        }
        static public medClass Get_med_cloud(sys_serverSettingClass sys_serverSettingClass, string code)
        {
            List<medClass> medClasses = Get_med_cloud(sys_serverSettingClass, new string[] { code });
            if (medClasses.Count == 0) return null;
            return medClasses[0];
        }
        static public List<medClass> Get_med_cloud(sys_serverSettingClass sys_serverSettingClass, string[] Codes)
        {
            try
            {
                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToDouble();
                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);

                List<object[]> list_med = new List<object[]>();

                string sqlList = string.Join(", ", Codes.Select(code => $"'{code}'"));
                //string command = $"select * from {DB}.{"medicine_page_cloud"} where UPPER({enum_雲端藥檔.藥品碼.GetEnumName()}) in ({sqlList});";
                string command = $"select * from {DB}.{"medicine_page_cloud"} where {enum_雲端藥檔.藥品碼.GetEnumName()} in ({sqlList});";

                DataTable dataTable = sQLControl_med.WtrteCommandAndExecuteReader(command);
                list_med = dataTable.DataTableToRowList();

                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_雲端藥檔>();

                return medClasses;
            }
            catch
            {
                return null;
            }
        }
        static public List<medClass> Get_med_cloud(sys_serverSettingClass sys_serverSettingClass)
        {
            try
            {
                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToDouble();
                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);
                
                List<object[]> list_med = sQLControl_med.GetAllRows(null);
                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_雲端藥檔>();
                return medClasses;
            }
            catch
            {
                return null;
            }
        }
        static public async Task<medClass> Get_med_cloudAsync(sys_serverSettingClass sys_serverSettingClass, string Code)
        {
            List<medClass> medClasses = await Get_med_cloudAsync(sys_serverSettingClass, new string[] { Code });
            if (medClasses.Count == 0) return null;
            return medClasses[0];
        }
        static public async Task<List<medClass>> Get_med_cloudAsync(sys_serverSettingClass sys_serverSettingClass, string[] Codes)
        {
            try
            {

                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToDouble();
                SQLControl sQLControl_med = new SQLControl(Server, DB, "medicine_page_cloud", UserName, Password, Port, SSLMode);

                string sqlList = string.Join(", ", Codes.Select(code => $"'{code}'"));
                string command = $"select * from {DB}.{"medicine_page_cloud"} where UPPER({enum_雲端藥檔.藥品碼.GetEnumName()}) in ({sqlList});";
                List<object[]> list_med = await sQLControl_med.WriteCommandAsync(command);

                List<medClass> medClasses = list_med.SQLToClass<medClass, enum_雲端藥檔>();

                return medClasses;
            }
            catch
            {
                return null;
            }
        }
        static public async Task<List<medClass>> Get_med_cloudAsync()
        {
            try
            {
                returnData returnData = new returnData();
                string result = await new MED_pageController().get_med_cloud(returnData);
                returnData = result.JsonDeserializet<returnData>();
                if (returnData == null || returnData.Code != 200) return new List<medClass>();
                List<medClass> medClasses = returnData.Data.ObjToClass<List<medClass>>();
                return medClasses;
            }
            catch
            {
                return null;
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_by_code(string code)
        {
            returnData returnData = new returnData();
            returnData.Value = code;
            returnData.TableName = "medicine_page_cloud";
            string result = get_by_code(returnData);
            return result.JsonDeserializet<returnData>();
        }

        public class Icp_藥品資料_藥檔資料 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string 藥品碼_0 = x[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
                string 藥品碼_1 = y[(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
                return 藥品碼_0.CompareTo(藥品碼_1);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_med_clouds_by_codes(string[] codes)
        {
            returnData returnData = new returnData();
            string code = string.Join(",", codes);
            returnData.ValueAry.Add(code);
            string result = await get_med_clouds_by_codes(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_med_cloud()
        {
            returnData returnData = new returnData();
            string result = await get_med_cloud(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }
    }
}