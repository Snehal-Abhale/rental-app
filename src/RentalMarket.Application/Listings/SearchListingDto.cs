namespace RentalMarket.Application.Listings;

public class SearchListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string Location { get; set; } = string.Empty;
}