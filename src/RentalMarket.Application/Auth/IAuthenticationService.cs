using System.Threading.Tasks;

namespace RentalMarket.Application.Auth;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(string email, string password, string username);
    Task<string> LoginAsync(string email, string password);
}