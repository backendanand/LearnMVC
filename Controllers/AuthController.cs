using Microsoft.AspNetCore.Mvc;
using LearnMVC.Models;
using LearnMVC.Data;
using LearnMVC.Models.ViewModels;
using Dapper;
using System.Threading.Tasks;

namespace LearnMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly DapperContext _context;

        public AuthController(DapperContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct the errors and try again.");
                return View(model);
            }

            using (var connection = _context.CreateConnection())
            {
                string selectQuery = "SELECT * FROM users WHERE name=@Username ORDER BY id ASC LIMIT 1";

                var parameter = new DynamicParameters();
                parameter.Add("Username", model.Username);

                var user = await connection.QueryFirstOrDefaultAsync<User>(selectQuery, parameter);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid credentials.");
                    return View(model);
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.password);

                if (!isPasswordValid)
                {
                    ModelState.AddModelError("", "Invalid credentials.");
                    return View(model);
                }

                // Authentication successful
                TempData["SuccessMessage"] = "Login successful!";
                return RedirectToAction("Index", "Home");

            }

        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct the errors and try again.");
                return View(model);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            using (var connection = _context.CreateConnection())
            {
                string insertQuery = "INSERT INTO users (name, role, password, created_at) VALUES (@Name, @Role, @Password, @CreatedAt)";

                var parameter = new DynamicParameters();
                parameter.Add("Name", model.Name);
                parameter.Add("Role", model.Role);
                parameter.Add("Password", hashedPassword);
                parameter.Add("CreatedAt", DateTime.Now);

                var executed = await connection.ExecuteAsync(insertQuery, parameter);

                if(executed > 0)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                }
            }


            return View(model);
        }
    }
}
