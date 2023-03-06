using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Account.Services;

public sealed class NnaUserManager : UserManager<NnaUser> {
    private readonly IUserRepository _userRepository;

    private readonly string? _nnaSetPasswordPurpose = Enum.GetName(SendMailReason.NnaSetPassword);

    private readonly string? _nnaResetPasswordPurpose = Enum.GetName(SendMailReason.NnaResetPassword);
    
    public NnaUserManager(
        IUserStore<NnaUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<NnaUser> passwordHasher,
        IEnumerable<IUserValidator<NnaUser>> userValidators,
        IEnumerable<IPasswordValidator<NnaUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<NnaUserManager> logger,
        IUserRepository userRepository)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
            services, logger) {
        _userRepository = userRepository;
    }

    public async Task<NnaUser?> FindByEmailWithRolesAsync(string email) {
        var user = await FindByEmailAsync(email);
        if (user is null) {
            return null;
        }

        var roles = await GetRolesAsync(user);
        if (roles.Count == 0) {
            return user;
        }

        user.Roles = roles;
        return user;
    }

    public override async Task<IdentityResult> ConfirmEmailAsync(NnaUser user, string token) {
        if (user == null) {
            throw new ArgumentNullException(nameof(user));
        }

        var isValid = !await VerifyUserTokenAsync(user, Options.Tokens.EmailConfirmationTokenProvider,
            ConfirmEmailTokenPurpose, token);
            
        if (!isValid) {
            return IdentityResult.Failed(new IdentityError
                { Code = "", Description = "Invalid confirm email nna token" });
        }
        
        user.EmailConfirmed = true;
        user.LastTimeEmailSent = null;
        _userRepository.UpdateUser(user);
        return IdentityResult.Success;
    }
    
    public async Task<string> GenerateNnaTokenForSetOrResetPasswordAsync(NnaUser user, SendMailReason reason) {
        var token = reason switch {
            SendMailReason.NnaSetPassword => await GenerateUserTokenAsync(user, Options.Tokens.PasswordResetTokenProvider, _nnaSetPasswordPurpose),
            SendMailReason.NnaResetPassword => await GenerateUserTokenAsync(user, Options.Tokens.PasswordResetTokenProvider, _nnaResetPasswordPurpose),
            _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
        };
        
        return WebUtility.UrlEncode(token);
    }

    public async Task<bool> ValidateNnaTokenForSetOrResetPasswordAsync(NnaUser user, string token, SendMailReason reason) {
        var isValid = reason switch {
            SendMailReason.NnaSetPassword => await VerifyUserTokenAsync(user, Options.Tokens.PasswordResetTokenProvider, _nnaSetPasswordPurpose, token),
            SendMailReason.NnaResetPassword =>  await VerifyUserTokenAsync(user, Options.Tokens.PasswordResetTokenProvider, _nnaResetPasswordPurpose, token),
            _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
        };

        return isValid;
    }

    public async Task ResetNnaPassword(NnaUser user, string password) {
        await UpdatePasswordHash(user, password, validatePassword: false);
        await UpdateUserAsync(user);
    }

    // pass only user with loaded roles
    public override async Task<IdentityResult> AddToRoleAsync(NnaUser user, string role) {
        var result = await base.AddToRoleAsync(user, role);
        if (result.Succeeded == false) {
            return result;
        }
        
        user.Roles.Add(role);
        return result;
    }
}