using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api2.Helper;
using core.Entities;
using core.Interfaces;
using core.Interfaces.Redis;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helper
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Console.WriteLine("Hello");
            var resultContext = await next();
            var userId = resultContext.HttpContext.User.GetUserId();
            var redis = resultContext.HttpContext.RequestServices.GetService<IRepository<AdminBlock>>();
            var checkedBlock = await redis.FindOneAsync(filter => filter.UserId == userId);

            if(!resultContext.HttpContext.User.Identity.IsAuthenticated && checkedBlock != null) return ;
        }
    }
}