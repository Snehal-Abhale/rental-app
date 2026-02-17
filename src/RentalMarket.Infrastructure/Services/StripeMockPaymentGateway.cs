using RentalMarket.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace RentalMarket.Infrastructure.Services
{
    public class StripeMockPaymentGateway : IPaymentGateway
    {
        private readonly ILogger<StripeMockPaymentGateway> _logger;

        public StripeMockPaymentGateway(ILogger<StripeMockPaymentGateway> logger)
        {
            _logger = logger;
        }

        public async Task<bool> ProcessPaymentAsync(decimal amount, string currency = "USD")
        {
            _logger.LogInformation("💳 STARTING PAYMENT: Processing {Amount} {Currency}...", amount, currency);
            
            await Task.Delay(3000); // 3 Seconds Delay

            _logger.LogInformation("✅ PAYMENT SUCCESSFUL!");
            return true; 
        }
    }
}