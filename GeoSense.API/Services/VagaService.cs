﻿using GeoSense.API.Domain.Enums;
using GeoSense.API.DTOs.Vaga;
using GeoSense.API.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GeoSense.API.Services
{
    public class VagaService(GeoSenseContext context)
    {
        private readonly GeoSenseContext _context = context;

        public async Task<VagasStatusDTO> ObterVagasLivresAsync()
        {
            // Busca apenas vagas com Status LIVRE, e seleciona apenas os campos necessários
            var vagasLivres = await _context.Vagas
                .Where(v => (int)v.Status == (int)StatusVaga.LIVRE)
                .Select(v => v.Tipo)
                .ToListAsync();

            // Conta as categorias de forma simples e direta
            var livresComProblema = vagasLivres.Count(tipo => tipo != TipoVaga.Sem_Problema);
            var livresSemProblema = vagasLivres.Count(tipo => tipo == TipoVaga.Sem_Problema);

            return new VagasStatusDTO
            {
                LivresComProblema = livresComProblema,
                LivresSemProblema = livresSemProblema
            };
        }
    }
}