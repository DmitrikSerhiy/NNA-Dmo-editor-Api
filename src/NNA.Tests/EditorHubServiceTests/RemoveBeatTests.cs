using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public class RemoveBeatTests : BaseHubServiceTests {
    // ReSharper disable once InconsistentNaming
    private Guid userId { get; set; }

    // ReSharper disable once InconsistentNaming
    private RemoveBeatDto beatDto { get; set; } = null!;
    private Beat BeatToRemove { get; set; } = null!;

    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        userId = Guid.NewGuid();
        beatDto = new RemoveBeatDto {
            Order = 0,
            DmoId = Guid.NewGuid().ToString(),
            Id = Guid.NewGuid().ToString()
        };
        BeatToRemove = new Beat();
    }

    [Fact]
    public void ShouldThrowWithInvalidEntryParamsTest() {
        //Arrange
        SetupMocksAndVariables();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act1 = async () => await Subject.RemoveBeat(null, userId);
        Func<Task> act2 = async () => await Subject.RemoveBeat(beatDto, Guid.Empty);

        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(beatDto));
        act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(userId));
    }

    [Fact]
    public void ShouldThrowIfDmoIdIsInvalidGuidTest() {
        //Arrange
        SetupMocksAndVariables();
        beatDto.DmoId = "invalid guid";
        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await subject.RemoveBeat(beatDto, userId);

        //Assert
        // ReSharper disable once PossibleNullReferenceException
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<DeleteBeatException>().Result
            .And.Message.Should().StartWith(DeleteBeatException.CustomMessage);
    }

    [Fact]
    public async Task ShouldDeleteBeatByMapperAndDeleteByIdIfBeatIdIsValidGuidTest() {
        //Arrange
        SetupMocksAndVariables();
        MapperMock.Setup(m => m.Map<Beat>(beatDto)).Returns(BeatToRemove);
        RepositoryMock.Setup(m => m.DeleteBeatByTempIdAsync(BeatToRemove)).Verifiable();
        RepositoryMock.Setup(rm => rm.DeleteBeatByIdAsync(BeatToRemove, Guid.Parse(beatDto.Id!))).ReturnsAsync(true);
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.RemoveBeat(beatDto, userId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(sbj => sbj.DeleteBeatByTempIdAsync(It.IsAny<Beat>()), Times.Never());
        RepositoryMock.Verify(sbj => sbj.DeleteBeatByIdAsync(It.IsAny<Beat>(), It.IsAny<Guid>()), Times.Once());
        MapperMock.Verify(sbj => sbj.Map<Beat>(It.IsAny<RemoveBeatDto>()), Times.Once());
    }

    [Fact]
    public async Task ShouldDeleteBeatWithoutMapperAndDeleteByTempIdIfBeatIdIsStringTest() {
        //Arrange
        SetupMocksAndVariables();
        beatDto.Id = "TempIdString";
        MapperMock.Setup(m => m.Map<Beat>(beatDto)).Verifiable();
        RepositoryMock.Setup(m => m.DeleteBeatByTempIdAsync(It.IsAny<Beat>())).ReturnsAsync(true);
        RepositoryMock.Setup(rm => rm.DeleteBeatByIdAsync(BeatToRemove, It.IsAny<Guid>())).Verifiable();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.RemoveBeat(beatDto, userId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(sbj => sbj.DeleteBeatByTempIdAsync(It.IsAny<Beat>()), Times.Once());
        RepositoryMock.Verify(sbj => sbj.DeleteBeatByIdAsync(It.IsAny<Beat>(), It.IsAny<Guid>()), Times.Never());
        MapperMock.Verify(sbj => sbj.Map<Beat>(It.IsAny<RemoveBeatDto>()), Times.Never());
    }

    [Fact]
    public void ShouldHandleRepositoryExceptionTest() {
        //Arrange
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        MapperMock.Setup(m => m.Map<Beat>(beatDto)).Returns(BeatToRemove);
        RepositoryMock.Setup(rm => rm.DeleteBeatByIdAsync(BeatToRemove, It.IsAny<Guid>()))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await Subject.RemoveBeat(beatDto, userId);

        //Assert
        // ReSharper disable once PossibleNullReferenceException
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<DeleteBeatException>().Result
            .And.InnerException!.Message.Should().Be(repositoryExceptionMessage);
    }

    [Fact]
    public void ShouldThrowIfDmoWasNotUpdatedTest() {
        //Arrange
        SetupMocksAndVariables();
        MapperMock.Setup(m => m.Map<Beat>(beatDto)).Returns(BeatToRemove);
        RepositoryMock.Setup(rm => rm.DeleteBeatByIdAsync(BeatToRemove, It.IsAny<Guid>())).ReturnsAsync(false);
        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await subject.RemoveBeat(beatDto, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<DeleteBeatException>().Result
            .And.InnerException.Should().BeNull();
    }
}