using Microsoft.EntityFrameworkCore;
using RoipBackend.Services;
using RoipBackend;
using RoipBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using RoipBackend.Utilities;
using RoipBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add IConfiguration to the builder  
var configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
configuration.AddEnvironmentVariables();

// Add services to the container.  
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseMySQL(connectionString: configuration.GetConnectionString(name: "RoipDbStoreConnection")));
builder.Services.AddSingleton<JwtHelper>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var secretKey = configuration["Jwt:SecretKey"];
    var issuer = configuration["Jwt:Issuer"];
    var audience = configuration["Jwt:Audience"];
    return new JwtHelper(secretKey, issuer, audience);
});
//builder.Services.AddScoped<ILoggerService, LoggerService>(); 
builder.Services.AddScoped<IUserService, UserService>(); // הוספת UserService ל-DI  
builder.Services.AddScoped<IProductService, ProductService>();  
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserConnectionService, UserConnectionService>(); 
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

app.MapRazorPages();
app.MapHub<UserConnectionHub>("/userConnectionHub");
app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



