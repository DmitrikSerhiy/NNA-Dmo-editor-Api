using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using NNA.Domain.DTOs.CharactersInBeats;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubTests;

public sealed class DetachCharacterTests : BaseEditorTests {
    private DetachCharacterToBeatDto DetachCharacterFromBeatDto { get; set; } = null!;

    private void SetMockAndVariables() {
        SetupConstructorMocks();
        DetachCharacterFromBeatDto = new DetachCharacterToBeatDto {
            Id = Guid.NewGuid().ToString(),
            BeatId = Guid.NewGuid().ToString(),
            DmoId = Guid.NewGuid().ToString(),
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
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(null);
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
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterFromBeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldReturnNotValidResponseIfDmoIdIsMissingTest() {
        //Arrange
        SetMockAndVariables();
        DetachCharacterFromBeatDto.DmoId = null!;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterFromBeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldReturnNotValidResponseIfIdIsMissingTest() {
        //Arrange
        SetMockAndVariables();
        DetachCharacterFromBeatDto.Id = null!;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterFromBeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldReturnNotValidResponseIfBeatIdIsMissingTest() {
        //Arrange
        SetMockAndVariables();
        DetachCharacterFromBeatDto.BeatId = null!;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterFromBeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldCallServiceMethodToDetachCharacterFromBeatTest() {
        //Arrange
        SetMockAndVariables();

        EditorServiceMock.Setup(esm => esm.DetachCharacterFromBeat(DetachCharacterFromBeatDto, UserId)).Verifiable();
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterFromBeatDto);
        await act.Invoke();

        //Assert
        EditorServiceMock.Verify(esm => esm.DetachCharacterFromBeat(DetachCharacterFromBeatDto, UserId), Times.Once);
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Never());
    }
    
    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.DetachCharacterFromBeat(DetachCharacterFromBeatDto, UserId))
            .ThrowsAsync(new RemoveCharacterFromBeatException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterFromBeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    
    [Fact]
    public async Task ShouldDisconnectUserIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";
        EditorServiceMock.Setup(esm => esm.DetachCharacterFromBeat(DetachCharacterFromBeatDto, UserId))
            .ThrowsAsync(new RemoveCharacterFromBeatException(exceptionMessage, new Exception("exception from repository")));

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
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterFromBeatDto);
        await act.Invoke();

        //Assert
        UserRepositoryMock.Verify(sbj => sbj.RemoveUserConnectionsAsync(
                It.Is<Guid>(userId => userId == EditorConnection.UserId), CancellationToken.None), 
            Times.Once());
        UserRepositoryMock.Verify(sbj => sbj.SyncContextImmediatelyAsync(CancellationToken.None), Times.Once());
        Subject.Context.Items.Should().NotContainKey("user");
    }
}