using System;
using System.Net;
using System.Threading.Tasks;
using API.Features.Account.Services;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs.Account;
using Model.Entities;


namespace API.Features.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase {
        private readonly NnaUserManager _userManager;
        private readonly IdentityService _identityService;
        private readonly ResponseBuilder _responseBuilder;

        public AccountController(
            NnaUserManager userManager, 
            IdentityService identityService, 
            ResponseBuilder responseBuilder) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
        }


        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto registerDto) {
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage("Email is already taken"));
            }

            if (await _userManager.FindByNameAsync(registerDto.UserName) != null) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage("Username is already taken"));
            }

            var result = await _userManager
                .CreateAsync(new NnaUser(registerDto.Email, registerDto.UserName), registerDto.Password);
            if (!result.Succeeded) {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }

            var identity = await _identityService.GetIdentity(registerDto.Email, registerDto.Password);
            var jwt = _identityService.CreateJwt(identity);

            return new JsonResult(new
            {
                accessToken = jwt,
                userName = identity.Name,
                email = registerDto.Email,
            });
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto) {
            var identity = await _identityService.GetIdentity(loginDto.Email, loginDto.Password);
            if (identity == null) {
                return BadRequest(_responseBuilder.AppendBadRequestErrorMessage("Invalid email or password"));
            }

            var jwt = _identityService.CreateJwt(identity);

            return new JsonResult(new {
                accessToken = jwt,
                userName = identity.Name,
                email = loginDto.Email
            });
        }
    }
}
