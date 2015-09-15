using System;
using System.Web;
using System.Web.Caching;

namespace Tyresales.Web.Sitefinity.Service.Caching
{
    public class CacheManager : ICacheManager
    {
        private object thisLock = new object();

        public T Get<T>(string key, Func<T> callback)
        {
            return CacheManagerAsync.Get(key, callback);
        }

        public T Get<T>(string key)
        {
            T item;

            if (HttpRuntime.Cache.Get(key) != null)
            {
                item = (T)HttpRuntime.Cache.Get(key);
            }
            else
            {
                item = default(T);
            }

            return item;
        }

        public void Insert(string key, object value)
        {
            Cache cache = HttpRuntime.Cache;
            if (key != null && value != null)
            {
                cache.Add(key, value, null, DateTime.Now.AddMinutes(240), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            }
        }

        /// <summary>
        /// Insert item into cache and also define a callback for when it expires
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="callback"></param>
        public void Insert(string key, object value, CacheItemRemovedCallback callback)
        {
        }
    }
}
