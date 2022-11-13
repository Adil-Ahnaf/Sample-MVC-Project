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
                if (signInresult.Succeeded)
                {
                    signIn.IsSuccess = signInresult.Succeeded;
                    signIn.EmailConfirmed = user.EmailConfirmed;
                    signIn.Name = user.UserName;
                    signIn.UserGuid = user.Id;
                    await signInManager.SignInAsync(user, isPersistent: false);
                }            
            }
            else
            {
                signIn.EmailConfirmed = false;
            }

            return Ok(signIn);
        }

        [HttpGet("signOut")]
        public async Task<IActionResult> UserSignOut()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("checkCurrentPassword")]
        public async Task<ChangePassword> checkCurrentPassword(ChangePassword changePassword)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(changePassword.Email);//checking if user exist or not
            if (user != null)
            {
                var result = await userManager.CheckPasswordAsync(user, changePassword.CurrentPassword);
                changePassword.IsSuccess = result;
                return changePassword;
            }
            else
            {
                changePassword.IsSuccess = false;
                return changePassword;
            }
        }

        [HttpPut("updatePassword")]
        public async Task<IActionResult> UpdatePassword(ChangePassword changePassword)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(changePassword.Email);//checking if user exist or not

            await userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);

            return Ok(0);
        }
    }
}
