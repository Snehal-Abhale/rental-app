namespace RentalMarket.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendMessageAsync(string message);
}
