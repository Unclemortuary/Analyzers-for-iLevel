using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace iLevel.CodeAnalysis.ServiceIntegrationTests.Common
{
    public class TestHttpClient
    {
        private HttpClient _client;

        public TestHttpClient()
        {
            _client = new HttpClient();
        }

        public HttpResponseMessage PostMIMEFiles(string uri, params string[] values)
        {
            return null;
        }

        public HttpResponseMessage Send(string uri)
        {
            return _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)).Result;
        }
    }
}
