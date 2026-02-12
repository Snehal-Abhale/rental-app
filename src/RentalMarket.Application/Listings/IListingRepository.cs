using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RentalMarket.Domain.Entities;

namespace RentalMarket.Application.Listings;

public interface IListingRepository
{
    Task<IEnumerable<Listing>> GetAllAsync(CancellationToken cancellationToken);
    Task<Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Listing listing, CancellationToken cancellationToken);
    Task<IEnumerable<Listing>> SearchAsync(decimal? maxPrice, CancellationToken ct);
}