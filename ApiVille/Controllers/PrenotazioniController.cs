using ApiVille.Data;
using ApiVille.DTOs;
using ApiVille.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiVille.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrenotazioniController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PrenotazioniController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("tutte")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PrenotazioneDto>>> GetTutte()
        {
            var prenotazioni = await _context.Prenotazioni
                .Include(p => p.User)
                .Include(p => p.Villa)
                .ToListAsync();

            var dtoList = prenotazioni.Select(p => new PrenotazioneDto
            {
                Id = p.Id,
                VillaId = p.VillaId,
                NomeVilla = p.Villa?.NomeVilla ?? "N/A",
                UserId = p.UserId,
                Nome = p.User?.Nome ?? "N/A",
                Cognome = p.User?.Cognome ?? "N/A",
                UserEmail = p.User?.Email ?? "N/A",
                DataInizio = p.DataInizio,
                DataFine = p.DataFine,
                PrezzoTotale = p.PrezzoTotale
            });

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PrenotazioneDto>> GetPrenotazione(int id)
        {
            var prenotazione = await _context.Prenotazioni
                .Include(p => p.User)
                .Include(p => p.Villa)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prenotazione == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && prenotazione.UserId != userId)
            {
                return Forbid();
            }

            var prenotazioneDto = new PrenotazioneDto
            {
                Id = prenotazione.Id,
                VillaId = prenotazione.VillaId,
                NomeVilla = prenotazione.Villa?.NomeVilla ?? "N/A",
                UserId = prenotazione.UserId,
                Nome = prenotazione.User?.Nome ?? "N/A",
                Cognome = prenotazione.User?.Cognome ?? "N/A",
                UserEmail = prenotazione.User?.Email ?? "N/A",
                DataInizio = prenotazione.DataInizio,
                DataFine = prenotazione.DataFine,
                PrezzoTotale = prenotazione.PrezzoTotale
            };

            return Ok(prenotazioneDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PrenotazioneDto>> CreatePrenotazione([FromBody] PrenotazioneCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var villa = await _context.Ville.FindAsync(dto.VillaId);
            if (villa == null)
            {
                return BadRequest(new { message = "Villa non trovata" });
            }

            var giorni = (dto.DataFine - dto.DataInizio).Days;
            if (giorni <= 0)
            {
                return BadRequest(new { message = "La data di fine deve essere successiva alla data di inizio" });
            }

            var prezzoTotale = villa.Prezzo * giorni;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "Utente non trovato" });
            }

            var sovrapposizioni = await _context.Prenotazioni
                .Where(p => p.VillaId == dto.VillaId &&
                           ((p.DataInizio <= dto.DataInizio && p.DataFine >= dto.DataInizio) ||
                            (p.DataInizio <= dto.DataFine && p.DataFine >= dto.DataFine) ||
                            (p.DataInizio >= dto.DataInizio && p.DataFine <= dto.DataFine)))
                .AnyAsync();

            if (sovrapposizioni)
            {
                return BadRequest(new { message = "La villa non è disponibile nel periodo selezionato" });
            }

            var prenotazione = new Prenotazione
            {
                VillaId = dto.VillaId,
                UserId = userId,
                Username = user.UserName,
                Nome = user.Nome,
                Cognome = user.Cognome ?? "Non specificato",
                DataInizio = dto.DataInizio,
                DataFine = dto.DataFine,
                PrezzoTotale = prezzoTotale
            };

            _context.Prenotazioni.Add(prenotazione);
            await _context.SaveChangesAsync();

            await _context.Entry(prenotazione).Reference(p => p.Villa).LoadAsync();
            await _context.Entry(prenotazione).Reference(p => p.User).LoadAsync();

            var responseDto = new PrenotazioneDto
            {
                Id = prenotazione.Id,
                VillaId = prenotazione.VillaId,
                NomeVilla = prenotazione.Villa?.NomeVilla ?? "N/A",
                UserId = prenotazione.UserId,
                Nome = prenotazione.User?.Nome ?? "N/A",
                Cognome = prenotazione.User?.Cognome ?? "N/A",
                UserEmail = prenotazione.User?.Email ?? "N/A",
                DataInizio = prenotazione.DataInizio,
                DataFine = prenotazione.DataFine,
                PrezzoTotale = prenotazione.PrezzoTotale
            };

            return CreatedAtAction(nameof(GetPrenotazione), new { id = prenotazione.Id }, responseDto);
        }

        [HttpGet("mie")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PrenotazioneDto>>> GetMie()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var prenotazioni = await _context.Prenotazioni
                .Where(p => p.UserId == userId)
                .Include(p => p.Villa)
                .Include(p => p.User)
                .ToListAsync();

            var dtoList = prenotazioni.Select(p => new PrenotazioneDto
            {
                Id = p.Id,
                VillaId = p.VillaId,
                NomeVilla = p.Villa?.NomeVilla ?? "N/A",
                UserId = p.UserId,
                Nome = p.User?.Nome ?? "N/A",
                Cognome = p.User?.Cognome ?? "N/A",
                UserEmail = p.User?.Email ?? "N/A",
                DataInizio = p.DataInizio,
                DataFine = p.DataFine,
                PrezzoTotale = p.PrezzoTotale
            });

            return Ok(dtoList);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> ModificaPrenotazione(int id, [FromBody] PrenotazioneCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var prenotazione = await _context.Prenotazioni.FindAsync(id);
            if (prenotazione == null)
            {
                return NotFound(new { message = "Prenotazione non trovata" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && prenotazione.UserId != userId)
            {
                return Forbid();
            }

            var villa = await _context.Ville.FindAsync(dto.VillaId);
            if (villa == null)
            {
                return BadRequest(new { message = "Villa non trovata" });
            }

            var giorni = (dto.DataFine - dto.DataInizio).Days;
            if (giorni <= 0)
            {
                return BadRequest(new { message = "La data di fine deve essere successiva alla data di inizio" });
            }

            var sovrapposizioni = await _context.Prenotazioni
                .Where(p => p.Id != id &&
                           p.VillaId == dto.VillaId &&
                           ((p.DataInizio <= dto.DataInizio && p.DataFine >= dto.DataInizio) ||
                            (p.DataInizio <= dto.DataFine && p.DataFine >= dto.DataFine) ||
                            (p.DataInizio >= dto.DataInizio && p.DataFine <= dto.DataFine)))
                .AnyAsync();

            if (sovrapposizioni)
            {
                return BadRequest(new { message = "La villa non è disponibile nel periodo selezionato" });
            }

            var prezzoTotale = villa.Prezzo * giorni;

            prenotazione.VillaId = dto.VillaId;
            prenotazione.DataInizio = dto.DataInizio;
            prenotazione.DataFine = dto.DataFine;
            prenotazione.PrezzoTotale = prezzoTotale;

            _context.Prenotazioni.Update(prenotazione);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> EliminaPrenotazione(int id)
        {
            var prenotazione = await _context.Prenotazioni.FindAsync(id);
            if (prenotazione == null)
            {
                return NotFound(new { message = "Prenotazione non trovata" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && prenotazione.UserId != userId)
            {
                return Forbid();
            }

            _context.Prenotazioni.Remove(prenotazione);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
