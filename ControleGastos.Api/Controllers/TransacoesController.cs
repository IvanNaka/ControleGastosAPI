using Microsoft.AspNetCore.Mvc;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.DTOs;
using ControleGastos.Domain.Enums;

namespace ControleGastos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;

        public TransacoesController(ITransacaoService transacaoService)
        {
            _transacaoService = transacaoService;
        }


        /// <summary>
        /// Obtém todas as transações de um usuário
        /// </summary>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<TransacaoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransacaoDto>>> GetTransacoesByUsuario(Guid usuarioId)
        {
            var transacoes = await _transacaoService.GetByUsuarioIdAsync(usuarioId);
            return Ok(transacoes);
        }


        /// <summary>
        /// Cria uma nova transação
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TransacaoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransacaoDto>> CreateTransacao([FromBody] CreateTransacaoDto createDto)
        {
            try
            {
                var transacao = await _transacaoService.CreateAsync(createDto);
                return CreatedAtAction(nameof(CreateTransacao), new { id = transacao.Id }, transacao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma transação
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
