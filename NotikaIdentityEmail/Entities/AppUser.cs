using Microsoft.AspNetCore.Identity;

namespace NotikaIdentityEmail.Entities
{
    public class AppUser: IdentityUser // IdentityUser sınıfını miras alır, bu sayede kullanıcı yönetimi için gerekli özellikleri içerir. Hem kendi özelliklerimizi ekleyebiliriz hem de IdentityUser sınıfının özelliklerini kullanabiliriz.
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; } // boş olabilir ? işareti sayesinde
        public string? City { get; set; } // boş olabilir
    }
}
