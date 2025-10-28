using Microsoft.AspNetCore.Http;
using System.Text;

namespace Application.Common.Excentions
{
    public static class ExceptionExcentions
    {
        public static void AddApplicationException(this HttpResponse response, string message)
        {
            response.Headers["Application-Exception"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            response.Headers["Access-Control-Expose-Headers"] = "Application-Exception";
            response.Headers["Access-Control-Allow-Origin"] = "*";
        }

        public static void AddArgumentException(this HttpResponse response, string message)
        {
            response.Headers["Argument-Exception"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            response.Headers["Access-Control-Expose-Headers"] = "Argument-Exception";
            response.Headers["Access-Control-Allow-Origin"] = "*";
        }

        public static void AddNotFoundException(this HttpResponse response, string message)
        {
            response.Headers["Not-Found-Exception"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            response.Headers["Access-Control-Expose-Headers"] = "Not-Found-Exception";
            response.Headers["Access-Control-Allow-Origin"] = "*";
        }

        public static void AddUnauthorizedException(this HttpResponse response, string message)
        {
            response.Headers["Unauthorized-Exception"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            response.Headers["Access-Control-Expose-Headers"] = "Unauthorized-Exception";
            response.Headers["Access-Control-Allow-Origin"] = "*";
        }

        public static void AddForbiddenException(this HttpResponse response, string message)
        {
            response.Headers["Forbidden-Exception"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            response.Headers["Access-Control-Expose-Headers"] = "Forbidden-Exception";
            response.Headers["Access-Control-Allow-Origin"] = "*";
        }
    }
}