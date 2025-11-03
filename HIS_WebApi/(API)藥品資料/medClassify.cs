using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using MyUI;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using SQLUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text.Json.Serialization;
using System.Threading;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class medClassify : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private string APIServer = Method.GetServerAPI("Main", "網頁", "API01");
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
           serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
           {
               var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

               if (string.IsNullOrWhiteSpace(Password))
                   throw new SecurityException("Database password cannot be null or empty (medUnit).");

               return (Server, DB, UserName, Password, Port);
           });
       
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
            tables.Add(MethodClass.CheckCreatTable<medUnitClass>(sys_serverSettingClasses[0]));
            return tables.JsonSerializationt(true);
        }
    }
}
