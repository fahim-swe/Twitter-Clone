
using core.Interfaces;
using infrastructure.Services.RabbitMQ;



using System.Text;
using infrastructure.Services.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using core.Entities.ServiceEntities;
using core.Interfaces.RabbitMQ;

namespace api.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<RabbitMQConnectionFactorySettings>(
                 _config.GetSection("RabbitMQConectionFactory"));
                 
            services.AddSingleton<ITweetDBConsumer, TweetDBConsumer>();
            services.AddSingleton<ILikesDBConsumer, LikesDBConsumer>();
            services.AddSingleton<ICommentsDBConsumer, CommentsDBConsumer>();
            services.AddSingleton<IFollowDBConsumer, FollowConsume>();
            services.AddSingleton<INotificationConsumer, NotificationConsumer>();


            services.AddSingleton<ISignalRConsumer, SignalRConsumer>();
            
            return services;
        }


         public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<TokenSettings>(_config.GetSection("JWT"));
            services.AddScoped<ITokenService, TokenService>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                            options => {
                                options.SaveToken = true;
                                options.RequireHttpsMetadata = false;
                                options.TokenValidationParameters = new TokenValidationParameters()
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = false,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ClockSkew = TimeSpan.Zero,

                                    ValidAudience = _config["JWT:ValidAudience"],
                                    ValidIssuer = _config["JWT:ValidIssuer"],
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]))
                                };

                                options.Events = new JwtBearerEvents{
                                    OnMessageReceived = context => {
                                        var access_token = context.Request.Query["access_token"];
                                        var path = context.HttpContext.Request.Path;

                                        if(!string.IsNullOrEmpty(access_token) && path.StartsWithSegments("/hubs")){
                                            context.Token = access_token;
                                        }
                                        
                                        return Task.CompletedTask;
                                    }
                                };
                            }
                        
                        );

            return services;
        }
    }
}