using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basic;
namespace 勤務傳送系統
{
    public partial class Dialog_更改病房名稱 : Form
    {
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
        private static Dialog_更改病房名稱 dialog_更改病房名稱;
        private static readonly object synRoot = new object();
        static private bool formIsCreate = false;
        public enum_Type Enum_Type = enum_Type.None;

        private string _原始名稱 = "";
        public string 原始名稱
        {
            get
            {
                return this.rJ_TextBox_原始名稱.Text;
            }
            private set
            {
                this._原始名稱 = value;
            }
        }
        private string _修改名稱 = "";
        public string 修改名稱
        {
            get
            {
                return this.rJ_TextBox_修改名稱.Text;
            }
            private set
            {
                this._修改名稱 = value;
            }
        }
        private List<string> _病房名稱 = new List<string>();
        public List<string> 病房名稱
        {
            get
            {
                _病房名稱.Clear();
                if (rJ_TextBox_病房01名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房01名稱.Text);
                if (rJ_TextBox_病房02名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房02名稱.Text);
                if (rJ_TextBox_病房03名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房03名稱.Text);
                if (rJ_TextBox_病房04名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房04名稱.Text);
                if (rJ_TextBox_病房05名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房05名稱.Text);
                if (rJ_TextBox_病房06名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房06名稱.Text);
                if (rJ_TextBox_病房07名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房07名稱.Text);
                if (rJ_TextBox_病房08名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房08名稱.Text);
                if (rJ_TextBox_病房09名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房09名稱.Text);
                if (rJ_TextBox_病房10名稱.Text.StringIsEmpty() == false) _病房名稱.Add(rJ_TextBox_病房10名稱.Text);
                _病房名稱 = (from temp in _病房名稱
                         select temp.ToUpper()).Distinct().ToList();


                return _病房名稱;
            }
            set
            {
                _病房名稱 = value;

     
            }
        }
        public enum enum_Type
        {
            OK,
            Cancel,
            None,
        }
        static public bool FormIsCreate
        {
            get
            {
                return formIsCreate;
            }
            private set
            {
                formIsCreate = value;
            }
        }

        public Dialog_更改病房名稱(string name, List<string> WardNames)
        {
            InitializeComponent();
            this.Load += Dialog_更改病房名稱_Load;
            this._原始名稱 = name;
            this._修改名稱 = name;
            if (WardNames == null) WardNames = new List<string>();
            this._病房名稱 = WardNames;
            this.rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
        }

  

        private void Dialog_更改病房名稱_Load(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate
            {

                for (int i = 0; i < _病房名稱.Count; i++)
                {
                    if (i == 0) rJ_TextBox_病房01名稱.Texts = _病房名稱[i];
                    if (i == 1) rJ_TextBox_病房02名稱.Texts = _病房名稱[i];
                    if (i == 2) rJ_TextBox_病房03名稱.Texts = _病房名稱[i];
                    if (i == 3) rJ_TextBox_病房04名稱.Texts = _病房名稱[i];
                    if (i == 4) rJ_TextBox_病房05名稱.Texts = _病房名稱[i];
                    if (i == 5) rJ_TextBox_病房06名稱.Texts = _病房名稱[i];
                    if (i == 6) rJ_TextBox_病房07名稱.Texts = _病房名稱[i];
                    if (i == 7) rJ_TextBox_病房08名稱.Texts = _病房名稱[i];
                    if (i == 8) rJ_TextBox_病房09名稱.Texts = _病房名稱[i];
                    if (i == 9) rJ_TextBox_病房10名稱.Texts = _病房名稱[i];
                }
                this.rJ_TextBox_原始名稱.Texts = _原始名稱;
                this.rJ_TextBox_修改名稱.Texts = _修改名稱;
            }));
        }
        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                Enum_Type = enum_Type.Cancel;
                this.Close();
            }));
        }
        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Enum_Type = enum_Type.OK;
                this.Close();
            }));
        }
        private void Dialog_更改病房名稱_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                Enum_Type = enum_Type.OK;
                this.Close();
            }
        }
        private void rJ_TextBox_修改名稱_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Enum_Type = enum_Type.OK;
                this.Close();
            }
        }
        private void Dialog_更改病房名稱_FormClosed(object sender, FormClosedEventArgs e)
        {
  
        }
    }
}
