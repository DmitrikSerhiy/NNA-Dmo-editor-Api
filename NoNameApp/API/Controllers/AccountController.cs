using API.Helpers;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Account;
using System;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase {
        private readonly NoNameUserManager _userManager;
        private readonly IdentityService _identityService;

        public AccountController(
            NoNameUserManager userManager, 
            IdentityService identityService) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }


        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel registerModel) {
            if (await _userManager.FindByEmailAsync(registerModel.Email) == null) {
                return Ok(new { errorMessage = "Email is already taken"});
            }

            if (await _userManager.FindByNameAsync(registerModel.UserName) == null) {
                return Ok(new { errorMessage = "Username is already taken" });
            }

            var result = await _userManager
                .CreateAsync(new NoNameUser(registerModel.Email, registerModel.UserName), registerModel.Password);
            if (!result.Succeeded) {
                return StatusCode((Int32) HttpStatusCode.InternalServerError);
            }

            var identity = await _identityService.GetIdentity(registerModel.Email, registerModel.Password);
            var jwt = _identityService.CreateJWT(identity);

            return new JsonResult(new
            {
                accessToken = jwt,
                userName = identity.Name,
                email = registerModel.Email
            });
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel loginModel) {
            var identity = await _identityService.GetIdentity(loginModel.Email, loginModel.Password);
            if (identity == null) {
                return Ok(new {errorMessage = "Invalid email or password."});
            }

            var jwt = _identityService.CreateJWT(identity);

            return new JsonResult(new {
                accessToken = jwt,
                userName = identity.Name,
                email = loginModel.Email
            });
        }
    }
}
