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

        public static byte[] Function_ReadBacodeScanner_pre(int index)
        {
            if (index == 0) return Main_Form.MySerialPort_Scanner01.ReadByte();
            return null;
        }
        public static string Function_ReadBacodeScanner(int index)
        {
            if (index == 0) return Function_ReadBacodeScanner01();
            return null;
        }
        public static string Function_ReadBacodeScanner01()
        {
            try
            {
                if (MySerialPort_Scanner01.IsConnected == false) return null;
                System.Threading.Thread.Sleep(200);
                string text = MySerialPort_Scanner01.ReadString();
                if (text == null) return null;
                text = text.Replace("\0", "");
                if (text.StringIsEmpty()) return null;
                if (text.Length <= 2 || text.Length > 200) return null;

                text = text.Replace("\r\n", "");
                return text;
            }
            catch (Exception ex)
            {
                Logger.Log("error", $"Function_ReadBacodeScanner01 : {ex.Message}");
                return null;
            }
            finally
            {
                MySerialPort_Scanner01.ClearReadByte();
            }

        }
  
    }
}
