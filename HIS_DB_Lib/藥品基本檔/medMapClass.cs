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
    public enum enum_medMap_section
    {
        GUID,
        Master_GUID,
        名稱,
        燈棒IP,
        device_type,
        位置,
        絕對位置,
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
        位置,
        IP,
        device_type,
        燈條亮燈位置,
        藥碼,
        藥名,
        料號,
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
    [Description("medMap_section")]
    public class medMap_sectionClass
    {
        /// <summary>
        /// 唯一KEY
        /// </summary>
        [Description("VARCHAR,50,PRIMARY")]
        [JsonPropertyName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Master_GUID
        /// </summary>
        [Description("VARCHAR,50,INDEX")]
        [JsonPropertyName("Master_GUID")]
        public string Master_GUID { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        
        [JsonPropertyName("name")]
        public string 名稱 { get; set; }
        /// <summary>
        /// 燈棒IP
        /// </summary>
        [Description("VARCHAR,10,NONE")]
        
        [JsonPropertyName("ip_light")]
        public string 燈棒IP { get; set; }
        /// <summary>
        /// 裝置類型
        /// </summary>
        [Description("VARCHAR,10,NONE")]

        public string device_type { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        [Description("VARCHAR,10,NONE")]
        [JsonPropertyName("position")]
        public string 位置 { get; set; }
        /// <summary>
        /// 絕對位置
        /// </summary>
        [Description("VARCHAR,20,NONE")]
        [JsonPropertyName("absolute_position")]
        public string 絕對位置 { get; set; }
        /// <summary>
        /// 種類
        /// </summary>
        [Description("type,VARCHAR,30,NONE")]
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
        public List<stockClass> medMapStock { get; set; }
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
    

    public static class medMapMethod
    {
        static public Dictionary<string, List<stockClass>> ToDictByShelfGUID(this List<stockClass> stockClasses)
        {
            Dictionary<string, List<stockClass>> dictionary = new Dictionary<string, List<stockClass>>();
            foreach (var item in stockClasses)
            {
                if (dictionary.TryGetValue(item.Shelf_GUID, out List<stockClass> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.Shelf_GUID] = new List<stockClass> { item };
                }
            }
            return dictionary;
        }
        static public List<stockClass> GetByShelfGUID(this Dictionary<string, List<stockClass>> dict, string Shelf_GUID)
        {
            if (dict.TryGetValue(Shelf_GUID, out List<stockClass> stockClasses))
            {
                return stockClasses;
            }
            else
            {
                return new List<stockClass>();
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
