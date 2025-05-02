using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using RoipBackend.Services;
using RoipBackend.Utilities;

namespace RoipBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        //TODO: Add Loggers to all functions
        private readonly IConfiguration _configuration;
        private readonly ProductService _productService;
        private readonly LoggerService _loggerService;
        private readonly JwtHelper _jwtHelper;

        public ProductsController(IConfiguration configuration, ProductService productService, LoggerService loggerService, JwtHelper jwtHelper)
        {
            _configuration = configuration;
            _productService = productService;
            _loggerService = loggerService;
            _jwtHelper = jwtHelper;
        }


        [HttpGet("get-all-products")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllProductsAsync(string jwt, int pageNumber, int pageSize)
        {          
            IActionResult jwt_ValidationResult = _jwtHelper.JwtCheck(jwt);
            if (jwt_ValidationResult is OkResult)
                return await _productService.GetAllProductsAsync(pageNumber, pageSize);
            return jwt_ValidationResult;
        }


        [HttpPost("add-product")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> AddProductAsync(string jwt, Product product)
        {
            IActionResult jwt_ValidationResult = _jwtHelper.JwtCheck(jwt);
            if (jwt_ValidationResult is UnauthorizedObjectResult)
                return jwt_ValidationResult;

            if (!ModelState.IsValid)                                
                return ModelStateError(Consts.VALIDATION_FAILED_STR);
            
            return await _productService.AddProductAsync(product);
        }


        [HttpPut("buy-product/{productName}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> BuyProductAsync(string jwt, string productName, int quantity)
        {
            IActionResult jwt_ValidationResult = _jwtHelper.JwtCheck(jwt);
            if (jwt_ValidationResult is UnauthorizedObjectResult)
                return jwt_ValidationResult;

            if (!ModelState.IsValid)
                return ModelStateError(Consts.VALIDATION_FAILED_STR);

            return await _productService.BuyProductAsync(productName, quantity);
        }


        [HttpDelete("delete-product/{productName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductAsync(string jwt, string productName)
        {
            IActionResult jwt_ValidationResult = _jwtHelper.JwtCheck(jwt);
            if (jwt_ValidationResult is UnauthorizedObjectResult)
                return jwt_ValidationResult;

            if (!ModelState.IsValid)
                return ModelStateError(Consts.VALIDATION_FAILED_STR);

            return await _productService.DeleteProductAsync(productName);
        }


        [HttpGet("search-filter")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> SearchFilterAsync(string filterText)
        {
            return await _productService.SearchFilterAsync(filterText);
        }


        private BadRequestObjectResult ModelStateError(string message)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(new { Message = message, Errors = errors })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }       
    }
}
