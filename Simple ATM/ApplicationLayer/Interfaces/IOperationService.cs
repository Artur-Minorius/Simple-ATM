using Simple_ATM.DomainLayer.Entities;
using Simple_ATM.DomainLayer.Helpers;

namespace Simple_ATM.ApplicationLayer.Interfaces
{
    public interface IOperationService
    {
        Task<OperationResult> WithdrawAsync(int userId, string amount);
        Task<OperationResult> DepositAsync(int userId, string amount);
    }
}
