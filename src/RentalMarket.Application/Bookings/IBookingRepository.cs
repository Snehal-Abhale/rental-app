using RentalMarket.Domain.Entities;

namespace RentalMarket.Application.Bookings;

public interface IBookingRepository
{
    Task<Guid> AddBookingAsync(Booking booking, CancellationToken ct);
}
