using Microsoft.AspNetCore.Mvc;

namespace UserAuthentication.Web.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
