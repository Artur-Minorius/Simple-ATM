using Simple_ATM.DomainLayer.Entities;

namespace Simple_ATM.ApplicationLayer.Interfaces.Repository
{
    public interface IOperationRepository
    {
        Task AddAsync(Operation operation);
        Task<List<Operation>> GetByUserIdAsync(int userId);
    }
}
