using System.ComponentModel.DataAnnotations;

namespace ApiVille.DTOs
{
    public class CategoriaCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        public string? Descrizione { get; set; }
    }
}