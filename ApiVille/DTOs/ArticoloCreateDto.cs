using System.ComponentModel.DataAnnotations;

namespace ApiVille.DTOs
{
    public class ArticoloCreateDto
    {
        [Required]
        public string Luogo { get; set; }
        [Required]
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(800)]
        public string Descrizione1 { get; set; }

        [Required]
        [MaxLength(8000)]
        public string Descrizione2 { get; set; }
    }
}
