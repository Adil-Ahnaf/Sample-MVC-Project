using Bootcamp.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using UserAuthentication.Business.Models;
using UserAuthentication.Web.ApiClient;
using UserAuthentication.Web.Models;
using UserAuthentication.Web.Repositories;

namespace UserAuthentication.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        IWebApiClient webApiClient;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
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
                    ModelState.AddModelError("Register", error);
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
                return RedirectToAction("Index", "Student");
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("Login", "Account is locked out.");
                return View("SignIn", model);
            }
            else
            {
                ModelState.AddModelError("Login", "Login Failed. Please try again.");
                return View("SignIn", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserSignOut()
        {
            SignInRepository signInRepository = new SignInRepository(webApiClient);
            await signInRepository.UserSignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
