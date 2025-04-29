using System.Reflection.Emit;
using ApiVille.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiVille.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Villa> Ville { get; set; }
        public DbSet<Prenotazione> Prenotazioni { get; set; }
        public DbSet<Categoria> Categorie { get; set; }
        public DbSet<Articolo> Articoli { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Villa>()
                .Property(v => v.Prezzo)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Villa>()
                .HasMany(v => v.Prenotazioni)
                .WithOne(p => p.Villa)
                .HasForeignKey(p => p.VillaId);

            builder.Entity<Villa>()
                .HasOne(v => v.Categoria)
                .WithMany(c => c.Ville)
                .HasForeignKey(v => v.CategoriaId);

            builder.Entity<AppUser>()
                .HasMany(u => u.Prenotazioni)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            builder.Entity<Prenotazione>()
                .Property(p => p.PrezzoTotale)
                .HasPrecision(18, 2);

            builder.Entity<Prenotazione>()
                .HasOne(p => p.Villa)
                .WithMany(v => v.Prenotazioni)
                .HasForeignKey(p => p.VillaId);

            builder.Entity<Prenotazione>()
                .HasOne(p => p.User)
                .WithMany(u => u.Prenotazioni)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
