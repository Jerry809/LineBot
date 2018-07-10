using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanChat.Models
{
    public class TemplateModel
    {
        public string type => "template";
        public string altText => "請在手機上瀏覽";
        public CarouselModel template { get; set; }
    }

    public class CarouselModel
    {
        public string type => "carousel";
        public string imageAspectRatio => "rectangle";
        public string imageSize => "contain";
        public List<ThumbnailImageModel> columns { get; set; }
    }

    public class ThumbnailImageModel
    {
        public string thumbnailImageUrl { get; set; }
        public string imageBackgroundColor => "#a8e8fb";
        public string title { get; set; }
        public string text { get; set; }
        public UriModel defaultAction { get; set; }
        public List<ActionModel> actions { get; set; }
    }

    public abstract class ActionModel
    {

    }

    public class UriModel : ActionModel
    {
        public string type => "uri";
        public string label { get; set; }
        public string uri { get; set; }
    }

    public class MessageModel : ActionModel
    {
        public string type => "message";
        public string label { get; set; }
        public string text { get; set; }
    }
}