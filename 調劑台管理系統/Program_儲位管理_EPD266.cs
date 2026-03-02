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
using SQLUI;
namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
        public enum ContextMenuStrip_儲位管理_EPD266_匯出
        {
            [Description("M8000")]
            匯出建置表,
            [Description("M8000")]
            匯出儲位表,
        }
        [EnumDescription("")]
        private enum enum_儲位管理_EPD266_匯出儲位表
        {
            IP,
            藥碼,
            藥名,
            單位,
        }

        static public List<Storage> List_EPD266_本地資料 = new List<Storage>();
        static public List<Storage> List_EPD266_雲端資料 = new List<Storage>();
        static public List<Storage> List_EPD266_入賬資料 = new List<Storage>();
        private Storage EPD266_Storage_Copy;

        [EnumDescription("")]
        private enum enum_儲位管理_EPD266_效期及庫存
        {
            [Description("效期,VARCHAR,300,NONE")]
            效期,
            [Description("批號,VARCHAR,300,NONE")]
            批號,
            [Description("庫存,VARCHAR,300,NONE")]
            庫存,
        }
        [EnumDescription("")]
        private enum enum_儲位管理_EPD266_儲位資料
        {
            [Description("IP,VARCHAR,300,NONE")]
            IP,
            [Description("儲位名稱,VARCHAR,300,NONE")]
            儲位名稱,
            [Description("藥品碼,VARCHAR,300,NONE")]
            藥品碼,
            [Description("藥品名稱,VARCHAR,300,NONE")]
            藥品名稱,
            [Description("包裝單位,VARCHAR,300,NONE")]
            包裝單位,
            [Description("庫存,VARCHAR,300,NONE")]
            庫存,
            [Description("區域,VARCHAR,15,NONE")]
            區域,

        }

        private bool flag_Program_儲位管理_EPD266_Init = false;
        private void Program_儲位管理_EPD266_Init()
        {
            this.storagePanel.SureClick += StoragePanel_SureClick;
            this.sqL_DataGridView_儲位管理_EPD266_藥品資料_藥檔資料.Init(this.sqL_DataGridView_藥品資料_藥檔資料);
            this.sqL_DataGridView_儲位管理_EPD266_藥品資料_藥檔資料.Set_ColumnVisible(false, new enum_藥品資料_藥檔資料().GetEnumNames());
            this.sqL_DataGridView_儲位管理_EPD266_藥品資料_藥檔資料.Set_ColumnVisible(true, enum_藥品資料_藥檔資料.藥品碼, enum_藥品資料_藥檔資料.藥品名稱);
            this.sqL_DataGridView_儲位管理_EPD266_藥品資料_藥檔資料.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品碼);
            this.sqL_DataGridView_儲位管理_EPD266_藥品資料_藥檔資料.Set_ColumnWidth(350, DataGridViewContentAlignment.MiddleLeft, enum_藥品資料_藥檔資料.藥品名稱);

            Table table_儲架電子紙列表 = new Table(new enum_儲位管理_EPD266_儲位資料());
            table_儲架電子紙列表[enum_儲位管理_EPD266_儲位資料.區域.GetEnumName()].TypeName = Table.GetTypeName(Table.OtherType.ENUM, Main_Form.Function_取得藥品區域名稱().ToArray());

            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.RowsHeight = 40;
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Init(table_儲架電子紙列表);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnVisible(false, new enum_儲位管理_EPD266_儲位資料().GetEnumNames());
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD266_儲位資料.IP);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD266_儲位資料.儲位名稱);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD266_儲位資料.藥品碼);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnWidth(750, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD266_儲位資料.藥品名稱);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_EPD266_儲位資料.包裝單位);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_EPD266_儲位資料.庫存);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_EPD266_儲位資料.區域);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnText("藥碼", enum_儲位管理_EPD266_儲位資料.藥品碼);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnText("藥名", enum_儲位管理_EPD266_儲位資料.藥品名稱);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Set_ColumnText("單位", enum_儲位管理_EPD266_儲位資料.包裝單位);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.RowEnterEvent += SqL_DataGridView_儲位管理_EPD266_儲位資料_RowEnterEvent;
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.ComboBoxSelectedIndexChangedEvent += SqL_DataGridView_儲位管理_EPD266_儲位資料_ComboBoxSelectedIndexChangedEvent;

            Table table = new SQLUI.Table(new enum_儲位管理_EPD266_效期及庫存());
            this.sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.Init(table);
            this.sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.Set_ColumnVisible(false, new enum_儲位管理_EPD266_效期及庫存().GetEnumNames());
            this.sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD266_效期及庫存.效期);
            this.sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD266_效期及庫存.批號);
            this.sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_EPD266_效期及庫存.庫存);

            this.rJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品碼.KeyPress += RJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品碼_KeyPress;
            this.rJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品名稱.KeyPress += RJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品名稱_KeyPress;
            this.rJ_TextBox_儲位管理_EPD266_儲位內容_儲位搜尋_藥品碼.KeyPress += RJ_TextBox_儲位管理_EPD266_儲位內容_儲位搜尋_藥品碼_KeyPress;
            this.rJ_TextBox_儲位管理_EPD266_儲位內容_語音.KeyPress += RJ_TextBox_儲位管理_EPD266_儲位內容_語音_KeyPress;

            this.plC_RJ_Button_儲位管理_EPD266_藥品搜尋_藥品碼_搜尋.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_藥品搜尋_藥品碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_藥品搜尋_藥品名稱_搜尋.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_藥品搜尋_藥品名稱_搜尋_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_藥品搜尋_填入資料.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_藥品搜尋_填入資料_MouseDownEvent;

            this.plC_RJ_Button_儲位管理_EPD266_儲位內容_效期管理_新增效期.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_儲位內容_效期管理_新增效期_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_儲位內容_效期管理_修正庫存.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_儲位內容_效期管理_修正庫存_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_儲位內容_效期管理_修正批號.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_儲位內容_效期管理_修正批號_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_面板亮燈.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_面板亮燈_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_清除燈號.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_清除燈號_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_上傳至面板.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_上傳至面板_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_刪除儲位.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_刪除儲位_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_儲位內容_儲位搜尋_藥品碼搜尋.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_儲位內容_儲位搜尋_藥品碼搜尋_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_開鎖.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_開鎖_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_貼上格式.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_貼上格式_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_複製格式.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_複製格式_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_儲位初始化.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_儲位初始化_MouseDownEvent;
            this.rJ_TextBox_儲位管理_EPD266_儲位名稱.KeyPress += RJ_TextBox_儲位管理_EPD266_儲位名稱_KeyPress;
            this.rJ_TextBox_儲位管理_EPD266_包裝數量.KeyPress += RJ_TextBox_儲位管理_EPD266_包裝數量_KeyPress;

            this.plC_CheckBox_儲位管理_EPD266_儲位內容_顯示空白儲位.CheckStateChanged += PlC_CheckBox_儲位管理_EPD266_儲位內容_顯示空白儲位_CheckStateChanged;
            this.plC_CheckBox_儲位管理_EPD266_儲位內容_手勢感測.CheckStateChanged += PlC_CheckBox_儲位管理_EPD266_儲位內容_手勢感測_CheckStateChanged;
            this.plC_CheckBox_儲位管理_EPD266_儲位內容_庫存儲位.CheckStateChanged += PlC_CheckBox_儲位管理_EPD266_儲位內容_庫存儲位_CheckStateChanged;
            this.plC_CheckBox_儲位管理_EPD266_警報.CheckStateChanged += PlC_RJ_Button_儲位管理_EPD266_警報_CheckStateChanged;


            this.plC_RJ_Button_儲位管理_EPD266_匯出.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_匯出_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_匯入.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_匯入_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_EPD266_自動填入儲位名稱.MouseDownEvent += PlC_RJ_Button_儲位管理_EPD266_自動填入儲位名稱_MouseDownEvent;

            this.storagePanel.SizeChanged += StoragePanel_SizeChanged;
            this.comboBox_儲位管理_EPD266_儲位內容_儲位搜尋.SelectedIndex = 0;
            this.plC_UI_Init.Add_Method(this.Program_儲位管理_EPD266);
        }

     

        private void StoragePanel_SizeChanged(object sender, EventArgs e)
        {
            this.storagePanel.Location = new Point((this.storagePanel.Parent.Width - this.storagePanel.Width) / 2, (this.storagePanel.Parent.Height - this.storagePanel.Height) / 2);
        }
        private void StoragePanel_SureClick(Storage storage)
        {
            _storageUI_EPD_266.SQL_ReplaceStorage(storage);
            this.storagePanel.DrawToPictureBox(storage);
        }

        private void Program_儲位管理_EPD266()
        {
            if (this.plC_ScreenPage_Main.PageText == "儲位管理" && this.plC_ScreenPage_儲位管理.PageText == "EPD266")
            {
                if (flag_Program_儲位管理_EPD266_Init == false)
                {
                    this.Invoke(new Action(delegate
                    {
                        this.storagePanel.Location = new Point((this.storagePanel.Parent.Width - this.storagePanel.Width) / 2, (this.storagePanel.Parent.Height - this.storagePanel.Height) / 2);
                    }));

                    PLC_Device_儲位管理_EPD266_資料更新.Bool = true;
                    flag_Program_儲位管理_EPD266_Init = true;
                }
            }
            else
            {
                flag_Program_儲位管理_EPD266_Init = false;
            }


            sub_Program_儲位管理_EPD266_資料更新();
        }

        #region PLC_儲位管理_EPD266_資料更新
        PLC_Device PLC_Device_儲位管理_EPD266_資料更新 = new PLC_Device("");
        int cnt_Program_儲位管理_EPD266_資料更新 = 65534;
        void sub_Program_儲位管理_EPD266_資料更新()
        {
            if (cnt_Program_儲位管理_EPD266_資料更新 == 65534)
            {
                PLC_Device_儲位管理_EPD266_資料更新.SetComment("PLC_儲位管理_EPD266_資料更新");
                PLC_Device_儲位管理_EPD266_資料更新.Bool = false;
                cnt_Program_儲位管理_EPD266_資料更新 = 65535;
            }
            if (cnt_Program_儲位管理_EPD266_資料更新 == 65535) cnt_Program_儲位管理_EPD266_資料更新 = 1;
            if (cnt_Program_儲位管理_EPD266_資料更新 == 1) cnt_Program_儲位管理_EPD266_資料更新_檢查按下(ref cnt_Program_儲位管理_EPD266_資料更新);
            if (cnt_Program_儲位管理_EPD266_資料更新 == 2) cnt_Program_儲位管理_EPD266_資料更新_初始化(ref cnt_Program_儲位管理_EPD266_資料更新);
            if (cnt_Program_儲位管理_EPD266_資料更新 == 3) cnt_Program_儲位管理_EPD266_資料更新_更新藥檔(ref cnt_Program_儲位管理_EPD266_資料更新);
            if (cnt_Program_儲位管理_EPD266_資料更新 == 4) cnt_Program_儲位管理_EPD266_資料更新_更新面板資料(ref cnt_Program_儲位管理_EPD266_資料更新);
            if (cnt_Program_儲位管理_EPD266_資料更新 == 5) cnt_Program_儲位管理_EPD266_資料更新 = 65500;
            if (cnt_Program_儲位管理_EPD266_資料更新 > 1) cnt_Program_儲位管理_EPD266_資料更新_檢查放開(ref cnt_Program_儲位管理_EPD266_資料更新);

            if (cnt_Program_儲位管理_EPD266_資料更新 == 65500)
            {
                PLC_Device_儲位管理_EPD266_資料更新.Bool = false;
                cnt_Program_儲位管理_EPD266_資料更新 = 65535;
            }
        }
        void cnt_Program_儲位管理_EPD266_資料更新_檢查按下(ref int cnt)
        {
            if (PLC_Device_儲位管理_EPD266_資料更新.Bool) cnt++;
        }
        void cnt_Program_儲位管理_EPD266_資料更新_檢查放開(ref int cnt)
        {
            if (!PLC_Device_儲位管理_EPD266_資料更新.Bool) cnt = 65500;
        }
        void cnt_Program_儲位管理_EPD266_資料更新_初始化(ref int cnt)
        {
            MyTimer_TickTime.TickStop();
            MyTimer_TickTime.StartTickTime(50000);
            List_EPD266_本地資料 = this.storageUI_EPD_266.SQL_GetAllStorage();
            Console.Write($"儲位管理EPD266:從SQL取得資料 ,耗時 :{MyTimer_TickTime.GetTickTime().ToString("0.000")}\n");
            cnt++;
        }
        void cnt_Program_儲位管理_EPD266_資料更新_更新藥檔(ref int cnt)
        {
            MyTimer_TickTime.TickStop();
            MyTimer_TickTime.StartTickTime(50000);

            List<object[]> list_藥品資料_藥檔資料 = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_藥品設定表 = this.sqL_DataGridView_藥品設定表.SQL_GetAllRows(false);

            List<Storage> list_replaceValue = new List<Storage>();


            Parallel.ForEach(List_EPD266_本地資料, value =>
            {
                if (List_EPD266_本地資料 == null) return;
                List<object[]> list_藥品資料_藥檔資料_buf = new List<object[]>();
                List<object[]> list_藥品設定表_buf = new List<object[]>();

                string 藥品碼 = "";
                string 料號 = "";
                string 藥品名稱 = "";
                string 中文名稱 = "";
                string 藥品學名 = "";
                string BarCode = "";
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
                string BarCode_buf = "";
                string 包裝單位_buf = "";
                string 警訊藥品_buf = "";
                string 麻醉藥品_buf = "";
                string 形狀相似_buf = "";
                string 發音相似_buf = "";
                string 管制級別_buf = "";
                string IP = value.IP;
                Storage storage = value;
                bool Is_Replace = false;
                藥品碼 = storage.GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
                if (藥品碼.StringIsEmpty()) return;
                list_藥品資料_藥檔資料_buf = list_藥品資料_藥檔資料.GetRows((int)enum_藥品資料_藥檔資料.藥品碼, 藥品碼);
                list_藥品設定表_buf = list_藥品設定表.GetRows((int)enum_medConfig.藥碼, 藥品碼);
                if (list_藥品資料_藥檔資料_buf.Count == 0)
                {
                    storage.Clear();
                    Is_Replace = true;
                }
                else
                {
                    藥品碼_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.藥品碼].ObjectToString();
                    料號_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.料號].ObjectToString();

                    藥品名稱_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.藥品名稱].ObjectToString();
                    中文名稱_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.中文名稱].ObjectToString();
                    藥品學名_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.藥品學名].ObjectToString();
                    BarCode_buf = list_藥品資料_藥檔資料_buf[0][(int)enum_藥品資料_藥檔資料.藥品條碼].ObjectToString();
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

                    藥品碼 = storage.GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
                    料號 = storage.SKDIACODE;
                    藥品名稱 = storage.GetValue(Device.ValueName.藥品名稱, Device.ValueType.Value).ObjectToString();
                    中文名稱 = storage.GetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Value).ObjectToString();
                    藥品學名 = storage.GetValue(Device.ValueName.藥品學名, Device.ValueType.Value).ObjectToString();
                    BarCode = storage.GetValue(Device.ValueName.BarCode, Device.ValueType.Value).ObjectToString();
                    包裝單位 = storage.GetValue(Device.ValueName.包裝單位, Device.ValueType.Value).ObjectToString();
                    管制級別 = storage.DRUGKIND;
                    警訊藥品 = storage.IsWarning ? "TRUE" : "FALSE";
                    麻醉藥品 = storage.IsAnesthetic ? "TRUE" : "FALSE";
                    形狀相似 = storage.IsShapeSimilar ? "TRUE" : "FALSE";
                    發音相似 = storage.IsSoundSimilar ? "TRUE" : "FALSE";

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

                    storage.SetValue(Device.ValueName.藥品碼, Device.ValueType.Value, 藥品碼_buf);
                    storage.SetValue(Device.ValueName.藥品名稱, Device.ValueType.Value, 藥品名稱_buf);
                    storage.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Value, 中文名稱_buf);
                    storage.SetValue(Device.ValueName.藥品學名, Device.ValueType.Value, 藥品學名_buf);
                    storage.SetValue(Device.ValueName.BarCode, Device.ValueType.Value, BarCode_buf);
                    storage.SetValue(Device.ValueName.包裝單位, Device.ValueType.Value, 包裝單位_buf);
                    storage.SKDIACODE = 料號_buf;
                    storage.DRUGKIND = 管制級別_buf;
                    storage.IsWarning = (警訊藥品_buf == "TRUE");
                    storage.IsAnesthetic = (麻醉藥品_buf == "TRUE");
                    storage.IsShapeSimilar = (形狀相似_buf == "TRUE");
                    storage.IsSoundSimilar = (發音相似_buf == "TRUE");

                    if (myConfigClass.QRCode_url.StringIsEmpty() == false)
                    {
                        storage.QRCode = $"{myConfigClass.QRCode_url}";
                        storage.QRCode = storage.QRCode.Replace("{code}", storage.Code);
                        storage.QRCode = storage.QRCode.Replace("{serverName}", ServerName);
                    }

                }
                if (Is_Replace)
                {
                    list_replaceValue.LockAdd(value);
                }
            });



            this.storageUI_EPD_266.SQL_ReplaceStorage(list_replaceValue);
            for (int i = 0; i < list_replaceValue.Count; i++)
            {
                List_EPD266_本地資料.Add_NewStorage(list_replaceValue[i]);
            }

            Console.Write($"儲位管理EPD266:更新藥檔完成 共<{list_replaceValue.Count}>筆,耗時 :{MyTimer_TickTime.GetTickTime().ToString("0.000")}\n");
            cnt++;
        }
        void cnt_Program_儲位管理_EPD266_資料更新_更新面板資料(ref int cnt)
        {
            MyTimer_TickTime.TickStop();
            MyTimer_TickTime.StartTickTime(50000);
            bool flag_顯示空白儲位 = plC_CheckBox_儲位管理_EPD266_儲位內容_顯示空白儲位.Checked;
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < List_EPD266_本地資料.Count; i++)
            {
                object[] value = new object[new enum_儲位管理_EPD266_儲位資料().GetLength()];
                string 藥品碼 = List_EPD266_本地資料[i].GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
                if (!flag_顯示空白儲位)
                {
                    if (藥品碼.StringIsEmpty()) continue;
                }
                value[(int)enum_儲位管理_EPD266_儲位資料.IP] = List_EPD266_本地資料[i].GetValue(Device.ValueName.IP, Device.ValueType.Value).ObjectToString();
                value[(int)enum_儲位管理_EPD266_儲位資料.儲位名稱] = List_EPD266_本地資料[i].GetValue(Device.ValueName.儲位名稱, Device.ValueType.Value).ObjectToString();
                value[(int)enum_儲位管理_EPD266_儲位資料.藥品碼] = List_EPD266_本地資料[i].GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
                value[(int)enum_儲位管理_EPD266_儲位資料.藥品名稱] = List_EPD266_本地資料[i].GetValue(Device.ValueName.藥品名稱, Device.ValueType.Value).ObjectToString();
                value[(int)enum_儲位管理_EPD266_儲位資料.包裝單位] = List_EPD266_本地資料[i].GetValue(Device.ValueName.包裝單位, Device.ValueType.Value).ObjectToString();
                value[(int)enum_儲位管理_EPD266_儲位資料.庫存] = List_EPD266_本地資料[i].GetValue(Device.ValueName.庫存, Device.ValueType.Value).ObjectToString();
                value[(int)enum_儲位管理_EPD266_儲位資料.區域] = List_EPD266_本地資料[i].Area;

                //value[(int)enum_儲位管理_EPD266_儲位資料.鎖控] = (List_EPD266_本地資料[i].DeviceType == DeviceType.EPD266_lock || List_EPD266_本地資料[i].DeviceType == DeviceType.EPD290_lock || List_EPD266_本地資料[i].DeviceType == DeviceType.EPD420_lock) ? true.ToString() : false.ToString();
                list_value.Add(value);
            }
            list_value.Sort(new ICP_儲位管理_EPD266_抽屜列表());
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.RefreshGrid(list_value);
            Console.Write($"儲位管理EPD266:更新儲位資料完成 ,耗時 :{MyTimer_TickTime.GetTickTime().ToString("0.000")}\n");
            cnt++;
        }

        #endregion
        #region Event
        private void SqL_DataGridView_儲位管理_EPD266_儲位資料_ComboBoxSelectedIndexChangedEvent(object sender, string colName, object[] RowValue)
        {
            string IP = RowValue[(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
            string 區域 = RowValue[(int)enum_儲位管理_EPD266_儲位資料.區域].ObjectToString();

            Storage storage = Main_Form._storageUI_EPD_266.SQL_GetStorage(IP);
            if (storage == null)
            {
                MyMessageBox.ShowDialog("查無儲位資料");
                return;
            }
            storage.Area = 區域;
            Main_Form._storageUI_EPD_266.SQL_ReplaceStorage(storage);
            PLC_Device_儲位管理_EPD266_資料更新.Bool = true;
        }
        private void SqL_DataGridView_儲位管理_EPD266_儲位資料_RowEnterEvent(object[] RowValue)
        {
            string IP = RowValue[(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
            string 儲位名稱 = RowValue[(int)enum_儲位管理_EPD266_儲位資料.儲位名稱].ObjectToString();
            string 藥品碼 = RowValue[(int)enum_儲位管理_EPD266_儲位資料.藥品碼].ObjectToString();
            string 藥品名稱 = RowValue[(int)enum_儲位管理_EPD266_儲位資料.藥品名稱].ObjectToString();
            string 包裝單位 = RowValue[(int)enum_儲位管理_EPD266_儲位資料.包裝單位].ObjectToString();
            string 庫存 = RowValue[(int)enum_儲位管理_EPD266_儲位資料.庫存].ObjectToString();



            Storage storage = this.storageUI_EPD_266.SQL_GetStorage(IP);
          
            //storage.IsWarning = (警訊藥品 == "True");
            if (storage != null)
            {
                if (Storage.ContainsBitmap(storage.Code) == false)
                {
                    List<Image> images = Function_取得藥品圖片(storage.Code);
                    if (images != null)
                    {
                        if (images.Count > 0)
                        {
                            Storage.SetBitmapToCache(storage.Code, (Bitmap)images[0]);
                        }
                    }
                }
                this.plC_CheckBox_儲位管理_EPD266_儲位內容_手勢感測.CheckStateChanged -= PlC_CheckBox_儲位管理_EPD266_儲位內容_手勢感測_CheckStateChanged;
                this.plC_CheckBox_儲位管理_EPD266_儲位內容_庫存儲位.CheckStateChanged -= PlC_CheckBox_儲位管理_EPD266_儲位內容_庫存儲位_CheckStateChanged;
                this.plC_CheckBox_儲位管理_EPD266_警報.CheckStateChanged -= PlC_RJ_Button_儲位管理_EPD266_警報_CheckStateChanged;

                rJ_TextBox_儲位管理_EPD266_儲位內容_語音.Texts = storage.Speaker;
                rJ_TextBox_儲位管理_EPD266_儲位名稱.Texts = storage.StorageName;
                rJ_TextBox_儲位管理_EPD266_包裝數量.Texts = storage.Min_Package_Num;

                this.Invoke(new Action(delegate { plC_CheckBox_儲位管理_EPD266_儲位內容_手勢感測.Checked = storage.TOFON; }));
                plC_CheckBox_儲位管理_EPD266_警報.Checked = storage.AlarmEnable;
                plC_CheckBox_儲位管理_EPD266_儲位內容_庫存儲位.Checked = storage.IsInventoryLocation;

                this.plC_CheckBox_儲位管理_EPD266_儲位內容_手勢感測.CheckStateChanged += PlC_CheckBox_儲位管理_EPD266_儲位內容_手勢感測_CheckStateChanged;
                this.plC_CheckBox_儲位管理_EPD266_警報.CheckStateChanged += PlC_RJ_Button_儲位管理_EPD266_警報_CheckStateChanged;
                this.plC_CheckBox_儲位管理_EPD266_儲位內容_庫存儲位.CheckStateChanged += PlC_CheckBox_儲位管理_EPD266_儲位內容_庫存儲位_CheckStateChanged;

                this.storagePanel.DrawToPictureBox(storage);
            }

            sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.ClearGrid();
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < storage.List_Validity_period.Count; i++)
            {
                object[] value = new object[new enum_儲位管理_EPD266_效期及庫存().GetLength()];
                value[(int)enum_儲位管理_EPD266_效期及庫存.效期] = storage.List_Validity_period[i];
                value[(int)enum_儲位管理_EPD266_效期及庫存.批號] = storage.List_Lot_number[i];
                value[(int)enum_儲位管理_EPD266_效期及庫存.庫存] = storage.List_Inventory[i];
                list_value.Add(value);
            }

            sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.RefreshGrid(list_value);
        }
        private void RJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品名稱_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_儲位管理_EPD266_藥品搜尋_藥品名稱_搜尋_MouseDownEvent(null);
            }
        }
        private void RJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_儲位管理_EPD266_藥品搜尋_藥品碼_搜尋_MouseDownEvent(null);
            }
        }
        private void RJ_TextBox_儲位管理_EPD266_儲位內容_儲位搜尋_藥品碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.storagePanel.CurrentStorage == null) return;
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_儲位管理_EPD266_儲位內容_儲位搜尋_藥品碼搜尋_MouseDownEvent(null);
            }
        }
        private void RJ_TextBox_儲位管理_EPD266_儲位名稱_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                storage.StorageName = this.rJ_TextBox_儲位管理_EPD266_儲位名稱.Texts;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.storagePanel.DrawToPictureBox(storage);
                this.Function_設定雲端資料更新();
                PLC_Device_儲位管理_EPD266_資料更新.Bool = true;
                MyMessageBox.ShowDialog("[儲位名稱]更新成功");

            }
        }
        private void RJ_TextBox_儲位管理_EPD266_包裝數量_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                storage.Min_Package_Num = this.rJ_TextBox_儲位管理_EPD266_包裝數量.Texts;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.storagePanel.DrawToPictureBox(storage);
                this.Function_設定雲端資料更新();
                PLC_Device_儲位管理_EPD266_資料更新.Bool = true;
                MyMessageBox.ShowDialog("[包裝數量]更新成功");
            }
        }
        private void RJ_TextBox_儲位管理_EPD266_儲位內容_語音_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.storagePanel.CurrentStorage == null) return;
            if (e.KeyChar == (char)Keys.Enter)
            {
                Storage storage = this.storagePanel.CurrentStorage;
                storage.Speaker = this.rJ_TextBox_儲位管理_EPD266_儲位內容_語音.Text;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                List_EPD266_本地資料.Add_NewStorage(storage);
                this.Function_設定雲端資料更新();
                MyMessageBox.ShowDialog("[語音內容]更新成功");

            }
        }
        private void PlC_CheckBox_儲位管理_EPD266_儲位內容_庫存儲位_CheckStateChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                storage.IsInventoryLocation = plC_CheckBox_儲位管理_EPD266_儲位內容_庫存儲位.Checked;
             
                List_EPD266_本地資料.Add_NewStorage(storage);
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.Function_設定雲端資料更新();
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_藥品搜尋_藥品名稱_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品名稱.Text.Length < 3)
            {
                MyMessageBox.ShowDialog("藥品名稱搜尋字元不得小於3個!");
                return;
            }
            List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            if (rJ_RatioButton_儲位管理_EPD266_藥品搜尋_前綴.Checked)
            {
                list_value = list_value.GetRowsStartWithByLike((int)enum_藥品資料_藥檔資料.藥品名稱, rJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品名稱.Text);
            }
            else if (rJ_RatioButton_儲位管理_EPD266_藥品搜尋_模糊.Checked)
            {
                list_value = list_value.GetRowsByLike((int)enum_藥品資料_藥檔資料.藥品名稱, rJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品名稱.Text);
            }

            this.sqL_DataGridView_儲位管理_EPD266_藥品資料_藥檔資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_儲位管理_EPD266_藥品搜尋_藥品碼_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品碼.Text.StringIsEmpty()) return;
            List<object[]> list_value = this.sqL_DataGridView_藥品資料_藥檔資料.SQL_GetAllRows(false);
            list_value = list_value.GetRowsByLike((int)enum_藥品資料_藥檔資料.藥品碼, rJ_TextBox_儲位管理_EPD266_藥品搜尋_藥品碼.Text);
            this.sqL_DataGridView_儲位管理_EPD266_藥品資料_藥檔資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_儲位管理_EPD266_藥品搜尋_填入資料_MouseDownEvent(MouseEventArgs mevent)
        {
            object[] value = this.sqL_DataGridView_儲位管理_EPD266_藥品資料_藥檔資料.GetRowValues();
            if (value == null) return;
            Storage storage = this.storagePanel.CurrentStorage;
            if (storage == null) return;
            storage.Clear();
            storage.SetValue(Device.ValueName.藥品碼, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.藥品碼]);
            storage.SetValue(Device.ValueName.藥品名稱, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.藥品名稱]);
            storage.SetValue(Device.ValueName.藥品學名, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.藥品學名]);
            storage.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.中文名稱]);
            storage.SetValue(Device.ValueName.包裝單位, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.包裝單位]);
            storage.SetValue(Device.ValueName.BarCode, Device.ValueType.Value, value[(int)enum_藥品資料_藥檔資料.藥品條碼]);
            if (value[(int)enum_藥品資料_藥檔資料.警訊藥品].ObjectToString().ToUpper() == true.ToString().ToUpper())
            {
                storage.BackColor = Color.Red;
                storage.ForeColor = Color.White;
            }
            else
            {
                storage.BackColor = Color.White;
                storage.ForeColor = Color.Black;
            }
            value = new object[new enum_儲位管理_EPD266_儲位資料().GetLength()];
            value[(int)enum_儲位管理_EPD266_儲位資料.IP] = storage.GetValue(Device.ValueName.IP, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_EPD266_儲位資料.儲位名稱] = storage.GetValue(Device.ValueName.儲位名稱, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_EPD266_儲位資料.藥品碼] = storage.GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_EPD266_儲位資料.藥品名稱] = storage.GetValue(Device.ValueName.藥品名稱, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_EPD266_儲位資料.包裝單位] = storage.GetValue(Device.ValueName.包裝單位, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_EPD266_儲位資料.庫存] = storage.GetValue(Device.ValueName.庫存, Device.ValueType.Value).ObjectToString();

            storage.Speaker = this.rJ_TextBox_儲位管理_EPD266_儲位內容_語音.Text;
            storage.StorageName = this.rJ_TextBox_儲位管理_EPD266_儲位名稱.Texts;
            storage.Min_Package_Num = this.rJ_TextBox_儲位管理_EPD266_包裝數量.Texts;

            medClass _medClass = medClass.get_med_clouds_by_code(Main_Form.API_Server, storage.Code);
            if (_medClass != null)
            {
                if (_medClass.storageInfo != null)
                {
                    List<string> storage_infos = new List<string>();
                    foreach (var storageInfo in _medClass.storageInfo)
                    {
                        storage_infos.Add(storageInfo.儲位描述);

                    }
                    string info_text = string.Join(",", storage_infos);
                    if (info_text.StringIsEmpty() == false)
                    {
                        storage.StorageName = info_text;
                        value[(int)enum_儲位管理_EPD266_儲位資料.儲位名稱] = info_text;
                    }
                }

            }

            List_EPD266_本地資料.Add_NewStorage(storage);
            this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
            this.storagePanel.DrawToPictureBox(storage);
            this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Replace(enum_儲位管理_EPD266_儲位資料.IP.GetEnumName(), value[(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString(), value, true);
            this.Function_設定雲端資料更新();

        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_總庫存字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                this.fontDialog.Font = storage.GetValue(Device.ValueName.庫存, Device.ValueType.Font) as Font;
                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {

                    storage.SetValue(Device.ValueName.庫存, Device.ValueType.Font, fontDialog.Font);
                    this.storagePanel.DrawToPictureBox(this.storagePanel.CurrentStorage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_儲位名稱字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                this.fontDialog.Font = storage.GetValue(Device.ValueName.儲位名稱, Device.ValueType.Font) as Font;
                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    storage.SetValue(Device.ValueName.儲位名稱, Device.ValueType.Font, fontDialog.Font);
                    this.storagePanel.DrawToPictureBox(this.storagePanel.CurrentStorage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_包裝單位字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                this.fontDialog.Font = storage.GetValue(Device.ValueName.包裝單位, Device.ValueType.Font) as Font;
                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    storage.SetValue(Device.ValueName.包裝單位, Device.ValueType.Font, fontDialog.Font);
                    this.storagePanel.DrawToPictureBox(this.storagePanel.CurrentStorage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_藥品碼字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                this.fontDialog.Font = storage.GetValue(Device.ValueName.藥品碼, Device.ValueType.Font) as Font;
                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    storage.SetValue(Device.ValueName.藥品碼, Device.ValueType.Font, fontDialog.Font);
                    this.storagePanel.DrawToPictureBox(this.storagePanel.CurrentStorage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_藥品條碼字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                this.fontDialog.Font = storage.GetValue(Device.ValueName.BarCode, Device.ValueType.Font) as Font;
                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    storage.SetValue(Device.ValueName.BarCode, Device.ValueType.Font, fontDialog.Font);
                    this.storagePanel.DrawToPictureBox(this.storagePanel.CurrentStorage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_中文名稱字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                this.fontDialog.Font = storage.GetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Font) as Font;
                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    storage.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Font, fontDialog.Font);
                    this.storagePanel.DrawToPictureBox(this.storagePanel.CurrentStorage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_藥品學名字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                this.fontDialog.Font = storage.GetValue(Device.ValueName.藥品學名, Device.ValueType.Font) as Font;
                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    storage.SetValue(Device.ValueName.藥品學名, Device.ValueType.Font, fontDialog.Font);
                    this.storagePanel.DrawToPictureBox(this.storagePanel.CurrentStorage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_藥品名稱字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                this.fontDialog.Font = storage.GetValue(Device.ValueName.藥品名稱, Device.ValueType.Font) as Font;
                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    storage.SetValue(Device.ValueName.藥品名稱, Device.ValueType.Font, fontDialog.Font);
                    this.storagePanel.DrawToPictureBox(this.storagePanel.CurrentStorage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_效期字體更動_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                this.fontDialog.Font = storage.GetValue(Device.ValueName.效期, Device.ValueType.Font) as Font;
                if (this.fontDialog.ShowDialog() == DialogResult.OK)
                {
                    storage.SetValue(Device.ValueName.效期, Device.ValueType.Font, fontDialog.Font);
                    this.storagePanel.DrawToPictureBox(this.storagePanel.CurrentStorage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                }
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_效期管理_修正庫存_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null)
                {
                    MyMessageBox.ShowDialog("未選擇儲位!");
                    return;
                }
                object[] value = sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.GetRowValues();
                if (value == null)
                {
                    MyMessageBox.ShowDialog("未選擇效期!");
                    return;
                }
                string 效期 = value[(int)enum_儲位管理_EPD266_效期及庫存.效期].ObjectToString();
                string 批號 = value[(int)enum_儲位管理_EPD266_效期及庫存.批號].ObjectToString();
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


                double 原有庫存 = storage.取得庫存();
                string 藥品碼 = storage.Code;
                藥品碼 = Function_藥品碼檢查(藥品碼);
                string 庫存量 = "";
                if (storage.IsInventoryLocation == false) 庫存量 = Function_從SQL取得庫存(藥品碼).ToString();
                else 庫存量 = Function_從SQL取得庫儲區庫存(藥品碼).ToString();
                storage.效期庫存覆蓋(效期, 數量);

                double 修正庫存 = storage.取得庫存();
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                if (storage.IsInventoryLocation == false) List_EPD266_本地資料.Add_NewStorage(storage);


                string GUID = Guid.NewGuid().ToString();
                string 動作 = enum_交易記錄查詢動作.效期庫存異動.GetEnumName();
                string 藥品名稱 = storage.Name;
                string 藥袋序號 = "";
                string 交易量 = (修正庫存 - 原有庫存).ToString();

                string 結存量 = "";
                if (storage.IsInventoryLocation == false) 結存量 = Function_從SQL取得庫存(藥品碼).ToString();
                else 結存量 = Function_從SQL取得庫儲區庫存(藥品碼).ToString();

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
                value_trading[(int)enum_交易記錄查詢資料.收支原因] = $"庫存異動{(storage.IsInventoryLocation ? "(庫儲)" : "")}";
                value_trading[(int)enum_交易記錄查詢資料.藥師證字號] = this.登入者藥師證字號;
                this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value_trading, false);

                List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.GetRows((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, false);
                if (list_value.Count == 0) return;
                list_value[0][(int)enum_儲位管理_EPD266_儲位資料.庫存] = storage.取得庫存();
                this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Replace((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, list_value[0], true);

                sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.ClearGrid();
                list_value = new List<object[]>();
                for (int i = 0; i < storage.List_Validity_period.Count; i++)
                {
                    value = new object[new enum_儲位管理_EPD266_效期及庫存().GetLength()];
                    value[(int)enum_儲位管理_EPD266_效期及庫存.效期] = storage.List_Validity_period[i];
                    value[(int)enum_儲位管理_EPD266_效期及庫存.批號] = storage.List_Lot_number[i];
                    value[(int)enum_儲位管理_EPD266_效期及庫存.庫存] = storage.List_Inventory[i];
                    list_value.Add(value);
                }

                sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.RefreshGrid(list_value);
                this.Function_設定雲端資料更新();
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_效期管理_新增效期_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null)
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

                double 原有庫存 = storage.取得庫存();
                string 藥品碼 = storage.Code;
                藥品碼 = Function_藥品碼檢查(藥品碼);
                string 庫存量 = "";
                if (storage.IsInventoryLocation == false) 庫存量 = Function_從SQL取得庫存(藥品碼).ToString();
                else 庫存量 = Function_從SQL取得庫儲區庫存(藥品碼).ToString();
                storage.效期庫存覆蓋(效期, 批號, 數量);
                double 修正庫存 = storage.取得庫存();
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);

                string GUID = Guid.NewGuid().ToString();
                string 動作 = enum_交易記錄查詢動作.效期庫存異動.GetEnumName();
                string 藥品名稱 = storage.Name;
                string 藥袋序號 = "";
                string 交易量 = (修正庫存 - 原有庫存).ToString();
                string 結存量 = "";
                if (storage.IsInventoryLocation == false) 結存量 = Function_從SQL取得庫存(藥品碼).ToString();
                else 結存量 = Function_從SQL取得庫儲區庫存(藥品碼).ToString();
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
                value_trading[(int)enum_交易記錄查詢資料.收支原因] = $"庫存異動{(storage.IsInventoryLocation ? "(庫儲)" : "")}{(storage.IsFADC ? $"({storage.IP})" : "")}";
                value_trading[(int)enum_交易記錄查詢資料.藥師證字號] = this.登入者藥師證字號;
                this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value_trading, false);

                List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.GetRows((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, false);
                if (list_value.Count == 0) return;
                list_value[0][(int)enum_儲位管理_EPD266_儲位資料.庫存] = storage.取得庫存();
                this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Replace((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, list_value[0], true);

                sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.ClearGrid();
                list_value = new List<object[]>();
                for (int i = 0; i < storage.List_Validity_period.Count; i++)
                {
                    object[] value = new object[new enum_儲位管理_EPD266_效期及庫存().GetLength()];
                    value[(int)enum_儲位管理_EPD266_效期及庫存.效期] = storage.List_Validity_period[i];
                    value[(int)enum_儲位管理_EPD266_效期及庫存.批號] = storage.List_Lot_number[i];
                    value[(int)enum_儲位管理_EPD266_效期及庫存.庫存] = storage.List_Inventory[i];
                    list_value.Add(value);
                }
                sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.RefreshGrid(list_value);
                this.Function_設定雲端資料更新();
            }));

        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_效期管理_修正批號_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null)
                {
                    MyMessageBox.ShowDialog("未選擇儲位!");
                    return;
                }
                object[] value = sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.GetRowValues();
                if (value == null)
                {
                    MyMessageBox.ShowDialog("未選擇效期!");
                    return;
                }
                string 效期 = value[(int)enum_儲位管理_EPD266_效期及庫存.效期].ObjectToString();
                string 舊批號 = value[(int)enum_儲位管理_EPD266_效期及庫存.批號].ObjectToString();
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


                storage.修正批號(效期, 新批號);
                List_EPD266_本地資料.Add_NewStorage(storage);
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);


                string GUID = Guid.NewGuid().ToString();
                string 動作 = enum_交易記錄查詢動作.效期庫存異動.GetEnumName();
                string 藥品名稱 = storage.Name;
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
                value_trading[(int)enum_交易記錄查詢資料.收支原因] = $"修正批號{(storage.IsInventoryLocation ? "(庫儲)" : "")}";
                value_trading[(int)enum_交易記錄查詢資料.藥師證字號] = this.登入者藥師證字號;
                this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value_trading, false);

                List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.GetRows((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, false);
                if (list_value.Count == 0) return;
                list_value[0][(int)enum_儲位管理_EPD266_儲位資料.庫存] = storage.取得庫存();
                this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Replace((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, list_value[0], true);

                sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.ClearGrid();
                list_value = new List<object[]>();
                for (int i = 0; i < storage.List_Validity_period.Count; i++)
                {
                    value = new object[new enum_儲位管理_EPD266_效期及庫存().GetLength()];
                    value[(int)enum_儲位管理_EPD266_效期及庫存.效期] = storage.List_Validity_period[i];
                    value[(int)enum_儲位管理_EPD266_效期及庫存.批號] = storage.List_Lot_number[i];
                    value[(int)enum_儲位管理_EPD266_效期及庫存.庫存] = storage.List_Inventory[i];
                    list_value.Add(value);
                }
                sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.RefreshGrid(list_value);
                this.Function_設定雲端資料更新();
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_面板亮燈_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                List<object[]> list_value = sqL_DataGridView_儲位管理_EPD266_儲位資料.Get_All_Select_RowsValues();
                if (list_value.Count == 0) return;
                Color color = Color.Black;

                if (this.radioButton_儲位管理_EPD266_面板亮燈_白.Checked)
                {
                    color = Color.White;
                }
                else if (this.radioButton_儲位管理_EPD266_面板亮燈_紅.Checked)
                {
                    color = Color.Red;
                }
                else if (this.radioButton_儲位管理_EPD266_面板亮燈_藍.Checked)
                {
                    color = Color.Blue;
                }
                else if (this.radioButton_儲位管理_EPD266_面板亮燈_綠.Checked)
                {
                    color = Color.Green;
                }
                List<Task> taskList = new List<Task>();
                string Error_msg = "";
                for (int i = 0; i < list_value.Count; i++)
                {
                    string IP = list_value[i][(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
                    Storage storage = this.storageUI_EPD_266.SQL_GetStorage(IP);

                    taskList.Add(Task.Run(() =>
                    {
                        if (storage != null)
                        {
                            if (!this.storageUI_EPD_266.Set_Stroage_LED_UDP(storage, color))
                            {
                                Console.WriteLine($"{storage.IP}:{storage.Port} : EPD266 面板亮燈失敗!");
                            }
                        }
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                allTask.Wait();

            }
            catch
            {

            }


        }
        private void PlC_RJ_Button_儲位管理_EPD266_清除燈號_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {

                List<object[]> list_value = sqL_DataGridView_儲位管理_EPD266_儲位資料.Get_All_Select_RowsValues();
                if (list_value.Count == 0) return;

                List<Task> taskList = new List<Task>();
                for (int i = 0; i < list_value.Count; i++)
                {
                    string IP = list_value[i][(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
                    Storage storage = this.storageUI_EPD_266.SQL_GetStorage(IP);

                    taskList.Add(Task.Run(() =>
                    {
                        if (storage != null)
                        {

                            if (!this.storageUI_EPD_266.Set_Stroage_LED_UDP(storage, Color.Black))
                            {
                                Console.WriteLine($"{storage.IP}:{storage.Port} : EPD266 面板滅燈失敗!");

                            }
                        }
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                allTask.Wait();
            }
            catch
            {

            }

        }
        private void PlC_RJ_Button_儲位管理_EPD266_上傳至面板_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                List<object[]> list_value = sqL_DataGridView_儲位管理_EPD266_儲位資料.Get_All_Select_RowsValues();
                if (list_value.Count == 0) return;

                List<Task> taskList = new List<Task>();
                for (int i = 0; i < list_value.Count; i++)
                {
                    string IP = list_value[i][(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
                    Storage storage = this.storageUI_EPD_266.SQL_GetStorage(IP);
                    if (myConfigClass.QRCode_url.StringIsEmpty() == false)
                    {
                        storage.QRCode = $"{myConfigClass.QRCode_url}";
                        storage.QRCode = storage.QRCode.Replace("{code}", storage.Code);
                        storage.QRCode = storage.QRCode.Replace("{serverName}", ServerName);
                    }
                    if (Storage.ContainsBitmap(storage.Code) == false)
                    {
                        List<Image> images = Function_取得藥品圖片(storage.Code);
                        if (images != null)
                        {
                            if (images.Count > 0)
                            {
                                Storage.SetBitmapToCache(storage.Code, (Bitmap)images[0]);
                            }
                        }
                    }

                    taskList.Add(Task.Run(() =>
                    {
                        if (storage != null)
                        {
                            if (!this.storageUI_EPD_266.DrawToEpd_UDP(storage))
                            {
                                Console.WriteLine($"{storage.IP}:{storage.Port} : EPD266 面板上傳失敗!");
                            }
                        }
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                allTask.Wait();

            }
            catch
            {

            }

        }
        private void PlC_RJ_Button_儲位管理_EPD266_刪除儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("使否刪除選取儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
            {
                List<object[]> list_value = sqL_DataGridView_儲位管理_EPD266_儲位資料.Get_All_Select_RowsValues();
                List<object[]> list_value_buf = new List<object[]>();
                if (list_value.Count == 0) return;
                for (int i = 0; i < list_value.Count; i++)
                {
                    string IP = list_value[i][(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
                    Storage storage = this.storageUI_EPD_266.SQL_GetStorage(IP);
                    storage.Clear();
                    List_EPD266_本地資料.Add_NewStorage(storage);
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);

                    list_value_buf = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.GetRows((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, false);
                    if (list_value_buf.Count == 0) continue;
                    list_value_buf[0] = new object[new enum_儲位管理_EPD266_儲位資料().GetLength()];
                    list_value_buf[0][(int)enum_儲位管理_EPD266_儲位資料.IP] = storage.IP;
                    //list_value_buf[0][(int)enum_儲位管理_EPD266_儲位資料.警訊藥品] = false.ToString();
                    //list_value_buf[0][(int)enum_儲位管理_EPD266_儲位資料.鎖控] = (storage.DeviceType == DeviceType.EPD266_lock || storage.DeviceType == DeviceType.EPD290_lock || storage.DeviceType == DeviceType.EPD420_lock) ? true.ToString() : false.ToString();

                    this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Replace((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, list_value_buf[0], true);

                }
                this.Function_設定雲端資料更新();
            }
        }
        private void PlC_RJ_Button_儲位管理_EPD266_開鎖_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Get_All_Select_RowsValues();
            string IP = "";
            string ID_Name = this.登入者名稱;

            foreach (object[] value in list_value)
            {
                IP = value[(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
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
                string 備註 = $"儲位名稱[{value[(int)enum_儲位管理_EPD266_儲位資料.儲位名稱].ObjectToString()}]";
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
        private void PlC_RJ_Button_儲位管理_EPD266_複製格式_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Get_All_Select_RowsValues();
            if (list_value.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("未選擇儲位!");
                }));
                return;
            }
            string IP = list_value[0][(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();

            Storage storage = this.storageUI_EPD_266.SQL_GetStorage(IP);
            if (storage == null)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog($"未搜尋到 {IP} 儲位!");
                }));
                return;
            }
            EPD266_Storage_Copy = storage;
            MyMessageBox.ShowDialog("已複製到剪貼簿!");

        }
        private void PlC_RJ_Button_儲位管理_EPD266_貼上格式_MouseDownEvent(MouseEventArgs mevent)
        {
            if (EPD266_Storage_Copy == null)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog($"尚未複製儲位!");
                }));
                return;
            }
            List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Get_All_Select_RowsValues();
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
                dialogResult = MyMessageBox.ShowDialog("是否覆蓋選取儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel);
            }));
            if (dialogResult != DialogResult.Yes) return;

            List<Storage> storages_replace = new List<Storage>();
            for (int i = 0; i < list_value.Count; i++)
            {
                string IP = list_value[i][(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
                Storage storage = List_EPD266_本地資料.SortByIP(IP);
                if (storage != null)
                {
                    storage.PasteFormat(EPD266_Storage_Copy);
                    List_EPD266_本地資料.Add_NewStorage(storage);
                    storages_replace.Add(storage);
                }
            }
            this.storageUI_EPD_266.SQL_ReplaceStorage(storages_replace);
            sqL_DataGridView_儲位管理_EPD266_儲位資料.On_RowEnter();
            this.Function_設定雲端資料更新();
            MyMessageBox.ShowDialog("貼上格式完成!");
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位內容_儲位搜尋_藥品碼搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            string text = "";
            string comboBox_text = "";
            this.Invoke(new Action(delegate
            {
                text = this.rJ_TextBox_儲位管理_EPD266_儲位內容_儲位搜尋_藥品碼.Text;
                comboBox_text = this.comboBox_儲位管理_EPD266_儲位內容_儲位搜尋.Text;
            }));

            if (text.StringIsEmpty()) return;
            List<Storage> storages = new List<Storage>();
            int select_index = -1;
            if (comboBox_text == "藥碼")
            {
                List<Storage> storages_buf = List_EPD266_本地資料;
                storages_buf = (from temp in storages_buf
                                where temp.Code.ToUpper().Contains(text.ToUpper())
                                select temp).ToList();
                storages = storages_buf;
            }
            if (comboBox_text == "藥名")
            {
                List<Storage> storages_buf = List_EPD266_本地資料;
                storages_buf = (from temp in storages_buf
                                where temp.Name.ToUpper().Contains(text.ToUpper())
                                select temp).ToList();
                storages = storages_buf;
            }
            if (comboBox_text == "商品名")
            {
                List<Storage> storages_buf = List_EPD266_本地資料;
                storages_buf = (from temp in storages_buf
                                where temp.Scientific_Name.ToUpper().Contains(text.ToUpper())
                                select temp).ToList();
                storages = storages_buf;
            }


            if (storages.Count == 0)
            {
                MyMessageBox.ShowDialog("查無無此藥品!!");
                return;
            }
            object[] value = sqL_DataGridView_儲位管理_EPD266_儲位資料.GetRowValues();
            if (value != null)
            {
                string IP = value[(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
                for (int i = 0; i < storages.Count; i++)
                {
                    if (storages[i].IP == IP)
                    {
                        select_index = i;
                    }
                }
            }

            Storage storage;
            if (storages.Count == 0) return;
            if (storages[0] == null) return;
            if (select_index == -1)
            {
                storage = storages[0];
            }
            else if ((select_index + 1) == storages.Count)
            {
                storage = storages[0];
            }
            else
            {
                storage = storages[select_index + 1];
            }

            this.storagePanel.DrawToPictureBox(storage);

            List<object[]> list_values = sqL_DataGridView_儲位管理_EPD266_儲位資料.GetAllRows();
            for (int i = 0; i < list_values.Count; i++)
            {
                if (list_values[i][(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString() == storage.IP)
                {
                    sqL_DataGridView_儲位管理_EPD266_儲位資料.SetSelectRow(i);
                    return;
                }
            }
        }
        private void PlC_RJ_Button_儲位管理_EPD266_儲位初始化_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (MyMessageBox.ShowDialog("確認將所有儲位效期庫存更動為測試版?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    List<Storage> storages = this.storageUI_EPD_266.SQL_GetAllStorage();
                    List<Device> devices = storages.GetAllDevice();
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
                    this.storageUI_EPD_266.SQL_ReplaceStorage(storages);
                    dialog_Prcessbar.Close();
                }
            }));
        }

        private void PlC_CheckBox_儲位管理_EPD266_儲位內容_顯示空白儲位_CheckStateChanged(object sender, EventArgs e)
        {
            PLC_Device_儲位管理_EPD266_資料更新.Bool = true;
        }
        private void PlC_CheckBox_儲位管理_EPD266_儲位內容_手勢感測_CheckStateChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                storage.TOFON = plC_CheckBox_儲位管理_EPD266_儲位內容_手勢感測.Checked;
                this.storageUI_EPD_266.Set_TOF(storage.IP, storage.Port, storage.TOFON);
                List_EPD266_本地資料.Add_NewStorage(storage);
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.Function_設定雲端資料更新();
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_警報_CheckStateChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate
            {
                Storage storage = this.storagePanel.CurrentStorage;
                if (storage == null) return;
                storage.AlarmEnable = plC_CheckBox_儲位管理_EPD266_警報.Checked;
                List_EPD266_本地資料.Add_NewStorage(storage);
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.Function_設定雲端資料更新();
                flag_Program_輸出入檢查_輸出刷新_Init = false;
            }));
        }
        private void PlC_RJ_Button_儲位管理_EPD266_自動填入儲位名稱_MouseDownEvent(MouseEventArgs mevent)

        {
            if (MyMessageBox.ShowDialog("確認自動填入儲位名稱?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            List<object[]> list_儲位列表 = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.GetAllRows();
            List<medClass> medClasses = medClass.get_med_cloud(API_Server);
            List<medClass> medClasses_buf = new List<medClass>();
            Dictionary<string, List<medClass>> keyValuePairs_medcloud = medClasses.CoverToDictionaryByCode();


            for (int i = 0; i < list_儲位列表.Count; i++)
            {
                string IP = list_儲位列表[i][(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
                Storage storage = List_EPD266_本地資料.SortByIP(IP);
                if (storage == null) continue;
                string 藥碼 = storage.Code;

                medClass medClass = keyValuePairs_medcloud.SortDictionaryByCode(藥碼).FirstOrDefault();
                if (medClass == null)
                {
                    storage.StorageName = $"{i + 1}";
                }
                else
                {
                    List<string> storage_info = new List<string>();
                    foreach (var storage_ in medClass.storageInfo)
                    {
                        storage_info.Add(storage_.儲位描述);
                    }
                    string 儲位描述 = string.Join(",", storage_info);
                    storage.StorageName = 儲位描述.StringIsEmpty() ? $"{i + 1}" : 儲位描述;
                }
                List_EPD266_本地資料.Add_NewStorage(storage);
            }
            this.storageUI_EPD_266.SQL_ReplaceStorage(List_EPD266_本地資料);
            this.Function_設定雲端資料更新();
            PLC_Device_儲位管理_EPD266_資料更新.Bool = true;
            while (true)
            {
                if (PLC_Device_儲位管理_EPD266_資料更新.Bool == false) break;
                System.Threading.Thread.Sleep(10);
            }

        }
        private void PlC_RJ_Button_儲位管理_EPD266_匯入_MouseDownEvent(MouseEventArgs mevent)
        {
            DialogResult dialogResult = DialogResult.None;
            if (MyMessageBox.ShowDialog("確認匯入所有儲位?將會全部覆蓋!", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;

            this.Invoke(new Action(delegate
            {
                dialogResult = this.openFileDialog_LoadExcel.ShowDialog();
            }));
            if (dialogResult != DialogResult.OK) return;
            DataTable dataTable = MyOffice.ExcelClass.NPOI_LoadFile(this.openFileDialog_LoadExcel.FileName);
            if (dataTable == null)
            {
                MyMessageBox.ShowDialog("匯入失敗");
                return;
            }

            for (int k = 0; k < dataTable.Rows.Count; k++)
            {
                string IP = dataTable.Rows[k]["IP"].ToString();
                string Code = dataTable.Rows[k]["藥碼"].ToString();
                Code = RemoveParenthesesContent(Code);
                Storage storage = List_EPD266_本地資料.SortByIP(IP);
                if (storage == null) continue;
                storage.Code = Code;
                List_EPD266_本地資料.Add_NewStorage(storage);
            }
            this.storageUI_EPD_266.SQL_ReplaceStorage(List_EPD266_本地資料);
            this.Function_設定雲端資料更新();
            PLC_Device_儲位管理_EPD266_資料更新.Bool = true;
            while (true)
            {
                if (PLC_Device_儲位管理_EPD266_資料更新.Bool == false) break;
                System.Threading.Thread.Sleep(10);
            }
            MyMessageBox.ShowDialog("匯入完成");
        }
        private void PlC_RJ_Button_儲位管理_EPD266_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_儲位管理_EPD266_匯出());
            if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
            {
                if (dialog_ContextMenuStrip.Value == ContextMenuStrip_儲位管理_EPD266_匯出.匯出建置表.GetEnumName())
                {
                    DialogResult dialogResult = DialogResult.None;
                    this.Invoke(new Action(delegate
                    {
                        dialogResult = this.saveFileDialog_SaveExcel.ShowDialog();
                    }));
                    if (dialogResult != DialogResult.OK) return;
                    List<object[]> list_value = new List<object[]>();
                    for (int i = 0; i < List_EPD266_本地資料.Count; i++)
                    {
                        Storage storage = List_EPD266_本地資料[i];
                        object[] value = new object[new enum_儲位管理_EPD266_匯出儲位表().GetLength()];
                        value[(int)enum_儲位管理_EPD266_匯出儲位表.IP] = storage.IP;
                        value[(int)enum_儲位管理_EPD266_匯出儲位表.藥碼] = storage.Code;
                        value[(int)enum_儲位管理_EPD266_匯出儲位表.藥名] = storage.Name;
                        value[(int)enum_儲位管理_EPD266_匯出儲位表.單位] = storage.Package;
                        list_value.Add(value);
                    }
                    DataTable dataTable = list_value.ToDataTable(new enum_儲位管理_EPD266_匯出儲位表());
                    dataTable.NPOI_SaveFile(this.saveFileDialog_SaveExcel.FileName);
                    MyMessageBox.ShowDialog("匯出完成!");
                }
                if (dialog_ContextMenuStrip.Value == ContextMenuStrip_儲位管理_EPD266_匯出.匯出儲位表.GetEnumName())
                {
                    List<object[]> list_儲位列表 = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.GetAllRows();
                    list_儲位列表 = (from temp in list_儲位列表
                                 where temp[(int)enum_儲位管理_EPD266_儲位資料.藥品碼].ObjectToString().StringIsEmpty() == false
                                 select temp).ToList();


                    DialogResult dialogResult = DialogResult.None;
                    this.Invoke(new Action(delegate
                    {
                        dialogResult = this.saveFileDialog_SaveExcel.ShowDialog();
                    }));
                    if (dialogResult != DialogResult.OK) return;
                    string[] colnames = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.GetAllColumn_Name();
                    DataTable dataTable = list_儲位列表.ToDataTable(colnames);
                    dataTable = dataTable.ReorderTable(new enum_儲位管理_EPD266_匯出儲位表());
                    dataTable.NPOI_SaveFile(this.saveFileDialog_SaveExcel.FileName);
                    MyMessageBox.ShowDialog("匯出完成!");

                }
            }


        }
        #endregion

        private class ICP_儲位管理_EPD266_抽屜列表 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string IP_0 = x[(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
                string IP_1 = y[(int)enum_儲位管理_EPD266_儲位資料.IP].ObjectToString();
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
