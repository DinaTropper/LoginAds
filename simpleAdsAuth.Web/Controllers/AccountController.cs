using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace simpleAdsAuth.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NewAd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogIn()
        {
            return View();
        }
       
    }
}
