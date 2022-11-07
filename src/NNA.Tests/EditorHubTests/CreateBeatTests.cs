using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubTests;

public sealed class CreateBeatTests : BaseEditorTests {
    private CreateBeatDto BeatDto { get; set; } = null!;

    private void SetMockAndVariables() {
        SetupConstructorMocks();
        BeatDto = new CreateBeatDto {
            Order = 0,
            DmoId = Guid.NewGuid().ToString(),
            TempId = "TempId"
        };
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
        Func<Task> act = async () => await Subject.CreateBeat(null);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
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
        SetupHubContext();
        var hubContext = new Mock<HubCallerContext>();
        hubContext.Setup(hm => hm.Items).Returns(new Dictionary<object, object?>());
        Subject.Context = hubContext.Object;

        //Act
        Func<Task> act = async () => await Subject.CreateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }


    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
        //Arrange
        SetMockAndVariables();
        BeatDto.DmoId = null;
        BeatDto.TempId = null;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.CreateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }


    [Fact]
    public async Task ShouldCallServiceMethodToCreateBeatTest() {
        //Arrange
        SetMockAndVariables();

        EditorServiceMock.Setup(esm => esm.CreateBeat(BeatDto, UserId)).Verifiable();
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.CreateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorServiceMock.Verify(esm => esm.CreateBeat(BeatDto, UserId), Times.Once);
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Never());
    }


    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.CreateBeat(BeatDto, UserId))
            .ThrowsAsync(new InsertNewBeatException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.CreateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldDisconnectUserIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.CreateBeat(BeatDto, UserId))
            .ThrowsAsync(new InsertNewBeatException(exceptionMessage, new Exception("exception from repository")));

        UserRepositoryMock
            .Setup(repository => repository.RemoveEditorConnection(EditorConnection))
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
        Func<Task> act = async () => await Subject.CreateBeat(BeatDto);
        await act.Invoke();

        //Assert
        UserRepositoryMock.Verify(sbj => sbj.RemoveEditorConnection(
                It.Is<EditorConnection>(ec => ec.ConnectionId == EditorConnection.ConnectionId && ec.UserId == EditorConnection.UserId )), 
            Times.Once());
        UserRepositoryMock.Verify(sbj => sbj.SyncContextImmediatelyAsync(CancellationToken.None), Times.Once());
        Subject.Context.Items.Should().NotContainKey("user");
    }
}