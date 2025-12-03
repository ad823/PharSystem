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

namespace FADC
{
    public partial class Main_Form : Form
    {
        private void Program_調劑作業_Init()
        {
            this.plC_RJ_Button_調劑作業_入庫作業.MouseDownEvent += PlC_RJ_Button_調劑作業_入庫作業_MouseDownEvent;
            this.plC_UI_Init.Add_Method(Program_調劑作業);
        }



        private void Program_調劑作業()
        {

        }

        private void PlC_RJ_Button_調劑作業_入庫作業_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_單品入庫作業 dialog_入庫作業 = new Dialog_單品入庫作業();
            dialog_入庫作業.ShowDialog();
        }
    }
}
