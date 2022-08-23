﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Model.DTOs.Editor;
using Model.Exceptions.Editor;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using Xunit;

namespace Tests.EditorHubTests; 
public class UpdateShortDmoTests : BaseEditorTests {

    private UpdateShortDmoDto DmoDto { get; set; }

    private void SetMockAndVariables() {
        SetupConstructorMocks();
        DmoDto = new UpdateShortDmoDto {
            Id = Guid.NewGuid().ToString(),
            MovieTitle = "movie title",
            Name = "dmo name",
            ShortComment = "some comment"
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
        Func<Task> act = async () => await Subject.UpdateShortDmo(null);
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
        Func<Task> act = async () => await Subject.UpdateShortDmo(DmoDto);
        await act.Invoke();
        
        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
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
        Func<Task> act = async () => await Subject.UpdateShortDmo(DmoDto);
        await act.Invoke();
        
        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
        
    [Fact]
    public async Task ShouldReturnNoContentResponseTest() {
        //Arrange
        SetMockAndVariables();
        
        EditorServiceMock.Setup(esm => esm.UpdateShortDmo(DmoDto, UserId)).Verifiable();
        Subject = new EditorHub(
            EditorServiceMock.Object, 
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();
        
        //Act
        Func<Task> act = async () => await Subject.UpdateShortDmo(DmoDto);
        await act.Invoke();
        
        //Assert
        EditorServiceMock.Verify(esm => esm.UpdateShortDmo(DmoDto, UserId), Times.Once);
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Never());
    }
        
    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";
        
        EditorServiceMock.Setup(esm => esm.UpdateShortDmo(DmoDto, UserId))
            .ThrowsAsync(new UpdateShortDmoException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object, 
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();
        
        //Act
        Func<Task> act = async () => await Subject.UpdateShortDmo(DmoDto);
        await act.Invoke();
        
        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
}