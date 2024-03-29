﻿using System.Security.Authentication;
using System.Security.Claims;
using FluentAssertions;
using Moq;
using NNA.Api.Helpers;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Interfaces.Repositories;
using Xunit;

namespace NNA.Tests.AccountTests;

public sealed class ClaimsValidatorTests {
    private readonly List<Claim> _claimsToValidate;
    private Mock<IUserRepository> _userRepositoryMock = null!;

    public ClaimsValidatorTests() {
        _claimsToValidate = new List<Claim>();
    }

    private ClaimsValidator SetupSubjectMock() {
        _userRepositoryMock = new Mock<IUserRepository>();
        return new ClaimsValidator(_userRepositoryMock.Object);
    }

    private static readonly NnaUser NnaUser = new() {
        Id = Guid.NewGuid(),
        UserName = "User Name",
        Email = "UserEmail@gmail.com"
    };

    private static readonly string AccessTokenId = Guid.NewGuid().ToString();

    private readonly Claim _claimForRefreshToken = new(nameof(NnaCustomTokenClaims.gtyp),
        NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.gtyp));

    private readonly Claim _userIdClaim = new(ClaimTypes.NameIdentifier, NnaUser.Id.ToString());
    private readonly Claim _userEmailClaim = new(ClaimTypes.Email, NnaUser.Email);

    private readonly Claim _tokenIdClaim =
        new(NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.oid), AccessTokenId);

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
    public async Task ShouldThrowExceptionIfRefreshTokenIsPassed() {
        // Arrange
        var subject = SetupSubjectMock();
        _claimsToValidate.Add(_claimForRefreshToken);

        // Act
        async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate, CancellationToken.None);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<AuthenticationException>()
            .WithMessage("Invalid token. Refresh token should not be used as authentication key");
    }

    [Fact]
    public async Task ShouldThrowExceptionIfUserIdClaimIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();

        // Act
        async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate, CancellationToken.None);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<AuthenticationException>()
            .WithMessage("User id claim is missing");
    }

    [Fact]
    public async Task ShouldThrowExceptionIfUserEmailClaimIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();
        _claimsToValidate.Add(_userIdClaim);

        // Act
        async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate, CancellationToken.None);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<AuthenticationException>()
            .WithMessage("User email claim is missing");
    }

    [Fact]
    public async Task ShouldThrowExceptionIfTokenIdClaimIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();
        _claimsToValidate.Add(_userIdClaim);
        _claimsToValidate.Add(_userEmailClaim);

        // Act
        async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate, CancellationToken.None);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<AuthenticationException>()
            .WithMessage("User oid claim is missing");
    }

    [Fact]
    public async Task ShouldThrowExceptionIfUserAuthenticationIsMissing() {
        // Arrange
        var subject = SetupSubjectMock();
        _claimsToValidate.Add(_userIdClaim);
        _claimsToValidate.Add(_userEmailClaim);
        _claimsToValidate.Add(_tokenIdClaim);
        _userRepositoryMock.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value, CancellationToken.None))
            .ReturnsAsync((UsersTokens?)null);

        // Act
        async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate, CancellationToken.None);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<AuthenticationException>()
            .WithMessage($"Authentication data for '{_userTokens.Email}' is not saved");
    }

    [Fact]
    public async Task ShouldThrowExceptionIfUserAuthenticationHasWrongEmail() {
        // Arrange
        var subject = SetupSubjectMock();
        _claimsToValidate.Add(_userIdClaim);
        _claimsToValidate.Add(_userEmailClaim);
        _claimsToValidate.Add(_tokenIdClaim);
        _userRepositoryMock.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value, CancellationToken.None))
            .ReturnsAsync(UserTokensWithWrongEmail);

        // Act
        async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate, CancellationToken.None);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<AuthenticationException>()
            .WithMessage($"Inconsistent auth data for user: '{_userEmailClaim.Value}'");
    }

    [Fact]
    public async Task ShouldThrowExceptionIfUserAuthenticationHasWrongUserId() {
        // Arrange
        var subject = SetupSubjectMock();
        _claimsToValidate.Add(_userIdClaim);
        _claimsToValidate.Add(_userEmailClaim);
        _claimsToValidate.Add(_tokenIdClaim);
        _userRepositoryMock.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value, CancellationToken.None))
            .ReturnsAsync(UserTokensWithWrongUserId);

        // Act
        async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate, CancellationToken.None);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<AuthenticationException>()
            .WithMessage($"Inconsistent auth data for user: '{_userEmailClaim.Value}'");
    }

    [Fact]
    public async Task ShouldThrowExceptionIfUserAuthenticationHasWrongAccessTokenId() {
        // Arrange
        var subject = SetupSubjectMock();
        _claimsToValidate.Add(_userIdClaim);
        _claimsToValidate.Add(_userEmailClaim);
        _claimsToValidate.Add(_tokenIdClaim);
        _userRepositoryMock.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value, CancellationToken.None))
            .ReturnsAsync(UserTokensWithWrongAccessTokenId);

        // Act
        async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate, CancellationToken.None);

        // Assert
        await FluentActions.Awaiting(Act)
            .Should()
            .ThrowExactlyAsync<AuthenticationException>()
            .WithMessage($"Inconsistent auth data for user: '{_userEmailClaim.Value}'");
    }

    [Fact]
    public async Task ShouldReturnValidatedUserTokens() {
        // Arrange
        var subject = SetupSubjectMock();
        _claimsToValidate.Add(_userIdClaim);
        _claimsToValidate.Add(_userEmailClaim);
        _claimsToValidate.Add(_tokenIdClaim);
        _userRepositoryMock.Setup(m => m.GetAuthenticatedUserDataAsync(_userEmailClaim.Value, CancellationToken.None))
            .ReturnsAsync(_userTokens);

        // Act
        async Task<UsersTokens> Act() => await subject.ValidateAndGetAuthDataAsync(_claimsToValidate, CancellationToken.None);
        var result = await Act();

        // Assert
        result.Should().Be(_userTokens);
        _userRepositoryMock.Verify(u => u.GetAuthenticatedUserDataAsync(_userEmailClaim.Value, CancellationToken.None), Times.Once);
    }
}