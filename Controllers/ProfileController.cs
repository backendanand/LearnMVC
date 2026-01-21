using Microsoft.AspNetCore.Mvc;

namespace LearnMVC.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
