using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_住院調劑系統
{
    [Route("api/[controller]")]
    [ApiController]
    public class med_carlog : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        private static readonly Lazy<Task<sys_serverSettingClass>>
           GetServerAsync = new Lazy<Task<sys_serverSettingClass>>(async () =>
           {
               sys_serverSettingClass sys_ServerSetting = await Method.GetServerAsync("Main", "網頁", "VM端");

               if (sys_ServerSetting == null)
                   throw new SecurityException("Database password cannot be null or empty (medUnit).");

               return sys_ServerSetting;
           });
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
                List<med_carlogClass> med_CarlogClasses = returnData.Data.ObjToClass<List<med_carlogClass>>();
                if (med_CarlogClasses == null)
                {
                    med_carlogClass med_CarlogClass = returnData.Data.ObjToClass<med_carlogClass>();
                    if (med_CarlogClass == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"資料格式錯誤";
                        return returnData.JsonSerializationt();
                    }
                    med_CarlogClasses = new List<med_carlogClass> { med_CarlogClass };
                }

                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                List<med_carlogClass> add = new List<med_carlogClass>();
                string time_now = DateTime.Now.ToDateTimeString();
                string time_min = DateTime.MinValue.ToDateTimeString();
                foreach (var item in med_CarlogClasses)
                {
                    item.GUID = Guid.NewGuid().ToString();
                    item.傳送時間 = time_now;
                    item.護理站簽收時間 = time_min;
                    add.Add(item);
                }
                SQLControl sQLControl = new SQLControl(Server, DB, "med_carlog", UserName, Password, Port, SSLMode);

                if (add.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無有效資料可寫入!";
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> add_ = add.ClassToSQL<med_carlogClass>();

                await sQLControl.AddRowsAsync(null, add_);

                returnData.Code = 200;
                returnData.Data = add;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add";
                returnData.Result = $"建立成功，共{add.Count}筆";
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
                List<med_carlogClass> med_CarlogClasses = returnData.Data.ObjToClass<List<med_carlogClass>>();
                if (med_CarlogClasses == null)
                {
                    med_carlogClass med_CarlogClass = returnData.Data.ObjToClass<med_carlogClass>();
                    if (med_CarlogClass == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"資料格式錯誤";
                        return returnData.JsonSerializationt();
                    }
                    med_CarlogClasses = new List<med_carlogClass> { med_CarlogClass };
                }

                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
               
                
                SQLControl sQLControl = new SQLControl(Server, DB, "med_carlog", UserName, Password, Port, SSLMode);

                
                List<object[]> update = med_CarlogClasses.ClassToSQL<med_carlogClass>();

                if (update.Count > 0) await sQLControl.UpdateRowsAsync(null, update);

                returnData.Code = 200;
                returnData.Data = med_CarlogClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update";
                returnData.Result = $"更新成功，共{update.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_by_time")]
        public async Task<string> get_by_time([FromBody] returnData returnData)
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
                List<med_carlogClass> med_CarlogClasses = returnData.Data.ObjToClass<List<med_carlogClass>>();
                if (med_CarlogClasses == null)
                {
                    med_carlogClass med_CarlogClass = returnData.Data.ObjToClass<med_carlogClass>();
                    if (med_CarlogClass == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"資料格式錯誤";
                        return returnData.JsonSerializationt();
                    }
                    med_CarlogClasses = new List<med_carlogClass> { med_CarlogClass };
                }

                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                List<med_carlogClass> add = new List<med_carlogClass>();
                string time_now = DateTime.Now.ToDateTimeString();
                string time_min = DateTime.MinValue.ToDateTimeString();
                foreach (var item in med_CarlogClasses)
                {
                    item.GUID = Guid.NewGuid().ToString();
                    item.傳送時間 = time_now;
                    item.護理站簽收時間 = time_min;
                    add.Add(item);
                }
                SQLControl sQLControl = new SQLControl(Server, DB, "med_carlog", UserName, Password, Port, SSLMode);

                if (add.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無有效資料可寫入!";
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> add_ = add.ClassToSQL<med_carlogClass>();

                await sQLControl.AddRowsAsync(null, add_);

                returnData.Code = 200;
                returnData.Data = add;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add";
                returnData.Result = $"建立成功，共{add.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        private async Task<string> CheckCreatTable(returnData returnData)
        {
            sys_serverSettingClass sys_ServerSettingClass = await GetServerAsync.Value;
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable<med_carlogClass>(sys_ServerSettingClass));
            return tables.JsonSerializationt(true);
        }
    }
}
