using System;
using System.Collections.Generic;
using System.Text;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public static class OffsetPagingPagingArgumentsExtensions
    {
        public static bool IsPagingArgumentsValid(this OffsetPagingArguments args)
        {
            //Offset paging has a minimum requirement of a Take parameter being specified!
            return args.Take >= 0;
        }
    }
}
