using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simple_ATM.Models;
using Simple_ATM.Models.ViewModels;
using System.Collections;
using Simple_ATM.DomainLayer.Entities;
using Simple_ATM.Infrastructure.Data;
using Simple_ATM.ApplicationLayer.Interfaces;
using Simple_ATM.DomainLayer.Consts;
namespace Simple_ATM.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _accountService.GetAllUsersAsync();
            return View(users);
        }
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //Reset on enter
            HttpContext.Session.Remove("PendingUserId");

            if (!ModelState.IsValid)
                return View(model);
            var cardNumber = model.CardNumber.Replace("-", "");
            var user = await _accountService.AuthenticateCardAsync(model.CardNumber);
            if (user == null)
            {
                ViewBag.Error = AccountConsts.CardNotFound;
                return View(model);
            }

            if (user.IsBlocked)
                return CardBlockedError();

            HttpContext.Session.SetInt32("PendingUserId", user.UserId);
            return RedirectToAction("EnterPin");
        }
        [HttpGet]
        public IActionResult EnterPin()
        {
            var pendingUserId = HttpContext.Session.GetInt32("PendingUserId");
            if (!pendingUserId.HasValue)
                return RedirectToAction("Login");
            return View(new EnterPinViewModel { Pin = "", UserId = pendingUserId.Value });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterPin(EnterPinViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var pendingUserId = HttpContext.Session.GetInt32("PendingUserId");
            // Session expired
            if (!pendingUserId.HasValue)
                return RedirectToAction("Login");
            // Manual change in field's value
            if (model.UserId != pendingUserId.Value)
            {
                HttpContext.Session.Remove("PendingUserId");
                return RedirectToAction("Login");
            }

            var result = await _accountService.VerifyPinAsync(model.UserId, model.Pin);

            if (!result.Success)
            {
                if (result.CardBlocked)
                    return CardBlockedError();
                ViewBag.Error = result.Message;
                return View(model);
            }
            HttpContext.Session.Remove("PendingUserId");
            HttpContext.Session.SetInt32("UserId", model.UserId);
            return RedirectToAction("Dashboard");
        }

        public IActionResult Error(ErrorViewModel model)
        {
            if (!ModelState.IsValid)
                return View();
            return View(model);
        }
        public async Task<IActionResult> Dashboard()
        {
            SetRevalidationHeaders();


            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
                return RedirectToAction("Login");

            var user = await _accountService.GetUserByIdAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Login");
            if (user.IsBlocked)
                return CardBlockedError();
            return View();
        }

        public IActionResult Exit()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Balance()
        {
            SetRevalidationHeaders();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var user = await _accountService.GetUserByIdAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Login");
            if (user.IsBlocked)
                return CardBlockedError();
            var sortedOperations = user.Operations
                .OrderByDescending(o => o.OperationTime)
                .ToList();
            var operationsModel = new UserOperationsViewModel
            {
                UserId = userId.Value,
                CardNumber = user.CardNumber,
                CurrentAmount = user.CardAmount,
                Operations = sortedOperations
            };
            return View(operationsModel);
        }



        [HttpGet]
        public async Task<IActionResult> Generate()
        {
            await _accountService.GenerateUserAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DropUsers()
        {
            await _accountService.DeleteAllUsersAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Error", new ErrorViewModel { RequestId = "Failed to delete user", BackAction = "Index", BackController = "Account" });
            var userId = model.Id;
            await _accountService.DeleteUserAsync(userId);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Unlock(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Error", new ErrorViewModel { RequestId = "Failed to unlock user", BackAction = "Index", BackController = "Account" });

            var userId = model.Id;
            await _accountService.UnlockUserAsync(userId);

            return RedirectToAction("Index");
        }
    }
}
