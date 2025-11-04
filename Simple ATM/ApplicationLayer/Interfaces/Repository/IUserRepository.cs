using Simple_ATM.DomainLayer.Entities;

namespace Simple_ATM.ApplicationLayer.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByCardNumberAsync(string cardNumber);
        Task AddAsync(User user);
        void Remove(User user);
        void DeleteAll();
        Task SaveChangesAsync();
    }
}
