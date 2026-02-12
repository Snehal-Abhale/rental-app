using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalMarket.Application.Bookings;
using RentalMarket.Application.Listings;
using RentalMarket.Domain.Entities;
using System.Security.Claims;

namespace RentalMarket.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IListingRepository _listingRepo;

    public BookingsController(IBookingRepository bookingRepo, IListingRepository listingRepo)
    {
        _bookingRepo = bookingRepo;
        _listingRepo = listingRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request, CancellationToken ct)
    {
        // 1. Get the Listing to check existance and price
        var listing = await _listingRepo.GetByIdAsync(request.ListingId, ct);
        if (listing == null)
        {
            return NotFound("Listing not found.");
        }

        // 2. Validate Dates
        if (request.StartDate >= request.EndDate)
        {
            return BadRequest("Start date must be before end date.");
        }
        if (request.StartDate < DateTime.UtcNow.Date)
        {
            return BadRequest("Start date cannot be in the past.");
        }

        // 3. Calculate Total Price
        var days = (request.EndDate - request.StartDate).Days;
        var totalPrice = days * listing.PricePerNight;

        // 4. Get User ID from Token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // 5. Create Entity
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ListingId = request.ListingId,
            GuestId = userId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalPrice = totalPrice
        };

        try
        {
            // 6. Save (Repository handles Concurrency checks)
            await _bookingRepo.AddBookingAsync(booking, ct);
            return CreatedAtAction(nameof(Create), new { id = booking.Id }, new { BookingId = booking.Id, TotalPrice = totalPrice });
        }
        catch (InvalidOperationException ex) // Catch the Double Booking error
        {
            return Conflict(new { Error = ex.Message });
        }
    }
}

public record CreateBookingRequest(Guid ListingId, DateTime StartDate, DateTime EndDate);
