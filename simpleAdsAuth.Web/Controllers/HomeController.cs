using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using simpleAdsAuth.Web.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace simpleAdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=LoginAds; Integrated Security=True;";
        public IActionResult Index(int id)
        {
            LoginAdsRepository repo = new(_connectionString);
            AdsViewModel vm = new();
            vm.Ads = repo.GetAds();
            vm.User = repo.GetByEmail(User.Identity.Name);

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult NewAd(Ad ad)
        {
            LoginAdsRepository repo = new(_connectionString);
            var user = repo.GetByEmail(User.Identity.Name);

            ad.UserId = user.Id;
            repo.AddAd(ad);
            
            return Redirect("/home/showaccount");
        }

        public IActionResult NewAd()
        {
            LoginAdsRepository repo = new(_connectionString);
            var user = repo.GetByEmail(User.Identity.Name);
            if (user == null)
            {
                TempData["Message"] = "Please log in!!";
                return Redirect("/home/login");
            }
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {
            var repo = new LoginAdsRepository(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["Message"] = "Invalid Login!";
                return RedirectToAction("Login");
            }


            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "Cookies", ClaimTypes.Email, "roles"))
                ).Wait();

            return Redirect("/home/Index");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User u)
        {
            LoginAdsRepository repo = new(_connectionString);
            repo.AddUser(u);
            return Redirect("/home/Login");
        }
        [Authorize]
        public IActionResult ShowAccount()
        {
            LoginAdsRepository repo = new(_connectionString);
            var user = repo.GetByEmail(User.Identity.Name);
            AdsViewModel vm = new()
            {
                Ads = repo.GetAdsForId(user.Id)
            };
            return View(vm);
        }
        public IActionResult Delete(int id)
        {
            LoginAdsRepository repo = new(_connectionString);
            repo.DeleteAd(id);
            return Redirect("/home/Index");
        }


    }
}
