using Application.Common.Interfaces;
using System.Security.Claims;

namespace WebAPI.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string UserId
        {
            get
            {
                ClaimsIdentity? clams = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;

                string userId = clams?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

                return userId;
            }
        }
    }
}