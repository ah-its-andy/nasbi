using NasBI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NasBI.Truenas
{
    public class TruenasRequest
    {
        public string Identifier { get; }
        public string ServiceHost { get; }
        public string ServiceUrl { get; }
        public HttpMethod Method { get; }
        public HttpContent Content { get; }
        public IDictionary<string, string> Headers { get; }
        public async Task<TruenasResponse> InvokeAsync()
        {
            var request = new HttpRequestMessage();
            request.Method = Method;
            request.Content = Content;
            request.RequestUri = new Uri($"{ServiceHost}/{ServiceUrl}", UriKind.Absolute);

            var resp = await HttpInvoker.InvokeAsync(Identifier, request);
            if (!resp.IsSuccessStatusCode)
            {
                return new TruenasResponse
                {
                    StatusCode = resp.StatusCode,
                    Data = new byte[0]
                };
            }

            var data = await resp.Content.ReadAsByteArrayAsync();
            return new TruenasResponse
            {
                StatusCode = resp.StatusCode,
                Data = data
            };
        }
    }
}
