using Microsoft.EntityFrameworkCore;
using Simple_ATM.ApplicationLayer.Interfaces.Repository;
using Simple_ATM.DomainLayer.Entities;
using Simple_ATM.Infrastructure.Data;

namespace Simple_ATM.Infrastructure.Repositories
{
    public class OperationRepository: IOperationRepository
    {
        private readonly AtmContext _context;
        public OperationRepository(AtmContext context) => _context = context;

        public async Task AddAsync(Operation operation) => await _context.Operations.AddAsync(operation);
        public Task<List<Operation>> GetByUserIdAsync(int userId) =>
            _context.Operations.Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OperationId)
            .ToListAsync();
    }
}
