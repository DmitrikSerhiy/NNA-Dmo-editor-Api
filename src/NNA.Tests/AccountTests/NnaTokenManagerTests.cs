using FluentAssertions;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NNA.Api.Features.Account.Services;
using NNA.Domain.DTOs.Account;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;
using Xunit;

namespace NNA.Tests.AccountTests; 
public class NnaTokenManagerTests {

    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<NnaTokenHandler> _nnaTokenHandlerMock = null!;
    private Mock<IAuthenticatedIdentityProvider> _authenticatedIdentityProviderMock = null!;

    private static readonly NnaUser NnaUser = new() {
        Id = Guid.NewGuid(),
        Email = "UserEmail@gmail.com"
    };
        
    private static readonly NnaToken NewAccessToken = new() {
        UserId = NnaUser.Id,
        Name = nameof(TokenName.Access),
        LoginProvider = Enum.GetName(LoginProviderName.password),
        Value = "NewAccessToken",
        TokenKeyId = "NewAccessTokenId"
    };
            
    private static readonly NnaToken NewRefreshToken = new() {
        UserId = NnaUser.Id,
        Name = nameof(TokenName.Refresh),
        LoginProvider = Enum.GetName(LoginProviderName.password),
        Value = "NewRefreshToken",
        TokenKeyId = "NewRefreshTokenId"
    };
    
    // ReSharper disable once EmptyConstructor
    public NnaTokenManagerTests() { }

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
    public async Task CreateTokensAsyncShouldCreateTokens() {
        // Arrange
        var subject = SetupSubjectMock();
            
        _nnaTokenHandlerMock.Setup(th => th.CreateNnaAccessToken(It.IsAny<NnaUser>()))
            .Returns(NewAccessToken.Value);
            
        _nnaTokenHandlerMock.Setup(th => th.CreateNnaRefreshToken(It.IsAny<NnaUser>()))
            .Returns(NewRefreshToken.Value);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(NewAccessToken.Value))))
            .Returns(NewAccessToken.TokenKeyId);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(NewRefreshToken.Value))))
            .Returns(NewRefreshToken.TokenKeyId);
            
        // Act
        async Task<TokensDto> Act() => await subject.CreateTokensAsync(NnaUser);
        var result = await Act();

        // Assert
        _nnaTokenHandlerMock.Verify(th => th
            .GetTokenKeyId(It.Is<string>(t => t.Equals(NewAccessToken.Value))), Times.Once());
        _nnaTokenHandlerMock.Verify(th => th
            .GetTokenKeyId(It.Is<string>(t => t.Equals(NewRefreshToken.Value))), Times.Once());
            
        _userRepositoryMock.Verify(ur => ur.SaveTokens(
                It.Is<NnaToken>(nnaToken => 
                    nnaToken.Value.Equals(NewAccessToken.Value) &&
                    nnaToken.TokenKeyId.Equals(NewAccessToken.TokenKeyId) &&
                    nnaToken.Name.Equals(NewAccessToken.Name) && 
                    nnaToken.LoginProvider.Equals(NewAccessToken.LoginProvider) &&
                    nnaToken.UserId.Equals(NewAccessToken.UserId)),
                It.Is<NnaToken>(nnaToken => 
                    nnaToken.Value.Equals(NewRefreshToken.Value) &&
                    nnaToken.TokenKeyId.Equals(NewRefreshToken.TokenKeyId) &&
                    nnaToken.Name.Equals(NewRefreshToken.Name) && 
                    nnaToken.LoginProvider.Equals(NewRefreshToken.LoginProvider) &&
                    nnaToken.UserId.Equals(NewRefreshToken.UserId))),
            Times.Once());
            
        result.AccessToken.Should().Be(NewAccessToken.Value);
        result.AccessTokenKeyId.Should().Be(NewAccessToken.TokenKeyId);
        result.RefreshToken.Should().Be(NewRefreshToken.Value);
        result.RefreshTokenKeyId.Should().Be(NewRefreshToken.TokenKeyId);
    }
        
    [Fact]
    public async Task CreateTokensAsyncShouldThrowIfUserIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();
            
        // Act
        async Task<TokensDto> Act() => await subject.CreateTokensAsync(null);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<ArgumentNullException>()
            .WithParameterName("user");
    }
        
    [Fact]
    public async Task VerifyTokenShouldReturnTrueIfTokensAreLoaded() {
        // Arrange
        var subject = SetupSubjectMock();
        _authenticatedIdentityProviderMock
            .Setup(ap => ap.AuthenticatedUserId)
            .Returns(NnaUser.Id);
            
        _userRepositoryMock.Setup(ur => ur.GetTokens(It.Is<Guid>(t => t.Equals(NnaUser.Id))))
            .ReturnsAsync((NewAccessToken, NewRefreshToken));
            
        // Act
        async Task<bool> Act() => await subject.VerifyTokenAsync();
        var result = await Act();
            
        // Assert
        result.Should().BeTrue();
    }
        
    [Fact]
    public async Task VerifyTokenShouldReturnFalseIfTokensAreMissing() {
        // Arrange
        var subject = SetupSubjectMock();
        _authenticatedIdentityProviderMock
            .Setup(ap => ap.AuthenticatedUserId)
            .Returns(NnaUser.Id);
            
        _userRepositoryMock.Setup(ur => ur.GetTokens(It.Is<Guid>(t => t.Equals(NnaUser.Id))))
            .ReturnsAsync(((NnaToken accessToken, NnaToken refreshToken)?)null);
            
        // Act
        async Task<bool> Act() => await subject.VerifyTokenAsync();
        var result = await Act();
            
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetOrCreateTokensShouldReturnSavedTokens() {
        // Arrange
        var subject = SetupSubjectMock();
            
        _userRepositoryMock.Setup(ur => ur.GetTokens(It.Is<Guid>(t => t.Equals(NnaUser.Id))))
            .ReturnsAsync((NewAccessToken, NewRefreshToken));
        _nnaTokenHandlerMock.Setup(th => th.ValidateAccessToken(It.IsAny<string>(), It.IsAny<NnaUser>()))
            .Returns(new TokenValidationResult { IsValid = true });
        _nnaTokenHandlerMock.Setup(th => th.ValidateRefreshToken(It.IsAny<string>(), It.IsAny<NnaUser>()))
            .Returns(new TokenValidationResult { IsValid = true });
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(NewAccessToken.Value))))
            .Returns(NewAccessToken.TokenKeyId);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(NewRefreshToken.Value))))
            .Returns(NewRefreshToken.TokenKeyId);
            
        // Act
        async Task<TokensDto> Act() => await subject.GetOrCreateTokensAsync(NnaUser);
        var result = await Act();

        // Assert
        _userRepositoryMock.Verify(ur => ur.UpdateTokens(
                It.IsAny<NnaToken>(),
                It.IsAny<NnaToken>()),
            Times.Never());
            
        _nnaTokenHandlerMock.Verify(th => th
            .GetTokenKeyId(It.Is<string>(t => t.Equals(NewAccessToken.Value))), Times.Once());
        _nnaTokenHandlerMock.Verify(th => th
            .GetTokenKeyId(It.Is<string>(t => t.Equals(NewRefreshToken.Value))), Times.Once());
            
        result.AccessToken.Should().Be(NewAccessToken.Value);
        result.AccessTokenKeyId.Should().Be(NewAccessToken.TokenKeyId);
        result.RefreshToken.Should().Be(NewRefreshToken.Value);
        result.RefreshTokenKeyId.Should().Be(NewRefreshToken.TokenKeyId);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task GetOrCreateTokensShouldUpdatePreviousTokensIfPreviousTokensInvalid(bool accessTokenIsValid, bool refreshTokenIsValid) {
        // Arrange
        var subject = SetupSubjectMock();
            
        _userRepositoryMock.Setup(ur => ur.GetTokens(It.Is<Guid>(t => t.Equals(NnaUser.Id))))
            .ReturnsAsync((NewAccessToken, NewRefreshToken));
        _nnaTokenHandlerMock.Setup(th => th.ValidateAccessToken(It.IsAny<string>(), It.IsAny<NnaUser>()))
            .Returns(new TokenValidationResult { IsValid = accessTokenIsValid });
        _nnaTokenHandlerMock.Setup(th => th.ValidateRefreshToken(It.IsAny<string>(), It.IsAny<NnaUser>()))
            .Returns(new TokenValidationResult { IsValid = refreshTokenIsValid });
            
        // Act
        await subject.GetOrCreateTokensAsync(NnaUser);

        // Assert
        _userRepositoryMock.Verify(ur => ur.UpdateTokens(
                It.IsAny<NnaToken>(),
                It.IsAny<NnaToken>()),
            Times.Once());
    }
        
    [Fact]
    public async Task GetOrCreateTokensShouldValidateLoadedTokens() {
        // Arrange
        var subject = SetupSubjectMock();
            
        _userRepositoryMock.Setup(ur => ur.GetTokens(It.Is<Guid>(t => t.Equals(NnaUser.Id))))
            .ReturnsAsync((NewAccessToken, NewRefreshToken));
        _nnaTokenHandlerMock.Setup(th => th.ValidateAccessToken(It.IsAny<string>(), It.IsAny<NnaUser>()))
            .Returns(new TokenValidationResult { IsValid = true });
        _nnaTokenHandlerMock.Setup(th => th.ValidateRefreshToken(It.IsAny<string>(), It.IsAny<NnaUser>()))
            .Returns(new TokenValidationResult { IsValid = true });
            
        // Act
        await subject.GetOrCreateTokensAsync(NnaUser);

        // Assert
        _nnaTokenHandlerMock.Verify(th => th.ValidateAccessToken(
                It.Is<string>(t => t.Equals(NewAccessToken.Value)), 
                It.Is<NnaUser>(t => t.Email.Equals(NnaUser.Email) && t.Id.Equals(NnaUser.Id))), 
            Times.Once );
            
        _nnaTokenHandlerMock.Verify(th => th.ValidateRefreshToken(
                It.Is<string>(t => t.Equals(NewRefreshToken.Value)), 
                It.Is<NnaUser>(t => t.Email.Equals(NnaUser.Email) && t.Id.Equals(NnaUser.Id))), 
            Times.Once );
    }
        
    [Fact]
    public async Task GetOrCreateTokensShouldCreateAndSaveNewTokensIfPreviousTokensWereRemoved() {
        // Arrange
        var subject = SetupSubjectMock();
            
        _nnaTokenHandlerMock.Setup(th => th.CreateNnaAccessToken(It.IsAny<NnaUser>()))
            .Returns(NewAccessToken.Value);
            
        _nnaTokenHandlerMock.Setup(th => th.CreateNnaRefreshToken(It.IsAny<NnaUser>()))
            .Returns(NewRefreshToken.Value);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(NewAccessToken.Value))))
            .Returns(NewAccessToken.TokenKeyId);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(NewRefreshToken.Value))))
            .Returns(NewRefreshToken.TokenKeyId);
            
        // Act
        async Task<TokensDto> Act() => await subject.GetOrCreateTokensAsync(NnaUser);
        var result = await Act();

        // Assert
        _userRepositoryMock.Verify(ur => ur.SaveTokens(
                It.Is<NnaToken>(nnaToken => 
                    nnaToken.Value.Equals(NewAccessToken.Value) &&
                    nnaToken.TokenKeyId.Equals(NewAccessToken.TokenKeyId) &&
                    nnaToken.Name.Equals(NewAccessToken.Name) && 
                    nnaToken.LoginProvider.Equals(NewAccessToken.LoginProvider) &&
                    nnaToken.UserId.Equals(NewAccessToken.UserId)),
                It.Is<NnaToken>(nnaToken => 
                    nnaToken.Value.Equals(NewRefreshToken.Value) &&
                    nnaToken.TokenKeyId.Equals(NewRefreshToken.TokenKeyId) &&
                    nnaToken.Name.Equals(NewRefreshToken.Name) && 
                    nnaToken.LoginProvider.Equals(NewRefreshToken.LoginProvider) &&
                    nnaToken.UserId.Equals(NewRefreshToken.UserId))),
            Times.Once());

        result.AccessToken.Should().Be(NewAccessToken.Value);
        result.AccessTokenKeyId.Should().Be(NewAccessToken.TokenKeyId);
        result.RefreshToken.Should().Be(NewRefreshToken.Value);
        result.RefreshTokenKeyId.Should().Be(NewRefreshToken.TokenKeyId);
    }
        
    [Fact]
    public async Task GetOrCreateTokensShouldThrowIfUserIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();
            
        // Act
        async Task<TokensDto> Act() => await subject.GetOrCreateTokensAsync(null);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<ArgumentNullException>()
            .WithParameterName("user");
    }
        
        
    [Fact]
    public async Task RefreshTokensShouldReturnTokens() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123",
            RefreshToken = "RefreshToken123"
        };

        var userTokens = new UsersTokens {
            AccessTokenId = "AccTknKey",
            RefreshTokenId = "RefTknKey",
            Email = NnaUser.Email,
            UserId = NnaUser.Id,
            LoginProvider = nameof(LoginProviderName.password)
        };

        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns(NnaUser.Email);

        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns(NnaUser.Email);

        _userRepositoryMock
            .Setup(ur => ur.GetAuthenticatedUserDataAsync(It.Is<string>(t => t.Equals(NnaUser.Email))))
            .ReturnsAsync(userTokens);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns(userTokens.AccessTokenId);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns(userTokens.RefreshTokenId);
            
        _nnaTokenHandlerMock
            .Setup(th => th.ValidateRefreshToken(
                It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken)),
                It.Is<NnaUser>(t => t.Email.Equals(NnaUser.Email) && t.Id.Equals(NnaUser.Id))))
            .Returns(new TokenValidationResult{IsValid = true});

        _nnaTokenHandlerMock.Setup(th => th.CreateNnaAccessToken(It.IsAny<NnaUser>()))
            .Returns(NewAccessToken.Value);
            
        _nnaTokenHandlerMock.Setup(th => th.CreateNnaRefreshToken(It.IsAny<NnaUser>()))
            .Returns(NewRefreshToken.Value);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(NewAccessToken.Value))))
            .Returns(NewAccessToken.TokenKeyId);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(NewRefreshToken.Value))))
            .Returns(NewRefreshToken.TokenKeyId);

        // Act
        async Task<TokensDto?> Act() => await subject.RefreshTokensAsync(refreshTokenDto);
        var result = await Act();

        // Assert
        _userRepositoryMock.Verify(ur => ur.UpdateTokens(
                It.Is<NnaToken>(nnaToken => 
                    nnaToken.Value.Equals(NewAccessToken.Value) &&
                    nnaToken.TokenKeyId.Equals(NewAccessToken.TokenKeyId) &&
                    nnaToken.Name.Equals(NewAccessToken.Name) && 
                    nnaToken.LoginProvider.Equals(NewAccessToken.LoginProvider) &&
                    nnaToken.UserId.Equals(NewAccessToken.UserId)),
                It.Is<NnaToken>(nnaToken => 
                    nnaToken.Value.Equals(NewRefreshToken.Value) &&
                    nnaToken.TokenKeyId.Equals(NewRefreshToken.TokenKeyId) &&
                    nnaToken.Name.Equals(NewRefreshToken.Name) && 
                    nnaToken.LoginProvider.Equals(NewRefreshToken.LoginProvider) &&
                    nnaToken.UserId.Equals(NewRefreshToken.UserId))),
            Times.Once());

        result!.AccessToken.Should().Be(NewAccessToken.Value);
        result.AccessTokenKeyId.Should().Be(NewAccessToken.TokenKeyId);
        result.RefreshToken.Should().Be(NewRefreshToken.Value);
        result.RefreshTokenKeyId.Should().Be(NewRefreshToken.TokenKeyId);
    }
        
    [Fact]
    public async Task RefreshTokensShouldReturnNullIfRefreshTokenIsNotValid() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123",
            RefreshToken = "RefreshToken123"
        };

        var userTokens = new UsersTokens {
            AccessTokenId = "AccTknKey",
            RefreshTokenId = "RefTknKey",
            Email = NnaUser.Email,
            UserId = NnaUser.Id
        };
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns(NnaUser.Email);

        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns(NnaUser.Email);

        _userRepositoryMock
            .Setup(ur => ur.GetAuthenticatedUserDataAsync(It.Is<string>(t => t.Equals(NnaUser.Email))))
            .ReturnsAsync(userTokens);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns(userTokens.AccessTokenId);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns(userTokens.RefreshTokenId);
            
        _nnaTokenHandlerMock
            .Setup(th => th.ValidateRefreshToken(
                It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken)),
                It.Is<NnaUser>(t => t.Email.Equals(NnaUser.Email) && t.Id.Equals(NnaUser.Id))))
            .Returns(new TokenValidationResult{IsValid = false});

        // Act
        async Task<TokensDto?> Act() => await subject.RefreshTokensAsync(refreshTokenDto);
        var result = await Act();

        // Assert
        result!.Should().BeNull();
    }
        
    [Fact]
    public async Task RefreshTokensShouldReturnNullIfRefreshTokenIdIsMissingIdDb() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123",
            RefreshToken = "RefreshToken123"
        };

        var userTokens = new UsersTokens {
            AccessTokenId = "AccTknKey",
            RefreshTokenId = "RefTknKey"
        };
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns(NnaUser.Email);

        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns(NnaUser.Email);

        _userRepositoryMock
            .Setup(ur => ur.GetAuthenticatedUserDataAsync(It.Is<string>(t => t.Equals(NnaUser.Email))))
            .ReturnsAsync(userTokens);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns(userTokens.AccessTokenId);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns("AnotherRefTknKey");

        // Act
        async Task<TokensDto?> Act() => await subject.RefreshTokensAsync(refreshTokenDto);
        var result = await Act();

        // Assert
        result.Should().BeNull();
    }
        
    [Fact]
    public async Task RefreshTokensShouldReturnNullIfAccessTokenIdIsMissingIdDb() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123",
            RefreshToken = "RefreshToken123"
        };

        var userTokens = new UsersTokens {
            AccessTokenId = "AccTknKey"
        };
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns(NnaUser.Email);

        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns(NnaUser.Email);

        _userRepositoryMock
            .Setup(ur => ur.GetAuthenticatedUserDataAsync(It.Is<string>(t => t.Equals(NnaUser.Email))))
            .ReturnsAsync(userTokens);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetTokenKeyId(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns("AnotherAccTknKey");

        // Act
        async Task<TokensDto?> Act() => await subject.RefreshTokensAsync(refreshTokenDto);
        var result = await Act();

        // Assert
        result.Should().BeNull();
    }
        

    [Fact]
    public async Task RefreshTokensShouldReturnNullIfAuthenticatedUserDataIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123",
            RefreshToken = "RefreshToken123"
        };
        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns(NnaUser.Email);

        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns(NnaUser.Email);

        _userRepositoryMock
            .Setup(ur => ur.GetAuthenticatedUserDataAsync(It.Is<string>(t => t.Equals(NnaUser.Email))))
            .ReturnsAsync((UsersTokens?)null);

        // Act
        async Task<TokensDto?> Act() => await subject.RefreshTokensAsync(refreshTokenDto);
        var result = await Act();

        // Assert
        result.Should().BeNull();
    }
        
        
    [Fact]
    public async Task RefreshTokensShouldReturnNullIfEmailsInRefreshTokenAndAccessTokenAreDifferent() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123",
            RefreshToken = "RefreshToken123"
        };
        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns("someMail@gmail.com");

        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns("anotherMail@gmail.com");

        // Act
        async Task<TokensDto?> Act() => await subject.RefreshTokensAsync(refreshTokenDto);
        var result = await Act();

        // Assert
        result.Should().BeNull();
    }
        
    [Fact]
    public async Task RefreshTokensShouldReturnNullIfEmailIsMissingInAccessToken() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123",
            RefreshToken = "RefreshToken123"
        };
        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns("someMail@gmail.com");
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns((string?)null);

        // Act
        async Task<TokensDto?> Act() => await subject.RefreshTokensAsync(refreshTokenDto);
        var result = await Act();

        // Assert
        result.Should().BeNull();
    }
        
    [Fact]
    public async Task RefreshTokensShouldReturnNullIfEmailIsMissingInRefreshToken() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123",
            RefreshToken = "RefreshToken123"
        };
        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.RefreshToken))))
            .Returns((string?)null);
            
        _nnaTokenHandlerMock
            .Setup(th => th.GetUserEmail(It.Is<string>(t => t.Equals(refreshTokenDto.AccessToken))))
            .Returns("someMail@gmail.com");


        // Act
        async Task<TokensDto?> Act() => await subject.RefreshTokensAsync(refreshTokenDto);
        var result = await Act();

        // Assert
        result.Should().BeNull();
    }
        
    [Fact]
    public async Task RefreshTokensShouldThrowIfRefreshTokenAndAccessTokenTheSame() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123",
            RefreshToken = "AccessToken123"
        };

        // Act
        async Task Act() => await subject.RefreshTokensAsync(refreshTokenDto);
            
        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<ArgumentException>();
    }
        
                        
    [Fact]
    public async Task RefreshTokensShouldThrowIfRefreshTokenIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();
        var refreshTokenDto = new RefreshDto {
            AccessToken = "AccessToken123"
        };

        // Act
        async Task Act() => await subject.RefreshTokensAsync(refreshTokenDto);
            
        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<ArgumentNullException>()
            .WithParameterName(nameof(RefreshDto.RefreshToken));
    }

                
    [Fact]
    public async Task RefreshTokensShouldThrowIfAccessTokenIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();

        // Act
        async Task Act() => await subject.RefreshTokensAsync(new RefreshDto());
            
        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<ArgumentNullException>()
            .WithParameterName(nameof(RefreshDto.AccessToken));
    }
        
    [Fact]
    public async Task ShouldClearTokens() {
        // Arrange
        var subject = SetupSubjectMock();
        _authenticatedIdentityProviderMock
            .Setup(ap => ap.AuthenticatedUserEmail)
            .Returns(NnaUser.Email);

        // Act
        await subject.ClearTokensAsync(NnaUser);

        // Assert
        _userRepositoryMock.Verify(up => up.ClearTokens(NnaUser), Times.Once);
        _authenticatedIdentityProviderMock.Verify(up => up.ClearAuthenticatedUserInfo(), Times.Once);
    }

    [Fact]
    public async Task ClearTokensShouldNotClearTokensIfUserIsNotAuthenticated() {
        // Arrange
        var subject = SetupSubjectMock();
        _authenticatedIdentityProviderMock
            .Setup(ap => ap.AuthenticatedUserEmail)
            .Returns(string.Empty);

        // Act
        await subject.ClearTokensAsync(NnaUser);

        // Assert
        _userRepositoryMock.Verify(up => up.ClearTokens(It.IsAny<NnaUser>()), Times.Never);
        _authenticatedIdentityProviderMock.Verify(up => up.ClearAuthenticatedUserInfo(), Times.Never);
    }
        
    [Fact]
    public async Task ClearTokensShouldThrowIfUserIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();

        // Act
        async Task Act() => await subject.ClearTokensAsync(null);
            
        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<ArgumentNullException>()
            .WithParameterName("user");
    }
        
    [Fact]
    public async Task ShouldValidateGoogleToken() {
        // Arrange
        var subject = SetupSubjectMock();
        var authGoogleDtoWithoutEmail = new AuthGoogleDto {
            Email = NnaUser.Email
        };
        _nnaTokenHandlerMock.Setup(th => th.ValidateGoogleTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new GoogleJsonWebSignature.Payload { EmailVerified = true, Email = NnaUser.Email});
            
        // Act
        async Task<bool> Act() => await subject.ValidateGoogleTokenAsync(authGoogleDtoWithoutEmail);
        var result = await Act();
            
        // Assert
        result.Should().BeTrue();
    }
        
    [Fact]
    public async Task ValidateGoogleTokenShouldReturnFalseIfGoogleTokenIsInvalid() {
        // Arrange
        var subject = SetupSubjectMock();
        var authGoogleDtoWithoutEmail = new AuthGoogleDto {
            Email = NnaUser.Email
        };
        _nnaTokenHandlerMock.Setup(th => th.ValidateGoogleTokenAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidJwtException("Token invalid"));
            
        // Act
        async Task<bool> Act() => await subject.ValidateGoogleTokenAsync(authGoogleDtoWithoutEmail);
        var result = await Act();
            
        // Assert
        result.Should().BeFalse();
    }
        
    [Fact]
    public async Task ValidateGoogleTokenShouldReturnFalseIfGoogleUsesEmailAndEmailFromTokenDifferent() {
        // Arrange
        var subject = SetupSubjectMock();
        var authGoogleDtoWithoutEmail = new AuthGoogleDto {
            Email = NnaUser.Email
        };
        _nnaTokenHandlerMock.Setup(th => th.ValidateGoogleTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new GoogleJsonWebSignature.Payload { EmailVerified = true, Email = "anotherEmail@gmail.com"});
            
        // Act
        async Task<bool> Act() => await subject.ValidateGoogleTokenAsync(authGoogleDtoWithoutEmail);
        var result = await Act();
            
        // Assert
        result.Should().BeFalse();
    }
        
    [Fact]
    public async Task ValidateGoogleTokenShouldReturnFalseIfGoogleUsesEmailIsNotVerified() {
        // Arrange
        var subject = SetupSubjectMock();
        var authGoogleDtoWithoutEmail = new AuthGoogleDto {
            Email = NnaUser.Email
        };
        _nnaTokenHandlerMock.Setup(th => th.ValidateGoogleTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new GoogleJsonWebSignature.Payload { EmailVerified = false });
            
        // Act
        async Task<bool> Act() => await subject.ValidateGoogleTokenAsync(authGoogleDtoWithoutEmail);
        var result = await Act();
            
        // Assert
        result.Should().BeFalse();
    }
        
    [Fact]
    public async Task ValidateGoogleTokenShouldThrowIfGoogleDtoInvalid() {
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