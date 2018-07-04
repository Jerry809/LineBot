﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BeanChat.Models;
using System.Text;
using System.Threading;
using BeanChat.Module;

namespace BeanChat.Controllers
{
    public class LetouController : ApiController
    {
        private readonly string path = HttpContext.Current.Server.MapPath("~/App_Data/Letou.json");

        [Route("api/GetNewLetou")]
        [HttpGet]
        public IEnumerable<LetouModel> GetNewLetou()
        {
            var crawler = new Crawler();
            crawler.GetLetou();
            return Letou.LetouList;
        }

        /// <summary>
        /// 取得特定日期的大樂透號碼
        /// </summary>
        /// <param name="date">日期 yyyyMMdd</param>
        /// <returns></returns>
        [Route("api/Letou/{date}")]
        [HttpGet]
        public LetouModel Get(string date)
        {
            return Letou.LetouList.FirstOrDefault(x => Convert.ToDateTime(x.Date).ToString("yyyyMMdd") == date);
        }

        /// <summary>
        /// 取得所有期數的大樂透號碼
        /// </summary>
        /// <returns></returns>
        [Route("api/Letou")]
        [HttpGet]
        public IEnumerable<LetouModel> Get()
        {
            return Letou.LetouList;
        }

    }
}
