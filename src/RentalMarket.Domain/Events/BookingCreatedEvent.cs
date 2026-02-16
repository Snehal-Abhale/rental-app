using MediatR;

namespace RentalMarket.Domain.Events;

public record BookingCreatedEvent(Guid BookingId, string GuestEmail) : INotification;