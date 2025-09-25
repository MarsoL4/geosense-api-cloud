using AutoMapper;
using GeoSense.API.Domain.Enums;
using GeoSense.API.DTOs;
using GeoSense.API.DTOs.Usuario;
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
    public class UsuarioController(GeoSenseContext context, IMapper mapper) : ControllerBase
    {
        private readonly GeoSenseContext _context = context;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Retorna uma lista paginada de usuários cadastrados.
        /// </summary>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
        /// <response code="200">Lista paginada de usuários</response>
        [HttpGet]
        [SwaggerResponse(200, "Lista paginada de usuários cadastrados", typeof(PagedHateoasDTO<UsuarioDTO>))]
        public async Task<ActionResult<PagedHateoasDTO<UsuarioDTO>>> GetUsuarios([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Usuarios.AsQueryable();
            var totalCount = await query.CountAsync();
            var usuarios = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<UsuarioDTO>>(usuarios);

            var links = HateoasHelper.GetPagedLinks(Url, "Usuarios", page, pageSize, totalCount);

            var result = new PagedHateoasDTO<UsuarioDTO>
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
        /// Retorna os dados de um usuário por ID.
        /// </summary>
        /// <param name="id">Identificador único do usuário</param>
        /// <response code="200">Usuário encontrado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Usuário encontrado", typeof(UsuarioDTO))]
        [SwaggerResponse(404, "Usuário não encontrado")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            var dto = _mapper.Map<UsuarioDTO>(usuario);
            return Ok(dto);
        }

        /// <summary>
        /// Cadastra um novo usuário.
        /// </summary>
        /// <param name="dto">Dados do novo usuário</param>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Email já cadastrado</response>
        [HttpPost]
        [SwaggerRequestExample(typeof(UsuarioDTO), typeof(GeoSense.API.Examples.UsuarioDTOExample))]
        [SwaggerResponse(201, "Usuário criado com sucesso", typeof(object))]
        [SwaggerResponse(400, "Email já cadastrado")]
        public async Task<ActionResult<UsuarioDTO>> PostUsuario(UsuarioDTO dto)
        {
            var emailExiste = await _context.Usuarios.CountAsync(u => u.Email == dto.Email) > 0;

            if (emailExiste)
                return BadRequest(new { mensagem = "Já existe um usuário com esse email." });

            var novoUsuario = new Usuario(0, dto.Nome, dto.Email, dto.Senha, (TipoUsuario)dto.Tipo);
            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            var usuarioCompleto = await _context.Usuarios.FindAsync(novoUsuario.Id);
            var resultDto = _mapper.Map<UsuarioDTO>(usuarioCompleto);

            return CreatedAtAction(nameof(GetUsuario), new { id = novoUsuario.Id }, new
            {
                mensagem = "Usuário cadastrado com sucesso.",
                dados = resultDto
            });
        }

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        /// <param name="id">Identificador único do usuário</param>
        /// <param name="dto">Dados do usuário a serem atualizados</param>
        /// <response code="200">Usuário atualizado com sucesso</response>
        /// <response code="400">Email já cadastrado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPut("{id}")]
        [SwaggerRequestExample(typeof(UsuarioDTO), typeof(GeoSense.API.Examples.UsuarioDTOExample))]
        [SwaggerResponse(200, "Usuário atualizado com sucesso", typeof(object))]
        [SwaggerResponse(400, "Email já cadastrado")]
        [SwaggerResponse(404, "Usuário não encontrado")]
        public async Task<IActionResult> PutUsuario(long id, UsuarioDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            var emailExiste = await _context.Usuarios.CountAsync(u => u.Email == dto.Email && u.Id != id) > 0;

            if (emailExiste)
                return BadRequest(new { mensagem = "Já existe um usuário com esse email." });

            _context.Entry(usuario).State = EntityState.Detached;
            var usuarioAtualizado = new Usuario(id, dto.Nome, dto.Email, dto.Senha, (TipoUsuario)dto.Tipo);

            _context.Usuarios.Update(usuarioAtualizado);
            await _context.SaveChangesAsync();

            var usuarioAtualizadoCompleto = await _context.Usuarios.FindAsync(id);
            var resultDto = _mapper.Map<UsuarioDTO>(usuarioAtualizadoCompleto);

            return Ok(new
            {
                mensagem = "Usuário atualizado com sucesso.",
                dados = resultDto
            });
        }

        /// <summary>
        /// Exclui um usuário do sistema.
        /// </summary>
        /// <param name="id">Identificador único do usuário</param>
        /// <response code="200">Usuário removido</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, "Usuário removido com sucesso", typeof(object))]
        [SwaggerResponse(404, "Usuário não encontrado")]
        public async Task<IActionResult> DeleteUsuario(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensagem = "Usuário deletado com sucesso."
            });
        }
    }
}