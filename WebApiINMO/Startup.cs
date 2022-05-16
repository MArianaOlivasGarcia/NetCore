using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiINMO.Filters;
using WebApiINMO.Middlewares;
using WebApiINMO.Services;
using WebApiINMO.Utils;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebApiINMO
{
    public class Startup
    {

        public Startup( IConfiguration configuration )
        {

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices( IServiceCollection services ) {

            services.AddControllers( options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
                // Añadir mi util de swagger para agrupar por versión
                options.Conventions.Add(new SwaggerByVersion());
            }).AddJsonOptions( options => 
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles )
            .AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>( options => 
                options.UseSqlServer( Configuration.GetConnectionString("DefaultConnection")));


            services.AddTransient<MyActionFilter>();


            //services.AddHostedService<WritingFile>();


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

                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "WebApiINMO", 
                    Version = "v1",
                    Description = "Esta es una Web API de Inmobiliarias",
                    Contact = new OpenApiContact
                    {
                        Email = "olivasgarcia031096@gmail.com",
                        Name = "MAriana Olivas",
                        Url = new Uri("http://marianaolivas.com")
                    }
                });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApiINMO", Version = "v2" });

                c.OperationFilter<AddParamsHATEOAS>();

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



                var fileXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var pathXML = Path.Combine(AppContext.BaseDirectory, fileXML);
                c.IncludeXmlComments(pathXML);

            });


            services.AddAutoMapper(typeof(Startup));

            // Condigurar Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();



            // Autenticación basada en Claims
            services.AddAuthorization(options =>
           {
               options.AddPolicy("ADMIN", policy => policy.RequireClaim("ADMIN"));
               options.AddPolicy("USER", policy => policy.RequireClaim("USER"));
           });

            services.AddCors(options =>
           {
               options.AddDefaultPolicy(builder =>
              {
                  builder.WithOrigins("https://www.apirequest.io")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders( new string[] { "totalResults"  });
              });
           });


            // Servicio de protección de datos
            services.AddDataProtection();

            // Nuestro servicio de hash que creamos
            services.AddTransient<HashService>();



            services.AddTransient<GeneratorUrls>();
            services.AddTransient<HATEOASPRopertyFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

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
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiINMO v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiINMO v2");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
           {
               endpoints.MapControllers();
           });

        }
    }
}
