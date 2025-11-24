using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
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
using Ionic.Zip;
using MyUI;
using H_Pannel_lib;
using HIS_DB_Lib;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : Controller
    {
        static private string API_Server = "http://127.0.0.1:4433/api/serversetting";
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        static private string ServerName = "update";
        static private string ServerType = "更新資訊";


        [Route("init")]
        [HttpGet]
        public string GET_init()
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(ServerName, ServerType, "一般資料");
                if (sys_serverSettingClasses.Count == 0)
                {
                    return $"找無Server資料!";
                }
                return CheckCreatTable(sys_serverSettingClasses[0]);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        [Route("")]
        [HttpGet]
        public string GET()
        {
            returnData returnData = new returnData();
            try
            {
                GET_init();
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(ServerName, ServerType, "一般資料");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Method = "all";
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt();
                }
                List<sys_updateVersionClass> sys_updateVersionClasses = GetAllUpdateVersion(sys_serverSettingClasses[0]);
                returnData.Code = 200;
                returnData.Data = sys_updateVersionClasses;
                returnData.Method = "all";
                returnData.Result = $"取得 update version 資訊成功! 共<{sys_updateVersionClasses.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch(Exception e)
            {
                returnData.Code = -200;
                returnData.Method = "all";
                returnData.Result = e.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [Route("add")]
        [HttpPost]
        public string POST_add([FromBody] returnData returnData)
        {
            try
            {
                GET_init();
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(ServerName, ServerType, "一般資料");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Method = "add";
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt(true);
                }
                List<sys_updateVersionClass> sys_updateVersionClasses = GetAllUpdateVersion(sys_serverSettingClasses[0]);
                List<sys_updateVersionClass> sys_updateVersionClasses_input = returnData.Data.ObjToListClass<sys_updateVersionClass>();
                if(sys_updateVersionClasses_input == null || sys_updateVersionClasses_input.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Method = "add";
                    returnData.Result = $"傳入資料資訊錯誤!";
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> list_value = sys_updateVersionClasses.ClassToSQL<sys_updateVersionClass, enum_sys_updateVersion>();
                List<object[]> list_value_buf = new List<object[]>();
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                List<object[]> list_value_input = sys_updateVersionClasses_input.ClassToSQL<sys_updateVersionClass, enum_sys_updateVersion>();
                for (int i = 0; i < list_value_input.Count; i++)
                {
                    object[] value = list_value_input[i];
                    string program_name = value[(int)enum_sys_updateVersion.program_name].ObjectToString();
                    list_value_buf = list_value.GetRows((int)enum_sys_updateVersion.program_name, program_name);
                    if(list_value_buf.Count == 0)
                    {
                        value[(int)enum_sys_updateVersion.GUID] = Guid.NewGuid().ToString();
                        list_value_add.Add(value);
                    }
                    else
                    {
                        value[(int)enum_sys_updateVersion.GUID] = list_value_buf[0][(int)enum_sys_updateVersion.GUID];
                        list_value_replace.Add(value);
                    }
                    value[(int)enum_sys_updateVersion.update_time] = DateTime.Now.ToDateTimeString();
                }
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                SQLControl sQLControl = new SQLControl(Server, DB, "update_version", UserName, Password, Port, SSLMode);
                if (list_value_add.Count > 0) sQLControl.AddRows(null, list_value_add);
                if (list_value_replace.Count > 0) sQLControl.UpdateByDefulteExtra(null, list_value_replace);
                returnData.Code = 200;
                returnData.Data = sys_updateVersionClasses_input;
                returnData.Method = "add";
                returnData.Result = $"新增 update version 資訊成功! 新增<{list_value_add.Count}>筆,修改<{list_value_replace.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Method = "add";
                returnData.Result = e.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [Route("delete")]
        [HttpPost]
        public string POST_delete([FromBody] returnData returnData)
        {
            try
            {
                GET_init();
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(ServerName, ServerType, "一般資料");
                if (sys_serverSettingClasses.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Method = "delete";
                    returnData.Result = $"找無Server資料!";
                    return returnData.JsonSerializationt(true);
                }
                List<sys_updateVersionClass> sys_updateVersionClasses = GetAllUpdateVersion(sys_serverSettingClasses[0]);
                sys_updateVersionClasses = (from temp in sys_updateVersionClasses
                                        where temp.program_name == returnData.Value
                                        select temp).ToList();

                if (returnData.Value.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Method = "delete";
                    returnData.Result = $"傳入資料資訊錯誤!";
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> list_value = sys_updateVersionClasses.ClassToSQL<sys_updateVersionClass, enum_sys_updateVersion>();
            
                string Server = sys_serverSettingClasses[0].Server;
                string DB = sys_serverSettingClasses[0].DBName;
                string UserName = sys_serverSettingClasses[0].User;
                string Password = sys_serverSettingClasses[0].Password;
                uint Port = (uint)sys_serverSettingClasses[0].Port.StringToInt32();
                SQLControl sQLControl = new SQLControl(Server, DB, "update_version", UserName, Password, Port, SSLMode);
                if (list_value.Count > 0) sQLControl.DeleteExtra(null, list_value);
                returnData.Code = 200;
                returnData.Data = "";
                returnData.Method = "delete";
                returnData.Result = $"刪除 update version 資訊成功! 刪除<{list_value.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Method = "delete";
                returnData.Result = e.Message;
                return returnData.JsonSerializationt(true);
            }
        }
        [Route("download/{value}")]
        [HttpGet]
        public async Task<ActionResult> GET_download(string value)
        {
            MemoryStream outputStream = new MemoryStream();
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(ServerName, ServerType, "一般資料");
                if (sys_serverSettingClasses.Count == 0) return null;
                List<sys_updateVersionClass> sys_updateVersionClasses = GetAllUpdateVersion(sys_serverSettingClasses[0]);
                sys_updateVersionClasses = (from temp in sys_updateVersionClasses
                                        where temp.program_name == value
                                        select temp).ToList();
                if (sys_updateVersionClasses.Count == 0) return null;
                string filepath = sys_updateVersionClasses[0].filepath;
                if (ContainerChecker.IsRunningInDocker())
                {
                    filepath = filepath.Replace("\\", "/");
                    string fileName = Path.GetFileName(filepath);
                    string directoryPath = Path.GetDirectoryName(filepath);
                    string folderName = Path.GetFileName(directoryPath);
                    filepath = $"/update_program/{folderName}/{fileName}";
                }
                using (ZipFile zip = new ZipFile(System.Text.Encoding.GetEncoding("big5")))
                {
                    string folderPath = Path.GetDirectoryName(filepath);
                    zip.AddDirectory(folderPath);
                    zip.Save(outputStream);
                }
                outputStream.Seek(0, SeekOrigin.Begin); // 将内存流的位置重置为开头

                return File(outputStream, "application/zip", "download.zip");
            }
            catch(Exception e)
            {
                //Console.WriteLine($"Exception : {e.Message}");
                return null;
            }
            finally
            {
                //outputStream.Dispose(); // 释放 outputStream 资源
            }
        }
        [Route("version/{value}")]
        [HttpGet]
        public string GET_version(string value)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(ServerName, ServerType, "一般資料");
                if (sys_serverSettingClasses.Count == 0) return null;
                List<sys_updateVersionClass> sys_updateVersionClasses = GetAllUpdateVersion(sys_serverSettingClasses[0]);
                sys_updateVersionClasses = (from temp in sys_updateVersionClasses
                                        where temp.program_name == value
                                        select temp).ToList();
                if (sys_updateVersionClasses.Count == 0) return "";
                string filepath = sys_updateVersionClasses[0].filepath;
     

                if (ContainerChecker.IsRunningInDocker())
                {
                    filepath = filepath.Replace("\\", "/");
                    string fileName = Path.GetFileName(filepath);
                    string directoryPath = Path.GetDirectoryName(filepath);
                    string folderName = Path.GetFileName(directoryPath);
                    filepath = $"/update_program/{folderName}/{fileName}";
                }
                Assembly assembly = Assembly.LoadFrom(filepath);
                string version = assembly.GetName().Version.ToString();

                return version;
            }
            catch(Exception e)
            {
                return e.Message;
            }

        }

        [Route("extension/{value}")]
        [HttpGet]
        public string GET_extension(string value)
        {
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = ServerSettingController.GetAllServerSetting();
                sys_serverSettingClasses = sys_serverSettingClasses.MyFind(ServerName, ServerType, "一般資料");
                if (sys_serverSettingClasses.Count == 0) return null;
                List<sys_updateVersionClass> sys_updateVersionClasses = GetAllUpdateVersion(sys_serverSettingClasses[0]);
                sys_updateVersionClasses = (from temp in sys_updateVersionClasses
                                        where temp.program_name == value
                                        select temp).ToList();
                if (sys_updateVersionClasses.Count == 0) return "";
                string filepath = sys_updateVersionClasses[0].filepath;
                return Path.GetExtension(filepath); 
            }
            catch
            {
                return "";
            }

        }
        private string CheckCreatTable(sys_serverSettingClass sys_serverSettingClass)
        {

            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            SQLControl sQLControl = new SQLControl(Server, DB, "update_version", UserName, Password, Port, SSLMode);

            Table table = new Table("update_version");
            table.AddColumnList("GUID", Table.StringType.VARCHAR, 50, Table.IndexType.PRIMARY);
            table.AddColumnList("program_name", Table.StringType.VARCHAR, 200, Table.IndexType.None);
            table.AddColumnList("version", Table.StringType.VARCHAR, 20, Table.IndexType.None);
            table.AddColumnList("filepath", Table.StringType.VARCHAR, 500, Table.IndexType.None);
            table.AddColumnList("update_time", Table.DateType.DATETIME, 50, Table.IndexType.None);
            table.AddColumnList("enable", Table.StringType.VARCHAR, 10, Table.IndexType.None);
            if (!sQLControl.IsTableCreat()) sQLControl.CreatTable(table);
            else sQLControl.CheckAllColumnName(table, true);
            return table.JsonSerializationt(true);
        }
        private List<sys_updateVersionClass> GetAllUpdateVersion(sys_serverSettingClass sys_serverSettingClass)
        {
            string Server = sys_serverSettingClass.Server;
            string DB = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = (uint)sys_serverSettingClass.Port.StringToInt32();

            SQLControl sQLControl = new SQLControl(Server, DB, "update_version", UserName, Password, Port, SSLMode);
            List<object[]> list_value = sQLControl.GetAllRows(null);
            List<sys_updateVersionClass> sys_updateVersionClasses = list_value.SQLToClass<sys_updateVersionClass, enum_sys_updateVersion>();
            return sys_updateVersionClasses;
        }

        private void GetZip(string folderPath)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(folderPath, Path.GetFileName(folderPath));
                    zip.Save(outputStream);
                }
            }
        }
    }
}
