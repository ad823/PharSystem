using Basic;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using H_Pannel_lib;
using System.Drawing;
using System.Diagnostics;
using MyUI;
using HIS_DB_Lib;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutTakeMedController : ControllerBase
    {

        public enum enum_設備資料
        {
            GUID,
            名稱,
            顏色,
            備註,
        }

        /// <summary>
        /// 取藥、加退藥、入庫、撥調作業功能類型。
        /// </summary>
        /// <remarks>
        /// 外部 API 仍維持使用 class_OutTakeMed_data.功能類型 字串傳入，
        /// Controller 內部才轉換為此 enum，以降低外部系統相容性風險。
        /// 
        /// 功能類型對照：
        /// 1  = 取藥亮燈
        /// -1 = 取藥扣帳
        /// 2  = 取藥亮燈並扣帳
        /// -2 = 滅燈
        /// -3 = 加藥
        /// -4 = 退藥
        /// 5  = 入庫亮燈
        /// -5 = 入庫扣帳
        /// -6 = 撥入
        /// -7 = 撥出
        /// -8 = 調入
        /// -9 = 調出
        /// </remarks>
        private enum OutTakeMedOpType
        {
            /// <summary>
            /// 取藥亮燈。
            /// </summary>
            TakeLight = 1,

            /// <summary>
            /// 取藥扣帳。
            /// </summary>
            TakeDeduct = -1,

            /// <summary>
            /// 取藥亮燈並扣帳。
            /// </summary>
            TakeLightAndDeduct = 2,

            /// <summary>
            /// 清除指定電腦名稱資料，滅燈。
            /// </summary>
            ClearLight = -2,

            /// <summary>
            /// 加藥。
            /// </summary>
            AddDrug = -3,

            /// <summary>
            /// 退藥。
            /// </summary>
            ReturnDrug = -4,

            /// <summary>
            /// 入庫亮燈。
            /// </summary>
            StockInLight = 5,

            /// <summary>
            /// 入庫扣帳。
            /// </summary>
            StockInDeduct = -5,

            /// <summary>
            /// 撥入。
            /// </summary>
            TransferIn = -6,

            /// <summary>
            /// 撥出。
            /// </summary>
            TransferOut = -7,

            /// <summary>
            /// 調入。
            /// </summary>
            MoveIn = -8,

            /// <summary>
            /// 調出。
            /// </summary>
            MoveOut = -9,
        }

        /// <summary>
        /// mul_med_take 流程內部使用的執行環境。
        /// </summary>
        /// <remarks>
        /// 用於集中保存 ServerSetting、SQLControl、設備清單與本次操作設備資訊，
        /// 避免重構後各方法之間傳遞過多參數。
        /// </remarks>
        private class MulMedTakeContext
        {
            /// <summary>
            /// 本地端 ServerSetting。
            /// </summary>
            public sys_serverSettingClass LocalServerSetting { get; set; }

            /// <summary>
            /// VM端或人員資料 ServerSetting。
            /// </summary>
            public sys_serverSettingClass VmServerSetting { get; set; }

            /// <summary>
            /// trading 資料表 SQL 控制器。
            /// </summary>
            public SQLControl TradingSQL { get; set; }

            /// <summary>
            /// take_medicine_stack_new 資料表 SQL 控制器。
            /// </summary>
            public SQLControl TakeMedicineStackSQL { get; set; }

            /// <summary>
            /// devicelist 資料表 SQL 控制器。
            /// </summary>
            public SQLControl DeviceListSQL { get; set; }

            /// <summary>
            /// devicelist 全部資料。
            /// </summary>
            public List<object[]> DeviceListRows { get; set; } = new List<object[]>();

            /// <summary>
            /// 本次電腦名稱對應的 devicelist 資料。
            /// </summary>
            public List<object[]> CurrentDeviceRows { get; set; } = new List<object[]>();

            /// <summary>
            /// 本次電腦名稱目前存在的取藥堆疊資料。
            /// </summary>
            public List<object[]> CurrentTakeMedicineStackRows { get; set; } = new List<object[]>();

            /// <summary>
            /// 所有可用儲位設備資料。
            /// </summary>
            public List<DeviceBasic> Devices { get; set; } = new List<DeviceBasic>();

            /// <summary>
            /// 本次操作設備名稱，來源為 class_OutTakeMed_data.電腦名稱。
            /// </summary>
            public string DeviceName { get; set; }
        }

        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";

        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        MyTimer myTimer = new MyTimer(50000);


        [Route("init")]
        [HttpPost]
        public string GET_init([FromBody] returnData returnData)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "一般資料");
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
        [Route("statu")]
        [HttpGet()]
        public string Get_statu()
        {
            string jsonString = "";
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };


            return jsonString;
        }
        [Route("Sample")]
        [HttpGet()]
        public string Get_Sample()
        {
            string jsonString = "";
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            List<class_OutTakeMed_data> list_class_OutTakeMed_data = new List<class_OutTakeMed_data>();
            class_OutTakeMed_data class_OutTakeMed_Data01 = new class_OutTakeMed_data();
            class_OutTakeMed_Data01.PRI_KEY = Guid.NewGuid().ToString();
            class_OutTakeMed_Data01.電腦名稱 = "PC001";
            class_OutTakeMed_Data01.成本中心 = "1";
            class_OutTakeMed_Data01.來源庫別 = "UD1F";
            class_OutTakeMed_Data01.藥品碼 = "25003";
            class_OutTakeMed_Data01.類別 = "F";
            class_OutTakeMed_Data01.交易量 = "-1";
            class_OutTakeMed_Data01.操作人 = "王曉明";
            class_OutTakeMed_Data01.ID = "HS001";
            class_OutTakeMed_Data01.病人姓名 = "章大同";
            class_OutTakeMed_Data01.床號 = "34-06061";
            class_OutTakeMed_Data01.病歷號 = "00000000";
            class_OutTakeMed_Data01.開方時間 = DateTime.Now.ToDateTimeString();
            class_OutTakeMed_Data01.功能類型 = "1";
            list_class_OutTakeMed_data.Add(class_OutTakeMed_Data01);

            class_OutTakeMed_data class_OutTakeMed_Data02 = new class_OutTakeMed_data();
            class_OutTakeMed_Data02.PRI_KEY = Guid.NewGuid().ToString();
            class_OutTakeMed_Data02.電腦名稱 = "PC001";
            class_OutTakeMed_Data02.成本中心 = "1";
            class_OutTakeMed_Data02.來源庫別 = "UD1F";
            class_OutTakeMed_Data02.藥品碼 = "25004";
            class_OutTakeMed_Data02.類別 = "F";
            class_OutTakeMed_Data02.交易量 = "-1";
            class_OutTakeMed_Data02.操作人 = "王曉明";
            class_OutTakeMed_Data02.ID = "HS001";
            class_OutTakeMed_Data02.病人姓名 = "章大同";
            class_OutTakeMed_Data02.床號 = "34-06061";
            class_OutTakeMed_Data02.病歷號 = "00000000";
            class_OutTakeMed_Data02.開方時間 = DateTime.Now.ToDateTimeString();
            class_OutTakeMed_Data02.功能類型 = "1";
            list_class_OutTakeMed_data.Add(class_OutTakeMed_Data02);

            jsonString = list_class_OutTakeMed_data.JsonSerializationt(true);

            return jsonString;
        }
        [Route("new")]
        [HttpPost]
        public string Post([FromBody] returnData returnData)
        {
            string result = "";
            try
            {
                string json = returnData.Data.JsonSerializationt();
                List<class_OutTakeMed_data> data = returnData.Data.ObjToClass<List<class_OutTakeMed_data>>();
                if (data == null)
                {
                    result = "-1";
                }
                if (data.Count == 0)
                {
                    result = "-1";
                }
                if (data.Count == 1)
                {
                    result = mul_med_take(returnData.ServerName, data);
                }
                else
                {
                    result = mul_med_take(returnData.ServerName, data);
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            finally
            {
                string json_out = returnData.JsonSerializationt(true);
                Logger.LogAddLine($"OutTakeMed");
                Logger.Log($"OutTakeMed", $"result : {result} \n{json_out}");
                Logger.LogAddLine($"OutTakeMed");
            }
            return result;
        }
        [Route("{value}")]
        [HttpPost]
        public string Post([FromBody] List<class_OutTakeMed_data> data, string value)
        {
            string result = "";
            try
            {
                if (data == null)
                {
                    result = "-1";
                }
                if (data.Count == 0)
                {
                    result = "-1";
                }
                if (data.Count == 1)
                {
                    result = mul_med_take(value, data);
                }
                else
                {
                    result = mul_med_take(value, data);
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            finally
            {
                string json_out = data.JsonSerializationt(true);
                Logger.LogAddLine($"OutTakeMed");
                Logger.Log($"OutTakeMed", $"value : {value} , result : {result} \n{json_out}");
                Logger.LogAddLine($"OutTakeMed");
            }

            return result;

        }
        [Route("light_on")]
        [HttpPost]
        public string POST_light_on(returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();

                returnData.Method = "POST_light_on";
                string input_str = returnData.Value;
                if (input_str.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "Value空白,請輸入[藥碼,R,G,B,亮燈時間]!";
                    return returnData.JsonSerializationt();
                }
                string[] input_str_Ary = input_str.Split(",");
                if (input_str_Ary.Length != 5)
                {
                    returnData.Code = -200;
                    returnData.Result = "Value格式錯誤,請輸入[藥碼,R,G,B,亮燈時間]!";
                    return returnData.JsonSerializationt();
                }
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "一般資料");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "找無sys_serverSettingClass資料!";
                    return returnData.JsonSerializationt();
                }
                sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses[0];
                string IP = sys_serverSettingClass.Server;
                string DataBaseName = sys_serverSettingClass.DBName;
                string UserName = sys_serverSettingClass.User;
                string Password = sys_serverSettingClass.Password;
                uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();
                SQLControl sQLControl_take_medicine_stack = new SQLControl(IP, DataBaseName, "take_medicine_stack_new", UserName, Password, Port, SSLMode);
                string 藥碼 = input_str_Ary[0];
                byte R = (byte)(input_str_Ary[1].StringToInt32());
                byte G = (byte)(input_str_Ary[2].StringToInt32());
                byte B = (byte)(input_str_Ary[3].StringToInt32());
                int time = input_str_Ary[4].StringToInt32();
                object[] value = new object[new enum_取藥堆疊母資料().GetLength()];
                value[(int)enum_取藥堆疊母資料.GUID] = Guid.NewGuid();
                value[(int)enum_取藥堆疊母資料.序號] = DateTime.Now.ToDateTimeString_6();
                value[(int)enum_取藥堆疊母資料.藥品碼] = 藥碼;
                value[(int)enum_取藥堆疊母資料.調劑台名稱] = "儲位亮燈";

                value[(int)enum_取藥堆疊母資料.開方時間] = DateTime.Now.ToDateTimeString();
                value[(int)enum_取藥堆疊母資料.操作時間] = DateTime.Now.ToDateTimeString();
                value[(int)enum_取藥堆疊母資料.顏色] = Color.FromArgb(R, G, B).ToColorString();
                value[(int)enum_取藥堆疊母資料.狀態] = "None";
                value[(int)enum_取藥堆疊母資料.總異動量] = time;




                sQLControl_take_medicine_stack.AddRow(null, value);

                returnData.Data = "";
                returnData.Code = 200;
                returnData.Result = $"亮燈完成! 藥碼:{input_str_Ary[0]},Color({input_str_Ary[1]},{input_str_Ary[2]},{input_str_Ary[3]})";
                returnData.TimeTaken = myTimerBasic.ToString();
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
        /// 設定扣帳資訊
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "ServerName" : "A6",
        ///     "ServerType" : "調劑台",
        ///     "ValueAry" : 
        ///     [
        ///       
        ///     ],
        ///     "Data" : 
        ///     {
        ///        [takeMedicineStackClass]
        ///     }
        ///     
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為[transactionsClass]陣列結構</returns>
        [Route("set_device_tradding")]
        [HttpPost]
        public string POST_set_device_tradding(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(50000);
            try
            {
                returnData.RequestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            }
            catch
            {

                returnData.Method = "set_device_tradding";
            }
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                List<sys_serverSettingClass> sys_serverSettingClasses_buf = new List<sys_serverSettingClass>();
                sys_serverSettingClasses_buf = sys_serverSettingClasses.MyFind(returnData.ServerName, returnData.ServerType, "儲位資料");
                sys_serverSettingClass sys_serverSettingClass_儲位資料 = sys_serverSettingClasses_buf[0];
                if (sys_serverSettingClass_儲位資料 == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                List<takeMedicineStackClass> takeMedicineStackClasses = returnData.Data.ObjToClass<List<takeMedicineStackClass>>();
                if (takeMedicineStackClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料異常";
                    return returnData.JsonSerializationt();
                }
                if (takeMedicineStackClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入資料空白";
                    return returnData.JsonSerializationt();
                }
                List<DeviceBasic> deviceBasics = deviceController.Function_Get_device(sys_serverSettingClass_儲位資料, returnData.TableName);
                Dictionary<string, List<DeviceBasic>> keyValuePairs = deviceBasics.CoverToDictionaryByCode();

                string GUID = "";
                string Master_GUID = "";
                double 庫存量 = 0;
                double 結存量 = 0;
                double 總異動量 = 0;
                string 盤點量 = "";
                string 動作 = "";
                string 藥品碼 = "";
                string 藥品名稱 = "";
                string 藥袋序號 = "";
                string 類別 = "";
                string 交易量 = "";
                string 操作人 = "";
                string 病人姓名 = "";
                string 床號 = "";
                string 頻次 = "";
                string 病歷號 = "";
                string 操作時間 = "";
                string 開方時間 = "";
                string 備註 = "";
                string 收支原因 = "";
                string 診別 = "";
                string 藥師證字號 = "";
                string 效期 = "";
                string 批號 = "";
                string 顏色 = "";
                string 領藥號 = "";
                string 病房號 = "";
                string 醫令_GUID = "";
                string 交易紀錄_GUID = "";

                List<DeviceBasic> deviceBasics_buf = new List<DeviceBasic>();
                List<DeviceBasic> deviceBasics_result = new List<DeviceBasic>();
                List<transactionsClass> transactionsClasses = new List<transactionsClass>();
                for (int i = 0; i < takeMedicineStackClasses.Count; i++)
                {
                    deviceBasics_buf = keyValuePairs.SortDictionaryByCode(takeMedicineStackClasses[i].藥品碼);
                    if (deviceBasics_buf.Count == 0)
                    {
                        continue;
                    }
                    transactionsClass _transactionsClass = new transactionsClass();

                    庫存量 = deviceBasics_buf.GetInventory();
                    總異動量 = takeMedicineStackClasses[i].總異動量.StringToInt32();
                    結存量 = (庫存量 + 總異動量);
                   

                    List<object[]> list_儲位資訊 = deviceController.Function_取得異動儲位資訊(deviceBasics_buf, takeMedicineStackClasses[i].藥品碼, 總異動量);

                    for (int k = 0; k < list_儲位資訊.Count; k++)
                    {

                        deviceController.Function_庫存異動(list_儲位資訊[k], sys_serverSettingClass_儲位資料);
                        備註 += $"[效期]:{list_儲位資訊[k][(int)deviceController.enum_儲位資訊.效期].ObjectToString()},[批號]:{list_儲位資訊[k][(int)deviceController.enum_儲位資訊.批號].ObjectToString()}";
                        if (k != list_儲位資訊.Count - 1) 備註 += "\n";
                    }

                    _transactionsClass.GUID = Guid.NewGuid().ToString();
                    _transactionsClass.動作 = takeMedicineStackClasses[i].動作.GetEnumName();
                    _transactionsClass.診別 = takeMedicineStackClasses[i].診別;
                    _transactionsClass.藥品碼 = takeMedicineStackClasses[i].藥品碼;
                    _transactionsClass.藥品名稱 = takeMedicineStackClasses[i].藥品名稱;
                    _transactionsClass.藥袋序號 = takeMedicineStackClasses[i].藥袋序號;
                    _transactionsClass.藥師證字號 = takeMedicineStackClasses[i].藥師證字號;
                    _transactionsClass.領藥號 = takeMedicineStackClasses[i].領藥號;
                    _transactionsClass.病房號 = takeMedicineStackClasses[i].病房號;
                    _transactionsClass.類別 = takeMedicineStackClasses[i].類別;
                    _transactionsClass.庫存量 = 庫存量.ToString();
                    _transactionsClass.交易量 = 總異動量.ToString();
                    _transactionsClass.結存量 = 結存量.ToString();
                    _transactionsClass.盤點量 = takeMedicineStackClasses[i].盤點量;
                    _transactionsClass.操作人 = takeMedicineStackClasses[i].操作人;
                    _transactionsClass.病人姓名 = takeMedicineStackClasses[i].病人姓名;
                    _transactionsClass.床號 = takeMedicineStackClasses[i].床號;
                    _transactionsClass.頻次 = takeMedicineStackClasses[i].頻次;
                    _transactionsClass.病歷號 = takeMedicineStackClasses[i].病歷號;
                    _transactionsClass.操作時間 = DateTime.Now.ToDateTimeString_6();
                    if (開方時間.StringIsEmpty()) 開方時間 = DateTime.Now.ToDateTimeString_6();
                    _transactionsClass.開方時間 = takeMedicineStackClasses[i].開方時間;
                    _transactionsClass.備註 = takeMedicineStackClasses[i].備註;
                    收支原因 = $"{收支原因}";
                    _transactionsClass.收支原因 = takeMedicineStackClasses[i].收支原因;

                    transactionsClass.add("http://127.0.0.1:4433", _transactionsClass, returnData.ServerName, returnData.ServerType);

                    transactionsClasses.Add(_transactionsClass);
                }
           

                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Code = 200;
                returnData.Result = $"設定扣帳資訊,共<{transactionsClasses.Count}>筆資料,TableName : {returnData.TableName}";
                returnData.Data = transactionsClasses;

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

        #region Function

        /// <summary>
        /// 批次處理多筆藥品異動資料，依功能類型建立取藥堆疊母資料並回傳處理結果。
        /// </summary>
        /// <remarks>
        /// 此方法為 OutTakeMedController 的核心資料入口，
        /// 主要負責將外部傳入的 class_OutTakeMed_data 轉換為 takeMedicineStackClass，
        /// 並寫入 take_medicine_stack_new 資料表。
        /// 
        /// 外部相容規則：
        /// 1. API 路由不變。
        /// 2. returnData 格式不變。
        /// 3. class_OutTakeMed_data 欄位名稱不變。
        /// 4. 功能類型仍維持使用字串傳入。
        /// 5. Controller 內部才轉換為 OutTakeMedOpType enum。
        /// 
        /// 功能類型：
        /// 1  = 取藥亮燈
        /// -1 = 取藥扣帳
        /// 2  = 取藥亮燈並扣帳
        /// -2 = 滅燈
        /// -3 = 加藥
        /// -4 = 退藥
        /// 5  = 入庫亮燈
        /// -5 = 入庫扣帳
        /// -6 = 撥入
        /// -7 = 撥出
        /// -8 = 調入
        /// -9 = 調出
        /// 
        /// 交易量規則：
        /// 1. 所有功能類型皆需為數字。
        /// 2. -1、2、-7、-9 必須為負數。
        /// 3. 其他功能類型暫不限制正負，只檢查是否為數字。
        /// 
        /// 特殊規則：
        /// -2 滅燈為獨立資料，會直接清除指定電腦名稱的 take_medicine_stack_new 資料後回傳。
        /// </remarks>
        /// <param name="name">調劑台名稱，通常對應 returnData.ServerName 或路由 value。</param>
        /// <param name="data">藥品異動資料清單。</param>
        /// <returns>returnData JSON 字串。</returns>
        private string mul_med_take(string name, List<class_OutTakeMed_data> data)
        {
            returnData returnData = new returnData();
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            returnData.Method = "mul_med_take";

            string validateResult = ValidateMulMedTakeInput(data, returnData);
            if (validateResult.StringIsEmpty() == false)
            {
                return validateResult;
            }

            MulMedTakeContext context = LoadMulMedTakeContext(name, data, returnData);
            if (context == null)
            {
                return returnData.JsonSerializationt(true);
            }

            OutTakeMedOpType firstOpType;
            if (TryParseOutTakeMedOpType(data[0].功能類型, out firstOpType))
            {
                if (firstOpType == OutTakeMedOpType.ClearLight)
                {
                    return HandleClearLight(context, returnData);
                }
            }

            List<takeMedicineStackClass> takeMedicineStackClasses = BuildTakeMedicineStackClasses(context, data, returnData);
            if (returnData.Code == -200)
            {
                return returnData.JsonSerializationt(true);
            }

            Function_取藥堆疊資料_新增母資料(context.LocalServerSetting, context.DeviceName, takeMedicineStackClasses);

            returnData.Code = 200;
            returnData.TimeTaken = $"{myTimerBasic}";
            returnData.Result = $"OK,共新增<{takeMedicineStackClasses.Count}筆資料!>";
            return returnData.JsonSerializationt(true);
        }

        /// <summary>
        /// 驗證 mul_med_take 傳入資料。
        /// </summary>
        /// <remarks>
        /// 驗證項目：
        /// 1. data 不可為 null。
        /// 2. data 筆數不可為 0。
        /// 3. 交易量必須為數字。
        /// 4. 藥品碼不可空白。
        /// 5. 操作人不可空白。
        /// 6. 電腦名稱若空白，預設為 System。
        /// 7. 功能類型不可空白且必須可轉換為 OutTakeMedOpType。
        /// 8. -1、2、-7、-9 的交易量必須為負數。
        /// 9. 開方時間若不是合法日期，使用目前時間補入。
        /// </remarks>
        /// <param name="data">藥品異動資料清單。</param>
        /// <param name="returnData">回傳資料物件。</param>
        /// <returns>若驗證失敗回傳 returnData JSON 字串；成功則回傳空字串。</returns>
        private string ValidateMulMedTakeInput(List<class_OutTakeMed_data> data, returnData returnData)
        {
            if (data == null)
            {
                returnData.Code = -200;
                returnData.Result = $"傳入資料空白";
                return returnData.JsonSerializationt(true);
            }

            if (data.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"傳入資料空白";
                return returnData.JsonSerializationt(true);
            }

            for (int i = 0; i < data.Count; i++)
            {
                if (!data[i].交易量.StringIsInt32())
                {
                    returnData.Code = -200;
                    returnData.Result = $"交易量不得為非數字";
                    return returnData.JsonSerializationt(true);
                }

                if (data[i].藥品碼.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥品碼(code)空白";
                    return returnData.JsonSerializationt(true);
                }

                if (data[i].操作人.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"操作人(operator)空白";
                    return returnData.JsonSerializationt(true);
                }

                if (data[i].電腦名稱.StringIsEmpty())
                {
                    data[i].電腦名稱 = "System";
                }

                if (data[i].功能類型.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"類別(op_type)空白";
                    return returnData.JsonSerializationt(true);
                }

                OutTakeMedOpType opType;
                if (!TryParseOutTakeMedOpType(data[i].功能類型, out opType))
                {
                    returnData.Code = -200;
                    returnData.Result = $"類別(op_type)錯誤";
                    return returnData.JsonSerializationt(true);
                }

                int tradeQty = data[i].交易量.StringToInt32();

                if (IsNegativeRequiredOpType(opType) && tradeQty >= 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"交易量必須為負數";
                    return returnData.JsonSerializationt(true);
                }

                if (!data[i].開方時間.Check_Date_String())
                {
                    data[i].開方時間 = DateTime.Now.ToDateTimeString();
                }
            }

            return "";
        }

        /// <summary>
        /// 將外部傳入的功能類型字串轉換為內部 OutTakeMedOpType enum。
        /// </summary>
        /// <remarks>
        /// 外部系統仍維持傳入字串，例如 "1"、"-1"、"2"、"-2"。
        /// 此方法只負責轉換與檢查該數值是否定義於 OutTakeMedOpType。
        /// </remarks>
        /// <param name="opTypeText">功能類型字串。</param>
        /// <param name="opType">轉換後的功能類型。</param>
        /// <returns>轉換成功回傳 true；失敗回傳 false。</returns>
        private bool TryParseOutTakeMedOpType(string opTypeText, out OutTakeMedOpType opType)
        {
            opType = default(OutTakeMedOpType);

            if (!opTypeText.StringIsInt32())
            {
                return false;
            }

            int value = opTypeText.StringToInt32();

            if (!Enum.IsDefined(typeof(OutTakeMedOpType), value))
            {
                return false;
            }

            opType = (OutTakeMedOpType)value;
            return true;
        }

        /// <summary>
        /// 判斷指定功能類型的交易量是否必須為負數。
        /// </summary>
        /// <remarks>
        /// 依目前規則，以下功能類型必須為負數：
        /// -1 = 取藥扣帳
        /// 2  = 取藥亮燈並扣帳
        /// -7 = 撥出
        /// -9 = 調出
        /// </remarks>
        /// <param name="opType">功能類型。</param>
        /// <returns>若該功能類型交易量必須為負數，回傳 true。</returns>
        private bool IsNegativeRequiredOpType(OutTakeMedOpType opType)
        {
            return opType == OutTakeMedOpType.TakeDeduct ||
                   opType == OutTakeMedOpType.TakeLightAndDeduct ||
                   opType == OutTakeMedOpType.TransferOut ||
                   opType == OutTakeMedOpType.MoveOut;
        }

        /// <summary>
        /// 載入 mul_med_take 流程所需的 ServerSetting、SQLControl、設備清單與指定電腦名稱資料。
        /// </summary>
        /// <remarks>
        /// 此方法集中處理：
        /// 1. 取得本地端 ServerSetting。
        /// 2. 取得 VM端 ServerSetting。
        /// 3. 建立 trading SQLControl。
        /// 4. 建立 take_medicine_stack_new SQLControl。
        /// 5. 建立 devicelist SQLControl。
        /// 6. 取得或建立電腦名稱對應的 devicelist 資料。
        /// 7. 讀取儲位設備資料。
        /// 8. 讀取目前電腦名稱既有的 take_medicine_stack_new 資料。
        /// </remarks>
        /// <param name="name">調劑台名稱。</param>
        /// <param name="data">藥品異動資料清單。</param>
        /// <param name="returnData">回傳資料物件。</param>
        /// <returns>mul_med_take 執行環境；失敗時回傳 null。</returns>
        private MulMedTakeContext LoadMulMedTakeContext(string name, List<class_OutTakeMed_data> data, returnData returnData)
        {
            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

            sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses.MyFind(
                name,
                enum_sys_serverSetting_Type.調劑台,
                enum_sys_serverSetting_調劑台.本地端
            );

            sys_serverSettingClass sys_serverSettingClass_人員資料 = sys_serverSettingClasses.MyFind(
                name,
                enum_sys_serverSetting_Type.調劑台,
                enum_sys_serverSetting_調劑台.VM端
            );

            if (sys_serverSettingClass == null)
            {
                returnData.Code = -200;
                returnData.Result = "sys_serverSettingClass[一般資料] is null!";
                return null;
            }

            if (sys_serverSettingClass_人員資料 == null)
            {
                returnData.Code = -200;
                returnData.Result = "sys_serverSettingClass[人員資料] is null!";
                return null;
            }

            string IP = sys_serverSettingClass.Server;
            string DataBaseName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            string devicelist_IP = sys_serverSettingClass_人員資料.Server;
            string devicelist_database = sys_serverSettingClass_人員資料.DBName;

            MulMedTakeContext context = new MulMedTakeContext();

            context.LocalServerSetting = sys_serverSettingClass;
            context.VmServerSetting = sys_serverSettingClass_人員資料;

            context.TradingSQL = new SQLControl(IP, DataBaseName, "trading", UserName, Password, Port, SSLMode);
            context.TakeMedicineStackSQL = new SQLControl(IP, DataBaseName, "take_medicine_stack_new", UserName, Password, Port, SSLMode);
            context.DeviceListSQL = new SQLControl(
                devicelist_IP,
                devicelist_database,
                "devicelist",
                UserName,
                Password,
                sys_serverSettingClass_人員資料.Port.StringToUInt32(),
                SSLMode
            );

            context.DeviceName = data[0].電腦名稱;

            context.DeviceListRows = context.DeviceListSQL.GetAllRows(null);
            context.CurrentDeviceRows = context.DeviceListRows.GetRows((int)enum_設備資料.名稱, context.DeviceName);

            if (context.CurrentDeviceRows.Count == 0)
            {
                object[] value = new object[new enum_設備資料().GetLength()];
                value[(int)enum_設備資料.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_設備資料.名稱] = context.DeviceName;

                Color color = this.Function_取得顏色(context.DeviceListRows.Count);
                value[(int)enum_設備資料.顏色] = color.ToColorString();

                context.DeviceListSQL.AddRow(null, value);
                context.CurrentDeviceRows.Add(value);
            }

            context.Devices = this.Function_讀取儲位(name);

            context.CurrentTakeMedicineStackRows = context.TakeMedicineStackSQL.GetRowsByDefult(
                null,
                (int)enum_取藥堆疊母資料.調劑台名稱,
                context.DeviceName
            );

            return context;
        }

        /// <summary>
        /// 處理滅燈功能，清除指定電腦名稱的取藥堆疊母資料。
        /// </summary>
        /// <remarks>
        /// 功能類型 -2 為獨立資料，不與其他功能類型混合批次處理。
        /// 此方法會刪除 take_medicine_stack_new 中指定調劑台名稱的資料。
        /// </remarks>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <param name="returnData">回傳資料物件。</param>
        /// <returns>returnData JSON 字串。</returns>
        private string HandleClearLight(MulMedTakeContext context, returnData returnData)
        {
            if (context.CurrentTakeMedicineStackRows.Count > 0)
            {
                context.TakeMedicineStackSQL.DeleteExtra(null, context.CurrentTakeMedicineStackRows);
            }

            returnData.Code = 200;
            returnData.Result = $"清除指定電腦名稱資料(滅燈)成功";
            return returnData.JsonSerializationt(true);
        }

        /// <summary>
        /// 根據傳入資料批次建立取藥堆疊母資料清單。
        /// </summary>
        /// <remarks>
        /// 此方法只負責分流功能類型，實際建立資料由各 Builder 方法負責。
        /// </remarks>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <param name="data">藥品異動資料清單。</param>
        /// <param name="returnData">回傳資料物件。</param>
        /// <returns>取藥堆疊母資料清單。</returns>
        private List<takeMedicineStackClass> BuildTakeMedicineStackClasses(
            MulMedTakeContext context,
            List<class_OutTakeMed_data> data,
            returnData returnData)
        {
            List<takeMedicineStackClass> takeMedicineStackClasses = new List<takeMedicineStackClass>();

            for (int i = 0; i < data.Count; i++)
            {
                string date_str = $"{data[i].日期} {data[i].時間}";
                if (date_str.Check_Date_String())
                {
                    data[i].開方時間 = date_str;
                }

                OutTakeMedOpType opType;
                if (!TryParseOutTakeMedOpType(data[i].功能類型, out opType))
                {
                    returnData.Code = -200;
                    returnData.Result = $"類別(op_type)錯誤";
                    return takeMedicineStackClasses;
                }

                takeMedicineStackClass item = null;

                if (opType == OutTakeMedOpType.TakeLight ||
                    opType == OutTakeMedOpType.TakeDeduct ||
                    opType == OutTakeMedOpType.TakeLightAndDeduct)
                {
                    item = BuildTakeDrugStack(context, data[i], opType, returnData);
                }
                else if (opType == OutTakeMedOpType.AddDrug ||
                         opType == OutTakeMedOpType.ReturnDrug)
                {
                    item = BuildAddOrReturnDrugStack(context, data[i], opType, returnData);
                }
                else if (opType == OutTakeMedOpType.StockInLight ||
                         opType == OutTakeMedOpType.StockInDeduct)
                {
                    item = BuildStockInStack(context, data[i], opType, returnData);
                }
                else if (opType == OutTakeMedOpType.TransferIn ||
                         opType == OutTakeMedOpType.TransferOut ||
                         opType == OutTakeMedOpType.MoveIn ||
                         opType == OutTakeMedOpType.MoveOut)
                {
                    item = BuildTransferOrMoveStack(context, data[i], opType, returnData);
                }

                if (returnData.Code == -200)
                {
                    return takeMedicineStackClasses;
                }

                if (item != null)
                {
                    takeMedicineStackClasses.Add(item);
                }
            }

            return takeMedicineStackClasses;
        }

        /// <summary>
        /// 建立取藥類型的取藥堆疊母資料。
        /// </summary>
        /// <remarks>
        /// 對應功能類型：
        /// 1  = 取藥亮燈
        /// -1 = 取藥扣帳
        /// 2  = 取藥亮燈並扣帳
        /// 
        /// 此類型沿用傳入資料中的藥名與單位，
        /// 若顏色空白則使用 devicelist 中該電腦名稱的顏色。
        /// -1 取藥扣帳時顏色設定為黑色。
        /// </remarks>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <param name="data">單筆藥品異動資料。</param>
        /// <param name="opType">功能類型。</param>
        /// <param name="returnData">回傳資料物件。</param>
        /// <returns>取藥堆疊母資料；若失敗則回傳 null。</returns>
        private takeMedicineStackClass BuildTakeDrugStack(
            MulMedTakeContext context,
            class_OutTakeMed_data data,
            OutTakeMedOpType opType,
            returnData returnData)
        {
            int totalQty = data.交易量.StringToInt32();

            string priKey = ResolvePriKeyAndCheckDuplicate(context, data, totalQty, returnData);
            if (returnData.Code == -200) return null;

            NormalizeCommonTextFields(data);

            string color = data.顏色;
            if (color.StringIsEmpty())
            {
                color = GetCurrentDeviceColor(context);
            }

            if (opType == OutTakeMedOpType.TakeDeduct)
            {
                color = Color.Black.ToColorString();
            }

            return CreateBaseTakeMedicineStack(
                context,
                data,
                priKey,
                totalQty,
                data.藥品碼,
                data.藥名,
                data.單位,
                enum_交易記錄查詢動作.系統領藥.GetEnumName(),
                color,
                data.收支原因
            );
        }

        /// <summary>
        /// 建立加藥或退藥類型的取藥堆疊母資料。
        /// </summary>
        /// <remarks>
        /// 對應功能類型：
        /// -3 = 加藥
        /// -4 = 退藥
        /// 
        /// 此類型會依藥品碼從儲位資料取得藥名與單位。
        /// 若找不到儲位資料，沿用舊版行為，直接略過該筆。
        /// </remarks>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <param name="data">單筆藥品異動資料。</param>
        /// <param name="opType">功能類型。</param>
        /// <param name="returnData">回傳資料物件。</param>
        /// <returns>取藥堆疊母資料；若略過或失敗則回傳 null。</returns>
        private takeMedicineStackClass BuildAddOrReturnDrugStack(
            MulMedTakeContext context,
            class_OutTakeMed_data data,
            OutTakeMedOpType opType,
            returnData returnData)
        {
            List<DeviceBasic> list_device = GetDeviceByCode(context, data.藥品碼);
            if (list_device.Count == 0)
            {
                return null;
            }

            int totalQty = data.交易量.StringToInt32();

            string priKey = ResolvePriKeyAndCheckDuplicate(context, data, totalQty, returnData);
            if (returnData.Code == -200) return null;

            string expirationDate = data.效期;
            if (expirationDate.Check_Date_String() == false)
            {
                expirationDate = "";
            }

            string reason = data.收支原因;
            if (reason.StringIsEmpty() == false) reason += "\n";
            if (data.加退藥來源.StringIsEmpty() == false) reason += $"[加退藥來源]:{data.加退藥來源}\n";
            if (data.護理站.StringIsEmpty() == false) reason += $"[護理站]:{data.護理站}";

            string drugName = list_device[0].Name;
            string unit = list_device[0].Package;

            if (drugName != null) drugName = drugName.Trim();
            if (unit != null) unit = unit.Trim();

            NormalizeCommonTextFields(data);

            string action = "";

            if (opType == OutTakeMedOpType.AddDrug)
            {
                action = enum_交易記錄查詢動作.系統加藥.GetEnumName();
            }
            else if (opType == OutTakeMedOpType.ReturnDrug)
            {
                action = enum_交易記錄查詢動作.系統退藥.GetEnumName();
            }

            return CreateBaseTakeMedicineStack(
                context,
                data,
                priKey,
                totalQty,
                data.藥品碼,
                drugName,
                unit,
                action,
                Color.Black.ToColorString(),
                reason,
                expirationDate,
                data.批號
            );
        }

        /// <summary>
        /// 建立入庫類型的取藥堆疊母資料。
        /// </summary>
        /// <remarks>
        /// 對應功能類型：
        /// 5  = 入庫亮燈
        /// -5 = 入庫扣帳
        /// 
        /// 此類型會依藥品碼從儲位資料取得藥名與單位。
        /// 若找不到儲位資料，沿用舊版行為，直接略過該筆。
        /// -5 入庫扣帳時顏色設定為黑色。
        /// </remarks>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <param name="data">單筆藥品異動資料。</param>
        /// <param name="opType">功能類型。</param>
        /// <param name="returnData">回傳資料物件。</param>
        /// <returns>取藥堆疊母資料；若略過或失敗則回傳 null。</returns>
        private takeMedicineStackClass BuildStockInStack(
            MulMedTakeContext context,
            class_OutTakeMed_data data,
            OutTakeMedOpType opType,
            returnData returnData)
        {
            List<DeviceBasic> list_device = GetDeviceByCode(context, data.藥品碼);
            if (list_device.Count == 0)
            {
                return null;
            }

            int totalQty = data.交易量.StringToInt32();

            string priKey = ResolvePriKeyAndCheckDuplicate(context, data, totalQty, returnData);
            if (returnData.Code == -200) return null;

            string expirationDate = data.效期;
            if (expirationDate.Check_Date_String() == false)
            {
                expirationDate = "";
            }

            string drugName = list_device[0].Name;
            string unit = list_device[0].Package;

            if (drugName != null) drugName = drugName.Trim();
            if (unit != null) unit = unit.Trim();

            NormalizeCommonTextFields(data);

            string color = GetCurrentDeviceColor(context);
            if (opType == OutTakeMedOpType.StockInDeduct)
            {
                color = Color.Black.ToColorString();
            }

            return CreateBaseTakeMedicineStack(
                context,
                data,
                priKey,
                totalQty,
                data.藥品碼,
                drugName,
                unit,
                enum_交易記錄查詢動作.系統入庫.GetEnumName(),
                color,
                data.收支原因,
                expirationDate,
                data.批號
            );
        }

        /// <summary>
        /// 建立撥入、撥出、調入、調出類型的取藥堆疊母資料。
        /// </summary>
        /// <remarks>
        /// 對應功能類型：
        /// -6 = 撥入
        /// -7 = 撥出
        /// -8 = 調入
        /// -9 = 調出
        /// 
        /// 此類型會依藥品碼從儲位資料取得藥名與單位。
        /// 若找不到儲位資料，沿用舊版行為，直接略過該筆。
        /// 撥調類型顏色一律設定為黑色。
        /// </remarks>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <param name="data">單筆藥品異動資料。</param>
        /// <param name="opType">功能類型。</param>
        /// <param name="returnData">回傳資料物件。</param>
        /// <returns>取藥堆疊母資料；若略過或失敗則回傳 null。</returns>
        private takeMedicineStackClass BuildTransferOrMoveStack(
            MulMedTakeContext context,
            class_OutTakeMed_data data,
            OutTakeMedOpType opType,
            returnData returnData)
        {
            List<DeviceBasic> list_device = GetDeviceByCode(context, data.藥品碼);
            if (list_device.Count == 0)
            {
                return null;
            }

            int totalQty = data.交易量.StringToInt32();

            string priKey = ResolvePriKeyAndCheckDuplicate(context, data, totalQty, returnData);
            if (returnData.Code == -200) return null;

            string expirationDate = data.效期;
            if (expirationDate.Check_Date_String() == false)
            {
                expirationDate = "";
            }

            string reason = data.收支原因;
            if (reason.StringIsEmpty() == false) reason += "\n";
            if (data.來源庫別.StringIsEmpty() == false) reason += $"[來源庫別]:{data.來源庫別}";

            string drugName = list_device[0].Name;
            string unit = list_device[0].Package;

            if (drugName != null) drugName = drugName.Trim();
            if (unit != null) unit = unit.Trim();

            NormalizeCommonTextFields(data);

            string action = "";

            if (opType == OutTakeMedOpType.TransferIn)
            {
                action = enum_交易記錄查詢動作.系統撥入.GetEnumName();
            }
            else if (opType == OutTakeMedOpType.TransferOut)
            {
                action = enum_交易記錄查詢動作.系統撥出.GetEnumName();
            }
            else if (opType == OutTakeMedOpType.MoveIn)
            {
                action = enum_交易記錄查詢動作.系統調入.GetEnumName();
            }
            else if (opType == OutTakeMedOpType.MoveOut)
            {
                action = enum_交易記錄查詢動作.系統調出.GetEnumName();
            }

            return CreateBaseTakeMedicineStack(
                context,
                data,
                priKey,
                totalQty,
                data.藥品碼,
                drugName,
                unit,
                action,
                Color.Black.ToColorString(),
                reason,
                expirationDate,
                data.批號
            );
        }

        /// <summary>
        /// 處理 PRI_KEY，並檢查 trading 是否已有相同藥袋序號。
        /// </summary>
        /// <remarks>
        /// 舊版邏輯：
        /// 1. PRI_KEY 空白時補 Guid。
        /// 2. 總異動量不為 0 時，檢查 trading 是否已有相同藥袋序號。
        /// 3. 若重複則回傳錯誤。
        /// 4. 總異動量為 0 時，重新產生新的 PRI_KEY。
        /// </remarks>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <param name="data">單筆藥品異動資料。</param>
        /// <param name="totalQty">總異動量。</param>
        /// <param name="returnData">回傳資料物件。</param>
        /// <returns>可使用的 PRI_KEY；若驗證失敗回傳空字串。</returns>
        private string ResolvePriKeyAndCheckDuplicate(
            MulMedTakeContext context,
            class_OutTakeMed_data data,
            int totalQty,
            returnData returnData)
        {
            if (data.PRI_KEY.StringIsEmpty())
            {
                data.PRI_KEY = Guid.NewGuid().ToString();
            }

            string priKey = data.PRI_KEY;

            if (totalQty != 0)
            {
                List<object[]> list_trading = context.TradingSQL.GetRowsByDefult(
                    null,
                    (int)enum_交易記錄查詢資料.藥袋序號,
                    data.PRI_KEY
                );

                if (list_trading.Count > 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"有重複領取序號,PRI_KEY:{data.PRI_KEY}";
                    return "";
                }
            }
            else
            {
                priKey = Guid.NewGuid().ToString();
            }

            return priKey;
        }

        /// <summary>
        /// 依藥品碼取得儲位設備資料。
        /// </summary>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <param name="code">藥品碼。</param>
        /// <returns>符合藥品碼的設備資料清單。</returns>
        private List<DeviceBasic> GetDeviceByCode(MulMedTakeContext context, string code)
        {
            return context.Devices.SortByCode(code);
        }

        /// <summary>
        /// 取得目前設備對應的顏色。
        /// </summary>
        /// <remarks>
        /// 若找不到 devicelist 對應資料，預設回傳紅色。
        /// </remarks>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <returns>顏色字串。</returns>
        private string GetCurrentDeviceColor(MulMedTakeContext context)
        {
            if (context.CurrentDeviceRows == null || context.CurrentDeviceRows.Count == 0)
            {
                return Color.Red.ToColorString();
            }

            return context.CurrentDeviceRows[0][(int)enum_設備資料.顏色].ObjectToString();
        }

        /// <summary>
        /// 標準化常用文字欄位。
        /// </summary>
        /// <remarks>
        /// 去除前後空白，避免病歷號、姓名、操作人、床號、領藥號、類別等欄位因空白造成資料不一致。
        /// </remarks>
        /// <param name="data">單筆藥品異動資料。</param>
        private void NormalizeCommonTextFields(class_OutTakeMed_data data)
        {
            if (data.病歷號 != null) data.病歷號 = data.病歷號.Trim();
            if (data.病人姓名 != null) data.病人姓名 = data.病人姓名.Trim();
            if (data.操作人 != null) data.操作人 = data.操作人.Trim();
            if (data.床號 != null) data.床號 = data.床號.Trim();
            if (data.領藥號 != null) data.領藥號 = data.領藥號.Trim();
            if (data.類別 != null) data.類別 = data.類別.Trim();
        }

        /// <summary>
        /// 建立取藥堆疊母資料共用物件。
        /// </summary>
        /// <remarks>
        /// 此方法集中設定 takeMedicineStackClass 的共同欄位，
        /// 各功能類型 Builder 僅需提供差異欄位，例如動作、顏色、藥名、單位、收支原因、效期與批號。
        /// </remarks>
        /// <param name="context">mul_med_take 執行環境。</param>
        /// <param name="data">單筆藥品異動資料。</param>
        /// <param name="priKey">藥袋序號。</param>
        /// <param name="totalQty">總異動量。</param>
        /// <param name="drugCode">藥品碼。</param>
        /// <param name="drugName">藥品名稱。</param>
        /// <param name="unit">單位。</param>
        /// <param name="action">動作。</param>
        /// <param name="color">顏色。</param>
        /// <param name="reason">收支原因。</param>
        /// <param name="expirationDate">效期。</param>
        /// <param name="lotNumber">批號。</param>
        /// <returns>取藥堆疊母資料。</returns>
        private takeMedicineStackClass CreateBaseTakeMedicineStack(
            MulMedTakeContext context,
            class_OutTakeMed_data data,
            string priKey,
            int totalQty,
            string drugCode,
            string drugName,
            string unit,
            string action,
            string color,
            string reason,
            string expirationDate = "",
            string lotNumber = "")
        {
            takeMedicineStackClass takeMedicineStack = new takeMedicineStackClass();

            takeMedicineStack.GUID = Guid.NewGuid().ToString();
            takeMedicineStack.Order_GUID = data.Order_GUID;
            takeMedicineStack.序號 = DateTime.Now.ToDateTimeString_6();
            takeMedicineStack.動作 = action;
            takeMedicineStack.調劑台名稱 = context.DeviceName;
            takeMedicineStack.藥袋序號 = priKey;
            takeMedicineStack.總異動量 = totalQty.ToString();
            takeMedicineStack.藥品碼 = drugCode;
            takeMedicineStack.藥品名稱 = drugName;
            takeMedicineStack.單位 = unit;
            takeMedicineStack.病歷號 = data.病歷號;
            takeMedicineStack.病人姓名 = data.病人姓名;
            takeMedicineStack.開方時間 = data.開方時間;
            takeMedicineStack.操作時間 = DateTime.Now.ToDateTimeString_6();
            takeMedicineStack.操作人 = data.操作人;
            takeMedicineStack.顏色 = color;
            takeMedicineStack.類別 = data.類別;
            takeMedicineStack.床號 = data.床號;
            takeMedicineStack.領藥號 = data.領藥號;
            takeMedicineStack.收支原因 = reason;
            takeMedicineStack.效期 = expirationDate;
            takeMedicineStack.批號 = lotNumber;

            return takeMedicineStack;
        }

        /// <summary>
        /// 讀取所有儲位設備資料。
        /// </summary>
        /// <remarks>
        /// 此方法會讀取多種設備序列化資料表，並轉換為 DeviceBasic 清單：
        /// 1. epd583_jsonstring
        /// 2. epd1020_jsonstring
        /// 3. epd266_jsonstring
        /// 4. rowsled_jsonstring
        /// 5. rfid_device_jsonstring
        /// 
        /// 最後會排除 Code 空白的設備資料。
        /// </remarks>
        /// <param name="name">調劑台名稱。</param>
        /// <returns>有效的 DeviceBasic 清單。</returns>
        private List<DeviceBasic> Function_讀取儲位(string name)
        {
            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses.MyFind(name, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.一般資料);
            sys_serverSettingClass sys_serverSettingClass_人員資料 = sys_serverSettingClasses.MyFind(name, enum_sys_serverSetting_Type.調劑台, enum_sys_serverSetting_調劑台.人員資料);

            if (sys_serverSettingClass == null)
            {
                return new List<DeviceBasic>();
            }

            if (sys_serverSettingClass_人員資料 == null)
            {
                return new List<DeviceBasic>();
            }

            string IP = sys_serverSettingClass.Server;
            string DataBaseName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            SQLControl sQLControl_EPD583_serialize = new SQLControl(IP, DataBaseName, "epd583_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_EPD1020_serialize = new SQLControl(IP, DataBaseName, "epd1020_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_EPD266_serialize = new SQLControl(IP, DataBaseName, "epd266_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RowsLED_serialize = new SQLControl(IP, DataBaseName, "rowsled_jsonstring", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_RFID_Device_serialize = new SQLControl(IP, DataBaseName, "rfid_device_jsonstring", UserName, Password, Port, SSLMode);

            List<object[]> list_EPD583 = sQLControl_EPD583_serialize.GetAllRows(null);
            List<object[]> list_EPD1020 = sQLControl_EPD1020_serialize.GetAllRows(null);
            List<object[]> list_EPD266 = sQLControl_EPD266_serialize.GetAllRows(null);
            List<object[]> list_RowsLED = sQLControl_RowsLED_serialize.GetAllRows(null);
            List<object[]> list_RFID_Device = sQLControl_RFID_Device_serialize.GetAllRows(null);

            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<DeviceBasic> deviceBasics_buf = new List<DeviceBasic>();

            if (list_EPD1020.Count > 0) deviceBasics.LockAdd(DrawerMethod.GetAllDeviceBasic(list_EPD1020));
            if (list_EPD583.Count > 0) deviceBasics.LockAdd(DrawerMethod.GetAllDeviceBasic(list_EPD583));
            if (list_EPD266.Count > 0) deviceBasics.LockAdd(StorageMethod.GetAllDeviceBasic(list_EPD266));
            if (list_RowsLED.Count > 0) deviceBasics.LockAdd(RowsLEDMethod.GetAllDeviceBasic(list_RowsLED));
            if (list_RFID_Device.Count > 0) deviceBasics.LockAdd(RFIDMethod.GetAllDeviceBasic(list_RFID_Device));

            deviceBasics_buf = (from value in deviceBasics
                                where value.Code.StringIsEmpty() == false
                                select value).ToList();

            return deviceBasics_buf;
        }

        /// <summary>
        /// 新增取藥堆疊母資料。
        /// </summary>
        /// <remarks>
        /// 寫入 take_medicine_stack_new 前會依動作設定狀態：
        /// 
        /// 系統入庫、系統撥入、系統調入、系統退藥：
        /// 狀態 = 新增效期
        /// 
        /// 掃碼領藥：
        /// 狀態 = 新增資料
        /// 
        /// 其他：
        /// 狀態 = 等待刷新
        /// </remarks>
        /// <param name="sys_serverSettingClass">ServerSetting。</param>
        /// <param name="設備名稱">調劑台或電腦名稱。</param>
        /// <param name="takeMedicineStackClasses">欲新增的取藥堆疊母資料清單。</param>
        /// <returns>新增成功回傳 true；失敗回傳 false。</returns>
        private bool Function_取藥堆疊資料_新增母資料(
            sys_serverSettingClass sys_serverSettingClass,
            string 設備名稱,
            List<takeMedicineStackClass> takeMedicineStackClasses)
        {
            if (sys_serverSettingClass == null)
            {
                return false;
            }

            string server = sys_serverSettingClass.Server;
            string DataBaseName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            SQLControl sQLControl_take_medicine_stack = new SQLControl(server, DataBaseName, "take_medicine_stack_new", UserName, Password, Port, SSLMode);

            for (int i = 0; i < takeMedicineStackClasses.Count; i++)
            {
                if (takeMedicineStackClasses[i].GUID == null) takeMedicineStackClasses[i].GUID = Guid.NewGuid().ToString();
                if (takeMedicineStackClasses[i].GUID == "") takeMedicineStackClasses[i].GUID = Guid.NewGuid().ToString();

                takeMedicineStackClasses[i].調劑台名稱 = 設備名稱;

                if (takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.系統入庫.GetEnumName())
                {
                    takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.新增效期.GetEnumName();
                }
                else if (takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.系統撥入.GetEnumName())
                {
                    takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.新增效期.GetEnumName();
                }
                else if (takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.系統調入.GetEnumName())
                {
                    takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.新增效期.GetEnumName();
                }
                else if (takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.系統退藥.GetEnumName())
                {
                    takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.新增效期.GetEnumName();
                }
                else if (takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.掃碼領藥.GetEnumName())
                {
                    takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.新增資料.GetEnumName();
                }
                else
                {
                    takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName();
                }
            }

            List<object[]> list_add = takeMedicineStackClasses.ClassToSQL<takeMedicineStackClass, enum_取藥堆疊母資料>();
            sQLControl_take_medicine_stack.AddRows(null, list_add);

            return true;
        }

        /// <summary>
        /// 依設備數量取得預設顏色。
        /// </summary>
        /// <remarks>
        /// 用於 devicelist 找不到指定電腦名稱時，建立新設備資料並給予預設顏色。
        /// </remarks>
        /// <param name="index">設備索引。</param>
        /// <returns>對應顏色。</returns>
        private Color Function_取得顏色(int index)
        {
            index = index % 7;

            if (index == 0)
            {
                return Color.Red;
            }
            else if (index == 1)
            {
                return Color.Orange;
            }
            else if (index == 2)
            {
                return Color.Green;
            }
            else if (index == 3)
            {
                return Color.Green;
            }
            else if (index == 4)
            {
                return Color.Blue;
            }
            else if (index == 5)
            {
                return Color.Purple;
            }
            else if (index == 6)
            {
                return Color.White;
            }

            return Color.Red;
        }

        #endregion
        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_取藥堆疊母資料()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_取藥堆疊子資料()));           
            return tables.JsonSerializationt(true);
        }
    }
}
