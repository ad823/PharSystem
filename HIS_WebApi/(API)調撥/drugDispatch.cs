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
    public class drugDispatch : Controller
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
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(drugDispatchClass))]
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
        ///        [drugDispatchClass]
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

                List<drugDispatchClass> drugDispatchClasses = returnData.Data.ObjToClass<List<drugDispatchClass>>();
                List<drugDispatchClass> drugDispatchClasses_buf = new List<drugDispatchClass>();
                if (drugDispatchClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }

                for (int i = 0; i < drugDispatchClasses.Count; i++)
                {
                    if (drugDispatchClasses[i].藥碼.StringIsEmpty()) continue;
                    drugDispatchClasses[i].GUID = Guid.NewGuid().ToString();
                    drugDispatchClasses[i].產出時間 = DateTime.Now.ToDateTimeString();
                    drugDispatchClasses[i].過帳時間 = DateTime.MinValue.ToDateTimeString();
                    drugDispatchClasses[i].出庫庫存 = "";
                    drugDispatchClasses[i].入庫庫存 = "";
                    drugDispatchClasses[i].出庫結存 = "";
                    drugDispatchClasses[i].入庫結存 = "";
                    drugDispatchClasses[i].狀態 = "等待過帳";
                    drugDispatchClasses_buf.Add(drugDispatchClasses[i]);
                }
                List<object[]> list_value = drugDispatchClasses_buf.ClassToSQL<drugDispatchClass, enum_drugDispatch>();
                Table table = new Table(new enum_drugDispatch());
                SQLControl sQLControl_drugDispatch = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                sQLControl_drugDispatch.AddRows(null, list_value);
                returnData.Code = 200;
                returnData.Result = $"新增調撥資料共<{list_value.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = drugDispatchClasses_buf;
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
        /// 新增過帳資料集
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
        ///        [drugDispatchClass]
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]</returns>
        [Route("datas_posting")]
        [HttpPost]
        public string POST_datas_posting([FromBody] returnData returnData)
        {

            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "datas_posting";
            try
            {
                init(returnData);
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = new List<sys_serverSettingClass>();
                sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses_buf.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass sys_serverSettingClass_VM = sys_serverSettingClasses_buf[0];

                string Server = sys_serverSettingClass_VM.Server;
                string DB = sys_serverSettingClass_VM.DBName;
                string UserName = sys_serverSettingClass_VM.User;
                string Password = sys_serverSettingClass_VM.Password;
                uint Port = (uint)sys_serverSettingClass_VM.Port.StringToInt32();

                List<drugDispatchClass> drugDispatchClasses = returnData.Data.ObjToClass<List<drugDispatchClass>>();
                List<drugDispatchClass> drugDispatchClasses_buf = new List<drugDispatchClass>();



                if (drugDispatchClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常!";
                    return returnData.JsonSerializationt();
                }
                if (drugDispatchClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"無資料可過帳!";
                    return returnData.JsonSerializationt();
                }

                drugDispatchClasses_buf = Function_Inbound(sys_serverSettingClasses, drugDispatchClasses);
                for (int i = 0; i < drugDispatchClasses_buf.Count; i++)
                {
                    drugDispatchClasses_buf[i].GUID = Guid.NewGuid().ToString();
                    drugDispatchClasses_buf[i].產出時間 = DateTime.Now.ToDateTimeString_6();
                    drugDispatchClasses_buf[i].過帳時間 = DateTime.Now.ToDateTimeString_6();
                }
                List<object[]> list_value = drugDispatchClasses_buf.ClassToSQL<drugDispatchClass, enum_drugDispatch>();
                Table table = new Table(new enum_drugDispatch());
                SQLControl sQLControl_drugDispatch = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                sQLControl_drugDispatch.AddRows(null, list_value);
                returnData.Code = 200;
                returnData.Result = $"新增調撥資料共<{list_value.Count}>筆";
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = drugDispatchClasses_buf;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }


        }

        private enum enum_儲位資訊
        {
            IP,
            TYPE,
            效期,
            批號,
            庫存,
            異動量,
            Value,
        }

        private List<drugDispatchClass> Function_Inbound(List<sys_serverSettingClass> sys_serverSettingClasses,List<drugDispatchClass> drugDispatchClasses)
        {
            List<drugDispatchClass> drugDispatchClasses_buf = new List<drugDispatchClass>();
            string Action = "";
            string 藥碼 = "";
            string 入庫庫別 = "";
            string 出庫庫別 = "";
            double 入庫庫存 = 0;
            double 出庫庫存 = 0;
            double 入庫結存;
            double 出庫結存;
            double 調出量 = 0;
            double 調入量 = 0;

            List<Task> tasks = new List<Task>();
            List<DeviceBasic> deviceBasics_Inbound = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_Outbound = new List<DeviceBasic>();
            for (int i = 0; i < drugDispatchClasses.Count; i++)
            {
                string 調入備註 = "";
                string 調出備註 = "";
                Action = drugDispatchClasses[i].動作類別;
                if (Action != enum_交易記錄查詢動作.調入作業.GetEnumName()) continue;
                藥碼 = drugDispatchClasses[i].藥碼;
                入庫庫別 = drugDispatchClasses[i].入庫庫別;
                出庫庫別 = drugDispatchClasses[i].出庫庫別;
                調出量 = drugDispatchClasses[i].出庫量.StringToInt32();
                調入量 = 0;
                sys_serverSettingClass sys_serverSettingClass_Inbound = sys_serverSettingClasses.myFind(入庫庫別, "調劑台", "儲位資料");
                sys_serverSettingClass sys_serverSettingClass_Outbound = sys_serverSettingClasses.myFind(出庫庫別, "調劑台", "儲位資料");
                if (sys_serverSettingClass_Inbound == null) continue;
                if (sys_serverSettingClass_Outbound == null) continue;
                tasks.Add(Task.Run(new Action(delegate
                {
                    deviceBasics_Inbound = deviceController.Function_讀取儲位_By_Code(sys_serverSettingClass_Inbound, 藥碼);
                })));

                tasks.Add(Task.Run(new Action(delegate
                {
                    deviceBasics_Outbound = deviceController.Function_讀取儲位_By_Code(sys_serverSettingClass_Outbound, 藥碼);
                })));

                Task.WhenAll(tasks).Wait();
                tasks.Clear();
                入庫庫存 = deviceBasics_Inbound.GetInventory();
                出庫庫存 = deviceBasics_Outbound.GetInventory();
                List<object[]> 儲位資訊_Outbound = Function_取得出庫儲位資訊(deviceBasics_Outbound, 調出量 * -1);

                for (int k = 0; k < 儲位資訊_Outbound.Count; k++)
                {               
                    調入量 += 儲位資訊_Outbound[k][(int)enum_儲位資訊.異動量].StringToInt32();
                }
                調入量 = 調入量 * -1;
                List<object[]> 儲位資訊_Inbound = Function_取得入庫儲位資訊(deviceBasics_Inbound, 儲位資訊_Outbound);


                tasks.Add(Task.Run(new Action(delegate
                {
                    string 效期 = "";
                    string 批號 = "";
                    for (int k = 0; k < 儲位資訊_Inbound.Count; k++)
                    {
                        Function_庫存異動上傳(sys_serverSettingClass_Inbound, 儲位資訊_Inbound[k]);

                        效期 = 儲位資訊_Inbound[k][(int)enum_儲位資訊.效期].ObjectToString();
                        批號 = 儲位資訊_Inbound[k][(int)enum_儲位資訊.批號].ObjectToString();

                        調入備註 += $"[效期]:{效期},[批號]:{批號}";
                        if (k != 儲位資訊_Inbound.Count - 1) 調入備註 += "\n";
                    }
                })));

                tasks.Add(Task.Run(new Action(delegate
                {
                    string 效期 = "";
                    string 批號 = "";
                    for (int k = 0; k < 儲位資訊_Outbound.Count; k++)
                    {
                        Function_庫存異動上傳(sys_serverSettingClass_Outbound, 儲位資訊_Outbound[k]);

                        效期 = 儲位資訊_Outbound[k][(int)enum_儲位資訊.效期].ObjectToString();
                        批號 = 儲位資訊_Outbound[k][(int)enum_儲位資訊.批號].ObjectToString();

                        調出備註 += $"[效期]:{效期},[批號]:{批號}";
                        if (k != 儲位資訊_Inbound.Count - 1) 調出備註 += "\n";
                    }
                })));

                Task.WhenAll(tasks).Wait();
                tasks.Clear();

                drugDispatchClasses[i].入庫庫存 = 入庫庫存.ToString();
                drugDispatchClasses[i].入庫量 = 調出量.ToString();
                drugDispatchClasses[i].入庫結存 = (入庫庫存 + 調出量).ToString();

                drugDispatchClasses[i].出庫庫存 = 出庫庫存.ToString();
                drugDispatchClasses[i].出庫量 = (調出量 * -1).ToString();
                drugDispatchClasses[i].出庫結存 = (出庫庫存 + (調出量 * -1)).ToString();

                tasks.Add(Task.Run(new Action(delegate
                {
                    transactionsClass transactionsClass = new transactionsClass();
                    transactionsClass.GUID = Guid.NewGuid().ToString();
                    transactionsClass.動作 = enum_交易記錄查詢動作.調入作業.GetEnumName();
                    transactionsClass.藥品碼 = drugDispatchClasses[i].藥碼;
                    transactionsClass.藥品名稱 = drugDispatchClasses[i].藥名;
                    transactionsClass.庫存量 = drugDispatchClasses[i].入庫庫存;
                    transactionsClass.交易量 = drugDispatchClasses[i].入庫量;
                    transactionsClass.結存量 = drugDispatchClasses[i].入庫結存;
                    transactionsClass.操作人 = drugDispatchClasses[i].入庫人員;
                    transactionsClass.收支原因 = $"{出庫庫別} >> {入庫庫別}";
                    transactionsClass.備註 = 調入備註;

                    transactionsClass.add("http://127.0.0.1:4433", transactionsClass, 入庫庫別, "調劑台");
                })));
                tasks.Add(Task.Run(new Action(delegate
                {
                    transactionsClass transactionsClass = new transactionsClass();
                    transactionsClass.GUID = Guid.NewGuid().ToString();
                    transactionsClass.動作 = enum_交易記錄查詢動作.調出作業.GetEnumName();
                    transactionsClass.藥品碼 = drugDispatchClasses[i].藥碼;
                    transactionsClass.藥品名稱 = drugDispatchClasses[i].藥名;
                    transactionsClass.庫存量 = drugDispatchClasses[i].出庫庫存;
                    transactionsClass.交易量 = drugDispatchClasses[i].出庫量;
                    transactionsClass.結存量 = drugDispatchClasses[i].出庫結存;
                    transactionsClass.操作人 = drugDispatchClasses[i].出庫人員;
                    transactionsClass.收支原因 = $"{出庫庫別} >> {入庫庫別}";
                    transactionsClass.備註 = 調出備註;

                    transactionsClass.add("http://127.0.0.1:4433", transactionsClass, 出庫庫別, "調劑台");
                })));

                Task.WhenAll(tasks).Wait();

                drugDispatchClasses_buf.Add(drugDispatchClasses[i]);

            }

            return drugDispatchClasses_buf;

        }
        private List<object[]> Function_取得入庫儲位資訊(List<DeviceBasic> deviceBasics, List<object[]> 儲位資訊_Outbound)
        {
            List<object[]> 儲位資訊 = new List<object[]>();
            if (deviceBasics.Count == 0) return 儲位資訊;
            for (int i = 0; i < 儲位資訊_Outbound.Count; i++)
            {
                string 效期 = 儲位資訊_Outbound[i][(int)enum_儲位資訊.效期].ObjectToString();
                string 批號 = 儲位資訊_Outbound[i][(int)enum_儲位資訊.批號].ObjectToString();
                int 異動量 = 儲位資訊_Outbound[i][(int)enum_儲位資訊.異動量].StringToInt32() * -1;
                object[] value = new object[new enum_儲位資訊().GetLength()];
                value[(int)enum_儲位資訊.IP] = deviceBasics[0].IP;
                value[(int)enum_儲位資訊.TYPE] = deviceBasics[0].DeviceType;
                value[(int)enum_儲位資訊.效期] = 效期;
                value[(int)enum_儲位資訊.批號] = 批號;
                value[(int)enum_儲位資訊.庫存] = deviceBasics[0].Inventory;
                value[(int)enum_儲位資訊.異動量] = 異動量;
                value[(int)enum_儲位資訊.Value] = deviceBasics[0];
                儲位資訊.Add(value);
            }
            return 儲位資訊;
        }
        private List<object[]> Function_取得出庫儲位資訊(List<DeviceBasic> deviceBasics, double 異動量)
        {
            List<object[]> 儲位資訊_buf = new List<object[]>();
            List<object[]> 儲位資訊 = new List<object[]>();
            for (int k = 0; k < deviceBasics.Count; k++)
            {
                for (int i = 0; i < deviceBasics[k].List_Validity_period.Count; i++)
                {
                    object[] value = new object[new enum_儲位資訊().GetLength()];
                    value[(int)enum_儲位資訊.IP] = deviceBasics[k].IP;
                    value[(int)enum_儲位資訊.TYPE] = deviceBasics[k].DeviceType;
                    value[(int)enum_儲位資訊.效期] = deviceBasics[k].List_Validity_period[i];
                    value[(int)enum_儲位資訊.批號] = deviceBasics[k].List_Lot_number[i];
                    value[(int)enum_儲位資訊.庫存] = deviceBasics[k].List_Inventory[i];
                    value[(int)enum_儲位資訊.異動量] = "0";
                    value[(int)enum_儲位資訊.Value] = deviceBasics[k];
                    儲位資訊.Add(value);
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
        private object Function_庫存異動上傳(sys_serverSettingClass sys_serverSettingClass,object[] 儲位資訊)
        {
            string Server = sys_serverSettingClass.Server;
            string DBName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;

            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();
            SQLControl sQLControl_EPD583_serialize = new SQLControl(Server, DBName, "epd583_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_EPD266_serialize = new SQLControl(Server, DBName, "epd266_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RowsLED_serialize = new SQLControl(Server, DBName, "rowsled_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RFID_Device_serialize = new SQLControl(Server, DBName, "rfid_device_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_WT32_serialize = new SQLControl(Server, DBName, "WT32_Jsonstring", UserName, Password, Port, SSLMode);

            string IP = "";
            object Value = 儲位資訊[(int)enum_儲位資訊.Value];
            string 效期 = 儲位資訊[(int)enum_儲位資訊.效期].ObjectToString();
            string 異動量 = 儲位資訊[(int)enum_儲位資訊.異動量].ObjectToString();
            string TYPE = "";
            if (Value is DeviceBasic)
            {
                DeviceBasic deviceBasic = (DeviceBasic)Value;
                TYPE = deviceBasic.DeviceType.GetEnumName();
                IP = deviceBasic.IP;
                if (TYPE == DeviceType.EPD266.GetEnumName() || TYPE == DeviceType.EPD266_lock.GetEnumName() || TYPE == DeviceType.EPD290.GetEnumName() || TYPE == DeviceType.EPD290_lock.GetEnumName())
                {
                    Storage storage = StorageMethod.SQL_GetStorageByIP(sQLControl_EPD266_serialize, IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, true);
                        StorageMethod.SQL_ReplaceByIP(sQLControl_EPD266_serialize, storage);
                        return storage;
                    }
                }
                if (TYPE == DeviceType.Pannel35.GetEnumName() || TYPE == DeviceType.Pannel35_lock.GetEnumName())
                {
                    Storage storage = StorageMethod.SQL_GetStorageByIP(sQLControl_WT32_serialize, IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, true);
                        StorageMethod.SQL_ReplaceByIP(sQLControl_WT32_serialize, storage);
                        return storage;
                    }
                }
                if (TYPE == DeviceType.EPD583.GetEnumName() || TYPE == DeviceType.EPD583_lock.GetEnumName())
                {
                    Drawer drawer = DrawerMethod.SQL_GetDrawerByIP(sQLControl_EPD583_serialize, IP);
                    Box box = drawer.GetBox(deviceBasic.GUID);
                    if (box != null)
                    {
                        box.效期庫存異動(效期, 異動量, true);
                        drawer.ReplaceByGUID(box);
                        DrawerMethod.SQL_ReplaceByIP(sQLControl_EPD583_serialize, drawer);
                        return drawer;
                    }
                }
                if (TYPE == DeviceType.EPD1020.GetEnumName() || TYPE == DeviceType.EPD1020_lock.GetEnumName())
                {
                   
                }
                if (TYPE == DeviceType.RowsLED.GetEnumName())
                {
                    RowsLED rowsLED = RowsLEDMethod.SQL_GetRowsLEDByIP(sQLControl_RowsLED_serialize, IP);
                    RowsDevice rowsDevice = rowsLED.GetRowsDevice(deviceBasic.GUID);
                    if (rowsDevice != null)
                    {
                        rowsDevice.效期庫存異動(效期, 異動量, true);
                        rowsLED.ReplaceRowsDevice(rowsDevice);
                        RowsLEDMethod.SQL_ReplaceByIP(sQLControl_RowsLED_serialize, rowsLED);
                        return rowsLED;
                    }

                }
                if (TYPE == DeviceType.RFID_Device.GetEnumName())
                {
              
                }
            }
   
            return null;
        }
        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_drugDispatch());
            return table.JsonSerializationt(true);
        }
    }
}
