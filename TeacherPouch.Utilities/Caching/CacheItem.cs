using System;
using System.Runtime.Caching;

namespace TeacherPouch.Utilities.Caching
{
    public class CacheItem<T>
    {
        public CacheItem InnerCacheItem { get; private set; }

        public string Key { get { return this.InnerCacheItem.Key; } }
        public T Value { get { return (T)this.InnerCacheItem.Value; } }

        public DateTime Inserted { get; set; }


        public CacheItem(string key, T value)
        {
            this.InnerCacheItem = new CacheItem(key, value);
            this.Inserted = DateTime.Now;
        }
    }
}
