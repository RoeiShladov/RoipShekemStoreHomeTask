using Microsoft.EntityFrameworkCore;
using RoipBackend.Services;
using RoipBackend;
using RoipBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using RoipBackend.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add IConfiguration to the builder  
var configuration = builder.Configuration;

// Add services to the container.  
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseMySQL(connectionString: configuration.GetConnectionString(name: "RoipDbStoreConnection")));

// הוספת קונפיגורציה של MySQL  
configuration["ConnectionStrings:RoipDbStoreConnection"] =
   $"Server=localhost;Port=3306;Database=MYSQLRoipTask;Uid={Environment.GetEnvironmentVariable("ROIP_DB_UID")};Pwd={Environment.GetEnvironmentVariable("ROIP_DB_PWD")};";
configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
configuration.AddEnvironmentVariables();
builder.Services.AddScoped<IUserService, UserService>(); // הוספת UserService ל-DI  
builder.Services.AddSingleton<JwtHelper>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var secretKey = configuration["Jwt:SecretKey"];
    var issuer = configuration["Jwt:Issuer"];
    var audience = configuration["Jwt:Audience"];
    return new JwtHelper(secretKey, issuer, audience);
});

//builder.Services.AddScoped<IProductService, ProductService>();  
//builder.Services.AddScoped<IUserConnectionService, UserConnectionService>();  
//builder.Services.AddScoped<ILoggerService, LoggerService>();  
builder.Services.AddSingleton<IAuthorizationHandler, TokenExpirationHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.  
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.  
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
