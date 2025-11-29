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
    /// <summary>
    /// 化療藥局癌症備藥機-出料馬達輸出索引表
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CPMP_StorageConfig : ControllerBase
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        /// <summary>
        /// 初始化出料馬達輸出索引表
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "cheom",
        ///     "ServerType" : "癌症備藥機",
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
        [HttpPost]
        public string init(returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_ServerSetting = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "一般資料").FirstOrDefault();
                if (sys_ServerSetting == null) 
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                return CheckCreatTable(sys_ServerSetting);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        /// <summary>
        /// 初始化出料馬達輸出索引表
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "cheom",
        ///     "ServerType" : "癌症備藥機",
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
        public string get_all(returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_ServerSetting = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "一般資料").FirstOrDefault();
                if (sys_ServerSetting == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string Server = sys_ServerSetting.Server;
                string DB = sys_ServerSetting.DBName;
                string UserName = sys_ServerSetting.User;
                string Password = sys_ServerSetting.Password;
                uint Port = (uint)sys_ServerSetting.Port.StringToInt32();
                Table table = new Table(new enum_CMPM_StorageConfig());
                SQLControl sQLControl = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);

                List<CMPM_StorageConfig_Class> cMPM_StorageConfig_Classes = sQLControl.GetAllRows(null).SQLToClass<CMPM_StorageConfig_Class , enum_CMPM_StorageConfig>();
                returnData.Data = cMPM_StorageConfig_Classes;
                string json_out = returnData.JsonSerializationt(true);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
        }

        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_CMPM_StorageConfig());
            return table.JsonSerializationt(true);       
        }
    }
}
