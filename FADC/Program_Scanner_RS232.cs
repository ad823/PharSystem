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
using System.Runtime.InteropServices;
using NPOI.SS.Formula.Functions;

namespace FADC
{
    public partial class Main_Form : Form
    {
        static public MySerialPort MySerialPort_Scanner01 = new MySerialPort();
        static public MySerialPort MySerialPort_Scanner02 = new MySerialPort();
        static public MySerialPort MySerialPort_Scanner03 = new MySerialPort();
        static public MySerialPort MySerialPort_Scanner04 = new MySerialPort();
        static int NumOfConnectedScanner
        {
            get
            {
                int index = 0;
                if (myConfigClass.Scanner01_COMPort.StringIsEmpty() == false) index++;
                return index;
            }
        }
        private void Program_Scanner_RS232_Init()
        {

            MySerialPort_Scanner01.ConsoleWrite = true;
            MySerialPort_Scanner02.ConsoleWrite = true;
            MySerialPort_Scanner03.ConsoleWrite = true;
            MySerialPort_Scanner04.ConsoleWrite = true;
            if (!myConfigClass.Scanner01_COMPort.StringIsEmpty())
            {
                MySerialPort_Scanner01.Init(myConfigClass.Scanner01_COMPort, 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
                if (!MySerialPort_Scanner01.IsConnected)
                {
                    MyMessageBox.ShowDialog("掃碼器[01]初始化失敗!");
                }
            }
            plC_UI_Init.Add_Method(sub_Program_Scanner_RS232);
        }
        private void sub_Program_Scanner_RS232()
        {

        }
    }
}
