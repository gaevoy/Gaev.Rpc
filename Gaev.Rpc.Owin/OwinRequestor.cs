using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gaev.Rpc.Owin
{
    public class OwinRequestor : IRequestor, IDisposable
    {
        private readonly HttpClient _client;

        public OwinRequestor(string baseUrl)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<object> Ask(object request)
        {
            var requestJson = JsonConvert.SerializeObject(request, RpcMiddleware.JsonSettings);
            var response = await _client.PostAsync("/rpc", new StringContent(requestJson));
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(responseJson, RpcMiddleware.JsonSettings);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}