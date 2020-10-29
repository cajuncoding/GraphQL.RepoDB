using System;

namespace HotChocolate.PreProcessingExtensions
{
    /// <summary>
    /// Marker classs for all pre-processed result classes; provides easy identification by Middleware
    ///     for specific handling to skip post-process for Sorting, Paging, etc. but enable leveraging of 
    ///     all other OOTB HotChocolate magic that it provides!
    /// </summary>
    public interface IAmPreProcessedResult
    {
    }
}
