using ApiVille.Data;
using ApiVille.DTOs;
using ApiVille.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiVille.Services
{
    public class PrenotazioniService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PrenotazioniService(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<PrenotazioneDto>> GetTutteAsync()
        {
            var prenotazioni = await _context.Prenotazioni
                .Include(p => p.User)
                .Include(p => p.Villa)
                .ToListAsync();

            return prenotazioni.Select(ToDto);
        }

        public async Task<PrenotazioneDto?> GetByIdAsync(int id, ClaimsPrincipal user)
        {
            var prenotazione = await _context.Prenotazioni
                .Include(p => p.User)
                .Include(p => p.Villa)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prenotazione == null) return null;

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!user.IsInRole("Admin") && prenotazione.UserId != userId)
            {
                return null;
            }

            return ToDto(prenotazione);
        }

        public async Task<(bool Success, string? Error, PrenotazioneDto? Prenotazione)> CreateAsync(PrenotazioneCreateDto dto, ClaimsPrincipal user)
        {
            var villa = await _context.Ville.FindAsync(dto.VillaId);
            if (villa == null) return (false, "Villa non trovata", null);

            var giorni = (dto.DataFine - dto.DataInizio).Days;
            if (giorni <= 0) return (false, "La data di fine deve essere successiva alla data di inizio", null);

            var prezzoTotale = villa.Prezzo * giorni;

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEntity = await _userManager.FindByIdAsync(userId);
            if (userEntity == null) return (false, "Utente non trovato", null);

            var sovrapposizioni = await _context.Prenotazioni
                .Where(p => p.VillaId == dto.VillaId &&
                           ((p.DataInizio <= dto.DataInizio && p.DataFine >= dto.DataInizio) ||
                            (p.DataInizio <= dto.DataFine && p.DataFine >= dto.DataFine) ||
                            (p.DataInizio >= dto.DataInizio && p.DataFine <= dto.DataFine)))
                .AnyAsync();

            if (sovrapposizioni)
            {
                return (false, "La villa non è disponibile nel periodo selezionato", null);
            }

            var prenotazione = new Prenotazione
            {
                VillaId = dto.VillaId,
                UserId = userId,
                Username = userEntity.UserName,
                Nome = userEntity.Nome,
                Cognome = userEntity.Cognome ?? "Non specificato",
                DataInizio = dto.DataInizio,
                DataFine = dto.DataFine,
                PrezzoTotale = prezzoTotale
            };

            _context.Prenotazioni.Add(prenotazione);
            await _context.SaveChangesAsync();

            await _context.Entry(prenotazione).Reference(p => p.Villa).LoadAsync();
            await _context.Entry(prenotazione).Reference(p => p.User).LoadAsync();

            return (true, null, ToDto(prenotazione));
        }

        public async Task<IEnumerable<PrenotazioneDto>> GetMieAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            var prenotazioni = await _context.Prenotazioni
                .Where(p => p.UserId == userId)
                .Include(p => p.Villa)
                .Include(p => p.User)
                .ToListAsync();

            return prenotazioni.Select(ToDto);
        }

        public async Task<(bool Success, string? Error)> ModificaAsync(int id, PrenotazioneCreateDto dto, ClaimsPrincipal user)
        {
            var prenotazione = await _context.Prenotazioni.FindAsync(id);
            if (prenotazione == null) return (false, "Prenotazione non trovata");

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!user.IsInRole("Admin") && prenotazione.UserId != userId)
            {
                return (false, "Non autorizzato");
            }

            var villa = await _context.Ville.FindAsync(dto.VillaId);
            if (villa == null) return (false, "Villa non trovata");

            var giorni = (dto.DataFine - dto.DataInizio).Days;
            if (giorni <= 0) return (false, "La data di fine deve essere successiva alla data di inizio");

            var sovrapposizioni = await _context.Prenotazioni
                .Where(p => p.Id != id &&
                           p.VillaId == dto.VillaId &&
                           ((p.DataInizio <= dto.DataInizio && p.DataFine >= dto.DataInizio) ||
                            (p.DataInizio <= dto.DataFine && p.DataFine >= dto.DataFine) ||
                            (p.DataInizio >= dto.DataInizio && p.DataFine <= dto.DataFine)))
                .AnyAsync();

            if (sovrapposizioni)
            {
                return (false, "La villa non è disponibile nel periodo selezionato");
            }

            prenotazione.VillaId = dto.VillaId;
            prenotazione.DataInizio = dto.DataInizio;
            prenotazione.DataFine = dto.DataFine;
            prenotazione.PrezzoTotale = villa.Prezzo * giorni;

            _context.Prenotazioni.Update(prenotazione);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> EliminaAsync(int id, ClaimsPrincipal user)
        {
            var prenotazione = await _context.Prenotazioni.FindAsync(id);
            if (prenotazione == null) return (false, "Prenotazione non trovata");

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!user.IsInRole("Admin") && prenotazione.UserId != userId)
            {
                return (false, "Non autorizzato");
            }

            _context.Prenotazioni.Remove(prenotazione);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        private PrenotazioneDto ToDto(Prenotazione p) => new PrenotazioneDto
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
        };
    }
}
