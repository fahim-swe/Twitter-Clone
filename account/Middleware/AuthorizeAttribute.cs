
using account.Helper;
using core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace account.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var block =  (AdminBlock)context.HttpContext.Items["block"];
            if(block != null) 
            {
                context.Result = new JsonResult( new Response<string>("Block By Admin" + block.CreatedAt)) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            

            var user = context.HttpContext.Items["userId"];
            if (user == null) {
                context.Result = new JsonResult(
                    new Response<string>("Unauthorized")
                ) {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
    
