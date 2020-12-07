using API.Features.Editor.Services;
using AutoMapper;
using FluentAssertions;
using Model.DTOs.Editor;
using Model.Interfaces.Repositories;
using Moq;
using System;
using System.Threading.Tasks;
using Model.Entities;
using Model.Exceptions.Editor;
using Xunit;

namespace Tests.EditorHubServiceTests {
    public class LoadShortDmoTests {

        private Mock<IEditorRepository> RepositoryMock { get; set; }
        private Mock<IMapper> MapperMock { get; set; }
        private EditorService Subject { get; set; }

        // ReSharper disable once InconsistentNaming
        private Guid userId { get; set; }

        // ReSharper disable once InconsistentNaming
        private LoadShortDmoDto dmoDto { get; set; }
        private Dmo InitialDmo { get; set; }

        //todo: add test which check all types in model assembly which ends on Dto is derived from BaseDto class
        //todo: same for entity
        //todo: assembly.Should().NotReference(otherAssembly); and vice-versa

        private void SetupMocksAndVariables() {
            RepositoryMock = new Mock<IEditorRepository>();
            MapperMock = new Mock<IMapper>();

            userId = Guid.NewGuid();
            dmoDto = new LoadShortDmoDto();
            InitialDmo = new Dmo();
        }

        [Fact]
        public void ShouldThrowWithInvalidEntryParamsTest() {
            //Arrange
            SetupMocksAndVariables();
            Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            Func<Task> act1 = async () => await Subject.LoadShortDmo(null, userId);
            Func<Task> act2 = async () => await Subject.LoadShortDmo(dmoDto, Guid.Empty);

            //Assert
            act1.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(nameof(dmoDto));
            act2.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(nameof(userId));
        }

        [Fact]
        public async Task ShouldSetUserIdBeforeRepositoryIsCalledTest() {
            //Arrange
            SetupMocksAndVariables();
            MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
            RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId)).ReturnsAsync(new Dmo());
            Subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            await Subject.LoadShortDmo(dmoDto, userId);

            //Assert
            InitialDmo.NnaUserId.Should().Be(userId);
        }

        [Fact]
        public void ShouldHandleRepositoryExceptionTest() {
            //Arrange
            SetupMocksAndVariables();
            var repositoryExceptionMessage = "some message from repository";
            MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
            RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId))
                .ThrowsAsync(new Exception(repositoryExceptionMessage));
            var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            async Task Act() => await subject.LoadShortDmo(dmoDto, userId);

            //Assert
            // ReSharper disable once PossibleNullReferenceException
            FluentActions.Awaiting(Act).Should().ThrowExactly<LoadShortDmoException>()
                .And.InnerException.Message.Should().Be(repositoryExceptionMessage);
        }

        [Fact]
        public void ShouldThrowIfDmoWasNotFoundTest() {
            //Arrange
            SetupMocksAndVariables();
            static Dmo Dmo() => null;
            MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
            RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId)).ReturnsAsync(Dmo);

            var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            async Task Act() => await subject.LoadShortDmo(dmoDto, userId);

            //Assert
            FluentActions.Awaiting(Act).Should().ThrowExactly<LoadShortDmoException>()
                .And.InnerException.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnDmoTest() {
            //Arrange
            SetupMocksAndVariables();
            var dmo = new Dmo();
            var searchedDmoDto = new LoadedShortDmoDto();
            MapperMock.Setup(m => m.Map<Dmo>(dmoDto)).Returns(InitialDmo);
            RepositoryMock.Setup(rm => rm.LoadShortDmoAsync(InitialDmo.Id, userId)).ReturnsAsync(dmo);
            MapperMock.Setup(m => m.Map<LoadedShortDmoDto>(dmo)).Returns(searchedDmoDto);
            var subject = new EditorService(RepositoryMock.Object, MapperMock.Object);

            //Act
            Func<Task> act = async () => await subject.LoadShortDmo(dmoDto, userId);

            //Assert
            act.Should().NotThrow();
            MapperMock.Verify(mm => mm.Map<LoadedShortDmoDto>(dmo), Times.Once);
        }
    }
}
