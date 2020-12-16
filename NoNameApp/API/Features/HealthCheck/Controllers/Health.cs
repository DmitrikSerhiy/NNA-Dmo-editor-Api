using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace API.Features.HealthCheck.Controllers {

    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class Health : ControllerBase {

        private readonly IUserRepository _repository;
        
        public Health(IUserRepository repository) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [Route("")]
        public ActionResult<string> Check() {
            return Ok("App is ok");
        }


        [HttpGet]
        [Route("database")]
        public async Task<ActionResult<string>> CheckDb() {
            var user = await _repository.FirstUser();
            return Ok($"Db is ok. First user: {user.Email}");
        }
    }
}
