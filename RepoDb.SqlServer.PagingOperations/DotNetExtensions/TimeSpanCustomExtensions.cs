using System.Diagnostics;

namespace RepoDb.SqlServer.PagingOperations.DotNetExtensions
{
    internal static class TimeSpanCustomExtensions
    {
        public static string ToElapsedTimeDescriptiveFormat(this Stopwatch timer)
        {
            var descriptiveFormat = $"{timer.Elapsed:hh\\h\\:mm\\m\\:ss\\s\\:fff\\m\\s}";
            return descriptiveFormat;
        }
    }
}
