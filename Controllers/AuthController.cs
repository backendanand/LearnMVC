using Dapper;
using LearnMVC.Data;
using LearnMVC.Models;
using LearnMVC.Models.ServiceModels;
using LearnMVC.Models.ViewModels;
using LearnMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LearnMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly DapperContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthContextService _authContextService;

        public AuthController(
            DapperContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuthContextService authContextService
            )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _authContextService = authContextService;
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
                string selectQuery = "SELECT * FROM users WHERE username=@Username ORDER BY id ASC LIMIT 1";

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
                var isAuthenticated = _authContextService.SetSession(user);

                if(isAuthenticated == false)
                {
                    ModelState.AddModelError("", "Authentication failed. Please try again.");
                    return View(model);
                }

                ModelState.AddModelError("", "Login Successful.");
                return View(model);
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
                string insertQuery = "INSERT INTO users (name, username, mobile, role, password, created_at, created_by, created_ip) VALUES (@Name, @Username, @Mobile, @Role, @Password, @CreatedAt, @CreatedBy, @CreatedIp)";

                var parameter = new DynamicParameters();
                parameter.Add("Name", model.Name);
                parameter.Add("Username", model.Username);
                parameter.Add("Mobile", model.Mobile);
                parameter.Add("Role", model.Role);
                parameter.Add("Password", hashedPassword);
                parameter.Add("CreatedAt", DateTime.Now);
                parameter.Add("CreatedBy", -1);
                parameter.Add("CreatedIp", _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());

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
