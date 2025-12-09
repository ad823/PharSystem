using Basic;
using FingerprintLib;
using FpMatchLib;
using HIS_DB_Lib;
using MyOffice;
using MyPrinterlib;
using MyUI;
using SQLUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static 調劑台管理系統.Main_Form;
namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
        public enum FingerModleType
        {
            None = 0,
            fpMatchSoket,
            fingerPrint,
        }
        static public FingerModleType fingerModle = FingerModleType.fingerPrint;
        static public FpMatchSoket fpMatchSoket = new FpMatchSoket();
        static public FingerprintReader fingerprintReader = new FingerprintReader();
        static public FingerprintEngine fingerprintEngine = new FingerprintEngine();
        static public CancellationTokenSource captureCts;
        static public bool flag_指紋辨識_Init = false;
        private void Program_指紋辨識_Init()
        {
            fingerModle = (myConfigClass.使用HID指紋機) ? FingerModleType.fingerPrint : FingerModleType.fpMatchSoket;
            if(fingerModle ==  FingerModleType.fpMatchSoket)
            {
                FpMatchSoket.ConsoleWrite = true;
                Task.Run(new Action(delegate
                {
                    if (this.ControlMode == false)
                    {
                        flag_指紋辨識_Init = fpMatchSoket.Open();
                        if (flag_指紋辨識_Init)
                        {
                            this.Invoke(new Action(delegate
                            {
                                plC_RJ_Button_指紋登入.Visible = true;
                                plC_Button_人員資料_指紋註冊.Visible = true;
                            }));
                        }
                    }
                }));
            }
            else if (fingerModle == FingerModleType.fingerPrint)
            {
                Function_指紋辨識初始化(true);
                this.Invoke(new Action(delegate
                {
                    plC_RJ_Button_指紋登入.Visible = true;
                    plC_Button_人員資料_指紋註冊.Visible = true;
                }));
            }
            
         

            this.plC_UI_Init.Add_Method(Program_指紋辨識);
        }
        private void Program_指紋辨識()
        {

        }
        #region Function
        static public bool Function_指紋辨識初始化(bool show_error_message , bool openSoket = false)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(2000);
            try
            {
                if (fingerModle == FingerModleType.fpMatchSoket)
                {
                    if (Main_Form.fpMatchSoket.StateCode != stateCode.READY && Main_Form.fpMatchSoket.StateCode != stateCode.NONE || Main_Form.fpMatchSoket.IsOpen == false || openSoket == true)
                    {
                        if (Main_Form.fpMatchSoket.Open(true) == false)
                        {
                            if (show_error_message)
                            {
                                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("指紋模組未啟用", 2000);
                                dialog_AlarmForm.ShowDialog();
                            }

                            return false;
                        }
                    }

                    while (true)
                    {
                        if (Main_Form.fpMatchSoket.IsOpen == true) break;
                        if (myTimerBasic.IsTimeOut())
                        {
                            if (show_error_message)
                            {
                                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("指紋模組未啟用", 2000);
                                dialog_AlarmForm.ShowDialog();
                            }
                            return false;
                        }
                        System.Threading.Thread.Sleep(10);
                    }
                }
                else if (fingerModle == FingerModleType.fingerPrint)
                {
                    fingerprintReader.BindUI(SynchronizationContext.Current);
                    fingerprintReader.Init();
                    Console.WriteLine("fingerprintReader 初始化成功");
                }
            }
            catch
            {
                Console.WriteLine($"FingerModle({fingerModle.GetEnumName()}) 初始化失敗");
                return false;
            }


            return true;
        }
        static public bool Function_指紋辨識初始化()
        {
            return Function_指紋辨識初始化(true , true);
        }
        #endregion
    }
}
