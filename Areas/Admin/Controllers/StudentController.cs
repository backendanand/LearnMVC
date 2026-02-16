using LearnMVC.Filters.Attributes;
using LearnMVC.Models.DomainModels;
using LearnMVC.Models.DTOs;
using LearnMVC.Models.ViewModels;
using LearnMVC.Repositories;
using LearnMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LearnMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RoleAuthorize(["Admin"])]
    [Route("admin")]
    public class StudentController : Controller
    {
        private readonly IGenericRepository _genericRepository;
        private readonly IAuthContextService _authContextService;

        public StudentController(IGenericRepository genericRepository, IAuthContextService authContextService)
        {
            _genericRepository = genericRepository;
            _authContextService = authContextService;
        }

        [Route("students")]
        public async Task<IActionResult> Index()
        {
            var students = await _genericRepository.GetAsync<StudentListVM>(options: new QueryOptions
            {
                SelectColumns = new List<string> { "id", "first_name", "middle_name", "last_name", "email", "phone", "age", "course", "is_active" },
                Table = "students",
                Sorts = new List<QuerySort> { new () { Column = "created_at", Descending = true } }
            });
            return View(students);
        }

        [HttpGet("students/create")]
        public IActionResult Create()
        {
            var viewModel = new StudentCreateVM();
            return View(viewModel);
        }

        [HttpGet("students/edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                if(id <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid student ID.";
                    return RedirectToAction(nameof(Index));
                }

                var student = await _genericRepository.GetByIdAsync<Student>("students", "id", id);
                if (student == null)
                {
                    TempData["ErrorMessage"] = "Student not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new StudentCreateVM
                {
                    Id = student.id,
                    FirstName = student.first_name,
                    MiddleName = student.middle_name,
                    LastName = student.last_name,
                    Email = student.email,
                    Password = student.password,
                    Phone = student.phone,
                    Age = student.age,
                    DateOfBirth = student.date_of_birth,
                    Gender = student.gender,
                    IsActive = student.is_active,
                    Hobbies = student.hobbies,
                    Course = student.course,
                    Skills = student.skills,
                    Address = student.address,
                    ProfileImage = null, // File upload will be handled separately in the view
                    ProfileImageUrl = student.profile_image
                };

                return View("Create", viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("students/save")]
        public async Task<IActionResult> Save(StudentCreateVM model)
        {
            try
            {
                if (model.Id == 0)
                {
                    ModelState.Remove("Id");
                }

                if (!ModelState.IsValid)
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
                    age = model.Age ?? 0,
                    date_of_birth = model.DateOfBirth ?? DateOnly.MinValue,
                    gender = model.Gender,
                    is_active = model.IsActive ?? true,
                    hobbies = model.Hobbies,
                    course = model.Course,
                    skills = model.Skills,
                    address = model.Address,
                    profile_image = model.ProfileImage.FileName,
                    created_at = DateTime.Now,
                    created_by = currentSession.UserId,
                    created_ip = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                var newStudentId = await _genericRepository.InsertAsync("students", newStudent);

                TempData["SuccessMessage"] = "Student created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
