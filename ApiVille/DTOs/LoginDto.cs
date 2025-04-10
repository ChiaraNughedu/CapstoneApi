using System.ComponentModel.DataAnnotations;

namespace ApiVille.DTOs
{
    public class LoginDto
    {

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }

}
