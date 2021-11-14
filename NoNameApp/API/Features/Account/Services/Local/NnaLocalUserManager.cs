using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Entities;

namespace API.Features.Account.Services.Local {
    public class NnaLocalUserManager : UserManager<NnaUser> {
        public NnaLocalUserManager(
            IUserStore<NnaUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<NnaUser> passwordHasher, 
            IEnumerable<IUserValidator<NnaUser>> userValidators, 
            IEnumerable<IPasswordValidator<NnaUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<NnaLocalUserManager> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) {
        }
    }
}
