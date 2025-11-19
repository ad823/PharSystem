using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_藥品資料
{
    [Route("api/[controller]")]
    [ApiController]
    public class medStorage : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private string tableName = "medStorage";

        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(200, "medStorageClass物件", typeof(medStorageClass))]

        /// <summary>
        ///初始化資料庫
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":[""]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("init")]
        public async Task<string> init([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "init";
            try
            {
                return await CheckCreatTable();
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return await returnData.JsonSerializationtAsync(true);
            }
        }
        /// <summary>
        /// 新增資料(不更新舊的)
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     [
        ///         {
        ///         "code":
        ///         "storage"
        ///         }
        ///     ]
        ///     "Value": "",
        ///     "ValueAry":[]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<string> add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                await init(returnData);
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data資料錯誤!";
                    return returnData.JsonSerializationt();
                }
                List<medStorageClass> medStorageClasses = returnData.Data.ObjToClass<List<medStorageClass>>();
                if (medStorageClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data資料錯誤，須為 List<nearMissClass>!";
                    return returnData.JsonSerializationt();
                }
                string tableName = "medStorage";
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                string[] codes = medStorageClasses.Select(x => x.藥碼).Distinct().ToArray();              
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medStroage.藥碼, codes);
                List<medStorageClass> medStorages = objects.SQLToClass<medStorageClass, enum_medStroage>();
                List<medStorageClass> add_medStorageClass = new List<medStorageClass>();
                foreach (var item in medStorageClasses)
                {
                    if (item.藥碼.StringIsEmpty() || item.儲位描述.StringIsEmpty()) continue;
                    medStorageClass medStorageClass = medStorages.Where(x => x.藥碼 == item.藥碼 && x.儲位描述 == item.儲位描述).FirstOrDefault();
                    if (medStorageClass != null) continue;
                    item.GUID = Guid.NewGuid().ToString();
                    add_medStorageClass.Add(item);
                }
                List<object[]> add = add_medStorageClass.ClassToSQL<medStorageClass, enum_medStroage>();
                if (add.Count > 0) await sQLControl.AddRowsAsync(null, add);
                

                returnData.Code = 200;
                returnData.Data = add_medStorageClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add";
                returnData.Result = $"資料寫入成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                //if (ex.Message == "Index was outside the bounds of the array.") init();
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 新增資料(舊的會刪掉)
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     [
        ///         {
        ///         "code":
        ///         "storage"
        ///         }
        ///     ]
        ///     "Value": "",
        ///     "ValueAry":[]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add_autoUpdate")]
        public async Task<string> add_autoUpdate([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                await init(returnData);
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data資料錯誤!";
                    return returnData.JsonSerializationt();
                }
                List<medStorageClass> medStorageClasses = returnData.Data.ObjToClass<List<medStorageClass>>();
                if (medStorageClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data資料錯誤，須為 List<nearMissClass>!";
                    return returnData.JsonSerializationt();
                }
                string tableName = "medStorage";
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                string[] codes = medStorageClasses.Select(x => x.藥碼).Distinct().ToArray();
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medStroage.藥碼, codes);
                List<medStorageClass> medStorages = objects.SQLToClass<medStorageClass, enum_medStroage>();
                Dictionary<string, List<medStorageClass>> medStorageDict = medStorageClass.ToDictByCode(medStorageClasses);
                List<medStorageClass> add_medStorageClass = new List<medStorageClass>();
                List<medStorageClass> delete_medStorageClass = new List<medStorageClass>();

                if (medStorages == null || medStorages.Count == 0)
                {
                    foreach (string code in medStorageDict.Keys)
                    {
                        List<medStorageClass> storageClasses = medStorageClass.GetDictByCode(medStorageDict, code);
                        foreach (var item in storageClasses)
                        {
                            if (item.藥碼.StringIsEmpty() || item.儲位描述.StringIsEmpty()) continue;
                            item.GUID = Guid.NewGuid().ToString();
                            add_medStorageClass.Add(item);
                        }

                    }
                }
                else
                {
                    Dictionary<string, List<medStorageClass>> medStorageDict_sql = medStorageClass.ToDictByCode(medStorages);
                    foreach (string code in medStorageDict.Keys)
                    {
                        List<medStorageClass> old_Data = medStorageClass.GetDictByCode(medStorageDict_sql, code);
                        delete_medStorageClass.AddRange(old_Data);
                        List<medStorageClass> storageClasses = medStorageClass.GetDictByCode(medStorageDict, code);
                        foreach (var item in storageClasses)
                        {
                            if (item.藥碼.StringIsEmpty() || item.儲位描述.StringIsEmpty()) continue;
                            item.GUID = Guid.NewGuid().ToString();
                            add_medStorageClass.Add(item);
                        }

                    }
                }
                

               
                List<object[]> add = add_medStorageClass.ClassToSQL<medStorageClass, enum_medStroage>();
                List<object[]> delete = delete_medStorageClass.ClassToSQL<medStorageClass, enum_medStroage>();

                if (add.Count > 0) await sQLControl.AddRowsAsync(null, add);
                if (delete.Count > 0) await sQLControl.DeleteRowsAsync(null, delete);


                returnData.Code = 200;
                returnData.Data = add_medStorageClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_autoUpdate";
                returnData.Result = $"資料寫入成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                //if (ex.Message == "Index was outside the bounds of the array.") init();
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 依照藥碼取得資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     [
        ///         {
        ///         }
        ///     ]
        ///     "Value": "",
        ///     "ValueAry":["code1;code2"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_code")]
        public async Task<string> get_by_code([FromBody] returnData returnData, CancellationToken ct)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤!";
                    return await returnData.JsonSerializationtAsync(true);
                }
                if (returnData.ValueAry.Count != 1 || returnData.ValueAry[0].StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.ValueAry資料錯誤，['藥碼']!";
                    return await returnData.JsonSerializationtAsync(true);
                }
                string 藥碼 = returnData.ValueAry[0];
                string[] codes = 藥碼.Split(';');
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medStroage.藥碼, codes);
                List<medStorageClass> medStorages = objects.SQLToClass<medStorageClass, enum_medStroage>();

                
                returnData.Code = 200;
                returnData.Data = medStorages;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_by_code";
                returnData.Result = $"取得藥碼({codes.JsonSerializationt()})的儲位資料，共{medStorages.Count}筆!";
                return await returnData.JsonSerializationtAsync(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return await returnData.JsonSerializationtAsync(true);
            }
        }
        /// <summary>
        /// 取得儲位資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     [
        ///         {
        ///         }
        ///     ]
        ///     "Value": "",
        ///     "ValueAry":[""]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_all")]
        public async Task<string> get_all([FromBody] returnData returnData, CancellationToken ct = default)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {            
                await init(returnData);
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);

                List<object[]> objects = await sQLControl.GetAllRowsAsync(null);
                List<medStorageClass> medStorages = objects.SQLToClass<medStorageClass, enum_medStroage>();

                returnData.Code = 200;
                returnData.Data = medStorages;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_all";
                returnData.Result = $"取得儲位資料，共{medStorages.Count}筆!";
                return await returnData.JsonSerializationtAsync(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return await returnData.JsonSerializationtAsync(true);
            }
        }
        private async Task<string> CheckCreatTable()
        {
            List<sys_serverSettingClass> sys_serverSettingClasses = await ServerSettingController.GetServerSettingasync("Main", "網頁", "VM端");
            if (sys_serverSettingClasses.Count == 0)
            {
                returnData returnData = new returnData();
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return returnData.JsonSerializationt();
            }

            List<Table> tables = new List<Table>();

            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medStroage()));

            return tables.JsonSerializationt(true);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_all()
        {
            returnData returnData = new returnData();
            string result = await get_all(returnData);
            return result.JsonDeserializet<returnData>();
        }
    }
}
