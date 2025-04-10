using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using ApiVille.Data;
using ApiVille.DTOs;
using ApiVille.Models;

namespace ApiVille.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VilleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VilleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetVille()
        {
            var ville = await _context.Ville
                .Include(v => v.Categoria)
                .Select(v => new VillaDto
                {
                    Id = v.Id,
                    NomeVilla = v.NomeVilla,
                    Immagine1 = v.Immagine1,
                    Immagine2 = v.Immagine2,
                    Immagine3 = v.Immagine3,
                    Immagine4 = v.Immagine4,
                    Prezzo = v.Prezzo,
                    Localita = v.Localita,
                    Descrizione = v.Descrizione,
                    CategoriaId = v.CategoriaId,
                    NomeCategoria = v.Categoria.NomeCategoria
                })
                .ToListAsync();

            return Ok(ville);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVilla(int id)
        {
            var v = await _context.Ville
                .Include(v => v.Categoria)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (v == null)
                return NotFound();

            var villaDto = new VillaDto
            {
                Id = v.Id,
                NomeVilla = v.NomeVilla,
                Immagine1 = v.Immagine1,
                Immagine2 = v.Immagine2,
                Immagine3 = v.Immagine3,
                Immagine4 = v.Immagine4,
                Prezzo = v.Prezzo,
                Localita = v.Localita,
                Descrizione = v.Descrizione,
                CategoriaId = v.CategoriaId,
                NomeCategoria = v.Categoria?.NomeCategoria ?? "N/A"
            };

            return Ok(villaDto);
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> GetVilleByCategoria(int categoriaId)
        {
            var categoria = await _context.Categorie.FindAsync(categoriaId);
            if (categoria == null)
                return NotFound(new { message = "Categoria non trovata" });

            var ville = await _context.Ville
                .Where(v => v.CategoriaId == categoriaId)
                .Include(v => v.Categoria)
                .Select(v => new VillaDto
                {
                    Id = v.Id,
                    NomeVilla = v.NomeVilla,
                    Immagine1 = v.Immagine1,
                    Immagine2 = v.Immagine2,
                    Immagine3 = v.Immagine3,
                    Immagine4 = v.Immagine4,
                    Prezzo = v.Prezzo,
                    Localita = v.Localita,
                    Descrizione = v.Descrizione,
                    CategoriaId = v.CategoriaId,
                    NomeCategoria = v.Categoria.NomeCategoria
                })
                .ToListAsync();

            return Ok(ville);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVilla([FromBody] VillaDto villaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriaEsiste = await _context.Categorie.AnyAsync(c => c.Id == villaDto.CategoriaId);
            if (!categoriaEsiste)
            {
                return BadRequest(new { message = "La categoria specificata non esiste" });
            }

            var villa = new Villa
            {
                NomeVilla = villaDto.NomeVilla,
                Immagine1 = villaDto.Immagine1,
                Immagine2 = villaDto.Immagine2,
                Immagine3 = villaDto.Immagine3,
                Immagine4 = villaDto.Immagine4,
                Prezzo = villaDto.Prezzo,
                Localita = villaDto.Localita,
                CategoriaId = villaDto.CategoriaId,
                Descrizione = villaDto.Descrizione
            };

            _context.Ville.Add(villa);
            await _context.SaveChangesAsync();

            villaDto.Id = villa.Id;

            return CreatedAtAction(nameof(GetVilla), new { id = villa.Id }, villaDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            if (id != villaDto.Id)
                return BadRequest("L'ID della villa non corrisponde");

            var villa = await _context.Ville.FindAsync(id);
            if (villa == null)
                return NotFound();


            if (villaDto.CategoriaId > 0)
            {
                var categoriaEsiste = await _context.Categorie.AnyAsync(c => c.Id == villaDto.CategoriaId);
                if (!categoriaEsiste)
                {
                    return BadRequest(new { message = "La categoria specificata non esiste" });
                }
                villa.CategoriaId = villaDto.CategoriaId;
            }

            villa.NomeVilla = villaDto.NomeVilla;
            villa.Immagine1 = villaDto.Immagine1;
            villa.Immagine2 = villaDto.Immagine2;
            villa.Immagine3 = villaDto.Immagine3;
            villa.Immagine4 = villaDto.Immagine4;
            villa.Prezzo = villaDto.Prezzo;
            villa.Localita = villaDto.Localita;
            villa.Descrizione = villaDto.Descrizione;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/categoria")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVillaCategoria(int id, [FromBody] int categoriaId)
        {
            var villa = await _context.Ville.FindAsync(id);
            if (villa == null)
                return NotFound(new { message = "Villa non trovata" });

            var categoriaEsiste = await _context.Categorie.AnyAsync(c => c.Id == categoriaId);
            if (!categoriaEsiste)
                return BadRequest(new { message = "La categoria specificata non esiste" });

            villa.CategoriaId = categoriaId;

            _context.Ville.Update(villa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            var villa = await _context.Ville.FindAsync(id);
            if (villa == null)
                return NotFound();

            _context.Ville.Remove(villa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("aggiorna-categorie/{categoriaId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAllVilleCategorie(int categoriaId)
        {
            var categoriaEsiste = await _context.Categorie.AnyAsync(c => c.Id == categoriaId);
            if (!categoriaEsiste)
                return BadRequest(new { message = "La categoria specificata non esiste" });

            var villeNonAssegnate = await _context.Ville
                .Where(v => v.CategoriaId == 0 || v.CategoriaId == null)
                .ToListAsync();

            if (!villeNonAssegnate.Any())
                return NotFound(new { message = "Non ci sono ville senza categoria" });

            foreach (var villa in villeNonAssegnate)
            {
                villa.CategoriaId = categoriaId;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Aggiornate {villeNonAssegnate.Count} ville alla categoria specificata" });
        }
    }
}
