using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RentalMarket.Application.Bookings;
using RentalMarket.Domain.Entities;
using System.Data;

namespace RentalMarket.Infrastructure.Persistence;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddBookingAsync(Booking booking, CancellationToken ct)
    {
        // Ensure ID is set
        if (booking.Id == Guid.Empty) booking.Id = Guid.NewGuid();

        var idParam = new SqlParameter("@Id", booking.Id);
        var listingIdParam = new SqlParameter("@ListingId", booking.ListingId);
        var guestIdParam = new SqlParameter("@GuestId", booking.GuestId);
        var startDateParam = new SqlParameter("@StartDate", booking.StartDate);
        var endDateParam = new SqlParameter("@EndDate", booking.EndDate);
        var totalPriceParam = new SqlParameter("@TotalPrice", booking.TotalPrice);

        var rowVersionParam = new SqlParameter("@RowVersion", SqlDbType.Timestamp)
        {
            Direction = ParameterDirection.Output
        };

        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Sp_CreateBooking @Id, @ListingId, @GuestId, @StartDate, @EndDate, @TotalPrice, @RowVersion OUTPUT",
                new[] { idParam, listingIdParam, guestIdParam, startDateParam, endDateParam, totalPriceParam, rowVersionParam },
                ct);

            if (rowVersionParam.Value != DBNull.Value)
            {
                booking.RowVersion = (byte[])rowVersionParam.Value;
            }

            return booking.Id;
        }
        catch (SqlException ex)
        {
            if (ex.Number == 50001)
            {
                throw new InvalidOperationException("Double Booking Detected: Dates already taken.");
            }
            throw;
        }
    }
}
