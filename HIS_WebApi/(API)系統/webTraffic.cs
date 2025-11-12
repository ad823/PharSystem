using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_系統
{
    [Route("api/[controller]")]
    [ApiController]
    public class webTraffic : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        static string API_Server = "http://127.0.0.1:4433";

        [HttpPost("init")]
        public string init()
        {
            try
            {
                return CheckCreatTable(null, new enum_webTraffic());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 資料寫入
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "Data":
        ///             {
        ///                 "page":"盤點管理",
        ///                 "parameter":"INV20250701-0",
        ///                 "user_id":"7777",
        ///                 "user_name":"ANNA",
        ///             }
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add")]
        public string add([FromBody] returnData returnData)
        {
            returnData.Method = "add";
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                webTrafficClass input_webTrafficClass = returnData.Data.ObjToClass<webTrafficClass>();
                if (input_webTrafficClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
               
                string 裝置IP = HttpContext.GetClientIP();
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "webTraffic", UserName, Password, Port, SSLMode);
                List<object[]> list_webTrafficClass = sQLControl.GetRowsByDefult(null,(int)enum_webTraffic.裝置IP, 裝置IP);
                List<webTrafficClass> webTrafficClasses = list_webTrafficClass.SQLToClass<webTrafficClass, enum_webTraffic>();
                
                if(webTrafficClasses.Count == 0)
                {
                    input_webTrafficClass.GUID = Guid.NewGuid().ToString();
                    input_webTrafficClass.裝置IP = 裝置IP;
                    input_webTrafficClass.加入時間 = DateTime.Now.ToDateTimeString();

                    object[] add = input_webTrafficClass.ClassToSQL<webTrafficClass, enum_webTraffic>();
                    sQLControl.AddRow(null, add);
                }
                else
                {

                    input_webTrafficClass.GUID = webTrafficClasses[0].GUID;
                    input_webTrafficClass.加入時間 = DateTime.Now.ToDateTimeString();
                    input_webTrafficClass.裝置IP = 裝置IP;
                    object[] update = input_webTrafficClass.ClassToSQL<webTrafficClass, enum_webTraffic>();
                    sQLControl.UpdateByDefulteExtra(null, update);
                }

                

                returnData.Code = 200;
                returnData.Data = input_webTrafficClass;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"資料寫入成功";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }


        }
        /// <summary>
        /// 以頁面、參數、時間區間取得資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///                 "盤點管理",
        ///                 "INV20250701-0",
        ///                 "10"
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_page")]
        public string get_by_page([FromBody] returnData returnData)
        {
            returnData.Method = "get_by_page";
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if(returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入returnData.ValueAry資料為空值";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入returnData.ValueAry資料應該為 [\"頁面\",\"秒數\"]";
                    return returnData.JsonSerializationt();
                }
                string 頁面 = returnData.ValueAry[0];
                string 秒數 = returnData.ValueAry[1];
                double 時間區間 = 秒數.StringToDouble() * -1;
                string now = DateTime.Now.AddSeconds(時間區間).ToDateTimeString();


                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "webTraffic", UserName, Password, Port, SSLMode);
                string command = $"SELECT * FROM {DB}.webTraffic WHERE  頁面 = '{頁面}' AND 加入時間 > '{now}';";
                DataTable dataTable_webTraffic = sQLControl.WtrteCommandAndExecuteReader(command);
                List<object[]> list_webTraffic = dataTable_webTraffic.DataTableToRowList();

                List<webTrafficClass> webTrafficClasses = list_webTraffic.SQLToClass<webTrafficClass, enum_webTraffic>();
                webTrafficClasses = webTrafficClasses
                    .GroupBy(item => item.參數)
                    .Select(group =>
                    {
                        int qty = group.Count();
                        webTrafficClass webTrafficClass = group.First();
                        webTrafficClass.統計 = qty;
                        webTrafficClass.使用者ID = string.Join(",", group.Select(x => x.使用者ID));
                        webTrafficClass.使用者姓名 = string.Join(",", group.Select(x => x.使用者姓名));
                        return webTrafficClass;
                    }).ToList();


                returnData.Code = 200;
                returnData.Data = webTrafficClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"{秒數}秒內有{webTrafficClasses.Count}資料";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }


        }
        private string CheckCreatTable(string tableName, System.Enum Enum)
        {

            if (tableName == null)
            {
                tableName = Enum.GetEnumDescription();
            }
            SQLUI.Table table = new SQLUI.Table(tableName);
            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
            if (sys_serverSettingClasses.Count == 0)
            {
                return $"找無Server資料!";
            }
            table = MethodClass.CheckCreatTable(sys_serverSettingClasses[0], Enum);

            return table.JsonSerializationt(true);
        }
    }
    public static class HttpContextExtensions
    {
        public static string GetClientIP(this HttpContext context)
        {
            string ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrEmpty(ip))
            {
                // X-Forwarded-For 可能是 "真實IP, proxy1, proxy2"，只取第一個
                ip = ip.Split(',').FirstOrDefault()?.Trim();
            }

            // 如果沒有 X-Forwarded-For，就用 RemoteIpAddress
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress?.ToString();
            }

            return ip ?? "未知";
        }
    }
}
