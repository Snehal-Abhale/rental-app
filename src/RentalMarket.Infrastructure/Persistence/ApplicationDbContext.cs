using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentalMarket.Domain.Entities;

namespace RentalMarket.Infrastructure.Persistence;


public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Listing> Listings { get; set; }
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Booking>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e=>e.GuestId)
            .IsRequired()
            .HasMaxLength(450);
            b.Property(e => e.StartDate).IsRequired();
            b.Property(e => e.EndDate).IsRequired();
            b.HasIndex(e => new { e.ListingId, e.StartDate, e.EndDate }).IsUnique();

        });

        
    }
}