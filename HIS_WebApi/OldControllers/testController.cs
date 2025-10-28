using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Configuration;
using IBM.Data.DB2.Core;
using MyOffice;
using NPOI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HIS_DB_Lib;
using H_Pannel_lib;
using System.Drawing;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            // 取得當前組件
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 取得 AssemblyInformationalVersion 屬性
            var version = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            var localIpAddress = HttpContext.Connection.LocalIpAddress?.ToString();
            var localPort = HttpContext.Connection.LocalPort;
            var protocol = HttpContext.Request.IsHttps ? "https" : "http";
            returnData returnData = new returnData();
            returnData.Code = 200;
            returnData.Result = $"Api test sucess 20250126!{protocol}://{localIpAddress}:{localPort}";

            string DB = ConfigurationManager.AppSettings["DB"];
            string Server = ConfigurationManager.AppSettings["Server"];
            string VM_Server = ConfigurationManager.AppSettings["VM_Server"];
            string VM_DB = ConfigurationManager.AppSettings["VM_DB"];

            List<string> strs = new List<string>();
            strs.Add($"local Server : {Server}");
            strs.Add($"local Database : {DB}");
            strs.Add($"VM Server : {VM_Server}");
            strs.Add($"VM Database : {VM_DB}");
            //strs.Add($"uDP_Class PORT: {Startup.uDP_Class.Port}");
            strs.Add($"Version : {version}");
            strs.Add($"RunningInDocker : " + $"{(ContainerChecker.IsRunningInDocker() ? "Y" : "N")}");
      
            returnData.Data = strs;

            string json = returnData.JsonSerializationt(true);

            //Logger.Log("test", json);
            return json;
        }
 

        [Route("check")]
        [HttpGet]
        public string get_check()
        {

            return "OK";
        }

   
    }

 
}
