using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Basic;
using System.ComponentModel;

namespace HIS_DB_Lib
{
    public enum enum_盤點定盤_Excel
    {
        [Description("GUID,VARCHAR,50,NONE")]
        GUID,
        [Description("藥碼,VARCHAR,50,NONE")]
        藥碼,
        [Description("料號,VARCHAR,50,NONE")]
        料號,
        [Description("藥名,VARCHAR,50,NONE")]
        藥名,
        [Description("別名,VARCHAR,50,NONE")]
        別名,
        [Description("單價,VARCHAR,50,NONE")]
        單價,
        [Description("庫存量,VARCHAR,50,NONE")]
        庫存量,
        [Description("盤點量,VARCHAR,50,NONE")]
        盤點量,
        [Description("覆盤量,VARCHAR,50,NONE")]
        覆盤量,
        [Description("庫存金額,VARCHAR,50,NONE")]
        庫存金額,
        [Description("結存金額,VARCHAR,50,NONE")]
        結存金額,
        [Description("消耗量,VARCHAR,50,NONE")]
        消耗量,
        [Description("誤差量,VARCHAR,50,NONE")]
        誤差量,
        [Description("誤差金額,VARCHAR,50,NONE")]
        誤差金額,
        [Description("誤差百分率,VARCHAR,50,NONE")]
        誤差百分率, 
        [Description("註記,VARCHAR,50,NONE")]
        註記,
    }
    public enum enum_盤點明細_Excel
    {
        藥碼,
        料號,
        藥名,
        單位,
        盤點量,
        盤點人員,
        盤點時間,
    }

    public enum enum_盤點單上傳_Excel
    {
        藥碼,
        藥名,
        儲位名稱,
        盤點量,
    }
    public enum enum_盤點狀態
    {
        盤點中,
        等待盤點,
        鎖定,
    }
    [EnumDescription("inventory_creat")]
    public enum enum_盤點單號
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("盤點名稱,VARCHAR,500,NONE")]
        盤點名稱,
        [Description("類型,VARCHAR,50,NONE")]
        類型,
        [Description("盤點單號,VARCHAR,50,NONE")]
        盤點單號,
        [Description("建表人,VARCHAR,50,NONE")]
        建表人,
        [Description("建表時間,DATETIME,50,NONE")]
        建表時間,
        [Description("盤點開始時間,DATETIME,50,NONE")]
        盤點開始時間,
        [Description("盤點結束時間,DATETIME,50,NONE")]
        盤點結束時間,
        [Description("盤點狀態,VARCHAR,50,NONE")]
        盤點狀態,
        [Description("預設盤點人,VARCHAR,300,NONE")]
        預設盤點人,
        [Description("合併單號,VARCHAR,50,NONE")]
        合併單號,
        [Description("備註,VARCHAR,200,NONE")]
        備註,
    }
    [EnumDescription("inventory_content")]
    public enum enum_盤點內容
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("序號,VARCHAR,10,NONE")]
        序號,
        [Description("盤點單號,VARCHAR,50,INDEX")]
        盤點單號,
        [Description("藥品碼,VARCHAR,20,INDEX")]
        藥品碼,
        [Description("料號,VARCHAR,20,INDEX")]
        料號,
        [Description("藥品條碼1,VARCHAR,50,NONE")]
        藥品條碼1,
        [Description("藥品條碼2,TEXT,50,NONE")]
        藥品條碼2,
        [Description("儲位名稱,VARCHAR,50,NONE")]
        儲位名稱,
        [Description("理論值,VARCHAR,10,NONE")]
        理論值,
        [Description("單價,VARCHAR,10,NONE")]
        單價,
        [Description("消耗量,VARCHAR,10,NONE")]
        消耗量,
        [Description("新增時間,DATETIME,50,NONE")]
        新增時間,
        [Description("備註,VARCHAR,200,NONE")]
        備註,
    }
    [EnumDescription("inventory_sub_content")]
    public enum enum_盤點明細
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("盤點單號,VARCHAR,50,INDEX")]
        盤點單號,
        [Description("藥品碼,VARCHAR,20,INDEX")]
        藥品碼,
        [Description("料號,VARCHAR,20,INDEX")]
        料號,
        [Description("藥品條碼1,VARCHAR,50,NONE")]
        藥品條碼1,
        [Description("藥品條碼2,TEXT,50,NONE")]
        藥品條碼2,
        [Description("盤點量,VARCHAR,10,NONE")]
        盤點量,
        [Description("效期,DATETIME,50,NONE")]
        效期,
        [Description("批號,VARCHAR,50,NONE")]
        批號,
        [Description("操作時間,DATETIME,50,INDEX")]
        操作時間,
        [Description("操作人,VARCHAR,50,INDEX")]
        操作人,
        [Description("狀態,VARCHAR,50,NONE")]
        狀態,
        [Description("備註,VARCHAR,200,NONE")]
        備註,
    }
    public class inventoryClass
    {
        public class creat
        {
            [JsonPropertyName("GUID")]
            public string GUID { get; set; }
            [JsonPropertyName("IC_NAME")]
            public string 盤點名稱 { get; set; }
            [JsonPropertyName("TYPE")]
            public string 類型 { get; set; }
            [JsonPropertyName("IC_SN")]
            public string 盤點單號 { get; set; }
            [JsonPropertyName("CT")]
            public string 建表人 { get; set; }
            [JsonPropertyName("CT_TIME")]
            public string 建表時間 { get; set; }
            [JsonPropertyName("START_TIME")]
            public string 盤點開始時間 { get; set; }
            [JsonPropertyName("END_TIME")]
            public string 盤點結束時間 { get; set; }
            [JsonPropertyName("STATE")]
            public string 盤點狀態 { get; set; }
            [JsonPropertyName("DEFAULT_OP")]
            public string 預設盤點人 { get; set; }
            [JsonPropertyName("INV_SN")]
            public string 合併單號 { get; set; }
            [JsonPropertyName("NOTE")]
            public string 備註 { get; set; }

            private List<content> _contents = new List<content>();
            public List<content> Contents { get => _contents; set => _contents = value; }

            public void ContentAdd(content content)
            {
                List<content> Contents_buf = new List<content>();
                Contents_buf = (from temp in Contents
                                where temp.藥品碼 == content.藥品碼
                                select temp).ToList();
                if(Contents_buf.Count == 0)
                {
                    Contents.Add(content);
                }
            }

            public System.Data.DataTable get_all_sub_contents_dt()
            {
                List<object[]> list_value = new List<object[]>();
                List<sub_content> sub_Contents = get_all_sub_contents();
                for (int i = 0; i < sub_Contents.Count; i++)
                {
                    object[] value = new object[new enum_盤點明細_Excel().GetLength()];
                    value[(int)enum_盤點明細_Excel.藥碼] = sub_Contents[i].藥品碼;
                    value[(int)enum_盤點明細_Excel.藥名] = sub_Contents[i].藥品名稱;
                    value[(int)enum_盤點明細_Excel.料號] = sub_Contents[i].料號;
                    value[(int)enum_盤點明細_Excel.盤點人員] = sub_Contents[i].操作人;
                    value[(int)enum_盤點明細_Excel.盤點時間] = sub_Contents[i].操作時間;
                    value[(int)enum_盤點明細_Excel.盤點量] = sub_Contents[i].盤點量;
                    list_value.Add(value);
                }

                System.Data.DataTable dataTable = list_value.ToDataTable(new enum_盤點明細_Excel());
                dataTable.TableName = $"{盤點名稱}({盤點單號})";
                return dataTable;
            }

            public List<sub_content> get_all_sub_contents()
            {
                List<sub_content> sub_Contents = new List<sub_content>();
                for (int i = 0; i < Contents.Count; i++)
                {
                    for (int k = 0; k < Contents[i].Sub_content.Count; k++)
                    {
                        sub_Contents.Add(Contents[i].Sub_content[k]);
                    }
                }
                return sub_Contents;
            }


        }
        public class content
        {
            [JsonPropertyName("GUID")]
            public string GUID { get; set; }
            [JsonPropertyName("Master_GUID")]
            public string Master_GUID { get; set; }
            [JsonPropertyName("INDEX")]
            public string 序號 { get; set; }
            [JsonPropertyName("IC_SN")]
            public string 盤點單號 { get; set; }
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
            [JsonPropertyName("STORAGE_NAME")]
            public string 儲位名稱 { get; set; }
            [JsonPropertyName("BARCODE1")]
            public string 藥品條碼1 { get; set; }
            [JsonPropertyName("BARCODE2")]
            public string 藥品條碼2 { get; set; }
            [JsonPropertyName("START_QTY")]
            public string 理論值 { get; set; }
            [JsonPropertyName("PRICE")]
            public string 單價 { get; set; }
            [JsonPropertyName("CONSUME")]
            public string 消耗量 { get; set; }
            [JsonPropertyName("END_QTY")]
            public string 盤點量 { get; set; }
            [JsonPropertyName("ADD_TIME")]
            public string 新增時間 { get; set; }
            [JsonPropertyName("NOTE")]
            public string 備註 { get; set; }

            private List<sub_content> _sub_content = new List<sub_content>();
            public List<sub_content> Sub_content { get => _sub_content; set => _sub_content = value; }

        
        }
        public class sub_content
        {
            [JsonPropertyName("GUID")]
            public string GUID { get; set; }
            [JsonPropertyName("Master_GUID")]
            public string Master_GUID { get; set; }
            [JsonPropertyName("IC_SN")]
            public string 盤點單號 { get; set; }
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
            public string 盤點量 { get; set; }
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

        static public List<inventoryClass.creat> creat_get_by_CT_TIME_ST_END(string API_Server, DateTime start, DateTime end)
        {
            string url = $"{API_Server}/api/inventory/creat_get_by_CT_TIME_ST_END";
            returnData returnData = new returnData();
            returnData.Value = $"{start.ToDateTimeString_6()},{end.ToDateTimeString_6()}";
            string json_in = returnData.JsonSerializationt();
            string json_out = Basic.Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null)
            {
                return null;
            }
            if (returnData.Code != 200)
            {
                Console.WriteLine($"-----------------------------------------------");
                Console.WriteLine($"url : {url}");
                Console.WriteLine($"Result : {returnData.Result}");
                Console.WriteLine($"-----------------------------------------------");
                return null;
            }
            List<inventoryClass.creat> creats = returnData.Data.ObjToClass<List<inventoryClass.creat>>();
            if (creats == null)
            {
                return null;
            }
            return creats;
        }
        static public void creat_delete_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inventory/creat_delete_by_IC_SN";
            returnData returnData = new returnData();
            returnData.Value = IC_SN;
            string json_in = returnData.JsonSerializationt();
            string json_out = Basic.Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();

            if (returnData.Code != 200)
            {
                Console.WriteLine($"-----------------------------------------------");
                Console.WriteLine($"url : {url}");
                Console.WriteLine($"Result : {returnData.Result}");
                Console.WriteLine($"-----------------------------------------------");
            }
        }
        static public List<string> creat_get_default_op_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inventory/creat_get_default_op_by_IC_SN";
            returnData returnData = new returnData();
            returnData.Value = IC_SN;
            string json_in = returnData.JsonSerializationt();
            string json_out = Basic.Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200)
            {
                Console.WriteLine($"-----------------------------------------------");
                Console.WriteLine($"url : {url}");
                Console.WriteLine($"Result : {returnData.Result}");
                Console.WriteLine($"-----------------------------------------------");
                return null;
            }
            List<string> list_op = new List<string>();
            creat creat = returnData.Data.ObjToClass<creat>();
            if (creat == null) return list_op;
            string[] temp_ary = creat.預設盤點人.Split(',');
            for (int i = 0; i < temp_ary.Length; i++)
            {
                list_op.Add(temp_ary[i]);
            }
            return list_op;
        }
        static public void creat_update_default_op_by_IC_SN(string API_Server, string IC_SN , List<string> list_op)
        {
            string url = $"{API_Server}/api/inventory/creat_update_default_op_by_IC_SN";
            creat creat = new creat();
            string str = "";
            for (int i = 0; i < list_op.Count; i++)
            {
                str += list_op[i];
                if (i != list_op.Count - 1) str += ",";
            }
            creat.預設盤點人 = str;
            returnData returnData = new returnData();
            returnData.Value = IC_SN;
            returnData.Data = creat;
            string json_in = returnData.JsonSerializationt();
            string json_out = Basic.Net.WEBApiPostJson(url, json_in);
            if (returnData == null) return;
            if (returnData.Code != 200)
            {
                Console.WriteLine($"-----------------------------------------------");
                Console.WriteLine($"url : {url}");
                Console.WriteLine($"Result : {returnData.Result}");
                Console.WriteLine($"-----------------------------------------------");
                return;
            }

        }
        static public creat creat_get_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inventory/creat_get_by_IC_SN";
            returnData returnData = new returnData();
            returnData.Value = IC_SN;
            string json_in = returnData.JsonSerializationt();
            string json_out = Basic.Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return null;
            if (returnData.Code != 200)
            {
                Console.WriteLine($"-----------------------------------------------");
                Console.WriteLine($"url : {url}");
                Console.WriteLine($"Result : {returnData.Result}");
                Console.WriteLine($"-----------------------------------------------");
                return null;
            }
            List<creat> creats = returnData.Data.ObjToClass<List<creat>>();
            if (creats == null) return null;
            if (creats.Count == 0) return null;
            return creats[0];
        }
        static public byte[] download_excel_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inventory/download_excel_by_IC_SN";
            returnData returnData = new returnData();
            returnData.Value = IC_SN;
            string json_in = returnData.JsonSerializationt();
            byte[] bytes = Basic.Net.WEBApiPostDownloaFile(url, json_in);
            return bytes;
        }
        static public bool excel_inventory_upload(string API_Server, string filename , string IC_NAME, string CT, string DEFAULT_OP)
        {
            List<string> names = new List<string>();
            names.Add("IC_NAME");
            names.Add("CT");
            names.Add("DEFAULT_OP");
            List<string> values = new List<string>();
            values.Add(IC_NAME);
            values.Add(CT);
            values.Add(DEFAULT_OP);

            byte[] bytes = MyOffice.ExcelClass.NPOI_LoadToBytes(filename);


            string json_out = Basic.Net.WEBApiPost($"{API_Server}/api/inventory/excel_upload", filename, bytes, names, values);
            returnData returnData_out = json_out.JsonDeserializet<returnData>();

            if (returnData_out == null)
            {
                return false;
            }
            if (returnData_out.Data == null)
            {
                return false;
            }
            if (returnData_out.Code != 200) return false;

            Console.WriteLine($"[{returnData_out.Method}]:{returnData_out.Result}");
            return true;
        }

        static public void contents_delete_by_GUID(string API_Server, string GUID)
        {
            List<content> contents = new List<content>();
            content content = new content();
            content.GUID = GUID;
            contents.Add(content);
            contents_delete_by_GUID(API_Server, contents);
        }
        static public void contents_delete_by_GUID(string API_Server, content content)
        {
            List<content> contents = new List<content>();
            contents.Add(content);
            contents_delete_by_GUID(API_Server, contents);
        }
        static public void contents_delete_by_GUID(string API_Server, List<content> contents)
        {
            string url = $"{API_Server}/api/inventory/contents_delete_by_GUID";
            returnData returnData = new returnData();
            returnData.Data = contents;
            string json_in = returnData.JsonSerializationt();
            string json_out = Basic.Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return;
            if (returnData.Code != 200)
            {
                Console.WriteLine($"-----------------------------------------------");
                Console.WriteLine($"url : {url}");
                Console.WriteLine($"Result : {returnData.Result}");
                Console.WriteLine($"-----------------------------------------------");
                return;
            }
        }

        static public void creat_add(string API_Server, creat _creat)
        {
            string url = $"{API_Server}/api/inventory/creat_add";
            returnData returnData = new returnData();
            returnData.Data = _creat;
            string json_in = returnData.JsonSerializationt();
            string json_out = Basic.Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            if (returnData == null) return;
            if (returnData.Code != 200)
            {
                Console.WriteLine($"-----------------------------------------------");
                Console.WriteLine($"url : {url}");
                Console.WriteLine($"Result : {returnData.Result}");
                Console.WriteLine($"-----------------------------------------------");
                return;
            }
        }
        static public returnData creat_lock_by_IC_SN(string API_Server, string IC_SN)
        {
            string url = $"{API_Server}/api/inventory/creat_lock_by_IC_SN";

            returnData returnData = new returnData();
            returnData.Value = IC_SN;
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public returnData creat_lock_by_IC_SN(string API_Server, List<string> IC_SN)
        {
            string url = $"{API_Server}/api/inventory/creat_lock_by_IC_SN";

            returnData returnData = new returnData();
            returnData.Value = string.Join(";", IC_SN);
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public returnData creat_evd_auto_add(string API_Server, creat _creat, string valueAry)
        {
            string url = $"{API_Server}/api/inventory/creat_evd_auto_add";

            returnData returnData = new returnData();
            returnData.Data = _creat;
            returnData.ValueAry = valueAry.Split(';').ToList();
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
        static public returnData update_content(string API_Server, List<string> EvdInv)
        {
            string url = $"{API_Server}/api/inv_daily/update_content";

            returnData returnData = new returnData();
            returnData.Value = string.Join(";", EvdInv);
            string json_in = returnData.JsonSerializationt();
            string json_out = Net.WEBApiPostJson(url, json_in);
            returnData = json_out.JsonDeserializet<returnData>();
            return returnData;
        }
    }

  
}
