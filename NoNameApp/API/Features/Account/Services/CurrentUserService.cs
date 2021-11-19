using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Model.Entities;
using Model.Interfaces;
using Model.Interfaces.Repositories;

namespace API.Features.Account.Services {
    public class CurrentUserService {

        private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;
        private readonly IUserRepository _userRepository;

        private NnaUser _cachedUser;

        public CurrentUserService(
            IAuthenticatedIdentityProvider authenticatedIdentityProvider, 
            IUserRepository userRepository) {
            _authenticatedIdentityProvider = authenticatedIdentityProvider ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        // todo: remove and use authenticatedIdentityProvider
        public async Task<NnaUser> GetAsync() {
            if (!_authenticatedIdentityProvider.IsAuthenticated) {
                throw new AuthenticationException("User is not authenticated");
            }

            if (_cachedUser != null) {
                return _cachedUser;
            }

            _cachedUser = await _userRepository.WithId(_authenticatedIdentityProvider.AuthenticatedUserId);
            return _cachedUser;
        }

        public bool IsAuthenticated() {
            return _authenticatedIdentityProvider.IsAuthenticated;
        } 

    }
}
