using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Entities;
using Model.Enums;
using Model.Interfaces.Repositories;

namespace API.Features.Account.Services {
    public sealed class NnaUserManager: UserManager<NnaUser> {

        private readonly IPasswordHasher<NnaUser> _passwordHasher;
        private readonly IUserStore<NnaUser> _userStore;
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
            _passwordHasher = passwordHasher;
            _userStore = store;
            _serviceProvider = services;
            _userRepository = _serviceProvider.GetService<IUserRepository>();
            RegisterTokenProvider(NnaTokenProviderName, GetNnaDataProtectorTokenProvider());
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

        public void UpdateAuthProvider(NnaUser user, LoginProviderName providerName) {
            user.AuthProvider = Enum.GetName(providerName);
            _userRepository.UpdateUser(user);
        }
        
        public bool HasAuthProvider(NnaUser user) {
            return user.AuthProvider != null;
        }
        
        private IUserTwoFactorTokenProvider<NnaUser> GetNnaDataProtectorTokenProvider() {
            return _serviceProvider.GetService<DataProtectorTokenProvider<NnaUser>>();
        }
    }
}