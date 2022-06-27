using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NasBI.Common
{
    public delegate void HttpInvokerExecuting(string identifier, Uri? requestUri, HttpMethod method);
    public delegate void HttpInvokerExecuted(string identifier, Uri? requestUri, HttpMethod method);

    public static class HttpInvoker
    {
        public static event HttpInvokerExecuting? OnHttpInvokerExecuting;
        public static event HttpInvokerExecuted? OnHttpInvokerExecuted;
        public static async Task<HttpResponseMessage> InvokeAsync(string identifier, HttpRequestMessage httpRequestMessage)
        {
            var client = new HttpClient();
            OnHttpInvokerExecuting?.Invoke(identifier, httpRequestMessage.RequestUri, httpRequestMessage.Method);
            var resp = await client.SendAsync(httpRequestMessage);
            OnHttpInvokerExecuted?.Invoke(identifier, httpRequestMessage.RequestUri, httpRequestMessage.Method);
            return resp;
        }
    }
}
