using Basic;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices; // Marshal.Copy
using System.Text.Json;
using System.Threading.Tasks;
using ZXing;
using ZXing.SkiaSharp; // 來自 ZXing.Net.Bindings.SkiaSharp
using SQLUI;
namespace HIS_WebApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class medCombo : ControllerBase
    {
        /// <summary>
        /// 初始化資料庫
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "A5",
        ///     "ServerType" : "調劑台",
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
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(medComboClass))]
        [HttpPost]
        public string init([FromBody] returnData returnData)
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
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "A5",
        ///     "ServerType" : "調劑台",
        ///     "Data": 
        ///     {
        ///        [medComboClass]
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("add")]
        [HttpPost]
        public string add([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "add";
            try
            {
                init(returnData);
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
                Table table = new Table(new enum_medCombo());

                List<medComboClass> medComboClasses = returnData.Data.ObjToClass<List<medComboClass>>();
                List<medComboClass> medComboClasses_buf = new List<medComboClass>();
                if (medComboClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }
                SQLControl sQLControl_medCombo = new SQLControl(Server, DB, table.TableName, UserName, Password, Port,MySql.Data.MySqlClient.MySqlSslMode.None);
                for(int i = 0; i < medComboClasses.Count; i++)
                {
                    string code = medComboClasses[i].藥碼;
                    if(sQLControl_medCombo.GetRowsByDefult(null,(int)enum_medCombo.藥碼, code).Count > 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"傳入資料已存在組合";
                        return returnData.JsonSerializationt();
                    }
                }
                string sn = Guid.NewGuid().ToString();
                for (int i = 0; i < medComboClasses.Count; i++)
                {
                    medComboClasses[i].GUID = Guid.NewGuid().ToString();
                    medComboClasses[i].序列號 = sn;
                }
             
                List<object[]> list_value = medComboClasses.ClassToSQL<medComboClass, enum_medCombo>();
                sQLControl_medCombo.AddRows(null, list_value);
                returnData.Code = 200;
                returnData.Result = $"新增資料共<{list_value.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = medComboClasses;
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
        /// 刪除資料
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "A5",
        ///     "ServerType" : "調劑台",
        ///     "Data": 
        ///     {
        ///        [medComboClass]
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("delete_by_guid")]
        [HttpPost]
        public string delete_by_guid([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "delete_by_guid";
            try
            {
                init(returnData);
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
                Table table = new Table(new enum_medCombo());

                List<medComboClass> medComboClasses = returnData.Data.ObjToClass<List<medComboClass>>();
                SQLControl sQLControl_medCombo = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, MySql.Data.MySqlClient.MySqlSslMode.None);


                List<object[]> list_value = medComboClasses.ClassToSQL<medComboClass, enum_medCombo>();
                sQLControl_medCombo.DeleteExtra(null, list_value);
                returnData.Code = 200;
                returnData.Result = $"刪除資料共<{list_value.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = medComboClasses;
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
        /// 取得資料
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "A5",
        ///     "ServerType" : "調劑台",
        ///     "ValueAry" : ["code"]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("get_by_code")]
        [HttpPost]
        public string get_by_code([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_code";
            try
            {
                init(returnData);
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if(returnData.ValueAry.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無代入參數";
                    return returnData.JsonSerializationt();
                }
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                Table table = new Table(new enum_medCombo());

                SQLControl sQLControl_medCombo = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, MySql.Data.MySqlClient.MySqlSslMode.None);
                List<object[]> list_value = sQLControl_medCombo.GetRowsByDefult(null, (int)enum_medCombo.藥碼, returnData.ValueAry[0]);
                if(list_value.Count == 0)
                {
                    returnData.Code = 200;
                    returnData.Result = $"取得資料共<{list_value.Count}>筆";
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Data = new List<medComboClass>();
                    return returnData.JsonSerializationt();
                }
                string sn = list_value[0][(int)enum_medCombo.序列號].ObjectToString();
                list_value = sQLControl_medCombo.GetRowsByDefult(null, (int)enum_medCombo.序列號, sn);
                List<medComboClass> medComboClasses = list_value.SQLToClass<medComboClass, enum_medCombo>();


                returnData.Code = 200;
                returnData.Result = $"取得資料共<{list_value.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = medComboClasses;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }
        }

        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_medCombo()));

            return tables.JsonSerializationt(true);
        }
    }
}
