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

            this.rJ_TextBox_調劑畫面_輸入條碼.KeyPress += RJ_TextBox_調劑畫面_輸入條碼_KeyPress;

            this.plC_UI_Init.Add_Method(Program_調劑作業);
        }

     

        private void Program_調劑作業()
        {

        }
        private void RJ_TextBox_調劑畫面_輸入條碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                List<chemotherapyOrderClass> chemotherapyOrderClasses = Function_取得醫令(this.rJ_TextBox_調劑畫面_輸入條碼.Text);
            }
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
