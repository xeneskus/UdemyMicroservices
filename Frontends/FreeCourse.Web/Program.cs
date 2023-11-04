using FreeCourse.Web.Models;
using FreeCourse.Web.Services;
using FreeCourse.Web.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<ServiceApiSettings>(builder.Configuration.GetSection("ServiceApiSettings"));
builder.Services.Configure<ServiceApiSettings>(builder.Configuration.GetSection("ClientSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IIdentityService, IdentityService>();
var serviceApiSettings = builder.Configuration.GetSection("ServiceApiSettings").Get<ServiceApiSettings>();
builder.Services.AddHttpClient<IUserService, UserService>(opt=>
{
    opt.BaseAddress = new Uri(serviceApiSettings.IdentityBaseUri);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
{
    opts.LoginPath = "/Auth/SignIn";
    opts.ExpireTimeSpan = TimeSpan.FromDays(60);
    opts.SlidingExpiration = true; // sürekli uzasýn mý 60 gün içinde tekrar girince
    opts.Cookie.Name = "udemywebcookie";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
