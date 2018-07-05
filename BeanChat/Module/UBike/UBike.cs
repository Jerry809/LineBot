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
    public class UBike
    {
        private static readonly Lazy<UBike> LazyInstance = new Lazy<UBike>(()=>new UBike());
        public static UBike Instance => LazyInstance.Value;
        private const string OdataUBikeNewTaipeiCityUrl= "http://data.ntpc.gov.tw/api/v1/rest/datastore/382000000A-000352-001";
        private const string OdataUBikeTaipeiUrl = "https://tcgbusfs.blob.core.windows.net/blobyoubike/YouBikeTP.gz";

        private UBike()
        {

        }

        public async Task<IEnumerable<UBikeRecord>> GetAllData()
        {
            var data = (await GetNewTaipeiCityData()).Concat(await GetTaipeiData());
            return data;
        }

        public async Task<IEnumerable<UBikeRecord>> GetNewTaipeiCityData()
        {
            var client = await new HttpClient().GetAsync(OdataUBikeNewTaipeiCityUrl);
            var result = await client.Content.ReadAsStringAsync();
            var ubike = JsonConvert.DeserializeObject<UBikeModel>(result);
            return ubike.result.records.OrderBy(x => x.sna);
        }

        public async Task<IEnumerable<UBikeRecord>> GetTaipeiData()
        {
            var result = new List<UBikeRecord>();

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = await new HttpClient(handler).GetAsync(OdataUBikeTaipeiUrl);
            var response = await client.Content.ReadAsStringAsync();
            var ubike = JsonConvert.DeserializeObject<TaipeiUBikeModel>(response);
            foreach (var item in ubike.retVal)
                result.Add(item.Value);

            return result.OrderBy(x=>x.sna);
        }
    }
}