using RoipBackend.Models;
using Microsoft.EntityFrameworkCore;
using RoipBackend.Interfaces;
using RoipBackend.Utilities;


namespace RoipBackend.Services
{    
    public class ProductService : IProductService
    {
        private readonly AppDbContext _DBcontext;
        private readonly LoggerService _loggerService;
        private readonly JwtAuthService _jwtAuthService;

        public ProductService(AppDbContext context, LoggerService loggerService, JwtAuthService jwtAuthService)
        {
            _DBcontext = context;
            _loggerService = loggerService;
            _jwtAuthService = jwtAuthService;
            _DBcontext.Database.SetCommandTimeout(C.DB_REQUEST_TIMEOUT);
        }

        public async Task<ServiceResult<List<Product>>> GetAllProductsAsync(string jwt, int pageNumber, int pageSize)
        {
            try
            {
                ServiceResult<string> validationResult = await _jwtAuthService.JwtContentCheck(jwt, $"{C.CUSTOMER_STR},{C.ADMIN_STR}");
                if (!validationResult.Success)
                {
                    await _loggerService.LogWarningAsync($"{validationResult.Message}. {validationResult.Error}. {validationResult.StatusCode}");
                    return new ServiceResult<List<Product>>
                    {
                        Message = validationResult.Message,
                        Error = validationResult.Error,
                        StatusCode = validationResult.StatusCode
                    };
                }

                //Client side validation should be fine but it is good to have server side validation as well.
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return new ServiceResult<List<Product>>
                    {
                        Success = false,
                        Message = C.INVALID_PAGINATION_PARAMETERS_STR,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                var result = await _DBcontext.Products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (result != null && result.Count > 0)
                {
                    //await _loggerService.LogInfoAsync(C.PRODUCTS_RETRIEVE_SUCCESS_STR, Consts.PRODUCT_RETRIEVE_SUCCESS_DESC_STR);                    
                    return new ServiceResult<List<Product>>
                    {
                        Success = true,
                        Message = C.PRODUCTS_RETRIEVE_SUCCESS_STR,
                        StatusCode = StatusCodes.Status200OK,
                        Data = result
                    };
                }
                else
                {
                    string messageDescribtion = $"{C.NO_PRODUCTS_FOUND_STR}. {C.NO_PRODUCTS_FOUND_DESC_STR}";
                    await _loggerService.LogErrorAsync(string.Empty, messageDescribtion);
                    return new ServiceResult<List<Product>>
                    {
                        Success = false,
                        Message = C.NO_PRODUCTS_FOUND_STR,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = C.NO_PRODUCTS_FOUND_DESC_STR
                    };
                }
            }
            catch (OperationCanceledException e)
            {
                string messageDescribtion = $"{C.DATABASE_CONNECTION_TIMEOUT_STR}. {C.DATABASE_CONNECTION_TIMEOUT_STR}";                
                await _loggerService.LogErrorAsync(e.Message, messageDescribtion);
                return new ServiceResult<List<Product>>
                {
                    Success = false,
                    Message = C.REQUEST_TIME_OUT_STR,
                    StatusCode = StatusCodes.Status408RequestTimeout,
                    Error = C.DATABASE_CONNECTION_TIMEOUT_STR
                };
            }
            catch (Exception e)
            {
                await _loggerService.LogErrorAsync(e.Message, C.FAILED_RETRIEVING_PRODUCTS_DESC_STR);
                return new ServiceResult<List<Product>>
                {
                    Success = false,
                    Message = C.FAILED_RETRIEVING_PRODUCTS_DESC_STR,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = C.INTERNAL_SERVER_ERROR_STR
                };              
            }            
        }
      
        public async Task<ServiceResult<string>> AddProductAsync(string jwt, Product newProduct)
        {
            try
            {
                ServiceResult<string> validationResult = await _jwtAuthService.JwtContentCheck(jwt, C.ADMIN_STR);
                if (!validationResult.Success)
                {
                    await _loggerService.LogWarningAsync($"{validationResult.Message}. {validationResult.Error}. {validationResult.StatusCode}");
                    return new ServiceResult<string>
                    {
                        Success = false,
                        Message = validationResult.Message,
                        Error = validationResult.Error,
                        StatusCode = validationResult.StatusCode
                    };
                }

                var existingProduct = await _DBcontext.Products.FirstOrDefaultAsync(p => p.ProductName == newProduct.ProductName);
                if (existingProduct != null)
                {
                    string friendlyDescribtion = $"{C.PRODUCT_ALREADY_EXISTS_STR}. {C.PRODUCT_ALREADY_EXISTS_DESC_STR}";
                    await _loggerService.LogWarningAsync(friendlyDescribtion);
                    return new ServiceResult<string>
                    {
                        Success = false,
                        Message = C.PRODUCT_ALREADY_EXISTS_STR,
                        StatusCode = StatusCodes.Status409Conflict,
                        Error = C.PRODUCT_ALREADY_EXISTS_DESC_STR
                    };                    
                }

                _DBcontext.Products.Add(newProduct);
                await _DBcontext.SaveChangesAsync();
                //await _loggerService.LogInfoAsync(C.PRODUCT_SUCCESSFULLY_ADDED_STR);
                return new ServiceResult<string>
                {
                    Success = true,
                    Message = C.PRODUCT_SUCCESSFULLY_ADDED_STR,
                    StatusCode = StatusCodes.Status200OK,
                };              
            }            
            catch (OperationCanceledException e)
            {
                string friendlyDescribtion = $"{C.REQUEST_TIME_OUT_STR}. {C.DATABASE_CONNECTION_TIMEOUT_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.REQUEST_TIME_OUT_STR,
                    StatusCode = StatusCodes.Status408RequestTimeout,
                    Error = C.DATABASE_CONNECTION_TIMEOUT_STR
                };
            }
            catch (DbUpdateException e)
            {
                string friendlyDescribtion = $"{C.DATABASE_UPDATE_ERROR_STR}. {C.DATABASE_UPDATE_ERROR_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.DATABASE_UPDATE_ERROR_STR,
                    StatusCode = StatusCodes.Status409Conflict,
                    Error = C.DATABASE_UPDATE_ERROR_DESC_STR
                };
            }
            catch (Exception e)
            {
                await _loggerService.LogErrorAsync(e.Message, C.FAILED_ADDING_PRODUCT_DESC_STR);
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.FAILED_ADDING_PRODUCT_DESC_STR,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = C.INTERNAL_SERVER_ERROR_STR
                };               
            }
        }

     
        public async Task<ServiceResult<Product>> DeleteProductAsync(string jwt, string productName)
        {
            try
            {
                ServiceResult<string> validationResult = await _jwtAuthService.JwtContentCheck(jwt, C.ADMIN_STR);
                if (!validationResult.Success)
                {
                    await _loggerService.LogWarningAsync($"{validationResult.Message}. {validationResult.Error}. {validationResult.StatusCode}");
                    return new ServiceResult<Product>
                    {
                        Success = false,
                        Message = validationResult.Message,
                        Error = validationResult.Error,
                        StatusCode = validationResult.StatusCode
                    };
                }

                var product = await _DBcontext.Products.FirstOrDefaultAsync(p => p.ProductName == productName);
                if (product == null)
                {
                    await _loggerService.LogWarningAsync($"{C.PRODUCT_NOT_FOUND1_STR} {productName} {C.PRODUCT_NOT_FOUND2_STR}.");
                    return new ServiceResult<Product>
                    {
                        Success = false,
                        Message = C.PRODUCT_NOT_FOUND_STR,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = $"{C.PRODUCT_NOT_FOUND1_STR} {productName} {C.PRODUCT_NOT_FOUND2_STR}"
                    };                   
                }

                // Handle concurrency by setting the original RowVersion, removing product, and saving changes
                _DBcontext.Entry(product).Property(p => p.RowVersion).OriginalValue = product.RowVersion;
                _DBcontext.Products.Remove(product);
                await _DBcontext.SaveChangesAsync();
                //await _loggerService.LogInfoAsync(C.PRODUCT_DELETED_SUCCESSFULLY_STR);
                return new ServiceResult<Product>
                {
                    Success = true,
                    Message = C.PRODUCT_DELETED_SUCCESSFULLY_STR,
                    StatusCode = StatusCodes.Status200OK,
                };               
            }
            catch (OperationCanceledException e)
            {
                string friendlyDescribtion = $"{C.REQUEST_TIME_OUT_STR}. {C.DATABASE_CONNECTION_TIMEOUT_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<Product>
                {
                    Success = false,
                    Message = C.REQUEST_TIME_OUT_STR,
                    StatusCode = StatusCodes.Status408RequestTimeout,
                    Error = C.DATABASE_CONNECTION_TIMEOUT_STR
                };
            }
            catch (DbUpdateException e)
            {
                string friendlyDescribtion = $"{C.DATABASE_UPDATE_ERROR_STR}. {C.DATABASE_UPDATE_ERROR_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<Product>
                {
                    Success = false,
                    Message = C.DATABASE_UPDATE_ERROR_STR,
                    StatusCode = StatusCodes.Status409Conflict,
                    Error = C.DATABASE_UPDATE_ERROR_DESC_STR
                };
            }           
            catch (Exception e)
            {
                await _loggerService.LogErrorAsync(e.Message, C.FAILED_DELETING_PRODUCT_DESC_STR);
                return new ServiceResult<Product>
                {
                    Success = false,
                    Message = C.FAILED_DELETING_PRODUCT_DESC_STR,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = C.INTERNAL_SERVER_ERROR_STR
                };                
            }          
        }

        public async Task<ServiceResult<Product>> BuyProductAsync(string jwt, string productName, int quantity)
        {            
            try
            {
                ServiceResult<string> validationResult = await _jwtAuthService.JwtContentCheck(jwt, $"{C.CUSTOMER_STR},{C.ADMIN_STR}");
                if (!validationResult.Success)
                {
                    await _loggerService.LogWarningAsync($"{validationResult.Message}. {validationResult.Error}. {validationResult.StatusCode}");
                    return new ServiceResult<Product>
                    {
                        Message = validationResult.Message,
                        Error = validationResult.Error,
                        StatusCode = validationResult.StatusCode
                    };
                }

                //Client side validation should be fine but it is good to have server side validation as well.
                if (string.IsNullOrEmpty(productName))
                {
                    await _loggerService.LogWarningAsync($"{C.PRODUCT_NOT_FOUND1_STR} {productName} {C.PRODUCT_NOT_FOUND2_STR}");
                    return new ServiceResult<Product>
                    {
                        Success = false,
                        Message = C.PRODUCT_NOT_FOUND_STR,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = $"{C.PRODUCT_NOT_FOUND1_STR} {productName} {C.PRODUCT_NOT_FOUND2_STR}"
                    };
                }
                var product = await _DBcontext.Products
                    .Where(p => p.ProductName == productName)
                    .FirstOrDefaultAsync();
                
                if (product == null)
                {
                    await _loggerService.LogWarningAsync($"{C.PRODUCT_NOT_FOUND1_STR} {productName} {C.PRODUCT_NOT_FOUND2_STR}");
                    return new ServiceResult<Product>
                    {
                        Success = false,
                        Message = C.PRODUCT_NOT_FOUND_STR,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = $"{C.PRODUCT_NOT_FOUND1_STR} {productName} {C.PRODUCT_NOT_FOUND2_STR}"
                    };
                }

                if (product.Quantity < quantity || quantity <= 0 || quantity > C.QUANTITY_MAXIMUM_VALUE)
                {
                    await _loggerService.LogWarningAsync(C.INSUFFICIENT_QUANTITY_STR);
                    return new ServiceResult<Product>
                    {
                        Success = false,
                        Message = C.INSUFFICIENT_QUANTITY_STR,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Error = C.INSUFFICIENT_QUANTITY_DESC_STR
                    };                   
                }
                // Handle concurrency by setting the original RowVersion, and save changes                
                _DBcontext.Entry(product).Property(p => p.RowVersion).OriginalValue = product.RowVersion;
                product.Quantity -= quantity;
                await _DBcontext.SaveChangesAsync();
                //await _loggerService.LogInfoAsync(C.PRODUCT_QUANTITY_SUCCESSFULLY_UPDATED_STR);
                return new ServiceResult<Product>
                {
                    Success = true,
                    Message = C.PRODUCT_QUANTITY_SUCCESSFULLY_UPDATED_STR,
                    StatusCode = StatusCodes.Status200OK
                };              
            }
            catch (OperationCanceledException e)
            {
                string friendlyDescribtion = $"{C.REQUEST_TIME_OUT_STR}. {C.DATABASE_CONNECTION_TIMEOUT_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<Product>
                {
                    Success = false,
                    Message = C.REQUEST_TIME_OUT_STR,
                    StatusCode = StatusCodes.Status408RequestTimeout,
                    Error = C.DATABASE_CONNECTION_TIMEOUT_STR
                };
            }
            catch (DbUpdateConcurrencyException e)
            {
                string friendlyDescribtion = $"{C.CONCURRENCY_ERROR_STR}. {C.CONCURRENCY_ERROR_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<Product>
                {
                    Success = false,
                    Message = C.CONCURRENCY_ERROR_STR,
                    StatusCode = StatusCodes.Status409Conflict,
                    Error = C.CONCURRENCY_ERROR_DESC_STR
                };
            }                       
            catch (Exception e)
            {
                string friendlyDescribtion = $"{C.FAILED_UPDATE_PRODUCT_STR}. {C.FAILED_UPDATE_PRODUCT_DESC_STR}";
                await _loggerService.LogCriticalAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<Product>
                {
                    Success = false,
                    Message = C.FAILED_UPDATE_PRODUCT_DESC_STR,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = C.INTERNAL_SERVER_ERROR_STR
                };
            }         
        }

        public async Task<ServiceResult<List<Product>>> SearchProductAsync(string jwt, string filterText, int? minPrice = null, int? maxPrice = null)
        { 
            try
            {
                ServiceResult<string> validationResult = await _jwtAuthService.JwtContentCheck(jwt, $"{C.CUSTOMER_STR},{C.ADMIN_STR}");
                if (!validationResult.Success)
                {
                    await _loggerService.LogWarningAsync($"{validationResult.Message}. {validationResult.Error}. {validationResult.StatusCode}");
                    return new ServiceResult<List<Product>>
                    {
                        Message = validationResult.Message,
                        Error = validationResult.Error,
                        StatusCode = validationResult.StatusCode
                    };
                }

                // Client sent empty filterText, the code will return all products list.
                if (string.IsNullOrEmpty(filterText))
                {
                    // Sanitize filterText to prevent SQL injection
                    filterText = filterText.Trim();
                    var allProducts = await _DBcontext.Products.ToListAsync();
                    return new ServiceResult<List<Product>>
                    {
                        Success = true,
                        StatusCode = StatusCodes.Status200OK,
                        Data = allProducts
                    };
                }

                var query = _DBcontext.Products.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    // Use parameterized queries with EF.Functions.Like to prevent SQL injection
                    query = query.Where(p => EF.Functions.Like(p.ProductName, $"%{filterText}%"));
                }

                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= maxPrice.Value);
                }

                var filteredProducts = await query.ToListAsync();
                //await _loggerService.LogInfoAsync(C.FILTERED_PRODUCTS_RETRIEVE_SUCCESS_STR);
                return new ServiceResult<List<Product>>
                {
                    Success = true,
                    Message = C.FILTERED_PRODUCTS_RETRIEVE_SUCCESS_STR,
                    StatusCode = StatusCodes.Status200OK,
                    Data = filteredProducts
                };
            }
            catch (ArgumentNullException e)
            {
                string friendlyDescribtion = $"{C.ARGUMENT_NULL_STR}. {C.ARGUMENT_NULL_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<List<Product>>
                {
                    Success = false,
                    Message = C.SOMETHING_WENT_WRONG_STR,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = C.SOMETHING_WENT_WRONG_DESC_STR
                };
            }
            catch (OperationCanceledException e)
            {
                string friendlyDescribtion = $"{C.REQUEST_TIME_OUT_STR}. {C.DATABASE_CONNECTION_TIMEOUT_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<List<Product>>
                {
                    Success = false,
                    Message = C.REQUEST_TIME_OUT_STR,
                    StatusCode = StatusCodes.Status408RequestTimeout,
                    Error = C.ACTION_CONNECTION_TIMEOUT_DESC_STR
                };
            }
            catch (InvalidOperationException e)
            {
                string friendlyDescribtion = $"{C.INVALID_OPERATION_STR}. {C.INVALID_OPERATION_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<List<Product>>
                {
                    Success = false,
                    Message = C.INVALID_OPERATION_STR,
                    StatusCode = StatusCodes.Status409Conflict,
                    Error = C.INVALID_OPERATION_DESC_STR
                };
            }
            catch (Exception e)
            {
                string friendlyDescribtion = $"{C.FAILED_RETRIEVING_PRODUCTS_STR}. {C.FAILED_RETRIEVING_PRODUCTS_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<List<Product>>
                {
                    Success = false,
                    Message = C.FAILED_RETRIEVING_PRODUCTS_DESC_STR,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = C.INTERNAL_SERVER_ERROR_STR
                };
            }
        }
    }    
}