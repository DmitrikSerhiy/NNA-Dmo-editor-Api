using Microsoft.AspNetCore.SignalR;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubTests;

public class UpdateDmosBeatsJsonTests : BaseEditorTests {
    // ReSharper disable once InconsistentNaming
    private UpdateDmoBeatsAsJsonDto update { get; set; } = null!;

    private void SetMockAndVariables() {
        SetupConstructorMocks();
        update = new UpdateDmoBeatsAsJsonDto { DmoId = Guid.NewGuid().ToString(), Data = "{}" };
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
        Func<Task> act = async () => await Subject.UpdateDmosJson(null);
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
        Func<Task> act = async () => await Subject.UpdateDmosJson(update);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }

    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
        //Arrange
        SetMockAndVariables();
        update.DmoId = null;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.UpdateDmosJson(update);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }

    [Fact]
    public async Task ShouldReturnNoContentResponseTest() {
        //Arrange
        SetMockAndVariables();

        EditorServiceMock.Setup(esm => esm.UpdateDmoBeatsAsJson(update, UserId)).Verifiable();
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.UpdateDmosJson(update);
        await act.Invoke();

        //Assert
        EditorServiceMock.Verify(esm => esm.UpdateDmoBeatsAsJson(update, UserId), Times.Once);
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Never());
    }


    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.UpdateDmoBeatsAsJson(update, UserId))
            .ThrowsAsync(
                new UpdateDmoBeatsAsJsonException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
        Func<Task> act = async () => await Subject.UpdateDmosJson(update);
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
}