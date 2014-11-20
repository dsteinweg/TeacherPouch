using System;
using System.Runtime.Caching;

namespace TeacherPouch.Utilities.Caching
{
    public static class CacheHelper
    {
        private static ObjectCache _cache = MemoryCache.Default;

        public static T RetrieveFromCache<T>(string key, Func<T> dataRetrieverMethod = null, string dependencyFilePath = null, TimeSpan? maxAge = null)
        {
            Guard.AgainstNullOrWhiteSpace("key", key);

            var maxAgeValue = maxAge.GetValueOrDefault();

            var existingCacheItem = _cache.Get(key) as CacheItem<T>;
            if (existingCacheItem != null)
            {
                return existingCacheItem.Value;
            }
            else if (dataRetrieverMethod != null)
            {
                var objectToCache = dataRetrieverMethod();

                if (objectToCache == null)
                    throw new Exception("Data retriever method returned null. Cannot cache null object.");

                var newCacheItem = Insert(key, objectToCache, dependencyFilePath, maxAge);

                return newCacheItem.Value;
            }
            else
            {
                return default(T);
            }
        }

        public static CacheItem<T> Insert<T>(string key, T objectToCache, string dependencyFilePath = null, TimeSpan? maxAge = null)
        {
            Guard.AgainstNullOrWhiteSpace("key", key);

            var maxAgeValue = maxAge.GetValueOrDefault();

            var cacheItem = new CacheItem<T>(key, objectToCache);
            var cacheItemPolicy = new CacheItemPolicy();

            if (!String.IsNullOrWhiteSpace(dependencyFilePath))
            {
                cacheItemPolicy.ChangeMonitors.Add(new HostFileChangeMonitor(new string[] { dependencyFilePath }));
            }
            
            if (maxAgeValue != TimeSpan.Zero)
            {
                cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.Add(maxAgeValue);
            }

            _cache.Add(key, cacheItem, cacheItemPolicy);

            return cacheItem;
        }
    }
}
