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


namespace batch_medpic_download_to_base64
{
    class Program
    {
        static private string API_Server = "http://127.0.0.1:4433";
        static public string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static void Main(string[] args)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                Logger.LogAddLine();

                List<medClass> medClasses = medClass.get_med_cloud(API_Server);
                Logger.Log($"取得藥品資料共<{medClasses.Count}>筆");
                List<Task> tasks = new List<Task>();
                List<medPicClass> medPicClasses = new List<medPicClass>();
                List<object[]> list_medpic = new List<object[]>();
                string log = "";
                //string pic_base64 = Basic.Net.DownloadImageAsBase64("https://reg.ntuh.gov.tw/pharmacyoutside/DrugImage/New/PAM1LD48-A.jpg");
                int index = 0;
                foreach (medClass medClass in medClasses)
                {
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        medPicClass medPicClass = new medPicClass();
                        medPicClass.藥碼 = medClass.藥品碼;
                        medPicClass.藥名 = medClass.藥品名稱;
                        List<medPicClass> medPicClasses_buf = new List<medPicClass>();
                        bool flag_pic0_OK = false;
                        bool flag_pic1_OK = false;
                        if (medClass.圖片網址.StringIsEmpty() == false)
                        {
                            string pic_base64 = Basic.Net.DownloadImageAsBase64(medClass.圖片網址);
                            if (pic_base64.StringIsEmpty() == false)
                            {
                                medPicClass.副檔名 = GetFileExtension(medClass.圖片網址);
                                medPicClass.pic_base64 = pic_base64;
                                string losg_temp = $"({medClass.藥品碼}){medClass.藥品名稱}".StringLength(150) + $"取得圖片(0)Base64成功\n";
                                log += losg_temp;
                                //Console.Write(losg_temp);
                                flag_pic0_OK = true;
                            }
                            else
                            {
                                string losg_temp = $"({medClass.藥品碼}){medClass.藥品名稱}".StringLength(150) + $"取得圖片(0)Base64【失敗】\n";
                                log += losg_temp;
                                //Console.Write(losg_temp);
                            }
                        }
                        if (medClass.圖片網址1.StringIsEmpty() == false)
                        {
                            string pic1_base64 = Basic.Net.DownloadImageAsBase64(medClass.圖片網址1);

                            if (pic1_base64.StringIsEmpty() == false)
                            {
                                medPicClass.副檔名1 = GetFileExtension(medClass.圖片網址1);
                                medPicClass.pic1_base64 = pic1_base64;
                                string losg_temp = $"({medClass.藥品碼}){medClass.藥品名稱}".StringLength(150) + $"取得圖片(1)Base64成功\n";
                                log += losg_temp;
                                //Console.Write(losg_temp);
                                flag_pic1_OK = true;
                            }
                            else
                            {
                                string losg_temp = $"({medClass.藥品碼}){medClass.藥品名稱}".StringLength(150) + $"取得圖片(1)Base64【失敗】\n";
                                log += losg_temp;
                                //Console.Write(losg_temp);
                            }
                        }
                        index++;
                        Console.WriteLine($"{index}/{medClasses.Count}");
             
                        if (flag_pic0_OK || flag_pic1_OK) medPicClasses.LockAdd(medPicClass);
                    })));

                }
                Logger.Log(log);
        
                Task.WhenAll(tasks).Wait();
                for(int i = 0; i < medPicClasses.Count; i++)
                {
                    medPicClass.add(API_Server, medPicClasses[i]);
                }
                Logger.Log($"新增<{medPicClasses.Count}>筆藥品圖片至資料庫");

                Logger.LogAddLine();
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception : {ex.Message}");
            }
            finally
            {

            }

        }
        public static string GetFileExtension(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL不能為空", nameof(url));
            }

            int lastDotIndex = url.LastIndexOf('.');
            if (lastDotIndex == -1 || lastDotIndex == url.Length - 1)
            {
                throw new ArgumentException("URL不包含有效的副檔名", nameof(url));
            }

            return url.Substring(lastDotIndex).Replace(".","");
        }
    }


}
