using BeanChat.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BeanChat.Controllers
{
    public class BeautyController : ApiController
    {
        // GET: api/Beauty
        public Dictionary<int,string> Get()
        {
            return Beauty.GetBeauty();
        }        
    }
}
