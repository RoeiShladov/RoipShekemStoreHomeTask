using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;
using RoipBackend.Utilities;

namespace RoipBackend.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult<List<Product>>> GetAllProductsAsync(string jwt, int pageNumber, int pageSize);
        Task<ServiceResult<string>> AddProductAsync(string jwt, Product newProduct);

        Task<ServiceResult<Product>> DeleteProductAsync(string jwt, string productName);

        Task<ServiceResult<Product>> BuyProductAsync(string jwt, string productName, int quantity);

        Task<ServiceResult<List<Product>>> SearchProductAsync(string jwt, string filterText, int? minPrice = null, int? maxPrice = null);

    }
}
