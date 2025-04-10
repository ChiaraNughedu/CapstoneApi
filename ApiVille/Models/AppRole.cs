using Microsoft.AspNetCore.Identity;

namespace ApiVille.Models
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }

        public AppRole(string roleName) : base(roleName) { }
        public string Descrizione { get; set; }
    }
}

