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

namespace BeanChat.Controllers
{
    public class LineChatController : ApiController
    {
        public string ChannelAccessToken = System.Environment.GetEnvironmentVariable("ChannelAccessToken");

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

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();
            try
            {
                var model = new AirQuality().GetList();
                response.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                response.Content = new StringContent(ex.Message, Encoding.UTF8);
            }
            return response;
        }

        [HttpPost]
        public IHttpActionResult POST()
        {
            string postData = Request.Content.ReadAsStringAsync().Result;
            //剖析JSON
            var messageObject = isRock.LineBot.Utility.Parsing(postData).events.FirstOrDefault();
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
                    var letouMsg = Letou.GetInstance().Compare(numbers);
                    reply = $"給你一組幸運號碼: {numbers}\\n";
                    reply += letouMsg;
                }
                else if (message == "大樂透機率")
                {
                    reply = Letou.GetInstance().GetHighRateNumbers();
                }
                else if (message.Contains("大樂透"))
                {
                    var numbers = message.Split(' ')?[1];
                    if (numbers != null && numbers.Split(',').Count()==6)
                        reply = Letou.GetInstance().Compare(numbers);
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
                return Ok();
            }
            catch (Exception ex)
            {
                //isRock.LineBot.Utility.ReplyMessage(messageObject.replyToken, $"{ex.Message}  \nitem.source.type {messageObject.source.type}", ChannelAccessToken);
                return Ok();
            }
        }

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
    }
}
