using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NPOI.SS.Formula.Functions;
using SkiaSharp;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class PCMPO : ControllerBase
    {
        //static private string API_Server = "http://220.135.128.247:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
           serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
           {
               var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

               if (string.IsNullOrWhiteSpace(Password))
                   throw new SecurityException("Database password cannot be null or empty (PCMPO).");

               return (Server, DB, UserName, Password, Port);
           });
        private static string project = "PO_vision";
        private static string Message = "---------------------------------------------------------------------------";

        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(200, "textVisionClass物件", typeof(textVisionClass))]
        [HttpPost("init_textVision")]
        public string init_textVision([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
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
                return CheckCreatTable(sys_serverSettingClasses[0], new enum_textVision());
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
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
        /// <summary>
        /// 執行文字辨識
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "Data":
        ///         [
        ///             {
        ///                 "op_id":
        ///                 "op_name":
        ///                 "bsse64":
        ///             }
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("OLDanalyze")]
        public string OLDanalyze([FromBody] returnData returnData)
        {
            string file = $"{DateTime.Now.ToString("yyyyMMdd")}{DateTime.Now.ToString("HHmmss")}";
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "analyze";
            try
            {
                string API_AI = GetServerAPI("Main", "網頁", "po_vision_api");

                string API = GetServerAPI("Main", "網頁", "API01");
                string project = "PO_Vision";

                List<textVisionClass> input_textVision = returnData.Data.ObjToClass<List<textVisionClass>>();
                List<Task> tasks = new List<Task>();
                returnData return_textVisionClass = ai_analyze(API_AI, input_textVision);
                if (return_textVisionClass == null)
                {
                    string picfile = "";
                    picfile = $"NG{file}";
                    returnData.Code = -200;
                    returnData.Result = $"AI未連線 檔案名稱{picfile}";
                    Logger.Log(file, project, returnData.JsonSerializationt());
                    return returnData.JsonSerializationt(true);
                }
                tasks.Add(Task.Run(new Action(delegate
                {
                    string picfile = "";
                    if (return_textVisionClass.Result == "False")
                    {
                        picfile = "NG" + file + ".jpg";
                    }
                    else
                    {
                        picfile = file + ".jpg";
                    }
                    string base64 = input_textVision[0].圖片;

                    SavePic(picfile, base64, "Po_vision");
               
                })));
                List<textVisionClass> textVisionClass_AI = new List<textVisionClass>();
                List<positionClass> positionClasses = new List<positionClass>();

                if (return_textVisionClass.Result == "False")
                {
                    Task.WhenAll(tasks).Wait();
                    string picfile = "";
                    picfile = $"NG{file}";
                    returnData.Code = -200;
                    returnData.Result = $"辨識失敗 檔案名稱{picfile}";
                    Logger.Log(file, project, returnData.JsonSerializationt());
                    return returnData.JsonSerializationt(true);
                }
                else
                {
                    textVisionClass_AI = return_textVisionClass.Data.ObjToClass<List<textVisionClass>>();
                    textVisionClass textVision = textVisionClass_AI[0];
                    inspectionClass.content content = new inspectionClass.content();
                    if (textVision.單號.StringIsEmpty() == false)
                    {
                        content = inspectionClass.sub_content_get_by_PON(API,textVision.單號);
                        if (content == null)
                        {
                            string picfile = "";
                            picfile = $"NG{file}";
                            returnData.Code = -200;
                            returnData.Result = $"查無對應單號{textVision.單號}資料 檔案名稱{picfile}";
                            Logger.Log(file, project, returnData.JsonSerializationt());
                            return returnData.JsonSerializationt(true);
                        }
                    }
                    List<medClass> medClasses = new List<medClass>();
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        if (content.藥品名稱.StringIsEmpty())
                        {
                            textVision.藥名 = content.Sub_content[0].藥品名稱;
                        }
                        else
                        {
                            textVision.藥名 = content.藥品名稱;
                        }
                        textVision.藥品碼 = content.藥品碼;
                        textVision.批號 = content.Sub_content[0].批號;
                        textVision.效期 = content.Sub_content[0].效期;
                        textVision.數量 = content.應收數量;
                        if (textVision.藥品碼.StringIsEmpty() == false)
                        {
                            medClass medClass = medClass.get_med_clouds_by_code(API, textVision.藥品碼);
                            medClasses = new List<medClass>() { medClass };
                        }
                        //medClasses = medClass.get_med_clouds_by_name(API, textVision.藥名);
                        if (medClasses.Count > 0)
                        {
                            textVision.中文名 = medClasses[0].中文名稱;
                        }
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        if (textVision.批號位置.StringIsEmpty() == false)
                        {
                            string[] position = textVision.批號位置.Split(";");
                            (string width, string height, string center) = GetSquare(position);
                            positionClass positionClass = new positionClass
                            {
                                高 = height,
                                寬 = width,
                                中心 = center,
                                信心分數 = textVision.批號信心分數,
                                keyWord = "batch_num",
                            };
                            positionClasses.LockAdd(positionClass);
                        }
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        if (textVision.中文名位置.StringIsEmpty() == false)
                        {
                            string[] position = textVision.中文名位置.Split(";");
                            (string width, string height, string center) = GetSquare(position);
                            positionClass positionClass = new positionClass
                            {
                                高 = height,
                                寬 = width,
                                中心 = center,
                                信心分數 = textVision.中文名信心分數,
                                keyWord = "cht_name",
                            };
                            positionClasses.LockAdd(positionClass);
                        }
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        if (textVision.效期位置.StringIsEmpty() == false)
                        {
                            string[] position = textVision.效期位置.Split(";");
                            (string width, string height, string center) = GetSquare(position);
                            positionClass positionClass = new positionClass
                            {
                                高 = height,
                                寬 = width,
                                中心 = center,
                                信心分數 = textVision.效期信心分數,
                                keyWord = "expirydate",
                            };
                            positionClasses.LockAdd(positionClass);

                        }
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        if (textVision.藥名位置.StringIsEmpty() == false)
                        {
                            string[] position = textVision.藥名位置.Split(";");
                            (string width, string height, string center) = GetSquare(position);
                            positionClass positionClass = new positionClass
                            {
                                高 = height,
                                寬 = width,
                                中心 = center,
                                信心分數 = textVision.藥名信心分數,
                                keyWord = "name",
                            };
                            positionClasses.LockAdd(positionClass);

                        }
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        if (textVision.單號位置.StringIsEmpty() == false)
                        {
                            string[] position = textVision.單號位置.Split(";");
                            (string width, string height, string center) = GetSquare(position);
                            positionClass positionClass = new positionClass
                            {
                                高 = height,
                                寬 = width,
                                中心 = center,
                                信心分數 = textVision.單號信心分數,
                                keyWord = "po",
                            };
                            positionClasses.LockAdd(positionClass);
                        }
                    })));
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        if (textVision.數量位置.StringIsEmpty() == false)
                        {
                            string[] position = textVision.數量位置.Split(";");
                            (string width, string height, string center) = GetSquare(position);
                            positionClass positionClass = new positionClass
                            {
                                高 = height,
                                寬 = width,
                                中心 = center,
                                信心分數 = textVision.數量信心分數,
                                keyWord = "qty",
                            };
                            positionClasses.LockAdd(positionClass);
                        }
                    })));
                    Task.WhenAll(tasks).Wait();
                    textVision.識別位置 = positionClasses;
                    textVision.圖片 = input_textVision[0].圖片;
                    textVisionClass_AI[0] = textVision;
                }

                returnData.Code = 200;
                returnData.Data = textVisionClass_AI;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"辨識成功 檔案名稱{file}";
                returnData.Method = "PCMPO/analyze";
                Logger.Log(file, project, returnData.JsonSerializationt());
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }


        }
        /// <summary>
        /// 執行文字辨識
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///             "GUID":
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("analyze")]
        public string analyze([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "api/pcmpo/analyze";
            try
            {
                if (returnData.ValueAry.Count == 0)
                {
                    returnData.Result = "returnData.ValueAry無傳入資料";
                    returnData.Code = -200;
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Result = "returnData.ValueAry應為[\"GUID\"]";
                    returnData.Code = -200;
                    return returnData.JsonSerializationt(true);
                }

                string GUID = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string API_AI = GetServerAPI("Main", "網頁", "po_vision_api");
                string API = GetServerAPI("Main", "網頁", "API01");
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);
                List<object[]> getDateByGuid = sQLControl_textVision.GetRowsByDefult(null, (int)enum_textVision.GUID, GUID);
                List<textVisionClass> textVisionClasses = getDateByGuid.SQLToClass<textVisionClass, enum_textVision>();
                if (textVisionClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此 GUID {GUID}";
                    Logger.Log(project, returnData.JsonSerializationt());
                    Logger.Log(project, Message);
                    return returnData.JsonSerializationt(true);
                }

                List<object[]> update_textVisionClass = new List<object[]>();
                returnData return_textVisionClass = ai_analyze(API_AI, textVisionClasses);

                if (return_textVisionClass == null)
                {
                    returnData.Result = $"AI連線失敗 url:{API_AI}";
                    returnData.Code = -200;
                    textVisionClasses[0].Code = "-3";
                    textVisionClasses[0].Result = returnData.Result;

                    update_textVisionClass = textVisionClasses.ClassToSQL<textVisionClass, enum_textVision>();
                    sQLControl_textVision.UpdateByDefulteExtra(null, update_textVisionClass);

                    returnData.Data = clearLongData(textVisionClasses[0]);
                    return returnData.JsonSerializationt(true);
                }
                textVisionClass textVision = return_textVisionClass.Data.ObjToClass<List<textVisionClass>>()[0];
                textVision.GUID = textVisionClasses[0].GUID;
                textVision.批次ID = textVisionClasses[0].批次ID;
                textVision.操作時間 = textVisionClasses[0].操作時間;
                textVision.確認 = textVisionClasses[0].確認;
                textVision.驗收單號 = textVisionClasses[0].驗收單號;
                textVision.Code = textVisionClasses[0].Code;
                textVision.Result = textVisionClasses[0].Result;
                textVision.操作者ID = textVisionClasses[0].操作者ID;
                textVision.操作者姓名 = textVisionClasses[0].操作者姓名;
                textVision.圖片 = textVisionClasses[0].圖片;
                textVision.PRI_KEY = $"{textVision.驗收單號}_{textVision.單號}";
                object[] update_text = textVision.ClassToSQL<textVisionClass, enum_textVision>();
                sQLControl_textVision.UpdateByDefulteExtra(null, update_text);
                //if (textVision.效期.StringIsEmpty())
                //{
                //    textVision.效期 = textVisionClasses[0].效期;
                //}
                //if (return_textVisionClass.Result == "False") // 批號或效期壞辨識失敗
                //{
                //    if(textVision.效期.StringIsEmpty()) textVision.效期 = textVisionClasses[0].效期;
                //    string base64 = textVision.圖片;
                //    string fileName = $"{DateTime.Now.ToString("yyyyMMdd")}{DateTime.Now.ToString("HHmmss")}.txt";
                //    SavePic(base64, project);
                //    if (textVision.單號.StringIsEmpty())
                //    {
                //        fileName = $"{DateTime.Now.ToString("yyyyMMdd")}{DateTime.Now.ToString("HHmmss")}.txt";
                //        SavePic(base64, project);
                //    }


                //    returnData.Data = textVision;
                //    Logger.Log(fileName, project, returnData.JsonSerializationt());
                //}
                if (return_textVisionClass.Result == "False" || textVision.單號.StringIsEmpty()) // 批號或效期壞辨識失敗
                {
                    string base64 = textVision.圖片;
                    string fileName = $"{DateTime.Now.ToString("yyyyMMdd")}{DateTime.Now.ToString("HHmmss")}.txt";
                    SavePic(base64, project);

                    textVision.Code = "-1";
                    textVision.Result = $"辨識單號失敗";
                    if (textVision.效期.Check_Date_String() == false)
                    {
                        textVision.效期 = DateTime.MinValue.ToDateTimeString();
                    }
                    else
                    {
                        textVision = EditExpirydate(textVision);
                    }
                    object[] update = textVision.ClassToSQL<textVisionClass, enum_textVision>();
                    sQLControl_textVision.UpdateByDefulteExtra(null, update);

                    returnData.Data = clearLongData(textVision);
                    returnData.Result = textVision.Result;
                    returnData.Code = -200;
                    Logger.Log(fileName, project, returnData.JsonSerializationt());
                    return returnData.JsonSerializationt(true);

                }
                //換API
                if (returnData.Value == "Y")
                {
                    returnData.Code = 200;
                    returnData.TimeTaken = $"{myTimerBasic}";
                    returnData.Result = $"AI 辨識完成";
                    returnData.Data = textVision;
                    return returnData.JsonSerializationt(true);
                }
                else
                {
                    returnData returnData_poNum = textVisionClass.analyze_by_po_num(API, textVision);
                    return returnData_poNum.JsonSerializationt(true);
                }



                //return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("analyze_new")]
        public string analyze_new([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "api/pcmpo/analyze_new";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Result = "returnData.ValueAry無傳入資料";
                    returnData.Code = -200;
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Result = "returnData.ValueAry應為[\"GUID\"]";
                    returnData.Code = -200;
                    return returnData.JsonSerializationt(true);
                }

                string GUID = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string API_AI = GetServerAPI("Main", "網頁", "po_vision_api");
                string API = GetServerAPI("Main", "網頁", "API01");
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);
                List<textVisionClass> textVisionClasses = sQLControl_textVision.GetRowsByDefult(null, (int)enum_textVision.GUID, GUID).SQLToClass<textVisionClass, enum_textVision>();
                if (textVisionClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無此 GUID {GUID}";
                    Logger.Log(project, returnData.JsonSerializationt());
                    Logger.Log(project, Message);
                    return returnData.JsonSerializationt(true);
                }

                List<object[]> update_textVisionClass = new List<object[]>();
                poVision return_poVision = poVision.ai_analyze(API_AI, textVisionClasses);

                if (return_poVision == null)
                {
                    returnData.Result = $"AI連線失敗 url:{API_AI}";
                    returnData.Code = -200;
                    textVisionClasses[0].Code = "-3";
                    textVisionClasses[0].Result = returnData.Result;

                    update_textVisionClass = textVisionClasses.ClassToSQL<textVisionClass, enum_textVision>();
                    sQLControl_textVision.UpdateByDefulteExtra(null, update_textVisionClass);

                    returnData.Data = clearLongData(textVisionClasses[0]);
                    return returnData.JsonSerializationt(true);
                }

                List<textVisionClass> analyzedVisions = return_poVision.Data.ObjToClass<List<textVisionClass>>();
                bool isMulti = analyzedVisions.Count > 1;

                for (int i = 0; i < analyzedVisions.Count; i++)
                {
                    textVisionClass baseTv = textVisionClasses[0];

                    analyzedVisions[i].批次ID = baseTv.批次ID;
                    analyzedVisions[i].操作時間 = baseTv.操作時間;
                    analyzedVisions[i].確認 = baseTv.確認;
                    analyzedVisions[i].驗收單號 = baseTv.驗收單號;
                    analyzedVisions[i].Code = baseTv.Code;
                    analyzedVisions[i].Result = baseTv.Result;
                    analyzedVisions[i].操作者ID = baseTv.操作者ID;
                    analyzedVisions[i].操作者姓名 = baseTv.操作者姓名;
                    analyzedVisions[i].圖片 = baseTv.圖片;

                    if (analyzedVisions[i].效期.StringIsEmpty()) analyzedVisions[i].效期 = baseTv.效期;

                    if (isMulti)
                    {
                        analyzedVisions[i].PRI_KEY = $"{analyzedVisions[i].驗收單號}_{analyzedVisions[i].單號}_{i}";
                    }
                    else
                    {
                        analyzedVisions[i].PRI_KEY = $"{analyzedVisions[i].驗收單號}_{analyzedVisions[i].單號}";
                    }

                    if(isMulti && i != 0)
                    {
                        analyzedVisions[i].GUID = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        analyzedVisions[i].GUID = baseTv.GUID;
                    }

                    // 儲存圖片失敗備援
                    if (return_poVision.Result == "False" && i == 0)
                    {
                        string fileName = string.Empty;
                        if (analyzedVisions[i].單號.StringIsEmpty())
                        {
                            fileName = $"{DateTime.Now:yyyyMMddHHmmss}.txt";
                            SavePic(analyzedVisions[i].圖片, project);
                        }
                        else
                        {
                            fileName = $"{analyzedVisions[i].單號}.txt";
                            SavePic(analyzedVisions[i].單號, analyzedVisions[i].圖片, project);

                        }
                        Logger.Log(fileName, project, returnData.JsonSerializationt());
                    }
                }

                if (returnData.Value == "Y")
                {
                    returnData.Code = 200;
                    returnData.Result = "AI 辨識完成";
                    returnData.Data = analyzedVisions;
                    returnData.TimeTaken = $"{myTimerBasic}";
                    return returnData.JsonSerializationt(true);
                }
                else
                {
                    returnData returnData_poNum = textVisionClass.analyze_by_po_num(API, analyzedVisions[0]);
                    return returnData_poNum.JsonSerializationt(true);
                }

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 執行文字辨識
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///             "GUID":
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("analyze_by_po_num")]
        public string analyze_by_po_num([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "api/pcmpo/analyze_by_po_num";
            try
            {
                textVisionClass textVision = returnData.Data.ObjToClass<textVisionClass>();
                if (textVision == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string API = GetServerAPI("Main", "網頁", "API01");
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);

                List<object[]> update_textVisionClass = new List<object[]>();
                List<positionClass> positionClasses = new List<positionClass>();
                inspectionClass.content content = new inspectionClass.content();
                returnData returnData_pri_key = get_by_pri_key(textVision.PRI_KEY);
                if (returnData_pri_key == null || returnData_pri_key.Code != 200) //找不到或錯誤
                {
                    returnData.Code = -200;
                    returnData.Result = $"取得資料失敗(get_by_pri_key)";
                    return returnData.JsonSerializationt();
                }
                List<textVisionClass> textVisions = returnData_pri_key.Data.ObjToClass<List<textVisionClass>>();

                if (textVisions != null)
                {
                    if (textVisions.Count > 1)
                    {
                        returnData.Result = "單號重複儲存，請確認";
                        returnData.Code = -200;
                        return returnData.JsonSerializationt(true);
                    }
                    if (textVisions[0].確認 == "已確認") //單號已經辨識過
                    {
                        returnData.Code = 200;
                        returnData.Result = $"此單號已辨識過 單號 {textVision.單號}";

                        textVision.Code = "-4";
                        textVision.Result = returnData.Result;

                        returnData.Data = clearLongData(textVision);
                        return returnData.JsonSerializationt(true);
                    }
                    else if (textVisions[0].確認 == "未確認" && textVisions[0].批次ID == textVision.批次ID && textVisions[0].GUID != textVision.GUID) //同一批上傳兩張一樣的
                    {
                        returnData.Code = 200;
                        returnData.Result = $"此單號已上傳過 單號 {textVision.單號}";

                        textVision.Code = "-5";
                        textVision.Result = returnData.Result;

                        //update_textVisionClass = new List<textVisionClass>() { textVision }.ClassToSQL<textVisionClass, enum_textVision>();
                        //sQLControl_textVision.UpdateByDefulteExtra(null, update_textVisionClass);

                        returnData.Data = clearLongData(textVision);
                        return returnData.JsonSerializationt(true);
                    }
                    else if (textVisions[0].確認 == "未確認" && textVisions[0].批次ID != textVision.批次ID)
                    {
                        string GUID_delete = textVisions[0].GUID;
                        textVisionClass.delete_by_GUID(API, GUID_delete);
                    }
                }

                content = inspectionClass.content_get_by_PON(API, textVision.驗收單號, textVision.單號);
                if (content == null)
                {
                    returnData.Code = 200;
                    returnData.Result = $"查無對應單號資料 單號 {textVision.驗收單號}-{textVision.單號}";

                    textVision.Code = "-2";
                    textVision.Result = returnData.Result;                  
                    if (textVision.效期.StringIsEmpty() == false) textVision = EditExpirydate(textVision);
                    update_textVisionClass = new List<textVisionClass>() { textVision }.ClassToSQL<textVisionClass, enum_textVision>();
                    sQLControl_textVision.UpdateByDefulteExtra(null, update_textVisionClass);

                    returnData.Data = clearLongData(textVision);
                    Logger.Log(project, returnData.JsonSerializationt());
                    Logger.Log(project, Message);
                    return returnData.JsonSerializationt(true);
                }


                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(new Action(delegate
                {
                    if (content.藥品名稱.StringIsEmpty() == false) textVision.藥名 = content.藥品名稱;
                    
                    textVision = EditExpirydate(textVision);                   
                    textVision.藥品碼 = content.藥品碼;
                    textVision.數量 = content.應收數量;
                    textVision.Master_GUID = content.GUID;

                    List<medClass> medClasses = new List<medClass>();
                    if (textVision.藥品碼.StringIsEmpty() == false)
                    {
                        medClass medClass = medClass.get_med_clouds_by_code(API, textVision.藥品碼);
                        medClasses = new List<medClass>() { medClass };
                    }
                    else
                    {
                        if (textVision.藥名.StringIsEmpty() == false)
                        {
                            medClasses = medClass.get_med_clouds_by_name(API, textVision.藥名);
                        }
                    }

                    if (medClasses.Count > 0 && medClasses[0] != null)
                    {
                        if (medClasses[0].中文名稱.StringIsEmpty() == false)
                        {
                            textVision.中文名 = medClasses[0].中文名稱;
                        }
                        else
                        {
                            textVision.中文名 = medClasses[0].藥品學名;
                        }
                    }
                    else
                    {                      
                        textVision.中文名 = Regex.Replace(textVision.中文名, @"^[0-9A-Za-z]+", "");
                        textVision.中文名 = Regex.Replace(textVision.中文名, @"（.*?）", "");
                    }
                })));
                Dictionary<string, (string Position, string Confidence, string Label)> dic_textVision = toDicByPosition(textVision);
                foreach (string key in dic_textVision.Keys)
                {
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        positionClass position = GetPosition(dic_textVision[key].Position, dic_textVision[key].Confidence, dic_textVision[key].Label);
                        positionClasses.LockAdd(position);

                    })));
                }

                Task.WhenAll(tasks).Wait();
                textVision.識別位置 = positionClasses;

                returnData.Code = 200;
                returnData.Result = $"辨識成功";

                textVision.Code = "200";
                textVision.Result = returnData.Result;

                update_textVisionClass = new List<textVisionClass>() { textVision }.ClassToSQL<textVisionClass, enum_textVision>();
                sQLControl_textVision.UpdateByDefulteExtra(null, update_textVisionClass);

                returnData.Data = clearLongData(textVision);
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
       
        /// <summary>
        /// 以GUID取得資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///                 "GUID",
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_GUID")]
        public string get_by_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_GUID";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"GUID\"]";
                    return returnData.JsonSerializationt(true);
                }
                string GUID = returnData.ValueAry[0];

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");

                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textvision", UserName, Password, Port, SSLMode);

                List<object[]> list_textVision = sQLControl_textVision.GetRowsByDefult(null, (int)enum_textVision.GUID, GUID);
                if (list_textVision.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<textVisionClass> textVisionClasses = list_textVision.SQLToClass<textVisionClass, enum_textVision>();

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"取得<{textVisionClasses.Count}>筆";
                returnData.Data = textVisionClasses;
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新文字辨識資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///                 "GUID",
        ///                 "po_num"
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("update_by_GUID_poNum")]
        public string update_by_GUID_poNum([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_by_GUID_poNum";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"GUID\",\"單號\"]";
                    return returnData.JsonSerializationt(true);
                }
                string GUID = returnData.ValueAry[0];
                string 單號 = returnData.ValueAry[1];

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string API = GetServerAPI("Main", "網頁", "API01");

                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textvision", UserName, Password, Port, SSLMode);

                List<object[]> list_textVision = sQLControl_textVision.GetRowsByDefult(null, (int)enum_textVision.GUID, GUID);
                if (list_textVision.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<textVisionClass> textVisionClasses = list_textVision.SQLToClass<textVisionClass, enum_textVision>();
                textVisionClass textVision = textVisionClasses[0];
                textVision.單號 = 單號;
                textVision.PRI_KEY = $"{textVision.驗收單號}-{textVision.單號}";
                returnData returnData_poNum = textVisionClass.analyze_by_po_num(API, textVision);
                return returnData_poNum.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以GUID更新圖片資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///                 "GUID",
        ///                 "base64"
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("update_pic_by_GUID")]
        public string update_pic_by_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_pic_by_GUID";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"GUID\",\"base64\"]";
                    return returnData.JsonSerializationt(true);
                }
                string GUID = returnData.ValueAry[0];
                string 圖片 = returnData.ValueAry[1];

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string API = GetServerAPI("Main", "網頁", "API01");

                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textvision", UserName, Password, Port, SSLMode);

                List<object[]> list_textVision = sQLControl_textVision.GetRowsByDefult(null, (int)enum_textVision.GUID, GUID);
                if (list_textVision.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<textVisionClass> textVisionClasses = list_textVision.SQLToClass<textVisionClass, enum_textVision>();
                textVisionClasses[0].圖片 = 圖片;
                List<object[]> Update_textVision = textVisionClasses.ClassToSQL<textVisionClass, enum_textVision>();
                sQLControl_textVision.UpdateByDefulteExtra(null, Update_textVision);

                returnData.Code = 200;
                //returnData.Data = sub_textVisionClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"編輯<{Update_textVision.Count}>筆";
                //Logger.Log(project, returnData.JsonSerializationt());
                //Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新檢索表
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "Data":
        ///         [
        ///            {
        ///                 "Master_GUID":""
        ///                 "recog_cht_name":""
        ///                 "recog_name":""
        ///                 "code":""
        ///            }
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        
        /// <summary>
        /// 以GUID刪除textVision資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///             "GUID"
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("delete_by_GUID")]
        public async Task<string> delete_by_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "delete_by_GUID";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"GUID\"]";
                    return returnData.JsonSerializationt(true);
                }

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string[] GUID = returnData.ValueAry[0].Split(";").ToArray();
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);

                List<object[]> list_textVision = await sQLControl_textVision.GetRowsByDefultAsync(null, (int)enum_textVision.GUID, GUID);
                //List<object[]> list_sub_textVision = sQLControl_sub_textVision.GetRowsByDefult(null, (int)enum_sub_textVision.Master_GUID, GUID);

                if (list_textVision.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt(true);
                }

                await sQLControl_textVision.DeleteRowsAsync(null, list_textVision);
                //sQLControl_sub_textVision.DeleteExtra(null, list_sub_textVision);
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以PRI_KEY(驗收單號-請購單號)取得資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///             "po_num"
        ///         ]
        ///         
        ///     }
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
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"驗收單號-請購單號\"]";
                    return returnData.JsonSerializationt(true);
                }

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string PRI_KEY = returnData.ValueAry[0];
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);

                List<object[]> list_textVision = sQLControl_textVision.GetRowsByDefult(null, (int)enum_textVision.PRI_KEY, PRI_KEY);
                if (list_textVision.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<textVisionClass> textVisionClasses = list_textVision.SQLToClass<textVisionClass, enum_textVision>();
               

                returnData.Code = 200;
                returnData.Data = clearLongData(textVisionClasses[0]);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"取得驗收-請購單號 : {PRI_KEY} 資料";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_by_MasterGUID")]
        public string get_by_MasterGUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_MasterGUID";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"Master_GUID\"]";
                    return returnData.JsonSerializationt(true);
                }

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string Master_GUID = returnData.ValueAry[0];
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);

                List<object[]> list_textVision = sQLControl_textVision.GetRowsByDefult(null, (int)enum_textVision.Master_GUID, Master_GUID);
                if (list_textVision.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt(true);
                }
                textVisionClass textVisionClasses = list_textVision[0].SQLToClass<textVisionClass, enum_textVision>();


                returnData.Code = 200;
                returnData.Data = textVisionClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"取得驗收-Master_GUID : {Master_GUID} 資料";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 資料預儲存
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "Data":
        ///         [
        ///             {
        ///                 "op_id":
        ///                 "op_name":
        ///                 "bsse64":
        ///                 "batch_id":"日期時間"
        ///                 "IC_SN":"驗收單號"
        ///             }
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("preSave")]
        public string preSave([FromBody] returnData returnData)
        {
            //string file = $"{DateTime.Now.ToString("yyyyMMdd")}{DateTime.Now.ToString("HHmmss")}";
            returnData.Method = "api/PCMPO/preSave";
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                List<textVisionClass> input_textVision = returnData.Data.ObjToClass<List<textVisionClass>>();
                if (input_textVision == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textvision", UserName, Password, Port, SSLMode);

                foreach (textVisionClass textVisionClass in input_textVision)
                {
                    textVisionClass.GUID = Guid.NewGuid().ToString();
                    textVisionClass.操作時間 = DateTime.Now.ToDateTimeString();
                    textVisionClass.確認 = "未確認";
                    textVisionClass.效期 = DateTime.MinValue.ToDateTimeString();
                    textVisionClass.Code = "-200";
                    textVisionClass.Result = "未辨識";
                    if (textVisionClass.圖片.StringIsEmpty())
                    {
                        string filPath = @"C:\Users\Administrator\OneDrive - 鴻森智能科技有限公司\軟體部\0.醫院客戶資料\3.衛服部立台北醫院\請購單辨識\測試單據\登銓\20250424-0 1140422003-37.jpg";
                        byte[] imageBytes = System.IO.File.ReadAllBytes(filPath);
                        string base64 = Convert.ToBase64String(imageBytes);
                        base64 = $"data:image/jpeg;base64,{base64}";
                        textVisionClass.圖片 = base64;
                    }

                }

                List<object[]> list_textVision = input_textVision.ClassToSQL<textVisionClass, enum_textVision>();
                sQLControl_textVision.AddRows(null, list_textVision);

                returnData.Code = 200;
                returnData.Data = clearLongData(input_textVision[0]);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"請購單資料預儲存成功 GUID:{input_textVision[0].GUID}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }


        }
      
        /// <summary>
        /// 以操作者ID找出最新的一批資料 "Y" Data 回傳完整資料/ "N" Data不回傳資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///                 "op_id"
        ///         ],
        ///         "Value":"Y"/"N" 
        ///         
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("preCheck")]
        public string preCheck([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "preCheck";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"操作者ID\"]";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.Value == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.Value 空白，內容應為\"Y\" or \"N\"";
                    return returnData.JsonSerializationt();
                }

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string API_Server = GetServerAPI("Main", "網頁", "API01");
                string 操作者ID = returnData.ValueAry[0];
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);
                //List<object[]> list_textVision = sQLControl_textVision.GetRowsByDefult(null, (int)enum_textVision.操作者ID, 操作者ID);
                DateTime today = DateTime.Now;

                string start = today.GetStartDate().ToDateTimeString();
                string end = today.GetEndDate().ToDateTimeString();
                string command = $"SELECT * FROM {DB}.textVision WHERE 操作時間 BETWEEN '{start}' AND '{end}' AND 操作者ID = '{操作者ID}';";
                DataTable dataTable_patient_info = sQLControl_textVision.WtrteCommandAndExecuteReader(command);
                List<object[]> list_textVision = dataTable_patient_info.DataTableToRowList();
                //List<object[]> list_textVision = sQLControl_textVision.GetRowsByBetween(null,(int)enum_textVision.操作時間, start, end);

                //List<object[]> list_textVision = sQLControl_textVision.GetAllRows(null);

                List<textVisionClass> textVisionClasses = list_textVision.SQLToClass<textVisionClass, enum_textVision>();
                //textVisionClasses = textVisionClasses.Where(temp => temp.操作者ID == 操作者ID).ToList();
                Dictionary<string, List<textVisionClass>> dicTextVision = textVisionClass.ToDicByBatchID(textVisionClasses);
                if (dicTextVision.Count == 0)
                {
                    returnData.Code = 202; //操作者最新的一批資料中沒有未確認資料
                    returnData.Result = $"id : {操作者ID} 未含有未確認資料";
                    returnData.TimeTaken = $"{myTimerBasic}";
                    return returnData.JsonSerializationt(true);
                }
                string maxBatchID = dicTextVision.Keys.Max();
                List<textVisionClass> textVisions = textVisionClass.GetValueByBatchID(dicTextVision, maxBatchID);
                bool flag = false;
                List<textVisionClass> textVision_buff = new List<textVisionClass>();
                //List<returnDataClass> returnDataClasses = new List<returnDataClass>();
                for (int i = 0; i < textVisions.Count; i++)
                {
                    if (textVisions[i].確認 == "未確認")
                    {
                        flag = true;
                        textVisions[i].圖片 = "";
                        textVisions[i].Log = "";
                        //returnDataClass returnDataClass = new returnDataClass()
                        //{
                        //    Code = textVisions[i].Code.StringToInt32(),
                        //    Result = textVisions[i].Code,
                        //    Data = new List<textVisionClass>() { textVisions[i] } 
                        //};
                        textVision_buff.Add(textVisions[i]);
                        //returnDataClasses.Add(returnDataClass);
                    }
                }
                if (flag == false)
                {
                    returnData.Code = 202; //操作者最新的一批資料中沒有未確認資料
                    returnData.Result = $"id : {操作者ID}, 批次ID : {maxBatchID}  資料共{textVisions.Count}筆，未含有未確認資料";
                    returnData.TimeTaken = $"{myTimerBasic}";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.Value == "N" && flag == true)
                {
                    returnData.Code = 201; //操作者最新的一批資料中有未確認資料
                    returnData.Result = $"id : {操作者ID}, 批次ID : {maxBatchID}  資料共{textVisions.Count}筆，含有未確認資料{textVision_buff.Count}筆";
                    returnData.TimeTaken = $"{myTimerBasic}";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.Value == "Y" && flag == true)
                {
                    returnData.Code = 201; //操作者最新的一批資料中有未確認資料
                    returnData.Data = textVision_buff;
                    returnData.Result = $"id : {操作者ID}, 批次ID : {maxBatchID}  資料共{textVisions.Count}筆，，含有未確認資料{textVision_buff.Count}筆";
                    returnData.TimeTaken = $"{myTimerBasic}";
                    return returnData.JsonSerializationt(true);
                }
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);

            }
        }
        /// <summary>
        /// 將辨識資料送出確認
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":
        ///         [
        ///                 "batchID",
        ///                 "GUID1;GUID2;GUID3"
        ///         ],
        ///         
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("Check")]
        public async Task<string> Check([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "Check";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"BatchID\",\"GUID1;GUID2\"]";
                    return returnData.JsonSerializationt(true);
                }

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string[] GUIDs = returnData.ValueAry[1].Split(";");
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);
                List<object[]> list_textVision = await sQLControl_textVision.GetRowsByDefultAsync(null, (int)enum_textVision.GUID, GUIDs);
                List<textVisionClass> textVisionClasses = list_textVision.SQLToClass<textVisionClass, enum_textVision>();
                List<textVisionClass> update_textVisionClass = new List<textVisionClass>();
                for (int i = 0; i < textVisionClasses.Count; i++)
                {                   
                    textVisionClasses[i].確認 = "已確認";
                    update_textVisionClass.Add(textVisionClasses[i]);                    
                }
                List<inspectionClass.sub_content> sub_Contents = new List<inspectionClass.sub_content>();
                
                List<object[]> list_update_textVisionClass = update_textVisionClass.ClassToSQL<textVisionClass, enum_textVision>();
                if (list_update_textVisionClass.Count > 0) sQLControl_textVision.UpdateByDefulteExtra(null, list_update_textVisionClass);



                //tasks.Add(Task.Run(new Action(delegate
                //{
                //    foreach (var textVisionClass in update_textVisionClass)
                //    {
                //        inspectionClass.sub_content sub_Content = new inspectionClass.sub_content
                //        {
                //            Master_GUID = textVisionClass.Master_GUID,
                //            效期 = textVisionClass.效期,
                //            批號 = textVisionClass.批號,
                //            實收數量 = textVisionClass.數量,
                //            操作人 = textVisionClass.操作者姓名,
                //        };
                //        sub_Contents.Add(sub_Content);
                //    }
                //})));
                //Task.WhenAll(tasks).Wait();
                //foreach (var item in sub_Contents)
                //{
                //    returnData returnData_out = inspectionClass.returnData_sub_content_add(API_Server, item);
                //    Logger.Log($"{project}/sub_content_add", returnData_out.JsonSerializationt(true));
                //    if (returnData_out.Code != 200 || returnData_out.Data == null) return returnData_out.JsonSerializationt(true);
                //}

                returnData.Code = 200;
                returnData.Data = clearLongData(update_textVisionClass);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"更新請購單資料共{update_textVisionClass.Count}筆";
                return returnData.JsonSerializationt(true);

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);

            }
        }      
        [HttpPost("recover")]
        public string recover([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "recover";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Data = -200;
                    returnData.Result = "returnData.ValueAry 空白，請輸入對應欄位資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"操作者ID\"]";
                    return returnData.JsonSerializationt(true);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string API_Server = GetServerAPI("Main", "網頁", "API01");
                string 操作者ID = returnData.ValueAry[0];
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);
                List<object[]> list_textVision = sQLControl_textVision.GetRowsByDefult(null, (int)enum_textVision.操作者ID, 操作者ID);
                List<textVisionClass> textVisionClasses = list_textVision.SQLToClass<textVisionClass, enum_textVision>();
                Dictionary<string, List<textVisionClass>> dicTextVision = textVisionClass.ToDicByBatchID(textVisionClasses);
                string maxBatchID = dicTextVision.Keys.Max();
                List<textVisionClass> textVisions = textVisionClass.GetValueByBatchID(dicTextVision, maxBatchID);
                List<textVisionClass> result = new List<textVisionClass>();
                for (int i = 0; i < textVisions.Count; i++)
                {
                    if (!textVisions[i].Log.StringIsEmpty())
                    {
                        returnData returnData_analyze = textVisionClass.analyze(API_Server, textVisions[i].GUID);
                        if (returnData_analyze.Code == 200)
                        {
                            List<textVisionClass> textVision = returnData_analyze.Data.ObjToClass<List<textVisionClass>>();
                            result.Add(textVision[0]);
                        }
                    }
                    else
                    {
                        result.Add(textVisions[i]);
                    }
                }
                returnData.Code = 200;
                returnData.Data = result;
                returnData.Result = $"操作者ID : {操作者ID}  批次ID: {maxBatchID}共{textVisions.Count}筆，辨識完成";
                returnData.TimeTaken = $"{myTimerBasic}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
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
        /// 資料儲存。
        /// </summary>
        /// <remarks>
        /// ### Request 範例 (JSON)
        /// <code>
        /// {
        ///     "Data": {
        ///         "GUID": "bb0af4c6-b847-4863-bc0e-0ccbb9a9a323",
        ///         "Master_GUID": "e590e1a6-c053-4ecb-b6f8-d380c79ab191",
        ///         "PON": "1140515001-13",
        ///         "IC_SN": "20250923-0",
        ///         "CODE": "OARC2",
        ///         "NAME": "Etor 60mg",
        ///         "BRD": "",
        ///         "SKDIACODE": "ETOO10",
        ///         "BARCODE1": "",
        ///         "BARCODE2": "",
        ///         "START_QTY": "1680",
        ///         "ADD_TIME": "2025/09/23 14:27:10",
        ///         "SEQ": "",
        ///         "FREE_CHARGE_FLAG": "",
        ///         "API_RETURN_NOTE": "",
        ///         "NOTE": "",
        ///         "CHT_NAME": "伊托膜衣錠",
        ///         "PAKAGE": "TAB",
        ///         "END_QTY": "1680",
        ///         "textVision": {
        ///             "op_id": "操作人ID",
        ///             "op_name": "操作人姓名",
        ///             "base64": "影像資料(Base64編碼)"
        ///         }
        ///     }
        /// }
        /// </code>
        ///
        /// ### Response
        /// - 回傳 <see cref="returnData"/> 結構，包含：  
        ///   - `Code`: 狀態碼 (成功=200，失敗=-200)  
        ///   - `Result`: 執行結果訊息  
        ///   - `TimeTaken`: 執行耗時  
        ///   - `Data`: 執行後的相關資料 (若有)  
        /// 
        /// ### 錯誤情境
        /// - 當 `Data` 為空或格式錯誤，回傳錯誤訊息與代碼  
        /// - 當 `textVision.base64` 無法解析，回傳辨識失敗訊息  
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，需包含 Data 物件：  
        /// - 基本藥品驗收欄位 (GUID, Master_GUID, CODE, NAME...等)  
        /// - `textVision`: 影像辨識相關欄位 (操作人ID/姓名與 Base64 圖檔)  
        /// </param>
        /// <returns>
        /// JSON 字串格式，序列化自 <see cref="returnData"/> 物件
        /// </returns>
        [HttpPost("preSave_single")]
        public async Task<string> preSave_single([FromBody] returnData returnData)
        {
            returnData.Method = "api/PCMPO/preSave_single";
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                inspectionClass.content content = returnData.Data.ObjToClass<inspectionClass.content>();
                if (content == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textvision", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_textVision.GetRowsByDefultAsync(null, (int)enum_textVision.Master_GUID, content.GUID);
                if(objects.Count != 0)
                {
                    textVisionClass textVision = objects[0].SQLToClass<textVisionClass, enum_textVision>();
                    returnData.Data = textVision;
                    returnData.Code = -200;
                    returnData.Result = $"此單號單子已存在";
                    return returnData.JsonSerializationt(true);
                }
                textVisionClass textVisionClass = new textVisionClass();
                textVisionClass.GUID = Guid.NewGuid().ToString();
                textVisionClass.Master_GUID = content.GUID;
                textVisionClass.操作時間 = DateTime.Now.ToDateTimeString();
                textVisionClass.確認 = "未確認";
                textVisionClass.效期 = DateTime.MinValue.ToDateTimeString();
                textVisionClass.Code = "-200";
                textVisionClass.Result = "未辨識";
                textVisionClass.藥品碼 = content.藥品碼;
                textVisionClass.中文名 = content.中文名稱;
                textVisionClass.藥名 = content.藥品名稱;
                textVisionClass.數量 = content.應收數量;
                textVisionClass.驗收單號 = content.驗收單號;
                textVisionClass.圖片 = content.textVision.圖片;
                textVisionClass.操作者ID = content.textVision.操作者ID;
                textVisionClass.操作者姓名 = content.textVision.操作者姓名;

                if (content.textVision.圖片.StringIsEmpty())
                {
                    string filPath = @"C:\Users\Administrator\OneDrive - 鴻森智能科技有限公司\軟體部\0.醫院客戶資料\3.衛服部立台北醫院\請購單辨識\測試單據\登銓\20250424-0 1140422003-37.jpg";
                    byte[] imageBytes = System.IO.File.ReadAllBytes(filPath);
                    string base64 = Convert.ToBase64String(imageBytes);
                    base64 = $"data:image/jpeg;base64,{base64}";
                    textVisionClass.圖片 = base64;
                }               

                object[] obj_textVision = textVisionClass.ClassToSQL<textVisionClass, enum_textVision>();
                await sQLControl_textVision.AddRowAsync(null, obj_textVision);

                returnData.Code = 200;
                returnData.Data = textVisionClass;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"請購單資料預儲存成功";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }


        }
        /// <summary>
        /// 執行文字辨識功能，接收單筆驗收/藥品資料及影像資訊進行處理。
        /// </summary>
        /// <remarks>
        /// ### Request 範例 (JSON)
        /// <code>
        /// {
        ///     "Data": {
        ///         "GUID": "bb0af4c6-b847-4863-bc0e-0ccbb9a9a323",
        ///         "Master_GUID": "e590e1a6-c053-4ecb-b6f8-d380c79ab191",
        ///         "PON": "1140515001-13",
        ///         "IC_SN": "20250923-0",
        ///         "CODE": "OARC2",
        ///         "NAME": "Etor 60mg",
        ///         "BRD": "",
        ///         "SKDIACODE": "ETOO10",
        ///         "BARCODE1": "",
        ///         "BARCODE2": "",
        ///         "START_QTY": "1680",
        ///         "ADD_TIME": "2025/09/23 14:27:10",
        ///         "SEQ": "",
        ///         "FREE_CHARGE_FLAG": "",
        ///         "API_RETURN_NOTE": "",
        ///         "NOTE": "",
        ///         "CHT_NAME": "伊托膜衣錠",
        ///         "PAKAGE": "TAB",
        ///         "END_QTY": "1680",
        ///         "textVision": {
        ///             "op_id": "操作人ID",
        ///             "op_name": "操作人姓名",
        ///             "base64": "影像資料(Base64編碼)"
        ///         }
        ///     }
        /// }
        /// </code>
        ///
        /// ### Response
        /// - 回傳 <see cref="returnData"/> 結構，包含：  
        ///   - `Code`: 狀態碼 (成功=200，失敗=-200)  
        ///   - `Result`: 執行結果訊息  
        ///   - `TimeTaken`: 執行耗時  
        ///   - `Data`: 執行後的相關資料 (若有)  
        /// 
        /// ### 錯誤情境
        /// - 當 `Data` 為空或格式錯誤，回傳錯誤訊息與代碼  
        /// - 當 `textVision.base64` 無法解析，回傳辨識失敗訊息  
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，需包含 Data 物件：  
        /// - 基本藥品驗收欄位 (GUID, Master_GUID, CODE, NAME...等)  
        /// - `textVision`: 影像辨識相關欄位 (操作人ID/姓名與 Base64 圖檔)  
        /// </param>
        /// <returns>
        /// JSON 字串格式，序列化自 <see cref="returnData"/> 物件
        /// </returns>
        [HttpPost("analyze_single")]
        public async Task<string> analyze_single([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                string result = await preSave_single(returnData);
                returnData returnData_preSave = result.JsonDeserializet<returnData>();
                if (returnData_preSave == null)
                {
                    returnData.Method = "pcmpo/analyze_single";
                    returnData.Code = -200;
                    returnData.Result = $"圖片儲存失敗";
                    return returnData.JsonSerializationt();
                }
                textVisionClass textVisionClass = returnData_preSave.Data.ObjToClass<textVisionClass>();
                if(textVisionClass == null)
                {
                    returnData.Method = "pcmpo/analyze_single";
                    returnData.Code = -200;
                    returnData.Result = $"圖片取得失敗";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                string API_AI = GetServerAPI("Main", "網頁", "po_vision_api");
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);

                returnData return_textVisionClass = ai_analyze(API_AI, textVisionClass);
                List<object[]> update_textVisionClass = new List<object[]>();
                if (return_textVisionClass == null)
                {
                    returnData.Result = $"AI連線失敗 url:{API_AI}";
                    returnData.Code = -200;
                    textVisionClass.Code = "-3";
                    textVisionClass.Result = returnData.Result;

                    object[] update = textVisionClass.ClassToSQL<textVisionClass, enum_textVision>();
                    await sQLControl_textVision.UpdateRowAsync(null, update);
                    returnData.Data = clearLongData(textVisionClass);
                    return returnData.JsonSerializationt(true);
                }
                if(return_textVisionClass.Data.ObjToClass<List<textVisionClass>>() == null || return_textVisionClass.Data.ObjToClass<List<textVisionClass>>().Count == 0)
                {
                    returnData.Result = $"AI連線失敗 url:{API_AI}";
                    returnData.Code = -200;
                    textVisionClass.Code = "-3";
                    textVisionClass.Result = returnData.Result;

                    object[] update = textVisionClass.ClassToSQL<textVisionClass, enum_textVision>();
                    await sQLControl_textVision.UpdateRowAsync(null, update);
                    returnData.Data = clearLongData(textVisionClass);
                    return returnData.JsonSerializationt(true);
                }
                textVisionClass textVision = return_textVisionClass.Data.ObjToClass<List<textVisionClass>>()[0];
                textVision.GUID = textVisionClass.GUID;
                textVision.批次ID = textVisionClass.批次ID;
                textVision.Master_GUID = textVisionClass.Master_GUID;
                textVision.操作時間 = textVisionClass.操作時間;
                textVision.確認 = textVisionClass.確認;
                textVision.驗收單號 = textVisionClass.驗收單號;
                textVision.Code = textVisionClass.Code;
                textVision.Result = textVisionClass.Result;
                textVision.操作者ID = textVisionClass.操作者ID;
                textVision.操作者姓名 = textVisionClass.操作者姓名;
                textVision.圖片 = textVisionClass.圖片;
                textVision.藥品碼  = textVisionClass.藥品碼;
                textVision.中文名 = textVisionClass.中文名;
                textVision.藥名 = textVisionClass.藥名;
                textVision.數量 = textVisionClass.數量;
                textVision.驗收單號 = textVisionClass.驗收單號;
                textVision.PRI_KEY = $"{textVision.驗收單號}_{textVision.單號}";
                //textVision = EditExpirydate(textVision);
                if (return_textVisionClass.Result == "False" || textVision.單號.StringIsEmpty()) // 批號或效期壞辨識失敗
                {
                    string base64 = textVisionClass.圖片;
                    string fileName = "";
                    fileName = $"{DateTime.Now.ToString("yyyyMMdd")}{DateTime.Now.ToString("HHmmss")}.txt";
                    SavePic(base64, project);

                    textVision.Code = "-1";
                    textVision.Result = $"辨識單號失敗";
                    if(textVision.效期.Check_Date_String() == false)
                    {
                        textVision.效期 = DateTime.MinValue.ToDateTimeString();
                    }
                    else
                    {
                        textVision = EditExpirydate(textVision);
                    }
                    object[] update = textVision.ClassToSQL<textVisionClass, enum_textVision>();
                    await sQLControl_textVision.UpdateRowAsync(null, update);

                    returnData.Data = clearLongData(textVisionClass);
                    returnData.Result = textVision.Result;
                    returnData.Code = -200;
                    Logger.Log(fileName, project, returnData.JsonSerializationt());
                    return returnData.JsonSerializationt(true);

                }
                textVision = EditExpirydate(textVision);

                //換API
                if (returnData.Value == "Y")
                {
                    returnData.Code = 200;
                    returnData.TimeTaken = $"{myTimerBasic}";
                    returnData.Result = $"AI 辨識完成";
                    returnData.Data = textVision;
                    returnData.Method = "api/pcmpo/analyze_single";
                    return returnData.JsonSerializationt(true);
                }
                else
                {
                    returnData.Data = textVision;
                    string poNum_str = await analyze_by_po_num_single(returnData);
                    returnData returnData_poNum = poNum_str.JsonDeserializet<returnData>();
                    return returnData_poNum.JsonSerializationt(true);
                }

            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 執行文字辨識
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///        
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("analyze_by_po_num_single")]
        public async Task<string> analyze_by_po_num_single([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "api/pcmpo/analyze_by_po_num_single";
            try
            {
                textVisionClass textVision = returnData.Data.ObjToClass<textVisionClass>();
                if (textVision == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }

                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);

                List<object[]> update_textVisionClass = new List<object[]>();
                List<positionClass> positionClasses = new List<positionClass>();
               
                Dictionary<string, (string Position, string Confidence, string Label)> dic_textVision = toDicByPosition(textVision);
                foreach (string key in dic_textVision.Keys)
                {
                      positionClass position = GetPosition(dic_textVision[key].Position, dic_textVision[key].Confidence, dic_textVision[key].Label);
                       positionClasses.LockAdd(position);                  
                }

                textVision.識別位置 = positionClasses;

                returnData.Code = 200;
                returnData.Result = $"辨識成功";

                textVision.Code = "200";
                textVision.Result = returnData.Result;

                object[] update_result = textVision.ClassToSQL<textVisionClass, enum_textVision>();
                await sQLControl_textVision.UpdateRowAsync(null, update_result);

                //update_textVisionClass = new List<textVisionClass>() { textVision }.ClassToSQL<textVisionClass, enum_textVision>();
                //sQLControl_textVision.UpdateByDefulteExtra(null, update_textVisionClass);

                returnData.Data = clearLongDataExtra(textVision);
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("analyze_batch")]
        public async Task<string> analyze_batch([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Method = "pcmpo/analyze_batch";
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 須為 [\"GUID\"]";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_textVision.GetRowsByDefultAsync(null, (int)enum_textVision.GUID, returnData.ValueAry[0]);
                if(objects.Count == 0)
                {
                    returnData.Method = "pcmpo/analyze_batch";
                    returnData.Code = -200;
                    returnData.Result = $"查無此資料 GUID({returnData.ValueAry[0]})";
                    return returnData.JsonSerializationt();
                }

                textVisionClass textVisionClass = objects.SQLToClass<textVisionClass, enum_textVision>().FirstOrDefault();
                if (textVisionClass == null)
                {
                    returnData.Method = "pcmpo/analyze_batch";
                    returnData.Code = -200;
                    returnData.Result = $"圖片取得失敗";
                    return returnData.JsonSerializationt();
                }
                string API_AI = GetServerAPI("Main", "網頁", "po_vision_api");
                returnData return_textVisionClass = ai_analyze(API_AI, textVisionClass);
                List<object[]> update_textVisionClass = new List<object[]>();
                if (return_textVisionClass == null)
                {
                    returnData.Result = $"AI連線失敗 url:{API_AI}";
                    returnData.Code = -200;
                    textVisionClass.Code = "-3";
                    textVisionClass.Result = returnData.Result;

                    object[] update = textVisionClass.ClassToSQL<textVisionClass, enum_textVision>();
                    await sQLControl_textVision.UpdateRowAsync(null, update);
                    returnData.Data = clearLongData(textVisionClass);
                    return returnData.JsonSerializationt(true);
                }
                if (return_textVisionClass.Data.ObjToClass<List<textVisionClass>>() == null || return_textVisionClass.Data.ObjToClass<List<textVisionClass>>().Count == 0)
                {
                    returnData.Result = $"AI連線失敗 url:{API_AI}";
                    returnData.Code = -200;
                    textVisionClass.Code = "-3";
                    textVisionClass.Result = returnData.Result;

                    object[] update = textVisionClass.ClassToSQL<textVisionClass, enum_textVision>();
                    await sQLControl_textVision.UpdateRowAsync(null, update);
                    returnData.Data = clearLongData(textVisionClass);
                    return returnData.JsonSerializationt(true);
                }
                textVisionClass textVision = return_textVisionClass.Data.ObjToClass<List<textVisionClass>>()[0];
                textVision.GUID = textVisionClass.GUID;
                textVision.批次ID = textVisionClass.批次ID;
                textVision.操作時間 = DateTime.Now.ToDateTimeString();
                textVision.確認 = textVisionClass.確認;
                textVision.Code = textVisionClass.Code;
                textVision.Result = textVisionClass.Result;
                textVision.操作者ID = textVisionClass.操作者ID;
                textVision.操作者姓名 = textVisionClass.操作者姓名;
                textVision.圖片 = textVisionClass.圖片;
                if (return_textVisionClass.Result == "False" || textVision.單號.StringIsEmpty()) // 批號或效期壞辨識失敗
                {
                    string base64 = textVisionClass.圖片;
                    string fileName = "";
                    fileName = $"{DateTime.Now.ToString("yyyyMMdd")}{DateTime.Now.ToString("HHmmss")}.txt";
                    SavePic(base64, project);

                    textVision.Code = "-1";
                    textVision.Result = $"辨識單號失敗";
                    if (textVision.效期.Check_Date_String() == false)
                    {
                        textVision.效期 = DateTime.MinValue.ToDateTimeString();
                    }
                    else
                    {
                        textVision = EditExpirydate(textVision);
                    }
                    object[] update = textVision.ClassToSQL<textVisionClass, enum_textVision>();
                    await sQLControl_textVision.UpdateRowAsync(null, update);

                    returnData.Data = clearLongData(textVisionClass);
                    returnData.Result = textVision.Result;
                    returnData.Code = -200;
                    Logger.Log(fileName, project, returnData.JsonSerializationt());
                    return returnData.JsonSerializationt(true);

                }
                textVision = EditExpirydate(textVision);
                try 
                {
                    returnData returnData_content = await new inspectionController().content_get_by_IC_SN(textVision.單號);
                    if(returnData_content != null && returnData_content.Code == 200)
                    {
                        List<inspectionClass.content> contents = returnData_content.Data.ObjToClass<List<inspectionClass.content>>();
                        if (contents != null && contents.Count == 1)
                        {
                            if (contents[0].Sub_content.Count == 0)
                            {
                                textVision.Master_GUID = contents[0].GUID;
                                textVision.數量 = contents[0].應收數量;
                                textVision.藥品碼 = contents[0].藥品碼;
                                textVision.藥名 = contents[0].藥品名稱;
                                textVision.驗收單號 = contents[0].驗收單號;
                            }
                            else
                            {
                                textVision.確認 = "此單號已驗收";
                            }
                        }
                    }
                }
                finally
                {
                    returnData.Code = 200;
                    returnData.Result = $"辨識成功";

                    textVision.Code = "200";
                    textVision.Result = returnData.Result;
                    object[] update_result = textVision.ClassToSQL<textVisionClass, enum_textVision>();
                    await sQLControl_textVision.UpdateRowAsync(null, update_result);
                }
                

                List<positionClass> positionClasses = new List<positionClass>();
                Dictionary<string, (string Position, string Confidence, string Label)> dic_textVision = toDicByPosition(textVision);
                foreach (string key in dic_textVision.Keys)
                {
                    positionClass position = GetPosition(dic_textVision[key].Position, dic_textVision[key].Confidence, dic_textVision[key].Label);
                    positionClasses.LockAdd(position);
                }

                textVision.識別位置 = positionClasses;
                

                returnData.Data = clearLongDataExtra(textVision);
                returnData.TimeTaken = $"{myTimerBasic}";
                return returnData.JsonSerializationt(true);



            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新出貨單辨識內容（textVision）
        /// </summary>
        /// <remarks>
        /// 依據傳入的 <c>Data</c>（textVisionClass），以 <c>GUID</c> 查詢 <c>textVision</c> 資料表，
        /// 將非空字串欄位（數量、藥品碼、中文名、藥名、Master_GUID、效期、批號、單號）覆寫更新，
        /// 其餘維持原值。成功後回傳處理結果與（經 <c>clearLongDataExtra</c> 處理後的）資料。
        ///
        /// 資料來源：<c>serverInfoTask.Value</c> 取得 DB 連線設定；目標表：<c>textVision</c>。
        ///
        /// 可能回應：
        /// - <c>200</c>：更新成功，<c>Result</c> = 「更新出貨單辨識內容」
        /// - <c>-200</c>：傳入資料異常 / 查無此資料 / 例外錯誤
        ///
        /// 重要行為：
        /// - 只更新來自請求、且「非空字串」的欄位；避免將空值覆蓋進資料表。
        /// - 以 <c>GUID</c> 為唯一鍵查詢並更新單筆資料。
        /// </remarks>
        /// <param name="returnData">
        /// 請求包裝物件。<br/>
        /// 必填：<c>Data</c>（可序列化為 <c>textVisionClass</c>）。<br/>
        /// 其中 <c>Data.GUID</c> 為必要條件，用於定位資料表中的對應紀錄。其餘欄位若為非空字串即參與更新。
        /// </param>
        /// <returns>
        /// 字串化的 JSON；格式同系統慣用 <c>returnData.JsonSerializationt(...)</c> 的輸出。
        /// </returns>
        /// <example>
        /// 請求（Request JSON）:
        /// {
        ///   "Data": {
        ///     "GUID": "10d2bb44-7dbf-4e40-aeed-0a800d954e92",
        ///     "Master_GUID": "",
        ///     "IC_SN": "20250818-0",
        ///     "op_name": "Anna",
        ///     "op_id": "7777",
        ///     "op_time": "2025/08/18 11:17:52",
        ///     "batch_num": "3J138B",
        ///     "po_num": "1140114013-08",
        ///     "name": "",
        ///     "cht_name": "",
        ///     "qty": "",
        ///     "expirydate": "2026/09/30 00:00:00",
        ///     "code": ""
        ///   }
        /// }
        ///
        /// 成功回應（範例）:
        /// {
        ///   "Code": 200,
        ///   "Result": "更新出貨單辨識內容",
        ///   "Data": {
        ///     "GUID": "10d2bb44-7dbf-4e40-aeed-0a800d954e92",
        ///     "Master_GUID": "",
        ///     "IC_SN": "20250818-0",
        ///     "op_name": "Anna",
        ///     "op_id": "7777",
        ///     "op_time": "2025/08/18 11:17:52",
        ///     "batch_num": "3J138B",
        ///     "po_num": "1140114013-08",
        ///     "name": "",
        ///     "cht_name": "",
        ///     "qty": "",
        ///     "expirydate": "2026/09/30 00:00:00",
        ///     "code": ""
        ///   }
        /// }
        ///
        /// 失敗回應（資料異常）:
        /// { "Code": -200, "Result": "傳入Data資料異常" }
        ///
        /// 失敗回應（查無資料）:
        /// { "Code": -200, "Result": "查無此資料 GUID(10d2bb44-7dbf-4e40-aeed-0a800d954e92)" }
        /// </example>

        [HttpPost("update")]
        public async Task<string> update([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Method = "pcmpo/update_content_analyze";
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
                textVisionClass textVisionClass = returnData.Data.ObjToClass<textVisionClass>();
                if (textVisionClass == null)
                {
                    returnData.Method = "pcmpo/update_content_analyze";
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl_textVision = new SQLControl(Server, DB, "textVision", UserName, Password, Port, SSLMode);
                string textVision_GUID = textVisionClass.GUID;               
                List<object[]> objects = await sQLControl_textVision.GetRowsByDefultAsync(null, (int)enum_textVision.GUID, textVision_GUID);
                if (objects.Count == 0)
                {
                    returnData.Method = "pcmpo/analyze_batch";
                    returnData.Code = -200;
                    returnData.Result = $"查無此資料 GUID({returnData.ValueAry[0]})";
                    return returnData.JsonSerializationt();
                }

                textVisionClass textVisionClass_sql = objects.SQLToClass<textVisionClass, enum_textVision>().FirstOrDefault();
                if (textVisionClass.數量.StringIsEmpty() == false) textVisionClass_sql.數量 = textVisionClass.數量;
                if (textVisionClass.藥品碼.StringIsEmpty() == false) textVisionClass_sql.藥品碼 = textVisionClass.藥品碼;
                if (textVisionClass.中文名.StringIsEmpty() == false) textVisionClass_sql.中文名 = textVisionClass.中文名;
                if (textVisionClass.藥名.StringIsEmpty() == false) textVisionClass_sql.藥名 = textVisionClass.藥名;
                if (textVisionClass.Master_GUID.StringIsEmpty() == false) textVisionClass_sql.Master_GUID = textVisionClass.Master_GUID;
                if (textVisionClass.效期.StringIsEmpty() == false) textVisionClass_sql.效期 = textVisionClass.效期;
                if (textVisionClass.批號.StringIsEmpty() == false) textVisionClass_sql.批號 = textVisionClass.批號;
                if (textVisionClass.單號.StringIsEmpty() == false) textVisionClass_sql.單號 = textVisionClass.單號;


               
                object[] update = textVisionClass_sql.ClassToSQL<textVisionClass, enum_textVision>();
                await sQLControl_textVision.UpdateRowAsync(null, update);

                returnData.Data = clearLongDataExtra(textVisionClass);
                returnData.Result = $"更新出貨單辨識內容";
                returnData.Code = 200;
                return returnData.JsonSerializationt(true);
                                       
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                Logger.Log(project, returnData.JsonSerializationt());
                Logger.Log(project, Message);
                return returnData.JsonSerializationt(true);
            }
        }

        private (string width, string height, string center) GetSquare(string[] position)
        {
            int xMax = position[2].Split(",")[0].StringToInt32();
            int yMax = position[2].Split(",")[1].StringToInt32();

            int xMin = position[0].Split(",")[0].StringToInt32();
            int yMin = position[0].Split(",")[1].StringToInt32();

            string width = (xMax - xMin).ToString();
            string height = (yMax - yMin).ToString();

            double centerX = (xMax + xMin) / 2.0;
            double centerY = (yMax + yMin) / 2.0;

            return (width, height, $"{centerX},{centerY}");
        }
        private (string Server, string DB, string UserName, string Password, uint Port) GetServerInfo(string Name, string Type, string Content)
        {
            List<sys_serverSettingClass> serverSetting = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClass sys_serverSettingClass = serverSetting.MyFind(Name, Type, Content).FirstOrDefault();
            if (sys_serverSettingClass == null)
            {
                throw new Exception("找無Server資料");
            }
            return (sys_serverSettingClass.Server, sys_serverSettingClass.DBName, sys_serverSettingClass.User, sys_serverSettingClass.Password, (uint)sys_serverSettingClass.Port.StringToInt32());
        }
        private string GetServerAPI(string Name, string Type, string Content)
        {
            List<sys_serverSettingClass> serverSetting = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClass sys_serverSettingClass = serverSetting.MyFind(Name, Type, Content).FirstOrDefault();
            if (sys_serverSettingClass == null)
            {
                throw new Exception("找無Server資料");
            }
            return sys_serverSettingClass.Server;
        }
        private positionClass GetPosition(string 位置, string 信心分數, string keyword)
        {
            string[] position = 位置.Split(";");
            (string width, string height, string center) = GetSquare(position);
            positionClass positionClass = new positionClass
            {
                高 = height,
                寬 = width,
                中心 = center,
                信心分數 = 信心分數,
                keyWord = keyword,
            };
            return positionClass;
        }
        private List<textVisionClass> clearLongData(textVisionClass textVisionClass)
        {
            textVisionClass.圖片 = "";
            textVisionClass.Log = "";
            return new List<textVisionClass>() { textVisionClass };
        }
        private List<textVisionClass> clearLongData(List<textVisionClass> textVisionClasses)
        {
            foreach(var textVisionClass in textVisionClasses)
            {
                textVisionClass.圖片 = "";
                textVisionClass.Log = "";
            }
            
            return textVisionClasses;
        }
        private List<textVisionClass> clearLongDataExtra(textVisionClass textVisionClass)
        {
            textVisionClass.Log = "";
            return new List<textVisionClass>() { textVisionClass };
        }
        private List<textVisionClass> clearLongDataExtra(List<textVisionClass> textVisionClasses)
        {
            foreach (var textVisionClass in textVisionClasses)
            {
                textVisionClass.Log = "";
            }

            return textVisionClasses;
        }
        private Dictionary<string, (string Position, string Confidence, string Label)> toDicByPosition(textVisionClass textVisionClass)
        {
            Dictionary<string, (string Position, string Confidence, string Label)> dic = new Dictionary<string, (string Position, string Confidence, string Label)>();
            List<(string Position, string Confidence, string Label)> fields = new List<(string Position, string Confidence, string Label)>
            {
                (textVisionClass.批號位置, textVisionClass.批號信心分數, "batch_num"),
                (textVisionClass.中文名位置, textVisionClass.中文名信心分數, "cht_name"),
                (textVisionClass.效期位置, textVisionClass.效期信心分數, "expirydate"),
                (textVisionClass.藥名位置, textVisionClass.藥名信心分數, "name"),
                (textVisionClass.單號位置, textVisionClass.單號信心分數, "po"),
                (textVisionClass.數量位置, textVisionClass.數量信心分數, "qty")

            };

            foreach (var field in fields)
            {
                if (field.Position.StringIsEmpty() == false) dic[field.Position] = field;
            }
            return dic;
        }
        private void SavePic(string base64, string folderName)
        {
            string fileName = $"{DateTime.Now.ToString("yyyyMMdd")}{DateTime.Now.ToString("HHmmss")}";
            SavePic(fileName, base64, folderName);
        }
        private void SavePic(string fileName, string base64, string folderName)
        {
            // 將檔名副檔名改為 .jpg
            fileName = Path.ChangeExtension(fileName, ".jpg");

            // 移除 Base64 字串前綴
            string pre = "data:image/jpeg;base64,";
            base64 = base64.Replace(pre, "");

            // 取得目前執行檔所在目錄，並組合出 log/目標資料夾路徑
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileDirectory = Path.Combine(currentDirectory, "log");
            string targetFolder = string.IsNullOrEmpty(folderName) ? "PO_vision" : folderName;
            string folderPath = Path.Combine(fileDirectory, targetFolder);

            // 確認目錄是否存在，不存在則建立
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filepath = Path.Combine(folderPath, fileName);

            try
            {
                // 轉換 Base64 字串為位元組陣列
                byte[] imageBytes = Convert.FromBase64String(base64);

                // 使用 MemoryStream 與 ImageSharp 載入圖片
                using (var ms = new MemoryStream(imageBytes))
                {
                    using (var image = SixLabors.ImageSharp.Image.Load(ms))
                    {
                        // 若在 Docker 容器中運行，可調整檔案路徑格式
                        if (ContainerChecker.IsRunningInDocker())
                        {
                            filepath = filepath.Replace("\\", "/");
                            string _fileName = Path.GetFileName(filepath);
                            string directoryPath = Path.GetDirectoryName(filepath);
                            string _folderName = Path.GetFileName(directoryPath);
                            filepath = $"/app/log/{_folderName}/{_fileName}";
                            Console.WriteLine($"SavePic ..filepath : {filepath}");
                        }
                        // 開啟檔案流，並使用 JpegEncoder 儲存圖片
                        var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 100 };
                        using (FileStream fileStream = System.IO.File.OpenWrite(filepath))
                        {
                            image.Save(fileStream, encoder);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("PCMPO", $"儲存圖片發生錯誤");

                throw;
            }
        }
        private textVisionClass EditExpirydate(textVisionClass textVisionClass)
        {
            if (Regex.IsMatch(textVisionClass.效期, @"^\d{3}/\d{2}/\d{2}$")) // 範例：025/08/16
            {
                textVisionClass.效期 = "2" + textVisionClass.效期; // 轉為 2025/08/16
            }
            string[] formats = { "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "yyyy.MM.dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd","yyyyMMdd" }; // 可擴展格式

            if (DateTime.TryParseExact(textVisionClass.效期, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                textVisionClass.效期 = date.ToString("yyyy/MM/dd");
            }
            else
            {
                textVisionClass.效期 = DateTime.MinValue.ToDateTimeString();
            }
            return textVisionClass;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_by_MasterGUID(string Master_GUID)
        {
            returnData returnData = new returnData();
            returnData.ValueAry = new List<string> { Master_GUID };
            string result = get_by_MasterGUID(returnData);
            returnData = result.JsonDeserializet<returnData>();
            return returnData;
        }
        private returnData ai_analyze(string API, textVisionClass textVisionClass)
        {
            List<textVisionClass> textVisionClasses = new List<textVisionClass> { textVisionClass };
            return ai_analyze(API, textVisionClasses);
        }
        private returnData ai_analyze(string API, List<textVisionClass> textVisionClasses)
        {
            returnData returnData = new returnData();
            returnData.Data = textVisionClasses;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(API, json_in);
            returnData returnData_AI = json_out.JsonDeserializet<returnData>();
            if (returnData_AI == null) return null;
            return returnData_AI; 
        }
        private returnData get_by_pri_key(string pri_key)
        {
            returnData returnData = new returnData();   
            returnData.ValueAry.Add(pri_key);
            string result = get_by_pri_key(returnData);
            returnData = result.JsonDeserializet<returnData>();
            return returnData;
        }
    }
}
