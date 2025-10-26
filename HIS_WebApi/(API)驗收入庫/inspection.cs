using Basic;
using Google.Protobuf.WellKnownTypes;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using MyUI;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Basic.Net;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]


    public class inspectionController : Controller
    {
        //private IHostingEnvironment _environment;
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>> serverInfoTask
        = new Lazy<Task<(string, string, string, string, uint)>>(() =>
            Method.GetServerInfoAsync("Main", "網頁", "VM端")
        );
        //public inspectionController(IHostingEnvironment env)
        //{
        //    _environment = env;

        //}
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        static private string API = "http://127.0.0.1:4433";


        /// <summary>
        /// 初始化驗收單資料庫
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
        /// 取得可建立今日最新驗收單號
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
        /// <returns>[returnData.Value]為建立驗收單號</returns>       
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


            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);

            List<object[]> list_inspection_creat = sQLControl_inspection_creat.GetAllRows(null);
            List<object[]> list_inspection_creat_buf = new List<object[]>();

            list_inspection_creat_buf = list_inspection_creat.GetRowsInDate((int)enum_驗收單號.建表時間, DateTime.Now);


            string 驗收單號 = "";
            int index = 0;
            while (true)
            {
                驗收單號 = $"{DateTime.Now.ToDateTinyString()}-{index}";
                index++;
                list_inspection_creat_buf = list_inspection_creat.GetRows((int)enum_驗收單號.驗收單號, 驗收單號);
                if (list_inspection_creat_buf.Count == 0) break;
            }
            returnData.Value = 驗收單號;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "new_IC_SN";
            returnData.Result = $"成功! {myTimer.ToString()}";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以建表日區間搜尋驗收單
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
        /// <returns>[returnData.Data]為驗收單結構</returns>
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

                SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
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
                sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
                List<object[]> list_inspection_creat = sQLControl_inspection_creat.GetAllRows(null);
                list_inspection_creat = list_inspection_creat.GetRowsInDateEx((int)enum_驗收單號.建表時間, date_st, date_end);
                returnData = Function_Get_inspection_creat(sys_serverSettingClasses[0], returnData.TableName, list_inspection_creat, false);
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Method = "creat_get_by_CT_TIME_ST_END";
                returnData.Result = $"取得驗收資料成功!";

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

                SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();
                if (creat.建表時間.Check_Date_String() == false)
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }
                sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
                List<object[]> list_inspection_creat = sQLControl_inspection_creat.GetAllRows(null);
                list_inspection_creat = list_inspection_creat.GetRowsInDate((int)enum_驗收單號.建表時間, creat.建表時間.StringToDateTime());
                if (returnData.Value == "1")
                {
                    returnData = Function_Get_inspection_creat(sys_serverSettingClasses[0], returnData.TableName, list_inspection_creat, false);
                }
                else
                {
                    returnData = Function_Get_inspection_creat(sys_serverSettingClasses[0], returnData.TableName, list_inspection_creat);
                }
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得驗收資料成功!";
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
        /// 以驗收單號更新驗收單開始時間
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 驗收單號 <br/> 
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
        /// <returns>[returnData.Data]為驗收單結構</returns>
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

                SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();
                string json_out = creat_get_by_IC_SN(returnData);
                returnData = json_out.JsonDeserializet<returnData>();
                if (returnData.Code < 0)
                {
                    returnData.Method = "creat_update_startime_by_CT_TIME";
                    return returnData.JsonSerializationt();
                }
                List<inspectionClass.creat> creats = returnData.Data.ObjToListClass<inspectionClass.creat>();
                if (creats.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Result = $"取得驗收資料失敗!";
                    returnData.Method = "creat_update_startime_by_CT_TIME";

                    return returnData.JsonSerializationt(true);
                }
                creat = creats[0];
                if (creat.驗收開始時間 == DateTime.MaxValue.ToDateTimeString())
                {
                    creat.驗收開始時間 = DateTime.Now.ToDateTimeString();
                }

                creat.驗收狀態 = "驗收中";
                object[] value = creat.ClassToSQL<inspectionClass.creat, enum_驗收單號>();
                List<object[]> list_value = new List<object[]>();
                list_value.Add(value);
                sQLControl_inspection_creat.UpdateByDefulteExtra(null, list_value);
                returnData.Code = 200;
                returnData.Data = creat;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"更新驗收開始時間成功!";
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
        /// 以驗收單號搜尋驗收單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 驗收單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "ValueAry" : ["驗收單號", "請購單號"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為驗收單結構</returns>
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

                SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();
                string IC_SN = "";
                if (creat != null)
                {
                    IC_SN = creat.驗收單號;
                }
                if (IC_SN.StringIsEmpty())
                {
                    IC_SN = returnData.Value;
                }
                if (IC_SN.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"驗收單號空白";
                    return returnData.JsonSerializationt();
                }
                sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);

                List<object[]> list_inspection_creat = sQLControl_inspection_creat.GetAllRows(null);
                list_inspection_creat = list_inspection_creat.GetRows((int)enum_驗收單號.驗收單號, IC_SN);
                if (list_inspection_creat.Count == 0)
                {
                    returnData.Code = -5;
                    returnData.Result = $"查無此單號資料[{returnData.Value}]!";
                    return returnData.JsonSerializationt();
                }
                MED_pageController mED_PageController = new MED_pageController();

                returnData = Function_Get_inspection_creat(sys_serverSettingClasses[0], returnData.TableName, list_inspection_creat);
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得驗收資料成功!";
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
        [Route("content_get_by_PON")]
        [HttpPost]
        public string content_get_by_PON([FromBody] returnData returnData)
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

                SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
                //SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                //inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>()
                //if (creat != null)
                //{
                //    PON = creat.請購單號;
                //}
                //if (PON.StringIsEmpty())
                //{
                //    PON = returnData.Value;
                //}
                //if (PON.StringIsEmpty())
                //{
                //    returnData.Code = -200;
                //    returnData.Result = $"請購單號空白";
                //    return returnData.JsonSerializationt();
                //}
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"驗收單號\",\"請購單號\"]";
                    return returnData.JsonSerializationt(true);
                }
                string 驗收單號 = returnData.ValueAry[0];
                string 請購單號 = returnData.ValueAry[1];

                List<object[]> list_inspection_content = sQLControl_inspection_content.GetRowsByDefult(null, (int)enum_驗收單號.驗收單號, 驗收單號);
                List<inspectionClass.content> contents = list_inspection_content.SQLToClass<inspectionClass.content, enum_驗收內容>();
                if (contents.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此單驗收單號[{驗收單號}]!";
                    return returnData.JsonSerializationt(true);
                }
                List<inspectionClass.content> contents_buff = contents.Where(temp => temp.請購單號 == 請購單號).ToList();
                if (contents_buff.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此單請購單號[{請購單號}]!";
                    return returnData.JsonSerializationt(true);
                }
                //單號可能找到兩種以上GUID，用不同GUID去搜尋詳細資料
                //bool flag = false;
                //for (int i = 0; i < contents.Count; i++)
                //{
                //    string GUID = contents[i].GUID;
                //    List<object[]> list_inspection_sub_content = sQLControl_inspection_sub_content.GetRowsByDefult(null, (int)enum_驗收明細.Master_GUID, GUID);
                //    List<inspectionClass.sub_content> sub_Contents = list_inspection_sub_content.SQLToClass<inspectionClass.sub_content, enum_驗收明細>();
                //    if (sub_Contents.Count > 0)
                //    {
                //        flag = true;
                //        contents[i].Sub_content.Add(sub_Contents[0]);
                //        break;
                //    }
                //}
                //List<inspectionClass.content> contents_buff = new List<inspectionClass.content>();
                //if (flag)
                //{
                //    contents = contents.Where(temp => temp.Sub_content.Count > 0).ToList();
                //}
                //else
                //{
                //    contents[0].Sub_content = new List<inspectionClass.sub_content>();
                //}


                returnData.Data = contents_buff[0];
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得驗收資料成功!";
                returnData.Method = "content_get_by_PON";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        [HttpPost("content_get_by_IC_SN")]
        public async Task<string> content_get_by_IC_SN([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();

                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"請購單號空白";
                    return returnData.JsonSerializationt();
                }
                string[] 請購單號 = returnData.ValueAry[0].Split(";");

                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;

                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                

                List<object[]> list_inspection_content = await sQLControl_inspection_content.GetRowsByDefultAsync(null, (int)enum_驗收內容.請購單號, 請購單號);
                if (list_inspection_content.Count == 0)
                {
                    string 請購單號_ = 請購單號[0].Split('-')[0];
                    list_inspection_content = await sQLControl_inspection_content.GetRowsByDefultAsync(null, (int)enum_驗收內容.請購單號, 請購單號_);
                }
                if (list_inspection_content.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此單請購單號[{請購單號}]!";
                    return returnData.JsonSerializationt(true);
                }

                List<inspectionClass.content> contents = list_inspection_content.SQLToClass<inspectionClass.content, enum_驗收內容>();
                string[] guids = contents.Select(x => x.GUID).ToArray();
                List<object[]> list_inspection_sub_content = await sQLControl_inspection_sub_content.GetRowsByDefultAsync(null, (int)enum_驗收明細.Master_GUID, guids);
                List<inspectionClass.sub_content> sub_Contents = list_inspection_sub_content.SQLToClass<inspectionClass.sub_content, enum_驗收明細>();
                
                if (sub_Contents.Count > 0)
                {
                    foreach (var content in contents)
                    {
                        content.Sub_content = sub_Contents.Where(x => x.Master_GUID == content.GUID).ToList();
                    }
                }

                returnData.Data = contents;
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"取得請購單號({請購單號})資料";
                returnData.Method = "content_get_by_IC_SN";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 依指定時間區間（<c>新增時間</c>）查詢驗收主檔 <c>inspection_content</c>，並載入其子明細
        /// <c>inspection_sub_content</c>；回傳對應的 <c>content</c> 物件清單（含 <c>Sub_content</c> 與 <c>textVision</c>）。
        /// </summary>
        /// <param name="returnData">
        /// 請求/回應包裹物件：
        /// <list type="bullet">
        /// <item><description>
        /// <c>ValueAry</c>：必填、長度 2。格式：<c>["開始時間","結束時間"]</c>（字串；建議使用 <c>yyyy-MM-dd HH:mm:ss</c>）。<br/>
        /// 查詢條件為：<c>新增時間 &gt;= 開始時間</c> 且 <c>新增時間 &lt; 結束時間</c>（右半開區間）。</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// 回傳 <see cref="returnData"/> 序列化字串：
        /// <list type="bullet">
        /// <item><description><b>成功：</b><c>Code = 200</c>；<c>Data</c> 為 <c>List&lt;content&gt;</c>；<c>Result</c> 為區間與筆數摘要；<c>TimeTaken</c> 為耗時。</description></item>
        /// <item><description><b>失敗：</b><c>Code = -200</c>；<c>Result</c> 為錯誤訊息。</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <para><b>資料來源與連線</b></para>
        /// 透過 <c>HIS_WebApi.Method.GetServerInfoAsync("Main","網頁","VM端")</c> 取得 DB 連線；使用 <c>SQLControl</c>
        /// 連線至 <c>dbvm.inspection_content</c> 與 <c>dbvm.inspection_sub_content</c>。
        ///
        /// <para><b>查詢流程</b></para>
        /// <list type="number">
        /// <item><description>主檔：<c>SELECT * FROM dbvm.inspection_content WHERE 新增時間 &gt;= @startTime AND 新增時間 &lt; @endTime</c></description></item>
        /// <item><description>彙整主檔 <c>GUID</c> 後，一次取回子明細：<c>SELECT * FROM dbvm.inspection_sub_content WHERE Master_GUID IN @guidList</c></description></item>
        /// <item><description>依 <c>Master_GUID</c> 關聯回填至對應主檔的 <c>Sub_content</c>；若無明細則為空集合。</description></item>
        /// </list>
        ///
        /// <para><b>回傳模型（主檔 <c>content</c>）欄位</b></para>
        /// <list type="bullet">
        /// <item><description><c>GUID</c>：主檔唯一識別碼</description></item>
        /// <item><description><c>Master_GUID</c>：上層/批次識別（如有）</description></item>
        /// <item><description><c>請購單號</c>（<c>PON</c>）</description></item>
        /// <item><description><c>驗收單號</c>（<c>IC_SN</c>）</description></item>
        /// <item><description><c>藥品碼</c>（<c>CODE</c>）</description></item>
        /// <item><description><c>廠牌</c>（<c>BRD</c>）</description></item>
        /// <item><description><c>料號</c>（<c>SKDIACODE</c>）</description></item>
        /// <item><description><c>中文名稱</c>（<c>CHT_NAME</c>）</description></item>
        /// <item><description><c>藥品名稱</c>（<c>NAME</c>）</description></item>
        /// <item><description><c>包裝單位</c>（<c>PAKAGE</c>）</description></item>
        /// <item><description><c>藥品條碼1</c>（<c>BARCODE1</c>）</description></item>
        /// <item><description><c>藥品條碼2</c>（<c>BARCODE2</c>）</description></item>
        /// <item><description><c>應收數量</c>（<c>START_QTY</c>）</description></item>
        /// <item><description><c>實收數量</c>（<c>END_QTY</c>）</description></item>
        /// <item><description><c>新增時間</c>（<c>ADD_TIME</c>）</description></item>
        /// <item><description><c>編號</c>（<c>SEQ</c>）</description></item>
        /// <item><description><c>贈品註記</c>（<c>FREE_CHARGE_FLAG</c>）</description></item>
        /// <item><description><c>API回寫註記</c>（<c>API_RETURN_NOTE</c>）</description></item>
        /// <item><description><c>備註</c>（<c>NOTE</c>）</description></item>
        /// <item><description><c>Sub_content</c>：對應的子明細集合（<c>List&lt;sub_content&gt;</c>）。</description></item>
        /// <item><description><c>textVision</c>：OCR/辨識結果等文字分析集合（如有；可為 <c>null</c>）。</description></item>
        /// </list>
        ///
        /// <para><b>回傳模型（子檔 <c>sub_content</c>）欄位</b></para>
        /// <list type="bullet">
        /// <item><description><c>GUID</c>、<c>Master_GUID</c></description></item>
        /// <item><description><c>驗收單號</c>（<c>ACPT_SN</c>）</description></item>
        /// <item><description><c>藥品碼</c>（<c>CODE</c>）</description></item>
        /// <item><description><c>料號</c>（<c>SKDIACODE</c>）</description></item>
        /// <item><description><c>中文名稱</c>（<c>CHT_NAME</c>）</description></item>
        /// <item><description><c>藥品名稱</c>（<c>NAME</c>）</description></item>
        /// <item><description><c>包裝單位</c>（<c>PAKAGE</c>）</description></item>
        /// <item><description><c>藥品條碼1</c>（<c>BARCODE1</c>）</description></item>
        /// <item><description><c>藥品條碼2</c>（<c>BARCODE2</c>）</description></item>
        /// <item><description><c>實收數量</c>（<c>END_QTY</c>）</description></item>
        /// <item><description><c>總量</c>（<c>TOLTAL_QTY</c>）</description></item>
        /// <item><description><c>效期</c>（<c>VAL</c>）</description></item>
        /// <item><description><c>批號</c>（<c>LOT</c>）</description></item>
        /// <item><description><c>操作人</c>（<c>OP</c>）</description></item>
        /// <item><description><c>操作時間</c>（<c>OP_TIME</c>）</description></item>
        /// <item><description><c>狀態</c>（<c>STATE</c>）</description></item>
        /// <item><description><c>備註</c>（<c>NOTE</c>）</description></item>
        /// </list>
        ///
        /// <para><b>效能與索引建議</b></para>
        /// <list type="bullet">
        /// <item><description>使用參數化查詢避免 SQL Injection。</description></item>
        /// <item><description>批次以 <c>IN @guidList</c> 擷取子明細，減少往返。</description></item>
        /// <item><description>建議在 <c>inspection_content(新增時間)</c> 與 <c>inspection_sub_content(Master_GUID)</c> 建索引。</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code language="json"><![CDATA[
        /// // Request
        /// {
        ///   "ValueAry": [
        ///     "2025-09-01 00:00:00",
        ///     "2025-09-20 23:59:59"
        ///   ]
        /// }
        ///
        /// // Response (節錄；欄位與 CLASS 對應)
        /// {
        ///   "Code": 200,
        ///   "Method": "content_get_by_addTime",
        ///   "Result": "此時間區域2025-09-01 00:00:00-2025-09-20 23:59:59資料，共11筆",
        ///   "TimeTaken": "891.714ms",
        ///   "Data": [
        ///     {
        ///       "GUID": "f6109304-9c41-4741-bc28-cdf3725fc2c2",
        ///       "Master_GUID": "5f75cdb3-f372-45a6-a156-1ffb25eed353",
        ///       "PON": "1140515001-13",
        ///       "IC_SN": "20250911-0",
        ///       "CODE": "OARC2",
        ///       "BRD": "",
        ///       "SKDIACODE": "ETOO10",
        ///       "CHT_NAME": "Etor 中文名(如有)",
        ///       "NAME": "Etor",
        ///       "PAKAGE": "盒",
        ///       "BARCODE1": "",
        ///       "BARCODE2": "",
        ///       "START_QTY": "1680",
        ///       "END_QTY": "",
        ///       "ADD_TIME": "2025/09/11 14:38:17",
        ///       "SEQ": "",
        ///       "FREE_CHARGE_FLAG": "",
        ///       "API_RETURN_NOTE": "",
        ///       "NOTE": "",
        ///       "Sub_content": [
        ///         {
        ///           "GUID": "8258d773-b642-4dc6-83a5-ec33d3853e90",
        ///           "Master_GUID": "f6109304-9c41-4741-bc28-cdf3725fc2c2",
        ///           "ACPT_SN": "20250911-0",
        ///           "CODE": "OARC2",
        ///           "SKDIACODE": "ETOO10",
        ///           "CHT_NAME": "Etor 中文名(如有)",
        ///           "NAME": "Etor",
        ///           "PAKAGE": "盒",
        ///           "BARCODE1": "",
        ///           "BARCODE2": "",
        ///           "END_QTY": "50",
        ///           "TOLTAL_QTY": "50",
        ///           "VAL": "2025/09/16 00:00:00",
        ///           "LOT": "gasdgasd",
        ///           "OP": "鴻森智能科技",
        ///           "OP_TIME": "2025/09/15 16:27:17",
        ///           "STATE": "未鎖定",
        ///           "NOTE": ""
        ///         }
        ///       ],
        ///       "textVision": null
        ///     }
        ///   ]
        /// }
        /// ]]></code>
        /// </example>
        /// <exception cref="System.Exception">
        /// 任何在連線、查詢、資料轉型或序列化過程中拋出的未處理例外；伺服端將以 <c>Code = -200</c>
        /// 與例外訊息回填至 <c>Result</c> 後回傳。
        /// </exception>
        [HttpPost("content_get_by_addTime")]
        public async Task<string> content_get_by_addTime([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"開始時間\",\"結束時間\"]";
                    return returnData.JsonSerializationt(true);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                string tableName_inspection_content = "inspection_content";
                string tableName_inspection_sub_content = "inspection_sub_content";

                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, tableName_inspection_content, UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);

                string 開始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];
                string command = $"SELECT * FROM dbvm.{tableName_inspection_content} WHERE 新增時間 >= @startTime AND 新增時間 < @endTime";
                object param = new { startTime = 開始時間, endTime = 結束時間 };
                List<object[]> list_inspection_content = await sQLControl_inspection_content.WriteCommandAsync(command, param);
                List<inspectionClass.content> contents = list_inspection_content.SQLToClass<inspectionClass.content, enum_驗收內容>();
                if (contents.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此時間區域{開始時間}-{結束時間}資料!";
                    return returnData.JsonSerializationt(true);
                }
                List<string> guids = contents.Select(x => x.GUID).ToList();
                string command_sub_content = $"SELECT * FROM dbvm.{tableName_inspection_sub_content} WHERE Master_GUID IN @guidList";
                object param_sub_content = new { guidList = guids };
                List<object[]> list_inspection_sub_content = await sQLControl_inspection_sub_content.WriteCommandAsync(command_sub_content, param_sub_content);
                List<inspectionClass.sub_content> sub_Contents = list_inspection_sub_content.SQLToClass<inspectionClass.sub_content, enum_驗收明細>();

                if (sub_Contents.Count > 0)
                {
                    Dictionary<string, List<inspectionClass.sub_content>> dic = sub_Contents.ToDictByMasterGUID();
                    foreach (var item in contents)
                    {
                        List<inspectionClass.sub_content> subs = dic.GetByMasterGUID(item.GUID);
                        item.Sub_content = subs;
                    }
                }

                returnData.Data = contents;
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"此時間區域{開始時間}-{結束時間}資料，共{contents.Count}筆";
                returnData.Method = "content_get_by_addTime";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        [HttpPost("content_get_by_sub_content_addTime")]
        public async Task<string> content_get_by_sub_content_addTime([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"開始時間\",\"結束時間\"]";
                    return returnData.JsonSerializationt(true);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                string tableName_inspection_content = "inspection_content";
                string tableName_inspection_sub_content = "inspection_sub_content";

                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, tableName_inspection_content, UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, tableName_inspection_sub_content, UserName, Password, Port, SSLMode);

                string 開始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];
                string command = $"SELECT * FROM dbvm.{tableName_inspection_sub_content} WHERE 操作時間 >= @startTime AND 操作時間 < @endTime";
                object param = new { startTime = 開始時間, endTime = 結束時間 };
                List<object[]> list_inspection_sub_content = await sQLControl_inspection_sub_content.WriteCommandAsync(command, param);
                List<inspectionClass.sub_content> sub_Contents = list_inspection_sub_content.SQLToClass<inspectionClass.sub_content, enum_驗收明細>();
                if (sub_Contents.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此時間區域{開始時間}-{結束時間}資料!";
                    return returnData.JsonSerializationt(true);
                }
                List<string> master_guid = sub_Contents.Select(x => x.Master_GUID).Distinct().ToList();
                string command_content = $"SELECT * FROM dbvm.{tableName_inspection_content} WHERE GUID IN @guidList";
                object param_content = new { guidList = master_guid };
                List<object[]> list_inspection_content = await sQLControl_inspection_content.WriteCommandAsync(command_content, param_content);
                List<inspectionClass.content> contents = list_inspection_content.SQLToClass<inspectionClass.content, enum_驗收內容>();
                                         

                if (sub_Contents.Count > 0)
                {
                    Dictionary<string, List<inspectionClass.sub_content>> dic = sub_Contents.ToDictByMasterGUID();
                    foreach (var item in contents)
                    {
                        List<inspectionClass.sub_content> subs = dic.GetByMasterGUID(item.GUID);
                        item.Sub_content = subs;
                    }
                }

                returnData.Data = contents;
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"此時間區域{開始時間}-{結束時間}資料，共{contents.Count}筆";
                returnData.Method = "content_get_by_sub_content_addTime";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        [Route("sub_content_get_by_PON")]
        [HttpPost]
        public string sub_content_get_by_PON([FromBody] returnData returnData)
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

                SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();
                string PON = "";
                if (creat != null)
                {
                    PON = creat.請購單號;
                }
                if (PON.StringIsEmpty())
                {
                    PON = returnData.Value;
                }
                if (PON.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"請購單號空白";
                    return returnData.JsonSerializationt();
                }
                //if (returnData.ValueAry == null)
                //{
                //    returnData.Data = -200;
                //    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                //    return returnData.JsonSerializationt();
                //}
                //if (returnData.ValueAry.Count != 2)
                //{
                //    returnData.Code = -200;
                //    returnData.Result = $"returnData.ValueAry 內容應為[\"驗收單號\",\"請購單號\"]";
                //    return returnData.JsonSerializationt(true);
                //}
                //string 驗收單號 = returnData.ValueAry[0];
                string 請購單號 = PON;

                List<object[]> list_inspection_content = sQLControl_inspection_content.GetRowsByDefult(null, (int)enum_驗收單號.請購單號, 請購單號);
                List<inspectionClass.content> contents = list_inspection_content.SQLToClass<inspectionClass.content, enum_驗收內容>();
                if (contents.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此單請購單號[{請購單號}]!";
                    return returnData.JsonSerializationt(true);
                }
                //List<inspectionClass.content> contents_buff = contents.Where(temp => temp.請購單號 == 請購單號).ToList();
                //if(contents_buff.Count == 0)
                //{
                //    returnData.Code = -200;
                //    returnData.Result = $"查無此單請購單號[{請購單號}]!";
                //    return returnData.JsonSerializationt(true);
                //}
                ///單號可能找到兩種以上GUID，用不同GUID去搜尋詳細資料
                bool flag = false;
                for (int i = 0; i < contents.Count; i++)
                {
                    string GUID = contents[i].GUID;
                    List<object[]> list_inspection_sub_content = sQLControl_inspection_sub_content.GetRowsByDefult(null, (int)enum_驗收明細.Master_GUID, GUID);
                    List<inspectionClass.sub_content> sub_Contents = list_inspection_sub_content.SQLToClass<inspectionClass.sub_content, enum_驗收明細>();
                    if (sub_Contents.Count > 0)
                    {
                        flag = true;
                        contents[i].Sub_content.Add(sub_Contents[0]);
                        break;
                    }
                }
                if (flag)
                {
                    contents = contents.Where(temp => temp.Sub_content.Count > 0).ToList();
                }
                else
                {
                    contents[0].Sub_content = new List<inspectionClass.sub_content>();
                }


                returnData.Data = contents[0];
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得驗收資料成功!";
                returnData.Method = "sub_content_get_by_PON";

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 創建驗收單(驗收單號自訂)
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 驗收單名稱 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///        "IC_NAME": "測試驗收",
        ///        "IC_SN": "Q202230101-測試驗收單",
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
        /// <returns>[returnData.Data]為驗收單結構</returns>
        [Route("creat_add")]
        [HttpPost]
        public string creat_add([FromBody] returnData returnData)
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

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();

            List<object[]> list_inspection_creat = sQLControl_inspection_creat.GetAllRows(null);
            List<object[]> list_inspection_creat_buf = new List<object[]>();


            if (creat == null)
            {
                returnData.Code = -5;
                returnData.Result += $"Data 資料錯誤 \n";
                return returnData.JsonSerializationt();
            }

            list_inspection_creat_buf = list_inspection_creat.GetRows((int)enum_驗收單號.驗收單號, creat.驗收單號);
            if (list_inspection_creat_buf.Count > 0)
            {
                returnData.Code = -6;
                returnData.Result += $"驗收單號: {creat.驗收單號} 已存在,請刪除後再建立! \n";
                return returnData.JsonSerializationt();
            }


            if (creat.驗收單號.StringIsEmpty())
            {
                string IC_SN_json = GET_new_IC_SN(returnData);
                returnData returnData_IC_SN = IC_SN_json.JsonDeserializet<returnData>();
                creat.驗收單號 = returnData_IC_SN.Value;
            }

            creat.GUID = Guid.NewGuid().ToString();
            creat.建表時間 = DateTime.Now.ToDateTimeString();
            creat.驗收開始時間 = DateTime.MaxValue.ToDateTimeString();
            creat.驗收結束時間 = DateTime.MaxValue.ToDateTimeString();
            creat.驗收狀態 = "等待驗收";
            List<object[]> list_inspection_creat_add = new List<object[]>();
            List<object[]> list_inspection_content_add = new List<object[]>();
            List<object[]> list_inspection_sub_content_add = new List<object[]>();
            object[] value;
            value = new object[new enum_驗收單號().GetLength()];

            value = creat.ClassToSQL<inspectionClass.creat, enum_驗收單號>();
            list_inspection_creat_add.Add(value);



            for (int i = 0; i < creat.Contents.Count; i++)
            {
                creat.Contents[i].GUID = Guid.NewGuid().ToString();
                creat.Contents[i].新增時間 = DateTime.Now.ToDateTimeString();
                creat.Contents[i].交貨時間 = DateTime.Now.ToDateTimeString();
                creat.Contents[i].Master_GUID = creat.GUID;
                creat.Contents[i].驗收單號 = creat.驗收單號;


                value = new object[new enum_驗收內容().GetLength()];
                value = creat.Contents[i].ClassToSQL<inspectionClass.content, enum_驗收內容>();
                list_inspection_content_add.Add(value);

                if (creat.Contents[i].Sub_content != null)
                {
                    for (int k = 0; k < creat.Contents[i].Sub_content.Count; k++)
                    {
                        creat.Contents[i].Sub_content[k].GUID = Guid.NewGuid().ToString();
                        creat.Contents[i].Sub_content[k].Master_GUID = creat.Contents[i].GUID;
                        creat.Contents[i].Sub_content[k].操作時間 = DateTime.Now.ToDateTimeString_6();
                        creat.Contents[i].Sub_content[k].藥品碼 = creat.Contents[i].藥品碼;
                        creat.Contents[i].Sub_content[k].藥品名稱 = creat.Contents[i].藥品名稱;
                        creat.Contents[i].Sub_content[k].中文名稱 = creat.Contents[i].中文名稱;
                        creat.Contents[i].Sub_content[k].驗收單號 = creat.Contents[i].驗收單號;
                        value = creat.Contents[i].Sub_content[k].ClassToSQL<inspectionClass.sub_content, enum_驗收明細>();

                        list_inspection_sub_content_add.Add(value);
                    }
                }

            }
            sQLControl_inspection_creat.AddRows(null, list_inspection_creat_add);
            sQLControl_inspection_content.AddRows(null, list_inspection_content_add);
            sQLControl_inspection_sub_content.AddRows(null, list_inspection_sub_content_add);
            returnData.Data = creat;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "creat_add";

            returnData.Result = $"成功加入新驗收單! 共{list_inspection_content_add.Count}筆資料";
            return returnData.JsonSerializationt(true);
        }
        [HttpPost("content_add")]
        public async Task<string> content_add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data 內容不可為空!";
                    return returnData.JsonSerializationt(true);
                }
                List<inspectionClass.content> content = returnData.Data.ObjToClass<List<inspectionClass.content>>();
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
                string[] master_guid = content.Select(x => x.Master_GUID).ToArray();
                List<object[]> objects = await sQLControl_inspection_content.GetRowsByDefultAsync(null, (int)enum_驗收內容.Master_GUID, master_guid);
                List<inspectionClass.content> content_sql = objects.SQLToClass<inspectionClass.content, enum_驗收內容>();
                string now = DateTime.Now.ToDateTimeString();
                List<inspectionClass.content> content_add = new List<inspectionClass.content>();
                foreach (var item in content)
                {
                    inspectionClass.content content_buff = content_sql.FirstOrDefault(x => x.Master_GUID == item.Master_GUID);
                    if (content_buff != null) continue;
                    item.GUID = Guid.NewGuid().ToString();
                    item.新增時間 = now;
                    if (item.交貨時間.StringIsEmpty()) item.交貨時間 = now;
                    if (item.訂單時間.StringIsEmpty()) item.訂單時間 = now;
                    content_add.Add(item);
                }
                
                List<object[]> list_inspection_content_add = content_add.ClassToSQL<inspectionClass.content, enum_驗收內容>();
                await sQLControl_inspection_content.AddRowsAsync(null, list_inspection_content_add);

                returnData.Data = content;
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "content_add";

                returnData.Result = $"成功加入新驗收單! 共{content.Count}筆資料";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "content_add";
                returnData.Result = $"{ex.Message}";
                return returnData.JsonSerializationt(true);

            }

            
            
        }
        /// <summary>
        /// 更新驗收單
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///        "IC_NAME": "測試驗收",
        ///        "IC_SN": "Q202230101-測試驗收單",
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
        /// <returns>[returnData.Data]為驗收單結構</returns>
        [Route("creat_update")]
        [HttpPost]
        public string creat_update([FromBody] returnData returnData)
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

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();



            sQLControl_inspection_creat.DeleteByDefult(null, (int)enum_驗收單號.驗收單號, creat.驗收單號);
            sQLControl_inspection_content.DeleteByDefult(null, (int)enum_驗收內容.驗收單號, creat.驗收單號);
            sQLControl_inspection_sub_content.DeleteByDefult(null, (int)enum_驗收明細.驗收單號, creat.驗收單號);
            if (creat == null)
            {
                returnData.Code = -200;
                returnData.Result += $"Data 資料錯誤 \n";
                return returnData.JsonSerializationt();
            }


            if (creat.驗收單號.StringIsEmpty())
            {
                string IC_SN_json = GET_new_IC_SN(returnData);
                returnData returnData_IC_SN = IC_SN_json.JsonDeserializet<returnData>();
                creat.驗收單號 = returnData_IC_SN.Value;
            }

            creat.GUID = Guid.NewGuid().ToString();
            creat.建表時間 = DateTime.Now.ToDateTimeString();
            creat.驗收開始時間 = DateTime.MaxValue.ToDateTimeString();
            creat.驗收結束時間 = DateTime.MaxValue.ToDateTimeString();
            creat.驗收狀態 = "等待驗收";
            List<object[]> list_inspection_creat_add = new List<object[]>();
            List<object[]> list_inspection_content_add = new List<object[]>();
            List<object[]> list_inspection_sub_content_add = new List<object[]>();
            object[] value;
            value = new object[new enum_驗收單號().GetLength()];

            value = creat.ClassToSQL<inspectionClass.creat, enum_驗收單號>();
            list_inspection_creat_add.Add(value);



            for (int i = 0; i < creat.Contents.Count; i++)
            {
                creat.Contents[i].GUID = Guid.NewGuid().ToString();
                creat.Contents[i].新增時間 = DateTime.Now.ToDateTimeString();
                creat.Contents[i].交貨時間 = DateTime.Now.ToDateTimeString();

                creat.Contents[i].Master_GUID = creat.GUID;
                creat.Contents[i].驗收單號 = creat.驗收單號;


                value = new object[new enum_驗收內容().GetLength()];
                value = creat.Contents[i].ClassToSQL<inspectionClass.content, enum_驗收內容>();
                list_inspection_content_add.Add(value);

                if (creat.Contents[i].Sub_content != null)
                {
                    for (int k = 0; k < creat.Contents[i].Sub_content.Count; k++)
                    {
                        creat.Contents[i].Sub_content[k].GUID = Guid.NewGuid().ToString();
                        creat.Contents[i].Sub_content[k].Master_GUID = creat.Contents[i].GUID;
                        creat.Contents[i].Sub_content[k].操作時間 = DateTime.Now.ToDateTimeString_6();
                        creat.Contents[i].Sub_content[k].藥品碼 = creat.Contents[i].藥品碼;
                        creat.Contents[i].Sub_content[k].藥品名稱 = creat.Contents[i].藥品名稱;
                        creat.Contents[i].Sub_content[k].中文名稱 = creat.Contents[i].中文名稱;
                        creat.Contents[i].Sub_content[k].驗收單號 = creat.Contents[i].驗收單號;
                        value = creat.Contents[i].Sub_content[k].ClassToSQL<inspectionClass.sub_content, enum_驗收明細>();

                        list_inspection_sub_content_add.Add(value);
                    }
                }

            }
            sQLControl_inspection_creat.AddRows(null, list_inspection_creat_add);
            sQLControl_inspection_content.AddRows(null, list_inspection_content_add);
            sQLControl_inspection_sub_content.AddRows(null, list_inspection_sub_content_add);
            returnData.Data = creat;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "creat_update";

            returnData.Result = $"成功更新驗收單! 共{list_inspection_content_add.Count}筆資料";
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
        ///  {
        ///    "Data": 
        ///    {                 
        ///        "IC_NAME": "測試驗收",
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
        /// <returns>[returnData.Data]為驗收單結構</returns>
        [Route("creat_auto_add")]
        [HttpPost]
        public string creat_auto_add([FromBody] returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
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

                returnData returnData_GET_new_IC_SN = this.GET_new_IC_SN(returnData).JsonDeserializet<returnData>();
                string str_IC_SN = returnData_GET_new_IC_SN.Value;

                MED_pageController mED_PageController = new MED_pageController();
                returnData returnData_med = new returnData();
                returnData_med.ServerName = returnData.ServerName;
                returnData_med.ServerType = returnData.ServerType;
                returnData_med.Server = Server;
                returnData_med.DbName = DB;
                returnData_med.TableName = "medicine_page_cloud";
                returnData_med.Port = Port;
                returnData_med.UserName = UserName;
                returnData_med.Password = Password;

                returnData_med = mED_PageController.get_by_apiserver(returnData_med).JsonDeserializet<returnData>();
                List<medClass> medClasses = returnData_med.Data.ObjToListClass<medClass>();

                inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();
                creat.驗收單號 = str_IC_SN;
                for (int i = 0; i < medClasses.Count; i++)
                {
                    inspectionClass.content content = new inspectionClass.content();
                    content.藥品碼 = medClasses[i].藥品碼;
                    content.藥品名稱 = medClasses[i].藥品名稱;
                    content.中文名稱 = medClasses[i].中文名稱;
                    content.廠牌 = medClasses[i].廠牌;
                    content.料號 = medClasses[i].料號;
                    content.包裝單位 = medClasses[i].包裝單位;
                    content.應收數量 = "0";

                    creat.Contents.Add(content);
                }
                if (creat.Contents.Count == 0)
                {
                    returnData.Code = -6;
                    returnData.Value = "無驗收資料可新增!";
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
        /// 以驗收單號鎖定驗收單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 驗收單號 <br/> 
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
        /// <returns>[returnData.Data]為驗收單結構</returns>
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
            string Server = sys_serverSettingClasses[0].Server;
            string DB = sys_serverSettingClasses[0].DBName;
            string UserName = sys_serverSettingClasses[0].User;
            string Password = sys_serverSettingClasses[0].Password;
            uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();
            if (creat == null)
            {
                returnData.Code = -5;
                returnData.Result += $"Data資料錯誤!";
                return returnData.JsonSerializationt();
            }
            List<object[]> list_inspection_creat = sQLControl_inspection_creat.GetAllRows(null);
            List<object[]> list_inspection_creat_buf = new List<object[]>();
            list_inspection_creat_buf = list_inspection_creat.GetRows((int)enum_驗收單號.驗收單號, creat.驗收單號);
            if (list_inspection_creat_buf.Count == 0)
            {
                returnData.Code = -5;
                returnData.Result += $"找無此驗收單號!";
                return returnData.JsonSerializationt();
            }
            list_inspection_creat_buf[0][(int)enum_驗收單號.驗收狀態] = "鎖定";
            list_inspection_creat_buf[0][(int)enum_驗收單號.驗收結束時間] = DateTime.Now.ToDateTimeString();
            sQLControl_inspection_creat.UpdateByDefulteExtra(null, list_inspection_creat_buf);
            creat.GUID = list_inspection_creat_buf[0][(int)enum_驗收單號.GUID].ObjectToString();
            creat.驗收狀態 = list_inspection_creat_buf[0][(int)enum_驗收單號.驗收狀態].ObjectToString();
            creat.建表人 = list_inspection_creat_buf[0][(int)enum_驗收單號.建表人].ObjectToString();
            creat.建表時間 = list_inspection_creat_buf[0][(int)enum_驗收單號.建表時間].ToDateTimeString();
            creat.驗收開始時間 = list_inspection_creat_buf[0][(int)enum_驗收單號.驗收開始時間].ToDateTimeString();
            creat.驗收結束時間 = list_inspection_creat_buf[0][(int)enum_驗收單號.驗收結束時間].ObjectToString();
            returnData.Code = 200;
            returnData.Value = "驗收單鎖定成功!";
            returnData.Data = creat;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "creat_lock_by_IC_SN";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以驗收單號解鎖驗收單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 驗收單號 <br/> 
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
        /// <returns>[returnData.Data]為驗收單結構</returns>
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
            string Server = sys_serverSettingClasses[0].Server;
            string DB = sys_serverSettingClasses[0].DBName;
            string UserName = sys_serverSettingClasses[0].User;
            string Password = sys_serverSettingClasses[0].Password;
            uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();
            if (creat == null)
            {
                returnData.Code = -5;
                returnData.Result += $"Data資料錯誤!";
                return returnData.JsonSerializationt();
            }
            List<object[]> list_inspection_creat = sQLControl_inspection_creat.GetAllRows(null);
            List<object[]> list_inspection_creat_buf = new List<object[]>();
            list_inspection_creat_buf = list_inspection_creat.GetRows((int)enum_驗收單號.驗收單號, creat.驗收單號);
            if (list_inspection_creat_buf.Count == 0)
            {
                returnData.Code = -5;
                returnData.Result += $"找無此驗收單號!";
                return returnData.JsonSerializationt();
            }
            list_inspection_creat_buf[0][(int)enum_驗收單號.驗收狀態] = "等待驗收";
            sQLControl_inspection_creat.UpdateByDefulteExtra(null, list_inspection_creat_buf);
            creat.GUID = list_inspection_creat_buf[0][(int)enum_驗收單號.GUID].ObjectToString();
            creat.驗收狀態 = list_inspection_creat_buf[0][(int)enum_驗收單號.驗收狀態].ObjectToString();
            creat.建表人 = list_inspection_creat_buf[0][(int)enum_驗收單號.建表人].ObjectToString();
            creat.建表時間 = list_inspection_creat_buf[0][(int)enum_驗收單號.建表時間].ToDateTimeString();
            creat.驗收開始時間 = list_inspection_creat_buf[0][(int)enum_驗收單號.驗收開始時間].ToDateTimeString();
            creat.驗收結束時間 = list_inspection_creat_buf[0][(int)enum_驗收單號.驗收結束時間].ToDateTimeString();
            returnData.Code = 200;
            returnData.Value = "驗收單解鎖成功!";
            returnData.Data = creat;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "creat_unlock_by_IC_SN";

            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以驗收單號刪除驗收單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 驗收單號 <br/> 
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
        /// <returns>[returnData.Data]為驗收單結構</returns>       
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

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            List<object[]> list_inspection_creat = sQLControl_inspection_creat.GetAllRows(null);
            List<object[]> list_inspection_creat_buf = new List<object[]>();
            List<object[]> list_inspection_content = sQLControl_inspection_content.GetAllRows(null);
            List<object[]> list_inspection_content_buf = new List<object[]>();
            List<object[]> list_inspection_sub_content = sQLControl_inspection_sub_content.GetAllRows(null);
            List<object[]> list_inspection_sub_content_buf = new List<object[]>();
            inspectionClass.creat creat = returnData.Data.ObjToClass<inspectionClass.creat>();

            //if(returnData.Value.StringIsEmpty())
            //{
            //    returnData.Code = -5;
            //    returnData.Result = $"請購單號不得為空白!";
            //    return returnData.JsonSerializationt();
            //}

            list_inspection_creat_buf = list_inspection_creat.GetRows((int)enum_驗收單號.驗收單號, creat.驗收單號);
            list_inspection_content_buf = list_inspection_content.GetRows((int)enum_驗收內容.驗收單號, creat.驗收單號);
            list_inspection_sub_content_buf = list_inspection_sub_content.GetRows((int)enum_驗收明細.驗收單號, creat.驗收單號);

            sQLControl_inspection_creat.DeleteExtra(null, list_inspection_creat_buf);
            sQLControl_inspection_content.DeleteExtra(null, list_inspection_content_buf);
            sQLControl_inspection_sub_content.DeleteExtra(null, list_inspection_sub_content_buf);
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"已將[{creat.驗收單號}]刪除!";
            returnData.Method = "creat_delete_by_IC_SN";

            return returnData.JsonSerializationt();


        }
        /// <summary>
        /// 以GUID刪除驗收單內驗收藥品
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

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            if (returnData.Data == null)
            {
                returnData.Code = -5;
                returnData.Result = $"Data資料長度錯誤!";
                return returnData.JsonSerializationt();
            }
            List<inspectionClass.content> contents = returnData.Data.ObjToListClass<inspectionClass.content>();
            List<object> list_GUID = new List<object>();
            for (int i = 0; i < contents.Count; i++)
            {
                list_GUID.Add(contents[i].GUID);
            }
            sQLControl_inspection_content.DeleteExtra(null, enum_驗收內容.GUID.GetEnumName(), list_GUID);
            sQLControl_inspection_sub_content.DeleteExtra(null, enum_驗收明細.Master_GUID.GetEnumName(), list_GUID);
            returnData.Data = null;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"已刪除驗收內容共<{list_GUID.Count}>筆資料!";
            returnData.Method = "contents_delete_by_GUID";

            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 以GUID取得驗收單內驗收藥品
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
        public async Task<string> content_get_by_content_GUID([FromBody] returnData returnData)
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

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);

            inspectionClass.content content = returnData.Data.ObjToClass<inspectionClass.content>(); ;
            //if (returnData.Value.StringIsEmpty())
            //{
            //    returnData.Code = -5;
            //    returnData.Result = "輸入資料不得空白!";
            //    return returnData.JsonSerializationt();
            //}

            string GUID = content.GUID;
            textVisionClass textVisionClass = new textVisionClass();

            Task<returnData> task_returnData_textVision = new PCMPO().get_by_MasterGUID(GUID);

            returnData.Data = null;
            List<object[]> list_inspection_content = sQLControl_inspection_content.GetAllRows(null);
            List<object[]> list_inspection_content_buf = new List<object[]>();
            List<object[]> list_inspection_sub_content = sQLControl_inspection_sub_content.GetAllRows(null);
            List<object[]> list_inspection_sub_content_buf = new List<object[]>();
            list_inspection_content_buf = list_inspection_content.GetRows((int)enum_驗收內容.GUID, GUID);
            if (list_inspection_content_buf.Count == 0)
            {
                returnData.Code = -1;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"查無驗收內容!";
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
                returnData_med.Value = list_inspection_content_buf[0][(int)enum_驗收內容.藥品碼].ObjectToString();
                string json_med = mED_PageController.get_by_code(returnData_med);
                returnData_med = json_med.JsonDeserializet<returnData>();

                content = list_inspection_content_buf[0].SQLToClass<inspectionClass.content, enum_驗收內容>();
                int 實收數量 = 0;
                list_inspection_sub_content_buf = list_inspection_sub_content.GetRows((int)enum_驗收明細.Master_GUID, content.GUID);
                for (int m = 0; m < list_inspection_sub_content_buf.Count; m++)
                {
                    inspectionClass.sub_content sub_Content = list_inspection_sub_content_buf[m].SQLToClass<inspectionClass.sub_content, enum_驗收明細>();

                    if (sub_Content.實收數量.StringIsInt32())
                    {

                        實收數量 += sub_Content.實收數量.StringToInt32();
                    }
                    content.Sub_content.Add(sub_Content);
                }
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
                                content.Sub_content[i].總量 = 實收數量.ToString();
                            }
                        }


                    }
                }
                returnData returnData_textVision = await task_returnData_textVision;
                textVisionClass = returnData_textVision.Data.ObjToClass<textVisionClass>();
                content.實收數量 = 實收數量.ToString();
                content.Sub_content.Sort(new ICP_sub_content());
                content.textVision = textVisionClass;
                returnData.Data = content;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得驗收內容成功!";
                returnData.Method = "content_get_by_content_GUID";

                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以GUID取得驗收單內驗收藥品中的明細
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

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            inspectionClass.content content = returnData.Data.ObjToClass<inspectionClass.content>(); ;
            //if (returnData.Value.StringIsEmpty())
            //{
            //    returnData.Code = -5;
            //    returnData.Result = "輸入資料不得空白!";
            //    return returnData.JsonSerializationt();
            //}

            string GUID = content.GUID;
            returnData.Data = null;
            List<object[]> list_inspection_sub_content = sQLControl_inspection_sub_content.GetAllRows(null);
            List<object[]> list_inspection_sub_content_buf = new List<object[]>();
            list_inspection_sub_content_buf = list_inspection_sub_content.GetRows((int)enum_驗收明細.Master_GUID, GUID);
            List<inspectionClass.sub_content> sub_Contents = new List<inspectionClass.sub_content>();
            for (int i = 0; i < list_inspection_sub_content_buf.Count; i++)
            {
                inspectionClass.sub_content sub_Content = list_inspection_sub_content_buf[i].SQLToClass<inspectionClass.sub_content, enum_驗收明細>();
                sub_Contents.Add(sub_Content);
            }
            returnData.Data = sub_Contents;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"取得驗收明細成功!";
            returnData.Method = "sub_content_get_by_content_GUID";

            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 新增單筆驗收藥品中的明細且刪除原本明細
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

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            List<object[]> list_inspection_content = sQLControl_inspection_content.GetAllRows(null);
            List<object[]> list_inspection_content_buf = new List<object[]>();
            List<object[]> list_add = new List<object[]>();
            inspectionClass.sub_content sub_content = returnData.Data.ObjToClass<inspectionClass.sub_content>();
            string Master_GUID = sub_content.Master_GUID;

            sQLControl_inspection_sub_content.DeleteByDefult(null, enum_驗收明細.Master_GUID.GetEnumName(), Master_GUID);

            list_inspection_content_buf = list_inspection_content.GetRows((int)enum_驗收內容.GUID, Master_GUID);
            if (list_inspection_content_buf.Count > 0)
            {
                object[] value = new object[new enum_驗收明細().GetLength()];
                value[(int)enum_驗收明細.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_驗收明細.藥品碼] = list_inspection_content_buf[0][(int)enum_驗收內容.藥品碼];
                value[(int)enum_驗收明細.料號] = list_inspection_content_buf[0][(int)enum_驗收內容.料號];
                value[(int)enum_驗收明細.驗收單號] = list_inspection_content_buf[0][(int)enum_驗收內容.驗收單號];
                value[(int)enum_驗收明細.藥品條碼1] = list_inspection_content_buf[0][(int)enum_驗收內容.藥品條碼1];
                value[(int)enum_驗收明細.藥品條碼2] = list_inspection_content_buf[0][(int)enum_驗收內容.藥品條碼2];

                value[(int)enum_驗收明細.Master_GUID] = Master_GUID;
                value[(int)enum_驗收明細.效期] = DateTime.MaxValue.ToDateString();
                value[(int)enum_驗收明細.批號] = "";
                value[(int)enum_驗收明細.實收數量] = sub_content.實收數量;
                value[(int)enum_驗收明細.操作人] = sub_content.操作人;
                value[(int)enum_驗收明細.操作時間] = DateTime.Now.ToDateTimeString();
                value[(int)enum_驗收明細.狀態] = "未鎖定";

                sub_content.藥品碼 = value[(int)enum_驗收明細.藥品碼].ObjectToString();
                sub_content.料號 = value[(int)enum_驗收明細.料號].ObjectToString();
                sub_content.驗收單號 = value[(int)enum_驗收明細.驗收單號].ObjectToString();
                sub_content.藥品條碼1 = value[(int)enum_驗收明細.藥品條碼1].ObjectToString();
                sub_content.藥品條碼2 = value[(int)enum_驗收明細.藥品條碼1].ObjectToString();
                sub_content.Master_GUID = value[(int)enum_驗收明細.Master_GUID].ObjectToString();
                sub_content.效期 = value[(int)enum_驗收明細.效期].ObjectToString();
                sub_content.操作人 = value[(int)enum_驗收明細.操作人].ObjectToString();
                sub_content.操作時間 = value[(int)enum_驗收明細.操作時間].ObjectToString();
                sub_content.狀態 = value[(int)enum_驗收明細.狀態].ObjectToString();

                list_add.Add(value);
            }
            sQLControl_inspection_sub_content.AddRows(null, list_add);
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"新增批效成功!";
            returnData.Data = sub_content;
            returnData.Method = "sub_content_add_single";

            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 新增單筆驗收藥品中的明細
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
        //[Route("sub_content_add")]
        //[HttpPost]
        //public string sub_content_add([FromBody] returnData returnData)
        //{
        //    MyTimer myTimer = new MyTimer();
        //    myTimer.StartTickTime(50000);

        //    List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
        //    sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
        //    if (sys_serverSettingClasses.Count == 0)
        //    {
        //        returnData.Code = -200;
        //        returnData.Result = $"找無Server資料!";
        //        return returnData.JsonSerializationt();
        //    }
        //    string Server = sys_serverSettingClasses[0].Server;
        //    string DB = sys_serverSettingClasses[0].DBName;
        //    string UserName = sys_serverSettingClasses[0].User;
        //    string Password = sys_serverSettingClasses[0].Password;
        //    uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

        //    SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
        //    SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
        //    SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
        //    List<object[]> list_inspection_content = sQLControl_inspection_content.GetAllRows(null);
        //    List<object[]> list_inspection_content_buf = new List<object[]>();
        //    List<object[]> list_add = new List<object[]>();
        //    inspectionClass.sub_content sub_content = returnData.Data.ObjToClass<inspectionClass.sub_content>();
        //    string Master_GUID = sub_content.Master_GUID;
        //    list_inspection_content_buf = list_inspection_content.GetRows((int)enum_驗收內容.GUID, Master_GUID);
        //    if (list_inspection_content_buf.Count == 0)
        //    {
        //        returnData.Code = -5;
        //        returnData.TimeTaken = myTimer.ToString();
        //        returnData.Result = $"找無資料!";
        //        returnData.Method = "sub_content_add";
        //        returnData.Data = null;
        //        return returnData.JsonSerializationt();
        //    }

        //    object[] value = new object[new enum_驗收明細().GetLength()];
        //    value[(int)enum_驗收明細.GUID] = Guid.NewGuid().ToString();
        //    value[(int)enum_驗收明細.藥品碼] = list_inspection_content_buf[0][(int)enum_驗收內容.藥品碼];
        //    value[(int)enum_驗收明細.料號] = list_inspection_content_buf[0][(int)enum_驗收內容.料號];
        //    value[(int)enum_驗收明細.驗收單號] = list_inspection_content_buf[0][(int)enum_驗收內容.驗收單號];
        //    value[(int)enum_驗收明細.藥品條碼1] = list_inspection_content_buf[0][(int)enum_驗收內容.藥品條碼1];
        //    value[(int)enum_驗收明細.藥品條碼1] = list_inspection_content_buf[0][(int)enum_驗收內容.藥品條碼2];

        //    value[(int)enum_驗收明細.Master_GUID] = Master_GUID;
        //    value[(int)enum_驗收明細.效期] = sub_content.效期;
        //    value[(int)enum_驗收明細.批號] = sub_content.批號;
        //    value[(int)enum_驗收明細.實收數量] = sub_content.實收數量;
        //    value[(int)enum_驗收明細.操作人] = sub_content.操作人;
        //    value[(int)enum_驗收明細.操作時間] = DateTime.Now.ToDateTimeString();
        //    value[(int)enum_驗收明細.狀態] = "未鎖定";

        //    list_add.Add(value);

        //    sQLControl_inspection_sub_content.AddRows(null, list_add);

        //    inspectionClass.content content = new inspectionClass.content();
        //    content.GUID = Master_GUID;
        //    returnData.Data = content;
        //    string json_content = content_get_by_content_GUID(returnData);
        //    returnData = json_content.JsonDeserializet<returnData>();
        //    if (returnData == null)
        //    {
        //        returnData.Code = -5;
        //        returnData.TimeTaken = myTimer.ToString();
        //        returnData.Result = $"搜尋content資料錯誤!";
        //        returnData.Method = "sub_content_add";
        //        returnData.Data = null;
        //        return returnData.JsonSerializationt();
        //    }
        //    if (returnData.Code < 0)
        //    {
        //        returnData.TimeTaken = myTimer.ToString();
        //        returnData.Method = "sub_content_add";
        //        returnData.Data = null;
        //        return returnData.JsonSerializationt();
        //    }

        //    returnData.Code = 200;
        //    returnData.TimeTaken = myTimer.ToString();
        //    returnData.Result = $"新增批效成功!";
        //    returnData.Method = "sub_content_add";
        //    return returnData.JsonSerializationt();
        //}
        /// <summary>
        /// 新增單筆驗收藥品中的明細
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
        [HttpPost("sub_content_add")]
        public async Task<string> sub_content_add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"Data資料不可為空";
                    return returnData.JsonSerializationt();
                }
                List<inspectionClass.sub_content> sub_contents = returnData.Data.ObjToClass<List<inspectionClass.sub_content>>();
                inspectionClass.sub_content sub_Content = new inspectionClass.sub_content();
                if (sub_contents == null)
                {
                    sub_Content = returnData.Data.ObjToClass<inspectionClass.sub_content>();
                    if (sub_Content != null) sub_contents = new List<inspectionClass.sub_content> { sub_Content };
                }
                if (sub_contents == null)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"Data資料應為[inspectionClass.sub_content]";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                List<string> GUIDs = sub_contents.Select(x => x.Master_GUID).Distinct().ToList();
                string inClause = string.Join(",", GUIDs.Select(g => $"'{g}'"));  // 加上單引號包起來
                string command = $"SELECT * FROM {DB}.inspection_content WHERE GUID IN ({inClause});";
                DataTable dataTable_inspection_content = sQLControl_inspection_content.WtrteCommandAndExecuteReader(command);
                List<object[]> list_inspection_content = dataTable_inspection_content.DataTableToRowList();
                if (list_inspection_content.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"找無資料!";
                    returnData.Method = "sub_contents_add";
                    returnData.Data = null;
                    return returnData.JsonSerializationt();
                }
                List<object[]> list_add = new List<object[]>();
                foreach (var item in sub_contents)
                {
                    List<object[]> list_inspection_content_buf = list_inspection_content.GetRows((int)enum_驗收內容.GUID, item.Master_GUID);
                    object[] value = new object[new enum_驗收明細().GetLength()];
                    value[(int)enum_驗收明細.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_驗收明細.藥品碼] = list_inspection_content_buf[0][(int)enum_驗收內容.藥品碼];
                    value[(int)enum_驗收明細.料號] = list_inspection_content_buf[0][(int)enum_驗收內容.料號];
                    value[(int)enum_驗收明細.驗收單號] = list_inspection_content_buf[0][(int)enum_驗收內容.驗收單號];
                    value[(int)enum_驗收明細.藥品條碼1] = list_inspection_content_buf[0][(int)enum_驗收內容.藥品條碼1];
                    value[(int)enum_驗收明細.藥品條碼1] = list_inspection_content_buf[0][(int)enum_驗收內容.藥品條碼2];

                    value[(int)enum_驗收明細.Master_GUID] = item.Master_GUID;
                    value[(int)enum_驗收明細.效期] = item.效期;
                    value[(int)enum_驗收明細.批號] = item.批號;
                    value[(int)enum_驗收明細.實收數量] = item.實收數量;
                    value[(int)enum_驗收明細.操作人] = item.操作人;
                    value[(int)enum_驗收明細.操作時間] = DateTime.Now.ToDateTimeString();
                    value[(int)enum_驗收明細.狀態] = "未鎖定";

                    list_add.Add(value);
                }
                sQLControl_inspection_sub_content.AddRows(null, list_add);
                if (GUIDs.Count == 1)
                {
                    inspectionClass.content content = new inspectionClass.content();
                    content.GUID = sub_Content.Master_GUID;
                    returnData.Data = content;
                    string json_content = await content_get_by_content_GUID(returnData);
                    returnData = json_content.JsonDeserializet<returnData>();
                    if (returnData == null)
                    {
                        returnData.Code = -5;
                        returnData.TimeTaken = myTimerBasic.ToString();
                        returnData.Result = $"搜尋content資料錯誤!";
                        returnData.Method = "sub_content_add";
                        returnData.Data = null;
                        return returnData.JsonSerializationt();
                    }
                    if (returnData.Code < 0)
                    {
                        returnData.TimeTaken = myTimerBasic.ToString();
                        returnData.Method = "sub_content_add";
                        returnData.Data = null;
                        return returnData.JsonSerializationt();
                    }
                }
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"新增批效成功!";
                returnData.Method = "sub_content_add";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"{ex.Message}";
                returnData.Method = "sub_content_add";
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        /// 驗收明細批次更新 API。
        /// 接收 <paramref name="returnData"/> 中的 <c>Data</c>（陣列），
        /// 轉為 <c>List&lt;inspectionClass.sub_content&gt;</c> 後，依 <c>GUID</c> 進行資料庫更新；
        /// 成功時回傳更新筆數與原始資料，失敗則回傳錯誤訊息。
        /// </summary>
        /// <remarks>
        /// <para>處理流程：</para>
        /// <list type="number">
        ///   <item><description>檢查 <c>returnData.Data</c> 是否為 <c>null</c>；為空則回傳 <c>Code = -200</c> 與「Data資料不可為空」。</description></item>
        ///   <item><description>嘗試將 <c>Data</c> 反序列化為 <c>List&lt;inspectionClass.sub_content&gt;</c>；失敗則回傳 <c>Code = -200</c> 與「Data資料格式錯誤」。</description></item>
        ///   <item><description>過濾：僅保留 <c>GUID</c> 非空之項目。</description></item>
        ///   <item><description>透過 <c>GetServerInfoAsync("Main","網頁","VM端")</c> 取得 DB 連線資訊。</description></item>
        ///   <item><description>使用 <c>SQLControl</c> 指向資料表 <c>inspection_sub_content</c>，
        ///     以 <c>ClassToSQL&lt;inspectionClass.sub_content, enum_驗收明細&gt;()</c> 轉為列資料，
        ///     呼叫 <c>UpdateRowsAsync</c> 進行批次更新。</description></item>
        ///   <item><description>成功：<c>Code = 200</c>、<c>Method = "sub_content_update"</c>、<c>Result = "更新批效成功,共{N}筆資料"</c>，
        ///     並回傳更新的 <c>sub_contents</c>。</description></item>
        ///   <item><description>例外：捕捉 <c>Exception</c>，以 <c>Code = -200</c> 回傳，<c>Result</c> 含錯誤訊息。</description></item>
        /// </list>
        /// </remarks>
        ///
        /// <param name="returnData">
        /// 請求/回應包裹物件。<br/>
        /// <b>請求重點：</b>
        /// <list type="bullet">
        ///   <item><description><c>Data</c>：必填；為 <c>inspectionClass.sub_content</c> 之陣列。</description></item>
        ///   <item><description><c>Data[i].GUID</c>：必填；作為更新鍵值。</description></item>
        /// </list>
        /// <b>sub_content 主要欄位（常用）：</b>
        /// <list type="table">
        ///   <item><term>GUID</term><description>主鍵（更新依據）。</description></item>
        ///   <item><term>Master_GUID</term><description>主檔 GUID。</description></item>
        ///   <item><term>ACPT_SN</term><description>驗收單號。</description></item>
        ///   <item><term>CODE / NAME / CHT_NAME / SKDIACODE</term><description>藥品代碼/名稱/中文名/料號。</description></item>
        ///   <item><term>BARCODE1 / BARCODE2</term><description>條碼。</description></item>
        ///   <item><term>END_QTY / TOLTAL_QTY</term><description>此次批效數量 / 總量。</description></item>
        ///   <item><term>VAL</term><description>效期（建議使用 <c>yyyy-MM-dd HH:mm:ss</c>）。</description></item>
        ///   <item><term>LOT</term><description>批號。</description></item>
        ///   <item><term>OP_TIME / OP</term><description>操作時間 / 操作者。</description></item>
        ///   <item><term>STATE / NOTE</term><description>狀態 / 備註（如「未鎖定」）。</description></item>
        ///   <item><term>PAKAGE</term><description>包裝單位（如 VIAL）。</description></item>
        /// </list>
        /// </param>
        ///
        /// <returns>
        /// 回傳 <c>string</c>（序列化 JSON）。常見回傳：
        /// <list type="table">
        ///   <item><term>成功</term><description><c>Code = 200</c>、<c>Result</c> 含更新筆數、<c>Data</c> 回傳更新內容、<c>Method = "sub_content_update"</c>。</description></item>
        ///   <item><term>失敗</term><description><c>Code = -200</c>，<c>Result</c> 含錯誤原因（如資料為空/格式錯誤/例外訊息）。</description></item>
        /// </list>
        /// </returns>
        ///
        /// <response code="200">
        /// { "Code":200, "Result":"更新批效成功,共{N}筆資料", "Method":"sub_content_update", "Data":[...] }
        /// </response>
        /// <response code="-200">
        /// { "Code":-200, "Result":"錯誤訊息", "Data":null }
        /// </response>
        ///
        /// <example>
        /// 請求範例（POST Body）：
        /// <![CDATA[
        /// {
        ///   "Data": [
        ///     {
        ///       "GUID": "5984517b-5ce0-4847-8be6-77678e2e18bd",
        ///       "Master_GUID": "e0320fe8-dc58-4d0f-a926-8ae3d1b56130",
        ///       "ACPT_SN": "20250911-0",
        ///       "CODE": "IOXA1",
        ///       "NAME": "Oxacillin inj. 1gm",
        ///       "SKDIACODE": "OXAI11",
        ///       "BARCODE1": "",
        ///       "BARCODE2": "",
        ///       "END_QTY": "30",
        ///       "VAL": "2025/09/27 00:00:00",
        ///       "LOT": "qqq",
        ///       "OP_TIME": "2025/09/23 12:03:11",
        ///       "OP": "Anna",
        ///       "STATE": "未鎖定",
        ///       "NOTE": "",
        ///       "CHT_NAME": "歐斯力娜乾粉注射劑",
        ///       "PAKAGE": "VIAL",
        ///       "TOLTAL_QTY": "30"
        ///     }
        ///   ]
        /// }
        /// ]]>
        /// </example>
        ///
        /// <example>
        /// 成功回應範例：
        /// <![CDATA[
        /// {
        ///   "Code": 200,
        ///   "TimeTaken": "0.123s",
        ///   "Data": [ { "GUID":"5984517b-5ce0-4847-8be6-77678e2e18bd", "CODE":"IOXA1", "NAME":"Oxacillin inj. 1gm", "LOT":"qqq", "VAL":"2025/09/27 00:00:00", "END_QTY":"30", "OP":"Anna" } ],
        ///   "Result": "更新批效成功,共1筆資料",
        ///   "Method": "sub_content_update"
        /// }
        /// ]]>
        /// </example>
        ///
        /// <exception cref="System.Exception">
        /// 當資料庫更新過程發生非預期錯誤時，會被捕捉並以 <c>Code = -200</c> 形式回傳；
        /// 方法本身不拋出例外給呼叫端（例外被轉為回傳內容）。
        /// </exception>
        [HttpPost("sub_content_update")]
        public async Task<string> sub_content_updateAsync([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"Data資料不可為空";
                    return returnData.JsonSerializationt();
                }
                List<inspectionClass.sub_content> sub_contents = returnData.Data.ObjToListClass<inspectionClass.sub_content>();
                if (sub_contents == null)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"Data資料格式錯誤";
                    return returnData.JsonSerializationt();
                }
                sub_contents = sub_contents.Where(x => x.GUID.StringIsEmpty() == false).ToList();
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                List<object[]> list_sub_Contents = sub_contents.ClassToSQL<inspectionClass.sub_content, enum_驗收明細>();
                await sQLControl_inspection_sub_content.UpdateRowsAsync(null, list_sub_Contents);


                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = sub_contents;
                returnData.Result = $"更新批效成功,共{sub_contents.Count}筆資料";
                returnData.Method = "sub_content_update";
                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"sub_content_updateAsync Error:{ex.Message}";
                return returnData.JsonSerializationt();
            }

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public string sub_content_update([FromBody] returnData returnData)
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

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            List<inspectionClass.sub_content> sub_Contents = returnData.Data.ObjToClass<List<inspectionClass.sub_content>>();
            List<object[]> list_sub_Contents = sub_Contents.ClassToSQL<inspectionClass.sub_content, enum_驗收明細>();



            sQLControl_inspection_sub_content.UpdateByDefulteExtra(null, list_sub_Contents);


            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"更新批效成功,共{sQLControl_inspection_sub_content}筆資料";
            returnData.Method = "sub_content_update";
            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 以GUID刪除驗收藥品中的明細
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
        public async Task<string> sub_contents_delete_by_GUID([FromBody] returnData returnData)
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

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
            List<inspectionClass.sub_content> sub_contents = returnData.Data.ObjToListClass<inspectionClass.sub_content>();
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
            sQLControl_inspection_sub_content.DeleteExtra(null, enum_驗收明細.GUID.GetEnumName(), list_GUID);

            inspectionClass.content content = new inspectionClass.content();
            content.GUID = Master_GUID;
            returnData.Data = content;
            string json_content = await content_get_by_content_GUID(returnData);
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
            returnData.Result = $"已刪除驗收明細共<{list_GUID.Count}>筆資料!";
            returnData.Method = "sub_contents_delete_by_GUID";

            return returnData.JsonSerializationt();
        }
        /// <summary>
        /// 依 <c>藥品碼</c> 批次查詢驗收明細（inspection_sub_content），
        /// 並以 <c>操作時間</c> 由新到舊排序後回傳。
        /// </summary>
        /// <remarks>
        /// <para>
        /// 本 API 會：
        /// </para>
        /// <list type="number">
        ///   <item><description>檢核 <c>returnData.ValueAry</c> 必須存在且僅含 1 個字串；否則回傳錯誤。</description></item>
        ///   <item><description>將 <c>ValueAry[0]</c> 以分號 <c>;</c> 拆解為多個 <c>藥品碼</c>，並去重（<c>Distinct()</c>）。</description></item>
        ///   <item><description>連線至資料表 <c>inspection_sub_content</c>，以欄位 <c>enum_驗收明細.藥品碼</c> 進行查詢。</description></item>
        ///   <item><description>將查得資料轉為 <c>inspectionClass.sub_content</c> 清單，並依 <c>操作時間</c> 進行遞減排序。</description></item>
        ///   <item><description>包裝為標準回傳格式（含 <c>Code</c>、<c>Result</c>、<c>TimeTaken</c>、<c>Method</c>、<c>Data</c>）。</description></item>
        /// </list>
        ///
        /// <para><b>輸入格式</b></para>
        /// <para>
        /// <c>returnData.ValueAry</c> 應為長度 1 的陣列；唯一元素為以分號 <c>;</c> 串接的多個 <c>藥品碼</c> 字串。
        /// 例如：<c>"OPER7;AB123;C0001"</c>
        /// </para>
        ///
        /// <para><b>排序規則</b></para>
        /// <para>回傳之 <c>sub_contents</c> 依 <c>操作時間</c> 以 <i>新到舊</i>（遞減）排序。</para>
        ///
        /// <para><b>成功回傳（HTTP 200 / Code=200）</b></para>
        /// <code language="json">
        /// {
        ///   "Code": 200,
        ///   "Result": "查詢驗收明細&lt;3&gt;筆成功!",
        ///   "TimeTaken": "00:00:123",
        ///   "Method": "sub_contents_get_by_code",
        ///   "Data": [
        ///     {
        ///       "GUID": "...",
        ///       "Master_GUID": "...",
        ///       "藥品碼": "OPER7",
        ///       "藥品名稱": "Carditonin 25mg",
        ///       "操作時間": "2025/09/30 14:20:33",
        ///       "...": "其他 enum_驗收明細 對應欄位"
        ///     }
        ///   ]
        /// }
        /// </code>
        ///
        /// <para><b>失敗回傳（HTTP 200 / Code=-200）</b></para>
        /// <code language="json">
        /// {
        ///   "Code": -200,
        ///   "Result": "ValueAry不得為空",
        ///   "TimeTaken": "00:00:001",
        ///   "Method": "sub_contents_get_by_code",
        ///   "Data": null
        /// }
        /// </code>
        ///
        /// <para><b>請求範例</b></para>
        /// <code language="json">
        /// {
        ///   "Data": null,
        ///   "Value": "",
        ///   "ValueAry": ["OPER7;AB123;C0001"],
        ///   "TableName": "",
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "TimeTaken": ""
        /// }
        /// </code>
        ///
        /// <para><b>備註</b></para>
        /// <list type="bullet">
        ///   <item><description><c>ValueAry</c> 長度必須為 1；內容以分號分隔多個藥碼。</description></item>
        ///   <item><description>若查無資料，仍回傳 <c>Code=200</c>，但 <c>Data</c> 可能為空陣列。</description></item>
        ///   <item><description><c>TimeTaken</c> 為伺服端量測之耗時字串，供除錯/監控使用。</description></item>
        /// </list>
        /// </remarks>
        /// <param name="returnData">
        /// 請求包裝物件：
        /// <list type="bullet">
        ///   <item><description><c>ValueAry[0]</c>：以分號分隔的藥品碼清單（必填）。</description></item>
        ///   <item><description><c>ServerName</c>/<c>ServerType</c>：將由伺服端讀取實際連線參數。</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// 一個序列化為 JSON 的字串：包含狀態碼、訊息、耗時、方法名與資料清單（<c>inspectionClass.sub_content</c> 陣列）。
        /// </returns>
        /// <exception cref="System.Exception">
        /// 異常時捕捉並回傳 <c>Code=-200</c> 與 <c>Result=ex.Message</c>。
        /// </exception>

        [HttpPost("sub_contents_get_by_code")]
        public async Task<string> sub_contents_get_by_code([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic   = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"ValueAry不得為空";
                    returnData.Method = "sub_contents_get_by_code";
                    returnData.Data = null;
                    return returnData.JsonSerializationt();
                }
                string[] codes = returnData.ValueAry[0].Split(";").Distinct().ToArray();

                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;

                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                List<object[]> rows = await sQLControl_inspection_sub_content.GetRowsByDefultAsync(null, (int)enum_驗收明細.藥品碼, codes);
                List<inspectionClass.sub_content> sub_contents = rows.SQLToClass<inspectionClass.sub_content, enum_驗收明細>();
                sub_contents = sub_contents.OrderByDescending(sc => DateTime.Parse(sc.操作時間)).ToList();

                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"查詢驗收明細<{sub_contents.Count}>筆成功!";
                returnData.Method = "sub_contents_get_by_code";
                returnData.Data = sub_contents;

                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = ex.Message;
                returnData.Method = "sub_contents_get_by_code";
                returnData.Data = null;
                return returnData.JsonSerializationt();
            }
        }

        [HttpPost("sub_contents_get_by_GUID")]
        public async Task<string> sub_contents_get_by_GUID([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"ValueAry不得為空";
                    returnData.Method = "sub_contents_get_by_GUID";
                    returnData.Data = null;
                    return returnData.JsonSerializationt();
                }
                string[] guids = returnData.ValueAry[0].Split(";").Distinct().ToArray();

                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;

                SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);
                List<object[]> rows = await sQLControl_inspection_sub_content.GetRowsByDefultAsync(null, (int)enum_驗收明細.GUID, guids);
                if (rows.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"查無資料";
                    returnData.Method = "sub_contents_get_by_GUID";
                    returnData.Data = null;
                    return returnData.JsonSerializationt();
                }
                List<inspectionClass.sub_content> sub_contents = rows.SQLToClass<inspectionClass.sub_content, enum_驗收明細>();
                sub_contents = sub_contents.OrderByDescending(sc => DateTime.Parse(sc.操作時間)).ToList();

                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"查詢驗收明細<{sub_contents.Count}>筆成功!";
                returnData.Method = "sub_contents_get_by_GUID";
                returnData.Data = sub_contents;

                return returnData.JsonSerializationt();
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = ex.Message;
                returnData.Method = "sub_contents_get_by_GUID";
                returnData.Data = null;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 以驗收單號下載
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 驗收單號 <br/> 
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
            string VM_API = Method.GetServerAPI("DS01", "藥庫", "API_inspection_excel_download");
            if (VM_API.StringIsEmpty() == false)
            {
                string json_in = returnData.JsonSerializationt();

                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(json_in, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(VM_API, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var fileBytes = await response.Content.ReadAsByteArrayAsync();
                        var contentType = response.Content.Headers.ContentType?.MediaType ??
                                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                        var fileName = $"{DateTime.Now.ToDateString("-")}_驗收表.xlsx";

                        return File(fileBytes, contentType, fileName);
                    }
                    else
                    {
                        return Content($"下載失敗：{response.StatusCode}");
                    }
                }
            }
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
            Server = server;
            DB = dbName;
            if (returnData.Code != 200)
            {
                return null;
            }
            List<inspectionClass.creat> creats = returnData.Data.ObjToListClass<inspectionClass.creat>();
            inspectionClass.creat creat = creats[0];
            List<SheetClass> sheetClasses = new List<SheetClass>();
            string loadText = Basic.MyFileStream.LoadFileAllText(@"./excel_inspection.txt", "utf-8");
            Console.WriteLine($"取得creats {myTimer.ToString()}");
            SheetClass sheetClass = loadText.JsonDeserializet<SheetClass>();
            sheetClass.ReplaceCell(1, 1, $"{creat.驗收單號}");
            sheetClass.ReplaceCell(1, 5, $"{creat.建表人}");
            sheetClass.ReplaceCell(2, 2, $"{creat.請購單號}");
            sheetClass.ReplaceCell(3, 1, $"{creat.驗收開始時間}");
            sheetClass.ReplaceCell(4, 1, $"{creat.驗收結束時間}");
            int NumOfRow = 0;
            for (int i = 0; i < creat.Contents.Count; i++)
            {
                int sub_num = creat.Contents[i].Sub_content.Count;
                int row_start = NumOfRow + 6;
                int row_end = NumOfRow + 6;
                if (sub_num > 0)
                {
                    row_end = NumOfRow + 6 + sub_num - 1;
                    sheetClass.AddNewCell_Webapi(row_start, row_end, 0, 0, $"{creat.Contents[i].藥品碼}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(row_start, row_end, 1, 1, $"{creat.Contents[i].料號}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(row_start, row_end, 2, 2, $"{creat.Contents[i].藥品名稱}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(row_start, row_end, 3, 3, $"{creat.Contents[i].中文名稱}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(row_start, row_end, 4, 4, $"{creat.Contents[i].包裝單位}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(row_start, row_end, 5, 5, $"{creat.Contents[i].應收數量}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    for (int k = 0; k < creat.Contents[i].Sub_content.Count; k++)
                    {
                        row_end = NumOfRow + 6;
                        sheetClass.AddNewCell_Webapi(row_start + k, row_end + k, 6, 6, $"{creat.Contents[i].Sub_content[k].實收數量}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Center, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(row_start + k, row_end + k, 7, 7, $"{creat.Contents[i].Sub_content[k].效期.StringToDateTime().ToDateString()}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Center, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(row_start + k, row_end + k, 8, 8, $"{creat.Contents[i].Sub_content[k].批號}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Center, NPOI.SS.UserModel.BorderStyle.Thin);

                    }
                }

                NumOfRow += sub_num;
            }
            Console.WriteLine($"寫入Sheet {myTimer.ToString()}");

            // sheetClass.NewCell_Webapi_Buffer_Caculate();
            Console.WriteLine($"NewCell_Webapi_Buffer_Caculate {myTimer.ToString()}");

            string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string xls_command = "application/vnd.ms-excel";

            byte[] excelData = sheetClass.NPOI_GetBytes(Excel_Type.xlsx);
            Stream stream = new MemoryStream(excelData);
            return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_驗收表.xlsx"));
        }
        [HttpPost("download_excle_by_content")]
        public async Task<ActionResult> download_excle_by_content([FromBody] returnData returnData)
        {
            try
            {
                string VM_API = Method.GetServerAPI("DS01", "藥庫", "API_inspection_excel_download");
                if (VM_API.StringIsEmpty() == false)
                {
                    string json_in = returnData.JsonSerializationt();

                    using (HttpClient client = new HttpClient())
                    {
                        var content = new StringContent(json_in, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(VM_API, content);

                        if (response.IsSuccessStatusCode)
                        {
                            var fileBytes = await response.Content.ReadAsByteArrayAsync();
                            var contentType = response.Content.Headers.ContentType?.MediaType ??
                                              "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                            var fileName = $"{DateTime.Now.ToDateString("-")}_驗收表.xlsx";

                            return File(fileBytes, contentType, fileName);
                        }
                        else
                        {
                            return Content($"下載失敗：{response.StatusCode}");
                        }
                    }
                }

                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data 無傳入資料";
                    return Content($"下載失敗：{returnData.Result}");
                }

                List<inspectionClass.content> contents = returnData.Data.ObjToClass<List<inspectionClass.content>>();
                if (contents == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data 無傳入資料";
                    return Content($"下載失敗：{returnData.Result}");
                }

                List<object[]> objects = new List<object[]>();
                foreach (var item in contents)
                {
                    object[] value = new object[new enum_驗收資料匯出().GetLength()];
                    value[(int)enum_驗收資料匯出.驗收單號] = item.驗收單號;
                    value[(int)enum_驗收資料匯出.請購單號] = item.請購單號;
                    value[(int)enum_驗收資料匯出.請購項次] = item.請購項次;
                    value[(int)enum_驗收資料匯出.藥碼] = item.藥品碼;
                    value[(int)enum_驗收資料匯出.藥名] = item.藥品名稱;
                    value[(int)enum_驗收資料匯出.應收數量] = item.應收數量;
                    value[(int)enum_驗收資料匯出.訂單時間] = item.訂單時間;
                    value[(int)enum_驗收資料匯出.預定交貨時間] = item.交貨時間;
                    if (item.Sub_content != null && item.Sub_content.Count > 0)
                    {
                        foreach (var sub in item.Sub_content)
                        {
                            value[(int)enum_驗收資料匯出.實收數量] = sub.實收數量;
                            value[(int)enum_驗收資料匯出.效期] = sub.效期.Substring(0,10);
                            value[(int)enum_驗收資料匯出.批號] = sub.批號;
                            value[(int)enum_驗收資料匯出.收貨時間] = sub.操作時間;
                            value[(int)enum_驗收資料匯出.操作人] = sub.操作人;

                            objects.Add(value);
                        }
                    }
                    else
                    {
                        objects.Add(value);
                    }


                    

                }
                System.Data.DataTable dataTable = objects.ToDataTable(new enum_驗收資料匯出());
                //dataTable = dataTable.ReorderTable(new enum_med_cpoe_export());

                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";
                byte[] excelData = MyOffice.ExcelClass.NPOI_GetBytes(dataTable, Excel_Type.xlsx);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_驗收資料.xlsx"));
            }
            catch
            {
                return null;
            }

        }

        [HttpPost]
        [Consumes("application/json", "multipart/form-data")]
        public async Task<string> PostExcel([FromForm] IFormFile file)
        {
            var formFile = Request.Form.Files.FirstOrDefault();

            if (formFile == null)
            {
                throw new Exception("文件不能為空");
            }
            string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string json = "";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                json = ExcelClass.NPOI_LoadSheetToJson(memoryStream.ToArray(), extension);
                // 在这里可以对 memoryStream 进行操作，例如读取或写入数据
            }

            return json;
        }

        /// <summary>
        /// 上傳Excel表單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[file] : 上傳xls/xlsx <br/> 
        ///  1.[IC_NAME] : 驗收單名稱 <br/> 
        ///  2.[PON] : 請購單號名稱 <br/> 
        ///  3.[CT] : 操作者 <br/> 
        /// </remarks>
        /// <returns>Excel</returns>
        [Route("excel_upload")]
        [HttpPost]
        public async Task<string> excel([FromForm] IFormFile file, [FromForm] string IC_NAME, [FromForm] string PON, [FromForm] string CT)
        {
            var formFile = Request.Form.Files.FirstOrDefault();

            if (formFile == null)
            {
                throw new Exception("文件不能為空");
            }
            string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端")[0];

            MED_pageController mED_PageController = new MED_pageController();
            returnData returnData_med = new returnData();
            returnData_med.Server = sys_serverSettingClasses_med.Server;
            returnData_med.DbName = sys_serverSettingClasses_med.DBName;
            returnData_med.TableName = "medicine_page_cloud";
            returnData_med.UserName = sys_serverSettingClasses_med.User;
            returnData_med.Password = sys_serverSettingClasses_med.Password;
            returnData_med.Port = sys_serverSettingClasses_med.Port.StringToUInt32();
            string json_med = Basic.Net.WEBApiPostJson("http://127.0.0.1:4433/api/MED_page", returnData_med.JsonSerializationt());
            returnData_med = json_med.JsonDeserializet<returnData>();
            List<medClass> medClasses = returnData_med.Data.ObjToListClass<medClass>();
            List<medClass> medClasses_buf = new List<medClass>();

            string json = "";
            inspectionClass.creat creat = new inspectionClass.creat();
            creat.驗收名稱 = IC_NAME;
            creat.請購單號 = PON;
            creat.建表人 = CT;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);
                List<object[]> list_value = dt.DataTableToRowList();
                for (int i = 0; i < list_value.Count; i++)
                {
                    inspectionClass.content content = new inspectionClass.content();
                    content.藥品碼 = list_value[i][(int)enum_驗收單匯入.藥碼].ObjectToString();
                    medClasses_buf = (from temp in medClasses
                                      where (temp.藥品碼 == content.藥品碼 || temp.料號 == content.藥品碼)
                                      select temp).ToList();
                    //medClasses_buf = medClasses_buf.Where(temp => temp.開檔狀態.StringIsEmpty() || temp.開檔狀態 == "開檔中").ToList();
                    medClasses_buf = medClasses_buf.Where(temp => temp.開檔狀態.Contains("關檔中") == false).ToList();

                    if (medClasses_buf.Count > 0)
                    {
                        content.藥品碼 = medClasses_buf[0].藥品碼;
                        content.料號 = medClasses_buf[0].料號;
                    }
                    else
                    {
                        content.藥品碼 = "無";
                        content.料號 = "無";
                    }
                    content.藥品名稱 = list_value[i][(int)enum_驗收單匯入.名稱].ObjectToString();
                    content.廠牌 = list_value[i][(int)enum_驗收單匯入.供應商名稱].ObjectToString();
                    content.請購單號 = list_value[i][(int)enum_驗收單匯入.請購單號].ObjectToString();
                    content.應收數量 = list_value[i][(int)enum_驗收單匯入.未交量].ObjectToString();
                    content.實收數量 = list_value[i][(int)enum_驗收單匯入.已交貨數量].ObjectToString();
                    creat.Contents.Add(content);
                }

            }
            returnData returnData = new returnData();

            returnData.Data = creat;

            return creat_add(returnData);
        }
        /// <summary>
        /// 由前端上傳 Excel 檔（表單 <c>multipart/form-data</c>）後，解析每一列資料，
        /// 依藥碼或料號對照雲端藥檔（過濾「關檔中」），組裝為 <see cref="inspectionClass.content"/> 清單，
        /// 最後以 <c>returnData.Data</c> 帶入後呼叫 <c>content_add</c> 進行資料新增處理。
        /// </summary>
        /// <remarks>
        /// 路由：<c>[POST] /excel_upload_extra</c><br/>
        /// 上傳檔案透過 <c>Request.Form.Files</c> 取得第一個檔案並讀取副檔名，
        /// 使用 <c>ExcelClass.NPOI_LoadFile</c> 讀入成 <c>DataTable</c>，
        /// 依 <c>enum_驗收單匯入</c> 欄位索引取值並轉為 <see cref="inspectionClass.content"/> 欄位：
        /// <list type="bullet">
        /// <item><description><c>藥品碼</c>：若能在 <c>medClass.get_med_cloud()</c> 結果中以「藥品碼 / 料號」符合，且「開檔狀態」不含「關檔中」，則以該筆的「藥品碼 / 料號」覆蓋；否則填入「無」。</description></item>
        /// <item><description><c>藥品名稱、廠牌、請購單號、應收數量、實收數量</c>：由 Excel 對應欄位帶入。</description></item>
        /// </list>
        /// 完成後將集合指定至 <c>returnData.Data</c>，並回傳 <c>content_add(returnData)</c> 的結果字串。
        /// </remarks>
        /// <param name="file">
        /// 由 <c>multipart/form-data</c> 上傳的檔案欄位（方法內實際以 <c>Request.Form.Files.FirstOrDefault()</c> 取得）。  
        /// 若未上傳檔案或檔案為空，將擲出例外。</param>
        /// <returns>
        /// 非同步回傳字串結果，內容為 <c>content_add</c> 的回應（通常為序列化後的 <c>returnData</c>）。
        /// </returns>
        /// <exception cref="System.Exception">
        /// 當未取得任何上傳檔案時，擲出訊息為「文件不能為空」的例外。
        /// </exception>
        /// <example>
        /// 範例（前端）：
        /// <code>
        /// POST /excel_upload_extra
        /// Content-Type: multipart/form-data
        /// Form-Field: file = &lt;Excel 檔案&gt;
        /// </code>
        /// </example>
        [Route("excel_upload_extra")]
        [HttpPost]
        public async Task<string> excel_upload_extra([FromForm] IFormFile file)
        {
            string VM_API = Method.GetServerAPI("DS01", "藥庫", "API_inspection_excel_upload");
            if (VM_API.StringIsEmpty() == false)
            {
                using (var client = new HttpClient())
                using (var form = new MultipartFormDataContent())
                {
                    // 把 IFormFile 轉成 StreamContent
                    using (var stream = file.OpenReadStream())
                    {
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                        // 加入到 multipart form，name 要和 API 預期的欄位名一致
                        form.Add(fileContent, "file", file.FileName);

                        // 發送 POST 請求
                        var response = await client.PostAsync(VM_API, form);
                        response.EnsureSuccessStatusCode();

                        // 讀取回應內容
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
            var formFile = Request.Form.Files.FirstOrDefault();

            if (formFile == null)
            {
                throw new Exception("文件不能為空");
            }
            string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

           
            List<medClass> medClasses = medClass.get_med_cloud("http://127.0.0.1:4433");
            List<medClass> medClasses_buf = new List<medClass>();

            string json = "";
            List< inspectionClass.content > contents = new List<inspectionClass.content>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);
                List<object[]> list_value = dt.DataTableToRowList();
                
                for (int i = 0; i < list_value.Count; i++)
                {
                    inspectionClass.content content = new inspectionClass.content();
                    content.藥品碼 = list_value[i][(int)enum_驗收單匯入.藥碼].ObjectToString();
                    medClasses_buf = (from temp in medClasses
                                      where (temp.藥品碼 == content.藥品碼 || temp.料號 == content.藥品碼)
                                      select temp).ToList();
                    medClasses_buf = medClasses_buf.Where(temp => temp.開檔狀態.Contains("關檔中") == false).ToList();

                    if (medClasses_buf.Count > 0)
                    {
                        content.藥品碼 = medClasses_buf[0].藥品碼;
                        content.料號 = medClasses_buf[0].料號;
                    }
                    else
                    {
                        content.藥品碼 = "無";
                        content.料號 = "無";
                    }
                    content.藥品名稱 = list_value[i][(int)enum_驗收單匯入.名稱].ObjectToString();
                    content.廠牌 = list_value[i][(int)enum_驗收單匯入.供應商名稱].ObjectToString();
                    content.請購單號 = list_value[i][(int)enum_驗收單匯入.請購單號].ObjectToString();
                    content.應收數量 = list_value[i][(int)enum_驗收單匯入.未交量].ObjectToString();
                    content.實收數量 = list_value[i][(int)enum_驗收單匯入.已交貨數量].ObjectToString();
                    content.Master_GUID = $"{content.請購單號}_{content.請購項次}_{content.驗收單號}";
                    contents.Add(content);
                }

            }
            returnData returnData = new returnData();

            returnData.Data = contents;

            return await content_add(returnData);
        }
        /// <summary>
        /// 上傳Excel表單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[file] : 上傳xls/xlsx <br/> 
        ///  1.[IC_SN] : 驗收單號 <br/> 
        ///  2.[CT] : 操作者 <br/> 
        /// </remarks>
        /// <returns>Excel</returns>
        [HttpPost("excel_upload_sub_content")]
        public async Task<string> excel([FromForm] IFormFile file, [FromForm] string IC_SN, [FromForm] string CT)
        {
            returnData returnData = new returnData();
            var formFile = Request.Form.Files.FirstOrDefault();

            if (formFile == null)
            {
                returnData.Code = -200;
                returnData.Result = "文件不可為空";
                return returnData.JsonSerializationt(true);
            }
            if (IC_SN.StringIsEmpty())
            {
                returnData.Code = -200;
                returnData.Result = "IC_SN(驗收單號)不可為空";
                return returnData.JsonSerializationt(true);
            }
            if (CT.StringIsEmpty())
            {
                returnData.Code = -200;
                returnData.Result = "CT(操作人)不可為空";
                return returnData.JsonSerializationt(true);
            }
            List<Task> tasks = new List<Task>();
            inspectionClass.creat creat = new inspectionClass.creat();
            tasks.Add(Task.Run(new Action(delegate
            {
                returnData.Value = IC_SN;
                string create_string = creat_get_by_IC_SN(returnData);
                returnData = create_string.JsonDeserializet<returnData>();
                creat = returnData.Data.ObjToClass<List<inspectionClass.creat>>()[0];
            })));

            string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<object[]> list_value = new List<object[]>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);
                dt = dt.ReorderTable(new enum_出貨單匯入());
                if (dt == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "上傳文件表頭應為請購單號、品名、出貨數量、效期!";
                    return returnData.JsonSerializationt(true);
                }
                list_value = dt.DataTableToRowList();
            }
            if (list_value.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = "無傳入資料!";
                return returnData.JsonSerializationt(true);
            }

            Task.WhenAll(tasks).Wait();
            tasks.Clear();

            if (creat == null)
            {
                returnData.Code = -200;
                returnData.Result = $"查無此驗收單號{IC_SN}";
                return returnData.JsonSerializationt(true);
            }
            List<inspectionClass.content> content = creat.Contents;

            List<inspectionClass.sub_content> list_sub_content = new List<inspectionClass.sub_content>();
            foreach (var item in list_value)
            {

                string 請購單號 = item[(int)enum_出貨單匯入.請購單號].ObjectToString();
                inspectionClass.content content_buff = content.Where(item => item.請購單號 == 請購單號).FirstOrDefault();
                if (content_buff == null) continue;
                inspectionClass.sub_content sub_content = new inspectionClass.sub_content();
                sub_content.Master_GUID = content_buff.GUID;
                string 效期 = EditExpirydate(item[(int)enum_出貨單匯入.效期].ObjectToString());
                sub_content.效期 = 效期;
                sub_content.批號 = item[(int)enum_出貨單匯入.批號].ObjectToString();
                sub_content.實收數量 = content_buff.應收數量;
                sub_content.操作人 = CT;
                list_sub_content.Add(sub_content);
            }
            returnData.Data = list_sub_content;
            string reslut = await sub_content_add(returnData);
            return reslut;
        }
        /// <summary>
        /// 上傳Excel表單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[file] : 上傳xls/xlsx <br/> 
        ///  2.[CT] : 操作者 <br/> 
        /// </remarks>
        /// <returns>Excel</returns>
        [HttpPost("excel_upload_sub_content_extra")]
        public async Task<string> excel_upload_sub_content_extra([FromForm] IFormFile file, [FromForm] string CT)
        {
            returnData returnData = new returnData();
            var formFile = Request.Form.Files.FirstOrDefault();

            if (formFile == null)
            {
                returnData.Code = -200;
                returnData.Result = "文件不可為空";
                return returnData.JsonSerializationt(true);
            }
            if (CT.StringIsEmpty())
            {
                returnData.Code = -200;
                returnData.Result = "CT(操作人)不可為空";
                return returnData.JsonSerializationt(true);
            }            

            string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<object[]> list_value = new List<object[]>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);
                dt = dt.ReorderTable(new enum_出貨單匯入());
                if (dt == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "上傳文件表頭應為請購單號、品名、出貨數量、效期!";
                    return returnData.JsonSerializationt(true);
                }
                list_value = dt.DataTableToRowList();
            }
            if (list_value.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = "無傳入資料!";
                return returnData.JsonSerializationt(true);
            }
            List<string> IC_SN = new List<string>();
            foreach (var item in list_value)
            {
                IC_SN.Add(item[(int)enum_出貨單匯入.請購單號].ObjectToString());
            }
            List<inspectionClass.content> content = new List<inspectionClass.content>();

            returnData returnData_content = await new inspectionController().content_get_by_IC_SN(string.Join(";", IC_SN));
            if (returnData_content != null && returnData_content.Code == 200)
            {
                content = returnData_content.Data.ObjToClass<List<inspectionClass.content>>();
            }
            if (content == null || content.Count == 0) 
            {
                returnData.Code = -200;
                returnData.Result = $"查無此單號{string.Join(";", IC_SN)}";
                return returnData.JsonSerializationt(true);
            }
            List<inspectionClass.sub_content> list_sub_content = new List<inspectionClass.sub_content>();
            foreach (var item in list_value)
            {

                string 請購單號 = item[(int)enum_出貨單匯入.請購單號].ObjectToString();
                inspectionClass.content content_buff = content.Where(item => item.請購單號 == 請購單號).FirstOrDefault();
                if (content_buff == null) continue;
                inspectionClass.sub_content sub_content = new inspectionClass.sub_content();
                sub_content.Master_GUID = content_buff.GUID;
                string 效期 = EditExpirydate(item[(int)enum_出貨單匯入.效期].ObjectToString());
                sub_content.效期 = 效期;
                sub_content.批號 = item[(int)enum_出貨單匯入.批號].ObjectToString();
                sub_content.實收數量 = content_buff.應收數量;
                sub_content.操作人 = CT;
                list_sub_content.Add(sub_content);
            }
            returnData.Data = list_sub_content;
            string reslut = await sub_content_add(returnData);
            return reslut;
        }

        private returnData Function_Get_inspection_creat(sys_serverSettingClass sys_serverSettingClass, string MED_TableName, List<object[]> list_inspection_creat)
        {
            return Function_Get_inspection_creat(sys_serverSettingClass, MED_TableName, list_inspection_creat, true);
        }
        private returnData Function_Get_inspection_creat(sys_serverSettingClass sys_serverSettingClass, string MED_TableName, List<object[]> list_inspection_creat, bool allData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            SQLControl sQLControl_inspection_creat = new SQLControl(Server, DB, "inspection_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_content = new SQLControl(Server, DB, "inspection_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inspection_sub_content = new SQLControl(Server, DB, "inspection_sub_content", UserName, Password, Port, SSLMode);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端")[0];

            MED_pageController mED_PageController = new MED_pageController();
            returnData returnData_med = new returnData();
            returnData_med.Server = sys_serverSettingClasses_med.Server;
            returnData_med.DbName = sys_serverSettingClasses_med.DBName;
            returnData_med.TableName = "medicine_page_cloud";
            returnData_med.UserName = sys_serverSettingClasses_med.User;
            returnData_med.Password = sys_serverSettingClasses_med.Password;
            returnData_med.Port = sys_serverSettingClasses_med.Port.StringToUInt32();
            returnData_med = mED_PageController.Get(returnData_med).JsonDeserializet<returnData>();
            List<medClass> medClasses = returnData_med.Data.ObjToListClass<medClass>();
            List<medClass> medClasses_buf = new List<medClass>();

            returnData returnData = new returnData();
            List<object[]> list_inspection_creat_buf = new List<object[]>();
            List<object[]> list_inspection_content = sQLControl_inspection_content.GetAllRows(null);
            List<object[]> list_inspection_content_buf = new List<object[]>();
            List<object[]> list_inspection_sub_content = sQLControl_inspection_sub_content.GetAllRows(null);
            List<object[]> list_inspection_sub_content_buf = new List<object[]>();
            List<object[]> list_sub_inspection = sQLControl_inspection_sub_content.GetAllRows(null);
            List<object[]> list_sub_inspection_buf = new List<object[]>();
            string 藥品碼 = "";
            string 料號 = "";
            string 藥品名稱 = "";
            string 中文名稱 = "";
            string 包裝單位 = "";
            List<inspectionClass.creat> creats = new List<inspectionClass.creat>();
            for (int i = 0; i < list_inspection_creat.Count; i++)
            {
                inspectionClass.creat creat = list_inspection_creat[i].SQLToClass<inspectionClass.creat, enum_驗收單號>();
                if (allData)
                {
                    料號 = "";
                    藥品名稱 = "";
                    中文名稱 = "";
                    包裝單位 = "";

                    list_inspection_content_buf = list_inspection_content.GetRows((int)enum_驗收內容.Master_GUID, creat.GUID);
                    for (int k = 0; k < list_inspection_content_buf.Count; k++)
                    {
                        inspectionClass.content content = list_inspection_content_buf[k].SQLToClass<inspectionClass.content, enum_驗收內容>();
                        藥品碼 = content.藥品碼;
                        if (medClasses != null)
                        {
                            medClasses_buf = (from value in medClasses
                                              where (value.藥品碼 == content.藥品碼 || value.料號 == content.藥品碼)
                                              select value).ToList();
                            if (medClasses_buf.Count > 0)
                            {
                                料號 = medClasses_buf[0].料號;
                                藥品名稱 = medClasses_buf[0].藥品名稱;
                                中文名稱 = medClasses_buf[0].中文名稱;
                                包裝單位 = medClasses_buf[0].包裝單位;
                            }
                        }
                        content.料號 = 料號;
                        content.藥品名稱 = 藥品名稱;
                        content.中文名稱 = 中文名稱;
                        content.包裝單位 = 包裝單位;

                        int 實收數量 = 0;
                        list_inspection_sub_content_buf = list_inspection_sub_content.GetRows((int)enum_驗收明細.Master_GUID, content.GUID);
                        for (int m = 0; m < list_inspection_sub_content_buf.Count; m++)
                        {
                            inspectionClass.sub_content sub_Content = list_inspection_sub_content_buf[m].SQLToClass<inspectionClass.sub_content, enum_驗收明細>();
                            sub_Content.藥品名稱 = content.藥品名稱;
                            sub_Content.中文名稱 = content.中文名稱;
                            sub_Content.包裝單位 = content.包裝單位;
                            if (sub_Content.實收數量.StringIsInt32())
                            {

                                實收數量 += sub_Content.實收數量.StringToInt32();
                            }
                            content.Sub_content.Add(sub_Content);
                        }
                        content.實收數量 = 實收數量.ToString();
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



        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_驗收單號()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_驗收內容()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_驗收明細()));
            return tables.JsonSerializationt(true);

        }

        private class ICP_creat_by_CT_TIME : IComparer<inspectionClass.creat>
        {
            public int Compare(inspectionClass.creat x, inspectionClass.creat y)
            {
                string Code0 = x.建表時間;
                string Code1 = y.建表時間;
                return Code1.CompareTo(Code0);
            }
        }
        private class ICP_Contents : IComparer<inspectionClass.content>
        {
            public int Compare(inspectionClass.content x, inspectionClass.content y)
            {
                if (x.應收數量 == "0" && y.應收數量 == "0")
                {
                    return x.藥品碼.CompareTo(y.藥品碼);
                }
                else if (x.應收數量 != "0" && y.應收數量 != "0")
                {
                    // 先按照應收數量进行排序
                    int theoryComparison = x.應收數量.CompareTo(y.應收數量);

                    if (theoryComparison == 0)
                    {
                        // 如果應收數量相同，则按照藥品碼进行排序
                        return x.藥品碼.CompareTo(y.藥品碼);
                    }
                    else
                    {
                        return theoryComparison;
                    }
                }
                else
                {
                    // 如果只有一个项的應收數量为0，则将具有非零應收數量的项排在前面
                    return y.應收數量.CompareTo(x.應收數量);
                }
            }
        }
        private class ICP_sub_content : IComparer<inspectionClass.sub_content>
        {
            public int Compare(inspectionClass.sub_content x, inspectionClass.sub_content y)
            {
                return x.操作時間.CompareTo(y.操作時間);
            }
        }
        private string EditExpirydate(string 效期)
        {
            if (Regex.IsMatch(效期, @"^\d{3}/\d{2}/\d{2}$")) // 範例：025/08/16
            {
                效期 = "2" + 效期; // 轉為 2025/08/16
            }
            string[] formats = { "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "yyyy.MM.dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd", "yyyyMMdd" }; // 可擴展格式

            if (DateTime.TryParseExact(效期, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                效期 = date.ToDateTimeString();
            }
            else
            {
                效期 = DateTime.MinValue.ToDateTimeString();
            }
            return 效期;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> content_get_by_content_GUID(string content_GUID)
        {
            returnData returnData = new returnData();
            inspectionClass.content content = new inspectionClass.content();
            content.GUID = content_GUID;
            returnData.Data = content;

            string result = await content_get_by_content_GUID(returnData);
            return result.JsonDeserializet<returnData>();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public  async Task<returnData> content_get_by_IC_SN(string IC_SN)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(IC_SN);
           
            string result = await content_get_by_IC_SN(returnData);
            return result.JsonDeserializet<returnData>();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> sub_contents_get_by_GUID(string guid)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(guid);

            string result = await sub_contents_get_by_GUID(returnData);
            return result.JsonDeserializet<returnData>();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> sub_contents_get_by_code(string code)
        {
            returnData returnData = new returnData();
            returnData.ValueAry.Add(code);

            string result = await sub_contents_get_by_code(returnData);
            return result.JsonDeserializet<returnData>();
        }
    }
}
