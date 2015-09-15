using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;

namespace Tyresales.Web.Sitefinity.Service.Caching
{
    public class CacheManagerTimeout : ICacheManager
    {
        private object thisLock = new object();

        public T Get<T>(string key, Func<T> callback)
        {
            //T item;

            if (Monitor.TryEnter(thisLock, new TimeSpan(0, 0, 20)))
            {
                T item;

                try
                {
                    if (HttpRuntime.Cache.Get(key) != null)
                    {
                        item = (T)HttpRuntime.Cache.Get(key);
                    }
                    else
                    {
                        item = callback();
                        Insert(key, item);
                    }
                }
                finally
                {
                    Monitor.Exit(thisLock);
                }

                return item;
            }

            return callback();
            //return item;
        }

        public void Insert(string key, object value)
        {
            Cache cache = HttpRuntime.Cache;
            if (key != null && value != null)
            {
                cache.Add(key, value, null, DateTime.Now.AddMinutes(60), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            }
        }
    }
}
