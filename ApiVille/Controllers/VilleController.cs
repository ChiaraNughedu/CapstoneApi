using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiVille.Services;
using ApiVille.DTOs;


namespace ApiVille.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VilleController : ControllerBase
    {
        private readonly VillaService _villaService;

        public VilleController(VillaService villaService)
        {
            _villaService = villaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVille()
        {
            var ville = await _villaService.GetVilleAsync();
            return Ok(ville);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVilla(int id)
        {
            var villa = await _villaService.GetVillaByIdAsync(id);
            if (villa == null) return NotFound();
            return Ok(villa);
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> GetVilleByCategoria(int categoriaId)
        {
            var result = await _villaService.GetVilleByCategoriaAsync(categoriaId);
            if (result == null) return NotFound(new { message = "Categoria non trovata" });
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVilla([FromBody] VillaCreateDto villaCreateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _villaService.CreateVillaAsync(villaCreateDto);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetVilla), new { id = result.Result?.Id }, result.Result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaCreateDto villaCreateDto)
        {
            var result = await _villaService.UpdateVillaAsync(id, villaCreateDto);
            if (!result) return BadRequest("Errore durante l'aggiornamento della villa.");
            return NoContent();
        }

        [HttpPatch("{id}/categoria")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVillaCategoria(int id, [FromBody] int categoriaId)
        {
            var result = await _villaService.UpdateVillaCategoriaAsync(id, categoriaId);
            if (!result) return BadRequest("Errore durante l'aggiornamento della categoria della villa.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            var deleted = await _villaService.DeleteVillaAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPatch("aggiorna-categorie/{categoriaId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAllVilleCategorie(int categoriaId)
        {
            var result = await _villaService.UpdateAllVilleCategorieAsync(categoriaId);

            if (!result.success)
                return BadRequest(new { message = result.message });

            if (result.updatedCount == 0)
                return NotFound(new { message = result.message });

            return Ok(new { message = $"Aggiornate {result.updatedCount} ville alla categoria specificata" });
        }
    }
}
