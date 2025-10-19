using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using MyUI;
using Basic;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using H_Pannel_lib;
using HIS_DB_Lib;
using ZXing.QrCode.Internal;
using LadderProperty;
using DrawingClass;
namespace 調劑台管理系統
{
    public enum enum_儲位資訊
    {
        IP,
        TYPE,
        包裝量,
        效期,
        批號,
        庫存,
        異動量,
        Value,
    }
    public partial class Main_Form : Form
    {
        public static StorageUI_EPD_266 _storageUI_EPD_266 = null;
        public static StorageUI_LCD_114 _storageUI_LCD_114 = null;
        public static StorageUI_WT32 _storageUI_WT32 = null;
        public static DrawerUI_EPD_583 _drawerUI_EPD_583 = null;

        public static List<medPicClass> medPicClasses = new List<medPicClass>();

        public static List<Image> Function_取得藥品圖片(string Code)
        {
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

        /// <summary>
        /// 確保圖片不為 null 且寬高有效
        /// </summary>
        private static bool IsValidImage(Image img)
        {
            return img != null && img.Width > 0 && img.Height > 0;
        }



        static public List<OrderClass> Function_醫令領藥(string barcode, personPageClass personPageClass, string deviceName, bool single_order , UC_調劑作業_TypeA uC_depensing = null)
        {
            MyMessageBox.CloseAllDialog();
            Dialog_用藥警示.CloseAllDialog();
            List<takeMedicineStackClass> takeMedicineStackClasses = new List<takeMedicineStackClass>();
            MyTimer myTimer_total = new MyTimer();
            myTimer_total.StartTickTime(50000);
            string alarm_text = "";
            string ID = personPageClass.ID;
            string 操作人 = personPageClass.姓名;
            string 藥師證字號 = personPageClass.藥師證字號;
            string 顏色 = personPageClass.顏色;


            if (barcode.StringIsEmpty())
            {
                Console.WriteLine("barcode is empty");
                return null;
            }
            sessionClass _sessionClass = sessionClass.LoginByID(Main_Form.API_Server, personPageClass.ID, personPageClass.密碼);


            int daynum = PLC_Device_醫令檢查範圍.Value;
            if (daynum == 7) daynum = 7;
            if (daynum == 8) daynum = 14;
            if (daynum == 9) daynum = 21;
            if (daynum == 10) daynum = 28;
            daynum *= -1;
            double 手輸數量 = 0;
            List<OrderClass> orderClasses = new List<OrderClass>();
            List<OrderClass> orderClasses_buf = new List<OrderClass>(); 
            DateTime dateTime_start = new DateTime(DateTime.Now.AddDays(daynum).Year, DateTime.Now.AddDays(daynum).Month, DateTime.Now.AddDays(daynum).Day, 0, 0, 0);
            DateTime dateTime_end = new DateTime(DateTime.Now.AddDays(0).Year, DateTime.Now.AddDays(0).Month, DateTime.Now.AddDays(0).Day, 23, 59, 59);

            List<object[]> list_堆疊資料 = Function_取藥堆疊資料_取得母資料();
            List<object[]> list_堆疊資料_buf = new List<object[]>();
            Task Task_刪除資料 = Task.Run(() =>
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                if (!Main_Form.PLC_Device_多醫令模式.Bool)
                {
                    Function_取藥堆疊資料_刪除指定調劑台名稱母資料(deviceName);
                }
                Console.Write($"刪除調劑台資料資料 , 耗時{myTimer.ToString()}\n");
            });
            Task Task_取得醫令 = Task.Run(() =>
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                if (PLC_Device_手輸數量.Bool)
                {

                    Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel("請輸入領藥數量");
                    DialogResult dialogResult = dialog_NumPannel.ShowDialog();

                    Fuction_領藥台_時間重置();

                    if (dialogResult != DialogResult.Yes) return;
                    手輸數量 = dialog_NumPannel.Value * 1;

                    orderClasses = Function_醫令資料_API呼叫_Ex(barcode, 手輸數量);
                }
                else
                {
                    orderClasses = Function_醫令資料_API呼叫_Ex(barcode, single_order, OrderAction.領藥);
                }
                if (orderClasses.Count == 0)
                {
                    Voice.MediaPlayAsync($@"{currentDirectory}\藥單無資料.wav");
                    //Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("藥單無資料", 1500);
                    //dialog_AlarmForm.ShowDialog();
                    return;
                }

                orderClasses_buf = (from temp in orderClasses
                                where temp.開方日期.StringToDateTime() >= dateTime_start && temp.開方日期.StringToDateTime() <= dateTime_end
                                || temp.展藥時間.StringToDateTime() >= dateTime_start && temp.展藥時間.StringToDateTime() <= dateTime_end
                                select temp).ToList();


                if (orderClasses_buf.Count == 0)
                {
                    Voice.MediaPlayAsync($@"{currentDirectory}\藥單已過期.wav");
                    if (MyMessageBox.ShowDialog("藥單已過期,是否繼續調劑?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes)
                    {
                        orderClasses = new List<OrderClass>();
                        return;
                    }
                }

                Console.Write($"取得醫令資料 , 耗時{myTimer.ToString()}\n");


                if (PLC_Device_領藥處方選取.Bool)
                {
                    orderClasses_buf = new List<OrderClass>();
                    for (int i = 0; i < orderClasses.Count; i++)
                    {
                        string 藥碼 = orderClasses[i].藥品碼;
                        string 狀態 = orderClasses[i].狀態;
                        if (Main_Form.Function_從本地資料取得儲位(藥碼).Count > 0)
                        {
                            if (狀態 == "未過帳") orderClasses_buf.Add(orderClasses[i]);
                        }
                    }
                    if (orderClasses_buf.Count > 1)
                    {
                        Dialog_醫令選擇 dialog_醫令選擇 = new Dialog_醫令選擇(orderClasses_buf);
                        dialog_醫令選擇.ShowDialog();
                        Fuction_領藥台_時間重置();
                        if (dialog_醫令選擇.DialogResult != DialogResult.Yes) return;
                        orderClasses = dialog_醫令選擇.OrderClasses;
                    }
                }



                Console.Write($"取得藥品資料 , 耗時{myTimer.ToString()}\n");

                List<string> Codes = (from temp in orderClasses
                                      select temp.藥品碼).Distinct().ToList();
                List<medClass> medClasses = medClass.get_med_clouds_by_codes(API_Server, Codes);
                List<medClass> medClasses_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medcloud = medClasses.CoverToDictionaryByCode();


                List<object[]> list_取藥堆疊資料 = Function_取藥堆疊資料_取得指定調劑台名稱母資料(deviceName);
                List<object[]> list_取藥堆疊資料_buf = new List<object[]>();
                int flag_重複刷取 = 0;
                for (int i = 0; i < orderClasses.Count; i++)
                {
                    OrderClass orderClass = orderClasses[i];
                    string GUID = Guid.NewGuid().ToString();
                    string 調劑台名稱 = deviceName;
                    enum_交易記錄查詢動作 動作 = enum_交易記錄查詢動作.掃碼領藥;

                  
                    medClasses_buf = keyValuePairs_medcloud.SortDictionaryByCode(orderClass.藥品碼);
                    bool flag_檢查過帳 = false;
                 

                    list_堆疊資料_buf = (from temp in list_堆疊資料
                                     where temp[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString() == orderClass.藥品碼
                                     where temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString() != "刷新面板"
                                     where temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString() != 調劑台名稱
                                     where temp[(int)enum_取藥堆疊母資料.操作人].ObjectToString() != 操作人
                                     select temp).ToList();



                    takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();

                    if (medClasses_buf.Count > 0)
                    {
                        orderClass.藥品名稱 = medClasses_buf[0].藥品名稱;
                        orderClass.劑量單位 = medClasses_buf[0].包裝單位;
                        //if (medClasses_buf[0].高價藥品.ToUpper() == true.ToString().ToUpper())
                        //{
                        //    flag_檢查過帳 = true;
                        //}
                        //if (medClasses_buf[0].管制級別.StringIsEmpty() == false && medClasses_buf[0].管制級別 != "N")
                        //{
                        //    flag_檢查過帳 = true;
                        //}
                        list_取藥堆疊資料_buf = (from temp in list_堆疊資料
                                           where temp[(int)enum_取藥堆疊母資料.Order_GUID].ObjectToString() == orderClass.GUID
                                           where temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString() == deviceName
                                           select temp).ToList();
                        if (list_取藥堆疊資料_buf.Count > 0)
                        {
                            flag_重複刷取++;
                            continue;
                        }
                        PermissionsClass permissionsClass = _sessionClass.GetPermission("調劑台", "禁止調劑1-3級管制藥品");
                        if (permissionsClass != null)
                        {
                            if (_sessionClass.GetPermission("調劑台", "禁止調劑1-3級管制藥品").狀態)
                            {
                                if (medClasses_buf[0].管制級別 == "1" || medClasses_buf[0].管制級別 == "2" || medClasses_buf[0].管制級別 == "3")
                                {
                                    takeMedicineStackClass.狀態 = enum_取藥堆疊母資料_狀態.未授權.GetEnumName();
                                    orderClass.交易量 = "0";
                                }
                            }
                        }
                      
                    }
                    if (takeMedicineStackClass.狀態 != enum_取藥堆疊母資料_狀態.未授權.GetEnumName())
                    {
                        if (PLC_Device_領藥不檢查是否掃碼領藥過.Bool == false || flag_檢查過帳 == true)
                        {
                            if (orderClass.狀態 == enum_醫囑資料_狀態.已過帳.GetEnumName())
                            {
                                if (orderClass.實際調劑量.StringIsDouble() == false)
                                {
                                    takeMedicineStackClass.狀態 = enum_取藥堆疊母資料_狀態.已領用過.GetEnumName();
                                }
                                else
                                {
                                    if (orderClass.交易量 == orderClass.實際調劑量)
                                    {
                                        takeMedicineStackClass.狀態 = enum_取藥堆疊母資料_狀態.已領用過.GetEnumName();
                                    }
                                    else
                                    {
                                        if ((orderClass.交易量.StringToDouble() - orderClass.實際調劑量.StringToDouble() < 0))
                                        {
                                            double 原始交易量 = orderClass.交易量.StringToDouble();
                                            double 計算交易量 = orderClass.交易量.StringToDouble() - orderClass.實際調劑量.StringToDouble();
                                            if (Math.Abs(計算交易量) > Math.Abs(原始交易量)) 計算交易量 = 原始交易量;
                                            orderClass.交易量 = 計算交易量.ToString();
                                        }
                                    }
                                }

                            }
                        }
                    }


                    if (orderClass.交易量.StringToDouble() > 0) 動作 = enum_交易記錄查詢動作.掃碼退藥;



                    takeMedicineStackClass.GUID = GUID;
                    takeMedicineStackClass.Order_GUID  = orderClass.GUID;
                    takeMedicineStackClass.序號 = orderClass.批序;
                    takeMedicineStackClass.動作 = 動作.GetEnumName();
                    takeMedicineStackClass.調劑台名稱 = 調劑台名稱;
                    takeMedicineStackClass.藥品碼 = orderClass.藥品碼;
                    takeMedicineStackClass.領藥號 = orderClass.領藥號;
                    takeMedicineStackClass.病房號 = orderClass.病房;
                    takeMedicineStackClass.診別 = orderClass.藥局代碼;
                    takeMedicineStackClass.顏色 = 顏色;
                    if (list_堆疊資料_buf.Count > 0 && PLC_Device_同藥碼同時取藥亮紫色.Bool)
                    {
                        takeMedicineStackClass.顏色 = Color.Purple.ToColorString();
                    }
                    takeMedicineStackClass.藥品名稱 = orderClass.藥品名稱;
                    takeMedicineStackClass.單位 = orderClass.劑量單位;
                    takeMedicineStackClass.藥袋序號 = orderClass.PRI_KEY;
                    takeMedicineStackClass.病歷號 = orderClass.病歷號;
                    takeMedicineStackClass.病人姓名 = orderClass.病人姓名;
                    takeMedicineStackClass.床號 = orderClass.床號;
                    takeMedicineStackClass.開方時間 = orderClass.開方日期;
                    takeMedicineStackClass.操作人 = 操作人;
                    takeMedicineStackClass.ID = ID;
                    takeMedicineStackClass.藥師證字號 = 藥師證字號;
                    takeMedicineStackClass.總異動量 = orderClass.交易量;
                    takeMedicineStackClass.收支原因 = $"調劑{(orderClass.交易量.StringToDouble() > 0 ? "退" : "領")}藥";


                 
                    if(orderClass.批序.StringIsEmpty() == false)
                    {
                        if (orderClass.批序.Contains("DC"))
                        {
                            takeMedicineStackClass.備註 = "[DC處方]";
                            takeMedicineStackClass.狀態 = enum_取藥堆疊母資料_狀態.DC處方.GetEnumName();
                        }
                        else if (orderClass.批序.Contains("NEW"))
                        {
                            alarm_text += $"[NEW]-{takeMedicineStackClass.藥品名稱} ({takeMedicineStackClass.總異動量})\n";
                            takeMedicineStackClass.備註 = "[NEW處方]";
                        }
                    }
                    
   

                    takeMedicineStackClasses.Add(takeMedicineStackClass);

                }
                if(flag_重複刷取 > 0)
                {
                    Task.Run(new Action(delegate { $"有{flag_重複刷取}筆藥單重複刷取".PlayGooleVoiceAsync(Main_Form.API_Server); }));
                    if (Main_Form.PLC_Device_藥單重複刷取開檢查.Bool) return;
                }
            });
            List<Task> taskList = new List<Task>();
            taskList.Add(Task_刪除資料);
            taskList.Add(Task_取得醫令);
            Task.WhenAll(taskList).Wait();

            if (PLC_Device_導引模式.Bool)
            {
                List<string> codes = (from temp in takeMedicineStackClasses
                                      select temp.藥品碼).Distinct().ToList();

                List<medConfigClass> medConfigClasses = medConfigClass.get_dispense_note_by_codes(API_Server, codes);
                if (medConfigClasses.Count > 0)
                {
                    Dialog_使用者登入 dialog_使用者登入 = new Dialog_使用者登入();
                    dialog_使用者登入.ShowDialog();
                    if(dialog_使用者登入.DialogResult != DialogResult.Yes) return null;
                    personPageClass = dialog_使用者登入.personPageClass;
                    ID = personPageClass.ID;
                    操作人 = personPageClass.姓名;
                    藥師證字號 = personPageClass.藥師證字號;
                    if (Main_Form.PLC_Device_掃碼顏色固定.Bool == false) 顏色 = personPageClass.顏色;
                    for (int i = 0; i < takeMedicineStackClasses.Count; i++)
                    {
                        takeMedicineStackClasses[i].操作人 = 操作人;
                        takeMedicineStackClasses[i].ID = ID;
                        takeMedicineStackClasses[i].藥師證字號 = 藥師證字號;
                        takeMedicineStackClasses[i].顏色 = 顏色;
                    }
                    if (uC_depensing != null)
                    {
                        uC_depensing.Title = $" {(Main_Form.PLC_Device_導引模式.Bool ? "(導引模式)" : "")}[{操作人}]";
                    }
                }
            }

            taskList.Clear();

            taskList.Add(Task.Run(new Action(delegate
            {
                Function_取藥堆疊資料_新增母資料(takeMedicineStackClasses);
            })));

            Task.WhenAll(taskList).Wait();

            Console.Write($"掃碼完成 , 總耗時{myTimer_total.ToString()}\n");
            Voice.MediaPlayAsync($@"{currentDirectory}\sucess_01.wav");
            return orderClasses;
        }
        static public void Function_醫令退藥(string barcode, personPageClass personPageClass, string deviceName, bool single_order)
        {
            List<takeMedicineStackClass> takeMedicineStackClasses = new List<takeMedicineStackClass>();
            MyTimer myTimer_total = new MyTimer();
            myTimer_total.StartTickTime(50000);
            string ID = personPageClass.ID;
            string 操作人 = personPageClass.姓名;
            string 藥師證字號 = personPageClass.藥師證字號;
            string 顏色 = personPageClass.顏色;

            if (barcode.StringIsEmpty())
            {
                Console.WriteLine("barcode is empty");
                return;
            }


            int daynum = PLC_Device_醫令檢查範圍.Value;
            if (daynum == 7) daynum = 7;
            if (daynum == 8) daynum = 14;
            if (daynum == 9) daynum = 21;
            if (daynum == 10) daynum = 28;
            daynum *= -1;
            double 手輸數量 = 0;
            List<OrderClass> orderClasses = new List<OrderClass>();
            DateTime dateTime_start = new DateTime(DateTime.Now.AddDays(daynum).Year, DateTime.Now.AddDays(daynum).Month, DateTime.Now.AddDays(daynum).Day, 0, 0, 0);
            DateTime dateTime_end = new DateTime(DateTime.Now.AddDays(0).Year, DateTime.Now.AddDays(0).Month, DateTime.Now.AddDays(0).Day, 23, 59, 59);

            List<object[]> list_堆疊資料 = Function_取藥堆疊資料_取得母資料();
            List<object[]> list_堆疊資料_buf = new List<object[]>();
            Task Task_刪除資料 = Task.Run(() =>
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                if (!Main_Form.PLC_Device_多醫令模式.Bool)
                {
                    Function_取藥堆疊資料_刪除指定調劑台名稱母資料(deviceName);
                }
                Console.Write($"刪除調劑台資料資料 , 耗時{myTimer.ToString()}\n");
            });
            Task Task_取得醫令 = Task.Run(() =>
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                orderClasses = Function_醫令資料_API呼叫_Ex(barcode, single_order, OrderAction.退藥);
                if (orderClasses.Count == 0)
                {
                    Voice.MediaPlayAsync($@"{currentDirectory}\藥單無資料.wav");
                    Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("藥單無資料", 1500);
                    dialog_AlarmForm.ShowDialog();
                    return;
                }
                orderClasses = (from temp in orderClasses
                                where temp.開方日期.StringToDateTime() >= dateTime_start && temp.開方日期.StringToDateTime() <= dateTime_end
                                || temp.展藥時間.StringToDateTime() >= dateTime_start && temp.展藥時間.StringToDateTime() <= dateTime_end
                                select temp).ToList();


                if (orderClasses.Count == 0)
                {
                    Voice.MediaPlayAsync($@"{currentDirectory}\藥單已過期.wav");
                    Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("藥單已過期", 1500);
                    dialog_AlarmForm.ShowDialog();
                    return;
                }

                Console.Write($"取得醫令資料 , 耗時{myTimer.ToString()}\n");


                Dialog_醫令退藥 dialog_醫令退藥 = new Dialog_醫令退藥(orderClasses);
                if (dialog_醫令退藥.ShowDialog() != DialogResult.Yes) return;
                orderClasses = dialog_醫令退藥.orderClasses;
                Fuction_領藥台_時間重置();
                Console.Write($"取得藥品資料 , 耗時{myTimer.ToString()}\n");

                List<string> Codes = (from temp in orderClasses
                                      select temp.藥品碼).Distinct().ToList();
                List<medClass> medClasses = medClass.get_med_clouds_by_codes(API_Server, Codes);
                List<medClass> medClasses_buf = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_medcloud = medClasses.CoverToDictionaryByCode();

                for (int i = 0; i < orderClasses.Count; i++)
                {
                    OrderClass orderClass = orderClasses[i];
                    string GUID = Guid.NewGuid().ToString();
                    string 調劑台名稱 = deviceName;
                    enum_交易記錄查詢動作 動作 = enum_交易記錄查詢動作.掃碼退藥;

                    medClasses_buf = keyValuePairs_medcloud.SortDictionaryByCode(orderClass.藥品碼);
                    bool flag_檢查過帳 = false;
                    if (medClasses_buf.Count > 0)
                    {
                        orderClass.藥品名稱 = medClasses_buf[0].藥品名稱;
                        orderClass.劑量單位 = medClasses_buf[0].包裝單位;
                        if (medClasses_buf[0].高價藥品.ToUpper() == true.ToString().ToUpper())
                        {
                            flag_檢查過帳 = true;
                        }
                        if (medClasses_buf[0].管制級別.StringIsEmpty() == false && medClasses_buf[0].管制級別 != "N")
                        {
                            flag_檢查過帳 = true;
                        }
                    }

                    list_堆疊資料_buf = (from temp in list_堆疊資料
                                     where temp[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString() == orderClass.藥品碼
                                     where temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString() != "刷新面板"
                                     where temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString() != 調劑台名稱
                                     where temp[(int)enum_取藥堆疊母資料.操作人].ObjectToString() != 操作人
                                     select temp).ToList();
                    string 備註 = orderClasses[i].備註;
                    double 總異動量 = 0;
                    string 效期 = "";
                    string 批號 = "";
                    StockClass stockClass = convert_note(備註);
                    if (stockClass != null)
                    {
                        效期 = stockClass.Validity_period;
                        批號 = stockClass.Lot_number;
                        總異動量 = stockClass.Qty.StringToDouble();
                    }


                    takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
                    takeMedicineStackClass.GUID = GUID;
                    takeMedicineStackClass.Order_GUID = orderClass.GUID;
                    takeMedicineStackClass.動作 = 動作.GetEnumName();
                    takeMedicineStackClass.調劑台名稱 = 調劑台名稱;
                    takeMedicineStackClass.藥品碼 = orderClass.藥品碼;
                    takeMedicineStackClass.領藥號 = orderClass.領藥號;
                    takeMedicineStackClass.病房號 = orderClass.病房;
                    takeMedicineStackClass.診別 = orderClass.藥局代碼;
                    takeMedicineStackClass.顏色 = 顏色;
                    if (list_堆疊資料_buf.Count > 0 && PLC_Device_同藥碼同時取藥亮紫色.Bool)
                    {
                        takeMedicineStackClass.顏色 = Color.Purple.ToColorString();
                    }
                    takeMedicineStackClass.藥品名稱 = orderClass.藥品名稱;
                    takeMedicineStackClass.單位 = orderClass.劑量單位;
                    takeMedicineStackClass.藥袋序號 = orderClass.PRI_KEY;
                    takeMedicineStackClass.病歷號 = orderClass.病歷號;
                    takeMedicineStackClass.病人姓名 = orderClass.病人姓名;
                    takeMedicineStackClass.床號 = orderClass.床號;
                    takeMedicineStackClass.開方時間 = orderClass.開方日期;
                    takeMedicineStackClass.操作人 = 操作人;
                    takeMedicineStackClass.ID = ID;

                    takeMedicineStackClass.藥師證字號 = 藥師證字號;
                    takeMedicineStackClass.效期 = 效期;
                    takeMedicineStackClass.批號 = 批號;
                    takeMedicineStackClass.總異動量 = 總異動量.ToString();
                    takeMedicineStackClass.收支原因 = "退回調劑";
                    if (dialog_醫令退藥.收支原因.StringIsEmpty() == false)
                    {
                        takeMedicineStackClass.收支原因 = $"{takeMedicineStackClass.收支原因}[{dialog_醫令退藥.收支原因}]";
                    }

         


                    takeMedicineStackClasses.Add(takeMedicineStackClass);

                }
            });
            List<Task> taskList = new List<Task>();
            taskList.Add(Task_刪除資料);
            taskList.Add(Task_取得醫令);
            Task.WhenAll(taskList).Wait();

            taskList.Clear();

            taskList.Add(Task.Run(new Action(delegate
            {
                Function_取藥堆疊資料_新增母資料(takeMedicineStackClasses);
            })));

            Task.WhenAll(taskList).Wait();

            Console.Write($"掃碼完成 , 總耗時{myTimer_total.ToString()}\n");
            Voice.MediaPlayAsync($@"{currentDirectory}\sucess_01.wav");
        }


        public void Function_從SQL取得儲位到入賬資料(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_雲端資料.SortByCode(藥品碼);
            List<Storage> storages = List_EPD266_雲端資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_雲端資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_雲端資料.SortByCode(藥品碼);
            List<RFIDDevice> rFIDDevices = List_RFID_雲端資料.SortByCode(藥品碼);
            List<Box> boxes_1020 = List_EPD1020_本地資料.SortByCode(藥品碼);
            for (int i = 0; i < boxes_1020.Count; i++)
            {
                Drawer drawer = this.drawerUI_EPD_1020.SQL_GetDrawer(boxes_1020[i].IP);
                List_EPD1020_入賬資料.Add_NewDrawer(drawer);
            }
            for (int i = 0; i < storages.Count; i++)
            {
                Storage storage = this.storageUI_EPD_266.SQL_GetStorage(storages[i]);
                List_EPD266_入賬資料.Add_NewStorage(storage);
            }
            for (int i = 0; i < boxes.Count; i++)
            {
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(boxes[i]);
                List_EPD583_入賬資料.Add_NewDrawer(drawer);
            }

            for (int i = 0; i < pannels.Count; i++)
            {
                Storage pannel = this.storageUI_WT32.SQL_GetStorage(pannels[i]);
                List_Pannel35_入賬資料.Add_NewStorage(pannel);
            }
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                RowsLED rowsLED = this.rowsLEDUI.SQL_GetRowsLED(rowsDevices[i].IP);
                List_RowsLED_入賬資料.Add_NewRowsLED(rowsLED);
            }
            for (int i = 0; i < rFIDDevices.Count; i++)
            {
                RFIDClass rFIDClass = this.rfiD_UI.SQL_GetRFIDClass(rFIDDevices[i].IP);
                List_RFID_入賬資料.Add_NewRFIDClass(rFIDClass);
            }
        }
        public double Function_從入賬資料取得庫存(string 藥品碼)
        {
            double 庫存 = 0;
            List<object> list_value = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            this.Function_從入賬資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref list_value);

            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    庫存 += ((Device)list_value[i]).Inventory.StringToDouble();
                }
            }
            if (list_value.Count == 0) return -999;
            return 庫存;
        }
        public void Function_從入賬資料取得儲位(string 藥品碼, ref List<string> TYPE, ref List<object> values)
        {
            List<object> list_value = this.Function_從入賬資料取得儲位(藥品碼);
            TYPE.Clear();
            values.Clear();
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    Device device = (Device)list_value[i];
                    values.Add(list_value[i]);
                    TYPE.Add(device.DeviceType.GetEnumName());
                }

            }
        }
        public List<object> Function_從入賬資料取得儲位(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_入賬資料.SortByCode(藥品碼);
            List<Storage> storages = List_EPD266_入賬資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_入賬資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_入賬資料.SortByCode(藥品碼);
            List<RFIDDevice> rFIDDevices = List_RFID_入賬資料.SortByCode(藥品碼);
            List<Box> boxes_1020 = List_EPD1020_入賬資料.SortByCode(藥品碼);
            for (int i = 0; i < boxes_1020.Count; i++)
            {
                list_value.Add(boxes_1020[i]);
            }

            for (int i = 0; i < boxes.Count; i++)
            {
                list_value.Add(boxes[i]);
            }
            for (int i = 0; i < storages.Count; i++)
            {
                list_value.Add(storages[i]);
            }
            for (int i = 0; i < pannels.Count; i++)
            {
                list_value.Add(pannels[i]);
            }
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                list_value.Add(rowsDevices[i]);
            }
            for (int i = 0; i < rFIDDevices.Count; i++)
            {
                list_value.Add(rFIDDevices[i]);
            }
            return list_value;
        }
        public List<object[]> Function_取得異動儲位資訊從入賬資料(string 藥品碼, string 效期, string 批號, int 異動量)
        {
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            this.Function_從入賬資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref 儲位);
            List<object[]> 儲位資訊_buf = new List<object[]>();
            List<object[]> 儲位資訊 = new List<object[]>();
            if (儲位.Count == 0)
            {
                if (異動量 >= 0 && !效期.StringIsEmpty())
                {

                }
                return 儲位資訊_buf;
            }

            for (int k = 0; k < 儲位.Count; k++)
            {
                object value_device = 儲位[k];
                if (value_device is Device)
                {
                    Device device = (Device)value_device;
                    for (int i = 0; i < device.List_Validity_period.Count; i++)
                    {
                        object[] value = new object[new enum_儲位資訊().GetLength()];
                        value[(int)enum_儲位資訊.IP] = device.IP;
                        value[(int)enum_儲位資訊.TYPE] = 儲位_TYPE[k];
                        value[(int)enum_儲位資訊.效期] = device.List_Validity_period[i];
                        value[(int)enum_儲位資訊.庫存] = device.List_Inventory[i];
                        value[(int)enum_儲位資訊.異動量] = "0";
                        value[(int)enum_儲位資訊.Value] = value_device;
                        儲位資訊.Add(value);
                    }
                }
            }
            儲位資訊 = 儲位資訊.OrderBy(r => DateTime.Parse(r[(int)enum_儲位資訊.效期].ToDateString())).ToList();

            if (異動量 == 0) return 儲位資訊;
            double 使用數量 = 異動量;
            double 庫存數量 = 0;
            double 剩餘庫存數量 = 0;
            for (int i = 0; i < 儲位資訊.Count; i++)
            {
                庫存數量 = 儲位資訊[i][(int)enum_儲位資訊.庫存].ObjectToString().StringToDouble();
                if ((使用數量 < 0 && 庫存數量 > 0) || (使用數量 > 0 && 庫存數量 >= 0))
                {
                    剩餘庫存數量 = 庫存數量 + 使用數量;
                    if (剩餘庫存數量 >= 0)
                    {
                        儲位資訊[i][(int)enum_儲位資訊.異動量] = (使用數量).ToString();
                        儲位資訊_buf.Add(儲位資訊[i]);
                        break;
                    }
                    else
                    {
                        儲位資訊[i][(int)enum_儲位資訊.異動量] = (庫存數量 * -1).ToString();
                        使用數量 = 剩餘庫存數量;
                        儲位資訊_buf.Add(儲位資訊[i]);
                    }
                }
            }

            return 儲位資訊_buf;
        }
        public object Function_庫存異動至入賬資料(object[] 儲位資訊, bool upToSQL)
        {
            object Value = 儲位資訊[(int)enum_儲位資訊.Value];
            string 效期 = 儲位資訊[(int)enum_儲位資訊.效期].ObjectToString();
            string 異動量 = 儲位資訊[(int)enum_儲位資訊.異動量].ObjectToString();
            string TYPE = 儲位資訊[(int)enum_儲位資訊.TYPE].ObjectToString();
            if (Value is Storage)
            {
                if (TYPE == DeviceType.EPD266.GetEnumName() || TYPE == DeviceType.EPD266_lock.GetEnumName()
                   || TYPE == DeviceType.EPD290.GetEnumName() || TYPE == DeviceType.EPD290_lock.GetEnumName()
                   || TYPE == DeviceType.EPD420.GetEnumName() || TYPE == DeviceType.EPD420_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = List_EPD266_入賬資料.SortByIP(storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, false);
                        List_EPD266_入賬資料.Add_NewStorage(storage);
                        if (upToSQL) this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                        storage.UpToSQL = true;
                        return storage;
                    }
                }
                if (TYPE == DeviceType.Pannel35.GetEnumName() || TYPE == DeviceType.Pannel35_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = List_Pannel35_入賬資料.SortByIP(storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, false);
                        List_Pannel35_入賬資料.Add_NewStorage(storage);
                        if (upToSQL) this.storageUI_WT32.SQL_ReplaceStorage(storage);
                        storage.UpToSQL = true;
                        return storage;
                    }
                }
            }
            else if (Value is Box)
            {
                if (TYPE == DeviceType.EPD583.GetEnumName() || TYPE == DeviceType.EPD583_lock.GetEnumName() || TYPE == DeviceType.EPD420_D.GetEnumName() || TYPE == DeviceType.EPD420_D_lock.GetEnumName())
                {
                    Box box = (Box)Value;
                    box.效期庫存異動(效期, 異動量, false);
                    List_EPD583_入賬資料.ReplaceBox(box);
                    Drawer drawer = List_EPD583_入賬資料.SortByIP(box.IP);
                    if (upToSQL) this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                    drawer.UpToSQL = true;
                    return drawer;
                }
                if (TYPE == DeviceType.EPD1020.GetEnumName() || TYPE == DeviceType.EPD1020_lock.GetEnumName())
                {
                    Box box = (Box)Value;
                    box.效期庫存異動(效期, 異動量, false);
                    List_EPD1020_入賬資料.ReplaceByGUID(box);
                    Drawer drawer = List_EPD1020_入賬資料.SortByIP(box.IP);
                    if (upToSQL) this.drawerUI_EPD_1020.SQL_ReplaceDrawer(drawer);
                    drawer.UpToSQL = true;
                    return drawer;
                }
            }
            else if (Value is RowsDevice)
            {
                if (TYPE == DeviceType.RowsLED.GetEnumName())
                {
                    RowsDevice rowsDevice = Value as RowsDevice;
                    rowsDevice.效期庫存異動(效期, 異動量, false);
                    List_RowsLED_入賬資料.Add_NewRowsLED(rowsDevice);
                    RowsLED rowsLED = List_RowsLED_入賬資料.SortByIP(rowsDevice.IP);
                    if (upToSQL) this.rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                    rowsLED.UpToSQL = true;
                    return rowsLED;
                }

            }
            else if (Value is RFIDDevice)
            {
                if (TYPE == DeviceType.RFID_Device.GetEnumName())
                {
                    RFIDDevice rFIDDevice = Value as RFIDDevice;
                    rFIDDevice.效期庫存異動(效期, 異動量, false);
                    List_RFID_入賬資料.Add_NewRFIDClass(rFIDDevice);
                    RFIDClass rFIDClass = List_RFID_入賬資料.SortByIP(rFIDDevice.IP);
                    if (upToSQL) this.rfiD_UI.SQL_ReplaceRFIDClass(rFIDClass);
                    rFIDClass.UpToSQL = true;
                    return rFIDClass;
                }
            }
            return null;
        }

        public void Function_設定雲端資料更新()
        {
            takeMedicineStackClass takeMedicineStackClass = new takeMedicineStackClass();
            takeMedicineStackClass.調劑台名稱 = "更新資料";
            takeMedicineStackClass.動作 = enum_交易記錄查詢動作.None.GetEnumName();
            Function_取藥堆疊資料_新增母資料(takeMedicineStackClass);
        }
        static public void Function_從SQL取得儲位到雲端資料()
        {
            try
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                Console.WriteLine($"開始SQL讀取儲位資料到雲端!");
                List<Task> taskList = new List<Task>();
                taskList.Add(Task.Run(() =>
                {
                    MyTimer myTimer0 = new MyTimer();
                    myTimer0.StartTickTime(50000);
                    List_EPD583_雲端資料 = _drawerUI_EPD_583.SQL_GetAllDrawers();
                    Console.WriteLine($"讀取EPD583資料! 耗時 :{myTimer0.GetTickTime().ToString("0.000")} ");
                }));
                taskList.Add(Task.Run(() =>
                {
                    MyTimer myTimer0 = new MyTimer();
                    myTimer0.StartTickTime(50000);
                    List_EPD1020_雲端資料 = _drawerUI_EPD_1020.SQL_GetAllDrawers();
                    Console.WriteLine($"讀取EPD1020資料! 耗時 :{myTimer0.GetTickTime().ToString("0.000")} ");
                }));
                taskList.Add(Task.Run(() =>
                {
                    MyTimer myTimer1 = new MyTimer();
                    myTimer1.StartTickTime(50000);
                    List_EPD266_雲端資料 = _storageUI_EPD_266.SQL_GetAllStorage();
                    Console.WriteLine($"讀取EPD266資料! 耗時 :{myTimer1.GetTickTime().ToString("0.000")} ");

                }));
                taskList.Add(Task.Run(() =>
                {
                    MyTimer myTimer2 = new MyTimer();
                    myTimer2.StartTickTime(50000);
                    List_RowsLED_雲端資料 = _rowsLEDUI.SQL_GetAllRowsLED();
                    Console.WriteLine($"讀取RowsLED資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

                }));
                taskList.Add(Task.Run(() =>
                {
                    MyTimer myTimer2 = new MyTimer();
                    myTimer2.StartTickTime(50000);
                    List_Pannel35_雲端資料 = _storageUI_WT32.SQL_GetAllStorage();
                    Console.WriteLine($"讀取Pannel35資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

                }));
                //taskList.Add(Task.Run(() =>
                //{
                //    MyTimer myTimer2 = new MyTimer();
                //    myTimer2.StartTickTime(50000);
                //    List_RFID_雲端資料 = _rfiD_UI.SQL_GetAllRFIDClass();
                //    Console.WriteLine($"外部設備資料資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

                //}));
                taskList.Add(Task.Run(() =>
                {
                    MyTimer myTimer2 = new MyTimer();
                    myTimer2.StartTickTime(50000);
                    commonSapceClasses = Function_取得共用區所有儲位();


                }));
                Task allTask = Task.WhenAll(taskList);
                allTask.Wait();
                Console.WriteLine($"SQL讀取儲位資料到雲端結束! 耗時 : {myTimer.GetTickTime().ToString("0.000")}");
            }
            catch
            {

            }

        }
        static public List<object> Function_從SQL取得儲位到雲端資料(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            List<Task> tasks = new List<Task>();

            tasks.Add(Task.Run(new Action(delegate
            {
                List<Box> boxes = List_EPD583_雲端資料.SortByCode(藥品碼);
                for (int i = 0; i < boxes.Count; i++)
                {
                    Box box = _drawerUI_EPD_583.SQL_GetBox(boxes[i]);
                    List_EPD583_雲端資料.Add_NewDrawer(box);
                    list_value.LockAdd(box);
                }
            })));
            tasks.Add(Task.Run(new Action(delegate
            {
                List<Box> boxes_1020 = List_EPD1020_雲端資料.SortByCode(藥品碼);
                for (int i = 0; i < boxes_1020.Count; i++)
                {
                    Drawer drawer = _drawerUI_EPD_1020.SQL_GetDrawer(boxes_1020[i].IP);
                    List_EPD1020_雲端資料.Add_NewDrawer(drawer);
                    Box box = drawer.GetByGUID(boxes_1020[i].GUID);
                    list_value.LockAdd(box);
                }
            })));
            tasks.Add(Task.Run(new Action(delegate
            {
                List<Storage> storages = List_EPD266_雲端資料.SortByCode(藥品碼);
                for (int i = 0; i < storages.Count; i++)
                {
                    Storage storage = _storageUI_EPD_266.SQL_GetStorage(storages[i]);
                    List_EPD266_雲端資料.Add_NewStorage(storage);
                    list_value.LockAdd(storage);
                }
            })));
            tasks.Add(Task.Run(new Action(delegate
            {
                List<Storage> pannels = List_Pannel35_雲端資料.SortByCode(藥品碼);
                for (int i = 0; i < pannels.Count; i++)
                {
                    Storage pannel = _storageUI_WT32.SQL_GetStorage(pannels[i]);
                    List_Pannel35_雲端資料.Add_NewStorage(pannel);
                    list_value.LockAdd(pannel);
                }
            })));
            tasks.Add(Task.Run(new Action(delegate
            {
                List<RowsDevice> rowsDevices = List_RowsLED_雲端資料.SortByCode(藥品碼);
                for (int i = 0; i < rowsDevices.Count; i++)
                {
                    RowsLED rowsLED = _rowsLEDUI.SQL_GetRowsLED(rowsDevices[i].IP);
                    RowsDevice rowsDevice = rowsLED.GetRowsDevice(rowsDevices[i].GUID);
                    if (rowsDevice != null) list_value.LockAdd(rowsDevice);
                    List_RowsLED_雲端資料.Add_NewRowsLED(rowsDevice);
                }
            })));
            //tasks.Add(Task.Run(new Action(delegate
            //{
            //    List<RFIDDevice> rFIDDevices = List_RFID_雲端資料.SortByCode(藥品碼);
            //    for (int i = 0; i < rFIDDevices.Count; i++)
            //    {
            //        RFIDDevice rFIDDevice = rfiD_UI.SQL_GetDevice(rFIDDevices[i]);
            //        List_RFID_雲端資料.Add_NewRFIDClass(rFIDDevice);
            //        list_value.LockAdd(rFIDDevices);
            //    }
            //})));

            Task.WhenAll(tasks).Wait();













            return list_value;
        }
        static public List<object> Function_從雲端資料取得儲位(string 藥品碼)
        {
            List<object> list_value = new List<object>();

            // 使用 Task 執行每個集合的 SortByCode 操作
            var taskBoxes = Task.Run(() => List_EPD583_雲端資料.SortByCode(藥品碼));
            var taskBoxes1020 = Task.Run(() => List_EPD1020_雲端資料.SortByCode(藥品碼));
            var taskStorages = Task.Run(() => List_EPD266_雲端資料.SortByCode(藥品碼));
            var taskPannels = Task.Run(() => List_Pannel35_雲端資料.SortByCode(藥品碼));
            var taskRowsDevices = Task.Run(() => List_RowsLED_雲端資料.SortByCode(藥品碼));
            var taskRFIDDevices = Task.Run(() => List_RFID_雲端資料.SortByCode(藥品碼));

            // 使用 Task.WaitAll 同步等待所有任務完成
            Task.WaitAll(taskBoxes, taskBoxes1020, taskStorages, taskPannels, taskRowsDevices, taskRFIDDevices);

            // 將所有結果加入 list_value
            list_value.AddRange(taskStorages.Result); // storages
            list_value.AddRange(taskBoxes.Result); // boxes
            list_value.AddRange(taskBoxes1020.Result); // boxes_1020
            list_value.AddRange(taskPannels.Result); // pannels
            list_value.AddRange(taskRowsDevices.Result); // rowsDevices
            list_value.AddRange(taskRFIDDevices.Result); // rFIDDevices

            return list_value;
        }
        static public void Function_從雲端資料取得儲位(string 藥品碼, ref List<string> TYPE, ref List<object> values)
        {
            List<object> list_value = Function_從雲端資料取得儲位(藥品碼);
            TYPE.Clear();
            values.Clear();
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    Device device = (Device)list_value[i];
                    values.Add(list_value[i]);
                    TYPE.Add(device.DeviceType.GetEnumName());
                }

            }
        }
        static public double Function_從雲端資料取得庫存(string 藥品碼)
        {
            double 庫存 = 0;
            List<object> list_value = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref list_value);

            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    庫存 += ((Device)list_value[i]).Inventory.StringToDouble();
                }
            }
            if (list_value.Count == 0) return -999;
            return 庫存;
        }
        static public List<object[]> Function_取得異動儲位資訊從雲端資料(string 藥品碼, double 異動量, string 效期, string IP)
        {
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref 儲位);
            List<object[]> 儲位資訊_buf = new List<object[]>();
            List<object[]> 儲位資訊 = new List<object[]>();
            for (int k = 0; k < 儲位.Count; k++)
            {
                object value_device = 儲位[k];
                object[] value = new object[new enum_儲位資訊().GetLength()];
                if (value_device is Device)
                {
                    Device device = (Device)value_device;
                    for (int i = 0; i < device.List_Validity_period.Count; i++)
                    {
                        string device_Validity = device.List_Validity_period[i].StringToDateTime().ToDateString();
                        string validity = 效期.StringToDateTime().ToDateString();
                        if ((device_Validity == validity || 效期.StringIsEmpty()) && device.IP == IP)
                        {
                            value[(int)enum_儲位資訊.IP] = device.IP;
                            value[(int)enum_儲位資訊.TYPE] = 儲位_TYPE[k];
                            value[(int)enum_儲位資訊.效期] = device.List_Validity_period[i];
                            value[(int)enum_儲位資訊.批號] = device.List_Lot_number[i];
                            value[(int)enum_儲位資訊.庫存] = device.List_Inventory[i];
                            value[(int)enum_儲位資訊.異動量] = 異動量.ToString();
                            value[(int)enum_儲位資訊.Value] = value_device;
                            儲位資訊.Add(value);
                            break;
                        }
                    }
                }
            }
            return 儲位資訊;
        }
        static public StockClass Function_取得庫存值從雲端資料(string 藥品碼, string 效期)
        {
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref 儲位);

            int totalQty = 0;
            string lotNumber = null;
            string name = null;
            string validityPeriod = null;

            for (int k = 0; k < 儲位.Count; k++)
            {
                object value_device = 儲位[k];
                if (value_device is Device device)
                {
                    for (int i = 0; i < device.List_Validity_period.Count; i++)
                    {
                        if (device.List_Validity_period[i].StringToDateTime().ToDateString() == 效期.StringToDateTime().ToDateString())
                        {
                            int.TryParse(device.List_Inventory[i], out int qty);
                            totalQty += qty;

                            // 只記錄第一次找到的 lotNumber / name / validity
                            if (lotNumber == null) lotNumber = device.List_Lot_number[i];
                            if (name == null) name = device.Name;
                            if (validityPeriod == null) validityPeriod = device.List_Validity_period[i];
                        }
                    }
                }
            }

            if (totalQty > 0)
            {
                return new StockClass()
                {
                    Code = 藥品碼,
                    Name = name ?? "",  // 若找不到則用空字串
                    Validity_period = validityPeriod ?? 效期,
                    Lot_number = lotNumber ?? "",
                    Qty = totalQty.ToString()
                };
            }

            return null; // 如果完全沒找到符合的效期，就回傳 null
        }
        static public List<StockClass> Function_取得庫存值從雲端資料(string 藥品碼)
        {
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref 儲位);

            Dictionary<(string validity, string lot), StockClass> stockDict = new Dictionary<(string, string), StockClass>();

            for (int k = 0; k < 儲位.Count; k++)
            {
                object value_device = 儲位[k];
                if (value_device is Device device)
                {
                    for (int i = 0; i < device.List_Validity_period.Count; i++)
                    {
                        string validity = device.List_Validity_period[i];
                        string lot = device.List_Lot_number[i];

                        int.TryParse(device.List_Inventory[i], out int qty);
                        if (qty <= 0) continue;

                        var key = (validity, lot);

                        if (!stockDict.ContainsKey(key))
                        {
                            stockDict[key] = new StockClass()
                            {
                                Code = 藥品碼,
                                Name = device.Name ?? "",
                                Validity_period = validity,
                                Lot_number = lot,
                                Qty = qty.ToString()
                            };
                        }
                        else
                        {
                            // 如果已存在，累加數量
                            int currentQty = stockDict[key].Qty.StringToInt32();
                            stockDict[key].Qty = (currentQty + qty).ToString();
                        }
                    }
                }
            }

            return stockDict.Values.ToList();
        }

        static public List<object[]> Function_取得異動儲位資訊從雲端資料(string 藥品碼, double 異動量, string 效期)
        {
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref 儲位);
            List<object[]> 儲位資訊_buf = new List<object[]>();
            List<object[]> 儲位資訊 = new List<object[]>();
            for (int k = 0; k < 儲位.Count; k++)
            {
                object value_device = 儲位[k];
                object[] value = new object[new enum_儲位資訊().GetLength()];
                if (value_device is Device)
                {
                    Device device = (Device)value_device;
                    for (int i = 0; i < device.List_Validity_period.Count; i++)
                    {
                        if (device.List_Validity_period[i].StringToDateTime().ToDateString() == 效期.StringToDateTime().ToDateString())
                        {
                            value[(int)enum_儲位資訊.IP] = device.IP;
                            value[(int)enum_儲位資訊.TYPE] = 儲位_TYPE[k];
                            value[(int)enum_儲位資訊.效期] = device.List_Validity_period[i];
                            value[(int)enum_儲位資訊.批號] = device.List_Lot_number[i];
                            value[(int)enum_儲位資訊.庫存] = device.List_Inventory[i];
                            value[(int)enum_儲位資訊.異動量] = 異動量.ToString();
                            value[(int)enum_儲位資訊.Value] = value_device;
                            儲位資訊.Add(value);
                            break;
                        }
                    }
                }
            }
            return 儲位資訊;
        }


        static public List<object[]> Function_取得異動儲位資訊從雲端資料(string 藥品碼, double 異動量)
        {
            bool debug = false;
            if (debug) Console.WriteLine($"[取得異動儲位資訊] 藥品碼={藥品碼}, 異動量={異動量}");

            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref 儲位);

            List<object[]> 儲位資訊_buf = new List<object[]>();
            List<object[]> 儲位資訊 = new List<object[]>();

            if (儲位.Count == 0)
            {
                if (debug) Console.WriteLine("[取得異動儲位資訊] 無儲位資料");
                return 儲位資訊_buf;
            }
            // 組儲位資訊
            for (int k = 0; k < 儲位.Count; k++)
            {
                object value_device = 儲位[k];
                if (value_device is Device device)
                {
                    for (int i = 0; i < device.List_Validity_period.Count; i++)
                    {
                        object[] value = new object[new enum_儲位資訊().GetLength()];
                        value[(int)enum_儲位資訊.IP] = device.IP;
                        value[(int)enum_儲位資訊.TYPE] = 儲位_TYPE[k];
                        if (device.Min_Package_Num.StringToDouble() < 1) device.Min_Package_Num = "1";
                        value[(int)enum_儲位資訊.包裝量] = device.Min_Package_Num;
                        value[(int)enum_儲位資訊.效期] = device.List_Validity_period[i];
                        value[(int)enum_儲位資訊.批號] = device.List_Lot_number[i];
                        value[(int)enum_儲位資訊.庫存] = device.List_Inventory[i];
                        value[(int)enum_儲位資訊.異動量] = "0";
                        value[(int)enum_儲位資訊.Value] = value_device;
                        儲位資訊.Add(value);
                    }
                }
            }
            if (debug) Console.WriteLine($"[儲位總數] {儲位資訊.Count} 筆");

            for (int i = 0; i < 儲位資訊.Count; i++)
            {
                string ip = 儲位資訊[i][(int)enum_儲位資訊.IP].ObjectToString();
                string type = 儲位資訊[i][(int)enum_儲位資訊.TYPE].ObjectToString();
                string pack = 儲位資訊[i][(int)enum_儲位資訊.包裝量].ObjectToString();
                string lot = 儲位資訊[i][(int)enum_儲位資訊.批號].ObjectToString();
                string exp = 儲位資訊[i][(int)enum_儲位資訊.效期].ToDateString();
                string stock = 儲位資訊[i][(int)enum_儲位資訊.庫存].ObjectToString();

                if (debug) Console.WriteLine($"[儲位明細] IP={ip}, TYPE={type}, 包裝量={pack}, 效期={exp}, 批號={lot}, 庫存={stock}");
            }

            if (異動量 == 0)
            {
                if (debug) Console.WriteLine("[異動量為0] 無需異動");
                return 儲位資訊;
            }

            double 使用數量 = 異動量;
            儲位資訊_buf.Clear();

            // 分組
            var 儲位_大包裝 = 儲位資訊
                  .Where(r => r[(int)enum_儲位資訊.包裝量].StringToDouble() > 1)
                  .OrderByDescending(r => r[(int)enum_儲位資訊.包裝量].StringToDouble())
                  .ThenBy(r => TryParseDateTimeOrMax(r[(int)enum_儲位資訊.效期].ToDateString()))
                  .ToList();

            var 儲位_單包裝 = 儲位資訊
                  .Where(r => r[(int)enum_儲位資訊.包裝量].StringToDouble() == 1)
                  .OrderBy(r => TryParseDateTimeOrMax(r[(int)enum_儲位資訊.效期].ToDateString()))
                  .ToList();

            if (debug) Console.WriteLine($"[大包裝儲位] {儲位_大包裝.Count} 筆");
            if (debug) Console.WriteLine($"[單包裝儲位] {儲位_單包裝.Count} 筆");

            void 處理異動量(List<object[]> 儲位清單, string 類型)
            {
                for (int i = 0; i < 儲位清單.Count; i++)
                {
                    double 庫存數量 = 儲位清單[i][(int)enum_儲位資訊.庫存].ObjectToString().StringToDouble();
                    double 包裝量 = 儲位清單[i][(int)enum_儲位資訊.包裝量].ObjectToString().StringToDouble();
                    string IP = 儲位清單[i][(int)enum_儲位資訊.IP].ObjectToString();
                    string 效期 = 儲位清單[i][(int)enum_儲位資訊.效期].ObjectToString();

                    if (包裝量 <= 0) 包裝量 = 1;

                    double 可用包數 = (double)(庫存數量 / 包裝量);
                    double 需異動包數 = (double)(Math.Abs(使用數量) / 包裝量);
                    //if (需異動包數 == 0 && Math.Abs(使用數量) > 0) 需異動包數 = 1;

                    if ((使用數量 < 0 && 可用包數 > 0) || (使用數量 > 0 && 庫存數量 >= 0))
                    {
                        double 實際異動包數 = Math.Min(可用包數, 需異動包數);
                        double 異動量實值 = 實際異動包數 * 包裝量 * (使用數量 > 0 ? 1 : -1);

                        if (實際異動包數 > 0)
                        {
                            儲位清單[i][(int)enum_儲位資訊.異動量] = 異動量實值.ToString("0.#####");
                            儲位資訊_buf.Add(儲位清單[i]);

                            if (debug) Console.WriteLine($"[{類型}] IP={IP}, 效期={效期}, 包裝量={包裝量}, 庫存={庫存數量}, 異動量={異動量實值}, 剩餘異動={使用數量 - 異動量實值}");

                            使用數量 -= 異動量實值;

                            if ((異動量 > 0 && 使用數量 <= 0) || (異動量 < 0 && 使用數量 >= 0))
                                break;
                        }
                    }
                }
            }

            處理異動量(儲位_大包裝, "大包裝處理");

            if ((異動量 > 0 && 使用數量 > 0) || (異動量 < 0 && 使用數量 < 0))
            {
                if (debug) Console.WriteLine($"[進入單包裝處理] 剩餘異動量={使用數量}");
                處理異動量(儲位_單包裝, "單包裝處理");
            }

            if ((異動量 > 0 && 使用數量 > 0) || (異動量 < 0 && 使用數量 < 0))
            {
                if (debug) Console.WriteLine($"[異動不足警告] 剩餘未完成異動量={使用數量}");
            }

            if (debug) Console.WriteLine($"[異動完成] 已異動儲位數={儲位資訊_buf.Count}");
            return 儲位資訊_buf;
        }
        private static DateTime TryParseDateTimeOrMax(string dateStr)
        {
            if (DateTime.TryParse(dateStr, out DateTime dt))
                return dt;
            return DateTime.MaxValue; // 無法解析的效期放在最後
        }

        //public List<object[]> Function_取得異動儲位資訊從雲端資料(string 藥品碼, double 異動量)
        //{
        //    List<object> 儲位 = new List<object>();
        //    List<string> 儲位_TYPE = new List<string>();
        //    Function_從雲端資料取得儲位(Function_藥品碼檢查(藥品碼), ref 儲位_TYPE, ref 儲位);
        //    List<object[]> 儲位資訊_buf = new List<object[]>();
        //    List<object[]> 儲位資訊 = new List<object[]>();
        //    if (儲位.Count == 0) return 儲位資訊_buf;


        //    for (int k = 0; k < 儲位.Count; k++)
        //    {
        //        object value_device = 儲位[k];
        //        if (value_device is Device)
        //        {
        //            Device device = (Device)value_device;
        //            for (int i = 0; i < device.List_Validity_period.Count; i++)
        //            {
        //                object[] value = new object[new enum_儲位資訊().GetLength()];
        //                value[(int)enum_儲位資訊.IP] = device.IP;
        //                value[(int)enum_儲位資訊.TYPE] = 儲位_TYPE[k];
        //                if (device.Min_Package_Num.StringIsDouble() == false) device.Min_Package_Num = "1";
        //                value[(int)enum_儲位資訊.包裝量] = device.Min_Package_Num;
        //                value[(int)enum_儲位資訊.效期] = device.List_Validity_period[i];
        //                value[(int)enum_儲位資訊.批號] = device.List_Lot_number[i];
        //                value[(int)enum_儲位資訊.庫存] = device.List_Inventory[i];
        //                value[(int)enum_儲位資訊.異動量] = "0";
        //                value[(int)enum_儲位資訊.Value] = value_device;
        //                儲位資訊.Add(value);
        //            }
        //        }
        //    }
        //    儲位資訊 = 儲位資訊.OrderBy(r => DateTime.Parse(r[(int)enum_儲位資訊.效期].ToDateString())).ToList();

        //    if (異動量 == 0) return 儲位資訊;
        //    double 使用數量 = 異動量;
        //    double 庫存數量 = 0;
        //    double 剩餘庫存數量 = 0;
        //    for (int i = 0; i < 儲位資訊.Count; i++)
        //    {
        //        庫存數量 = 儲位資訊[i][(int)enum_儲位資訊.庫存].ObjectToString().StringToDouble();
        //        if ((使用數量 < 0 && 庫存數量 > 0) || (使用數量 > 0 && 庫存數量 >= 0))
        //        {
        //            剩餘庫存數量 = 庫存數量 + 使用數量;
        //            if (剩餘庫存數量 >= 0)
        //            {
        //                儲位資訊[i][(int)enum_儲位資訊.異動量] = (使用數量).ToString();
        //                儲位資訊_buf.Add(儲位資訊[i]);
        //                break;
        //            }
        //            else
        //            {
        //                儲位資訊[i][(int)enum_儲位資訊.異動量] = (庫存數量 * -1).ToString();
        //                使用數量 = 剩餘庫存數量;
        //                儲位資訊_buf.Add(儲位資訊[i]);
        //            }
        //        }
        //    }

        //    return 儲位資訊_buf;
        //}

        public List<object[]> Function_新增效期至雲端資料(string 藥品碼, double 異動量, string 效期, string 批號 ,string iP ="")
        {
            object value_device = new object();
            List<object[]> 儲位資訊 = new List<object[]>();
            List<string> TYPE = new List<string>();
            List<object> values = new List<object>();
            string IP = iP;
            Function_從雲端資料取得儲位(藥品碼, ref TYPE, ref values);
            string Type_str = "";
            for (int k = 0; k < values.Count; k++)
            {
                if (TYPE[k] == DeviceType.EPD266_lock.GetEnumName() || TYPE[k] == DeviceType.EPD266.GetEnumName()
                   || TYPE[k] == DeviceType.EPD290_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290.GetEnumName()
                   || TYPE[k] == DeviceType.EPD420_lock.GetEnumName() || TYPE[k] == DeviceType.EPD420.GetEnumName())
                {

                    Storage storage = (Storage)values[k];
                    value_device = storage;
                    if (storage.取得庫存(效期) == -1)
                    {
                        if (!IP.StringIsEmpty())
                        {
                            if (storage.IP != IP) continue;
                        }
                        storage.新增效期(效期, 批號, 異動量.ToString());
                        List_EPD266_雲端資料.Add_NewStorage(storage);

                        Type_str = TYPE[k];
                        break;
                    }

                }
                else if (TYPE[k] == DeviceType.Pannel35.GetEnumName() || TYPE[k] == DeviceType.Pannel35_lock.GetEnumName())
                {

                    Storage storage = (Storage)values[k];
                    value_device = storage;
                    if (storage.取得庫存(效期) == -1)
                    {
                        if (!IP.StringIsEmpty())
                        {
                            if (storage.IP != IP) continue;
                        }
                        storage.新增效期(效期, 批號, 異動量.ToString());
                        List_Pannel35_雲端資料.Add_NewStorage(storage);

                        Type_str = TYPE[k];
                        break;
                    }

                }
                else if (TYPE[k] == DeviceType.EPD583_lock.GetEnumName() || TYPE[k] == DeviceType.EPD583.GetEnumName() || TYPE[k] == DeviceType.EPD420_D.GetEnumName() || TYPE[k] == DeviceType.EPD420_D_lock.GetEnumName())
                {
                    Box box = (Box)values[k];
                    if (!IP.StringIsEmpty())
                    {
                        if (box.IP != IP) continue;
                    }
                    value_device = box;
                    if (box.取得庫存(效期) == -1)
                    {
                        box.新增效期(效期, 批號, "00");
                        Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                        drawer.ReplaceBox(box);
                        List_EPD583_雲端資料.Add_NewDrawer(drawer);
                        Type_str = TYPE[k];
                        break;
                    }
                }
                else if (TYPE[k] == DeviceType.EPD1020_lock.GetEnumName() || TYPE[k] == DeviceType.EPD1020.GetEnumName())
                {
                    Box box = (Box)values[k];
                    if (!IP.StringIsEmpty())
                    {
                        if (box.IP != IP) continue;
                    }
                    value_device = box;
                    if (box.取得庫存(效期) == -1)
                    {
                        box.新增效期(效期, 批號, "00");
                        Drawer drawer = List_EPD1020_雲端資料.SortByIP(box.IP);
                        drawer.ReplaceByGUID(box);
                        List_EPD1020_雲端資料.Add_NewDrawer(drawer);
                        Type_str = TYPE[k];
                        break;
                    }
                }
                else if (TYPE[k] == DeviceType.RowsLED.GetEnumName())
                {
                    RowsDevice rowsDevice = values[k] as RowsDevice;
                    if (!IP.StringIsEmpty())
                    {
                        if (rowsDevice.IP != IP) continue;
                    }
                    value_device = rowsDevice;
                    if (rowsDevice.取得庫存(效期) == -1)
                    {
                        rowsDevice.新增效期(效期, 批號, 異動量.ToString());
                        RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(rowsDevice.IP);
                        rowsLED.ReplaceRowsDevice(rowsDevice);
                        List_RowsLED_雲端資料.Add_NewRowsLED(rowsDevice);
                        Type_str = TYPE[k];
                        break;
                    }
                }
                else if (TYPE[k] == DeviceType.RFID_Device.GetEnumName())
                {
                    RFIDDevice rFIDDevice = values[k] as RFIDDevice;
                    if (!IP.StringIsEmpty())
                    {
                        if (rFIDDevice.IP != IP) continue;
                    }
                    value_device = rFIDDevice;
                    if (rFIDDevice.取得庫存(效期) == -1)
                    {
                        rFIDDevice.新增效期(效期, 批號, 異動量.ToString());
                        RFIDClass rFIDClass = List_RFID_雲端資料.SortByIP(rFIDDevice.IP);
                        rFIDClass.ReplaceRFIDDevice(rFIDDevice);
                        List_RFID_雲端資料.Add_NewRFIDClass(rFIDDevice);
                        Type_str = TYPE[k];
                        break;
                    }
                }
            }

            if (value_device is Device == false) return new List<object[]>();
            Device device = (Device)value_device;
            object[] value = new object[new enum_儲位資訊().GetLength()];
            value[(int)enum_儲位資訊.IP] = device.IP;
            value[(int)enum_儲位資訊.TYPE] = Type_str;
            value[(int)enum_儲位資訊.效期] = 效期;
            value[(int)enum_儲位資訊.批號] = 批號;
            value[(int)enum_儲位資訊.庫存] = device.Inventory;
            value[(int)enum_儲位資訊.異動量] = 異動量.ToString();
            value[(int)enum_儲位資訊.Value] = value_device;
            儲位資訊.Add(value);
            return 儲位資訊;
        }

        static public object Function_庫存異動至雲端資料(object[] 儲位資訊)
        {
            return Function_庫存異動至雲端資料(儲位資訊, false);
        }
        static public object Function_庫存異動至雲端資料(object device, string TYPE, string 效期, string 批號, string 異動量, bool upToSQL)
        {
            object Value = device;

            if (Value is Storage)
            {
                if (TYPE == DeviceType.EPD266.GetEnumName() || TYPE == DeviceType.EPD266_lock.GetEnumName()
                 || TYPE == DeviceType.EPD290.GetEnumName() || TYPE == DeviceType.EPD290_lock.GetEnumName()
                 || TYPE == DeviceType.EPD420.GetEnumName() || TYPE == DeviceType.EPD420_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = List_EPD266_雲端資料.SortByIP(storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 批號, 異動量, false);
                        List_EPD266_雲端資料.Add_NewStorage(storage);
                        if (upToSQL) _storageUI_EPD_266.SQL_ReplaceStorage(storage);
                        storage.UpToSQL = true;
                        return storage;
                    }
                }
                if (TYPE == DeviceType.Pannel35.GetEnumName() || TYPE == DeviceType.Pannel35_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = List_Pannel35_雲端資料.SortByIP(storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 批號, 異動量, false);
                        List_Pannel35_雲端資料.Add_NewStorage(storage);
                        if (upToSQL) _storageUI_WT32.SQL_ReplaceStorage(storage);
                        storage.UpToSQL = true;
                        return storage;
                    }
                }
            }
            else if (Value is Box)
            {
                if (TYPE == DeviceType.EPD583.GetEnumName() || TYPE == DeviceType.EPD583_lock.GetEnumName() || TYPE == DeviceType.EPD420_D.GetEnumName() || TYPE == DeviceType.EPD420_D_lock.GetEnumName())
                {
                    Box box = (Box)Value;
                    box.效期庫存異動(效期, 批號, 異動量, false);
                    List_EPD583_雲端資料.ReplaceBox(box);
                    Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                    if (upToSQL) _drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                    drawer.UpToSQL = true;
                    return drawer;
                }
                if (TYPE == DeviceType.EPD1020.GetEnumName() || TYPE == DeviceType.EPD1020_lock.GetEnumName())
                {
                    Box box = (Box)Value;
                    box.效期庫存異動(效期, 批號, 異動量, false);
                    List_EPD1020_雲端資料.ReplaceByGUID(box);
                    Drawer drawer = List_EPD1020_雲端資料.SortByIP(box.IP);
                    if (upToSQL) _drawerUI_EPD_1020.SQL_ReplaceDrawer(drawer);
                    drawer.UpToSQL = true;
                    return drawer;
                }
            }
            else if (Value is RowsDevice)
            {
                if (TYPE == DeviceType.RowsLED.GetEnumName())
                {
                    RowsDevice rowsDevice = Value as RowsDevice;
                    rowsDevice.效期庫存異動(效期, 批號, 異動量, false);
                    List_RowsLED_雲端資料.Add_NewRowsLED(rowsDevice);
                    RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(rowsDevice.IP);
                    if (upToSQL) _rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                    rowsLED.UpToSQL = true;
                    return rowsLED;
                }

            }
            //else if (Value is RFIDDevice)
            //{
            //    if (TYPE == DeviceType.RFID_Device.GetEnumName())
            //    {
            //        RFIDDevice rFIDDevice = Value as RFIDDevice;
            //        rFIDDevice.效期庫存異動(效期, 批號, 異動量, false);
            //        List_RFID_雲端資料.Add_NewRFIDClass(rFIDDevice);
            //        RFIDClass rFIDClass = List_RFID_雲端資料.SortByIP(rFIDDevice.IP);
            //        if (upToSQL) _rfiD_UI.SQL_ReplaceRFIDClass(rFIDClass);
            //        rFIDClass.UpToSQL = true;
            //        return rFIDClass;
            //    }
            //}
            return null;
        }
        static public object Function_庫存異動至雲端資料(object[] 儲位資訊, bool upToSQL)
        {
            object Value = 儲位資訊[(int)enum_儲位資訊.Value];
            string 效期 = 儲位資訊[(int)enum_儲位資訊.效期].ObjectToString();
            string 異動量 = 儲位資訊[(int)enum_儲位資訊.異動量].ObjectToString();
            string TYPE = 儲位資訊[(int)enum_儲位資訊.TYPE].ObjectToString();
            if (Value is Storage)
            {
                if (TYPE == DeviceType.EPD266.GetEnumName() || TYPE == DeviceType.EPD266_lock.GetEnumName()
                 || TYPE == DeviceType.EPD290.GetEnumName() || TYPE == DeviceType.EPD290_lock.GetEnumName()
                 || TYPE == DeviceType.EPD420.GetEnumName() || TYPE == DeviceType.EPD420_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = List_EPD266_雲端資料.SortByIP(storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, false);
                        List_EPD266_雲端資料.Add_NewStorage(storage);
                        if (upToSQL) _storageUI_EPD_266.SQL_ReplaceStorage(storage);
                        storage.UpToSQL = true;
                        return storage;
                    }
                }
                if (TYPE == DeviceType.Pannel35.GetEnumName() || TYPE == DeviceType.Pannel35_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = List_Pannel35_雲端資料.SortByIP(storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, false);
                        List_Pannel35_雲端資料.Add_NewStorage(storage);
                        if (upToSQL) _storageUI_WT32.SQL_ReplaceStorage(storage);
                        storage.UpToSQL = true;
                        return storage;
                    }
                }
            }
            else if (Value is Box)
            {
                if (TYPE == DeviceType.EPD583.GetEnumName() || TYPE == DeviceType.EPD583_lock.GetEnumName() || TYPE == DeviceType.EPD420_D.GetEnumName() || TYPE == DeviceType.EPD420_D_lock.GetEnumName())
                {
                    Box box = (Box)Value;
                    box.效期庫存異動(效期, 異動量, false);
                    List_EPD583_雲端資料.ReplaceBox(box);
                    Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                    if (upToSQL) _drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                    drawer.UpToSQL = true;
                    return drawer;
                }
                if (TYPE == DeviceType.EPD1020.GetEnumName() || TYPE == DeviceType.EPD1020_lock.GetEnumName())
                {
                    Box box = (Box)Value;
                    box.效期庫存異動(效期, 異動量, false);
                    List_EPD1020_雲端資料.ReplaceByGUID(box);
                    Drawer drawer = List_EPD1020_雲端資料.SortByIP(box.IP);
                    if (upToSQL) _drawerUI_EPD_1020.SQL_ReplaceDrawer(drawer);
                    drawer.UpToSQL = true;
                    return drawer;
                }
            }
            else if (Value is RowsDevice)
            {
                if (TYPE == DeviceType.RowsLED.GetEnumName())
                {
                    RowsDevice rowsDevice = Value as RowsDevice;
                    rowsDevice.效期庫存異動(效期, 異動量, false);
                    List_RowsLED_雲端資料.Add_NewRowsLED(rowsDevice);
                    RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(rowsDevice.IP);
                    if (upToSQL) _rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                    rowsLED.UpToSQL = true;
                    return rowsLED;
                }

            }
            //else if (Value is RFIDDevice)
            //{
            //    if (TYPE == DeviceType.RFID_Device.GetEnumName())
            //    {
            //        RFIDDevice rFIDDevice = Value as RFIDDevice;
            //        rFIDDevice.效期庫存異動(效期, 異動量, false);
            //        List_RFID_雲端資料.Add_NewRFIDClass(rFIDDevice);
            //        RFIDClass rFIDClass = List_RFID_雲端資料.SortByIP(rFIDDevice.IP);
            //        if (upToSQL) this.rfiD_UI.SQL_ReplaceRFIDClass(rFIDClass);
            //        rFIDClass.UpToSQL = true;
            //        return rFIDClass;
            //    }
            //}
            return null;
        }
        public void Function_雲端資料上傳至SQL()
        {
            List<Storage> list_EPD266 = List_EPD266_雲端資料.GetUpToSQL();
            List<Storage> list_Pannel35 = List_Pannel35_雲端資料.GetUpToSQL();
            List<Drawer> list_EPD583 = List_EPD583_雲端資料.GetUpToSQL();
            List<Drawer> list_EPD1020 = List_EPD1020_雲端資料.GetUpToSQL();
            List<RowsLED> list_RowsLED = List_RowsLED_雲端資料.GetUpToSQL();
            List<RFIDClass> list_RFID = List_RFID_雲端資料.GetUpToSQL();

            if (list_EPD266.Count > 0) this.storageUI_EPD_266.SQL_ReplaceStorage(list_EPD266);
            if (list_Pannel35.Count > 0) this.storageUI_WT32.SQL_ReplaceStorage(list_Pannel35);
            if (list_EPD583.Count > 0) this.drawerUI_EPD_583.SQL_ReplaceDrawer(list_EPD583);
            if (list_EPD1020.Count > 0) this.drawerUI_EPD_1020.SQL_ReplaceDrawer(list_EPD1020);
            if (list_RowsLED.Count > 0) this.rowsLEDUI.SQL_ReplaceRowsLED(list_RowsLED);
            if (list_RFID.Count > 0) this.rfiD_UI.SQL_ReplaceRFIDClass(list_RFID);

        }

        static public void Function_從SQL取得儲位到本地資料()
        {

            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            Console.WriteLine($"開始SQL讀取儲位資料到本地!");
            List<Task> taskList = new List<Task>();
            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer0 = new MyTimer();
                myTimer0.StartTickTime(50000);
                List_EPD583_本地資料 = _drawerUI_EPD_583.SQL_GetAllDrawers();
                Console.WriteLine($"讀取EPD583資料! 耗時 :{myTimer0.GetTickTime().ToString("0.000")} ");
            }));
            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer0 = new MyTimer();
                myTimer0.StartTickTime(50000);
                List_EPD1020_本地資料 = _drawerUI_EPD_1020.SQL_GetAllDrawers();
                Console.WriteLine($"讀取EPD1020資料! 耗時 :{myTimer0.GetTickTime().ToString("0.000")} ");
            }));
            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer1 = new MyTimer();
                myTimer1.StartTickTime(50000);
                List_EPD266_本地資料 = _storageUI_EPD_266.SQL_GetAllStorage();
                Console.WriteLine($"讀取EPD266資料! 耗時 :{myTimer1.GetTickTime().ToString("0.000")} ");

            }));
            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer2 = new MyTimer();
                myTimer2.StartTickTime(50000);
                List_RowsLED_本地資料 = _rowsLEDUI.SQL_GetAllRowsLED();
                Console.WriteLine($"讀取RowsLED資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

            }));
            //taskList.Add(Task.Run(() =>
            //{
            //    MyTimer myTimer2 = new MyTimer();
            //    myTimer2.StartTickTime(50000);
            //    List_RFID_本地資料 = this.rfiD_UI.SQL_GetAllRFIDClass();
            //    Console.WriteLine($"外部設備資料資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

            //}));
            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer2 = new MyTimer();
                myTimer2.StartTickTime(50000);
                List_Pannel35_本地資料 = _storageUI_WT32.SQL_GetAllStorage();
                Console.WriteLine($"讀取Pannel35資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

            }));
            List<Device> deviceBasics = new List<Device>();

            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();


            Console.WriteLine($"SQL讀取儲位資料到本地結束! 耗時 : {myTimer.GetTickTime().ToString("0.000")}");
        }
        static public List<object> Function_從本地資料取得儲位(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_本地資料.SortByCode(藥品碼);
            List<Box> boxes_1020 = List_EPD1020_本地資料.SortByCode(藥品碼);
            List<Storage> storages = List_EPD266_本地資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_本地資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_本地資料.SortByCode(藥品碼);
            List<RFIDDevice> rFIDDevices = List_RFID_本地資料.SortByCode(藥品碼);
            for (int i = 0; i < boxes.Count; i++)
            {
                list_value.Add(boxes[i]);
            }
            for (int i = 0; i < boxes_1020.Count; i++)
            {
                list_value.Add(boxes_1020[i]);
            }
            for (int i = 0; i < storages.Count; i++)
            {
                list_value.Add(storages[i]);
            }
            for (int i = 0; i < pannels.Count; i++)
            {
                list_value.Add(pannels[i]);
            }
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                list_value.Add(rowsDevices[i]);
            }
            for (int i = 0; i < rFIDDevices.Count; i++)
            {
                list_value.Add(rFIDDevices[i]);
            }
            return list_value;
        }
        static public object Fucnction_從本地資料取得儲位(string IP)
        {
            Storage storage = List_EPD266_本地資料.SortByIP(IP);
            if (storage != null) return storage;
            Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
            if (drawer != null) return drawer;
            Drawer drawer_1020 = List_EPD1020_本地資料.SortByIP(IP);
            if (drawer_1020 != null) return drawer_1020;
            RowsLED rowsLED = List_RowsLED_本地資料.SortByIP(IP);
            if (rowsLED != null) return rowsLED;
            RFIDClass rFIDClass = List_RFID_本地資料.SortByIP(IP);
            if (rFIDClass != null) return rFIDClass;
            Storage pannel35 = List_Pannel35_本地資料.SortByIP(IP);
            if (pannel35 != null) return pannel35;
            return null;
        }
        static public object Fucnction_從雲端資料取得儲位(string IP)
        {
            Storage storage = List_EPD266_雲端資料.SortByIP(IP);
            if (storage != null) return storage;
            Drawer drawer = List_EPD583_雲端資料.SortByIP(IP);
            if (drawer != null) return drawer;
            Drawer drawer_1020 = List_EPD1020_雲端資料.SortByIP(IP);
            if (drawer_1020 != null) return drawer_1020;
            RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(IP);
            if (rowsLED != null) return rowsLED;
            RFIDClass rFIDClass = List_RFID_雲端資料.SortByIP(IP);
            if (rFIDClass != null) return rFIDClass;
            Storage pannel35 = List_Pannel35_雲端資料.SortByIP(IP);
            if (pannel35 != null) return pannel35;
            return null;
        }
        public List<Device> Function_從SQL取得所有儲位()
        {
            List<List<Device>> list_list_devices = new List<List<Device>>();
            List<Device> devices = new List<Device>();
            Function_從SQL取得儲位到本地資料();

            list_list_devices.Add(List_EPD583_本地資料.GetAllDevice());
            list_list_devices.Add(List_EPD1020_本地資料.GetAllDevice());
            list_list_devices.Add(List_EPD266_本地資料.GetAllDevice());
            list_list_devices.Add(List_RowsLED_本地資料.GetAllDevice());
            list_list_devices.Add(List_RFID_本地資料.GetAllDevice());
            list_list_devices.Add(List_Pannel35_本地資料.GetAllDevice());

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
        static public List<object> Function_從SQL取得儲位到本地資料(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_本地資料.SortByCode(藥品碼);
            List<Box> boxes_1020 = List_EPD1020_本地資料.SortByCode(藥品碼);

            List<Storage> storages = List_EPD266_本地資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_本地資料.SortByCode(藥品碼);
            List<RFIDDevice> rFIDDevices = List_RFID_本地資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_本地資料.SortByCode(藥品碼);

            for (int i = 0; i < boxes.Count; i++)
            {
                Box box = _drawerUI_EPD_583.SQL_GetBox(boxes[i]);
                List_EPD583_本地資料.Add_NewDrawer(box);
                list_value.Add(box);
            }

            for (int i = 0; i < boxes_1020.Count; i++)
            {
                Box box = _drawerUI_EPD_1020.SQL_GetBox(boxes_1020[i]);
                List_EPD1020_本地資料.Add_NewDrawer(box);
                list_value.Add(box);
            }
            for (int i = 0; i < storages.Count; i++)
            {
                Storage storage = _storageUI_EPD_266.SQL_GetStorage(storages[i]);
                List_EPD266_本地資料.Add_NewStorage(storage);
                list_value.Add(storage);
            }
            for (int i = 0; i < pannels.Count; i++)
            {
                Storage pannel = _storageUI_WT32.SQL_GetStorage(pannels[i]);
                List_Pannel35_本地資料.Add_NewStorage(pannel);
                list_value.Add(pannel);
            }
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                RowsDevice rowsDevice = _rowsLEDUI.SQL_GetRowsDevice(rowsDevices[i]);
                List_RowsLED_本地資料.Add_NewRowsLED(rowsDevice);
                list_value.Add(rowsDevice);
            }
            for (int i = 0; i < rFIDDevices.Count; i++)
            {
                RFIDDevice rFIDDevice = _rFID_UI.SQL_GetDevice(rFIDDevices[i]);
                list_value.Add(rFIDDevice);
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
        static public long Function_從SQL取得排列號(string 藥品碼)
        {
            long index = 0;
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_本地資料.SortByCode(藥品碼);
            List<Box> boxes_1020 = List_EPD1020_本地資料.SortByCode(藥品碼);

            List<Storage> storages = List_EPD266_本地資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_本地資料.SortByCode(藥品碼);
            List<RFIDDevice> rFIDDevices = List_RFID_本地資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_本地資料.SortByCode(藥品碼);

            for (int i = 0; i < boxes.Count; i++)
            {
                Box box = boxes[i];
                string[] IPs = box.IP.Split('.');
                if (IPs.Length == 4)
                {
                    index += (IPs[2].StringToInt32() * 1000 + IPs[3].StringToInt32() * 100 + box.Column * 10 + box.Row * 1);
                }
            }

            for (int i = 0; i < storages.Count; i++)
            {
                Storage storage = storages[i];
                string[] IPs = storage.IP.Split('.');
                if (IPs.Length == 4)
                {
                    index += (IPs[2].StringToInt32() * 10000000 + IPs[3].StringToInt32() * 10000);
                }
            }
            for (int i = 0; i < pannels.Count; i++)
            {
                Storage storage = pannels[i];
                string[] IPs = storage.IP.Split('.');
                if (IPs.Length == 4)
                {
                    index += (IPs[2].StringToInt32() * 10000000 + IPs[3].StringToInt32() * 10000);
                }
            }

            return index;
        }
        public double Function_從本地資料取得庫存(string 藥品碼)
        {
            double 庫存 = 0;
            List<object> list_value = Function_從本地資料取得儲位(藥品碼);
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    Device device = (Device)list_value[i];
                    if (device != null)
                    {
                        庫存 += device.Inventory.StringToDouble();
                    }
                }
            }
            if (list_value.Count == 0) return -999;

            return 庫存;
        }
        static public void Function_全部滅燈()
        {
            List<Drawer> drawers_epd583 = new List<Drawer>();
            List<Storage> storages_epd266 = new List<Storage>();
            List<RowsLED> rowsLEDs = new List<RowsLED>();

            List<CommonSapceClass> commonSapceClasses = Function_取得共用區所有儲位();
            for (int i = 0; i < commonSapceClasses.Count; i++)
            {
                drawers_epd583.LockAdd(commonSapceClasses[i].List_EPD583);
                storages_epd266.LockAdd(commonSapceClasses[i].List_EPD266);
                rowsLEDs.LockAdd(commonSapceClasses[i].List_RowsLED);
            }
            drawers_epd583.LockAdd(List_EPD583_本地資料);
            storages_epd266.LockAdd(List_EPD266_本地資料);
            rowsLEDs.LockAdd(List_RowsLED_本地資料);

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < drawers_epd583.Count; i++)
            {
                Drawer drawer = drawers_epd583[i];
                tasks.Add(Task.Run(new Action(delegate
                {
                    _drawerUI_EPD_583.Set_LED_Clear_UDP(drawer);
                })));
            }


            for (int i = 0; i < storages_epd266.Count; i++)
            {
                Storage storage = storages_epd266[i];
                tasks.Add(Task.Run(new Action(delegate
                {
                    _storageUI_EPD_266.Set_Stroage_LED_UDP(storage, Color.Black);
                })));
            }

            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                RowsLED rowsLED = rowsLEDs[i];
                tasks.Add(Task.Run(new Action(delegate
                {
                    _rowsLEDUI.Set_Rows_LED_Clear_UDP(rowsLED);
                })));
            }

            List<object[]> list_value = _sqL_DataGridView_LCD114_索引表.SQL_GetAllRows(false);
            for (int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_LCD114_索引表.index_IP].ObjectToString();
                try
                {
                    tasks.Add(Task.Run(new Action(delegate
                    {
                        _storageUI_LCD_114.ClearCanvas(IP, 29008);
                    })));

                }
                catch
                {

                }


            }
        }

        static public void Function_儲位亮燈(LightOn lightOn)
        {
            if (PLC_Device_主機輸出模式.Bool == false)
            {
                return;
            }
            List<string> list_lock_IP = new List<string>();
            Function_儲位亮燈(lightOn, ref list_lock_IP);
        }
        static public void Function_儲位亮燈(LightOn lightOn, ref List<string> list_lock_IP)
        {
            string 藥品碼 = lightOn.藥品碼;
            Color color = lightOn.顏色;
            if (藥品碼.StringIsEmpty()) return;

            if (color == Color.Black)
            {
                List<object[]> list_取藥堆疊母資料 = _sqL_DataGridView_取藥堆疊母資料.SQL_GetRows((int)enum_取藥堆疊母資料.藥品碼, 藥品碼, false);

                list_取藥堆疊母資料 = (from temp in list_取藥堆疊母資料
                                where temp[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString() == 藥品碼
                                where temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString() != "刷新面板"
                                where temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != "入賬完成"
                                select temp).ToList();
                if (list_取藥堆疊母資料.Count != 0) return;
            }

            if (color == Color.DarkGray)
            {
                color = Color.Black;
                lightOn.顏色 = color;
            }

            List<LightOn> lightOns_buf = (from temp in lightOns
                                          where temp.藥品碼 == lightOn.藥品碼
                                          where temp.顏色 == lightOn.顏色
                                          where temp.flag_Refresh_LCD == lightOn.flag_Refresh_LCD
                                          where temp.flag_Refresh_Light == lightOn.flag_Refresh_Light
                                          where temp.flag_Refresh_breathing == lightOn.flag_Refresh_breathing
                                          select temp).ToList();

            if (lightOns_buf.Count == 0) lightOns.Add(lightOn);

            List<object> list_Device = new List<object>();
            list_Device.LockAdd(Function_從雲端資料取得儲位(藥品碼));
            //list_Device.LockAdd(Function_從共用區取得儲位(藥品碼));
            Task allTask;
            List<Task> taskList = new List<Task>();
            List<string> list_IP = new List<string>();
            List<string> list_IP_buf = new List<string>();
            bool flag_led_refresh = false;

            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;
                list_IP_buf = (from value in list_IP
                               where value == IP
                               select value).ToList();
                if (list_IP_buf.Count > 0) continue;

                if (device != null)
                {
                    if (device.DeviceIsStorage())
                    {
                        Storage storage = list_Device[i] as Storage;
                        if (storage != null)
                        {
                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.EPD266_lock || device.DeviceType == DeviceType.EPD290_lock
                              || device.DeviceType == DeviceType.EPD420_lock)
                            {
                                list_lock_IP.Add(IP);
                            }
                        }
                    }
                    else if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock || device.DeviceType == DeviceType.EPD420_D || device.DeviceType == DeviceType.EPD420_D_lock)
                    {
                        Box box = list_Device[i] as Box;
                        if (box != null)
                        {
                            list_IP.Add(IP);
                            list_lock_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.EPD1020 || device.DeviceType == DeviceType.EPD1020_lock)
                    {
                        Box box = list_Device[i] as Box;
                        if (box != null)
                        {

                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.EPD1020_lock) list_lock_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.Pannel35 || device.DeviceType == DeviceType.Pannel35_lock)
                    {
                        Storage storage = list_Device[i] as Storage;
                        if (storage != null)
                        {
                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.Pannel35 || device.DeviceType == DeviceType.Pannel35_lock) list_lock_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.RowsLED)
                    {
                        RowsDevice rowsDevice = list_Device[i] as RowsDevice;
                        if (rowsDevice != null)
                        {
                            list_IP.Add(IP);
                        }
                    }
              
                }
            }

        }


        static public void Function_儲位刷新(string 藥品碼, int 庫存)
        {
            List<object> list_Device = Function_從本地資料取得儲位(藥品碼);
            List<Task> taskList = new List<Task>();
            List<string> list_IP = new List<string>();
            List<string> list_IP_buf = new List<string>();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;

                list_IP_buf = (from value in list_IP
                               where value == IP
                               select value).ToList();
                if (list_IP_buf.Count > 0) continue;

                if (device != null)
                {
                    if (device.DeviceType == DeviceType.EPD213 || device.DeviceType == DeviceType.EPD213_lock
                     || device.DeviceType == DeviceType.EPD266 || device.DeviceType == DeviceType.EPD266_lock
                     || device.DeviceType == DeviceType.EPD290 || device.DeviceType == DeviceType.EPD290_lock
                     || device.DeviceType == DeviceType.EPD420 || device.DeviceType == DeviceType.EPD420_lock)
                    {
                        Storage storage = list_Device[i] as Storage;
                        if (storage != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                storage.清除所有庫存資料();
                                storage.新增效期("2050/12/31", 庫存.ToString());
                                _storageUI_EPD_266.DrawToEpd_UDP(storage);
                            }));

                            list_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock || device.DeviceType == DeviceType.EPD420_D || device.DeviceType == DeviceType.EPD420_D_lock)
                    {
                        Box box = list_Device[i] as Box;
                        if (box != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                Drawer drawer = List_EPD583_雲端資料.SortByIP(IP);
                                List<Box> boxes = drawer.SortByCode(藥品碼);
                                for (int k = 0; k < boxes.Count; k++)
                                {
                                    boxes[k].清除所有庫存資料();
                                    boxes[k].新增效期("2050/12/31", 庫存.ToString());
                                }
                                _drawerUI_EPD_583.DrawToEpd_UDP(drawer);
                            }));

                            list_IP.Add(IP);
                        }
                    }



                    Task.WhenAll(taskList).Wait();
                }
            }

        }
        static public void Function_儲位刷新(string 藥品碼)
        {
            List<string> list_lock_IP = new List<string>();
            Function_儲位刷新(藥品碼, ref list_lock_IP);
        }
        static public void Function_儲位刷新(string 藥品碼, ref List<string> list_lock_IP)
        {
            List<object> list_Device = Function_從本地資料取得儲位(藥品碼);
            List<Task> taskList = new List<Task>();
            List<string> list_IP = new List<string>();
            List<string> list_IP_buf = new List<string>();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;

                list_IP_buf = (from value in list_IP
                               where value == IP
                               select value).ToList();
                if (list_IP_buf.Count > 0) continue;

                if (device != null)
                {
                    if (device.DeviceType == DeviceType.EPD213 || device.DeviceType == DeviceType.EPD213_lock
                     || device.DeviceType == DeviceType.EPD266 || device.DeviceType == DeviceType.EPD266_lock
                     || device.DeviceType == DeviceType.EPD290 || device.DeviceType == DeviceType.EPD290_lock
                     || device.DeviceType == DeviceType.EPD420 || device.DeviceType == DeviceType.EPD420_lock)
                    {
                        Storage storage = list_Device[i] as Storage;
                        if (storage != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                _storageUI_EPD_266.DrawToEpd_UDP(storage);
                            }));

                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.EPD266_lock || device.DeviceType == DeviceType.EPD290_lock || device.DeviceType == DeviceType.EPD420_lock) list_lock_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock || device.DeviceType == DeviceType.EPD420_D || device.DeviceType == DeviceType.EPD420_D_lock)
                    {
                        Box box = list_Device[i] as Box;
                        if (box != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                Drawer drawer = List_EPD583_雲端資料.SortByIP(IP);
                                List<Box> boxes = drawer.SortByCode(藥品碼);
                                _drawerUI_EPD_583.DrawToEpd_UDP(drawer);
                            }));

                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.EPD583_lock || device.DeviceType == DeviceType.EPD420_D_lock) list_lock_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.EPD1020 || device.DeviceType == DeviceType.EPD1020_lock)
                    {
                        Box box = list_Device[i] as Box;
                        if (box != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                Drawer drawer = List_EPD1020_雲端資料.SortByIP(IP);
                                List<Box> boxes = drawer.SortByCode(藥品碼);
                                //if (!plC_CheckBox_測試模式.Checked)
                                //{
                                //    this.drawerUI_EPD_1020.Set_LED_UDP(drawer);
                                //}

                            }));

                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.EPD1020_lock) list_lock_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.Pannel35 || device.DeviceType == DeviceType.Pannel35_lock)
                    {
                        Storage storage = list_Device[i] as Storage;
                        if (storage != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                _storageUI_WT32.Set_DrawPannelJEPG(storage);

                            }));

                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.Pannel35 || device.DeviceType == DeviceType.Pannel35_lock) list_lock_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.RowsLED)
                    {
                        RowsDevice rowsDevice = list_Device[i] as RowsDevice;
                        if (rowsDevice != null)
                        {
                            RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(rowsDevice.IP);

                            list_IP.Add(IP);
                        }
                    }

                    Task.WhenAll(taskList).Wait();
                }
            }

        }
        public void Function_儲位刷新_byIP(string IP)
        {
            List<string> list_lock_IP = new List<string>();
            this.Function_儲位刷新_byIP(IP, ref list_lock_IP);
        }
        public void Function_儲位刷新_byIP(string IP, ref List<string> list_lock_IP)
        {
            object device = Fucnction_從本地資料取得儲位(IP);
            List<Task> taskList = new List<Task>();
            List<string> list_IP = new List<string>();
            List<string> list_IP_buf = new List<string>();

    

            if (device != null)
            {
                if (device is Storage)
                {
                    Storage storage = device as Storage;
                    if (storage != null)
                    {
                        taskList.Add(Task.Run(() =>
                        {
                            storage = List_EPD266_雲端資料.SortByIP(IP);
                            if (storage.DeviceType == DeviceType.Pannel35 || storage.DeviceType == DeviceType.Pannel35_lock)
                            {
                                if (!plC_CheckBox_測試模式.Checked)
                                {
                                    this.storageUI_WT32.Set_DrawPannelJEPG(storage);
                                }
                            }
                            else
                            {
                                this.storageUI_EPD_266.DrawToEpd_UDP(storage);
                            }
                        
                        }));

                        list_IP.Add(IP);
                    }
                }
                else if (device is Drawer)
                {
                    Drawer drawer = device as Drawer;
                    if (drawer != null)
                    {
                        taskList.Add(Task.Run(() =>
                        {
                            drawer = List_EPD583_雲端資料.SortByIP(IP);
                            this.drawerUI_EPD_583.DrawToEpd_UDP(drawer);
                        }));
                        list_IP.Add(IP);
                    }
                }
            

                Task.WhenAll(taskList).Wait();
            }

        }

        public string Function_取得藥品網址(string 藥品碼)
        {
            if (藥品碼.Length < 5) 藥品碼 = "0" + 藥品碼;
            string URL = $@"{myConfigClass.藥物辨識網址}{藥品碼}.jpg";
            return string.Format(URL, 藥品碼);
        }
        public void Function_顯示藥物辨識圖片(string 藥品碼, PictureBox pictureBox)
        {
            if (myConfigClass.藥物辨識網址.StringIsEmpty() == false)
            {
                this.Invoke(new Action(delegate
                {
                    string URL = this.Function_取得藥品網址(藥品碼);
                    Basic.Net.DowloadToPictureBox(URL, pictureBox);
                }));

            }
        }
        static public string Function_藥品碼檢查(string Code)
        {

            return Code;
        }


        public static void Function_抽屜以藥品碼解鎖(string Code)
        {
            List<Task> tasks = new List<Task>();
            List<string> list_IP = new List<string>();
            List<Storage> storages_epd266 = List_EPD266_本地資料.SortByCode(Code);
            List<Storage> storages_WT32 = List_Pannel35_本地資料.SortByCode(Code);
            List<Box> Boxes_EPD583 = List_EPD583_雲端資料.SortByCode(Code);
            for (int i = 0; i < storages_epd266.Count; i++)
            {
                list_IP.Add(storages_epd266[i].IP);
            }
            for (int i = 0; i < storages_WT32.Count; i++)
            {
                list_IP.Add(storages_WT32[i].IP);
            }
            for (int i = 0; i < Boxes_EPD583.Count; i++)
            {
                list_IP.Add(Boxes_EPD583[i].IP);
            }
            list_IP = (from temp in list_IP
                       select temp).Distinct().ToList();

            Function_抽屜解鎖(list_IP);

        }
        public static List<string> Function_取得抽屜以藥品碼解鎖IP(string Code)
        {
            List<Task> tasks = new List<Task>();
            List<string> list_IP = new List<string>();
            List<Storage> storages_epd266 = List_EPD266_本地資料.SortByCode(Code);
            List<Storage> storages_WT32 = List_Pannel35_本地資料.SortByCode(Code);
            List<Box> Boxes_EPD583 = List_EPD583_雲端資料.SortByCode(Code);
            for (int i = 0; i < storages_epd266.Count; i++)
            {
                list_IP.Add(storages_epd266[i].IP);
            }
            for (int i = 0; i < storages_WT32.Count; i++)
            {
                list_IP.Add(storages_WT32[i].IP);
            }
            for (int i = 0; i < Boxes_EPD583.Count; i++)
            {
                list_IP.Add(Boxes_EPD583[i].IP);
            }
            list_IP = (from temp in list_IP
                       select temp).Distinct().ToList();

            return list_IP;
        }
        public static void Function_抽屜解鎖(List<string> list_IP)
        {
            List<object[]> list_locker_table_value = _sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(false);
            List<object[]> list_locker_table_value_buf = new List<object[]>();
            List<object[]> list_locker_table_value_replace = new List<object[]>();
            for (int i = 0; i < list_IP.Count; i++)
            {
                string IP = list_IP[i];

                list_locker_table_value_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.IP, IP);
                if (list_locker_table_value_buf.Count > 0)
                {
                    list_locker_table_value_buf[0][(int)enum_lockerIndex.Master_GUID] = "";
                    list_locker_table_value_buf[0][(int)enum_lockerIndex.輸出狀態] = true.ToString();

                    list_locker_table_value_replace.Add(list_locker_table_value_buf[0]);

                }
            }
            if (list_locker_table_value_replace.Count > 0) _sqL_DataGridView_Locker_Index_Table.SQL_ReplaceExtra(list_locker_table_value_replace, false);

        }
        public static void Function_外門片解鎖()
        {
            List<object[]> list_locker_table_value = _sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(false);
            List<string> list_IP = (from temp in list_locker_table_value
                                    select temp[(int)enum_lockerIndex.IP].ObjectToString()).ToList();
            Function_外門片解鎖(list_IP);
        }
        public static void Function_外門片解鎖(List<string> list_IP)
        {
            List<object[]> list_locker_table_value = _sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(false);
            List<object[]> list_locker_table_value_buf = new List<object[]>();
            List<object[]> list_locker_table_value_外門片_buf = new List<object[]>();
            List<object[]> list_locker_table_value_replace = new List<object[]>();
            for (int i = 0; i < list_IP.Count; i++)
            {
                string IP = list_IP[i];

                list_locker_table_value_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.IP, IP);
                if (list_locker_table_value_buf.Count > 0)
                {
                    string 同步輸出 = list_locker_table_value_buf[0][(int)enum_lockerIndex.同步輸出].ObjectToString();
                    if (同步輸出.StringIsEmpty()) continue;
                    list_locker_table_value_外門片_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.輸出位置, 同步輸出);
                    if (list_locker_table_value_外門片_buf.Count > 0)
                    {
                        list_locker_table_value_外門片_buf[0][(int)enum_lockerIndex.Master_GUID] = "";
                        list_locker_table_value_外門片_buf[0][(int)enum_lockerIndex.輸出狀態] = true.ToString();

                        list_locker_table_value_replace.Add(list_locker_table_value_外門片_buf[0]);
                    }



                }
            }
            if (list_locker_table_value_replace.Count > 0) _sqL_DataGridView_Locker_Index_Table.SQL_ReplaceExtra(list_locker_table_value_replace, false);

        }

        public static bool Function_檢查是否完成交班()
        {
            if(PLC_Device_未交班無法調劑.Bool == false) return true;
            string nowTime = DateTime.Now.ToDateTimeString_6();
            shiftClass shiftClass = shiftClass.get_shift_name_by_name(Main_Form.API_Server, nowTime);

            // 解析時間字串
            TimeSpan 開始時間 = TimeSpan.Parse(shiftClass.開始時間);
            TimeSpan 結束時間 = TimeSpan.Parse(shiftClass.結束時間);

            // 當前時間資訊
            DateTime today = DateTime.Now.Date;
            TimeSpan 現在時間 = DateTime.Now.TimeOfDay;

            DateTime startTime;
            DateTime endTime;

            // 判斷是否為跨日班別（例如大夜班 22:00 ~ 07:59）
            bool 是跨日班別 = 開始時間 > 結束時間;

            if (是跨日班別)
            {
                // 跨日處理邏輯
                if (現在時間 <= 結束時間)
                {
                    // 屬於昨天晚上的大夜班
                    startTime = today.AddDays(-1).Add(開始時間);
                    endTime = today.Add(結束時間);
                }
                else
                {
                    // 屬於今天晚上的大夜班
                    startTime = today.Add(開始時間);
                    endTime = today.AddDays(1).Add(結束時間);
                }
            }
            else
            {
                // 非跨日班別（如早班、小夜班）
                startTime = today.Add(開始時間);
                endTime = today.Add(結束時間);
            }




            List<transactionsClass> transactionsClasses = transactionsClass.get_datas_by_rx_time_st_end(Main_Form.API_Server, startTime, endTime, Main_Form.ServerName, Main_Form.ServerType);
            transactionsClasses = (from temp in transactionsClasses
                                   where temp.備註 == "交班盤點完成"
                                   select temp).ToList();
            return transactionsClasses.Count > 0;
        }
        public static byte[] Function_ReadBacodeScanner_pre(int index)
        {
            if (index == 0) return Main_Form.MySerialPort_Scanner01.ReadByte();
            if (index == 1) return Main_Form.MySerialPort_Scanner02.ReadByte();
            if (index == 2) return Main_Form.MySerialPort_Scanner03.ReadByte();
            if (index == 3) return Main_Form.MySerialPort_Scanner04.ReadByte();
            return null;
        }
        public static string Function_ReadBacodeScanner(int index)
        {
            if(index == 0) return Function_ReadBacodeScanner01();
            if (index == 1) return Function_ReadBacodeScanner02();
            if (index == 2) return Function_ReadBacodeScanner03();
            if (index == 3) return Function_ReadBacodeScanner04();
            return null;
        }
        public static string Function_ReadBacodeScanner01()
        {
            try
            {
                if (MySerialPort_Scanner01.IsConnected == false && myConfigClass.鍵盤掃碼模式 == false) return null;
                System.Threading.Thread.Sleep(200);
                string text = MySerialPort_Scanner01.ReadString();
                if (text == null) return null;       
                text = text.Replace("\0", "");
                if (text.StringIsEmpty()) return null;
                if (text.Length <= 2 || text.Length > 200) return null;
                //if (text.Substring(text.Length - 2, 2) != "\r\n") return null;
                MySerialPort_Scanner01.ClearReadByte();
                text = text.Replace("\r\n", "");
                return text;
            }
            catch(Exception ex)
            {
                Logger.Log("error", $"Function_ReadBacodeScanner01 : {ex.Message}");
                return null;
            }
          
        }
        public static string Function_ReadBacodeScanner02()
        {
            try
            {
                if (MySerialPort_Scanner02.IsConnected == false) return null;

                string text = MySerialPort_Scanner02.ReadString();
                if (text == null) return null;
                System.Threading.Thread.Sleep(200);
                text = MySerialPort_Scanner02.ReadString();
                      if (text == null) return null;       
                text = text.Replace("\0", "");
                if (text.StringIsEmpty()) return null;
                if (text.Length <= 2 || text.Length > 200) return null;
                //if (text.Substring(text.Length - 2, 2) != "\r\n") return null;
                MySerialPort_Scanner02.ClearReadByte();
                text = text.Replace("\r\n", "");
                return text;
            }
            catch (Exception ex)
            {
                Logger.Log("error", $"Function_ReadBacodeScanner01 : {ex.Message}");
                return null;
            }
           
        }
        public static string Function_ReadBacodeScanner03()
        {
            try
            {
                if (MySerialPort_Scanner03.IsConnected == false) return null;

                string text = MySerialPort_Scanner03.ReadString();
                if (text == null) return null;
                System.Threading.Thread.Sleep(200);
                text = MySerialPort_Scanner03.ReadString();
                text = text.Replace("\0", "");
                if (text.StringIsEmpty()) return null;
                if (text.Length <= 2 || text.Length > 200) return null;
                //if (text.Substring(text.Length - 2, 2) != "\r\n") return null;
                MySerialPort_Scanner03.ClearReadByte();
                text = text.Replace("\r\n", "");
                return text;
            }
            catch (Exception ex)
            {
                Logger.Log("error", $"Function_ReadBacodeScanner01 : {ex.Message}");
                return null;
            }
            
        }
        public static string Function_ReadBacodeScanner04()
        {
            try
            {
                if (MySerialPort_Scanner04.IsConnected == false) return null;

                string text = MySerialPort_Scanner04.ReadString();
                if (text == null) return null;
                System.Threading.Thread.Sleep(200);
                text = MySerialPort_Scanner04.ReadString();
                text = text.Replace("\0", "");
                if (text.StringIsEmpty()) return null;
                if (text.Length <= 2 || text.Length > 200) return null;
                //if (text.Substring(text.Length - 2, 2) != "\r\n") return null;
                MySerialPort_Scanner04.ClearReadByte();
                text = text.Replace("\r\n", "");
                return text;
            }
            catch (Exception ex)
            {
                Logger.Log("error", $"Function_ReadBacodeScanner01 : {ex.Message}");
                return null;
            }
       
        }

        public static string[] Function_ReadBacodeScanner()
        {
            string[] strs = new string[4];
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(new Action(delegate
            {
                strs[0] = Function_ReadBacodeScanner01();
            })));
            tasks.Add(Task.Run(new Action(delegate
            {
                strs[1] = Function_ReadBacodeScanner02();
            })));
            tasks.Add(Task.Run(new Action(delegate
            {
                strs[2] = Function_ReadBacodeScanner03();
            })));
            tasks.Add(Task.Run(new Action(delegate
            {
                strs[3] = Function_ReadBacodeScanner04();
            })));
            Task.WhenAll(tasks).Wait();

            return strs;
        }
        public static string RemoveParenthesesContent(string input)
        {
            // 使用正則表達式替換括號及其內部的內容
            return System.Text.RegularExpressions.Regex.Replace(input, @"\s*\(.*\)", ""); ;
        }



        public static StockClass convert_note(string 備註)
        {
            if (string.IsNullOrEmpty(備註)) return null;

            StockClass stockClass = null;
          
            備註 = 備註.Replace("[NEW處方]", "");
            備註 = 備註.Replace("[DC處方]", "");
            string[] temp_ary = 備註.Split('\n');
            for (int k = 0; k < temp_ary.Length; k++)
            {
                string 效期 = temp_ary[k].GetTextValue("效期");
                string 批號 = temp_ary[k].GetTextValue("批號");
                string 數量 = temp_ary[k].GetTextValue("數量");

                if (效期.StringIsEmpty() == true) continue;

                stockClass = new StockClass();
                stockClass.Lot_number = 批號;
                stockClass.Validity_period = 效期;
                stockClass.Qty = 數量;


            }
            return stockClass;
        }
    }


}
