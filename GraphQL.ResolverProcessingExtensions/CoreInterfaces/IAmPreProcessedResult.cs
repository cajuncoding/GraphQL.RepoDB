using System;

namespace HotChocolate.ResolverProcessingExtensions
{
    /// <summary>
    /// Marker class for all resolver processed result classes; provides easy identification by Middleware
    ///     for specific handling to skip post-process for Sorting, Paging, etc. but enable leveraging of 
    ///     all other OOTB HotChocolate magic that it provides!
    /// </summary>
    public interface IAmResolverProcessedResult
    {
    }
}
