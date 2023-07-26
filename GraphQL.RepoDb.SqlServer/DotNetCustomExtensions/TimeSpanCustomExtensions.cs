using System;
using System.Diagnostics;

namespace HotChocolate.RepoDb
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
