using System;
using System.Net.Sockets;
using System.Text;

namespace UdpVoiceTestSender
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "UDP 語音播放測試發送端 (.NET Framework 4.7.2)";

            Console.Write("請輸入目標 IP (預設 127.0.0.1)：");
            string ip = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ip))
                ip = "127.0.0.1";

            int port = 5200;

            Console.WriteLine();
            Console.WriteLine("輸入指令即可發送：");
            Console.WriteLine(" ALARM  → 播放警告音");
            Console.WriteLine(" OK     → 播放成功音");
            Console.WriteLine(" ERROR  → 播放錯誤音");
            Console.WriteLine(" exit   → 離開程式");
            Console.WriteLine();

            while (true)
            {
                Console.Write("指令 > ");
                string command = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(command))
                    continue;

                if (command.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                SendPlayVoiceCommand(ip, port, command);
            }

            Console.WriteLine("程式結束，按任意鍵關閉...");
            Console.ReadKey();
        }

        /// <summary>
        /// 傳送 UDP 指令給播放器
        /// </summary>
        private static void SendPlayVoiceCommand(string targetIP, int port, string command)
        {
            try
            {
                using (UdpClient udp = new UdpClient())
                {
                    byte[] data = Encoding.UTF8.GetBytes(command);

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 發送中...");
                    Console.WriteLine($"  → 目標 IP   : {targetIP}");
                    Console.WriteLine($"  → 目標 Port : {port}");
                    Console.WriteLine($"  → 指令內容 : {command}");

                    int sendLen = udp.Send(data, data.Length, targetIP, port);


                    Console.WriteLine($"[SUCCESS] 傳送成功，共 {sendLen} bytes");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] 傳送失敗！");
                Console.WriteLine($"  錯誤訊息: {ex.Message}");
                Console.WriteLine();
            }
        }
    }
}
