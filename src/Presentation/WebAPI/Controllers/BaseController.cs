using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        private ISender _sender = null!;

        protected ISender Sender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
    }
}