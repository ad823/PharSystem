using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using MyUI;
using Basic;
using SQLUI;
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using HIS_DB_Lib;
using MyOffice;
using H_Pannel_lib;


namespace 調劑台管理系統
{
    public partial class Dialog_交班對點 : MyDialog
    {
        public enum enum_交班藥品
        {
            [Description("GUID,VARCHAR,200,NONE")]
            GUID,
            [Description("排列號,VARCHAR,200,NONE")]
            排列號,
            [Description("藥碼,VARCHAR,200,NONE")]
            藥碼,
            [Description("藥名,VARCHAR,200,NONE")]
            藥名,
            [Description("單位,VARCHAR,200,NONE")]
            單位,
            [Description("庫存,VARCHAR,200,NONE")]
            庫存,
            [Description("盤點量,VARCHAR,200,NONE")]
            盤點量,
            [Description("覆盤量,VARCHAR,200,NONE")]
            覆盤量,
            [Description("差異值,VARCHAR,200,NONE")]
            差異值,
            [Description("收支原因,VARCHAR,200,NONE")]
            收支原因,
            [Description("確認時間,VARCHAR,200,NONE")]
            確認時間,
            [Description("備註,VARCHAR,200,NONE")]
            備註,
        }
        static public List<object[]> _list_交班對點_buf = null;
        static public List<object[]> list_交班對點_buf
        {
            get
            {
                return _list_交班對點_buf;
            }
            set
            {
                _list_交班對點_buf = value;
                if (value == null) guid = string.Empty;
            }
        }
        static string guid = string.Empty;
        public bool flag_單人交班 = false;  
        private bool flag_確認輸入 = false;
        private MyThread myThread_program;
        private personPageClass personPageClass_盤點人員 = null;
        private personPageClass personPageClass_覆盤人員 = null;
        private List<medGroupClass> medGroupClasses = null;
        public Dialog_交班對點()
        {
            form.Invoke(new Action(delegate { InitializeComponent(); }));

            this.sqL_DataGridView_交班藥品.RowEnterEvent += SqL_DataGridView_交班藥品_RowEnterEvent;
            this.LoadFinishedEvent += Dialog_交班對點_LoadFinishedEvent;
            this.FormClosing += Dialog_交班對點_FormClosing;
            this.rJ_Button_藥品群組_選擇.MouseDownEvent += RJ_Button_藥品群組_選擇_MouseDownEvent;
            this.plC_RJ_Button_解鎖.MouseDownEvent += PlC_RJ_Button_解鎖_MouseDownEvent;
            this.plC_RJ_Button_盤點登入.MouseDownEvent += PlC_RJ_Button_盤點登入_MouseDownEvent;
            this.plC_RJ_Button_覆盤登入.MouseDownEvent += PlC_RJ_Button_覆盤登入_MouseDownEvent;
            this.rJ_Button_確認輸入.MouseDownEvent += RJ_Button_確認輸入_MouseDownEvent;
            this.rJ_Button_0.MouseDownEvent += RJ_Button_0_MouseDownEvent;
            this.rJ_Button_9.MouseDownEvent += RJ_Button_9_MouseDownEvent;
            this.rJ_Button_8.MouseDownEvent += RJ_Button_8_MouseDownEvent;
            this.rJ_Button_7.MouseDownEvent += RJ_Button_7_MouseDownEvent;
            this.rJ_Button_6.MouseDownEvent += RJ_Button_6_MouseDownEvent;
            this.rJ_Button_5.MouseDownEvent += RJ_Button_5_MouseDownEvent;
            this.rJ_Button_4.MouseDownEvent += RJ_Button_4_MouseDownEvent;
            this.rJ_Button_3.MouseDownEvent += RJ_Button_3_MouseDownEvent;
            this.rJ_Button_2.MouseDownEvent += RJ_Button_2_MouseDownEvent;
            this.rJ_Button_1.MouseDownEvent += RJ_Button_1_MouseDownEvent;
            this.rJ_Button_CE.MouseDownEvent += RJ_Button_CE_MouseDownEvent;
            this.rJ_Button_dot.MouseDownEvent += RJ_Button_dot_MouseDownEvent;

            this.plC_RJ_Button_確認送出.MouseDownEvent += PlC_RJ_Button_確認送出_MouseDownEvent;
            this.plC_RJ_Button_重新盤點.MouseDownEvent += PlC_RJ_Button_重新盤點_MouseDownEvent;

            Main_Form.LockClosingEvent += Main_Form_LockClosingEvent;
        }

        private void Main_Form_LockClosingEvent(object sender, PLC_Device PLC_Device_Input, PLC_Device PLC_Device_Output, string GUID)
        {
            try
            {
                string input_adress = PLC_Device_Input.GetAdress();
                Logger.Log($"[確認輸入] 取得輸入位置: {input_adress}");

                List<object[]> list_locker_table_value = Main_Form._sqL_DataGridView_Locker_Index_Table.SQL_GetRows((int)enum_lockerIndex.輸入位置, input_adress, false);
                Logger.Log($"[確認輸入] Locker Index 查詢筆數: {list_locker_table_value.Count}");

                List<lockerIndexClass> lockerIndexClasses = list_locker_table_value.SQLToClass<lockerIndexClass, enum_lockerIndex>();
                if (lockerIndexClasses.Count == 0)
                {
                    Logger.Log($"[確認輸入] 查無對應 Locker Index");
                    return;
                }

                lockerIndexClass _lockerIndexClass = lockerIndexClasses[0];
                Logger.Log($"[確認輸入] 對應儲位 IP: {_lockerIndexClass.IP}");

                object device = Main_Form.Fucnction_從雲端資料取得儲位(_lockerIndexClass.IP);
                if (device == null)
                {
                    Logger.Log($"[確認輸入] 無法取得儲位 Device，IP: {_lockerIndexClass.IP}");
                    return;
                }
                List<object[]> list_交班對點 = this.sqL_DataGridView_交班藥品.Get_All_Select_RowsValues();

                string 藥碼 = list_交班對點[0][(int)enum_交班藥品.藥碼].ObjectToString();
                if (list_交班對點.Count == 0) return;
                if (device is DeviceBasic)
                {
                    DeviceBasic deviceBasic = (DeviceBasic)device;
                    Logger.Log($"[確認輸入] 取得 DeviceBasic，藥碼: {deviceBasic.Code}");

                    if (list_交班對點.Count == 0)
                    {
                        Logger.Log($"[確認輸入] 無交班藥品被選取");
                        return;
                    }

                    Logger.Log($"[確認輸入] 被選取藥碼: {藥碼}");

                    if (藥碼 == deviceBasic.Code)
                    {
                        Logger.Log($"[確認輸入] 藥碼比對成功: {藥碼}");
                        flag_確認輸入 = true;
                    }
                    else
                    {
                        Logger.Log($"[確認輸入] 藥碼比對失敗，DeviceIP: {deviceBasic.IP}, 選取藥碼: {藥碼}");
                    }
                }
                else if (device is Drawer)
                {
                    Drawer drawer = (Drawer)device;
                    if (drawer.SortByCode(藥碼).Count > 0)
                    {
                        Logger.Log($"[確認輸入] 藥碼比對成功: {藥碼}");
                        flag_確認輸入 = true;
                    }
                    else
                    {
                        Logger.Log($"[確認輸入] 藥碼比對失敗，DeviceIP: {drawer.IP}, 選取藥碼: {藥碼}");
                    }
                }
                else
                {
                    Logger.Log($"[確認輸入] 取得的 device 並非 DeviceBasic 類型，實際型別: {device.GetType().Name}");
                }
            }
            catch(Exception ex)
            {
                Logger.Log($"Exception : {ex.Message}");

            }
            finally
            {

            }
          

        }

        private void RJ_Button_dot_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (rJ_TextBox_盤點量.Text.StringToDouble() != 0 && rJ_TextBox_盤點量.Text.Contains(".") == false)
                {
                    rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}.".ToString(); ;
                }
            }));
        }
        private void RJ_Button_CE_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = "";
            }));
        }
        private void RJ_Button_1_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}1".StringToDouble().ToString();
            }));
        }
        private void RJ_Button_2_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}2".StringToDouble().ToString();
            }));
        }
        private void RJ_Button_3_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}3".StringToDouble().ToString();
            }));
        }
        private void RJ_Button_4_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}4".StringToDouble().ToString();
            }));
        }
        private void RJ_Button_5_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}5".StringToDouble().ToString();
            }));
        }
        private void RJ_Button_6_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}6".StringToDouble().ToString();
            }));
        }
        private void RJ_Button_7_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}7".StringToDouble().ToString();
            }));
        }
        private void RJ_Button_8_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}8".StringToDouble().ToString();
            }));
        }
        private void RJ_Button_9_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}9".StringToDouble().ToString();
            }));
        }
        private void RJ_Button_0_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (rJ_TextBox_盤點量.Text.StringToDouble() != 0)
                {
                    rJ_TextBox_盤點量.Text = $"{rJ_TextBox_盤點量.Text}0".StringToDouble().ToString(); ;
                }         
            }));
        }

        #region Function
        private bool flag_program_init = false;
        private bool flag_自動彈出詢問送出報表 = false;
        private bool flag_繼續盤點 = false;
        private void sub_program()
        {
            try
            {
                if(flag_program_init == false)
                {
                    flag_program_init = true;
                    PlC_RJ_Button_盤點登入_MouseDownEvent(null);

                    if (list_交班對點_buf != null && list_交班對點_buf.Count != 0)
                    {
                        if (MyMessageBox.ShowDialog("有交班表未完成,是否繼續盤點?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                        {
                            LoadingForm.ShowLoadingForm();

                            for (int i = 0; i < list_交班對點_buf.Count; i++)
                            {
                                string code = list_交班對點_buf[i][(int)enum_交班藥品.藥碼].ObjectToString();
                                int 差異值 = medRecheckLogClass.get_unresolved_qty_by_code(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, code);
                                double 庫存 = Main_Form.Function_從SQL取得庫存(code);
                                list_交班對點_buf[i][(int)enum_交班藥品.庫存] = 差異值 + 庫存;
                            }

                            this.sqL_DataGridView_交班藥品.RefreshGrid(list_交班對點_buf);
                            flag_繼續盤點 = true;

                            LoadingForm.CloseLoadingForm();
                        }
                        else
                        {
                            list_交班對點_buf = null;
                        }
                    }
                }
                if (this.stepViewer.CurrentStep == 1)
                {
                    this.Invoke(new Action(delegate
                    {
                        plC_RJ_Button_盤點登入.Enabled = true;
                        plC_RJ_Button_覆盤登入.Enabled = false;
                        panel_藥品選擇.Enabled = false;
                        panel_交班內容.Enabled = false;
                    }));
                }
                if (this.stepViewer.CurrentStep == 2)
                {
                    this.Invoke(new Action(delegate
                    {
                    
                        plC_RJ_Button_盤點登入.Enabled = false;
                        plC_RJ_Button_覆盤登入.Enabled = true;
                        panel_藥品選擇.Enabled = false;
                        panel_交班內容.Enabled = false;
                    }));


                }
                if (this.stepViewer.CurrentStep == 3)
                {
                    plC_RJ_Button_解鎖.Enabled = true;

                    List<string> list_IP = new List<string>();
                    List<object[]> _list_交班對點 = this.sqL_DataGridView_交班藥品.GetAllRows();
                    for (int i = 0; i < _list_交班對點.Count; i++)
                    {
                        string code = _list_交班對點[i][(int)enum_交班藥品.藥碼].ObjectToString();

                        list_IP.LockAdd(Main_Form.Function_取得抽屜以藥品碼解鎖IP(code));
                    }
                    
                    list_IP = (from temp in list_IP
                               select temp).Distinct().ToList();

                    for (int i = 0; i < list_IP.Count; i++)
                    {
                        List<string> IPs = new List<string>();
                        IPs.LockAdd(list_IP[i]);
                        Main_Form.Function_抽屜解鎖(IPs);
                        System.Threading.Thread.Sleep(100);
                    }

                    if (list_交班對點_buf != null && flag_繼續盤點)
                    {
                        int index = 0;
                        for (int i = 0; i < list_交班對點_buf.Count; i++)
                        {
                            if (list_交班對點_buf[i][(int)enum_交班藥品.盤點量].ObjectToString().StringIsEmpty())
                            {
                                index = i;
                                break;
                            }
                        }
                        this.sqL_DataGridView_交班藥品.SetSelectRow(index);
                        this.stepViewer.Next();
                        PLC_Device_交班對點.Bool = true;
                    }
                    this.Invoke(new Action(delegate
                    {
                        plC_RJ_Button_盤點登入.Enabled = false;
                        plC_RJ_Button_覆盤登入.Enabled = false;
                        panel_藥品選擇.Enabled = true;
                        panel_交班內容.Enabled = false;
                    }));
                }
                if (this.stepViewer.CurrentStep == 4)
                {
                    this.Invoke(new Action(delegate
                    {
                        plC_RJ_Button_盤點登入.Enabled = false;
                        plC_RJ_Button_覆盤登入.Enabled = false;
                        panel_藥品選擇.Enabled = false;
                        panel_交班內容.Enabled = true;
                    }));
                }
                if (this.stepViewer.CurrentStep == 5)
                {
                    this.Invoke(new Action(delegate
                    {
                        
                        plC_RJ_Button_確認送出.Enabled = true;
                    }));
                }
                
                sub_Program_交班對點();
            }
            catch
            {

            }
   
        }
        #region PLC_交班對點
        int retry = 0;
        PLC_Device PLC_Device_交班對點 = new PLC_Device("");
        PLC_Device PLC_Device_交班對點_OK = new PLC_Device("");
        Task Task_交班對點;
        MyTimer MyTimer_交班對點_結束延遲 = new MyTimer();
        MyTimer MyTimer_交班對點_開始延遲 = new MyTimer();
        int cnt_Program_交班對點 = 65534;
        void sub_Program_交班對點()
        {
         
            if (cnt_Program_交班對點 == 65534)
            {
                this.MyTimer_交班對點_結束延遲.StartTickTime(10000);
                this.MyTimer_交班對點_開始延遲.StartTickTime(10000);
                PLC_Device_交班對點.SetComment("PLC_交班對點");
                PLC_Device_交班對點_OK.SetComment("PLC_交班對點_OK");
                PLC_Device_交班對點.Bool = false;
                cnt_Program_交班對點 = 65535;
            }
            if (this.stepViewer.CurrentStep != 4) return;
            if (cnt_Program_交班對點 == 65535) cnt_Program_交班對點 = 1;
            if (cnt_Program_交班對點 == 1) cnt_Program_交班對點_檢查按下(ref cnt_Program_交班對點);
            if (cnt_Program_交班對點 == 2) cnt_Program_交班對點_初始化(ref cnt_Program_交班對點);
            if (cnt_Program_交班對點 == 3) cnt_Program_交班對點_等待輸入盤點數量(ref cnt_Program_交班對點);
            if (cnt_Program_交班對點 == 4) cnt_Program_交班對點_選擇下一筆(ref cnt_Program_交班對點);
            if (cnt_Program_交班對點 == 5) cnt_Program_交班對點 = 65500;
            if (cnt_Program_交班對點 > 1) cnt_Program_交班對點_檢查放開(ref cnt_Program_交班對點);

            if (cnt_Program_交班對點 == 65500)
            {
                this.MyTimer_交班對點_結束延遲.TickStop();
                this.MyTimer_交班對點_結束延遲.StartTickTime(10000);
                PLC_Device_交班對點.Bool = false;
                PLC_Device_交班對點_OK.Bool = false;
                cnt_Program_交班對點 = 65535;
            }
        }
        void cnt_Program_交班對點_檢查按下(ref int cnt)
        {
            if (PLC_Device_交班對點.Bool) cnt++;
        }
        void cnt_Program_交班對點_檢查放開(ref int cnt)
        {
            if (!PLC_Device_交班對點.Bool) cnt = 65500;
        }
        void cnt_Program_交班對點_初始化(ref int cnt)
        {
            this.Invoke(new Action(delegate 
            {
                List<object[]> list_交班對點 = this.sqL_DataGridView_交班藥品.Get_All_Select_RowsValues();
                double 庫存 = list_交班對點[0][(int)enum_交班藥品.庫存].StringToDouble();
                rJ_TextBox_盤點量.Text = (Main_Form.PLC_Device_盤點_顯示庫存量及預帶盤點量.Bool ? 庫存.ToString() : "");
                rJ_Lable_狀態.Text = "請輸入【盤點】數量";
            }));
            cnt++;
        }
        void cnt_Program_交班對點_等待輸入盤點數量(ref int cnt)
        {
            if(flag_確認輸入)
            {
                System.Threading.Thread.Sleep(200);
                flag_確認輸入 = false;

                List<object[]> list_交班對點 = this.sqL_DataGridView_交班藥品.Get_All_Select_RowsValues();
                if (list_交班對點.Count == 0)
                {
                    MyMessageBox.ShowDialog("交班表選擇異常");
                    cnt = 65534;
                    return;
                }
                double 盤點量 = rJ_TextBox_盤點量.Text.StringToDouble();
                double 庫存 = list_交班對點[0][(int)enum_交班藥品.庫存].StringToDouble();
             
                if (盤點量.StringToDouble() < 0)
                {
                    this.Invoke(new Action(delegate
                    {
                        $"未輸入盤點值".PlayGooleVoiceAsync(Main_Form.API_Server);
                    }));
                    MyMessageBox.ShowDialog("未輸入盤點值");
                    this.Invoke(new Action(delegate
                    {
                        rJ_TextBox_盤點量.Text = (Main_Form.PLC_Device_盤點_顯示庫存量及預帶盤點量.Bool ? 庫存.ToString() : "");
                    }));
                    return;
                }
                list_交班對點[0][(int)enum_交班藥品.盤點量] = 盤點量.StringToDouble();


                if (盤點量 != 庫存)
                {
                    if (retry == 0)
                    {
                        this.Invoke(new Action(delegate 
                        {
                            $"盤點量與庫存不符,請確認是否正確".PlayGooleVoiceAsync(Main_Form.API_Server);
                        }));
                        MyMessageBox.ShowDialog("盤點量與庫存不符");
                        this.Invoke(new Action(delegate
                        {
                            rJ_TextBox_盤點量.Text = (Main_Form.PLC_Device_盤點_顯示庫存量及預帶盤點量.Bool ? 庫存.ToString() : "");
                            rJ_Lable_狀態.Text = "請輸入【覆盤】數量";
                        }));
                        retry++;
                        return;
                    }
                    if (retry == 1)
                    {
                        this.Invoke(new Action(delegate
                        {
                            $"盤點量與庫存再次不符,請確認是否正確".PlayGooleVoiceAsync(Main_Form.API_Server);
                        }));
                        if (MyMessageBox.ShowDialog($"盤點量與庫存不符,是否繼續下一步驟", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
                        Dialog_收支原因選擇 dialog_收支原因選擇 = new Dialog_收支原因選擇();
                        if (dialog_收支原因選擇.ShowDialog() != DialogResult.Yes) return;
                        list_交班對點[0][(int)enum_交班藥品.收支原因] = dialog_收支原因選擇.Value;
                    }

                }
                double 差異值 = 盤點量 - 庫存;
                list_交班對點[0][(int)enum_交班藥品.差異值] = 差異值;
                list_交班對點[0][(int)enum_交班藥品.確認時間] = DateTime.Now.ToDateTimeString_6();

                this.Invoke(new Action(delegate
                {
                    retry = 0;
                    rJ_TextBox_盤點量.Text = "";
                    rJ_Lable_狀態.Text = "-----------";
                }));
                this.sqL_DataGridView_交班藥品.ReplaceExtra(list_交班對點, true);
                cnt++;
            }
        }
        void cnt_Program_交班對點_等待輸入覆盤數量(ref int cnt)
        {
            if (flag_確認輸入)
            {
                flag_確認輸入 = false;

                List<object[]> list_交班對點 = this.sqL_DataGridView_交班藥品.Get_All_Select_RowsValues();
                if (list_交班對點.Count == 0)
                {
                    MyMessageBox.ShowDialog("交班表選擇異常");
                    cnt = 65534;
                    return;
                }
                double 覆盤量 = rJ_TextBox_盤點量.Text.StringToDouble();
                double 庫存 = list_交班對點[0][(int)enum_交班藥品.庫存].StringToDouble();

                if (覆盤量.StringToDouble() < 0)
                {
                    this.Invoke(new Action(delegate
                    {
                        $"未輸入覆盤值".PlayGooleVoiceAsync(Main_Form.API_Server);
                    }));
                    MyMessageBox.ShowDialog("未輸入覆盤值");
                    this.Invoke(new Action(delegate
                    {
                        rJ_TextBox_盤點量.Text = (Main_Form.PLC_Device_盤點_顯示庫存量及預帶盤點量.Bool ? 庫存.ToString() : "");
                    }));
                    return;
                }
                list_交班對點[0][(int)enum_交班藥品.覆盤量] = 覆盤量.StringToDouble();


                if (覆盤量 != 庫存)
                {
                    if (retry == 0)
                    {
                        this.Invoke(new Action(delegate
                        {
                            $"覆點量與庫存不符,請確認是否正確".PlayGooleVoiceAsync(Main_Form.API_Server);
                        }));

                        MyMessageBox.ShowDialog("覆盤量與庫存不符");
                        this.Invoke(new Action(delegate
                        {
                            rJ_TextBox_盤點量.Text = (Main_Form.PLC_Device_盤點_顯示庫存量及預帶盤點量.Bool ? 庫存.ToString() : "");
                            rJ_Lable_狀態.Text = "請輸入【覆盤】數量";
                        }));
                        retry++;
                        return;
                    }
                    if (retry == 1)
                    {
                        this.Invoke(new Action(delegate
                        {
                            $"覆盤量與庫存再次不符,請確認是否正確".PlayGooleVoiceAsync(Main_Form.API_Server);
                        }));

                        if (MyMessageBox.ShowDialog($"覆盤量與庫存不符,是否繼續下一步驟", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;
                    }

                }
                this.Invoke(new Action(delegate
                {
                    retry = 0;
                    rJ_TextBox_盤點量.Text = (Main_Form.PLC_Device_盤點_顯示庫存量及預帶盤點量.Bool ? 庫存.ToString() : "");
                    rJ_Lable_狀態.Text = "請輸入【覆盤】數量";
                }));
                this.sqL_DataGridView_交班藥品.ReplaceExtra(list_交班對點, true);
                cnt++;
            }
        }
        void cnt_Program_交班對點_選擇下一筆(ref int cnt)
        {
            this.Invoke(new Action(delegate
            {
                $"數量正確".PlayGooleVoiceAsync(Main_Form.API_Server);
            }));
            int selectRow = this.sqL_DataGridView_交班藥品.GetSelectRow();
            List<object[]> list_value = this.sqL_DataGridView_交班藥品.Get_All_Select_RowsValues();
            if (list_value.Count > 0)
            {
                string 藥碼 = list_value[0][(int)enum_交班藥品.藥碼].ObjectToString();
                Main_Form.Function_儲位亮燈(new Main_Form.LightOn(藥碼, Color.Black));
            }
            bool flag_全部盤點完成 = true;
            for (int i = 0; i < list_value.Count; i++)
            {
                string 盤點量 = list_value[i][(int)enum_交班藥品.盤點量].ObjectToString();
                if (盤點量.StringIsInt32() == false)
                {
                    flag_全部盤點完成 = false;
                    break;
                }

            }

            if (selectRow == this.sqL_DataGridView_交班藥品.GetAllRows().Count - 1)
            {

                if (flag_全部盤點完成 && list_value.Count > 0)
                {
                    this.stepViewer.Next();
                    this.Invoke(new Action(delegate
                    {
                        if (flag_自動彈出詢問送出報表 == false)
                        {
                            PlC_RJ_Button_確認送出_MouseDownEvent(null);
                            flag_自動彈出詢問送出報表 = true;
                        }
                    }));
                    cnt++;
                    return;
                }

            }
            this.Invoke(new Action(delegate
            {
                this.sqL_DataGridView_交班藥品.SetSelectRow(selectRow + 1);
            }));

            PLC_Device_交班對點.Bool = true;
            cnt = 1;
            return;
        }
        #endregion
        private void Function_寫入交易紀錄(string 備註)
        {
            if (personPageClass_盤點人員 == null) return;
            if (personPageClass_覆盤人員 == null) personPageClass_覆盤人員 = new personPageClass();
            List<object[]> list_交班對點 = this.sqL_DataGridView_交班藥品.GetAllRows();
            if (備註 == "交班盤點完成")
            {
                List<medRecheckLogClass> medRecheckLogClasses = new List<medRecheckLogClass>();
                for (int i = 0; i < list_交班對點.Count; i++)
                {
                    if (list_交班對點[i][(int)enum_交班藥品.差異值].ObjectToString().StringIsDouble() == false) continue;
                    if (list_交班對點[i][(int)enum_交班藥品.差異值].StringToDouble() != 0)
                    {
                        string 藥碼 = list_交班對點[i][(int)enum_交班藥品.藥碼].ObjectToString();
                        string 藥名 = list_交班對點[i][(int)enum_交班藥品.藥名].ObjectToString();
                        double 差異值 = list_交班對點[i][(int)enum_交班藥品.差異值].StringToDouble();
                        string 盤點藥師1 = personPageClass_盤點人員.姓名;
                        if (Main_Form.PLC_Device_盤點異常量直接寫入庫存.Bool == false)
                        {
                            medRecheckLogClass medRecheckLogClass = new medRecheckLogClass();
                            medRecheckLogClass.發生類別 = "交班對點";
                            medRecheckLogClass.藥碼 = 藥碼;
                            medRecheckLogClass.藥名 = 藥名;
                            medRecheckLogClass.庫存值 = list_交班對點[i][(int)enum_交班藥品.庫存].ObjectToString();
                            medRecheckLogClass.盤點值 = list_交班對點[i][(int)enum_交班藥品.盤點量].ObjectToString();
                            medRecheckLogClass.盤點藥師1 = personPageClass_盤點人員.姓名;
                            medRecheckLogClass.盤點藥師ID1 = personPageClass_盤點人員.ID;
                            medRecheckLogClass.盤點藥師2 = personPageClass_覆盤人員.姓名;
                            medRecheckLogClass.盤點藥師ID2 = personPageClass_覆盤人員.ID;
                            medRecheckLogClass.異常原因 = list_交班對點[i][(int)enum_交班藥品.收支原因].ObjectToString();

                            medRecheckLogClasses.Add(medRecheckLogClass);
                        }
                        else
                        {
                            Main_Form.Function_異常通知_盤點錯誤_庫存異動(藥碼, 藥名, 差異值, 盤點藥師1);
                            Main_Form.Function_從SQL取得儲位到本地資料();
                            Main_Form.Function_儲位刷新(藥碼);
                        }
                    }

                }
                medRecheckLogClass.add(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, medRecheckLogClasses);
            }


            List<transactionsClass> transactionsClasses_add = new List<transactionsClass>();
            List<transactionsClass> transactionsClasses_update = new List<transactionsClass>();
            List<transactionsClass> transactionsClasses_old = new List<transactionsClass>();
            for (int i = 0; i < list_交班對點.Count; i++)
            {
                string Code = list_交班對點[i][(int)enum_交班藥品.藥碼].ObjectToString();
                string op_time = list_交班對點[i][(int)enum_交班藥品.確認時間].ObjectToString();
                string Order_GUID = $"{Code}-{op_time}";
                transactionsClasses_old = transactionsClass.get_datas_by_order_guid(Main_Form.API_Server, Order_GUID, Main_Form.ServerName, Main_Form.ServerType);
          
                transactionsClass _transactionsClass = new transactionsClass();
                _transactionsClass.Order_GUID = $"{Order_GUID}";
                if (transactionsClasses_old.Count > 0) _transactionsClass = transactionsClasses_old[0];
                _transactionsClass.動作 = enum_交易記錄查詢動作.交班對點.GetEnumName();
    
                _transactionsClass.藥品碼 = Code;
                _transactionsClass.藥品名稱 = list_交班對點[i][(int)enum_交班藥品.藥名].ObjectToString();
                _transactionsClass.庫存量 = list_交班對點[i][(int)enum_交班藥品.庫存].ObjectToString();
                _transactionsClass.盤點量 = list_交班對點[i][(int)enum_交班藥品.盤點量].ObjectToString();
                _transactionsClass.操作人 = personPageClass_盤點人員.姓名;
                _transactionsClass.藥師證字號 = personPageClass_盤點人員.藥師證字號;
                _transactionsClass.覆核藥師 = personPageClass_覆盤人員.姓名;
                _transactionsClass.開方時間 = list_交班對點[i][(int)enum_交班藥品.確認時間].ObjectToString();
                _transactionsClass.操作時間 = list_交班對點[i][(int)enum_交班藥品.確認時間].ObjectToString();
                _transactionsClass.收支原因 = list_交班對點[i][(int)enum_交班藥品.收支原因].ObjectToString();
                if(_transactionsClass.收支原因.StringIsEmpty())
                {
                    _transactionsClass.收支原因 = "交班對點";
                }
                _transactionsClass.備註 = 備註;
                if (_transactionsClass.盤點量.ObjectToString().StringIsEmpty()) continue;

                if (transactionsClasses_old.Count > 0) transactionsClasses_update.Add(_transactionsClass);
                else transactionsClasses_add.Add(_transactionsClass);
            }
            if (備註 == "交班盤點完成")
            {
                transactionsClass transactionsClass = new transactionsClass();
                transactionsClass.動作 = enum_交易記錄查詢動作.交班對點.GetEnumName();
                transactionsClass.Order_GUID = guid;
                transactionsClass.開方時間 = DateTime.Now.ToDateTimeString_6();
                transactionsClass.操作時間 = DateTime.Now.ToDateTimeString_6();
                transactionsClass.備註 = 備註;
                transactionsClasses_add.Add(transactionsClass);
            }
                
            transactionsClass.add(Main_Form.API_Server, transactionsClasses_add, Main_Form.ServerName, Main_Form.ServerType);
            transactionsClass.update_by_guid(Main_Form.API_Server, transactionsClasses_update, Main_Form.ServerName, Main_Form.ServerType);
        }
        #endregion
        #region Event
        private void Dialog_交班對點_LoadFinishedEvent(EventArgs e)
        {
            List<StepEntity> list = new List<StepEntity>();
            list.Add(new StepEntity("1", "登入", 1, "請登入使用者(盤點)", eumStepState.Waiting, null));
            list.Add(new StepEntity("2", "登入", 2, "請登入使用者(覆盤)", eumStepState.Waiting, null));
            list.Add(new StepEntity("3", "藥品選擇", 3, "選擇交班藥品", eumStepState.Waiting, null));
            list.Add(new StepEntity("4", "清點藥品", 4, "清點交班藥品", eumStepState.Waiting, null));
            list.Add(new StepEntity("5", "確認送出", 5, "交班表紀錄送出", eumStepState.Waiting, null));
            this.stepViewer.CurrentStep = 1;
            this.stepViewer.ListDataSource = list;


     
            List<string> medGroupList_headers = medGroupClass.get_medGroupList_header(Main_Form.API_Server, Main_Form.ServerName);
            medGroupList_headers.Add("自選藥品");
            this.comboBox_藥品群組.DataSource = medGroupList_headers.ToArray();
            if (this.comboBox_藥品群組.Items.Count > 0)
            {
                this.comboBox_藥品群組.SelectedIndex = 0;
            }
            this.Invoke(new Action(delegate
            {
                Table table_交班藥品 = new Table(new enum_交班藥品());
                this.sqL_DataGridView_交班藥品.Init(table_交班藥品);
                this.sqL_DataGridView_交班藥品.Set_ColumnVisible(false, new enum_交班藥品().GetEnumNames());
                //this.sqL_DataGridView_交班藥品.Set_ColumnWidth(300, DataGridViewContentAlignment.MiddleCenter, enum_交班藥品.排列號);
                this.sqL_DataGridView_交班藥品.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_交班藥品.藥碼);
                this.sqL_DataGridView_交班藥品.Set_ColumnWidth(400, DataGridViewContentAlignment.MiddleLeft, enum_交班藥品.藥名);
                this.sqL_DataGridView_交班藥品.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_交班藥品.單位);
                this.sqL_DataGridView_交班藥品.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_交班藥品.盤點量);
                this.sqL_DataGridView_交班藥品.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, enum_交班藥品.差異值);
                this.sqL_DataGridView_交班藥品.ClearGrid();

                rJ_Lable_現有庫存_Title.Visible = Main_Form.PLC_Device_盤點_顯示庫存量及預帶盤點量.Bool;
                rJ_Lable_現有庫存.Visible = Main_Form.PLC_Device_盤點_顯示庫存量及預帶盤點量.Bool;
            }));



            myThread_program = new MyThread();
            myThread_program.Add_Method(sub_program);
            myThread_program.AutoRun(true);
            myThread_program.SetSleepTime(100);
            myThread_program.Trigger();


        }
        private void SqL_DataGridView_交班藥品_RowEnterEvent(object[] RowValue)
        {
            if (RowValue != null)
            {
                string 藥碼 = RowValue[(int)enum_交班藥品.藥碼].ObjectToString();
                string 藥名 = RowValue[(int)enum_交班藥品.藥名].ObjectToString();
                string 庫存 = RowValue[(int)enum_交班藥品.庫存].ObjectToString();
                this.Invoke(new Action(delegate
                {

                    List<Image> images = medPicClass.get_images_by_code(Main_Form.API_Server, 藥碼);
                    if (images == null)
                    {
                        pictureBox_藥品資訊.Image = null;
                        return;
                    }
                    pictureBox_藥品資訊.Image = images[0];
                    this.rJ_Lable_藥品資訊.Text = $"({藥碼}){藥名}";
                    this.rJ_Lable_現有庫存.Text = $"{庫存}";
                    rJ_TextBox_盤點量.Text = (Main_Form.PLC_Device_盤點_顯示庫存量及預帶盤點量.Bool ? 庫存.ToString() : "");

                }));

                if (CodeLast.StringIsEmpty() == false)
                {
                    Main_Form.Function_儲位亮燈(new Main_Form.LightOn(CodeLast, Color.Black));
                }
                Main_Form.Function_儲位亮燈(new Main_Form.LightOn(藥碼, Color.Blue));
                CodeLast = 藥碼;
            }


        }
        private void Dialog_交班對點_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.Yes)
            {
                Function_寫入交易紀錄("盤點中斷");
                list_交班對點_buf = this.sqL_DataGridView_交班藥品.GetAllRows();
                List<object[]> list_value = this.sqL_DataGridView_交班藥品.Get_All_Select_RowsValues();
                if (list_value.Count > 0)
                {
                    string 藥碼 = list_value[0][(int)enum_交班藥品.藥碼].ObjectToString();
                    Main_Form.Function_儲位亮燈(new Main_Form.LightOn(藥碼, Color.Black));
                }
            }
            else
            {
                list_交班對點_buf = null;
                Main_Form.LockClosingEvent -= Main_Form_LockClosingEvent;
            }
        }
     
        string CodeLast = "";
        private void RJ_Button_藥品群組_選擇_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                LoadingForm.ShowLoadingForm();
                string text = this.comboBox_藥品群組.GetComboBoxText();

                List<DeviceBasic> devices_buf = new List<DeviceBasic>();
                List<object[]> list_value = new List<object[]>();
                List<object[]> list_value_buf = new List<object[]>();
                List<string> list_IP = new List<string>();
                List<medClass> medClasses = new List<medClass>();
                if (text == "自選藥品")
                {
                    Dialog_藥品搜尋 dialog_藥品搜尋 = new Dialog_藥品搜尋();
                    if (dialog_藥品搜尋.ShowDialog() != DialogResult.Yes) return;
                    medClasses.Add(dialog_藥品搜尋.Value);
                }
                else
                {
                    medGroupClass _medGroupClass = medGroupClass.get_group_by_name(Main_Form.API_Server, text);
                    if (_medGroupClass == null)
                    {
                        MyMessageBox.ShowDialog("藥品群組不存在");
                        return;
                    }
                    medClasses = _medGroupClass.MedClasses;

                }

                for (int i = 0; i < medClasses.Count; i++)
                {

                    medClass medClass = medClasses[i];
                    object[] value = new object[new enum_交班藥品().GetLength()];
                    value[(int)enum_交班藥品.GUID] = medClass.GUID;
                    if (Main_Form.PLC_Device_使用藥品群組排序盤點.Bool)
                    {
                        value[(int)enum_交班藥品.排列號] = medClass.排列號;
                    }
                    else
                    {
                        value[(int)enum_交班藥品.排列號] = Main_Form.Function_從SQL取得排列號(medClass.藥品碼).ToString();
                    }

                    value[(int)enum_交班藥品.藥碼] = medClass.藥品碼;
                    value[(int)enum_交班藥品.藥名] = medClass.藥品名稱;
                    value[(int)enum_交班藥品.單位] = medClass.包裝單位;
                    int 差異值 = medRecheckLogClass.get_unresolved_qty_by_code(Main_Form.API_Server, Main_Form.ServerName, Main_Form.ServerType, medClass.藥品碼);
                    double 庫存 = Main_Form.Function_從SQL取得庫存(medClass.藥品碼);
                    value[(int)enum_交班藥品.庫存] = 差異值 + 庫存;
                    list_IP.LockAdd(Main_Form.Function_取得抽屜以藥品碼解鎖IP(medClass.藥品碼));
                    list_value.Add(value);
                }

                list_IP = (from temp in list_IP
                           select temp).Distinct().ToList();

                for (int i = 0; i < list_IP.Count; i++)
                {
                    List<string> IPs = new List<string>();
                    IPs.LockAdd(list_IP[i]);
                    Main_Form.Function_抽屜解鎖(IPs);
                    System.Threading.Thread.Sleep(100);
                }

                list_value.Sort(new ICP_交班藥品());
                this.sqL_DataGridView_交班藥品.RefreshGrid(list_value);
                this.Invoke(new Action(delegate
                {
                    this.sqL_DataGridView_交班藥品.SetSelectRow(0);
                }));
                list_交班對點_buf = this.sqL_DataGridView_交班藥品.GetAllRows();
                PLC_Device_交班對點.Bool = true;
                this.stepViewer.Next();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message);
            }
            finally
            {
                LoadingForm.CloseLoadingForm();
            }           
        }
        private void PlC_RJ_Button_盤點登入_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_使用者登入 dialog_使用者登入 = new Dialog_使用者登入();
            if (dialog_使用者登入.ShowDialog() != DialogResult.Yes) return;
            personPageClass_盤點人員 = dialog_使用者登入.personPageClass;
            this.stepViewer.Next();
            this.Invoke(new Action(delegate
            {
                rJ_Lable_盤點人員.Text = $"盤點人員 : {personPageClass_盤點人員.姓名}";
                rJ_Lable_盤點人員.BackgroundColor = Color.YellowGreen;
            }));
            if (flag_單人交班 == true)
            {
                personPageClass_覆盤人員 = dialog_使用者登入.personPageClass;
                this.stepViewer.Next();
                this.Invoke(new Action(delegate
                {
                    rJ_Lable_覆盤人員.Text = $"覆盤人員 : {personPageClass_覆盤人員.姓名}";
                    rJ_Lable_覆盤人員.BackgroundColor = Color.YellowGreen;
                }));
            }
        }
        private void PlC_RJ_Button_覆盤登入_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_使用者登入 dialog_使用者登入 = new Dialog_使用者登入(personPageClass_盤點人員.ID);
            if (dialog_使用者登入.ShowDialog() != DialogResult.Yes) return;
            personPageClass_覆盤人員 = dialog_使用者登入.personPageClass;
            this.stepViewer.Next();
            this.Invoke(new Action(delegate
            {
                rJ_Lable_覆盤人員.Text = $"覆盤人員 : {personPageClass_覆盤人員.姓名}";
                rJ_Lable_覆盤人員.BackgroundColor = Color.YellowGreen;
            }));
        }
        private void PlC_RJ_Button_確認送出_MouseDownEvent(MouseEventArgs mevent)
        {
            "是否送出交班表?".PlayGooleVoiceAsync(Main_Form.API_Server);
            if (MyMessageBox.ShowDialog("確認送出交班表?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;

            Function_寫入交易紀錄("交班盤點完成");
          
            this.DialogResult = DialogResult.Yes;
            this.Close();

        }
        private void PlC_RJ_Button_解鎖_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_交班對點 = this.sqL_DataGridView_交班藥品.Get_All_Select_RowsValues();
            if (list_交班對點.Count == 0)
            {
                MyMessageBox.ShowDialog("未選擇交班藥品");
                return;
            }
            string Code = list_交班對點[0][(int)enum_交班藥品.藥碼].ObjectToString();
            Main_Form.Function_抽屜以藥品碼解鎖(Code);
        }
        private void PlC_RJ_Button_重新盤點_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("是否重新開始盤點?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;

            this.stepViewer.CurrentStep = 1;
            this.sqL_DataGridView_交班藥品.ClearGrid();
            list_交班對點_buf = null;
        }

        private void RJ_Button_確認輸入_MouseDownEvent(MouseEventArgs mevent)
        {
            flag_確認輸入 = true;
        }
        #endregion


        public class ICP_交班藥品 : IComparer<object[]>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(object[] x, object[] y)
            {

                int 排列號0 = x[(int)enum_交班藥品.排列號].StringToInt32();
                int 排列號1 = y[(int)enum_交班藥品.排列號].StringToInt32();
                return 排列號0.CompareTo(排列號1);
            }
        }
    }
}
