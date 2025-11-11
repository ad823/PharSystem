using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using MyUI;
using NPOI;
using NPOI.SS.Formula.Functions;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class inventoryController : Controller
    {
        private class SheetTemp
        {
            public SheetTemp(string name)
            {
                Name = name;
            }
            public string Name = "";
            public List<object[]> list_value = new List<object[]>();
        }
        static public string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        /// <summary>
        /// 初始化盤點單資料庫
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
        [HttpPost]
        public string GET_init([FromBody] returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
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
                string msg = "";
                return msg;
            }

        }
        /// <summary>
        /// 取得可建立今日最新盤點單號
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
        /// <returns>[returnData.Value]為建立盤點單號</returns>
        [Route("new_IC_SN")]
        [HttpPost]
        public string GET_new_IC_SN([FromBody] returnData returnData)
        {
            GET_init(returnData);
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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


            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);

            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
            List<object[]> list_inventory_creat_buf = new List<object[]>();

            list_inventory_creat_buf = list_inventory_creat.GetRowsInDate((int)enum_盤點單號.建表時間, DateTime.Now);


            string 盤點單號 = "";
            int index = 0;
            while (true)
            {
                盤點單號 = $"INV{DateTime.Now.ToDateTinyString()}-{index}";
                index++;
                list_inventory_creat_buf = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, 盤點單號);
                if (list_inventory_creat_buf.Count == 0) break;
            }
            returnData.Value = 盤點單號;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "new_IC_SN";
            returnData.Result = $"成功! {myTimer.ToString()}";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 取得所有解鎖盤點單
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
        /// <returns>[returnData.Data]為盤點單陣列結構</returns>
        [Route("get_all_unlock_creat")]
        [HttpPost]
        public string get_all_unlock_creat([FromBody] returnData returnData)
        {
            try
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);

                List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
                list_inventory_creat = (from temp in list_inventory_creat
                                        where temp[(int)enum_盤點單號.盤點狀態].ObjectToString() != enum_盤點狀態.鎖定.GetEnumName()
                                        select temp).ToList();
                List<inventoryClass.creat> creats = list_inventory_creat.SQLToClass<inventoryClass.creat, enum_盤點單號>();
                returnData.Code = 200;
                returnData.Data = creats;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Method = "get_all_unlock_creat";
                returnData.Result = $"取得盤點資料成功!";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }
        /// <summary>
        /// 以建表日區間搜尋盤點單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 起始日期,結束日期 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "2023/10/26,2023/10/27"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_get_by_CT_TIME_ST_END")]
        [HttpPost]
        public string creat_get_by_CT_TIME_ST_END([FromBody] returnData returnData)
        {
            try
            {

                GET_init(returnData);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
                string[] date_ary = returnData.Value.Split(',');
                if (date_ary.Length != 2)
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }
                else
                {
                    if (!date_ary[0].Check_Date_String() || !date_ary[1].Check_Date_String())
                    {
                        returnData.Code = -5;
                        returnData.Result = "輸入日期格式錯誤!";
                        return returnData.JsonSerializationt();
                    }
                }
                DateTime date_st = date_ary[0].StringToDateTime();
                DateTime date_end = date_ary[1].StringToDateTime();
                sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
                list_inventory_creat = list_inventory_creat.GetRowsInDateEx((int)enum_盤點單號.建表時間, date_st, date_end);
                returnData = Function_Get_inventory_creat(sys_serverSettingClasses[0], returnData.TableName, list_inventory_creat, false);
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Method = "creat_get_by_CT_TIME_ST_END";
                returnData.Result = $"取得盤點資料成功!";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 以建表日區間搜尋覆盤單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 起始日期,結束日期 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "2023/10/26,2023/10/27"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>

        [Route("creat_review_get_by_CT_TIME_ST_END")]
        [HttpPost]
        public string creat_review_get_by_CT_TIME_ST_END([FromBody] returnData returnData)
        {
            try
            {

                GET_init(returnData);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
                string[] date_ary = returnData.Value.Split(',');
                if (date_ary.Length != 2)
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }
                else
                {
                    if (!date_ary[0].Check_Date_String() || !date_ary[1].Check_Date_String())
                    {
                        returnData.Code = -5;
                        returnData.Result = "輸入日期格式錯誤!";
                        return returnData.JsonSerializationt();
                    }
                }
                DateTime date_st = date_ary[0].StringToDateTime();
                DateTime date_end = date_ary[1].StringToDateTime();
                sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
                list_inventory_creat = list_inventory_creat.GetRowsInDateEx((int)enum_盤點單號.建表時間, date_st, date_end);
                returnData = Function_Get_inventory_creat(sys_serverSettingClasses[0], returnData.TableName, list_inventory_creat, false);
                List<inventoryClass.creat> creats = returnData.Data.ObjToClass<List<inventoryClass.creat>>();
                creats = creats.Where(c => c.類型.Contains("覆盤")).ToList();
                returnData.Data = creats;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Method = "creat_get_by_CT_TIME_ST_END";
                returnData.Result = $"取得盤點資料成功!";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 以建表日搜尋盤點單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 建表日期 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "2023/10/26"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_get_by_CT_TIME")]
        [HttpPost]
        public string creat_get_by_CT_TIME([FromBody] returnData returnData)
        {
            try
            {
                GET_init(returnData);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
                inventoryClass.creat creat = returnData.Data.ObjToClass<inventoryClass.creat>();
                if (returnData.Value.Check_Date_String() == false)
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }
                sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
                list_inventory_creat = list_inventory_creat.GetRowsInDate((int)enum_盤點單號.建表時間, returnData.Value.StringToDateTime());
                if (returnData.Value == "1")
                {
                    returnData = Function_Get_inventory_creat(sys_serverSettingClasses[0], returnData.TableName, list_inventory_creat, false);
                }
                else
                {
                    returnData = Function_Get_inventory_creat(sys_serverSettingClasses[0], returnData.TableName, list_inventory_creat);
                }
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得盤點資料成功!";
                returnData.Method = "creat_get_by_CT_TIME";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 以盤點單號更新盤點單開始時間
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "20231026-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_update_startime_by_IC_SN")]
        [HttpPost]
        public string creat_update_startime_by_IC_SN([FromBody] returnData returnData)
        {
            try
            {

                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
                inventoryClass.creat creat = returnData.Data.ObjToClass<inventoryClass.creat>();
                string json_out = creat_get_by_IC_SN(returnData);
                returnData = json_out.JsonDeserializet<returnData>();
                if (returnData.Code < 0)
                {
                    returnData.Method = "creat_update_startime_by_CT_TIME";
                    return returnData.JsonSerializationt();
                }
                List<inventoryClass.creat> creats = returnData.Data.ObjToListClass<inventoryClass.creat>();
                if (creats.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Result = $"取得盤點資料失敗!";
                    returnData.Method = "creat_update_startime_by_CT_TIME";

                    return returnData.JsonSerializationt(true);
                }
                creat = creats[0];
                if (creat.盤點開始時間 == DateTime.MaxValue.ToDateTimeString())
                {
                    creat.盤點開始時間 = DateTime.Now.ToDateTimeString();
                }

                creat.盤點狀態 = "盤點中";
                object[] value = creat.ClassToSQL<inventoryClass.creat, enum_盤點單號>();
                List<object[]> list_value = new List<object[]>();
                list_value.Add(value);
                sQLControl_inventory_creat.UpdateByDefulteExtra(null, list_value);
                returnData.Code = 200;
                returnData.Data = creat;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"更新盤點開始時間成功!";
                returnData.Method = "creat_update_startime_by_CT_TIME";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 以盤點單號搜尋盤點單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "20231026-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_get_by_IC_SN")]
        [HttpPost]
        public string creat_get_by_IC_SN([FromBody] returnData returnData)
        {
            try
            {
                GET_init(returnData);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
                //inventoryClass.creat creat = returnData.Data.ObjToClass<inventoryClass.creat>();

                if (returnData.Value.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"搜尋內容空白!";
                    return returnData.JsonSerializationt();
                }
                string command = $"SELECT * FROM {DB}.inventory_creat WHERE {enum_盤點單號.盤點單號.GetEnumName()} = '{returnData.Value}';";
                DataTable dataTable = sQLControl_inventory_creat.WtrteCommandAndExecuteReader(command);

                List<object[]> list_inventory_creat = dataTable.DataTableToRowList();
                //List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
                //list_inventory_creat = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, returnData.Value);
                if (list_inventory_creat.Count == 0)
                {
                    returnData.Code = -5;
                    returnData.Result = $"查無此單號資料[{returnData.Value}]!";
                    return returnData.JsonSerializationt();
                }
                MED_pageController mED_PageController = new MED_pageController();

                returnData = Function_Get_inventory_creat(sys_serverSettingClasses[0], returnData.TableName, list_inventory_creat);
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得盤點資料成功!";
                returnData.Method = "creat_get_by_IC_SN";

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
        /// 以合併單號搜尋覆盤單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "20231026-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_get_by_INVC")]
        [HttpPost]
        public string creat_get_by_INVC([FromBody] returnData returnData)
        {
            try
            {
                GET_init(returnData);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                

                if (returnData.Value.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"搜尋內容空白!";
                    return returnData.JsonSerializationt();
                }
                string command = $"SELECT * FROM {DB}.inventory_creat WHERE {enum_盤點單號.合併單號.GetEnumName()} = '{returnData.Value}';";
                DataTable dataTable = sQLControl_inventory_creat.WtrteCommandAndExecuteReader(command);

                List<object[]> list_inventory_creat = dataTable.DataTableToRowList();
                
                if (list_inventory_creat.Count == 0)
                {
                    returnData.Code = -5;
                    returnData.Result = $"查無此單號資料[{returnData.Value}]!";
                    return returnData.JsonSerializationt();
                }

                returnData = Function_Get_inventory_creat(sys_serverSettingClasses[0], returnData.TableName, list_inventory_creat);
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得盤點資料成功!";
                returnData.Method = "creat_get_by_INVC";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 快速創建盤點單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單名稱 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "測試盤點單"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_quick_add")]
        [HttpPost]
        public string creat_quick_add([FromBody] returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端")[0];
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                string Server = sys_serverSettingClasses_buf[0].Server;
                string DB = sys_serverSettingClasses_buf[0].DBName;
                string UserName = sys_serverSettingClasses_buf[0].User;
                string Password = sys_serverSettingClasses_buf[0].Password;
                uint Port = (uint)sys_serverSettingClasses_buf[0].Port.StringToInt32();

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);

                string creatSN = $"Q{DateTime.Now.ToDateTinyString()}-{returnData.Value}";
                List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
                List<object[]> list_inventory_creat_buf = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, creatSN);
                if (list_inventory_creat_buf.Count == 0)
                {
                    MED_pageController mED_PageController = new MED_pageController();
                    returnData returnData_med = new returnData();
                    returnData_med.Server = sys_serverSettingClasses_med.Server;
                    returnData_med.DbName = sys_serverSettingClasses_med.DBName;
                    returnData_med.TableName = "medicine_page_cloud";
                    returnData_med.UserName = sys_serverSettingClasses_med.User;
                    returnData_med.Password = sys_serverSettingClasses_med.Password;
                    returnData_med.Port = sys_serverSettingClasses_med.Port.StringToUInt32();
                    returnData_med = mED_PageController.Get(returnData_med).JsonDeserializet<returnData>();

                    returnData_med = mED_PageController.get_by_apiserver(returnData_med).JsonDeserializet<returnData>();
                    List<medClass> medClasses = returnData_med.Data.ObjToListClass<medClass>();

                    deviceController deviceController = new deviceController();
                    sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");
                    List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
                    if (sys_serverSettingClasses_buf.Count > 0)
                    {
                        deviceBasics = deviceController.Function_Get_device(sys_serverSettingClasses_buf[0]);
                    }


                    inventoryClass.creat creat = returnData.Data.ObjToClass<inventoryClass.creat>();
                    creat.盤點單號 = creatSN;
                    creat.盤點名稱 = $"{returnData.Value}";
                    for (int i = 0; i < medClasses.Count; i++)
                    {
                        //if (medClasses[i].開檔狀態.StringIsEmpty() == false)
                        //{
                        //    if (medClasses[i].開檔狀態 != enum_開檔狀態.開檔中.GetEnumName())
                        //    {
                        //        continue;
                        //    }
                        //}
                        inventoryClass.content content = new inventoryClass.content();
                        content.藥品碼 = medClasses[i].藥品碼;
                        content.藥品名稱 = medClasses[i].藥品名稱;
                        content.中文名稱 = medClasses[i].中文名稱;
                        content.料號 = medClasses[i].料號;
                        content.藥品條碼1 = medClasses[i].藥品條碼1;
                        content.藥品條碼2 = medClasses[i].藥品條碼2;
                        content.包裝單位 = medClasses[i].包裝單位;
                        content.理論值 = "0";
                        List<DeviceBasic> deviceBasic_buf = deviceBasics.SortByCode(content.藥品碼);
                        if (deviceBasic_buf.Count > 0)
                        {
                            content.理論值 = deviceBasic_buf[0].Inventory;
                            if (deviceBasic_buf[0].Inventory.StringToInt32() > 0)
                            {

                            }
                        }



                        creat.Contents.Add(content);
                    }
                    if (creat.Contents.Count == 0)
                    {
                        returnData.Code = -6;
                        returnData.Value = "無盤點資料可新增!";
                        return returnData.JsonSerializationt();
                    }
                    returnData.Data = creat;
                    returnData.Method = "creat_auto_add";
                    returnData.Value = creatSN;
                    creat_add(returnData);
                    return returnData.JsonSerializationt();
                }
                else
                {
                    inventoryClass.creat creat = new inventoryClass.creat();
                    creat.盤點單號 = creatSN;
                    returnData.Data = creat;
                    returnData.Value = creatSN;
                    returnData.Code = 200;
                    return returnData.JsonSerializationt();
                }


            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 創建盤點單(盤點單號自訂)
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單名稱 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///        "IC_NAME": "測試盤點",
        ///        "IC_SN": "Q202230101-測試盤點單",
        ///        "CT": "",
        ///        "NOTE": "",
        ///        "Contents": 
        ///         [
        ///            {
        ///                "CODE": "220302IHYA",
        ///                "SKDIACODE": "",
        ///                "CHT_NAME": "Hyaluronate Sodium",
        ///                "NAME": "(申報)Hyalgan膝爾康 關節腔注射劑",
        ///                "PAKAGE": "Syri",
        ///                "BARCODE1": "",
        ///                "BARCODE2": "[]",
        ///                "START_QTY": "0",
        ///                "END_QTY": "0",
        ///                "ADD_TIME": "2023/10/30 20:41:53",
        ///                "NOTE": "",
        ///                "Sub_content": []
        ///            },
        ///            {
        ///                "CODE": "220IHYA",
        ///                "SKDIACODE": "",
        ///                "CHT_NAME": "Hyaluronate Sodium",
        ///                "NAME": "(申報)Hyalgan膝爾康 關節腔注射劑",
        ///                "PAKAGE": "",
        ///                "BARCODE1": "",
        ///                "BARCODE2": "[]",
        ///                "START_QTY": "0",
        ///                "END_QTY": "0",
        ///                "ADD_TIME": "2023/10/30 20:41:53",
        ///                "NOTE": "",
        ///                "Sub_content": []
        ///             }
        ///         ]       
        ///     }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_add")]
        [HttpPost]
        public string creat_add([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            GET_init(returnData);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
            inventoryClass.creat creat = returnData.Data.ObjToClass<inventoryClass.creat>();

            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
            List<object[]> list_inventory_creat_buf = new List<object[]>();



            if (creat == null)
            {
                returnData.Code = -5;
                returnData.Result += $"Data 資料錯誤 \n";
                return returnData.JsonSerializationt();
            }

            list_inventory_creat_buf = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, creat.盤點單號);
            if (list_inventory_creat_buf.Count > 0)
            {
                returnData.Code = -6;
                returnData.Result += $"盤點單號: {creat.盤點單號} 已存在,請刪除後再建立! \n";
                return returnData.JsonSerializationt();
            }
            creat.GUID = Guid.NewGuid().ToString();
            creat.建表時間 = DateTime.Now.ToDateTimeString();
            creat.盤點開始時間 = DateTime.MaxValue.ToDateTimeString();
            creat.盤點結束時間 = DateTime.MaxValue.ToDateTimeString();

            List<object[]> list_inventory_creat_add = new List<object[]>();
            List<object[]> list_inventory_content_add = new List<object[]>();
            List<object[]> list_inventory_sub_content_add = new List<object[]>();
            object[] value;
            value = new object[new enum_盤點單號().GetLength()];

            value[(int)enum_盤點單號.GUID] = creat.GUID;
            value[(int)enum_盤點單號.盤點單號] = creat.盤點單號;
            value[(int)enum_盤點單號.盤點名稱] = creat.盤點名稱;
            value[(int)enum_盤點單號.建表人] = creat.建表人;
            value[(int)enum_盤點單號.建表時間] = creat.建表時間;
            value[(int)enum_盤點單號.盤點開始時間] = creat.盤點開始時間;
            value[(int)enum_盤點單號.盤點結束時間] = creat.盤點結束時間;
            value[(int)enum_盤點單號.預設盤點人] = creat.預設盤點人;
            value[(int)enum_盤點單號.盤點狀態] = "等待盤點";
            value[(int)enum_盤點單號.備註] = creat.備註;

            list_inventory_creat_add.Add(value);

            for (int i = 0; i < creat.Contents.Count; i++)
            {
                value = new object[new enum_盤點內容().GetLength()];
                creat.Contents[i].GUID = Guid.NewGuid().ToString();
                creat.Contents[i].新增時間 = DateTime.Now.ToDateTimeString();
                creat.Contents[i].Master_GUID = creat.GUID;
                creat.Contents[i].盤點單號 = creat.盤點單號;
                value[(int)enum_盤點內容.GUID] = creat.Contents[i].GUID;
                value[(int)enum_盤點內容.Master_GUID] = creat.Contents[i].Master_GUID;
                value[(int)enum_盤點內容.序號] = creat.Contents[i].序號;
                value[(int)enum_盤點內容.儲位名稱] = creat.Contents[i].儲位名稱;
                value[(int)enum_盤點內容.藥品碼] = creat.Contents[i].藥品碼;
                value[(int)enum_盤點內容.料號] = creat.Contents[i].料號;
                value[(int)enum_盤點內容.藥品條碼1] = creat.Contents[i].藥品條碼1;
                value[(int)enum_盤點內容.藥品條碼2] = creat.Contents[i].藥品條碼2;
                value[(int)enum_盤點內容.盤點單號] = creat.Contents[i].盤點單號;
                value[(int)enum_盤點內容.理論值] = creat.Contents[i].理論值;
                value[(int)enum_盤點內容.新增時間] = creat.Contents[i].新增時間;
                value[(int)enum_盤點內容.備註] = creat.Contents[i].備註;
                for (int k = 0; k < creat.Contents[i].Sub_content.Count; k++)
                {
                    object[] sub_value = new object[new enum_盤點明細().GetLength()];
                    sub_value = creat.Contents[i].Sub_content[k].ClassToSQL<inventoryClass.sub_content, enum_盤點明細>();
                    sub_value[(int)enum_盤點明細.GUID] = Guid.NewGuid().ToString();
                    sub_value[(int)enum_盤點明細.Master_GUID] = creat.Contents[i].GUID;
                    sub_value[(int)enum_盤點明細.操作時間] = DateTime.Now.ToDateTimeString_6();
                    sub_value[(int)enum_盤點明細.盤點單號] = creat.盤點單號;
                    list_inventory_sub_content_add.Add(sub_value);
                }
                list_inventory_content_add.Add(value);
            }
            sQLControl_inventory_creat.AddRows(null, list_inventory_creat_add);
            sQLControl_inventory_content.AddRows(null, list_inventory_content_add);
            sQLControl_inventory_sub_content.AddRows(null, list_inventory_sub_content_add);
            returnData.Data = creat;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "creat_add";

            returnData.Result = $"成功加入新盤點單! 共{list_inventory_content_add.Count}筆資料";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 創建驗收單(自動填入驗收單號)
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 驗收單名稱 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///   "ValueAry": [
        ///     "medGroup=大瓶藥;冷藏藥",
        ///     "control=1;3", // N,1,2,3,4
        ///     "medType=口服;針劑"
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為驗收單結構</returns>

        [Route("creat_auto_add")]
        [HttpPost]
        public async Task<string> creat_auto_add([FromBody] returnData returnData)
        {
            try
            {
                string GetVal(string key) =>
                   returnData.ValueAry.FirstOrDefault(x => x.StartsWith($"{key}=", StringComparison.OrdinalIgnoreCase))
                    ?.Split('=')[1];
                string medGroup = GetVal("medGroup") ?? "";
                string control = GetVal("control") ?? "";
                string medType = GetVal("medType") ?? "";
                GET_init(returnData);
                
                
                (string Server, string DB, string UserName, string Password, uint Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

             
                returnData returnData_GET_new_IC_SN = this.GET_new_IC_SN(returnData).JsonDeserializet<returnData>();
                string str_IC_SN = returnData_GET_new_IC_SN.Value;               

                List<medClass> medClasses = new List<medClass>();
                if (medGroup.StringIsEmpty() == false)
                {
                    returnData returnData_group = await new medGroup().get_groups_by_name(medGroup);
                    if(returnData_group == null || returnData_group.Code != 200)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"藥品群組取得失敗";
                        return returnData.JsonSerializationt();
                    }
                    List<medGroupClass>  medGroupClasses = returnData_group.Data.ObjToClass<List<medGroupClass>>();
                    foreach(var medGroupClasses_buf in medGroupClasses)
                    {
                        if(medGroupClasses_buf.MedClasses != null && medGroupClasses_buf.MedClasses.Count > 0)
                        {
                            medClasses.AddRange(medGroupClasses_buf.MedClasses);
                        }
                    }
                    medClasses = medClasses.GroupBy(x => x.GUID).Select(g => g.First()).ToList();
                   
                }
                if(control.StringIsEmpty() == false)
                {
                    List<string> control_ = control.Split(';').ToList();
                    if (medClasses == null || medClasses.Count == 0)
                    {
                        returnData returnData_med = await new MED_pageController().get_med_cloud();
                        medClasses = returnData_med.Data.ObjToListClass<medClass>();
                    }
                    medClasses = medClasses.Where(item => control_.Contains(item.管制級別)).ToList();
                }
                if (medType.StringIsEmpty() == false)
                {
                    List<string> medType_ = medType.Split(';').ToList();
                    if (medClasses == null || medClasses.Count == 0)
                    {
                        returnData returnData_med = await new MED_pageController().get_med_cloud();
                        medClasses = returnData_med.Data.ObjToListClass<medClass>();
                    }
                    medClasses = medClasses.Where(item => medType_.Contains(item.類別)).ToList();
                }
                if (medGroup.StringIsEmpty()　&& control.StringIsEmpty() && medType.StringIsEmpty() 　)
                {
                    returnData returnData_med = await new MED_pageController().get_med_cloud();
                    medClasses = returnData_med.Data.ObjToListClass<medClass>();
                }
                inventoryClass.creat creat = returnData.Data.ObjToClass<inventoryClass.creat>();
                creat.盤點單號 = str_IC_SN;
                for (int i = 0; i < medClasses.Count; i++)
                {
                                        
                    inventoryClass.content content = new inventoryClass.content();
                    content.藥品碼 = medClasses[i].藥品碼;
                    content.藥品名稱 = medClasses[i].藥品名稱;
                    content.中文名稱 = medClasses[i].中文名稱;
                    content.料號 = medClasses[i].料號;
                    content.藥品條碼1 = medClasses[i].藥品條碼1;
                    content.藥品條碼2 = medClasses[i].藥品條碼2;
                    content.包裝單位 = medClasses[i].包裝單位;
                    content.理論值 = "0";
                  
                    creat.Contents.Add(content);
                }
                if (creat.Contents.Count == 0)
                {
                    returnData.Code = -6;
                    returnData.Value = "無盤點資料可新增!";
                    return returnData.JsonSerializationt();
                }
                returnData.Data = creat;
                returnData.Method = "creat_auto_add";

                return creat_add(returnData);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以盤點單號鎖定盤點單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "20231026-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_lock_by_IC_SN")]
        [HttpPost]
        public string creat_lock([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
            if (sys_serverSettingClasses.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return returnData.JsonSerializationt();
            }
            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value空白,請輸入盤點單號!";
                return returnData.JsonSerializationt();
            }
            string Server = sys_serverSettingClasses[0].Server;
            string DB = sys_serverSettingClasses[0].DBName;
            string UserName = sys_serverSettingClasses[0].User;
            string Password = sys_serverSettingClasses[0].Password;
            uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
      
            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
            List<object[]> list_inventory_creat_buf = new List<object[]>();
            list_inventory_creat_buf = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, returnData.Value);
            if (list_inventory_creat_buf.Count == 0)
            {
                returnData.Code = -5;
                returnData.Result += $"找無此盤點單號!";
                return returnData.JsonSerializationt();
            }
            list_inventory_creat_buf[0][(int)enum_盤點單號.盤點狀態] = "鎖定";
            list_inventory_creat_buf[0][(int)enum_盤點單號.盤點結束時間] = DateTime.Now.ToDateTimeString();
            sQLControl_inventory_creat.UpdateByDefulteExtra(null, list_inventory_creat_buf);

            inventoryClass.creat creat = returnData.Data.ObjToClass<inventoryClass.creat>();
            creat.GUID = list_inventory_creat_buf[0][(int)enum_盤點單號.GUID].ObjectToString();
            creat.盤點單號 = returnData.Value;
            creat.盤點狀態 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點狀態].ObjectToString();
            creat.建表人 = list_inventory_creat_buf[0][(int)enum_盤點單號.建表人].ObjectToString();
            creat.建表時間 = list_inventory_creat_buf[0][(int)enum_盤點單號.建表時間].ToDateTimeString();
            creat.盤點開始時間 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點開始時間].ToDateTimeString();
            creat.盤點結束時間 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點結束時間].ObjectToString();
            returnData.Code = 200;
            returnData.Value = "盤點單鎖定成功!";
            returnData.Data = creat;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "creat_lock_by_IC_SN";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以盤點單號解鎖盤點單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "20231026-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_unlock_by_IC_SN")]
        [HttpPost]
        public string creat_unlock([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
            if (sys_serverSettingClasses.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return returnData.JsonSerializationt();
            }
            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -5;
                returnData.Result = "returnData.Value空白,請輸入盤點單號!";
                return returnData.JsonSerializationt();
            }
            string Server = sys_serverSettingClasses[0].Server;
            string DB = sys_serverSettingClasses[0].DBName;
            string UserName = sys_serverSettingClasses[0].User;
            string Password = sys_serverSettingClasses[0].Password;
            uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
        
            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
            List<object[]> list_inventory_creat_buf = new List<object[]>();
            list_inventory_creat_buf = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, returnData.Value);
            if (list_inventory_creat_buf.Count == 0)
            {
                returnData.Code = -5;
                returnData.Result += $"找無此盤點單號!";
                return returnData.JsonSerializationt();
            }
            list_inventory_creat_buf[0][(int)enum_盤點單號.盤點狀態] = "等待盤點";
            sQLControl_inventory_creat.UpdateByDefulteExtra(null, list_inventory_creat_buf);

            inventoryClass.creat creat = new inventoryClass.creat();
            creat.GUID = list_inventory_creat_buf[0][(int)enum_盤點單號.GUID].ObjectToString();
            creat.盤點單號 = returnData.Value;
            creat.盤點狀態 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點狀態].ObjectToString();
            creat.建表人 = list_inventory_creat_buf[0][(int)enum_盤點單號.建表人].ObjectToString();
            creat.建表時間 = list_inventory_creat_buf[0][(int)enum_盤點單號.建表時間].ToDateTimeString();
            creat.盤點開始時間 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點開始時間].ToDateTimeString();
            creat.盤點結束時間 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點結束時間].ToDateTimeString();
            returnData.Code = 200;
            returnData.Value = "盤點單解鎖成功!";
            returnData.Data = creat;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "creat_unlock_by_IC_SN";

            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以盤點單號刪除盤點單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "20231026-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_delete_by_IC_SN")]
        [HttpPost]
        public string creat_delete_by_IC_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
            List<object[]> list_inventory_creat_buf = new List<object[]>();
            List<object[]> list_inventory_content = sQLControl_inventory_content.GetAllRows(null);
            List<object[]> list_inventory_content_buf = new List<object[]>();
            List<object[]> list_inventory_sub_content = sQLControl_inventory_sub_content.GetAllRows(null);
            List<object[]> list_inventory_sub_content_buf = new List<object[]>();

            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value空白,請輸入盤點單號!";
                return returnData.JsonSerializationt();
            }
       

            list_inventory_creat_buf = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, returnData.Value);
            list_inventory_content_buf = list_inventory_content.GetRows((int)enum_盤點內容.盤點單號, returnData.Value);
            list_inventory_sub_content_buf = list_inventory_sub_content.GetRows((int)enum_盤點明細.盤點單號, returnData.Value);

            inventoryClass.creat creat = new inventoryClass.creat();
            sQLControl_inventory_creat.DeleteExtra(null, list_inventory_creat_buf);
            sQLControl_inventory_content.DeleteExtra(null, list_inventory_content_buf);
            sQLControl_inventory_sub_content.DeleteExtra(null, list_inventory_sub_content_buf);
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"已將[{returnData.Value}]刪除!";
            returnData.Method = "creat_delete_by_IC_SN";

            return returnData.JsonSerializationt();


        }
        /// <summary>
        /// 以盤點單號取得盤點單是否鎖定
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "20231026-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("get_creat_Islocked_by_IC_SN")]
        [HttpPost]
        public string get_creat_Islocked_by_IC_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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
            if (returnData.Value.StringIsEmpty())
            {
                returnData.Code = -200;
                returnData.Result = $"Value 空白!";
                return returnData.JsonSerializationt();
            }
            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);

            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
            List<object[]> list_inventory_creat_buf = new List<object[]>();
            list_inventory_creat_buf = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, returnData.Value);
            if (list_inventory_creat_buf.Count == 0)
            {
                returnData.Code = -5;
                returnData.Result += $"找無此盤點單號!";
                return returnData.JsonSerializationt();
            }
            inventoryClass.creat creat = list_inventory_creat_buf[0].SQLToClass<inventoryClass.creat, enum_盤點單號>();

            returnData.Code = 200;
            returnData.Value = $"盤點單檢查鎖單成功! {creat.盤點狀態}";
            returnData.Data = creat.盤點狀態;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "get_creat_Islocked_by_IC_SN";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以GUID刪除盤點單內盤點藥品
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    [                 
        ///        {
        ///            "GUID": "a6f58a75-094a-411c-8f87-9804d46b78ea"
        ///        },
        ///        {
        ///            "GUID": "024e17eb-fa68-4495-92fa-d78ed753c23b"
        ///        }
        ///    ]
        ///  }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>無</returns>
        [Route("contents_delete_by_GUID")]
        [HttpPost]
        public string contents_delete_by_GUID([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
            if (returnData.Data == null)
            {
                returnData.Code = -5;
                returnData.Result = $"Data資料長度錯誤!";
                return returnData.JsonSerializationt();
            }
            List<inventoryClass.content> contents = returnData.Data.ObjToListClass<inventoryClass.content>();
            List<object> list_GUID = new List<object>();
            for (int i = 0; i < contents.Count; i++)
            {
                list_GUID.Add(contents[i].GUID);
            }
            sQLControl_inventory_content.DeleteExtra(null, enum_盤點內容.GUID.GetEnumName(), list_GUID);
            sQLControl_inventory_sub_content.DeleteExtra(null, enum_盤點明細.Master_GUID.GetEnumName(), list_GUID);
            returnData.Data = null;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"已刪除盤點內容共<{list_GUID.Count}>筆資料!";
            returnData.Method = "contents_delete_by_GUID";

            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 以GUID取得盤點單內盤點藥品
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///        "GUID": "a6f58a75-094a-411c-8f87-9804d46b78ea"         
        ///    }
        ///  }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data] :content資料結構 </returns>
        [Route("content_get_by_content_GUID")]
        [HttpPost]
        public string content_get_by_content_GUID([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);

            inventoryClass.content content = returnData.Data.ObjToClass<inventoryClass.content>();
            //if (returnData.Value.StringIsEmpty())
            //{
            //    returnData.Code = -5;
            //    returnData.Result = "輸入資料不得空白!";
            //    return returnData.JsonSerializationt();
            //}

            string GUID = content.GUID;
            returnData.Data = null;
            List<object[]> list_inventory_content_buf = sQLControl_inventory_content.GetRowsByDefult(null, (int)enum_盤點內容.GUID, GUID);

            if (list_inventory_content_buf.Count == 0)
            {
                returnData.Code = -1;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"查無盤點內容!";
                returnData.Method = "content_get_by_content_GUID";

                return returnData.JsonSerializationt();
            }
            else
            {

                MED_pageController mED_PageController = new MED_pageController();
                returnData returnData_med = new returnData();
                returnData_med.TableName = "medicine_page_cloud";
                returnData_med.ServerType = "網頁";
                returnData_med.ServerName = "Main";
                returnData_med.Value = list_inventory_content_buf[0][(int)enum_盤點內容.藥品碼].ObjectToString();
                string json_med = mED_PageController.get_by_code(returnData_med);
                returnData_med = json_med.JsonDeserializet<returnData>();



                content = list_inventory_content_buf[0].SQLToClass<inventoryClass.content, enum_盤點內容>();
                int 盤點量 = 0;
                List<object[]> list_inventory_sub_content_buf = sQLControl_inventory_sub_content.GetRowsByDefult(null, (int)enum_盤點明細.Master_GUID, content.GUID);
                for (int m = 0; m < list_inventory_sub_content_buf.Count; m++)
                {
                    inventoryClass.sub_content sub_Content = list_inventory_sub_content_buf[m].SQLToClass<inventoryClass.sub_content, enum_盤點明細>();

                    if (sub_Content.盤點量.StringIsInt32())
                    {
                        盤點量 += sub_Content.盤點量.StringToInt32();
                    }
                    content.Sub_content.Add(sub_Content);
                }
                content.盤點量 = 盤點量.ToString();

                if (returnData_med != null)
                {
                    if (returnData_med.Code == 200)
                    {
                        List<medClass> medClasses = returnData_med.Data.ObjToListClass<medClass>();
                        if (medClasses.Count > 0)
                        {
                            string 藥品名稱 = medClasses[0].藥品名稱;
                            string 中文名稱 = medClasses[0].中文名稱;
                            string 包裝單位 = medClasses[0].包裝單位;
                            content.藥品名稱 = 藥品名稱;
                            content.中文名稱 = 中文名稱;
                            content.包裝單位 = 包裝單位;
                            for (int i = 0; i < content.Sub_content.Count; i++)
                            {
                                content.Sub_content[i].藥品名稱 = 藥品名稱;
                                content.Sub_content[i].中文名稱 = 中文名稱;
                                content.Sub_content[i].包裝單位 = 包裝單位;
                                content.Sub_content[i].總量 = 盤點量.ToString();
                            }
                        }


                    }

                }

                content.Sub_content.Sort(new ICP_sub_content());
                returnData.Data = content;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得盤點內容成功!";
                returnData.Method = "content_get_by_content_GUID";

                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以GUID取得盤點單內盤點藥品中的明細
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///       "GUID": "4710df0c-5bfe-4c98-ac0e-46453ab8b043"
        ///    }
        ///  }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data] :sub_content陣列資料結構 </returns>
        [Route("sub_content_get_by_content_GUID")]
        [HttpPost]
        public string sub_content_get_by_content_GUID([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
            inventoryClass.content content = returnData.Data.ObjToClass<inventoryClass.content>();
            //if (returnData.Value.StringIsEmpty())
            //{
            //    returnData.Code = -5;
            //    returnData.Result = "輸入資料不得空白!";
            //    return returnData.JsonSerializationt();
            //}

            string GUID = content.GUID;
            returnData.Data = null;
            List<object[]> list_inventory_sub_content = sQLControl_inventory_sub_content.GetAllRows(null);
            List<object[]> list_inventory_sub_content_buf = new List<object[]>();
            list_inventory_sub_content_buf = list_inventory_sub_content.GetRows((int)enum_盤點明細.Master_GUID, GUID);
            List<inventoryClass.sub_content> sub_Contents = new List<inventoryClass.sub_content>();
            for (int i = 0; i < list_inventory_sub_content_buf.Count; i++)
            {
                inventoryClass.sub_content sub_Content = list_inventory_sub_content_buf[i].SQLToClass<inventoryClass.sub_content, enum_盤點明細>();
                sub_Contents.Add(sub_Content);
            }
            returnData.Data = sub_Contents;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"取得盤點明細成功!";
            returnData.Method = "sub_content_get_by_content_GUID";

            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 新增單筆盤點藥品中的明細且刪除原本明細
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///       "Master_GUID": "13a6625b-b7b2-43b0-ba59-7c451a4912e0",
        ///       "OP": "測試者",
        ///       "END_QTY": "56"
        ///    }
        ///  }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data] :sub_content資料結構 </returns>
        [Route("sub_content_add_single")]
        [HttpPost]
        public string sub_content_add_single([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
            List<object[]> list_inventory_content = sQLControl_inventory_content.GetAllRows(null);
            List<object[]> list_inventory_content_buf = new List<object[]>();
            List<object[]> list_add = new List<object[]>();
            inventoryClass.sub_content sub_content = returnData.Data.ObjToClass<inventoryClass.sub_content>();
            string Master_GUID = sub_content.Master_GUID;

            sQLControl_inventory_sub_content.DeleteByDefult(null, enum_盤點明細.Master_GUID.GetEnumName(), Master_GUID);

            list_inventory_content_buf = list_inventory_content.GetRows((int)enum_盤點內容.GUID, Master_GUID);
            if (list_inventory_content_buf.Count > 0)
            {
                object[] value = new object[new enum_盤點明細().GetLength()];
                value[(int)enum_盤點明細.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_盤點明細.藥品碼] = list_inventory_content_buf[0][(int)enum_盤點內容.藥品碼];
                value[(int)enum_盤點明細.料號] = list_inventory_content_buf[0][(int)enum_盤點內容.料號];

                value[(int)enum_盤點明細.盤點單號] = list_inventory_content_buf[0][(int)enum_盤點內容.盤點單號];
                value[(int)enum_盤點明細.藥品條碼1] = list_inventory_content_buf[0][(int)enum_盤點內容.藥品條碼1];
                value[(int)enum_盤點明細.藥品條碼2] = list_inventory_content_buf[0][(int)enum_盤點內容.藥品條碼2];

                value[(int)enum_盤點明細.Master_GUID] = Master_GUID;
                value[(int)enum_盤點明細.效期] = DateTime.MaxValue.ToDateString();
                value[(int)enum_盤點明細.批號] = "";
                value[(int)enum_盤點明細.盤點量] = sub_content.盤點量;
                value[(int)enum_盤點明細.操作人] = sub_content.操作人;
                value[(int)enum_盤點明細.操作時間] = DateTime.Now.ToDateTimeString();
                value[(int)enum_盤點明細.狀態] = "未鎖定";

                sub_content.藥品碼 = value[(int)enum_盤點明細.藥品碼].ObjectToString();
                sub_content.料號 = value[(int)enum_盤點明細.料號].ObjectToString();

                sub_content.盤點單號 = value[(int)enum_盤點明細.盤點單號].ObjectToString();
                sub_content.藥品條碼1 = value[(int)enum_盤點明細.藥品條碼1].ObjectToString();
                sub_content.藥品條碼2 = value[(int)enum_盤點明細.藥品條碼1].ObjectToString();
                sub_content.Master_GUID = value[(int)enum_盤點明細.Master_GUID].ObjectToString();
                sub_content.效期 = value[(int)enum_盤點明細.效期].ObjectToString();
                sub_content.操作人 = value[(int)enum_盤點明細.操作人].ObjectToString();
                sub_content.操作時間 = value[(int)enum_盤點明細.操作時間].ObjectToString();
                sub_content.狀態 = value[(int)enum_盤點明細.狀態].ObjectToString();

                list_add.Add(value);
            }
            sQLControl_inventory_sub_content.AddRows(null, list_add);
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"新增批效成功!";
            returnData.Data = sub_content;
            returnData.Method = "sub_content_add_single";

            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 新增單筆盤點藥品中的明細
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///       "Master_GUID": "13a6625b-b7b2-43b0-ba59-7c451a4912e0",
        ///       "OP": "測試者",
        ///       "END_QTY": "56"
        ///    }
        ///  }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data] :sub_content資料結構 </returns>
        [Route("sub_content_add")]
        [HttpPost]
        public async Task<string> sub_content_add([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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
            inventoryClass.sub_content sub_content = returnData.Data.ObjToClass<inventoryClass.sub_content>();
            string Master_GUID = sub_content.Master_GUID;

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
            List<object[]> list_inventory_content = sQLControl_inventory_content.GetRowsByDefult(null, (int)enum_盤點內容.GUID, Master_GUID);

            if (list_inventory_content.Count == 0)
            {
                returnData.Code = -5;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"找無資料!";
                returnData.Method = "sub_content_add";
                returnData.Data = null;
                return returnData.JsonSerializationt();
            }
            medClass medClass = await MED_pageController.Get_med_cloudAsync(sys_serverSettingClasses[0], list_inventory_content[0][(int)enum_盤點內容.藥品碼].ToString());
            if(medClass == null || medClass.開檔狀態.Contains("關檔中"))
            {
                returnData.Code = -5;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"此藥品關檔中!";
                returnData.Method = "sub_content_add";
                returnData.Data = null;
                return returnData.JsonSerializationt();
            }
            
            if (sub_content.效期.Check_Date_String() == false)
            {
                sub_content.效期 = "1911-01-01";
            }

            object[] value = new object[new enum_盤點明細().GetLength()];
            value[(int)enum_盤點明細.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_盤點明細.藥品碼] = list_inventory_content[0][(int)enum_盤點內容.藥品碼];
            value[(int)enum_盤點明細.料號] = list_inventory_content[0][(int)enum_盤點內容.料號];

            value[(int)enum_盤點明細.盤點單號] = list_inventory_content[0][(int)enum_盤點內容.盤點單號];
            //value[(int)enum_盤點明細.藥品條碼1] = list_inventory_content[0][(int)enum_盤點內容.藥品條碼1];
            //value[(int)enum_盤點明細.藥品條碼1] = list_inventory_content[0][(int)enum_盤點內容.藥品條碼2];

            value[(int)enum_盤點明細.Master_GUID] = Master_GUID;
            value[(int)enum_盤點明細.效期] = sub_content.效期;
            value[(int)enum_盤點明細.批號] = sub_content.批號;
            value[(int)enum_盤點明細.盤點量] = sub_content.盤點量;
            value[(int)enum_盤點明細.操作人] = sub_content.操作人;
            value[(int)enum_盤點明細.操作時間] = DateTime.Now.ToDateTimeString();
            value[(int)enum_盤點明細.狀態] = "未鎖定";


            await sQLControl_inventory_sub_content.AddRowAsync(null, value);

            inventoryClass.content content = new inventoryClass.content();
            content.GUID = Master_GUID;
            returnData.Data = content;
            string json_content = content_get_by_content_GUID(returnData);
            returnData = json_content.JsonDeserializet<returnData>();
            if (returnData == null)
            {
                returnData.Code = -5;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"搜尋content資料錯誤!";
                returnData.Method = "sub_content_add";
                returnData.Data = null;
                return returnData.JsonSerializationt();
            }
            if (returnData.Code < 0)
            {
                returnData.TimeTaken = myTimer.ToString();
                returnData.Method = "sub_content_add";
                returnData.Data = null;
                return returnData.JsonSerializationt();
            }

            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"新增批效成功!";
            returnData.Method = "sub_content_add";
            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 以GUID刪除盤點藥品中的明細
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    [                 
        ///        {
        ///            "GUID": "a6f58a75-094a-411c-8f87-9804d46b78ea",
        ///            "Master_GUID": "13a6625b-b7b2-43b0-ba59-7c451a4912e0"     
        ///        },
        ///        {
        ///            "GUID": "024e17eb-fa68-4495-92fa-d78ed753c23b",
        ///            "Master_GUID": "13a6625b-b7b2-43b0-ba59-7c451a4912e0"
        ///        }
        ///    ]
        ///  }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data] :content資料結構 </returns>
        [Route("sub_contents_delete_by_GUID")]
        [HttpPost]
        public string sub_contents_delete_by_GUID([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
            List<inventoryClass.sub_content> sub_contents = returnData.Data.ObjToListClass<inventoryClass.sub_content>();
            List<object> list_GUID = new List<object>();
            if (sub_contents.Count == 0)
            {
                returnData.Code = -5;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"搜尋sub_content資料錯誤!";
                returnData.Method = "sub_contents_delete_by_GUID";
                returnData.Data = null;
                return returnData.JsonSerializationt();
            }
            for (int i = 0; i < sub_contents.Count; i++)
            {
                list_GUID.Add(sub_contents[i].GUID);
            }
            string Master_GUID = sub_contents[0].Master_GUID;
            sQLControl_inventory_sub_content.DeleteExtra(null, enum_盤點明細.GUID.GetEnumName(), list_GUID);

            inventoryClass.content content = new inventoryClass.content();
            content.GUID = Master_GUID;
            returnData.Data = content;
            string json_content = content_get_by_content_GUID(returnData);
            returnData = json_content.JsonDeserializet<returnData>();
            if (returnData == null)
            {
                returnData.Code = -5;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"搜尋content資料錯誤!";
                returnData.Method = "sub_contents_delete_by_GUID";
                returnData.Data = null;
                return returnData.JsonSerializationt();
            }
            if (returnData.Code < 0)
            {
                returnData.TimeTaken = myTimer.ToString();
                returnData.Method = "sub_contents_delete_by_GUID";
                returnData.Data = null;
                return returnData.JsonSerializationt();
            }


            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"已刪除盤點明細共<{list_GUID.Count}>筆資料!";
            returnData.Method = "sub_contents_delete_by_GUID";

            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 以盤點單號寫入預設盤點人
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        "DEFAULT_OP" : "王曉明,張大同,克里斯"
        ///     },
        ///     "Value" : "20240104-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_update_default_op_by_IC_SN")]
        [HttpPost]
        public string creat_update_default_op_by_IC_SN([FromBody] returnData returnData)
        {
            try
            {

                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
                inventoryClass.creat creat_temp = returnData.Data.ObjToClass<inventoryClass.creat>();
                inventoryClass.creat creat = new inventoryClass.creat();
                string json_out = creat_get_by_IC_SN(returnData);
                returnData = json_out.JsonDeserializet<returnData>();
                if (returnData.Code < 0)
                {
                    returnData.Method = "creat_update_default_op_by_IC_SN";
                    return returnData.JsonSerializationt();
                }
                List<inventoryClass.creat> creats = returnData.Data.ObjToListClass<inventoryClass.creat>();
                if (creats.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Result = $"取得盤點資料失敗!";
                    returnData.Method = "creat_update_default_op_by_IC_SN";

                    return returnData.JsonSerializationt(true);
                }
                creat = creats[0];
          
                creat.預設盤點人 = creat_temp.預設盤點人;
                object[] value = creat.ClassToSQL<inventoryClass.creat, enum_盤點單號>();
                List<object[]> list_value = new List<object[]>();
                list_value.Add(value);
                sQLControl_inventory_creat.UpdateByDefulteExtra(null, list_value);
                returnData.Code = 200;
                returnData.Data = creat;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"更新預設盤點人成功!";
                returnData.Method = "creat_update_default_op_by_IC_SN";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 以盤點單號取得預設盤點人
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "20240104-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("creat_get_default_op_by_IC_SN")]
        [HttpPost]
        public string creat_get_default_op_by_IC_SN([FromBody] returnData returnData)
        {
            try
            {

                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
                inventoryClass.creat creat_temp = returnData.Data.ObjToClass<inventoryClass.creat>();
                inventoryClass.creat creat = new inventoryClass.creat();
                string json_out = creat_get_by_IC_SN(returnData);
                returnData = json_out.JsonDeserializet<returnData>();
                if (returnData.Code < 0)
                {
                    returnData.Method = "creat_get_default_op_by_IC_SN";
                    return returnData.JsonSerializationt();
                }
                List<inventoryClass.creat> creats = returnData.Data.ObjToListClass<inventoryClass.creat>();
                if (creats.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Result = $"取得盤點資料失敗!";
                    returnData.Method = "creat_get_default_op_by_IC_SN";

                    return returnData.JsonSerializationt(true);
                }
                creat = creats[0];
            
                object[] value = creat.ClassToSQL<inventoryClass.creat, enum_盤點單號>();
                List<object[]> list_value = new List<object[]>();
                list_value.Add(value);
                sQLControl_inventory_creat.UpdateByDefulteExtra(null, list_value);
                returnData.Code = 200;
                returnData.Data = creat;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得預設盤點人成功!";
                returnData.Method = "creat_get_default_op_by_IC_SN";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 以盤點單號新增預藥品
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///    "Data": 
        ///    {                 
        ///        "Contents": 
        ///         [
        ///            {
        ///                "CODE": "220302IHYA",
        ///                "SKDIACODE": "",
        ///                "CHT_NAME": "Hyaluronate Sodium",
        ///                "NAME": "(申報)Hyalgan膝爾康 關節腔注射劑",
        ///                "PAKAGE": "Syri",
        ///                "BARCODE1": "",
        ///                "BARCODE2": "[]",
        ///                "START_QTY": "0",
        ///                "END_QTY": "0",
        ///                "NOTE": "",
        ///                "Sub_content": []
        ///            },
        ///            {
        ///                "CODE": "220IHYA",
        ///                "SKDIACODE": "",
        ///                "CHT_NAME": "Hyaluronate Sodium",
        ///                "NAME": "(申報)Hyalgan膝爾康 關節腔注射劑",
        ///                "PAKAGE": "",
        ///                "BARCODE1": "",
        ///                "BARCODE2": "[]",
        ///                "START_QTY": "0",
        ///                "END_QTY": "0",
        ///                "NOTE": "",
        ///                "Sub_content": []
        ///             }
        ///         ]       
        ///     },
        ///     "Value" : "20240104-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為盤點單結構</returns>
        [Route("content_add_by_IC_SN")]
        [HttpPost]
        public string content_add_by_IC_SN([FromBody] returnData returnData)
        {
            try
            {

                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
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

                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
                inventoryClass.creat creat_temp = returnData.Data.ObjToClass<inventoryClass.creat>();
                if (creat_temp == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data不是盤點資料結構";
                    return returnData.JsonSerializationt();
                }
                List<inventoryClass.content> contents = creat_temp.Contents;
                string SN = returnData.Value;
                if(SN.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = $"return.Value空白!";
                    return returnData.JsonSerializationt();
                }
                if(contents.Count == 0 || contents == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data內無藥品可新增";
                    return returnData.JsonSerializationt();
                }
                string json_out = creat_get_by_IC_SN(returnData);
                returnData = json_out.JsonDeserializet<returnData>();
                if (returnData.Code < 0)
                {
                    returnData.Method = "content_add_by_IC_SN";
                    return returnData.JsonSerializationt();
                }
                List<inventoryClass.creat> creats = returnData.Data.ObjToListClass<inventoryClass.creat>();
                if (creats.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Result = $"取得盤點資料失敗!";
                    returnData.Method = "content_add_by_IC_SN";

                    return returnData.JsonSerializationt(true);
                }
                inventoryClass.creat creat = creats[0];

                for(int i = 0; i < contents.Count; i++)
                {
                    creat.ContentAdd(contents[i]);
                }
                List<object[]> list_content = sQLControl_inventory_content.GetRowsByDefult(null, (int)enum_盤點內容.盤點單號, SN);
                List<object[]> list_content_buf = new List<object[]>();
                List<object[]> list_content_add = new List<object[]>();
                for (int i = 0; i < contents.Count; i++)
                {
                    string 藥碼 = contents[i].藥品碼;
                    list_content_buf = list_content.GetRows((int)enum_盤點內容.藥品碼, 藥碼);
                    if(list_content_buf.Count == 0)
                    {
                        object[] value = contents[i].ClassToSQL<inventoryClass.content, enum_盤點內容>();
                        value[(int)enum_盤點內容.GUID] = Guid.NewGuid().ToString();
                        value[(int)enum_盤點內容.Master_GUID] = creat.GUID;
                        value[(int)enum_盤點內容.盤點單號] = SN;
                        list_content_add.Add(value);
                    }
                  
                }

                for (int i = 0; i < list_content_add.Count; i++)
                {
                    list_content_add[i][(int)enum_盤點內容.序號] = (list_content.Count + i + 1);
                }
                sQLControl_inventory_content.AddRows(null, list_content_add);
                returnData.Code = 200;
                returnData.Data = creat;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"新增盤點內容成功!共<{list_content_add.Count}>筆";
                returnData.Method = "content_add_by_IC_SN";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 以盤點單號下載
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 盤點單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "20231026-0"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>Excel</returns>
        [Route("download_excel_by_IC_SN")]
        [HttpPost]
        public async Task<ActionResult> download_excel_by_IC_SN([FromBody] returnData returnData)
        {

            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
            if (sys_serverSettingClasses.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return null;
            }
            string Server = sys_serverSettingClasses[0].Server;
            string DB = sys_serverSettingClasses[0].DBName;
            string UserName = sys_serverSettingClasses[0].User;
            string Password = sys_serverSettingClasses[0].Password;
            uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

            string server = Server;
            string dbName = DB;
            string json = creat_get_by_IC_SN(returnData);
            returnData = json.JsonDeserializet<returnData>();

            if (returnData.Code != 200)
            {
                return null;
            }
            List<inventoryClass.creat> creats = returnData.Data.ObjToListClass<inventoryClass.creat>();
            List<object[]> list_value = new List<object[]>();
            System.Data.DataTable dataTable;
            SheetClass sheetClass;
            inventoryClass.creat creat = creats[0];
            List<SheetClass> sheetClasses = new List<SheetClass>();
            List<SheetTemp> sheetTemps = new List<SheetTemp>();
            List<SheetTemp> sheetTemps_buf = new List<SheetTemp>();
            Console.WriteLine($"取得creats {myTimer.ToString()}");
            for (int i = 0; i < creat.Contents.Count; i++)
            {
                if (creat.Contents[i].盤點量.StringToInt32() <= 0)
                {
                    continue;
                }
                object[] value = new object[new enum_盤點定盤_Excel().GetLength()];
                value[(int)enum_盤點定盤_Excel.藥碼] = creat.Contents[i].藥品碼;
                value[(int)enum_盤點定盤_Excel.料號] = creat.Contents[i].料號;
                value[(int)enum_盤點定盤_Excel.藥名] = creat.Contents[i].藥品名稱;
                value[(int)enum_盤點定盤_Excel.庫存量] = creat.Contents[i].理論值;
                value[(int)enum_盤點定盤_Excel.盤點量] = creat.Contents[i].盤點量;
                list_value.Add(value);

                for (int k = 0; k < creat.Contents[i].Sub_content.Count; k++)
                {
                    string 操作人 = creat.Contents[i].Sub_content[k].操作人;
                    sheetTemps_buf = (from temp in sheetTemps
                                      where temp.Name == 操作人
                                      select temp).ToList();
                    if (sheetTemps_buf.Count == 0)
                    {
                        sheetTemps.Add(new SheetTemp(操作人));
                        sheetTemps_buf = (from temp in sheetTemps
                                          where temp.Name == 操作人
                                          select temp).ToList();
                    }
                    value = new object[new enum_盤點定盤_Excel().GetLength()];
                    value[(int)enum_盤點定盤_Excel.藥碼] = creat.Contents[i].Sub_content[k].藥品碼;
                    value[(int)enum_盤點定盤_Excel.料號] = creat.Contents[i].Sub_content[k].料號;
                    value[(int)enum_盤點定盤_Excel.藥名] = creat.Contents[i].Sub_content[k].藥品名稱;
                    value[(int)enum_盤點定盤_Excel.庫存量] = creat.Contents[i].理論值;
                    value[(int)enum_盤點定盤_Excel.盤點量] = creat.Contents[i].Sub_content[k].盤點量;
                    sheetTemps_buf[0].list_value.Add(value);
                }

            }

            dataTable = list_value.ToDataTable(new enum_盤點定盤_Excel());
            Enum[] enums = new Enum[] {  enum_盤點定盤_Excel.庫存量, enum_盤點定盤_Excel .盤點量 ,enum_盤點定盤_Excel .單價 ,enum_盤點定盤_Excel .庫存金額 ,enum_盤點定盤_Excel .消耗量 ,
                enum_盤點定盤_Excel.結存金額, enum_盤點定盤_Excel .誤差量 ,enum_盤點定盤_Excel.誤差金額,enum_盤點定盤_Excel.覆盤量 };

            sheetClass = dataTable.NPOI_GetSheetClass(enums);
            sheetClass.Name = "盤點總表";
            sheetClasses.Add(sheetClass);
            for (int i = 0; i < sheetTemps.Count; i++)
            {
                dataTable = sheetTemps[i].list_value.ToDataTable(new enum_盤點定盤_Excel());
                sheetClass = dataTable.NPOI_GetSheetClass(enums);
                sheetClass.Name = sheetTemps[i].Name;
                sheetClasses.Add(sheetClass);
            }

            Console.WriteLine($"NewCell_Webapi_Buffer_Caculate {myTimer.ToString()}");

            string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string xls_command = "application/vnd.ms-excel";

            byte[] excelData = sheetClasses.NPOI_GetBytes(Excel_Type.xlsx);
            Stream stream = new MemoryStream(excelData);
            return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_盤點表.xlsx"));
        }
        /// <summary>
        /// 取得盤點上傳表頭
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("get_excel_header")]
        [HttpPost]
        public async Task<ActionResult> get_excel_header()
        {
            string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string xls_command = "application/vnd.ms-excel";

            List<object[]> list_value = new List<object[]>();
            System.Data.DataTable dataTable = list_value.ToDataTable(new enum_盤點單上傳_Excel());
            //SheetClass sheetClass = dataTable.NPOI_GetSheetClass(new int[] { (int)enum_盤點單上傳_Excel.理論值});
         
            byte[] excelData = dataTable.NPOI_GetBytes(Excel_Type.xls);
            Stream stream = new MemoryStream(excelData);
            return await Task.FromResult(File(stream, xlsx_command, $"盤點上傳_header.xls"));


        }
        /// <summary>
        /// 以盤點單號上傳
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        /// 1. [file] : 盤點單<br/> 
        /// 2. [IC_NAME] : 盤點單名稱<br/> 
        /// 3. [CT] : 建表人<br/> 
        /// 4. [CT] : 預設盤點人<br/> 
        ///  --------------------------------------------<br/> 
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("excel_upload")]
        [Route("excel_inventory_upload")]
        [HttpPost]
        public async Task<string> excel_upload([FromForm] IFormFile file, [FromForm] string IC_NAME, [FromForm] string CT, [FromForm] string DEFAULT_OP)
        {
            returnData returnData = new returnData();
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端")[0];


                List<medClass> medClasses = medClass.get_med_cloud("http://127.0.0.1:4433");
                if (medClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "ServerSetting VM端設定異常!";
                    return returnData.JsonSerializationt(true);
                }
 

                returnData.Method = "excel_upload";
                var formFile = Request.Form.Files.FirstOrDefault();

                if (formFile == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "文件不得為空";
                    return returnData.JsonSerializationt(true);
                }
                List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
                medClass medclass = null;

                string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                inventoryClass.creat creat = new inventoryClass.creat();
                string error = "";
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);
                    dt = dt.ReorderTable(new enum_盤點單上傳_Excel());
                    if (dt == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = "上傳文件表頭無效!";
                        return returnData.JsonSerializationt(true);
                    }
                    List<object[]> list_value = dt.DataTableToRowList();
                    returnData returnData_new_IC_SN = GET_new_IC_SN(returnData).JsonDeserializet<returnData>();
                    if(IC_NAME.StringIsEmpty())
                    {
                        IC_NAME = Path.GetFileNameWithoutExtension(file.FileName);
                    }
                    creat.盤點名稱 = IC_NAME;
                    creat.盤點單號 = returnData_new_IC_SN.Value;
                    creat.建表人 = CT;
                    creat.預設盤點人 = DEFAULT_OP;
                    creat.建表時間 = DateTime.Now.ToDateTimeString();
                    creat.盤點開始時間 = DateTime.MinValue.ToDateTimeString();
                    creat.盤點結束時間 = DateTime.MinValue.ToDateTimeString();
                    creat.盤點狀態 = "等待盤點";
                    string 藥碼 = "";
                    string 料號 = "";
                    string 盤點量 = "";
                    for (int i = 0; i < list_value.Count; i++)
                    {

                        inventoryClass.content content = new inventoryClass.content();
                        content.GUID = Guid.NewGuid().ToString();
                        藥碼 = list_value[i][(int)enum_盤點單上傳_Excel.藥碼].ObjectToString();
                        content.序號 = (i + 1).ToString();
                        content.藥品碼 = list_value[i][(int)enum_盤點單上傳_Excel.藥碼].ObjectToString();
                        content.儲位名稱 = list_value[i][(int)enum_盤點單上傳_Excel.儲位名稱].ObjectToString();
                        盤點量 = list_value[i][(int)enum_盤點單上傳_Excel.盤點量].ObjectToString();
 
                        medclass = medClasses_cloud.SerchByBarcode(藥碼);
                        if (medclass == null) continue;
                        content.藥品碼 = medclass.藥品碼.ObjectToString();
                        content.料號 = medclass.料號.ObjectToString();
                        content.藥品名稱 = medclass.藥品名稱.ObjectToString();
                        content.中文名稱 = medclass.中文名稱.ObjectToString();
                        content.包裝單位 = medclass.包裝單位.ObjectToString();
                        if (盤點量.StringIsDouble())
                        {
                            inventoryClass.sub_content sub_Content = new inventoryClass.sub_content();
                            sub_Content.操作人 = "系統";
                            sub_Content.藥品碼 = content.藥品碼;
                            sub_Content.料號 = content.料號;
                            sub_Content.盤點量 = 盤點量;
                            content.Sub_content.Add(sub_Content);

                        }
                        creat.Contents.Add(content);
                    }
                }
                returnData.Data = creat;
                string json_creat_add = creat_add(returnData);
                returnData = json_creat_add.JsonDeserializet<returnData>();
                if (returnData.Code != 200)
                {
                    return returnData.JsonSerializationt(true);
                }
            
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = "接收上傳文件成功";
                if (error.StringIsEmpty() == false)
                {
                    returnData.Result += "\n";
                    returnData.Result += "-----下列藥碼,藥檔內無資料-----\n";
                    returnData.Result += error;
                }
                return returnData.JsonSerializationt(true);
            }
            catch(Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
            finally
            {
             
            }
         


            
        }

        private returnData Function_Get_inventory_creat(sys_serverSettingClass sys_serverSettingClass, string MED_TableName, List<object[]> list_inventory_creat)
        {
            return Function_Get_inventory_creat(sys_serverSettingClass, MED_TableName, list_inventory_creat, true);
        }
        private returnData Function_Get_inventory_creat(sys_serverSettingClass sys_serverSettingClass, string MED_TableName, List<object[]> list_inventory_creat, bool allData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];

            MED_pageController mED_PageController = new MED_pageController();
            returnData returnData_med = new returnData();
            returnData_med.ServerName = "Main";
            returnData_med.ServerType = "網頁";
            returnData_med.Server = sys_serverSettingClasses_med.Server;
            returnData_med.DbName = sys_serverSettingClasses_med.DBName;
            returnData_med.TableName = "medicine_page_cloud";
            returnData_med.UserName = sys_serverSettingClasses_med.User;
            returnData_med.Password = sys_serverSettingClasses_med.Password;
            returnData_med.Port = sys_serverSettingClasses_med.Port.StringToUInt32();
            returnData_med = mED_PageController.get_by_apiserver(returnData_med).JsonDeserializet<returnData>();
            List<medClass> medClasses = returnData_med.Data.ObjToListClass<medClass>();
            List<medClass> medClasses_buf = new List<medClass>();

            returnData returnData = new returnData();
            List<object[]> list_inventory_creat_buf = new List<object[]>();
            List<object[]> list_inventory_content_buf = new List<object[]>();
            List<object[]> list_inventory_sub_content_buf = new List<object[]>();
 
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 中文名稱 = "";
            string 包裝單位 = "";
            List<inventoryClass.creat> creats = new List<inventoryClass.creat>();
            for (int i = 0; i < list_inventory_creat.Count; i++)
            {
                inventoryClass.creat creat = list_inventory_creat[i].SQLToClass<inventoryClass.creat, enum_盤點單號>();
            
                if (allData)
                {
                    List<object[]> list_inventory_content = sQLControl_inventory_content.GetRowsByDefult(null, (int)enum_盤點內容.盤點單號, creat.盤點單號);
                    List<object[]> list_inventory_sub_content = sQLControl_inventory_sub_content.GetRowsByDefult(null, (int)enum_盤點明細.盤點單號, creat.盤點單號);
                    藥品碼 = "";
                    藥品名稱 = "";
                    中文名稱 = "";
                    包裝單位 = "";
                    list_inventory_content_buf = list_inventory_content.GetRows((int)enum_盤點內容.Master_GUID, creat.GUID);
                    for (int k = 0; k < list_inventory_content_buf.Count; k++)
                    {
                        inventoryClass.content content = list_inventory_content_buf[k].SQLToClass<inventoryClass.content, enum_盤點內容>();
                        藥品碼 = content.藥品碼;
                        if (medClasses != null)
                        {
                            medClasses_buf = (from value in medClasses
                                              where value.藥品碼 == 藥品碼
                                              select value).ToList();
                            if (medClasses_buf.Count > 0)
                            {
                                藥品名稱 = medClasses_buf[0].藥品名稱;
                                中文名稱 = medClasses_buf[0].中文名稱;
                                包裝單位 = medClasses_buf[0].包裝單位;
                            }
                        }
                        content.藥品名稱 = 藥品名稱;
                        content.中文名稱 = 中文名稱;
                        content.包裝單位 = 包裝單位;

                        int 盤點量 = 0;
                        list_inventory_sub_content_buf = list_inventory_sub_content.GetRows((int)enum_盤點明細.Master_GUID, content.GUID);
                        for (int m = 0; m < list_inventory_sub_content_buf.Count; m++)
                        {
                            inventoryClass.sub_content sub_Content = list_inventory_sub_content_buf[m].SQLToClass<inventoryClass.sub_content, enum_盤點明細>();
                            sub_Content.藥品名稱 = 藥品名稱;
                            sub_Content.中文名稱 = 中文名稱;
                            sub_Content.包裝單位 = 包裝單位;
                            if (sub_Content.盤點量.StringIsInt32())
                            {

                                盤點量 += sub_Content.盤點量.StringToInt32();
                            }
                            content.Sub_content.Add(sub_Content);
                        }
                        for (int m = 0; m < content.Sub_content.Count; m++)
                        {
                            content.Sub_content[m].總量 = 盤點量.ToString();
                        }
                        content.盤點量 = 盤點量.ToString();
                        content.Sub_content.Sort(new ICP_sub_content());
                        creat.Contents.Add(content);
                    }
                }
                creat.Contents.Sort(new ICP_Contents());
                creats.Add(creat);
            }
            creats.Sort(new ICP_creat_by_CT_TIME());
            returnData.Data = creats;
            returnData.Code = 200;
            returnData.Server = Server;
            returnData.DbName = DB;



            returnData.Result = $"成功! {myTimer.ToString()}";
            return returnData;
        }

        static private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {

            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_盤點單號()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_盤點內容()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_盤點明細()));
            return tables.JsonSerializationt(true);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public string init(returnData returnData)
        {
            return this.GET_init(returnData);
        }
        private class ICP_creat_by_CT_TIME : IComparer<inventoryClass.creat>
        {
            public int Compare(inventoryClass.creat x, inventoryClass.creat y)
            {
                string Code0 = x.建表時間;
                string Code1 = y.建表時間;
                return Code1.CompareTo(Code0);
            }
        }
        private class ICP_Contents : IComparer<inventoryClass.content>
        {
            public int Compare(inventoryClass.content x, inventoryClass.content y)
            {
                int 序號1 = x.序號.StringToInt32();
                int 序號2 = x.序號.StringToInt32();
                if (序號1 > 0 && 序號2 > 0)
                {
                    return 序號1.CompareTo(序號2);
                }
                else
                {
                    if (x.理論值 == "0" && y.理論值 == "0")
                    {
                        return x.藥品碼.CompareTo(y.藥品碼);
                    }
                    else if (x.理論值 != "0" && y.理論值 != "0")
                    {
                        // 先按照理論值进行排序
                        int theoryComparison = x.理論值.CompareTo(y.理論值);

                        if (theoryComparison == 0)
                        {
                            // 如果理論值相同，则按照藥品碼进行排序
                            return x.藥品碼.CompareTo(y.藥品碼);
                        }
                        else
                        {
                            return theoryComparison;
                        }
                    }
                    else
                    {
                        // 如果只有一个项的理論值为0，则将具有非零理論值的项排在前面
                        return y.理論值.CompareTo(x.理論值);
                    }
                }
                
            }
        }
        private class ICP_sub_content : IComparer<inventoryClass.sub_content>
        {
            public int Compare(inventoryClass.sub_content x, inventoryClass.sub_content y)
            {
                return x.操作時間.CompareTo(y.操作時間);
            }
        }
    }

}

