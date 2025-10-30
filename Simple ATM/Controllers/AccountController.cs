using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simple_ATM.Models;
using Simple_ATM.Models.ATM_Data;
using Simple_ATM.Models.ViewModels;
using Simple_ATM.Common.Consts;
using Simple_ATM.Common.Enums;
using System.Collections;
namespace Simple_ATM.Controllers
{
    public class AccountController : Controller
    {
        private readonly AtmContext _context;
        static Random random = new Random();
        public AccountController(AtmContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();

            if (users.Count == 0)
            {
                string cardNumber;
                string cardPin;
                decimal cardAmount;
                for (int i = 0; i < 4; i++)
                {
                    cardNumber = Enumerable.Range(0, 16).Aggregate("", (a, c) => a + random.Next(0, 10).ToString());
                    cardPin = Enumerable.Range(0, 4).Aggregate("", (a, c) => a + random.Next(0, 10).ToString());
                    cardAmount = random.Next(10, 10000) + (decimal)Math.Round(random.NextDouble(), 2);
                    await _context.AddAsync(new User { CardNumber = cardNumber, CardAmount = cardAmount, CardPin = cardPin });
                }
                await _context.SaveChangesAsync();
            }


            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Login(LoginViewModel? model)
        {
            //Reset on enter
            HttpContext.Session.Remove("PendingUserId");

            if (!ModelState.IsValid || model == null)
            {
                return View();
            }
            var cardNumber = model.CardNumber.Replace("-", "");
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.CardNumber == model.CardNumber);

            if (user == null)
            {
                ViewBag.Error = AccountConsts.CardNotFound;
                return View(model);
            }

            if (user.IsBlocked)
                return View("Error", new ErrorViewModel { RequestId = AccountConsts.CardIsBlocked });

            HttpContext.Session.SetInt32("PendingUserId", user.UserId);
            return RedirectToAction("EnterPin");
        }
        [HttpGet]
        public async Task<IActionResult> EnterPin()
        {
            //If session contains PendingUserId, create a model with it
            var userId = HttpContext.Session.GetInt32("PendingUserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var (user, res) = await CheckUser(userId.Value, "Login", "Error", AccountConsts.CardIsBlocked);
            if (res != null)
                return res;

            return View(new EnterPinViewModel { UserId = user.UserId, Pin = "" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterPin(EnterPinViewModel model)
        {
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

            if (!ModelState.IsValid)
                return RedirectToAction("Login");

            var userId = model.UserId;
            var pin = model.Pin;

            var (user, res) = await CheckUser(userId, "Login", "Error", AccountConsts.CardIsBlocked);
            if (res != null)
                return res;

            if (string.IsNullOrEmpty(pin))
            {
                return View(new EnterPinViewModel { UserId = user.UserId, Pin = "" });
            }




            if (user.CardPin == pin)
            {
                user.FailedAttempts = 0;
                await _context.SaveChangesAsync();

                HttpContext.Session.SetInt32("UserId", user.UserId);
                return RedirectToAction("Dashboard");
            }

            user.FailedAttempts++;
            await _context.SaveChangesAsync();

            if (user.FailedAttempts >= 4)
            {
                user.IsBlocked = true;
                await _context.SaveChangesAsync();

                var cardBlockedError = new ErrorViewModel { RequestId = AccountConsts.CardNowBlocked };
                return RedirectToAction("Error", cardBlockedError);
            }
            else
            {
                ViewBag.Error = AccountConsts.CardWillBeBlockedAfter(4 - user.FailedAttempts);
            }

            return View(new EnterPinViewModel { UserId = user.UserId, Pin = "" });
        }

        public IActionResult Error(ErrorViewModel model)
        {
            if (!ModelState.IsValid)
                return View();
            return View(model);
        }
        public async Task<IActionResult> Dashboard()
        {
            //Revalidate every time page is loaded
            SetRevalidationHeaders();


            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
                return RedirectToAction("Login");

            var (user, res) = await CheckUser(userId.Value, "Login", "Error", AccountConsts.CardIsBlocked);
            if (res != null)
                return res;

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

            var (user, res) = await CheckUser(userId.Value, "Login", "Error", AccountConsts.CardIsBlocked);
            if (res != null)
                return res;

            var operations = await _context.Operations
                .Where(o => o.UserId == userId)
                .ToListAsync();

            var operationsModel = new UserOperationsViewModel
            {
                UserId = userId.Value,
                CardNumber = user.CardNumber,
                CurrentAmount = user.CardAmount,
                Operations = operations
            };
            return View(operationsModel);
        }
        public async Task<IActionResult> Withdraw()
        {
            SetRevalidationHeaders();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var (user, res) = await CheckUser(userId.Value, "Login", "Error", AccountConsts.CardIsBlocked);
            if (res != null)
                return res;

            return View();
        }

        public async Task<IActionResult> SubmitWithdrawal(OperationViewModel model)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var (user, res) = await CheckUser(userId.Value, "Login", "Error", AccountConsts.CardIsBlocked);
            if (res != null)
                return res;

            if (!ModelState.IsValid)
                return RedirectToAction("Error", new ErrorViewModel { RequestId = AccountConsts.SomethingWentWrong, BackAction = "Dashboard", BackController = "Account" });
            if (user.CardAmount < model.Amount)
                return RedirectToAction("Error", new ErrorViewModel { RequestId = AccountConsts.InsufficientFunds, BackAction = "Dashboard", BackController = "Account" });

            

            _context.Operations.Add(new Operation { Amount = model.Amount, UserId = userId.Value, User = user });
            user.CardAmount += model.OperationType == OperationType.Withdrawal ? -model.Amount : model.Amount;
            await _context.SaveChangesAsync();

            var operationResult = new OperationResultViewModel
            {
                Amount = model.Amount,
                CardNumber = user.CardNumber,
                RemainingAmount = user.CardAmount,
                OperationType = OperationType.Withdrawal

            };

            return RedirectToAction("OperationResult", operationResult);
        }
        public async Task<IActionResult> OperationResult(OperationResultViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Dashboard");
            return View(model);
        }
        async Task<(User?, IActionResult?)> CheckUser(int userId, string notFoundAction, string blockedAction, string blockedText)
        {
            IActionResult? result = null;
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                result = RedirectToAction("Login");
                return (null, result);
            }

            if (user.IsBlocked)
            {
                var cardBlockedError = new ErrorViewModel
                {
                    RequestId = AccountConsts.CardIsBlocked,
                    BackAction = "Login",
                    BackController = "Account"
                };
                result = RedirectToAction("Error", cardBlockedError);
                return (null, result);
            }
            return (user, null);
        }
        private void SetRevalidationHeaders()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }
    }
}
