using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Interfaces.Repositories;

namespace ControleGastos.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoriaDto>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            var categorias = await _unitOfWork.Categorias.GetByUsuarioIdAsync(usuarioId);
            return categorias.Select(MapToDto);
        }

        public async Task<CategoriaDto> CreateAsync(CreateCategoriaDto createDto)
        {
            // Valida se usuario existe
            var usuarioExists = await _unitOfWork.Usuarios.AnyAsync(u => u.Id == createDto.UsuarioId);
            if (!usuarioExists)
            {
                throw new ArgumentException($"Usuario with Id {createDto.UsuarioId} not found");
            }

            var categoria = new Categoria
            {
                Descricao = createDto.Descricao,
                Finalidade = createDto.Finalidade,
                UsuarioId = createDto.UsuarioId
            };

            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.CompleteAsync();

            return MapToDto(categoria);
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(id);
            if (categoria == null)
            {
                return false;
            }

            // Soft delete: set Ativo = false 
            categoria.Deletar();
            categoria.UltimaAtualizacao = DateTime.UtcNow;
            
            _unitOfWork.Categorias.Update(categoria);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private static CategoriaDto MapToDto(Categoria categoria)
        {
            return new CategoriaDto
            {
                Id = categoria.Id,
                Descricao = categoria.Descricao,
                Finalidade = categoria.Finalidade,
                UsuarioId = categoria.UsuarioId,
                DataCriacao = categoria.DataCriacao,
                UltimaAtualizacao = categoria.UltimaAtualizacao,
                Ativo = categoria.Ativo
            };
        }
    }
}
