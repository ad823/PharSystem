using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS_DB_Lib;
using Basic;
using MyUI;
using FingerprintLib;
using System.Threading;

namespace 調劑台管理系統
{
    public partial class Dialog_HID指紋登入 : MyDialog
    {
        static private FingerprintReader _reader;
        static private FingerprintEngine fingerprintEngine;
        static private CancellationTokenSource _enrollCts;
        public personPageClass Value = null;
        public DPUruNet.Fmd resultFmd;
        public Dialog_HID指紋登入()
        {
            InitializeComponent();
            this.LoadFinishedEvent += Dialog_HID指紋登入_LoadFinishedEvent; ;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
            _reader = Main_Form.fingerprintReader;
            fingerprintEngine = Main_Form.fingerprintEngine;
            _enrollCts = Main_Form.captureCts;
        }

        async private void Dialog_HID指紋登入_LoadFinishedEvent(EventArgs e)
        {
            try
            {
                if (_enrollCts != null)
                {
                    this.Invoke(new Action(delegate { rJ_Lable_state.Text = "目前已有流程在執行"; }));
                    _enrollCts.Cancel();
                    _enrollCts.Dispose();
                    _enrollCts = null;
                }

                this.Invoke(new Action(delegate { rJ_Lable_state.Text = "請將手指放上指紋機"; }));

                Dialog_AlarmForm alarmForm;
                _enrollCts = new CancellationTokenSource();
                var result = await _reader.CaptureAsync(_enrollCts.Token);
                DPUruNet.Fmd fingersrc = resultFmd = result.Fmd;
                pbFinger1.Image = result.Bitmap;

                string b64 = FingerprintSerializer.ToBase64(fingersrc);

                List<personPageClass> personPageClasses = personPageClass.get_all(Main_Form.API_Server);

                foreach (var personPageClass in personPageClasses)
                {
                    if (personPageClass.指紋辨識.StringIsEmpty()) continue;
                    DPUruNet.Fmd fingerdst = null;
                    try
                    {
                        fingerdst = FingerprintSerializer.FromBase64(personPageClass.指紋辨識);
                    }
                    catch
                    {
                        continue;
                    }
                    bool match = fingerprintEngine.Compare(fingersrc, fingerdst);
                    if (match == false) continue;
                    Value = personPageClass;
                    alarmForm = new Dialog_AlarmForm($"【{Value.姓名}】登入成功", 1500, Color.Green);
                    alarmForm.ShowDialog();
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                    return;
                }
                if (_enrollCts != null)
                {
                    _enrollCts.Cancel();
                    _enrollCts.Dispose();
                    _enrollCts = null;
                }
                alarmForm = new Dialog_AlarmForm($"找無匹配指紋", 1500, Color.Red);
                alarmForm.ShowDialog();
                Dialog_HID指紋登入_LoadFinishedEvent(null);
            }
            catch(Exception ex) 
            {

            }
           



        }

        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            if (_enrollCts != null)
            {
                _enrollCts.Cancel();
            }
            this.Close();
        }
    }
}
