using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyUI;
using Basic;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using SQLUI;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using HIS_DB_Lib;
namespace 勤務傳送系統
{
    public partial class Main_Form : Form
    {
        static public void MediaPlay(string fileName)
        {
            try
            {
                using (System.Media.SoundPlayer sp = new System.Media.SoundPlayer($@"{fileName}"))
                {
                    sp.Stop();
                    sp.Play();
                    sp.PlaySync();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now.ToDateTimeString()} : [{System.Reflection.MethodBase.GetCurrentMethod().Name}] Exception:{e.Message}");
            }
    
        }
        public string Function_藥品碼檢查(string Code)
        {

            return Code;
        }
        public List<medClass> Function_搜尋Barcode(string barcode)
        {
            string url = $"{dBConfigClass.Api_URL}/api/MED_page/serch_by_BarCode";
            returnData returnData = new returnData(url);
            returnData.ServerName = dBConfigClass.Name;
            returnData.ServerType = enum_sys_serverSetting_Type.調劑台.GetEnumName();
            returnData.TableName = "medicine_page_cloud";
            returnData.Value = barcode;
            string json = returnData.ApiPostJson();
            if (returnData.ResultData == null)
            {
                MyMessageBox.ShowDialog("API連結失敗,請檢查網路或設定!");
                return new List<medClass>();
            }
            List<medClass> medClasses = returnData.ResultData.Data.ObjToListClass<medClass>();
            return medClasses;
        }
        private List<object[]> Function_醫令資料_API呼叫(string barcode)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            List<OrderClass> orderClasses = this.Function_醫令資料_API呼叫(dBConfigClass.OrderApiURL, barcode);
            List<object[]> list_value = orderClasses.ClassToSQL<OrderClass, enum_醫囑資料>();
       
            Console.Write($"醫令資料搜尋共<{list_value.Count}>筆,耗時{myTimer.ToString()}ms\n");
            return list_value;
        }
        private List<OrderClass> Function_醫令資料_API呼叫(string url, string barcode)
        {
            barcode = barcode.Replace("\r\n", "");
            barcode = Uri.EscapeDataString(barcode);
            List<OrderClass> orderClasses = new List<OrderClass>();
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            string apitext = $"{url}{barcode}";

            Console.Write($"Call api : {apitext}\n");
            string jsonString = Basic.Net.WEBApiGet(apitext);
            Console.Write($"{jsonString}\n");
            Console.Write($"耗時 {myTimer.ToString()}ms\n");
            if (jsonString.StringIsEmpty())
            {
                this.voice.SpeakOnTask("網路異常");
                MyMessageBox.ShowDialog($"呼叫串接資料失敗!請檢查網路連線...");
                return orderClasses;
            }
            returnData returnData = jsonString.JsonDeserializet<returnData>();
            if (returnData == null)
            {
                this.voice.SpeakOnTask("藥單條碼錯誤");
                MyMessageBox.ShowDialog(jsonString);
                return new List<OrderClass>();
            }
            if (returnData.Code != 200)
            {
                //MyMessageBox.ShowDialog($"{returnData.Result}");
                return new List<OrderClass>();

            }
            orderClasses = returnData.Data.ObjToListClass<OrderClass>();
            if (orderClasses == null)
            {
                Console.WriteLine($"串接資料傳回格式錯誤!");
                this.voice.SpeakOnTask("資料錯誤");
                orderClasses = new List<OrderClass>();

            }

            return orderClasses;
        }
        private void Funtion_藥袋刷入API(List<OrderClass> orderClasses)
        {
            string url = dBConfigClass.OrderCheckinApiURL;
   
            if (url.StringIsEmpty() == true) return;
            returnData returnData = new returnData();
            returnData.Data = orderClasses;
            string jsonin = returnData.JsonSerializationt();
            string json_result = Net.WEBApiPostJson(url, jsonin);
            Console.WriteLine($"\n");
            Console.WriteLine($"----------------------[藥袋刷入]----------------------");
            Console.WriteLine($"{json_result}");
        }

        private void Funtion_藥袋刷入API(OrderClass orderClass , string 操作人 ,string ID)
        {
            string url = dBConfigClass.OrderCheckinApiURL;
            orderClass.藥師姓名 = 操作人;
            orderClass.藥師ID = ID;
            if (url.StringIsEmpty() == true) return;
            returnData returnData = new returnData();
            returnData.Data = orderClass;
            string jsonin = returnData.JsonSerializationt();
            string json_result = Net.WEBApiPostJson(url, jsonin);
            Console.WriteLine($"\n");
            Console.WriteLine($"----------------------[藥袋刷入]----------------------");
            Console.WriteLine($"{json_result}");
        }
        private void Funtion_勤務取藥API(List<OrderClass> orderClasses)
        {
            string url = dBConfigClass.OrderTakeOutApiURL;

            if (url.StringIsEmpty() == true) return;
            returnData returnData = new returnData();
            returnData.Data = orderClasses;
            string jsonin = returnData.JsonSerializationt();
            string json_result = Net.WEBApiPostJson(url, jsonin);
            Console.WriteLine($"\n");
            Console.WriteLine($"----------------------[勤務取藥]----------------------");
            Console.WriteLine($"{json_result}");
        }
        private void Funtion_勤務取藥API(OrderClass orderClass, string 操作人, string ID)
        {
            string url = dBConfigClass.OrderTakeOutApiURL;
            orderClass.領藥姓名 = 操作人;
            orderClass.領藥ID = ID;
            if (url.StringIsEmpty() == true) return;
            returnData returnData = new returnData();
            returnData.Data = orderClass;
            string jsonin = returnData.JsonSerializationt();
            string json_result = Net.WEBApiPostJson(url, jsonin);
            Console.WriteLine($"\n");
            Console.WriteLine($"----------------------[勤務取藥]----------------------");
            Console.WriteLine($"{json_result}");
        }

        private void Funtion_開門回寫API(string 操作人, string ID , string WardName)
        {
            string url = dBConfigClass.DoorOpenApiURL;

            if (url.StringIsEmpty() == true) return;
            returnData returnData = new returnData();
            returnData.ValueAry.Add($"name={操作人}");
            returnData.ValueAry.Add($"id={ID}");
            returnData.ValueAry.Add($"wardname={WardName}");

            string jsonin = returnData.JsonSerializationt();
            string json_result = Net.WEBApiPostJson(url, jsonin);
            Console.WriteLine($"\n");
            Console.WriteLine($"----------------------[開門回寫]----------------------");
            Console.WriteLine($"{json_result}");
        }
        private void Funtion_關門回寫API(string 操作人, string ID, string WardName)
        {
            string url = dBConfigClass.DoorCloseApiURL;

            if (url.StringIsEmpty() == true) return;
            returnData returnData = new returnData();
            returnData.ValueAry.Add($"name={操作人}");
            returnData.ValueAry.Add($"id={ID}");
            returnData.ValueAry.Add($"wardname={WardName}");

            string jsonin = returnData.JsonSerializationt();
            string json_result = Net.WEBApiPostJson(url, jsonin);
            Console.WriteLine($"\n");
            Console.WriteLine($"----------------------[關門回寫]----------------------");
            Console.WriteLine($"{json_result}");
        }
    }
}
