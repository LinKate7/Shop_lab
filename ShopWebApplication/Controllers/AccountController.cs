using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ShopWebApplication.ViewModels;
using ShopWebApplication.Models;

namespace ShopWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserForIdentity> _userManager;
        private readonly SignInManager<UserForIdentity> _signInManager;
        private readonly IdentityContext _identityContext;

        public AccountController(UserManager<UserForIdentity> userManager, SignInManager<UserForIdentity> signInManager, IdentityContext identityContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityContext = identityContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserForIdentity user = new UserForIdentity { Email = model.Email, UserName = model.Email, FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber, Year = model.Year, };
                // додаємо користувача
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка кукі
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Categories");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

    }
}
