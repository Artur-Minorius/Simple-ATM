using Simple_ATM.ApplicationLayer.Interfaces;
using Simple_ATM.ApplicationLayer.Interfaces.Repository;
using Simple_ATM.DomainLayer.Entities;
using Simple_ATM.DomainLayer.Helpers;
using Simple_ATM.Infrastructure.Repositories;
using Simple_ATM.DomainLayer.Enums;
using Simple_ATM.DomainLayer.Consts;
namespace Simple_ATM.ApplicationLayer.Services
{
    public class OperationService : IOperationService
    {
        private readonly IOperationRepository _operationRepository;
        private readonly IAccountService _accountService;
        public OperationService(IOperationRepository operationRepository, IAccountService accountService)
        {
            _operationRepository = operationRepository;
            _accountService = accountService;
        }
        public Task<OperationResult> DepositAsync(int userId, string amount)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> WithdrawAsync(int userId, string amount)
        {

            var user = await _accountService.GetUserByIdAsync(userId);
            if (user == null)
                return new OperationResult { Success = false, Message = AccountConsts.CardNotFound };
            if (!decimal.TryParse(amount, out decimal amountConverted))
            {
                return new OperationResult { Success = false };
            }
            amountConverted = decimal.Abs(decimal.Round(amountConverted, 2));
            if (amountConverted > user.CardAmount)
            {
                return new OperationResult { Success = false, IsInsufficientFunds = true, Message = AccountConsts.InsufficientFunds };
            }
            var operation = new Operation
            {
                User = user,
                Amount = amountConverted,
                OperationType = OperationType.Withdrawal
            };
            await _operationRepository.AddAsync(operation);
            await _operationRepository.SaveChangesAsync();
            return new OperationResult { Success = true, Amount = -amountConverted };
        }
    }
}
