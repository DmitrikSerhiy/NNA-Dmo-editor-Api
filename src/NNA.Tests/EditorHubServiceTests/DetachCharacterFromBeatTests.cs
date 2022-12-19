using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.CharactersInBeats;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public sealed class DetachCharacterFromBeatTests: BaseHubServiceTests  {
    
    private DetachCharacterToBeatDto DetachCharacterToBeatDto { get; set; } = null!; 

    private Guid UserId { get; set; }
    
    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        DetachCharacterToBeatDto = new DetachCharacterToBeatDto {
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
        Func<Task> act1 = async () => await Subject.DetachCharacterFromBeat(null!, UserId);
        Func<Task> act2 = async () => await Subject.DetachCharacterFromBeat(DetachCharacterToBeatDto, Guid.Empty);
#pragma warning restore CS0612
        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be("characterToDetachDto");
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
            .Setup(rm => rm.DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);
        
        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task ShouldLoadBeatByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        DetachCharacterToBeatDto.BeatId = "TempId123";
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);
        
        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r.LoadBeatIdByTempId(Guid.Parse(DetachCharacterToBeatDto.DmoId), DetachCharacterToBeatDto.BeatId, UserId), Times.Once);
    }
    
    [Fact]
    public async Task ShouldCallRepositoryMethodToDetachCharacterWithDataProvidedInDtoByIdTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);
        
        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r
            .DeleteCharacterFromBeatByIdAsync(It.Is<NnaMovieCharacterInBeat>(beat => 
                beat.Id == Guid.Parse(DetachCharacterToBeatDto.Id) &&
                beat.BeatId == Guid.Parse(DetachCharacterToBeatDto.BeatId) &&
                beat.TempId == null)
            ), Times.Once);
        
        RepositoryMock.Verify(r => r
            .DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()), Times.Never);
    }
    
    [Fact]
    public async Task ShouldCallRepositoryMethodToDetachCharacterWithDataProvidedInDtoByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        DetachCharacterToBeatDto.Id = "TempId123";
        var beatId = Guid.NewGuid();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(beatId);
        
        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);
        
        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.DetachCharacterFromBeat(DetachCharacterToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r
            .DeleteCharacterFromBeatByTempIdAsync(It.Is<NnaMovieCharacterInBeat>(beat => 
                beat.TempId == DetachCharacterToBeatDto.Id &&
                beat.BeatId == Guid.Parse(DetachCharacterToBeatDto.BeatId))
            ), Times.Once);
        
        RepositoryMock.Verify(r => r
            .DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()), Times.Never);
    }
    
    [Fact]
    public void ShouldHandleRepositoryExceptionAndThrowServiceExceptionByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        DetachCharacterToBeatDto.Id = "TempId123";
        var exceptionMessage = "Message from repository";
        var beatId = Guid.NewGuid();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(beatId);
        
        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);

        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.DetachCharacterFromBeat(DetachCharacterToBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<RemoveCharacterFromBeatException>().Result
            .And.InnerException!.Message.Should().Be(exceptionMessage);

        RepositoryMock.Verify(r => r
            .DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()), Times.Never);
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
            .Setup(rm => rm.DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.DetachCharacterFromBeat(DetachCharacterToBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<RemoveCharacterFromBeatException>().Result
            .And.InnerException!.Message.Should().Be(exceptionMessage);

        RepositoryMock.Verify(r => r
            .DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()), Times.Never);
    }
    
    [Fact]
    public void ShouldThrowServiceExceptionIfRepositoryDoNotUpdateDataByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        DetachCharacterToBeatDto.Id = "TempId123";
        var beatId = Guid.NewGuid();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(beatId);
        
        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(false);

        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(false);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.DetachCharacterFromBeat(DetachCharacterToBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<RemoveCharacterFromBeatException>().Result
            .And.InnerException.Should().BeNull();

        RepositoryMock.Verify(r => r
            .DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()), Times.Never);
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
            .Setup(rm => rm.DeleteCharacterFromBeatByIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(false);

        RepositoryMock
            .Setup(rm => rm.DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(false);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.DetachCharacterFromBeat(DetachCharacterToBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<RemoveCharacterFromBeatException>().Result
            .And.InnerException.Should().BeNull();

        RepositoryMock.Verify(r => r
            .DeleteCharacterFromBeatByTempIdAsync(It.IsAny<NnaMovieCharacterInBeat>()), Times.Never);
    }
}