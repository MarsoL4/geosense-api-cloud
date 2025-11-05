using System.Net;
using Xunit;

namespace GeoSense.API.Tests.Dashboard
{
    public class DashboardIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetDashboard_DeveRetornar200()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/dashboard");
            request.Headers.Add("GeoSense-Api-Key", "SEGREDO-GEOSENSE-123");

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}