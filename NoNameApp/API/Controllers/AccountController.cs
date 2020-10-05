using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using API.Infrastructure.Authentication;
using Model.Account;
using Model.Entities;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase {
        private readonly NoNameUserManager _userManager;
        private readonly IdentityService _identityService;
        private readonly ResponseBuilder _responseBuilder;

        public AccountController(
            NoNameUserManager userManager, 
            IdentityService identityService, 
            ResponseBuilder responseBuilder) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
        }


        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel registerModel) {
            if (await _userManager.FindByEmailAsync(registerModel.Email) != null) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage("Email is already taken"));
            }

            if (await _userManager.FindByNameAsync(registerModel.UserName) != null) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage("Username is already taken"));
            }

            var result = await _userManager
                .CreateAsync(new NoNameUser(registerModel.Email, registerModel.UserName), registerModel.Password);
            if (!result.Succeeded) {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }

            var identity = await _identityService.GetIdentity(registerModel.Email, registerModel.Password);
            var jwt = _identityService.CreateJwt(identity);

            return new JsonResult(new
            {
                accessToken = jwt,
                userName = identity.Name,
                email = registerModel.Email,
            });
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel loginModel) {
            var identity = await _identityService.GetIdentity(loginModel.Email, loginModel.Password);
            if (identity == null) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage("Invalid email or password"));
            }

            var jwt = _identityService.CreateJwt(identity);

            return new JsonResult(new {
                accessToken = jwt,
                userName = identity.Name,
                email = loginModel.Email
            });
        }
    }
}
