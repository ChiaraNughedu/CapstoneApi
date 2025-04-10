using ApiVille.Data;
using ApiVille.DTOs;
using ApiVille.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVille.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategorieController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorie()
        {
            var categorie = await _context.Categorie.ToListAsync();

            var categorieDto = categorie.Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nome = c.NomeCategoria
            });

            return Ok(categorieDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            var categoria = await _context.Categorie.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            var categoriaDto = new CategoriaDto
            {
                Id = categoria.Id,
                Nome = categoria.NomeCategoria
            };

            return categoriaDto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria(CategoriaCreateDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = new Categoria
            {
                NomeCategoria = categoriaDto.Nome
            };

            _context.Categorie.Add(categoria);
            await _context.SaveChangesAsync();

            var result = new CategoriaDto
            {
                Id = categoria.Id,
                Nome = categoria.NomeCategoria
            };

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategoria(int id, CategoriaCreateDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = await _context.Categorie.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            categoria.NomeCategoria = categoriaDto.Nome;

            _context.Categorie.Update(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorie.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            var villeAssociate = await _context.Ville.AnyAsync(v => v.CategoriaId == id);
            if (villeAssociate)
            {
                return BadRequest(new { message = "Non è possibile eliminare la categoria perché ci sono ville associate" });
            }

            _context.Categorie.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
