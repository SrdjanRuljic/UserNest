using Application.Common.Pagination.Models;
using Application.Users.Commands.Delete;
using Application.Users.Commands.Insert;
using Application.Users.Commands.Update;
using Application.Users.Queries.GetAll;
using Application.Users.Queries.GetById;
using Application.Users.Queries.ValidatePassword;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        #region [GET]

        /// <summary>
        /// Retrieves a specific user by their ID
        /// </summary>
        /// <param name="id">The unique identifier of the user</param>
        /// <returns>User details</returns>
        /// <response code="200">User found and returned successfully.</response>
        /// <response code="400">Invalid user ID format.</response>
        /// <response code="401">Unauthorized - Invalid or missing JWT token.</response>
        /// <response code="403">Forbidden - Insufficient permissions (Admin role required).</response>
        /// <response code="404">User not found.</response>
        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Get User by ID",
            Description = "Retrieves detailed information about a specific user by their unique identifier. Requires Admin role.",
            OperationId = "GetUserById",
            Tags = new[] { "Users" }
        )]
        [SwaggerResponse(200, "User found", typeof(GetUserByIdDto), contentTypes: new[] { "application/json" })]
        [SwaggerResponse(400, "Bad request - Invalid user ID", typeof(object), contentTypes: new[] { "application/json" })]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing JWT token", typeof(object), contentTypes: new[] { "application/json" })]
        [SwaggerResponse(403, "Forbidden - Admin role required", typeof(object), contentTypes: new[] { "application/json" })]
        [SwaggerResponse(404, "User not found", typeof(object), contentTypes: new[] { "application/json" })]
        public async Task<IActionResult> GetById(
            [FromRoute, SwaggerParameter("User ID", Required = true)] string id)
        {
            var query = new GetUserByIdQuery { Id = id };
            GetUserByIdDto dto = await Sender.Send(query);

            return Ok(dto);
        }

        /// <summary>
        /// Retrieves a paginated list of all users with optional search filtering
        /// </summary>
        /// <param name="query">Pagination and search parameters</param>
        /// <returns>Paginated list of users</returns>
        /// <response code="200">Users retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">Unauthorized - Invalid or missing JWT token.</response>
        /// <response code="403">Forbidden - Insufficient permissions (Admin role required).</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get All Users",
            Description = "Retrieves a paginated list of all users with optional search filtering. Requires Admin role.",
            OperationId = "GetAllUsers",
            Tags = new[] { "Users" }
        )]
        [SwaggerResponse(200, "Users retrieved successfully", typeof(PaginationResultViewModel<GetAllUsersViewModel>), contentTypes: new[] { "application/json" })]
        [SwaggerResponse(400, "Bad request - Invalid pagination parameters", typeof(object), contentTypes: new[] { "application/json" })]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing JWT token", typeof(object), contentTypes: new[] { "application/json" })]
        [SwaggerResponse(403, "Forbidden - Admin role required", typeof(object), contentTypes: new[] { "application/json" })]
        public async Task<IActionResult> GetAll(
            [FromQuery, SwaggerParameter("Pagination and search parameters")] GetAllUsersQuery query)
        {
            PaginationResultViewModel<GetAllUsersViewModel> paginatedUsers = await Sender.Send(query);

            return Ok(paginatedUsers);
        }

        /// <summary>
        /// Validates a password against the current user's password
        /// </summary>
        /// <param name="query">Password validation query</param>
        /// <returns>Boolean indicating if password is valid</returns>
        /// <response code="200">OK - Password validation completed.</response>
        /// <response code="400">BadRequest - Invalid password format.</response>
        /// <response code="401">Unauthorized - Invalid or missing JWT token.</response>
        [HttpGet]
        [Route("validate-password")]
        [SwaggerOperation(
            Summary = "Validate Password",
            Description = "Validates a password against the current authenticated user's password. Requires valid JWT authentication.",
            OperationId = "ValidatePassword",
            Tags = new[] { "Users" }
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "Password validation completed", typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request - Invalid password format")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized - Invalid or missing JWT token")]
        public async Task<IActionResult> ValidatePassword(
            [FromQuery, SwaggerParameter("Password validation query")] ValidatePasswordQuery query)
        {
            bool isValid = await Sender.Send(query);

            return Ok(isValid);
        }

        #endregion [GET]

        #region [POST]

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="command">User creation command with all required fields</param>
        /// <returns>Created user ID</returns>
        /// <response code="201">Created - User created successfully.</response>
        /// <response code="400">BadRequest - Invalid input data or validation errors.</response>
        /// <response code="401">Unauthorized - Invalid or missing JWT token.</response>
        /// <response code="403">Forbidden - Insufficient permissions (Admin role required).</response>
        /// <response code="409">Conflict - User with same username or email already exists.</response>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create User",
            Description = "Creates a new user with the provided information. Requires Admin role.",
            OperationId = "CreateUser",
            Tags = new[] { "Users" }
        )]
        [SwaggerResponse((int)HttpStatusCode.Created, "User created successfully", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request - Invalid input data or validation errors")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized - Invalid or missing JWT token")]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict - User already exists")]
        public async Task<IActionResult> Insert(
            [FromBody, SwaggerRequestBody("User creation data")] InsertUserCommand command)
        {
            string id = await Sender.Send(command);

            return Created(string.Empty, id);
        }

        #endregion [POST]

        #region [PUT]

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">The unique identifier of the user to update</param>
        /// <param name="command">User update command with fields to modify</param>
        /// <returns>Updated user ID</returns>
        /// <response code="200">OK - User updated successfully.</response>
        /// <response code="400">BadRequest - Invalid input data or validation errors.</response>
        /// <response code="401">Unauthorized - Invalid or missing JWT token.</response>
        /// <response code="403">Forbidden - Insufficient permissions (Admin role required).</response>
        /// <response code="404">NotFound - User not found.</response>
        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Update User",
            Description = "Updates an existing user with the provided information. Requires Admin role.",
            OperationId = "UpdateUser",
            Tags = new[] { "Users" }
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "User updated successfully", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request - Invalid input data or validation errors")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized - Invalid or missing JWT token")]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User not found")]
        public async Task<IActionResult> Update(
            [FromRoute, SwaggerParameter("User ID")] string id, 
            [FromBody, SwaggerRequestBody("User update data")] UpdateUserCommand command)
        {
            command.Id = id;

            string result = await Sender.Send(command);

            return Ok(result);
        }

        #endregion [PUT]

        #region [PATCH]

        /// <summary>
        /// Soft deletes a user (marks as deleted)
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">NoContent - User deleted successfully.</response>
        /// <response code="400">BadRequest - Invalid user ID format.</response>
        /// <response code="401">Unauthorized - Invalid or missing JWT token.</response>
        /// <response code="403">Forbidden - Insufficient permissions (Admin role required).</response>
        /// <response code="404">NotFound - User not found.</response>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Delete User",
            Description = "Soft deletes a user by marking them as deleted. Requires Admin role.",
            OperationId = "DeleteUser",
            Tags = new[] { "Users" }
        )]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "User deleted successfully")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request - Invalid user ID")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized - Invalid or missing JWT token")]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User not found")]
        public async Task<IActionResult> Delete(
            [FromRoute, SwaggerParameter("User ID")] string id)
        {
            var command = new DeleteUserCommand(id);
            await Sender.Send(command);

            return NoContent();
        }

        #endregion [PATCH]
    }
}