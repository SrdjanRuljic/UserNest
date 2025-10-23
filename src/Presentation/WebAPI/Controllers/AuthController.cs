using Application.Auth;
using Application.Auth.Commands.Login;
using Application.Auth.Commands.Logout;
using Application.Auth.Commands.Refresh;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class AuthController : BaseController
    {
        #region [POST]

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            LoginViewModel result = await Sender.Send(command);

            return Ok(result);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout(LogoutCommand command)
        {
            await Sender.Send(command);

            return Ok();
        }

        [HttpPost]
        [Route("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenCommand command)
        {
            RefreshViewModel result = await Sender.Send(command);

            return Ok(result);
        }

        #endregion [POST]
    }
}