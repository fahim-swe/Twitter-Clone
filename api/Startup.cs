
using System.Reflection;
using api.Extensions;
using api.Helper;
using api.Helpers;
using api.Middleware;
using core.Entities.ServiceEntities;
using core.Interfaces;
using core.Interfaces.Email;
using core.Interfaces.RabbitMQ;
using core.Interfaces.Redis;
using FluentValidation.AspNetCore;
using infrastructure.Database.Repository;
using infrastructure.Database.StoreContext;
using infrastructure.Database.UnitOfWork;
using infrastructure.Services.Email;
using infrastructure.Services.Redis;
using infrastructure.Services.SignalR;
using StackExchange.Redis;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddApplicationServices(Configuration);
            services.AddDatabaseServices(Configuration);        
            services.AddIdentityServices(Configuration);
            services.AddCacheServices(Configuration);
            services.AddRabbitMQServices(Configuration);
            services.AddSwaggerService();
            services.AddOptions();


            services.AddSignalR();

            services.AddCors(opt => 
            {
                opt.AddPolicy("CorsPolicy", policy => 
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                });
            });
            
            services.AddControllers();
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            }

            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("hubs/notification");
            });


             lifetime.ApplicationStarted.Register(() => RegisterSignalRWithRabbitMQ(app.ApplicationServices));
        }





        public void RegisterSignalRWithRabbitMQ(IServiceProvider serviceProvider)
        {
            try{
                var rabbitMQService6 = (ISignalRConsumer?)serviceProvider.GetService(typeof(ISignalRConsumer));
                if(rabbitMQService6 != null){
                    rabbitMQService6.Connect();
                }
            }catch(Exception e)
            {
                Console.WriteLine( "Problem\n" +  e);
            }
            
        }
    }
}

