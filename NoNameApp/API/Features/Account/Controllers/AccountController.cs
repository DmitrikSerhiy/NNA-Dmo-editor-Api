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
        private readonly NnaTokenManager _nnaTokenManager;
        private readonly ResponseBuilder _responseBuilder;

        public AccountController(
            NnaUserManager userManager,
            NnaTokenManager nnaTokenManager, 
            ResponseBuilder responseBuilder) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _nnaTokenManager = nnaTokenManager ?? throw new ArgumentNullException(nameof(nnaTokenManager));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
        }
        
        [HttpPost]
        [Route("email")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailExists(CheckEmailDto checkEmailDto) {
            if (checkEmailDto is null) return BadRequest();
            return (await _userManager.FindByEmailAsync(checkEmailDto.Email) != null)
                ? Ok(true)
                : Ok(false);
        }

        [HttpPost]
        [Route("name")]
        [AllowAnonymous]
        public async Task<IActionResult> IsNameExists(CheckNameDto checkNameDto) {
            if (checkNameDto is null) return BadRequest();
            return (await _userManager.FindByNameAsync(checkNameDto.Name) != null)
                ? Ok(true)
                : Ok(false);
        }
        
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto registerDto) {
            if (registerDto is null) return BadRequest();
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
            
            var tokens = await _nnaTokenManager.CreateTokens(registerDto.Email);
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
            if (loginDto is null) return BadRequest();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("User with this email is not found"));
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) {
                return StatusCode((int) HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendBadRequestErrorMessage("Password is not correct"));
            }

            var tokens = await _nnaTokenManager.GetOrCreateTokensAsync(user);
            return new JsonResult(new {
                accessToken = tokens.AccessToken,
                refreshToken = tokens.RefreshToken,
                userName = user.UserName,
                email = user.Email
            });
        }
        
        [HttpGet]
        [Route("verify")]
        [Authorize]
        public async Task<IActionResult> ValidateToken() {
            var isVerified = await _nnaTokenManager.VerifyTokenAsync();
            return isVerified
                ? NoContent()
                : Unauthorized();
        }
        

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(RefreshDto refreshDto) {
            if (refreshDto is null) return BadRequest();
            var tokensDto = await _nnaTokenManager.RefreshTokens(refreshDto);

            if (tokensDto is null) {
                HttpContext.Response.Headers.Add(NnaHeaders.Get(NnaHeaderNames.RedirectToLogin));
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return new JsonResult(new {
                accessToken = tokensDto.AccessToken,
                refreshToken = tokensDto.RefreshToken
            });
        }
        
        [HttpDelete]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(LogoutDto logoutDto) {
            if (logoutDto is null) return BadRequest();
            
            await _nnaTokenManager.ClearTokens(logoutDto.Email);

            return NoContent();
        }
        
        // todo: check whether user with password uses google and vise versa
        [HttpPost]
        [Route("google")]
        [AllowAnonymous]
        public async Task<IActionResult> Google(AuthGoogleDto authGoogleDto) {
            if (authGoogleDto is null) return BadRequest();
            
            var isValid = await _nnaTokenManager.ValidateGoogleTokenAsync(authGoogleDto);
            if (!isValid) {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            var user = await _userManager.FindByEmailAsync(authGoogleDto.Email);
            if (user is not null) {
                // todo: add login provider. [AspNetUserTokens]
                var tokens = await _nnaTokenManager.GetOrCreateTokensAsync(user);
                return new JsonResult(new {
                    accessToken = tokens.AccessToken,
                    refreshToken = tokens.RefreshToken,
                    userName = user.UserName,
                    email = user.Email
                });            
            }
            
            var userWithTakenName = await _userManager.FindByNameAsync(authGoogleDto.Name);
            if (userWithTakenName is not null) {
                authGoogleDto.Name = authGoogleDto.Email;
            }

            var result = await _userManager.CreateAsync(new NnaUser(authGoogleDto.Email, authGoogleDto.Name));
            if (!result.Succeeded) {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            
            var newUser = await _userManager.FindByEmailAsync(authGoogleDto.Email);
            
            // todo: add login provider. [AspNetUserTokens]
            var tokensForNewUser = await _nnaTokenManager.CreateTokens(newUser.Email);
            return new JsonResult(new {
                accessToken = tokensForNewUser.AccessToken,
                refreshToken = tokensForNewUser.RefreshToken,
                userName = newUser.UserName,
                email = newUser.Email,
            });
        }
    }
}