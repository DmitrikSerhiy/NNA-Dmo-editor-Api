using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.TagsInBeats;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests;

public sealed class AttachTagToBeatTests : BaseHubServiceTests {
    private AttachTagToBeatDto AttachTagToBeatDto { get; set; } = null!; 
    private Guid UserId { get; set; }
    
    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        AttachTagToBeatDto = new AttachTagToBeatDto {
            Id = Guid.NewGuid().ToString(),
            BeatId = Guid.NewGuid().ToString(),
            DmoId = Guid.NewGuid().ToString(),
            TagId = Guid.NewGuid().ToString()
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
        Func<Task> act1 = async () => await Subject.AttachTagToBeat(null!, UserId);
        Func<Task> act2 = async () => await Subject.AttachTagToBeat(AttachTagToBeatDto, Guid.Empty);
#pragma warning restore CS0612
        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be("attachTagToBeatDto");
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
            .Setup(rm => rm.CreateTagInBeatAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.AttachTagToBeat(AttachTagToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task ShouldLoadBeatByTempIdTest() {
        //Arrange
        SetupMocksAndVariables();
        AttachTagToBeatDto.BeatId = "TempId123";
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.CreateTagInBeatAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.AttachTagToBeat(AttachTagToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r.LoadBeatIdByTempId(Guid.Parse(AttachTagToBeatDto.DmoId), AttachTagToBeatDto.BeatId, UserId), Times.Once);
    }
    
    [Fact]
    public async Task ShouldCallRepositoryMethodToAttachTagWithDataProvidedInDtoTest() {
        //Arrange
        SetupMocksAndVariables();
        RepositoryMock
            .Setup(rm => rm.LoadBeatIdByTempId(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Verifiable();
        
        RepositoryMock
            .Setup(rm => rm.CreateTagInBeatAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(true);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        Func<Task> act = async () => await Subject.AttachTagToBeat(AttachTagToBeatDto, UserId);
        await act.Invoke();

        //Assert
        RepositoryMock.Verify(r => r
            .CreateTagInBeatAsync(It.Is<NnaTagInBeat>(beat => 
                beat.TagId == Guid.Parse(AttachTagToBeatDto.TagId) &&
                beat.BeatId == Guid.Parse(AttachTagToBeatDto.BeatId) &&
                beat.TempId == AttachTagToBeatDto.Id)
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
            .Setup(rm => rm.CreateTagInBeatAsync(It.IsAny<NnaTagInBeat>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.AttachTagToBeat(AttachTagToBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<AttachTagToBeatException>().Result
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
            .Setup(rm => rm.CreateTagInBeatAsync(It.IsAny<NnaTagInBeat>()))
            .ReturnsAsync(false);

        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
        //Act
        async Task Act() => await Subject.AttachTagToBeat(AttachTagToBeatDto, UserId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<AttachTagToBeatException>().Result
            .And.InnerException.Should().BeNull();
    }
    
    
}