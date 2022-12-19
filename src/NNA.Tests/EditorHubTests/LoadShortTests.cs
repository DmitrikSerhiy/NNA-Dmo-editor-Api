using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.DTOs.Editor.Response;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubTests;

public sealed class LoadShortTests : BaseEditorTests {
    private LoadShortDmoDto DmoDto { get; set; } = null!;

    private void SetMockAndVariables() {
        SetupConstructorMocks();
        DmoDto = new LoadShortDmoDto { Id = Guid.NewGuid().ToString() };
    }


    [Fact]
    public async Task ShouldReturnBadRequestIfEntryDtoIsEmptyTest() {
        //Arrange
        SetMockAndVariables();
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
#pragma warning disable CS0612
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(null);
#pragma warning restore CS0612
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateBadRequestResponse(),
            config => config
                .Excluding(exclude => exclude.errors)
                .Excluding(exclude => exclude.warnings)
                .Excluding(exclude => exclude.message));
    }

    [Fact]
    public async Task ShouldReturnNotAuthorizedIfNoUserInContextTest() {
        //Arrange
        SetMockAndVariables();
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        var hubContext = new Mock<HubCallerContext>();
        hubContext.Setup(hm => hm.Items).Returns(new Dictionary<object, object?>());
        Subject.Context = hubContext.Object;

        //Act
#pragma warning disable CS0612
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(DmoDto);
#pragma warning restore CS0612
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateFailedAuthResponse(),
            config => config
                .Excluding(exclude => exclude.warnings));
    }

    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
        //Arrange
        SetMockAndVariables();
        DmoDto.Id = null;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
#pragma warning disable CS0612
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(DmoDto);
#pragma warning restore CS0612
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(
            BaseEditorResponseDto.CreateFailedValidationResponse(new List<Tuple<string, string>>()),
            config => config
                .Including(include => include.httpCode)
                .Including(include => include.header)
                .Including(include => include.message)
                .Including(include => include.isSuccessful));
    }

    [Fact]
    public async Task ShouldReturnShortDmoTest() {
        //Arrange
        SetMockAndVariables();
        var loadedDmo = new LoadedShortDmoDto {
            id = "test dmo id",
            name = "test dmo name",
            dmoStatus = 1,
            movieTitle = "Some test movie",
            shortComment = "Short comment"
        };

        EditorServiceMock.Setup(esm => esm.LoadShortDmo(DmoDto, UserId)).ReturnsAsync(loadedDmo);
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
#pragma warning disable CS0612
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(DmoDto);
#pragma warning restore CS0612
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateSuccessfulResult(),
            config => config
                .Excluding(exclude => exclude.errors)
                .Excluding(exclude => exclude.warnings)
                .Excluding(exclude => exclude.message));

        // ReSharper disable once PossibleNullReferenceException
        response.GetType().GetProperty("data")!.GetValue(response).Should().BeEquivalentTo(loadedDmo);
    }

    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.LoadShortDmo(DmoDto, UserId))
            .ThrowsAsync(new LoadShortDmoException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
#pragma warning disable CS0612
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(DmoDto);
#pragma warning restore CS0612
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(
            BaseEditorResponseDto.CreateInternalServerErrorResponse(
                $"{LoadShortDmoException.CustomMessage} {exceptionMessage}"),
            config => config
                .Excluding(exclude => exclude.warnings));
    }
    
        
    [Fact]
    public async Task ShouldDisconnectUserIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.LoadShortDmo(DmoDto, UserId))
            .ThrowsAsync(new LoadShortDmoException(exceptionMessage, new Exception("exception from repository")));

        UserRepositoryMock
            .Setup(repository => repository.RemoveUserConnectionsAsync(EditorConnection.UserId, CancellationToken.None))
            .Verifiable();
        
        UserRepositoryMock
            .Setup(repository => repository.SyncContextImmediatelyAsync(CancellationToken.None))
            .Verifiable();

        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
#pragma warning disable CS0612
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(DmoDto);
#pragma warning restore CS0612
        await act.Invoke();

        //Assert
        UserRepositoryMock.Verify(sbj => sbj.RemoveUserConnectionsAsync(
                It.Is<Guid>(userId => userId == EditorConnection.UserId), CancellationToken.None), 
            Times.Once());
        UserRepositoryMock.Verify(sbj => sbj.SyncContextImmediatelyAsync(CancellationToken.None), Times.Once());
        Subject.Context.Items.Should().NotContainKey("user");
    }
}