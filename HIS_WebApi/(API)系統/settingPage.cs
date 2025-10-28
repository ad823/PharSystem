using Basic;
using Google.Protobuf.WellKnownTypes;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SQLUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace HIS_WebApi._API_系統
{
    [Route("api/[controller]")]
    [ApiController]
    public class settingPage : ControllerBase
    {
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private static readonly Lazy<Task<(string Server, string DB, string UserName, string Password, uint Port)>>
             serverInfoTask = new Lazy<Task<(string, string, string, string, uint)>>(async () =>
             {
                 var (Server, DB, UserName, Password, Port) = await Method.GetServerInfoAsync("Main", "網頁", "VM端");

                 if (string.IsNullOrWhiteSpace(Password))
                     throw new SecurityException("Database password cannot be null or empty (medUnit).");

                 return (Server, DB, UserName, Password, Port);
             });
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(200, "settingPageClass物件", typeof(settingPageClass))]

        [HttpPost("init_settingPage")]
        public string init_settingPage([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "init_settingPage";
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
                return CheckCreatTable(sys_serverSettingClasses[0], new enum_settingPage());
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        ///以page_name取得資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":["page_name"]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_page_name")]
        public async Task<string> get_by_page_name([FromBody] returnData returnData,CancellationToken ct)
        {
            loadData();
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_page_name";
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
                    returnData.Result = $"returnData.ValueAry 內容應為[\"page_name\"]";
                    return returnData.JsonSerializationt(true);
                }
                string 頁面名稱 = returnData.ValueAry[0];
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "settingPage", UserName, Password, Port, SSLMode);
                string command = $@"SELECT * 
                        FROM {DB}.settingPage 
                        WHERE 頁面名稱 = '{頁面名稱}';";
                
                List<object[]> list_settingPage = await sQLControl.WriteCommandAsync(command , ct);

                if (list_settingPage == null || list_settingPage.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<settingPageClass> settingPageClasses = list_settingPage.SQLToClass<settingPageClass, enum_settingPage>();
                for(int i = 0; i < settingPageClasses.Count; i++)
                {
                    if (settingPageClasses[i].選項.StringIsEmpty() == false)
                    {
                        List<string> option = settingPageClasses[i].選項.Split(";").ToList();
                        settingPageClasses[i].option = option;
                    }
                    if(settingPageClasses[i].欄位代碼 == "display_block" || settingPageClasses[i].欄位代碼 == "display_block_nocheck")
                    {
                        List<uiConfig> uiConfigs = Convert(settingPageClasses[i].設定值);
                        settingPageClasses[i].value = uiConfigs;
                    }                   
                    else
                    {
                        settingPageClasses[i].value = settingPageClasses[i].設定值;

                    }
                }
                settingPageClasses.Sort(new settingPageClass.ICP_By_type());
                returnData.Code = 200;
                returnData.Data = settingPageClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"取得頁面 : {頁面名稱}的資料";
                string result = await returnData.JsonSerializationtAsync(true);
                return result;
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                string result = await returnData.JsonSerializationtAsync(true);
                return result;
            }
        }
        /// <summary>
        ///以page_name取得資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":["page_name"]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_by_page_name_cht")]
        public async Task<string> get_by_page_name_cht([FromBody] returnData returnData, CancellationToken ct)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_page_name";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"page_name\",\"cht\"]";
                    return returnData.JsonSerializationt(true);
                }
                string 頁面名稱 = returnData.ValueAry[0];
                string 欄位名稱 = returnData.ValueAry[1];
                var (Server, DB, UserName, Password, Port) = await serverInfoTask.Value;

                SQLControl sQLControl = new SQLControl(Server, DB, "settingPage", UserName, Password, Port, SSLMode);

                (string sql, object param) = getCommand(頁面名稱, 欄位名稱);
                

                List<object[]> list_settingPage = await sQLControl.WriteCommandAsync(sql, param, ct);

                if (list_settingPage == null || list_settingPage.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<settingPageClass> settingPageClasses = list_settingPage.SQLToClass<settingPageClass, enum_settingPage>();
               
                for (int i = 0; i < settingPageClasses.Count; i++)
                {
                    if (settingPageClasses[i].選項.StringIsEmpty() == false)
                    {
                        List<string> option = settingPageClasses[i].選項.Split(";").ToList();
                        settingPageClasses[i].option = option;
                    }
                    if (settingPageClasses[i].欄位代碼 == "display_block" || settingPageClasses[i].欄位代碼 == "display_block_nocheck")
                    {
                        List<uiConfig> uiConfigs = Convert(settingPageClasses[i].設定值);
                        settingPageClasses[i].value = uiConfigs;
                    }
                    else
                    {
                        settingPageClasses[i].value = settingPageClasses[i].設定值;

                    }
                }

                returnData.Code = 200;
                returnData.Data = settingPageClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"取得頁面 : {頁面名稱}{欄位名稱}的資料";
                string result = await returnData.JsonSerializationtAsync(true);
                return result;
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                string result = await returnData.JsonSerializationtAsync(true);
                return result;
            }
        }
        /// <summary>
        ///取得頁面設定資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":["page_name"]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_all")]
        public async Task<string> get_all([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_all";
            try
            {
                loadData();
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "settingPage", UserName, Password, Port, SSLMode);
                List<object[]> list_settingPage = sQLControl.GetAllRows(null);
                if (list_settingPage.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<settingPageClass> settingPageClasses = list_settingPage.SQLToClass<settingPageClass, enum_settingPage>();
                returnData.Code = 200;
                returnData.Data = settingPageClasses;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"取得設定頁面資料";
                string result = await returnData.JsonSerializationtAsync(true);
                return result;
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                string result = await returnData.JsonSerializationtAsync(true);
                return result;
            }
        }
        /// <summary>
        /// 增加資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "Data":
        ///         [
        ///             {
        ///                 "page_name":"",
        ///                 "cht":"",
        ///                 "name":"",
        ///                 "value_type":"",
        ///                 "value_db":""
        ///             }
        ///         ]
        ///         
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<string> add([FromBody] returnData returnData)
        {
            returnData.Method = "add";
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                List<settingPageClass> input_settingPageClass = returnData.Data.ObjToClass<List<settingPageClass>>();
                if (input_settingPageClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
                
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "settingPage", UserName, Password, Port, SSLMode);
                string command = $@"SELECT * 
                        FROM {DB}.settingPage;";

                List<object[]> list_settingPage = await sQLControl.WriteCommandAsync(command);
                //List<object[]> list_settingPage = sQLControl.GetAllRows(null);
                List<settingPageClass> settingPageClasses = list_settingPage.SQLToClass<settingPageClass, enum_settingPage>();

                List<settingPageClass> settingPage_buff = new List<settingPageClass>();
                List<settingPageClass> settingPage_add = new List<settingPageClass>();
                List<settingPageClass> settingPage = new List<settingPageClass>();


                foreach (var item in input_settingPageClass)
                {
                    string 頁面名稱 = item.頁面名稱;
                    string 欄位名稱 = item.欄位名稱;
                    settingPage_buff = settingPageClasses.Where(temp => temp.頁面名稱 == 頁面名稱 && temp.欄位名稱 == 欄位名稱).ToList();
                    if (settingPage_buff == null || settingPage_buff.Count == 0)
                    {
                        item.GUID = Guid.NewGuid().ToString();
                        settingPage_add.Add(item);

                    }
                    else
                    {
                        settingPage.AddRange(settingPage_buff);
                    }
                    
                }
                List<string> GUID = settingPage.Select(temp => temp.GUID).ToList();
                List<settingPageClass> settingPage_delete = settingPageClasses.Where(temp => GUID.Contains(temp.GUID) == false).ToList();
                List<object[]> add = settingPage_add.ClassToSQL<settingPageClass, enum_settingPage>();
                List<object[]> delete = settingPage_delete.ClassToSQL<settingPageClass, enum_settingPage>();

                if (add.Count > 0) sQLControl.AddRows(null, add);
                if (delete.Count > 0) sQLControl.DeleteExtra(null, delete);

                returnData.Code = 200;
                returnData.Data = input_settingPageClass;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"資料設定，新增{add.Count}筆，刪除{delete.Count}成功";
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
        /// 更新資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":["GUID", "value"]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("update")]
        public string update([FromBody] returnData returnData)
        {
            returnData.Method = "update";
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[\"GUID\",\"value\"]";
                    return returnData.JsonSerializationt(true);
                }

                string GUID = returnData.ValueAry[0];
                string value_db = returnData.ValueAry[1];

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "settingPage", UserName, Password, Port, SSLMode);
                List<object[]> list_settingPage = sQLControl.GetRowsByDefult(null, (int)enum_settingPage.GUID, GUID);
                list_settingPage[0][(int)enum_settingPage.設定值] = value_db;
                sQLControl.UpdateByDefulteExtra(null, list_settingPage);

                returnData.Code = 200;
                returnData.Data = list_settingPage;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"資料變更共{list_settingPage.Count}筆 成功";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }

        }

        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass, System.Enum enumInstance)
        {
            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, enumInstance);
            return table.JsonSerializationt(true);
        }
        private Dictionary<string, string> dicColumnName = new Dictionary<string, string>
        {
            { "ordseq", "序號" },
            { "dosage", "劑量" },
            { "dunit", "單位" },
            { "freqn", "頻次" },
            { "route", "途徑" },
            { "code", "藥碼" },
            { "storage", "儲位" }
        };
        private List<uiConfig> Convert(string value_db)
        {
            List<uiConfig> uiConfigs = new List<uiConfig>();
            List<string> list_value = value_db.Split(";").ToList();
            foreach(string key in dicColumnName.Keys)
            {
                uiConfig uiConfig = new uiConfig();
                uiConfig.欄位代碼 = key;
                uiConfig.欄位名稱 = dicColumnName[key];

                if (list_value.Contains(key))
                {
                    uiConfig.設定值 = "True";
                }
                else
                {
                    uiConfig.設定值 = "False";
                }
                uiConfigs.Add(uiConfig);
            }
            return uiConfigs;
        }
        private void loadData()
        {
            init_settingPage(new returnData());
            string data = Basic.MyFileStream.LoadFileAllText(@"./setting_page.txt", "utf-8");
            returnData returnData = data.JsonDeserializet<returnData>();
            add(returnData);
        }
        private (string Sql, object Parameters) getCommand(string page_name, string cht)
        {
            string sql = $@"
                SELECT *
                FROM dbvm.settingPage
                WHERE 頁面名稱 = @page_name
                AND 欄位名稱 =  @cht";

            var parameters = new
            {
                page_name = page_name,
                cht = cht,
            };

            return (sql, parameters);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<settingPageClass> get_by_page_name_cht(string 頁面名稱, string 欄位名稱, CancellationToken ct = default) 
        {
            returnData returnData = new returnData();
            returnData.ValueAry = new List<string> { 頁面名稱, 欄位名稱 };
            string result =  await get_by_page_name_cht(returnData, ct);
            returnData =  result.JsonDeserializet<returnData>();
            if (returnData == null || returnData.Code != 200) 
            {
                Logger.Log($"{result}");
                return null;
            } 
            settingPageClass settingPageClass = returnData.Data.ObjToClass<List<settingPageClass>>().FirstOrDefault();
            return settingPageClass;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_by_page_name(string 頁面名稱, CancellationToken ct = default)
        {
            returnData returnData = new returnData();
            returnData.ValueAry = new List<string> { 頁面名稱};
            string result = await get_by_page_name(returnData, ct);
            returnData = result.JsonDeserializet<returnData>();
            if (returnData.Code != 200) Logger.Log($"{returnData.JsonSerializationtAsync(true)}");
            return returnData;
        }


    }
}
