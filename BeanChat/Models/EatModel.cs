using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanChat.Models
{
    public class EatModel
    {
        public Info info { get; set; }
        public List<EatBody> response { get; set; }
        public bool success { get; set; }
    }

    public class Info
    {
        public double range { get; set; }
    }

    public class Ranking
    {
    }

    public class Stat
    {
        public string blog_id { get; set; }
        public int recommend_cnt { get; set; }
        public int browse_cnt { get; set; }
        public double score { get; set; }
        public int share_cnt { get; set; }
        public int favorite_cnt { get; set; }
    }

    public class User
    {
        public int fav_cnt { get; set; }
        public string profile_pic { get; set; }
        public int follower_cnt { get; set; }
        public string display_name { get; set; }
        public string thumb { get; set; }
        public int following_cnt { get; set; }
        public int post_cnt { get; set; }
        public string cover_url { get; set; }
        public string profile_pic_origin { get; set; }
        public int browse_cnt { get; set; }
        public int checkin_cnt { get; set; }
        public bool is_following { get; set; }
        public bool is_vip { get; set; }
        public string id { get; set; }
        public bool certified { get; set; }
    }

    public class Restaurant
    {
        public string city { get; set; }
        public string name { get; set; }
        public double last_visit { get; set; }
        public double rating { get; set; }
        public string opening_hours { get; set; }
        public string address { get; set; }
        public int blog_cnt { get; set; }
        public string cover_url { get; set; }
        public string cover_author { get; set; }
        public string admin_name { get; set; }
        public string postal_code { get; set; }
        public bool open_now { get; set; }
        public int avg_price { get; set; }
        public double lat { get; set; }
        public int visit_cnt { get; set; }
        public double lng { get; set; }
        public int price_level { get; set; }
        public string id { get; set; }
        public List<string> categories { get; set; }
        public string phone { get; set; }
    }

    public class EatBody
    {
        public double rating { get; set; }
        public object deal { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string id { get; set; }
        public string city { get; set; }
        public int blog_type { get; set; }
        public string thumb { get; set; }
        public string author { get; set; }
        public string source { get; set; }
        public List<double> location { get; set; }
        public int status { get; set; }
        public Ranking ranking { get; set; }
        public Stat stat { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public string phone { get; set; }
        public User user { get; set; }
        public string address { get; set; }
        public double date { get; set; }
        public bool tracking { get; set; }
        public Restaurant restaurant { get; set; }
        public string url { get; set; }
        public bool is_paid { get; set; }
        public string area { get; set; }
        public List<object> posts { get; set; }
        public object tracking_id { get; set; }
        public string title { get; set; }
    }

   
}