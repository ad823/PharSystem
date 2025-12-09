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
    public partial class Dialog_HID指紋註冊 : MyDialog
    {
        static private FingerprintReader _reader;
        static private FingerprintEngine fingerprintEngine;
        static private CancellationTokenSource _enrollCts;

        public DPUruNet.Fmd resultFmd;
        public Dialog_HID指紋註冊()
        {
            InitializeComponent();
            this.LoadFinishedEvent += Dialog_HID指紋註冊_LoadFinishedEvent;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
            _reader = Main_Form.fingerprintReader;
            _enrollCts = Main_Form.captureCts;
        }

        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            if (_enrollCts != null)
            {
                _enrollCts.Cancel();
            }
            this.Close();
        }
        async private void Dialog_HID指紋註冊_LoadFinishedEvent(EventArgs e)
        {
           
            try
            {
                if (_enrollCts != null)
                {
                    this.Invoke(new Action(delegate { rJ_Lable_state.Text = "目前已有註冊流程在執行"; }));
                    _enrollCts.Cancel();
                    _enrollCts.Dispose();
                    _enrollCts = null;
                }

                _enrollCts = new CancellationTokenSource();

                this.Invoke(new Action(delegate { rJ_Lable_state.Text = "開始註冊流程，需要多次採樣（至少 4 次）..."; }));

                // 用來記錄最後一次採樣序號
                int lastSampleIndex = 0;

                // 🔥 調用 FingerprintReader.EnrollAsync（含 callback）
                var enrollResult = await _reader.EnrollAsync(
                    minSamples: 2,     
                    maxSamples: 6,     // 最多 6 次（避免 Enrollment Not Ready）
                    token: _enrollCts.Token,
                    onSampleCaptured: (idx, bmp) =>
                    {
                        lastSampleIndex = idx;

                        // 將採樣影像放入註冊預覽 PictureBox（如有）
                        if (pbEnroll != null)
                            pbEnroll.Image = bmp;
                        this.Invoke(new Action(delegate { rJ_Lable_state.Text = $"第 {idx} 次採樣成功。"; }));

                    }
                );

                resultFmd = enrollResult.Fmd;

                string b64 = FingerprintSerializer.ToBase64(resultFmd);
                Dialog_AlarmForm dialog_AlarmForm = new Dialog_AlarmForm("註冊成功", 1000, Color.Green);
                dialog_AlarmForm.ShowDialog();
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("註冊流程已取消。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("註冊流程錯誤：" + ex.Message);
            }
            finally
            {

                _enrollCts.Dispose();
                _enrollCts = null;
            }
        }


    }
}
