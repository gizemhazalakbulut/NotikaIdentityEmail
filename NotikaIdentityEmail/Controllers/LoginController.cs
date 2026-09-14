using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;

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
            if (value.EmailConfirmed == true) // Kullanıcının emaili doğrulanmışsa giriş yapmasına izin veriyoruz.
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("EditProfile", "Profile");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
                    return View(model);
                }
            }
            return View(); // Kullanıcının emaili doğrulanmamışsa giriş yapmasına izin vermiyoruz.


        }
    }
}
