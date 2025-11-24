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
using MyOffice;
using MyUI;
using System.IO;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class chemotherapyOrder : Controller
    {
        static private string API_Server = "http://127.0.0.1:4433";
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
        [HttpPost("init")]
        public string init([FromBody] returnData returnData)
        {
            var timer = new MyTimerBasic();
            returnData.Method = "init";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting("220.135.128.247");
                sys_serverSettingClass settingClass = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端").FirstOrDefault();
                if (settingClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                List<Table> tables = new List<Table>();
                tables.Add(MethodClass.CheckCreatTable<chemotherapyOrderClass>(settingClass));
                tables.Add(MethodClass.CheckCreatTable<chemotherapyOrderDayClass>(settingClass));

                returnData.Code = 200;
                returnData.Data = tables;
                returnData.Result = "初始化 chemotherapyOrder 資料表完成";
                returnData.TimeTaken = $"{timer}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception: {ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
    }
}
