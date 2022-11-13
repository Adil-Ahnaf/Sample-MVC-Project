using System.Threading.Tasks;
using UserAuthentication.Business.Models;
using UserAuthentication.Web.ApiClient;

namespace UserAuthentication.Web.Repositories
{
    public class ChangePasswordRepository
    {
        private readonly IWebApiClient _webApiClient;

        public ChangePasswordRepository(IWebApiClient webApiClient)
        {
            _webApiClient = webApiClient;
        }
        public async Task<ChangePassword> CheckCurrentPassword(ChangePassword changePassword)
        {
            return await _webApiClient.InvokePostAsync($"api/account/checkCurrentPassword", changePassword);
        }
        public async Task UpdatePasswordAsync(ChangePassword changePassword)
        {
            await _webApiClient.InvokePutAsync($"api/account/updatePassword", changePassword);
        }
    }
}
