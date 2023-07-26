using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HotChocolate.PreProcessingExtensions.Sorting
{
    public static class IEnumerableSortingCustomExtensions
    {
        /// <summary>
        /// Provides dynamic Linq in-memory sorting implementation for an IEnumerable using the GraphQL PreProcessing Extensions.
        /// NOTE: This is primarily used for Unit Testing of in-memory data sets and is generally not recommended for production
        ///     use unless you always have 100% of all your data in-memory; this is because sorting must be done on a pre-filtered and/or
        /// complete data set to yield proper results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="sortArgs"></param>
        /// <returns></returns>
        public static IEnumerable<T> SortDynamically<T>(this IEnumerable<T> items, IReadOnlyList<ISortOrderField> sortArgs)
        {
            if (items == null || !items.Any())
                return new List<T>();

            if (sortArgs?.Any() == false)
                return items;

            //Map the Sort by property string names to actual Property Descriptors
            //for dynamic processing...
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
            return orderedItems?.ToList() ?? items;
        }
    }
}
