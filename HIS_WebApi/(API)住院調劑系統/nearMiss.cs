using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using SQLUI;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Linq;
using HIS_WebApi._API_系統;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_住院調劑系統
{
    [Route("api/[controller]")]
    [ApiController]
    public class nearMiss : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(200, "nearMissClass物件", typeof(nearMissClass))]

        /// <summary>
        ///初始化住院藥車資料庫
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
        /// 新增(更新)資料
        /// </summary>
        /// <remarks>
        /// <code>
        ///   {
        ///     "Data": 
        ///     [
        ///         {
        ///         "pat_GUID":
        ///         "cpoe_GUID"
        ///         "pharm": "",
        ///         "nurnum":"",
        ///         "disp_id":"",
        ///         "disp_name":"",
        ///         "reporter_id":"",
        ///         "reporter_name":"",
        ///         "creat_time":"",
        ///         "reason":"",
        ///         "note":""
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
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data資料錯誤!";
                    return returnData.JsonSerializationt();
                }
                List<nearMissClass> nearMissClasses = returnData.Data.ObjToClass<List<nearMissClass>>();
                if (nearMissClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data資料錯誤，須為 List<nearMissClass>!";
                    return returnData.JsonSerializationt();
                }
                string tableName = "nearMiss";
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<string> cpoe_guids = nearMissClasses.Select(x => x.cpoe_GUID).Distinct().ToList();
                string command = getCommand(DB, tableName, cpoe_guids);
                List<object[]> objects = await sQLControl.WriteCommandAsync(command);
                List<nearMissClass> nearMisses = objects.SQLToClass<nearMissClass, enum_nearMiss>();
                List<nearMissClass> add_nearMisses = new List<nearMissClass>();
                List<nearMissClass> update_nearMisses = new List<nearMissClass>();


                foreach (var item in nearMissClasses)
                {
                    nearMissClass nearMiss_buff = nearMisses.Where(m => m.cpoe_GUID == item.cpoe_GUID).FirstOrDefault();
                    if (nearMiss_buff == null)
                    {
                        item.GUID = Guid.NewGuid().ToString();
                        item.建立時間 = DateTime.Now.ToDateTimeString();
                        add_nearMisses.Add(item);
                    }
                    else
                    {
                        if (item.原因.StringIsEmpty() == false && item.原因 != nearMiss_buff.原因) nearMiss_buff.原因 = item.原因;
                        if (item.備註.StringIsEmpty() == false && item.備註 != nearMiss_buff.備註) nearMiss_buff.備註 = item.備註;
                        if (item.調劑人ID.StringIsEmpty() == false && item.調劑人ID != nearMiss_buff.調劑人ID) nearMiss_buff.調劑人ID = item.調劑人ID;
                        if (item.調劑人姓名.StringIsEmpty() == false && item.調劑人姓名 != nearMiss_buff.調劑人姓名) nearMiss_buff.備註 = item.調劑人姓名;
                        if (item.通報人ID.StringIsEmpty() == false && item.通報人ID != nearMiss_buff.通報人ID) nearMiss_buff.通報人ID = item.通報人ID;
                        if (item.通報人姓名.StringIsEmpty() == false && item.通報人姓名 != nearMiss_buff.通報人姓名) nearMiss_buff.通報人姓名 = item.通報人姓名;
                        update_nearMisses.Add(nearMiss_buff);
                    }
                }
                List<Task> tasks = new List<Task>();
                List<object[]> add = add_nearMisses.ClassToSQL<nearMissClass, enum_nearMiss>();
                List<object[]> update = update_nearMisses.ClassToSQL<nearMissClass, enum_nearMiss>();
                if (add.Count > 0) tasks.Add(sQLControl.AddRowsAsync(null, add));
                if (update.Count > 0) tasks.Add(sQLControl.UpdateRowsAsync(null, update));
                if (tasks.Count > 0) await Task.WhenAll(tasks);

                returnData.Code = 200;
                returnData.Data = nearMissClasses;
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
        /// 依照建立時間取得資料
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
        ///     "ValueAry":["2025-08-24 00:00:00","2025-08-24 23:59:59"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_creat_time")]
        public async Task<string> get_by_creat_time([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.ValueAry資料錯誤，['2025-08-24 00:00:00','2025-08-24 23:59:59']!";
                    return returnData.JsonSerializationt();
                }
                string startTime = returnData.ValueAry[0];
                string endTime = returnData.ValueAry[1];
                if (startTime.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.ValueAry[0]資料錯誤，須為 '2025-08-24 00:00:00' 格式!";
                    return returnData.JsonSerializationt();
                }
                if (endTime.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.ValueAry[0]資料錯誤，須為 '2025-08-24 00:00:00' 格式!";
                    return returnData.JsonSerializationt();
                }
                string tableName = "nearMiss";
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                string command = getCommand(DB, tableName, startTime, endTime);
                List<object[]> objects = await sQLControl.WriteCommandAsync(command);
                List<nearMissClass> nearMisses = objects.SQLToClass<nearMissClass, enum_nearMiss>();
                

                returnData.Code = 200;
                returnData.Data = nearMisses;
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
        /// 依照藥局取得當天資料
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
        ///     "ValueAry":["藥局"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_phar")]
        public async Task<string> get_by_phar([FromBody] returnData returnData, CancellationToken ct)
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
                    returnData.Result = "returnData.ValueAry資料錯誤，['藥局']!";
                    return await returnData.JsonSerializationtAsync(true);
                }
                string 藥局 = returnData.ValueAry[0];               
                string tableName = "nearMiss";
                Task<(string startTime, string endTime)> taskGetToday = GetTodayAsync(ct);
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                (string startTime, string endTime) = await taskGetToday;
                if (startTime.StringIsEmpty() || endTime.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "取得交車時間失敗";
                    return await returnData.JsonSerializationtAsync(true);
                }
                string command = getCommand(DB, tableName, startTime, endTime, 藥局);
                List<object[]> objects = await sQLControl.WriteCommandAsync(command ,ct);
                
                List<nearMissClass> nearMisses = objects.SQLToClass<nearMissClass, enum_nearMiss>();
                if(nearMisses.Count == 0)
                {
                    returnData.Code = 200;
                    returnData.Data = nearMisses;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Method = "get_by_phar";
                    returnData.Result = $"查無今日調劑錯誤資料!";
                    return await returnData.JsonSerializationtAsync(true);
                }
                string GUIDs = string.Join(";", nearMisses.Select(x => x.cpoe_GUID).Distinct().ToList());
                returnData task_cpoe = await new med_cart().get_medCpoe_by_GUID(GUIDs);
                if(task_cpoe.Code != 200)
                {
                    returnData.Code = -200;
                    returnData.Result = "取得處方失敗";
                    returnData.Method = "get_by_phar";
                    return await returnData.JsonSerializationtAsync(true);
                }
                List<medCpoeClass> medCpoeClasses = task_cpoe.Data.ObjToClass<List<medCpoeClass>>();
                foreach (var item in nearMisses)
                {
                    medCpoeClass medCpoe_buff = medCpoeClasses.Where(x => x.GUID == item.cpoe_GUID).FirstOrDefault();
                    if (medCpoe_buff == null) continue;
                    item.medCpoe = medCpoe_buff;
                }
                returnData.Code = 200;
                returnData.Data = nearMisses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_by_phar";
                returnData.Result = $"取得藥局{藥局}Nearmiss資料，共{nearMisses.Count}筆!";
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
        /// 依照病人GUID取得資料
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
        ///     "ValueAry":["PAT_GUID"]
        ///     "TableName": "",
        ///     "ServerName": "",
        ///     "ServerType": "",
        ///     "TimeTaken": ""
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_pat_GUID")]
        public async Task<string> get_by_pat_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry資料錯誤!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1 || returnData.ValueAry[0].StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.ValueAry資料錯誤，['pat_GUID']!";
                    return returnData.JsonSerializationt();
                }
                
                string tableName = "nearMiss";
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_nearMiss.pat_GUID, returnData.ValueAry[0]);
                List<nearMissClass> nearMisses = objects.SQLToClass<nearMissClass, enum_nearMiss>();


                returnData.Code = 200;
                returnData.Data = nearMisses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_by_pat_GUID";
                returnData.Result = $"病人({returnData.ValueAry[0]})調劑錯誤資料，共{nearMisses.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
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

            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClasses[0], new enum_nearMiss()));

            return tables.JsonSerializationt(true);
        }
        private string getCommand (string db, string tableName, List<string> strings)
        {
            string inClause = string.Join(",", strings.Select(g => $"'{g}'"));  // 加上單引號包起來
            string command = $"SELECT * FROM {db}.{tableName} WHERE cpoe_GUID IN ({inClause});";
            return command;
        }
        private string getCommand(string db, string tableName,string startTime, string endTime)
        {
            string command = @$"SELECT * FROM {db}.{tableName} 
                                WHERE 建立時間 >= '{startTime}' 
                                AND 建立時間 <= '{endTime}';";
            return command;
        }
        private string getCommand(string db, string tableName, string startTime, string endTime, string phar)
        {
            string command = @$"SELECT * FROM {db}.{tableName} 
                                WHERE 建立時間 >= '{startTime}' 
                                AND 建立時間 <= '{endTime}'
                                AND 藥局 = '{phar}';";
            return command;
        }
        private async Task<(string StartTime, string Endtime)> GetTodayAsync(CancellationToken ct = default)
        {

            settingPageClass settingPageClass = await new settingPage().get_by_page_name_cht("medicine_cart", "交車時間", ct);
            if (settingPageClass == null)
            {
                return ("", "");
            }
            string 交車 = settingPageClass.設定值;
            if (TimeSpan.TryParse(交車, out TimeSpan 交車時間) == false)
            {
                return ("", "");
            }
            DateTime startTime_datetime, endTime_datetime;
            DateTime 現在 = DateTime.Now;
            if (現在.TimeOfDay >= 交車時間)
            {
                // 今天交車時間 ~ 明天交車時間
                startTime_datetime = new DateTime(現在.Year, 現在.Month, 現在.Day,
                                                  交車時間.Hours, 交車時間.Minutes, 0);
                endTime_datetime = startTime_datetime.AddDays(1);
            }
            else
            {
                // 現在時間還沒到交車時間：昨天~今天
                endTime_datetime = new DateTime(現在.Year, 現在.Month, 現在.Day,
                                        交車時間.Hours, 交車時間.Minutes, 0);
                startTime_datetime = endTime_datetime.AddDays(-1);
            }

            string startTime = startTime_datetime.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = endTime_datetime.ToString("yyyy-MM-dd HH:mm:ss");
            return (startTime, endTime);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public static async Task<returnData> get_by_pat_GUID(string GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(GUID);
            string result = await new nearMiss().get_by_pat_GUID(returnData);
            returnData = await result.JsonDeserializetAsync<returnData>();
            return returnData;
        }


    }
}
