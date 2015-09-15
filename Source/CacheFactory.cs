namespace Tyresales.Web.Sitefinity.Service.Caching
{
    public class CacheFactory
    {
        private static ICacheManager manager;

        public static ICacheManager GetInstance
        {
            get{ return manager;}
        }

        static CacheFactory()
        {
            manager = CreateCacheManager();
        }

        private static ICacheManager CreateCacheManager()
        {
            return new CacheManager();
        }
    }
}
