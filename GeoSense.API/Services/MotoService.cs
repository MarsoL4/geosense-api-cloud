using AutoMapper;
using GeoSense.API.DTOs.Moto;
using GeoSense.Infrastructure.Repositories.Interfaces;

namespace GeoSense.API.Services
{
    public class MotoService
    {
        private readonly IMotoRepository _repo;
        private readonly IMapper _mapper;

        public MotoService(IMotoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<MotoDetalhesDTO>> ObterTodasAsync()
        {
            var motos = await _repo.ObterTodasAsync();
            return _mapper.Map<List<MotoDetalhesDTO>>(motos);
        }

        public async Task<MotoDetalhesDTO?> ObterPorIdAsync(long id)
        {
            var moto = await _repo.ObterPorIdComVagaEDefeitosAsync(id);
            if (moto == null) return null;

            return new MotoDetalhesDTO
            {
                Id = moto.Id,
                Modelo = moto.Modelo,
                Placa = moto.Placa,
                Chassi = moto.Chassi,
                ProblemaIdentificado = moto.ProblemaIdentificado,
                VagaId = moto.VagaId
            };
        }
    }
}