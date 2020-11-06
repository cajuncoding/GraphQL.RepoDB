using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HotChocolate.PreProcessingExtensions.Sorting
{
    public static class IEnumerableSortingCustomExtensions
    {
        public static IEnumerable<T> SortDynamically<T>(this IEnumerable<T> items, IReadOnlyList<ISortOrderField> sortArgs)
        {
            if (items == null || !items.Any())
                return new List<T>();

            //Map the Sort by property string names to actual Property Descriptors
            //for dynamic processign...
            var propCollection = TypeDescriptor.GetProperties(typeof(T));
            var sortGetters = sortArgs?.Select(s => new {
                SortArg = s,
                Getter = propCollection.Find(s.FieldName, true)
            });

            IOrderedEnumerable<T> orderedItems = null;
            foreach (var sort in sortGetters)
            {
                if (orderedItems == null)
                {
                    orderedItems = sort.SortArg.IsAscending()
                        ? items.OrderBy(c => sort.Getter.GetValue(c))
                        : items.OrderByDescending(c => sort.Getter.GetValue(c));
                }
                else
                {
                    orderedItems = sort.SortArg.IsAscending()
                        ? orderedItems.ThenBy(c => sort.Getter.GetValue(c))
                        : orderedItems.ThenByDescending(c => sort.Getter.GetValue(c));
                }
            }

            //NOTE: To Finish the sorting, we materialize the Results!
            return orderedItems.ToList();
        }
    }
}
