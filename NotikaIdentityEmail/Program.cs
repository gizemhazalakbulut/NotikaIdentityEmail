using Microsoft.AspNetCore.Identity;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<EmailContext>(); // DbContext sýnýfýný dependency injection ile ekliyoruz. Böylece veritabaný iþlemlerini gerçekleþtirebiliriz.
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<EmailContext>().AddErrorDescriber<CustomIdentityValidator>(); // Identity sýnýfýný ekliyoruz. AppUser sýnýfýný kullanýcý sýnýfý olarak, IdentityRole sýnýfýný ise rol sýnýfý olarak kullanýyoruz. AddEntityFrameworkStores metodu ile veritabaný iþlemlerini gerçekleþtirecek olan DbContext sýnýfýný belirtiyoruz. CustomIdentityValidator sýnýfýný ise hata mesajlarýný özelleþtirmek için kullanýyoruz.Türkçeleþtirmek için AddErrorDescriber metodu ile CustomIdentityValidator sýnýfýný ekliyoruz.

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
