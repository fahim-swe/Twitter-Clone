using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.ServiceEntities;
using core.Interfaces.Redis;
using infrastructure.Services.Redis;

namespace api2.Extensions
{
    public static class ApplicationCacheServicesExtensions
    {
        public static  IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration _config )
        {
             services.Configure<RedisCacheSetting>(_config.GetSection("RedisCacheSetting"));
            services.AddSingleton<IResponseCacheService, ResponseCacheService>(); 
            services.AddTransient<IRedisService, RedisService>();
             return services;
        }
    }
}