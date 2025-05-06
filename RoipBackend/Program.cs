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
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
//builder.Services.AddHostedService<DatabaseLogCleanupService>();
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular's default dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        }
    };

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
// Fix 1: Change DatabaseLogCleanupService to use a scoped service provider for AppDbContext  
//builder.Services.AddHostedService<DatabaseLogCleanupService>(provider =>
//{
//    var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
//    // Fix 1: Change DatabaseLogCleanupService to use a scoped service provider for AppDbContext  
//    builder.Services.AddHostedService(provider =>
//    {
//        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
//        using var scope = scopeFactory.CreateScope();
//        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//        return new DatabaseLogCleanupService(dbContext);
//    });
//    return new DatabaseLogCleanupService(scopeFactory);
//});

// Fix 2: Ensure LoggerService is registered as a scoped service  
builder.Services.AddScoped<LoggerService>();

// Fix 3: Update UserService and ProductService constructors to accept ILoggerService instead of LoggerService  
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
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

app.UseCors("AllowAngularClient");

app.MapRazorPages();
// The purpose of app.MapHub<UserConnectionHub>("/userConnectionHub") is to map the SignalR hub named 'UserConnectionHub' to the specified endpoint '/userConnectionHub'.
// This allows clients to establish WebSocket connections to the hub for real-time communication.
app.MapHub<UserConnectionHub>(C.HUB_LIVE_BROADCAST_URL_STR);

app.MapControllerRoute(
  name: "default",
  pattern: "${controller=User}/{action=login}");
app.Run();



