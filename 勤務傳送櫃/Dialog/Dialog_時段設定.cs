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
using HIS_DB_Lib;

namespace 勤務傳送系統
{
    public partial class Dialog_時段設定 : MyDialog
    {
        public TimeSpan TimeSpan_st = new TimeSpan(00, 00, 00);
        public TimeSpan TimeSpan_end = new TimeSpan(23, 59, 00);
        private string _value = "";
        public string Value
        {
            get
            {
                return lockerAccessClass.TimePeriodToString(TimeSpan_st, TimeSpan_end); 
            }
            set
            {
                if (lockerAccessClass.IsValidTimePeriodString(value) == false) return;
                lockerAccessClass.StringToTimePeriod(value, out TimeSpan_st, out TimeSpan_end);
            }
        }
        public Dialog_時段設定()
        {
            InitializeComponent();
            this.LoadFinishedEvent += Dialog_時段設定_LoadFinishedEvent;
            this.plC_RJ_Button_確認送出.MouseDownEvent += PlC_RJ_Button_確認送出_MouseDownEvent;
        }

        private void Dialog_時段設定_LoadFinishedEvent(EventArgs e)
        {
            numTextBox_st_HH.Text = TimeSpan_st.Hours.ToString();
            numTextBox_st_MM.Text = TimeSpan_st.Minutes.ToString();
            numTextBox_end_HH.Text = TimeSpan_end.Hours.ToString();
            numTextBox_end_MM.Text = TimeSpan_end.Minutes.ToString();
            this.Refresh();
        }

        private void PlC_RJ_Button_確認送出_MouseDownEvent(MouseEventArgs mevent)
        {
            int st_HH = numTextBox_st_HH.Text.StringToInt32();
            int st_MM = numTextBox_st_MM.Text.StringToInt32();
            int end_HH = numTextBox_end_HH.Text.StringToInt32();
            int end_MM = numTextBox_end_MM.Text.StringToInt32();
            if (st_HH < 0 || st_HH > 24)
            {
                MyMessageBox.ShowDialog("輸入數值不合法");
                return;
            }
            if (end_HH < 0 || end_HH > 24)
            {
                MyMessageBox.ShowDialog("輸入數值不合法");
                return;
            }

            if (st_MM < 0 || st_MM > 60)
            {
                MyMessageBox.ShowDialog("輸入數值不合法");
                return;
            }
            if (end_MM < 0 || end_MM > 60)
            {
                MyMessageBox.ShowDialog("輸入數值不合法");
                return;
            }
            TimeSpan_st = new TimeSpan(st_HH, st_MM, 00);
            TimeSpan_end = new TimeSpan(end_HH, end_MM, 00);
            string str_timespan = lockerAccessClass.TimePeriodToString(TimeSpan_st, TimeSpan_end);
            this.Close();
            this.DialogResult = DialogResult.Yes;
        }
    }
}
