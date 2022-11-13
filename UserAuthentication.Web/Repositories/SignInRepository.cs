
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthentication.Business.Models;
using UserAuthentication.Web.ApiClient;

namespace Bootcamp.Web.Repositories
{
    public class SignInRepository
    {
        private readonly IWebApiClient _webApiClient;

        public SignInRepository(IWebApiClient webApiClient)
        {
            _webApiClient = webApiClient;
        }

        public async Task<SignIn> GetSignInAsync(SignIn signIn)
        {
            return await _webApiClient.InvokePostAsync("api/account/signIn", signIn);
        }

        public async Task UserSignOut()
        {
            await _webApiClient.InvokeSignOut("api/account/signOut");
        }
    }
}
