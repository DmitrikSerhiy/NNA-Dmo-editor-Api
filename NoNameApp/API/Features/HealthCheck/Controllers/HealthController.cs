using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace API.Features.HealthCheck.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase {

        private readonly IUserRepository _repository;
        
        public HealthController(IUserRepository repository) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public ActionResult<string> Check() {
            return Ok("App is ok");
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("database")]
        public async Task<ActionResult<string>> CheckDb() {
            var user = await _repository.FirstUser();
            return Ok($"Db is ok. First user: {user.Email}");
        }
        
        [HttpGet]
        [Authorize]
        [Route("security")]
        public async Task<ActionResult<string>> CheckToken() {
            return Ok("Token is valid");
        }
    }
}
