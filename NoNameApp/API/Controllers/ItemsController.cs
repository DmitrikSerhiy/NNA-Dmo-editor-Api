using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase {

        [HttpPost]
        [Route("test")]
        public IActionResult Test(String data) {
            return Ok(data);
        }

    }
}