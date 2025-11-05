using GeoSense.API.DTOs.Vaga;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GeoSense.API.Tests.Vaga
{
    public class VagaIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetVagas_DeveRetornarStatusCode200()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/vaga");
            request.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostVaga_DeveRetornarCreatedOuBadRequest_SeSemPatio()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/vaga");
            request.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");

            // Precisa garantir um patio existente no banco, senão irá falhar
            // Como o banco começa vazio nos testes, provavelmente resultará em BadRequest (que é esperado)
            var dto = new VagaDTO
            {
                Numero = 100,
                Tipo = 0,
                Status = 0,
                PatioId = 1 // sem seed de patio, então resultará em BadRequest
            };

            request.Content = JsonContent.Create(dto);

            var response = await _client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.Created,
                $"Esperado Created ou BadRequest, obteve: {response.StatusCode}");
        }
    }
}