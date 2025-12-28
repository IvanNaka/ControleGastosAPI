using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Interfaces.Repositories;

namespace ControleGastos.Application.Services
{
    public class PessoaService : IPessoaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PessoaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PessoaDto>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            var pessoas = await _unitOfWork.Pessoas.GetByUsuarioIdAsync(usuarioId);
            return pessoas.Select(MapToDto);
        }

        public async Task<PessoaDto> CreateAsync(CreatePessoaDto createDto)
        {
            // Valida se usuário existe
            var usuarioExists = await _unitOfWork.Usuarios.AnyAsync(u => u.Id == createDto.UsuarioId);
            if (!usuarioExists)
            {
                throw new ArgumentException($"Usuario with Id {createDto.UsuarioId} not found");
            }

            var pessoa = new Pessoa
            {
                Nome = createDto.Nome,
                Idade = createDto.Idade,
                UsuarioId = createDto.UsuarioId
            };

            await _unitOfWork.Pessoas.AddAsync(pessoa);
            await _unitOfWork.CompleteAsync();

            return MapToDto(pessoa);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var pessoa = await _unitOfWork.Pessoas.GetByIdAsync(id);
            if (pessoa == null)
            {
                return false;
            }
            var transacoes = await _unitOfWork.Transacoes.GetByPessoaIdAsync(id);
            foreach (var transacao in transacoes) {
                transacao.Deletar();
                transacao.UltimaAtualizacao = DateTime.UtcNow;
            }
            
            pessoa.Deletar();
            pessoa.UltimaAtualizacao = DateTime.UtcNow;
            
            _unitOfWork.Transacoes.UpdateRange(transacoes);
            _unitOfWork.Pessoas.Update(pessoa);
            await _unitOfWork.CompleteAsync();

            return true;
        }
        private static PessoaDto MapToDto(Pessoa pessoa)
        {
            return new PessoaDto
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Idade = pessoa.Idade,
                UsuarioId = pessoa.UsuarioId
            };
        }
    }
}
