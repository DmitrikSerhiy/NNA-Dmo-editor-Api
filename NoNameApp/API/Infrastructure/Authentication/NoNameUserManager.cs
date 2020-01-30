using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Entities;

namespace API.Infrastructure.Authentication {
    public class NoNameUserManager : UserManager<NoNameUser> {
        public NoNameUserManager(
            IUserStore<NoNameUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<NoNameUser> passwordHasher, 
            IEnumerable<IUserValidator<NoNameUser>> userValidators, 
            IEnumerable<IPasswordValidator<NoNameUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<NoNameUser>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) {
        }
    }
}
