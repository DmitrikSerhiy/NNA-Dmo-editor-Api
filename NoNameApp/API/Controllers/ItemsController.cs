using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using API.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase {

        private readonly CurrentUserService _currentUserService;
        public ItemsController(CurrentUserService currentUserService) {
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Route("test")]
        public async Task<IActionResult> Test(String data) {
            var user = await _currentUserService.GetAsync();

            return Ok(data);
        }

    }
}