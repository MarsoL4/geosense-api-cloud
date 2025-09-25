using Xunit;
using GeoSense.API.Controllers;
using GeoSense.API.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GeoSense.API.Tests
{
    public class DashboardControllerTests
    {
        [Fact]
        public async Task GetDashboardData_DeveRetornarOk()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Dashboard")
                .Options;

            using var context = new GeoSenseContext(options);
            var controller = new DashboardController(context);

            var result = await controller.GetDashboardData();

            Assert.IsType<OkObjectResult>(result);
        }
    }
}