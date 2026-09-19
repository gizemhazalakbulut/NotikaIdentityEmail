using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Models.JwtModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager; // SignInManager sınıfı, kullanıcı giriş işlemlerini gerçekleştirmek için kullanılır. AppUser sınıfını generic olarak veriyoruz, böylece kendi kullanıcı sınıfımızı kullanabiliriz.
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtSettingsModel _jwtSettingsModel;

        public LoginController(SignInManager<AppUser> signInManager, EmailContext context, UserManager<AppUser> userManager, IOptions<JwtSettingsModel> jwtSettingsModel)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _jwtSettingsModel = jwtSettingsModel.Value;
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

            var value = _context.Users.FirstOrDefault(x => x.UserName == model.Username); // Kullanıcı adı ile veritabanında kullanıcıyı buluyoruz.

            SimpleUserViewModel simpleUserViewModel = new SimpleUserViewModel()
            {
                City = value.City,
                Email = value.Email,
                Id = value.Id,
                Name = value.Name,
                Surname = value.Surname,
                Username =value.UserName

            };

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

            if (!value.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı pasif durumda, giriş yapamaz!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, true); // Kullanıcı adı ve şifreyi kontrol ediyoruz. true parametresi ile kullanıcıyı hatırlamasını sağlıyoruz. true parametresi ile başarısız giriş denemelerinde hesabı kilitlemesini sağlıyoruz.
            if (result.Succeeded)
            {
                
                var token = GenerateJwtToken(simpleUserViewModel);

                Response.Cookies.Append("jwtToken", token, new CookieOptions
                {
                    HttpOnly =true,
                    Secure = true,
                    SameSite= SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettingsModel.ExpireMinutes)
                });

                return RedirectToAction("EditProfile", "Profile");
            }
            ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre yanlış");
            return View(model);


        }


       
        public string GenerateJwtToken(SimpleUserViewModel simpleUserViewModel)
        {
            var claim = new[] // claimler token icerisine koyacagımız bilgileri temsil eder. Claimler key-value seklindedir. Key claimin ismi, value ise claimin degeri olur. Token icerisine koyacagımız claimleri belirlerken dikkat etmemiz gereken nokta, claimlerin cok fazla yer kaplamaması ve hassas bilgiler içermemesidir.
            {
                new Claim("name",simpleUserViewModel.Name),
                new Claim("surname",simpleUserViewModel.Surname),
                new Claim("city",simpleUserViewModel.City),
                new Claim("username",simpleUserViewModel.Username),
                new Claim(ClaimTypes.NameIdentifier,simpleUserViewModel.Id),
                new Claim(ClaimTypes.Email,simpleUserViewModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                // JwtRegisteredClaimNames.Jti claimi tokenin unique olmasını saglar. Bu claim tokenin tekrar kullanılmasını engeller. guid ile unique bir id olusturuyoruz ve bu idyi claim olarak ekliyoruz. Bu sayede token tekrar kullanılmaya calısılırsa, bu claim sayesinde tokenin tekrar kullanılmasını engelleyebiliriz.
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettingsModel.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _jwtSettingsModel.Issuer,
                audience: _jwtSettingsModel.Audience,
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettingsModel.ExpireMinutes),
                signingCredentials: creds);

            //simpleUserViewModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            //return View(simpleUserViewModel);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [HttpGet]
        public IActionResult LoginWithGoogle()
        { 
            return View(); 
        }  

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string? returnUrl= null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Login", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallBack(string? returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/"); // ?? null atama operatörüdür. returnUrl nullsa returnUrl = Url.Content("~/") 
            if (remoteError != null)
            {
                ModelState.AddModelError("", $"External Provider Error: {remoteError}");
                return RedirectToAction("Login");

            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Inbox", "Message");
            }
           
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = new AppUser()
                {
                    UserName = email,
                    Email = email,
                    Name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "Google",
                    Surname = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "User"
                };

                var identityResult = await _userManager.CreateAsync(user);
                if (identityResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Inbox", "Message");
                }

                return RedirectToAction("UserLogin");
            
        }   
    }
}
