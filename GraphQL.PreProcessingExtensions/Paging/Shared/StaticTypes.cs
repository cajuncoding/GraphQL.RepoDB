using System;
using System.Collections.Generic;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Pagination;

namespace GraphQL.PreProcessingExtensions
{
    public class StaticTypes
    {
        public static readonly Type IHavePreProcessingPageInfo = typeof(IHavePreProcessedPagingInfo);
        public static readonly Type IPreProcessedOffsetPageResults = typeof(IPreProcessedOffsetPageResults<>);
        public static readonly Type IPreProcessedCursorSlice = typeof(IPreProcessedCursorSlice<>);
        public static readonly Type IAmPreProcessingResult = typeof(IAmPreProcessedResult);
        public static readonly Type IEnumerableGeneric = typeof(IEnumerable<>);
    }
}
