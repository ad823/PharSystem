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
using SQLUI;
using ExcelScaleLib;

namespace 中藥調劑系統
{
    public partial class Main_Form : Form
    {
        private Port.enum_unit_type enum_Unit_Type = Port.enum_unit_type.g;
        private MyThread myThread_RS232_EXCELL_SCALE_Init;
        private ExcelScaleLib.Port ExcelScaleLib_Port = new Port();
        private bool flag_EXCELL_SCALE_IS_READY = false;
        private void Program_RS232_EXCELL_SCALE_Init()
        {
            Console.WriteLine($"{DateTime.Now.ToDateTimeString()} Program_RS232_EXCELL_SCALE_Init");
            ExcelScaleLib.Communication.ConsoleWrite = false;
            Task.Run(new Action(delegate
            {
                int retry = 0;
                while (true)
                {
                    if (myConfigClass.SCALE_COMPort.StringIsEmpty())
                    {
                        Console.WriteLine($"{DateTime.Now.ToDateTimeString()} myConfigClass.SCALE_COMPort.StringIsEmpty() break");
                        break;
                    }
                    Console.WriteLine($"{DateTime.Now.ToDateTimeString()} ExcelScaleLib_Port.Init({myConfigClass.SCALE_COMPort}, 9600) ...");
                    if (ExcelScaleLib_Port.Init(myConfigClass.SCALE_COMPort, 9600))
                    {
                        myThread_RS232_EXCELL_SCALE_Init = new MyThread();
                        myThread_RS232_EXCELL_SCALE_Init.Add_Method(Program_RS232_EXCELL_SCALE);
                        myThread_RS232_EXCELL_SCALE_Init.SetSleepTime(10);
                        myThread_RS232_EXCELL_SCALE_Init.AutoRun(true);
                        myThread_RS232_EXCELL_SCALE_Init.Trigger();
                        Console.WriteLine($"{DateTime.Now.ToDateTimeString()} ExcelScaleLib_Port [{myConfigClass.SCALE_COMPort}] open sucess!");
                        break;
                    }
                    Console.WriteLine($"{DateTime.Now.ToDateTimeString()} ExcelScaleLib_Port [{myConfigClass.SCALE_COMPort}] open failed!");
                    System.Threading.Thread.Sleep(2000);
                    retry++;
                    if (retry >= 3)
                    {
                        break;
                    }
                }


            }));

        }
        private void EXCELL_set_sub_current_weight()
        {
            if (plC_CheckBox_自動歸零.Checked) flag_EXCELL_set_sub_current_weight = true;
            else flag_EXCELL_set_sub_current_weight = false;
            while (true)
            {    
                if (flag_EXCELL_set_sub_current_weight == false) return;
                System.Threading.Thread.Sleep(100);
            }
        }
        bool flag_EXCELL_set_sub_current_weight = false;
        private void Program_RS232_EXCELL_SCALE()
        {
            double? weight = null;
            if (flag_EXCELL_set_sub_current_weight && plC_CheckBox_自動歸零.Checked)
            {
                System.Threading.Thread.Sleep(100);
                int retry = 0;
                LoadingForm.ShowLoadingForm();
                while (true)
                {
                
                    if (retry > 5) break; 
                    if (ExcelScaleLib_Port.set_sub_current_weight() == true)
                    {
                        weight = ExcelScaleLib_Port.get_weight(Port.enum_unit_type.g);
                        if (weight != 0)
                        {
                            retry++;
                            continue;
                        }
                        break;
                    }
                    retry++;
                    System.Threading.Thread.Sleep(50);
                }
                LoadingForm.CloseLoadingForm();
                flag_EXCELL_set_sub_current_weight = false;
            }
   

            if (rJ_RatioButton_調劑種類_科中.Checked)
            {
                weight = ExcelScaleLib_Port.get_weight(Port.enum_unit_type.g);
            }
            if (rJ_RatioButton_調劑種類_飲片.Checked)
            {
                weight = ExcelScaleLib_Port.get_weight(Port.enum_unit_type.hh);
            }
            //if (rJ_Lable_應調單位.Text != "錢")
            //{
            //    weight = ExcelScaleLib_Port.get_weight(Port.enum_unit_type.g);
            //}
            //else
            //{
            //    weight = ExcelScaleLib_Port.get_weight(Port.enum_unit_type.hh);
            //}


            if (weight == null)
            {
                flag_EXCELL_SCALE_IS_READY = false;
                return;
            }
            else
            {
                flag_EXCELL_SCALE_IS_READY = true;
            }
            this.Invoke(new Action(delegate
            {
                double temp = (double)weight;
                rJ_Lable_實調.Text = temp.ToString("0.00");
            }));
        }
    }
}
