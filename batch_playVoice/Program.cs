using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace batch_playVoice
{

    internal class Program
    {
        public static string currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        private static System.Threading.Mutex mutex;
        // ✅ 監聽的 UDP Port
        private const int ListenPort = 5200;

        // ✅ 指令 對應 音效檔路徑
        private static readonly Dictionary<string, string> SoundMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ALARM", $"{currentDirectory}\\sounds\\alarm.wav" },
                { "OK",    $"{currentDirectory}\\sounds\\ok.wav" },
                { "ERROR", $"{currentDirectory}\\sounds\\error.wav"},
            };

        private static UdpClient _udpClient;
        private static bool _isRunning = true;

        static void Main(string[] args)
        {
            Console.Title = "batch_playVoice";
            mutex = new System.Threading.Mutex(true, "batch_playVoice");
            if (mutex.WaitOne(0, false))
            {

            }
            else
            {

                return;
            }

            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                // ✅ 強制綁定 IPv4 + 4500，避免 IPv6 問題
                _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, ListenPort));
                _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                Console.WriteLine($"[OK] UDP 已成功綁定 0.0.0.0:{ListenPort}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATAL] 啟動 UDP 監聽失敗: {ex.Message}");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("[INFO] 指令範例：ALARM / OK / ERROR");
            Console.WriteLine("[INFO] 在本視窗輸入 test 會送出 OK 做本機測試");
            Console.WriteLine("[INFO] 輸入 exit 可正常關閉程式");
            Console.WriteLine();

            // ✅ 背景接收任務
            Task.Run(ListenLoop);

            // ✅ 主執行緒操作
            while (_isRunning)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    _isRunning = false;
                    break;
                }

                // ✅【test 改成送 OK】
                if (line.Equals("test", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("[TEST] 發送本機測試封包 OK → 127.0.0.1:4500");

                    using (UdpClient testUdp = new UdpClient())
                    {
                        byte[] data = Encoding.UTF8.GetBytes("OK");
                        testUdp.Send(data, data.Length, "127.0.0.1", ListenPort);
                    }

                    continue;
                }

                Console.WriteLine("可用指令：test / exit");
            }

            // ✅ 關閉資源
            try
            {
                _udpClient?.Close();
                _udpClient?.Dispose();
            }
            catch { }

            Console.WriteLine("程式已安全關閉，按任意鍵結束...");
            Console.ReadKey();
        }

        /// <summary>
        /// ✅ 背景 UDP 接收主迴圈（防呆版）
        /// </summary>
        private static async Task ListenLoop()
        {
            while (_isRunning)
            {
                try
                {
                    UdpReceiveResult result = await _udpClient.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer).Trim();

                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss}] 收到 {result.RemoteEndPoint} → \"{message}\"");

                    HandleCommand(message);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] 接收 UDP 發生例外: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// ✅ 指令處理
        /// </summary>
        private static void HandleCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;

            command = command.Trim();

            // ✅ 指令對應播放
            if (SoundMap.TryGetValue(command, out string soundPath))
            {
                Console.WriteLine($"[DEBUG] 對應到音效路徑: {soundPath}");

                if (File.Exists(soundPath))
                {
                    Console.WriteLine($"[PLAY] 播放音效: {soundPath}");
                    PlaySoundAsync(soundPath);
                    return;
                }
                else
                {
                    Console.WriteLine($"[ERROR] 找不到音效檔案: {soundPath}");
                    return;
                }
            }

            // ✅ 直接傳完整路徑播放
            if (File.Exists(command))
            {
                Console.WriteLine($"[PLAY] 播放指定檔案: {command}");
                PlaySoundAsync(command);
                return;
            }

            Console.WriteLine($"[WARN] 無法識別的指令: \"{command}\"");
        }

        /// <summary>
        /// ✅ 非同步播放（不阻塞）
        /// </summary>
        private static void PlaySoundAsync(string filePath)
        {
            Task.Run(() =>
            {
                try
                {
                    using (SoundPlayer player = new SoundPlayer(filePath))
                    {
                        player.Play(); // 非同步播放
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] 播放音效失敗: {ex.Message}");
                }
            });
        }
    }
}
