using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubTests;

public class UpdateBeatTest : BaseEditorTests {
    private UpdateBeatDto BeatDto { get; set; } = null!;

    private void SetMockAndVariables() {
        SetupConstructorMocks();
        BeatDto = new UpdateBeatDto {
            BeatId = Guid.NewGuid().ToString(),
            Text = "Beat description",
            Time = new UpdateBeatTimeDto {
                Hours = 0,
                Minutes = 10,
                Seconds = 10
            }
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
        Func<Task> act = async () => await Subject.UpdateBeat(null);
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
        Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }


    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidDueToMissingBeatIdTest() {
        //Arrange
        SetMockAndVariables();
        BeatDto.BeatId = null;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidDueToInvalidMinutesBeatIdTest() {
        //Arrange
        SetMockAndVariables();
        BeatDto.Time.Minutes = 70;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidDueToInvalidSecondsBeatIdTest() {
        //Arrange
        SetMockAndVariables();
        BeatDto.Time.Seconds = 70;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }


    [Fact]
    public async Task ShouldReturnNoContentResponseTest() {
        //Arrange
        SetMockAndVariables();

        EditorServiceMock.Setup(esm => esm.UpdateBeat(BeatDto, UserId)).Verifiable();
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorServiceMock.Verify(esm => esm.UpdateBeat(BeatDto, UserId), Times.Once);
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Never());
    }


    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.UpdateBeat(BeatDto, UserId))
            .ThrowsAsync(new UpdateBeatException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldDisconnectUserIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.UpdateBeat(BeatDto, UserId))
            .ThrowsAsync(new UpdateBeatException(exceptionMessage, new Exception("exception from repository")));

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
        Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
        await act.Invoke();

        //Assert
        UserRepositoryMock.Verify(sbj => sbj.RemoveEditorConnection(
                It.Is<EditorConnection>(ec => ec.ConnectionId == EditorConnection.ConnectionId && ec.UserId == EditorConnection.UserId )), 
            Times.Once());
        UserRepositoryMock.Verify(sbj => sbj.SyncContextImmediatelyAsync(CancellationToken.None), Times.Once());
        Subject.Context.Items.Should().NotContainKey("user");
    }
}