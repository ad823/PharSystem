using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using MyUI;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class order : Controller
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

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
        [Route("init")]
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(OrderClass))]
        [HttpPost]
        public string GET_init([FromBody] returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以開方時間取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "起始時間",
        ///       "結束時間"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_rx_time_st_end")]
        [HttpPost]
        public string POST_get_by_rx_time_st_end([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_rx_time_st_end";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[起始時間][結束時間]";
                    return returnData.JsonSerializationt(true);
                }
                string 起始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];

                if (!起始時間.Check_Date_String() || !結束時間.Check_Date_String())
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }
                DateTime date_st = 起始時間.StringToDateTime();
                DateTime date_end = 結束時間.StringToDateTime();

                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByBetween(null, (int)enum_醫囑資料.開方日期, date_st.ToDateTimeString(), date_end.ToDateTimeString());
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();

                OrderClasses.sort(OrderClassMethod.SortType.產出時間);

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以過帳時間取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "起始時間",
        ///       "結束時間"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_post_time_st_end")]
        [HttpPost]
        public string POST_get_by_post_time_st_end([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_post_time_st_end";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[起始時間][結束時間]";
                    return returnData.JsonSerializationt(true);
                }
                string 起始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];

                if (!起始時間.Check_Date_String() || !結束時間.Check_Date_String())
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }
                DateTime date_st = 起始時間.StringToDateTime();
                DateTime date_end = 結束時間.StringToDateTime();

                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByBetween(null, (int)enum_醫囑資料.過帳時間, date_st.ToDateTimeString(), date_end.ToDateTimeString());
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                OrderClasses.sort(OrderClassMethod.SortType.產出時間);
                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以產出時間取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "起始時間",
        ///       "結束時間"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_creat_time_st_end")]
        [HttpPost]
        public string POST_get_by_creat_time_st_end([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_creat_time_st_end";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[起始時間][結束時間]";
                    return returnData.JsonSerializationt(true);
                }
                string 起始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];

                if (!起始時間.Check_Date_String() || !結束時間.Check_Date_String())
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }
                DateTime date_st = 起始時間.StringToDateTime();
                DateTime date_end = 結束時間.StringToDateTime();

                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByBetween(null, (int)enum_醫囑資料.產出時間, date_st.ToDateTimeString(), date_end.ToDateTimeString());
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                OrderClasses.sort(OrderClassMethod.SortType.產出時間);
                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以PRI_KEY取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "PRI_KEY",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_pri_key")]
        public string get_by_pri_key([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_pri_key";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[PRI_KEY]";
                    return returnData.JsonSerializationt(true);
                }
                List<string> PRI_KEY = returnData.ValueAry[0].Split(";").ToList();
                
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                string priKeyStr = string.Join(",", PRI_KEY.Select(x => $"'{x}'"));
                string command = $"SELECT * FROM {DB}.{TableName} WHERE PRI_KEY IN ({priKeyStr});";
                
                DataTable dataTable = sQLControl_醫令資料.WtrteCommandAndExecuteReader(command);
                List<object[]> list_value_buf = dataTable.DataTableToRowList();

                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if(OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"取無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以GUID取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "GUID",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_guid")]
        [HttpPost]
        public string POST_get_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_guid";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID]";
                    return returnData.JsonSerializationt(true);
                }
                string GUID = returnData.ValueAry[0];



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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByDefult(null, (int)enum_醫囑資料.GUID, GUID);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses[0];
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
        /// 以領藥號取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "領藥號",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_MED_BAG_NUM")]
        [HttpPost]
        public string POST_get_by_MED_BAG_NUM([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_MED_BAG_NUM";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[領藥號]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByLike(null, (int)enum_醫囑資料.領藥號, returnData.ValueAry[0]);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<OrderClass>();
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以藥袋條碼取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "藥袋條碼",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_barcode")]
        [HttpPost]
        public string POST_get_by_barcode([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_barcode";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥袋條碼]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByDefult(null, (int)enum_醫囑資料.藥袋條碼, returnData.ValueAry[0]);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<OrderClass>();
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以藥碼取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "藥碼",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_drugcode")]
        [HttpPost]
        public string POST_get_by_drugcode([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_drugcode";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByDefult(null, (int)enum_醫囑資料.藥品碼, returnData.ValueAry[0]);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<OrderClass>();
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以藥名取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "藥名",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_drugname")]
        [HttpPost]
        public string POST_get_by_drugname([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_drugname";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥名]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByLike(null, (int)enum_醫囑資料.藥品名稱, $"{returnData.ValueAry[0]}%");
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<OrderClass>();
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以病歷號取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "病歷號",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_PATCODE")]
        [HttpPost]
        public string POST_get_by_PATCODE([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_PATCODE";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[領藥號]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByLike(null, (int)enum_醫囑資料.病歷號, returnData.ValueAry[0]);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<OrderClass>();
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以病人姓名取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "病人姓名",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_PATNAME")]
        [HttpPost]
        /// <summary>
        /// 以病房號取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "病房號",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        public string POST_get_by_PATNAME([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_PATNAME";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[領藥號]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByLike(null, (int)enum_醫囑資料.病人姓名, returnData.ValueAry[0]);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Data = new List<OrderClass>();
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        [Route("get_by_ward")]
        [HttpPost]
        public string POST_get_by_ward([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_ward";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[病房號]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByDefult(null, (int)enum_醫囑資料.病房, returnData.ValueAry[0]);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<OrderClass>();
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以護理站代碼和日期取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "護理站代碼","日期",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_nursingstation_day")]
        [HttpPost]
        public string get_by_nursingstation_day([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_nursingstation_day";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[護理站代碼,日期]";
                    return returnData.JsonSerializationt(true);
                }
                string station = returnData.ValueAry[0];
                string date = returnData.ValueAry[1];
                if (date.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"輸入日期不合法";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                string cmd = $"SELECT * FROM dbvm.order_list WHERE DATE(開方日期) = '{date}' AND 病房 = '{station}';";
                System.Data.DataTable dataTable = sQLControl_醫令資料.WtrteCommandAndExecuteReader(cmd);
                List<object[]> list_value_buf = dataTable.DataTableToRowList();
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                for(int i = 0; i < OrderClasses.Count; i++)
                {
                    if (OrderClasses[i].實際調劑量.StringIsInt32() == false) OrderClasses[i].實際調劑量 = "0";
                }
                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令 {returnData.ValueAry[1]} 病房: {returnData.ValueAry[0]}!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以日期取得批次領藥的西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///   "Data": {},
        ///   "ValueAry": [
        ///     "日期"
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_batch_order_by_day")]
        [HttpPost]
        public string get_batch_order_by_day([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_batch_order_by_day";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為 [日期]";
                    return returnData.JsonSerializationt(true);
                }

                string date = returnData.ValueAry[0];

                if (date.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"輸入日期不合法";
                    return returnData.JsonSerializationt(true);
                }

                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }

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
                string TableName = "order_list";

                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                string cmd = $"SELECT * FROM dbvm.order_list WHERE DATE(開方日期) = '{date}' AND 藥袋類型 = '批次領藥';";

                System.Data.DataTable dataTable = sQLControl_醫令資料.WtrteCommandAndExecuteReader(cmd);
                List<object[]> list_value_buf = dataTable.DataTableToRowList();
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();

                for (int i = 0; i < OrderClasses.Count; i++)
                {
                    if (OrderClasses[i].實際調劑量.StringIsInt32() == false) OrderClasses[i].實際調劑量 = "0";
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令 {date} 藥袋類型: 批次領藥，共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;

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
        /// 以領藥號和日期取得西藥醫令
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "領藥號","日期",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_by_BagNum_day")]
        [HttpPost]
        public string POST_get_by_BagNum_day([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_BagNum_day";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[領藥號,日期]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByDefult(null, (int)enum_醫囑資料.領藥號, returnData.ValueAry[0]);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Data = new List<OrderClass>();
                    returnData.Result = $"查無此領藥號";
                    return returnData.JsonSerializationt();
                }
                OrderClasses = OrderClasses.Where(temp => temp.產出時間 == returnData.ValueAry[1]).ToList();
                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令 {returnData.ValueAry[1]} 領藥號: {returnData.ValueAry[0]}!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以GUID更新西藥醫令
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
        [Route("updete_by_guid")]
        [HttpPost]
        public string POST_updete_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "updete_by_guid";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                List<OrderClass> OrderClasses = returnData.Data.ObjToClass<List<OrderClass>>();
                List<object[]> list_value = OrderClasses.ClassToSQL<OrderClass, enum_醫囑資料>();
                sQLControl_醫令資料.UpdateByDefulteExtra(null, list_value);

                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"更新西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses[0];
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
        /// 以GUID刪除西藥醫令
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
        [Route("delete_by_guid")]
        [HttpPost]
        public string POST_delete_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "delete_by_guid";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                List<OrderClass> OrderClasses = returnData.Data.ObjToClass<List<OrderClass>>();
                List<object[]> list_value = OrderClasses.ClassToSQL<OrderClass, enum_醫囑資料>();
                sQLControl_醫令資料.DeleteExtra(null, list_value);

                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"刪除西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses[0];
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        [HttpPost("update_UDorder_list")]
        public async Task<string> update_UDorder_list([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_UDorder_list";
            try
            {
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                List<OrderClass> input_orderClass = returnData.Data.ObjToClass<List<OrderClass>>();
                if (input_orderClass == null || input_orderClass.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
                List<Task> tasks = new List<Task>();
                List<string> list_priKey = input_orderClass.Select(x => x.PRI_KEY).ToList();
                string priKey = string.Join(", ", list_priKey.Select(key => $"'{key}'"));

                SQLControl sQLControl_order_list = new SQLControl(Server, DB, "order_list", UserName, Password, Port, SSLMode);
                string sql = $@"
                     SELECT *
                     FROM dbvm.order_list
                     WHERE PRI_KEY IN @pri_key";


                var parameters = new
                {
                    pri_key = list_priKey,
                };
                
                List<object[]> order_object = await sQLControl_order_list.WriteCommandAsync(sql, parameters);

                List<OrderClass> orderClasses = order_object.SQLToClass<OrderClass, enum_醫囑資料>();

                List<OrderClass> add_order_list = new List<OrderClass>();
                List<OrderClass> result = new List<OrderClass>();
                List<OrderClass> order_buff = new List<OrderClass>();
                foreach (var item in input_orderClass)
                {
                    OrderClass orderClass = orderClasses.Where(order => order.PRI_KEY == item.PRI_KEY).FirstOrDefault();
                    if (orderClass == null)
                    {
                        item.GUID = Guid.NewGuid().ToString();
                        item.產出時間 = DateTime.Now.ToDateTimeString();
                        item.過帳時間 = DateTime.MinValue.ToDateTimeString();
                        item.展藥時間 = DateTime.MinValue.ToDateTimeString();
                        item.實際調劑量 = "0";
                        if (item.結方日期.StringIsEmpty()) item.結方日期 = DateTime.Now.ToDateTimeString();
                        if (item.就醫時間.StringIsEmpty()) item.就醫時間 = DateTime.Now.ToDateTimeString();
                        if (item.開方日期.StringIsEmpty()) item.開方日期 = DateTime.Now.ToDateTimeString();
                        item.狀態 = "未過帳";
                        add_order_list.Add(item);
                    }
                    else
                    {
                        result.Add(orderClass);
                    }
                }
                List<OrderClass> dc_order = new List<OrderClass>();
                List<object[]> list_add_order_list = add_order_list.ClassToSQL<OrderClass, enum_醫囑資料>();
                if (list_add_order_list.Count > 0) sQLControl_order_list.AddRows(null, list_add_order_list);

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
        /// <summary>
        /// 更新西藥醫令(限同一位病人的處方PRI_KEY相同)
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
        [HttpPost("update_order_list")]
        public string update_order_list([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_order_list";
            try
            {
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

                List<OrderClass> input_orderClass = returnData.Data.ObjToClass<List<OrderClass>>();
                string priKey = input_orderClass[0].PRI_KEY;
                if (input_orderClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
                SQLControl sQLControl_order_list = new SQLControl(Server, DB, "order_list", UserName, Password, Port, SSLMode);
                List<object[]> list_order_list = sQLControl_order_list.GetRowsByDefult(null, (int)enum_醫囑資料.PRI_KEY, priKey);
                List<OrderClass> sql_order_list = list_order_list.SQLToClass<OrderClass, enum_醫囑資料>();
                sql_order_list = sql_order_list.Where(item => item.批序.Contains("DC") == false).ToList();
                List<OrderClass> add_order_list = new List<OrderClass>();
                List<OrderClass> update_order_list = new List<OrderClass>();
                List<OrderClass> result_order_list = new List<OrderClass>();

                foreach (var orderClass in input_orderClass)
                {
                    string 批序 = orderClass.批序.Split("-")[0];
                    OrderClass orderClass_db = sql_order_list.Where(temp => temp.批序.StartsWith(批序)).FirstOrDefault();

                    if (orderClass_db == null)
                    {
                        orderClass.GUID = Guid.NewGuid().ToString();
                        orderClass.產出時間 = DateTime.Now.ToDateTimeString_6();
                        orderClass.過帳時間 = DateTime.MinValue.ToDateTimeString_6();
                        orderClass.展藥時間 = DateTime.MinValue.ToDateTimeString_6();
                        orderClass.狀態 = "未過帳";
                        orderClass.實際調劑量 = "0";
                        //if(orderClass.備註.IndexOf("[NEW]") == -1) orderClass.備註 = $"[NEW]{orderClass.備註}";
                        //if(orderClass.備註.Contains("[NEW]") == false) orderClass.備註 = $"[NEW]{orderClass.備註}";
                        add_order_list.Add(orderClass);
                    }
                    else
                    {
                        bool flag = false;
                        if (orderClass_db.藥品碼 != orderClass.藥品碼) flag = true;
                        if (orderClass_db.交易量 != orderClass.交易量) flag = true;
                        if (flag)
                        {
                            orderClass.GUID = Guid.NewGuid().ToString();
                            orderClass.產出時間 = DateTime.Now.ToDateTimeString_6();
                            orderClass.過帳時間 = DateTime.MinValue.ToDateTimeString_6();
                            orderClass.展藥時間 = DateTime.MinValue.ToDateTimeString_6();
                            orderClass.狀態 = "未過帳";
                            orderClass.批序 += "-[New]";
                            add_order_list.Add(orderClass);

                            orderClass_db.批序 += "-[DC]";
                            update_order_list.Add(orderClass_db);
                        }
                        else
                        {
                            result_order_list.Add(orderClass_db);
                        }
                    }
                }


                List<object[]> list_add_order_list = add_order_list.ClassToSQL<OrderClass, enum_醫囑資料>();
                List<object[]> list_update_order_list = update_order_list.ClassToSQL<OrderClass, enum_醫囑資料>();

                if (list_add_order_list.Count > 0) sQLControl_order_list.AddRows(null, list_add_order_list);
                if (list_update_order_list.Count > 0) sQLControl_order_list.UpdateByDefulteExtra(null, list_update_order_list);
                result_order_list.AddRange(add_order_list);
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = result_order_list;
                returnData.Result = $"取得醫令成功,共<{input_orderClass.Count}>筆,新增<{list_add_order_list.Count}>筆,更新<{list_update_order_list.Count}>筆";
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
        [HttpPost("update_order_list_new")]
        public string update_order_list_new([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_order_list_new";
            try
            {
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

                string API = HIS_WebApi.Method.GetServerAPI("Main", "網頁", "API01");
                List<OrderClass> input_orderClass = returnData.Data.ObjToClass<List<OrderClass>>();
                if (input_orderClass == null || input_orderClass.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }


                SQLControl sQLControl_order_list = new SQLControl(Server, DB, "order_list", UserName, Password, Port, SSLMode);
                string command = string.Empty;

                string[] array_priKey = input_orderClass[0].PRI_KEY.Split("-");
                if (array_priKey.Length < 3)
                {
                    returnData.Code = -200;
                    returnData.Result = $"PRI_KEY格式錯誤，應為 '開方時間(yyyyMMddHHmmss)-病歷號'";
                    return returnData.JsonSerializationt();
                }
                string PRI_KEY = $"{array_priKey[0]}-{array_priKey[1]}";
                string 開方日期 = input_orderClass[0].開方日期.StringToDateTime().ToString("yyyy-MM-dd");

                command = $"SELECT * FROM {DB}.order_list " +
                    $"WHERE PRI_KEY like '{PRI_KEY}%' " +
                    $" AND 開方日期 >= '{開方日期} 00:00:00'" +
                    $" AND 開方日期 <= '{開方日期} 23:59:59';";


                DataTable dataTable = sQLControl_order_list.WtrteCommandAndExecuteReader(command);
                List<object[]> order_object = dataTable.DataTableToRowList();
                List<OrderClass> orderClasses = order_object.SQLToClass<OrderClass, enum_醫囑資料>();

                List<OrderClass> add_order_list = new List<OrderClass>();
                List<OrderClass> result = new List<OrderClass>();
                List<OrderClass> order_buff = new List<OrderClass>();

                foreach (var item in input_orderClass)
                {
                    OrderClass orderClass = orderClasses.Where(order => order.PRI_KEY == item.PRI_KEY).FirstOrDefault();
                    if (orderClass == null)
                    {
                        item.GUID = Guid.NewGuid().ToString();
                        item.產出時間 = DateTime.Now.ToDateTimeString();
                        item.過帳時間 = DateTime.MinValue.ToDateTimeString();
                        item.展藥時間 = DateTime.MinValue.ToDateTimeString();
                        item.實際調劑量 = "0";
                        if (item.結方日期.StringIsEmpty()) item.結方日期 = DateTime.Now.ToDateTimeString();
                        if (item.就醫時間.StringIsEmpty()) item.就醫時間 = DateTime.Now.ToDateTimeString();
                        if (item.開方日期.StringIsEmpty()) item.開方日期 = DateTime.Now.ToDateTimeString();
                        item.狀態 = "未過帳";
                        add_order_list.Add(item);
                    }
                    else
                    {
                        order_buff.Add(orderClass);
                        result.Add(orderClass);
                    }
                }
                List<OrderClass> update_order_list = new List<OrderClass>();
                List<OrderClass> dc_order = new List<OrderClass>();


                List<string> list_priKey_buff = order_buff.Select(x => x.PRI_KEY).ToList();
                dc_order = orderClasses.Where(x => list_priKey_buff.Contains(x.PRI_KEY) == false).ToList();
                foreach (var item in dc_order)
                {
                    if (item.批序.Contains("[DC]")) continue;
                    item.批序 += $"-[DC]";

                    update_order_list.Add(item);
                }

                List<object[]> list_add_order_list = add_order_list.ClassToSQL<OrderClass, enum_醫囑資料>();
                List<object[]> list_update_order_list = update_order_list.ClassToSQL<OrderClass, enum_醫囑資料>();


                if (list_add_order_list.Count > 0) sQLControl_order_list.AddRows(null, list_add_order_list);
                if (list_update_order_list.Count > 0) sQLControl_order_list.UpdateByDefulteExtra(null, list_update_order_list);

                result.AddRange(add_order_list);
                if (dc_order.Count > 0) result.AddRange(dc_order);

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = result;
                returnData.Result = $"取得醫令成功,共<{result.Count}>筆,新增<{list_add_order_list.Count}>筆,更新DC<{list_update_order_list.Count}>筆";
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
        /// 新增西藥醫令
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
        [Route("add")]
        [HttpPost]
        public string POST_add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "add";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                List<OrderClass> OrderClasses = returnData.Data.ObjToClass<List<OrderClass>>();
                for (int i = 0; i < OrderClasses.Count; i++)
                {
                    OrderClasses[i].GUID = Guid.NewGuid().ToString();
                    OrderClasses[i].產出時間 = DateTime.Now.ToDateTimeString_6();
                    OrderClasses[i].過帳時間 = DateTime.MinValue.ToDateTimeString();
                    OrderClasses[i].核對時間 = DateTime.MinValue.ToDateTimeString();
                    OrderClasses[i].發藥時間 = DateTime.MinValue.ToDateTimeString();
                    OrderClasses[i].領藥時間 = DateTime.MinValue.ToDateTimeString();
                    OrderClasses[i].就醫時間 = DateTime.MinValue.ToDateTimeString();
                    OrderClasses[i].狀態 = "未過帳";
 
                }
                List<object[]> list_value = OrderClasses.ClassToSQL<OrderClass, enum_醫囑資料>();
                sQLControl_醫令資料.AddRows(null, list_value);

                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"新增西藥醫令!共<{OrderClasses.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以GUID更新西藥醫令
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
        [Route("add_and_updete_by_guid")]
        [HttpPost]
        public string POST_add_and_updete_by_guid([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "add_and_updete_by_guid";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                List<OrderClass> OrderClasses = returnData.Data.ObjToClass<List<OrderClass>>();
                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入醫令資料為空值";
                    return returnData.JsonSerializationt();
                }
                List<object[]> list_value_buf = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                for (int i = 0; i < OrderClasses.Count; i++)
                {
                    string GUID = OrderClasses[i].GUID;
                    object[] value = OrderClasses[i].ClassToSQL<OrderClass, enum_醫囑資料>();
                    list_value_buf = sQLControl_醫令資料.GetRowsByDefult(null, (int)enum_醫囑資料.GUID, GUID);
                    if(list_value_buf.Count == 0)
                    {
                        list_value_add.Add(value);
                    }
                    else
                    {
                        list_value_replace.Add(value);
                    }
                }

                sQLControl_醫令資料.AddRows(null, list_value_add);
                sQLControl_醫令資料.UpdateByDefulteExtra(null, list_value_replace);



                returnData.Code = 200;
                returnData.Result = $"新增及更新西藥醫令,共新增<{list_value_add.Count}>筆資料,修改<{list_value_replace.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses;
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
        /// 以領藥號取得西藥醫令病患列表
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "領藥號",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_header_by_MED_BAG_NUM")]
        [HttpPost]
        public string POST_header_by_MED_BAG_NUM([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "header_by_MED_BAG_NUM";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[領藥號]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByLike(null, (int)enum_醫囑資料.領藥號, returnData.ValueAry[0]);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                List<OrderClass> OrderClasses_buf = new List<OrderClass>();
                List<OrderClass> OrderClasses_result = new List<OrderClass>();
                List<string> list_PRI_KEY = (from temp in OrderClasses
                                             select temp.PRI_KEY).Distinct().ToList();
                var keyValuePairs = OrderClasses.CoverToDictionaryBy_PRI_KEY();

                for (int i = 0; i < list_PRI_KEY.Count; i++)
                {
                    string PRI_KEY = list_PRI_KEY[i];
                    string 調劑完成 = "Y";
                    OrderClasses_buf = keyValuePairs.SortDictionaryBy_PRI_KEY(list_PRI_KEY[i]);
                    for (int k = 0; k < OrderClasses_buf.Count; k++)
                    {
                        if (OrderClasses_buf[k].實際調劑量.StringIsDouble() == false)
                        {
                            調劑完成 = "N";
                        }
                    }
                    if (OrderClasses_buf.Count > 0)
                    {
                        OrderClasses_buf[0].住院序號 = "";
                        OrderClasses_buf[0].備註 = "";
                        OrderClasses_buf[0].劑量單位 = "";
                        OrderClasses_buf[0].單次劑量 = "";
                        OrderClasses_buf[0].天數 = "";
                        OrderClasses_buf[0].實際調劑量 = "";
                        OrderClasses_buf[0].就醫序號 = "";
                        OrderClasses_buf[0].就醫類別 = "";
                        OrderClasses_buf[0].展藥時間 = "";
                        OrderClasses_buf[0].床號 = "";
                        OrderClasses_buf[0].批序 = "";
                        OrderClasses_buf[0].核對ID = "";
                        OrderClasses_buf[0].核對姓名 = "";
                        OrderClasses_buf[0].藥品名稱 = "";
                        OrderClasses_buf[0].藥品碼 = "";
                        OrderClasses_buf[0].藥局代碼 = "";
                        OrderClasses_buf[0].藥師ID = "";
                        OrderClasses_buf[0].藥師姓名 = "";
                        OrderClasses_buf[0].藥袋條碼 = "";
                        OrderClasses_buf[0].藥袋類型 = "";
                        OrderClasses_buf[0].費用別 = "";
                        OrderClasses_buf[0].途徑 = "";
                        OrderClasses_buf[0].醫師代碼 = "";
                        OrderClasses_buf[0].狀態 = 調劑完成;
                        OrderClasses_result.Add(OrderClasses_buf[0]);
                    }

                }


                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<OrderClass>();
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses_result.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses_result;
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
        /// 以病歷號取得西藥醫令病患列表
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "病歷號",
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_header_by_PATCODE")]
        [HttpPost]
        public string POST_get_header_by_PATCODE([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_header_by_PATCODE";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                string serverName = returnData.ServerName;
                string serverType = returnData.ServerType;


                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[病歷號]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                }
                else
                {
                    sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "醫囑資料");
                }
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
                string TableName = "order_list";
                SQLControl sQLControl_醫令資料 = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value_buf = sQLControl_醫令資料.GetRowsByLike(null, (int)enum_醫囑資料.病歷號, returnData.ValueAry[0]);
                List<OrderClass> OrderClasses = list_value_buf.SQLToClass<OrderClass, enum_醫囑資料>();
                List<OrderClass> OrderClasses_buf = new List<OrderClass>();
                List<OrderClass> OrderClasses_result = new List<OrderClass>();
                List<string> list_PRI_KEY = (from temp in OrderClasses
                                             select temp.PRI_KEY).Distinct().ToList();
                var keyValuePairs = OrderClasses.CoverToDictionaryBy_PRI_KEY();

                for (int i = 0; i < list_PRI_KEY.Count; i++)
                {
                    string PRI_KEY = list_PRI_KEY[i];
                    string 調劑完成 = "Y";
                    OrderClasses_buf = keyValuePairs.SortDictionaryBy_PRI_KEY(list_PRI_KEY[i]);
                    for (int k = 0; k < OrderClasses_buf.Count; k++)
                    {
                        if (OrderClasses_buf[k].實際調劑量.StringIsDouble() == false)
                        {
                            調劑完成 = "N";
                        }
                    }
                    if (OrderClasses_buf.Count > 0)
                    {
                        OrderClasses_buf[0].住院序號 = "";
                        OrderClasses_buf[0].備註 = "";
                        OrderClasses_buf[0].劑量單位 = "";
                        OrderClasses_buf[0].單次劑量 = "";
                        OrderClasses_buf[0].天數 = "";
                        OrderClasses_buf[0].實際調劑量 = "";
                        OrderClasses_buf[0].就醫序號 = "";
                        OrderClasses_buf[0].就醫類別 = "";
                        OrderClasses_buf[0].展藥時間 = "";
                        OrderClasses_buf[0].床號 = "";
                        OrderClasses_buf[0].批序 = "";
                        OrderClasses_buf[0].核對ID = "";
                        OrderClasses_buf[0].核對姓名 = "";
                        OrderClasses_buf[0].藥品名稱 = "";
                        OrderClasses_buf[0].藥品碼 = "";
                        OrderClasses_buf[0].藥局代碼 = "";
                        OrderClasses_buf[0].藥師ID = "";
                        OrderClasses_buf[0].藥師姓名 = "";
                        OrderClasses_buf[0].藥袋條碼 = "";
                        OrderClasses_buf[0].藥袋類型 = "";
                        OrderClasses_buf[0].費用別 = "";
                        OrderClasses_buf[0].途徑 = "";
                        OrderClasses_buf[0].醫師代碼 = "";
                        OrderClasses_buf[0].狀態 = 調劑完成;
                        OrderClasses_result.Add(OrderClasses_buf[0]);
                    }

                }


                if (OrderClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    returnData.Data = new List<OrderClass>();
                    return returnData.JsonSerializationt();
                }

                returnData.Code = 200;
                returnData.Result = $"取得西藥醫令!共<{OrderClasses_result.Count}>筆資料";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = OrderClasses_result;
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
        /// 取得庫存清單(Excel)
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        List(OrderClass)
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為交易紀錄結構</returns>
        [HttpPost("download_datas_excel")]
        public async Task<ActionResult> download_datas_excel([FromBody] returnData returnData)
        {
            try
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                List<OrderClass> OrderClasses = returnData.Data.ObjToClass<List<OrderClass>>();
                if (OrderClasses == null)
                {
                    return BadRequest("returnData.Data不能是空的");
                }
                List<object[]> list_orderClasses = OrderClasses.ClassToSQL<OrderClass, enum_醫囑資料>();
                System.Data.DataTable dataTable = list_orderClasses.ToDataTable(new enum_醫囑資料());

                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";
                List<System.Data.DataTable> dataTables = new List<System.Data.DataTable>();
                dataTables.Add(dataTable);
                byte[] excelData = MyOffice.ExcelClass.NPOI_GetBytes(dataTable, Excel_Type.xlsx, (int)enum_醫囑資料.交易量, (int)enum_醫囑資料.實際調劑量);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_西藥醫令.xlsx"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"系統錯誤：{ex.Message}");
            }

        }
        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_醫囑資料());               
            return table.JsonSerializationt(true);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public returnData get_by_pri_key(string pri_key)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(pri_key);
            string result = get_by_pri_key(returnData);
            return result.JsonDeserializet<returnData>();
        }

    }
}
