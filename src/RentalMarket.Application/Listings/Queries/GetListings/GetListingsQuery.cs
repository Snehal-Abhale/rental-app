using Dapper; // <--- Dapper!
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RentalMarket.Application.Listings.Queries.GetListings;

// 1. The Request (Empty because we want ALL listings)
public record GetListingsQuery : IRequest<IEnumerable<ListingDto>>;

// 2. The DTO (Shape of valid data)
public class ListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
}

// 3. The Handler (Using Dapper directly!)
public class GetListingsQueryHandler : IRequestHandler<GetListingsQuery, IEnumerable<ListingDto>>
{
    private readonly string _connectionString;

    public GetListingsQueryHandler(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<IEnumerable<ListingDto>> Handle(GetListingsQuery request, CancellationToken cancellationToken)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        var sql = "SELECT Id, Title, PricePerNight, Location, Bedrooms FROM Listings";
        
        return await connection.QueryAsync<ListingDto>(sql);
    }
}