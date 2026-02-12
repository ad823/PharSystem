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
using System.Reflection;
using MyUI;
using Basic;
using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using SQLUI;
using H_Pannel_lib;
using HIS_DB_Lib;

namespace 勤務傳送系統
{
    public partial class Dialog_EPDPanel : Form
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

        private StorageUI_EPD_266 storageUI_EPD_266;
        private Storage storage;

        public Dialog_EPDPanel(StorageUI_EPD_266 _storageUI_EPD_266 , Storage _storage)
        {
            InitializeComponent();
            this.Load += Dialog_EPDPanel_Load;
            this.storageUI_EPD_266 = _storageUI_EPD_266;
            this.storage = _storage;

            rJ_RatioButton_背景顏色_紅.CheckedChanged += RJ_RatioButton_背景顏色_紅_CheckedChanged;
            rJ_RatioButton_背景顏色_白.CheckedChanged += RJ_RatioButton_背景顏色_白_CheckedChanged;
            rJ_RatioButton_背景顏色_黑.CheckedChanged += RJ_RatioButton_背景顏色_黑_CheckedChanged;

            rJ_RatioButton_字體顏色_紅.CheckedChanged += RJ_RatioButton_字體顏色_紅_CheckedChanged;
            rJ_RatioButton_字體顏色_白.CheckedChanged += RJ_RatioButton_字體顏色_白_CheckedChanged;
            rJ_RatioButton_字體顏色_黑.CheckedChanged += RJ_RatioButton_字體顏色_黑_CheckedChanged;

            button_字體選擇.Click += Button_字體選擇_Click;
            button_上傳至面板.Click += Button_上傳至面板_Click;
        }

        private void Dialog_EPDPanel_Load(object sender, EventArgs e)
        {
            this.epD_266_Pannel.Init(storageUI_EPD_266.GetLoacalUDP_Class());

            if (this.storage.BackColor.ToColorString() == Color.Red.ToColorString())
            {
                rJ_RatioButton_背景顏色_紅.Checked = true;
            }
            else if (this.storage.BackColor.ToColorString() == Color.White.ToColorString())
            {
                rJ_RatioButton_背景顏色_白.Checked = true;
            }
            else if (this.storage.BackColor.ToColorString() == Color.Black.ToColorString())
            {
                rJ_RatioButton_背景顏色_黑.Checked = true;
            }
            else
            {
                this.storage.BackColor = Color.White;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
            }

            if (this.storage.Name_ForeColor.ToColorString() == Color.Red.ToColorString())
            {
                rJ_RatioButton_字體顏色_紅.Checked = true;
            }
            else if (this.storage.Name_ForeColor.ToColorString() == Color.White.ToColorString())
            {
                rJ_RatioButton_字體顏色_白.Checked = true;
            }
            else if (this.storage.Name_ForeColor.ToColorString() == Color.Black.ToColorString())
            {
                rJ_RatioButton_字體顏色_黑.Checked = true;
            }
            else
            {
                this.storage.Name_ForeColor = Color.Black;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
            }

            this.fontDialog.Font = this.storage.Name_font;
            this.epD_266_Pannel.DrawToPictureBox(this.storage);

        }

        #region Event
        private void Button_上傳至面板_Click(object sender, EventArgs e)
        {
            this.storageUI_EPD_266.DrawToEpd_UDP(storage);
        }
        private void Button_字體選擇_Click(object sender, EventArgs e)
        {
            if(this.fontDialog.ShowDialog() == DialogResult.OK)
            {
                this.storage.Name_font = this.fontDialog.Font;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.epD_266_Pannel.DrawToPictureBox(this.storage);
            }
        }
        private void RJ_RatioButton_背景顏色_紅_CheckedChanged(object sender, EventArgs e)
        {
            if (rJ_RatioButton_背景顏色_紅.Checked)
            {
                this.storage.BackColor = Color.Red;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.epD_266_Pannel.DrawToPictureBox(this.storage);
            }
        }
        private void RJ_RatioButton_背景顏色_黑_CheckedChanged(object sender, EventArgs e)
        {
            if (rJ_RatioButton_背景顏色_黑.Checked)
            {
                this.storage.BackColor = Color.Black;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.epD_266_Pannel.DrawToPictureBox(this.storage);
            }
        }
        private void RJ_RatioButton_背景顏色_白_CheckedChanged(object sender, EventArgs e)
        {
            if (rJ_RatioButton_背景顏色_白.Checked)
            {
                this.storage.BackColor = Color.White;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.epD_266_Pannel.DrawToPictureBox(this.storage);
            }
        }

        private void RJ_RatioButton_字體顏色_黑_CheckedChanged(object sender, EventArgs e)
        {
            if (rJ_RatioButton_字體顏色_黑.Checked)
            {
                this.storage.Name_ForeColor = Color.Black;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.epD_266_Pannel.DrawToPictureBox(this.storage);
            }
        }
        private void RJ_RatioButton_字體顏色_白_CheckedChanged(object sender, EventArgs e)
        {
            if (rJ_RatioButton_字體顏色_白.Checked)
            {
                this.storage.Name_ForeColor = Color.White;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.epD_266_Pannel.DrawToPictureBox(this.storage);
            }
        }
        private void RJ_RatioButton_字體顏色_紅_CheckedChanged(object sender, EventArgs e)
        {
            if (rJ_RatioButton_字體顏色_紅.Checked)
            {
                this.storage.Name_ForeColor = Color.Red;
                this.storageUI_EPD_266.SQL_ReplaceStorage(storage);
                this.epD_266_Pannel.DrawToPictureBox(this.storage);
            }
        }
        #endregion
    }
}
