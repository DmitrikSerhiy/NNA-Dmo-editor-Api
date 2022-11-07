using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public sealed class UpdateDmoBeatsAsJsonTests : BaseHubServiceTests {
    // ReSharper disable once InconsistentNaming
    private Guid userId { get; set; }

    // ReSharper disable once InconsistentNaming
    private UpdateDmoBeatsAsJsonDto dmoDto { get; set; } = null!;

    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        userId = Guid.NewGuid();
        dmoDto = new UpdateDmoBeatsAsJsonDto { DmoId = Guid.NewGuid().ToString(), Data = "{}" };
    }


    [Fact]
    public void ShouldThrowWithInvalidEntryParamsTest() {
        //Arrange
        SetupMocksAndVariables();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act1 = async () => await Subject.UpdateDmoBeatsAsJson(null, userId);
        Func<Task> act2 = async () => await Subject.UpdateDmoBeatsAsJson(dmoDto, Guid.Empty);

        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(dmoDto));
        act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(userId));
    }

    [Fact]
    public void ShouldThrowIfDmoIdIsInvalidGuidTest() {
        //Arrange
        SetupMocksAndVariables();
        dmoDto.DmoId = "invalid guid";
        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await subject.UpdateDmoBeatsAsJson(dmoDto, userId);

        //Assert
        // ReSharper disable once PossibleNullReferenceException
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<UpdateDmoBeatsAsJsonException>().Result
            .And.Message.Should().StartWith(UpdateDmoBeatsAsJsonException.CustomMessage);
    }


    [Fact]
    public void ShouldHandleRepositoryExceptionTest() {
        //Arrange
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        RepositoryMock.Setup(rm => rm.UpdateJsonBeatsAsync(dmoDto.Data, Guid.Parse(dmoDto.DmoId!), userId))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));
        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await subject.UpdateDmoBeatsAsJson(dmoDto, userId);

        //Assert
        // ReSharper disable once PossibleNullReferenceException
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<UpdateDmoBeatsAsJsonException>().Result
            .And.InnerException!.Message.Should().Be(repositoryExceptionMessage);
    }

    [Fact]
    public void ShouldThrowIfDmoWasNotUpdatedTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock.Setup(rm => rm.UpdateJsonBeatsAsync(dmoDto.Data, Guid.Parse(dmoDto.DmoId!), userId))
            .ReturnsAsync(false);

        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await subject.UpdateDmoBeatsAsJson(dmoDto, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<UpdateDmoBeatsAsJsonException>().Result
            .And.InnerException.Should().BeNull();
    }
}