using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Interfaces;
using Model.Interfaces.Repositories;

namespace API.Features.Dummy.Controllers {
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
            return Ok("Hello from dummy this is update for continuous deliverance. Update once more");
        }


        [HttpGet]
        [Route("firstUserFromDb")]
        public async Task<ActionResult<string>> FirstFromDb() {
            var user = await _repository.FirstUser();
            return Ok(user.Email);
        }
    }
}