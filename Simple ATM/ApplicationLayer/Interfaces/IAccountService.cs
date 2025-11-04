using Simple_ATM.DomainLayer.Entities;
using Simple_ATM.DomainLayer.Helpers;

namespace Simple_ATM.ApplicationLayer.Interfaces
{
    public interface IAccountService
    {
        Task<User?> AuthenticateCardAsync(string cardNumber);
        Task<PinVerificationResult> VerifyPinAsync(int userId, string pin);
        Task<User?> GetUserByIdAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GenerateUserAsync();
        Task<bool> DeleteUserAsync(int userId);
        Task DeleteAllUsersAsync();
        Task<bool> UnlockUserAsync(int userId);
    }
}
