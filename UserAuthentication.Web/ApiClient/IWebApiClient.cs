using System.Threading.Tasks;

namespace UserAuthentication.Web.ApiClient
{
    public interface IWebApiClient
    {
        Task InvokeDeleteAsync(string uri);       
        Task<T> InvokeGetAsync<T>(string uri);
        Task<T> InvokePostAsync<T>(string uri, T obj);
        Task InvokePutAsync<T>(string uri, T obj);
    }
}
