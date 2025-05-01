using Microsoft.AspNetCore.Mvc;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using System.ComponentModel.DataAnnotations;

namespace RoipBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IProductService _productService;

        public TransactionController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("buy-products")]
        public async Task<IActionResult> BuyProduct([FromBody] PurchaseRequest request)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the product exists
            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                return NotFound(new { Message = "Product not found." });
            }

            // Check if there is enough stock
            if (product.Quantity < request.Quantity)
            {
                return BadRequest(new { Message = "Insufficient stock." });
            }

            // Deduct the quantity
            product.Quantity -= request.Quantity;
            await _productService.UpdateProductAsync(product);

            // Optionally, log the transaction (not implemented here)

            return Ok(new { Message = "Purchase successful.", Product = product });
        }
    }

    public class PurchaseRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of products must be at least 1.")]
        public int NumberOfProducts { get; set; }
    }
}
