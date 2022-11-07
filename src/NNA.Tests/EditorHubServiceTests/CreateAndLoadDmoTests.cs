using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public sealed class CreateAndLoadDmoTests : BaseHubServiceTests {
    // ReSharper disable once InconsistentNaming
    private Guid userId { get; set; }

    // ReSharper disable once InconsistentNaming
    private CreateDmoDto dmoDto { get; set; } = null!;
    private Dmo InitialDmo { get; set; } = null!;

    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        userId = Guid.NewGuid();
        dmoDto = new CreateDmoDto();
        InitialDmo = new Dmo();
    }

    [Fact]
    public void ShouldThrowWithInvalidEntryParamsTest() {
        //Arrange
        SetupMocksAndVariables();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
#pragma warning disable CS0612
        Func<Task> act1 = async () => await Subject.CreateAndLoadDmo(null, userId);
        Func<Task> act2 = async () => await Subject.CreateAndLoadDmo(dmoDto, Guid.Empty);
#pragma warning restore CS0612
        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(dmoDto));
        act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(userId));
    }

    [Fact]
    public async Task ShouldSetUserIdBeforeRepositoryIsCalledTest() {
        //Arrange
        SetupMocksAndVariables();
        MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
        RepositoryMock.Setup(rm => rm.CreateDmoAsync(InitialDmo)).ReturnsAsync(true);
        RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId)).ReturnsAsync(new Dmo());
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
#pragma warning disable CS0612
        await Subject.CreateAndLoadDmo(dmoDto, userId);
#pragma warning restore CS0612

        //Assert
        InitialDmo.NnaUserId.Should().Be(userId);
    }

    [Fact]
    public void ShouldThrowIfDmoWasNotCreatedTest() {
        //Arrange
        SetupMocksAndVariables();
        MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
        RepositoryMock.Setup(rm => rm.CreateDmoAsync(InitialDmo)).ReturnsAsync(false);
        RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId)).ReturnsAsync(new Dmo());

        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
#pragma warning disable CS0612
        async Task Act() => await subject.CreateAndLoadDmo(dmoDto, userId);
#pragma warning restore CS0612

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<CreateDmoException>().Result
            .And.InnerException.Should().BeNull();
    }


    [Fact]
    public void ShouldHandleRepositoryExceptionOnDmoCreateTest() {
        //Arrange
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
        RepositoryMock.Setup(rm => rm.CreateDmoAsync(InitialDmo))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));
        RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId)).ReturnsAsync(new Dmo());
        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
#pragma warning disable CS0612
        async Task Act() => await subject.CreateAndLoadDmo(dmoDto, userId);
#pragma warning restore CS0612

        //Assert
        // ReSharper disable once PossibleNullReferenceException
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<CreateDmoException>().Result
            .And.InnerException!.Message.Should().Be(repositoryExceptionMessage);
    }

    [Fact]
    public void ShouldThrowIfDmoWasNotFoundAfterCreationTest() {
        //Arrange
        SetupMocksAndVariables();
        static Dmo? Dmo() => null;
        MapperMock.Setup(m => m.Map<Dmo?>(dmoDto)).Returns(InitialDmo);
        RepositoryMock.Setup(rm => rm.CreateDmoAsync(InitialDmo)).ReturnsAsync(true);
        RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId))!.ReturnsAsync(Dmo);

        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
#pragma warning disable CS0612
        async Task Act() => await subject.CreateAndLoadDmo(dmoDto, userId);
#pragma warning restore CS0612

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<LoadShortDmoException>().Result
            .And.InnerException.Should().BeNull();
    }


    [Fact]
    public void ShouldHandleRepositoryExceptionOnDmoLoadAfterCreationTest() {
        //Arrange
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
        RepositoryMock.Setup(rm => rm.CreateDmoAsync(InitialDmo)).ReturnsAsync(true);
        RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));

        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
#pragma warning disable CS0612
        async Task Act() => await subject.CreateAndLoadDmo(dmoDto, userId);
#pragma warning restore CS0612

        //Assert
        // ReSharper disable once PossibleNullReferenceException
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<LoadShortDmoException>().Result
            .And.InnerException!.Message.Should().Be(repositoryExceptionMessage);
    }
}