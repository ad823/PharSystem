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

namespace 調劑台管理系統
{
    public partial class Main_Form : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);


        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private MyTimerBasic MyTimerBasic_Keyboard = new MyTimerBasic();
        static public string keyDataString = "";

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                // 當剪貼簿更新時處理
                if (myConfigClass.鍵盤掃碼模式)
                {
                    try
                    {
                        IDataObject data = Clipboard.GetDataObject();
                        if (data != null && data.GetDataPresent(DataFormats.Text))
                        {
                            if (data.GetData(DataFormats.Text).ToString() != keyDataString)
                            {
                                keyDataString = data.GetData(DataFormats.Text).ToString();

                                if (keyDataString.Length > 2)
                                {
                                    string temp = keyDataString;
                                    if (temp.StringIsEmpty()) return;
                                    if(temp.Contains("[1]"))
                                    {
                                        temp = temp.Replace("[1]", "");
                                        byte[] byteArray = Encoding.UTF8.GetBytes(temp);
                                        Console.WriteLine($"keyDataString : {temp}");
                                        MySerialPort_Scanner01.SetReadByte(byteArray);
                                    }    
                                    else if(temp.Contains("[2]"))
                                    {
                                        temp = temp.Replace("[2]", "");
                                        byte[] byteArray = Encoding.UTF8.GetBytes(temp);
                                        Console.WriteLine($"keyDataString : {temp}");
                                        MySerialPort_Scanner02.SetReadByte(byteArray);
                                    }
                                    else if (temp.Contains("[3]"))
                                    {
                                        temp = temp.Replace("[3]", "");
                                        byte[] byteArray = Encoding.UTF8.GetBytes(temp);
                                        Console.WriteLine($"keyDataString : {temp}");
                                        MySerialPort_Scanner03.SetReadByte(byteArray);
                                    }
                                    else if (temp.Contains("[4]"))
                                    {
                                        temp = temp.Replace("[4]", "");
                                        byte[] byteArray = Encoding.UTF8.GetBytes(temp);
                                        Console.WriteLine($"keyDataString : {temp}");
                                        MySerialPort_Scanner04.SetReadByte(byteArray);
                                    }
                                    else
                                    {
                                        byte[] byteArray = Encoding.UTF8.GetBytes(temp);
                                        Console.WriteLine($"keyDataString : {temp}");
                                        MySerialPort_Scanner01.SetReadByte(byteArray);
                                    }

                                    MyTimerBasic_Keyboard.TickStop();
                                    MyTimerBasic_Keyboard.StartTickTime(200);

                                }
                            }

                        }
                    }
                    catch(Exception ex)
                    {   
                        Logger.Log(ex.Message);
                    }
                   
                }
              
            }
            base.WndProc(ref m);
        }

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
                if (myConfigClass.Scanner02_COMPort.StringIsEmpty() == false) index++;
                if (myConfigClass.Scanner03_COMPort.StringIsEmpty() == false) index++;
                if (myConfigClass.Scanner04_COMPort.StringIsEmpty() == false) index++;
                return index;
            }
        }
        private enum enum_Scanner_陣列內容
        {
            病人姓名 = 4,
            藥品碼 = 14,
            藥袋序號 = 15,
            使用數量 = 9,
            病歷號 = 10,
            開方日期 = 11,
            開方時間 = 13,
            藥品名稱 = 0,
            中文名稱 = 1,
            頻次 = 7,
            包裝單位 = 8,

        }
        private bool ScannerPortExists(string comPort, string scannerName)
        {
            if (comPort.StringIsEmpty()) return false;

            string[] portNames = System.IO.Ports.SerialPort.GetPortNames();
            bool exists = portNames.Any(temp => string.Equals(temp, comPort, StringComparison.OrdinalIgnoreCase));
            if (!exists)
            {
                Console.WriteLine($"掃碼器[{scannerName}]略過初始化, 找不到 {comPort}");
            }
            return exists;
        }

        private void InitScannerPort(MySerialPort scannerPort, string comPort, string scannerName)
        {
            if (!ScannerPortExists(comPort, scannerName)) return;

            scannerPort.Init(comPort, 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
            if (!scannerPort.IsConnected)
            {
                Console.WriteLine($"掃碼器[{scannerName}]初始化失敗: {comPort}");
            }
        }
        void Program_Scanner_RS232_Init()
        {
            AddClipboardFormatListener(this.Handle);

            MySerialPort_Scanner01.ConsoleWrite = true;
            MySerialPort_Scanner02.ConsoleWrite = true;
            MySerialPort_Scanner03.ConsoleWrite = true;
            MySerialPort_Scanner04.ConsoleWrite = true;
            if (PLC_Device_掃碼槍COM通訊.Bool)
            {
           
                if(myConfigClass.鍵盤掃碼模式 == false)
                {
                    InitScannerPort(MySerialPort_Scanner01, myConfigClass.Scanner01_COMPort, "01");
                }
                InitScannerPort(MySerialPort_Scanner02, myConfigClass.Scanner02_COMPort, "02");
                InitScannerPort(MySerialPort_Scanner03, myConfigClass.Scanner03_COMPort, "03");
                InitScannerPort(MySerialPort_Scanner04, myConfigClass.Scanner04_COMPort, "04");
            }

            plC_UI_Init.Add_Method(sub_Program_Scanner_RS232);
        }
        void sub_Program_Scanner_RS232()
        {
            if (MyTimerBasic_Keyboard.IsTimeOut())
            {
                keyDataString = "";
            }
        }
      
    }
}
