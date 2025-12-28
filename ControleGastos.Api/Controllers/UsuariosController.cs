using Microsoft.AspNetCore.Mvc;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.DTOs;

namespace ControleGastos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Cria um novo usuário ou retorna o existente se já cadastrado
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsuarioDto>> CreateUsuario([FromBody] CreateUsuarioDto createDto)
        {
            var usuario = await _usuarioService.CreateAsync(createDto);
            return Ok(usuario);
        }
    }
}
