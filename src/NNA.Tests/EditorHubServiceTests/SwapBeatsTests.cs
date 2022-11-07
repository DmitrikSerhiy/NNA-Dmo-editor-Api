using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public sealed class SwapBeatsTests : BaseHubServiceTests {
    private Guid userId { get; set; }
    private string dmoId { get; set; } = null!;
    private SwapBeatsDto update { get; set; } = null!;
    private BeatToSwapDto beatToMove { get; set; } = null!;
    private BeatToSwapDto beatToReplace { get; set; } = null!;
    private string beatIdToMove { get; set; } = null!;
    private string beatIdToReplace { get; set; } = null!;

    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        dmoId = Guid.NewGuid().ToString();
        userId = Guid.NewGuid();
        beatIdToMove = Guid.NewGuid().ToString();
        beatIdToReplace = Guid.NewGuid().ToString();
        beatToMove = new BeatToSwapDto { id = beatIdToMove, order = 0};
        beatToReplace = new BeatToSwapDto { id = beatIdToReplace, order = 1};
        update = new SwapBeatsDto {
            dmoId = dmoId,
            beatToMove = beatToMove,
            beatToReplace = beatToReplace
        };
    }
    
    
    [Fact]
    public void ShouldThrowWithInvalidEntryParamsTest() {
        //Arrange
        SetupMocksAndVariables();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act1 = async () => await Subject.SwapBeats(null!, userId);
        Func<Task> act2 = async () => await Subject.SwapBeats(update, Guid.Empty);

        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(update));
        act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(userId));
    }
    
    [Fact]
    public async Task ShouldSetBeatsOrderByBeatIdsBeforeRepositoryIsCalledTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock.Setup(rm => rm.SetBeatOrderByIdAsync(It.IsAny<Beat>()))
            .ReturnsAsync(true)
            .Verifiable();
        RepositoryMock.Setup(rm => rm.SetBeatOrderByTempIdAsync(It.IsAny<Beat>())).Verifiable();

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.SwapBeats(update, userId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(rep => rep.SetBeatOrderByIdAsync(It.Is<Beat>(beat => beat.Order == beatToMove.order && beat.Id.ToString() == beatIdToReplace)), Times.Once);
        RepositoryMock.Verify(rep => rep.SetBeatOrderByIdAsync(It.Is<Beat>(beat => beat.Order == beatToReplace.order && beat.Id.ToString() == beatIdToMove)), Times.Once);
        RepositoryMock.Verify(rep => rep.SetBeatOrderByTempIdAsync(It.IsAny<Beat>()), Times.Never);
    }
    
    [Fact]
    public async Task ShouldSetBeatsOrderByBeatTempIdsBeforeRepositoryIsCalledTest() {
        //Arrange
        SetupMocksAndVariables();
        beatIdToReplace = "tempBeatToMoveId";
        beatIdToMove = "tempBeatToReplaceId";
        beatToMove = new BeatToSwapDto { id = beatIdToMove, order = 0};
        beatToReplace = new BeatToSwapDto { id = beatIdToReplace, order = 1};
        update.beatToMove = beatToMove;
        update.beatToReplace = beatToReplace;
        
        RepositoryMock.Setup(rm => rm.SetBeatOrderByTempIdAsync(It.IsAny<Beat>()))
            .ReturnsAsync(true)
            .Verifiable();
        RepositoryMock.Setup(rm => rm.SetBeatOrderByIdAsync(It.IsAny<Beat>())).Verifiable();

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.SwapBeats(update, userId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(rep => rep.SetBeatOrderByTempIdAsync(It.Is<Beat>(beat => beat.Order == beatToMove.order && beat.TempId == beatIdToReplace)), Times.Once);
        RepositoryMock.Verify(rep => rep.SetBeatOrderByTempIdAsync(It.Is<Beat>(beat => beat.Order == beatToReplace.order && beat.TempId == beatIdToMove)), Times.Once);
        RepositoryMock.Verify(rep => rep.SetBeatOrderByIdAsync(It.IsAny<Beat>()), Times.Never);
    }
    
    [Fact]
    public async Task ShouldSetBeatsOrderByBeatTempIdsAndByIdsBeforeRepositoryIsCalledTest() {
        //Arrange
        SetupMocksAndVariables();
        beatIdToReplace = "tempBeatToMoveId";
        beatToReplace = new BeatToSwapDto { id = beatIdToReplace, order = 1};
        update.beatToReplace = beatToReplace;
        
        RepositoryMock.Setup(rm => rm.SetBeatOrderByTempIdAsync(It.IsAny<Beat>()))
            .ReturnsAsync(true)
            .Verifiable();
        RepositoryMock.Setup(rm => rm.SetBeatOrderByIdAsync(It.IsAny<Beat>()))
            .ReturnsAsync(true)
            .Verifiable();

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.SwapBeats(update, userId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(rep => rep.SetBeatOrderByTempIdAsync(It.Is<Beat>(beat => beat.Order == beatToMove.order && beat.TempId == beatIdToReplace)), Times.Once);
        RepositoryMock.Verify(rep => rep.SetBeatOrderByIdAsync(It.Is<Beat>(beat => beat.Order == beatToReplace.order && beat.Id.ToString() == beatIdToMove)), Times.Once);
    }
    
    [Fact]
    public void ShouldHandleRepositoryExceptionTest() {
        //Arrange
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        RepositoryMock.Setup(rm => rm.SetBeatOrderByIdAsync(It.IsAny<Beat>()))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await Subject.SwapBeats(update, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<SwapBeatsException>().Result
            .And.InnerException!.Message.Should().Be(repositoryExceptionMessage);
    }
    
    [Fact]
    public void ShouldThrowIfDmoWasNotUpdatedTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock.Setup(rm => rm.SetBeatOrderByIdAsync(It.IsAny<Beat>()))
            .ReturnsAsync(false);
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await Subject.SwapBeats(update, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<SwapBeatsException>().Result
            .And.InnerException.Should().BeNull();
    }
}
