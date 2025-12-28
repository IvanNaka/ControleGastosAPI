using Microsoft.AspNetCore.Mvc;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.DTOs;

namespace ControleGastos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }


        /// <summary>
        /// Obtém todas as categorias de um usuário
        /// </summary>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategoriasByUsuario(Guid usuarioId)
        {
            var categorias = await _categoriaService.GetByUsuarioIdAsync(usuarioId);
            return Ok(categorias);
        }


        /// <summary>
        /// Cria uma nova categoria
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria([FromBody] CreateCategoriaDto createDto)
        {
            try
            {
                var categoria = await _categoriaService.CreateAsync(createDto);
                return CreatedAtAction(nameof(CreateCategoria), new { id = categoria.Id }, categoria);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Deleta permanentemente uma categoria
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
