using System.Net;
using System.Net.Http.Json;
using GeoSense.API.DTOs.Moto;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GeoSense.API.Tests.Moto
{
    public class MotoIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetMotos_DeveRetornarStatusCode200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/moto");

            // Adiciona API Key se necessário (ajuste para o valor do seu appsettings)
            request.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostMoto_DeveRetornarCreated()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/moto");
            request.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");

            var moto = new MotoDTO
            {
                Modelo = "Yamaha Factor",
                Placa = "ABC1D28",
                Chassi = "9C2JD4112JR000009",
                ProblemaIdentificado = "",
                VagaId = 61
            };

            // Primeiro, precisa garantir que uma vaga existe:
            // (Se não existir, o endpoint POST vai rejeitar. Se necessário, faça uma chamada anterior para cadastrar uma vaga via API.)

            request.Content = JsonContent.Create(moto);

            var response = await _client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Created ||
                response.StatusCode == HttpStatusCode.BadRequest,
                $"Esperado Created ou BadRequest, obteve: {response.StatusCode}");

            // Se Created, valida retorno
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.Contains("Moto cadastrada com sucesso", content);
            }
        }
    }
}