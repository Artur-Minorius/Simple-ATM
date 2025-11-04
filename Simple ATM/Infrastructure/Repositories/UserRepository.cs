using Microsoft.EntityFrameworkCore;
using Simple_ATM.ApplicationLayer.Interfaces.Repository;
using Simple_ATM.DomainLayer.Entities;
using Simple_ATM.Infrastructure.Data;

namespace Simple_ATM.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AtmContext _context;
        public UserRepository(AtmContext context) => _context = context;
        public Task<List<User>> GetAllUsersAsync() =>
            _context.Users.Include(u => u.Operations).ToListAsync();
        public Task<User?> GetByIdAsync(int id) => _context.Users.Include(u => u.Operations).FirstOrDefaultAsync(u => u.UserId == id);
        public Task<User?> GetByCardNumberAsync(string cardNumber) =>
            _context.Users.FirstOrDefaultAsync(u => u.CardNumber == cardNumber);
        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);
        public void Remove(User user) => _context.Users.Remove(user);
        public void DeleteAll() => _context.Users.RemoveRange(_context.Users);
        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
