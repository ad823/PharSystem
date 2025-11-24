using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLUI;
using HIS_DB_Lib;
using H_Pannel_lib;
using Basic;
using MyUI;
using System.IO;
using System.Reflection;

namespace batch_dps_medUpdate
{
    class Program
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static public string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static void Main(string[] args)
        {
            string med_cloud_update = Basic.Net.WEBApiGet($"http://127.0.0.1:4434/dbvm/bbcm");
            List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet(API_Server);
            List<sys_serverSettingClass> sys_serverSettingClasses_buf = (from value in sys_serverSettingClasses
                                                                 where value.類別 == "調劑台"
                                                                 where value.內容 == "一般資料"
                                                                 select value).ToList();
            returnData returnData_med_cloud = new returnData();
            string json_result = Basic.Net.WEBApiPostJson($"http://127.0.0.1:4433/api/MED_page/get_med_cloud", returnData_med_cloud.JsonSerializationt());
            returnData_med_cloud = json_result.JsonDeserializet<returnData>();
            if (returnData_med_cloud.Code != 200)
            {
                Console.WriteLine($"雲端藥黨取得失敗,Result : {returnData_med_cloud.Result}");
                System.Threading.Thread.Sleep(3000);
                return;
            }
            List<medClass> med_cloud = returnData_med_cloud.Data.ObjToClass<List<medClass>>();
            if (sys_serverSettingClasses.Count == 0)
            {

                Console.WriteLine($"找無[sys_serverSettingClass]資料");
                System.Threading.Thread.Sleep(3000);
                return;
            }
            Logger.LogAddLine();
            for (int i = 0; i < sys_serverSettingClasses_buf.Count; i++)
            {
                try
                {
                    sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses_buf[i];
                    string 設備名稱 = sys_serverSettingClass.設備名稱;
                    string Server = sys_serverSettingClass.Server;
                    string DB = sys_serverSettingClass.DBName;
                    string UserName = sys_serverSettingClass.User;
                    string Password = sys_serverSettingClass.Password;
                    uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();
                    string TableName_medicine_page = "medicine_page";
                    SQLControl sQLControl_med_dps = new SQLControl(Server, DB, TableName_medicine_page, UserName, Password, Port, MySql.Data.MySqlClient.MySqlSslMode.None);

                    List<object[]> list_medcine_page = sQLControl_med_dps.GetAllRows(null);

                    List<medClass> med_cloud_buf = new List<medClass>();
                    List<medClass> medClasses = list_medcine_page.SQLToClass<medClass, enum_藥品資料_藥檔資料>();
                    List<medClass> medClasses_buf = new List<medClass>();
                    List<medClass> medClasses_add = new List<medClass>();
                    List<medClass> medClasses_replace = new List<medClass>();
                    List<object[]> list_med_add = new List<object[]>();
                    List<object[]> list_med_replace = new List<object[]>();
                    for (int k = 0; k < med_cloud.Count; k++)
                    {
               
                        medClasses_buf = (from temp in medClasses
                                          where temp.藥品碼 == med_cloud[k].藥品碼
                                          select temp).ToList();
                        if (medClasses_buf.Count == 0)
                        {
                            medClass medClass = new medClass();
                            medClass = med_cloud[k];
                            medClass.GUID = Guid.NewGuid().ToString();
                            medClasses_add.Add(medClass);
                        }
                        else
                        {
                            medClass medClass = new medClass();
                            medClass = medClasses_buf[0];
                            bool flag_replace = false;
                            if (medClass.中文名稱 == null) medClass.中文名稱 = "";
                            if (medClass.健保碼 == null) medClass.健保碼 = "";
                            if (medClass.包裝單位 == null) medClass.包裝單位 = "";
                            if (medClass.包裝數量 == null) medClass.包裝數量 = "";
                            if (medClass.廠牌 == null) medClass.廠牌 = "";
                            if (medClass.料號 == null) medClass.料號 = "";
                            if (medClass.最小包裝單位 == null) medClass.最小包裝單位 = "";
                            if (medClass.最小包裝數量 == null) medClass.最小包裝數量 = "";
                            if (medClass.生物製劑 == null) medClass.生物製劑 = "";
                            if (medClass.管制級別 == null) medClass.管制級別 = "";
                            if (medClass.藥品名稱 == null) medClass.藥品名稱 = "";
                            if (medClass.藥品學名 == null) medClass.藥品學名 = "";
                            if (medClass.藥品許可證號 == null) medClass.藥品許可證號 = "";
                            if (medClass.警訊藥品 == null) medClass.警訊藥品 = "";
                            if (medClass.開檔狀態 == null) medClass.開檔狀態 = "";
                            if (medClass.高價藥品 == null) medClass.高價藥品 = "";

                            if (med_cloud[k].中文名稱 == null) med_cloud[k].中文名稱 = "";
                            if (med_cloud[k].健保碼 == null) med_cloud[k].健保碼 = "";
                            if (med_cloud[k].包裝單位 == null) med_cloud[k].包裝單位 = "";
                            if (med_cloud[k].包裝數量 == null) med_cloud[k].包裝數量 = "";
                            if (med_cloud[k].廠牌 == null) med_cloud[k].廠牌 = "";
                            if (med_cloud[k].料號 == null) med_cloud[k].料號 = "";
                            if (med_cloud[k].最小包裝單位 == null) med_cloud[k].最小包裝單位 = "";
                            if (med_cloud[k].最小包裝數量 == null) med_cloud[k].最小包裝數量 = "";
                            if (med_cloud[k].生物製劑 == null) med_cloud[k].生物製劑 = "";
                            if (med_cloud[k].管制級別 == null) med_cloud[k].管制級別 = "";
                            if (med_cloud[k].藥品名稱 == null) med_cloud[k].藥品名稱 = "";
                            if (med_cloud[k].藥品學名 == null) med_cloud[k].藥品學名 = "";
                            if (med_cloud[k].藥品許可證號 == null) med_cloud[k].藥品許可證號 = "";
                            if (med_cloud[k].警訊藥品 == null) med_cloud[k].警訊藥品 = "";
                            if (med_cloud[k].開檔狀態 == null) med_cloud[k].開檔狀態 = "";
                            if (med_cloud[k].高價藥品 == null) med_cloud[k].高價藥品 = "";

                            if (medClass.中文名稱 != med_cloud[k].中文名稱) flag_replace = true;
                            if (medClass.健保碼 != med_cloud[k].健保碼) flag_replace = true;
                            if (medClass.包裝單位 != med_cloud[k].包裝單位) flag_replace = true;
                            //if (medClass.包裝數量 != med_cloud[k].包裝數量) flag_replace = true;
                            if (medClass.廠牌 != med_cloud[k].廠牌) flag_replace = true;
                            if (medClass.料號 != med_cloud[k].料號) flag_replace = true;
                            //if (medClass.最小包裝單位 != med_cloud[k].最小包裝單位) flag_replace = true;
                            //if (medClass.最小包裝數量 != med_cloud[k].最小包裝數量) flag_replace = true;
                            if (medClass.生物製劑 != med_cloud[k].生物製劑) flag_replace = true;
                            if (medClass.管制級別 != med_cloud[k].管制級別) flag_replace = true;
                            if (medClass.藥品名稱 != med_cloud[k].藥品名稱) flag_replace = true;
                            if (medClass.藥品學名 != med_cloud[k].藥品學名) flag_replace = true;
                            if (medClass.藥品許可證號 != med_cloud[k].藥品許可證號) flag_replace = true;
                            if (medClass.警訊藥品 != med_cloud[k].警訊藥品) flag_replace = true;
                            if (medClass.開檔狀態 != med_cloud[k].開檔狀態 && med_cloud[k].開檔狀態.StringIsEmpty() == false) flag_replace = true;
                            if (medClass.高價藥品 != med_cloud[k].高價藥品) flag_replace = true;

                            if (flag_replace)
                            {
                                medClass.中文名稱 = med_cloud[k].中文名稱;
                                medClass.健保碼 = med_cloud[k].健保碼;
                                medClass.包裝單位 = med_cloud[k].包裝單位;
                                medClass.包裝數量 = med_cloud[k].包裝數量;
                                medClass.廠牌 = med_cloud[k].廠牌;
                                medClass.料號 = med_cloud[k].料號;
                                medClass.最小包裝單位 = med_cloud[k].最小包裝單位;
                                medClass.最小包裝數量 = med_cloud[k].最小包裝數量;
                                medClass.生物製劑 = med_cloud[k].生物製劑;
                                medClass.管制級別 = med_cloud[k].管制級別;
                                medClass.藥品名稱 = med_cloud[k].藥品名稱;
                                medClass.藥品學名 = med_cloud[k].藥品學名;
                                medClass.藥品許可證號 = med_cloud[k].藥品許可證號;
                                medClass.警訊藥品 = med_cloud[k].警訊藥品;
                                if(med_cloud[k].開檔狀態.StringIsEmpty())
                                {
                                    if (medClass.開檔狀態.StringIsEmpty()) med_cloud[k].開檔狀態 = enum_開檔狀態.開檔中.GetEnumName();
                                }
                                else
                                {
                                    medClass.開檔狀態 = med_cloud[k].開檔狀態;
                                }
                      
              
                                medClass.高價藥品 = med_cloud[k].高價藥品;
                                medClasses_replace.Add(medClass);
                            }

                        }
                    }

                    list_med_add = medClasses_add.ClassToSQL<medClass, enum_藥品資料_藥檔資料>();
                    list_med_replace = medClasses_replace.ClassToSQL<medClass, enum_藥品資料_藥檔資料>();
                    if (list_med_add.Count > 0) sQLControl_med_dps.AddRows(null, list_med_add);
                    if (list_med_replace.Count > 0) sQLControl_med_dps.UpdateByDefulteExtra(null, list_med_replace);
               
                    Logger.Log($"{DateTime.Now.ToDateTimeString()} 設備名稱 : {設備名稱} , 新增<{list_med_add.Count}>筆 , 修改<{list_med_replace.Count}>筆");

                }
                catch(Exception e)
                {
                    Logger.Log($"Exception : {e.Message}");
                }
                finally
                {
                 
                }
              
            }
            Logger.LogAddLine();
            System.Threading.Thread.Sleep(5000);
        }
    }
}
