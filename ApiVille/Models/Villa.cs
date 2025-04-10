namespace ApiVille.Models
{
    public class Villa
    {
        public int Id { get; set; }
        public string NomeVilla { get; set; }
        public string Immagine1 { get; set; }
        public string Immagine2 { get; set; }
        public string Immagine3 { get; set; }
        public string Immagine4 { get; set; }
        public decimal Prezzo { get; set; }
        public string Localita { get; set; }
        public string Descrizione { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }


        public ICollection<Prenotazione> Prenotazioni { get; set; }
    }

}
