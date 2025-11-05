using Xunit;
using GeoSense.API.Controllers;
using GeoSense.API.Infrastructure.Contexts;
using GeoSense.API.Infrastructure.Repositories;
using GeoSense.API.Infrastructure.Repositories.Interfaces;
using GeoSense.API.Services;
using Microsoft.EntityFrameworkCore;
using GeoSense.API.DTOs.Moto;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GeoSense.API.AutoMapper;

namespace GeoSense.API.Tests.Moto
{
    public class MotoControllerTests
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
        public async Task PostMoto_DeveRetornarCreated()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb")
                .Options;

            using var context = new GeoSenseContext(options);
            context.Vagas.Add(new GeoSense.API.Infrastructure.Persistence.Vaga(1, 1));
            await context.SaveChangesAsync();

            IMotoRepository motoRepo = new MotoRepository(context);
            var service = new MotoService(motoRepo);

            var mapper = CreateMapper();
            var riscoService = new MotoRiscoMlService();
            var controller = new MotoController(service, mapper, riscoService);

            var dto = new MotoDTO
            {
                Modelo = "Teste",
                Placa = "ABC1234",
                Chassi = "CHASSITESTE",
                ProblemaIdentificado = "Nenhum",
                VagaId = 1
            };

            var result = await controller.PostMoto(dto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task GetMoto_DeveRetornarRiscoAlto_SeProblemaRelacionadoAoMotor()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_GetMoto_RiscoAlto")
                .Options;

            using var context = new GeoSenseContext(options);
            // Cria vaga e moto
            var vaga = new GeoSense.API.Infrastructure.Persistence.Vaga(1, 1);
            context.Vagas.Add(vaga);
            var moto = new GeoSense.API.Infrastructure.Persistence.Moto
            {
                Modelo = "Honda CG",
                Placa = "ABC1D23",
                Chassi = "9C2JC4110JR000001",
                ProblemaIdentificado = "Motor com ruído excessivo",
                VagaId = vaga.Id
            };
            context.Motos.Add(moto);
            await context.SaveChangesAsync();

            IMotoRepository motoRepo = new MotoRepository(context);
            var service = new MotoService(motoRepo);
            var mapper = CreateMapper();
            var riscoService = new MotoRiscoMlService();
            var controller = new MotoController(service, mapper, riscoService);

            var result = await controller.GetMoto(moto.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<MotoDetalhesDTO>(okResult.Value);
            Assert.Equal("Honda CG", dto.Modelo);
            Assert.Equal("Motor com ruído excessivo", dto.ProblemaIdentificado);
            Assert.Equal("ALTO", dto.Risco); // Esperado risco ALTO na lógica dummy
        }

        [Fact]
        public async Task GetMoto_DeveRetornarRiscoBaixo_SeSemProblema()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_GetMoto_RiscoBaixo")
                .Options;

            using var context = new GeoSenseContext(options);
            var vaga = new GeoSense.API.Infrastructure.Persistence.Vaga(2, 1);
            vaga.GetType().GetProperty("Tipo")?.SetValue(vaga, GeoSense.API.Domain.Enums.TipoVaga.Reparo_Simples);

            context.Vagas.Add(vaga);
            var moto = new GeoSense.API.Infrastructure.Persistence.Moto
            {
                Modelo = "Yamaha",
                Placa = "XYZ5678",
                Chassi = "9C2JC4110JR000002",
                ProblemaIdentificado = "",
                VagaId = vaga.Id
            };
            context.Motos.Add(moto);
            await context.SaveChangesAsync();

            IMotoRepository motoRepo = new MotoRepository(context);
            var service = new MotoService(motoRepo);
            var mapper = CreateMapper();
            var riscoService = new MotoRiscoMlService();
            var controller = new MotoController(service, mapper, riscoService);

            var result = await controller.GetMoto(moto.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<MotoDetalhesDTO>(okResult.Value);
            Assert.Equal("Yamaha", dto.Modelo);
            Assert.Equal("", dto.ProblemaIdentificado);
            Assert.Equal("BAIXO", dto.Risco); // Agora deve passar!
        }

        [Fact]
        public async Task PutMoto_DeveRetornarNoContent_SeExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Put")
                .Options;

            using var context = new GeoSenseContext(options);

            var vaga = new GeoSense.API.Infrastructure.Persistence.Vaga(1, 1);
            context.Vagas.Add(vaga);

            var moto = new GeoSense.API.Infrastructure.Persistence.Moto
            {
                Modelo = "Honda",
                Placa = "XYZ0001",
                Chassi = "CHASSI123",
                ProblemaIdentificado = "Ruído",
                VagaId = vaga.Id
            };
            context.Motos.Add(moto);
            await context.SaveChangesAsync();

            IMotoRepository motoRepo = new MotoRepository(context);
            var service = new MotoService(motoRepo);

            var mapper = CreateMapper();
            var riscoService = new MotoRiscoMlService();
            var controller = new MotoController(service, mapper, riscoService);

            var dto = new MotoDTO
            {
                Modelo = "Honda CG",
                Placa = "XYZ0002",
                Chassi = "CHASSI1234",
                ProblemaIdentificado = "Novo problema",
                VagaId = vaga.Id
            };

            var result = await controller.PutMoto(moto.Id, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutMoto_DeveRetornarNotFound_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Put_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);

            IMotoRepository motoRepo = new MotoRepository(context);
            var service = new MotoService(motoRepo);

            var mapper = CreateMapper();
            var riscoService = new MotoRiscoMlService();
            var controller = new MotoController(service, mapper, riscoService);

            var dto = new MotoDTO
            {
                Modelo = "Honda CG",
                Placa = "XYZ0002",
                Chassi = "CHASSI1234",
                ProblemaIdentificado = "Novo problema",
                VagaId = 1
            };

            var result = await controller.PutMoto(999, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteMoto_DeveRetornarNoContent_SeExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Delete")
                .Options;

            using var context = new GeoSenseContext(options);

            var vaga = new GeoSense.API.Infrastructure.Persistence.Vaga(1, 1);
            context.Vagas.Add(vaga);

            var moto = new GeoSense.API.Infrastructure.Persistence.Moto
            {
                Modelo = "Honda",
                Placa = "XYZ0001",
                Chassi = "CHASSI123",
                ProblemaIdentificado = "Ruído",
                VagaId = vaga.Id
            };
            context.Motos.Add(moto);
            await context.SaveChangesAsync();

            IMotoRepository motoRepo = new MotoRepository(context);
            var service = new MotoService(motoRepo);

            var mapper = CreateMapper();
            var riscoService = new MotoRiscoMlService();
            var controller = new MotoController(service, mapper, riscoService);

            var result = await controller.DeleteMoto(moto.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMoto_DeveRetornarNotFound_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Delete_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);

            IMotoRepository motoRepo = new MotoRepository(context);
            var service = new MotoService(motoRepo);

            var mapper = CreateMapper();
            var riscoService = new MotoRiscoMlService();
            var controller = new MotoController(service, mapper, riscoService);

            var result = await controller.DeleteMoto(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}