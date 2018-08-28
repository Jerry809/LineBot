using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Runtime.Caching;
using isRock.LineBot;
using System.Text.RegularExpressions;
using BeanChat.Module;
using BeanChat.Models;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Cryptography;

namespace BeanChat.Controllers
{
    public class LineChatController : ApiController
    {
        public string ChannelAccessToken = System.Environment.GetEnvironmentVariable("ChannelAccessToken");
        public string Secret = System.Environment.GetEnvironmentVariable("Secret");
        ObjectCache cache = MemoryCache.Default;

        internal string GetLetou()
        {
            var dic = new SortedDictionary<int, int>();
            var random = new Random();
            for (int i = 1; i <= 49; i++)
                dic.Add(random.Next(), i);

            int cnt = 0;
            int[] arr = new int[6];
            foreach (var item in dic.Keys)
            {
                arr[cnt] = dic[item];
                cnt++;
                if (cnt >= 6)
                    break;
            }


            return string.Join(", ", arr.OrderBy(x => x).ToArray());
        }

        private async Task<bool> VaridateSignature(HttpRequestMessage request)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret));
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(await request.Content.ReadAsStringAsync()));
            var contentHash = Convert.ToBase64String(computeHash);
            var headerHash = Request.Headers.GetValues("X-Line-Signature").First();

            return contentHash == headerHash;
        }       

        [HttpPost]
        public async Task<IHttpActionResult> POST()
        {
            string postData = Request.Content.ReadAsStringAsync().Result;
            //剖析JSON
            var messageObject = isRock.LineBot.Utility.Parsing(postData).events.FirstOrDefault();

            if (!await VaridateSignature(Request))
            {
                isRock.LineBot.Utility.ReplyMessage(messageObject.replyToken, "驗證失敗", ChannelAccessToken);
                return Ok();
            }

            //取得user說的話
            string message = messageObject.message.text;
            //回覆訊息
            string reply = string.Empty;
            //Message = "你說了:" + item.message.text;

            string id = string.Empty;
            LineUserInfo userInfo = GetUserInfoAndId(messageObject, out id);
            string ownKey = userInfo.userId + "Own";
            var regex = new Regex("\\D", RegexOptions.IgnoreCase);

            try
            {
                if (message == "大樂透")
                {
                    var numbers = GetLetou();
                    var letouMsg = Letou.Instance.Compare(numbers);
                    reply = $"給你一組幸運號碼: {numbers}\\n";
                    reply += letouMsg;
                }
                else if (message == "安安9527" || message == "9527安安" || message=="安安")
                {
                    Utility.ReplyMessageWithJSON(messageObject.replyToken, Demo.Show(), ChannelAccessToken);
                }
                else if (message.Contains("抽"))
                {
                    var beauty = Beauty.GetBeauty();
                    var random = new Random(new Random().Next(1, beauty.Count()));
                    var image = beauty[random.Next(1, beauty.Count())];
                    Utility.ReplyImageMessage(messageObject.replyToken, image, image, ChannelAccessToken);
                    return Ok();
                }
                else if (message.Contains("美食"))
                {
                    var eat = new Eat();
                    var data = await eat.GetEatData(message.Replace("美食", ""));

                    var list = new List<TemplateModel>();

                    var model = new TemplateModel();
                    model.template = new CarouselModel();
                    model.template.columns = new List<ThumbnailImageModel>();

                    foreach (var item in data.response.Where(x => x.restaurant != null).Take(10))
                    {
                        model.template.columns.Add(new ThumbnailImageModel()
                        {
                            thumbnailImageUrl = item.restaurant.cover_url,
                            title = item.restaurant.name,
                            text = item.address,
                            defaultAction = new UriModel()
                            {
                                label = "瀏覽網誌",
                                uri = item.url.IndexOf('-') > 0 ? item.url.Substring(0, item.url.IndexOf('-')) : item.url
                            },
                            actions = new List<ActionModel>() {
                            new UriModel()
                            {
                                label = "導航",
                                uri =  $"https://www.google.com.tw/maps/place/{item.restaurant.address}"
                            }
                        }
                        });
                    }
                    list.Add(model);

                    if (model.template.columns.Count > 0)
                        Utility.ReplyMessageWithJSON(messageObject.replyToken, JsonConvert.SerializeObject(list), ChannelAccessToken);
                    else
                        reply = "查無資料...";
                }
                else if (message.Contains("天氣"))
                {
                    var area = message.Replace("天氣", "").Replace("台", "臺").Trim();
                    var data = (await Weather.Instance.GetData()).records.location.FirstOrDefault(x => x.locationName.Contains(area));
                    var dic = new Dictionary<string, string>();
                    foreach (var item in data.weatherElement.OrderBy(x => x.elementName))
                    {
                        foreach (var time in item.time)
                        {
                            var key = $"{time.startTime}~{time.endTime}";
                            if (!dic.Keys.Contains(key))
                                dic.Add(key, "");

                            switch (item.elementName)
                            {
                                case "Wx":
                                    dic[key] += $"天氣:{time.parameter.parameterName}\\n";
                                    break;
                                case "PoP":
                                    dic[key] += $"降雨機率:{time.parameter.parameterName}%\\n";
                                    break;
                                case "MinT":
                                    dic[key] += $"低溫:{time.parameter.parameterName}C\\n";
                                    break;
                                case "CI":
                                    dic[key] += $"適度:{time.parameter.parameterName}\\n";
                                    break;
                                case "MaxT":
                                    dic[key] += $"高溫:{time.parameter.parameterName}C\\n";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    foreach (var item in dic)
                        reply += $"{item.Key}\\n{item.Value}";

                    if (reply == string.Empty)
                        reply = "查無資料... 試試台北天氣";

                }
                else if (message.ToLower().Contains("ubike"))
                {
                    var data = await UBike.Instance.GetAllData();
                    var mday = data.FirstOrDefault().mday;
                    var refreshTime = new DateTime(Convert.ToInt16(mday.Substring(0, 4)), Convert.ToInt16(mday.Substring(4, 2)),
                                                   Convert.ToInt16(mday.Substring(6, 2)), Convert.ToInt16(mday.Substring(8, 2)),
                                                   Convert.ToInt16(mday.Substring(10, 2)), Convert.ToInt16(mday.Substring(12, 2)));
                    var location = message.ToLower().Replace("ubike", "").Trim();
                    if (location == "三重家")
                    {
                        foreach (var item in data.Where(x => x.sno == "1008" || x.sno == "1010"))
                            reply += $"[{item.sarea}-{item.sna}]車{item.sbi}空{item.bemp}\\n";

                        if (reply == string.Empty)
                            reply = "查無資料...";
                        else
                            reply += $"更新時間{refreshTime.ToString("yyyy/MM/dd HH:mm:ss")}";
                    }
                    else if (location.Contains("區"))
                    {
                        foreach (var item in data.Where(x => x.sarea == location))
                            reply += $"[{item.sarea}-{item.sna}]車{item.sbi}空{item.bemp}\\n";

                        if (reply == string.Empty)
                            reply = "查無資料...";
                        else
                            reply += $"更新時間{refreshTime.ToString("yyyy/MM/dd HH:mm:ss")}";
                    }
                    else if (location != string.Empty)
                    {
                        foreach (var item in data.Where(x => x.sna.Contains(location)))
                            reply += $"[{item.sarea}-{item.sna}]車{item.sbi}空{item.bemp}\\n";

                        if (reply == string.Empty)
                            reply = "查無資料...";
                        else
                            reply += $"更新時間{refreshTime.ToString("yyyy/MM/dd HH:mm:ss")}";
                    }
                }
                else if (message == "大樂透機率")
                {
                    reply = Letou.Instance.GetHighRateNumbers();
                }
                else if (message.Contains("大樂透"))
                {
                    var numbers = message.Split(' ')?[1];
                    if (numbers != null && numbers.Split(',').Count() == 6)
                        reply = Letou.Instance.Compare(numbers);
                }
                //else if (LuisMaster.HitLuis(message,out string luisMessage))
                //{
                //    reply = $"{luisMessage}";
                //}
                else if (message.Contains("打球了"))
                {
                    bool isFriday = false;
                    DateTime date = DateTime.Now;

                    if (date.DayOfWeek == DayOfWeek.Friday)
                        reply = $"今晚7點半,高品打老虎";
                    else
                    {
                        while (!isFriday)
                        {
                            if (date.DayOfWeek == DayOfWeek.Friday)
                                isFriday = true;
                            else
                                date = date.AddDays(1);
                        }

                        reply = $"{date.ToString("yyyy/M/d")} 星期五 晚上7點半, 高品集合~~";
                    }
                }
                else if (message == "猜數字")
                {
                    reply = $"{userInfo.displayName} 開始猜數字 , 請輸入: 4位數字 (ex:1234) , 時間20分鐘";
                    var number = new GuessNum().GenNum();
                    var policy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20)
                    };
                    cache.Set(ownKey, number, policy);
                    cache.Set($"{ownKey}Count", 0, policy);
                    cache.Remove(id);
                }
                else if (!regex.IsMatch(message) && message.Length == 4 && cache[ownKey] != null && cache[id] == null)
                {
                    var ans = cache[ownKey].ToString();
                    int.TryParse(cache[$"{ownKey}Count"].ToString(), out int count);
                    count++;
                    var compare = new GuessNum().Compare(ans, message, out string errorMessage);
                    if (compare.A == 4)
                    {
                        reply = $"恭喜你猜對了 : {message} [你一共猜了{count}次]";
                        cache.Remove(ownKey);
                        cache.Remove($"{ownKey}Count");
                    }
                    else if (errorMessage != string.Empty)
                    {
                        reply += $"{errorMessage}";
                    }
                    else
                    {
                        reply += $"{message}==>{compare.A} A {compare.B} B ...己經猜了{count}次";
                        cache[$"{ownKey}Count"] = count;
                    }
                }
                else if (message == "一起猜")
                {
                    reply = $"大家開始猜數字 , 請輸入: 4位數字 (ex:1234) , 時間20分鐘";
                    var number = new GuessNum().GenNum();
                    var policy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20)
                    };
                    cache.Set(id, number, policy);
                    cache.Set($"{id}Count", 0, policy);
                }
                else if (!regex.IsMatch(message) && message.Length == 4 && cache[id] != null)
                {
                    var ans = cache[id].ToString();
                    int.TryParse(cache[$"{id}Count"].ToString(), out int count);
                    count++;
                    var compare = new GuessNum().Compare(ans, message, out string errorMessage);
                    if (compare.A == 4)
                    {
                        reply += $"恭喜猜對了 : {message} [一共猜了{count}次]" +
                        cache.Remove(id);
                        cache.Remove($"{id}Count");
                    }
                    else if (errorMessage != string.Empty)
                    {
                        reply += $"{errorMessage}";
                    }
                    else
                    {
                        reply += $"{message}==>{compare.A} A {compare.B} B ...己經猜了{count}次";
                        cache[$"{id}Count"] = count;
                    }

                }
                else if (message.Contains("PM2.5"))
                {
                    var word = message.Split('的');
                    if (word.Length == 2 && word[1].Contains("PM2.5"))
                    {
                        var area = word[0].Replace("台", "臺");
                        var air = new AirQuality();
                        var model = air.GetList().Where(x => x.county.Contains(area) || x.Site.Contains(area));
                        if (model.Count() > 0)
                        {
                            foreach (var item in model)
                            {
                                reply += $"{item.county}{item.Site}的PM2.5為[{item.PM25}], {item.Status}\\n";
                            }
                            reply += $"更新時間:{model.FirstOrDefault().DataCreationDate.Value}";
                        }
                        else
                        {
                            reply = "查無資料 , 有可能打錯關鍵字, 可以試試 新北市的PM2.5 或 三重的PM2.5";
                        }
                    }
                }

                //回覆API OK       
                isRock.LineBot.Utility.ReplyMessage(messageObject.replyToken, $"{reply}", ChannelAccessToken);

                //Utility.ReplyMessageWithJSON(messageObject.replyToken,FlexMessage(), ChannelAccessToken);
                return Ok();
            }
            catch (Exception ex)
            {
                //isRock.LineBot.Utility.ReplyMessage(messageObject.replyToken, $"{ex.Message}  \nitem.source.type {messageObject.source.type}", ChannelAccessToken);
                return Ok();
            }
        }

        //private async Task<bool> VaridateSignature(HttpRequestMessage request)
        //{
        //    var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["ChannelSecret"].ToString()));
        //    var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(await request.Content.ReadAsStringAsync()));
        //    var contentHash = Convert.ToBase64String(computeHash);
        //    var headerHash = Request.Headers.GetValues("X-Line-Signature").First();

        //    return contentHash == headerHash;
        //}

        private LineUserInfo GetUserInfoAndId(Event message, out string id)
        {
            switch (message.source.type)
            {
                case "room":
                    id = message.source.roomId;
                    return isRock.LineBot.Utility.GetRoomMemberProfile(message.source.roomId, message.source.userId, ChannelAccessToken);
                case "group":
                    id = message.source.groupId;
                    return isRock.LineBot.Utility.GetGroupMemberProfile(message.source.groupId, message.source.userId, ChannelAccessToken);
                case "user":
                    id = message.source.userId;
                    return isRock.LineBot.Utility.GetUserInfo(message.source.userId, ChannelAccessToken);
                default:
                    id = "";
                    return null;
            }
        }

        private static string FlexMessage()
        {
            var Flex = @"
[
    {
      ""type"": ""flex"",
      ""altText"": ""This is a Flex Message"",
      ""contents"": 
{
  ""type"": ""bubble"",
  ""header"": {
                ""type"": ""box"",
    ""layout"": ""horizontal"",
    ""contents"": [
      {
        ""type"": ""text"",
        ""text"": ""NEWS DIGEST"",
        ""weight"": ""bold"",
        ""color"": ""#aaaaaa"",
        ""size"": ""sm""
      }
    ]
  },
  ""hero"": {
    ""type"": ""image"",
    ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/01_4_news.png"",
    ""size"": ""full"",
    ""aspectRatio"": ""20:13"",
    ""aspectMode"": ""cover"",
    ""action"": {
      ""type"": ""uri"",
      ""uri"": ""http://linecorp.com/""
    }
  },
  ""body"": {
    ""type"": ""box"",
    ""layout"": ""horizontal"",
    ""spacing"": ""md"",
    ""contents"": [
      {
        ""type"": ""box"",
        ""layout"": ""vertical"",
        ""flex"": 1,
        ""contents"": [
          {
            ""type"": ""image"",
            ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/02_1_news_thumbnail_1.png"",
            ""aspectMode"": ""cover"",
            ""aspectRatio"": ""4:3"",
            ""size"": ""sm"",
            ""gravity"": ""bottom""
          },
          {
            ""type"": ""image"",
            ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/02_1_news_thumbnail_2.png"",
            ""aspectMode"": ""cover"",
            ""aspectRatio"": ""4:3"",
            ""margin"": ""md"",
            ""size"": ""sm""
          }
        ]
      },
      {
        ""type"": ""box"",
        ""layout"": ""vertical"",
        ""flex"": 2,
        ""contents"": [
          {
            ""type"": ""text"",
            ""text"": ""7 Things to Know for Today"",
            ""gravity"": ""top"",
            ""size"": ""xs"",
            ""flex"": 1
          },
          {
            ""type"": ""separator""
          },
          {
            ""type"": ""text"",
            ""text"": ""Hay fever goes wild"",
            ""gravity"": ""center"",
            ""size"": ""xs"",
            ""flex"": 2
          },
          {
            ""type"": ""separator""
          },
          {
            ""type"": ""text"",
            ""text"": ""LINE Pay Begins Barcode Payment Service"",
            ""gravity"": ""center"",
            ""size"": ""xs"",
            ""flex"": 2
          },
          {
            ""type"": ""separator""
          },
          {
            ""type"": ""text"",
            ""text"": ""LINE Adds LINE Wallet"",
            ""gravity"": ""bottom"",
            ""size"": ""xs"",
            ""flex"": 1
          }
        ]
      }
    ]
  },
  ""footer"": {
    ""type"": ""box"",
    ""layout"": ""horizontal"",
    ""contents"": [
      {
        ""type"": ""button"",
        ""action"": {
          ""type"": ""uri"",
          ""label"": ""More"",
          ""uri"": ""https://linecorp.com""
        }
      }
    ]
  }
}
    }
  ]
";

            return Flex;
        }
    }
}
