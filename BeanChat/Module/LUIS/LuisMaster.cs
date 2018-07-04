using Microsoft.Cognitive.LUIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanChat.Module
{
    public class LuisMaster
    {
        private static string LuisAppId = System.Environment.GetEnvironmentVariable("LuisAppId");
        private static string LuisAppKey = System.Environment.GetEnvironmentVariable("LuisAppKey");
        private static string Luisdomain = System.Environment.GetEnvironmentVariable("Luisdomain");

        public static bool HitLuis(string lineMessage, out string message)
        {

            message = string.Empty;
            //return false;
            //建立LuisClient
            var lc = new LuisClient(LuisAppId, LuisAppKey, true, Luisdomain);

            //Call Luis API 查詢
            var ret = lc.Predict(lineMessage).Result;
            if (ret.Intents.Count() <= 0)
                return false;
            else if (ret.TopScoringIntent.Name == "None")
                return false;
            else
            {
                message = $"OK，我知道你想 '{ret.TopScoringIntent.Name}'";
                if (ret.Entities.Count > 0)
                    message += $"對象 {ret.Entities.FirstOrDefault().Value.FirstOrDefault().Value}";
                return true;
            }
        }
    }
}