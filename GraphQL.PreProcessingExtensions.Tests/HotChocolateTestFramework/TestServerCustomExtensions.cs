using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace GraphQL.PreProcessingExtensions.Tests
{
    public static class TestServerCustomExtensions
    {
        public static async Task<GraphQLQueryResult> PostAsync(
            this TestServer testServer,
            GraphQLQueryRequest request,
            string path = "/graphql")
        {
            HttpResponseMessage response = await SendPostRequestAsync(
                testServer,
                JsonConvert.SerializeObject(request),
                path
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new GraphQLQueryResult() {StatusCode = HttpStatusCode.NotFound};
            }

            var json = await response.Content.ReadAsStringAsync();
            GraphQLQueryResult result = JsonConvert.DeserializeObject<GraphQLQueryResult>(json);
            result.StatusCode = response.StatusCode;
            result.ContentType = response.Content.Headers.ContentType.ToString();
            return result;
        }

        public static Task<HttpResponseMessage> SendPostRequestAsync(
            this TestServer testServer, string requestBody, string path = null)
        {
            return SendPostRequestAsync(
                testServer, requestBody,
                "application/json", path);
        }

        public static Task<HttpResponseMessage> SendPostRequestAsync(
            this TestServer testServer, string requestBody,
            string contentType, string path)
        {
            return testServer.CreateClient()
                .PostAsync(CreateUrl(path),
                    new StringContent(requestBody,
                        Encoding.UTF8, contentType));
        }

        public static string CreateUrl(string path)
        {
            var url = "http://localhost:5000";

            if (path != null)
            {
                url += "/" + path.TrimStart('/');
            }

            return url;
        }
    }
}
