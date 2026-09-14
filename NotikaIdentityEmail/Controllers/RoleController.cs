using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;

namespace NotikaIdentityEmail.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> RoleList()
        {
            var values = await _roleManager.Roles.ToListAsync();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            IdentityRole role = new IdentityRole()
            {
                Name = model.RoleName
            };
            await _roleManager.CreateAsync(role);
            return RedirectToAction("RoleList");
        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            var value = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);
            await _roleManager.DeleteAsync(value);
            return RedirectToAction("RoleList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRole(string id)
        {
            var value = await _roleManager.Roles.FirstOrDefaultAsync(y => y.Id == id);
            UpdateRoleViewModel updateRoleViewModel = new UpdateRoleViewModel()
            {
                RoleId = value.Id,
                RoleName = value.Name
            };
            return View(updateRoleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(UpdateRoleViewModel model)
        {
            var value = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == model.RoleId);
            value.Name = model.RoleName;
            await _roleManager.UpdateAsync(value);
            return RedirectToAction("RoleList");
        }

        public async Task<IActionResult> UserList() // Kullanıcıları listelemek.
        {
            var values = await _userManager.Users.ToListAsync();
            return View(values);
        }
        

        [HttpGet]
        public async Task<IActionResult> AssignRole(string id) // Rol ata butonundan kullanıcı id gelecek buraya. 
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id); // Kullanıcıyı buluyoruz.
            TempData["userId"] = user.Id; // Kullanıcı id'sini TempData ile saklıyoruz ki post metodunda kullanabilelim.
            var roles = await _roleManager.Roles.ToListAsync(); // Tüm rolleri çekiyoruz.
            var userRoles = await _userManager.GetRolesAsync(user); // Kullanıcının sahip olduğu rolleri çekiyoruz.
            List<RoleAssignViewModel> roleAssignViewModels = new List<RoleAssignViewModel>(); // Rol atama sayfasında göstereceğimiz model listesi oluşturuyoruz. İlk olarak boş bir liste oluşturuyoruz.
            foreach (var item in roles) // Tüm rolleri dönüyoruz.
            {
                RoleAssignViewModel model = new RoleAssignViewModel(); // Her bir rol için yeni bir model oluşturuyoruz.
                model.RoleId = item.Id; // Rol id'sini modele atıyoruz.
                model.RoleName = item.Name; // Rol adını modele atıyoruz.
                model.RoleExist = userRoles.Contains(item.Name); // Kullanıcının sahip olduğu rolleri kontrol ediyoruz. Eğer kullanıcı bu role sahipse model.RoleExist true olacak, yoksa false olacak.
                roleAssignViewModels.Add(model); // Modeli listeye ekliyoruz.
            }
            return View(roleAssignViewModels); // Rol atama sayfasına model listesini gönderiyoruz.
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(List<RoleAssignViewModel> model) //Birden fazla rol atayabileceğimiz için (seçebileceğimiz için) List<RoleAssignViewModel> model olarak yani liste olarak alıyoruz.
        {
            var userId = TempData["userId"].ToString(); // TempData ile sakladığımız kullanıcı id'sini alıyoruz.
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId); // Kullanıcıyı buluyoruz.
            foreach (var item in model) // Model listesini dönüyoruz. Yani her bir rol için işlemler yapacağız.
            {
                if (item.RoleExist) // RoleExist true ise yani rol seçilmişse
                {
                    await _userManager.AddToRoleAsync(user, item.RoleName); // kişiye rol ataması yapılıyor. 
                }
                else // seçilmediyse 
                {
                    await _userManager.RemoveFromRoleAsync(user, item.RoleName); // kişiden rol kaldırılıyor 
                }
            }
            return RedirectToAction("UserList"); 
        }
    }
}
