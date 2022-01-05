using System;
using System.Collections.Generic;
using System.Linq;

namespace Wallpaper.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy((item) => rnd.Next());
        }
    }
}
