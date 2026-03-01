using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HIS_DB_Lib;
using Basic;

namespace batch_UPDlightCheck
{
    internal class Program
    {
        private static System.Threading.Mutex mutex;
        static string API_Server = "http://127.0.0.1:4433";

        static async Task Main(string[] args)
        {
            Console.Title = "batch_UPDlightCheck";

            mutex = new System.Threading.Mutex(true, Console.Title);
            if (mutex.WaitOne(0, false) == false) return;
          
            while (true)
            {
                returnData returnData = stockLightClass.get_stockLight_all(API_Server);
                List<stockLightClass> stockLightClasses = returnData.Data.ObjToClass<List<stockLightClass>>();
                stockLightClasses = stockLightClasses.Where (x => x.end_time.StringToDateTime() <= DateTime.Now).ToList();
                foreach (var stockLightClass in stockLightClasses)
                {
                    Console.WriteLine($"IP: {stockLightClass.ip} 已過期，開始時間: {stockLightClass.start_time} 結束時間: {stockLightClass.end_time}");
                    string command = $"ip={stockLightClass.ip};start_num={stockLightClass.start_num};end_num={stockLightClass.end_num};color=0,0,0;lightness=0.9;device_type={stockLightClass.device_type};time={10}";
                    List<string> command_arry = command.Split(';').ToList();
                    returnData returnData_light = await deviceApiClass.light_action(API_Server, command_arry);
                    Console.WriteLine($"更新結果: {returnData}");
                }
                returnData returnData_delete = stockLightClass.delete_stockLight(API_Server, stockLightClasses);

                Thread.Sleep(6000);
            }
        }
    }
}
