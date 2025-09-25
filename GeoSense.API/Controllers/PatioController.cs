using AutoMapper;
using GeoSense.API.DTOs;
using GeoSense.API.DTOs.Patio;
using GeoSense.API.DTOs.Vaga;
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
    public class PatioController(GeoSenseContext context, IMapper mapper) : ControllerBase
    {
        private readonly GeoSenseContext _context = context;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Retorna uma lista paginada de pátios cadastrados.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista dos pátios existentes no sistema, com suporte à paginação através dos parâmetros <b>page</b> e <b>pageSize</b>.
        /// Cada item contém dados básicos do pátio.
        /// </remarks>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
        /// <response code="200">Lista paginada de pátios cadastrados</response>
        [HttpGet]
        [SwaggerResponse(200, "Lista paginada de pátios cadastrados", typeof(PagedHateoasDTO<PatioDTO>))]
        public async Task<ActionResult<PagedHateoasDTO<PatioDTO>>> GetPatios([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Patios.AsNoTracking();
            var totalCount = await query.CountAsync();
            var patios = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<PatioDTO>>(patios);

            var links = HateoasHelper.GetPagedLinks(Url, "Patios", page, pageSize, totalCount);

            var result = new PagedHateoasDTO<PatioDTO>
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
        /// Retorna os dados detalhados de um pátio por ID.
        /// </summary>
        /// <remarks>
        /// Retorna os detalhes de um pátio específico identificado por <b>id</b>, incluindo a lista de suas vagas.
        /// </remarks>
        /// <param name="id">Identificador único do pátio</param>
        /// <response code="200">Pátio encontrado, detalhes retornados</response>
        /// <response code="404">Pátio não encontrado</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Pátio encontrado", typeof(PatioDetalhesDTO))]
        [SwaggerResponse(404, "Pátio não encontrado")]
        public async Task<ActionResult<PatioDetalhesDTO>> GetPatio(long id)
        {
            var patio = await _context.Patios
                .Include(p => p.Vagas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patio == null)
                return NotFound(new { mensagem = "Pátio não encontrado." });

            var dto = new PatioDetalhesDTO
            {
                Id = patio.Id,
                Nome = patio.Nome,
                Vagas = [.. patio.Vagas.Select(v => _mapper.Map<VagaDTO>(v))]
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cadastra um novo pátio no sistema.
        /// </summary>
        /// <remarks>
        /// Cria um novo pátio, recebendo os dados via modelo <see cref="PatioDTO"/>.
        /// O nome do pátio deve ser informado e ser único no sistema.
        /// </remarks>
        /// <param name="_dto">Dados do novo pátio</param>
        /// <response code="201">Pátio cadastrado com sucesso</response>
        [HttpPost]
        [SwaggerRequestExample(typeof(PatioDTO), typeof(GeoSense.API.Examples.PatioDTOExample))]
        [SwaggerResponse(201, "Pátio criado com sucesso", typeof(object))]
        public async Task<ActionResult<PatioDTO>> PostPatio(PatioDTO _dto)
        {
            var novoPatio = new Patio { Nome = _dto.Nome };
            _context.Patios.Add(novoPatio);
            await _context.SaveChangesAsync();

            var patioCompleto = await _context.Patios.FindAsync(novoPatio.Id);
            var resultDto = _mapper.Map<PatioDTO>(patioCompleto);

            return CreatedAtAction(nameof(GetPatio), new { id = novoPatio.Id }, new
            {
                mensagem = "Pátio cadastrado com sucesso.",
                dados = resultDto
            });
        }

        /// <summary>
        /// Atualiza os dados de um pátio existente.
        /// </summary>
        /// <remarks>
        /// Atualiza os dados do pátio identificado por <b>id</b>. O corpo da requisição deve conter o modelo <see cref="PatioDTO"/>.
        /// Apenas o nome pode ser alterado.
        /// </remarks>
        /// <param name="id">Identificador único do pátio</param>
        /// <param name="_dto">Novos dados do pátio</param>
        /// <response code="200">Pátio atualizado com sucesso</response>
        /// <response code="404">Pátio não encontrado</response>
        [HttpPut("{id}")]
        [SwaggerRequestExample(typeof(PatioDTO), typeof(GeoSense.API.Examples.PatioDTOExample))]
        [SwaggerResponse(200, "Pátio atualizado com sucesso", typeof(object))]
        [SwaggerResponse(404, "Pátio não encontrado")]
        public async Task<ActionResult<PatioDTO>> PutPatio(long id, PatioDTO _dto)
        {
            var patio = await _context.Patios.FindAsync(id);
            if (patio == null)
                return NotFound(new { mensagem = "Pátio não encontrado." });

            patio.Nome = _dto.Nome;
            await _context.SaveChangesAsync();

            var patioAtualizado = await _context.Patios.FindAsync(id);
            var resultDto = _mapper.Map<PatioDTO>(patioAtualizado);

            return Ok(new
            {
                mensagem = "Pátio atualizado com sucesso.",
                dados = resultDto
            });
        }

        /// <summary>
        /// Exclui um pátio do sistema.
        /// </summary>
        /// <remarks>
        /// Remove o pátio identificado por <b>id</b> do sistema de forma permanente.
        /// </remarks>
        /// <param name="id">Identificador único do pátio</param>
        /// <response code="200">Pátio removido com sucesso</response>
        /// <response code="404">Pátio não encontrado</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, "Pátio removido com sucesso", typeof(object))]
        [SwaggerResponse(404, "Pátio não encontrado")]
        public async Task<IActionResult> DeletePatio(long id)
        {
            var patio = await _context.Patios.FindAsync(id);
            if (patio == null)
                return NotFound(new { mensagem = "Pátio não encontrado." });

            _context.Patios.Remove(patio);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensagem = "Pátio deletado com sucesso."
            });
        }
    }
}