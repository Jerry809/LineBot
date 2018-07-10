using BeanChat.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanChat.Module
{
    public static class Demo
    {
        public static Dictionary<string, string> Menu = new Dictionary<string, string>() {
            {"美食","https://images.unsplash.com/photo-1506280754576-f6fa8a873550" } ,
            {"天氣","https://images.unsplash.com/photo-1447311158516-a2578c22260d" },
            {"ubike","https://images.unsplash.com/photo-1487557231561-e697e7b52a66" },
            {"猜數字" ,"https://images.unsplash.com/photo-1524932114343-c195623b9c8a" },
            {"大樂透", "https://images.unsplash.com/photo-1518688248740-7c31f1a945c4" },
            {"抽卡", "https://images.unsplash.com/photo-1496671431883-c102df9ae8f9" }
        };

        public static string Show()
        {
            var list = new List<TemplateModel>();

            var model = new TemplateModel();
            model.template = new CarouselModel();
            model.template.columns = new List<ThumbnailImageModel>();

            foreach (var key in Menu.Keys)
            {
                var imageModel = new ThumbnailImageModel()
                {
                    thumbnailImageUrl = Menu[key],
                    title = key,
                    text = key,
                    defaultAction = new UriModel()
                    {
                        label = key,
                        uri = Menu[key]
                    },
                    actions = new List<ActionModel>()
                };

                imageModel.actions.AddRange(GetAction(key));

                model.template.columns.Add(imageModel);

            }
            list.Add(model);
            return JsonConvert.SerializeObject(list);
        }

        internal static List<ActionModel> GetAction(string key)
        {
            var result = new List<ActionModel>();
            switch (key)
            {
                case "美食":
                    result.Add(new MessageModel() { label="地點+美食", text="台北美食" });
                    result.Add(new MessageModel() { label = "地點+類別+美食", text = "台北早午餐美食" });
                    break;
                case "天氣":
                    result.Add(new MessageModel() { label = "城市+天氣", text = "台北天氣" });
                    result.Add(new MessageModel() { label = "城市+天氣", text = "台南天氣" });
                    break;
                case "ubike":
                    result.Add(new MessageModel() { label = "地區+ubike", text = "板橋區ubike" });
                    result.Add(new MessageModel() { label = "特定字+ubike", text = "公館ubike" });
                    break;
                case "猜數字":
                    result.Add(new MessageModel() { label = "個人猜數字", text = "猜數字" });
                    result.Add(new MessageModel() { label = "一起猜數字", text = "一起猜" });
                    break;
                case "大樂透":
                    result.Add(new MessageModel() { label = "取得一組大樂透號碼", text = "大樂透" });
                    result.Add(new MessageModel() { label = "查詢一組大槳透號碼", text = "大樂透 1,2,3,4,5,6" });
                    break;
                case "抽卡":
                    result.Add(new MessageModel() { label = "抽卡", text = "抽" });
                    result.Add(new MessageModel() { label = "抽卡", text = "抽" });
                    break;
            }
            return result;
        }
    }
}