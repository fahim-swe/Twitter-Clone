
using api.Extensions;
using api2.Extensions;
using api2.Helper;
using api2.Middleware;
using core.Entities.ServiceEntities;
using core.Interfaces;
using core.Interfaces.RabbitMQ;
using infrastructure.Database.Repository;
using infrastructure.Database.StoreContext;
using infrastructure.Database.UnitOfWork;
using infrastructure.Services.SignalR;

namespace api2
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
            services.Configure<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));
            services.AddSingleton<IMongoContext, MongoContext>();

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddRabbitMQServices(Configuration);
            services.AddIdentityServices(Configuration);
            services.AddSwaggerService();
            services.AddControllers();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddCacheService(Configuration);

             services.AddCors(opt => 
            {
                opt.AddPolicy("CorsPolicy", policy => 
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                });
            });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            }

           
            app.UseHttpsRedirection();
            // app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();



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
                var rabbitMQService = (ITweetDBConsumer?)serviceProvider.GetService(typeof(ITweetDBConsumer));
                if(rabbitMQService != null){
                    rabbitMQService.Connect();
                }


                var rabbitMQService2 = (ILikesDBConsumer?)serviceProvider.GetService(typeof(ILikesDBConsumer));
                if(rabbitMQService2 != null){
                    rabbitMQService2.Connect();
                }

                var rabbitMQService3 = (ICommentsDBConsumer?)serviceProvider.GetService(typeof(ICommentsDBConsumer));
                if(rabbitMQService3 != null){
                    rabbitMQService3.Connect();
                }

                var rabbitMQService4 = (IFollowDBConsumer?)serviceProvider.GetService(typeof(IFollowDBConsumer));
                if(rabbitMQService4 != null){
                    rabbitMQService4.Connect();
                }

                var rabbitMQService5 = (INotificationConsumer?)serviceProvider.GetService(typeof(INotificationConsumer));
                if(rabbitMQService5 != null){
                    rabbitMQService5.Connect();
                }

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
