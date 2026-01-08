using Basic;
using H_Pannel_lib;
using HIS_DB_Lib;
using MyUI;
using SQLUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace batch_StackDataAccounting
{
   
    public enum enum_LCD114_索引表
    {
        [Description("GUID,VARCHAR,50,PRIKEY")]
        GUID,
        [Description("IP,VARCHAR,50,INDEX")]
        IP,
        [Description("index_IP,VARCHAR,50,INDEX")]
        index_IP,
    }
    static public class CommonSapceMethod
    {
        public static void WriteTakeMedicineStack(this List<CommonSapceClass> commonSapceClasses, List<object[]> list_堆疊母資料_add)
        {
            Table table = new Table(new enum_取藥堆疊母資料());
            SQLControl sQLControl = new SQLControl();
            for (int i = 0; i < commonSapceClasses.Count; i++)
            {
                table.Server = commonSapceClasses[i].sys_serverSettingClass.Server;
                table.Username = commonSapceClasses[i].sys_serverSettingClass.User;
                table.Password = commonSapceClasses[i].sys_serverSettingClass.Password;
                table.Port = commonSapceClasses[i].sys_serverSettingClass.Port;
                table.DBName = commonSapceClasses[i].sys_serverSettingClass.DBName;
                sQLControl.Init(table);
                List<string> list_str = (from temp in list_堆疊母資料_add
                                         select temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString()).ToList();
                for (int k = 0; k < list_str.Count; k++)
                {
                    Console.WriteLine($"刪除共用台資料,名稱 : {list_str[k]}");
                    sQLControl.DeleteByDefult(null, (int)enum_取藥堆疊母資料.調劑台名稱, list_str[k]);
                }
                for (int k = 0; k < list_堆疊母資料_add.Count; k++)
                {
                    if (list_堆疊母資料_add[k][(int)enum_取藥堆疊母資料.動作].ObjectToString() == "系統領藥") list_堆疊母資料_add[k][(int)enum_取藥堆疊母資料.動作] = "掃碼領藥";
                }
                sQLControl.AddRows(null, list_堆疊母資料_add);
                Console.WriteLine($"{commonSapceClasses[i]} 新增共用台資料,共<{list_堆疊母資料_add.Count}>筆");
            }
        }
    }
    public class CommonSapceClass
    {
        public override string ToString()
        {
            return ($"{DateTime.Now.ToDateTimeString()} - ({sys_serverSettingClass.設備名稱}) EPD583<{List_EPD583.Count}>,EPD266<{List_EPD266.Count}>,RowsLED<{List_RowsLED.Count}>");
        }
        public sys_serverSettingClass sys_serverSettingClass = new sys_serverSettingClass();
        public List<Storage> List_EPD266 = new List<Storage>();
        public List<Drawer> List_EPD583 = new List<Drawer>();
        public List<RowsLED> List_RowsLED = new List<RowsLED>();


        public CommonSapceClass(sys_serverSettingClass sys_serverSettingClass)
        {
            this.sys_serverSettingClass = sys_serverSettingClass;
        }

        public void WriteTakeMedicineStack(List<takeMedicineStackClass> takeMedicineStackClasses)
        {
            takeMedicineStackClass.set_device_tradding(Program.API_Server, sys_serverSettingClass.設備名稱, Program.ServerType, takeMedicineStackClasses);
        }
        public static Drawer GetEPD583(string IP, ref List<CommonSapceClass> commonSapceClasses)
        {
            for (int i = 0; i < commonSapceClasses.Count; i++)
            {
                Drawer drawer = commonSapceClasses[i].List_EPD583.SortByIP(IP);
                if (drawer != null) return drawer;
            }
            return null;
        }
        public static RowsLED GetRowsLED(string IP, ref List<CommonSapceClass> commonSapceClasses)
        {
            for (int i = 0; i < commonSapceClasses.Count; i++)
            {
                RowsLED rowsLED = commonSapceClasses[i].List_RowsLED.SortByIP(IP);
                if (rowsLED != null) return rowsLED;
            }
            return null;
        }
        public static Storage GetEPD266(string IP, ref List<CommonSapceClass> commonSapceClasses)
        {
            for (int i = 0; i < commonSapceClasses.Count; i++)
            {
                Storage storage = commonSapceClasses[i].List_EPD266.SortByIP(IP);
                if (storage != null) return storage;
            }
            return null;
        }
        public void Load()
        {
            string IP = sys_serverSettingClass.Server;
            string DataBaseName = sys_serverSettingClass.DBName;
            string UserName = sys_serverSettingClass.User;
            string Password = sys_serverSettingClass.Password;
            uint Port = sys_serverSettingClass.Port.StringToUInt32();
            SQLControl sQLControl_EPD583_serialize = new SQLControl(IP, DataBaseName, "epd583_jsonstring", UserName, Password, Port, MySql.Data.MySqlClient.MySqlSslMode.None);
            SQLControl sQLControl_EPD266_serialize = new SQLControl(IP, DataBaseName, "epd266_jsonstring", UserName, Password, Port, MySql.Data.MySqlClient.MySqlSslMode.None);
            SQLControl sQLControl_RowsLED_serialize = new SQLControl(IP, DataBaseName, "rowsled_jsonstring", UserName, Password, Port, MySql.Data.MySqlClient.MySqlSslMode.None);
            List<object[]> list_EPD583 = sQLControl_EPD583_serialize.GetAllRows(null);
            List<object[]> list_EPD266 = sQLControl_EPD266_serialize.GetAllRows(null);
            List<object[]> list_RowsLED = sQLControl_RowsLED_serialize.GetAllRows(null);
            List_EPD583 = H_Pannel_lib.DrawerMethod.SQL_GetAllDrawers(list_EPD583);
            List_EPD266 = H_Pannel_lib.StorageMethod.SQL_GetAllStorage(list_EPD266);
            List_RowsLED = H_Pannel_lib.RowsLEDMethod.SQL_GetAllRowsLED(list_RowsLED);

            Console.WriteLine($"{DateTime.Now.ToDateTimeString()} - ({sys_serverSettingClass.設備名稱}) EPD583<{list_EPD583.Count}>,EPD266<{List_EPD266.Count}>,RowsLED<{List_RowsLED.Count}>");
        }
    }

    class Program
    {
        static public List<CommonSapceClass> commonSapceClasses = new List<CommonSapceClass>();
        private static List<medClass> medClasses_cloud_global = null;

        static public List<object> Function_從共用區取得儲位(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            for (int m = 0; m < commonSapceClasses.Count; m++)
            {
                List<Box> boxes = commonSapceClasses[m].List_EPD583.SortByCode(藥品碼);
                List<Storage> storages = commonSapceClasses[m].List_EPD266.SortByCode(藥品碼);
                List<RowsDevice> rowsDevices = commonSapceClasses[m].List_RowsLED.SortByCode(藥品碼);
                for (int i = 0; i < boxes.Count; i++)
                {
                    list_value.Add(boxes[i]);
                }

                for (int i = 0; i < storages.Count; i++)
                {
                    list_value.Add(storages[i]);
                }

                for (int i = 0; i < rowsDevices.Count; i++)
                {
                    list_value.Add(rowsDevices[i]);
                }

            }



            return list_value;
        }
        static public int Function_從共用區取得庫存(string 藥品碼)
        {
            int 庫存 = 0;
            List<object> list_value = Function_從共用區取得儲位(藥品碼);
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    Device device = (Device)list_value[i];
                    if (device != null)
                    {
                        庫存 += device.Inventory.StringToInt32();
                    }
                }
            }
            if (list_value.Count == 0) return -999;
            return 庫存;
        }


        static public List<CommonSapceClass> Function_取得共用區所有儲位()
        {
            List<CommonSapceClass> commonSapceClasses = new List<CommonSapceClass>();
            List<HIS_DB_Lib.sys_serverSettingClass> sys_serverSettingClasses = Function_取得共用區連線資訊();

            for (int i = 0; i < sys_serverSettingClasses.Count; i++)
            {
                CommonSapceClass commonSapceClass = new CommonSapceClass(sys_serverSettingClasses[i]);
                commonSapceClass.Load();
                commonSapceClasses.Add(commonSapceClass);
            }


            return commonSapceClasses;
        }
        static private List<HIS_DB_Lib.sys_serverSettingClass> Function_取得共用區連線資訊()
        {
            List<object[]> list_value = sQLControl_共用區設定.GetAllRows(null);

            list_value = (from temp in list_value
                          where temp[(int)enum_commonSpaceSetup.是否共用].ObjectToString().ToUpper() == true.ToString().ToUpper()
                          select temp).ToList();

            string json_result = Basic.Net.WEBApiGet($"{dBConfigClass.Api_Server}/api/ServerSetting");
            if (json_result.StringIsEmpty())
            {
                Console.WriteLine($"API 連結失敗 : {dBConfigClass.Api_Server}/api/ServerSetting");
                return new List<sys_serverSettingClass>();
            }
            Console.WriteLine(json_result);
            returnData returnData = json_result.JsonDeserializet<returnData>();
            List<HIS_DB_Lib.sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();
            List<HIS_DB_Lib.sys_serverSettingClass> sys_serverSettingClasses_buf = new List<sys_serverSettingClass>();
            List<HIS_DB_Lib.sys_serverSettingClass> sys_serverSettingClasses_return = new List<sys_serverSettingClass>();
            for (int i = 0; i < list_value.Count; i++)
            {
                string 名稱 = list_value[i][(int)enum_commonSpaceSetup.共用區名稱].ObjectToString();
                sys_serverSettingClasses_buf = (from temp in sys_serverSettingClasses
                                                where temp.設備名稱 == 名稱
                                                where temp.類別 == "調劑台"
                                                where temp.內容 == "儲位資料"
                                                select temp).ToList();

                if (sys_serverSettingClasses_buf.Count > 0)
                {
                    sys_serverSettingClasses_return.Add(sys_serverSettingClasses_buf[0]);
                }
            }
            return sys_serverSettingClasses_return;

        }


        private static System.Threading.Mutex mutex;

        static public string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static private string DBConfigFileName = $"{currentDirectory}//DBConfig.txt";
        public class DBConfigClass
        {
            private string name = "";
            private string api_Server = "http://127.0.0.1:4433";
            public string Api_Server { get => api_Server; set => api_Server = value; }
            public string Name { get => name; set => name = value; }
        }
        static DBConfigClass dBConfigClass = new DBConfigClass();

        static public bool LoadDBConfig()
        {
            string jsonstr = MyFileStream.LoadFileAllTextAuto($"{DBConfigFileName}");
            Console.WriteLine($"路徑 : {DBConfigFileName} 開始讀取");
            Console.WriteLine($"-------------------------------------------------------------------------");
            if (jsonstr.StringIsEmpty())
            {
                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(new DBConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{DBConfigFileName}", list_jsonstring))
                {
                    Console.WriteLine($"建立{DBConfigFileName}檔案失敗!");
                    return false;
                }
                Console.WriteLine($"未建立參數文件!請至子目錄設定{DBConfigFileName}");
                return false;
            }
            else
            {
                dBConfigClass = Basic.Net.JsonDeserializet<DBConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(dBConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                //if (!MyFileStream.SaveFile($"{DBConfigFileName}", list_jsonstring))
                //{
                //    Console.WriteLine($"建立{DBConfigFileName}檔案失敗!");
                //    return false;
                //}

            }
            return true;

        }
        public enum enum_儲位資訊
        {
            IP,
            TYPE,
            效期,
            批號,
            庫存,
            異動量,
            Value,
        }
        static public double lightness = 0.5;
        public class LightOn
        {
            public LightOn(string 藥碼, Color color, double qty)
            {
                藥品碼 = 藥碼;
                顏色 = color;
                數量 = qty;
                LCD_Color = color;
                flag_Refresh_LCD = true;
            }
            public LightOn(string 藥碼, Color color)
            {
                藥品碼 = 藥碼;
                顏色 = color;
                LCD_Color = color;
                數量 = 0;
            }
            public LightOn()
            {

            }
            public LightOn Copy()
            {
                return Copy(this);
            }
            public LightOn Copy(LightOn lightOn)
            {
                LightOn lightOn_buf = new LightOn();

                lightOn_buf.藥品碼 = lightOn.藥品碼;
                lightOn_buf.顏色 = lightOn.顏色;
                lightOn_buf.數量 = lightOn.數量;
                lightOn_buf.LCD_Color = lightOn.LCD_Color;
                lightOn_buf.flag_Refresh_LCD = lightOn.flag_Refresh_LCD;
                lightOn_buf.flag_Refresh_Light = lightOn.flag_Refresh_Light;

                return lightOn_buf;
            }
            public string IP { get; set; }
            public string 藥品碼 { get; set; }
            public Color 顏色 { get; set; }
            public Color LCD_Color { get; set; }
            public double 數量 { get; set; }
            public bool flag_Refresh_LCD = false;
            public bool flag_Refresh_Light = false;
        }
        public class MyColor
        {
            public int R = 0;
            public int G = 0;
            public int B = 0;
            public MyColor(int r, int g, int b)
            {
                R = r;
                G = g;
                B = b;
            }


            public bool IsEqual(int r, int g, int b)
            {
                return (R == r && G == g && B == b);
            }
        }
        #region MyConfigClass
        private static string MyConfigFileName = $@"{currentDirectory}\MyConfig.txt";
        static public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {


            private int ePD583_Port = 29005;
            private int ePD266_Port = 29000;
            private int ePD1020_Port = 29012;
            private int rowsLED_Port = 29001;
            private int pannel35_Port = 29020;
            private int lCD114_Port = 29008;
            private string voice_IP = "192.168.5.241";
            private int voice_port = 5200;

            public int EPD583_Port { get => ePD583_Port; set => ePD583_Port = value; }
            public int EPD266_Port { get => ePD266_Port; set => ePD266_Port = value; }
            public int EPD1020_Port { get => ePD1020_Port; set => ePD1020_Port = value; }
            public int RowsLED_Port { get => rowsLED_Port; set => rowsLED_Port = value; }
            public int Pannel35_Port { get => pannel35_Port; set => pannel35_Port = value; }
            public int LCD114_Port { get => lCD114_Port; set => lCD114_Port = value; }
            public string Voice_IP { get => voice_IP; set => voice_IP = value; }
            public int Voice_port { get => voice_port; set => voice_port = value; }
            public bool 舊版晶片 { get; set; } = false;
        }
        static private void LoadMyConfig()
        {
            string jsonstr = MyFileStream.LoadFileAllTextAuto($"{MyConfigFileName}");
            if (jsonstr.StringIsEmpty())
            {
                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(new MyConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                }
                MyMessageBox.ShowDialog($"未建立參數文件!請至子目錄設定{MyConfigFileName}");
            }
            else
            {
                myConfigClass = Basic.Net.JsonDeserializet<MyConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(myConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                //if (!MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring))
                //{
                //    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                //}

            }

        }
        #endregion
        static int 處方存在時間 = 120000;
        static private List<LightOn> lightOns = new List<LightOn>();


        static private int cnt_儲位亮燈 = 0;
        static private int 儲位亮燈數量_temp = 0;
        static private MyTimerBasic MyTimerBasic_儲位亮燈 = new MyTimerBasic();
        static private void sub_Program_取藥堆疊資料_儲位亮燈()
        {
            if (lightOns.Count > 0)
            {
                lock (lightOns)
                {
                    try
                    {
                        if (cnt_儲位亮燈 == 0)
                        {
                            儲位亮燈數量_temp = lightOns.Count;
                            MyTimerBasic_儲位亮燈.TickStop();
                            MyTimerBasic_儲位亮燈.StartTickTime(20);
                            cnt_儲位亮燈++;
                        }
                        if (cnt_儲位亮燈 == 1)
                        {
                            if (MyTimerBasic_儲位亮燈.IsTimeOut())
                            {
                                if (儲位亮燈數量_temp == lightOns.Count)
                                {
                                    cnt_儲位亮燈++;
                                }
                                else
                                {
                                    cnt_儲位亮燈 = 0;
                                }
                            }
                        }
                        if (cnt_儲位亮燈 == 2)
                        {
                            List<LightOn> lightOns_buf = new List<LightOn>();
                            for (int i = 0; i < lightOns.Count; i++)
                            {
                                LightOn lightOn = lightOns[i].Copy();

                                lightOns_buf.Add(lightOn);
                            }
                            lightOns.Clear();

                            List<Task> taskList_抽屜層架 = new List<Task>();

                            List<string> list_抽屜亮燈_IP = new List<string>();
                            List<string> list_層架亮燈_IP = new List<string>();

                            for (int i = 0; i < lightOns_buf.Count; i++)
                            {
                                string 藥品碼 = lightOns_buf[i].藥品碼;
                                Color 顏色 = lightOns_buf[i].顏色;
                                list_抽屜亮燈_IP.LockAdd(Function_儲位亮燈_取得抽屜亮燈IP(lightOns_buf[i]));
                                list_層架亮燈_IP.LockAdd(Function_儲位亮燈_取得層架亮燈IP(藥品碼, 顏色));
                            }

                            taskList_抽屜層架.Add(Task.Run(() =>
                            {
                                Function_儲位亮燈_抽屜亮燈(list_抽屜亮燈_IP);
                            }));
                            taskList_抽屜層架.Add(Task.Run(() =>
                            {
                                Function_儲位亮燈_層架亮燈(list_層架亮燈_IP);
                            }));


                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < lightOns_buf.Count; i++)
                            {
                                LightOn lightOn = lightOns_buf[i];

                                taskList.Add(Task.Run(() =>
                                {
                                    Function_儲位亮燈_Ex(lightOn);

                                }));
                            }
                            Task.WhenAll(taskList).Wait();
                            Task.WhenAll(taskList_抽屜層架).Wait();
                            cnt_儲位亮燈++;
                        }
                        if (cnt_儲位亮燈 == 3)
                        {
                            cnt_儲位亮燈 = 0;
                        }


                    }
                    catch
                    {

                    }
                    finally
                    {

                    }


                }
            }

        }
        static private List<string> Function_儲位亮燈_取得抽屜亮燈IP(LightOn lightOn)
        {
            string 藥品碼 = lightOn.藥品碼;
            Color color = lightOn.顏色;
            if (藥品碼.StringIsEmpty()) return new List<string>();
            List<object> list_Device = Function_從本地資料取得儲位(藥品碼);
            //List<object> list_commonSpace_device = Function_從共用區取得儲位(藥品碼);
            //for (int i = 0; i < list_commonSpace_device.Count; i++)
            //{
            //    list_Device.Add(list_commonSpace_device[i]);
            //}
            bool flag_led_refresh = true;
            List<string> list_IP = new List<string>();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;

                if (device == null) continue;

                if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock
                    || device.DeviceType == DeviceType.EPD730E || device.DeviceType == DeviceType.EPD730E_lock)
                {
                    Box box = list_Device[i] as Box;
                    if (box != null)
                    {
                        bool flag_common_device = false;
                        Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
                        int numOfLED = 400;
                        if (drawer.DrawerType == Drawer.Enum_DrawerType._4X8 || drawer.DrawerType == Drawer.Enum_DrawerType._3X8) numOfLED = 400;
                        if (drawer.DrawerType == Drawer.Enum_DrawerType._4X8_A || drawer.DrawerType == Drawer.Enum_DrawerType._5X8_A) numOfLED = 365;
                        if (drawer == null)
                        {
                            drawer = CommonSapceClass.GetEPD583(IP, ref commonSapceClasses);
                            //drawer.LED_Bytes = this.drawerUI_EPD_583.Get_Drawer_LED_UDP(drawer);

                            flag_common_device = true;
                        }
                        if (drawer == null) continue;
                        List<Box> boxes = drawer.SortByCode(藥品碼);

                        if (drawer.IsAllLight)
                        {
                            byte[] LED_Bytes = new byte[drawer.LED_Bytes.Length];
                            for (int k = 0; k < drawer.LED_Bytes.Length; k++)
                            {
                                LED_Bytes[k] = drawer.LED_Bytes[k];
                            }
                            bool flag_commonlight = false;
                            List<MyColor> myColors = new List<MyColor>();
                            if (color != Color.Black)
                            {
                                for (int k = 0; k < numOfLED; k++)
                                {
                                    int R = LED_Bytes[k * 3 + 0];
                                    int G = LED_Bytes[k * 3 + 1];
                                    int B = LED_Bytes[k * 3 + 2];
                                    if (R == 0 && G == 0 && B == 0) continue;

                                    if (R != color.R || G != color.G || B != color.B)
                                    {
                                        Console.WriteLine($"藥品碼 : {藥品碼} , {color.R},{color.G},{color.B} [{R},{G},{B}]");
                                        flag_commonlight = true;
                                        break;
                                    }
                                }
                            }

                            drawer.LED_Bytes = DrawerUI_EPD_583.Set_LEDBytes(drawer, boxes, color, !lightOn.flag_Refresh_Light);
                            if (!flag_commonlight || lightOn.flag_Refresh_Light)
                            {
                                if (color != Color.Black)
                                {
                                    if (lightOn.flag_Refresh_Light == false) drawer.LED_Bytes = DrawerUI_EPD_583.Set_Pannel_LEDBytes(drawer, color);
                                }
                                else
                                {
                                    bool flag_led_black_enable = true;
                                    for (int k = 0; k < numOfLED; k++)
                                    {
                                        int R = drawer.LED_Bytes[k * 3 + 0];
                                        int G = drawer.LED_Bytes[k * 3 + 1];
                                        int B = drawer.LED_Bytes[k * 3 + 2];
                                        if (R != 0 || G != 0 || B != 0)
                                        {
                                            flag_led_black_enable = false;
                                            break;
                                        }
                                    }
                                    if (flag_led_black_enable || flag_common_device) drawer.LED_Bytes = DrawerUI_EPD_583.Set_Pannel_LEDBytes(drawer, color);

                                }
                            }
                            else drawer.LED_Bytes = DrawerUI_EPD_583.Set_Pannel_LEDBytes(drawer, Color.Purple);


                            flag_led_refresh = true;
                            for (int k = 0; k < drawer.LED_Bytes.Length; k++)
                            {
                                if (LED_Bytes[k] != drawer.LED_Bytes[k])
                                {
                                    flag_led_refresh = true;
                                }
                            }
                        }
                        else
                        {
                            drawer.LED_Bytes = DrawerUI_EPD_583.Set_LEDBytes(drawer, color);
                            flag_led_refresh = true;
                        }
                    }
                    list_IP.Add(IP);
                }

            }
            list_IP = (from temp in list_IP
                       select temp).Distinct().ToList();
            return list_IP;
        }
        static private void Function_儲位亮燈_抽屜亮燈(List<string> list_IP)
        {
            try
            {
                list_IP = (from temp in list_IP
                           select temp).Distinct().ToList();
                Task allTask;
                List<Task> taskList = new List<Task>();
                for (int i = 0; i < list_IP.Count; i++)
                {
                    string IP = list_IP[i];
                    taskList.Add(Task.Run(() =>
                    {
                        Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
                        if (drawer == null)
                        {
                            drawer = CommonSapceClass.GetEPD583(IP, ref commonSapceClasses);
                        }
                        if (drawer == null) return;
                        drawerUI_EPD_583.Set_LED_UDP(drawer);
                    }));

                }
                allTask = Task.WhenAll(taskList);
                allTask.Wait();
            }
            catch
            {

            }
            finally
            {

            }

        }

        static private List<string> Function_儲位亮燈_取得層架亮燈IP(string 藥品碼, Color color)
        {
            if (藥品碼.StringIsEmpty()) return new List<string>();
            List<object> list_Device = Function_從本地資料取得儲位(藥品碼);
            //List<object> list_commonSpace_device = Function_從共用區取得儲位(藥品碼);
            //for (int i = 0; i < list_commonSpace_device.Count; i++)
            //{
            //    list_Device.Add(list_commonSpace_device[i]);
            //}
            bool flag_led_refresh = true;
            List<string> list_IP = new List<string>();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;

                if (device == null) continue;
                if (device.DeviceType == DeviceType.RowsLED)
                {
                    RowsDevice rowsDevice = list_Device[i] as RowsDevice;
                    if (rowsDevice != null)
                    {
                        RowsLED rowsLED = List_RowsLED_本地資料.SortByIP(rowsDevice.IP);
                        if (rowsLED == null)
                        {
                            rowsLED = CommonSapceClass.GetRowsLED(IP, ref commonSapceClasses);
                            //rowsLED.LED_Bytes = this.rowsLEDUI.Get_RowsLED_LED_UDP(rowsLED);
                        }
                        rowsLED.LED_Bytes = RowsLEDUI.Get_Rows_LEDBytes(ref rowsLED.LED_Bytes, rowsDevice, color);
                    }

                    list_IP.Add(IP);
                }



            }
            list_IP = (from temp in list_IP
                       select temp).Distinct().ToList();
            return list_IP;
        }
        static private void Function_儲位亮燈_層架亮燈(List<string> list_IP)
        {
            try
            {
                list_IP = (from temp in list_IP
                           select temp).Distinct().ToList();
                Task allTask;
                List<Task> taskList = new List<Task>();
                for (int i = 0; i < list_IP.Count; i++)
                {
                    string IP = list_IP[i];
                    taskList.Add(Task.Run(() =>
                    {
                        RowsLED rowsLED = List_RowsLED_本地資料.SortByIP(IP);
                        if (rowsLED == null)
                        {
                            rowsLED = CommonSapceClass.GetRowsLED(IP, ref commonSapceClasses);
                        }
                        taskList.Add(Task.Run(() =>
                        {
                            rowsLEDUI.Set_Rows_LED_UDP(rowsLED);
                        }));


                    }));

                }
                allTask = Task.WhenAll(taskList);
                allTask.Wait();
            }
            catch
            {

            }
            finally
            {

            }

        }



        static public StorageUI_EPD_266 storageUI_EPD_266 = new StorageUI_EPD_266();
        static public StorageUI_WT32 storageUI_WT32 = new StorageUI_WT32();
        static public DrawerUI_EPD_583 drawerUI_EPD_583 = new DrawerUI_EPD_583();
        static public StorageUI_LCD_114 storageUI_LCD_114 = new StorageUI_LCD_114();
        static public RowsLEDUI rowsLEDUI = new RowsLEDUI();

        static public List<Drawer> List_EPD583_入賬資料 = new List<Drawer>();
        static public List<Storage> List_EPD266_入賬資料 = new List<Storage>();
        static public List<Storage> List_Pannel35_入賬資料 = new List<Storage>();
        static public List<RowsLED> List_RowsLED_入賬資料 = new List<RowsLED>();

        static public List<Drawer> List_EPD583_本地資料 = new List<Drawer>();
        static public List<Storage> List_EPD266_本地資料 = new List<Storage>();
        static public List<Storage> List_Pannel35_本地資料 = new List<Storage>();
        static public List<RowsLED> List_RowsLED_本地資料 = new List<RowsLED>();

        static public List<Drawer> List_EPD583_雲端資料 = new List<Drawer>();
        static public List<Storage> List_EPD266_雲端資料 = new List<Storage>();
        static public List<Storage> List_Pannel35_雲端資料 = new List<Storage>();
        static public List<RowsLED> List_RowsLED_雲端資料 = new List<RowsLED>();

        static public string API_Server = "http://127.0.0.1:4433";
        static public string ServerName = "A5";
        static public string ServerType = "調劑台";
        static public SQLControl sQLControl_取藥堆疊母資料 = new SQLControl();
        static public SQLControl sQLControl_取藥堆疊子資料 = new SQLControl();
        static public SQLControl sQLControl_醫令資料 = new SQLControl();
        static public SQLControl sQLControl_藥品設定表 = new SQLControl();
        static public SQLControl sQLControl_交易記錄查詢 = new SQLControl();
        static public SQLControl sQLControl_共用區設定 = new SQLControl();
        static public SQLControl sQLControl_Locker_Index_Table = new SQLControl();
        static public SQLControl sQLControl_LCD114_Index = new SQLControl();
        static public List<object[]> list_取藥堆疊母資料 = new List<object[]>();
        static public List<object[]> list_取藥堆疊子資料 = new List<object[]>();
        static bool flag_系統取藥模式 = false;
        static bool flag_同藥碼全亮 = true;
        static MyThread MyThread_取藥堆疊資料_儲位亮燈;
        static public void Main(string[] args)
        {

            LoadDBConfig();
            LoadMyConfig();

            Console.Title = dBConfigClass.Name;

            mutex = new System.Threading.Mutex(true, dBConfigClass.Name);
            if (mutex.WaitOne(0, false))
            {

            }
            else
            {

                return;
            }
            API_Server = dBConfigClass.Api_Server;
            ServerName = dBConfigClass.Name;
            List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{API_Server}/api/serversetting");
            sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses.myFind(ServerName, "調劑台", "一般資料");
            sys_serverSettingClass sys_serverSettingClass_儲位資料 = sys_serverSettingClasses.myFind(ServerName, "調劑台", "儲位資料");
            List<SQLUI.Table> tables = new List<Table>();
            Table table;
            tables = takeMedicineStackClass.init(API_Server, $"{ServerName}", enum_sys_serverSetting_Type.調劑台.GetEnumName());
            sQLControl_取藥堆疊母資料.Init(tables.GetTable(new enum_取藥堆疊母資料()));
            sQLControl_取藥堆疊子資料.Init(tables.GetTable(new enum_取藥堆疊子資料()));
            table = medConfigClass.init(API_Server);
            sQLControl_藥品設定表.Init(table);
            table = transactionsClass.Init(API_Server, ServerName, "調劑台");
            sQLControl_交易記錄查詢.Init(table);
            table = OrderClass.init(API_Server);
            sQLControl_醫令資料.Init(table);

            table = new Table(new enum_lockerIndex());
            table.Server = sys_serverSettingClass.Server;
            table.Username = sys_serverSettingClass.User;
            table.Password = sys_serverSettingClass.Password;
            table.Port = sys_serverSettingClass.Port;
            table.DBName = sys_serverSettingClass.DBName;
            sQLControl_Locker_Index_Table.Init(table);


            table.Server = sys_serverSettingClass.Server;
            table.Port = sys_serverSettingClass.Port;
            table.Username = sys_serverSettingClass.User;
            table.Password = sys_serverSettingClass.Password;
            table.DBName = sys_serverSettingClass.DBName;
            table.TableName = "common_space_setup";
            table.AddColumnList("GUID", Table.StringType.VARCHAR, Table.IndexType.PRIMARY);
            table.AddColumnList("共用區名稱", Table.StringType.VARCHAR, 200, Table.IndexType.None);
            table.AddColumnList("共用區類型", Table.StringType.VARCHAR, Table.IndexType.None);
            table.AddColumnList("是否共用", Table.StringType.VARCHAR, Table.IndexType.None);
            table.AddColumnList("設置時間", Table.DateType.DATETIME, Table.IndexType.None);
            sQLControl_共用區設定.Init(table);

            table = new Table(new enum_LCD114_索引表());
            table.Server = sys_serverSettingClass.Server;
            table.Username = sys_serverSettingClass.User;
            table.Password = sys_serverSettingClass.Password;
            table.Port = sys_serverSettingClass.Port;
            table.DBName = sys_serverSettingClass.DBName;
            table.TableName = "lcd114_index";            
            sQLControl_LCD114_Index.Init(table);

            list_LCD114_索引表 = sQLControl_LCD114_Index.GetAllRows(null);

            Console.WriteLine($"EPD583_Port : {myConfigClass.EPD583_Port} \n");
            Console.WriteLine($"EPD266_Port : {myConfigClass.EPD266_Port} \n");
            Console.WriteLine($"Pannel35_Port : {myConfigClass.Pannel35_Port} \n");
            Console.WriteLine($"RowsLED_Port : {myConfigClass.RowsLED_Port} \n");

            //drawerUI_EPD_583.Console_Init(sys_serverSettingClass_儲位資料.DBName, sys_serverSettingClass_儲位資料.User, sys_serverSettingClass_儲位資料.Password, sys_serverSettingClass_儲位資料.Server, sys_serverSettingClass_儲位資料.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None, myConfigClass.EPD583_Port + 1000, myConfigClass.EPD583_Port);
            //storageUI_EPD_266.Console_Init(sys_serverSettingClass_儲位資料.DBName, sys_serverSettingClass_儲位資料.User, sys_serverSettingClass_儲位資料.Password, sys_serverSettingClass_儲位資料.Server, sys_serverSettingClass_儲位資料.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None, myConfigClass.EPD266_Port + 1000, myConfigClass.EPD266_Port);
            //storageUI_WT32.Console_Init(sys_serverSettingClass_儲位資料.DBName, sys_serverSettingClass_儲位資料.User, sys_serverSettingClass_儲位資料.Password, sys_serverSettingClass_儲位資料.Server, sys_serverSettingClass_儲位資料.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None, myConfigClass.Pannel35_Port + 1000, myConfigClass.Pannel35_Port);
            //rowsLEDUI.Console_Init(sys_serverSettingClass_儲位資料.DBName, sys_serverSettingClass_儲位資料.User, sys_serverSettingClass_儲位資料.Password, sys_serverSettingClass_儲位資料.Server, sys_serverSettingClass_儲位資料.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None, myConfigClass.RowsLED_Port + 1000, myConfigClass.RowsLED_Port);

            drawerUI_EPD_583.Console_Init(sys_serverSettingClass_儲位資料.DBName, sys_serverSettingClass_儲位資料.User, sys_serverSettingClass_儲位資料.Password, sys_serverSettingClass_儲位資料.Server, sys_serverSettingClass_儲位資料.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None, myConfigClass.EPD583_Port, myConfigClass.EPD583_Port,false);
            storageUI_EPD_266.Console_Init(sys_serverSettingClass_儲位資料.DBName, sys_serverSettingClass_儲位資料.User, sys_serverSettingClass_儲位資料.Password, sys_serverSettingClass_儲位資料.Server, sys_serverSettingClass_儲位資料.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None, myConfigClass.EPD266_Port, myConfigClass.EPD266_Port,false);
            storageUI_WT32.Console_Init(sys_serverSettingClass_儲位資料.DBName, sys_serverSettingClass_儲位資料.User, sys_serverSettingClass_儲位資料.Password, sys_serverSettingClass_儲位資料.Server, sys_serverSettingClass_儲位資料.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None, myConfigClass.Pannel35_Port, myConfigClass.Pannel35_Port,false);
            rowsLEDUI.Console_Init(sys_serverSettingClass_儲位資料.DBName, sys_serverSettingClass_儲位資料.User, sys_serverSettingClass_儲位資料.Password, sys_serverSettingClass_儲位資料.Server, sys_serverSettingClass_儲位資料.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None, myConfigClass.RowsLED_Port, myConfigClass.RowsLED_Port,false);
            storageUI_LCD_114.Console_Init(sys_serverSettingClass_儲位資料.DBName, sys_serverSettingClass_儲位資料.User, sys_serverSettingClass_儲位資料.Password, sys_serverSettingClass_儲位資料.Server, sys_serverSettingClass_儲位資料.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None, myConfigClass.LCD114_Port, myConfigClass.LCD114_Port, false);
            Function_從SQL取得儲位到本地資料();
            Function_從SQL取得儲位到雲端資料();
            commonSapceClasses = Program.Function_取得共用區所有儲位();
            MyThread_取藥堆疊資料_儲位亮燈 = new MyThread();
            MyThread_取藥堆疊資料_儲位亮燈.AutoRun(true);
            MyThread_取藥堆疊資料_儲位亮燈.AutoStop(true);
            MyThread_取藥堆疊資料_儲位亮燈.Add_Method(sub_Program_取藥堆疊資料_儲位亮燈);
            MyThread_取藥堆疊資料_儲位亮燈.SetSleepTime(50);
            MyThread_取藥堆疊資料_儲位亮燈.Trigger();

            List<object[]> list_value = sQLControl_取藥堆疊母資料.GetAllRows(null);
            for (int i = 0; i < list_value.Count; i++)
            {
                sQLControl_取藥堆疊母資料.DeleteExtra(null, list_value);
                string 藥品碼 = list_value[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                Function_儲位亮燈(new LightOn(藥品碼, Color.Black));

            }
            Task.Run(new Action(delegate
            {
                while (true)
                {
                    sub_Program_取藥堆疊資料_檢查資料();
                    sub_Program_取藥堆疊資料_狀態更新();
                    sub_Program_取藥堆疊資料_流程作業檢查();
                    sub_Program_取藥堆疊資料_入賬檢查();
                    //Function_從SQL取得儲位到本地資料();
                    System.Threading.Thread.Sleep(1);
                }

            }));
            Task.Run(new Action(delegate
            {
                while (true)
                {
                    var originalConsole = Console.Out;
                    Console.SetOut(TextWriter.Null); // 關掉輸出

                    try
                    {
                        Function_從SQL取得儲位到本地資料();
                        Function_從SQL取得儲位到雲端資料();
                    }
                    finally
                    {
                        Console.SetOut(originalConsole); // 還原輸出
                    }

                    System.Threading.Thread.Sleep(60000);
                }
            }));
            Task.Run(new Action(delegate
            {
                while (true)
                {
                    Function_從SQL取得儲位到本地資料();
                    Function_從SQL取得儲位到雲端資料();
                    System.Threading.Thread.Sleep(60000);
                }

            }));
            while (true)
            {

                for (int i = 0; i < List_EPD583_本地資料.Count; i++)
                {
                    Drawer drawer = List_EPD583_本地資料[i];
                    string json = drawerUI_EPD_583.Get_JsonStrin(drawer.IP , myConfigClass.EPD583_Port);
                    if (json.StringIsEmpty()) continue;
                    DrawerUI_EPD_583.UDP_READ uDP_READ = json.JsonDeserializet<DrawerUI_EPD_583.UDP_READ>();
                    if (uDP_READ == null) continue;
                    bool flag_input = uDP_READ.Get_Input(0);
                    if (drawer.input != flag_input)
                    {
                        if (flag_input)
                        {
                            Console.WriteLine($"抽屜[{drawer.IP}]關閉");
                            drawer.LED_Bytes = DrawerUI_EPD_583.Get_Empty_LEDBytes();
                            Function_取藥堆疊子資料_設定配藥完成ByIP("None", drawer.IP, "-1");
                            drawer.ActionDone = true;
                            drawerUI_EPD_583.Set_LED_Clear_UDP(drawer);
                            string index_IP = Funcion_取得LCD114索引表_index_IP(drawer.IP);
                            if (index_IP.StringIsEmpty() == false)
                            {
                                Task.Run(new Action(delegate { storageUI_LCD_114.ClearCanvas(index_IP, myConfigClass.LCD114_Port); }));

                            }
                            drawer.SetAllBoxes_LightOff();
                            List_EPD583_本地資料.Add_NewDrawer(drawer);

                        }

                        drawer.input = flag_input;
                    }
                }
                List<object[]> list_locker_table_value = sQLControl_Locker_Index_Table.GetAllRows(null);
                List<object[]> list_locker_table_value_replace = new List<object[]>();
                for (int i = 0; i < list_locker_table_value.Count; i++)
                {
                    string IP = list_locker_table_value[i][(int)enum_lockerIndex.IP].ObjectToString();


                    if (IP.Check_IP_Adress() == false) continue;
                    if (list_locker_table_value[i][(int)enum_lockerIndex.輸出狀態].ObjectToString() != true.ToString()) continue;

                    Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
                    if (drawer == null) continue;
                    list_locker_table_value[i][(int)enum_lockerIndex.輸出狀態] = false.ToString();
                    list_locker_table_value_replace.Add(list_locker_table_value[i]);

                    Console.WriteLine($"抽屜[{drawer.IP}] 【鎖控開啟】....");
                    drawerUI_EPD_583.Set_LockOpen(drawer);
                }
                if (list_locker_table_value_replace.Count > 0) sQLControl_Locker_Index_Table.UpdateByDefulteExtra(null, list_locker_table_value_replace);
                System.Threading.Thread.Sleep(10);
            }
        }

        static public List<object> Function_從入賬資料取得儲位(string 藥品碼)
        {
            if (藥品碼.StringIsEmpty()) return new List<object>();
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_入賬資料.SortByCode(藥品碼);
            List<Storage> storages = List_EPD266_入賬資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_入賬資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_入賬資料.SortByCode(藥品碼);


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

            return list_value;
        }
        static public void Function_從入賬資料取得儲位(string 藥品碼, ref List<string> TYPE, ref List<object> values)
        {
            if (藥品碼.StringIsEmpty()) return;
            List<object> list_value = Function_從入賬資料取得儲位(藥品碼);
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
        static public List<object[]> Function_取得異動儲位資訊從入賬資料(string 藥品碼, string 效期, string 批號, int 異動量)
        {
            if (藥品碼.StringIsEmpty()) return new List<object[]>();
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從入賬資料取得儲位(藥品碼, ref 儲位_TYPE, ref 儲位);
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
            int 使用數量 = 異動量;
            int 庫存數量 = 0;
            int 剩餘庫存數量 = 0;
            for (int i = 0; i < 儲位資訊.Count; i++)
            {
                庫存數量 = 儲位資訊[i][(int)enum_儲位資訊.庫存].ObjectToString().StringToInt32();
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
                List_EPD583_本地資料 = drawerUI_EPD_583.SQL_GetAllDrawers();
                Console.WriteLine($"讀取EPD583資料! 耗時 :{myTimer0.GetTickTime().ToString("0.000")} ");
            }));

            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer1 = new MyTimer();
                myTimer1.StartTickTime(50000);
                List_EPD266_本地資料 = storageUI_EPD_266.SQL_GetAllStorage();
                Console.WriteLine($"讀取EPD266資料! 耗時 :{myTimer1.GetTickTime().ToString("0.000")} ");

            }));
            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer2 = new MyTimer();
                myTimer2.StartTickTime(50000);
                List_RowsLED_本地資料 = rowsLEDUI.SQL_GetAllRowsLED();
                Console.WriteLine($"讀取RowsLED資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

            }));

            taskList.Add(Task.Run(() =>
            {
                MyTimer myTimer2 = new MyTimer();
                myTimer2.StartTickTime(50000);
                List_Pannel35_本地資料 = storageUI_WT32.SQL_GetAllStorage();
                Console.WriteLine($"讀取Pannel35資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

            }));
            List<Device> deviceBasics = new List<Device>();

            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();


            Console.WriteLine($"SQL讀取儲位資料到本地結束! 耗時 : {myTimer.GetTickTime().ToString("0.000")}");
        }
        static public List<object> Function_從本地資料取得儲位(string 藥品碼)
        {
            if (藥品碼.StringIsEmpty()) return new List<object>();
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_本地資料.SortByCode(藥品碼);
            List<Storage> storages = List_EPD266_本地資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_本地資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_本地資料.SortByCode(藥品碼);
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

            return list_value;
        }
        static public object Fucnction_從本地資料取得儲位(string IP)
        {
            Storage storage = List_EPD266_本地資料.SortByIP(IP);
            if (storage != null) return storage;
            Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
            if (drawer != null) return drawer;
            RowsLED rowsLED = List_RowsLED_本地資料.SortByIP(IP);
            if (rowsLED != null) return rowsLED;
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
            RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(IP);
            if (rowsLED != null) return rowsLED;
            Storage pannel35 = List_Pannel35_雲端資料.SortByIP(IP);
            if (pannel35 != null) return pannel35;
            return null;
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
                    List_EPD583_雲端資料 = drawerUI_EPD_583.SQL_GetAllDrawers();
                    Console.WriteLine($"讀取EPD583資料! 耗時 :{myTimer0.GetTickTime().ToString("0.000")} ");
                }));

                taskList.Add(Task.Run(() =>
                {
                    MyTimer myTimer1 = new MyTimer();
                    myTimer1.StartTickTime(50000);
                    List_EPD266_雲端資料 = storageUI_EPD_266.SQL_GetAllStorage();
                    Console.WriteLine($"讀取EPD266資料! 耗時 :{myTimer1.GetTickTime().ToString("0.000")} ");

                }));
                taskList.Add(Task.Run(() =>
                {
                    MyTimer myTimer2 = new MyTimer();
                    myTimer2.StartTickTime(50000);
                    List_RowsLED_雲端資料 = rowsLEDUI.SQL_GetAllRowsLED();
                    Console.WriteLine($"讀取RowsLED資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

                }));
                taskList.Add(Task.Run(() =>
                {
                    MyTimer myTimer2 = new MyTimer();
                    myTimer2.StartTickTime(50000);
                    List_Pannel35_雲端資料 = storageUI_WT32.SQL_GetAllStorage();
                    Console.WriteLine($"讀取Pannel35資料! 耗時 :{myTimer2.GetTickTime().ToString("0.000")} ");

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
            if (藥品碼.StringIsEmpty()) return new List<object>();
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_雲端資料.SortByCode(藥品碼);
            List<Storage> storages = List_EPD266_雲端資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_雲端資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_雲端資料.SortByCode(藥品碼);
            for (int i = 0; i < boxes.Count; i++)
            {
                Box box = drawerUI_EPD_583.SQL_GetBox(boxes[i]);
                List_EPD583_雲端資料.Add_NewDrawer(box);
                list_value.Add(box);
            }

            for (int i = 0; i < storages.Count; i++)
            {
                Storage storage = storageUI_EPD_266.SQL_GetStorage(storages[i]);
                List_EPD266_雲端資料.Add_NewStorage(storage);
                list_value.Add(storage);
            }
            for (int i = 0; i < pannels.Count; i++)
            {
                Storage pannel = storageUI_WT32.SQL_GetStorage(pannels[i]);
                List_Pannel35_雲端資料.Add_NewStorage(pannel);
                list_value.Add(pannel);
            }
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                RowsLED rowsLED = rowsLEDUI.SQL_GetRowsLED(rowsDevices[i].IP);
                RowsDevice rowsDevice = rowsLED.GetRowsDevice(rowsDevices[i].GUID);
                if (rowsDevice != null) list_value.Add(rowsDevice);
                List_RowsLED_雲端資料.Add_NewRowsLED(rowsDevice);
            }

            return list_value;
        }
        static public List<object> Function_從雲端資料取得儲位(string 藥品碼)
        {
            if (藥品碼.StringIsEmpty()) return new List<object>();
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_雲端資料.SortByCode(藥品碼);
            List<Storage> storages = List_EPD266_雲端資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_雲端資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_雲端資料.SortByCode(藥品碼);
            for (int i = 0; i < storages.Count; i++)
            {
                list_value.Add(storages[i]);
            }
            for (int i = 0; i < boxes.Count; i++)
            {
                list_value.Add(boxes[i]);
            }


            for (int i = 0; i < pannels.Count; i++)
            {
                list_value.Add(pannels[i]);
            }
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                list_value.Add(rowsDevices[i]);
            }

            return list_value;
        }
        static public int Function_從雲端資料取得庫存(string 藥品碼)
        {
            if (藥品碼.StringIsEmpty()) return 0;
            int 庫存 = 0;
            List<object> list_value = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(藥品碼, ref 儲位_TYPE, ref list_value);

            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    庫存 += ((Device)list_value[i]).Inventory.StringToInt32();
                }
            }
            if (list_value.Count == 0) return -999;
            return 庫存;
        }
        static public void Function_從雲端資料取得儲位(string 藥品碼, ref List<string> TYPE, ref List<object> values)
        {
            if (藥品碼.StringIsEmpty()) return;
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
        static public List<object[]> Function_新增效期至雲端資料(string 藥品碼, int 異動量, string 效期, string 批號)
        {
            if (藥品碼.StringIsEmpty()) return new List<object[]>();
            object value_device = new object();
            List<object[]> 儲位資訊 = new List<object[]>();
            List<string> TYPE = new List<string>();
            List<object> values = new List<object>();
            string IP = "";
            Function_從雲端資料取得儲位(藥品碼, ref TYPE, ref values);
            string Type_str = "";
            for (int k = 0; k < values.Count; k++)
            {
                if (TYPE[k] == DeviceType.EPD266_lock.GetEnumName() || TYPE[k] == DeviceType.EPD266.GetEnumName()
                    || TYPE[k] == DeviceType.EPD290_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290.GetEnumName()
                    || TYPE[k] == DeviceType.EPD290G_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290G.GetEnumName()
                    || TYPE[k] == DeviceType.EPD420G.GetEnumName() || TYPE[k] == DeviceType.EPD420G_lock.GetEnumName()
                    || TYPE[k] == DeviceType.EPD360E.GetEnumName() || TYPE[k] == DeviceType.EPD360E_lock.GetEnumName())
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
                else if (TYPE[k] == DeviceType.EPD583_lock.GetEnumName() || TYPE[k] == DeviceType.EPD583.GetEnumName()
                         || TYPE[k] == DeviceType.EPD730E_lock.GetEnumName() || TYPE[k] == DeviceType.EPD730E.GetEnumName())
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

            }
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
        static public List<object[]> Function_取得異動儲位資訊從雲端資料(string 藥品碼, int 異動量, string 效期, string IP)
        {
            if (藥品碼.StringIsEmpty()) return new List<object[]>();
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(藥品碼, ref 儲位_TYPE, ref 儲位);
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
                        if (device.List_Validity_period[i] == 效期 && device.IP == IP)
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
        static public List<object[]> Function_取得異動儲位資訊從雲端資料(string 藥品碼, int 異動量, string 效期)
        {
            if (藥品碼.StringIsEmpty()) return new List<object[]>();
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(藥品碼, ref 儲位_TYPE, ref 儲位);
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
                        if (device.List_Validity_period[i] == 效期)
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
        static public List<object[]> Function_取得異動儲位資訊從雲端資料(string 藥品碼, int 異動量)
        {
            if (藥品碼.StringIsEmpty()) return new List<object[]>();
            List<object> 儲位 = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從雲端資料取得儲位(藥品碼, ref 儲位_TYPE, ref 儲位);
            List<object[]> 儲位資訊_buf = new List<object[]>();
            List<object[]> 儲位資訊 = new List<object[]>();
            if (儲位.Count == 0) return 儲位資訊_buf;


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
                        value[(int)enum_儲位資訊.批號] = device.List_Lot_number[i];
                        value[(int)enum_儲位資訊.庫存] = device.List_Inventory[i];
                        value[(int)enum_儲位資訊.異動量] = "0";
                        value[(int)enum_儲位資訊.Value] = value_device;
                        儲位資訊.Add(value);
                    }
                }
            }
            儲位資訊 = 儲位資訊.OrderBy(r => DateTime.Parse(r[(int)enum_儲位資訊.效期].ToDateString())).ToList();

            if (異動量 == 0)
            {
                if (儲位資訊.Count == 0)
                {

                }
                return 儲位資訊;
            }
            int 使用數量 = 異動量;
            int 庫存數量 = 0;
            int 剩餘庫存數量 = 0;
            for (int i = 0; i < 儲位資訊.Count; i++)
            {
                庫存數量 = 儲位資訊[i][(int)enum_儲位資訊.庫存].ObjectToString().StringToInt32();
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
                    || TYPE == DeviceType.EPD290G.GetEnumName() || TYPE == DeviceType.EPD290G_lock.GetEnumName()
                    || TYPE == DeviceType.EPD420G.GetEnumName() || TYPE == DeviceType.EPD420G_lock.GetEnumName()
                    || TYPE == DeviceType.EPD360E.GetEnumName() || TYPE == DeviceType.EPD360E_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = List_EPD266_雲端資料.SortByIP(storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 批號, 異動量, false);
                        List_EPD266_雲端資料.Add_NewStorage(storage);
                        if (upToSQL) storageUI_EPD_266.SQL_ReplaceStorage(storage);
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
                        if (upToSQL) storageUI_WT32.SQL_ReplaceStorage(storage);
                        storage.UpToSQL = true;
                        return storage;
                    }
                }
            }
            else if (Value is Box)
            {
                if (TYPE == DeviceType.EPD583.GetEnumName() || TYPE == DeviceType.EPD583_lock.GetEnumName()
                    || TYPE == DeviceType.EPD730E.GetEnumName() || TYPE == DeviceType.EPD730E_lock.GetEnumName())
                {
                    Box box = (Box)Value;
                    box.效期庫存異動(效期, 批號, 異動量, false);
                    List_EPD583_雲端資料.ReplaceBox(box);
                    Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                    if (upToSQL) drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
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
                    if (upToSQL) rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                    rowsLED.UpToSQL = true;
                    return rowsLED;
                }

            }

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
                    || TYPE == DeviceType.EPD290G.GetEnumName() || TYPE == DeviceType.EPD290G_lock.GetEnumName()
                    || TYPE == DeviceType.EPD420G.GetEnumName() || TYPE == DeviceType.EPD420G_lock.GetEnumName()
                    || TYPE == DeviceType.EPD360E.GetEnumName() || TYPE == DeviceType.EPD360E_lock.GetEnumName())
                {
                    Storage storage = (Storage)Value;
                    storage = List_EPD266_雲端資料.SortByIP(storage.IP);
                    if (storage != null)
                    {
                        storage.效期庫存異動(效期, 異動量, false);
                        List_EPD266_雲端資料.Add_NewStorage(storage);
                        if (upToSQL) storageUI_EPD_266.SQL_ReplaceStorage(storage);
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
                        if (upToSQL) storageUI_WT32.SQL_ReplaceStorage(storage);
                        storage.UpToSQL = true;
                        return storage;
                    }
                }
            }
            else if (Value is Box)
            {
                if (TYPE == DeviceType.EPD583.GetEnumName() || TYPE == DeviceType.EPD583_lock.GetEnumName()
                    || TYPE == DeviceType.EPD730E.GetEnumName() || TYPE == DeviceType.EPD730E_lock.GetEnumName())
                {
                    Box box = (Box)Value;
                    box.效期庫存異動(效期, 異動量, false);
                    List_EPD583_雲端資料.ReplaceBox(box);
                    Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                    if (upToSQL) drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
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
                    if (upToSQL) rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                    rowsLED.UpToSQL = true;
                    return rowsLED;
                }

            }

            return null;
        }

        static public void Function_取藥堆疊資料_取得儲位資訊內容(object[] value, ref string Device_GUID, ref string TYPE, ref string IP, ref string Num, ref string 效期, ref string 批號, ref string 庫存, ref string 異動量)
        {
            if (value[(int)enum_儲位資訊.Value] is Device)
            {
                Device device = value[(int)enum_儲位資訊.Value] as Device;
                Num = (-1).ToString();
                value[(int)enum_儲位資訊.IP] = device.IP;
                value[(int)enum_儲位資訊.TYPE] = device.DeviceType.GetEnumName();
                Device_GUID = device.GUID;
                if (device.DeviceType == DeviceType.RFID_Device)
                {
                    Num = device.MasterIndex.ToString();
                }

            }
            IP = value[(int)enum_儲位資訊.IP].ObjectToString();
            TYPE = value[(int)enum_儲位資訊.TYPE].ObjectToString();
            效期 = value[(int)enum_儲位資訊.效期].ObjectToString();
            批號 = value[(int)enum_儲位資訊.批號].ObjectToString();
            庫存 = value[(int)enum_儲位資訊.庫存].ObjectToString();
            異動量 = value[(int)enum_儲位資訊.異動量].ObjectToString();

        }
        static public void Function_取藥堆疊資料_檢查資料儲位正常(object[] list_母資料)
        {
            string Master_GUID = list_母資料[(int)enum_取藥堆疊母資料.GUID].ObjectToString();
            if (list_母資料[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == "系統領藥") return;
            int 總異動量 = list_母資料[(int)enum_取藥堆疊母資料.總異動量].StringToInt32();
            if (list_母資料[(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringIsInt32() == false)
            {
                總異動量 = -99999;
            }
            List<object[]> list_取藥堆疊子資料 = sQLControl_取藥堆疊子資料.GetAllRows(null);
            list_取藥堆疊子資料 = list_取藥堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);
            int 累積異動量 = 0;
            for (int i = 0; i < list_取藥堆疊子資料.Count; i++)
            {
                累積異動量 += list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.異動量].StringToInt32();
            }
            if (總異動量 != 累積異動量)
            {
                sQLControl_取藥堆疊子資料.DeleteExtra(null, list_取藥堆疊子資料);
            }
        }

        static public List<Device> Function_從SQL取得所有儲位()
        {
            List<List<Device>> list_list_devices = new List<List<Device>>();
            List<Device> devices = new List<Device>();
            Function_從SQL取得儲位到本地資料();

            list_list_devices.Add(List_EPD583_本地資料.GetAllDevice());
            list_list_devices.Add(List_EPD266_本地資料.GetAllDevice());
            list_list_devices.Add(List_RowsLED_本地資料.GetAllDevice());
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
            if (藥品碼.StringIsEmpty()) return new List<object>();
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_本地資料.SortByCode(藥品碼);

            List<Storage> storages = List_EPD266_本地資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_本地資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_本地資料.SortByCode(藥品碼);

            for (int i = 0; i < boxes.Count; i++)
            {
                Box box = drawerUI_EPD_583.SQL_GetBox(boxes[i]);
                List_EPD583_本地資料.Add_NewDrawer(box);
                list_value.Add(box);
            }

            for (int i = 0; i < storages.Count; i++)
            {
                Storage storage = storageUI_EPD_266.SQL_GetStorage(storages[i]);
                List_EPD266_本地資料.Add_NewStorage(storage);
                list_value.Add(storage);
            }
            for (int i = 0; i < pannels.Count; i++)
            {
                Storage pannel = storageUI_WT32.SQL_GetStorage(pannels[i]);
                List_Pannel35_本地資料.Add_NewStorage(pannel);
                list_value.Add(pannel);
            }
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                RowsDevice rowsDevice = rowsLEDUI.SQL_GetRowsDevice(rowsDevices[i]);
                List_RowsLED_本地資料.Add_NewRowsLED(rowsDevice);
                list_value.Add(rowsDevice);
            }

            return list_value;
        }
        static public int Function_從SQL取得庫存(string 藥品碼)
        {
            if (藥品碼.StringIsEmpty()) return 0;
            int 庫存 = 0;
            List<object> list_value = Function_從SQL取得儲位到本地資料(藥品碼);
            for (int i = 0; i < list_value.Count; i++)
            {

                if (list_value[i] is Device)
                {
                    Device device = list_value[i] as Device;
                    if (device != null)
                    {
                        庫存 += device.Inventory.StringToInt32();
                    }
                }

            }

            return 庫存;
        }
        static public int Function_從本地資料取得庫存(string 藥品碼)
        {
            if (藥品碼.StringIsEmpty()) return 0;
            int 庫存 = 0;
            List<object> list_value = Function_從本地資料取得儲位(藥品碼);
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    Device device = (Device)list_value[i];
                    if (device != null)
                    {
                        庫存 += device.Inventory.StringToInt32();
                    }
                }
            }
            if (list_value.Count == 0) return -999;

            return 庫存;
        }

        static public void Function_儲位刷新(string 藥品碼)
        {
            if (藥品碼.StringIsEmpty()) return;
            List<string> list_lock_IP = new List<string>();
            Function_儲位刷新(藥品碼, ref list_lock_IP);
        }
        static public void Function_儲位刷新(string 藥品碼, ref List<string> list_lock_IP)
        {
            if (藥品碼.StringIsEmpty()) return;
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
                    if (device.DeviceType == DeviceType.EPD266 || device.DeviceType == DeviceType.EPD266_lock
                        || device.DeviceType == DeviceType.EPD290 || device.DeviceType == DeviceType.EPD290_lock
                        || device.DeviceType == DeviceType.EPD290G || device.DeviceType == DeviceType.EPD290G_lock
                        || device.DeviceType == DeviceType.EPD420G || device.DeviceType == DeviceType.EPD420G_lock
                        || device.DeviceType == DeviceType.EPD360E || device.DeviceType == DeviceType.EPD360E_lock)
                    {
                        Storage storage = list_Device[i] as Storage;
                        if (storage != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                storageUI_EPD_266.DrawToEpd_UDP(storage);
                                storageUI_EPD_266.Set_Stroage_LED_UDP(storage, Color.Black);
                            }));

                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.EPD266_lock || device.DeviceType == DeviceType.EPD290_lock
                                || device.DeviceType == DeviceType.EPD290G_lock || device.DeviceType == DeviceType.EPD420G_lock
                                || device.DeviceType == DeviceType.EPD360E_lock) list_lock_IP.Add(IP);
                        }
                    }
                    else if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock
                        || device.DeviceType == DeviceType.EPD730E || device.DeviceType == DeviceType.EPD730E_lock)
                    {
                        Box box = list_Device[i] as Box;
                        if (box != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                Drawer drawer = List_EPD583_雲端資料.SortByIP(IP);
                                List<Box> boxes = drawer.SortByCode(藥品碼);

                            }));

                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.EPD583_lock) list_lock_IP.Add(IP);
                        }
                    }

                    else if (device.DeviceType == DeviceType.Pannel35 || device.DeviceType == DeviceType.Pannel35_lock)
                    {
                        Storage storage = list_Device[i] as Storage;
                        if (storage != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                storageUI_WT32.Set_DrawPannelJEPG(storage);
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

        static public void Function_儲位亮燈(LightOn lightOn)
        {

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
                List<object[]> list_取藥堆疊母資料 = sQLControl_取藥堆疊母資料.GetRowsByDefult(null, (int)enum_取藥堆疊母資料.藥品碼, 藥品碼);

                list_取藥堆疊母資料 = (from temp in list_取藥堆疊母資料
                                where temp[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString() == 藥品碼
                                where temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString() != "刷新面板"
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
                            if (device.DeviceType == DeviceType.EPD266_lock || device.DeviceType == DeviceType.EPD290_lock || device.DeviceType == DeviceType.EPD290G_lock
                              || device.DeviceType == DeviceType.EPD420_lock || device.DeviceType == DeviceType.EPD420G_lock || device.DeviceType == DeviceType.EPD360E_lock)
                            {
                                list_lock_IP.Add(IP);
                            }
                        }
                    }
                    else if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock
                        || device.DeviceType == DeviceType.EPD730E || device.DeviceType == DeviceType.EPD730E_lock)
                    {
                        Box box = list_Device[i] as Box;
                        if (box != null)
                        {
                            list_IP.Add(IP);
                            if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock
                                || device.DeviceType == DeviceType.EPD730E || device.DeviceType == DeviceType.EPD730E_lock)
                            {
                                list_lock_IP.Add(IP);
                            }
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

        static public void Function_儲位刷新_byIP(string IP)
        {
            List<string> list_lock_IP = new List<string>();
            Function_儲位刷新_byIP(IP, ref list_lock_IP);
        }
        static public void Function_儲位刷新_byIP(string IP, ref List<string> list_lock_IP)
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
                            storageUI_EPD_266.DrawToEpd_UDP(storage);

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
                            drawerUI_EPD_583.DrawToEpd_UDP(drawer);
                        }));
                        list_IP.Add(IP);
                    }
                }


                Task.WhenAll(taskList).Wait();
            }

        }

        static public void Function_儲位亮燈_Ex(LightOn lightOn)
        {
            string 藥品碼 = lightOn.藥品碼;
            Color color = lightOn.顏色;
            double 數量 = lightOn.數量;

            if (藥品碼.StringIsEmpty()) return;
            List<object> list_Device = Function_從雲端資料取得儲位(藥品碼);
            List<object> list_commonSpace_device = Function_從共用區取得儲位(藥品碼);
            for (int i = 0; i < list_commonSpace_device.Count; i++)
            {
                list_Device.Add(list_commonSpace_device[i]);
            }


            if (color == Color.Black)
            {
                Console.WriteLine($"●●儲位滅燈●●,藥品碼:{藥品碼},color{color.ToColorString()}-------------");
            }
            else
            {
                color = Color.FromArgb((int)(color.R * lightness), (int)(color.G * lightness), (int)(color.B * lightness));
                Console.WriteLine($"◇◇儲位亮燈◇◇,藥品碼:{藥品碼},color{color.ToColorString()}");
            }
            Task allTask;
            List<Task> taskList = new List<Task>();
            List<string> list_IP = new List<string>();
            List<string> list_IP_buf = new List<string>();
            bool flag_led_refresh = false;

            list_IP.Clear();
            list_IP_buf.Clear();
            for (int i = 0; i < list_Device.Count; i++)
            {
                Device device = list_Device[i] as Device;
                string IP = device.IP;

                if (device != null)
                {


                }
            }
            taskList.Clear();
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
                        if (storage == null) continue;
                        taskList.Add(Task.Run(() =>
                        {
                            if (storage.TOFON == false)
                            {
                                if (color == Color.Black)
                                {
                                    if (myConfigClass.舊版晶片 == false)
                                    {
                                        storageUI_EPD_266.Set_WS2812B_breathing(storage, 30, 30, color);
                                    }
                                    else
                                    {
                                        storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                                    }
                                }
                                else
                                {
                                    storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                                }
                            }
                            else
                            {
                                if (color == Color.Black)
                                {
                                    if (myConfigClass.舊版晶片 == false)
                                    {
                                        storageUI_EPD_266.Set_WS2812B_breathing(storage, 30, 30, color);
                                    }
                                    else
                                    {
                                        storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);

                                    }

                                }
                                else if (lightOn.flag_Refresh_LCD || lightOn.flag_Refresh_Light)
                                {
                                    storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                                }

                            }
                            string index_IP = Funcion_取得LCD114索引表_index_IP(storage.IP);
                            if (index_IP.StringIsEmpty()) return;
                            if (color == Color.Black) storageUI_LCD_114.ClearCanvas(index_IP, 29008);
                            if (lightOn.flag_Refresh_LCD)
                            {
                                Color color_fore = Color.White;
                                if (lightOn.LCD_Color.R > 230 && lightOn.LCD_Color.G > 230 && lightOn.LCD_Color.B > 230) color_fore = Color.Black;
                                storageUI_LCD_114.DrawImage(index_IP, 29008, 數量.ToString(), new Font("標楷體", 70, FontStyle.Bold), color_fore, lightOn.LCD_Color);
                            }

                        }));

                        list_IP.Add(IP);
                    }
                    else if (device.DeviceType == DeviceType.EPD583 || device.DeviceType == DeviceType.EPD583_lock
                         || device.DeviceType == DeviceType.EPD420_D || device.DeviceType == DeviceType.EPD420_D_lock
                         || device.DeviceType == DeviceType.EPD730E || device.DeviceType == DeviceType.EPD730E_lock)
                    {
                        taskList.Add(Task.Run(() =>
                        {

                            string index_IP = Funcion_取得LCD114索引表_index_IP(device.IP);
                            if (index_IP.StringIsEmpty()) return;
                            if (color == Color.Black) storageUI_LCD_114.ClearCanvas(index_IP, myConfigClass.LCD114_Port);
                            if (lightOn.flag_Refresh_LCD)
                            {
                                Color color_fore = Color.White;
                                if (lightOn.LCD_Color.R > 230 && lightOn.LCD_Color.G > 230 && lightOn.LCD_Color.B > 230) color_fore = Color.Black;
                                storageUI_LCD_114.DrawImage(index_IP, myConfigClass.LCD114_Port, 數量.ToString(), new Font("標楷體", 70, FontStyle.Bold), color_fore, lightOn.LCD_Color);
                            }

                        }));
                    }
                }
            }
            allTask = Task.WhenAll(taskList);
            allTask.Wait();
        }

        static public void Funnction_交易記錄查詢_取得指定藥碼批號期效期(string 藥碼, ref List<string> list_效期, ref List<string> list_批號)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            List<string> list_效期_buf = new List<string>();
            List<string> list_操作時間 = new List<string>();
            if (藥碼.StringIsEmpty()) return;
            List<object[]> list_value = sQLControl_交易記錄查詢.GetRowsByDefult(null, (int)enum_交易記錄查詢資料.藥品碼, 藥碼);
            Console.WriteLine($"搜尋藥碼: {藥碼} , {myTimerBasic.ToString()}");
            string 備註 = "";
            string 操作時間 = "";
            for (int i = 0; i < list_value.Count; i++)
            {
                備註 = list_value[i][(int)enum_交易記錄查詢資料.備註].ObjectToString();
                string[] temp_ary = 備註.Split('\n');
                for (int k = 0; k < temp_ary.Length; k++)
                {
                    string 效期 = temp_ary[k].GetTextValue("效期");
                    string 批號 = temp_ary[k].GetTextValue("批號");
                    操作時間 = list_value[i][(int)enum_交易記錄查詢資料.操作時間].ToDateTimeString();
                    if (效期.StringIsEmpty() == true) continue;
                    list_效期_buf = (from temp in list_效期
                                   where temp == 效期
                                   select temp).ToList();
                    if (list_效期_buf.Count > 0) continue;
                    list_效期.Add(效期);
                    list_批號.Add(批號);
                    list_操作時間.Add(操作時間);
                }
            }
            // 組合效期、批號和操作時間
            List<Tuple<string, string, DateTime>> combinedList = new List<Tuple<string, string, DateTime>>();
            for (int i = 0; i < list_效期.Count; i++)
            {
                combinedList.Add(new Tuple<string, string, DateTime>(list_效期[i], list_批號[i], DateTime.Parse(list_操作時間[i])));
            }

            // 根據操作時間排序
            combinedList.Sort((x, y) => DateTime.Compare(y.Item3, x.Item3));

            // 更新list_效期、list_批號和list_操作時間
            list_效期.Clear();
            list_批號.Clear();
            list_操作時間.Clear();
            foreach (var item in combinedList)
            {
                list_效期.Add(item.Item1);
                list_批號.Add(item.Item2);
                list_操作時間.Add(item.Item3.ToString());
            }
        }
        static public List<object[]> Function_取藥堆疊子資料_取得可致能(ref List<object[]> list_value)
        {
            string IP;
            string Num = "";
            string 調劑台名稱 = "";
            string 藥品碼 = "";
            string 致能 = "";
            string 流程作業完成 = "";
            string 配藥完成 = "";
            bool flag_可致能資料 = true;
            List<object[]> list_取藥堆疊子資料 = list_value;
            List<object[]> list_取藥堆疊子資料_buf = new List<object[]>();
            list_取藥堆疊子資料.Sort(new Icp_取藥堆疊子資料_致能排序());
            for (int i = 0; i < list_取藥堆疊子資料.Count; i++)
            {
                flag_可致能資料 = true;
                IP = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.IP].ObjectToString();
                Num = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.Num].ObjectToString();
                致能 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.致能].ObjectToString();
                流程作業完成 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString();
                配藥完成 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.配藥完成].ObjectToString();
                藥品碼 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();

                if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD266.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD266_lock.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290_lock.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290G.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290G_lock.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420G.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420G_lock.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD360E.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD360E_lock.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.Pannel35.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.Pannel35_lock.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD583.GetEnumName()
                     || list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD583_lock.GetEnumName()
                     || list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD730E.GetEnumName()
                     || list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD730E_lock.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
          
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD1020.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD1020_lock.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.RowsLED.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                else if (list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.RFID_Device.GetEnumName())
                {
                    if (致能 == true.ToString() && 流程作業完成 == true.ToString() && 配藥完成 == true.ToString())
                    {
                        flag_可致能資料 = false;
                    }
                }
                if (flag_可致能資料)
                {
                    List<object[]> list_temp = (from value in list_取藥堆疊子資料_buf
                                                where IP == value[(int)enum_取藥堆疊子資料.IP].ObjectToString()
                                                where Num == value[(int)enum_取藥堆疊子資料.Num].ObjectToString()
                                                where 調劑台名稱 != value[(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString()
                                                where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                                select value).ToList();
                    if (list_temp.Count > 0) flag_可致能資料 = false;
                }
                if (flag_可致能資料)
                {
                    list_取藥堆疊子資料_buf.Add(list_取藥堆疊子資料[i]);
                }
            }
            return list_取藥堆疊子資料_buf;
        }
        static public List<object[]> Function_取藥堆疊母資料_取得可入賬資料()
        {
            List<object[]> list_取藥堆疊母資料 = Function_取藥堆疊資料_取得母資料();
            list_取藥堆疊母資料 = (from value in list_取藥堆疊母資料
                            where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName()
                            select value).ToList();
            return list_取藥堆疊母資料;
        }
        static public object[] Function_取藥堆疊子資料_設定已入帳(object[] 堆疊子資料)
        {
            string IP = 堆疊子資料[(int)enum_取藥堆疊子資料.IP].ObjectToString();
            string 藥品碼 = 堆疊子資料[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
            string str_TYPE = 堆疊子資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString();
            string 效期 = 堆疊子資料[(int)enum_取藥堆疊子資料.效期].ObjectToString();
            double 異動量 = 堆疊子資料[(int)enum_取藥堆疊子資料.異動量].StringToInt32();
            double 儲位庫存 = 0;
            string 批號 = 堆疊子資料[(int)enum_取藥堆疊子資料.批號].ObjectToString();
            if (str_TYPE == DeviceType.EPD583.GetEnumName() || str_TYPE == DeviceType.EPD583_lock.GetEnumName()
                || str_TYPE == DeviceType.EPD730E.GetEnumName() || str_TYPE == DeviceType.EPD730E_lock.GetEnumName())
            {
                List<Box> boxes = List_EPD583_入賬資料.SortByCode(藥品碼);
                for (int i = 0; i < boxes.Count; i++)
                {
                    if (boxes[i].IP != IP) continue;
                    boxes[i] = drawerUI_EPD_583.SQL_GetBox(boxes[i]);
                    儲位庫存 = boxes[i].取得庫存(效期);
                    if (儲位庫存 + 異動量 < 0)
                    {
                        List<string> list_效期 = new List<string>();
                        List<string> list_批號 = new List<string>();
                        List<string> list_異動量 = new List<string>();
                        boxes[i].庫存異動(異動量, out list_效期, out list_批號);

                        Drawer drawer = List_EPD583_入賬資料.ReplaceBox(boxes[i]);
                        List_EPD583_入賬資料.Add_NewDrawer(boxes[i]);
                        drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                        break;
                    }
                    else if ((儲位庫存) >= 0)
                    {
                        boxes[i].效期庫存異動(效期, 異動量);
                        批號 = boxes[i].取得批號(效期);
                        Drawer drawer = List_EPD583_入賬資料.ReplaceBox(boxes[i]);
                        List_EPD583_入賬資料.Add_NewDrawer(boxes[i]);
                        drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                        break;
                    }
                    else if ((儲位庫存) == -1)
                    {
                        boxes[i].新增效期(效期, 批號, 異動量.ToString());
                        Drawer drawer = List_EPD583_入賬資料.ReplaceBox(boxes[i]);
                        List_EPD583_入賬資料.Add_NewDrawer(boxes[i]);
                        drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                        break;
                    }
                }
            }
            else if (str_TYPE == DeviceType.EPD266.GetEnumName() || str_TYPE == DeviceType.EPD266_lock.GetEnumName()
                || str_TYPE == DeviceType.EPD290.GetEnumName() || str_TYPE == DeviceType.EPD290_lock.GetEnumName()
                || str_TYPE == DeviceType.EPD290G.GetEnumName() || str_TYPE == DeviceType.EPD290G_lock.GetEnumName()
                || str_TYPE == DeviceType.EPD420G.GetEnumName() || str_TYPE == DeviceType.EPD420G_lock.GetEnumName()
                || str_TYPE == DeviceType.EPD360E.GetEnumName() || str_TYPE == DeviceType.EPD360E_lock.GetEnumName())
            {
                Storage storage = List_EPD266_入賬資料.SortByIP(IP);
                storage = storageUI_EPD_266.SQL_GetStorage(storage);
                儲位庫存 = storage.取得庫存(效期);
                if (儲位庫存 + 異動量 < 0)
                {
                    List<string> list_效期 = new List<string>();
                    List<string> list_批號 = new List<string>();
                    List<string> list_異動量 = new List<string>();
                    storage.庫存異動(異動量, out list_效期, out list_批號);

                    List_EPD266_入賬資料.Add_NewStorage(storage);
                    storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
                else if ((儲位庫存) >= 0)
                {
                    storage.效期庫存異動(效期, 異動量);
                    List_EPD266_入賬資料.Add_NewStorage(storage);
                    storageUI_EPD_266.SQL_ReplaceStorage(storage);

                }
                else if ((儲位庫存) == -1)
                {
                    storage.新增效期(效期, 批號, 異動量.ToString());
                    List_EPD266_入賬資料.Add_NewStorage(storage);
                    storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }
            else if (str_TYPE == DeviceType.Pannel35_lock.GetEnumName() || str_TYPE == DeviceType.Pannel35.GetEnumName())
            {
                Storage storage = List_Pannel35_入賬資料.SortByIP(IP);
                storage = storageUI_WT32.SQL_GetStorage(storage);
                儲位庫存 = storage.取得庫存(效期);
                if (儲位庫存 + 異動量 < 0)
                {
                    List<string> list_效期 = new List<string>();
                    List<string> list_批號 = new List<string>();
                    List<string> list_異動量 = new List<string>();
                    storage.庫存異動(異動量, out list_效期, out list_批號);

                    List_Pannel35_入賬資料.Add_NewStorage(storage);
                    storageUI_WT32.SQL_ReplaceStorage(storage);
                }
                else if ((儲位庫存) >= 0)
                {
                    storage.效期庫存異動(效期, 異動量);
                    List_Pannel35_入賬資料.Add_NewStorage(storage);
                    storageUI_WT32.SQL_ReplaceStorage(storage);
                    Task.Run(() =>
                    {
                        storageUI_WT32.Set_DrawPannelJEPG(storage);
                    });

                }
                else if ((儲位庫存) == -1)
                {
                    storage.新增效期(效期, 批號, 異動量.ToString());
                    List_Pannel35_入賬資料.Add_NewStorage(storage);
                    storageUI_WT32.SQL_ReplaceStorage(storage);
                }
            }
            else if (str_TYPE == DeviceType.RowsLED.GetEnumName())
            {
                List<RowsDevice> rowsDevices = List_RowsLED_入賬資料.SortByCode(藥品碼);
                for (int i = 0; i < rowsDevices.Count; i++)
                {
                    if (rowsDevices[i].IP != IP) continue;
                    rowsDevices[i] = rowsLEDUI.SQL_GetRowsDevice(rowsDevices[i]);
                    儲位庫存 = rowsDevices[i].取得庫存(效期);
                    if (儲位庫存 + 異動量 < 0)
                    {
                        List<string> list_效期 = new List<string>();
                        List<string> list_批號 = new List<string>();
                        List<string> list_異動量 = new List<string>();
                        rowsDevices[i].庫存異動(異動量, out list_效期, out list_批號);

                        List_RowsLED_入賬資料.Add_NewRowsLED(rowsDevices[i]);
                        RowsLED rowsLED = List_RowsLED_入賬資料.SortByIP(rowsDevices[i].IP);
                        rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                        break;
                    }
                    else if ((儲位庫存) >= 0)
                    {
                        rowsDevices[i].效期庫存異動(效期, 異動量);
                        List_RowsLED_入賬資料.Add_NewRowsLED(rowsDevices[i]);
                        RowsLED rowsLED = List_RowsLED_入賬資料.SortByIP(rowsDevices[i].IP);
                        rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                        break;
                    }
                    else if ((儲位庫存) == -1)
                    {
                        rowsDevices[i].新增效期(效期, 批號, 異動量.ToString());
                        List_RowsLED_入賬資料.Add_NewRowsLED(rowsDevices[i]);
                        RowsLED rowsLED = List_RowsLED_入賬資料.SortByIP(rowsDevices[i].IP);
                        rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                    }
                }
            }
            堆疊子資料[(int)enum_取藥堆疊子資料.已入帳] = true.ToString();
            堆疊子資料[(int)enum_取藥堆疊子資料.致能] = true.ToString();
            堆疊子資料[(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
            堆疊子資料[(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
            return 堆疊子資料;
        }


        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱母資料(string 調劑台名稱)
        {
            List<object[]> list_values = Function_取藥堆疊資料_取得母資料();
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString() == 調劑台名稱).ToList();
            return list_values;
        }
        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱母資料(string 調劑台名稱, string 藥品碼)
        {
            List<object[]> list_values = Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱);
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString() == 藥品碼).ToList();
            return list_values;
        }
        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱子資料(string 調劑台名稱)
        {
            List<object[]> list_values = Function_取藥堆疊資料_取得子資料();
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString() == 調劑台名稱).ToList();
            return list_values;
        }
        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱子資料(string 調劑台名稱, string 藥品碼)
        {
            List<object[]> list_values = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱);
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString() == 藥品碼).ToList();
            return list_values;
        }
        static public List<object[]> Function_取藥堆疊資料_取得指定調劑台名稱子資料(string 調劑台名稱, string 藥品碼, string IP)
        {
            List<object[]> list_values = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱);
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString() == 藥品碼).ToList();
            list_values = list_values.Where(a => a[(int)enum_取藥堆疊子資料.IP].ObjectToString() == IP).ToList();
            return list_values;
        }
        static public void Function_取藥堆疊資料_刪除母資料(string GUID)
        {
            List<object[]> list_value = sQLControl_取藥堆疊母資料.GetAllRows(null);
            List<object[]> list_value_buf = list_value.GetRows((int)enum_取藥堆疊母資料.GUID, GUID);
            if (list_value_buf.Count == 0) return;
            sQLControl_取藥堆疊母資料.DeleteExtra(null, list_value_buf);


            string 藥品碼 = list_value_buf[0][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();

            Function_儲位亮燈(new LightOn(藥品碼, Color.Black));

        }
        static public void Function_取藥堆疊資料_刪除子資料(string GUID)
        {
            Function_取藥堆疊資料_刪除子資料(GUID, true);
        }
        static public void Function_取藥堆疊資料_刪除子資料(string GUID, bool color_off)
        {
            List<object[]> list_value = sQLControl_取藥堆疊子資料.GetRowsByDefult(null, enum_取藥堆疊子資料.GUID.GetEnumName(), GUID);
            if (list_value.Count > 0)
            {
                string 藥品碼 = list_value[0][(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                sQLControl_取藥堆疊子資料.DeleteByDefult(null, enum_取藥堆疊子資料.GUID.GetEnumName(), GUID);
                if (藥品碼.StringIsEmpty()) return;

                Console.WriteLine($"{DateTime.Now.ToDateTimeString()}-刪除子資料 藥品碼: {藥品碼}");

                Function_儲位亮燈(new LightOn(藥品碼, Color.Black));

            }
        }
        static public void Function_取藥堆疊資料_刪除指定調劑台名稱母資料(string 調劑台名稱)
        {
            while (true)
            {
                List<object[]> list_value = sQLControl_取藥堆疊母資料.GetAllRows(null);
                List<object[]> list_value_buf = list_value.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, 調劑台名稱);
                List<object[]> list_value_add = new List<object[]>();
                List<object[]> list_value_replace = new List<object[]>();
                if (list_value_buf.Count == 0) break;
                sQLControl_取藥堆疊母資料.DeleteExtra(null, list_value_buf);
                for (int i = 0; i < list_value_buf.Count; i++)
                {

                    string 藥品碼 = list_value_buf[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    string Master_GUID = list_value_buf[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                    //List<object[]> list_取藥堆疊子資料_delete = this.sqL_DataGridView_取藥堆疊子資料.SQL_GetRows(enum_取藥堆疊子資料.Master_GUID.GetEnumName(), Master_GUID, false);
                    //this.sqL_DataGridView_取藥堆疊子資料.SQL_DeleteExtra(list_取藥堆疊子資料_delete, false);
                    if (藥品碼.StringIsEmpty()) continue;
                    Function_儲位亮燈(new LightOn(藥品碼, Color.Black));

                }

            }
            System.Threading.Thread.Sleep(100);
        }
        static public void Function_取藥堆疊資料_設定作業模式(object[] value, enum_取藥堆疊母資料_作業模式 enum_value)
        {
            Function_取藥堆疊資料_設定作業模式(value, enum_value, true);
        }
        static public void Function_取藥堆疊資料_設定作業模式(object[] value, enum_取藥堆疊母資料_作業模式 enum_value, bool state)
        {
            UInt32 temp = value[(int)enum_取藥堆疊母資料.作業模式].StringToUInt32();
            temp.SetBit((int)enum_value, state);
            value[(int)enum_取藥堆疊母資料.作業模式] = temp.ToString();
        }
        static public bool Function_取藥堆疊資料_取得作業模式(object[] value, enum_取藥堆疊母資料_作業模式 enum_value)
        {
            UInt32 temp = value[(int)enum_取藥堆疊母資料.作業模式].StringToUInt32();
            return temp.GetBit((int)enum_value);
        }
        static private void Function_取藥堆疊資料_新增母資料(takeMedicineStackClass takeMedicineStackClass)
        {
            List<takeMedicineStackClass> takeMedicineStackClasses = new List<takeMedicineStackClass>();
            takeMedicineStackClasses.Add(takeMedicineStackClass);
            Function_取藥堆疊資料_新增母資料(takeMedicineStackClasses);

        }
        static private void Function_取藥堆疊資料_新增母資料(List<takeMedicineStackClass> takeMedicineStackClasses)
        {
            List<takeMedicineStackClass> takeMedicineStackClasses_buf = new List<takeMedicineStackClass>();
            List<Task> taskList = new List<Task>();
            MyTimer myTimer = new MyTimer(500000);
            MyTimer myTimer_total = new MyTimer(500000);
            List<string> codes = takeMedicineStackClasses.Select(x => x.藥品碼).ToList();
            List<medClass> medClasses_cloud = medClass.get_med_clouds_by_codes(API_Server, codes);
            List<medClass> medClasses_cloud_buf = new List<medClass>();
            if (medClasses_cloud_global == null) medClasses_cloud_global = medClass.get_med_cloud(API_Server);

            List<object[]> list_堆疊母資料 = sQLControl_取藥堆疊母資料.GetAllRows(null);
            List<object[]> list_堆疊母資料_add = new List<object[]>();
            bool flag_複盤 = false;
            bool flag_盲盤 = false;
            bool flag_效期管理 = false;
            bool flag_雙人覆核 = false;


            string 顏色 = "";
            for (int i = 0; i < takeMedicineStackClasses.Count; i++)
            {
                List<object[]> list_堆疊母資料_buf = new List<object[]>();
                string 藥品碼 = takeMedicineStackClasses[i].藥品碼;
                string 病歷號 = takeMedicineStackClasses[i].病歷號;
                string 開方時間 = takeMedicineStackClasses[i].開方時間;
                string 藥袋序號 = takeMedicineStackClasses[i].藥袋序號;
                顏色 = takeMedicineStackClasses[i].顏色;

                medClasses_cloud_buf = medClasses_cloud_global.Where(x => x.藥品碼 == 藥品碼).ToList();
                if (medClasses_cloud_buf.Count > 0)
                {
                    string 料號 = medClasses_cloud_buf[0].料號;
                    medClasses_cloud_buf = medClasses_cloud_global.Where(x => x.料號 == 料號).ToList();
                    if (medClasses_cloud_buf.Count > 1)
                    {
                        藥品碼 = medClasses_cloud_buf[0].料號;
                        takeMedicineStackClasses[i].藥品碼 = 藥品碼;
                    }
                }

                list_堆疊母資料 = list_堆疊母資料_buf.GetRows((int)enum_取藥堆疊母資料.藥品碼, 藥品碼);
                list_堆疊母資料_buf = list_堆疊母資料_buf.GetRows((int)enum_取藥堆疊母資料.病歷號, takeMedicineStackClasses[i].病歷號);
                list_堆疊母資料_buf = list_堆疊母資料_buf.GetRows((int)enum_取藥堆疊母資料.開方時間, takeMedicineStackClasses[i].開方時間);
                list_堆疊母資料_buf = list_堆疊母資料_buf.GetRows((int)enum_取藥堆疊母資料.藥袋序號, takeMedicineStackClasses[i].藥袋序號);
                if (list_堆疊母資料_buf.Count != 0) continue;
                if (takeMedicineStackClasses[i].GUID.StringIsEmpty())
                {
                    takeMedicineStackClasses[i].GUID = Guid.NewGuid().ToString();

                }
                takeMedicineStackClasses[i].序號 = DateTime.Now.ToDateTimeString_6();
                takeMedicineStackClasses[i].操作時間 = DateTime.Now.ToDateTimeString_6();
                if (takeMedicineStackClasses[i].狀態 != enum_取藥堆疊母資料_狀態.刪除資料.GetEnumName() && takeMedicineStackClasses[i].狀態 != enum_取藥堆疊母資料_狀態.已領用過.GetEnumName()) takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName();

                if (takeMedicineStackClasses[i].動作 != enum_交易記錄查詢動作.入庫作業.GetEnumName()) takeMedicineStackClasses[i].IP = "";
                if (takeMedicineStackClasses[i].效期.Check_Date_String())
                {
                    if (takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.入庫作業.GetEnumName() || takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.調入作業.GetEnumName()
                        || takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.掃碼退藥.GetEnumName() || takeMedicineStackClasses[i].動作 == enum_交易記錄查詢動作.手輸退藥.GetEnumName())
                    {
                        takeMedicineStackClasses[i].狀態 = enum_取藥堆疊母資料_狀態.新增效期.GetEnumName();
                    }
                }
                takeMedicineStackClasses[i].庫存量 = "0";
                takeMedicineStackClasses[i].結存量 = "0";
                takeMedicineStackClasses[i].作業模式 = "0";
                object[] value = takeMedicineStackClasses[i].ClassToSQL<takeMedicineStackClass, enum_取藥堆疊母資料>();
                value[(int)enum_取藥堆疊母資料.動作] = takeMedicineStackClasses[i].動作.GetEnumName();
                value[(int)enum_取藥堆疊母資料.狀態] = takeMedicineStackClasses[i].狀態.GetEnumName();



                list_堆疊母資料_add.Add(value);

                Console.WriteLine($"----------------------------------------------");
                Console.WriteLine($"{i + 1}.新增取藥堆疊 : ");
                Console.WriteLine($" (動作){takeMedicineStackClasses[i].動作.GetEnumName()} ");
                Console.WriteLine($" (藥品碼){藥品碼} ");
                Console.WriteLine($" (病歷號){病歷號} ");
                Console.WriteLine($" (開方時間){開方時間} ");
                Console.WriteLine($" (狀態){takeMedicineStackClasses[i].狀態.GetEnumName()} ");
                Console.WriteLine($" (總異動量){takeMedicineStackClasses[i].總異動量} ");
                Console.WriteLine($" (顏色){takeMedicineStackClasses[i].顏色} ");
                Console.WriteLine($" (複盤){flag_複盤} ");
                Console.WriteLine($" (盲盤){flag_盲盤} ");
                Console.WriteLine($" (效期管理){flag_效期管理} ");
                Console.WriteLine($" (雙人覆核){flag_雙人覆核} ");
                Console.WriteLine($" (效期管理){flag_效期管理} ");
                Console.WriteLine($" (耗時){myTimer.ToString()} ");
                Console.WriteLine($"----------------------------------------------");

            }


            List<string> list_藥品碼 = (from temp in takeMedicineStackClasses
                                     select temp.藥品碼).Distinct().ToList();
            List<string> list_lock_IP = new List<string>();
            //for (int i = 0; i < list_藥品碼.Count; i++)
            //{
            //    Function_儲位亮燈(new LightOn(list_藥品碼[i], 顏色.ToColor()), ref list_lock_IP);
            //}
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();
            //for (int i = 0; i < list_堆疊母資料_add.Count; i++)
            //{
            //    Function_抽屜解鎖(list_lock_IP);
            //}

            Console.WriteLine($"#1 commonSapceClasses : {commonSapceClasses.Count}");
            if (list_堆疊母資料_add.Count > 0)
            {

                Task.Run(new Action(delegate
                {
                    commonSapceClasses.WriteTakeMedicineStack(list_堆疊母資料_add);
                }));

                sQLControl_取藥堆疊母資料.AddRows(null, list_堆疊母資料_add);
            }


            Console.WriteLine($" 新增取藥資料 (耗時){myTimer_total.ToString()} ");


        }
        static public object[] Function_取藥堆疊資料_新增子資料(string Master_GUID, string Device_GUID, string 調劑台名稱, string 藥品碼, string IP, string Num, string _enum_取藥堆疊_TYPE, string 效期, string 批號, string 異動量)
        {

            object[] value = new object[new enum_取藥堆疊子資料().GetLength()];
            value[(int)enum_取藥堆疊子資料.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_取藥堆疊子資料.Master_GUID] = Master_GUID;
            value[(int)enum_取藥堆疊子資料.Device_GUID] = Device_GUID;
            value[(int)enum_取藥堆疊子資料.序號] = SQLControl.GetTimeNow_6();
            value[(int)enum_取藥堆疊子資料.調劑台名稱] = 調劑台名稱;
            value[(int)enum_取藥堆疊子資料.藥品碼] = 藥品碼;
            value[(int)enum_取藥堆疊子資料.IP] = IP;
            value[(int)enum_取藥堆疊子資料.Num] = Num;
            value[(int)enum_取藥堆疊子資料.TYPE] = _enum_取藥堆疊_TYPE;
            value[(int)enum_取藥堆疊子資料.效期] = 效期;
            value[(int)enum_取藥堆疊子資料.批號] = 批號;
            value[(int)enum_取藥堆疊子資料.異動量] = 異動量.ToString();
            value[(int)enum_取藥堆疊子資料.致能] = false.ToString();
            value[(int)enum_取藥堆疊子資料.流程作業完成] = false.ToString();
            value[(int)enum_取藥堆疊子資料.配藥完成] = false.ToString();
            value[(int)enum_取藥堆疊子資料.調劑結束] = false.ToString();
            value[(int)enum_取藥堆疊子資料.已入帳] = false.ToString();
            Console.WriteLine($"{DateTime.Now.ToDateTimeString()} - 新增子資料 藥碼 : {藥品碼}");
            sQLControl_取藥堆疊子資料.AddRow(null, value);
            return value;
        }
        static public List<object[]> Function_取藥堆疊資料_取得母資料()
        {
            return sQLControl_取藥堆疊母資料.GetAllRows(null);
        }
        static public List<object[]> Function_取藥堆疊資料_取得子資料()
        {
            return sQLControl_取藥堆疊子資料.GetAllRows(null);
        }
        static public List<object[]> Function_取藥堆疊子資料_設定流程作業完成ByIP(string 調劑台名稱, string IP)
        {
            return Function_取藥堆疊子資料_設定流程作業完成ByIP(調劑台名稱, IP, "-1");
        }
        static public List<object[]> Function_取藥堆疊子資料_設定流程作業完成ByIP(string 調劑台名稱, string IP, string Num)
        {
            List<object[]> list_堆疊子資料 = new List<object[]>();
            List<object[]> serch_values = new List<object[]>();
            if (調劑台名稱 != "None")
            {
                list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.IP, IP);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Num, Num);

                for (int i = 0; i < list_堆疊子資料.Count; i++)
                {
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    serch_values.Add(list_堆疊子資料[i]);

                }
                sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, serch_values);
            }
            else
            {
                if (Num.StringIsEmpty()) Num = "-1";
                list_堆疊子資料 = Function_取藥堆疊資料_取得子資料();
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.IP, IP);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Num, Num);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.致能, true.ToString());
                for (int i = 0; i < list_堆疊子資料.Count; i++)
                {
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    serch_values.Add(list_堆疊子資料[i]);
                }
                sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, serch_values);
            }
            return list_堆疊子資料;
        }
        static public void Function_取藥堆疊子資料_設定流程作業完成ByCode(string 調劑台名稱, string 藥品碼)
        {
            string Master_GUID = "";
            List<object[]> list_堆疊母資料 = Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱, 藥品碼);
            List<object[]> list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱, 藥品碼);
            List<object[]> list_堆疊子資料_buf;
            List<object[]> list_serch_values = new List<object[]>();
            for (int i = 0; i < list_堆疊母資料.Count; i++)
            {
                Master_GUID = list_堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                list_堆疊子資料_buf = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);

                for (int k = 0; k < list_堆疊子資料_buf.Count; k++)
                {
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_serch_values.Add(list_堆疊子資料_buf[k]);
                }
            }
            sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_serch_values);
        }
        static public void Function_取藥堆疊子資料_設定流程作業完成ByCode(string 調劑台名稱, string 藥品碼, string IP)
        {
            List<object[]> list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱, 藥品碼, IP);
            List<object[]> list_serch_values = new List<object[]>();
            for (int k = 0; k < list_堆疊子資料.Count; k++)
            {
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                list_serch_values.Add(list_堆疊子資料[k]);
            }
            sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_serch_values);
        }
        static public void Function_取藥堆疊子資料_設定配藥完成ByIP(string 調劑台名稱, string IP)
        {
            Function_取藥堆疊子資料_設定配藥完成ByIP(調劑台名稱, IP, "-1");
        }
        static public void Function_取藥堆疊子資料_設定配藥完成ByIP(string 調劑台名稱, string IP, string Num)
        {
            List<object[]> list_堆疊子資料 = new List<object[]>();
            List<object[]> serch_values = new List<object[]>();
            if (調劑台名稱 != "None")
            {
                list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.IP, IP);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Num, Num);
                for (int i = 0; i < list_堆疊子資料.Count; i++)
                {
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    serch_values.Add(list_堆疊子資料[i]);

                }
                sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, serch_values);
            }
            else
            {
                if (Num.StringIsEmpty()) Num = "-1";
                list_堆疊子資料 = Function_取藥堆疊資料_取得子資料();
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.IP, IP);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Num, Num);
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.致能, true.ToString());
                list_堆疊子資料 = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.流程作業完成, true.ToString());
                for (int i = 0; i < list_堆疊子資料.Count; i++)
                {
                    list_堆疊子資料[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    serch_values.Add(list_堆疊子資料[i]);
                }
                sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, serch_values);
            }
        }
        static public void Function_取藥堆疊子資料_設定配藥完成ByCode(string 調劑台名稱, string 藥品碼)
        {
            string Master_GUID = "";
            List<object[]> list_堆疊母資料 = Function_取藥堆疊資料_取得指定調劑台名稱母資料(調劑台名稱, 藥品碼);
            List<object[]> list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱, 藥品碼);
            List<object[]> list_堆疊子資料_buf;
            List<object[]> list_serch_values = new List<object[]>();
            for (int i = 0; i < list_堆疊母資料.Count; i++)
            {
                Master_GUID = list_堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                list_堆疊子資料_buf = list_堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);
                for (int k = 0; k < list_堆疊子資料_buf.Count; k++)
                {
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    list_serch_values.Add(list_堆疊子資料_buf[k]);
                }
            }
            sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_serch_values);
        }
        static public void Function_取藥堆疊子資料_設定配藥完成ByCode(string 調劑台名稱, string 藥品碼, string IP)
        {
            List<object[]> list_堆疊子資料 = Function_取藥堆疊資料_取得指定調劑台名稱子資料(調劑台名稱, 藥品碼, IP);
            List<object[]> list_serch_values = new List<object[]>();
            for (int k = 0; k < list_堆疊子資料.Count; k++)
            {
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                list_堆疊子資料[k][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                list_serch_values.Add(list_堆疊子資料[k]);
            }
            sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_serch_values);
        }
        static public void Function_設定配藥完成()
        {
            List<object[]> list_取藥堆疊母資料 = Function_取藥堆疊資料_取得母資料();
            List<object[]> list_取藥堆疊子資料 = Function_取藥堆疊資料_取得子資料();
            List<object[]> list_取藥堆疊子資料_buf = new List<object[]>();
            List<object[]> list_取藥堆疊子資料_replace = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_replace = new List<object[]>();
            list_取藥堆疊母資料 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.作業完成.GetEnumName());
            for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
            {
                string Master_GUID = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                string 藥碼 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 藥名 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                //if (Function_取藥堆疊資料_取得作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.複盤))
                //{
                //    voice.SpeakOnTask("請輸入盤點數量");
                //    Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel($"藥碼:{藥碼} 藥名:{藥名}  請輸入盤點數量");
                //    dialog_NumPannel.ShowDialog();
                //    list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.盤點量] = dialog_NumPannel.Value.ToString();
                //    list_取藥堆疊母資料_replace.Add(list_取藥堆疊母資料[i]);
                //}

                list_取藥堆疊子資料_buf = list_取藥堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);
                for (int k = 0; k < list_取藥堆疊子資料_buf.Count; k++)
                {
                    list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                    list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.調劑結束] = true.ToString();
                    list_取藥堆疊子資料_replace.Add(list_取藥堆疊子資料_buf[k]);
                }
            }
            if (list_取藥堆疊母資料_replace.Count > 0) sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, list_取藥堆疊母資料_replace);
            if (list_取藥堆疊子資料_replace.Count > 0) sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_取藥堆疊子資料_replace);
        }


        static public void Function_從SQL取得儲位到入賬資料(string 藥品碼)
        {
            List<object> list_value = new List<object>();
            List<Box> boxes = List_EPD583_雲端資料.SortByCode(藥品碼);
            List<Storage> storages = List_EPD266_雲端資料.SortByCode(藥品碼);
            List<Storage> pannels = List_Pannel35_雲端資料.SortByCode(藥品碼);
            List<RowsDevice> rowsDevices = List_RowsLED_雲端資料.SortByCode(藥品碼);

            for (int i = 0; i < storages.Count; i++)
            {
                Storage storage = storageUI_EPD_266.SQL_GetStorage(storages[i]);
                List_EPD266_入賬資料.Add_NewStorage(storage);
            }
            for (int i = 0; i < boxes.Count; i++)
            {
                Drawer drawer = drawerUI_EPD_583.SQL_GetDrawer(boxes[i]);
                List_EPD583_入賬資料.Add_NewDrawer(drawer);
            }

            for (int i = 0; i < pannels.Count; i++)
            {
                Storage pannel = storageUI_WT32.SQL_GetStorage(pannels[i]);
                List_Pannel35_入賬資料.Add_NewStorage(pannel);
            }
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                RowsLED rowsLED = rowsLEDUI.SQL_GetRowsLED(rowsDevices[i].IP);
                List_RowsLED_入賬資料.Add_NewRowsLED(rowsLED);
            }

        }
        static public int Function_從入賬資料取得庫存(string 藥品碼)
        {
            int 庫存 = 0;
            List<object> list_value = new List<object>();
            List<string> 儲位_TYPE = new List<string>();
            Function_從入賬資料取得儲位(藥品碼, ref 儲位_TYPE, ref list_value);

            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] is Device)
                {
                    庫存 += ((Device)list_value[i]).Inventory.StringToInt32();
                }
            }
            if (list_value.Count == 0) return -999;
            return 庫存;
        }

        static List<object[]> list_LCD114_索引表 = new List<object[]>();
        static public  string Funcion_取得LCD114索引表_index_IP(string IP)
        {
            List<object[]> list_LCD114_索引表_buf = list_LCD114_索引表.GetRows((int)enum_LCD114_索引表.IP, IP);

            if (list_LCD114_索引表_buf.Count == 0) return "";
            return list_LCD114_索引表_buf[0][(int)enum_LCD114_索引表.index_IP].ObjectToString();
        }
  
        #region PLC_取藥堆疊資料_檢查資料
        static public bool PLC_Device_取藥堆疊資料_檢查資料 = false;
        static public bool PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料 = false;
        static public MyTimer MyTimer_取藥堆疊資料_刷新時間 = new MyTimer(100);
        static public MyTimer MyTimer_取藥堆疊資料_自動過帳時間 = new MyTimer(100);
        static public MyTimer MyTimer_取藥堆疊資料_資料更新時間 = new MyTimer(100);
        static public int cnt_Program_取藥堆疊資料_檢查資料 = 65534;
        static public void sub_Program_取藥堆疊資料_檢查資料()
        {
            //MyThread_取藥堆疊資料_檢查資料.GetCycleTime(100, label_取要推疊_資料更新時間);
            PLC_Device_取藥堆疊資料_檢查資料 = true;
            if (cnt_Program_取藥堆疊資料_檢查資料 == 65534)
            {

                cnt_Program_取藥堆疊資料_檢查資料 = 65535;
            }
            if (cnt_Program_取藥堆疊資料_檢查資料 == 65535) cnt_Program_取藥堆疊資料_檢查資料 = 1;
            if (cnt_Program_取藥堆疊資料_檢查資料 == 1) cnt_Program_取藥堆疊資料_檢查資料_檢查按下(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 2) cnt_Program_取藥堆疊資料_檢查資料_初始化(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 3) cnt_Program_取藥堆疊資料_檢查資料_檢查新增資料(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 4) cnt_Program_取藥堆疊資料_檢查資料_檢查儲位刷新(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 5) cnt_Program_取藥堆疊資料_檢查資料_檢查儲位亮燈(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 6) cnt_Program_取藥堆疊資料_檢查資料_檢查系統領藥(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 7) cnt_Program_取藥堆疊資料_檢查資料_刷新新增效期(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 8) cnt_Program_取藥堆疊資料_檢查資料_堆疊資料整理(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 9) cnt_Program_取藥堆疊資料_檢查資料_從SQL讀取儲位資料(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 10) cnt_Program_取藥堆疊資料_檢查資料_刷新無庫存(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 11) cnt_Program_取藥堆疊資料_檢查資料_刷新資料(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 12) cnt_Program_取藥堆疊資料_檢查資料_設定致能(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 13) cnt_Program_取藥堆疊資料_檢查資料_等待刷新時間到達(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 14) cnt_Program_取藥堆疊資料_檢查資料 = 65500;
            if (cnt_Program_取藥堆疊資料_檢查資料 > 1) cnt_Program_取藥堆疊資料_檢查資料_檢查放開(ref cnt_Program_取藥堆疊資料_檢查資料);
            if (cnt_Program_取藥堆疊資料_檢查資料 == 65500)
            {
                PLC_Device_取藥堆疊資料_檢查資料 = false;
                cnt_Program_取藥堆疊資料_檢查資料 = 65535;
            }
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_檢查按下(ref int cnt)
        {
            if (PLC_Device_取藥堆疊資料_檢查資料) cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取藥堆疊資料_檢查資料) cnt = 65500;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_初始化(ref int cnt)
        {
            MyTimer_取藥堆疊資料_資料更新時間.TickStop();
            MyTimer_取藥堆疊資料_資料更新時間.StartTickTime(9999999);



            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_檢查新增資料(ref int cnt)
        {
            List<object[]> list_value = sQLControl_取藥堆疊母資料.GetRowsByDefult(null, (int)enum_取藥堆疊母資料.狀態, "新增資料");

            if (list_value.Count > 0)
            {
                List<string> names = (from temp in list_value
                                      select temp[(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString()).Distinct().ToList();

                for (int i = 0; i < names.Count; i++)
                {
                    Function_取藥堆疊資料_刪除指定調劑台名稱母資料(names[i]);
                }

                sQLControl_取藥堆疊母資料.DeleteExtra(null, list_value);
                Console.WriteLine($"刪除[新增資料]共<{list_value.Count}>筆");
                List<takeMedicineStackClass> takeMedicineStackClasses = list_value.SQLToClass<takeMedicineStackClass, enum_取藥堆疊母資料>();
                Function_取藥堆疊資料_新增母資料(takeMedicineStackClasses);
            }

            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_檢查儲位刷新(ref int cnt)
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------
            //檢查系統領藥是否資料是否到達時間
            list_取藥堆疊母資料 = sQLControl_取藥堆疊母資料.GetAllRows(null);
            list_取藥堆疊母資料 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, "刷新面板");
            List<object[]> list_取藥堆疊母資料_delete = new List<object[]>();
            int 刷新時間 = 2;
            for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
            {
                DateTime dt_start = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.操作時間].ObjectToString().StringToDateTime();
                DateTime dt_end = DateTime.Now;
                TimeSpan ts = dt_end - dt_start;
                if (ts.TotalSeconds >= 刷新時間)
                {
                    list_取藥堆疊母資料_delete.Add(list_取藥堆疊母資料[i]);
                    string 藥品碼 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    string IP = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.IP].ObjectToString();

                    if (藥品碼.StringIsEmpty() == false)
                    {
                        if (藥品碼.StringIsEmpty() == false && 藥品碼 != "None")
                        {
                            Task.Run(() =>
                            {
                                Function_從SQL取得儲位到本地資料(藥品碼);
                                Function_儲位刷新(藥品碼);
                            });
                        }
                        if (IP.StringIsEmpty() == false)
                        {
                            Task.Run(() =>
                            {
                                //Function_從SQL取得儲位到本地資料(IP);
                                Function_儲位刷新_byIP(IP);
                            });
                        }
                    }

                }
            }
            if (list_取藥堆疊母資料_delete.Count > 0) sQLControl_取藥堆疊母資料.DeleteExtra(null, list_取藥堆疊母資料_delete);

            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_檢查儲位亮燈(ref int cnt)
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------
            //檢查系統領藥是否資料是否到達時間
            List<object[]> list_取藥堆疊母資料 = sQLControl_取藥堆疊母資料.GetAllRows(null);
            list_取藥堆疊母資料 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, "儲位亮燈");
            List<object[]> list_取藥堆疊母資料_buf_未亮燈 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.備註, "");
            List<object[]> list_取藥堆疊母資料_buf_已亮燈 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.備註, "已亮燈");
            List<object[]> list_取藥堆疊母資料_delete = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_retplace = new List<object[]>();
            int 刷新時間 = 10;
            if (list_取藥堆疊母資料_buf_未亮燈.Count > 0 && list_取藥堆疊母資料_buf_已亮燈.Count > 0)
            {
                for (int i = 0; i < list_取藥堆疊母資料_buf_已亮燈.Count; i++)
                {
                    list_取藥堆疊母資料_delete.Add(list_取藥堆疊母資料_buf_已亮燈[i]);
                    string 藥品碼 = list_取藥堆疊母資料_buf_已亮燈[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    string 顏色 = list_取藥堆疊母資料_buf_已亮燈[i][(int)enum_取藥堆疊母資料.顏色].ObjectToString();
                    if (藥品碼.StringIsEmpty() == false)
                    {
                        Task.Run(() =>
                        {
                            Function_儲位亮燈(new LightOn(藥品碼, Color.Black));
                        });
                    }
                }
                if (list_取藥堆疊母資料_delete.Count > 0)
                {


                    sQLControl_取藥堆疊母資料.DeleteExtra(null, list_取藥堆疊母資料_delete);
                    for (int i = 0; i < list_取藥堆疊母資料_delete.Count; i++)
                    {
                        string 藥品碼 = list_取藥堆疊母資料_delete[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                        List<object[]> list_value_sub_take = sQLControl_取藥堆疊子資料.GetRowsByDefult(null, (int)enum_取藥堆疊子資料.Master_GUID, list_取藥堆疊母資料_delete[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString());
                        sQLControl_取藥堆疊子資料.DeleteExtra(null, list_value_sub_take);
                        Console.WriteLine($"儲位亮燈-刪除資料 藥品碼 : {藥品碼}");

                    }

                    return;
                }
            }
            list_取藥堆疊母資料_delete.Clear();
            for (int i = 0; i < list_取藥堆疊母資料_buf_已亮燈.Count; i++)
            {
                string 藥品碼 = list_取藥堆疊母資料_buf_已亮燈[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 顏色 = list_取藥堆疊母資料_buf_已亮燈[i][(int)enum_取藥堆疊母資料.顏色].ObjectToString();
                DateTime dt_start = list_取藥堆疊母資料_buf_已亮燈[i][(int)enum_取藥堆疊母資料.操作時間].ObjectToString().StringToDateTime();
                DateTime dt_end = DateTime.Now;
                TimeSpan ts = dt_end - dt_start;
                if (ts.TotalSeconds >= 刷新時間)
                {
                    list_取藥堆疊母資料_delete.Add(list_取藥堆疊母資料_buf_已亮燈[i]);
                    if (藥品碼.StringIsEmpty() == false)
                    {
                        Task.Run(() =>
                        {
                            Function_儲位亮燈(new LightOn(藥品碼, Color.Black));
                        });
                    }

                }

            }
            if (list_取藥堆疊母資料_delete.Count > 0)
            {
                sQLControl_取藥堆疊母資料.DeleteExtra(null, list_取藥堆疊母資料_delete);
                for (int i = 0; i < list_取藥堆疊母資料_delete.Count; i++)
                {
                    string 藥品碼 = list_取藥堆疊母資料_delete[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    List<object[]> list_value_sub_take = sQLControl_取藥堆疊子資料.GetRowsByDefult(null, (int)enum_取藥堆疊子資料.Master_GUID, list_取藥堆疊母資料_delete[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString());
                    sQLControl_取藥堆疊子資料.DeleteExtra(null, list_value_sub_take);
                    Console.WriteLine($"儲位亮燈-刪除資料 藥品碼 : {藥品碼}");

                }
                return;
            }

            for (int i = 0; i < list_取藥堆疊母資料_buf_未亮燈.Count; i++)
            {
                list_取藥堆疊母資料_buf_未亮燈[i][(int)enum_取藥堆疊母資料.備註] = "已亮燈";
                list_取藥堆疊母資料_retplace.Add(list_取藥堆疊母資料_buf_未亮燈[i]);
                string 藥品碼 = list_取藥堆疊母資料_buf_未亮燈[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                string 顏色 = list_取藥堆疊母資料_buf_未亮燈[i][(int)enum_取藥堆疊母資料.顏色].ObjectToString();
                if (藥品碼.StringIsEmpty() == false)
                {
                    Task.Run(() =>
                    {
                        Function_儲位亮燈(new LightOn(藥品碼, 顏色.ToColor()));
                    });
                }
            }
            if (list_取藥堆疊母資料_retplace.Count > 0)
            {
                sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, list_取藥堆疊母資料_retplace);
                return;
            }

            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_檢查系統領藥(ref int cnt)
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------
            //檢查領藥是否資料是否到達時間
            list_取藥堆疊母資料 = sQLControl_取藥堆疊母資料.GetAllRows(null);
            list_取藥堆疊母資料 = (from temp in list_取藥堆疊母資料
                            where temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()
                            || temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName()
                            || temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.已領用過.GetEnumName()
                            || temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName()
                            select temp).ToList();
            List<object[]> list_取藥堆疊母資料_delete = new List<object[]>();
            int 處方存在時間_temp = 處方存在時間 / 1000;
            for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
            {
                string code = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                DateTime dt_start = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.操作時間].ObjectToString().StringToDateTime();
                DateTime dt_end = DateTime.Now;
                TimeSpan ts = dt_end - dt_start;
                if (ts.TotalSeconds >= 處方存在時間_temp)
                {
                    Console.WriteLine($"藥碼:{code} 處方時間到達 $ ({ts.TotalSeconds} >= {處方存在時間_temp})");
                    list_取藥堆疊母資料_delete.Add(list_取藥堆疊母資料[i]);

                }
            }
            if (list_取藥堆疊母資料_delete.Count > 0) sQLControl_取藥堆疊母資料.DeleteExtra(null, list_取藥堆疊母資料_delete);

            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_堆疊資料整理(ref int cnt)
        {
            string GUID = "";
            list_取藥堆疊母資料 = sQLControl_取藥堆疊母資料.GetAllRows(null);
            list_取藥堆疊子資料 = sQLControl_取藥堆疊子資料.GetAllRows(null);
            List<object[]> list_取藥堆疊子資料_DeleteValue = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_資料更新 = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_取消作業 = new List<object[]>();

            //檢查更新雲端資料
            list_取藥堆疊母資料_資料更新 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.調劑台名稱, "更新資料");
            for (int i = 0; i < list_取藥堆疊母資料_資料更新.Count; i++)
            {
                GUID = list_取藥堆疊母資料_資料更新[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                list_取藥堆疊母資料.Remove(list_取藥堆疊母資料_資料更新[i]);
                Function_取藥堆疊資料_刪除母資料(GUID);
                PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料 = true;
            }
            if (PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料)
            {

                Function_從SQL取得儲位到雲端資料();
                PLC_Device_取藥堆疊資料_檢查資料_更新儲位資料 = false;
            }

            //檢查取消作業-刪除母資料
            list_取藥堆疊母資料_取消作業 = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.取消作業.GetEnumName());
            for (int i = 0; i < list_取藥堆疊母資料_取消作業.Count; i++)
            {
                GUID = list_取藥堆疊母資料_取消作業[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                list_取藥堆疊母資料.Remove(list_取藥堆疊母資料_取消作業[i]);
                Function_取藥堆疊資料_刪除母資料(GUID);
            }

            //檢查無效資料-刪除子資料
            for (int i = 0; i < list_取藥堆疊子資料.Count; i++)
            {
                GUID = list_取藥堆疊子資料[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                if (list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.GUID, GUID).Count == 0)
                {
                    list_取藥堆疊子資料_DeleteValue.Add(list_取藥堆疊子資料[i]);
                }

            }
            for (int i = 0; i < list_取藥堆疊子資料_DeleteValue.Count; i++)
            {
                Function_取藥堆疊資料_刪除子資料(list_取藥堆疊子資料_DeleteValue[i][(int)enum_取藥堆疊子資料.GUID].ObjectToString(), true);
            }
            list_取藥堆疊子資料 = sQLControl_取藥堆疊子資料.GetAllRows(null);
            list_取藥堆疊子資料.Sort(new Icp_取藥堆疊子資料_index排序());
            list_取藥堆疊母資料.Sort(new Icp_取藥堆疊母資料_index排序());
            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_刷新新增效期(ref int cnt)
        {
            List<object[]> _list_取藥堆疊母資料 = sQLControl_取藥堆疊母資料.GetAllRows(null);
            if (_list_取藥堆疊母資料.Count > 0)
            {
                List<object[]> list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
                List<object[]> list_取藥堆疊母資料_buf = new List<object[]>();
                List<string> TYPE = new List<string>();
                List<object> values = new List<object>();
                string 藥品碼 = "";
                string 異動量 = "";
                string 效期 = "";
                string 批號 = "";
                string IP = "";
                list_取藥堆疊母資料_buf = _list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.新增效期.GetEnumName());
                for (int i = 0; i < list_取藥堆疊母資料_buf.Count; i++)
                {
                    藥品碼 = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    效期 = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.效期].ObjectToString();
                    批號 = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.批號].ObjectToString();
                    IP = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.IP].ObjectToString();
                    Function_從SQL取得儲位到雲端資料(藥品碼);
                    Function_從雲端資料取得儲位(藥品碼, ref TYPE, ref values);
                    for (int k = 0; k < values.Count; k++)
                    {
                        if (TYPE[k] == DeviceType.EPD266_lock.GetEnumName() || TYPE[k] == DeviceType.EPD266.GetEnumName()
                            || TYPE[k] == DeviceType.EPD290_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290.GetEnumName()
                            || TYPE[k] == DeviceType.EPD290G_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290G.GetEnumName()
                            || TYPE[k] == DeviceType.EPD420G_lock.GetEnumName() || TYPE[k] == DeviceType.EPD420G_lock.GetEnumName()
                            || TYPE[k] == DeviceType.EPD360E_lock.GetEnumName() || TYPE[k] == DeviceType.EPD360E.GetEnumName())
                        {

                            Storage storage = (Storage)values[k];
                            if (storage.取得庫存(效期) == -1)
                            {
                                if (!IP.StringIsEmpty())
                                {
                                    if (storage.IP != IP) continue;
                                }
                                storage.新增效期(效期, 批號, "00");
                                List_EPD266_雲端資料.Add_NewStorage(storage);
                                //storageUI_EPD_266.SQL_ReplaceStorage(storage);
                                break;
                            }

                        }
                        else if (TYPE[k] == DeviceType.Pannel35.GetEnumName() || TYPE[k] == DeviceType.Pannel35_lock.GetEnumName())
                        {

                            Storage storage = (Storage)values[k];
                            if (storage.取得庫存(效期) == -1)
                            {
                                if (!IP.StringIsEmpty())
                                {
                                    if (storage.IP != IP) continue;
                                }
                                storage.新增效期(效期, 批號, "00");
                                List_Pannel35_雲端資料.Add_NewStorage(storage);
                                //storageUI_WT32.SQL_ReplaceStorage(storage);
                                break;
                            }

                        }
                        else if (TYPE[k] == DeviceType.EPD583_lock.GetEnumName() || TYPE[k] == DeviceType.EPD583.GetEnumName()
                             || TYPE[k] == DeviceType.EPD730E.GetEnumName() || TYPE[k] == DeviceType.EPD730E_lock.GetEnumName())
                        {
                            Box box = (Box)values[k];
                            if (!IP.StringIsEmpty())
                            {
                                if (box.IP != IP) continue;
                            }
                            if (box.取得庫存(效期) == -1)
                            {
                                box.新增效期(效期, 批號, "00");
                                Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                                drawer.ReplaceBox(box);
                                List_EPD583_雲端資料.Add_NewDrawer(drawer);
                                //drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
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
                            if (rowsDevice.取得庫存(效期) == -1)
                            {
                                rowsDevice.新增效期(效期, 批號, "00");
                                RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(rowsDevice.IP);
                                rowsLED.ReplaceRowsDevice(rowsDevice);
                                List_RowsLED_雲端資料.Add_NewRowsLED(rowsDevice);
                                //rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                                break;
                            }
                        }

                    }

                    list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName();
                    list_取藥堆疊母資料_ReplaceValue.Add(list_取藥堆疊母資料_buf[i]);
                }
                if (list_取藥堆疊母資料_ReplaceValue.Count > 0) sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, list_取藥堆疊母資料_ReplaceValue);
            }
            cnt++;

        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_從SQL讀取儲位資料(ref int cnt)
        {
            list_取藥堆疊母資料 = sQLControl_取藥堆疊母資料.GetAllRows(null);
            list_取藥堆疊母資料 = (from temp in list_取藥堆疊母資料
                            where temp[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != "新增資料"
                            select temp).ToList();
            if (list_取藥堆疊母資料.Count > 0)
            {
                var Code_LINQ = (from value in list_取藥堆疊母資料
                                 select value[(int)enum_取藥堆疊母資料.藥品碼]).ToList().Distinct();
                List<object> list_code = Code_LINQ.ToList();
                for (int i = 0; i < list_code.Count; i++)
                {
                    Function_從SQL取得儲位到雲端資料(list_code[i].ObjectToString());
                }

                List<object[]> list_取藥堆疊母資料_buf = new List<object[]>();
                List<object[]> list_藥品設定表 = sQLControl_藥品設定表.GetAllRows(null);
                bool flag_獨立作業 = false;
                bool flag_雙人覆核 = false;
                string GUID = "";
                string 藥品碼 = "";
                string 調劑台名稱 = "";
                int 總異動量 = 0;
                int 庫存量 = 0;
                int 結存量 = 0;
                List<string> lock_ip = new List<string>();
                for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
                {
                    GUID = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                    藥品碼 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    調劑台名稱 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString();
                    總異動量 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToInt32();
                    庫存量 = Function_從雲端資料取得庫存(藥品碼);
                    結存量 = (庫存量 + 總異動量);
                    flag_獨立作業 = Function_取藥堆疊資料_取得作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.獨立作業);
                    flag_雙人覆核 = Function_取藥堆疊資料_取得作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.雙人覆核);
                    if (庫存量 == -999)
                    {

                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()) continue;
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.無儲位.GetEnumName();
                        Color color = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                        if (flag_系統取藥模式)
                        {
                            Function_取藥堆疊資料_新增子資料(GUID, "", 調劑台名稱, 藥品碼, "", "", "", "", "", "0");
                            Function_儲位亮燈(new LightOn(藥品碼, color));
                        }
                        sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, list_取藥堆疊母資料[i]);
                        return;
                    }
                    if (結存量 < 0)
                    {
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName()) continue;
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName();
                        Function_取藥堆疊資料_設定作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.庫存不足語音提示);
                        sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, list_取藥堆疊母資料[i]);
                        return;
                    }
                    if (flag_獨立作業)
                    {
                        if (flag_雙人覆核)
                        {
                            if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName())
                            {
                                list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.雙人覆核.GetEnumName();
                                sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, list_取藥堆疊母資料[i]);
                                return;
                            }
                        }
                        bool flag_單循環取藥 = false;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.雙人覆核.GetEnumName()) flag_單循環取藥 = true;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待作業.GetEnumName()) flag_單循環取藥 = true;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName()) flag_單循環取藥 = true;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName()) flag_單循環取藥 = true;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待複盤.GetEnumName()) flag_單循環取藥 = true;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.盲盤完成.GetEnumName()) flag_單循環取藥 = true;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.複盤完成.GetEnumName()) flag_單循環取藥 = true;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.作業完成.GetEnumName()) flag_單循環取藥 = true;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.選擇效期.GetEnumName()) flag_單循環取藥 = true;
                        if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.輸入新效期.GetEnumName()) flag_單循環取藥 = true;
                        if (flag_單循環取藥)
                        {
                            list_取藥堆疊母資料_buf.Add(list_取藥堆疊母資料[i]);
                            list_取藥堆疊母資料 = list_取藥堆疊母資料_buf;
                            cnt++;
                            return;
                        }
                    }
                }
            }
            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_刷新無庫存(ref int cnt)
        {
            if (list_取藥堆疊母資料.Count > 0)
            {

                List<object[]> list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
                List<object[]> list_取藥堆疊母資料_buf = new List<object[]>();
                List<string> TYPE = new List<string>();
                List<object> values = new List<object>();
                string 藥品碼 = "";
                string 異動量 = "";
                string 效期 = "";
                string 批號 = "";
                string IP = "";
                list_取藥堆疊母資料_buf = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.狀態, enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName());
                for (int i = 0; i < list_取藥堆疊母資料_buf.Count; i++)
                {
                    藥品碼 = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    效期 = "2200/01/01";
                    批號 = "自動補足";
                    IP = list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.IP].ObjectToString();
                    Function_從SQL取得儲位到雲端資料(藥品碼);

                    Function_從雲端資料取得儲位(藥品碼, ref TYPE, ref values);
                    for (int k = 0; k < values.Count; k++)
                    {
                        if (TYPE[k] == DeviceType.EPD266_lock.GetEnumName() || TYPE[k] == DeviceType.EPD266.GetEnumName()
                            || TYPE[k] == DeviceType.EPD290_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290.GetEnumName()
                            || TYPE[k] == DeviceType.EPD290G_lock.GetEnumName() || TYPE[k] == DeviceType.EPD290G.GetEnumName()
                            || TYPE[k] == DeviceType.EPD420G.GetEnumName() || TYPE[k] == DeviceType.EPD420G_lock.GetEnumName()
                            || TYPE[k] == DeviceType.EPD360E.GetEnumName() || TYPE[k] == DeviceType.EPD360E_lock.GetEnumName())
                        {

                            Storage storage = (Storage)values[k];
                            if (!IP.StringIsEmpty())
                            {
                                if (storage.IP != IP) continue;
                            }
                            storage.新增效期(效期, 批號, "100000");
                            List_EPD266_雲端資料.Add_NewStorage(storage);
                            storageUI_EPD_266.SQL_ReplaceStorage(storage);
                            break;

                        }
                        else if (TYPE[k] == DeviceType.Pannel35.GetEnumName() || TYPE[k] == DeviceType.Pannel35_lock.GetEnumName())
                        {

                            Storage storage = (Storage)values[k];
                            if (!IP.StringIsEmpty())
                            {
                                if (storage.IP != IP) continue;
                            }
                            storage.新增效期(效期, 批號, "100000");
                            List_Pannel35_雲端資料.Add_NewStorage(storage);
                            storageUI_WT32.SQL_ReplaceStorage(storage);
                            break;

                        }
                        else if (TYPE[k] == DeviceType.EPD583_lock.GetEnumName() || TYPE[k] == DeviceType.EPD583.GetEnumName()
                            || TYPE[k] == DeviceType.EPD730E.GetEnumName() || TYPE[k] == DeviceType.EPD730E_lock.GetEnumName())
                        {

                            Box box = (Box)values[k];
                            if (!IP.StringIsEmpty())
                            {
                                if (box.IP != IP) continue;
                            }
                            box.新增效期(效期, 批號, "100000");
                            Drawer drawer = List_EPD583_雲端資料.SortByIP(box.IP);
                            drawer.ReplaceBox(box);
                            List_EPD583_雲端資料.Add_NewDrawer(drawer);
                            drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                            break;

                        }

                        else if (TYPE[k] == DeviceType.RowsLED.GetEnumName())
                        {
                            RowsDevice rowsDevice = values[k] as RowsDevice;
                            if (!IP.StringIsEmpty())
                            {
                                if (rowsDevice.IP != IP) continue;
                            }
                            rowsDevice.新增效期(效期, 批號, "100000");
                            RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(rowsDevice.IP);
                            rowsLED.ReplaceRowsDevice(rowsDevice);
                            List_RowsLED_雲端資料.Add_NewRowsLED(rowsDevice);
                            rowsLEDUI.SQL_ReplaceRowsLED(rowsLED);
                            break;
                        }

                    }

                    list_取藥堆疊母資料_buf[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.等待刷新.GetEnumName();
                    list_取藥堆疊母資料_ReplaceValue.Add(list_取藥堆疊母資料_buf[i]);
                }
                if (list_取藥堆疊母資料_ReplaceValue.Count > 0) sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, list_取藥堆疊母資料_ReplaceValue);
            }
            cnt++;

        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_刷新資料(ref int cnt)
        {
            if (list_取藥堆疊母資料.Count > 0)
            {
                string 藥品碼 = "";
                string 調劑台名稱 = "";
                string GUID = "";
                string 效期 = "";
                string 批號 = "";
                string IP = "";
                int 總異動量 = 0;
                int 庫存量 = 0;
                int 結存量 = 0;
                bool flag_取藥堆疊母資料_Update = false;
                List<object[]> list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
                List<object[]> list_取藥堆疊母資料_DeleteValue = new List<object[]>();
                List<object[]> list_取藥堆疊子資料_buf = new List<object[]>();
                List<object[]> list_取藥堆疊子資料_DeleteValue = new List<object[]>();
                List<object[]> list_取藥堆疊子資料_ReplaceValue = new List<object[]>();


                list_取藥堆疊母資料 = (from value in list_取藥堆疊母資料
                                where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.None.GetEnumName()
                                where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName()
                                where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName()
                                where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.新增效期.GetEnumName()
                                where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName()
                                where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.選擇效期.GetEnumName()
                                where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.雙人覆核.GetEnumName()
                                where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.已領用過.GetEnumName()
                                where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()
                                select value).ToList();

                for (int i = 0; i < list_取藥堆疊母資料.Count; i++)
                {
                    flag_取藥堆疊母資料_Update = false;
                    GUID = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                    調劑台名稱 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.調劑台名稱].ObjectToString();
                    藥品碼 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                    總異動量 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToInt32();
                    庫存量 = Function_從雲端資料取得庫存(藥品碼);
                    結存量 = (庫存量 + 總異動量);
                    效期 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.效期].ObjectToString();
                    批號 = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.批號].ObjectToString();
                    IP = list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.IP].ObjectToString();
                    if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.庫存量].ObjectToString() != 庫存量.ToString())
                    {
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.庫存量] = 庫存量.ToString();
                        flag_取藥堆疊母資料_Update = true;
                    }
                    if (list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.結存量].ObjectToString() != 結存量.ToString())
                    {
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.結存量] = 結存量.ToString();
                        flag_取藥堆疊母資料_Update = true;
                    }


                    //找無儲位
                    if (庫存量 == -999)
                    {
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.無儲位.GetEnumName();
                        flag_取藥堆疊母資料_Update = true;
                    }
                    //無庫存
                    else if (結存量 < 0 || 庫存量 == 0)
                    {
                        list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName();
                        Function_儲位亮燈_Ex(new LightOn(藥品碼, Color.Black));
                        // Function_取藥堆疊資料_設定作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.庫存不足語音提示);
                        flag_取藥堆疊母資料_Update = true;
                    }
                    //更新取藥子堆疊資料
                    else if (總異動量 == 0 || 庫存量 >= 0)
                    {
                        if (Function_取藥堆疊資料_取得作業模式(list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.效期管控) && 效期.StringIsEmpty())
                        {
                            list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.選擇效期.GetEnumName();
                            flag_取藥堆疊母資料_Update = true;
                        }
                        else
                        {
                            List<object[]> 儲位資訊 = new List<object[]>();
                            string 儲位資訊_TYPE = "";
                            string 儲位資訊_IP = "";
                            string 儲位資訊_Num = "";
                            string 儲位資訊_效期 = "";
                            string 儲位資訊_批號 = "";

                            string 儲位資訊_庫存 = "";
                            string 儲位資訊_異動量 = "";
                            string 儲位資訊_GUID = "";
                            list_取藥堆疊子資料_buf = list_取藥堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, GUID);


                            if (效期.StringIsEmpty())
                            {
                                儲位資訊 = Function_取得異動儲位資訊從雲端資料(藥品碼, 總異動量);
                            }
                            else
                            {
                                if (IP.StringIsEmpty())
                                {
                                    儲位資訊 = Function_取得異動儲位資訊從雲端資料(藥品碼, 總異動量, 效期);
                                }
                                else
                                {
                                    儲位資訊 = Function_取得異動儲位資訊從雲端資料(藥品碼, 總異動量, 效期, IP);
                                }
                            }


                            if (儲位資訊.Count == 0 && 結存量 > 0)
                            {
                                List<string> list_效期 = new List<string>();
                                List<string> list_批號 = new List<string>();
                                if (效期.StringIsEmpty())
                                {
                                    Funnction_交易記錄查詢_取得指定藥碼批號期效期(藥品碼, ref list_效期, ref list_批號);
                                    if (list_效期.Count > 0 && list_批號.Count > 0)
                                    {
                                        效期 = list_效期[0];
                                        批號 = list_批號[0];
                                    }
                                }

                                儲位資訊 = Function_新增效期至雲端資料(藥品碼, 總異動量, 效期, 批號);
                            }

                            List<object[]> list_sortValue = new List<object[]>();
                            //檢查子資料新增及修改
                            for (int m = 0; m < list_取藥堆疊子資料_buf.Count; m++)
                            {
                                bool flag_Delete = true;
                                for (int k = 0; k < 儲位資訊.Count; k++)
                                {
                                    Function_取藥堆疊資料_取得儲位資訊內容(儲位資訊[k], ref 儲位資訊_GUID, ref 儲位資訊_TYPE, ref 儲位資訊_IP, ref 儲位資訊_Num, ref 儲位資訊_效期, ref 儲位資訊_批號, ref 儲位資訊_庫存, ref 儲位資訊_異動量);
                                    if (list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == 儲位資訊_TYPE)
                                        if (list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.IP].ObjectToString() == 儲位資訊_IP)
                                            if (list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.Num].ObjectToString() == 儲位資訊_Num)
                                                if (list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.效期].ObjectToString() == 儲位資訊_效期)
                                                {
                                                    flag_Delete = false;
                                                    break;
                                                }
                                }
                                if (flag_Delete)
                                {
                                    Function_取藥堆疊資料_刪除子資料(list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.GUID].ObjectToString(), false);
                                }
                            }
                            for (int k = 0; k < 儲位資訊.Count; k++)
                            {

                                Function_取藥堆疊資料_取得儲位資訊內容(儲位資訊[k], ref 儲位資訊_GUID, ref 儲位資訊_TYPE, ref 儲位資訊_IP, ref 儲位資訊_Num, ref 儲位資訊_效期, ref 儲位資訊_批號, ref 儲位資訊_庫存, ref 儲位資訊_異動量);

                                list_sortValue = (from value in list_取藥堆疊子資料_buf
                                                  where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == 儲位資訊_TYPE
                                                  where value[(int)enum_取藥堆疊子資料.IP].ObjectToString() == 儲位資訊_IP
                                                  where value[(int)enum_取藥堆疊子資料.Num].ObjectToString() == 儲位資訊_Num
                                                  where value[(int)enum_取藥堆疊子資料.效期].ObjectToString() == 儲位資訊_效期
                                                  select value).ToList();
                                if (list_sortValue.Count != 1)
                                {
                                    for (int m = 0; m < list_sortValue.Count; m++)
                                    {
                                        Function_取藥堆疊資料_刪除子資料(list_取藥堆疊子資料_buf[m][(int)enum_取藥堆疊子資料.GUID].ObjectToString(), false);
                                    }
                                    object[] value = Function_取藥堆疊資料_新增子資料(GUID, 儲位資訊_GUID, 調劑台名稱, 藥品碼, 儲位資訊_IP, 儲位資訊_Num, 儲位資訊_TYPE, 儲位資訊_效期, 儲位資訊_批號, 儲位資訊_異動量);

                                    list_取藥堆疊子資料_buf.Add(value);
                                    Function_庫存異動至雲端資料(儲位資訊[k]);
                                }
                                else
                                {
                                    if (list_sortValue[0][(int)enum_取藥堆疊子資料.異動量].ObjectToString() != 儲位資訊_異動量)
                                    {
                                        list_sortValue[0][(int)enum_取藥堆疊子資料.異動量] = 儲位資訊_異動量;
                                        list_取藥堆疊子資料_ReplaceValue.Add(list_sortValue[0]);
                                    }
                                    if (list_sortValue[0][(int)enum_取藥堆疊子資料.已入帳].ObjectToString() == false.ToString())
                                    {
                                        Function_庫存異動至雲端資料(儲位資訊[k]);
                                    }
                                }
                            }
                        }



                    }



                    if (flag_取藥堆疊母資料_Update)
                    {
                        list_取藥堆疊母資料_ReplaceValue.Add(list_取藥堆疊母資料[i]);
                    }
                    else
                    {
                        Function_取藥堆疊資料_檢查資料儲位正常(list_取藥堆疊母資料[i]);
                    }
                }

                if (list_取藥堆疊母資料_DeleteValue.Count > 0) sQLControl_取藥堆疊母資料.DeleteExtra(null, list_取藥堆疊母資料_DeleteValue);
                if (list_取藥堆疊母資料_ReplaceValue.Count > 0)
                {
                    sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, list_取藥堆疊母資料_ReplaceValue);
                }
                if (list_取藥堆疊子資料_ReplaceValue.Count > 0) sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_取藥堆疊子資料_ReplaceValue);

            }
            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_設定致能(ref int cnt)
        {
            List<object[]> _list_取藥堆疊子資料 = Function_取藥堆疊子資料_取得可致能(ref list_取藥堆疊子資料);
            List<object[]> list_取藥堆疊母資料_buf;
            List<object[]> list_取藥堆疊資料_ReplaceValue = new List<object[]>();
            List<object[]> list_locker_table_value = sQLControl_Locker_Index_Table.GetAllRows(null);
            List<object[]> list_locker_table_value_buf = new List<object[]>();
            List<object[]> list_locker_table_value_ReplaceValue = new List<object[]>();
            List<string> list_lock_IP = new List<string>();
            string IP = "";
            string Slave_GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            string Num = "";
            string 藥品碼 = "";
           
            Color color = Color.Black;

            List<string> list_已亮燈藥碼 = new List<string>();
            List<string> list_已亮燈藥碼_buf = new List<string>();
            _list_取藥堆疊子資料 = _list_取藥堆疊子資料.GetRows((int)enum_取藥堆疊子資料.致能, false.ToString());

            foreach (object[] 取藥堆疊資料 in _list_取藥堆疊子資料)
            {
                IP = 取藥堆疊資料[(int)enum_取藥堆疊子資料.IP].ObjectToString();
                Master_GUID = 取藥堆疊資料[(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                Slave_GUID = 取藥堆疊資料[(int)enum_取藥堆疊子資料.GUID].ObjectToString();
                Device_GUID = 取藥堆疊資料[(int)enum_取藥堆疊子資料.Device_GUID].ObjectToString();
                藥品碼 = 取藥堆疊資料[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();

                list_取藥堆疊母資料_buf = list_取藥堆疊母資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);
                if (list_取藥堆疊母資料_buf.Count > 0) color = list_取藥堆疊母資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();

                取藥堆疊資料[(int)enum_取藥堆疊子資料.致能] = true.ToString();
                list_取藥堆疊資料_ReplaceValue.Add(取藥堆疊資料);

                list_已亮燈藥碼_buf = (from temp in list_已亮燈藥碼
                                  where temp == 藥品碼
                                  select temp).ToList();
                if (list_已亮燈藥碼_buf.Count != 0) continue;

                if (!flag_同藥碼全亮 || flag_系統取藥模式)
                {
                    if (取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD583_lock.GetEnumName()
                        || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD730E_lock.GetEnumName())
                    {
                        if (藥品碼.StringIsEmpty()) return;
                        //Function_儲位亮燈(藥品碼, color, ref list_lock_IP);

                        Drawer drawer = List_EPD583_雲端資料.SortByIP(IP);

                        List<Box> boxes = drawer.SortByCode(藥品碼);
                        if (drawer.IsAllLight)
                        {
                            drawer.LED_Bytes = DrawerUI_EPD_583.Set_LEDBytes(drawer, boxes, color);
                            drawer.LED_Bytes = DrawerUI_EPD_583.Set_Pannel_LEDBytes(drawer, color);
                        }
                        else
                        {
                            drawer.LED_Bytes = DrawerUI_EPD_583.Set_Pannel_LEDBytes(drawer, color);
                        }
                        drawerUI_EPD_583.Set_LED_UDP(drawer);
                        list_已亮燈藥碼.Add(藥品碼);

                        list_locker_table_value_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.IP, IP);
                    }

                    if (取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD266_lock.GetEnumName()
                        || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD266.GetEnumName()
                         || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290_lock.GetEnumName()
                          || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290.GetEnumName()
                            || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290G_lock.GetEnumName()
                              || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290G.GetEnumName()
                                || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420G.GetEnumName()
                                  || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420G_lock.GetEnumName()
                                   || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD360E.GetEnumName()
                                  || 取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD360E_lock.GetEnumName())

                    {
                        Storage storage = List_EPD266_雲端資料.SortByIP(IP);
                        storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                        list_已亮燈藥碼.Add(藥品碼);
                        list_locker_table_value_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.IP, IP);
                    }

                }
                else if (flag_同藥碼全亮)
                {

                    Function_儲位亮燈(new LightOn(藥品碼, color,Math.Abs( 取藥堆疊資料[(int)enum_取藥堆疊子資料.異動量].StringToDouble())), ref list_lock_IP); list_已亮燈藥碼.Add(藥品碼);
                    for (int k = 0; k < list_lock_IP.Count; k++)
                    {
                        list_locker_table_value_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.IP, list_lock_IP[k]);
                        if (list_locker_table_value_buf.Count > 0)
                        {
                            list_locker_table_value_buf[0][(int)enum_lockerIndex.Master_GUID] = Master_GUID;
                            list_locker_table_value_buf[0][(int)enum_lockerIndex.Device_GUID] = Device_GUID;
                            list_locker_table_value_buf[0][(int)enum_lockerIndex.Slave_GUID] = Slave_GUID;
                            list_locker_table_value_buf[0][(int)enum_lockerIndex.輸出狀態] = true.ToString();
                            list_locker_table_value_ReplaceValue.Add(list_locker_table_value_buf[0]);
                        }
                    }

                }




                if (取藥堆疊資料[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.Pannel35_lock.GetEnumName())
                {
                    Storage storage = List_Pannel35_雲端資料.SortByIP(IP);
                    storageUI_WT32.Set_Stroage_LED_UDP(storage, color);
                    list_已亮燈藥碼.Add(藥品碼);

                    list_locker_table_value_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.IP, IP);
                }



                if (list_locker_table_value_buf.Count == 0) continue;
                list_locker_table_value_buf[0][(int)enum_lockerIndex.Master_GUID] = Master_GUID;
                list_locker_table_value_buf[0][(int)enum_lockerIndex.Device_GUID] = Device_GUID;
                list_locker_table_value_buf[0][(int)enum_lockerIndex.Slave_GUID] = Slave_GUID;
                list_locker_table_value_buf[0][(int)enum_lockerIndex.輸出狀態] = true.ToString();
                list_locker_table_value_ReplaceValue.Add(list_locker_table_value_buf[0]);
                Console.WriteLine($"開啟抽屜致能,藥品碼:{藥品碼} {DateTime.Now.ToDateTimeString()}");
            }

            if (list_locker_table_value_ReplaceValue.Count > 0) sQLControl_Locker_Index_Table.UpdateByDefulteExtra(null, list_locker_table_value_ReplaceValue);
            if (list_取藥堆疊資料_ReplaceValue.Count > 0) sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_取藥堆疊資料_ReplaceValue);

            MyTimer_取藥堆疊資料_刷新時間.TickStop();
            MyTimer_取藥堆疊資料_刷新時間.StartTickTime(100);
            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_檢查資料_等待刷新時間到達(ref int cnt)
        {
            if (MyTimer_取藥堆疊資料_刷新時間.IsTimeOut())
            {
                cnt++;
            }
        }
        #endregion
        #region PLC_取藥堆疊資料_狀態更新
        static public bool PLC_Device_取藥堆疊資料_狀態更新 = false;
        static public bool PLC_Device_取藥堆疊資料_狀態更新_OK = false;
        static public int cnt_Program_取藥堆疊資料_狀態更新 = 65534;
        static public void sub_Program_取藥堆疊資料_狀態更新()
        {
            PLC_Device_取藥堆疊資料_狀態更新 = true;
            if (cnt_Program_取藥堆疊資料_狀態更新 == 65534)
            {
                PLC_Device_取藥堆疊資料_狀態更新 = false;
                cnt_Program_取藥堆疊資料_狀態更新 = 65535;
            }
            if (cnt_Program_取藥堆疊資料_狀態更新 == 65535) cnt_Program_取藥堆疊資料_狀態更新 = 1;
            if (cnt_Program_取藥堆疊資料_狀態更新 == 1) cnt_Program_取藥堆疊資料_狀態更新_檢查按下(ref cnt_Program_取藥堆疊資料_狀態更新);
            if (cnt_Program_取藥堆疊資料_狀態更新 == 2) cnt_Program_取藥堆疊資料_狀態更新_初始化(ref cnt_Program_取藥堆疊資料_狀態更新);
            if (cnt_Program_取藥堆疊資料_狀態更新 == 3) cnt_Program_取藥堆疊資料_狀態更新 = 65500;
            if (cnt_Program_取藥堆疊資料_狀態更新 > 1) cnt_Program_取藥堆疊資料_狀態更新_檢查放開(ref cnt_Program_取藥堆疊資料_狀態更新);

            if (cnt_Program_取藥堆疊資料_狀態更新 == 65500)
            {
                PLC_Device_取藥堆疊資料_狀態更新 = false;
                PLC_Device_取藥堆疊資料_狀態更新_OK = false;
                cnt_Program_取藥堆疊資料_狀態更新 = 65535;
            }
        }
        static public void cnt_Program_取藥堆疊資料_狀態更新_檢查按下(ref int cnt)
        {
            if (PLC_Device_取藥堆疊資料_狀態更新) cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_狀態更新_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取藥堆疊資料_狀態更新) cnt = 65500;
        }
        static public void cnt_Program_取藥堆疊資料_狀態更新_初始化(ref int cnt)
        {
            string 狀態 = "";
            string 狀態_buf = "";
            string GUID = "";
            bool 致能 = true;
            bool 流程作業完成 = true;
            bool 配藥完成 = true;
            bool 調劑結束 = true;
            bool 已入帳 = true;
            List<object[]> _list_取藥堆疊母資料 = Function_取藥堆疊資料_取得母資料();
            List<object[]> _list_取藥堆疊子資料 = Function_取藥堆疊資料_取得子資料();
            List<object[]> _list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
            List<object[]> _list_取藥堆疊子資料_ReplaceValue = new List<object[]>();
            List<object[]> _list_取藥堆疊子資料_buf = new List<object[]>();


            _list_取藥堆疊母資料 = (from value in _list_取藥堆疊母資料
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName()
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName()
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.新增效期.GetEnumName()
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.庫存不足.GetEnumName()
                             where value[(int)enum_取藥堆疊母資料.狀態].ObjectToString() != enum_取藥堆疊母資料_狀態.無儲位.GetEnumName()
                             select value).ToList();

            _list_取藥堆疊母資料.Sort(new Icp_取藥堆疊母資料_index排序());
            for (int i = 0; i < _list_取藥堆疊母資料.Count; i++)
            {
                致能 = true;
                流程作業完成 = true;
                配藥完成 = true;
                調劑結束 = true;
                已入帳 = true;
                狀態_buf = 狀態 = _list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態].ObjectToString();
                GUID = _list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                _list_取藥堆疊子資料_buf = _list_取藥堆疊子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, GUID);

                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能].ObjectToString() == false.ToString())
                    {
                        致能 = false;
                        break;
                    }
                }
                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == false.ToString())
                    {
                        流程作業完成 = false;
                        break;
                    }
                }
                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString())
                    {
                        配藥完成 = false;
                        break;
                    }
                }
                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.調劑結束].ObjectToString() == false.ToString())
                    {
                        調劑結束 = false;
                        break;
                    }
                }
                for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                {
                    if (_list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.已入帳].ObjectToString() == false.ToString())
                    {
                        已入帳 = false;
                        break;
                    }
                }
                if (_list_取藥堆疊子資料_buf.Count > 0)
                {
                    if (已入帳) 狀態_buf = enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName();
                    else if (調劑結束) 狀態_buf = enum_取藥堆疊母資料_狀態.等待入賬.GetEnumName();
                    else if (配藥完成) 狀態_buf = enum_取藥堆疊母資料_狀態.作業完成.GetEnumName();
                    else if (致能)
                    {
                        if (Function_取藥堆疊資料_取得作業模式(_list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.複盤)) 狀態_buf = enum_取藥堆疊母資料_狀態.等待複盤.GetEnumName();
                        else if (Function_取藥堆疊資料_取得作業模式(_list_取藥堆疊母資料[i], enum_取藥堆疊母資料_作業模式.盲盤)) 狀態_buf = enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName();
                        else 狀態_buf = enum_取藥堆疊母資料_狀態.等待作業.GetEnumName();
                    }
                    if (_list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.動作].ObjectToString().Contains("系統"))

                    {
                        if (狀態_buf == enum_取藥堆疊母資料_狀態.作業完成.GetEnumName())
                        {
                            狀態_buf = new enum_取藥堆疊母資料_狀態().GetEnumName();
                            for (int k = 0; k < _list_取藥堆疊子資料_buf.Count; k++)
                            {
                                _list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.致能] = true.ToString();
                                _list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                _list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                                _list_取藥堆疊子資料_buf[k][(int)enum_取藥堆疊子資料.調劑結束] = true.ToString();
                                _list_取藥堆疊子資料_ReplaceValue.Add(_list_取藥堆疊子資料_buf[k]);
                            }
                        }
                    }
                }

                if (狀態_buf != 狀態)
                {
                    狀態 = 狀態_buf;

                    _list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.狀態] = 狀態;
                    _list_取藥堆疊母資料[i][(int)enum_取藥堆疊母資料.操作時間] = DateTime.Now.ToDateTimeString_6();
                    _list_取藥堆疊母資料_ReplaceValue.Add(_list_取藥堆疊母資料[i]);
                }



            }
            if (_list_取藥堆疊母資料_ReplaceValue.Count > 0)
            {
                sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, _list_取藥堆疊母資料_ReplaceValue);
            }
            if (_list_取藥堆疊子資料_ReplaceValue.Count > 0)
            {
                sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, _list_取藥堆疊子資料_ReplaceValue);
            }
            cnt++;
        }


        #endregion
        #region PLC_取藥堆疊資料_流程作業檢查
        static public List<object[]> list_流程作業檢查_取藥母堆疊資料 = new List<object[]>();
        static public List<object[]> list_流程作業檢查_取藥子堆疊資料 = new List<object[]>();
        static public bool PLC_Device_取藥堆疊資料_流程作業檢查 = false;
        static public bool PLC_Device_取藥堆疊資料_流程作業檢查_不檢測 = false;
        static public int 取藥堆疊資料_流程作業檢查_感測設定值 = 80;
        static public MyTimer MyTimer_取藥堆疊資料_流程作業檢查 = new MyTimer(100);
        static public MyTimer MyTimer_取藥堆疊資料_流程作業檢查時間 = new MyTimer(100);
        static public int cnt_Program_取藥堆疊資料_流程作業檢查 = 65534;
        static public void sub_Program_取藥堆疊資料_流程作業檢查()
        {
            PLC_Device_取藥堆疊資料_流程作業檢查 = true;
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 65534)
            {
                cnt_Program_取藥堆疊資料_流程作業檢查 = 65535;
            }
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 65535) cnt_Program_取藥堆疊資料_流程作業檢查 = 1;
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 1) cnt_Program_取藥堆疊資料_流程作業檢查_檢查按下(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 2) cnt_Program_取藥堆疊資料_流程作業檢查_初始化(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 3) cnt_Program_取藥堆疊資料_流程作業檢查_檢查盲盤複盤(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 4) cnt_Program_取藥堆疊資料_流程作業檢查_檢查同藥碼全亮(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 5) cnt_Program_取藥堆疊資料_流程作業檢查_檢查層架及手勢感測(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 6) cnt_Program_取藥堆疊資料_流程作業檢查_檢查抽屜手勢感測感應到(ref cnt_Program_取藥堆疊資料_流程作業檢查);           
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 7) cnt_Program_取藥堆疊資料_流程作業檢查 = 65500;
            if (cnt_Program_取藥堆疊資料_流程作業檢查 > 1) cnt_Program_取藥堆疊資料_流程作業檢查_檢查放開(ref cnt_Program_取藥堆疊資料_流程作業檢查);
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 65500)
            {
                MyTimer_取藥堆疊資料_流程作業檢查.TickStop();
                MyTimer_取藥堆疊資料_流程作業檢查.StartTickTime(100);
                cnt_Program_取藥堆疊資料_流程作業檢查 = 65501;
            }
            if (cnt_Program_取藥堆疊資料_流程作業檢查 == 65501)
            {
                if (MyTimer_取藥堆疊資料_流程作業檢查.IsTimeOut())
                {
                    PLC_Device_取藥堆疊資料_流程作業檢查 = false;
                    cnt_Program_取藥堆疊資料_流程作業檢查 = 65535;
                }
            }

            Function_設定配藥完成();
        }
        static public void cnt_Program_取藥堆疊資料_流程作業檢查_檢查按下(ref int cnt)
        {
            if (PLC_Device_取藥堆疊資料_流程作業檢查) cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_流程作業檢查_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取藥堆疊資料_流程作業檢查) cnt = 65500;
        }
        static public void cnt_Program_取藥堆疊資料_流程作業檢查_初始化(ref int cnt)
        {




            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_流程作業檢查_檢查盲盤複盤(ref int cnt)
        {
            string IP = "";
            string 藥品碼 = "";
            string 調劑台名稱 = "";
            string GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            bool flag_TOFON = false;
            Color color = Color.Black;
            list_流程作業檢查_取藥母堆疊資料 = Function_取藥堆疊資料_取得母資料();
            list_流程作業檢查_取藥子堆疊資料 = Function_取藥堆疊資料_取得子資料();

            List<object[]> list_取藥母堆疊資料 = list_流程作業檢查_取藥母堆疊資料;
            List<object[]> list_取藥母堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料 = list_流程作業檢查_取藥子堆疊資料;
            List<object[]> list_取藥子堆疊資料_buf = new List<object[]>();

            list_取藥子堆疊資料_buf = (from value in list_取藥子堆疊資料
                                where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                select value).ToList();

            for (int i = 0; i < list_取藥子堆疊資料_buf.Count; i++)
            {
                GUID = list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.GUID].ObjectToString();
                Master_GUID = list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();

                List<object[]> _list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);

                if (_list_取藥母堆疊資料_buf.Count > 0)
                {
                    bool flag_remove = false;
                    if (_list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待複盤.GetEnumName()) flag_remove = true;
                    if (_list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == enum_取藥堆疊母資料_狀態.等待盲盤.GetEnumName()) flag_remove = true;
                    if (flag_remove)
                    {
                        list_取藥母堆疊資料.RemoveByGUID(_list_取藥母堆疊資料_buf[0]);
                        list_取藥子堆疊資料.RemoveByGUID(GUID);
                    }

                }
            }
            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_流程作業檢查_檢查同藥碼全亮(ref int cnt)
        {
            string IP = "";
            string 藥品碼 = "";
            string 調劑台名稱 = "";
            string GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            bool flag_TOFON = false;
            Color color = Color.Black;
            List<object[]> list_取藥母堆疊資料 = list_流程作業檢查_取藥母堆疊資料;
            List<object[]> list_取藥母堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料 = list_流程作業檢查_取藥子堆疊資料;
            List<object[]> list_取藥子堆疊資料_buf = new List<object[]>();

            if (flag_同藥碼全亮)
            {
                list_取藥子堆疊資料_buf = (from value in list_取藥子堆疊資料
                                    where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                    where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                    select value).ToList();

                List<object[]> list_取藥子堆疊資料_Replace = new List<object[]>();
                for (int i = 0; i < list_取藥子堆疊資料_buf.Count; i++)
                {
                    Master_GUID = list_取藥子堆疊資料[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                    IP = list_取藥子堆疊資料[i][(int)enum_取藥堆疊子資料.IP].ObjectToString();

                    Storage storage = List_EPD266_雲端資料.SortByIP(IP);
                    if (storage != null && (storage.DeviceType == DeviceType.EPD266 || storage.DeviceType == DeviceType.EPD290
                        || storage.DeviceType == DeviceType.EPD290G || storage.DeviceType == DeviceType.EPD420G || storage.DeviceType == DeviceType.EPD360E))
                    {
                        if (!storage.TOFON)
                        {
                            list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                            list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                            list_取藥子堆疊資料_Replace.Add(list_取藥子堆疊資料_buf[i]);
                        }
                        else
                        {
                            flag_TOFON = true;
                        }
                    }
                    //else
                    //{
                    //    list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                    //    list_取藥子堆疊資料_buf[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                    //    list_取藥子堆疊資料_Replace.Add(list_取藥子堆疊資料_buf[i]);
                    //}


                }
                sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_取藥子堆疊資料_Replace);
                MyTimer_取藥堆疊資料_流程作業檢查.TickStop();
                MyTimer_取藥堆疊資料_流程作業檢查.StartTickTime(100);

                //if (!flag_TOFON)
                //{
                //    cnt = 65500;
                //    return;
                //}
                //else
                //{
                //    cnt++;
                //    return;
                //}
            }
            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_流程作業檢查_檢查層架及手勢感測(ref int cnt)
        {
            List<Task> taskList = new List<Task>();
            string IP = "";
            string 藥品碼 = "";
            string 調劑台名稱 = "";
            string GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            bool flag_TOFON = false;
            Color color = Color.Black;
            List<object[]> list_取藥母堆疊資料 = list_流程作業檢查_取藥母堆疊資料;
            List<object[]> list_取藥母堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料 = list_流程作業檢查_取藥子堆疊資料;
            List<object[]> list_取藥子堆疊資料_buf = new List<object[]>();

            list_取藥子堆疊資料_buf = (from value in list_取藥子堆疊資料
                                where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                select value).ToList();




            List<string[]> list_需更新資料;
            List<object[]> list_取藥子堆疊資料_2_66層架_作業未完成 = new List<object[]>();
            List<object[]> list_取藥子堆疊資料_2_66層架_作業已完成 = new List<object[]>();
            List<object[]> list_取藥子堆疊資料_LED層架_作業未完成 = new List<object[]>();
            List<object[]> list_取藥子堆疊資料_LED層架_作業已完成 = new List<object[]>();

            list_取藥子堆疊資料_2_66層架_作業未完成 = (from value in list_取藥子堆疊資料
                                         where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                         where value[(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == false.ToString()
                                         where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                         where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD266.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290G.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420G.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD360E.GetEnumName()
                                         select value).ToList();
            list_取藥子堆疊資料_2_66層架_作業已完成 = (from value in list_取藥子堆疊資料
                                         where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                         where value[(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == true.ToString()
                                         where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                         where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD266.GetEnumName()
                                          || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD290G.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD420G.GetEnumName()
                                         || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD360E.GetEnumName()
                                         select value).ToList();
            list_取藥子堆疊資料_LED層架_作業未完成 = (from value in list_取藥子堆疊資料
                                        where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                        where value[(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == false.ToString()
                                        where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                        where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.RowsLED.GetEnumName()
                                        select value).ToList();
            list_取藥子堆疊資料_LED層架_作業已完成 = (from value in list_取藥子堆疊資料
                                        where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                        where value[(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == true.ToString()
                                        where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                        where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.RowsLED.GetEnumName()
                                        select value).ToList();


            #region 2_66層架_作業未完成
            taskList = new List<Task>();
            list_需更新資料 = new List<string[]>();
            foreach (object[] value in list_取藥子堆疊資料_2_66層架_作業未完成)
            {
                IP = value[(int)enum_取藥堆疊子資料.IP].ObjectToString();
                藥品碼 = value[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = value[(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();
                Master_GUID = value[(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();

                list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);

                if (list_取藥母堆疊資料_buf.Count > 0)
                {
                    color = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                    int match = (from values in list_需更新資料
                                 where values[0] == 調劑台名稱
                                 where values[1] == IP
                                 select values).ToList().Count;
                    if (match == 0)
                    {
                        list_需更新資料.Add(new string[] { 調劑台名稱, IP });
                        Storage storage = List_EPD266_雲端資料.SortByIP(IP);
                        if (storage != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color);
                            }));
                        }
                    }
                }
            }
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();

            for (int i = 0; i < list_需更新資料.Count; i++)
            {
                Function_取藥堆疊子資料_設定流程作業完成ByIP(list_需更新資料[i][0], list_需更新資料[i][1]);
            }
            #endregion
            #region 2_66層架_作業已完成
            taskList = new List<Task>();
            list_需更新資料 = new List<string[]>();
            List<string[]> list_手勢檢查資料 = new List<string[]>();
            foreach (object[] value in list_取藥子堆疊資料_2_66層架_作業已完成)
            {
                IP = value[(int)enum_取藥堆疊子資料.IP].ObjectToString();
                藥品碼 = value[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = value[(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();
                Master_GUID = value[(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);
                if (list_取藥母堆疊資料_buf.Count > 0)
                {
                    color = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                    int match = (from values in list_手勢檢查資料
                                 where values[0] == 調劑台名稱
                                 where values[1] == IP
                                 select values).ToList().Count;
                    if (match == 0)
                    {
                        Storage storage = List_EPD266_雲端資料.SortByIP(IP);
                        if (storage != null)
                        {
                            taskList.Add(Task.Run(() =>
                            {
                                int Dis_value = storageUI_EPD_266.Get_LaserDistance(storage);
                                Console.WriteLine($"IP: {storage.IP} ,雷射數值 :{Dis_value}");
                                if (Dis_value <= 取藥堆疊資料_流程作業檢查_感測設定值 || PLC_Device_取藥堆疊資料_流程作業檢查_不檢測 || !storage.TOFON)
                                {
                                    //if (!PLC_Device_取藥堆疊資料_流程作業檢查_不檢測 || !storage.TOFON) storageUI_EPD_266.Set_Stroage_LED_UDP(storage, Color.Black);
                                    list_需更新資料.Add(new string[] { 調劑台名稱, IP });
                                }

                            }));
                            list_手勢檢查資料.Add(new string[] { 調劑台名稱, IP });
                        }
                    }
                }

            }




            allTask = Task.WhenAll(taskList);
            allTask.Wait();
            for (int i = 0; i < list_需更新資料.Count; i++)
            {
                Function_取藥堆疊子資料_設定配藥完成ByIP(list_需更新資料[i][0], list_需更新資料[i][1]);
            }
            #endregion

            #region LED層架_作業未完成
            taskList = new List<Task>();
            list_需更新資料 = new List<string[]>();
            foreach (object[] value in list_取藥子堆疊資料_LED層架_作業未完成)
            {
                IP = value[(int)enum_取藥堆疊子資料.IP].ObjectToString();
                藥品碼 = value[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = value[(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();
                Master_GUID = value[(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                Device_GUID = value[(int)enum_取藥堆疊子資料.Device_GUID].ObjectToString();

                list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);

                if (list_取藥母堆疊資料_buf.Count > 0)
                {
                    color = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                    int match = (from values in list_需更新資料
                                 where values[0] == 調劑台名稱
                                 where values[1] == IP
                                 select values).ToList().Count;
                    if (match == 0)
                    {
                        list_需更新資料.Add(new string[] { 調劑台名稱, 藥品碼, IP });

                        if (flag_同藥碼全亮)
                        {

                            List<RowsDevice> rowsDevices = List_RowsLED_雲端資料.SortByCode(藥品碼);
                            for (int i = 0; i < rowsDevices.Count; i++)
                            {
                                RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(rowsDevices[i].IP);
                                RowsDevice rowsDevice = rowsDevices[i];
                                rowsLED.LED_Bytes = RowsLEDUI.Get_Rows_LEDBytes(ref rowsLED.LED_Bytes, rowsDevice, color);
                                rowsLEDUI.Set_Rows_LED_UDP(rowsLED);
                            }
                        }
                        else
                        {
                            RowsLED rowsLED = List_RowsLED_雲端資料.SortByIP(IP);
                            RowsDevice rowsDevice = rowsLED.SortByGUID(Device_GUID);

                            if (rowsDevice != null)
                            {
                                rowsLED.LED_Bytes = RowsLEDUI.Get_Rows_LEDBytes(ref rowsLED.LED_Bytes, rowsDevice, color);
                                rowsLEDUI.Set_Rows_LED_UDP(rowsLED);
                            }
                        }

                    }
                }
            }
            allTask = Task.WhenAll(taskList);
            allTask.Wait();

            for (int i = 0; i < list_需更新資料.Count; i++)
            {
                Function_取藥堆疊子資料_設定流程作業完成ByCode(list_需更新資料[i][0], list_需更新資料[i][1], list_需更新資料[i][2]);
            }
            #endregion
            #region LED層架_作業已完成
            taskList = new List<Task>();
            list_需更新資料 = new List<string[]>();
            foreach (object[] value in list_取藥子堆疊資料_LED層架_作業已完成)
            {
                IP = value[(int)enum_取藥堆疊子資料.IP].ObjectToString();
                藥品碼 = value[(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = value[(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();
                Master_GUID = value[(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);
                if (list_取藥母堆疊資料_buf.Count > 0)
                {
                    color = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                    int match = (from values in list_需更新資料
                                 where values[0] == 調劑台名稱
                                 where values[1] == IP
                                 select values).ToList().Count;
                    if (match == 0)
                    {
                        list_需更新資料.Add(new string[] { 調劑台名稱, 藥品碼, IP });
                    }
                }

            }
            allTask = Task.WhenAll(taskList);
            allTask.Wait();
            for (int i = 0; i < list_需更新資料.Count; i++)
            {
                Function_取藥堆疊子資料_設定配藥完成ByCode(list_需更新資料[i][0], list_需更新資料[i][1], list_需更新資料[i][2]);
            }
            #endregion
            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_流程作業檢查_檢查抽屜手勢感測感應到(ref int cnt)
        {
            List<Task> taskList = new List<Task>();
            string IP = "";
            string 藥品碼 = "";
            string 調劑台名稱 = "";
            string GUID = "";
            string Master_GUID = "";
            string Device_GUID = "";
            bool flag_TOFON = false;
            Color color = Color.Black;

            List<object[]> list_取藥母堆疊資料 = Function_取藥堆疊資料_取得母資料();
            List<object[]> list_取藥母堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料 = Function_取藥堆疊資料_取得子資料();
            List<object[]> list_取藥子堆疊資料_buf = new List<object[]>();
            List<object[]> list_取藥子堆疊資料_replace = new List<object[]>();



            Task allTask;
            List<string[]> list_需更新資料;
            List<object[]> list_取藥子堆疊資料_手勢感測作業檢查 = new List<object[]>();

            list_取藥子堆疊資料_手勢感測作業檢查 = (from value in list_取藥子堆疊資料
                                     where value[(int)enum_取藥堆疊子資料.致能].ObjectToString() == true.ToString()
                                     where value[(int)enum_取藥堆疊子資料.流程作業完成].ObjectToString() == false.ToString()
                                     where value[(int)enum_取藥堆疊子資料.配藥完成].ObjectToString() == false.ToString()
                                     where value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD583.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD583_lock.GetEnumName()
                                           || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD730E.GetEnumName() || value[(int)enum_取藥堆疊子資料.TYPE].ObjectToString() == DeviceType.EPD730E_lock.GetEnumName()
                                     select value).ToList();

            //if (plC_CheckBox_同藥品全部亮燈.Bool)
            //{
            //    for (int i = 0; i < list_取藥子堆疊資料_手勢感測作業檢查.Count; i++) list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
            //    if (list_取藥子堆疊資料_手勢感測作業檢查.Count > 0) this.sqL_DataGridView_取藥堆疊子資料.SQL_ReplaceExtra(list_取藥子堆疊資料_手勢感測作業檢查, false);
            //    cnt++;
            //    return;
            //}

            taskList = new List<Task>();
            list_需更新資料 = new List<string[]>();
            List<string[]> list_手勢檢查資料 = new List<string[]>();
            for (int i = 0; i < list_取藥子堆疊資料_手勢感測作業檢查.Count; i++)
            {
                IP = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.IP].ObjectToString();
                藥品碼 = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.藥品碼].ObjectToString();
                調劑台名稱 = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.調劑台名稱].ObjectToString();
                Master_GUID = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.Master_GUID].ObjectToString();
                list_取藥母堆疊資料_buf = list_取藥母堆疊資料.GetRows((int)enum_取藥堆疊母資料.GUID, Master_GUID);

                list_取藥子堆疊資料_buf = (from temp in list_取藥子堆疊資料_replace
                                    where temp[(int)enum_取藥堆疊母資料.IP].ObjectToString() == IP
                                    select temp).ToList();
                if (list_取藥母堆疊資料_buf.Count > 0)
                {
                    color = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.顏色].ObjectToString().ToColor();
                    double 數量 = list_取藥母堆疊資料_buf[0][(int)enum_取藥堆疊母資料.總異動量].StringToDouble();
                    if (數量 < 0) 數量 = 數量 * -1;
                    if (list_取藥子堆疊資料_buf.Count == 0)
                    {
                        List<Box> boxes = List_EPD583_本地資料.SortByCode(藥品碼);
                        for (int k = 0; k < boxes.Count; k++)
                        {
                            Drawer drawer = List_EPD583_本地資料.SortByIP(boxes[k].IP);
                            if (drawer == null) continue;


                            string index_IP = Funcion_取得LCD114索引表_index_IP(boxes[k].IP);
                            if (index_IP.StringIsEmpty())
                            {
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.配藥完成] = true.ToString();
                                list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);
                                continue;
                            }
                            Rectangle rectangle = DrawerUI_EPD_583.Get_Box_rect(drawer, boxes[k]);
                            DrawerUI_EPD_583.LightSensorClass lightSensorClass = DrawerUI_EPD_583.Get_LightSensorClass(rectangle);
                            string json = storageUI_LCD_114.Get_JsonStrin(index_IP , myConfigClass.LCD114_Port);
                            StorageUI_LCD_114.UDP_READ uDP_READ = json.JsonDeserializet<StorageUI_LCD_114.UDP_READ>();
                            if (uDP_READ == null) continue;
                            bool Sensor_ON = uDP_READ.IsSensorOn(lightSensorClass);
                            if (Sensor_ON)
                            {
                                Console.WriteLine($"lightSensorClass : {lightSensorClass}");
                                Console.WriteLine($"IP : {boxes[k].IP} , index_IP : {index_IP}, Sensor_ON : {Sensor_ON}");
                                SendPlayVoiceCommand(myConfigClass.Voice_IP, myConfigClass.Voice_port, "OK");
                                list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.流程作業完成] = true.ToString();
                                list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);
                                LightOn lightOn = new LightOn(藥品碼, color, 數量);
                                lightOn.顏色 = Color.FromArgb((int)(color.R * 0.1), (int)(color.G * 0.1), (int)(color.B * 0.1));
                                lightOn.flag_Refresh_Light = true;
                                Function_儲位亮燈(lightOn);
                          

                            }
                            else if (uDP_READ.Input > 0)
                            {
                                int temp = list_取藥子堆疊資料_手勢感測作業檢查[i][(int)enum_取藥堆疊子資料.暫存參數].StringToInt32();
                                list_取藥子堆疊資料_replace.Add(list_取藥子堆疊資料_手勢感測作業檢查[i]);

                           


                            }
                        }

                    }
                }
            }

            allTask = Task.WhenAll(taskList);
            allTask.Wait();
            if (list_取藥子堆疊資料_replace.Count > 0)
            {
                sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_取藥子堆疊資料_replace);
            }

            cnt++;
        }
        #endregion
        #region PLC_取藥堆疊資料_入賬檢查
        static public bool PLC_Device_取藥堆疊資料_入賬檢查 = false;
        static public MyTimer MyTimer_取藥堆疊資料_入賬檢查刷新延遲 = new MyTimer();
        static public int cnt_Program_取藥堆疊資料_入賬檢查 = 65534;
        static public void sub_Program_取藥堆疊資料_入賬檢查()
        {
            PLC_Device_取藥堆疊資料_入賬檢查 = true;
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 65534)
            {
                PLC_Device_取藥堆疊資料_入賬檢查 = false;
                cnt_Program_取藥堆疊資料_入賬檢查 = 65535;
            }
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 65535) cnt_Program_取藥堆疊資料_入賬檢查 = 1;
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 1) cnt_Program_取藥堆疊資料_入賬檢查_檢查按下(ref cnt_Program_取藥堆疊資料_入賬檢查);
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 2) cnt_Program_取藥堆疊資料_入賬檢查_初始化(ref cnt_Program_取藥堆疊資料_入賬檢查);
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 3) cnt_Program_取藥堆疊資料_入賬檢查_等待延遲(ref cnt_Program_取藥堆疊資料_入賬檢查);
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 4) cnt_Program_取藥堆疊資料_入賬檢查 = 65500;
            if (cnt_Program_取藥堆疊資料_入賬檢查 > 1) cnt_Program_取藥堆疊資料_入賬檢查_檢查放開(ref cnt_Program_取藥堆疊資料_入賬檢查);
            if (cnt_Program_取藥堆疊資料_入賬檢查 == 65500)
            {

                PLC_Device_取藥堆疊資料_入賬檢查 = false;
                cnt_Program_取藥堆疊資料_入賬檢查 = 65535;
            }
        }
        static public void cnt_Program_取藥堆疊資料_入賬檢查_檢查按下(ref int cnt)
        {
            if (PLC_Device_取藥堆疊資料_入賬檢查) cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_入賬檢查_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取藥堆疊資料_入賬檢查) cnt = 65500;
        }
        static public void cnt_Program_取藥堆疊資料_入賬檢查_初始化(ref int cnt)
        {
            List<object[]> list_可入賬母資料 = Function_取藥堆疊母資料_取得可入賬資料();
            List<object[]> list_子資料 = Function_取藥堆疊資料_取得子資料();
            List<object[]> list_子資料_buf;
            List<object[]> list_取藥堆疊子資料_ReplaceValue = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_ReplaceValue = new List<object[]>();
            List<object[]> list_取藥堆疊母資料_Add = new List<object[]>();
            List<object[]> list_交易紀錄新增資料_AddValue = new List<object[]>();
            List<object[]> list_醫囑資料_ReplaceValue = new List<object[]>();

            bool flag_修正盤點量 = false;
            string GUID = "";
            string Master_GUID = "";
            double 庫存量 = 0;
            double 結存量 = 0;
            double 總異動量 = 0;
            string 盤點量 = "";
            string 動作 = "";
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 藥袋序號 = "";
            string 類別 = "";
            string 交易量 = "";
            string 操作人 = "";
            string ID = "";
            string 覆核藥師姓名 = "";
            string 覆核藥師ID = "";
            string 病人姓名 = "";
            string 床號 = "";
            string 頻次 = "";
            string 病歷號 = "";
            string 操作時間 = "";
            string 開方時間 = "";
            string 備註 = "";
            string 收支原因 = "";
            string 診別 = "";
            string 藥師證字號 = "";
            string 效期 = "";
            string 批號 = "";
            string 顏色 = "";
            string 領藥號 = "";
            string 病房號 = "";
            string 醫令_GUID = "";
            string 交易紀錄_GUID = "";
            string Order_GUID = "";
            List<string> List_效期 = new List<string>();
            List<string> List_批號 = new List<string>();
            List<string> list_儲位刷新_藥品碼 = new List<string>();
            List<string> list_儲位刷新_藥品碼_buf = new List<string>();
            list_可入賬母資料.Sort(new Icp_取藥堆疊母資料_index排序());
            List<string> list_Codes = (from temp in list_可入賬母資料
                                       select temp[(int)enum_取藥堆疊母資料.藥品碼].ObjectToString()).Distinct().ToList();
            for (int i = 0; i < list_Codes.Count; i++)
            {
                Function_從SQL取得儲位到入賬資料(list_Codes[i]);
            }
            for (int i = 0; i < list_可入賬母資料.Count; i++)
            {

                Master_GUID = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.GUID].ObjectToString();
                動作 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.動作].ObjectToString();
                診別 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.診別].ObjectToString();
                藥品碼 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.藥品碼].ObjectToString();
                //Function_從SQL取得儲位到入賬資料(藥品碼);
                藥品名稱 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.藥品名稱].ObjectToString();
                藥袋序號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.藥袋序號].ObjectToString();
                類別 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.類別].ObjectToString();
                操作人 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.操作人].ObjectToString();
                ID = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.ID].ObjectToString();
                藥師證字號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.藥師證字號].ObjectToString();
                總異動量 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToInt32();
                交易量 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString();
                盤點量 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.盤點量].ObjectToString();
                顏色 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.顏色].ObjectToString();
                領藥號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.領藥號].ObjectToString();
                病房號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.病房號].ObjectToString();
                病人姓名 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.病人姓名].ObjectToString();
                床號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.床號].ObjectToString();
                頻次 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.頻次].ObjectToString();
                病歷號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.病歷號].ObjectToString();
                操作時間 = DateTime.Now.ToDateTimeString_6();
                開方時間 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.開方時間].ObjectToString();
                備註 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.備註].ObjectToString();
                收支原因 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.收支原因].ObjectToString();
                效期 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.效期].ObjectToString();
                批號 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.批號].ObjectToString();
                覆核藥師姓名 = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.覆核藥師姓名].ObjectToString();
                覆核藥師ID = list_可入賬母資料[i][(int)enum_取藥堆疊母資料.覆核藥師ID].ObjectToString();
                庫存量 = Function_從入賬資料取得庫存(藥品碼);
                結存量 = (庫存量 + 總異動量);


                if (藥品名稱.StringIsEmpty())
                {
                    medClass medClass = medClass.get_med_clouds_by_code(API_Server, 藥品碼);
                    if (medClass != null)
                    {
                        藥品名稱 = medClass.藥品名稱;
                    }
                }

                List_效期.Clear();
                List_批號.Clear();
                list_子資料_buf = list_子資料.GetRows((int)enum_取藥堆疊子資料.Master_GUID, Master_GUID);

                for (int k = 0; k < list_子資料_buf.Count; k++)
                {

                    list_子資料_buf[k] = Function_取藥堆疊子資料_設定已入帳(list_子資料_buf[k]);
                    List_效期.Add(list_子資料_buf[k][(int)enum_取藥堆疊子資料.效期].ObjectToString());
                    List_批號.Add(list_子資料_buf[k][(int)enum_取藥堆疊子資料.批號].ObjectToString());
                    list_取藥堆疊子資料_ReplaceValue.Add(list_子資料_buf[k]);
                    //Function_取藥堆疊資料_語音提示(list_子資料_buf[k]);
                }
                list_可入賬母資料[i][(int)enum_取藥堆疊母資料.庫存量] = 庫存量.ToString();
                list_可入賬母資料[i][(int)enum_取藥堆疊母資料.結存量] = 結存量.ToString();
                list_可入賬母資料[i][(int)enum_取藥堆疊母資料.狀態] = enum_取藥堆疊母資料_狀態.入賬完成.GetEnumName();
                list_取藥堆疊母資料_ReplaceValue.Add(list_可入賬母資料[i]);



                //新增交易紀錄資料
                for (int k = 0; k < List_效期.Count; k++)
                {
                    備註 += $"[效期]:{List_效期[k]},[批號]:{List_批號[k]}";
                    if (k != List_效期.Count - 1) 備註 += "\n";
                }

                object[] value_trading = new object[new enum_交易記錄查詢資料().GetLength()];
                value_trading[(int)enum_交易記錄查詢資料.GUID] = Guid.NewGuid().ToString();
                交易紀錄_GUID = value_trading[(int)enum_交易記錄查詢資料.GUID].ObjectToString();
                value_trading[(int)enum_交易記錄查詢資料.動作] = 動作;
                value_trading[(int)enum_交易記錄查詢資料.診別] = 診別;
                value_trading[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
                value_trading[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
                value_trading[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
                value_trading[(int)enum_交易記錄查詢資料.藥師證字號] = 藥師證字號;
                value_trading[(int)enum_交易記錄查詢資料.領藥號] = 領藥號;
                value_trading[(int)enum_交易記錄查詢資料.病房號] = 病房號;
                value_trading[(int)enum_交易記錄查詢資料.類別] = 類別;
                value_trading[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
                value_trading[(int)enum_交易記錄查詢資料.交易量] = 交易量;
                value_trading[(int)enum_交易記錄查詢資料.結存量] = 結存量;
                if (盤點量.StringIsEmpty()) 盤點量 = "無";
                else flag_修正盤點量 = true;
                value_trading[(int)enum_交易記錄查詢資料.盤點量] = 盤點量;
                value_trading[(int)enum_交易記錄查詢資料.操作人] = 操作人;
                value_trading[(int)enum_交易記錄查詢資料.覆核藥師] = 覆核藥師姓名;
                value_trading[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
                value_trading[(int)enum_交易記錄查詢資料.床號] = 床號;
                value_trading[(int)enum_交易記錄查詢資料.頻次] = 頻次;
                value_trading[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
                value_trading[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
                if (開方時間.StringIsEmpty()) 開方時間 = DateTime.Now.ToDateTimeString_6();
                value_trading[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
                value_trading[(int)enum_交易記錄查詢資料.備註] = 備註;
                收支原因 = $"{收支原因}";
                value_trading[(int)enum_交易記錄查詢資料.收支原因] = 收支原因;

                if ((動作.Contains("系統")))
                {
                    if (總異動量 == 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (顏色 == Color.Black.ToColorString())
                        {
                            list_儲位刷新_藥品碼_buf = (from temp in list_儲位刷新_藥品碼
                                                 where temp == 藥品碼
                                                 select temp).ToList();
                            if (list_儲位刷新_藥品碼_buf.Count == 0)
                            {
                                list_儲位刷新_藥品碼.Add(藥品碼);
                            }
                        }

                    }
                }
                if (收支原因.Contains("盤點異常"))
                {

                    medRecheckLogClass medRecheckLogClass = new medRecheckLogClass();
                    int 差異值 = medRecheckLogClass.get_unresolved_qty_by_code(API_Server, ServerName, ServerType, 藥品碼);
                    medRecheckLogClass.發生類別 = "盤點異常";
                    medRecheckLogClass.藥碼 = 藥品碼;
                    medRecheckLogClass.藥名 = 藥品名稱;
                    medRecheckLogClass.庫存值 = (結存量 + 差異值).ToString();
                    medRecheckLogClass.盤點值 = 盤點量;
                    medRecheckLogClass.盤點藥師1 = 操作人;
                    medRecheckLogClass.盤點藥師2 = 覆核藥師姓名;
                    medRecheckLogClass.交易紀錄_GUID = 交易紀錄_GUID;
                    for (int m = 0; m < List_效期.Count; m++)
                    {
                        medRecheckLogClass.效期 += List_效期[m];
                        if (m != List_效期.Count - 1) medRecheckLogClass.效期 += ",";
                    }
                    for (int m = 0; m < List_批號.Count; m++)
                    {
                        medRecheckLogClass.批號 += List_批號[m];
                        if (m != List_批號.Count - 1) medRecheckLogClass.批號 += ",";
                    }
                    medRecheckLogClass.add(API_Server, ServerName, ServerType, medRecheckLogClass);

                }
                list_交易紀錄新增資料_AddValue.Add(value_trading);
                Console.WriteLine($"寫入交易紀錄,藥碼 : {藥品碼} ,交易量 : {交易量}");


            }
            for (int i = 0; i < list_取藥堆疊母資料_ReplaceValue.Count; i++)
            {

                Order_GUID = list_取藥堆疊母資料_ReplaceValue[i][(int)enum_取藥堆疊母資料.Order_GUID].ObjectToString();
                List<object[]> list_value = sQLControl_醫令資料.GetRowsByDefult(null, (int)enum_醫囑資料.GUID, Order_GUID);

                操作人 = list_取藥堆疊母資料_ReplaceValue[i][(int)enum_取藥堆疊母資料.操作人].ObjectToString();
                總異動量 = list_取藥堆疊母資料_ReplaceValue[i][(int)enum_取藥堆疊母資料.總異動量].ObjectToString().StringToDouble();
                if (list_value.Count == 0) continue;
                for (int m = 0; m < list_value.Count; m++)
                {
                    //if (list_value[m][(int)enum_醫囑資料.狀態].ObjectToString() == enum_醫囑資料_狀態.已過帳.GetEnumName()) continue;
                    list_value[m][(int)enum_醫囑資料.狀態] = enum_醫囑資料_狀態.已過帳.GetEnumName();
                    list_value[m][(int)enum_醫囑資料.過帳時間] = DateTime.Now.ToDateTimeString_6();
                    list_value[m][(int)enum_醫囑資料.藥師ID] = ID;
                    list_value[m][(int)enum_醫囑資料.藥師姓名] = 操作人;

                    string 實際調劑量 = list_value[m][(int)enum_醫囑資料.實際調劑量].ObjectToString();
                    if (實際調劑量.StringIsEmpty()) 實際調劑量 = "0";
                    實際調劑量 = (實際調劑量.StringToDouble() + 總異動量.StringToDouble()).ToString();
                    list_value[m][(int)enum_醫囑資料.實際調劑量] = 實際調劑量;
                    list_醫囑資料_ReplaceValue.Add(list_value[m]);
                }


                //Console.WriteLine($"{orderClasses.JsonSerializationt()}");
            }
            for (int i = 0; i < list_儲位刷新_藥品碼.Count; i++)
            {
                Function_儲位刷新(list_儲位刷新_藥品碼[i]);
            }

            if (list_交易紀錄新增資料_AddValue.Count > 0) sQLControl_交易記錄查詢.AddRows(null, list_交易紀錄新增資料_AddValue);
            if (list_取藥堆疊子資料_ReplaceValue.Count > 0) sQLControl_取藥堆疊子資料.UpdateByDefulteExtra(null, list_取藥堆疊子資料_ReplaceValue);
            if (list_取藥堆疊母資料_ReplaceValue.Count > 0) sQLControl_取藥堆疊母資料.UpdateByDefulteExtra(null, list_取藥堆疊母資料_ReplaceValue);
            if (list_醫囑資料_ReplaceValue.Count > 0)
            {
                sQLControl_醫令資料.UpdateByDefulteExtra(null, list_醫囑資料_ReplaceValue);
            }
            cnt++;
        }
        static public void cnt_Program_取藥堆疊資料_入賬檢查_等待延遲(ref int cnt)
        {
            cnt++;
        }




        #endregion

        public class Icp_取藥堆疊母資料_index排序 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string index_0 = x[(int)enum_取藥堆疊母資料.序號].ObjectToString();
                string index_1 = y[(int)enum_取藥堆疊母資料.序號].ObjectToString();
                DateTime date0 = index_0.StringToDateTime();
                DateTime date1 = index_1.StringToDateTime();
                return DateTime.Compare(date0, date1);

            }
        }
        public class Icp_取藥堆疊母資料_操作時間排序 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string index_0 = x[(int)enum_取藥堆疊母資料.操作時間].ToDateTimeString_6();
                string index_1 = y[(int)enum_取藥堆疊母資料.操作時間].ToDateTimeString_6();
                DateTime date0 = index_0.StringToDateTime();
                DateTime date1 = index_1.StringToDateTime();
                return DateTime.Compare(date0, date1);

            }
        }
        public class Icp_取藥堆疊子資料_index排序 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string index_0 = x[(int)enum_取藥堆疊子資料.序號].ObjectToString();
                string index_1 = y[(int)enum_取藥堆疊子資料.序號].ObjectToString();
                UInt64 temp0 = 0;
                UInt64 temp1 = 0;
                UInt64.TryParse(index_0, out temp0);
                UInt64.TryParse(index_1, out temp1);
                if (temp0 > temp1) return 1;
                else if (temp0 < temp1) return -1;
                else return 0;
            }
        }
        public class Icp_取藥堆疊子資料_致能排序 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string 致能_A = x[(int)enum_取藥堆疊子資料.致能].ObjectToString();
                string 致能_B = y[(int)enum_取藥堆疊子資料.致能].ObjectToString();
                if (致能_A == true.ToString()) 致能_A = "1";
                else 致能_A = "0";
                if (致能_B == true.ToString()) 致能_B = "1";
                else 致能_B = "0";
                return 致能_B.CompareTo(致能_A);
            }
        }

        public static void SendPlayVoiceCommand(string targetIP, int port, string command)
        {
            try
            {
                using (UdpClient udp = new UdpClient())
                {
                    byte[] data = Encoding.UTF8.GetBytes(command);

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 準備發送 UDP...");
                    Console.WriteLine($"  → 目標 IP   : {targetIP}");
                    Console.WriteLine($"  → 目標 Port : {port}");
                    Console.WriteLine($"  → 指令內容 : {command}");

                    int sendLength = udp.Send(data, data.Length, targetIP, port);

                    Console.WriteLine($"[SUCCESS] 已成功送出 {sendLength} bytes");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] UDP 發送失敗！");
                Console.WriteLine($"  錯誤訊息: {ex.Message}");
                Console.WriteLine();
            }
        }
    }
}
