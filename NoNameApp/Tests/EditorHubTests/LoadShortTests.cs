using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Model.DTOs.Editor;
using Model.DTOs.Editor.Response;
using Model.Exceptions.Editor;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NNA.Api.Features.Editor.Hubs;
using Xunit;

namespace Tests.EditorHubTests; 
public class LoadShortTests : BaseEditorTests {

    private LoadShortDmoDto DmoDto { get; set; }

    private void SetMockAndVariables() {
        SetupConstructorMocks();
        DmoDto = new LoadShortDmoDto {Id = Guid.NewGuid().ToString()};
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
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(null);
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
        hubContext.Setup(hm => hm.Items).Returns(new Dictionary<object, object>());
        Subject.Context = hubContext.Object;

        //Act
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(DmoDto);
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
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(DmoDto);
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
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(DmoDto);
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateSuccessfulResult(),
            config => config
                .Excluding(exclude => exclude.errors)
                .Excluding(exclude => exclude.warnings)
                .Excluding(exclude => exclude.message));

        // ReSharper disable once PossibleNullReferenceException
        response.GetType().GetProperty("data").GetValue(response).Should().BeEquivalentTo(loadedDmo);
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
        Func<Task<object>> act = async () => await Subject.LoadShortDmo(DmoDto);
        var response = await act.Invoke();

        //Assert
        response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateInternalServerErrorResponse($"{LoadShortDmoException.CustomMessage} {exceptionMessage}"),
            config => config
                .Excluding(exclude => exclude.warnings));
    }
}