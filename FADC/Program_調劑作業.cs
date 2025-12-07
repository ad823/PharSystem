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
        private void Program_調劑作業_Init()
        {
            this.plC_RJ_Button_調劑作業_入庫作業.MouseDownEvent += PlC_RJ_Button_調劑作業_入庫作業_MouseDownEvent;
            this.plC_RJ_Button_調劑作業_辨識登入.MouseDownEvent += PlC_RJ_Button_調劑作業_辨識登入_MouseDownEvent;
            this.plC_RJ_Button_調劑作業_指紋登入.MouseDownEvent += PlC_RJ_Button_調劑作業_指紋登入_MouseDownEvent;

            this.rJ_TextBox_調劑畫面_輸入條碼.KeyPress += RJ_TextBox_調劑畫面_輸入條碼_KeyPress;
            tabControlEx_調劑畫面.TabIndexChanged += TabControlEx_調劑畫面_TabIndexChanged;
            this.plC_UI_Init.Add_Method(Program_調劑作業);
        }

    

        private void TabControlEx_調劑畫面_TabIndexChanged(object sender, EventArgs e)
        {
            if(tabControlEx_調劑畫面.SelectedTab.Text == "刷取藥單")
            {

            }
        }
        private void Program_調劑作業()
        {
            
        }
        private void RJ_TextBox_調劑畫面_輸入條碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                List<chemotherapyOrderClass> chemotherapyOrderClasses = Function_取得醫令(this.rJ_TextBox_調劑畫面_輸入條碼.Text);

                List<DateTime> dateTimes = chemotherapyOrderClasses.GetOrderAllDates();


                flowLayoutPanel_調劑畫面_處方內容.Controls.Clear();
                flowLayoutPanel_調劑畫面_處方內容.SuspendLayout();
                foreach (DateTime date in dateTimes)
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
                    pLC_RJ_Button.Location = new System.Drawing.Point(6, 6);
                    pLC_RJ_Button.OFF_文字內容 = date.ToDateString();
                    pLC_RJ_Button.OFF_文字字體 = new System.Drawing.Font("新細明體", 14F);
                    pLC_RJ_Button.OFF_文字顏色 = System.Drawing.Color.Black;
                    pLC_RJ_Button.OFF_背景顏色 = System.Drawing.SystemColors.Control;
                    pLC_RJ_Button.ON_BorderSize = 1;
                    pLC_RJ_Button.ON_文字內容 = date.ToDateString();
                    pLC_RJ_Button.ON_文字字體 = new System.Drawing.Font("新細明體", 14F);
                    pLC_RJ_Button.ON_文字顏色 = System.Drawing.Color.Black;
                    pLC_RJ_Button.ON_背景顏色 = System.Drawing.Color.Yellow;
                    pLC_RJ_Button.ProhibitionBorderLineWidth = 1;
                    pLC_RJ_Button.ProhibitionLineWidth = 4;
                    pLC_RJ_Button.ProhibitionSymbolSize = 30;
                    pLC_RJ_Button.ShadowColor = System.Drawing.Color.DimGray;
                    pLC_RJ_Button.ShadowSize = 0;
                    pLC_RJ_Button.ShowLoadingForm = false;
                    pLC_RJ_Button.Size = new System.Drawing.Size(151, 62);
                    pLC_RJ_Button.State = false;
                    pLC_RJ_Button.TabIndex = 44;
                    pLC_RJ_Button.Text = date.ToDateString();
                    pLC_RJ_Button.TextColor = System.Drawing.Color.Black;
                    pLC_RJ_Button.TextHeight = 0;
                    pLC_RJ_Button.Texts = date.ToDateString();
                    pLC_RJ_Button.UseVisualStyleBackColor = false;
                    pLC_RJ_Button.字型鎖住 = false;
                    pLC_RJ_Button.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.交替型;
                    pLC_RJ_Button.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
                    pLC_RJ_Button.文字鎖住 = false;
                    pLC_RJ_Button.背景圖片 = null;
                    pLC_RJ_Button.讀取位元反向 = false;
                    pLC_RJ_Button.讀寫鎖住 = false;
                    pLC_RJ_Button.音效 = true;
                    pLC_RJ_Button.顯示 = false;
                    pLC_RJ_Button.顯示狀態 = false;
                    flowLayoutPanel_調劑畫面_處方內容.Controls.Add(pLC_RJ_Button);
                }
                flowLayoutPanel_調劑畫面_處方內容.ResumeLayout(false);

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
    }
}
