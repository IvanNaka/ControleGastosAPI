using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Interfaces.Repositories;

namespace ControleGastos.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public async Task<UsuarioDto?> GetByAzureAdIdAsync(string azureAdId)
        {
            var usuario = await _unitOfWork.Usuarios.GetByAzureAdIdAsync(azureAdId);
            return usuario != null ? MapToDto(usuario) : null;
        }

        public async Task<UsuarioDto> CreateAsync(CreateUsuarioDto createDto)
        {
            // Verifica se o usuário já está cadastrado por AzureAd
            var usuarioPorAzureId = await _unitOfWork.Usuarios.GetByAzureAdIdAsync(createDto.AzureAdId);
            if (usuarioPorAzureId != null && usuarioPorAzureId.Ativo == true)
            {                
                return MapToDto(usuarioPorAzureId);
            }

            // Se não existe, cria novo usuário
            var usuarioNovo = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = createDto.Nome,
                Email = createDto.Email,
                AzureAdId = createDto.AzureAdId
            };

            await _unitOfWork.Usuarios.AddAsync(usuarioNovo);
            await _unitOfWork.CompleteAsync();

            return MapToDto(usuarioNovo);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
            if (usuario == null)
            {
                return false;
            }

            // Soft delete: set Ativo = false
            usuario.Deletar();
            usuario.UltimaAtualizacao = DateTime.UtcNow;
            
            _unitOfWork.Usuarios.Update(usuario);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private static UsuarioDto MapToDto(Usuario usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                AzureAdId = usuario.AzureAdId
            };
        }
    }
}
