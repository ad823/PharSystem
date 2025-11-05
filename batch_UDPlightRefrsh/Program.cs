using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H_Pannel_lib;
using Basic;
namespace batch_UDPlightRefrsh
{
    class Program
    {
        public class UDP_READ_basic
        {
            private string iP = "0.0.0.0";
            private int port = 0;
            private string version = "";
            private int input = 0;
            private int output = 0;
            private int rSSI = -100;
            private int input_dir = 0;
            private int output_dir = 0;
            private int laserDistance = 0;
            private bool lASER_ON = false;
            private float _dht_h = 0;
            private float _dht_t = 0;

            public string IP { get => iP; set => iP = value; }
            public int Port { get => port; set => port = value; }
            public string Version { get => version; set => version = value; }
            public int Input { get => input; set => input = value; }
            public int Output { get => output; set => output = value; }
            public int RSSI { get => rSSI; set => rSSI = value; }
            public int Input_dir { get => input_dir; set => input_dir = value; }
            public int Output_dir { get => output_dir; set => output_dir = value; }
            public int LaserDistance { get => laserDistance; set => laserDistance = value; }
            public bool WS2812_State { get; set; }
            public bool LASER_ON { get => lASER_ON; set => lASER_ON = value; }
            public float dht_h { get => _dht_h; set => _dht_h = value; }
            public float dht_t { get => _dht_t; set => _dht_t = value; }

        }

        static UDP_Class uDP_Class;
        static void Main(string[] args)
        {
            uDP_Class = new UDP_Class("0.0.0.0", 30001, true);
            while (true)
            {
                List<object[]> list_udp_rx = uDP_Class.List_UDP_Rx;
                List<string> jsons = list_udp_rx.Select(x => x[(int)UDP_Class.UDP_Rx.Readline].ObjectToString()).ToList();
                List<string> light_ip = new List<string>();
                foreach(string json in jsons)
                {

                }
                Console.WriteLine($"接收到訊息共<{jsons.Count}>筆");


                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
