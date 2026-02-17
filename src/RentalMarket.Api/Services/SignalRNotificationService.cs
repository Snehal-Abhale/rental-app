using Microsoft.AspNetCore.SignalR;
using RentalMarket.Api.Hubs;
using RentalMarket.Application.Common.Interfaces;

namespace RentalMarket.Api.Services;
public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageAsync(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }
}