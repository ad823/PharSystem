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
using System.Diagnostics;//記得取用 FileVersionInfo繼承
using System.Reflection;//記得取用 Assembly繼承
using HIS_DB_Lib;
using SQLUI;

namespace 調劑台管理系統
{
    public partial class Dialog_時段設定 : MyDialog
    {
        private string ID = "";
        public Dialog_時段設定(string ID)
        {
            InitializeComponent();
            this.LoadFinishedEvent += Dialog_時段設定_LoadFinishedEvent;
            this.rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
            this.ID = ID;
        }

        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Close();
        }

        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            personTimePeriodClass personTimePeriod = new personTimePeriodClass();
            personTimePeriod.ID = ID;
            personTimePeriod.start_date = rJ_TextBox_起始日期.Texts;
            personTimePeriod.end_date = rJ_TextBox_結束日期.Texts;
            personTimePeriod.period1 = rJ_TextBox_操作時段_1.Texts;
            personTimePeriod.period2 = rJ_TextBox_操作時段_2.Texts;
            personTimePeriod.period3 = rJ_TextBox_操作時段_3.Texts;

            if (personTimePeriod.start_date.Check_Date_String() == false)
            {
                MyMessageBox.ShowDialog("輸入日期不合法");
                return;
            }
            if (personTimePeriod.end_date.Check_Date_String() == false)
            {
                MyMessageBox.ShowDialog("輸入日期不合法");
                return;
            }
            if (IsValidPeriod(personTimePeriod.period1) == false && personTimePeriod.period1.StringIsEmpty() == false)
            {
                MyMessageBox.ShowDialog("輸入時段不合法");
                return;
            }
            if (IsValidPeriod(personTimePeriod.period2) == false && personTimePeriod.period2.StringIsEmpty() == false)
            {
                MyMessageBox.ShowDialog("輸入時段不合法");
                return;
            }
            if (IsValidPeriod(personTimePeriod.period3) == false && personTimePeriod.period3.StringIsEmpty() == false)
            {
                MyMessageBox.ShowDialog("輸入時段不合法");
                return;
            }

            personPageClass.add_person_time_period(Main_Form.API_Server, personTimePeriod);
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void Dialog_時段設定_LoadFinishedEvent(EventArgs e)
        {
            personTimePeriodClass personTimePeriod = null;
            List<personTimePeriodClass> personTimes = personPageClass.get_person_time_period(Main_Form.API_Server);
            if(personTimes != null)
            {
                personTimePeriod = personTimes.Where(x => x.ID == ID).FirstOrDefault();
            }
            if (personTimePeriod != null)
            {
                rJ_TextBox_起始日期.Texts = personTimePeriod.start_date.StringToDateTime().ToDateString();
                rJ_TextBox_結束日期.Texts = personTimePeriod.end_date.StringToDateTime().ToDateString();
                rJ_TextBox_操作時段_1.Texts = personTimePeriod.period1;
                rJ_TextBox_操作時段_2.Texts = personTimePeriod.period2;
                rJ_TextBox_操作時段_3.Texts = personTimePeriod.period3;

            }
        }


        /// <summary>
        /// 檢查時段字串是否合法（格式：HHmm-HHmm）
        /// </summary>
        public static bool IsValidPeriod(string period)
        {
            // 空白表示未設定，視為合法
            if (string.IsNullOrWhiteSpace(period)) return true;

            // 格式必須是 4碼-4碼
            // 例如：1200-1800
            var parts = period.Split('-');
            if (parts.Length != 2) return false;

            string start = parts[0];
            string end = parts[1];

            // 必須都是 4 位數字
            if (start.Length != 4 || end.Length != 4) return false;
            if (!int.TryParse(start, out int s) || !int.TryParse(end, out int e))
                return false;

            // 解析時間
            int sh = s / 100; // 時
            int sm = s % 100; // 分
            int eh = e / 100;
            int em = e % 100;

            // 時間是否合法（0–23 小時，0–59 分）
            if (sh < 0 || sh > 23 || eh < 0 || eh > 23) return false;
            if (sm < 0 || sm > 59 || em < 0 || em > 59) return false;

            // 起始 < 結束
            TimeSpan tsStart = new TimeSpan(sh, sm, 0);
            TimeSpan tsEnd = new TimeSpan(eh, em, 0);

            return tsStart < tsEnd;
        }
    }
}
