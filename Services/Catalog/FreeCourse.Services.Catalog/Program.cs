using System.Reflection;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Services.Catalog.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile($"configuration.{builder.Environment.EnvironmentName.ToLower()}.json");
// Add services to the container.

builder.Services.AddControllers(opt =>
{
    //opt.Filters.Add(new AauthorizeFilter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(Options =>
{
    Options.Authority = builder.Configuration["IdentityServerUrl"]; //token ý daðýtan bilgisi
    Options.Audience = "resource_catalog";//gelen token içerisinde mutlaka resource catolog olmasý lazým belirtiyoru<
    Options.RequireHttpsMetadata = false;//https kullanmadýk burada belirtmek lazým
});//bayi - müþteri için ayrý ayrý þema

//builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped(typeof(ICategoryService), typeof(CategoryService));

builder.Services.AddScoped(typeof(ICourseService), typeof(CourseService));

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());




builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});

//var dbs = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

//builder.Services.AddSingleton<IDatabaseSettings, DatabaseSettings>(sp => { return dbs; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())

{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();//biz ekledik
app.UseAuthorization();

app.MapControllers();

app.Run();
