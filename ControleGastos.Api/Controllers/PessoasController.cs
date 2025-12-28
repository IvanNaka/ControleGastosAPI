using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.DTOs;

namespace ControleGastos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;

        public PessoasController(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        /// <summary>
        /// Obtém todas as pessoas de um usuário
        /// </summary>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<PessoaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PessoaDto>>> GetPessoasByUsuario(Guid usuarioId)
        {
            var pessoas = await _pessoaService.GetByUsuarioIdAsync(usuarioId);
            return Ok(pessoas);
        }


        /// <summary>
        /// Cria uma nova pessoa
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PessoaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PessoaDto>> CreatePessoa([FromBody] CreatePessoaDto createDto)
        {
            try
            {
                var pessoa = await _pessoaService.CreateAsync(createDto);
                return CreatedAtAction(nameof(CreatePessoa), new { id = pessoa.Id }, pessoa);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma pessoa
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePessoa(Guid id)
        {
            var deleted = await _pessoaService.DeleteAsync(id);
            
            if (!deleted)
            {
                return NotFound(new { message = $"Pessoa com ID {id} não encontrada" });
            }

            return NoContent();
        }
    }
}
