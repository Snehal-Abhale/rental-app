using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalMarket.Application.Bookings;
using RentalMarket.Application.Listings;
using RentalMarket.Domain.Entities;
using System.Security.Claims;
using MediatR;
using RentalMarket.Domain.Events;
using RentalMarket.Application.Common.Interfaces;

namespace RentalMarket.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IListingRepository _listingRepo;
    private readonly IMediator _mediator; 
    private readonly IPaymentGateway _paymentGateway;

    public BookingsController(IBookingRepository bookingRepo, IListingRepository listingRepo, IMediator mediator, IPaymentGateway paymentGateway)
    {
        _bookingRepo = bookingRepo;
        _listingRepo = listingRepo;
        _mediator = mediator;
        _paymentGateway = paymentGateway;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        // Simple placeholder for now. 
        // In a real app we'd fetch this from Repo, but for now just return OK to fix compilation.
        return Ok(new { Id = id, Status = "Confirmed" });
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

        // 5. Process Payment (Mock)
        var paymentSuccess = await _paymentGateway.ProcessPaymentAsync(totalPrice, "USD");
        if (!paymentSuccess)
        {
            return BadRequest("Payment failed.");
        }

        // 6. Create Entity
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
            await _mediator.Publish(new BookingCreatedEvent(booking.Id, "snehalabhale93@gmail.com"), ct); // TODO: Get real email from User
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);

        }
        catch (InvalidOperationException ex) // Catch the Double Booking error
        {
            return Conflict(new { Error = ex.Message });
        }
    }
}

public record CreateBookingRequest(Guid ListingId, DateTime StartDate, DateTime EndDate);
