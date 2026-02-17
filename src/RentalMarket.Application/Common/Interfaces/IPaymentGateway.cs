namespace RentalMarket.Application.Common.Interfaces
{
    public interface IPaymentGateway
    {
        Task<bool> ProcessPaymentAsync(decimal amount, string currency = "USD");
    }
}