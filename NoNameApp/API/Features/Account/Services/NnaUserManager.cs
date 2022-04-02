using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Entities;
using Model.Enums;
using Model.Extensions;
using Model.Interfaces.Repositories;

namespace API.Features.Account.Services {
    public sealed class NnaUserManager: UserManager<NnaUser> {
        
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserRepository _userRepository; 

        private readonly string _nnaSetPasswordPurpose =
            Enum.GetName(typeof(SendMailReason), SendMailReason.NnaSetPassword);
        private readonly string _nnaResetPasswordPurpose =
            Enum.GetName(typeof(SendMailReason), SendMailReason.NnaResetPassword);

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
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) {
            _serviceProvider = services;
            _userRepository = _serviceProvider.GetService<IUserRepository>();
            RegisterTokenProvider(NnaTokenProviderName, GetNnaDataProtectorTokenProvider());
        }

        public override async Task<IdentityResult> ConfirmEmailAsync(NnaUser user, string token) {
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }

            var isValid = await GetNnaDataProtectorTokenProvider().ValidateAsync(
                Enum.GetName(typeof(TokenGenerationReasons), TokenGenerationReasons.NnaConfirmEmail), token, this, user);

            if (!isValid) {
                return IdentityResult.Failed(new IdentityError { Code = "", Description = "Invalid confirm email nna token" });
            }
            
            _userRepository.ConfirmUserEmail(user);
            return IdentityResult.Success;
        }

        public async Task<string> GenerateNnaUserTokenAsync(NnaUser user, string reason) {
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
        
        public async Task<bool> ValidateNnaTokenForSetOrResetPasswordAsync(NnaUser user, string token, SendMailReason reason) {
            return reason switch {
                SendMailReason.NnaSetPassword => await GetNnaDataProtectorTokenProvider()
                    .ValidateAsync(_nnaSetPasswordPurpose, token, this, user),
                SendMailReason.NnaResetPassword => await GetNnaDataProtectorTokenProvider()
                    .ValidateAsync(_nnaResetPasswordPurpose, token, this, user),
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
            };
        }

        public async Task ResetNnaPassword(NnaUser user, string password) {
            await UpdatePasswordHash(user, password, validatePassword: false); 
            await UpdateUserAsync(user);
        }

        private IUserTwoFactorTokenProvider<NnaUser> GetNnaDataProtectorTokenProvider() {
            return _serviceProvider.GetService<DataProtectorTokenProvider<NnaUser>>();
        }
    }
}