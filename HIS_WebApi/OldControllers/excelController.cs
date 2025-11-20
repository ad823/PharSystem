using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Configuration;
using MyOffice;
using NPOI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class excelController : Controller
    {
        public class class_emg_apply
        {
            [JsonPropertyName("cost_center")]
            public string 成本中心 { get; set; }
            [JsonPropertyName("code")]
            public string 藥品碼 { get; set; }     
            [JsonPropertyName("name")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("value")]
            public string 撥出量 { get; set; }

        }
        //[HttpPost]
        //public string Post([FromBody] class_medicine_page_firstclass_data data)

        [Route("emg_apply")]
        [HttpPost]
        public string Get_emg_apply([FromBody] List<class_emg_apply> class_Emg_Applies)
        {
            string json = "";
            int row_max = 50;
            string loadText = Basic.MyFileStream.LoadFileAllText(@"C:\excel.txt", "utf-8");
            List<SheetClass> sheetClasses = new List<SheetClass>();
            List<class_emg_apply> class_Emg_Applies_Distinct = new List<class_emg_apply>();
            List<class_emg_apply> class_Emg_Applies_buf = new List<class_emg_apply>();
            int NumOfRow = 0;
            int page_num = 0;

            SheetClass sheetClass = loadText.JsonDeserializet<SheetClass>();
            sheetClass.ReplaceCell(1, 2, $"{DateTime.Now.ToDateString()}");
            sheetClasses.Add(sheetClass);
            class_Emg_Applies_Distinct = class_Emg_Applies.Distinct(new Distinct_class_Emg_Applies()).ToList();
            for(int i = 0; i < class_Emg_Applies_Distinct.Count; i++)
            {
                if (NumOfRow >= row_max)
                {
                    sheetClass = loadText.JsonDeserializet<SheetClass>();
                    sheetClass.ReplaceCell(1, 2, $"{DateTime.Now.ToDateString()}");
                    sheetClasses.Add(sheetClass);
                    NumOfRow = 0;
                }

                class_Emg_Applies_buf = (from value in class_Emg_Applies
                                         where value.藥品碼 == class_Emg_Applies_Distinct[i].藥品碼
                                         select value).ToList();
                int 總撥出量 = 0;
                for (int k = 0; k < class_Emg_Applies_buf.Count; k++)
                {
                    string 成本中心 = class_Emg_Applies_buf[k].成本中心;
                    string 藥品碼 = class_Emg_Applies_buf[k].藥品碼;
                    string 藥名 = class_Emg_Applies_buf[k].藥品名稱;
                    string 撥出量 = class_Emg_Applies_buf[k].撥出量;
                    總撥出量 += 撥出量.StringToInt32();
                    sheetClass.AddNewCell_Webapi(NumOfRow + 3, 0, $"{成本中心}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(NumOfRow + 3, 1, $"{藥品碼}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(NumOfRow + 3, 2, $"{藥名}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    sheetClass.AddNewCell_Webapi(NumOfRow + 3, 3, $"{撥出量}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    NumOfRow++;
                }

                sheetClass.AddNewCell_Webapi(NumOfRow + 3, NumOfRow + 3, 0, 3, $"總量 : {總撥出量}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Right, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.None);
                NumOfRow++;
            }
            json = sheetClasses.JsonSerializationt();
            return json;
        }

        [Route("test")]
        [HttpGet]
        public HttpResponseMessage Get_test()
        {
            //string loadText = Basic.MyFileStream.LoadFileAllText(@"C:\excel.txt", "utf-8");
            //SheetClass sheetClass = loadText.JsonDeserializet<SheetClass>();
            //byte[] excelBytes = sheetClass.NPOI_GetBytes();

            byte[] excelBytes = System.IO.File.ReadAllBytes(@"C:\TEST.xls");

            // 創建 HttpResponseMessage 對象
            HttpResponseMessage response = new HttpResponseMessage();

            response.Content = new ByteArrayContent(excelBytes);

            // 設置 Content-Type 和 Content-Disposition 標頭
            response.Content.Headers.Add("Content-Disposition", string.Format("attachment; filename=Excel.xls"));

            return response;

        }
        [Route("downloadfile")]
        [HttpGet]
        public async Task<ActionResult> DownloadExcelFile()
        {
            string loadText = Basic.MyFileStream.LoadFileAllText(@"C:\excel.txt", "utf-8");
            SheetClass sheetClass = loadText.JsonDeserializet<SheetClass>();
            byte[] excelData = sheetClass.NPOI_GetBytes();
            Stream stream = new MemoryStream(excelData);
            return await Task.FromResult(File(stream, "application/vnd.ms-excel", "excel-file.xls"));
        }

        [Route("image")]
        [HttpGet]
        public HttpResponseMessage GetImage()
        {
            var imgPath = @"C:\background.jpg";
            //从图片中读取byte  
            var imgByte = System.IO.File.ReadAllBytes(imgPath);
            //从图片中读取流  
            var imgStream = new MemoryStream(System.IO.File.ReadAllBytes(imgPath));
            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(imgByte)
                //或者  
                //Content = new StreamContent(stream)  
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return resp;
        }
        public class Distinct_class_Emg_Applies : IEqualityComparer<class_emg_apply>
        {
            public bool Equals(class_emg_apply x, class_emg_apply y)
            {
                return (x.藥品碼 == y.藥品碼);
            }

            public int GetHashCode(class_emg_apply obj)
            {
                return 1;
            }
        }
    }
}
