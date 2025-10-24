using Application.Common.Pagination.Models;
using Application.Users.Commands.Delete;
using Application.Users.Commands.Insert;
using Application.Users.Queries.GetAll;
using Application.Users.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class UsersController : BaseController
    {
        #region [GET]

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            GetUserByIdDto dto = await Sender.Send(new GetUserByIdQuery(id));

            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query)
        {
            PaginationResultViewModel<GetAllUsersViewModel> paginatedUsers = await Sender.Send(query);

            return Ok(paginatedUsers);
        }

        #endregion [GET]

        #region [POST]

        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert(InsertUserCommand command)
        {
            string id = await Sender.Send(command);

            return Created(string.Empty, id);
        }

        #endregion [POST]

        #region [PATCH]

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await Sender.Send(new DeleteUserCommand(id));

            return NoContent();
        }

        #endregion [PATCH]
    }
}