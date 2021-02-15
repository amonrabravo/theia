using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheiaData.Data;
using Theia.Models;

namespace Theia.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        public AccountController(
            SignInManager<User> signInManager
            )
        {
            this.signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel { ReturnUrl = HttpContext.Request.Query["ReturnUrl"], RememberMe = true });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                    return Redirect(model.ReturnUrl ?? "/");
            }
            ModelState.AddModelError("","Geçersiz kullanıcı girişi.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
