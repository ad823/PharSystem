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
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using H_Pannel_lib;
using MyOffice;
using HIS_DB_Lib;
using DrawingClass;

namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
        public enum ContextMenuStrip_儲位管理_EPD583_匯出
        {
            [Description("M8000")]
            匯出建置表,
            [Description("M8000")]
            匯出儲位表,
        }
        [EnumDescription("")]
        private enum enum_儲位管理_EPD583_匯出儲位表
        {
            藥碼,
            藥名,
            單位,
            儲位名稱,
        }

        static public List<Drawer> List_EPD583_本地資料 = new List<Drawer>();
        static public List<Drawer> List_EPD583_雲端資料 = new List<Drawer>();
        static public List<Drawer> List_EPD583_入賬資料 = new List<Drawer>();
        private Drawer EPD583_Drawer_Copy;
        private Box EPD583_Box_Copy;

        [EnumDescription("")]
        private enum enum_儲位管理_EPD583_效期及庫存
        {
            [Description("效期,VARCHAR,300,NONE")]
            效期,
            [Description("批號,VARCHAR,300,NONE")]
            批號,
            [Description("庫存,VARCHAR,300,NONE")]
            庫存,
        }
        [EnumDescription("")]
        private enum enum_儲位管理_EPD583_抽屜列表
        {
            [Description("IP,VARCHAR,300,NONE")]
            IP,
            [Description("名稱,VARCHAR,300,NONE")]
            名稱,
        }

        private bool flag_Program_儲位管理_EPD583_Init = false;
        private void Program_儲位管理_EPD583_Init()
        {

            this.drawerUI_EPD_583.DrawerChangeEvent += DrawerUI_EPD_583_DrawerChangeEvent;

            SQLUI.Table table = new SQLUI.Table(new enum_儲位管理_EPD583_效期及庫存());
            this.sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.dataGridView.Font = new Font("微軟正黑體", 12);
            this.sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.Init(table);
            this.sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.Set_ColumnVisible(false, new enum_儲位管理_EPD583_效期及庫存().GetEnumNames());
            this.sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD583_效期及庫存.效期);
            this.sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD583_效期及庫存.批號);
            this.sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD583_效期及庫存.庫存);

            table = new SQLUI.Table(new enum_儲位管理_EPD583_抽屜列表());
            this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.Init(table);
            this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.Set_ColumnVisible(false, new enum_儲位管理_EPD583_抽屜列表().GetEnumNames());
            this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_EPD583_抽屜列表.IP);
            this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_EPD583_抽屜列表.名稱);

            this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.RowEnterEvent += SqL_DataGridView_儲位管理_EPD583_抽屜列表_RowEnterEvent;

            this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.Init(this.sqL_DataGridView_藥品資料_藥檔資料);
            this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.Set_ColumnVisible(false, new enum_藥品資料_藥檔資料().GetEnumNames());
            this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.Set_ColumnVisible(true, enum_藥品資料_藥檔資料.藥品碼, enum_藥品資料_藥檔資料.藥品名稱, enum_藥品資料_藥檔資料.藥品學名, enum_藥品資料_藥檔資料.中文名稱, enum_藥品資料_藥檔資料.包裝單位);
            this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.RowDoubleClickEvent += SqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料_RowDoubleClickEvent; 

            this.rJ_TextBox_儲位管理_EPD583_抽屜列表_儲位名稱.KeyPress += RJ_TextBox_儲位管理_EPD583_抽屜列表_儲位名稱_KeyPress;
            this.rJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品碼.KeyPress += RJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品碼_KeyPress;
            this.rJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品名稱.KeyPress += RJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品名稱_KeyPress;
            this.rJ_TextBox_儲位管理_EPD583_儲位內容_儲位名稱.KeyPress += RJ_TextBox_儲位管理_EPD583_儲位內容_儲位名稱_KeyPress;
            this.rJ_TextBox_儲位管理_EPD583_儲位內容_儲位搜尋_藥品碼.KeyPress += RJ_TextBox_儲位管理_EPD583_儲位內容_儲位搜尋_藥品碼_KeyPress;
            this.rJ_TextBox_儲位管理_EPD583_抽屜列表_語音.KeyPress += RJ_TextBox_儲位管理_EPD583_抽屜列表_語音_KeyPress;

            this.plC_RJ_Button_儲位管理_EPD583_面板亮燈.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_面板亮燈_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_寫入.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_寫入_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_清除燈號.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_清除燈號_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_上傳至面板.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_上傳至面板_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_藥品搜尋_藥品碼_搜尋.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_藥品搜尋_藥品碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_藥品搜尋_藥品名稱_搜尋.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_藥品搜尋_藥品名稱_搜尋_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_藥品搜尋_填入資料.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_藥品搜尋_填入資料_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_合併儲位.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_合併儲位_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_分割儲位.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_分割儲位_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_刪除儲位.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_刪除儲位_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_初始化儲位.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_初始化儲位_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_藥品名稱字體更動.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品名稱字體更動_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_藥品學名字體更動.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品學名字體更動_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_中文名稱字體更動.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_中文名稱字體更動_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_藥品碼字體更動.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品碼字體更動_MouseDownEvent;

            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_儲位名稱字體更動.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_儲位名稱字體更動_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_總庫存字體更動.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_總庫存字體更動_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_效期字體更動.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_效期字體更動_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_效期管理_新增效期.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_效期管理_新增效期_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_效期管理_修正庫存.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_效期管理_修正庫存_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_效期管理_修正批號.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_效期管理_修正批號_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_全部上鎖.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_全部上鎖_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_全部解鎖.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_全部解鎖_MouseDownEvent;

            this.plC_RJ_Button_儲位管理_EPD583_單格亮燈.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_單格亮燈_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_儲位搜尋_藥品碼_搜尋.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_開鎖.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_開鎖_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_複製儲位.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_複製儲位_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_貼上儲位.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_貼上儲位_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_儲位初始化.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位初始化_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_複製格式.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_複製格式_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_貼上格式.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_貼上格式_MouseDownEvent;


            this.plC_CheckBox_儲位管理_EPD583_儲位內容_效期顯示.CheckStateChanged += PlC_CheckBox_儲位管理_EPD583_儲位內容_效期顯示_CheckStateChanged;
            this.plC_CheckBox_儲位管理_EPD583_儲位內容_藥品碼顯示.CheckStateChanged += PlC_CheckBox_儲位管理_EPD583_儲位內容_藥品碼顯示_CheckStateChanged;
            this.plC_CheckBox_儲位管理_EPD583_儲位內容_庫存顯示.CheckStateChanged += PlC_CheckBox_儲位管理_EPD583_儲位內容_庫存顯示_CheckStateChanged;
            this.plC_CheckBox_儲位管理_EPD583_庫存儲位.CheckedChanged += PlC_CheckBox_儲位管理_EPD583_庫存儲位_CheckedChanged;

            this.plC_CheckBox_儲位管理_EPD583_隔板亮燈.CheckStateChanged += PlC_CheckBox_儲位管理_EPD583_隔板亮燈_CheckStateChanged;
            this.plC_RJ_Button_儲位管理_EPD583_更新.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_更新_MouseDownEvent;
            this.plC_CheckBox_儲位管理_EPD583_輸出.CheckStateChanged += PlC_CheckBox_儲位管理_EPD583_輸出_CheckStateChanged;
            this.plC_CheckBox_儲位管理_EPD583_輸入方向.CheckStateChanged += PlC_CheckBox_儲位管理_EPD583_輸入方向_CheckStateChanged;
            this.plC_CheckBox_儲位管理_EPD583_輸出方向.CheckStateChanged += PlC_CheckBox_儲位管理_EPD583_輸出方向_CheckStateChanged;
            this.plC_CheckBox_儲位管理_EPD583_顯示為條碼.CheckStateChanged += PlC_CheckBox_儲位管理_EPD583_顯示為條碼_CheckStateChanged;
            this.plC_RJ_Button_儲位管理_EPD583_匯出.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_匯出_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_匯入.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_匯入_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD583_自動填入儲位名稱.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_自動填入儲位名稱_MouseDownEvent;
            this.plC_CheckBox_儲位管理_EPD583_警報.CheckStateChanged += PlC_CheckBox_儲位管理_EPD583_警報_CheckStateChanged;

            this.comboBox_儲位管理_EPD583_儲位內容_儲位搜尋.SelectedIndex = 0;

            List<string> enum_PictureType_strings = new enum_PictureType().GetEnumNames().ToList();
            enum_PictureType_strings.Remove("藥品圖片");
            this.comboBox_儲位管理_EPD583_儲位內容_圖片1.DataSource = enum_PictureType_strings.ToArray();
            this.comboBox_儲位管理_EPD583_儲位內容_圖片2.DataSource = enum_PictureType_strings.ToArray();
            this.plC_RJ_Button_儲位管理_EPD583_儲位內容_儲存.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD583_儲位內容_儲存_MouseDownEvent;

            this.epD_583_Pannel.Init(this.drawerUI_EPD_583.List_UDP_Local);
            this.epD_583_Pannel.DrawerChangeEvent += EpD_583_Pannel_DrawerChangeEvent;
            this.epD_583_Pannel.MouseDownEvent += EpD_583_Pannel_MouseDownEvent;
            this.plC_UI_Init.Add_Method(this.Program_儲位管理_EPD583);
        }

    

        private void Program_儲位管理_EPD583()
        {
            if (this.plC_ScreenPage_Main.PageText == "儲位管理" && this.plC_ScreenPage_儲位管理.PageText == "EPD583")
            {
                if(flag_Program_儲位管理_EPD583_Init == false)
                {
                    PLC_Device_儲位管理_EPD583_資料更新.Bool = true;
                    sqL_DataGridView_儲位管理_EPD583_抽屜列表.On_RowEnter();
                    flag_Program_儲位管理_EPD583_Init = true;
                }
            }
            else
            {
                flag_Program_儲位管理_EPD583_Init = false;
            }

            sub_Program_儲位管理_EPD583_資料更新();
        }

        #region PLC_儲位管理_EPD583_資料更新
        PLC_Device PLC_Device_儲位管理_EPD583_資料更新 = new PLC_Device("S9005");
        int cnt_Program_儲位管理_EPD583_資料更新 = 65534;
        void sub_Program_儲位管理_EPD583_資料更新()
        {
            if (cnt_Program_儲位管理_EPD583_資料更新 == 65534)
            {
                PLC_Device_儲位管理_EPD583_資料更新.SetComment("PLC_儲位管理_EPD583_資料更新");
                PLC_Device_儲位管理_EPD583_資料更新.Bool = false;
                cnt_Program_儲位管理_EPD583_資料更新 = 65535;
            }
            if (cnt_Program_儲位管理_EPD583_資料更新 == 65535) cnt_Program_儲位管理_EPD583_資料更新 = 1;
            if (cnt_Program_儲位管理_EPD583_資料更新 == 1) cnt_Program_儲位管理_EPD583_資料更新_檢查按下(ref cnt_Program_儲位管理_EPD583_資料更新);
            if (cnt_Program_儲位管理_EPD583_資料更新 == 2) cnt_Program_儲位管理_EPD583_資料更新_初始化(ref cnt_Program_儲位管理_EPD583_資料更新);
            if (cnt_Program_儲位管理_EPD583_資料更新 == 3) cnt_Program_儲位管理_EPD583_資料更新_更新面板資料(ref cnt_Program_儲位管理_EPD583_資料更新);
            if (cnt_Program_儲位管理_EPD583_資料更新 == 4) cnt_Program_儲位管理_EPD583_資料更新_更新藥檔(ref cnt_Program_儲位管理_EPD583_資料更新);
            if (cnt_Program_儲位管理_EPD583_資料更新 == 5) cnt_Program_儲位管理_EPD583_資料更新 = 65500;
            if (cnt_Program_儲位管理_EPD583_資料更新 > 1) cnt_Program_儲位管理_EPD583_資料更新_檢查放開(ref cnt_Program_儲位管理_EPD583_資料更新);

            if (cnt_Program_儲位管理_EPD583_資料更新 == 65500)
            {
                PLC_Device_儲位管理_EPD583_資料更新.Bool = false;
                cnt_Program_儲位管理_EPD583_資料更新 = 65535;
            }
        }
        void cnt_Program_儲位管理_EPD583_資料更新_檢查按下(ref int cnt)
        {
            if (PLC_Device_儲位管理_EPD583_資料更新.Bool) cnt++;
        }
        void cnt_Program_儲位管理_EPD583_資料更新_檢查放開(ref int cnt)
        {
            if (!PLC_Device_儲位管理_EPD583_資料更新.Bool) cnt = 65500;
        }
        void cnt_Program_儲位管理_EPD583_資料更新_初始化(ref int cnt)
        {
            MyTimer_TickTime.TickStop();
            MyTimer_TickTime.StartTickTime(50000);
            List_EPD583_本地資料 = deviceApiClass.Get_EPD583_Drawers(API_Server, ServerName, ServerType);
            Console.Write($"儲位管理EPD583:從SQL取得資料 ,耗時 :{MyTimer_TickTime.GetTickTime().ToString("0.000")}\n");
            cnt++;
        }
        void cnt_Program_儲位管理_EPD583_資料更新_更新面板資料(ref int cnt)
        {
            MyTimer_TickTime.TickStop();
            MyTimer_TickTime.StartTickTime(50000);
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < List_EPD583_本地資料.Count; i++)
            {
                object[] value = new object[new enum_儲位管理_EPD583_抽屜列表().GetLength()];
                value[(int)enum_儲位管理_EPD583_抽屜列表.IP] = List_EPD583_本地資料[i].IP;
                value[(int)enum_儲位管理_EPD583_抽屜列表.名稱] = List_EPD583_本地資料[i].Name;
                list_value.Add(value);
            }
            list_value.Sort(new ICP_儲位管理_EPD583_抽屜列表());
            this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.RefreshGrid(list_value);
            Console.Write($"儲位管理EPD583:更新抽屜列表完成 ,耗時 :{MyTimer_TickTime.GetTickTime().ToString("0.000")}\n");
            cnt++;
        }
        void cnt_Program_儲位管理_EPD583_資料更新_更新藥檔(ref int cnt)
        {
            MyTimer_TickTime.TickStop();
            MyTimer_TickTime.StartTickTime(50000);

            List<object[]> list_藥品資料_藥檔資料 = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_藥品設定表 = this.sqL_DataGridView_藥品設定表.SQL_GetAllRows(false);
            List<object[]> list_藥品資料_藥檔資料_buf = new List<object[]>();
            List<object[]> list_藥品設定表_buf = new List<object[]>();
            List<Drawer> list_replaceValue = new List<Drawer>();
            string 藥品碼 = "";
            string 料號 = "";
            string 藥品名稱 = "";
            string 中文名稱 = "";
            string 藥品學名 = "";
            string 包裝單位 = "";
            string 警訊藥品 = "";
            string 麻醉藥品 = "";
            string 形狀相似 = "";
            string 發音相似 = "";
            string 管制級別 = "";

            string 藥品碼_buf = "";
            string 料號_buf = "";
            string 藥品名稱_buf = "";
            string 中文名稱_buf = "";
            string 藥品學名_buf = "";
            string 包裝單位_buf = "";
            string 警訊藥品_buf = "";
            string 麻醉藥品_buf = "";
            string 形狀相似_buf = "";
            string 發音相似_buf = "";
            string 管制級別_buf = "";
            for (int i = 0; i < List_EPD583_本地資料.Count; i++)
            {
                string IP = List_EPD583_本地資料[i].IP;
                List<Box> boxes = List_EPD583_本地資料[i].GetAllBoxes();
                bool Is_Replace = false;
                for (int k = 0; k < boxes.Count; k++)
                {
                    藥品碼 = boxes[k].GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
                    if (藥品碼.StringIsEmpty()) continue;
                    list_藥品資料_藥檔資料_buf = list_藥品資料_藥檔資料.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, 藥品碼);
                    list_藥品設定表_buf = list_藥品設定表.GetRows((int)enum_medConfig.藥碼, 藥品碼);
                    if (list_藥品資料_藥檔資料_buf.Count == 0)
                    {
                        boxes[k].Clear();
                        Is_Replace = true;              
                    }
                    else
                    {
                        藥品碼_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
                        料號_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.料號].ObjectToString();
                        藥品名稱_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.藥品名稱].ObjectToString();
                        中文名稱_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.中文名稱].ObjectToString();
                        藥品學名_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.藥品學名].ObjectToString();
                        包裝單位_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.包裝單位].ObjectToString();
                        管制級別_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.管制級別].ObjectToString();
                        警訊藥品_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.警訊藥品].ObjectToString().ToUpper();
                        if (警訊藥品_buf.StringIsEmpty()) 警訊藥品_buf = false.ToString().ToUpper();

                        if (list_藥品設定表_buf.Count > 0)
                        {
                            麻醉藥品_buf = list_藥品設定表_buf[0][(int)enum_medConfig.麻醉藥品].ObjectToString().ToUpper();
                            形狀相似_buf = list_藥品設定表_buf[0][(int)enum_medConfig.形狀相似].ObjectToString().ToUpper();
                            發音相似_buf = list_藥品設定表_buf[0][(int)enum_medConfig.發音相似].ObjectToString().ToUpper();
                        }
                        else
                        {
                            麻醉藥品_buf = false.ToString().ToUpper();
                            形狀相似_buf = false.ToString().ToUpper();
                            發音相似_buf = false.ToString().ToUpper();
                        }
                     

                        藥品碼 = boxes[k].GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
                        藥品名稱 = boxes[k].GetValue(Device.ValueName.藥品名稱, Device.ValueType.Value).ObjectToString();
                        中文名稱 = boxes[k].GetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Value).ObjectToString();
                        藥品學名 = boxes[k].GetValue(Device.ValueName.藥品學名, Device.ValueType.Value).ObjectToString();
                        包裝單位 = boxes[k].GetValue(Device.ValueName.包裝單位, Device.ValueType.Value).ObjectToString();
                        管制級別 = boxes[k].DRUGKIND;
                        警訊藥品 = boxes[k].IsWarning ? "TRUE" : "FALSE";
                        麻醉藥品 = boxes[k].IsAnesthetic ? "TRUE" : "FALSE";
                        形狀相似 = boxes[k].IsShapeSimilar ? "TRUE" : "FALSE";
                        發音相似 = boxes[k].IsSoundSimilar ? "TRUE" : "FALSE";
                        料號 = boxes[k].SKDIACODE;

                        if (藥品碼 != 藥品碼_buf) Is_Replace = true;
                        if (料號 != 料號_buf) Is_Replace = true;
                        if (藥品名稱 != 藥品名稱_buf) Is_Replace = true;
                        if (中文名稱 != 中文名稱_buf) Is_Replace = true;
                        if (藥品學名 != 藥品學名_buf) Is_Replace = true;
                        if (包裝單位 != 包裝單位_buf) Is_Replace = true;
                        if (警訊藥品 != 警訊藥品_buf) Is_Replace = true;
                        if (管制級別 != 管制級別_buf) Is_Replace = true;
                        if (麻醉藥品 != 麻醉藥品_buf) Is_Replace = true;
                        if (形狀相似 != 形狀相似_buf) Is_Replace = true;
                        if (發音相似 != 發音相似_buf) Is_Replace = true;

                        boxes[k].SetValue(Device.ValueName.藥品碼, Device.ValueType.Value, 藥品碼_buf);
                        boxes[k].SetValue(Device.ValueName.藥品名稱, Device.ValueType.Value, 藥品名稱_buf);
                        boxes[k].SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Value, 中文名稱_buf);
                        boxes[k].SetValue(Device.ValueName.藥品學名, Device.ValueType.Value, 藥品學名_buf);
                        boxes[k].SetValue(Device.ValueName.包裝單位, Device.ValueType.Value, 包裝單位_buf);
                        boxes[k].SKDIACODE = 料號_buf;
                        boxes[k].DRUGKIND = 管制級別_buf;
                        boxes[k].IsWarning = (警訊藥品_buf == "TRUE");
                        boxes[k].IsAnesthetic = (麻醉藥品_buf == "TRUE");
                        boxes[k].IsShapeSimilar = (形狀相似_buf == "TRUE");
                        boxes[k].IsSoundSimilar = (發音相似_buf == "TRUE");

                    }
                }
                if (Is_Replace)
                {
                    list_replaceValue.Add(List_EPD583_本地資料[i]);
                }
            }
            for(int i = 0; i < list_replaceValue.Count; i++)
            {
                List_EPD583_本地資料.Add_NewDrawer(list_replaceValue[i]);
            }
       
            this.drawerUI_EPD_583.SQL_ReplaceDrawer(list_replaceValue);
            Console.Write($"儲位管理EPD583:更新藥檔完成 共<{list_replaceValue.Count}>筆,耗時 :{MyTimer_TickTime.GetTickTime().ToString("0.000")}\n");
            cnt++;
        }
        #endregion
        #region Event
        private void DrawerUI_EPD_583_DrawerChangeEvent(List<Drawer> drawers)
        {
            //for (int i = 0; i < drawers.Count; i++)
            //{
            //    List_EPD583_本地資料.Add_NewDrawer(drawers[i]);
            //}

        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位初始化_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (MyMessageBox.ShowDialog("確認將所有儲位效期庫存更動為測試版?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    List<Drawer> drawers = this.drawerUI_EPD_583.SQL_GetAllDrawers();
                    List<Device> devices = drawers.GetAllDevice();
                    Dialog_Prcessbar dialog_Prcessbar = new Dialog_Prcessbar(devices.Count);
                    dialog_Prcessbar.State = "更動效期庫存..";
                    for (int i = 0; i < devices.Count; i++)
                    {
                        dialog_Prcessbar.Value = i;
                      
                        devices[i].清除所有庫存資料();
                        if (!devices[i].Code.StringIsEmpty())
                        {
                            devices[i].效期庫存覆蓋("2050/01/01", "999999");
                        }
             
                    }
                    dialog_Prcessbar.State = "上傳資料..";
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawers);
                    dialog_Prcessbar.Close();
                }
            }));
      
        }
        private void SqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料_RowDoubleClickEvent(object[] RowValue)
        {

        }
        private void SqL_DataGridView_儲位管理_EPD583_抽屜列表_RowEnterEvent(object[] RowValue)
        {
            try
            {
                string IP = RowValue[(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                string 儲位名稱 = RowValue[(int)enum_儲位管理_EPD583_抽屜列表.名稱].ObjectToString();

                rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts = IP;
                rJ_TextBox_儲位管理_EPD583_抽屜列表_儲位名稱.Texts = 儲位名稱;
                Drawer drawer = deviceApiClass.Get_EPD583_Drawer_ByIP(API_Server, ServerName, ServerType, IP);
                List_EPD583_本地資料.Add_NewDrawer(drawer);
                rJ_TextBox_儲位管理_EPD583_抽屜列表_語音.Texts = drawer.Speaker;
                if (drawer != null)
                {
                    this.epD_583_Pannel.CurrentDrawer = drawer;
                    plC_CheckBox_儲位管理_EPD583_隔板亮燈.Checked = drawer.IsAllLight;
                    plC_CheckBox_儲位管理_EPD583_警報.Checked = drawer.AlarmEnable;
                    plC_CheckBox_儲位管理_EPD583_庫存儲位.Checked = drawer.IsInventoryLocation;
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);

                    List<Task> taskList = new List<Task>();
                    taskList.Add(Task.Run(() =>
                    {

                        PlC_RJ_Button_儲位管理_EPD583_更新_MouseDownEvent(null);
                    }));
                }

               
                //Console.WriteLine($"SqL_DataGridView_儲位管理_EPD583_抽屜列表_RowEnterEvent");
            }
            catch (Exception e)
            {
                MyMessageBox.ShowDialog($"{e.Message}");
            }
          
        }
        private void PlC_RJ_Button_儲位管理_EPD583_全部解鎖_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("是否全部解鎖?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            List<object[]> list_value = sqL_DataGridView_儲位管理_EPD583_抽屜列表.GetAllRows();
            if (list_value.Count == 0) return;

            List<Task> taskList = new List<Task>();
            for (int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
                taskList.Add(Task.Run(() =>
                {

                    if (drawer != null)
                    {
                        this.drawerUI_EPD_583.SetOutput_dir(drawer, false);
                    }
                }));
            }
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();
            MyMessageBox.ShowDialog("解鎖完成!");
        }
        private void PlC_RJ_Button_儲位管理_EPD583_全部上鎖_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("是否全部上鎖?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            List<object[]> list_value = sqL_DataGridView_儲位管理_EPD583_抽屜列表.GetAllRows();
            if (list_value.Count == 0) return;

            List<Task> taskList = new List<Task>();
            for (int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
                taskList.Add(Task.Run(() =>
                {

                    if (drawer != null)
                    {
                        this.drawerUI_EPD_583.SetOutput_dir(drawer, true);
                    }
                }));
            }
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();
            MyMessageBox.ShowDialog("上鎖完成!");
        }
        private void EpD_583_Pannel_MouseDownEvent(List<Box> Boxes)
        {
            try
            {
                if (Boxes.Count == 0) return;
                this.Invoke(new Action(delegate
                {
                    rJ_TextBox_儲位管理_EPD583_儲位內容_藥品名稱.Text = Boxes[0].GetValue(Device.ValueName.藥品名稱, Device.ValueType.Value).ObjectToString();
                    rJ_TextBox_儲位管理_EPD583_儲位內容_藥品學名.Text = Boxes[0].GetValue(Device.ValueName.藥品學名, Device.ValueType.Value).ObjectToString();
                    rJ_TextBox_儲位管理_EPD583_儲位內容_中文名稱.Text = Boxes[0].GetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Value).ObjectToString();
                    rJ_TextBox_儲位管理_EPD583_儲位內容_藥品碼.Text = Boxes[0].GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
                   
                    rJ_TextBox_儲位管理_EPD583_儲位內容_儲位名稱.Text = Boxes[0].GetValue(Device.ValueName.儲位名稱, Device.ValueType.Value).ObjectToString();
                    rJ_TextBox_儲位管理_EPD583_儲位內容_總庫存.Text = Boxes[0].GetValue(Device.ValueName.庫存, Device.ValueType.Value).ObjectToString();
                    this.plC_CheckBox_儲位管理_EPD583_儲位內容_效期顯示.Checked = (bool)Boxes[0].GetValue(Device.ValueName.效期, Device.ValueType.Visable);
                    this.plC_CheckBox_儲位管理_EPD583_儲位內容_藥品碼顯示.Checked = (bool)Boxes[0].GetValue(Device.ValueName.藥品碼, Device.ValueType.Visable);
                    this.plC_CheckBox_儲位管理_EPD583_儲位內容_庫存顯示.Checked = (bool)Boxes[0].GetValue(Device.ValueName.庫存, Device.ValueType.Visable);

                    if (Boxes[0].Picture1_Name.StringIsEmpty()) comboBox_儲位管理_EPD583_儲位內容_圖片1.Text = "無";
                    else comboBox_儲位管理_EPD583_儲位內容_圖片1.Text = Boxes[0].Picture1_Name;

                    if (Boxes[0].Picture2_Name.StringIsEmpty()) comboBox_儲位管理_EPD583_儲位內容_圖片2.Text = "無";
                    else comboBox_儲位管理_EPD583_儲位內容_圖片2.Text = Boxes[0].Picture2_Name;
                }));



                sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.ClearGrid();
                List<object[]> list_value = new List<object[]>();
                for (int i = 0; i < Boxes[0].List_Validity_period.Count; i++)
                {
                    object[] value = new object[new enum_儲位管理_EPD583_效期及庫存().GetLength()];
                    value[(int)enum_儲位管理_EPD583_效期及庫存.效期] = Boxes[0].List_Validity_period[i];
                    value[(int)enum_儲位管理_EPD583_效期及庫存.批號] = Boxes[0].List_Lot_number[i];
                    value[(int)enum_儲位管理_EPD583_效期及庫存.庫存] = Boxes[0].List_Inventory[i];
                    list_value.Add(value);
                }

                sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.RefreshGrid(list_value);
            }
            catch(Exception e)
            {
                MyMessageBox.ShowDialog($"{e.Message}");
            }
          
        }
        private void EpD_583_Pannel_DrawerChangeEvent(Drawer drawer)
        {
            this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
        }
        private void PlC_CheckBox_儲位管理_EPD583_顯示為條碼_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.epD_583_Pannel.CurrentDrawer == null) return;
            if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
            else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
        }
        private void PlC_CheckBox_儲位管理_EPD583_隔板亮燈_CheckStateChanged(object sender, EventArgs e)
        {
            if (PLC_Device_最高權限.Bool == false) return;
            this.Invoke(new Action(delegate
            {
                string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
                if (drawer != null)
                {
                    drawer.IsAllLight = plC_CheckBox_儲位管理_EPD583_隔板亮燈.Checked;
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                    List_EPD583_本地資料.Add_NewDrawer(drawer);
                    this.epD_583_Pannel.CurrentDrawer = drawer;
                    this.Function_設定雲端資料更新();
                }
            }));
        }
        private void PlC_CheckBox_儲位管理_EPD583_警報_CheckStateChanged(object sender, EventArgs e)
        {
            if (PLC_Device_最高權限.Bool == false) return;
            this.Invoke(new Action(delegate
            {
                string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
                if (drawer != null)
                {
                    drawer.AlarmEnable = plC_CheckBox_儲位管理_EPD583_警報.Checked;
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                    List_EPD583_本地資料.Add_NewDrawer(drawer);
                    this.epD_583_Pannel.CurrentDrawer = drawer;
                    this.Function_設定雲端資料更新();
                    flag_Program_輸出入檢查_輸出刷新_Init = false;
                }
            }));
        }
        private void PlC_CheckBox_儲位管理_EPD583_庫存儲位_CheckedChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate
            {
                string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
                if (drawer != null)
                {
                    drawer.IsInventoryLocation = plC_CheckBox_儲位管理_EPD583_庫存儲位.Checked;
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                    List_EPD583_本地資料.Add_NewDrawer(drawer);
                    this.epD_583_Pannel.CurrentDrawer = drawer;
                    this.Function_設定雲端資料更新();
                    flag_Program_輸出入檢查_輸出刷新_Init = false;
                }
            }));
        }
        private void PlC_CheckBox_儲位管理_EPD583_輸出_CheckStateChanged(object sender, EventArgs e)
        {
            if (PLC_Device_最高權限.Bool == false) return;
            string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
            Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
            if (drawer == null) return;
            this.drawerUI_EPD_583.SetOutput(drawer, plC_CheckBox_儲位管理_EPD583_輸出.Checked);
        }
        private void PlC_CheckBox_儲位管理_EPD583_輸入方向_CheckStateChanged(object sender, EventArgs e)
        {
            if (PLC_Device_最高權限.Bool == false) return;
            string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
            Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
            if (drawer == null) return;
            this.drawerUI_EPD_583.SetInput_dir(drawer, plC_CheckBox_儲位管理_EPD583_輸入方向.Checked);
        }
        private void PlC_CheckBox_儲位管理_EPD583_輸出方向_CheckStateChanged(object sender, EventArgs e)
        {
            if (PLC_Device_最高權限.Bool == false) return;
            string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
            Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
            if (drawer == null) return;
            this.drawerUI_EPD_583.SetOutput_dir(drawer, plC_CheckBox_儲位管理_EPD583_輸出方向.Checked);
        }
        private void RJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品名稱_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_儲位管理_EPD583_藥品搜尋_藥品名稱_搜尋_MouseDownEvent(null);
            }
        }
        private void RJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_儲位管理_EPD583_藥品搜尋_藥品碼_搜尋_MouseDownEvent(null);
            }
        }
        private void RJ_TextBox_儲位管理_EPD583_抽屜列表_儲位名稱_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
                if (drawer != null)
                {
                    drawer.Name = rJ_TextBox_儲位管理_EPD583_抽屜列表_儲位名稱.Text;
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                    sqL_DataGridView_儲位管理_EPD583_抽屜列表.Replace(new object[] { drawer.IP, drawer.Name }, true);
                }
            }
        }
        private void RJ_TextBox_儲位管理_EPD583_儲位內容_儲位名稱_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇儲位!");
                    return;
                }
                boxes[0].SetValue(Device.ValueName.儲位名稱, Device.ValueType.Value, rJ_TextBox_儲位管理_EPD583_儲位內容_儲位名稱.Text);
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(epD_583_Pannel.CurrentDrawer);
                EpD_583_Pannel_MouseDownEvent(boxes);
            }
        }
        private void RJ_TextBox_儲位管理_EPD583_儲位內容_儲位搜尋_藥品碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品碼_搜尋_MouseDownEvent(null);
            }
        }
        private void RJ_TextBox_儲位管理_EPD583_抽屜列表_語音_KeyPress(object sender, KeyPressEventArgs e)
        {
            string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
            Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
            if (drawer != null)
            {
                drawer.Speaker = rJ_TextBox_儲位管理_EPD583_抽屜列表_語音.Text;
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                List_EPD583_本地資料.Add_NewDrawer(drawer);
                sqL_DataGridView_儲位管理_EPD583_抽屜列表.Replace(new object[] { drawer.IP, drawer.Name }, true);
                sqL_DataGridView_儲位管理_EPD583_抽屜列表.On_RowEnter();
                this.Function_設定雲端資料更新();
            }
        }
        private void PlC_RJ_Button_儲位管理_EPD583_面板亮燈_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = sqL_DataGridView_儲位管理_EPD583_抽屜列表.Get_All_Select_RowsValues();
            if (list_value.Count == 0) return;
            Color color = Color.Black;
            if (this.radioButton_儲位管理_EPD583_面板亮燈_白.Checked)
            {
                color = Color.White;
            }
            else if (this.radioButton_儲位管理_EPD583_面板亮燈_紅.Checked)
            {
                color = Color.Red;
            }
            else if (this.radioButton_儲位管理_EPD583_面板亮燈_藍.Checked)
            {
                color = Color.Blue;
            }
            else if (this.radioButton_儲位管理_EPD583_面板亮燈_綠.Checked)
            {
                color = Color.Green;
            }
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
                taskList.Add(Task.Run(() =>
                {
                  
                    if (drawer != null)
                    {
                        if (!this.drawerUI_EPD_583.Set_Pannel_LED_UDP(drawer, color))
                        {
                            //MyMessageBox.ShowDialog($"{drawer.IP}:{drawer.Port} : EPD 抽屜面板亮燈失敗!");
                        }
                        Console.WriteLine($"{drawer.IP}:{drawer.Port} : EPD 抽屜亮燈成功!");
                    }
                }));
            }
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();

         
        }
        private void PlC_RJ_Button_儲位管理_EPD583_單格亮燈_MouseDownEvent(MouseEventArgs mevent)
        {
            Color color = Color.Black;
            if (rJ_RatioButton_儲位管理_EPD583_單格亮燈_紅.Checked)
            {
                color = Color.Red;
            }
            else if (rJ_RatioButton_儲位管理_EPD583_單格亮燈_綠.Checked)
            {
                color = Color.Lime;
            }
            else if (rJ_RatioButton_儲位管理_EPD583_單格亮燈_藍.Checked)
            {
                color = Color.Blue;
            }
            else if (rJ_RatioButton_儲位管理_EPD583_單格亮燈_白.Checked)
            {
                color = Color.White;
            }
            List<int> cols = new List<int>();
            List<int> rows = new List<int>();
            int index = this.epD_583_Pannel.GetSelectBoxes(ref cols, ref rows);
            if (index == 0) return;
            epD_583_Pannel.CurrentDrawer.LED_Bytes = DrawerUI_EPD_583.Get_Empty_LEDBytes();
            epD_583_Pannel.CurrentDrawer.LED_Bytes = DrawerUI_EPD_583.Set_LEDBytes(epD_583_Pannel.CurrentDrawer, this.epD_583_Pannel.GetSelectBoxes(), color);
            epD_583_Pannel.CurrentDrawer.LED_Bytes = DrawerUI_EPD_583.Set_Pannel_LEDBytes(epD_583_Pannel.CurrentDrawer, color);
            this.drawerUI_EPD_583.Set_LED_UDP(epD_583_Pannel.CurrentDrawer);

            List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
            Rectangle rectangle = DrawerUI_EPD_583.Get_Box_rect(epD_583_Pannel.CurrentDrawer, boxes[0]);
            Console.WriteLine($"rectangle : {rectangle}");
            DrawerUI_EPD_583.LightSensorClass lightSensorClass = DrawerUI_EPD_583.Get_LightSensorClass(rectangle);
            Console.WriteLine($"lightSensorClass : {lightSensorClass}");
            string index_IP = Funcion_取得LCD114索引表_index_IP(epD_583_Pannel.CurrentDrawer.IP);
            if (index_IP.StringIsEmpty()) return;
            StorageUI_LCD_114.UDP_READ uDP_READ = this.storageUI_LCD_114.Get_UDP_READ(index_IP);
            uDP_READ.IsSensorOn(lightSensorClass);

        }
        private void PlC_RJ_Button_儲位管理_EPD583_清除燈號_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = sqL_DataGridView_儲位管理_EPD583_抽屜列表.Get_All_Select_RowsValues();
            if (list_value.Count == 0) return;
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
                taskList.Add(Task.Run(() =>
                {

                    if (drawer != null)
                    {
                        if (!this.drawerUI_EPD_583.Set_LED_Clear_UDP(drawer))
                        {
                            //MyMessageBox.ShowDialog($"{drawer.IP}:{drawer.Port} : EPD 抽屜滅燈失敗!");
                        }
                        Console.WriteLine($"{drawer.IP}:{drawer.Port} : EPD 抽屜成功!");
                    }
                }));
            }
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();

        }
        private void PlC_RJ_Button_儲位管理_EPD583_上傳至面板_MouseDownEvent(MouseEventArgs mevent)
        {

            List<object[]> list_value = sqL_DataGridView_儲位管理_EPD583_抽屜列表.Get_All_Select_RowsValues();
            if (list_value.Count == 0) return;
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
                taskList.Add(Task.Run(() =>
                {
                    if (drawer != null)
                    {
                        if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked)
                        {
                            if (!this.drawerUI_EPD_583.DrawToEpd_UDP(drawer))
                            {
                                //MyMessageBox.ShowDialog($"{drawer.IP}:{drawer.Port} : EPD 抽屜上傳失敗!");
                            }
                        }
                        else
                        {
                            if (!this.drawerUI_EPD_583.DrawToEpd_BarCode_UDP(drawer))
                            {
                                //MyMessageBox.ShowDialog($"{drawer.IP}:{drawer.Port} : EPD 抽屜上傳失敗!");
                            }
                        }
                        Console.WriteLine($"{drawer.IP}:{drawer.Port} : EPD 抽屜上傳成功!");
                    }
                }));
            }
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();
            return;
            //if (ControlMode)
            //{
            //    List<object[]> list_value = sqL_DataGridView_儲位管理_EPD583_抽屜列表.Get_All_Select_RowsValues();
            //    if (list_value.Count == 0) return;

            //    // 擷取選取的 IP 清單
            //    List<string> ipList = list_value
            //        .Select(row => row[(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString())
            //        .Where(ip => !string.IsNullOrWhiteSpace(ip))
            //        .Distinct()
            //        .ToList();

            //    if (ipList.Count == 0) return;

            //    // 呼叫後端 API
            //    var (code, result) = deviceApiClass.refresh_canvas_by_ip_full(Main_Form.API_Server, ipList, Main_Form.ServerName, Main_Form.ServerType);

            //    // 顯示結果（可依你系統決定是否彈窗或列印 log）
            //    if (code == 200)
            //    {
            //        Console.WriteLine($"✅ 成功上傳 {ipList.Count} 筆面板資料！\n{result}");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"❌ 上傳失敗！\n{result}");
            //    }
            //}
            //else
            //{
               
            //}






        }
        private void PlC_RJ_Button_儲位管理_EPD583_寫入_MouseDownEvent(MouseEventArgs mevent)
        {
            string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
            Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
            if (drawer != null)
            {
                drawer.Name = rJ_TextBox_儲位管理_EPD583_抽屜列表_儲位名稱.Text;
                drawer.Speaker = rJ_TextBox_儲位管理_EPD583_抽屜列表_語音.Text;
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                List_EPD583_本地資料.Add_NewDrawer(drawer);
                sqL_DataGridView_儲位管理_EPD583_抽屜列表.Replace(new object[] { drawer.IP, drawer.Name }, true);
                sqL_DataGridView_儲位管理_EPD583_抽屜列表.On_RowEnter();
                this.Function_設定雲端資料更新();
            }
        }
        private void PlC_RJ_Button_儲位管理_EPD583_藥品搜尋_藥品碼_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品碼.Text.StringIsEmpty()) return;
            List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.SQL_GetAllRows(false);
            list_value = list_value.GetRowsByLike((int)enum_藥品資料_藥檔資料.藥品碼, rJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品碼.Text);
            this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_儲位管理_EPD583_藥品搜尋_藥品名稱_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品名稱.Text.Length < 3)
            {
                MyMessageBox.ShowDialog("藥品名稱搜尋字元不得小於3個!");
                return;
            }
            List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.SQL_GetAllRows(false);
            if(rJ_RatioButton_儲位管理_EPD583_藥品搜尋_前綴.Checked)
            {
                list_value = list_value.GetRowsStartWithByLike((int)enum_藥品資料_藥檔資料.藥品名稱, rJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品名稱.Text);
            }
            else if (rJ_RatioButton_儲位管理_EPD583_藥品搜尋_模糊.Checked)
            {
                list_value = list_value.GetRowsByLike((int)enum_藥品資料_藥檔資料.藥品名稱, rJ_TextBox_儲位管理_EPD583_藥品搜尋_藥品名稱.Text);
            }


            this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_儲位管理_EPD583_藥品搜尋_填入資料_MouseDownEvent(MouseEventArgs mevent)
        {
            object[] value = this.sqL_DataGridView_儲位管理_EPD583_藥品資料_藥檔資料.GetRowValues();
            if(value == null) return;
            List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
            if (boxes.Count == 0)
            {
                MyMessageBox.ShowDialog("未選取儲位!");
                return;
            }
            boxes[0].Clear();
            boxes[0].SetValue(Device.ValueName.藥品碼, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.藥品碼]);
            boxes[0].SetValue(Device.ValueName.藥品名稱, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.藥品名稱]);
            boxes[0].SetValue(Device.ValueName.藥品學名, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.藥品學名]);
            boxes[0].SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.中文名稱]);
            boxes[0].SetValue(Device.ValueName.包裝單位, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.包裝單位]);
            boxes[0].SetValue(Device.ValueName.BarCode, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.藥品條碼]);

            if (value[(int)enum_藥品資料_藥檔資料.警訊藥品].ObjectToString().ToUpper() == true.ToString().ToUpper())
            {
                boxes[0].BackColor = Color.Red;
                boxes[0].ForeColor = Color.White;
            }
            else
            {
                boxes[0].BackColor = Color.White;
                boxes[0].ForeColor = Color.Black;
            }
            medClass _medClass = medClass.get_med_clouds_by_code(Main_Form.API_Server, boxes[0].Code);
            if (_medClass != null)
            {
                if (_medClass.storageInfo != null)
                {
                    List<string> storage_infos = new List<string>();
                   foreach (var storage in _medClass.storageInfo)
                    {
                        storage_infos.Add(storage.儲位描述);
            
                    }
                    string info_text = string.Join(",", storage_infos);
                    if (info_text.StringIsEmpty() == false)
                    {
                        boxes[0].StorageName = info_text;
                    }
                }

            }

            if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
            else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
            Drawer drawer = this.epD_583_Pannel.CurrentDrawer;
            deviceApiClass.Replace_EPD583_Drawers(API_Server, ServerName, ServerType, drawer);
            List_EPD583_本地資料.Add_NewDrawer(drawer);
            this.Function_設定雲端資料更新();
        }
        private void PlC_RJ_Button_儲位管理_EPD583_分割儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            (List<int> cols, List<int> rows) = this.epD_583_Pannel.GetSelect();
            Drawer drawer = deviceApiClass.separate_drawer_boxes(API_Server, ServerName, ServerType, this.epD_583_Pannel.CurrentDrawer, cols, rows);
            deviceApiClass.Replace_EPD583_Drawers(API_Server, ServerName, ServerType, drawer);
            this.epD_583_Pannel.DrawToPictureBox(drawer);
            List_EPD583_本地資料.Add_NewDrawer(drawer);
            this.Function_設定雲端資料更新();
        }
        private void PlC_RJ_Button_儲位管理_EPD583_合併儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            (List<int> cols, List<int> rows) = this.epD_583_Pannel.GetSelect();
            Drawer drawer = deviceApiClass.combine_drawer_boxes(API_Server, ServerName, ServerType, this.epD_583_Pannel.CurrentDrawer, cols, rows);
            deviceApiClass.Replace_EPD583_Drawers(API_Server, ServerName, ServerType, drawer);
            this.epD_583_Pannel.DrawToPictureBox(drawer);
            List_EPD583_本地資料.Add_NewDrawer(drawer);
            this.Function_設定雲端資料更新();
        }
        private void PlC_RJ_Button_儲位管理_EPD583_初始化儲位_MouseDownEvent(MouseEventArgs mevent)
        {
       
            if (MyMessageBox.ShowDialog("確認初始化儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
            {
                this.epD_583_Pannel.InitBoxes();
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                List_EPD583_本地資料.Add_NewDrawer(this.epD_583_Pannel.CurrentDrawer);
                this.Function_設定雲端資料更新();
            }
        }
        private void PlC_RJ_Button_儲位管理_EPD583_刪除儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("確認刪除選取儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
            {
                this.epD_583_Pannel.ClearBoxes();
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                List_EPD583_本地資料.Add_NewDrawer(this.epD_583_Pannel.CurrentDrawer);
                this.Function_設定雲端資料更新();
            }
        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_總庫存字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                this.fontDialog.Font = boxes[0].GetValue(Device.ValueName.庫存, Device.ValueType.Font) as Font;

                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    boxes[0].SetValue(Device.ValueName.庫存, Device.ValueType.Font, fontDialog.Font);
                    this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                }
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_儲位名稱字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                this.fontDialog.Font = boxes[0].GetValue(Device.ValueName.儲位名稱, Device.ValueType.Font) as Font;

                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    boxes[0].SetValue(Device.ValueName.儲位名稱, Device.ValueType.Font, fontDialog.Font);
                    this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                }
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_包裝單位字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                this.fontDialog.Font = boxes[0].GetValue(Device.ValueName.包裝單位, Device.ValueType.Font) as Font;

                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    boxes[0].SetValue(Device.ValueName.包裝單位, Device.ValueType.Font, fontDialog.Font);
                    this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                }
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品條碼字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                this.fontDialog.Font = boxes[0].GetValue(Device.ValueName.BarCode, Device.ValueType.Font) as Font;

                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    boxes[0].SetValue(Device.ValueName.BarCode, Device.ValueType.Font, fontDialog.Font);
                    this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                }
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品碼字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                this.fontDialog.Font = boxes[0].GetValue(Device.ValueName.藥品碼, Device.ValueType.Font) as Font;

                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    boxes[0].SetValue(Device.ValueName.藥品碼, Device.ValueType.Font, fontDialog.Font);
                    this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                }
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_中文名稱字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                this.fontDialog.Font = boxes[0].GetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Font) as Font;

                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    boxes[0].SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Font, fontDialog.Font);
                    this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                }
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品學名字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                this.fontDialog.Font = boxes[0].GetValue(Device.ValueName.藥品學名, Device.ValueType.Font) as Font;

                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    boxes[0].SetValue(Device.ValueName.藥品學名, Device.ValueType.Font, fontDialog.Font);
                    this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                }
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品名稱字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                this.fontDialog.Font = boxes[0].GetValue(Device.ValueName.藥品名稱, Device.ValueType.Font) as Font;

                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    boxes[0].SetValue(Device.ValueName.藥品名稱, Device.ValueType.Font, fontDialog.Font);
                    this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                }
            }));
     
        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_效期字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                this.fontDialog.Font = boxes[0].GetValue(Device.ValueName.效期, Device.ValueType.Font) as Font;

                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {

                    boxes[0].SetValue(Device.ValueName.效期, Device.ValueType.Font, fontDialog.Font);
                    this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                    if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                    this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
                }
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_效期管理_新增效期_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇儲位!");
                    return;
                }
                string 效期 = "";
                string 批號 = "";
                string 數量 = "";
                Dialog_DateTime dialog_DateTime = new Dialog_DateTime();
                if (dialog_DateTime.ShowDialog() == DialogResult.Yes)
                {
                    效期 = dialog_DateTime.Value.ToDateString();
                }
                else
                {
                    return;
                }
                Dialog_輸入批號 dialog_輸入批號 = new Dialog_輸入批號();
                if (dialog_輸入批號.ShowDialog() == DialogResult.Yes)
                {
                    批號 = dialog_輸入批號.Value;
                }
                else
                {
                    return;
                }
                Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                if (dialog_NumPannel.ShowDialog() == DialogResult.Yes)
                {
                    數量 = dialog_NumPannel.Value.ToString();
                }
                else
                {
                    return;
                }

                double 原有庫存 = boxes[0].取得庫存();
                string 藥品碼 = boxes[0].Code;
                藥品碼 = Function_藥品碼檢查(藥品碼);
                string 庫存量 = Function_從SQL取得庫存(藥品碼).ToString();
                boxes[0].效期庫存覆蓋(效期, 批號, 數量);
                double 修正庫存 = boxes[0].取得庫存();
                epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(epD_583_Pannel.CurrentDrawer);
                List_EPD583_本地資料.Add_NewDrawer(epD_583_Pannel.CurrentDrawer);

                string GUID = Guid.NewGuid().ToString();
                string 動作 = enum_交易記錄查詢動作.效期庫存異動.GetEnumName();
                string 藥品名稱 = boxes[0].Name;
                string 藥袋序號 = "";
                string 交易量 = (修正庫存 - 原有庫存).ToString();
                string 結存量 = Function_從SQL取得庫存(藥品碼).ToString();
                string 操作人 = this.登入者名稱;
                string 病人姓名 = "";
                string 病歷號 = "";
                string 操作時間 = DateTime.Now.ToDateTimeString_6();
                string 開方時間 = DateTime.Now.ToDateTimeString_6();
                string 備註 = $"[效期]:{效期},[批號]:{批號}";
                object[] value_trading = new object[new enum_交易記錄查詢資料().GetLength()];
                value_trading[(int)enum_交易記錄查詢資料.GUID] = GUID;
                value_trading[(int)enum_交易記錄查詢資料.動作] = 動作;
                value_trading[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
                value_trading[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
                value_trading[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
                value_trading[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
                value_trading[(int)enum_交易記錄查詢資料.交易量] = 交易量;
                value_trading[(int)enum_交易記錄查詢資料.結存量] = 結存量;
                value_trading[(int)enum_交易記錄查詢資料.操作人] = 操作人;
                value_trading[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
                value_trading[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
                value_trading[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
                value_trading[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
                value_trading[(int)enum_交易記錄查詢資料.備註] = 備註;
                value_trading[(int)enum_交易記錄查詢資料.收支原因] = "庫存異動";
                value_trading[(int)enum_交易記錄查詢資料.藥師證字號] = this.登入者藥師證字號;
                this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value_trading, false);
                this.Invoke(new Action(delegate { EpD_583_Pannel_MouseDownEvent(boxes); }));
                this.Function_設定雲端資料更新();
                sqL_DataGridView_儲位管理_EPD583_抽屜列表.On_RowEnter();
            }));
      
        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_效期管理_修正庫存_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇儲位!");
                    return;
                }
                object[] value = sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.GetRowValues();
                if (value == null)
                {
                    MyMessageBox.ShowDialog("未選擇效期!");
                    return;
                }
                string 效期 = value[(int)enum_儲位管理_EPD583_效期及庫存.效期].ObjectToString();
                string 批號 = value[(int)enum_儲位管理_EPD583_效期及庫存.批號].ObjectToString();
                string 數量 = "";
                Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                if (dialog_NumPannel.ShowDialog() == DialogResult.Yes)
                {
                    數量 = dialog_NumPannel.Value.ToString();
                }
                else
                {
                    return;
                }

                double 原有庫存 = boxes[0].取得庫存();
                string 藥品碼 = boxes[0].Code;
                藥品碼 = Function_藥品碼檢查(藥品碼);
                string 庫存量 = Function_從SQL取得庫存(藥品碼).ToString();
                boxes[0].效期庫存覆蓋(效期, 批號, 數量);
                double 修正庫存 = boxes[0].取得庫存();
                epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(epD_583_Pannel.CurrentDrawer);

                string GUID = Guid.NewGuid().ToString();
                string 動作 = enum_交易記錄查詢動作.效期庫存異動.GetEnumName();
                string 藥品名稱 = boxes[0].Name;
                string 藥袋序號 = "";
                string 交易量 = (修正庫存 - 原有庫存).ToString();
                string 結存量 = Function_從SQL取得庫存(藥品碼).ToString();
                string 操作人 = this.登入者名稱;
                string 病人姓名 = "";
                string 病歷號 = "";
                string 操作時間 = DateTime.Now.ToDateTimeString_6();
                string 開方時間 = DateTime.Now.ToDateTimeString_6();
                string 備註 = $"[效期]:{效期},[批號]:{批號}";
                object[] value_trading = new object[new enum_交易記錄查詢資料().GetLength()];
                value_trading[(int)enum_交易記錄查詢資料.GUID] = GUID;
                value_trading[(int)enum_交易記錄查詢資料.動作] = 動作;
                value_trading[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
                value_trading[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
                value_trading[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
                value_trading[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
                value_trading[(int)enum_交易記錄查詢資料.交易量] = 交易量;
                value_trading[(int)enum_交易記錄查詢資料.結存量] = 結存量;
                value_trading[(int)enum_交易記錄查詢資料.操作人] = 操作人;
                value_trading[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
                value_trading[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
                value_trading[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
                value_trading[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
                value_trading[(int)enum_交易記錄查詢資料.備註] = 備註;
                value_trading[(int)enum_交易記錄查詢資料.收支原因] = "庫存異動";
                value_trading[(int)enum_交易記錄查詢資料.藥師證字號] = this.登入者藥師證字號;
                this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value_trading, false);
                this.Invoke(new Action(delegate { EpD_583_Pannel_MouseDownEvent(boxes); }));
                this.Function_設定雲端資料更新();
                sqL_DataGridView_儲位管理_EPD583_抽屜列表.On_RowEnter();
            }));     
        }
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_效期管理_修正批號_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇儲位!");
                    return;
                }
                object[] value = sqL_DataGridView_儲位管理_EPD583_儲位內容_效期及庫存.GetRowValues();
                if (value == null)
                {
                    MyMessageBox.ShowDialog("未選擇效期!");
                    return;
                }

                string 效期 = value[(int)enum_儲位管理_EPD583_效期及庫存.效期].ObjectToString();
                string 舊批號 = value[(int)enum_儲位管理_EPD583_效期及庫存.批號].ObjectToString();
                string 新批號 = "";

                Dialog_輸入批號 dialog_輸入批號 = new Dialog_輸入批號();
                if (dialog_輸入批號.ShowDialog() == DialogResult.Yes)
                {
                    新批號 = dialog_輸入批號.Value;
                }
                else
                {
                    return;
                }

                boxes[0].修正批號(效期, 新批號);
                epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                List_EPD583_本地資料.Add_NewDrawer(epD_583_Pannel.CurrentDrawer);
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(epD_583_Pannel.CurrentDrawer);

                string GUID = Guid.NewGuid().ToString();
                string 動作 = enum_交易記錄查詢動作.效期庫存異動.GetEnumName();
                string 藥品名稱 = boxes[0].Name;
                string 藥袋序號 = "";
                string 交易量 = (0).ToString();
                string 結存量 = 0.ToString();
                string 操作人 = this.登入者名稱;
                string 病人姓名 = "";
                string 病歷號 = "";
                string 操作時間 = DateTime.Now.ToDateTimeString_6();
                string 開方時間 = DateTime.Now.ToDateTimeString_6();
                string 備註 = $"[效期]:{效期},[批號]:{新批號}";
                object[] value_trading = new object[new enum_交易記錄查詢資料().GetLength()];
                value_trading[(int)enum_交易記錄查詢資料.GUID] = GUID;
                value_trading[(int)enum_交易記錄查詢資料.動作] = 動作;
                value_trading[(int)enum_交易記錄查詢資料.藥品碼] = "";
                value_trading[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
                value_trading[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
                value_trading[(int)enum_交易記錄查詢資料.庫存量] = 0.ToString();
                value_trading[(int)enum_交易記錄查詢資料.交易量] = 交易量;
                value_trading[(int)enum_交易記錄查詢資料.結存量] = 結存量;
                value_trading[(int)enum_交易記錄查詢資料.操作人] = 操作人;
                value_trading[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
                value_trading[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
                value_trading[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
                value_trading[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
                value_trading[(int)enum_交易記錄查詢資料.備註] = 備註;
                value_trading[(int)enum_交易記錄查詢資料.收支原因] = "庫存異動";
                value_trading[(int)enum_交易記錄查詢資料.藥師證字號] = this.登入者藥師證字號;
                this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value_trading, false);
                this.Invoke(new Action(delegate { EpD_583_Pannel_MouseDownEvent(boxes); }));
                this.Function_設定雲端資料更新();
                sqL_DataGridView_儲位管理_EPD583_抽屜列表.On_RowEnter();

            }));
        }
    
        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_藥品碼_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            string text = "";
            string comboBox_text = "";
            this.Invoke(new Action(delegate
            {
                text = this.rJ_TextBox_儲位管理_EPD583_儲位內容_儲位搜尋_藥品碼.Text;
                comboBox_text = this.comboBox_儲位管理_EPD583_儲位內容_儲位搜尋.Text;
            }));
           
            if (text.StringIsEmpty()) return;
            List<Box> list_boxes = new List<Box>();
            int select_index = -1;
            if (comboBox_text == "藥碼")
            {
                list_boxes = List_EPD583_本地資料.SortLikeByCode(text.ToUpper());
            }
            if (comboBox_text == "藥名")
            {
                List<Box> list_boxes_buf = List_EPD583_本地資料.GetAllBoxes();
                list_boxes_buf = (from temp in list_boxes_buf
                                  where temp.Name.ToUpper().Contains(text.ToUpper())
                                  select temp).ToList();
                list_boxes = list_boxes_buf;
            }
            if (comboBox_text == "商品名")
            {
                List<Box> list_boxes_buf = List_EPD583_本地資料.GetAllBoxes();
                list_boxes_buf = (from temp in list_boxes_buf
                                  where temp.Scientific_Name.ToUpper().Contains(text.ToUpper())
                                  select temp).ToList();
                list_boxes = list_boxes_buf;
            }


            if (list_boxes.Count == 0)
            {
                MyMessageBox.ShowDialog("抽屜欄位無此藥品!!");
                return;
            }  
            string IP ="0.0.0.0";
            if (this.epD_583_Pannel.CurrentDrawer != null) IP = this.epD_583_Pannel.CurrentDrawer.IP;
            List<int> select_cols = new List<int>();
            List<int> select_rows = new List<int>();
            this.epD_583_Pannel.GetSelectBoxes(ref select_cols, ref select_rows);
            for (int i = 0; i < list_boxes.Count; i++)
            {
                if (list_boxes[i].IP == IP)
                {
                    for (int k = 0; k < select_cols.Count; k++)
                    {
                        if (list_boxes[i].Column == select_cols[k])
                        {
                            if (list_boxes[i].Row == select_rows[k])
                            {
                                select_index = i;
                                break;
                            }
                        }
                    }
                }
            }
            Box box;
            if (select_index == -1)
            {
   
                box = list_boxes[0];
            }
            else if ((select_index + 1) == list_boxes.Count)
            {
                box = list_boxes[0];
            }
            else
            {
                box = list_boxes[select_index + 1];
            }


            Drawer drawer = List_EPD583_本地資料.SortByIP(box.IP);
            if (drawer != null) this.epD_583_Pannel.SetSelectBox(drawer , box.Column, box.Row);
        }
        private void PlC_RJ_Button_儲位管理_EPD583_開鎖_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.Get_All_Select_RowsValues();
            string IP = "";
            string ID_Name = this.登入者名稱;

            foreach (object[] value in list_value)
            {
                IP = value[(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                // Function_取藥堆疊資料_新增母資料(Guid.NewGuid().ToString(), this.textBox_工程模式_領藥台_01_名稱.Text, enum_取藥堆疊_TYPE.EPD_5_83鎖控, "", IP, 登入者名稱, Color.Lime.ToColorString(), 0);
                List<object[]> list_locker_table_value = this.sqL_DataGridView_Locker_Index_Table.SQL_GetRows(enum_lockerIndex.IP.GetEnumName(), IP, false);
                if (list_locker_table_value.Count > 0)
                {
                    list_locker_table_value[0][(int)enum_lockerIndex.輸出狀態] = true.ToString();
                    this.sqL_DataGridView_Locker_Index_Table.SQL_Replace(list_locker_table_value[0], false);
                }
                string GUID = Guid.NewGuid().ToString();
                string 動作 = enum_交易記錄查詢動作.操作工程模式.GetEnumName();
                string 藥品名稱 = "";
                string 藥袋序號 = "";
                string 藥品碼 = "";
                string 庫存量 = 0.ToString();
                string 交易量 = 0.ToString();
                string 結存量 = 0.ToString();
                string 病人姓名 = "";
                string 操作人 = ID_Name;
                string 病歷號 = "";
                string 操作時間 = DateTime.Now.ToDateTimeString_6();
                string 開方時間 = DateTime.Now.ToDateTimeString_6();
                string 備註 = $"儲位名稱[{value[(int)enum_儲位管理_EPD583_抽屜列表.名稱].ObjectToString()}]";
                object[] value_trading = new object[new enum_交易記錄查詢資料().GetLength()];
                value_trading[(int)enum_交易記錄查詢資料.GUID] = GUID;
                value_trading[(int)enum_交易記錄查詢資料.動作] = 動作;
                value_trading[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
                value_trading[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
                value_trading[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
                value_trading[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
                value_trading[(int)enum_交易記錄查詢資料.交易量] = 交易量;
                value_trading[(int)enum_交易記錄查詢資料.結存量] = 結存量;
                value_trading[(int)enum_交易記錄查詢資料.操作人] = 操作人;
                value_trading[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
                value_trading[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
                value_trading[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
                value_trading[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
                value_trading[(int)enum_交易記錄查詢資料.備註] = 備註;
                this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value_trading, false);
            }
        }
        private void PlC_RJ_Button_儲位管理_EPD583_複製儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.Get_All_Select_RowsValues();
            if(list_value.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("未選擇儲位!");
                }));
                return;
            }
            string IP = list_value[0][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();

            Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
            if (drawer == null)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog($"未搜尋到 {IP} 儲位!");
                }));
                return;
            }
            EPD583_Drawer_Copy = drawer;
            MyMessageBox.ShowDialog("已複製到剪貼簿!");

        }
        private void PlC_RJ_Button_儲位管理_EPD583_貼上儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            if (EPD583_Drawer_Copy == null)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog($"尚未複製儲位!");
                }));
                return;
            }

            List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.Get_All_Select_RowsValues();
            if (list_value.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("未選擇儲位!");
                }));
                return;
            }
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate
            {
                dialogResult =  MyMessageBox.ShowDialog("是否覆蓋選取儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel);
            }));
            if (dialogResult != DialogResult.Yes) return;
            string IP = list_value[0][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
            Drawer drawer = EPD583_Drawer_Copy.DeepClone();
            drawer.ReplaceIP(IP);
            this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
            List_EPD583_本地資料.Add_NewDrawer(drawer);
            sqL_DataGridView_儲位管理_EPD583_抽屜列表.Replace(new object[] { drawer.IP, drawer.Name }, true);
            sqL_DataGridView_儲位管理_EPD583_抽屜列表.On_RowEnter();
            this.Function_設定雲端資料更新();
            MyMessageBox.ShowDialog("貼上格式完成!");
        }
        private void PlC_RJ_Button_儲位管理_EPD583_複製格式_MouseDownEvent(MouseEventArgs mevent)
        {
            List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
            if (boxes.Count == 0)
            {
                MyMessageBox.ShowDialog("未選取儲位!");
                return;
            }
            EPD583_Box_Copy = boxes[0];
            MyMessageBox.ShowDialog("已複製到剪貼簿!");
        }
        private void PlC_RJ_Button_儲位管理_EPD583_貼上格式_MouseDownEvent(MouseEventArgs mevent)
        {
            if (EPD583_Box_Copy == null)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog($"尚未複製儲位!");
                }));
                return;
            }
            List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
            if (boxes.Count == 0)
            {
                MyMessageBox.ShowDialog("未選取儲位!");
                return;
            }
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate
            {
                dialogResult = MyMessageBox.ShowDialog("是否覆蓋選取儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel);
            }));
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].PasteFormat(EPD583_Box_Copy);
            }
            Drawer drawer = List_EPD583_本地資料.SortByIP(boxes[0].IP);
            for (int i = 0; i < boxes.Count; i++) drawer.ReplaceBox(boxes[i]);
            List_EPD583_本地資料.Add_NewDrawer(drawer);
            this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
            this.Function_設定雲端資料更新();
            if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
            else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
            MyMessageBox.ShowDialog("貼上格式完成!");
        }
        private void PlC_CheckBox_儲位管理_EPD583_儲位內容_效期顯示_CheckStateChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                boxes[0].SetValue(Device.ValueName.效期, Device.ValueType.Visable, this.plC_CheckBox_儲位管理_EPD583_儲位內容_效期顯示.Checked);
                this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
            }));
        }
        private void PlC_CheckBox_儲位管理_EPD583_儲位內容_庫存顯示_CheckStateChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                boxes[0].SetValue(Device.ValueName.庫存, Device.ValueType.Visable, this.plC_CheckBox_儲位管理_EPD583_儲位內容_庫存顯示.Checked);
                this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
            }));
        }
        private void PlC_CheckBox_儲位管理_EPD583_儲位內容_藥品碼顯示_CheckStateChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate
            {
                List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
                if (boxes.Count == 0) return;
                boxes[0].SetValue(Device.ValueName.藥品碼, Device.ValueType.Visable, this.plC_CheckBox_儲位管理_EPD583_儲位內容_藥品碼顯示.Checked);
                this.epD_583_Pannel.CurrentDrawer.ReplaceBox(boxes[0]);
                if (!plC_CheckBox_儲位管理_EPD583_顯示為條碼.Checked) this.epD_583_Pannel.DrawToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                else this.epD_583_Pannel.DrawBarCodeToPictureBox(this.epD_583_Pannel.CurrentDrawer);
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(this.epD_583_Pannel.CurrentDrawer);
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD583_更新_MouseDownEvent(MouseEventArgs mevent)
        {
            string IP = rJ_TextBox_儲位管理_EPD583_抽屜列表_IP.Texts;
            Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(IP);
            if (drawer == null) return;
            bool input = this.drawerUI_EPD_583.GetInput(drawer);
            bool output = this.drawerUI_EPD_583.GetOutput(drawer);
            bool input_dir = this.drawerUI_EPD_583.GetInput_dir(drawer);
            bool output_dir = this.drawerUI_EPD_583.GetOutput_dir(drawer);
            this.Invoke(new Action(delegate 
            {
                plC_CheckBox_儲位管理_EPD583_輸入.Checked = input;
                plC_CheckBox_儲位管理_EPD583_輸出.Checked = output;

                plC_CheckBox_儲位管理_EPD583_輸入方向.Checked = input_dir;
                plC_CheckBox_儲位管理_EPD583_輸出方向.Checked = output_dir;
            }));

        }

        private void PlC_RJ_Button_儲位管理_EPD583_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                try
                {
                    Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_儲位管理_EPD583_匯出());
                    if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                    {
                        if (dialog_ContextMenuStrip.Value == ContextMenuStrip_儲位管理_EPD583_匯出.匯出建置表.GetEnumName())
                        {
                            DialogResult dialogResult = DialogResult.None;
                            this.Invoke(new Action(delegate
                            {
                                dialogResult = this.saveFileDialog_SaveExcel.ShowDialog();
                            }));
                            if (dialogResult != DialogResult.OK) return;
                            List<SheetClass> sheetClasses = new List<SheetClass>();
                            List<object[]> list_抽屜列表 = this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.GetAllRows();
                            for (int d = 0; d < list_抽屜列表.Count; d++)
                            {
                                string IP = list_抽屜列表[d][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                                Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
                                if (drawer == null) continue;
                                List<Box[]> Boxes = drawer.Boxes;
                                SheetClass sheetClass = new SheetClass(drawer.Name);
                                sheetClass.ColumnsWidth.Add(10000);
                                sheetClass.ColumnsWidth.Add(10000);
                                sheetClass.ColumnsWidth.Add(10000);
                                sheetClass.ColumnsWidth.Add(10000);


                                for (int i = 0; i < Boxes.Count; i++)
                                {
                                    for (int k = 0; k < Boxes[i].Length; k++)
                                    {
                                        Rectangle rect = DrawerUI_EPD_583.Get_Box_Combine(drawer, Boxes[i][k]);
                                        Box _box = Boxes[i][k];
                                        int width = _box.Width;
                                        int height = _box.Height;
                                        rect.X /= width;
                                        rect.Y /= height;
                                        rect.Width /= width;
                                        rect.Height /= height;
                                        if (Boxes[i][k].Slave == false)
                                        {
                                            int colStart = rect.X;
                                            int colEnd = rect.X + rect.Width - 1;
                                            int rowStart = rect.Y;
                                            int rowEnd = rect.Y + rect.Height - 1;
                                            sheetClass.AddNewCell(rowStart, rowEnd, colStart, colEnd, $"{_box.Code}({_box.Name})", new Font("微軟正黑體", 14), 1000);
                                            //sheetClass.SetSlave(k, i, false);
                                        }
                                        else
                                        {
                                            sheetClass.AddNewCell(k, i, $"", new Font("微軟正黑體", 14), 1000);
                                            sheetClass.SetSlave(k, i, true);
                                        }
                                    }
                                }
                                sheetClasses.Add(sheetClass);
                            }
                            sheetClasses.NPOI_SaveFile(this.saveFileDialog_SaveExcel.FileName);
                            MyMessageBox.ShowDialog("匯出完成!");
                        }
                        if (dialog_ContextMenuStrip.Value == ContextMenuStrip_儲位管理_EPD583_匯出.匯出儲位表.GetEnumName())
                        {
                            DialogResult dialogResult = DialogResult.None;
                            this.Invoke(new Action(delegate
                            {
                                dialogResult = this.saveFileDialog_SaveExcel.ShowDialog();
                            }));
                            if (dialogResult != DialogResult.OK) return;
                            List<SheetClass> sheetClasses = new List<SheetClass>();
                            List<object[]> list_抽屜列表 = this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.GetAllRows();
                            List<object[]> list_匯出資料 = new List<object[]>();

                            for (int d = 0; d < list_抽屜列表.Count; d++)
                            {
                                string IP = list_抽屜列表[d][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                                Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
                                if (drawer == null) continue;
                                List<Box[]> Boxes = drawer.Boxes;
                                for (int i = 0; i < Boxes.Count; i++)
                                {
                                    for (int k = 0; k < Boxes[i].Length; k++)
                                    {
                                        if (Boxes[i][k].Slave == false)
                                        {
                                            if (Boxes[i][k].Code.StringIsEmpty()) continue;
                                            object[] value = new object[new enum_儲位管理_EPD583_匯出儲位表().GetLength()];
                                            value[(int)enum_儲位管理_EPD583_匯出儲位表.藥碼] = Boxes[i][k].Code;
                                            value[(int)enum_儲位管理_EPD583_匯出儲位表.藥名] = Boxes[i][k].Name;
                                            value[(int)enum_儲位管理_EPD583_匯出儲位表.單位] = Boxes[i][k].Package;
                                            value[(int)enum_儲位管理_EPD583_匯出儲位表.儲位名稱] = drawer.Name;
                                            list_匯出資料.Add(value);
                                        }

                                    }
                                }
                            }
                            DataTable dataTable = list_匯出資料.ToDataTable(new enum_儲位管理_EPD583_匯出儲位表());
                            dataTable.NPOI_SaveFile(saveFileDialog_SaveExcel.FileName);

                            MyMessageBox.ShowDialog("匯出完成!");
                        }
                    }
                }
                catch
                {

                }
                finally
                {

                }
            }));
           
          
             

        }
        private void PlC_RJ_Button_儲位管理_EPD583_匯入_MouseDownEvent(MouseEventArgs mevent)
        {
            //bool flag_replaceByName = false;
            if (MyMessageBox.ShowDialog("確認匯入所有儲位?將會全部覆蓋!", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            //if (MyMessageBox.ShowDialog("是否依照儲位名稱覆蓋?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes) flag_replaceByName = true;
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate
            {
                dialogResult = this.openFileDialog_LoadExcel.ShowDialog();
            }));
            if (dialogResult != DialogResult.OK) return;
            List<SheetClass> sheetClasses = MyOffice.ExcelClass.NPOI_LoadToSheetClasses(this.openFileDialog_LoadExcel.FileName);
            for (int i = 0; i < sheetClasses.Count; i++)
            {
                string 儲位名稱 = sheetClasses[i].Name;
                Drawer drawer = List_EPD583_本地資料.SortByName(儲位名稱);
                if (drawer == null) continue;
                //drawer = this.epD_583_Pannel.SeparateBoxesAll(drawer);
                for (int k = 0; k < sheetClasses[i].CellValues.Count; k++)
                {
                    if (sheetClasses[i].CellValues[k].Slave == false)
                    {
                        string 藥品碼 = sheetClasses[i].CellValues[k].Text;
                        藥品碼 = RemoveParenthesesContent(藥品碼);
                        int colStart = sheetClasses[i].CellValues[k].ColStart;
                        int colEnd = sheetClasses[i].CellValues[k].ColEnd;
                        int rowStart = sheetClasses[i].CellValues[k].RowStart;
                        int rowEnd = sheetClasses[i].CellValues[k].RowEnd;
                        List<int> list_cols = new List<int>();
                        List<int> list_rows = new List<int>();
                        for (int col = colStart; col <= colEnd; col++)
                        {
                            for (int row = rowStart; row <= rowEnd; row++)
                            {
                                list_cols.Add(col);
                                list_rows.Add(row);                           
                            }
                        }
                        drawer = this.epD_583_Pannel.CombineBoxes(list_cols, list_rows, drawer);
                        drawer.Boxes[list_cols[0]][list_rows[0]].Code = 藥品碼;
                    }
                   
                }
                this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
                List_EPD583_本地資料.Add_NewDrawer(drawer);
                this.epD_583_Pannel.DrawToPictureBox(drawer);
            }
            List<object[]> list_抽屜列表 = this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.Get_All_Select_RowsValues();
            PLC_Device_儲位管理_EPD583_資料更新.Bool = true;
            while(true)
            {
                if (!PLC_Device_儲位管理_EPD583_資料更新.Bool) break;
                System.Threading.Thread.Sleep(10);
            }
            if (list_抽屜列表.Count > 0)
            {
                string IP = list_抽屜列表[0][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
                if (drawer != null) this.epD_583_Pannel.DrawToPictureBox(drawer);
            }
            this.Function_設定雲端資料更新();
            MyMessageBox.ShowDialog("匯入完成!");
        }
        private void PlC_RJ_Button_儲位管理_EPD583_自動填入儲位名稱_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("確認自動填入儲位名稱?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;

            List<object[]> list_儲位列表 = this.sqL_DataGridView_儲位管理_EPD583_抽屜列表.GetAllRows();
            int index = 1;
            for (int i = 0; i < list_儲位列表.Count; i++)
            {
                string IP = list_儲位列表[i][(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                Drawer drawer = List_EPD583_本地資料.SortByIP(IP);
                if (drawer == null) continue;
                drawer.Name = $"{i / 4 + 1}-{index}";
                index++;
                List_EPD583_本地資料.Add_NewDrawer(drawer);
                if (index == 5) index = 1;
            }
            this.drawerUI_EPD_583.SQL_ReplaceDrawer(List_EPD583_本地資料);
            this.Function_設定雲端資料更新();
            PLC_Device_儲位管理_EPD583_資料更新.Bool = true;
            while (true)
            {
                if (PLC_Device_儲位管理_EPD583_資料更新.Bool == false) break;
                System.Threading.Thread.Sleep(10);
            }

        }

        private void PlC_RJ_Button_儲位管理_EPD583_儲位內容_儲存_MouseDownEvent(MouseEventArgs mevent)
        {
            Drawer drawer = this.epD_583_Pannel.CurrentDrawer;
            List<Box> boxes = this.epD_583_Pannel.GetSelectBoxes();
            if (boxes.Count == 0) return;
            boxes[0].Picture1_Name = comboBox_儲位管理_EPD583_儲位內容_圖片1.GetComboBoxText();
            boxes[0].Picture2_Name = comboBox_儲位管理_EPD583_儲位內容_圖片2.GetComboBoxText();
            deviceApiClass.Replace_EPD583_Drawers(API_Server, ServerName, ServerType, drawer);
            this.epD_583_Pannel.DrawToPictureBox(drawer);
            MyMessageBox.ShowDialog("儲存完成!");
        }
        #endregion

        private class ICP_儲位管理_EPD583_抽屜列表 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string IP_0 = x[(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                string IP_1 = y[(int)enum_儲位管理_EPD583_抽屜列表.IP].ObjectToString();
                string[] IP_0_Array = IP_0.Split('.');
                string[] IP_1_Array = IP_1.Split('.');
                IP_0 = "";
                IP_1 = "";
                for (int i = 0; i < 4; i++)
                {
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];

                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];

                    IP_0 += IP_0_Array[i];
                    IP_1 += IP_1_Array[i];
                }
                int cmp = IP_0_Array[2].CompareTo(IP_1_Array[2]);
                if (cmp > 0)
                {
                    return 1;
                }
                else if (cmp < 0)
                {
                    return -1;
                }
                else if (cmp == 0)
                {
                    cmp = IP_0_Array[3].CompareTo(IP_1_Array[3]);
                    if (cmp > 0)
                    {
                        return 1;
                    }
                    else if (cmp < 0)
                    {
                        return -1;
                    }
                    else if (cmp == 0)
                    {
                        return 0;
                    }
                }

                return 0;

            }
        }
    }
}
