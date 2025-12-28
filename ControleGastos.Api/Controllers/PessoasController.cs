using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.DTOs;
using ControleGastos.Api.Extensions;

namespace ControleGastos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;
        private readonly IUsuarioService _usuarioService;

        public PessoasController(IPessoaService pessoaService, IUsuarioService usuarioService)
        {
            _pessoaService = pessoaService;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Obtém todas as pessoas do usuário autenticado
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PessoaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PessoaDto>>> GetPessoas()
        {
            var azureAdId = User.GetAzureAdId();
            var usuario = await _usuarioService.GetByAzureAdIdAsync(azureAdId);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuário não encontrado. Por favor, registre-se primeiro." });
            }

            var pessoas = await _pessoaService.GetByUsuarioIdAsync(usuario.Id);
            return Ok(pessoas);
        }


        /// <summary>
        /// Cria uma nova pessoa para o usuário autenticado
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PessoaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PessoaDto>> CreatePessoa([FromBody] CreatePessoaDto createDto)
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

                var pessoa = await _pessoaService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetPessoas), new { id = pessoa.Id }, pessoa);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma pessoa do usuário autenticado
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
