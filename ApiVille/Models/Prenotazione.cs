using System;
using System.ComponentModel.DataAnnotations;


namespace ApiVille.Models
{
    public class Prenotazione
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string Nome { get; set; }
        public string? Cognome { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int VillaId { get; set; }
        public Villa Villa { get; set; }

        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }

        public decimal PrezzoTotale { get; set; }
    }
}

