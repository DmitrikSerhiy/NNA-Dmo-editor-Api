using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class DummyController : ControllerBase {

        [HttpGet]
        [Route("")]
        public ActionResult<string> Hello() {
            return Ok("Hello from dummy");
        }
    }
}