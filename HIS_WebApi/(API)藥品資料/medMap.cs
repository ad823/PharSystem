using AutoMapper.Mappers;
using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MyUI;
using NPOI.SS.Formula.Functions;
using SQLUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_藥品資料
{
    [Route("api/[controller]")]
    [ApiController]
    public class medMap : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        static private string API_server = HIS_WebApi.Method.GetServerAPI("Main", "網頁", "API01");
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
           serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
           {
               var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

               if (string.IsNullOrWhiteSpace(Password))
                   throw new SecurityException("Database password cannot be null or empty (medUnit).");

               return (Server, DB, UserName, Password, Port);
           });
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(200, "藥品地圖物件", typeof(medMapClass))]


        /// <summary>
        ///初始化藥品地圖資料庫
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
        public string init([FromBody] returnData returnData)
        {
            try
            {
                return CheckCreatTable();
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"{ex.Message}";
                return returnData.JsonSerializationt();
            }
        }
        /// <summary>
        /// 新增父容器
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["設備名稱","類別","位置", "絕對位置"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add_medMap")]
        public async Task<string> add_medMap([FromBody] returnData returnData)
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
                if (returnData.ValueAry.Count != 4)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"設備名稱\",\"類別\",\"位置\",\"絕對位置\"]";
                    return returnData.JsonSerializationt();
                }
                string 設備名稱 = returnData.ValueAry[0];
                string 類別 = returnData.ValueAry[1];
                string 位置 = returnData.ValueAry[2];
                string 絕對位置 = returnData.ValueAry[3];

                if (位置.Split(",").Count() != 2 || 絕對位置.Split(",").Count() != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"位置資料錯誤，須為 \"0,1\"";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                List<sys_serverSettingClass> sys_ServerSettingClasses = await ServerSettingController.GetAllServerSettingasync(設備名稱, 類別, "一般資料");
                if (sys_ServerSettingClasses == null || sys_ServerSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                string Master_GUID = sys_ServerSettingClasses[0].GUID;

                SQLControl sQLControl_medMap = new SQLControl(Server, DB, "medMap", UserName, Password, Port, SSLMode);

                List<object[]> objects = await sQLControl_medMap.GetRowsByDefultAsync(null, (int)enum_medMap.Master_GUID, Master_GUID);
                if (objects.Count > 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料已存在!";
                    return returnData.JsonSerializationt();
                }
                medMapClass medMapClass = new medMapClass();
                medMapClass.GUID = Guid.NewGuid().ToString();
                medMapClass.Master_GUID = Master_GUID;
                medMapClass.位置 = 位置;
                medMapClass.絕對位置 = 位置;
                medMapClass.type = "container";

                object[] add = medMapClass.ClassToSQL<medMapClass, enum_medMap>();
                await sQLControl_medMap.AddRowAsync(null, add);
                medMapClass.sys_ServerSetting = sys_ServerSettingClasses[0];

                returnData.Code = 200;
                returnData.Data = medMapClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_group";
                returnData.Result = $"父容器資料寫入成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 修改父容器(位置)
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     [
        ///         {
        ///             "GUID":
        ///             "position":"0,1",
        ///             "absolute_position":"10,10"
        ///         }
        ///     ],
        ///     "Value": "",
        ///     "ValueAry":["GUID","Master_GUID","位置"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("update_medMap")]
        public async Task<string> update_medMap([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry不得為空";
                    return returnData.JsonSerializationt();
                }
                List<medMapClass> medMapClasses = returnData.Data.ObjToClass<List<medMapClass>>();
                string[] GUID = medMapClasses.Select(x => x.GUID).ToArray();
                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap = new SQLControl(Server, DB, "medMap", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_medMap.GetRowsByDefultAsync(null, (int)enum_medMap.GUID, GUID);
                if (objects.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                List<medMapClass> medMaps = objects.SQLToClass<medMapClass, enum_medMap>();
                foreach (var item in medMaps)
                {
                    medMapClass medMap_buff = medMapClasses.Where(x => x.GUID == item.GUID).FirstOrDefault();
                    if (medMap_buff == null) continue;
                    if (medMap_buff.位置.StringIsEmpty() == false && medMap_buff.位置.Split(",").Count() == 2) item.位置 = medMap_buff.位置;
                    if (medMap_buff.絕對位置.StringIsEmpty() == false && medMap_buff.絕對位置.Split(",").Count() == 2)
                        item.絕對位置 = medMap_buff.絕對位置;
                }
                List<object[]> update = medMaps.ClassToSQL<medMapClass, enum_medMap>();
                await sQLControl_medMap.UpdateRowsAsync(null, update);

                returnData.Code = 200;
                returnData.Data = medMaps;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_medMap";
                returnData.Result = $"父容器資料更新成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以設備名稱和類別取得父容器資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["設備名稱","類別"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medMap_by_department")]
        public async Task<string> get_medMap_by_department([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[department type]";
                    return returnData.JsonSerializationt(true);
                }
                ServerSettingController controller = new ServerSettingController();
                string result = await controller.POST_get_serversetting_by_department_type(returnData);
                returnData returnData_get_serversetting_by_type = result.JsonDeserializet<returnData>();

                List<sys_serverSettingClass> sys_serverSettingClasses = returnData_get_serversetting_by_type.Data.ObjToClass<List<sys_serverSettingClass>>();
                if (sys_serverSettingClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"POST_get_serversetting_by_type 回傳為空";
                    return returnData.JsonSerializationt(true);
                }
                List<Task<medMapClass>> tasks = new List<Task<medMapClass>>();
                List<medMapClass> medMapClasses = new List<medMapClass>();

                foreach (var item in sys_serverSettingClasses)
                {
                    returnData returnData_get_medMap_by_name_type = await get_medMap_by_name_type(item.設備名稱, item.類別);
                    medMapClass medMapClass = returnData_get_medMap_by_name_type.Data.ObjToClass<medMapClass>();
                    medMapClasses.Add(medMapClass);
                }


                returnData.Code = 200;
                returnData.Data = medMapClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_by_department";
                returnData.Result = $"取得{returnData.ValueAry[0]}單位藥品地圖資料，共{medMapClasses.Count}!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                if (ex.Message == "Index was outside the bounds of the array.") init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以設備名稱和類別取得父容器資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["設備名稱","類別"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medMap_by_name_type")]
        public async Task<string> get_medMap_by_name_type([FromBody] returnData returnData, CancellationToken ct = default)
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
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"設備名稱\",\"類別\"]";
                    return returnData.JsonSerializationt();
                }
                string 設備名稱 = returnData.ValueAry[0];
                string 類別 = returnData.ValueAry[1];
                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                sys_serverSettingClass sys_ServerSetting = await Method.GetServerAsync(設備名稱, 類別, "一般資料");

                if (sys_ServerSetting == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                string Master_GUID = sys_ServerSetting.GUID;
                string tableName = "medMap";
                SQLControl sQLControl_medMap = new SQLControl(Server, DB, "medMap", UserName, Password, Port, SSLMode);

                List<object[]> objects = await sQLControl_medMap.GetRowsByDefultAsync(null, (int)enum_medMap.Master_GUID, Master_GUID);

                if (objects.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                medMapClass medMapClasses = objects.SQLToClass<medMapClass, enum_medMap>()[0];
                medMapClasses.sys_ServerSetting = sys_ServerSetting;

                returnData returnData_get_medMap_section_by_Master_GUID = await get_medMap_section_by_Master_GUID(medMapClasses.GUID);
                if (returnData_get_medMap_section_by_Master_GUID == null || returnData_get_medMap_section_by_Master_GUID.Code != 200)
                {
                    return returnData_get_medMap_section_by_Master_GUID.JsonSerializationt(true);
                }
                List<medMap_sectionClass> medMap_SectionClasses = returnData_get_medMap_section_by_Master_GUID.Data.ObjToClass<List<medMap_sectionClass>>();
                medMapClasses.medMap_Section = medMap_SectionClasses;

                returnData.Code = 200;
                returnData.Data = medMapClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_by_name_type";
                returnData.Result = $"取得父容器資料成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以GUID取得父容器資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["設備名稱","類別"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medMap_by_GUID")]
        public async Task<string> get_medMap_by_GUID([FromBody] returnData returnData)
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
                string GUID = returnData.ValueAry[0];
                Task<List<sys_serverSettingClass>> task_sys_serverSettingClasses = ServerSettingController.GetAllServerSettingasync();

                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap = new SQLControl(Server, DB, "medMap", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_medMap.GetRowsByDefultAsync(null, (int)enum_medMap.GUID, GUID);
                if (objects.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                medMapClass medMapClasses = objects.SQLToClass<medMapClass, enum_medMap>()[0];
                returnData returnData_get_by_GUID = await new ServerSettingController().get_by_GUID(medMapClasses.Master_GUID);
                if (returnData_get_by_GUID == null || returnData_get_by_GUID.Code != 200)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                List<sys_serverSettingClass> sys_serverSettingClasses = returnData_get_by_GUID.Data.ObjToClass<List<sys_serverSettingClass>>();



                returnData returnData_get_medMap_section_by_Master_GUID = await get_medMap_section_by_Master_GUID(GUID);
                if (returnData_get_medMap_section_by_Master_GUID.Code != 200)
                {
                    return returnData_get_medMap_section_by_Master_GUID.JsonSerializationt(true);
                }
                List<medMap_sectionClass> medMap_SectionClasses = returnData_get_medMap_section_by_Master_GUID.Data.ObjToClass<List<medMap_sectionClass>>();

                if (sys_serverSettingClasses.Count > 0) medMapClasses.sys_ServerSetting = sys_serverSettingClasses[0];
                medMapClasses.medMap_Section = medMap_SectionClasses;

                returnData.Code = 200;
                returnData.Data = medMapClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_by_GUID";
                returnData.Result = $"取得父容器資料成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 新增子容器
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Value": "",
        ///     "ValueAry":["所屬父容器GUID","位置"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add_medMap_section")]
        public string add_medMap_section([FromBody] returnData returnData)
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
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"所屬父容器GUID\",\"位置\"]";
                    return returnData.JsonSerializationt();
                }
                string Master_GUID = returnData.ValueAry[0];
                string 位置 = returnData.ValueAry[1];
                if (位置.Split(",").Count() != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"位置資料錯誤，須為 \"0,1\"";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_section = new SQLControl(Server, DB, "medMap_section", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl_medMap_section.GetRowsByDefult(null, (int)enum_medMap_section.Master_GUID, Master_GUID);
                List<medMap_sectionClass> medMap_sectionClasses = objects.SQLToClass<medMap_sectionClass, enum_medMap_section>();
                if (medMap_sectionClasses.Count > 0)
                {
                    List<medMap_sectionClass> medMap_sectionClass_buff = medMap_sectionClasses.Where(item => item.位置 == 位置).ToList();
                    if (medMap_sectionClass_buff.Count > 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"位置已重複!";
                        return returnData.JsonSerializationt(true);
                    }
                }

                medMap_sectionClass medMap_sectionClass = new medMap_sectionClass();
                medMap_sectionClass.GUID = Guid.NewGuid().ToString();
                medMap_sectionClass.Master_GUID = Master_GUID;
                medMap_sectionClass.位置 = 位置;

                object[] add = medMap_sectionClass.ClassToSQL<medMap_sectionClass, enum_medMap_section>();
                sQLControl_medMap_section.AddRow(null, add);

                returnData.Code = 200;
                returnData.Data = medMap_sectionClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_medMap_section";
                returnData.Result = $"子容器資料寫入成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新子容器位置
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data":
        ///     [
        ///         {
        ///             "GUID":
        ///             "position":"0,1",
        ///             "absolute_position":"10,10"
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
        [HttpPost("update_medMap_section")]

        public async Task<string> update_medMap_section([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry不得為空";
                    return returnData.JsonSerializationt();
                }
                List<medMap_sectionClass> medMap_sectionClasses = returnData.Data.ObjToClass<List<medMap_sectionClass>>();
                string[] GUID = medMap_sectionClasses.Select(x => x.GUID).ToArray();
                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_section = new SQLControl(Server, DB, "medMap_section", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_medMap_section.GetRowsByDefultAsync(null, (int)enum_medMap_section.GUID, GUID);
                List<medMap_sectionClass> medMaps_section = objects.SQLToClass<medMap_sectionClass, enum_medMap_section>();
                if (medMaps_section.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                foreach (var item in medMaps_section)
                {
                    medMap_sectionClass medMap_section_buff = medMap_sectionClasses.Where(x => x.GUID == item.GUID).FirstOrDefault();
                    if (medMap_section_buff == null) continue;
                    if (medMap_section_buff.位置.StringIsEmpty() == false && medMap_section_buff.位置.Split(",").Count() == 2) item.位置 = medMap_section_buff.位置;
                    if (medMap_section_buff.絕對位置.StringIsEmpty() == false && medMap_section_buff.絕對位置.Split(",").Count() == 2) item.絕對位置 = medMap_section_buff.絕對位置;
                    if (medMap_section_buff.Master_GUID.StringIsEmpty() == false) item.Master_GUID = medMap_section_buff.Master_GUID;
                    if (medMap_section_buff.名稱.StringIsEmpty() == false) item.名稱 = medMap_section_buff.名稱;
                    if (medMap_section_buff.燈棒IP.StringIsEmpty() == false) item.燈棒IP = medMap_section_buff.燈棒IP;
                    if (medMap_section_buff.device_type.StringIsEmpty() == false) item.device_type = medMap_section_buff.device_type;
                    if (medMap_section_buff.type.StringIsEmpty() == false) item.type = medMap_section_buff.type;
                }

                List<object[]> update = medMaps_section.ClassToSQL<medMap_sectionClass, enum_medMap_section>();
                await sQLControl_medMap_section.UpdateRowsAsync(null, update);

                returnData.Code = 200;
                returnData.Data = medMaps_section;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_medMap_section";
                returnData.Result = $"子容器資料更新成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以Master_GUID取得子容器資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["Master_GUID"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medMap_section_by_Master_GUID")]
        public async Task<string> get_medMap_section_by_Master_GUID([FromBody] returnData returnData)
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
                string Master_GUID = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "medMap_section", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl.GetRowsByDefult(null, (int)enum_medMap_section.Master_GUID, Master_GUID);
                List<medMap_sectionClass> medMap_sectionClass = objects.SQLToClass<medMap_sectionClass, enum_medMap_section>();
                foreach (var item in medMap_sectionClass)
                {
                   
                        string subSection = await get_medMap_sub_section_by_Master_GUID(item.GUID);
                        returnData returnData_get_medMap_sub_section_by_Master_GUID = subSection.JsonDeserializet<returnData>();
                        if (returnData_get_medMap_sub_section_by_Master_GUID.Code == 200)
                        {
                            List<medMap_sub_sectionClass> medMap_sub_sectionClass = returnData_get_medMap_sub_section_by_Master_GUID.Data.ObjToClass<List<medMap_sub_sectionClass>>();
                            if (medMap_sub_sectionClass != null) item.sub_section = medMap_sub_sectionClass;
                        }
                        
                 


                }

                returnData.Code = 200;
                returnData.Data = medMap_sectionClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_section_by_Master_GUID";
                returnData.Result = $"取得子容器資料成功，共{medMap_sectionClass.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }

        [HttpPost("get_section_by_GUID")]
        public async Task<string> get_section_by_GUID([FromBody] returnData returnData)
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
                string[] GUID = returnData.ValueAry[0].Split(";").ToArray();
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "medMap_section", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medMap_section.GUID, GUID);
                List<medMap_sectionClass> medMap_sectionClass = objects.SQLToClass<medMap_sectionClass, enum_medMap_section>();

                returnData.Code = 200;
                returnData.Data = medMap_sectionClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_section_by_GUID";
                returnData.Result = $"取得子容器資料成功，共{medMap_sectionClass.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_section_by_IP")]
        public async Task<string> get_section_by_IP([FromBody] returnData returnData)
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
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"IP\"]";
                    return returnData.JsonSerializationt();
                }
                string IP = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                string command = @$"SELECT sec.*
                    FROM dbvm.stock AS s
                    JOIN dbvm.medmap_shelf AS sh
                    ON sh.GUID = s.shelf_GUID
                    JOIN dbvm.medmap_sub_section AS sub
                    ON sub.GUID = sh.Master_GUID
                    JOIN dbvm.medmap_section AS sec
                    ON sec.GUID = sub.Master_GUID
                    WHERE s.IP = '{IP}';";

                SQLControl sQLControl = new SQLControl(Server, DB, "medMap_section", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl.WriteCommandAsync(command);
                if (objects.Count == 0)
                {
                    command = @$"SELECT sec.*
                    FROM dbvm.medmap_shelf AS sh
                    JOIN dbvm.medmap_sub_section AS sub
                    ON sub.GUID = sh.Master_GUID
                    JOIN dbvm.medmap_section AS sec
                    ON sec.GUID = sub.Master_GUID
                    WHERE sh.燈條IP = '{IP}';";
                    objects = await sQLControl.WriteCommandAsync(command);
                    if (objects.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"查無對應SECTION";
                        return returnData.JsonSerializationt();
                    }
                }
                
                medMap_sectionClass medMap_sectionClass = objects.SQLToClass<medMap_sectionClass>()[0];

                returnData.Code = 200;
                returnData.Data = medMap_sectionClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_section_by_GUID";
                returnData.Result = $"取得子容器資料成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 新增sub_section容器
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Value": "",
        ///     "ValueAry":["所屬父容器GUID","位置"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add_sub_section")]
        public string add_sub_section([FromBody] returnData returnData)
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
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"所屬section\",\"位置\"]";
                    return returnData.JsonSerializationt();
                }
                string Master_GUID = returnData.ValueAry[0];
                string 位置 = returnData.ValueAry[1];
                if (位置.Split(",").Count() != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"位置資料錯誤，須為 \"0,1\"";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_sub_section = new SQLControl(Server, DB, "medMap_sub_section", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl_medMap_sub_section.GetRowsByDefult(null, (int)enum_medMap_sub_section.Master_GUID, Master_GUID);
                List<medMap_sub_sectionClass> medMap_sub_sectionClasses = objects.SQLToClass<medMap_sub_sectionClass, enum_medMap_sub_section>();
                if (medMap_sub_sectionClasses.Count > 0)
                {
                    medMap_sub_sectionClass medMap_sub_sectionClass_buff = medMap_sub_sectionClasses.FirstOrDefault(item => item.位置 == 位置);
                    if (medMap_sub_sectionClass_buff != null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"位置已重複!";
                        return returnData.JsonSerializationt(true);
                    }
                }

                medMap_sub_sectionClass medMap_sub_sectionClass = new medMap_sub_sectionClass();
                medMap_sub_sectionClass.GUID = Guid.NewGuid().ToString();
                medMap_sub_sectionClass.Master_GUID = Master_GUID;
                medMap_sub_sectionClass.位置 = 位置;
                medMap_sub_sectionClass.type = "sub_section";


                object[] add = medMap_sub_sectionClass.ClassToSQL<medMap_sub_sectionClass, enum_medMap_sub_section>();
                sQLControl_medMap_sub_section.AddRow(null, add);

                returnData.Code = 200;
                returnData.Data = medMap_sub_sectionClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_sub_section";
                returnData.Result = $"sub_section資料寫入成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("add_sub_section_data")]
        public string add_sub_section_data([FromBody] returnData returnData)
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
                List<medMap_sub_sectionClass> medMap_sub_sectionClasses = returnData.Data.ObjToClass<List<medMap_sub_sectionClass>>();
                if (medMap_sub_sectionClasses == null)
                {
                    medMap_sub_sectionClass medMap_sub_sectionClass = returnData.Data.ObjToClass<medMap_sub_sectionClass>();
                    if (medMap_sub_sectionClass == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data不得為空";
                        return returnData.JsonSerializationt();
                    }
                    medMap_sub_sectionClasses = new List<medMap_sub_sectionClass> { medMap_sub_sectionClass };
                }
                foreach (var item in medMap_sub_sectionClasses)
                {
                    item.GUID = Guid.NewGuid().ToString();
                    item.type = "sub_section";
                }

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_sub_section = new SQLControl(Server, DB, "medMap_sub_section", UserName, Password, Port, SSLMode);




                List<object[]> add = medMap_sub_sectionClasses.ClassToSQL<medMap_sub_sectionClass, enum_medMap_sub_section>();
                sQLControl_medMap_sub_section.AddRows(null, add);

                returnData.Code = 200;
                returnData.Data = medMap_sub_sectionClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_sub_section_data";
                returnData.Result = $"sub_section資料寫入成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新sub_section位置
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data":
        ///     [
        ///         {
        ///             "GUID":
        ///             "position":"0,1",
        ///             "absolute_position":"10,10",
        ///             "type":""
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
        [HttpPost("update_sub_section")]
        public async Task<string> update_sub_section([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry不得為空";
                    return returnData.JsonSerializationt();
                }
                List<medMap_sub_sectionClass> medMap_sub_sectionClasses = returnData.Data.ObjToClass<List<medMap_sub_sectionClass>>();
                string[] GUID = medMap_sub_sectionClasses.Select(x => x.GUID).ToArray();
                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_sub_section = new SQLControl(Server, DB, "medMap_sub_section", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_medMap_sub_section.GetRowsByDefultAsync(null, (int)enum_medMap_sub_section.GUID, GUID);
                List<medMap_sub_sectionClass> medMaps_sub_section = objects.SQLToClass<medMap_sub_sectionClass, enum_medMap_sub_section>();
                if (medMaps_sub_section.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                foreach (var item in medMaps_sub_section)
                {
                    medMap_sub_sectionClass medMap_sub_section_buff = medMap_sub_sectionClasses.FirstOrDefault(x => x.GUID == item.GUID);
                    if (medMap_sub_section_buff == null) continue;
                    if (medMap_sub_section_buff.位置.StringIsEmpty() == false && medMap_sub_section_buff.位置.Split(",").Count() == 2) item.位置 = medMap_sub_section_buff.位置;
                    if (medMap_sub_section_buff.Master_GUID.StringIsEmpty() == false) item.Master_GUID = medMap_sub_section_buff.Master_GUID;
                    if (medMap_sub_section_buff.type.StringIsEmpty() == false) item.type = medMap_sub_section_buff.type;
                    if (medMap_sub_section_buff.名稱.StringIsEmpty() == false) item.名稱 = medMap_sub_section_buff.名稱;

                }

                List<object[]> update = medMaps_sub_section.ClassToSQL<medMap_sub_sectionClass, enum_medMap_sub_section>();
                await sQLControl_medMap_sub_section.UpdateRowsAsync(null, update);

                returnData.Code = 200;
                returnData.Data = medMaps_sub_section;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_sub_section";
                returnData.Result = $"sub_section資料更新成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以GUID取得子容器資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["GUID"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_sub_section_by_GUID")]
        public async Task<string> get_sub_section_by_GUID([FromBody] returnData returnData)
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
                string[] GUID = returnData.ValueAry[0].Split(";").Distinct().ToArray();
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "medMap_sub_section", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medMap_sub_section.GUID, GUID);
                List<medMap_sub_sectionClass> medMap_Sub_Sections = objects.SQLToClass<medMap_sub_sectionClass, enum_medMap_sub_section>();

                returnData.Code = 200;
                returnData.Data = medMap_Sub_Sections;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_sub_section_by_GUID";
                returnData.Result = $"取得子容器資料成功，共{medMap_Sub_Sections.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以Master_GUID取得子容器資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["Master_GUID"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medMap_sub_section_by_Master_GUID")]
        public async Task<string> get_medMap_sub_section_by_Master_GUID([FromBody] returnData returnData)
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
                string Master_GUID = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "medMap_sub_section", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl.GetRowsByDefult(null, (int)enum_medMap_sub_section.Master_GUID, Master_GUID);
                List<medMap_sub_sectionClass> medMap_Sub_Sections = objects.SQLToClass<medMap_sub_sectionClass, enum_medMap_sub_section>();
                List<Task> tasks = new List<Task>();
                foreach (var item in medMap_Sub_Sections)
                {
                    
                        returnData returnData_get_medMap_shelf_by_Master_GUID = await get_medMap_shelf_by_Master_GUID(item.GUID);
                        if (returnData_get_medMap_shelf_by_Master_GUID.Code == 200)
                        {
                            List<medMap_shelfClass> medMap_ShelfClasses = returnData_get_medMap_shelf_by_Master_GUID.Data.ObjToClass<List<medMap_shelfClass>>();
                            if (medMap_ShelfClasses != null) item.shelf = medMap_ShelfClasses;
                        }
                       
                    
                    
                        string shelf_jsonString = get_medMap_drawer_by_Master_GUID(item.GUID);
                        returnData returnData_get_medMap_drawer_by_Master_GUID = shelf_jsonString.JsonDeserializet<returnData>();
                        if (returnData_get_medMap_drawer_by_Master_GUID.Code != 200) returnData_get_medMap_drawer_by_Master_GUID.JsonSerializationt(true);
                        List<medMap_drawerClass> medMap_drawerClasses = returnData_get_medMap_drawer_by_Master_GUID.Data.ObjToClass<List<medMap_drawerClass>>();
                        if (medMap_drawerClasses != null) item.drawer = medMap_drawerClasses;
                    

                }
                Task.WhenAll(tasks).Wait();
                medMap_Sub_Sections = medMap_Sub_Sections
                .OrderBy(x => int.Parse(x.位置.Split(',')[0]))               // 同理處理 位置
                .ThenBy(x => int.Parse(x.位置.Split(',')[1]))
                .ToList();
                returnData.Code = 200;
                returnData.Data = medMap_Sub_Sections;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_section_by_Master_GUID";
                returnData.Result = $"取得子容器資料成功，共{medMap_Sub_Sections.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 新增層架
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data":
        ///     {
        ///         "Master_GUID":"子容器GUID",
        ///         "position":"位置",
        ///         "width":"寬度",
        ///         "height":"高度",
        ///         "ip_light":"燈條IP",
        ///         "type":""
        ///         
        /// 
        ///     },
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
        [HttpPost("add_medMap_shelf")]
        [HttpPost("add_shelf")]
        public async Task<string> add_shelf([FromBody] returnData returnData)
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
                List<medMap_shelfClass> medMap_ShelfClasses = returnData.Data.ObjToClass<List<medMap_shelfClass>>();
                if (medMap_ShelfClasses == null)
                {
                    medMap_shelfClass medMap_ShelfClass = returnData.Data.ObjToClass<medMap_shelfClass>();
                    if (medMap_ShelfClass == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data資料錯誤";
                        return returnData.JsonSerializationt();
                    }
                    medMap_ShelfClasses = new List<medMap_shelfClass> { medMap_ShelfClass };
                }
                foreach (var item in medMap_ShelfClasses)
                {
                    item.GUID = Guid.NewGuid().ToString();
                }


                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_shelf = new SQLControl(Server, DB, "medMap_shelf", UserName, Password, Port, SSLMode);
               

                List<object[]> add = medMap_ShelfClasses.ClassToSQL<medMap_shelfClass, enum_medMap_shelf>();
                await sQLControl_medMap_shelf.AddRowsAsync(null, add);

               

                returnData.Code = 200;
                returnData.Data = medMap_ShelfClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_medMap_shelf";
                returnData.Result = $"層架資料寫入成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新層架資料(位置、寬度、高度、燈條IP)
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data":
        ///     {
        ///         "GUID":"",
        ///         "position":"位置",
        ///         "width":"寬度",
        ///         "height":"高度",
        ///         "ip_light":"燈條IP"
        ///     },
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
        [HttpPost("update_medMap_shelf")]
        [HttpPost("update_shelf")]
        public async Task<string> update_shelf([FromBody] returnData returnData)
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
                List<medMap_shelfClass> medMap_ShelfClass = returnData.Data.ObjToClass<List<medMap_shelfClass>>();
                string[] GUID = medMap_ShelfClass.Select(x => x.GUID).ToArray();

                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_shelf = new SQLControl(Server, DB, "medMap_shelf", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_medMap_shelf.GetRowsByDefultAsync(null, (int)enum_medMap_shelf.GUID, GUID);
                List<medMap_shelfClass> medMap_shelfClasses = objects.SQLToClass<medMap_shelfClass, enum_medMap_shelf>();
                if (medMap_shelfClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                foreach (var item in medMap_shelfClasses)
                {
                    medMap_shelfClass medMap_shelfClass_buff = medMap_ShelfClass.Where(x => x.GUID == item.GUID).FirstOrDefault();
                    if (medMap_shelfClass_buff == null) continue;
                    if (medMap_shelfClass_buff.位置.StringIsEmpty() == false && medMap_shelfClass_buff.位置.Split(",").Count() == 2) item.位置 = medMap_shelfClass_buff.位置;
                    if (medMap_shelfClass_buff.寬度.StringIsEmpty() == false) item.寬度 = medMap_shelfClass_buff.寬度;
                    if (medMap_shelfClass_buff.高度.StringIsEmpty() == false) item.高度 = medMap_shelfClass_buff.高度;
                    if (medMap_shelfClass_buff.燈條IP.StringIsEmpty() == false) item.燈條IP = medMap_shelfClass_buff.燈條IP;
                    if (medMap_shelfClass_buff.名稱.StringIsEmpty() == false) item.名稱 = medMap_shelfClass_buff.名稱;
                    if (medMap_shelfClass_buff.device_type.StringIsEmpty() == false) item.device_type = medMap_shelfClass_buff.device_type;


                    if (medMap_shelfClass_buff.Master_GUID.StringIsEmpty() == false && medMap_shelfClass_buff.serverName.StringIsEmpty() == false && medMap_shelfClass_buff.serverType.StringIsEmpty() == false)
                    {
                        item.Master_GUID = medMap_shelfClass_buff.Master_GUID;
                        item.serverName = medMap_shelfClass_buff.serverName;
                        item.serverType = medMap_shelfClass_buff.serverType;

                    }



                }
                List<object[]> update = medMap_shelfClasses.ClassToSQL<medMap_shelfClass, enum_medMap_shelf>();

                await sQLControl_medMap_shelf.UpdateRowsAsync(null, update);

                returnData.Code = 200;
                returnData.Data = medMap_shelfClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_medMap_section";
                returnData.Result = $"層架資料更新成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("delete_shelf_by_GUID")]
        public async Task<string> delete_shelf_by_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null || returnData.ValueAry.Count() != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry不得為空";
                    return returnData.JsonSerializationt();
                }
                string[] GUID = returnData.ValueAry[0].Split(";").Distinct().ToArray();

                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_shelf = new SQLControl(Server, DB, "medMap_shelf", UserName, Password, Port, SSLMode);
                List<object[]> delete = await sQLControl_medMap_shelf.GetRowsByDefultAsync(null, (int)enum_medMap_shelf.GUID, GUID);


                await sQLControl_medMap_shelf.DeleteRowsAsync(null, delete);
                await delete_box_by_Master_GUID(string.Join(";", GUID));

                returnData.Code = 200;
                returnData.Data = null;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "delete_shelf";
                returnData.Result = $"層架資料更新成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以GUID取得層架資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["GUID"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medMap_shelf_by_GUID")]
        public async Task<string> get_medMap_shelf_by_GUID([FromBody] returnData returnData)
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
                string GUID = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap = new SQLControl(Server, DB, "medMap_shelf", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl_medMap.GetRowsByDefult(null, (int)enum_medMap_shelf.GUID, GUID);
                if (objects.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                medMap_shelfClass medMap_shelfClass = objects.SQLToClass<medMap_shelfClass, enum_medMap_shelf>()[0];
                returnData returnData_get_medMap_box_by_Master_GUID = await get_medMap_box_by_Master_GUID(GUID);
                if (returnData_get_medMap_box_by_Master_GUID.Code != 200) return returnData_get_medMap_box_by_Master_GUID.JsonSerializationt(true);
                List<medMap_boxClass> medMap_boxClassses = returnData_get_medMap_box_by_Master_GUID.Data.ObjToClass<List<medMap_boxClass>>();
                medMap_shelfClass.medMapBox = medMap_boxClassses;



                returnData.Code = 200;
                returnData.Data = medMap_shelfClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_shelf_by_GUID";
                returnData.Result = $"取得層架資料成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        
        /// <summary>
        /// 以Master_GUID取得層架資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["Master_GUID"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medMap_shelf_by_Master_GUID")]
        public async Task<string> get_medMap_shelf_by_Master_GUID([FromBody] returnData returnData)
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
                if (returnData.ValueAry.Count != 1 || returnData.ValueAry[0].StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"GUID\"]";
                    return returnData.JsonSerializationt();
                }
                string Master_GUID = returnData.ValueAry[0];

                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "medMap_shelf", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl.GetRowsByDefult(null, (int)enum_medMap_shelf.Master_GUID, Master_GUID);
                List<medMap_shelfClass> medMap_shelfClass = objects.SQLToClass<medMap_shelfClass, enum_medMap_shelf>();
                foreach (var item in medMap_shelfClass)
                {
                    
                    RowsLED rowsLED = deviceApiClass.GetRowsLED_ByIP(API_server, item.serverName, item.serverType, item.燈條IP);
                    if (rowsLED != null) item.rowsLED = rowsLED;
                    
                   
                    returnData returnData_get_medMap_box_by_Master_GUID = await get_medMap_box_by_Master_GUID(item.GUID);
                    if (returnData_get_medMap_box_by_Master_GUID.Code != 200)
                    {
                        item.medMapBox = new List<medMap_boxClass>();
                           
                    }
                    else
                    {
                        List<medMap_boxClass> medMap_boxClassses = returnData_get_medMap_box_by_Master_GUID.Data.ObjToClass<List<medMap_boxClass>>();
                        item.medMapBox = medMap_boxClassses;
                    }

                    


                }
                string shelf_GUID = string.Join(";", medMap_shelfClass.Select(x => x.GUID).ToArray());
                returnData returnData_get_stock_by_shelf_GUID = await get_stock_by_shelf_GUID(shelf_GUID);

                if (returnData_get_stock_by_shelf_GUID.Code != 200)
                {
                    returnData_get_stock_by_shelf_GUID.Method = "get_shelf_by_type";
                    returnData_get_stock_by_shelf_GUID.TimeTaken = myTimerBasic.ToString();
                    return returnData_get_stock_by_shelf_GUID.JsonSerializationt(true);
                }
                List<stockClass> medMap_stockClasses = returnData_get_stock_by_shelf_GUID.Data.ObjToClass<List<stockClass>>();
                Dictionary<string, List<stockClass>> dic_stock = medMap_stockClasses.ToDictByShelfGUID();
                foreach (var item in medMap_shelfClass)
                {
                    List<stockClass> medMap_stock = dic_stock.GetByShelfGUID(item.GUID);
                    
                    item.medMapStock = medMap_stock;
                }
                medMap_shelfClass = medMap_shelfClass
                .OrderBy(x => int.Parse(x.位置.Split(',')[0]))               // 同理處理 位置
                .ThenBy(x => int.Parse(x.位置.Split(',')[1]))
                .ToList();

                returnData.Code = 200;
                returnData.Data = medMap_shelfClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_shelf_by_GUID";
                returnData.Result = $"取得層架資料成功，共{medMap_shelfClass.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_sub_section_by_shelf_type")]
        public async Task<string> get_sub_section_by_shelf_type([FromBody] returnData returnData)
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
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"shelf OR dispensing_shelves\"]";
                    return returnData.JsonSerializationt();
                }
                string type = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap = new SQLControl(Server, DB, "medMap_shelf", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl_medMap.GetRowsByDefult(null, (int)enum_medMap_shelf.type, type);
                if (objects.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                List<medMap_shelfClass> medMap_ShelfClasses = objects.SQLToClass<medMap_shelfClass, enum_medMap_shelf>();
                string GUID = string.Join(";", medMap_ShelfClasses.Select(x => x.GUID).ToArray());
                string master_GUID = string.Join(";", medMap_ShelfClasses.Select(x => x.Master_GUID).ToArray());
                Task<returnData> task_get_sub_section_by_GUID = get_sub_section_by_GUID(master_GUID);
                returnData returnData_get_stock_by_shelf_GUID = await get_stock_by_shelf_GUID(GUID);

                if (returnData_get_stock_by_shelf_GUID.Code != 200)
                {
                    returnData_get_stock_by_shelf_GUID.Method = "get_shelf_by_type";
                    returnData_get_stock_by_shelf_GUID.TimeTaken = myTimerBasic.ToString();
                    return returnData_get_stock_by_shelf_GUID.JsonSerializationt(true);
                }
                List<stockClass> medMap_stockClasses = returnData_get_stock_by_shelf_GUID.Data.ObjToClass<List<stockClass>>();
                Dictionary<string, List<stockClass>> dic_stock = medMap_stockClasses.ToDictByShelfGUID();
                foreach (var item in medMap_ShelfClasses)
                {
                    List<stockClass> medMap_stock = dic_stock.GetByShelfGUID(item.GUID);
                    item.medMapStock = medMap_stock;
                }
                Dictionary<string, List<medMap_shelfClass>> dic_shelf = medMap_ShelfClasses.ToDictByMasterGUID();
                returnData returnData_get_sub_section_by_GUID = await task_get_sub_section_by_GUID;
                if (returnData_get_sub_section_by_GUID.Code != 200)
                {
                    returnData_get_sub_section_by_GUID.Method = "get_shelf_by_type";
                    returnData_get_sub_section_by_GUID.TimeTaken = myTimerBasic.ToString();
                    return returnData_get_sub_section_by_GUID.JsonSerializationt(true);
                }
                List<medMap_sub_sectionClass> medMap_Sub_SectionClasses = returnData_get_sub_section_by_GUID.Data.ObjToClass<List<medMap_sub_sectionClass>>();

                string section_guid = string.Join(";", medMap_Sub_SectionClasses.Select(x => x.Master_GUID).ToArray());
                returnData returnData_get_section_by_GUID = await get_section_by_GUID(section_guid);
                if (returnData_get_section_by_GUID.Code != 200)
                {
                    returnData_get_section_by_GUID.Method = "get_shelf_by_type";
                    returnData_get_section_by_GUID.TimeTaken = myTimerBasic.ToString();
                    return returnData_get_section_by_GUID.JsonSerializationt(true);
                }
                List<medMap_sectionClass> medMap_SectionClasses = returnData_get_section_by_GUID.Data.ObjToClass<List<medMap_sectionClass>>();

                foreach (var item in medMap_Sub_SectionClasses)
                {
                    medMap_sectionClass section_buff = medMap_SectionClasses.FirstOrDefault(x => x.GUID == item.Master_GUID);
                    if (section_buff != null) item.section_position = section_buff.位置;
                    List<medMap_shelfClass> shelf = dic_shelf.GetByMasterGUID(item.GUID);
                    shelf = shelf.OrderBy(x => x.位置).ToList();
                    item.shelf = shelf;
                }
                medMap_Sub_SectionClasses = medMap_Sub_SectionClasses
                .OrderBy(x => int.Parse(x.section_position.Split(',')[0]))  // 取前半部分 (例如 0)
                .ThenBy(x => int.Parse(x.section_position.Split(',')[1]))   // 取後半部分 (例如 1)
                .ThenBy(x => int.Parse(x.位置.Split(',')[0]))               // 同理處理 位置
                .ThenBy(x => int.Parse(x.位置.Split(',')[1]))
                .ToList();

                returnData.Code = 200;
                returnData.Data = medMap_Sub_SectionClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_sub_section_by_shelf_type";
                returnData.Result = $"取得層架資料成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 新增抽屜
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data":
        ///     {
        ///         "Master_GUID":"子容器GUID",
        ///         "position":"位置",
        ///         "width":"寬度",
        ///         "height":"高度",
        ///         "ip_drawer":"抽屜IP"
        ///     },
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
        [HttpPost("add_medMap_drawer")]
        public string add_medMap_drawer([FromBody] returnData returnData)
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
                medMap_drawerClass medMap_drawerClass = returnData.Data.ObjToClass<medMap_drawerClass>();
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data資料錯誤，須為 medMap_shelfClass";
                    return returnData.JsonSerializationt();
                }
                if (medMap_drawerClass.位置.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"位置資料為空，須為 \"0,1\"";
                    return returnData.JsonSerializationt();
                }
                if (medMap_drawerClass.位置.Split(",").Count() != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"位置資料錯誤，須為 \"0,1\"";
                    return returnData.JsonSerializationt();
                }
                if (medMap_drawerClass.Master_GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"Master_GUID資料為空";
                    return returnData.JsonSerializationt();
                }
                if (medMap_drawerClass.寬度.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"寬度資料為空";
                    return returnData.JsonSerializationt();
                }
                if (medMap_drawerClass.高度.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"高度資料為空";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_drawer = new SQLControl(Server, DB, "medMap_drawer", UserName, Password, Port, SSLMode);

                List<object[]> objects = sQLControl_medMap_drawer.GetRowsByDefult(null, (int)enum_medMap_drawer.Master_GUID, medMap_drawerClass.Master_GUID);
                List<medMap_drawerClass> medMap_drawerClasses = objects.SQLToClass<medMap_drawerClass, enum_medMap_drawer>();
                if (medMap_drawerClasses.Count > 0)
                {
                    List<medMap_drawerClass> medMap_drawerClass_buff = medMap_drawerClasses.Where(item => item.位置 == medMap_drawerClass.位置).ToList();
                    if (medMap_drawerClass_buff.Count > 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"位置已重複!";
                        return returnData.JsonSerializationt();
                    }
                }
                medMap_drawerClass.GUID = Guid.NewGuid().ToString();

                object[] add = medMap_drawerClass.ClassToSQL<medMap_drawerClass, enum_medMap_drawer>();
                sQLControl_medMap_drawer.AddRow(null, add);

                returnData.Code = 200;
                returnData.Data = medMap_drawerClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_medMap_drawer";
                returnData.Result = $"抽屜資料寫入成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新抽屜資料(位置、寬度、高度、抽屜IP)
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data":
        ///     {
        ///         "GUID":"",
        ///         "position":"位置",
        ///         "width":"寬度",
        ///         "height":"高度",
        ///         "ip_light":"抽屜IP"
        ///     },
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
        [HttpPost("update_medMap_drawer")]
        public async Task<string> update_medMap_drawer([FromBody] returnData returnData)
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
                List<medMap_drawerClass> medMap_drawerClass = returnData.Data.ObjToClass<List<medMap_drawerClass>>();
                string[] GUID = medMap_drawerClass.Select(x => x.GUID).ToArray();

                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_drawer = new SQLControl(Server, DB, "medMap_drawer", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_medMap_drawer.GetRowsByDefultAsync(null, (int)enum_medMap_drawer.GUID, GUID);
                List<medMap_drawerClass> medMap_drawerClasses = objects.SQLToClass<medMap_drawerClass, enum_medMap_drawer>();
                if (medMap_drawerClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                foreach (var item in medMap_drawerClasses)
                {
                    medMap_drawerClass medMap_shelfClass_buff = medMap_drawerClass.Where(x => x.GUID == item.GUID).FirstOrDefault();
                    if (medMap_shelfClass_buff == null) continue;
                    if (medMap_shelfClass_buff.位置.StringIsEmpty() == false && medMap_shelfClass_buff.位置.Split(",").Count() == 2) item.位置 = medMap_shelfClass_buff.位置;
                    if (medMap_shelfClass_buff.寬度.StringIsEmpty() == false) item.寬度 = medMap_shelfClass_buff.寬度;
                    if (medMap_shelfClass_buff.高度.StringIsEmpty() == false) item.高度 = medMap_shelfClass_buff.高度;
                    if (medMap_shelfClass_buff.抽屜IP.StringIsEmpty() == false) item.抽屜IP = medMap_shelfClass_buff.抽屜IP;
                    if (medMap_shelfClass_buff.Master_GUID.StringIsEmpty() == false && medMap_shelfClass_buff.serverName.StringIsEmpty() == false && medMap_shelfClass_buff.serverType.StringIsEmpty() == false)
                    {
                        item.Master_GUID = medMap_shelfClass_buff.Master_GUID;
                        item.serverName = medMap_shelfClass_buff.serverName;
                        item.serverType = medMap_shelfClass_buff.serverType;
                    }

                }

                List<object[]> update = medMap_drawerClasses.ClassToSQL<medMap_drawerClass, enum_medMap_drawer>();
                await sQLControl_medMap_drawer.UpdateRowsAsync(null, update);

                returnData.Code = 200;
                returnData.Data = medMap_drawerClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_medMap_drawer";
                returnData.Result = $"抽屜資料更新成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以Master_GUID取得抽屜資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["Master_GUID"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medMap_drawer_by_Master_GUID")]
        public string get_medMap_drawer_by_Master_GUID([FromBody] returnData returnData)
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
                string Master_GUID = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "medMap_drawer", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl.GetRowsByDefult(null, (int)enum_medMap_drawer.Master_GUID, Master_GUID);
                List<medMap_drawerClass> medMap_DrawerClasses = objects.SQLToClass<medMap_drawerClass, enum_medMap_drawer>();
                List<Task> tasks = new List<Task>();
                foreach (var item in medMap_DrawerClasses)
                {
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        Drawer drawer = deviceApiClass.Get_EPD583_Drawer_ByIP(API_server, item.serverName, item.serverType, item.抽屜IP);
                        if (drawer != null) item.drawer = drawer;
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        Storage storage = deviceApiClass.Get_EPD266_Storage_ByIP(API_server, item.serverName, item.serverType, item.抽屜IP);
                        if (storage != null) item.storage = storage;
                    })));
                }
                Task.WhenAll(tasks).Wait();
                medMap_DrawerClasses = medMap_DrawerClasses
                .OrderBy(x => int.Parse(x.位置.Split(',')[0]))               // 同理處理 位置
                .ThenBy(x => int.Parse(x.位置.Split(',')[1]))
                .ToList();
                returnData.Code = 200;
                returnData.Data = medMap_DrawerClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_shelf_by_GUID";
                returnData.Result = $"取得抽屜資料成功，共{medMap_DrawerClasses.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 新增藥盒
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data":
        ///     {
        ///         "Master_GUID":"子容器GUID",
        ///         "position":"位置",
        ///         "width":"寬度",
        ///         "height":"高度",
        ///         "ip_box":"藥盒IP",
        ///         "serverName":"",
        ///         "serverType":""
        ///     },
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
        [HttpPost("add_medMap_box")]
        public string add_medMap_box([FromBody] returnData returnData)
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
                medMap_boxClass medMap_boxClass = returnData.Data.ObjToClass<medMap_boxClass>();
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data資料錯誤，須為medMap_boxClass";
                    return returnData.JsonSerializationt();
                }
                if (medMap_boxClass.位置.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"位置資料為空，須為 \"0,1\"";
                    return returnData.JsonSerializationt();
                }
                if (medMap_boxClass.位置.Split(",").Count() != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"位置資料錯誤，須為 \"0,1\"";
                    return returnData.JsonSerializationt();
                }
                if (medMap_boxClass.Master_GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"Master_GUID資料為空";
                    return returnData.JsonSerializationt();
                }
                if (medMap_boxClass.寬度.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"寬度資料為空";
                    return returnData.JsonSerializationt();
                }
                if (medMap_boxClass.高度.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"高度資料為空";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_box = new SQLControl(Server, DB, "medMap_box", UserName, Password, Port, SSLMode);

                List<object[]> objects = sQLControl_medMap_box.GetRowsByDefult(null, (int)enum_medMap_box.Master_GUID, medMap_boxClass.Master_GUID);
                List<medMap_boxClass> medMap_boxClasses = objects.SQLToClass<medMap_boxClass, enum_medMap_box>();
                if (medMap_boxClasses.Count > 0)
                {
                    List<medMap_boxClass> medMap_boxClass_buff = medMap_boxClasses.Where(item => item.位置 == medMap_boxClass.位置).ToList();
                    if (medMap_boxClass_buff.Count > 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"位置已重複!";
                        return returnData.JsonSerializationt();
                    }
                }
                medMap_boxClass.GUID = Guid.NewGuid().ToString();

                object[] add = medMap_boxClass.ClassToSQL<medMap_boxClass, enum_medMap_box>();
                sQLControl_medMap_box.AddRow(null, add);

                returnData.Code = 200;
                returnData.Data = medMap_boxClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_medMap_box";
                returnData.Result = $"藥盒寫入成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新藥盒資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data":
        ///     [
        ///         {
        ///             "GUID":"",
        ///             "Master_GUID":"",
        ///             "position":"位置",
        ///             "width":"寬度",
        ///             "height":"高度",
        ///             "ip_light":"藥盒IP",
        ///             "serverName":"",
        ///             "serverType":"",
        ///         }
        ///     ],
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
        [HttpPost("update_medMap_box")]
        public async Task<string> update_medMap_box([FromBody] returnData returnData)
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
                List<medMap_boxClass> medMap_boxClass = returnData.Data.ObjToClass<List<medMap_boxClass>>();
                string[] GUID = medMap_boxClass.Select(x => x.GUID).ToArray();


                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_box = new SQLControl(Server, DB, "medMap_box", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_medMap_box.GetRowsByDefultAsync(null, (int)enum_medMap_box.GUID, GUID);
                List<medMap_boxClass> medMap_boxClasses = objects.SQLToClass<medMap_boxClass, enum_medMap_box>();
                if (medMap_boxClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                foreach (var item in medMap_boxClasses)
                {
                    medMap_boxClass medMap_box_buff = medMap_boxClass.Where(x => x.GUID == item.GUID).FirstOrDefault();
                    if (medMap_box_buff == null) continue;
                    if (medMap_box_buff.位置.StringIsEmpty() == false && medMap_box_buff.位置.Split(",").Count() == 2) item.位置 = medMap_box_buff.位置;
                    if (medMap_box_buff.寬度.StringIsEmpty() == false && medMap_box_buff.寬度.Split(",").Count() == 2) item.寬度 = medMap_box_buff.寬度;
                    if (medMap_box_buff.高度.StringIsEmpty() == false) item.高度 = medMap_box_buff.高度;
                    if (medMap_box_buff.藥盒IP.StringIsEmpty() == false) item.藥盒IP = medMap_box_buff.藥盒IP;
                    if (medMap_box_buff.Master_GUID.StringIsEmpty() == false && medMap_box_buff.serverName.StringIsEmpty() == false && medMap_box_buff.serverType.StringIsEmpty() == false)
                    {
                        item.Master_GUID = medMap_box_buff.Master_GUID;
                        item.serverName = medMap_box_buff.serverName;
                        item.serverType = medMap_box_buff.serverType;
                    }
                }

                List<object[]> update = medMap_boxClasses.ClassToSQL<medMap_boxClass, enum_medMap_box>();
                await sQLControl_medMap_box.UpdateRowsAsync(null, update);

                returnData.Code = 200;
                returnData.Data = medMap_boxClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_medMap_box";
                returnData.Result = $"藥盒資料更新成功!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以Master_GUID取得藥盒資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["Master_GUID"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medMap_box_by_Master_GUID")]
        public async Task<string> get_medMap_box_by_Master_GUID([FromBody] returnData returnData)
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
                if (returnData.ValueAry.Count != 1 || returnData.ValueAry[0].StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"GUID\"]";
                    return returnData.JsonSerializationt();
                }
                string Master_GUID = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");

                SQLControl sQLControl = new SQLControl(Server, DB, "medMap_box", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medMap_box.Master_GUID, Master_GUID);
                if (objects.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料不存在!";
                    return returnData.JsonSerializationt();
                }
                List<medMap_boxClass> medMap_BoxClasses = objects.SQLToClass<medMap_boxClass, enum_medMap_box>();
                List<Task> tasks = new List<Task>();
                foreach (var item in medMap_BoxClasses)
                {
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        Storage storage = deviceApiClass.Get_EPD266_Storage_ByIP(API_server, item.serverName, item.serverType, item.藥盒IP);
                        if (storage != null) item.storage = storage;
                    })));
                }
                Task.WhenAll(tasks).Wait();
                medMap_BoxClasses = medMap_BoxClasses
                .OrderBy(x => int.Parse(x.位置.Split(',')[0]))               // 同理處理 位置
                .ThenBy(x => int.Parse(x.位置.Split(',')[1]))
                .ToList();
                returnData.Code = 200;
                returnData.Data = medMap_BoxClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_shelf_by_GUID";
                returnData.Result = $"取得藥盒資料成功，共{medMap_BoxClasses.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }

        [HttpPost("delete_box_by_Master_GUID")]
        public async Task<string> delete_box_by_Master_GUID([FromBody] returnData returnData)
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
                if (returnData.ValueAry.Count != 1 || returnData.ValueAry[0].StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"GUID\"]";
                    return returnData.JsonSerializationt();
                }
                string[] Master_GUID = returnData.ValueAry[0].Split(";").Distinct().ToArray();
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");

                SQLControl sQLControl = new SQLControl(Server, DB, "medMap_box", UserName, Password, Port, SSLMode);
                List<object[]> delete = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medMap_box.Master_GUID, Master_GUID);

                if (delete.Count() > 0) await sQLControl.DeleteRowsAsync(null, delete);

                returnData.Code = 200;
                returnData.Data = null;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_shelf_by_GUID";
                returnData.Result = $"刪除藥盒資料成功，共{delete.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 新增 medMap_stock 儲位資料（支援單筆或多筆）
        /// </summary>
        /// <remarks>
        /// 本 API 會：
        /// 1. 驗證 <c>returnData.Data</c> 是否存在，並嘗試轉為 <c>List&lt;stockClass&gt;</c>；若失敗則再嘗試單筆 <c>stockClass</c>。  
        /// 2. 為每一筆資料自動產生 <c>GUID</c>。  
        /// 3. 以批次方式寫入資料表 <c>medMap_stock</c>。  
        ///
        /// 【必要欄位】(每筆)
        /// - <c>Shelf_GUID</c>：對應層架 GUID。  
        /// - <c>位置</c>：儲位座標，建議格式 <c>"row,col"</c>（兩段）。  
        /// - <c>code / 藥碼</c>：藥碼（實際欄位名稱依你的 class/enum）。  
        ///
        /// 【常見驗證錯誤】
        /// - <c>returnData.Data</c> 為空。  
        /// - <c>returnData.Data</c> 轉型失敗（非 <c>stockClass</c> / <c>List&lt;stockClass&gt;</c>）。  
        /// - 必填欄位為空（例如 <c>Shelf_GUID</c>、<c>位置</c>、<c>藥碼</c>）。  
        ///
        /// 【單筆請求範例】
        /// <code>
        /// {
        ///   "Data": {
        ///     "GUID": "",
        ///     "shelf_guid": "SHELF-001",
        ///     "device_type": "LED_BAR",
        ///     "location": "0,1",
        ///     "ip": "192.168.1.20",
        ///     "led_index": "12",
        ///     "code": "OPER7"
        ///   },
        ///   "Value": "",
        ///   "ValueAry": [],
        ///   "TableName": "stock",
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "TimeTaken": ""
        /// }
        /// </code>
        ///
        /// 【多筆請求範例】
        /// <code>
        /// {
        ///   "Data": [
        ///     {
        ///       "GUID": "",
        ///       "shelf_guid": "SHELF-001",
        ///       "device_type": "LED_BAR",
        ///       "location": "0,1",
        ///       "ip": "192.168.1.20",
        ///       "led_index": "12",
        ///       "code": "OPER7"
        ///     },
        ///     {
        ///       "GUID": "",
        ///       "shelf_guid": "SHELF-001",
        ///       "device_type": "LED_BAR",
        ///       "location": "0,2",
        ///       "ip": "192.168.1.21",
        ///       "led_index": "13",
        ///       "code": "EPAR"
        ///     }
        ///   ],
        ///   "Value": "",
        ///   "ValueAry": [],
        ///   "TableName": "stock",
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "TimeTaken": ""
        /// }
        /// </code>
        ///
        /// 【成功回應範例】
        /// <code>
        /// {
        ///   "Code": 200,
        ///   "Result": "儲位寫入成功!",
        ///   "Data": [
        ///     {
        ///       "GUID": "f3c5a0b2-5d7a-46a8-9a2a-5f6b1d0e1c22",
        ///       "shelf_guid": "SHELF-001",
        ///       "device_type": "LED_BAR",
        ///       "location": "0,1",
        ///       "ip": "192.168.1.20",
        ///       "led_index": "12",
        ///       "code": "OPER7"
        ///     },
        ///     {
        ///       "GUID": "4b65f0fb-8f2a-4f0a-8a77-3d0f2a1b9b33",
        ///       "shelf_guid": "SHELF-001",
        ///       "device_type": "LED_BAR",
        ///       "location": "0,2",
        ///       "ip": "192.168.1.21",
        ///       "led_index": "13",
        ///       "code": "EPAR"
        ///     }
        ///   ],
        ///   "TimeTaken": "85.123ms",
        ///   "Method": "add_medMap_stock"
        /// }
        /// </code>
        ///
        /// 【失敗回應範例】（資料為空）
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "returnData.Data不得為空",
        ///   "Data": null,
        ///   "Method": "add_medMap_stock",
        ///   "TimeTaken": "3.221ms"
        /// }
        /// </code>
        ///
        /// 備註：本 API 僅新增資料，不負責檢查 <c>位置</c> 重複或唯一性；若需要「同一 <c>Shelf_GUID</c> + <c>位置</c> 不可重複」之約束，請在資料庫層加上複合唯一鍵或在呼叫前先自檢。
        /// </remarks>
        /// <param name="returnData">
        /// 請求包裝物件；其中 <c>Data</c> 可為 <c>stockClass</c>（單筆）或 <c>List&lt;stockClass&gt;</c>（多筆）。
        /// </param>
        /// <returns>
        /// <para>JSON 字串；欄位說明：</para>
        /// <list type="bullet">
        /// <item><description><c>Code</c>：200 表示成功，-200 表示失敗。</description></item>
        /// <item><description><c>Result</c>：結果描述。</description></item>
        /// <item><description><c>Data</c>：成功時回傳實際寫入（並補上 GUID）的單筆或多筆資料。</description></item>
        /// <item><description><c>TimeTaken</c>：耗時字串。</description></item>
        /// <item><description><c>Method</c>：方法名稱 <c>"add_medMap_stock"</c>。</description></item>
        /// </list>
        /// </returns>
        [HttpPost("add_stock")]
        public async Task<string> add_medMap_stock([FromBody] returnData returnData)
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
                List<stockClass> medMap_StockClasses = returnData.Data.ObjToClass<List<stockClass>>();
                if (medMap_StockClasses == null)
                {
                    stockClass medMap_stock = returnData.Data.ObjToClass<stockClass>();
                    if (medMap_stock == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data資料錯誤，須為stockClass";
                        return returnData.JsonSerializationt();
                    }
                    medMap_StockClasses = new List<stockClass>() { medMap_stock };
                }
                DateTime dateTime = DateTime.Now;
                foreach (var item in medMap_StockClasses)
                {
                    item.GUID = Guid.NewGuid().ToString();
                    string value = item.Value;
                    for (int i = 0; i < item.效期.Count; i++)
                    {
                        if (item.效期.Count != item.批號.Count && item.效期.Count != item.數量.Count) continue;
                        if (value.StringIsEmpty()) value = new DeviceBasic().JsonSerializationt();
                        DeviceBasic deviceBasic = value.JsonDeserializet<DeviceBasic>();

                        deviceBasic.新增效期(item.效期[i], item.批號[i], item.數量[i]);
                        item.Value = deviceBasic.JsonSerializationt();
                    }
                }
                // DB 連線與資料表
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_stock = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);

                List<object[]> add = medMap_StockClasses.ClassToSQL<stockClass>();
                await sQLControl_medMap_stock.AddRowsAsync(null, add);
                // 回傳
                returnData.Code = 200;
                returnData.Data = medMap_StockClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_medMap_stock";
                returnData.Result = $"儲位寫入成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("update_stock")]
        public async Task<string> update_stock([FromBody] returnData returnData)
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
                List<stockClass> medMap_StockClasses = returnData.Data.ObjToClass<List<stockClass>>();
                if (medMap_StockClasses == null)
                {
                    stockClass medMap_stock = returnData.Data.ObjToClass<stockClass>();
                    if (medMap_stock == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data資料錯誤，須為stockClass";
                        return returnData.JsonSerializationt();
                    }
                    medMap_StockClasses = new List<stockClass>() { medMap_stock };
                }
                // DB 連線與資料表
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_stock = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_medMap_stock.GetRowsByDefultAsync(null, (int)enum_medMap_stock.GUID, medMap_StockClasses.Select(x => x.GUID).ToArray());
                List<stockClass> db_medMap_StockClasses = objects.SQLToClass<stockClass>();
                foreach (var item in db_medMap_StockClasses)
                {
                    stockClass medMap_stock_buff = medMap_StockClasses.Where(x => x.GUID == item.GUID).FirstOrDefault();
                    if (medMap_stock_buff == null) continue;
                    if (medMap_stock_buff.Shelf_GUID.StringIsEmpty() == false) item.Shelf_GUID = medMap_stock_buff.Shelf_GUID;
                    if (medMap_stock_buff.位置.StringIsEmpty() == false) item.位置 = medMap_stock_buff.位置;
                    if (medMap_stock_buff.IP.StringIsEmpty() == false) item.IP = medMap_stock_buff.IP;
                    if (medMap_stock_buff.device_type.StringIsEmpty() == false) item.device_type = medMap_stock_buff.device_type;
                    if (medMap_stock_buff.燈條亮燈位置.StringIsEmpty() == false) item.燈條亮燈位置 = medMap_stock_buff.燈條亮燈位置;
                    if (medMap_stock_buff.Classify_GUID.StringIsEmpty() == false) item.Classify_GUID = medMap_stock_buff.Classify_GUID;

                    if (medMap_stock_buff.藥碼.StringIsEmpty() == false) item.藥碼 = medMap_stock_buff.藥碼;
                    if (medMap_stock_buff.藥名.StringIsEmpty() == false) item.藥名 = medMap_stock_buff.藥名;
                    if (medMap_stock_buff.料號.StringIsEmpty() == false) item.料號 = medMap_stock_buff.料號;
                    if (medMap_stock_buff.效期 == null || (medMap_stock_buff.效期.Count != medMap_stock_buff.批號.Count && medMap_stock_buff.效期.Count != medMap_stock_buff.數量.Count)) continue;
                    for (int i = 0; i < medMap_stock_buff.效期.Count; i++)
                    {
                        string value = item.Value;                        
                        if (value.StringIsEmpty()) value = new DeviceBasic().JsonSerializationt();
                        DeviceBasic deviceBasic = value.JsonDeserializet<DeviceBasic>();
                        deviceBasic.效期庫存覆蓋(medMap_stock_buff.效期[i], medMap_stock_buff.批號[i], medMap_stock_buff.數量[i]);
                        item.Value = deviceBasic.JsonSerializationt();
                    }
                }               
                List<object[]> update = db_medMap_StockClasses.ClassToSQL<stockClass>();
                await sQLControl_medMap_stock.UpdateRowsAsync(null, update);
                
                returnData.Code = 200;
                returnData.Data = db_medMap_StockClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_medMap_stock";
                returnData.Result = $"儲位寫入成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }

        /// <summary>
        /// 依指定 <c>GUID</c>，從該藥品庫存的 <c>Value</c>（<c>DeviceBasic</c> 序列化資料）中
        /// 清除一筆指定效期（有效日期）的紀錄。
        /// </summary>
        /// <remarks>
        /// <example>
        /// <code>
        /// 請求（Request）Body：
        /// {
        ///   "ValueAry": [ "b9f6d6e2-3a9b-4f2e-8d56-7c0f1f0a1a23", "2026-12-31" ],
        /// }
        ///
        /// 成功回應（Success, Code=200）：
        /// {
        ///   "Code": 200,
        ///   "Result": "清除效期成功!",
        ///   "Data": [
        ///     {
        ///       "GUID": "b9f6d6e2-3a9b-4f2e-8d56-7c0f1f0a1a23",
        ///       "Value": "{...更新後的 DeviceBasic JSON...}",
        ///       "...其他欄位..."
        ///     }
        ///   ],
        ///   "TimeTaken": "00:00:00.123",
        ///   "Method": "stock_deldte_Validity_period"
        /// }
        ///
        /// 失敗回應（參數錯誤, Code=-200）：
        /// {
        ///   "Code": -200,
        ///   "Result": "ValueAry應為[\"GUID\",\"效期\"] ",
        ///   "TimeTaken": "00:00:00.002"
        /// }
        ///
        /// 失敗回應（GUID 不存在, Code=-200）：
        /// {
        ///   "Code": -200,
        ///   "Result": "查無此GUID資料(bad-guid)",
        ///   "TimeTaken": "00:00:00.005"
        /// }
        ///
        /// 失敗回應（效期格式不正確, Code=-200）：
        /// {
        ///   "Code": -200,
        ///   "Result": "效期日期格式錯誤",
        ///   "TimeTaken": "00:00:00.003"
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="returnData">
        /// 請求物件。<c>ValueAry</c> 必須為長度 2 的陣列：<c>["GUID","效期"]</c>。
        /// </param>
        /// <returns>
        /// <c>returnData</c> 的 JSON 字串：成功 <c>Code=200</c> 並回傳更新後資料；失敗 <c>Code=-200</c> 並含錯誤訊息。
        /// </returns>

        [HttpPost("stock_deldte_Validity_period")]
        public async Task<string> stock_deldte_Validity_period([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry應為[\"GUID\",\"效期\"] ";
                    return returnData.JsonSerializationt();
                }
                string GUID = returnData.ValueAry[0];
                string 效期 = returnData.ValueAry[1];

                if (GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"GUID不得為空值";
                    return returnData.JsonSerializationt();
                }
              
                if (效期.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"效期日期格式錯誤";
                    return returnData.JsonSerializationt();
                }

                // DB 連線與資料表
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl_medMap_stock = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);

                List<object[]> data = await sQLControl_medMap_stock.GetRowsByDefultAsync(null, (int)enum_medMap_stock.GUID, GUID);
                List<stockClass> medMap_StockClasses = data.SQLToClass<stockClass>();
                if (medMap_StockClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此GUID資料({GUID})";
                    return returnData.JsonSerializationt();
                }
                string value = medMap_StockClasses[0].Value;
                if (value.StringIsEmpty()) value = new DeviceBasic().JsonSerializationt();
                DeviceBasic deviceBasic = value.JsonDeserializet<DeviceBasic>();
                deviceBasic.清除效期(效期);
                medMap_StockClasses[0].Value = deviceBasic.JsonSerializationt();
                object[] update = medMap_StockClasses[0].ClassToSQL<stockClass>();
                await sQLControl_medMap_stock.UpdateRowAsync(null, update);
                returnData.Code = 200;
                returnData.Data = medMap_StockClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "stock_deldte_Validity_period";
                returnData.Result = $"清除效期成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("delete_stock_by_GUID")]
        public async Task<string> delete_stock_by_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data不得為空";
                    return returnData.JsonSerializationt();
                }
                string[] GUID = returnData.ValueAry[0].Split(";").Distinct().ToArray();

                // DB 連線與資料表
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_stock = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);
                List<object[]> delete = await sQLControl_medMap_stock.GetRowsByDefultAsync(null, (int)enum_medMap_stock.GUID, GUID);

                if (delete.Count > 0) await sQLControl_medMap_stock.DeleteRowsAsync(null, delete);

                // 回傳
                returnData.Code = 200;
                returnData.Data = null;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "delete_stock_by_GUID";
                returnData.Result = $"刪除儲位共<{delete.Count}>筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得所有儲位資料（medMap_stock）。
        /// </summary>
        /// <param name="returnData">
        /// 呼叫端傳入的通用包裝物件（此API不需額外條件，傳空物件即可）。
        /// </param>
        /// <returns>
        /// 回傳以 <c>returnData.JsonSerializationt(true)</c> 序列化之 JSON 字串：
        /// - <c>Code = 200</c> 代表成功，<c>Data</c> 內為 <c>stockClass</c> 陣列。<br/>
        /// - <c>Code = -200</c> 代表失敗，錯誤原因見 <c>Result</c>。
        /// </returns>
        /// <remarks>
        /// <para>
        /// 功能說明：從資料表 <c>medMap_stock</c> 讀取全部資料，並以陣列回傳。<br/>
        /// 欄位說明（對應 <c>stockClass</c>）：<br/>
        /// - <c>GUID</c>：唯一識別碼。<br/>
        /// - <c>shelf_guid</c>：層架 GUID。<br/>
        /// - <c>location</c>：位置座標字串（如 <c>"0,1"</c>；允許空字串）。<br/>
        /// - <c>ip</c>：裝置 IP（允許空字串）。<br/>
        /// - <c>device_type</c>：裝置型別（例：<c>EPD290</c>；允許空字串）。<br/>
        /// - <c>led_index</c>：燈條亮燈位置，逗號分隔索引（如 <c>"145,162"</c>；允許空字串）。<br/>
        /// - <c>code</c>：藥碼。<br/>
        /// - <c>name</c>：藥名。<br/>
        /// - <c>material_no</c>：料號。<br/>
        /// - <c>lot</c>：批號（允許空字串）。<br/>
        /// - <c>expiry_date</c>：效期（格式 <c>yyyy-MM-dd HH:mm:ss</c>；可能為整點 <c>00:00:00</c>）。<br/>
        /// - <c>qty</c>：數量（字串型別；允許空字串）。<br/>
        /// </para>
        ///
        /// <para>
        /// 🔐 權限需求：無特別權限檢核（依現行程式碼）。<br/>
        /// ⚠️ 注意事項：若資料表不存在，例外訊息將被捕捉並以 <c>Code = -200</c> 回傳（同時內部會嘗試進行初始化）。<br/>
        /// </para>
        ///
        /// <para>
        /// 📨 <b>REQUEST DATA</b><br/>
        /// 傳入空物件：
        /// <code>
        /// {}
        /// </code>
        /// </para>
        ///
        /// <para>
        /// 🟩 <b>RESPONSE DATA（成功範例，Code = 200）</b>
        /// <code>
        /// {
        ///  "Data": [
        ///    {
        ///      "GUID": "0ab29de6-131c-452c-9f74-631a2e907e19",
        ///      "shelf_guid": "2745fed0-0584-4653-9c6a-834a347192bc",
        ///      "location": "",
        ///      "ip": "",
        ///      "device_type": "EPD290",
        ///      "led_index": "",
        ///      "code": "0280",
        ///      "name": "* ※ Adrenalin inj. 1mg/ml 1ml",
        ///      "material_no": "BOSI00",
        ///      "lot": "",
        ///      "expiry_date": "2025-10-08 04:01:01",
        ///      "qty": ""
        ///    },
        ///    {
        ///      "GUID": "251c1889-ba8d-458b-9e1a-11529fc3008b",
        ///      "shelf_guid": "88434dad-9a18-4994-b872-37a579a27b65",
        ///      "location": "0,1",
        ///      "ip": "",
        ///      "device_type": "EPD290",
        ///      "led_index": "",
        ///      "code": "IGLU3",
        ///      "name": "※ 50% GLUCOSE 50% 500ML",
        ///      "material_no": "G50I02",
        ///      "lot": "",
        ///      "expiry_date": "2025-10-09 00:00:00",
        ///      "qty": ""
        ///    }
        ///     ],
        ///  "Code": 200,
        ///  "Method": "get_medMap_stock",
        ///  "Result": "儲位寫入成功!",
        ///  "Value": "",
        ///  "ValueAry": [],
        ///  "TimeTaken": "56.629ms",
        ///  "Token": "",
        ///  "Server": "",
        ///  "DbName": "",
        ///  "TableName": "",
        ///  "Port": 0,
        ///  "UserName": "",
        ///  "Password": "",
        ///  "ServerType": "",
        ///  "ServerName": "",
        ///  "ServerContent": "",
        ///  "RequestUrl": ""
        /// }
        /// </code>
        /// </para>
        ///
        /// <para>
        /// 🟥 <b>RESPONSE DATA（失敗範例，Code = -200）</b><br/>
        /// <i>（實際訊息以例外狀況為準）</i>
        /// <code>
        ///{
        ///  "Data": null,
        ///  "Code": -200,
        ///  "Method": "get_medMap_stock",
        ///  "Result": "Table 'dbvm.medmap_stock' doesn't exist",
        ///  "Value": "",
        ///  "ValueAry": [],
        ///  "TimeTaken": "X.XXXms"
        ///}
        /// </code>
        /// </para>
        ///
        /// <para>
        /// ✅ 回傳碼：<br/>
        /// - <b>200</b>：取得成功。<br/>
        /// - <b>-200</b>：執行失敗（例外或資料表不存在等）。<br/>
        /// </para>
        /// </remarks>
        [HttpPost("get_stock")]
        public async Task<string> get_stock([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_medMap_stock = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);

                List<object[]> rows = await sQLControl_medMap_stock.GetAllRowsAsync(null);
                List<stockClass> medMap_StockClasses = rows.SQLToClass<stockClass>();
                
                medMap_StockClasses = get_stockInfo(medMap_StockClasses);

                returnData.Code = 200;
                returnData.Data = medMap_StockClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_medMap_stock";
                returnData.Result = $"儲位寫入成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Table 'dbvm.medmap_stock' doesn't exist") init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_stock_by_shelf_GUID")]
        public async Task<string> get_stock_by_shelf_GUID([FromBody] returnData returnData)
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
                if (returnData.ValueAry.Count != 1 || returnData.ValueAry[0].StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"GUID\"]";
                    return returnData.JsonSerializationt();
                }
                string[] shelf_GUID = returnData.ValueAry[0].Split(";").Distinct().ToArray();
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");

                SQLControl sQLControl = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medMap_stock.shelf_GUID, shelf_GUID);
                List<stockClass> medMap_StockClasses = objects.SQLToClass<stockClass>();
                medMap_StockClasses = get_stockInfo(medMap_StockClasses);

                returnData.Code = 200;
                returnData.Data = medMap_StockClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_stock_by_shelf_GUID";
                returnData.Result = $"取得儲位資料成功，共{medMap_StockClasses.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("delete_stock_by_shelf_GUID")]
        public async Task<string> delete_stock_by_shelf_GUID([FromBody] returnData returnData)
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
                if (returnData.ValueAry.Count != 1 || returnData.ValueAry[0].StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"GUID\"]";
                    return returnData.JsonSerializationt();
                }
                string[] shelf_GUID = returnData.ValueAry[0].Split(";").Distinct().ToArray();
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");

                SQLControl sQLControl = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);
                List<object[]> delete = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medMap_stock.shelf_GUID, shelf_GUID);
                if (delete.Count > 0) await sQLControl.DeleteRowsAsync(null, delete);

                returnData.Code = 200;
                returnData.Data = null;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "delete_stock_by_shelf_GUID";
                returnData.Result = $"刪除儲位資料成功，共{delete.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_available_shelves")]
        public async Task<string> get_available_shelves([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry須為[\"GUID\"]";
                    return returnData.JsonSerializationt();
                }
                returnData returnData_sub_content = await new inspectionController().sub_contents_get_by_GUID(returnData.ValueAry[0]);
                if (returnData_sub_content == null || returnData_sub_content.Code != 200)
                {
                    returnData.Code = -200;
                    returnData.Result = $"sub_content取得失敗";
                    return returnData.JsonSerializationt();
                }
                List<inspectionClass.sub_content> sub_contents = returnData_sub_content.Data.ObjToClass<List<inspectionClass.sub_content>>();
                if (sub_contents == null || sub_contents.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"sub_content取得失敗";
                    return returnData.JsonSerializationt();
                }
                string code = sub_contents[0].藥品碼;
                returnData returnData_medSize = await new medSize().get_by_code(code);
                if (returnData_medSize == null || returnData_medSize.Code != 200)
                {
                    returnData.Code = -200;
                    returnData.Result = $"medsize取得失敗";
                    return returnData.JsonSerializationt();
                }
                List<medSizeClass> medSizeClasses = returnData_medSize.Data.ObjToClass<List<medSizeClass>>();
                if (medSizeClasses == null || medSizeClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"medsize取得失敗";
                    return returnData.JsonSerializationt();
                }



                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        [HttpPost("get_med_by_code_name_type")]
        public async Task<string> get_med_by_code_name_type([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 3)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry須為[\"設備名稱\", \"類別\", \"藥碼\"]";
                    return returnData.JsonSerializationt();
                }
                string 藥碼 = returnData.ValueAry[2];
                returnData returnData_get_medMap_by_name_type = await get_medMap_by_name_type(returnData.ValueAry[0], returnData.ValueAry[1]);
                medMapClass medMapClass = returnData_get_medMap_by_name_type.Data.ObjToClass<medMapClass>();
                List<medMap_shelfClass> medMap_ShelfClasses = new List<medMap_shelfClass>();
                foreach (var item in medMapClass.medMap_Section)
                {
                    if (item.sub_section == null || item.sub_section.Count == 0) continue;
                    foreach (var sub_section in item.sub_section)
                    {
                        if (sub_section.shelf == null || sub_section.shelf.Count == 0) continue;
                        foreach (var shelf in sub_section.shelf)
                        {
                            List<stockClass> medMap_Stocks_buff = shelf.medMapStock.Where(x => x.藥碼 == 藥碼).ToList();
                            shelf.medMapStock = medMap_Stocks_buff;
                            if (medMap_Stocks_buff.Count > 0) medMap_ShelfClasses.Add(shelf);
                        }

                    }
                }

                returnData.Data = medMap_ShelfClasses;
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 控制指定藥碼對應裝置之亮燈行為。
        /// 依藥碼查詢對應的層架與庫存位置，
        /// 系統會根據每個位置自動產生亮燈指令，
        /// 並依裝置類型 (RowsLED / EPD) 執行亮燈控制。
        /// </summary>
        /// <remarks>
        /// 一、<b>API 說明</b><br/>
        /// 此 API 用於依藥碼 (<c>code</c>) 查找對應的燈條或電子標籤裝置，  
        /// 並自動發送亮燈指令以輔助人員定位藥品存放位置。  
        /// 支援以下裝置類型：  
        /// - <b>RowsLED</b>：控制燈條中指定範圍 (start_num ~ end_num) 的燈號顏色。  
        /// - <b>EPD 類型裝置</b>：控制整體亮燈顏色。  
        /// 系統會依裝置類型自動建立或快取對應的控制物件。<br/><br/>
        ///
        /// 二、<b>串接 URL</b><br/>
        /// /api/medMap/light_by_code_name_type<br/><br/>
        ///
        /// 三、<b>方法類別 (Method)</b><br/>
        /// POST<br/><br/>
        ///
        /// 四、<b>JSON 傳入範例</b><br/>
        /// <code>
        /// {
        ///   "Method": "light_by_code_name_type",
        ///   "ValueAry": [
        ///     "ServerName=MainDB",
        ///     "ServerType=MySQL",
        ///     "code=EPAR",
        ///     "color=255,0,0",
        ///     "lightness=0.9"
        ///   ],
        ///   "Data": {}
        /// }
        /// </code><br/>
        ///
        /// 五、<b>JSON 回傳範例 (成功回傳)</b><br/>
        /// <code>
        /// [
        ///   {
        ///     "Code": 200,
        ///     "Method": "light_action",
        ///     "Result": "裝置觸發成功 (IP:192.168.1.50)",
        ///     "TimeTaken": "35ms"
        ///   },
        ///   {
        ///     "Code": 200,
        ///     "Method": "light_action",
        ///     "Result": "裝置觸發成功 (IP:192.168.1.51)",
        ///     "TimeTaken": "33ms"
        ///   }
        /// ]
        /// </code><br/>
        ///
        /// 六、<b>JSON 錯誤回傳範例 (失敗回傳)</b><br/>
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "returnData.ValueAry錯誤"
        /// }
        /// </code>
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "Exception : 連線逾時"
        /// }
        /// </code><br/>
        ///
        /// 七、<b>輸入資料來源與參數說明</b><br/>
        /// - <b>ServerName</b>：資料庫伺服器名稱 (必填)。<br/>
        /// - <b>ServerType</b>：資料庫伺服器類型 (MySQL、Oracle...，必填)。<br/>
        /// - <b>code</b>：藥碼，用於查詢層架與燈號資訊 (必填)。<br/>
        /// - <b>color</b>：RGB 顏色字串 (格式 "R,G,B"，例如 "255,0,0" 代表紅色，必填)。<br/>
        /// - <b>lightness</b>：亮度設定 (0~1，必填)。<br/>
        ///
        /// 八、<b>系統行為</b><br/>
        /// 1. 依藥碼查詢對應之層架與燈條資料。<br/>
        /// 2. 自動組出每個位置對應的控制命令字串：<br/>
        ///    <c>ip=&lt;IP&gt;;start_num=&lt;起點&gt;;end_num=&lt;終點&gt;;color=&lt;顏色&gt;;lightness=&lt;亮度&gt;;device_type=&lt;裝置類型&gt;</c><br/>
        /// 3. 並行呼叫 <c>/api/device_control/light_action</c>，批次控制所有裝置。<br/>
        /// 4. 回傳所有控制結果陣列 (每個裝置對應一筆回傳)。<br/><br/>
        ///
        /// 九、<b>裝置類型說明</b><br/>
        /// - RowsLED：分段控制指定範圍亮燈。<br/>
        /// - EPDXXXX：整體亮燈控制。<br/><br/>
        ///
        /// 十、<b>前端建議實作</b><br/>
        /// 前端應提供可調整輸入參數介面，包括：  
        /// IP、顏色 (RGB 選擇器)、亮度 (0~1)、控制範圍 (start_num / end_num)、裝置類型 (RowsLED / EPD)。  
        /// 呼叫完成後顯示回傳結果 (成功或錯誤訊息)。<br/><br/>
        ///
        /// 十一、<b>注意事項</b><br/>
        /// - 若查無資料或參數不完整，系統將回傳 Code = -200。<br/>
        /// - 若任何裝置觸發失敗，會顯示具體錯誤訊息 (例如連線逾時或 IP 無效)。<br/>
        /// - 所有裝置呼叫皆為非同步並行執行。<br/>
        /// </remarks>
        [HttpPost("light_by_code_name_type")]
        public async Task<string> light_by_code_name_type([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                string GetVal(string key) =>
                   returnData.ValueAry.FirstOrDefault(x => x.StartsWith($"{key}=", StringComparison.OrdinalIgnoreCase))
                    ?.Split('=')[1];
                string ServerName = GetVal("ServerName") ?? "";
                string ServerType = GetVal("ServerType") ?? "";
                string code = GetVal("code") ?? "";
                string color = GetVal("color") ?? "";
                string lightness = GetVal("lightness") ?? "";

                if (ServerName.StringIsEmpty() || ServerType.StringIsEmpty()|| code.StringIsEmpty() || color.StringIsEmpty()
                    || lightness.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry錯誤";
                    return returnData.JsonSerializationt();
                }
                
                returnData returnData_get_med_by_code_name_type = await get_med_by_code_name_type(ServerName, ServerType, code);
                
                List<medMap_shelfClass> medMap_ShelfClasses = returnData_get_med_by_code_name_type.Data.ObjToClass<List<medMap_shelfClass>>();
                List<string> list_light = new List<string>();
                if (medMap_ShelfClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此藥碼:{code}";
                    return returnData.JsonSerializationt();
                }
                foreach (var item in medMap_ShelfClasses)
                {
                    string shelf_ip = item.燈條IP;
                    string shelf_device_type = item.device_type;
                    string stock_ip = string.Empty;
                    string stock_device_type = string.Empty;
                    string start = string.Empty;
                    string end = string.Empty;
                    if (item.medMapStock != null)
                    {
                        foreach (var med in item.medMapStock)
                        {
                            stock_ip = med.IP;
                            stock_device_type = med.device_type;
                            start = med.燈條亮燈位置.Split(",")[0];
                            end = med.燈條亮燈位置.Split(",").Last();
                            string command = $"ip={shelf_ip};start_num={start};end_num={end};color={color};lightness={lightness};device_type={shelf_device_type}";
                            string command_1 = $"ip={stock_ip};start_num={start};end_num={end};color={color};lightness={lightness};device_type={stock_device_type}";
                            list_light.Add(command);
                            list_light.Add(command_1);
                        }
                    }                  
                }
                List<Task<returnData>> tasks = new List<Task<returnData>>();
                foreach (var light_ in list_light)
                {
                    List<string> command_arry = light_.Split(";").ToList();
                    Task<returnData> task  = deviceApiClass.light_action(API_server, command_arry);
                    tasks.Add(task);
                }
                returnData[] results = await Task.WhenAll(tasks);
                return results.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }

        }
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
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medMap()));
            tables.Add(MethodClass.CheckCreatTable<medMap_sectionClass>(sys_serverSettingClasses[0]));
            //tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medMap_section()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medMap_sub_section()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medMap_shelf()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medMap_drawer()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medMap_box()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_medSize()));
            tables.Add(MethodClass.CheckCreatTable<stockClass>(sys_serverSettingClasses[0]));

            return tables.JsonSerializationt(true);
        }
        private async Task<returnData> get_medMap_section_by_Master_GUID(string Master_GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(Master_GUID);
            string result = await get_medMap_section_by_Master_GUID(returnData);
            return result.JsonDeserializet<returnData>();
        }
        private async Task<string> get_medMap_sub_section_by_Master_GUID(string Master_GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(Master_GUID);
            return await get_medMap_sub_section_by_Master_GUID(returnData);
        }
        private async Task<returnData> get_medMap_shelf_by_Master_GUID(string Master_GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(Master_GUID);
            string result = await get_medMap_shelf_by_Master_GUID(returnData);
            return result.JsonDeserializet<returnData>();
        }
        private string get_medMap_drawer_by_Master_GUID(string Master_GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(Master_GUID);
            return get_medMap_drawer_by_Master_GUID(returnData);
        }
        private async Task<returnData> get_medMap_box_by_Master_GUID(string Master_GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(Master_GUID);
            string result = await get_medMap_box_by_Master_GUID(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }
        private async Task<returnData> get_medMap_by_name_type(string serverName, string serverType)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(serverName);
            returnData.ValueAry.Add(serverType);
            string result = await get_medMap_by_name_type(returnData);
            return result.JsonDeserializet<returnData>();
        }
        private async Task<returnData> get_stock_by_shelf_GUID(string shelf_GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(shelf_GUID);
            string result = await get_stock_by_shelf_GUID(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }
        private async Task<returnData> get_sub_section_by_GUID(string GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(GUID);
            string result = await get_sub_section_by_GUID(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }
        private async Task<returnData> delete_box_by_Master_GUID(string master_guid)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(master_guid);
            string result = await delete_box_by_Master_GUID(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }
        private async Task<returnData> get_section_by_GUID(string guid)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(guid);
            string result = await get_section_by_GUID(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_section_by_IP(string IP)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(IP);
            string result = await get_section_by_IP(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }
        private async Task<returnData> get_med_by_code_name_type(string ServerName, string ServerType, string code)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(ServerName);
            returnData.ValueAry.Add(ServerType);
            returnData.ValueAry.Add(code);
            string result = await get_med_by_code_name_type(returnData);
            return await result.JsonDeserializetAsync<returnData>();
        }

        private List<stockClass> get_stockInfo(List<stockClass> medMap_stockClasses)
        {
            foreach (var stock in medMap_stockClasses)
            {
                string value = stock.Value;
                if (value.StringIsEmpty()) value = new DeviceBasic().JsonSerializationt();
                DeviceBasic deviceBasic = value.JsonDeserializet<DeviceBasic>();
                stock.效期 = deviceBasic.List_Validity_period;
                stock.數量 = deviceBasic.List_Inventory;
                stock.批號 = deviceBasic.List_Lot_number;
            }
            return medMap_stockClasses;
        }

    }

}
