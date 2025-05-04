using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;
using RoipBackend.Services;
using RoipBackend.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RoipBackend.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly LoggerService _loggerService;

        public ProductsController(ProductService productService, LoggerService loggerService)
        {
            _productService = productService;
            _loggerService = loggerService;
        }


        [HttpGet(C.GET_ALL_PRODUCTS_API_STR)]
        [Authorize(Roles = $"{C.ADMIN_STR},{C.CUSTOMER_STR}")]
        public async Task<IActionResult> GetAllProductsAsync(
            [Required(ErrorMessage = C.JWT_MODEL_STATE_INVALID_STR)] string jwt,
            int pageNumber,
            int pageSize)
        {            
            ServiceResult<List<Product>> result = await _productService.GetAllProductsAsync(jwt, pageNumber, pageSize);
            if (result.Success)
            {                
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                    result.Data
                });
            }

            return HandleStatusCode(result.StatusCode, result.Message, result.Error);
        }


        [HttpPost(C.ADD_PRODUCT_API_STR)]
        [Authorize(Roles = $"{C.ADMIN_STR}")]
        public async Task<IActionResult> AddProductAsync(
            [Required(ErrorMessage = C.JWT_MODEL_STATE_INVALID_STR)] string jwt,
             Product product)
        {          
            if (!ModelState.IsValid)
            {
                await _loggerService.LogWarningAsync(C.MODEL_STATE_VALIDATION_FAILED_STR);
                return ModelStateWarning(C.MODEL_STATE_VALIDATION_FAILED_STR);
            }

            ServiceResult<string> result = await _productService.AddProductAsync(jwt, product);
            if (result.Success)
            {
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                    result.RoipShekemStoreJWT
                });
            }

            return HandleStatusCode(result.StatusCode, result.Message, result.Error);
        }


        [HttpDelete(C.DELETE_PRODUCT_API_STR)]
        [Authorize(Roles = C.ADMIN_STR)]
        public async Task<IActionResult> DeleteProductAsync(
            [Required(ErrorMessage = C.JWT_MODEL_STATE_INVALID_STR)] string jwt,
            [StringLength(100, ErrorMessage = C.PRODUCT_NAME_MODEL_STATE_INVALID_STR)] string productName)
        {
            if (!ModelState.IsValid)
            {
                await _loggerService.LogWarningAsync(C.MODEL_STATE_VALIDATION_FAILED_STR);
                return ModelStateWarning(C.MODEL_STATE_VALIDATION_FAILED_STR);
            }

            var result = await _productService.DeleteProductAsync(jwt, productName);
            if (result.Success)
            {
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                    result.Data
                });
            }

            return HandleStatusCode(result.StatusCode, result.Message, result.Error);
        }


        [HttpPut(C.BUY_PRODUCT_API_STR)]
        [Authorize(Roles = $"{C.ADMIN_STR},{C.CUSTOMER_STR}")]
        public async Task<IActionResult> BuyProductAsync(
            [Required(ErrorMessage = C.JWT_MODEL_STATE_INVALID_STR)] string jwt,
            [StringLength(100, ErrorMessage = C.PRODUCT_NAME_MODEL_STATE_INVALID_STR)] string productName,
            [StringLength(10, ErrorMessage = C.PRODUCT_QUANTITY_MODEL_STATE_INVALID_STR)]  int quantity)
        {
            if (!ModelState.IsValid)
            {
                await _loggerService.LogWarningAsync(C.MODEL_STATE_VALIDATION_FAILED_STR);
                return ModelStateWarning(C.MODEL_STATE_VALIDATION_FAILED_STR);
            }    

            var result = await _productService.BuyProductAsync(jwt, productName, quantity);
            if (result.Success)
            {
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                    result.Data
                });
            }

            return HandleStatusCode(result.StatusCode, result.Message, result.Error);
        }


        [HttpGet(C.SEARCH_PRODUCT_API_STR)]
        [Authorize(Roles = $"{C.ADMIN_STR},{C.CUSTOMER_STR}")]
        public async Task<IActionResult> SearchProductAsync(
            [Required(ErrorMessage = C.JWT_MODEL_STATE_INVALID_STR)] string jwt,
            [StringLength(100, ErrorMessage = C.FILTER_MODEL_STATE_INVALID_STR)] string filterText,
            [Range(0, int.MaxValue, ErrorMessage = C.MINIMUM_PRICE_MODEL_STATE_INVALID_STR)] int minPrice,
            [Range(0, int.MaxValue, ErrorMessage = C.MAXIMUM_PRICE_MODEL_STATE_INVALID_STR)] int maxPrice)
        {
            if(!ModelState.IsValid)
            {
                await _loggerService.LogWarningAsync(C.MODEL_STATE_VALIDATION_FAILED_STR);
                return ModelStateWarning(C.MODEL_STATE_VALIDATION_FAILED_STR);
            }

            var result = await _productService.SearchProductAsync(jwt, filterText, minPrice, maxPrice);
            if(result.Success)
            {
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                    result.Data
                });
            }

            return HandleStatusCode(result.StatusCode, result.Message, result.Error);
        }

      
        private BadRequestObjectResult ModelStateWarning(string message)
        {
            var warning = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new { Message = message, Warning = warning, StatusCode = StatusCodes.Status400BadRequest });
        }
        
        
        private IActionResult HandleStatusCode(int statusCode, string message, string error)
        {
            switch (statusCode)
            {
                case StatusCodes.Status400BadRequest:
                    return BadRequest(new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status401Unauthorized:
                    return Unauthorized(new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status403Forbidden:
                    return StatusCode(StatusCodes.Status403Forbidden, new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status404NotFound:
                    return NotFound(new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status408RequestTimeout:
                    return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status409Conflict:
                    return Conflict(new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status500InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = message, Error = error, StatusCode = statusCode });

                default:
                    return new ObjectResult(new { Message = message, Error = error, StatusCode = statusCode });
            }
        }
    }
}