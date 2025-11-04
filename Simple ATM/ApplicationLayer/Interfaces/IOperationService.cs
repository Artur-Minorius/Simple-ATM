using Simple_ATM.DomainLayer.Entities;

namespace Simple_ATM.ApplicationLayer.Interfaces
{
    public interface IOperationService
    {
        Task<(bool Success, string Message)> WithdrawAsync(int userId, decimal amount);
        Task<(bool Success, string Message)> DepositAsync(int userId, decimal amount);
    }
}
