using System;
using System.Windows.Forms;
using FingerprintLib;
using System.Threading;
using Basic;
using FpMatchLib;
using MyUI;

namespace FADC
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
        private void Program_fingerMatch_Init()
        {
            try
            {
                Function_指紋辨識初始化(true);
                flag_指紋辨識_Init = true;
            }
            catch
            {
     
            }
      

            plC_UI_Init.Add_Method(sub_Program_fingerMatch);
        }
        private void sub_Program_fingerMatch()
        {

        }
        #region Function
        static public bool Function_指紋辨識初始化(bool show_error_message, bool openSoket  = true, FingerModleType fingerModleType = FingerModleType.fingerPrint)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            myTimerBasic.StartTickTime(2000);
            try
            {
                if (fingerModleType == FingerModleType.fpMatchSoket)
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
                else if(fingerModleType == FingerModleType.fingerPrint)
                {
                    fingerprintReader.BindUI(SynchronizationContext.Current);
                    fingerprintReader.Init();
                    Console.WriteLine("fingerprintReader 初始化成功");
                }
            }
            catch
            {
                Console.WriteLine($"FingerModle({fingerModleType.GetEnumName()}) 初始化失敗");
                return false;
            }
         
            
            return true;
        }
        static public bool Function_指紋辨識初始化()
        {
            return Function_指紋辨識初始化(true, true);
        }
        #endregion
    }
}
