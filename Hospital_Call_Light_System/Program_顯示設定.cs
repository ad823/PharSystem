using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MyUI;
using Basic;
using SQLUI;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

using System.Reflection;
using System.Runtime.InteropServices;
using HIS_DB_Lib;


namespace Hospital_Call_Light_System
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private void Program_顯示設定_Init()
        {
            this.button_顯示設定_存檔.Click += Button_顯示設定_存檔_Click;
            this.button_顯示設定_讀取.Click += Button_顯示設定_讀取_Click;
            this.button_公告名稱_重新整理.Click += Button_公告名稱_重新整理_Click;


            comboBox_一號台名稱.Click += ComboBox_一號台名稱_Click; ;
            comboBox_二號台名稱.Click += ComboBox_二號台名稱_Click;



            ComboBox_一號台名稱_Click(null, null);
            ComboBox_二號台名稱_Click(null, null);
            Button_顯示設定_讀取_Click(null, null);

            plC_UI_Init.Add_Method(Program_顯示設定);
        }

     

        private void Program_顯示設定()
        {

        }

        #region Function
        private int Function_取得一號台圖片選取()
        {
            int temp = 0;
            if (checkBox_一號台_顯示圖片01.Checked) temp = temp.SetBit(0, true);
            if (checkBox_一號台_顯示圖片02.Checked) temp = temp.SetBit(1, true);
            if (checkBox_一號台_顯示圖片03.Checked) temp = temp.SetBit(2, true);
            if (checkBox_一號台_顯示圖片04.Checked) temp = temp.SetBit(3, true);
            if (checkBox_一號台_顯示圖片05.Checked) temp = temp.SetBit(4, true);
            if (checkBox_一號台_顯示圖片06.Checked) temp = temp.SetBit(5, true);
            if (checkBox_一號台_顯示圖片07.Checked) temp = temp.SetBit(6, true);
            if (checkBox_一號台_顯示圖片08.Checked) temp = temp.SetBit(7, true);
            if (checkBox_一號台_顯示圖片09.Checked) temp = temp.SetBit(8, true);
            return temp;
        }
        private int Function_取得二號台圖片選取()
        {
            int temp = 0;
            if (checkBox_二號台_顯示圖片01.Checked) temp = temp.SetBit(0, true);
            if (checkBox_二號台_顯示圖片02.Checked) temp = temp.SetBit(1, true);
            if (checkBox_二號台_顯示圖片03.Checked) temp = temp.SetBit(2, true);
            if (checkBox_二號台_顯示圖片04.Checked) temp = temp.SetBit(3, true);
            if (checkBox_二號台_顯示圖片05.Checked) temp = temp.SetBit(4, true);
            if (checkBox_二號台_顯示圖片06.Checked) temp = temp.SetBit(5, true);
            if (checkBox_二號台_顯示圖片07.Checked) temp = temp.SetBit(6, true);
            if (checkBox_二號台_顯示圖片08.Checked) temp = temp.SetBit(7, true);
            if (checkBox_二號台_顯示圖片09.Checked) temp = temp.SetBit(8, true);
            return temp;
        }
        #endregion
        #region Event
        private void ComboBox_二號台名稱_Click(object sender, EventArgs e)
        {
            comboBox_二號台名稱.Items.Clear();
            List<string> list_str = Function_取得叫號名稱();
            for (int i = 0; i < list_str.Count; i++)
            {
                comboBox_二號台名稱.Items.Add(list_str[i]);
            }
        }
        private void ComboBox_一號台名稱_Click(object sender, EventArgs e)
        {
            comboBox_一號台名稱.Items.Clear();
            List<string> list_str = Function_取得叫號名稱();
            for (int i = 0; i < list_str.Count; i++)
            {
                comboBox_一號台名稱.Items.Add(list_str[i]);
            }

        }
        private void Button_公告名稱_重新整理_Click(object sender, EventArgs e)
        {
            List<object[]> list_value = this.sqL_DataGridView_公告設定.SQL_GetAllRows(true);
            List<string> list_名稱 = (from temp in list_value
                                    select temp[(int)enum_公告設定.名稱].ObjectToString()).ToList();
            comboBox_公告名稱.Items.Clear();
            for (int i = 0; i < list_名稱.Count; i++)
            {
                comboBox_公告名稱.Items.Add(list_名稱[i]);
            }
            comboBox_公告名稱.Items.Add("無");
            comboBox_公告名稱.SelectedIndex = 0;
        }
        private void Button_顯示設定_讀取_Click(object sender, EventArgs e)
        {
            this.comboBox_一號台名稱.Text = myConfigClass.一號台名稱;
            this.comboBox_二號台名稱.Text = myConfigClass.二號台名稱;
            radioButton_一號台_號碼.Checked = (myConfigClass.一號台_顯示方式 == enum_顯示方式.號碼);
            radioButton_一號台_圖片.Checked = (myConfigClass.一號台_顯示方式 == enum_顯示方式.圖片);
            radioButton_一號台_不顯示.Checked = (myConfigClass.一號台_顯示方式 == enum_顯示方式.不顯示);
            radioButton_二號台_號碼.Checked = (myConfigClass.二號台_顯示方式 == enum_顯示方式.號碼);
            radioButton_二號台_圖片.Checked = (myConfigClass.二號台_顯示方式 == enum_顯示方式.圖片);
            radioButton_二號台_不顯示.Checked = (myConfigClass.二號台_顯示方式 == enum_顯示方式.不顯示);
            Button_公告名稱_重新整理_Click(null, null);

            comboBox_公告名稱.Text = myConfigClass.公告名稱;

            int temp = myConfigClass.一號台_顯示圖片控制;
            checkBox_一號台_顯示圖片01.Checked = temp.GetBit(0);
            checkBox_一號台_顯示圖片02.Checked = temp.GetBit(1);
            checkBox_一號台_顯示圖片03.Checked = temp.GetBit(2);
            checkBox_一號台_顯示圖片04.Checked = temp.GetBit(3);
            checkBox_一號台_顯示圖片05.Checked = temp.GetBit(4);
            checkBox_一號台_顯示圖片06.Checked = temp.GetBit(5);
            checkBox_一號台_顯示圖片07.Checked = temp.GetBit(6);
            checkBox_一號台_顯示圖片08.Checked = temp.GetBit(7);
            checkBox_一號台_顯示圖片09.Checked = temp.GetBit(8);

            temp = myConfigClass.二號台_顯示圖片控制;
            checkBox_二號台_顯示圖片01.Checked = temp.GetBit(0);
            checkBox_二號台_顯示圖片02.Checked = temp.GetBit(1);
            checkBox_二號台_顯示圖片03.Checked = temp.GetBit(2);
            checkBox_二號台_顯示圖片04.Checked = temp.GetBit(3);
            checkBox_二號台_顯示圖片05.Checked = temp.GetBit(4);
            checkBox_二號台_顯示圖片06.Checked = temp.GetBit(5);
            checkBox_二號台_顯示圖片07.Checked = temp.GetBit(6);
            checkBox_二號台_顯示圖片08.Checked = temp.GetBit(7);
            checkBox_二號台_顯示圖片09.Checked = temp.GetBit(8);

        }
        private void Button_顯示設定_存檔_Click(object sender, EventArgs e)
        {
            myConfigClass.一號台名稱 = this.comboBox_一號台名稱.Text;
            myConfigClass.二號台名稱 = this.comboBox_二號台名稱.Text;

            myConfigClass.全局音效 = checkBox_全局音效.Checked;
            myConfigClass.推撥語音至資料庫 = checkBox_推撥語音至資料庫.Checked;
            myConfigClass.本地音效 = checkBox_本地音效.Checked;
            myConfigClass.公告名稱 = comboBox_公告名稱.Text;

            if (radioButton_一號台_號碼.Checked) myConfigClass.一號台_顯示方式 = enum_顯示方式.號碼;
            if (radioButton_一號台_圖片.Checked) myConfigClass.一號台_顯示方式 = enum_顯示方式.圖片;
            if (radioButton_一號台_不顯示.Checked) myConfigClass.一號台_顯示方式 = enum_顯示方式.不顯示;

            if (radioButton_二號台_號碼.Checked) myConfigClass.二號台_顯示方式 = enum_顯示方式.號碼;
            if (radioButton_二號台_圖片.Checked) myConfigClass.二號台_顯示方式 = enum_顯示方式.圖片;
            if (radioButton_二號台_不顯示.Checked) myConfigClass.二號台_顯示方式 = enum_顯示方式.不顯示;

            myConfigClass.一號台_顯示圖片控制 = Function_取得一號台圖片選取();

            myConfigClass.二號台_顯示圖片控制 = Function_取得二號台圖片選取();

            string jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(myConfigClass, true);
            List<string> list_jsonstring = new List<string>();
            list_jsonstring.Add(jsonstr);
            if (!MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring))
            {
                MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                return;
            }
            MyMessageBox.ShowDialog("完成!");
        }
        #endregion
    }
}
