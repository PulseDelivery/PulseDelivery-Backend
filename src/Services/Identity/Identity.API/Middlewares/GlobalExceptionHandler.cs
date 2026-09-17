using Identity.API.DTOs;
using Microsoft.AspNetCore.Diagnostics;

namespace Identity.API.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = 500;

        var response = ResponseDto<string>.Fail(exception.Message, 500);

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}