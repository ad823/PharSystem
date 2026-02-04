using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basic;
using HIS_DB_Lib;
namespace 勤務傳送系統
{
    public partial class OpenDoorPermission_UI : UserControl
    {
        public static string LoginID = "";
        private string wardName = "病房名稱";
        public string WardName
        {
            get
            {
                return wardName;
            }
            set
            {
                this.wardName = value;
                this.Invoke(new Action(delegate
                {
                    this.label_wardname.Text = wardName;
                }));
            }
        }
        private bool permission = false;
        public bool Permission
        {
            get
            {
                return this.permission;
            }
            set
            {
                this.Invoke(new Action(delegate
                {
                    this.permission = value;
                    if(permission)
                    {
                        this.label_wardname.BackColor = Color.YellowGreen;
                    }
                    else
                    {
                        this.label_wardname.BackColor = Color.GhostWhite;
                    }
                }));
            }
        }


        public OpenDoorPermission_UI()
        {
            InitializeComponent();
            
            this.Click += OpenDoorPermission_UI_Click;
            this.label_wardname.Click += OpenDoorPermission_UI_Click;
            this.button_setting.Click += Button_setting_Click;
        }

        private void Button_setting_Click(object sender, EventArgs e)
        {
            if (LoginID.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未選取資料");
                return;
            }
            Dialog_時段設定 dialog_時段設定 = new Dialog_時段設定();
            lockerAccessClass lockerAccessClass = lockerAccessClass.get_by_id_and_lcname(Main_Form.API_Server, LoginID, wardName);
            if(lockerAccessClass != null)
            {
                dialog_時段設定.Value = lockerAccessClass.鎖控可開啟時段;
            }
            else
            {
                lockerAccessClass = new lockerAccessClass();         
            }
            lockerAccessClass.ID = LoginID;
            lockerAccessClass.鎖控名稱 = wardName;
            if (dialog_時段設定.ShowDialog() != DialogResult.Yes) return;

            lockerAccessClass.鎖控可開啟時段 = dialog_時段設定.Value;

            if (lockerAccessClass.IsTimeInPeriod(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 00), lockerAccessClass.鎖控可開啟時段) == false)
            {
             
            }
            else
            {
               
            }

            lockerAccessClass.add(Main_Form.API_Server, lockerAccessClass);


        }
        private void OpenDoorPermission_UI_Click(object sender, EventArgs e)
        {
            Permission = !Permission; // 切換 Permission 狀態
        }

        private void OpenDoorPermission_UI_Load(object sender, EventArgs e)
        {
            this.Refresh();
        }


    }

    static public class OpenDoorPermissionMethod
    {
        static private MyConvert myConvert = new MyConvert();
        static public bool GetOpenDoorPermission(string value, List<string> wardNames)
        {
            List<string> list_value = value.JsonDeserializet<List<string>>();
            if (list_value == null) return false;
            for (int i = 0; i < list_value.Count; i++)
            {
                for(int k = 0; k < wardNames.Count; k++)
                {
                    if (list_value[i] == wardNames[k]) return true;
                }
              
            }
            return false;
        }
        static public bool GetOpenDoorPermission(string value, string wardName)
        {
            List<string> list_value = value.JsonDeserializet<List<string>>();
            if (list_value == null) return false;
            for (int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i] == wardName) return true;
            }
            return false;
        }
        static public string GetOpenDoorPermission(this List<OpenDoorPermission_UI> openDoorPermission_UIs)
        {
            List<string> list_value = new List<string>();
            for (int i = 0; i < openDoorPermission_UIs.Count; i++)
            {
                if(openDoorPermission_UIs[i].Permission)
                {
                    if (openDoorPermission_UIs[i].WardName.StringIsEmpty() == false)
                    {
                        list_value.Add(openDoorPermission_UIs[i].WardName);
                    }
                }
            }

            return list_value.JsonSerializationt();
        }
        static public void SetOpenDoorPermission(this List<OpenDoorPermission_UI> openDoorPermission_UIs, string value)
        {
            List<string> list_value = value.JsonDeserializet<List<string>>();
     
            if (list_value == null)
            {
                for (int k = 0; k < openDoorPermission_UIs.Count; k++)
                {
                    openDoorPermission_UIs[k].Permission = false;
                }
                return;
            }
            SetOpenDoorPermission(openDoorPermission_UIs, list_value);
        }
        static public void SetOpenDoorPermission(this List<OpenDoorPermission_UI> openDoorPermission_UIs , List<string> list_value)
        {
            for (int k = 0; k < openDoorPermission_UIs.Count; k++)
            {
                openDoorPermission_UIs[k].Permission = false;
            }
            List<OpenDoorPermission_UI> openDoorPermission_UIs_buf = new List<OpenDoorPermission_UI>();
            for (int i = 0; i < list_value.Count; i++)
            {
                openDoorPermission_UIs_buf = (from temp in openDoorPermission_UIs
                                              where temp.WardName == list_value[i]
                                              select temp).ToList();
                for (int k = 0; k< openDoorPermission_UIs_buf.Count; k++)
                {
                    openDoorPermission_UIs_buf[k].Permission = true;
                }
            }
        }
    
    }
}
