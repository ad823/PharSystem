using Basic;
using H_Pannel_lib;
using MinasA6DLL;
using MyUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLUI;
namespace FADC
{
  

    public partial class MainForm : Form
    {
        [EnumDescription("")]
        private enum enum_儲位管理_儲位資料
        {
            [Description("IP,VARCHAR,300,NONE")]
            IP,
            [Description("儲位名稱,VARCHAR,300,NONE")]
            儲位名稱,
            [Description("藥碼,VARCHAR,300,NONE")]
            藥碼,
            [Description("藥名,VARCHAR,300,NONE")]
            藥名,
            [Description("包裝數量,VARCHAR,300,NONE")]
            包裝數量,
            [Description("包裝單位,VARCHAR,300,NONE")]
            包裝單位,
            [Description("庫存,VARCHAR,300,NONE")]
            庫存,
            [Description("區域,VARCHAR,15,NONE")]
            區域,

        }
        private void Program_儲位管理_Init()
        {
            _storageUI_EPD_266 = this.storageUI_EPD_266;
            this.storageUI_EPD_266.InitEx(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);

            _rfiD_UI = this.rfiD_UI;
            this.rfiD_UI.Init(dBConfigClass.DB_storage.DataBaseName, dBConfigClass.DB_storage.UserName, dBConfigClass.DB_storage.Password, dBConfigClass.DB_storage.IP, dBConfigClass.DB_storage.Port, dBConfigClass.DB_storage.MySqlSslMode);



            this.sqL_DataGridView_儲位管理_儲位資料.RowsHeight = 40;
            this.sqL_DataGridView_儲位管理_儲位資料.Init(new Table(new enum_儲位管理_儲位資料()));
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnVisible(false, new enum_儲位管理_儲位資料().GetEnumNames());
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(150, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_儲位資料.IP);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_儲位資料.儲位名稱);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_儲位資料.藥碼);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(750, DataGridViewContentAlignment.MiddleLeft, enum_儲位管理_儲位資料.藥名);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_儲位資料.包裝數量);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(100, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_儲位資料.包裝單位);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(120, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_儲位資料.庫存);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnWidth(200, DataGridViewContentAlignment.MiddleCenter, enum_儲位管理_儲位資料.區域);
            this.sqL_DataGridView_儲位管理_儲位資料.Set_ColumnVisible(false, enum_儲位管理_儲位資料.區域);

            this.plC_UI_Init.Add_Method(this.Program_儲位管理);
        }
        private void Program_儲位管理()
        {

        }
    }
}
