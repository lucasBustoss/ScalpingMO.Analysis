using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScalpingMO.Analysis.Analysis.API.Models.Users;
using ScalpingMO.Analysis.Analysis.API.Models.Users.Request;
using ScalpingMO.Analysis.Analysis.API.Models.Users.Response;
using ScalpingMO.Analysis.Analysis.API.Repositories.Users;

namespace ScalpingMO.Analysis.Analysis.API.Controllers.Users
{
    [Route("api/users")]
    public class UsersController : BaseController
    {
        private readonly IUsersRepository _repository;

        public UsersController(IUsersRepository repository)
        {
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateUser([FromBody] UserCreateRequest userCreateRequest)
        {
            try
            {
                if (!userCreateRequest.IsValid()) return BadRequestResponse(userCreateRequest.Errors.FirstOrDefault());
                if (!userCreateRequest.IsValidPassword()) return BadRequestResponse(userCreateRequest.Errors.FirstOrDefault());
                if (!userCreateRequest.PasswordConfirmed()) return BadRequestResponse(userCreateRequest.Errors.FirstOrDefault());
                if (!userCreateRequest.ExistsUsername(_repository)) return BadRequestResponse(userCreateRequest.Errors.FirstOrDefault());

                UserResponse user = _repository.Create(new UserModel(userCreateRequest));

                return OkResponse(user);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        [HttpGet("findByUsername")]
        public ActionResult GetUserByUsername([FromQuery] string username)
        {
            try
            {
                if (username == "" || username == String.Empty)
                    return BadRequestResponse("username parameter cannot be null");

                UserResponse user = _repository.FindUserByUsername(username);

                return OkResponse(user);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }
    }
}
