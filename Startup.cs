using WebAPI_tutorial.Context;
using WebAPI_tutorial.Repository;
using WebAPI_tutorial.Repository.Interfaces;
using WebAPI_tutorial.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebAPI_tutorial.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebAPI_tutorial
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuración de los Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // para arreglar errores de loop de relaciones 1..n y viceversa

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Configuración de la base de datos
            var isLocalConnectionString = Configuration.GetValue<bool>("ConnectionStrings:ConnectionString_isLocal");
            var connectionStringKey = isLocalConnectionString ? "ConnectionString_WebAPI_tutorial_local" : "ConnectionString_WebAPI_tutorial_remote";
            services.AddDbContext<ContextDB>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringKey));
            });

            services.AddAutoMapper(typeof(MappingConfig));

            // Registro de servicios 
            // AddTransient: cambia dentro del contexto
            // AddScoped: se mantiene dentro del contexto (mejor para los servicios)
            // AddSingleton: no cambia nunca
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IBookRepository, BookRepository>();

            services.AddResponseCaching();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        }

        /// <summary>
        /// Configuración del Middleware
        /// Middlewares son los métodos "Use..()"
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Middleware customizado: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/26839760#notes
            app.UseLogResponseHTTP();

            // Middleware para intervenir en la ejecución
            app.Map("/exitRoute", app => // si entra una url "ruta1" se ejecuta
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Estoy interceptando la tubería de ejecución.");
                });
            });

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}