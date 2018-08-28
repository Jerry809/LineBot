using BeanChat.Models;
using BeanChat.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BeanChat.Controllers
{
    public class EatController : ApiController
    {
        [Route("eat/{area}")]
        public async Task<List<TemplateModel>> Get(string area)
        {
            var eat = new Eat();
            var data = await eat.GetEatData(area);

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
                        uri = HttpUtility.UrlDecode(item.url)
                    },
                    actions = new List<ActionModel>() {
                            new UriModel()
                            {
                                label = "導航",
                                uri =  $"https://www.google.com.tw/maps/place/{item.address}"
                            }
                        }
                });
            }
            list.Add(model);
            return list;
        }
    }
}
