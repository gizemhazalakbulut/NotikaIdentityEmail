using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.Controllers
{
    public class UserController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> UserList()
        {
            var values = await _userManager.Users.ToListAsync();
            return View(values);
        }

        public async Task<IActionResult> DeactivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> ActivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = true;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("UserList");
        }
    }
}
