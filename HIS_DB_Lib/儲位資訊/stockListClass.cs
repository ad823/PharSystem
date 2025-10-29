using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HIS_DB_Lib
{
    [Description("stockList")]
    public class stockListClass
    {
        /// <summary>
        /// 單位 GUID（唯一識別碼）
        /// </summary>
        [Description("VARCHAR,50,PRIMARY")]
        public string GUID { get; set; }

        /// <summary>
        /// 對應stock GUID
        /// </summary>
        [Description("VARCHAR,50,INDEX")]
        public string stock_GUID { get; set; }

        /// <summary>
        /// Value(deviceClass)
        /// </summary>
        [Description("VARCHAR,50,NONE")]
        public string Value { get; set; }
    }
}
