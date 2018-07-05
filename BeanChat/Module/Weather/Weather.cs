using BeanChat.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace BeanChat.Module
{
    public class Weather
    {
        private const string url = "https://opendata.cwb.gov.tw/api/v1/rest/datastore/F-C0032-001?Authorization=rdec-key-123-45678-011121314";
        private static readonly Lazy<Weather> LazyInstance = new Lazy<Weather>(() => new Weather());
        public static Weather Instance => LazyInstance.Value;
        private static DateTime FirstTime { get; set; }
        private static WeatherModel Data { get; set; }

        private Weather()
        {
        }

        public async Task<WeatherModel> GetData()
        {
            if (Refresh())
                await SetData();

            return Data;
        }

        private async Task SetData()
        {
            Data = await GetRemoteData();
            SetFirstTime();
        }

        private async Task<WeatherModel> GetRemoteData()
        {
            string content;
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client = new HttpClient(handler))
            {
                content = await (await client.GetAsync(url)).Content.ReadAsStringAsync();
            }
            var data = JsonConvert.DeserializeObject<WeatherModel>(content);
            return data;
        }

        private void SetFirstTime()
        {
            FirstTime = Convert.ToDateTime(Data?.records.location.FirstOrDefault().weatherElement.FirstOrDefault().time.FirstOrDefault().endTime);
        }

        private bool Refresh()
        {
            if (Data == null)
                return true;
            else if (DateTime.Now > FirstTime)
                return true;

            return false;
        }
    }
}