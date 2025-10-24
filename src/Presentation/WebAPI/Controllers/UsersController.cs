using Application.Users.Commands.Insert;
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
    }
}