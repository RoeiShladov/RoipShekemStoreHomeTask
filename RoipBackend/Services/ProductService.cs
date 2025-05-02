using RoipBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoipBackend.Interfaces;

namespace RoipBackend.Services
{
    
    public class ProductService : IProductService
    {
        private readonly AppDbContext _DBcontext;
        private readonly LoggerService _loggerService;

        public object Guard { get; }

        public ProductService(AppDbContext context, LoggerService loggerService)
        {
            _DBcontext = context;
            _loggerService = loggerService;
            _DBcontext.Database.SetCommandTimeout(Consts.DB_REQUEST_TIMEOUT);
        }

        public async Task<IActionResult> GetAllProductsAsync()//int pageNumber, int pageSize)
        {
            // Add pagination to this function.
            try
            {
                //var result = await  _DBcontext.Products
                //    .Skip((pageNumber - 1) * pageSize)
                //    .Take(pageSize)
                //    .ToListAsync()
                //);

                var result = await _DBcontext.Products.ToListAsync();

                if (result != null && result.Count > 0)
                {
                    //await _loggerService.LogInfoAsync(Consts.PRODUCTS_RETRIEVE_SUCCESS_STR, Consts.PRODUCT_RETRIEVE_SUCCESS_DESC_STR);
                    return new OkObjectResult(new { Message = Consts.PRODUCTS_RETRIEVE_SUCCESS_STR, Products = result })
                    {
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                else
                {                    
                    return new NotFoundObjectResult(new { Message = Consts.NO_PRODUCTS_FOUND_STR, Error = Consts.NO_PRODUCTS_FOUND_DESC_STR });
                }
            }           
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);

                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (Exception e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.FAILED_RETRIEVING_USERS);
                return new BadRequestObjectResult(new { Message = Consts.FAILED_RETRIEVING_PRODUCTS_STR, Error = Consts.FAILED_RETRIEVING_PRODUCTS_DESC_STR });
            }
        }

        //TODO: Add Authorize check (admin only)
        public async Task<IActionResult> AddProductAsync(Product product)
        {
            try
            {                            
                _DBcontext.Products.Add(product);
                await _DBcontext.SaveChangesAsync();
                return new OkObjectResult(new { Message = Consts.PRODUCT_SUCCESSFULLY_ADDED_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }          
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);

                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (Exception e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.FAILED_ADDING_PRODUCT_STR);
                return new BadRequestObjectResult(new { Message = Consts.FAILED_ADDING_PRODUCT_STR, Error = Consts.FAILED_ADDING_PRODUCT_DESC_STR });
            }
        }

        public async Task<IActionResult> DeleteProductAsync(string productName)
        {
            try
            {
                var product = await _DBcontext.Products.FirstOrDefaultAsync(p => p.ProductName == productName);
                if (product == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        Message = Consts.PRODUCT_NOT_FOUND_STR,
                        Error = $"{Consts.PRODUCT_NOT_FOUND1_STR} {productName} {Consts.PRODUCT_NOT_FOUND2_STR}"
                    })
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                _DBcontext.Products.Remove(product);
                await _DBcontext.SaveChangesAsync();

                return new OkObjectResult(new { Message = Consts.PRODUCT_DELETED_SUCCESSFULLY_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }            
            catch (OperationCanceledException e)
            {
                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Message = Consts.FAILED_DELETING_PRODUCT_STR, Error = Consts.FAILED_DELETING_PRODUCT_DESC_STR });
            }
        }

        public async Task<IActionResult> BuyProductAsync(string productName, int quantity)
        {
            try
            {
                var product = await _DBcontext.Products.FirstOrDefaultAsync(p => p.ProductName == productName);
                if (product == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        Message = Consts.PRODUCT_NOT_FOUND_STR,
                        Error = $"{Consts.PRODUCT_NOT_FOUND1_STR} {productName} {Consts.PRODUCT_NOT_FOUND2_STR}"
                    })
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                else
                {          
                    if (product.Quantity <= quantity)
                    {
                        return new BadRequestObjectResult(new { Message = Consts.INSUFFICIENT_QUANTITY_STR, Error = Consts.INSUFFICIENT_QUANTITY_DESC_STR })
                        {
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                    product.Quantity -= quantity;
                        
                    // Save changes to the database  
                    await _DBcontext.SaveChangesAsync();
                    return new OkObjectResult(new { Message = Consts.PRODUCT_QUANTITY_SUCCESSFULLY_UPDATED_STR })
                    {
                        StatusCode = StatusCodes.Status200OK
                    };                                                                        
                }                    
            }            
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);  

                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (Exception e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.FAILED_LOGIN_STR);  
                return new BadRequestObjectResult(new { Message = Consts.FAILED_UPDATE_PRODUCT_STR, Error = Consts.FAILED_UPDATE_PRODUCT_DESC_STR });
            }
        }

        public async Task<IActionResult> SearchFilterAsync(string filterText)
        {
            try
            {
                // Use EF.Functions.Like for case-insensitive search
                var filteredProducts = await _DBcontext.Products
                    .Where(p => EF.Functions.Like(p.ProductName, $"%{filterText}%"))
                    .ToListAsync();

                if (filteredProducts == null || !filteredProducts.Any())
                {
                    return new NotFoundObjectResult(new
                    {
                        Message = Consts.NO_PRODUCTS_FOUND_STR,
                        Error = Consts.NO_PRODUCTS_FOUND_DESC_STR
                    })
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                return new OkObjectResult(new
                {
                    Message = Consts.FILTERED_PRODUCTS_RETRIEVE_SUCCESS_STR,
                    Products = filteredProducts
                })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (OperationCanceledException e)
            {
                return new ObjectResult(new
                {
                    Message = Consts.REQUEST_TIME_OUT_STR,
                    Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR
                })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (DbUpdateException e)
            {
                return new ObjectResult(new
                {
                    Message = Consts.DATABASE_UPDATE_ERROR_STR,
                    Error = Consts.DATABASE_UPDATE_ERROR_DESC_STR
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            catch (InvalidOperationException e)
            {
                return new ObjectResult(new
                {
                    Message = Consts.INVALID_OPERATION_STR,
                    Error = Consts.INVALID_OPERATION_DESC_STR
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            catch (ArgumentNullException e)
            {
                return new BadRequestObjectResult(new
                {
                    Message = Consts.ARGUMENT_NULL_STR,
                    Error = Consts.ARGUMENT_NULL_DESC_STR
                });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new
                {
                    Message = Consts.FAILED_RETRIEVING_PRODUCTS_STR,
                    Error = Consts.FAILED_RETRIEVING_PRODUCTS_DESC_STR
                });
            }
        }
    }    
}

