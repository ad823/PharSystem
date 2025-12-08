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
using System.Diagnostics;
using FpMatchLib;
using FingerprintLib;
using NPOI.SS.Formula.Functions;
using DPUruNet;
using FontAwesome.Sharp;

namespace FADC
{
    public partial class Main_Form : Form
    {
        private List<chemotherapyOrderClass>  chemotherapyOrders = null;
        public enum enum_處方藥品
        {
            [Description("GUID,VARCHAR,15,NONE")]
            GUID,
            [Description("序號,VARCHAR,15,NONE")]
            序號,
            [Description("藥碼,VARCHAR,15,NONE")]
            藥碼,
            [Description("藥名,VARCHAR,15,NONE")]
            藥名,
            [Description("總量,VARCHAR,15,NONE")]
            總量,
            [Description("調劑時間,VARCHAR,15,NONE")]
            調劑時間,
            [Description("調劑藥師,VARCHAR,15,NONE")]
            調劑藥師,
            [Description("審核時間,VARCHAR,15,NONE")]
            審核時間,
            [Description("審核藥師,VARCHAR,15,NONE")]
            審核藥師,
        }
        private List<PLC_RJ_Button> pLC_RJ_Buttons = new List<PLC_RJ_Button>();

        private void Program_調劑作業_Init()
        {
            this.plC_RJ_Button_調劑作業_入庫作業.MouseDownEvent += PlC_RJ_Button_調劑作業_入庫作業_MouseDownEvent;
            this.plC_RJ_Button_調劑作業_辨識登入.MouseDownEvent += PlC_RJ_Button_調劑作業_辨識登入_MouseDownEvent;
            this.plC_RJ_Button_調劑作業_指紋登入.MouseDownEvent += PlC_RJ_Button_調劑作業_指紋登入_MouseDownEvent;
            this.rJ_Button_調劑畫面_登出.MouseDownEvent += RJ_Button_調劑畫面_登出_MouseDownEvent;
            this.rJ_Button_調劑畫面_開始調劑.MouseDownEvent += RJ_Button_調劑畫面_開始調劑_MouseDownEvent;

            this.rJ_TextBox_調劑畫面_輸入條碼.KeyPress += RJ_TextBox_調劑畫面_輸入條碼_KeyPress;

            this.sqL_DataGridView_調劑畫面_處方藥品.Init(new Table(new enum_處方藥品()));
            this.sqL_DataGridView_調劑畫面_處方藥品.RowsHeight = 60;
            this.sqL_DataGridView_調劑畫面_處方藥品.Set_ColumnVisible(false, new enum_處方藥品().GetEnumNames());
            this.sqL_DataGridView_調劑畫面_處方藥品.Set_ColumnWidth(60, enum_處方藥品.序號);
            this.sqL_DataGridView_調劑畫面_處方藥品.Set_ColumnWidth(80, enum_處方藥品.藥碼);
            this.sqL_DataGridView_調劑畫面_處方藥品.Set_ColumnWidth(400, DataGridViewContentAlignment.MiddleLeft, enum_處方藥品.藥名);
            this.sqL_DataGridView_調劑畫面_處方藥品.Set_ColumnWidth(60, enum_處方藥品.總量);
            this.sqL_DataGridView_調劑畫面_處方藥品.Set_ColumnWidth(100, enum_處方藥品.調劑時間);
            this.sqL_DataGridView_調劑畫面_處方藥品.Set_ColumnWidth(80, enum_處方藥品.調劑藥師);
            this.sqL_DataGridView_調劑畫面_處方藥品.Set_ColumnWidth(100, enum_處方藥品.審核時間);
            this.sqL_DataGridView_調劑畫面_處方藥品.Set_ColumnWidth(80, enum_處方藥品.審核藥師);


            tabControlEx_調劑畫面.TabIndexChanged += TabControlEx_調劑畫面_TabIndexChanged;
            rJ_TextBox_調劑畫面_密碼.PassWordChar = true;
            rJ_TextBox_調劑畫面_密碼.KeyPress += RJ_TextBox_調劑畫面_密碼_KeyPress;
            flowLayoutPanel_調劑畫面_處方內容.SuspendLayout();
            flowLayoutPanel_調劑畫面_處方內容.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel_調劑畫面_處方內容.WrapContents = true;
            //flowLayoutPanel_調劑畫面_處方內容.AutoScroll = true;
            flowLayoutPanel_調劑畫面_處方內容.Controls.Clear();

            rJ_Button_調劑畫面_登入.MouseDownEvent += RJ_Button_調劑畫面_登入_MouseDownEvent;

            pLC_RJ_Buttons.Clear();
            for (int i = 0; i < 14; i++)
            {
                PLC_RJ_Button pLC_RJ_Button = new PLC_RJ_Button();
                pLC_RJ_Button.AutoResetState = false;
                pLC_RJ_Button.BackgroundColor = System.Drawing.SystemColors.Control;
                pLC_RJ_Button.Bool = false;
                pLC_RJ_Button.BorderColor = System.Drawing.Color.Black;
                pLC_RJ_Button.BorderRadius = 10;
                pLC_RJ_Button.BorderSize = 0;
                pLC_RJ_Button.but_press = false;
                pLC_RJ_Button.buttonType = MyUI.RJ_Button.ButtonType.Push;
                pLC_RJ_Button.DisenableColor = System.Drawing.Color.Gray;
                pLC_RJ_Button.FlatAppearance.BorderSize = 0;
                pLC_RJ_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                pLC_RJ_Button.Font = new System.Drawing.Font("新細明體", 14F);
                pLC_RJ_Button.GUID = "";
                pLC_RJ_Button.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
                pLC_RJ_Button.Image_padding = new System.Windows.Forms.Padding(0);
                pLC_RJ_Button.OFF_文字內容 = "";
                pLC_RJ_Button.OFF_文字字體 = new System.Drawing.Font("新細明體", 14F);
                pLC_RJ_Button.OFF_文字顏色 = System.Drawing.Color.Black;
                pLC_RJ_Button.OFF_背景顏色 = System.Drawing.SystemColors.Control;
                pLC_RJ_Button.ON_BorderSize = 1;
                pLC_RJ_Button.ON_文字內容 = "";
                pLC_RJ_Button.ON_文字字體 = new System.Drawing.Font("新細明體", 14F);
                pLC_RJ_Button.ON_文字顏色 = System.Drawing.Color.Black;
                pLC_RJ_Button.ON_背景顏色 = System.Drawing.Color.Yellow;
                pLC_RJ_Button.ProhibitionBorderLineWidth = 1;
                pLC_RJ_Button.ProhibitionLineWidth = 4;
                pLC_RJ_Button.ProhibitionSymbolSize = 30;
                pLC_RJ_Button.ShadowColor = System.Drawing.Color.DimGray;
                pLC_RJ_Button.ShadowSize = 0;
                pLC_RJ_Button.ShowLoadingForm = false;
                //pLC_RJ_Button.Location = new Point(6, 6);
                pLC_RJ_Button.Size = new System.Drawing.Size(140, 60);
                pLC_RJ_Button.Enabled = false;
                pLC_RJ_Button.State = false;
                pLC_RJ_Button.Text = "";
                pLC_RJ_Button.TextColor = System.Drawing.Color.Black;
                pLC_RJ_Button.TextHeight = 0;
                pLC_RJ_Button.Texts = "";
                pLC_RJ_Button.UseVisualStyleBackColor = false;
                pLC_RJ_Button.Visible = true;
                pLC_RJ_Button.字型鎖住 = false;
                pLC_RJ_Button.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.交替型;
                pLC_RJ_Button.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
                pLC_RJ_Button.文字鎖住 = false;
                pLC_RJ_Button.背景圖片 = null;
                pLC_RJ_Button.讀取位元反向 = false;
                pLC_RJ_Button.讀寫鎖住 = false;
                pLC_RJ_Button.音效 = false;
                pLC_RJ_Button.顯示 = false;
                pLC_RJ_Button.顯示狀態 = false;
                pLC_RJ_Button.MouseDownEventEx += PLC_RJ_Button_MouseDownEventEx;
                flowLayoutPanel_調劑畫面_處方內容.Controls.Add(pLC_RJ_Button);
                pLC_RJ_Buttons.Add(pLC_RJ_Button);

            }
            flowLayoutPanel_調劑畫面_處方內容.ResumeLayout(true);

            this.plC_UI_Init.Add_Method(Program_調劑作業);
        }

  
        private void Program_調劑作業()
        {
            foreach (PLC_RJ_Button pLC_RJ_Button in pLC_RJ_Buttons)
            {
                pLC_RJ_Button.Run();
            }
        }


        private void Function_調劑畫面_處方資訊更新(List<chemotherapyOrderClass> chemotherapyOrderClasses = null)
        {
            chemotherapyOrderClasses.Sort(new ICP_By_chemotherapyOrderClass_sn());
            chemotherapyOrders = chemotherapyOrderClasses;
            this.Invoke(new Action(delegate 
            {
                rJ_Lable_調劑畫面_病歷號.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].病歷號 : "-----------";
                rJ_Lable_調劑畫面_處方名稱.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].化療處方名稱 : "-----------";
                rJ_Lable_調劑畫面_處方流水號.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].化療處方流水號 : "-----------";
                rJ_Lable_調劑畫面_處方來源.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].處方來源 : "-----------";
                rJ_Lable_調劑畫面_處方階段.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].類型_化療階段 : "-----------";
                rJ_Lable_調劑畫面_頻次.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].頻次 : "-----------";
                rJ_Lable_調劑畫面_途徑.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].用藥途徑 : "-----------";
                rJ_Lable_調劑畫面_自費.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].費用別 : "-----------";
                rJ_Lable_調劑畫面_流速.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false   ? chemotherapyOrderClasses[0].流速 : "-----------";
                rJ_Lable_調劑畫面_總時間.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false  ? chemotherapyOrderClasses[0].總時間分鐘 : "-----------";
                rJ_Lable_調劑畫面_開始執行.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].開始執行日期 : "-----------";
                rJ_Lable_調劑畫面_結束執行.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses[0].結束執行日期 : "-----------";
                rJ_Lable_調劑畫面_總筆數.Text = (chemotherapyOrderClasses == null || chemotherapyOrderClasses.Count == 0) == false ? chemotherapyOrderClasses.Count.ToString() : "-----------";
            }));
      
            
   
        }

        private void TabControlEx_調劑畫面_TabIndexChanged(object sender, EventArgs e)
        {
            if(tabControlEx_調劑畫面.SelectedTab.Text == "刷取藥單")
            {

            }
        }   
        private void RJ_Button_調劑畫面_登入_MouseDownEvent(MouseEventArgs mevent)
        {
            if (rJ_TextBox_調劑畫面_帳號.Text.StringIsEmpty() == true)
            {
                MyMessageBox.ShowDialog("請輸入帳號");
                return;
            }
            string userID = rJ_TextBox_調劑畫面_帳號.Texts;
            string passWord = rJ_TextBox_調劑畫面_密碼.Texts;
            sessionClass session = sessionClass.LoginByID(Main_Form.API_Server, userID, passWord, "", Main_Form.ServerName, Main_Form.ServerType);
            if(session.Name == null)
            {
                MyMessageBox.ShowDialog("帳號或密碼錯誤");
                return;
            }
            personPageClass personPage = personPageClass.serch_by_id(Main_Form.API_Server, userID);
            if(personPage == null)
            {
                MyMessageBox.ShowDialog("找無人員資訊");
                return;
            }
            this.Invoke(new Action(delegate 
            {
                rJ_Lable_調劑畫面_登入資訊.Text = $"{personPage.姓名}({personPage.ID})";
                tabControlEx_調劑畫面.SelectTab("刷取藥單");
            }));
          
        }
        private void RJ_Button_調劑畫面_登出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                this.rJ_TextBox_調劑畫面_輸入條碼.Text = "";
                RJ_TextBox_調劑畫面_輸入條碼_KeyPress(null, new KeyPressEventArgs((char)Keys.Enter));
                string userID = rJ_TextBox_調劑畫面_帳號.Text;
                sessionClass.Logout(Main_Form.API_Server, userID);
                tabControlEx_調劑畫面.SelectTab("登入畫面");
            }));
          
        }
        private void RJ_TextBox_調劑畫面_密碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                RJ_Button_調劑畫面_登入_MouseDownEvent(null);
            }
        }
        private void RJ_TextBox_調劑畫面_輸入條碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                List<chemotherapyOrderClass> chemotherapyOrderClasses = Function_取得醫令(this.rJ_TextBox_調劑畫面_輸入條碼.Text);
                Function_調劑畫面_處方資訊更新(chemotherapyOrderClasses);
                List<DateTime> dateTimes = chemotherapyOrderClasses.GetOrderAllDates();

                foreach (PLC_RJ_Button pLC_RJ_Button in pLC_RJ_Buttons)
                {
                    pLC_RJ_Button.OFF_文字內容 = "";
                    pLC_RJ_Button.ON_文字內容 = "";
                    pLC_RJ_Button.Size = new System.Drawing.Size(140, 60);
                    pLC_RJ_Button.Text = "";
                    pLC_RJ_Button.Bool = false;
                    pLC_RJ_Button.Visible = true;
                    pLC_RJ_Button.Enabled = false;
                }
                if(chemotherapyOrderClasses.Count == 0)
                {
                    if (sender != null) MyMessageBox.ShowDialog("查無處方資訊");
                    return;
                }
                int index = 0;
                foreach (DateTime date in dateTimes)
                {
                    pLC_RJ_Buttons[index].Enabled = true;
                    pLC_RJ_Buttons[index].OFF_文字內容 = date.ToDateString();
                    pLC_RJ_Buttons[index].ON_文字內容 = date.ToDateString();
                    pLC_RJ_Buttons[index].Text = date.ToDateString();
                    index++;


                }


            }
        }
        private void RJ_Button_調劑畫面_開始調劑_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_ = sqL_DataGridView_調劑畫面_處方藥品.Get_All_Checked_RowsValuesEx();
        }

        private void PLC_RJ_Button_MouseDownEventEx(RJ_Button rJ_Button, MouseEventArgs mevent)
        {
            sqL_DataGridView_調劑畫面_處方藥品.ClearDataKeys();
            sqL_DataGridView_調劑畫面_處方藥品.ClearGrid();

            foreach (PLC_RJ_Button pLC_RJ_Button in pLC_RJ_Buttons)
            {
                pLC_RJ_Button.Bool = false;
                pLC_RJ_Button.Set_LoadState(false);
                if (rJ_Button.Text == pLC_RJ_Button.Text) pLC_RJ_Button.Bool = true;

                if (chemotherapyOrders != null)
                {
                    List<object[]> objects = new List<object[]>();
                    for (int i = 0; i < chemotherapyOrders.Count; i++)
                    {
                        object[] value = new object[new enum_處方藥品().GetLength()];

                        for (int k = 0; k < chemotherapyOrders[i].每日紀錄.Count; k++)
                        {
                            if (chemotherapyOrders[i].每日紀錄[k].日期.StringToDateTime().ToDateString() == rJ_Button.Text.StringToDateTime().ToDateString())
                            {
                                value[(int)enum_處方藥品.GUID] = chemotherapyOrders[i].GUID;
                                value[(int)enum_處方藥品.序號] = chemotherapyOrders[i].醫令順序流水號;
                                value[(int)enum_處方藥品.藥碼] = chemotherapyOrders[i].藥碼;
                                value[(int)enum_處方藥品.藥名] = chemotherapyOrders[i].藥品名稱;
                                value[(int)enum_處方藥品.總量] = chemotherapyOrders[i].總量;
                                value[(int)enum_處方藥品.調劑時間] = chemotherapyOrders[i].每日紀錄[k].調劑時間;
                                if (chemotherapyOrders[i].每日紀錄[k].調劑藥師.StringIsEmpty()) chemotherapyOrders[i].每日紀錄[k].調劑藥師 = "-";
                                value[(int)enum_處方藥品.調劑藥師] = chemotherapyOrders[i].每日紀錄[k].調劑藥師;

                                value[(int)enum_處方藥品.審核時間] = chemotherapyOrders[i].每日紀錄[k].審核時間;
                                if (chemotherapyOrders[i].每日紀錄[k].審核藥師.StringIsEmpty()) chemotherapyOrders[i].每日紀錄[k].審核藥師 = "-";
                                value[(int)enum_處方藥品.審核藥師] = chemotherapyOrders[i].每日紀錄[k].審核藥師;
                                objects.Add(value);
                            }
                       
                        }                        
                    }
                    sqL_DataGridView_調劑畫面_處方藥品.RefreshGrid(objects);

                }
            }

        }


        private void PlC_RJ_Button_調劑作業_指紋登入_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_HID指紋登入 dialog_HID = new Dialog_HID指紋登入();
            if (dialog_HID.ShowDialog() != DialogResult.Yes) return;
            personPageClass personPageClass = dialog_HID.Value;
            this.Invoke(new Action(delegate
            {
                rJ_Lable_調劑畫面_登入資訊.Text = $"{personPageClass.姓名}({personPageClass.ID})";
                tabControlEx_調劑畫面.SelectTab("刷取藥單");
            }));
        }
        private void PlC_RJ_Button_調劑作業_辨識登入_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_人臉辨識 dialog_人臉辨識 = new Dialog_人臉辨識();
            if (dialog_人臉辨識.ShowDialog() != DialogResult.Yes) return;
            personPageClass personPageClass = dialog_人臉辨識.Value;
            this.Invoke(new Action(delegate 
            {
                rJ_Lable_調劑畫面_登入資訊.Text = $"{personPageClass.姓名}({personPageClass.ID})";
                tabControlEx_調劑畫面.SelectTab("刷取藥單");
            }));
    
        }
        private void PlC_RJ_Button_調劑作業_入庫作業_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_單品入庫作業 dialog_入庫作業 = new Dialog_單品入庫作業();
            dialog_入庫作業.ShowDialog();
        }

        public class ICP_By_chemotherapyOrderClass_sn : IComparer<chemotherapyOrderClass>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(chemotherapyOrderClass x, chemotherapyOrderClass y)
            {
                int temp0 = x.醫令順序流水號.StringToInt32();
                int temp1 = y.醫令順序流水號.StringToInt32();
                int compare = temp0.CompareTo(temp1);
                return compare;

            }
        }
    }
}
