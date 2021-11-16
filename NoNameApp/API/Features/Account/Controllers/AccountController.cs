using System;
using System.Net;
using System.Threading.Tasks;
using API.Features.Account.Services;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs.Account;
using Model.Entities;

namespace API.Features.Account.Controllers {
    
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase {
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
        [Route("email")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailExists(CheckEmailDto checkEmailDto) {
            return (await _userManager.FindByEmailAsync(checkEmailDto.Email) != null)
                ? Ok(true)
                : Ok(false);
        }

        [HttpPost]
        [Route("name")]
        [AllowAnonymous]
        public async Task<IActionResult> IsNameExists(CheckNameDto checkNameDto) {
            return (await _userManager.FindByNameAsync(checkNameDto.Name) != null)
                ? Ok(true)
                : Ok(false);
        }
        
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto registerDto) {
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("Email is already taken"));
            }

            if (await _userManager.FindByNameAsync(registerDto.UserName) != null) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("Username is already taken"));
            }

            var result = await _userManager
                .CreateAsync(new NnaUser(registerDto.Email, registerDto.UserName), registerDto.Password);
            if (!result.Succeeded) {
                return StatusCode((int) HttpStatusCode.InternalServerError, 
                    _responseBuilder.AppendBadRequestErrorMessage($"Failed to create user with name: {registerDto.UserName} and email: {registerDto.Email}"));
            }
            
            var tokens = await _identityService.CreateTokensAsync(registerDto.Email);
            return new JsonResult(new {
                accessToken = tokens.AccessToken,
                refreshToken = tokens.RefreshToken,
                userName = registerDto.UserName,
                email = registerDto.Email,
            });
        }
        
        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto) {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("User with this email is not found"));
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("Password is not correct"));
            }

            var tokens = await _identityService.CreateTokensAsync(user.Email);
            return new JsonResult(new {
                accessToken = tokens.AccessToken,
                refreshToken = tokens.RefreshToken,
                userName = user.UserName,
                email = user.Email
            });
        }
    }
}