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
        private const string url = "https://ifoodie.tw/api/blog/?offset={0}&limit={1}&order_by=-date&q={2}";
        private HttpClient Client;

        public Eat()
        {
            Client = new HttpClient();
        }

        public async Task<EatModel> GetEatData(string area)
        {
            EatModel result = null;
            var random = new Random().Next(0, 10);
            int offset = random * 20;
            int limit = 20;
            int count = 0;

            while (count == 0 && offset > 0)
            {
                var data = await Client.GetAsync(string.Format(url, offset, limit, HttpUtility.UrlEncode(area)));
                var content = await data.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<EatModel>(content);
                count = result.response.Count;
                offset -= 20;
            }

            return result;
        }

    }
}