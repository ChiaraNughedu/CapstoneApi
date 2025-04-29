using ApiVille.Data;
using ApiVille.DTOs;
using ApiVille.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVille.Services
{
    public class VillaService
    {
        private readonly ApplicationDbContext _context;

        public VillaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VillaDto>> GetVilleAsync()
        {
            return await _context.Ville
                .Include(v => v.Categoria)
                .Select(v => new VillaDto
                {
                    Id = v.Id,
                    NomeVilla = v.NomeVilla,
                    ImgCopertina = v.ImgCopertina,
                    Immagine1 = v.Immagine1,
                    Immagine2 = v.Immagine2,
                    Immagine3 = v.Immagine3,
                    Immagine4 = v.Immagine4,
                    Immagine5 = v.Immagine5,
                    Immagine6 = v.Immagine6,
                    Prezzo = v.Prezzo,
                    Localita = v.Localita,
                    Descrizione = v.Descrizione,
                    CategoriaId = v.CategoriaId,
                    NomeCategoria = v.Categoria.NomeCategoria
                })
                .ToListAsync();
        }

        public async Task<VillaDto?> GetVillaByIdAsync(int id)
        {
            var v = await _context.Ville
                .Include(v => v.Categoria)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (v == null) return null;

            return new VillaDto
            {
                Id = v.Id,
                NomeVilla = v.NomeVilla,
                ImgCopertina = v.ImgCopertina,
                Immagine1 = v.Immagine1,
                Immagine2 = v.Immagine2,
                Immagine3 = v.Immagine3,
                Immagine4 = v.Immagine4,
                Immagine5 = v.Immagine5,
                Immagine6 = v.Immagine6,
                Prezzo = v.Prezzo,
                Localita = v.Localita,
                Descrizione = v.Descrizione,
                CategoriaId = v.CategoriaId,
                NomeCategoria = v.Categoria?.NomeCategoria ?? "N/A"
            };
        }

        public async Task<List<VillaDto>> GetVilleByCategoriaAsync(int categoriaId)
        {
            return await _context.Ville
                .Where(v => v.CategoriaId == categoriaId)
                .Include(v => v.Categoria)
                .Select(v => new VillaDto
                {
                    Id = v.Id,
                    NomeVilla = v.NomeVilla,
                    ImgCopertina = v.ImgCopertina,
                    Immagine1 = v.Immagine1,
                    Immagine2 = v.Immagine2,
                    Immagine3 = v.Immagine3,
                    Immagine4 = v.Immagine4,
                    Immagine5 = v.Immagine5,
                    Immagine6 = v.Immagine6,
                    Prezzo = v.Prezzo,
                    Localita = v.Localita,
                    Descrizione = v.Descrizione,
                    CategoriaId = v.CategoriaId,
                    NomeCategoria = v.Categoria.NomeCategoria
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string? ErrorMessage, VillaDto? Result)> CreateVillaAsync(VillaCreateDto dto)
        {
            if (!await _context.Categorie.AnyAsync(c => c.Id == dto.CategoriaId))
                return (false, "La categoria specificata non esiste", null);

            var villa = new Villa
            {
                NomeVilla = dto.NomeVilla,
                ImgCopertina = dto.ImgCopertina,
                Immagine1 = dto.Immagine1,
                Immagine2 = dto.Immagine2,
                Immagine3 = dto.Immagine3,
                Immagine4 = dto.Immagine4,
                Immagine5 = dto.Immagine5,
                Immagine6 = dto.Immagine6,
                Prezzo = dto.Prezzo,
                Localita = dto.Localita,
                CategoriaId = dto.CategoriaId,
                Descrizione = dto.Descrizione
            };

            _context.Ville.Add(villa);
            await _context.SaveChangesAsync();

            var villaDto = await GetVillaByIdAsync(villa.Id);

            return (true, null, villaDto);
        }

        public async Task<bool> UpdateVillaAsync(int id, VillaCreateDto dto)
        {
            var villa = await _context.Ville.FindAsync(id);
            if (villa == null) return false;

            if (dto.CategoriaId > 0 &&
                !await _context.Categorie.AnyAsync(c => c.Id == dto.CategoriaId))
                return false;

            villa.NomeVilla = dto.NomeVilla;
            villa.ImgCopertina = dto.ImgCopertina;
            villa.Immagine1 = dto.Immagine1;
            villa.Immagine2 = dto.Immagine2;
            villa.Immagine3 = dto.Immagine3;
            villa.Immagine4 = dto.Immagine4;
            villa.Immagine5 = dto.Immagine5;
            villa.Immagine6 = dto.Immagine6;
            villa.Prezzo = dto.Prezzo;
            villa.Localita = dto.Localita;
            villa.Descrizione = dto.Descrizione;
            villa.CategoriaId = dto.CategoriaId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVillaCategoriaAsync(int id, int categoriaId)
        {
            var villa = await _context.Ville.FindAsync(id);
            if (villa == null || !await _context.Categorie.AnyAsync(c => c.Id == categoriaId))
                return false;

            villa.CategoriaId = categoriaId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVillaAsync(int id)
        {
            var villa = await _context.Ville.FindAsync(id);
            if (villa == null) return false;

            _context.Ville.Remove(villa);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool success, string message, int updatedCount)> UpdateAllVilleCategorieAsync(int categoriaId)
        {
            if (!await _context.Categorie.AnyAsync(c => c.Id == categoriaId))
                return (false, "Categoria non trovata", 0);

            var ville = await _context.Ville
                .Where(v => v.CategoriaId == 0 || v.CategoriaId == null)
                .ToListAsync();

            if (!ville.Any())
                return (true, "Non ci sono ville senza categoria", 0);

            foreach (var v in ville)
                v.CategoriaId = categoriaId;

            await _context.SaveChangesAsync();
            return (true, "Categoria aggiornata correttamente", ville.Count);
        }
    }
}
