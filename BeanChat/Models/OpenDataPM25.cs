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
        public DateTime? DataCreationDate { get; set; }
        public string ItemUnit { get; set; }

        public string Status
        {
            get
            {
                if (Convert.ToInt16(this.PM25) >= 25)
                    return "空氣品質不良";
                else if (Convert.ToInt16(this.PM25) <= 12)
                    return "空氣品質良好";
                else
                    return "空氣品質普通";
            }
        }
    }
}