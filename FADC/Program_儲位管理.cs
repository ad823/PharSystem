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
        MyTimer MyTimer_TickTime = new MyTimer();
        static public List<Storage> List_EPD266_本地資料 = new List<Storage>();
        private bool flag_Program_儲位管理_EPD266_Init = false;

        [EnumDescription("")]
        private enum enum_儲位管理_效期及庫存
        {
            [Description("效期,VARCHAR,300,NONE")]
            效期,
            [Description("批號,VARCHAR,300,NONE")]
            批號,
            [Description("庫存,VARCHAR,300,NONE")]
            庫存,
        }
        [EnumDescription("")]
        private enum enum_儲位管理_儲位資料
        {
            [Description("IP,VARCHAR,300,NONE")]
            IP,
            [Description("儲位名稱,VARCHAR,300,NONE")]
            儲位名稱,
            [Description("藥碼,VARCHAR,300,NONE")]
            藥碼,
            [Description("藥名,VARCHAR,300,NONE")]
            藥名,
            [Description("包裝數量,VARCHAR,300,NONE")]
            包裝數量,
            [Description("包裝單位,VARCHAR,300,NONE")]
            包裝單位,
            [Description("庫存,VARCHAR,300,NONE")]
            庫存,
            [Description("區域,VARCHAR,15,NONE")]
            區域,

        }
        private void Program_儲位管理_Init()
        {
            _storageUI_EPD_266 = this.storageUI_EPD_266;
            this.storageUI_EPD_266.InitEx(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);

            _rfiD_UI = this.rfiD_UI;
            this.rfiD_UI.Init(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);

            sqL_DataGridView_儲位管理_藥品資料_藥檔資料.InitEx(medClass.init(API_Server));

            this.sqL_DataGridView_儲位管理_藥品資料_藥檔資料.Set_ColumnVisible(false, new enum_雲端藥檔().GetEnumNames());
            this.sqL_DataGridView_儲位管理_藥品資料_藥檔資料.Set_ColumnVisible(true, enum_雲端藥檔.藥品碼, enum_雲端藥檔.藥品名稱);
            this.sqL_DataGridView_儲位管理_藥品資料_藥檔資料.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品碼);
            this.sqL_DataGridView_儲位管理_藥品資料_藥檔資料.Set_ColumnWidth(350, DataGridViewContentAlignment.MiddleLeft, enum_雲端藥檔.藥品名稱);
            this.sqL_DataGridView_儲位管理_藥品資料_藥檔資料.Set_ColumnText("藥碼", enum_雲端藥檔.藥品碼);
            this.sqL_DataGridView_儲位管理_藥品資料_藥檔資料.Set_ColumnText("藥名", enum_雲端藥檔.藥品名稱);


            Table table = new SQLUI.Table(new enum_儲位管理_效期及庫存());
            this.sqL_DataGridView_儲位管理_儲位內容_效期及庫存.Init(table);
            this.sqL_DataGridView_儲位管理_儲位內容_效期及庫存.Set_ColumnVisible(false, new enum_儲位管理_效期及庫存().GetEnumNames());
            this.sqL_DataGridView_儲位管理_儲位內容_效期及庫存.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_效期及庫存.效期);
            this.sqL_DataGridView_儲位管理_儲位內容_效期及庫存.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_效期及庫存.批號);
            this.sqL_DataGridView_儲位管理_儲位內容_效期及庫存.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_效期及庫存.庫存);

            this.sqL_DataGridView_儲位管理_儲位資料.RowsHeight = 40;
            this.sqL_DataGridView_儲位管理_儲位資料.Init(new Table(new enum_儲位管理_儲位資料()));
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnVisible(false, new enum_儲位管理_儲位資料().GetEnumNames());
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_儲位資料.IP);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_儲位資料.儲位名稱);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_儲位資料.藥碼);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(750, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_儲位資料.藥名);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_儲位資料.包裝數量);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_儲位資料.包裝單位);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_儲位資料.庫存);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_儲位資料.區域);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnVisible(false, enum_儲位管理_儲位資料.區域);
            this.sqL_DataGridView_儲位管理_儲位資料.RowEnterEvent += SqL_DataGridView_儲位管理_儲位資料_RowEnterEvent;
            this.sqL_DataGridView_儲位管理_儲位資料.DataGridRowsChangeRefEvent += SqL_DataGridView_儲位管理_儲位資料_DataGridRowsChangeRefEvent;

            this.plC_RJ_Button_儲位管理_藥品搜尋_藥碼_搜尋.MouseDownEvent += PlC_RJ_Button_儲位管理_藥品搜尋_藥碼_搜尋_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_藥品搜尋_藥名_搜尋.MouseDownEvent += PlC_RJ_Button_儲位管理_藥品搜尋_藥名_搜尋_MouseDownEvent;

            this.plC_RJ_Button_儲位管理_藥品搜尋_填入資料.MouseDownEvent += PlC_RJ_Button_儲位管理_藥品搜尋_填入資料_MouseDownEvent;

            this.plC_RJ_Button_儲位管理_儲位內容_效期管理_新增效期.MouseDownEvent += PlC_RJ_Button_儲位管理_儲位內容_效期管理_新增效期_MouseDownEvent;

            this.storagePanel.SizeChanged += StoragePanel_SizeChanged;
            this.storagePanel.SureClick += StoragePanel_SureClick;
            this.plC_UI_Init.Add_Method(this.Program_儲位管理);
        }



        #region Event
        private void PlC_RJ_Button_儲位管理_儲位內容_效期管理_新增效期_MouseDownEvent(MouseEventArgs mevent)
        {
            //this.Invoke(new Action(delegate
            //{
            //    Storage storage = this.storagePanel.CurrentStorage;
            //    if (storage == null)
            //    {
            //        MyMessageBox.ShowDialog("未選擇儲位!");
            //        return;
            //    }
            //    string 效期 = "";
            //    string 批號 = "";
            //    string 數量 = "";
            //    Dialog_DateTime dialog_DateTime = new Dialog_DateTime();
            //    if (dialog_DateTime.ShowDialog() == DialogResult.Yes)
            //    {
            //        效期 = dialog_DateTime.Value.ToDateString();
            //    }
            //    else
            //    {
            //        return;
            //    }
            //    Dialog_輸入批號 dialog_輸入批號 = new Dialog_輸入批號();
            //    if (dialog_輸入批號.ShowDialog() == DialogResult.Yes)
            //    {
            //        批號 = dialog_輸入批號.Value;
            //    }
            //    else
            //    {
            //        return;
            //    }
            //    Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            //    if (dialog_NumPannel.ShowDialog() == DialogResult.Yes)
            //    {
            //        數量 = dialog_NumPannel.Value.ToString();
            //    }
            //    else
            //    {
            //        return;
            //    }

            //    double 原有庫存 = storage.取得庫存();
            //    string 藥品碼 = storage.Code;
            //    string 庫存量 = Function_從SQL取得庫存(藥品碼).ToString();
            //    storage.效期庫存覆蓋(效期, 批號, 數量);
            //    double 修正庫存 = storage.取得庫存();
            //    this.storageUI_EPD_266.SQL_ReplaceStorage(storage);

            //    string GUID = Guid.NewGuid().ToString();
            //    string 動作 = enum_交易記錄查詢動作.效期庫存異動.GetEnumName();
            //    string 藥品名稱 = storage.Name;
            //    string 藥袋序號 = "";
            //    string 交易量 = (修正庫存 - 原有庫存).ToString();
            //    string 結存量 = Function_從SQL取得庫存(藥品碼).ToString();
            //    string 操作人 = this.登入者名稱;
            //    string 病人姓名 = "";
            //    string 病歷號 = "";
            //    string 操作時間 = DateTime.Now.ToDateTimeString_6();
            //    string 開方時間 = DateTime.Now.ToDateTimeString_6();
            //    string 備註 = $"[效期]:{效期},[批號]:{批號}";

            //    object[] value_trading = new object[new enum_交易記錄查詢資料().GetLength()];
            //    value_trading[(int)enum_交易記錄查詢資料.GUID] = GUID;
            //    value_trading[(int)enum_交易記錄查詢資料.動作] = 動作;
            //    value_trading[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
            //    value_trading[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
            //    value_trading[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
            //    value_trading[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
            //    value_trading[(int)enum_交易記錄查詢資料.交易量] = 交易量;
            //    value_trading[(int)enum_交易記錄查詢資料.結存量] = 結存量;
            //    value_trading[(int)enum_交易記錄查詢資料.操作人] = 操作人;
            //    value_trading[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
            //    value_trading[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
            //    value_trading[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
            //    value_trading[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
            //    value_trading[(int)enum_交易記錄查詢資料.備註] = 備註;
            //    value_trading[(int)enum_交易記錄查詢資料.收支原因] = "庫存異動";
            //    value_trading[(int)enum_交易記錄查詢資料.藥師證字號] = this.登入者藥師證字號;
            //    this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value_trading, false);

            //    List<object[]> list_value = this.sqL_DataGridView_儲位管理_EPD266_儲位資料.GetRows((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, false);
            //    if (list_value.Count == 0) return;
            //    list_value[0][(int)enum_儲位管理_EPD266_儲位資料.庫存] = storage.取得庫存();
            //    this.sqL_DataGridView_儲位管理_EPD266_儲位資料.Replace((int)enum_儲位管理_EPD266_儲位資料.IP, storage.IP, list_value[0], true);

            //    sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.ClearGrid();
            //    list_value = new List<object[]>();
            //    for (int i = 0; i < storage.List_Validity_period.Count; i++)
            //    {
            //        object[] value = new object[new enum_儲位管理_EPD266_效期及庫存().GetLength()];
            //        value[(int)enum_儲位管理_EPD266_效期及庫存.效期] = storage.List_Validity_period[i];
            //        value[(int)enum_儲位管理_EPD266_效期及庫存.批號] = storage.List_Lot_number[i];
            //        value[(int)enum_儲位管理_EPD266_效期及庫存.庫存] = storage.List_Inventory[i];
            //        list_value.Add(value);
            //    }
            //    sqL_DataGridView_儲位管理_EPD266_儲位內容_效期及庫存.RefreshGrid(list_value);
            //    this.Function_設定雲端資料更新();
            //}));
        }
        private void SqL_DataGridView_儲位管理_儲位資料_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {
            Dictionary<string, List<Storage>> keyValuePairs_storages = List_EPD266_本地資料.CoverToDictionaryByIP();
            foreach (object[] obj in RowsList)
            {
                string IP = obj[(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                Storage storage = keyValuePairs_storages.SortDictionaryByIP(IP).FirstOrDefault();
                if (storage == null) continue;
                obj[(int)enum_儲位管理_儲位資料.IP] = storage.IP;
                obj[(int)enum_儲位管理_儲位資料.儲位名稱] = storage.StorageName;
                obj[(int)enum_儲位管理_儲位資料.藥碼] = storage.Code;
                obj[(int)enum_儲位管理_儲位資料.藥名] = storage.Name;
                obj[(int)enum_儲位管理_儲位資料.包裝數量] = storage.Min_Package_Num;
                obj[(int)enum_儲位管理_儲位資料.包裝單位] = storage.Package;
                obj[(int)enum_儲位管理_儲位資料.庫存] = storage.Inventory;
                obj[(int)enum_儲位管理_儲位資料.區域] = storage.Area;
            }
        }
        private void PlC_RJ_Button_儲位管理_藥品搜尋_填入資料_MouseDownEvent(MouseEventArgs mevent)
        {
            object[] value = this.sqL_DataGridView_儲位管理_藥品資料_藥檔資料.GetRowValues();
            if (value == null) return;
            Storage storage = this.storagePanel.CurrentStorage;
            if (storage == null) return;

            medClass medClass = value.SQLToClass<medClass , enum_雲端藥檔>();

            storage.Clear();
            storage.SetValue(Device.ValueName.藥品碼, Device.ValueType.Value, medClass.藥品碼);
            storage.SetValue(Device.ValueName.藥品名稱, Device.ValueType.Value, medClass.藥品名稱);
            storage.SetValue(Device.ValueName.藥品學名, Device.ValueType.Value, medClass.藥品學名);
            storage.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Value, medClass.中文名稱);
            storage.SetValue(Device.ValueName.包裝單位, Device.ValueType.Value, medClass.包裝單位);

            value = new object[new enum_儲位管理_儲位資料().GetLength()];
            value[(int)enum_儲位管理_儲位資料.IP] = storage.GetValue(Device.ValueName.IP, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_儲位資料.儲位名稱] = storage.GetValue(Device.ValueName.儲位名稱, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_儲位資料.藥碼] = storage.GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_儲位資料.藥名] = storage.GetValue(Device.ValueName.藥品名稱, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_儲位資料.包裝單位] = storage.GetValue(Device.ValueName.包裝單位, Device.ValueType.Value).ObjectToString();
            value[(int)enum_儲位管理_儲位資料.庫存] = storage.GetValue(Device.ValueName.庫存, Device.ValueType.Value).ObjectToString();

            storage.Speaker = this.rJ_TextBox_儲位管理_儲位內容_語音.Text;
            storage.StorageName = this.rJ_TextBox_儲位管理_儲位名稱.Texts;
            storage.Min_Package_Num = this.rJ_TextBox_儲位管理_包裝數量.Texts;

            if (storage.Min_Package_Num.StringIsDouble() == false) storage.Min_Package_Num = "1";

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
                        value[(int)enum_儲位管理_儲位資料.儲位名稱] = info_text;
                    }
                }

            }

            List_EPD266_本地資料.Add_NewStorage(storage);
            this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
            this.storagePanel.DrawToPictureBox(storage);
            this.sqL_DataGridView_儲位管理_儲位資料.Replace(enum_儲位管理_儲位資料.IP.GetEnumName(), value[(int)enum_儲位管理_儲位資料.IP].ObjectToString(), value, true);

        }
        private void PlC_RJ_Button_儲位管理_藥品搜尋_藥名_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                LoadingForm.ShowLoadingForm();
                if (rJ_TextBox_儲位管理_藥品搜尋_藥名.Text.StringIsEmpty() == true)
                {
                    MyMessageBox.ShowDialog("未輸入搜尋資訊");
                    return;
                }
                string name = rJ_TextBox_儲位管理_藥品搜尋_藥名.Text;

                List<medClass> medClasses = medClass.get_med_cloud(API_Server);
                if (rJ_RatioButton_儲位管理_藥品搜尋_前綴.Checked)
                {
                    medClasses = medClasses.Where(x => x.藥品名稱.ToLower().StartsWith(name.ToLower())).ToList();
                }
                else if (rJ_RatioButton_儲位管理_藥品搜尋_模糊.Checked)
                {
                    medClasses = medClasses.Where(x => x.藥品名稱.ToLower().Contains(name.ToLower())).ToList();
                }
                if (medClasses.Count == 0)
                {
                    MyMessageBox.ShowDialog("查無資料");
                    return;
                }
                this.sqL_DataGridView_儲位管理_藥品資料_藥檔資料.RefreshGrid(medClasses.ClassToSQL<medClass, enum_雲端藥檔>());
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
          
        }
        private void PlC_RJ_Button_儲位管理_藥品搜尋_藥碼_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                LoadingForm.ShowLoadingForm();
                if (rJ_TextBox_儲位管理_藥品搜尋_藥碼.Texts.StringIsEmpty() == true)
                {
                    MyMessageBox.ShowDialog("未輸入搜尋資訊");
                    return;
                }
                string code = rJ_TextBox_儲位管理_藥品搜尋_藥碼.Texts;

                List<medClass> medClasses = medClass.get_med_cloud(API_Server);
                if (rJ_RatioButton_儲位管理_藥品搜尋_前綴.Checked)
                {
                    medClasses = medClasses.Where(x => x.藥品碼.ToLower().StartsWith(code.ToLower())).ToList();
                }
                else if (rJ_RatioButton_儲位管理_藥品搜尋_模糊.Checked)
                {
                    medClasses = medClasses.Where(x => x.藥品碼.ToLower().Contains(code.ToLower())).ToList();
                }

                if (medClasses.Count == 0)
                {
                    MyMessageBox.ShowDialog("查無資料");
                    return;
                }
                this.sqL_DataGridView_儲位管理_藥品資料_藥檔資料.RefreshGrid(medClasses.ClassToSQL<medClass, enum_雲端藥檔>());
            }
            catch
            {

            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }
          
        }

        private void SqL_DataGridView_儲位管理_儲位資料_RowEnterEvent(object[] RowValue)
        {
            string IP = RowValue[(int)enum_儲位管理_儲位資料.IP].ObjectToString();
            string 儲位名稱 = RowValue[(int)enum_儲位管理_儲位資料.儲位名稱].ObjectToString();
            string 藥品碼 = RowValue[(int)enum_儲位管理_儲位資料.藥碼].ObjectToString();
            string 藥品名稱 = RowValue[(int)enum_儲位管理_儲位資料.藥名].ObjectToString();
            string 包裝單位 = RowValue[(int)enum_儲位管理_儲位資料.包裝單位].ObjectToString();
            string 庫存 = RowValue[(int)enum_儲位管理_儲位資料.庫存].ObjectToString();


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

                rJ_TextBox_儲位管理_儲位內容_語音.Texts = storage.Speaker;
                rJ_TextBox_儲位管理_儲位名稱.Texts = storage.StorageName;
                rJ_TextBox_儲位管理_包裝數量.Texts = storage.Min_Package_Num;

                this.Invoke(new Action(delegate { plC_CheckBox_儲位管理_儲位內容_手勢感測.Checked = storage.TOFON; }));
                plC_CheckBox_儲位管理_警報.Checked = storage.AlarmEnable;

                this.storagePanel.DrawToPictureBox(storage);
            }

            sqL_DataGridView_儲位管理_儲位內容_效期及庫存.ClearGrid();
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < storage.List_Validity_period.Count; i++)
            {
                object[] value = new object[new enum_儲位管理_效期及庫存().GetLength()];
                value[(int)enum_儲位管理_效期及庫存.效期] = storage.List_Validity_period[i];
                value[(int)enum_儲位管理_效期及庫存.批號] = storage.List_Lot_number[i];
                value[(int)enum_儲位管理_效期及庫存.庫存] = storage.List_Inventory[i];
                list_value.Add(value);
            }

            sqL_DataGridView_儲位管理_儲位內容_效期及庫存.RefreshGrid(list_value);
        }

        private void StoragePanel_SureClick(Storage storage)
        {
            _storageUI_EPD_266.SQL_ReplaceStorage(storage);
            this.storagePanel.DrawToPictureBox(storage);
        }
        private void StoragePanel_SizeChanged(object sender, EventArgs e)
        {
            this.storagePanel.Location = new Point((this.storagePanel.Parent.Width - this.storagePanel.Width) / 2, (this.storagePanel.Parent.Height - this.storagePanel.Height) / 2);
        }

        private void Program_儲位管理()
        {
            if (this.plC_ScreenPage_Main.PageText == "儲位管理")
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
        #endregion

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

            Dictionary<string, List<medClass>> keyValuePairs_medcloud = medClass.get_med_cloud(API_Server)?.CoverToDictionaryByCode();
            Dictionary<string, List<medConfigClass>> keyValuePairs_medConfig = medConfigClass.get_all(API_Server)?.CoverToDictionaryByCode();

            List<Storage> list_replaceValue = new List<Storage>();


            Parallel.ForEach(List_EPD266_本地資料, value =>
            {
                if (List_EPD266_本地資料 == null) return;

                string 藥品碼 = "";
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
                medClass _medClass = keyValuePairs_medcloud.SortDictionaryByCode(藥品碼).FirstOrDefault();
                medConfigClass medConfigClass = keyValuePairs_medConfig.SortDictionaryByCode(藥品碼).FirstOrDefault();
                if (_medClass == null)
                {
                    storage.Clear();
                    Is_Replace = true;
                }
                else
                {
                    藥品碼_buf = _medClass.藥品碼;
                    藥品名稱_buf = _medClass.藥品名稱;
                    中文名稱_buf = _medClass.中文名稱;
                    藥品學名_buf = _medClass.藥品學名;
                    包裝單位_buf = _medClass.包裝單位;
                    管制級別_buf = _medClass.管制級別;
                    警訊藥品_buf = _medClass.警訊藥品.ToUpper();
                    if (_medClass.警訊藥品.StringIsEmpty()) _medClass.警訊藥品 = false.ToString().ToUpper();

                    if (medConfigClass !=null)
                    {
                        麻醉藥品_buf = medConfigClass.麻醉藥品;
                        形狀相似_buf = medConfigClass.形狀相似;
                        發音相似_buf = medConfigClass.發音相似;
                    }
                    else
                    {
                        麻醉藥品_buf = false.ToString().ToUpper();
                        形狀相似_buf = false.ToString().ToUpper();
                        發音相似_buf = false.ToString().ToUpper();
                    }

                    藥品碼 = storage.GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
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

                    if (藥品碼 != _medClass.藥品碼) Is_Replace = true;
                    if (藥品名稱 != _medClass.藥品名稱) Is_Replace = true;
                    if (中文名稱 != _medClass.中文名稱) Is_Replace = true;
                    if (藥品學名 != _medClass.藥品學名) Is_Replace = true;
                    if (包裝單位 != _medClass.包裝單位) Is_Replace = true;
                    if (警訊藥品 != _medClass.警訊藥品) Is_Replace = true;
                    if (管制級別 != _medClass.管制級別) Is_Replace = true;
                    if (麻醉藥品 != 麻醉藥品_buf) Is_Replace = true;
                    if (形狀相似 != 形狀相似_buf) Is_Replace = true;
                    if (發音相似 != 發音相似_buf) Is_Replace = true;

                    storage.SetValue(Device.ValueName.藥品碼, Device.ValueType.Value, 藥品碼_buf);
                    storage.SetValue(Device.ValueName.藥品名稱, Device.ValueType.Value, 藥品名稱_buf);
                    storage.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.Value, 中文名稱_buf);
                    storage.SetValue(Device.ValueName.藥品學名, Device.ValueType.Value, 藥品學名_buf);
                    storage.SetValue(Device.ValueName.BarCode, Device.ValueType.Value, BarCode_buf);
                    storage.SetValue(Device.ValueName.包裝單位, Device.ValueType.Value, 包裝單位_buf);
                    storage.DRUGKIND = 管制級別_buf;
                    storage.IsWarning = (警訊藥品_buf == "TRUE");
                    storage.IsAnesthetic = (麻醉藥品_buf == "TRUE");
                    storage.IsShapeSimilar = (形狀相似_buf == "TRUE");
                    storage.IsSoundSimilar = (發音相似_buf == "TRUE");

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
            bool flag_顯示空白儲位 = true;
            List<object[]> list_value = new List<object[]>();
            for (int i = 0; i < List_EPD266_本地資料.Count; i++)
            {
                object[] value = new object[new enum_儲位管理_儲位資料().GetLength()];
                string 藥品碼 = List_EPD266_本地資料[i].GetValue(Device.ValueName.藥品碼, Device.ValueType.Value).ObjectToString();
                if (!flag_顯示空白儲位)
                {
                    if (藥品碼.StringIsEmpty()) continue;
                }
                value[(int)enum_儲位管理_儲位資料.IP] = List_EPD266_本地資料[i].GetValue(Device.ValueName.IP, Device.ValueType.Value).ObjectToString();
                list_value.Add(value);
            }
            list_value.Sort(new ICP_儲位管理_抽屜列表());
            this.sqL_DataGridView_儲位管理_儲位資料.RefreshGrid(list_value);
            Console.Write($"儲位管理EPD266:更新儲位資料完成 ,耗時 :{MyTimer_TickTime.GetTickTime().ToString("0.000")}\n");
            cnt++;
        }

        #endregion

        private class ICP_儲位管理_抽屜列表 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string IP_0 = x[(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                string IP_1 = y[(int)enum_儲位管理_儲位資料.IP].ObjectToString();
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
