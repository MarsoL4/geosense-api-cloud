using AutoMapper;
using GeoSense.API.DTOs;
using GeoSense.API.DTOs.Moto;
using GeoSense.API.Helpers;
using GeoSense.API.Infrastructure.Contexts;
using GeoSense.API.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace GeoSense.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotoController(GeoSenseContext context, IMapper mapper) : ControllerBase
    {
        private readonly GeoSenseContext _context = context;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Retorna uma lista paginada de motos cadastradas.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista de motos, podendo utilizar paginação via parâmetros <b>page</b> e <b>pageSize</b>.
        /// </remarks>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
        /// <response code="200">Lista paginada de motos</response>
        [HttpGet]
        [SwaggerResponse(200, "Lista paginada de motos cadastradas", typeof(PagedHateoasDTO<MotoDetalhesDTO>))]
        public async Task<ActionResult<PagedHateoasDTO<MotoDetalhesDTO>>> GetMotos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Motos.Include(m => m.Vaga);
            var totalCount = await query.CountAsync();
            var motos = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<MotoDetalhesDTO>>(motos);

            var links = HateoasHelper.GetPagedLinks(Url, "Motos", page, pageSize, totalCount);

            var result = new PagedHateoasDTO<MotoDetalhesDTO>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Links = links
            };

            return Ok(result);
        }

        /// <summary>
        /// Retorna os dados de uma moto por ID.
        /// </summary>
        /// <remarks>
        /// Retorna os detalhes de uma moto específica a partir do seu identificador.
        /// </remarks>
        /// <param name="id">Identificador único da moto</param>
        /// <response code="200">Moto encontrada</response>
        /// <response code="404">Moto não encontrada</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Moto encontrada", typeof(MotoDetalhesDTO))]
        [SwaggerResponse(404, "Moto não encontrada")]
        public async Task<ActionResult<MotoDetalhesDTO>> GetMoto(long id)
        {
            var moto = await _context.Motos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (moto == null)
            {
                return NotFound(new { mensagem = "Moto não encontrada." });
            }

            var dto = _mapper.Map<MotoDetalhesDTO>(moto);
            return Ok(dto);
        }

        /// <summary>
        /// Atualiza os dados de uma moto existente.
        /// </summary>
        /// <remarks>
        /// Atualiza os dados da moto informada pelo ID. O corpo da requisição deve conter o modelo <see cref="MotoDTO"/>.
        /// O campo <b>ProblemaIdentificado</b> é opcional.
        /// </remarks>
        /// <param name="id">Identificador único da moto</param>
        /// <param name="dto">Dados da moto a serem atualizados</param>
        /// <response code="200">Moto atualizada com sucesso</response>
        /// <response code="400">Alguma restrição de negócio foi violada</response>
        /// <response code="404">Moto não encontrada</response>
        [HttpPut("{id}")]
        [SwaggerRequestExample(typeof(MotoDTO), typeof(GeoSense.API.Examples.MotoDTOExample))]
        [SwaggerResponse(200, "Moto atualizada com sucesso", typeof(object))]
        [SwaggerResponse(400, "Restrição de negócio violada")]
        [SwaggerResponse(404, "Moto não encontrada")]
        public async Task<IActionResult> PutMoto(long id, MotoDTO dto)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return NotFound(new { mensagem = "Moto não encontrada." });

            var vagaOcupada = await _context.Motos.CountAsync(m => m.VagaId == dto.VagaId && m.Id != id) > 0;
            if (vagaOcupada)
                return BadRequest(new { mensagem = "Esta vaga já está ocupada por outra moto." });

            var placaExiste = await _context.Motos.CountAsync(m => m.Placa == dto.Placa && m.Id != id) > 0;
            if (placaExiste)
                return BadRequest(new { mensagem = "Já existe uma moto com essa placa." });

            var chassiExiste = await _context.Motos.CountAsync(m => m.Chassi == dto.Chassi && m.Id != id) > 0;
            if (chassiExiste)
                return BadRequest(new { mensagem = "Já existe uma moto com esse chassi." });

            moto.Modelo = dto.Modelo;
            moto.Placa = dto.Placa;
            moto.Chassi = dto.Chassi;
            moto.ProblemaIdentificado = dto.ProblemaIdentificado;
            moto.VagaId = dto.VagaId;

            await _context.SaveChangesAsync();

            var motoAtualizada = await _context.Motos.FirstOrDefaultAsync(m => m.Id == id);
            var dtoAtualizado = _mapper.Map<MotoDetalhesDTO>(motoAtualizada);

            return Ok(new
            {
                mensagem = "Moto atualizada com sucesso.",
                dados = dtoAtualizado
            });
        }

        /// <summary>
        /// Cadastra uma nova moto.
        /// </summary>
        /// <remarks>
        /// Cadastra uma nova moto no sistema. O corpo da requisição deve conter o modelo <see cref="MotoDTO"/>.
        /// O campo <b>ProblemaIdentificado</b> é opcional.
        /// </remarks>
        /// <param name="dto">Dados da nova moto</param>
        /// <response code="201">Moto criada com sucesso</response>
        /// <response code="400">Alguma restrição de negócio foi violada</response>
        [HttpPost]
        [SwaggerRequestExample(typeof(MotoDTO), typeof(GeoSense.API.Examples.MotoDTOExample))]
        [SwaggerResponse(201, "Moto criada com sucesso", typeof(object))]
        [SwaggerResponse(400, "Restrição de negócio violada")]
        public async Task<ActionResult<MotoDetalhesDTO>> PostMoto(MotoDTO dto)
        {
            var vagaOcupada = await _context.Motos.CountAsync(m => m.VagaId == dto.VagaId) > 0;
            if (vagaOcupada)
                return BadRequest(new { mensagem = "Esta vaga já está ocupada por outra moto." });

            var placaExiste = await _context.Motos.CountAsync(m => m.Placa == dto.Placa) > 0;
            if (placaExiste)
                return BadRequest(new { mensagem = "Já existe uma moto com essa placa." });

            var chassiExiste = await _context.Motos.CountAsync(m => m.Chassi == dto.Chassi) > 0;
            if (chassiExiste)
                return BadRequest(new { mensagem = "Já existe uma moto com esse chassi." });

            var novaMoto = new Moto
            {
                Modelo = dto.Modelo,
                Placa = dto.Placa,
                Chassi = dto.Chassi,
                ProblemaIdentificado = dto.ProblemaIdentificado,
                VagaId = dto.VagaId
            };

            _context.Motos.Add(novaMoto);
            await _context.SaveChangesAsync();

            var motoCompleta = await _context.Motos
                .FirstOrDefaultAsync(m => m.Id == novaMoto.Id);

            var resultDto = _mapper.Map<MotoDetalhesDTO>(motoCompleta);

            return CreatedAtAction(nameof(GetMoto), new { id = novaMoto.Id }, new
            {
                mensagem = "Moto cadastrada com sucesso.",
                dados = resultDto
            });
        }

        /// <summary>
        /// Exclui uma moto do sistema.
        /// </summary>
        /// <remarks>
        /// Remove a moto informada pelo ID.
        /// </remarks>
        /// <param name="id">Identificador único da moto</param>
        /// <response code="200">Moto removida</response>
        /// <response code="404">Moto não encontrada</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, "Moto removida com sucesso", typeof(object))]
        [SwaggerResponse(404, "Moto não encontrada")]
        public async Task<IActionResult> DeleteMoto(long id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return NotFound(new { mensagem = "Moto não encontrada." });

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Moto deletada com sucesso."
            });
        }
    }
}