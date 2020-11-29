using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Entities;

namespace API.Features.Account.Services {
    public class NnaUserManager : UserManager<NnaUser> {
        public NnaUserManager(
            IUserStore<NnaUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<NnaUser> passwordHasher, 
            IEnumerable<IUserValidator<NnaUser>> userValidators, 
            IEnumerable<IPasswordValidator<NnaUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<NnaUser>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) {
        }
    }
}
