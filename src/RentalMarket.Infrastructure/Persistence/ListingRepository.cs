using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalMarket.Application.Listings;
using RentalMarket.Domain.Entities;

namespace RentalMarket.Infrastructure.Persistence;

public class ListingRepository : IListingRepository
{
    private readonly ApplicationDbContext _context;

    public ListingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Listing>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Listings.ToListAsync(cancellationToken);
    }

    public async Task<Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Listings.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task AddAsync(Listing listing, CancellationToken cancellationToken)
    {
        await _context.Listings.AddAsync(listing, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<IEnumerable<Listing>> SearchAsync(decimal? maxPrice, CancellationToken ct)
    {
        IQueryable<Listing> query = _context.Listings;

        if (maxPrice.HasValue)
        {
            query = query.Where(l => l.PricePerNight <= maxPrice.Value);
        }

        return await query.ToListAsync(ct);
    }
}