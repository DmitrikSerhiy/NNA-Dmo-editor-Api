using System;
using System.Threading.Tasks;
using API.Features.Editor.Services;
using FluentAssertions;
using Model.DTOs.Editor;
using Model.Entities;
using Model.Exceptions.Editor;
using Moq;
using Xunit;

namespace Tests.EditorHubServiceTests {
    public class UpdateBeatTests : BaseHubServiceTests {
        // ReSharper disable once InconsistentNaming
        private Guid userId { get; set; }

        // ReSharper disable once InconsistentNaming
        private UpdateBeatDto update { get; set; }
        private Beat BeatToUpdate { get; set; }

        private void SetupMocksAndVariables() {
            SetupConstructorMocks();
            userId = Guid.NewGuid();
            update = new UpdateBeatDto {
                Text = "beat description",
                BeatId = Guid.NewGuid().ToString(),
                Time = new UpdateBeatTimeDto {
                    Hours = 0,
                    Minutes = 10,
                    Seconds = 10
                }
            };
            BeatToUpdate = new Beat();
        }
        
        
        [Fact]
        public void ShouldThrowWithInvalidEntryParamsTest() {
            //Arrange
            SetupMocksAndVariables();
            Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            Func<Task> act1 = async () => await Subject.UpdateBeat(null, userId);
            Func<Task> act2 = async () => await Subject.UpdateBeat(update, Guid.Empty);

            //Assert
            act1.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(update));
            act2.Should().ThrowAsync<ArgumentNullException>().Result.And.ParamName.Should().Be(nameof(userId));
        }
        
        
        [Fact]
        public async Task ShouldSetUserIdBeforeRepositoryIsCalledTest() {
            //Arrange
            SetupMocksAndVariables();
            MapperMock.Setup(m => m.Map<Beat>(update)).Returns(BeatToUpdate);
            RepositoryMock.Setup(rm => rm.UpdateBeatByIdAsync(It.IsAny<Beat>(), It.IsAny<Guid>())).ReturnsAsync(true);
            Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);
        
            //Act
            Func<Task> act = async () => await Subject.UpdateBeat(update, userId);
            await act.Invoke();
        
            //Assert
            BeatToUpdate.UserId.Should().Be(userId);
        }
        
                
        [Fact]
        public async Task ShouldUpdateBeatByIdIfBeatIdIsValidGuidTest() {
            //Arrange
            SetupMocksAndVariables();
            MapperMock.Setup(m => m.Map<Beat>(update)).Returns(BeatToUpdate);
            RepositoryMock.Setup(rm => rm.UpdateBeatByIdAsync(It.IsAny<Beat>(), It.IsAny<Guid>())).ReturnsAsync(true);
            RepositoryMock.Setup(rm => rm.UpdateBeatByTempIdAsync(It.IsAny<Beat>(), It.IsAny<string>())).Verifiable();
            Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            Func<Task> act = async () => await Subject.UpdateBeat(update, userId);
            await act.Invoke();

            //Assert
            RepositoryMock.Verify(sbj => sbj.UpdateBeatByTempIdAsync(It.IsAny<Beat>(), It.IsAny<string>()), Times.Never());
            RepositoryMock.Verify(sbj => sbj.UpdateBeatByIdAsync(It.IsAny<Beat>(), It.IsAny<Guid>()), Times.Once());
        }
        
        [Fact]
        public async Task ShouldUpdateBeatByTempIdIfBeatIdIsStringTest() {
            //Arrange
            SetupMocksAndVariables();
            update.BeatId = "TempBeatId";
            MapperMock.Setup(m => m.Map<Beat>(update)).Returns(BeatToUpdate);
            RepositoryMock.Setup(rm => rm.UpdateBeatByIdAsync(It.IsAny<Beat>(), It.IsAny<Guid>())).Verifiable();
            RepositoryMock.Setup(rm => rm.UpdateBeatByTempIdAsync(It.IsAny<Beat>(), It.IsAny<string>())).ReturnsAsync(true);
            Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            Func<Task> act = async () => await Subject.UpdateBeat(update, userId);
            await act.Invoke();

            //Assert
            RepositoryMock.Verify(sbj => sbj.UpdateBeatByTempIdAsync(It.IsAny<Beat>(), It.IsAny<string>()), Times.Once());
            RepositoryMock.Verify(sbj => sbj.UpdateBeatByIdAsync(It.IsAny<Beat>(), It.IsAny<Guid>()), Times.Never());
        }
        
        [Fact]
        public void ShouldHandleRepositoryExceptionTest() {
            //Arrange
            SetupMocksAndVariables();
            var repositoryExceptionMessage = "some message from repository";
            MapperMock.Setup(m => m.Map<Beat>(update)).Returns(BeatToUpdate);
            RepositoryMock.Setup(rm => rm.UpdateBeatByIdAsync(It.IsAny<Beat>(), It.IsAny<Guid>()))
                .ThrowsAsync(new Exception(repositoryExceptionMessage));
            Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            async Task Act() => await Subject.UpdateBeat(update, userId);

            //Assert
            // ReSharper disable once PossibleNullReferenceException
            FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<UpdateBeatException>().Result
                .And.InnerException.Message.Should().Be(repositoryExceptionMessage);
        }
        
        
        [Fact]
        public void ShouldThrowIfDmoWasNotUpdatedTest() {
            //Arrange
            
            SetupMocksAndVariables();
            MapperMock.Setup(m => m.Map<Beat>(update)).Returns(BeatToUpdate);
            RepositoryMock.Setup(rm => rm.UpdateBeatByIdAsync(It.IsAny<Beat>(), It.IsAny<Guid>())).ReturnsAsync(false);
            var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            async Task Act() => await subject.UpdateBeat(update, userId);

            //Assert
            FluentActions.Awaiting(Act).Should().ThrowExactlyAsync<UpdateBeatException>().Result
                .And.InnerException.Should().BeNull();
        }
    }
}