using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherPouch.Utilities.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool SafeAny<T>(this IEnumerable<T> collection)
        {
            return (collection != null && collection.Any());
        }

        public static bool SafeAny<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            return (collection != null && collection.Any(predicate));
        }
    }
}
