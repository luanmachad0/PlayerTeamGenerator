using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Helpers
{
    public class RequireBearerTokenAttribute : TypeFilterAttribute
    {
        public RequireBearerTokenAttribute() : base(typeof(RequireBearerTokenFilter))
        {
        }

        private class RequireBearerTokenFilter : IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var authHeader = context.HttpContext.Request.Headers["Authorization"];
                var authHeaderString = authHeader.ToString();
                if (string.IsNullOrEmpty(authHeaderString) || !authHeaderString.StartsWith("Bearer "))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var token = authHeaderString.Substring("Bearer ".Length);
                if (token != "SkFabTZibXE1aE14ckpQUUxHc2dnQ2RzdlFRTTM2NFE2cGI4d3RQNjZmdEFITmdBQkE=")
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
        }
    }
}