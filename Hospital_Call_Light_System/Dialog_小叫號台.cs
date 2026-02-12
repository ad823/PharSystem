using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hospital_Call_Light_System
{
    public partial class Dialog_小叫號台 : Form
    {
        public delegate void FormClosingEventHandler();
        static public event FormClosingEventHandler FormClosingEvent;
        public delegate void EnrterNum01EventHandler();
        static public event EnrterNum01EventHandler EnrterNum01Event;
        public delegate void EnrterNum02EventHandler();
        static public event EnrterNum02EventHandler EnrterNum02Event;

        static Dialog_小叫號台 form = new Dialog_小叫號台();
        static public bool IsShown
        {
            get
            {
                bool temp = false;

                if (form.IsHandleCreated == false) return false;
                form.Invoke(new Action(delegate 
                {
                    temp = form.CanFocus;
                }));
                return temp;
            }
        }
        public static void ShowForm()
        {
            if (IsShown == false) form.Show();
        }
        public Dialog_小叫號台()
        {
            InitializeComponent();
        }

        private void Dialog_小叫號台_Load(object sender, EventArgs e)
        {
          
            this.FormClosed += Dialog_小叫號台_FormClosed;
            this.FormClosing += Dialog_小叫號台_FormClosing;
            this.Shown += Dialog_小叫號台_Shown;
            this.button_第一台號碼輸入.Click += Button_第一台號碼輸入_Click;
            this.button_第一台號碼輸入.TabStop = false;
            this.button_第一台號碼輸入.MouseDown += Button_第一台號碼輸入_MouseDown;
            this.button_第二台號碼輸入.Click += Button_第二台號碼輸入_Click;
            this.button_第二台號碼輸入.TabStop = false;
            this.button_第二台號碼輸入.MouseDown += Button_第二台號碼輸入_MouseDown;
            this.button1.Focus();
            this.button1.LostFocus += Button1_LostFocus;
        }

        private void Button1_LostFocus(object sender, EventArgs e)
        {
            //this.button1.Focus();
        }

        private void Dialog_小叫號台_Shown(object sender, EventArgs e)
        {
    
        }

        private void Dialog_小叫號台_FormClosing(object sender, FormClosingEventArgs e)
        {
            form.Hide();
            if (FormClosingEvent != null) FormClosingEvent();
            e.Cancel = true;
        }
        private void Dialog_小叫號台_FormClosed(object sender, FormClosedEventArgs e)
        {
         

        }

        static new public void Refresh(int 叫號號碼_1, string 叫號台1, int 叫號號碼_2, string 叫號台2)
        {
            //if (!IsShown) return;
            form.Invoke(new Action(delegate 
            {
                form.label_第一台號碼.Text = 叫號號碼_1.ToString("0000");
                form.label_第二台號碼.Text = 叫號號碼_2.ToString("0000");
                form.rJ_Lable_第一台.Text = 叫號台1;
                form.rJ_Lable_第二台.Text = 叫號台2;

            }));
        }
        private void Button_第二台號碼輸入_Click(object sender, EventArgs e)
        {
            if (EnrterNum02Event != null) EnrterNum02Event();
        }

        private void Button_第一台號碼輸入_Click(object sender, EventArgs e)
        {
            if (EnrterNum01Event != null) EnrterNum01Event();
        }

        private void Button_第二台號碼輸入_MouseDown(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Button_第一台號碼輸入_MouseDown(object sender, MouseEventArgs e)
        {

        }
    }
}
