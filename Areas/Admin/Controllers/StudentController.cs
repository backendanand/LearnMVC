using LearnMVC.Filters.Attributes;
using LearnMVC.Models.DomainModels;
using LearnMVC.Models.ViewModels;
using LearnMVC.Repositories;
using LearnMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LearnMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RoleAuthorize(["Admin"])]
    public class StudentController : Controller
    {
        private readonly IGenericRepository _genericRepository;
        private readonly IAuthContextService _authContextService;

        public StudentController(IGenericRepository genericRepository, IAuthContextService authContextService)
        {
            _genericRepository = genericRepository;
            _authContextService = authContextService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentVM model)
        {
            try
            {
                if(model.Id == 0)
                {
                    ModelState.Remove("Id");
                }

                if(!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fill all the required fields.";
                    return View(model);
                }

                var currentSession = _authContextService.GetSession();

                var newStudent = new Student
                {
                    first_name = model.FirstName,
                    middle_name = model.MiddleName,
                    last_name = model.LastName,
                    email = model.Email,
                    password = model.Password,
                    phone = model.Phone,
                    age = model.Age,
                    date_of_birth = model.DateOfBirth,
                    gender = model.Gender,
                    is_active = model.IsActive,
                    hobbies = model.Hobbies,
                    course = model.Course,
                    skills = model.Skills,
                    address = model.Address,
                    profile_image = model.ProfileImage.FileName,
                    created_at = DateTime.Now,
                    created_by = currentSession.UserId,
                    created_ip = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                await _genericRepository.InsertAsync("students", newStudent);

                TempData["SuccessMessage"] = "Student created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
