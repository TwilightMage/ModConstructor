using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModConstructor
{
    public static class Extensions
    {
        public static void AddUnique<T>(this IList<T> target, T item)
        {
            if (!target.Contains(item)) target.Add(item);
        }
    }
}
