using System;
using System.Net;
using System.Threading.Tasks;
using API.Features.Account.Services;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs.Account;
using Model.Entities;
using Model.Enums;
using Model.Interfaces;

namespace API.Features.Account.Controllers {
    
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase {
        private readonly NnaUserManager _userManager;
        private readonly NnaTokenManager _nnaTokenManager;
        private readonly ResponseBuilder _responseBuilder;
        private readonly MailService _mailService;
        private readonly IAuthenticatedIdentityProvider _identityProvider;

        public AccountController(
            NnaUserManager userManager,
            NnaTokenManager nnaTokenManager, 
            ResponseBuilder responseBuilder, 
            MailService mailService,
            IAuthenticatedIdentityProvider identityProvider) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _nnaTokenManager = nnaTokenManager ?? throw new ArgumentNullException(nameof(nnaTokenManager));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _identityProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
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
        [Route("authprovider")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuthProviderForPasswordlessEmail(SsoCheckDto ssoCheckDto) {
            var user = await _userManager.FindByEmailAsync(ssoCheckDto.Email);
            if (user is null) {
                return NoContent();
            }
            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (user.AuthProvider != null && hasPassword == false) {
                return new JsonResult(user.AuthProvider); // todo: change it to array. Maybe save it as string[] withing one column
            }

            return NoContent();
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
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessageToForm("Email is already taken"));
            }

            if (await _userManager.FindByNameAsync(registerDto.UserName) != null) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessageToForm("Username is already taken"));
            }

            var result = await _userManager
                .CreateAsync(new NnaUser(registerDto.Email, registerDto.UserName), registerDto.Password);
            if (!result.Succeeded) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessage($"Failed to create account with name: {registerDto.UserName} and email: {registerDto.Email}"));
            }

            var newlyCreatedUser = await _userManager.FindByEmailAsync(registerDto.Email);
            await _mailService.SendConfirmAccountEmailAsync(newlyCreatedUser);
            var tokens = await _nnaTokenManager.CreateTokensAsync(newlyCreatedUser);
            
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
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessageToForm("User with this email is not found"));
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessageToForm("Password is not correct"));
            }

            var tokens = await _nnaTokenManager.GetOrCreateTokensAsync(user);
            return new JsonResult(new {
                accessToken = tokens.AccessToken,
                refreshToken = tokens.RefreshToken,
                userName = user.UserName,
                email = user.Email
            });
        }
        
    

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(RefreshDto refreshDto) {
            var tokensDto = await _nnaTokenManager.RefreshTokensAsync(refreshDto);

            if (tokensDto is null) {
                HttpContext.Response.Headers.Add(NnaHeaders.Get(NnaHeaderNames.RedirectToLogin));
                return Unauthorized();
            }

            return new JsonResult(new {
                accessToken = tokensDto.AccessToken,
                refreshToken = tokensDto.RefreshToken
            });
        }

        /// <summary>
        /// Validate google trusted jwt tokens.
        /// If user with such email exists then login
        /// otherwise create new.
        /// </summary>
        /// <param name="authGoogleDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("google")]
        [AllowAnonymous]
        public async Task<IActionResult> Google(AuthGoogleDto authGoogleDto) {
            var isValid = await _nnaTokenManager.ValidateGoogleTokenAsync(authGoogleDto);
            if (!isValid) {
                return StatusCode((int)HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendValidationErrorMessage(nameof(AuthGoogleDto.GoogleToken), "Token is invalid"));
            }

            var user = await _userManager.FindByEmailAsync(authGoogleDto.Email);
            if (user is not null) {
                var tokens = await _nnaTokenManager.GetOrCreateTokensAsync(user, LoginProviderName.google);
                if (!_userManager.HasAuthProvider(user)) {
                    _userManager.UpdateAuthProvider(user, LoginProviderName.google);
                }

                return new JsonResult(new {
                    accessToken = tokens.AccessToken,
                    refreshToken = tokens.RefreshToken,
                    userName = user.UserName,
                    email = user.Email
                });            
            }
            
            var userWithTakenName = await _userManager.FindByNameAsync(authGoogleDto.Name);
            if (userWithTakenName is not null) {
                var isNameUnique = false;
                string newName;
                do {
                    newName = $"{authGoogleDto.Name}{new Random().Next(1000, 9999)}";
                    var nnaUser = await _userManager.FindByNameAsync(newName);
                    if (nnaUser is null) {
                        isNameUnique = true;
                    }
                } while (!isNameUnique);

                authGoogleDto.Name = newName;
            }

            var result = await _userManager.CreateAsync(
                new NnaUser(authGoogleDto.Email, authGoogleDto.Name) {
                    AuthProvider = Enum.GetName(LoginProviderName.google)
                });
            
            
            if (!result.Succeeded) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder
                        .AppendBadRequestErrorMessage($"Failed to create account from Google data with name: {authGoogleDto.Name} and email: {authGoogleDto.Email}"));
            }
            
            var newUser = await _userManager.FindByEmailAsync(authGoogleDto.Email);
            await _mailService.SendConfirmAccountEmailAsync(newUser);
            var tokensForNewUser = await _nnaTokenManager.CreateTokensAsync(newUser, LoginProviderName.google);
            
            return new JsonResult(new {
                accessToken = tokensForNewUser.AccessToken,
                refreshToken = tokensForNewUser.RefreshToken,
                userName = newUser.UserName,
                email = newUser.Email,
            });
        }
        
        
        [HttpPost]
        [Route("mail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMail(SendMailDto update) {
            var user = await _userManager.FindByEmailAsync(update.Email);
            if (user is null) {
                return Ok(false);
            }

            return await _mailService.SendSetOrResetPasswordEmailAsync(user, update.Reason)
                ? Ok(true)
                : Ok(false);
        }

        [HttpPost]
        [Route("validate/tokenFromMail")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateTokenForPasswordChange(ValidateNnaTokenForSetOrResetPasswordDto nnaTokenDto) {
            var user = await _userManager.FindByEmailAsync(nnaTokenDto.Email);
            if (user is null) {
                return new JsonResult(new { valid = false });
            }

            return new JsonResult(
                new {
                    valid = await _userManager
                        .ValidateNnaTokenForSetOrResetPasswordAsync(user, nnaTokenDto.Token, nnaTokenDto.Reason)
                });
        }
        
        [HttpPost]
        [Route("password")]
        [AllowAnonymous]
        public async Task<IActionResult> SetOrResetPassword(SetOrResetPasswordDto newPasswordDto) {
            var user = await _userManager.FindByEmailAsync(newPasswordDto.Email);
            if (user is null) {
                return BadRequest();
            }

            var isTokenValid = await _userManager
                .ValidateNnaTokenForSetOrResetPasswordAsync(user, newPasswordDto.Token, newPasswordDto.Reason);

            if (!isTokenValid) {
                return StatusCode((int)HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendValidationErrorMessage(nameof(SetOrResetPasswordDto.Token), "Token is invalid"));
            }

            switch (newPasswordDto.Reason) {
                case SendMailReason.NnaSetPassword: {
                    if (await _userManager.HasPasswordAsync(user)) {
                        await _userManager.ResetNnaPassword(user, newPasswordDto.NewPassword);
                    } else {
                        await _userManager.AddPasswordAsync(user, newPasswordDto.NewPassword);
                    }
                    break;
                }
                case SendMailReason.NnaResetPassword:
                    await _userManager.ResetNnaPassword(user, newPasswordDto.NewPassword);
                    break;
                default:
                    return BadRequest();
            }
            return NoContent();
        }


        
        
        #region Authorized end-points
        
        [HttpGet]
        [Route("personalInfo")]
        [Authorize]
        public async Task<ActionResult<PersonalInfoDto>> GetPersonalInfo() {
            var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
            if (user is null) {
                return NoContent();
            }

            return new ObjectResult(new PersonalInfoDto{
                AuthProviders = user.AuthProvider is null ? Array.Empty<string>() : new []{ user.AuthProvider },
                UserEmail = user.Email,
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                IsEmailVerified = user.EmailConfirmed,
                HasPassword = !string.IsNullOrEmpty(user.PasswordHash)
            });
        }
        
        [HttpPost]
        [Route("mail/confirmation")]
        [Authorize]
        public async Task<IActionResult> SendEmailForAccountConfirmation(SendMailDto update) {
            var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
            if (user.Email != update.Email) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessage("Wrong email address."));
            }

            await _mailService.SendConfirmAccountEmailAsync(user);
            return NoContent();
        }
        
        [HttpPut]
        [Route("name")]
        [Authorize]
        public async Task<IActionResult> UpdateUserName(UpdateUserNameDto updateUserNameDto) {
            var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
            if (user.Email != updateUserNameDto.Email) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessage("Wrong email address"));
            }
            
            var userWithNewName = await _userManager.FindByNameAsync(updateUserNameDto.UserName);
            if (userWithNewName != null) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessageToForm("User with such name is already registered"));
            }

            await _userManager.SetUserNameAsync(user, updateUserNameDto.UserName);
            return NoContent();
        }

        [HttpPut]
        [Route("password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(ChangePasswordDto changePasswordDto) {
            var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
            if (user.Email != changePasswordDto.Email) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessage("Wrong email address"));
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
            if (!isPasswordValid) {
                return StatusCode((int)HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendValidationErrorMessage( "Current password","Current password is wrong")); 
            }

            if (changePasswordDto.CurrentPassword == changePasswordDto.NewPassword) {
                return StatusCode((int)HttpStatusCode.UnprocessableEntity, 
                    _responseBuilder.AppendValidationErrorMessage("Current password & New password","New password and your current password must be different")); 
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessage("Failed to change password"));
            }
            return NoContent();
        }
        
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto changePasswordDto) {
            var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
            if (user.Email != changePasswordDto.Email) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessage("Wrong email address"));
            }

            if (await _userManager.IsEmailConfirmedAsync(user)) {
                return NoContent();
            }
            
            var isEmailConfirmed = await _userManager.ConfirmEmailAsync(user, changePasswordDto.Token);
            if (!isEmailConfirmed.Succeeded) {
                return StatusCode((int) HttpStatusCode.BadRequest, 
                    _responseBuilder.AppendBadRequestErrorMessageToForm("Failed to confirm email. Try again."));
            }
            return NoContent();
        }

        [HttpGet]
        [Route("ping")]
        [Authorize]
        public async Task<IActionResult> ValidateToken() {
            var isVerified = await _nnaTokenManager.VerifyTokenAsync();
            return isVerified
                ? NoContent()
                : Unauthorized();
        }
        
        [HttpDelete]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(LogoutDto logoutDto) {
            var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);

            if (user.Email != logoutDto.Email) {
                if (user.Email != logoutDto.Email) {
                    return StatusCode((int) HttpStatusCode.BadRequest, 
                        _responseBuilder.AppendBadRequestErrorMessage("Wrong email address"));
                }
            }
            await _nnaTokenManager.ClearTokensAsync(user);
            return NoContent();
        }
        
        #endregion
    }
}