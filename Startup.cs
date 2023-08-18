using API_testing3.Context;
using API_testing3.Repository;
using API_testing3.Repository.Interfaces;
using API_testing3.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace API_testing3
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
            var connectionStringKey = isLocalConnectionString ? "ConnectionString_apitesting3db_local" : "ConnectionString_apitesting3db_remote";
            services.AddDbContext<ContextDB>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringKey));
            });

            services.AddAutoMapper(typeof(MappingConfig));

            // Registro de servicios
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
        }

        /// <summary>
        /// Configuración del Middleware
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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