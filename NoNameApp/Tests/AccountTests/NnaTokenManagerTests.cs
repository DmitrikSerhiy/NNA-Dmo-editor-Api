using System;
using System.Threading.Tasks;
using API.Features.Account.Services;
using FluentAssertions;
using Model.DTOs.Account;
using Model.Interfaces;
using Model.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Tests.AccountTests {
    public class NnaTokenManagerTests {

        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<NnaTokenHandler> _nnaTokenHandlerMock;
        private Mock<IAuthenticatedIdentityProvider> _authenticatedIdentityProviderMock;

        public NnaTokenManagerTests() {
            
        }

        private NnaTokenManager SetupSubjectMock() {
            _userRepositoryMock = new Mock<IUserRepository>();
            _nnaTokenHandlerMock = new Mock<NnaTokenHandler>();
            _authenticatedIdentityProviderMock = new Mock<IAuthenticatedIdentityProvider>();

            return new NnaTokenManager(
                _userRepositoryMock.Object, 
                _nnaTokenHandlerMock.Object,
                _authenticatedIdentityProviderMock.Object);
        }

        [Fact]
        public async Task ShouldThrowIfGoogleDtoInvalid1() {
            // Arrange
            
            // Act
            
            // Assert
            
        }
        
        [Fact]
        public async Task ShouldThrowIfGoogleDtoInvalid() {
            // Arrange
            var subject = SetupSubjectMock();
            var authGoogleDtoWithoutEmail = new AuthGoogleDto();
            
            // Act
            async Task<bool> Act() => await subject.ValidateGoogleTokenAsync(authGoogleDtoWithoutEmail);
            
            // Assert
            await FluentActions.Awaiting(Act)
                .Should()
                .ThrowExactlyAsync<ArgumentNullException>()
                .WithParameterName(nameof(AuthGoogleDto.Email));
        }
        
    }
}