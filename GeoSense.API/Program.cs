using GeoSense.API.AutoMapper;
using GeoSense.API.Infrastructure.Contexts;
using GeoSense.API.Services;
using GeoSense.API.Infrastructure.Repositories;
using GeoSense.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace GeoSense.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<MotoService>();
            builder.Services.AddScoped<IMotoRepository, MotoRepository>();

            builder.Services.AddScoped<VagaService>();
            builder.Services.AddScoped<IVagaRepository, VagaRepository>();

            builder.Services.AddScoped<PatioService>();
            builder.Services.AddScoped<IPatioRepository, PatioRepository>();

            builder.Services.AddScoped<UsuarioService>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            builder.Services.AddScoped<DashboardService>();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            var connectionString = builder.Configuration.GetConnectionString("Oracle");
            builder.Services.AddDbContext<GeoSenseContext>(options =>
                options.UseOracle(connectionString));

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            builder.Services.AddEndpointsApiExplorer();

            // Swagger customizado, incluindo exemplos de payloads
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);

                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "GeoSense API",
                    Version = "v1",
                    Description = "API RESTful para gerenciamento de Motos, Vagas, Pátios e Usuários.\nEndpoints CRUD, paginação, HATEOAS e exemplos de payload."
                });

                options.ExampleFilters();
            });

            builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}