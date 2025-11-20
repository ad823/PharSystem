using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Data;
using NPOI.HPSF;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class inv_combinelist : Controller
    {
        static string API01 = "http://127.0.0.1:4433";
        private class SheetTemp
        {
            public SheetTemp(string name)
            {
                Name = name;
            }
            public string Name = "";
            public List<object[]> list_value = new List<object[]>();
        }

        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        /// <summary>
        /// 初始化合併單資料庫
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
        [HttpPost]
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(200, "inv_combinelist_dataTable物件", typeof(inv_combinelist_report_Class))]
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
                string msg = "";
                return msg;
            }
        }
        /// <summary>
        /// 取得可建立今日最新合併單號
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
        /// <returns>[returnData.Value]為建立合併單號</returns>
        [Route("new_IC_SN")]
        [HttpPost]
        public string GET_new_IC_SN([FromBody] returnData returnData)
        {
            GET_init(returnData);
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

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


            SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inv_sub_combinelist = new SQLControl(Server, DB, "inv_sub_combinelist", UserName, Password, Port, SSLMode);

            List<object[]> list_inv_combinelist = sQLControl_inv_combinelist.GetAllRows(null);
            List<object[]> list_inv_combinelist_buf = new List<object[]>();

            list_inv_combinelist_buf = list_inv_combinelist.GetRowsInDate((int)enum_inv_combinelist.建表時間, DateTime.Now);


            string 單號 = "";
            int index = 0;
            while (true)
            {
                單號 = $"I{DateTime.Now.ToDateTinyString()}-{index}";
                index++;
                list_inv_combinelist_buf = list_inv_combinelist.GetRows((int)enum_inv_combinelist.合併單號, 單號);
                if (list_inv_combinelist_buf.Count == 0) break;
            }
            returnData.Value = 單號;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "new_IC_SN";
            returnData.Result = $"成功! {myTimer.ToString()}";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 取得今日可建立最新覆盤單號
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
        /// <returns>[returnData.Value]為建立盤點單號</returns>
        [HttpPost("new_IC_SN_review")]
        public string new_IC_SN_review([FromBody] returnData returnData)
        {
            if (returnData.Value.StringIsEmpty())
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 應該為合併單號";
                return returnData.JsonSerializationt(true);
            }
            string 合併單號 = returnData.Value;
            MyTimerBasic myTimerBasic = new MyTimerBasic();

            (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);

            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
            List<object[]> list_inventory_creat_time = new List<object[]>();
            List<object[]> list_inventory_creat_SN = new List<object[]>();
            string 覆盤單號 = "";
            inventoryClass.creat creat = new inventoryClass.creat();


            list_inventory_creat_time = list_inventory_creat.GetRowsInDate((int)enum_盤點單號.建表時間, DateTime.Now);
            list_inventory_creat_SN = list_inventory_creat.GetRowsByLike((int)enum_盤點單號.合併單號, 合併單號);

            if(list_inventory_creat_SN.Count > 0)
            {
                覆盤單號 = list_inventory_creat_SN[0][(int)enum_盤點單號.盤點單號].ToString();
                returnData.Value = 覆盤單號;
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "new_IC_SN_review";
                returnData.Result = $"已存在合併單號{合併單號}的覆盤單{覆盤單號}";
                return returnData.JsonSerializationt(true);
            }

            int index = 0;
            while (true)
            {
                覆盤單號 = $"REV{DateTime.Now.ToDateTinyString()}-{index}";
                index++;
                list_inventory_creat_time = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, 覆盤單號);
                if (list_inventory_creat_time.Count == 0) break;
            }
            creat.GUID = Guid.NewGuid().ToString();
            creat.盤點名稱 = "";
            creat.類型 = "覆盤單";
            creat.盤點單號 = "覆盤單號";
            creat.建表人 = "";
            creat.建表時間 = DateTime.Now.ToDateString();
            creat.盤點開始時間 = DateTime.MinValue.ToDateTimeString();
            creat.盤點結束時間 = DateTime.MinValue.ToDateTimeString();
            creat.盤點狀態 = "等待盤點";
            creat.合併單號 = 合併單號;

            returnData.Value = 覆盤單號;
            returnData.Code = 200;
            returnData.TimeTaken = myTimerBasic.ToString();
            returnData.Method = "new_IC_SN_review";
            returnData.Result = $"成功! {myTimerBasic.ToString()}";
            return returnData.JsonSerializationt(true);
        }
        [HttpPost("review_creat_add")]
        public string review_creat_add([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

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

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_content = new SQLControl(Server, DB, "inventory_content", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inventory_sub_content = new SQLControl(Server, DB, "inventory_sub_content", UserName, Password, Port, SSLMode);
            inventoryClass.creat creat = returnData.Data.ObjToClass<inventoryClass.creat>();

            if (creat == null)
            {
                returnData.Code = -5;
                returnData.Result += $"Data 資料錯誤 \n";
                return returnData.JsonSerializationt();
            }

            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetRowsByDefult(null,(int)enum_盤點單號.盤點單號, creat.盤點單號);
            if (list_inventory_creat.Count > 0)
            {
                List<object[]> list_inventory_content = sQLControl_inventory_content.GetRowsByDefult(null, (int)enum_盤點內容.盤點單號, creat.盤點單號);
                List<object[]> list_inventory_sub_content = sQLControl_inventory_sub_content.GetRowsByDefult(null, (int)enum_盤點明細.盤點單號, creat.盤點單號);

                if (list_inventory_creat.Count > 0) sQLControl_inventory_creat.DeleteExtra(null, list_inventory_creat);
                if (list_inventory_content.Count > 0) sQLControl_inventory_content.DeleteExtra(null, list_inventory_content);
                if (list_inventory_sub_content.Count > 0) sQLControl_inventory_sub_content.DeleteExtra(null, list_inventory_sub_content);
            }
            creat.GUID = Guid.NewGuid().ToString();
            creat.建表時間 = DateTime.Now.ToDateTimeString();
            creat.盤點開始時間 = DateTime.MaxValue.ToDateTimeString();
            creat.盤點結束時間 = DateTime.MaxValue.ToDateTimeString();

            List<object[]> list_inventory_creat_add = new List<object[]>();
            List<object[]> list_inventory_content_add = new List<object[]>();
            List<object[]> list_inventory_sub_content_add = new List<object[]>();
            object[] value;
            value = new object[new enum_盤點單號().GetLength()];

            value[(int)enum_盤點單號.GUID] = creat.GUID;
            value[(int)enum_盤點單號.類型] = creat.類型;
            value[(int)enum_盤點單號.合併單號] = creat.合併單號;
            value[(int)enum_盤點單號.盤點單號] = creat.盤點單號;
            value[(int)enum_盤點單號.盤點名稱] = creat.盤點名稱;
            value[(int)enum_盤點單號.建表人] = creat.建表人;
            value[(int)enum_盤點單號.建表時間] = creat.建表時間;
            value[(int)enum_盤點單號.盤點開始時間] = creat.盤點開始時間;
            value[(int)enum_盤點單號.盤點結束時間] = creat.盤點結束時間;
            value[(int)enum_盤點單號.預設盤點人] = creat.預設盤點人;
            value[(int)enum_盤點單號.盤點狀態] = "等待盤點";
            value[(int)enum_盤點單號.備註] = creat.備註;

            list_inventory_creat_add.Add(value);
            

            for (int i = 0; i < creat.Contents.Count; i++)
            {
                value = new object[new enum_盤點內容().GetLength()];
                creat.Contents[i].GUID = Guid.NewGuid().ToString();
                creat.Contents[i].新增時間 = DateTime.Now.ToDateTimeString();
                creat.Contents[i].Master_GUID = creat.GUID;
                creat.Contents[i].盤點單號 = creat.盤點單號;
                value[(int)enum_盤點內容.GUID] = creat.Contents[i].GUID;
                value[(int)enum_盤點內容.Master_GUID] = creat.Contents[i].Master_GUID;
                value[(int)enum_盤點內容.序號] = creat.Contents[i].序號;
                value[(int)enum_盤點內容.儲位名稱] = creat.Contents[i].儲位名稱;
                value[(int)enum_盤點內容.藥品碼] = creat.Contents[i].藥品碼;
                value[(int)enum_盤點內容.料號] = creat.Contents[i].料號;
                value[(int)enum_盤點內容.藥品條碼1] = creat.Contents[i].藥品條碼1;
                value[(int)enum_盤點內容.藥品條碼2] = creat.Contents[i].藥品條碼2;
                value[(int)enum_盤點內容.盤點單號] = creat.Contents[i].盤點單號;
                value[(int)enum_盤點內容.理論值] = creat.Contents[i].理論值;
                value[(int)enum_盤點內容.新增時間] = creat.Contents[i].新增時間;
                value[(int)enum_盤點內容.備註] = creat.Contents[i].備註;
                for (int k = 0; k < creat.Contents[i].Sub_content.Count; k++)
                {
                    object[] sub_value = new object[new enum_盤點明細().GetLength()];
                    sub_value = creat.Contents[i].Sub_content[k].ClassToSQL<inventoryClass.sub_content, enum_盤點明細>();
                    sub_value[(int)enum_盤點明細.GUID] = Guid.NewGuid().ToString();
                    sub_value[(int)enum_盤點明細.Master_GUID] = creat.Contents[i].GUID;
                    sub_value[(int)enum_盤點明細.操作時間] = DateTime.Now.ToDateTimeString_6();
                    sub_value[(int)enum_盤點明細.盤點單號] = creat.盤點單號;
                    list_inventory_sub_content_add.Add(sub_value);
                }
                list_inventory_content_add.Add(value);
            }
            sQLControl_inventory_creat.AddRows(null, list_inventory_creat_add);
            sQLControl_inventory_content.AddRows(null, list_inventory_content_add);
            sQLControl_inventory_sub_content.AddRows(null, list_inventory_sub_content_add);
            returnData.Data = creat;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "creat_add";

            returnData.Result = $"成功加入新覆盤單! 共{list_inventory_content_add.Count}筆資料";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 取得今日可建立最新覆盤單號
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///         "CT":"建表人",
        ///         "IC_NAME":"覆盤單名稱"
        ///     }
        ///     "Value":"合併單號"
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Value]為建立盤點單號</returns>
        [HttpPost("review_auto_add")]
        public string review_auto_add([FromBody] returnData returnData)
        {
            try
            {
                if (returnData.Value.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Value 應輸入\"合併單號\"";
                    return returnData.JsonSerializationt(true);
                }
                if(returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data 應輸入 inventoryClass.creat";
                    return returnData.JsonSerializationt(true);
                }
                inventoryClass.creat creat = returnData.Data.ObjToClass<inventoryClass.creat>();
                string 合併單號 = returnData.Value;
                List<Task> tasks = new List<Task>();
                inv_combinelistClass inv_CombinelistClass = new inv_combinelistClass();
                List<inv_combinelist_report_Class> inv_combinelist_report_Classes = new List<inv_combinelist_report_Class>();
                string str_REV_SN = string.Empty;
                tasks.Add(Task.Run(new Action(delegate 
                {
                    returnData returnData_get_full_inv_by_SN = new returnData();
                    returnData_get_full_inv_by_SN.Value = 合併單號;
                    string jsonString = get_full_inv_by_SN(returnData_get_full_inv_by_SN);
                    returnData_get_full_inv_by_SN = jsonString.JsonDeserializet<returnData>();
                    inv_CombinelistClass = returnData_get_full_inv_by_SN.Data.ObjToClass<inv_combinelistClass>();
                })));
                tasks.Add(Task.Run(new Action(delegate 
                {
                    returnData returnData_GET_new_IC_SN = new returnData();
                    returnData_GET_new_IC_SN.Value = 合併單號;
                    returnData_GET_new_IC_SN = new_IC_SN_review(returnData_GET_new_IC_SN).JsonDeserializet<returnData>();
                    str_REV_SN = returnData_GET_new_IC_SN.Value;
                })));
                                
                tasks.Add(Task.Run(new Action(delegate
                {
                    returnData returnData_get_detail_inv_by_SN = new returnData();
                    returnData_get_detail_inv_by_SN.Value = 合併單號;
                    string jsonString = get_report_by_SN(returnData_get_detail_inv_by_SN);
                    returnData_get_detail_inv_by_SN = jsonString.JsonDeserializet<returnData>();
                    inv_combinelist_report_Classes = returnData_get_detail_inv_by_SN.Data.ObjToClass<List<inv_combinelist_report_Class>>();
                })));
                Task.WhenAll(tasks).Wait();
                tasks.Clear();

                if (inv_CombinelistClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "FUNTION get_full_inv_by_SN 發生錯誤";
                    return returnData.JsonSerializationt(true);
                }
                if (str_REV_SN.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "FUNTION new_IC_SN_review 發生錯誤";
                    return returnData.JsonSerializationt(true);
                }
                //檢查所有盤點單都鎖單
            
                bool flag_鎖單 = false;
                string 盤點單號 = string.Empty;
                for(int i = 0; i < inv_CombinelistClass.Records_Ary.Count; i++)
                {
                    inventoryClass.creat _creat = inv_CombinelistClass.Records_Ary[i].Creat;

                    if (_creat.盤點狀態 != "鎖定") 
                    {
                        flag_鎖單 = true;
                        盤點單號 = _creat.盤點單號;
                    } 
                    
                    break;
                }
                if (flag_鎖單)
                {
                    returnData.Code = -200;
                    returnData.Result = $"此合併單{合併單號}內的盤點單{盤點單號}沒有鎖定!!";
                    returnData.Data = string.Empty;
                    return returnData.JsonSerializationt(true);
                }
                List<medClass> medClasses = medClass.get_med_cloud(API01);
                Dictionary<string, List<medClass>> dic_medClass = medClasses.CoverToDictionaryByCode();
                
                
                creat.盤點單號 = str_REV_SN;
                creat.合併單號 = 合併單號;
                creat.類型 = "覆盤單";
                //if(inv_Combinelist_DataTables.Count > 0)
                //{
                //   return inv_Combinelist_DataTables.JsonSerializationt(true);
                //}
                for (int i = 0; i < inv_combinelist_report_Classes.Count; i++)
                {
                    if (inv_combinelist_report_Classes[i].註記.Contains("覆盤") == false) continue;
                    inventoryClass.content content = new inventoryClass.content();
                    string 藥碼 = inv_combinelist_report_Classes[i].藥碼;
                    List<medClass> medClass_buff = dic_medClass.SortDictionaryByCode(藥碼);
                    if(medClass_buff.Count > 0)
                    {
                        medClass medClass = medClass_buff.Where(item => item.開檔狀態.Contains("關檔中") == false).FirstOrDefault();
                        if (medClass != null) medClass_buff[0] = medClass;

                        content.GUID = Guid.NewGuid().ToString();
                        content.藥品碼 = medClass_buff[0].藥品碼;
                        content.藥品名稱 = medClass_buff[0].藥品名稱;
                        content.中文名稱 = medClass_buff[0].中文名稱;
                        content.料號 = medClass_buff[0].料號;
                        content.藥品條碼1 = medClass_buff[0].藥品條碼1;
                        content.藥品條碼2 = medClass_buff[0].藥品條碼2;
                        content.包裝單位 = medClass_buff[0].包裝單位;
                        content.新增時間 = DateTime.Now.ToDateTimeString();
                        content.盤點單號 = str_REV_SN;
                        content.理論值 = "0";
                        creat.Contents.Add(content);
                    }
                }
                if (creat.Contents.Count == 0)
                {
                    returnData.Code = -6;
                    returnData.Value = "無覆盤資料可新增!";
                    return returnData.JsonSerializationt();
                }
                returnData.Data = creat;
                returnData.Method = "creat_auto_add";
                return review_creat_add(returnData);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }
        }
        /// <summary>
        /// 創建合併單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單名稱 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///        "INV_NAME": "測試合併",
        ///        "INV_SN": "I20240103-14",
        ///        "CT": "王曉明",
        ///        "NOTE": "",
        ///        "records_Ary" :
        ///        [
        ///          {
        ///             "SN" : "20240103-1",
        ///             "TYPE" : "盤點單"
        ///          },
        ///          {
        ///             "SN" : "20240102-0",
        ///             "TYPE" : "盤點單"
        ///          }
        ///        ]
        ///     }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [Route("inv_creat_update")]
        [HttpPost]
        public string POST_inv_creat_update([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
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

            SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inv_sub_combinelist = new SQLControl(Server, DB, "inv_sub_combinelist", UserName, Password, Port, SSLMode);
            inv_combinelistClass inv_CombinelistClass = returnData.Data.ObjToClass<inv_combinelistClass>();

            List<object[]> list_inv_combinelist = sQLControl_inv_combinelist.GetAllRows(null);
            List<object[]> list_inv_combinelist_buf = new List<object[]>();

            List<object[]> list_inv_sub_combinelist = sQLControl_inv_sub_combinelist.GetAllRows(null);
            List<object[]> list_inv_sub_combinelist_buf = new List<object[]>();
            if (inv_CombinelistClass.合併單號.StringIsEmpty() == true)
            {
                returnData returnData_buf = new returnData();
                returnData_buf = GET_new_IC_SN(returnData).JsonDeserializet<returnData>();
                if (returnData_buf.Code != 200)
                {
                    returnData.Code = -200;
                    returnData.Result = "inv_CombinelistClass.合併單號, 空白,請輸入合併單號!";
                    return returnData.JsonSerializationt();
                }
                inv_CombinelistClass.合併單號 = returnData.Value;
            }
            if (inv_CombinelistClass == null)
            {
                returnData.Code = -200;
                returnData.Result += $"Data 資料錯誤 \n";
                return returnData.JsonSerializationt();
            }
            //returnData returnData_SN = GET_new_IC_SN(returnData).JsonDeserializet<returnData>();
            //inv_CombinelistClass.合併單號 = returnData_SN.Value;
            list_inv_combinelist_buf = list_inv_combinelist.GetRows((int)enum_inv_combinelist.合併單號, inv_CombinelistClass.合併單號);
            list_inv_sub_combinelist_buf = list_inv_sub_combinelist.GetRows((int)enum_inv_sub_combinelist.合併單號, inv_CombinelistClass.合併單號);

            if (list_inv_combinelist_buf.Count > 0) sQLControl_inv_combinelist.DeleteExtra(null, list_inv_combinelist_buf);
            if (list_inv_sub_combinelist_buf.Count > 0) sQLControl_inv_sub_combinelist.DeleteExtra(null, list_inv_sub_combinelist_buf);


            inv_CombinelistClass.GUID = Guid.NewGuid().ToString();
            inv_CombinelistClass.建表時間 = DateTime.Now.ToDateTimeString();


            List<object[]> list_inv_Combinelist_add = new List<object[]>();
            List<object[]> list_inv_sub_combinelist_add = new List<object[]>();
            inv_CombinelistClass.消耗量起始時間 = DateTime.MinValue.ToDateTimeString();
            inv_CombinelistClass.消耗量結束時間 = DateTime.MinValue.ToDateTimeString();
            if (inv_CombinelistClass.誤差總金額致能.StringIsEmpty()) inv_CombinelistClass.誤差總金額致能 = false.ToString();
            if (inv_CombinelistClass.誤差百分率致能.StringIsEmpty()) inv_CombinelistClass.誤差百分率致能 = false.ToString();
            if (inv_CombinelistClass.誤差數量致能.StringIsEmpty()) inv_CombinelistClass.誤差數量致能 = false.ToString();

            object[] value;
            value = inv_CombinelistClass.ClassToSQL<inv_combinelistClass, enum_inv_combinelist>();

            list_inv_Combinelist_add.Add(value);

            for (int i = 0; i < inv_CombinelistClass.Records_Ary.Count; i++)
            {
                value = new object[new enum_inv_sub_combinelist().GetLength()];
                value[(int)enum_inv_sub_combinelist.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_inv_sub_combinelist.Master_GUID] = inv_CombinelistClass.GUID;
                value[(int)enum_inv_sub_combinelist.合併單號] = inv_CombinelistClass.合併單號;
                value[(int)enum_inv_sub_combinelist.單號] = inv_CombinelistClass.Records_Ary[i].單號;
                value[(int)enum_inv_sub_combinelist.類型] = inv_CombinelistClass.Records_Ary[i].類型;
                value[(int)enum_inv_sub_combinelist.新增時間] = DateTime.Now.ToDateTimeString();
                value[(int)enum_inv_sub_combinelist.新增時間] = DateTime.Now.ToDateTimeString();
                value[(int)enum_inv_sub_combinelist.新增時間] = DateTime.Now.ToDateTimeString();

                list_inv_sub_combinelist_add.Add(value);
            }

            sQLControl_inv_combinelist.AddRows(null, list_inv_Combinelist_add);
            sQLControl_inv_sub_combinelist.AddRows(null, list_inv_sub_combinelist_add);
            returnData.Data = inv_CombinelistClass;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "inv_creat_update";

            returnData.Result = $"成功加入新合併單! 共{list_inv_sub_combinelist_add.Count}筆明細資料";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以GUID更新 盤點日庫存單號
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "GUID",
        ///       "StockRecord_GUID"
        ///       "StockRecord_ServerName"
        ///       "StockRecord_ServerType"
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("inv_stockrecord_update_by_GUID")]
        public string POST_inv_stockrecord_update_by_GUID([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
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
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 4)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID],[StockRecord_GUID],[StockRecord_ServerName],[StockRecord_ServerType]";
                    return returnData.JsonSerializationt(true);
                }
                string GUID = returnData.ValueAry[0];
                string StockRecord_GUID = returnData.ValueAry[1];
                string StockRecord_ServerName = returnData.ValueAry[2];
                string StockRecord_ServerType = returnData.ValueAry[3];
                if (GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Value [GUID]不得為空白";
                    return returnData.JsonSerializationt(true);
                }



                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);

                List<object[]> list_inv_combinelist = sQLControl_inv_combinelist.GetRowsByDefult(null, (int)enum_inv_combinelist.GUID, GUID);
                if (list_inv_combinelist.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> list_inv_combinelist_buf = new List<object[]>();


                object[] value;
                value = list_inv_combinelist[0];
                value[(int)enum_inv_combinelist.StockRecord_GUID] = StockRecord_GUID;
                value[(int)enum_inv_combinelist.StockRecord_ServerName] = StockRecord_ServerName;
                value[(int)enum_inv_combinelist.StockRecord_ServerType] = StockRecord_ServerType;

                sQLControl_inv_combinelist.UpdateByDefulteExtra(null, value);
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Method = "inv_stockrecord_update_by_GUID";
                returnData.Result = $"更新盤點日庫存單號成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以GUID更新 誤差資料
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///         "GUID",
        ///         "MaxTotalErrorAmount"
        ///         "MinTotalErrorAmount"
        ///         "MaxErrorPercentage"
        ///         "MinErrorPercentage"
        ///         "MaxErrorCount"
        ///         "MinErrorCount"
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///     
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("update_error_by_GUID")]
        public string update_error_by_GUID([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                inv_combinelistClass inv_CombinelistClass = returnData.Data.ObjToClass<inv_combinelistClass>();

                if (inv_CombinelistClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入Data資料異常";
                    return returnData.JsonSerializationt();
                }
                (string Server, string DB, string UserName, string Password, uint Port) = Method.GetServerInfo("Main", "網頁", "VM端");
                string GUID = inv_CombinelistClass.GUID;
                SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);

                List<object[]> list_inv_combinelist = sQLControl_inv_combinelist.GetRowsByDefult(null, (int)enum_inv_combinelist.GUID, GUID);
                if (list_inv_combinelist.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt(true);
                }

                if (inv_CombinelistClass.誤差數量上限.StringIsEmpty() == false) list_inv_combinelist[0][(int)enum_inv_combinelist.誤差數量上限] = inv_CombinelistClass.誤差數量上限;
                if (inv_CombinelistClass.誤差數量下限.StringIsEmpty() == false) list_inv_combinelist[0][(int)enum_inv_combinelist.誤差數量下限] = inv_CombinelistClass.誤差數量下限;
                if (inv_CombinelistClass.誤差百分率上限.StringIsEmpty() == false) list_inv_combinelist[0][(int)enum_inv_combinelist.誤差百分率上限] = inv_CombinelistClass.誤差百分率上限;
                if (inv_CombinelistClass.誤差百分率下限.StringIsEmpty() == false) list_inv_combinelist[0][(int)enum_inv_combinelist.誤差百分率下限] = inv_CombinelistClass.誤差百分率下限;
                if (inv_CombinelistClass.誤差總金額上限.StringIsEmpty() == false) list_inv_combinelist[0][(int)enum_inv_combinelist.誤差總金額上限] = inv_CombinelistClass.誤差總金額上限;
                if (inv_CombinelistClass.誤差總金額下限.StringIsEmpty() == false) list_inv_combinelist[0][(int)enum_inv_combinelist.誤差總金額下限] = inv_CombinelistClass.誤差總金額下限;

                sQLControl_inv_combinelist.UpdateByDefulteExtra(null, list_inv_combinelist);
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_error_by_GUID";
                returnData.Result = $"更新誤差設定成功!";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以GUID更新 消耗量計算起始及結束日期
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        /// {
        ///     "Data": 
        ///     {
        ///  
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "GUID",
        ///       "startTime"
        ///       "endTime"
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("inv_consumption_time_update_by_GUID")]
        public string POST_inv_consumption_time_update_by_GUID([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
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
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 3)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[GUID],[startTime],[endTime]";
                    return returnData.JsonSerializationt(true);
                }
                string GUID = returnData.ValueAry[0];
                string startTime = returnData.ValueAry[1];
                string endTime = returnData.ValueAry[2];
                if (GUID.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Value [GUID]不得為空白";
                    return returnData.JsonSerializationt(true);
                }
                if (startTime.Check_Date_String() == false || endTime.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Value [startTime]或[endTime] 日期格式錯誤";
                    return returnData.JsonSerializationt(true);
                }


                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

                SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);

                List<object[]> list_inv_combinelist = sQLControl_inv_combinelist.GetRowsByDefult(null, (int)enum_inv_combinelist.GUID, GUID);
                if (list_inv_combinelist.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"查無資料";
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> list_inv_combinelist_buf = new List<object[]>();


                object[] value;
                value = list_inv_combinelist[0];
                value[(int)enum_inv_combinelist.消耗量起始時間] = startTime;
                value[(int)enum_inv_combinelist.消耗量結束時間] = endTime;

                sQLControl_inv_combinelist.UpdateByDefulteExtra(null, value);
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Method = "inv_stockrecord_update_by_GUID";
                returnData.Result = $"更新消耗量計算起始及結束日期成功! 起始時間[{startTime}] 結束時間[{endTime}]";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 取得所有合併單資料
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單名稱 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [Route("get_all_inv")]
        [HttpPost]
        public string POST_get_all_inv([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
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

            SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inv_sub_combinelist = new SQLControl(Server, DB, "inv_sub_combinelist", UserName, Password, Port, SSLMode);

            List<object[]> list_inv_combinelist = sQLControl_inv_combinelist.GetAllRows(null);
            List<object[]> list_inv_combinelist_buf = new List<object[]>();

            List<object[]> list_inv_sub_combinelist = sQLControl_inv_sub_combinelist.GetAllRows(null);
            List<object[]> list_inv_sub_combinelist_buf = new List<object[]>();


            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
            List<object[]> list_inventory_creat_buf = new List<object[]>();

            List<inv_combinelistClass> inv_CombinelistClasses = new List<inv_combinelistClass>();
            string 合併單號 = "";

            for (int i = 0; i < list_inv_combinelist.Count; i++)
            {
                合併單號 = list_inv_combinelist[i][(int)enum_inv_combinelist.合併單號].ObjectToString();
                list_inv_combinelist_buf = list_inv_combinelist.GetRows((int)enum_inv_combinelist.合併單號, 合併單號);
                list_inv_sub_combinelist_buf = list_inv_sub_combinelist.GetRows((int)enum_inv_sub_combinelist.合併單號, 合併單號);
                inv_combinelistClass inv_CombinelistClass = list_inv_combinelist_buf[0].SQLToClass<inv_combinelistClass, enum_inv_combinelist>();
                List<inv_records_Class> inv_Sub_CombinelistClasses = list_inv_sub_combinelist_buf.SQLToClass<inv_records_Class, enum_inv_sub_combinelist>();
                List<inv_records_Class> inv_Sub_CombinelistClasses_buf = new List<inv_records_Class>();
                for (int k = 0; k < inv_Sub_CombinelistClasses.Count; k++)
                {
                    string 單號 = inv_Sub_CombinelistClasses[k].單號;
                    string 類型 = inv_Sub_CombinelistClasses[k].類型;
                    if (類型 == "盤點單")
                    {
                        list_inventory_creat_buf = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, 單號);
                        if (list_inventory_creat_buf.Count > 0)
                        {
                            inv_Sub_CombinelistClasses[k].名稱 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點名稱].ObjectToString();
                            inv_Sub_CombinelistClasses[k].單號 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點單號].ObjectToString();
                            inv_Sub_CombinelistClasses[k].類型 = "盤點單";
                            inv_Sub_CombinelistClasses_buf.Add(inv_Sub_CombinelistClasses[k]);
                        }
                    }
                }
                inv_CombinelistClass.Records_Ary = inv_Sub_CombinelistClasses_buf;
                inv_CombinelistClasses.Add(inv_CombinelistClass);
            }

            inv_CombinelistClasses.Sort(new inv_combinelistClass.ICP_By_CT_time());
            returnData.Data = inv_CombinelistClasses;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "get_all_inv";

            returnData.Result = $"成功取得資料";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以合併單號取得合併單資料
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單名稱 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///    
        ///    },
        ///    "Value":"合併單號"
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [Route("get_inv_by_SN")]
        [HttpPost]
        public string get_inv_by_SN([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            if (returnData.Value.StringIsEmpty())
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value應填入合併單號";
                return returnData.JsonSerializationt(true);
            }
            string 合併單號 = returnData.Value;
            (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");           

            SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inv_sub_combinelist = new SQLControl(Server, DB, "inv_sub_combinelist", UserName, Password, Port, SSLMode);

            List<object[]> list_inv_combinelist = sQLControl_inv_combinelist.GetRowsByDefult(null,(int)enum_inv_combinelist.合併單號, 合併單號);
            inv_combinelistClass inv_CombinelistClass = list_inv_combinelist[0].SQLToClass<inv_combinelistClass, enum_inv_combinelist>();

            List<object[]> list_inv_sub_combinelist = sQLControl_inv_sub_combinelist.GetRowsByDefult(null,(int)enum_inv_sub_combinelist.合併單號, 合併單號);
            List<inv_records_Class> inv_Sub_CombinelistClasses = list_inv_sub_combinelist.SQLToClass<inv_records_Class, enum_inv_sub_combinelist>();



            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);
            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);
            List<object[]> list_inventory_creat_buf = new List<object[]>();

            List<inv_combinelistClass> inv_CombinelistClasses = new List<inv_combinelistClass>();

            
           
            List<inv_records_Class> inv_Sub_CombinelistClasses_buf = new List<inv_records_Class>();
            for (int k = 0; k < inv_Sub_CombinelistClasses.Count; k++)
            {
                string 單號 = inv_Sub_CombinelistClasses[k].單號;
                string 類型 = inv_Sub_CombinelistClasses[k].類型;
                if (類型 == "盤點單")
                {
                    list_inventory_creat_buf = list_inventory_creat.GetRows((int)enum_盤點單號.盤點單號, 單號);
                    if (list_inventory_creat_buf.Count > 0)
                    {
                        inv_Sub_CombinelistClasses[k].名稱 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點名稱].ObjectToString();
                        inv_Sub_CombinelistClasses[k].單號 = list_inventory_creat_buf[0][(int)enum_盤點單號.盤點單號].ObjectToString();
                        inv_Sub_CombinelistClasses[k].類型 = "盤點單";
                        inv_Sub_CombinelistClasses_buf.Add(inv_Sub_CombinelistClasses[k]);
                    }
                }
            }
            inv_CombinelistClass.Records_Ary = inv_Sub_CombinelistClasses_buf;
            inv_CombinelistClasses.Add(inv_CombinelistClass);
            
            returnData.Data = inv_CombinelistClasses;
            returnData.Code = 200;
            returnData.TimeTaken = myTimerBasic.ToString();
            returnData.Method = "get_inv_by_SN";

            returnData.Result = $"成功取得資料";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以合併單號取得合併單覆盤設定資料
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單名稱 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///    
        ///    },
        ///    "Value":"合併單號"
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_setting_by_SN")]
        public string get_setting_by_SN([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            if (returnData.Value.StringIsEmpty())
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value應填入合併單號";
                return returnData.JsonSerializationt(true);
            }
            string 合併單號 = returnData.Value;
            (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

            SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);

            List<object[]> list_inv_combinelist = sQLControl_inv_combinelist.GetRowsByDefult(null, (int)enum_inv_combinelist.合併單號, 合併單號);
            inv_combinelistClass inv_CombinelistClass = list_inv_combinelist[0].SQLToClass<inv_combinelistClass, enum_inv_combinelist>();
           
            returnData.Data = inv_CombinelistClass;
            returnData.Code = 200;
            returnData.TimeTaken = myTimerBasic.ToString();
            returnData.Method = "get_setting_by_SN";

            returnData.Result = $"成功取得資料";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以合併單號取得合併單資料
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單名稱 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///         inv_combinelistClass
        ///    },
        ///    "Value":""
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("update_setting_by_SN")]
        public string update_setting_by_SN([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data應填入合併單號";
                    return returnData.JsonSerializationt(true);
                }
                inv_combinelistClass inv_CombinelistClass = returnData.Data.ObjToClass<inv_combinelistClass>();
                if(inv_CombinelistClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data應填入合併單資料";
                    return returnData.JsonSerializationt(true);
                }
                if (inv_CombinelistClass.誤差總金額致能.ToLower() == "true") inv_CombinelistClass.誤差總金額致能 = true.ToString();
                if (inv_CombinelistClass.誤差百分率致能.ToLower() == "true") inv_CombinelistClass.誤差百分率致能 = true.ToString();
                if (inv_CombinelistClass.誤差數量致能.ToLower() == "true") inv_CombinelistClass.誤差數量致能 = true.ToString();

                if (inv_CombinelistClass.誤差總金額致能.ToLower() == "false") inv_CombinelistClass.誤差總金額致能 = false.ToString();
                if (inv_CombinelistClass.誤差百分率致能.ToLower() == "false") inv_CombinelistClass.誤差百分率致能 = false.ToString();
                if (inv_CombinelistClass.誤差數量致能.ToLower() == "false") inv_CombinelistClass.誤差數量致能 = false.ToString();

                if (inv_CombinelistClass.誤差總金額致能.StringIsEmpty()) inv_CombinelistClass.誤差總金額致能 = false.ToString();
                if (inv_CombinelistClass.誤差百分率致能.StringIsEmpty()) inv_CombinelistClass.誤差百分率致能 = false.ToString();
                if (inv_CombinelistClass.誤差數量致能.StringIsEmpty()) inv_CombinelistClass.誤差數量致能 = false.ToString();

                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");

                SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);

                object[] list_inv_combinelist = inv_CombinelistClass.ClassToSQL<inv_combinelistClass, enum_inv_combinelist>();
                if (list_inv_combinelist != null) sQLControl_inv_combinelist.UpdateByDefulteExtra(null, list_inv_combinelist);

                returnData.Data = inv_CombinelistClass;
                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Method = "update_setting_by_SN";

                returnData.Result = $"成功取得資料";
                return returnData.JsonSerializationt(true);
            }
            catch(Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";       
                return returnData.JsonSerializationt(true);
            }
            
        }
        /// <summary>
        /// 取得所有可合併單
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Data": 
        ///    {                 
        ///        
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [Route("get_all_records")]
        [HttpPost]
        public string POST_get_all_records([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
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

            SQLControl sQLControl_inventory_creat = new SQLControl(Server, DB, "inventory_creat", UserName, Password, Port, SSLMode);


            List<object[]> list_inventory_creat = sQLControl_inventory_creat.GetAllRows(null);

            List<inv_records_Class> inv_Records_Classes = new List<inv_records_Class>();
            for (int i = 0; i < list_inventory_creat.Count; i++)
            {
                inv_records_Class inv_Records_Class = new inv_records_Class();
                inv_Records_Class.GUID = list_inventory_creat[i][(int)enum_盤點單號.GUID].ObjectToString();
                inv_Records_Class.名稱 = list_inventory_creat[i][(int)enum_盤點單號.盤點名稱].ObjectToString();
                inv_Records_Class.單號 = list_inventory_creat[i][(int)enum_盤點單號.盤點單號].ObjectToString();
                inv_Records_Class.類型 = "盤點單";
                inv_Records_Classes.Add(inv_Records_Class);
            }

            returnData.Data = inv_Records_Classes;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "get_all_records";

            returnData.Result = $"取得所有單資料共<{list_inventory_creat.Count}>筆";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以合併單號刪除合併單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240102-0",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [Route("inv_delete_by_SN")]
        [HttpPost]
        public string POST_inv_delete_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
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

            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                return returnData.JsonSerializationt();
            }

            SQLControl sQLControl_inv_combinelist = new SQLControl(Server, DB, "inv_combinelist", UserName, Password, Port, SSLMode);
            SQLControl sQLControl_inv_sub_combinelist = new SQLControl(Server, DB, "inv_sub_combinelist", UserName, Password, Port, SSLMode);

            List<object[]> list_inv_combinelist = sQLControl_inv_combinelist.GetAllRows(null);
            List<object[]> list_inv_combinelist_buf = new List<object[]>();

            List<object[]> list_inv_sub_combinelist = sQLControl_inv_sub_combinelist.GetAllRows(null);
            List<object[]> list_inv_sub_combinelist_buf = new List<object[]>();


            List<inv_combinelistClass> inv_CombinelistClasses = new List<inv_combinelistClass>();
            string 合併單號 = returnData.Value;

            list_inv_combinelist_buf = list_inv_combinelist.GetRows((int)enum_inv_combinelist.合併單號, 合併單號);
            list_inv_sub_combinelist_buf = list_inv_sub_combinelist.GetRows((int)enum_inv_sub_combinelist.合併單號, 合併單號);
            if (list_inv_combinelist_buf.Count > 0) sQLControl_inv_combinelist.DeleteExtra(null, list_inv_combinelist_buf);
            if (list_inv_sub_combinelist_buf.Count > 0) sQLControl_inv_sub_combinelist.DeleteExtra(null, list_inv_sub_combinelist_buf);

            returnData.Data = inv_CombinelistClasses;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "inv_delete_by_SN";
            returnData.Result = $"成功刪除資料";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以合併單號取得完整合併單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost(("get_full_inv_by_SN_old"))]
        public string POST_get_full_inv_by_SN_old([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            GET_init(returnData);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "API01");
            if (sys_serverSettingClasses.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return returnData.JsonSerializationt();
            }
            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                return returnData.JsonSerializationt();
            }
            string api01_url = sys_serverSettingClasses[0].Server;
            string Server = sys_serverSettingClasses[0].Server;
            string DB = sys_serverSettingClasses[0].DBName;
            string UserName = sys_serverSettingClasses[0].User;
            string Password = sys_serverSettingClasses[0].Password;
            uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
            string json_get_all_inv = POST_get_all_inv(returnData);
            returnData returnData_get_all_inv = json_get_all_inv.JsonDeserializet<returnData>();
            if (returnData_get_all_inv == null)
            {
                returnData.Code = -200;
                return returnData.JsonSerializationt();
            }
            List<inv_combinelistClass> inv_CombinelistClasses = returnData_get_all_inv.Data.ObjToClass<List<inv_combinelistClass>>();
            if (returnData_get_all_inv == null)
            {
                returnData.Code = -200;
                returnData.Result = $"資料初始化失敗!";
                return returnData.JsonSerializationt();
            }
            List<inv_combinelistClass> inv_CombinelistClasses_buf = (from temp in inv_CombinelistClasses
                                                                     where temp.合併單號 == returnData.Value
                                                                     select temp).ToList();
            if (inv_CombinelistClasses_buf.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無此合併單號! {returnData.Value} ";
                return returnData.JsonSerializationt();
            }
            inv_combinelistClass inv_CombinelistClass = inv_CombinelistClasses_buf[0];
            inv_CombinelistClass.get_all_full_creat("http://127.0.0.1:4433");

            List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
            List<medClass> medClasses_cloud_buf = new List<medClass>();
            Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClasses_cloud.CoverToDictionaryByCode();

            List<inventoryClass.content> contents = new List<inventoryClass.content>();
            List<inventoryClass.content> contents_buf = new List<inventoryClass.content>();

            List<inv_combinelist_stock_Class> _inv_Combinelist_Stock_Classes = inv_combinelistClass.get_stocks_by_SN("http://127.0.0.1:4433", returnData.Value);
            List<inv_combinelist_stock_Class> inv_Combinelist_Stock_Classes_buf = new List<inv_combinelist_stock_Class>();
            Dictionary<string, List<inv_combinelist_stock_Class>> keyValuePairs_inv_Combinelist_Stock_Classes = _inv_Combinelist_Stock_Classes.CoverToDictionaryByCode();

            List<inv_combinelist_price_Class> _inv_Combinelist_price_Classes = inv_combinelistClass.get_medPrices_by_SN("http://127.0.0.1:4433", returnData.Value);
            List<inv_combinelist_price_Class> inv_Combinelist_price_Classes_buf = new List<inv_combinelist_price_Class>();
            Dictionary<string, List<inv_combinelist_price_Class>> keyValuePairs_inv_Combinelist_price_Classes = _inv_Combinelist_price_Classes.CoverToDictionaryByCode();

            List<inv_combinelist_note_Class> _inv_Combinelist_note_Classes = inv_combinelistClass.get_medNote_by_SN("http://127.0.0.1:4433", returnData.Value);
            List<inv_combinelist_note_Class> inv_Combinelist_note_Classes_buf = new List<inv_combinelist_note_Class>();
            Dictionary<string, List<inv_combinelist_note_Class>> keyValuePairs_inv_Combinelist_note_Classes = _inv_Combinelist_note_Classes.CoverToDictionaryByCode();

            List<inv_combinelist_review_Class> _inv_Combinelist_review_Classes = inv_combinelistClass.get_medReview_by_SN("http://127.0.0.1:4433", returnData.Value);
            List<inv_combinelist_review_Class> inv_Combinelist_review_Classes_buf = new List<inv_combinelist_review_Class>();
            Dictionary<string, List<inv_combinelist_review_Class>> keyValuePairs_inv_Combinelist_review_Classes = _inv_Combinelist_review_Classes.CoverToDictionaryByCode();
            string 藥品碼 = "";
            for (int i = 0; i < inv_CombinelistClass.Records_Ary.Count; i++)
            {
                inventoryClass.creat creat = inv_CombinelistClass.Records_Ary[i].Creat;
                for (int k = 0; k < creat.Contents.Count; k++)
                {

                    藥品碼 = creat.Contents[k].藥品碼;
                    contents_buf = (from temp in contents
                                    where temp.藥品碼 == 藥品碼
                                    select temp).ToList();
                    if (contents_buf.Count == 0)
                    {
                        inventoryClass.content content = new inventoryClass.content();
                        content.藥品碼 = creat.Contents[k].藥品碼;
                        content.料號 = creat.Contents[k].料號;
                        content.廠牌 = creat.Contents[k].廠牌;
                        content.藥品名稱 = creat.Contents[k].藥品名稱;
                        content.中文名稱 = creat.Contents[k].中文名稱;
                        content.盤點量 = creat.Contents[k].盤點量;
                        contents.Add(content);
                    }
                    else
                    {
                        contents_buf[0].盤點量 = (creat.Contents[k].盤點量.StringToInt32() + contents_buf[0].盤點量.StringToInt32()).ToString();
                    }
                }
            }
            for (int i = 0; i < contents.Count; i++)
            {
                inv_Combinelist_Stock_Classes_buf = keyValuePairs_inv_Combinelist_Stock_Classes.SortDictionaryByCode(contents[i].藥品碼);
                inv_Combinelist_price_Classes_buf = keyValuePairs_inv_Combinelist_price_Classes.SortDictionaryByCode(contents[i].藥品碼);
                inv_Combinelist_note_Classes_buf = keyValuePairs_inv_Combinelist_note_Classes.SortDictionaryByCode(contents[i].藥品碼);
                inv_Combinelist_review_Classes_buf = keyValuePairs_inv_Combinelist_review_Classes.SortDictionaryByCode(contents[i].藥品碼);
                medClasses_cloud_buf = keyValuePairs_med_cloud.SortDictionaryByCode(contents[i].藥品碼);
                if (medClasses_cloud_buf.Count > 0)
                {
                    contents[i].藥品名稱 = medClasses_cloud_buf[0].藥品名稱;
                    contents[i].中文名稱 = medClasses_cloud_buf[0].中文名稱;
                    contents[i].廠牌 = medClasses_cloud_buf[0].廠牌;
                    contents[i].料號 = medClasses_cloud_buf[0].料號;
                }


                if (inv_Combinelist_review_Classes_buf.Count > 0)
                {
                    //contents[i].盤點量 = inv_Combinelist_review_Classes_buf[0].數量;
                    inv_CombinelistClass.MedReviews.Add(inv_Combinelist_review_Classes_buf[0]);
                }

                if (inv_Combinelist_Stock_Classes_buf.Count > 0)
                {
                    inv_CombinelistClass.Stocks.Add(inv_Combinelist_Stock_Classes_buf[0]);
                }
                else
                {
                    inv_combinelist_stock_Class inv_Combinelist_Stock_Class = new inv_combinelist_stock_Class();
                    inv_Combinelist_Stock_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_Stock_Class.藥碼 = contents[i].藥品碼;
                    inv_Combinelist_Stock_Class.藥名 = contents[i].藥品名稱;
                    inv_Combinelist_Stock_Class.數量 = "0";
                    inv_CombinelistClass.Stocks.Add(inv_Combinelist_Stock_Class);
                }
                if (inv_Combinelist_price_Classes_buf.Count > 0)
                {
                    inv_CombinelistClass.MedPrices.Add(inv_Combinelist_price_Classes_buf[0]);
                }
                else
                {
                    inv_combinelist_price_Class inv_Combinelist_Price_Class = new inv_combinelist_price_Class();
                    inv_Combinelist_Price_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_Price_Class.藥碼 = contents[i].藥品碼;
                    inv_Combinelist_Price_Class.藥名 = contents[i].藥品名稱;
                    inv_Combinelist_Price_Class.單價 = "0";
                    inv_CombinelistClass.MedPrices.Add(inv_Combinelist_Price_Class);

                }

                if (inv_Combinelist_note_Classes_buf.Count > 0)
                {
                    inv_CombinelistClass.MedNotes.Add(inv_Combinelist_note_Classes_buf[0]);
                }
                else
                {
                    inv_combinelist_note_Class inv_Combinelist_note_Class = new inv_combinelist_note_Class();
                    inv_Combinelist_note_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_note_Class.藥碼 = contents[i].藥品碼;
                    inv_Combinelist_note_Class.藥名 = contents[i].藥品名稱;
                    inv_Combinelist_note_Class.備註 = "-";
                    inv_CombinelistClass.MedNotes.Add(inv_Combinelist_note_Class);

                }
            }

            inv_CombinelistClass.Contents = contents;

            if (inv_CombinelistClass.誤差總金額致能.StringIsEmpty()) inv_CombinelistClass.誤差總金額致能 = true.ToString();
            if (inv_CombinelistClass.誤差百分率致能.StringIsEmpty()) inv_CombinelistClass.誤差百分率致能 = true.ToString();
            if (inv_CombinelistClass.誤差數量致能.StringIsEmpty()) inv_CombinelistClass.誤差數量致能 = true.ToString();


            if (inv_CombinelistClass.誤差總金額上限.StringIsDouble() == false) inv_CombinelistClass.誤差總金額上限 = "500";
            if (inv_CombinelistClass.誤差總金額下限.StringIsDouble() == false) inv_CombinelistClass.誤差總金額下限 = "0";

            if (inv_CombinelistClass.誤差百分率上限.StringIsDouble() == false) inv_CombinelistClass.誤差百分率上限 = "10";
            if (inv_CombinelistClass.誤差百分率下限.StringIsDouble() == false) inv_CombinelistClass.誤差百分率下限 = "0";

            if (inv_CombinelistClass.誤差數量上限.StringIsDouble() == false) inv_CombinelistClass.誤差數量上限 = "100";
            if (inv_CombinelistClass.誤差數量下限.StringIsDouble() == false) inv_CombinelistClass.誤差數量下限 = "0";

            returnData.Data = inv_CombinelistClass;
            returnData.Code = 200;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Method = "get_full_inv_by_SN";
            returnData.Result = $"成功取得盤點單合併完成資料";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以合併單號取得完整合併單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>

        [HttpPost("get_full_inv_by_SN")]
        public string get_full_inv_by_SN([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            
            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                return returnData.JsonSerializationt();
            }
            (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
            string 合併單號 = returnData.Value;

            string json_get_all_inv = get_inv_by_SN(returnData);
            returnData returnData_get_all_inv = json_get_all_inv.JsonDeserializet<returnData>();
            if (returnData_get_all_inv == null)
            {
                returnData.Code = -200;
                return returnData.JsonSerializationt();
            }
            List<inv_combinelistClass> inv_CombinelistClasses = returnData_get_all_inv.Data.ObjToClass<List<inv_combinelistClass>>();
            if (returnData_get_all_inv == null)
            {
                returnData.Code = -200;
                returnData.Result = $"資料初始化失敗!";
                return returnData.JsonSerializationt();
            }
            
            List<inv_combinelistClass> inv_CombinelistClasses_buf = inv_CombinelistClasses.Where(item => item.合併單號 == 合併單號).ToList();
            if (inv_CombinelistClasses_buf.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無此合併單號! {returnData.Value} ";
                return returnData.JsonSerializationt();
            }
            inv_combinelistClass inv_CombinelistClass = inv_CombinelistClasses_buf[0];
            inv_CombinelistClass.get_all_full_creat("http://127.0.0.1:4433");
            
            for (int i = 0; i < inv_CombinelistClass.Records_Ary.Count; i++)
            {
                inventoryClass.creat creat = inv_CombinelistClass.Records_Ary[i].Creat;
                List<inventoryClass.content> Contents = creat.Contents;
                Contents = Contents.Where(item => item.Sub_content.Count > 0).ToList();
                creat.Contents = Contents;
            }

            List<medClass> medClasses_cloud = medClass.get_med_cloud(API01);
            List<medClass> medClasses_cloud_buf = new List<medClass>();
            Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClasses_cloud.CoverToDictionaryByCode();

            List<inventoryClass.content> contents = new List<inventoryClass.content>();
            List<inventoryClass.content> contents_buf = new List<inventoryClass.content>();
            List<Task> tasks = new List<Task>();

            List<inv_combinelist_stock_Class> inv_Combinelist_Stock_Classes_buf = new List<inv_combinelist_stock_Class>();
            List<inv_combinelist_price_Class> inv_Combinelist_price_Classes_buf = new List<inv_combinelist_price_Class>();
            List<inv_combinelist_note_Class> inv_Combinelist_note_Classes_buf = new List<inv_combinelist_note_Class>();
            List<inv_combinelist_review_Class> inv_Combinelist_review_Classes_buf = new List<inv_combinelist_review_Class>();


            Dictionary<string, List<inv_combinelist_stock_Class>> keyValuePairs_inv_Combinelist_Stock_Classes = new Dictionary<string, List<inv_combinelist_stock_Class>>();
            Dictionary<string, List<inv_combinelist_price_Class>> keyValuePairs_inv_Combinelist_price_Classes = new Dictionary<string, List<inv_combinelist_price_Class>>();
            Dictionary<string, List<inv_combinelist_note_Class>> keyValuePairs_inv_Combinelist_note_Classes = new Dictionary<string, List<inv_combinelist_note_Class>>();
            Dictionary<string, List<inv_combinelist_review_Class>> keyValuePairs_inv_Combinelist_review_Classes = new Dictionary<string, List<inv_combinelist_review_Class>>();

            tasks.Add(Task.Run(new Action(delegate 
            {
                List<inv_combinelist_stock_Class> _inv_Combinelist_Stock_Classes = inv_combinelistClass.get_stocks_by_SN("http://127.0.0.1:4433", returnData.Value);
                keyValuePairs_inv_Combinelist_Stock_Classes = _inv_Combinelist_Stock_Classes.CoverToDictionaryByCode();
            })));
            tasks.Add(Task.Run(new Action(delegate 
            {
                List<inv_combinelist_price_Class> _inv_Combinelist_price_Classes = inv_combinelistClass.get_medPrices_by_SN("http://127.0.0.1:4433", returnData.Value);
                keyValuePairs_inv_Combinelist_price_Classes = _inv_Combinelist_price_Classes.CoverToDictionaryByCode();
            })));
            tasks.Add(Task.Run(new Action(delegate 
            {
                List<inv_combinelist_note_Class> _inv_Combinelist_note_Classes = inv_combinelistClass.get_medNote_by_SN("http://127.0.0.1:4433", returnData.Value);
                keyValuePairs_inv_Combinelist_note_Classes = _inv_Combinelist_note_Classes.CoverToDictionaryByCode();
            })));
            tasks.Add(Task.Run(new Action(delegate
            {
                List<inv_combinelist_review_Class> _inv_Combinelist_review_Classes = inv_combinelistClass.get_medReview_by_SN("http://127.0.0.1:4433", returnData.Value);
                keyValuePairs_inv_Combinelist_review_Classes = _inv_Combinelist_review_Classes.CoverToDictionaryByCode();
            })));
            Task.WhenAll(tasks).Wait();
            tasks.Clear();


          //patt1



            string 藥品碼 = "";
            string 料號 = string.Empty;
            for (int i = 0; i < inv_CombinelistClass.Records_Ary.Count; i++)  //合併單有幾張盤點單
            {
                inventoryClass.creat creat = inv_CombinelistClass.Records_Ary[i].Creat;
                for (int k = 0; k < creat.Contents.Count; k++)
                {
                    if (creat.Contents[k].Sub_content.Count == 0) continue;
                    料號 = creat.Contents[k].料號;
                    contents_buf = (from temp in contents
                                    where temp.料號 == 料號
                                    select temp).ToList();
                    if (contents_buf.Count == 0)
                    {
                        inventoryClass.content content = new inventoryClass.content();
                        content.藥品碼 = creat.Contents[k].藥品碼;
                        content.料號 = creat.Contents[k].料號;
                        content.廠牌 = creat.Contents[k].廠牌;
                        content.藥品名稱 = creat.Contents[k].藥品名稱;
                        content.中文名稱 = creat.Contents[k].中文名稱;
                        content.盤點量 = creat.Contents[k].盤點量;
                        contents.Add(content);
                    }
                    else
                    {
                        contents_buf[0].盤點量 = (creat.Contents[k].盤點量.StringToInt32() + contents_buf[0].盤點量.StringToInt32()).ToString();
                    }
                }
            }
            //if (contents != null)
            //{
            //    return contents.JsonSerializationt(true);
            //}
            for (int i = 0; i < contents.Count; i++)
            {
                inv_Combinelist_Stock_Classes_buf = keyValuePairs_inv_Combinelist_Stock_Classes.SortDictionaryByCode(contents[i].藥品碼);
                inv_Combinelist_price_Classes_buf = keyValuePairs_inv_Combinelist_price_Classes.SortDictionaryByCode(contents[i].藥品碼);
                inv_Combinelist_note_Classes_buf = keyValuePairs_inv_Combinelist_note_Classes.SortDictionaryByCode(contents[i].藥品碼);
                inv_Combinelist_review_Classes_buf = keyValuePairs_inv_Combinelist_review_Classes.SortDictionaryByCode(contents[i].藥品碼);
                medClasses_cloud_buf = keyValuePairs_med_cloud.SortDictionaryByCode(contents[i].藥品碼);
                if (medClasses_cloud_buf.Count > 0)
                {
                    medClass medClass = medClasses_cloud_buf.Find(item => item.開檔狀態.Contains("關檔中") == false);
                    if(medClass != null) medClasses_cloud_buf[0] = medClass;
                    contents[i].藥品名稱 = medClasses_cloud_buf[0].藥品名稱;
                    contents[i].中文名稱 = medClasses_cloud_buf[0].中文名稱;
                    contents[i].廠牌 = medClasses_cloud_buf[0].廠牌;
                    contents[i].料號 = medClasses_cloud_buf[0].料號;
                }


                if (inv_Combinelist_review_Classes_buf.Count > 0)
                {
                    inv_CombinelistClass.MedReviews.Add(inv_Combinelist_review_Classes_buf[0]);
                }

                if (inv_Combinelist_Stock_Classes_buf.Count > 0)
                {
                    inv_CombinelistClass.Stocks.Add(inv_Combinelist_Stock_Classes_buf[0]);
                }
                else
                {
                    inv_combinelist_stock_Class inv_Combinelist_Stock_Class = new inv_combinelist_stock_Class();
                    inv_Combinelist_Stock_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_Stock_Class.藥碼 = contents[i].藥品碼;
                    inv_Combinelist_Stock_Class.藥名 = contents[i].藥品名稱;
                    inv_Combinelist_Stock_Class.數量 = "0";
                    inv_CombinelistClass.Stocks.Add(inv_Combinelist_Stock_Class);
                }
                if (inv_Combinelist_price_Classes_buf.Count > 0)
                {
                    inv_CombinelistClass.MedPrices.Add(inv_Combinelist_price_Classes_buf[0]);
                }
                else
                {
                    inv_combinelist_price_Class inv_Combinelist_Price_Class = new inv_combinelist_price_Class();
                    inv_Combinelist_Price_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_Price_Class.藥碼 = contents[i].藥品碼;
                    inv_Combinelist_Price_Class.藥名 = contents[i].藥品名稱;
                    inv_Combinelist_Price_Class.單價 = "0";
                    inv_CombinelistClass.MedPrices.Add(inv_Combinelist_Price_Class);

                }

                if (inv_Combinelist_note_Classes_buf.Count > 0)
                {
                    inv_CombinelistClass.MedNotes.Add(inv_Combinelist_note_Classes_buf[0]);
                }
                else
                {
                    inv_combinelist_note_Class inv_Combinelist_note_Class = new inv_combinelist_note_Class();
                    inv_Combinelist_note_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_note_Class.藥碼 = contents[i].藥品碼;
                    inv_Combinelist_note_Class.藥名 = contents[i].藥品名稱;
                    inv_Combinelist_note_Class.備註 = "-";
                    inv_CombinelistClass.MedNotes.Add(inv_Combinelist_note_Class);

                }
            }
            
            inv_CombinelistClass.Contents = contents;
            //if (inv_CombinelistClass != null)
            //{
            //    return inv_CombinelistClass.JsonSerializationt(true);
            //}
            if (inv_CombinelistClass.誤差總金額致能.StringIsEmpty()) inv_CombinelistClass.誤差總金額致能 = true.ToString();
            if (inv_CombinelistClass.誤差百分率致能.StringIsEmpty()) inv_CombinelistClass.誤差百分率致能 = true.ToString();
            if (inv_CombinelistClass.誤差數量致能.StringIsEmpty()) inv_CombinelistClass.誤差數量致能 = true.ToString();


            if (inv_CombinelistClass.誤差總金額上限.StringIsDouble() == false) inv_CombinelistClass.誤差總金額上限 = "500";
            if (inv_CombinelistClass.誤差總金額下限.StringIsDouble() == false) inv_CombinelistClass.誤差總金額下限 = "0";

            if (inv_CombinelistClass.誤差百分率上限.StringIsDouble() == false) inv_CombinelistClass.誤差百分率上限 = "10";
            if (inv_CombinelistClass.誤差百分率下限.StringIsDouble() == false) inv_CombinelistClass.誤差百分率下限 = "0";

            if (inv_CombinelistClass.誤差數量上限.StringIsDouble() == false) inv_CombinelistClass.誤差數量上限 = "100";
            if (inv_CombinelistClass.誤差數量下限.StringIsDouble() == false) inv_CombinelistClass.誤差數量下限 = "0";

            //if (inv_CombinelistClass != null)
            //{
            //    return inv_CombinelistClass.JsonSerializationt(true);
            //}
            returnData.Data = inv_CombinelistClass;
            returnData.Code = 200;
            returnData.TimeTaken = myTimerBasic.ToString();
            returnData.Method = "get_full_inv_by_SN";
            returnData.Result = $"成功取得盤點單合併完成資料";
            return returnData.JsonSerializationt(true);
        }
        /// <summary>
        /// 以合併單號新增參考庫存
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///       [inv_combinelist_stock_Class]
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [Route("add_stocks_by_SN")]
        [HttpPost]
        public string POST_add_stocks_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "add_stocks_by_SN";

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
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號";
                    return returnData.JsonSerializationt();
                }
                List<inv_combinelist_stock_Class> inv_Combinelist_Stock_Classes = returnData.Data.ObjToClass<List<inv_combinelist_stock_Class>>();
                List<inv_combinelist_stock_Class> inv_Combinelist_Stock_Classes_buf = new List<inv_combinelist_stock_Class>();
                List<inv_combinelist_stock_Class> inv_Combinelist_Stock_Classes_add = new List<inv_combinelist_stock_Class>();
                Table table = new Table(new enum_inv_combinelist_stock());
                table.DBName = sys_serverSettingClasses[0].DBName;
                table.Server = sys_serverSettingClasses[0].Server;
                table.Port = sys_serverSettingClasses[0].Port;
                table.Username = sys_serverSettingClasses[0].User;
                table.Password = sys_serverSettingClasses[0].Password;
                SQLControl sQLControl_inv_Combinelist_Stock = new SQLControl(table);

                if (inv_Combinelist_Stock_Classes == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data 傳入資料無效";
                    return returnData.JsonSerializationt();
                }
                if (inv_Combinelist_Stock_Classes.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data 傳入資料無效";
                    return returnData.JsonSerializationt();
                }
                sQLControl_inv_Combinelist_Stock.DeleteByDefult(null, (int)enum_inv_combinelist_stock.合併單號, returnData.Value);
                List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
                medClass medclass = null;

                for (int i = 0; i < inv_Combinelist_Stock_Classes.Count; i++)
                {
                    string code = inv_Combinelist_Stock_Classes[i].藥碼;
                    medclass = medClasses_cloud.SerchByBarcode(code);
                    if (medclass == null) continue;
                    string 料號 = medclass.料號;
                    string 藥碼 = medclass.藥品碼;
                    string 藥名 = medclass.藥品名稱;
                    inv_Combinelist_Stock_Classes_buf = (from temp in inv_Combinelist_Stock_Classes
                                                         where temp.藥碼 == 藥碼 || temp.藥碼 == 料號
                                                         select temp).ToList();
                    inv_combinelist_stock_Class inv_Combinelist_Stock_Class = new inv_combinelist_stock_Class();
                    inv_Combinelist_Stock_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_Stock_Class.合併單號 = returnData.Value;
                    inv_Combinelist_Stock_Class.藥碼 = code;
                    inv_Combinelist_Stock_Class.藥名 = 藥名;
                    inv_Combinelist_Stock_Class.數量 = "0";
                    inv_Combinelist_Stock_Class.加入時間 = DateTime.Now.ToDateTimeString_6();
                    for (int k = 0; k < inv_Combinelist_Stock_Classes_buf.Count; k++)
                    {
                        if (inv_Combinelist_Stock_Classes_buf[k].數量.StringToInt32() > 0)
                        {
                            inv_Combinelist_Stock_Class.數量 = (inv_Combinelist_Stock_Class.數量.StringToInt32() + inv_Combinelist_Stock_Classes_buf[k].數量.StringToInt32()).ToString();
                        }
                    }
                    inv_Combinelist_Stock_Classes_add.Add(inv_Combinelist_Stock_Class);
                }
                List<object[]> list_vlaue = inv_Combinelist_Stock_Classes_add.ClassToSQL<inv_combinelist_stock_Class, enum_inv_combinelist_stock>();
                sQLControl_inv_Combinelist_Stock.AddRows(null, list_vlaue);
                returnData.Data = inv_Combinelist_Stock_Classes_add;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"新增參考庫存資料,共<{inv_Combinelist_Stock_Classes_add.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }
            finally
            {

            }

        }
        /// <summary>
        /// 以合併單號取得參考庫存
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///      
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [Route("get_stocks_by_SN")]
        [HttpPost]
        public string POST_get_stocks_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "get_stocks_by_SN";

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
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號";
                    return returnData.JsonSerializationt();
                }

                Table table = new Table(new enum_inv_combinelist_stock());
                table.DBName = sys_serverSettingClasses[0].DBName;
                table.Server = sys_serverSettingClasses[0].Server;
                table.Port = sys_serverSettingClasses[0].Port;
                table.Username = sys_serverSettingClasses[0].User;
                table.Password = sys_serverSettingClasses[0].Password;
                SQLControl sQLControl_inv_Combinelist_Stock = new SQLControl(table);
                List<object[]> list_value = sQLControl_inv_Combinelist_Stock.GetRowsByDefult(null, (int)enum_inv_combinelist_stock.合併單號, returnData.Value);
                List<inv_combinelist_stock_Class> inv_Combinelist_Stock_Classes = list_value.SQLToClass<inv_combinelist_stock_Class, enum_inv_combinelist_stock>();
                returnData.Data = inv_Combinelist_Stock_Classes;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得參考庫存資料,共<{inv_Combinelist_Stock_Classes.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }
            finally
            {

            }

        }
        /// <summary>
        /// 以合併單號上傳參考庫存
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "合併單號",
        ///    "Data": 
        ///    {                 
        ///      
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("excel_upload_stocks/{value}")]
        public async Task<string> excel_upload_stocks([FromRoute] string value)
        {
            string 合併單號 = value;
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData returnData = new returnData();
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

                string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                List<object[]> list_value = new List<object[]>();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);

                    list_value = dt.DataTableToRowList();
                    if (list_value.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"文件內容不得為空";
                        return returnData.JsonSerializationt(true);
                    }
                }

                List<medClass> medClasses = medClass.get_med_cloud("http://127.0.0.1:4433");
                List<Task> tasks = new List<Task>();
                List<inv_combinelist_stock_Class> inv_Combinelist_Stock_Classes = new List<inv_combinelist_stock_Class>();
                for (int i = 0; i < list_value.Count; i++)
                {
                    inv_combinelist_stock_Class inv_Combinelist_Stock_Class = new inv_combinelist_stock_Class();
                    string code = list_value[i][(int)enum_合併單庫存EXCLL.藥碼].ObjectToString();
                    medClass medClass = medClasses.SerchByBarcode(code);
                    if (medClass != null)
                    {
                        inv_Combinelist_Stock_Class.藥碼 = medClass.料號;
                        inv_Combinelist_Stock_Class.數量 = list_value[i][(int)enum_合併單庫存EXCLL.庫存].ObjectToString();
                        inv_Combinelist_Stock_Classes.LockAdd(inv_Combinelist_Stock_Class);
                    }
                }
                Task.WhenAll(tasks).Wait();
                returnData.Value = 合併單號;
                returnData.Data = inv_Combinelist_Stock_Classes;
                Logger.Log("add", $"{returnData.JsonSerializationt(true)}");
                return POST_add_stocks_by_SN(returnData);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 匯出庫存
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "",
        ///    "Data": 
        ///    [                 
        ///     {inv_combinelist_stock_Class} 
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("download_excel_Stock")]
        public async Task<ActionResult> download_excel_Stock([FromBody] returnData returnData)
        {
            try
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                List<inv_combinelist_stock_Class> inv_Combinelist_Stock_Classes = returnData.Data.ObjToClass<List<inv_combinelist_stock_Class>>();
                if (inv_Combinelist_Stock_Classes == null)
                {
                    return BadRequest("returnData.Data不能是空的");
                }
                List<object[]> list_Combinelist_Stock = new List<object[]>();
                foreach (var item in inv_Combinelist_Stock_Classes)
                {
                    object[] value = new object[new enum_合併單庫存EXCLL().GetLength()];
                    value[(int)enum_合併單庫存EXCLL.藥碼] = item.藥碼;
                    value[(int)enum_合併單庫存EXCLL.藥名] = item.藥名;
                    value[(int)enum_合併單庫存EXCLL.庫存] = item.數量;
                    list_Combinelist_Stock.Add(value);
                }
                System.Data.DataTable dataTable = list_Combinelist_Stock.ToDataTable(new enum_合併單庫存EXCLL());

                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";
                List<System.Data.DataTable> dataTables = new List<System.Data.DataTable>();
                dataTables.Add(dataTable);
                byte[] excelData = MyOffice.ExcelClass.NPOI_GetBytes(dataTable, Excel_Type.xlsx, (int)enum_合併單庫存EXCLL.庫存);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}.xlsx"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"系統錯誤：{ex.Message}");
            }

        }
        /// <summary>
        /// 以合併單號新增藥品單價
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///       [inv_combinelist_price_Class]
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [Route("add_medPrices_by_SN")]
        [HttpPost]
        public string POST_add_medPrices_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "add_medPrices_by_SN";

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
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號";
                    return returnData.JsonSerializationt();
                }
                List<inv_combinelist_price_Class> inv_Combinelist_Price_Classes = returnData.Data.ObjToClass<List<inv_combinelist_price_Class>>();
                List<inv_combinelist_price_Class> inv_Combinelist_Price_Classes_buf = new List<inv_combinelist_price_Class>();
                List<inv_combinelist_price_Class> inv_Combinelist_Price_Classes_add = new List<inv_combinelist_price_Class>();
                Table table = new Table(new enum_inv_combinelist_price());
                table.DBName = sys_serverSettingClasses[0].DBName;
                table.Server = sys_serverSettingClasses[0].Server;
                table.Port = sys_serverSettingClasses[0].Port;
                table.Username = sys_serverSettingClasses[0].User;
                table.Password = sys_serverSettingClasses[0].Password;
                SQLControl sQLControl_inv_Combinelist_Price = new SQLControl(table);

                if (inv_Combinelist_Price_Classes == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data 傳入資料無效";
                    return returnData.JsonSerializationt();
                }
                if (inv_Combinelist_Price_Classes.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data 傳入資料無效";
                    return returnData.JsonSerializationt();
                }
                sQLControl_inv_Combinelist_Price.DeleteByDefult(null, (int)enum_inv_combinelist_price.合併單號, returnData.Value);
                List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
                medClass medclass = null;

                for (int i = 0; i < inv_Combinelist_Price_Classes.Count; i++)
                {
                    string code = inv_Combinelist_Price_Classes[i].藥碼;
                    medclass = medClasses_cloud.SerchByBarcode(code);
                    if (medclass == null) continue;
                    string 藥碼 = medclass.藥品碼;
                    string 藥名 = medclass.藥品名稱;

                    inv_combinelist_price_Class inv_Combinelist_Price_Class = inv_Combinelist_Price_Classes[i];
                    inv_Combinelist_Price_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_Price_Class.合併單號 = returnData.Value;
                    inv_Combinelist_Price_Class.藥碼 = code;
                    inv_Combinelist_Price_Class.藥名 = 藥名;
                    inv_Combinelist_Price_Class.加入時間 = DateTime.Now.ToDateTimeString_6();

                    inv_Combinelist_Price_Classes_add.Add(inv_Combinelist_Price_Class);
                }
                List<object[]> list_vlaue = inv_Combinelist_Price_Classes_add.ClassToSQL<inv_combinelist_price_Class, enum_inv_combinelist_price>();
                sQLControl_inv_Combinelist_Price.AddRows(null, list_vlaue);
                returnData.Data = inv_Combinelist_Price_Classes_add;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"新增藥品單價,共<{inv_Combinelist_Price_Classes_add.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }
            finally
            {

            }

        }
        /// <summary>
        /// 以合併單號取得藥品單價
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///      
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_medPrices_by_SN")]
        public string POST_get_medPrices_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "get_medPrices_by_SN";

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
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號";
                    return returnData.JsonSerializationt();
                }

                Table table = new Table(new enum_inv_combinelist_price());
                table.DBName = sys_serverSettingClasses[0].DBName;
                table.Server = sys_serverSettingClasses[0].Server;
                table.Port = sys_serverSettingClasses[0].Port;
                table.Username = sys_serverSettingClasses[0].User;
                table.Password = sys_serverSettingClasses[0].Password;
                SQLControl sQLControl_inv_Combinelist_Price = new SQLControl(table);
                List<object[]> list_value = sQLControl_inv_Combinelist_Price.GetRowsByDefult(null, (int)enum_inv_combinelist_price.合併單號, returnData.Value);
                List<inv_combinelist_price_Class> inv_Combinelist_Price_Classes = list_value.SQLToClass<inv_combinelist_price_Class, enum_inv_combinelist_price>();
                returnData.Data = inv_Combinelist_Price_Classes;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得藥品單價,共<{inv_Combinelist_Price_Classes.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }
            finally
            {

            }

        }
        /// <summary>
        /// 以合併單號上傳單價
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "合併單號",
        ///    "Data": 
        ///    {                 
        ///      
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("excel_upload_medPrices/{value}")]
        public async Task<string> excel_upload_medPrices([FromRoute] string value)
        {
            string 合併單號 = value;
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData returnData = new returnData();
            myTimerBasic.StartTickTime(50000);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                returnData.Method = "excel_upload_medPrices";
                var formFile = Request.Form.Files.FirstOrDefault();

                if (formFile == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "文件不得為空";
                    return returnData.JsonSerializationt(true);
                }

                string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                List<object[]> list_value = new List<object[]>();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);

                    list_value = dt.DataTableToRowList();
                    if (list_value.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"文件內容不得為空";
                        return returnData.JsonSerializationt(true);
                    }
                }

                List<medClass> medClasses = medClass.get_med_cloud("http://127.0.0.1:4433");
                List<inv_combinelist_price_Class> inv_combinelist_price_Classes = new List<inv_combinelist_price_Class>();
                for (int i = 0; i < list_value.Count; i++)
                {
                    inv_combinelist_price_Class inv_combinelist_price_Class = new inv_combinelist_price_Class();
                    string code = list_value[i][(int)enum_合併單單價EXCLL.藥碼].ObjectToString();
                    medClass medClass = medClasses.SerchByBarcode(code);
                    if (medClass != null)
                    {
                        inv_combinelist_price_Class.藥碼 = medClass.料號;
                        inv_combinelist_price_Class.單價 = list_value[i][(int)enum_合併單單價EXCLL.單價].ObjectToString();
                        inv_combinelist_price_Classes.LockAdd(inv_combinelist_price_Class);
                    }
                }
                returnData.Value = 合併單號;
                returnData.Data = inv_combinelist_price_Classes;
                Logger.Log("add", $"{returnData.JsonSerializationt(true)}");
                return POST_add_medPrices_by_SN(returnData);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 匯出單價
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "",
        ///    "Data": 
        ///    [                 
        ///     {inv_combinelist_price_Class} 
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("download_excel_medPrices")]
        public async Task<ActionResult> download_excel_medPrices([FromBody] returnData returnData)
        {
            try
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                List<inv_combinelist_price_Class> inv_combinelist_price_Classes = returnData.Data.ObjToClass<List<inv_combinelist_price_Class>>();
                if (inv_combinelist_price_Classes == null)
                {
                    return BadRequest("returnData.Data不能是空的");
                }
                List<object[]> list_Combinelist_price = new List<object[]>();
                foreach (var item in inv_combinelist_price_Classes)
                {
                    object[] value = new object[new enum_合併單庫存EXCLL().GetLength()];
                    value[(int)enum_合併單單價EXCLL.藥碼] = item.藥碼;
                    value[(int)enum_合併單單價EXCLL.藥名] = item.藥名;
                    value[(int)enum_合併單單價EXCLL.單價] = item.單價;
                    list_Combinelist_price.Add(value);
                }
                System.Data.DataTable dataTable = list_Combinelist_price.ToDataTable(new enum_合併單單價EXCLL());

                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";
                List<System.Data.DataTable> dataTables = new List<System.Data.DataTable>();
                dataTables.Add(dataTable);
                byte[] excelData = MyOffice.ExcelClass.NPOI_GetBytes(dataTable, Excel_Type.xlsx, (int)enum_合併單單價EXCLL.單價);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}.xlsx"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"系統錯誤：{ex.Message}");
            }

        }
        /// <summary>
        /// 以合併單號新增藥品備註
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///       [inv_combinelist_price_Class]
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("add_medNote_by_SN")]
        public string POST_add_medNote_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "add_medNote_by_SN";

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
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號";
                    return returnData.JsonSerializationt();
                }
                List<inv_combinelist_note_Class> inv_Combinelist_note_Classes = returnData.Data.ObjToClass<List<inv_combinelist_note_Class>>();
                List<inv_combinelist_note_Class> inv_Combinelist_note_Classes_buf = new List<inv_combinelist_note_Class>();
                List<inv_combinelist_note_Class> inv_Combinelist_note_Classes_add = new List<inv_combinelist_note_Class>();
                Table table = new Table(new enum_inv_combinelist_note());
                table.DBName = sys_serverSettingClasses[0].DBName;
                table.Server = sys_serverSettingClasses[0].Server;
                table.Port = sys_serverSettingClasses[0].Port;
                table.Username = sys_serverSettingClasses[0].User;
                table.Password = sys_serverSettingClasses[0].Password;
                SQLControl sQLControl_inv_Combinelist_note = new SQLControl(table);

                if (inv_Combinelist_note_Classes == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data 傳入資料無效";
                    return returnData.JsonSerializationt();
                }
                if (inv_Combinelist_note_Classes.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data 傳入資料無效";
                    return returnData.JsonSerializationt();
                }
                sQLControl_inv_Combinelist_note.DeleteByDefult(null, (int)enum_inv_combinelist_note.合併單號, returnData.Value);
                List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
                medClass medclass = null;

                for (int i = 0; i < inv_Combinelist_note_Classes.Count; i++)
                {
                    string code = inv_Combinelist_note_Classes[i].藥碼;
                    medclass = medClasses_cloud.SerchByBarcode(code);
                    if (medclass == null) continue;
                    string 藥碼 = medclass.藥品碼;
                    string 藥名 = medclass.藥品名稱;

                    inv_combinelist_note_Class inv_Combinelist_note_Class = inv_Combinelist_note_Classes[i];
                    inv_Combinelist_note_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_note_Class.合併單號 = returnData.Value;
                    inv_Combinelist_note_Class.藥碼 = code;
                    inv_Combinelist_note_Class.藥名 = 藥名;
                    inv_Combinelist_note_Class.加入時間 = DateTime.Now.ToDateTimeString_6();

                    inv_Combinelist_note_Classes_add.Add(inv_Combinelist_note_Class);
                }
                List<object[]> list_vlaue = inv_Combinelist_note_Classes_add.ClassToSQL<inv_combinelist_note_Class, enum_inv_combinelist_note>();
                sQLControl_inv_Combinelist_note.AddRows(null, list_vlaue);
                returnData.Data = inv_Combinelist_note_Classes_add;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"新增藥品備註,共<{inv_Combinelist_note_Classes_add.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }
            finally
            {

            }

        }
        /// <summary>
        /// 以合併單號取得藥品備註
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///      
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_medNote_by_SN")]
        public string POST_get_medNote_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "get_medNote_by_SN";

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
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號";
                    return returnData.JsonSerializationt();
                }

                Table table = new Table(new enum_inv_combinelist_note());
                table.DBName = sys_serverSettingClasses[0].DBName;
                table.Server = sys_serverSettingClasses[0].Server;
                table.Port = sys_serverSettingClasses[0].Port;
                table.Username = sys_serverSettingClasses[0].User;
                table.Password = sys_serverSettingClasses[0].Password;
                SQLControl sQLControl_inv_Combinelist_note = new SQLControl(table);
                List<object[]> list_value = sQLControl_inv_Combinelist_note.GetRowsByDefult(null, (int)enum_inv_combinelist_note.合併單號, returnData.Value);
                List<inv_combinelist_note_Class> inv_Combinelist_note_Classes = list_value.SQLToClass<inv_combinelist_note_Class, enum_inv_combinelist_note>();
                returnData.Data = inv_Combinelist_note_Classes;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得藥品備註,共<{inv_Combinelist_note_Classes.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }
            finally
            {

            }

        }
        /// <summary>
        /// 以合併單號上傳單價
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "合併單號",
        ///    "Data": 
        ///    {                 
        ///      
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("excel_upload_medNote/{value}")]
        public async Task<string> excel_upload_medNote([FromRoute] string value)
        {
            string 合併單號 = value;
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData returnData = new returnData();
            myTimerBasic.StartTickTime(50000);
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                returnData.Method = "excel_upload_medNote";
                var formFile = Request.Form.Files.FirstOrDefault();

                if (formFile == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "文件不得為空";
                    return returnData.JsonSerializationt(true);
                }

                string extension = Path.GetExtension(formFile.FileName); // 获取文件的扩展名
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                List<object[]> list_value = new List<object[]>();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    System.Data.DataTable dt = ExcelClass.NPOI_LoadFile(memoryStream.ToArray(), extension);

                    list_value = dt.DataTableToRowList();
                    if (list_value.Count == 0)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"文件內容不得為空";
                        return returnData.JsonSerializationt(true);
                    }
                }

                List<medClass> medClasses = medClass.get_med_cloud("http://127.0.0.1:4433");
                List<Task> tasks = new List<Task>();
                List<inv_combinelist_note_Class> inv_combinelist_note_Classes = new List<inv_combinelist_note_Class>();
                for (int i = 0; i < list_value.Count; i++)
                {
                    inv_combinelist_note_Class inv_combinelist_note_Class = new inv_combinelist_note_Class();
                    string code = list_value[i][(int)enum_合併單備註EXCLL.藥碼].ObjectToString();
                    medClass medClass = medClasses.SerchByBarcode(code);
                    if (medClass != null)
                    {
                        inv_combinelist_note_Class.藥碼 = medClass.料號;
                        inv_combinelist_note_Class.備註 = list_value[i][(int)enum_合併單備註EXCLL.備註].ObjectToString();
                        inv_combinelist_note_Classes.LockAdd(inv_combinelist_note_Class);
                    }
                }
                Task.WhenAll(tasks).Wait();
                returnData.Value = 合併單號;
                returnData.Data = inv_combinelist_note_Classes;
                Logger.Log("add", $"{returnData.JsonSerializationt(true)}");
                return POST_add_medNote_by_SN(returnData);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 匯出單價
        /// </summary>
        /// <remarks>
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "",
        ///    "Data": 
        ///    [                 
        ///     {inv_combinelist_price_Class} 
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("download_excel_medNote")]
        public async Task<ActionResult> download_excel_medNote([FromBody] returnData returnData)
        {
            try
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                List<inv_combinelist_note_Class> inv_combinelist_note_Classes = returnData.Data.ObjToClass<List<inv_combinelist_note_Class>>();
                if (inv_combinelist_note_Classes == null)
                {
                    return BadRequest("returnData.Data不能是空的");
                }
                List<object[]> list_Combinelist_note = new List<object[]>();
                foreach (var item in inv_combinelist_note_Classes)
                {
                    object[] value = new object[new enum_合併單庫存EXCLL().GetLength()];
                    value[(int)enum_合併單備註EXCLL.藥碼] = item.藥碼;
                    value[(int)enum_合併單備註EXCLL.藥名] = item.藥名;
                    value[(int)enum_合併單備註EXCLL.備註] = item.備註;
                    list_Combinelist_note.Add(value);
                }
                System.Data.DataTable dataTable = list_Combinelist_note.ToDataTable(new enum_合併單備註EXCLL());

                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";
                List<System.Data.DataTable> dataTables = new List<System.Data.DataTable>();
                dataTables.Add(dataTable);
                byte[] excelData = MyOffice.ExcelClass.NPOI_GetBytes(dataTable, Excel_Type.xlsx);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}.xlsx"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"系統錯誤：{ex.Message}");
            }

        }
        /// <summary>
        /// 以合併單號新增藥品覆盤量
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14", //合併單號
        ///    "Data": 
        ///    {                 
        ///       "CODE":"藥碼或料號",
        ///       "NAME":"藥名",
        ///       "QTY":"數量"
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("add_medReview_by_SN")]
        public string POST_add_medReview_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "add_medReview_by_SN";

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
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號";
                    return returnData.JsonSerializationt();
                }
                inv_combinelist_review_Class inv_Combinelist_Review_Class = returnData.Data.ObjToClass<inv_combinelist_review_Class>();
                List<inv_combinelist_review_Class> inv_Combinelist_review_Classes_buf = new List<inv_combinelist_review_Class>();
                List<inv_combinelist_review_Class> inv_Combinelist_review_Classes_add = new List<inv_combinelist_review_Class>();
                Table table = new Table(new enum_inv_combinelist_review());
                table.DBName = sys_serverSettingClasses[0].DBName;
                table.Server = sys_serverSettingClasses[0].Server;
                table.Port = sys_serverSettingClasses[0].Port;
                table.Username = sys_serverSettingClasses[0].User;
                table.Password = sys_serverSettingClasses[0].Password;
                SQLControl sQLControl_inv_Combinelist_review = new SQLControl(table);

                if (inv_Combinelist_Review_Class == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"Data 傳入資料無效";
                    return returnData.JsonSerializationt();
                }

                sQLControl_inv_Combinelist_review.DeleteByDefult(null, new string[] { enum_inv_combinelist_review.合併單號.GetEnumName(), enum_inv_combinelist_review.藥碼.GetEnumName() }
                , new string[] { returnData.Value, inv_Combinelist_Review_Class.藥碼 });

                if (inv_Combinelist_Review_Class.數量.StringIsEmpty())
                {
                    returnData.Data = inv_Combinelist_Review_Class;
                    returnData.Code = 200;
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Result = $"刪除資料成功";
                    return returnData.JsonSerializationt(true);
                }

                List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
                medClass medclass = null;

                string code = inv_Combinelist_Review_Class.藥碼;
                medclass = medClasses_cloud.SerchByBarcode(code);
                if (medclass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"線上藥檔找無資料";
                    return returnData.JsonSerializationt();
                }
                string 藥碼 = medclass.藥品碼;
                string 藥名 = medclass.藥品名稱;

                inv_Combinelist_Review_Class.GUID = Guid.NewGuid().ToString();
                inv_Combinelist_Review_Class.合併單號 = returnData.Value;
                inv_Combinelist_Review_Class.藥碼 = 藥碼;
                inv_Combinelist_Review_Class.藥名 = 藥名;
                inv_Combinelist_Review_Class.加入時間 = DateTime.Now.ToDateTimeString_6();

                inv_Combinelist_review_Classes_add.Add(inv_Combinelist_Review_Class);
                List<object[]> list_vlaue = inv_Combinelist_review_Classes_add.ClassToSQL<inv_combinelist_review_Class, enum_inv_combinelist_review>();
                sQLControl_inv_Combinelist_review.AddRows(null, list_vlaue);
                returnData.Data = inv_Combinelist_Review_Class;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"新增藥品覆盤量,共<{inv_Combinelist_review_Classes_add.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }
            finally
            {

            }

        }
        /// <summary>
        /// 以合併單號取得藥品覆盤量
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///      
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost(("get_medReview_by_SN"))]
        public string POST_get_medReview_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            returnData.Method = "get_medReview_by_SN";

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
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號";
                    return returnData.JsonSerializationt();
                }

                Table table = new Table(new enum_inv_combinelist_review());
                table.DBName = sys_serverSettingClasses[0].DBName;
                table.Server = sys_serverSettingClasses[0].Server;
                table.Port = sys_serverSettingClasses[0].Port;
                table.Username = sys_serverSettingClasses[0].User;
                table.Password = sys_serverSettingClasses[0].Password;
                SQLControl sQLControl_inv_Combinelist_review = new SQLControl(table);
                List<object[]> list_value = sQLControl_inv_Combinelist_review.GetRowsByDefult(null, (int)enum_inv_combinelist_review.合併單號, returnData.Value);
                List<inv_combinelist_review_Class> inv_Combinelist_review_Classes = list_value.SQLToClass<inv_combinelist_review_Class, enum_inv_combinelist_review>();
                returnData.Data = inv_Combinelist_review_Classes;
                returnData.Code = 200;
                returnData.TimeTaken = myTimer.ToString();
                returnData.Result = $"取得藥品覆盤量,共<{inv_Combinelist_review_Classes.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();

            }
            finally
            {

            }

        }
        /// <summary>
        /// 以合併單號取得完整合併單DataTable
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_full_inv_DataTable_by_SN_old")]
        public string POST_get_full_inv_DataTable_by_SN_old([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            GET_init(returnData);
            returnData.Method = "get_full_inv_DataTable_by_SN";
            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "API01");
            if (sys_serverSettingClasses.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return returnData.JsonSerializationt(true);
            }
            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                return returnData.JsonSerializationt(true);
            }
            string api01_url = sys_serverSettingClasses[0].Server;
            string Server = sys_serverSettingClasses[0].Server;
            string DB = sys_serverSettingClasses[0].DBName;
            string UserName = sys_serverSettingClasses[0].User;
            string Password = sys_serverSettingClasses[0].Password;
            uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
            string json_get_all_inv = POST_get_all_inv(returnData);
            returnData returnData_get_all_inv = json_get_all_inv.JsonDeserializet<returnData>();
            if (returnData_get_all_inv == null)
            {
                returnData.Code = -200;
                return returnData.JsonSerializationt(true);
            }
            List<inv_combinelistClass> inv_CombinelistClasses = returnData_get_all_inv.Data.ObjToClass<List<inv_combinelistClass>>();
            if (returnData_get_all_inv == null)
            {
                returnData.Code = -200;
                returnData.Result = $"資料初始化失敗!";
                return returnData.JsonSerializationt(true);
            }
            List<inv_combinelistClass> inv_CombinelistClasses_buf = (from temp in inv_CombinelistClasses
                                                                     where temp.合併單號 == returnData.Value
                                                                     select temp).ToList();
            if (inv_CombinelistClasses_buf.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無此合併單號! {returnData.Value} ";
                return returnData.JsonSerializationt(true);
            }

            inv_combinelistClass inv_CombinelistClass = inv_combinelistClass.get_full_inv_by_SN("http://127.0.0.1:4433", returnData.Value);
            //inv_CombinelistClass.get_all_full_creat("http://127.0.0.1:4433");
            List<inventoryClass.creat> creats = inv_CombinelistClass.Creats;
            List<inventoryClass.content> contents = new List<inventoryClass.content>();
            List<inventoryClass.content> contents_buf = new List<inventoryClass.content>();
            List<System.Data.DataTable> dataTables_creat = new List<System.Data.DataTable>();
            List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
            List<medClass> medClasses_cloud_buf = new List<medClass>();
            Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClasses_cloud.CoverToDictionaryByCode();
            string 藥品碼 = "";
            for (int i = 0; i < creats.Count; i++)
            {
                List<object[]> list_creat_buf = new List<object[]>();
                System.Data.DataTable dataTable_buf = new System.Data.DataTable();
                for (int k = 0; k < creats[i].Contents.Count; k++)
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
            SheetClass sheetClass;
            //Console.WriteLine($"取得creats {myTimer.ToString()}");



            for (int i = 0; i < contents.Count; i++)
            {
                bool flag_覆盤 = false;
                string 藥碼 = contents[i].藥品碼;

                object[] value = new object[new enum_盤點定盤_Excel().GetLength()];
                value[(int)enum_盤點定盤_Excel.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_盤點定盤_Excel.藥碼] = contents[i].藥品碼;
                value[(int)enum_盤點定盤_Excel.料號] = contents[i].料號;
                value[(int)enum_盤點定盤_Excel.藥名] = contents[i].藥品名稱;
                value[(int)enum_盤點定盤_Excel.盤點量] = contents[i].盤點量;
                value[(int)enum_盤點定盤_Excel.單價] = "0";
                value[(int)enum_盤點定盤_Excel.誤差百分率] = "0";
                value[(int)enum_盤點定盤_Excel.消耗量] = "0";
                inv_combinelist_stock_Class inv_Combinelist_Stock_Class = inv_CombinelistClass.GetStockByCode(藥碼);
                inv_combinelist_price_Class inv_Combinelist_Price_Class = inv_CombinelistClass.GetMedPriceByCode(藥碼);
                inv_combinelist_note_Class inv_Combinelist_Note_Class = inv_CombinelistClass.GetMedNoteByCode(藥碼);
                inv_combinelist_review_Class inv_Combinelist_Review_Class = inv_CombinelistClass.GetMedReviewByCode(藥碼);
                if (inv_Combinelist_Stock_Class != null)
                {
                    value[(int)enum_盤點定盤_Excel.庫存量] = inv_Combinelist_Stock_Class.數量;
                }
                if (inv_Combinelist_Note_Class != null)
                {
                    value[(int)enum_盤點定盤_Excel.別名] = inv_Combinelist_Note_Class.備註;
                }

                if (inv_Combinelist_Review_Class != null)
                {
                    value[(int)enum_盤點定盤_Excel.覆盤量] = inv_Combinelist_Review_Class.數量;
                }

                if (inv_Combinelist_Price_Class != null)
                {
                    if (inv_Combinelist_Price_Class.單價.StringIsDouble()) value[(int)enum_盤點定盤_Excel.單價] = inv_Combinelist_Price_Class.單價;
                }
                value[(int)enum_盤點定盤_Excel.庫存金額] = value[(int)enum_盤點定盤_Excel.庫存量].StringToDouble() * value[(int)enum_盤點定盤_Excel.單價].StringToDouble();

                if (value[(int)enum_盤點定盤_Excel.覆盤量].ObjectToString().StringIsEmpty())
                {
                    value[(int)enum_盤點定盤_Excel.結存金額] = value[(int)enum_盤點定盤_Excel.盤點量].StringToDouble() * value[(int)enum_盤點定盤_Excel.單價].StringToDouble();
                    value[(int)enum_盤點定盤_Excel.誤差量] = value[(int)enum_盤點定盤_Excel.盤點量].StringToDouble() - value[(int)enum_盤點定盤_Excel.庫存量].StringToDouble();
                }
                else
                {
                    value[(int)enum_盤點定盤_Excel.結存金額] = value[(int)enum_盤點定盤_Excel.覆盤量].StringToDouble() * value[(int)enum_盤點定盤_Excel.單價].StringToDouble();
                    value[(int)enum_盤點定盤_Excel.誤差量] = value[(int)enum_盤點定盤_Excel.覆盤量].StringToDouble() - value[(int)enum_盤點定盤_Excel.庫存量].StringToDouble();
                }

                value[(int)enum_盤點定盤_Excel.誤差金額] = value[(int)enum_盤點定盤_Excel.誤差量].StringToDouble() * value[(int)enum_盤點定盤_Excel.單價].StringToDouble();

                value[(int)enum_盤點定盤_Excel.庫存金額] = value[(int)enum_盤點定盤_Excel.庫存金額].StringToDouble().ToString("0.00");
                value[(int)enum_盤點定盤_Excel.結存金額] = value[(int)enum_盤點定盤_Excel.結存金額].StringToDouble().ToString("0.00");
                value[(int)enum_盤點定盤_Excel.誤差金額] = value[(int)enum_盤點定盤_Excel.誤差金額].StringToDouble().ToString("0.00");

                if (value[(int)enum_盤點定盤_Excel.消耗量].StringToInt32() > 0)
                {
                    value[(int)enum_盤點定盤_Excel.誤差百分率] = ((value[(int)enum_盤點定盤_Excel.誤差量].StringToDouble() / value[(int)enum_盤點定盤_Excel.消耗量].StringToDouble()) * 100).ToString("0.00");
                }

                if (inv_CombinelistClass.誤差總金額致能.StringToBool())
                {
                    double 上限 = inv_CombinelistClass.誤差總金額上限.StringToDouble();
                    double 下限 = inv_CombinelistClass.誤差總金額下限.StringToDouble();
                    double temp = value[(int)enum_盤點定盤_Excel.誤差金額].StringToDouble();
                    if (temp < 下限 || temp > 上限)
                    {
                        flag_覆盤 = true;
                    }
                }

                if (inv_CombinelistClass.誤差數量致能.StringToBool())
                {
                    double 上限 = inv_CombinelistClass.誤差數量上限.StringToDouble();
                    double 下限 = inv_CombinelistClass.誤差數量下限.StringToDouble();
                    double temp = value[(int)enum_盤點定盤_Excel.誤差量].StringToDouble();
                    if (temp < 下限 || temp > 上限)
                    {
                        flag_覆盤 = true;
                    }
                }
                if (inv_CombinelistClass.誤差百分率致能.StringToBool())
                {
                    double 上限 = inv_CombinelistClass.誤差百分率上限.StringToDouble();
                    double 下限 = inv_CombinelistClass.誤差百分率下限.StringToDouble();
                    double temp = value[(int)enum_盤點定盤_Excel.誤差百分率].StringToDouble();
                    if (temp < 下限 || temp > 上限)
                    {
                        flag_覆盤 = true;
                    }
                }
                if (flag_覆盤) value[(int)enum_盤點定盤_Excel.註記] = "覆盤";
                list_value.Add(value);
            }
            List<System.Data.DataTable> dataTables = new List<System.Data.DataTable>();
            dataTable = list_value.ToDataTable(new enum_盤點定盤_Excel());
            dataTable.TableName = "盤點總表";
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
        /// <summary>
        /// 以合併單號取得完整合併單DataTable
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_full_inv_DataTable_by_SN")]
        public string POST_get_full_inv_DataTable_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            GET_init(returnData);
            returnData.Method = "get_full_inv_DataTable_by_SN";
            
            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                return returnData.JsonSerializationt(true);
            }
           
            string jsonString = get_full_inv_by_SN(returnData); 
            returnData = jsonString.JsonDeserializet<returnData>();
            if(returnData == null || returnData.Code != 200) 
            {
                return returnData.JsonSerializationt(true);
            }
            inv_combinelistClass inv_CombinelistClass = returnData.Data.ObjToClass<inv_combinelistClass>();

            if(inv_CombinelistClass == null)
            {
                returnData.Code = -200;
                returnData.Result = $"資料初始化失敗!";
                return returnData.JsonSerializationt(true);
            }
            
            List<inventoryClass.creat> creats = inv_CombinelistClass.Creats; //盤點單內容
            
            List<inventoryClass.content> contents = new List<inventoryClass.content>();
            List<inventoryClass.content> contents_buf = new List<inventoryClass.content>();
            List<System.Data.DataTable> dataTables_creat = new List<System.Data.DataTable>();
            List<medClass> medClasses_cloud = medClass.get_med_cloud("http://127.0.0.1:4433");
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
                    料號 = creats[i].Contents[k].料號;
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


                    contents_buf = (from temp in contents
                                    where temp.料號 == 料號
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
            SheetClass sheetClass;
            //Console.WriteLine($"取得creats {myTimer.ToString()}");



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
                value[(int)enum_盤點定盤_Excel.單價] = "0";
                value[(int)enum_盤點定盤_Excel.誤差百分率] = "0";
                value[(int)enum_盤點定盤_Excel.消耗量] = "0";
                inv_combinelist_stock_Class inv_Combinelist_Stock_Class = inv_CombinelistClass.GetStockByCode(藥碼);
                inv_combinelist_price_Class inv_Combinelist_Price_Class = inv_CombinelistClass.GetMedPriceByCode(藥碼);
                inv_combinelist_note_Class inv_Combinelist_Note_Class = inv_CombinelistClass.GetMedNoteByCode(藥碼);
                inv_combinelist_review_Class inv_Combinelist_Review_Class = inv_CombinelistClass.GetMedReviewByCode(藥碼);
                if (inv_Combinelist_Stock_Class != null)
                {
                    value[(int)enum_盤點定盤_Excel.庫存量] = inv_Combinelist_Stock_Class.數量;
                }
                if (inv_Combinelist_Note_Class != null)
                {
                    value[(int)enum_盤點定盤_Excel.別名] = inv_Combinelist_Note_Class.備註;
                }

                if (inv_Combinelist_Review_Class != null)
                {
                    value[(int)enum_盤點定盤_Excel.覆盤量] = inv_Combinelist_Review_Class.數量;
                }

                if (inv_Combinelist_Price_Class != null)
                {
                    if (inv_Combinelist_Price_Class.單價.StringIsDouble()) value[(int)enum_盤點定盤_Excel.單價] = inv_Combinelist_Price_Class.單價;
                }
                value[(int)enum_盤點定盤_Excel.庫存金額] = value[(int)enum_盤點定盤_Excel.庫存量].StringToDouble() * value[(int)enum_盤點定盤_Excel.單價].StringToDouble();

                if (value[(int)enum_盤點定盤_Excel.覆盤量].ObjectToString().StringIsEmpty())
                {
                    value[(int)enum_盤點定盤_Excel.結存金額] = value[(int)enum_盤點定盤_Excel.盤點量].StringToDouble() * value[(int)enum_盤點定盤_Excel.單價].StringToDouble();
                    value[(int)enum_盤點定盤_Excel.誤差量] = value[(int)enum_盤點定盤_Excel.盤點量].StringToDouble() - value[(int)enum_盤點定盤_Excel.庫存量].StringToDouble();
                }
                else
                {
                    value[(int)enum_盤點定盤_Excel.結存金額] = value[(int)enum_盤點定盤_Excel.覆盤量].StringToDouble() * value[(int)enum_盤點定盤_Excel.單價].StringToDouble();
                    value[(int)enum_盤點定盤_Excel.誤差量] = value[(int)enum_盤點定盤_Excel.覆盤量].StringToDouble() - value[(int)enum_盤點定盤_Excel.庫存量].StringToDouble();
                }

                value[(int)enum_盤點定盤_Excel.誤差金額] = value[(int)enum_盤點定盤_Excel.誤差量].StringToDouble() * value[(int)enum_盤點定盤_Excel.單價].StringToDouble();

                value[(int)enum_盤點定盤_Excel.庫存金額] = value[(int)enum_盤點定盤_Excel.庫存金額].StringToDouble().ToString("0.00");
                value[(int)enum_盤點定盤_Excel.結存金額] = value[(int)enum_盤點定盤_Excel.結存金額].StringToDouble().ToString("0.00");
                value[(int)enum_盤點定盤_Excel.誤差金額] = value[(int)enum_盤點定盤_Excel.誤差金額].StringToDouble().ToString("0.00");

                if (value[(int)enum_盤點定盤_Excel.消耗量].StringToInt32() > 0)
                {
                    value[(int)enum_盤點定盤_Excel.誤差百分率] = ((value[(int)enum_盤點定盤_Excel.誤差量].StringToDouble() / value[(int)enum_盤點定盤_Excel.消耗量].StringToDouble()) * 100).ToString("0.00");
                }

                if (inv_CombinelistClass.誤差總金額致能.StringToBool())
                {
                    double 上限 = inv_CombinelistClass.誤差總金額上限.StringToDouble();
                    double 下限 = inv_CombinelistClass.誤差總金額下限.StringToDouble();
                    double temp = value[(int)enum_盤點定盤_Excel.誤差金額].StringToDouble();
                    if (temp < 下限 || temp > 上限)
                    {
                        flag_覆盤 = true;
                    }
                }

                if (inv_CombinelistClass.誤差數量致能.StringToBool())
                {
                    double 上限 = inv_CombinelistClass.誤差數量上限.StringToDouble();
                    double 下限 = inv_CombinelistClass.誤差數量下限.StringToDouble();
                    double temp = value[(int)enum_盤點定盤_Excel.誤差量].StringToDouble();
                    if (temp < 下限 || temp > 上限)
                    {
                        flag_覆盤 = true;
                    }
                }
                if (inv_CombinelistClass.誤差百分率致能.StringToBool())
                {
                    double 上限 = inv_CombinelistClass.誤差百分率上限.StringToDouble();
                    double 下限 = inv_CombinelistClass.誤差百分率下限.StringToDouble();
                    double temp = value[(int)enum_盤點定盤_Excel.誤差百分率].StringToDouble();
                    if (temp < 下限 || temp > 上限)
                    {
                        flag_覆盤 = true;
                    }
                }
                if (flag_覆盤) value[(int)enum_盤點定盤_Excel.註記] = "覆盤";
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
        /// <summary>
        /// 以合併單號生成總表
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("add_report_by_SN")]
        public string add_report_by_SN([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try 
            {
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                    return returnData.JsonSerializationt(true);
                }
                string 合併單號 = returnData.Value;
                string jsonString = get_full_inv_by_SN(returnData);
                returnData = jsonString.JsonDeserializet<returnData>();
                if (returnData == null || returnData.Code != 200)
                {
                    return returnData.JsonSerializationt(true);
                }
                inv_combinelistClass inv_CombinelistClass = returnData.Data.ObjToClass<inv_combinelistClass>();

                if (inv_CombinelistClass == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"資料初始化失敗!";
                    return returnData.JsonSerializationt(true);
                }

                List<inventoryClass.content> contents = inv_CombinelistClass.Contents;
                List<inv_combinelist_report_Class> inv_Combinelist_Report_Classes = new List<inv_combinelist_report_Class>();
                if(contents.Count == 0)
                {
                    returnData.Data = inv_Combinelist_Report_Classes;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"無盤點資料";
                    returnData.Method = "add_report_by_SN";
                    returnData.Code = 200;
                    return returnData.JsonSerializationt(true);
                }
                for (int i = 0; i < contents.Count; i++) //總表藥品項目
                {
                    bool flag_覆盤 = false;
                    string 藥碼 = contents[i].藥品碼;
                    string 料號 = contents[i].料號;
                    inv_combinelist_report_Class inv_Combinelist_Report_Class = new inv_combinelist_report_Class();

                    inv_Combinelist_Report_Class.GUID = Guid.NewGuid().ToString();
                    inv_Combinelist_Report_Class.藥碼 = contents[i].藥品碼;
                    inv_Combinelist_Report_Class.料號 = contents[i].料號;
                    inv_Combinelist_Report_Class.藥名 = contents[i].藥品名稱;
                    inv_Combinelist_Report_Class.盤點量 = contents[i].盤點量;
                    inv_Combinelist_Report_Class.單價 = "0";
                    inv_Combinelist_Report_Class.誤差百分率 = "0";
                    inv_Combinelist_Report_Class.消耗量 = "0";
                    inv_Combinelist_Report_Class.合併單號 = 合併單號;


                    inv_combinelist_stock_Class inv_Combinelist_Stock_Class = inv_CombinelistClass.GetStockByCode(藥碼, 料號);
                    inv_combinelist_price_Class inv_Combinelist_Price_Class = inv_CombinelistClass.GetMedPriceByCode(藥碼, 料號);
                    inv_combinelist_note_Class inv_Combinelist_Note_Class = inv_CombinelistClass.GetMedNoteByCode(藥碼, 料號);
                    inv_combinelist_review_Class inv_Combinelist_Review_Class = inv_CombinelistClass.GetMedReviewByCode(藥碼);

                    if (inv_Combinelist_Stock_Class != null) inv_Combinelist_Report_Class.庫存量 = inv_Combinelist_Stock_Class.數量;
                    if (inv_Combinelist_Price_Class != null) inv_Combinelist_Report_Class.單價 = inv_Combinelist_Price_Class.單價;
                    if (inv_Combinelist_Note_Class != null) inv_Combinelist_Report_Class.備註 = inv_Combinelist_Note_Class.備註;
                    if (inv_Combinelist_Review_Class != null) inv_Combinelist_Report_Class.覆盤量 = inv_Combinelist_Review_Class.數量;

                    inv_Combinelist_Report_Class.庫存金額 = (inv_Combinelist_Report_Class.庫存量.StringToDouble() * inv_Combinelist_Report_Class.單價.StringToDouble()).ToString("0.00");
                    if (inv_Combinelist_Report_Class.覆盤量.StringIsEmpty())
                    {
                        inv_Combinelist_Report_Class.結存金額 = (inv_Combinelist_Report_Class.盤點量.StringToDouble() * inv_Combinelist_Report_Class.單價.StringToDouble()).ToString("0.00");
                        inv_Combinelist_Report_Class.誤差量 = (inv_Combinelist_Report_Class.盤點量.StringToDouble() - inv_Combinelist_Report_Class.庫存量.StringToDouble()).ToString();
                    }
                    else
                    {
                        inv_Combinelist_Report_Class.結存金額 = (inv_Combinelist_Report_Class.覆盤量.StringToDouble() * inv_Combinelist_Report_Class.單價.StringToDouble()).ToString("0.00");
                        inv_Combinelist_Report_Class.誤差量 = (inv_Combinelist_Report_Class.覆盤量.StringToDouble() - inv_Combinelist_Report_Class.庫存量.StringToDouble()).ToString();
                    }
                    inv_Combinelist_Report_Class.誤差金額 = (inv_Combinelist_Report_Class.誤差量.StringToDouble() * inv_Combinelist_Report_Class.單價.StringToDouble()).ToString("0.00");
                    if (inv_Combinelist_Report_Class.消耗量.StringToDouble() > 0)
                    {
                        inv_Combinelist_Report_Class.誤差百分率 = (inv_Combinelist_Report_Class.誤差量.StringToDouble() / inv_Combinelist_Report_Class.消耗量.StringToDouble() * 100).ToString("0.00");
                    }


                    if (inv_CombinelistClass.誤差總金額致能.StringToBool())
                    {
                        double 上限 = inv_CombinelistClass.誤差總金額上限.StringToDouble();
                        double 下限 = inv_CombinelistClass.誤差總金額下限.StringToDouble();
                        double temp = inv_Combinelist_Report_Class.誤差金額.StringToDouble();
                        if (temp < 下限 || temp > 上限) flag_覆盤 = true;
                    }

                    if (inv_CombinelistClass.誤差數量致能.StringToBool())
                    {
                        double 上限 = inv_CombinelistClass.誤差數量上限.StringToDouble();
                        double 下限 = inv_CombinelistClass.誤差數量下限.StringToDouble();
                        double temp = inv_Combinelist_Report_Class.誤差量.StringToDouble();
                        if (temp < 下限 || temp > 上限) flag_覆盤 = true;
                    }
                    if (inv_CombinelistClass.誤差百分率致能.StringToBool())
                    {
                        double 上限 = inv_CombinelistClass.誤差百分率上限.StringToDouble();
                        double 下限 = inv_CombinelistClass.誤差百分率下限.StringToDouble();
                        double temp = inv_Combinelist_Report_Class.誤差百分率.StringToDouble();
                        if (temp < 下限 || temp > 上限) flag_覆盤 = true;
                    }
                    if (flag_覆盤) inv_Combinelist_Report_Class.註記 = "覆盤";
                    inv_Combinelist_Report_Classes.Add(inv_Combinelist_Report_Class);
                }
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "inv_combinelist_report", UserName, Password, Port, SSLMode);
                sQLControl.DeleteByDefult(null, (int)enum_inv_combinelist_report.合併單號, returnData.Value);
                List<object[]> objects = inv_Combinelist_Report_Classes.ClassToSQL<inv_combinelist_report_Class, enum_inv_combinelist_report>();
                sQLControl.AddRows(null, objects);

                returnData.Data = inv_Combinelist_Report_Classes;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"成功取得總表，共{inv_Combinelist_Report_Classes.Count}筆";
                returnData.Method = "add_report_by_SN";
                returnData.Code = 200;
                return returnData.JsonSerializationt(true);
            }
            catch(Exception ex)
            {
                if (ex.Message == "Index was outside the bounds of the array.") GET_init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
            
        }
        /// <summary>
        /// 以合併單號取得總表
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_report_by_SN")]
        public string get_report_by_SN([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Value.StringIsEmpty() == true)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                    return returnData.JsonSerializationt(true);
                }
                string 合併單號 = returnData.Value;
                
                
                (string Server, string DB, string UserName, string Password, uint Port) = HIS_WebApi.Method.GetServerInfo("Main", "網頁", "VM端");
                SQLControl sQLControl = new SQLControl(Server, DB, "inv_combinelist_report", UserName, Password, Port, SSLMode);
                List<object[]> objects = sQLControl.GetRowsByDefult(null, (int)enum_inv_combinelist_report.合併單號, returnData.Value);
                List<inv_combinelist_report_Class> inv_Combinelist_Report_Classes = objects.SQLToClass<inv_combinelist_report_Class, enum_inv_combinelist_report>();
                if (inv_Combinelist_Report_Classes.Count == 0)
                {
                    returnData.Data = inv_Combinelist_Report_Classes;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"無總表資料";
                    returnData.Method = "get_report_by_SN";
                    returnData.Code = 200;
                    return returnData.JsonSerializationt(true);
                }
                returnData.Data = inv_Combinelist_Report_Classes;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"成功取得總表，共{inv_Combinelist_Report_Classes.Count}筆";
                returnData.Method = "get_report_by_SN";
                returnData.Code = 200;
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Index was outside the bounds of the array.") GET_init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 更新總表註記覆盤資料
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : ""
        ///    "Data": 
        ///    [
        ///     {  
        ///         "GUID":"",
        ///         "COMMENT":"",   
        ///     }
        ///    ] 
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("update_report")]
        public async Task<string> update_report([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Data 空白,請輸入inv_combinelist_report_Class!";
                    return returnData.JsonSerializationt(true);
                }
                List<inv_combinelist_report_Class> inv_Combinelist_Report_Classes = returnData.Data.ObjToClass<List<inv_combinelist_report_Class>>();

                string[] guids = inv_Combinelist_Report_Classes.Select(temp => temp.GUID).ToArray();
                string 合併單號 = returnData.Value;

                (string Server, string DB, string UserName, string Password, uint Port) = await HIS_WebApi.Method.GetServerInfoAsync("Main", "網頁", "VM端");

                SQLControl sQLControl = new SQLControl(Server, DB, "inv_combinelist_report", UserName, Password, Port, SSLMode);

                List<object[]> objects = await sQLControl.GetRowsByDefultAsync(null, (int)enum_inv_combinelist_report.GUID, guids);
                List<inv_combinelist_report_Class> inv_Combinelist_Report = objects.SQLToClass<inv_combinelist_report_Class, enum_inv_combinelist_report>();

                if (inv_Combinelist_Report_Classes.Count == 0)
                {
                    returnData.Data = inv_Combinelist_Report_Classes;
                    returnData.TimeTaken = myTimerBasic.ToString();
                    returnData.Result = $"無總表資料";
                    returnData.Method = "update_report";
                    returnData.Code = 200;
                    return returnData.JsonSerializationt(true);
                }
                foreach(var item in inv_Combinelist_Report)
                {
                    inv_combinelist_report_Class inv_Combinelist_Report_Class = inv_Combinelist_Report_Classes.FirstOrDefault(temp => temp.GUID == item.GUID);
                    if (inv_Combinelist_Report_Class == null) continue;
                    item.註記 = inv_Combinelist_Report_Class.註記;
                }

                List<object[]> objects_update = inv_Combinelist_Report.ClassToSQL<inv_combinelist_report_Class, enum_inv_combinelist_report>();
                await sQLControl.UpdateRowsAsync(null, objects_update);

                returnData.Data = inv_Combinelist_Report;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"更新總表，共{inv_Combinelist_Report_Classes.Count}筆";
                returnData.Method = "update_report";
                returnData.Code = 200;
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Index was outside the bounds of the array.") GET_init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        /// <summary>
        /// 以合併單號取得完整合併單
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_detail_inv_by_SN")]
        public string get_detail_inv_by_SN([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            try
            {
                if (returnData.Value.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                    return returnData.JsonSerializationt();
                }
                string json_out = POST_get_full_inv_DataTable_by_SN(returnData);
                returnData = json_out.JsonDeserializet<returnData>();
                string dataTable_string = returnData.Data.ObjToClass<string>();
                List<System.Data.DataTable> dataTables = dataTable_string.JsonDeserializeToDataTables(); 
                List<object[]> list_value = dataTables[0].DataTableToRowList();
                List<inv_combinelist_report_Class> inv_CombinelistClasses = list_value.SQLToClass<inv_combinelist_report_Class, enum_inv_combinelist_report>();

                returnData.Data = inv_CombinelistClasses;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Result = $"成功取得合併單-{returnData.Value} 資料,共<{inv_CombinelistClasses.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 以合併單號取得完整合併單Excel
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_full_inv_Excel_by_SN")]
        public async Task<ActionResult> POST_get_full_inv_Excel_by_SN([FromBody] returnData returnData)
        {
            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "API01");

            string json_out = POST_get_full_inv_DataTable_by_SN(returnData);
            returnData = json_out.JsonDeserializet<returnData>();
            string dataTable_string = returnData.Data.ObjToClass<string>();
            List<System.Data.DataTable> dataTables = dataTable_string.JsonDeserializeToDataTables();
            
            if (dataTables == null)
            {
                return null;
            }
            for(int i = 0; i < dataTables.Count; i++)
            {
                if(dataTables[i].Columns.Count > 0)
                {
                    dataTables[i].Columns.RemoveAt(0);
                }
            }

            string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string xls_command = "application/vnd.ms-excel";

            Enum[] enums = new Enum[] {  enum_盤點定盤_Excel.庫存量, enum_盤點定盤_Excel .盤點量 ,enum_盤點定盤_Excel .單價 ,enum_盤點定盤_Excel .庫存金額 ,enum_盤點定盤_Excel .消耗量 ,
                enum_盤點定盤_Excel.結存金額, enum_盤點定盤_Excel .誤差量 ,enum_盤點定盤_Excel.誤差金額,enum_盤點定盤_Excel.覆盤量 };
            byte[] excelData = ExcelClass.NPOI_GetBytes(dataTables, Excel_Type.xlsx, enums);

            Stream stream = new MemoryStream(excelData);
            return await Task.FromResult(File(stream, xlsx_command, $"{returnData.Value}_InventorySummary.xlsx"));
        }
        /// <summary>
        /// 以單號取得(盤點單/消耗單)Excel
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 單號,(盤點單/消耗單) <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "20240103-1,盤點單",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_record_Excel_by_SN")]
        public async Task<ActionResult> POST_get_record_Excel_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            GET_init(returnData);

            List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "API01");
            if (sys_serverSettingClasses.Count == 0)
            {
                returnData.Code = -200;
                returnData.Result = $"找無Server資料!";
                return null;
            }
            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入單號!";
                return null;
            }
            string[] str_ary = returnData.Value.Split(",");
            if (str_ary.Length != 2)
            {
                return null;
            }
            string 單號 = str_ary[0];
            string 類別 = str_ary[1];
            string api01_url = sys_serverSettingClasses[0].Server;
            string Server = sys_serverSettingClasses[0].Server;
            string DB = sys_serverSettingClasses[0].DBName;
            string UserName = sys_serverSettingClasses[0].User;
            string Password = sys_serverSettingClasses[0].Password;
            uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();

            string url = $"{api01_url}/api/inventory/creat_get_by_IC_SN";
            returnData returnData_post_in = new returnData();
            returnData_post_in.Value = 單號;
            string json_post_out = Basic.Net.WEBApiPostJson(url, returnData_post_in.JsonSerializationt());
            returnData returnData_post_out = json_post_out.JsonDeserializet<returnData>();
            List<inventoryClass.creat> creats_buf = returnData_post_out.Data.ObjToClass<List<inventoryClass.creat>>();
            if (creats_buf.Count == 0)
            {
                return null;
            }


            List<inventoryClass.content> contents = creats_buf[0].Contents;
            List<object[]> list_value = new List<object[]>();
            System.Data.DataTable dataTable;
            SheetClass sheetClass;
            //Console.WriteLine($"取得creats {myTimer.ToString()}");

            for (int i = 0; i < contents.Count; i++)
            {

                object[] value = new object[new enum_盤點定盤_Excel().GetLength()];
                value[(int)enum_盤點定盤_Excel.藥碼] = contents[i].藥品碼;
                value[(int)enum_盤點定盤_Excel.料號] = contents[i].料號;
                value[(int)enum_盤點定盤_Excel.藥名] = contents[i].藥品名稱;
                value[(int)enum_盤點定盤_Excel.庫存量] = contents[i].理論值;
                value[(int)enum_盤點定盤_Excel.盤點量] = contents[i].盤點量;
                list_value.Add(value);

            }
            dataTable = list_value.ToDataTable(new enum_盤點定盤_Excel());
            sheetClass = dataTable.NPOI_GetSheetClass(new int[] {(int)enum_盤點定盤_Excel.庫存量, (int)enum_盤點定盤_Excel.盤點量, (int)enum_盤點定盤_Excel.單價
            , (int)enum_盤點定盤_Excel.庫存金額, (int)enum_盤點定盤_Excel.消耗量
            ,(int)enum_盤點定盤_Excel.結存金額 , (int)enum_盤點定盤_Excel.誤差量 , (int)enum_盤點定盤_Excel.誤差金額 });
            sheetClass.Name = "盤點總表";


            //Console.WriteLine($"NewCell_Webapi_Buffer_Caculate {myTimer.ToString()}");

            string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string xls_command = "application/vnd.ms-excel";

            byte[] excelData = sheetClass.NPOI_GetBytes(Excel_Type.xls);
            Stream stream = new MemoryStream(excelData);
            return await Task.FromResult(File(stream, xlsx_command, $"{單號}_盤點單.xlsx"));
        }

        /// <summary>
        /// 以合併單號取得所有盤點單數量統計
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_inv_EndQty_by_SN")]
        public string get_inv_EndQty_by_SN([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                return returnData.JsonSerializationt(true);
            }

            string jsonString = get_full_inv_by_SN(returnData);
            returnData = jsonString.JsonDeserializet<returnData>();
            if (returnData == null || returnData.Code != 200)
            {
                return returnData.JsonSerializationt(true);
            }
            inv_combinelistClass inv_CombinelistClass = returnData.Data.ObjToClass<inv_combinelistClass>();

            if (inv_CombinelistClass == null)
            {
                returnData.Code = -200;
                returnData.Result = $"資料初始化失敗!";
                return returnData.JsonSerializationt(true);
            }
            List<ScheduledCountClass> scheduledCountClasses = new List<ScheduledCountClass>();
            List<ScheduledCountClass> scheduledCountClasses_buff = new List<ScheduledCountClass>();


            for (int i = 0; i < inv_CombinelistClass.Records_Ary.Count; i++)
            {
                for(int k = 0; k < inv_CombinelistClass.Records_Ary[i].Creat.Contents.Count; k++)
                {
                    scheduledCountClasses_buff = scheduledCountClasses.Where(item => item.料號 == inv_CombinelistClass.Records_Ary[i].Creat.Contents[k].料號).ToList();
                    if(scheduledCountClasses_buff.Count == 0)
                    {
                        ScheduledCountClass scheduledCount = new ScheduledCountClass();
                        scheduledCount.藥碼 = inv_CombinelistClass.Records_Ary[i].Creat.Contents[k].藥品碼;
                        scheduledCount.料號 = inv_CombinelistClass.Records_Ary[i].Creat.Contents[k].料號;
                        scheduledCount.藥名 = inv_CombinelistClass.Records_Ary[i].Creat.Contents[k].藥品名稱;
                        scheduledCount.結存量 = inv_CombinelistClass.Records_Ary[i].Creat.Contents[k].盤點量;

                        scheduledCountClasses.Add(scheduledCount);
                    }
                    else
                    {
                        scheduledCountClasses_buff[0].結存量 = (scheduledCountClasses_buff[0].結存量.StringToDouble() + inv_CombinelistClass.Records_Ary[i].Creat.Contents[k].盤點量.StringToDouble()).ToString();
                    }
                   
                }
            }
                       
            returnData.Method = "get_inv_EndQty_by_SN";
            returnData.Data = scheduledCountClasses;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"取得合併單號{returnData.Value}內所有盤點單盤點項目，共{scheduledCountClasses.Count}個";
            return returnData.JsonSerializationt(true);
        }

        /// <summary>
        /// 以合併單號取得所有盤點單數量統計
        /// </summary>
        /// <remarks>
        /// [必要輸入參數說明]<br/> 
        ///  1.[returnData.Value] : 合併單號 <br/> 
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///  {
        ///    "Value" : "I20240103-14",
        ///    "ServerName": "A7"
        ///    "ServerType":"調劑台 or 藥庫 or 藥局"
        ///    "Data": 
        ///    {                 
        ///    
        ///    }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為合併單結構</returns>
        [HttpPost("get_scheduledCount_by_SN_ServerName")]
        public string get_scheduledCount_by_SN_ServerName([FromBody] returnData returnData)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            if (returnData.Value.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.Value 空白,請輸入合併單號!";
                return returnData.JsonSerializationt(true);
            }
            if (returnData.ServerName.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.ServerName 空白,請輸入ServerName!";
                return returnData.JsonSerializationt(true);
            }
            if (returnData.ServerType.StringIsEmpty() == true)
            {
                returnData.Code = -200;
                returnData.Result = "returnData.ServerType 空白,請輸入ServerType!";
                return returnData.JsonSerializationt(true);
            }
            string serverName = returnData.ServerName;
            string serverType = returnData.ServerType;
            string jsonString = get_inv_EndQty_by_SN(returnData);
            returnData = jsonString.JsonDeserializet<returnData>();

            List<ScheduledCountClass> scheduledCountClasses = returnData.Data.ObjToClass<List<ScheduledCountClass>>();

            List<string> codes = scheduledCountClasses.Select(x => x.藥碼).Distinct().ToList();
            List<medClass> medClasses = medClass.get_dps_medClass_by_code(API01, serverName, codes);
            List<medClass> medClasses_buff = new List<medClass>();
            foreach(var item in scheduledCountClasses)
            {
                double 庫存量 = 0;
                List<string> lot_number = new List<string>();
                List<string> validity_period = new List<string>();

                medClasses_buff = medClasses.Where(x => x.藥品碼 == item.藥碼).ToList();
                for(int i = 0; i < medClasses_buff.Count; i++)
                {
                    for (int k = 0; k < medClasses_buff[i].DeviceBasics.Count; k++)
                    {
                        List<string> qty = medClasses_buff[i].DeviceBasics[k].List_Inventory;
                        double total = qty.Sum(item => double.Parse(item));
                        庫存量 += total;
                        lot_number.AddRange(medClasses_buff[i].DeviceBasics[k].List_Lot_number);
                        validity_period.AddRange(medClasses_buff[i].DeviceBasics[k].List_Validity_period);
                    }
                }
                item.庫存量 = 庫存量.ToString();
                item.批號 = lot_number;
                item.效期 = validity_period;
                item.盤差量 = (item.結存量.StringToDouble() - item.庫存量.StringToDouble()).ToString();
            }

            returnData.Method = "get_scheduledCount_by_SN_ServerName";
            returnData.Data = scheduledCountClasses;
            returnData.TimeTaken = myTimer.ToString();
            returnData.Result = $"取得合併單號{returnData.Value}內所有盤點單盤點項目，共{scheduledCountClasses.Count}個";
            return returnData.JsonSerializationt(true);
        }


        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {

            List<Table> tables = new List<Table>();
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_inv_combinelist()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_inv_sub_combinelist()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_inv_combinelist_stock()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_inv_combinelist_consumption()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_inv_combinelist_price()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_inv_combinelist_note()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_inv_combinelist_review()));
            tables.Add(MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_inv_combinelist_report()));

            return tables.JsonSerializationt(true);
        }
        private enum enum_合併單庫存EXCLL
        {
            藥碼,
            藥名,
            庫存
        }
        private enum enum_合併單單價EXCLL
        {
            藥碼,
            藥名,
            單價
        }
        private enum enum_合併單備註EXCLL
        {
            藥碼,
            藥名,
            備註
        }
        
        

    }
}
