using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Drawing;
using System.Text;
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
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using DrawingClass;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class materialRequisition : Controller
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
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(materialRequisitionClass))]
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
        ///     "Data": 
        ///     {
        ///        [materialRequisitionClass Ary]
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("add")]
        [HttpPost]
        public string POST_add([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "add";
            try
            {
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

                List<materialRequisitionClass> materialRequisitionClasses = returnData.Data.ObjToClass<List<materialRequisitionClass>>();
                List<materialRequisitionClass> materialRequisitionClasses_buf = new List<materialRequisitionClass>();
                if (materialRequisitionClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }

                for (int i = 0; i < materialRequisitionClasses.Count; i++)
                {
                    if (materialRequisitionClasses[i].藥碼.StringIsEmpty()) continue;
                    materialRequisitionClasses[i].GUID = Guid.NewGuid().ToString();
                    materialRequisitionClasses[i].申領時間 = DateTime.Now.ToDateTimeString_6();
                    materialRequisitionClasses[i].核撥時間 = DateTime.MinValue.ToDateTimeString_6();
                    materialRequisitionClasses[i].簽收時間 = DateTime.MinValue.ToDateTimeString_6();

                    materialRequisitionClasses[i].申領庫庫存 = "";
                    materialRequisitionClasses[i].申領庫結存 = "";
                    //materialRequisitionClasses[i].實撥庫庫存 = "";
                    materialRequisitionClasses[i].實撥庫結存 = "";
                    materialRequisitionClasses[i].狀態 = "等待過帳";
                    materialRequisitionClasses_buf.Add(materialRequisitionClasses[i]);
                }
                List<object[]> list_value = materialRequisitionClasses_buf.ClassToSQL<materialRequisitionClass, enum_materialRequisition>();
                Table table = new Table(new enum_materialRequisition());
                SQLControl sQLControl_materialRequisition = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                sQLControl_materialRequisition.AddRows(null, list_value);
                returnData.Code = 200;
                returnData.Result = $"新增申領資料共<{list_value.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = materialRequisitionClasses_buf;
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
        /// 根據 GUID 獲取更新申領單。
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        [materialRequisitionClass Ary]
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("update_by_guid")]
        [HttpPost]
        public string POST_update([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_by_guid";
            try
            {
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

                List<materialRequisitionClass> materialRequisitionClasses = returnData.Data.ObjToClass<List<materialRequisitionClass>>();
                List<materialRequisitionClass> materialRequisitionClasses_buf = new List<materialRequisitionClass>();
                if (materialRequisitionClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }

    
                List<object[]> list_value = materialRequisitionClasses.ClassToSQL<materialRequisitionClass, enum_materialRequisition>();
                Table table = new Table(new enum_materialRequisition());
                SQLControl sQLControl_materialRequisition = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                sQLControl_materialRequisition.UpdateByDefulteExtra(null, list_value);
                returnData.Code = 200;
                returnData.Result = $"更新申領資料共<{list_value.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = materialRequisitionClasses_buf;
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
        /// 根據申領時間獲取申領單
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///     
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///        "起始時間",
        ///        "結束時間"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("get_by_requestTime")]
        [HttpPost]
        public string POST_get_by_requestTime([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_requestTime";
            try
            {
                GET_init(returnData);
                List<sys_serverSettingClass> sys_serverSettingClasses =ServerSettingController.GetAllServerSetting();
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

                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[起始時間][結束時間]";
                    return returnData.JsonSerializationt(true);
                }
                string 起始時間 = returnData.ValueAry[0];
                string 結束時間 = returnData.ValueAry[1];
                if (起始時間.Check_Date_String() == false || 結束時間.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"時間範圍格式錯誤";
                    return returnData.JsonSerializationt(true);
                }
                DateTime dateTime_st = 起始時間.StringToDateTime();
                DateTime dateTime_end = 結束時間.StringToDateTime();

                Table table = new Table(new enum_materialRequisition());
                SQLControl sQLControl_materialRequisition = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_materialRequisition.GetRowsByBetween(null, (int)enum_materialRequisition.申領時間, 起始時間, 結束時間);
                List<materialRequisitionClass> materialRequisitionClasses = list_value.SQLToClass<materialRequisitionClass, enum_materialRequisition>();
                List<materialRequisitionClass> materialRequisitionClasses_buf = new List<materialRequisitionClass>();
                if (materialRequisitionClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料取得異常";
                    return returnData.JsonSerializationt();
                }
                if (materialRequisitionClasses.Count == 0)
                {
                    returnData.Code = 200;
                    returnData.Result = $"查無資料!";
                    returnData.Data = materialRequisitionClasses;
                    return returnData.JsonSerializationt();
                }
                materialRequisitionClasses.Sort(new materialRequisitionClass.ICP_By_requestTime());

                returnData.Code = 200;
                returnData.Result = $"取得申領資料共<{materialRequisitionClasses.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = materialRequisitionClasses;
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
        /// 根據 GUID 獲取申領單。
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "GUID"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("get_by_guid")]
        [HttpPost]
        public string POST_get_by_guid([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_guid";
            try
            {
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

                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID]";
                    return returnData.JsonSerializationt(true);
                }

                Table table = new Table(new enum_materialRequisition());
                SQLControl sQLControl_materialRequisition = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_materialRequisition.GetRowsByDefult(null, (int)enum_materialRequisition.GUID, returnData.ValueAry[0]);

                if (list_value.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                materialRequisitionClass materialRequisition = list_value[0].SQLToClass<materialRequisitionClass, enum_materialRequisition>();
                returnData.Code = 200;
                returnData.Result = $"更新申領資料共<{list_value.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = materialRequisition;
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
        /// 修改實撥量。
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        "GUID": "unique-guid",
        ///        "actualQuantity": "new-actual-quantity"
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("update_actual_quantity")]
        [HttpPost]
        public string POST_update_actual_quantity([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_actual_quantity";
            try
            {
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

                materialRequisitionClass updatedData = returnData.Data.ObjToClass<materialRequisitionClass>();
                if (updatedData == null || updatedData.GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }

                Table table = new Table(new enum_materialRequisition());
                SQLControl sQLControl_materialRequisition = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);

                List<object[]> list_value = sQLControl_materialRequisition.GetRowsByDefult(null, (int)enum_materialRequisition.GUID, updatedData.GUID);
                if (list_value.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料!";
                    return returnData.JsonSerializationt();
                }

                materialRequisitionClass existingData = list_value[0].SQLToClass<materialRequisitionClass, enum_materialRequisition>();
                existingData.實撥量 = updatedData.實撥量;

                List<object[]> updatedListValue = new List<object[]> { existingData.ClassToSQL<materialRequisitionClass, enum_materialRequisition>() };

                sQLControl_materialRequisition.UpdateByDefulteExtra(null, updatedListValue);

                returnData.Code = 200;
                returnData.Result = $"更新成功! 實撥量 : {updatedData.實撥量}";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = existingData;
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
        /// 修改實撥量。
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        "GUID": "unique-guid",
        ///        "actualQuantity": "new-actual-quantity"
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("update_signed_quantity")]
        [HttpPost]
        public string update_signed_quantity([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_signed_quantity";
            try
            {
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

                materialRequisitionClass updatedData = returnData.Data.ObjToClass<materialRequisitionClass>();
                if (updatedData == null || updatedData.GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }

                Table table = new Table(new enum_materialRequisition());
                SQLControl sQLControl_materialRequisition = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);

                List<object[]> list_value = sQLControl_materialRequisition.GetRowsByDefult(null, (int)enum_materialRequisition.GUID, updatedData.GUID);
                if (list_value.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料!";
                    return returnData.JsonSerializationt();
                }

                materialRequisitionClass existingData = list_value[0].SQLToClass<materialRequisitionClass, enum_materialRequisition>();
                existingData.簽收量 = updatedData.簽收量;

                List<object[]> updatedListValue = new List<object[]> { existingData.ClassToSQL<materialRequisitionClass, enum_materialRequisition>() };

                sQLControl_materialRequisition.UpdateByDefulteExtra(null, updatedListValue);

                returnData.Code = 200;
                returnData.Result = $"更新成功! 簽收量 : {updatedData.簽收量}";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = existingData;
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
        /// 修改申領量。
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        "GUID": "unique-guid",
        ///        "requestedQuantity": "new-quantity"
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("update_qty")]
        [HttpPost]
        public string update_qty_by_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_qty_by_GUID";
            try
            {
               
              
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                materialRequisitionClass updatedData = returnData.Data.ObjToClass<materialRequisitionClass>();
                if (updatedData == null || updatedData.GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }

                Table table = new Table(new enum_materialRequisition());
                SQLControl sQLControl_materialRequisition = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);

                List<object[]> list_value = sQLControl_materialRequisition.GetRowsByDefult(null, (int)enum_materialRequisition.GUID, updatedData.GUID);
                if (list_value.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料!";
                    return returnData.JsonSerializationt();
                }

                materialRequisitionClass existingData = list_value[0].SQLToClass<materialRequisitionClass, enum_materialRequisition>();
                existingData.申領量 = updatedData.申領量;
                List<object[]> updatedListValue = new List<object[]> { existingData.ClassToSQL<materialRequisitionClass, enum_materialRequisition>() };
                sQLControl_materialRequisition.UpdateByDefulteExtra(null, updatedListValue);

                returnData.Code = 200;
                returnData.Result = $"更新成功! 申領量 : {updatedData.申領量}";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = existingData;
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
        /// 修改通知註記。
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        "GUID": "unique-guid",
        ///        "notice": "True/False"
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [HttpPost("update_notice")]
        public string update_notice([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_notice";
            try
            {


                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                materialRequisitionClass updatedData = returnData.Data.ObjToClass<materialRequisitionClass>();
                if (updatedData == null || updatedData.GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }

                Table table = new Table(new enum_materialRequisition());
                SQLControl sQLControl_materialRequisition = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);

                List<object[]> list_value = sQLControl_materialRequisition.GetRowsByDefult(null, (int)enum_materialRequisition.GUID, updatedData.GUID);
                if (list_value.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料!";
                    return returnData.JsonSerializationt();
                }

                materialRequisitionClass existingData = list_value[0].SQLToClass<materialRequisitionClass, enum_materialRequisition>();
                existingData.通知註記 = updatedData.通知註記.StringToBool().ToString();
                List<object[]> updatedListValue = new List<object[]> { existingData.ClassToSQL<materialRequisitionClass, enum_materialRequisition>() };
                sQLControl_materialRequisition.UpdateByDefulteExtra(null, updatedListValue);

                returnData.Code = 200;
                returnData.Result = $"更新成功! 通知註記 : {updatedData.通知註記}";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = existingData;
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
        /// 修改狀態為等待過帳。
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        "GUID": "unique-guid"
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("update_status_pending")]
        [HttpPost]
        public string POST_update_status_pending([FromBody] returnData returnData)
        {
            return UpdateStatus(returnData, "等待過帳");
        }
        /// <summary>
        /// 修改狀態為已過帳。
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        "GUID": "unique-guid"
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("update_status_posted")]
        [HttpPost]
        public string POST_update_status_posted([FromBody] returnData returnData)
        {
            return UpdateStatus(returnData, "已過帳");
        }
        /// <summary>
        /// 以請領時間範圍下載請領單
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
        ///        "起始時間",
        ///        "結束時間"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>Excel</returns>
        [Route("download_excel_by_requestTime")]
        [HttpPost]
        public async Task<ActionResult> Post_download_excel_by_requestTime([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "download_excel_by_requestTime";
            try
            {
                string VM_API = Method.GetServerAPI("Main", "網頁", "download_excel_by_requestTime");
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
                            var fileName = $"{DateTime.Now.ToDateString("-")}_申領明細.xlsx";

                            return File(fileBytes, contentType, fileName);
                        }
                        else
                        {
                            return Content($"下載失敗：{response.StatusCode}");
                        }
                    }
                }
                string json_result = POST_get_by_requestTime(returnData);

                if (json_result.StringIsEmpty()) return null;

                returnData returnData_result = json_result.JsonDeserializet<returnData>();

                List<materialRequisitionClass> materialRequisitionClasses = returnData_result.Data.ObjToClass<List<materialRequisitionClass>>();
                if (materialRequisitionClasses == null) return null;

                List<object[]> list_materialRequisitionClasses = materialRequisitionClasses.ClassToSQL<materialRequisitionClass,enum_materialRequisition>();
                System.Data.DataTable dataTable = list_materialRequisitionClasses.ToDataTable(new enum_materialRequisition());
                dataTable = dataTable.ReorderTable(new enum_materialRequisition_Excel_Export());
                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";

                byte[] excelData = dataTable.NPOI_GetBytes(Excel_Type.xlsx);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_申領明細.xlsx"));
            }
            catch(Exception ex)
            {
                return null;
            }
            finally
            {
      
            }  
     
        }
        /// <summary>
        /// 上傳一般申領語音檔案。
        /// </summary>
        /// <remarks>
        /// 此方法用於上傳一般的 wav 格式語音檔案，並將檔案存放於應用程式目錄下的 "wav" 資料夾中。
        /// 若上傳的檔案為空或格式不符，將回傳錯誤訊息，並不進行檔案儲存。
        /// 檔案將固定存為 "materialRequisition_normal.wav"。
        /// </remarks>
        /// <param name="file">從表單上傳的檔案，必須為 wav 格式。</param>
        /// <returns>
        /// 回傳一個 JSON 字串，包含狀態碼 (Code) 與結果訊息 (Result)。
        /// </returns>
        [Route("normal_voice_upload")]
        [HttpPost]
        public async Task<string> normal_voice_upload([FromForm] Microsoft.AspNetCore.Http.IFormFile file)
        {
            returnData returnData = new returnData();
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

                returnData.Method = "excel_upload";
                var formFile = Request.Form.Files.FirstOrDefault();

                if (formFile == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "文件不得為空";
                    return returnData.JsonSerializationt(true);
                }

                // 取得檔案副檔名
                string extension = Path.GetExtension(formFile.FileName);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // 檢查是否為wav檔案
                if (!extension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
                {
                    returnData.Code = -200;
                    returnData.Result = "僅支援wav檔案";
                    return returnData.JsonSerializationt(true);
                }

                // 指定存放目錄 (與應用程式目錄下的 wav 資料夾)
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wav");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 建立檔案完整路徑，建議可加入時間戳記或 GUID 避免檔名重複
                string fileName = Path.GetFileName("materialRequisition_normal.wav");
                string filePath = Path.Combine(folderPath, fileName);

                // 儲存檔案
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }

                returnData.Code = 200;
                returnData.Result = "檔案上傳成功";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 上傳緊急申領語音檔案。
        /// </summary>
        /// <remarks>
        /// 此方法用於上傳一般的 wav 格式語音檔案，並將檔案存放於應用程式目錄下的 "wav" 資料夾中。
        /// 若上傳的檔案為空或格式不符，將回傳錯誤訊息，並不進行檔案儲存。
        /// 檔案將固定存為 "materialRequisition_emg.wav"。
        /// </remarks>
        /// <param name="file">從表單上傳的檔案，必須為 wav 格式。</param>
        /// <returns>
        /// 回傳一個 JSON 字串，包含狀態碼 (Code) 與結果訊息 (Result)。
        /// </returns>
        [Route("emg_voice_upload")]
        [HttpPost]
        public async Task<string> emg_voice_upload([FromForm] Microsoft.AspNetCore.Http.IFormFile file)
        {
            returnData returnData = new returnData();
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

                returnData.Method = "excel_upload";
                var formFile = Request.Form.Files.FirstOrDefault();

                if (formFile == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "文件不得為空";
                    return returnData.JsonSerializationt(true);
                }

                // 取得檔案副檔名
                string extension = Path.GetExtension(formFile.FileName);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // 檢查是否為wav檔案
                if (!extension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
                {
                    returnData.Code = -200;
                    returnData.Result = "僅支援wav檔案";
                    return returnData.JsonSerializationt(true);
                }

                // 指定存放目錄 (與應用程式目錄下的 wav 資料夾)
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wav");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 建立檔案完整路徑，建議可加入時間戳記或 GUID 避免檔名重複
                string fileName = Path.GetFileName("materialRequisition_emg.wav");
                string filePath = Path.Combine(folderPath, fileName);

                // 儲存檔案
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }

                returnData.Code = 200;
                returnData.Result = "檔案上傳成功";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 下載指定的語音檔案。
        /// </summary>
        /// <remarks>
        /// 此 API 根據提供的檔名，從應用程式目錄下的 "wav" 資料夾讀取語音檔案，並以檔案串流的方式回傳給用戶端。
        /// 若檔案不存在，將回傳 404 NotFound 狀態與錯誤訊息。
        /// </remarks>
        /// <param name="fileName">
        /// 欲下載的語音檔名（包含副檔名，例如 "materialRequisition_normal.wav"）。
        /// </param>
        /// <returns>
        /// 語音檔案內容 (MIME 類型為 audio/wav) 或錯誤訊息。
        /// </returns>
        [Route("normal_voice_download")]
        [HttpPost]
        public async Task<IActionResult> normal_voice_download([FromBody] returnData returnData)
        {
            try
            {
                string fileName = "materialRequisition_normal.wav";
                // 指定語音檔案所在目錄 (應用程式目錄下的 "wav" 資料夾)
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wav");
                string filePath = Path.Combine(folderPath, fileName);

                // 檢查檔案是否存在
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { Code = -200, Result = "檔案不存在" });
                }

                // 將檔案讀入記憶體中
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                // 指定回傳的檔案內容類型
                string contentType = "audio/wav";
                return File(memory, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Code = -200, Result = ex.Message });
            }
        }
        /// <summary>
        /// 下載指定的語音檔案。
        /// </summary>
        /// <remarks>
        /// 此 API 根據提供的檔名，從應用程式目錄下的 "wav" 資料夾讀取語音檔案，並以檔案串流的方式回傳給用戶端。
        /// 若檔案不存在，將回傳 404 NotFound 狀態與錯誤訊息。
        /// </remarks>
        /// <param name="fileName">
        /// 欲下載的語音檔名（包含副檔名，例如 "materialRequisition_emg.wav"）。
        /// </param>
        /// <returns>
        /// 語音檔案內容 (MIME 類型為 audio/wav) 或錯誤訊息。
        /// </returns>
        [Route("emg_voice_download")]
        [HttpPost]
        public async Task<IActionResult> emg_voice_download([FromBody] returnData returnData)
        {
            try
            {
                string fileName = "materialRequisition_emg.wav";
                // 指定語音檔案所在目錄 (應用程式目錄下的 "wav" 資料夾)
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wav");
                string filePath = Path.Combine(folderPath, fileName);

                // 檢查檔案是否存在
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { Code = -200, Result = "檔案不存在" });
                }

                // 將檔案讀入記憶體中
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                // 指定回傳的檔案內容類型
                string contentType = "audio/wav";
                return File(memory, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Code = -200, Result = ex.Message });
            }
        }


        /// <summary>
        /// 更新狀態的通用方法。
        /// </summary>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <param name="newStatus">新的狀態</param>
        /// <returns>[returnData.Data]</returns>
        private string UpdateStatus(returnData returnData, string newStatus)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_status";
            try
            {
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

                materialRequisitionClass updatedData = returnData.Data.ObjToClass<materialRequisitionClass>();
                if (updatedData == null || updatedData.GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }

                Table table = new Table(new enum_materialRequisition());
                SQLControl sQLControl_materialRequisition = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);

                List<object[]> list_value = sQLControl_materialRequisition.GetRowsByDefult(null, (int)enum_materialRequisition.GUID, updatedData.GUID);
                if (list_value.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料!";
                    return returnData.JsonSerializationt();
                }

                materialRequisitionClass existingData = list_value[0].SQLToClass<materialRequisitionClass, enum_materialRequisition>();
                existingData.狀態 = newStatus;

                List<object[]> updatedListValue = new List<object[]> { existingData.ClassToSQL<materialRequisitionClass, enum_materialRequisition>() };

                sQLControl_materialRequisition.UpdateByDefulteExtra(null, updatedListValue);

                returnData.Code = 200;
                returnData.Result = $"更新狀態成功!";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = existingData;
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
            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_materialRequisition());
            return table.JsonSerializationt(true);
        }
    }
}
