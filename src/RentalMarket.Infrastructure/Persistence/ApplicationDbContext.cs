using Microsoft.EntityFrameworkCore;
using RentalMarket.Domain.Entities;

namespace RentalMarket.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Listing> Listings { get; set; }
}