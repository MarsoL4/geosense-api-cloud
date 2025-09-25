using AutoMapper;
using GeoSense.API.Domain.Enums;
using GeoSense.API.DTOs;
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
    public class VagaController(GeoSenseContext context, IMapper mapper) : ControllerBase
    {
        private readonly GeoSenseContext _context = context;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Retorna uma lista paginada de vagas cadastradas.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista de vagas, podendo utilizar paginação via parâmetros <b>page</b> e <b>pageSize</b>.
        /// </remarks>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
        /// <response code="200">Lista paginada de vagas</response>
        [HttpGet]
        [SwaggerResponse(200, "Lista paginada de vagas cadastradas", typeof(PagedHateoasDTO<VagaDTO>))]
        public async Task<ActionResult<PagedHateoasDTO<VagaDTO>>> GetVagas([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Vagas.Include(v => v.Motos);
            var totalCount = await query.CountAsync();
            var vagas = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<VagaDTO>>(vagas);

            var links = HateoasHelper.GetPagedLinks(Url, "Vagas", page, pageSize, totalCount);

            var result = new PagedHateoasDTO<VagaDTO>
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
        /// Retorna os dados de uma vaga por ID.
        /// </summary>
        /// <remarks>
        /// Retorna os detalhes de uma vaga específica a partir do seu identificador.
        /// </remarks>
        /// <param name="id">Identificador único da vaga</param>
        /// <response code="200">Vaga encontrada</response>
        /// <response code="404">Vaga não encontrada</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Vaga encontrada", typeof(VagaDTO))]
        [SwaggerResponse(404, "Vaga não encontrada")]
        public async Task<ActionResult<VagaDTO>> GetVaga(long id)
        {
            var vaga = await _context.Vagas
                .Include(v => v.Motos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });

            var dto = _mapper.Map<VagaDTO>(vaga);
            return Ok(dto);
        }

        /// <summary>
        /// Cadastra uma nova vaga.
        /// </summary>
        /// <remarks>
        /// Cadastra uma nova vaga no sistema. O corpo da requisição deve conter o modelo <see cref="VagaDTO"/>.
        /// </remarks>
        /// <param name="dto">Dados da nova vaga</param>
        /// <response code="201">Vaga criada com sucesso</response>
        [HttpPost]
        [SwaggerRequestExample(typeof(VagaDTO), typeof(GeoSense.API.Examples.VagaDTOExample))]
        [SwaggerResponse(201, "Vaga criada com sucesso", typeof(object))]
        public async Task<ActionResult<VagaDTO>> PostVaga(VagaDTO dto)
        {
            // Verifica se já existe vaga com mesmo número no mesmo pátio
            var vagaExistente = await _context.Vagas
                .CountAsync(v => v.Numero == dto.Numero && v.PatioId == dto.PatioId) > 0;

            if (vagaExistente)
                return BadRequest(new { mensagem = "Já existe uma vaga com esse número neste pátio." });

            var novaVaga = new Vaga(dto.Numero, dto.PatioId);

            novaVaga.GetType().GetProperty("Tipo")?.SetValue(novaVaga, (TipoVaga)dto.Tipo);
            novaVaga.GetType().GetProperty("Status")?.SetValue(novaVaga, (StatusVaga)dto.Status);

            _context.Vagas.Add(novaVaga);
            await _context.SaveChangesAsync();

            var vagaCompleta = await _context.Vagas
                .Include(v => v.Motos)
                .FirstOrDefaultAsync(v => v.Id == novaVaga.Id);
            var resultDto = _mapper.Map<VagaDTO>(vagaCompleta);

            return CreatedAtAction(nameof(GetVaga), new { id = novaVaga.Id }, new
            {
                mensagem = "Vaga cadastrada com sucesso.",
                dados = resultDto
            });
        }

        /// <summary>
        /// Atualiza os dados de uma vaga existente.
        /// </summary>
        /// <remarks>
        /// Atualiza os dados da vaga informada pelo ID. O corpo da requisição deve conter o modelo <see cref="VagaDTO"/>.
        /// </remarks>
        /// <param name="id">Identificador único da vaga</param>
        /// <param name="dto">Dados da vaga a serem atualizados</param>
        /// <response code="200">Vaga atualizada com sucesso</response>
        /// <response code="404">Vaga não encontrada</response>
        [HttpPut("{id}")]
        [SwaggerRequestExample(typeof(VagaDTO), typeof(GeoSense.API.Examples.VagaDTOExample))]
        [SwaggerResponse(200, "Vaga atualizada com sucesso", typeof(object))]
        [SwaggerResponse(404, "Vaga não encontrada")]
        public async Task<ActionResult<VagaDTO>> PutVaga(long id, VagaDTO dto)
        {
            var vaga = await _context.Vagas
                .Include(v => v.Motos)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });

            // Verifica se já existe vaga com mesmo número no mesmo pátio (exceto esta vaga)
            var vagaExistente = await _context.Vagas
                .CountAsync(v => v.Numero == dto.Numero && v.PatioId == dto.PatioId && v.Id != id) > 0;

            if (vagaExistente)
                return BadRequest(new { mensagem = "Já existe uma vaga com esse número neste pátio." });

            vaga.GetType().GetProperty("Numero")?.SetValue(vaga, dto.Numero);
            vaga.GetType().GetProperty("Tipo")?.SetValue(vaga, (TipoVaga)dto.Tipo);
            vaga.GetType().GetProperty("Status")?.SetValue(vaga, (StatusVaga)dto.Status);
            vaga.GetType().GetProperty("PatioId")?.SetValue(vaga, dto.PatioId);

            await _context.SaveChangesAsync();

            var vagaAtualizada = await _context.Vagas
                .Include(v => v.Motos)
                .FirstOrDefaultAsync(v => v.Id == id);
            var resultDto = _mapper.Map<VagaDTO>(vagaAtualizada);

            return Ok(new
            {
                mensagem = "Vaga atualizada com sucesso.",
                dados = resultDto
            });
        }

        /// <summary>
        /// Exclui uma vaga do sistema.
        /// </summary>
        /// <remarks>
        /// Remove a vaga informada pelo ID.
        /// </remarks>
        /// <param name="id">Identificador único da vaga</param>
        /// <response code="200">Vaga removida</response>
        /// <response code="404">Vaga não encontrada</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, "Vaga removida com sucesso", typeof(object))]
        [SwaggerResponse(404, "Vaga não encontrada")]
        public async Task<IActionResult> DeleteVaga(long id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });

            _context.Vagas.Remove(vaga);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensagem = "Vaga deletada com sucesso."
            });
        }
    }
}