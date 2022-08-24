using FluentAssertions;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Entities;
using NNA.Domain.Exceptions.Editor;
using Xunit;

namespace NNA.Tests.EditorHubServiceTests; 
public class CreateBeatTests : BaseHubServiceTests {
    // ReSharper disable once InconsistentNaming
    private Guid userId { get; set; }
    // ReSharper disable once InconsistentNaming
    private CreateBeatDto beatDto { get; set; }
    private Beat NewBeat { get; set; }
        
    private void SetupMocksAndVariables() {
        SetupConstructorMocks();
        userId = Guid.NewGuid();
        beatDto = new CreateBeatDto {
            Order = 0,
            DmoId = Guid.NewGuid().ToString(),
            TempId = Guid.NewGuid().ToString()
        };
        NewBeat = new Beat();
    }
        
    [Fact]
    public void ShouldThrowWithInvalidEntryParamsTest() {
        //Arrange
        SetupMocksAndVariables();
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act1 = async () => await Subject.CreateBeat(null, userId);
        Func<Task> act2 = async () => await Subject.CreateBeat(beatDto, Guid.Empty);

        //Assert
        act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(beatDto));
        act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(userId));
    }
        
    [Fact]
    public async Task ShouldSetUserIdBeforeRepositoryIsCalledTest() {
        //Arrange
        SetupMocksAndVariables();
        MapperMock.Setup(m => m.Map<Beat>(beatDto)).Returns(NewBeat);
        RepositoryMock.Setup(rm => rm.InsertNewBeatAsync(NewBeat)).ReturnsAsync(true);
        Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        Func<Task> act = async () => await Subject.CreateBeat(beatDto, userId);
        await act.Invoke();

        //Assert
        NewBeat.UserId.Should().Be(userId);
    }
        
        
    [Fact]
    public void ShouldHandleRepositoryExceptionTest() {
        //Arrange
        SetupMocksAndVariables();
        var repositoryExceptionMessage = "some message from repository";
        MapperMock.Setup(m => m.Map<Beat>(beatDto)).Returns(NewBeat);
        RepositoryMock.Setup(rm => rm.InsertNewBeatAsync(NewBeat))
            .ThrowsAsync(new Exception(repositoryExceptionMessage));
        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await subject.CreateBeat(beatDto, userId);

        //Assert
        // ReSharper disable once PossibleNullReferenceException
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<InsertNewBeatException>().Result
            .And.InnerException.Message.Should().Be(repositoryExceptionMessage);
    }
        
    [Fact]
    public void ShouldThrowIfDmoWasNotUpdatedTest() {
        //Arrange
        SetupMocksAndVariables();
        MapperMock.Setup(m => m.Map<Beat>(beatDto)).Returns(NewBeat);
        RepositoryMock.Setup(rm => rm.InsertNewBeatAsync(NewBeat)).ReturnsAsync(false);
        var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

        //Act
        async Task Act() => await subject.CreateBeat(beatDto, userId);

        //Assert
        FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<InsertNewBeatException>().Result
            .And.InnerException.Should().BeNull();
    }
        
}