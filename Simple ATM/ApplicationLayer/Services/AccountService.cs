using Simple_ATM.ApplicationLayer.Interfaces;
using Simple_ATM.ApplicationLayer.Interfaces.Repository;
using Simple_ATM.DomainLayer.Consts;
using Simple_ATM.DomainLayer.Entities;
using Simple_ATM.DomainLayer.Helpers;
namespace Simple_ATM.ApplicationLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly Random _random = new();

        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateCardAsync(string cardNumber)
        {
            cardNumber = cardNumber.Replace("-", "");
            return await _userRepository.GetByCardNumberAsync(cardNumber);
        }

        public async Task<PinVerificationResult> VerifyPinAsync(int userId, string pin)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return new PinVerificationResult { Success = false, Message = AccountConsts.CardNotFound };
            if (user.IsBlocked) return new PinVerificationResult { Success = false, Message = AccountConsts.CardIsBlocked };

            if (user.CardPin == pin)
            {
                user.FailedAttempts = 0;
                await _userRepository.SaveChangesAsync();
                return new PinVerificationResult { Success = true };
            }

            user.FailedAttempts++;
            if (user.FailedAttempts >= 4)
            {
                user.IsBlocked = true;
                await _userRepository.SaveChangesAsync();
                return new PinVerificationResult
                {
                    Success = false,
                    Message = AccountConsts.CardNowBlocked,
                    CardBlocked = true
                };
            }

            await _userRepository.SaveChangesAsync();
            return new PinVerificationResult
            {
                Success = false,
                Message = AccountConsts.CardWillBeBlockedAfter(4 - user.FailedAttempts)
            };
        }
        public Task<User?> GetUserByIdAsync(int userId) => _userRepository.GetByIdAsync(userId);
        public Task<List<User>> GetAllUsersAsync()
            => _userRepository.GetAllUsersAsync();
        public async Task<User> GenerateUserAsync()
        {
            var user = new User
            {
                CardNumber = string.Concat(Enumerable.Range(0, 16).Select(_ => _random.Next(0, 10))),
                CardPin = string.Concat(Enumerable.Range(0, 4).Select(_ => _random.Next(0, 10))),
            };
            var operation = new Operation
            {
                User = user,
                OperationType = DomainLayer.Enums.OperationType.Deposit,
                Amount = (decimal)_random.Next(10, 10000)
            };
            user.Operations.Add(operation);
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;
            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }
        public async Task DeleteAllUsersAsync()
        {
            _userRepository.DeleteAll();
            await _userRepository.SaveChangesAsync();
        }


        public async Task<bool> UnlockUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;
            user.IsBlocked = false;
            await _userRepository.SaveChangesAsync();
            return true;
        }


    }
}
