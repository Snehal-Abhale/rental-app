using System;
namespace RentalMarket.Domain.Entities;
public class Listing
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
}