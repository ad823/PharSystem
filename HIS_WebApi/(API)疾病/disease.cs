using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HIS_DB_Lib;
using Basic;
using MySql.Data.MySqlClient;
using SQLUI;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using H_Pannel_lib;
using Org.BouncyCastle.Bcpg.OpenPgp;
using NPOI.HPSF;
using NPOI.HSSF.Util;
using System.Text.RegularExpressions;
using System.Data;
using MyUI;
using System.IO;
using HIS_DB_Lib;
using System.Net;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using MyOffice;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using NPOI.SS.Formula.Functions;
using Google.Protobuf.WellKnownTypes;
using SkiaSharp;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_疾病
{
    [Route("api/[controller]")]
    [ApiController]
    public class disease : ControllerBase
    {
        static string API_Server = Method.GetServerAPI("Main", "網頁", "API01");
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        /// <summary>
        /// 初始化資料庫
        /// </summary>
        /// <remarks>
        /// {
        ///     
        /// }
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(1, "", typeof(diseaseClass))]
        [HttpPost("init")]
        public string init()
        {
            try
            {
                return CheckCreatTable(null, new enum_disease());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost("load")]
        public string load([FromBody] returnData returnData)
        {
            try
            {

                string filePath = (@"./ICD10-CM.csv");
                //byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                List<object[]> list_value = new List<object[]>();
                List<diseaseClass> diseaseClasses = new List<diseaseClass>();

                using (TextFieldParser parser = new TextFieldParser(filePath, Encoding.UTF8))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.HasFieldsEnclosedInQuotes = true;

                    bool isFirstLine = true;
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (isFirstLine)
                        {
                            isFirstLine = false; // 跳過標題列
                            continue;
                        }
                        list_value.Add(fields);
                    }
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "disease", UserName, Password, Port, SSLMode);
                List<object[]> sqlData = sQLControl.GetAllRows(null);
                List<diseaseClass> diseases = sqlData.SQLToClass<diseaseClass, enum_disease>();
                Dictionary<string, diseaseClass> dict = diseases.ToDictByICD();
                for (int i = 0; i < list_value.Count; i++)
                {
                    //diseaseClass diseaseClass = diseaseClass.GetByICD(dict, list_value[i][(int)enum_disease_EXCEL.code].ObjectToString()).FirstOrDefault();
                    diseaseClass diseaseClass = dict.GetByICD(list_value[i][(int)enum_disease_EXCEL.code].ObjectToString());

                    if (diseaseClass != null) continue;
                    
                    diseaseClass add_diseaseClass = new diseaseClass
                    {
                        GUID = Guid.NewGuid().ToString(),
                        疾病代碼 = list_value[i][(int)enum_disease_EXCEL.code].ObjectToString(),
                        中文說明 = list_value[i][(int)enum_disease_EXCEL.中文名稱].ObjectToString(),
                        英文說明 = list_value[i][(int)enum_disease_EXCEL.英文名稱].ObjectToString()
                    };
                    diseaseClasses.Add(add_diseaseClass);
                }
                init();
                
                List<object[]> add = diseaseClasses.ClassToSQL<diseaseClass, enum_disease>();
                //sQLControl.AddRows(null,add);
                
                returnData.Data = diseaseClasses;
                return returnData.JsonSerializationt(true);
                
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost("load_txt")]
        public string load_txt([FromBody] returnData returnData)
        {
            try
            {
                string loadText = Basic.MyFileStream.LoadFileAllText(@"./ICD10-CM.txt", "utf-8");
                List<diseaseClass> list_value = loadText.JsonDeserializet<List<diseaseClass>>();

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "disease", UserName, Password, Port, SSLMode);
                List<object[]> sqlData = sQLControl.GetAllRows(null);
                List<diseaseClass> diseases = sqlData.SQLToClass<diseaseClass, enum_disease>();
                Dictionary<string, diseaseClass> dict = diseases.ToDictByICD();
                List<diseaseClass> diseaseClasses = new List<diseaseClass>();

                for (int i = 0; i < list_value.Count; i++)
                {
                    //diseaseClass diseaseClass = diseaseClass.GetByICD(dict, list_value[i].疾病代碼).FirstOrDefault();
                    diseaseClass diseaseClass = dict.GetByICD(list_value[i].疾病代碼);

                    if (diseaseClass != null) continue;

                    diseaseClass add_diseaseClass = new diseaseClass
                    {
                        GUID = Guid.NewGuid().ToString(),
                        疾病代碼 = list_value[i].疾病代碼,
                        中文說明 = list_value[i].中文說明,
                        英文說明 = list_value[i].英文說明
                    };
                    diseaseClasses.Add(add_diseaseClass);
                }

                List<object[]> add = diseaseClasses.ClassToSQL<diseaseClass, enum_disease>();
                sQLControl.AddRows(null, add);

                returnData.Data = diseaseClasses;
                return returnData.JsonSerializationt(true);


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 新增/更新資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        diseaseClass
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("update")]
        public string update([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);
                diseaseClass diseaseClass = returnData.Data.ObjToClass<diseaseClass>();
                if (diseaseClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                string 疾病代碼 = diseaseClass.疾病代碼;

                string TableName = "disease";
                SQLControl sQLControl = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                List<object[]> data = sQLControl.GetRowsByDefult(null, (int)enum_disease.疾病代碼, 疾病代碼);
                List<object[]> update_diseaseClass = new List<object[]>();
                List<object[]> add_diseaseClass = new List<object[]>();
                if (data.Count != 0)
                {
                    diseaseClass disease = data.SQLToClass<diseaseClass, enum_disease>()[0];
                    bool flag = false;
                    if (diseaseClass.中文說明 != disease.中文說明) flag = true;
                    if (diseaseClass.英文說明 != disease.英文說明) flag = true;
                    diseaseClass.GUID = disease.GUID;
                    if (flag) update_diseaseClass = new List<diseaseClass>() { diseaseClass }.ClassToSQL<diseaseClass, enum_disease>();
                }
                else
                {
                    add_diseaseClass = new List<diseaseClass>() { diseaseClass }.ClassToSQL<diseaseClass, enum_disease>();
                }
                if (add_diseaseClass.Count > 0) sQLControl.AddRows(null, add_diseaseClass);
                if (update_diseaseClass.Count > 0) sQLControl.UpdateByDefulteExtra(null, update_diseaseClass);

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = diseaseClass;
                returnData.Result = $"新增{add_diseaseClass.Count}筆資料，更新{update_diseaseClass.Count}筆資料";
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
        /// 新增/更新資料
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        diseaseClass
        ///     }
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("update_server")]
        public string update_server([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "update_server";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

                string TableName = "disease";
                SQLControl sQLControl = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);
                SQLControl sQLControl_Server = new SQLControl(, "dbvm", TableName, "user", "66437068", 3306, SSLMode);

                List<object[]> data = sQLControl.GetAllRows(null);
                List<object[]> data_server = sQLControl_Server.GetAllRows(null);

                List<diseaseClass> diseaseClasses = data.SQLToClass<diseaseClass, enum_disease>();
                List<diseaseClass> diseaseClasses_server = data_server.SQLToClass<diseaseClass, enum_disease>();
                Dictionary<string, diseaseClass> Dic_disease = diseaseClasses.ToDictByICD();
                Dictionary<string, diseaseClass> Dic_disease_server = diseaseClasses_server.ToDictByICD();
                List<diseaseClass> add_disease = new List<diseaseClass>();
                List<diseaseClass> update_disease = new List<diseaseClass>();

               
                foreach(string ICD in Dic_disease.Keys)
                {
                    diseaseClass diseases = Dic_disease.GetByICD(ICD);
                    diseaseClass diseases_server = Dic_disease_server.GetByICD(ICD);
                    if (diseases_server.疾病代碼.StringIsEmpty())
                    {
                        add_disease.Add(diseases);
                    }
                    else
                    {
                        bool flag = false;
                        if (diseases_server.中文說明 != diseases.中文說明) flag = true;
                        if (diseases_server.英文說明 != diseases.英文說明) flag = true;
                        diseases.GUID = diseases_server.GUID;
                        if (flag) update_disease.Add(diseases);
                    }                      
                }
                List<object[]> add_diseaseClass = add_disease.ClassToSQL<diseaseClass, enum_disease>();
                List<object[]> update_diseaseClass = update_disease.ClassToSQL<diseaseClass, enum_disease>();
                if (add_diseaseClass.Count > 0) sQLControl.AddRows(null, add_diseaseClass);
                if (update_diseaseClass.Count > 0) sQLControl.UpdateByDefulteExtra(null, update_diseaseClass);

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = diseaseClasses;
                returnData.Result = $"新增{add_diseaseClass.Count}筆資料，更新{update_diseaseClass.Count}筆資料";
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }
        [HttpPost("get_by_ICD")]
        public string get_by_ICD([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_by_ICD";
            try
            {
                returnData.RequestUrl = Method.GetRequestPath(HttpContext, includeQuery: false);

                if (returnData.ValueAry.Count != 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry應該為[\"疾病代碼\"]";
                    return returnData.JsonSerializationt();
                }
                string 疾病代碼 = returnData.ValueAry[0];
                if (疾病代碼.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry應該為[\"疾病代碼\"]";
                    return returnData.JsonSerializationt();
                }
                string[] 疾病代碼_Array = 疾病代碼.Split(";");
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

                string TableName = "disease";
                SQLControl sQLControl = new SQLControl(Server, DB, TableName, UserName, Password, Port, SSLMode);                            
                string command = $"SELECT * FROM {DB}.{TableName} WHERE"; 

                for (int i = 0; i < 疾病代碼_Array.Length; i++)
                {
                    if (i == 0)
                    {
                        command += $" 疾病代碼  = '{疾病代碼_Array[i]}'";
                    }
                    else if(i == 疾病代碼_Array.Length-1)
                    {
                        command += $" OR 疾病代碼 = '{疾病代碼_Array[i]}';";
                    }
                    else
                    {
                        command += $" OR 疾病代碼 = '{疾病代碼_Array[i]}'";
                    }
                }
                DataTable dataTable = sQLControl.WtrteCommandAndExecuteReader(command);           
                List<object[]> list_diseaseClass = dataTable.DataTableToRowList();
                List<diseaseClass> diseaseClasses = list_diseaseClass.SQLToClass<diseaseClass, enum_disease>();
                if (diseaseClasses == null) diseaseClasses = new List<diseaseClass>();
                if (diseaseClasses.Count == 0 || diseaseClasses.Count != 疾病代碼_Array.Length)
                {
                    foreach(var item in 疾病代碼_Array)
                    {
                        diseaseClass diseaseClass_buff = diseaseClasses.Find(temp => temp.疾病代碼 == item);
                        if (diseaseClass_buff != null) continue;
                        string icd10_url = $"https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search?df=code,name&terms={item}";

                        string icd10_json = Basic.Net.WEBApiGet(icd10_url);
                        CodeDescription iCD10Data = CodeDescription.ParseCodeDescription(icd10_json);
                        diseaseClass diseaseClass = new diseaseClass();
                        string 疾病代碼_ = string.Empty;
                        string 英文說明_ = string.Empty;
                        if (iCD10Data != null) 英文說明_ = iCD10Data.Description;
                        diseaseClass.GUID = Guid.NewGuid().ToString();

                        diseaseClass.疾病代碼 = item;
                        diseaseClass.英文說明 = 英文說明_;

                        returnData returnData_update = new returnData();
                        returnData_update.Data = diseaseClass;
                        update(returnData_update);
                        diseaseClasses.Add(diseaseClass);
                    }
                }
                
                returnData.Data = diseaseClasses;
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Result = $"取得疾病資料共<{diseaseClasses.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }

        private string CheckCreatTable(string tableName, System.Enum Enum)
        {
            if (tableName == null)
            {
                tableName = Enum.GetEnumDescription();
            }
            SQLUI.Table table = new SQLUI.Table(tableName);
            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料");
            if (sys_serverSettingClasses.Count == 0)
            {
                return $"找無Server資料!";
            }
            table = MethodClass.CheckCreatTable(sys_serverSettingClasses[0], Enum);

            return table.JsonSerializationt(true);
        }

        public class CodeDescription
        {
            public string Code { get; set; }
            public string Description { get; set; }

            public CodeDescription() { }

            public CodeDescription(string code, string description)
            {
                Code = code;
                Description = description;
            }

            // 若來源為 string[] 陣列
            public CodeDescription(string[] array)
            {
                if (array?.Length == 2)
                {
                    Code = array[0];
                    Description = array[1];
                }
            }

            public static CodeDescription ParseCodeDescription(string json)
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    var lastArray = root[3]; // 第四個元素是 [["R05.9", "Cough, unspecified"]]

                    if (lastArray.GetArrayLength() > 0)
                    {
                        var item = lastArray[0];
                        return new CodeDescription
                        {
                            Code = item[0].GetString(),
                            Description = item[1].GetString()
                        };
                    }
                }
                return null;
            }
        }
    }
}
