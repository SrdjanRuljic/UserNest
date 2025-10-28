using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WebAPI.Services
{
    public class LoggingHelperService : ILoggingHelperService
    {
        public string GetClientIpAddress(IHttpContextAccessor httpContextAccessor)
        {
            Microsoft.AspNetCore.Http.HttpContext? httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null) return "Unknown";

            string? forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            string? realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        public string GetClientName(IHttpContextAccessor httpContextAccessor)
        {
            Microsoft.AspNetCore.Http.HttpContext? httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.Name != null)
            {
                return httpContext.User.Identity.Name;
            }

            string? userAgent = httpContext?.Request.Headers["User-Agent"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userAgent))
            {
                return $"User-Agent: {userAgent[..Math.Min(userAgent.Length, 100)]}";
            }

            return "Anonymous";
        }

        public string SerializeRequestParameters<T>(T request)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(request, options);
                json = MaskSensitiveData(json);
                
                return json.Length > 1000 ? json[..1000] + "..." : json;
            }
            catch
            {
                return $"Request type: {typeof(T).Name}";
            }
        }

        private string MaskSensitiveData(string json)
        {
            json = Regex.Replace(json, 
                @"""password""\s*:\s*""[^""]*""", 
                @"""password"":""***""", 
                RegexOptions.IgnoreCase);
            
            json = Regex.Replace(json, 
                @"""token""\s*:\s*""[^""]*""", 
                @"""token"":""***""", 
                RegexOptions.IgnoreCase);
            
            json = Regex.Replace(json, 
                @"""secret""\s*:\s*""[^""]*""", 
                @"""secret"":""***""", 
                RegexOptions.IgnoreCase);
            
            return json;
        }
    }
}
