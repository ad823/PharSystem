using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_儲位資訊
{
    [Route("api/[controller]")]
    [ApiController]
    public class stockList : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
           serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
           {
               var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

               if (string.IsNullOrWhiteSpace(Password))
                   throw new SecurityException("Database password cannot be null or empty (medUnit).");

               return (Server, DB, UserName, Password, Port);
           });
        private static readonly Lazy<Task<sys_serverSettingClass>>
           serverSettingTask = new Lazy<Task<sys_serverSettingClass>>(async () =>
           {
               var sys_serverSetting = await Method.GetServerAsync("Main", "網頁", "VM端");
               return sys_serverSetting;

           });

        private static string tableName = "stockList";
        [HttpPost("init")]
        public async Task<string> init()
        {
            returnData returnData = new returnData();
            try
            {
                return await CheckCreatTable();
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"{ex.Message}";
                return returnData.JsonSerializationt();
            }
        }

        private async Task<string> CheckCreatTable()
        {
            sys_serverSettingClass sys_serverSetting = await serverSettingTask.Value;    
            if (sys_serverSetting == null)
            {
                returnData returnData = new returnData();
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return returnData.JsonSerializationt();
            }

            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable<stockListClass>(sys_serverSetting));
            return tables.JsonSerializationt(true);
        }
    }
}
