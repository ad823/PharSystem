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
using System.Configuration;
using HIS_DB_Lib;
using System.IO;
namespace HIS_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class cert : Controller
    {
        private static string currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private static string fileDirectory = $"{currentDirectory}/log/";

        [HttpGet]
        public IActionResult GetCertificate()
        {
            var certPath = Path.Combine(Directory.GetCurrentDirectory(), currentDirectory, "certlm.cer");
            var certBytes = System.IO.File.ReadAllBytes(certPath);
            return File(certBytes, "application/x-x509-ca-cert", "certlm.cer");
        }
        
        [HttpGet("ai")]
        public IActionResult GetCertificate_ai()
        {
            var certPath = Path.Combine(Directory.GetCurrentDirectory(), currentDirectory, "certlm_ai.cer");
            var certBytes = System.IO.File.ReadAllBytes(certPath);
            return File(certBytes, "application/x-x509-ca-cert", "certlm_ai.cer");
        }
        [HttpGet("lan")]
        public IActionResult GetCertificate_lan()
        {
            try 
            {
                var certPath = Path.Combine(Directory.GetCurrentDirectory(), currentDirectory, "full_chain.cer");

                var certBytes = System.IO.File.ReadAllBytes(certPath);
                return File(certBytes, "application/x-x509-ca-cert", "full_chain.cer");
            }
            catch(Exception ex)
            {
                Logger.Log($"{ex.Message}");
                return null;
            }
            
        }
    }
}
