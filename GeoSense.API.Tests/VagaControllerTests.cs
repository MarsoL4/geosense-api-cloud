using Xunit;
using GeoSense.API.Controllers;
using GeoSense.API.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using GeoSense.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using GeoSense.API.Infrastructure.Persistence;
using GeoSense.API.Domain.Enums;
using AutoMapper;
using GeoSense.API.AutoMapper;
using GeoSense.API.DTOs.Vaga;

namespace GeoSense.API.Tests
{
    public class VagaControllerTests
    {
        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            return config.CreateMapper();
        }

        [Fact]
        public async Task PostVaga_DeveRetornarCreated()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga")
                .Options;

            using var context = new GeoSenseContext(options);
            context.Patios.Add(new Patio { Nome = "Pátio Central" });
            await context.SaveChangesAsync();

            var mapper = CreateMapper();
            var controller = new VagaController(context, mapper);

            var patio = context.Patios.FirstOrDefault();
            Assert.NotNull(patio);

            var dto = new VagaDTO
            {
                Numero = 1,
                Tipo = (int)TipoVaga.Reparo_Simples,
                Status = (int)StatusVaga.LIVRE,
                PatioId = patio.Id
            };

            var result = await controller.PostVaga(dto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task GetVaga_DeveRetornarNotFoundObject_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);
            var mapper = CreateMapper();
            var controller = new VagaController(context, mapper);

            var result = await controller.GetVaga(999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutVaga_DeveRetornarOk_SeExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_Put")
                .Options;

            using var context = new GeoSenseContext(options);
            var patio = new Patio { Nome = "Pátio Central" };
            context.Patios.Add(patio);
            var vaga = new Vaga(1, patio.Id);
            context.Vagas.Add(vaga);
            await context.SaveChangesAsync();

            var mapper = CreateMapper();
            var controller = new VagaController(context, mapper);

            var dto = new VagaDTO
            {
                Numero = 2,
                Tipo = (int)TipoVaga.Motor_Defeituoso,
                Status = (int)StatusVaga.OCUPADA,
                PatioId = patio.Id
            };

            var result = await controller.PutVaga(vaga.Id, dto);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutVaga_DeveRetornarNotFoundObject_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_Put_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);
            var mapper = CreateMapper();
            var controller = new VagaController(context, mapper);

            var dto = new VagaDTO
            {
                Numero = 2,
                Tipo = (int)TipoVaga.Motor_Defeituoso,
                Status = (int)StatusVaga.OCUPADA,
                PatioId = 1
            };

            var result = await controller.PutVaga(999, dto);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteVaga_DeveRetornarOk_SeExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_Delete")
                .Options;

            using var context = new GeoSenseContext(options);
            var patio = new Patio { Nome = "Pátio Central" };
            context.Patios.Add(patio);
            var vaga = new Vaga(1, patio.Id);
            context.Vagas.Add(vaga);
            await context.SaveChangesAsync();

            var mapper = CreateMapper();
            var controller = new VagaController(context, mapper);

            var result = await controller.DeleteVaga(vaga.Id);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteVaga_DeveRetornarNotFoundObject_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_Delete_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);
            var mapper = CreateMapper();
            var controller = new VagaController(context, mapper);

            var result = await controller.DeleteVaga(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}