using Bootcamp.Web.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using UserAuthentication.Business.Models;
using UserAuthentication.Web.ApiClient;
using UserAuthentication.Web.Models;
using UserAuthentication.Web.Repositories;

namespace UserAuthentication.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration configuration;
        IWebApiClient webApiClient;

        public AccountController(IConfiguration configuration)
        {
            this.configuration = configuration;
            webApiClient = new WebApiClient(configuration.GetValue<string>("ApiUrl"), new HttpClient());
        }
        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserSignUp(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("SignUp", model);

            SignUpRepository signUpRepository = new SignUpRepository(webApiClient);
            SignUp result = new SignUp();

            result = await signUpRepository.CreateAsync(new SignUp()
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password
            });

            if (result.ExceptionMessage == null)
            {
                return RedirectToAction("SignIn");
            }
            else
            {
                string[] ErrorList = result.ExceptionMessage.Split(",");

                foreach (string error in ErrorList)
                {
                    ModelState.AddModelError("SignUp", error);
                }
                return View("SignUp", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UserSignIn(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("SignIn", model);

            SignInRepository signInRepository = new SignInRepository(webApiClient);

            var result = await signInRepository.GetSignInAsync(new SignIn()
            {
                Email = model.Email,
                Password = model.Password
            });

            if (result.IsSuccess)
            {
                HttpContext.Session.SetString("UserEmail", result.Email);
                HttpContext.Session.SetString("UserId", result.UserGuid);

                // create claims  
                List<Claim> claims = new List<Claim>
                {
                      new Claim(ClaimTypes.Name, result.Name),
                      new Claim(ClaimTypes.Email, result.Email),
                      new Claim("Student", "true")
                };

                // create identity  
                ClaimsIdentity identity = new ClaimsIdentity(claims, "UserAuthenticationCookieAuth");

                // create principal  
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.IsPersistentCookie,
                };

                // sign-in  
                await HttpContext.SignInAsync("UserAuthenticationCookieAuth", principal, authProperties);
                return RedirectToAction("Index", "Student");
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("SignIn", "Account is locked out.");
                return View("SignIn", model);
            }
            else
            {
                ModelState.AddModelError("SignIn", "Login Failed. Please try again.");
                return View("SignIn", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserSignOut()
        {
            SignInRepository signInRepository = new SignInRepository(webApiClient);
            await signInRepository.UserSignOut();
            await HttpContext.SignOutAsync(scheme: "UserAuthenticationCookieAuth");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ChangePassword", model);
            else
            {
                if (model.NewPassword != model.NewPasswordRetype)
                {
                    ModelState.AddModelError("ChangePassword", "Please enter the new password correctly.");
                    return View("ChangePassword", model);
                }
                else
                {
                    var userEmail = HttpContext.Session.GetString("UserEmail");
                    if (userEmail == null)
                    {
                        userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                        HttpContext.Session.SetString("UserEmail", userEmail);
                    }

                    ChangePasswordRepository changePasswordRepository = new ChangePasswordRepository(webApiClient);

                    var result = await changePasswordRepository.CheckCurrentPassword(new ChangePassword
                    {
                        Email = userEmail,
                        CurrentPassword = model.CurrentPassword,
                        NewPassword = model.NewPassword,
                    });

                    if (result.IsSuccess)
                    {
                        await changePasswordRepository.UpdatePasswordAsync(new ChangePassword
                        {
                            Email = userEmail,
                            CurrentPassword = model.CurrentPassword,
                            NewPassword = model.NewPassword,
                        });

                        // sign out user
                        SignInRepository signInRepository = new SignInRepository(webApiClient);
                        await signInRepository.UserSignOut();
                        await HttpContext.SignOutAsync(scheme: "UserAuthenticationCookieAuth");
                        HttpContext.Session.Clear();

                        return RedirectToAction("SignIn", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("ChangePassword", "You have entered wrong password.");
                        return View("ChangePassword", model);
                    }
                }
            }
        }
    }
}
