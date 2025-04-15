using Microsoft.AspNetCore.Identity;

namespace ApiVille.Models
{
    public class AppUser : IdentityUser
    {
        public string? Nome { get; set; }
        public string? Cognome { get; set; }

        public ICollection<Prenotazione> Prenotazioni { get; set; }
    }
}
