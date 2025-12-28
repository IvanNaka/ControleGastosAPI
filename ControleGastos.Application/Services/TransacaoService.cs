using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Interfaces.Repositories;

namespace ControleGastos.Application.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransacaoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<TransacaoDto>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            var transacoes = await _unitOfWork.Transacoes.GetByUsuarioIdAsync(usuarioId);
            return transacoes.Select(MapToDto);
        }

        public async Task<TransacaoDto> CreateAsync(CreateTransacaoDto createDto)
        {
            // Valida se usuário existe
            var usuarioExists = await _unitOfWork.Usuarios.AnyAsync(u => u.Id == createDto.UsuarioId);
            if (!usuarioExists)
            {
                throw new ArgumentException($"Usuario with Id {createDto.UsuarioId} not found");
            }

            // Valida se pessoa existe
            var pessoaExists = await _unitOfWork.Pessoas.AnyAsync(p => p.Id == createDto.PessoaId);
            if (!pessoaExists)
            {
                throw new ArgumentException($"Pessoa with Id {createDto.PessoaId} not found");
            }

            // Validate se categoria existe
            var categoriaExists = await _unitOfWork.Categorias.AnyAsync(c => c.Id == createDto.CategoriaId);
            if (!categoriaExists)
            {
                throw new ArgumentException($"Categoria with Id {createDto.CategoriaId} not found");
            }

            var transacao = new Transacao
            {
                Descricao = createDto.Descricao,
                Valor = createDto.Valor,
                Tipo = createDto.Tipo,
                PessoaId = createDto.PessoaId,
                CategoriaId = createDto.CategoriaId,
                UsuarioId = createDto.UsuarioId
            };

            await _unitOfWork.Transacoes.AddAsync(transacao);
            await _unitOfWork.CompleteAsync();

            var createdTransacao = await _unitOfWork.Transacoes.GetByIdWithDetailsAsync(transacao.Id);
            return MapToDto(createdTransacao!);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var transacao = await _unitOfWork.Transacoes.GetByIdAsync(id);
            if (transacao == null)
            {
                return false;
            }

            // Soft delete: set Ativo = false 
            transacao.Deletar();
            transacao.UltimaAtualizacao = DateTime.UtcNow;
            
            _unitOfWork.Transacoes.Update(transacao);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private static TransacaoDto MapToDto(Transacao transacao)
        {
            return new TransacaoDto
            {
                Id = transacao.Id,
                Descricao = transacao.Descricao,
                Valor = transacao.Valor,
                Tipo = transacao.Tipo,
                PessoaId = transacao.PessoaId,
                PessoaNome = transacao.Pessoa?.Nome,
                CategoriaId = transacao.CategoriaId,
                CategoriaDescricao = transacao.Categoria?.Descricao,
                UsuarioId = transacao.UsuarioId
            };
        }
    }
}
