using BeanChat.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace BeanChat.Module
{
    public class Crawler
    {
        private static object _lock = new object();
        private static readonly Lazy<Crawler> LazyInstance = new Lazy<Crawler>(() => new Crawler());
        public static Crawler Instance => LazyInstance.Value;

        private Crawler()
        {

        }        

        public void GetNewLetouData()
        {
            bool refresh = false;
            var data = Letou.Instance.LetouList;
            bool flag = false;
            int count = 1;
            var list = new List<LetouModel>();

            while (!flag)
            {
                WebClient client = new WebClient();
                MemoryStream ms = new MemoryStream(client.DownloadData($"http://www.lotto-8.com/listltobig.asp?indexpage={count}&orderby=new"));

                // 使用預設編碼讀入 HTML
                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms, Encoding.UTF8);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("/html[1]/body[1]/table[3]/tr[1]/td[1]/table[2]/tr");

                foreach (HtmlNode node in nodes)
                {
                    var td = node.SelectNodes("td[2]").FirstOrDefault().InnerHtml;
                    if (td == "大樂透中獎號碼")
                        continue;
                    try
                    {
                        var date = node.SelectNodes("td[1]").FirstOrDefault().InnerHtml;
                        var number = td.Replace("&nbsp;", "");
                        var special = node.SelectNodes("td[3]").FirstOrDefault().InnerHtml;

                        if (data.FirstOrDefault(x => Convert.ToDateTime(x.Date) == Convert.ToDateTime(date)) != null)
                        {
                            flag = true;
                            break;
                        }
                        else
                        {
                            data.Add(new LetouModel() { Date = Convert.ToDateTime(date).ToString("yyyy/M/d"), Numbers = number, Special = special });
                            refresh = true;
                        }

                    }
                    catch
                    {

                    }
                }
                doc = null;
                nodes = null;
                count++;
            }

            if (refresh)
                Letou.Instance.RefreshLetou(data);

        }
    }
}