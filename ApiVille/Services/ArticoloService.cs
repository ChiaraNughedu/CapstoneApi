using ApiVille.Data;
using ApiVille.Dtos;
using ApiVille.DTOs;
using ApiVille.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiVille.Services
{
    public class ArticoloService
    {
        private readonly ApplicationDbContext _context;

        public ArticoloService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ArticoloDto>> GetAllAsync()
        {
            var articoli = await _context.Articoli.ToListAsync();

            return articoli.Select(a => new ArticoloDto
            {
                Id = a.Id,
                Luogo = a.Luogo,
                ImageUrl = a.ImageUrl,
                Descrizione1 = a.Descrizione1,
                Descrizione2 = a.Descrizione2
            }).ToList();
        }

        public async Task<ArticoloDto?> GetByIdAsync(int id)
        {
            var articolo = await _context.Articoli.FindAsync(id);
            if (articolo == null)
                return null;

            return new ArticoloDto
            {
                Id = articolo.Id,
                Luogo = articolo.Luogo,
                ImageUrl = articolo.ImageUrl,
                Descrizione1 = articolo.Descrizione1,
                Descrizione2 = articolo.Descrizione2
            };
        }

        public async Task<ArticoloDto> CreateAsync(ArticoloCreateDto dto)
        {
            var articolo = new Articolo
            {
                Luogo = dto.Luogo,
                ImageUrl = dto.ImageUrl,
                Descrizione1 = dto.Descrizione1,
                Descrizione2 = dto.Descrizione2
            };

            _context.Articoli.Add(articolo);
            await _context.SaveChangesAsync();

            return new ArticoloDto
            {
                Id = articolo.Id,
                Luogo = articolo.Luogo,
                ImageUrl = articolo.ImageUrl,
                Descrizione1 = articolo.Descrizione1,
                Descrizione2 = articolo.Descrizione2
            };
        }

        public async Task<bool> UpdateAsync(int id, ArticoloCreateDto dto)
        {
            var articolo = await _context.Articoli.FindAsync(id);
            if (articolo == null)
                return false;

            articolo.Luogo = dto.Luogo;
            articolo.ImageUrl = dto.ImageUrl;
            articolo.Descrizione1 = dto.Descrizione1;
            articolo.Descrizione2 = dto.Descrizione2;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var articolo = await _context.Articoli.FindAsync(id);
            if (articolo == null)
                return false;

            _context.Articoli.Remove(articolo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
