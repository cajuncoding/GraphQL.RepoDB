using System.Collections.Generic;
using System.Linq;

namespace RepoDb.SqlServer.PagingOperations.DotNetExtensions
{
    internal static class IEnumerableCustomExtensions
    {
        public static string ToCsvString<T>(this IEnumerable<T> enumerableItems, bool includeSpaceAfterComma = false)
        {
            var list = enumerableItems?.ToList();
            if (list == null || !list.Any()) return string.Empty;

            var comma = includeSpaceAfterComma ? ", " : ",";
            var csvValues = string.Join(comma, list.Select(i => i.ToString()).ToList());
            return csvValues;
        }
    }
}