using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;

namespace RoipBackend.Interfaces
{
    public interface IProductService
    {
        Task<IActionResult> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<IActionResult> AddProductAsync(Product newProduct);        

        Task<IActionResult> BuyProductAsync(string productName, int quantity);
        Task<IActionResult> DeleteProductAsync(string productName);

        Task<IActionResult> SearchFilterAsync(string filterText, int? minPrice = null, int? maxPrice = null);

    }
}
