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
        private static readonly Lazy<Letou> LazyInstance = new Lazy<Letou>(() => new Letou());
        private static object _lock = new object();
        private static readonly string path = HttpContext.Current.Server.MapPath("~/App_Data/Letou.json");
        public static Letou Instance { get { return LazyInstance.Value; } }

        public List<LetouModel> LetouList { get; set; }

        private Letou()
        {
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
            return JsonConvert.DeserializeObject<List<LetouModel>>(sb.ToString());
        }

        public string Compare(string input)
        {
            string message = string.Empty;
            var counter = 0;
            bool special = false;
            var hit = new List<Hit>();
            var arr = Array.ConvertAll<string, int>(input.Split(','), int.Parse); ;

            foreach (LetouModel letou in LetouList)
            {
                foreach (var num in letou.Numbers.Split(','))
                {
                    if (arr.Contains(Convert.ToInt16(num)))
                        counter++;
                }

                if (arr.Contains(Convert.ToInt16(letou.Special)))
                    counter++;

                if (counter > 0)
                {
                    hit.Add(new Hit()
                    {
                        Date = letou.Date,
                        Total = counter,
                        Numbers = letou.Numbers,
                        Special = letou.Special
                    });
                }

                counter = 0;
                special = false;
            }

            var max = hit.Max(x => x.Total);
            var result = hit.Where(x => x.Total == max);

            message = $"此組號碼最多中{max}個號碼,共{result.Count()}次\\n";
            foreach (var item in result.Take(10))
            {
                message += $"[{item.Date}][{item.Numbers}][特{item.Special}]\\n";
            }
            return message;
        }

        public string GetHighRateNumbers()
        {
            string message = string.Empty;
            float totalCount = LetouList.Count();
            var dic = new Dictionary<int, float>();
            foreach (var item in LetouList)
            {
                foreach (var num in item.Numbers.Split(','))
                {
                    var n = Convert.ToInt16(num);
                    if (dic.Keys.Contains(n))
                        dic[n]++;
                    else
                        dic.Add(n, 1);
                }

                var special = Convert.ToInt16(item.Special);
                if (dic.Keys.Contains(special))
                    dic[special]++;
                else
                    dic.Add(special, 1);
            }

            message = $"共{totalCount}次開獎\n";
            foreach (var item in dic.OrderByDescending(x=>x.Value))
                message += $"{item.Key}開過{item.Value}次[{(item.Value/ totalCount).ToString("F2")}%]\n";
            
            return message;
        }

        public void RefreshLetou(List<LetouModel> letous)
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

        private static void RefreshList() => Instance.LetouList = GetLetou();

        private class Hit
        {
            public string Date { get; set; }
            public string Numbers { get; set; }
            public string Special { get; set; }
            public int Total { get; set; }
        }

    }


}