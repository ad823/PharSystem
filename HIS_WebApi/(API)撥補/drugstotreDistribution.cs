using System;
using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class drugStotreDistribution : ControllerBase
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        /// <summary>
        /// 初始化撥補紀錄資料庫
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ds01",
        ///     "ServerType" : "藥庫",
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
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(drugStotreDistributionClass))]
        [HttpPost]
        public string POST_init([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);

            try
            {
                //returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

                returnData.Method = "POST_init";
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    returnData.TimeTaken = $"{myTimerBasic}";
                    return returnData.JsonSerializationt();
                }
                return CheckCreatTable(sys_serverSettingClasses[0]);
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
        /// 新增撥補資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ds01",
        ///     "ServerType" : "藥庫",
        ///     "Data": 
        ///     {
        ///         [drugStotreDistributionClass]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("add")]
        [HttpPost]
        public string POST_add(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "add";
            try
            {
                POST_init(returnData);
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses[0];
                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

                List<drugStotreDistributionClass> drugstotreDistributions = returnData.Data.ObjToClass<List<drugStotreDistributionClass>>();

                SQLControl sQLControl_drugstotreDistribution = new SQLControl(Server, DB, new enum_drugStotreDistribution().GetEnumDescription(), UserName, Password, Port, SSLMode);
                List<object[]> list_drugstotreDistributions = drugstotreDistributions.ClassToSQL<drugStotreDistributionClass, enum_drugStotreDistribution>();
                for (int i = 0; i < list_drugstotreDistributions.Count; i++)
                {
                    list_drugstotreDistributions[i][(int)enum_drugStotreDistribution.GUID] = Guid.NewGuid().ToString();
                    list_drugstotreDistributions[i][(int)enum_drugStotreDistribution.加入時間] = DateTime.Now.ToDateTimeString_6();
                    list_drugstotreDistributions[i][(int)enum_drugStotreDistribution.報表生成時間] = DateTime.Now.ToDateTimeString_6();
                    list_drugstotreDistributions[i][(int)enum_drugStotreDistribution.撥發時間] = DateTime.MinValue.ToDateTimeString_6();
                    list_drugstotreDistributions[i][(int)enum_drugStotreDistribution.簽收時間] = DateTime.MinValue.ToDateTimeString_6();

                }

                sQLControl_drugstotreDistribution.AddRows(null, list_drugstotreDistributions);
                List<drugStotreDistributionClass> drugstotre = list_drugstotreDistributions.SQLToClass<drugStotreDistributionClass, enum_drugStotreDistribution>();

                returnData.Result = $"新增撥補資料成功,共<{list_drugstotreDistributions.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Code = 200;
                returnData.Data = drugstotre;
                Logger.LogAddLine($"drugstotreDistribution");
                Logger.Log($"drugstotreDistribution", $"{ returnData.JsonSerializationt(true)}");
                Logger.LogAddLine($"drugstotreDistribution");
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"drugstotreDistribution", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新撥補資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ds01",
        ///     "ServerType" : "藥庫",
        ///     "Data": 
        ///     {
        ///         [drugStotreDistributionClass]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [HttpPost("delete_by_guid")]
        public string delete_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "delete_by_guid";
            try
            {
                if(returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID]";
                    return returnData.JsonSerializationt(true);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                string GUID = returnData.ValueAry[0];

                SQLControl sQLControl_drugstotreDistribution = new SQLControl(Server, DB, new enum_drugStotreDistribution().GetEnumDescription(), UserName, Password, Port, SSLMode);
                List<object[]> list_drugstotreDistributions = sQLControl_drugstotreDistribution.GetRowsByLike(null, (int)enum_drugStotreDistribution.GUID, GUID);
                
                if(list_drugstotreDistributions.Count() == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此GUID{GUID}";
                    return returnData.JsonSerializationt(true);
                }
                if (list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.狀態].ToString() != "等待過帳")
                {
                    returnData.Code = -200;
                    returnData.Result = $"此GUID狀態為<{list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.狀態]}> 不得刪除";
                    return returnData.JsonSerializationt(true);
                }
                sQLControl_drugstotreDistribution.DeleteExtra(null, list_drugstotreDistributions);

                returnData.Result = $"刪除撥補資料,共<{list_drugstotreDistributions.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Code = 200;

                Logger.LogAddLine($"delete_by_guid");
                Logger.Log($"drugstotreDistribution", $"{returnData.JsonSerializationt(true)}");
                Logger.LogAddLine($"delete_by_guid");
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"drugstotreDistribution", $"[異常] {returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新撥補資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ds01",
        ///     "ServerType" : "藥庫",
        ///     "Data": 
        ///     {
        ///         [drugStotreDistributionClass]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [HttpPost("update_qty_by_guid")]
        public string update_qty_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_qty_by_guid";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID,撥發量]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID,撥發量]";
                    return returnData.JsonSerializationt(true);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                string GUID = returnData.ValueAry[0];
                string 撥發量 = returnData.ValueAry[1];

                SQLControl sQLControl_drugstotreDistribution = new SQLControl(Server, DB, new enum_drugStotreDistribution().GetEnumDescription(), UserName, Password, Port, SSLMode);
                List<object[]> list_drugstotreDistributions = sQLControl_drugstotreDistribution.GetRowsByLike(null, (int)enum_drugStotreDistribution.GUID, GUID);

                if (list_drugstotreDistributions.Count() == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此GUID{GUID}";
                    return returnData.JsonSerializationt(true);
                }
                if (list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.狀態].ToString() != "等待過帳")
                {
                    returnData.Code = -200;
                    returnData.Result = $"此GUID狀態為<{list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.狀態]}> 不得更改撥發量";
                    return returnData.JsonSerializationt(true);
                }
                list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.撥發量] = 撥發量;
                sQLControl_drugstotreDistribution.UpdateByDefulteExtra(null, list_drugstotreDistributions);
                List<drugStotreDistributionClass> drugstotreDistributions = list_drugstotreDistributions.SQLToClass<drugStotreDistributionClass, enum_drugStotreDistribution>();

                returnData.Data = drugstotreDistributions;
                returnData.Result = $"更新資料{GUID} 撥發量{撥發量}";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Code = 200;

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"drugstotreDistribution", $"[異常] {returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新撥補資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ds01",
        ///     "ServerType" : "藥庫",
        ///     "ValueAry": 
        ///     [
        ///         [drugStotreDistributionClass]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [HttpPost("update_actqty_by_guid")]
        public string update_actqty_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_actqty_by_guid";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID,撥發量]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID,撥發量]";
                    return returnData.JsonSerializationt(true);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                string GUID = returnData.ValueAry[0];
                string 實撥量 = returnData.ValueAry[1];

                SQLControl sQLControl_drugstotreDistribution = new SQLControl(Server, DB, new enum_drugStotreDistribution().GetEnumDescription(), UserName, Password, Port, SSLMode);
                List<object[]> list_drugstotreDistributions = sQLControl_drugstotreDistribution.GetRowsByLike(null, (int)enum_drugStotreDistribution.GUID, GUID);

                if (list_drugstotreDistributions.Count() == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此GUID{GUID}";
                    return returnData.JsonSerializationt(true);
                }
                if (list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.狀態].ToString() != "等待過帳")
                {
                    returnData.Code = -200;
                    returnData.Result = $"此GUID狀態為<{list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.狀態]}> 不得更改實撥量";
                    return returnData.JsonSerializationt(true);
                }
                list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.實撥量] = 實撥量;
                sQLControl_drugstotreDistribution.UpdateByDefulteExtra(null, list_drugstotreDistributions);
                List<drugStotreDistributionClass> drugstotreDistributions = list_drugstotreDistributions.SQLToClass<drugStotreDistributionClass, enum_drugStotreDistribution>();

                returnData.Data = drugstotreDistributions;
                returnData.Result = $"更新資料{GUID} 撥發量{實撥量}";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Code = 200;

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"drugstotreDistribution", $"[異常] {returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新撥補資料(簽收數量)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ds01",
        ///     "ServerType" : "藥庫",
        ///     "ValueAry": 
        ///     [
        ///         [drugStotreDistributionClass]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [HttpPost("update_signedqty_by_guid")]
        public string update_signedqty_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_actqty_by_guid";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID,簽收量]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID,簽收量]";
                    return returnData.JsonSerializationt(true);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                string GUID = returnData.ValueAry[0];
                string 簽收量 = returnData.ValueAry[1];

                SQLControl sQLControl_drugstotreDistribution = new SQLControl(Server, DB, new enum_drugStotreDistribution().GetEnumDescription(), UserName, Password, Port, SSLMode);
                List<object[]> list_drugstotreDistributions = sQLControl_drugstotreDistribution.GetRowsByLike(null, (int)enum_drugStotreDistribution.GUID, GUID);

                if (list_drugstotreDistributions.Count() == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此GUID{GUID}";
                    return returnData.JsonSerializationt(true);
                }
                if (list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.狀態].ToString() != "已過帳")
                {
                    returnData.Code = -200;
                    returnData.Result = $"此GUID狀態為<{list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.狀態]}> 不得更改簽收量";
                    return returnData.JsonSerializationt(true);
                }
                list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.簽收量] = 簽收量;
                sQLControl_drugstotreDistribution.UpdateByDefulteExtra(null, list_drugstotreDistributions);
                List<drugStotreDistributionClass> drugstotreDistributions = list_drugstotreDistributions.SQLToClass<drugStotreDistributionClass, enum_drugStotreDistribution>();

                returnData.Data = drugstotreDistributions;
                returnData.Result = $"更新資料{GUID} 簽收量{簽收量}";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Code = 200;

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"drugstotreDistribution", $"[異常] {returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新撥補資料(簽收數量)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ds01",
        ///     "ServerType" : "藥庫",
        ///     "ValueAry": 
        ///     [
        ///         ["GUID","True/False"]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [HttpPost("update_notice")]
        public string update_notice(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_actqty_by_guid";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID,True/False]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID,True/False]";
                    return returnData.JsonSerializationt(true);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                string GUID = returnData.ValueAry[0];
                string flag_notice = returnData.ValueAry[1];

                SQLControl sQLControl_drugstotreDistribution = new SQLControl(Server, DB, new enum_drugStotreDistribution().GetEnumDescription(), UserName, Password, Port, SSLMode);
                List<object[]> list_drugstotreDistributions = sQLControl_drugstotreDistribution.GetRowsByLike(null, (int)enum_drugStotreDistribution.GUID, GUID);

                if (list_drugstotreDistributions.Count() == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此GUID{GUID}";
                    return returnData.JsonSerializationt(true);
                }
                
                list_drugstotreDistributions[0][(int)enum_drugStotreDistribution.通知註記] = flag_notice;
                sQLControl_drugstotreDistribution.UpdateByDefulteExtra(null, list_drugstotreDistributions);
                List<drugStotreDistributionClass> drugstotreDistributions = list_drugstotreDistributions.SQLToClass<drugStotreDistributionClass, enum_drugStotreDistribution>();

                returnData.Data = drugstotreDistributions;
                returnData.Result = $"更新資料{GUID} 通知註記{flag_notice}";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Code = 200;

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"drugstotreDistribution", $"[異常] {returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新撥補資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ds01",
        ///     "ServerType" : "藥庫",
        ///     "Data": 
        ///     {
        ///         [drugStotreDistributionClass]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("update_by_guid")]
        [HttpPost]
        public string POST_update_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_by_guid";
            try
            {
                POST_init(returnData);
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses[0];
                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

                List<drugStotreDistributionClass> drugstotreDistributions = returnData.Data.ObjToClass<List<drugStotreDistributionClass>>();

                SQLControl sQLControl_drugstotreDistribution = new SQLControl(Server, DB, new enum_drugStotreDistribution().GetEnumDescription(), UserName, Password, Port, SSLMode);
                List<object[]> list_drugstotreDistributions = drugstotreDistributions.ClassToSQL<drugStotreDistributionClass, enum_drugStotreDistribution>();

                sQLControl_drugstotreDistribution.UpdateByDefulteExtra(null, list_drugstotreDistributions);

                returnData.Result = $"更新撥補資料,共<{list_drugstotreDistributions.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Code = 200;
            
                Logger.LogAddLine($"drugstotreDistribution");
                Logger.Log($"drugstotreDistribution", $"{ returnData.JsonSerializationt(true)}");
                Logger.LogAddLine($"drugstotreDistribution");
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"drugstotreDistribution", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得撥補資料(新增時間範圍)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///         [drugStotreDistributionClass]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///        "起始時間",
        ///        "結束時間"
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_by_addedTime")]
        [HttpPost]
        public string get_by_addedTime(returnData returnData)
        {
            POST_init(returnData);
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_by_addedTime";
            try
            {
                POST_init(returnData);
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[起始時間][結束時間]";
                    return returnData.JsonSerializationt(true);
                }
                string 起始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];
                if (起始時間.Check_Date_String() == false || 結束時間.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"時間範圍格式錯誤";
                    return returnData.JsonSerializationt(true);
                }
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses[0];
                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

              

                SQLControl sQLControl_drugstotreDistribution = new SQLControl(Server, DB, new enum_drugStotreDistribution().GetEnumDescription(), UserName, Password, Port, SSLMode);
                List<object[]> list_drugstotreDistributions = new List<object[]>();

                list_drugstotreDistributions =  sQLControl_drugstotreDistribution.GetRowsByBetween(null, (int)enum_drugStotreDistribution.加入時間, 起始時間, 結束時間);

                list_drugstotreDistributions.Sort((x, y) => y[(int)enum_drugStotreDistribution.加入時間].StringToDateTime().CompareTo(x[(int)enum_drugStotreDistribution.加入時間].StringToDateTime()));

                List<drugStotreDistributionClass> drugstotreDistributions = list_drugstotreDistributions.SQLToClass<drugStotreDistributionClass , enum_drugStotreDistribution>();


                returnData.Result = $"取得撥補資料(新增時間範圍),共<{list_drugstotreDistributions.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = drugstotreDistributions;
                returnData.Code = 200;
                Logger.LogAddLine($"drugstotreDistribution");
                Logger.Log($"drugstotreDistribution", $"{ returnData.JsonSerializationt(true)}");
                Logger.LogAddLine($"drugstotreDistribution");
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"drugstotreDistribution", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得撥補資料(GUID)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///         [drugStotreDistributionClass]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///        "GUID",
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        [Route("get_by_guid")]
        [HttpPost]
        public string get_by_guid(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_by_guid";
            try
            {
                POST_init(returnData);
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID]";
                    return returnData.JsonSerializationt(true);
                }
                string GUID = returnData.ValueAry[0];
         
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses[0];
                string Server = sys_serverSettingClass.Server;
                string DB = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();



                SQLControl sQLControl_drugstotreDistribution = new SQLControl(Server, DB, new enum_drugStotreDistribution().GetEnumDescription(), UserName, Password, Port, SSLMode);
                List<object[]> list_drugstotreDistributions = new List<object[]>();

                list_drugstotreDistributions = sQLControl_drugstotreDistribution.GetRowsByDefult(null, (int)enum_drugStotreDistribution.GUID, GUID);

                list_drugstotreDistributions.Sort((x, y) => y[(int)enum_drugStotreDistribution.加入時間].StringToDateTime().CompareTo(x[(int)enum_drugStotreDistribution.加入時間].StringToDateTime()));

                List<drugStotreDistributionClass> drugstotreDistributions = list_drugstotreDistributions.SQLToClass<drugStotreDistributionClass, enum_drugStotreDistribution>();
                if(drugstotreDistributions.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }

                returnData.Result = $"取得撥補資料(GUID),共<{list_drugstotreDistributions.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = drugstotreDistributions[0];
                returnData.Code = 200;
                Logger.LogAddLine($"drugstotreDistribution");
                Logger.Log($"drugstotreDistribution", $"{ returnData.JsonSerializationt(true)}");
                Logger.LogAddLine($"drugstotreDistribution");
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Data = null;
                returnData.Result = $"{e.Message}";
                Logger.Log($"drugstotreDistribution", $"[異常] { returnData.Result}");
                return returnData.JsonSerializationt(true);
            }
        }
        [Route("excel_upload")]
        [HttpPost]
        public async Task<string> POST_excel_upload([FromForm] IFormFile file, [FromForm] string IC_NAME, [FromForm] string CT, [FromForm] string DEFAULT_OP)
        {
            returnData returnData = new returnData();
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

                List<medClass> medClasses = medClass.get_med_cloud("http://127.0.0.1:4433");
                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "ServerSetting VM端設定異常!";
                    return returnData.JsonSerializationt(true);
                }

                returnData.Method = "POST_excel_upload";
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
                List<distribution_excel> distributionList = new List<distribution_excel>();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);
                    dt = dt.ReorderTable(new enum_撥補單上傳_Excel());
                    if (dt == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = "上傳文件表頭無效!";
                        return returnData.JsonSerializationt(true);
                    }
                    List<object[]> list_value = dt.DataTableToRowList();

                    if (IC_NAME.StringIsEmpty())
                    {
                        IC_NAME = Path.GetFileNameWithoutExtension(file.FileName);
                    }
                    for (int i = 0; i < list_value.Count; i++)
                    {

                        distribution_excel rowvalue = new distribution_excel
                        {
                            藥碼 = list_value[i][(int)enum_撥補單上傳_Excel.藥碼].ObjectToString(),
                            藥名 = list_value[i][(int)enum_撥補單上傳_Excel.藥名].ObjectToString(),
                            撥發量 = list_value[i][(int)enum_撥補單上傳_Excel.撥發量].ObjectToString(),
                            效期 = list_value[i][(int)enum_撥補單上傳_Excel.效期].ObjectToString(),
                            批號 = list_value[i][(int)enum_撥補單上傳_Excel.批號].ObjectToString(),
                        };
                        distributionList.Add(rowvalue);


                    }
                }
                returnData.Data = distributionList;
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = "接收上傳文件成功";
                return returnData.JsonSerializationt(true);
            }

            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }

        /// <summary>
        /// 以請領時間範圍下載請領單
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
        ///        "起始時間",
        ///        "結束時間"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>Excel</returns>
        [Route("download_excel_by_addedTime")]
        [HttpPost]
        public async Task<ActionResult> Post_download_excel_by_requestTime([FromBody] returnData returnData)
        {
            string VM_API = Method.GetServerAPI("Main", "網頁", "download_drugStotreDistribution_excel");
            if (VM_API.StringIsEmpty() == false)
            {
                string json_in = returnData.JsonSerializationt();

                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(json_in, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(VM_API, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var fileBytes = await response.Content.ReadAsByteArrayAsync();
                        var contentType = response.Content.Headers.ContentType?.MediaType ??
                                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                        var fileName = $"{DateTime.Now.ToDateString("-")}_撥補明細.xlsx";

                        return File(fileBytes, contentType, fileName);
                    }
                    else
                    {
                        return Content($"下載失敗：{response.StatusCode}");
                    }
                }
            }
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "download_excel_by_addedTime";
            try
            {
                string json_result = get_by_addedTime(returnData);

                if (json_result.StringIsEmpty()) return null;

                returnData returnData_result = json_result.JsonDeserializet<returnData>();

                List<drugStotreDistributionClass> drugStotreDistributionClasses = returnData_result.Data.ObjToClass<List<drugStotreDistributionClass>>();
                if (drugStotreDistributionClasses == null) return null;

                List<object[]> list_drugStotreDistributionClasses = drugStotreDistributionClasses.ClassToSQL<drugStotreDistributionClass, enum_drugStotreDistribution>();
                System.Data.DataTable dataTable = list_drugStotreDistributionClasses.ToDataTable(new enum_drugStotreDistribution());
                dataTable = dataTable.ReorderTable(new enum_materialRequisition_Excel_Export());
                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";

                byte[] excelData = dataTable.NPOI_GetBytes(Excel_Type.xlsx);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_撥補明細.xlsx"));
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

            }

        }

        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_drugStotreDistribution()));
            return tables.JsonSerializationt(true);
        }
    }
}
