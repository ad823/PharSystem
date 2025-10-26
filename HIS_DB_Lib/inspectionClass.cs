using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.ComponentModel;
using System.Reflection;

namespace HIS_DB_Lib
{
    public enum enum_驗收單匯入
    {
        請購單號,
        藥碼,
        名稱,
        規格,
        單位,
        採購數量,
        已交貨數量,
        未交量,
        供應商名稱,
        供應商電話,
        請購日期,
        採購日期,
        下單日期,
        請購發票年月
    }
    public enum enum_出貨單匯入
    {
        請購單號,
        品名,
        出貨數量,
        批號,
        效期
    }
    public enum enum_驗收資料匯出
    {
        驗收單號,
        請購單號,
        請購項次,
        藥碼,
        藥名,
        應收數量,
        實收數量,
        效期,
        批號,
        訂單時間,
        預定交貨時間,
        收貨時間,
        操作人,
    }


    [EnumDescription("inspection_creat")]
    public enum enum_驗收單號
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("驗收名稱,VARCHAR,200,NONE")]
        驗收名稱,
        [Description("請購單號,VARCHAR,30,NONE")]
        請購單號,
        [Description("驗收單號,VARCHAR,30,NONE")]
        驗收單號,
        [Description("建表人,VARCHAR,30,NONE")]
        建表人,
        [Description("建表時間,DATETIME,200,NONE")]
        建表時間,
        [Description("驗收開始時間,DATETIME,200,NONE")]
        驗收開始時間,
        [Description("驗收結束時間,DATETIME,200,NONE")]
        驗收結束時間,
        [Description("驗收狀態,VARCHAR,30,NONE")]
        驗收狀態,
        [Description("備註,VARCHAR,200,NONE")]
        備註,
    }
    [EnumDescription("inspection_content")]
    public enum enum_驗收內容
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("請購單號,VARCHAR,30,INDEX")]
        請購單號,
        [Description("請購項次,VARCHAR,10,NONE")]
        請購項次,
        [Description("驗收單號,VARCHAR,30,INDEX")]
        驗收單號,
        [Description("藥品碼,VARCHAR,20,INDEX")]
        藥品碼,
        [Description("藥品名稱,VARCHAR,300,NONE")]
        藥品名稱,
        [Description("廠牌,VARCHAR,200,NONE")]
        廠牌,
        [Description("料號,VARCHAR,30,INDEX")]
        料號,
        [Description("藥品條碼1,VARCHAR,50,NONE")]
        藥品條碼1,
        [Description("藥品條碼2,TEXT,50,NONE")]
        藥品條碼2,
        [Description("應收數量,VARCHAR,10,NONE")]
        應收數量,
        [Description("新增時間,DATETIME,50,INDEX")]
        新增時間,
        [Description("訂單時間,DATETIME,50,INDEX")]
        訂單時間,
        [Description("交貨時間,DATETIME,50,INDEX")]
        交貨時間,
        [Description("編號,VARCHAR,10,NONE")]
        編號,
        [Description("贈品註記,VARCHAR,10,NONE")]
        贈品註記,
        [Description("API回寫註記,VARCHAR,100,NONE")]
        API回寫註記,
        [Description("備註,VARCHAR,200,NONE")]
        備註,

    }
    [EnumDescription("inspection_sub_content")]
    public enum enum_驗收明細
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("驗收單號,VARCHAR,30,INDEX")]
        驗收單號,
        [Description("藥品碼,VARCHAR,20,INDEX")]
        藥品碼,
        [Description("藥品名稱,VARCHAR,300,NONE")]
        藥品名稱,
        [Description("料號,VARCHAR,30,INDEX")]
        料號,
        [Description("藥品條碼1,VARCHAR,50,NONE")]
        藥品條碼1,
        [Description("藥品條碼2,TEXT,50,NONE")]
        藥品條碼2,
        [Description("實收數量,VARCHAR,10,NONE")]
        實收數量,
        [Description("效期,DATETIME,50,NONE")]
        效期,
        [Description("批號,VARCHAR,20,NONE")]
        批號,
        [Description("操作時間,DATETIME,50,INDEX")]
        操作時間,
        [Description("操作人,VARCHAR,50,NONE")]
        操作人,
        [Description("狀態,VARCHAR,20,NONE")]
        狀態,
        [Description("備註,VARCHAR,200,NONE")]
        備註
    }
    public class inspectionClass
    {

        static public List<SQLUI.Table> Init(string API_Server)
        {
            string url = $"{API_Server}/api/inspection/init";

            returnData returnData = new returnData();

            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            List<SQLUI.Table> tables = json_out.JsonDeserializet<List<SQLUI.Table>>();
            return tables;
        }
        static public string new_IC_SN(string API_Server)
        {
            string url = $"{API_Server}/api/inspection/new_IC_SN";
            returnData returnData = new returnData();

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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");
            return returnData_out.Value;
        }
        static public List<inspectionClass.creat> creat_get_by_CT_TIME_ST_END(string API_Server, DateTime dateTime)
        {
            return creat_get_by_CT_TIME_ST_END(API_Server, dateTime.GetStartDate(), dateTime.GetEndDate());
        }
        static public List<inspectionClass.creat> creat_get_by_CT_TIME_ST_END(string API_Server, DateTime dateTime_st, DateTime dateTime_end)
        {
            string url = $"{API_Server}/api/inspection/creat_get_by_CT_TIME_ST_END";
            returnData returnData = new returnData();
            returnData.Value = $"{dateTime_st.ToDateTimeString()},{dateTime_end.ToDateTimeString()}";
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            List<inspectionClass.creat> creats = returnData_out.Data.ObjToClass<List<inspectionClass.creat>>();

            return creats;
        }
        static public inspectionClass.creat creat_update_startime_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inspection/creat_update_startime_by_IC_SN";
            returnData returnData = new returnData();
            returnData.Value = $"{IC_SN}";
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            inspectionClass.creat creat = returnData_out.Data.ObjToClass<inspectionClass.creat>();

            return creat;
        }
        static public inspectionClass.creat creat_get_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inspection/creat_get_by_IC_SN";
            returnData returnData = new returnData();
            returnData.Value = $"{IC_SN}";
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            List<inspectionClass.creat> creats = returnData_out.Data.ObjToClass<List<inspectionClass.creat>>();
            if (creats == null) return null;
            if (creats.Count == 0) return null;
            return creats[0];
        }
        static public inspectionClass.creat creat_add(string API_Server, inspectionClass.creat creat)
        {
            string url = $"{API_Server}/api/inspection/creat_add";
            returnData returnData = new returnData();
            returnData.Data = creat;
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            inspectionClass.creat creat_out = returnData_out.Data.ObjToClass<inspectionClass.creat>();
            return creat_out;
        }
        static public inspectionClass.creat creat_update(string API_Server, inspectionClass.creat creat)
        {
            string url = $"{API_Server}/api/inspection/creat_update";
            returnData returnData = new returnData();
            returnData.Data = creat;
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            inspectionClass.creat creat_out = returnData_out.Data.ObjToClass<inspectionClass.creat>();
            return creat_out;
        }

        
        static public void creat_lock_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inspection/creat_lock_by_IC_SN";
            returnData returnData = new returnData();
            returnData.Value = $"{IC_SN}";
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

        }
        static public void creat_unlock_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inspection/creat_unlock_by_IC_SN";
            returnData returnData = new returnData();
            returnData.Value = $"{IC_SN}";
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");


        }
        static public void creat_delete_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inspection/creat_delete_by_IC_SN";
            returnData returnData = new returnData();
            returnData.Value = $"{IC_SN}";
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

         
        }
        static public void contents_delete_by_GUID(string API_Server, List<inspectionClass.content> contents)
        {
            string url = $"{API_Server}/api/inspection/contents_delete_by_GUID";
            returnData returnData = new returnData();
            returnData.Data = contents;
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

        }
        static public returnData content_add(string API_Server, List<inspectionClass.content> contents)
        {
            string url = $"{API_Server}/api/inspection/content_add";
            returnData returnData = new returnData();
            returnData.Data = contents;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            return returnData_out;



        }
        static public inspectionClass.content content_get_by_content_GUID(string API_Server, inspectionClass.content content)
        {
            string url = $"{API_Server}/api/inspection/content_get_by_content_GUID";
            returnData returnData = new returnData();
            returnData.Data = content;
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            inspectionClass.content content_out = returnData_out.Data.ObjToClass<inspectionClass.content>();
            return content_out;
        }
        static public inspectionClass.content content_get_by_PON(string API_Server, string 驗收單號, string 請購單號)
        {
            string url = $"{API_Server}/api/inspection/content_get_by_PON";
            returnData returnData = new returnData();
            returnData.ValueAry.Add(驗收單號);
            returnData.ValueAry.Add(請購單號);
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return null;
            if (returnData_out.Code != 200) return null;
            inspectionClass.content content = returnData_out.Data.ObjToClass<inspectionClass.content>();
            return content;
        }
        static public inspectionClass.content sub_content_get_by_PON(string API_Server, string 請購單號)
        {
            string url = $"{API_Server}/api/inspection/sub_content_get_by_PON";
            returnData returnData = new returnData();
            returnData.Value = 請購單號;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            if (returnData_out == null) return null;
            if (returnData_out.Code != 200) return null;
            inspectionClass.content content = returnData_out.Data.ObjToClass<inspectionClass.content>();
            return content;
        }
        static public List<inspectionClass.sub_content> sub_content_get_by_content_GUID(string API_Server, inspectionClass.content content)
        {
            string url = $"{API_Server}/api/inspection/sub_content_get_by_content_GUID";
            returnData returnData = new returnData();
            returnData.Data = content;
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            List<inspectionClass.sub_content> sub_Contents = returnData_out.Data.ObjToClass<List<inspectionClass.sub_content>>();
            return sub_Contents;
        }
        static public inspectionClass.content sub_content_add_single(string API_Server, inspectionClass.sub_content sub_Content)
        {
            string url = $"{API_Server}/api/inspection/sub_content_add_single";
            returnData returnData = new returnData();
            returnData.Data = sub_Content;
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            inspectionClass.content content = returnData_out.Data.ObjToClass<inspectionClass.content>();
            return content;
        }
        static public inspectionClass.content sub_content_add(string API_Server, inspectionClass.sub_content sub_Content)
        {
            string url = $"{API_Server}/api/inspection/sub_content_add";
            returnData returnData = new returnData();
            returnData.Data = sub_Content;
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            inspectionClass.content content = returnData_out.Data.ObjToClass<inspectionClass.content>();
            return content;
        }
        static public returnData returnData_sub_content_add(string API_Server, inspectionClass.sub_content sub_Content)
        {
            string url = $"{API_Server}/api/inspection/sub_content_add";
            returnData returnData = new returnData();
            returnData.Data = sub_Content;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();
            
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

            //inspectionClass.content content = returnData_out.Data.ObjToClass<inspectionClass.content>();
            return returnData_out;
        }
        static public void sub_content_update(string API_Server, inspectionClass.sub_content sub_Content)
        {
            List<inspectionClass.sub_content> sub_Contents = new List<sub_content>();
            sub_Contents.Add(sub_Content);

            sub_content_update(API_Server, sub_Contents);
        }
        static public void sub_content_update(string API_Server, List<inspectionClass.sub_content> sub_Contents)
        {
            string url = $"{API_Server}/api/inspection/sub_content_update";
            returnData returnData = new returnData();
            returnData.Data = sub_Contents;
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");

        }

        static public inspectionClass.content sub_contents_delete_by_GUID(string API_Server, inspectionClass.sub_content sub_Content)
        {
            List<inspectionClass.sub_content> sub_Contents = new List<sub_content>();
            sub_Contents.Add(sub_Content);
            return sub_contents_delete_by_GUID(API_Server, sub_Contents);
        }
        static public inspectionClass.content sub_contents_delete_by_GUID(string API_Server, List<inspectionClass.sub_content> sub_Contents)
        {
            string url = $"{API_Server}/api/inspection/sub_contents_delete_by_GUID";
            returnData returnData = new returnData();
            returnData.Data = sub_Contents;
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
            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");
            inspectionClass.content content = returnData_out.Data.ObjToClass<inspectionClass.content>();
            return content;
        }
     

        static public void dbvm_refresh()
        {
        }

        static public void MergeData(creat original, creat compare)
        {
            // 处理 Contents
            foreach (var compareContent in compare.Contents)
            {
                var originalContent = original.Contents.FirstOrDefault(c => c.藥品碼 == compareContent.藥品碼);
                if (originalContent == null)
                {
                    // 新增的内容
                    original.Contents.Add(compareContent);
                }
                else
                {
                    // 处理 Sub_content
                    foreach (var compareSubContent in compareContent.Sub_content)
                    {
                        var originalSubContent = originalContent.Sub_content.FirstOrDefault(s => s.備註 == compareSubContent.備註);
                        if (originalSubContent == null)
                        {
                            // 新增的子内容
                            originalContent.Sub_content.Add(compareSubContent);
                        }
                    }

                    // 删除原始 Sub_content 中没有的内容
                    originalContent.Sub_content.RemoveAll(s => !compareContent.Sub_content.Any(c => c.備註 == s.備註));
                }
            }

            // 删除原始 Contents 中没有的内容
            original.Contents.RemoveAll(c => !compare.Contents.Any(c2 => c2.藥品碼 == c.藥品碼));
        }

        public class creat
        {
            [JsonPropertyName("GUID")]
            public string GUID { get; set; }
            [JsonPropertyName("IC_NAME")]
            public string 驗收名稱 { get; set; }
            [JsonPropertyName("PON")]
            public string 請購單號 { get; set; }
            [JsonPropertyName("IC_SN")]
            public string 驗收單號 { get; set; }
            [JsonPropertyName("CT")]
            public string 建表人 { get; set; }
            [JsonPropertyName("CT_TIME")]
            public string 建表時間 { get; set; }
            [JsonPropertyName("START_TIME")]
            public string 驗收開始時間 { get; set; }
            [JsonPropertyName("END_TIME")]
            public string 驗收結束時間 { get; set; }
            [JsonPropertyName("STATE")]
            public string 驗收狀態 { get; set; }
            [JsonPropertyName("NOTE")]
            public string 備註 { get; set; }

            private List<content> _contents = new List<content>();
            public List<content> Contents { get => _contents; set => _contents = value; }

        }
        public class content
        {
            [JsonPropertyName("GUID")]
            public string GUID { get; set; }
            [JsonPropertyName("Master_GUID")]
            public string Master_GUID { get; set; }
            [JsonPropertyName("PON")]
            public string 請購單號 { get; set; }
            [JsonPropertyName("INDEX")]
            public string 請購項次 { get; set; }
            [JsonPropertyName("IC_SN")]
            public string 驗收單號 { get; set; }
            [JsonPropertyName("CODE")]
            public string 藥品碼 { get; set; }
            [JsonPropertyName("BRD")]
            public string 廠牌 { get; set; }
            [JsonPropertyName("SKDIACODE")]
            public string 料號 { get; set; }
            [JsonPropertyName("CHT_NAME")]
            public string 中文名稱 { get; set; }
            [JsonPropertyName("NAME")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("PAKAGE")]
            public string 包裝單位 { get; set; }
            [JsonPropertyName("BARCODE1")]
            public string 藥品條碼1 { get; set; }
            [JsonPropertyName("BARCODE2")]
            public string 藥品條碼2 { get; set; }
            [JsonPropertyName("START_QTY")]
            public string 應收數量 { get; set; }
            [JsonPropertyName("END_QTY")]
            public string 實收數量 { get; set; }
            [JsonPropertyName("ADD_TIME")]
            public string 新增時間 { get; set; }
            [JsonPropertyName("ORDER_TIME")]
            public string 訂單時間 { get; set; }
            [JsonPropertyName("DELIVERY_TIME")]
            public string 交貨時間 { get; set; }
            [JsonPropertyName("SEQ")]
            public string 編號 { get; set; }
            [JsonPropertyName("FREE_CHARGE_FLAG")]
            public string 贈品註記 { get; set; }
            [JsonPropertyName("API_RETURN_NOTE")]
            public string API回寫註記 { get; set; }
            [JsonPropertyName("NOTE")]
            public string 備註 { get; set; }

            private List<sub_content> _sub_content = new List<sub_content>();
            public List<sub_content> Sub_content { get => _sub_content; set => _sub_content = value; }
            public textVisionClass textVision { get; set; }


        }
        public class sub_content
        {
            [JsonPropertyName("GUID")]
            public string GUID { get; set; }
            [JsonPropertyName("Master_GUID")]
            public string Master_GUID { get; set; }
            [JsonPropertyName("ACPT_SN")]
            public string 驗收單號 { get; set; }
            [JsonPropertyName("CODE")]
            public string 藥品碼 { get; set; }
            [JsonPropertyName("SKDIACODE")]
            public string 料號 { get; set; }
            [JsonPropertyName("CHT_NAME")]
            public string 中文名稱 { get; set; }
            [JsonPropertyName("NAME")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("PAKAGE")]
            public string 包裝單位 { get; set; }
            [JsonPropertyName("BARCODE1")]
            public string 藥品條碼1 { get; set; }
            [JsonPropertyName("BARCODE2")]
            public string 藥品條碼2 { get; set; }
            [JsonPropertyName("END_QTY")]
            public string 實收數量 { get; set; }
            [JsonPropertyName("TOLTAL_QTY")]
            public string 總量 { get; set; }
            [JsonPropertyName("VAL")]
            public string 效期 { get; set; }
            [JsonPropertyName("LOT")]
            public string 批號 { get; set; }
            [JsonPropertyName("OP")]
            public string 操作人 { get; set; }
            [JsonPropertyName("OP_TIME")]
            public string 操作時間 { get; set; }
            [JsonPropertyName("STATE")]
            public string 狀態 { get; set; }
            [JsonPropertyName("NOTE")]
            public string 備註 { get; set; }

         
        }
    }
    static public class inspectionMethod
    {
        static public inspectionClass.content Get_Content_By_GUID(this inspectionClass.creat creat, string GUID)
        {
            List<inspectionClass.content> contents = (from temp in creat.Contents
                                                      where temp.GUID == GUID
                                                      select temp).ToList();
            if (contents.Count == 0) return null;
            return contents[0];
        }
        static public inspectionClass.sub_content Get_SubContent_By_GUID(this inspectionClass.creat creat, string GUID)
        {
            for (int i = 0; i < creat.Contents.Count; i++)
            {
                for (int k = 0; k < creat.Contents[i].Sub_content.Count; k++)
                {
                    if (creat.Contents[i].Sub_content[k].GUID == GUID) return creat.Contents[i].Sub_content[k];
                }
            }
            return null;
        }
        static public List<inspectionClass.sub_content> Get_SubContent_By_MasterGUID(this inspectionClass.creat creat, string Master_GUID)
        {
            List<inspectionClass.sub_content> sub_Contents = new List<inspectionClass.sub_content>();
            for (int i = 0; i < creat.Contents.Count; i++)
            {
                for (int k = 0; k < creat.Contents[i].Sub_content.Count; k++)
                {
                    if (creat.Contents[i].Sub_content[k].Master_GUID == Master_GUID) sub_Contents.Add(creat.Contents[i].Sub_content[k]);
                }
            }
            return sub_Contents;
        }
        static public Dictionary<string, List<inspectionClass.sub_content>> ToDictByMasterGUID(this List<inspectionClass.sub_content> sub_Contents)
        {
            Dictionary<string, List<inspectionClass.sub_content>> dictionary = new Dictionary<string, List<inspectionClass.sub_content>>();
            foreach (var item in sub_Contents)
            {
                if (dictionary.TryGetValue(item.Master_GUID, out List<inspectionClass.sub_content> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.Master_GUID] = new List<inspectionClass.sub_content> { item };
                }
            }
            return dictionary;
        }
        static public List<inspectionClass.sub_content> GetByMasterGUID(this Dictionary<string, List<inspectionClass.sub_content>> dict, string master_GUID)
        {
            if (dict.TryGetValue(master_GUID, out List<inspectionClass.sub_content> content))
            {
                return content;
            }
            else
            {
                return new List<inspectionClass.sub_content>();
            }
        }
    }

}
