using MediatR;
using Microsoft.Extensions.Logging;
using RentalMarket.Domain.Events;

namespace RentalMarket.Application.Bookings.Events;

public class EmailNotificationHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly ILogger<EmailNotificationHandler> _logger;
    private readonly IEmailService _emailService;

    public EmailNotificationHandler(ILogger<EmailNotificationHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📧 SENDING EMAIL to {Email}: Confirmation for Booking {Id}", 
            notification.GuestEmail, notification.BookingId);

        var subject = $"Booking Confirmation: {notification.BookingId}";
        var body = $"Thank you for booking with us! Your booking ID is {notification.BookingId}.";

        await _emailService.SendEmailAsync(notification.GuestEmail, subject, body);
    }
}