using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace BeanChat.Module
{
    public static class Beauty
    {
        private static readonly string path = HttpContext.Current.Server.MapPath("~/App_Data/Beauty.json");

        private static Dictionary<int, string> BeautyData { get; set; }

        public static Dictionary<int, string> GetBeauty()
        {
            if (BeautyData == null || BeautyData.Count == 0)
                GetData();
            
            return BeautyData;
        }        

        private static void GetData()
        {
            var sb = new StringBuilder();
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        sb.Append(reader.ReadLine());
                    }
                }
            }
            BeautyData = JsonConvert.DeserializeObject<Dictionary<int, string>>(sb.ToString());
        }
    }
}