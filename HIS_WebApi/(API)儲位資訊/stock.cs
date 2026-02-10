using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using MyUI;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using SQLUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text.Json.Serialization;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class stock : ControllerBase
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
        [HttpPost("get_stock")]
        public async Task<string> get_stock([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "ServerName or ServerType is null";
                    return returnData.JsonSerializationt(true);
                }
                await init(returnData);
                string ServerName = returnData.ServerName;
                string ServerType = returnData.ServerType;
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync(returnData.ServerName, returnData.ServerType, "儲位資料");
                SQLControl sQLControl = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);

                List<object[]> rows = await sQLControl.GetAllRowsAsync(null);
                List<stockClass> stockClasses = rows.SQLToClass<stockClass>();
                string[] code = stockClasses.Select(x => x.藥碼).Distinct().ToArray();
                returnData returnData_med_cloud = await new MED_pageController().get_med_clouds_by_codes(code);
                if (returnData_med_cloud == null || returnData_med_cloud.Code != 200)
                {
                    returnData_med_cloud.Result += "藥檔取得失敗";
                    return returnData_med_cloud.JsonSerializationt(true);
                }
                returnData returnData_med_unit = await new medUnit().get_all();
                if (returnData_med_unit == null || returnData_med_unit.Code != 200)
                {
                    returnData_med_unit.Result += "藥品單位取得失敗";
                    return returnData_med_unit.JsonSerializationt(true);
                }
                returnData returnData_content_get = await new inspectionController().content_get();
                List<inspectionClass.content> contents = new List<inspectionClass.content>();

                if (returnData_content_get != null || returnData_content_get.Code == 200)
                {
                    contents = returnData_content_get.Data.ObjToClass<List<inspectionClass.content>>();
                }
                if (contents == null) contents = new List<inspectionClass.content>();

                List<medClass> med_cloud = returnData_med_cloud.Data.ObjToClass<List<medClass>>();
                Dictionary<string, List<medClass>> medCloudDict = medClass.CoverToDictionaryByCode(med_cloud);

                List<medUnitClass> medUnitClasses = returnData_med_unit.Data.ObjToClass<List<medUnitClass>>();
                Dictionary<string, List<medUnitClass>> medUnitDict = medUnitClasses.ToDictByMedGuid();

                Dictionary<string, List<inspectionClass.content>> contentDict = contents.ToDictByCode();

                string clssify_GUID = string.Join(";", stockClasses.Select(x => x.Classify_GUID).Distinct());
                returnData returnData_get_by_GUID = await new medClassify().get_by_GUID(clssify_GUID);
                List<medClassifyClass> medClassifyClasses = returnData_get_by_GUID.Data.ObjToClass<List<medClassifyClass>>();
                if (medClassifyClasses == null) medClassifyClasses = new List<medClassifyClass>();
                List<List<stockClass>> groupedList = stockClasses
                    .GroupBy(s => s.Classify_GUID)  
                    .Select(g => g.ToList())        
                    .ToList();            
                List<stockClass> stock_delete = new List<stockClass>();
                List<stockClass> stock_result = new List<stockClass>();

                foreach (var list in groupedList)
                {
                    string classify_GUID = list[0].Classify_GUID;
                    medClassifyClass medClassifyClass = medClassifyClasses.FirstOrDefault(x => x.GUID == classify_GUID);
                    if (medClassifyClass == null) medClassifyClass = new medClassifyClass();
                    foreach (var stock in list)
                    {
                        medClass medClass = medClass.SortDictionaryByCode(medCloudDict, stock.藥碼).FirstOrDefault();
                        if (medClass == null) 
                        {
                            stock_delete.Add(stock);
                            continue;
                        } 

                        List<inspectionClass.content> contents_buff = contentDict.GetByCode(stock.藥碼);
                        string med_GUID = medClass.GUID;
                        List<medUnitClass> medUnits = medUnitDict.GetByMasterGUID(med_GUID);
                        string value = stock.Value;
                        if (value.StringIsEmpty()) value = new DeviceBasic().JsonSerializationt();
                        DeviceBasic deviceBasic = value.JsonDeserializet<DeviceBasic>();
                        stock.效期 = deviceBasic.List_Validity_period;
                        stock.數量 = deviceBasic.List_Inventory;
                        stock.批號 = deviceBasic.List_Lot_number;
                        stock.Classify = medClassifyClass;
                        stock.med_cloud = medClass;
                        stock.藥名 = medClass.藥品名稱;
                        stock.料號 = medClass.料號;
                        stock.med_unit = medUnits;
                        stock.content = contents_buff;
                        stock.serverName = ServerName;
                        stock.serverType = ServerType;
                        stock_result.Add(stock);
                    }
                }
                if (stock_delete.Count > 0)
                {
                    List<object[]> list_delete = stock_delete.ClassToSQL<stockClass>();
                    await sQLControl.DeleteRowsAsync(null, list_delete);
                }
                returnData.Code = 200;
                returnData.Data = stock_result;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_stock";
                returnData.Result = $"取得ServerName{ServerName} ServerType{ServerType}儲位資料，共{stockClasses.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                //if (ex.Message == "Table 'dbvm.medmap_stock' doesn't exist") init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_stock_all_server")]
        public async Task<string> get_stock_all_server([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                returnData returnData_server = await new ServerSettingController().get_name();
                if (returnData_server == null || returnData_server.Code != 200)
                {
                    returnData_server.Result += "伺服器取得失敗";
                    return returnData_server.JsonSerializationt(true);
                }
                List<sys_serverSettingClass> serverSettingClasses = returnData_server.Data.ObjToClass<List<sys_serverSettingClass>>();
                List<Task<string>> tasks = new List<Task<string>>();

                foreach (var item in serverSettingClasses)
                {
                    returnData returnData_ = new returnData();

                    returnData_.ServerName = item.設備名稱;
                    returnData_.ServerType = item.類別;
                    Task<string> stock = get_stock(returnData_);
                    tasks.Add(stock);

                }
                string[] result = await Task.WhenAll(tasks);
                List<stockClass> stockClasses = new List<stockClass>();
                for (int i = 0; i < result.Length; i++)
                {
                    returnData returnData_result = result[i].JsonDeserializet<returnData>();
                    if (returnData_result.Code == 200)
                    {
                        List<stockClass> temp_stockClasses = returnData_result.Data.ObjToClass<List<stockClass>>();
                        stockClasses.AddRange(temp_stockClasses);
                    }
                }



                returnData.Code = 200;
                returnData.Data = stockClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_stock_all_server";
                returnData.Result = $"取得共{stockClasses.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                //if (ex.Message == "Table 'dbvm.medmap_stock' doesn't exist") init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_stock_by_code")]
        public async Task<string> get_stock_by_code([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "ServerName or ServerType is null";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry == null || returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = "ValueAry應為[\"code\"]";
                    return returnData.JsonSerializationt(true);
                }
                
                string ServerName = returnData.ServerName;
                string ServerType = returnData.ServerType;
                string[] code = returnData.ValueAry[0].Split(";");
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync(returnData.ServerName, returnData.ServerType, "儲位資料");
                SQLControl sQLControl = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);

                List<object[]> rows = await sQLControl.GetRowsByDefultAsync(null, (int)enum_medMap_stock.藥碼, code);
                List<stockClass> stockClasses = rows.SQLToClass<stockClass>();
                returnData returnData_med_cloud = await new MED_pageController().get_med_clouds_by_codes(code);
                if (returnData_med_cloud == null || returnData_med_cloud.Code != 200)
                {
                    returnData_med_cloud.Result += "藥檔取得失敗";
                    return returnData_med_cloud.JsonSerializationt(true);
                }
                List<medClass> med_cloud = returnData_med_cloud.Data.ObjToClass<List<medClass>>();
                Dictionary<string, List<medClass>> medCloudDict = medClass.CoverToDictionaryByCode(med_cloud);

                string clssify_GUID = string.Join(";", stockClasses.Select(x => x.Classify_GUID).Distinct());
                returnData returnData1_get_by_GUID = await new medClassify().get_by_GUID(clssify_GUID);
                List<medClassifyClass> medClassifyClasses = returnData1_get_by_GUID.Data.ObjToClass<List<medClassifyClass>>();
                if (medClassifyClasses == null) medClassifyClasses = new List<medClassifyClass>();
                List<List<stockClass>> groupedList = stockClasses
                    .GroupBy(s => s.Classify_GUID)
                    .Select(g => g.ToList())
                    .ToList();
                foreach (var list in groupedList)
                {
                    string classify_GUID = list[0].Classify_GUID;
                    medClassifyClass medClassifyClass = medClassifyClasses.FirstOrDefault(x => x.GUID == classify_GUID);
                    if (medClassifyClass == null) medClassifyClass = new medClassifyClass();
                    foreach (var stock in list)
                    {
                        List<medClass> medClasses = medClass.SortDictionaryByCode(medCloudDict, stock.藥碼);
                        string value = stock.Value;
                        if (value.StringIsEmpty()) value = new DeviceBasic().JsonSerializationt();
                        DeviceBasic deviceBasic = value.JsonDeserializet<DeviceBasic>();
                        stock.效期 = deviceBasic.List_Validity_period;
                        stock.數量 = deviceBasic.List_Inventory;
                        stock.批號 = deviceBasic.List_Lot_number;
                        stock.Classify = medClassifyClass;
                        stock.med_cloud = medClasses.Count > 0 ? medClasses[0] : null;
                        stock.藥名 = medClasses.Count > 0 ? medClasses[0].藥品名稱 : "";
                        stock.料號 = medClasses.Count > 0 ? medClasses[0].料號 : "";
                    }
                }

                returnData.Code = 200;
                returnData.Data = stockClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "get_stock";
                returnData.Result = $"取得ServerName{ServerName} ServerType{ServerType}儲位資料，共{stockClasses.Count}筆!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                //if (ex.Message == "Table 'dbvm.medmap_stock' doesn't exist") init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("update_stock")]
        [HttpPost("update")]
        public async Task<string> update([FromBody] returnData returnData)
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
                List<stockClass> medMap_StockClasses = returnData.Data.ObjToClass<List<stockClass>>();
                if (medMap_StockClasses == null)
                {
                    stockClass medMap_stock = returnData.Data.ObjToClass<stockClass>();
                    if (medMap_stock == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data資料錯誤，須為stockClass";
                        return returnData.JsonSerializationt();
                    }
                    medMap_StockClasses = new List<stockClass>() { medMap_stock };
                }
                // DB 連線與資料表
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "ServerName or ServerType is null";
                    return returnData.JsonSerializationt(true);
                }
                string ServerName = returnData.ServerName;
                string ServerType = returnData.ServerType;
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync(returnData.ServerName, returnData.ServerType, "儲位資料");
                SQLControl sQLControl_stock = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);
                List<object[]> objects = await sQLControl_stock.GetRowsByDefultAsync(null, (int)enum_medMap_stock.GUID, medMap_StockClasses.Select(x => x.GUID).ToArray());
                List<stockClass> db_medMap_StockClasses = objects.SQLToClass<stockClass>();
                foreach (var item in db_medMap_StockClasses)
                {
                    stockClass medMap_stock_buff = medMap_StockClasses.Where(x => x.GUID == item.GUID).FirstOrDefault();
                    if (medMap_stock_buff == null) continue;
                    if (medMap_stock_buff.Shelf_GUID.StringIsEmpty() == false) item.Shelf_GUID = medMap_stock_buff.Shelf_GUID;
                    if (medMap_stock_buff.位置.StringIsEmpty() == false) item.位置 = medMap_stock_buff.位置;
                    if (medMap_stock_buff.IP.StringIsEmpty() == false) item.IP = medMap_stock_buff.IP;
                    if (medMap_stock_buff.device_type.StringIsEmpty() == false) item.device_type = medMap_stock_buff.device_type;
                    if (medMap_stock_buff.燈條亮燈位置.StringIsEmpty() == false) item.燈條亮燈位置 = medMap_stock_buff.燈條亮燈位置;
                    if (medMap_stock_buff.Classify_GUID.StringIsEmpty() == false) item.Classify_GUID = medMap_stock_buff.Classify_GUID;

                    if (medMap_stock_buff.藥碼.StringIsEmpty() == false) item.藥碼 = medMap_stock_buff.藥碼;
                    if (medMap_stock_buff.藥名.StringIsEmpty() == false) item.藥名 = medMap_stock_buff.藥名;
                    if (medMap_stock_buff.料號.StringIsEmpty() == false) item.料號 = medMap_stock_buff.料號;
                    if (medMap_stock_buff.效期 == null || (medMap_stock_buff.效期.Count != medMap_stock_buff.批號.Count && medMap_stock_buff.效期.Count != medMap_stock_buff.數量.Count)) continue;
                    string value = item.Value;
                    if (value.StringIsEmpty()) value = new DeviceBasic().JsonSerializationt();
                    DeviceBasic deviceBasic = value.JsonDeserializet<DeviceBasic>();
                    List<string> 效期 = deviceBasic.List_Validity_period; //原來的

                    for (int i = 0; i < medMap_stock_buff.效期.Count; i++)
                    {
                        if (效期.Contains(medMap_stock_buff.效期[i]) == false)
                        {
                            deviceBasic.新增效期(medMap_stock_buff.效期[i], medMap_stock_buff.批號[i], medMap_stock_buff.數量[i]);
                            效期.Add(medMap_stock_buff.效期[i]);
                        }
                        else
                        {
                            deviceBasic.效期庫存覆蓋(medMap_stock_buff.效期[i], medMap_stock_buff.批號[i], medMap_stock_buff.數量[i]);
                        }
                                                                              
                    }
                    for (int i = 0; i < 效期.Count; i++)
                    {
                        string 效期_ = 效期[i].StringToDateTime().ToDateTimeString();
                        if (medMap_stock_buff.效期.Contains(效期_) == false) 
                        
                        {
                            deviceBasic.清除效期(效期[i]);
                        } 
                    }
                    item.Value = deviceBasic.JsonSerializationt();
                }
                List<object[]> update = db_medMap_StockClasses.ClassToSQL<stockClass>();
                await sQLControl_stock.UpdateRowsAsync(null, update);

                returnData.Code = 200;
                returnData.Data = db_medMap_StockClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_stock";
                returnData.Result = $"儲位寫入成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
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
                List<stockClass> medMap_StockClasses = returnData.Data.ObjToClass<List<stockClass>>();
                if (medMap_StockClasses == null)
                {
                    stockClass medMap_stock = returnData.Data.ObjToClass<stockClass>();
                    if (medMap_stock == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data資料錯誤，須為stockClass";
                        return returnData.JsonSerializationt();
                    }
                    medMap_StockClasses = new List<stockClass>() { medMap_stock };
                }
                // DB 連線與資料表
                if (returnData.ServerName.StringIsEmpty() || returnData.ServerType.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "ServerName or ServerType is null";
                    return returnData.JsonSerializationt(true);
                }
                string ServerName = returnData.ServerName;
                string ServerType = returnData.ServerType;
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync(returnData.ServerName, returnData.ServerType, "儲位資料");
                SQLControl sQLControl_stock = new SQLControl(Server, DB, "stock", UserName, Password, Port, SSLMode);
                foreach (var item in medMap_StockClasses)
                {
                    item.GUID = Guid.NewGuid().ToString();


                    if (item.效期 == null || (item.效期.Count != item.批號.Count && item.效期.Count != item.數量.Count)) continue;
                    for (int i = 0; i < item.效期.Count; i++)
                    {
                        string value = item.Value;
                        if (value.StringIsEmpty()) value = new DeviceBasic().JsonSerializationt();
                        DeviceBasic deviceBasic = value.JsonDeserializet<DeviceBasic>();

                        deviceBasic.新增效期(item.效期[i], item.批號[i], item.數量[i]);
                        item.Value = deviceBasic.JsonSerializationt();
                    }
                }
                List<object[]> add = medMap_StockClasses.ClassToSQL<stockClass>();
                await sQLControl_stock.AddRowsAsync(null, add);

                returnData.Code = 200;
                returnData.Data = medMap_StockClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add";
                returnData.Result = $"儲位寫入成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("add_stockLight")]
        public async Task<string> add_stockLight([FromBody] returnData returnData)
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
                List<stockLightClass> stockLightClasses = returnData.Data.ObjToClass<List<stockLightClass>>();
                if (stockLightClasses == null)
                {
                    stockLightClass stockLightClass = returnData.Data.ObjToClass<stockLightClass>();
                    if (stockLightClass == null)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"returnData.Data資料錯誤，須為stockLightClass";
                        return returnData.JsonSerializationt();
                    }
                    stockLightClasses = new List<stockLightClass>() { stockLightClass };
                }
                
                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");
                SQLControl sQLControl_stock = new SQLControl(Server, DB, "stockLight", UserName, Password, Port, SSLMode);
                foreach (var item in stockLightClasses)
                {
                    item.GUID = Guid.NewGuid().ToString();


                   
                }
                List<object[]> add = stockLightClasses.ClassToSQL<stockLightClass>();
                await sQLControl_stock.AddRowsAsync(null, add);

                returnData.Code = 200;
                returnData.Data = stockLightClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "add_stockLight";
                returnData.Result = $"儲位寫入成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }

        private async Task<string> CheckCreatTable(returnData returnData)
        {
            sys_serverSettingClass sys_ServerSettingClass = new sys_serverSettingClass();
            if (returnData.ServerName.StringIsEmpty() && returnData.ServerType.StringIsEmpty())
            {
                sys_ServerSettingClass = await HIS_WebApi.Method.GetServerAsync("Main", "網頁", "VM端");
            }
            else
            {
                sys_ServerSettingClass = await HIS_WebApi.Method.GetServerAsync(returnData.ServerName, returnData.ServerType, "儲位資料");
            }
            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable<stockClass>(sys_ServerSettingClass));
            tables.Add(MethodClass.CheckCreatTable<stockLightClass>(sys_ServerSettingClass));

            return tables.JsonSerializationt(true);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_stock_all_server()
        {
            returnData returnData = new returnData();      
            string result = await get_stock_all_server(returnData);
            return result.JsonDeserializet<returnData>();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<returnData> get_stock_by_code(List<string> codes ,string serverName, string serverType)
        {
            returnData returnData = new returnData();
            string code_str = string.Join(";", codes);
            returnData.ValueAry.Add(code_str);
            returnData.ServerName = serverName;
            returnData.ServerType = serverType;
            string result = await get_stock_by_code(returnData);
            return result.JsonDeserializet<returnData>();
        }
    }
}
