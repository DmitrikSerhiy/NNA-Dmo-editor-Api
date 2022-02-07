using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Helpers;
using FluentAssertions;
using Model.Entities;
using Model.Enums;
using Model.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Tests.AccountTests {
    public class ClaimsValidatorTests {

        private readonly List<Claim> _claimsToValidate;
        
        public ClaimsValidatorTests() {
            _claimsToValidate = new List<Claim>();
        }

        private static readonly NnaUser NnaUser = new() {
            Id = Guid.NewGuid(),
            UserName = "User Name",
            Email = "UserEmail@gmail.com"
        };
        private static readonly string AccessTokenId = Guid.NewGuid().ToString();
        
        
        private readonly Claim _claimForRefreshToken = new (nameof(NnaCustomTokenClaims.gtyp), NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.gtyp));
        private readonly Claim _userIdClaim = new(ClaimTypes.NameIdentifier, NnaUser.Id.ToString());
        private readonly Claim _userEmailClaim = new(ClaimTypes.Email, NnaUser.Email);
        private readonly Claim _tokenIdClaim = new(NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.oid), AccessTokenId);

        private readonly UsersTokens _userTokens = new() {
            Email = NnaUser.Email,
            UserId = NnaUser.Id,
            AccessTokenId = AccessTokenId
        };

        private static readonly UsersTokens UserTokensWithWrongEmail = new() {
            Email = "other@gmail.com",
            UserId = NnaUser.Id,
            AccessTokenId = AccessTokenId
        };
        
        private static readonly UsersTokens UserTokensWithWrongAccessTokenId = new() {
            Email = NnaUser.Email,
            UserId = NnaUser.Id,
            AccessTokenId = Guid.NewGuid().ToString()
        };
        
        private static readonly UsersTokens UserTokensWithWrongUserId = new() {
            Email = NnaUser.Email,
            UserId = Guid.NewGuid(),
            AccessTokenId = AccessTokenId
        };
        
        
        [Fact]
        public async Task  ShouldThrowExceptionIfRefreshTokenIsPassed() {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var subject = new ClaimsValidator(userRepository.Object);
            _claimsToValidate.Add(_claimForRefreshToken);

            // Act
            async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate);

            // Assert
            await FluentActions.Awaiting(Act)
                .Should()
                .ThrowExactlyAsync<AuthenticationException>()
                .WithMessage("Invalid token. Refresh token should not be used as authentication key");
        }
        
        [Fact]
        public async Task  ShouldThrowExceptionIfUserIdClaimIsMissing() {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var subject = new ClaimsValidator(userRepository.Object);
            
            // Act
            async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate);

            // Assert
            await FluentActions.Awaiting(Act)
                .Should()
                .ThrowExactlyAsync<AuthenticationException>()
                .WithMessage("User id claim is missing");
        }
        
        [Fact]
        public async Task  ShouldThrowExceptionIfUserEmailClaimIsMissing() {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var subject = new ClaimsValidator(userRepository.Object);
            _claimsToValidate.Add(_userIdClaim);
                
            // Act
            async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate);

            // Assert
            await FluentActions.Awaiting(Act)
                .Should()
                .ThrowExactlyAsync<AuthenticationException>()
                .WithMessage("User email claim is missing");
        }
        
        [Fact]
        public async Task  ShouldThrowExceptionIfTokenIdClaimIsMissing() {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var subject = new ClaimsValidator(userRepository.Object);
            _claimsToValidate.Add(_userIdClaim);
            _claimsToValidate.Add(_userEmailClaim);

            // Act
            async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate);

            // Assert
            await FluentActions.Awaiting(Act)
                .Should()
                .ThrowExactlyAsync<AuthenticationException>()
                .WithMessage("User oid claim is missing");
        }

        [Fact]
        public async Task  ShouldThrowExceptionIfUserAuthenticationIsMissing() {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var subject = new ClaimsValidator(userRepository.Object);
            _claimsToValidate.Add(_userIdClaim);
            _claimsToValidate.Add(_userEmailClaim);
            _claimsToValidate.Add(_tokenIdClaim);
            userRepository.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value))
                .ReturnsAsync((UsersTokens)null);
            
            // Act
            async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate);
            
            // Assert
            await FluentActions.Awaiting(Act)
                .Should()
                .ThrowExactlyAsync<AuthenticationException>()
                .WithMessage($"Authentication data for '{_userTokens.Email}' is not saved");
        }
        
        [Fact]
        public async Task  ShouldThrowExceptionIfUserAuthenticationHasWrongEmail() {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var subject = new ClaimsValidator(userRepository.Object);
            _claimsToValidate.Add(_userIdClaim);
            _claimsToValidate.Add(_userEmailClaim);
            _claimsToValidate.Add(_tokenIdClaim);
            userRepository.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value))
                .ReturnsAsync(UserTokensWithWrongEmail);
            
            // Act
            async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate);
            
            // Assert
            await FluentActions.Awaiting(Act)
                .Should()
                .ThrowExactlyAsync<AuthenticationException>()
                .WithMessage($"Inconsistent auth data for user: '{_userEmailClaim.Value}'");
        }
        
        [Fact]
        public async Task  ShouldThrowExceptionIfUserAuthenticationHasWrongUserId() {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var subject = new ClaimsValidator(userRepository.Object);
            _claimsToValidate.Add(_userIdClaim);
            _claimsToValidate.Add(_userEmailClaim);
            _claimsToValidate.Add(_tokenIdClaim);
            userRepository.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value))
                .ReturnsAsync(UserTokensWithWrongUserId);
            
            // Act
            async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate);
            
            // Assert
            await FluentActions.Awaiting(Act)
                .Should()
                .ThrowExactlyAsync<AuthenticationException>()
                .WithMessage($"Inconsistent auth data for user: '{_userEmailClaim.Value}'");
        }
        
        [Fact]
        public async Task  ShouldThrowExceptionIfUserAuthenticationHasWrongAccessTokenId() {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var subject = new ClaimsValidator(userRepository.Object);
            _claimsToValidate.Add(_userIdClaim);
            _claimsToValidate.Add(_userEmailClaim);
            _claimsToValidate.Add(_tokenIdClaim);
            userRepository.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value))
                .ReturnsAsync(UserTokensWithWrongAccessTokenId);
            
            // Act
            async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate);
            
            // Assert
            await FluentActions.Awaiting(Act)
                .Should()
                .ThrowExactlyAsync<AuthenticationException>()
                .WithMessage($"Inconsistent auth data for user: '{_userEmailClaim.Value}'");
        }
        
        [Fact]
        public async Task  ShouldReturnValidatedUserTokens() {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var subject = new ClaimsValidator(userRepository.Object);
            _claimsToValidate.Add(_userIdClaim);
            _claimsToValidate.Add(_userEmailClaim);
            _claimsToValidate.Add(_tokenIdClaim);
            userRepository.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value))
                .ReturnsAsync(_userTokens);
            
            // Act
            async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate);
            var result = await Act();
            
            // Assert
            result.Should().Be(_userTokens);
            userRepository.Verify(u => u.GetAuthenticatedUserDataAsync(_userEmailClaim.Value), Times.Once);
        }
    }
}