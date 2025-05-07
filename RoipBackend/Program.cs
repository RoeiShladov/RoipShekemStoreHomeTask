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
using System.Data.Entity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using RoipBackend.Models;

var builder = WebApplication.CreateBuilder(args);
// Add IConfiguration to the builder  
var configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
configuration.AddEnvironmentVariables();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add DbContext with MySQL provider
builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseMySQL(connectionString: configuration.GetConnectionString(name: "RoipDbStoreConnection")));

// Add a hosted service to clean database logs every two weeks
//builder.Services.AddHostedService<DatabaseLogCleanupService>();
// Add services to the DI container.
builder.Services.AddScoped<LoggerService>();
builder.Services.AddScoped<UserService>(); 
builder.Services.AddScoped<ProductService>();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddScoped<JwtAuthService>(provider =>
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

//Access the environment
var environment = builder.Environment.EnvironmentName; // e.g., "Development", "Production"
Console.WriteLine($"Current Environment: {environment}");

//DI container is being finalized
var app = builder.Build();
//Ensure database and tables are created
using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Check if the database exists
    var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
    if (databaseCreator.Exists())
    {
        // Check if the database has tables
        if (databaseCreator.HasTables())
        {
            Console.WriteLine("Database and tables exist.");
        }
        else
        {

            // Apply pending migrations instead of ensuring database creation
            //context.Database.Migrate(); // Applies any pending migrations for the context to the database
            context.Database.EnsureCreated();
            var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            // Seed the database with initial data
            SeedData.Initialize(serviceProvider);

            Console.WriteLine("Database exists, but no tables found. Migrations applied.");
            
    
            //context.Database.EnsureCreated(); // Creates the database if it doesn't exist
            //var scope = app.Services.CreateScope();
            //var serviceProvider = scope.ServiceProvider;
            //SeedData.Initialize(serviceProvider);


                //SeedData.Initialize(app.Services); // Seed the database with initial data (custom hand writed 10 rows for 'User' and 'Product' tables, 1 of the Users is Admin)
                Console.WriteLine("Database exists, but no tables found.");
        }
    }
    else
    {
        Console.WriteLine("Database does not exist.");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
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



