using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.DTOs;
using ControleGastos.Api.Extensions;

namespace ControleGastos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        private readonly IUsuarioService _usuarioService;

        public CategoriasController(ICategoriaService categoriaService, IUsuarioService usuarioService)
        {
            _categoriaService = categoriaService;
            _usuarioService = usuarioService;
        }


        /// <summary>
        /// Obtém todas as categorias do usuário autenticado
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            var azureAdId = User.GetAzureAdId();
            var usuario = await _usuarioService.GetByAzureAdIdAsync(azureAdId);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuário não encontrado. Por favor, registre-se primeiro." });
            }

            var categorias = await _categoriaService.GetByUsuarioIdAsync(usuario.Id);
            return Ok(categorias);
        }


        /// <summary>
        /// Cria uma nova categoria para o usuário autenticado
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria([FromBody] CreateCategoriaDto createDto)
        {
            try
            {
                var azureAdId = User.GetAzureAdId();
                var usuario = await _usuarioService.GetByAzureAdIdAsync(azureAdId);

                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado. Por favor, registre-se primeiro." });
                }

                // Sobrescreve o UsuarioId com o ID do usuário autenticado
                createDto.UsuarioId = usuario.Id;

                var categoria = await _categoriaService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetCategorias), new { id = categoria.Id }, categoria);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Deleta uma categoria do usuário autenticado
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCategoria(Guid id)
        {
            var deleted = await _categoriaService.DeleteAsync(id);
            
            if (!deleted)
            {
                return NotFound(new { message = $"Categoria com ID {id} não encontrada" });
            }

            return NoContent();
        }
    }
}
