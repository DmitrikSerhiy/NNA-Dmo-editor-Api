using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public sealed class MoveBeatTests: BaseHubServiceTests {
    private Guid userId { get; set; }
    private string dmoId { get; set; } = null!;
    private string beatId { get; set; } = null!;
    private MoveBeatDto update { get; set; } = null!;
    
    private int order { get; set; } = 1;
    private int previousOrder { get; set; } = 2;
    
    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        dmoId = Guid.NewGuid().ToString();
        userId = Guid.NewGuid();
        update = new MoveBeatDto {
            id = beatId,
            dmoId = dmoId,
            order = order,
            previousOrder = previousOrder
        };
    }
    
    [Fact]
    public void ShouldThrowWithInvalidEntryParamsTest() {
        //Arrange
        SetupMocksAndVariables();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act1 = async () => await Subject.MoveBeat(null!, userId);
        Func<Task> act2 = async () => await Subject.MoveBeat(update, Guid.Empty);

        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(update));
        act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(userId));
    }
    
    [Fact]
    public void ShouldNotCallRepositoryIfOrderEqualPreviousOrderTest() {
        //Arrange
        previousOrder = order;
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.ResetBeatsOrderByIdAsync(It.IsAny<Beat>(), It.IsAny<int>()))
            .ReturnsAsync(true)
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.ResetBeatsOrderByTempIdAsync(It.IsAny<Beat>(), It.IsAny<int>()))
            .ReturnsAsync(true)
            .Verifiable();
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.MoveBeat(update, userId);
        act.Invoke();

        //Assert
        RepositoryMock.Verify(rep => rep.ResetBeatsOrderByIdAsync(It.IsAny<Beat>(), It.IsAny<int>()), Times.Never);
        RepositoryMock.Verify(rep => rep.ResetBeatsOrderByTempIdAsync(It.IsAny<Beat>(), It.IsAny<int>()), Times.Never);
    }
    
    [Fact]
    public void ShouldMoveBeatByIdIfBeatIdIsGuidTest() {
        //Arrange
        beatId = Guid.NewGuid().ToString();
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.ResetBeatsOrderByIdAsync(It.Is<Beat>(b => b.Id == Guid.Parse(beatId) && b.Order == update.order), update.previousOrder))
            .ReturnsAsync(true)
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.ResetBeatsOrderByTempIdAsync(It.IsAny<Beat>(), It.IsAny<int>()))
            .ReturnsAsync(true)
            .Verifiable();
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.MoveBeat(update, userId);
        act.Invoke();

        //Assert
        RepositoryMock.Verify(rep => rep.ResetBeatsOrderByIdAsync(It.Is<Beat>(b => b.Id == Guid.Parse(beatId) && b.Order == update.order), update.previousOrder), Times.Once);
        RepositoryMock.Verify(rep => rep.ResetBeatsOrderByTempIdAsync(It.IsAny<Beat>(), It.IsAny<int>()), Times.Never);
    }
    
    [Fact]
    public void ShouldMoveBeatByTempIdIfBeatIdIsGuidTest() {
        //Arrange
        beatId = "beat_tempId";
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.ResetBeatsOrderByIdAsync(It.IsAny<Beat>(), It.IsAny<int>()))
            .ReturnsAsync(true)
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.ResetBeatsOrderByTempIdAsync(It.Is<Beat>(b => b.TempId == beatId && b.Order == update.order), update.previousOrder))
            .ReturnsAsync(true)
            .Verifiable();
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.MoveBeat(update, userId);
        act.Invoke();

        //Assert
        RepositoryMock.Verify(rep => rep.ResetBeatsOrderByIdAsync(It.IsAny<Beat>(), It.IsAny<int>()), Times.Never);
        RepositoryMock.Verify(rep => rep.ResetBeatsOrderByTempIdAsync(It.Is<Beat>(b => b.TempId == beatId && b.Order == update.order), update.previousOrder), Times.Once);
    }
    
    
    [Fact]
    public void ShouldHandleRepositoryExceptionIfMoveByIdThrowsTest() {
        //Arrange
        beatId = Guid.NewGuid().ToString();
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        RepositoryMock.Setup(rm => rm.ResetBeatsOrderByIdAsync(It.IsAny<Beat>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await Subject.MoveBeat(update, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<MoveBeatException>().Result
            .And.InnerException!.Message.Should().Be(repositoryExceptionMessage);
    }
    
    [Fact]
    public void ShouldThrowIfBeatWasNotMovedByIdTest() {
        //Arrange
        beatId = Guid.NewGuid().ToString();
        SetupMocksAndVariables();
        RepositoryMock.Setup(rm => rm.ResetBeatsOrderByIdAsync(It.IsAny<Beat>(), It.IsAny<int>()))
            .ReturnsAsync(false);
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await Subject.MoveBeat(update, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<MoveBeatException>().Result
            .And.InnerException.Should().BeNull();
    }

    [Fact]
    public void ShouldHandleRepositoryExceptionIfMoveByTempIdThrowsTest() {
        //Arrange
        beatId = "beat_tempId";
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        RepositoryMock.Setup(rm => rm.ResetBeatsOrderByTempIdAsync(It.IsAny<Beat>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await Subject.MoveBeat(update, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<MoveBeatException>().Result
            .And.InnerException!.Message.Should().Be(repositoryExceptionMessage);
    }
    
    [Fact]
    public void ShouldThrowIfBeatWasNotMovedByTempIdTest() {
        //Arrange
        beatId = "beat_tempId";
        SetupMocksAndVariables();
        RepositoryMock.Setup(rm => rm.ResetBeatsOrderByTempIdAsync(It.IsAny<Beat>(), It.IsAny<int>()))
            .ReturnsAsync(false);
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await Subject.MoveBeat(update, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<MoveBeatException>().Result
            .And.InnerException.Should().BeNull();
    }
}
