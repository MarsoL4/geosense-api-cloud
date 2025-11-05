using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace GeoSense.API.Middleware
{
    /// <summary>
    /// Middleware que exige a presença de uma API Key no header GeoSense-Api-Key para acesso aos endpoints.
    /// </summary>
    public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private const string APIKEY_NAME = "GeoSense-Api-Key";
        private readonly RequestDelegate _next = next;
        private readonly IConfiguration _configuration = configuration;

        public async Task InvokeAsync(HttpContext context)
        {
            // Permite HealthCheck e Swagger sem chave:
            var path = context.Request.Path.ToString().ToLower();
            if (path.StartsWith("/swagger") || path.StartsWith("/health"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(APIKEY_NAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            var apiKey = _configuration.GetValue<string>("ApiKey");
            if (string.IsNullOrWhiteSpace(apiKey) || !apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            await _next(context);
        }
    }
}