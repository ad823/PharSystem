using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;
using System.Net;
using System.IO;
using System.Reflection;
using H_Pannel_lib;
using Basic;
using HIS_DB_Lib;
using System.Collections.Generic;
namespace batch_MQTT_Server
{
    internal class Program
    {
        static Dictionary<string, DateTime> uploadTimeRecords = new Dictionary<string, DateTime>();

        private static System.Threading.Mutex mutex;
        static public string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static private string DBConfigFileName = $"{currentDirectory}//DBConfig.txt";
        public class DBConfigClass
        {
            private string name = "MQTT_Server";
            private string api_Server = "http://127.0.0.1:4433";
            public string Api_Server { get => api_Server; set => api_Server = value; }
            public string Name { get => name; set => name = value; }
        }
        static DBConfigClass dBConfigClass = new DBConfigClass();
        static public bool LoadDBConfig()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string jsonstr = MyFileStream.LoadFileAllText($"{DBConfigFileName}");
            Console.WriteLine($"路徑 : {DBConfigFileName} 開始讀取");
            Console.WriteLine($"-------------------------------------------------------------------------");
            if (jsonstr.StringIsEmpty())
            {
                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(new DBConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{DBConfigFileName}", list_jsonstring))
                {
                    Console.WriteLine($"建立{DBConfigFileName}檔案失敗!");
                    return false;
                }
                Console.WriteLine($"未建立參數文件!請至子目錄設定{DBConfigFileName}");
                return false;
            }
            else
            {
                dBConfigClass = Basic.Net.JsonDeserializet<DBConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(dBConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{DBConfigFileName}", list_jsonstring))
                {
                    Console.WriteLine($"建立{DBConfigFileName}檔案失敗!");
                    return false;
                }

            }
            return true;

        }
        static async Task Main(string[] args)
        {
            LoadDBConfig();
            Console.Title = dBConfigClass.Name;

            mutex = new System.Threading.Mutex(true, dBConfigClass.Name);
            if (mutex.WaitOne(0, false))
            {

            }
            else
            {

                return;
            }

            Console.WriteLine("啟動 MQTT Server (.NET 5)");

            // 建立 Server 設定
            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883)
                .WithDefaultEndpointBoundIPAddress(IPAddress.Any)
                .Build();

            var mqttFactory = new MqttFactory();
            var mqttServer = mqttFactory.CreateMqttServer(options);

            mqttServer.ClientConnectedAsync += e =>
            {
                Console.WriteLine($"裝置已連線：ClientId = {e.ClientId}");
                return Task.CompletedTask;
            };

            mqttServer.ClientDisconnectedAsync += e =>
            {
                Console.WriteLine($"裝置已斷線：ClientId = {e.ClientId}");
                return Task.CompletedTask;
            };

            mqttServer.InterceptingPublishAsync += e =>
            {
                string topic = e.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Console.WriteLine($"[{e.ClientId}] 收到訊息 - 主題: {topic}");
                if( topic == "DHTSensor")
                {
                    HandleDHTSensorMessage(e.ClientId, payload);
                }
            

                return Task.CompletedTask;
            };

            await mqttServer.StartAsync();
            Console.WriteLine("MQTT Server 啟動完成，等待裝置連線中...");


            await Task.Delay(Timeout.Infinite); // ✅ 永遠不結束
        }
        static void HandleDHTSensorMessage(string clientId, string payload)
        {
            try
            {
                UDP_READ_basic uDP_READ_Basic = payload.JsonDeserializet<UDP_READ_basic>();
                DateTime now = DateTime.Now;

                if (uploadTimeRecords.ContainsKey(clientId))
                {
                    TimeSpan gap = now - uploadTimeRecords[clientId];
                    if (gap.TotalSeconds < 180)
                    {
                        Console.WriteLine($"裝置 {clientId} 未達上傳間隔（{gap.TotalSeconds:F1} 秒），跳過上傳");
                        return;
                    }
                }

                uploadTimeRecords[clientId] = now;
                temperatureClass temperatureClass = new temperatureClass();
                if (clientId == "00:E0:4C:04:B7:A1")
                {
                    temperatureClass.IP = clientId;
                    temperatureClass.溫度 = uDP_READ_Basic.dht_t.ToString();
                    temperatureClass.濕度 = uDP_READ_Basic.dht_h.ToString();
                }
                if (clientId == "00:E0:4C:04:AD:4F")
                {
                    temperatureClass.IP = clientId;
                    temperatureClass.溫度 = uDP_READ_Basic.dht_t.ToString();
                    temperatureClass.濕度 = uDP_READ_Basic.dht_h.ToString();
                }
                if (clientId == "00:E0:4C:04:B2:A5")
                {
                    temperatureClass.IP = clientId;
                    temperatureClass.溫度 = uDP_READ_Basic.dht_t.ToString();
                    temperatureClass.濕度 = uDP_READ_Basic.dht_h.ToString();
                }

                Console.WriteLine($"上傳溫濕度： 溫度 = {temperatureClass.溫度}°C，濕度 = {temperatureClass.濕度}%");
                temperatureClass.add(dBConfigClass.Api_Server, temperatureClass);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DHTSensor 訊息處理錯誤：{ex.Message}");
            }
        }
    }
}
