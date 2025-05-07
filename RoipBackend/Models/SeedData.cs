using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;

namespace RoipBackend.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                using (var context = serviceProvider.GetRequiredService<AppDbContext>())
                {
                    // Ensure database is created  
                    context.Database.EnsureCreated();

                    // Seed Users table  
                    if (!context.Users.Any())
                    {
                        context.Users.AddRange(
                            new User("Roei", HashPassword("Aa123456"), "Customer", "Roei@gmail.com", "0540123456", "Golda 67, Dimona"),
                            new User("Roee", HashPassword("Aa123456"), "Customer", "Roee@gmail.com", "0549012345", "Ben-Gurion 48, Beer-Sheva"),
                            new User("Roy", HashPassword("Aa123456"), "Customer", "Roy@gmail.com", "0548901234", "Waizman 22, Ness-Ziona"),
                            new User("Roey", HashPassword("Aa123456"), "Customer", "Roey@gmail.com", "0547890123", "Herzel 1, Rehovot"),
                            new User("Roi", HashPassword("Aa123456"), "Customer", "Roi@gmail.com", "0546789012", "Rothschild 53, Rishon-Lezion"),
                            new User("Fayena", HashPassword("Aa123456"), "Customer", "Fayena@gmail.com", "0545678901", "Tollip 11, Bat-Yam"),
                            new User("Assaf", HashPassword("Aa123456"), "Customer", "Assaf@gmail.com", "0544567890", "Rakefet 7, Yavne"),
                            new User("Manor", HashPassword("Aa123456"), "Customer", "Manor@gmail.com", "0543456789", "Narkis 8, Herzelia"),
                            new User("Amir", HashPassword("Aa123456"), "Customer", "Amir@gmail.com", "0542345678", "Kalanit 6, Tel-Aviv"),
                            new User("Tomer", HashPassword("Aa123456"), "Admin", "Tomer@gmail.com", "0541234567", "Vered 4, Ramat-Gan")
                        );
                    }

                    // Seed Products table  
                    if (!context.Products.Any())
                    {
                        context.Products.AddRange(
                            new Product("Bisli", "Snack", 7, 20, ""),
                            new Product("Apple", "Fruit", 3, 30, ""),
                            new Product("Snack", "Sweet Snack", 5, 50, ""),
                            new Product("Beer", "Beverage Drink", 22, 25, ""),
                            new Product("Zero", "Soft drink", 10, 70, ""),
                            new Product("Tomato", "Vegetable", 2, 150, ""),
                            new Product("Pasta", "Barilla", 16, 40, ""),
                            new Product("Water", "Steel drink", 8, 120, ""),
                            new Product("Salmon", "Norwegian", 40, 30, "")
                        );
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed  
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            }
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
