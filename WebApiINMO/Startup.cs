using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiINMO.Filters;
using WebApiINMO.Middlewares;
using WebApiINMO.Services;

namespace WebApiINMO
{
    public class Startup
    {

        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices( IServiceCollection services ) {

            services.AddControllers( options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            }).AddJsonOptions( options => 
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles );

            services.AddDbContext<ApplicationDbContext>( options => 
                options.UseSqlServer( Configuration.GetConnectionString("DefaultConnection")));


            services.AddTransient<MyActionFilter>();


            services.AddHostedService<WritingFile>();


            services.AddResponseCaching();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            services.AddAutoMapper(typeof(Startup));
        }


        public void Configure( IApplicationBuilder app, IWebHostEnvironment env, ILogger logger )
        {
            // Si crear la static class
            // app.UseMiddleware<LoggerResponseHTTPMiddleware>();
            // Creando una static class
            app.UseLoggerResponseHttp();

            //app.Run(async context =>
            //{
            //  await context.Response.WriteAsync("Estoy interceptando la tubería");
            //});
                

            // Configure the HTTP request pipeline.
            if ( env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
           {
               endpoints.MapControllers();
           });

        }
    }
}
