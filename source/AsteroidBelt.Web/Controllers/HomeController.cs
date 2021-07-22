using Microsoft.AspNetCore.Mvc;

namespace AsteroidBelt.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
