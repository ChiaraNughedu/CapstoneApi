using ApiVille.Data;
using ApiVille.DTOs;
using ApiVille.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVille.Services
{
    public class CategoriaService
    {
        private readonly ApplicationDbContext _context;

        public CategoriaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriaDto>> GetCategorieAsync()
        {
            return await _context.Categorie
                .Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nome = c.NomeCategoria
                })
                .ToListAsync();
        }

        public async Task<CategoriaDto?> GetCategoriaByIdAsync(int id)
        {
            var categoria = await _context.Categorie.FindAsync(id);
            if (categoria == null) return null;

            return new CategoriaDto
            {
                Id = categoria.Id,
                Nome = categoria.NomeCategoria
            };
        }

        public async Task<CategoriaDto> CreateCategoriaAsync(CategoriaCreateDto dto)
        {
            var categoria = new Categoria
            {
                NomeCategoria = dto.Nome
            };

            _context.Categorie.Add(categoria);
            await _context.SaveChangesAsync();

            return new CategoriaDto
            {
                Id = categoria.Id,
                Nome = categoria.NomeCategoria
            };
        }

        public async Task<bool> UpdateCategoriaAsync(int id, CategoriaCreateDto dto)
        {
            var categoria = await _context.Categorie.FindAsync(id);
            if (categoria == null) return false;

            categoria.NomeCategoria = dto.Nome;
            _context.Categorie.Update(categoria);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string? Message)> DeleteCategoriaAsync(int id)
        {
            var categoria = await _context.Categorie.FindAsync(id);
            if (categoria == null)
                return (false, "Categoria non trovata");

            var villeAssociate = await _context.Ville.AnyAsync(v => v.CategoriaId == id);
            if (villeAssociate)
                return (false, "Non è possibile eliminare la categoria perché ci sono ville associate");

            _context.Categorie.Remove(categoria);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}
