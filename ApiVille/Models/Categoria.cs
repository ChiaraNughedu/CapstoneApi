using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiVille.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NomeCategoria { get; set; }

        public string? Descrizione { get; set; }
        public virtual ICollection<Villa> Ville { get; set; }
    }
}