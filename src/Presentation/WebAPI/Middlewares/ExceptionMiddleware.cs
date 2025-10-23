using Application.Common.Excentions;
using Application.Exceptions;
using System.Net;

namespace WebAPI.Middlewares
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }

    public class ExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            switch (exception)
            {
                case BadRequestException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.AddArgumentException(exception.Message);
                    break;

                case ForbiddenAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.AddForbiddenException(exception.Message);
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.AddUnauthorizedException(exception.Message);
                    break;

                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.AddNotFoundException(exception.Message);
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.AddApplicationException(exception.Message);
                    break;
            }

            await context.Response.WriteAsync(exception.Message);
        }
    }
}