
using System.Text;
using core.Interfaces.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace api2.Cache
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;
        private readonly int _checkMark;
        public CachedAttribute(int timeToLiveSeconds, int checkMark = 0)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
            _checkMark = checkMark;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var cacheKey = generateCacheKeyFromRequest(context);
            var chacheResponse = await cacheService.GetCachedResponseAsync(cacheKey);
            
            if(!string.IsNullOrEmpty(chacheResponse)){

              
                var contentResult = new ContentResult
                {
                    Content = chacheResponse,
                    ContentType =  "application/json; charset=utf-8",
                    StatusCode = 200
                };

                context.Result = contentResult;
                return ;
            }

            var executedContext = await next();
            
            if(executedContext.Result is OkObjectResult okObjectResult)
            {
               
               await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }

        private string generateCacheKeyFromRequest(ActionExecutingContext context)
        {
            var keyBuilder = new StringBuilder();
            var path = context.HttpContext.Request.Path;

            var userId =  (string)context.HttpContext.Items["userId"];

            if(this.containKey(path)) userId = "";

            keyBuilder.Append($"{userId + path}");
            
            foreach(var (key, value) in context.HttpContext.Request.Query.OrderBy(x => x.Key)){
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }


        private bool containKey(string path)
        {
            string[] keyword = {"Like", "Like/Isliked", "Like/likes", "Tweet/search", "Tweet/get-tweets",  "tweet/hashTag", "Comment/Comments/","Users/serach-users"};

            foreach(var key in keyword){
                if(path.Contains(key)) return true;
            }
            return false;
        }
    }
}


