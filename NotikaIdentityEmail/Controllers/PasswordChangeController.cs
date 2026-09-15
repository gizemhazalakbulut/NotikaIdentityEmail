using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.ForgetPasswordModels;
using NuGet.Common;
using System.Security.Principal;

namespace NotikaIdentityEmail.Controllers
{
    public class PasswordChangeController : Controller
    {
        private readonly UserManager<AppUser> _userManager; //UserManager, ASP.NET Core Identity'nin kullanıcı işlemlerini yapan servisidir.
        public PasswordChangeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordViewModel.Email); // Kullanıcının e-posta adresine göre kullanıcıyı buluyoruz.
            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user); // Identity kullanıcı için özel bir şifre sıfırlama token'ı oluşturuyor.

            //Şifre değiştirme linki hazırlanıyor
            var passwordResetTokenLink = Url.Action("ResetPassword", "PasswordChange", new
            {
                userId = user.Id,
                token = passwordResetToken
            }, HttpContext.Request.Scheme); // userId , token linke parametre olarak ekleniyor. HttpContext.Request.Scheme ile linkin http veya https olacağı belirleniyor.

            //Mail oluşturuluyor
            //MailKit / MimeKit kullanılarak boş bir e-mail oluşturuyorsun.Sonraki satırlarda dolduruyorsun.
            MimeMessage mimeMessage = new MimeMessage();

            MailboxAddress mailboxAddressFrom = new MailboxAddress("Notika Admin", "akbulutgizem331@gmail.com"); // Maili gönderen kişi

            mimeMessage.From.Add(mailboxAddressFrom); // Maili gönderen kişi ekleniyor.

            MailboxAddress mailboxAddressTo = new MailboxAddress("User", forgetPasswordViewModel.Email); // Maili alan kişi ekleniyor.
            mimeMessage.To.Add(mailboxAddressTo); // Maili alan kişi ekleniyor.

            var bodyBuilder = new BodyBuilder(); // Mail gövdesini hazırlamak için kullanılıyor.
            bodyBuilder.TextBody = passwordResetTokenLink; //mailin içerisine oluşturduğumuz link koyuluyor.
            mimeMessage.Body = bodyBuilder.ToMessageBody(); //hazırladığın içerik mail mesajına ekleniyor.

            mimeMessage.Subject = "Şifre Değişiklik Talebi"; //mailin konusu belirleniyor.
            SmtpClient client = new SmtpClient(); //Mail gönderecek SMTP istemcisi oluşturuluyor.
            client.Connect("smtp.gmail.com", 587, false); //SMTP sunucusuna bağlanıyor. Gmail için smtp.gmail.com ve port 587 kullanılıyor. Güvenli bağlantı (SSL) kullanılmıyor.
            client.Authenticate("akbulutgizem331@gmail.com", "elyugkuudvzdpzbz"); //SMTP sunucusuna bağlanmak için kimlik doğrulaması yapılıyor. Gmail hesabının kullanıcı adı ve uygulama şifresi kullanılıyor.
            client.Send(mimeMessage); //Mail gönderiliyor.
            client.Disconnect(true); //SMTP sunucusundan bağlantı kesiliyor.

            return View(); //Mail gönderildikten sonra aynı sayfa tekrar gösteriliyor.
        }

//Birisi sadece mail adresini bildiği için şifre değiştiremesin.
//Şifre değiştirmek için bu özel token'a da sahip olması gerekiyor.
//Akış şöyle oluyor : 
//Kullanıcı
//   ↓
//Şifremi unuttum
//   ↓
//E-mail adresi
//   ↓
//Identity token oluşturur
//   ↓
//Token mail ile gönderilir
//   ↓
//Kullanıcı linke tıklar
//   ↓
//Yeni şifre belirler

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var userid = TempData["userId"];
            var token = TempData["token"];

            if (userid == null || token == null)
            {
                ViewBag.v = "Hata Oluştu"; // Hata mesajı gösteriliyor.
            }
            var user = await _userManager.FindByIdAsync(userid.ToString()); // Kullanıcıyı userId ile buluyoruz.
            var result = await _userManager.ResetPasswordAsync(user, token.ToString(), resetPasswordViewModel.Password); // Identity, kullanıcının şifresini sıfırlamak için ResetPasswordAsync metodunu kullanıyor. Bu metod, kullanıcıyı, token'ı ve yeni şifreyi parametre olarak alıyor.
            if (result.Succeeded)
            {
                return RedirectToAction("UserLogin", "Login");
            }
            return View();

        }
    }
}
