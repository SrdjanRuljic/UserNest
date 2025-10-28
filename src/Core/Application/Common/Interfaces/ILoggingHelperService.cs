using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces
{
    public interface ILoggingHelperService
    {
        string GetClientIpAddress(IHttpContextAccessor httpContextAccessor);
        string GetClientName(IHttpContextAccessor httpContextAccessor);
        string SerializeRequestParameters<T>(T request);
    }
}
