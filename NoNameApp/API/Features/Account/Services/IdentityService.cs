using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;

namespace API.Features.Account.Services {
    public sealed class IdentityService {
        
        private readonly NnaUserManager _userManager;
        private readonly TokenDescriptorProvider _descriptorProvider;
        private readonly JsonWebTokenHandler _tokenHandler;

        public IdentityService(NnaUserManager userManager, TokenDescriptorProvider descriptorProvider) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _descriptorProvider = descriptorProvider ?? throw new ArgumentNullException(nameof(descriptorProvider));
            _tokenHandler = new JsonWebTokenHandler();
        }
        
        public async Task<string> CreateJwtAsync(string email) {
            if (email == null) throw new ArgumentNullException(nameof(email));

            var user = await _userManager.FindByEmailAsync(email);
            
            _descriptorProvider.AddSigningCredentials();
            _descriptorProvider.AddSubjectClaims(email, user.Id);
            
            return _tokenHandler.CreateToken(_descriptorProvider.Provide());
        }
    }
}