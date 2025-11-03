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
        //private string APIServer = Method.GetServerAPI("Main", "網頁", "API01");
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
           serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
           {
               var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

               if (string.IsNullOrWhiteSpace(Password))
                   throw new SecurityException("Database password cannot be null or empty (medUnit).");

               return (Server, DB, UserName, Password, Port);
           });
        private static readonly Lazy<Task<sys_serverSettingClass>>
           GetServerAsync = new Lazy<Task<sys_serverSettingClass>>(async () =>
           {
               sys_serverSettingClass sys_ServerSetting = await Method.GetServerAsync("Main", "網頁", "VM端");

               if (sys_ServerSetting == null)
                   throw new SecurityException("Database password cannot be null or empty (medUnit).");

               return sys_ServerSetting;
           });
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
                List<medClassifyClass> medClassifyClasses = returnData.Data.ObjToClass<List<medClassifyClass>>();
                if (medClassifyClasses == null)
                {
                    medClassifyClass medClassify = returnData.Data.ObjToClass<medClassifyClass>();
                    if (medClassify == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"資料格式錯誤";
                        return returnData.JsonSerializationt();
                    }
                    medClassifyClasses = new List<medClassifyClass>{ medClassify };
                }
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                List< medClassifyClass > add = new List<medClassifyClass>();
                foreach (var item in medClassifyClasses)
                {
                    if (item.安全量天數.StringToDouble() == 0) continue;
                    if (item.基準量天數.StringToDouble() == 0) continue;
                    if (item.分類名稱.StringIsEmpty()) continue;
                    
                    item.GUID = Guid.NewGuid().ToString();
                    add.Add(item);
                }
                SQLControl sQLControl = new SQLControl(Server, DB, "medClassify", UserName, Password, Port, SSLMode);

                if (add.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無有效資料可寫入!";
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> add_ = add.ClassToSQL<medClassifyClass>();

                await sQLControl.AddRowsAsync(null, add_);

                returnData.Code = 200;
                returnData.Data = add;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add";
                returnData.Result = $"分類建立成功，共{add.Count}筆";
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
                List<medClassifyClass> medClassifyClasses = returnData.Data.ObjToClass<List<medClassifyClass>>();
                if (medClassifyClasses == null)
                {
                    medClassifyClass medClassify = returnData.Data.ObjToClass<medClassifyClass>();
                    if (medClassify == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"資料格式錯誤";
                        return returnData.JsonSerializationt();
                    }
                    medClassifyClasses = new List<medClassifyClass> { medClassify };
                }
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, "medClassify", UserName, Password, Port, SSLMode);

                string[] GUID = medClassifyClasses.Select(x => x.GUID).Distinct().ToArray();
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medClassify.GUID, GUID);
                List<medClassifyClass> medClassifies = objects.SQLToClass<medClassifyClass>();
                if (medClassifies.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無相關資料!";
                    return returnData.JsonSerializationt(true);
                }
               
                foreach (var item in medClassifies)
                {
                    medClassifyClass buff = medClassifyClasses.FirstOrDefault(x => x.GUID == item.GUID);
                    if (buff == null) continue;

                    if (buff.分類名稱.StringIsEmpty() == false) item.分類名稱 = buff.分類名稱;
                    if (buff.安全量天數.StringToDouble() != 0) item.安全量天數 = buff.安全量天數;
                    if (buff.基準量天數.StringToDouble() != 0) item.基準量天數 = buff.基準量天數;
                }

                
                List<object[]> update_ = medClassifies.ClassToSQL<medClassifyClass>();

                await sQLControl.UpdateRowsAsync(null, update_);

                returnData.Code = 200;
                returnData.Data = medClassifies;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update";
                returnData.Result = $"分類更新成功，共{medClassifies.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_by_GUID")]
        public async Task<string> get_by_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                
                if (returnData.ValueAry == null || returnData.ValueAry.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry不得為空";
                    return returnData.JsonSerializationt();
                }
                string[] GUID = returnData.ValueAry[0].Split(";").ToArray();
                
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, "medClassify", UserName, Password, Port, SSLMode);

                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medClassify.GUID, GUID);
                List<medClassifyClass> medClassifies = objects.SQLToClass<medClassifyClass>();
                if (medClassifies.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無相關資料!";
                    return returnData.JsonSerializationt(true);
                }
               
                returnData.Code = 200;
                returnData.Data = medClassifies;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_by_GUID";
                returnData.Result = $"取得資料成功，共{medClassifies.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Index was outside the bounds of the array.") init();
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_all")]
        public async Task<string> get_all([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {

               
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl = new SQLControl(Server, DB, "medClassify", UserName, Password, Port, SSLMode);

                List<object[]> objects = await sQLControl.GetAllRowsAsync(null);
                List<medClassifyClass> medClassifies = objects.SQLToClass<medClassifyClass>();
                

                returnData.Code = 200;
                returnData.Data = medClassifies;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_all";
                returnData.Result = $"取得資料成功，共{medClassifies.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Index was outside the bounds of the array.") init();
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_by_GUID(string GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(GUID);
            string result = await get_by_GUID(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }

        private async Task<string> CheckCreatTable()
        {
            sys_serverSettingClass sys_serverSettingClass = await GetServerAsync.Value;
            if (sys_serverSettingClass == null)
            {
                returnData returnData = new returnData();
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return returnData.JsonSerializationt();
            }

            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable<medClassifyClass>(sys_serverSettingClass));
            return tables.JsonSerializationt(true);
        }
    }
}
