using Microsoft.AspNetCore.Mvc;
using Simple_ATM.DomainLayer.Consts;
using Simple_ATM.Models;

namespace Simple_ATM.Controllers
{
    public abstract class BaseController : Controller
    {
        protected void SetRevalidationHeaders()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }

        protected IActionResult SomethingWentWrong(string? message = null)
        {
            return RedirectToAction("Error", new ErrorViewModel
            {
                RequestId = message ?? AccountConsts.SomethingWentWrong,
                BackAction = "Dashboard",
                BackController = "Account"
            });
        }
    }
}
