using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PulseDelivery.Shared.DTOs;
using System.Text.Json;

namespace PulseDelivery.Shared.Exceptions;

public static class CustomExceptionMiddleware
{
    public static void UseCustomException(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(config =>
        {
            config.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var exceptionFeature =
                    context.Features.Get<IExceptionHandlerFeature>();

                var statusCode = 500;
                context.Response.StatusCode = statusCode;

                var response = ResponseDto<string>.Fail(
                    exceptionFeature?.Error.Message
                    ?? "An unknown error occurred.",
                    statusCode);

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            });
        });
    }
}