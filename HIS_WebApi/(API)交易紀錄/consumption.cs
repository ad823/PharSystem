using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Configuration;
using MyOffice;
using NPOI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using MyUI;
using H_Pannel_lib;
using HIS_DB_Lib;
using System.Security;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class consumption : Controller
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
           serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
           {
               var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

               if (string.IsNullOrWhiteSpace(Password))
                   throw new SecurityException("Database password cannot be null or empty (medUnit).");

               return (Server, DB, UserName, Password, Port);
           });
        private static readonly Lazy<Task<sys_serverSettingClass>>
           GetServerAsync = new Lazy<Task<sys_serverSettingClass>>(async () =>
           {
               sys_serverSettingClass sys_ServerSetting = await Method.GetServerAsync("Main", "網頁", "VM端");

               if (sys_ServerSetting == null)
                   throw new SecurityException("Database password cannot be null or empty (medUnit).");

               return sys_ServerSetting;
           });
        /// <summary>
        /// 取得庫存及消耗總量
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "口服2",
        ///     "ServerType" : "調劑台",
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "Value" : "[起始日期],[結束日期]"
        ///     "ValyeAry" : ["藥品群組名稱"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[consumption]陣列結構</returns>
        [Route("serch_by_ST_END")]
        [HttpPost]
        public async Task<string> POST_serch_consumption_by_ST_END([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "serch_by_ST_END";
            var timer = new TimeLogHelper();

            try
            {
                // 取得伺服器設定
                var serverSettings = ServerSettingController.GetAllServerSetting();
                var vmSetting = serverSettings.myFind("Main", "網頁", "VM端");
                var tradingSetting = serverSettings.myFind(returnData.ServerName, returnData.ServerType, "交易紀錄資料");

                if (tradingSetting == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                // 參數解析與驗證
                string[] dateAry = returnData.Value.Split(',');
                if (dateAry.Length != 2 || !dateAry[0].Check_Date_String() || !dateAry[1].Check_Date_String())
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }

                string startDate = dateAry[0];
                string endDate = dateAry[1];
                string groupName = returnData.ValueAry?.FirstOrDefault() ?? "";

                var sqlControl = new SQLControl(
                    tradingSetting.Server,
                    tradingSetting.DBName,
                    "trading",
                    tradingSetting.User,
                    tradingSetting.Password,
                    (uint)tradingSetting.Port.StringToInt32(),
                    SSLMode);

                string sql = @$"
            SELECT 
                t2.藥品碼,
                t2.藥品名稱,
                IFNULL(t1.交易量, 0) AS 交易量,
                t2.結存量
            FROM (
                SELECT 藥品碼, 
                       SUM(
                           CASE 
                               WHEN 交易量 REGEXP '^-?[0-9]+(\\.[0-9]+)?$' THEN CAST(交易量 AS DECIMAL(20,6))
                               ELSE 0
                           END
                       ) AS 交易量
                FROM trading
                WHERE 操作時間 BETWEEN '{startDate}' AND '{endDate}'
                  AND 藥品碼 IS NOT NULL
                  AND 藥品碼 <> ''
                GROUP BY 藥品碼
            ) t1
            RIGHT JOIN (
                SELECT a.*
                FROM trading a
                LEFT JOIN trading b
                  ON a.藥品碼 = b.藥品碼
                 AND a.操作時間 < b.操作時間
                 AND b.操作時間 <= '{endDate}'
                WHERE a.操作時間 <= '{endDate}'
                  AND b.藥品碼 IS NULL
                  AND a.藥品碼 IS NOT NULL
                  AND a.藥品碼 <> ''
            ) t2 ON t1.藥品碼 = t2.藥品碼
        ";

                // ⏱ 執行 SQL 與 取得群組 → 平行 Task
                Task<DataTable> taskDataTable = Task.Run(() => sqlControl.WtrteCommandAndExecuteReader(sql));

                Task<medGroupClass> taskGroup = Task.Run(() =>
                    !groupName.StringIsEmpty()
                        ? medGroup.Function_Get_medGroupClass_ByName(vmSetting, groupName)
                        : (medGroupClass)null
                );

                await Task.WhenAll(taskDataTable, taskGroup);
                timer.Tick("查詢完成");

                // 轉換 DataTable 資料為物件
                var table = taskDataTable.Result;
                var _medGroup = taskGroup.Result;

                var consumptionList = table.Rows.Cast<DataRow>()
                    .Select(row => new consumptionClass
                    {
                        藥碼 = row["藥品碼"].ToString(),
                        藥名 = row["藥品名稱"].ToString(),
                        庫存量 = row["結存量"].ToString(),
                        消耗量 = (row["交易量"].ToString().StringToDouble() * -1).ToString("0.00"),
                        實調量 = (row["交易量"].ToString().StringToDouble() * -1).ToString("0.00")
                    }).ToList();

                timer.Tick("資料轉換");

                // 如果有群組名稱，則做比對排序
                if (_medGroup?.MedClasses != null)
                {
                    consumptionList = consumptionList
                        .SortAndFilterByMedClass(_medGroup.MedClasses, x => x.藥碼);
                    timer.Tick("群組排序過濾");
                }

                returnData.Code = 200;
                returnData.Result = $"成功取得庫存結餘表，共<{consumptionList.Count}>筆。 各階段耗時：{timer.GetResult()}（總耗時：{timer.Total}）";
                returnData.Data = consumptionList;
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"❌ 錯誤：{ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 取得庫存及消耗總量報表結構(SheetClass)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "口服2",
        ///     "ServerType" : "調劑台",
        ///     "Data": {},
        ///     "Value" : "[起始日期],[結束日期]"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[SheetClass]陣列結構</returns>
        [Route("get_sheet_by_serch")]
        [HttpPost]
        public async Task<string> POST_get_sheet_by_serch([FromBody] returnData returnData)
        {
            try
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "交易紀錄資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                // ✅ 非同步呼叫 POST_serch_consumption_by_ST_END
                string json = await Task.Run(() => POST_serch_consumption_by_ST_END(returnData));
                returnData = json.JsonDeserializet<returnData>();

                if (returnData.Code != 200) return null;

                List<consumptionClass> consumptionClasses = returnData.Data.ObjToListClass<consumptionClass>();
                string[] date_ary = returnData.Value.Split(',');

                // ✅ 非同步處理資料轉報表（主要耗時）
                List<SheetClass> sheetClasses = await Task.Run(() =>
                {
                    string loadText = Basic.MyFileStream.LoadFileAllText(@"./excel_consumption.txt", "utf-8");
                    int row_max = 5000;
                    int NumOfRow = -1;
                    double total_消耗量 = 0;
                    List<SheetClass> resultSheets = new List<SheetClass>();
                    SheetClass sheetClass = loadText.JsonDeserializet<SheetClass>();

                    for (int i = 0; i < consumptionClasses.Count; i++)
                    {
                        if (NumOfRow >= row_max || NumOfRow == -1)
                        {
                            sheetClass = loadText.JsonDeserializet<SheetClass>();
                            sheetClass.Name = $"{i}";
                            sheetClass.ReplaceCell(1, 2, $"{date_ary[0]}");
                            sheetClass.ReplaceCell(2, 2, $"{date_ary[1]}");
                            resultSheets.Add(sheetClass);
                            NumOfRow = 0;
                        }

                        var item = consumptionClasses[i];
                        total_消耗量 += item.消耗量.StringToDouble() * -1;

                        int rowIndex = NumOfRow + 4;
                        sheetClass.AddNewCell_Webapi(rowIndex, 0, $"{i + 1}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430,
                            NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(rowIndex, 1, item.藥碼, "微軟正黑體", 14, false, NPOI_Color.BLACK, 430,
                            NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(rowIndex, 2, item.藥名, "微軟正黑體", 14, false, NPOI_Color.BLACK, 430,
                            NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(rowIndex, 3, item.消耗量, "微軟正黑體", 14, false, NPOI_Color.BLACK, 430,
                            NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(rowIndex, 4, item.結存量, "微軟正黑體", 14, false, NPOI_Color.BLACK, 430,
                            NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);

                        NumOfRow++;
                    }

                    return resultSheets;
                });

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
        /// 取得庫存及消耗總量Excel(xlsx)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "口服2",
        ///     "ServerType" : "調劑台",
        ///     "Data": {},
        ///     "Value" : "[起始日期],[結束日期]"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>Excel 檔案下載</returns>
        [Route("download_excel_by_serch")]
        [HttpPost]
        public async Task<ActionResult> Post_download_excel_by_serch([FromBody] returnData returnData)
        {
            try
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);

                // ✅ 非同步取得報表 JSON 結構
                string sheetJson = await POST_get_sheet_by_serch(returnData);
                returnData = sheetJson.JsonDeserializet<returnData>();
                if (returnData.Code != 200) return BadRequest("報表產生失敗");

                // ✅ 解析 SheetClass 陣列
                List<SheetClass> sheetClasses = returnData.Data.ObjToListClass<SheetClass>();

                // ✅ 非同步產生 Excel 二進位資料
                byte[] excelData = await Task.Run(() => sheetClasses.NPOI_GetBytes(Excel_Type.xlsx));

                // ✅ 回傳 MemoryStream 作為下載檔案
                string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var stream = new MemoryStream(excelData);
                var fileName = $"{DateTime.Now.ToDateString("-")}_消耗量表.xlsx";
                return File(stream, mimeType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"錯誤：{ex.Message}");
            }
        }

        /// <summary>
        /// 取得庫存及消耗總量(多台合併)
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
        ///       "結束時間",
        ///       "口服1,口服2",
        ///       "調劑台,調劑台"
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[consumption]陣列結構</returns>
        [Route("serch_datas_by_ST_END")]
        [HttpPost]
        public string POST_serch_datas_by_ST_END([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "serch_datas_by_ST_END";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 4)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[起始時間][結束時間][ServerName1,ServerName2][ServerType1,ServerType2]";
                    return returnData.JsonSerializationt(true);
                }
                string 起始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];
                string[] ServerNames = returnData.ValueAry[2].Split(',');
                string[] ServerTypes = returnData.ValueAry[3].Split(',');

                if (!起始時間.Check_Date_String() || !結束時間.Check_Date_String())
                {
                    returnData.Code = -5;
                    returnData.Result = "輸入日期格式錯誤!";
                    return returnData.JsonSerializationt();
                }
                起始時間 = 起始時間.StringToDateTime().GetStartDate().ToDateTimeString();
                結束時間 = 結束時間.StringToDateTime().GetEndDate().ToDateTimeString();
                DateTime date_st = 起始時間.StringToDateTime();
                DateTime date_end = 結束時間.StringToDateTime();

                if (ServerNames.Length != ServerTypes.Length)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ServerNames及ServerTypes長度不同";
                    return returnData.JsonSerializationt(true);
                }

                List<Task> tasks = new List<Task>();
                List<List<consumptionClass>> list_consumptionClasses = new List<List<consumptionClass>>();
                for (int i = 0; i < ServerNames.Length; i++)
                {
                    string serverName = ServerNames[i];
                    string serverType = ServerTypes[i];
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        List<consumptionClass> consumptionClasses = new List<consumptionClass>();
                        List<sys_serverSettingClass> _sys_serverSettingClasses = sys_serverSettingClasses.MyFind(serverName, serverType, "交易紀錄資料");
                        if (_sys_serverSettingClasses.Count == 0) return;
                        string Server = _sys_serverSettingClasses[0].Server;
                        string DB = _sys_serverSettingClasses[0].DBName;
                        string UserName = _sys_serverSettingClasses[0].User;
                        string Password = _sys_serverSettingClasses[0].Password;
                        uint Port = (uint)_sys_serverSettingClasses[0].Port.StringToInt32();
                        string TableName = "trading";
                        SQLControl sQLControl_trading = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);

                        string sql = @$"
                            SELECT 
                                t2.藥品碼,
                                t2.藥品名稱,
                                IFNULL(t1.交易量, 0) AS 交易量,
                                t2.結存量
                            FROM (
                                -- 統計交易量（只加總為數字的交易量）
                                SELECT 藥品碼, 
                                       SUM(
                                           CASE 
                                               WHEN 交易量 REGEXP '^-?[0-9]+(\\.[0-9]+)?$' THEN CAST(交易量 AS DECIMAL(20,6))
                                               ELSE 0
                                           END
                                       ) AS 交易量
                                FROM trading
                                WHERE 操作時間 BETWEEN '{起始時間}' AND '{結束時間}'
                                  AND 藥品碼 IS NOT NULL
                                  AND 藥品碼 <> ''
                                GROUP BY 藥品碼
                            ) t1
                            RIGHT JOIN (
                                -- 取每個藥品碼在 {結束時間} 前的最新一筆完整資料
                                SELECT a.*
                                FROM trading a
                                LEFT JOIN trading b
                                  ON a.藥品碼 = b.藥品碼
                                 AND a.操作時間 < b.操作時間
                                 AND b.操作時間 <= '{結束時間}'
                                WHERE a.操作時間 <= '{結束時間}'
                                  AND b.藥品碼 IS NULL
                                  AND a.藥品碼 IS NOT NULL
                                  AND a.藥品碼 <> ''
                            ) t2 ON t1.藥品碼 = t2.藥品碼
                        ";

                        System.Data.DataTable dataTable = sQLControl_trading.WtrteCommandAndExecuteReader(sql);
                        foreach (System.Data.DataRow row in dataTable.Rows)
                        {
                            consumptionClass consumptionClass = new consumptionClass();
                            consumptionClass.藥碼 = row["藥品碼"].ToString();
                            consumptionClass.藥名 = row["藥品名稱"].ToString();
                            consumptionClass.庫存量 = row["結存量"].ToString();
                            consumptionClass.消耗量 = (row["交易量"].ToString().StringToDouble() * -1).ToString("0.00");
                            consumptionClass.實調量 = (row["交易量"].ToString().StringToDouble() * -1).ToString("0.00");
                            consumptionClasses.Add(consumptionClass);
                        }

                        list_consumptionClasses.Add(consumptionClasses);

                     
                    })));
                }
                Task.WhenAll(tasks).Wait();
                sys_serverSettingClass sys_ServerSetting_order = sys_serverSettingClasses.myFind("Main", "網頁", "VM端");
                List<OrderClass> orderClasses = OrderClass.get_by_rx_time_st_end("http://127.0.0.1:4433", date_st, date_end);
                Dictionary<string, List<OrderClass>> keyValuePairs_orders = orderClasses.CoverToDictionaryBy_Code();
                List<OrderClass> orderClasses_buf = new List<OrderClass>();


                List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
                List<medClass> medClasses_cloud_buf = new List<medClass>();
                System.Collections.Generic.Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClass.CoverToDictionaryByCode(medClasses_cloud);


                List<consumptionClass> consumptionClasses = new List<consumptionClass>();
                List<consumptionClass> consumptionClasse_buf = new List<consumptionClass>();
                for (int i = 0; i < list_consumptionClasses.Count; i++)
                {
                    foreach (consumptionClass value in list_consumptionClasses[i])
                    {

                        consumptionClasse_buf = (from temp in consumptionClasses
                                                 where temp.藥碼 == value.藥碼
                                                 select temp).ToList();
                        if (consumptionClasse_buf.Count > 0)
                        {
                            consumptionClasse_buf[0].消耗量 = (consumptionClasse_buf[0].消耗量.StringToDouble() + value.消耗量.StringToDouble()).ToString();
                            consumptionClasse_buf[0].實調量 = (consumptionClasse_buf[0].實調量.StringToDouble() + value.實調量.StringToDouble()).ToString();
                            consumptionClasse_buf[0].庫存量 = (consumptionClasse_buf[0].庫存量.StringToDouble() + value.庫存量.StringToDouble()).ToString();
                        }
                        else
                        {
                            medClasses_cloud_buf = medClass.SortDictionaryByCode(keyValuePairs_med_cloud, value.藥碼);
                            if (medClasses_cloud_buf.Count > 0) value.類別 = medClasses_cloud_buf[0].類別;
                            consumptionClasses.Add(value);
                        }
                    }
                }

                returnData.Code = 200;
                returnData.Result = $"取得總消耗量表成功,{returnData.ValueAry[2]}";
                returnData.Data = consumptionClasses;
                returnData.TimeTaken = $"{myTimer}";
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
        /// 取得庫存及消耗總量報表結構(SheetClass)(多台合併)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "口服2",
        ///     "ServerType" : "調劑台",
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "起始時間",
        ///       "結束時間",
        ///       "口服1,口服2",
        ///       "調劑台,調劑台"
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[SheetClass]陣列結構</returns>
        [Route("get_datas_sheet_by_serch")]
        [HttpPost]
        public string POST_get_datas_sheet_by_serch([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "get_datas_sheet_by_serch";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 4)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[起始時間][結束時間][ServerName1,ServerName2][ServerType1,ServerType2]";
                    return returnData.JsonSerializationt(true);
                }
                string 起始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];
                string[] ServerNames = returnData.ValueAry[2].Split(',');
                string[] ServerTypes = returnData.ValueAry[3].Split(',');


                string json = POST_serch_datas_by_ST_END(returnData);
                returnData = json.JsonDeserializet<returnData>();

                if (returnData.Code != 200)
                {
                    return null;
                }
                List<consumptionClass> consumptionClasses = returnData.Data.ObjToListClass<consumptionClass>();



                string loadText = Basic.MyFileStream.LoadFileAllText(@"./excel_consumption.txt", "utf-8");
                Console.WriteLine($"取得creats {myTimer.ToString()}");
                int row_max = 5000;
                List<SheetClass> sheetClasses = new List<SheetClass>();
                SheetClass sheetClass = loadText.JsonDeserializet<SheetClass>();

                string[] date_ary = returnData.Value.Split(',');
                double 消耗量 = 0;
                int NumOfRow = -1;
                for (int i = 0; i < consumptionClasses.Count; i++)
                {
                    if (NumOfRow >= row_max || NumOfRow == -1)
                    {
                        sheetClass = loadText.JsonDeserializet<SheetClass>();
                        sheetClass.Name = $"{i}";
                        sheetClass.ReplaceCell(1, 2, $"{起始時間}");
                        sheetClass.ReplaceCell(2, 2, $"{結束時間}");

                        sheetClasses.Add(sheetClass);
                        NumOfRow = 0;
                    }

                    消耗量 += consumptionClasses[i].消耗量.StringToDouble();
                    消耗量 *= -1;
                    sheetClass.AddNewCell_Webapi(NumOfRow + 4, 0, $"{i + 1}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(NumOfRow + 4, 1, $"{consumptionClasses[i].藥碼}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(NumOfRow + 4, 2, $"{consumptionClasses[i].藥名}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(NumOfRow + 4, 3, $"{consumptionClasses[i].消耗量}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(NumOfRow + 4, 4, $"{consumptionClasses[i].結存量}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);

                    NumOfRow++;
                }
                Console.WriteLine($"寫入Sheet {myTimer.ToString()}");
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
        [Route("download/{value}")]
        [HttpGet]
        public async Task<ActionResult> get_download_datas_excel_by_serch(string value)
        {
           
            try
            {
                string[] date_strs = value.Split(',');

                string date_str_st = date_strs[0];
                string date_str_end = date_strs[1];
                if (date_str_st.Length != 12 || date_str_end.Length != 12)
                {
                    return BadRequest("輸入日期格式錯誤");
                }
                date_str_st = $"{date_str_st.Substring(0, 4)}-{date_str_st.Substring(4, 2)}-{date_str_st.Substring(6, 2)} {date_str_st.Substring(8, 2)}:{date_str_st.Substring(10, 2)}";
                date_str_end = $"{date_str_end.Substring(0, 4)}-{date_str_end.Substring(4, 2)}-{date_str_end.Substring(6, 2)} {date_str_st.Substring(8, 2)}:{date_str_st.Substring(10, 2)}";
                if(date_str_st.Check_Date_String() == false || date_str_end.Check_Date_String() == false)
                {
                    return BadRequest("輸入日期格式錯誤");
                }
                DateTime dateTime_st = date_str_st.StringToDateTime();
                DateTime dateTime_end = date_str_end.StringToDateTime();


                List<sys_serverSettingClass> list_sys_serverSettingClasses = sys_serverSettingClass.get_serversetting_by_type("http://127.0.0.1:4433", "調劑台");

                List<string> list_dps = (from temp in list_sys_serverSettingClasses
                                         select temp.設備名稱).Distinct().ToList();

                string dps_name = "";
                string dps_type = "";
                for (int i = 0; i < list_dps.Count; i++)
                {
                    dps_name += list_dps[i];
                    dps_type += "調劑台";

                    if (i != list_dps.Count - 1) dps_name += ",";
                    if (i != list_dps.Count - 1) dps_type += ",";
                }
                returnData returnData = new returnData();

                returnData.ValueAry.Add(dateTime_st.ToDateTimeString());
                returnData.ValueAry.Add(dateTime_end.ToDateTimeString());
                returnData.ValueAry.Add(dps_name);
                returnData.ValueAry.Add(dps_type);
                return await Post_download_datas_excel_by_serch(returnData);
            }
            catch(Exception ex)
            {
                return BadRequest("錯誤訊息：" + ex.Message);
            }
        }

        /// <summary>
        /// 取得庫存及消耗總量Excel(xlxs)(多台合併)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "",
        ///     "ServerType" : "",
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "起始時間",
        ///       "結束時間",
        ///       "口服1,口服2",
        ///       "調劑台,調劑台"
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[SheetClass]陣列結構</returns>
        [Route("download_datas_excel_by_serch")]
        [HttpPost]
        public async Task<ActionResult> Post_download_datas_excel_by_serch([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "download_datas_excel_by_serch";

            try
            {

                returnData = POST_serch_datas_by_ST_END(returnData).JsonDeserializet<returnData>();
                if (returnData.Code != 200)
                {
                    return null;
                }
                List<consumptionClass> consumptionClasses = returnData.Data.ObjToClass<List<consumptionClass>>();
                List<object[]> list_value = consumptionClasses.ClassToSQL<consumptionClass , enum_consumption>();
                System.Data.DataTable dataTable = list_value.ToDataTable(new enum_consumption());

                string 起始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];

                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";
                dataTable.RemoveColumn(enum_consumption.結存量);
                byte[] excelData = dataTable.NPOI_GetBytes( Excel_Type.xlsx);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{起始時間}_{結束時間}_消耗量表.xlsx"));
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤訊息：" + ex.Message);
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
        ///        List(consumptionClass)
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
                List<consumptionClass> consumptionClasses = returnData.Data.ObjToClass<List<consumptionClass>>();
                if(consumptionClasses == null)
                {
                    return BadRequest("returnData.Data不能是空的");
                }
                List<object[]> list_transactionsClasses = consumptionClasses.ClassToSQL<consumptionClass, enum_consumption>();
                System.Data.DataTable dataTable = list_transactionsClasses.ToDataTable(new enum_consumption());
                dataTable.RemoveColumn(enum_consumption.結存量);

                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";
                List<System.Data.DataTable> dataTables = new List<System.Data.DataTable>();
                dataTables.Add(dataTable);
                byte[] excelData = MyOffice.ExcelClass.NPOI_GetBytes(dataTable, Excel_Type.xlsx, (int)enum_consumption.消耗量, (int)enum_consumption.實調量, (int)enum_consumption.庫存量);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_庫存清單.xlsx"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"系統錯誤：{ex.Message}");
            }

        }

        [HttpPost("init")]
        public async Task<string> init([FromBody] returnData returnData)
        {
            try
            {
                return await CheckCreatTable(returnData);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"{ex.Message}";
                return returnData.JsonSerializationt();
            }
        }
        [HttpPost("add")]
        public async Task<string> add([FromBody] returnData returnData)
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
                List<consumptionClass> consumptionClasses = returnData.Data.ObjToClass<List<consumptionClass>>();
                if (consumptionClasses == null)
                {
                    consumptionClass consumption = returnData.Data.ObjToClass<consumptionClass>();
                    if (consumption == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"資料格式錯誤";
                        return returnData.JsonSerializationt();
                    }
                    consumptionClasses = new List<consumptionClass> { consumption };
                }
                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                List<consumptionClass> add = new List<consumptionClass>();
                string time_now = DateTime.Now.ToDateTimeString();
                foreach (var item in consumptionClasses)
                {                  
                    item.GUID = Guid.NewGuid().ToString();
                    item.建立時間 = time_now;
                    add.Add(item);
                }
                SQLControl sQLControl = new SQLControl(Server, DB, "consumption", UserName, Password, Port, SSLMode);

                if (add.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無有效資料可寫入!";
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> add_ = add.ClassToSQL<consumptionClass>();

                await sQLControl.AddRowsAsync(null, add_);

                returnData.Code = 200;
                returnData.Data = add;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add";
                returnData.Result = $"建立成功，共{add.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_by_start_end")]
        public async Task<string> get_by_start_end([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry須包含[開始時間][結束時間]";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"ServerName和ServerType不得為空";
                    return returnData.JsonSerializationt();
                }
                //if (returnData.ValueAry[0].Check_Date_String() || returnData.ValueAry[1].Check_Date_String())
                //{
                //    returnData.Code = -200;
                //    returnData.Result = $"[開始時間][結束時間]格式錯誤";
                //    return returnData.JsonSerializationt();
                //}
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync(returnData.ServerName, returnData.ServerType, "儲位資料");

                SQLControl sQLControl = new SQLControl(Server, DB, "consumption", UserName, Password, Port, SSLMode);
                string command = $@"
                    SELECT *
                    FROM {DB}.consumption
                    WHERE 建立時間 >= '{returnData.ValueAry[0]}' AND 建立時間 < '{returnData.ValueAry[1]}'";

                List<object[]> objects = await sQLControl.WriteCommandAsync(command);
                List<consumptionClass> medClassifies = objects.SQLToClass<consumptionClass>();


                returnData.Code = 200;
                returnData.Data = medClassifies;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_by_start_end";
                returnData.Result = $"取得資料成功，共{medClassifies.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {

                if (ex.Message == "Table 'dbvm.consumption' doesn't exist") init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }

        private async Task<string> CheckCreatTable(returnData returnData)
        {
            sys_serverSettingClass sys_ServerSettingClass = await HIS_WebApi.Method.GetServerAsync(returnData.ServerName, returnData.ServerType, "儲位資料");
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable<consumptionClass>(sys_ServerSettingClass));
            return tables.JsonSerializationt(true);
        }

        public class ICP_交易記錄查詢 : IComparer<object[]>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(object[] x, object[] y)
            {
                DateTime datetime1 = x[(int)enum_交易記錄查詢資料.操作時間].ToDateTimeString_6().StringToDateTime();
                DateTime datetime2 = y[(int)enum_交易記錄查詢資料.操作時間].ToDateTimeString_6().StringToDateTime();
                int compare = DateTime.Compare(datetime2, datetime1);
                return compare;

            }
        }
        public class ICP_transactionsClass : IComparer<transactionsClass>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(transactionsClass x, transactionsClass y)
            {
                DateTime datetime1 = x.操作時間.StringToDateTime();
                DateTime datetime2 = y.操作時間.StringToDateTime();
                int compare = DateTime.Compare(datetime2, datetime1);
                return compare;

            }
        }
    }
}
