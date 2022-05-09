using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
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
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles )
            .AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>( options => 
                options.UseSqlServer( Configuration.GetConnectionString("DefaultConnection")));


            services.AddTransient<MyActionFilter>();


            services.AddHostedService<WritingFile>();


            services.AddResponseCaching();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTSEED"])),
                    ClockSkew = TimeSpan.Zero
                });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

            });


            services.AddAutoMapper(typeof(Startup));

            // Condigurar Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
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
