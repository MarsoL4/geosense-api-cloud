﻿using AutoMapper;
using GeoSense.API.AutoMapper;
using GeoSense.API.Controllers;
using GeoSense.API.DTOs.Usuario;
using GeoSense.API.Infrastructure.Contexts;
using GeoSense.API.Infrastructure.Repositories;
using GeoSense.API.Infrastructure.Repositories.Interfaces;
using GeoSense.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoSense.API.Infrastructure.Persistence;

namespace GeoSense.API.Tests
{
    public class UsuarioControllerTests
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
        public async Task PostUsuario_DeveRetornarCreated()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Usuario")
                .Options;

            using var context = new GeoSenseContext(options);

            IUsuarioRepository usuarioRepo = new UsuarioRepository(context);
            var service = new UsuarioService(usuarioRepo);

            var mapper = CreateMapper();
            var controller = new UsuarioController(service, mapper);

            var dto = new UsuarioDTO
            {
                Nome = "Teste",
                Email = "teste@exemplo.com",
                Senha = "senha123",
                Tipo = 0 // ADMINISTRADOR
            };

            var result = await controller.PostUsuario(dto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task GetUsuario_DeveRetornarNotFound_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Usuario_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);

            IUsuarioRepository usuarioRepo = new UsuarioRepository(context);
            var service = new UsuarioService(usuarioRepo);

            var mapper = CreateMapper();
            var controller = new UsuarioController(service, mapper);

            var result = await controller.GetUsuario(999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutUsuario_DeveRetornarNoContent_SeExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Usuario_Put")
                .Options;

            using var context = new GeoSenseContext(options);
            var usuario = new Usuario(0, "Teste", "teste@exemplo.com", "senha123", Domain.Enums.TipoUsuario.ADMINISTRADOR);
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            IUsuarioRepository usuarioRepo = new UsuarioRepository(context);
            var service = new UsuarioService(usuarioRepo);

            var mapper = CreateMapper();
            var controller = new UsuarioController(service, mapper);

            var dto = new UsuarioDTO
            {
                Nome = "Novo Nome",
                Email = "novoemail@exemplo.com",
                Senha = "novasenha",
                Tipo = 1
            };

            var result = await controller.PutUsuario(usuario.Id, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutUsuario_DeveRetornarNotFound_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Usuario_Put_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);

            IUsuarioRepository usuarioRepo = new UsuarioRepository(context);
            var service = new UsuarioService(usuarioRepo);

            var mapper = CreateMapper();
            var controller = new UsuarioController(service, mapper);

            var dto = new UsuarioDTO
            {
                Nome = "Novo Nome",
                Email = "novoemail@exemplo.com",
                Senha = "novasenha",
                Tipo = 1
            };

            var result = await controller.PutUsuario(999, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUsuario_DeveRetornarNoContent_SeExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Usuario_Delete")
                .Options;

            using var context = new GeoSenseContext(options);
            var usuario = new Usuario(0, "Teste", "teste@exemplo.com", "senha123", Domain.Enums.TipoUsuario.ADMINISTRADOR);
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            IUsuarioRepository usuarioRepo = new UsuarioRepository(context);
            var service = new UsuarioService(usuarioRepo);

            var mapper = CreateMapper();
            var controller = new UsuarioController(service, mapper);

            var result = await controller.DeleteUsuario(usuario.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUsuario_DeveRetornarNotFound_SeNaoExistir()
        {
            var options = new DbContextOptionsBuilder<GeoSenseContext>()
                .UseInMemoryDatabase(databaseName: "GeoSenseTestDb_Usuario_Delete_NotFound")
                .Options;

            using var context = new GeoSenseContext(options);

            IUsuarioRepository usuarioRepo = new UsuarioRepository(context);
            var service = new UsuarioService(usuarioRepo);

            var mapper = CreateMapper();
            var controller = new UsuarioController(service, mapper);

            var result = await controller.DeleteUsuario(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}