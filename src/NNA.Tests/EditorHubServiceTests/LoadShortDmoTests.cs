using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public sealed class LoadShortDmoTests : BaseHubServiceTests {
    // ReSharper disable once InconsistentNaming
    private Guid userId { get; set; }

    // ReSharper disable once InconsistentNaming
    private LoadShortDmoDto dmoDto { get; set; } = null!;
    private Dmo InitialDmo { get; set; } = null!;

    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        userId = Guid.NewGuid();
        dmoDto = new LoadShortDmoDto();
        InitialDmo = new Dmo();
    }

    [Fact]
    public void ShouldThrowWithInvalidEntryParamsTest() {
        //Arrange
        SetupMocksAndVariables();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act1 = async () => await Subject.LoadShortDmo(null, userId);
        Func<Task> act2 = async () => await Subject.LoadShortDmo(dmoDto, Guid.Empty);

        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(dmoDto));
        act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(userId));
    }

    [Fact]
    public async Task ShouldSetUserIdBeforeRepositoryIsCalledTest() {
        //Arrange
        SetupMocksAndVariables();
        MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
        RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId)).ReturnsAsync(new Dmo());
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        await Subject.LoadShortDmo(dmoDto, userId);

        //Assert
        InitialDmo.NnaUserId.Should().Be(userId);
    }

    [Fact]
    public void ShouldHandleRepositoryExceptionTest() {
        //Arrange
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
        RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));
        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await subject.LoadShortDmo(dmoDto, userId);

        //Assert
        // ReSharper disable once PossibleNullReferenceException
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<LoadShortDmoException>().Result
            .And.InnerException!.Message.Should().Be(repositoryExceptionMessage);
    }

    [Fact]
    public void ShouldThrowIfDmoWasNotFoundTest() {
        //Arrange
        SetupMocksAndVariables();
        static Dmo? Dmo() => null;
        MapperMock.Setup(m => m.Map<Dmo?>(dmoDto)).Returns(InitialDmo);
        RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId))!.ReturnsAsync(Dmo);

        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await subject.LoadShortDmo(dmoDto, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<LoadShortDmoException>().Result
            .And.InnerException.Should().BeNull();
    }

    [Fact]
    public void ShouldReturnDmoTest() {
        //Arrange
        SetupMocksAndVariables();
        var dmo = new Dmo();
        var searchedDmoDto = new LoadedShortDmoDto();
        MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
        RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId)).ReturnsAsync(dmo);
        MapperMock.Setup(m => m.Map<LoadedShortDmoDto>(dmo)).Returns(searchedDmoDto);
        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await subject.LoadShortDmo(dmoDto, userId);

        //Assert
        act.Should().NotThrowAsync();
        MapperMock.Verify(mm => mm.Map<LoadedShortDmoDto>(dmo), Times.Once);
    }
}