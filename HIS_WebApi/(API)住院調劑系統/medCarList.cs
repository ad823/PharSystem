using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basic;
using MySql.Data.MySqlClient;
using SQLUI;
using HIS_DB_Lib;
using System.Runtime.InteropServices;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class medCarList : ControllerBase
    {
        static private string API_Server = "http://127.0.0.1:4433";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        /// <summary>
        ///初始化dbvm.med_carList資料庫
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":[""]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("init")]
        [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(200, "medCarListClass物件", typeof(medCarListClass))]
        public string init([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "medCarList/init_med_carList";
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
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {
            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            Table table = MethodClass.CheckCreatTable(sys_serverSettingClass, new enum_med_carList());
            return table.JsonSerializationt(true);
        }
        /// <summary>
        ///新增護理站資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":[藥局代號,藥局, 護理站]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("add")]
        public string add([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "medCarList/add";
            try
            {
                if (returnData.Data == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.Data 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                //if (returnData.ValueAry.Count != 3)
                //{
                //    returnData.Code = -200;
                //    returnData.Result = $"returnData.ValueAry 內容應為[藥局代號,藥局, 護理站]";
                //    return returnData.JsonSerializationt(true);
                //}
                //string 藥局代號 = returnData.ValueAry[0];
                //string 藥局 = returnData.ValueAry[1];
                //string 護理站 = returnData.ValueAry[2];
                List<medCarListClass> medCarListClasses = returnData.Data.ObjToClass<List<medCarListClass>>();
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                //List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                Table table = new Table(new enum_med_carList());
                SQLControl sQLControl_med_carInfo = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_medCart = sQLControl_med_carInfo.GetAllRows(null);

                List<medCarListClass> medCart_sql = list_medCart.SQLToClass<medCarListClass, enum_med_carList>();
                List<medCarListClass> medCart_sql_buf = new List<medCarListClass>();
                List<medCarListClass> medCart_sql_add = new List<medCarListClass>();
                List<medCarListClass> medCart_sql_replace = new List<medCarListClass>();
                foreach (var item in medCarListClasses)
                {
                    string 藥局 = item.藥局;
                    string 藥局名 = item.藥局名;
                    string 護理站 = item.護理站;
                    medCart_sql_buf = (from temp in medCart_sql
                                       where temp.藥局 == 藥局
                                       where temp.護理站 == 護理站
                                       select temp).ToList();
                    if (medCart_sql_buf.Count != 0) continue;


                    string GUID = Guid.NewGuid().ToString();
                    medCarListClass medCarListClass = new medCarListClass
                    {
                        GUID = GUID,
                        藥局 = 藥局,
                        藥局名 = 藥局名,
                        護理站 = 護理站,
                        交車時間 = DateTime.MinValue.ToDateTimeString()
                    };
                    medCart_sql_add.Add(medCarListClass);
                }
                List<object[]> list_medCart_add = new List<object[]>();
                list_medCart_add = medCart_sql_add.ClassToSQL<medCarListClass, enum_med_carList>();
                if (list_medCart_add.Count > 0) sQLControl_med_carInfo.AddRows(null, list_medCart_add);
                List<object[]> list_carList = sQLControl_med_carInfo.GetAllRows(null);
                List<medCarListClass> carList = list_carList.SQLToClass<medCarListClass, enum_med_carList>();
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = carList;
                returnData.Result = $"護理站清單共{carList.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        ///刪除護理站資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":[藥局, 護理站]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("delete")]
        public string delete([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "medCarList/delete";
            try
            {
                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥局, 護理站]";
                    return returnData.JsonSerializationt(true);
                }
                string 藥局 = returnData.ValueAry[0];
                string 護理站 = returnData.ValueAry[1];

                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                Table table = new Table(new enum_med_carList());
                SQLControl sQLControl_med_carInfo = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_medCart = sQLControl_med_carInfo.GetAllRows(null);

                List<medCarListClass> medCart_sql = list_medCart.SQLToClass<medCarListClass, enum_med_carList>();
                List<medCarListClass> medCart_sql_buf = new List<medCarListClass>();
                List<medCarListClass> medCart_sql_delete = new List<medCarListClass>();

                medCart_sql_buf = (from temp in medCart_sql
                                   where temp.藥局 == 藥局
                                   where temp.護理站 == 護理站
                                   select temp).ToList();
                if (medCart_sql_buf.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = "資料不存在";
                    return returnData.JsonSerializationt(true);
                }
                else
                {
                    medCart_sql_delete.Add(medCart_sql_buf[0]);
                }
                List<object[]> list_medCart_delete = new List<object[]>();
                list_medCart_delete = medCart_sql_delete.ClassToSQL<medCarListClass, enum_med_carList>();
                if (medCart_sql_delete.Count > 0) sQLControl_med_carInfo.DeleteExtra(null, list_medCart_delete);
                List<object[]> list_carList = sQLControl_med_carInfo.GetAllRows(null);
                List<medCarListClass> carList = list_carList.SQLToClass<medCarListClass, enum_med_carList>();
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medCart_sql_delete;
                returnData.Result = $"刪除護理站資料共{list_medCart_delete.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        ///取得藥局護理站資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":[藥局]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_medcar_by_phar")]
        public string get_medcar_by_phar([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "medCarList/get_medcar_by_phar";
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
                    returnData.Result = $"returnData.ValueAry 內容應為[藥局]";
                    return returnData.JsonSerializationt(true);
                }
                string 藥局 = returnData.ValueAry[0];
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();

                //List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                SQLControl sQLControl_med_carList = new SQLControl(Server, DB, "med_carList", UserName, Password, Port, SSLMode);
                List<object[]> list_medCart = sQLControl_med_carList.GetRowsByDefult(null, (int)enum_med_carList.藥局, 藥局);
                List<medCarListClass> medCart_sql = list_medCart.SQLToClass<medCarListClass, enum_med_carList>();
                List<medCarListClass> medCarListClasses = new List<medCarListClass>();
                DateTime today = DateTime.Now.Date;
                foreach (var medCarListClass in medCart_sql)
                {
                    DateTime handTime = medCarListClass.交車時間.StringToDateTime();

                    if (handTime.Date != today)
                    {
                        medCarListClass.交車狀態 = "";
                        medCarListClasses.Add(medCarListClass);
                    }
                }
                List<object[]> medCarList_sql_update = new List<object[]>();
                medCarList_sql_update = medCarListClasses.ClassToSQL<medCarListClass, enum_med_carList>();
                sQLControl_med_carList.UpdateByDefulteExtra(null, medCarList_sql_update);

                List<object[]> list_medCartList = sQLControl_med_carList.GetRowsByDefult(null, (int)enum_med_carList.藥局, 藥局);
                List<medCarListClass> medCartList_sql = list_medCartList.SQLToClass<medCarListClass, enum_med_carList>();

                //medCartList_sql.Sort(new medCarListClass.ICP_By_phar_name());
                if (medCartList_sql[0].排序.StringIsEmpty())
                {
                    medCartList_sql = medCartList_sql.OrderBy(x => x.護理站).ToList();

                }
                else
                {
                    medCartList_sql = medCartList_sql.OrderBy(x => x.排序).ToList();
                }

                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medCartList_sql;
                returnData.Result = $"取得{藥局}的護理站共{medCart_sql.Count}筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        ///取得藥局護理站資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":[]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_phar")]
        public string get_phar([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "medCarList/get_phar";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();


                //List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                Table table = new Table(new enum_med_carList());
                SQLControl sQLControl_med_carInfo = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_medCart = sQLControl_med_carInfo.GetAllRows(null);

                List<medCarListClass> medCart_sql = list_medCart.SQLToClass<medCarListClass, enum_med_carList>();

                List<medCarListClass> medCart_sql_buf = medCart_sql
                    .GroupBy(medCart => medCart.藥局)
                    .Select(group => group.First())
                    .ToList();
                medCart_sql_buf = medCart_sql_buf.OrderBy(x => x.藥局, new NaturalComparer()).ToList();

                //medCart_sql_buf.Sort(new medCarListClass.ICP_By_phar_name());
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medCart_sql_buf;
                returnData.Result = $"取得藥局資料共{medCart_sql_buf.Count}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Index was outside the bounds of the array.") init(returnData);
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        /// <summary>
        ///取得藥局護理站資料
        /// </summary>
        /// <remarks>
        /// 以下為JSON範例
        /// <code>
        ///     {
        ///         "ValueAry":[]
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns></returns>
        [HttpPost("get_all")]
        public string get_all([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "medCarList/get_all";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();


                //List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}");
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind("Main", "網頁", "VM端");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"找無Server資料";
                    return returnData.JsonSerializationt();
                }

                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                Table table = new Table(new enum_med_carList());
                SQLControl sQLControl_med_carInfo = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, SSLMode);
                List<object[]> list_medCart = sQLControl_med_carInfo.GetAllRows(null);

                List<medCarListClass> medCart_sql = list_medCart.SQLToClass<medCarListClass, enum_med_carList>();


                medCart_sql.Sort(new medCarListClass.ICP_By_phar_name());
                returnData.Code = 200;
                returnData.TimeTaken = $"{myTimerBasic}";
                returnData.Data = medCart_sql;
                returnData.Result = $"取得藥局資料共{medCart_sql.Count}";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = ex.Message;
                return returnData.JsonSerializationt(true);
            }

        }
        public class NaturalComparer : IComparer<string>
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            private static extern int StrCmpLogicalW(string psz1, string psz2);

            public int Compare(string x, string y)
            {
                return StrCmpLogicalW(x, y);
            }
        }


    }
}
