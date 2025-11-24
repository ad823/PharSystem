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
using SQLUI;

namespace 調劑台管理系統
{
    static public class CommonSapceMethod
    {
        public static void WriteTakeMedicineStack(this List<CommonSapceClass> commonSapceClasses ,List<object[]> list_堆疊母資料_add)
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
                    List<object[]> var = sQLControl.GetRowsByDefult(null, (int)enum_取藥堆疊母資料.調劑台名稱, list_str[k]);
                    sQLControl.DeleteExtra(null, var);
                }            
                for(int k = 0; k < list_堆疊母資料_add.Count; k++)
                {
                    if (list_堆疊母資料_add[k][(int)enum_取藥堆疊母資料.動作].ObjectToString() == "系統領藥") list_堆疊母資料_add[k][(int)enum_取藥堆疊母資料.動作] = "掃碼領藥";
                    if (list_堆疊母資料_add[k][(int)enum_取藥堆疊母資料.狀態].ObjectToString() == "已領用過") list_堆疊母資料_add[k][(int)enum_取藥堆疊母資料.狀態] = "儲位亮燈";
                }
                sQLControl.AddRows(null, list_堆疊母資料_add);
                Console.WriteLine($"新增共用台資料,共<{list_堆疊母資料_add.Count}>筆");
            }
        }
        public static void DeleteTakeMedicineStack(this List<CommonSapceClass> commonSapceClasses,string deviceName)
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
                List<object[]> list_value = sQLControl.GetRowsByDefult(null, (int)enum_取藥堆疊母資料.調劑台名稱, deviceName);
                sQLControl.DeleteExtra(null, list_value);
              
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
            takeMedicineStackClass.set_device_tradding(Main_Form.API_Server, sys_serverSettingClass.設備名稱, Main_Form.ServerType, takeMedicineStackClasses);
        }
        public static Drawer GetEPD583(string IP , ref List<CommonSapceClass> commonSapceClasses)
        {
            for(int i = 0; i < commonSapceClasses.Count; i++)
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

    public partial class Main_Form : Form
    {
        static public SQL_DataGridView _sqL_DataGridView_共用區設定 = null;
        static public List<CommonSapceClass> commonSapceClasses = new List<CommonSapceClass>();
        public Table table_共用區 = null;
        private void Program_共用區_Init()
        {
            Table table = new Table("");
            table.Server = dBConfigClass.DB_Basic.IP;
            table.Port = dBConfigClass.DB_Basic.Port.ToString();
            table.Username = dBConfigClass.DB_Basic.UserName;
            table.Password = dBConfigClass.DB_Basic.Password;
            table.DBName = dBConfigClass.DB_Basic.DataBaseName;
            table.TableName = "common_space_setup";
            table.AddColumnList("GUID", Table.StringType.VARCHAR, Table.IndexType.PRIMARY);
            table.AddColumnList("共用區名稱", Table.StringType.VARCHAR, 200 ,Table.IndexType.None);
            table.AddColumnList("共用區類型", Table.StringType.VARCHAR, Table.IndexType.None);
            table.AddColumnList("是否共用", Table.StringType.VARCHAR, Table.IndexType.None);
            table.AddColumnList("設置時間", Table.DateType.DATETIME, Table.IndexType.None);
            table_共用區 = table;
            this.sqL_DataGridView_共用區設定.Init(table);
            if(this.sqL_DataGridView_共用區設定.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_共用區設定.SQL_CheckAllColumnName(true);
            }
            else
            {
                this.sqL_DataGridView_共用區設定.SQL_CreateTable();
            }

            _sqL_DataGridView_共用區設定 = this.sqL_DataGridView_共用區設定;
            this.plC_RJ_Button_共用區亮燈範圍設置.MouseDownEvent += PlC_RJ_Button_共用區亮燈範圍設置_MouseDownEvent;
            this.plC_UI_Init.Add_Method(this.Program_共用區);

        }
        private void Program_共用區()
        {

        }
        #region Function

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
        static public double Function_從共用區取得庫存(string 藥品碼)
        {
            double 庫存 = 0;
            List<object> list_value = Function_從共用區取得儲位(藥品碼);
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
            List<object[]> list_value = _sqL_DataGridView_共用區設定.SQL_GetAllRows(false);

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
            for (int i = 0; i < list_value.Count; i++ )
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
        #endregion
        #region Event

        #endregion
        private void PlC_RJ_Button_共用區亮燈範圍設置_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_共用區設置 dialog_共用區設置 = new Dialog_共用區設置(table_共用區);
            if (dialog_共用區設置.ShowDialog() != DialogResult.Yes) return;
            commonSapceClasses = Function_取得共用區所有儲位();
            MyMessageBox.ShowDialog("已完成共用區設定!");
        }


    }
}
