using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;

namespace TeacherPouch.Utilities.Caching
{
    public static class CacheHelper
    {
        private static Cache _cache = HttpRuntime.Cache;

        public static T RetrieveFromCache<T>(string key, Func<T> dataRetrieverMethod = null, CacheDependency dependencies = null, TimeSpan? maxAge = null)
        {
            var maxAgeValue = maxAge.GetValueOrDefault();

            if ((_cache[key] != null) && // If the local cache contains the item we're looking for
                (
                    (maxAgeValue == TimeSpan.Zero) || // and either the maxAge is unspecified
                    ((CacheItem<T>)_cache[key]).DateTimeInserted >= DateTime.Now.Subtract(maxAgeValue)) // or the item in cache is newer than the maxAge specified
                )
            {
                // The output window gets pretty cluttered when you log each cache hit, but this is still useful in some debugging scenarios
                // Debug.WriteLine("CacheHelper - Cache Hit - " + key);

                return ((CacheItem<T>)_cache[key]).Value;
            }
            else if (dataRetrieverMethod != null)
            {
                Debug.WriteLine("CacheHelper - Cache Miss - " + key);

                var objectToCache = dataRetrieverMethod();

                if (objectToCache == null)
                    throw new ApplicationException("Data retriever method returned null. Cannot cache null object.");

                var cacheItem = Insert(key, objectToCache, dependencies, maxAge);

                return cacheItem.Value;
            }
            else
            {
                return default(T);
            }
        }

        public static CacheItem<T> Insert<T>(string key, T objectToCache, CacheDependency dependencies = null, TimeSpan? maxAge = null)
        {
            var maxAgeValue = maxAge.GetValueOrDefault();

            var cacheItem = new CacheItem<T>(key, objectToCache, dependencies, maxAge.GetValueOrDefault());

            _cache.Insert(
                key,
                cacheItem,
                dependencies,
                ((maxAgeValue == TimeSpan.Zero) ? Cache.NoAbsoluteExpiration : DateTime.UtcNow.Add(maxAgeValue)),
                Cache.NoSlidingExpiration
            );

            return cacheItem;
        }

        public static object RemoveFromLocalCache(string key)
        {
            object returnValue = _cache.Remove(key);

            Debug.WriteLine("CacheHelper - Remove - Key: [" + key + "] - Item Found: [" + ((returnValue == null) ? "No" : "Yes") + "]");

            return returnValue;
        }

        public static int RemoveAllItemsFromLocalCache()
        {
            var keys = GetAllCacheItemKeys();

            foreach (var key in keys)
            {
                RemoveFromLocalCache(key);
            }

            return keys.Count;
        }

        public static bool ItemExistsInCache(string key)
        {
            return (_cache.Get(key) != null);
        }

        public static List<CacheItem<object>> GetAllCacheItems()
        {
            List<CacheItem<object>> cacheItems = new List<CacheItem<object>>(_cache.Count);

            foreach (string key in GetAllCacheItemKeys())
            {
                var cacheItem = _cache[key] as CacheItem<object>;

                if (cacheItem != null)
                    cacheItems.Add(cacheItem);
            }

            return cacheItems;
        }

        public static List<string> GetAllCacheItemKeys()
        {
            var keys = new List<string>(_cache.Count);

            var cacheIterator = _cache.GetEnumerator();
            while (cacheIterator.MoveNext())
            {
                if (cacheIterator.Value is CacheItem<object> && cacheIterator.Key is string)
                    keys.Add(cacheIterator.Key as string);
            }

            keys.Sort();

            return keys;
        }
    }
}
