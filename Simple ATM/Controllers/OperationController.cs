using Microsoft.AspNetCore.Mvc;
using Simple_ATM.ApplicationLayer.Interfaces;
using Simple_ATM.DomainLayer.Consts;
using Simple_ATM.Models;
using Simple_ATM.Models.ViewModels;

namespace Simple_ATM.Controllers
{
    public class OperationController : BaseController
    {
        private readonly IOperationService _operationService;
        private readonly IAccountService _accountService;
        public OperationController(IOperationService operationService, IAccountService accountService)
        {
            _operationService = operationService;
            _accountService = accountService;
        }
        public IActionResult Withdraw()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SubmitWithdrawal(OperationViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            //Session expired or not logged in
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var user = await _accountService.GetUserByIdAsync(userId.Value);

            if (user == null)
                return RedirectToAction("Login", "Account");
            if (user.IsBlocked)
                return CardBlockedError();

            var result = await _operationService.WithdrawAsync(userId.Value, model.Amount);

            if (!result.Success)
            {
                if (result.IsInsufficientFunds)
                {
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = AccountConsts.InsufficientFunds,
                        BackAction = "Withdraw",
                        BackController = "Operation"
                    };
                    return RedirectToAction("Error", "Account", errorModel);
                }
                return SomethingWentWrong();
            }

            var operationResultViewModel = new OperationResultViewModel
            {
                CardNumber = user.CardNumber,
                Amount = result.Amount,
                RemainingAmount = user.CardAmount,
                OperationType = DomainLayer.Enums.OperationType.Withdrawal
            };
            return RedirectToAction("OperationResult", operationResultViewModel);
        }
        public IActionResult OperationResult(OperationResultViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Login", "Account");

            return View(model);
        }
    }
}
