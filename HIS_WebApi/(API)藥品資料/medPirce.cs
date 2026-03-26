using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using MyUI;
using NPOI;
using NPOI.SS.Formula.Functions;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class medPirce : Controller
    {
        static public string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
          serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
          {
              var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

              if (string.IsNullOrWhiteSpace(Password))
                  throw new SecurityException("Database password cannot be null or empty (medUnit).");

              return (Server, DB, UserName, Password, Port);
          });
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
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(medConfigClass))]
        [Route("init")]
        [HttpPost]
        public string GET_init([FromBody] returnData returnData)
        {
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
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
                string msg = "";
                return msg;
            }
        }
        /// <summary>
        /// 新增及修改藥價
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {      
        ///      [
        ///        (medPrice)
        ///      ]
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data] : medPriceClass</returns>
        [Route("update")]
        [HttpPost]
        public string POST_update([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
        
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

                GET_init(returnData);
                returnData.Method = "update";
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                Table table = new Table(new enum_medPrice());
                SQLControl sQLControl_medPrice = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_medPreice = sQLControl_medPrice.GetAllRows(null);
                List<medPriceClass> medPirce_sql = list_medPreice.SQLToClass<medPriceClass, enum_medPrice>();
                List<medPriceClass> medPirce_sql_buf = new List<medPriceClass>();
                List<medPriceClass> medPirce_sql_add = new List<medPriceClass>();
                List<medPriceClass> medPirce_sql_replace = new List<medPriceClass>();

                List<medPriceClass> medPirce_input = returnData.Data.ObjToClass<List<medPriceClass>>();

                List<object[]> list_medPreice_add = new List<object[]>();
                List<object[]> list_medPreice_replace = new List<object[]>();

                if (medPirce_input == null)
                {
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Code = -200;
                    returnData.Result = $"[returnData.Data] 值異常";
                }
                for (int i = 0; i < medPirce_input.Count; i++)
                {
                    string Code = medPirce_input[i].藥品碼;
                    medPirce_sql_buf = (from temp in medPirce_sql
                                        where temp.藥品碼 == Code
                                          select temp).ToList();
                    if(medPirce_sql_buf.Count == 0)
                    {
                        string GUID = Guid.NewGuid().ToString();
                        medPriceClass medPriceClass = medPirce_input[i];
                        medPriceClass.GUID = GUID;
                        medPriceClass.加入時間 = DateTime.Now.ToDateTimeString();
                        medPirce_sql_add.Add(medPriceClass);
                    }
                    else
                    {
                        string GUID = medPirce_sql_buf[0].GUID;
                        medPriceClass medPriceClass = medPirce_input[i];
                        medPriceClass.GUID = GUID;
                        medPriceClass.加入時間 = DateTime.Now.ToDateTimeString();
                        medPirce_sql_replace.Add(medPriceClass);
                    }
                }

                list_medPreice_add = medPirce_sql_add.ClassToSQL<medPriceClass, enum_medPrice>();
                list_medPreice_replace = medPirce_sql_replace.ClassToSQL<medPriceClass, enum_medPrice>();
                if (list_medPreice_add.Count > 0) sQLControl_medPrice.AddRows(null, list_medPreice_add);
                if (list_medPreice_replace.Count > 0) sQLControl_medPrice.UpdateByDefulteExtra(null, list_medPreice_replace);
                returnData.TimeTaken = myTimer.ToString();
                returnData.Data = "";
                returnData.Code = 200;
                returnData.Result = $"修改藥價,新增<{list_medPreice_add.Count}>筆,修改<{list_medPreice_replace.Count}>筆";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception e)
            {
                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = -200;
                returnData.Result = $"Exception : {e.Message}";
                return returnData.JsonSerializationt(true);
            }

  


        }
        /// <summary>
        /// 取得所有藥品價格資訊
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {      
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data] : medPriceClass</returns>
        [Route("get_all")]
        [HttpPost]
        public string POST_get_all([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
    
            try
            {
                GET_init(returnData);
                returnData.Method = "get_all";
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                Table table = new Table(new enum_medPrice());
                SQLControl sQLControl_medPrice = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_medPreice = sQLControl_medPrice.GetAllRows(null);
                List<medPriceClass> medPirce_sql = list_medPreice.SQLToClass<medPriceClass, enum_medPrice>();
               
                returnData.TimeTaken = myTimer.ToString();
                returnData.Data = medPirce_sql;
                returnData.Code = 200;
                returnData.Result = $"取得所有藥品價格資訊,共<{medPirce_sql.Count}>筆";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception e)
            {
                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = -200;
                returnData.Result = $"Exception : {e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        [Route("get_by_codes")]
        [HttpPost]
        public async Task<string> get_by_codes([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            try
            {
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼1,藥碼2,藥碼3]";
                    return returnData.JsonSerializationt(true);
                }
                string[] Codes = returnData.ValueAry[0].Split(",");
                if (Codes.Length == 1)
                {
                    Codes = returnData.ValueAry[0].Split(";");
                }
                GET_init(returnData);
                returnData.Method = "get_by_codes";

                Table table = new Table(new enum_medPrice());
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl_medPrice = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);

                List<object[]> list_value_buf = await sQLControl_medPrice.GetRowsByDefultAsync(null, (int)enum_medPrice.藥品碼, Codes);           
                List<medPriceClass> medPirce_sql = list_value_buf.SQLToClass<medPriceClass, enum_medPrice>();

                returnData.TimeTaken = myTimer.ToString();
                returnData.Data = medPirce_sql;
                returnData.Code = 200;
                returnData.Result = $"取得藥品價格資訊,共<{medPirce_sql.Count}>筆";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception e)
            {
                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = -200;
                returnData.Result = $"Exception : {e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }

        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {

            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_medPrice()));
            return tables.JsonSerializationt(true);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_by_codes(List<string> strings)
        {
            returnData returnData = new returnData();
            returnData.ValueAry = string.Join(";",strings);
            string result = await get_by_codes(returnData);
            return result.JsonDeserializet<returnData>();
        }
    }
}
