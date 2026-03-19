using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using MyUI;
using NPOI.SS.Formula.Functions;
using SQLUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_系統
{
    [Route("api/[controller]")]
    [ApiController]
    public class temperature : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(200, "溫度物件", typeof(temperatureClass))]

        /// <summary>
        ///初始化溫度資料庫
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
        public string init()
        {
            returnData returnData = new returnData();
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
        /// 新增溫度資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///         "GUID":
        ///         "IP"
        ///         "name": "",
        ///         "temp":"",
        ///         "humidity":"",
        ///         "add_time":""
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
        [HttpPost("add")]
        public string add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if(returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data資料錯誤!";
                    return returnData.JsonSerializationt();
                }
                temperatureClass temperatureClass = returnData.Data.ObjToClass<temperatureClass>();
                if(temperatureClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data資料錯誤，須為 {temperatureClass}!";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");               

                SQLControl sQLControl = new SQLControl(Server, DB, "temperature", UserName, Password, Port, SSLMode);

                temperatureClass.GUID = Guid.NewGuid().ToString();
                if (temperatureClass.IP.StringIsEmpty()) Logger.Log("temperature", $"IP資料為空 \n{returnData.JsonSerializationt(true)}");
                if (temperatureClass.溫度.StringIsEmpty()) Logger.Log("temperature", $"溫度資料為空 \n{returnData.JsonSerializationt(true)}");
                if (temperatureClass.濕度.StringIsEmpty()) Logger.Log("temperature", $"濕度資料為空 \n{returnData.JsonSerializationt(true)}");
                if (temperatureClass.新增時間.StringIsEmpty()) temperatureClass.新增時間 = DateTime.Now.ToDateTimeString();

                object[] add = temperatureClass.ClassToSQL<temperatureClass, enum_temperature>();
                sQLControl.AddRow(null, add);

                returnData.Code = 200;
                returnData.Data = temperatureClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add";
                returnData.Result = $"資料寫入成功!";
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
        /// <summary>
        /// 以別名和新增時間取得資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     },
        ///     "Value": "",
        ///     "ValueAry":["開始時間","結束時間"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_temp_by_time")]
        public async Task<string> get_temp_by_time([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                List<Task> tasks = new List<Task>();
                List<temperature_setClass> temperature_SetClasses = new List<temperature_setClass>();
                returnData returnData_get_set = new returnData();
                tasks.Add(Task.Run(new Action(delegate
                {
                    //取得設定
                    (returnData_get_set, temperature_SetClasses) = get_set();
                })));
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry不得為空";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"開始時間,\"結束時間\"]";
                    return returnData.JsonSerializationt();
                }
                string 開始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];
                if (開始時間.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"開始時間格式錯誤";
                    return returnData.JsonSerializationt();
                }
                if (結束時間.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"結束時間格式錯誤";
                    return returnData.JsonSerializationt();
                }
                
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

                SQLControl sQLControl = new SQLControl(Server, DB, "temperature", UserName, Password, Port, SSLMode);
                string command = $"SELECT * FROM {DB}.temperature WHERE 新增時間 BETWEEN '{開始時間}' AND '{結束時間}';";
                DataTable dataTable = sQLControl.WtrteCommandAndExecuteReader(command);
                List<object[]> objects = dataTable.DataTableToRowList();
                List<temperatureClass> temperatureClasses = objects.SQLToClass<temperatureClass, enum_temperature>();
                List<List<temperatureClass>> list_temperatureClass = GroupOrders(temperatureClasses);
                List<temperature_setClass> temperature_SetClass_buff = new List<temperature_setClass>();
                Task.WhenAll(tasks).Wait();
                if(temperature_SetClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"取得設定失敗, {returnData_get_set.Result}";
                    return returnData.JsonSerializationt(true);
                }
                for(int i = 0; i < list_temperatureClass.Count(); i++)
                {
                    List<temperatureClass> temperatureClass_buff = list_temperatureClass[i];
                    temperatureClass_buff.Sort(new temperatureClass.ICP_By_time());

                    temperature_SetClass_buff = temperature_SetClasses.Where(set => set.IP == temperatureClass_buff[0].IP).ToList();
                    temperature_SetClass_buff[0].temperatureClasses = temperatureClass_buff;
                }
                temperature_SetClasses = temperature_SetClasses.Where(item => item.temperatureClasses != null).ToList();
                returnData.Code = 200;
                returnData.Data = temperature_SetClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_temp_by_time";
                returnData.Result = $"取得{開始時間}~{結束時間}溫度資料成功，共{temperatureClasses.Count()}!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Table 'dbvm.temperature' doesn't exist") init();
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得最新資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
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
        [HttpPost("get_latest_today")]
        public string get_latest_today([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                List<Task> tasks = new List<Task>();
                List<temperature_setClass> temperature_SetClasses = new List<temperature_setClass>();
                returnData returnData_get_set = new returnData();
                tasks.Add(Task.Run(new Action(delegate
                {
                    (returnData_get_set,temperature_SetClasses) = get_set();
                })));
                DateTime now = DateTime.Now;
                string 開始時間 = now.GetStartDate().ToDateTimeString();
                string 結束時間 = now.GetEndDate().ToDateTimeString();

                
                if (開始時間.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"開始時間格式錯誤";
                    return returnData.JsonSerializationt();
                }
                if (結束時間.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"結束時間格式錯誤";
                    return returnData.JsonSerializationt();
                }

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

                SQLControl sQLControl = new SQLControl(Server, DB, "temperature", UserName, Password, Port, SSLMode);
                string command = $"SELECT * FROM {DB}.temperature WHERE 新增時間 BETWEEN '{開始時間}' AND '{結束時間}';";
                DataTable dataTable = sQLControl.WtrteCommandAndExecuteReader(command);
                List<object[]> objects = dataTable.DataTableToRowList();
                List<temperatureClass> temperatureClasses = objects.SQLToClass<temperatureClass, enum_temperature>();
                List<temperatureClass> temperatureClasses_buff = temperatureClasses
                    .GroupBy(g => g.IP)
                    .Select(grouped =>
                    {
                        temperatureClass temperatureClass = grouped.OrderByDescending(item => item.新增時間).First();
                        return temperatureClass;
                    }).ToList();
                
                Task.WhenAll(tasks).Wait();
                if (temperature_SetClasses == null || temperature_SetClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"取得設定失敗: {returnData_get_set.Result}";
                    return returnData.JsonSerializationt(true);
                }
                for (int i = 0; i < temperature_SetClasses.Count(); i++)
                {
                    List<temperatureClass> temperatures = temperatureClasses_buff.Where(item => item.IP == temperature_SetClasses[i].IP).ToList();
                    temperature_SetClasses[i].temperatureClasses = temperatures;
                }
                temperature_SetClasses = temperature_SetClasses.Where(item => item.temperatureClasses.Count != 0).ToList();
                returnData.Code = 200;
                returnData.Data = temperature_SetClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_latest_today";
                returnData.Result = $"取得最新溫度資料成功，共{temperature_SetClasses.Count()}!";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                if (ex.Message == "Table 'dbvm.temperature' doesn't exist") init();
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 新增設定資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///         "GUID":
        ///         "IP"
        ///         "name": "",
        ///         "temp_max":"",
        ///         "temp_min":"",
        ///         "humidity_max":"",
        ///         "humidity_min":"",
        ///         "alert":"",
        ///         "mail":"",   
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
        [HttpPost("add_set")]
        public string add_set([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                init();
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data資料錯誤!";
                    return returnData.JsonSerializationt();
                }
                temperature_setClass temperature_setClass = returnData.Data.ObjToClass<temperature_setClass>();
                if (temperature_setClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data資料錯誤，須為 {temperature_setClass}!";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

                SQLControl sQLControl = new SQLControl(Server, DB, "temperature_set", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl.GetRowsByDefult(null, (int)enum_temperature_set.IP, temperature_setClass.IP);
                if(objects.Count > 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"此IP:{temperature_setClass.IP}已存在設定";
                    return returnData.JsonSerializationt(true);
                }

                temperature_setClass.GUID = Guid.NewGuid().ToString();
                if (temperature_setClass.警報.StringIsEmpty() == false) temperature_setClass.警報 = temperature_setClass.警報.StringToBool().ToString();
                if (temperature_setClass.發信.StringIsEmpty() == false) temperature_setClass.發信 = temperature_setClass.發信.StringToBool().ToString();

                object[] add = temperature_setClass.ClassToSQL<temperature_setClass, enum_temperature_set>();
                sQLControl.AddRow(null, add);

                returnData.Code = 200;
                returnData.Data = temperature_setClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_set";
                returnData.Result = $"資料寫入成功!";
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
        /// <summary>
        /// 更新溫度資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///         "GUID":
        ///         "IP"
        ///         "name": "",
        ///         "temp_max":"",
        ///         "temp_min":"",
        ///         "humidity_max":"",
        ///         "humidity_min":"",
        ///         "alert":"",
        ///         "mail":""  
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
        [HttpPost("update_set")]
        public string update_set([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                List<Task> tasks = new List<Task>();
                List<temperature_setClass> temperature_SetClasses = new List<temperature_setClass>();
                returnData returnData_get_set = new returnData();
                tasks.Add(Task.Run(new Action(delegate
                {
                    (returnData_get_set, temperature_SetClasses) = get_set();
                })));
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data資料錯誤!";
                    return returnData.JsonSerializationt();
                }
                List<temperature_setClass> input_temperature_setClass = returnData.Data.ObjToClass<List<temperature_setClass>>();
                if (input_temperature_setClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data資料錯誤，須為 [{temperature_setClass}]!";
                    return returnData.JsonSerializationt();
                }
                
                Task.WhenAll(tasks).Wait();
                if (temperature_SetClasses == null || temperature_SetClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"取得設定失敗: {returnData_get_set.Result}";
                    return returnData.JsonSerializationt(true);
                }
                List<temperature_setClass> update_temperature_setClass = new List<temperature_setClass>();
                for (int i =0; i < input_temperature_setClass.Count; i++)
                {
                    temperature_setClass temperature_set_Class_buff = temperature_SetClasses.Where(item => item.GUID == input_temperature_setClass[i].GUID).FirstOrDefault();
                    if (temperature_set_Class_buff == null) continue;
                    if (input_temperature_setClass[i].IP.StringIsEmpty() == false) temperature_set_Class_buff.IP = input_temperature_setClass[i].IP;
                    if (input_temperature_setClass[i].別名.StringIsEmpty() == false) temperature_set_Class_buff.別名 = input_temperature_setClass[i].別名;
                    if (input_temperature_setClass[i].溫度上限.StringIsEmpty() == false) temperature_set_Class_buff.溫度上限 = input_temperature_setClass[i].溫度上限;
                    if (input_temperature_setClass[i].溫度下限.StringIsEmpty() == false) temperature_set_Class_buff.溫度下限 = input_temperature_setClass[i].溫度下限;
                    if (input_temperature_setClass[i].溫度補償.StringIsEmpty() == false) temperature_set_Class_buff.溫度補償 = input_temperature_setClass[i].溫度補償;
                    if (input_temperature_setClass[i].濕度上限.StringIsEmpty() == false) temperature_set_Class_buff.濕度上限 = input_temperature_setClass[i].濕度上限;
                    if (input_temperature_setClass[i].濕度下限.StringIsEmpty() == false) temperature_set_Class_buff.濕度下限 = input_temperature_setClass[i].濕度下限;
                    if (input_temperature_setClass[i].濕度補償.StringIsEmpty() == false) temperature_set_Class_buff.濕度補償 = input_temperature_setClass[i].濕度補償;
                    if (input_temperature_setClass[i].警報.StringIsEmpty() == false) temperature_set_Class_buff.警報 = input_temperature_setClass[i].警報.StringToBool().ToString();
                    if (input_temperature_setClass[i].發信.StringIsEmpty() == false) temperature_set_Class_buff.發信 = input_temperature_setClass[i].發信.StringToBool().ToString();
                    update_temperature_setClass.Add(temperature_set_Class_buff);

                }

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "temperature_set", UserName, Password, Port, SSLMode);
                List<object[]> update = update_temperature_setClass.ClassToSQL<temperature_setClass, enum_temperature_set>();
                sQLControl.UpdateByDefulteExtra(null, update);

                returnData.Code = 200;
                returnData.Data = update_temperature_setClass;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_set";
                returnData.Result = $"資料更新，共{update_temperature_setClass.Count}筆!";
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
        /// <summary>
        /// 刪除設定資料
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
        [HttpPost("delete_set")]
        public string delete_set([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                init();
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count() != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data資料錯誤，須為[\"GUID\"]!";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                string GUID = returnData.ValueAry[0];
                SQLControl sQLControl = new SQLControl(Server, DB, "temperature_set", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl.GetRowsByDefult(null, (int)enum_temperature_set.GUID, GUID);
                if (objects.Count > 0) sQLControl.DeleteExtra(null, objects);

                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_set";
                returnData.Result = $"資料{GUID}刪除成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                init();
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新溫度資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
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
        [HttpPost("get_set")]
        public string get_set([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
              
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

                SQLControl sQLControl = new SQLControl(Server, DB, "temperature_set", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl.GetAllRows(null);
                List<temperature_setClass> temperature_SetClasses = objects.SQLToClass<temperature_setClass, enum_temperature_set>();
       
                returnData.Code = 200;
                returnData.Data = temperature_SetClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_set";
                returnData.Result = $"資料寫入成功!";
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
        [HttpPost("get_datas_sheet")]
        public async Task<string> get_datas_sheet([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_datas_sheet";
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
                    returnData.Result = $"returnData.ValueAry資料錯誤，須為 [\"開始時間,\"結束時間\"]";
                    return returnData.JsonSerializationt();
                }

                string 開始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];


                returnData returnData_get_temp = await get_temp_by_time(開始時間, 結束時間);
                if (returnData_get_temp == null || returnData_get_temp.Code != 200)
                {
                    returnData.Code = -200;
                    returnData.Result = $"取得溫度資料失敗";
                    return returnData.JsonSerializationt();
                }
                List<temperature_setClass> temperature_SetClasses = returnData_get_temp.Data.ObjToClass<List<temperature_setClass>>();

                List<SheetClass> sheetClasses = new List<SheetClass>();
                //List<List<transactionsClass>> list_transactionsClasses = new List<List<transactionsClass>>();

                for (int k = 0; k < temperature_SetClasses.Count; k++)
                {
                    temperature_setClass  temperature_SetClass = temperature_SetClasses[k];
                    List<temperatureClass> temperatureClasses = temperature_SetClass.temperatureClasses;
                    List<temperatureClass> temperatures_buff = temperatureClasses
                        .GroupBy(x => x.新增時間.StringToDateTime().Date)
                        .SelectMany(g =>
                        {
                            var temperatureClass_9 = g
                                .Where(x => x.新增時間.StringToDateTime().Hour == 9)
                                .OrderBy(x => x.新增時間.StringToDateTime())
                                .FirstOrDefault();

                            var temperatureClass_17 = g
                                .Where(x => x.新增時間.StringToDateTime().Hour == 17)
                                .OrderBy(x => x.新增時間.StringToDateTime())
                                .FirstOrDefault();

                            return new[] { temperatureClass_9, temperatureClass_17 }
                                   .Where(x => x != null);
                        }).ToList();

                    temperatures_buff = temperatures_buff.OrderBy(x => x.新增時間.StringToDateTime()).ToList();

                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "溫度記錄表.xlsx");
                    string loadText = MyOffice.ExcelClass.NPOI_LoadSheetsToJson_PreserveStyle(path);
                    List<SheetClass> sheetClasslist = loadText.JsonDeserializet<List<SheetClass>>();
                    SheetClass sheetClass = sheetClasslist[0];

                    //Logger.Log("excel_emg_tradding", $"{sheetClass.JsonSerializationt(true)}");

                    
                    

                    sheetClass.Name = $"{temperature_SetClass.別名}";
                    string year = temperatures_buff[0].新增時間.StringToDateTime().Year.ToString();
                    string month = temperatures_buff[0].新增時間.StringToDateTime().Month.ToString();
                    sheetClass.Rows[2].Cell[57].Text = $"{year}年{month}月";
                    sheetClass.Rows[0].Cell[57].Text = $"{temperature_SetClass.別名}";
                    Logger.Log(temperatures_buff.JsonSerializationt(true));
                    for (int i = 0; i < temperatures_buff.Count; i++)
                    {
                        double 溫度 = temperatures_buff[i].溫度.StringToDouble();
                        溫度 = Math.Round(溫度 * 2, MidpointRounding.AwayFromZero) / 2.0;
                        int rows_溫度 = (int)Math.Round(57 - 2 * 溫度); //根據EXCEL算出來的位置

                        double 濕度 = temperatures_buff[i].濕度.StringToDouble();
                        濕度 = Math.Round(濕度 * 2, MidpointRounding.AwayFromZero) / 2.0;
                        int rows_濕度 = (int)Math.Round(125 - 2 * 濕度); //根據EXCEL算出來的位置

                        int day = temperatures_buff[i].新增時間.StringToDateTime().Day;
                        int hour = temperatures_buff[i].新增時間.StringToDateTime().Hour;
                        int cell = 0;
                        if (hour < 12) 
                        {
                            cell = 2 * day - 1;
                        }
                        else
                        {
                            cell = 2 * day;
                        }
                        if (rows_溫度 > 0 && rows_溫度 <= 53 && cell > 0 && cell <= 63) sheetClass.Rows[rows_溫度].Cell[cell].Text = "★";
                        if (rows_濕度 > 0 && rows_濕度 <= 53 && cell > 0 && cell <= 63) 
                        {
                            if (sheetClass.Rows[rows_濕度].Cell[cell].Text.StringIsEmpty()==false)
                            {
                                sheetClass.Rows[rows_濕度].Cell[cell].Text = "○";
                            }
                            else
                            {
                                sheetClass.Rows[rows_濕度].Cell[cell].Text = "●";
                            }
                        }
                        

                    }
                    //sheetClass.Rows[1].Cell[1].Text = $"{medClasses_buf[0].藥品碼}";
                    //sheetClass.Rows[1].Cell[4].Text = $"{medClasses_buf[0].包裝單位}";
                    //sheetClass.Rows[1].Cell[8].Text = $"{medClasses_buf[0].管制級別}";
                    //sheetClass.Rows[1].Cell[11].Text = $"{起始時間}";
                    //sheetClass.Rows[2].Cell[1].Text = $"{medClasses_buf[0].藥品名稱}";
                    //sheetClass.Rows[2].Cell[8].Text = $"{medClasses_buf[0].廠牌}";

                    //sheetClass.Rows[2].Cell[11].Text = $"{結束時間}";

                    //sheetClass.Rows[3].Cell[1].Text = $"{medClasses_buf[0].藥品學名}";
                    //sheetClass.Rows[3].Cell[8].Text = $"{medClasses_buf[0].藥品許可證號}";

                    //sheetClass.Rows[4].Cell[2].Text = $"{medClasses_buf[0].供貨廠商}";
                    //sheetClass.Rows[4].Cell[8].Text = $"{medClasses_buf[0].供貨商證字號}";

                    sheetClasses.Add(sheetClass);
                                     

                }

                returnData.Code = 200;
                returnData.Result = "Sheet取得成功!";
                returnData.Data = sheetClasses;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 取得收支結存報表(Excel)(多台合併)
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "藥碼1,藥碼2,藥碼",
        ///       "起始時間",
        ///       "結束時間",
        ///       "口服1,口服2",
        ///       "調劑台,調劑台"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為交易紀錄結構</returns>
        [Route("download_datas_excel")]
        [HttpPost]
        public async Task<ActionResult> download_datas_excel([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                myTimerBasic.StartTickTime(50000);
                string result = await get_datas_sheet(returnData);
                returnData = result.JsonDeserializet<returnData>();
                if (returnData.Code != 200)
                {
                    return null;
                }
                string jsondata = returnData.Data.JsonSerializationt();

                List<SheetClass> sheetClasses = jsondata.JsonDeserializet<List<SheetClass>>();
                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";

                byte[] excelData = sheetClasses.NPOI_GetBytes(Excel_Type.xlsx);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_收支結存簿冊.xlsx"));
            }
            catch
            {
                return null;
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
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_temperature()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_temperature_set()));

            return tables.JsonSerializationt(true);
        }
     
        public static List<List<temperatureClass>> GroupOrders(List<temperatureClass> temperatureClasses)
        {
            List<List<temperatureClass>> groupedtemperature = temperatureClasses
                .GroupBy(o => new { o.IP})
                .Select(group => group.ToList())
                .ToList();

            return groupedtemperature;
        }
        (returnData, List<temperature_setClass>) get_set()
        {
            returnData returnData_get_set = new returnData();
            string result = get_set(returnData_get_set);
            returnData_get_set = result.JsonDeserializet<returnData>();
            if (returnData_get_set.Code != 200 || returnData_get_set.Data == null) return (returnData_get_set,null);
            List<temperature_setClass> temperature_SetClasses = returnData_get_set.Data.ObjToClass<List<temperature_setClass>>();
            return (returnData_get_set,temperature_SetClasses);
        }
        public async Task<returnData> get_temp_by_time(string start_time, string end_time)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(start_time);
            returnData.ValueAry.Add(end_time);
            string result = await get_temp_by_time(returnData);
            return result.JsonDeserializet<returnData>();
        }

    }
}
    
