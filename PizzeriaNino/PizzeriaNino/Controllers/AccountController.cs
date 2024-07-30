using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PizzeriaNino.Services;
using System.Security.Claims;
using PizzeriaNino.Models;

namespace PizzeriaNino.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        // Vista di registrazione
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Azione di registrazione
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.RegisterUserAsync(model.Username, model.Email, model.Password, "User");
                if (user != null)
                {
                    // Redirect al login o alla homepage
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "Registration failed.");
            }
            return View(model);
        }

        // Vista di login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Azione di login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.AuthenticateUserAsync(model.Email, model.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Login failed.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
