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
        private readonly string url = System.Environment.GetEnvironmentVariable("EatUrl");
        private HttpClient Client;

        public Eat()
        {
            Client = new HttpClient();
        }

        public async Task<EatModel> GetEatData(string area)
        {
            EatModel result = null;
            var random = new Random().Next(0, 10);
            random = random == 0 ? 1 : random;
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