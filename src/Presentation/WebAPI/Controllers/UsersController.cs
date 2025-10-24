using Application.Users.Commands.Insert;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class UsersController : BaseController
    {
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