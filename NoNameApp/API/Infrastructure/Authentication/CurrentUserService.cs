using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Model;
using Model.Entities;

namespace API.Infrastructure.Authentication {
    public class CurrentUserService {

        private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;
        private readonly IUserRepository _userRepository;

        private NoNameUser _cachedUser;

        public CurrentUserService(
            IAuthenticatedIdentityProvider authenticatedIdentityProvider, 
            IUserRepository userRepository) {
            _authenticatedIdentityProvider = authenticatedIdentityProvider ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<NoNameUser> GetAsync() {
            if (!_authenticatedIdentityProvider.IsAuthenticated) {
                throw new AuthenticationException("User is not authenticated");
            }

            if (_cachedUser != null) {
                return _cachedUser;
            }

            _cachedUser = await _userRepository.WithId(_authenticatedIdentityProvider.AuthenticatedUserId);
            return _cachedUser;
        }

        public Boolean IsAuthenticated() {
            return _authenticatedIdentityProvider.IsAuthenticated;
        } 

    }
}
