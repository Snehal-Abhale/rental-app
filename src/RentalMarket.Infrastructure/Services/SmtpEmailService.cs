using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using RentalMarket.Application.Bookings.Events;

namespace RentalMarket.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var emailSettings = _config.GetSection("EmailSettings");
        var senderEmail = emailSettings["SenderEmail"];
        var password = emailSettings["Password"];

        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(senderEmail, password)
        };

        var mailMessage = new MailMessage(from: senderEmail!, to: to, subject: subject, body: body);
        
        await client.SendMailAsync(mailMessage);
    }
}