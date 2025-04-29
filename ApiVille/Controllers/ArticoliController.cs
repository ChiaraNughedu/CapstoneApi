using ApiVille.Dtos;
using ApiVille.DTOs;
using ApiVille.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiVille.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticoliController : ControllerBase
    {
        private readonly ArticoloService _articoloService;

        public ArticoliController(ArticoloService articoloService)
        {
            _articoloService = articoloService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticoloDto>>> GetArticoli()
        {
            var articoli = await _articoloService.GetAllAsync();
            return Ok(articoli);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticoloDto>> GetArticolo(int id)
        {
            var articolo = await _articoloService.GetByIdAsync(id);
            if (articolo == null)
                return NotFound();

            return Ok(articolo);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ArticoloDto>> CreaArticolo([FromBody] ArticoloCreateDto dto)
        {
            var nuovoArticolo = await _articoloService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetArticolo), new { id = nuovoArticolo.Id }, nuovoArticolo);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ModificaArticolo(int id, [FromBody] ArticoloCreateDto dto)
        {
            var risultato = await _articoloService.UpdateAsync(id, dto);
            if (!risultato)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminaArticolo(int id)
        {
            var risultato = await _articoloService.DeleteAsync(id);
            if (!risultato)
                return NotFound();

            return NoContent();
        }
    }
}
