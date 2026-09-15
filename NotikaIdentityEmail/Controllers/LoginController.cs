using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager; // SignInManager sınıfı, kullanıcı giriş işlemlerini gerçekleştirmek için kullanılır. AppUser sınıfını generic olarak veriyoruz, böylece kendi kullanıcı sınıfımızı kullanabiliriz.
        private readonly EmailContext _context;

        public LoginController(SignInManager<AppUser> signInManager, EmailContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginViewModel model)
        {
            // Kullanıcı giriş işlemleri burada yapılacak

            var value = _context.Users.Where(x => x.UserName == model.Username).FirstOrDefault(); // Kullanıcı adı ile veritabanında kullanıcıyı buluyoruz.

            if (value == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                return View(model);
            }

            if (!value.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "e-mail adresiniz henüz onaylanmamış.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, true); // Kullanıcı adı ve şifreyi kontrol ediyoruz. true parametresi ile kullanıcıyı hatırlamasını sağlıyoruz. true parametresi ile başarısız giriş denemelerinde hesabı kilitlemesini sağlıyoruz.
            if (result.Succeeded)
            {
                return RedirectToAction("EditProfile", "Profile");
            }
            ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre yanlış");
            return View(model);


        }
    }
}
