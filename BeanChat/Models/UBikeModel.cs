using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanChat.Models
{
    public class UBikeModel
    {
        public bool success { get; set; }
        public Result result { get; set; }
    }

    public class TaipeiUBikeModel
    {
        public string retCode { get; set; }
        public Dictionary<string, UBikeRecord> retVal { get; set; }
    }

    public class Result
    {
        public string resource_id { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public List<Field> fields { get; set; }
        public List<UBikeRecord> records { get; set; }
    }

    public class Field
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class UBikeRecord
    {
        /// <summary>
        /// 站點代號
        /// </summary>
        public string sno { get; set; }
        /// <summary>
        /// 場站名稱(中文)
        /// </summary>
        public string sna { get; set; }
        /// <summary>
        /// 場站總停車格
        /// </summary>
        public string tot { get; set; }
        /// <summary>
        /// 場站目前車輛數量
        /// </summary>
        public string sbi { get; set; }
        /// <summary>
        /// 場站區域(中文)
        /// </summary>
        public string sarea { get; set; }
        /// <summary>
        /// 資料更新時間
        /// </summary>
        public string mday { get; set; }
        /// <summary>
        /// 緯度
        /// </summary>
        public string lat { get; set; }
        /// <summary>
        /// 經度
        /// </summary>
        public string lng { get; set; }
        /// <summary>
        /// 地(中文)
        /// </summary>
        public string ar { get; set; }
        /// <summary>
        /// 場站區域(英文)
        /// </summary>
        public string sareaen { get; set; }
        /// <summary>
        /// 場站名稱(英文)
        /// </summary>
        public string snaen { get; set; }
        /// <summary>
        /// 地址(英文)
        /// </summary>
        public string aren { get; set; }
        /// <summary>
        /// 空位數量
        /// </summary>
        public string bemp { get; set; }
        /// <summary>
        /// 全站禁用狀態
        /// </summary>
        public string act { get; set; }
    }      
}