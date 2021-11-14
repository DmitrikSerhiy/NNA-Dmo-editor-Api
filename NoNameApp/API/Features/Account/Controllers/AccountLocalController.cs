using System;
using System.Net;
using System.Threading.Tasks;
using API.Features.Account.Services.Local;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs.Account;
using Model.Entities;

namespace API.Features.Account.Controllers
{
    [Route("api/local/account")]
    [ApiController]
    [Authorize]
    public class AccountLocalController : ControllerBase {
        private readonly NnaLocalUserManager _localUserManager;
        private readonly LocalIdentityService _localIdentityService;
        private readonly ResponseBuilder _responseBuilder;

        public AccountLocalController(
            NnaLocalUserManager localUserManager, 
            LocalIdentityService localIdentityService, 
            ResponseBuilder responseBuilder) {
            _localUserManager = localUserManager ?? throw new ArgumentNullException(nameof(localUserManager));
            _localIdentityService = localIdentityService ?? throw new ArgumentNullException(nameof(localIdentityService));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
        }

        [HttpPost]
        [Route("email")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailExists(CheckEmailDto checkEmailDto) {
            return (await _localUserManager.FindByEmailAsync(checkEmailDto.Email) != null)
                ? Ok(true)
                : Ok(false);
        }

        [HttpPost]
        [Route("name")]
        [AllowAnonymous]
        public async Task<IActionResult> IsNameExists(CheckNameDto checkNameDto) {
            return (await _localUserManager.FindByNameAsync(checkNameDto.Name) != null)
                ? Ok(true)
                : Ok(false);
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto registerDto) {
            if (await _localUserManager.FindByEmailAsync(registerDto.Email) != null) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("Email is already taken"));
            }

            if (await _localUserManager.FindByNameAsync(registerDto.UserName) != null) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("Username is already taken"));
            }

            var result = await _localUserManager
                .CreateAsync(new NnaUser(registerDto.Email, registerDto.UserName), registerDto.Password);
            if (!result.Succeeded) {
                return StatusCode((int) HttpStatusCode.InternalServerError, 
                    _responseBuilder.AppendBadRequestErrorMessage($"Failed to create user with name: {registerDto.UserName} and email: {registerDto.Email}"));
            }
            
            return new JsonResult(new
            {
                accessToken =  await _localIdentityService.CreateJwtAsync(registerDto.Email),
                userName = registerDto.UserName,
                email = registerDto.Email,
            });
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto) {
            var user = await _localUserManager.FindByEmailAsync(loginDto.Email);
            if (user is null) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("User with this email is not found"));
            }

            if (!await _localUserManager.CheckPasswordAsync(user, loginDto.Password)) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("Password is not correct"));
            }
            
            return new JsonResult(new {
                accessToken = await _localIdentityService.CreateJwtAsync(user.Email),
                userName = user.UserName,
                email = user.Email
            });
        }
    }
}
