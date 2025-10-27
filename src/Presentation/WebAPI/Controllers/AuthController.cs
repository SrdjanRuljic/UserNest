using Application.Auth;
using Application.Auth.Commands.Login;
using Application.Auth.Commands.Logout;
using Application.Auth.Commands.Refresh;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        #region [POST]

        /// <summary>
        /// Authenticates a user and returns JWT tokens
        /// </summary>
        /// <param name="command">Login credentials</param>
        /// <returns>JWT authentication and refresh tokens</returns>
        /// <response code="200">OK - Successfully authenticated. Returns JWT tokens.</response>
        /// <response code="400">BadRequest - Invalid credentials or validation errors.</response>
        /// <response code="401">Unauthorized - Authentication failed.</response>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "User Login",
            Description = "Authenticates a user with username and password, returning JWT authentication and refresh tokens.",
            OperationId = "Login",
            Tags = new[] { "Auth" }
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "Authentication successful", typeof(LoginViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request - Invalid input or validation errors")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized - Invalid credentials")]
        public async Task<IActionResult> Login(
            [FromBody, SwaggerRequestBody("User login credentials")] LoginCommand command)
        {
            LoginViewModel result = await Sender.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Logs out a user by invalidating their refresh token
        /// </summary>
        /// <param name="command">Logout command containing refresh token</param>
        /// <returns>Success confirmation</returns>
        /// <response code="200">OK - Successfully logged out.</response>
        /// <response code="400">BadRequest - Invalid refresh token or validation errors.</response>
        /// <response code="401">Unauthorized - Invalid or expired token.</response>
        [HttpPost]
        [Route("logout")]
        [SwaggerOperation(
            Summary = "User Logout",
            Description = "Logs out a user by invalidating their refresh token. Requires valid JWT authentication.",
            OperationId = "Logout",
            Tags = new[] { "Auth" }
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "Successfully logged out")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request - Invalid refresh token or validation errors")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized - Invalid or expired JWT token")]
        public async Task<IActionResult> Logout(
            [FromBody, SwaggerRequestBody("Logout command with refresh token")] LogoutCommand command)
        {
            await Sender.Send(command);

            return Ok();
        }

        /// <summary>
        /// Refreshes JWT authentication token using a valid refresh token
        /// </summary>
        /// <param name="command">Refresh token command</param>
        /// <returns>New JWT authentication and refresh tokens</returns>
        /// <response code="200">OK - Successfully refreshed tokens.</response>
        /// <response code="400">BadRequest - Invalid refresh token or validation errors.</response>
        /// <response code="401">Unauthorized - Invalid or expired refresh token.</response>
        [HttpPost]
        [Route("refresh-token")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Refresh Token",
            Description = "Refreshes JWT authentication token using a valid refresh token. Returns new authentication and refresh tokens.",
            OperationId = "RefreshToken",
            Tags = new[] { "Auth" }
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "Tokens successfully refreshed", typeof(RefreshViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request - Invalid refresh token or validation errors")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized - Invalid or expired refresh token")]
        public async Task<IActionResult> RefreshToken(
            [FromBody, SwaggerRequestBody("Refresh token command")] RefreshTokenCommand command)
        {
            RefreshViewModel result = await Sender.Send(command);

            return Ok(result);
        }

        #endregion [POST]
    }
}