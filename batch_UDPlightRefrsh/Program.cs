using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using HIS_DB_Lib;
using H_Pannel_lib;
using Basic;

namespace batch_UDPlightRefrsh
{
    class Program
    {
        public class UDP_READ_basic
        {
            public string IP { get; set; }
            public int Port { get; set; }
            public string Version { get; set; }
            public int Input { get; set; }
            public int Output { get; set; }
            public int RSSI { get; set; }
            public int Input_dir { get; set; }
            public int Output_dir { get; set; }
            public int LaserDistance { get; set; }
            public bool WS2812_State { get; set; }
            public bool LASER_ON { get; set; }
            public float dht_h { get; set; }
            public float dht_t { get; set; }
        }
        private static System.Threading.Mutex mutex;

        static string API_Server = "http://127.0.0.1:4433";
        static UDP_Class uDP_Class_lights;
        static UDP_Class uDP_Class_lights_send;
        static UDP_Class uDP_Class_rows_led;

        // --- Log Queue 保留最後 30 筆 ---
        static Queue<string> actionLogs = new Queue<string>();
        static int maxLogs = 100;

        static void AddLog(string msg)
        {
            actionLogs.Enqueue(msg);
            if (actionLogs.Count > maxLogs) actionLogs.Dequeue();
        }

        static void Main(string[] args)
        {
            Console.Title = "batch_UDPlightRefrsh";

            mutex = new System.Threading.Mutex(true, Console.Title);
            if (mutex.WaitOne(0, false))
            {

            }
            else
            {

                return;
            }
            uDP_Class_rows_led = new UDP_Class("0.0.0.0", 30001, true);
            uDP_Class_lights = new UDP_Class("0.0.0.0", 30005, true);
            uDP_Class_lights_send = new UDP_Class("0.0.0.0", 29005, true);

            while (true)
            {
                List<medMap_sectionClass> medMap_SectionClasses = medMap_sectionClass.get_sections(API_Server);
                List<string> jsons_lights = uDP_Class_lights.List_UDP_Rx.Select(x => x[(int)UDP_Class.UDP_Rx.Readline].ObjectToString()).ToList();
                List<string> jsons_rows_led = uDP_Class_rows_led.List_UDP_Rx.Select(x => x[(int)UDP_Class.UDP_Rx.Readline].ObjectToString()).ToList();

                Dictionary<string, bool> ipLightStatus = new Dictionary<string, bool>();
                foreach (var sectionClass in medMap_SectionClasses)
                {
                    ipLightStatus[sectionClass.燈棒IP] = false;
                }

                foreach (string json in jsons_rows_led)
                {
                    if (string.IsNullOrWhiteSpace(json)) continue;

                    string ipKey = "\"IP\":\"";
                    string ip = "";
                    int ipIdx = json.IndexOf(ipKey);
                    if (ipIdx != -1)
                    {
                        int ipStart = ipIdx + ipKey.Length;
                        int ipEnd = json.IndexOf('"', ipStart);
                        ip = json.Substring(ipStart, ipEnd - ipStart);
                    }
                    if (!ip.Check_IP_Adress()) continue;

                    string key = "\"WS2812_State\":";
                    int idx = json.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                    if (idx == -1) continue;

                    int start = idx + key.Length;
                    int end = json.IndexOfAny(new char[] { ',', '}', ' ' }, start);
                    if (end == -1) end = json.Length;

                    string rawVal = json.Substring(start, end - start).Trim();
                    bool isLightOn = rawVal == "1"
                                   || rawVal.Equals("true", StringComparison.OrdinalIgnoreCase)
                                   || rawVal.Equals("\"ON\"", StringComparison.OrdinalIgnoreCase);

                    if (isLightOn)
                    {
                        var section = medMap_sectionClass.get_section_by_IP(API_Server, ip);
                        if (section != null)
                        {
                            ipLightStatus[section.燈棒IP] = true;
                        }
                    }
                }

                Console.Clear();
                Console.WriteLine("===== UDP Light Status Refresh =====");
                Console.WriteLine("Time: " + DateTime.Now.ToString("HH:mm:ss"));
                Console.WriteLine("Received lights: " + jsons_lights.Count + " rows_led: " + jsons_rows_led.Count);
                Console.WriteLine("---------------------------------------------");

                List<UDP_READ_basic> uDP_READ_Basics = new List<UDP_READ_basic>();
                foreach (string json in jsons_lights)
                {
                    UDP_READ_basic u = json.JsonDeserializet<UDP_READ_basic>();
                    if (u != null) uDP_READ_Basics.Add(u);
                }

                foreach (var kv in ipLightStatus.OrderBy(x =>
                {
                    byte[] bytes = IPAddress.Parse(x.Key).GetAddressBytes();
                    return bytes[0] * 16777216 + bytes[1] * 65536 + bytes[2] * 256 + bytes[3];
                }))
                {
                    string ip = kv.Key;
                    bool lightOn = kv.Value;

                    var udev = uDP_READ_Basics.FirstOrDefault(x => x.IP == ip);
                    if (udev == null)
                    {
                        Console.WriteLine(ip + " Not in UDP return");
                        continue;
                    }

                    bool udpLightOutput = (udev.Output != 0);

                    // *** 有不同才下指令，並記錄 ***
                    if (udpLightOutput != lightOn)
                    {
                        Communication.Set_OutputPIN(uDP_Class_lights_send, ip, 1, lightOn);

                        string log = DateTime.Now.ToString("HH:mm:ss") + " Set " + ip + " -> " + (lightOn ? "ON" : "OFF");
                        AddLog(log);
                    }

                    Console.Write(ip.PadRight(18));
                    Console.ForegroundColor = lightOn ? ConsoleColor.Green : ConsoleColor.DarkGray;
                    Console.WriteLine(lightOn ? "ON" : "OFF");
                    Console.ResetColor();
                }

                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Total devices: " + ipLightStatus.Count);

                // --- 印出連續行為 Log ---
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Action Logs:");
                foreach (var log in actionLogs)
                {
                    Console.WriteLine(log);
                }

                Thread.Sleep(200);
            }
        }
    }
}
