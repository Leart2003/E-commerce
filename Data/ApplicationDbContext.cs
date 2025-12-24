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
    public DbSet<OrderProduct> OrderProducts { get; set; }
   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Produkt>()
              .HasMany(p => p.Koments)
              .WithOne(k => k.Produkt)
              .HasForeignKey(k => k.ProduktId)
              .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<Koment>()
        //    .HasOne(k => k.User)
        //    .WithMany()
        //    .HasForeignKey(k => k.IdentityUserId)
        //    .HasForeignKey(k => k.ProduktId)
        //    .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<OrderProduct>()
        .HasOne(o => o.User)
        .WithMany()
        .HasForeignKey(o => o.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(o => o.Produkt)
            .WithMany()
            .HasForeignKey(o => o.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


    }
    
}
