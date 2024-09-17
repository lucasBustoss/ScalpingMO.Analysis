using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ScalpingMO.Analysis.Analysis.API.Models.Login.Request;
using ScalpingMO.Analysis.Analysis.API.Models.Login.Response;
using ScalpingMO.Analysis.Analysis.API.Repositories.Login;

namespace ScalpingMO.Analysis.Analysis.API.Controllers.Users
{
    [Route("api/login")]
    public class LoginController : BaseController
    {
        private readonly ILoginRepository _repository;

        public LoginController(ILoginRepository repository)
        {
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginRequest loginRequest)
        {
            try
            {
                LoginResponse userlogin = _repository.Login(loginRequest);

                return OkResponse(userlogin);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }
    }
}
