using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;

namespace RoipBackend.Interfaces
{
    public interface IProductService
    {
        Task<IActionResult> GetAllProductsAsync();
        Task<IActionResult> AddProductAsync(Product product);        

        Task<IActionResult> BuyProductAsync(string productName, int quantity);
        Task<IActionResult> DeleteProductAsync(string productName);

        Task<IActionResult> SearchFilterAsync(string filterText);

    }
}
