using AutoMapper;
using GeoSense.API.AutoMapper;
using GeoSense.API.Controllers;
using GeoSense.API.Domain.Enums;
using GeoSense.API.DTOs.Vaga;
using GeoSense.API.Infrastructure.Contexts;
using GeoSense.API.Infrastructure.Repositories;
using GeoSense.API.Infrastructure.Repositories.Interfaces;
using GeoSense.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GeoSense.API.Tests.Vaga
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
            context.Patios.Add(new GeoSense.API.Infrastructure.Persistence.Patio { Nome = "Pátio Central" });
            await context.SaveChangesAsync();

            IVagaRepository vagaRepo = new VagaRepository(context);
            var service = new VagaService(vagaRepo);

            var mapper = CreateMapper();
            var controller = new VagaController(service, mapper);

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
        public async Task GetVaga_DeveRetornarNotFound_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);

            IVagaRepository vagaRepo = new VagaRepository(context);
            var service = new VagaService(vagaRepo);

            var mapper = CreateMapper();
            var controller = new VagaController(service, mapper);

            var result = await controller.GetVaga(999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutVaga_DeveRetornarNoContent_SeExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_Put")
                .Options;

            using var context = new GeoSenseContext(options);
            var patio = new GeoSense.API.Infrastructure.Persistence.Patio { Nome = "Pátio Central" };
            context.Patios.Add(patio);
            var vaga = new GeoSense.API.Infrastructure.Persistence.Vaga(1, patio.Id);
            context.Vagas.Add(vaga);
            await context.SaveChangesAsync();

            IVagaRepository vagaRepo = new VagaRepository(context);
            var service = new VagaService(vagaRepo);

            var mapper = CreateMapper();
            var controller = new VagaController(service, mapper);

            var dto = new VagaDTO
            {
                Numero = 2,
                Tipo = (int)TipoVaga.Motor_Defeituoso,
                Status = (int)StatusVaga.OCUPADA,
                PatioId = patio.Id
            };

            var result = await controller.PutVaga(vaga.Id, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutVaga_DeveRetornarNotFound_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_Put_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);

            IVagaRepository vagaRepo = new VagaRepository(context);
            var service = new VagaService(vagaRepo);

            var mapper = CreateMapper();
            var controller = new VagaController(service, mapper);

            var dto = new VagaDTO
            {
                Numero = 2,
                Tipo = (int)TipoVaga.Motor_Defeituoso,
                Status = (int)StatusVaga.OCUPADA,
                PatioId = 1
            };

            var result = await controller.PutVaga(999, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteVaga_DeveRetornarNoContent_SeExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_Delete")
                .Options;

            using var context = new GeoSenseContext(options);
            var patio = new GeoSense.API.Infrastructure.Persistence.Patio { Nome = "Pátio Central" };
            context.Patios.Add(patio);
            var vaga = new GeoSense.API.Infrastructure.Persistence.Vaga(1, patio.Id);
            context.Vagas.Add(vaga);
            await context.SaveChangesAsync();

            IVagaRepository vagaRepo = new VagaRepository(context);
            var service = new VagaService(vagaRepo);

            var mapper = CreateMapper();
            var controller = new VagaController(service, mapper);

            var result = await controller.DeleteVaga(vaga.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteVaga_DeveRetornarNotFound_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Vaga_Delete_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);

            IVagaRepository vagaRepo = new VagaRepository(context);
            var service = new VagaService(vagaRepo);

            var mapper = CreateMapper();
            var controller = new VagaController(service, mapper);

            var result = await controller.DeleteVaga(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}