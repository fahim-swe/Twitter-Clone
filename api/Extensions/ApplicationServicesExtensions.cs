
using api.Helper;
using core.Entities.ServiceEntities;
using core.Interfaces;
using core.Interfaces.Email;
using core.Interfaces.RabbitMQ;
using core.Interfaces.Redis;
using infrastructure.Database.Repository;
using infrastructure.Database.StoreContext;
using infrastructure.Database.UnitOfWork;
using infrastructure.Services.Email;
using infrastructure.Services.RabbitMQ;
using infrastructure.Services.Redis;

namespace api.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _config)
        {

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.Configure<MailSettings>(_config.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();

            return services;
        }

        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<DatabaseSettings>(_config.GetSection("DatabaseSettings"));
            services.AddScoped<IMongoContext, MongoContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<RabbitMQConnectionFactorySettings>(
                 _config.GetSection("RabbitMQConectionFactory"));

            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();

            services.AddSingleton<ISignalRConsumer, SignalRConsumer>();
            
            
            services.AddScoped(typeof(IPublish<>), typeof(Publish<>));
            
            return services;
        }

        public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<RedisCacheSetting>(_config.GetSection("RedisCacheSetting"));
            services.AddSingleton<IResponseCacheService, ResponseCacheService>(); 
            services.AddTransient<IRedisService, RedisService>();
            return services;
        }
    
    
        
    }
}