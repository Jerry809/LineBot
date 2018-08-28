using BeanChat.Models;
using BeanChat.Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BeanChat.Controllers
{
    public class UBikeController : ApiController
    {
        // GET: api/UBike
        [Route("UBike/{city}")]
        public async Task<IEnumerable<UBikeRecord>> Get(string city)
        {
            IEnumerable<UBikeRecord> result = null;
            if (city.ToLower() == "taipei")
            {
                result = await UBike.Instance.GetTaipeiData();
            }
            else if (city.ToLower() == "newtaipei")
            {
                result = await UBike.Instance.GetNewTaipeiCityData();
            }
            else 
            {
                result = await UBike.Instance.GetAllData();
            }
            return result;
        }


    }
}
