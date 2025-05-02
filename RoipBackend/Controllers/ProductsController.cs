using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;
using RoipBackend.Services;
using RoipBackend.Utilities;

namespace RoipBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        //TODO: Add refresh token / cookies functionality (???)

        private readonly IConfiguration _configuration;
        private readonly ProductService _productService;
        private readonly LoggerService _loggerService;

        public ProductsController(IConfiguration configuration, ProductService productService, LoggerService loggerService)
        {
            _configuration = configuration;
            _productService = productService;
            _loggerService = loggerService;
        }

        [HttpGet("get-all-products")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var serviceResult = await _productService.GetAllProductsAsync();
            return HandleServiceResult(serviceResult);
        }

        [HttpPost("add-product")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> AddProductAsync([FromBody] Product product)
        {
            if (!ModelState.IsValid)                                
                return ModelStateError(Consts.VALIDATION_FAILED_STR);
            
            IActionResult serviceResult = await _productService.AddProductAsync(product);
            return HandleServiceResult(serviceResult);
        }

        [HttpPut("buy-product/{productName}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> BuyProductAsync(string productName, [FromBody] int quantity)
        {
            if (!ModelState.IsValid)
                return ModelStateError(Consts.VALIDATION_FAILED_STR);

            IActionResult serviceResult = await _productService.BuyProductAsync(productName, quantity);
            return HandleServiceResult(serviceResult);
        }

        [HttpDelete("delete-product/{productName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductAsync(string productName)
        {
            if (!ModelState.IsValid)
                return ModelStateError(Consts.VALIDATION_FAILED_STR);

            IActionResult serviceResult = await _productService.DeleteProductAsync(productName);
            return HandleServiceResult(serviceResult);
        }

        [HttpGet("search-filter")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> SearchFilterAsync([FromQuery] string filterText)
        {
            IActionResult serviceResult = await _productService.SearchFilterAsync(filterText);
            return HandleServiceResult(serviceResult);
        }

        private BadRequestObjectResult ModelStateError(string message)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(new { Message = message, Errors = errors });
        }

        private static ActionResult HandleServiceResult(IActionResult serviceResult)
        {
            return ServiceResultHandler.HandleServiceResult(serviceResult);
        }        
    }
}
