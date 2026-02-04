using Microsoft.AspNetCore.Mvc;

namespace LearnMVC.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
