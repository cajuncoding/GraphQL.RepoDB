using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GraphQL.RepoDb
{
    public static class TimeSpanCustomExtensions
    {
        public static string ToElapsedTimeDescriptiveFormat(this Stopwatch timer)
        {
            var descriptiveFormat = $"{timer.Elapsed:hh\\h\\:mm\\m\\:ss\\s\\:fff\\m\\s}";
            return descriptiveFormat;
        }
    }
}
