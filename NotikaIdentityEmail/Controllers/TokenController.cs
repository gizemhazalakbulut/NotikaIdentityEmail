using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Models.JwtModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotikaIdentityEmail.Controllers
{
    public class TokenController : Controller
    {
        private readonly JwtSettingsModel _jwtSettingsModel;
        public TokenController(IOptions<JwtSettingsModel> jwtSettingsModel)
        {
            _jwtSettingsModel = jwtSettingsModel.Value;
        }
        public IActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(SimpleUserViewModel simpleUserViewModel)
        {
            var claim = new[] // claimler token icerisine koyacagımız bilgileri temsil eder. Claimler key-value seklindedir. Key claimin ismi, value ise claimin degeri olur. Token icerisine koyacagımız claimleri belirlerken dikkat etmemiz gereken nokta, claimlerin cok fazla yer kaplamaması ve hassas bilgiler içermemesidir.
            {
                new Claim("name",simpleUserViewModel.Name),
                new Claim("surname",simpleUserViewModel.Surname),
                new Claim("city",simpleUserViewModel.City),
                new Claim("username",simpleUserViewModel.Username),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), // JwtRegisteredClaimNames.Jti claimi tokenin unique olmasını saglar. Bu claim tokenin tekrar kullanılmasını engeller. guid ile unique bir id olusturuyoruz ve bu idyi claim olarak ekliyoruz. Bu sayede token tekrar kullanılmaya calısılırsa, bu claim sayesinde tokenin tekrar kullanılmasını engelleyebiliriz.
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettingsModel.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettingsModel.Issuer,
                audience: _jwtSettingsModel.Audience,
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettingsModel.ExpireMinutes),
                signingCredentials: creds);

            simpleUserViewModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return View(simpleUserViewModel);
        }
    }
}
