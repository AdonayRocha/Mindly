using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mindly.Controllers
{
    public class AdminProtectAttribute : TypeFilterAttribute
    {
        public AdminProtectAttribute() : base(typeof(AdminProtectFilter)) { }
    }

    public class AdminProtectFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        public AdminProtectFilter(IConfiguration configuration) => _configuration = configuration;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var expected = _configuration["Auth:AdminPassword"] ?? "admin"; // fallback

            string? provided = null;
            if (httpContext.Request.Headers.TryGetValue("X-Admin-Password", out var pwdHeader))
                provided = pwdHeader.ToString();
            else if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
                provided = authHeader.ToString();
            else if (httpContext.Request.Query.TryGetValue("admin", out var adminQuery))
                provided = adminQuery.ToString();

            if (!string.Equals(provided, expected, StringComparison.Ordinal))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Content = "Senha inválida"
                };
            }
        }
    }
}
