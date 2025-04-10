namespace ApiVille.DTOs
{
    public class PrenotazioneDto
    {
        public int Id { get; set; }
        public int VillaId { get; set; }
        public string NomeVilla { get; set; }
        public string UserId { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string UserEmail { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public decimal PrezzoTotale { get; set; }
    }
}