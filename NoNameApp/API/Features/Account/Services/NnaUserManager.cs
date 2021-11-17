using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Entities;

namespace API.Features.Account.Services {
    public sealed class NnaUserManager: UserManager<NnaUser> {

        private readonly IPasswordHasher<NnaUser> _passwordHasher;
        private readonly IUserStore<NnaUser> _userStore;
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
        }
    }
}