using BeanChat.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace BeanChat.Module
{
    public class Eat
    {
        private const string url = "https://ifoodie.tw/api/blog/?order_by=-date&q={0}";
        private HttpClient Client;

        public Eat()
        {
            Client = new HttpClient();
        }

        public async Task<EatModel> GetEatData(string area)
        {
            var data = await Client.GetAsync(string.Format(url, HttpUtility.UrlEncode(area)));
            var content = await data.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EatModel>(content);
        }

    }
}