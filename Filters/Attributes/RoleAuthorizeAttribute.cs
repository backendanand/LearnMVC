using LearnMVC.Models.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace LearnMVC.Filters.Attributes
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _allowedRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var sessionString = httpContext.Session.GetString("AuthSession");

            // If no session found
            if (string.IsNullOrEmpty(sessionString))
            {
                // Force to clear other session data
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            var session = JsonSerializer.Deserialize<AuthSession>(sessionString);

            // If role not allowed
            if (session == null || !_allowedRoles.Contains(session.Role))
            {
                //context.Result = new ForbidResult();
                context.Result = new RedirectToActionResult("Forbidden", "Error", new { area = "" });
            }
        }
    }
}
