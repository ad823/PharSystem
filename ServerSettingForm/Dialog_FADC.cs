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
using Basic;
using HIS_DB_Lib;

namespace ServerSettingForm
{
    public partial class Dialog_FADC : Form
    {
        #region MyConfigClass
        private const string MyConfigFileName = "WebConfig.txt";
        public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {
            private string api_server = "";

            public string Api_server { get => api_server; set => api_server = value; }
        }
        private void LoadMyConfig()
        {
            string jsonstr = MyFileStream.LoadFileAllText($".//{MyConfigFileName}");
            if (jsonstr.StringIsEmpty())
            {
                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(new MyConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($".//{MyConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                }
            }
            else
            {
                myConfigClass = Basic.Net.JsonDeserializet<MyConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(myConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($".//{MyConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                }

            }


        }
        private void SaveConfig()
        {
            string jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(myConfigClass, true);
            List<string> list_jsonstring = new List<string>();
            list_jsonstring.Add(jsonstr);
            if (!MyFileStream.SaveFile($".//{MyConfigFileName}", list_jsonstring))
            {
                MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                return;
            }
        }
        #endregion

        public static Form form;
        public DialogResult ShowDialog()
        {
            if (form == null)
            {
                base.ShowDialog();
            }
            else
            {
                form.Invoke(new Action(delegate
                {
                    base.ShowDialog();
                }));
            }

            return this.DialogResult;
        }
        public Dialog_FADC()
        {
            InitializeComponent();
            this.Load += Dialog_FADC_Load;
        }

        private void Dialog_FADC_Load(object sender, EventArgs e)
        {
            this.FormClosing += Dialog_FADC_FormClosing;
            button_上傳.Click += Button_上傳_Click;
            button_新增.Click += Button_新增_Click;
            button_測試.Click += Button_測試_Click;
            button_刪除.Click += Button_刪除_Click;
            button_讀取.Click += Button_讀取_Click;

            this.LoadMyConfig();

            if (myConfigClass != null)
            {
                rJ_TextBox_API_Server.Texts = myConfigClass.Api_server;
            }
            Button_測試_Click(null, null);
        }
        #region Function
        private void Function_Add(string Name)
        {
            string json_result = Basic.Net.WEBApiGet($"{myConfigClass.Api_server}/api/serversetting");
            if (json_result.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("讀取失敗!");
                return;
            }
            returnData returnData = json_result.JsonDeserializet<returnData>();
            List<sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind(Name, enum_sys_serverSetting_Type.FADC);
            Panel_SQLContent.SetValue(this.FindForm(), Name, enum_sys_serverSetting_Type.FADC);
            Panel_SQLContent.SaveAll(this.FindForm(), ref sys_serverSettingClasses);
            Panel_API_URL.SetValue(this.FindForm(), Name, enum_sys_serverSetting_Type.FADC);
            Panel_API_URL.SaveAll(this.FindForm(), ref sys_serverSettingClasses);
            Panel_CheckBox.SetValue(this.FindForm(), enum_sys_serverSetting_Type.FADC);
            Panel_CheckBox.SaveAll(this.FindForm(), ref sys_serverSettingClasses);

            sys_serverSettingClasses.Set_department_type(comboBox_單位.Text);

            returnData.Data = sys_serverSettingClasses;
            string json_in = returnData.JsonSerializationt(true);
            Console.WriteLine(json_in);
            json_result = Basic.Net.WEBApiPostJson($"{myConfigClass.Api_server}/api/serversetting/add", json_in);
            Console.WriteLine(json_result);
        }
        #endregion
        #region Event
        private void Dialog_FADC_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        private void Button_測試_Click(object sender, EventArgs e)
        {
            myConfigClass.Api_server = rJ_TextBox_API_Server.Text;
            string json_result = Basic.Net.WEBApiGet($"{myConfigClass.Api_server}/api/serversetting");
            if (json_result.StringIsEmpty())
            {
                if (sender != null) MyMessageBox.ShowDialog("測試失敗!");
                return;
            }
            Console.WriteLine($"{json_result}");
            SaveConfig();
            returnData returnData = json_result.JsonDeserializet<returnData>();
            List<sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();
            sys_serverSettingClasses = sys_serverSettingClassMethod.MyFind(sys_serverSettingClasses, enum_sys_serverSetting_Type.FADC);
            comboBox_名稱.Items.Clear();

            for (int i = 0; i < sys_serverSettingClasses.Count; i++)
            {
                if (!comboBox_名稱.Items.Contains(sys_serverSettingClasses[i].設備名稱))
                {
                    if (!sys_serverSettingClasses[i].設備名稱.StringIsEmpty()) comboBox_名稱.Items.Add(sys_serverSettingClasses[i].設備名稱);
                }
            }
            if (comboBox_名稱.Items.Count > 0) comboBox_名稱.SelectedIndex = 0;

            if (returnData.Code == 200)
            {
                if (sender != null) MyMessageBox.ShowDialog("測試成功!");
            }
        }
        private void Button_讀取_Click(object sender, EventArgs e)
        {
            string json_result = Basic.Net.WEBApiGet($"{myConfigClass.Api_server}/api/serversetting");
            if (json_result.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("讀取失敗!");
                return;
            }
            returnData returnData = json_result.JsonDeserializet<returnData>();
            List<sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();
            sys_serverSettingClasses = sys_serverSettingClasses.MyFind(enum_sys_serverSetting_Type.FADC);
            Panel_SQLContent.SetValue(this.FindForm(), comboBox_名稱.Text, enum_sys_serverSetting_Type.FADC);
            Panel_SQLContent.LoadAll(this.FindForm(), sys_serverSettingClasses);

            Panel_API_URL.SetValue(this.FindForm(), comboBox_名稱.Text, enum_sys_serverSetting_Type.FADC);
            Panel_API_URL.LoadAll(this.FindForm(), sys_serverSettingClasses);

            Panel_CheckBox.SetValue(this.FindForm(), enum_sys_serverSetting_Type.FADC);
            Panel_CheckBox.LoadAll(this.FindForm(), sys_serverSettingClasses);

            this.comboBox_單位.DataSource = sys_serverSettingClasses.Get_department_types();
            sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses.myFind(comboBox_名稱.Text, enum_sys_serverSetting_Type.FADC.GetEnumName(), "一般資料");
            if (sys_serverSettingClass.單位 == null) sys_serverSettingClass.單位 = "";
            this.comboBox_單位.Text = sys_serverSettingClass.單位;
        }
        private void Button_刪除_Click(object sender, EventArgs e)
        {
            if (MyMessageBox.ShowDialog("確認刪除?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;

            string json_result = Basic.Net.WEBApiGet($"{myConfigClass.Api_server}/api/serversetting");
            returnData returnData = json_result.JsonDeserializet<returnData>();
            List<sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();
            sys_serverSettingClasses = (from value in sys_serverSettingClasses
                                        where value.類別 == enum_sys_serverSetting_Type.FADC.GetEnumName()
                                        where value.設備名稱 == comboBox_名稱.Text
                                        select value).ToList();
            returnData.Data = sys_serverSettingClasses;
            string json_in = returnData.JsonSerializationt(true);
            Console.WriteLine(json_in);
            json_result = Basic.Net.WEBApiPostJson($"{myConfigClass.Api_server}/api/serversetting/delete", json_in);
            Console.WriteLine(json_result);
            MyMessageBox.ShowDialog("完成!");

        }
        private void Button_新增_Click(object sender, EventArgs e)
        {
            Dialog_新增 dialog_新增 = new Dialog_新增("請輸入FADC名稱...");
            dialog_新增.ShowDialog();
            if (dialog_新增.DialogResult != DialogResult.Yes) return;
            Function_Add(dialog_新增.Value);

            string json_result = Basic.Net.WEBApiGet($"{myConfigClass.Api_server}/api/serversetting");
            returnData returnData = json_result.JsonDeserializet<returnData>();
            List<sys_serverSettingClass> sys_serverSettingClasses = returnData.Data.ObjToListClass<sys_serverSettingClass>();

            sys_serverSettingClasses = (from value in sys_serverSettingClasses
                                        where value.類別 == enum_sys_serverSetting_Type.FADC.GetEnumName()
                                        select value).ToList();
            comboBox_名稱.Items.Clear();

            for (int i = 0; i < sys_serverSettingClasses.Count; i++)
            {
                if (!comboBox_名稱.Items.Contains(sys_serverSettingClasses[i].設備名稱))
                {
                    comboBox_名稱.Items.Add(sys_serverSettingClasses[i].設備名稱);
                }
            }
            if (comboBox_名稱.Items.Count > 0) comboBox_名稱.SelectedIndex = 0;
        }
        private void Button_上傳_Click(object sender, EventArgs e)
        {
            Function_Add(comboBox_名稱.Text);
            MyMessageBox.ShowDialog("完成!");
        }

        #endregion
    }
}
