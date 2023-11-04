using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(Options =>
{
    Options.Authority = builder.Configuration["IdentityServerUrl"]; //token ý daðýtan bilgisi
    Options.Audience = "resource_photo_stock";//gelen token içerisinde mutlaka resource catolog olmasý lazým belirtiyoru<
    Options.RequireHttpsMetadata = false;//https kullanmadýk burada belirtmek lazým
});//bay

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.PhotoStock", Version = "v1" });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();//public olarak dýþarýdan eriþmiþ olucaz
app.UseAuthorization();
app.UseAuthentication();//
app.MapControllers();

app.Run();
