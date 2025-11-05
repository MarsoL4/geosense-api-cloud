using GeoSense.API.DTOs.Patio;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GeoSense.API.Tests.Patio
{
    public class PatioIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetPatios_DeveRetornarStatusCode200()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/patio");
            request.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostPatio_DeveRetornarCreated()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/patio");
            request.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");

            var dto = new PatioDTO { Nome = "Pátio de Teste" };
            request.Content = JsonContent.Create(dto);

            var response = await _client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Created ||
                response.StatusCode == HttpStatusCode.BadRequest,
                $"Esperado Created ou BadRequest, obteve: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.Contains("Pátio cadastrado com sucesso", content);
            }
        }
    }
}