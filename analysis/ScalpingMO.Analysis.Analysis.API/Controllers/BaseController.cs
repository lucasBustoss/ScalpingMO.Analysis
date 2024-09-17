using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ScalpingMO.Analysis.Analysis.API.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult OkResponse(object data)
        {
            return Ok(new
            {
                StatusCode = HttpStatusCode.OK,
                Data = data
            });
        }

        protected ActionResult CreatedResponse(string uri, object data)
        {
            return Created(uri, new
            {
                StatusCode = HttpStatusCode.Created,
                Data = data
            });
        }

        protected ActionResult BadRequestResponse(string message)
        {
            return BadRequest(new
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = message ?? "error in request."
            });
        }

        protected ActionResult NotFoundResponse()
        {
            return NotFound(new
            {
                StatusCode = HttpStatusCode.NotFound,
                Error = "resource not found."
            });
        }
    }
}
