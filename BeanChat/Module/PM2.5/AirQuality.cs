using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BeanChat.Models;
using System.Runtime.Caching;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BeanChat.Module
{
    public class AirQuality
    {
        private const string openDataUrl = "https://opendata.epa.gov.tw/ws/Data/ATM00625/?$format=json";
        private const string key = "AirOpenData";
        private List<OpenDataPM25> List { get; set; }
        ObjectCache _cache = MemoryCache.Default;        
        
        public AirQuality()
        {
            if (_cache[key] == null)
            {
                List = GetData().Result;
                _cache.Set(key, List, new CacheItemPolicy().AbsoluteExpiration.AddHours(1));
            }
            else
            {
                List = _cache[key] as List<OpenDataPM25>;
                if (List.FirstOrDefault().DataCreationDate.Value.Hour < DateTime.Now.Hour)
                {
                    List = GetData().Result;
                    _cache.Set(key, List, new CacheItemPolicy().AbsoluteExpiration.AddHours(1));
                }
            }            
        }

        public List<OpenDataPM25> GetList()
        {
            return List;
        }

        private async Task<List<OpenDataPM25>> GetData()
        {
            var client = new HttpClient();
            var result = await client.GetAsync(openDataUrl);
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<OpenDataPM25>>(content);
        }
    }
}