using ApiVille.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ApiVille.Data
{
    public class SeedData
    {
        public static async Task SeedCategorieAsync(ApplicationDbContext context)
        {
            if (!context.Categorie.Any())
            {
                var categorie = new List<Categoria>
        {
            new Categoria { NomeCategoria = "Ville", Descrizione = "Ville di lusso" },
            new Categoria { NomeCategoria = "Appartamenti", Descrizione = "Appartamenti" }
        };

                await context.Categorie.AddRangeAsync(categorie);
                await context.SaveChangesAsync();
            }
        }

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roles = new[] { "Admin", "User" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var newRole = new AppRole(roleName)
                    {
                        Name = roleName,
                        Descrizione = roleName == "Admin"
                            ? "Amministratore del sistema"
                            : "Utente generico"
                    };

                    await roleManager.CreateAsync(newRole);
                }
            }

            // Creazione del mio admin di default
            string adminEmail = "chiaraadmin@luxuryvillas.com";
            string adminUserName = "ChiaraAdmin";
            string adminPassword = "ChiaraAdmin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}

