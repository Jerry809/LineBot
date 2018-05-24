using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanChat.Models
{
    public class OpenDataPM25
    {
        public string Site { get; set; }
        public string county { get; set; }
        public string PM25 { get; set; }
        public string DataCreationDate { get; set; }
        public string ItemUnit { get; set; }

        public string Status(string PM25)
        {
            if (Convert.ToInt16(PM25) >= 25)
                return "不良";
            else if (Convert.ToInt16(PM25) <= 12)
                return "良好";
            else
                return "普通";
        }
    }
}