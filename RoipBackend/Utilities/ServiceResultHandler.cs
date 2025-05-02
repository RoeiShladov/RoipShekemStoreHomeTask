using Microsoft.AspNetCore.Mvc;

namespace RoipBackend.Utilities
{
    public static class ServiceResultHandler
    {
        public static ActionResult HandleServiceResult(IActionResult serviceResult)
        {
            if (serviceResult == null)
                return new ObjectResult(new { Message = Consts.INTERNAL_SERVER_ERROR_STR })
                {
                    StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                };

            string? message, error;
            switch (serviceResult)
            {
                case OkObjectResult okResult:
                    var okResponse = okResult.Value as dynamic;
                    message = okResponse?.Message;
                    if (message != null)
                        return new OkObjectResult(new
                        {
                            Message = message,
                            Data = okResult.Value
                        });
                    return new OkObjectResult(new { Data = okResult.Value });

                case BadRequestObjectResult badRequestResult:
                    var badRequestResponse = badRequestResult.Value as dynamic;
                    message = badRequestResponse?.Message;
                    error = badRequestResponse?.Error;
                    if (error != null)
                        return new BadRequestObjectResult(new { Message = message, Error = error });
                    return new BadRequestObjectResult(new { Message = message });

                case NotFoundObjectResult notFoundResult:
                    var notFoundResponse = notFoundResult.Value as dynamic;
                    message = notFoundResponse?.Message;
                    error = notFoundResponse?.Error;
                    if (error != null)
                        return new NotFoundObjectResult(new { Message = message, Error = error });
                    return new NotFoundObjectResult(new { Message = message });

                case UnauthorizedObjectResult unauthorizedResult:
                    var unauthorizedResponse = unauthorizedResult.Value as dynamic;
                    message = unauthorizedResponse?.Message;
                    error = unauthorizedResponse?.Error;
                    if (error != null)
                        return new UnauthorizedObjectResult(new { Message = message, Error = error });
                    return new UnauthorizedObjectResult(new { Message = message });

                default:
                    return new ObjectResult(new { Message = Consts.INTERNAL_SERVER_ERROR_STR })
                    {
                        StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                    };
            }
        }
    }
}

