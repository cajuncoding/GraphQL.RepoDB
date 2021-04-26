using System;
using System.Collections.Generic;
using System.Text;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    /// <summary>
    /// NOTE: These must be defined here because the constants in the HC Core are marked as Internal and not accessible.
    /// </summary>
    public static class CursorPagingArgNames
    {
        public const string FirstDescription = "first";
        public const string AfterDescription = "after";
        public const string LastDescription = "last";
        public const string BeforeDescription = "before";
    }
}
