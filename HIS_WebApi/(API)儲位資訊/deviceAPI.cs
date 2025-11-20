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
using System.Configuration;
using MyOffice;
using MyUI;
using H_Pannel_lib;
using HIS_DB_Lib;

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class deviceController : Controller
    {
        public enum enum_儲位資訊
        {
            IP,
            TYPE,
            效期,
            批號,
            庫存,
            異動量,
            Value,
        }

        static private string API_Server = "http://127.0.0.1:4433";

        static private string device_Server = ConfigurationManager.AppSettings["device_Server"];
        static private string device_DB = ConfigurationManager.AppSettings["device_DB"];
        static private string device_TableName = ConfigurationManager.AppSettings["device_TableName"];

        static private string UserName = ConfigurationManager.AppSettings["user"];
        static private string Password = ConfigurationManager.AppSettings["password"];
        static private uint Port = (uint)ConfigurationManager.AppSettings["port"].StringToInt32();
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        [Route("test")]
        [HttpGet]
        public string GET_test(string Code)
        {
            if (Code.StringIsEmpty()) return null;
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<DeviceBasic> deviceBasics = Function_Get_device(device_Server, device_DB, device_TableName, UserName, Password, Port);
            List<DeviceBasic> deviceBasics_buf = new List<DeviceBasic>();
            deviceBasics_buf = deviceBasics.SortByCode($"{Code}");
            for (int i = 0; i < deviceBasics_buf.Count; i++)
            {
                DeviceType deviceType = deviceBasics_buf[i].DeviceType;
                if (deviceType == DeviceType.EPD290 || deviceType == DeviceType.EPD290_lock)
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    H_Pannel_lib.Communication.Set_WS2812_Buffer(Startup.uDP_Class, deviceBasics_buf[i].IP, 0, Get_EPD290_LEDBytes(Color.Red));
                }
                if (deviceType == DeviceType.EPD266 || deviceType == DeviceType.EPD266_lock)
                {

                    H_Pannel_lib.Communication.Set_WS2812_Buffer(Startup.uDP_Class, deviceBasics_buf[i].IP, 0, Get_EPD266_LEDBytes(Color.Red));
                }
                if (deviceType == DeviceType.RowsLED)
                {
                    SQLControl sQLControl_RowsLED_serialize = new SQLControl(device_Server, device_DB, "rowsled_jsonstring", UserName, Password, Port, SSLMode);
                    RowsLED rowsLED = RowsLEDMethod.SQL_GetDevice(sQLControl_RowsLED_serialize, deviceBasics_buf[i].IP);
                    if (rowsLED == null) continue;
                    RowsDevice rowsDevice = rowsLED.SortByGUID(deviceBasics_buf[i].GUID);
                    if (rowsDevice == null) continue;
                    int maxLED = rowsLED.Maximum;
                    byte[] LED_Bytes = new byte[maxLED];
                    Get_Rows_LEDBytes(ref LED_Bytes, rowsDevice.StartLED, rowsDevice.EndLED, Color.Red);
                    H_Pannel_lib.Communication.Set_WS2812_Buffer(Startup.uDP_Class, deviceBasics_buf[i].IP, 0, LED_Bytes);

                }
                if (deviceType == DeviceType.EPD583 || deviceType == DeviceType.EPD583_lock)
                {
                    SQLControl sQLControl_EPD583_serialize = new SQLControl(device_Server, device_DB, "epd583_jsonstring", UserName, Password, Port, SSLMode);
                    Drawer drawer = DrawerMethod.SQL_GetDevice(sQLControl_EPD583_serialize, deviceBasics_buf[i].IP);
                    if (drawer == null) continue;
                    Box box = drawer.GetBox(deviceBasics_buf[i].GUID);
                    if (box == null) continue;
                    WS2812_Init();
                    Set_LED_UDP(Startup.uDP_Class, drawer, box, Color.Red);

                }
            }
            returnData returnData = new returnData();
            returnData.Code = 200;
            returnData.Result = $"Device取得成功!TableName : {device_TableName}";
            returnData.TimeTaken = myTimer.ToString();
            returnData.Data = deviceBasics_buf;
            return returnData.JsonSerializationt(true);
        }

        [Route("list/{value}")]
        [HttpGet]
        public string GET_list(string value)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData returnData = new returnData();
            returnData.Method = "GET_list";
            try
            {

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses.MyFind(value, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.儲位資料);
                string IP = sys_serverSettingClass.Server;
                int port = sys_serverSettingClass.Port.StringToInt32();
                if (Basic.Net.Ping(IP, port, 300) == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"伺服器無回應,{IP}";
                    return returnData.JsonSerializationt(true);
                }
                if (sys_serverSettingClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無伺服器資料";
                    return returnData.JsonSerializationt(true);
                }
                List<DeviceBasic> deviceBasics = Function_Get_device(sys_serverSettingClass);
                deviceBasics = (from temp in deviceBasics
                                    where temp.Code.StringIsEmpty() == false
                                    select temp).ToList();
                returnData.Data = deviceBasics;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = 200;
                returnData.Result = $"取得儲位資訊成功";

                return returnData.JsonSerializationt(true);

            }
            catch(Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }

        [Route("all")]
        [HttpGet]
        public string GET_all()
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData returnData = new returnData();
            List<DeviceBasic> deviceBasics = Function_Get_device(device_Server, device_DB, device_TableName, UserName, Password, Port);
            returnData.Code = 200;
            returnData.Result = $"Device取得成功!TableName : {device_TableName}";
            returnData.TimeTaken = myTimer.ToString();
            returnData.Data = deviceBasics;
            return returnData.JsonSerializationt(true);
        }


        /// <summary>
        /// 查詢儲位資料
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
        ///     }
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[DeviceBasic]陣列結構</returns>
        [Route("all")]
        [HttpPost]
        public string all(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            returnData.Method = "all";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }

                List<DeviceBasic> deviceBasics = Function_Get_device(sys_serverSettingClasses[0], returnData.TableName);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = $"Device取得成功!,共<{deviceBasics.Count}>筆資料,TableName : {returnData.TableName}";
                returnData.Data = deviceBasics;
    
                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 以Code查詢庫儲系統儲位資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "口服2",
        ///     "ServerType" : "調劑台",
        ///     "ValueAry" : 
        ///     [
        ///       "藥碼1,藥碼2,藥碼3"
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[DeviceBasic]陣列結構</returns>
        [Route("get_form_storehouse_by_codes")]
        [HttpPost]
        public string get_form_storehouse_by_codes(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

                returnData.Method = "get_form_storehouse_by_codes";
            }
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼1,藥碼2,藥碼3]";
                    return returnData.JsonSerializationt(true);
                }

                string[] Codes = returnData.ValueAry[0].Split(",");
                List<DeviceBasic> deviceBasics = Function_Get_device(sys_serverSettingClasses[0], returnData.TableName);
                Dictionary<string, List<DeviceBasic>> keyValuePairs = deviceBasics.CoverToDictionaryByCode();


                List<DeviceBasic> deviceBasics_buf = new List<DeviceBasic>();
                List<DeviceBasic> deviceBasics_result = new List<DeviceBasic>();

                for (int i = 0; i < Codes.Length; i++)
                {
                    deviceBasics_buf = keyValuePairs.SortDictionaryByCode(Codes[i]);
                    if (deviceBasics_buf.Count > 0)
                    {
                        deviceBasics_result.LockAdd(deviceBasics_buf);
                    }
                }


                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = $"Device取得成功!,共<{deviceBasics_result.Count}>筆資料,TableName : {returnData.TableName}";
                returnData.Data = deviceBasics_result;

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 以Code查詢藥局系統儲位資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "口服2",
        ///     "ServerType" : "調劑台",
        ///     "ValueAry" : 
        ///     [
        ///       "藥碼1"
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[DeviceBasic]陣列結構</returns>
        [Route("get_from_pharma_by_code")]
        [HttpPost]
        public string get_from_pharma_by_code(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

            }
         
            returnData.Method = "get_from_pharma_by_code";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                if(returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼1]";
                    return returnData.JsonSerializationt(true);
                }
                string Code = returnData.ValueAry[0];
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                List<DeviceBasic> deviceBasics = Function_讀取儲位_By_Code(Server, DB, UserName, Password, Port, Code);
              
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = $"Device取得成功!,共<{deviceBasics.Count}>筆資料,TableName : {returnData.TableName}";
                returnData.Data = deviceBasics;

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 設定儲位資料(DeviceBasic)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ds01",
        ///     "ServerType" : "藥庫",
        ///     "Data" : 
        ///     {
        ///       [deviceBasic陣列]
        ///     }
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[DeviceBasic]陣列結構</returns>
        [Route("update_deviceBasic")]
        [HttpPost]
        public string update_deviceBasic(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            returnData.Method = "update_deviceBasic";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                List<DeviceBasic> deviceBasics = returnData.Data.ObjToClass<List<DeviceBasic>>();

                if (deviceBasics == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data資料錯誤";
                    return returnData.JsonSerializationt();
                }

                Function_Set_device(sys_serverSettingClasses[0], returnData.TableName, deviceBasics);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = $"Device取得成功!,共<{deviceBasics.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 取得儲位資料(Device)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "ServerName",
        ///     "ServerType" : "ServerType",
        ///     "TableName" : "",
        ///     "ValueAry" : 
        ///     [
        ///       
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[Device]陣列結構</returns>
        [Route("get_device")]
        [HttpPost]
        public string get_device(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_device";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string tableName = returnData.ValueAry[0];

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<Device> devices = DeviceMethod.SQL_GetAllDevice(sQLControl_device);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = devices;
                returnData.Result = $"Device取得成功!,共<{devices.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 取得儲位資料(RowLED)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "中藥台",
        ///     "ServerType": "中藥調劑系統",
        ///     "TableName" : "",
        ///     "ValueAry" : 
        ///     [
        ///       
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[Device]陣列結構</returns>
        [Route("get_rowLEDs")]
        [HttpPost]
        public string get_rowLEDs(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_rowLEDs";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                string tableName = "RowsLED_Jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<RowsLED> rowsLEDs = RowsLEDMethod.SQL_GetAllRowsLED(sQLControl_device);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = rowsLEDs;
                returnData.Result = $"rowsLEDs 取得成功!,共<{rowsLEDs.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 以藥碼取得儲位資料(RowLED)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "中藥台",
        ///     "ServerType": "中藥調劑系統",
        ///     "TableName" : "",
        ///     "ValueAry" : 
        ///     [
        ///        "藥碼"
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[Device]陣列結構</returns>
        [Route("get_rowLED_By_Code")]
        [HttpPost]
        public string get_rowLED_By_Code(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_rowLED_By_Code";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼]";
                    return returnData.JsonSerializationt(true);
                }
                string Code = returnData.ValueAry[0];
                string tableName = "RowsLED_Jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<RowsLED> rowsLEDs = RowsLEDMethod.SQL_GetRowsLEDByCode(sQLControl_device, Code);

                if (rowsLEDs == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = rowsLEDs;
                returnData.Result = $"取得成功共<{rowsLEDs.Count}>筆資料!,TableName : {returnData.TableName} , Code : {Code}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 以藥碼取得儲位資料(RowLED.DeviceBasics)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "中藥台",
        ///     "ServerType": "中藥調劑系統",
        ///     "TableName" : "",
        ///     "ValueAry" : 
        ///     [
        ///        "藥碼"
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[Device]陣列結構</returns>
        [Route("get_rowLED_DeviceBasics_By_Code")]
        [HttpPost]
        public string get_rowLED_DeviceBasics_By_Code(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_rowLED_DeviceBasics_By_Code";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼]";
                    return returnData.JsonSerializationt(true);
                }
                string Code = returnData.ValueAry[0];
                string tableName = "RowsLED_Jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<DeviceBasic> deviceBasics = RowsLEDMethod.GetDeviceBasicByCode(sQLControl_device, Code);

                if (deviceBasics == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = deviceBasics;
                returnData.Result = $"取得成功共<{deviceBasics.Count}>筆資料!,TableName : {returnData.TableName} , Code : {Code}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 以IP取得儲位資料(RowLED)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "中藥台",
        ///     "ServerType": "中藥調劑系統",
        ///     "TableName" : "",
        ///     "ValueAry" : 
        ///     [
        ///        "IP",
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[Device]陣列結構</returns>
        [Route("get_rowLED_ByIP")]
        [HttpPost]
        public string get_rowLED_ByIP(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_rowLED_ByIP";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[IP]";
                    return returnData.JsonSerializationt(true);
                }
                string IP = returnData.ValueAry[0];
                string tableName = "RowsLED_Jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<RowsLED> rowsLEDs = RowsLEDMethod.SQL_GetAllRowsLED(sQLControl_device);

                RowsLED rowsLED = rowsLEDs.SortByIP(IP);
                if (rowsLED == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = rowsLED;
                returnData.Result = $"jsonStr取得成功!,TableName : {returnData.TableName} , IP : {IP}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 更新儲位資料(rowsLED)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "中藥台",
        ///     "ServerType": "中藥調劑系統",
        ///     "Data" : 
        ///     {
        ///       [rowsLED陣列]
        ///     }
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[DeviceBasic]陣列結構</returns>
        [Route("update_rowsLEDs")]
        [HttpPost]
        public string update_rowsLED(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            //returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            returnData.Method = "update_rowsLED";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string tableName = "RowsLED_Jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                List<RowsLED> rowsLEDs = returnData.Data.ObjToClass<List<RowsLED>>();
                if (rowsLEDs.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data無資料";
                    return returnData.JsonSerializationt();
                }
                if (rowsLEDs == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料錯誤";
                    return returnData.JsonSerializationt();
                }
                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_device.GetAllRows(null);
                List<object[]> list_value_buf = new List<object[]>();
                List<object[]> list_replace = new List<object[]>();
                for (int i = 0; i < rowsLEDs.Count; i++)
                {
                    string IP = rowsLEDs[i].IP;
                    list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, IP);
                    if(list_value_buf.Count > 0)
                    {
                        list_value_buf[0][(int)enum_DeviceTable.Value] = rowsLEDs[i].JsonSerializationt();
                        list_replace.Add(list_value_buf[0]);
                    }
                }

                sQLControl_device.UpdateByDefulteExtra(null, list_replace);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = $"更新rowsLED成功!,共<{list_replace.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }


        }

        /// <summary>
        /// 取得所有儲位資料 (EPD583)
        /// </summary>
        /// <remarks>
        /// 此 API 用於查詢 EPD583 顯示裝置的所有儲位資料，並回傳完整的 <c>Drawer</c> 陣列。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 <c>ServerName</c> 與 <c>ServerType</c>，並取得資料庫連線資訊。  
        /// 2. 連線至 <c>epd583_jsonstring</c> 資料表，讀取所有抽屜 (Drawer) 資料。  
        /// 3. 將資料轉換為 <c>Drawer</c> 陣列並回傳。  
        ///
        /// <b>注意事項：</b>
        /// - 此 API 會一次回傳所有 EPD583 的抽屜資料。  
        /// - 若資料庫連線設定錯誤，將回傳錯誤訊息。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "Data": null
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (成功)：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "drawers 取得成功!,共15筆資料,TableName : epd583_jsonstring",
        ///   "Data": [
        ///     { "IP": "192.168.0.10", "Name": "抽屜1", "Boxes": [[{},{},{}]] },
        ///     { "IP": "192.168.0.11", "Name": "抽屜2", "Boxes": [[{},{},{}]] }
        ///   ],
        ///   "TimeTaken": "0.124 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (失敗)：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "找無Server資料",
        ///   "Data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，Data 可為 null，僅需提供 <c>ServerName</c> 與 <c>ServerType</c>。
        /// </param>
        /// <returns>
        /// [returnData.Data] 為 <c>Drawer</c> 陣列結構。
        /// </returns>
        [Route("get_epd583_drawers")]
        [HttpPost]
        public string get_epd583_drawers(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_Drawers";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                string tableName = "epd583_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<Drawer> drawers = DrawerMethod.SQL_GetAllDrawers(sQLControl_device);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = drawers;
                returnData.Result = $"drawers 取得成功!,共<{drawers.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 依據藥碼取得儲位資料 (EPD583)
        /// </summary>
        /// <remarks>
        /// 此 API 用於查詢 EPD583 顯示裝置的儲位資料，會依據傳入的 <c>藥碼 (Code)</c> 回傳對應的 <c>Drawer</c> 陣列。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 <c>ValueAry</c> 不可為空，且必須只包含一個藥碼。  
        /// 2. 依據 <c>ServerName</c>、<c>ServerType</c> 取得資料庫連線資訊。  
        /// 3. 查詢 <c>epd583_jsonstring</c> 資料表，依據傳入的藥碼取得對應的 Drawer 清單。  
        /// 4. 若找到資料，回傳 <c>Drawer</c> 陣列；若查無資料，回傳錯誤訊息。  
        ///
        /// <b>注意事項：</b>
        /// - <c>ValueAry</c> 必須只包含一個藥碼。  
        /// - 若藥碼在資料庫中不存在，則回傳錯誤訊息。  
        /// - 此 API 可能回傳多筆資料，因為同一藥碼可能存在於多個抽屜 (Drawer)。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [ "18217" ],
        ///   "Data": null
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (成功)：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "取得成功共2筆資料!,TableName : epd583_jsonstring , Code : 18217",
        ///   "Data": [
        ///     { "IP": "192.168.0.10", "Code": "18217", "Name": "抽屜1", "Boxes": [[{},{},{}]] },
        ///     { "IP": "192.168.0.11", "Code": "18217", "Name": "抽屜2", "Boxes": [[{},{},{}]] }
        ///   ],
        ///   "TimeTaken": "0.121 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (失敗)：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "查無資料",
        ///   "Data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，ValueAry 必須包含一個藥碼 (Code)，Data 可為 null。
        /// </param>
        /// <returns>
        /// [returnData.Data] 為符合條件的 <c>Drawer</c> 陣列結構。
        /// </returns>
        [Route("get_epd583_drawer_by_code")]
        [HttpPost]
        public string get_epd583_drawer_by_code(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "epd583_Drawer_By_Code";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼]";
                    return returnData.JsonSerializationt(true);
                }
                string Code = returnData.ValueAry[0];
                string tableName = "epd583_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<Drawer> drawers = DrawerMethod.SQL_GetDrawersByCode(sQLControl_device, Code);
                if (drawers.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = drawers;
                returnData.Result = $"取得成功共<{drawers.Count}>筆資料!,TableName : {returnData.TableName} , Code : {Code}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 依據藥碼取得裝置基礎資料 (EPD583)
        /// </summary>
        /// <remarks>
        /// 此 API 用於查詢 EPD583 顯示裝置的基礎資訊，會依據傳入的 <c>藥碼 (Code)</c> 回傳對應的 <c>DeviceBasic</c> 陣列。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 <c>ValueAry</c> 不可為空，且必須只包含一個藥碼。  
        /// 2. 依據 <c>ServerName</c>、<c>ServerType</c> 取得資料庫連線資訊。  
        /// 3. 查詢 <c>epd583_jsonstring</c> 資料表，依據傳入的藥碼取得對應的 DeviceBasic 清單。  
        /// 4. 若找到資料，回傳 <c>DeviceBasic</c> 陣列；若查無資料，回傳錯誤訊息。  
        ///
        /// <b>注意事項：</b>
        /// - <c>ValueAry</c> 必須只包含一個藥碼。  
        /// - 若藥碼在資料庫中不存在，則回傳錯誤訊息。  
        /// - 此 API 可能回傳多筆資料，因為同一藥碼可能存在於多個裝置。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [ "18217" ],
        ///   "Data": null
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (成功)：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "取得成功共2筆資料!,TableName : epd583_jsonstring , Code : 18217",
        ///   "Data": [
        ///     { "IP": "192.168.0.10", "Code": "18217", "Name": "裝置1", "Value": "some data" },
        ///     { "IP": "192.168.0.11", "Code": "18217", "Name": "裝置2", "Value": "some data" }
        ///   ],
        ///   "TimeTaken": "0.137 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (失敗)：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "查無資料",
        ///   "Data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，ValueAry 必須包含一個藥碼 (Code)，Data 可為 null。
        /// </param>
        /// <returns>
        /// [returnData.Data] 為符合條件的 <c>DeviceBasic</c> 陣列結構。
        /// </returns>
        [Route("get_epd583_deviceBasics_by_code")]
        [HttpPost]
        public string get_epd583_deviceBasics_by_code(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_epd583_DeviceBasics_By_Code";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼]";
                    return returnData.JsonSerializationt(true);
                }
                string Code = returnData.ValueAry[0];
                string tableName = "epd583_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<DeviceBasic> deviceBasics = DrawerMethod.GetDeviceBasicByCode(sQLControl_device, Code);
                if (deviceBasics.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = deviceBasics;
                returnData.Result = $"取得成功共<{deviceBasics.Count}>筆資料!,TableName : {returnData.TableName} , Code : {Code}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 依據 IP 取得指定的儲位資料 (EPD583)
        /// </summary>
        /// <remarks>
        /// 此 API 用於查詢 EPD583 顯示裝置的儲位資料，會依據傳入的 <c>IP</c> 回傳對應的 <c>Drawer</c>。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 <c>ValueAry</c> 不可為空，且必須只包含一個 <c>IP</c>。  
        /// 2. 依據 <c>ServerName</c>、<c>ServerType</c> 取得資料庫連線資訊。  
        /// 3. 查詢 <c>epd583_jsonstring</c> 資料表，依據傳入的 IP 取得對應的 Drawer。  
        /// 4. 若找到資料，回傳 Drawer 結構；若查無資料，回傳錯誤訊息。  
        ///
        /// <b>注意事項：</b>
        /// - <c>ValueAry</c> 必須只包含一個 <c>IP</c>。  
        /// - 若 IP 在資料庫中不存在，則回傳錯誤訊息。  
        /// - 此 API 僅支援單一 IP 查詢。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [ "192.168.0.10" ],
        ///   "Data": null
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (成功)：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "jsonStr取得成功!,TableName : epd583_jsonstring , IP : 192.168.0.10",
        ///   "Data": {
        ///     "IP": "192.168.0.10",
        ///     "Name": "抽屜1",
        ///     "Boxes": [[{},{},{}]]
        ///   },
        ///   "TimeTaken": "0.103 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (失敗)：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "查無資料",
        ///   "Data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，ValueAry 必須包含一個 <c>IP</c>，Data 可為 null。
        /// </param>
        /// <returns>
        /// [returnData.Data] 為對應的 <c>Drawer</c> 結構。
        /// </returns>
        [Route("get_epd583_drawer_byIP")]
        [HttpPost]
        public string get_epd583_drawer_byIP(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_Drawer_ByIP";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[IP]";
                    return returnData.JsonSerializationt(true);
                }
                string IP = returnData.ValueAry[0];
                string tableName = "epd583_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                Drawer drawer = DrawerMethod.SQL_GetDrawerByIP(sQLControl_device , IP);
                if (drawer == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = drawer;
                returnData.Result = $"jsonStr取得成功!,TableName : {returnData.TableName} , IP : {IP}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 更新儲位資料 (EPD583)
        /// </summary>
        /// <remarks>
        /// 此 API 用於更新 EPD583 顯示裝置的儲位資料，會依據 <c>IP</c> 進行比對並更新對應的資料。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 Data 欄位不可為空，且必須為 <c>Drawer</c> 陣列。
        /// 2. 依據傳入資料的 <c>IP</c>，比對資料庫中的既有儲位紀錄。
        /// 3. 若 IP 存在，則更新該筆紀錄的 JSON 值。
        /// 4. 批次更新資料庫，完成後回傳更新結果。
        ///
        /// <b>注意事項：</b>
        /// - Data 必須為 <c>Drawer</c> 陣列格式。
        /// - 系統會依據 <c>IP</c> 進行比對，若 IP 在資料庫不存在則不會新增。
        /// - 僅更新傳入有對應 IP 的紀錄，其餘資料保持不變。
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "Data": [
        ///     { "IP": "192.168.0.10", "Name": "抽屜1", "Boxes": [[{},{},{}]] },
        ///     { "IP": "192.168.0.11", "Name": "抽屜2", "Boxes": [[{},{},{}]] }
        ///   ]
        /// }
        ///```
        /// <b>JSON 回應範例：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "更新Drawers成功!,共2筆資料,TableName : epd583_jsonstring",
        ///   "Data": null,
        ///   "TimeTaken": "0.145 秒"
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，Data 必須為 <c>Drawer</c> 陣列，且包含 <c>IP</c> 欄位。
        /// </param>
        /// <returns>
        /// [returnData.Result] 為更新結果，[returnData.Code] 為狀態碼，[returnData.Data] 為處理後資料。
        /// </returns>
        [Route("update_epd583_drawers")]
        [HttpPost]
        public string update_epd583_drawers(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            //returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            returnData.Method = "update_Drawers";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string tableName = "epd583_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                List<Drawer> drawers = returnData.Data.ObjToClass<List<Drawer>>();
                if (drawers.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data無資料";
                    return returnData.JsonSerializationt();
                }
                if (drawers == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料錯誤";
                    return returnData.JsonSerializationt();
                }
                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_device.GetAllRows(null);
                List<object[]> list_value_buf = new List<object[]>();
                List<object[]> list_replace = new List<object[]>();
                for (int i = 0; i < drawers.Count; i++)
                {
                    string IP = drawers[i].IP;
                    list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, IP);
                    if (list_value_buf.Count > 0)
                    {
                        list_value_buf[0][(int)enum_DeviceTable.Value] = drawers[i].JsonSerializationt();
                        list_replace.Add(list_value_buf[0]);
                    }
                }

                sQLControl_device.UpdateByDefulteExtra(null, list_replace);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = $"更新Drawers成功!,共<{list_replace.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }


        }

        /// <summary>
        /// 合併儲位 (CombineBoxes)
        /// </summary>
        /// <remarks>
        /// 此 API 用於合併指定 Drawer 內的多個 Box，會依據 <c>ValueAry</c> 內傳入的 <c>SelectColumns</c> 與 <c>SelectRows</c> 進行設定：  
        /// - 第一個 Box 作為 Master。  
        /// - 中間的 Box 設為 Slave，並指向下一個 Box。  
        /// - 最後一個 Box 設為 Slave，不再指向下一個。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 Data（Drawer）不可為空。  
        /// 2. 驗證 <c>ValueAry</c> 必須同時包含 <c>SelectColumns</c> 與 <c>SelectRows</c>，且數量需一致。  
        /// 3. 將指定座標的 Box 重設屬性（Name、Code 清空），並依規則建立 Master/Slave 關聯。  
        /// 4. 回傳更新後的 Drawer。  
        ///
        /// <b>注意事項：</b>
        /// - <c>SelectColumns</c> 與 <c>SelectRows</c> 長度必須相同。  
        /// - 若傳入的座標數量小於等於 1，不會執行合併。  
        /// - 所有被合併的 Box 資料（Name、Code）會被清空。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [
        ///     "SelectColumns=0,1,2",
        ///     "SelectRows=0,0,0"
        ///   ],
        ///   "Data": {
        ///     "IP": "192.168.0.10",
        ///     "Boxes": [[{},{},{}]]
        ///   }
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "CombineBoxes 執行成功！",
        ///   "Data": {
        ///     "IP": "192.168.0.10",
        ///     "Boxes": [[{},{},{}]]
        ///   },
        ///   "TimeTaken": "0.087 秒"
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，Data 必須包含 Drawer，ValueAry 必須有 SelectColumns 與 SelectRows
        /// </param>
        /// <returns>
        /// [returnData.Data] 會回傳更新後的 Drawer 結構
        /// </returns>
        [Route("combine_drawer_boxes")]
        [HttpPost]
        public string combine_drawer_boxes([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "combine_drawer_boxes";
            try
            {
                // 取得 Server 資料
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                // 驗證 Data 是否正確
                Drawer drawer = returnData.Data.ObjToClass<Drawer>();
                if (drawer == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入 Data 格式錯誤，必須為 Drawer";
                    return returnData.JsonSerializationt();
                }

                // 從 ValueAry 解析 SelectColumns / SelectRows
                List<int> selectColumns = new List<int>();
                List<int> selectRows = new List<int>();
                if (returnData.ValueAry != null)
                {
                    foreach (var item in returnData.ValueAry)
                    {
                        if (item.StartsWith("SelectColumns="))
                        {
                            string cols = item.Replace("SelectColumns=", "");
                            selectColumns = cols.Split(',').Select(s => s.StringToInt32()).ToList();
                        }
                        else if (item.StartsWith("SelectRows="))
                        {
                            string rows = item.Replace("SelectRows=", "");
                            selectRows = rows.Split(',').Select(s => s.StringToInt32()).ToList();
                        }
                    }
                }

                if (selectColumns.Count == 0 || selectRows.Count == 0 || selectColumns.Count != selectRows.Count)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry 中缺少 SelectColumns 或 SelectRows";
                    return returnData.JsonSerializationt();
                }

                // 呼叫 CombineBoxes 靜態函式

                Drawer updatedDrawer = DrawerMethod.CombineBoxes(drawer, selectColumns, selectRows);

                // 回傳更新後的 Drawer
                returnData.Data = updatedDrawer;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = "CombineBoxes 執行成功！";

                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"CombineBoxes 發生錯誤: {e.Message}";
                return returnData.JsonSerializationt();
            }
        }
        /// <summary>
        /// 分離儲位 (SeparateBoxes)
        /// </summary>
        /// <remarks>
        /// 此 API 用於分離指定 Drawer 內已合併的 Box，會依據 <c>ValueAry</c> 內傳入的 <c>SelectColumns</c> 與 <c>SelectRows</c> 進行重置：  
        /// - 將所有指定 Box 的 Master/Slave 屬性清空。  
        /// - 清除關聯欄位（SlaveBox_Column, SlaveBox_Row, MasterBox_Column, MasterBox_Row）。  
        /// - 清空 Name、Code。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 Data（Drawer）不可為空。  
        /// 2. 驗證 <c>ValueAry</c> 必須同時包含 <c>SelectColumns</c> 與 <c>SelectRows</c>，且數量需一致。  
        /// 3. 將指定座標的 Box 重設為獨立狀態（Slave=false, Master=-1,-1）。  
        /// 4. 回傳更新後的 Drawer。  
        ///
        /// <b>注意事項：</b>
        /// - <c>SelectColumns</c> 與 <c>SelectRows</c> 長度必須相同。  
        /// - 若傳入的座標數量小於等於 1，不會執行分離。  
        /// - 所有被分離的 Box 資料（Name、Code）會被清空。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [
        ///     "SelectColumns=0,1,2",
        ///     "SelectRows=0,0,0"
        ///   ],
        ///   "Data": {
        ///     "IP": "192.168.0.10",
        ///     "Boxes": [[{},{},{}]]
        ///   }
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "SeparateBoxes 執行成功！",
        ///   "Data": {
        ///     "IP": "192.168.0.10",
        ///     "Boxes": [[{},{},{}]]
        ///   },
        ///   "TimeTaken": "0.092 秒"
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，Data 必須包含 Drawer，ValueAry 必須有 SelectColumns 與 SelectRows
        /// </param>
        /// <returns>
        /// [returnData.Data] 會回傳更新後的 Drawer 結構
        /// </returns>
        [Route("separate_drawer_boxes")]
        [HttpPost]
        public string separate_drawer_boxes([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "separate_drawer_boxes";
            try
            {
                // 取得 Server 資料
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                // 驗證 Data 是否正確
                Drawer drawer = returnData.Data.ObjToClass<Drawer>();
                if (drawer == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入 Data 格式錯誤，必須為 Drawer";
                    return returnData.JsonSerializationt();
                }

                // 從 ValueAry 解析 SelectColumns / SelectRows
                List<int> selectColumns = new List<int>();
                List<int> selectRows = new List<int>();
                if (returnData.ValueAry != null)
                {
                    foreach (var item in returnData.ValueAry)
                    {
                        if (item.StartsWith("SelectColumns="))
                        {
                            string cols = item.Replace("SelectColumns=", "");
                            selectColumns = cols.Split(',').Select(s => s.StringToInt32()).ToList();
                        }
                        else if (item.StartsWith("SelectRows="))
                        {
                            string rows = item.Replace("SelectRows=", "");
                            selectRows = rows.Split(',').Select(s => s.StringToInt32()).ToList();
                        }
                    }
                }

                if (selectColumns.Count == 0 || selectRows.Count == 0 || selectColumns.Count != selectRows.Count)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry 中缺少 SelectColumns 或 SelectRows";
                    return returnData.JsonSerializationt();
                }

                // 呼叫 SeparateBoxes 靜態函式
                Drawer updatedDrawer = DrawerMethod.SeparateBoxes(drawer, selectColumns, selectRows);

                // 回傳更新後的 Drawer
                returnData.Data = updatedDrawer;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = "SeparateBoxes 執行成功！";

                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"SeparateBoxes 發生錯誤: {e.Message}";
                return returnData.JsonSerializationt();
            }
        }
        /// <summary>
        /// 更新儲位藥碼 (update_drawer_medcode)
        /// </summary>
        /// <remarks>
        /// 此 API 用於更新指定 Drawer 內某個 Box 的藥品資訊，會依據 <c>ValueAry</c> 內傳入的 <c>guid</c> 與 <c>code</c> 進行更新：  
        /// - 依據 guid 找到對應的 Box。  
        /// - 透過藥碼 code 查詢雲端藥檔資料。  
        /// - 更新該 Box 的藥品資訊（Name、Code、藥品屬性等）。  
        ///
        /// <b>邏輯流程：</b>  
        /// 1. 驗證 Server 設定是否存在。  
        /// 2. 驗證 Data 必須為 <c>Drawer</c> 結構。  
        /// 3. 驗證 <c>ValueAry</c> 必須包含 <c>guid</c> 與 <c>code</c>。  
        /// 4. 查詢雲端藥檔 (medClass)。  
        /// 5. 更新指定 Box 的藥品資訊。  
        /// 6. 回傳更新後的 Drawer。  
        ///
        /// <b>注意事項：</b>  
        /// - <c>guid</c> 為 Box 的唯一識別碼，必須存在於 Drawer 內。  
        /// - <c>code</c> 必須能在雲端藥檔找到對應藥品。  
        /// - <c>ValueAry</c> 的格式為「key=value」字串，例如 "guid=BOX123"。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "ValueAry": [
        ///     "guid=BOX-1234567890",
        ///     "code=MED-00123"
        ///   ],
        ///   "Data": {
        ///     "GUID": "DRAWER-ABCDEF123456",
        ///     "Name": "藥局抽屜-01",
        ///     "Boxes": [
        ///       {
        ///         "GUID": "BOX-1234567890",
        ///         "Column": 0,
        ///         "Row": 0,
        ///         "Name": "",
        ///         "Code": ""
        ///       }
        ///     ]
        ///   }
        /// }
        /// ```
        ///
        /// <b>JSON 成功回應範例：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "update_drawer_medcode 執行成功！",
        ///   "Data": {
        ///     "GUID": "DRAWER-ABCDEF123456",
        ///     "Name": "藥局抽屜-01",
        ///     "Boxes": [
        ///       {
        ///         "GUID": "BOX-1234567890",
        ///         "Column": 0,
        ///         "Row": 0,
        ///         "Name": "普拿疼錠劑 500mg",
        ///         "Code": "MED-00123"
        ///       }
        ///     ]
        ///   },
        ///   "TimeTaken": "0.084 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 錯誤回應範例：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "找無藥碼 : MED-00123 的藥品資料"
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，Data 必須包含 Drawer，ValueAry 必須有「guid=...」與「code=...」
        /// </param>
        /// <returns>
        /// [returnData.Data] 會回傳更新後的 Drawer 結構
        /// </returns>
        [Route("update_drawer_medcode")]
        [HttpPost]
        public string update_drawer_medcode([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "update_drawer_medcode";
            try
            {
                // 取得 Server 資料
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                // 驗證 Data 是否正確
                Drawer drawer = returnData.Data.ObjToClass<Drawer>();
                if (drawer == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入 Data 格式錯誤，必須為 Drawer";
                    return returnData.JsonSerializationt();
                }
                string guid = returnData.ValueAry.GetVal("guid", "");
                string code = returnData.ValueAry.GetVal("code", "");


                if (guid.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"ValueAry 中缺少 guid";
                    return returnData.JsonSerializationt();
                }
                Box box = drawer.GetByGUID(guid);
                if (box == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無 GUID : {guid} 的 Box";
                    return returnData.JsonSerializationt();
                }

                medClass medClass = medClass.get_med_clouds_by_code(API_Server, code);
                if (medClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無藥碼 : {code} 的藥品資料";
                    return returnData.JsonSerializationt();
                }
                box.SetMedClass(medClass);


                // 回傳更新後的 Drawer
                returnData.Data = drawer;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = "update_drawer_medcode 執行成功！";

                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"update_drawer_medcode 發生錯誤: {e.Message}";
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 取得所有儲位資料 (EPD266)
        /// </summary>
        /// <remarks>
        /// 此 API 用於查詢 EPD266 顯示裝置的所有儲位資料，並回傳完整的 <c>Storage</c> 陣列。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 <c>ServerName</c> 與 <c>ServerType</c>，並取得資料庫連線資訊。  
        /// 2. 連線至 <c>epd266_jsonstring</c> 資料表，讀取所有儲位資料。  
        /// 3. 將資料轉換為 <c>Storage</c> 陣列並回傳。  
        ///
        /// <b>注意事項：</b>
        /// - 此 API 會一次回傳所有 EPD266 的儲位資料。  
        /// - 若資料庫連線設定錯誤，將回傳錯誤訊息。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "Data": null
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (成功)：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "storages 取得成功!,共25筆資料,TableName : epd266_jsonstring",
        ///   "Data": [
        ///     { "IP": "192.168.0.10", "Name": "儲位1", "Value": "some data" },
        ///     { "IP": "192.168.0.11", "Name": "儲位2", "Value": "some data" }
        ///   ],
        ///   "TimeTaken": "0.115 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (失敗)：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "找無Server資料",
        ///   "Data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，Data 可為 null，僅需提供 <c>ServerName</c> 與 <c>ServerType</c>。
        /// </param>
        /// <returns>
        /// [returnData.Data] 為 <c>Storage</c> 陣列結構。
        /// </returns>
        [Route("get_epd266_storage")]
        [HttpPost]
        public string get_epd266_storage(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_epd266_storage";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                string tableName = "epd266_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<Storage> storages = StorageMethod.SQL_GetAllStorage(sQLControl_device);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = storages;
                returnData.Result = $"storages 取得成功!,共<{storages.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }

        /// <summary>
        /// 依據藥碼取得儲位資料 (EPD266)
        /// </summary>
        /// <remarks>
        /// 此 API 用於查詢 EPD266 顯示裝置的儲位資料，會依據傳入的 <c>藥碼 (Code)</c> 回傳對應的 <c>Storage</c> 陣列。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 <c>ValueAry</c> 不可為空，且必須只包含一個藥碼。  
        /// 2. 依據 <c>ServerName</c>、<c>ServerType</c> 取得資料庫連線資訊。  
        /// 3. 查詢 <c>epd266_jsonstring</c> 資料表，依據傳入的藥碼取得對應的 Storage 清單。  
        /// 4. 若找到資料，回傳 <c>Storage</c> 陣列；若查無資料，回傳錯誤訊息。  
        ///
        /// <b>注意事項：</b>
        /// - <c>ValueAry</c> 必須只包含一個藥碼。  
        /// - 若藥碼在資料庫中不存在，則回傳錯誤訊息。  
        /// - 此 API 可能回傳多筆資料，因為同一藥碼可能存在於多個儲位。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [ "18217" ],
        ///   "Data": null
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (成功)：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "已取得資料共2筆!,TableName : epd266_jsonstring , Code : 18217",
        ///   "Data": [
        ///     { "IP": "192.168.0.10", "Code": "18217", "Name": "儲位1", "Value": "some data" },
        ///     { "IP": "192.168.0.11", "Code": "18217", "Name": "儲位2", "Value": "some data" }
        ///   ],
        ///   "TimeTaken": "0.134 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (失敗)：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "查無資料",
        ///   "Data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，ValueAry 必須包含一個藥碼 (Code)，Data 可為 null。
        /// </param>
        /// <returns>
        /// [returnData.Data] 為符合條件的 <c>Storage</c> 陣列結構。
        /// </returns>
        [Route("get_epd266_storage_by_code")]
        [HttpPost]
        public string get_epd266_storage_by_code(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_epd266_storage_By_Code";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼]";
                    return returnData.JsonSerializationt(true);
                }
                string Code = returnData.ValueAry[0];
                string tableName = "epd266_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<Storage> storages = StorageMethod.SQL_GetStorageByCode(sQLControl_device, Code);
                if (storages == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = storages;
                returnData.Result = $"已取得資料共<{storages.Count}>筆!,TableName : {returnData.TableName} , Code : {Code}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 依據藥碼取得裝置基礎資料 (EPD266)
        /// </summary>
        /// <remarks>
        /// 此 API 用於查詢 EPD266 顯示裝置的基礎資訊，會依據傳入的 <c>藥碼 (Code)</c> 回傳對應的 <c>DeviceBasic</c> 陣列。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 <c>ValueAry</c> 不可為空，且必須只包含一個藥碼。  
        /// 2. 依據 <c>ServerName</c>、<c>ServerType</c> 取得資料庫連線資訊。  
        /// 3. 查詢 <c>epd266_jsonstring</c> 資料表，依據傳入的藥碼取得對應的 DeviceBasic 清單。  
        /// 4. 若找到資料，回傳 <c>DeviceBasic</c> 陣列；若查無資料，回傳錯誤訊息。  
        ///
        /// <b>注意事項：</b>
        /// - <c>ValueAry</c> 必須只包含一個藥碼。  
        /// - 若藥碼在資料庫中不存在，則回傳錯誤訊息。  
        /// - 此 API 可能回傳多筆資料，因為同一藥碼可能存在於多個裝置。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [ "18217" ],
        ///   "Data": null
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (成功)：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "已取得資料共2筆!,TableName : epd266_jsonstring , Code : 18217",
        ///   "Data": [
        ///     { "IP": "192.168.0.10", "Code": "18217", "Name": "裝置1", "Value": "some data" },
        ///     { "IP": "192.168.0.11", "Code": "18217", "Name": "裝置2", "Value": "some data" }
        ///   ],
        ///   "TimeTaken": "0.142 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (失敗)：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "查無資料",
        ///   "Data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，ValueAry 必須包含一個藥碼 (Code)，Data 可為 null。
        /// </param>
        /// <returns>
        /// [returnData.Data] 為符合條件的 <c>DeviceBasic</c> 陣列結構。
        /// </returns>
        [Route("get_epd266_deviceBasics_by_code")]
        [HttpPost]
        public string get_epd266_deviceBasics_by_code(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_epd266_storage_By_Code";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼]";
                    return returnData.JsonSerializationt(true);
                }
                string Code = returnData.ValueAry[0];
                string tableName = "epd266_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<DeviceBasic> deviceBasics = StorageMethod.GetDeviceBasicByCode(sQLControl_device, Code);
                if (deviceBasics == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = deviceBasics;
                returnData.Result = $"已取得資料共<{deviceBasics.Count}>筆!,TableName : {returnData.TableName} , Code : {Code}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 依據 IP 取得指定的儲位資料 (EPD266)
        /// </summary>
        /// <remarks>
        /// 此 API 用於查詢 EPD266 顯示裝置的儲位資料，會依據傳入的 <c>IP</c> 回傳對應的 <c>Storage</c>。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 <c>ValueAry</c> 不可為空，且必須只包含一個 <c>IP</c>。  
        /// 2. 依據 <c>ServerName</c>、<c>ServerType</c> 取得資料庫連線資訊。  
        /// 3. 查詢 <c>epd266_jsonstring</c> 資料表，依據傳入的 IP 取得對應的 Storage。  
        /// 4. 若找到資料，回傳 Storage 結構；若查無資料，回傳錯誤訊息。  
        ///
        /// <b>注意事項：</b>
        /// - <c>ValueAry</c> 必須只包含一個 <c>IP</c>。  
        /// - 若 IP 在資料庫中不存在，則回傳錯誤訊息。  
        /// - 此 API 僅支援單一 IP 查詢。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [ "192.168.0.10" ],
        ///   "Data": null
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (成功)：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "jsonStr取得成功!,TableName : epd266_jsonstring , IP : 192.168.0.10",
        ///   "Data": {
        ///     "IP": "192.168.0.10",
        ///     "Code": "18217",
        ///     "Name": "儲位1",
        ///     "Value": "some data"
        ///   },
        ///   "TimeTaken": "0.098 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (失敗)：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "查無資料",
        ///   "Data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，ValueAry 必須包含一個 <c>IP</c>，Data 可為 null。
        /// </param>
        /// <returns>
        /// [returnData.Data] 為對應的 <c>Storage</c> 結構。
        /// </returns>
        [Route("get_epd266_storage_ByIP")]
        [HttpPost]
        public string get_epd266_storage_ByIP(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_epd266_storage_ByIP";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[IP]";
                    return returnData.JsonSerializationt(true);
                }
                string IP = returnData.ValueAry[0];
                string tableName = "epd266_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                Storage storage = StorageMethod.SQL_GetStorageByIP(sQLControl_device, IP);
                if (storage == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = storage;
                returnData.Result = $"jsonStr取得成功!,TableName : {returnData.TableName} , IP : {IP}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 更新儲位資料 (EPD266)
        /// </summary>
        /// <remarks>
        /// 此 API 用於更新 EPD266 顯示裝置的儲位資料，會依據 <c>IP</c> 進行比對並更新對應的資料。  
        ///
        /// <b>邏輯流程：</b>
        /// 1. 驗證 Data 欄位不可為空，且必須為 <c>Storage</c> 陣列。  
        /// 2. 依據傳入資料的 <c>IP</c>，比對資料庫中的既有儲位紀錄。  
        /// 3. 若 IP 存在，則更新該筆紀錄的 JSON 值。  
        /// 4. 批次更新資料庫，完成後回傳更新結果。  
        ///
        /// <b>注意事項：</b>
        /// - Data 必須為 <c>Storage</c> 陣列格式。  
        /// - 系統會依據 <c>IP</c> 進行比對，若 IP 在資料庫不存在則不會新增。  
        /// - 僅更新傳入有對應 IP 的紀錄，其餘資料保持不變。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "口服2",
        ///   "ServerType": "調劑台",
        ///   "Data": [
        ///     { "IP": "192.168.0.10", "Name": "儲位1", "Value": "some data" },
        ///     { "IP": "192.168.0.11", "Name": "儲位2", "Value": "some data" }
        ///   ]
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (成功)：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "更新storages成功!,共2筆資料,TableName : epd266_jsonstring",
        ///   "Data": null,
        ///   "TimeTaken": "0.134 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 回應範例 (失敗)：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "傳入Data無資料",
        ///   "Data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，Data 必須為 <c>Storage</c> 陣列，且包含 <c>IP</c> 欄位。
        /// </param>
        /// <returns>
        /// [returnData.Result] 為更新結果，[returnData.Code] 為狀態碼，[returnData.Data] 為處理後資料。
        /// </returns>
        [Route("update_epd266_storages")]
        [HttpPost]
        public string update_epd266_storages(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            //returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            returnData.Method = "update_epd266_storages";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string tableName = "epd266_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                List<Storage> storages = returnData.Data.ObjToClass<List<Storage>>();
                if (storages.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data無資料";
                    return returnData.JsonSerializationt();
                }
                if (storages == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料錯誤";
                    return returnData.JsonSerializationt();
                }
                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_device.GetAllRows(null);
                List<object[]> list_value_buf = new List<object[]>();
                List<object[]> list_replace = new List<object[]>();
                for (int i = 0; i < storages.Count; i++)
                {
                    string IP = storages[i].IP;
                    list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, IP);
                    if (list_value_buf.Count > 0)
                    {
                        list_value_buf[0][(int)enum_DeviceTable.Value] = storages[i].JsonSerializationt();
                        list_replace.Add(list_value_buf[0]);
                    }
                }

                sQLControl_device.UpdateByDefulteExtra(null, list_replace);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = $"更新storages成功!,共<{list_replace.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }


        }
        /// <summary>
        /// 更新 EPD266 儲位藥碼 (update_epd266_medcode)
        /// </summary>
        /// <remarks>
        /// 此 API 用於更新指定 EPD266 儲位的藥品資訊，會依據 <c>ValueAry</c> 內傳入的 <c>code</c> 更新 Storage 內容：  
        /// - 透過藥碼 code 查詢雲端藥檔資料。  
        /// - 更新目標 Storage 的藥品屬性 (Name、Code、其他屬性)。  
        ///
        /// <b>邏輯流程：</b>  
        /// 1. 驗證 Server 設定是否存在。  
        /// 2. 驗證 Data 必須為 <c>Storage</c> 結構。  
        /// 3. 驗證 <c>ValueAry</c> 必須包含 <c>code</c>。  
        /// 4. 呼叫 <c>medClass.get_med_clouds_by_code</c> 查詢雲端藥檔。  
        /// 5. 更新 Storage 的藥品資訊。  
        /// 6. 回傳更新後的 Storage。  
        ///
        /// <b>注意事項：</b>  
        /// - <c>code</c> 為必填欄位，格式為「code=藥碼」。  
        /// - 若傳入的藥碼無法在雲端藥檔中找到，將回傳錯誤。  
        /// - Data 必須為有效的 <c>Storage</c> 物件。  
        ///
        /// <b>JSON 請求範例：</b>
        /// ```json
        /// {
        ///   "ServerName": "Main",
        ///   "ServerType": "網頁",
        ///   "ValueAry": [
        ///     "code=MED-00123"
        ///   ],
        ///   "Data": {
        ///     "StorageID": "EPD266-01",
        ///     "Name": "EPD266 儲位 A1",
        ///     "MedCode": "",
        ///     "MedName": ""
        ///   }
        /// }
        /// ```
        ///
        /// <b>JSON 成功回應範例：</b>
        /// ```json
        /// {
        ///   "Code": 200,
        ///   "Result": "更新medcode成功!,TableName : epd266_jsonstring",
        ///   "Data": {
        ///     "StorageID": "EPD266-01",
        ///     "Name": "EPD266 儲位 A1",
        ///     "MedCode": "MED-00123",
        ///     "MedName": "普拿疼錠劑 500mg"
        ///   },
        ///   "TimeTaken": "0.092 秒"
        /// }
        /// ```
        ///
        /// <b>JSON 錯誤回應範例：</b>
        /// ```json
        /// {
        ///   "Code": -200,
        ///   "Result": "傳入ValueAry無code資料"
        /// }
        /// ```
        /// </remarks>
        /// <param name="returnData">
        /// 共用傳遞資料結構，Data 必須包含 Storage，ValueAry 必須有「code=...」
        /// </param>
        /// <returns>
        /// [returnData.Data] 會回傳更新後的 Storage 結構
        /// </returns>
        [Route("update_epd266_medcode")]
        [HttpPost]
        public string update_epd266_medcode(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            //returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            returnData.Method = "update_epd266_medcode";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string tableName = "epd266_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                Storage storage = returnData.Data.ObjToClass<Storage>();
          
                if (storage == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料錯誤";
                    return returnData.JsonSerializationt();
                }

                string code = returnData.ValueAry.GetVal("code", "");

                if (code == "")
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入ValueAry無code資料";
                    return returnData.JsonSerializationt();
                }
                medClass medClass = medClass.get_med_clouds_by_code(API_Server, code);
                if(medClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無藥碼資料";
                    return returnData.JsonSerializationt();
                }
                storage.SetMedClass(medClass);
                          
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = storage;
                returnData.Result = $"更新medcode成功!,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }


        }

        /// <summary>
        /// 取得儲位資料(Panel35)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "口服2",
        ///     "ServerType": "調劑台",
        ///     "TableName" : "",
        ///     "ValueAry" : 
        ///     [
        ///       
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[Device]陣列結構</returns>
        [Route("get_panel35_storage")]
        [HttpPost]
        public string get_panel35_storage(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_panel35_storage";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                string tableName = "wt32_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<Storage> storages = StorageMethod.SQL_GetAllStorage(sQLControl_device);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = storages;
                returnData.Result = $"storages 取得成功!,共<{storages.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 以藥碼取得儲位資料(Panel35)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "口服2",
        ///     "ServerType": "調劑台",
        ///     "TableName" : "",
        ///     "ValueAry" : 
        ///     [
        ///        [藥碼]
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[Device]陣列結構</returns>
        [Route("get_Panel35_storage_By_Code")]
        [HttpPost]
        public string get_Panel35_storage_By_Code(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_Panel35_storage_By_Code";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼]";
                    return returnData.JsonSerializationt(true);
                }
                string Code = returnData.ValueAry[0];
                string tableName = "Panel35_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<Storage> storages = StorageMethod.SQL_GetStorageByCode(sQLControl_device, Code);
                if (storages == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = storages;
                returnData.Result = $"已取得資料共<{storages.Count}>筆!,TableName : {returnData.TableName} , Code : {Code}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 以藥碼取得儲位資料(Panel35.DeviceBasics)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "口服2",
        ///     "ServerType": "調劑台",
        ///     "TableName" : "",
        ///     "ValueAry" : 
        ///     [
        ///        [藥碼]
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[Device]陣列結構</returns>
        [Route("get_Panel35_DeviceBasics_By_Code")]
        [HttpPost]
        public string get_Panel35_DeviceBasics_By_Code(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_Panel35_storage_By_Code";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼]";
                    return returnData.JsonSerializationt(true);
                }
                string Code = returnData.ValueAry[0];
                string tableName = "Panel35_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<DeviceBasic> deviceBasics = StorageMethod.GetDeviceBasicByCode(sQLControl_device, Code);
                if (deviceBasics == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = deviceBasics;
                returnData.Result = $"已取得資料共<{deviceBasics.Count}>筆!,TableName : {returnData.TableName} , Code : {Code}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 以IP取得儲位資料(Panel35)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "口服2",
        ///     "ServerType": "調劑台",
        ///     "TableName" : "",
        ///     "ValueAry" : 
        ///     [
        ///       
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[Device]陣列結構</returns>
        [Route("get_panel35_storage_ByIP")]
        [HttpPost]
        public string get_panel35_storage_ByIP(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            returnData.Method = "get_panel35_storage_ByIP";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[IP]";
                    return returnData.JsonSerializationt(true);
                }
                string IP = returnData.ValueAry[0];
                string tableName = "wt32_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                Storage storage = StorageMethod.SQL_GetStorageByIP(sQLControl_device, IP);
                if (storage == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt();
                }
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Data = storage;
                returnData.Result = $"jsonStr取得成功!,TableName : {returnData.TableName} , IP : {IP}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }
        /// <summary>
        /// 更新儲位資料(Panel35)
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "口服2",
        ///     "ServerType": "調劑台",
        ///     "Data" : 
        ///     {
        ///       [rowsLED陣列]
        ///     }
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[DeviceBasic]陣列結構</returns>
        [Route("update_panel35_storages")]
        [HttpPost]
        public string update_panel35_storages(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            //returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            returnData.Method = "update_panel35_storages";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");

                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }
                string tableName = "wt32_jsonstring";
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                List<Storage> storages = returnData.Data.ObjToClass<List<Storage>>();
                if (storages.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data無資料";
                    return returnData.JsonSerializationt();
                }
                if (storages == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料錯誤";
                    return returnData.JsonSerializationt();
                }
                SQLControl sQLControl_device = new SQLControl(Server, DB, tableName, UserName, Password, Port, SSLMode);
                List<object[]> list_value = sQLControl_device.GetAllRows(null);
                List<object[]> list_value_buf = new List<object[]>();
                List<object[]> list_replace = new List<object[]>();
                for (int i = 0; i < storages.Count; i++)
                {
                    string IP = storages[i].IP;
                    list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, IP);
                    if (list_value_buf.Count > 0)
                    {
                        list_value_buf[0][(int)enum_DeviceTable.Value] = storages[i].JsonSerializationt();
                        list_replace.Add(list_value_buf[0]);
                    }
                }

                sQLControl_device.UpdateByDefulteExtra(null, list_replace);
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = $"更新storages成功!,共<{list_replace.Count}>筆資料,TableName : {returnData.TableName}";

                string json_out = returnData.JsonSerializationt();

                return json_out;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Value = $"{e.Message}";
                return returnData.JsonSerializationt();
            }
        }

        /// <summary>
        /// 以藥碼進行亮燈
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName": "ds01",
        ///     "ServerType": "藥庫",
        ///     "ValueAry" : 
        ///     [
        ///       [藥碼1,藥碼2....],
        ///       [顏色],
        ///       [亮燈時間(s)]
        ///     ]
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Route("light_on_by_code")]
        [HttpPost]
        public string light_on_by_code(returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "light_on_by_code";
            try
            {
                List<object[]> list_add = new List<object[]>();
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = new List<sys_serverSettingClass>();
                sys_serverSettingClass sys_serverSettingClass_儲位資料 = null;
                sys_serverSettingClass sys_serverSettingClass_堆疊資料 = null;
                sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass_儲位資料 = sys_serverSettingClasses_buf[0];

                sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "一般資料");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass_堆疊資料 = sys_serverSettingClasses_buf[0];

                if (returnData.ValueAry.Count != 3)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼1,藥碼2....][顏色][亮燈時間(s)]";
                    return returnData.JsonSerializationt(true);
                }
                
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                List<Task> tasks = new List<Task>();
                string str_codes = returnData.ValueAry[0];
                string str_color = returnData.ValueAry[1];
                string str_time = returnData.ValueAry[2];
                if (str_time.StringIsDouble() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"亮燈時間(s) 輸入內容錯誤";
                    return returnData.JsonSerializationt(true);
                }
                string Server = sys_serverSettingClass_儲位資料.Server;
                string DB = sys_serverSettingClass_儲位資料.DBName;
                string UserName = sys_serverSettingClass_儲位資料.User;
                string Password = sys_serverSettingClass_儲位資料.Password;
                uint Port = (uint)sys_serverSettingClass_儲位資料.Port.StringToInt32();

                string[] codes = str_codes.Split(",");

                if (codes.Length == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料無可用藥碼";
                    return returnData.JsonSerializationt(true);
                }
           

                Color color = str_color.ToColor();
                for (int i = 0; i < codes.Length; i++)
                {
                    object[] value = new object[new enum_取藥堆疊母資料().GetLength()];
                    value[(int)enum_取藥堆疊母資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_取藥堆疊母資料.顏色] = color.ToColorString();
                    value[(int)enum_取藥堆疊母資料.藥品碼] = codes[i];
                    value[(int)enum_取藥堆疊母資料.調劑台名稱] = "儲位亮燈";
                    value[(int)enum_取藥堆疊母資料.狀態] = "None";
                    value[(int)enum_取藥堆疊母資料.總異動量] = str_time;
                    value[(int)enum_取藥堆疊母資料.操作時間] = DateTime.Now.ToDateTimeString();
                    list_add.Add(value);
                }
                SQLControl sQLControl_take_medicine_stack_new = new SQLControl(sys_serverSettingClass_堆疊資料.Server, sys_serverSettingClass_堆疊資料.DBName, "take_medicine_stack_new", 
                    sys_serverSettingClass_堆疊資料.User, sys_serverSettingClass_堆疊資料.Password, sys_serverSettingClass_堆疊資料.Port.StringToUInt32(), SSLMode);

                sQLControl_take_medicine_stack_new.AddRows(null, list_add);

                returnData.Code = 200;
                returnData.Result = $"設備更新亮燈完成,共<{codes.Length}>筆";
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt();

            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                returnData.TimeTaken = myTimer.ToString();
                returnData.Data = null;
                return returnData.JsonSerializationt(true);
            }
            finally
            {

            }
        }
        /// <summary>
        /// 以藥碼對應設備進行亮燈
        /// </summary>
        /// <remarks>
        /// 請求 JSON 範例如下：
        /// <code>
        /// {
        ///   "ServerName": "A6",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [
        ///     ["藥碼", "顏色", "亮燈時間(s)"],
        ///     ["A123", "#FF0000", "5"],
        ///     ["B456", "#00FF00", "3"]
        ///   ]
        /// }
        /// </code>
        /// 系統將根據藥碼查詢對應的設備 IP，自動進行亮燈處理。
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構，包含 ServerName、ServerType 與 ValueAry</param>
        /// <returns>處理結果 JSON 字串，包含回應代碼、訊息與耗時</returns>
        [Route("light_by_drugCodes")]
        [HttpPost]
        public string light_by_drugCodes(returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "light_by_deviceBasics";
            TimeLogHelper timer = new TimeLogHelper();

            try
            {
                timer.Tick("啟動");
                List<object[]> list_add = new List<object[]>();

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                timer.Tick("取得Server設定");

                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!\n⏱ {timer.GetResult()}";
                    return returnData.JsonSerializationt();
                }

                string device_Server = sys_serverSettingClasses[0].Server;
                string device_DB = sys_serverSettingClasses[0].DBName;
                string port = sys_serverSettingClasses[0].Port;
                string username = sys_serverSettingClasses[0].User;
                string pwd = sys_serverSettingClasses[0].Password;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                List<string[]> valueAry = returnData.ValueAry.Select(v => v.JsonDeserializet<string[]>()).ToList();
                timer.Tick("資料解析完成");

                foreach (var item in valueAry)
                {
                    if (item.Length < 3) continue;

                    string code = item[0];
                    string colorStr = item[1];
                    string seconds = item[2];

                    Color color = colorStr.ToColor();

                    object[] value = new object[new enum_取藥堆疊母資料().GetLength()];
                    value[(int)enum_取藥堆疊母資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_取藥堆疊母資料.藥品碼] = code;
                    value[(int)enum_取藥堆疊母資料.顏色] = color.ToColorString();
                    value[(int)enum_取藥堆疊母資料.調劑台名稱] = "儲位亮燈";
                    value[(int)enum_取藥堆疊母資料.總異動量] = seconds;
                    value[(int)enum_取藥堆疊母資料.狀態] = "None";
                    value[(int)enum_取藥堆疊母資料.操作時間] = DateTime.Now.ToDateTimeString();
                    list_add.Add(value);
                }
                timer.Tick("轉換為SQL資料完成");

                SQLControl sQLControl_take_medicine_stack_new = new SQLControl(device_Server, device_DB, "take_medicine_stack_new", username, pwd, port.StringToUInt32(), SSLMode);
                sQLControl_take_medicine_stack_new.AddRows(null, list_add);
                timer.Tick("SQL寫入完成");

                returnData.Code = 200;
                returnData.Result = $"設備更新亮燈完成！共更新 {list_add.Count} 筆\n⏱ {timer.GetResult()} (Total: {timer.Total})";
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                timer.Tick("例外處理");
                returnData.Code = -200;
                returnData.Result = $"{e.Message}\n⏱ {timer.GetResult()} (Total: {timer.Total})";
                returnData.TimeTaken = myTimer.ToString();
                returnData.Data = null;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以 IP 清單進行面板刷新
        /// </summary>
        /// <remarks>
        /// 請求 JSON 範例如下：
        /// <code>
        /// {
        ///   "ServerName": "A6",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [
        ///     ["192.168.0.101"],
        ///     ["192.168.0.102"]
        ///   ]
        /// }
        /// </code>
        /// 系統會根據傳入的 IP 清單，建立對應的面板刷新資料。
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構，包含 ServerName、ServerType 與 IP 陣列</param>
        /// <returns>JSON 回傳字串，包含代碼、訊息與耗時</returns>
        [Route("refresh_canvas_by_ip")]
        [HttpPost]
        public string refresh_canvas_by_ip(returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "refresh_canvas_by_ip";
            TimeLogHelper timer = new TimeLogHelper();

            try
            {
                timer.Tick("啟動");

                List<object[]> list_add = new List<object[]>();

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                timer.Tick("取得Server設定");

                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!\n⏱ {timer.GetResult()}";
                    return returnData.JsonSerializationt();
                }

                string device_Server = sys_serverSettingClasses[0].Server;
                string device_DB = sys_serverSettingClasses[0].DBName;
                string port = sys_serverSettingClasses[0].Port;
                string username = sys_serverSettingClasses[0].User;
                string pwd = sys_serverSettingClasses[0].Password;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // 從 ValueAry 取得 IP 列表
                List<string[]> ipList = returnData.ValueAry.Select(v => v.JsonDeserializet<string[]>()).ToList();
                timer.Tick("資料解析完成");

                foreach (var item in ipList)
                {
                    if (item.Length < 1) continue;

                    string ip = item[0];
                    object[] value = new object[new enum_取藥堆疊母資料().GetLength()];
                    value[(int)enum_取藥堆疊母資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_取藥堆疊母資料.IP] = ip;
                    value[(int)enum_取藥堆疊母資料.藥品碼] = "None";
                    value[(int)enum_取藥堆疊母資料.調劑台名稱] = "刷新面板";
                    value[(int)enum_取藥堆疊母資料.狀態] = "None";
                    value[(int)enum_取藥堆疊母資料.總異動量] = "1";
                    value[(int)enum_取藥堆疊母資料.操作時間] = DateTime.Now.ToDateTimeString();
                    list_add.Add(value);
                }
                timer.Tick("轉換為SQL資料完成");

                SQLControl sQLControl_take_medicine_stack_new = new SQLControl(device_Server, device_DB, "take_medicine_stack_new", username, pwd, port.StringToUInt32(), SSLMode);
                sQLControl_take_medicine_stack_new.AddRows(null, list_add);
                timer.Tick("SQL寫入完成");

                returnData.Code = 200;
                returnData.Result = $"設備刷新上傳完成！共更新 {list_add.Count} 筆\n⏱ {timer.GetResult()} (Total: {timer.Total})";
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                timer.Tick("例外處理");
                returnData.Code = -200;
                returnData.Result = $"{e.Message}\n⏱ {timer.GetResult()} (Total: {timer.Total})";
                returnData.TimeTaken = myTimer.ToString();
                returnData.Data = null;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以藥碼清單進行面板刷新
        /// </summary>
        /// <remarks>
        /// 請求 JSON 範例如下：
        /// <code>
        /// {
        ///   "ServerName": "A6",
        ///   "ServerType": "調劑台",
        ///   "ValueAry": [
        ///     ["A123", "2"],
        ///     ["B456", "0"]
        ///   ]
        /// }
        /// </code>
        /// 系統會根據傳入藥碼查詢設備 IP，並寫入刷新資料，延遲時間以秒為單位。
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構，包含藥碼與延遲秒數</param>
        /// <returns>JSON 回傳字串，包含代碼、訊息與耗時</returns>
        [Route("refresh_canvas_by_drugCodes")]
        [HttpPost]
        public string refresh_canvas_by_drugCodes(returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "refresh_canvas_by_drugCodes";
            TimeLogHelper timer = new TimeLogHelper();

            try
            {
                timer.Tick("啟動");

                List<object[]> list_add = new List<object[]>();
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                timer.Tick("取得Server設定");

                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "本地端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!\n⏱ {timer.GetResult()}";
                    return returnData.JsonSerializationt();
                }

                string device_Server = sys_serverSettingClasses[0].Server;
                string device_DB = sys_serverSettingClasses[0].DBName;
                string port = sys_serverSettingClasses[0].Port;
                string username = sys_serverSettingClasses[0].User;
                string pwd = sys_serverSettingClasses[0].Password;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                List<string[]> inputList = returnData.ValueAry.Select(v => v.JsonDeserializet<string[]>()).ToList();
                timer.Tick("資料解析完成");

                foreach (var item in inputList)
                {
                    if (item.Length < 1) continue;

                    string drugCode = item[0];
                    string delaySec = item.Length >= 2 ? item[1] : "0";



                    object[] value = new object[new enum_取藥堆疊母資料().GetLength()];
                    value[(int)enum_取藥堆疊母資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_取藥堆疊母資料.藥品碼] = drugCode;
                    value[(int)enum_取藥堆疊母資料.調劑台名稱] = "刷新面板";
                    value[(int)enum_取藥堆疊母資料.操作時間] = DateTime.Now.ToDateTimeString();
                    value[(int)enum_取藥堆疊母資料.狀態] = "None";
                    value[(int)enum_取藥堆疊母資料.總異動量] = delaySec; // 儲存延遲秒數

                    list_add.Add(value);
                }
                timer.Tick("轉換為SQL資料完成");

                SQLControl sQLControl_take_medicine_stack_new = new SQLControl(device_Server, device_DB, "take_medicine_stack_new", username, pwd, port.StringToUInt32(), SSLMode);
                sQLControl_take_medicine_stack_new.AddRows(null, list_add);
                timer.Tick("SQL寫入完成");

                returnData.Code = 200;
                returnData.Result = $"面板刷新資料上傳完成！共更新 {list_add.Count} 筆\n⏱ {timer.GetResult()} (Total: {timer.Total})";
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                timer.Tick("例外處理");
                returnData.Code = -200;
                returnData.Result = $"{e.Message}\n⏱ {timer.GetResult()} (Total: {timer.Total})";
                returnData.TimeTaken = myTimer.ToString();
                returnData.Data = null;
                return returnData.JsonSerializationt(true);
            }
        }


        [Route("light")]
        [HttpPost]
        public string light(returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "light";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                string device_Server = sys_serverSettingClasses[0].Server;
                string device_DB = sys_serverSettingClasses[0].DBName;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Color color = returnData.Value.ToColor();
                string json_in = returnData.Data.JsonSerializationt();
                List<DeviceBasic> deviceBasics = json_in.JsonDeserializet<List<DeviceBasic>>();
                for (int i = 0; i < deviceBasics.Count; i++)
                {
                    DeviceType deviceType = deviceBasics[i].DeviceType;
                    if (deviceType == DeviceType.EPD290 || deviceType == DeviceType.EPD290_lock)
                    {
                        H_Pannel_lib.Communication.Set_WS2812_Buffer(Startup.uDP_Class, deviceBasics[i].IP, 0, Get_EPD290_LEDBytes(color));
                    }
                    if (deviceType == DeviceType.EPD266 || deviceType == DeviceType.EPD266_lock)
                    {
                      
                        H_Pannel_lib.Communication.Set_WS2812_Buffer(Startup.uDP_Class, deviceBasics[i].IP, 0, Get_EPD266_LEDBytes(color));
                    }
                    if (deviceType == DeviceType.RowsLED)
                    {
                        SQLControl sQLControl_RowsLED_serialize = new SQLControl(device_Server, device_DB, "rowsled_jsonstring", UserName, Password, Port, SSLMode);
                        RowsLED rowsLED = RowsLEDMethod.SQL_GetDevice(sQLControl_RowsLED_serialize, deviceBasics[i].IP);
                        if (rowsLED == null) continue;
                        RowsDevice rowsDevice = rowsLED.SortByGUID(deviceBasics[i].GUID);
                        if (rowsDevice == null) continue;
                        int maxLED = rowsLED.Maximum;
                        byte[] LED_Bytes = new byte[maxLED];
                        Get_Rows_LEDBytes(ref LED_Bytes, rowsDevice.StartLED, rowsDevice.EndLED, color);
                        H_Pannel_lib.Communication.Set_WS2812_Buffer(Startup.uDP_Class, deviceBasics[i].IP, 0, LED_Bytes);

                    }
                    if (deviceType == DeviceType.EPD583 || deviceType == DeviceType.EPD583_lock)
                    {
                        SQLControl sQLControl_EPD583_serialize = new SQLControl(device_Server, device_DB, "epd583_jsonstring", UserName, Password, Port, SSLMode);
                        Drawer drawer = DrawerMethod.SQL_GetDevice(sQLControl_EPD583_serialize, deviceBasics[i].IP);
                        if (drawer == null) continue;
                        Box box = drawer.GetBox(deviceBasics[i].GUID);
                        if (box == null) continue;
                        WS2812_Init();
                        Set_LED_UDP(Startup.uDP_Class, drawer, box, color);

                    }
                }
                returnData.Code = 200;
                returnData.Result = $"設備亮燈完成!";
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt();

            }
            catch(Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt();
            }
            finally
            {

            }
        }

       

        [Route("sort_by_ip")]
        [HttpPost]
        public string sort_by_ip(returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "sort_by_ip";
            try
            {
                List<object[]> list_add = new List<object[]>();
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                string Storage_IP = "";
                Storage_IP = $"192.168.{returnData.Value}";
                if($"192.168.{returnData.Value}".Check_IP_Adress() == true)
                {
                    Storage_IP = $"192.168.{returnData.Value}";
                }
                if ($"{returnData.Value}".Check_IP_Adress() == true)
                {
                    Storage_IP = $"{returnData.Value}";
                }
                if(Storage_IP.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = $"IP 值無效! {returnData.Value}";
                    return returnData.JsonSerializationt();
                }
             

                string device_Server = sys_serverSettingClasses[0].Server;
                string device_DB = sys_serverSettingClasses[0].DBName;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Color color = returnData.Value.ToColor();
                string json_in = returnData.Data.JsonSerializationt();
                List<DeviceBasic> deviceBasics = Function_Get_device_by_ip(sys_serverSettingClasses[0], Storage_IP);
                returnData.Data = deviceBasics;

                returnData.Code = 200;
                returnData.Result = $"搜尋儲位IP完成,共<{deviceBasics.Count}>筆!";
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt();

            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                returnData.TimeTaken = myTimer.ToString();
                returnData.Data = null;
                return returnData.JsonSerializationt(true);
            }
            finally
            {

            }
        }
        [HttpPost("get_list_by_department")]
        public string get_list_by_department([FromBody]returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
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
                    returnData.Result = $"returnData.ValueAry 內容應為[Type]";
                    return returnData.JsonSerializationt(true);
                }


                List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClass.get_serversetting_by_department_type(API_Server, returnData.ValueAry[0]);
                if (sys_serverSettingClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"get_serversetting_by_type 回傳為空";
                    return returnData.JsonSerializationt(true);
                }
                List<Task> tasks = new List<Task>();
                List<DeviceBasic> deviceBasics_total = new List<DeviceBasic>();
                foreach (var item in sys_serverSettingClasses)
                {                    
                    string 調劑台 = item.設備名稱;
                    string 藥品清單 = GET_list(調劑台);
                    returnData returnData_get_list = 藥品清單.JsonDeserializet<returnData>();
                    List<DeviceBasic> deviceBasics = returnData_get_list.Data.ObjToClass<List<DeviceBasic>>();
                    if(deviceBasics != null)
                    {
                        for (int i = 0; i < deviceBasics.Count; i++)
                        {
                            deviceBasics[i].Area = 調劑台;
                        }
                        deviceBasics_total.AddRange(deviceBasics);
                    }                                          
                }
                returnData.Code = 200;
                returnData.Result = $"取得單位:{returnData.ValueAry[0]}內所有資料，共{deviceBasics_total.Count}筆";
                returnData.Data = deviceBasics_total;
                returnData.TimeTaken = myTimerBasic.ToString();
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"{ex.Message}";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = null;
                return returnData.JsonSerializationt(true);
            }
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        static public List<DeviceBasic> Function_Get_device()
        {
            return Function_Get_device(device_Server, device_DB, device_TableName, UserName, Password, Port);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public List<DeviceBasic> Function_Get_device(returnData returnData)
        {
            return Function_Get_device(returnData.Server, returnData.DbName, returnData.TableName, returnData.UserName, returnData.Password, returnData.Port);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public List<DeviceBasic> Function_Get_device(sys_serverSettingClass sys_serverSettingClass)
        {
            return Function_Get_device(sys_serverSettingClass, "");
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public List<DeviceBasic> Function_Get_device(sys_serverSettingClass sys_serverSettingClass , string TableName)
        {
            string Server = sys_serverSettingClass.Server;
            string DBName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            if (sys_serverSettingClass.類別 == "藥庫" && (TableName == "firstclass_device_jsonstring" || TableName == "sd0_device_jsonstring"))
            {
                SQLControl sQLControl_device = new SQLControl(Server, DBName, TableName, UserName, Password, Port, SSLMode);
                deviceBasics = DeviceBasicMethod.SQL_GetAllDeviceBasic(sQLControl_device);

            }
            else
            {
                deviceBasics = Function_讀取儲位(Server, DBName, UserName, Password, Port);
            }
       
            return deviceBasics;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public List<DeviceSimple> Function_Get_deviceSimple(sys_serverSettingClass sys_serverSettingClass, string TableName)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string Server = sys_serverSettingClass.Server;
            string DBName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();
            List<DeviceSimple> deviceBasics = new List<DeviceSimple>();
            if (sys_serverSettingClass.類別 == "藥庫" && (TableName == "firstclass_device_jsonstring" || TableName == "sd0_device_jsonstring"))
            {
                SQLControl sQLControl_device = new SQLControl(Server, DBName, TableName, UserName, Password, Port, SSLMode);
                deviceBasics = DeviceBasicMethod.SQL_GetAllDeviceSimple(sQLControl_device);
                //Console.WriteLine($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] 類別 : {sys_serverSettingClass.類別} , TableName {TableName} , {myTimerBasic.ToString()}");
            }
            return deviceBasics;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public void Function_Set_device(sys_serverSettingClass sys_serverSettingClass, string TableName , List<DeviceBasic> deviceBasics)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            string Server = sys_serverSettingClass.Server;
            string DBName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();
            if (sys_serverSettingClass.類別 == "藥庫" && (TableName == "firstclass_device_jsonstring" || TableName == "sd0_device_jsonstring"))
            {
                SQLControl sQLControl_device = new SQLControl(Server, DBName, TableName, UserName, Password, Port, SSLMode);
                DeviceBasicMethod.SQL_ReplaceDeviceBasic(sQLControl_device , deviceBasics);
                //Console.WriteLine($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] 類別 : {sys_serverSettingClass.類別} , TableName {TableName} , {myTimerBasic.ToString()}");
            }
        }
        static private List<DeviceBasic> Function_Get_device(string IP, string DBName, string TableName, string UserName, string Password, uint Port)
        {
            SQLControl sQLControl_device = new SQLControl(IP, DBName, TableName, UserName, Password, Port, SSLMode);
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            if (TableName.StringIsEmpty() == false)
            {
                deviceBasics = DeviceBasicMethod.SQL_GetAllDeviceBasic(sQLControl_device);
            }
            else
            {
                deviceBasics = Function_讀取儲位(IP, DBName, UserName, Password, Port);
            }

            return deviceBasics;
        }
        static private List<DeviceBasic> Function_Get_device_by_ip(sys_serverSettingClass sys_serverSettingClass, string storageIP)
        {
            string IP = sys_serverSettingClass.Server;
            string DBName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            List<DeviceBasic> deviceBasics = Function_讀取儲位_by_ip(IP, DBName, UserName, Password, Port, storageIP);

            return deviceBasics;
        }

        static private List<DeviceBasic> Function_讀取儲位_by_ip(string IP, string DBName, string UserName, string Password, uint Port, string storageIP)
        {
            SQLControl sQLControl_EPD583_serialize = new SQLControl(IP, DBName, "epd583_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_EPD266_serialize = new SQLControl(IP, DBName, "epd266_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RowsLED_serialize = new SQLControl(IP, DBName, "rowsled_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RFID_Device_serialize = new SQLControl(IP, DBName, "rfid_device_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_WT32_serialize = new SQLControl(IP, DBName, "WT32_Jsonstring", UserName, Password, Port, SSLMode);


            List<object[]> list_EPD583 = new List<object[]>();
            List<object[]> list_EPD266 = new List<object[]>();
            List<object[]> list_RowsLED = new List<object[]>();
            List<object[]> list_RFID_Device = new List<object[]>();
            List<object[]> list_WT32 = new List<object[]>();
            List<Task> taskList = new List<Task>();
            taskList.Add(Task.Run(() =>
            {
              
                if (sQLControl_EPD583_serialize.IsTableCreat()) list_EPD583 = sQLControl_EPD583_serialize.GetRowsByDefult(null,(int)H_Pannel_lib.enum_DeviceTable.IP, storageIP);
            }));
            taskList.Add(Task.Run(() =>
            {
                if (sQLControl_EPD266_serialize.IsTableCreat()) list_EPD266 = sQLControl_EPD266_serialize.GetRowsByDefult(null, (int)H_Pannel_lib.enum_DeviceTable.IP, storageIP);
            }));
            taskList.Add(Task.Run(() =>
            {
                if (sQLControl_RowsLED_serialize.IsTableCreat()) list_RowsLED = sQLControl_RowsLED_serialize.GetRowsByDefult(null, (int)H_Pannel_lib.enum_DeviceTable.IP, storageIP);
            }));
            taskList.Add(Task.Run(() =>
            {
                if (sQLControl_RFID_Device_serialize.IsTableCreat()) list_RFID_Device = sQLControl_RFID_Device_serialize.GetRowsByDefult(null, (int)H_Pannel_lib.enum_DeviceTable.IP, storageIP);
            }));
            taskList.Add(Task.Run(() =>
            {
                if (sQLControl_WT32_serialize.IsTableCreat()) list_WT32 = sQLControl_WT32_serialize.GetRowsByDefult(null, (int)H_Pannel_lib.enum_DeviceTable.IP, storageIP);
            }));
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_buf = new List<DeviceBasic>();
            deviceBasics.LockAdd(DrawerMethod.GetAllDeviceBasic(list_EPD583));
            deviceBasics.LockAdd(StorageMethod.GetAllDeviceBasic(list_EPD266));
            deviceBasics.LockAdd(RowsLEDMethod.GetAllDeviceBasic(list_RowsLED));
            deviceBasics.LockAdd(RFIDMethod.GetAllDeviceBasic(list_RFID_Device));
            deviceBasics.LockAdd(StorageMethod.GetAllDeviceBasic(list_WT32));
            deviceBasics_buf = deviceBasics;
            //deviceBasics_buf = (from value in deviceBasics
            //                    where value.Code.StringIsEmpty() == false
            //                    select value).ToList();

            return deviceBasics_buf;
        }
        static private List<DeviceBasic> Function_讀取儲位(string IP, string DBName, string UserName, string Password, uint Port)
        {
    
            SQLControl sQLControl_EPD583_serialize = new SQLControl(IP, DBName, "epd583_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_EPD266_serialize = new SQLControl(IP, DBName, "epd266_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RowsLED_serialize = new SQLControl(IP, DBName, "rowsled_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RFID_Device_serialize = new SQLControl(IP, DBName, "rfid_device_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_WT32_serialize = new SQLControl(IP, DBName, "WT32_Jsonstring", UserName, Password, Port, SSLMode);

            
            List<object[]> list_EPD583 = new List<object[]>();
            List<object[]> list_EPD266 = new List<object[]>();
            List<object[]> list_RowsLED = new List<object[]>();
            List<object[]> list_RFID_Device = new List<object[]>();
            List<object[]> list_WT32 = new List<object[]>();

            List<DeviceBasic> deviceBasics_EPD583 = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_EPD266 = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_RowsLED = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_RFID_Device = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_WT32 = new List<DeviceBasic>();
            List<Task> taskList = new List<Task>();
            taskList.Add(Task.Run(() =>
            {
                if (sQLControl_EPD583_serialize.IsTableCreat()) list_EPD583 = sQLControl_EPD583_serialize.GetAllRows(null);
                if (list_EPD583.Count > 0)
                {
                    deviceBasics_EPD583 = DrawerMethod.GetAllDeviceBasic(list_EPD583);
                }
            }));
            taskList.Add(Task.Run(() =>
            {
                if (sQLControl_EPD266_serialize.IsTableCreat()) list_EPD266 = sQLControl_EPD266_serialize.GetAllRows(null);
                if (list_EPD266.Count > 0)
                {
                    deviceBasics_EPD266 = StorageMethod.GetAllDeviceBasic(list_EPD266);
                }

            }));
            taskList.Add(Task.Run(() =>
            {
                if (sQLControl_RowsLED_serialize.IsTableCreat()) list_RowsLED = sQLControl_RowsLED_serialize.GetAllRows(null);

                if (list_RowsLED.Count > 0)
                {
                    deviceBasics_RowsLED = RowsLEDMethod.GetAllDeviceBasic(list_RowsLED);
                }

            }));
            taskList.Add(Task.Run(() =>
            {
                if (sQLControl_RFID_Device_serialize.IsTableCreat()) list_RFID_Device = sQLControl_RFID_Device_serialize.GetAllRows(null);

                if (list_RFID_Device.Count > 0)
                {
                    deviceBasics_RFID_Device = RFIDMethod.GetAllDeviceBasic(list_RFID_Device);
                }

            }));
            taskList.Add(Task.Run(() =>
            {
                if (sQLControl_WT32_serialize.IsTableCreat()) list_WT32 = sQLControl_WT32_serialize.GetAllRows(null);


                if (list_WT32.Count > 0)
                {
                    deviceBasics_WT32 = StorageMethod.GetAllDeviceBasic(list_WT32);
                }
            }));
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_buf = new List<DeviceBasic>();
            deviceBasics.LockAdd(deviceBasics_EPD583);
            deviceBasics.LockAdd(deviceBasics_EPD266);
            deviceBasics.LockAdd(deviceBasics_RowsLED);
            deviceBasics.LockAdd(deviceBasics_RFID_Device);
            deviceBasics.LockAdd(deviceBasics_WT32);

            deviceBasics_buf = deviceBasics;
            //deviceBasics_buf = (from value in deviceBasics
            //                    where value.Code.StringIsEmpty() == false
            //                    select value).ToList();

            return deviceBasics_buf;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public List<DeviceBasic> Function_讀取儲位_By_Code(sys_serverSettingClass sys_serverSettingClass, string Code)
        {
            string IP = sys_serverSettingClass.Server;
            string DBName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();
            return Function_讀取儲位_By_Code(IP, DBName, UserName, Password, Port, Code);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        static public List<DeviceBasic> Function_讀取儲位_By_Code(string IP, string DBName, string UserName, string Password, uint Port , string Code)
        {
            SQLControl sQLControl_EPD583_serialize = new SQLControl(IP, DBName, "epd583_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_EPD266_serialize = new SQLControl(IP, DBName, "epd266_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RowsLED_serialize = new SQLControl(IP, DBName, "rowsled_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RFID_Device_serialize = new SQLControl(IP, DBName, "rfid_device_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_WT32_serialize = new SQLControl(IP, DBName, "WT32_Jsonstring", UserName, Password, Port, SSLMode);

            List<DeviceBasic> deviceBasics_EPD583 = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_EPD266 = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_RowsLED = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_RFID_Device = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_WT32 = new List<DeviceBasic>();

            List<Task> taskList = new List<Task>();
            taskList.Add(Task.Run(() =>
            {
                deviceBasics_EPD583 = DrawerMethod.GetDeviceBasicByCode(sQLControl_EPD583_serialize, Code);
            }));
            taskList.Add(Task.Run(() =>
            {
                deviceBasics_EPD266 = StorageMethod.GetDeviceBasicByCode(sQLControl_EPD266_serialize, Code);
            }));
            taskList.Add(Task.Run(() =>
            {
                deviceBasics_RowsLED = RowsLEDMethod.GetDeviceBasicByCode(sQLControl_RowsLED_serialize, Code);
            }));
            taskList.Add(Task.Run(() =>
            {
                //deviceBasics_RFID_Device = DrawerMethod.GetDeviceBasicByCode(sQLControl_RFID_Device_serialize, Code);
            }));
            taskList.Add(Task.Run(() =>
            {          
                deviceBasics_WT32 = StorageMethod.GetDeviceBasicByCode(sQLControl_WT32_serialize, Code);
            }));
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_buf = new List<DeviceBasic>();
            deviceBasics.LockAdd(deviceBasics_EPD583);
            deviceBasics.LockAdd(deviceBasics_EPD266);
            deviceBasics.LockAdd(deviceBasics_RowsLED);
            deviceBasics.LockAdd(deviceBasics_RFID_Device);
            deviceBasics.LockAdd(deviceBasics_WT32);

            deviceBasics_buf = deviceBasics;
            //deviceBasics_buf = (from value in deviceBasics
            //                    where value.Code.StringIsEmpty() == false
            //                    select value).ToList();

            return deviceBasics_buf;
        }

        static private byte[] Get_EPD290_LEDBytes(Color color)
        {
            byte[] LED_Bytes = new byte[10 * 3];

            for (int i = 0; i < 10; i++)
            {
                LED_Bytes[i * 3 + 0] = color.R;
                LED_Bytes[i * 3 + 1] = color.G;
                LED_Bytes[i * 3 + 2] = color.B;
            }
            return LED_Bytes;
        }
        static private byte[] Get_EPD266_LEDBytes(Color color)
        {
            byte[] LED_Bytes = new byte[10 * 3];

            for (int i = 0; i < 10; i++)
            {
                LED_Bytes[i * 3 + 0] = color.R;
                LED_Bytes[i * 3 + 1] = color.G;
                LED_Bytes[i * 3 + 2] = color.B;
            }
            return LED_Bytes;
        }
        static private byte[] Get_Rows_LEDBytes(ref byte[] LED_Bytes, int startNum, int EndNum, Color color)
        {
            for (int i = startNum; i < EndNum; i++)
            {
                if (i > LED_Bytes.Length) break;
                LED_Bytes[i * 3 + 0] = color.R;
                LED_Bytes[i * 3 + 1] = color.G;
                LED_Bytes[i * 3 + 2] = color.B;
            }
            return LED_Bytes;
        }

        #region Drawer Function
        static private int NumOfLED = 450;
        static private int Drawer_NumOf_H_Line = 4;
        static private int Drawer_NumOf_V_Line = 8;
        static private int NumOfLED_Pannel = 42;
        static private int NumOfLED_Drawer = 450 - NumOfLED_Pannel;

        static private int Pannel_Width
        {
            get
            {
                return 648;
            }
        }
        static private int Pannel_Height
        {
            get
            {
                return 480;
            }
        }

        static private bool Set_LED_UDP(UDP_Class uDP_Class, string IP, byte[] LED_Bytes)
        {
            if (uDP_Class != null)
            {
                return Communication.Set_WS2812_Buffer(uDP_Class, IP, 0, LED_Bytes);
            }
            return false;
        }
        static private bool Set_LED_UDP(UDP_Class uDP_Class, Drawer drawer, Box box, Color color)
        {
            List<Box> boxes = new List<Box>();
            boxes.Add(box);
            return Set_LED_UDP(uDP_Class, drawer, boxes, color);
        }
        static private bool Set_LED_UDP(UDP_Class uDP_Class, Drawer drawer, List<Box> boxes, Color color)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].IP == drawer.IP)
                {
                    drawer.LED_Bytes = Set_LEDBytes(drawer, boxes[i], color);
                }
            }
            drawer.LED_Bytes = Set_Pannel_LEDBytes(drawer, color);
            return Set_LED_UDP(uDP_Class, drawer.IP, drawer.LED_Bytes);
        }
        static private byte[] Set_Pannel_LEDBytes(Drawer drawer, Color color)
        {
            if (drawer.IsAllLight == false)
            {
                return Set_LEDBytes(drawer, color);
            }
            return Set_Pannel_LEDBytes(ref drawer.LED_Bytes, color);
        }
        static private byte[] Set_Pannel_LEDBytes(ref byte[] LED_Bytes, Color color)
        {
            for (int i = NumOfLED_Drawer; i < NumOfLED; i++)
            {
                if (i * 3 > LED_Bytes.Length) return LED_Bytes;
                LED_Bytes[i * 3 + 0] = color.R;
                LED_Bytes[i * 3 + 1] = color.G;
                LED_Bytes[i * 3 + 2] = color.B;
            }
            return LED_Bytes;
        }
        static private byte[] Set_LEDBytes(Drawer drawer, Color color)
        {
            for (int i = 0; i < NumOfLED; i++)
            {
                drawer.LED_Bytes[i * 3 + 0] = color.R;
                drawer.LED_Bytes[i * 3 + 1] = color.G;
                drawer.LED_Bytes[i * 3 + 2] = color.B;
            }
            return drawer.LED_Bytes;
        }
        static private byte[] Set_LEDBytes(Drawer drawer, Box box, Color color)
        {
            return Set_LEDBytes(drawer, box.Column, box.Row, color);
        }
        static private byte[] Set_LEDBytes(Drawer drawer, int col, int row, Color color)
        {
            if (col >= drawer.Boxes.Count) return drawer.LED_Bytes;
            for (int i = 0; i < drawer.Boxes.Count; i++)
            {
                if (row >= drawer.Boxes[i].Length) return drawer.LED_Bytes;
            }
            Rectangle rect = Get_Box_Combine(drawer, col, row);
            int width = rect.Width / (Pannel_Width / Drawer_NumOf_H_Line);
            int height = rect.Height / (Pannel_Height / Drawer_NumOf_V_Line);
            return Set_LEDBytes(col, row, width, height, ref drawer.LED_Bytes, color);
        }
        static private byte[] Set_LEDBytes(int col_x, int row_y, int width, int height, ref byte[] LEDBytes, Color color)
        {
            for (int i = 0; i < width; i++)
            {
                Set_Drawer_H_Leds(col_x + i + row_y * Drawer_NumOf_H_Line, ref LEDBytes, color);
                Set_Drawer_H_Leds(col_x + i + (height + row_y) * Drawer_NumOf_H_Line, ref LEDBytes, color);

            }
            for (int k = 0; k < height; k++)
            {
                Set_Drawer_V_Leds(row_y + k + col_x * Drawer_NumOf_V_Line, ref LEDBytes, color);
                Set_Drawer_V_Leds(row_y + k + (width + col_x) * Drawer_NumOf_V_Line, ref LEDBytes, color);
            }
            return LEDBytes;
        }
        static private void Set_Drawer_H_Leds(int col, ref byte[] LEDBytes, Color color)
        {
            for (int i = 0; i < List_Drawer_H_Line_Leds[col].Length; i++)
            {
                LEDBytes[List_Drawer_H_Line_Leds[col][i] * 3 + 0] = color.R;
                LEDBytes[List_Drawer_H_Line_Leds[col][i] * 3 + 1] = color.G;
                LEDBytes[List_Drawer_H_Line_Leds[col][i] * 3 + 2] = color.B;
            }
        }
        static private void Set_Drawer_V_Leds(int row, ref byte[] LEDBytes, Color color)
        {
            for (int i = 0; i < List_Drawer_V_Line_Leds[row].Length; i++)
            {
                LEDBytes[List_Drawer_V_Line_Leds[row][i] * 3 + 0] = color.R;
                LEDBytes[List_Drawer_V_Line_Leds[row][i] * 3 + 1] = color.G;
                LEDBytes[List_Drawer_V_Line_Leds[row][i] * 3 + 2] = color.B;
            }
        }
        static private Rectangle Get_Box_Combine(Drawer drawer, int col, int row)
        {
            if (col > drawer.Boxes.Count) return new Rectangle();
            if (row > drawer.Boxes[col].Length) return new Rectangle();
            return Get_Box_Combine(drawer, drawer.Boxes[col][row]);
        }
        static private Rectangle Get_Box_Combine(Drawer drawer, Box box)
        {
            Box _box;
            if (box.Slave)
            {
                _box = drawer.Boxes[box.MasterBox_Column][box.MasterBox_Row];
            }
            else
            {
                _box = box;
            }
            int X0 = _box.X;
            int Y0 = _box.Y;
            int X1 = X0 + _box.Width;
            int Y1 = Y0 + _box.Height;
            int X_temp = 0;
            int Y_temp = 0;
            int SlaveBox_Column = _box.SlaveBox_Column;
            int SlaveBox_Row = _box.SlaveBox_Row;
            int SlaveBox_Column_temp = SlaveBox_Column;
            int SlaveBox_Row_temp = SlaveBox_Row;
            while (true)
            {
                if (SlaveBox_Column_temp == -1 && SlaveBox_Row_temp == -1) break;
                SlaveBox_Column = SlaveBox_Column_temp;
                SlaveBox_Row = SlaveBox_Row_temp;
                X_temp = drawer.Boxes[SlaveBox_Column][SlaveBox_Row].X + drawer.Boxes[SlaveBox_Column][SlaveBox_Row].Width;
                Y_temp = drawer.Boxes[SlaveBox_Column][SlaveBox_Row].Y + drawer.Boxes[SlaveBox_Column][SlaveBox_Row].Height;
                if (X_temp > X1) X1 = X_temp;
                if (Y_temp > Y1) Y1 = Y_temp;
                SlaveBox_Column_temp = drawer.Boxes[SlaveBox_Column][SlaveBox_Row].SlaveBox_Column;
                SlaveBox_Row_temp = drawer.Boxes[SlaveBox_Column][SlaveBox_Row].SlaveBox_Row;
            }
            Rectangle rectangle = new Rectangle(X0, Y0, X1 - X0, Y1 - Y0);
            return rectangle;


        }
        #region LED_Line

        private static List<int[]> List_Drawer_H_Line_Leds = new List<int[]>();
        private static List<int[]> List_Drawer_V_Line_Leds = new List<int[]>();
        private static int[] Line_H_00 = new int[] { 376 + 0, 376 + 1, 376 + 2, 376 + 3, 376 + 4, 376 + 5, 376 + 6, 376 + 7 };
        private static int[] Line_H_01 = new int[] { 384 + 0, 384 + 1, 384 + 2, 384 + 3, 384 + 4, 384 + 5, 384 + 6, 384 + 7 };
        private static int[] Line_H_02 = new int[] { 392 + 0, 392 + 1, 392 + 2, 392 + 3, 392 + 4, 392 + 5, 392 + 6, 392 + 7 };
        private static int[] Line_H_03 = new int[] { 400 + 0, 400 + 1, 400 + 2, 400 + 3, 400 + 4, 400 + 5, 400 + 6, 400 + 7 };

        private static int[] Line_H_04 = new int[] { 368 + 0, 368 + 1, 368 + 2, 368 + 3, 368 + 4, 368 + 5, 368 + 6, 368 + 7 };
        private static int[] Line_H_05 = new int[] { 360 + 0, 360 + 1, 360 + 2, 360 + 3, 360 + 4, 360 + 5, 360 + 6, 360 + 7 };
        private static int[] Line_H_06 = new int[] { 352 + 0, 352 + 1, 352 + 2, 352 + 3, 352 + 4, 352 + 5, 352 + 6, 352 + 7 };
        private static int[] Line_H_07 = new int[] { 344 + 0, 344 + 1, 344 + 2, 344 + 3, 344 + 4, 344 + 5, 344 + 6, 344 + 7 };

        private static int[] Line_H_08 = new int[] { 312 + 0, 312 + 1, 312 + 2, 312 + 3, 312 + 4, 312 + 5, 312 + 6, 312 + 7 };
        private static int[] Line_H_09 = new int[] { 320 + 0, 320 + 1, 320 + 2, 320 + 3, 320 + 4, 320 + 5, 320 + 6, 320 + 7 };
        private static int[] Line_H_10 = new int[] { 328 + 0, 328 + 1, 328 + 2, 328 + 3, 328 + 4, 328 + 5, 328 + 6, 328 + 7 };
        private static int[] Line_H_11 = new int[] { 336 + 0, 336 + 1, 336 + 2, 336 + 3, 336 + 4, 336 + 5, 336 + 6, 336 + 7 };

        private static int[] Line_H_12 = new int[] { 304 + 0, 304 + 1, 304 + 2, 304 + 3, 304 + 4, 304 + 5, 304 + 6, 304 + 7 };
        private static int[] Line_H_13 = new int[] { 296 + 0, 296 + 1, 296 + 2, 296 + 3, 296 + 4, 296 + 5, 296 + 6, 296 + 7 };
        private static int[] Line_H_14 = new int[] { 288 + 0, 288 + 1, 288 + 2, 288 + 3, 288 + 4, 288 + 5, 288 + 6, 288 + 7 };
        private static int[] Line_H_15 = new int[] { 280 + 0, 280 + 1, 280 + 2, 280 + 3, 280 + 4, 280 + 5, 280 + 6, 280 + 7 };

        private static int[] Line_H_16 = new int[] { 248 + 0, 248 + 1, 248 + 2, 248 + 3, 248 + 4, 248 + 5, 248 + 6, 248 + 7 };
        private static int[] Line_H_17 = new int[] { 256 + 0, 256 + 1, 256 + 2, 256 + 3, 256 + 4, 256 + 5, 256 + 6, 256 + 7 };
        private static int[] Line_H_18 = new int[] { 264 + 0, 264 + 1, 264 + 2, 264 + 3, 264 + 4, 264 + 5, 264 + 6, 264 + 7 };
        private static int[] Line_H_19 = new int[] { 272 + 0, 272 + 1, 272 + 2, 272 + 3, 272 + 4, 272 + 5, 272 + 6, 272 + 7 };

        private static int[] Line_H_20 = new int[] { 240 + 0, 240 + 1, 240 + 2, 240 + 3, 240 + 4, 240 + 5, 240 + 6, 240 + 7 };
        private static int[] Line_H_21 = new int[] { 232 + 0, 232 + 1, 232 + 2, 232 + 3, 232 + 4, 232 + 5, 232 + 6, 232 + 7 };
        private static int[] Line_H_22 = new int[] { 224 + 0, 224 + 1, 224 + 2, 224 + 3, 224 + 4, 224 + 5, 224 + 6, 224 + 7 };
        private static int[] Line_H_23 = new int[] { 216 + 0, 216 + 1, 216 + 2, 216 + 3, 216 + 4, 216 + 5, 216 + 6, 216 + 7 };

        private static int[] Line_H_24 = new int[] { 184 + 0, 184 + 1, 184 + 2, 184 + 3, 184 + 4, 184 + 5, 184 + 6, 184 + 7 };
        private static int[] Line_H_25 = new int[] { 192 + 0, 192 + 1, 192 + 2, 192 + 3, 192 + 4, 192 + 5, 192 + 6, 192 + 7 };
        private static int[] Line_H_26 = new int[] { 200 + 0, 200 + 1, 200 + 2, 200 + 3, 200 + 4, 200 + 5, 200 + 6, 200 + 7 };
        private static int[] Line_H_27 = new int[] { 208 + 0, 208 + 1, 208 + 2, 208 + 3, 208 + 4, 208 + 5, 208 + 6, 208 + 7 };

        private static int[] Line_H_28 = new int[] { 176 + 0, 176 + 1, 176 + 2, 176 + 3, 176 + 4, 176 + 5, 176 + 6, 176 + 7 };
        private static int[] Line_H_29 = new int[] { 168 + 0, 168 + 1, 168 + 2, 168 + 3, 168 + 4, 168 + 5, 168 + 6, 168 + 7 };
        private static int[] Line_H_30 = new int[] { 160 + 0, 160 + 1, 160 + 2, 160 + 3, 160 + 4, 160 + 5, 160 + 6, 160 + 7 };
        private static int[] Line_H_31 = new int[] { 152 + 0, 152 + 1, 152 + 2, 152 + 3, 152 + 4, 152 + 5, 152 + 6, 152 + 7 };

        private static int[] Line_H_32 = new int[] { 120 + 0, 120 + 1, 120 + 2, 120 + 3, 120 + 4, 120 + 5, 120 + 6, 120 + 7 };
        private static int[] Line_H_33 = new int[] { 128 + 0, 128 + 1, 128 + 2, 128 + 3, 128 + 4, 128 + 5, 128 + 6, 128 + 7 };
        private static int[] Line_H_34 = new int[] { 136 + 0, 136 + 1, 136 + 2, 136 + 3, 136 + 4, 136 + 5, 136 + 6, 136 + 7 };
        private static int[] Line_H_35 = new int[] { 144 + 0, 144 + 1, 144 + 2, 144 + 3, 144 + 4, 144 + 5, 144 + 6, 144 + 7 };


        private static int[] Line_V_00 = new int[] { 96 + (0 * 3) + 0, 96 + (0 * 3) + 1, 96 + (0 * 3) + 2 };
        private static int[] Line_V_01 = new int[] { 96 + (1 * 3) + 0, 96 + (1 * 3) + 1, 96 + (1 * 3) + 2 };
        private static int[] Line_V_02 = new int[] { 96 + (2 * 3) + 0, 96 + (2 * 3) + 1, 96 + (2 * 3) + 2 };
        private static int[] Line_V_03 = new int[] { 96 + (3 * 3) + 0, 96 + (3 * 3) + 1, 96 + (3 * 3) + 2 };
        private static int[] Line_V_04 = new int[] { 96 + (4 * 3) + 0, 96 + (4 * 3) + 1, 96 + (4 * 3) + 2 };
        private static int[] Line_V_05 = new int[] { 96 + (5 * 3) + 0, 96 + (5 * 3) + 1, 96 + (5 * 3) + 2 };
        private static int[] Line_V_06 = new int[] { 96 + (6 * 3) + 0, 96 + (6 * 3) + 1, 96 + (6 * 3) + 2 };
        private static int[] Line_V_07 = new int[] { 96 + (7 * 3) + 0, 96 + (7 * 3) + 1, 96 + (7 * 3) + 2 };

        private static int[] Line_V_08 = new int[] { 95 - (0 * 3) - 0, 95 - (0 * 3) - 1, 95 - (0 * 3) - 2 };
        private static int[] Line_V_09 = new int[] { 95 - (1 * 3) - 0, 95 - (1 * 3) - 1, 95 - (1 * 3) - 2 };
        private static int[] Line_V_10 = new int[] { 95 - (2 * 3) - 0, 95 - (2 * 3) - 1, 95 - (2 * 3) - 2 };
        private static int[] Line_V_11 = new int[] { 95 - (3 * 3) - 0, 95 - (3 * 3) - 1, 95 - (3 * 3) - 2 };
        private static int[] Line_V_12 = new int[] { 95 - (4 * 3) - 0, 95 - (4 * 3) - 1, 95 - (4 * 3) - 2 };
        private static int[] Line_V_13 = new int[] { 95 - (5 * 3) - 0, 95 - (5 * 3) - 1, 95 - (5 * 3) - 2 };
        private static int[] Line_V_14 = new int[] { 95 - (6 * 3) - 0, 95 - (6 * 3) - 1, 95 - (6 * 3) - 2 };
        private static int[] Line_V_15 = new int[] { 95 - (7 * 3) - 0, 95 - (7 * 3) - 1, 95 - (7 * 3) - 2 };

        private static int[] Line_V_16 = new int[] { 48 + (0 * 3) + 0, 48 + (0 * 3) + 1, 48 + (0 * 3) + 2 };
        private static int[] Line_V_17 = new int[] { 48 + (1 * 3) + 0, 48 + (1 * 3) + 1, 48 + (1 * 3) + 2 };
        private static int[] Line_V_18 = new int[] { 48 + (2 * 3) + 0, 48 + (2 * 3) + 1, 48 + (2 * 3) + 2 };
        private static int[] Line_V_19 = new int[] { 48 + (3 * 3) + 0, 48 + (3 * 3) + 1, 48 + (3 * 3) + 2 };
        private static int[] Line_V_20 = new int[] { 48 + (4 * 3) + 0, 48 + (4 * 3) + 1, 48 + (4 * 3) + 2 };
        private static int[] Line_V_21 = new int[] { 48 + (5 * 3) + 0, 48 + (5 * 3) + 1, 48 + (5 * 3) + 2 };
        private static int[] Line_V_22 = new int[] { 48 + (6 * 3) + 0, 48 + (6 * 3) + 1, 48 + (6 * 3) + 2 };
        private static int[] Line_V_23 = new int[] { 48 + (7 * 3) + 0, 48 + (7 * 3) + 1, 48 + (7 * 3) + 2 };

        private static int[] Line_V_24 = new int[] { 47 - (0 * 3) - 0, 47 - (0 * 3) - 1, 47 - (0 * 3) - 2 };
        private static int[] Line_V_25 = new int[] { 47 - (1 * 3) - 0, 47 - (1 * 3) - 1, 47 - (1 * 3) - 2 };
        private static int[] Line_V_26 = new int[] { 47 - (2 * 3) - 0, 47 - (2 * 3) - 1, 47 - (2 * 3) - 2 };
        private static int[] Line_V_27 = new int[] { 47 - (3 * 3) - 0, 47 - (3 * 3) - 1, 47 - (3 * 3) - 2 };
        private static int[] Line_V_28 = new int[] { 47 - (4 * 3) - 0, 47 - (4 * 3) - 1, 47 - (4 * 3) - 2 };
        private static int[] Line_V_29 = new int[] { 47 - (5 * 3) - 0, 47 - (5 * 3) - 1, 47 - (5 * 3) - 2 };
        private static int[] Line_V_30 = new int[] { 47 - (6 * 3) - 0, 47 - (6 * 3) - 1, 47 - (6 * 3) - 2 };
        private static int[] Line_V_31 = new int[] { 47 - (7 * 3) - 0, 47 - (7 * 3) - 1, 47 - (7 * 3) - 2 };

        private static int[] Line_V_32 = new int[] { 00 + (0 * 3) + 0, 00 + (0 * 3) + 1, 00 + (0 * 3) + 2 };
        private static int[] Line_V_33 = new int[] { 00 + (1 * 3) + 0, 00 + (1 * 3) + 1, 00 + (1 * 3) + 2 };
        private static int[] Line_V_34 = new int[] { 00 + (2 * 3) + 0, 00 + (2 * 3) + 1, 00 + (2 * 3) + 2 };
        private static int[] Line_V_35 = new int[] { 00 + (3 * 3) + 0, 00 + (3 * 3) + 1, 00 + (3 * 3) + 2 };
        private static int[] Line_V_36 = new int[] { 00 + (4 * 3) + 0, 00 + (4 * 3) + 1, 00 + (4 * 3) + 2 };
        private static int[] Line_V_37 = new int[] { 00 + (5 * 3) + 0, 00 + (5 * 3) + 1, 00 + (5 * 3) + 2 };
        private static int[] Line_V_38 = new int[] { 00 + (6 * 3) + 0, 00 + (6 * 3) + 1, 00 + (6 * 3) + 2 };
        private static int[] Line_V_39 = new int[] { 00 + (7 * 3) + 0, 00 + (7 * 3) + 1, 00 + (7 * 3) + 2 };
        static private void WS2812_Init()
        {
            List_Drawer_H_Line_Leds.Clear();
            List_Drawer_V_Line_Leds.Clear();

            List_Drawer_H_Line_Leds.Add(Line_H_00);
            List_Drawer_H_Line_Leds.Add(Line_H_01);
            List_Drawer_H_Line_Leds.Add(Line_H_02);
            List_Drawer_H_Line_Leds.Add(Line_H_03);

            List_Drawer_H_Line_Leds.Add(Line_H_04);
            List_Drawer_H_Line_Leds.Add(Line_H_05);
            List_Drawer_H_Line_Leds.Add(Line_H_06);
            List_Drawer_H_Line_Leds.Add(Line_H_07);

            List_Drawer_H_Line_Leds.Add(Line_H_08);
            List_Drawer_H_Line_Leds.Add(Line_H_09);
            List_Drawer_H_Line_Leds.Add(Line_H_10);
            List_Drawer_H_Line_Leds.Add(Line_H_11);

            List_Drawer_H_Line_Leds.Add(Line_H_12);
            List_Drawer_H_Line_Leds.Add(Line_H_13);
            List_Drawer_H_Line_Leds.Add(Line_H_14);
            List_Drawer_H_Line_Leds.Add(Line_H_15);

            List_Drawer_H_Line_Leds.Add(Line_H_16);
            List_Drawer_H_Line_Leds.Add(Line_H_17);
            List_Drawer_H_Line_Leds.Add(Line_H_18);
            List_Drawer_H_Line_Leds.Add(Line_H_19);

            List_Drawer_H_Line_Leds.Add(Line_H_20);
            List_Drawer_H_Line_Leds.Add(Line_H_21);
            List_Drawer_H_Line_Leds.Add(Line_H_22);
            List_Drawer_H_Line_Leds.Add(Line_H_23);

            List_Drawer_H_Line_Leds.Add(Line_H_24);
            List_Drawer_H_Line_Leds.Add(Line_H_25);
            List_Drawer_H_Line_Leds.Add(Line_H_26);
            List_Drawer_H_Line_Leds.Add(Line_H_27);

            List_Drawer_H_Line_Leds.Add(Line_H_28);
            List_Drawer_H_Line_Leds.Add(Line_H_29);
            List_Drawer_H_Line_Leds.Add(Line_H_30);
            List_Drawer_H_Line_Leds.Add(Line_H_31);

            List_Drawer_H_Line_Leds.Add(Line_H_32);
            List_Drawer_H_Line_Leds.Add(Line_H_33);
            List_Drawer_H_Line_Leds.Add(Line_H_34);
            List_Drawer_H_Line_Leds.Add(Line_H_35);

            List_Drawer_V_Line_Leds.Add(Line_V_00);
            List_Drawer_V_Line_Leds.Add(Line_V_01);
            List_Drawer_V_Line_Leds.Add(Line_V_02);
            List_Drawer_V_Line_Leds.Add(Line_V_03);
            List_Drawer_V_Line_Leds.Add(Line_V_04);

            List_Drawer_V_Line_Leds.Add(Line_V_05);
            List_Drawer_V_Line_Leds.Add(Line_V_06);
            List_Drawer_V_Line_Leds.Add(Line_V_07);
            List_Drawer_V_Line_Leds.Add(Line_V_08);
            List_Drawer_V_Line_Leds.Add(Line_V_09);

            List_Drawer_V_Line_Leds.Add(Line_V_10);
            List_Drawer_V_Line_Leds.Add(Line_V_11);
            List_Drawer_V_Line_Leds.Add(Line_V_12);
            List_Drawer_V_Line_Leds.Add(Line_V_13);
            List_Drawer_V_Line_Leds.Add(Line_V_14);

            List_Drawer_V_Line_Leds.Add(Line_V_15);
            List_Drawer_V_Line_Leds.Add(Line_V_16);
            List_Drawer_V_Line_Leds.Add(Line_V_17);
            List_Drawer_V_Line_Leds.Add(Line_V_18);
            List_Drawer_V_Line_Leds.Add(Line_V_19);

            List_Drawer_V_Line_Leds.Add(Line_V_20);
            List_Drawer_V_Line_Leds.Add(Line_V_21);
            List_Drawer_V_Line_Leds.Add(Line_V_22);
            List_Drawer_V_Line_Leds.Add(Line_V_23);
            List_Drawer_V_Line_Leds.Add(Line_V_24);

            List_Drawer_V_Line_Leds.Add(Line_V_25);
            List_Drawer_V_Line_Leds.Add(Line_V_26);
            List_Drawer_V_Line_Leds.Add(Line_V_27);
            List_Drawer_V_Line_Leds.Add(Line_V_28);
            List_Drawer_V_Line_Leds.Add(Line_V_29);

            List_Drawer_V_Line_Leds.Add(Line_V_30);
            List_Drawer_V_Line_Leds.Add(Line_V_31);
            List_Drawer_V_Line_Leds.Add(Line_V_32);
            List_Drawer_V_Line_Leds.Add(Line_V_33);
            List_Drawer_V_Line_Leds.Add(Line_V_34);

            List_Drawer_V_Line_Leds.Add(Line_V_35);
            List_Drawer_V_Line_Leds.Add(Line_V_36);
            List_Drawer_V_Line_Leds.Add(Line_V_37);
            List_Drawer_V_Line_Leds.Add(Line_V_38);
            List_Drawer_V_Line_Leds.Add(Line_V_39);
        }
        #endregion
        #endregion


        static public List<object[]> Function_取得異動儲位資訊(List<DeviceBasic> deviceBasics, string 藥品碼, double 異動量)
        {
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            deviceBasics = deviceBasics.SortByCode(藥品碼);

            for (int i = 0; i < deviceBasics.Count; i++)
            {
                儲位.Add(deviceBasics[i]);
                儲位_TYPE.Add(deviceBasics[i].DeviceType.GetEnumName());

            }

            List<object[]> 儲位資訊_buf = new List<object[]>();
            List<object[]> 儲位資訊 = new List<object[]>();
            if (儲位.Count == 0) return 儲位資訊_buf;


            for (int k = 0; k < 儲位.Count; k++)
            {
                object value_device = 儲位[k];
                if (value_device is Device)
                {
                    Device device = (Device)value_device;
                    for (int i = 0; i < device.List_Validity_period.Count; i++)
                    {
                        object[] value = new object[new enum_儲位資訊().GetLength()];
                        value[(int)enum_儲位資訊.IP] = device.IP;
                        value[(int)enum_儲位資訊.TYPE] = 儲位_TYPE[k];
                        value[(int)enum_儲位資訊.效期] = device.List_Validity_period[i];
                        value[(int)enum_儲位資訊.批號] = device.List_Lot_number[i];
                        value[(int)enum_儲位資訊.庫存] = device.List_Inventory[i];
                        value[(int)enum_儲位資訊.異動量] = "0";
                        value[(int)enum_儲位資訊.Value] = value_device;
                        儲位資訊.Add(value);
                    }
                }
            }
            儲位資訊 = 儲位資訊.OrderBy(r => DateTime.Parse(r[(int)enum_儲位資訊.效期].ToDateString())).ToList();

            if (異動量 == 0) return 儲位資訊;
            double 使用數量 = 異動量;
            double 庫存數量 = 0;
            double 剩餘庫存數量 = 0;
            for (int i = 0; i < 儲位資訊.Count; i++)
            {
                庫存數量 = 儲位資訊[i][(int)enum_儲位資訊.庫存].ObjectToString().StringToInt32();
                if ((使用數量 < 0 && 庫存數量 > 0) || (使用數量 > 0 && 庫存數量 >= 0))
                {
                    剩餘庫存數量 = 庫存數量 + 使用數量;
                    if (剩餘庫存數量 >= 0)
                    {
                        儲位資訊[i][(int)enum_儲位資訊.異動量] = (使用數量).ToString();
                        儲位資訊_buf.Add(儲位資訊[i]);
                        break;
                    }
                    else
                    {
                        儲位資訊[i][(int)enum_儲位資訊.異動量] = (庫存數量 * -1).ToString();
                        使用數量 = 剩餘庫存數量;
                        儲位資訊_buf.Add(儲位資訊[i]);
                    }
                }
            }

            return 儲位資訊_buf;
        }
        static public object Function_庫存異動(object[] 儲位資訊 , sys_serverSettingClass sys_serverSettingClass)
        {
            SQLControl sQLControl_EPD583_serialize = new SQLControl(sys_serverSettingClass.Server, sys_serverSettingClass.DBName, "epd583_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_EPD266_serialize = new SQLControl(sys_serverSettingClass.Server, sys_serverSettingClass.DBName, "epd266_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RowsLED_serialize = new SQLControl(sys_serverSettingClass.Server, sys_serverSettingClass.DBName, "rowsled_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RFID_Device_serialize = new SQLControl(sys_serverSettingClass.Server, sys_serverSettingClass.DBName, "rfid_device_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_WT32_serialize = new SQLControl(sys_serverSettingClass.Server, sys_serverSettingClass.DBName, "WT32_Jsonstring", UserName, Password, Port, SSLMode);

            object Value = 儲位資訊[(int)enum_儲位資訊.Value];
            string 效期 = 儲位資訊[(int)enum_儲位資訊.效期].ObjectToString();
            string 異動量 = 儲位資訊[(int)enum_儲位資訊.異動量].ObjectToString();
            string TYPE = 儲位資訊[(int)enum_儲位資訊.TYPE].ObjectToString();
            if (Value is Storage)
            {
                if (TYPE == DeviceType.EPD266.GetEnumName() || TYPE == DeviceType.EPD266_lock.GetEnumName() || TYPE == DeviceType.EPD290.GetEnumName() || TYPE == DeviceType.EPD290_lock.GetEnumName())
                {
                  
                    Storage storage = (Storage)Value;
                    storage = StorageMethod.SQL_GetStorageByIP(sQLControl_EPD266_serialize ,storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, false);
                        StorageMethod.SQL_ReplaceByIP(sQLControl_EPD266_serialize ,storage);
                        return storage;
                    }
                }
                if (TYPE == DeviceType.Pannel35.GetEnumName() || TYPE == DeviceType.Pannel35_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = StorageMethod.SQL_GetStorageByIP(sQLControl_WT32_serialize ,storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, false);
                        StorageMethod.SQL_ReplaceByIP(sQLControl_WT32_serialize, storage);
                        return storage;
                    }
                }
            }
            else if (Value is Box)
            {
                if (TYPE == DeviceType.EPD583.GetEnumName() || TYPE == DeviceType.EPD583_lock.GetEnumName())
                {
                    Box box = (Box)Value;
                    Drawer drawer = DrawerMethod.SQL_GetDrawerByIP(sQLControl_EPD583_serialize, box.IP);                   
                    box.效期庫存異動(效期, 異動量, false);
                    drawer.ReplaceBox(box);
                    DrawerMethod.SQL_ReplaceByIP(sQLControl_EPD583_serialize, drawer);
                    return drawer;
                }
          
            }
            else if (Value is RowsDevice)
            {
                if (TYPE == DeviceType.RowsLED.GetEnumName())
                {
                    RowsDevice rowsDevice = Value as RowsDevice;
                    RowsLED rowsLED = RowsLEDMethod.SQL_GetRowsLEDByIP(sQLControl_RFID_Device_serialize, rowsDevice.IP);
                    rowsDevice.效期庫存異動(效期, 異動量, false);
                    RowsLEDMethod.SQL_ReplaceByIP(sQLControl_RowsLED_serialize, rowsLED);
                    return rowsLED;
                }

            }
      
            return null;
        }


    }
}
