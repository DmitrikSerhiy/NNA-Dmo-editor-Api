using System;
using System.Threading.Tasks;
using API.Helpers;
using API.Helpers.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Model.DTOs.Account;
using Model.Entities;

namespace API.Features.Account.Services {
    public sealed class IdentityService {
        
        private readonly NnaUserManager _userManager;
        private readonly TokenDescriptorProvider _descriptorProvider;
        private readonly JsonWebTokenHandler _tokenHandler;
        private readonly JwtOptions _jwtOptions;

        public IdentityService(
            NnaUserManager userManager, 
            TokenDescriptorProvider descriptorProvider,
            IOptions<JwtOptions> jwtOptions) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _descriptorProvider = descriptorProvider ?? throw new ArgumentNullException(nameof(descriptorProvider));
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _tokenHandler = new JsonWebTokenHandler();
        }
        
        public string CreateAccessJwt(NnaUser user) {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            var accessTokenDescriptor = _descriptorProvider.ProvideForAccessToken();
            accessTokenDescriptor.AddSigningCredentials(_jwtOptions);
            accessTokenDescriptor.AddSubjectClaims(user.Email, user.Id);
            
            return _tokenHandler.CreateToken(accessTokenDescriptor);
        }
        
        public string CreateRefreshJwt(NnaUser user) {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var refreshTokenDescriptor = _descriptorProvider.ProvideForRefreshToken();
            refreshTokenDescriptor.AddSigningCredentials(_jwtOptions);
            refreshTokenDescriptor.AddSubjectClaims(user.Email, user.Id);
            
            return _tokenHandler.CreateToken(refreshTokenDescriptor);
        }
        
        public async Task<TokensDto> CreateTokensAsync(string email) {
            if (email == null) throw new ArgumentNullException(nameof(email));

            // todo: save login
            var user = await _userManager.FindByEmailAsync(email);
            return new TokensDto {
                AccessToken = CreateAccessJwt(user),
                RefreshToken = CreateRefreshJwt(user)
            };
        }
    }
}