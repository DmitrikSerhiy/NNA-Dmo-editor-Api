using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Features.Account.Services;
using NNA.Api.Helpers;
using NNA.Domain.DTOs.Account;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Extensions;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Account.Controllers; 

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController: NnaController {
    private readonly NnaUserManager _userManager;
    private readonly NnaTokenManager _nnaTokenManager;
    private readonly MailService _mailService;
    private readonly IAuthenticatedIdentityProvider _identityProvider;
    private readonly IUserRepository _userRepository;

    public AccountController(
        NnaUserManager userManager,
        NnaTokenManager nnaTokenManager, 
        MailService mailService,
        IAuthenticatedIdentityProvider identityProvider, 
        IUserRepository userRepository) {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _nnaTokenManager = nnaTokenManager ?? throw new ArgumentNullException(nameof(nnaTokenManager));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _identityProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    [HttpPost]
    [Route("email")]
    [AllowAnonymous]
    public async Task<IActionResult> IsEmailExists(CheckEmailDto checkEmailDto) {
        return await _userManager.FindByEmailAsync(checkEmailDto.Email) != null
            ? OkWithData(true)
            : OkWithData(false);
    }
        
    [HttpPost]
    [Route("name")]
    [AllowAnonymous]
    public async Task<IActionResult> IsNameExists(CheckNameDto checkNameDto) {
        return await _userManager.FindByNameAsync(checkNameDto.Name) != null
            ? OkWithData(true)
            : OkWithData(false);
    }
        
    [HttpGet]
    [Route("authproviders")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAuthProviderForPasswordlessEmail([FromQuery]SsoCheckDto ssoCheckDto) {
        var user = await _userManager.FindByEmailAsync(ssoCheckDto.Email);
        if (user == null) {
            return NoContent();
        }
        var hasPassword = await _userManager.HasPasswordAsync(user);

        if (user.AuthProviders != null && hasPassword == false) {
            return OkWithData(user.GetAuthProviders());
        }

        return NoContent();
    }

    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto registerDto) {
        if (await _userManager.FindByEmailAsync(registerDto.Email) != null) {
            return BadRequestWithMessageForUi("Email is already taken");
        }

        if (await _userManager.FindByNameAsync(registerDto.UserName) != null) {
            return BadRequestWithMessageForUi("Username is already taken");
        }

        var result = await _userManager.CreateAsync(new NnaUser(registerDto.Email, registerDto.UserName), registerDto.Password);
        if (!result.Succeeded) {
            return BadRequestWithMessageForUi("Failed to create account");
        }

        var newlyCreatedUser = await _userManager.FindByEmailAsync(registerDto.Email);
        await _mailService.SendConfirmAccountEmailAsync(newlyCreatedUser);
        var tokens = await _nnaTokenManager.CreateTokensAsync(newlyCreatedUser);

        return OkWithData(new {
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
        if (user == null) {
            return BadRequestWithMessageForUi("User with this email is not found");
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) {
            return BadRequestWithMessageForUi("Password is not correct");
        }

        var tokens = await _nnaTokenManager.GetOrCreateTokensAsync(user);
        return OkWithData(new {
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

        if (tokensDto == null) {
            HttpContext.Response.Headers.Add(NnaHeaders.Get(NnaHeaderNames.RedirectToLogin));
            return Unauthorized();
        }

        return OkWithData(new {
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
            return InvalidRequestWithValidationMessagesToToastr(nameof(AuthGoogleDto.GoogleToken), "Token is invalid");
        }

        var user = await _userManager.FindByEmailAsync(authGoogleDto.Email);
        if (user != null) {
            var tokens = await _nnaTokenManager.GetOrCreateTokensAsync(user, LoginProviderName.google);
            user.AddAuthProvider(Enum.GetName(LoginProviderName.google));
                
            return OkWithData(new {
                accessToken = tokens.AccessToken,
                refreshToken = tokens.RefreshToken,
                userName = user.UserName,
                email = user.Email
            });            
        }
            
        var userWithTakenName = await _userManager.FindByNameAsync(authGoogleDto.Name);
        if (userWithTakenName != null) {
            var isNameUnique = false;
            string newName;
            do {
                newName = $"{authGoogleDto.Name}{new Random().Next(1000, 9999)}";
                var nnaUser = await _userManager.FindByNameAsync(newName);
                if (nnaUser == null) {
                    isNameUnique = true;
                }
            } while (!isNameUnique);

            authGoogleDto.Name = newName;
        }

        var googleUser = new NnaUser(authGoogleDto.Email, authGoogleDto.Name);
        googleUser.AddAuthProvider(Enum.GetName(LoginProviderName.google));
        var result = await _userManager.CreateAsync(googleUser);

        if (!result.Succeeded) {
            return BadRequestWithMessageToToastr("Failed to create account with Google data");
        }
            
        var newUser = await _userManager.FindByEmailAsync(authGoogleDto.Email);
        await _mailService.SendConfirmAccountEmailAsync(newUser);
        var tokensForNewUser = await _nnaTokenManager.CreateTokensAsync(newUser, LoginProviderName.google);

        return OkWithData(new {
            accessToken = tokensForNewUser.AccessToken,
            refreshToken = tokensForNewUser.RefreshToken,
            userName = newUser.UserName,
            email = newUser.Email,
        });
    }
        
    [HttpPost]
    [Route("mail/password")]
    [AllowAnonymous]
    public async Task<IActionResult> SendMail(SendMailDto update) {
        var user = await _userManager.FindByEmailAsync(update.Email);
        if (user == null) {
            return OkWithData(false);
        }

        return await _mailService.SendSetOrResetPasswordEmailAsync(user, update.Reason)
            ? OkWithData(true)
            : OkWithData(false);
    }

    [HttpPost]
    [Route("validate/tokenFromMail")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateTokenForPasswordChange(ValidateNnaTokenForSetOrResetPasswordDto nnaTokenDto) {
        var user = await _userManager.FindByEmailAsync(nnaTokenDto.Email);
        if (user == null) {
            return OkWithData(
                new { valid = false }
            );
        }

        return OkWithData(
            new { valid = await _userManager.ValidateNnaTokenForSetOrResetPasswordAsync(user, nnaTokenDto.Token, nnaTokenDto.Reason) }
        );
    }
        
    [HttpPost]
    [Route("password")]
    [AllowAnonymous]
    public async Task<IActionResult> SetOrResetPassword(SetOrResetPasswordDto newPasswordDto) {
        var user = await _userManager.FindByEmailAsync(newPasswordDto.Email);
        if (user == null) {
            return BadRequestWithMessageToToastr("User is not found");
        }

        var isTokenValid = await _userManager
            .ValidateNnaTokenForSetOrResetPasswordAsync(user, newPasswordDto.Token, newPasswordDto.Reason);

        if (!isTokenValid) {
            return BadRequestWithMessageToToastr("Token is invalid");
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
                return BadRequestWithMessageToToastr("Reason is invalid");
        }
        return NoContent();
    }


        
        
    #region Authorized end-points
        
    [HttpGet]
    [Route("personalInfo")]
    public async Task<IActionResult> GetPersonalInfo() {
        var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
        if (user == null) {
            return NoContent();
        }

        return OkWithData(new PersonalInfoDto(user.UserName, user.Email, user.Id.ToString(), user.GetAuthProviders())
        {
            IsEmailVerified = user.EmailConfirmed,
            HasPassword = !string.IsNullOrEmpty(user.PasswordHash)
        });
    }
        
    [HttpPost]
    [Route("mail/confirmation")]
    public async Task<IActionResult> SendEmailForAccountConfirmation(SendMailDto update) {
        var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
        if (user.Email != update.Email) {
            return BadRequestWithMessageToToastr("Wrong email address");
        }

        await _mailService.SendConfirmAccountEmailAsync(user);
        return NoContent();
    }
        
    [HttpPut]
    [Route("name")]
    public async Task<IActionResult> UpdateUserName(UpdateUserNameDto updateUserNameDto) {
        var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
        if (user.Email != updateUserNameDto.Email) {
            return BadRequestWithMessageToToastr("Wrong email address");
        }
            
        var userWithNewName = await _userManager.FindByNameAsync(updateUserNameDto.UserName);
        if (userWithNewName != null) {
            return BadRequestWithMessageForUi("User with such name is already registered");
        }

        await _userManager.SetUserNameAsync(user, updateUserNameDto.UserName);
        return NoContent();
    }

    [HttpPut]
    [Route("password")]
    public async Task<IActionResult> UpdatePassword(ChangePasswordDto changePasswordDto) {
        var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
        if (user.Email != changePasswordDto.Email) {
            return BadRequestWithMessageToToastr("Wrong email address");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
        if (!isPasswordValid) {
            return InvalidRequestWithValidationMessagesToToastr("Current password","Current password is wrong"); 
        }

        if (changePasswordDto.CurrentPassword == changePasswordDto.NewPassword) {
            return InvalidRequestWithValidationMessagesToToastr("Current password & New password","New password and your current password must be different"); 
        }

        var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded) {
            return BadRequestWithMessageToToastr("Failed to change password");
        }
        return NoContent();
    }
        
    [HttpPost]
    [Route("confirmation")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto changePasswordDto) {
        var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
        if (user.Email != changePasswordDto.Email) {
            return BadRequestWithMessageToToastr("Wrong email address");
        }

        if (await _userManager.IsEmailConfirmedAsync(user)) {
            return NoContent();
        }
            
        var isEmailConfirmed = await _userManager.ConfirmEmailAsync(user, changePasswordDto.Token);
        if (!isEmailConfirmed.Succeeded) {
            return BadRequestWithMessageForUi("Failed to confirm email");
        }
        return NoContent();
    }

    [HttpGet]
    [Route("ping")]
    public async Task<IActionResult> ValidateToken() {
        var isVerified = await _nnaTokenManager.VerifyTokenAsync();
        return isVerified
            ? NoContent()
            : Unauthorized();
    }
        
    [HttpDelete]
    [Route("logout")]
    public async Task<IActionResult> Logout(LogoutDto logoutDto) {
        var user = await _userManager.FindByEmailAsync(_identityProvider.AuthenticatedUserEmail);
        if (user.Email != logoutDto.Email) {
            return BadRequestWithMessageToToastr("Wrong email address");
        }
            
        await _nnaTokenManager.ClearTokensAsync(user);
        await _userRepository.RemoveEditorConnectionOnLogout(user.Id);
        return NoContent();
    }
        
    #endregion
}