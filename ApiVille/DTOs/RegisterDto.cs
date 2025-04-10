using System.ComponentModel.DataAnnotations;

namespace ApiVille.DTOs
{
    public class RegisterDto
    {
        [Required]
        public required string Nome { get; set; }

        [Required]
        public required string Cognome { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "La password deve essere lunga almeno 8 caratteri.")]
        public required string Password { get; set; }


    }

}

