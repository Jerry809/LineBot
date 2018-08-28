using BeanChat.Models;
using BeanChat.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BeanChat.Controllers
{
    public class WeatherController : ApiController
    {
        [Route("Weather")]
        public async Task<WeatherModel> Get()
        {
            return await Weather.Instance.GetData();
        }
    }
}
