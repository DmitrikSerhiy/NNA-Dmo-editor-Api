using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Model.DTOs.Account;

namespace API.Features.Account.Services.Local {
    public class LocalIdentityService {
        private readonly NnaLocalUserManager _localUserManager;
        private readonly TokenDescriptorProvider _descriptorProvider;
        private readonly JsonWebTokenHandler _tokenHandler;

        public LocalIdentityService(NnaLocalUserManager localUserManager,  TokenDescriptorProvider descriptorProvider) {
            _localUserManager = localUserManager ?? throw new ArgumentNullException(nameof(localUserManager));
            _descriptorProvider = descriptorProvider ?? throw new ArgumentNullException(nameof(descriptorProvider));
            _tokenHandler = new JsonWebTokenHandler();
        }
        
        public async Task<string> CreateJwtAsync(string email) {
            if (email == null) throw new ArgumentNullException(nameof(email));

            var user = await _localUserManager.FindByEmailAsync(email);
            
            _descriptorProvider.AddSigningCredentials();
            _descriptorProvider.AddSubjectClaims(email, user.Id);
            
            return _tokenHandler.CreateToken(_descriptorProvider.Provide());
        }
    }
}
