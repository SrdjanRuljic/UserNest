using Microsoft.AspNetCore.Http;
using System.Text;

namespace Application.Common.Excentions
{
    public static class ExceptionExcentions
    {
        public static void AddApplicationException(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Exception", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Exception");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddArgumentException(this HttpResponse response, string message)
        {
            response.Headers.Add("Argument-Exception", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
            response.Headers.Add("Access-Control-Expose-Headers", "Argument-Exception");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddNotFoundException(this HttpResponse response, string message)
        {
            response.Headers.Add("Not-Found-Exception", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
            response.Headers.Add("Access-Control-Expose-Headers", "Not-Found-Exception");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddUnauthorizedException(this HttpResponse response, string message)
        {
            response.Headers.Add("Unauthorized-Exception", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
            response.Headers.Add("Access-Control-Expose-Headers", "Unauthorized-Exception");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddForbiddenException(this HttpResponse response, string message)
        {
            response.Headers.Add("Forbidden-Exception", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
            response.Headers.Add("Access-Control-Expose-Headers", "Forbidden-Exception");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}