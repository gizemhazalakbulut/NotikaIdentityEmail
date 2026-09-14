namespace NotikaIdentityEmail.Models.JwtModels
{
    public class SimpleUserViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}
// kullanıcıdan aldıgımız hangi bilgileri token içine koyacagımızı belirliyoruz. Bu bilgiler token icerisinde claim olarak tutulur. Claimler key-value seklindedir. Key claimin ismi, value ise claimin degeri olur. Token icerisine koyacagımız claimleri belirlerken dikkat etmemiz gereken nokta, claimlerin cok fazla yer kaplamaması ve hassas bilgiler içermemesidir.