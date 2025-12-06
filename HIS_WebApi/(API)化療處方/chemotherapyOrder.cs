using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using System.Security;

using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Configuration;
using HIS_DB_Lib;
using System.Data;
using MyOffice;
using MyUI;
using System.IO;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class chemotherapyOrder : Controller
    {
        static private string API_Server = "http://127.0.0.1:4433";
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
        [HttpPost("init")]
        public string init([FromBody] returnData returnData)
        {
            var timer = new MyTimerBasic();
            returnData.Method = "init";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass settingClass = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端").FirstOrDefault();
                if (settingClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                List<Table> tables = new List<Table>();
                tables.Add(MethodClass.CheckCreatTable<chemotherapyOrderClass>(settingClass));
                tables.Add(MethodClass.CheckCreatTable<chemotherapyOrderDayClass>(settingClass));

                returnData.Code = 200;
                returnData.Data = tables;
                returnData.Result = "初始化 chemotherapyOrder 資料表完成";
                returnData.TimeTaken = $"{timer}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception: {ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新西藥醫令(同一位病人PRI_KEY 不同)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///       [OrderClass(陣列)]
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("update_order_list")]
        public async Task<string> update_order_list([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_order_list";
            try
            {
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;

                List<chemotherapyOrderClass> input_orderClass = returnData.Data.ObjToClass<List<chemotherapyOrderClass>>();
                if (input_orderClass == null || input_orderClass.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }


                SQLControl sQLControl = new SQLControl(Server, DB, "chemotherapy_orders", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_sub = new SQLControl(Server, DB, "chemotherapy_order_days", UserName, Password, Port, SSLMode);

                string command = string.Empty;

                string[] array_priKey = input_orderClass[0].PRI_KEY.Split("-");
                if (array_priKey.Length < 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"PRI_KEY格式錯誤，應為 '開方時間(yyyyMMddHHmmss)-病歷號'";
                    return returnData.JsonSerializationt();
                }
                input_orderClass = input_orderClass.OrderByDescending(x => x.處方日期).ToList();

                int count_ = input_orderClass.Count;
                string PRI_KEY = $"{array_priKey[0]}-{array_priKey[1]}";
                string start = input_orderClass[count_ - 1].處方日期.StringToDateTime().ToString("yyyy-MM-dd");
                string end = input_orderClass[0].處方日期.StringToDateTime().ToString("yyyy-MM-dd");
                string 病歷號 = input_orderClass[0].病歷號;

                command = $"SELECT * FROM {DB}.chemotherapy_orders " +
                    $"WHERE 病歷號 = '{病歷號}' " +
                    $" AND 處方日期 >= '{start} 00:00:00'" +
                    $" AND 處方日期 <= '{end} 23:59:59';";


                List<object[]> order_object = await sQLControl.WriteCommandAsync(command);
                List<chemotherapyOrderClass> orderClasses = order_object.SQLToClass<chemotherapyOrderClass>();
                string[] GUID = orderClasses.Select(x => x.GUID).ToArray();
                string GUID_string = GUID.Length > 0 ?string.Join(",", GUID.Select(x => $"'{x}'")):"''";
                command = $"SELECT * FROM {DB}.chemotherapy_order_days " +
                    $"WHERE 主表GUID IN ({GUID_string});";
                List<object[]> sub_order_object = await sQLControl_sub.WriteCommandAsync(command); //主表GUID

                //List<object[]> sub_order_object = await sQLControl_sub.GetRowsByDefultAsync(null, 2, GUID); //主表GUID
                List<chemotherapyOrderDayClass> sub_orderClasses = sub_order_object.SQLToClass<chemotherapyOrderDayClass>();

                List<chemotherapyOrderClass> add_order_list = new List<chemotherapyOrderClass>();
                List<chemotherapyOrderDayClass> add_sub_order_list = new List<chemotherapyOrderDayClass>();

                List<chemotherapyOrderClass> result = new List<chemotherapyOrderClass>();
                string now = DateTime.Now.ToDateTimeString();
                string min_time = DateTime.MinValue.ToDateTimeString();
                foreach (var item in input_orderClass)
                {
                    chemotherapyOrderClass orderClass = orderClasses.Where(order => order.PRI_KEY == item.PRI_KEY).FirstOrDefault();
                    if (orderClass == null)
                    {
                        item.GUID = Guid.NewGuid().ToString();
                        item.建立時間 = now;
                        item.更新時間 = now;                                             
                        foreach(var sub_order in item.每日紀錄)
                        {
                            sub_order.GUID = Guid.NewGuid().ToString();
                            sub_order.主表GUID = item.GUID;
                            sub_order.審核時間 = min_time;
                            sub_order.調劑時間 = min_time;
                            sub_order.核對時間 = min_time;
                            sub_order.建立時間 = now;
                            sub_order.更新時間 = now;
                            add_sub_order_list.Add(sub_order);
                        }
                        item.每日紀錄 = add_sub_order_list;
                        add_order_list.Add(item);

                    }
                    else
                    {
                        List<chemotherapyOrderDayClass> chemotherapyOrderDayClass = sub_orderClasses.Where(order => order.主表GUID == orderClass.GUID).ToList();
                        orderClass.每日紀錄 = chemotherapyOrderDayClass;
                        result.Add(orderClass);
                    }
                }
                List<object[]> list_add_order_list = add_order_list.ClassToSQL<chemotherapyOrderClass>();
                List<object[]> list_add_sub_order_list = add_sub_order_list.ClassToSQL<chemotherapyOrderDayClass>();

                if (list_add_order_list.Count > 0) 
                {
                    Logger.Log("FADC", $"{add_order_list.JsonSerializationt(true)}");
                    sQLControl.AddRows(null, list_add_order_list);
                }

                if (list_add_sub_order_list.Count > 0)
                {
                    Logger.Log("FADC", $"{add_sub_order_list.JsonSerializationt(true)}");
                    sQLControl_sub.AddRows(null, list_add_sub_order_list);
                }

                result.AddRange(add_order_list);

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = result;
                returnData.Result = $"取得醫令成功,共<{result.Count}>筆,新增<{list_add_order_list.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);

            }
        }
    }
}
