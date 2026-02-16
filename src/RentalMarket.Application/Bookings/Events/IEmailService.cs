namespace RentalMarket.Application.Bookings.Events;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}