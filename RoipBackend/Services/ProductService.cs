using RoipBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoipBackend.Interfaces;
using Microsoft.VisualStudio.OLE.Interop;

namespace RoipBackend.Services
{
    
    public class ProductService : IProductService
    {
        //TODO: Add Loggers to all functions
        private readonly AppDbContext _DBcontext;
        private readonly LoggerService _loggerService;

        public ProductService(AppDbContext context, LoggerService loggerService)
        {
            _DBcontext = context;
            _loggerService = loggerService;
            _DBcontext.Database.SetCommandTimeout(Consts.DB_REQUEST_TIMEOUT);
        }

        public async Task<IActionResult> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var result = await _DBcontext.Products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (result != null && result.Count > 0)
                {
                    //await _loggerService.LogInfoAsync(Consts.PRODUCTS_RETRIEVE_SUCCESS_STR, Consts.PRODUCT_RETRIEVE_SUCCESS_DESC_STR);
                    return new OkObjectResult(new { Message = Consts.PRODUCTS_RETRIEVE_SUCCESS_STR, Data = result })
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
                return new ObjectResult(new { Message = Consts.FAILED_RETRIEVING_PRODUCTS_DESC_STR, Error = Consts.INTERNAL_SERVER_ERROR_STR })
                {
                    StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                };
            }
        }
        public async Task<IActionResult> AddProductAsync(Product newProduct)
        {
            try
            {
                var existingProduct = await _DBcontext.Products.FirstOrDefaultAsync(p => p.ProductName == newProduct.ProductName);
                if (existingProduct != null)
                {
                    return new ConflictObjectResult(new
                    {
                        Message = Consts.PRODUCT_ALREADY_EXISTS_STR,
                        Error = Consts.PRODUCT_ALREADY_EXISTS_DESC_STR
                    })
                    {
                        StatusCode = StatusCodes.Status409Conflict
                    };
                }

                // Add the new product and save changes, handle concurrency
                _DBcontext.Products.Add(newProduct);
                await _DBcontext.SaveChangesAsync();

                return new OkObjectResult(new { Message = Consts.PRODUCT_SUCCESSFULLY_ADDED_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (DbUpdateException)
            {
                return new ConflictObjectResult(new
                {
                    Message = Consts.DATABASE_UPDATE_ERROR_STR,
                    Error = Consts.DATABASE_UPDATE_ERROR_DESC_STR
                });
            }
            catch (OperationCanceledException)
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
            catch (Exception)
            {               
                return new ObjectResult(new { Message = Consts.FAILED_ADDING_PRODUCT_DESC_STR, Error = Consts.INTERNAL_SERVER_ERROR_STR })
                {
                    StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                };
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

                // Handle concurrency by setting the original RowVersion, removing product, and saving changes
                _DBcontext.Entry(product).Property(p => p.RowVersion).OriginalValue = product.RowVersion;
                _DBcontext.Products.Remove(product);
                await _DBcontext.SaveChangesAsync();

                return new OkObjectResult(new { Message = Consts.PRODUCT_DELETED_SUCCESSFULLY_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ConflictObjectResult(new
                {
                    Message = Consts.CONCURRENCY_ERROR_STR,
                    Error = Consts.CONCURRENCY_ERROR_DESC_STR
                })
                {
                    StatusCode = StatusCodes.Status409Conflict
                };
            }
            catch (DbUpdateException)
            {
                return new ConflictObjectResult(new { Message = Consts.DATABASE_UPDATE_ERROR_STR, Error = Consts.DATABASE_UPDATE_ERROR_DESC_STR });
            }
            catch (OperationCanceledException)
            {
                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (Exception)
            {
                return new ObjectResult(new { Message = Consts.FAILED_DELETING_PRODUCT_DESC_STR, Error = Consts.INTERNAL_SERVER_ERROR_STR })
                {
                    StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                };
            }
        }

        public async Task<IActionResult> BuyProductAsync(string productName, int quantity)
        {
            //Client side validation should be fine but it is good to have server side validation as well.
            if (string.IsNullOrWhiteSpace(productName))
            {
                return new BadRequestObjectResult(new { Message = Consts.PRODUCT_NOT_FOUND_STR, Error = Consts.PRODUCT_NOT_FOUND_STR });
            }

            if (quantity < 0)
            {
                return new BadRequestObjectResult(new { Message = Consts.INSUFFICIENT_QUANTITY_STR, Eror = Consts.INSUFFICIENT_QUANTITY_STR });
            }

            using var transaction = await _DBcontext.Database.BeginTransactionAsync();
            try
            {
                var product = await _DBcontext.Products
                    .Where(p => p.ProductName == productName)
                    .FirstOrDefaultAsync();

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

                if (product.Quantity < quantity)
                {
                    return new BadRequestObjectResult(new
                    {
                        Message = Consts.INSUFFICIENT_QUANTITY_STR,
                        Error = Consts.INSUFFICIENT_QUANTITY_DESC_STR
                    })
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                product.Quantity -= quantity;

                // Handle concurrency by setting the original RowVersion, and save changes
                _DBcontext.Entry(product).Property(p => p.RowVersion).OriginalValue = product.RowVersion;
                await _DBcontext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OkObjectResult(new { Message = Consts.PRODUCT_QUANTITY_SUCCESSFULLY_UPDATED_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (DbUpdateConcurrencyException e)
            {
                await transaction.RollbackAsync();
                return new ConflictObjectResult(new { Message = Consts.CONCURRENCY_ERROR_STR, Error = Consts.CONCURRENCY_ERROR_DESC_STR });
            }
            catch (DbUpdateException e)
            {
                await transaction.RollbackAsync();
                return new ConflictObjectResult(new { Message = Consts.DATABASE_UPDATE_ERROR_STR, Error = Consts.DATABASE_UPDATE_ERROR_DESC_STR });
            }
            catch (OperationCanceledException e)
            {
                await transaction.RollbackAsync();
                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return new ObjectResult(new { Message = Consts.FAILED_UPDATE_PRODUCT_DESC_STR, Error = Consts.INTERNAL_SERVER_ERROR_STR })
                {
                    StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                };
            }
        }


        public async Task<IActionResult> SearchFilterAsync(string filterText, int? minPrice = null, int? maxPrice = null)
        {
            try
            {                
                var query = _DBcontext.Products.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filterText))
                {
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
                //returned data can be null, the client will handle it and represent a empty list to the user.
                return new OkObjectResult(new
                {
                    Message = Consts.FILTERED_PRODUCTS_RETRIEVE_SUCCESS_STR,
                    Data = filteredProducts
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
                return new ObjectResult(new { Message = Consts.FAILED_RETRIEVING_PRODUCTS_DESC_STR, Error = Consts.INTERNAL_SERVER_ERROR_STR })
                {
                    StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                };                
            }
        }
    }    
}

