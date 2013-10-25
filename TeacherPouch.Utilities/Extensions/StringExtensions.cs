using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TeacherPouch.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this IEnumerable<string> strings, string value, StringComparison comparisonOption)
        {
            if (!strings.SafeAny()) return false;

            return (strings.Any(str => str.IndexOf(value, comparisonOption) >= 0));
        }

        public static bool Contains(this string str, string value, StringComparison comparisonOption)
        {
            if (str == null) return false;

            return (str.IndexOf(value, comparisonOption) >= 0);
        }

        public static string ToTitleCase(this string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }
    }
}
