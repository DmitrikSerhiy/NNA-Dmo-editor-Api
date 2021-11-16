using System;
using System.Threading.Tasks;
using API.Helpers;
using API.Helpers.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace API.Features.Account.Services.Local {
    public class LocalIdentityService {
        private readonly NnaLocalUserManager _localUserManager;
        private readonly TokenDescriptorProvider _descriptorProvider;
        private readonly JsonWebTokenHandler _tokenHandler;
        private readonly JwtOptions _jwtOptions;

        public LocalIdentityService(
            NnaLocalUserManager localUserManager, 
            TokenDescriptorProvider descriptorProvider,
            IOptions<JwtOptions> jwtOptions) {
            _localUserManager = localUserManager ?? throw new ArgumentNullException(nameof(localUserManager));
            _descriptorProvider = descriptorProvider ?? throw new ArgumentNullException(nameof(descriptorProvider));
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _tokenHandler = new JsonWebTokenHandler();
        }
        
        public async Task<string> CreateJwtAsync(string email) {
            if (email == null) throw new ArgumentNullException(nameof(email));

            var user = await _localUserManager.FindByEmailAsync(email);

            var tokenDescriptor = _descriptorProvider.ProvideForAccessToken();
            tokenDescriptor.AddSigningCredentials(_jwtOptions);
            tokenDescriptor.AddSubjectClaims(user.Email, user.Id);

            return _tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}
