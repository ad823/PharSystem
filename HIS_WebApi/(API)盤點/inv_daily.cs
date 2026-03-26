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
using SQLUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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


        
        [HttpPost("get_inv_by_SN")]
        public async Task<string> get_full_inv_by_SN([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                return returnData.JsonSerializationt();
            }
            (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
            returnData returnData_creat = await new inventoryController().creat_get_by_IC_SN(returnData.Value);
            List< inventoryClass.creat > creats = returnData_creat.Data.ObjToClass<List< inventoryClass.creat >>();
           
            creats = creats
            .Select(creat =>
            {
                creat.Contents = creat.Contents?
                    .Where(item => item.Sub_content != null && item.Sub_content.Count > 0)
                    .ToList()
                    ?? new List<inventoryClass.content>();

                return creat;
            }).ToList();
            
            string[] code = creats
                .Where(c => c.Contents != null)
                .SelectMany(c => c.Contents)
                .Where(x => x.藥品碼.StringIsEmpty() == false)
                .Select(x => x.藥品碼)
                .Distinct()
                .ToArray();
            returnData returnData_med_cloud = await new MED_pageController().get_med_clouds_by_codes(code);
            if (returnData_med_cloud == null || returnData_med_cloud.Code != 200)
            {
                returnData_med_cloud.Result += "藥檔取得失敗";
                return returnData_med_cloud.JsonSerializationt(true);
            }
            List<medClass> medClasses_cloud = returnData_med_cloud.Data.ObjToClass<List<medClass>>();

            Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClasses_cloud.CoverToDictionaryByCode();

            List<inventoryClass.content> contents = new List<inventoryClass.content>();
            List<inventoryClass.content> contents_buf = new List<inventoryClass.content>();

                      
            //patt1

            string 藥品碼 = "";
            string 料號 = string.Empty;
            for (int i = 0; i < creats.Count; i++)  //合併單有幾張盤點單
            {           
                for (int k = 0; k < creats[i].Contents.Count; k++)
                {
                    if (creats[i].Contents[k].Sub_content.Count == 0) continue;
                    料號 = creats[i].Contents[k].料號;
                    contents_buf = (from temp in contents
                                    where temp.料號 == 料號
                                    select temp).ToList();
                    if (contents_buf.Count == 0)
                    {
                        inventoryClass.content content = new inventoryClass.content();
                        content.藥品碼 = creats[i].Contents[k].藥品碼;
                        content.料號 = creats[i].Contents[k].料號;
                        content.廠牌 = creats[i].Contents[k].廠牌;
                        content.藥品名稱 = creats[i].Contents[k].藥品名稱;
                        content.中文名稱 = creats[i].Contents[k].中文名稱;
                        content.盤點量 = creats[i].Contents[k].盤點量;
                        contents.Add(content);
                    }
                    else
                    {
                        contents_buf[0].盤點量 = (creats[i].Contents[k].盤點量.StringToInt32() + contents_buf[0].盤點量.StringToInt32()).ToString();
                    }
                }
                creats[i].Contents = contents;
            }
                    
            returnData.Data = creats;
            returnData.Code = 200;
            returnData.TimeTaken = myTimerBasic.ToString();
            returnData.Method = "get_full_inv_by_SN";
            returnData.Result = $"成功取得盤點單合併完成資料";
            return returnData.JsonSerializationt(true);
        }

        [HttpPost("get_full_inv_DataTable_by_SN")]
        public async Task<string> POST_get_full_inv_DataTable_by_SN([FromBody] returnData returnData)
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

            string jsonString = await get_full_inv_by_SN(returnData);
            returnData = jsonString.JsonDeserializet<returnData>();
            if (returnData == null || returnData.Code != 200)
            {
                return returnData.JsonSerializationt(true);
            }
            List<inventoryClass.creat> creats = returnData.Data.ObjToClass<List<inventoryClass.creat>>();
            if (creats == null)
            {
                returnData.Code = -200;
                returnData.Result = $"資料初始化失敗!";
                return returnData.JsonSerializationt(true);
            }
            //inv_combinelistClass inv_CombinelistClass = returnData.Data.ObjToClass<inv_combinelistClass>();

            List<inventoryClass.content> contents = new List<inventoryClass.content>();
            List<inventoryClass.content> contents_buf = new List<inventoryClass.content>();
            List<System.Data.DataTable> dataTables_creat = new List<System.Data.DataTable>();
            returnData returnData_medCloud = await new MED_pageController().get_med_cloud();

            List<medClass> medClasses_cloud = returnData_medCloud.Data.ObjToClass<List<medClass>>();
            if (medClasses_cloud != null)
            {
                returnData.Code = -200;
                returnData.Result = $"藥檔取得失敗!";
                return returnData.JsonSerializationt(true);
            }
            List<medClass> medClasses_cloud_buf = new List<medClass>();
            Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClasses_cloud.CoverToDictionaryByCode();
            string 藥品碼 = "";
            string 料號 = string.Empty;

            for (int i = 0; i < creats.Count; i++) //數量是合併了幾張盤點單
            {
                List<object[]> list_creat_buf = new List<object[]>();
                System.Data.DataTable dataTable_buf = new System.Data.DataTable();
                for (int k = 0; k < creats[i].Contents.Count; k++) //盤點內容(藥品為單位)
                {
                    if (creats[i].Contents[k].Sub_content.Count == 0) continue;
                    藥品碼 = creats[i].Contents[k].藥品碼;
                    medClasses_cloud_buf = keyValuePairs_med_cloud.SortDictionaryByCode(藥品碼);
                    if (medClasses_cloud_buf.Count > 0)
                    {
                        creats[i].Contents[k].料號 = medClasses_cloud_buf[0].料號;
                        creats[i].Contents[k].藥品名稱 = medClasses_cloud_buf[0].藥品名稱;
                    }
                    object[] value = new object[new enum_盤點定盤_Excel().GetLength()];
                    value[(int)enum_盤點定盤_Excel.藥碼] = creats[i].Contents[k].藥品碼;
                    value[(int)enum_盤點定盤_Excel.料號] = creats[i].Contents[k].料號;
                    value[(int)enum_盤點定盤_Excel.藥名] = creats[i].Contents[k].藥品名稱;
                    value[(int)enum_盤點定盤_Excel.庫存量] = creats[i].Contents[k].理論值;
                    value[(int)enum_盤點定盤_Excel.盤點量] = creats[i].Contents[k].盤點量;
                    list_creat_buf.Add(value);
                    contents_buf = contents.Where(temp => temp.藥品碼 == 藥品碼).ToList();
                    contents_buf = (from temp in contents
                                    where temp.藥品碼 == 藥品碼
                                    select temp).ToList();
                    if (contents_buf.Count == 0)
                    {
                        inventoryClass.content content = creats[i].Contents[k];
                        content.GUID = "";
                        content.Master_GUID = "";
                        content.理論值 = "";
                        content.新增時間 = "";
                        content.盤點單號 = "";
                        content.Sub_content.Clear();
                        contents.Add(content);
                    }
                    else
                    {
                        contents_buf[0].盤點量 = (creats[i].Contents[k].盤點量.StringToInt32() + contents_buf[0].盤點量.StringToInt32()).ToString();
                    }
                }
                dataTable_buf = list_creat_buf.ToDataTable(new enum_盤點定盤_Excel());

                string tableName = $"{i}.{creats[i].盤點名稱}";

                // 移除或替換非法字元
                string safeFileName = Regex.Replace(tableName, @"[\\/:*?""<>|]", "_");

                // 指定為合法的檔案名稱
                dataTable_buf.TableName = safeFileName;

                dataTables_creat.Add(dataTable_buf);
            }



            List<object[]> list_value = new List<object[]>();
            System.Data.DataTable dataTable;


            for (int i = 0; i < contents.Count; i++) //總表藥品項目
            {
                bool flag_覆盤 = false;
                string 藥碼 = contents[i].藥品碼;
                string __料號 = contents[i].料號;

                object[] value = new object[new enum_盤點定盤_Excel().GetLength()];
                value[(int)enum_盤點定盤_Excel.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_盤點定盤_Excel.藥碼] = contents[i].藥品碼;
                value[(int)enum_盤點定盤_Excel.料號] = contents[i].料號;
                value[(int)enum_盤點定盤_Excel.藥名] = contents[i].藥品名稱;
                value[(int)enum_盤點定盤_Excel.盤點量] = contents[i].盤點量;

          
                
                list_value.Add(value);
            }
            List<System.Data.DataTable> dataTables = new List<System.Data.DataTable>();
            dataTable = list_value.ToDataTable(new enum_盤點定盤_Excel());
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
            dataTables.Add(dataTable);


            for (int i = 0; i < dataTables_creat.Count; i++)
            {
                dataTables.Add(dataTables_creat[i]);
            }
            if (returnData.ValueAry != null)
            {
                for (int i = 0; i < returnData.ValueAry.Count; i++)
                {
                    foreach (System.Data.DataTable dt in dataTables)
                    {
                        dt.Columns.Remove(returnData.ValueAry[i]);
                    }
                }
            }

            returnData.Data = dataTables.JsonSerializeDataTable();
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"成功轉換表單<{dataTables.Count}>張";
            return returnData.JsonSerializationt();
        }
        [HttpPost("get_full_inv_Excel_by_SN")]
        public async Task<ActionResult> POST_get_full_inv_Excel_by_SN([FromBody] returnData returnData)
        {

            string json_out = await POST_get_full_inv_DataTable_by_SN(returnData);
            returnData = json_out.JsonDeserializet<returnData>();
            string dataTable_string = returnData.Data.ObjToClass<string>();
            List<System.Data.DataTable> dataTables = dataTable_string.JsonDeserializeToDataTables();

            if (dataTables == null)
            {
                return null;
            }
            for (int i = 0; i < dataTables.Count; i++)
            {
                if (dataTables[i].Columns.Count > 0)
                {
                    dataTables[i].Columns.RemoveAt(0);
                }
            }

            string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string xls_command = "application/vnd.ms-excel";

            System.Enum[] enums = new System.Enum[] {  enum_盤點定盤_Excel.庫存量, enum_盤點定盤_Excel .盤點量 ,enum_盤點定盤_Excel .單價 ,enum_盤點定盤_Excel .庫存金額 ,enum_盤點定盤_Excel .消耗量 ,
                enum_盤點定盤_Excel.結存金額, enum_盤點定盤_Excel .誤差量 ,enum_盤點定盤_Excel.誤差金額,enum_盤點定盤_Excel.覆盤量 };
            byte[] excelData = ExcelClass.NPOI_GetBytes(dataTables, Excel_Type.xlsx, enums);

            Stream stream = new MemoryStream(excelData);
            return await Task.FromResult(File(stream, xlsx_command, $"{returnData.Value}_InventorySummary.xlsx"));
        }
    }
}
