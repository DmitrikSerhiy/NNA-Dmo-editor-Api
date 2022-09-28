﻿using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubTests;

public class SwapBeatsTests : BaseEditorTests {

    private SwapBeatsDto SwapBeatsDto { get; set; } = null!;
    
    private void SetMockAndVariables() {
        SetupConstructorMocks();
        SwapBeatsDto = new SwapBeatsDto {
            dmoId = Guid.NewGuid().ToString(),
            beatToMove = new BeatToSwapDto { id = "beatIdToMove", order = 1 },
            beatToReplace = new BeatToSwapDto { id = "beatIdToReplace", order = 2 }
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
        Func<Task> act = async () => await Subject.SwapBeats(null);
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
        Func<Task> act = async () => await Subject.SwapBeats(SwapBeatsDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
        //Arrange
        SetMockAndVariables();
        SwapBeatsDto.dmoId = string.Empty;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.SwapBeats(SwapBeatsDto);
        await act.Invoke();
        
        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidDueToMissingBeatToMoveIdTest() {
        //Arrange
        SetMockAndVariables();
        SwapBeatsDto.beatToMove.id = string.Empty;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.SwapBeats(SwapBeatsDto);
        await act.Invoke();
        
        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidDueToMissingBeatToReplaceIdTest() {
        //Arrange
        SetMockAndVariables();
        SwapBeatsDto.beatToMove.id = string.Empty;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.SwapBeats(SwapBeatsDto);
        await act.Invoke();
        
        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldCallServiceMethodToSwapBeatsTest() {
        //Arrange
        SetMockAndVariables();
        EditorServiceMock.Setup(esm => esm.SwapBeats(SwapBeatsDto, UserId)).Verifiable();
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.SwapBeats(SwapBeatsDto);
        await act.Invoke();
        
        //Assert
        EditorServiceMock.Verify(esm => esm.SwapBeats(SwapBeatsDto, UserId), Times.Once);
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Never());
    }
    
    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.SwapBeats(SwapBeatsDto, UserId))
            .ThrowsAsync(new SwapBeatsException(exceptionMessage, new Exception("exception from repository")));

        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.SwapBeats(SwapBeatsDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldDisconnectUserIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.SwapBeats(SwapBeatsDto, UserId))
            .ThrowsAsync(new SwapBeatsException(exceptionMessage, new Exception("exception from repository")));

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
        Func<Task> act = async () => await Subject.SwapBeats(SwapBeatsDto);
        await act.Invoke();

        //Assert
        UserRepositoryMock.Verify(sbj => sbj.RemoveEditorConnection(
                It.Is<EditorConnection>(ec => ec.ConnectionId == EditorConnection.ConnectionId && ec.UserId == EditorConnection.UserId )), 
            Times.Once());
        UserRepositoryMock.Verify(sbj => sbj.SyncContextImmediatelyAsync(CancellationToken.None), Times.Once());
        Subject.Context.Items.Should().NotContainKey("user");
    }
}