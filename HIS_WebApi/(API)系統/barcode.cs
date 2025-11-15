using Basic;
using Google.Protobuf.WellKnownTypes;
using H_Pannel_lib;
using HIS_DB_Lib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyOffice;
using MySql.Data.MySqlClient;
using MyUI;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Basic.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HIS_WebApi._API_系統
{
    [Route("api/[controller]")]
    [ApiController]
    public class barcode : ControllerBase
    {

        [HttpPost]
        public async Task<string> excel_upload_extra([FromForm] IFormFile file)
        {
            try
            {
                using (var client = new HttpClient())
                using (var form = new MultipartFormDataContent())
                {
                    // 把 IFormFile 轉成 StreamContent
                    using (var stream = file.OpenReadStream())
                    {
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                        // 加入到 multipart form，name 要和 API 預期的欄位名一致
                        form.Add(fileContent, "file", file.FileName);

                        // 發送 POST 請求
                        var response = await client.PostAsync("http://192.168.5.205:3100/barcode", form);
                        response.EnsureSuccessStatusCode();

                        // 讀取回應內容
                        return await response.Content.ReadAsStringAsync();
                    }
                }

            }
            catch(Exception ex)
            {
                Logger.Log($"{ex.Message}");
                return null;
            }
            
            
            
        }
        [HttpPost("pill_counter")]
        public async Task<string> pill_counter([FromForm] IFormFile file)
        {
            try
            {
                using (var client = new HttpClient())
                using (var form = new MultipartFormDataContent())
                {
                    // 把 IFormFile 轉成 StreamContent
                    using (var stream = file.OpenReadStream())
                    {
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                        // 加入到 multipart form，name 要和 API 預期的欄位名一致
                        form.Add(fileContent, "file", file.FileName);

                        // 發送 POST 請求
                        var response = await client.PostAsync("http://192.168.5.205:3100/pill_counter", form);
                        response.EnsureSuccessStatusCode();

                        // 讀取回應內容
                        return await response.Content.ReadAsStringAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}");
                return null;
            }



        }
    }
}
