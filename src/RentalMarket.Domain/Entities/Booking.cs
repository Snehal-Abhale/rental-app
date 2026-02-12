using System.ComponentModel.DataAnnotations;

namespace RentalMarket.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }

    public Guid ListingId { get; set; }
    // Navigation Property (Optional, but good for EF)
    // public Listing Listing { get; set; } = null!;

    public string GuestId { get; set; } = string.Empty; // User ID

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }

    // THE MOST IMPORTANT PART:
    // This value changes AUTOMATICALLY every time the row is updated.
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}