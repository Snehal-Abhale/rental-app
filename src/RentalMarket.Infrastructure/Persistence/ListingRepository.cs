using System;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RentalMarket.Application.Listings;
using RentalMarket.Domain.Entities;

namespace RentalMarket.Infrastructure.Persistence;

public class ListingRepository : IListingRepository
{
    private readonly ApplicationDbContext _context;
    private readonly string _connectionString;

    public ListingRepository(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
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
     public async Task<IEnumerable<SearchListingDto>> SearchAsync(decimal? maxPrice, CancellationToken ct)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = "SELECT Id, Title, PricePerNight, Location FROM Listings WHERE 1=1";
        
        if (maxPrice.HasValue)
        {
            sql += " AND PricePerNight <= @MaxPrice";
        }
        // Dapper Magic! 🪄
        return await connection.QueryAsync<SearchListingDto>(sql, new { MaxPrice = maxPrice });
    }
}