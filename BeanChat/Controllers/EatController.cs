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
    public class EatController : ApiController
    {
        // GET: api/Eat
        [Route("~/api/eat/{area}")]
        public async Task<EatModel> Get(string area)
        {
            var eat = new Eat();
            return await eat.GetEatData(area);
        }

        public async Task<List<TemplateModel>> Get()
        {
            var eat = new Eat();
            var data = await eat.GetEatData("三重");

            var list = new List<TemplateModel>();

            var model = new TemplateModel();
            model.template = new CarouselModel();
            model.template.columns = new List<ThumbnailImageModel>();

            foreach (var item in data.response.Where(x => x.restaurant != null).Take(3))
            {
                model.template.columns.Add(new ThumbnailImageModel()
                {
                    thumbnailImageUrl = item.restaurant.cover_url,
                    title = item.restaurant.name,
                    text = item.address,
                    defaultAction = new UriModel()
                    {
                        label = "瀏覽網誌",
                        uri = item.url
                    },
                    actions = new List<UriModel>() {
                            new UriModel()
                            {
                                label = "導航",
                                uri =  $"https://www.google.com.tw/maps/place/{item.lat},{item.lng}"
                            }
                        }
                });
            }
            list.Add(model);

            //var list = new List<TemplateModel>();

            //var model = new TemplateModel();
            //model.template = new CarouselModel();
            //model.template.columns = new List<ThumbnailImageModel>();
            //model.template.columns.Add(new ThumbnailImageModel()
            //{
            //    thumbnailImageUrl = "https://lh3.googleusercontent.com/4b4HA5R94a4xeljMuskixfjJvuhcvRyKcqqLM-_ouckm4zp_lxXEMR3H6UyofG0xnEnEv_WHfwupbK_5AQ5XkaunHFfHIB3J=s600",
            //    title = "title",
            //    text = "text",
            //    defaultAction = new UriModel()
            //    {
            //        label = "打開地圖",
            //        uri = "https://www.google.com.tw/maps/place/23.230344,120.353937"
            //    },
            //    actions = new List<UriModel>() {
            //                new UriModel()
            //                {
            //                    label = "打開地圖",
            //                    uri = "https://www.google.com.tw/maps/place/23.230344,120.353937"
            //                },
            //                 new UriModel()
            //                {
            //                    label = "瀏覽網誌",
            //                    uri = "https://www.google.com.tw/maps/place/23.230344,120.353937"
            //                }
            //            }
            //});

            //list.Add(model);
            return list;
        }
    }
}
