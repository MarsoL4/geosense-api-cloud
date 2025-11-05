using GeoSense.API.DTOs.Usuario;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GeoSense.API.Tests.Usuario
{
    public class UsuarioIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetUsuarios_DeveRetornarStatusCode200()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/usuario");
            request.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostUsuario_DeveRetornarCreatedOuBadRequest_SeEmailDuplicado()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/usuario");
            request.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");

            var dto = new UsuarioDTO
            {
                Nome = "Teste Integracao",
                Email = "user1@int.test",
                Senha = "1234",
                Tipo = 0
            };
            request.Content = JsonContent.Create(dto);

            var response = await _client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Created ||
                response.StatusCode == HttpStatusCode.BadRequest,
                $"Esperado Created ou BadRequest, obteve: {response.StatusCode}");

            // Rodando de novo (email duplicado), deve retornar BadRequest
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var secondReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/usuario");
                secondReq.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");
                secondReq.Content = JsonContent.Create(dto);

                var secondResp = await _client.SendAsync(secondReq);

                Assert.Equal(HttpStatusCode.BadRequest, secondResp.StatusCode);
            }
        }
    }
}