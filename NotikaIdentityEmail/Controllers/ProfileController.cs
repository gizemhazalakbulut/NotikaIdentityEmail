using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {

            var values = await _userManager.GetUserAsync(User);

            if (values == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            /* var values = await _userManager.FindByNameAsync(User.Identity.Name);*/ // Kullanıcının kullanıcı adını alıyoruz ve veritabanında bu kullanıcıyı buluyoruz.
            UserEditViewModel userEditViewModel = new UserEditViewModel();
            userEditViewModel.Name = values.Name;
            userEditViewModel.Surname = values.Surname;
            userEditViewModel.PhoneNumber = values.PhoneNumber;
            userEditViewModel.ImageUrl = values.ImageUrl;
            userEditViewModel.City = values.City;
            userEditViewModel.UserName = values.UserName;
            userEditViewModel.Email = values.Email;
            return View(userEditViewModel);
          
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditViewModel model)
        {
            if (model.Password == model.PasswordConfirm) // Şifre ve şifre onayı eşleşiyorsa kullanıcı bilgilerini güncelliyoruz.
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name); // Kullanıcının kullanıcı adını alıyoruz ve veritabanında bu kullanıcıyı buluyoruz.
                user.Name = model.Name;
                user.Surname = model.Surname;
                user.PhoneNumber = model.PhoneNumber;
                user.City = model.City;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.ImageUrl = model.ImageUrl;
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                await _userManager.UpdateAsync(user);
            }
            return View();
        }
    }
}
