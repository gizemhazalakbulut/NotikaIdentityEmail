namespace NotikaIdentityEmail.Models.JwtModels
{
    public class JwtSettingsModel // Token olustururken kullanacagımız ayarları belirliyoruz. Bu ayarlar appsettings.json dosyasında tutulur ve bu model ile bu ayarlara erisim saglarız.
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireMinutes { get; set; }
    }
}
