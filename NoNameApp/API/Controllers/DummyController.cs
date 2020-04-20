using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API.Controllers {
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class DummyController : ControllerBase {

        private readonly IUserRepository _repository;
        public DummyController(IUserRepository repository) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [Route("")]
        public ActionResult<string> Hello() {
            return Ok("Hello from dummy");
        }


        [HttpGet]
        [Route("firstUserFromdb")]
        public async Task<ActionResult<string>> FirstFromDb() {
            var user = await _repository.FirstUser();
            return Ok(user.Email);
        }
    }
}