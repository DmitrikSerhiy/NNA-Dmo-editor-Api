using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Model.DTOs.Editor;
using Model.Exceptions.Editor;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using Xunit;

namespace Tests.EditorHubTests; 
public class DeleteBeatTest: BaseEditorTests {
    private RemoveBeatDto BeatDto { get; set; }
        
    private void SetMockAndVariables()
    {
        SetupConstructorMocks();
        BeatDto = new RemoveBeatDto {
            Order = 0,
            DmoId = Guid.NewGuid().ToString(),
            Id = Guid.NewGuid().ToString()
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
        Func<Task> act = async () => await Subject.RemoveBeat(null);
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
        hubContext.Setup(hm => hm.Items).Returns(new Dictionary<object, object>());
        Subject.Context = hubContext.Object;
        
        //Act
        Func<Task> act = async () => await Subject.RemoveBeat(BeatDto);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
        
        
    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
        //Arrange
        SetMockAndVariables();
        BeatDto.DmoId = null;
        BeatDto.Id = null;
        Subject = new EditorHub(
            EditorServiceMock.Object, 
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();
        
        //Act
        Func<Task> act = async () => await Subject.RemoveBeat(BeatDto);
        await act.Invoke();
        
        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
        
        
    [Fact]
    public async Task ShouldReturnNoContentResponseTest() {
        //Arrange
        SetMockAndVariables();
        
        EditorServiceMock.Setup(esm => esm.RemoveBeat(BeatDto, UserId)).Verifiable();
        Subject = new EditorHub(
            EditorServiceMock.Object, 
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();
        
        //Act
        Func<Task> act = async () => await Subject.RemoveBeat(BeatDto);
        await act.Invoke();
        
        //Assert
        EditorServiceMock.Verify(esm => esm.RemoveBeat(BeatDto, UserId), Times.Once);
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Never());
    }
        
                
    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";
        
        EditorServiceMock.Setup(esm => esm.RemoveBeat(BeatDto, UserId))
            .ThrowsAsync(new DeleteBeatException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object, 
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();
        
        //Act
        Func<Task> act = async () => await Subject.RemoveBeat(BeatDto);
        await act.Invoke();
        
        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
}