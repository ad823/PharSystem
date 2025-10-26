using Basic;
using H_Pannel_lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace HIS_DB_Lib
{
    [EnumDescription("medMap")]
    public enum enum_medMap
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("位置,VARCHAR,10,NONE")]
        位置,
        [Description("絕對位置,VARCHAR,20,NONE")]
        絕對位置,
        [Description("type,VARCHAR,30,NONE")]
        type
    }
    [EnumDescription("medMap_section")]
    public enum enum_medMap_section
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("位置,VARCHAR,10,NONE")]
        位置,
        [Description("絕對位置,VARCHAR,20,NONE")]
        絕對位置,
        [Description("type,VARCHAR,30,NONE")]
        type
    }
    [EnumDescription("medMap_sub_section")]
    public enum enum_medMap_sub_section
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("名稱,VARCHAR,20,NONE")]
        名稱,
        [Description("位置,VARCHAR,10,NONE")]
        位置,
        [Description("type,VARCHAR,30,NONE")]
        type
    }
    [EnumDescription("medMap_shelf")]
    public enum enum_medMap_shelf
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("名稱,VARCHAR,20,NONE")]
        名稱,
        [Description("位置,VARCHAR,10,NONE")]
        位置,
        [Description("type,VARCHAR,30,NONE")]
        type,
        [Description("寬度,VARCHAR,10,NONE")]
        寬度,
        [Description("高度,VARCHAR,10,NONE")]
        高度,
        [Description("燈條IP,VARCHAR,20,NONE")]
        燈條IP,
        [Description("device_type,VARCHAR,50,NONE")]
        device_type,
        [Description("serverName,VARCHAR,20,NONE")]
        serverName,
        [Description("serverType,VARCHAR,20,NONE")]
        serverType,
    }
    public enum  enum_shelfType
    {
        store_shelves,
        dispensing_shelves
    }
    [EnumDescription("medMap_drawer")]
    public enum enum_medMap_drawer
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("位置,VARCHAR,10,NONE")]
        位置,
        [Description("type,VARCHAR,30,NONE")]
        type,
        [Description("寬度,VARCHAR,10,NONE")]
        寬度,
        [Description("高度,VARCHAR,10,NONE")]
        高度,
        [Description("抽屜IP,VARCHAR,20,NONE")]
        抽屜IP,
        [Description("serverName,VARCHAR,20,NONE")]
        serverName,
        [Description("serverType,VARCHAR,20,NONE")]
        serverType,
    }
    [EnumDescription("medMap_box")]
    public enum enum_medMap_box
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("Master_GUID,VARCHAR,50,INDEX")]
        Master_GUID,
        [Description("位置,VARCHAR,10,NONE")]
        位置,
        [Description("type,VARCHAR,30,NONE")]
        type,
        [Description("寬度,VARCHAR,10,NONE")]
        寬度,
        [Description("高度,VARCHAR,10,NONE")]
        高度,
        [Description("藥盒IP,VARCHAR,20,NONE")]
        藥盒IP,
        [Description("serverName,VARCHAR,20,NONE")]
        serverName,
        [Description("serverType,VARCHAR,20,NONE")]
        serverType,
    }
    public enum enum_medMap_stock
    {
        GUID,
        shelf_GUID,
        device_type,
        device_Type1,
        位置,
        IP,
        燈條亮燈位置,
        藥碼,
        藥名,
        料號,
        批號,
        效期,
        數量
    }
    /// <summary>
    /// 藥品地圖_父容器
    /// </summary>
    public class medMapClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Master_GUID
        /// </summary>
        [JsonPropertyName("Master_GUID")]
        public string Master_GUID { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        [JsonPropertyName("position")]
        public string 位置 { get; set; }
        /// <summary>
        /// 絕對位置
        /// </summary>
        [JsonPropertyName("absolute_position")]
        public string 絕對位置 { get; set; }
        /// <summary>
        /// 種類
        /// </summary>
        [JsonPropertyName("type")]
        public string type{ get; set; }
        public sys_serverSettingClass sys_ServerSetting { get; set; }
        public List<medMap_sectionClass> medMap_Section {  get; set; }
    }
    /// <summary>
    /// 藥品地圖_子容器
    /// </summary>
    public class medMap_sectionClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Master_GUID
        /// </summary>
        [JsonPropertyName("Master_GUID")]
        public string Master_GUID { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        [JsonPropertyName("position")]
        public string 位置 { get; set; }
        /// <summary>
        /// 絕對位置
        /// </summary>
        [JsonPropertyName("absolute_position")]
        public string 絕對位置 { get; set; }
        /// <summary>
        /// 種類
        /// </summary>
        [JsonPropertyName("type")]
        public string type { get; set; }
        public List<medMap_sub_sectionClass> sub_section { get; set; }

    }
    /// <summary>
    /// 藥品地圖_子容器
    /// </summary>
    public class medMap_sub_sectionClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Master_GUID
        /// </summary>
        [JsonPropertyName("Master_GUID")]
        public string Master_GUID { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        [JsonPropertyName("name")]
        public string 名稱 { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        [JsonPropertyName("position")]
        public string 位置 { get; set; }
        /// <summary>
        /// 種類
        /// </summary>
        [JsonPropertyName("type")]
        public string type { get; set; }
        public string section_position { get; set; }
        public List<medMap_shelfClass> shelf { get; set; }
        public List<medMap_drawerClass> drawer { get; set; }

    }
    /// <summary>
    /// 藥品地圖_層架
    /// </summary>
    public class medMap_shelfClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Master_GUID
        /// </summary>
        [JsonPropertyName("Master_GUID")]
        public string Master_GUID { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        [JsonPropertyName("name")]
        public string 名稱 { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        [JsonPropertyName("position")]
        public string 位置 { get; set; }
        /// <summary>
        /// 種類
        /// </summary>
        [JsonPropertyName("type")]
        public string type { get; set; }
        /// <summary>
        /// 寬度
        /// </summary>
        [JsonPropertyName("width")]
        public string 寬度 { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        [JsonPropertyName("height")]
        public string 高度 { get; set; }
        /// <summary>
        /// 燈條IP
        /// </summary>
        [JsonPropertyName("ip_light")]
        public string 燈條IP { get; set; }
        /// <summary>
        /// 裝置類型
        /// </summary>
        [JsonPropertyName("device_type")]
        public string device_type { get; set; }
        /// <summary>
        /// serverName
        /// </summary>
        [JsonPropertyName("serverName")]
        public string serverName { get; set; }
        /// <summary>
        /// serverType
        /// </summary>
        [JsonPropertyName("serverType")]
        public string serverType { get; set; }
        public List<medMap_boxClass> medMapBox { get; set; }
        public List<medMap_stockClass> medMapStock { get; set; }
        public RowsLED rowsLED { get; set; }

    }
    /// <summary>
    /// 藥品地圖_抽屜
    /// </summary>
    public class medMap_drawerClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Master_GUID
        /// </summary>
        [JsonPropertyName("Master_GUID")]
        public string Master_GUID { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        [JsonPropertyName("position")]
        public string 位置 { get; set; }
        /// <summary>
        /// 種類
        /// </summary>
        [JsonPropertyName("type")]
        public string type { get; set; }
        /// <summary>
        /// 寬度
        /// </summary>
        [JsonPropertyName("width")]
        public string 寬度 { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        [JsonPropertyName("height")]
        public string 高度 { get; set; }
        /// <summary>
        /// 抽屜IP
        /// </summary>
        [JsonPropertyName("ip_drawer")]
        public string 抽屜IP { get; set; }
        /// <summary>
        /// serverName
        /// </summary>
        [JsonPropertyName("serverName")]
        public string serverName { get; set; }
        /// <summary>
        /// serverType
        /// </summary>
        [JsonPropertyName("serverType")]
        public string serverType { get; set; }
        public Drawer drawer { get; set; }
        public Storage storage { get; set; }

    }
    /// <summary>
    /// 藥品地圖_藥盒
    /// </summary>
    public class medMap_boxClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Master_GUID
        /// </summary>
        [JsonPropertyName("Master_GUID")]
        public string Master_GUID { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        [JsonPropertyName("position")]
        public string 位置 { get; set; }
        /// <summary>
        /// 種類
        /// </summary>
        [JsonPropertyName("type")]
        public string type { get; set; }
        /// <summary>
        /// 寬度
        /// </summary>
        [JsonPropertyName("width")]
        public string 寬度 { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        [JsonPropertyName("height")]
        public string 高度 { get; set; }
        /// <summary>
        /// 藥盒IP
        /// </summary>
        [JsonPropertyName("ip_box")]
        public string 藥盒IP { get; set; }
        /// <summary>
        /// serverName
        /// </summary>
        [JsonPropertyName("serverName")]
        public string serverName { get; set; }
        /// <summary>
        /// serverType
        /// </summary>
        [JsonPropertyName("serverType")]
        public string serverType { get; set; }
        public Storage storage { get; set; }


    }
    /// <summary>
    /// 貨品表資料結構  
    /// 用於紀錄藥品在各層架或裝置上的位置、批號、效期與數量。
    /// </summary>
    [Description("medMap_stock")]
    public class medMap_stockClass
    {
        /// <summary>
        /// 唯一識別碼 (GUID)
        /// </summary>
        [Description("GUID,VARCHAR,50,PRIMARY")]
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }

        /// <summary>
        /// 對應的層架 GUID
        /// </summary>
        [Description("shelf_GUID,VARCHAR,50,INDEX")]
        [JsonPropertyName("shelf_guid")]
        public string Shelf_GUID { get; set; }       

        /// <summary>
        /// 位置描述 (例如 上層第2層第3格)
        /// </summary>
        [Description("位置,VARCHAR,100,NONE")]
        [JsonPropertyName("location")]
        public string 位置 { get; set; }

        /// <summary>
        /// 裝置 IP 位址
        /// </summary>
        [Description("IP,VARCHAR,50,NONE")]
        [JsonPropertyName("ip")]
        public string IP { get; set; }

        /// <summary>
        /// 裝置類型 (例如 shelf、drawer、cabinet)
        /// </summary>
        [Description("device_type,VARCHAR,50,NONE")]
        [JsonPropertyName("device_type")]
        public string device_type { get; set; }

        /// <summary>
        /// 燈條亮燈位置 (LED Index)
        /// </summary>
        [Description("燈條亮燈位置,VARCHAR,50,NONE")]
        [JsonPropertyName("led_index")]
        public string 燈條亮燈位置 { get; set; }

        /// <summary>
        /// 藥品代碼
        /// </summary>
        [Description("藥碼,VARCHAR,50,INDEX")]
        [JsonPropertyName("code")]
        public string 藥碼 { get; set; }

        /// <summary>
        /// 藥品名稱
        /// </summary>
        [Description("藥名,VARCHAR,200,NONE")]
        [JsonPropertyName("name")]
        public string 藥名 { get; set; }

        /// <summary>
        /// 料號 (Material Number)
        /// </summary>
        [Description("料號,VARCHAR,100,NONE")]
        [JsonPropertyName("material_no")]
        public string 料號 { get; set; }

        /// <summary>
        /// 批號 (Lot Number)
        /// </summary>
        [Description("批號,VARCHAR,100,NONE")]
        [JsonPropertyName("lot")]
        public string 批號 { get; set; }

        /// <summary>
        /// 效期 (Expiry Date)
        /// </summary>
        [Description("效期,DATETIME,10,NONE")]
        [JsonPropertyName("expiry_date")]
        public string 效期 { get; set; }

        /// <summary>
        /// 數量 (Quantity)
        /// </summary>
        [Description("數量,VARCHAR,10,NONE")]
        [JsonPropertyName("qty")]
        public string 數量 { get; set; }
    }

    public static class medMapMethod
    {
        static public Dictionary<string, List<medMap_stockClass>> ToDictByShelfGUID(this List<medMap_stockClass> stockClasses)
        {
            Dictionary<string, List<medMap_stockClass>> dictionary = new Dictionary<string, List<medMap_stockClass>>();
            foreach (var item in stockClasses)
            {
                if (dictionary.TryGetValue(item.Shelf_GUID, out List<medMap_stockClass> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.Shelf_GUID] = new List<medMap_stockClass> { item };
                }
            }
            return dictionary;
        }
        static public List<medMap_stockClass> GetByShelfGUID(this Dictionary<string, List<medMap_stockClass>> dict, string Shelf_GUID)
        {
            if (dict.TryGetValue(Shelf_GUID, out List<medMap_stockClass> stockClasses))
            {
                return stockClasses;
            }
            else
            {
                return new List<medMap_stockClass>();
            }
        }
        static public Dictionary<string, List<medMap_shelfClass>> ToDictByMasterGUID(this List<medMap_shelfClass> shelfClasses)
        {
            Dictionary<string, List<medMap_shelfClass>> dictionary = new Dictionary<string, List<medMap_shelfClass>>();
            foreach (var item in shelfClasses)
            {
                if (dictionary.TryGetValue(item.Master_GUID, out List<medMap_shelfClass> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.Master_GUID] = new List<medMap_shelfClass> { item };
                }
            }
            return dictionary;
        }
        static public List<medMap_shelfClass> GetByMasterGUID(this Dictionary<string, List<medMap_shelfClass>> dict, string Master_GUID)
        {
            if (dict.TryGetValue(Master_GUID, out List<medMap_shelfClass> shelfClasses))
            {
                return shelfClasses;
            }
            else
            {
                return new List<medMap_shelfClass>();
            }
        }

    }
}
