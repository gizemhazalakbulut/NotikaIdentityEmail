namespace NotikaIdentityEmail.Models.IdentityModels
{
    public class RegisterUserViewModel // RegisterUserViewModel sınıfı, kullanıcı kayıt formundan gelen verileri tutmak için kullanılır. Bu sınıf, kullanıcı adı, e-posta, şifre gibi bilgileri içerir ve bu bilgileri controller'a iletmek için kullanılır.
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
