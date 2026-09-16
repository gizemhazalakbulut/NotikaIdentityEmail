using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.ViewComponents
{
    public class _HeaderUserLayoutComponentPartial:ViewComponent
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;
        public _HeaderUserLayoutComponentPartial(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userValue = await _userManager.FindByNameAsync(User.Identity.Name); // Kullanıcıyı bulmak için UserManager sınıfının FindByNameAsync metodunu kullanıyoruz. User.Identity.Name ile giriş yapmış kullanıcının kullanıcı adını alıyoruz.
            var userEmail = userValue.Email; // Kullanıcının email adresini alıyoruz.
            var userEmailCount = _context.Messages.Where(x => x.ReceiverEmail == userEmail).Count(); // Kullanıcının gelen kutusundaki mesaj sayısını almak için Messages tablosunda ReceiverEmail alanı kullanıcı email adresine eşit olan kayıtların sayısını alıyoruz.
            ViewBag.userEmailCount = userEmailCount; // ViewBag ile kullanıcıya ait gelen kutusundaki mesaj sayısını view tarafına gönderiyoruz.
            ViewBag.notificationCount = _context.Notifications.Count(); // ViewBag ile bildirim sayısını view tarafına gönderiyoruz.
            return View();
        }
    }
}
