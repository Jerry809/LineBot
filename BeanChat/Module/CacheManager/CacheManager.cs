using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace BeanChat.Module
{
    public class CacheManager
    {
        public string Key { get; set; }
        private ObjectCache _cache = MemoryCache.Default;

        public CacheManager(string key)
        {
            this.Key = key;
        }

        public bool IsExist(string key)
        { 
            return _cache[key] != null ? true : false;
        }

        public void Set(string item)
        {
            _cache.Set(this.Key, item, new CacheItemPolicy().AbsoluteExpiration.AddMinutes(10));
        }

        public void Set(string item , CacheItemPolicy policy)
        {
            _cache.Set(this.Key, item, policy);
        }

        public string Get(string key)
        {
            return _cache[key] != null ? _cache[key].ToString() : string.Empty;
        }
    }
}