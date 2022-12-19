using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NNA.Api.Features.Editor.Hubs;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubTests;

public sealed class SanitizeTempIdsTests : BaseEditorTests {
    private SanitizeTempIdsDto update { get; set; } = null!;

    private void SetMockAndVariables() {
        SetupConstructorMocks();
        update = new SanitizeTempIdsDto { dmoId = "someDmoId" };
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
#pragma warning disable CS0618
        Func<Task> act = async () => await Subject.SanitizeTempIds(null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        Func<Task> act = async () => await Subject.SanitizeTempIds(update);
#pragma warning restore CS0618
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
        //Arrange
        SetMockAndVariables();
        update.dmoId = string.Empty;
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
#pragma warning disable CS0618
        Func<Task> act = async () => await Subject.SanitizeTempIds(update);
#pragma warning restore CS0618
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldCallServiceMethodToSanitizeTempIdsTest() {
        //Arrange
        SetMockAndVariables();

        EditorServiceMock.Setup(esm => esm.SanitizeTempIds(update, UserId)).Verifiable();
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
#pragma warning disable CS0618
        Func<Task> act = async () => await Subject.SanitizeTempIds(update);
#pragma warning restore CS0618
        await act.Invoke();

        //Assert
        EditorServiceMock.Verify(esm => esm.SanitizeTempIds(update, UserId), Times.Once);
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Never());
    }
    
    [Fact]
    public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.SanitizeTempIds(update, UserId))
            .ThrowsAsync(new SanitizeTempIdsException(exceptionMessage, new Exception("exception from repository")));
        Subject = new EditorHub(
            EditorServiceMock.Object,
            EnvironmentMock.Object,
            ClaimsValidatorMock.Object,
            UserRepositoryMock.Object);
        SetupHubContext();

        //Act
#pragma warning disable CS0618
        Func<Task> act = async () => await Subject.SanitizeTempIds(update);
#pragma warning restore CS0618
        await act.Invoke();

        //Assert
        EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
    }
    
    [Fact]
    public async Task ShouldDisconnectUserIfRepositoryThrowsTest() {
        //Arrange
        SetMockAndVariables();
        var exceptionMessage = "some message";

        EditorServiceMock.Setup(esm => esm.SanitizeTempIds(update, UserId))
            .ThrowsAsync(new SanitizeTempIdsException(exceptionMessage, new Exception("exception from repository")));

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
#pragma warning disable CS0618
        Func<Task> act = async () => await Subject.SanitizeTempIds(update);
#pragma warning restore CS0618
        await act.Invoke();

        //Assert
        UserRepositoryMock.Verify(sbj => sbj.RemoveUserConnectionsAsync(
                It.Is<Guid>(userId => userId == EditorConnection.UserId), CancellationToken.None), 
            Times.Once());
        UserRepositoryMock.Verify(sbj => sbj.SyncContextImmediatelyAsync(CancellationToken.None), Times.Once());
        Subject.Context.Items.Should().NotContainKey("user");
    }
    
}
