using System.Collections.Generic;
using System.Net;

namespace GraphQL.PreProcessingExtensions.Tests
{
    /// <summary>
    /// BBernard
    /// Generate an in-memory Test Server for Asp.Net Core!
    /// NOTE: Borrowed from HotChocolate.AspNet.Core tests project from the Core HotChocolate source.
    /// </summary>
    public class GraphQLQueryResult
    {
        public string ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public List<Dictionary<string, object>> Errors { get; set; }
        public Dictionary<string, object> Extensions { get; set; }
    }

    public class ClientRawResult
    {
        public string ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
    }
}
