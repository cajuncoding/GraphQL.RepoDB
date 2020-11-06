using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public static class CursorPagingPagingArgumentsExtensions
    {
        public static bool IsPagingArgumentsValid(this CursorPagingArguments args)
        {
            //Offset paging has a minimum requirement of a Take parameter being specified!
            return string.IsNullOrWhiteSpace(args.After)
                    || string.IsNullOrWhiteSpace(args.Before)
                    || args.First.HasValue
                    || args.Last.HasValue;
        }
    }
}
