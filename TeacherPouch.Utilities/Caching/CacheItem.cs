using System;
using System.Web.Caching;

namespace TeacherPouch.Utilities.Caching
{
    public class CacheItem<T>
    {
        public string Key { get; set; }
        public T Value { get; set; }

        public DateTime DateTimeInserted { get; set; }
        public bool HasDependency { get; set; }
        public TimeSpan MaxAge { get; set; }

        public CacheItem(string key, T value, CacheDependency dependency, TimeSpan maxAge)
        {
            this.Key = key;
            this.Value = value;

            this.DateTimeInserted = DateTime.Now;
            this.HasDependency = (dependency != null);
            this.MaxAge = maxAge;
        }
    }
}
