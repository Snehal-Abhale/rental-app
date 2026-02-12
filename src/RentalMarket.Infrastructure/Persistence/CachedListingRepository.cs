using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using RentalMarket.Application.Listings;
using RentalMarket.Domain.Entities;

namespace RentalMarket.Infrastructure.Persistence;

public class CachedListingRepository: IListingRepository
{
 private readonly ListingRepository _decorated ;
 private readonly IDistributedCache _cache;

    public CachedListingRepository(ListingRepository decorated, IDistributedCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public Task AddAsync(Listing listing, CancellationToken cancellationToken)
    {
        return _decorated.AddAsync(listing, cancellationToken);
    }

    public Task<IEnumerable<Listing>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _decorated.GetAllAsync(cancellationToken);
    }

    public async Task<Listing?> GetByIdAsync(Guid id, CancellationToken CancellationToken)
    {
        
        var cacheKey= $"listing_{id}";
        string? cachedData = await _cache.GetStringAsync(cacheKey, CancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            Console.WriteLine($"[CACHE HIT] Found {id} in Redis!"); // <--- LOG
            return JsonSerializer.Deserialize<Listing>(cachedData);
        }

        Console.WriteLine($"[CACHE MISS] Fetching {id} from SQL..."); // <--- LOG
        var listing = await _decorated.GetByIdAsync(id, CancellationToken);

        if (listing != null)
        {
           await _cache.SetStringAsync(
            cacheKey, 
            JsonSerializer.Serialize(listing), 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
            }, CancellationToken);
        }
        return listing;
    }

    public Task<IEnumerable<Listing>> SearchAsync(decimal? maxPrice, CancellationToken ct)
    {
        return _decorated.SearchAsync(maxPrice, ct);
    }

  
}