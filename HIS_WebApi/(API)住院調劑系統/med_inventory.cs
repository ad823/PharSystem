using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class med_inventory : ControllerBase
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        /// <summary>
        ///初始化dbvm.med_inventory資料庫
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
        [HttpPost("init_med_inventory_log")]
        public string init_med_inventory_log([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                return CheckCreatTable(sys_serverSettingClasses[0], new enum_med_inventory_log());
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        ///初始化dbvm.med_inventory資料庫
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
        [HttpPost("init_med_inventory")]
        public string init_med_inventory([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "med_inventory/init_med_inventory";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                return CheckCreatTable(sys_serverSettingClasses[0], new enum_med_inventory());
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        ///新增藥品調劑/覆核LOG
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "Data":[
        ///         {
        ///         "op_id":"",
        ///         "op_name":""
        ///         }]
        ///         "Value":"調劑"or"覆核 or "取消調劑"
        ///         "ValueAry":["處方GUID;處方GUID"]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add_med_inventory_log")]
        public string add_med_inventory_log([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "med_inventory/add_med_inventory_log";
            try
            {
                (string Server, string DB, string UserName, string Password, uint Port) = Method.GetServerInfo("Main", "網頁", "VM端");

                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data ";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[處方GUID]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.Value == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Value 內容應為\"調劑\" OR \"覆核\" OR \"取消調劑\" OR \"退藥\"";
                    return returnData.JsonSerializationt(true);
                }
                List<medInventoryLogClass> input_medInventoryLogClass = returnData.Data.ObjToClass<List<medInventoryLogClass>>();
                string 操作人ID = input_medInventoryLogClass[0].操作者代號;
                string 操作人姓名 = input_medInventoryLogClass[0].操作者姓名;

                string[] GUID = returnData.ValueAry[0].Split(";");
                DateTime today = DateTime.Now;
                string strat = today.GetStartDate().ToDateTimeString();
                string end = today.GetEndDate().ToDateTimeString();
                (string StartTime, string Endtime) = GetToday();


                SQLControl sQLControl_med_cpoe = new SQLControl(Server, DB, "med_cpoe", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_med_inventoryLog = new SQLControl(Server, DB, "med_inventory_log", UserName, Password, Port, SSLMode);
                List<object[]> list_med_cpoe = sQLControl_med_cpoe.GetRowsByBetween(null, (int)enum_med_cpoe.更新時間, StartTime, Endtime);

                List<object[]> list_med_inventoryLog = sQLControl_med_inventoryLog.GetRowsByBetween(null, (int)enum_med_inventory_log.操作時間, StartTime, Endtime);

                List<medCpoeClass> sql_medCpoe = list_med_cpoe.SQLToClass<medCpoeClass, enum_med_cpoe>();
                List<medInventoryLogClass> sql_med_inventoryLog = list_med_inventoryLog.SQLToClass<medInventoryLogClass, enum_med_inventory_log>();

                List<medCpoeClass> filterMedCpoe = sql_medCpoe.Where(data => GUID.Contains(data.GUID)).ToList();

                if (filterMedCpoe.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "無對應處方";
                    return returnData.JsonSerializationt(true);
                }
                List<medInventoryLogClass> add_medInventoryLogClass = new List<medInventoryLogClass>();
                List<medInventoryLogClass> update_medInventoryLogClass = new List<medInventoryLogClass>();

                foreach (var medCpoeClass in filterMedCpoe)
                {
                    medInventoryLogClass medInventoryLogClass = new medInventoryLogClass
                    {
                        GUID = Guid.NewGuid().ToString(),
                        Master_GUID = medCpoeClass.GUID,
                        操作行為 = returnData.Value,
                        藥局 = medCpoeClass.藥局,
                        護理站 = medCpoeClass.護理站,
                        床號 = medCpoeClass.床號,
                        操作者代號 = 操作人ID,
                        操作者姓名 = 操作人姓名,
                        操作時間 = DateTime.Now.ToDateTimeString(),
                        藥碼 = medCpoeClass.藥碼,
                        藥品名 = medCpoeClass.藥品名,
                        中文名 = medCpoeClass.中文名,
                        單位 = medCpoeClass.單位,
                        劑量 = medCpoeClass.劑量,
                        數量 = medCpoeClass.數量,
                    };
                    add_medInventoryLogClass.Add(medInventoryLogClass);                   
                }
                List<object[]> list_add_medInventoryLog = new List<object[]>();

                list_add_medInventoryLog = add_medInventoryLogClass.ClassToSQL<medInventoryLogClass, enum_med_inventory_log>();
                sQLControl_med_inventoryLog.AddRows(null, list_add_medInventoryLog);


                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"新增藥品處方{returnData.Value}LOG共{add_medInventoryLogClass.Count}筆";
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
        ///新增藥品初盤紀錄
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "Data":[medInventoryClass]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add_med_inventory")]
        public string add_med_inventory([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "med_inventory/add_med_inventory";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                List<medInventoryClass> input_medInven = returnData.Data.ObjToClass<List<medInventoryClass>>();
                if (input_medInven == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt(true);
                }  
                foreach(var medInventoryClass in input_medInven)
                {
                    medInventoryClass.GUID = Guid.NewGuid().ToString();
                }
                SQLControl sQLControl_med_inventoryLog = new SQLControl(Server, DB, "med_inventory", UserName, Password, Port, SSLMode);
                List<object[]> list_medInventoryLog = new List<object[]>();
                list_medInventoryLog = input_medInven.ClassToSQL<medInventoryClass, enum_med_inventory>();
                sQLControl_med_inventoryLog.AddRows(null, list_medInventoryLog);
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = input_medInven;
                returnData.Result = $"新增藥品處方調劑初盤共{input_medInven.Count}筆";
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
        ///以藥局、護理站和日期取得操作人姓名
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":[藥局, 護理站, 日期]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_opid_by_time")]
        public string get_opid_by_time([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "med_inventory/get_opid_by_time";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 3)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥局, 護理站, 日期]";
                    return returnData.JsonSerializationt(true);
                }
                string 藥局 = returnData.ValueAry[0];
                string 護理站 = returnData.ValueAry[1];
                DateTime 操作時間 = returnData.ValueAry[2].StringToDateTime();
                string dateStart = 操作時間.GetStartDate().ToDateTimeString();
                string dateEnd = 操作時間.GetEndDate().ToDateTimeString();
                List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_med_carInfo = new SQLControl(Server, DB, "med_inventory", UserName, Password, Port, SSLMode);
                List<object[]> list_pat_carInfo = sQLControl_med_carInfo.GetRowsByBetween(null, (int)enum_med_inventory.操作時間, dateStart, dateEnd);
                List<medInventoryClass> sql_medInvent = list_pat_carInfo.SQLToClass<medInventoryClass, enum_med_inventory>();
                List<medInventoryClass> result = sql_medInvent
                    .Where(temp => temp.護理站 == 護理站 && temp.藥局 == 藥局)
                    .GroupBy(value => value.操作者代號)
                    .Select(group => group.First())
                    .ToList();
                returnData.Data = result;
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
             
                returnData.Result = $"取得{操作時間.ToDateString()} {藥局} {護理站} 的操作人員ID";
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
        ///以藥局、護理站和日期取得操作人姓名
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     "Data": [
        ///         {
        ///            "pharm":"UC02",
        ///            "nurnum":"C109",
        ///            "op_id": "HS001",
        ///            "op_time": "2024-08-26"
        ///         }
        ///      ]
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_time_by_op_id")]
        public string get_time_by_op_id([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "med_inventory/get_time_by_op_id";
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                List<medInventoryClass> medInventoryClass = returnData.Data.ObjToClass<List<medInventoryClass>>();
                string 藥局 = medInventoryClass[0].藥局;
                string 護理站 = medInventoryClass[0].護理站;
                string 操作者代號 = medInventoryClass[0].操作者代號;
                DateTime 操作時間 = medInventoryClass[0].操作時間.StringToDateTime();

                string dateStart = 操作時間.GetStartDate().ToDateTimeString();
                string dateEnd = 操作時間.GetEndDate().ToDateTimeString();
                List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_med_carInfo = new SQLControl(Server, DB, "med_inventory", UserName, Password, Port, SSLMode);
                List<object[]> list_pat_carInfo = sQLControl_med_carInfo.GetRowsByBetween(null, (int)enum_med_inventory.操作時間, dateStart, dateEnd);
                List<medInventoryClass> sql_medInvent = list_pat_carInfo.SQLToClass<medInventoryClass, enum_med_inventory>();
                List<medInventoryClass> result = sql_medInvent
                    .Where(temp => temp.護理站 == 護理站 && temp.藥局 == 藥局 && temp.操作者代號 == 操作者代號)
                    .ToList();

                foreach (var value in result)
                {
                    DateTime opTime = value.操作時間.StringToDateTime();
                    value.操作時間 = opTime.ToString("HH:mm:ss");
                }
                returnData.Data = result;
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";

                returnData.Result = $"取得{操作時間.ToDateString()} {藥局} {護理站} 的操作人員ID";
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
        ///以藥局、護理站、日期、操作者ID取得操作人LOG
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     "Data": [
        ///         {
        ///            
        ///         }
        ///      ],
        ///      "ValueAry":["op_id"]
        ///      
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_logtime_by_opid")]
        public string get_logtime_by_opid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "med_inventory/get_logtime_by_opid";
            try
            {
                
                if(returnData.ValueAry == null)
                {
                    returnData.Code= -200;
                    returnData.Result = "returnData.ValueAry為空";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.ValueAry須為[\"操作者代號\"]";
                    return returnData.JsonSerializationt(true);
                }
                string 操作者代號 = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = Method.GetServerInfo("Main", "網頁", "VM端");
                (string StartTime, string Endtime) = GetToday();

                SQLControl sQLControl_med_inventoryLog = new SQLControl(Server, DB, "med_inventory_log", UserName, Password, Port, SSLMode);

                string command = $"SELECT * FROM {DB}.med_inventory_log WHERE 操作時間 BETWEEN '{StartTime}' AND '{Endtime}' AND 操作者代號 = '{操作者代號}';";
                DataTable dataTable = sQLControl_med_inventoryLog.WtrteCommandAndExecuteReader(command);
                List<object[]> list_medInventoryLog = dataTable.DataTableToRowList();
                List<medInventoryLogClass> sql_medInventoryLog = list_medInventoryLog.SQLToClass<medInventoryLogClass, enum_med_inventory_log>();
                sql_medInventoryLog.Sort(new medInventoryLogClass.ICP_By_optime());
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = sql_medInventoryLog;
                returnData.Result = $"取得{操作者代號}操作紀錄，共{sql_medInventoryLog.Count}筆";
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
        ///以Master_GUID取得調劑紀錄
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     "ValueAry": ["Master_GUID"]
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_logtime_by_master_GUID")]
        public string get_logtime_by_master_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "med_inventory/get_logtime_by_master_GUID";
            try
            {

                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 應為[\"處方1GUID;處方2GUID;處方3GUID\"]";
                    return returnData.JsonSerializationt(true);
                }
                string[] Master_GUIDs = returnData.ValueAry[0].Split(";");
                string guidList = string.Join(",", Master_GUIDs.Select(guid => $"'{guid}'"));
                List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                SQLControl sQLControl_med_inventoryLog = new SQLControl(Server, DB, "med_inventory_log", UserName, Password, Port, SSLMode);

                string command = $"SELECT * FROM {DB}.med_inventory_log WHERE Master_GUID IN ({guidList});";
                DataTable dataTable = sQLControl_med_inventoryLog.WtrteCommandAndExecuteReader(command);
                List<object[]> list_medInventoryLog = dataTable.DataTableToRowList();
                List<medInventoryLogClass> sql_medInventoryLog = list_medInventoryLog.SQLToClass<medInventoryLogClass, enum_med_inventory_log>();              
                sql_medInventoryLog.Sort(new medInventoryLogClass.ICP_By_optime());
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = sql_medInventoryLog;
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass, Enum enumInstance)
        {
            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, enumInstance);
            return table.JsonSerializationt(true);
        }
        private (string StartTime, string Endtime) GetToday()
        {
            string API = "http://127.0.0.1:4433";
            List<settingPageClass> settingPageClasses = settingPageClass.get_all(API);
            settingPageClass settingPage = settingPageClasses.myFind("medicine_cart", "交車時間");

            DateTime startTime_datetime = new DateTime();
            DateTime endTime_datetime = new DateTime();

            if (settingPage != null)
            {
                string 交車 = settingPage.設定值;
                TimeSpan 交車時間 = TimeSpan.Parse(交車);

                DateTime 現在 = DateTime.Now;
                TimeSpan 現在時間 = 現在.TimeOfDay;
                if (現在時間 >= 交車時間)
                {
                    // 現在時間已經過了交車時間：今天~明天
                    startTime_datetime = new DateTime(現在.Year, 現在.Month, 現在.Day, 交車時間.Hours, 交車時間.Minutes, 0);
                    endTime_datetime = startTime_datetime.AddDays(1);
                }
                else
                {
                    // 現在時間還沒到交車時間：昨天~今天
                    endTime_datetime = new DateTime(現在.Year, 現在.Month, 現在.Day, 交車時間.Hours, 交車時間.Minutes, 0);
                    startTime_datetime = endTime_datetime.AddDays(-1);
                }
            }
            string startTime = startTime_datetime.ToDateTimeString_6().Replace("/", "-");
            string endTime = endTime_datetime.ToDateTimeString_6().Replace("/", "-");
            return (startTime, endTime);
        }



    }
}
