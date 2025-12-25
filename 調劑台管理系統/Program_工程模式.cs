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
using H_Pannel_lib;
using HIS_DB_Lib;
using SQLUI;
using DrawingClass;
namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
        public static Panel _panel_工程模式_領藥台_01_顏色;
        public static Panel _panel_工程模式_領藥台_02_顏色;
        public static Panel _panel_工程模式_領藥台_03_顏色;
        public static Panel _panel_工程模式_領藥台_04_顏色;

        private void Program_工程模式_Init()
        {
            _panel_工程模式_領藥台_01_顏色 = this.panel_工程模式_領藥台_01_顏色;
            _panel_工程模式_領藥台_02_顏色 = this.panel_工程模式_領藥台_02_顏色;
            _panel_工程模式_領藥台_03_顏色 = this.panel_工程模式_領藥台_03_顏色;
            _panel_工程模式_領藥台_04_顏色 = this.panel_工程模式_領藥台_04_顏色;

            this.plC_Button_工程模式_全部開鎖.btnClick += PlC_Button_工程模式_全部開鎖_btnClick;
            this.button_工程模式_調劑台名稱儲存.Click += Button_工程模式_調劑台名稱儲存_Click;
            this.plC_UI_Init.Add_Method(this.sub_Program_工程模式);

            panel_工程模式_領藥台_01_顏色.Click += Panel_工程模式_領藥台_01_顏色_Click;
            panel_工程模式_領藥台_02_顏色.Click += Panel_工程模式_領藥台_02_顏色_Click;
            panel_工程模式_領藥台_03_顏色.Click += Panel_工程模式_領藥台_03_顏色_Click;
            panel_工程模式_領藥台_04_顏色.Click += Panel_工程模式_領藥台_04_顏色_Click;
        }

        bool flag_工程模式_頁面更新 = false;
        private void sub_Program_工程模式()
        {
            if (plC_NumBox_亮燈亮度.Value > 100) plC_NumBox_亮燈亮度.Value = 100;
            if (plC_NumBox_亮燈亮度.Value <= 0) plC_NumBox_亮燈亮度.Value = 80;

            if (PLC_Device_導引模式.Bool) PLC_Device_掃碼顏色固定.Bool = true;

            DrawerUI_EPD_583.Lightness = plC_NumBox_亮燈亮度.Value / 100D;
            RowsLEDUI.Lightness = plC_NumBox_亮燈亮度.Value / 100D;
            StorageUI_EPD_266.Lightness = plC_NumBox_亮燈亮度.Value / 100D;
            CommonSapceMethod.Lightness = plC_NumBox_亮燈亮度.Value / 100D;

            if (this.plC_ScreenPage_Main.PageText == "工程模式")
            {
                if (!this.flag_工程模式_頁面更新)
                {
                    this.sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(true);
                    Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.操作工程模式, this.登入者名稱, "");
                    this.Function_工程模式_鎖控按鈕更新();
                    this.flag_工程模式_頁面更新 = true;
                }
            }
            else
            {
                this.flag_工程模式_頁面更新 = false;
            }
            this.sub_Program_檢查Pannel35_主畫面();
        }

        #region PLC_檢查Pannel35_主畫面
        PLC_Device PLC_Device_檢查Pannel35_主畫面 = new PLC_Device("");
        PLC_Device PLC_Device_檢查Pannel35_主畫面_OK = new PLC_Device("");
        int cnt_Program_檢查Pannel35_主畫面 = 65534;
        void sub_Program_檢查Pannel35_主畫面()
        {
            if (PLC_Device_主機扣賬模式.Bool) PLC_Device_檢查Pannel35_主畫面.Bool = true;
            if (cnt_Program_檢查Pannel35_主畫面 == 65534)
            {
                PLC_Device_檢查Pannel35_主畫面.SetComment("PLC_檢查Pannel35_主畫面");
                PLC_Device_檢查Pannel35_主畫面_OK.SetComment("PLC_檢查Pannel35_主畫面_OK");
                PLC_Device_檢查Pannel35_主畫面.Bool = false;
                cnt_Program_檢查Pannel35_主畫面 = 65535;
            }
            if (cnt_Program_檢查Pannel35_主畫面 == 65535) cnt_Program_檢查Pannel35_主畫面 = 1;
            if (cnt_Program_檢查Pannel35_主畫面 == 1) cnt_Program_檢查Pannel35_主畫面_檢查按下(ref cnt_Program_檢查Pannel35_主畫面);
            if (cnt_Program_檢查Pannel35_主畫面 == 2) cnt_Program_檢查Pannel35_主畫面_初始化(ref cnt_Program_檢查Pannel35_主畫面);
            if (cnt_Program_檢查Pannel35_主畫面 == 3) cnt_Program_檢查Pannel35_主畫面 = 65500;
            if (cnt_Program_檢查Pannel35_主畫面 > 1) cnt_Program_檢查Pannel35_主畫面_檢查放開(ref cnt_Program_檢查Pannel35_主畫面);

            if (cnt_Program_檢查Pannel35_主畫面 == 65500)
            {
                PLC_Device_檢查Pannel35_主畫面.Bool = false;
                PLC_Device_檢查Pannel35_主畫面_OK.Bool = false;
                cnt_Program_檢查Pannel35_主畫面 = 65535;
            }
        }
        void cnt_Program_檢查Pannel35_主畫面_檢查按下(ref int cnt)
        {
            if (PLC_Device_檢查Pannel35_主畫面.Bool) cnt++;
        }
        void cnt_Program_檢查Pannel35_主畫面_檢查放開(ref int cnt)
        {
            if (!PLC_Device_檢查Pannel35_主畫面.Bool) cnt = 65500;
        }
        void cnt_Program_檢查Pannel35_主畫面_初始化(ref int cnt)
        {
            List<StorageUI_WT32.UDP_READ> uDP_READs = this.storageUI_WT32.GerAllUDP_READ();
            List<string> list_refresh_ip = new List<string>();
            for (int i = 0; i < uDP_READs.Count; i++)
            {
                if (uDP_READs[i].Screen_Page == (int)StorageUI_WT32.enum_Page.主頁面 && uDP_READs[i].ScreenPage_Init == true)
                {
                    list_refresh_ip.Add(uDP_READs[i].IP);
                }
            }
            List<Task> taskList = new List<Task>();
            List<Storage> storages = List_Pannel35_雲端資料;
            try
            {
                for (int i = 0; i < list_refresh_ip.Count; i++)
                {
                    List<Storage> storages_buf = (from value in storages
                                                  where value.IP == list_refresh_ip[i]
                                                  select value).ToList();
                    if (storages_buf.Count > 0)
                    {
                        taskList.Add(Task.Run(() =>
                        {
                            this.storageUI_WT32.Set_DrawPannelJEPG(storages_buf[0]);
                        }));
                    }

                }

                Task allTask = Task.WhenAll(taskList);
                allTask.Wait();
            }
            catch
            {

            }

            cnt++;
        }


        #endregion
        #region Function
        private void Function_工程模式_鎖控按鈕更新()
        {
            Function_從SQL取得儲位到本地資料();
            List<object[]> list_locker_table_value = this.sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(false);
            List<object[]> list_locker_table_value_buf = new List<object[]>();
            string OutputAdress = "";
            string IP = "";
            int Num = -1;
            for (int i = 0; i < this.List_Locker.Count; i++)
            {
                OutputAdress = List_Locker[i].Get_OutputAdress();
                list_locker_table_value_buf = list_locker_table_value.GetRows((int)enum_lockerIndex.輸出位置, OutputAdress);
                if (list_locker_table_value_buf.Count == 0)
                {
                    List_Locker[i].Visible = false;
                    continue;
                }
                IP = list_locker_table_value_buf[0][(int)enum_lockerIndex.IP].ObjectToString();
                Num = list_locker_table_value_buf[0][(int)enum_lockerIndex.Num].ObjectToString().StringToInt32();
                object device = Fucnction_從本地資料取得儲位(IP);
                if (device == null)
                {
                    List_Locker[i].Visible = false;
                    continue;
                }
                if(device is Drawer)
                {
                    Drawer drawer = device as Drawer;
                    List_Locker[i].IP = IP;
                    List_Locker[i].Visible = true;
                    if (drawer.Name.StringIsEmpty()) continue;
                    List_Locker[i].Name = drawer.Name;
                }
                if(device is Storage)
                {
                    Storage storage = device as Storage;
                    List_Locker[i].IP = IP;
                    List_Locker[i].Visible = true;
                    if (storage.Name.StringIsEmpty()) continue;
                    List_Locker[i].Name = storage.Name;
                }
                if(device is RowsLED)
                {
                    RowsLED rowsLED = device as RowsLED;
                    List_Locker[i].IP = IP;
                    List_Locker[i].Visible = true;
                    if (rowsLED.Name.StringIsEmpty()) continue;
                    List_Locker[i].Name = rowsLED.Name;
                }
                if(device is RFIDClass)
                {
                    if (Num == -1) return;
                    RFIDClass rFIDClass = device as RFIDClass;
                    RFIDClass.DeviceClass deviceClass = rFIDClass.DeviceClasses[Num];
                    List_Locker[i].IP = IP;
                    List_Locker[i].Num = Num;
                    List_Locker[i].Visible = true;
                    if (deviceClass.Name.StringIsEmpty()) continue;
                    List_Locker[i].Name = deviceClass.Name;

                }
                else
                {
                    
                }
            }

        }

        #endregion
        #region Event

        private void Button_工程模式_調劑台名稱儲存_Click(object sender, EventArgs e)
        {
            SaveConfig工程模式();
            PLC.properties.Device.Set_Device("S8", true);
            MyMessageBox.ShowDialog("儲存成功!");
        }
        private void Panel_工程模式_領藥台_01_顏色_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.panel_工程模式_領藥台_01_顏色.BackColor = colorDialog.Color;
            }
        }
        private void Panel_工程模式_領藥台_02_顏色_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.panel_工程模式_領藥台_02_顏色.BackColor = colorDialog.Color;
            }
        }
        private void Panel_工程模式_領藥台_03_顏色_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.panel_工程模式_領藥台_03_顏色.BackColor = colorDialog.Color;
            }
        }
        private void Panel_工程模式_領藥台_04_顏色_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.panel_工程模式_領藥台_04_顏色.BackColor = colorDialog.Color;
            }
        }
       
        private void PlC_Button_工程模式_全部開鎖_btnClick(object sender, EventArgs e)
        {
            if (MyMessageBox.ShowDialog("是否全部抽屜解鎖?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
            for (int i = 0; i < this.List_Locker.Count; i++)
            {
                if (this.List_Locker[i].Visible) this.List_Locker[i].Open();
            }
            List<object[]> list_locker_table_value = this.sqL_DataGridView_Locker_Index_Table.SQL_GetAllRows(false);
            for (int i = 0; i < list_locker_table_value.Count; i++)
            {
                list_locker_table_value[i][(int)enum_lockerIndex.輸出狀態] = true.ToString();
            }
            this.sqL_DataGridView_Locker_Index_Table.SQL_ReplaceExtra(list_locker_table_value, false);
        }
        #endregion
        #region StreamIO
        [Serializable]
        private class SaveConfig工程模式Class
        {
            public string 領藥台_01_名稱 = "";
            public string 領藥台_02_名稱 = "";
            public string 領藥台_03_名稱 = "";
            public string 領藥台_04_名稱 = "";
            public string 門未關閉警報語音內容 = "";

            public Color 領藥台_01_Color = Color.Black;
            public Color 領藥台_02_Color = Color.Black;
            public Color 領藥台_03_Color = Color.Black;
            public Color 領藥台_04_Color = Color.Black;

            public int 領藥台_01_RowsHeight = 80;
            public int 領藥台_02_RowsHeight = 80;
            public int 領藥台_03_RowsHeight = 80;
            public int 領藥台_04_RowsHeight = 80;

            public string 領藥台_01_columns_jsonStr;
            public string 領藥台_02_columns_jsonStr;
            public string 領藥台_03_columns_jsonStr;
            public string 領藥台_04_columns_jsonStr;

            public int 領藥台_01_藥品資訊_Height = 275;
            public int 領藥台_02_藥品資訊_Height = 275;

            public bool flah_圖文辨識樣式01 = false;
            public bool flah_圖文辨識樣式02 = false;
            public bool flah_圖文辨識樣式03 = false;
            public bool flah_圖文辨識樣式04 = false;
        }
        static public void SaveConfig工程模式()
        {
            
            string StreamName = $@"{currentDirectory}\adminConfig.pro";
            SaveConfig工程模式Class saveConfig = new SaveConfig工程模式Class();

            saveConfig.領藥台_01_Color = _panel_工程模式_領藥台_01_顏色.BackColor;
            saveConfig.領藥台_02_Color = _panel_工程模式_領藥台_02_顏色.BackColor;
            saveConfig.領藥台_03_Color = _panel_工程模式_領藥台_03_顏色.BackColor;
            saveConfig.領藥台_04_Color = _panel_工程模式_領藥台_04_顏色.BackColor;

            saveConfig.領藥台_01_columns_jsonStr = _sqL_DataGridView_領藥台_01_領藥內容.GetColumnsJsonStr();
            saveConfig.領藥台_02_columns_jsonStr = _sqL_DataGridView_領藥台_02_領藥內容.GetColumnsJsonStr();
            saveConfig.領藥台_03_columns_jsonStr = _sqL_DataGridView_領藥台_03_領藥內容.GetColumnsJsonStr();
            saveConfig.領藥台_04_columns_jsonStr = _sqL_DataGridView_領藥台_04_領藥內容.GetColumnsJsonStr();

            saveConfig.領藥台_01_藥品資訊_Height = _panel_領藥台_01_藥品資訊.Height;
            saveConfig.領藥台_02_藥品資訊_Height = _panel_領藥台_02_藥品資訊.Height;

            saveConfig.flah_圖文辨識樣式01 = _sqL_DataGridView_領藥台_01_領藥內容.CustomEnable;
            saveConfig.flah_圖文辨識樣式02 = _sqL_DataGridView_領藥台_02_領藥內容.CustomEnable;
            saveConfig.flah_圖文辨識樣式03 = _sqL_DataGridView_領藥台_03_領藥內容.CustomEnable;
            saveConfig.flah_圖文辨識樣式04 = _sqL_DataGridView_領藥台_04_領藥內容.CustomEnable;

            saveConfig.領藥台_01_RowsHeight = _sqL_DataGridView_領藥台_01_領藥內容.RowsHeight;
            saveConfig.領藥台_02_RowsHeight = _sqL_DataGridView_領藥台_02_領藥內容.RowsHeight;
            saveConfig.領藥台_03_RowsHeight = _sqL_DataGridView_領藥台_03_領藥內容.RowsHeight;
            saveConfig.領藥台_04_RowsHeight = _sqL_DataGridView_領藥台_04_領藥內容.RowsHeight;

            Basic.FileIO.SaveProperties(saveConfig, StreamName);
        }
        public void LoadConfig工程模式()
        {
            string StreamName = $@"{currentDirectory}\adminConfig.pro";
            object temp = new object();
            Basic.FileIO.LoadProperties(ref temp, StreamName);
            if(temp is SaveConfig工程模式Class)
            {
                SaveConfig工程模式Class saveConfig = (SaveConfig工程模式Class)temp;
               this.Invoke(new Action(delegate 
                {

                    this.panel_工程模式_領藥台_01_顏色.BackColor = saveConfig.領藥台_01_Color;
                    this.panel_工程模式_領藥台_02_顏色.BackColor = saveConfig.領藥台_02_Color;
                    this.panel_工程模式_領藥台_03_顏色.BackColor = saveConfig.領藥台_03_Color;
                    this.panel_工程模式_領藥台_04_顏色.BackColor = saveConfig.領藥台_04_Color;
                    if(ControlMode == false)
                    {
                        _sqL_DataGridView_領藥台_01_領藥內容.CustomEnable = saveConfig.flah_圖文辨識樣式01;
                        _sqL_DataGridView_領藥台_02_領藥內容.CustomEnable = saveConfig.flah_圖文辨識樣式02;
                        _sqL_DataGridView_領藥台_03_領藥內容.CustomEnable = saveConfig.flah_圖文辨識樣式03;
                        _sqL_DataGridView_領藥台_04_領藥內容.CustomEnable = saveConfig.flah_圖文辨識樣式04;
                        if (saveConfig.領藥台_01_columns_jsonStr.StringIsEmpty() == false) _sqL_DataGridView_領藥台_01_領藥內容.SetColumnsJsonStr(saveConfig.領藥台_01_columns_jsonStr);
                        if (saveConfig.領藥台_02_columns_jsonStr.StringIsEmpty() == false) _sqL_DataGridView_領藥台_02_領藥內容.SetColumnsJsonStr(saveConfig.領藥台_02_columns_jsonStr);
                        if (saveConfig.領藥台_03_columns_jsonStr.StringIsEmpty() == false) _sqL_DataGridView_領藥台_03_領藥內容.SetColumnsJsonStr(saveConfig.領藥台_03_columns_jsonStr);
                        if (saveConfig.領藥台_04_columns_jsonStr.StringIsEmpty() == false) _sqL_DataGridView_領藥台_04_領藥內容.SetColumnsJsonStr(saveConfig.領藥台_04_columns_jsonStr);

                        if (_sqL_DataGridView_領藥台_01_領藥內容.CustomEnable) { _sqL_DataGridView_領藥台_01_領藥內容.顯示首列 = false; _sqL_DataGridView_領藥台_01_領藥內容.顯示首行 = false; }
                        if (_sqL_DataGridView_領藥台_02_領藥內容.CustomEnable) { _sqL_DataGridView_領藥台_02_領藥內容.顯示首列 = false; _sqL_DataGridView_領藥台_02_領藥內容.顯示首行 = false; }
                        if (_sqL_DataGridView_領藥台_03_領藥內容.CustomEnable) { _sqL_DataGridView_領藥台_03_領藥內容.顯示首列 = false; _sqL_DataGridView_領藥台_03_領藥內容.顯示首行 = false; }
                        if (_sqL_DataGridView_領藥台_04_領藥內容.CustomEnable) { _sqL_DataGridView_領藥台_04_領藥內容.顯示首列 = false; _sqL_DataGridView_領藥台_04_領藥內容.顯示首行 = false; }

                        if (saveConfig.領藥台_01_RowsHeight == 0) saveConfig.領藥台_01_RowsHeight = 80;
                        if (saveConfig.領藥台_02_RowsHeight == 0) saveConfig.領藥台_02_RowsHeight = 80;
                        if (saveConfig.領藥台_03_RowsHeight == 0) saveConfig.領藥台_03_RowsHeight = 80;
                        if (saveConfig.領藥台_04_RowsHeight == 0) saveConfig.領藥台_04_RowsHeight = 80;

                        _sqL_DataGridView_領藥台_01_領藥內容.RowsHeight = saveConfig.領藥台_01_RowsHeight;
                        _sqL_DataGridView_領藥台_02_領藥內容.RowsHeight = saveConfig.領藥台_02_RowsHeight;
                        _sqL_DataGridView_領藥台_03_領藥內容.RowsHeight = saveConfig.領藥台_03_RowsHeight;
                        _sqL_DataGridView_領藥台_04_領藥內容.RowsHeight = saveConfig.領藥台_04_RowsHeight;


                        _sqL_DataGridView_領藥台_01_領藥內容.Init();
                        _sqL_DataGridView_領藥台_02_領藥內容.Init();
                        _sqL_DataGridView_領藥台_03_領藥內容.Init();
                        _sqL_DataGridView_領藥台_04_領藥內容.Init();




                        if (_sqL_DataGridView_領藥台_01_領藥內容.CustomEnable)
                        {
                            uC_調劑作業_TypeA_1.sqL_DataGridView_領藥內容.RowPostPaintingEventEx += SqL_DataGridView_領藥內容_RowPostPaintingEventEx;
                            uC_調劑作業_TypeA_1.panel_藥品資訊.Visible = false;
                        }
                        if (_sqL_DataGridView_領藥台_02_領藥內容.CustomEnable)
                        {
                            uC_調劑作業_TypeA_2.sqL_DataGridView_領藥內容.RowPostPaintingEventEx += SqL_DataGridView_領藥內容_RowPostPaintingEventEx;
                            uC_調劑作業_TypeA_2.panel_藥品資訊.Visible = false;
                        }
                        if (_sqL_DataGridView_領藥台_03_領藥內容.CustomEnable) _sqL_DataGridView_領藥台_03_領藥內容.RowPostPaintingEventEx += SqL_DataGridView_領藥內容_RowPostPaintingEventEx;
                        if (_sqL_DataGridView_領藥台_04_領藥內容.CustomEnable) _sqL_DataGridView_領藥台_04_領藥內容.RowPostPaintingEventEx += SqL_DataGridView_領藥內容_RowPostPaintingEventEx;

                        if (saveConfig.領藥台_01_藥品資訊_Height == 0) saveConfig.領藥台_01_藥品資訊_Height = 275;
                        if (saveConfig.領藥台_02_藥品資訊_Height == 0) saveConfig.領藥台_02_藥品資訊_Height = 275;
                        _panel_領藥台_01_藥品資訊.Height = saveConfig.領藥台_01_藥品資訊_Height;
                        _panel_領藥台_02_藥品資訊.Height = saveConfig.領藥台_02_藥品資訊_Height;
                    }
                

                }));
            }

        }



   
        #endregion
    }
}
