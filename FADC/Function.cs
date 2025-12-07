using Basic;
using H_Pannel_lib;
using MinasA6DLL;
using MyUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLUI;
using HIS_DB_Lib;

namespace FADC
{
    public partial class Main_Form : Form
    {
        public static List<medPicClass> medPicClasses = new List<medPicClass>();
        public static List<Image> Function_取得藥品圖片(string Code)
        {
            bool IsValidImage(Image img)
            {
                return img != null && img.Width > 0 && img.Height > 0;
            }

            List<medPicClass> medPicClasse_buf = medPicClasses.Where(temp => temp.藥碼 == Code).ToList();
            List<Image> images = new List<Image>();

            if (medPicClasse_buf.Count == 0)
            {
                medPicClass medPicClass = new medPicClass();
                List<Image> loadedImages = medPicClass.get_images_by_code(Main_Form.API_Server, Code);

                medPicClass.藥碼 = Code;

                if (loadedImages != null)
                {
                    if (loadedImages.Count > 0 && IsValidImage(loadedImages[0]))
                    {
                        medPicClass.Image_0 = loadedImages[0];
                        images.Add(loadedImages[0]);
                    }
                    if (loadedImages.Count > 1 && IsValidImage(loadedImages[1]))
                    {
                        medPicClass.Image_1 = loadedImages[1];
                        images.Add(loadedImages[1]);
                    }
                }

                medPicClasses.Add(medPicClass);
                return images;
            }
            else
            {
                var cached = medPicClasse_buf[0];
                if (IsValidImage(cached.Image_0)) images.Add(cached.Image_0);
                if (IsValidImage(cached.Image_1)) images.Add(cached.Image_1);
                return images;
            }
        }
        static public void Function_從SQL取得儲位到本地資料()
        {

            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            Console.WriteLine($"開始SQL讀取儲位資料到本地!");
            List<Task> taskList = new List<Task>();
     
            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer1 = new MyTimer();
                myTimer1.StartTickTime(50000);
                List_EPD266_本地資料 = _storageUI_EPD_266.SQL_GetAllStorage();
                Console.WriteLine($"讀取EPD266資料! 耗時 :{myTimer1.GetTickTime().ToString("0.000")} ");

            }));
          
            List<Device> deviceBasics = new List<Device>();

            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();


            Console.WriteLine($"SQL讀取儲位資料到本地結束! 耗時 : {myTimer.GetTickTime().ToString("0.000")}");
        }
        static public List<object> Function_從SQL取得儲位到本地資料(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            List<Storage> storages = List_EPD266_本地資料.SortByCode(藥品碼);

            for (int i = 0; i < storages.Count; i++)
            {
                Storage storage = _storageUI_EPD_266.SQL_GetStorage(storages[i]);
                List_EPD266_本地資料.Add_NewStorage(storage);
                list_value.Add(storage);
            }
          
            return list_value;
        }
        static public double Function_從SQL取得庫存(string 藥品碼)
        {
            double 庫存 = 0;
            List<object> list_value = Function_從SQL取得儲位到本地資料(藥品碼);
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    Device device = list_value[i] as Device;
                    if (device != null)
                    {
                        庫存 += device.Inventory.StringToDouble();
                    }
                }
            }
            return 庫存;
        }

        static public List<Device> Function_從SQL取得所有儲位()
        {
            List<List<Device>> list_list_devices = new List<List<Device>>();
            List<Device> devices = new List<Device>();
            Function_從SQL取得儲位到本地資料();

            list_list_devices.Add(List_EPD266_本地資料.GetAllDevice());


            for (int i = 0; i < list_list_devices.Count; i++)
            {
                foreach (Device device in list_list_devices[i])
                {
                    device.確認效期庫存(true);
                    devices.Add(device);
                }
            }
            return devices;
        }
        static public List<chemotherapyOrderClass> Function_取得醫令(string barcode)
        {
            string url = $"{Order_URL}{barcode}";
            string json = Basic.Net.WEBApiGet(url);
            returnData returnData = json.JsonDeserializet<returnData>();
            if(returnData == null)
            {
                return null;
            }
            List<chemotherapyOrderClass> chemotherapyOrderClasses = returnData.Data.ObjToClass<List<chemotherapyOrderClass>>();
            return chemotherapyOrderClasses;

        }

        static public void Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作 enum_交易記錄查詢動作, string 操作人, string 備註)
        {
            if (操作人.StringIsEmpty()) return;
            string GUID = Guid.NewGuid().ToString();
            string 動作 = enum_交易記錄查詢動作.GetEnumName();
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 藥袋序號 = "";
            string 庫存量 = "";
            string 交易量 = "";
            string 結存量 = "";
            string 病人姓名 = "";
            string 病歷號 = "";
            string 操作時間 = DateTime.Now.ToDateTimeString_6();
            string 開方時間 = DateTime.Now.ToDateTimeString_6();
            object[] value = new object[new enum_交易記錄查詢資料().GetLength()];
            value[(int)enum_交易記錄查詢資料.GUID] = GUID;
            value[(int)enum_交易記錄查詢資料.動作] = 動作;
            value[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
            value[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
            value[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
            value[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
            value[(int)enum_交易記錄查詢資料.交易量] = 交易量;
            value[(int)enum_交易記錄查詢資料.結存量] = 結存量;
            value[(int)enum_交易記錄查詢資料.操作人] = 操作人;
            value[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
            value[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
            value[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
            value[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
            value[(int)enum_交易記錄查詢資料.領用時間] = DateTime.MinValue.ToDateTimeString();
            value[(int)enum_交易記錄查詢資料.備註] = 備註;

            transactionsClass.add(API_Server, value.SQLToClass<transactionsClass, enum_交易記錄查詢資料>(), ServerName, ServerType);
        }
    }
}
