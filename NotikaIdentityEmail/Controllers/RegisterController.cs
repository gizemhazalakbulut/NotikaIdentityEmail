using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;

namespace NotikaIdentityEmail.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager; // UserManager sınıfı, kullanıcı yönetimi işlemlerini gerçekleştirmek için kullanılır. AppUser sınıfını generic olarak veriyoruz, böylece kendi kullanıcı sınıfımızı kullanabiliriz.

        public RegisterController(UserManager<AppUser> userManager) // UserManager sınıfını dependency injection ile alıyoruz. Bu sayede kullanıcı yönetimi işlemlerini gerçekleştirebiliriz.
        {
            _userManager = userManager; 
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterUserViewModel model)
        {
            // Kullanıcı oluşturma işlemleri burada yapılacak

            Random rnd = new Random(); // Kullanıcıya rastgele bir aktivasyon kodu oluşturmak için Random sınıfını kullanıyoruz.
            int code = rnd.Next(100000, 1000000); // 6 haneli bir aktivasyon kodu oluşturmak için 100000 ile 999999 arasında rastgele bir sayı üretiyoruz.

            AppUser appUser = new AppUser()
            {
                Name = model.Name,
                Surname = model.Surname,
                UserName = model.Username,
                Email = model.Email,
                ActivationCode = code
            };

            var result = await _userManager.CreateAsync(appUser, model.Password);
            if (result.Succeeded)
            {
                //Buraya mail kodları gelecek (elyu gkuu dvzd pzbz) aktivasyon kodum 

                MimeMessage mimeMessage = new MimeMessage(); // MimeMessage sınıfı, e-posta mesajlarını oluşturmak için kullanılır. MimeKit kütüphanesinden gelir.
                MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin", "akbulutgizem331@gmail.com"); // MailboxAddress sınıfı, e-posta adreslerini temsil eder. Burada gönderenin adı ve e-posta adresi belirtilir.
                mimeMessage.From.Add(mailboxAddressFrom); // Gönderenin e-posta adresini mesajın From alanına ekliyoruz.

                MailboxAddress mailboxAddressTo = new MailboxAddress("User", model.Email);
                mimeMessage.To.Add(mailboxAddressTo);

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "Hesabınızı doğrulamak için gerekli olan aktivasyon kodu: " + code;
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                mimeMessage.Subject = "Notika Identity Aktivasyon Kodu";

                SmtpClient client = new SmtpClient();
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("akbulutgizem331@gmail.com", "elyugkuudvzdpzbz");
                client.Send(mimeMessage);
                client.Disconnect(true);

                TempData["EmailMove"] = model.Email;

                return RedirectToAction("UserActivation", "Activation");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            
        }
    }
}