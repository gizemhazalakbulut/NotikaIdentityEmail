using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Models.JwtModels;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



// DbContext ve Identity
builder.Services.AddDbContext<EmailContext>(); // DbContext sýnýfýný dependency injection ile ekliyoruz. Böylece veritabaný iþlemlerini gerçekleþtirebiliriz.
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<EmailContext>().AddErrorDescriber<CustomIdentityValidator>(); // Identity sýnýfýný ekliyoruz. AppUser sýnýfýný kullanýcý sýnýfý olarak, IdentityRole sýnýfýný ise rol sýnýfý olarak kullanýyoruz. AddEntityFrameworkStores metodu ile veritabaný iþlemlerini gerçekleþtirecek olan DbContext sýnýfýný belirtiyoruz. CustomIdentityValidator sýnýfýný ise hata mesajlarýný özelleþtirmek için kullanýyoruz.Türkçeleþtirmek için AddErrorDescriber metodu ile CustomIdentityValidator sýnýfýný ekliyoruz.


// JWT Ayarlarý
builder.Services.Configure<JwtSettingsModel>(builder.Configuration.GetSection("JwtSettingsKey"));


// Cookie + JWT birlikte authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettingsKey").Get<JwtSettingsModel>();
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});


builder.Services.AddControllersWithViews();

var app = builder.Build();





// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // Authentication middleware'i ekliyoruz. Bu middleware, gelen isteklerde kimlik doðrulama iþlemlerini gerçekleþtirir. Token doðrulama iþlemi burada yapýlýr.

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
