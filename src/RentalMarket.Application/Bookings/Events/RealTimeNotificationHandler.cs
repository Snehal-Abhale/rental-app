using MediatR;
using RentalMarket.Application.Common.Interfaces;
using RentalMarket.Domain.Events;

namespace RentalMarket.Application.Bookings.Events;

public class RealTimeNotificationHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly INotificationService _notificationService;

    public RealTimeNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"🔔 SIGNALR HANDLER TRIGGERED for {notification.BookingId}");
        await _notificationService.SendMessageAsync($"New Booking Created! ID: {notification.BookingId}");
    }
}
