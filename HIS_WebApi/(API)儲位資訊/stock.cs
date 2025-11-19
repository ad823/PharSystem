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

                if (returnData_med_unit != null || returnData_med_unit.Code == 200)
                {
                    contents = returnData_content_get.Data.ObjToClass<List<inspectionClass.content>>();
                }
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
                foreach (var list in groupedList)
                {
                    string classify_GUID = list[0].Classify_GUID;
                    medClassifyClass medClassifyClass = medClassifyClasses.FirstOrDefault(x => x.GUID == classify_GUID);
                    if (medClassifyClass == null) medClassifyClass = new medClassifyClass();
                    foreach (var stock in list)
                    {
                        List<medClass> medClasses = medClass.SortDictionaryByCode(medCloudDict, stock.藥碼);
                        string med_GUID = medClasses.Count > 0 ? medClasses[0].GUID : "";
                        List<medUnitClass> medUnits = medUnitDict.GetByMasterGUID(med_GUID);
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
                        stock.med_unit = medUnits;
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
        private List<stockClass> get_stockInfo(List<stockClass> medMap_stockClasses)
        {
            foreach (var stock in medMap_stockClasses)
            {
                string value = stock.Value;
                if (value.StringIsEmpty()) value = new DeviceBasic().JsonSerializationt();
                DeviceBasic deviceBasic = value.JsonDeserializet<DeviceBasic>();
                stock.效期 = deviceBasic.List_Validity_period;
                stock.數量 = deviceBasic.List_Inventory;
                stock.批號 = deviceBasic.List_Lot_number;
            }
            return medMap_stockClasses;
        }
    }
}
