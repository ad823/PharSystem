using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Configuration;
using HIS_DB_Lib;
namespace HIS_WebApi
{
    /// <summary>
    /// 化療藥局癌症備藥機-出料馬達輸出索引表
    /// </summary>
    [Route("api/[controller]")]
    [Route("api/CPMP_StorageConfig")]
    [ApiController]
    public class storageMedBoxIOConfig : ControllerBase
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        /// <summary>
        /// 初始化出料馬達輸出索引表
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "cheom",
        ///     "ServerType" : "癌症備藥機",
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
        public string init(returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_ServerSetting = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "一般資料").FirstOrDefault();
                if (sys_ServerSetting == null) 
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                return CheckCreatTable(sys_ServerSetting);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// 取得所有 Storage 藥盒 I/O（鎖控 / 馬達 / 位置）設定資料
        /// </summary>
        /// <remarks>
        /// 此 API 會依據傳入的 ServerName、ServerType，
        /// 從 sys_serverSetting 取得資料庫連線資訊，
        /// 並讀取 storage_med_box_io_config（enum_storageMedBoxIOConfig）
        /// 回傳所有馬達/鎖控/位置/方位/區域的 I/O 設定資料。
        ///
        /// 【請求 JSON 範例】：
        /// <code>
        /// {
        ///   "ServerName": "cheom",
        ///   "ServerType": "癌症備藥機",
        ///   "Data": null
        /// }
        /// </code>
        ///
        /// 【成功回應 JSON 範例】：
        /// <code>
        /// {
        ///   "Code": 200,
        ///   "Result": "OK",
        ///   "ServerName": "cheom",
        ///   "ServerType": "癌症備藥機",
        ///   "Data": [
        ///     {
        ///       "GUID": "xxxx-xxxx",
        ///       "IP": "192.168.1.10",
        ///       "lock_output_index": "0",
        ///       "lock_output_trigger": "PULSE_300",
        ///       "lock_input_index": "1",
        ///       "lock_input_state": "OFF",
        ///       "motor_output_index": "2",
        ///       "motor_output_trigger": "PULSE_500",
        ///       "motor_input_index": "3",
        ///       "motor_input_state": "ON",
        ///       "motor_input_delay_time": "300",
        ///       "position_x": "15",
        ///       "position_y": "7",
        ///       "box_direction": "Left",
        ///       "area": "A1"
        ///     }
        ///   ]
        /// }
        /// </code>
        ///
        /// 【失敗回應 JSON 範例（找不到 Server 設定）】：
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "找無Server資料",
        ///   "Data": null
        /// }
        /// </code>
        ///
        /// 【例外錯誤格式】：
        /// Code = -200，Result = 例外的錯誤訊息字串。
        /// </remarks>
        /// <param name="returnData">
        /// 傳入結構包含：
        /// - ServerName：資料來源伺服器名稱（必要）
        /// - ServerType：伺服器類型（必要，如：癌症備藥機）
        /// - Data：可為 null，本 API 不需傳入資料
        /// </param>
        /// <returns>
        /// 回傳 returnData 序列化後的 JSON 字串。
        /// </returns>
        [Route("get_all")]
        [HttpPost]
        public string get_all(returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_ServerSetting = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "一般資料").FirstOrDefault();
                if (sys_ServerSetting == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string Server = sys_ServerSetting.Server;
                string DB = sys_ServerSetting.DBName;
                string UserName = sys_ServerSetting.User;
                string Password = sys_ServerSetting.Password;
                uint Port = (uint)sys_ServerSetting.Port.StringToInt32();
                Table table = new Table(new enum_storageMedBoxIOConfig());
                SQLControl sQLControl = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);

                List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses = sQLControl.GetAllRows(null).SQLToClass<storageMedBoxIOConfigClass , enum_storageMedBoxIOConfig>();
                returnData.Data = storageMedBoxIOConfigClasses;
                string json_out = returnData.JsonSerializationt(true);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 新增或更新 Storage 藥盒 I/O 設定資料
        /// </summary>
        /// <remarks>
        /// 此 API 接收多筆 storageMedBoxIOConfigClass 設定資料：
        /// - 若資料的 IP 在資料庫已存在 → 執行修改(Update)
        /// - 若資料的 IP 不存在 → 新增(Add)
        ///
        /// 判斷方式：
        /// 1. 以 IP 作為唯一識別（不允許重複）
        /// 2. 已存在的資料會沿用原本 GUID
        /// 3. 新增資料會自動產生 GUID
        ///
        /// 【請求 JSON 範例】
        /// <code>
        /// {
        ///   "ServerName": "cheom",
        ///   "ServerType": "癌症備藥機",
        ///   "Data": [
        ///     {
        ///       "IP": "192.168.1.10",
        ///       "lock_output_index": "0",
        ///       "lock_output_trigger": "PULSE_300",
        ///       "lock_input_index": "1",
        ///       "lock_input_state": "OFF",
        ///       "motor_output_index": "2",
        ///       "motor_output_trigger": "PULSE_500",
        ///       "motor_input_index": "3",
        ///       "motor_input_state": "ON",
        ///       "motor_input_delay_time": "200",
        ///       "position_x": "12",
        ///       "position_y": "5",
        ///       "box_direction": "Left",
        ///       "area": "A1"
        ///     }
        ///   ]
        /// }
        /// </code>
        ///
        /// 【成功回應 JSON 範例】
        /// <code>
        /// {
        ///   "Code": 200,
        ///   "Result": "新增<1>筆,修改<0>筆",
        ///   "ServerName": "cheom",
        ///   "ServerType": "癌症備藥機",
        ///   "Data": [
        ///     {
        ///       "GUID": "xxxx-xxxx",
        ///       "IP": "192.168.1.10",
        ///       "lock_output_index": "0",
        ///       "lock_output_trigger": "PULSE_300",
        ///       "lock_input_index": "1",
        ///       "lock_input_state": "OFF",
        ///       "motor_output_index": "2",
        ///       "motor_output_trigger": "PULSE_500",
        ///       "motor_input_index": "3",
        ///       "motor_input_state": "ON",
        ///       "motor_input_delay_time": "200",
        ///       "position_x": "12",
        ///       "position_y": "5",
        ///       "box_direction": "Left",
        ///       "area": "A1"
        ///     }
        ///   ]
        /// }
        /// </code>
        ///
        /// 【失敗範例：找不到 Server 設定】
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "找無Server資料",
        ///   "Data": null
        /// }
        /// </code>
        ///
        /// 【失敗範例：Data 為空】
        /// <code>
        /// {
        ///   "Code": -200,
        ///   "Result": "Data 資料數量為 '0'",
        ///   "Data": null
        /// }
        /// </code>
        ///
        /// 【例外錯誤格式】
        /// Code = -200, Result = 錯誤訊息字串
        ///
        /// </remarks>
        /// <param name="returnData">
        /// 請求資料結構：
        /// - ServerName：要操作的伺服器名稱（必要）
        /// - ServerType：伺服器類型（必要）
        /// - Data：List&lt;storageMedBoxIOConfigClass&gt;，可包含多筆資料
        /// </param>
        /// <returns>回傳序列化後的 JSON 字串。</returns>
        [Route("add_update")]
        [HttpPost]
        public string add_update(returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_ServerSetting = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "一般資料").FirstOrDefault();
                if (sys_ServerSetting == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string Server = sys_ServerSetting.Server;
                string DB = sys_ServerSetting.DBName;
                string UserName = sys_ServerSetting.User;
                string Password = sys_ServerSetting.Password;
                uint Port = (uint)sys_ServerSetting.Port.StringToInt32();
                Table table = new Table(new enum_storageMedBoxIOConfig());
                SQLControl sQLControl = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses_sql = sQLControl.GetAllRows(null).SQLToClass<storageMedBoxIOConfigClass, enum_storageMedBoxIOConfig>();

                List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses = returnData.Data.ObjToClass<List<storageMedBoxIOConfigClass>>();
                List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses_add = new List<storageMedBoxIOConfigClass>();
                List<storageMedBoxIOConfigClass> storageMedBoxIOConfigClasses_replace = new List<storageMedBoxIOConfigClass>();


                if (storageMedBoxIOConfigClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data 資料不合法";
                    return returnData.JsonSerializationt();
                }
                if (storageMedBoxIOConfigClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data 資料數量為'0'";
                    return returnData.JsonSerializationt();
                }

                foreach(storageMedBoxIOConfigClass storageMedBoxIO in storageMedBoxIOConfigClasses)
                {
                    string IP = storageMedBoxIO.IP;
                    storageMedBoxIOConfigClass temp = storageMedBoxIOConfigClasses_sql.Where(x=> x.IP == IP).FirstOrDefault();

                    if (temp != null)
                    {
                        storageMedBoxIO.GUID = temp.GUID;
                        storageMedBoxIOConfigClasses_replace.Add(storageMedBoxIO);
                    }
                    else
                    {
                        storageMedBoxIO.GUID = Guid.NewGuid().ToString();
                        storageMedBoxIOConfigClasses_add.Add(storageMedBoxIO);
                    }
                }
                if (storageMedBoxIOConfigClasses_replace.Count > 0) sQLControl.UpdateByDefulteExtra(null, storageMedBoxIOConfigClasses_replace.ClassToSQL<storageMedBoxIOConfigClass, enum_storageMedBoxIOConfig>());
                if (storageMedBoxIOConfigClasses_add.Count > 0) sQLControl.AddRows(null, storageMedBoxIOConfigClasses_add.ClassToSQL<storageMedBoxIOConfigClass, enum_storageMedBoxIOConfig>());
                returnData.Result = $"新增<{storageMedBoxIOConfigClasses_add.Count}>筆,修改<{storageMedBoxIOConfigClasses_replace.Count}>筆";
                returnData.Data = storageMedBoxIOConfigClasses;
                string json_out = returnData.JsonSerializationt(true);
                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
        }

        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_storageMedBoxIOConfig());
            return table.JsonSerializationt(true);       
        }
    }
}
