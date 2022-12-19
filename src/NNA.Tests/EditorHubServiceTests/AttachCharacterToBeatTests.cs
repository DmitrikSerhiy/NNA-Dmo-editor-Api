using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.CharactersInBeats;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public sealed class AttachCharacterToBeatTests : BaseHubServiceTests {
    private AttachCharacterToBeatDto AttachCharacterToBeatDto { get; set; } = null!; 
        private Guid UserId { get; set; }
    
    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        AttachCharacterToBeatDto = new AttachCharacterToBeatDto {
            Id = Guid.NewGuid().ToString(),
            BeatId = Guid.NewGuid().ToString(),
            CharacterId = Guid.NewGuid().ToString(),
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
        Func<Task> act1 = async () => await Subject.AttachCharacterToBeat(null!, UserId);
        Func<Task> act2 = async () => await Subject.AttachCharacterToBeat(AttachCharacterToBeatDto, Guid.Empty);
#pragma warning restore CS0612
        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be("characterToAttachDto");
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
            .Setup(rm => rm.CreateCharacterInBeatAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.AttachCharacterToBeat(AttachCharacterToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task ShouldLoadBeatByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        AttachCharacterToBeatDto.BeatId = "TempId123";
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.CreateCharacterInBeatAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.AttachCharacterToBeat(AttachCharacterToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r.LoadBeatIdByTempId(Guid.Parse(AttachCharacterToBeatDto.DmoId), AttachCharacterToBeatDto.BeatId, UserId), Times.Once);
    }
    
    [Fact]
    public async Task ShouldCallRepositoryMethodToAttachCharacterWithDataProvidedInDtoTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.CreateCharacterInBeatAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.AttachCharacterToBeat(AttachCharacterToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r
            .CreateCharacterInBeatAsync(It.Is<NnaMovieCharacterInBeat>(beat => 
                beat.CharacterId == Guid.Parse(AttachCharacterToBeatDto.CharacterId) &&
                beat.BeatId == Guid.Parse(AttachCharacterToBeatDto.BeatId) &&
                beat.TempId == AttachCharacterToBeatDto.Id)
            ), Times.Once);
    }
    
    
    [Fact]
    public void ShouldHandleRepositoryExceptionAndThrowServiceExceptionTestTest() {
        //Arrange
        var exceptionMessage = "Exception from repository";
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();

        RepositoryMock
            .Setup(rm => rm.CreateCharacterInBeatAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.AttachCharacterToBeat(AttachCharacterToBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<AttachCharacterToBeatException>().Result
            .And.InnerException!.Message.Should().StartWith(exceptionMessage);
    }
    
    [Fact]
    public void ShouldThrowServiceExceptionIfRepositoryDoNotUpdateDataTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.CreateCharacterInBeatAsync(It.IsAny<NnaMovieCharacterInBeat>()))
            .ReturnsAsync(false);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.AttachCharacterToBeat(AttachCharacterToBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<AttachCharacterToBeatException>().Result
            .And.InnerException.Should().BeNull();
    }
}