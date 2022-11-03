using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public class SanitizeTempIdsTests : BaseHubServiceTests {
    private Guid userId { get; set; }
    private Guid dmoId { get; set; }
    private SanitizeTempIdsDto update { get; set; } = null!;

    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        userId = Guid.NewGuid();
        dmoId = Guid.NewGuid();
        update = new SanitizeTempIdsDto {
            dmoId = dmoId.ToString()
        };
    }
    
    [Fact]
    public void ShouldThrowWithInvalidEntryParamsTest() {
        //Arrange
        SetupMocksAndVariables();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act1 = async () => await Subject.SanitizeTempIds(null!, userId);
        Func<Task> act2 = async () => await Subject.SanitizeTempIds(update, Guid.Empty);

        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(update));
        act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(userId));
    }

    [Fact]
    public void ShouldCallRepositoryMethodTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock.Setup(rm => rm.SanitizeTempIdsForBeatsAsync(dmoId, userId))
            .ReturnsAsync(true)
            .Verifiable();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.SanitizeTempIds(update, userId);
        act.Invoke();

        //Assert
        RepositoryMock.Verify(rep => rep.SanitizeTempIdsForBeatsAsync(dmoId, userId), Times.Once);
    }
    
    [Fact]
    public void ShouldHandleRepositoryExceptionTest() {
        //Arrange
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        RepositoryMock.Setup(rm => rm.SanitizeTempIdsForBeatsAsync(dmoId, userId))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await Subject.SanitizeTempIds(update, userId);

        //Assert
        // ReSharper disable once PossibleNullReferenceException
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<SanitizeTempIdsException>().Result
            .And.InnerException!.Message.Should().Be(repositoryExceptionMessage);
    }
    
    
    [Fact]
    public void ShouldThrowIfDmoWasNotUpdatedTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock.Setup(rm => rm.SanitizeTempIdsForBeatsAsync(dmoId, userId)).ReturnsAsync(false);
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await Subject.SanitizeTempIds(update, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<SanitizeTempIdsException>().Result
            .And.InnerException.Should().BeNull();
    }
    
}
