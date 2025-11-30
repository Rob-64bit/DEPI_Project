using System.Threading.Tasks;
using Bookify.Domain.Entities;

namespace Bookify.wep.Services.Auth
{
    public interface IAuthService
    {
        Task<Usere?> ValidateUserAsync(string emailOrUsername, string password);
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashed, string providedPassword);

        Task<Usere> CreateUserAsync(string userName, string email, string password, string? role = "client");
        Task<bool> IsEmailOrUserNameTakenAsync(string userName, string email);
    }
}
