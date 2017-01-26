using Authorization.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policies.Over18Years)]
        public IActionResult Requirements()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
