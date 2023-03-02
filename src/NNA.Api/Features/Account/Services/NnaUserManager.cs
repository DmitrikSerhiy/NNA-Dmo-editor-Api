﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.Account.Services;

public sealed class NnaUserManager : UserManager<NnaUser> {
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserRepository _userRepository;

    private readonly string? _nnaSetPasswordPurpose = Enum.GetName(SendMailReason.NnaSetPassword);

    private readonly string? _nnaResetPasswordPurpose = Enum.GetName(SendMailReason.NnaResetPassword);

    private const string NnaTokenProviderName = "NnaDataProtectorTokenProvider";

    public NnaUserManager(
        IUserStore<NnaUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<NnaUser> passwordHasher,
        IEnumerable<IUserValidator<NnaUser>> userValidators,
        IEnumerable<IPasswordValidator<NnaUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<NnaUserManager> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
            services, logger) {
        _serviceProvider = services;
        _userRepository = _serviceProvider.GetService<IUserRepository>()!;
        RegisterTokenProvider(NnaTokenProviderName, GetNnaTokenProvider());
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

        var isValid = await GetNnaTokenProvider().ValidateAsync(
            Enum.GetName(TokenGenerationReasons.NnaConfirmEmail), token, this, user);

        if (!isValid) {
            return IdentityResult.Failed(new IdentityError
                { Code = "", Description = "Invalid confirm email nna token" });
        }

        _userRepository.ConfirmUserEmail(user);
        return IdentityResult.Success;
    }

    public async Task<string> GenerateNnaUserTokenAsync(NnaUser user, string? reason) {
        return await GenerateUserTokenAsync(user, NnaTokenProviderName, reason);
    }

    public async Task<string> GenerateNnaTokenForSetOrResetPasswordAsync(NnaUser user, SendMailReason reason) {
        return reason switch {
            SendMailReason.NnaSetPassword => await GenerateUserTokenAsync(
                user,
                NnaTokenProviderName,
                _nnaSetPasswordPurpose),
            SendMailReason.NnaResetPassword => await GenerateUserTokenAsync(
                user,
                NnaTokenProviderName,
                _nnaResetPasswordPurpose),
            _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
        };
    }

    public async Task<bool> ValidateNnaTokenForSetOrResetPasswordAsync(NnaUser user, string token,
        SendMailReason reason) {
        return reason switch {
            SendMailReason.NnaSetPassword => await GetNnaTokenProvider()
                .ValidateAsync(_nnaSetPasswordPurpose, token, this, user),
            SendMailReason.NnaResetPassword => await GetNnaTokenProvider()
                .ValidateAsync(_nnaResetPasswordPurpose, token, this, user),
            _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
        };
    }

    public async Task ResetNnaPassword(NnaUser user, string password) {
        await UpdatePasswordHash(user, password, validatePassword: false);
        await UpdateUserAsync(user);
    }

    private EmailTokenProvider<NnaUser> GetNnaTokenProvider() {
        return _serviceProvider.GetService<EmailTokenProvider<NnaUser>>()!;
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