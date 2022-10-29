using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api2.Helper;
using core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api2.Middleware
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
                // user not logged in
                context.Result = new JsonResult(
                    new Response<string>("Unauthorized")
                ) {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
    
