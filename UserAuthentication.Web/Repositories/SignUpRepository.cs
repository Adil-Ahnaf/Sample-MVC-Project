using System.Threading.Tasks;
using UserAuthentication.Business.Models;
using UserAuthentication.Web.ApiClient;

namespace UserAuthentication.Web.Repositories
{
    public class SignUpRepository
    {
        private readonly IWebApiClient _webApiClient;

        public SignUpRepository(IWebApiClient webApiClient)
        {
            _webApiClient = webApiClient;
        }

        public async Task<SignUp> CreateAsync(SignUp signUp)
        {
            var result = await _webApiClient.InvokePostAsync("api/account/signUp", signUp);
            return result;
        }
    }
}
