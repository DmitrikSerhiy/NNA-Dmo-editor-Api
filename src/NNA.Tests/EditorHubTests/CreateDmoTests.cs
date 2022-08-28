﻿using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.DTOs.Editor.Response;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubTests; 
public class CreateDmoTests : BaseEditorTests
{
    private CreateDmoDto DmoDto { get; set; } = null!;

    private void SetMockAndVariables()
    {
        SetupConstructorMocks();
        DmoDto = new CreateDmoDto {
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
        Func<Task<object>> act = async () => await Subject.CreateDmo(null);
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateBadRequestResponse(), 
            config => config
                .Excluding(exclude => exclude.errors)
                .Excluding(exclude => exclude.warnings)
                .Excluding(exclude => exclude.message)
        );
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
        Func<Task<object>> act = async () => await Subject.CreateDmo(DmoDto);
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
        DmoDto.Name = null;
        Subject = new EditorHub(
            EditorServiceMock.Object, 
            EnvironmentMock.Object, 
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task<object>> act = async () => await Subject.CreateDmo(DmoDto);
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateFailedValidationResponse(new List<Tuple<string, string>>()),
            config => config
                .Including(include => include.httpCode)
                .Including(include => include.header)
                .Including(include => include.message)
                .Including(include => include.isSuccessful));
    }

    [Fact]
    public async Task ShouldReturnCreatedDmoTest() {
        //Arrange
        SetMockAndVariables();
        var createdDmoDto = new CreatedDmoDto {
            id = "test dmo id",
            name = "test dmo name",
            dmoStatus = 1,
            movieTitle = "Some test movie",
            shortComment = "Short comment"
        };
            
        EditorServiceMock.Setup(esm => esm.CreateAndLoadDmo(DmoDto, UserId)).ReturnsAsync(createdDmoDto);
        Subject = new EditorHub(
            EditorServiceMock.Object, 
            EnvironmentMock.Object, 
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task<object>> act = async () => await Subject.CreateDmo(DmoDto);
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateSuccessfulResult(),
            config => config
                .Excluding(exclude => exclude.errors)
                .Excluding(exclude => exclude.warnings)
                .Excluding(exclude => exclude.message));
            
        // ReSharper disable once PossibleNullReferenceException
        response.GetType().GetProperty("data")!.GetValue(response).Should().BeEquivalentTo(createdDmoDto);
    }

    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfDmoWasNotCreatedTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";
            
        EditorServiceMock.Setup(esm => esm.CreateAndLoadDmo(DmoDto, UserId))
            .ThrowsAsync(new CreateDmoException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task<object>> act = async () => await Subject.CreateDmo(DmoDto);
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateInternalServerErrorResponse($"{CreateDmoException.CustomMessage} {exceptionMessage}"),
            config => config
                .Excluding(exclude => exclude.warnings));
    }

    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfDmoWasNotFoundAfterCreationTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.CreateAndLoadDmo(DmoDto, UserId))
            .ThrowsAsync(new LoadShortDmoException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object, 
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task<object>> act = async () => await Subject.CreateDmo(DmoDto);
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateInternalServerErrorResponse($"{LoadShortDmoException.CustomMessage} {exceptionMessage}"),
            config => config
                .Excluding(exclude => exclude.warnings));
    }
}