using AspNetCore.Identity.Dapper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserAuthentication.Business.Models;

namespace UserAuthentication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> UserSignUp(SignUp signUp)
        {
            var user = new ApplicationUser
            {
                UserName = signUp.UserName,
                Email = signUp.Email,
            };

            var result = await userManager.CreateAsync(user, signUp.Password);
            bool flag = true;

            foreach (var item in result.Errors)
            {
                if (flag)
                {
                    signUp.ExceptionMessage = item.Description;
                    flag = false;
                }
                else
                {
                    signUp.ExceptionMessage = signUp.ExceptionMessage + ',' + item.Description;
                }
            }
            return Ok(signUp);
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> ValidateLogin(SignIn signIn)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(signIn.Email);
            if (user != null)
            {
                var signInresult = await signInManager.PasswordSignInAsync(user, signIn.Password, signIn.IsPersistentCookie, true);
                signIn.IsSuccess = signInresult.Succeeded;
                signIn.EmailConfirmed = user.EmailConfirmed;
                signIn.Name = user.UserName;
                signIn.UserGuid = user.Id;
            }
            else
            {
                signIn.EmailConfirmed = false;
            }

            return Ok(signIn);
        }

        [HttpGet("signOut")]
        public async Task UserSignOut()
        {
            await signInManager.SignOutAsync();
            httpContextAccessor.HttpContext.Session.Clear();
        }
    }
}
