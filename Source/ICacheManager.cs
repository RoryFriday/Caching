using System;

namespace Tyresales.Web.Sitefinity.Service.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key, Func<T> callback);
        T Get<T>(string key);
        void Insert(string key, object value);
    }
}
