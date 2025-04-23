using ApiVille.DTOs;
using ApiVille.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiVille.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorieController : ControllerBase
    {
        private readonly CategoriaService _categoriaService;

        public CategorieController(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorie()
        {
            var categorieDto = await _categoriaService.GetCategorieAsync();
            return Ok(categorieDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            var categoriaDto = await _categoriaService.GetCategoriaByIdAsync(id);
            if (categoriaDto == null) return NotFound();
            return categoriaDto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria(CategoriaCreateDto categoriaDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _categoriaService.CreateCategoriaAsync(categoriaDto);
            return CreatedAtAction(nameof(GetCategoria), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategoria(int id, CategoriaCreateDto categoriaDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _categoriaService.UpdateCategoriaAsync(id, categoriaDto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var result = await _categoriaService.DeleteCategoriaAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return NoContent();
        }
    }
}
