using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Punim_Diplome.Models;


public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Koment> Comments { get; set; }
    public DbSet<Produkt> Produktet { get; set; }
   



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Produkt>()
              .HasMany(p => p.Koments)
              .WithOne(k => k.Produkt)
              .HasForeignKey(k => k.ProduktId)
              .OnDelete(DeleteBehavior.Cascade);





        








    }
    
}
