using ApiVille.DTOs;
using ApiVille.Models;
using ApiVille.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiVille.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrenotazioniController : ControllerBase
    {
        private readonly PrenotazioniService _prenotazioniService;

        public PrenotazioniController(PrenotazioniService prenotazioniService)
        {
            _prenotazioniService = prenotazioniService;
        }

        [HttpGet("tutte")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PrenotazioneDto>>> GetTutte()
        {
            var result = await _prenotazioniService.GetTutteAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PrenotazioneDto>> GetPrenotazione(int id)
        {
            var result = await _prenotazioniService.GetByIdAsync(id, User);
            if (result == null) return Forbid();
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PrenotazioneDto>> CreatePrenotazione([FromBody] PrenotazioneCreateDto dto)
        {
            var result = await _prenotazioniService.CreateAsync(dto, User);
            if (!result.Success) return BadRequest(new { message = result.Error });
            return CreatedAtAction(nameof(GetPrenotazione), new { id = result.Prenotazione!.Id }, result.Prenotazione);
        }

        [HttpGet("mie")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PrenotazioneDto>>> GetMie()
        {
            var result = await _prenotazioniService.GetMieAsync(User);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> ModificaPrenotazione(int id, [FromBody] PrenotazioneCreateDto dto)
        {
            var result = await _prenotazioniService.ModificaAsync(id, dto, User);

            if (!result.Success)
            {
                if (result.Error == "Prenotazione non trovata") return NotFound(new { message = result.Error });
                if (result.Error == "Non autorizzato") return Forbid();
                return BadRequest(new { message = result.Error });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> EliminaPrenotazione(int id)
        {
            var result = await _prenotazioniService.EliminaAsync(id, User);

            if (!result.Success)
            {
                if (result.Error == "Prenotazione non trovata") return NotFound(new { message = result.Error });
                if (result.Error == "Non autorizzato") return Forbid();
                return BadRequest(new { message = result.Error });
            }

            return NoContent();
        }
    }
}
