using BeanChat.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace BeanChat.Module
{
    public class Letou
    {
        private static readonly string path = HttpContext.Current.Server.MapPath("~/App_Data/Letou.json");

        public static List<LetouModel> LetouList { get; set; }
        
        private Letou()
        {
            if (LetouList.Count == 0)
                LetouList = GetLetou();
        }

        private static List<LetouModel> GetLetou()
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
            return  JsonConvert.DeserializeObject<List<LetouModel>>(sb.ToString());
        }

        public static void RefreshLetou(List<LetouModel> letous)
        {
            bool flag = false;
            int count = 0;

            while (!flag && count <= 10)
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(letous.OrderByDescending(x => Convert.ToDateTime(x.Date))));
                    flag = true;
                    RefreshList();
                }
                catch
                {
                    Thread.Sleep(500);
                    count++;
                }
            }
        }

        private static void RefreshList()=> LetouList = GetLetou();
        
    }
}