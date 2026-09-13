using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Inbox() // Gelen kutusu. Sisteme giriş yapan kullanıcıya ait mesajların listelendiği sayfa.
        {
            var values = _context.Messages.Where(x =>x.ReceiverEmail == "ali@gmail.com").ToList();
            return View(values);
        }

        public IActionResult Sendbox() // Giden kutusu. 
        {
            var values = _context.Messages.Where(x => x.SenderEmail == "ali@gmail.com").ToList();
            return View(values);
        }

        public IActionResult MessageDetail(int id)
        {
            var value = _context.Messages.Where(x => x.MessageId == id).FirstOrDefault();
            return View(value);
        }

        [HttpGet]
        public IActionResult ComposeMessage()
        {
            var categories = _context.Categories.ToList();
            ViewBag.v = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.CategoryId.ToString()
            }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ComposeMessage(Message message)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            message.SenderEmail = user.Email;
            message.SendDate = DateTime.Now;
            message.IsRead = false;
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Sendbox");
        }

    }
}
