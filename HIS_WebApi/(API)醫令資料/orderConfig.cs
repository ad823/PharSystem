using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using HIS_WebApi._API_系統;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_醫令資料
{
    [Route("api/[controller]")]
    [ApiController]
    public class orderConfig : ControllerBase
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
        [HttpPost("init")]
        public async Task<string> init([FromBody] returnData returnData)
        {
            try
            {
                return await CheckCreatTable(returnData);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"{ex.Message}";
                return returnData.JsonSerializationt();
            }
        }
        [HttpPost("add")]
        public async Task<string> add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data不得為空";
                    return returnData.JsonSerializationt();
                }
                List<orderConfigClass> orderConfigClasses = returnData.Data.ObjToClass<List<orderConfigClass>>();
                if (orderConfigClasses == null)
                {
                    orderConfigClass orderConfig = returnData.Data.ObjToClass<orderConfigClass>();
                    if (orderConfig == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data資料錯誤，須為orderConfigClass";
                        return returnData.JsonSerializationt();
                    }
                    orderConfigClasses = new List<orderConfigClass>() { orderConfig };
                }


                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, "orderConfig", UserName, Password, Port, SSLMode);
                string now = DateTime.Now.ToDateTimeString();
                orderConfigClasses = orderConfigClasses.Where(x => x.Order_GUID.StringIsEmpty() == false).ToList();
                foreach (var item in orderConfigClasses)
                {
                    item.GUID = Guid.NewGuid().ToString(); 
                    item.更新時間 = now;
                }
                
                List<object[]> add = orderConfigClasses.ClassToSQL<orderConfigClass>();
                await sQLControl.AddRowsAsync(null, add);

                returnData.Code = 200;
                returnData.Data = orderConfigClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add";
                returnData.Result = $"寫入成功，共<{orderConfigClasses.Count}>筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("update")]
        public async Task<string> update([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data不得為空";
                    return returnData.JsonSerializationt();
                }
                List<orderConfigClass> orderConfigClasses = returnData.Data.ObjToClass<List<orderConfigClass>>();
                if (orderConfigClasses == null)
                {
                    orderConfigClass orderConfig = returnData.Data.ObjToClass<orderConfigClass>();
                    if (orderConfig == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data資料錯誤，須為orderConfigClass";
                        return returnData.JsonSerializationt();
                    }
                    orderConfigClasses = new List<orderConfigClass>() { orderConfig };
                }


                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, "orderConfig", UserName, Password, Port, SSLMode);
                List<object[]> objects_ = await sQLControl.GetRowsByDefultAsync(null, (int)enum_orderConfig.GUID, orderConfigClasses.Select(x => x.GUID).ToArray());
                List<orderConfigClass> db_orderConfig = objects_.SQLToClass<orderConfigClass>();
                string now = DateTime.Now.ToDateTimeString();

                foreach (var item in db_orderConfig)
                {
                    orderConfigClass orderConfig = orderConfigClasses.FirstOrDefault(x => x.GUID == item.GUID);
                    if (orderConfig == null) continue;
                    item.功能備註 = orderConfig.功能備註;
                    item.狀態 = orderConfig.狀態;
                    item.更新時間 = now;
                }

                List<object[]> update = db_orderConfig.ClassToSQL<orderConfigClass>();
                await sQLControl.UpdateRowsAsync(null, update);

                returnData.Code = 200;
                returnData.Data = db_orderConfig;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update";
                returnData.Result = $"更新成功，共<{db_orderConfig.Count}>筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_by_orderGUID")]
        public async Task<string> get_by_orderGUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry不得為空";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"GUID\"]";
                    return returnData.JsonSerializationt();
                }
                string[] orderGUID = returnData.ValueAry[0].Split(";");
                
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, "orderConfig", UserName, Password, Port, SSLMode);
                List<object[]> objects_ = await sQLControl.GetRowsByDefultAsync(null, (int)enum_orderConfig.Order_GUID, orderGUID);
                List<orderConfigClass> db_orderConfig = objects_.SQLToClass<orderConfigClass>();
               
                returnData.Code = 200;
                returnData.Data = db_orderConfig;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_by_orderGUID";
                returnData.Result = $"取得成功，共<{db_orderConfig.Count}>筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                if (ex.Message.Contains("Index was outside the bounds of the array.")) init(returnData);
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }

        private async Task<string> CheckCreatTable(returnData returnData)
        {
            sys_serverSettingClass sys_ServerSettingClass = await HIS_WebApi.Method.GetServerAsync("Main", "網頁", "VM端");
                                            
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable<orderConfigClass>(sys_ServerSettingClass));

            return tables.JsonSerializationt(true);
        }
    }
}
