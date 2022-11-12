using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace UserAuthentication.Web.ApiClient
{
    public class WebApiClient : IWebApiClient
    {
        private readonly string baseUrl;
        private readonly HttpClient httpClient;

        public WebApiClient(string baseUrl, HttpClient httpClient)
        {
            this.baseUrl = baseUrl;
            this.httpClient = httpClient;

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> InvokeGetAsync<T>(string uri)
        {
            return await httpClient.GetFromJsonAsync<T>(GetUrl(uri));
        }

        public async Task<T> InvokePostAsync<T>(string uri, T obj)
        {
            var response = await httpClient.PostAsJsonAsync<T>(GetUrl(uri), obj);
            await HandleError(response);
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task InvokePutAsync<T>(string uri, T obj)
        {
            var response = await httpClient.PutAsJsonAsync<T>(GetUrl(uri), obj);
            await HandleError(response);
        }

        public async Task InvokeDeleteAsync(string uri)
        {
            var response = await httpClient.DeleteAsync(GetUrl(uri));
            await HandleError(response);
        }

        private string GetUrl(string uri)
        {
            return $"{baseUrl}/{uri}";
        }

        private async Task HandleError(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(error);
            }
        }
    }
}
