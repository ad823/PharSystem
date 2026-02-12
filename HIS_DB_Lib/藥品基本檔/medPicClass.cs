using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.ComponentModel;
using System.Reflection;
using SQLUI;
namespace HIS_DB_Lib
{
    [EnumDescription("medPic")]
    public enum enum_medPic
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("藥碼,VARCHAR,50,INDEX")]
        藥碼,
        [Description("藥名,VARCHAR,300,NONE")]
        藥名,
        [Description("副檔名,VARCHAR,20,NONE")]
        副檔名,
        [Description("副檔名1,VARCHAR,20,NONE")]
        副檔名1,
        [Description("pic_base64,LONGTEXT,50,NONE")]
        pic_base64,
        [Description("pic1_base64,LONGTEXT,50,NONE")]
        pic1_base64,

    }
    public class medPicClass
    { /// <summary>
      /// 唯一KEY
      /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// 藥碼
        /// </summary>
        [JsonPropertyName("code")]
        public string 藥碼 { get; set; }
        /// <summary>
        /// 藥名
        /// </summary>
        [JsonPropertyName("name")]
        public string 藥名 { get; set; }
        /// <summary>
        /// 副檔名
        /// </summary>
        [JsonPropertyName("extension")]
        public string 副檔名 { get; set; }
        /// <summary>
        /// 副檔名
        /// </summary>
        [JsonPropertyName("extension1")]
        public string 副檔名1 { get; set; }
        /// <summary>
        /// pic_base64
        /// </summary>
        [JsonPropertyName("pic_base64")]
        public string pic_base64 { get; set; }
        /// <summary>
        /// pic_base64
        /// </summary>
        [JsonPropertyName("pic1_base64")]
        public string pic1_base64 { get; set; }

        [JsonIgnore]
        public System.Drawing.Image Image_0;
        [JsonIgnore]
        public System.Drawing.Image Image_1;
        static public SQLUI.Table init(string API_Server)
        {
            string url = $"{API_Server}/api/medPic/init";

            returnData returnData = new returnData();
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            SQLUI.Table table = json_out.JsonDeserializet<SQLUI.Table>();
            return table;
        }
        static public void add(string API_Server, medPicClass medPicClass)
        {
            List<medPicClass> medPicClasses = new List<medPicClass>();
            medPicClasses.Add(medPicClass);
            add(API_Server, medPicClasses);
        }
        static public void add(string API_Server, List<medPicClass> medPicClasses)
        {
            string url = $"{API_Server}/api/medPic/add";

            returnData returnData = new returnData();
            returnData.Data = medPicClasses;

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {
                return;
            }
            if (returnData_out.Data == null)
            {
                return;
            }
            Console.WriteLine($"{returnData_out}");
            return;
        }

        static public medPicClass get_by_code(string API_Server, string code)
        {
            string url = $"{API_Server}/api/medPic/get_by_code";

            returnData returnData = new returnData();
            returnData.ValueAry.Add(code);

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null)
            {
                return null;
            }
            if (returnData_out.Data == null)
            {
                return null;
            }
            Console.WriteLine($"url:[{url}],{returnData_out}");

            medPicClass medPicClass = returnData_out.Data.ObjToClass<medPicClass>();
            return medPicClass;
        }

        static public List<System.Drawing.Image> get_images_by_code(string API_Server, string code)
        {
            List<System.Drawing.Image> images = new List<System.Drawing.Image>();
            medPicClass medPicClass = get_by_code(API_Server, code);

            if (medPicClass == null) return null;
            System.Drawing.Image image0 = null;
            System.Drawing.Image image1 = null;

            if (medPicClass.pic_base64.StringIsEmpty() == false)
            {
                image0 = medPicClass.pic_base64.Base64ToImage();
            }
            if (medPicClass.pic1_base64.StringIsEmpty() == false)
            {
                image1 = medPicClass.pic1_base64.Base64ToImage();
            }
            images.Add(image0);
            images.Add(image1);

            return images;
        }
        static public System.Drawing.Image get_image_by_code(string API_Server, string code)
        {
            medPicClass medPicClass = get_by_code(API_Server, code);

            if (medPicClass == null) return null;
            string base64 = medPicClass.pic_base64;
            System.Drawing.Image image = base64.Base64ToImage();
            return image;
        }

    }
}
