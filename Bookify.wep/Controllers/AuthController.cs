using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Bookify.Domain.Entities;
using Bookify.wep.Services.Auth;
using Bookify.wep.Models.Auth;

namespace Bookify.wep.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string usernameOrEmail, string password)
        {
            usernameOrEmail = usernameOrEmail?.Trim();
            password = password?.Trim();

            var user = await _auth.ValidateUserAsync(usernameOrEmail, password);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "اسم المستخدم أو كلمة المرور غير صحيحة");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserRole", user.UserRole ?? "client");
            HttpContext.Session.SetString("UserName", user.UserName ?? user.Email ?? "");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userName = model.UserName?.Trim();
            var email = model.Email?.Trim();

            if (await _auth.IsEmailOrUserNameTakenAsync(userName!, email!))
            {
                ModelState.AddModelError(string.Empty, "اسم المستخدم أو البريد الإلكتروني مستخدم بالفعل.");
                return View(model);
            }

            var user = await _auth.CreateUserAsync(userName!, email!, model.Password, "client");

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserRole", user.UserRole ?? "client");
            HttpContext.Session.SetString("UserName", user.UserName ?? user.Email ?? "");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


    }
}
