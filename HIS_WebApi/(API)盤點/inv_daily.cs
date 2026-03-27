using Basic;
using Google.Protobuf.WellKnownTypes;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using MyUI;
using NPOI;
using NPOI.HPSF;
using NPOI.OpenXmlFormats.Dml.Diagram;
using SQLUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_盤點
{
    [Route("api/[controller]")]
    [ApiController]
    public class inv_daily : ControllerBase
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

        [HttpPost("update_content")]
        public async Task<string> update_content([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                returnData.Method = "get_full_inv_DataTable_by_SN";

                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入盤點單號!";
                    return returnData.JsonSerializationt(true);
                }
                string[] EvdInv = returnData.Value.Split(';').ToArray();

                (string Server, string DB, string UserName, string Password, uint Port) = await serverInfoTask.Value;
                SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
                SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);

                List<object[]> list_inventory_content = await sQLControl_inventory_content.GetRowsByDefultAsync(null, (int)enum_盤點內容.盤點單號, EvdInv);
                List<inventoryClass.content> contents = list_inventory_content.SQLToClass<inventoryClass.content, enum_盤點內容>();
                string[] Master_GUID = contents.Select(x => x.Master_GUID).Distinct().ToArray();
                List<object[]> list_inventory_creat = await sQLControl_inventory_creat.GetRowsByDefultAsync(null, (int)enum_盤點單號.GUID, Master_GUID);
                List<inventoryClass.creat> creat = list_inventory_creat.SQLToClass<inventoryClass.creat, enum_盤點單號>();

                returnData returnData_stock = await new stock().get_stock_all_server();
                List<stockClass> stockClasses = returnData_stock.Data.ObjToClass<List<stockClass>>();
                if (stockClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"庫存取得失敗";
                    return returnData.JsonSerializationt(true);
                }

                returnData returnData_consume = await new consumption().get_consume_all_server_today();
                List<consumptionClass> consumptionClasses = returnData_consume.Data.ObjToClass<List<consumptionClass>>();
                if (consumptionClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"消耗量取得失敗";
                    return returnData.JsonSerializationt(true);
                }

                returnData returnData_med_price = await new medPirce().get_by_codes(contents.Select(x => x.藥品碼).Distinct().ToList());
                List<medPriceClass> medPriceClasses = returnData_med_price.Data.ObjToClass<List<medPriceClass>>();
                if (medPriceClasses == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"藥檔單價取得失敗";
                    return returnData.JsonSerializationt(true);
                }
                List<List<inventoryClass.content>> contents_group = contents.GroupBy(x => x.Master_GUID).Select(g => g.ToList()).ToList();
                foreach (var content in contents_group)
                {
                    string 盤點名稱 = creat.Where(x => x.GUID == content[0].Master_GUID).Select(x => x.盤點名稱).FirstOrDefault();
                    for (int i = 0; i < content.Count; i++)
                    {
                        stockClass stockClass = stockClasses.Where(x => x.藥碼 == content[i].藥品碼 && 盤點名稱.Contains(x.serverName) && x.total_qty.StringToDouble() > 0).FirstOrDefault();
                        consumptionClass consumptionClass = consumptionClasses.Where(x => x.藥碼 == content[i].藥品碼 && 盤點名稱.Contains(x.serverName)).FirstOrDefault();
                        medPriceClass medPriceClass = medPriceClasses.Where(x => x.藥品碼 == content[i].藥品碼).FirstOrDefault();

                        content[i].理論值 = stockClass != null ? stockClass.total_qty : "0";
                        content[i].消耗量 = consumptionClass != null ? consumptionClass.平均消耗量 : "0";
                        content[i].單價 = medPriceClass != null ? medPriceClass.售價 : "0";

                    }
                }
                List<object[]> update_content = contents.ClassToSQL<inventoryClass.content, enum_盤點內容>();
                await sQLControl_inventory_content.UpdateRowsAsync(null, update_content);

                returnData.Data = contents;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"更新盤點內容，共{contents.Count}筆";
                returnData.Method = "update_content";
                returnData.Code = 200;
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                //if (ex.Message == "Index was outside the bounds of the array.") GET_init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }

        }

        [HttpPost("get_daily_report_DataTable")]
        public async Task<string> get_daily_report_DataTable([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_daily_report_DataTable";

            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入盤點單號!";
                return returnData.JsonSerializationt(true);
            }

            List<inventoryClass.creat> creats = await get_creat(returnData.Value);           
            
            if (creats == null)
            {
                returnData.Code = -200;
                returnData.Result = $"資料初始化失敗!";
                return returnData.JsonSerializationt(true);
            }
            (System.Data.DataTable dataTable, List < System.Data.DataTable > dataTables_creat)  = await get_datatalbe(creats);
            List<inventoryClass.content> contents = new List<inventoryClass.content>();
            List<inventoryClass.content> contents_buf = new List<inventoryClass.content>();
           
            string 藥品碼 = "";
            string 料號 = string.Empty;

            List<System.Data.DataTable> dataTables = new List<DataTable>();
            dataTables.Add(dataTable);

            for (int i = 0; i < dataTables_creat.Count; i++)
            {
                dataTables.Add(dataTables_creat[i]);
            }
            
            returnData.Data = dataTables.JsonSerializeDataTable();
            returnData.TimeTaken = myTimerBasic.ToString();
            returnData.Result = $"成功轉換表單<{dataTables.Count}>張";
            return returnData.JsonSerializationt();
        }
        [HttpPost("get_month_report_DataTable")]
        public async Task<string> get_month_report_DataTable([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_month_report_DataTable";

            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入盤點單號!";
                return returnData.JsonSerializationt(true);
            }

            List<inventoryClass.creat> creats = await get_creat(returnData.Value);

            if (creats == null)
            {
                returnData.Code = -200;
                returnData.Result = $"資料初始化失敗!";
                return returnData.JsonSerializationt(true);
            }
            List<List<inventoryClass.creat>> creats_group = creats.GroupBy(x => x.盤點單號.Split("-")[0]).Select(g => g.ToList()).ToList();
            List<System.Data.DataTable> dataTables = new List<DataTable>();

            foreach (var creat in creats_group)
            {
                string 盤點單號 = creat[0].盤點單號.Length > 11 ? creat[0].盤點單號.Substring(3, 8) : "";
                (System.Data.DataTable dataTable, List<System.Data.DataTable> dataTables_creat) = await get_datatalbe(creats);
                dataTable.TableName = $"{盤點單號}每日盤點";
                dataTables.Add(dataTable);
            }
            System.Data.DataTable total_dataTable = new DataTable();
            total_dataTable.Columns.Add("藥碼", typeof(string));
            total_dataTable.Columns.Add("料號", typeof(string));
            total_dataTable.Columns.Add("藥名", typeof(string));
            total_dataTable.TableName = "盤點總表";
            foreach (var dt in dataTables)
            {
                string colName = dt.TableName + "誤差量";           
                total_dataTable.Columns.Add(colName, typeof(decimal));
                foreach (DataRow row in dt.Rows)
                {
                    string 藥碼 = row["藥碼"].ToString();
                    string 料號 = row["料號"].ToString();
                    string 藥名 = row["藥名"].ToString();
                    string 誤差量= row["誤差量"].ToString();

                    // 找看看 mergedTable 是否已經有這個藥碼
                    DataRow[] existingRows = total_dataTable.Select($"藥碼 = '{藥碼}'");

                    if (existingRows.Length > 0)
                    {
                        object currentVal = existingRows[0][colName];

                        double currentQty = 0;
                        if (currentVal != DBNull.Value && currentVal != null && currentVal.ToString() != "")
                        {
                            currentQty = Convert.ToDouble(currentVal);
                        }

                        // 累加
                        existingRows[0][colName] = currentQty + Convert.ToDouble(誤差量);
                    }
                    else
                    {
                        DataRow newRow = total_dataTable.NewRow();
                        newRow["藥碼"] = 藥碼;
                        newRow["料號"] = 料號;
                        newRow["藥名"] = 藥名;
                        newRow[colName] = 誤差量;
                        total_dataTable.Rows.Add(newRow);
                    }

                }
            }
            List<System.Data.DataTable> dataTables_ = new List<DataTable>();
            dataTables_.Add(total_dataTable);

            for (int i = 0; i < dataTables.Count; i++)
            {
                dataTables_.Add(dataTables[i]);
            }
            returnData.Data = dataTables_.JsonSerializeDataTable();
            returnData.TimeTaken = myTimerBasic.ToString();
            returnData.Result = $"成功轉換表單<{dataTables_.Count}>張";
            return returnData.JsonSerializationt();
        }   
        [HttpPost("get_daily_report_excel")]
        public async Task<ActionResult> get_daily_report_excel([FromBody] returnData returnData)
        {

            string json_out = await get_daily_report_DataTable(returnData);
            returnData = json_out.JsonDeserializet<returnData>();
            string dataTable_string = returnData.Data.ObjToClass<string>();
            List<System.Data.DataTable> dataTables = dataTable_string.JsonDeserializeToDataTables();

            if (dataTables == null)
            {
                return null;
            }
            

            string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string xls_command = "application/vnd.ms-excel";

            System.Enum[] enums = new System.Enum[] {  enum_每日盤點_Excel.庫存量, enum_每日盤點_Excel .盤點量 ,enum_每日盤點_Excel .單價 ,enum_每日盤點_Excel .消耗量 ,
                enum_每日盤點_Excel .誤差量 ,enum_每日盤點_Excel.誤差金額};
            byte[] excelData = ExcelClass.NPOI_GetBytes(dataTables, Excel_Type.xlsx, enums);
            Stream stream = new MemoryStream(excelData);
            return await Task.FromResult(File(stream, xlsx_command, $"{returnData.Value}_InventorySummary.xlsx"));
        }
        [HttpPost("get_month_report_excel")]
        public async Task<ActionResult> get_daily_repoget_month_report_excelrt_excel([FromBody] returnData returnData)
        {

            string json_out = await get_month_report_DataTable(returnData);
            returnData = json_out.JsonDeserializet<returnData>();
            string dataTable_string = returnData.Data.ObjToClass<string>();
            List<System.Data.DataTable> dataTables = dataTable_string.JsonDeserializeToDataTables();

            if (dataTables == null)
            {
                return null;
            }
            
            string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string xls_command = "application/vnd.ms-excel";

            System.Enum[] enums = new System.Enum[] {  enum_每日盤點_Excel.庫存量, enum_每日盤點_Excel .盤點量 ,enum_每日盤點_Excel .單價 ,enum_每日盤點_Excel .消耗量 ,
                enum_每日盤點_Excel .誤差量 ,enum_每日盤點_Excel.誤差金額};
            byte[] excelData = ExcelClass.NPOI_GetBytes(dataTables, Excel_Type.xlsx, enums);
            Stream stream = new MemoryStream(excelData);
            return await Task.FromResult(File(stream, xlsx_command, $"{returnData.Value}_InventorySummary.xlsx"));
        }
        private async Task<List<inventoryClass.creat>> get_creat(string value)
        {
            returnData returnData_creat = await new inventoryController().creat_get_by_IC_SN(value); //盤點單號
            List<inventoryClass.creat> creats = returnData_creat.Data.ObjToClass<List<inventoryClass.creat>>();

            creats = creats
            .Select(creat =>
            {
                creat.Contents = creat.Contents?
                    .Where(item => item.Sub_content != null && item.Sub_content.Count > 0)
                    .ToList()
                    ?? new List<inventoryClass.content>();

                return creat;
            }).ToList();
            return creats;
        }
        private async Task<(System.Data.DataTable, List<System.Data.DataTable>)> get_datatalbe(List<inventoryClass.creat> creats) 
        {
            List<inventoryClass.content> contents = new List<inventoryClass.content>();
            List<inventoryClass.content> contents_buf = new List<inventoryClass.content>();
            List<System.Data.DataTable> dataTables_creat = new List<System.Data.DataTable>();

            for (int i = 0; i < creats.Count; i++) //數量是合併了幾張盤點單
            {
                List<object[]> list_creat_buf = new List<object[]>();
                System.Data.DataTable dataTable_buf = new System.Data.DataTable();
                for (int k = 0; k < creats[i].Contents.Count; k++) //盤點內容(藥品為單位)
                {
                    if (creats[i].Contents[k].Sub_content.Count == 0) continue;

                    object[] value = new object[new enum_每日盤點_Excel().GetLength()];
                    value[(int)enum_每日盤點_Excel.藥碼] = creats[i].Contents[k].藥品碼;
                    value[(int)enum_每日盤點_Excel.料號] = creats[i].Contents[k].料號;
                    value[(int)enum_每日盤點_Excel.藥名] = creats[i].Contents[k].藥品名稱;
                    value[(int)enum_每日盤點_Excel.單價] = creats[i].Contents[k].單價;
                    value[(int)enum_每日盤點_Excel.庫存量] = creats[i].Contents[k].理論值;
                    value[(int)enum_每日盤點_Excel.盤點量] = creats[i].Contents[k].盤點量;
                    value[(int)enum_每日盤點_Excel.消耗量] = creats[i].Contents[k].消耗量;
                    value[(int)enum_每日盤點_Excel.誤差量] = (creats[i].Contents[k].盤點量.StringToDouble() - creats[i].Contents[k].理論值.StringToDouble()).ToString();
                    value[(int)enum_每日盤點_Excel.誤差金額] = (value[(int)enum_每日盤點_Excel.誤差量].StringToDouble() * creats[i].Contents[k].單價.StringToDouble()).ToString();

                    if (creats[i].Contents[k].消耗量.StringToDouble() > 0)
                    {
                        value[(int)enum_每日盤點_Excel.誤差百分率] = (value[(int)enum_每日盤點_Excel.誤差量].StringToDouble() / value[(int)enum_每日盤點_Excel.消耗量].StringToDouble() * 100).ToString("0.00");
                    }
                    list_creat_buf.Add(value);

                    contents_buf = contents.Where(temp => temp.藥品碼 == creats[i].Contents[k].藥品碼).ToList();
                    if (contents_buf.Count == 0)
                    {
                        inventoryClass.content content = creats[i].Contents[k];
                        content.GUID = "";
                        content.Master_GUID = "";
                        content.新增時間 = "";
                        content.盤點單號 = "";
                        content.Sub_content.Clear();
                        contents.Add(content);
                    }
                    else
                    {
                        contents_buf[0].盤點量 = (creats[i].Contents[k].盤點量.StringToInt32() + contents_buf[0].盤點量.StringToInt32()).ToString();
                        contents_buf[0].理論值 = (creats[i].Contents[k].理論值.StringToInt32() + contents_buf[0].理論值.StringToInt32()).ToString();
                        contents_buf[0].消耗量 = (creats[i].Contents[k].消耗量.StringToInt32() + contents_buf[0].消耗量.StringToInt32()).ToString();
                    }
                }
                dataTable_buf = list_creat_buf.ToDataTable(new enum_每日盤點_Excel());

                string tableName = $"{i}.{creats[i].盤點名稱}";

                // 移除或替換非法字元
                string safeFileName = Regex.Replace(tableName, @"[\\/:*?""<>|]", "_");

                // 指定為合法的檔案名稱
                dataTable_buf.TableName = safeFileName;

                dataTables_creat.Add(dataTable_buf);
            }

            List<object[]> list_value = new List<object[]>();


            for (int i = 0; i < contents.Count; i++) //總表藥品項目
            {
                //string 藥碼 = contents[i].藥品碼;
                //string __料號 = contents[i].料號;

                object[] value = new object[new enum_每日盤點_Excel().GetLength()];
                value[(int)enum_每日盤點_Excel.藥碼] = contents[i].藥品碼;
                value[(int)enum_每日盤點_Excel.料號] = contents[i].料號;
                value[(int)enum_每日盤點_Excel.藥名] = contents[i].藥品名稱;
                value[(int)enum_每日盤點_Excel.單價] = contents[i].單價;
                value[(int)enum_每日盤點_Excel.庫存量] = contents[i].理論值;
                value[(int)enum_每日盤點_Excel.盤點量] = contents[i].盤點量;
                value[(int)enum_每日盤點_Excel.消耗量] = contents[i].消耗量;
                value[(int)enum_每日盤點_Excel.誤差量] = (contents[i].盤點量.StringToDouble() - contents[i].理論值.StringToDouble()).ToString();
                value[(int)enum_每日盤點_Excel.誤差金額] = (value[(int)enum_每日盤點_Excel.誤差量].StringToDouble() * contents[i].單價.StringToDouble()).ToString();

                if (contents[i].消耗量.StringToDouble() > 0)
                {
                    value[(int)enum_每日盤點_Excel.誤差百分率] = (value[(int)enum_每日盤點_Excel.誤差量].StringToDouble() / value[(int)enum_每日盤點_Excel.消耗量].StringToDouble() * 100).ToString("0.00");
                }

                list_value.Add(value);
            }

            System.Data.DataTable dataTable = list_value.ToDataTable(new enum_每日盤點_Excel());
            dataTable.TableName = "盤點總表";
            foreach (var dt in dataTables_creat)
            {
                string colName = dt.TableName;
                dataTable.Columns.Add(colName, typeof(decimal));
                foreach (DataRow row in dt.Rows)
                {
                    string drugCode = row["料號"].ToString();
                    string qty = row["盤點量"].ToString();

                    // 找看看 mergedTable 是否已經有這個藥碼
                    DataRow[] existingRows = dataTable.Select($"料號 = '{drugCode}'");

                    if (existingRows.Length > 0)
                    {
                        object currentVal = existingRows[0][colName];

                        double currentQty = 0;
                        if (currentVal != DBNull.Value && currentVal != null && currentVal.ToString() != "")
                        {
                            currentQty = Convert.ToDouble(currentVal);
                        }

                        // 累加
                        existingRows[0][colName] = currentQty + Convert.ToDouble(qty);
                    }

                }
            }
            return (dataTable, dataTables_creat);
        }
        public enum enum_每日盤點_Excel
        {
            藥碼,
            料號,
            藥名,
            單價,
            庫存量,
            盤點量,
            消耗量,
            誤差量,
            誤差金額,
            誤差百分率,
        }
    }
   
}
