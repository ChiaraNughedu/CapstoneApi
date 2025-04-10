using System.ComponentModel.DataAnnotations;

namespace ApiVille.DTOs
{
    public class VillaDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string NomeVilla { get; set; }

        [Url]
        public string Immagine1 { get; set; }

        [Url]
        public string Immagine2 { get; set; }

        [Url]
        public string Immagine3 { get; set; }

        [Url]
        public string Immagine4 { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal Prezzo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Localita { get; set; }


        public int CategoriaId { get; set; }

        [Required]
        public string NomeCategoria { get; set; }

        [Required]
        [MaxLength(8000)]
        public string Descrizione { get; set; }


    }
}
