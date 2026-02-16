using MediatR;
using RentalMarket.Domain.Entities;

namespace RentalMarket.Application.Listings.Commands.CreateListing;

// 1. The Request (Data only)
public record CreateListingCommand(string Title, string Description, decimal PricePerNight, string Location, int Bedrooms) : IRequest<Guid>;

// 2. The Handler (Logic only)
public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, Guid>
{
    private readonly IListingRepository _repository;

    public CreateListingCommandHandler(IListingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateListingCommand request, CancellationToken cancellationToken)
    {
        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            PricePerNight = request.PricePerNight,
            Location = request.Location,
            Bedrooms = request.Bedrooms
        };

        await _repository.AddAsync(listing, cancellationToken);

        return listing.Id;
    }
}