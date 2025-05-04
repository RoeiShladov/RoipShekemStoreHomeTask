using Microsoft.EntityFrameworkCore;
using RoipBackend.Services;
using RoipBackend;
using RoipBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using RoipBackend.Hubs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add IConfiguration to the builder  
var configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
configuration.AddEnvironmentVariables();

// Add services to the container.  
builder.Services.AddControllersWithViews();

// Add DbContext with MySQL provider
builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseMySQL(connectionString: configuration.GetConnectionString(name: "RoipDbStoreConnection")));

// Add a hosted service to clean database logs every two weeks
builder.Services.AddHostedService<DatabaseLogCleanupService>();
// Add services to the DI container.
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSingleton<JwtAuthService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<LoggerService>();
    var secretKey = configuration["Jwt:SecretKey"];
    var issuer = configuration["Jwt:Issuer"];
    var audience = configuration["Jwt:Audience"];
    return new JwtAuthService(secretKey, issuer, audience, logger);
});
builder.Services.AddAuthentication("JwtBearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
    };
});

// Add Azure Key Vault to configuration  
//var keyVaultName = configuration["KeyVaultName"]; // e.g., from appsettings.json or environment variable  
//var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
//builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
// Create a SecretClient to interact with Azure Key Vault  
//var client = new SecretClient(keyVaultUri, new DefaultAzureCredential());
//var _keyVaultName = configuration["KeyVaultName"]; // e.g., from appsettings.json or environment variable
//var _keyVaultUri = new Uri($"https://{_keyVaultName}.vault.azure.net/");
// Create a SecretClient to interact with Azure Key Vault
//var _client = new SecretClient(_keyVaultUri, new DefaultAzureCredential());
// Retrieve a secret from the Key Vault
//KeyVaultSecret secret = client.GetSecret("MySecret"); // Synchronous retrieval
//builder.Services.AddAuthentication("JwtBearer").AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = configuration["Jwt:Issuer"],
//        ValidAudience = configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Value))
//    };
//});

builder.Services.AddAuthorization();

//Access the environment
var environment = builder.Environment.EnvironmentName; // e.g., "Development", "Production"
Console.WriteLine($"Current Environment: {environment}");

//DI container is being finalized
var app = builder.Build();

// Ensure database tables are created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //dbContext.Database.Migrate(); // Applies pending migrations
    dbContext.Database.EnsureCreated(); // Ensures that the database tables are created
}

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
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<UserConnectionHub>("/userConnectionHub");
//app.MapControllerRoute(
//   name: "default",
//   pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Auth}/{action=HealthCheck}/{id?}");
app.Run();



