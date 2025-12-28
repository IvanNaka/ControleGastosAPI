using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.DTOs;
using ControleGastos.Domain.Enums;
using ControleGastos.Api.Extensions;

namespace ControleGastos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;
        private readonly IUsuarioService _usuarioService;

        public TransacoesController(ITransacaoService transacaoService, IUsuarioService usuarioService)
        {
            _transacaoService = transacaoService;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Test endpoint to verify authentication works
        /// </summary>
        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new
            {
                Message = "✅ Authentication successful!",
                IsAuthenticated = User.Identity?.IsAuthenticated,
                Name = User.Identity?.Name,
                Claims = claims
            });
        }

        /// <summary>
        /// Obtém todas as transações do usuário autenticado
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransacaoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TransacaoDto>>> GetTransacoes()
        {
            try
            {
                var azureAdId = User.GetAzureAdId();
                var usuario = await _usuarioService.GetByAzureAdIdAsync(azureAdId);

                if (usuario == null)
                {
                    return NotFound(new { 
                        message = "Usuário não encontrado. Por favor, registre-se primeiro.",
                        azureAdId = azureAdId  // Include this for debugging
                    });
                }

                var transacoes = await _transacaoService.GetByUsuarioIdAsync(usuario.Id);
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao buscar transações",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }


        /// <summary>
        /// Cria uma nova transação para o usuário autenticado
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TransacaoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransacaoDto>> CreateTransacao([FromBody] CreateTransacaoDto createDto)
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

                var transacao = await _transacaoService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetTransacoes), new { id = transacao.Id }, transacao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma transação do usuário autenticado
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteTransacao(Guid id)
        {
            var deleted = await _transacaoService.DeleteAsync(id);
            
            if (!deleted)
            {
                return NotFound(new { message = $"Transação com ID {id} não encontrada" });
            }

            return NoContent();
        }
    }
}
