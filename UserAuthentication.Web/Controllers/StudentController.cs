using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using UserAuthentication.Web.ApiClient;

namespace UserAuthentication.Web.Controllers
{
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> logger;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;
        IWebApiClient webApiClient;

        public StudentController(ILogger<StudentController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            webApiClient = new WebApiClient(configuration.GetValue<string>("ApiUrl"), new HttpClient());
        }

        [Authorize(Policy = "MustBeStudent")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
