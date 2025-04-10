namespace ApiVille.DTOs
{
    public class PrenotazioneCreateDto
    {
        public int VillaId { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
    }
}