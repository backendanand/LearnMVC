using LearnMVC.Filters.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace LearnMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RoleAuthorize(["Admin"])]
    public class StudentController : Controller
    {
        public StudentController()
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
