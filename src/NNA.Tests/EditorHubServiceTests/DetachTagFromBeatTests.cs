using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.TagsInBeats;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public sealed class DetachTagFromBeatTests : BaseHubServiceTests{
    private DetachTagFromBeatDto DetachTagFromBeatDto { get; set; } = null!; 

    private Guid UserId { get; set; }
    
    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        DetachTagFromBeatDto = new DetachTagFromBeatDto {
            Id = Guid.NewGuid().ToString(),
            BeatId = Guid.NewGuid().ToString(),
            DmoId = Guid.NewGuid().ToString()
        };
        UserId = Guid.NewGuid();
    }
    
    
    [Fact]
    public void ShouldThrowWithInvalidEntryParamsTest() {
        //Arrange
        SetupMocksAndVariables();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
#pragma warning disable CS0612
        Func<Task> act1 = async () => await Subject.DetachTagFromBeat(null!, UserId);
        Func<Task> act2 = async () => await Subject.DetachTagFromBeat(DetachTagFromBeatDto, Guid.Empty);
#pragma warning restore CS0612
        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be("detachTagFromBeatDto");
        act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be("userId");
    }
    
    [Fact]
    public async Task ShouldNotLoadBeatIfBeatIdIsGuidTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.DetachTagFromBeat(DetachTagFromBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task ShouldLoadBeatByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        DetachTagFromBeatDto.BeatId = "TempId123";
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.DetachTagFromBeat(DetachTagFromBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r.LoadBeatIdByTempId(Guid.Parse(DetachTagFromBeatDto.DmoId), DetachTagFromBeatDto.BeatId, UserId), Times.Once);
    }
    
    [Fact]
    public async Task ShouldCallRepositoryMethodToDetachTagWithDataProvidedInDtoByIdTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.DetachTagFromBeat(DetachTagFromBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r
            .DeleteTagFromBeatByIdAsync(It.Is<NnaTagInBeat>(beat => 
                beat.Id == Guid.Parse(DetachTagFromBeatDto.Id) &&
                beat.BeatId == Guid.Parse(DetachTagFromBeatDto.BeatId) &&
                beat.TempId == null)
            ), Times.Once);
        
        RepositoryMock.Verify(r => r
            .DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()), Times.Never);
    }
    
    [Fact]
    public async Task ShouldCallRepositoryMethodToDetachTagWithDataProvidedInDtoByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        DetachTagFromBeatDto.Id = "TempId123";
        var beatId = Guid.NewGuid();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(beatId);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.DetachTagFromBeat(DetachTagFromBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r
            .DeleteTagFromBeatByTempIdAsync(It.Is<NnaTagInBeat>(beat => 
                beat.TempId == DetachTagFromBeatDto.Id &&
                beat.BeatId == Guid.Parse(DetachTagFromBeatDto.BeatId))
            ), Times.Once);
        
        RepositoryMock.Verify(r => r
            .DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()), Times.Never);
    }
    
    [Fact]
    public void ShouldHandleRepositoryExceptionAndThrowServiceExceptionByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        DetachTagFromBeatDto.Id = "TempId123";
        var exceptionMessage = "Message from repository";
        var beatId = Guid.NewGuid();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(beatId);

        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()))
            .ThrowsAsync(new Exception(exceptionMessage));        
        
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.DetachTagFromBeat(DetachTagFromBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<DetachTagFromBeatException>().Result
            .And.InnerException!.Message.Should().Be(exceptionMessage);

        RepositoryMock.Verify(r => r
            .DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()), Times.Never);
    }
    
    [Fact]
    public void ShouldHandleRepositoryExceptionAndThrowServiceExceptionByIdTest() {
        //Arrange
        SetupMocksAndVariables();
        var exceptionMessage = "Message from repository";
        var beatId = Guid.NewGuid();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(beatId);

        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()))
            .ThrowsAsync(new Exception(exceptionMessage));
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.DetachTagFromBeat(DetachTagFromBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<DetachTagFromBeatException>().Result
            .And.InnerException!.Message.Should().Be(exceptionMessage);

        RepositoryMock.Verify(r => r
            .DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()), Times.Never);
    }
    
    [Fact]
    public void ShouldThrowServiceExceptionIfRepositoryDoNotUpdateDataByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        DetachTagFromBeatDto.Id = "TempId123";
        var beatId = Guid.NewGuid();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(beatId);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(false);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(false);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.DetachTagFromBeat(DetachTagFromBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<DetachTagFromBeatException>().Result
            .And.InnerException.Should().BeNull();

        RepositoryMock.Verify(r => r
            .DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()), Times.Never);
    }
    
    [Fact]
    public void ShouldThrowServiceExceptionIfRepositoryDoNotUpdateDataByIdTest() {
        //Arrange
        SetupMocksAndVariables();
        var beatId = Guid.NewGuid();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(beatId);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(false);
        
        RepositoryMock
            .Setup(rm => rm.DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(false);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.DetachTagFromBeat(DetachTagFromBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<DetachTagFromBeatException>().Result
            .And.InnerException.Should().BeNull();

        RepositoryMock.Verify(r => r
            .DeleteTagFromBeatByTempIdAsync(It.IsAny<NnaTagInBeat>()), Times.Never);
    }
}