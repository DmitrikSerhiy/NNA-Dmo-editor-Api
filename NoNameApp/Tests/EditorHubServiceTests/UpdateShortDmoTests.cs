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
    public class UpdateShortDmoTests : BaseHubServiceTests {
        // ReSharper disable once InconsistentNaming
        private Guid userId { get; set; }
        // ReSharper disable once InconsistentNaming
        private UpdateShortDmoDto dmoDto { get; set; }
        private Dmo InitialDmo { get; set; }

        private void SetupMocksAndVariables() {
            SetupConstructorMocks();
            userId = Guid.NewGuid();
            dmoDto = new UpdateShortDmoDto();
            InitialDmo = new Dmo();
        }


        [Fact]
        public void ShouldThrowWithInvalidEntryParamsTest() {
            //Arrange
            SetupMocksAndVariables();
            Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            Func<Task> act1 = async () => await Subject.UpdateShortDmo(null, userId);
            Func<Task> act2 = async () => await Subject.UpdateShortDmo(dmoDto, Guid.Empty);

            //Assert
            act1.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(nameof(dmoDto));
            act2.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(nameof(userId));
        }

        [Fact]
        public async Task ShouldSetUserIdBeforeRepositoryIsCalledTest() {
            //Arrange
            SetupMocksAndVariables();
            MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
            RepositoryMock.Setup(rm => rm.UpdateShortDmoAsync(InitialDmo)).ReturnsAsync(true);
            Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            await Subject.UpdateShortDmo(dmoDto, userId);

            //Assert
            InitialDmo.NnaUserId.Should().Be(userId);
        }

        [Fact]
        public void ShouldHandleRepositoryExceptionTest() {
            //Arrange
            SetupMocksAndVariables();
            var repositoryExceptionMessage = "some message from repository";
            MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
            RepositoryMock.Setup(rm => rm.UpdateShortDmoAsync(InitialDmo))
                .ThrowsAsync(new Exception(repositoryExceptionMessage));
            var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            async Task Act() => await subject.UpdateShortDmo(dmoDto, userId);

            //Assert
            // ReSharper disable once PossibleNullReferenceException
            FluentActions.Awaiting(Act).Should().ThrowExactly<UpdateShortDmoException>()
                .And.InnerException.Message.Should().Be(repositoryExceptionMessage);
        }

        [Fact]
        public void ShouldThrowIfDmoWasNotUpdatedTest() {
            //Arrange
            SetupMocksAndVariables();
            MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
            RepositoryMock.Setup(rm => rm.UpdateShortDmoAsync(InitialDmo)).ReturnsAsync(false);

            var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            async Task Act() => await subject.UpdateShortDmo(dmoDto, userId);

            //Assert
            FluentActions.Awaiting(Act).Should().ThrowExactly<UpdateShortDmoException>()
                .And.InnerException.Should().BeNull();
        }

    }
}
